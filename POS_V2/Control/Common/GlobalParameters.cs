using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Control.Main.MainTouch;
using POS.Control.Main.MainTouchClte;
using POS.Models;

namespace POS.Control.Common
{

    public static class GlobalParameters
    {
        public static AppData DataForFingerprint { get; set; }
        public static string DecriptedPwd { get; set; }
        public static string Establecimiento { get; set; }
        public static string EstablecimientoAxCode { get; set; }
        public static string EstablecimientoNombre { get; set; }
        public static string EstablecimientoDireccion { get; set; }
        public static string EstablecimientoTelefono { get; set; }
        public static string PuntoEmision { get; set; }
        public static string IpMaquina { get; set; }
        public static string Usuario { get; set; }
        public static string UsuarioNombre { get; set; }
        public static User UserObj { get; set; }
        public static string SelectedServerIp { get; set; }
        public static string Memo { get; set; }
        private static bool _mustCloseApplication = false;

        private static bool _acumulaPromoAx = false;
        public static string ipServerSelect { get; set; }
        public static string ipServerPedAPP { get; set; }
        public static string srvPrincipal { get; set; }
        public static string UserServerSelect { get; set; }
        public static string UserServerPedAPP { get; set; }
        public static string PaswServerSelect { get; set; }
        public static string PaswServerPedAPP { get; set; }
        public static decimal PromoValorFactItemValor { get; set; }
        public static int PromoValorFactItemIdTick { get; set; }
        public static int PromoValorFactItemCF { get; set; }
        public static bool PermitirPromocionTicketSinItem { get; set; }
        public static bool MustCloseApplication
        {
            get { return _mustCloseApplication; }
            set { _mustCloseApplication = value; }
        }

        public static bool acumulaPromoAx
        {
            get { return _acumulaPromoAx; }
            set { _acumulaPromoAx = value; }
        }
        public static bool TieneAperturaCajaManual { get; set; } //eevv 
        public static string VoucherInsertPath { get; set; } // eevv 2020-01-13
        public static string ConsumoBilleteraInsertPath { get; set; }
        public static string ConsumoGiftCardInsertPath { get; set; }
        public static string ConsumoCuponAppInsertPath { get; set; }
        public static string xulrunnerPath { get; set; }
        public static string ConServerPuntos { get; set; }
        public static string ConServerMkt { get; set; }
        public static string Linkbtn3Proveedor { get; set; }
        public static string Labelbtn3Proveedor { get; set; }
        public static bool VisibleFormaPagoRetencion { get; set; }
        public static bool VisibleDevolucionDineroRetencionFisica { get; set; }
        public static bool PresentarMensajeServidorIncorrecto { get; set; }
        public static bool PreguntarSiImprimeDatoRetencion { get; set; }
        public static bool ImprimeDatoRetencion { get; set; }
        public static bool DatoRetencionEnFactura { get; set; }
        public static decimal MontoVoucherTarjetaCreditoSinFirma { get; set; }
        public static decimal MontoDiferenciaPermitidaEnDevolucionRetencion { get; set; }
        public static bool CopiaVoucherTarjetaCreditoSinFirma { get; set; }
        public static string DBIdCaja { get; set; }
        public static string DBIdCajaLocal { get; set; }

        public static string ListaProductos_Path { get; set; }
        public static string ListaProductos_Path_Valida { get; set; }
        public static string ListaProductos_File { get; set; }
        public static string checkRevisionesRet { get; set; }
        public static string checkRevisionGeneralRet { get; set; }

        public static string NRuc { get; set; }
        public static string TipoAmbiente { get; set; }
        public static string TipoEmision { get; set; }

        public static bool ActivarFormaPagoTC { get; set; }

        public static string AplicaPromoMayor { get; set; }

        public static bool EsAmbienteProduccion { get; set; }
        public static List<string> ListaIpPermitidasAmbienteDev { get; set; }

        public static int IvaPorc { get { return 12; } }

        public static string IdConsumidorFinal { get { return "9999999999999"; } }

        public static string NameConsumidorFinal { get { return "Consumidor Final"; } }

        public static int CantidadDecimalesBascula { get { return 3; } }

        public static string ComprobanteVoucherTarjetaCredito { get { return "T_VOUCHER_TARJETA_CREDITO" /* "VOUCHER_TARJETA_CREDITO" */ ; } }
        public static string ComprobanteVoucherTarjetaCreditoSinFirma { get { return "T_VOUCHER_TARJETA_CREDITO_SIN_FIRMA" /* "VOUCHER_TARJETA_CREDITO" */ ; } }

        public static string ComprobanteVoucherAnula { get { return "T_VOUCHER_ANULA" /* "VOUCHER_ANULA" */ ; } }

        public static string ComprobanteFactura { get { return "T_FACTURA" /* "FACTURA_ALT" "FACTURA_E" */ ; } }
        public static string ComprobanteNota_Credito { get { return "NOTA_CREDITO"; } }
        public static string ComprobanteNota_Credito_Dev_Iva { get { return "NOTA_CREDITO_DEV_IVA"; } }
        public static Boolean NCConsumidorFinal { get; set; }
        public static string ComprobanteGiftCard { get { return "GIFTCARD"; } }

        private static string _masterPassword = "test";
        public static string MasterPassword { get { return _masterPassword; } set { _masterPassword = value; } }

        private static string _masterUser = "1234";
        public static string MasterUser { get { return _masterUser; } set { _masterUser = value; } }

        public static int SensorInactividadSegundosIntervalo { get; set; }
        public static bool ActivaIntegracionPedidos { get; set; }
        public static string CiudadLocal { get; set; }

        public static List<string> PasaporteCaracteresValidos { get; set; }

        public static string RetIvaRuc { get; set; }
        public static string RetIvaTipo { get; set; }
        public static string RetIvaLeyenda { get; set; }
        public static string ValidaCashBack { get; set; }

        public static string XmlPagoCanjePuntos { get; set; }
        public static string XmlAcumulaPuntos { get; set; }
        public static bool PosEnServidorCorrecto { get; set; }

        public static bool BloqRetFte { get; set; }
        public static string BloqRetFteCodigo { get; set; }
        public static string BloqRetFteMsj { get; set; }
        public static int BloqRetFteAnio { get; set; }

        #region Productos

        public static string ProductoIdentificadorItemPeso { get; set; }
        public static string ProductoIdentificadorItemPesoAlt { get; set; }

        #endregion

        #region Pinpad

        public static string PinpadMsjAutorizadorNoValido { get; set; }

        #endregion

        #region Giftcard Venta


        private static bool _tienePermisoGiftcardVenta = false;

        public static bool TienePermisoGiftcardVenta
        {
            get { return _tienePermisoGiftcardVenta; }
            set { _tienePermisoGiftcardVenta = value; }
        }

        #endregion

        #region Notificaciones Mail

        public static Models.Mail.MailModel MailModelGeneral { get; set; }

        #endregion

        #region Parking

        private static bool _tienePermisoParking = false;

        public static bool Parking_TienePermiso
        {
            get { return _tienePermisoParking; }
            set { _tienePermisoParking = value; }
        }

        private static int _minutosGracia;
        public static int Parking_MinutosGracia
        {
            get { return _minutosGracia; }
            set { _minutosGracia = value; }
        }

        private static string _pathSalida;
        public static string Parking_PathSalida
        {
            get { return _pathSalida; }
            set { _pathSalida = value; }
        }

        private static string _WSSalida;
        public static string Parking_WSSalida
        {
            get { return _WSSalida; }
            set { _WSSalida = value; }
        }

        private static string _WSGetTicket;
        public static string Parking_WSGetTicket
        {
            get { return _WSGetTicket; }
            set { _WSGetTicket = value; }
        }

        private static string _pathIngreso;
        public static string Parking_PathIngreso
        {
            get { return _pathIngreso; }
            set { _pathIngreso = value; }
        }

        private static string _parkingReciboPerdida;
        public static string Parking_ReciboPerdida
        {
            get { return _parkingReciboPerdida; }
            set { _parkingReciboPerdida = value; }
        }

        private static string _parkingReciboPerdidaLeyenda;
        public static string ParkingReciboPerdidaLeyenda
        {
            get { return _parkingReciboPerdidaLeyenda; }
            set { _parkingReciboPerdidaLeyenda = value; }
        }

        private static string _parkingItemPerdidaTicket;
        public static string Parking_ItemPerdidaTicket
        {
            get { return _parkingItemPerdidaTicket; }
            set { _parkingItemPerdidaTicket = value; }
        }

        private static List<string> _parkingListaItemPerdida;
        public static List<string> Parking_ListaItemPerdida
        {
            get { return _parkingListaItemPerdida; }
            set { _parkingListaItemPerdida = value; }
        }

        private static List<string> _parkingListaItemParqueo;
        public static List<string> Parking_ListaItemParqueo
        {
            get { return _parkingListaItemParqueo; }
            set { _parkingListaItemParqueo = value; }
        }

        private static string _parkingHoraDesde;
        public static string Parking_HoraDesde
        {
            get { return _parkingHoraDesde; }
            set { _parkingHoraDesde = value; }
        }

        private static string _parkingHoraHasta;
        public static string Parking_HoraHasta
        {
            get { return _parkingHoraHasta; }
            set { _parkingHoraHasta = value; }
        }

        #endregion

        #region AppMovil

        public static string AppMovil_PrefijoUsaApp { get; set; }

        #endregion

        #region ColoresPOS

        //Primario para paneles
        // public static System.Drawing.Color Color_PanelBackground = System.Drawing.Color.FromArgb(121, 134, 60);
        //0, 88, 42
        public static System.Drawing.Color Color_PanelBackground = System.Drawing.Color.FromArgb(0, 88, 42);

        //Para fondo de las filas en los grid
        public static System.Drawing.Color Color_GridViewBackground = System.Drawing.Color.FromArgb(232, 229, 182);

        //Para celda seleccionada en los grid
        public static System.Drawing.Color Color_GridViewSelectedCell = System.Drawing.Color.FromArgb(209, 204, 114);

        //Para bordes de los grid
        public static System.Drawing.Color Color_GridViewBorder = System.Drawing.Color.FromArgb(209, 204, 114);

        //Para boton pulsado
        public static System.Drawing.Color Color_ButtonPulsed = System.Drawing.Color.FromArgb(171, 173, 35);

        //Para borde de botones
        public static System.Drawing.Color Color_ButtonBorder = System.Drawing.Color.FromArgb(209, 204, 114);

        //Para botones de Venta Rapida en MainWindow
        public static System.Drawing.Color Color_ButtonDirec = System.Drawing.Color.FromArgb(121, 134, 60);

        #endregion

        #region RetencionElectronica
        private static string _pathRetElectPDF;
        public static string RetencionElect_PathPDF
        {
            get { return _pathRetElectPDF; }
            set { _pathRetElectPDF = value; }
        }

        private static bool _tienePermisoRetencion = false;
        public static bool Retencion_TienePermiso
        {
            get { return _tienePermisoRetencion; }
            set { _tienePermisoRetencion = value; }
        }

        private static int _diasVigencia;
        public static int Retencion_DiasVigencia
        {
            get { return _diasVigencia; }
            set { _diasVigencia = value; }
        }

        private static int _diasVigenciaAdicional;
        public static int Retencion_DiasVigenciaAdicional
        {
            get { return _diasVigenciaAdicional; }
            set { _diasVigenciaAdicional = value; }
        }
        #endregion

        #region Tarjeta Empresarial
        private static bool _tienePermisoTarjetaEmpresa = false;
        public static bool TarjetaEmpresa_TienePermiso
        {
            get { return _tienePermisoTarjetaEmpresa; }
            set { _tienePermisoTarjetaEmpresa = value; }
        }
        #endregion

  

        private static string _deleteProductUsrAdm;
        public static string DeleteProductUsrAdm
        {
            get { return _deleteProductUsrAdm; }
            set { _deleteProductUsrAdm = value; }
        }

        private static bool _compraGratis;
        public static bool CompraGratis
        {
            get { return _compraGratis; }
            set { _compraGratis = value; }
        }

        private static string _monederoEtiquetaRecibo;
        public static string MonederoEtiquetaRecibo
        {
            get { return _monederoEtiquetaRecibo; }
            set { _monederoEtiquetaRecibo = value; }
        }

        private static decimal _monederoPorcentajeConsumo;
        public static decimal MonederoPorcentajeConsumo
        {
            get { return _monederoPorcentajeConsumo; }
            set { _monederoPorcentajeConsumo = value; }
        }

        private static bool _monederoActivo;
        public static bool MonederoActivo
        {
            get { return _monederoActivo; }
            set { _monederoActivo = value; }
        }
        private static bool _monederoConsumoActivo;
        public static bool MonederoConsumoActivo
        {
            get { return _monederoConsumoActivo; }
            set { _monederoConsumoActivo = value; }
        }

        private static bool _activaVersionPinPadMedianet;
        public static bool ActivaVersionPinPadMedianet
        {
            get { return _activaVersionPinPadMedianet; }
            set { _activaVersionPinPadMedianet = value; }
        }

        private static int _pinPadReceiveTimeout;
        private static int _pinPadSendTimeout;
        private static bool _posQuitaTopMost;

        public static int PinPadReceiveTimeout
        {
            get { return _pinPadReceiveTimeout; }
            set { _pinPadReceiveTimeout = value; }
        }
        public static int PinPadSendTimeout
        {
            get { return _pinPadSendTimeout; }
            set { _pinPadSendTimeout = value; }
        }

        public static bool PosQuitaTopMost
        {
            get { return _posQuitaTopMost; }
            set { _posQuitaTopMost = value; }
        }
        //PINPAD_RECEIVE_TIMEOUT

        public static string PinpadKeyDerechoTCPIP { get; internal set; }
        public static string PinpadKeyIzquierdoTCPIP { get; internal set; }
        public static string IPPinPad { get; internal set; }
        public static bool EstTcpIpPinpad { get; internal set; }
        public static int PuertoPinPad { get; internal set; }

        public static string LogoLocal { get; internal set; }
        public static string WallpaperLocal { get; internal set; }
        public static decimal DESC_PROMO_IVA { get; internal set; }
        public static bool PROMO_IVA { get; internal set; }
        public static decimal CUPO_CF { get; internal set; }
        public static decimal IVAGEN { get; internal set; }
        public static bool BUSCAR_DETALLE_IVA12_NC { get; internal set; }
        public static DateTime VIGENCIA_IVA12_SEGUN_FECHA_NC { get; internal set; }
        public static bool VALIDAR_VIGENCIA_IVA12 { get; internal set; }
        public static bool RECALCULAR_IVA12_X_PAGONC { get; internal set; }
        public static decimal IVA_ANTERIOR { get; internal set; }

        /*Datos PinPad */
        public static int USA_NUM_INPAD { get; set; }
        public static string IpPinPadMEDIANET { get; set; }
        public static int PuertoPinPadMEDIANET { get; set; }
        public static string IpPinPadDATAFAST { get; internal set; }
        public static int PuertoPinPadDataFast { get; set; }
        public static string IpPinPadAustro { get; set; }
        public static int PuertoPinPadAustro { get; set; }
        public static string MID_MEDIANET { get; internal set; }
        public static string TID_MEDIANET { get; internal set; }
        public static string MID_DATAFAST { get; internal set; }
        public static string TID_DATAFAST { get; internal set; }
        public static string MID_AUSTRO { get; internal set; }
        public static string TID_AUSTRO { get; internal set; }
        public static string CID { get; internal set; }
        public static int Autorizador { get; internal set; }
        public static Control.CajaPinpad.Modelo.ConexionContingente ConectContingente { get; set; }
        public static bool PINPAD_MULTIRED { get; internal set; }
        public static List<Precio> ListPrecioInit { get; internal set; }
        public static List<RutaImagen> ListWallPapers { get; set; }
        public static List<RutaImagen> ListWallPapersCltes { get; set; }
        public static int AutorizadorDefault { get; set; }
        public static bool VOUCHER_SIN_PINPAD { get; set; }
        public static string TituloApp { get; set; }
        public static bool USA_BALANZA { get; internal set; }
        public static bool PESO_MANUAL { get; internal set; }
        //public static string fontStyle { get; set; }
        //public static string fontFamily { get; set; }
        //public static float emSize { get; set; }
        public static FontFamily fontFamily { get; set; }
        public static FontStyle fontStyle { get; set; }
        public static float emSize { get; set; }

        public static List<Mensajes> ListMensaje { get; set; }
        public static decimal VALOR_MINIMO_RECARGA { get; set; }
        public static decimal VALOR_MAXIMO_RECARGA { get; set; }
        public static decimal VALOR_MINIMO_PAGO_SERV { get; set; }
        public static decimal VALOR_MAXIMO_PAGO_SERV { get; set; }
        public static bool FINGERPRINT_ALMACEN { get; set; }
        public static bool PANTALLA_CLIENTE { get; set; }
        public static frmMainTouchClte frmTouchClte { get; set; } = null;
        public static frmPromocionPantallaCliente frmPromocionPantallaCliente { get; set; } = null;
        public static int targetWidth { get; set; } = 1024;
        public static int targetHeight { get; set; } = 768;
        public static string DeviceName { get; set; }

        public static bool SRI_ACTIVAR_CLAVE_ACCESO { get; set; }
        public static string SRI_RUC { get; set; }
        public static string SRI_USUARIO_ADICIONAL { get; set; }
        public static string SRI_CLAVE_ADICIONAL { get; set; }
        public static string SRI_PIN_CLTE { get; set; }
        public static string SRI_METODO_ENCRYPT { get; set; }
        public static string SRI_URL_ACCESS_TOKEN { get; set; }
        public static string SRI_URL_DEV_INDV { get; set; }
        public static string SRI_PARAM_DEV_INDV { get; set; }
        public static string SRI_URL_DEV_INDV_ANULACION { get; set; }
        public static string SRI_PARAM_DEV_INDV_ANULACION { get; set; }
        public static string PIN_BENEFICIARIO { get; set; }
        public static string BEAER_TOCKEN { get; set; }
        public static Models.DevolucionIVA.RespuestaToken tokenResponse { get; set; }
        public static string SRI_TEXTO_APLICA_DEVOLUCION_IVA { get; set; }
        public static string SRI_TEXTO_REVERSO_DEVOLUCION_IVA { get; set; }
        public static bool SRI_DEVOLUCION_IVA { get; set; }
        public static int MIN_CARACTER_CODIGO_ART { get; set; }
        public static List<ProductoArticulo> ProductoArticuloList { get; set; }

        public static bool EsModoImpresionBankard = false;  // jchid descuento bankard

    }
}