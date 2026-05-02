using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using POS.Models;
using Trx.Messaging;
using POS.Control.PINPAD;
using Telerik.WinControls.UI;
using POS.Control.Clientes;
using System.IO;
using System.Data.SqlClient;
using POS.Control;
using System.Globalization;
using POS.Control.CajaPinpad.Modelo;
using MensajesLibrary;
using Liris_BasePagoDLL;
using System.Reflection;
using POS.Models.Monedero;

namespace POS.Control.Pagos
{
    public partial class BasePagos : Telerik.WinControls.UI.RadForm
    {
        System.Windows.Forms.Control focused;
        public enum PagoTipo
        {
            TarjetaCredito,
            Cheque,
            TarjetaRegalo,
            TarjetaInterna,
            Retencion,
            NotaCredito,
            DineroElectronico,
            Efectivo,
            CompraGratis  // jchid agregado para la nueva modalidad de compra gratis 
        }

        public PagoTipo _pagoTipo;
        Interfaces.ISale _factura;
        Factura _facturaApp = null;
        TarjetaRegalo _tarjetaRegalo;
        NotaCredito _notaCredito;
        core_giftcard _Giftcard;
        core_notacredito _Notacredito;
        private TomaPeso scanner;


        int PUERTOCOM;
        bool EsPagoOkPromoTarjeta = false;
        int EsManual;
        bool tarjetaDetectadaPagoManual;
        private string _numTarjeta;
        private string _tarjetaHabiente;
        InputLanguage original;
        public core_tarjetacreditointerno _tarjetaInterna;
        public core_tarjetacreditointerno _tarjetaInternaAdicional;
        bool _esPedidoDomicilio = false;
        string _ordenApp = string.Empty;
        public bool esAppMovil = false;
        private DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();
        private bool EsPinPad = false;
        private bool _EsUsoAppMovil = false;
        public decimal valorRestante = 0;
        private bool flagEjecutaPago = false;
        private bool flagTarjetaValida = false;
        private bool _isUpdatingText = false;
        private string _textoOriginal = string.Empty;
        private string ValorOriginalTarjeta = string.Empty;


        private Timer _enforceFocusTimer;

        public decimal DescuentoPromoTarjBines = 0;
        public bool aplicaDsctoPromoTarjetaBines = false;
        public string binTarjetaPromoDscto = string.Empty;
        public MainWindow _mainWindow;
        private bool _compraGratisCalculada = false;
        public bool EsUsoAppMovil
        {
            get { return _EsUsoAppMovil; }
        }

        private bool _debeActualizarClienteFactura = false;
        public bool DebeActualizarClienteFactura
        {
            get { return _debeActualizarClienteFactura; }
        }

        private string _identificacionActualizarClienteFactura = string.Empty;
        public string IdentificacionActualizarClienteFactura
        {
            get { return _identificacionActualizarClienteFactura; }
        }

        private string _nombreActualizarClienteFactura = string.Empty;
        public string NombreActualizarClienteFactura
        {
            get { return _nombreActualizarClienteFactura; }
        }
        public string numTarjeta
        {
            get { return _numTarjeta; }
        }
        public string tarjetaHabiente
        {
            get { return _tarjetaHabiente; }
        }
        private BasePagos _basePagoInstance;


        private const int WS_EX_COMPOSITED = 0x02000000;
        private StringBuilder _barcodeBuffer = new StringBuilder();
        private Timer _resetTimer;
        public decimal saldoFacturaPago = 0;

        // Al inicio de la clase BasePagos, junto a las otras variables
        public static string _tarjetaCompraGratisEnUso = string.Empty;
        public static string _facturaCompraGratisEnUso = string.Empty;



        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED; // Forzar dibujo doble buffer para todo el formulario y sus controles hijos
                return cp;
            }
        }

        private void InicializarControlesPorTipoPago()
        {
            switch (_pagoTipo)
            {
                case PagoTipo.TarjetaInterna:
                    _tarjetaInterna = _factura.TarjetaCreditoInterno;
                    _tarjetaInternaAdicional = _factura.TarjetaCreditoInternoAdicional;
                    EsManual = 0;
                    tarjetaDetectadaPagoManual = false;
                    txtCuenta.TabStop = true;
                    break;

                case PagoTipo.DineroElectronico:
                    EsManual = 0;
                    txtCuenta.TabStop = true;
                    break;

                default:

                    txtCuenta.TabStop = false;
                    break;
            }
        }


        private void EstablecerFocoInicial()
        {
            if (txtCuenta.Visible && txtCuenta.Enabled)
                txtCuenta.Focus();
            else if (txtValor.Visible && txtValor.Enabled)
                txtValor.Focus();
        }

        public void MostrarCentradoEnPantalla(Screen screen)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                screen.Bounds.Left + (screen.Bounds.Width - this.Width) / 2,
                screen.Bounds.Top + (screen.Bounds.Height - this.Height) / 2
            );
        }


        public void TraerAFocoYActualizar()
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            this.BringToFront();
            this.Activate();
            this.Focus();
        }


        public BasePagos()
        {
            InicializarBuffer();
        }


        private void InicializarBuffer()
        {

            _resetTimer = new Timer { Interval = 100 };
            _resetTimer.Tick += (s, e) =>
            {
                _resetTimer.Stop();
                _barcodeBuffer.Clear();
            };
        }

        public BasePagos(PagoTipo p, ref Models.Factura f, decimal val = 0M)
        {
            decimal TotalPagar = decimal.Parse((from deta in f.Productos
                                                select deta).Sum(prod => prod.Total).ToString());


            InitializeComponent();

            InicializarBuffer();

            _pagoTipo = p;

            string tituloOpcion = string.Empty;

            switch (_pagoTipo)
            {
                case PagoTipo.TarjetaCredito:
                    tituloOpcion = "BasePagos - MEDIANET";
                    if (Control.Common.GlobalParameters.PINPAD_MULTIRED)
                    {
                        tituloOpcion = "BasePagos - PINPAD_MULTIRED (ACTIVO)";
                    }
                    break;

                case PagoTipo.Cheque:
                    tituloOpcion = "BasePagos - Cheque"; break;

                case PagoTipo.TarjetaRegalo:
                    tituloOpcion = "BasePagos - GiftCard "; break;
                case PagoTipo.NotaCredito:
                    tituloOpcion = "BasePagos - Nota de Crédito "; break;
                case PagoTipo.TarjetaInterna:
                    tituloOpcion = "BasePagos - Tarjeta Empresarial"; break;
                case PagoTipo.DineroElectronico:
                    tituloOpcion = "BasePagos - Dinero Electrónico "; break;
                default:
                    tituloOpcion = "BasePagos";
                    break;
            }


            this.Text = tituloOpcion;

            _factura = f;
            _facturaApp = f;

            //val = TotalPagar;
            BasePagosIniciar(p, val);
        }



        public BasePagos(PagoTipo p, ref Models.Giftcard.ClsGiftcardSale f, decimal val = 0M)
        {
            InitializeComponent();
            _factura = f;

            BasePagosIniciar(p, val);
        }

        public BasePagos(PagoTipo p, ref Models.TarjeEmpresa.ClsTarjetaEmpresaPagos f, decimal val = 0M)
        {
            InitializeComponent();
            _factura = f;
            BasePagosIniciar(p, val);
        }

        private void EnforceFocusTimer_Tick(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.Visible) return;

            if (!this.Focused && !this.ContainsFocus)
            {
                this.TopMost = true;
                this.Focus();
                this.Activate();
                this.TopMost = false;
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            return;
            base.OnDeactivate(e);

            // Intentar recuperar el foco si se pierde
            this.BeginInvoke((MethodInvoker)(() =>
            {
                if (!this.Focused)
                {
                    this.TopMost = true;
                    this.Focus();
                    this.Activate();
                    this.TopMost = false;
                }
            }));
        }

        private void BasePagos_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Limpiar timer al cerrar para evitar fugas de memoria
            if (_enforceFocusTimer != null)
            {
                _enforceFocusTimer.Stop();
                _enforceFocusTimer.Dispose();
                _enforceFocusTimer = null;
            }
        }



        public void BasePagosIniciar(PagoTipo p, decimal val = 0M)
        {

            _pagoTipo = p;
            _tarjetaInterna = _factura.TarjetaCreditoInterno;
            _tarjetaInternaAdicional = _factura.TarjetaCreditoInternoAdicional;
            this.txtValor.Text = val.ToString();
            saldoFacturaPago = decimal.Parse(val.ToString());


            EsManual = 0;
            tarjetaDetectadaPagoManual = false;

        }



        private void btnEvent(object sender, EventArgs e)
        {
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                var button = sender as System.Windows.Forms.Control;
                text.Text = text.Text + button.Text;
                text.Focus();
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                if (text.Text.Length > 1)
                    text.Text = text.Text.Substring(0, text.Text.Length - 1);
                else
                    text.Text = "";
            }
        }


        private bool validarTarjetaCredito()
        {
            bool _ret = true;
            string numAutori = string.Empty, numTransaccion = string.Empty;
            var obj = cmbBancoTarjeta.SelectedValue as core_tarjetacredito_bin;
            //valida si no pasa la tarjeta, entonces obtiene el dato del combo de bancos y tipo.
            if (obj == null)
            {
                var obj1 = cmbBancoTarjeta.SelectedValue as core_tarjetacredito;
                return obj1 != null;
            }



            return obj != null;
        }
        /// <summary>
        /// Valida que esté lleno la información del Número de Transacción y Numero de Autorización.
        /// </summary>
        /// <returns></returns>
        private bool validaDatosVoucherPManual()
        {
            bool _ret = true;
            string numAutori = string.Empty, numTransaccion = string.Empty;

            numAutori = txtNumAutorizacionVoucher.Text.Replace("_", string.Empty);
            numTransaccion = txtNumTransaccionVoucher.Text.Replace("_", string.Empty);
            try
            {
                if (Control.Common.GlobalParameters.ActivarFormaPagoTC)
                {
                    if (!_esPedidoDomicilio)
                    {

                        if (numAutori.Length == 0 || numTransaccion.Length == 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        txtNumTransaccionVoucher.Text = "000000";
                        txtNumAutorizacionVoucher.Text = "000000";
                    }


                }
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }

        private bool validarCheque()
        {
            var result = false;
            var obj = cmbBancoTarjeta.SelectedValue as core_banco;
            result = obj != null;
            if (txtNumCheque.Text.Length == 0 || string.IsNullOrEmpty(txtNumCheque.Text))
            {
                result = false;
            }
            if (txtCuenta.Text.Length == 0 || string.IsNullOrEmpty(txtCuenta.Text))
            {
                result = false;
            }
            return result;
        }

        private bool validarTarjetaRegalo(decimal valor, decimal valorGiftCard = 0)
        {
            var result = false;

            if (_tarjetaRegalo != null)
            {
                result = (_tarjetaRegalo.tarjetaValida() && _tarjetaRegalo.getSaldo() >= valor) || (_tarjetaRegalo.tarjetaValidaGiftCard() && valorGiftCard > 0);

                if (!_tarjetaRegalo.tarjetaValida() && !_tarjetaRegalo.tarjetaValidaGiftCard())  //JCanarte 22Mar2021 Validar tambien que tenga Gift matriculadas
                {
                    Control.Common.General.GetMensajeToList(179);
                }

                if (_tarjetaRegalo.getSaldo() < valor && valorGiftCard < valor) //JCanarte 22Mar2021 Validar tambien que tenga saldo Gift matriculadas
                {
                    Control.Common.General.GetMensajeToList(180);
                }
            }
            else
            {
                result = false;
            }

            return result;
        }


        private bool validarNotaCredito(decimal valor)
        {
            var result = false;
            if (_notaCredito != null)
            {
                result = _notaCredito.tarjetaValida() && _notaCredito.getSaldo() >= valor;
                if (!_notaCredito.tarjetaValida())
                {
                    Control.Common.General.GetMensajeToList(181);

                }
                if (_notaCredito.getSaldo() < valor)
                {
                    Control.Common.General.GetMensajeToList(182);
                }

                if (_notaCredito.getSaldo() != valor || _factura.GetTotal() < _notaCredito.getSaldo())
                {
                    Control.Common.General.GetMensajeToList(183);
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }


        private bool validarTarjetaInterna(decimal valor)
        {
            try
            {
                bool result;

                if (string.IsNullOrEmpty(txtCuenta.Text))
                {
                    Control.Common.General.GetMensajeToList(637);
                    return false;
                }


                if (_tarjetaInternaAdicional != null)
                {
                    if (string.IsNullOrEmpty(_tarjetaInternaAdicional.codigo) || string.IsNullOrEmpty(txtCuenta.Text))
                    {

                        Control.Common.General.GetMensajeToList(637);
                        return false;
                    }
                    result = _tarjetaInternaAdicional.saldo >= valor;
                    if (!result)
                    {
                        //MessageBox.Show(this, "No tiene suficiente saldo en su tarjeta adicional!");
                        Control.Common.General.GetMensajeToList(184);
                        return false;
                    }
                    if (!_tarjetaInternaAdicional.activo)
                    {
                        //MessageBox.Show(this, "Tarjeta adicional Inactiva");
                        //Control.Common.General.GetMensajeToList(185);
                        Control.Common.General.GetMensajeToList(185);
                        return false;
                    }
                    if (_tarjetaInterna.saldo < valor)
                    {
                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

                        parametros.Add(new ParametrosMensajes() { codigo = "[nombre_tarjeta]", valor = _tarjetaInterna.nombre_tarjeta });
                        parametros.Add(new ParametrosMensajes() { codigo = "[saldo_empleado]", valor = _tarjetaInterna.saldo.ToString("N2") });
                        Control.Common.General.GetMensajeToList(186, parametros);
                        //Control.Common.General.GetMensajeToListv2(this, 186, parametros);


                        //MessageBox.Show(this, "La tarjeta de empleado principal no cuenta con suficiente saldo para cubrir el pago solicitado. Empleado: '" + _tarjetaInterna.nombre_tarjeta + "' Saldo actual empleado: '" + _tarjetaInterna.saldo.ToString("N2") + "'!");
                        return false;
                    }
                    if (!_tarjetaInterna.activo)
                    {
                        //MessageBox.Show(this, "La tarjeta de empleado principal se encuentra inactiva");
                        Control.Common.General.GetMensajeToList(187);
                        return false;
                    }
                }
                else
                {
                    if (_tarjetaInterna == null)
                    {
                        Control.Common.General.GetMensajeToList(612);
                        return false;
                    }


                    result = _tarjetaInterna.saldo >= valor;
                    if (!result)
                    {
                        // MessageBox.Show(this, "No tiene suficiente saldo!");
                        // Control.Common.General.GetMensaje("POS", "No tiene suficiente saldo", "I");
                        Control.Common.General.GetMensajeToList(188);

                    }
                    if (!_tarjetaInterna.activo)
                    {
                        // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Tarjeta Inactiva", " ");
                        // Control.Common.General.GetMensaje("POS", "Tarjeta Inactiva", "I");
                        Control.Common.General.GetMensajeToList(189);
                        //MessageBox.Show(this, "Tarjeta Inactiva");
                        return false;
                    }
                }


                return result;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "validarTarjetaInterna", "Se presentaron novedades durante la validación d la tarjeta interna, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //MessageBox.Show(this, "No ha cargado tarjeta interna");

                //Control.Common.General.GetMensaje("POS", "No ha cargado tarjeta interna", "I");
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No ha cargado tarjeta interna", " ");
                Control.Common.General.GetMensajeToList(190);

                return false;
            }
        }

        private void prCambioCadenaConexion(string srvSelect, string srvPedidos)
        {
            try
            {
                string yourConnection = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"].ConnectionString.Replace(srvSelect, srvPedidos);
                //dcon = new POSEntities(yourConnection);
                var DBCS = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"];
                var writable = typeof(System.Configuration.ConfigurationElement).GetField("_bReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                writable.SetValue(DBCS, false);
                DBCS.ConnectionString = yourConnection;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
            }
            catch (Exception ex)
            {
            }

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
        }
        private bool validarMonedero(decimal valor)
        {
            var result = false;
            try
            {
                //if (Control.Common.GlobalParameters.MonederoPorcentajeConsumo <= 0)
                //{
                //    MessageBox.Show(this, "No existe el parámetro porcentaje de consumo", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return false;
                //}                
                //decimal maxconsumo = decimal.Round(_factura.GetTotal() * (Control.Common.GlobalParameters.MonederoPorcentajeConsumo / 100),2);
                //if(maxconsumo < valor)
                //{
                //    MessageBox.Show(this, "El valor excede al " + decimal.Round(Control.Common.GlobalParameters.MonederoPorcentajeConsumo) + "% ($ " + maxconsumo + ") máximo de consumo del total de la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return false;
                //}

                result = _factura.GetSaldoMonedero() >= valor;
                if (_factura.GetSaldoMonedero() < valor)
                {
                    Control.Common.General.GetMensajeToList(191);
                    // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "No tiene suficiente saldo!", "POS - Validación Monedero");
                    // Control.Common.General.GetMensaje("POS", " tiene suficiente saldo!", "I");
                    // MessageBox.Show(this, "No tiene suficiente saldo!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "validarMonedero", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }

            return result;
        }


        private void prepararVoucherTarjeta(string tipo_voucher, Tramas.RespuestaProcesoPago trama, Tramas.ProcesaPago pp, ref string response, bool esCopia = false)
        {
            try
            {
                var db = new POSEntities();

                var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
                recipe.NomTarjeta = trama.nomGruTar;
                recipe.MID = pp.MID;  //pp.MID;
                recipe.MID2 = trama.merchantId;
                recipe.TID = pp.TID;

                recipe.NumTarjeta = trama.numTarTuncate;
                recipe.NumLote = trama.numerolote;
                recipe.Adquiriente = trama.nomBancoAdq;
                recipe.Aprobacion = pp.TipoTransaccion == "03" ? pp.numAutorizacion : trama.numAut;
                recipe.Secuencial = trama.secuencialtransaccion;
                recipe.NombreTarjetaHabiente = trama.nombreTarjetaHabiente;
                recipe.FechaTrans = trama.fechaTrans.Substring(0, 4) + "/" + trama.fechaTrans.Substring(4, 2) + "/" + trama.fechaTrans.Substring(6, 2);
                recipe.HoraTrans = trama.horaTrans.Substring(0, 2) + ":" + trama.horaTrans.Substring(2, 2) + ":" + trama.horaTrans.Substring(4, 2);
                recipe.VenTarjeta = "XX/XXXX";
                //Medianet indica que debe imprimirse enmascarado.     JM  31-08-2020
                //recipe.VenTarjeta = trama.codigoRed == "02" ? trama.fechaVencTar.Substring(0, 2) + "/" + trama.fechaVencTar.Substring(2, 2) : "XX/XX";
                recipe.Factura = pp.FacturaComprobante;
                var valoranulacion = "";
                if (pp.TipoTransaccion == "03")
                {
                    valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion).FirstOrDefault().VALORCONSUMO;
                    valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00");
                }

                //decimal valorinteres = trama.valInteres == "            " ? 0 : Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2));


                decimal valorinteres;
                if (cmbBancoTarjeta.Text.Contains("DIFERIDO CON INTERESES"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "prepararVoucherTarjeta", "VALOR INTERES:" + trama.valInteres);
                    // recipe.Intereses = Decimal.Parse("0.00").ToString("###,##0.00");
                    try
                    {
                        decimal valorinteres1 = trama.valInteres == "             " ? 0 : Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2));
                        valorinteres = valorinteres1;
                        //recipe.Intereses = Decimal.Parse(trama.valInteres).ToString("###,##0.00");
                        //recipe.Intereses = valorinteres1.ToString("###,##0.00");
                    }
                    catch (Exception ex2)
                    {
                        valorinteres = 0;
                        //recipe.Intereses = Decimal.Parse("0.00").ToString("###,##0.00");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "prepararVoucherTarjeta", "Error :" + ex2.Message);
                    }
                }
                else
                {
                    valorinteres = 0;
                }

                recipe.ValorTotal = pp.TipoTransaccion == "03" ? valoranulacion : (Decimal.Parse(pp.montoTotalTransaccion.Substring(0, 10) + "." + pp.montoTotalTransaccion.Substring(10, 2)) + valorinteres).ToString("###,##0.00");
                string modolectura = "";
                switch (trama.modoLectura)
                {
                    case "01":
                        modolectura = "Manual";
                        break;
                    case "02":
                        modolectura = "Banda";
                        break;
                    case "03":
                        modolectura = "Chip";
                        break;
                    case "04":
                        modolectura = "Fallback Manual (Chip)";
                        break;
                    case "05":
                        modolectura = "Fallback Banda (Chip) ";
                        break;
                    case "07":
                        modolectura = "Contactless";
                        break;
                }
                recipe.ModoLectura = modolectura;

                if (pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
                {
                    recipe.BaseIva = Decimal.Parse(pp.montoBaseGravaIVa.Substring(0, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)).ToString("###,##0.00");
                    recipe.BaseSinIva = Decimal.Parse(pp.montoBaseNoGravaIVa.Substring(0, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString("###,##0.00");
                    recipe.Subtotal = (Decimal.Parse(pp.montoBaseGravaIVa.Substring(0, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)) + Decimal.Parse(pp.montoBaseNoGravaIVa.Substring(0, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2))).ToString("###,##0.00");
                    recipe.Iva = Decimal.Parse(pp.impuestoIvaTransaccion.Substring(0, 10) + "." + pp.impuestoIvaTransaccion.Substring(10, 2)).ToString("###,##0.00");
                }
                else
                {
                    recipe.BaseIva = "";
                    recipe.BaseSinIva = "";
                    recipe.Subtotal = "";
                    recipe.Iva = "";
                }

                // Nuevo Autorizador "AUSTRO".  JM  04-12-2020
                //recipe.CodigoRed = trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST";
                if (trama.codigoRed == "01")
                    recipe.CodigoRed = "DATAFAST";
                else if (trama.codigoRed == "02")
                    recipe.CodigoRed = "MEDIANET";
                else if (trama.codigoRed == "03")
                    recipe.CodigoRed = "AUSTRO";
                // Nuevo Autorizador "AUSTRO".  JM  04-12-2020

                string tipodebcred = "";
                if (trama.nomGruTar.Contains("DEBIT"))
                {
                    tipodebcred = "DEBITO";
                    recipe.Pagare = " ";
                }
                else
                {
                    if (cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                        tipodebcred = "ROTATIVO";
                    else
                        tipodebcred = "" + cmbBancoTarjeta.Text + "\n\n                              PLAZO MESES: " + cmbDiferido.Text + (cmbMesesGracia.Text != "" ? "\n                              " + "MESES DE GRACIA:" + cmbMesesGracia.Text : "");
                }
                if (pp.TipoTransaccion != "03")
                    recipe.TipoDebCredito = tipodebcred;

                recipe.Intereses = valorinteres.ToString("###,##0.00");
                recipe.Arqc = trama.ARQC;
                recipe.Aidemv = trama.AIDEMV.Trim();
                recipe.Emv = trama.idEMV;
                recipe.Tc = trama.tipoCritoyValorEMV;
                recipe.Publicidad = trama.mensajePremioPublicidad;
                recipe.TVR = trama.TVR;
                recipe.TSI = trama.TSI;

                decimal ValorVoucher = 0.0M;
                ValorVoucher = decimal.Parse(recipe.ValorTotal);

                Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe, esCopia);
            }
            catch (Exception ex)
            {
                response = Common.ExceptionHandler.GetExceptionMessages(ex) + Environment.NewLine + "Stacktrace " + ex.StackTrace;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "preparaVoucherTrajeta", "error: " + response);

            }
        }
        private void prepararVoucherTarjeta(RepuestaPago Pago, string tipo_voucher, Tramas.RespuestaProcesoPago trama, Tramas.ProcesaPago pp, ref string response, bool esCopia = false)
        {
            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "prepararVoucherTarjeta", "inicia prepararVoucherTarjeta");

                var db = new POSEntities();

                var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
                recipe.NomTarjeta = Pago.GrupoTarjeta.Trim();
                recipe.MID = pp.MID;  //pp.MID;
                recipe.MID2 = Pago.MerchantID;
                recipe.TID = pp.TID;

                recipe.NumTarjeta = Pago.NumTrajetaPayClub_DBPWallet.Trim();
                recipe.NumLote = Pago.NumLote;
                recipe.Adquiriente = Pago.NombBcoAdq; // trama.nomBancoAdq;
                recipe.Aprobacion = pp.TipoTransaccion == "03" ? pp.numAutorizacion : Pago.NumAutorizacion;
                recipe.Secuencial = Pago.SecTrans.ToString();// trama.secuencialtransaccion;
                recipe.NombreTarjetaHabiente = Pago.NombTarjetaHabiente.Trim();// trama.nombreTarjetaHabiente;

                string Anio = Pago.FechaTrans.ToString().Substring(0, 4);
                string Mes = Pago.FechaTrans.ToString().Substring(4, 2);
                string Dia = Pago.FechaTrans.ToString().Substring(6, 2);

                string HH = Pago.HoraTrans.ToString().Substring(0, 2);
                string mm = Pago.HoraTrans.ToString().Substring(2, 2);
                string ss = Pago.HoraTrans.ToString().Substring(4, 2);


                recipe.FechaTrans = string.Concat(Anio, "/", Mes, "/", Dia);
                recipe.HoraTrans = string.Concat(HH, ":", mm, ":", ss);
                recipe.VenTarjeta = "XX/XXXX";
                recipe.Factura = _factura.GetNumeroFacturaEnmascarado();

                var valoranulacion = "";
                var valorVoucher = "";
                var valorinteres = "";
                var valorT = "";
                var VALORCONSUMO = string.Empty;
                decimal valorTotal = 0;
                string secuencialTransaccion = Pago.SecTrans.ToString().Trim().PadLeft(6, '0');
                POS_VOUCHER voucher = new POS_VOUCHER();



                var voucherList = (from deta in db.POS_VOUCHER
                                   where deta.AUTORIZACION == Pago.NumAutorizacion.ToString()
                                   && deta.NUMEROVOUCHER == secuencialTransaccion
                                   && deta.FACTURA == recipe.Factura
                                   select deta).ToList();

                if (voucherList.Count > 0) { voucher = voucherList.FirstOrDefault(); }

                VALORCONSUMO = voucher.VALORCONSUMO;
                valoranulacion = voucher.VALORCONSUMO;
                valorinteres = voucher.VALORINTERES;
                if (pp.TipoTransaccion == "03")
                {
                    valoranulacion = (Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)) * -1).ToString("###,##0.00");
                    valorinteres = (Decimal.Parse(valorinteres.Substring(0, 11) + "." + valorinteres.Substring(11, 2)) * -1).ToString("###,##0.00");
                    valorT = (decimal.Parse(valoranulacion)).ToString("###,##0.00");
                    valoranulacion = valorT;
                    valorVoucher = valorT;
                }
                else
                {
                    decimal valorConsumo = Decimal.Parse(VALORCONSUMO.Substring(0, 11) + "." + VALORCONSUMO.Substring(11, 2));
                    decimal valInt = Decimal.Parse(valorinteres.Substring(0, 11) + "." + valorinteres.Substring(11, 2));
                    valorTotal = valorConsumo + valInt;
                    valorVoucher = (valorTotal).ToString("###,##0.00");
                }

                recipe.ValorTotal = valorVoucher;
                string modolectura = "";
                switch (Pago.ModLectura)
                {
                    case "01":
                        modolectura = "Manual";
                        break;
                    case "02":
                        modolectura = "Banda";
                        break;
                    case "03":
                        modolectura = "Chip";
                        break;
                    case "04":
                        modolectura = "Fallback Manual (Chip)";
                        break;
                    case "05":
                        modolectura = "Fallback Banda (Chip) ";
                        break;
                    case "07":
                        modolectura = "Contactless";
                        break;
                }

                recipe.ModoLectura = modolectura;

                if (pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
                {
                    decimal BaseIva = 0;
                    decimal BaseSinIva = 0;
                    decimal Subtotal = 0;
                    decimal Iva = 0;


                    BaseIva = Decimal.Parse(voucher.MONTOGRAVAIVA.Substring(0, 11) + "." + voucher.MONTOGRAVAIVA.Substring(11, 2));
                    BaseSinIva = Decimal.Parse(voucher.MONTONOGRAVAIVA.Substring(0, 11) + "." + voucher.MONTONOGRAVAIVA.Substring(11, 2));
                    Subtotal = BaseIva + BaseSinIva;
                    Iva = Decimal.Parse(voucher.VALORIVA.Substring(0, 11) + "." + voucher.VALORIVA.Substring(11, 2));


                    recipe.BaseIva = BaseIva.ToString("###,##0.00");
                    recipe.BaseSinIva = BaseSinIva.ToString("###,##0.00");
                    recipe.Subtotal = Subtotal.ToString("###,##0.00");
                    recipe.Iva = Iva.ToString("###,##0.00");
                }
                else
                {
                    recipe.BaseIva = "";
                    recipe.BaseSinIva = "";
                    recipe.Subtotal = "";
                    recipe.Iva = "";
                }

                // Nuevo Autorizador "AUSTRO".  JM  04-12-2020
                //recipe.CodigoRed = trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST";
                if (Pago.CodRed == "01")
                    recipe.CodigoRed = "DATAFAST";
                else if (Pago.CodRed == "02")
                    recipe.CodigoRed = "MEDIANET";
                else if (Pago.CodRed == "03")
                    recipe.CodigoRed = "AUSTRO";
                // Nuevo Autorizador "AUSTRO".  JM  04-12-2020

                string tipodebcred = "";
                if (trama.nomGruTar.Contains("DEBIT"))
                {
                    tipodebcred = "DEBITO";
                    recipe.Pagare = " ";
                }
                else
                {
                    if (cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                        tipodebcred = "ROTATIVO";
                    else
                        tipodebcred = "" + cmbBancoTarjeta.Text + "\n\n                              PLAZO MESES: " + cmbDiferido.Text + (cmbMesesGracia.Text != "" ? "\n                              " + "MESES DE GRACIA:" + cmbMesesGracia.Text : "");
                }
                if (pp.TipoTransaccion != "03")
                    recipe.TipoDebCredito = tipodebcred;


                if (cmbBancoTarjeta.Text.Contains("DIFERIDO CON INTERESES"))
                {

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "prepararVoucherTarjeta", "VALOR INTERES:" + trama.valInteres);
                    // recipe.Intereses = Decimal.Parse("0.00").ToString("###,##0.00");
                    try
                    {
                        decimal valorinteres1 = trama.valInteres == "             " ? 0 : Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2));

                        //recipe.Intereses = Decimal.Parse(trama.valInteres).ToString("###,##0.00");
                        recipe.Intereses = valorinteres1.ToString("###,##0.00");
                    }
                    catch (Exception ex2)
                    {
                        recipe.Intereses = Decimal.Parse("0.00").ToString("###,##0.00");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "prepararVoucherTarjeta", "Error :" + ex2.Message);
                    }
                }
                else
                {
                    recipe.Intereses = "";
                }

                recipe.Arqc = trama.ARQC;
                recipe.Aidemv = trama.AIDEMV.Trim();
                recipe.Emv = trama.idEMV;
                recipe.Tc = trama.tipoCritoyValorEMV;
                recipe.Publicidad = trama.mensajePremioPublicidad;
                recipe.TVR = trama.TVR;
                recipe.TSI = trama.TSI;

                decimal ValorVoucher = 0.0M;
                ValorVoucher = decimal.Parse(recipe.ValorTotal);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "prepararVoucherTarjeta", "recipe :" + recipe);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "prepararVoucherTarjeta", "ImprimirVoucherTarjetaCredito tipo_voucher: " + tipo_voucher + "; recipe: " + recipe + "; esCopia: " + esCopia);

                Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe, esCopia);
            }
            catch (Exception ex)
            {
                response = Common.ExceptionHandler.GetExceptionMessages(ex) + Environment.NewLine + "Stacktrace " + ex.StackTrace;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "preparaVoucherTrajeta", "error: " + response);
            }
        }

        private void ProcesaPinpadDev()
        {

            try
            {
                using (POSEntities pos = new POSEntities())
                {
                    Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                    Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
                    string valpag = Decimal.Round(Decimal.Parse(txtValor.Text) * 1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";

                    string bin_descripcion = "";

                    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                    trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                    trama.plazoDiferido = cmbDiferido.Text;
                    trama.mesGracia = cmbMesesGracia.Text;

                    var porc_iva = Control.Common.GlobalParameters.IVAGEN / 100;
                    var PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;// Decimal.Parse((pos.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));

                    //var porc_pago = decimal.Round(decimal.Parse(txtValor.Text) / _factura.GetTotal(),2);
                    var porc_pago = (decimal.Parse(txtValor.Text) / _factura.GetTotal());
                    decimal porc_Desc2 = 0;
                    if (_facturaApp == null)
                    {
                        porc_Desc2 = 0;
                    }
                    else
                        if (_facturaApp.Descuentos2.Count > 0)
                        {
                            porc_Desc2 = _facturaApp.Descuentos2.Max(x => x.Porcentaje);
                        }
                    var base0 = _factura.GetBase0() - (_factura.GetDescuentos() - (_factura.GetBase12() - _factura.GetBase12Desc()));
                    base0 = base0 - (base0 * (porc_Desc2 / 100));
                    var base12 = _factura.GetBase12Desc();
                    base12 = base12 - (base12 * (porc_Desc2 / 100));
                    if (_factura.GetPromoIva() > 0)
                    {
                        base12 = base12 - ((base12 * PorcPromo) / 100);
                    }
                    var iva = decimal.Round(base12 * porc_iva, 2);
                    trama.montoTotalTransaccion = valpag;//12N 10N2D

                    if (decimal.Parse(txtValor.Text) == _factura.GetTotal())
                    {
                        trama.montoBaseGravaIVa = decimal.Round((base12 * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = decimal.Round(((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0) * porc_pago), 2, MidpointRounding.ToEven).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = decimal.Round((iva * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                    }
                    else
                    {

                        base12 = Decimal.Round(base12 * porc_pago, 2);
                        iva = decimal.Round(base12 * porc_iva, 2);
                        base0 = decimal.Parse(txtValor.Text) - base12 - iva;

                        if (base0 < decimal.Parse("0"))
                        {
                            porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                            base12 = _factura.GetBase12Desc();
                            if (_factura.GetPromoIva() > 0)
                            {
                                base12 = base12 - ((base12 * PorcPromo) / 100);
                            }
                            iva = decimal.Round(base12 * porc_iva, 2);


                            base12 = base12 * porc_pago;
                            iva = base12 * porc_iva;
                            base0 = decimal.Parse(txtValor.Text) - base12 - iva;
                        }

                        trama.montoBaseGravaIVa = (base12).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = ((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0)).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = (iva).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              

                    }

                    trama.impuestoServicioTransaccion = "";//12N 10N2D
                    trama.popinaTransaccion = "";//12N 10N2D
                    trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                    trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                    trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                    trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); //AAAAMMDD
                    trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos
                    trama.CID = _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 


                    if (trama.TipoTransaccion == "03")
                    {
                        //trama.numAutorizacion = txtnumAut.Text;//6N  solo anulaciones envia autorizacion compra original / resto blancos
                        //trama.secuencialTransaccion = txtSecuencial.Text.PadLeft(6, '0');//6N  -- Anulaciones enviar Secuencial / resto en cero
                    }

                    resptrama.SetValuesTest("00",
                                            "02",
                                            "00",
                                            "Demo MensajeRespuesta",
                                            "000001",
                                            "000001",
                                            DateTime.Now.ToString("yyyyMMdd"),
                                            DateTime.Now.ToString("HHmmss"),
                                            "000001",
                                            "",
                                            "",
                                            "            ",
                                            "",
                                            "Demo CodBancoAdquieriente",
                                            "Demo BancoAdquieriente",
                                            "Demo GrupoTarjeta",
                                            "03",
                                            "Demo TarjetaHabiente",
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            "999999XXXXXX0000",
                                            "XXXX",
                                            "");

                    //Agregar el nro del comprobante de la factura para que se pueda incluir en la impresion
                    trama.FacturaComprobante = _factura.GetNumeroFacturaEnmascarado();
                    //Impresion de voucher
                    String tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                    string printResponse = string.Empty;
                    Decimal valor = Decimal.Parse(txtValor.Text);

                    prepararVoucherTarjeta(tipovoucher, resptrama, trama, ref printResponse);

                    if (valor > 15)
                    {
                        prepararVoucherTarjeta(tipovoucher, resptrama, trama, ref printResponse, true);
                    }

                    bin_descripcion = "";//evelasco para pruebas, enviar la trama de ejemplo. 20 Septuembre 2019.

                    // Nuevo Autorizador "AUSTRO".  JM  04-12-2020
                    string nombreRed = "";
                    if (resptrama.codigoRed == "01")
                        nombreRed = "DATAFAST";
                    else if (resptrama.codigoRed == "02")
                        nombreRed = "MEDIANET";
                    else if (resptrama.codigoRed == "03")
                        nombreRed = "AUSTRO";

                    _factura.AgregarPagoTarjetaCredito(valor,
                                        nombreRed,
                                        resptrama.nomGruTar,
                                        resptrama.codBancoAdq,
                                        nombreRed,
                                        bin_descripcion);

                    /*_factura.AgregarPagoTarjetaCredito(valor,
                                        resptrama.codigoRed == "02" ? "MEDIANET" : "DATAFAST",
                                        resptrama.nomGruTar,
                                        resptrama.codBancoAdq,
                                        resptrama.codigoRed == "02" ? "MEDIANET" : "DATAFAST",
                                        bin_descripcion);*/
                    // Nuevo Autorizador "AUSTRO".  JM  04-12-2020

                    //grabar transaccion en tabla pos_voucher
                    //cambiar aqui
                    POS_VOUCHER pos_voucher = new POS_VOUCHER();

                    try
                    {
                        pos_voucher.TARJETA = resptrama.numTarTuncate.Trim().PadRight(19, ' ');// ("520081XXXXXX6017   "); //19 ;

                        //revisar
                        string codigoproceso = "000200";
                        //es 003000 cuando es transacciones con tarjeta de crédito, 001000 cuando es transacción de tarjeta de debito cuenta de ahorro y 002000 cuando es transacción de tarjeta de debito cuenta corriente.

                        pos_voucher.CODIGOPROCESO = codigoproceso;// ("000200"); //6 ;
                                                                  //revisar

                        pos_voucher.FECHACONSUMO = resptrama.fechaTrans;// ("20161122"); //8 ;
                        pos_voucher.HORACONSUMO = resptrama.horaTrans;// ("114339"); //6 ;
                        pos_voucher.NUMEROVOUCHER = resptrama.secuencialtransaccion;// ("000002");//6 ;

                        pos_voucher.AUTORIZACION = trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut; //6 ;
                        pos_voucher.ANULADO = trama.TipoTransaccion == "03" ? true : false;

                        pos_voucher.VALORCONSUMO = trama.montoTotalTransaccion.PadLeft(13, '0');// ("0000000001200"); //13 ;
                        pos_voucher.FORMAAUTORIZA = ("1"); //1 ;
                        if (resptrama.codigoRed == "02")
                            pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo; //2 ;
                        else
                            pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData; //2 ;
                        pos_voucher.PLAZO = trama.plazoDiferido.PadLeft(2, '0');// ("06"); //2 ;

                        pos_voucher.TIPOLECTURA = resptrama.modoLectura.PadLeft(3, '0');// ("005"); //3 ;
                        pos_voucher.TIPOMONEDA = ("840"); //3 ;
                        pos_voucher.VALORIVA = trama.impuestoIvaTransaccion.PadLeft(13, '0');// ("0000000000147"); //13 ;
                        pos_voucher.VALORSERVICIO = ("0000000000000"); //13 ;
                        pos_voucher.VALORPROPINA = trama.popinaTransaccion.PadLeft(13, '0');// ("0000000000000"); //13 ;
                        pos_voucher.VALORINTERES = resptrama.valInteres.Trim().PadLeft(13, '0');// ("0000000000057");  //13 ;
                        pos_voucher.VALORFIJO = resptrama.montoFijo.Trim().PadLeft(13, '0');// ("0000000000000");  //13 ;
                        pos_voucher.TIPOPROMOCION = ("00"); //2 ;
                        pos_voucher.MESESGRACIA = trama.mesGracia.PadLeft(2, '0');//                        ("00");  //2 ;
                        pos_voucher.EMPRESASERVICIO = ("0000");  //4 ;
                                                                 //pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "R"); //1 ;
                        pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "O"); //JCanarte
                        pos_voucher.CODIGORESPUESTA = resptrama.codigoRespuesta;// ("00"); //2 ;
                        pos_voucher.TIPODISPOSITIVO = ("2"); //1 ;
                        pos_voucher.ADQUIRENTETARJETA = ("CREDIMATIC01"); //12 ;
                        pos_voucher.ADQUIRENTESERVICIO = ("            ");  //12 ;
                        pos_voucher.MONTOGRAVAIVA = trama.montoBaseGravaIVa.PadLeft(13, '0');// ("0000000001053"); //13 ;
                        pos_voucher.MONTONOGRAVAIVA = trama.montoBaseNoGravaIVa.PadLeft(13, '0');// ("0000000000000");  //13 ;
                        pos_voucher.PUNTOEMISION = _factura.Establecimiento + _factura.PtoEmision;
                        pos_voucher.PROCESADO = false;
                        pos_voucher.GRUPOTAR = resptrama.nomGruTar;
                        pos_voucher.AUTORIZADOR = int.Parse(resptrama.codigoRed);
                        pos_voucher.LOTE = resptrama.numerolote;
                        pos_voucher.FACTURA = _factura.GetNumeroFactura();
                        //ML [16/01/2018]: Nuevos campos
                        pos_voucher.ARQC = resptrama.ARQC;
                        pos_voucher.AIDEMV = resptrama.AIDEMV;
                        pos_voucher.EMV = resptrama.idEMV;
                        pos_voucher.TC = resptrama.tipoCritoyValorEMV;
                        pos_voucher.PUBLICIDAD = resptrama.mensajePremioPublicidad;
                        pos_voucher.TIPOTRANSACCION = trama.TipoTransaccion;
                        pos_voucher.BANCOADQUIRIENTE = resptrama.nomBancoAdq;
                        pos_voucher.TARJETAHABIENTE = resptrama.nombreTarjetaHabiente;
                        pos_voucher.MID = trama.MID;
                        pos_voucher.TID = trama.TID;
                        pos_voucher.VENCTAR = resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XXXX";
                        //pos_voucher.VENCTAR = resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX";
                        pos_voucher.ANULAUTORIZACION = trama.numAutorizacion;
                        pos_voucher.TIPOBANCOTARJETA = (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);
                        //
                        bool ExisteVoucher = pos.POS_VOUCHER.Any(x => x.FECHACONSUMO == pos_voucher.FECHACONSUMO && x.VALORCONSUMO == pos_voucher.VALORCONSUMO &&
                        x.AUTORIZACION == pos_voucher.AUTORIZACION && x.NUMEROVOUCHER == pos_voucher.NUMEROVOUCHER && x.FACTURA == pos_voucher.FACTURA);
                        if (!ExisteVoucher)
                        {
                            pos.POS_VOUCHER.Add(pos_voucher);
                        }
                        // pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet += 1;
                        pos.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string datosVoucher = "\nTARJETA: " + resptrama.numTarTuncate.Trim().PadRight(19, ' ') +
                                                "\nCODIGOPROCESO: 000200" +
                                                "\nFECHACONSUMO: " + resptrama.fechaTrans +
                                                "\nHORACONSUMO: " + resptrama.horaTrans +
                                                "\nNUMEROVOUCHER: " + resptrama.secuencialtransaccion +
                                                "\nAUTORIZACION: " + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) +
                                                "\nANULADO: " + (trama.TipoTransaccion == "03" ? "1" : "0") +
                                                "\nVALORCONSUMO: " + trama.montoTotalTransaccion.PadLeft(13, '0') +
                                                "\nFORMAAUTORIZA: 1" +
                                                "\nTIPOCONSUMO: " + (resptrama.codigoRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData) +
                                                "\nPLAZO: " + trama.plazoDiferido.PadLeft(2, '0') +
                                                "\nTIPOLECTURA: " + resptrama.modoLectura.PadLeft(3, '0') +
                                                "\nTIPOMONEDA: 840" +
                                                "\nVALORIVA: " + trama.impuestoIvaTransaccion.PadLeft(13, '0') +
                                                "\nVALORSERVICIO: 0000000000000" +
                                                "\nVALORPROPINA: " + trama.popinaTransaccion.PadLeft(13, '0') +
                                                "\nVALORINTERES: " + resptrama.valInteres.Trim().PadLeft(13, '0') +
                                                "\nVALORFIJO: " + resptrama.montoFijo.Trim().PadLeft(13, '0') +
                                                "\nTIPOPROMOCION: 00" +
                                                "\nMESESGRACIA: " + trama.mesGracia.PadLeft(2, '0') +
                                                "\nEMPRESASERVICIO: 0000" +
                                                //"\nESTADOTRX: " + (pos_voucher.TIPOCONSUMO == "01" ? "O" : "R") +
                                                "\nESTADOTRX: " + (pos_voucher.TIPOCONSUMO == "01" ? "O" : "O") +//JCanarte
                                                "\nCODIGORESPUESTA: " + resptrama.codigoRespuesta +
                                                "\nTIPODISPOSITIVO: 2" +
                                                "\nADQUIRENTETARJETA: CREDIMATIC01" +
                                                "\nADQUIRENTESERVICIO:             " +
                                                "\nMONTOGRAVAIVA: " + trama.montoBaseGravaIVa.PadLeft(13, '0') +
                                                "\nMONTONOGRAVAIVA: " + trama.montoBaseNoGravaIVa.PadLeft(13, '0') +
                                                "\nPUNTOEMISION: " + _factura.Establecimiento + _factura.PtoEmision +
                                                "\nPROCESADO: 0" +
                                                "\nGRUPOTAR: " + resptrama.nomGruTar +
                                                "\nAUTORIZADOR: " + resptrama.codigoRed +
                                                "\nLOTE: " + resptrama.numerolote +
                                                "\nFACTURA: " + _factura.GetNumeroFactura() +
                                                "\nARQC: " + resptrama.ARQC +
                                                "\nAIDEMV: " + resptrama.AIDEMV +
                                                "\nEMV: " + resptrama.idEMV +
                                                "\nTC: " + resptrama.tipoCritoyValorEMV +
                                                "\nPUBLICIDAD: " + resptrama.mensajePremioPublicidad +
                                                "\nTIPOTRANSACCION: " + trama.TipoTransaccion +
                                                "\nBANCOADQUIRIENTE: " + resptrama.nomBancoAdq +
                                                "\nTARJETAHABIENTE: " + resptrama.nombreTarjetaHabiente +
                                                "\nMID: " + trama.MID +
                                                "\nTID: " + trama.TID +
                                                "\nVENCTAR: " + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") +
                                                "\nANULAUTORIZACION: " + trama.numAutorizacion +
                                                "\nTIPOBANCOTARJETA: " + (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Se realizo correctamente una transaccion pinpad al banco, asi como las impresiones de los recibos, pero el voucher no pudo ser grabado en nuestra base interna, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex) + Environment.NewLine + "Datos Voucher: " + datosVoucher.Replace("\n", Environment.NewLine));

                        try
                        {
                            var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                Properties.Settings.Default.MAILERROR_FROM,
                                Properties.Settings.Default.MAILERROR_ALIAS,
                                Properties.Settings.Default.MAILERROR_DESTINO,
                                Properties.Settings.Default.MAILERROR_CC,
                                "Voucher pinpad no se grabó en nuestra base interna",
                                String.Format("En el POS de TEST del siguiente punto de emision, si bien se realizo correctamente una transaccion pinpad al banco y se imprimieron los recibos, el voucher no pudo ser grabado en nuestra base interna. Generar el registro en POS_VOUCHER inmediatamente pues puede provocar descuadres en los cierres. Esto pudo deberse a un breve inconveniente, se recomienda una vez generado el registro, verificar la causa de la novedad" +
                                                "  \n\nDatos caja-----------------" +
                                                "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n" +
                                                "  \n\nDatos voucher-----------------" +
                                                datosVoucher,
                                            Control.Common.GlobalParameters.Establecimiento,
                                            Control.Common.GlobalParameters.PuntoEmision,
                                            Control.Common.GlobalParameters.IpMaquina,
                                            Control.Common.GlobalParameters.UsuarioNombre,
                                            Control.Common.GlobalParameters.Usuario,
                                            Control.Common.ExceptionHandler.GetExceptionMessages(ex)),
                                false,
                                String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "No se pudo enviar notificacion del problema al grabar voucher en nuestra base interna, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }
                        }
                        catch { }

                        ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco y se imprimieron los recibos, pero detectamos que el voucher no pudo ser grabado en nuestra base interna. Por favor comuníquelo inmediatamente al administrador para que realice su gestión y evitar descuadre durante su cierre";
                    }

                    //Agregar lineas de insert
                    Control.Common.Logger.Agregar_Trace_Voucher(pos_voucher);

                    DebeCerrarFormBackground = true;
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "ProcesaPinpadDev", "error: " + ex.Message);
            }

        }

        #region Background ProcesaPinpad

        private string ResponseBackground { get; set; }
        private string ResponseBackgroundAdicional { get; set; }
        private bool EstadoControles { get; set; }
        private bool DebeCerrarFormBackground { get; set; }
        private Models.PagoPinpad PagoPinpadBackground { get; set; }
        private Models.PagoTarjetaInterna PagoTarjetaInterna { get; set; }

        private bool ValidarSolicitudRecarga()
        {
            return true;
        }

        BackgroundWorker bgw;
        private void ProcesaPinpadBackGround()
        {
            if (!ValidarSolicitudRecarga()) return;

            paneLoading.Visible = true;

            if (bgw == null)
            {
                bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            }

            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
            bgw.RunWorkerAsync();
            System.Threading.Thread.Sleep(500);
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {

            if (Control.Common.GlobalParameters.PINPAD_MULTIRED)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "bgw_DoWork", "BackGround, Se procesa por PINPAD_MULTIRED");
                ProcesaPinpadBackgroundMultiRed();
            }
            else
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "bgw_DoWork", "BackGround, Se procesa por PINPAD por defecto MEDIANET");
                ProcesaPinpad();
            }


        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                paneLoading.Visible = false;

                if (!string.IsNullOrWhiteSpace(ResponseBackground))
                {
                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[ResponseBackground]", valor = ResponseBackground });
                    Control.Common.General.GetMensajeToList(402, parametros, ResponseBackgroundAdicional);
                }


                //Agregar pago desde repositorio background si existe
                if (PagoPinpadBackground != null)
                {
                    Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "BasePagos", "bgw_RunWorkerCompleted", "Cobro tarjeta background agregado a los pagos de la factura '"
                                + _factura.GetNumeroFactura()
                                + "'. Datos cobro [ Valor: " + PagoPinpadBackground.Valor.ToString("N2") + "; Banco: "
                                + PagoPinpadBackground.Banco + "; Nombre: " + PagoPinpadBackground.Nombre + "; Marca: " + PagoPinpadBackground.Marca + "; TipoPOS: " + PagoPinpadBackground.TipoPos + "; BinDescripcion: "
                                + "; NumBin: " + PagoPinpadBackground.NumBin
                                + PagoPinpadBackground.BinDescripcion + " ]");



                    _factura.AgregarPagoTarjetaCredito(
                        PagoPinpadBackground.Valor,
                        PagoPinpadBackground.Banco,
                        PagoPinpadBackground.Nombre,
                        PagoPinpadBackground.Marca,
                        PagoPinpadBackground.TipoPos,
                        PagoPinpadBackground.NumBin,
                        PagoPinpadBackground.BinDescripcion);
                }

                //Background activara una bandera si se debe cerrar el formulario
                if (DebeCerrarFormBackground) this.Close();
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "bgw_RunWorkerCompleted", "La solicitud no pudo ser realizada debido a un incidente en los controles. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                MessageBox.Show("La solicitud no pudo ser completada. Contacte a administrador");
            }
        }

        private void On_Off_Controles(bool estado)
        {
            EstadoControles = estado;
            txtValor.Enabled = estado;
            cmbBancoTarjeta.Enabled = estado;
            cmbTipoTransaccion.Enabled = estado;
            btn0.Enabled = estado;
            btn1.Enabled = estado;
            btn2.Enabled = estado;
            btn3.Enabled = estado;
            btn4.Enabled = estado;
            btn5.Enabled = estado;
            btn6.Enabled = estado;
            btn7.Enabled = estado;
            btn8.Enabled = estado;
            btn9.Enabled = estado;
            btnBorrar.Enabled = estado;
            btnCancelar.Enabled = estado;
            btnEliminar.Enabled = estado;
            btnPagoManual.Enabled = estado;
            btnEnter.Enabled = estado;
            btnVerificar.Enabled = estado;
            btnPunto.Enabled = estado;
            txtCuenta.Enabled = estado;
            txtNumCheque.Enabled = estado;
            gridPagos.Enabled = estado;
        }

        #endregion






        private void AplicarDescuentoTarjetaBines(decimal valor)
        {
            _mainWindow.aplicaDsctoPromoTarjetaBines = this.aplicaDsctoPromoTarjetaBines;
            _mainWindow?.agregarDescuentoPromocionTarjetaBines(valor);
        }







        public void ProcesaPinpadBackgroundMultiRed()
        {
            ResponseBackground = string.Empty;
            DebeCerrarFormBackground = false;
            On_Off_Controles(false);
            PagoPinpadBackground = null;


            POS_VOUCHER pos_voucher = new POS_VOUCHER();
            ClsEnviaPinPadGeneral envioGen = new ClsEnviaPinPadGeneral();
            Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
            Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();

            string autorizador = "2";
            string IPPinPad = string.Empty;
            int PuertoPinPad = 0;
            PinPadRespuesta resultadolectura = new PinPadRespuesta();

            var pos = new POSEntities();
            string TarjetaCreditoBin = string.Empty;
            string plazoDiferido = string.Empty;
            string mesGracia = string.Empty;
            string bin_descripcion = string.Empty;
            string valpag = string.Empty;
            decimal valor = 0;
            int timeOutCP = Control.Common.GlobalParameters.ConectContingente.TiempoOutCP;
            string nombreRed = "";
            string MID = string.Empty;
            string TID = string.Empty;

            string numBinTC = string.Empty;
            string codDiferido = string.Empty;
            bool validaAutorizador = true;


            try
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Ejecuta proceso ProcesaPinpadBackgroundMultiRed ");

                if (!Control.Common.GlobalParameters.EsAmbienteProduccion && Control.Common.GlobalParameters.EstTcpIpPinpad == false) // comebt
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Ejecuta ProcesaPinpadDev  ");

                    ProcesaPinpadDev();
                    return;
                }


                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Inicializo la variable de Trama: Tramas.ProcesaPago  ");
                trama = new Tramas.ProcesaPago();

                valpag = Decimal.Round(Decimal.Parse(txtValor.Text) * 1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0');
                valor = Decimal.Parse(txtValor.Text);

                MID = string.Empty;
                TID = string.Empty;


                trama = new Tramas.ProcesaPago();
                trama.codRed = "2";
                trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;


                if (_factura.AplicaDescuentoPromoBines || !Control.Common.GlobalParameters.ActivaVersionPinPadMedianet)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Se invoca General.ValidaContingente, en caso de que el servicio de Medianet se ha reestablecido o el tiempo para generar pruebas ha concluido");
                    Control.Common.General.ValidaContingente();


                    IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                    PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " IP PinPad: " + IPPinPad + "; PuertoPinPad: " + PuertoPinPad);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " GlobalParameters.IPPinPad: " + Control.Common.GlobalParameters.IPPinPad
                                                 + "; GlobalParameters.PuertoPinPad: " + Control.Common.GlobalParameters.PuertoPinPad);


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Lectura Pinpad ");
                    resultadolectura = envioGen.LecturaTarjeta(IPPinPad, PuertoPinPad, 65000, "LT", "", 1);



                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $" Trama Repuesta (LT): {resultadolectura.TramaRespuesta}");

                    if (resultadolectura.CodigoRespuesta != "00")
                    {
                        ResponseBackground = "Error en PINPAD: " + resultadolectura.CodigoRespuesta + " - " + resultadolectura.MensajeRespuesta + "\n\n";
                        ResponseBackground = ResponseBackground + " IP PinPad: " + IPPinPad + "; PuertoPinPad: " + PuertoPinPad + "\n\n";
                        ResponseBackground = ResponseBackground + " GlobalParameters.IPPinPad: " + Control.Common.GlobalParameters.IPPinPad + "\n\n";
                        ResponseBackground = ResponseBackground + " GlobalParameters.PuertoPinPad: " + Control.Common.GlobalParameters.PuertoPinPad + "\n\n";

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", ResponseBackground);
                        return;
                    }

                    int leerTramaRes = 75;
                    if (resultadolectura.TramaRespuesta.Length > 98)
                    {
                        leerTramaRes = leerTramaRes + 24;
                    }


                    numBinTC = resultadolectura.NumBin;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Consulto BIN de la tarjeta, para saber por cual autorizador se va. ");
                    var ConsultaBin = pos.core_tarjetacredito_bin.Where(x => x.bin == numBinTC).FirstOrDefault();

                    if (ConsultaBin == null)
                    {
                        ResponseBackground = "Error en PINPAD: " + resultadolectura.CodigoRespuesta + " - " + resultadolectura.MensajeRespuesta + "\n\n";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", ResponseBackground);
                        return;
                    }


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Recupero el valor de la Red Autorizador, según el BIN de la Tarjeta ");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Autorizador BIN: " + ConsultaBin.bin_red);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Asigno valor por defecto de autorizador. basado en las referencias de la tabla BIN ");


                    autorizador = ConsultaBin.bin_red;
                    bin_descripcion = ConsultaBin.bin_descripcion;

                    string textValidacion = string.Empty;
                    //string numBin = string.Empty;
                    string localidad = string.Empty;

                    var validaBin = Control.PINPAD.bines.validaBines(numBinTC, _factura.Establecimiento);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $" validaBin.BinBloqueado {validaBin.BinBloqueado} ");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $" validaBin.binTieneDescuento {validaBin.binTieneDescuento} ");

                    if (validaBin.BinBloqueado)
                    {
                        validaAutorizador = false;
                        textValidacion = validaBin.TxtValidacionBloqueo;

                        ResponseBackground = textValidacion;
                        On_Off_Controles(true);
                        return;
                    }



                    //if (validaBin.binTieneDescuento)
                    //{

                    //    this.aplicaDsctoPromoTarjetaBines = validaBin.binTieneDescuento;
                    //    this.binTarjetaPromoDscto = numBinTC;
                    //    this.DescuentoPromoTarjBines = validaBin.porcDsctoBin;


                    //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed: ", ResponseBackground);

                    //    _factura.BinNumeroTarjetaPromo = validaBin.NumeroBin.ToString();
                    //    AplicarDescuentoTarjetaBines(validaBin.porcDsctoBin);

                    //    txtValor.Text = _factura.GetTotal().ToString();

                    //    ResponseBackground = validaBin.TxtDsctBin;
                    //    On_Off_Controles(true);
                    //    return;

                    //}

                    // Nuevo metodo para ver si el bin tiene  descuento JCHID 
                    decimal valorACobrar = Decimal.Parse(txtValor.Text);
                    decimal totalFactura = _factura.GetTotal();

                    bool esPagoTotal = valorACobrar >= (totalFactura - 0.01m);

                    if (validaBin.binTieneDescuento && esPagoTotal)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "IntegracionTotal", "¡Descuento Detectado! Aplicando...");
                        decimal valorDescuentoDinero = 0;
                        // Aplicamos el descuento a la factura interna
                        this.aplicaDsctoPromoTarjetaBines = true;
                        this.binTarjetaPromoDscto = numBinTC;
                        this.DescuentoPromoTarjBines = validaBin.porcDsctoBin;
                        Control.Common.GlobalParameters.EsModoImpresionBankard = true;
                        _factura.BinNumeroTarjetaPromo = Convert.ToString(validaBin.NumeroBin);

                        // Método que recalcula los totales dentro del objeto _factura
                        AplicarDescuentoTarjetaBines(validaBin.porcDsctoBin);

                        // Actualizamos la pantalla para que el cajero vea que bajó el precio
                        txtValor.Text = _factura.GetTotal().ToString("N2");

                        // *** PASO CRÍTICO: ACTUALIZAR LAS VARIABLES DE COBRO ***
                        // Si no hacemos esto, cobraremos el precio original sin descuento
                        valor = _factura.GetTotal();
                        // Regeneramos el string de pago (Ej: de "100" baja a "90")
                        valpag = valor.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "IntegracionTotal", $"Nuevo monto a cobrar: {valor} ({valpag})");
                    }
                    else
                    {
                        // Si no hubo descuento, nos aseguramos que 'valor' sea el actual de la caja de texto
                        valor = Decimal.Parse(txtValor.Text);
                        valpag = valor.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');
                        Control.Common.GlobalParameters.EsModoImpresionBankard = false;
                    }
                    // Nuevo metodo para ver si el bin tiene  descuento JCHID 


                    //var validaBin = (from deta in pos.core_parametro
                    //                 where deta.identificador == "BLOQUEO_BINES"
                    //                 && deta.valor == "TRUE"
                    //                 select deta).ToList().FirstOrDefault();


                    //if (validaBin != null) { textValidacion = validaBin.documento; }


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Valida contingente PINPAD; por defecto MEDIANET se encuentra configurado por defecto ");
                    if (Control.Common.GlobalParameters.ConectContingente.PinPadContingente)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "ConectContingente.PinPadContingente: " + Control.Common.GlobalParameters.ConectContingente.PinPadContingente);
                        if (Control.Common.GlobalParameters.ConectContingente.Autorizador.ToString() != autorizador)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "ConectContingente.Autorizador: " + Control.Common.GlobalParameters.ConectContingente.Autorizador + "; autorizador: " + autorizador);
                            switch (autorizador)
                            {
                                case "1":
                                    autorizador = "2";
                                    break;
                                case "2":
                                    autorizador = "1";
                                    break;
                            }

                            //autorizador = Control.Common.GlobalParameters.ConectContingente.Autorizador.ToString();
                        }

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Recupero IP, PuertoPinpad, TID, MID");

                        if (autorizador == "2")
                        {
                            IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                            PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                            MID = Control.Common.GlobalParameters.MID_MEDIANET;
                            TID = Control.Common.GlobalParameters.TID_MEDIANET;
                        }
                        else
                        {
                            IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadDATAFAST;
                            PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadDataFast;

                            MID = Control.Common.GlobalParameters.MID_DATAFAST;
                            TID = Control.Common.GlobalParameters.TID_DATAFAST;
                        }

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $"(PinPadContingente): autorizador: {autorizador}:" +
                                $" MID: {MID} ; TID: {TID} ");

                    }
                    else
                    {

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Valor Contingente FALSE, reasigno TID / MID ");


                        switch (autorizador)
                        {
                            case "2":

                                IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                                PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                                MID = Control.Common.GlobalParameters.MID_MEDIANET;
                                TID = Control.Common.GlobalParameters.TID_MEDIANET;
                                break;

                            case "1":
                                IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadDATAFAST;
                                PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadDataFast;

                                MID = Control.Common.GlobalParameters.MID_DATAFAST;
                                TID = Control.Common.GlobalParameters.TID_DATAFAST;
                                break;

                            case "99":
                                validaAutorizador = false;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "BIN se encuentra bloqueado ");
                                break;

                            default:
                                validaAutorizador = false;
                                break;
                        }

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $"(PinPadContingente): autorizador: {autorizador}:" +
                                $" MID: {MID} ; TID: {TID} ");



                        if (validaAutorizador == false)
                        {
                            ResponseBackground = textValidacion;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed: ", ResponseBackground);
                            return;
                        }

                    }

                    if (!Control.Common.GlobalParameters.EstTcpIpPinpad)
                    {
                        ResponseBackground = Common.GlobalParameters.PinpadMsjAutorizadorNoValido;
                        return;
                    }

                    trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                    trama.codRed = autorizador;
                    trama.MID = MID;
                    trama.TID = TID;
                }


                codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                if (autorizador == "1")
                {
                    codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                }

                trama.codDiferido = codDiferido;
                plazoDiferido = cmbDiferido.Text.PadLeft(2, ' ');
                trama.plazoDiferido = plazoDiferido;

                mesGracia = cmbMesesGracia.Text.PadLeft(2, ' ');
                trama.mesGracia = mesGracia;

                var porc_iva = Control.Common.GlobalParameters.IVAGEN / 100;
                var PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;/// Decimal.Parse((pos.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));
                var porc_pago = (decimal.Parse(txtValor.Text) / _factura.GetTotal());
                //decimal porc_Desc2 = 0;

                //if (_facturaApp == null)
                //{
                //    porc_Desc2 = 0;
                //}
                //else
                //    if (_facturaApp.Descuentos2.Count > 0)
                //    {
                //        porc_Desc2 = _facturaApp.Descuentos2.Sum(x => x.Porcentaje);
                //    }

                //var base0 = _factura.GetBase0() - (_factura.GetDescuentos() - (_factura.GetBase12() - _factura.GetBase12Desc()));
                //base0 = base0 - (base0 * (porc_Desc2 / 100));
                //var base12 = _factura.GetBase12DescPromoIVA(false);
                //base12 = base12 - (base12 * (porc_Desc2 / 100));

                //var base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                //if (_factura.GetPromoIva() > 0)
                //{
                //    base12 = base12 - ((base12 * PorcPromo) / 100);
                //}

                //base12 += base12PromoIVAExcluye;
                //var iva = decimal.Round(base12 * porc_iva, 2);
                //trama.montoTotalTransaccion = valpag;//12N 10N2D

                //if (decimal.Parse(txtValor.Text) == _factura.GetTotal())
                //{
                //    trama.montoBaseGravaIVa = decimal.Round((base12 * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                //    trama.montoBaseNoGravaIVa = decimal.Round(((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0) * porc_pago), 2, MidpointRounding.ToEven).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                //    trama.impuestoIvaTransaccion = decimal.Round((iva * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                //}
                //else
                decimal porc_Desc2 = 0;

                if (_facturaApp != null && _facturaApp.Descuentos2.Count > 0)
                {
                    porc_Desc2 = _facturaApp.getDescuentos2();
                }

                var base0 = _factura.GetBase0() - (_factura.GetDescuentos() - (_factura.GetBase12() - _factura.GetBase12Desc()));
                var base12 = _factura.GetBase12DescPromoIVA(false);

                var base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                if (_factura.GetPromoIva() > 0)
                {
                    base12 = base12 - ((base12 * PorcPromo) / 100);
                }
                base12 += base12PromoIVAExcluye;
                var iva = decimal.Round(base12 * porc_iva, 2);

                if (porc_Desc2 > 0)
                {
                    base0 = base0 - decimal.Round((base0 / (base0 + base12)) * porc_Desc2, 2, MidpointRounding.AwayFromZero);
                    base12 = base12 - decimal.Round((base12 / (base0 + base12)) * porc_Desc2, 2, MidpointRounding.AwayFromZero);
                    iva = decimal.Round(base12 * porc_iva, 2);
                }
                trama.montoTotalTransaccion = valpag;

                if (decimal.Parse(txtValor.Text) == _factura.GetTotal())
                {
                    trama.montoBaseGravaIVa = decimal.Round((base12 * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');
                    trama.montoBaseNoGravaIVa = decimal.Round(((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0) * porc_pago), 2, MidpointRounding.ToEven).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');
                    trama.impuestoIvaTransaccion = decimal.Round((iva * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');
                }
                else
                {

                    base12 = Decimal.Round(base12 * porc_pago, 2);
                    iva = decimal.Round(base12 * porc_iva, 2);
                    base0 = decimal.Parse(txtValor.Text) - base12 - iva;

                    if (base0 < decimal.Parse("0"))
                    {
                        porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                        base12 = _factura.GetBase12DescPromoIVA(false);
                        base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                        if (_factura.GetPromoIva() > 0)
                        {
                            base12 = base12 - ((base12 * PorcPromo) / 100);
                        }
                        base12 += base12PromoIVAExcluye;
                        iva = decimal.Round(base12 * porc_iva, 2);

                        base12 = base12 * porc_pago;
                        iva = base12 * porc_iva;
                        base0 = decimal.Parse(txtValor.Text) - base12 - iva;
                    }

                    trama.montoBaseGravaIVa = (base12).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                    trama.montoBaseNoGravaIVa = ((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0)).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                    trama.impuestoIvaTransaccion = (iva).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                }

                trama.impuestoServicioTransaccion = "";//12N 10N2D
                trama.popinaTransaccion = "";//12N 10N2D
                trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
                trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos

                if (string.IsNullOrEmpty(Control.Common.GlobalParameters.CID)) { trama.CID = "LIRISCID0" + _factura.Establecimiento + _factura.PtoEmision; }
                trama.CID = Control.Common.GlobalParameters.CID;

                var envioGenResponse = new ClsEnviaPinPadGeneral();
                ClsEnviaPinPadGeneral objContingente = new ClsEnviaPinPadGeneral();
                PinPadRespuesta PagoResp = new PinPadRespuesta();
                string strTrama = string.Empty;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Termino de asignar valores al objeto Trama ");

                trama.TipoMensaje = "PP";
                strTrama = trama.DevuelveTramaPPMultiRed.ToString();
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Enviando requerimiento PINPAD, trama: " + strTrama);

                try
                {
                    /*Se ejecuta el cobro por tarjeta. Se envia parametro*/
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Ejecuta metoro EjecutaTramaMultired");

                    PagoResp = new PinPadRespuesta();
                    PagoResp = envioGenResponse.EjecutaTrama(IPPinPad, PuertoPinPad, timeOutCP, strTrama, "", 1, "PP");

                    if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                    {
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.CodigoRespuesta : " + PagoResp.CodigoRespuesta);
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.CodigoRespuestaEntidad : " + PagoResp.CodigoRespuestaEntidad);


                        if (PagoResp.CodigoRespuesta == "-2" || PagoResp.CodigoRespuesta == "-1" || PagoResp.CodigoRespuesta == "-21")
                        {

                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.CodigoRespuestaEntidad: -2 || -1;" +
                                                    " objeto no encontrado o sin respuesta, Ejecuta trama para reverso ");

                            trama.TipoTransaccion = "04"; //Reversos de Transacciones de Compras Corrientes y Diferidos
                            string tramaReverso = trama.DevuelveTramaPPMultiRed.ToString();
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $"tramaReverso: {tramaReverso} ");
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Genera trama de reverso previo a generar el proceso de cobro por  establecimiento");
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Ejecuto Reverso de transaccion codRed: " + trama.codRed + "; Contiengente: " + Control.Common.GlobalParameters.ConectContingente.PinPadContingente);

                            PagoResp = new PinPadRespuesta();
                            PagoResp = envioGenResponse.EjecutaTrama(IPPinPad, PuertoPinPad, timeOutCP, tramaReverso, "", 1, "PP");

                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Respuesta Reverso: " + PagoResp.CodigoRespuesta + "; PagoResp.CodigoRespuestaEntidad: " + PagoResp.CodigoRespuestaEntidad);
                            if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                            {
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Genera reverso de trnasaccion incial");

                                ResponseBackground = "Error : " + PagoResp.MensajeRespuesta;
                                ResponseBackground = ResponseBackground + " \n" + "Reverso : " + PagoResp.MensajeRespuesta;

                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Reverso de transacción no exitosa, envio: " + trama.DevuelveTramaPPMultiRed);
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Reverso de transacción no exitosa, respuesta : " + resptrama.mensajeRespuesta);

                                On_Off_Controles(true);
                                return;
                            }

                            On_Off_Controles(true);
                            return;

                        }

                        if (PagoResp.CodigoRespuestaEntidad == "TO" || PagoResp.CodigoRespuestaEntidad == "20"
                                    || PagoResp.CodigoRespuestaEntidad == "91" || PagoResp.CodigoRespuestaEntidad == "96"
                                    || PagoResp.CodigoRespuestaEntidad == "09" || PagoResp.CodigoRespuesta == "20")
                        {

                            if (Control.Common.GlobalParameters.ConectContingente.PinPadContingente == false)
                            {
                                //PagoResp.MensajeRespuesta = "Fuera de Linea / TimeOut ";
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.MensajeRespuesta : " + PagoResp.MensajeRespuesta);


                                Control.Common.GlobalParameters.ConectContingente.ValidaRedPinPad = true;
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Contingente Activado");

                                Control.Common.GlobalParameters.ConectContingente.PinPadContingente = true;
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Contingente Activado");

                                DateTime FechaInicioEspera = DateTime.Now;
                                DateTime FechaFinEspera = FechaInicioEspera.AddMinutes(Control.Common.GlobalParameters.ConectContingente.TiempoEsperaContingente);
                                Control.Common.GlobalParameters.ConectContingente.FechaInicioEspera = FechaInicioEspera;
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "FechaInicioEspera: " + FechaInicioEspera);

                                Control.Common.GlobalParameters.ConectContingente.FechaFinEspera = FechaFinEspera;
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "FechaFinEspera: " + FechaFinEspera);


                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Valido contingente, Si PinPadContingente es false . Se activa contiengente");
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Valido contingente, Si PinPadContingente es TRUE . no se ejecuto parametros de contingente");


                                switch (autorizador)
                                {
                                    case "1":
                                        autorizador = "2";
                                        break;
                                    case "2":

                                        autorizador = "1";
                                        break;
                                }


                                Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = Int32.Parse(autorizador);
                                Control.Common.GlobalParameters.ConectContingente.Autorizador = Int32.Parse(autorizador);

                            }

                            string strRespuesta = "";
                            switch (PagoResp.CodigoRespuestaEntidad)
                            {
                                case "TO": strRespuesta = "Tiempo de Espera culminado, volver a intentar. "; break;
                                case "20": strRespuesta = "Error durante el proceso volver a intentar. "; break;
                                case "91": strRespuesta = "Banco Fuera de Línea. Volver a intentar"; break;
                                case "96": strRespuesta = "Banco Fuera de Línea. Tiempo de Espera. Volver a intentar"; break;
                                case "09": strRespuesta = "Mal funcionamiento del sistema, volver a intentar"; break;
                            }

                            PagoResp.MensajeRespuestaEntidad = strRespuesta;
                            ResponseBackground = "" + PagoResp.CodigoRespuestaEntidad + $"; {strRespuesta} ";
                            ResponseBackground = string.Concat(ResponseBackground, "");

                            On_Off_Controles(true);
                            return;

                        }
                        //Generó Return;

                        string Respuesta = string.Empty;
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Si la respuesta es diferente de CERO y el codigo no se encuentra asociado las contingente, se retorna error");

                        switch (PagoResp.CodigoRespuestaEntidad)
                        {
                            case "03": Respuesta = "Establecimiento Invalido."; break;
                            case "05": Respuesta = "Trans. Rechazada"; break;
                            case "51": Respuesta = "Fondos Insificientes"; break;

                            default: Respuesta = "Trans. Rechazada"; break;
                        }

                        ResponseBackground = "Error : " + PagoResp.MensajeRespuesta;
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $"PagoResp.CodigoRespuestaEntidad: {PagoResp.CodigoRespuestaEntidad}; ResponseBackground: {ResponseBackground} ");

                        On_Off_Controles(true);
                        return;



                    }


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $"Trama Respuesta: {PagoResp.TramaRespuesta}");

                    string RetornaRespuestaPago = PagoResp.MensajeRespuesta.Trim();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $"Ejecuta Metodo  PagoResp.GetDatosPago");
                    RepuestaPago Pago = PagoResp.GetDatosPago(PagoResp.TramaRespuesta, autorizador);


                    if (Pago.CodRespMsj != "00")
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $"Pago.CodRespMsj {Pago.CodRespMsj}; Pago.MsjRespMsjAut: {Pago.MsjRespMsjAut} ");

                        ResponseBackground = Pago.MsjRespMsjAut;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Pago.Valor " + Pago.MsjRespMsjAut);
                        return;
                    }


                    //Pago.Valor = valor;
                    resptrama.ObtieneDato = PagoResp.TramaRespuesta;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "asigno el valor / total de la factura al objeto Pago.valor;  " + valor);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Transacción Realizada Correctamente: " + PagoResp.TramaRespuesta);
                    //Si la respuesta del PinPad es CERO, entonces la transacción fue realizada con exito, por lo que se regsitra el voucher
                    var objGeneraVoucher = GeneraVoucher(Pago, trama, resptrama, bin_descripcion);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Respuesta objGeneraVoucher: "
                                                                                + "CodError:" + objGeneraVoucher.CodigoRespuesta + " Mensaje: "
                                                                                + objGeneraVoucher.MensajeRespuesta
                                                                                );

                    string numeroFactura = _factura.GetNumeroFactura();

                    //Objeto de respuesta al registrar el Voucer. Si no se registra 
                    if (objGeneraVoucher.CodigoRespuesta != "0")
                    {
                        string msjSeImprimieronVouchers = string.IsNullOrWhiteSpace(objGeneraVoucher.printWarnings) ? " y se imprimieron los recibos," : ", a pesar de que no se pudo imprimir los recibos en el momento,";
                        string msjParaDevteam = "En el POS del siguiente punto de emision, si bien se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers
                                              + " el voucher no pudo ser grabado en nuestra base interna. Generar el registro en POS_VOUCHER inmediatamente"
                                              + " pues puede provocar descuadres en los cierres. Esto pudo deberse a un breve inconveniente,"
                                              + " se recomienda una vez generado el registro, verificar la causa de la novedad";

                        ResponseBackground = "Error : " + resptrama.mensajeRespuesta;

                        string datosVoucher = "\nTARJETA: " + resptrama.numTarTuncate.Trim().PadRight(19, ' ') +
                                                    "\nCODIGOPROCESO: 000200" +
                                                    "\nFECHACONSUMO: " + resptrama.fechaTrans +
                                                    "\nHORACONSUMO: " + resptrama.horaTrans +
                                                    "\nNUMEROVOUCHER: " + resptrama.secuencialtransaccion +
                                                    "\nAUTORIZACION: " + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) +
                                                    "\nANULADO: " + (trama.TipoTransaccion == "03" ? "1" : "0") +
                                                    "\nVALORCONSUMO: " + trama.montoTotalTransaccion.PadLeft(13, '0') +
                                                    "\nFORMAAUTORIZA: 1" +
                                                    "\nTIPOCONSUMO: " + (resptrama.codigoRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData) +
                                                    "\nPLAZO: " + trama.plazoDiferido.PadLeft(2, '0') +
                                                    "\nTIPOLECTURA: " + resptrama.modoLectura.PadLeft(3, '0') +
                                                    "\nTIPOMONEDA: 840" +
                                                    "\nVALORIVA: " + trama.impuestoIvaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORSERVICIO: 0000000000000" +
                                                    "\nVALORPROPINA: " + trama.popinaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORINTERES: " + resptrama.valInteres.Trim().PadLeft(13, '0') +
                                                    "\nVALORFIJO: " + resptrama.montoFijo.Trim().PadLeft(13, '0') +
                                                    "\nTIPOPROMOCION: 00" +
                                                    "\nMESESGRACIA: " + trama.mesGracia.PadLeft(2, '0') +
                                                    "\nEMPRESASERVICIO: 0000" +
                                                    //"\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "R") +
                                                    "\nESTADOTRX: " + (objGeneraVoucher.tipoConsumo == "01" ? "O" : "O") +  //JCanarte
                                                    "\nCODIGORESPUESTA: " + resptrama.codigoRespuesta +
                                                    "\nTIPODISPOSITIVO: 2" +
                                                    "\nADQUIRENTETARJETA: CREDIMATIC01" +
                                                    "\nADQUIRENTESERVICIO:             " +
                                                    "\nMONTOGRAVAIVA: " + trama.montoBaseGravaIVa.PadLeft(13, '0') +
                                                    "\nMONTONOGRAVAIVA: " + trama.montoBaseNoGravaIVa.PadLeft(13, '0') +
                                                    "\nPUNTOEMISION: " + _factura.Establecimiento + _factura.PtoEmision +
                                                    "\nPROCESADO: 0" +
                                                    "\nGRUPOTAR: " + resptrama.nomGruTar +
                                                    "\nAUTORIZADOR: " + resptrama.codigoRed +
                                                    "\nLOTE: " + resptrama.numerolote +
                                                    "\nFACTURA: " + _factura.GetNumeroFactura() +
                                                    "\nARQC: " + resptrama.ARQC +
                                                    "\nAIDEMV: " + resptrama.AIDEMV +
                                                    "\nEMV: " + resptrama.idEMV +
                                                    "\nTC: " + resptrama.tipoCritoyValorEMV +
                                                    "\nPUBLICIDAD: " + resptrama.mensajePremioPublicidad +
                                                    "\nTIPOTRANSACCION: " + trama.TipoTransaccion +
                                                    "\nBANCOADQUIRIENTE: " + resptrama.nomBancoAdq +
                                                    "\nTARJETAHABIENTE: " + resptrama.nombreTarjetaHabiente +
                                                    "\nMID: " + resptrama.merchantId + //trama.MID +
                                                    "\nTID: " + trama.TID +
                                                    "\nVENCTAR: " + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") +
                                                  "\nANULAUTORIZACION: " + trama.numAutorizacion +
                                                    "\nTIPOBANCOTARJETA: " + (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);


                        datosVoucher += Environment.NewLine + Environment.NewLine + objGeneraVoucher.scriptInsertarVoucher;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero el voucher no pudo ser grabado en nuestra base interna, " + Environment.NewLine + "a continuacion las excepciones encontradas - " +
                                                                  Control.Common.ExceptionHandler.GetExceptionMessages(objGeneraVoucher.exception)
                                                                  + Environment.NewLine + "StackTrace:"
                                                                  + Environment.NewLine + objGeneraVoucher.StackTrace
                                                                  + Environment.NewLine + "Datos Voucher: " + datosVoucher.Replace("\n", Environment.NewLine));


                        try
                        {

                            var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                Properties.Settings.Default.MAILERROR_FROM,
                                Properties.Settings.Default.MAILERROR_ALIAS,
                                Properties.Settings.Default.MAILERROR_DESTINO,
                                Properties.Settings.Default.MAILERROR_CC,

                                "Voucher pinpad no se grabó en nuestra base interna",
                                String.Format(msjParaDevteam +
                                                "  \n\nDatos caja-----------------" +
                                                "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                                "  \n\nDatos voucher-----------------" +
                                                datosVoucher,
                                            Control.Common.GlobalParameters.Establecimiento,
                                            Control.Common.GlobalParameters.PuntoEmision,
                                            Control.Common.GlobalParameters.IpMaquina,
                                            Control.Common.GlobalParameters.UsuarioNombre,
                                            Control.Common.GlobalParameters.Usuario,
                                            Control.Common.ExceptionHandler.GetExceptionMessages(objGeneraVoucher.exception),
                                            (objGeneraVoucher.fueProcesadoImpresionVocuhers ? "" : ("StackTrace: " + objGeneraVoucher.StackTrace + " \n"))),
                                false, String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "No se pudo enviar notificacion del problema al grabar voucher en nuestra base interna, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }

                            //eevv 2020-01-13
                            try
                            {
                                string Stringremove = string.Empty;
                                Stringremove = "SCRIPT PARA INSERTAR: " + Environment.NewLine;
                                creaArhivoTemporalVoucher(objGeneraVoucher.scriptInsertarVoucher.Replace(Stringremove, string.Empty));
                            }
                            catch (Exception ex3)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "No se pudo generar archivo TXT de insert para la POS_VOUCHER , a continuacion las excepciones encontradas - " + ex3.Message);

                            }
                        }
                        catch { }

                        ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero detectamos que el voucher no pudo ser grabado en nuestra base interna. Por favor comuníquelo inmediatamente al administrador para que realice su gestión y evitar descuadre durante su cierre";

                        //Agregar lineas de insert
                        Control.Common.Logger.Agregar_Trace_Voucher(objGeneraVoucher.pos_voucher);

                        DebeCerrarFormBackground = true;

                        return;
                    }

                    if (Pago.CodRed == "01")
                        nombreRed = "DATAFAST";
                    else if (Pago.CodRed == "02")
                        nombreRed = "MEDIANET";
                    else if (Pago.CodRed == "03")
                        nombreRed = "AUSTRO";


                    //Agregar el pago a un repositorio temporal
                    PagoPinpadBackground = new Models.PagoPinpad
                    {
                        Valor = valor,
                        Banco = nombreRed,
                        Nombre = Pago.GrupoTarjeta,
                        Marca = Pago.NombBcoAdq.Trim(),
                        TipoPos = nombreRed,
                        BinDescripcion = bin_descripcion,
                        NumBin = resultadolectura.NumBin
                    };

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Imprime voucher Original.'" + _factura.GetNumeroFactura() + "' ");
                    string printResponse = string.Empty;
                    string printWarnings = string.Empty;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "objGeneraVoucher.tipovoucher " + objGeneraVoucher.tipovoucher + "' ");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Inserta el valor de la trama en el modelo PagoPinpadBackground.'" + numeroFactura + "' ");


                    /*Prepara voucher para imprimir*/
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Imprime voucher Original.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                    prepararVoucherTarjeta(Pago, objGeneraVoucher.tipovoucher, resptrama, trama, ref printResponse, false);
                    printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 1: " + Environment.NewLine + printResponse + Environment.NewLine));

                    printResponse = string.Empty;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Imprime voucher Copia.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                    prepararVoucherTarjeta(Pago, objGeneraVoucher.tipovoucher, resptrama, trama, ref printResponse, true);

                    printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 2: " + Environment.NewLine + printResponse + Environment.NewLine));

                    if (!string.IsNullOrWhiteSpace(printWarnings))
                    {
                        ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco pero algunos recibos no pudieron ser impresos en este momento. Por favor realice la reimpresion usando el comando F4 y seleccionando la opcion Reimpresion de Vouchers";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Ocurrieron incidencias durante la impresion de los recibos voucher en la factura '" + _factura.GetNumeroFactura() + "'" + Environment.NewLine + printWarnings);
                    }

                    //Agregar lineas de insert
                    Control.Common.Logger.Agregar_Trace_Voucher(objGeneraVoucher.pos_voucher);
                    DebeCerrarFormBackground = true;


                }
                catch (Exception ex)
                {

                    ResponseBackground = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a una breve interrupcion en el servicio, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco";

                    Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed"
                            , "La solicitud no pudo ser realizada debido a una breve interrupcion en el servicio, esto puede deberse a un breve mantenimiento, por favor antes de volverlo a intentar verifique "
                            + "  que transaccion no haya sido cargada al cliente en el banco. A continuacion las excepciones encontradas - "
                            + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace "
                            + ex.StackTrace);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "ProcesaPinpadBackgroundMultiRed", "error: " + ex.Message);
                }


            }
            catch (Exception ex)
            {
                ResponseBackground = "Error al generar las tramas, favor revisar con el Dpto de Sistemas.";
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "ResponseBackground " + ResponseBackground);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "ProcesaPinpadBackgroundMultiRed", "error: " + ex.Message);
            }
            finally
            {
                On_Off_Controles(true);
            }



        }
        public void ProcesaPinpadBackgroundMultiRedAnt()
        {
            ResponseBackground = string.Empty;
            DebeCerrarFormBackground = false;
            On_Off_Controles(false);
            PagoPinpadBackground = null;

            POS_VOUCHER pos_voucher = new POS_VOUCHER();
            ClsEnviaPinPadGeneral envioGen = new ClsEnviaPinPadGeneral();
            Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
            Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();

            string autorizador = "2";
            string IPPinPad = string.Empty;
            int PuertoPinPad = 0;
            PinPadRespuesta resultadolectura = new PinPadRespuesta();

            var pos = new POSEntities();
            string TarjetaCreditoBin = string.Empty;
            string plazoDiferido = string.Empty;
            string mesGracia = string.Empty;
            string bin_descripcion = string.Empty;
            string valpag = string.Empty;
            decimal valor = 0;
            int timeOutCP = Control.Common.GlobalParameters.ConectContingente.TiempoOutCP;
            string nombreRed = "";
            string MID = string.Empty;
            string TID = string.Empty;

            PagoPinpadBackground = null;


            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Ejecuta proceso ProcesaPinpadBackgroundMultiRed ");

                if (Control.Common.GlobalParameters.ConectContingente.EsPinPad)
                {
                    trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                    valpag = Decimal.Round(Decimal.Parse(txtValor.Text) * 1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";
                    valor = Decimal.Parse(txtValor.Text);

                    if (_factura.AplicaDescuentoPromoBines || !Control.Common.GlobalParameters.ActivaVersionPinPadMedianet)
                    {

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Se invoca General.ValidaContingente, en caso de que el servicio de Medianet se ha reestablecido o el tiempo para generar pruebas ha concluido");
                        Control.Common.General.ValidaContingente();

                        IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                        PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " IP PinPad: " + IPPinPad + "; PuertoPinPad: " + PuertoPinPad);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " GlobalParameters.IPPinPad: " + Control.Common.GlobalParameters.IPPinPad
                                                     + "; GlobalParameters.PuertoPinPad: " + Control.Common.GlobalParameters.PuertoPinPad);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Lectura Pinpad ");
                        resultadolectura = envioGen.LecturaTarjeta(IPPinPad, PuertoPinPad, 65000, "LT", "", 1);


                        //Si la lectura de la tarjeta es exitosa, se realiza el proceso de pago (PP)
                        if (resultadolectura.CodigoRespuesta == "00")
                        {
                            int leerTramaRes = 75;
                            if (resultadolectura.TramaRespuesta.Length > 98)
                            {
                                leerTramaRes = leerTramaRes + 24;
                            }

                            var ConsultaBin = pos.core_tarjetacredito_bin.Where(x => x.bin == resultadolectura.NumBin).FirstOrDefault();

                            if (ConsultaBin != null)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Recupero el valor de la Red Autorizador, según el BIN de la Tarjeta ");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Autorizador BIN: " + ConsultaBin.bin_red);
                                // autorizador = trama.TipoTransaccion == "01" ? ConsultaBin.bin_red : ConsultaBin.bin_red_cred;
                                bin_descripcion = ConsultaBin.bin_descripcion;
                                //autorizador = Control.Common.GlobalParameters.ConectContingente.Autorizador.ToString();

                                MID = string.Empty;
                                TID = string.Empty;

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Asigno valor por defecto de autorizador. basado en las referencias de la tabla BIN ");
                                autorizador = ConsultaBin.bin_red;

                                //trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Valida contingente PINPAD; por defecto MEDIANET se encuentra configurado por defecto ");

                                //autorizador = Control.Common.GlobalParameters.AutorizadorDefault.ToString();
                                if (Control.Common.GlobalParameters.ConectContingente.PinPadContingente)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "ConectContingente.PinPadContingente: " + Control.Common.GlobalParameters.ConectContingente.PinPadContingente);

                                    if (Control.Common.GlobalParameters.ConectContingente.Autorizador.ToString() != autorizador)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "ConectContingente.Autorizador: " + Control.Common.GlobalParameters.ConectContingente.Autorizador + "; autorizador: " + autorizador);
                                        switch (autorizador)
                                        {
                                            case "1":
                                                autorizador = "2";
                                                break;
                                            case "2":
                                                autorizador = "1";
                                                break;
                                        }
                                    }

                                    Control.Common.GlobalParameters.AutorizadorDefault = Int32.Parse(autorizador);
                                    Control.Common.GlobalParameters.ConectContingente.Autorizador = Int32.Parse(autorizador);
                                }


                                if (autorizador == "2")
                                {
                                    IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                                    PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                                    MID = Control.Common.GlobalParameters.MID_MEDIANET;
                                    TID = Control.Common.GlobalParameters.TID_MEDIANET;
                                }
                                else
                                {

                                    IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadDATAFAST;
                                    PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadDataFast;

                                    MID = Control.Common.GlobalParameters.MID_DATAFAST;
                                    TID = Control.Common.GlobalParameters.TID_DATAFAST;
                                }

                                trama.codRed = autorizador;
                                trama.MID = MID;
                                trama.TID = TID;

                                if (!Control.Common.GlobalParameters.EstTcpIpPinpad)
                                {
                                    ResponseBackground = Common.GlobalParameters.PinpadMsjAutorizadorNoValido;
                                    return;
                                }
                            }
                            else
                            {//Validación de Objeto ConsultaBin
                                ResponseBackground = "Tarjeta no se encuentra en listado de bines";
                                return;
                            }
                        }
                        else
                        {
                            ResponseBackground = "Error en PINPAD: " + resultadolectura.CodigoRespuesta + " - " + resultadolectura.MensajeRespuesta + "\n\n";
                            ResponseBackground = ResponseBackground + " IP PinPad: " + IPPinPad + "; PuertoPinPad: " + PuertoPinPad;
                            ResponseBackground = ResponseBackground + " GlobalParameters.IPPinPad: " + Control.Common.GlobalParameters.IPPinPad + "\n\n";
                            ResponseBackground = ResponseBackground + " GlobalParameters.PuertoPinPad: " + Control.Common.GlobalParameters.PuertoPinPad + "\n\n";

                            return;
                        }

                    }
                    else
                    {
                        trama.codRed = "2";
                        trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                        trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;
                        trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                    }


                    string codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                    //trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                    if (autorizador == "1")
                    {
                        //trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                        codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                    }
                    trama.codDiferido = codDiferido;

                    plazoDiferido = cmbDiferido.Text.PadLeft(2, ' ');
                    trama.plazoDiferido = plazoDiferido;

                    mesGracia = cmbMesesGracia.Text.PadLeft(2, ' ');
                    trama.mesGracia = mesGracia;

                    //  MessageBox.Show(this,"Verificar el calculo para  armar los totales");
                    var porc_iva = Control.Common.GlobalParameters.IVAGEN / 100;
                    var PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;/// Decimal.Parse((pos.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));
                    var porc_pago = (decimal.Parse(txtValor.Text) / _factura.GetTotal());
                    decimal porc_Desc2 = 0;

                    if (_facturaApp == null)
                    {
                        porc_Desc2 = 0;
                    }
                    else
                        if (_facturaApp.Descuentos2.Count > 0)
                        {
                            porc_Desc2 = _facturaApp.Descuentos2.Max(x => x.Porcentaje);
                        }

                    var base0 = _factura.GetBase0() - (_factura.GetDescuentos() - (_factura.GetBase12() - _factura.GetBase12Desc()));
                    base0 = base0 - (base0 * (porc_Desc2 / 100));
                    var base12 = _factura.GetBase12DescPromoIVA(false);
                    base12 = base12 - (base12 * (porc_Desc2 / 100));

                    var base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                    if (_factura.GetPromoIva() > 0)
                    {
                        base12 = base12 - ((base12 * PorcPromo) / 100);
                    }

                    base12 += base12PromoIVAExcluye;
                    var iva = decimal.Round(base12 * porc_iva, 2);
                    trama.montoTotalTransaccion = valpag;//12N 10N2D

                    if (decimal.Parse(txtValor.Text) == _factura.GetTotal())
                    {
                        trama.montoBaseGravaIVa = decimal.Round((base12 * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = decimal.Round(((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0) * porc_pago), 2, MidpointRounding.ToEven).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = decimal.Round((iva * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                    }
                    else
                    {

                        base12 = Decimal.Round(base12 * porc_pago, 2);
                        iva = decimal.Round(base12 * porc_iva, 2);
                        base0 = decimal.Parse(txtValor.Text) - base12 - iva;

                        if (base0 < decimal.Parse("0"))
                        {
                            porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                            base12 = _factura.GetBase12DescPromoIVA(false);
                            base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                            if (_factura.GetPromoIva() > 0)
                            {
                                base12 = base12 - ((base12 * PorcPromo) / 100);
                            }
                            base12 += base12PromoIVAExcluye;
                            iva = decimal.Round(base12 * porc_iva, 2);

                            base12 = base12 * porc_pago;
                            iva = base12 * porc_iva;
                            base0 = decimal.Parse(txtValor.Text) - base12 - iva;
                        }

                        trama.montoBaseGravaIVa = (base12).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = ((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0)).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = (iva).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                    }

                    trama.impuestoServicioTransaccion = "";//12N 10N2D
                    trama.popinaTransaccion = "";//12N 10N2D
                    trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                    trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                    trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                    trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
                    trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos
                                               // trama.CID = _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 

                    //CID123456789012
                    // trama.CID = "LIRISCID0" + _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 
                    trama.CID = Control.Common.GlobalParameters.CID;

                    var envioGenResponse = new ClsEnviaPinPadGeneral();
                    ClsEnviaPinPadGeneral objContingente = new ClsEnviaPinPadGeneral();
                    PinPadRespuesta PagoResp = new PinPadRespuesta();
                    string strTrama = string.Empty;


                    trama.TipoMensaje = "PP";
                    strTrama = trama.DevuelveTrama.ToString();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Enviando requerimiento PINPAD, trama: " + strTrama);


                    /*Se ejecuta el cobro por tarjeta. Se envia parametro*/
                    PagoResp = new PinPadRespuesta();
                    PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, trama.codRed, strTrama, "", 1, "PP");

                    //Si la respuesta es difernete de CERO, ingresa a las excepciones 
                    if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                    {

                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.CodigoRespuesta : " + PagoResp.CodigoRespuesta);
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.CodigoRespuestaEntidad : " + PagoResp.CodigoRespuestaEntidad);

                        /* si el objeto me devuelve con CodigoRespuesta = 20 y RespuestaEntidad -1 , eso indica que el equipo no respondio la petición o se perdio el proceso*/
                        /* Esto va a disparar el reverso sobre el mismo cobro generado previamente */

                        if (PagoResp.CodigoRespuestaEntidad == "-1")
                        {
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.CodigoRespuestaEntidad: -1; objeto no encontrado o sin respuesta, se genera reverso ");
                            PagoResp = new PinPadRespuesta();

                            trama.TipoTransaccion = "04"; //Reversos de Transacciones de Compras Corrientes y Diferidos
                            string tramaReverso = trama.DevuelveTrama.ToString();
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Genera trama de reverso previo a generar el proceso de cobro por otro establecimiento");

                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Ejecuto Reverso de transaccion codRed: " + trama.codRed + "; Contiengente: " + Control.Common.GlobalParameters.ConectContingente.PinPadContingente);
                            PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, autorizador, tramaReverso, "", 1, "PP");

                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Respuesta Reverso: " + PagoResp.CodigoRespuesta + "; PagoResp.CodigoRespuestaEntidad: " + PagoResp.CodigoRespuestaEntidad);
                            if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                            {
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Genera reverso de trnasaccion incial");

                                ResponseBackground = "Error : " + PagoResp.MensajeRespuesta;
                                ResponseBackground = ResponseBackground + " \n" + "Reverso : " + PagoResp.MensajeRespuesta;

                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Reverso de transacción no exitosa, envio: " + trama.DevuelveTrama);
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Reverso de transacción no exitosa, respuesta : " + resptrama.mensajeRespuesta);

                                On_Off_Controles(true);
                                return;
                            }

                            ResponseBackground = "Reverso generado, favor volver a intentar el cobro. ";
                            On_Off_Controles(true);
                            return;

                        }


                        if (PagoResp.CodigoRespuestaEntidad == "TO" || PagoResp.CodigoRespuestaEntidad == "20"
                            || PagoResp.CodigoRespuestaEntidad == "91" || PagoResp.CodigoRespuestaEntidad == "96"
                            || PagoResp.CodigoRespuestaEntidad == "09")
                        {
                            PagoResp.MensajeRespuesta = "Fuera de Linea / TimeOut ";
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "PagoResp.MensajeRespuesta : " + PagoResp.MensajeRespuesta);

                            Control.Common.GlobalParameters.ConectContingente.ValidaRedPinPad = true;
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Contingente Activado");

                            Control.Common.GlobalParameters.ConectContingente.PinPadContingente = true;
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Contingente Activado");

                            DateTime FechaInicioEspera = DateTime.Now;
                            DateTime FechaFinEspera = FechaInicioEspera.AddMinutes(Control.Common.GlobalParameters.ConectContingente.TiempoEsperaContingente);
                            Control.Common.GlobalParameters.ConectContingente.FechaInicioEspera = FechaInicioEspera;
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "FechaInicioEspera: " + FechaInicioEspera);

                            Control.Common.GlobalParameters.ConectContingente.FechaFinEspera = FechaFinEspera;
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "FechaFinEspera: " + FechaFinEspera);


                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Valido contingente, Si PinPadContingente es false . Se activa contiengente");
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Valido contingente, Si PinPadContingente es TRUE . no se ejecuto parametros de contingente");

                            if (Control.Common.GlobalParameters.ConectContingente.PinPadContingente == false)
                            {
                                switch (autorizador)
                                {
                                    case "1":
                                        autorizador = "2";
                                        trama.codRed = autorizador;
                                        trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                                        trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;
                                        break;
                                    case "2":

                                        autorizador = "1";
                                        trama.codRed = autorizador;
                                        trama.MID = Control.Common.GlobalParameters.MID_DATAFAST;
                                        trama.TID = Control.Common.GlobalParameters.TID_DATAFAST;
                                        break;
                                }

                                Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = Int32.Parse(autorizador);
                                Control.Common.GlobalParameters.ConectContingente.Autorizador = Int32.Parse(autorizador);

                                string strTramaCont = trama.DevuelveTrama.ToString();

                                PagoResp = new PinPadRespuesta();
                                PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, autorizador, strTramaCont, "", 1, "PP");

                                if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                                {
                                    Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Genera Reverso de contingente");

                                    ResponseBackground = "Error : " + PagoResp.MensajeRespuesta;
                                    On_Off_Controles(true);
                                    return;
                                }

                            }

                        }
                        else
                        {
                            string Respuesta = string.Empty;

                            switch (PagoResp.CodigoRespuestaEntidad)
                            {
                                case "03": Respuesta = "Establecimiento Invalido."; break;
                                case "05": Respuesta = "Trans. Rechazada"; break;
                            }

                            ResponseBackground = "Error : " + PagoResp.MensajeRespuesta;
                            On_Off_Controles(true);
                            return;

                        }
                    }

                    string RetornaRespuesta = PagoResp.MensajeRespuesta.Trim();
                    RepuestaPago Pago = PagoResp.GetDatosPago(PagoResp.TramaRespuesta, autorizador);

                    if (Pago.CodRespMsj != "00")
                    {

                        ResponseBackground = Pago.MsjRespMsjAut;

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Pago.Valor " + Pago.MsjRespMsjAut);
                        return;
                    }


                    //Pago.Valor = valor;
                    resptrama.ObtieneDato = PagoResp.TramaRespuesta;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "asigno el valor / total de la factura al objeto Pago.valor;  " + valor);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Transacción Realizada Correctamente: " + PagoResp.TramaRespuesta);
                    //Si la respuesta del PinPad es CERO, entonces la transacción fue realizada con exito, por lo que se regsitra el voucher
                    var objGeneraVoucher = GeneraVoucher(Pago, trama, resptrama, bin_descripcion);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Respuesta objGeneraVoucher: "
                                                                                + "CodError:" + objGeneraVoucher.CodigoRespuesta + " Mensaje: "
                                                                                + objGeneraVoucher.MensajeRespuesta
                                                                                );

                    string numeroFactura = _factura.GetNumeroFactura();

                    //Objeto de respuesta al registrar el Voucer. Si no se registra 
                    if (objGeneraVoucher.CodigoRespuesta != "0")
                    {
                        string msjSeImprimieronVouchers = string.IsNullOrWhiteSpace(objGeneraVoucher.printWarnings) ? " y se imprimieron los recibos," : ", a pesar de que no se pudo imprimir los recibos en el momento,";
                        string msjParaDevteam = "En el POS del siguiente punto de emision, si bien se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers
                                              + " el voucher no pudo ser grabado en nuestra base interna. Generar el registro en POS_VOUCHER inmediatamente"
                                              + " pues puede provocar descuadres en los cierres. Esto pudo deberse a un breve inconveniente,"
                                              + " se recomienda una vez generado el registro, verificar la causa de la novedad";

                        ResponseBackground = "Error : " + resptrama.mensajeRespuesta;

                        string datosVoucher = "\nTARJETA: " + resptrama.numTarTuncate.Trim().PadRight(19, ' ') +
                                                    "\nCODIGOPROCESO: 000200" +
                                                    "\nFECHACONSUMO: " + resptrama.fechaTrans +
                                                    "\nHORACONSUMO: " + resptrama.horaTrans +
                                                    "\nNUMEROVOUCHER: " + resptrama.secuencialtransaccion +
                                                    "\nAUTORIZACION: " + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) +
                                                    "\nANULADO: " + (trama.TipoTransaccion == "03" ? "1" : "0") +
                                                    "\nVALORCONSUMO: " + trama.montoTotalTransaccion.PadLeft(13, '0') +
                                                    "\nFORMAAUTORIZA: 1" +
                                                    "\nTIPOCONSUMO: " + (resptrama.codigoRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData) +
                                                    "\nPLAZO: " + trama.plazoDiferido.PadLeft(2, '0') +
                                                    "\nTIPOLECTURA: " + resptrama.modoLectura.PadLeft(3, '0') +
                                                    "\nTIPOMONEDA: 840" +
                                                    "\nVALORIVA: " + trama.impuestoIvaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORSERVICIO: 0000000000000" +
                                                    "\nVALORPROPINA: " + trama.popinaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORINTERES: " + resptrama.valInteres.Trim().PadLeft(13, '0') +
                                                    "\nVALORFIJO: " + resptrama.montoFijo.Trim().PadLeft(13, '0') +
                                                    "\nTIPOPROMOCION: 00" +
                                                    "\nMESESGRACIA: " + trama.mesGracia.PadLeft(2, '0') +
                                                    "\nEMPRESASERVICIO: 0000" +
                                                    //"\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "R") +
                                                    "\nESTADOTRX: " + (objGeneraVoucher.tipoConsumo == "01" ? "O" : "O") +  //JCanarte
                                                    "\nCODIGORESPUESTA: " + resptrama.codigoRespuesta +
                                                    "\nTIPODISPOSITIVO: 2" +
                                                    "\nADQUIRENTETARJETA: CREDIMATIC01" +
                                                    "\nADQUIRENTESERVICIO:             " +
                                                    "\nMONTOGRAVAIVA: " + trama.montoBaseGravaIVa.PadLeft(13, '0') +
                                                    "\nMONTONOGRAVAIVA: " + trama.montoBaseNoGravaIVa.PadLeft(13, '0') +
                                                    "\nPUNTOEMISION: " + _factura.Establecimiento + _factura.PtoEmision +
                                                    "\nPROCESADO: 0" +
                                                    "\nGRUPOTAR: " + resptrama.nomGruTar +
                                                    "\nAUTORIZADOR: " + resptrama.codigoRed +
                                                    "\nLOTE: " + resptrama.numerolote +
                                                    "\nFACTURA: " + _factura.GetNumeroFactura() +
                                                    "\nARQC: " + resptrama.ARQC +
                                                    "\nAIDEMV: " + resptrama.AIDEMV +
                                                    "\nEMV: " + resptrama.idEMV +
                                                    "\nTC: " + resptrama.tipoCritoyValorEMV +
                                                    "\nPUBLICIDAD: " + resptrama.mensajePremioPublicidad +
                                                    "\nTIPOTRANSACCION: " + trama.TipoTransaccion +
                                                    "\nBANCOADQUIRIENTE: " + resptrama.nomBancoAdq +
                                                    "\nTARJETAHABIENTE: " + resptrama.nombreTarjetaHabiente +
                                                    "\nMID: " + resptrama.merchantId + //trama.MID +
                                                    "\nTID: " + trama.TID +
                                                    "\nVENCTAR: " + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") +
                                                    "\nANULAUTORIZACION: " + trama.numAutorizacion +
                                                    "\nTIPOBANCOTARJETA: " + (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);


                        datosVoucher += Environment.NewLine + Environment.NewLine + objGeneraVoucher.scriptInsertarVoucher;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero el voucher no pudo ser grabado en nuestra base interna, " + Environment.NewLine + "a continuacion las excepciones encontradas - " +
                                                                  Control.Common.ExceptionHandler.GetExceptionMessages(objGeneraVoucher.exception)
                                                                  + Environment.NewLine + "StackTrace:"
                                                                  + Environment.NewLine + objGeneraVoucher.StackTrace
                                                                  + Environment.NewLine + "Datos Voucher: " + datosVoucher.Replace("\n", Environment.NewLine));


                        try
                        {

                            var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                Properties.Settings.Default.MAILERROR_FROM,
                                Properties.Settings.Default.MAILERROR_ALIAS,
                                Properties.Settings.Default.MAILERROR_DESTINO,
                                Properties.Settings.Default.MAILERROR_CC,

                                "Voucher pinpad no se grabó en nuestra base interna",
                                String.Format(msjParaDevteam +
                                                "  \n\nDatos caja-----------------" +
                                                "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                                "  \n\nDatos voucher-----------------" +
                                                datosVoucher,
                                            Control.Common.GlobalParameters.Establecimiento,
                                            Control.Common.GlobalParameters.PuntoEmision,
                                            Control.Common.GlobalParameters.IpMaquina,
                                            Control.Common.GlobalParameters.UsuarioNombre,
                                            Control.Common.GlobalParameters.Usuario,
                                            Control.Common.ExceptionHandler.GetExceptionMessages(objGeneraVoucher.exception),
                                            (objGeneraVoucher.fueProcesadoImpresionVocuhers ? "" : ("StackTrace: " + objGeneraVoucher.StackTrace + " \n"))),
                                false, String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "No se pudo enviar notificacion del problema al grabar voucher en nuestra base interna, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }

                            //eevv 2020-01-13
                            try
                            {
                                string Stringremove = string.Empty;
                                Stringremove = "SCRIPT PARA INSERTAR: " + Environment.NewLine;
                                creaArhivoTemporalVoucher(objGeneraVoucher.scriptInsertarVoucher.Replace(Stringremove, string.Empty));
                            }
                            catch (Exception ex3)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "No se pudo generar archivo TXT de insert para la POS_VOUCHER , a continuacion las excepciones encontradas - " + ex3.Message);

                            }
                        }
                        catch { }

                        ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero detectamos que el voucher no pudo ser grabado en nuestra base interna. Por favor comuníquelo inmediatamente al administrador para que realice su gestión y evitar descuadre durante su cierre";

                        //Agregar lineas de insert
                        Control.Common.Logger.Agregar_Trace_Voucher(objGeneraVoucher.pos_voucher);

                        DebeCerrarFormBackground = true;

                        return;
                    }

                    if (Pago.CodRed == "01")
                        nombreRed = "DATAFAST";
                    else if (Pago.CodRed == "02")
                        nombreRed = "MEDIANET";
                    else if (Pago.CodRed == "03")
                        nombreRed = "AUSTRO";


                    //Agregar el pago a un repositorio temporal
                    PagoPinpadBackground = new Models.PagoPinpad
                    {
                        Valor = valor,
                        Banco = nombreRed,
                        Nombre = Pago.GrupoTarjeta,
                        Marca = Pago.NombBcoAdq.Trim(),
                        TipoPos = nombreRed,
                        BinDescripcion = bin_descripcion,
                        NumBin = resultadolectura.NumBin
                    };

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Imprime voucher Original.'" + _factura.GetNumeroFactura() + "' ");
                    string printResponse = string.Empty;
                    string printWarnings = string.Empty;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "objGeneraVoucher.tipovoucher " + objGeneraVoucher.tipovoucher + "' ");


                    //tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCreditoSinFirma;
                    //if (valor >= Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma)
                    //{
                    //    tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                    //}

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Inserta el valor de la trama en el modelo PagoPinpadBackground.'" + numeroFactura + "' ");


                    /*Prepara voucher para imprimir*/
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Imprime voucher Original.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                    prepararVoucherTarjeta(Pago, objGeneraVoucher.tipovoucher, resptrama, trama, ref printResponse, false);
                    printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 1: " + Environment.NewLine + printResponse + Environment.NewLine));

                    printResponse = string.Empty;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Imprime voucher Copia.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                    prepararVoucherTarjeta(Pago, objGeneraVoucher.tipovoucher, resptrama, trama, ref printResponse, true);

                    printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 2: " + Environment.NewLine + printResponse + Environment.NewLine));

                    if (!string.IsNullOrWhiteSpace(printWarnings))
                    {
                        ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco pero algunos recibos no pudieron ser impresos en este momento. Por favor realice la reimpresion usando el comando F4 y seleccionando la opcion Reimpresion de Vouchers";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "Ocurrieron incidencias durante la impresion de los recibos voucher en la factura '" + _factura.GetNumeroFactura() + "'" + Environment.NewLine + printWarnings);
                    }

                    //Agregar lineas de insert
                    Control.Common.Logger.Agregar_Trace_Voucher(objGeneraVoucher.pos_voucher);
                    DebeCerrarFormBackground = true;

                }

            }
            catch (Exception ex)
            {
                ResponseBackground = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a una breve interrupcion en el servicio, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco";

                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpadBackgroundMultiRed"
                        , "La solicitud no pudo ser realizada debido a una breve interrupcion en el servicio, esto puede deberse a un breve mantenimiento, por favor antes de volverlo a intentar verifique "
                        + "  que transaccion no haya sido cargada al cliente en el banco. A continuacion las excepciones encontradas - "
                        + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace "
                        + ex.StackTrace);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "ProcesaPinpadBackgroundMultiRed", "error: " + ex.Message);
            }
            finally
            {
                On_Off_Controles(true);
            }


        }

        public RespuestaVoucher GeneraVoucher(RepuestaPago repuestaPago, Tramas.ProcesaPago trama, Tramas.RespuestaProcesoPago resptrama, string bin_descripcion)
        {
            RespuestaVoucher objRespuesta = new RespuestaVoucher();

            DateTime VencTarjeta = new DateTime();
            string FacturaComprobante = string.Empty;
            string NumFactura = string.Empty;
            string tipoConsumo = string.Empty;
            string tipovoucher = "";
            string TramaCobro = string.Empty;
            string codigoproceso = "000200";

            string printWarnings = string.Empty;
            string scriptInsertarVoucher = string.Empty;

            string autori = string.Empty;

            DetVoucher detVoucher = new DetVoucher();
            POS_VOUCHER pos_voucher = new POS_VOUCHER();

            string valPago = string.Empty;
            decimal valorTC = 0M;
            Decimal valor = Decimal.Parse(txtValor.Text);

            try
            {
                EsPagoOkPromoTarjeta = true;
                objRespuesta.EsPagoOkPromoTarjeta = EsPagoOkPromoTarjeta;

                NumFactura = _factura.GetNumeroFactura();
                FacturaComprobante = _factura.GetNumeroFacturaEnmascarado();
                tipoConsumo = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                autori = repuestaPago.NumAutorizacion;


                if (repuestaPago.FechaVenc != 0)
                {
                    string mesVenc = repuestaPago.FechaVenc.ToString().Substring(2, 2);
                    string AnioVenc = repuestaPago.FechaVenc.ToString().Substring(0, 2);
                    VencTarjeta = DateTime.ParseExact("01-" + mesVenc + "-" + AnioVenc, "dd-MM-yy", CultureInfo.InvariantCulture);

                }

                string fvencTarj = Convert.ToString(VencTarjeta).Substring(5, 2) + '/' + Convert.ToString(VencTarjeta).Substring(0, 4);
                fvencTarj = fvencTarj.Length > 5 ? VencTarjeta.ToString("MM/yy") : fvencTarj;


                if (repuestaPago.CodRed == "02")
                    tipoConsumo = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo;


                tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCreditoSinFirma;
                if (valor > Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma)
                {
                    tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                }

                //if (Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma > 0 && valor <= Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma)
                //{
                //    tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCreditoSinFirma;
                //}
                //else
                //{
                //    tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                //}

                //tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                //if (Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma > 0 && repuestaPago.Valor <= Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma)
                //    tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCreditoSinFirma;


                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", "Previo a obtener trama pinpad (BinDescripcion).'" + NumFactura + "' : '" + bin_descripcion + "'");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", "Cambio realizado por evelasco para identificar version.'");


                try
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", "Obtiene Fecha Transaccion pinpad.'" + NumFactura + "' : " + repuestaPago.FechaTrans);
                    //autori = trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut;  ;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", "Obtiene Numero Autorización pinpad.'" + NumFactura + "' : " + autori + " ");

                    valorTC = repuestaPago.Valor;
                    TramaCobro = "|" + resptrama.secuencialtransaccion + ";" + autori + ";" + resptrama.fechaTrans + ";" + valorTC.ToString();
                    //evelasco se envia trama para utilizar el proceso AX para cruzar con los cobros por TC - MEDIANET, DATAFAST.

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", "Obtiene Trama del pinpad (Num Voucher+Autorización+fecha Transaccion+ Valor Transacción) pinpad '" + NumFactura + "' :" + TramaCobro);
                    bin_descripcion = bin_descripcion + " " + TramaCobro;
                }
                catch (Exception exTramaCobro)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "GeneraVoucher", "Error al Obtener la Trama del PinPad para identificar en el diario de Cobro (PaymentNotes)'" + NumFactura + "'" + Environment.NewLine + exTramaCobro.InnerException.Message);

                }
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", "Después de armar trama para bin_descripcion.'" + NumFactura + "' : '" + bin_descripcion + "'");

                string nombreRed = Control.Common.General.ObtenerDescripcionRed(trama.codRed);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", "Inserta el valor de la trama en el modelo PagoPinpadBackground.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-23
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", " Asignacion de variables para grabar voucher ");//Jorge Medina 19/01/2025

                detVoucher = new DetVoucher();
                detVoucher.TARJETA = repuestaPago.NumTrajetaPayClub_DBPWallet.Trim().PadRight(19, ' ');
                detVoucher.CODIGOPROCESO = codigoproceso;
                detVoucher.FECHACONSUMO = repuestaPago.FechaTrans.ToString();
                detVoucher.HORACONSUMO = repuestaPago.HoraTrans.ToString();
                detVoucher.NUMEROVOUCHER = repuestaPago.SecTrans.ToString().PadLeft(6, '0');
                detVoucher.AUTORIZACION = repuestaPago.NumAutorizacion.PadLeft(6, '0');
                detVoucher.VALORCONSUMO = trama.montoTotalTransaccion.ToString().PadLeft(13, '0');
                detVoucher.FORMAAUTORIZA = "1";
                detVoucher.TIPOCONSUMO = (repuestaPago.CodRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData);
                detVoucher.PLAZO = trama.plazoDiferido.Trim().PadLeft(2, '0');
                detVoucher.TIPOLECTURA = repuestaPago.ModLectura.PadLeft(3, '0');
                detVoucher.TIPOMONEDA = "840";
                detVoucher.VALORIVA = trama.impuestoIvaTransaccion.PadLeft(13, '0');
                detVoucher.VALORSERVICIO = "0000000000000";
                detVoucher.VALORPROPINA = trama.popinaTransaccion.ToString().PadLeft(13, '0');
                detVoucher.VALORINTERES = repuestaPago.ValorInteresFin.ToString().Trim().PadLeft(13, '0');
                detVoucher.VALORFIJO = repuestaPago.MontoFijo.ToString().PadLeft(13, '0');
                detVoucher.TIPOPROMOCION = "00";
                detVoucher.MESESGRACIA = trama.mesGracia.Trim().PadLeft(2, '0');
                detVoucher.EMPRESASERVICIO = "0000";
                detVoucher.ESTADOTRX = (tipoConsumo == "01" ? "O" : "O");
                detVoucher.CODIGORESPUESTA = resptrama.codigoRespuesta;
                detVoucher.TIPODISPOSITIVO = "2";
                detVoucher.ADQUIRENTETARJETA = "CREDIMATIC01";
                detVoucher.ADQUIRENTESERVICIO = "            ";
                detVoucher.MONTOGRAVAIVA = trama.montoBaseGravaIVa.PadLeft(13, '0');
                detVoucher.MONTONOGRAVAIVA = trama.montoBaseNoGravaIVa.PadLeft(13, '0');
                detVoucher.PUNTOEMISION = _factura.Establecimiento + _factura.PtoEmision;
                detVoucher.PROCESADO = "0";
                detVoucher.GRUPOTAR = repuestaPago.GrupoTarjeta;
                detVoucher.AUTORIZADOR = repuestaPago.CodRed;
                detVoucher.LOTE = repuestaPago.NumLote;
                detVoucher.ANULADO = trama.TipoTransaccion == "03" ? "1" : "0";
                detVoucher.PROCESADOTURNO = "0";
                detVoucher.FACTURA = _factura.GetNumeroFactura();
                detVoucher.ARQC = repuestaPago.ARQC;
                detVoucher.AIDEMV = repuestaPago.AID_EMV;
                detVoucher.EMV = repuestaPago.EMV;
                detVoucher.TC = repuestaPago.TipoCriptogramaEMV;
                detVoucher.PUBLICIDAD = repuestaPago.MsjImpPremiosPub;
                detVoucher.TIPOTRANSACCION = trama.TipoTransaccion;
                detVoucher.BANCOADQUIRIENTE = repuestaPago.NombBcoAdq;
                detVoucher.TARJETAHABIENTE = repuestaPago.NombTarjetaHabiente;
                detVoucher.MID = repuestaPago.MerchantID;
                detVoucher.TID = repuestaPago.TerminalID;
                detVoucher.VENCTAR = fvencTarj;
                detVoucher.ANULAUTORIZACION = "";
                detVoucher.TIPOBANCOTARJETA = (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);

                /*recupero el script de insert para registrar en el log del POS */
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", " Recupero string de Insert ");//Jorge Medina 19/01/2025
                scriptInsertarVoucher = detVoucher.GetInsertString();

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraVoucher", scriptInsertarVoucher);//Jorge Medina 19/01/2025

                /*Ejecuta guardar con Innyección SQL - UDT */
                //var objGrabaVoucher = detVoucher.EjecutaGrabarUDT();
                var objGrabaVoucher = detVoucher.EjecutaGrabar();

                if (objGrabaVoucher.CodigoRespuesta != "0")
                {
                    objRespuesta.CodigoRespuesta = objGrabaVoucher.CodigoRespuesta;
                    objRespuesta.MensajeRespuesta = "Error al insertar el voucher";
                    objRespuesta.scriptInsertarVoucher = scriptInsertarVoucher;
                    objRespuesta.fueProcesadoImpresionVocuhers = true;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "GeneraVoucher - No se genero correctamente el voucher al momento de insertar ");
                    return objRespuesta;
                }


                objRespuesta.CodigoRespuesta = objGrabaVoucher.CodigoRespuesta;
                objRespuesta.MensajeRespuesta = "Registro de Voucher, registrado correctamente";
                objRespuesta.scriptInsertarVoucher = scriptInsertarVoucher;
                objRespuesta.fueProcesadoImpresionVocuhers = true;
                objRespuesta.valor = repuestaPago.Valor;
                objRespuesta.nombreRed = nombreRed;
                objRespuesta.tipovoucher = tipovoucher;
                objRespuesta.tipoConsumo = tipoConsumo;
                objRespuesta.pos_voucher = detVoucher.pos_voucher;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "GeneraVoucher - Termina proceso - Registro voucher ");

                return objRespuesta;
            }
            catch (Exception ex)
            {

                objRespuesta.CodigoRespuesta = "-15";
                objRespuesta.MensajeRespuesta = "Error (GeneraVoucher): " + ex.Message;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "GeneraVoucher", "error: " + ex.Message);
                return objRespuesta;
            }


        }


        private void ProcesaPinpadv2()
        {
            ResponseBackground = string.Empty;
            DebeCerrarFormBackground = false;
            On_Off_Controles(false);
            PagoPinpadBackground = null;

            try
            {
                if (!Control.Common.GlobalParameters.EsAmbienteProduccion && Control.Common.GlobalParameters.EstTcpIpPinpad == false)
                {
                    ProcesaPinpadDev();
                    return;
                }

                var pos = new POSEntities();

                if (pos.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                {
                    //Envio envio = new Envio();
                    //string resultadolectura = envio.Envio_requerimientoPinpad("", PUERTOCOM, 65000, "LT", "", 1);
                    ClsEnviaPinPadGeneral envioGen = new ClsEnviaPinPadGeneral();

                    Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                    Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();

                    trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                    trama.plazoDiferido = cmbDiferido.Text;
                    trama.mesGracia = cmbMesesGracia.Text;

                    string bin_descripcion = "";
                    string valpag = Decimal.Round(Decimal.Parse(txtValor.Text) * 1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";

                    if (_factura.AplicaDescuentoPromoBines || !Control.Common.GlobalParameters.ActivaVersionPinPadMedianet)
                    {
                        string resultadolectura = envioGen.SendRequestPinpad("", PUERTOCOM, 65000, "LT", "", 1);
                        //   envioGen.SendRequestPinpad(_factura)
                        //SendRequestPinpad(string IP, int Puerto, int timeout, string trama, string rutabines, int grabalog, Factura _Factura, string keyderecha, string keyizquierda)
                        // resultadolectura = "LT0000475398XXXXXX7010         2111DA86EFF99EEF04E955AB5E81F6DF5C9B07F38CDDLECTURA OK          ";

                        int leerTramaRes = 75;

                        if (resultadolectura.Length > 98)
                        {
                            leerTramaRes = leerTramaRes + 24;
                        }

                        if (resultadolectura.Substring(leerTramaRes, 10) == "LECTURA OK")
                        {
                            var ctb = pos.core_tarjetacredito_bin.Where(x => x.bin == resultadolectura.Substring(6, 6)).FirstOrDefault();
                            if (ctb != null)
                            {
                                var autorizador = trama.TipoTransaccion == "01" ? ctb.bin_red : ctb.bin_red_cred;
                                trama.codRed = autorizador;
                                if (autorizador == "2" && !Control.Common.GlobalParameters.EstTcpIpPinpad)
                                {
                                    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                                    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                                }
                                else if (autorizador == "1" && !Control.Common.GlobalParameters.EstTcpIpPinpad)
                                {
                                    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                                    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                                }
                                else if (Control.Common.GlobalParameters.EstTcpIpPinpad)
                                {
                                    //    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    //    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                                    //    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;

                                    trama.codRed = "2";
                                    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                                    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                                }
                                else
                                {
                                    ResponseBackground = Common.GlobalParameters.PinpadMsjAutorizadorNoValido;
                                    return;
                                }

                                bin_descripcion = ctb.bin_descripcion;
                            }
                            else
                            {
                                ResponseBackground = "Tarjeta no se encuentra en listado de bines";
                                return;
                            }
                        }
                        else
                        {
                            ResponseBackground = "Error en PINPAD";
                            return;
                        }
                    }
                    else
                    {
                        trama.codRed = "2";
                        trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                        trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                        trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                    }


                    //  MessageBox.Show(this,"Verificar el calculo para  armar los totales");
                    var porc_iva = Control.Common.GlobalParameters.IVAGEN / 100;
                    var PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;/// Decimal.Parse((pos.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));

                    var porc_pago = (decimal.Parse(txtValor.Text) / _factura.GetTotal());
                    //var porc_pago = decimal.Round( decimal.Parse(txtValor.Text) / _factura.GetTotal(),2);
                    //var porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                    decimal porc_Desc2 = 0;
                    if (_facturaApp == null)
                    {
                        porc_Desc2 = 0;
                    }
                    else
                        if (_facturaApp.Descuentos2.Count > 0)
                        {
                            porc_Desc2 = _facturaApp.Descuentos2.Max(x => x.Porcentaje);
                        }
                    var base0 = _factura.GetBase0() - (_factura.GetDescuentos() - (_factura.GetBase12() - _factura.GetBase12Desc()));
                    base0 = base0 - (base0 * (porc_Desc2 / 100));
                    var base12 = _factura.GetBase12DescPromoIVA(false);
                    base12 = base12 - (base12 * (porc_Desc2 / 100));
                    var base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true); ;
                    if (_factura.GetPromoIva() > 0)
                    {
                        base12 = base12 - ((base12 * PorcPromo) / 100);
                    }
                    base12 += base12PromoIVAExcluye;
                    var iva = decimal.Round(base12 * porc_iva, 2);
                    trama.montoTotalTransaccion = valpag;//12N 10N2D

                    if (decimal.Parse(txtValor.Text) == _factura.GetTotal())
                    {
                        trama.montoBaseGravaIVa = decimal.Round((base12 * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = decimal.Round(((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0) * porc_pago), 2, MidpointRounding.ToEven).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = decimal.Round((iva * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                    }
                    else
                    {

                        base12 = Decimal.Round(base12 * porc_pago, 2);
                        iva = decimal.Round(base12 * porc_iva, 2);
                        base0 = decimal.Parse(txtValor.Text) - base12 - iva;

                        if (base0 < decimal.Parse("0"))
                        {
                            porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                            base12 = _factura.GetBase12DescPromoIVA(false);
                            base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                            if (_factura.GetPromoIva() > 0)
                            {
                                base12 = base12 - ((base12 * PorcPromo) / 100);
                            }
                            base12 += base12PromoIVAExcluye;
                            iva = decimal.Round(base12 * porc_iva, 2);

                            base12 = base12 * porc_pago;
                            iva = base12 * porc_iva;
                            base0 = decimal.Parse(txtValor.Text) - base12 - iva;
                        }


                        trama.montoBaseGravaIVa = (base12).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = ((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0)).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = (iva).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              

                    }

                    trama.impuestoServicioTransaccion = "";//12N 10N2D
                    trama.popinaTransaccion = "";//12N 10N2D
                    trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                    trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                    trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                    trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
                    trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos
                    trama.CID = _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 


                    if (trama.TipoTransaccion == "03")
                    {
                        //trama.numAutorizacion = txtnumAut.Text;//6N  solo anulaciones envia autorizacion compra original / resto blancos
                        //trama.secuencialTransaccion = txtSecuencial.Text.PadLeft(6, '0');//6N  -- Anulaciones enviar Secuencial / resto en cero
                    }

                    if (string.IsNullOrEmpty(trama.TipoMensaje)) { trama.TipoMensaje = "PP"; }


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Enviando requerimiento PINPAD, trama: " + trama.DevuelveTrama);

                    // envio = new Envio();
                    int timeOutCP = 45000;
                    int.TryParse(pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutCP);
                    //var pinpadResponse = envio.Envio_requerimientoPinpad("", PUERTOCOM, 45000, trama.DevuelveTrama, "", 1);
                    envioGen = new ClsEnviaPinPadGeneral();
                    var pinpadResponse = envioGen.SendRequestPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);

                    // var pinpadResponse = envio.Envio_requerimientoPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);
                    resptrama.ObtieneDato = pinpadResponse;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Respuesta requerimiento PINPAD recibida: " + pinpadResponse);


                    if ((resptrama.mensajeRespuesta.Trim() == "AUTORIZACION OK." || resptrama.mensajeRespuesta.Trim() == "APROBADA  TRANS." || resptrama.mensajeRespuesta.Trim() == "APROBADA." || resptrama.mensajeRespuesta.Trim() == "APROBADA") && resptrama.numAut.Trim() != "")
                    {
                        EsPagoOkPromoTarjeta = true;
                        POS_VOUCHER pos_voucher = new POS_VOUCHER();
                        bool fueProcesadoImpresionVocuhers = false;
                        string printWarnings = string.Empty;
                        string scriptInsertarVoucher = string.Empty;
                        string tipoConsumo = "";
                        DateTime fecha = DateTime.ParseExact("01-" + resptrama.fechaVencTar.Substring(2, 2) + "-" + resptrama.fechaVencTar.Substring(0, 2), "dd-MM-yy", CultureInfo.InvariantCulture);
                        string fvencTarj = Convert.ToString(fecha).Substring(5, 2) + '/' + Convert.ToString(fecha).Substring(0, 4);
                        try
                        {
                            //Agregar el nro del comprobante de la factura para que se pueda incluir en la impresion
                            trama.FacturaComprobante = _factura.GetNumeroFacturaEnmascarado();

                            if (resptrama.codigoRed == "02")
                                tipoConsumo = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo;
                            else
                                tipoConsumo = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;

                            scriptInsertarVoucher += "SCRIPT PARA INSERTAR:" + Environment.NewLine;
                            scriptInsertarVoucher += "INSERT INTO dbo.POS_VOUCHER(TARJETA,CODIGOPROCESO,FECHACONSUMO,HORACONSUMO,NUMEROVOUCHER,AUTORIZACION,VALORCONSUMO,FORMAAUTORIZA,TIPOCONSUMO,PLAZO,TIPOLECTURA,TIPOMONEDA,VALORIVA,VALORSERVICIO,VALORPROPINA,VALORINTERES,VALORFIJO,TIPOPROMOCION,MESESGRACIA,EMPRESASERVICIO,ESTADOTRX,CODIGORESPUESTA,TIPODISPOSITIVO,ADQUIRENTETARJETA,ADQUIRENTESERVICIO,MONTOGRAVAIVA,MONTONOGRAVAIVA,PUNTOEMISION,PROCESADO,GRUPOTAR,AUTORIZADOR,LOTE,ANULADO,PROCESADOTURNO,FACTURA,ARQC,AIDEMV,EMV,TC,PUBLICIDAD,TIPOTRANSACCION,BANCOADQUIRIENTE,TARJETAHABIENTE,MID,TID,VENCTAR,ANULAUTORIZACION,TIPOBANCOTARJETA) " + Environment.NewLine;
                            scriptInsertarVoucher += "VALUES('" +
                                resptrama.numTarTuncate.Trim().PadRight(19, ' ') + "'," +
                                "'000200'," +
                                "'" + resptrama.fechaTrans + "'," +
                                "'" + resptrama.horaTrans + "'," +
                                "'" + resptrama.secuencialtransaccion + "'," +
                                "'" + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) + "'," +
                                "'" + trama.montoTotalTransaccion.PadLeft(13, '0') + "'," +
                                "1," +
                                "'" + (resptrama.codigoRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData) + "'," +
                                "'" + trama.plazoDiferido.PadLeft(2, '0') + "'," +
                                "'" + resptrama.modoLectura.PadLeft(3, '0') + "'," +
                                "'840'," +
                                "'" + trama.impuestoIvaTransaccion.PadLeft(13, '0') + "'," +
                                "'0000000000000'," +
                                "'" + trama.popinaTransaccion.PadLeft(13, '0') + "'," +
                                "'" + resptrama.valInteres.Trim().PadLeft(13, '0') + "'," +
                                "'" + resptrama.montoFijo.Trim().PadLeft(13, '0') + "'," +
                                "'00'," +
                                "'" + trama.mesGracia.PadLeft(2, '0') + "'," +
                                "'0000'," +
                                 //"'" + (tipoConsumo == "01" ? "O" : "R") + "'," +
                                 "'" + (tipoConsumo == "01" ? "O" : "O") + "'," +
                                "'" + resptrama.codigoRespuesta + "'," +
                                "'2'," +
                                "'CREDIMATIC01'," +
                                "'            '," +
                                "'" + trama.montoBaseGravaIVa.PadLeft(13, '0') + "'," +
                                "'" + trama.montoBaseNoGravaIVa.PadLeft(13, '0') + "'," +
                                "'" + _factura.Establecimiento + _factura.PtoEmision + "'," +
                                "0," +
                                "'" + resptrama.nomGruTar + "'," +
                                resptrama.codigoRed + "," +
                                "'" + resptrama.numerolote + "'," +
                                (trama.TipoTransaccion == "03" ? "1" : "0") + "," +
                                "0," +
                                "'" + _factura.GetNumeroFactura() + "'," +
                                "'" + resptrama.ARQC + "'," +
                                "'" + resptrama.AIDEMV + "'," +
                                "'" + resptrama.idEMV + "'," +
                                "'" + resptrama.tipoCritoyValorEMV + "'," +
                                "'" + resptrama.mensajePremioPublicidad + "'," +
                                "'" + trama.TipoTransaccion + "'," +
                                "'" + resptrama.nomBancoAdq + "'," +
                                "'" + resptrama.nombreTarjetaHabiente + "'," +
                                "'" + resptrama.merchantId + "'," + //trama.MID + "'," +
                                "'" + trama.TID + "'," +
                                "'" + (resptrama.codigoRed == "02" ? fvencTarj : "XX/XXXX") + "'," +
                                //"'" + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") + "'," +
                                "'" + trama.numAutorizacion + "'," +
                                "'" + (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text) + "'" +
                                ")";

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", scriptInsertarVoucher);

                            Decimal valor = Decimal.Parse(txtValor.Text);
                            //evelasco .ini
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Previo a obtener trama pinpad (BinDescripcion).'" + _factura.GetNumeroFactura() + "' : '" + bin_descripcion + "'");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Cambio realizado por evelasco para identificar version.'");
                            string autori = string.Empty;
                            string TramaCobro = string.Empty;

                            try
                            {
                                /*  if (string.IsNullOrEmpty(resptrama.fechaTrans))
                                      {
                                      Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "La Fecha Transaccion está vacia.'" + _factura.GetNumeroFactura() + "':  "+ resptrama.fechaTrans+" " + Environment.NewLine);
                                      }
                                  DateTime fechaTrans = Convert.ToDateTime(resptrama.fechaTrans);*/
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Obtiene Fecha Transaccion pinpad.'" + _factura.GetNumeroFactura() + "' : " + resptrama.fechaTrans);
                                autori = trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut; //6 ;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Obtiene Numero Autorización pinpad.'" + _factura.GetNumeroFactura() + "' : " + autori + " ");
                                string valPago = string.Empty;

                                decimal valorTC = 0M;
                                string autorizador;
                                try
                                {
                                    decimal.TryParse(txtValor.Text.Trim(), out valorTC);
                                }
                                catch (Exception)
                                {
                                    if (txtValor.Text.Trim().Substring(0, 1) == ".")
                                    {
                                        txtValor.Text = "0" + txtValor.Text;
                                        decimal.TryParse(txtValor.Text.Trim(), out valorTC);
                                    }
                                }

                                //TramaCobro = "|" + resptrama.secuencialtransaccion + ";" + autori + ";" + resptrama.fechaTrans + ";" + trama.montoTotalTransaccion.PadLeft(13, '0');//evelasco se envia trama para utilizar el proceso AX para cruzar con los cobros por TC - MEDIANET, DATAFAST.
                                TramaCobro = "|" + resptrama.secuencialtransaccion + ";" + autori + ";" + resptrama.fechaTrans + ";" + valorTC.ToString();//evelasco se envia trama para utilizar el proceso AX para cruzar con los cobros por TC - MEDIANET, DATAFAST.
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Obtiene Trama del pinpad (Num Voucher+Autorización+fecha Transaccion+ Valor Transacción) pinpad '" + _factura.GetNumeroFactura() + "' :" + TramaCobro);
                                bin_descripcion = bin_descripcion + " " + TramaCobro;
                            }
                            catch (Exception exTramaCobro)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Error al Obtener la Trama del PinPad para identificar en el diario de Cobro (PaymentNotes)'" + _factura.GetNumeroFactura() + "'" + Environment.NewLine + exTramaCobro.InnerException.Message);

                            }
                            //evelasco .fin    
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Después de armar trama para bin_descripcion.'" + _factura.GetNumeroFactura() + "' : '" + bin_descripcion + "'");


                            // Nuevo Autorizador "AUSTRO".  JM  04-12-2020
                            string nombreRed = "";
                            if (resptrama.codigoRed == "01")
                                nombreRed = "DATAFAST";
                            else if (resptrama.codigoRed == "02")
                                nombreRed = "MEDIANET";
                            else if (resptrama.codigoRed == "03")
                                nombreRed = "AUSTRO";

                            //Agregar el pago a un repositorio temporal
                            PagoPinpadBackground = new Models.PagoPinpad
                            {
                                Valor = valor,
                                Banco = nombreRed,
                                Nombre = resptrama.nomGruTar,
                                Marca = resptrama.codBancoAdq,
                                TipoPos = nombreRed,
                                BinDescripcion = bin_descripcion
                            };

                            // Nuevo Autorizador "AUSTRO".  JM  04-12-2020

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Inserta el valor de la trama en el modelo PagoPinpadBackground.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-23
                            //Impresion de voucher
                            String tipovoucher = "";
                            if (Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma > 0 && valor <= Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma)
                            {
                                tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCreditoSinFirma;
                            }
                            else
                            {
                                tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                            }

                            //Recibo original
                            string printResponse = string.Empty;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Imprime voucher Original.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                            prepararVoucherTarjeta(tipovoucher, resptrama, trama, ref printResponse);
                            printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 1: " + Environment.NewLine + printResponse + Environment.NewLine));
                            //Recibo copia
                            printResponse = string.Empty;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Imprime voucher Copia.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                            prepararVoucherTarjeta(tipovoucher, resptrama, trama, ref printResponse, true);
                            printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 2: " + Environment.NewLine + printResponse + Environment.NewLine));

                            if (!string.IsNullOrWhiteSpace(printWarnings))
                            {
                                ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco pero algunos recibos no pudieron ser impresos en este momento. Por favor realice la reimpresion usando el comando F4 y seleccionando la opcion Reimpresion de Vouchers";
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Ocurrieron incidencias durante la impresion de los recibos voucher en la factura '" + _factura.GetNumeroFactura() + "'" + Environment.NewLine + printWarnings);
                            }

                            fueProcesadoImpresionVocuhers = true;

                            //grabar transaccion en tabla pos_voucher
                            //cambiar aqui

                            pos_voucher.TARJETA = resptrama.numTarTuncate.Trim().PadRight(19, ' ');// ("520081XXXXXX6017   "); //19 ;

                            //revisar
                            string codigoproceso = "000200";
                            //es 003000 cuando es transacciones con tarjeta de crédito, 001000 cuando es transacción de tarjeta de debito cuenta de ahorro y 002000 cuando es transacción de tarjeta de debito cuenta corriente.

                            pos_voucher.CODIGOPROCESO = codigoproceso;// ("000200"); //6 ;
                                                                      //revisar

                            pos_voucher.FECHACONSUMO = resptrama.fechaTrans;// ("20161122"); //8 ;
                            pos_voucher.HORACONSUMO = resptrama.horaTrans;// ("114339"); //6 ;
                            pos_voucher.NUMEROVOUCHER = resptrama.secuencialtransaccion;// ("000002");//6 ;

                            pos_voucher.AUTORIZACION = trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut; //6 ;
                            pos_voucher.ANULADO = trama.TipoTransaccion == "03" ? true : false;

                            pos_voucher.VALORCONSUMO = trama.montoTotalTransaccion.PadLeft(13, '0');// ("0000000001200"); //13 ;
                            pos_voucher.FORMAAUTORIZA = ("1"); //1 ;
                            if (resptrama.codigoRed == "02")
                                pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo; //2 ;
                            else
                                pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData; //2 ;
                            pos_voucher.PLAZO = trama.plazoDiferido.PadLeft(2, '0');// ("06"); //2 ;

                            pos_voucher.TIPOLECTURA = resptrama.modoLectura.PadLeft(3, '0');// ("005"); //3 ;
                            pos_voucher.TIPOMONEDA = ("840"); //3 ;
                            pos_voucher.VALORIVA = trama.impuestoIvaTransaccion.PadLeft(13, '0');// ("0000000000147"); //13 ;
                            pos_voucher.VALORSERVICIO = ("0000000000000"); //13 ;
                            pos_voucher.VALORPROPINA = trama.popinaTransaccion.PadLeft(13, '0');// ("0000000000000"); //13 ;
                            pos_voucher.VALORINTERES = resptrama.valInteres.Trim().PadLeft(13, '0');// ("0000000000057");  //13 ;
                            pos_voucher.VALORFIJO = resptrama.montoFijo.Trim().PadLeft(13, '0');// ("0000000000000");  //13 ;
                            pos_voucher.TIPOPROMOCION = ("00"); //2 ;
                            pos_voucher.MESESGRACIA = trama.mesGracia.PadLeft(2, '0');//                        ("00");  //2 ;
                            pos_voucher.EMPRESASERVICIO = ("0000");  //4 ;
                            //pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "R"); //1 ;
                            pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "O"); //JCañarte
                            pos_voucher.CODIGORESPUESTA = resptrama.codigoRespuesta;// ("00"); //2 ;
                            pos_voucher.TIPODISPOSITIVO = ("2"); //1 ;
                            pos_voucher.ADQUIRENTETARJETA = ("CREDIMATIC01"); //12 ;
                            pos_voucher.ADQUIRENTESERVICIO = ("            ");  //12 ;
                            pos_voucher.MONTOGRAVAIVA = trama.montoBaseGravaIVa.PadLeft(13, '0');// ("0000000001053"); //13 ;
                            pos_voucher.MONTONOGRAVAIVA = trama.montoBaseNoGravaIVa.PadLeft(13, '0');// ("0000000000000");  //13 ;
                            pos_voucher.PUNTOEMISION = _factura.Establecimiento + _factura.PtoEmision;
                            pos_voucher.PROCESADO = false;
                            pos_voucher.GRUPOTAR = resptrama.nomGruTar;
                            pos_voucher.AUTORIZADOR = int.Parse(resptrama.codigoRed);
                            pos_voucher.LOTE = resptrama.numerolote;
                            pos_voucher.FACTURA = _factura.GetNumeroFactura();
                            //ML [16/01/2018]: Nuevos campos
                            pos_voucher.ARQC = resptrama.ARQC;
                            pos_voucher.AIDEMV = resptrama.AIDEMV;
                            pos_voucher.EMV = resptrama.idEMV;
                            pos_voucher.TC = resptrama.tipoCritoyValorEMV;
                            pos_voucher.PUBLICIDAD = resptrama.mensajePremioPublicidad;
                            pos_voucher.TIPOTRANSACCION = trama.TipoTransaccion;
                            pos_voucher.BANCOADQUIRIENTE = resptrama.nomBancoAdq;
                            pos_voucher.TARJETAHABIENTE = resptrama.nombreTarjetaHabiente;
                            pos_voucher.MID = resptrama.merchantId; //trama.MID;
                            pos_voucher.TID = trama.TID;
                            pos_voucher.VENCTAR = resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX";
                            pos_voucher.ANULAUTORIZACION = trama.numAutorizacion;
                            pos_voucher.TIPOBANCOTARJETA = (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);
                            //------------------------------
                            bool ExisteVoucher = pos.POS_VOUCHER.Any(x => x.FECHACONSUMO == pos_voucher.FECHACONSUMO && x.VALORCONSUMO == pos_voucher.VALORCONSUMO && x.AUTORIZACION == pos_voucher.AUTORIZACION && x.NUMEROVOUCHER == pos_voucher.NUMEROVOUCHER && x.FACTURA == pos_voucher.FACTURA);
                            if (!ExisteVoucher)
                            {
                                pos.POS_VOUCHER.Add(pos_voucher);
                            }
                            // pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet += 1;
                            pos.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            if (!fueProcesadoImpresionVocuhers)
                            {
                                printWarnings += "Acaba de ocurrir una excepcion durante la grabacion del voucher al sistema sin llegar siquiera al bloque de impresion de voucher";
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", printWarnings);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Excepcion: " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "StackTrace: " + ex.StackTrace);
                            }
                            string msjSeImprimieronVouchers = string.IsNullOrWhiteSpace(printWarnings) ? " y se imprimieron los recibos," : ", a pesar de que no se pudo imprimir los recibos en el momento,";
                            string msjParaDevteam = "En el POS del siguiente punto de emision, si bien se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers + " el voucher no pudo ser grabado en nuestra base interna. Generar el registro en POS_VOUCHER inmediatamente pues puede provocar descuadres en los cierres. Esto pudo deberse a un breve inconveniente, se recomienda una vez generado el registro, verificar la causa de la novedad";

                            string datosVoucher = "\nTARJETA: " + resptrama.numTarTuncate.Trim().PadRight(19, ' ') +
                                                    "\nCODIGOPROCESO: 000200" +
                                                    "\nFECHACONSUMO: " + resptrama.fechaTrans +
                                                    "\nHORACONSUMO: " + resptrama.horaTrans +
                                                    "\nNUMEROVOUCHER: " + resptrama.secuencialtransaccion +
                                                    "\nAUTORIZACION: " + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) +
                                                    "\nANULADO: " + (trama.TipoTransaccion == "03" ? "1" : "0") +
                                                    "\nVALORCONSUMO: " + trama.montoTotalTransaccion.PadLeft(13, '0') +
                                                    "\nFORMAAUTORIZA: 1" +
                                                    "\nTIPOCONSUMO: " + (resptrama.codigoRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData) +
                                                    "\nPLAZO: " + trama.plazoDiferido.PadLeft(2, '0') +
                                                    "\nTIPOLECTURA: " + resptrama.modoLectura.PadLeft(3, '0') +
                                                    "\nTIPOMONEDA: 840" +
                                                    "\nVALORIVA: " + trama.impuestoIvaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORSERVICIO: 0000000000000" +
                                                    "\nVALORPROPINA: " + trama.popinaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORINTERES: " + resptrama.valInteres.Trim().PadLeft(13, '0') +
                                                    "\nVALORFIJO: " + resptrama.montoFijo.Trim().PadLeft(13, '0') +
                                                    "\nTIPOPROMOCION: 00" +
                                                    "\nMESESGRACIA: " + trama.mesGracia.PadLeft(2, '0') +
                                                    "\nEMPRESASERVICIO: 0000" +
                                                    //"\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "R") +
                                                    "\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "O") +  //JCanarte
                                                    "\nCODIGORESPUESTA: " + resptrama.codigoRespuesta +
                                                    "\nTIPODISPOSITIVO: 2" +
                                                    "\nADQUIRENTETARJETA: CREDIMATIC01" +
                                                    "\nADQUIRENTESERVICIO:             " +
                                                    "\nMONTOGRAVAIVA: " + trama.montoBaseGravaIVa.PadLeft(13, '0') +
                                                    "\nMONTONOGRAVAIVA: " + trama.montoBaseNoGravaIVa.PadLeft(13, '0') +
                                                    "\nPUNTOEMISION: " + _factura.Establecimiento + _factura.PtoEmision +
                                                    "\nPROCESADO: 0" +
                                                    "\nGRUPOTAR: " + resptrama.nomGruTar +
                                                    "\nAUTORIZADOR: " + resptrama.codigoRed +
                                                    "\nLOTE: " + resptrama.numerolote +
                                                    "\nFACTURA: " + _factura.GetNumeroFactura() +
                                                    "\nARQC: " + resptrama.ARQC +
                                                    "\nAIDEMV: " + resptrama.AIDEMV +
                                                    "\nEMV: " + resptrama.idEMV +
                                                    "\nTC: " + resptrama.tipoCritoyValorEMV +
                                                    "\nPUBLICIDAD: " + resptrama.mensajePremioPublicidad +
                                                    "\nTIPOTRANSACCION: " + trama.TipoTransaccion +
                                                    "\nBANCOADQUIRIENTE: " + resptrama.nomBancoAdq +
                                                    "\nTARJETAHABIENTE: " + resptrama.nombreTarjetaHabiente +
                                                    "\nMID: " + resptrama.merchantId + //trama.MID +
                                                    "\nTID: " + trama.TID +
                                                    "\nVENCTAR: " + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") +
                                                    "\nANULAUTORIZACION: " + trama.numAutorizacion +
                                                    "\nTIPOBANCOTARJETA: " + (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);

                            datosVoucher += Environment.NewLine + Environment.NewLine + scriptInsertarVoucher;

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero el voucher no pudo ser grabado en nuestra base interna, " + Environment.NewLine + "a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex) + Environment.NewLine + "StackTrace:" + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Datos Voucher: " + datosVoucher.Replace("\n", Environment.NewLine));

                            try
                            {
                                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                    Properties.Settings.Default.MAILERROR_FROM,
                                    Properties.Settings.Default.MAILERROR_ALIAS,
                                    Properties.Settings.Default.MAILERROR_DESTINO,
                                    Properties.Settings.Default.MAILERROR_CC,
                                    "Voucher pinpad no se grabó en nuestra base interna",
                                    String.Format(msjParaDevteam +
                                                    "  \n\nDatos caja-----------------" +
                                                    "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                                    "  \n\nDatos voucher-----------------" +
                                                    datosVoucher,
                                                Control.Common.GlobalParameters.Establecimiento,
                                                Control.Common.GlobalParameters.PuntoEmision,
                                                Control.Common.GlobalParameters.IpMaquina,
                                                Control.Common.GlobalParameters.UsuarioNombre,
                                                Control.Common.GlobalParameters.Usuario,
                                                Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                                (fueProcesadoImpresionVocuhers ? "" : ("StackTrace: " + ex.StackTrace + " \n"))),
                                    false,
                                    String.Empty);

                                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "No se pudo enviar notificacion del problema al grabar voucher en nuestra base interna, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                                }

                                //eevv 2020-01-13
                                try
                                {
                                    string Stringremove = string.Empty;
                                    Stringremove = "SCRIPT PARA INSERTAR: " + Environment.NewLine;
                                    creaArhivoTemporalVoucher(scriptInsertarVoucher.Replace(Stringremove, string.Empty));
                                }
                                catch (Exception ex3)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "No se pudo generar archivo TXT de insert para la POS_VOUCHER , a continuacion las excepciones encontradas - " + ex3.Message);

                                }
                            }
                            catch { }

                            ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero detectamos que el voucher no pudo ser grabado en nuestra base interna. Por favor comuníquelo inmediatamente al administrador para que realice su gestión y evitar descuadre durante su cierre";
                        }

                        //Agregar lineas de insert
                        Control.Common.Logger.Agregar_Trace_Voucher(pos_voucher);

                        DebeCerrarFormBackground = true;
                    }
                    else
                    {
                        ResponseBackground = "Error : " + resptrama.mensajeRespuesta;

                        //Enviar trama de reverso de transaccion tipo 04.   JM  25-08-2020
                        envioGen = new ClsEnviaPinPadGeneral();
                        trama.TipoTransaccion = "04";
                        pinpadResponse = envioGen.SendRequestPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);
                        // var pinpadResponse = envio.Envio_requerimientoPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);                       
                        resptrama.ObtieneDato = pinpadResponse;
                        ResponseBackground = ResponseBackground + " \n" + "Reverso : " + resptrama.mensajeRespuesta;
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Reverso de transacción no exitosa, envio: " + trama.DevuelveTrama);
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Reverso de transacción no exitosa, respuesta : " + resptrama.mensajeRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                ResponseBackground = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a una breve interrupcion en el servicio, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco";
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "La solicitud no pudo ser realizada debido a una breve interrupcion en el servicio, esto puede deberse a un breve mantenimiento, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "ProcesaPinpad", "error: " + ex.Message);
            }
            finally
            {
                On_Off_Controles(true);
            }
        }

        private void ProcesaPinpad()
        {
            ResponseBackground = string.Empty;
            DebeCerrarFormBackground = false;
            On_Off_Controles(false);
            PagoPinpadBackground = null;

            string numBinTC = string.Empty;


            try
            {
                if (!Control.Common.GlobalParameters.EsAmbienteProduccion && Control.Common.GlobalParameters.EstTcpIpPinpad == false)
                {
                    ProcesaPinpadDev();
                    return;
                }

                var pos = new POSEntities();

                if (pos.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                {
                    //Envio envio = new Envio();
                    //string resultadolectura = envio.Envio_requerimientoPinpad("", PUERTOCOM, 65000, "LT", "", 1);
                    ClsEnviaPinPadGeneral envioGen = new ClsEnviaPinPadGeneral();

                    Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                    Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();

                    trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                    trama.plazoDiferido = cmbDiferido.Text;
                    trama.mesGracia = cmbMesesGracia.Text;

                    string bin_descripcion = "";
                    string valpag = Decimal.Round(Decimal.Parse(txtValor.Text) * 1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";

                    if (_factura.AplicaDescuentoPromoBines || !Control.Common.GlobalParameters.ActivaVersionPinPadMedianet)
                    {
                        string resultadolectura = envioGen.SendRequestPinpad("", PUERTOCOM, 65000, "LT", "", 1);
                        //   envioGen.SendRequestPinpad(_factura)
                        // SendRequestPinpad(string IP, int Puerto, int timeout, string trama, string rutabines, int grabalog, Factura _Factura, string keyderecha, string keyizquierda)
                        // resultadolectura = "LT0000475398XXXXXX7010         2111DA86EFF99EEF04E955AB5E81F6DF5C9B07F38CDDLECTURA OK          ";

                        int leerTramaRes = 75;

                        if (resultadolectura.Length > 98)
                        {
                            leerTramaRes = leerTramaRes + 24;
                        }


                        //filler 1spc

                        if (resultadolectura.Substring(leerTramaRes, 10) == "LECTURA OK")
                        {

                            numBinTC = resultadolectura.Substring(6, 6);
                            var ctb = pos.core_tarjetacredito_bin.Where(x => x.bin == numBinTC).FirstOrDefault();


                            if (ctb != null)
                            {
                                var autorizador = trama.TipoTransaccion == "01" ? ctb.bin_red : ctb.bin_red_cred;
                                trama.codRed = autorizador;
                                if (autorizador == "2" && !Control.Common.GlobalParameters.EstTcpIpPinpad)
                                {
                                    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                                    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                                }
                                else if (autorizador == "1" && !Control.Common.GlobalParameters.EstTcpIpPinpad)
                                {
                                    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                                    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                                }
                                else if (Control.Common.GlobalParameters.EstTcpIpPinpad)
                                {
                                    //    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    //    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                                    //    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;

                                    trama.codRed = "2";
                                    trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                    trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                                    trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                                }
                                else
                                {
                                    ResponseBackground = Common.GlobalParameters.PinpadMsjAutorizadorNoValido;
                                    return;
                                }

                                bin_descripcion = ctb.bin_descripcion;
                            }
                            else
                            {
                                ResponseBackground = "Tarjeta no se encuentra en listado de bines";
                                return;
                            }
                        }
                        else
                        {
                            ResponseBackground = "Error en PINPAD";
                            return;
                        }
                    }
                    else
                    {
                        trama.codRed = "2";
                        trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                        trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                        trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                    }


                    //  MessageBox.Show(this,"Verificar el calculo para  armar los totales");
                    var porc_iva = Control.Common.GlobalParameters.IVAGEN / 100;
                    var PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;/// Decimal.Parse((pos.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));

                    var porc_pago = (decimal.Parse(txtValor.Text) / _factura.GetTotal());
                    //var porc_pago = decimal.Round( decimal.Parse(txtValor.Text) / _factura.GetTotal(),2);
                    //var porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                    decimal porc_Desc2 = 0;
                    if (_facturaApp == null)
                    {
                        porc_Desc2 = 0;
                    }
                    else
                        if (_facturaApp.Descuentos2.Count > 0)
                        {
                            porc_Desc2 = _facturaApp.Descuentos2.Max(x => x.Porcentaje);
                        }
                    var base0 = _factura.GetBase0() - (_factura.GetDescuentos() - (_factura.GetBase12() - _factura.GetBase12Desc()));
                    base0 = base0 - (base0 * (porc_Desc2 / 100));
                    var base12 = _factura.GetBase12DescPromoIVA(false);
                    base12 = base12 - (base12 * (porc_Desc2 / 100));
                    var base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true); ;
                    if (_factura.GetPromoIva() > 0)
                    {
                        base12 = base12 - ((base12 * PorcPromo) / 100);
                    }
                    base12 += base12PromoIVAExcluye;
                    var iva = decimal.Round(base12 * porc_iva, 2);
                    trama.montoTotalTransaccion = valpag;//12N 10N2D

                    if (decimal.Parse(txtValor.Text) == _factura.GetTotal())
                    {
                        trama.montoBaseGravaIVa = decimal.Round((base12 * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = decimal.Round(((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0) * porc_pago), 2, MidpointRounding.ToEven).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = decimal.Round((iva * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                    }
                    else
                    {

                        base12 = Decimal.Round(base12 * porc_pago, 2);
                        iva = decimal.Round(base12 * porc_iva, 2);
                        base0 = decimal.Parse(txtValor.Text) - base12 - iva;

                        if (base0 < decimal.Parse("0"))
                        {
                            porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                            base12 = _factura.GetBase12DescPromoIVA(false);
                            base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                            if (_factura.GetPromoIva() > 0)
                            {
                                base12 = base12 - ((base12 * PorcPromo) / 100);
                            }
                            base12 += base12PromoIVAExcluye;
                            iva = decimal.Round(base12 * porc_iva, 2);

                            base12 = base12 * porc_pago;
                            iva = base12 * porc_iva;
                            base0 = decimal.Parse(txtValor.Text) - base12 - iva;
                        }


                        trama.montoBaseGravaIVa = (base12).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = ((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0)).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = (iva).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              

                    }

                    trama.impuestoServicioTransaccion = "";//12N 10N2D
                    trama.popinaTransaccion = "";//12N 10N2D
                    trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                    trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                    trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                    trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
                    trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos
                    trama.CID = _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 


                    if (trama.TipoTransaccion == "03")
                    {
                        //trama.numAutorizacion = txtnumAut.Text;//6N  solo anulaciones envia autorizacion compra original / resto blancos
                        //trama.secuencialTransaccion = txtSecuencial.Text.PadLeft(6, '0');//6N  -- Anulaciones enviar Secuencial / resto en cero
                    }

                    trama.TipoMensaje = "PP";
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Enviando requerimiento PINPAD, trama: " + trama.DevuelveTrama);

                    // envio = new Envio();
                    int timeOutCP = 45000;
                    int.TryParse(pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutCP);

                    envioGen = new ClsEnviaPinPadGeneral();
                    //var pinpadResponse = "PP000200AUTORIZACION OK.    00000100000507523820250424143224LI024007000000852320                                                                                                                                VISA ELECT/DEB           07PAYWAVE/VISA                                        VISA DEBITO         A0000000031010      80                                   83E6BC7A85223B2200000000000000475395XXXXXX0060         2906A78F4F3630685C9107754A19BF0BCB6D4D0AD96AF725A9FDA9FF5D3F87AAC64F                           688759A332ADB9797656016F6F82B0A8 ";
                    var pinpadResponse = envioGen.SendRequestPinpadAnt("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);

                    // var pinpadResponse = envio.Envio_requerimientoPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);
                    resptrama.ObtieneDato = pinpadResponse;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Respuesta requerimiento PINPAD recibida: " + pinpadResponse);


                    if ((resptrama.mensajeRespuesta.Trim() == "AUTORIZACION OK." || resptrama.mensajeRespuesta.Trim() == "APROBADA  TRANS." || resptrama.mensajeRespuesta.Trim() == "APROBADA." || resptrama.mensajeRespuesta.Trim() == "APROBADA") && resptrama.numAut.Trim() != "")
                    {
                        EsPagoOkPromoTarjeta = true;
                        POS_VOUCHER pos_voucher = new POS_VOUCHER();
                        bool fueProcesadoImpresionVocuhers = false;
                        string printWarnings = string.Empty;
                        string scriptInsertarVoucher = string.Empty;
                        string tipoConsumo = "";
                        DateTime fecha = DateTime.ParseExact("01-" + resptrama.fechaVencTar.Substring(2, 2) + "-" + resptrama.fechaVencTar.Substring(0, 2), "dd-MM-yy", CultureInfo.InvariantCulture);
                        string fvencTarj = Convert.ToString(fecha).Substring(5, 2) + '/' + Convert.ToString(fecha).Substring(0, 4);
                        try
                        {
                            //Agregar el nro del comprobante de la factura para que se pueda incluir en la impresion
                            trama.FacturaComprobante = _factura.GetNumeroFacturaEnmascarado();

                            if (resptrama.codigoRed == "02")
                                tipoConsumo = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo;
                            else
                                tipoConsumo = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;

                            scriptInsertarVoucher += "SCRIPT PARA INSERTAR:" + Environment.NewLine;
                            scriptInsertarVoucher += "INSERT INTO dbo.POS_VOUCHER(TARJETA,CODIGOPROCESO,FECHACONSUMO,HORACONSUMO,NUMEROVOUCHER,AUTORIZACION,VALORCONSUMO,FORMAAUTORIZA,TIPOCONSUMO,PLAZO,TIPOLECTURA,TIPOMONEDA,VALORIVA,VALORSERVICIO,VALORPROPINA,VALORINTERES,VALORFIJO,TIPOPROMOCION,MESESGRACIA,EMPRESASERVICIO,ESTADOTRX,CODIGORESPUESTA,TIPODISPOSITIVO,ADQUIRENTETARJETA,ADQUIRENTESERVICIO,MONTOGRAVAIVA,MONTONOGRAVAIVA,PUNTOEMISION,PROCESADO,GRUPOTAR,AUTORIZADOR,LOTE,ANULADO,PROCESADOTURNO,FACTURA,ARQC,AIDEMV,EMV,TC,PUBLICIDAD,TIPOTRANSACCION,BANCOADQUIRIENTE,TARJETAHABIENTE,MID,TID,VENCTAR,ANULAUTORIZACION,TIPOBANCOTARJETA) " + Environment.NewLine;
                            scriptInsertarVoucher += "VALUES('" +
                                resptrama.numTarTuncate.Trim().PadRight(19, ' ') + "'," +
                                "'000200'," +
                                "'" + resptrama.fechaTrans + "'," +
                                "'" + resptrama.horaTrans + "'," +
                                "'" + resptrama.secuencialtransaccion + "'," +
                                "'" + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) + "'," +
                                "'" + trama.montoTotalTransaccion.PadLeft(13, '0') + "'," +
                                "1," +
                                "'" + (resptrama.codigoRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData) + "'," +
                                "'" + trama.plazoDiferido.PadLeft(2, '0') + "'," +
                                "'" + resptrama.modoLectura.PadLeft(3, '0') + "'," +
                                "'840'," +
                                "'" + trama.impuestoIvaTransaccion.PadLeft(13, '0') + "'," +
                                "'0000000000000'," +
                                "'" + trama.popinaTransaccion.PadLeft(13, '0') + "'," +
                                "'" + resptrama.valInteres.Trim().PadLeft(13, '0') + "'," +
                                "'" + resptrama.montoFijo.Trim().PadLeft(13, '0') + "'," +
                                "'00'," +
                                "'" + trama.mesGracia.PadLeft(2, '0') + "'," +
                                "'0000'," +
                                 //"'" + (tipoConsumo == "01" ? "O" : "R") + "'," +
                                 "'" + (tipoConsumo == "01" ? "O" : "O") + "'," +
                                "'" + resptrama.codigoRespuesta + "'," +
                                "'2'," +
                                "'CREDIMATIC01'," +
                                "'            '," +
                                "'" + trama.montoBaseGravaIVa.PadLeft(13, '0') + "'," +
                                "'" + trama.montoBaseNoGravaIVa.PadLeft(13, '0') + "'," +
                                "'" + _factura.Establecimiento + _factura.PtoEmision + "'," +
                                "0," +
                                "'" + resptrama.nomGruTar + "'," +
                                resptrama.codigoRed + "," +
                                "'" + resptrama.numerolote + "'," +
                                (trama.TipoTransaccion == "03" ? "1" : "0") + "," +
                                "0," +
                                "'" + _factura.GetNumeroFactura() + "'," +
                                "'" + resptrama.ARQC + "'," +
                                "'" + resptrama.AIDEMV + "'," +
                                "'" + resptrama.idEMV + "'," +
                                "'" + resptrama.tipoCritoyValorEMV + "'," +
                                "'" + resptrama.mensajePremioPublicidad + "'," +
                                "'" + trama.TipoTransaccion + "'," +
                                "'" + resptrama.nomBancoAdq + "'," +
                                "'" + resptrama.nombreTarjetaHabiente + "'," +
                                "'" + resptrama.merchantId + "'," + //trama.MID + "'," +
                                "'" + trama.TID + "'," +
                                "'" + (resptrama.codigoRed == "02" ? fvencTarj : "XX/XXXX") + "'," +
                                //"'" + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") + "'," +
                                "'" + trama.numAutorizacion + "'," +
                                "'" + (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text) + "'" +
                                ")";

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", scriptInsertarVoucher);

                            Decimal valor = Decimal.Parse(txtValor.Text);
                            //evelasco .ini
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Previo a obtener trama pinpad (BinDescripcion).'" + _factura.GetNumeroFactura() + "' : '" + bin_descripcion + "'");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Cambio realizado por evelasco para identificar version.'");
                            string autori = string.Empty;
                            string TramaCobro = string.Empty;

                            try
                            {
                                /*  if (string.IsNullOrEmpty(resptrama.fechaTrans))
                                      {
                                      Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "La Fecha Transaccion está vacia.'" + _factura.GetNumeroFactura() + "':  "+ resptrama.fechaTrans+" " + Environment.NewLine);
                                      }
                                  DateTime fechaTrans = Convert.ToDateTime(resptrama.fechaTrans);*/
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Obtiene Fecha Transaccion pinpad.'" + _factura.GetNumeroFactura() + "' : " + resptrama.fechaTrans);
                                autori = trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut; //6 ;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Obtiene Numero Autorización pinpad.'" + _factura.GetNumeroFactura() + "' : " + autori + " ");
                                string valPago = string.Empty;

                                decimal valorTC = 0M;
                                string autorizador;
                                try
                                {
                                    decimal.TryParse(txtValor.Text.Trim(), out valorTC);
                                }
                                catch (Exception)
                                {
                                    if (txtValor.Text.Trim().Substring(0, 1) == ".")
                                    {
                                        txtValor.Text = "0" + txtValor.Text;
                                        decimal.TryParse(txtValor.Text.Trim(), out valorTC);
                                    }
                                }

                                //TramaCobro = "|" + resptrama.secuencialtransaccion + ";" + autori + ";" + resptrama.fechaTrans + ";" + trama.montoTotalTransaccion.PadLeft(13, '0');//evelasco se envia trama para utilizar el proceso AX para cruzar con los cobros por TC - MEDIANET, DATAFAST.
                                TramaCobro = "|" + resptrama.secuencialtransaccion + ";" + autori + ";" + resptrama.fechaTrans + ";" + valorTC.ToString();//evelasco se envia trama para utilizar el proceso AX para cruzar con los cobros por TC - MEDIANET, DATAFAST.
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Obtiene Trama del pinpad (Num Voucher+Autorización+fecha Transaccion+ Valor Transacción) pinpad '" + _factura.GetNumeroFactura() + "' :" + TramaCobro);
                                bin_descripcion = bin_descripcion + " " + TramaCobro;
                            }
                            catch (Exception exTramaCobro)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Error al Obtener la Trama del PinPad para identificar en el diario de Cobro (PaymentNotes)'" + _factura.GetNumeroFactura() + "'" + Environment.NewLine + exTramaCobro.InnerException.Message);

                            }
                            //evelasco .fin    
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Después de armar trama para bin_descripcion.'" + _factura.GetNumeroFactura() + "' : '" + bin_descripcion + "'");


                            // Nuevo Autorizador "AUSTRO".  JM  04-12-2020
                            string nombreRed = "";
                            if (resptrama.codigoRed == "01")
                                nombreRed = "DATAFAST";
                            else if (resptrama.codigoRed == "02")
                                nombreRed = "MEDIANET";
                            else if (resptrama.codigoRed == "03")
                                nombreRed = "AUSTRO";

                            //Agregar el pago a un repositorio temporal
                            PagoPinpadBackground = new Models.PagoPinpad
                            {
                                Valor = valor,
                                Banco = nombreRed,
                                Nombre = resptrama.nomGruTar,
                                Marca = resptrama.codBancoAdq,
                                TipoPos = nombreRed,
                                NumBin = numBinTC,
                                BinDescripcion = bin_descripcion
                            };
                            /*PagoPinpadBackground = new Models.PagoPinpad
                            {
                                Valor = valor,
                                Banco = resptrama.codigoRed == "02" ? "MEDIANET" : "DATAFAST",
                                Nombre = resptrama.nomGruTar,
                                Marca = resptrama.codBancoAdq,
                                TipoPos = resptrama.codigoRed == "02" ? "MEDIANET" : "DATAFAST",
                                BinDescripcion = bin_descripcion
                            };*/
                            // Nuevo Autorizador "AUSTRO".  JM  04-12-2020

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Inserta el valor de la trama en el modelo PagoPinpadBackground.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-23
                            //Impresion de voucher
                            String tipovoucher = "";
                            if (Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma > 0 && valor <= Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma)
                            {
                                tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCreditoSinFirma;
                            }
                            else
                            {
                                tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                            }

                            //Recibo original
                            string printResponse = string.Empty;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Imprime voucher Original.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                            prepararVoucherTarjeta(tipovoucher, resptrama, trama, ref printResponse);
                            printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 1: " + Environment.NewLine + printResponse + Environment.NewLine));
                            //Recibo copia
                            printResponse = string.Empty;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Imprime voucher Copia.'" + _factura.GetNumeroFactura() + "' ");//evelasco 2019-09-25
                            prepararVoucherTarjeta(tipovoucher, resptrama, trama, ref printResponse, true);
                            printWarnings += (string.IsNullOrEmpty(printResponse) ? "" : ("Incidencias durante impresion 2: " + Environment.NewLine + printResponse + Environment.NewLine));

                            if (!string.IsNullOrWhiteSpace(printWarnings))
                            {
                                ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco pero algunos recibos no pudieron ser impresos en este momento. Por favor realice la reimpresion usando el comando F4 y seleccionando la opcion Reimpresion de Vouchers";
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Ocurrieron incidencias durante la impresion de los recibos voucher en la factura '" + _factura.GetNumeroFactura() + "'" + Environment.NewLine + printWarnings);
                            }

                            fueProcesadoImpresionVocuhers = true;

                            //grabar transaccion en tabla pos_voucher
                            //cambiar aqui

                            pos_voucher.TARJETA = resptrama.numTarTuncate.Trim().PadRight(19, ' ');// ("520081XXXXXX6017   "); //19 ;

                            //revisar
                            string codigoproceso = "000200";
                            //es 003000 cuando es transacciones con tarjeta de crédito, 001000 cuando es transacción de tarjeta de debito cuenta de ahorro y 002000 cuando es transacción de tarjeta de debito cuenta corriente.

                            pos_voucher.CODIGOPROCESO = codigoproceso;// ("000200"); //6 ;
                                                                      //revisar

                            pos_voucher.FECHACONSUMO = resptrama.fechaTrans;// ("20161122"); //8 ;
                            pos_voucher.HORACONSUMO = resptrama.horaTrans;// ("114339"); //6 ;
                            pos_voucher.NUMEROVOUCHER = resptrama.secuencialtransaccion;// ("000002");//6 ;

                            pos_voucher.AUTORIZACION = trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut; //6 ;
                            pos_voucher.ANULADO = trama.TipoTransaccion == "03" ? true : false;

                            pos_voucher.VALORCONSUMO = trama.montoTotalTransaccion.PadLeft(13, '0');// ("0000000001200"); //13 ;
                            pos_voucher.FORMAAUTORIZA = ("1"); //1 ;
                            if (resptrama.codigoRed == "02")
                                pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo; //2 ;
                            else
                                pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData; //2 ;
                            pos_voucher.PLAZO = trama.plazoDiferido.PadLeft(2, '0');// ("06"); //2 ;

                            pos_voucher.TIPOLECTURA = resptrama.modoLectura.PadLeft(3, '0');// ("005"); //3 ;
                            pos_voucher.TIPOMONEDA = ("840"); //3 ;
                            pos_voucher.VALORIVA = trama.impuestoIvaTransaccion.PadLeft(13, '0');// ("0000000000147"); //13 ;
                            pos_voucher.VALORSERVICIO = ("0000000000000"); //13 ;
                            pos_voucher.VALORPROPINA = trama.popinaTransaccion.PadLeft(13, '0');// ("0000000000000"); //13 ;
                            pos_voucher.VALORINTERES = resptrama.valInteres.Trim().PadLeft(13, '0');// ("0000000000057");  //13 ;
                            pos_voucher.VALORFIJO = resptrama.montoFijo.Trim().PadLeft(13, '0');// ("0000000000000");  //13 ;
                            pos_voucher.TIPOPROMOCION = ("00"); //2 ;
                            pos_voucher.MESESGRACIA = trama.mesGracia.PadLeft(2, '0');//                        ("00");  //2 ;
                            pos_voucher.EMPRESASERVICIO = ("0000");  //4 ;
                            //pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "R"); //1 ;
                            pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "O"); //JCañarte
                            pos_voucher.CODIGORESPUESTA = resptrama.codigoRespuesta;// ("00"); //2 ;
                            pos_voucher.TIPODISPOSITIVO = ("2"); //1 ;
                            pos_voucher.ADQUIRENTETARJETA = ("CREDIMATIC01"); //12 ;
                            pos_voucher.ADQUIRENTESERVICIO = ("            ");  //12 ;
                            pos_voucher.MONTOGRAVAIVA = trama.montoBaseGravaIVa.PadLeft(13, '0');// ("0000000001053"); //13 ;
                            pos_voucher.MONTONOGRAVAIVA = trama.montoBaseNoGravaIVa.PadLeft(13, '0');// ("0000000000000");  //13 ;
                            pos_voucher.PUNTOEMISION = _factura.Establecimiento + _factura.PtoEmision;
                            pos_voucher.PROCESADO = false;
                            pos_voucher.GRUPOTAR = resptrama.nomGruTar;
                            pos_voucher.AUTORIZADOR = int.Parse(resptrama.codigoRed);
                            pos_voucher.LOTE = resptrama.numerolote;
                            pos_voucher.FACTURA = _factura.GetNumeroFactura();
                            //ML [16/01/2018]: Nuevos campos
                            pos_voucher.ARQC = resptrama.ARQC;
                            pos_voucher.AIDEMV = resptrama.AIDEMV;
                            pos_voucher.EMV = resptrama.idEMV;
                            pos_voucher.TC = resptrama.tipoCritoyValorEMV;
                            pos_voucher.PUBLICIDAD = resptrama.mensajePremioPublicidad;
                            pos_voucher.TIPOTRANSACCION = trama.TipoTransaccion;
                            pos_voucher.BANCOADQUIRIENTE = resptrama.nomBancoAdq;
                            pos_voucher.TARJETAHABIENTE = resptrama.nombreTarjetaHabiente;
                            pos_voucher.MID = resptrama.merchantId; //trama.MID;
                            pos_voucher.TID = trama.TID;
                            pos_voucher.VENCTAR = resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX";
                            pos_voucher.ANULAUTORIZACION = trama.numAutorizacion;
                            pos_voucher.TIPOBANCOTARJETA = (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);
                            //------------------------------
                            bool ExisteVoucher = pos.POS_VOUCHER.Any(x => x.FECHACONSUMO == pos_voucher.FECHACONSUMO && x.VALORCONSUMO == pos_voucher.VALORCONSUMO && x.AUTORIZACION == pos_voucher.AUTORIZACION && x.NUMEROVOUCHER == pos_voucher.NUMEROVOUCHER && x.FACTURA == pos_voucher.FACTURA);
                            if (!ExisteVoucher)
                            {
                                pos.POS_VOUCHER.Add(pos_voucher);
                            }
                            // pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet += 1;
                            pos.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            if (!fueProcesadoImpresionVocuhers)
                            {
                                printWarnings += "Acaba de ocurrir una excepcion durante la grabacion del voucher al sistema sin llegar siquiera al bloque de impresion de voucher";
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", printWarnings);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Excepcion: " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "StackTrace: " + ex.StackTrace);
                            }
                            string msjSeImprimieronVouchers = string.IsNullOrWhiteSpace(printWarnings) ? " y se imprimieron los recibos," : ", a pesar de que no se pudo imprimir los recibos en el momento,";
                            string msjParaDevteam = "En el POS del siguiente punto de emision, si bien se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers + " el voucher no pudo ser grabado en nuestra base interna. Generar el registro en POS_VOUCHER inmediatamente pues puede provocar descuadres en los cierres. Esto pudo deberse a un breve inconveniente, se recomienda una vez generado el registro, verificar la causa de la novedad";

                            string datosVoucher = "\nTARJETA: " + resptrama.numTarTuncate.Trim().PadRight(19, ' ') +
                                                    "\nCODIGOPROCESO: 000200" +
                                                    "\nFECHACONSUMO: " + resptrama.fechaTrans +
                                                    "\nHORACONSUMO: " + resptrama.horaTrans +
                                                    "\nNUMEROVOUCHER: " + resptrama.secuencialtransaccion +
                                                    "\nAUTORIZACION: " + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) +
                                                    "\nANULADO: " + (trama.TipoTransaccion == "03" ? "1" : "0") +
                                                    "\nVALORCONSUMO: " + trama.montoTotalTransaccion.PadLeft(13, '0') +
                                                    "\nFORMAAUTORIZA: 1" +
                                                    "\nTIPOCONSUMO: " + (resptrama.codigoRed == "02" ? ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo : ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData) +
                                                    "\nPLAZO: " + trama.plazoDiferido.PadLeft(2, '0') +
                                                    "\nTIPOLECTURA: " + resptrama.modoLectura.PadLeft(3, '0') +
                                                    "\nTIPOMONEDA: 840" +
                                                    "\nVALORIVA: " + trama.impuestoIvaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORSERVICIO: 0000000000000" +
                                                    "\nVALORPROPINA: " + trama.popinaTransaccion.PadLeft(13, '0') +
                                                    "\nVALORINTERES: " + resptrama.valInteres.Trim().PadLeft(13, '0') +
                                                    "\nVALORFIJO: " + resptrama.montoFijo.Trim().PadLeft(13, '0') +
                                                    "\nTIPOPROMOCION: 00" +
                                                    "\nMESESGRACIA: " + trama.mesGracia.PadLeft(2, '0') +
                                                    "\nEMPRESASERVICIO: 0000" +
                                                    //"\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "R") +
                                                    "\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "O") +  //JCanarte
                                                    "\nCODIGORESPUESTA: " + resptrama.codigoRespuesta +
                                                    "\nTIPODISPOSITIVO: 2" +
                                                    "\nADQUIRENTETARJETA: CREDIMATIC01" +
                                                    "\nADQUIRENTESERVICIO:             " +
                                                    "\nMONTOGRAVAIVA: " + trama.montoBaseGravaIVa.PadLeft(13, '0') +
                                                    "\nMONTONOGRAVAIVA: " + trama.montoBaseNoGravaIVa.PadLeft(13, '0') +
                                                    "\nPUNTOEMISION: " + _factura.Establecimiento + _factura.PtoEmision +
                                                    "\nPROCESADO: 0" +
                                                    "\nGRUPOTAR: " + resptrama.nomGruTar +
                                                    "\nAUTORIZADOR: " + resptrama.codigoRed +
                                                    "\nLOTE: " + resptrama.numerolote +
                                                    "\nFACTURA: " + _factura.GetNumeroFactura() +
                                                    "\nARQC: " + resptrama.ARQC +
                                                    "\nAIDEMV: " + resptrama.AIDEMV +
                                                    "\nEMV: " + resptrama.idEMV +
                                                    "\nTC: " + resptrama.tipoCritoyValorEMV +
                                                    "\nPUBLICIDAD: " + resptrama.mensajePremioPublicidad +
                                                    "\nTIPOTRANSACCION: " + trama.TipoTransaccion +
                                                    "\nBANCOADQUIRIENTE: " + resptrama.nomBancoAdq +
                                                    "\nTARJETAHABIENTE: " + resptrama.nombreTarjetaHabiente +
                                                    "\nMID: " + resptrama.merchantId + //trama.MID +
                                                    "\nTID: " + trama.TID +
                                                    "\nVENCTAR: " + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") +
                                                    "\nANULAUTORIZACION: " + trama.numAutorizacion +
                                                    "\nTIPOBANCOTARJETA: " + (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);

                            datosVoucher += Environment.NewLine + Environment.NewLine + scriptInsertarVoucher;

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero el voucher no pudo ser grabado en nuestra base interna, " + Environment.NewLine + "a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex) + Environment.NewLine + "StackTrace:" + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Datos Voucher: " + datosVoucher.Replace("\n", Environment.NewLine));

                            try
                            {
                                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                    Properties.Settings.Default.MAILERROR_FROM,
                                    Properties.Settings.Default.MAILERROR_ALIAS,
                                    Properties.Settings.Default.MAILERROR_DESTINO,
                                    Properties.Settings.Default.MAILERROR_CC,
                                    "Voucher pinpad no se grabó en nuestra base interna",
                                    String.Format(msjParaDevteam +
                                                    "  \n\nDatos caja-----------------" +
                                                    "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                                    "  \n\nDatos voucher-----------------" +
                                                    datosVoucher,
                                                Control.Common.GlobalParameters.Establecimiento,
                                                Control.Common.GlobalParameters.PuntoEmision,
                                                Control.Common.GlobalParameters.IpMaquina,
                                                Control.Common.GlobalParameters.UsuarioNombre,
                                                Control.Common.GlobalParameters.Usuario,
                                                Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                                (fueProcesadoImpresionVocuhers ? "" : ("StackTrace: " + ex.StackTrace + " \n"))),
                                    false,
                                    String.Empty);

                                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "No se pudo enviar notificacion del problema al grabar voucher en nuestra base interna, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                                }

                                //eevv 2020-01-13
                                try
                                {
                                    string Stringremove = string.Empty;
                                    Stringremove = "SCRIPT PARA INSERTAR: " + Environment.NewLine;
                                    creaArhivoTemporalVoucher(scriptInsertarVoucher.Replace(Stringremove, string.Empty));
                                }
                                catch (Exception ex3)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "No se pudo generar archivo TXT de insert para la POS_VOUCHER , a continuacion las excepciones encontradas - " + ex3.Message);

                                }
                            }
                            catch { }

                            ResponseBackground = "Estimad@ usuario, se realizo correctamente la transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero detectamos que el voucher no pudo ser grabado en nuestra base interna. Por favor comuníquelo inmediatamente al administrador para que realice su gestión y evitar descuadre durante su cierre";
                        }

                        //Agregar lineas de insert
                        Control.Common.Logger.Agregar_Trace_Voucher(pos_voucher);

                        DebeCerrarFormBackground = true;
                    }
                    else
                    {
                        ResponseBackground = "Error : " + resptrama.mensajeRespuesta;

                        //Enviar trama de reverso de transaccion tipo 04.   JM  25-08-2020
                        envioGen = new ClsEnviaPinPadGeneral();
                        trama.TipoTransaccion = "04";
                        pinpadResponse = envioGen.SendRequestPinpadAnt("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);
                        // var pinpadResponse = envio.Envio_requerimientoPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);                       
                        resptrama.ObtieneDato = pinpadResponse;
                        ResponseBackground = ResponseBackground + " \n" + "Reverso : " + resptrama.mensajeRespuesta;
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Reverso de transacción no exitosa, envio: " + trama.DevuelveTrama);
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Reverso de transacción no exitosa, respuesta : " + resptrama.mensajeRespuesta);
                    }
                }
            }
            catch (Exception ex)
            {
                ResponseBackground = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a una breve interrupcion en el servicio, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco";
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "La solicitud no pudo ser realizada debido a una breve interrupcion en el servicio, esto puede deberse a un breve mantenimiento, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
            }
            finally
            {


                On_Off_Controles(true);
            }
        }

        private void creaArhivoTemporalVoucher(string voucher)
        {
            try
            {
                //se adiciona alguna información y la fecha
                DateTime dateTime = new DateTime();
                dateTime = DateTime.Now;
                string strDate = Convert.ToDateTime(dateTime).ToString("yyyyMMddHHmmss");
                string rutaCompleta = Control.Common.GlobalParameters.VoucherInsertPath + _factura.GetNumeroFactura() + "_" + strDate + ".txt";
                if (!string.IsNullOrEmpty(Control.Common.GlobalParameters.VoucherInsertPath))
                {
                    CreateEmptyDirectory(Control.Common.GlobalParameters.VoucherInsertPath);
                    using (StreamWriter mylogs = File.AppendText(rutaCompleta))         //se crea el archivo
                    {
                        mylogs.WriteLine(voucher);

                        mylogs.Close();
                    }

                }

            }
            catch (Exception ex)
            {


                //Insertar en el log y luego enviar correo.
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "creaArhivoTemporalVoucher", "Ha ocurrido una excepción al momento de generar archivo POS VOUCHER - A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                //Enviar Correo:
                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                           Properties.Settings.Default.MAILERROR_FROM,
                           Properties.Settings.Default.MAILERROR_ALIAS,
                           Properties.Settings.Default.MAILERROR_DESTINO,
                           Properties.Settings.Default.MAILERROR_CC,
                           "Voucher pinpad no se generó archivo temporal para el insert en la pos_voucher",
                           String.Format("  \n\nDatos caja-----------------" +
                                           "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                           "  \n\n-----------------" +
                                         //datosVoucher,
                                         string.Empty,
                                       Control.Common.GlobalParameters.Establecimiento,
                                       Control.Common.GlobalParameters.PuntoEmision,
                                       Control.Common.GlobalParameters.IpMaquina,
                                       Control.Common.GlobalParameters.UsuarioNombre,
                                       Control.Common.GlobalParameters.Usuario,
                                       Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                       (false ? "" : ("StackTrace: " + ex.StackTrace + " \n"))),
                           false,
                           String.Empty);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "creaArhivoTemporalVoucher", "No se pudo enviar notificacion del problema al grabar voucher en archivo temporal, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }

            }
        }

        /// <summary>
        /// Crear un directorio vacio
        /// </summary>
        public static void CreateEmptyDirectory(string fullPath)
        {
            if (!System.IO.Directory.Exists(fullPath))
            {
                System.IO.Directory.CreateDirectory(fullPath);
            }
        }
        private void btnEnter_Click(object sender, EventArgs e)
        {
            try
            {
                EjecutarPago();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnEnter_Click", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }

        }

        private void EjecutarPago()
        {
            Screen targetScreen = Control.Common.General.GetScreenCajero();
            string msjError = string.Empty;
            var valor = 0M;
            string autorizador;




            try
            {

                if (decimal.TryParse(txtValor.Text.Trim(), out valor) && valor > 0)
                {
                    txtValor.Text = decimal.Parse(txtValor.Text.Trim()).ToString("#######.00");
                    if (txtValor.Text.Substring(0, 1) == ".")
                    {
                        txtValor.Text = "0" + txtValor.Text;
                    }





                    switch (_pagoTipo)
                    {

                        case PagoTipo.TarjetaCredito:

                            if (decimal.Parse(txtValor.Text) == 0)
                            {
                                Control.Common.General.GetMensajeToList(614);
                                return;
                            }


                            if (decimal.Parse(txtValor.Text) > _factura.GetTotal())
                            {
                                Control.Common.General.GetMensajeToList(193);
                                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El valor a pagar no debe ser mayor a la venta", " ");
                                //MessageBox.Show(this, "El valor a pagar no debe ser mayor a la venta");
                                return;
                            }

                            //JCañarte Validad Diferido no sea 0 meses
                            if (this.cmbTipoTransaccion.SelectedItem.ToString() == "DIFERIDO" && this.cmbDiferido.SelectedIndex == -1)
                            {
                                //MessageBox.Show(this, "Debe seleccionar número de meses en plazo diferido");
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Debe seleccionar número de meses en plazo diferido", " ");
                                Control.Common.General.GetMensajeToList(194);
                                return;
                            }

                            var pos = new POSEntities();

                            var ProcesaPinpad = (from deta in pos.core_parametro
                                                 where deta.identificador == "PINPAD"
                                                 && deta.parametro2 == this._factura.Establecimiento
                                                 && deta.valor == "TRUE"
                                                 select deta).ToList();

                            if (ProcesaPinpad.ToList().Count() > 0 && EsManual == 0)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutarPago", "Usuario solicita pago PINPAD por un valor de " + txtValor.Text);
                                ProcesaPinpadBackGround();
                            }

                            else if (validarTarjetaCredito())
                            {
                                /*if (cmbBancoTarjeta.SelectedValue.ToString() !="0"  && cmbTipoTransaccion.SelectedValue.ToString() != "0")
                                {*/
                                if (!ValidaPagoTotalPromoBines(decimal.Parse(txtValor.Text)))
                                    return;

                                //Valida si tiene lleno los campos del voucher.
                                if (validaDatosVoucherPManual())
                                {
                                    string tramaTC = string.Empty; //comentar despues de pruebas
                                                                   //TramaCobro = "|" + resptrama.secuencialtransaccion + ";" + autori + ";" + resptrama.fechaTrans + ";" + txtValor.Text;//evelasco se envia trama para utilizar el proceso AX para cruzar con los cobros por TC - MEDIANET, DATAFAST.
                                    tramaTC = "|" + txtNumTransaccionVoucher.Text.Replace("_", string.Empty) + ";" + txtNumAutorizacionVoucher.Text.Replace("_", string.Empty) + ";" + DateTime.Now.ToString("yyyyMMdd") + ";" + valor.ToString();
                                    //tramaTC = Control.Common.GlobalParameters.ActivarFormaPagoTC ? tramaTC : string.Empty;

                                    if (cmbTipo.Text == "-- Seleccione --" || cmbTID.Text == "" || string.IsNullOrWhiteSpace(txtLote.Text))
                                    {
                                        var msgerror = (cmbTipo.Text == "-- Seleccione --" ? "seleccionar el Tipo que está usando " : "") + (cmbTID.Text == "" ? ", seleccionar el TID " : "");
                                        msgerror += (string.IsNullOrWhiteSpace(txtLote.Text) ? (cmbTID.Text == "" ? ", digitar el Lote" : " , digitar el Lote") : "");

                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutarPago", "Usuario solicita el cobro manual sin : " + msgerror);
                                        //MessageBox.Show(this, "Tiene que " + msgerror, "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        Control.Common.General.GetMensaje("POS - Pago Manual", "Tiene que " + msgerror, "I");

                                        return;
                                    }
                                    else if (cmbTipo.Text == "DIFERIDO" && cmbDiferido.SelectedIndex == -1)
                                    {
                                        //MessageBox.Show(this, "Tiene que seleccionar el plazo a diferir", "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        //Control.Common.General.GetMensaje("POS - Pago Manual", "Tiene que seleccionar el plazo a diferir", "I");
                                        Control.Common.General.GetMensajeToList(195);
                                        return;
                                    }
                                    else if (cmbTipo.Text == "DIFERIDO" && cmbTipoPago.Text.Contains("GRACIA") && cmbMesesGracia.SelectedIndex == -1)
                                    {
                                        // MessageBox.Show(this, "Tiene que seleccionar los meses de gracia", "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        // Control.Common.General.GetMensaje("POS - Pago Manual", "Tiene que seleccionar los meses de gracia", "I");
                                        Control.Common.General.GetMensajeToList(196);
                                        return;
                                    }


                                    if (cmbTipoTransaccion.Text != "-- Seleccione --" && cmbBancoTarjeta.Text != "-- Seleccione --")//if (cmbTipoTransaccion.SelectedIndex != null && cmbBancoTarjeta.Text != "  -  ")
                                    {
                                        string bin_red = string.Empty;
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutarPago", "Usuario solicita pago MANUAL por un valor de " + txtValor.Text);
                                        //var obj = cmbBancoTarjeta.SelectedValue as core_tarjetacredito;
                                        var obj = cmbBancoTarjeta.SelectedValue as core_tarjetacredito_bin;
                                        //validacion si no detecta la tarjeta de credito, entonces el usuario seleccionó el cambo manual de bancos.
                                        if (obj == null)
                                        {
                                            var obj1 = cmbBancoTarjeta.SelectedValue as core_tarjetacredito;
                                            Dictionary<string, string> _dTipoTran;
                                            RadListDataItem raitemSelected;

                                            bin_red = cmbTipoTransaccion.SelectedValue.ToString();

                                            autorizador = bin_red == "2" ? "Medianet" : "DataFast";
                                            autorizador = bin_red == "02" ? "Medianet" : autorizador;
                                            // _factura.AgregarPagoTarjetaCredito(valor, obj1.core_banco.nombre, obj1.nombre, obj1.tipo, cmbTipoTransaccion.SelectedItem.ToString());
                                            _factura.AgregarPagoTarjetaCredito(valor, obj1.core_banco.nombre, obj1.nombre, cmbTipoTransaccion.SelectedItem.ToString(), autorizador, tramaTC); // descomentar 15/10/1019                                    
                                                                                                                                                                                              //tramaTC = "000004;735740;" + System.DateTime.Now.ToString("yyyyMMdd") + ";" + valor.ToString().Trim();
                                                                                                                                                                                              //_factura.AgregarPagoTarjetaCredito(valor, autorizador, obj1.nombre, autorizador, cmbTipoTransaccion.SelectedItem.ToString(), tramaTC);//comentar despues de pruebas
                                        }
                                        else
                                        {
                                            bin_red = obj.bin_red;
                                            autorizador = obj.bin_red == "2" ? "MEDIANET" : "DATAFAST";
                                            autorizador = bin_red == "02" ? "MEDIANET" : autorizador;
                                            this.guardarPagoManualPosVoucher(bin_red);


                                            _factura.AgregarPagoTarjetaCredito(valor, autorizador, obj.bin_descripcion, autorizador, cmbTipoTransaccion.SelectedItem.ToString(), tramaTC);
                                        }


                                        EsPagoOkPromoTarjeta = true;
                                        EsManual = 0;
                                        tarjetaDetectadaPagoManual = false;
                                        //evelasco 2019-12-17 .ini
                                        if (_esPedidoDomicilio)
                                        {
                                            _factura.AgregaOrdenApp(_ordenApp);
                                        }
                                        //evelasco 2019-12-17 .fin
                                        this.Close();

                                        //eevv 2020-01-13
                                        try
                                        {
                                            string strA = string.Empty;
                                            strA = "SCRIPT PARA INSERTAR: " + Environment.NewLine;
                                            strA += "INSERT INTO " + Environment.NewLine;
                                            strA += "PAGO MANUAL";

                                            string Stringremove = string.Empty;
                                            Stringremove = "SCRIPT PARA INSERTAR: " + Environment.NewLine;
                                            //creaArhivoTemporalVoucher(scriptInsertarVoucher.Replace(Stringremove, string.Empty));
                                            creaArhivoTemporalVoucher(strA.Replace(Stringremove, string.Empty));
                                        }
                                        catch (Exception ex3)
                                        {
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "No se pudo generar archivo TXT de insert para la POS_VOUCHER , a continuacion las excepciones encontradas - " + ex3.Message);
                                        }
                                    }
                                    else
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutarPago", "Usuario solicita el cobro manual sin haber indicado el tipo de tarjeta");
                                        //MessageBox.Show(this, "Tiene que escoger la tarjeta que esta usando!");
                                        //MessageBox.Show(this, "Tiene que Seleccionar la Tarjeta y el Autorizador que está usando!", "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);                                   
                                        //Control.Common.General.GetMensaje("POS - Pago Manual", "Tiene que Seleccionar la Tarjeta y el Autorizador que está usando!", "I");
                                        Control.Common.General.GetMensajeToList(197);


                                    }
                                }
                                else
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutarPago", "Usuario solicita el cobro manual sin haber indicado el tipo de tarjeta");
                                    //MessageBox.Show(this, "Debe Llenar los datos del Voucher (TRANSACCION# Y AUTORIZACION#) para realizar el cobro!", "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);                               
                                    //Control.Common.General.GetMensaje("POS - Pago Manual", "Llenar los datos del Voucher (TRANSACCION# Y AUTORIZACION#) para realizar el cobro!", "I");
                                    Control.Common.General.GetMensajeToList(198);

                                }
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutarPago", "Usuario solicita el cobro de tarjeta credito sin haber llenado la información de la misma");
                                //MessageBox.Show(this, "Favor llenar la informacion de la tarjeta");
                                //Control.Common.General.GetMensaje("POS - Pago Manual", "Favor llenar la informacion de la tarjeta", "I");
                                Control.Common.General.GetMensajeToList(199);

                            }
                            break;

                        case PagoTipo.Cheque:
                            if (validarCheque())
                            {
                                var obj = cmbBancoTarjeta.SelectedValue as core_banco;
                                _factura.AgregarPagoCheque(valor, obj.nombre, txtNumCheque.Text, txtCuenta.Text);
                                this.Close();
                            }
                            else
                            {
                                Control.Common.General.GetMensajeToList(200);
                                // Control.Common.General.GetMensaje("POS - Pago Manual", "Favor llenar la informacion del cheque", "I");
                                //MessageBox.Show(this, "Favor llenar la informacion del cheque");

                            }
                            break;

                        case PagoTipo.TarjetaRegalo:
                            {
                                // 1. VALIDACIÓN PRELIMINAR (Tu código original - está bien)
                                if (valor > saldoFacturaPago)
                                {
                                    this.BeginInvoke((MethodInvoker)delegate
                                    {
                                        Control.Common.General.GetMensajeToList(689); // "El valor ingresado es mayor al saldo"
                                    });
                                    return;
                                }

                                // 2. DECLARACIÓN DE VARIABLES
                                string identificacion = string.Empty;
                                string nombreGrupo = string.Empty;
                                bool estaAsociadaGrupoCliente = false;
                                Decimal valorgiftcard = 0;
                                Decimal dValorGiftCard = 0;

                                ClienteEmpleado clteEmpleado2 = Common.General.ValidaClienteEmpleado(_textoOriginal, Common.GlobalParameters.Establecimiento, Common.GlobalParameters.PINPAD_MULTIRED);

                                // ====================================================================================
                                // INICIO: LÓGICA DE APP MÓVIL (MÉTODO DE SUMA TOTAL) jchid
                                // ====================================================================================
                                if (_textoOriginal.Trim().StartsWith(Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
                                {
                                    string queryVT = "Exec [PtsCliente].[spConsultaGiftCardAppGen] @Identificacion";
                                    string cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;

                                    if (string.IsNullOrEmpty(cadenaCon))
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/BasePagos", "EjecutarPago", "No hay parametro 'CON_SERVER_PUNTOS'.");
                                        return;
                                    }

                                    try
                                    {
                                        // --- PASO 1: LEER TODAS LAS TARJETAS A UNA LISTA ---
                                        var tarjetasEncontradas = new List<dynamic>();
                                        using (SqlConnection conn = new SqlConnection(cadenaCon))
                                        {
                                            using (SqlCommand select = new SqlCommand(queryVT, conn))
                                            {
                                                select.Parameters.Add(new SqlParameter("@Identificacion", clteEmpleado2.Identificacion));
                                                conn.Open();
                                                SqlDataReader dr = select.ExecuteReader();
                                                while (dr.Read())
                                                {
                                                    tarjetasEncontradas.Add(new
                                                    {
                                                        IdGiftCard = dr.GetValue(1).ToString(),
                                                        IdCliente = dr.GetValue(2).ToString(),
                                                        Saldo = dr.GetValue(3).ToString()
                                                    });
                                                }
                                            }
                                        }

                                        // --- PASO 2: SUMAR EL SALDO TOTAL Y VALIDAR ---
                                        decimal totalSaldoGiftCard = 0;
                                        foreach (var tarjetaData in tarjetasEncontradas)
                                        {
                                            // (Omitimos 'getTarjetaGen' y 'TieneSaldoCuadrado' que estaban comentados)
                                            decimal saldoEstaTarjeta = 0;
                                            decimal.TryParse(tarjetaData.Saldo, out saldoEstaTarjeta);
                                            totalSaldoGiftCard += saldoEstaTarjeta;
                                        }

                                        // Si el monto a pagar (valor) es mayor que el saldo total, mostramos error y salimos.
                                        if (valor > totalSaldoGiftCard)
                                        {
                                            this.BeginInvoke((MethodInvoker)delegate {
                                                Control.Common.General.GetMensajeToList(180); // "Saldo insuficiente"
                                            });
                                            return;
                                        }

                                        // --- PASO 3: CREAR EL PAGO "GIFT CARDV" UNA SOLA VEZ ---
                                        // Le pasamos el 'valor' total que vamos a pagar 
                                        Pago pago = ((Factura)_factura).AgregarPago("GIFT CARDV", valor);

                                        // --- PASO 4: APLICAR LA CASCADA (PERO SOLO A LOS SUB-PAGOS) ---
                                        decimal montoPendiente = valor;

                                        foreach (var tarjetaData in tarjetasEncontradas)
                                        {
                                            if (montoPendiente <= 0) break;

                                            // Cargamos el objeto 't' solo para obtener el saldo actualizado
                                            var t = new TarjetaRegalo();
                                            t.getTarjetaGen(tarjetaData.IdGiftCard, tarjetaData.IdCliente, true);

                                            decimal saldoEstaTarjeta = 0;
                                            decimal.TryParse(tarjetaData.Saldo, out saldoEstaTarjeta);

                                            if (saldoEstaTarjeta <= 0)
                                                continue;

                                            // Calculamos cuánto consumir de ESTA tarjeta
                                            decimal montoAConsumir = Math.Min(montoPendiente, saldoEstaTarjeta);

                                            // AÑADIMOS EL SUB-PAGO DIRECTAMENTE A LA LISTA INTERNA DEL 'pago'
                                            pago.Pagos.Add(new PagoGiftCard()
                                            {
                                                Codigo = tarjetaData.IdGiftCard,
                                                Valor = montoAConsumir, // varias itereaciones dependiendo cuantas giftcard este activas
                                                Saldo = t.getSaldoGiftCard() - montoAConsumir, // Saldo restante en la tarjeta
                                                EstaAsociadaGrupoCliente = estaAsociadaGrupoCliente,
                                                IdentificacionGrupoCliente = identificacion,
                                                NombreGrupoCliente = nombreGrupo
                                            });

                                            // Actualizamos el monto pendiente
                                            montoPendiente = montoPendiente - montoAConsumir;
                                        }

                                        // --- PASO 5: RECALCULAR Y CERRAR ---

                                        // Recalculamos el valor total del pago UNA SOLA VEZ
                                        pago.calcularTotal();

                                        // Cerramos con OK
                                        this.DialogResult = DialogResult.OK;
                                        this.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        // Si algo falla (como la NullReferenceException que buscábamos)
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/BasePagos", "EjecutarPago", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                                        this.BeginInvoke((MethodInvoker)delegate
                                        {
                                            Control.Common.General.GetMensajeToList(180);
                                        });
                                        return;
                                    }
                                }
                                // ====================================================================================
                                // FIN: BLOQUE DE APP MÓVIL jchid
                                // ====================================================================================
                                else
                                {
                                    // ====================================================================================
                                    // <-- INICIO: BLOQUE DE TARJETA FÍSICA (Tu código original, SIN CAMBIOS)
                                    // ====================================================================================
                                    try
                                    {
                                        valorgiftcard = Decimal.Parse(this.lblGiftCardSaldo.Text.Substring(1, this.lblGiftCardSaldo.Text.Length - 1)); //valor que se ingresa para pagar el cajero JCHID
                                    }
                                    catch
                                    {
                                        //aqui falta agregar codigo para que atrape el error que se esta dando. JCHID
                                    }

                                    if (validarTarjetaRegalo(valor, valorgiftcard))
                                    {
                                        bool puedeAgregarPago = true;

                                        string msgError = string.Empty;
                                        // Estas variables se re-declaran aquí, lo cual está bien porque están en un ámbito diferente
                                        string identificacion_else = string.Empty;
                                        string nombreGrupo_else = string.Empty;
                                        bool estaAsociadaGrupoCliente_else = _tarjetaRegalo.EstaAsociadaGrupoCliente(ref identificacion_else, ref msgError, ref nombreGrupo_else);

                                        if (estaAsociadaGrupoCliente_else)
                                        {
                                            if (!string.IsNullOrEmpty(msgError))
                                            {
                                                puedeAgregarPago = false;
                                                Control.Common.General.GetMensaje("POS", msgError, "I"); ;
                                            }
                                            else
                                            {
                                                _debeActualizarClienteFactura = true;
                                                _identificacionActualizarClienteFactura = identificacion_else;
                                                _nombreActualizarClienteFactura = nombreGrupo_else;
                                            }
                                        }

                                        if (!_tarjetaRegalo.TieneSaldoCuadrado(((Models.Factura)_factura).Secuencia))
                                        {
                                            puedeAgregarPago = false;
                                        }

                                        if (puedeAgregarPago)
                                        {
                                            if (_tarjetaRegalo.tarjetaValida() && _tarjetaRegalo.getSaldo() >= valor)
                                            {
                                                _factura.AgregarPagoTarjetaRegalo(valor, _tarjetaRegalo.getCodigo(), _tarjetaRegalo.getSaldo() - valor, estaAsociadaGrupoCliente_else, identificacion_else, nombreGrupo_else);
                                                this.Close();
                                            }
                                            // <-- ADVERTENCIA: Este bloque 'else if' tiene el mismo error que acabamos de corregir.
                                            else if (_tarjetaRegalo.tarjetaValidaGiftCard() && valorgiftcard > 0)
                                            {
                                                var t = new TarjetaRegalo();
                                                try
                                                {
                                                    string QueryVT = "Exec [PtsCliente].[spConsultaGiftCardAppGen] '" + this._factura.ClienteIdentificacion + "' ";

                                                    if (Control.Common.GlobalParameters.ConServerPuntos != "")
                                                    {
                                                        SqlConnection conn = new SqlConnection(Control.Common.GlobalParameters.ConServerPuntos);
                                                        try
                                                        {
                                                            conn.Open();
                                                            SqlCommand select = new SqlCommand(QueryVT, conn);
                                                            IAsyncResult iar = select.BeginExecuteReader();
                                                            SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                                                            while (dr.Read())
                                                            {
                                                                if (t.getTarjetaGen(dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), true) && valor > 0)
                                                                {
                                                                    if (t.TieneSaldoCuadradoGiftEmpl(((Models.Factura)_factura).Secuencia))
                                                                    {
                                                                        _tarjetaRegalo = t;
                                                                        dValorGiftCard = decimal.Parse(dr.GetValue(3).ToString());
                                                                        if (valor <= dValorGiftCard)
                                                                        {
                                                                            if (_tarjetaRegalo.getCodigoGiftCard().Trim().StartsWith("2222"))
                                                                            {
                                                                                _factura.AgregarPagoTarjetaRegalo(valor, _tarjetaRegalo.getCodigoGiftCard(), _tarjetaRegalo.getSaldoGiftCard() - valor, estaAsociadaGrupoCliente_else, identificacion_else, nombreGrupo_else, "GIFT CARD");
                                                                            }
                                                                            else
                                                                            {
                                                                                _factura.AgregarPagoTarjetaRegalo(valor, _tarjetaRegalo.getCodigoGiftCard(), _tarjetaRegalo.getSaldoGiftCard() - valor, estaAsociadaGrupoCliente_else, identificacion_else, nombreGrupo_else, "GIFT CARDV");
                                                                            }
                                                                            valor = 0;
                                                                            this.Close(); // <-- Error: Cierra en la primera tarjeta
                                                                        }
                                                                        else
                                                                        {
                                                                            if (_tarjetaRegalo.getCodigoGiftCard().Trim().StartsWith("2222"))
                                                                            {
                                                                                _factura.AgregarPagoTarjetaRegalo(dValorGiftCard, _tarjetaRegalo.getCodigoGiftCard(), 0, estaAsociadaGrupoCliente_else, identificacion_else, nombreGrupo_else, "GIFT CARD");
                                                                            }
                                                                            else
                                                                            {
                                                                                _factura.AgregarPagoTarjetaRegalo(dValorGiftCard, _tarjetaRegalo.getCodigoGiftCard(), 0, estaAsociadaGrupoCliente_else, identificacion_else, nombreGrupo_else, "GIFT CARDV");
                                                                            }
                                                                            valor = valor - dValorGiftCard;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            conn.Close();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            conn.Close();
                                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/BasePagos", "EjecutarPago", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/BasePagos", "EjecutarPago", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Control.Common.General.GetMensajeToList(201);
                                    }
                                    // ====================================================================================
                                    // <-- FIN: BLOQUE DE TARJETA FÍSICA
                                    // ====================================================================================
                                }

                                break;
                            } // <-- Fin del 'case PagoTipo.TarjetaRegalo:'

                        case PagoTipo.NotaCredito:

                            if (validarNotaCredito(valor))
                            {

                                _factura.AgregarPagoNotaCredito(valor, _notaCredito.getCodigo());
                                this.Close();
                            }

                            //else
                            //{
                            //    //MessageBox.Show(this, "Favor llenar la informacion de la tarjeta regalo");
                            //    //Control.Common.General.GetMensaje("POS", "Favor llenar la informacion de la tarjeta regalo", "I");
                            //    Control.Common.General.GetMensajeToList(202);
                            //}
                            break;

                        case PagoTipo.TarjetaInterna:


                            try
                            {


                                if (validarTarjetaInterna(valor))
                                {
                                    _factura.AgregarPagoTarjetaInterna(valor, _tarjetaInterna.codigo, _tarjetaInterna.identificacion);

                                    _factura.TarjetaCreditoInterno = _tarjetaInterna;
                                    _factura.TarjetaCreditoInternoAdicional = _tarjetaInternaAdicional;

                                    _tarjetaInterna = null;
                                    _tarjetaInternaAdicional = null;


                                    this.Close();



                                }
                                break;
                            }
                            catch (Exception)
                            {

                                throw;
                            }





                        case PagoTipo.Retencion:

                        case PagoTipo.DineroElectronico:


                            string codigo = _textoOriginal;
                            ClienteEmpleado clteEmpleado = Common.General.ValidaClienteEmpleado(_textoOriginal, Common.GlobalParameters.Establecimiento, Common.GlobalParameters.PINPAD_MULTIRED);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnVerificar_Click"
                          , " Ejecuta consulta de saldo de GiftCard / Tarjeta de Regalo ");

                            var validaSaldos = MetodosBilletera.RecuperaSaldosPorIdentificacion(clteEmpleado.Identificacion);


                            if (validaSaldos.SaldoMonedero == 0)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "Ejecuta Pago"
                                , " Consutla Saldo. valor CERO ");

                                Control.Common.General.GetMensajeToList(111);
                                return;
                            }


                            if (validaSaldos.SaldoMonedero < valor)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "Ejecuta Pago"
                                 , " Consutla SaldoMonedero es menor al valor ");

                                Control.Common.General.GetMensajeToList(111);
                                return;
                            }


                            _factura.AgregarPagoMonedero(valor);
                            this.Close();


                            break;
                        case PagoTipo.CompraGratis:

                            // 1. VALIDACIÓN DE SEGURIDAD: ¿Ya se verificó la tarjeta y el saldo?
                            if (!_compraGratisCalculada)
                            {
                                // Mensaje: "Debe verificar la tarjeta y el saldo antes de aplicar el pago."
                                // Puedes usar un ID de mensaje existente o este genérico:
                                Control.Common.General.GetMensaje("POS - Compra Gratis", "Debe presionar 'Enter' o verificar la tarjeta para calcular el saldo autorizado antes de pagar.", "I");
                                txtCuenta.Focus();
                                return; // <--- ESTO DETIENE LA EJECUCIÓN Y EVITA EL PAGO EN FALSO
                            }

                            string codigoCG = !string.IsNullOrEmpty(_textoOriginal) ? _textoOriginal : txtCuenta.Text.Trim();

                            // 2. Validación extra: Que el código no se haya borrado
                            if (string.IsNullOrEmpty(codigoCG))
                            {
                                Control.Common.General.GetMensaje("POS - Compra Gratis", "No se detectó el código de la tarjeta.", "I");
                                txtCuenta.Focus();
                                return;
                            }

                            // 3. Validación de consistencia (Opcional pero recomendada)
                            // Aseguramos que el valor a pagar no sea mayor al permitido por la regla del 10% que calculamos antes
                            // (Asumiendo que txtValor ya tiene el valor correcto tras la verificación)

                            // JCHID Compra Gratis 
                            _factura.AgregarDescuentoPagoCompraGratis(valor, codigoCG);

                            this.Close();

                            break;


                        default:
                            break;
                    }
                }
                else
                {
                    //MessageBox.Show(this, "Asegúrese de ingresar un valor válido superior a cero (0). Recuerde solo ingresar valor numérico y no usar signos de moneda");               
                    // Control.Common.General.GetMensaje("POS", "Asegúrese de ingresar un valor válido superior a cero (0). Recuerde solo ingresar valor numérico y no usar signos de moneda", "I");
                    Control.Common.General.GetMensajeToList(203);

                    txtValor.Focus();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "ProcesaPinpad", "error: " + ex.Message);
            }

        }

        private bool validaHistoricoConsumoPtos(string clienteAx)
        {
            bool validaHistorico = false;

            try
            {

                if (Control.Common.GlobalParameters.ValidaCashBack == "")
                {

                    validaHistorico = true;
                }
                else
                {
                    decimal saldoPuntos = 0.0M;
                    string cadenaCon = "";

                    if (Control.Common.GlobalParameters.ConServerPuntos != "")
                    {
                        cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
                        // MessageBox.Show(this, "La Billetera Electrónica esta fuera de línea y no se puede utilizar (No hay parámetro 'CON_SERVER_PUNTOS' para este local), intente más tarde", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Control.Common.General.GetMensaje("POS - Consumo Puntos", "La Billetera Electrónica esta fuera de línea y no se puede utilizar (No hay parámetro 'CON_SERVER_PUNTOS' para este local), intente más tarde", "I");
                        Control.Common.General.GetMensajeToList(204);
                    }

                    if (cadenaCon != "")
                    {
                        SqlConnection conn = new SqlConnection(cadenaCon);
                        try
                        {
                            SqlParameter paramResult = new SqlParameter("@respuesta", SqlDbType.VarChar, -1);
                            paramResult.IsNullable = true;
                            paramResult.Direction = System.Data.ParameterDirection.Output;
                            var addParameters = new List<SqlParameter>
                                 {
                                    new SqlParameter("@AccountNum", clienteAx),
                                    paramResult
                                 };
                            SqlCommand select = new SqlCommand("Exec PtsCliente.spValidarHistoricoPuntosGen @AccountNum, @respuesta out", conn);
                            select.Parameters.AddRange(addParameters.ToArray());
                            conn.Open();
                            select.ExecuteNonQuery();
                            conn.Close();
                            string saldoPuntos_ = (string)paramResult.Value;
                            decimal.TryParse(paramResult.Value.ToString(), out saldoPuntos);

                            if (saldoPuntos > 0.0M)
                            {
                                _facturaApp.SaldoPuntos = saldoPuntos * Control.WalletPoints.ClsPoints.FactorCanje;
                                validaHistorico = true;
                            }
                            else
                            {
                                _facturaApp.SaldoPuntos = 0;
                                validaHistorico = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            validaHistorico = true;
                            //MessageBox.Show(this, "La Billetera Electrónica esta fuera de línea y no se puede utilizar, intente más tarde", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // Control.Common.General.GetMensaje("POS - Billetera Electrónica", "La Billetera Electrónica esta fuera de línea y no se puede utilizar, intente más tarde", "I");
                            Control.Common.General.GetMensajeToList(205);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "validaHistoricoConsumoPtos", "Billetera Electrónica esta fuera de línea, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        validaHistorico = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePago", "validaHistoricoConsumoPtos", "error: " + ex.Message);
            }

            return validaHistorico;
        }

        private bool validaSaldoPuntos(string clienteAx)
        {
            bool validaSaldo = false;
            decimal saldoPuntos = 0;
            decimal saldoPuntosdet = 0;
            if (Control.Common.GlobalParameters.ValidaCashBack == "")
            {

                validaSaldo = true;
            }
            else
            {
                using (POSEntities db = new POSEntities())
                {
                    if (db.TblPuntosCab.Any(x => x.AccountNum == clienteAx && x.Estado == 1))
                    {
                        var puntoscab = db.TblPuntosCab.Where(x => x.AccountNum == clienteAx && x.Estado == 1).ToList();

                        if (puntoscab != null && puntoscab.Count > 0)
                        {
                            saldoPuntos = puntoscab.Sum(x => x.Saldo);
                            int idptocab = puntoscab.FirstOrDefault().IdTblPuntosCab;
                            var puntosdet = db.TblPuntos.Where(x => x.IdTblPuntosCab == idptocab && x.Estado == 1 && x.IdLstCampania == 4).ToList();
                            saldoPuntosdet = puntosdet.Sum(x => x.Saldo);
                            if (Math.Round(saldoPuntos, 2) != Math.Round(saldoPuntosdet, 2))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "validaSaldoPuntos", "CI:" + clienteAx + ", saldo de puntos TblPuntosCab:" + saldoPuntos.ToString() + "TblPuntos:" + saldoPuntosdet.ToString());//stefany
                                string destinoMail = "devteam@liris.com.ec";

                                string msj = String.Format("En la siguiente caja se esta reportando inconsistencia debido a saldo de puntos. AccountNum:" + clienteAx + ". Saldo TblPuntosCab:" + saldoPuntos.ToString() + ". Saldo TblPuntos:" + saldoPuntosdet.ToString() + ", verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nIpServidor: {3} \nCajeroNombre: {4} \nCajeroIdentificacion: {5}",
                                    Control.Common.GlobalParameters.Establecimiento,
                                    Control.Common.GlobalParameters.PuntoEmision,
                                    Control.Common.GlobalParameters.IpMaquina,
                                    Control.Common.GlobalParameters.SelectedServerIp,
                                    Control.Common.GlobalParameters.UsuarioNombre,
                                    Control.Common.GlobalParameters.Usuario);

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "validaSaldoPuntos", msj);

                                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                Properties.Settings.Default.MAILERROR_FROM,
                                Properties.Settings.Default.MAILERROR_ALIAS,
                                destinoMail,
                                Properties.Settings.Default.MAILERROR_CC,
                                "POS - SALDO DINE ELECT INCONSISTENTE",
                                msj,
                                false,
                                String.Empty);

                                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos", "validaSaldoPuntos", "No se pudo enviar email de alerta durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                                }


                                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[clienteAx]", valor = clienteAx });
                                Control.Common.General.GetMensajeToList(215, parametros);


                                // Control.Common.WinForm.ShowMessage("El cliente: '" + clienteAx + "' en su historial de consumos y acumulacion de cashback presenta datos incorrectos. Contacte a administrador ahora!");
                                validaSaldo = false;
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "validaSaldoPuntos", "CI:" + clienteAx + ", saldo de puntos TblPuntosCab y TblPuntos estan correctos" + saldoPuntos.ToString());
                                validaSaldo = true;
                            }
                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "validaSaldoPuntos", "CI:" + clienteAx + ", saldo de puntos TblPuntosCab:" + saldoPuntos.ToString());//stefany

                            string destinoMail = "devteam@liris.com.ec";
                            string msj = String.Format("No existe informacion en TblPuntosCab, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nIpServidor: {3} \nCajeroNombre: {4} \nCajeroIdentificacion: {5}",
                                Control.Common.GlobalParameters.Establecimiento,
                                Control.Common.GlobalParameters.PuntoEmision,
                                Control.Common.GlobalParameters.IpMaquina,
                                Control.Common.GlobalParameters.SelectedServerIp,
                                Control.Common.GlobalParameters.UsuarioNombre,
                                Control.Common.GlobalParameters.Usuario);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "validaSaldoPuntos", msj);

                            var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                            Properties.Settings.Default.MAILERROR_FROM,
                            Properties.Settings.Default.MAILERROR_ALIAS,
                            destinoMail,
                            Properties.Settings.Default.MAILERROR_CC,
                            "POS - SALDO DINE ELECT INCONSISTENTE",
                            msj,
                            false,
                            String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de alerta durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }

                            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[clienteAx]", valor = clienteAx });
                            Control.Common.General.GetMensajeToList(216, parametros);

                            //Control.Common.WinForm.ShowMessage("El cliente: '" + clienteAx + "' tiene valores alterados,con respecto a su Monedero electronico. Contacte a administrador ahora!");
                            validaSaldo = false;
                        }

                    }
                }
            }
            return validaSaldo;
        }
        private bool ValidaPagoTotalPromoBines(decimal pago)
        {
            bool valida = true;
            try
            {
                if (_factura.AplicaDescuentoPromoBines)
                {
                    if (_factura.GetTotal() != pago)
                    {

                        valida = false;
                        var result = Control.Common.General.GetMensajeToList(206);

                        if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                        {
                            //((MainWindow)this.ParentForm).QuitarDescuentoPromocionTarjetaBines();
                            ((MainWindow)this.Owner).QuitarDescuentoPromocionTarjetaBines();
                            valida = true;
                        }
                        else
                        {
                            // If 'No', do something here.
                            valida = false;

                        }

                        // MessageBox.show("Tarjeta no coincide con la utilizada anteriormente para aplicar promoción");

                    }
                }
            }
            catch (Exception)
            {
                valida = false;
            }

            return valida;
        }

        private void guardarPagoManualPosVoucher(string bin_red)
        {
            string codigoBin = this._numTarjeta.Substring(0, 6);
            try
            {
                var pos = new POSEntities();

                if (pos.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                {
                    //string bin_descripcion = "";

                    Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                    Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
                    string valpag = Decimal.Round(Decimal.Parse(txtValor.Text) * 1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";

                    trama.codRed = bin_red;
                    switch (bin_red)
                    {
                        case "1":
                            trama.MID = Control.Common.GlobalParameters.MID_DATAFAST;
                            trama.TID = Control.Common.GlobalParameters.TID_DATAFAST;

                            //trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                            //trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                            break;
                        case "2":
                            trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                            trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;

                            //trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                            //trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                            break;
                        default:
                            ResponseBackground = Common.GlobalParameters.PinpadMsjAutorizadorNoValido;
                            break;
                    }

                    //bin_descripcion = ctb.bin_descripcion; 

                    //  MessageBox.Show(this,"Verificar el calculo para  armar los totales");
                    var porc_iva = Control.Common.GlobalParameters.IVAGEN / 100;
                    var PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;// Decimal.Parse((pos.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));

                    var porc_pago = (decimal.Parse(txtValor.Text) / _factura.GetTotal());
                    //var porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(),2);
                    decimal porc_Desc2 = 0;
                    if (_facturaApp == null)
                    {
                        porc_Desc2 = 0;
                    }
                    else
                        if (_facturaApp.Descuentos2.Count > 0)
                        {
                            porc_Desc2 = _facturaApp.Descuentos2.Max(x => x.Porcentaje);
                        }
                    var base0 = _factura.GetBase0() - (_factura.GetDescuentos() - (_factura.GetBase12() - _factura.GetBase12Desc()));
                    base0 = base0 - (base0 * (porc_Desc2 / 100));
                    var base12 = _factura.GetBase12DescPromoIVA(false);
                    base12 = base12 - (base12 * (porc_Desc2 / 100));
                    var base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                    if (_factura.GetPromoIva() > 0)
                    {
                        base12 = base12 - ((base12 * PorcPromo) / 100);
                    }
                    base12 += base12PromoIVAExcluye;
                    var iva = decimal.Round(base12 * porc_iva, 2);
                    trama.montoTotalTransaccion = valpag;//12N 10N2D

                    if (decimal.Parse(txtValor.Text) == _factura.GetTotal())
                    {
                        trama.montoBaseGravaIVa = decimal.Round((base12 * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = decimal.Round(((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0) * porc_pago), 2, MidpointRounding.ToEven).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = decimal.Round((iva * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                    }
                    else
                    {

                        base12 = Decimal.Round(base12 * porc_pago, 2);
                        iva = decimal.Round(base12 * porc_iva, 2);
                        base0 = decimal.Parse(txtValor.Text) - base12 - iva;

                        if (base0 < decimal.Parse("0"))
                        {
                            porc_pago = TruncateDecimal(decimal.Parse(txtValor.Text) / _factura.GetTotal(), 2);
                            base12 = _factura.GetBase12DescPromoIVA(false);
                            base12PromoIVAExcluye = _factura.GetBase12DescPromoIVA(true);
                            if (_factura.GetPromoIva() > 0)
                            {
                                base12 = base12 - ((base12 * PorcPromo) / 100);
                            }
                            base12 += base12PromoIVAExcluye;
                            iva = decimal.Round(base12 * porc_iva, 2);

                            base12 = base12 * porc_pago;
                            iva = base12 * porc_iva;
                            base0 = decimal.Parse(txtValor.Text) - base12 - iva;
                        }

                        trama.montoBaseGravaIVa = (base12).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0'); //12N 10N2D
                        trama.montoBaseNoGravaIVa = ((base0 < decimal.Parse("0") ? decimal.Parse("0") : base0)).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D
                        trama.impuestoIvaTransaccion = (iva).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(12, '0');//12N 10N2D              
                    }

                    trama.impuestoServicioTransaccion = "";//12N 10N2D
                    trama.popinaTransaccion = "";//12N 10N2D
                    trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                    trama.secuencialTransaccion = txtNumTransaccionVoucher.Text;  // "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                    trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                    trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd");//AAAAMMDD
                    trama.numAutorizacion = txtNumAutorizacionVoucher.Text; // "";//6N  solo anulaciones envia autorizacion compra original / resto blancos

                    //trama.CID = _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 
                    trama.CID = "LIRISCID0" + _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 

                    trama.plazoDiferido = cmbDiferido.Text;  //"0";
                    trama.mesGracia = cmbMesesGracia.Text; //"0";                 
                    trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipo.SelectedValue).idTipo;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", "Respuesta requerimiento PINPAD recibida: ");// + pinpadResponse);

                    //revisar
                    string codigoproceso = "000200";

                    POS_VOUCHER pos_voucher = new POS_VOUCHER();
                    bool fueProcesadoImpresionVocuhers = false;
                    string printWarnings = string.Empty;
                    string scriptInsertarVoucher = string.Empty;
                    string tipoConsumo = "";

                    try
                    {
                        if (trama.codRed == "2")
                            tipoConsumo = ((pos_tarjeta_tipopago)cmbTipoPago.SelectedValue).tipoConsumo;
                        else
                            tipoConsumo = ((pos_tarjeta_tipopago)cmbTipoPago.SelectedValue).tipoConsumoData;

                        //Agregar el nro del comprobante de la factura para que se pueda incluir en la impresion
                        trama.FacturaComprobante = _factura.GetNumeroFacturaEnmascarado();

                        scriptInsertarVoucher += "SCRIPT PARA INSERTAR:" + Environment.NewLine;
                        scriptInsertarVoucher += "INSERT INTO dbo.POS_VOUCHER(TARJETA,CODIGOPROCESO,FECHACONSUMO,HORACONSUMO,NUMEROVOUCHER,AUTORIZACION,VALORCONSUMO,FORMAAUTORIZA,TIPOCONSUMO,PLAZO,TIPOLECTURA,TIPOMONEDA,VALORIVA,VALORSERVICIO,VALORPROPINA,VALORINTERES,VALORFIJO,TIPOPROMOCION,MESESGRACIA,EMPRESASERVICIO,ESTADOTRX,CODIGORESPUESTA,TIPODISPOSITIVO,ADQUIRENTETARJETA,ADQUIRENTESERVICIO,MONTOGRAVAIVA,MONTONOGRAVAIVA,PUNTOEMISION,PROCESADO,GRUPOTAR,AUTORIZADOR,LOTE,ANULADO,PROCESADOTURNO,FACTURA,ARQC,AIDEMV,EMV,TC,PUBLICIDAD,TIPOTRANSACCION,BANCOADQUIRIENTE,TARJETAHABIENTE,MID,TID,VENCTAR,ANULAUTORIZACION,TIPOBANCOTARJETA) " + Environment.NewLine;
                        scriptInsertarVoucher += "VALUES('" +
                        this._numTarjeta.Trim().PadRight(19, ' ').Substring(0, 19) + "'," +
                        "'" + codigoproceso + "'," +
                        "'" + trama.fechaTransaccion + "'," +
                        "'" + trama.horaTransccion + "'," +
                        "'" + trama.secuencialTransaccion + "'," +  // "'000000'," + 
                        "'" + trama.numAutorizacion + "'," +  // "'000000'," +   //AUTORIZACION
                        "'" + trama.montoTotalTransaccion.PadLeft(13, '0') + "'," +
                        "'1'," +
                        "'" + tipoConsumo + "'," +    // "'PE'," + 
                        "'" + trama.plazoDiferido.PadLeft(2, '0') + "'," +
                        "'000'," + //resptrama.modoLectura.PadLeft(3, '0') + "'," +
                        "'840'," +                  //TIPOMONEDA
                        "'" + trama.impuestoIvaTransaccion.PadLeft(13, '0') + "'," +
                        "'0000000000000'," +
                        "'" + trama.popinaTransaccion.PadLeft(13, '0') + "'," +
                        "'0000000000000'," +//trama.impuestoIvaTransaccion.PadLeft(13, '0') + "'," +
                        "'" + trama.montoFijo.Trim().PadLeft(13, '0') + "'," +
                        "'00'," +
                        "'" + trama.mesGracia.PadLeft(2, '0') + "'," +
                        "'0000'," +
                        //"'" + (tipoConsumo == "01" ? "O" : "R") + "'," +
                        "'" + (tipoConsumo == "01" ? "O" : "O") + "'," +
                        "'0'," + //resptrama.codigoRespuesta + "'," +
                        "'2'," +
                        "'CREDIMATIC01'," +
                        "'            '," +
                        "'" + trama.montoBaseGravaIVa.PadLeft(13, '0') + "'," +
                        "'" + trama.montoBaseNoGravaIVa.PadLeft(13, '0') + "'," +
                        "'" + _factura.Establecimiento + _factura.PtoEmision + "'," +
                        "'1'," +                              //PROCESADO
                        "'" + cmbBancoTarjeta.Text.Trim().PadRight(50, ' ').Substring(0, 50) + "'," +//"'" + resptrama.nomGruTar + "'," +
                        bin_red + "," +//resptrama.codigoRed + "," +
                        "'" + (!string.IsNullOrEmpty(txtLote.Text) ? txtLote.Text : "000000") + "'," +//"'000000'," +//"'" + resptrama.numerolote + "'," +
                        "'0'," +  //(trama.TipoTransaccion == "03" ? "1" : "0") + "," +
                        "'1'," +
                        "'" + _factura.GetNumeroFactura() + "'," +
                        "'            '," +//"'" + resptrama.ARQC + "'," +
                        "'            '," +//"'" + resptrama.AIDEMV + "'," +
                        "'" + cmbBancoTarjeta.Text.Trim().PadRight(20, ' ').Substring(0, 20) + "'," + //cmbTipoTransaccion.Text.Trim().PadRight(20, ' ').Substring(0, 20) + "'," +//"'" + resptrama.idEMV + "'," +
                        "'            '," +//"'" + resptrama.tipoCritoyValorEMV + "'," +
                        "'            '," +//"'" + resptrama.mensajePremioPublicidad + "'," +
                        "'" + trama.TipoTransaccion + "'," +  //"'01'," +
                        "'            '," +//"'" + resptrama.nomBancoAdq + "'," +
                        "'" + this._tarjetaHabiente.Trim().PadRight(40, ' ').Substring(0, 40) + "'," +//"'" + resptrama.nombreTarjetaHabiente + "'," +
                        "'" + trama.MID + "'," +
                        "'" + (cmbTID.Text != "" ? cmbTID.Text : trama.TID) + "'," +
                        "'XX/XXXX'," +//"'" + (bin_red == "2" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") + "'," +
                        "'" + trama.numAutorizacion + "'," +
                        "'" + (cmbTipoPago.Text.Length > 25 ? cmbTipoPago.Text.Substring(0, 25) : cmbTipoPago.Text) + "'" + // "'CORRIENTE'" + //cmbBancoTarjeta.Text.Trim().PadRight(25, ' ').Substring(0, 25) + "'" +
                        ")";


                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpad", scriptInsertarVoucher);

                        Decimal valor = Decimal.Parse(txtValor.Text);

                        fueProcesadoImpresionVocuhers = true;

                        //grabar transaccion en tabla pos_voucher
                        //cambiar aqui

                        pos_voucher.TARJETA = this._numTarjeta.Trim().PadRight(19, ' ').Substring(0, 19);// ("520081XXXXXX6017   "); //19 ;

                        //es 003000 cuando es transacciones con tarjeta de crédito, 001000 cuando es transacción de tarjeta de debito cuenta de ahorro y 002000 cuando es transacción de tarjeta de debito cuenta corriente.
                        pos_voucher.CODIGOPROCESO = codigoproceso;// ("000200"); //6 ;
                                                                  //revisar

                        pos_voucher.FECHACONSUMO = trama.fechaTransaccion;// ("20161122"); //8 ;
                        pos_voucher.HORACONSUMO = trama.horaTransccion;// ("114339"); //6 ;
                        pos_voucher.NUMEROVOUCHER = trama.secuencialTransaccion; // "000000";// ("000002");//6 ;
                        pos_voucher.AUTORIZACION = trama.numAutorizacion;  // "000000"; //6 ;
                        pos_voucher.VALORCONSUMO = trama.montoTotalTransaccion.PadLeft(13, '0');// ("0000000001200"); //13 ;
                        pos_voucher.FORMAAUTORIZA = ("1"); //1 ;
                        pos_voucher.TIPOCONSUMO = tipoConsumo; // "PE";
                        pos_voucher.PLAZO = trama.plazoDiferido.PadLeft(2, '0');// ("06"); //2 ;
                        pos_voucher.TIPOLECTURA = "000";// resptrama.modoLectura.PadLeft(3, '0');// ("005"); //3 ;
                        pos_voucher.TIPOMONEDA = ("840"); //3 ;
                        pos_voucher.VALORIVA = trama.impuestoIvaTransaccion.PadLeft(13, '0');// ("0000000000147"); //13 ;
                        pos_voucher.VALORSERVICIO = ("0000000000000"); //13 ;
                        pos_voucher.VALORPROPINA = trama.popinaTransaccion.PadLeft(13, '0');// ("0000000000000"); //13 ;
                        pos_voucher.VALORINTERES = ("0000000000000"); ;//trama.impuestoIvaTransaccion.PadLeft(13, '0');//resptrama.valInteres.Trim().PadLeft(13, '0');// ("0000000000057");  //13 ;
                        pos_voucher.VALORFIJO = trama.montoFijo.Trim().PadLeft(13, '0');// ("0000000000000");  //13 ;
                        pos_voucher.TIPOPROMOCION = ("00"); //2 ;
                        pos_voucher.MESESGRACIA = trama.mesGracia.PadLeft(2, '0');//                        ("00");  //2 ;
                        pos_voucher.EMPRESASERVICIO = ("0000");  //4 ;
                        //pos_voucher.ESTADOTRX = (tipoConsumo == "01" ? "O" : "R"); // (pos_voucher.TIPOCONSUMO == "01" ? "O" : "R"); //1 ;
                        pos_voucher.ESTADOTRX = (tipoConsumo == "01" ? "O" : "O"); //JCanarte
                        pos_voucher.CODIGORESPUESTA = "00";//resptrama.codigoRespuesta;// ("00"); //2 ;
                        pos_voucher.TIPODISPOSITIVO = ("2"); //1 ;
                        pos_voucher.ADQUIRENTETARJETA = ("CREDIMATIC01"); //12 ;
                        pos_voucher.ADQUIRENTESERVICIO = ("            ");  //12 ;
                        pos_voucher.MONTOGRAVAIVA = trama.montoBaseGravaIVa.PadLeft(13, '0');// ("0000000001053"); //13 ;
                        pos_voucher.MONTONOGRAVAIVA = trama.montoBaseNoGravaIVa.PadLeft(13, '0');// ("0000000000000");  //13 ;
                        pos_voucher.PUNTOEMISION = _factura.Establecimiento + _factura.PtoEmision;
                        pos_voucher.PROCESADO = true;
                        pos_voucher.GRUPOTAR = cmbBancoTarjeta.Text.Trim().PadRight(50, ' ').Substring(0, 50);// resptrama.nomGruTar;
                        pos_voucher.AUTORIZADOR = int.Parse(bin_red);// int.Parse(resptrama.codigoRed);
                        pos_voucher.ANULADO = false;
                        pos_voucher.LOTE = (!string.IsNullOrEmpty(txtLote.Text) ? txtLote.Text : "000000").PadLeft(6, '0'); //"000000"; //resptrama.numerolote;
                        pos_voucher.PROCESADOTURNO = true;
                        pos_voucher.FACTURA = _factura.GetNumeroFactura();
                        //ML [16/01/2018]: Nuevos campos
                        pos_voucher.ARQC = "            "; //resptrama.ARQC;
                        pos_voucher.AIDEMV = "            ";// resptrama.AIDEMV;
                        pos_voucher.EMV = cmbBancoTarjeta.Text.Trim().PadRight(20, ' ').Substring(0, 20); //cmbTipoTransaccion.Text.Trim().PadRight(20, ' ').Substring(0, 20);// resptrama.idEMV;
                        pos_voucher.TC = "            ";// resptrama.tipoCritoyValorEMV;
                        pos_voucher.PUBLICIDAD = "            ";// resptrama.mensajePremioPublicidad;
                        pos_voucher.TIPOTRANSACCION = trama.TipoTransaccion;  // "01";
                        pos_voucher.BANCOADQUIRIENTE = "PAGO MANUAL";// resptrama.nomBancoAdq;
                        pos_voucher.TARJETAHABIENTE = this._tarjetaHabiente.Trim().PadRight(40, ' ').Substring(0, 40);// resptrama.nombreTarjetaHabiente;
                        pos_voucher.MID = trama.MID;
                        pos_voucher.TID = (cmbTID.Text != "" ? cmbTID.Text : trama.TID);  //trama.TID;
                        pos_voucher.VENCTAR = "XX/XX";// resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX";
                        pos_voucher.ANULAUTORIZACION = trama.numAutorizacion;
                        pos_voucher.TIPOBANCOTARJETA = (cmbTipoPago.Text.Length > 25 ? cmbTipoPago.Text.Substring(0, 25) : cmbTipoPago.Text); // "CORRIENTE";    //  (cmbBancoTarjeta.Text.Length > 25 ? cmbBancoTarjeta.Text.Substring(0, 25) : cmbBancoTarjeta.Text);

                        //------------------------------
                        bool ExisteVoucher = pos.POS_VOUCHER.Any(x => x.FECHACONSUMO == pos_voucher.FECHACONSUMO && x.VALORCONSUMO == pos_voucher.VALORCONSUMO && x.AUTORIZACION == pos_voucher.AUTORIZACION && x.NUMEROVOUCHER == pos_voucher.NUMEROVOUCHER && x.FACTURA == pos_voucher.FACTURA);
                        if (!ExisteVoucher)
                        {
                            pos.POS_VOUCHER.Add(pos_voucher);
                        }
                        // pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet += 1;
                        pos.SaveChanges();
                    }
                    catch (Exception ex)
                    {


                        string datosVoucher = "\nTARJETA: " + this._numTarjeta.Trim().PadRight(19, ' ').Substring(0, 19) +
                                                "\nCODIGOPROCESO: 000200" +
                                                "\nFECHACONSUMO: " + trama.fechaTransaccion +
                                                "\nHORACONSUMO: " + trama.horaTransccion +
                                                "\nNUMEROVOUCHER: " + trama.secuencialTransaccion + // "000000" +
                                                "\nAUTORIZACION: " + trama.numAutorizacion +  // "000000" + //(trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) +

                                                "\nVALORCONSUMO: " + trama.montoTotalTransaccion.PadLeft(13, '0') +
                                                "\nFORMAAUTORIZA: 1" +
                                                "\nTIPOCONSUMO: " + tipoConsumo + //PE" +                                                 
                                                "\nPLAZO: " + trama.plazoDiferido.PadLeft(2, '0') +
                                                "\nTIPOLECTURA: 000" + //resptrama.modoLectura.PadLeft(3, '0') +
                                                "\nTIPOMONEDA: 840" +
                                                "\nVALORIVA: " + trama.impuestoIvaTransaccion.PadLeft(13, '0') +
                                                "\nVALORSERVICIO: 0000000000000" +
                                                "\nVALORPROPINA: " + trama.popinaTransaccion.PadLeft(13, '0') +
                                                "\nVALORINTERES: 0000000000000" + //trama.impuestoIvaTransaccion.PadLeft(13, '0') +//resptrama.valInteres.Trim().PadLeft(13, '0') +
                                                "\nVALORFIJO: " + trama.montoFijo.Trim().PadLeft(13, '0') + //resptrama.montoFijo.Trim().PadLeft(13, '0') +
                                                "\nTIPOPROMOCION: 00" +
                                                "\nMESESGRACIA: " + trama.mesGracia.PadLeft(2, '0') +
                                                "\nEMPRESASERVICIO: 0000" +
                                                //"\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "R") +
                                                "\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "O") +  //JCanarte
                                                "\nCODIGORESPUESTA: 00" + //resptrama.codigoRespuesta +
                                                "\nTIPODISPOSITIVO: 2" +
                                                "\nADQUIRENTETARJETA: CREDIMATIC01" +
                                                "\nADQUIRENTESERVICIO:             " +
                                                "\nMONTOGRAVAIVA: " + trama.montoBaseGravaIVa.PadLeft(13, '0') +
                                                "\nMONTONOGRAVAIVA: " + trama.montoBaseNoGravaIVa.PadLeft(13, '0') +
                                                "\nPUNTOEMISION: " + _factura.Establecimiento + _factura.PtoEmision +
                                                "\nPROCESADO: 1" +
                                                "\nGRUPOTAR: " + cmbBancoTarjeta.Text.Trim().PadRight(50, ' ').Substring(0, 50) + //resptrama.nomGruTar +
                                                "\nAUTORIZADOR: " + bin_red +
                                                "\nANULADO: " + "0" +//(trama.TipoTransaccion == "03" ? "1" : "0") +
                                                "\nLOTE: " + (!string.IsNullOrEmpty(txtLote.Text) ? txtLote.Text : "000000") + //000000" + //resptrama.numerolote +
                                                "\nFACTURA: " + _factura.GetNumeroFactura() +
                                                "\nARQC: " + resptrama.ARQC +
                                                "\nAIDEMV: " + resptrama.AIDEMV +
                                                "\nEMV: " + cmbBancoTarjeta.Text.Trim().PadRight(25, ' ').Substring(0, 25) +//cmbTipoTransaccion.Text.Trim().PadRight(20, ' ').Substring(0, 20) + //resptrama.idEMV +
                                                "\nTC: " + //resptrama.tipoCritoyValorEMV +
                                                "\nPUBLICIDAD: " + //resptrama.mensajePremioPublicidad +
                                                "\nTIPOTRANSACCION: " + trama.TipoTransaccion + // 01" + 
                                                "\nBANCOADQUIRIENTE: " + //resptrama.nomBancoAdq +
                                                "\nTARJETAHABIENTE: " + this._tarjetaHabiente.Trim().PadRight(40, ' ').Substring(0, 40) + //resptrama.nombreTarjetaHabiente +
                                                "\nMID: " + trama.MID +
                                                "\nTID: " + (cmbTID.Text != "" ? cmbTID.Text : trama.TID) + //trama.TID +
                                                "\nVENCTAR: XX/XX" + //(resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") +
                                                "\nANULAUTORIZACION: " + trama.numAutorizacion +
                                                "\nTIPOBANCOTARJETA: " + (cmbTipoPago.Text.Length > 25 ? cmbTipoPago.Text.Substring(0, 25) : cmbTipoPago.Text);  // CORRIENTE";// + cmbBancoTarjeta.Text.Trim().PadRight(25, ' ').Substring(0, 25); //cmbBancoTarjeta.Text;

                        datosVoucher += Environment.NewLine + Environment.NewLine + scriptInsertarVoucher;




                        var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                            Properties.Settings.Default.MAILERROR_FROM,
                            Properties.Settings.Default.MAILERROR_ALIAS,
                            Properties.Settings.Default.MAILERROR_DESTINO,
                            Properties.Settings.Default.MAILERROR_CC,
                            "Voucher pinpad no se grabó en nuestra base interna",
                            String.Format("  \n\nDatos caja-----------------" +
                                            "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                            "  \n\nDatos voucher-----------------" +
                                            datosVoucher,
                                        Control.Common.GlobalParameters.Establecimiento,
                                        Control.Common.GlobalParameters.PuntoEmision,
                                        Control.Common.GlobalParameters.IpMaquina,
                                        Control.Common.GlobalParameters.UsuarioNombre,
                                        Control.Common.GlobalParameters.Usuario,
                                        Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                        (fueProcesadoImpresionVocuhers ? "" : ("StackTrace: " + ex.StackTrace + " \n"))),
                            false,
                            String.Empty);

                        if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "No se pudo enviar notificacion del problema al grabar voucher en nuestra base interna, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                        }

                    }

                    //Agregar lineas de insert
                    Control.Common.Logger.Agregar_Trace_Voucher(pos_voucher);

                    DebeCerrarFormBackground = true;


                }
            }
            catch (Exception ex)
            {
                ResponseBackground = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a una breve interrupcion en el servicio, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco";
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "La solicitud no pudo ser realizada debido a una breve interrupcion en el servicio, esto puede deberse a un breve mantenimiento, por favor antes de volverlo a intentar verifique que transaccion no haya sido cargada al cliente en el banco. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
            }
            finally
            {
                On_Off_Controles(true);
            }
        }
        public decimal TruncateDecimal(decimal value, int precision)
        {
            decimal step = (decimal)Math.Pow(10, precision);
            decimal tmp = Math.Truncate(step * value);
            return tmp / step;
        }
        private void BasePagos_Load(object sender, EventArgs e)
        {
            Decimal dValorGiftCard = 0;
            string sCuenta = "";


            this.SuspendLayout();
            Cursor.Current = Cursors.WaitCursor;


            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "BasePago_Load", "Ejecuta Load de base apgo");

            try
            {


                boxDiferido.Visible = false;
                EstadoControles = true;

                lblTipoPagoTarjeta.Visible = false;
                cmbTipoTransaccion.Visible = false;
                lblBanco.Visible = false;
                cmbBancoTarjeta.Visible = false;

                lblCuenta.Visible = false;
                lblNum.Visible = false;
                txtCuenta.Visible = false;
                txtNumCheque.Visible = false;
                lblGiftCard.Visible = false;
                lblGiftCardSaldo.Visible = false;
                btnVerificar.Visible = false;
                btnPagoManual.Visible = false;

                if (_pagoTipo == PagoTipo.TarjetaCredito)
                {
                    cmbTipoTransaccion.SelectedIndex = 0;
                    btnPagoManual.Visible = true;

                    lblTipoPagoTarjeta.Visible = true;
                    cmbTipoTransaccion.Visible = true;
                    lblBanco.Visible = true;
                    cmbBancoTarjeta.Visible = true;
                    txtValor.Focus();

                    Control.Common.General.ValidaContingente();

                    using (var db = new POSEntities())
                    {
                        PUERTOCOM = db.core_puntoemision.Where(x => x.establecimiento_id == this._factura.Establecimiento).FirstOrDefault().puerto_pinpad;

                        if ((db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE"))
                        {
                            lblTipoPagoTarjeta.Text = "Tipo";
                            lblTipoPagoTarjeta.Left = 12;
                            lblTipoPagoTarjeta.Top = 67;
                            cmbTipoTransaccion.Left = 172;
                            cmbTipoTransaccion.Top = 60;

                            lblBanco.Text = "Transacción";
                            lblBanco.Left = 12;
                            lblBanco.Top = 120;

                            cmbBancoTarjeta.Left = 172;
                            cmbBancoTarjeta.Top = 118;

                            //cmbTipoTransaccion.Visible = true;
                            cmbTipoTransaccion.DataSource = db.pos_tarjeta_transaccion.Where(x => x.estado == true).ToList();
                            cmbTipoTransaccion.DataMember = "idTipo";
                            cmbTipoTransaccion.DisplayMember = "NombreTarjetaTransaccion";
                            //   boxDiferido.Visible = true;
                        }
                        else
                        {
                            boxDiferido.Visible = false;
                            btnPagoManual.Visible = false;
                            cmbBancoTarjeta.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
                            //   cmbBancoTarjeta.DataSource = db.core_tarjetacredito.ToList();
                            cmbBancoTarjeta.DisplayMember = "nombre_completo";

                            //    cmbBancoTarjeta.ValueMember = "operador";
                            //cmbTipoTransaccion.Visible = false;
                        }
                    }

                    if (_factura.Pagos.Any(x => x.Descripcion == "T. CREDITO"))
                    {
                        gridPagos.DataSource = _factura.Pagos.First(x => x.Descripcion == "T. CREDITO").Pagos;
                    }
                }
                if (_pagoTipo == PagoTipo.Cheque)
                {
                    using (var db = new POSEntities())
                    {
                        cmbBancoTarjeta.DataSource = db.core_banco.ToList();
                        cmbBancoTarjeta.DisplayMember = "nombre";
                    }

                    if (_factura.Pagos.Any(x => x.Descripcion == "CHEQUE"))
                    {
                        gridPagos.DataSource = _factura.Pagos.First(x => x.Descripcion == "CHEQUE").Pagos;
                    }
                }
                if (_pagoTipo == PagoTipo.TarjetaRegalo)
                {
                    txtCuenta.Visible = true;
                    lblCuenta.Visible = true;
                    btnVerificar.Visible = true;

                    txtCuenta.Text = "";
                    txtCuenta.NullText = "";
                    txtCuenta.Enabled = false;
                    txtCuenta.BackColor = System.Drawing.Color.White;

                    // ── CAMPO OCULTO que recibe el pistolero ────────────────
                    TextBox txtCaptura = new TextBox();
                    txtCaptura.Name = "txtCapturaPistolero";
                    txtCaptura.Size = new Size(0, 0);
                    txtCaptura.Location = new Point(0, 0);
                    txtCaptura.TabStop = false;
                    splitContainer1.Panel1.Controls.Add(txtCaptura);

                    txtCaptura.KeyPress += (s, ev) =>
                    {
                        // 1. VALIDAR: GiftCard solo acepta dígitos
                        if (!char.IsControl(ev.KeyChar) && !char.IsDigit(ev.KeyChar) && ev.KeyChar != '-')
                        {
                            ev.Handled = true; // Bloquear letras, guiones, cualquier otro caracter
                            return;
                        }

                        // 2. ENTER = fin de lectura
                        if (ev.KeyChar == (char)Keys.Enter)
                        {
                            // Mostrar asteriscos en pantalla (el cajero no ve el código real)
                            txtCuenta.Text = new string('*', txtCaptura.Text.Length);

                            // Guardar el código real en las variables
                            _textoOriginal = txtCaptura.Text;
                            ValorOriginalTarjeta = txtCaptura.Text;

                            txtCaptura.Text = string.Empty; // Limpiar para próximo uso
                            btnVerificar.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    // ── Garantizar foco en campo oculto ─────────────────────
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        txtCaptura.Focus();
                    });

                    lblGiftCard.Visible = true;
                    lblGiftCardSaldo.Visible = true;
                    lblGiftCard.Text = "Saldo Gift Card:";

                    Label nuevoLabelGiftCard = new Label();
                    nuevoLabelGiftCard.Text = "Por favor, pistolee el código de la GiftCard";
                    nuevoLabelGiftCard.Location = new Point(170, 170);
                    nuevoLabelGiftCard.Visible = true;
                    nuevoLabelGiftCard.Font = new Font("Arial", 12, FontStyle.Bold);
                    nuevoLabelGiftCard.ForeColor = Color.Red;
                    nuevoLabelGiftCard.Size = new Size(255, 60);
                    nuevoLabelGiftCard.Name = "lblError";
                    splitContainer1.Panel1.Controls.Add(nuevoLabelGiftCard);

                    if (_factura.Pagos.Any(x => x.Descripcion == "GIFT CARD" || x.Descripcion == "GIFT CARDV"))
                    {
                        gridPagos.DataSource = _factura.Pagos
                            .First(x => x.Descripcion == "GIFT CARD" || x.Descripcion == "GIFT CARDV").Pagos;
                    }

                    var t = new TarjetaRegalo();
                }
                if (_pagoTipo == PagoTipo.NotaCredito)
                {
                    txtCuenta.Visible = true;
                    lblCuenta.Visible = true;
                    btnVerificar.Visible = true;

                    txtCuenta.Text = "";
                    txtCuenta.NullText = "";
                    //txtCuenta.PasswordChar = '*';

                    txtCuenta.TabStop = true;


                    MethodInvoker actionNC = delegate { txtCuenta.Focus(); };
                    this.BeginInvoke(actionNC);


                    Label nuevoLabelNC = new Label();
                    nuevoLabelNC.Text = "Por favor, pistolee el código de la GiftCard /Nota de Crédito. ";
                    nuevoLabelNC.Location = new Point(170, 170); // Posición en el formulario
                    nuevoLabelNC.Visible = true;
                    nuevoLabelNC.Font = new Font("Arial", 12, FontStyle.Bold); // Tamaño 14, negrita
                    nuevoLabelNC.ForeColor = Color.Red;
                    nuevoLabelNC.Size = new Size(255, 60);
                    nuevoLabelNC.Name = "lblError";
                    splitContainer1.Panel1.Controls.Add(nuevoLabelNC);

                    lblGiftCard.Text = "Saldo Nota Credito:";
                    txtValor.Enabled = false;
                    cmbBancoTarjeta.Enabled = false;

                    if (_factura.Pagos.Any(x => x.Descripcion == "ANTCLIEN/C"))
                    {
                        gridPagos.DataSource = _factura.Pagos.First(x => x.Descripcion == "ANTCLIEN/C").Pagos;
                    }
                    txtCuenta.Focus();
                }
                if (_pagoTipo == PagoTipo.TarjetaInterna)
                {
                    cmbBancoTarjeta.Enabled = false;
                    txtCuenta.Visible = true;
                    lblCuenta.Visible = true;
                    btnVerificar.Visible = true;



                    lblGiftCard.Visible = true;
                    lblGiftCardSaldo.Visible = true;

                    lblGiftCard.Text = "Saldo Tarjeta";

                    decimal saldo = 0;
                    if (_tarjetaInternaAdicional == null && _tarjetaInterna == null)
                    {
                        saldo = 0;
                    }
                    else
                    {
                        saldo = (_tarjetaInternaAdicional == null) ? decimal.Parse(_tarjetaInterna.saldo.ToString("N2")) : decimal.Parse(_tarjetaInternaAdicional.saldo.ToString("N2"));
                    }

                    lblGiftCardSaldo.Text = saldo.ToString("N2");

                    if (_tarjetaInterna != null)
                    {
                        if (_factura.Pagos.Any(x => x.Descripcion == "TAR PORTAL"))
                        {
                            gridPagos.DataSource = _factura.Pagos.First(x => x.Descripcion == "TAR PORTAL").Pagos;
                        }
                    }

                    Label nuevoLabel = new Label();
                    nuevoLabel.Text = "Por favor, deslice la tarjeta empresarial";
                    nuevoLabel.Location = new Point(170, 170); // Posición en el formulario
                    nuevoLabel.Visible = true;
                    nuevoLabel.Font = new Font("Arial", 12, FontStyle.Bold); // Tamaño 14, negrita
                    nuevoLabel.ForeColor = Color.Red;
                    nuevoLabel.Size = new Size(255, 60);
                    nuevoLabel.Name = "lblError";
                    splitContainer1.Panel1.Controls.Add(nuevoLabel);


                    //txtCuenta.PasswordChar = '*';
                    txtCuenta.Text = "";
                    txtCuenta.NullText = "";
                    txtCuenta.TabStop = true;

                    MethodInvoker actionTajertaInterna = delegate { txtCuenta.Focus(); };
                    this.BeginInvoke(actionTajertaInterna);

                    //this.ActiveControl = this.txtCuenta;
                }
                if (_pagoTipo == PagoTipo.DineroElectronico)
                {
                    lblGiftCard.Visible = true;
                    lblGiftCard.Text = "Dinero Electrónico:";
                    lblBanco.Visible = false;
                    lblGiftCardSaldo.Visible = true;

                    lblCuenta.Visible = true;
                    txtCuenta.Visible = true;
                    btnVerificar.Visible = true;
                    txtCuenta.Text = "";
                    txtCuenta.NullText = "";

                    // jchid no permita escribir en el campo de texto, solo pistolear el código, para evitar errores de digitación
                    // ── BLOQUEAR txtCuenta visible ──────────────────────────
                    txtCuenta.Enabled = false;
                    txtCuenta.BackColor = System.Drawing.Color.White;

                    // ── CAMPO OCULTO que recibe el pistolero ────────────────
                    TextBox txtCaptura = new TextBox();
                    txtCaptura.Name = "txtCapturaPistolero";
                    txtCaptura.Size = new Size(0, 0);
                    txtCaptura.Location = new Point(0, 0);
                    txtCaptura.TabStop = false;
                    splitContainer1.Panel1.Controls.Add(txtCaptura);

                    // ── Cuando el pistolero termina (Enter) → pasar a txtCuenta
                    txtCaptura.KeyPress += (s, ev) =>
                    {
                        // 1. VALIDAR caracteres permitidos (solo dígitos y guion)
                        if (!char.IsControl(ev.KeyChar) &&
                            !char.IsDigit(ev.KeyChar) &&
                            ev.KeyChar != '-')
                        {
                            ev.Handled = true; // Bloquear carácter inválido
                            return;
                        }

                        // 2. ENTER = fin de lectura del pistoleador
                        if (ev.KeyChar == (char)Keys.Enter)
                        {
                            txtCuenta.Text = new string('*', txtCaptura.Text.Length);
                            _textoOriginal = txtCaptura.Text;
                            ValorOriginalTarjeta = txtCaptura.Text;
                            txtCaptura.Text = string.Empty;
                            btnVerificar.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    // ── Garantizar foco en campo oculto ─────────────────────
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        txtCaptura.Focus();
                    });


                    //// Garantizamos el foco después de que todo esté listo
                    //this.BeginInvoke((MethodInvoker)delegate {
                    //    txtCuenta.TabStop = true;
                    //    txtCuenta.Focus();
                    //});


                    Label nuevoLabelClteApp = new Label();
                    nuevoLabelClteApp.Text = "Por favor, pistolee el código de Cliente App";
                    nuevoLabelClteApp.Location = new Point(170, 170); // Posición en el formulario
                    nuevoLabelClteApp.Visible = true;
                    nuevoLabelClteApp.Font = new Font("Arial", 12, FontStyle.Bold); // Tamaño 14, negrita
                    nuevoLabelClteApp.ForeColor = Color.Red;
                    nuevoLabelClteApp.Size = new Size(255, 60);
                    nuevoLabelClteApp.Name = "lblError";

                    // Añadir el Label al formulario
                    splitContainer1.Panel1.Controls.Add(nuevoLabelClteApp);


                    if (Control.Common.GlobalParameters.MonederoPorcentajeConsumo <= 0)
                    {
                        Common.General.GetMensajeToList(207);
                        return;
                    }


                    if (_factura.Pagos.Any(x => x.Descripcion == "DINE ELECT"))
                    {
                        gridPagos.DataSource = _factura.Pagos.First(x => x.Descripcion == "DINE ELECT").Pagos;
                    }


                }

                // -------------------------------------------------------
                // NUEVO BLOQUE: LOGICA PARA COMPRA GRATIS
                // -------------------------------------------------------
                if (_pagoTipo == PagoTipo.CompraGratis)
                {
                    // 1. Configurar Títulos y Visibilidad
                    lblGiftCard.Visible = true;
                    lblGiftCard.Text = "Compra Gratis"; // Título del campo
                    lblBanco.Visible = false;
                    lblGiftCardSaldo.Visible = false; // En compra gratis no solemos mostrar saldo

                    lblCuenta.Visible = true;
                    txtCuenta.Visible = true;
                    btnVerificar.Visible = true; // Botón para validar el código

                    txtCuenta.Text = "";
                    txtCuenta.NullText = "";

                    // Opcional: Si es una clave secreta, descomenta la siguiente línea:
                    // txtCuenta.PasswordChar = '*'; 

                    // 2. Crear el Label de Instrucciones (letras rojas)
                    Label nuevoLabelGratis = new Label();
                    nuevoLabelGratis.Text = "Por favor, pistolee el código de Cliente App";
                    nuevoLabelGratis.Location = new Point(170, 170);
                    nuevoLabelGratis.Visible = true;
                    nuevoLabelGratis.Font = new Font("Arial", 12, FontStyle.Bold);
                    nuevoLabelGratis.ForeColor = Color.Red;
                    nuevoLabelGratis.Size = new Size(255, 60);
                    nuevoLabelGratis.Name = "lblInstruccionGratis";

                    splitContainer1.Panel1.Controls.Add(nuevoLabelGratis);

                    // 3. Cargar pagos previos si existen (para evitar duplicados visuales)
                    // Asegúrate que el string coincida con lo que guardas en base de datos
                    if (_factura.Pagos.Any(x => x.Descripcion == "COMPRA GRATIS"))
                    {
                        gridPagos.DataSource = _factura.Pagos.First(x => x.Descripcion == "COMPRA GRATIS").Pagos;
                    }

                    // 4. Forzar el foco al campo de texto
                    this.BeginInvoke((MethodInvoker)delegate {
                        txtCuenta.Focus();
                    });
                }


                // ✔️ Garantizar foco al final, cuando todo esté listo
                this.BeginInvoke((MethodInvoker)delegate
                {
                    if (_pagoTipo == PagoTipo.DineroElectronico ||
                         _pagoTipo == PagoTipo.TarjetaRegalo)
                    {
                        return;
                    }

                    if (txtCuenta.Visible)
                    {
                        txtCuenta.Focus();
                        txtCuenta.SelectAll();
                    }
                    else if (_pagoTipo == PagoTipo.TarjetaCredito)
                    {
                        txtValor.Focus();
                    }
                });

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "BasePagos_Load", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.ResumeLayout();

            }


        }



        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridPagos.SelectedRows.Count > 0)
                {

                    //((MainWindow)this.Owner).QuitarDescuentoPromocionTarjetaBines();
                    var pago = gridPagos.SelectedRows[0].DataBoundItem as Models.PagoBase;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "btnEliminar_Click", "Acción eliminar PagoBase solicitada por usuario sobre pago con código: " + pago.Codigo + ", valor: " + pago.Valor.ToString("N2") + ". Factura: " + _factura.GetNumeroFactura());
                    gridPagos.SelectedRows[0].Delete();
                    _factura.CalcularPagos();
                    _factura.AgregaOrdenApp(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnEliminar_Click", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private bool verificaTarjetaInterna(decimal valor = 0)
        {
            bool result = false;
            string NumeroTarjeta = _textoOriginal;
            Screen targetScreen = Control.Common.General.GetScreenCajero();

            string Cedula = string.Empty;

            core_tarjetacreditointerno tarjetaInterna = null;
            core_tarjetacreditointerno tarjetaInternaAdi = null;
            core_tarjetacreditointerno tarjetaFinal = null;
            decimal valorPago = 0;

            try
            {
                if (string.IsNullOrEmpty(NumeroTarjeta))
                {
                    Control.Common.General.GetMensajeToList(634);
                    return false;
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "Inicia verificaTarjetaInterna");

                if (NumeroTarjeta.StartsWith("%"))
                {
                    var tarjeta = System.Text.RegularExpressions.Regex.Match(NumeroTarjeta, @"\%B(\d+)\^").Groups[1].Value;

                    if (string.IsNullOrEmpty(tarjeta))
                        tarjeta = System.Text.RegularExpressions.Regex.Match(NumeroTarjeta, @"\%B(\d+)\&").Groups[1].Value;

                    if (string.IsNullOrEmpty(tarjeta))
                        tarjeta = System.Text.RegularExpressions.Regex.Match(NumeroTarjeta, @"\%B(\d+)\*").Groups[1].Value;

                    // Captura solo dígitos - cubre cualquier carácter especial al final
                    if (string.IsNullOrEmpty(tarjeta))
                        tarjeta = System.Text.RegularExpressions.Regex.Match(NumeroTarjeta, @"\%B(\d+)").Groups[1].Value;

                    NumeroTarjeta = tarjeta.ToUpper();
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "Ejecuta ConsultarTarjetaInternaGen");
                tarjetaInterna = Control.Cliente.ConsultarTarjetaInternaGen(NumeroTarjeta);

                if (tarjetaInterna != null)
                {
                    if (string.IsNullOrEmpty(tarjetaInterna.identificacionPrincipal))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "identificacionPrincipal es NULL, la tarjeta es principal.");

                        if (_factura.ClienteIdentificacion.Trim().ToString() != tarjetaInterna.identificacion.Trim().ToString())
                        {
                            Control.Common.General.GetMensajeToList(690);
                            return result;
                        }

                        if (!tarjetaInterna.activo)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "Tarjeta Principal no activa");
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Control.Common.General.GetMensajeToList(189, targetScreen);
                            });
                            return result;
                        }

                        if (tarjetaInterna.saldo == 0)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "Tarjeta Principal no tiene Saldo");
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Control.Common.General.GetMensajeToList(621, targetScreen);
                            });
                            return result;
                        }

                        if (tarjetaInterna.fecha_expiracion.HasValue)
                        {
                            if (tarjetaInterna.fecha_expiracion < DateTime.Now)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "La tarjeta ha expirado");
                                this.BeginInvoke((MethodInvoker)delegate
                                {
                                    Control.Common.General.GetMensajeToList(562, targetScreen);
                                });
                                return result;
                            }
                        }

                        if (tarjetaInterna.fecha_expiracion == null)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "La tarjeta no tiene fecha de expiración");
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Control.Common.General.GetMensajeToList(563, targetScreen);
                            });
                            return result;
                        }
                    }
                    else
                    {
                        tarjetaInternaAdi = tarjetaInterna;

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "identificacionPrincipal es diferente de NULL, es dependiente o tarjeta adicional.");

                        tarjetaFinal = null;
                        string identificacionPrincipal = tarjetaInterna.identificacionPrincipal;

                        tarjetaInterna = Control.Cliente.ConsultarTarjetaInternaAdicionalGen(identificacionPrincipal);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "identificacionPrincipal es NULL, la tarjeta es principal.");

                        if (!tarjetaInterna.activo)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "Tarjeta Principal no activa");
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Control.Common.General.GetMensajeToList(189, targetScreen);
                            });
                            return result;
                        }

                        if (tarjetaInterna.saldo == 0)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "Tarjeta Principal no tiene Saldo");
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Control.Common.General.GetMensajeToList(621, targetScreen);
                            });
                            return result;
                        }

                        if (tarjetaInterna.fecha_expiracion.HasValue)
                        {
                            if (tarjetaInterna.fecha_expiracion < DateTime.Now)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "La tarjeta ha expirado");
                                this.BeginInvoke((MethodInvoker)delegate
                                {
                                    Control.Common.General.GetMensajeToList(562, targetScreen);
                                });
                                return result;
                            }
                        }

                        if (tarjetaInterna.fecha_expiracion == null)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", "La tarjeta no tiene fecha de expiración");
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Control.Common.General.GetMensajeToList(563, targetScreen);
                            });
                            return result;
                        }
                    }

                    if (tarjetaInterna != null || tarjetaInternaAdi != null)
                    {
                        tarjetaFinal = tarjetaInterna;

                        Cedula = tarjetaFinal.identificacion != null ? tarjetaFinal.identificacion.ToString() : null;

                        valorPago = tarjetaFinal.saldo;
                        if (tarjetaFinal.saldo > decimal.Parse(txtValor.Text))
                        {
                            valorPago = decimal.Parse(txtValor.Text);
                        }

                        lblGiftCard.Text = "Saldo Tarjeta";
                        lblGiftCardSaldo.Text = tarjetaFinal.saldo.ToString("N2");
                        txtValor.Text = valorPago.ToString();

                        _tarjetaInterna = tarjetaInterna;
                        _tarjetaInternaAdicional = tarjetaInternaAdi;
                    }

                    if (_factura.Pagos.Any(x => x.Descripcion == "TAR PORTAL"))
                    {
                        gridPagos.DataSource = _factura.Pagos.First(x => x.Descripcion == "TAR PORTAL").Pagos;
                    }

                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "verificaTarjetaInterna", $"Error: {ex.Message}");
                return result;
            }

            return result;
        }

        private void verificaTarjetaInternaBackGround()
        {

            try
            {
                paneLoading.Visible = true;

                if (bgw == null)
                {
                    bgw = new BackgroundWorker();
                    bgw.DoWork += new DoWorkEventHandler(bgw_DoWorkTarjetaInterna);
                    bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerTarejtaInternaCompleted);
                }

                bgw.WorkerReportsProgress = true;
                bgw.WorkerSupportsCancellation = true;
                bgw.RunWorkerAsync();
                System.Threading.Thread.Sleep(500);


            }
            catch (Exception)
            {

                throw;
            }

        }

        void bgw_DoWorkTarjetaInterna(object sender, DoWorkEventArgs e)
        {
            decimal valor = decimal.Parse(txtValor.Text.Trim());
            //verificaTarjetaInterna(valor);
        }


        void bgw_RunWorkerTarejtaInternaCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            core_tarjetacreditointerno tarjetaInterna = null;
            core_tarjetacreditointerno tarjetaInternaAdi = null;
            string NumeroTarjeta = txtCuenta.Text;

            try
            {
                //loading.Hide();
                paneLoading.Visible = false;
                if (!string.IsNullOrWhiteSpace(ResponseBackground))
                {
                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[ResponseBackground]", valor = ResponseBackground });
                    Control.Common.General.GetMensajeToList(402, parametros);
                }


                tarjetaInterna = Control.Cliente.ConsultarTarjetaInternaGen(txtCuenta.Text);

                decimal saldoTarjeta = 0;
                decimal saldoConsumo = 0;

                if (tarjetaInterna != null || tarjetaInternaAdi != null)
                {
                    if (_tarjetaInterna == null) { _tarjetaInterna = new core_tarjetacreditointerno(); }

                    tarjetaInternaAdi = Control.Cliente.ConsultarTarjetaInternaAdicionalGen(txtCuenta.Text);
                    if (tarjetaInternaAdi == null) { _tarjetaInternaAdicional = new core_tarjetacreditointerno(); }
                }

                saldoTarjeta = decimal.Parse(string.Format("{0:C}", (_tarjetaInterna == null) ? _tarjetaInterna.saldo : _tarjetaInternaAdicional.saldo));


                decimal valorConsumo = decimal.Parse(string.Format("{0:C}", txtValor.Text));

                if (saldoTarjeta < valorConsumo)
                {
                    saldoConsumo = valorConsumo;
                }
                else
                {
                    saldoConsumo = saldoTarjeta - valorConsumo;
                }

                lblGiftCardSaldo.Text = saldoConsumo.ToString();


                //Background activara una bandera si se debe cerrar el formulario
                if (DebeCerrarFormBackground) this.Close();
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "bgw_RunWorkerCompleted", "La solicitud no pudo ser realizada debido a un incidente en los controles. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                Control.Common.General.GetMensajeToList(192);

            }
        }


        private void validaNotaCredito()
        {
            string saldoNC = "0.00";
            decimal valoRestante = decimal.Parse(txtValor.Text);
            Screen targetScreen = Control.Common.General.GetScreenCajero();
            Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
            var t = new NotaCredito();

            if (t.getTarjeta(_textoOriginal))
            {
                _notaCredito = t;
                var numeroNC = t.getCodigo().Remove(1, 3);

                using (POSEntities db = new POSEntities())
                {
                    if (!db.core_notacredito.Any(x => x.numdocumento == numeroNC)) // && (x.cliente == _factura.ClienteIdentificacion)))
                    {
                        string msj = "No existe una NC con código '" + numeroNC + "'. Revise que el código que está ingresando sea correcto '" + txtCuenta.Text + "'";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "btnVerificar_Click", msj);

                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[numeroNC]", valor = numeroNC });
                        parametros.Add(new ParametrosMensajes() { codigo = "[cuenta]", valor = _textoOriginal });

                        //this.TopLevel = false;
                        //Control.Common.General.GetMensajeToList(403, parametros);
                        this.TopLevel = true;

                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            Control.Common.General.GetMensajeToList(403, targetScreen, parametros);
                        });




                        _notaCredito = null;
                        lblGiftCardSaldo.Text = string.Format("{0:C}", 0M);
                        txtValor.Text = string.Empty;
                        return;
                    }
                    else if (Control.Common.GlobalParameters.VALIDAR_VIGENCIA_IVA12)//segun sea la NC con 12 se recalcula el iva a la factura 12
                    {
                        var NC = db.core_notacredito.Where(x => x.numdocumento == numeroNC
                        && x.fecha <= Control.Common.GlobalParameters.VIGENCIA_IVA12_SEGUN_FECHA_NC);
                        if (NC != null)
                        {
                            if (NC.ToList().Count > 0)
                            {
                                if (Control.Common.GlobalParameters.BUSCAR_DETALLE_IVA12_NC)
                                {
                                    var factnum = int.Parse(NC.First().documentoaplica.Substring(6, 9));
                                    var estab = NC.First().documentoaplica.Substring(0, 3);
                                    var punto = NC.First().documentoaplica.Substring(3, 3);
                                    var fact = db.core_factura.Where(x => x.establecimiento == estab && x.punto_emision == punto && x.numero == factnum);

                                    if (fact != null)
                                    {
                                        if (fact.ToList().Count > 0)
                                        {
                                            var idNC = NC.First().id;
                                            var idFact = fact.First().id;
                                            var ncDet = db.core_notacreditodetalle.Where(x => x.notacredito_id == idNC);
                                            var factDet = db.core_facturadetalle.Where(x => x.factura_id == idFact);

                                            var aplicaIva = factDet.Join(ncDet,
                                                                            factItem => factItem.item_id, // Clave de la primera colección
                                                                            ncItem => ncItem.item_id, // Clave de la segunda colección
                                                                            (factItem, ncItem) => new {
                                                                                FacturaDetalle = factItem,
                                                                                NotaCreditoDetalle = ncItem
                                                                            }).Where(s => s.FacturaDetalle.item_id == s.NotaCreditoDetalle.item_id
                                                                            && s.FacturaDetalle.iva > 0)
                                                                        .ToList();

                                            if (aplicaIva != null)
                                            {
                                                if (aplicaIva.Count() > 0)
                                                {
                                                    //Recalcular IVA al 12
                                                    Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = true;
                                                }
                                                else
                                                {
                                                    Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //Recalcular IVA al 12
                                    Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = true;
                                }
                            }
                        }
                    }
                }


                decimal saldoNotaCredito = _notaCredito.getSaldo();

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "VerificaSaLdoNC", "Saldo Tarjeta: ");
                saldoNC = _notaCredito.getSaldo().ToString();

                if (saldoNotaCredito <= valoRestante)
                {
                    saldoNC = saldoNotaCredito.ToString();
                }


                flagTarjetaValida = true;


            }
            else
            {
                txtCuenta.Text = "";
                flagTarjetaValida = false;
            }

        }


        private void validaNotaCreditoLocal(string tarjeta)
        {

            Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
            var t = new NotaCredito();

            if (t.getTarjeta(tarjeta))
            {
                _notaCredito = t;
                var numeroNC = t.getCodigo().Remove(1, 3);

                using (POSEntities db = new POSEntities())
                {
                    if (!db.core_notacredito.Any(x => x.numdocumento == numeroNC)) // && (x.cliente == _factura.ClienteIdentificacion)))
                    {
                        //string msj = "No existe una NC con código '" + numeroNC + "'. Revise que el código que está ingresando sea correcto '" + txtCuenta.Text + "'";
                        //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "btnVerificar_Click", msj);
                        //MessageBox.Show(this, msj);

                        Control.Common.General.GetMensajeToList(671);

                        _notaCredito = null;
                        //lblGiftCardSaldo.Text = string.Format("{0:C}", 0M);
                        lblGiftCardSaldo.Text = "0.0";
                        txtValor.Text = string.Empty;
                        return;
                    }
                    else if (Control.Common.GlobalParameters.VALIDAR_VIGENCIA_IVA12)//segun sea la NC con 12 se recalcula el iva a la factura 12
                    {
                        var NC = db.core_notacredito.Where(x => x.numdocumento == numeroNC
                                    && x.fecha <= Control.Common.GlobalParameters.VIGENCIA_IVA12_SEGUN_FECHA_NC);

                        if (NC != null)
                        {
                            if (NC.ToList().Count > 0)
                            {
                                if (Control.Common.GlobalParameters.BUSCAR_DETALLE_IVA12_NC)
                                {
                                    var factnum = int.Parse(NC.First().documentoaplica.Substring(6, 9));
                                    var estab = NC.First().documentoaplica.Substring(0, 3);
                                    var punto = NC.First().documentoaplica.Substring(3, 3);
                                    var fact = db.core_factura.Where(x => x.establecimiento == estab && x.punto_emision == punto && x.numero == factnum);

                                    if (fact != null)
                                    {
                                        if (fact.ToList().Count > 0)
                                        {
                                            var idNC = NC.First().id;
                                            var idFact = fact.First().id;
                                            var ncDet = db.core_notacreditodetalle.Where(x => x.notacredito_id == idNC);
                                            var factDet = db.core_facturadetalle.Where(x => x.factura_id == idFact);

                                            var aplicaIva = factDet.Join(ncDet,
                                                                            factItem => factItem.item_id, // Clave de la primera colección
                                                                            ncItem => ncItem.item_id, // Clave de la segunda colección
                                                                            (factItem, ncItem) => new {
                                                                                FacturaDetalle = factItem,
                                                                                NotaCreditoDetalle = ncItem
                                                                            })
                                                                            .Where(s => s.FacturaDetalle.item_id == s.NotaCreditoDetalle.item_id
                                                                            && s.FacturaDetalle.iva > 0)
                                                                        .ToList();

                                            if (aplicaIva != null)
                                            {
                                                if (aplicaIva.Count() > 0)
                                                {
                                                    //Recalcular IVA al 12
                                                    Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = true;
                                                }
                                                else
                                                {
                                                    Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //Recalcular IVA al 12
                                    Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = true;
                                }
                            }
                        }
                    }
                }

                //lblGiftCardSaldo.Text = string.Format("{0:C}", _notaCredito.getSaldo());
                lblGiftCardSaldo.Text = _notaCredito.getSaldo().ToString();
                txtValor.Text = _notaCredito.getSaldo().ToString();

            }
            else
            {
                txtCuenta.Text = "";
            }


        }

        /// <summary>
        /// Valida y carga el saldo de una Tarjeta de Regalo o GiftCard.
        /// </summary>
        /// <returns>True si la tarjeta es válida y se cargó el saldo; False si hubo error.</returns>
        private bool ProcesarValidacionTarjetaRegalo(ClienteEmpleado clteEmpleado, decimal valorRestante)
        {

            if (_pagoTipo != PagoTipo.TarjetaRegalo) return false;

            lblGiftCard.Text = "Saldo GiftCard";
            //decimal maxconsumo = decimal.Round(_factura.GetTotal() * (Control.Common.GlobalParameters.MonederoPorcentajeConsumo / 100), 2);
            decimal maxconsumo = decimal.Round(valorRestante * (Control.Common.GlobalParameters.MonederoPorcentajeConsumo / 100), 2);

            using (var db = new POSEntities())
            {
                // 1. Validar si existe nota de crédito
                if (db.core_notacredito.Any(x => ("000" + x.numdocumento) == txtCuenta.Text))
                {
                    MostrarErrorConParametro(209, "[cuenta]", txtCuenta.Text);
                    txtCuenta.Text = string.Empty;
                    txtValor.Text = "0.00";
                    lblGiftCardSaldo.Text = "0.00";

                    LimpiarEstadoTarjeta();
                    return false;
                }
            }

            string numeroTarjeta = ValorOriginalTarjeta?.Trim();

            // 2. Caso especial: App Móvil por prefijo en _textoOriginal
            if (!string.IsNullOrEmpty(_textoOriginal) &&
                _textoOriginal.Trim().StartsWith(Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
            {
                if (_factura.ClienteIdentificacion.Trim().ToString() != clteEmpleado.Identificacion.Trim().ToString())
                {
                    Control.Common.General.GetMensajeToList(639);

                    txtCuenta.Text = string.Empty;
                    txtValor.Text = "0.00";
                    lblGiftCardSaldo.Text = "0.00";
                    return false;
                }

                return ProcesarTarjetaAppMovil(maxconsumo, clteEmpleado);
            }

            if (!string.IsNullOrEmpty(_textoOriginal) &&
                _textoOriginal.Trim().StartsWith("000"))
            {
                MostrarError(687);
                return false;
            }



            var t = new TarjetaRegalo();
            bool tarjetaEncontrada = false;
            decimal SaldoGifCard = 0M;

            //jchid Giftcard que empiezan con 99 

            bool omitirConsultaLocal = numeroTarjeta?.StartsWith("99") == true;
            bool omitirTarjetaConsultaLocal = numeroTarjeta?.StartsWith("13") == true;  // código de la version 219 - opozo

            if (!(omitirConsultaLocal || omitirTarjetaConsultaLocal)) // código de la versión 219 - opozo
            {
                // 3. Primera consulta: Tarjeta Genérica en LOCAL (último parámetro = true)
                bool encontradoLocal = t.getTarjetaGen(ValorOriginalTarjeta, "", false, true);

                if (encontradoLocal)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "POS.Control.Pagos.BasePagos",
                        "ProcesarValidacionTarjetaRegalo",
                        $"Tarjeta encontrada en srv-pos local: {ValorOriginalTarjeta}"
                    );

                    _tarjetaRegalo = t;
                    SaldoGifCard = _tarjetaRegalo.getSaldo();
                    tarjetaEncontrada = true;
                }
                else
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Warning,
                        "POS.Control.Pagos.BasePagos",
                        "ProcesarValidacionTarjetaRegalo",
                        $"Tarjeta NO encontrada en srv-pos local: {ValorOriginalTarjeta}. Intentando en global..."
                    );
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Info,
                    "POS.Control.Pagos.BasePagos",
                    "ProcesarValidacionTarjetaRegalo",
                    $"Tarjeta con prefijo 99 o 13 detectada: {ValorOriginalTarjeta}. Omitiendo consulta local, consultando directamente en global..."
                );
            }

            // 4. Segunda consulta: Tarjeta Genérica en GLOBAL (último parámetro = false)
            // Solo ejecutar si no se encontró en local O si se omitió la consulta local
            if (!tarjetaEncontrada)
            {
                bool encontradoGlobal = t.getTarjetaGen(ValorOriginalTarjeta, "", false, false);

                if (encontradoGlobal)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "POS.Control.Pagos.BasePagos",
                        "ProcesarValidacionTarjetaRegalo",
                        $"Tarjeta encontrada en srv-pos global: {ValorOriginalTarjeta}"
                    );

                    _tarjetaRegalo = t;
                    SaldoGifCard = _tarjetaRegalo.getSaldo();
                    tarjetaEncontrada = true;
                }
                else
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        "POS.Control.Pagos.BasePagos",
                        "ProcesarValidacionTarjetaRegalo",
                        $"Tarjeta NO encontrada ni en local ni en global: {ValorOriginalTarjeta}"
                    );
                }
            }



            // 5. Si no se encontró en ninguno de los dos servicios
            if (!tarjetaEncontrada)
            {
                MostrarError(209);

                txtCuenta.Text = string.Empty;
                txtValor.Text = "0.00";
                lblGiftCardSaldo.Text = "0.00";


                LimpiarEstadoTarjeta();
                return false;
            }

            // 6. Casos especiales tras encontrar la tarjeta
            if (numeroTarjeta?.StartsWith("2222") == true && SaldoGifCard == 0M)
            {
                // Intentar con versión móvil (AppMovil)
                if (t.getTarjetaGen(ValorOriginalTarjeta, "", true))
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "POS.Control.Pagos.BasePagos",
                        "ProcesarValidacionTarjetaRegalo",
                        $"Tarjeta 2222 encontrada en modo móvil: {ValorOriginalTarjeta}"
                    );

                    _tarjetaRegalo = t;
                    SaldoGifCard = _tarjetaRegalo.getSaldoGiftCard();
                }
                else
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        "POS.Control.Pagos.BasePagos",
                        "ProcesarValidacionTarjetaRegalo",
                        $"No se pudo obtener saldo para tarjeta 2222 en modo móvil: {ValorOriginalTarjeta}"
                    );

                    MostrarError(209);
                    LimpiarEstadoTarjeta();
                    return false;
                }
            }
            else if (numeroTarjeta?.StartsWith(Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp) == true && SaldoGifCard == 0M)
            {
                // Si es App Móvil y saldo 0, usar billetera
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Info,
                    "POS.Control.Pagos.BasePagos",
                    "ProcesarValidacionTarjetaRegalo",
                    "Tarjeta App Móvil con saldo 0: consultando billetera digital"
                );

                var validaSaldos = MetodosBilletera.RecuperaSaldosPorIdentificacion(clteEmpleado.Identificacion);
                SaldoGifCard = validaSaldos.SaldoGifCard;

                if (validaSaldos.codError != 0)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        "POS.Control.Pagos.BasePagos",
                        "ProcesarValidacionTarjetaRegalo",
                        $"Error al consultar billetera: codError={validaSaldos.codError}, msjError={validaSaldos.msjError}"
                    );

                    MostrarError(209, validaSaldos.msjError);
                    LimpiarEstadoTarjeta();
                    return false;
                }
            }

            // 7. Aplicar monto según saldo disponible
            if (maxconsumo <= SaldoGifCard)
            {
                txtValor.Text = maxconsumo.ToString("N2");
                lblGiftCardSaldo.Text = SaldoGifCard.ToString("N2");
            }
            else
            {
                txtValor.Text = SaldoGifCard.ToString("N2");
                lblGiftCard.Visible = true;
                lblGiftCardSaldo.Visible = true;
                lblGiftCardSaldo.Text = SaldoGifCard.ToString("N2");
            }

            flagTarjetaValida = true;
            return true;
        }

        private void MostrarError(int codigo, string mensajeAdicional = null)
        {
            Screen targetScreen = Control.Common.General.GetScreenCajero();

            this.BeginInvoke((MethodInvoker)delegate
            {
                Control.Common.General.GetMensajeToList(codigo, targetScreen, null, mensajeAdicional);
            });
        }

        private void MostrarErrorConParametro(int codigo, string clave, string valor)
        {
            Screen targetScreen = Control.Common.General.GetScreenCajero();

            var parametros = new List<ParametrosMensajes>
            {
                new ParametrosMensajes { codigo = clave, valor = valor }
            };

            this.BeginInvoke((MethodInvoker)delegate
            {
                Control.Common.General.GetMensajeToList(codigo, targetScreen, parametros);
            });
        }

        private void LimpiarEstadoTarjeta()
        {
            _tarjetaRegalo = null;
            txtValor.Text = string.Empty;
            lblGiftCardSaldo.Text = string.Format("{0:C}", 0M);
        }

        private bool ProcesarTarjetaAppMovil(decimal maxconsumo, ClienteEmpleado clteEmpleado)
        {
            Control.Common.Logger.LogMessage(
                Control.Common.Enum.LogTypes.Error,
                "POS.Control.Pagos.BasePagos",
                "ProcesarValidacionTarjetaRegalo",
                "Ejecuta consulta de saldo de GiftCard / Tarjeta de Regalo (App Móvil)"
            );

            var validaSaldos2 = MetodosBilletera.RecuperaSaldosPorIdentificacion(clteEmpleado.Identificacion);

            if (validaSaldos2.codError != 0)
            {
                MostrarError(209, validaSaldos2.msjError);
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "POS.Control.Pagos.BasePagos",
                    "ProcesarValidacionTarjetaRegalo",
                    $"codError: {validaSaldos2.codError}; msjError: {validaSaldos2.msjError}"
                );
                LimpiarEstadoTarjeta();
                return false;
            }

            decimal saldoGifCard = validaSaldos2.SaldoGifCard;

            if (maxconsumo <= saldoGifCard)
            {
                txtValor.Text = maxconsumo.ToString("N2");
                lblGiftCardSaldo.Text = saldoGifCard.ToString("N2");
            }
            else
            {
                txtValor.Text = saldoGifCard.ToString("N2");
                lblGiftCard.Visible = true;
                lblGiftCardSaldo.Visible = true;
                lblGiftCardSaldo.Text = saldoGifCard.ToString("N2");
            }

            flagTarjetaValida = true;
            return true;
        }



        private void btnVerificar_Click(object sender, EventArgs e)
        {


            decimal valorRestante = string.IsNullOrEmpty(txtValor.Text) ? 0M : decimal.Parse(txtValor.Text);
            Screen targetScreen = Control.Common.General.GetScreenCajero();
            string codigo = _textoOriginal;
            string tarjetaleida = string.Empty;

            decimal GiftCardSaldo = 0;
            decimal SaldoMonedero = 0;
            decimal saldo = 0;


            try
            {
                lblGiftCard.Visible = true;
                lblGiftCardSaldo.Visible = true;
                lblGiftCardSaldo.Text = "0.00";
                //txtValor.Text = "0.00";
                tarjetaleida = _textoOriginal;
                NotaCredito notaCredito = new NotaCredito();
                TarjetaRegalo tarjetaRegalo = new TarjetaRegalo();

                ClienteEmpleado clteEmpleado = Common.General.ValidaClienteEmpleado(codigo, Common.GlobalParameters.Establecimiento, Common.GlobalParameters.PINPAD_MULTIRED);
                //decimal maxconsumo = decimal.Round(_factura.GetTotal() * (Control.Common.GlobalParameters.MonederoPorcentajeConsumo / 100), 2);
                decimal maxconsumo = decimal.Round(valorRestante * (Control.Common.GlobalParameters.MonederoPorcentajeConsumo / 100), 2);


                // Tarjeta de Regalo o GiftCard
                if (_pagoTipo == PagoTipo.TarjetaRegalo)
                {
                    var validacion = notaCredito.validaNotaCredito(tarjetaleida, valorRestante, true);


                    if (tarjetaleida.Trim().StartsWith("000"))
                    {

                        txtCuenta.Text = string.Empty;
                        txtValor.Text = "0.00";
                        lblGiftCardSaldo.Text = "0.00";

                        Control.Common.General.GetMensajeToList(687);
                        return;
                    }



                    bool resultado = ProcesarValidacionTarjetaRegalo(clteEmpleado, valorRestante);

                }

                if (_pagoTipo == PagoTipo.NotaCredito)
                {
                    valorRestante = decimal.Parse(string.IsNullOrEmpty(txtValor.Text) ? "0.0" : txtValor.Text);
                    notaCredito = new NotaCredito();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnVerificar_Click"
                                , " Ejecuta Consulta en el local ");

                    var validaNC = notaCredito.validaNotaCredito(tarjetaleida, valorRestante, true);

                    if (validaNC.CodError == 0)
                    {
                        _notaCredito = notaCredito;

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnVerificar_Click"
                                , " Valida Cliente factura y validaNC ");

                        if (_factura.ClienteIdentificacion.Trim().ToString() != validaNC.identificacionClte.Trim().ToString())
                        {
                            Control.Common.General.GetMensajeToList(686);


                            lblGiftCardSaldo.Text = "0.00";
                            txtValor.Text = "0.00";
                            txtCuenta.Text = "";
                            return;
                        }


                        if (validaNC.saldo > valorRestante)
                        {
                            txtValor.Text = valorRestante.ToString("N2");
                        }
                        else
                        {
                            txtValor.Text = validaNC.saldo.ToString("N2");
                        }

                        lblGiftCardSaldo.Text = validaNC.saldo.ToString("N2");
                        return;

                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnVerificar_Click"
                               , " Ejecuta Consulta en el SRV-POS ");
                    var validaNC_SRV = notaCredito.validaNotaCredito(tarjetaleida, valorRestante, false);
                    if (validaNC.CodError == 0)
                    {
                        _notaCredito = notaCredito;

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnVerificar_Click"
                                , " Valida Cliente factura y validaNC / SRV-POS ");

                        if (_factura.ClienteIdentificacion.Trim().ToString() != validaNC.identificacionClte.Trim().ToString())
                        {
                            Control.Common.General.GetMensajeToList(686);
                            return;
                        }

                        if (validaNC.saldo > valorRestante)
                        {
                            txtValor.Text = valorRestante.ToString("N2");
                        }
                        else
                        {
                            txtValor.Text = validaNC.saldo.ToString("N2");
                        }

                        lblGiftCardSaldo.Text = validaNC.saldo.ToString("N2");

                        return;
                    }
                    else
                    {
                        Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
                        var t = new NotaCredito();

                        if (t.getTarjeta(tarjetaleida))
                        {
                            _notaCredito = t;
                            var numeroNC = t.getCodigo().Remove(1, 3);

                            using (POSEntities db = new POSEntities())
                            {
                                if (!db.core_notacredito.Any(x => x.numdocumento == numeroNC)) // && (x.cliente == _factura.ClienteIdentificacion)))
                                {
                                    //string msj = "No existe una NC con código '" + numeroNC + "'. Revise que el código que está ingresando sea correcto '" + txtCuenta.Text + "'";
                                    //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "btnVerificar_Click", msj);
                                    //MessageBox.Show(this, msj);

                                    this.BeginInvoke((MethodInvoker)delegate
                                    {
                                        Control.Common.General.GetMensajeToList(209, targetScreen);
                                    });

                                    _notaCredito = null;
                                    lblGiftCardSaldo.Text = string.Format("{0:C}", 0M);
                                    txtValor.Text = string.Empty;
                                    return;
                                }
                                else if (Control.Common.GlobalParameters.VALIDAR_VIGENCIA_IVA12)//segun sea la NC con 12 se recalcula el iva a la factura 12
                                {
                                    var NC = db.core_notacredito.Where(x => x.numdocumento == numeroNC
                                                && x.fecha <= Control.Common.GlobalParameters.VIGENCIA_IVA12_SEGUN_FECHA_NC);

                                    if (NC != null)
                                    {
                                        if (NC.ToList().Count > 0)
                                        {
                                            if (Control.Common.GlobalParameters.BUSCAR_DETALLE_IVA12_NC)
                                            {
                                                var factnum = int.Parse(NC.First().documentoaplica.Substring(6, 9));
                                                var estab = NC.First().documentoaplica.Substring(0, 3);
                                                var punto = NC.First().documentoaplica.Substring(3, 3);
                                                var fact = db.core_factura.Where(x => x.establecimiento == estab && x.punto_emision == punto && x.numero == factnum);

                                                if (fact != null)
                                                {
                                                    if (fact.ToList().Count > 0)
                                                    {
                                                        var idNC = NC.First().id;
                                                        var idFact = fact.First().id;
                                                        var ncDet = db.core_notacreditodetalle.Where(x => x.notacredito_id == idNC);
                                                        var factDet = db.core_facturadetalle.Where(x => x.factura_id == idFact);

                                                        var aplicaIva = factDet.Join(ncDet,
                                                                                        factItem => factItem.item_id, // Clave de la primera colección
                                                                                        ncItem => ncItem.item_id, // Clave de la segunda colección
                                                                                        (factItem, ncItem) => new {
                                                                                            FacturaDetalle = factItem,
                                                                                            NotaCreditoDetalle = ncItem
                                                                                        })
                                                                                        .Where(s => s.FacturaDetalle.item_id == s.NotaCreditoDetalle.item_id
                                                                                        && s.FacturaDetalle.iva > 0)
                                                                                    .ToList();

                                                        if (aplicaIva != null)
                                                        {
                                                            if (aplicaIva.Count() > 0)
                                                            {
                                                                //Recalcular IVA al 12
                                                                Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = true;
                                                            }
                                                            else
                                                            {
                                                                Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Recalcular IVA al 12
                                                Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = true;
                                            }
                                        }
                                    }
                                }
                            }

                            lblGiftCardSaldo.Text = string.Format("{0:C}", _notaCredito.getSaldo());
                            txtValor.Text = _notaCredito.getSaldo().ToString();


                        }
                        else
                        {
                            txtCuenta.Text = "";
                        }

                        return;
                    }



                }

                if (_pagoTipo == PagoTipo.TarjetaInterna)
                {
                    lblGiftCard.Text = "Saldo Tarjeta";
                    var validTarejta = verificaTarjetaInterna();


                    if (!validTarejta)
                    {
                        txtValor.Text = string.Empty;
                        txtCuenta.Focus();
                        flagTarjetaValida = false;
                        return;
                    }



                    flagTarjetaValida = true;

                }

                if (_pagoTipo == PagoTipo.DineroElectronico)
                {
                    string textoValidar = tarjetaleida.Trim();

                    if (tarjetaleida.Trim().StartsWith("000"))
                    {
                        Control.Common.General.GetMensajeToList(688);
                        txtCuenta.Text = string.Empty;
                        txtValor.Text = "0.00";
                        lblGiftCardSaldo.Text = "0.00";
                        return;
                    }


                    if (!textoValidar.StartsWith("666") || !textoValidar.Contains("-"))
                    {
                        Common.General.GetMensajeToList(9008); // o un mensaje nuevo específico
                        txtCuenta.Text = string.Empty;
                        txtValor.Text = "0.00";
                        lblGiftCardSaldo.Text = "0.00";
                        return;
                    }


                    clteEmpleado = Common.General.ValidaClienteEmpleado(_textoOriginal, Common.GlobalParameters.Establecimiento, Common.GlobalParameters.PINPAD_MULTIRED);

                    using (var db = new POSEntities())
                    {
                        flagTarjetaValida = false;


                        if (Control.Common.GlobalParameters.MonederoPorcentajeConsumo <= 0)
                        {
                            this.TopLevel = false;
                            Common.General.GetMensajeToList(207);
                            this.TopLevel = true;

                            return;
                        }

                        maxconsumo = decimal.Round(valorRestante * (Control.Common.GlobalParameters.MonederoPorcentajeConsumo / 100), 2);
                        if (_factura.CodigoClienteApp != clteEmpleado.CodigoClienteApp)
                        {
                            Common.General.GetMensajeToList(639);

                            txtCuenta.Text = string.Empty;
                            txtValor.Text = "0.00";
                            lblGiftCardSaldo.Text = "0.00";

                            return;
                        }

                        var validaMonedero = MetodosBilletera.RecuperaSaldosPorIdentificacion(clteEmpleado.Identificacion);

                        if (validaMonedero.codError != 0)
                        {
                            lblGiftCardSaldo.Text = "0.00";
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnVerificar BIlletera Electronica"
                                , " validaMonedero msjError: " + validaMonedero.msjError);

                            return;
                        }


                        if (validaMonedero.SaldoMonedero == 0)
                        {
                            Common.General.GetMensajeToList(640);
                            return;
                        }

                        if (maxconsumo <= validaMonedero.SaldoMonedero)
                        {
                            txtValor.Text = maxconsumo.ToString("N2");
                            lblGiftCardSaldo.Text = validaMonedero.SaldoMonedero.ToString("N2");

                            flagTarjetaValida = true;
                        }
                        else
                        {
                            txtValor.Text = validaMonedero.SaldoMonedero.ToString("N2");

                            lblGiftCard.Visible = true;
                            lblGiftCardSaldo.Visible = true;
                            lblGiftCardSaldo.Text = validaMonedero.SaldoMonedero.ToString("N2");

                            flagTarjetaValida = true;
                        }
                    }
                }
                // Coloca esto dentro de btnVerificar_Click, justo antes de cerrar el método

                if (_pagoTipo == PagoTipo.CompraGratis)
                {
                    // 1. BLOQUEO DE SEGURIDAD (Evita doble clic)
                    if (_compraGratisCalculada) return;

                    try
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "btnVerificar", "Iniciando validación");

                        // A. OBTENEMOS EL TEXTO CRUDO
                        string codigoIngresado = !string.IsNullOrEmpty(_textoOriginal) ? _textoOriginal : txtCuenta.Text.Trim();

                        // LOGICA DEL SPLIT (Limpiar guion si existe)
                        if (!string.IsNullOrEmpty(codigoIngresado) && codigoIngresado.Contains("-"))
                        {
                            string[] partes = codigoIngresado.Split('-');
                            if (partes.Length > 0)
                            {
                                codigoIngresado = partes[0].Trim();
                                txtCuenta.Text = codigoIngresado; // Visual
                            }
                        }

                        // Validación básica de texto vacío
                        if (string.IsNullOrEmpty(codigoIngresado) || codigoIngresado.Contains("*"))
                        {
                            Control.Common.General.GetMensajeToList(10005);
                            txtCuenta.Text = "";
                            _textoOriginal = "";
                            txtCuenta.Focus();
                            return;
                        }

                        // =========================================================================
                        // PASO CRÍTICO: VALIDACIÓN DE PERTENENCIA (CÉDULA FACTURA VS TARJETA)
                        // =========================================================================

                        string cedulaClienteActual = "";
                        if (this._facturaApp != null)
                        {
                            cedulaClienteActual = this._facturaApp.ClienteIdentificacion;
                        }
                        else if (this._factura != null)
                        {
                            cedulaClienteActual = this._factura.ClienteIdentificacion;
                        }

                        if (string.IsNullOrEmpty(cedulaClienteActual))
                        {
                            MessageBox.Show("No se pudo identificar la cédula del cliente en la factura.", "Error Técnico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string codigoEsperado = "666" + cedulaClienteActual.Trim();

                        if (codigoIngresado != codigoEsperado)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warning, "BasePagos", "btnVerificar",
                                "Tarjeta ajena. Esperado: " + codigoEsperado + " - Leído: " + codigoIngresado);
                            Control.Common.General.GetMensajeToList(10003); // Mensaje Tarjeta Ajena

                            txtCuenta.Text = "";
                            _textoOriginal = "";
                            txtCuenta.Focus();
                            return;
                        }

                        string numeroFacturaActual = this._factura?.GetNumeroFactura() ?? "";

                        if (!string.IsNullOrEmpty(_tarjetaCompraGratisEnUso)
                            && _facturaCompraGratisEnUso == numeroFacturaActual)
                        {
                            Control.Common.General.GetMensajeToList(10026); // "Ya se aplicó CompraGratis en esta transacción"
                            txtCuenta.Text = "";
                            _textoOriginal = "";
                            txtCuenta.Focus();
                            return;
                        }

                        // =========================================================================
                        // VALIDACIÓN CON SERVIDOR CENTRAL (CAMBIO SOLICITADO)
                        // =========================================================================

                        decimal saldoEnTarjeta = 0;
                        bool terminosAceptados = false;
                        bool registroExiste = false;
                        string connectionStringCentral = Control.Common.GlobalParameters.ConServerPuntos;

                        try
                        {
                            using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(connectionStringCentral))
                            {
                                cn.Open();
                                // Consultamos la tarjeta directamente en el central
                                string sqlCentral = "SELECT TOP 1 saldo, acepto_terminos FROM core_TarjetaDescuento WHERE codigo = @cod AND activo = 1";

                                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sqlCentral, cn))
                                {
                                    cmd.Parameters.AddWithValue("@cod", codigoIngresado);
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            registroExiste = true;
                                            saldoEnTarjeta = Convert.ToDecimal(reader["saldo"]);
                                            terminosAceptados = reader["acepto_terminos"] != DBNull.Value && Convert.ToBoolean(reader["acepto_terminos"]);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "btnVerificar", "Fallo conexión Central: " + ex.Message);
                            MessageBox.Show("Error al conectar con el servidor central: " + ex.Message);
                            return;
                        }

                        if (!registroExiste)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "btnVerificar", "Código no encontrado en Central: " + codigoIngresado);
                            Control.Common.General.GetMensajeToList(10006);

                            txtCuenta.Text = "";
                            _textoOriginal = "";
                            return;
                        }

                        if (!terminosAceptados)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warning, "BasePagos", "btnVerificar", "Bloqueo por TyC.");
                            Control.Common.General.GetMensajeToList(10001);

                            lblGiftCardSaldo.ForeColor = System.Drawing.Color.Red;
                            lblGiftCardSaldo.Text = "BLOQUEADO";
                            txtCuenta.Focus();
                            return;
                        }

                        // Obtener Saldo
                        lblGiftCardSaldo.Visible = true;
                        lblGiftCardSaldo.ForeColor = System.Drawing.Color.Black;
                        lblGiftCardSaldo.Text = saldoEnTarjeta.ToString("N2");

                        if (saldoEnTarjeta <= 0)
                        {
                            Control.Common.General.GetMensajeToList(10004); // Mensaje "Sin saldo tarjeta"
                            return;
                        }

                        // Calcular Monto a Pagar (Regla del 10%)
                        decimal totalFactura = 0;
                        decimal.TryParse(txtValor.Text.Replace("$", "").Trim(), out totalFactura);
                        if (totalFactura <= 0) totalFactura = this.valorRestante;

                        decimal montoMaximo = Math.Round(totalFactura * 0.10m, 2);
                        decimal valorAPagar = (saldoEnTarjeta >= montoMaximo) ? montoMaximo : saldoEnTarjeta;

                        // Mostrar Resultado
                        txtValor.Text = valorAPagar.ToString("N2");

                        // Mensajes de éxito
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "btnVerificar", "Aplicado (Central): $" + valorAPagar);

                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[valorAplicado]", valor = "$" + valorAPagar.ToString("N2") });
                        Control.Common.General.GetMensajeToList(10000, parametros);

                        // Bloqueo Final
                        // Registrar en variable global para bloquear si cierran y reabren el form
                        _tarjetaCompraGratisEnUso = codigoIngresado;
                        _facturaCompraGratisEnUso = numeroFacturaActual;
                        _compraGratisCalculada = true;
                        txtCuenta.Enabled = false;
                        btnVerificar.Enabled = false;
                        txtValor.Focus();
                        txtValor.SelectAll();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error técnico: " + ex.Message);
                        _compraGratisCalculada = false;
                        txtCuenta.Enabled = true;
                    }
                }



            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "cmbBancoTarjeta_SelectedIndexChanged", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }

        }



        private void txtCuenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {

                // Para DineroElectronico y TarjetaRegalo este evento
                // nunca llega porque txtCuenta está deshabilitado.
                // El flujo lo maneja txtCaptura en el Load.
                // Solo dejamos la lógica para los otros tipos de pago.
                if (_pagoTipo == PagoTipo.DineroElectronico ||
                    _pagoTipo == PagoTipo.TarjetaRegalo)
                {
                    e.Handled = true; // por seguridad, ignorar todo
                    return;
                }

                // Para NotaCredito, TarjetaInterna, CompraGratis
                // el cajero puede escribir o pistoleas directo en txtCuenta
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    btnVerificar_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "BasePagos",
                    "txtCuenta_KeyPress",
                    "Error: " + ex.Message,
                    ex.StackTrace);
            }
        }

        private void cmbBancoTarjeta_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            try
            {
                if (cmbBancoTarjeta.SelectedIndex != -1)
                {
                    if (_pagoTipo == PagoTipo.TarjetaCredito)
                    {
                        //Validar si es pago manual y el usuario no pasó la tarjeta por el lector universa, entonces mostrar mensaje de observación. esto con el objetivo de que utilicen el lector universal para los pagos manuales y solo utilicen la seleccion de combos cuando sea pedido a domicilio.
                        if (EsManual == 1 && !tarjetaDetectadaPagoManual)
                        {
                            Control.Common.General.GetMensajeToList(210);

                            // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Recuerde que debe seleccionar el Banco/Tarjet Adecuada ya que está utilizando la opción Pago Manual.", "POS - Pago Manual ");
                            // MessageBox.Show(this, "Recuerde que debe seleccionar el Banco/Tarjet Adecuada ya que está utilizando la opción Pago Manual.", "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // MsgBox m = new MsgBox("info", "Recuerde que debe seleccionar el Banco/Tarjet Adecuada ya que está utilizando la opción Pago Manual.", "Notificación del Sistema para Pago Manual");
                            // DialogResult dg = m.ShowDialog();

                        }

                        var db = new POSEntities();
                        if (db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE" && EsManual == 0)
                        {
                        }
                        else
                        {
                            var obj = cmbBancoTarjeta.SelectedValue as core_tarjetacredito_bin;//var obj = cmbBancoTarjeta.SelectedValue as core_tarjetacredito;
                            if (obj != null)
                                if (obj.bin_red == "1")//if (obj.operador == 1)
                                    cmbTipoTransaccion.SelectedIndex = 1;
                                else
                                    cmbTipoTransaccion.SelectedIndex = 0;

                            //if (_factura.Establecimiento=="029")
                            //    cmbTipoTransaccion.SelectedIndex = 1;
                        }
                        //Inicio Controlar meses gracia JCanarte 4Ene2021
                        if (cmbBancoTarjeta.SelectedItem.Text.ToUpper().Contains("MESES DE GRACIA"))
                        {
                            this.cmbMesesGracia.SelectedIndex = -1;
                            this.cmbMesesGracia.Enabled = true;
                        }
                        else
                        {
                            this.cmbMesesGracia.SelectedIndex = -1;
                            this.cmbMesesGracia.Enabled = false;
                        }
                        //Fin JCanarte 4Ene2021  
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "cmbBancoTarjeta_SelectedIndexChanged", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void txtCuenta_TextChanged(object sender, EventArgs e)
        {
            // ── PROTECCIÓN 1: evitar recursividad (ya la tenías) ──
            if (_isUpdatingText) return;

            // ── PROTECCIÓN 2: NUEVA ──────────────────────────────
            // Si txtCuenta está deshabilitado significa que el pistoleador
            // usa txtCaptura. _textoOriginal ya fue asignado correctamente
            // en el KeyPress de txtCaptura. No hacer NADA aquí.
            if (!txtCuenta.Enabled) return;

            // ── TODO LO DE ABAJO solo aplica cuando el cajero
            //    escribe manualmente (otros tipos de pago) ─────────
            try
            {
                _isUpdatingText = true;

                string textoActual = txtCuenta.Text;

                if (textoActual == _textoOriginal)
                    return;

                if (textoActual.Length > _textoOriginal.Length)
                {
                    string nuevoCaracter = textoActual.Substring(_textoOriginal.Length);
                    _textoOriginal += nuevoCaracter;
                }
                else if (textoActual.Length < _textoOriginal.Length)
                {
                    _textoOriginal = textoActual;
                }

                ValorOriginalTarjeta = _textoOriginal;

                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Info,
                    "BasePagos",
                    "txtCuenta_TextChanged",
                    $"ValorOriginalTarjeta: {ValorOriginalTarjeta}");

                string textoEnmascarado = Control.Common.General.EnmascararTexto(_textoOriginal, 0, '*');
                txtCuenta.Text = textoEnmascarado;
                txtCuenta.SelectionStart = txtCuenta.Text.Length;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "BasePagos",
                    "txtCuenta_TextChanged",
                    "Error: " + ex.Message, ex.StackTrace);
            }
            finally
            {
                _isUpdatingText = false;
            }
        }

        private void prepararVoucherTarjeta(string tipo_voucher, Tramas.RespuestaProcesoPago trama)
        {
            var db = new POSEntities();

            if (db.core_recibo.Any(x => x.identificador == tipo_voucher))
            {
                var voucher = db.core_recibo.First(x => x.identificador == tipo_voucher);
                string texto = voucher.cuerpo;
                DateTime fecha = DateTime.ParseExact("01-" + trama.fechaVencTar.Substring(2, 2) + "-" + trama.fechaVencTar.Substring(0, 2), "dd-MM-yy", CultureInfo.InvariantCulture);
                string fechaVencTar = Convert.ToString(fecha).Substring(5, 2) + '/' + Convert.ToString(fecha).Substring(0, 4);

                texto = texto.Replace("<<NOM_TARJETA>>", trama.nomGruTar);
                texto = texto.Replace("<<NUM_TARJETA>>", trama.numTarTuncate);
                texto = texto.Replace("<<NUM_LOTE>>", trama.numerolote);
                texto = texto.Replace("<<ADQUIRIENTE>>", trama.nomBancoAdq);
                texto = texto.Replace("<<APROBACION>>", trama.numAut);
                texto = texto.Replace("<<SECUENCIAL>>", trama.secuencialtransaccion);
                texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", trama.nombreTarjetaHabiente);
                texto = texto.Replace("<<FECHA_TRANS>>", trama.fechaTrans);
                texto = texto.Replace("<<HORA_TRANS>>", trama.horaTrans);
                texto = texto.Replace("<<VENC_TAR>>", fechaVencTar);
                texto = texto.Replace("<<VALOR_TOTAL>>", txtValor.Text);
                /*texto = texto.Replace("", trama.nomGruTar);
                texto = texto.Replace("", trama.nomGruTar);
                texto = texto.Replace("", trama.nomGruTar);
                texto = texto.Replace("", trama.nomGruTar);
                texto = texto.Replace("", trama.nomGruTar);
                texto = texto.Replace("", trama.nomGruTar);
                texto = texto.Replace("", trama.nomGruTar);
                */
                printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                printer.TextToPrint = texto;
                printer.Print();

            }
        }
        private void cmbTipoTransaccion_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (cmbTipoTransaccion.SelectedIndex != -1)
            {
                bool carga_bancotarjeta = true;
                //Validar si es pago manual y el usuario no pasó la tarjeta por el lector universa, entonces mostrar mensaje de observación. esto con el objetivo de que utilicen el lector universal para los pagos manuales y solo utilicen la seleccion de combos cuando sea pedido a domicilio.
                if (EsManual == 1 && !tarjetaDetectadaPagoManual)
                {
                    //MessageBox.Show(this, "Recuerde que debe seleccionar el Tipo Adecuado ya que está utilizando la opción Pago Manual.", "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Recuerde que debe seleccionar el Tipo Adecuado ya que está utilizando la opción Pago Manual.", "POS - Pago Manual ");
                    Control.Common.General.GetMensajeToList(211);
                }

                try
                {
                    var db = new POSEntities();
                    if ((db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE") && (cmbTipoTransaccion.SelectedValue != null))
                    {

                        try
                        {
                            //Validación cuando es por pago manual no corresponde al tipo pos_tarjeta_transaccion.
                            pos_tarjeta_transaccion objtTransaccion = (pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue;
                        }
                        catch (Exception ex1)
                        {
                            //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "cmbTipoTransaccion_SelectedIndexChanged", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas al moento de asignar el valor de carga_bancotarjeta - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex1), "StackTrace: " + ex1.StackTrace);
                            carga_bancotarjeta = false;
                        }
                        if (carga_bancotarjeta)
                        {
                            cmbBancoTarjeta.DataSource = db.pos_tarjeta_tipopago.Where(x => x.idTipo == ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo).ToList();
                            cmbBancoTarjeta.DataMember = "idPago";
                            cmbBancoTarjeta.DisplayMember = "NombreTipoPago";

                            switch (((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo)
                            {
                                //case "03":
                                //    boxAnula.Top = 157;
                                //    boxAnula.Left = 21;
                                //    boxAnula.Visible = true;
                                //    boxDiferido.Visible = false;
                                //    break;
                                case "02":
                                    //boxAnula.Visible = false;
                                    boxDiferido.Visible = true;
                                    break;
                                default:
                                    //boxAnula.Visible = false;
                                    boxDiferido.Visible = false;
                                    break;

                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "cmbTipoTransaccion_SelectedIndexChanged", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                }
            }
        }

        //private void txtValor_Leave(object sender, EventArgs e)
        //{
        //    txtValor.Text = decimal.Parse(txtValor.Text).ToString();
        //}

        private void RecargaCombosPagoManual()
        {
            try
            {
                cmbTipoTransaccion.DataMember = null;
                cmbTipoTransaccion.DisplayMember = null;
                cmbTipoTransaccion.DataSource = null;
                cmbTipoTransaccion.Items.Clear();

                using (var db = new POSEntities())
                {
                    cmbBancoTarjeta.DataSource = null;
                    cmbBancoTarjeta.DisplayMember = "nombre_completo";
                    cmbBancoTarjeta.DataMember = null;
                    cmbBancoTarjeta.Rebind();
                    cmbBancoTarjeta.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
                }
                //obliga al usuario a seleccionar la opcion del combo.
                RadListDataItem raitem = new RadListDataItem("-- Seleccione --", 0);
                cmbBancoTarjeta.Items.Add(raitem);
                cmbBancoTarjeta.SelectedValue = 0;


                RadListDataItem raitemTipo = new RadListDataItem("-- Seleccione --", "0");
                RadListDataItem raitemTipo1 = new RadListDataItem("Datafast", "1");
                RadListDataItem raitemTipo2 = new RadListDataItem("Medianet", "2");
                RadListDataItem raitemTipo3 = new RadListDataItem("Austro", "3");

                cmbTipoTransaccion.Items.Add(raitemTipo);
                cmbTipoTransaccion.Items.Add(raitemTipo1);
                cmbTipoTransaccion.Items.Add(raitemTipo2);
                cmbTipoTransaccion.Items.Add(raitemTipo3);
                cmbTipoTransaccion.SelectedValue = "0";

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnPagoManual_Click(object sender, EventArgs e)
        {
            try
            {

                lblNoTarjeta.Left = 12;
                lblNoTarjeta.Top = 60;
                lblNoTarjeta.Visible = true;
                txtNoTarjeta.Left = 172;
                txtNoTarjeta.Top = 60;
                txtNoTarjeta.Visible = true;

                lblBanco.Text = "Tar. Crédito";
                lblBanco.Left = 12;
                lblBanco.Top = 118;
                cmbBancoTarjeta.Left = 172;
                cmbBancoTarjeta.Top = 115;

                lblTipoPagoTarjeta.Text = "Autorizador";
                lblTipoPagoTarjeta.Left = 12;
                lblTipoPagoTarjeta.Top = 172;
                cmbTipoTransaccion.Left = 172;
                cmbTipoTransaccion.Top = 168;

                // chkPedidoDomicilio.Visible = true;
                btnServDomicilio.Enabled = true;
                btnServDomicilio.Visible = true;

                //habilita siempre y cuando este activo parametro ACTIVAR_TRAMA_TC = true.
                // if (Control.Common.GlobalParameters.ActivarFormaPagoTC)
                // {
                lblNumTransaccionvoucher.Text = "Transacción#";
                lblNumTransaccionvoucher.Left = 12;
                lblNumTransaccionvoucher.Top = 218;
                lblNumTransaccionvoucher.Visible = true;

                txtNumTransaccionVoucher.Left = 172;
                txtNumTransaccionVoucher.Top = 218;
                txtNumTransaccionVoucher.Width = 255;
                txtNumTransaccionVoucher.Visible = true;


                lblAutorizacionVoucher.Text = "Autorización#";
                lblAutorizacionVoucher.Left = 12;
                lblAutorizacionVoucher.Top = 255;
                //lblAutorizacionVoucher.Left = 270;
                //lblAutorizacionVoucher.Top = 218;
                lblAutorizacionVoucher.Visible = true;

                txtNumAutorizacionVoucher.Left = 172;
                txtNumAutorizacionVoucher.Top = 255;
                txtNumAutorizacionVoucher.Width = 255;
                txtNumAutorizacionVoucher.Visible = true;

                // Visible controles de corriente y diferido.   JM  13-10-2020
                lblTID.Left = 10;
                lblTID.Top = 15;
                lblTID.Visible = true;

                cmbTID.Left = 80;
                cmbTID.Top = 15;
                cmbTID.Width = 255;
                cmbTID.Visible = true;

                using (var db = new POSEntities())
                {
                    cmbTID.DataSource = db.core_parametro.Where(x => x.identificador == "TIDCARD" && x.parametro2 == _factura.Establecimiento).ToList();
                    cmbTID.DisplayMember = "valor";
                }

                lblLote.Left = 10;
                lblLote.Top = 53;
                lblLote.Visible = true;

                txtLote.Left = 80;
                txtLote.Top = 55;
                txtLote.Width = 255;
                txtLote.Visible = true;

                this.Height = 742;
                this.Width = 950;
                this.splitContainer1.SplitterDistance = 445;
                this.CenterToParent();

                btnServDomicilio.Top = 525;
                btnPagoManual.Top = 525;
                btnEliminar.Top = 525;
                gridPagos.Top = 567;

                lblTipo.Visible = true;
                lblTipo.Left = 12;
                lblTipo.Top = 297;
                cmbTipo.Visible = true;
                cmbTipo.Left = 172;
                cmbTipo.Top = 292;

                lblTipoPago.Visible = true;
                lblTipoPago.Left = 12;
                lblTipoPago.Top = 345;
                cmbTipoPago.Visible = true;
                cmbTipoPago.Left = 172;
                cmbTipoPago.Top = 342;

                this.splitContainer1.Panel1.Controls.Add(this.lblTipo);
                this.splitContainer1.Panel1.Controls.Add(this.cmbTipo);
                this.splitContainer1.Panel1.Controls.Add(this.lblTipoPago);
                this.splitContainer1.Panel1.Controls.Add(this.cmbTipoPago);

                //  JM  13-10-2020

                //  }

                boxDiferido.Visible = false;

                cmbTipoTransaccion.DataMember = null;
                cmbTipoTransaccion.DisplayMember = null;
                cmbTipoTransaccion.DataSource = null;
                cmbTipoTransaccion.Items.Clear();

                /*evelasco comentado 
                cmbBancoTarjeta.DataSource = null;
                cmbBancoTarjeta.DisplayMember = null;//cmbBancoTarjeta.DisplayMember = "nombre_completo";        
                cmbBancoTarjeta.DataMember = null;
                cmbBancoTarjeta.Rebind();
                //cmbBancoTarjeta.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
                */

                using (var db = new POSEntities())
                {
                    // JM  13-10-2020 

                    List<pos_tarjeta_transaccion> listTarTrans = new List<pos_tarjeta_transaccion>();
                    listTarTrans.Add(new pos_tarjeta_transaccion { idTipo = "00", NombreTarjetaTransaccion = "-- Seleccione --", estado = true });
                    listTarTrans.AddRange(db.pos_tarjeta_transaccion.Where(x => x.estado == true).ToList());

                    cmbTipo.DataSource = listTarTrans; //db.pos_tarjeta_transaccion.Where(x => x.estado == true).ToList();
                    cmbTipo.DataMember = "idTipo";
                    cmbTipo.DisplayMember = "NombreTarjetaTransaccion";
                    cmbTipo.SelectedValue = "00";
                    // JM  13-10-2020

                    cmbBancoTarjeta.DataSource = null;
                    //cmbBancoTarjeta.DisplayMember = null;//cmbBancoTarjeta.DisplayMember = "nombre_completo";
                    cmbBancoTarjeta.DisplayMember = "nombre_completo";
                    cmbBancoTarjeta.DataMember = null;
                    cmbBancoTarjeta.Rebind();
                    //cmbBancoTarjeta.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
                    cmbBancoTarjeta.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
                }
                //obliga al usuario a seleccionar la opcion del combo.
                RadListDataItem raitem = new RadListDataItem("-- Seleccione --", 0);
                cmbBancoTarjeta.Items.Add(raitem);
                cmbBancoTarjeta.SelectedValue = 0;


                RadListDataItem raitemTipo = new RadListDataItem("-- Seleccione --", "0");
                RadListDataItem raitemTipo1 = new RadListDataItem("Datafast", "1");
                RadListDataItem raitemTipo2 = new RadListDataItem("Medianet", "2");
                RadListDataItem raitemTipo3 = new RadListDataItem("Austro", "3");

                cmbTipoTransaccion.Items.Add(raitemTipo);
                cmbTipoTransaccion.Items.Add(raitemTipo1);
                cmbTipoTransaccion.Items.Add(raitemTipo2);
                cmbTipoTransaccion.Items.Add(raitemTipo3);
                cmbTipoTransaccion.SelectedValue = "0";

                /*Dictionary<string,string> _DTipoTransaccion = new Dictionary<string, string>();
                _DTipoTransaccion.Add("00", "-- Seleccione --");
                _DTipoTransaccion.Add("01", "Datafast");
                _DTipoTransaccion.Add("02", "Medianet");*/
                //evelasco cargar los combos.
                //cmbTipoTransaccion.DataSource = _DTipoTransaccion.ToList();


                /* cmbTipoTransaccion.DataMember = "key";
                 cmbTipoTransaccion.DisplayMember = "value";*/

                // cmbTipoTransaccion.Rebind();
                /*cmbTipoTransaccion.Items.Add("Datafast");
                cmbTipoTransaccion.Items.Add("Medianet");
                */
                //cmbTipoTransaccion.SelectedIndex = _factura.Establecimiento == "029" ? 0 : 1;
                //cmbTipoTransaccion.SelectedIndex = 0;


                //cmbBancoTarjeta.Text = " - ";


                EsManual = 1;
                txtValor.Focus();


                /* using (var db = new POSEntities())
                 {
                     lblTipoPagoTarjeta.Text = "Tipo";
                     lblTipoPagoTarjeta.Left = 12;
                     lblTipoPagoTarjeta.Top = 118;
                     cmbTipoTransaccion.Left = 172;
                     cmbTipoTransaccion.Top = 118;

                     lblBanco.Text = "TCREDITO";
                     lblBanco.Left = 12;
                     lblBanco.Top = 60;

                     cmbBancoTarjeta.Left = 172;
                     cmbBancoTarjeta.Top = 60;

                     boxDiferido.Visible = false;

                     cmbTipoTransaccion.DataMember = null;
                     cmbTipoTransaccion.DisplayMember = null;
                     cmbTipoTransaccion.DataSource = null;
                     cmbTipoTransaccion.Items.Clear();
                     cmbTipoTransaccion.Items.Add("Datafast");

                     cmbTipoTransaccion.Items.Add("Medianet");
                     cmbTipoTransaccion.SelectedIndex = _factura.Establecimiento == "029" ? 0 : 1;

                     cmbBancoTarjeta.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
                     cmbBancoTarjeta.DisplayMember = "nombre_completo";
                     cmbBancoTarjeta.DataMember = null;
                     cmbBancoTarjeta.Rebind();
                     cmbBancoTarjeta.SelectedIndex = 6;
                     //cmbBancoTarjeta.Text = " - ";

                     EsManual = 1;

                     cmbTipoTransaccion.Enabled = true;
                     cmbTipoTransaccion.SelectedIndex = 1;
                     // cmbTipoTransaccion.Enabled = false;
                 }*/
                cmbBancoTarjeta.Enabled = false;
                cmbTipoTransaccion.Enabled = false;

                if (_factura != null)
                {
                    if (_factura.AplicaDescuentoPromoBines)
                    {
                        this._numTarjeta = _factura.BinNumeroTarjetaPromo;
                        txtNoTarjeta.Text = "*********************************";
                        ConsultaTarjetaBines(_factura.BinNumeroTarjetaPromo, _factura.BinNumeroTarjetaPromo);
                    }
                }
            }
            catch (Exception exkp)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "btnPagoManual_Click", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exkp), "StackTrace: " + exkp.StackTrace);

            }
        }

        private void txtValor_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;

        }

        private void txtNumCheque_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
        }

        private void txtCuenta_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
        }

        private void BasePagos_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (EstadoControles == false) e.Cancel = true;

            if (gridPagos.RowCount == 0)
            {
                if (this.Owner != null)// !_factura.AplicaDescuentoPromoBines)
                {
                    if (!EsPagoOkPromoTarjeta)
                    {
                        ((MainWindow)this.Owner).QuitarDescuentoPromocionTarjetaBines();
                    }
                    //((MainWindow)this.Owner).EjecutaBotonPago("RETURN");
                }
            }

        }



        private void BasePagos_Shown(object sender, EventArgs e)
        {

            // Deshabilitar combo si es Giftcard
            if (_factura.GetType().Name == typeof(Models.Giftcard.ClsGiftcardSale).Name)
            {
                cmbTipoTransaccion.Enabled = false;
            }

            if (_pagoTipo == PagoTipo.TarjetaRegalo || _pagoTipo == PagoTipo.NotaCredito
                || _pagoTipo == PagoTipo.TarjetaInterna || _pagoTipo == PagoTipo.DineroElectronico)
            {


                // Limpiar campo
                txtCuenta.Clear();
                txtCuenta.Focus();
            }

            if (_pagoTipo == PagoTipo.TarjetaCredito)
            {
                txtValor.Focus();
            }

            this.BringToFront();
        }


        void scanner_ScannerDataReceived(object sender, EventArgs e)
        {
            scanner.ControlToShowText = this.txtCuenta;
            this.txtCuenta.Focus();

            SendKeys.Send("{Enter}");
        }



        private void txtNoTarjeta_KeyPress(object sender, KeyPressEventArgs e)
        {
            string CodigoRed;
            string codigoBin;
            int indice = 0;
            string numtarjetaTodo = string.Empty;
            bool separadorValidoNumeroTarjeta = true;
            try
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "caracter: " + e.KeyChar);
                if (e.KeyChar == (char)Keys.Enter)
                {
                    numtarjetaTodo = txtNoTarjeta.Text;

                    this._tarjetaHabiente = string.Empty;
                    this._numTarjeta = string.Empty;

                    var numeroTarjeta = txtNoTarjeta.Text.Trim().Replace("%B", "");
                    numeroTarjeta = numeroTarjeta.Replace("\t", string.Empty);
                    if (numeroTarjeta.Length >= 8)
                    {
                        //' '
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "numero de tarjeta leida: " + numeroTarjeta);
                        indice = numeroTarjeta.IndexOf('^');
                        if (indice < 0)
                        {
                            indice = numeroTarjeta.IndexOf('&');
                            if (indice < 0) //Validación para Tarjetas ALIA - CUOTAFACIL
                            {
                                indice = numeroTarjeta.IndexOf('=');//Tarjetas ALIA - CUOTAFACIL.
                            }
                        }

                        //Si no encuentra caracter separador, no seguir con la rutina y enviar mensaje.
                        if (indice < 0)
                        {
                            separadorValidoNumeroTarjeta = false;
                        }
                        //Validación de que tenga el separador para leer el número de Bin de la tarjeta. Si no existe validación de caracter para obtener el IndexOf, no puede leer el bin.
                        if (separadorValidoNumeroTarjeta)
                        {

                            this._numTarjeta = this.enmascararTarjeta(numeroTarjeta.Substring(0, indice));

                            indice += 1;
                            for (int i = indice; i < numeroTarjeta.Length; i++)
                            {
                                if (numeroTarjeta[i] == '-')
                                {
                                    break;
                                }
                                if (numeroTarjeta[i] == '^')
                                {
                                    break;
                                }
                                if (numeroTarjeta[i] == '&')
                                {
                                    break;
                                }
                                if (numeroTarjeta[i] == '=')
                                {
                                    break;
                                }
                                if (numeroTarjeta[i] == '=')
                                {
                                    break;
                                }
                                this._tarjetaHabiente += numeroTarjeta[i];
                            }


                            ConsultaTarjetaBines(_numTarjeta, numtarjetaTodo);

                            /*codigoBin = this._numTarjeta.Substring(0, 6);
                            using (var db = new POSEntities())
                            {


                                var core_tarjetacredito_bin = db.core_tarjetacredito_bin.Where(x => x.bin == codigoBin).FirstOrDefault();
                                if (core_tarjetacredito_bin != null)
                                {

                                    //variable para saber que la tarjeta fué pasada por el lector universal.
                                    tarjetaDetectadaPagoManual = true;

                                    cmbTipoTransaccion.DataMember = null;
                                    cmbTipoTransaccion.DisplayMember = null;
                                    cmbTipoTransaccion.DataSource = null;
                                    cmbTipoTransaccion.Items.Clear();

                                    cmbBancoTarjeta.DataSource = null;
                                    cmbBancoTarjeta.DisplayMember = null;
                                    cmbBancoTarjeta.DataMember = null;
                                    cmbBancoTarjeta.Rebind();



                                    cmbBancoTarjeta.DataSource = db.core_tarjetacredito_bin.Where(x => x.bin == codigoBin).ToList(); //core_tarjetacredito_bin;
                                    cmbBancoTarjeta.DisplayMember = "bin_descripcion";//cmbBancoTarjeta.DisplayMember = "nombre_completo";
                                    cmbBancoTarjeta.DataMember = "bin";
                                    cmbBancoTarjeta.Rebind();
                                    cmbBancoTarjeta.SelectedIndex = 0;

                                    CodigoRed = core_tarjetacredito_bin.bin_red == "2" ? "Medianet" : "Datafast";
                                    cmbTipoTransaccion.Items.Add(CodigoRed);
                                    cmbTipoTransaccion.SelectedIndex = 0;
                                    cmbTipoTransaccion.Enabled = true;

                                }


                                else
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "el bin de tarjeta '" + numeroTarjeta + "' no fué encontrado: bin(" + codigoBin + ")");
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "Numero Tarjeta: " + numtarjetaTodo);
                                    tarjetaDetectadaPagoManual = true;
                                    // btnPagoManual_Click(null, null);
                                    RecargaCombosPagoManual();
                                    tarjetaDetectadaPagoManual = false;

                                    ResponseBackground = "Tarjeta no se encuentra en listado de bines";
                                    txtNoTarjeta.Clear();
                                    MessageBox.Show("Tarjeta no pudo ser leida correctamente, favor pase de nuevo la tarjeta por el lector, si no lee, seleccione la tarjeta y tipo del listado", "Notificación Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                
                            }*/
                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "No pudo encontrar el caracter separador para obtener numero de Tarjeta '" + numeroTarjeta + "'");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "obtener el caracter que utiliza la tarjeta de credito y agregar a la validación para obtener el indice. (Ejemplo: indice = numeroTarjeta.IndexOf('&');) ");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "Numero Tarjeta: " + numtarjetaTodo);


                            ConsultaTarjetaBines(_numTarjeta, numtarjetaTodo);

                            //tarjetaDetectadaPagoManual = true;
                            //RecargaCombosPagoManual();
                            //tarjetaDetectadaPagoManual = false;

                            ResponseBackground = "Tarjeta no pudo ser leída correctamente";
                            txtNoTarjeta.Clear();

                            //MessageBox.Show("Esta Tarjeta no pudo ser leída correctamente, favor SELECCIONE la tarjeta (TCREDITO) y el tipo del listado", "Notificación Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //cmbBancoTarjeta.Show();
                            //cmbBancoTarjeta.ShowDropDown(); abre el combo para que el usuario seleccione la tarjeta.
                            //return;

                        }

                        //txtNoTarjeta.SelectAll();
                    }
                }
            }
            catch (Exception exkp)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exkp), "StackTrace: " + exkp.StackTrace);
                txtNoTarjeta.Clear();
                try
                {
                    tarjetaDetectadaPagoManual = true;
                    //btnPagoManual_Click(null, null);
                    RecargaCombosPagoManual();
                    tarjetaDetectadaPagoManual = false;
                }
                catch (Exception ex1)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "Se presentaron novedades durante la ejecución del método (en la sección catch), a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex1), "StackTrace: " + ex1.StackTrace);
                }

                Control.Common.General.GetMensajeToList(213);
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Warning, "Tarjeta no pudo ser leida correctamente, favor a continuación ingrese los 6 primeros dígitos de la tarjeta.", "POS - Pago Manual ");
                //MessageBox.Show("Tarjeta no pudo ser leida correctamente, favor a continuación ingrese los 6 primeros dígitos de la tarjeta.", "Notificación Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);


                SearchTarjetaCreditoBines frmtarcredbin = new SearchTarjetaCreditoBines();
                frmtarcredbin.ShowDialog();
                if (!string.IsNullOrEmpty(frmtarcredbin.NumeroTarjeta))
                {
                    txtNoTarjeta.Text = frmtarcredbin.NumeroTarjeta;
                    this._numTarjeta = txtNoTarjeta.Text;
                    ConsultaTarjetaBines(frmtarcredbin.NumeroTarjeta, frmtarcredbin.NumeroTarjeta);
                    return;
                }

            }
        }

        private void ConsultaTarjetaBines(string _numTarjeta, string numtarjetaTodo)
        {
            string CodigoRed;
            string codigoBin;
            int indice = 0;
            // string numtarjetaTodo = string.Empty;
            bool separadorValidoNumeroTarjeta = true;
            try
            {
                codigoBin = this._numTarjeta.Substring(0, 6);
                using (var db = new POSEntities())
                {


                    var core_tarjetacredito_bin = db.core_tarjetacredito_bin.Where(x => x.bin == codigoBin).FirstOrDefault();
                    if (core_tarjetacredito_bin != null)
                    {

                        //variable para saber que la tarjeta fué pasada por el lector universal.
                        tarjetaDetectadaPagoManual = true;

                        cmbTipoTransaccion.DataMember = null;
                        cmbTipoTransaccion.DisplayMember = null;
                        cmbTipoTransaccion.DataSource = null;
                        cmbTipoTransaccion.Items.Clear();

                        cmbBancoTarjeta.DataSource = null;
                        cmbBancoTarjeta.DisplayMember = null;
                        cmbBancoTarjeta.DataMember = null;
                        cmbBancoTarjeta.Rebind();

                        cmbBancoTarjeta.DataSource = db.core_tarjetacredito_bin.Where(x => x.bin == codigoBin).ToList(); //core_tarjetacredito_bin;
                        cmbBancoTarjeta.DisplayMember = "bin_descripcion";//cmbBancoTarjeta.DisplayMember = "nombre_completo";
                        cmbBancoTarjeta.DataMember = "bin";
                        cmbBancoTarjeta.Rebind();
                        cmbBancoTarjeta.SelectedIndex = 0;

                        CodigoRed = core_tarjetacredito_bin.bin_red == "2" ? "Medianet" : "Datafast";
                        //cmbTipoTransaccion.SelectedValue = core_tarjetacredito_bin.bin_red;
                        cmbTipoTransaccion.Items.Add(CodigoRed);
                        cmbTipoTransaccion.SelectedIndex = 0;
                        cmbTipoTransaccion.Enabled = true;
                        ValidaPagoTarjetaBines(codigoBin);

                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "el bin de tarjeta '" + _numTarjeta + "' no fué encontrado: bin(" + codigoBin + ")");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "Numero Tarjeta: " + numtarjetaTodo);

                        tarjetaDetectadaPagoManual = true;
                        // btnPagoManual_Click(null, null);
                        RecargaCombosPagoManual();
                        tarjetaDetectadaPagoManual = false;

                        ResponseBackground = "Tarjeta no se encuentra en listado de bines";
                        txtNoTarjeta.Clear();

                        Control.Common.General.GetMensajeToList(213);
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Warning, "Tarjeta no pudo ser leida correctamente, favor a continuación ingrese los 6 primeros dígitos de la tarjeta ", "POS . Pago Manual ");

                        //MessageBox.Show("Tarjeta no pudo ser leida correctamente, favor a continuación ingrese los 6 primeros dígitos de la tarjeta.", "Notificación Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Si no se encuentra el bin llama al formulario para digitarlo.
                        SearchTarjetaCreditoBines frmtarcredbin = new SearchTarjetaCreditoBines();
                        frmtarcredbin.ShowDialog();
                        if (!string.IsNullOrEmpty(frmtarcredbin.NumeroTarjeta))
                        {
                            txtNoTarjeta.Text = frmtarcredbin.NumeroTarjeta;
                            this._numTarjeta = txtNoTarjeta.Text;
                            ConsultaTarjetaBines(frmtarcredbin.NumeroTarjeta, frmtarcredbin.NumeroTarjeta);
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void ValidaPagoTarjetaBines(string codigoBin)
        {
            try
            {
                if (_factura.AplicaDescuentoPromoBines)
                {
                    if (_factura.BinNumeroTarjetaPromo != codigoBin)
                    {
                        var result = Control.Common.General.GetMensajeToList(214);
                        if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                        {
                            ((MainWindow)this.Owner).QuitarDescuentoPromocionTarjetaBines();
                        }
                        else
                        {
                            // If 'No', do something here.

                        }

                        // MessageBox.show("Tarjeta no coincide con la utilizada anteriormente para aplicar promoción");

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string enmascararTarjeta(string numTarjeta)
        {
            string numeroTarjeta = "";
            try
            {


                for (int i = 0; i < numTarjeta.Length; i++)
                {
                    if (i >= 6 && i < numTarjeta.Length - 3)
                    {
                        numeroTarjeta += 'X';
                    }
                    else
                    {
                        numeroTarjeta += numTarjeta[i];
                    }

                }
            }
            catch (Exception exkp)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "enmascararTarjeta", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exkp), "StackTrace: " + exkp.StackTrace);
            }

            return numeroTarjeta;
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {

                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (EsManual == 1)
                    {
                        txtNoTarjeta.Focus();
                    }

                }

            }
            catch (Exception exl)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "txtValor_KeyPress", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exl), "StackTrace: " + exl.StackTrace);
            }
        }

        private void txtNoTarjeta_Enter(object sender, EventArgs e)
        {
            /* try { 
                 original = InputLanguage.CurrentInputLanguage;
                 var culture = System.Globalization.CultureInfo.GetCultureInfo("eng");
                 var language = InputLanguage.FromCulture(culture);
                 if (InputLanguage.InstalledInputLanguages.IndexOf(language) >= 0)
                     InputLanguage.CurrentInputLanguage = language;
                 else
                     InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;

             }
             catch (Exception exkp)
             {
                 Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_Enter", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exkp), "StackTrace: " + exkp.StackTrace);
             }*/
            /* try { 

             if (InputLanguage.CurrentInputLanguage.Culture.EnglishName.ToUpper() != "ENGLISH (UNITED STATES)")
             {
                 foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
                 {
                     if (lang.Culture.EnglishName.ToUpper() == "ENGLISH (UNITED STATES)")
                     {
                         InputLanguage.CurrentInputLanguage = lang;
                     }
                 }
             }

         }
              catch (Exception exkp)
              {
                  Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_Enter", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exkp), "StackTrace: " + exkp.StackTrace);
              }*/


        }

        private void txtNoTarjeta_Leave(object sender, EventArgs e)
        {
            try
            {
                InputLanguage.CurrentInputLanguage = original;

            }
            catch (Exception exl)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_Enter", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exl), "StackTrace: " + exl.StackTrace);
            }

        }

        private void txtNumAutorizacionVoucher_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //Only numbers & Alpha
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    var NumAutorizacionVoucher = txtNumAutorizacionVoucher.Text.Trim().Replace("_", "");
                    txtNumAutorizacionVoucher.Text = txtNumAutorizacionVoucher.Text.Replace("__", "");
                    if (NumAutorizacionVoucher.Length < 6)
                    {

                        //txtNumAutorizacionVoucher.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                        //txtNumAutorizacionVoucher.TextMaskFormat = MaskFormat.e;
                        //  txtNumAutorizacionVoucher.MaskType = Telerik.WinControls.UI.MaskType.None;
                        // txtNumAutorizacionVoucher.Clear();
                        // txtNumAutorizacionVoucher.Text = NumAutorizacionVoucher.PadLeft(6, '0');
                        //txtEstab.MaskType = Telerik.WinControls.UI.MaskType.Standard;
                    }

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtEstab_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void txtNumTransaccionVoucher_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //Only numbers & Alpha
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {

                    var NumTransaccionVoucher = txtNumTransaccionVoucher.Text.Trim().Replace("_", "");
                    txtNumTransaccionVoucher.Text = txtNumTransaccionVoucher.Text.Replace("__", "");

                    if (NumTransaccionVoucher.Length < 7)
                    {
                        //txtNumTransaccionVoucher.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                        // txtNumTransaccionVoucher.MaskType = Telerik.WinControls.UI.MaskType.None;
                        // txtNumTransaccionVoucher.Clear();
                        // txtNumTransaccionVoucher.Text = NumTransaccionVoucher.PadLeft(6, '0');
                        //txtEstab.MaskType = Telerik.WinControls.UI.ME:\evelasco\TFS\POS\DEV\POS\Resources\askType.Standard;
                    }

                    if (txtNumTransaccionVoucher.Text.Length >= 6)
                    {
                        txtNumAutorizacionVoucher.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtEstab_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void chkPedidoDomicilio_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {

        }

        private void btnServDomicilio_Click(object sender, EventArgs e)
        {
            //bool _esPedidoDomicilio = false;
            try
            {
                SearchPedidoDomicilio BusqPedidoDomicilio = new SearchPedidoDomicilio(_factura.ClienteIdentificacion);
                BusqPedidoDomicilio.ShowDialog();
                _esPedidoDomicilio = BusqPedidoDomicilio.EsPedidoDomicilio();
                if (!_esPedidoDomicilio)
                {
                    //chkPedidoDomicilio.Checked = false;
                    cmbBancoTarjeta.Enabled = false;
                    cmbTipoTransaccion.Enabled = false;
                    txtNumAutorizacionVoucher.Enabled = true;
                    txtNumTransaccionVoucher.Enabled = true;
                }
                else
                {
                    cmbBancoTarjeta.Enabled = true;
                    cmbTipoTransaccion.Enabled = true;
                    _ordenApp = BusqPedidoDomicilio.NumeroPedidoDomicilio;
                    txtNumAutorizacionVoucher.Enabled = false;
                    txtNumTransaccionVoucher.Enabled = false;
                }

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "btnServDomicilio_Click", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void cmbTipo_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (cmbTipo.SelectedIndex != -1)
            {
                bool carga_bancotarjeta = true;
                //Validar si es pago manual y el usuario no pasó la tarjeta por el lector universa, entonces mostrar mensaje de observación. esto con el objetivo de que utilicen el lector universal para los pagos manuales y solo utilicen la seleccion de combos cuando sea pedido a domicilio.
                if (EsManual == 1 && !tarjetaDetectadaPagoManual)
                {
                    Control.Common.General.GetMensajeToList(212);

                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " Recuerde que debe seleccionar el Tipo Adecuado ya que está utilizando la opción Pago Manual.?", " POS - Pago Manual ");

                    ////MessageBox.Show(this, "Recuerde que debe seleccionar el Tipo Adecuado ya que está utilizando la opción Pago Manual.", "Notificación del Sistema para Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MsgBox m2 = new MsgBox("info", "Recuerde que debe seleccionar el Tipo Adecuado ya que está utilizando la opción Pago Manual.?", "Notificación del Sistema para Pago Manual");
                    //DialogResult dg2 = m2.ShowDialog();
                }

                try
                {
                    var db = new POSEntities();
                    if ((db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE") && (cmbTipo.SelectedValue != null))
                    {
                        try
                        {
                            //Validación cuando es por pago manual no corresponde al tipo pos_tarjeta_transaccion.
                            pos_tarjeta_transaccion objtTransaccion = (pos_tarjeta_transaccion)cmbTipo.SelectedValue;
                        }
                        catch (Exception ex1)
                        {
                            carga_bancotarjeta = false;
                        }

                        if (carga_bancotarjeta)
                        {
                            cmbTipoPago.SelectedIndex = -1;
                            cmbTipoPago.DataSource = db.pos_tarjeta_tipopago.Where(x => x.idTipo == ((pos_tarjeta_transaccion)cmbTipo.SelectedValue).idTipo).ToList();
                            cmbTipoPago.DataMember = "idPago";
                            cmbTipoPago.DisplayMember = "NombreTipoPago";

                            switch (((pos_tarjeta_transaccion)cmbTipo.SelectedValue).idTipo)
                            {
                                case "02":
                                    boxDiferido.Top = 390;
                                    boxDiferido.Visible = true;
                                    break;
                                default:
                                    boxDiferido.Visible = false;
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "cmbTipoTransaccion_SelectedIndexChanged", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                }
            }
        }

        private bool EsCampoEditable(System.Windows.Forms.Control control)
        {
            return control is TextBox || control is ComboBox ||
                   control is RichTextBox || control is MaskedTextBox;
        }

        private bool EsTeclaControl(Keys key)
        {
            return (key & Keys.KeyCode) != key;
        }

        private char? ConvertKeyToChar(Keys key)

        {
            if (key >= Keys.D0 && key <= Keys.D9) return (char)('0' + (key - Keys.D0));
            if (key >= Keys.NumPad0 && key <= Keys.NumPad9) return (char)('0' + (key - Keys.NumPad0));
            if (key >= Keys.A && key <= Keys.Z) return (char)('A' + (key - Keys.A));
            return null;
        }



        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            var focused = this.ActiveControl;

            // Permitir que txtCuenta maneje sus teclas
            if (focused == txtCuenta && txtCuenta.Visible)
            {
                if (keyData == Keys.Enter)
                {
                    btnVerificar_Click(this, EventArgs.Empty);
                    return true; // 🔥 Consumido
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // Evitar interferir con otros controles editables
            if (EsCampoEditable(focused))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            // Manejar Escape
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            //// Acumular teclas alfanuméricas
            //if (!EsTeclaControl(keyData))
            //{
            //    char? ch = ConvertKeyToChar(keyData);
            //    if (ch.HasValue)
            //    {
            //        _barcodeBuffer.Append(ch.Value);
            //        _resetTimer.Stop();
            //        _resetTimer.Start();
            //        return true; // 🔥 Consumido
            //    }
            //}



            // Manejar Enter: código escaneado
            if (keyData == Keys.Enter)
            {
                _resetTimer.Stop();
                if (_barcodeBuffer.Length > 0)
                {
                    string codigo = _barcodeBuffer.ToString().Trim();
                    _barcodeBuffer.Clear();

                    if (txtCuenta.Visible)
                    {
                        txtCuenta.Text = codigo;
                        btnVerificar_Click(this, EventArgs.Empty);
                    }
                    return true; // 🔥 Consumido
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
