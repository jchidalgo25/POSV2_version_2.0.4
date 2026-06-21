using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace POS.Models.TarjeEmpresa
{
    public class ClsTarjetaEmpresaPagos : POS.Models.Abstract.Sale, Interfaces.ISale
    {
        public string MsgError { get; set; } 
        public decimal Total { get; set; }
        public decimal Interes { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal SaldoFinal { get; set; }
        public decimal SaldoPorProcesar { get; set; }
        public pos_customer Customer { get; set; }        
        //public string Establecimiento { get; set; }
        //public string PuntoEmision { get; set; }
        public string ClienteAx { get; set; } 
        public string Usuario { get; set; }
        public string IdCaja { get; set; }
        public string JournalNum { get; set; }
        public byte Procesado { get; set; }
        public byte TipoTransaccion { get; set; }
        public string CodigoGiftCard { get; set; }
        public core_tarjetacreditointerno TarjeCredInterno { get; set; }
        public decimal Cambio { get; set; }
        public string Recibo { get; set; }
        public string XML { get; set; }
        public string PDF { get; set; }
        public string pieRecibo { get; set; }

        private DateTime _fechaTrans;
        public DateTime FechaTrans
        {
            get { return _fechaTrans; }
            set { _fechaTrans = value; }
        }

        private string _atendidoPor = string.Empty;
        public string AtendidoPor
        {
            get { return _atendidoPor; }
            set { _atendidoPor = value; }
        }
         

        /// <summary>
        /// Sum of both purchase and current values, it's a reference of the new value for the giftcard after 
        /// the transaccion will be completed
        /// </summary>
        private decimal _futureValue = 0M;
        public decimal FutureValue { get { return _futureValue; } set { _futureValue = value; } }

        BindingList<Pago> _pagos = new BindingList<Pago>();

        public BindingList<Pago> Pagos
        {
            get { return _pagos; }
            protected set { _pagos = value; }
        }

        public decimal GetPagos()
        {
            return this.Pagos.Sum(x => x.Valor);
        }

        public override decimal GetBase0()
        {
            return this.Total;
        }

        public override decimal GetTotal()
        {
            return this.SaldoFinal;
        }        
        public decimal GetTotalPagar()
        {
            return this.Total - this.Cambio;
        }

        public override void CalcularPagos()
        {
            foreach (var p in this.Pagos)
            {
                p.calcularTotal();
            }
        }

        public void AgregarDescuentoPagoCompraGratis(decimal valor, string codigo)
        {
            // Implementación vacía para cumplir la interfaz ISale
            throw new NotImplementedException();
        }

        public override string GetNumeroFactura()
        {
            //Ajuste realizado porque tabla POS_VOUCHER admite un tope de 20 caracteres en el campo factura
            return ((TarjeCredInterno == null) ? string.Empty : (TarjeCredInterno.codigo.Trim().Length > 20 ? (TarjeCredInterno.codigo.Trim().Substring(0, 20)) : TarjeCredInterno.codigo.Trim()));
        }

        public override string GetNumeroFacturaEnmascarado()
        {
            string nroFactura = GetNumeroFactura();
            //Enmascarar tarjeta
            if (nroFactura.Length > 6)
            {
                string first = nroFactura.Substring(0, 3);
                string last = nroFactura.Substring(nroFactura.Length - 3, 3);
                nroFactura = first + string.Empty.PadRight(nroFactura.Length - 6, 'X') + last;
            }

            return nroFactura;
        }

        public override void AgregarPagoTarjetaCredito(decimal valor, string banco, string nombre, string marca, string tipoPos, string numBin, string bin_descripcion)
        {
            var pago = this.AgregarPago("T. CREDITO", valor);
            pago.Pagos.Add(new PagoTarjetaCredito() { Banco = banco, Codigo = nombre + " - " + marca, Nombre = nombre, Marca = marca, Valor = valor, TipoPos = tipoPos, NumeroBin = numBin, BinDescripcion = bin_descripcion });
            pago.calcularTotal();
        }
   
        public void AgregarPagoCheque(decimal valor, string banco, string numero, string cuenta)
        {
            var pago = this.AgregarPago("CHEQUE", valor);
            pago.Pagos.Add(new PagoCheque() { Banco = banco, Codigo = banco + " - " + cuenta + " - " + numero, Numero = numero, Cuenta = cuenta, Valor = valor });
            pago.calcularTotal();
        }

        public void AgregarPagoEfectivo(decimal valor)
        {
            if (this.Pagos.Any(x => x.Descripcion == "EFECTIVO"))
            {
                var pago = this.Pagos.Single(x => x.Descripcion == "EFECTIVO");
                pago.Valor = valor;
            }
            else 
            {
                this.Pagos.Add(new Pago() { Descripcion = "EFECTIVO", Valor = valor });
            }
        }
        public decimal GetPagosNoEfectivo()
        {
            return this.Pagos.Where(x => x.Descripcion != "EFECTIVO").Sum(x => x.Valor);
        }

        public bool ValidarPagos(out decimal cambio,decimal totalpagar)
        {
            cambio = 0m;
            this.Cambio = 0m;
            this.CalcularPagos();
            if (this.GetPagosNoEfectivo() > totalpagar)//this.GetTotal())
            {
                return false;
            }
            var saldo = totalpagar; //this.GetTotal();
            foreach (var p in this.Pagos.Where(x => x.Descripcion != "EFECTIVO"))
            {
                saldo = saldo - p.Valor;
            }
            var pago_efectivo = this.Pagos.Where(x => x.Descripcion == "EFECTIVO").Sum(x => x.Valor);
            saldo = saldo - pago_efectivo;
            if (saldo > 0)
            {
                return false;
            }
            if (saldo < 0)
            {
                cambio = saldo * -1;
            }
            this.Cambio = cambio;
            if (cambio == pago_efectivo && pago_efectivo > 0)
            {
                return false;
            }
            return true;
        }

        public string GetNumeroFacturaCompleto()
        {
            return ((TarjeCredInterno == null) ? string.Empty : "G-" + TarjeCredInterno.codigo.Trim());
        }

        Pago AgregarPago(string tipo, decimal valor)
        {
            if (this.Pagos.Any(x => x.Descripcion == tipo))
            {
                var pago = this.Pagos.Single(x => x.Descripcion == tipo);
                pago.Valor = valor;
                return pago;
            }
            else
            {
                var nuevo = new Pago() { Descripcion = tipo, Valor = valor };
                this.Pagos.Add(nuevo);
                return nuevo;
            }
        }

        public bool Validar()
        {
            var result = false;
            var cambio = 0M;
            result = this.Pagos.Count > 0 && this.ValidarPagos(out cambio,this.GetTotalPagar());
            return result;
        }

        public bool Grabar()
        {
            bool response = true;
            using (var db = new POSEntities())
            {
                using (System.Data.Entity.DbContextTransaction dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    { 
                        FechaTrans = DateTime.Now;
                        var cobroTarjeEmpre = new TblCobroCreditoEmp();
                        cobroTarjeEmpre.Cliente_Ax = Customer.ACCOUNTNUM;
                        cobroTarjeEmpre.CodigoGiftcard = TarjeCredInterno.codigo;
                        cobroTarjeEmpre.Establecimiento = Control.Common.GlobalParameters.Establecimiento;
                        cobroTarjeEmpre.PuntoEmision = Control.Common.GlobalParameters.PuntoEmision;
                        cobroTarjeEmpre.FechaCreacion = FechaTrans;
                        cobroTarjeEmpre.FechaModificacion = FechaTrans;
                        cobroTarjeEmpre.IdCaja = Program.ID_Caja_POS;
                        cobroTarjeEmpre.Razon_Social = Customer.NAME;
                        cobroTarjeEmpre.TipoTransaccion = TipoTransaccion;
                        cobroTarjeEmpre.Procesado = Procesado;
                        cobroTarjeEmpre.Total = GetTotalPagar();
                        cobroTarjeEmpre.Usuario = Control.Common.GlobalParameters.UserObj.username;

                        foreach (var p in this.Pagos)
                        {
                            var pago = new TblCobroCreditoEmpPago();
                            pago.FechaCreacion = cobroTarjeEmpre.FechaCreacion;
                            if (p.Pagos.Count > 0)
                            {
                                foreach (var subp in p.Pagos)
                                {
                                    pago = new TblCobroCreditoEmpPago();
                                    pago.FechaCreacion = cobroTarjeEmpre.FechaCreacion;
                                    pago.Tipo_Id = p.Descripcion;
                                    pago.Valor = subp.Valor;
                                    //Solo si es efectivo se resta el cambio.
                                    //if (subp.Codigo == "EFECTIVO")
                                    //{
                                    //    pago.Datos = "Efectivo";
                                    //    pago.Valor = pago.Valor - this.Cambio;
                                    //}
                                    if (subp is PagoTarjetaCredito)
                                    {
                                        var pTarjeta = subp as PagoTarjetaCredito;
                                        pago.Datos = pTarjeta.Nombre + "-" + pTarjeta.Marca + "-" + pTarjeta.Codigo;
                                        pago.Voucher = pTarjeta.TipoPos;
                                    }
                                    else if (subp is PagoCheque)
                                    {
                                        var pCheque = subp as PagoCheque;
                                        pago.Datos = pCheque.Banco + "-" + pCheque.Cuenta + "-" + pCheque.Numero;
                                    }
                                    cobroTarjeEmpre.TblCobroCreditoEmpPago.Add(pago);
                                }
                            }
                            else
                            {
                                //Efectivo no tiene subpagos
                                pago.Tipo_Id = p.Descripcion;
                                pago.Valor = p.Valor - this.Cambio;
                                pago.Datos = "Efectivo";
                            }
                            cobroTarjeEmpre.TblCobroCreditoEmpPago.Add(pago);
                        }

                        db.TblCobroCreditoEmp.Add(cobroTarjeEmpre);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.TarjeEmpresa.ClsTarjetaEmpresaPagos", "Grabar", "Imposible grabar en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                        response = false;
                        MsgError = ex.ToString();

                        var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        Properties.Settings.Default.MAILERROR_MOTIVO,
                        String.Format("Establecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroId: {4} \n\nDatos Excepcion ------------\nClass: {5} \nMethod: {6} \nMessage: {7} \nStackTrace: {8}",
                                      Control.Common.GlobalParameters.Establecimiento,
                                      Control.Common.GlobalParameters.PuntoEmision,
                                      Control.Common.GlobalParameters.IpMaquina,
                                      Control.Common.GlobalParameters.UserObj.nombres,
                                      Control.Common.GlobalParameters.UserObj.username,
                                      "POS.Control.TarjeEmpre.TarjeEmprePagos",
                                      "grabar",
                                      Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                      ex.StackTrace),
                        false,
                        String.Empty);

                        if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Tarjeta Empresarial", "Grabar", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                        }
                        dbContextTransaction.Rollback();
                        Control.Common.WinForm.ShowMessage("No se pudo grabar la transacción en este momento, esto puede deberse a una breve interrupción en la comunicación, inténtelo nuevamente en unos momentos");
                    }
                }
            }
            return response;
        }

        public bool ImprimirRecibo()
        {
            bool response = true;
            try
            {  
                var receiptModel = new Models.PrinterRecipes.ReceiptTarjetaEmpresarial();
              
                //receiptModel.CambioTransacccion = Cambio;         

                receiptModel.CajeroNombre = (string.IsNullOrEmpty(AtendidoPor)) ? Control.Common.GlobalParameters.UserObj.nombres : AtendidoPor;
                receiptModel.ClienteDireccion = Customer.STREET;
                receiptModel.ClienteIdentificacion = Customer.ACCOUNTNUM;
                receiptModel.ClienteNombre = Customer.NAME;
                receiptModel.ClienteTelefono = Customer.PHONE;
                receiptModel.ConceptoTransaccion = "Pago de Tarjeta Empresarial";
                receiptModel.FechaEmision = FechaTrans.ToString("dd/MM/yyyy HH:mm:ss");
                receiptModel.CodigoTarjeta = TarjeCredInterno.codigo;

                //-------DIRECCION ESTABLECIMIENTO------------------------------------

                var sub1 = "";
                var sub2 = "";
                var pos = new POSEntities();

                if (Control.Common.GlobalParameters.EstablecimientoDireccion.Length > 20)
                {
                    int largo = Control.Common.GlobalParameters.EstablecimientoDireccion.Length;
                    sub1 = Environment.NewLine + Control.Common.GlobalParameters.EstablecimientoDireccion.Substring(0, 20);
                    sub2 = Environment.NewLine + Control.Common.GlobalParameters.EstablecimientoDireccion.Substring(20, Control.Common.GlobalParameters.EstablecimientoDireccion.Length - 20);
                }
                receiptModel.Oficina = Control.Common.GlobalParameters.EstablecimientoDireccion + sub1 + sub2;
                //-----------------------------------------------------------------
                receiptModel.Telefono = Control.Common.GlobalParameters.EstablecimientoTelefono;


                receiptModel.Total = (Total-Cambio).ToString("N2");  

                Control.Common.Printer.ImprimirPagoTarjetaEmpresarial(Recibo, receiptModel);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.TarjeEmpresa.ClsTarjetaEmpresaPagos", "ImprimirRecibo", "Imposible imprimir comprobante en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                response = false;
                MsgError = "No se pudo realizar la impresion del comprobante de la transacción realizada";
            }

            return response;
        }
    }
}
