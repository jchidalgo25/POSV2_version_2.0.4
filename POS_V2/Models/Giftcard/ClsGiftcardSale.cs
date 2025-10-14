using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Giftcard
{
    public class ClsGiftcardSale : POS.Models.Abstract.Sale, Interfaces.ISale
    {
        public string MsgError { get; set; }

        private int _cantidadVigencia = 0;
        public int CantidadVigencia { get { return _cantidadVigencia; } set { _cantidadVigencia = value; } }

        private Control.Common.Enum.TimeAdditionType _tipoAdicionTiempo;
        public Control.Common.Enum.TimeAdditionType TipoAdicionTiempo { get { return _tipoAdicionTiempo; } set { _tipoAdicionTiempo = value; } }
        public Control.Common.Enum.GiftcardSaleType GiftcardSaleType { get; set; }
        public pos_customer Customer { get; set; }
        public core_giftcard Giftcard { get; set; }

        public BindingList<core_giftcard> LstGiftcard { get; set; }

        private bool _confirmed = false;
        public bool Confirmed { get { return _confirmed; } set { _confirmed = value; } }

        private string _recibo;
        public string Recibo
        {
            get { return _recibo; }
            set { _recibo = value; }
        }

        private string _pieRecibo;
        public string PieRecibo
        {
            get { return _pieRecibo; }
            set { _pieRecibo = value; }
        }

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

        private decimal _purchaseValue = 0M;
        public decimal PurchaseValue { get { return _purchaseValue; } set { _purchaseValue = value; } }

        private decimal _currentValue = 0M;
        public decimal CurrentValue { get { return _currentValue; } set { _currentValue = value; } }

        private decimal _cambio = 0M;
        public decimal Cambio { get { return _cambio; } set { _cambio = value; } }

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
            decimal Value = 0M;
            if (LstGiftcard != null)
            {
                Value = LstGiftcard.Sum(x => x.monto);
            }
            return Value;// this.PurchaseValue;
        }

        public override decimal GetTotal()
        {
            decimal Value = 0M;
            if (LstGiftcard != null)
            {
                Value = (decimal)LstGiftcard.Sum(x => x.monto);
            }

            return Value;// this.PurchaseValue;
        }

        public override string GetNumeroFactura()
        {
            //Ajuste realizado porque tabla POS_VOUCHER admite un tope de 20 caracteres en el campo factura
            return ((Giftcard == null) ? string.Empty : (Giftcard.codigo.Trim().Length > 20 ? (Giftcard.codigo.Trim().Substring(0, 20)) : Giftcard.codigo.Trim()));
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

        public string GetNumeroFacturaCompleto()
        {
            return ((Giftcard == null) ? string.Empty : "G-" + Giftcard.codigo.Trim());
        }

        public decimal GetSaldoDisplay()
        {
            decimal Value=0M;
            if (LstGiftcard != null)
            {
                Value = (decimal)LstGiftcard.Sum(x => x.monto);
            }
                //var saldo = this.PurchaseValue - this.GetPagos();
            var saldo = Value - this.GetPagos();
            if (saldo <= 0)
            {
                return 0M;
            }
            else
            {
                return saldo;
            }
        }

        public override void CalcularPagos()
        {
            foreach (var p in this.Pagos)
            {
                p.calcularTotal();
            }
        }

        public decimal GetPagosNoEfectivo()
        {
            return this.Pagos.Where(x => x.Descripcion != "EFECTIVO").Sum(x => x.Valor);
        }

        public bool ValidarPagos(out decimal cambio)
        {
            cambio = 0m;
            this.Cambio = 0m;
            this.CalcularPagos();
            if (this.GetPagosNoEfectivo() > this.GetTotal())
            {
                return false;
            }
            var saldo = this.GetTotal();
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

        public override void AgregarPagoTarjetaCredito(decimal valor, string banco, string nombre, string marca, string tipoPos, string numBin, string bin_descripcion)
        {
            var pago = this.AgregarPago("T. CREDITO", valor);

            pago.Pagos.Add(new PagoTarjetaCredito() {
                Banco = banco,
                Codigo = nombre + " - " + marca,
                Nombre = nombre,
                Marca = marca,
                Valor = valor,
                TipoPos = tipoPos,
                NumeroBin = numBin,
                BinDescripcion = bin_descripcion });

            pago.calcularTotal();
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
            result = this.Pagos.Count > 0 && this.ValidarPagos(out cambio);
            return result;
        }


        public DataTable GetDataTableTblVentaPago()
        {
            DataTable TblVentaPago = new DataTable();
            TblVentaPago.Columns.Add("Tipo_Id", typeof(string));
            TblVentaPago.Columns.Add("Valor", typeof(decimal));
            TblVentaPago.Columns.Add("Datos", typeof(string));
            TblVentaPago.Columns.Add("Voucher", typeof(string));
            TblVentaPago.Columns.Add("Codigo", typeof(string));           
            return TblVentaPago;
        }



        public bool GarbarSQL()
        {
            bool response = true;
            bool pagoIngresado = false;
            string VentaPagoIds = "";
            string separador = "";
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            int codError = 0;
            string msjError = string.Empty;
            int idMensaje = 0; 

            try
            {
                //SqlConnection con = new SqlConnection();
                using (POSEntities pos = new POSEntities())
                {
                    string CadenaConexion = pos.Database.Connection.ConnectionString;

                    foreach (var gc in this.LstGiftcard)
                    {
                        sQuery = string.Empty;
                        //sQuery = string.Concat(sQuery, "Declare @UDT_TblVentaPagoType UDT_TblVentaPagoType ", Environment.NewLine);

                        sQuery = string.Concat(sQuery, $" exec spInsertGiftcardSale ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" @tarjeta = '{ gc.codigo}' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @Monto = { gc.monto} ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @TipoTransaccionId = { gc.tipoTransaccionId} ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @ClienteAx = '{ Customer.ACCOUNTNUM }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @RazonSocial = '{ Customer.NAME }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @Usuario = '{ Control.Common.GlobalParameters.UserObj.username }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @IpProceso = '{ Control.Common.GlobalParameters.IpMaquina }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @IdCaja = '{ Program.ID_Caja_POS }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @Establecimiento = '{ Control.Common.GlobalParameters.Establecimiento }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @PuntoEmision = '{ Control.Common.GlobalParameters.PuntoEmision }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @TipoAdicionTiempo = '{ (byte)TipoAdicionTiempo }' ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @CantidadVigencia = { CantidadVigencia } ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, "  , @UDT_TblVentaPagoType = @UDT_TblVentaPagoType ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $" , @Cambio = { this.Cambio } ", Environment.NewLine);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsGiftCardSale", "GarbarSQL", "Ejecuta spInsertGiftcardSale");

                        DataTable dtVenta = GetDataTableTblVentaPago();

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsGiftCardSale", "GarbarSQL", "Ejecuta GetDataTableTblVentaPago");
                        DataRow workRow = dtVenta.NewRow();

                        if (pagoIngresado == false)
                        {
                            foreach (var p in this.Pagos)
                            {
                                workRow = dtVenta.NewRow();

                                if (p.Pagos.Count > 0)
                                {
                                    
                                    foreach (var subp in p.Pagos)
                                    {
                                        workRow["Tipo_Id"] = p.Descripcion;
                                        workRow["Valor"] = subp.Valor;

                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  Tipo_Id {p.Descripcion}");
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  Valor {subp.Valor}");

                                        if (subp is PagoTarjetaCredito)
                                        {
                                            var pTarjeta = subp as PagoTarjetaCredito;
                                            workRow["Datos"] = pTarjeta.Nombre + "-" + pTarjeta.Marca + "-" + pTarjeta.Codigo;
                                            workRow["Voucher"] = pTarjeta.TipoPos;

                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  Datos {pTarjeta.Nombre + "-" + pTarjeta.Marca + "-" + pTarjeta.Codigo}");
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  Voucher {pTarjeta.TipoPos}");

                                        }
                                        else if (subp is PagoCheque)
                                        {

                                            var pCheque = subp as PagoCheque;
                                            workRow["Datos"] = pCheque.Banco + "-" + pCheque.Cuenta + "-" + pCheque.Numero;

                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  Datos {pCheque.Banco + "-" + pCheque.Cuenta + "-" + pCheque.Numero}");
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  Voucher ");
                                        }

                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  Codigo> Efectivo ");
                                    }

                                }
                                else
                                {
                                    workRow["Tipo_Id"] = p.Descripcion;
                                    workRow["Valor"] = p.Valor - this.Cambio;
                                    workRow["Datos"] = "Efectivo";
                                    workRow["Voucher"] = "";
                                    workRow["Codigo"] = "Efectivo";
                                }


                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"Ejecuta  Tipo_Id {p.Descripcion}");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"Ejecuta  Valor {p.Valor - this.Cambio}");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"Ejecuta  Datos Efectivo");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"Ejecuta  Voucher ");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"Ejecuta  Codigo> Efectivo ");
                                dtVenta.Rows.Add(workRow);
                            }
                        }

                     
                        using (var con = new SqlConnection(CadenaConexion))
                        {

                            try
                            {
                                con.Open();

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $"  query {sQuery}");

                                using (SqlCommand cmd = new SqlCommand(sQuery, con))
                                {


                                    var objDetpago = new SqlParameter("@UDT_TblVentaPagoType", SqlDbType.Structured);
                                    objDetpago.TypeName = "dbo.UDT_TblVentaPagoType";
                                    objDetpago.Value = dtVenta;
                                    cmd.Parameters.Add(objDetpago);

                                    cmd.CommandTimeout = 0;
                                    cmd.Connection = con;
                                    dtsConsulta = new DataSet();

                                    try
                                    {
                                        using (SqlDataReader readerOferta = cmd.ExecuteReader())
                                        {
                                            while (!readerOferta.IsClosed)
                                            {
                                                DataTable dt = new DataTable();
                                                dt.Load(readerOferta);
                                                dtsConsulta.Tables.Add(dt);
                                            }
                                            readerOferta.Close();
                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsGiftCardSale", "GarbarSQL", $"error: {ex.Message}");
                                        
                                    }


                                }

                                con.Close();
                                con.Dispose();

                            }
                            catch (Exception ex)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsGiftCardSale", "GarbarSQL", $"error: {ex.Message}");
                                con.Close();
                                con.Dispose();
                            }
                        }
                      
                    }

                    if(dtsConsulta.Tables.Count >0)
                    {
                        if (dtsConsulta.Tables[0].Rows.Count > 0)
                        {
                            codError = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["codError"].ToString());
                            msjError = dtsConsulta.Tables[0].Rows[0]["msjError"].ToString();
                            idMensaje = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["idMensaje"].ToString());

                        }

                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $" MensajeRespuesta: {msjError} ");
                    Control.Common.General.GetMensajeToList(idMensaje);

                    if (codError != 0) {
                        return false;
                    }

                    return true;
                       
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsGiftCardSale", "GarbarSQL", $" Error : {ex.Message} ");
                return false;

            }

        }
        public bool Grabar()
        {
            bool response = true;
            bool pagoIngresado = false;
            string VentaPagoIds = "";
            string separador = "";
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    foreach (var gc in this.LstGiftcard)
                    {

                        var giftcard2Update = db.core_giftcard.Where(x => x.codigo == gc.codigo).FirstOrDefault();

                        giftcard2Update.saldo = (decimal)(giftcard2Update.saldo + gc.monto);
                        giftcard2Update.valor = (giftcard2Update.valor == null ? 0M : giftcard2Update.valor) + gc.monto;
                        giftcard2Update.activo = true;
                        giftcard2Update.fecha_modificacion = DateTime.Now;
                        giftcard2Update.fecha_desactivacion = null;
                        if (giftcard2Update.fecha_activacion == null || giftcard2Update.fecha_expiracion == null)
                        {
                            giftcard2Update.fecha_activacion = DateTime.Now;
                        }
                        switch (TipoAdicionTiempo)
                        {
                            case Control.Common.Enum.TimeAdditionType.Month:
                                giftcard2Update.fecha_expiracion = DateTime.Now.Date.AddMonths(CantidadVigencia).AddHours(23).AddMinutes(59);
                                break;
                            case Control.Common.Enum.TimeAdditionType.Day:
                                giftcard2Update.fecha_expiracion = DateTime.Now.Date.AddDays(CantidadVigencia).AddHours(23).AddMinutes(59);
                                break;
                        }


                        var ventaGiftcard = new TblVentaTarjeta();

                        ventaGiftcard.Cliente_Ax = Customer.ACCOUNTNUM;
                        ventaGiftcard.CodigoGiftcard = gc.codigo;
                        ventaGiftcard.Establecimiento = Control.Common.GlobalParameters.Establecimiento;
                        ventaGiftcard.PuntoEmision = Control.Common.GlobalParameters.PuntoEmision;
                        ventaGiftcard.FechaCreacion = DateTime.Now;
                        ventaGiftcard.FechaModificacion = DateTime.Now;
                        ventaGiftcard.IdCaja = Program.ID_Caja_POS;
                        ventaGiftcard.IpProceso = Control.Common.GlobalParameters.IpMaquina;
                        ventaGiftcard.Razon_Social = Customer.NAME;
                        ventaGiftcard.TipoTransaccion = (byte)gc.tipoTransaccionId;
                        ventaGiftcard.Total = (decimal)gc.monto;
                        ventaGiftcard.Usuario = Control.Common.GlobalParameters.UserObj.username;


                        if (pagoIngresado == false)
                        { 
                            foreach (var p in this.Pagos)
                            {
                                var pago = new TblVentaPago();
                                pago.FechaCreacion = ventaGiftcard.FechaCreacion;
                                if (p.Pagos.Count > 0)
                                {
                                    foreach (var subp in p.Pagos)
                                    {
                                        pago = new TblVentaPago();
                                        pago.FechaCreacion = ventaGiftcard.FechaCreacion;
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
                                        ventaGiftcard.TblVentaPago.Add(pago);
                                       
                                    }
                                }
                                else
                                {
                                    //Efectivo no tiene subpagos
                                    pago.Tipo_Id = p.Descripcion;
                                    pago.Valor = p.Valor - this.Cambio;
                                    pago.Datos = "Efectivo";
                                }
                                ventaGiftcard.TblVentaPago.Add(pago);
                               
                                pagoIngresado = true;
                            }
                    }
                        ventaGiftcard.VentaPagoId  = VentaPagoIds;
                        db.TblVentaTarjeta.Add(ventaGiftcard);
                        db.SaveChanges();
                         
                        if (VentaPagoIds == "")
                        {
                            foreach (var p in ventaGiftcard.TblVentaPago)
                            {
                                VentaPagoIds = VentaPagoIds + separador + p.Id.ToString();
                                separador = "|";
                            }

                            ventaGiftcard.VentaPagoId = VentaPagoIds;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.Giftcard.ClsGiftcardSale", "Grabar", "Imposible grabar en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                response = false;
                MsgError = ex.ToString();
            }

            return response;
        }

        public bool ImprimirRecibo()
        {
            bool response = true;
            try
            {
                var receiptModel = new Models.PrinterRecipes.ReceiptGiftcardSale();
                receiptModel.CajeroNombre = (string.IsNullOrEmpty(AtendidoPor)) ? Control.Common.GlobalParameters.UserObj.nombres : AtendidoPor;
                receiptModel.CambioTransacccion = Cambio;
                receiptModel.ClienteDireccion = Customer.STREET;
                receiptModel.ClienteIdentificacion = Customer.ACCOUNTNUM;
                receiptModel.ClienteNombre = Customer.NAME;
                receiptModel.ClienteTelefono = Customer.PHONE;
                receiptModel.CodigoTarjeta = Giftcard.codigo;
                switch (GiftcardSaleType)
                {
                    case Control.Common.Enum.GiftcardSaleType.Sale:
                        receiptModel.ConceptoTransaccion = "Venta de giftcard";
                        break;
                    case Control.Common.Enum.GiftcardSaleType.Recharge:
                        receiptModel.ConceptoTransaccion = "Recarga de giftcard";
                        break;
                }
                receiptModel.FechaTransaccion = (FechaTrans == null || FechaTrans.ToString().Contains("0001")) ? DateTime.Now : FechaTrans;
                receiptModel.ItemDescripcion = receiptModel.ConceptoTransaccion;
                receiptModel.ItemValor = PurchaseValue;
                receiptModel.NroComprobante = GetNumeroFactura();
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
                receiptModel.Oficina = Control.Common.GlobalParameters.EstablecimientoNombre+ " " + sub1 + sub2;
                //-----------------------------------------------------------------
                receiptModel.Telefono = Control.Common.GlobalParameters.EstablecimientoTelefono;
                receiptModel.TotalTransacccion = GetTotal();
                receiptModel.Pagos = Pagos;
                receiptModel.PieRecibo = PieRecibo;
                receiptModel.LstGiftcard = LstGiftcard;

                Control.Common.Printer.ImprimirReciboVentaGiftcard(Recibo, receiptModel);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.Giftcard.ClsGiftcardSale", "ImprimirRecibo", "Imposible imprimir comprobante en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                response = false;
                MsgError = "No se pudo realizar la impresion del comprobante de la transacción realizada";
            }

            return response;
        }
    }
}
