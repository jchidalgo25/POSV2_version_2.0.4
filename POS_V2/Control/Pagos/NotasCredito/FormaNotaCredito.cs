using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;
using Telerik.WinControls;
using POS.Control;
using Telerik.WinControls.UI;
using System.Data.SqlClient;
using MensajesLibrary;
using POS.Models.DevolucionIVA;
using POS.Control.Main.MainTouch;
using POS.Models.SRI;
using POS.Models.NotaCredito;

namespace POS.Control.Pagos
{

    public partial class FormaNotaCredito : Telerik.WinControls.UI.RadForm
    {
        Factura _factura;
        Factura _facturaActual;
        User _user;


        System.Windows.Forms.Control focused;
        decimal DesctSel;
        decimal IvaSel;
        decimal SubTotalSel;
        decimal IvaDevolverSel;
        decimal SubtotalDosSel;

        string facturaCompleta = string.Empty;
        int nc_DiasVigencia = 0;
        bool esBeneficiarioDevolucionIVA = false;
        bool esConsumidorFinal = false;
        bool permiteNCConsumidorFinal = false;
        bool tienePago = false;
        decimal montoIvaDevolverMax = 0;


        public Models.Parking.clsParking ObjParking { get; set; }

        public FormaNotaCredito(Factura facturaActual)
        {
            InitializeComponent();

            _factura = new Factura();

            //facturaActual.Documento = "NC";
            _facturaActual = facturaActual.Clone();

            _factura = facturaActual.Clone();
            _factura.Documento = "NC";
            _user = facturaActual.User;

            lblSecuenciaNC.Text = POS.obtenerSecuenciaNC(_facturaActual);
            secNotaCredito.Text = POS.GetSecuenciaNC(_facturaActual).ToString();

            cmbFecha.Value = DateTime.Now.Date;

            this.Load += FormaNotaCredito_Load;
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // evita que se cierre con ALT+F4 o botón cerrar
                this.Hide();     // o mejor: this.Visible = false;
            }

            base.OnFormClosing(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            //txtValor.Focus(); // o cualquier control principal que quieras
        }

        public void ReactivarFoco()
        {
            this.TopMost = true;
            this.BringToFront();
            this.Focus();
        }


        private NotaCreditoModel consultaFactura()
        {
            NotaCreditoModel notaCreditoModel = new NotaCreditoModel();
            _factura = new Factura();
            Int64 secuencia = 0;


            try
            {
                string EstablecimientoAxCode = Control.Common.GlobalParameters.EstablecimientoAxCode;

                string cadena = _factura.Establecimiento.PadLeft(3, '0') + txtPtoEmision.Text.PadLeft(3, '0') + txtNum.Text.PadLeft(9, '0');
                _factura.PtoEmision = txtPtoEmision.Text.PadLeft(3, '0');

                Int64.TryParse(txtNum.Text, out secuencia);
                _factura.Secuencia = secuencia;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FormaNotaCredito", "consultaFactura", "Ejecuta COnsulta Factura para la NC");
                var objNC = notaCreditoModel.GetFactura(_factura.Establecimiento.PadLeft(3, '0'), txtPtoEmision.Text.PadLeft(3, '0'), txtNum.Text);



                if (objNC.cabNotaCredito.esConsumidorFinal)
                {

                    notaCreditoModel.codError = -1;
                    notaCreditoModel.msjError = $"Error:";

                    Control.Common.General.GetMensajeToList(428);
                    return notaCreditoModel;
                }


                lblCedula.ReadOnly = true;
                string Identificacion = objNC.cabNotaCredito.cliente;
                string nombreCliente = objNC.cabNotaCredito.nombreCliente;
                string direccionCliente = objNC.cabNotaCredito.direccionCliente;
                string telefonoCliente = objNC.cabNotaCredito.telefonoCliente;


                esBeneficiarioDevolucionIVA = objNC.cabNotaCredito.esBeneficiarioDevolucionIVA;
                montoIvaDevolverMax = objNC.cabNotaCredito.montoIvaDevolver;
                esConsumidorFinal = objNC.cabNotaCredito.esConsumidorFinal;
                permiteNCConsumidorFinal = objNC.cabNotaCredito.permiteNCConsumidorFinal;
                tienePago = objNC.cabNotaCredito.tienePago;


                lblCedula.Text = Identificacion;
                lblNombre.Text = nombreCliente;
                lblDireccionCliente.Text = direccionCliente;
                lblTelefonoCliente.Text = telefonoCliente;

                lblSubtotal.Text = objNC.cabNotaCredito.subtotal.ToString("###,###,###,##0.00");
                lblDscto.Text = objNC.cabNotaCredito.descuento.ToString("###,###,###,##0.00");
                lblIva.Text = objNC.cabNotaCredito.iva.ToString("###,###,###,##0.00");
                lblTotal.Text = objNC.cabNotaCredito.total.ToString("###,###,###,##0.00");
                lblmontoIvaDevolver.Text = "0";
                radLabelDesc2.Text = objNC.cabNotaCredito.descuento2.ToString("###,###,###,##0.00"); // TRAER DESCUENTOS 2 PARA EL CASO DE LOS DESCUENTO DE EMPLEADOS O DESCUENTOS PROMOCIONALES A LA TOTALIDAD DE LA FACTURA
                //if (objNC.cabNotaCredito.descuento2 != 0 && objNC.cabNotaCredito.descuento2 != null)


                _factura.esBeneficiarioDevolucionIVA = objNC.cabNotaCredito.esBeneficiarioDevolucionIVA;
                _factura.Ruc_matriz = _facturaActual.Ruc_matriz;
                _factura.ClienteIdentificacion = Identificacion;
                _factura.Cliente_nombre = nombreCliente;
                _factura.Cliente_direccion = direccionCliente;
                _factura.Cliente_telefono = telefonoCliente;


                foreach (var coreprod in objNC.detNotaCredito)
                {

                    Producto prod = new Producto();
                    prod.Id = coreprod.item_id;
                    prod.Nombre = coreprod.item_name;
                    prod.Costo = coreprod.costo;
                    prod.Unidad = coreprod.unidad;
                    prod.Pvp = coreprod.pvp;
                    prod.Subtotal = coreprod.subtotal;
                    prod.Total = coreprod.total;
                    prod.Iva = coreprod.iva;
                    prod.IvaProducto = coreprod.iva;
                    prod.Descuento = coreprod.descuento;
                    prod.FechaCreacion = coreprod.fechaCreacion;
                    prod.Cantidad = coreprod.cantidad;
                    prod.Unidades = Int32.Parse(coreprod.unidades.ToString());
                    prod.CantidadOriginal = coreprod.cantidad;
                    prod.DescuentoOriginal = coreprod.descuento;

                    if (coreprod.cantidad > 0)
                    {
                        if (coreprod.EsExcluidoPromoIVA)
                        {
                            prod.EsExcluidoPromoIVA = Control.Common.Promo.EsItemExcluidoPromoIVA(coreprod.item_id);
                        }




                        _factura.Productos.Add(prod);
                    }
                }



                if (_factura.Productos.Count == 0)
                {
                    //MessageBox.Show(this,"Nota de Crédito ya fue aplicada a esta Factura ");
                    Control.Common.General.GetMensajeToList(429);

                    btnGrabar.Visible = false;
                }
                else btnGrabar.Visible = true;

                this.gridItems.DataSource = _factura.Productos;

                _facturaActual = _factura;
                _facturaActual.User = _user;
                return notaCreditoModel; ;
            }
            catch (Exception ex)
            {
                notaCreditoModel.codError = -1;
                notaCreditoModel.msjError = $"Error: { ex.Message}";
                return notaCreditoModel;
            }



        }



        private void consultaFacturaAnt()
        {
            var pos = new POSEntities();
            _factura = new Factura();

            string cadena = _factura.Establecimiento.PadLeft(3, '0') + txtPtoEmision.Text.PadLeft(3, '0') + txtNum.Text.PadLeft(9, '0');
            _factura.PtoEmision = txtPtoEmision.Text.PadLeft(3, '0');

            Int64 secuencia = 0;
            Int64.TryParse(txtNum.Text, out secuencia);
            _factura.Secuencia = secuencia;

            var value = (pos.core_factura.Where(x => x.numero.ToString() == txtNum.Text && x.establecimiento == this._factura.Establecimiento && x.punto_emision == txtPtoEmision.Text /*&& x.cliente != "9999999999999"*/).FirstOrDefault());
            if (value != null)
            {
                if (value.cliente == "9999999999999")
                    lblCedula.ReadOnly = false;
                else
                    lblCedula.ReadOnly = true;


                string EstablecimientoAxCode = Control.Common.GlobalParameters.EstablecimientoAxCode;
                ClienteEmpleado clteEmpleado = Control.Common.General.ValidaClienteEmpleado(value.cliente, EstablecimientoAxCode);

                string Identificacion = "";
                string NombreCliente = "";
                string DireccionEmpleado = "";
                string TelefonoCliente = "";

                if (clteEmpleado != null)
                {
                    Identificacion = clteEmpleado.Identificacion;
                    NombreCliente = clteEmpleado.NombreCliente;
                    DireccionEmpleado = clteEmpleado.DireccionCliente;
                    TelefonoCliente = clteEmpleado.TelefonoCliente;
                }


                lblCedula.Text = Identificacion;
                lblNombre.Text = NombreCliente;
                lblDireccionCliente.Text = DireccionEmpleado;
                lblTelefonoCliente.Text = TelefonoCliente;

                lblSubtotal.Text = value.subtotal.ToString("###,###,###,##0.00");
                lblDscto.Text = value.descuento.ToString("###,###,###,##0.00");
                lblIva.Text = value.iva.ToString("###,###,###,##0.00");
                lblTotal.Text = value.total.ToString("###,###,###,##0.00");
                lblmontoIvaDevolver.Text = "0";


                _factura.esBeneficiarioDevolucionIVA = clteEmpleado.esBeneficiarioDevolucionIVA;
                _factura.Ruc_matriz = _facturaActual.Ruc_matriz;
                _factura.ClienteIdentificacion = Identificacion;
                _factura.Cliente_nombre = NombreCliente;
                _factura.Cliente_direccion = DireccionEmpleado;
                _factura.Cliente_telefono = TelefonoCliente;

                var query = (from detalle in pos.core_facturadetalle
                             where detalle.factura_id == value.id
                             select detalle).ToList();

                // Iterate through the collection of Contact items.
                foreach (var coreprod in query)
                {
                    var producto_con_nc =
                     from nc in pos.core_notacredito
                     join ncproddet in pos.core_notacreditodetalle on nc.id equals ncproddet.notacredito_id
                     where ncproddet.item_id == coreprod.item_id && nc.documentoaplica == cadena
                     select new { DocAplica = nc.documentoaplica, Item = ncproddet.item_id, CantDes = ncproddet.cantidad }; //produces flat sequence

                    Producto prod = new Producto();
                    if (producto_con_nc.Any())
                    {
                        prod.Cantidad = coreprod.cantidad - producto_con_nc.Sum(x => x.CantDes);
                        prod.CantidadINEC = prod.Cantidad;
                        if (coreprod.unidad.ToUpper() == "LB" || coreprod.unidad.ToUpper() == "KG")
                            prod.Unidades = (int)Math.Truncate(coreprod.cantidad - producto_con_nc.Sum(x => x.CantDes)) + 1;
                        else
                            prod.Unidades = (int)coreprod.unidades - (int)producto_con_nc.Sum(x => x.CantDes);
                    }
                    else
                    {
                        prod.Cantidad = coreprod.cantidad;
                        prod.CantidadINEC = prod.Cantidad;
                        if (coreprod.unidad.ToUpper() == "LB" || coreprod.unidad.ToUpper() == "KG")
                            prod.Unidades = (int)Math.Truncate(coreprod.cantidad) + 1;

                        else
                            prod.Unidades = (int)coreprod.unidades;
                    }

                    prod.Id = coreprod.item_id;
                    prod.Nombre = coreprod.item_nombre;
                    prod.Costo = coreprod.costo;
                    prod.Unidad = coreprod.unidad;
                    prod.Pvp = coreprod.precio;
                    prod.Subtotal = coreprod.subtotal;
                    prod.Total = coreprod.total;
                    prod.Iva = coreprod.iva;
                    prod.IvaProducto = coreprod.iva;
                    prod.Descuento = coreprod.descuento;
                    prod.FechaCreacion = value.fecha_creacion;
                    if (prod.Cantidad > 0)
                    {
                        prod.EsExcluidoPromoIVA = Control.Common.Promo.EsItemExcluidoPromoIVA(coreprod.item_id);
                        _factura.Productos.Add(prod);
                    }
                }

                var pago = pos.core_facturapago.Where(x => x.factura_id == value.id).FirstOrDefault();
                if (pago != null)
                {
                    ObjParking = new Models.Parking.clsParking() { Codigo = pago.cliente };
                }
            }
            else
            {
                // MessageBox.Show(this,"Factura no existe\nO Factura es consumidor final");
                Control.Common.General.GetMensajeToList(428);
            }

            this.gridItems.DataSource = _factura.Productos;
            if (_factura.Productos.Count == 0)
            {
                //MessageBox.Show(this,"Nota de Crédito ya fue aplicada a esta Factura ");
                Control.Common.General.GetMensajeToList(429);

                btnGrabar.Visible = false;
            }
            else btnGrabar.Visible = true;

            _facturaActual = _factura;
            _facturaActual.User = _user;

        }


        private void btnBuscarFact_Click(object sender, EventArgs e)
        {

            lblCedula.Text = "";
            lblNombre.Text = "";
            lblSubtotal.Text = "";
            lblDscto.Text = "";
            lblIva.Text = "";
            lblTotal.Text = "";
            lblTotSel.Text = "";
            lblmontoIvaDevolver.Text = "";

            Int64 secuencia = 0;

            consultaFactura();



        }

        private bool ValidarIdentificacionClienteNC()
        {
            //Logica positiva
            bool response = true;
            try
            {
                string identificacion = lblCedula.Text;

                using (POSEntities db = new POSEntities())
                {
                    if (!db.pos_customer.Any(x => x.ACCOUNTNUM == identificacion))
                    {
                        response = false;
                        //MessageBox.Show("La identificación para la NC no está registrada como cliente de POS. Asegúrese de haberla escrito correctamente, caso contrario ingresar al cliente en la pantalla principal. Si cliente es Pasaporte ingréselo primero en Ax", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        Control.Common.General.GetMensajeToList(430);
                    }
                }

            }
            catch (Exception ex)
            {
                response = false;
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "ValidarIdentificacionClienteNC", "No se pudo validar la identificación en este momento, esto pudo deberse a una breve interrupción en la comunicación. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                //MessageBox.Show("No se pudo validar la identificación en este momento, esto pudo deberse a una breve interrupción en la comunicación, por favor vuélvalo a intentar en unos momentos", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Control.Common.General.GetMensajeToList(431);
            }

            return response;
        }





        private bool validaNC()
        {
            bool valida = false;
            try
            {
                //No permitir el error del usuario
                lblCedula.Text = lblCedula.Text.Trim();

                if (lblCedula.Text == Control.Common.GlobalParameters.IdConsumidorFinal && Control.Common.GlobalParameters.NCConsumidorFinal == false)
                {
                    //MessageBox.Show(this, "Cliente no debe ser consumidor Final");
                    Control.Common.General.GetMensajeToList(432);

                    return valida;
                }

                if (string.IsNullOrWhiteSpace(lblCedula.Text) || string.IsNullOrEmpty(lblNombre.Text))
                {
                    //MessageBox.Show(this,"Debe seleccionar un Cliente");
                    Control.Common.General.GetMensajeToList(433);
                    return valida;
                }

                if (!ValidarIdentificacionClienteNC())
                {
                    return valida;
                    //Control.Common.General.GetMensajeToList(430);
                }


                if (decimal.Parse(lblTotSel.Text) <= 0)
                {
                    //MessageBox.Show(this, "Debe seleccionar al menos un Producto");
                    Control.Common.General.GetMensajeToList(434);
                    return valida;
                }

                valida = true;
                return valida;
            }
            catch (Exception)
            {
                valida = false;
                return valida;
            }

        }


        private void btnGrabar_Click(object sender, EventArgs e)
        {
            int versionNotaCredito = 0;

            using (var pos = new POSEntities())
            {
                var param = (from deta in pos.core_parametro
                             where deta.identificador == "EJECUTA_NOTACREDITO"
                             select deta).FirstOrDefault();

                versionNotaCredito = 1;
                if (param != null)
                {
                    versionNotaCredito = Int32.Parse(param.valor);
                }
            }

            if (ejecutaGrabarUDT())
            {
                this.Close();
            }
        }


        private bool ejecutaGrabarUDT()
        {

            string textoImprimir = "";
            decimal TotalCredito = 0;
            decimal TotalCreditoDevIVA = 0;

            decimal DescIva = 0;
            core_giftcard giftcard = new core_giftcard();
            core_notacredito nc = new core_notacredito();
            string reciboNC = string.Empty;
            string reciboGC = string.Empty;
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            RespuestaNotaCredito objRespuesta = new RespuestaNotaCredito();

            DataTable tblNotaCredito = new DataTable();
            DataTable tblNotaCreditoDetalle = new DataTable();
            SqlConnection con = new SqlConnection();
            UDT_NotaCredito uDT_NotaCredito = new UDT_NotaCredito();
            UDT_NotaCreditoDetalle uDT_NotaCreditoDetalle = new UDT_NotaCreditoDetalle();
            bool bGrabar = false;


            //No permitir el error del usuario
            lblCedula.Text = lblCedula.Text.Trim();

            if (lblCedula.Text == Control.Common.GlobalParameters.IdConsumidorFinal && Control.Common.GlobalParameters.NCConsumidorFinal == false)
            {

                //MessageBox.Show(this, "Cliente no debe ser consumidor Final");
                Control.Common.General.GetMensajeToList(432);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Cliente no debe ser consumidor Final ");
                return bGrabar;
            }

            if (string.IsNullOrWhiteSpace(lblCedula.Text) || string.IsNullOrEmpty(lblNombre.Text))
            {
                //MessageBox.Show(this, "Debe seleccionar un Cliente");
                Control.Common.General.GetMensajeToList(433);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Debe seleccionar un Cliente ");
                return bGrabar;
            }

            if (!ValidarIdentificacionClienteNC())
            {
                return bGrabar;
            }


            if (decimal.Parse(lblTotSel.Text) <= 0)
            {
                //MessageBox.Show(this, "Debe seleccionar al menos un Producto");
                Control.Common.General.GetMensajeToList(434);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Debe seleccionar al menos un Producto ");
                return bGrabar;
            }



            string ClaveAccesoSRI = string.Empty;
            decimal IvaDevolver = 0;

            using (var pos = new POSEntities())
            {
                try
                {
                    string CadenaConexion = pos.Database.Connection.ConnectionString;


                    var param = pos.core_parametro.Where(x => x.identificador == "NC_DIASVIGENCIA").FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'NC_DIASVIGENCIA' en core_parametro");
                    }

                    int nc_DiasVigencia = 0;
                    if (!int.TryParse(param.valor, out nc_DiasVigencia))
                    {
                        throw new Exception("Campo 'valor' de registro 'NC_DIASVIGENCIA' en core_parametro es incorrecto y no se puede parsear a int");
                    }

                    if (nc_DiasVigencia < 0)
                    {
                        throw new Exception("Parametro 'NC_DIASVIGENCIA' tiene configurado un valor incorrecto. Valor de dias vigencia debe ser mayor o igual a 0");
                    }

                    #region Registro Nota de Credito / giftCard
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Inicio de proceso para grabar Nota de Credito / GiftCard mediante UDT " + Environment.NewLine);


                    using (con = new SqlConnection(CadenaConexion))
                    {
                        con.Open();



                        sQuery = string.Empty;
                        sQuery = string.Concat(sQuery, "BEGIN TRY  ", Environment.NewLine);

                        sQuery = string.Concat(sQuery, "   Exec spGeneraNotaCredito ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, $"   @establecimiento = '{ _factura.Establecimiento.Trim().PadLeft(3, '0') }'", Environment.NewLine);
                        sQuery = string.Concat(sQuery, "   , @UDT_NotaCredito = @UDT_NotaCredito  ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, "   , @UDT_NotaCreditoDetalle = @UDT_NotaCreditoDetalle ", Environment.NewLine);

                        int iBeneficiaDevIVA = esBeneficiarioDevolucionIVA ? 1 : 0;
                        sQuery = string.Concat(sQuery, $"   , @aplicaDevolucionIVA = {iBeneficiaDevIVA}", Environment.NewLine);
                        decimal valorIvaDevolver = decimal.Parse(lblmontoIvaDevolver.Text);
                        sQuery = string.Concat(sQuery, $"   , @valorIVADevolucion = {valorIvaDevolver}", Environment.NewLine);

                        sQuery = string.Concat(sQuery, "END TRY   ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, "BEGIN CATCH   ", Environment.NewLine);
                        sQuery = string.Concat(sQuery, "    Select CodigoRespuesta = ERROR_NUMBER()", Environment.NewLine);
                        sQuery = string.Concat(sQuery, "    , MensajeRespuesta = convert(varchar(300), '(POS_VOUCHER) ERROR: ' + ERROR_MESSAGE())", Environment.NewLine);
                        sQuery = string.Concat(sQuery, "END CATCH  ", Environment.NewLine);

                        tblNotaCredito = uDT_NotaCredito.GetDataTable();
                        tblNotaCreditoDetalle = uDT_NotaCreditoDetalle.GetDataTable();

                        StringBuilder logContent = new StringBuilder();
                        logContent.AppendLine("Inserto datos UDT_NotaCredito ");


                        string numdocumento = lblSecuenciaNC.Text.Replace("-", "");
                        string establecimiento = Control.Common.GlobalParameters.Establecimiento;
                        string punto_emision = Control.Common.GlobalParameters.PuntoEmision;
                        string numero = lblSecuenciaNC.Text.Substring(8, 9);

                        using (SqlCommand cmd = new SqlCommand(sQuery, con))
                        {
                            DataRow workRow = tblNotaCredito.NewRow();

                            workRow["cliente"] = lblCedula.Text;
                            logContent.AppendLine($"cliente: {lblCedula.Text}");
                            workRow["nombre"] = lblNombre.Text;
                            logContent.AppendLine($"nombre: {lblNombre.Text}");
                            workRow["numdocumento"] = numdocumento;
                            logContent.AppendLine($"numdocumento: {numdocumento}");
                            workRow["fecha"] = cmbFecha.Value;
                            logContent.AppendLine($"fecha: {cmbFecha.Value}");
                            workRow["usuario"] = _facturaActual.User.username;
                            logContent.AppendLine($"usuario: {_facturaActual.User.username}");
                            workRow["valor"] = decimal.Parse(lblTotSel.Text);
                            logContent.AppendLine($"valor: {decimal.Parse(lblTotSel.Text)}");

                            string documentoAplica = _factura.Establecimiento.Trim().PadLeft(3, '0') + txtPtoEmision.Text.Trim().PadLeft(3, '0') + txtNum.Text.Trim().PadLeft(9, '0');
                            workRow["documentoAplica"] = documentoAplica;
                            logContent.AppendLine($"documentoAplica: {documentoAplica}");

                            workRow["motivo"] = cmbMotivo.SelectedValue.ToString();
                            logContent.AppendLine($"motivo: {cmbMotivo.SelectedValue.ToString()}");
                            workRow["msgError"] = Program.ID_Caja_POS;
                            logContent.AppendLine($"msgError: {Program.ID_Caja_POS}");


                            //crea clave de acceso sri
                            ClaveAccesoSRI = string.Empty;
                            var paramCl = pos.core_parametro.Where(x => x.identificador.Equals("ACTIVAR_CLAVE_ACCESO")
                                                        &&
                                                        (x.parametro2 == "TRUE" && x.valor.Contains(Control.Common.GlobalParameters.Establecimiento + ";")))
                                                 .FirstOrDefault();
                            if (paramCl != null)
                            {
                                ClaveAccesoSRI = generarClaveAccesoSRI();
                            }

                            //ClaveAccesoSRI = _factura.generarClaveAccesoSRI("NC");
                            workRow["claveAccesoSRI"] = ClaveAccesoSRI;
                            logContent.AppendLine($"claveAccesoSRI: {ClaveAccesoSRI}");

                            workRow["establecimiento"] = establecimiento;
                            logContent.AppendLine($"establecimiento: {establecimiento}");

                            //workRow["punto_emision"] = _factura.PtoEmision;
                            workRow["punto_emision"] = punto_emision;

                            logContent.AppendLine($"punto_emision: {punto_emision}");

                            workRow["secuenciaNC"] = Int32.Parse(secNotaCredito.Text);
                            logContent.AppendLine($"secuenciaNC: {Int32.Parse(secNotaCredito.Text)}");

                            tblNotaCredito.Rows.Add(workRow);


                            StringBuilder items = new StringBuilder();
                            foreach (var prod in _factura.Productos)
                            {
                                DataRow detalle = tblNotaCreditoDetalle.NewRow();

                                int cont = 0;
                                if (prod.esAjustado)
                                {
                                    cont++;
                                    detalle["numdocumento"] = numdocumento;
                                    logContent.AppendLine($"numdocumento: {numdocumento}");

                                    detalle["fecha"] = cmbFecha.Value;
                                    logContent.AppendLine($"fecha: {cmbFecha.Value}");

                                    detalle["linea"] = cont;
                                    logContent.AppendLine($"linea: {cont}");

                                    detalle["unidad"] = prod.Unidad;
                                    logContent.AppendLine($"unidad: {prod.Unidad}");

                                    detalle["cantidad"] = prod.Cantidad;
                                    logContent.AppendLine($"cantidad: {prod.Cantidad}");

                                    detalle["item_id"] = prod.Id;
                                    logContent.AppendLine($"item_id: {prod.Id}");

                                    detalle["Nombre"] = prod.Nombre;
                                    logContent.AppendLine($"Nombre: {prod.Nombre}");

                                    detalle["pvp"] = prod.Pvp;
                                    logContent.AppendLine($"pvp: {prod.Pvp}");

                                    detalle["descuento"] = prod.Descuento;
                                    logContent.AppendLine($"descuento: {prod.Descuento}");

                                    detalle["iva"] = prod.Iva;
                                    logContent.AppendLine($"iva: {prod.Iva}");

                                    detalle["total"] = prod.Total;
                                    logContent.AppendLine($"total: {prod.Total}");

                                    detalle["TotalNC"] = prod.TotalNC;
                                    logContent.AppendLine($"TotalNC: {prod.TotalNC}");

                                    detalle["SubtotalSinDescuento"] = prod.SubtotalSinDescuento;
                                    logContent.AppendLine($"SubtotalSinDescuento: {prod.SubtotalSinDescuento}");

                                    tblNotaCreditoDetalle.Rows.Add(detalle);

                                    TotalCredito = TotalCredito + prod.TotalNC;

                                    textoImprimir += prod.Nombre.PadRight(40, ' ') + "\n";

                                    textoImprimir += (Control.Common.StringHelper.DevolverConPadding(prod.Iva > 0 ? "I " : "", 1, 1, false) +
                                        Control.Common.StringHelper.DevolverConPadding(prod.Cantidad.ToString("N2"), 50) +
                                        Control.Common.StringHelper.DevolverConPadding(prod.Pvp.ToString("N2"), 12) +
                                        Control.Common.StringHelper.DevolverConPadding(prod.SubtotalSinDescuento.ToString("N2"), 12)) + "\n";


                                }
                            }



                            if (esBeneficiarioDevolucionIVA)
                            {
                                decimal ivadevolucion = decimal.Parse(lblmontoIvaDevolver.Text);
                                IvaDevolver = decimal.Parse(lblmontoIvaDevolver.Text);

                                lblTotSel.Text = (TotalCredito - ivadevolucion).ToString();
                            }
                           


                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "StringInsertNotaCredito", "EjecutaGrabarUDT - Query Insert" + sQuery);
                            string StringInsertNotaCredito = uDT_NotaCredito.GetStringInsert(tblNotaCredito);
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "StringInsertNotaCredito", $"  {StringInsertNotaCredito} ");


                            string StringInsertDetNotaCredito = uDT_NotaCreditoDetalle.GetStringInsert(tblNotaCreditoDetalle);
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "StringInsertDetNotaCredito", $"  {StringInsertDetNotaCredito} ");


                            var objNotacredito = new SqlParameter("@UDT_NotaCredito", SqlDbType.Structured);
                            objNotacredito.TypeName = "dbo.UDT_NotaCredito";
                            objNotacredito.Value = tblNotaCredito;
                            cmd.Parameters.Add(objNotacredito);

                            var objNotacreditoDetalle = new SqlParameter("@UDT_NotaCreditoDetalle", SqlDbType.Structured);
                            objNotacreditoDetalle.TypeName = "dbo.UDT_NotaCreditoDetalle";
                            objNotacreditoDetalle.Value = tblNotaCreditoDetalle;
                            cmd.Parameters.Add(objNotacreditoDetalle);

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
                                Console.Write(ex.Message);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" Error Grabar UDT: {ex.Message} ");

                                con.Close();
                                con.Dispose();

                                return false;
                            }
                        }
                        con.Close();
                        con.Dispose();
                    }



                    int IdNotaCredito = 0;

                    if (dtsConsulta.Tables.Count > 0)
                    {
                        if (dtsConsulta.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                            {
                                objRespuesta.CodigoRespuesta = Int32.Parse(data["codError"].ToString());
                                objRespuesta.MensajeRespuesta = data["msjError"].ToString();

                                if (objRespuesta.CodigoRespuesta == 0)
                                {
                                    IvaDevolver = decimal.Parse(data["IvaDevolver"].ToString());
                                }

                            }

                        }
                        else
                        {
                            objRespuesta.CodigoRespuesta = -3;
                            objRespuesta.MensajeRespuesta = "No se encontraron registros de respuesta, favor contactar a sistemas. ";

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" dtsConsulta.Tables[0].Rows.Count: {dtsConsulta.Tables[0].Rows.Count} ");
                            return false;
                        }
                    }
                    else
                    {
                        objRespuesta.CodigoRespuesta = -2;
                        objRespuesta.MensajeRespuesta = "Objeto Respuesta no es valido, favor contactar a sistemas.";

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" dtsConsulta.Tables, el objeto de la tabla, no tiene datos ");
                        return false;

                    }

                    if (objRespuesta.CodigoRespuesta != 0)
                    {
                        
                        var parametros = new List<ParametrosMensajes>
                        {
                            new ParametrosMensajes { codigo = "[Message]", valor = "Error al registrar la NC" }
                        };

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            Control.Common.General.GetMensajeToList(999, parametros, objRespuesta.MensajeRespuesta);
                        });

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" objRespuesta.MensajeRespuesta: {objRespuesta.MensajeRespuesta}  ");
                        return false;
                    }


                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" CodigoRespuesta: {objRespuesta.CodigoRespuesta} ");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" MensajeRespuesta: {objRespuesta.MensajeRespuesta} ");


                    string identificadorNC = "";
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" identificadorNC: {identificadorNC} ");
                    core_recibo reciboNC_ = new core_recibo();


                    try
                    {


                        nc = (from deta in pos.core_notacredito
                              where deta.id == IdNotaCredito
                              select deta).FirstOrDefault();

                        if (nc != null)
                        {
                            nc.ClaveAccesoSRI = ClaveAccesoSRI;
                        }


                        if (esBeneficiarioDevolucionIVA)
                        {
                            DevolucionIvaModel devolucionIva = new DevolucionIvaModel();
                            devolucionIva.tipoDocumento = "NC";
                            devolucionIva.establecimiento = _factura.Establecimiento.Trim().PadLeft(3, '0');
                            devolucionIva.puntoEmision = txtPtoEmision.Text.PadLeft(3, '0');
                            devolucionIva.numDocumento = txtNum.Text;
                            devolucionIva.ClaveAccesoSRI = ClaveAccesoSRI;
                            devolucionIva.cliente = lblCedula.Text;
                            devolucionIva.estado = "G";

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FormaNotaCredito", "ejecutaGrabarUDT ", $"Ejecuta metodo  grabaDevolucionIVA");
                            _factura.grabaDevolucionIVA(devolucionIva);
                        }



                        if (IvaDevolver != 0)
                        {
                            identificadorNC = "NOTA_CREDITO_DEV_IVA";
                            reciboNC_ = pos.core_recibo.FirstOrDefault(deta => deta.identificador == identificadorNC);
                        }
                        else
                        {
                            identificadorNC = (Control.Common.GlobalParameters.ComprobanteNota_Credito + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode);
                            reciboNC_ = pos.core_recibo.FirstOrDefault(deta => deta.identificador == identificadorNC);


                            if (reciboNC_ == null)
                            {

                                identificadorNC = Control.Common.GlobalParameters.ComprobanteNota_Credito;

                                Control.Common.Logger.LogMessage(
                                    Control.Common.Enum.LogTypes.Info,
                                    "POS.Control.Pagos.FormaNotaCredito",
                                    "btnGrabar_Click",
                                    $"identificadorNC: {identificadorNC}"
                                );

                                reciboNC_ = pos.core_recibo.FirstOrDefault(deta => deta.identificador == identificadorNC);
                            }
                        }


                        reciboNC = reciboNC_?.cuerpo ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        // Opcional: Registrar el error real
                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Error,
                            "POS.Control.Pagos.FormaNotaCredito",
                            "btnGrabar_Click",
                            $"Error al obtener recibo: {ex.Message}, StackTrace: {ex.StackTrace}"
                        );

                        reciboNC = string.Empty;
                    }


                    string identificadorGC = (Control.Common.GlobalParameters.ComprobanteGiftCard + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" identificadorGC: {identificadorGC} ");

                    try
                    {
                        var pos1 = new POSEntities();
                        var reciboGC_ = pos1.core_recibo.FirstOrDefault(d => d.identificador == identificadorGC);

                        if (reciboGC_ == null)
                        {
                            identificadorGC = Control.Common.GlobalParameters.ComprobanteGiftCard;

                            Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                "POS.Control.Pagos.FormaNotaCredito",
                                "btnGrabar_Click",
                                $"identificadorGC: {identificadorGC}"
                            );


                            reciboGC_ = pos1.core_recibo.FirstOrDefault(d => d.identificador == identificadorGC);
                        }

                        // Asignación segura usando operador condicional nulo
                        reciboGC = reciboGC_?.cuerpo ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Error,
                            "POS.Control.Pagos.FormaNotaCredito",
                            "btnGrabar_Click",
                            $"Error al obtener el reciboGC: {ex.Message}, StackTrace: {ex.StackTrace}"
                        );

                        reciboGC = string.Empty;
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $"reciboNC: {reciboNC}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $"reciboGC: {reciboGC}");


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $"imprimeNC: " +
                          $" nc_DiasVigencia|ClaveAccesoSRI:" +
                          $" {nc_DiasVigencia}|{ClaveAccesoSRI}|");

                    imprimeNC(textoImprimir, nc_DiasVigencia, reciboNC, ClaveAccesoSRI);


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $"imprimeGC: " +
                        $" nc_DiasVigencia|ClaveAccesoSRI:" +
                        $" {nc_DiasVigencia}|{ClaveAccesoSRI}|");

                    imprimeGC(nc_DiasVigencia, reciboGC);


                    //if (reciboNC == "" || reciboGC == "")
                    //{
                    //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $"imprimeGIFT");
                    //    imprimeGIFT(textoImprimir, nc_DiasVigencia);
                    //}
                    //else
                    //{

                    //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $"imprimeNC: " +
                    //        $" nc_DiasVigencia|ClaveAccesoSRI:" +
                    //        $" {nc_DiasVigencia}|{ClaveAccesoSRI}|");


                    //    imprimeNC(textoImprimir, nc_DiasVigencia, reciboNC, ClaveAccesoSRI);

                    //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $"imprimeGC: " +
                    //        $" nc_DiasVigencia|ClaveAccesoSRI:" +
                    //        $" {nc_DiasVigencia}|{ClaveAccesoSRI}|");

                    //    imprimeGC(nc_DiasVigencia, reciboGC);
                    //}

                    if (objRespuesta.CodigoRespuesta == 0)
                    {
                        Control.Common.General.GetMensajeToList(670);
                        bGrabar = true;
                    }
                    else {




                        //Control.Common.General.GetMensajeToList(999);
                        bGrabar = false;
                    }
                    #endregion

                    return bGrabar;
                }
                catch (Exception ex)
                {
                    string destinoMail = "devteam@liris.com.ec";
                    string msj = String.Format("En la siguiente caja se está intentando crear una Nota de crédito, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nIpServidor: {3} \nCajeroNombre: {4} \nCajeroIdentificacion: {5} \nError: {6}",
                                       Control.Common.GlobalParameters.Establecimiento,
                                       Control.Common.GlobalParameters.PuntoEmision,

                                       Control.Common.GlobalParameters.IpMaquina,
                                       Control.Common.GlobalParameters.SelectedServerIp,
                                       Control.Common.GlobalParameters.UsuarioNombre,
                                       Control.Common.GlobalParameters.Usuario,
                                       ex.ToString());

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pago.FormaNotaCredito", "btnGrabar", msj);

                    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                    Properties.Settings.Default.MAILERROR_FROM,
                    Properties.Settings.Default.MAILERROR_ALIAS,
                    destinoMail,
                    Properties.Settings.Default.MAILERROR_CC,
                    "POS - Error al guardar Nota Credito",
                    msj,
                    false,
                    String.Empty);

                    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "btnGrabar", "No se pudo guardar la Nota de credito, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                    }

                    Control.Common.WinForm.ShowMessage("La nota de credito no pudo ser guarda, debido a un error de sistemas. Contacte a administrador ahora!");

                    bGrabar = false;
                }

                return bGrabar;
            }



        }


        private void ejecutaGrabar()
        {
            //No permitir el error del usuario
            lblCedula.Text = lblCedula.Text.Trim();

            if (lblCedula.Text == Control.Common.GlobalParameters.IdConsumidorFinal && Control.Common.GlobalParameters.NCConsumidorFinal == false)
            {
                MessageBox.Show(this, "Cliente no debe ser consumidor Final");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Cliente no debe ser consumidor Final ");
                return;
            }

            if (string.IsNullOrWhiteSpace(lblCedula.Text) || string.IsNullOrEmpty(lblNombre.Text))
            {
                MessageBox.Show(this, "Debe seleccionar un Cliente");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Debe seleccionar un Cliente ");
                return;
            }

            if (!ValidarIdentificacionClienteNC())
            {
                return;
            }


            if (decimal.Parse(lblTotSel.Text) <= 0)
            {
                MessageBox.Show(this, "Debe seleccionar al menos un Producto");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Debe seleccionar al menos un Producto ");
                return;
            }

            string textoImprimir = "";
            decimal TotalCredito = 0;
            decimal DescIva = 0;
            core_giftcard giftcard = new core_giftcard();
            core_notacredito nc = new core_notacredito();
            string reciboNC = string.Empty;
            string reciboGC = string.Empty;


            try
            {
                ReversarParqueo();

                var pos = new POSEntities();

                var param = pos.core_parametro.Where(x => x.identificador == "NC_DIASVIGENCIA").FirstOrDefault();
                if (param == null)
                {
                    throw new Exception("No hay parametro 'NC_DIASVIGENCIA' en core_parametro");
                }

                int nc_DiasVigencia = 0;
                if (!int.TryParse(param.valor, out nc_DiasVigencia))
                {
                    throw new Exception("Campo 'valor' de registro 'NC_DIASVIGENCIA' en core_parametro es incorrecto y no se puede parsear a int");
                }

                if (nc_DiasVigencia < 0)
                {
                    throw new Exception("Parametro 'NC_DIASVIGENCIA' tiene configurado un valor incorrecto. Valor de dias vigencia debe ser mayor o igual a 0");
                }

                #region Registro Nota de Credito



                try
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Inicia registo core_notacredito ");

                    nc = new core_notacredito();
                    nc.numdocumento = lblSecuenciaNC.Text.Replace("-", "");
                    nc.fecha = cmbFecha.Value;
                    nc.cliente = lblCedula.Text;
                    nc.usuario = _facturaActual.User.username;
                    nc.valor = decimal.Parse(lblTotSel.Text);
                    nc.documentoaplica = _factura.Establecimiento.Trim().PadLeft(3, '0') + txtPtoEmision.Text.Trim().PadLeft(3, '0') + txtNum.Text.Trim().PadLeft(9, '0');
                    nc.motivo = cmbMotivo.SelectedValue.ToString();
                    nc.msgError = Program.ID_Caja_POS;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Recupera info de core_recibo   ");


                    //var reciboNC_ = pos.core_recibo.Where(x => x.identificador == (Control.Common.GlobalParameters.ComprobanteNota_Credito + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();
                    //if (reciboNC_ == null)
                    //    reciboNC_ = pos.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteNota_Credito).FirstOrDefault();

                    //reciboNC = string.Empty;

                    //if (reciboNC_ != null)
                    //    reciboNC = reciboNC_.cuerpo;


                    string identificadorNC = (Control.Common.GlobalParameters.ComprobanteNota_Credito + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" identificadorNC: {identificadorNC} ");



                    try
                    {
                        var reciboNC_ = pos.core_recibo
                            .FirstOrDefault(deta => deta.identificador == identificadorNC);

                        if (reciboNC_ == null)
                        {
                            Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                "POS.Control.Pagos.FormaNotaCredito",
                                "btnGrabar_Click",
                                $"identificadorNC: {identificadorNC}"
                            );

                            identificadorNC = Control.Common.GlobalParameters.ComprobanteNota_Credito;

                            reciboNC_ = pos.core_recibo
                                .FirstOrDefault(deta => deta.identificador == identificadorNC);
                        }

                        reciboNC = reciboNC_?.cuerpo ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        // Opcional: Registrar el error real
                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Error,
                            "POS.Control.Pagos.FormaNotaCredito",
                            "btnGrabar_Click",
                            $"Error al obtener recibo: {ex.Message}, StackTrace: {ex.StackTrace}"
                        );

                        reciboNC = string.Empty;
                    }





                    foreach (var prod in _factura.Productos)
                    {
                        int cont = 0;
                        if (prod.esAjustado)
                        {
                            cont++;
                            var ncdet = new core_notacreditodetalle();
                            // MessageBox.Show(this,prod.esAjustado.ToString() + "  " + prod.Nombre + "   " + prod.Cantidad);
                            ncdet.fecha = cmbFecha.Value;
                            ncdet.linea = cont;
                            ncdet.unidad = prod.Unidad;
                            ncdet.cantidad = prod.Cantidad;
                            ncdet.item_id = prod.Id;
                            ncdet.pvp = prod.Pvp;
                            ncdet.descuento = prod.Descuento;
                            ncdet.total = prod.Total;
                            TotalCredito = TotalCredito + prod.TotalNC;

                            if (reciboNC == "")
                            {
                                textoImprimir += prod.Cantidad.ToString() + "     " + prod.Nombre + "     " + prod.TotalNC.ToString() + "\n";
                            }
                            else
                            {
                                textoImprimir += prod.Nombre.PadRight(40, ' ') + "\n";
                                textoImprimir += (Control.Common.StringHelper.DevolverConPadding(prod.Iva > 0 ? "I " : "", 1, 1, false) +
                                    Control.Common.StringHelper.DevolverConPadding(prod.Cantidad.ToString("N2"), 50) +
                                    Control.Common.StringHelper.DevolverConPadding(prod.Pvp.ToString("N2"), 12) +
                                    Control.Common.StringHelper.DevolverConPadding(prod.SubtotalSinDescuento.ToString("N2"), 12)) + "\n";
                            }
                            nc.core_notacreditodetalle.Add(ncdet);
                        }

                    }

                    lblTotSel.Text = (TotalCredito).ToString();
                    nc.valor = TotalCredito;

                    //crea clave de acceso sri
                    var paramCl = pos.core_parametro.Where(x => x.identificador.Equals("ACTIVAR_CLAVE_ACCESO")
                                                &&
                                                (x.parametro2 == "TRUE" && x.valor.Contains(Control.Common.GlobalParameters.Establecimiento + ";")))
                                         .FirstOrDefault();
                    if (paramCl != null)
                    {
                        nc.ClaveAccesoSRI = generarClaveAccesoSRI();
                    }
                    else
                    {
                        nc.ClaveAccesoSRI = "";
                    }


                    pos.core_notacredito.Add(nc);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " método add core_notacredito ");

                }
                catch (Exception ex)
                {

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Error: " + ex.Message);

                }
                #endregion

                #region Registro GiftCard
                try
                {
                    giftcard = new core_giftcard();
                    giftcard.fecha_creacion = DateTime.Now;
                    giftcard.fecha_modificacion = DateTime.Now;
                    giftcard.fecha_activacion = DateTime.Now;
                    giftcard.fecha_expiracion = DateTime.Now.Date.AddDays(nc_DiasVigencia).AddHours(23).AddMinutes(59);
                    giftcard.activo = true;
                    giftcard.bono = true;
                    giftcard.codigo = "000" + lblSecuenciaNC.Text.Replace("-", "");
                    giftcard.saldo = decimal.Parse(lblTotSel.Text);
                    giftcard.valor = decimal.Parse(lblTotSel.Text);
                    giftcard.tipoTransaccion = string.Empty;
                    pos.core_giftcard.Add(giftcard);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " método add core_giftcard ");

                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Error: " + ex.Message);
                }
                #endregion

                try
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Ejecuta metodos SaveChange");
                    pos.SaveChanges();

                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " Error: " + ex.Message);

                    string msj = String.Format("En la siguiente caja se está intentando crear una Nota de crédito, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nIpServidor: {3} \nCajeroNombre: {4} \nCajeroIdentificacion: {5} \nError: {6}",
                                   Control.Common.GlobalParameters.Establecimiento,
                                   Control.Common.GlobalParameters.PuntoEmision,

                                   Control.Common.GlobalParameters.IpMaquina,
                                   Control.Common.GlobalParameters.SelectedServerIp,
                                   Control.Common.GlobalParameters.UsuarioNombre,
                                   Control.Common.GlobalParameters.Usuario,
                                   ex.ToString());

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pago.FormaNotaCredito", "btnGrabar", msj);


                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", " recupero formato  del ticket para Nota de Credito / GiftCard ");


                //var reciboGC_ = pos.core_recibo.Where(x => x.identificador == (Control.Common.GlobalParameters.ComprobanteGiftCard + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();
                //reciboGC_ = pos.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteGiftCard).FirstOrDefault();


                string identificadorGC = (Control.Common.GlobalParameters.ComprobanteGiftCard + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click", $" identificadorGC: {identificadorGC} ");
                try
                {
                    var reciboGC_ = pos.core_recibo.FirstOrDefault(d => d.identificador == identificadorGC);

                    if (reciboGC_ == null)
                    {
                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Info,
                            "POS.Control.Pagos.FormaNotaCredito",
                            "btnGrabar_Click",
                            $"identificadorGC: {identificadorGC}"
                        );

                        identificadorGC = Control.Common.GlobalParameters.ComprobanteGiftCard;

                        reciboGC_ = pos.core_recibo.FirstOrDefault(d => d.identificador == identificadorGC);
                    }

                    // Asignación segura usando operador condicional nulo
                    reciboGC = reciboGC_?.cuerpo ?? string.Empty;
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        "POS.Control.Pagos.FormaNotaCredito",
                        "btnGrabar_Click",
                        $"Error al obtener el reciboGC: {ex.Message}, StackTrace: {ex.StackTrace}"
                    );

                    reciboGC = string.Empty;
                }



                //Debitar puntos de promociones que tenga la factura
                var coreFactura = pos.core_factura.Include("core_facturadetalle")
                                                  .Include("core_facturapago")
                                                  .Where(x => x.numero.ToString() == txtNum.Text
                                                            && x.establecimiento == this._factura.Establecimiento
                                                            && x.punto_emision == txtPtoEmision.Text)
                                                  .FirstOrDefault();


                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click",
                    " Ejeucta debito de puntos de promociones que tenga la factura ");

                var acumuladorPuntos = new Control.WalletPoints.ClsAcumulacion();
                var facturaSoporteNC = _factura.Clone();

                acumuladorPuntos.DebitarPuntosNC(nc, facturaSoporteNC, coreFactura);



                //Agregar lineas de insert
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click",
                    " Ejecuta Agregar_Trace_NotaCredito ");

                Control.Common.Logger.Agregar_Trace_NotaCredito(nc);


                //Agregar lineas de insert
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click",
                    " Ejecuta Agregar_Trace_Giftcard ");
                Control.Common.Logger.Agregar_Trace_Giftcard(giftcard);


                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.FormaNotaCredito", "btnGrabar_Click",
                    " Ejecuta actualizarSecuenciaNC ");
                POS.actualizarSecuenciaNC(_facturaActual);

                // Acumular compra gratis.  JM  3/7/2020
                //grabarAcumulaCompraGratis(nc.id);
                if (reciboNC == "" || reciboGC == "")
                {
                    imprimeGIFT(textoImprimir, nc_DiasVigencia);
                }
                else
                {


                    imprimeNC(textoImprimir, nc_DiasVigencia, reciboNC, nc.ClaveAccesoSRI);

                    imprimeGC(nc_DiasVigencia, reciboGC);
                }

                MessageBox.Show(this, "Nota de Credito Generada");
                this.Close();
            }
            catch (Exception ex)
            {
                string destinoMail = "devteam@liris.com.ec";
                string msj = String.Format("En la siguiente caja se está intentando crear una Nota de crédito, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nIpServidor: {3} \nCajeroNombre: {4} \nCajeroIdentificacion: {5} \nError: {6}",
                                   Control.Common.GlobalParameters.Establecimiento,
                                   Control.Common.GlobalParameters.PuntoEmision,

                                   Control.Common.GlobalParameters.IpMaquina,
                                   Control.Common.GlobalParameters.SelectedServerIp,
                                   Control.Common.GlobalParameters.UsuarioNombre,
                                   Control.Common.GlobalParameters.Usuario,
                                   ex.ToString());

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pago.FormaNotaCredito", "btnGrabar", msj);

                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                Properties.Settings.Default.MAILERROR_FROM,
                Properties.Settings.Default.MAILERROR_ALIAS,
                destinoMail,
                Properties.Settings.Default.MAILERROR_CC,
                "POS - Error al guardar Nota Credito",
                msj,
                false,
                String.Empty);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "btnGrabar", "No se pudo guardar la Nota de credito, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }

                Control.Common.WinForm.ShowMessage("La nota de credito no pudo ser guarda, debido a un error de sistemas. Contacte a administrador ahora!");
            }

        }
        public string generarClaveAccesoSRI()
        {
            string claveAcceso = string.Empty;
            string fecha = DateTime.Now.ToString("ddMMyyyy");
            string tipoComprobante = "04";
            string nRuc = Control.Common.GlobalParameters.NRuc;
            string tipoAmbiente = Control.Common.GlobalParameters.TipoAmbiente;
            string establecimiento = Control.Common.GlobalParameters.Establecimiento;
            string punto_emision = Control.Common.GlobalParameters.PuntoEmision;
            string numero = lblSecuenciaNC.Text.Substring(8, 9);
            string codigoNumerico = nRuc.Substring(0, 8);
            string tipoEmision = Control.Common.GlobalParameters.TipoEmision;
            int modulo = 11;
            int inicio = 7;
            int sumaT = 0;

            claveAcceso = claveAcceso + fecha + tipoComprobante + nRuc + tipoAmbiente + establecimiento +
                punto_emision + numero + codigoNumerico + tipoEmision;
            for (int i = 0; i < claveAcceso.Length; i++)
            {
                if (inicio == 1)
                {
                    inicio = 7;
                }
                sumaT = sumaT + (inicio * Convert.ToInt32(claveAcceso.Substring(i, 1)));

                inicio--;
            }
            int digitoVerificador = (modulo - (sumaT % modulo));

            if (digitoVerificador == 10)
            {
                digitoVerificador = 1;
            }
            else if (digitoVerificador == 11)
            {
                digitoVerificador = 0;
            }
            claveAcceso = claveAcceso + digitoVerificador.ToString();

            return claveAcceso;
        }
        public string GenerarClaveAccesoSRI_NC()
        {
            string claveAcceso = string.Empty;

            // Obtener la fecha en formato ddMMyyyy
            string fecha = DateTime.Now.ToString("ddMMyyyy");


            // Recuperar parámetros globales
            string nRuc = Control.Common.GlobalParameters.NRuc;
            string tipoAmbiente = Control.Common.GlobalParameters.TipoAmbiente;
            string establecimiento = Control.Common.GlobalParameters.Establecimiento;
            string puntoEmision = Control.Common.GlobalParameters.PuntoEmision;
            string tipoEmision = Control.Common.GlobalParameters.TipoEmision;

            // Validar que los parámetros esenciales no sean nulos ni vacíos
            if (string.IsNullOrEmpty(nRuc)) throw new ArgumentException("El RUC no puede estar vacío.");
            if (string.IsNullOrEmpty(establecimiento)) throw new ArgumentException("El establecimiento no puede estar vacío.");
            if (string.IsNullOrEmpty(puntoEmision)) throw new ArgumentException("El punto de emisión no puede estar vacío.");
            if (string.IsNullOrEmpty(tipoEmision)) throw new ArgumentException("El tipo de emisión no puede estar vacío.");

            // Obtener la secuencia completa de la Nota de Crédito (asegurarse que tiene 9 dígitos)
            //string numero = lblSecuenciaNC.Text.Trim();
            string numero = secNotaCredito.Text.Trim();

            if (numero.Length < 9) // Si la secuencia tiene menos de 9 dígitos, completar con ceros a la izquierda
            {
                numero = numero.PadLeft(9, '0');
            }

            // Validar que la secuencia sea válida
            if (numero.Length != 9) throw new ArgumentException("La secuencia debe tener 9 dígitos.");

            // Tomar los primeros 8 dígitos del RUC (código numérico)
            string codigoNumerico = nRuc.Substring(0, 8);

            // Construcción de la clave de acceso
            claveAcceso = fecha
                          + "04"
                          + nRuc
                          + tipoAmbiente
                          + establecimiento
                          + puntoEmision
                          + numero
                          + codigoNumerico
                          + tipoEmision;

            // Cálculo del dígito verificador utilizando el algoritmo de módulo 11
            int modulo = 11;
            int sumaT = 0;
            int inicio = 7;

            for (int i = 0; i < claveAcceso.Length; i++)
            {
                if (inicio == 1)
                {
                    inicio = 7;
                }
                sumaT += (inicio * Convert.ToInt32(claveAcceso.Substring(i, 1)));
                inicio--;
            }

            // Calcular el dígito verificador
            int digitoVerificador = (modulo - (sumaT % modulo));

            // Ajustes en el dígito verificador según las reglas del SRI
            if (digitoVerificador == 10)
            {
                digitoVerificador = 1;
            }
            else if (digitoVerificador == 11)
            {
                digitoVerificador = 0;
            }

            // Agregar el dígito verificador a la clave de acceso
            claveAcceso += digitoVerificador.ToString();

            return claveAcceso;
        }



        public string generarClaveAccesoSRI_V1()
        {
            string claveAcceso = string.Empty;
            string fecha = DateTime.Now.ToString("ddMMyyyy");
            string tipoComprobante = "04";
            string nRuc = Control.Common.GlobalParameters.NRuc;
            string tipoAmbiente = Control.Common.GlobalParameters.TipoAmbiente;
            string establecimiento = Control.Common.GlobalParameters.Establecimiento;
            string punto_emision = Control.Common.GlobalParameters.PuntoEmision;
            string numero = lblSecuenciaNC.Text.Substring(8, 9);
            string codigoNumerico = nRuc.Substring(0, 8);
            string tipoEmision = Control.Common.GlobalParameters.TipoEmision;
            int modulo = 11;
            int inicio = 7;
            int sumaT = 0;


            claveAcceso = claveAcceso +
                fecha +
                tipoComprobante +
                nRuc +
                tipoAmbiente +
                establecimiento +
                punto_emision +
                numero +
                codigoNumerico +
                tipoEmision;


            for (int i = 0; i < claveAcceso.Length; i++)
            {
                if (inicio == 1)
                {
                    inicio = 7;
                }
                sumaT = sumaT + (inicio * Convert.ToInt32(claveAcceso.Substring(i, 1)));

                inicio--;
            }
            int digitoVerificador = (modulo - (sumaT % modulo));

            if (digitoVerificador == 10)
            {
                digitoVerificador = 1;
            }
            else if (digitoVerificador == 11)
            {
                digitoVerificador = 0;
            }
            claveAcceso = claveAcceso + digitoVerificador.ToString();

            return claveAcceso;
        }


        private void ReversarParqueo()
        {
            if (ObjParking != null)
            {
                if (ObjParking.EstaConfirmado)
                {
                    string path, pathDestino;
                    System.IO.DirectoryInfo directoryInfo;
                    System.IO.FileStream fileStream;
                    System.Diagnostics.TextWriterTraceListener listener;
                    DateTime fechaIngresoOriginal = new DateTime();

                    //----------------------------------------Archivo INGRESO
                    path = System.IO.Path.Combine(Control.Common.GlobalParameters.Parking_PathIngreso, DateTime.Now.Date.ToString("yyyyMMdd"), "PROCESADO", ObjParking.Codigo + "_*.txt");
                    pathDestino = System.IO.Path.Combine(Control.Common.GlobalParameters.Parking_PathIngreso, DateTime.Now.Date.ToString("yyyyMMdd"), ObjParking.Codigo + ".txt");

                    directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(path));
                    //Si no existe directorio, crearlo
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    switch (ObjParking.TipoItem)
                    {
                        case Common.Enum.ParkingItemType.ParqueoConCompra:
                            break;
                        case Common.Enum.ParkingItemType.Parqueo:
                            var latestFile = directoryInfo.GetFiles().Where(x => x.Name.Contains(ObjParking.Codigo)).OrderByDescending(f => f.FullName).FirstOrDefault();

                            if (latestFile != null)
                            {
                                var content = System.IO.File.ReadAllText(latestFile.FullName);
                                fechaIngresoOriginal = DateTime.Now.Date.Add(TimeSpan.Parse(content));

                                //Si archivo ya existe, eliminarlo
                                if (System.IO.File.Exists(pathDestino))
                                {
                                    System.IO.File.Delete(pathDestino);
                                }

                                //Crear archivo con hora de ingreso
                                fileStream = new System.IO.FileStream(pathDestino, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                                listener = new System.Diagnostics.TextWriterTraceListener(fileStream);
                                System.Diagnostics.Trace.Listeners.Add(listener);
                                System.Diagnostics.Trace.Write(string.Concat(new string[]
                                {
                                    fechaIngresoOriginal.ToString("HH:mm")
                                }));
                                System.Diagnostics.Trace.Flush();
                                System.Diagnostics.Trace.Close();
                                fileStream.Close();
                            }


                            break;
                        case Common.Enum.ParkingItemType.PerdidaTicket:
                            //Si archivo ya existe, eliminarlo
                            if (System.IO.File.Exists(pathDestino))
                            {
                                System.IO.File.Delete(pathDestino);
                            }

                            break;
                        default:
                            break;
                    }

                    //----------------------------------------Archivo SALIDA
                    path = System.IO.Path.Combine(Control.Common.GlobalParameters.Parking_PathSalida, DateTime.Now.Date.ToString("yyyyMMdd"), ObjParking.Codigo + ".txt");

                    directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(path));
                    //Si no existe directorio, crearlo
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    //Si archivo ya existe, eliminarlo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    if (ObjParking.TipoItem == Common.Enum.ParkingItemType.Parqueo)
                    {
                        //Crear archivo con hora de salida
                        fileStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                        listener = new System.Diagnostics.TextWriterTraceListener(fileStream);
                        System.Diagnostics.Trace.Listeners.Add(listener);
                        System.Diagnostics.Trace.Write(string.Concat(new string[]
                        {
                            fechaIngresoOriginal.AddMinutes(Control.Common.GlobalParameters.Parking_MinutosGracia).ToString("HH:mm")
                        }));
                        System.Diagnostics.Trace.Flush();
                        System.Diagnostics.Trace.Close();
                        fileStream.Close();
                    }

                    //Item Perdida de Ticket no vuelve a generar archivos
                }
            }
        }

        private void FormaNotaCredito_Load(object sender, EventArgs e)
        {


            using (var pos = new POSEntities())
            {
                cmbMotivo.DataSource = pos.VW_NC_Motivo.OrderBy(x => x.DESCRIPTION).ToList();
                cmbMotivo.DisplayMember = "DESCRIPTION";
                cmbMotivo.ValueMember = "ID";



                var param = pos.core_parametro.Where(x => x.identificador == "NC_DIASVIGENCIA").FirstOrDefault();
                if (param == null)
                {
                    throw new Exception("No hay parametro 'NC_DIASVIGENCIA' en core_parametro");
                }

                if (!int.TryParse(param.valor, out nc_DiasVigencia))
                {
                    throw new Exception("Campo 'valor' de registro 'NC_DIASVIGENCIA' en core_parametro es incorrecto y no se puede parsear a int");
                }

                if (nc_DiasVigencia < 0)
                {
                    throw new Exception("Parametro 'NC_DIASVIGENCIA' tiene configurado un valor incorrecto. Valor de dias vigencia debe ser mayor o igual a 0");
                }

                this.ShowInTaskbar = false;
                this.TopMost = false;

            }



        }

        private void calcula()
        {
            decimal totalValor = 0M;
            decimal totalDescuento = 0M;
            decimal totalIva = 0M;
            decimal totalSubtotal = 0M;
            decimal totalIvaDevolver = 0M;
            decimal totalDescuentoLinea = 0M;
            const decimal TASA_IVA = 0.15M; // 15%
            decimal valorNeto = 0M;
            decimal subtotaldosC = 0M;
          

            try
            {
                // Verificar si el campo Desc2 tiene contenido
                bool tieneDesc2 = !string.IsNullOrEmpty(this.radLabelDesc2.Text) && this.radLabelDesc2.Text != "0" && this.radLabelDesc2.Text != "0.00";
                bool tienesubtotalCabecera = !string.IsNullOrEmpty(this.lblSubtotal.Text) && this.lblSubtotal.Text != "0" && this.lblSubtotal.Text != "0.00";
                bool tienedescuentoCabecera = !string.IsNullOrEmpty(this.lblDscto.Text) && this.lblDscto.Text != "0" && this.lblDscto.Text != "0.00";
                decimal descuento2 = 0M;
                decimal subtotalCabecera = 0M;
                decimal descuentoCabecera = 0M;


                if (tieneDesc2)
                {
                    decimal.TryParse(this.radLabelDesc2.Text.Replace(",", ""), out descuento2);
                }
                if (tienesubtotalCabecera)
                {
                    decimal.TryParse(this.lblSubtotal.Text.Replace(",", ""), out subtotalCabecera);
                }
                if (tienedescuentoCabecera)
                {
                    decimal.TryParse(this.lblDscto.Text.Replace(",", ""), out descuentoCabecera);
                }
                if (tieneDesc2)
                {
                    // ============================================
                    // LÓGICA ESPECIAL CUANDO DESC2 ESTÁ LLENO
                    // ============================================

                    // AQUÍ VA LA NUEVA LÓGICA QUE ME DIRÁS
                    foreach (GridViewRowInfo fila in gridItems.Rows)
                    {
                        bool isSelected = Convert.ToBoolean(fila.Cells["chk"].Value ?? false);

                        if (isSelected)
                        {
                            var producto = fila.DataBoundItem as Producto;
                            if (producto == null) continue;

                            // Valores de la nota de crédito
                            decimal cantidadNC = Convert.ToDecimal(fila.Cells["Cantidad"].Value ?? 0);
                            decimal pvp = Convert.ToDecimal(fila.Cells[5].Value ?? 0);

                            // 1. CALCULAR SUBTOTAL (Cantidad × Precio)
                            decimal subtotalLinea = cantidadNC * pvp;
                            // Variable que me trae el Descuento del Producto
                            // 2. CALCULAR DESCUENTO PROPORCIONAL
                            decimal descuentoUnitario = 0M;


                            if (producto.CantidadOriginal > 0)
                            {
                                descuentoUnitario = producto.DescuentoOriginal / producto.CantidadOriginal;
                            }
                            decimal descuentoLinea = descuentoUnitario * cantidadNC;

                            // Variable de calculo de Primer Subtotal2
                            decimal subtotal2 = subtotalLinea - descuentoLinea;

                            // Varible de calculo de descuento2 nueva de acuerdo a los articulos selecionados 
                            decimal desctpro2 = subtotal2 * (descuento2 / (subtotalCabecera - descuentoCabecera));

                            // Variale Subtotal3 despues de aplicar el descuento
                            decimal subtotal3 = subtotal2 - desctpro2;

                            // calculo del IVA nuevo en base a los nuevos valores 
                            decimal IVAnuevo = 0M;
                            if (producto.IvaProducto > 0)
                            {
                                IVAnuevo = subtotal3 * TASA_IVA;
                            }

                            producto.esAjustado = true;
                            producto.Cantidad = cantidadNC;
                            producto.Descuento = descuentoLinea;
                            producto.DescuentoActual = desctpro2;
                            producto.Subtotal = subtotalLinea;
                            producto.Iva = IVAnuevo;


                            totalSubtotal += subtotalLinea;
                            totalDescuento += desctpro2;
                            totalDescuentoLinea += descuentoLinea;
                            totalIva += IVAnuevo;

                        }
                    }
                }
                else
                {
                    // ============================================
                    // LÓGICA ORIGINAL CUANDO DESC2 ESTÁ VACÍO
                    // ============================================

                    foreach (GridViewRowInfo fila in gridItems.Rows)
                    {
                        bool isSelected = Convert.ToBoolean(fila.Cells["chk"].Value ?? false);

                        if (isSelected)
                        {
                            var producto = fila.DataBoundItem as Producto;
                            if (producto == null) continue;

                            // Valores de la nota de crédito
                            decimal cantidadNC = Convert.ToDecimal(fila.Cells["Cantidad"].Value ?? 0);
                            decimal pvp = Convert.ToDecimal(fila.Cells[5].Value ?? 0);

                            // 1. CALCULAR SUBTOTAL (Cantidad × Precio)
                            decimal subtotalLinea = cantidadNC * pvp;

                            // 2. CALCULAR DESCUENTO PROPORCIONAL
                            decimal descuentoUnitario = 0M;


                            if (producto.CantidadOriginal > 0)
                            {
                                descuentoUnitario = producto.DescuentoOriginal / producto.CantidadOriginal;
                            }
                            decimal descuentoLinea = descuentoUnitario * cantidadNC;

                            // 3. CALCULAR IVA CORRECTAMENTE
                            decimal ivaLinea = 0M;

                            // Verificar si el producto tiene IVA (productos gravados)
                            if (producto.IvaProducto > 0)
                            {
                                // Base imponible = Subtotal - Descuentos
                                decimal baseImponible = subtotalLinea - descuentoLinea;

                                // IVA = Base Imponible × Tasa IVA (15%)
                                ivaLinea = baseImponible * TASA_IVA;
                            }

                            // 4. CALCULAR TOTAL DE LA LÍNEA
                            decimal totalLinea = subtotalLinea - descuentoLinea + ivaLinea;

                            // Actualizar producto
                            producto.esAjustado = true;
                            producto.Cantidad = cantidadNC;
                            producto.Descuento = descuentoLinea;
                            producto.Subtotal = subtotalLinea;
                            producto.Iva = ivaLinea;
                            //producto.TotalNC = totalLinea;

                            // Acumular totales
                            totalSubtotal += subtotalLinea;
                            totalDescuentoLinea += descuentoLinea;
                            totalIva += ivaLinea;
                            totalIvaDevolver += ivaLinea; // Para beneficiarios de devolución IVA
                        }
                        else
                        {
                            // Marcar como no ajustado si no está seleccionado
                            var producto = fila.DataBoundItem as Producto;
                            if (producto != null)
                            {
                                producto.esAjustado = false;
                            }
                        }
                    }
                }

                // ============================================
                // LÓGICA COMÚN PARA AMBOS CASOS
                // ============================================

                // Aplicar límite máximo de devolución IVA
                if (totalIvaDevolver > montoIvaDevolverMax)
                {
                    totalIvaDevolver = montoIvaDevolverMax;
                }

                // Mostrar totales
                lblSubtotalSel.Text = totalSubtotal.ToString("N2");
                lblDsctoSel.Text = totalDescuentoLinea.ToString("N2");
                lblIvaSel.Text = totalIva.ToString("N2");
                lblmontoIvaDevolver.Text = totalIvaDevolver.ToString("N2");
                lbldescSel.Text = totalDescuento.ToString("N2"); // ESTO TIENE QUE TENER VALOR SI  ES QUE VIENE  DEL PRIMER IF CASO CONTRARIO ES 0

                //if (tieneDesc2)
                //{
                //    lblSubtotalSel.Text = totalSubtotal.ToString("N2");
                //    lblDsctoSel.Text = totalDescuentoLinea.ToString("N2");
                //    lblIvaSel.Text = totalIva.ToString("N2");
                //    lblmontoIvaDevolver.Text = totalIvaDevolver.ToString("N2");
                //    lbldescSel.Text = totalDescuento.ToString("N2"); // ESTO TIENE QUE TENER VALOR SI  ES QUE VIENE  DEL PRIMER IF CASO CONTRARIO ES 0
                //}
                //else
                //{
                //    lblSubtotalSel.Text = totalSubtotal.ToString("N2");
                //    lblIvaSel.Text = totalIva.ToString("N2");
                //    lblmontoIvaDevolver.Text = totalIvaDevolver.ToString("N2");
                //    lblDsctoSel.Text = totalDescuentoLinea.ToString("N2");
                //}
                // Calcular total final
               
                
                //lblTotSel.Text = valorNeto.ToString("N2");

                if (tieneDesc2)
                {
                    valorNeto = totalSubtotal -( totalDescuento  + totalDescuentoLinea) + totalIva;
                    DesctSel = totalDescuento + totalDescuentoLinea;
                    subtotaldosC = totalSubtotal - (totalDescuento + totalDescuentoLinea);
                } 
                else
                {
                    valorNeto = totalSubtotal - totalDescuentoLinea + totalIva;
                    DesctSel =  totalDescuentoLinea;
                    subtotaldosC = totalSubtotal - (totalDescuento + totalDescuentoLinea);
                }

                if (esBeneficiarioDevolucionIVA)
                {
                    valorNeto -= totalIvaDevolver;
                    DesctSel = totalDescuento;

                }

                lblTotSel.Text = valorNeto.ToString("N2");


                // Guardar para otros procesos
                SubTotalSel = totalSubtotal;
                //DesctSel = totalDescuento;
                IvaSel = totalIva;
                IvaDevolverSel = totalIvaDevolver;
                SubtotalDosSel = subtotaldosC;



            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "FormaNotaCredito",
                    "calcula",
                    $"Error: {ex.Message}"
                );
            }
        }
        private void calculaAnt()
        {
            var cantidad = 0M;
            var Valor = 0M;
            var Desct = 0M;
            var Iva = 0M;
            var IvaDevolverSel = 0M;
            var SubTotal = 0M;
            decimal montoIvaDevolver = 0;


            try
            {
                foreach (GridViewRowInfo fila in gridItems.Rows)
                {
                    if ((bool)fila.Cells[0].Value == true)
                    {
                        cantidad = (decimal)fila.Cells[2].Value;
                        Valor += (decimal)fila.Cells[9].Value;
                        Desct += (decimal)fila.Cells[7].Value;
                        Iva += (decimal)fila.Cells[8].Value;
                        IvaDevolverSel += (decimal)fila.Cells[8].Value;
                        SubTotal += (decimal)fila.Cells[6].Value;
                    }


                    if (Desct != 0)
                    {
                        Desct = cantidad / Desct;

                    }

                }

                if (IvaDevolverSel > montoIvaDevolverMax) { IvaDevolverSel = montoIvaDevolverMax; }

                lblDsctoSel.Text = Desct.ToString();
                lblIvaSel.Text = Iva.ToString();
                lblmontoIvaDevolver.Text = IvaDevolverSel.ToString();
                lblSubtotalSel.Text = SubTotal.ToString();


                if (Valor == 0)
                {
                    Valor = SubTotal - Iva;
                }


                if (esBeneficiarioDevolucionIVA)
                {
                    lblmontoIvaDevolver.Text = IvaDevolverSel.ToString();
                    Valor = Valor - IvaDevolverSel;
                }


                lblTotSel.Text = Valor.ToString();
                DesctSel = Desct;
                IvaSel = Iva;
                SubTotalSel = SubTotal;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FormaNotaCredito", "calcula", $"Error:{ex.Message}");
            }

        }
        private void chkSelecciona_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {

            foreach (GridViewRowInfo fila in gridItems.Rows)
            {
                var item = fila.DataBoundItem as Models.Producto;
                if (!Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(x => x.Equals(item.Id)) && !Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(x => x.Equals(item.Id)))
                {
                    fila.Cells[0].Value = chkSelecciona.Checked;
                }
            }
            calcula();
        }

        private void gridItems_CurrentCellChanged(object sender, CurrentCellChangedEventArgs e)
        {

            calcula();
        }

        private void gridItems_CellValidating(object sender, CellValidatingEventArgs e)
        {


            if (e.Column.Name == "Cantidad")
            {
                decimal number1 = 0M;

                if (e.Value == null)
                {
                    e.Cancel = true;
                    return;
                }

                bool canConvert = decimal.TryParse(e.Value.ToString(), out number1);
                if (canConvert != true)
                {
                    e.Cancel = true;
                    return;
                }

                decimal canitdad = decimal.Parse(e.Value.ToString());
                decimal unidades = decimal.Parse(e.Row.Cells[3].Value.ToString());

                if (Math.Abs(canitdad) > Math.Abs(unidades) || Math.Abs(canitdad) == 0)
                {
                    //MessageBox.Show(this,"Cantidad no puede ser mayor que el registrado o 0");
                    Control.Common.General.GetMensajeToList(437);

                    e.Cancel = true;
                }

            }


            try
            {
                if (e.Column.Name == "chk")
                {
                    var item = gridItems.SelectedRows[0].DataBoundItem as Models.Producto;
                    if (e.Value.ToString() != "Off")
                    {

                        try
                        {
                            if (Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(x => x.Equals(item.Id)) || Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(x => x.Equals(item.Id)))
                            {
                                var pathSalida = System.IO.Path.Combine(Control.Common.GlobalParameters.Parking_PathSalida, DateTime.Now.Date.ToString("yyyyMMdd"), ObjParking.Codigo + ".txt");
                                if (!System.IO.File.Exists(pathSalida))
                                {
                                    //MessageBox.Show("El item '" + item.Nombre + "' ya no puede ser seleccionado porque es de servicio parqueo y el vehículo ya no está en el estacionamiento");

                                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                    parametros.Add(new ParametrosMensajes() { codigo = "[itemName]", valor = item.Nombre });
                                    Control.Common.General.GetMensajeToList(438, parametros);

                                    ObjParking.EstaConfirmado = false;
                                    e.Cancel = true;

                                    return;
                                }

                                if (!Control.Common.GlobalParameters.UserObj.isSuperUser)
                                {
                                    var Verifier = new Fingerprint.VerificationForm(Control.Common.GlobalParameters.DataForFingerprint, null);
                                    Verifier.Tag = "adm";
                                    if (Verifier.ShowDialog() != DialogResult.OK)
                                    {
                                        //MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Items de servicio parqueo solo puede ser seleccionado por administrador. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                        Control.Common.General.GetMensajeToList(439);

                                        ObjParking.EstaConfirmado = false;
                                        e.Cancel = true;
                                        return;
                                    }
                                }

                                //MessageBox.Show("El item '" + item.Nombre + "' no puede ser seleccionado porque es de servicio parqueo");
                                //e.Cancel = true;
                                //return;

                                ObjParking.EstaConfirmado = true;
                                if (Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(x => x.Equals(item.Id)))
                                {
                                    ObjParking.TipoItem = Control.Common.Enum.ParkingItemType.Parqueo;
                                }
                                if (Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(x => x.Equals(item.Id)))
                                {
                                    ObjParking.TipoItem = Control.Common.Enum.ParkingItemType.PerdidaTicket;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FormaNotaCredito", "gridItems_CellValidating", $"Error:{ex.Message}");
                        }


                    }
                    else
                    {
                        ObjParking.EstaConfirmado = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FormaNotaCredito", "gridItems_CellValidating", $"Error:{ex.Message}");
            }


            calcula();
        }

        private void gridItems_CellValidated(object sender, CellValidatedEventArgs e)
        {
            calcula();
        }
        private void addCL(DSS.Controles.Impresion.DSSPrint printer, StringBuilder s, string text)
        {
            s.AppendLine(text);
            if (printer.PrinterSettings.PrinterName.Contains("Generic"))
                s.Append("\n");
        }

        private DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();

        private void imprimeGIFT(string texto, int cantDias)
        {
            StringBuilder lineas_impresion = new StringBuilder();
            addCL(printer, lineas_impresion, " ");
            addCL(printer, lineas_impresion, "                LIRIS S.A.");
            addCL(printer, lineas_impresion, "            RUC 0990865477001    ");
            addCL(printer, lineas_impresion, "      CONTRIBUYENTE ESPECIAL      ");
            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, "          NOTA DE CREDITO");
            addCL(printer, lineas_impresion, "      #" + lblSecuenciaNC.Text);
            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, "CED/RUC: " + lblCedula.Text);
            addCL(printer, lineas_impresion, " NOMBRE: " + lblNombre.Text);
            addCL(printer, lineas_impresion, "  FECHA: " + DateTime.Now.ToString());
            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, "CANT.   NOMBRE           VALOR    ");
            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, texto);
            addCL(printer, lineas_impresion, " VALOR: " + lblTotSel.Text);
            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, " ");
            printer.TextToPrint = lineas_impresion.ToString();

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "imprimeGIFT", $"lineas_impresion: {lineas_impresion}");
            printer.Print();


            lineas_impresion = new StringBuilder();
            addCL(printer, lineas_impresion, "                LIRIS S.A.");
            addCL(printer, lineas_impresion, "            RUC 0990865477001    ");
            addCL(printer, lineas_impresion, "      CONTRIBUYENTE ESPECIAL      ");
            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, "          TICKET POR DEVOLUCION");
            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, "CED/RUC: " + lblCedula.Text);
            addCL(printer, lineas_impresion, " NOMBRE: " + lblNombre.Text);
            addCL(printer, lineas_impresion, "  FECHA: " + DateTime.Now.ToString());
            addCL(printer, lineas_impresion, "==================================");


            string coded;
            string secuenciaNC = "000" + lblSecuenciaNC.Text.Replace("-", "");


            Barcode bc = new Barcode();
            coded = bc.encodeString("000" + lblSecuenciaNC.Text.Replace("-", ""));
            addCL(printer, lineas_impresion, "<barcode>" + "           " + secuenciaNC + "</barcode>");
            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, " VALOR: " + lblTotSel.Text);
            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, "   ___________________________");
            addCL(printer, lineas_impresion, "    " + lblNombre.Text);
            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, "Declaro que acepto la totalidad de");
            addCL(printer, lineas_impresion, "las políticas de emisión y fecha de");
            addCL(printer, lineas_impresion, "vigencia del presente instrumento");

            addCL(printer, lineas_impresion, "==================================");
            addCL(printer, lineas_impresion, " SE APLICARA EL VALOR TOTAL A UNA ");
            addCL(printer, lineas_impresion, "  NUEVA COMPRA DE IGUAL O MAYOR   ");
            addCL(printer, lineas_impresion, "   VALOR.    ");
            addCL(printer, lineas_impresion, "-Tiempo de vigencia " + cantDias.ToString() + " días contados");
            addCL(printer, lineas_impresion, " a partir de su emisión");
            addCL(printer, lineas_impresion, "-Personal e intransferible");
            addCL(printer, lineas_impresion, "-Aplica para compras en los ");
            addCL(printer, lineas_impresion, " establecimientos DELPORTAL.");
            addCL(printer, lineas_impresion, " No podrá ser canjeado por ");
            addCL(printer, lineas_impresion, " dinero efectivo");
            addCL(printer, lineas_impresion, "==================================");
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "imprimeGIFT", $"lineas_impresion: {lineas_impresion}");

            printer.TextToPrint = lineas_impresion.ToString();
            printer.Print();
        }

        private void imprimeNC(string texto, int cantDias, string recibo_NC, string ClaveAccesoSRI)
        {
            if (ClaveAccesoSRI == "")
            { ClaveAccesoSRI = null; }
            var sub1 = "";
            var sub2 = "";
            var pos = new POSEntities();

            if (string.IsNullOrEmpty(recibo_NC))
            {
                StringBuilder lineas_impresion = new StringBuilder();
                addCL(printer, lineas_impresion, " ");
                addCL(printer, lineas_impresion, "                LIRIS S.A.");
                addCL(printer, lineas_impresion, "            RUC 0990865477001    ");
                addCL(printer, lineas_impresion, "      CONTRIBUYENTE ESPECIAL      ");
                addCL(printer, lineas_impresion, "==================================");
                addCL(printer, lineas_impresion, "          NOTA DE CREDITO");
                addCL(printer, lineas_impresion, "      #" + lblSecuenciaNC.Text);
                addCL(printer, lineas_impresion, "==================================");
                addCL(printer, lineas_impresion, "CED/RUC: " + lblCedula.Text);
                addCL(printer, lineas_impresion, " NOMBRE: " + lblNombre.Text);
                addCL(printer, lineas_impresion, "  FECHA: " + DateTime.Now.ToString());
                addCL(printer, lineas_impresion, "==================================");
                addCL(printer, lineas_impresion, "CANT.   NOMBRE           VALOR    ");
                addCL(printer, lineas_impresion, "==================================");
                addCL(printer, lineas_impresion, texto);
                addCL(printer, lineas_impresion, " VALOR: " + lblTotSel.Text);
                addCL(printer, lineas_impresion, "==================================");
                addCL(printer, lineas_impresion, " ");
                printer.TextToPrint = lineas_impresion.ToString();

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "imprimeGIFT", $"lineas_impresion: {lineas_impresion}");
                printer.Print();
                return;
            }



            if (Control.Common.GlobalParameters.EstablecimientoDireccion.Length > 20)
            {
                int largo = Control.Common.GlobalParameters.EstablecimientoDireccion.Length;
                sub1 = Control.Common.GlobalParameters.EstablecimientoDireccion.Substring(0, 20);
                sub2 = Control.Common.GlobalParameters.EstablecimientoDireccion.Substring(20, Control.Common.GlobalParameters.EstablecimientoDireccion.Length - 20);
            }


            recibo_NC = recibo_NC.Replace("<<oficina>>", Control.Common.GlobalParameters.EstablecimientoNombre + Environment.NewLine + sub1 + Environment.NewLine + sub2);
            recibo_NC = recibo_NC.Replace("<<telefono>>", Control.Common.GlobalParameters.EstablecimientoTelefono);
            recibo_NC = recibo_NC.Replace("<<cajero>>", Control.Common.GlobalParameters.UsuarioNombre);
            recibo_NC = recibo_NC.Replace("<<nc_fecha>>", DateTime.Now.ToString("dd/MM/yyyy HH:MM"));
            recibo_NC = recibo_NC.Replace("<<direccion>>", lblDireccionCliente.Text);
            recibo_NC = recibo_NC.Replace("<<cliente_telefono>>", lblTelefonoCliente.Text);

            //Reemplazo clave de acceso
            System.Text.RegularExpressions.Regex regexAcc = new System.Text.RegularExpressions.Regex(@"<claveAcceso>(.*)\</claveAcceso>");
            StringBuilder claveAcc = new StringBuilder();
            if (ClaveAccesoSRI != null)
                claveAcc.AppendLine(Environment.NewLine + "" + ClaveAccesoSRI.Substring(0, 40) + Environment.NewLine + ClaveAccesoSRI.Substring(40, ClaveAccesoSRI.Length - 40));
            else
                claveAcc.AppendLine("");
            recibo_NC = regexAcc.Replace(recibo_NC, claveAcc.ToString());


            recibo_NC = recibo_NC.Replace("<<nota_credito>>", "NC-" + lblSecuenciaNC.Text);
            recibo_NC = recibo_NC.Replace("<<cedula>>", lblCedula.Text);
            recibo_NC = recibo_NC.Replace("<<cliente>>", lblNombre.Text);

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaItem>(.*)\</plantillaItem>");
            recibo_NC = regex.Replace(recibo_NC, texto.ToString());


            recibo_NC = recibo_NC.Replace("<<subtotal>>", Control.Common.StringHelper.DevolverConPadding(SubTotalSel.ToString("N2"), 65));
            recibo_NC = recibo_NC.Replace("<<descuentototal>>", Control.Common.StringHelper.DevolverConPadding(DesctSel.ToString("N2"), 65));   // CAMBIO AQUI  SE REALIZA 
            recibo_NC = recibo_NC.Replace("<<subtotaldos>>", Control.Common.StringHelper.DevolverConPadding(SubtotalDosSel.ToString("N2"), 65)); // NUEVO PARAMETOR EN LA IMPRESION 
            recibo_NC = recibo_NC.Replace("<<iva>>", Control.Common.StringHelper.DevolverConPadding(IvaSel.ToString("N2"), 65));


            if (_facturaActual.aplicaBeneficioDevolucionIVA)
            {
                recibo_NC = recibo_NC.Replace("<<montoIvaDevolver>>", _facturaActual.montoIvaDevolver.ToString());
            }

            decimal IvaDevolver = decimal.Parse(lblmontoIvaDevolver.Text);

            if (IvaDevolver != 0)
            {
                recibo_NC = recibo_NC.Replace("<<montoIvaDevolver>>", Control.Common.StringHelper.DevolverConPadding(lblmontoIvaDevolver.Text, 65));
            }



            recibo_NC = recibo_NC.Replace("<<total>>", Control.Common.StringHelper.DevolverConPadding(lblTotSel.Text, 65));
            recibo_NC = recibo_NC.Replace("<<total_pagar>>", lblTotSel.Text);
            recibo_NC = recibo_NC.Replace("<<motivo_nc>>", cmbMotivo.SelectedValue.ToString());
            recibo_NC = recibo_NC.Replace("<<factura>>", "F" + _factura.GetNumeroFacturaEnmascarado());



            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "imprimeGIFT", $"ticket recibo_NC: {recibo_NC}");
            Control.Common.Printer.Imprimir(recibo_NC, 3, 11);
        }

        private void imprimeGC(int cantDias, string recibo_GC)
        {



            StringBuilder lineas_impresion = new StringBuilder();
            string coded = string.Empty;
            Barcode bc = new Barcode();

            try
            {

                if (string.IsNullOrEmpty(recibo_GC))
                {

                    addCL(printer, lineas_impresion, "                LIRIS S.A.");
                    addCL(printer, lineas_impresion, "            RUC 0990865477001    ");
                    addCL(printer, lineas_impresion, "      CONTRIBUYENTE ESPECIAL      ");
                    addCL(printer, lineas_impresion, "==================================");
                    addCL(printer, lineas_impresion, "          TICKET POR DEVOLUCION");
                    addCL(printer, lineas_impresion, "==================================");
                    addCL(printer, lineas_impresion, "CED/RUC: " + lblCedula.Text);
                    addCL(printer, lineas_impresion, " NOMBRE: " + lblNombre.Text);
                    addCL(printer, lineas_impresion, "  FECHA: " + DateTime.Now.ToString());
                    addCL(printer, lineas_impresion, "==================================");



                    string secuenciaNC = "000" + lblSecuenciaNC.Text.Replace("-", "");


                    coded = bc.encodeString("000" + lblSecuenciaNC.Text.Replace("-", ""));
                    addCL(printer, lineas_impresion, "<barcode>" + "           " + secuenciaNC + "</barcode>");
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, " VALOR: " + lblTotSel.Text);
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "   ___________________________");
                    addCL(printer, lineas_impresion, "    " + lblNombre.Text);
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "Declaro que acepto la totalidad de");
                    addCL(printer, lineas_impresion, "las políticas de emisión y fecha de");
                    addCL(printer, lineas_impresion, "vigencia del presente instrumento");

                    addCL(printer, lineas_impresion, "==================================");
                    addCL(printer, lineas_impresion, " SE APLICARA EL VALOR TOTAL A UNA ");
                    addCL(printer, lineas_impresion, "  NUEVA COMPRA DE IGUAL O MAYOR   ");
                    addCL(printer, lineas_impresion, "   VALOR.    ");
                    addCL(printer, lineas_impresion, "-Tiempo de vigencia " + cantDias.ToString() + " días contados");
                    addCL(printer, lineas_impresion, " a partir de su emisión");
                    addCL(printer, lineas_impresion, "-Personal e intransferible");
                    addCL(printer, lineas_impresion, "-Aplica para compras en los ");
                    addCL(printer, lineas_impresion, " establecimientos DELPORTAL.");
                    addCL(printer, lineas_impresion, " No podrá ser canjeado por ");
                    addCL(printer, lineas_impresion, " dinero efectivo");
                    addCL(printer, lineas_impresion, "==================================");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "imprimeGIFT", $"lineas_impresion: {lineas_impresion}");

                    printer.TextToPrint = lineas_impresion.ToString();
                    printer.Print();

                    return;
                }



                addCL(printer, lineas_impresion, " ");
                recibo_GC = recibo_GC.Replace("<<cedula>>", lblCedula.Text);
                recibo_GC = recibo_GC.Replace("<<cliente>>", lblNombre.Text);
                recibo_GC = recibo_GC.Replace("<<fecha>>", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                recibo_GC = recibo_GC.Replace("<<total>>", lblTotSel.Text);
                recibo_GC = recibo_GC.Replace("<<valor>>", lblTotSel.Text);
                recibo_GC = recibo_GC.Replace("<<vigencia>>", cantDias.ToString());
                recibo_GC = recibo_GC.Replace("<<diasVigencia>>", cantDias.ToString());


                string recibo_GC_Ini = string.Empty;
                string recibo_GC_End = string.Empty;

                recibo_GC_Ini = string.Empty;
                recibo_GC_End = string.Empty;

                string codigoBarras = "000" + lblSecuenciaNC.Text.Replace("-", "").Trim();
                recibo_GC = recibo_GC.Replace("<<diasVigencia>>", cantDias.ToString());
                recibo_GC = recibo_GC.Replace("<<codigo>>", codigoBarras.ToString());
                recibo_GC = recibo_GC.Replace("<<secuenciaNC>>", codigoBarras.ToString());

                //recibo_GC_End = recibo_GC.Substring(recibo_GC.IndexOf("<barcode>") + 11, recibo_GC.Length - (recibo_GC.IndexOf("<barcode>") + 11));
                //addCL(printer, lineas_impresion, recibo_GC_End.ToString());

                //recibo_GC = recibo_GC.Replace("<barcode>", codigoBarras.ToString());

                //recibo_GC = recibo_GC.Replace("<<codigo>>", codigoBarras.ToString());

                //printer.TextToPrint = lineas_impresion.ToString();
                //printer.TextToPrint = recibo_GC.ToString();
                addCL(printer, lineas_impresion, recibo_GC.ToString());
                printer.TextToPrint = lineas_impresion.ToString();
                //printer.Print();
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pago.FormaNotaCredito", "imprimeGIFT", $"ticket recibo_GC: {recibo_GC}");
                Control.Common.Printer.Imprimir(recibo_GC, 3, 11);
            }
            catch (Exception)
            {

                throw;
            }



        }

        private void gridItems_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (e.Column.Name == "chk")
            {
                // Ejecutar después de que se complete el click
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    gridItems.EndEdit();
                    calcula();
                }));
            }
        }

        private void gridItems_CellPaint(object sender, GridViewCellPaintEventArgs e)
        {

        }

        //private void gridItems_CellValueChanged(object sender, GridViewCellEventArgs e)
        //{
        //    if (e.Column.Name == "chk") // o usa e.ColumnIndex == 0 si es la primera columna
        //    {
        //        var fila = e.Row;
        //        var producto = fila.DataBoundItem as Models.Producto;

        //        bool valorCheck = Convert.ToBoolean(fila.Cells["chk"].Value);


        //        calcula(); // Llamada a tu método de recalculo
        //    }
        //}

        private void gridItems_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            if (e.Column.Name == "chk" || e.Column.FieldName == "chk")
            {
                // Forzar que se confirme el cambio inmediatamente
                gridItems.EndEdit();

                var fila = e.Row;
                var producto = fila.DataBoundItem as Models.Producto;

                if (producto != null)
                {
                    bool valorCheck = Convert.ToBoolean(fila.Cells["chk"].Value ?? false);

                    // Marcar el producto como ajustado según el estado del checkbox
                    producto.esAjustado = valorCheck;
                }

                // Recalcular inmediatamente
                calcula();

                // Forzar actualización visual
                gridItems.Refresh();
            }
        }

        private void gridItems_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            if (e.Column.Name == "chk")
            {
                calcula();
            }
        }


        private void gridItems_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {

            //if(e.Column.FieldName== "Cantidad")
            //{
            //    var text = focused as  Telerik.WinControls.UI.RadTextBox;
            //    if (text != null)
            //    {
            //        KeyboardControl kbd = new KeyboardControl(text, this.Text);
            //        kbd.Top = this.Height + this.Top - 50;
            //        kbd.Left = this.Left - 200;
            //        kbd.ShowDialog();
            //        //code = kbd.Tecla;
            //        //this.Close();
            //    }
            //}
            calcula();
        }

        private void lblCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                using (POSEntities db = new POSEntities())
                {
                    var cliente = db.pos_customer.Where(x => x.ACCOUNTNUM == lblCedula.Text).FirstOrDefault();
                    if (cliente != null)
                    {
                        lblCedula.Text = cliente.ACCOUNTNUM;
                        lblNombre.Text = cliente.NAME;
                        lblDireccionCliente.Text = cliente.ADDRESS;
                        lblTelefonoCliente.Text = cliente.PHONE;

                    }
                    else
                    {
                        MessageBox.Show(this, "Cliente No Existe");
                        lblCedula.Text = null;
                        lblNombre.Text = null;
                        lblDireccionCliente.Text = null;
                        lblTelefonoCliente.Text = null;
                    }
                }
            }
        }



        private void btnKb_Click(object sender, EventArgs e)
        {

            //Control.Common.General.TecladoPantalla();
            Control.ToolBox.frmTecladoCompleto frmTeclado = new ToolBox.frmTecladoCompleto();
            frmTeclado.ShowDialog();

        }

        private void txtPtoEmision_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
            if (txtNum.Text != string.Empty && txtPtoEmision.Text != _factura.PtoEmision)
            {
                btnBuscarFact_Click(sender, e);
            }

        }

        private void txtNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)Keys.Tab && e.KeyChar != (char)Keys.Enter && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscarFact_Click(sender, e);
            }
        }

        private void txtPtoEmision_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)Keys.Tab && e.KeyChar != (char)Keys.Enter && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void txtNum_Leave(object sender, EventArgs e)
        {

            if (txtPtoEmision.Text != string.Empty)
            {
                btnBuscarFact_Click(sender, e);
            }
        }


        public void grabarAcumulaCompraGratis(int idNC)
        {
            using (POSEntities db = new POSEntities())
            {
                SqlParameter paramResult = new SqlParameter("@respuesta", "");
                paramResult.Direction = System.Data.ParameterDirection.Output;

                var addParameters = new List<SqlParameter>
                 {
                    new SqlParameter("@idNC", idNC),
                    paramResult
                 };

                db.Database.ExecuteSqlCommand("PtsCliente.spAcumularCompraGratisNC @idNC, @respuesta out", addParameters.ToArray());
                string response = (string)paramResult.Value;
            }
        }



        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    this.Close();
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtNum_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblCedula_TextChanged(object sender, EventArgs e)
        {

        }

        private void gridItems_Click(object sender, EventArgs e)
        {
            //calcula();
        }
    }
}