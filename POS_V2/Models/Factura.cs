using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using POS.Control.PINPAD;
using Trx.Messaging;
using System.Xml;
using System.Data;
using System.Data.Entity.Core.Objects;
using POS.Control.Pagos;
using POS.Control;
using POS.Control.CajaPinpad.Modelo;
using POS.Models.SRI;
using System.Web.Instrumentation;

namespace POS.Models
{
    public class Factura : Interfaces.ISale
    {

        public enum TipoComprobante
        {
            FACTURA = 1,       // "01"
            NOTA_CREDITO = 4,  // "04"
            NOTA_DEBITO = 5,   // "05"
            LIQUIDACION_COMPRA = 3, // "03"
            GUIA_REMISION = 6, // "06"
            COMPROBANTE_RETENCION = 7 // "07"
            //COMPROBANTE_INGRESO = 7, // "07"
            //COMPROBANTE_EGRESO = 8,  // "08"
            //COMPROBANTE_ANTICIPOS = 9, // "09"
            //FACTURA_EXPORTACION = 10, // "10"
            //NOTA_CREDITO_EXPORTACION = 11, // "11"
            //NOTA_DEBITO_EXPORTACION = 12  // "12"
        }



        public const string DESCUENTO_FORMA_PAGO = "FormaPago";
        public const string DESCUENTO_DIVISION_FORMA_PAGO = "DIvisionFormaPago";
        public bool flagMain = false;
        public bool activeInvoiceWinner = false;
        string resource;
        public bool esComboPerfecto = false;
        public decimal total_combo = 0;
        public string mensaje_promo = "                                       ¡GRACIAS!";
        public Models.Parking.clsParking ObjParking { get; set; }
        public Models.Parking.clsParking ObjParkingLost { get; set; }
        public Control.WalletPoints.ClsCuponApp ObjCuponApp { get; set; }

        // Objeto de estado para el nuevo sistema de cupones (MODERNO), para aislarlo del sistema antiguo.
        public Control.WalletPoints.ClsCuponApp ObjCuponAppModerno { get; set; }

        string razon_social_matriz;

        public string Razon_social_matriz
        {
            get { return razon_social_matriz; }
            set { razon_social_matriz = value; }
        }

        string ruc_matriz;

        public string Ruc_matriz
        {
            get { return ruc_matriz; }
            set { ruc_matriz = value; }
        }

        string direccion_matriz;

        public string Direccion_matriz
        {
            get { return direccion_matriz; }
            set { direccion_matriz = value; }
        }

        string direccion_sucursal;

        public string Direccion_sucursal
        {
            get { return direccion_sucursal; }
            set { direccion_sucursal = value; }
        }


        bool _esEmpleadoLiris = false;

        public bool EsEmpleadoLiris
        {
            get { return _esEmpleadoLiris; }
            set { _esEmpleadoLiris = value; }
        }


        decimal _porcDsctoEmpleadoLiris = 0;
        public decimal porcDsctoEmpleadoLiris
        {
            get { return _porcDsctoEmpleadoLiris; }
            set { _porcDsctoEmpleadoLiris = value; }
        }

        string _numeroTarjetaEmpresa = string.Empty;

        public string NumeroTarjetaEmpresa
        {
            get { return _numeroTarjetaEmpresa; }
            set { _numeroTarjetaEmpresa = value; }
        }


        bool _esTarjetaCreditoInterno = false;

        public bool EsTarjetaCreditoInterno
        {
            get { return _esTarjetaCreditoInterno; }
            set { _esTarjetaCreditoInterno = value; }
        }

        bool _esTarjetaCreditoInternoAdicional = false;
        public bool EsTarjetaCreditoInternoAdicional
        {
            get { return _esTarjetaCreditoInternoAdicional; }
            set { _esTarjetaCreditoInternoAdicional = value; }
        }

        string _numeroTarjetaEmpresaAdicional = string.Empty;

        public string NumeroTarjetaEmpresaAdicional
        {
            get { return _numeroTarjetaEmpresaAdicional; }
            set { _numeroTarjetaEmpresaAdicional = value; }
        }


        bool _esUsoAppMovil = false;
        public bool EsUsoAppMovil
        {
            get { return _esUsoAppMovil; }
            set { _esUsoAppMovil = value; }
        }

        int _ordenApp = 0;
        public int OrdenApp
        {
            get { return _ordenApp; }
            set { _ordenApp = value; }
        }

        bool _esUsoCuponPromocional = false;

        public bool EsUsoCuponPromocional
        {
            get { return _esUsoCuponPromocional; }
            set { _esUsoCuponPromocional = value; }
        }

        bool _usoTarjetaCompraGratis = false;

        public bool usoTarjetaCompraGratis
        {
            get { return _usoTarjetaCompraGratis; }
            set { _usoTarjetaCompraGratis = value; }
        }

        decimal _cuponPromocionalPorcDesc = 0;

        public decimal CuponPromocionalPorcDesc
        {
            get { return _cuponPromocionalPorcDesc; }
            set { _cuponPromocionalPorcDesc = value; }
        }

        string _cuponPromocionalCodigo = "";
        public string CuponPromocionalCodigo
        {
            get { return _cuponPromocionalCodigo; }
            set { _cuponPromocionalCodigo = value; }
        }


        int _idFacturaPOS;

        public int IdFacturaPOS
        {
            get { return _idFacturaPOS; }
            set { _idFacturaPOS = value; }
        }


        string nombre_sucursal;

        public string Nombre_sucursal
        {
            get { return nombre_sucursal; }
            set { nombre_sucursal = value; }
        }

        string _recibo;

        public string Recibo
        {
            get { return _recibo; }
            set { _recibo = value; }
        }
        string _reciboCovid19;
        public string ReciboCovid19
        {
            get { return _reciboCovid19; }
            set { _reciboCovid19 = value; }
        }

        List<Voucher> _voucher;

        public List<Voucher> Voucher
        {
            get { return _voucher; }
            set { _voucher = value; }
        }

        string _recibocorte;
        public string ReciboCorte
        {
            get { return _recibocorte; }
            set { _recibocorte = value; }
        }
        string _recibocortelote;
        public string ReciboCorteLote
        {
            get { return _recibocortelote; }
            set { _recibocortelote = value; }
        }
        List<Cupones> _cupon;
        public List<Cupones> Cupon
        {
            get { return _cupon; }
            set { _cupon = value; }
        }

        List<Cupones> _cuponBines;
        public List<Cupones> CuponBines
        {
            get { return _cuponBines; }
            set { _cuponBines = value; }
        }


        bool usarScannerIntegrado;

        public bool UsarScannerIntegrado
        {
            get { return usarScannerIntegrado; }
            set { usarScannerIntegrado = value; }
        }

        bool usaBalanza;

        public bool UsaBalanza
        {
            get { return usaBalanza; }
            set { usaBalanza = value; }
        }

        string _PuertoBalanza;

        public string PuertoBalanza
        {
            get { return _PuertoBalanza; }
            set { _PuertoBalanza = value; }
        }

        string _MarcaBalanza;

        public string MarcaBalanza
        {
            get { return _MarcaBalanza; }
            set { _MarcaBalanza = value; }
        }

        DateTime _fecha_inicio_autorizacion;

        public DateTime Fecha_inicio_autorizacion
        {
            get { return _fecha_inicio_autorizacion; }
            set { _fecha_inicio_autorizacion = value; }
        }
        DateTime _fecha_fin_autorizacion;

        public DateTime Fecha_fin_autorizacion
        {
            get { return _fecha_fin_autorizacion; }
            set { _fecha_fin_autorizacion = value; }
        }

        string _establecimiento;

        public string Establecimiento
        {
            get { return _establecimiento; }
            set { _establecimiento = value; }
        }
        string _pto_emision;

        public string PtoEmision
        {
            get { return _pto_emision; }
            set { _pto_emision = value; }
        }
        long _secuencia;

        public long Secuencia
        {
            get { return _secuencia; }
            set { _secuencia = value; }
        }

        //Get/Set de lista tipo Promocion
        List<Promocion> _promocionesActuales;
        public List<Promocion> PromocionesActuales
        {
            get { return _promocionesActuales; }
            set { _promocionesActuales = value; }
        }

        string _documento;

        public string Documento
        {
            get { return _documento; }
            set { _documento = value; }
        }

        string _cliente_grupo;

        public string Cliente_grupo
        {
            get { return _cliente_grupo; }
            set { _cliente_grupo = value; }
        }

        string _cliente_codigo;

        public string Cliente_codigo
        {
            get { return _cliente_codigo; }
            set { _cliente_codigo = value; }
        }
        string _cliente_cedula_ruc;

        public string ClienteIdentificacion
        {
            get { return _cliente_cedula_ruc; }
            set { _cliente_cedula_ruc = value; }
        }
        string _cliente_nombre;

        public string Cliente_nombre
        {
            get { return _cliente_nombre; }
            set { _cliente_nombre = value; }
        }
        string _cliente_direccion;

        public string Cliente_direccion
        {
            get { return _cliente_direccion; }
            set { _cliente_direccion = value; }
        }
        string _cliente_telefono;

        public string Cliente_telefono
        {
            get { return _cliente_telefono; }
            set { _cliente_telefono = value; }
        }
        DateTime _fecha;

        public DateTime Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }
        decimal _subtotal;

        public decimal Subtotal
        {
            get { return _subtotal; }
            set { _subtotal = value; }
        }
        decimal _descuento;

        public decimal Descuento
        {
            get { return _descuento; }
            set { _descuento = value; }
        }

        decimal _descuento2;

        public decimal Descuento2
        {
            get { return _descuento2; }
            set { _descuento2 = value; }
        }
        //Variable que aloja el descuento de ax
        decimal _descuentoAX;

        public decimal DescuentoAX
        {
            get { return Math.Round(_descuentoAX, 2); }
            set { _descuentoAX = value; }
        }

        //eevv .ini
        bool _aplicadescuentopromobines;
        public bool AplicaDescuentoPromoBines //eevv
        {
            get { return _aplicadescuentopromobines; }
            set { _aplicadescuentopromobines = value; }
        }

        decimal _descuentoPromoBines; //eevv 
        public decimal DescuentoPromoBines //eevv
        {
            get { return Math.Round(_descuentoPromoBines, 2); }
            set { _descuentoPromoBines = value; }
        }

        string _numerobinetarjetapromo; //eevv 
        public string BinNumeroTarjetaPromo //eevv
        {
            get { return _numerobinetarjetapromo; }
            set { _numerobinetarjetapromo = value; }
        }
        //eevv .fin

        string _clave_acceso_sri;

        public string ClaveAccesoSRI
        {
            get { return _clave_acceso_sri; }
            set { _clave_acceso_sri = value; }
        }

        decimal _iva;

        public decimal Iva
        {
            get { return _iva; }
            set { _iva = value; }
        }
        decimal _total;

        public decimal Total
        {
            get { return _total; }
            set { _total = value; }
        }
        decimal _base_imponible;

        public decimal Base_imponible
        {
            get { return _base_imponible; }
            set { _base_imponible = value; }
        }
        decimal _no_base_imponible;

        public decimal No_base_imponible
        {
            get { return _no_base_imponible; }
            set { _no_base_imponible = value; }
        }
        string _autorizacion;

        public string Autorizacion
        {
            get { return _autorizacion; }
            set { _autorizacion = value; }
        }
        string _ip_address;

        public string Ip_address
        {
            get { return _ip_address; }
            set { _ip_address = value; }
        }

        decimal _cambio;

        public decimal Cambio
        {
            get { return _cambio; }
            set { _cambio = value; }
        }

        string _pto_emision_origen;

        public string PtoEmisionOrigen
        {
            get { return _pto_emision_origen; }
            set { _pto_emision_origen = value; }
        }

        Models.User _user;

        public Models.User User
        {
            get { return _user; }
            set { _user = value; }
        }

        Models.Retencion _retencion;

        public Models.Retencion Retencion
        {
            get { return _retencion; }
            set { _retencion = value; }
        }


        BindingList<Producto> _productos;


        public BindingList<Producto> Productos
        {
            get { return _productos; }
            set { _productos = value; }
        }

        BindingList<Pago> _pagos;

        public BindingList<Pago> Pagos
        {
            get { return _pagos; }
            set { _pagos = value; }
        }

        public BindingList<Pago> TmpGiftPagos
        {
            get { return _pagos; }
            protected set { _pagos = value; }
        }

        List<DescuentoAdicional> _descuentos2;

        public List<DescuentoAdicional> Descuentos2
        {
            get { return _descuentos2; }
            set { _descuentos2 = value; }
        }

        //eevv .ini 
        core_tarjetacreditointerno _tarjetaCreditoInterno;
        TarjetaRegalo _tarjetaRegalo;
        TblPedido _tblPedido;
        bool _esPedidoOtraApp;
        string _delivery;
        string _deliveryFormaPago;

        public bool EsPedidoOtraApp
        {
            get { return _esPedidoOtraApp; }
            set { _esPedidoOtraApp = value; }
        }

        public TblPedido PedidoOtraApp
        {
            get { return _tblPedido; }
            set { _tblPedido = value; }
        }

        private bool _acumulaBilletera;
        public bool AcumulaBilletera
        {
            get { return _acumulaBilletera; }
            set { _acumulaBilletera = value; }
        }



        public string Delivery
        {
            get { return _delivery; }
            set { _delivery = value; }
        }

        public string DeliveryFormaPago
        {
            get { return _deliveryFormaPago; }
            set { _deliveryFormaPago = value; }
        }

        //eevv .fin 

        public core_tarjetacreditointerno TarjetaCreditoInterno
        {
            get { return _tarjetaCreditoInterno; }
            set { _tarjetaCreditoInterno = value; }
        }

        core_tarjetacreditointerno _tarjetaCreditoInternoAdicional;

        public core_tarjetacreditointerno TarjetaCreditoInternoAdicional
        {
            get { return _tarjetaCreditoInternoAdicional; }
            set { _tarjetaCreditoInternoAdicional = value; }
        }

        bool _EsClienteApp;
        public bool EsClienteApp
        {
            get { return _EsClienteApp; }
            set { _EsClienteApp = value; }
        }

        string _codigoclienteAPP;
        public string codigoclienteAPP
        {
            get { return _codigoclienteAPP; }
            set { _codigoclienteAPP = value; }
        }

        decimal _saldoPuntos;

        public decimal SaldoPuntos
        {
            get { return _saldoPuntos; }
            set { _saldoPuntos = value; }
        }

        //Variables para proceso contingencia PINPAD
        public DateTime FechaInicioEspera { get; set; }
        public DateTime FechaFinEspera { get; set; }
        public int IdentificaAutorizadorDefault { get; set; }
        public bool ValidaContingenciaPinPad { get; set; }
        public string CodigoClienteApp { get; set; }
        public string pinClteDevolucion { get; set; }

        public bool _aplicaBeneficioDevolucionIVA = false;
        public bool aplicaBeneficioDevolucionIVA
        {
            get { return _aplicaBeneficioDevolucionIVA; }
            set { _aplicaBeneficioDevolucionIVA = value; }
        }

        public bool _esBeneficiarioDevolucionIVA = false;
        public bool esBeneficiarioDevolucionIVA
        {
            get { return _esBeneficiarioDevolucionIVA; }
            set { _esBeneficiarioDevolucionIVA = value; }
        }
        public decimal _montoIvaDevolver = 0;
        public decimal montoIvaDevolver
        {
            get { return _montoIvaDevolver; }
            set { _montoIvaDevolver = value; }
        }





        public Control.CajaPinpad.Modelo.ConexionContingente ConectContingente { get; set; }



        public Factura()
        {
            this.Productos = new BindingList<Producto>();
            this.Pagos = new BindingList<Pago>();
            this.Descuentos2 = new List<DescuentoAdicional>();
            this.Voucher = new List<Voucher>();
            this.Cupon = new List<Cupones>();
            this.Retencion = new Retencion();
            this._tblPedido = new TblPedido();  //eevv 

            if (MainWindow.establecimiento_inicio != null)
            {
                this._establecimiento = MainWindow.establecimiento_inicio;
                configurarPromociones();
                //this.PromocionesActuales = MainWindow.listPromociones.ToList();
            }

        }

        //Obtener promociones de vista vwDescuentosCabecera
        public void configurarPromociones()
        {
            this.PromocionesActuales = getPromociones(MainWindow.establecimiento_inicio);
        }

        //Metodo para obtener RecId de la cabecera de los descuentos
        List<Promocion> getPromociones(string _establecimiento)
        {
            List<Promocion> list = new List<Promocion>();
            using (var db = new POSEntities())
            {
                try
                {
                    foreach (var vw_cabecera in (new Promocion().getCabeceraPorDiaHoy(db, _establecimiento)))
                    {
                        Promocion promo = new Promocion();
                        promo.Establecimiento = vw_cabecera.ALMACEN;
                        promo.Tipo = vw_cabecera.TIPODESCUENTO;
                        promo.Descripcion = vw_cabecera.DESCRIPCION;
                        promo.FechaDesde = (DateTime)vw_cabecera.FECHADESDE;
                        promo.FechaHasta = (DateTime)vw_cabecera.FECHAHASTA;
                        promo.Estado = vw_cabecera.ESTADO;
                        promo.RecId = vw_cabecera.RECID;
                        promo.ListProductos = new List<Producto>();

                        var param = db.core_parametro.Where(x => x.identificador == "PROMOAX_MAXCANTDSCTO"
                                                                    &&
                                                                    x.valor == promo.RecId.ToString()).FirstOrDefault();
                        if (param != null)
                        {
                            decimal maxCantDscto;
                            if (decimal.TryParse(param.parametro2, out maxCantDscto))
                            {
                                //La cantidad maxima debe ser configurada con un valor superior a cero
                                if (maxCantDscto > 0) promo.MaxCantidadDscto = maxCantDscto;
                            }

                            promo.EsRestrictiva = (param.documento == "1");
                        }


                        var param2 = db.core_parametro.Where(x => x.identificador == "PROMOAX_MAXCANTDSCTOG"
                                                                    &&
                                                                    x.valor == promo.RecId.ToString()).FirstOrDefault();
                        if (param2 != null)
                        {
                            decimal maxCantDscto;
                            if (decimal.TryParse(param2.parametro2, out maxCantDscto))
                            {
                                promo.GeneralPromo = true;
                                //La cantidad maxima debe ser configurada con un valor superior a cero
                                if (maxCantDscto > 0) promo.MaxCantidadDscto = maxCantDscto;


                            }

                            promo.EsRestrictiva = (param2.documento == "1");
                        }

                        list.Add(promo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No fue posible obtener la lista de promociones para estas fechas. Contacte al administrador");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "getPromociones", "No fue posible obtener la lista de promociones para estas fechas, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                }
            }

            return list;
        }


        public bool validar_pagos(out decimal cambio)
        {
            cambio = 0m;
            this.Cambio = 0m;
            this.CalcularPagos();
            if (this.getPagosNoEfectivo() > this.GetTotal())
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

        public decimal getSubTotal()
        {

            decimal SubTotal0 = this.Productos.Where(x => x.IvaProducto <= 0).Sum(x => x.Subtotal);
            decimal SubTotal12 = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == false).Sum(x => x.Subtotal);
            decimal SubTotal12PromoIvaExcluye = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == true).Sum(x => x.Subtotal);

            //////////PROMO IVA////////////////////////
            string AplicaPromo;
            decimal PorcPromo;
            using (var db = new POSEntities())
            {
                // hdhdheh
                // AplicaPromo = ((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").valor));
                PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;
            }
            if (POS.Control.Common.Promo.EsDiaPromoIVA() && PorcPromo > 0)
            {
                SubTotal12 = SubTotal12 - ((SubTotal12 * PorcPromo) / 100M);
            }

            SubTotal12 += SubTotal12PromoIvaExcluye;
            ////////////////////////////////////////////

            return decimal.Round(SubTotal12 + SubTotal0, 2);
        }

        /// <summary>
        /// Obtiene el valor de la retencion de la Fuente de la Factura 1%.
        /// </summary>
        /// <returns></returns>
        public decimal getSubTotalRetFte1()
        {
            decimal SubTotal0 = 0;
            decimal SubTotal12 = 0;
            decimal SubTotal12PromoIvaExcluye = 0;
            decimal porcrtencion = 1M;
            try
            {
                SubTotal0 = this.Productos.Where(x => x.IvaProducto <= 0 && x.RetencionPorcentaje == porcrtencion).Sum(x => x.Subtotal);
                SubTotal12 = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == false && x.RetencionPorcentaje == porcrtencion).Sum(x => x.Subtotal);
                SubTotal12PromoIvaExcluye = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == true && x.RetencionPorcentaje == porcrtencion).Sum(x => x.Subtotal);


                //////////PROMO IVA////////////////////////
                string AplicaPromo;
                decimal PorcPromo;
                using (var db = new POSEntities())
                {
                    //   AplicaPromo =  ((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").valor));
                    PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;// Decimal.Parse((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));
                }
                if (POS.Control.Common.Promo.EsDiaPromoIVA() && PorcPromo > 0)
                {
                    SubTotal12 = SubTotal12 - ((SubTotal12 * PorcPromo) / 100M);
                }

                SubTotal12 += SubTotal12PromoIvaExcluye;
                ////////////////////////////////////////////

                return decimal.Round(SubTotal12 + SubTotal0, 2);
            }
            catch (Exception)
            {

                throw;
            }

            return 0;
        }

        /// <summary>
        /// Obtiene el valor de la retencion de la Fuente de la Factura 1.75%.
        /// </summary>
        /// <returns></returns>
        public decimal getSubTotalRetFte175()
        {
            decimal SubTotal0 = 0;
            decimal SubTotal12 = 0;
            decimal SubTotal12PromoIvaExcluye = 0;
            decimal porcrtencion = 1.75M;
            try
            {
                SubTotal0 = this.Productos.Where(x => x.IvaProducto <= 0 && x.RetencionPorcentaje == porcrtencion).Sum(x => x.Subtotal);
                SubTotal12 = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == false && x.RetencionPorcentaje == porcrtencion).Sum(x => x.Subtotal);
                SubTotal12PromoIvaExcluye = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == true && x.RetencionPorcentaje == porcrtencion).Sum(x => x.Subtotal);


                //////////PROMO IVA////////////////////////
                string AplicaPromo;
                decimal PorcPromo;
                using (var db = new POSEntities())
                {

                    //AplicaPromo = ((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").valor));
                    PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;// Decimal.Parse((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));
                }
                if (POS.Control.Common.Promo.EsDiaPromoIVA() && PorcPromo > 0)
                {
                    SubTotal12 = SubTotal12 - ((SubTotal12 * PorcPromo) / 100M);
                }

                SubTotal12 += SubTotal12PromoIvaExcluye;
                ////////////////////////////////////////////

                return decimal.Round(SubTotal12 + SubTotal0, 2);
            }
            catch (Exception)
            {

                throw;
            }

            return 0;
        }
        public decimal getDescuentoPromocionesAX()
        {
            return decimal.Round(this.Productos.Sum(x => x.DescuentoAX), 2);
        }

        public decimal getSubTotalSinDescuento()
        {
            return decimal.Round(this.Productos.Sum(x => x.SubtotalSinDescuento), 2);
        }

        public decimal GetBase0()
        {
            return decimal.Round(this.Productos.Where(x => x.Iva == 0).Sum(x => x.SubtotalSinDescuento), 2);
        }

        public decimal GetBase12()
        {
            return decimal.Round(this.Productos.Where(x => x.Iva > 0).Sum(x => x.SubtotalSinDescuento), 2);
        }

        public decimal GetBase12Desc()
        {
            return decimal.Round(this.Productos.Where(x => x.Iva > 0).Sum(x => x.Subtotal), 2);
        }

        public decimal GetBase12DescPromoIVA(bool verExcluidos)
        {
            return decimal.Round(this.Productos.Where(x => x.Iva > 0 && x.EsExcluidoPromoIVA == verExcluidos).Sum(x => x.Subtotal), 2);
        }

        public decimal GetDescuentos()
        {
            return decimal.Round(this.Productos.Sum(x => x.Descuento), 2);
        }

        public decimal getDescuentos0()
        {
            return decimal.Round(this.Productos.Where(x => x.Iva == 0).Sum(x => x.Descuento), 2);
        }

        public decimal getDescuentos2()
        {
            if (this.Descuentos2.Count > 0)
                return decimal.Round(this.Descuentos2.Sum(x => x.Valor), 2, MidpointRounding.AwayFromZero);
            else
                return 0M;
        }

        public decimal getIVA(bool debeSumarPromoIVAExcluidos = true)
        {

            var calculo_iva = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == false).Sum(x => x.Subtotal);
            var calculo_ivaPromoIvaExcluye = this.Productos.Where(x => x.IvaProducto > 0 && x.EsExcluidoPromoIVA == true).Sum(x => x.Subtotal);
            var sub = getSubTotal();

            //////////PROMO IVA////////////////////////
            string AplicaPromo;
            decimal PorcPromo;
            using (var db = new POSEntities())
            {
                // AplicaPromo = ((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").valor));
                PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;
            }
            if (POS.Control.Common.Promo.EsDiaPromoIVA() && PorcPromo > 0)
            {
                calculo_iva = calculo_iva - ((calculo_iva * PorcPromo) / 100M);
            }

            if (debeSumarPromoIVAExcluidos) calculo_iva += calculo_ivaPromoIvaExcluye;
            //////////////////////////////////////

            if (calculo_iva > 0)
            {
                decimal porc_iva;
                using (var db = new POSEntities())
                {
                    porc_iva = ((decimal.Parse(db.core_parametro.First(x => x.identificador == "IVA").valor) / 100));
                }

                if (Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC)
                {
                    porc_iva = (Control.Common.GlobalParameters.IVA_ANTERIOR / 100);
                }




                return decimal.Round((calculo_iva - ((((this.Descuentos2.Count > 0) ? this.getDescuentos2() : this.Descuento2) / sub) * calculo_iva)) * porc_iva, 2);



                /*
                 * (this.Descuentos2.Count > 0) ? decimal.Round(this.Descuentos2.Max(x => x.Valor), 2, MidpointRounding.AwayFromZero): this.Descuento2
                if (!POS.Control.Common.Promo.EsPromoIVA(_establecimiento))
                {
                    var sumaIvaLineas = this.Productos.Where(x => x.IvaProducto > 0).Sum(x => x.Iva);
                    calculo_iva = decimal.Round(sumaIvaLineas - (((this.getDescuentos2() / sub) * calculo_iva) * porc_iva), 2);
                }
                else
                {
                    calculo_iva = decimal.Round((calculo_iva - ((this.getDescuentos2() / sub) * calculo_iva)) * porc_iva, 2);
                }

                return calculo_iva;  
                */
            }
            else
            {
                return 0M;
            }
        }

        public decimal GetPromoIva()
        {

            //////////PROMO IVA////////////////////////
            //  string AplicaPromo;
            decimal PorcPromo;
            decimal porc_iva;
            using (var db = new POSEntities())
            {

                // AplicaPromo = ((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").valor));
                PorcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;
                porc_iva = ((Control.Common.GlobalParameters.IVAGEN / 100));
            }
            //////////////////////////////////////
            if (POS.Control.Common.Promo.EsDiaPromoIVA() && PorcPromo > 0)
            {
                return decimal.Round(((GetBase12DescPromoIVA(false) + (GetBase12DescPromoIVA(false) * porc_iva)) * PorcPromo) / 100M, 2);

            }
            else
            {
                return 0M;
            }
        }

        public decimal GetTotal()
        {
            decimal total_fact = 0;

            total_fact = Math.Round(this.getSubTotal() - this.getDescuentos2() - this.Descuento2 + this.getIVA(), 2);

            if (this.aplicaBeneficioDevolucionIVA)
            {
                total_fact = total_fact - this.montoIvaDevolver;
                //this.aplicaBeneficioDevolucionIVA = false; // prueba de cambiar esta variable para inicializarla

            }

            if (esComboPerfecto)
            {
                return Math.Round(total_combo, 2);
            }
            else
            {
                return Math.Round(total_fact, 2);
            }



        }


        public decimal getPagos()
        {
            return this.Pagos.Sum(x => x.Valor);
        }

        public decimal getPagosNoEfectivo()
        {
            return this.Pagos.Where(x => x.Descripcion != "EFECTIVO").Sum(x => x.Valor);
        }

        public decimal getSaldo()
        {
            return this.GetTotal() - this.getPagos();
        }

        public decimal getSaldoDisplay()
        {
            var saldo = this.GetTotal() - this.getPagos();
            if (saldo <= 0)
            {
                return 0M;
            }
            else
            {
                return saldo;
            }
        }

        public void CalcularPagos()
        {
            foreach (var p in this.Pagos)
            {
                p.calcularTotal();
            }
        }

        public void agregarPagoEfectivo(decimal valor)
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

        #region Tarjeta Promocion Descuento Española - flara
        public void agregarPromocionEspanola(decimal descuento) //decimal valor, 
        {
            foreach (var p in this.Productos)
            {
                //p.DescuentoActual = 0;
                //p.Descuento = 0;
                //p.DescuentoAX = 0;
                //p.DescuentoLocal = 0;
                //p.calcularIVA(14);


                // p.Descuento= Math.Round((p.SubtotalSinDescuento * (descuento / 100)), 2, MidpointRounding.AwayFromZero);
                //p.calcularDescuento(Math.Round((p.SubtotalSinDescuento * (descuento / 100)), 2, MidpointRounding.AwayFromZero));
                if ((p.Descuento / p.SubtotalSinDescuento) * 100 < descuento)
                {
                    p.calcularDescuento(descuento);
                    p.update();
                }
            }


            /*  
              decimal valorDsct = Math.Round((this.getSubTotal() * (descuento / 100)), 2, MidpointRounding.AwayFromZero);
          //decimal valorDsct = decimal.Parse((this.getTotal() * (descuento / 100)).ToString("###,##0.00"));
          var pago = this.agregarPago("DSCT_PROMO", valorDsct);
          pago.Pagos.Add(new PagoDescuentoEspanola() { Codigo = "DSCT_PROMO", Valor = valorDsct });
          */
            //pago.calcularTotal();
        }
        public void agregarPromocionEspanola(decimal descuento, decimal saldoDisponible) //decimal valor, 
        {
            decimal prodPreDescuento = 0M, prodPreDescuentoActual = 0M;
            foreach (var p in this.Productos)
            {
                //Si no queda saldo de la tarjeta, salir del bucle
                if (saldoDisponible <= 0) break;


                prodPreDescuentoActual = decimal.Round((descuento / 100M), 2);
                prodPreDescuento = Math.Round(Math.Round(p.Pvp * p.Cantidad, 2) * (descuento / 100M), 2);

                //El descuento calculado no puede ser mayor que el saldo disponible
                if (prodPreDescuento > saldoDisponible)
                {
                    //Calculo para obtener un porcentaje proporcional que debe ir en DescuentoActual en relacion al saldoDisponible
                    prodPreDescuentoActual = decimal.Round(saldoDisponible / (p.Pvp * p.Cantidad), 2);
                    prodPreDescuento = saldoDisponible;
                }

                p.DescuentoActual = prodPreDescuentoActual;
                p.DescuentoAX = prodPreDescuento;
                p.update(false);

                p.DescuentoTarjetasCompraGratis += prodPreDescuento;

                saldoDisponible -= prodPreDescuento;

                /*if ((p.Descuento / p.SubtotalSinDescuento) * 100 < descuento)
                {
                }*/
            }
        }

        public void agregarPromocionTarjetaBines(decimal descuento) //decimal valor, 
        {
            decimal DescuentoAnterior;
            int bandera = 0;
            DescuentoAnterior = DescuentoPromoBines;
            descuento = decimal.Round((descuento / 100M), 2);

            // Aplica Dscto a productos parametrizados, sino existen aplica a toda la factura.  JM  19-11-2020
            using (var db = new POSEntities())
            {
                var productosdscto = db.core_parametro.Where(x => x.identificador == "DESCUENTO_BINES_TARJETA_PRODUCTOS" && x.valor == "TRUE").FirstOrDefault();

                if (productosdscto != null)
                {
                    var list_productos = productosdscto.documento.Split(';');
                    foreach (var p in this.Productos)
                    {
                        if (list_productos.Contains(p.Id))
                        {
                            if (bandera == 0)
                                bandera++;

                            p.DescuentoActual = (p.DescuentoActual - DescuentoPromoBines) + descuento;
                            p.TieneDescuentoPromoBines = true;
                            p.DescuentoPromoBines = descuento;
                            p.update();
                        }
                    }
                }
                else
                {
                    foreach (var p in this.Productos)
                    {
                        if (bandera == 0)
                            bandera++;

                        p.DescuentoActual = (p.DescuentoActual - DescuentoPromoBines) + descuento;
                        //p.Descuento = p.Descuento ==0? p.SubtotalSinDescuento * descuento : p.Descuento -( p.Descuento * decimal.Round((descuento / 100M), 2));
                        p.TieneDescuentoPromoBines = true;
                        p.DescuentoPromoBines = descuento;
                        p.update();
                    }
                }

                if (bandera > 0)
                {
                    DescuentoPromoBines = descuento;
                    AplicaDescuentoPromoBines = true;
                }
            }
        }

        public void quitarPromocionTarjetaBines() //decimal valor, 
        {
            int bandera = 0;

            foreach (var p in this.Productos)
            {
                if (bandera == 0)
                {
                    bandera++;
                }

                if (p.TieneDescuentoPromoBines)
                {
                    p.DescuentoActual = p.DescuentoActual > 0 ? (p.DescuentoActual - DescuentoPromoBines) : 0;
                    //p.Descuento = p.Descuento == 0 ? 0 : p.Descuento - (p.Descuento -(p.Descuento * DescuentoPromoBines));
                    p.TieneDescuentoPromoBines = false;
                    p.DescuentoPromoBines = 0;
                    this.AplicaDescuentoPromoBines = false;

                    p.update();
                }

            }
            this.DescuentoPromoBines = 0;


        }

        public decimal getDescuentoEspanola()
        {
            if (this.Pagos.Where(y => y.Descripcion == "DSCT_PROMO").Count() > 0)
                return Math.Round(this.Pagos.Where(x => x.Descripcion == "DSCT_PROMO").FirstOrDefault().Valor, 2, MidpointRounding.AwayFromZero);
            else
                return 0M;
        }

        #endregion

        public Pago AgregarPago(string tipo, decimal valor)
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

        Pago AgregarPago(string tipo, decimal valor, string numBin)
        {


            if (this.Pagos.Any(x => x.Descripcion == tipo))
            {
                var pago = this.Pagos.Single(x => x.Descripcion == tipo);
                pago.Valor = valor;
                pago.NumBin = numBin;

                return pago;
            }
            else
            {
                var nuevo = new Pago()
                {
                    Descripcion = tipo,
                    Valor = valor,
                    NumBin = numBin
                };
                this.Pagos.Add(nuevo);
                return nuevo;
            }
        }


        public void AgregarPagoTarjetaCredito(decimal valor, string banco, string nombre, string marca, string tipoPos, string numBin = "", string bin_descripcion = "N/A")
        {
            var pago = this.AgregarPago("T. CREDITO", valor, numBin);

            pago.Pagos.Add(new PagoTarjetaCredito()
            {
                Banco = banco,
                Codigo = nombre + " - " + marca,
                Nombre = nombre,
                Marca = marca,
                Valor = valor,
                TipoPos = tipoPos,
                NumeroBin = numBin,
                BinDescripcion = bin_descripcion
            });

            pago.calcularTotal();
        }

        public void AgregarPagoCheque(decimal valor, string banco, string numero, string cuenta)
        {
            var pago = this.AgregarPago("CHEQUE", valor);
            pago.Pagos.Add(new PagoCheque() { Banco = banco, Codigo = banco + " - " + cuenta + " - " + numero, Numero = numero, Cuenta = cuenta, Valor = valor });
            pago.calcularTotal();
        }

        public void AgregarPagoTarjetaRegalo(decimal valor, string codigo, decimal saldo, bool estaAsociadaGrupoCliente, string identificacionGrupoCliente, string nombreGrupoCliente, string tipo = "GIFT CARD")
        {
            var pago = this.AgregarPago(tipo, valor);
            var exitsGif = pago.Pagos.Any(x => x.Codigo == codigo);
            if (!exitsGif)
                pago.Pagos.Add(new PagoGiftCard()
                {
                    Codigo = codigo,
                    Valor = valor,
                    Saldo = saldo,
                    EstaAsociadaGrupoCliente = estaAsociadaGrupoCliente,
                    IdentificacionGrupoCliente = identificacionGrupoCliente,
                    NombreGrupoCliente = nombreGrupoCliente
                });


            pago.calcularTotal();
        }
        public void AgregarPagoNotaCredito(decimal valor, string codigo)
        {
            //Validar se aun se recalcula el iva al 12%
            if (Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC)
            {
                foreach (var item in this.Productos)
                {
                    if (item.Iva > 0M)
                    {
                        item.calcularIVA(Control.Common.GlobalParameters.IVA_ANTERIOR);
                    }
                }


            }

            var pago = this.AgregarPago("ANTCLIEN/C", valor);
            pago.Pagos.Add(new PagoNotaCredito() { Codigo = codigo, Valor = valor });
            pago.calcularTotal();
        }

        public void AgregarPagoTarjetaInterna(decimal valor, string codigo, string titularIdentificacion)
        {
            var pago = this.AgregarPago("TAR PORTAL", valor);
            pago.Pagos.Add(new PagoTarjetaInterna() { Codigo = codigo, Valor = valor, Titular = titularIdentificacion });
            pago.calcularTotal();
        }

        public void agregarPagoRetencion(decimal valor, string codigo)
        {
            var pago = this.AgregarPago("RETCLIENTE", valor);
            pago.Pagos.Add(new PagoRetencion() { Codigo = codigo, Valor = valor });
            pago.calcularTotal();
        }

        public void AgregarPagoMonedero(decimal valor)
        {
            var pago = this.AgregarPago("DINE ELECT", valor);
            pago.Pagos.Add(new PagoMonedero() { Valor = valor });
            pago.calcularTotal();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "AgregarPagoMonedero", "Se va a consumir el siguiente valor de dinero electronico :" + pago.Valor.ToString());//stefany
        }

        public void agregarPagoDelivery(decimal valor)
        {
            var pago = this.AgregarPago(this.DeliveryFormaPago, valor);
            pago.Pagos.Add(new PagoDelivery() { Codigo = this.DeliveryFormaPago, Valor = valor });
            pago.calcularTotal();

            /*if (this.Pagos.Any(x => x.Descripcion == "EFECTIVO"))
            {
                var pago = this.Pagos.Single(x => x.Descripcion == "EFECTIVO");
                pago.Valor = valor;
            }
            else
            {
                this.Pagos.Add(new Pago() { Descripcion = "EFECTIVO", Valor = valor });
            }*/
        }

        public void AgregaOrdenApp(string OrdenApp)
        {
            if (!string.IsNullOrEmpty(OrdenApp))
            {
                this._ordenApp = Convert.ToInt32(OrdenApp);
            }
            else
            {
                this._ordenApp = 0;
            }
        }

        public void AgregarDescuentoPagoCompraGratis(decimal valor, string codigo)
        {
            this.Descuentos2.Add(new DescuentoAdicional()
            {
                Tipo = "COMPRA GRATIS",
                Valor = valor,
                Porcentaje = 10,
                Codigo = codigo
            });
            CalcularPagos();

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "AgregarPagoCompraGratis", "Se aplicó beneficio Compra Gratis...");
        }


        public void borrarDescuentoFormaPago(string codigo)
        {
            for (var i = 0; i < this.Descuentos2.Count; i++)
            {
                if (this.Descuentos2[i].Tipo == codigo)
                {
                    this.Descuentos2.Remove(this.Descuentos2[i]);
                }
            }
        }

        public bool agregaDescuentoFormaPagoEmpleado(decimal valor, string forma_pago, string codigo)
        {
            var result = false;

            try
            {
                //result = agregaDescuentoEmpleado(valor, forma_pago, codigo);;
                //result = buscarDescuentosFormaPago("TAR PORTAL", "TAR PORTAL");
                //if (!result) {
                //    result = agregaDescuentoEmpleado(valor, forma_pago, codigo) ;
                //}

                result = buscarDescuentosFormaPago(forma_pago, codigo);


                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public bool buscarDescuentosFormaPago(string forma_pago, string codigo)
        {
            var hoy = DateTime.Now;
            var result = false;

            if (this.Descuentos2.Count > 0 && !this.Descuentos2.Any(x => x.Codigo == codigo)
                || this.Pagos.Count > 1
                || (this.Descuentos2.Count == 0 && this.Pagos.Count > 0))
            {
                if (this.Descuentos2.Count == 0)
                {
                    return false;
                }
                // se excluye los dsctos NA(No Aplican) como forma de pago.  JM 3-12-2019
                else if (this.Descuentos2.Any(x => x.Tipo == "NA"))
                {
                    this.Descuentos2.RemoveAll(x => x.Tipo != "NA");
                    return false;
                }

                //var dsctoEmp = this.Descuentos2.FirstOrDefault(x => x.Codigo == "DESC_EMP");
                //this.Descuentos2.Clear();

                //// Restaurar descuento de empleado
                //if (dsctoEmp != null)
                //    this.Descuentos2.Add(dsctoEmp);


                return true;
            }

            using (var db = new POSEntities())
            {
                var descuentos = db.core_descuento.Where(x => (x.tipo_descuento == DESCUENTO_FORMA_PAGO && x.activo) || (x.tipo_descuento == DESCUENTO_DIVISION_FORMA_PAGO && x.activo));

                foreach (var d in descuentos)
                {
                    if (d.fecha_desde.HasValue && d.fecha_hasta.HasValue)
                    {
                        if (d.fecha_desde <= hoy && hoy <= d.fecha_hasta)
                        {
                            if (forma_pago.ToUpper() == d.parametro)
                            {
                                result = this.agregarDescuentoAdicional(d, forma_pago, codigo);
                            }
                            else if (forma_pago.ToUpper() == d.parametro2 && _cliente_grupo.ToUpper() == d.parametro)
                            {
                                result = this.agregarDescuentoAdicional(d, forma_pago, codigo);
                            }
                        }

                    }
                    else
                    {
                        if (forma_pago.ToUpper() == d.parametro)
                        {
                            result = this.agregarDescuentoAdicional(d, forma_pago, codigo);

                        }
                        else if (forma_pago.ToUpper() == d.parametro2 && _cliente_grupo.ToUpper() == d.parametro)
                        {
                            result = this.agregarDescuentoAdicional(d, forma_pago, codigo);
                        }
                    }
                }
            }

            return result;
        }

        public bool agregaDescuentoEmpleado(decimal valor, string forma_pago, string codigo)
        {

            if (!this.Descuentos2.Any(x => x.Codigo == codigo))
            {
                this.Descuentos2.Add(new DescuentoAdicional() { Tipo = forma_pago, Valor = this.getSubTotal() * (valor / 100M), Porcentaje = valor, Codigo = codigo });
                return true;
            }
            return false;
        }


        bool agregarDescuentoAdicional(core_descuento descuento, string forma_pago, string codigo)
        {
            decimal subTotalActual = this.getSubTotal();
            System.Diagnostics.Debug.WriteLine($"[DEBUG POS - Dscto Adicional] Subtotal (getSubTotal()): {subTotalActual.ToString("N2")}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG POS - Dscto Adicional] Porcentaje aplicado: {descuento.valor}%");

            if (!this.Descuentos2.Any(x => x.Codigo == codigo))
            {
                this.Descuentos2.Add(new DescuentoAdicional()
                {
                    Tipo = forma_pago,
                    Valor = this.getSubTotal() * (descuento.valor / 100M),
                    Porcentaje = descuento.valor,
                    Codigo = codigo
                });
                return true;
            }
            return false;
        }


        public string GetNumeroFactura()
        {
            return this.Documento + "-" + this.Establecimiento + "-" + this.PtoEmision + "-" + this.Secuencia.ToString("000000000.##");
        }

        public string GetNumeroFacturaReimprime(long Secuencia)
        {
            return this.Documento + "-" + this.Establecimiento + "-" + this.PtoEmision + "-" + Secuencia.ToString("000000000.##");
        }

        public string GetNumeroFacturaEnmascarado()
        {
            //Cumplir la directiva de interface para su uso en formulario BasePagos
            return GetNumeroFactura();
        }

        public string getNumeroFacturaGiftcard()
        {
            return int.Parse(this.Establecimiento).ToString().PadLeft(3, '0') + int.Parse(this.PtoEmision).ToString().PadLeft(2, '0') + this.Secuencia.ToString("000000000.##");
        }

        public string getNumeroFacturaOpcional()
        {
            return this.Establecimiento + this.PtoEmision + this.Secuencia.ToString("000000000.##");
        }

        public bool validar()
        {
            var result = false;
            var cambio = 0M;
            result = this.Pagos.Count > 0 && this.Productos.Count > 0 && !string.IsNullOrEmpty(this.Cliente_codigo) && this.Cliente_codigo.Length > 0 && this.validar_pagos(out cambio);
            return result;
        }

        public decimal GetSaldoMonedero()
        {
            return this.SaldoPuntos;
        }

        public bool updateItems(pos_customer c)
        {
            for (var i = 0; i < this.Productos.Count; i++)
            {
                var item = this.Productos[i];
                decimal porcDescuentoDivisionEmpleado = 0;
                var descuento = item.getDescuento(this, c, ref porcDescuentoDivisionEmpleado);
                item.PorcDescuentoDivisionEmpleado = porcDescuentoDivisionEmpleado;
                item.calcularDescuento(descuento);
                item.update();
            }
            return true;
        }
        /// <summary>
        /// Limpia todos los items de la factura, aplica cuando es pedido App DelPortal.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool LimpiarItems()
        {
            this.Productos = null;

            return true;
        }

        public bool grabar(out string msj)
        {

            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", "Inicia Metodo grabar");

                bool tieneRet = false;

                if (this.EsPedidoOtraApp)
                {
                    string canal = string.Empty;
                    switch (this.PedidoOtraApp.Tipo)
                    {
                        case 0:
                            canal = "Venta POS";
                            break;
                        case 1:
                            canal = "App DelPortal";
                            break;
                        case 2:
                            canal = "Glovo";
                            break;
                        case 3:
                            canal = "Rappi";
                            break;
                        default:
                            canal = this.Delivery;
                            //Console.WriteLine("Default case");
                            break;
                    }

                    DialogResult dr = MessageBox.Show("La factura tiene asociado un pedido " + canal + ". ¿Está seguro de Continuar? ", "Pedidos Ya", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.No)
                    {
                        msj = string.Empty;
                        return false;
                    }
                }


                using (var db = new POSEntities())
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", "BeginTransaction");
                    using (System.Data.Entity.DbContextTransaction dbContextTransaction = db.Database.BeginTransaction())
                    {
                        msj = "";

                        try
                        {

                            var nueva = new core_factura();
                            nueva.fecha_creacion = DateTime.Now;
                            nueva.fecha_modificacion = nueva.fecha_creacion;
                            nueva.establecimiento = this.Establecimiento;
                            nueva.punto_emision = this.PtoEmision;
                            nueva.numero = this.Secuencia;
                            nueva.documento = this.Documento;
                            nueva.base0 = this.GetBase0();
                            nueva.base12 = this.GetBase12();
                            nueva.cliente_ax = this.Cliente_codigo;
                            nueva.cliente = this.ClienteIdentificacion;
                            nueva.direccion = this.Cliente_direccion;
                            nueva.razon_social = this._cliente_nombre;

                            //validar si viene desde app del portal no recalcule el iva ni promociones del POS.
                            if (this.PedidoOtraApp.Tipo == (byte)CANALVENTA.VENTAAPPPOS)
                            {
                                nueva.subtotal = this.Subtotal;
                                nueva.descuento2 = this.Descuento2;
                                nueva.iva = this.Iva;
                                nueva.total = this.Total;
                            }
                            else
                            {
                                if (this.GetPromoIva() > 0)
                                {
                                    nueva.descuento = this.GetDescuentos() + (this.GetPromoIva() - (this.GetPromoIva() - this.getIVA(false)));
                                }
                                else
                                {
                                    nueva.descuento = this.GetDescuentos();
                                }
                                nueva.subtotal = this.getSubTotal();
                                nueva.descuento2 = this.getDescuentos2();
                                nueva.iva = this.getIVA();
                                nueva.total = this.GetTotal();
                            }

                            //nueva.autorizacion = this.Autorizacion ;
                            nueva.autorizacion = string.IsNullOrEmpty(this.Autorizacion) ? "0000000000" : this.Autorizacion;
                            nueva.ip = this.Ip_address;
                            nueva.usuario = this.User.username;
                            nueva.msgError = Program.ID_Caja_POS;
                            nueva.paymID = Control.Common.GlobalParameters.SelectedServerIp;

                            int linea = 1;
                            foreach (var d in this.Productos)
                            {
                                var item = new core_facturadetalle();
                                item.fecha_creacion = nueva.fecha_creacion;
                                item.fecha_modificacion = nueva.fecha_creacion;
                                item.linea = linea;
                                //var itemAX = new pos_item { ITEMID = d.Id};//20200331
                                //db.pos_item.Attach(itemAX); //20200331
                                item.item_id = d.Id;
                                //item.pos_item = itemAX;
                                item.item_nombre = d.Nombre;
                                item.cantidad = d.Cantidad;
                                item.unidad = d.Unidad;
                                item.unidades = d.Unidades;
                                item.subtotal = d.SubtotalSinDescuento;
                                //if (this.getPromoIva() > 0 && d.Iva > 0)
                                //{
                                //    item.descuento = decimal.Round(d.Descuento + ((d.SubtotalSinDescuento - d.Descuento) * 0.10715M),2);
                                //    item.iva =   decimal.Round((d.SubtotalSinDescuento - (d.Descuento + ((d.SubtotalSinDescuento - d.Descuento) * 0.10715M))) * 0.12M,2);
                                //    item.total = decimal.Round((d.SubtotalSinDescuento - (d.Descuento + ((d.SubtotalSinDescuento - d.Descuento) * 0.10715M)))  + (d.SubtotalSinDescuento - (d.Descuento + ((d.SubtotalSinDescuento - d.Descuento) * 0.10715M))) * 0.12M,2);
                                //}
                                //else
                                //{
                                item.descuento = d.Descuento;
                                item.iva = d.Iva;
                                item.total = d.Total;
                                //}
                                item.precio = d.Pvp;
                                item.costo = d.Costo;
                                linea++;
                                nueva.core_facturadetalle.Add(item);

                                if (d.CarniceroCOD != null)
                                {
                                    if (!d.CarniceroCOD.Equals(String.Empty))
                                    {
                                        var _itemcarnicero = new core_facturadetalle_carnicero();
                                        _itemcarnicero.factura_id = nueva.id;
                                        _itemcarnicero.itemID = d.Id;
                                        _itemcarnicero.CarniceroCOD = d.CarniceroCOD;
                                        //nueva.core_facturadetalle_carnicero.Add(_itemcarnicero);
                                    }
                                }
                                if (d.TarjetasRegalo.Count > 0)
                                {
                                    foreach (var t in d.TarjetasRegalo)
                                    {
                                        t.activarTarjeta(item.total, this.Establecimiento, this.GetNumeroFactura(), db);
                                    }
                                }

                                //INICIO JCanarte 5Ene2021 Grabar tabla core_transaccion_operador
                                if (d.Operador != null && d.Operador != "")
                                {
                                    var _transaccion_operador = new core_transaccion_operador();
                                    _transaccion_operador.operador = d.Operador;
                                    _transaccion_operador.fecha = nueva.fecha_creacion;
                                    item.core_transaccion_operador.Add(_transaccion_operador);
                                }
                                //FIN JCanarte 5Ene2021 Grabar tabla core_transaccion_operador
                            }

                            var t1 = new POS.Control.TarjetaRegalo();
                            foreach (var p in this.Pagos)
                            {
                                var pago = new core_facturapago();
                                pago.fecha_creacion = nueva.fecha_creacion;
                                pago.fecha_modificacion = nueva.fecha_creacion;
                                if (p.Pagos.Count > 0)
                                {
                                    foreach (var subp in p.Pagos)
                                    {
                                        pago = new core_facturapago();
                                        pago.fecha_creacion = nueva.fecha_creacion;
                                        pago.fecha_modificacion = nueva.fecha_creacion;
                                        var pagoAX = new pos_paymentmode { PAYMMODE = p.Descripcion };
                                        //db.pos_paymentmode.Attach(pagoAX);
                                        // db.pos_paymentmode.Add(pagoAX.PAYMMODE);
                                        //db.pos_paymentmode.Add(pagoAX);
                                        pago.tipo_id = pagoAX.PAYMMODE;
                                        //pago.pos_paymentmode = pagoAX;
                                        pago.valor = subp.Valor;
                                        //Solo si es efectivo se resta el cambio.
                                        if (subp.Codigo == "EFECTIVO")
                                        {
                                            pago.valor = pago.valor - this.Cambio;
                                        }
                                        #region Tarjeta Promoción Española
                                        if (subp is PagoDescuentoEspanola)
                                        {
                                            var pagoTj = subp as PagoDescuentoEspanola;
                                            //db.pos_paymentmode.Attach(pagoAX);
                                            pago.tipo_id = pagoTj.Codigo;
                                            pago.datos = "DESCUENTO PROMOCION (-)";
                                        }
                                        #endregion
                                        if (subp is PagoTarjetaCredito)
                                        {
                                            var pTarjeta = subp as PagoTarjetaCredito;
                                            //pago.datos = pTarjeta.Banco + "-" + pTarjeta.Nombre + "-" + pTarjeta.Marca + "-"+pTarjeta.Codigo; 
                                            //pago.datos = pTarjeta.Nombre + "-" + pTarjeta.Marca + "-" + pTarjeta.Codigo; //evelasco 20 Septiembre 2019 comentado.

                                            pago.datos = pTarjeta.Nombre + "-" + pTarjeta.Marca + "-" + pTarjeta.Codigo + " " + pTarjeta.BinDescripcion;  //evelasco se envia, la trama "Autorización;NumVale;FechaVale;Valor" utilizado en proceso AX para cruce  de cobros con T.C. MEDIANET, DATAFAST.
                                            pago.voucher = pTarjeta.TipoPos;

                                            //evelasco .ini
                                            string activarTramaTC = string.Empty;
                                            string fpago = string.Empty;
                                            var pos = new POSEntities();

                                            try
                                            {
                                                if (pos.core_parametro.Any(x => x.identificador == "ACTIVAR_TRAMA_TC"))
                                                {
                                                    activarTramaTC = pos.core_parametro.Where(y => y.identificador == "ACTIVAR_TRAMA_TC").FirstOrDefault().valor;
                                                }
                                                else
                                                {
                                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "Grabar", "No se encontró el parámetro ACTIVAR_TRAMA_TC de la tabla core_parametro '" + this.GetNumeroFactura() + "'" + Environment.NewLine);
                                                }
                                            }
                                            catch (Exception exActivaTramaTC)
                                            {
                                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "Grabar", "Error al Obtener el parámetro ACTIVAR_TRAMA_TC de la tabla core_parametro. '" + this.GetNumeroFactura() + "'" + Environment.NewLine + exActivaTramaTC.InnerException.Message);
                                                activarTramaTC = string.Empty;
                                            }

                                            if (activarTramaTC.ToLower().Equals("true"))
                                            {
                                                try
                                                {
                                                    //MEDIANET
                                                    if (pTarjeta.TipoPos.ToUpper() == "MEDIANET")
                                                    {
                                                        if (pos.core_parametro.Any(x => x.identificador == "FP_MEDIANET"))
                                                        {
                                                            fpago = pos.core_parametro.Where(y => y.identificador == "FP_MEDIANET").FirstOrDefault().valor;
                                                            pago.tipo_id = fpago;
                                                        }
                                                        else
                                                        {
                                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "Grabar", "No se encontró el parámetro 'FP_MEDIANET' de la tabla core_parametro  '" + this.GetNumeroFactura() + "'" + Environment.NewLine);
                                                        }
                                                    }

                                                    //DATAFAST
                                                    if (pTarjeta.TipoPos.ToUpper() == "DATAFAST")
                                                    {
                                                        if (pos.core_parametro.Any(x => x.identificador == "FP_DATAFAST"))
                                                        {
                                                            fpago = pos.core_parametro.Where(y => y.identificador == "FP_DATAFAST").FirstOrDefault().valor;
                                                            pago.tipo_id = fpago;
                                                        }
                                                        else
                                                        {
                                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "Grabar", "No se encontró el parametro 'FP_DATAFAST' de la tabla core_parametro  '" + this.GetNumeroFactura() + "'" + Environment.NewLine);
                                                        }

                                                    }

                                                }
                                                catch (Exception exTramaCobro)
                                                {
                                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "Grabar", "Error al Obtener la Forma de Pago de la tabla parámetro core_parametro (FP_MEDIANET,FP_DATAFAST)'" + this.GetNumeroFactura() + "'" + Environment.NewLine + exTramaCobro.InnerException.Message);
                                                }


                                            }
                                            //evelasco .fin


                                        }
                                        else if (subp is PagoRetencion)
                                        {
                                            pago.datos = "Retencion";
                                            tieneRet = true;
                                        }
                                        else if (subp is PagoCheque)
                                        {
                                            var pCheque = subp as PagoCheque;
                                            pago.datos = pCheque.Banco + "-" + pCheque.Cuenta + "-" + pCheque.Numero;// +"-" + pCheque.Codigo;
                                        }
                                        else if (subp is PagoGiftCard)
                                        {
                                            var pGC = subp as PagoGiftCard;
                                            pago.datos = pGC.Codigo;

                                            //giftcardV reducir saldo tarjeta matriculada
                                            //if ((pago.tipo_id == "GIFT CARDV" || pago.tipo_id == "GIFT CARD") && this.EsUsoAppMovil == true)
                                            if (pago.tipo_id == "GIFT CARDV" && this.EsUsoAppMovil == true)
                                            {
                                                if (t1.getTarjetaGen(pGC.Codigo, this.ClienteIdentificacion, true))
                                                {
                                                    if (POS.Control.Common.GlobalParameters.EsAmbienteProduccion)
                                                    {
                                                        t1.realizarConsumoGiftCardGen(pGC.Valor, this.Establecimiento, this.GetNumeroFactura(), true);
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("La Gift Card esta fuera de línea y no se puede utilizar. No fue posible realizar el consumo de la GiftCard '" + pGC.Codigo + "'.");
                                                }
                                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar", "Se realizo consumo de giftcard : " + pGC.Codigo + " Valor:" + Convert.ToString(pGC.Valor));
                                            }
                                            else if (pago.tipo_id == "GIFT CARD" && this.EsUsoAppMovil == true && pGC.Codigo.Trim().StartsWith("2222"))
                                            {
                                                if (t1.getTarjetaGen(pGC.Codigo, this.ClienteIdentificacion, true))
                                                {
                                                    if (POS.Control.Common.GlobalParameters.EsAmbienteProduccion)
                                                    {
                                                        t1.realizarConsumoGiftCardGen(pGC.Valor, this.Establecimiento, this.GetNumeroFactura(), true);
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("La Gift Card esta fuera de línea y no se puede utilizar. No fue posible realizar el consumo de la GiftCard '" + pGC.Codigo + "'.");
                                                }
                                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar", "Se realizo consumo de giftcard : " + pGC.Codigo + " Valor:" + Convert.ToString(pGC.Valor));
                                            }
                                            else
                                            {
                                                //Si la giftcard es de tipo Venta, cambiar el tipo de pago en consecuencia
                                                if (db.core_giftcard.Any(x => x.codigo == pGC.Codigo && x.tipo == 1))
                                                {
                                                    pago.tipo_id = "GIFT CARDV";
                                                }

                                                if (t1.getTarjetaGen(pGC.Codigo, "", false))//busca en srv-pos
                                                {
                                                    if (POS.Control.Common.GlobalParameters.EsAmbienteProduccion)
                                                    {
                                                        t1.realizarConsumoGiftCardGen(pGC.Valor, this.Establecimiento, this.GetNumeroFactura(), false);
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("La Gift Card esta fuera de línea y no se puede utilizar. No fue posible realizar el consumo de la GiftCard '" + pGC.Codigo + "'");
                                                }
                                            }
                                        }
                                        else if (subp is PagoNotaCredito)
                                        {
                                            var pGC = subp as PagoNotaCredito;
                                            pago.datos = pGC.Codigo;
                                            var t = new POS.Control.NotaCredito();
                                            if (t.getTarjeta(pGC.Codigo))
                                            {
                                                if (!t.realizarConsumo(pGC.Valor, this.Establecimiento, "Consumo en factura: " + this.GetNumeroFactura(), db))
                                                {
                                                    throw new Exception("No fue posible realizar el consumo de la Nota Credito '" + pGC.Codigo + "'");
                                                }
                                            }
                                        }
                                        else if (subp is PagoTarjetaInterna)
                                        {
                                            var pTI = subp as PagoTarjetaInterna;
                                            pago.datos = pTI.Codigo;
                                            var tInterna = new POS.Control.TarjetaCreditoInterno(_tarjetaCreditoInternoAdicional);
                                            //if (tInterna.getTarjeta(pTI.Codigo))
                                            if (tInterna.getTarjetaGen(pTI.Codigo))
                                            {
                                                //if (!tInterna.realizarConsumo(pTI.Valor, this.Establecimiento, "Consumo en factura: " + this.GetNumeroFactura(), db))
                                                if (!tInterna.realizarConsumoGen(pTI.Valor, this.Establecimiento, "Consumo en factura: " + this.GetNumeroFactura()))
                                                {
                                                    throw new Exception("No fue posible realizar el consumo de la Tarjeta Delportal '" + pTI.Codigo + "'");
                                                }
                                            }
                                            //Para tener trazabilidad del uso de tarjetas adicionales, debe hacerse como ultimo paso
                                            if (_tarjetaCreditoInternoAdicional != null) pago.datos = _tarjetaCreditoInternoAdicional.codigo;
                                        }
                                        else if (subp is PagoMonedero)
                                        {
                                            pago.datos = "Dinero Electronico";
                                            decimal puntos_consumo = pago.valor / Control.WalletPoints.ClsPoints.FactorCanje;
                                            // ConsumirPuntosMonedero(db, puntos_consumo);
                                            decimal canjeado = 0, ultpuntos = 0;
                                            PagoxCanjePuntosMonedero(pago.valor, this.GetNumeroFactura(), this.Cliente_codigo, this.User.username);
                                            /*var puntoscab = db.TblPuntosCab.FirstOrDefault(x => x.AccountNum == this.ClienteIdentificacion && x.Estado == 1 && x.Saldo > 0);//comentado evelasco
                                            if (puntoscab != null)
                                            {
                                                var puntos_expirar = (from x in db.TblPuntos
                                                                      where x.IdTblPuntosCab == puntoscab.IdTblPuntosCab
                                                                      && x.FechaExpiracion != null && x.Estado == 1 && x.Saldo > 0
                                                                      orderby x.FechaExpiracion ascending
                                                                      select x).ToList();

                                                for (int x = 0; x < puntos_expirar.Count; x++)
                                                {
                                                    int id_canje = puntos_expirar[x].IdTblPuntos;
                                                    canjeado = canjeado + puntos_expirar[x].Saldo;

                                                    TblPuntos pto = (from a in db.TblPuntos
                                                                   where a.IdTblPuntos == id_canje
                                                                   && a.Estado == 1
                                                                   select a).First();
                                                    if (puntos_consumo >= canjeado)
                                                    {
                                                        pto.Saldo = 0;
                                                        pto.Estado = 0;
                                                        //db.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        pto.Saldo = canjeado - puntos_consumo;
                                                        canjeado = puntos_consumo;
                                                        //db.SaveChanges();
                                                        break;
                                                    }
                                                }

                                                if (puntos_consumo >= canjeado)
                                                {
                                                    var puntos_saldo = (from x in db.TblPuntos
                                                                        where x.IdTblPuntosCab == puntoscab.IdTblPuntosCab
                                                                        && x.FechaExpiracion == null && x.Estado == 1 && x.Saldo > 0
                                                                        orderby x.Saldo ascending
                                                                        select x).ToList();
                                                    for (int y = 0; y < puntos_saldo.Count; y++)
                                                    {
                                                        int id_canje = puntos_saldo[y].IdTblPuntos;
                                                        canjeado = canjeado + puntos_saldo[y].Saldo;

                                                        TblPuntos pto = (from a in db.TblPuntos
                                                                       where a.IdTblPuntos == id_canje
                                                                       && a.Estado == 1
                                                                       select a).First();
                                                        if (puntos_consumo >= canjeado)
                                                        {
                                                            pto.Saldo = 0;
                                                            pto.Estado = 0;
                                                            //db.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            pto.Saldo = canjeado - puntos_consumo;
                                                            canjeado = puntos_consumo;
                                                            //db.SaveChanges();
                                                            break;
                                                        }
                                                    }
                                                }

                                                ultpuntos = (from x in db.TblPuntos
                                                             where x.IdTblPuntosCab == puntoscab.IdTblPuntosCab
                                                             && x.Estado == 1
                                                             select x).ToList().Select(x => x.Saldo).Sum();
                                                TblPuntosCab pc = puntoscab;
                                                pc.Saldo = ultpuntos; 
                                            }*/
                                        }
                                        if (subp is PagoDelivery)
                                        {
                                            pago.datos = this.Delivery;
                                        }



                                        nueva.core_facturapago.Add(pago);
                                    }
                                }
                                else
                                {
                                    var pagoAX = new pos_paymentmode { PAYMMODE = p.Descripcion };
                                    //db.pos_paymentmode.Attach(pagoAX);
                                    pago.tipo_id = p.Descripcion;
                                    //pago.pos_paymentmode = pagoAX;                               
                                    pago.valor = p.Valor - this.Cambio;
                                    pago.datos = "Efectivo";
                                }
                                nueva.core_facturapago.Add(pago);
                            }

                            if (tieneRet)
                            {
                                core_retencion newRet = new core_retencion();
                                newRet.fecha_autorizacion = this.Retencion.FechaAutorizacion;
                                newRet.fecha_creacion = nueva.fecha_creacion;
                                newRet.fecha_modificacion = nueva.fecha_creacion;
                                newRet.num_autorizacion = this.Retencion.NumAutorizacion;
                                newRet.num_retencion = this.Retencion.NumRetencion;
                                newRet.num_factura = this.Retencion.NumFactura;
                                newRet.valor_base = this.Retencion.ValorBase;
                                //newRet.valor_ret_fte = this.Retencion.ValorRetFte - this.Retencion.ValorRetIVA;
                                newRet.valor_ret_fte = this.Retencion.ValorRetFte1;

                                newRet.CodRetIVA = this.Retencion.CodRetIVA;
                                newRet.CodPorcRetIVA = this.Retencion.CodPorcRetIVA;
                                newRet.ValorBaseIVA = this.Retencion.ValorBaseIVA;
                                newRet.ValorRetIVA = this.Retencion.ValorRetIVA;
                                newRet.ConceptoRetIVA = this.Retencion.ConceptoRetIVA;
                                newRet.valor_base1 = this.Retencion.valor_base1;
                                newRet.valor_base175 = this.Retencion.valor_base175;
                                newRet.valor_ret_fte175 = this.Retencion.valor_ret_fte175;

                                nueva.core_retencion.Add(newRet);
                            }
                            //eevv si la factura tiene pedido de Otra App que nno sea la delportal, entonces inserta en nueva tabla.
                            if (EsPedidoOtraApp)
                            {
                                TblPedido newPedido = new TblPedido();
                                newPedido.Pedido = PedidoOtraApp.Pedido;
                                newPedido.FechaCreacion = nueva.fecha_creacion;
                                newPedido.FechaModificacion = nueva.fecha_creacion;
                                newPedido.Tipo = PedidoOtraApp.Tipo; //1: Glovo
                                newPedido.Estado = 0;
                                nueva.TblPedido.Add(newPedido);
                            }

                            //Para Evitar Total cero
                            if (nueva.subtotal == 0 || nueva.total == 0)
                            {
                                //validar si viene desde app del portal no recalcule el iva ni promociones del POS.
                                if (this.PedidoOtraApp.Tipo == (byte)CANALVENTA.VENTAAPPPOS)
                                {
                                    nueva.subtotal = this.Subtotal;
                                    nueva.descuento2 = this.Descuento2;
                                    nueva.iva = this.Iva;
                                    nueva.total = this.Total;
                                }
                                else
                                {
                                    if (this.GetPromoIva() > 0)
                                    {
                                        nueva.descuento = this.GetDescuentos() + (this.GetPromoIva() - (this.GetPromoIva() - this.getIVA(false)));
                                    }
                                    else
                                    {
                                        nueva.descuento = this.GetDescuentos();
                                    }
                                    nueva.subtotal = this.getSubTotal();
                                    nueva.descuento2 = this.getDescuentos2();
                                    nueva.iva = this.getIVA();
                                    nueva.total = this.GetTotal();
                                }
                            }


                            /*  */

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", "Ejecuta core_factura.Add");
                            db.core_factura.Add(nueva);


                            if (!string.IsNullOrEmpty(this.ClaveAccesoSRI))
                            {
                                var claveAccFE = new core_ClaveAccesoFE();
                                claveAccFE.ClaveAcceso = this.ClaveAccesoSRI;
                                db.core_ClaveAccesoFE.Add(claveAccFE);
                            }

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", "Ejecuta SaveChanges ");
                            db.SaveChanges();

                            //Acciones si hay parqueo detectado

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", $"valida objeto ObjParking> {ObjParking}");
                            if (ObjParking != null)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", $"ejecuta foreach core_facturapago");
                                foreach (var pago in nueva.core_facturapago)
                                {
                                    if (ObjParking.Codigo.Length > 20)
                                    {
                                        pago.cliente = ObjParking.Codigo.Substring(ObjParking.Codigo.Length - 20, 20);
                                    }
                                    else
                                    {
                                        pago.cliente = ObjParking.Codigo;
                                    }

                                    if (ObjParking.DebeEnlazarFactura)
                                    {
                                        pago.JournalNum = ObjParking.FacEnlaceId.ToString();
                                    }
                                }

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", $"ejecuta ObjParking.DebeEnlazarFactura: {ObjParking.DebeEnlazarFactura}");
                                if (ObjParking.DebeEnlazarFactura)
                                {
                                    var facturaEnlacePagos = db.core_facturapago.Where(x => x.factura_id == ObjParking.FacEnlaceId).ToList();
                                    foreach (var pago in nueva.core_facturapago)
                                    {
                                        if (ObjParking.Codigo.Length > 20)
                                        {
                                            pago.cliente = ObjParking.Codigo.Substring(ObjParking.Codigo.Length - 20, 20);
                                        }
                                        else
                                        {
                                            pago.cliente = ObjParking.Codigo;
                                            pago.JournalNum = "ENLAZADO";
                                        }
                                    }
                                }

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", $"ejecuta TblRegistroParqueo");
                                var registroSalida = new TblRegistroParqueo()
                                {
                                    CodigoTicket = ObjParking.Codigo,
                                    FacturaId = nueva.id,
                                    FechaIngreso = ObjParking.FechaIngreso,
                                    FechaSalida = nueva.fecha_creacion,
                                    FechaCreacion = nueva.fecha_creacion,
                                    FechaModificacion = nueva.fecha_creacion,
                                    UsuarioCreacion = Control.Common.GlobalParameters.UserObj.username,
                                    UsuarioModificacion = Control.Common.GlobalParameters.UserObj.username,
                                    ItemId = this.Productos.Count > 1 ? ObjParking.ItemIdParqueo : ObjParking.ItemIdParqueoSinCompra
                                };
                                db.TblRegistroParqueo.Add(registroSalida);
                            }

                            //throw new Exception("test");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", $"Ejecuta SaveChanges TblRegistroParqueo");
                            db.SaveChanges();



                            //Acciones si hay perdida ticket parqueo
                            if (ObjParkingLost != null)
                            {
                                foreach (var pago in nueva.core_facturapago)
                                {
                                    pago.cliente = ObjParkingLost.Codigo;
                                }
                            }

                            this.Fecha = nueva.fecha_creacion;

                            //Agregar lineas de insert
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", $"Ejecuta Agregar_Trace_Factura");
                            POS.Control.Common.Logger.Agregar_Trace_Factura(nueva);

                            //Agregar lineas de xmlpagocanjepuntos
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabar", $"Ejecuta Agregar_Trace_PagoCanjePuntos");
                            POS.Control.Common.Logger.Agregar_Trace_PagoCanjePuntos(Control.Common.GlobalParameters.XmlPagoCanjePuntos);

                            //Persistir encuesta
                            POS.Control.Encuestas.EncuestaHandler.PersistirEncuestaEnMemoria(nueva.id);

                            //Almacenar id factura generada en base POS
                            IdFacturaPOS = nueva.id;

                            try
                            {
                                if (this._ordenApp != 0)
                                {
                                    core_facturaAPP appMovil_Core_Factura;
                                    appMovil_Core_Factura = db.core_facturaAPP.Where(x => x.orderApp == this._ordenApp && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                    if (Control.Common.GlobalParameters.srvPrincipal == "TRUE" && appMovil_Core_Factura == null)
                                    {
                                        prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                                        POSEntities db1 = new POSEntities();
                                        appMovil_Core_Factura = db1.core_facturaAPP.Where(x => x.orderApp == this._ordenApp && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                        appMovil_Core_Factura.orderID = "despachado";
                                        db1.SaveChanges();
                                        prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                                    }
                                    else
                                    {

                                        appMovil_Core_Factura.orderID = "despachado";
                                        db.SaveChanges();
                                    }

                                    //if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                                    //{
                                    //    prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                                    //    POSEntities db1 = new POSEntities();
                                    //    core_facturaAPP appMovil_Core_Factura;
                                    //    appMovil_Core_Factura = db1.core_facturaAPP.Where(x => x.orderApp == this._ordenApp && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                    //    appMovil_Core_Factura.orderID = "despachado";
                                    //    db1.SaveChanges();
                                    //    prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                                    //}
                                    //else
                                    //{
                                    //    core_facturaAPP appMovil_Core_Factura;
                                    //    appMovil_Core_Factura = db.core_facturaAPP.Where(x => x.orderApp == this._ordenApp && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                    //    appMovil_Core_Factura.orderID = "despachado";
                                    //    db.SaveChanges();
                                    //}
                                }
                            }
                            catch (Exception ex)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "Grabar", "Bloque Actualiza OrderId appMovil_Core_Factura - No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                            }
                            //actualizo los pedidos de App en esquema AppMovil:

                            try
                            {
                                if (this.PedidoOtraApp != null)
                                {
                                    if (this.PedidoOtraApp.Tipo == (byte)CANALVENTA.VENTAAPPPOS)
                                    {
                                        int numPedidoAppDelPortal = 0;
                                        numPedidoAppDelPortal = Convert.ToInt32(this.PedidoOtraApp.Pedido);
                                        core_facturaAPP appMovil_Core_Factura;
                                        appMovil_Core_Factura = db.core_facturaAPP.Where(x => x.orderApp == numPedidoAppDelPortal && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE" && appMovil_Core_Factura == null)
                                        {
                                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                                            POSEntities db1 = new POSEntities();
                                            appMovil_Core_Factura = db1.core_facturaAPP.Where(x => x.orderApp == numPedidoAppDelPortal && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                            appMovil_Core_Factura.orderID = "procesado POS";
                                            db1.SaveChanges();
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar", "Se realizo cambio en servidor : " + Control.Common.GlobalParameters.ipServerPedAPP);
                                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                                        }
                                        else
                                        {
                                            appMovil_Core_Factura.orderID = "procesado POS";
                                            db.SaveChanges();
                                        }


                                        //if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                                        //{
                                        //    prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                                        //    POSEntities db1 = new POSEntities();
                                        //    core_facturaAPP appMovil_Core_Factura;
                                        //    appMovil_Core_Factura = db1.core_facturaAPP.Where(x => x.orderApp == numPedidoAppDelPortal && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                        //    appMovil_Core_Factura.orderID = "procesado POS";
                                        //    db1.SaveChanges();
                                        //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar", "Se realizo cambio en servidor : " + Control.Common.GlobalParameters.ipServerPedAPP);
                                        //    prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                                        //}
                                        //else
                                        //{
                                        //    core_facturaAPP appMovil_Core_Factura;
                                        //    appMovil_Core_Factura = db.core_facturaAPP.Where(x => x.orderApp == numPedidoAppDelPortal && x.establecimiento == this.Establecimiento).FirstOrDefault();
                                        //    appMovil_Core_Factura.orderID = "procesado POS";
                                        //    db.SaveChanges();
                                        //}
                                    }
                                    else if (this.PedidoOtraApp.Tipo == (byte)CANALVENTA.VENTAPEDIDOGLOVO)
                                    {
                                        if (Control.Common.GlobalParameters.ActivaIntegracionPedidos)
                                        {
                                            using (WSIntegracion.Service1Client Integ = new WSIntegracion.Service1Client())
                                            {
                                                var xml = Integ.IntegrationDelivery(XmlIntegracion("Glovo", 5));
                                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar", "Dispatch:" + xml.ToString());

                                                var xmlF = Integ.IntegrationDelivery(XmlIntegracion("Glovo", 6));
                                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar ", "Billing:" + xml.ToString());
                                            }
                                        }
                                    }

                                }
                            }
                            catch (Exception ex3)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "Grabar", "Bloque Actualiza OrderId AppMovil.core_facturaApp - No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex3), "StackTrace: " + ex3.StackTrace);
                            }




                            try
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", $"Ejecuto codigo de acutalización de secuencial");

                                long secuencia1, secuenciaMSQUEUELocal;

                                // Tomar el max secuencial utilizado para verificar que numero de secuencia este correcto
                                int maxNroFactura = db.core_factura.Where(x => x.establecimiento == Control.Common.GlobalParameters.Establecimiento
                                                                                    && x.punto_emision == Control.Common.GlobalParameters.PuntoEmision)
                                                                       .Max(x => (int?)x.numero) ?? 0;


                                secuenciaMSQUEUELocal = Control.POS.validaSecuencialMQ("F");
                                var secuencia = db.core_documentosecuencia.Single(x => x.core_puntoemision.establecimiento_id == nueva.establecimiento
                                                    && x.core_puntoemision.punto_emision == nueva.punto_emision
                                                    && x.core_documento.codigo == nueva.documento);


                                if (secuenciaMSQUEUELocal > secuencia.siguiente)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "Grabar", "La secuencia de la cola de mensajes '" + secuenciaMSQUEUELocal + "' es mayor a la secuencia de la tabla core_documentosecuencia '" + secuencia.siguiente.ToString() + "', se procederá a actualizar la secuencia con el valor de la cola de mensaje.");
                                    secuencia1 = secuenciaMSQUEUELocal;
                                }
                                else if ((maxNroFactura + 1) > secuencia.siguiente)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "Grabar", "ultima secuencia de las facturas '" + maxNroFactura.ToString() + "' es mayor a la secuencia de la tabla core_documentosecuencia '" + secuencia.siguiente.ToString() + "', se procederá a actualizar la secuencia con el valor de la ultima secuencia+1 de la tabla de factura ('" + (maxNroFactura + 1).ToString() + "').");
                                    secuencia1 = (maxNroFactura + 1);
                                }
                                else
                                {
                                    secuencia1 = secuencia.siguiente + 1;
                                }

                                secuencia.siguiente = secuencia1;
                                db.SaveChanges();



                                try
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "actualizarSecuencia", "Antes de actualizar Secuencial en la cola de mensajes");
                                    ClsMessageQueue.receiveMessageQueue("F", nueva.establecimiento, nueva.punto_emision);
                                    ClsMessageQueue.setMessageQueue(nueva.establecimiento, nueva.punto_emision, secuencia1.ToString(), "F");//evelasco se graba el secuencial del documento en el message queue del S.O.
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "actualizarSecuencia", "La cola de mensaje se actualizó con el valor de " + secuencia.siguiente.ToString());
                                }
                                catch (Exception ex)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", $"Ejecuta Rollback");
                                    dbContextTransaction.Rollback();
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "actualizarSecuencia", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                                    return false;
                                }


                            }
                            catch (Exception ex3)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", $"Ejecuta Rollback");
                                dbContextTransaction.Rollback();
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "Grabar", "Bloque Actualiza OrderId AppMovil.core_facturaApp - No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex3), "StackTrace: " + ex3.StackTrace);
                                return false;
                            }



                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", $"Ejecuta Commit");
                            dbContextTransaction.Commit();



                            //Acciones si hay cupon de app
                            if (ObjCuponApp != null)
                            {
                                if (ObjCuponApp.SeUsoCuponApp)
                                {
                                    if (ObjCuponApp.realizarConsumoCuponApp(ObjCuponApp.IdTblPremio))
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar", "Se realizo consumo de cupon APP : " + ObjCuponApp.IdTblPremio.ToString() + "|" + ObjCuponApp.Codigo);
                                    }
                                    else
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "grabar", "El Cupon APP esta fuera de línea y no se puedo dar de baja el consumo del cupon '" + ObjCuponApp.IdTblPremio.ToString() + "|" + ObjCuponApp.Codigo + "'.");

                                    }
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            msj = ex.ToString();

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "Grabar", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                            var xmlRespuesta = POS.Control.Common.Mail.EnviaCorreo(
                            Properties.Settings.Default.MAILERROR_FROM,
                            Properties.Settings.Default.MAILERROR_ALIAS,
                            Properties.Settings.Default.MAILERROR_DESTINO,
                            Properties.Settings.Default.MAILERROR_CC,
                            Properties.Settings.Default.MAILERROR_MOTIVO,
                            String.Format("Establecimiento: {0} \nPto Emision: {1} \nSecuencia: {2} \nDocumento: {3} \nIpMaquina: {4} \nCajeroNombre: {5} \nCajeroId: {6} \n\nDatos Excepcion ------------\nClass: {7} \nMethod: {8} \nMessage: {9} \nStackTrace: {10}",
                                          this.Establecimiento,
                                          this.PtoEmision,
                                          this.Secuencia,
                                          this.Documento,
                                          Control.Common.GlobalParameters.IpMaquina,
                                          Control.Common.GlobalParameters.UserObj.nombres,
                                          Control.Common.GlobalParameters.UserObj.username,
                                          "POS.Models.factura",
                                          "grabar",
                                          POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                          ex.StackTrace),
                            false,
                            String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "Grabar", "No se pudo enviar email de error durante la ejecución del método, a continuacion el  de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "grabar", $"Ejecuta Rollback");
                            dbContextTransaction.Rollback();

                            return false;
                        }


                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        private string XmlIntegracion(string canalVenta, int opcion)
        {
            string xml = string.Empty;
            string Integ = string.Empty;
            string Metodo = string.Empty;
            string versionPOS = "1.1.1.972";
            string OrderId = string.Empty;
            string sInvoice = string.Empty;
            string Id = string.Empty;
            string Description = string.Empty;
            Version ver = null;
            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                    ver = ad.CurrentVersion;
                    versionPOS += ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision;
                }
                switch (canalVenta)
                {
                    case "Glovo":
                        Integ = "YA";
                        break;
                    case "Rappi":
                        Integ = "RA";
                        break;
                    default:
                        //Console.WriteLine("Default case");
                        break;

                }
                if (opcion == 1) //Inicializacion
                {

                    Metodo = "IN" + Integ;
                }
                else if (opcion == 2)//Recepcion
                {
                    Metodo = "RE" + Integ;
                    OrderId = this.PedidoOtraApp.Pedido;
                }
                else if (opcion == 3)//reconocimiento
                {
                    Metodo = "AL" + Integ;
                    OrderId = this.PedidoOtraApp.Pedido;
                }
                else if (opcion == 4)//State_change
                {
                    Metodo = "SC" + Integ;
                    OrderId = this.PedidoOtraApp.Pedido;
                }
                else if (opcion == 5)//Dispatch
                {
                    Metodo = "DI" + Integ;
                    OrderId = this.PedidoOtraApp.Pedido;
                }
                else if (opcion == 6)//Billing
                {
                    Metodo = "BI" + Integ;
                    OrderId = this.PedidoOtraApp.Pedido;
                    sInvoice = this.GetNumeroFactura();
                }
                XmlDocument xmlDoc = new XmlDocument();

                XmlNode rootNode = xmlDoc.CreateElement("Root");
                XmlAttribute attributer1 = xmlDoc.CreateAttribute("Integ");
                attributer1.Value = Integ;
                rootNode.Attributes.Append(attributer1);

                XmlAttribute attributer2 = xmlDoc.CreateAttribute("ProgId");
                attributer2.Value = Metodo;
                rootNode.Attributes.Append(attributer2);

                xmlDoc.AppendChild(rootNode);

                XmlNode userNode = xmlDoc.CreateElement("req");
                XmlAttribute attribute = xmlDoc.CreateAttribute("DelportalId");
                attribute.Value = Control.Common.GlobalParameters.Establecimiento;
                userNode.Attributes.Append(attribute);

                XmlAttribute attribute1 = xmlDoc.CreateAttribute("orderId");
                attribute1.Value = OrderId;
                userNode.Attributes.Append(attribute1);

                XmlAttribute attribute2 = xmlDoc.CreateAttribute("sInvoice");
                attribute2.Value = sInvoice;
                userNode.Attributes.Append(attribute2);

                XmlAttribute attribute3 = xmlDoc.CreateAttribute("Id");
                attribute3.Value = Id;
                userNode.Attributes.Append(attribute3);

                XmlAttribute attribute4 = xmlDoc.CreateAttribute("Name");
                attribute4.Value = "";
                userNode.Attributes.Append(attribute4);

                XmlAttribute attribute5 = xmlDoc.CreateAttribute("Description");
                attribute5.Value = Description;
                userNode.Attributes.Append(attribute5);

                XmlAttribute attribute6 = xmlDoc.CreateAttribute("VersionOs");
                attribute6.Value = System.Environment.OSVersion.ToString();
                userNode.Attributes.Append(attribute6);

                XmlAttribute attribute7 = xmlDoc.CreateAttribute("VersionPos");
                attribute7.Value = versionPOS;
                userNode.Attributes.Append(attribute7);

                rootNode.AppendChild(userNode);

                xml = xmlDoc.InnerXml.ToString();
            }
            catch (Exception ex)
            {

            }
            return xml;
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
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
            }
            catch (Exception ex)
            {
            }
        }

        public string generarClaveAccesoSRI(string tipoDocumento)
        {
            string tipoComprobante = string.Empty;

            if (tipoDocumento == "F")
            {
                tipoComprobante = "01"; // Factura
            }
            else if (tipoDocumento == "NC")
            {
                tipoComprobante = "04"; // Nota de Crédito
            }
            else if (this.Documento == "ND")
            {
                tipoComprobante = "05"; // Nota de Débito
            }
            else if (this.Documento == "GR")
            {
                tipoComprobante = "06"; // Guia de Remisión 
            }
            else if (this.Documento == "CR")
            {
                tipoComprobante = "06"; // Comprobante de Retención
            }



            string claveAcceso = string.Empty;
            string fecha = DateTime.Now.ToString("ddMMyyyy");//DateTime.Now;
            //string tipoComprobante = (this.Documento == "F" ? "01" : "");// "";
            string nRuc = POS.Control.Common.GlobalParameters.NRuc;
            string tipoAmbiente = POS.Control.Common.GlobalParameters.TipoAmbiente;
            string establecimiento = this.Establecimiento;
            string punto_emision = this.PtoEmision;
            string numero = this.Secuencia.ToString().PadLeft(9, '0');

            Random generador = new Random();
            int min = 10000000;
            int max = 100000000; // (99,999,999 + 1)
            int numeroAleatorio = generador.Next(min, max);

            string codigoNumerico = numeroAleatorio.ToString().Substring(0, 8);
            string tipoEmision = POS.Control.Common.GlobalParameters.TipoEmision;
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
        //Obtener descuento

        public decimal getDescuentosPromocionesAX()
        {
            //recorrer lista de productos en factura
            for (int j = 0; j < this.Productos.Count; j++)
            {
                decimal descuento_a_aplicar = 0;
                //recorrer lista de promociones activas de almacen
                for (int i = 0; i < this.PromocionesActuales.Count; i++)
                {
                    Promocion promo = this.PromocionesActuales[i];
                    //recorrer productos en promocion por cantidad

                    for (int k = 0; k < promo.ListProductos.Count; k++)
                    {
                        decimal prod_cantidad = this.Productos[j].Cantidad;
                        decimal prod_precio = this.Productos[j].Pvp;
                        decimal promo_cant = promo.ListProductos[k].Cantidad;
                        decimal promo_descuento = promo.ListProductos[k].Descuento;

                        if (promo_descuento > descuento_a_aplicar)
                        {
                            if (promo_cant <= prod_cantidad)
                            {
                                descuento_a_aplicar = promo_descuento;
                            }
                        }
                    }
                }
                //setear descuento                 
                this.Productos[j].DescuentoAX = this.Productos[j].SubtotalSinDescuento * (descuento_a_aplicar / 100);
            }
            return this.Productos.Sum(x => x.DescuentoAX);
        }

        /// <summary>
        /// 
        /// </summary>
        public void prepararImpresion(long Secuencia = 0)
        {
            var sub1 = "";
            var sub2 = "";
            var pos = new POSEntities();
            string Recibo = this.Recibo;

            if (this.aplicaBeneficioDevolucionIVA && this.montoIvaDevolver != 0)
            {

                var FactDevIVA = (from deta in pos.core_recibo
                                  where deta.identificador == "T_FACTURA_DEV_IVA"
                                  select deta).FirstOrDefault();

                if (FactDevIVA != null)
                {
                    Recibo = FactDevIVA.cuerpo;
                }
                else
                {
                    var recibo = pos.core_recibo.Where(x => x.identificador == ("T_FACTURA_DEV_IVA_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();

                    if (recibo == null)
                        recibo = pos.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteFactura).FirstOrDefault();

                    Recibo = recibo.cuerpo;
                }
            }

            this.Recibo = Recibo;

            if (this.Establecimiento_direccion.Length > 20)
            {
                int largo = this.Establecimiento_direccion.Length;
                sub1 = this.Establecimiento_direccion.Substring(0, 20);
                sub2 = this.Establecimiento_direccion.Substring(20, this.Establecimiento_direccion.Length - 20);
            }

            this.Recibo = this.Recibo.Replace("<<oficina>>", this.Establecimiento_nombre + Environment.NewLine + sub1 + Environment.NewLine + sub2);
            if (Secuencia == 0)
                this.Recibo = this.Recibo.Replace("<<telefono>>", this.Establecimiento_telefono);
            else
            {
                this.Recibo = this.Recibo.Replace("<<telefono>>", this.Establecimiento_telefono + "          C   O   P   I   A");
                this.Recibo = this.Recibo.Replace("<<WALLETPOINTS>>", string.Empty);
            }
            this.Recibo = this.Recibo.Replace("<<autorizacion>>", this.Autorizacion);
            this.Recibo = this.Recibo.Replace("<<desde>>", this.Fecha_inicio_autorizacion.ToShortDateString());
            this.Recibo = this.Recibo.Replace("<<hasta>>", this.Fecha_fin_autorizacion.ToShortDateString());

            if (Secuencia == 0)
                this.Recibo = this.Recibo.Replace("<<factura>>", this.GetNumeroFactura());
            else
                this.Recibo = this.Recibo.Replace("<<factura>>", this.GetNumeroFacturaReimprime(Secuencia));

            //Reemplazo clave de acceso
            System.Text.RegularExpressions.Regex regexAcc = new System.Text.RegularExpressions.Regex(@"<claveAcceso>(.*)\</claveAcceso>");
            StringBuilder claveAcc = new StringBuilder();

            if (!string.IsNullOrEmpty(this.ClaveAccesoSRI))
                claveAcc.AppendLine(Environment.NewLine + "" + ClaveAccesoSRI.Substring(0, 40) + Environment.NewLine + ClaveAccesoSRI.Substring(40, ClaveAccesoSRI.Length - 40));
            else
                claveAcc.AppendLine("");
            this.Recibo = regexAcc.Replace(this.Recibo, claveAcc.ToString());



            this.Recibo = this.Recibo.Replace("<<cajero>>", this.User.nombres);
            this.Recibo = this.Recibo.Replace("<<factura_fecha>>", this.Fecha.ToString("dd/MM/yyyy HH:mm:ss"));

            this.Recibo = this.Recibo.Replace("<<cedula>>", this.ClienteIdentificacion);
            this.Recibo = this.Recibo.Replace("<<cliente>>", this.Cliente_nombre);
            this.Recibo = this.Recibo.Replace("<<direccion>>", this.Cliente_direccion);
            this.Recibo = this.Recibo.Replace("<<cliente_telefono>>", this.Cliente_telefono);

            //TODO: Descomentar para facturación electrónica
            //this.Recibo = this.Recibo.Replace("<<clave_portal>>", this.Cliente_codigo);  

            var ValorFactura = 0M;

            //PRODUCTOS SELECCIONADOS
            int numpromo = 36;
            //    ValorFactura = this.GetTotal();
            if (this.PedidoOtraApp != null)
            {
                if (this.PedidoOtraApp.Tipo == (byte)CANALVENTA.VENTAAPPPOS)
                {
                    ValorFactura = this.Total;
                }
                else
                    ValorFactura = this.GetTotal();
            }


            if (this.Establecimiento == "001")
            {
                var db1 = new POSEntities();
                ValorFactura = 0;
                numpromo = 39;
                foreach (var producto in this.Productos)
                {
                    //promo parrillero
                    var query = (from tran in db1.pos_item
                                 where tran.ITEMID == producto.Id && tran.categoria == "AB"
                                 select tran).FirstOrDefault();
                    if (query != null)
                    {
                        ValorFactura += producto.Total;
                    }

                }



            }

            var selected = _productos.Where(u => pos.core_promocionticket_items.Where(x => x.promocionticket_id == numpromo).Select(y => y.itemid).Contains(u.Id));
            /// VARIACION para GM set parrillero

            decimal cantidad = 0;
            /*
                        if (this.Establecimiento == "001")
                        {
                            foreach (var producto in selected)
                            {

                                switch (producto.Id)
                                {
                                    case "PG-AB-000001"://	CERVEZA CLUB VERDE 330CC
                                        cantidad += (producto.Cantidad);
                                        break;
                                    case "PG-AB-006541"://		CERVEZA CLUB NEGRA 330CC
                                        cantidad += (producto.Cantidad);
                                        break;
                                    case "PG-AB-014400"://		CERVEZA CLUB ROJA 330 CC.
                                        cantidad += (producto.Cantidad);
                                        break;
                                    case "PG-AB-013257"://		CERVEZA CLUB CACAO 330 ML.
                                        cantidad += (producto.Cantidad);
                                        break;
                                    case "PG-AB-012963"://		CERVEZA CLUB VERDE PREMIUM LATA 355 FOUR PACK
                                        cantidad += (producto.Cantidad * 4);
                                        break;
                                    case "PG-AB-006542"://		CERVEZA CLUB PREMIUM NEGRA 330  SIX PACK
                                        cantidad += (producto.Cantidad * 6);
                                        break;
                                    case "PG-AB-004602"://		CERVEZA CLUB VERDE SIX PACK
                                        cantidad += (producto.Cantidad * 6);
                                        break;
                                    case "PG-AB-012962"://		CERVEZA CLUB PREMIUM ROJA 330  SIX PACK
                                        cantidad += (producto.Cantidad * 6);
                                        break;
                                }
                            }

                            if (cantidad >= 6 && ValorFactura >= 19)
                            {
                                this.Recibo = this.Recibo.Replace("<<mensaje_promo>>", "          ¡100% SUERTUDO!") +
                                "\n<footer>" + " *** ¡GANASTE! ***" + "</footer>\n" +
                                 " " +
                                "========================================\n" +
                                " ¡GANASTE CON CERVEZA CLUB Y DELPORTAL!\n" +
                                "Con la compra que acabaste de realizar, \n" +
                                " DELPORTAL te premia con un\n" +
                                "  kit CLUB parrillero para que armes\n" +
                                "      tus mejores asados.\n" +
                                "========================================\n" +
                                "              ¡Felicidades! \n" +
                                "========================================\n";


                                //flagMain = true;
                            }
                            else
                            {
                                this.Recibo = this.Recibo.Replace("<<mensaje_promo>>", " ¡El próximo suertudo puedes ser tú!") +
                                "\n=====================================\n" +
                                " ¡Sigue  comprando en  DELPORTAL, tú\n" +
                                " puedes  ser  el  próximo ganador de\n" +
                                "     nuestra promoción  \n" +
                                " del Kit CLUB parrillero.\n" +
                                " ¡No dejes de comprar! \n" +
                                "=====================================\n" +
                                "        GRACIAS POR ELEGIRNOS";

                                //flagMain = false;
                            }
                        }
          */

            //var selected = _productos.Where(u => new[] { "PG-AB-011332", "PG-AB-011320", "PG-AB-011324", "PG-AB-000036", "PG-AB-000800", "PG-AB-004106", "PG-AB-003309", "PG-AB-011361", "PG-AB-000738", "PG-AB-000661", "PG-AB-006326", "PG-AB-011771", "PG-AB-011259", "PG-AB-011259", "PG-AB-000881", "PG-AB-013339", "PG-AB-013346", "PG-AB-011811", "PG-AB-013357", "PG-AB-013353", "PG-AB-011808", "PG-AB-011111", "PG-AB-011106", "PG-AB-005616", "PG-AB-000671", "PG-AB-004744", "PG-AB-011929", "PG-AB-012719", "PG-AB-012714", "PG-AB-012715", "PG-AB-001115", "PG-AB-001117", "PG-AB-003075", "PG-AB-010955", "PG-AB-012005", "PG-AB-012016", "PG-AB-005904", "PG-AB-010944", "PG-AB-005914", "PG-AB-001307", "PG-AB-001305", "PG-AB-005860", "PG-AB-001306", "PG-AB-001304", "PG-AB-001302", "PG-AB-001300", "PG-AB-001303", "PG-AB-001299", "PG-AB-001301", "PG-AB-010940", "PG-AB-010951", "PG-AB-005056", "PG-AB-005052", "PG-AB-005124", "PG-AB-005218", "PG-AB-005292", "PG-AB-004871", "PG-AB-003556", "PG-AB-002596", "PG-AB-002831", "PG-AB-000967", "PG-AB-013338", "PG-AB-011855", "PG-AB-000012", "PG-AB-002752", "PG-AB-004947", "PG-AB-012449", "PG-AB-012450", "PG-AB-012451", "PG-AB-004966", "PG-AB-011049", "PG-AB-003135", "PG-AB-004995", "PG-AB-000433", "PG-AB-002308" }.Contains(u.Id));
            /*
            foreach(_productos.i u in selected)
            {
                //Do stuff on each selected user;
            }*/

            if (pos.core_parametro.Where(x => x.identificador == "GANAFACT" && x.valor == "TRUE" && x.parametro2 == this.Establecimiento).FirstOrDefault() != null)
            {
                decimal value = decimal.Parse(pos.core_parametro.Where(y => y.identificador == "MONTO_INVOICE_WIN").FirstOrDefault().valor);
                decimal valueMin = decimal.Parse(pos.core_parametro.Where(y => y.identificador == "MONTOMIN_INVOICE_WIN" && y.parametro2 == this.Establecimiento).FirstOrDefault().valor);


                if (this._cliente_grupo != "07" && this._cliente_grupo != "EM" && ValorFactura <= value && ValorFactura >= valueMin)
                {
                    activeInvoiceWinner = true;

                    var q = (from tran in pos.TblInvoiceCounters
                             where tran.establishment == this.Establecimiento
                             select tran).FirstOrDefault();

                    var q2 = (from tran2 in pos.TblInvoiceRandoms
                              where tran2.won == false
                              && tran2.establishment == this.Establecimiento
                              && tran2.date <= this.Fecha
                              select tran2).FirstOrDefault();

                    if (q != null && q2 != null)
                    {

                        //                        if (q2.winner == q.counter + 1 && selected.Count() >= 1) //Para productos seleccionados
                        if (q2.winner == q.counter + 1)                         //Para todos los productos
                        {
                            this.mensaje_promo = "          ¡100% SUERTUDO!";
                            this.Recibo = this.Recibo.Replace("<<mensaje_promo>>", this.mensaje_promo);
                            flagMain = true;
                        }
                        else
                        {
                            this.mensaje_promo = " ¡El próximo suertudo puedes ser tú!";
                            this.Recibo = this.Recibo.Replace("<<mensaje_promo>>", this.mensaje_promo);
                            if (q2.winner == q.counter + 1)
                            {
                                var db1 = new POSEntities();
                                var query = (from tran in db1.TblInvoiceCounters
                                             where tran.establishment == this.Establecimiento
                                             select tran).FirstOrDefault();

                                query.counter = query.counter - 1;

                                try
                                {
                                    db1.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "PrepararImpresion", "Imposible ejecutar SaveChanges para actualizar campo 'counter' en 'TblInvoiceCounters', a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                }

                            }
                        }
                    }
                    else
                    {
                        this.mensaje_promo = " ¡El próximo suertudo puedes ser tú!";
                        this.Recibo = this.Recibo.Replace("<<mensaje_promo>>", this.mensaje_promo);
                    }
                }
                else
                {
                    this.mensaje_promo = " ¡El próximo suertudo puedes ser tú!";
                    this.Recibo = this.Recibo.Replace("<<mensaje_promo>>", this.mensaje_promo);
                }
            }
            else
            {
                this.Recibo = this.Recibo.Replace("<<mensaje_promo>>", this.mensaje_promo);
            }

            //Reemplazo de productos
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaItem>(.*)\</plantillaItem>");
            StringBuilder items = new StringBuilder();
            StringBuilder itemsDsctos = new StringBuilder();
            decimal totalDsctosProductos = 0;
            decimal totalValorComercial = 0;
            decimal valorDsctosCuponPromocional = 0M;
            decimal totalDsctosCompraGratis = 0;
            decimal dsctoCompraGratis = 0;
            decimal porcDescuento = 0;
            decimal porcDsctosCuponPromocional = 0;
            decimal retencionSubtotalBase1 = 0;
            decimal retencionSubtotalBase175 = 0;
            decimal granTotalAhorradoApp = 0;
            Dictionary<string, decimal> dsctoAppPorProducto = new Dictionary<string, decimal>();
            decimal totalCuponImpresoFactura = 0M;
            bool esCuponImpresoFactura =
                EsUsoCuponPromocional &&
                !string.IsNullOrEmpty(this.CuponPromocionalCodigo) &&
                this.CuponPromocionalCodigo.StartsWith("CP");

            if (this.ObjCuponAppModerno != null && !string.IsNullOrEmpty(this.ObjCuponAppModerno.Codigo) && this.ObjCuponAppModerno.Codigo.Contains(";"))
            {
                string[] arrayItems = this.ObjCuponAppModerno.Codigo.Split('|');
                foreach (string iStr in arrayItems)
                {
                    string[] partes = iStr.Split(';');
                    if (partes.Length == 2)
                    {
                        if (decimal.TryParse(partes[1], out decimal valor))
                            dsctoAppPorProducto[partes[0]] = valor;
                    }
                }
            }

            foreach (var item in this.Productos)
            {
                dsctoCompraGratis = 0;
                string asterisk = " ";
                if (item.Descuento > 0)
                {
                    //asterisk = "Dc." + (Math.Round((item.Descuento / (item.Pvp * item.Cantidad)), 2) * 100) + "%";
                }

                // 1. DIBUJAR LA LÍNEA DEL PRODUCTO (Se mantiene igual)
                items.AppendLine(item.Nombre.PadRight(40, ' '));
                items.AppendLine(Control.Common.StringHelper.DevolverConPadding(item.Iva > 0 ? "I " : "", 1, 1, false) + Control.Common.StringHelper.DevolverConPadding(item.Cantidad.ToString("N2"), 50) + Control.Common.StringHelper.DevolverConPadding(item.Pvp.ToString("N2"), 12) + Control.Common.StringHelper.DevolverConPadding(item.SubtotalSinDescuento.ToString("N2"), 12));


                // -----------------------------------------------------------------------------------
                // 2. MATEMÁTICA PURA: SEPARAR DINERO DE APP VS DINERO DE TIENDA
                // -----------------------------------------------------------------------------------

                // A. ¿Cuánto de este producto se pagó con cupón? (Desempaquetado previamente)
                decimal descuentoTotalApp = dsctoAppPorProducto.ContainsKey(item.Id) ? dsctoAppPorProducto[item.Id] : 0;

                // B. ¿Cuánto de este producto es descuento REAL de la tienda?
                decimal descuentoRealTienda = item.Descuento - descuentoTotalApp;
                if (descuentoRealTienda < 0) descuentoRealTienda = 0; // Freno de seguridad

                // C. Acumulador para el TOTAL GLOBAL de la factura (Esto debe estar afuera de los ifs)
                // El total global necesita TODO el dinero (Tienda + App)
                if (item.DescuentoTarjetasCompraGratis > 0 && this.usoTarjetaCompraGratis)
                {
                    dsctoCompraGratis = item.DescuentoTarjetasCompraGratis;
                    totalDsctosCompraGratis += item.DescuentoTarjetasCompraGratis;
                }
                totalDsctosProductos += (item.Descuento - dsctoCompraGratis);

                // -----------------------------------------------------------------------------------
                // 3. SECCIÓN ANTIGUA: DETALLE DE DESCUENTOS (Solo se imprime si sobra plata de Tienda)
                // -----------------------------------------------------------------------------------

                if (descuentoRealTienda > 0)
                {
                    // CALCULO DE PORCENTAJES BASADO SOLO EN TIENDA
                    decimal valorDescSoloTienda = descuentoRealTienda - dsctoCompraGratis;
                    porcDescuento = (valorDescSoloTienda / item.SubtotalSinDescuento) * 100;

                    // Lógica antigua (se mantiene por si acaso)
                    if (EsUsoCuponPromocional)
                    {
                        if (valorDescSoloTienda > 0 && item.DescuentosCupon != null)
                        {
                            // Usamos el real de tienda, no el total
                            valorDsctosCuponPromocional += descuentoRealTienda;
                            porcDsctosCuponPromocional = porcDescuento;
                        }
                    }

                    
                    // CONDICIONAL PARA IMPRIMIR EN LA LISTA DE DESCUENTOS
                    if (valorDescSoloTienda > 0)
                    {
                        decimal valorCuponImpresoProducto = 0M;

                        if (item.DescuentosCupon != null)
                        {
                            valorCuponImpresoProducto = item.DescuentosCupon
                                .Where(d =>
                                    !string.IsNullOrEmpty(d.codigo) &&
                                    d.codigo.StartsWith("CP"))
                                .Sum(d => d.valor);
                        }

                        bool tieneCuponImpresoProducto = valorCuponImpresoProducto > 0;

                        // 1. Caso cupón impreso por producto/categoría/subcategoría/proveedor
                        // Aunque venga configurado por categoría o proveedor, el descuento termina aplicado al producto.
                        if (tieneCuponImpresoProducto)
                        {
                            string nombreProductoCorto = item.Nombre;

                            if (nombreProductoCorto.Length > 18)
                                nombreProductoCorto = nombreProductoCorto.Substring(0, 18).TrimEnd();

                            itemsDsctos.AppendLine(String.Concat(
                                "<bcol>",
                                "CUPON PRODUCTO " + nombreProductoCorto,
                                "|",
                                valorCuponImpresoProducto.ToString("N2"),
                                "</bcol>"
                            ));
                        }

                        // 2. Caso cupón impreso por factura
                        // No imprimimos aquí por producto. Solo acumulamos para imprimir una sola línea al final.
                        if (esCuponImpresoFactura)
                        {
                            totalCuponImpresoFactura += valorDescSoloTienda;
                        }
                        else
                        {
                            // 3. Descuentos normales o descuentos mixtos
                            // Si el producto tiene cupón impreso y además otro descuento normal,
                            // imprimimos solo el sobrante como descuento normal.
                            decimal valorNormalParaImprimir = valorDescSoloTienda - valorCuponImpresoProducto;

                            if (valorNormalParaImprimir < 0)
                                valorNormalParaImprimir = 0;

                            if (valorNormalParaImprimir > 0)
                            {
                                if ((valorNormalParaImprimir > 0 && !EsUsoCuponPromocional) ||
                                    (valorNormalParaImprimir > 0 && EsUsoCuponPromocional && item.DescuentosCupon == null))
                                {
                                    decimal porcNormal = (valorNormalParaImprimir / item.SubtotalSinDescuento) * 100;

                                    string etiquetaMostrar = ((porcNormal).ToString("N2") + "% ").PadLeft(7);

                                    string nombreProductoCorto = item.Nombre;
                                    if (nombreProductoCorto.Length > 11)
                                        nombreProductoCorto = nombreProductoCorto.Substring(0, 11);
                                    else
                                        nombreProductoCorto = nombreProductoCorto.PadRight(11, ' ');

                                    itemsDsctos.AppendLine(String.Concat(
                                        "<bcol>",
                                        etiquetaMostrar,
                                        nombreProductoCorto,
                                        "|",
                                        valorNormalParaImprimir.ToString("N2"),
                                        "</bcol>"
                                    ));
                                }
                            }
                        }
                    }
                }

                // -----------------------------------------------------------------------------------
                // 4. LÓGICA DE VALOR COMERCIAL Y RETENCIÓN (Intacta)
                // -----------------------------------------------------------------------------------

                if (!item.EsRegalo)
                {
                    try
                    {
                        SqlConnection conexion = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                        string Query = null;
                        SqlCommand comando = default(SqlCommand);
                        using (conexion)
                        {
                            conexion.Open();
                            Query = "Select top 1 PRICE_PVP,PRICE from InventTableModule mod with (nolock) WHERE ModuleType = 2 and mod.ItemId = '" + item.Id + "' and DataAreaId = 'liri'";
                            comando = new SqlCommand(Query, conexion);
                            SqlDataReader dr = comando.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                if (Decimal.Parse(dr.GetValue(0).ToString()) > 0)
                                    totalValorComercial += (Decimal.Parse(dr.GetValue(0).ToString()) * item.Cantidad) + (item.Iva > 0 ? ((Decimal.Parse(dr.GetValue(0).ToString()) * item.Cantidad) * Control.Common.GlobalParameters.IvaPorc / 100) : 0);
                                else
                                    totalValorComercial += (Decimal.Parse(dr.GetValue(1).ToString()) * item.Cantidad) + (item.Iva > 0 ? ((Decimal.Parse(dr.GetValue(1).ToString()) * item.Cantidad) * Control.Common.GlobalParameters.IvaPorc / 100) : 0);
                            }
                            conexion.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "prepararImpresion", "Error InventTableModule - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                        totalValorComercial += 0;
                    }
                }

                if (POS.Control.Common.GlobalParameters.DatoRetencionEnFactura == true)
                {
                    if (item.RetencionPorcentaje == 1)
                        retencionSubtotalBase1 = retencionSubtotalBase1 + item.Subtotal;
                    if (item.RetencionPorcentaje > 1)
                        retencionSubtotalBase175 = retencionSubtotalBase175 + item.Subtotal;
                }
            }

            // Cupón impreso por factura: se imprime una sola línea totalizada
            if (esCuponImpresoFactura && totalCuponImpresoFactura > 0)
            {
                itemsDsctos.AppendLine(String.Concat(
                    "<bcol>",
                    "CUPON FACTURA",
                    "|",
                    totalCuponImpresoFactura.ToString("N2"),
                    "</bcol>"
                ));
            }

            // =========================================================================
            // NUEVA SECCIÓN: CUPONES APLICADOS (Sin puntos, sin $, letra resaltada)
            // =========================================================================
            if (this.ObjCuponAppModerno != null && !string.IsNullOrEmpty(this.ObjCuponAppModerno.Descripcion))
            {
                StringBuilder sbCupones = new StringBuilder();
                // Opcional: También le ponemos <b> al título para que resalte más
                sbCupones.AppendLine("<b>CUPONES APLICADOS</b>");

                decimal granTotalApp = 0;
                string[] arrayCupones = this.ObjCuponAppModerno.Descripcion.Split('|');

                foreach (string cStr in arrayCupones)
                {
                    if (string.IsNullOrEmpty(cStr)) continue;

                    string[] partes = cStr.Split(';');
                    string nombre = partes[0].Trim();
                    decimal valorCup = 0;

                    if (partes.Length > 1 && decimal.TryParse(partes[1], out decimal v))
                        valorCup = v;

                    granTotalApp += valorCup;

                    string valorTexto = valorCup.ToString("N2");

                    // ✅ Ajusta este número hasta que visualmente quede bien
                    // Cuenta los caracteres que tiene "25.00% CERVEZA CLUB" como referencia
                    int anchoNombre = 18;

                    // Recortar nombre si excede el ancho máximo
                    if (nombre.Length > anchoNombre)
                        nombre = nombre.Substring(0, anchoNombre).TrimEnd();

                    // PadRight para que todos los nombres tengan el mismo ancho
                    string nombreAlineado = nombre.PadRight(anchoNombre);

                    // Formato: NOMBRE__________: 0.17
                    string linea = $"<bcol>{nombre}|{valorTexto}</bcol>";
                    sbCupones.AppendLine(linea);
                }

                sbCupones.AppendLine("");

                // Aplicamos lo mismo al Total (Sin $ y en negrita)
                //string textoTotal = "TOTAL AHORRADO APP".PadRight(38, ' ');
                //sbCupones.AppendLine("<b>" + textoTotal + "</b>" + granTotalApp.ToString("N2"));

                //sbCupones.AppendLine("---------------------------------------------------------------------");
                //sbCupones.AppendLine("");

                this.Recibo = this.Recibo.Replace("<<BLOQUE_CUPONES>>", sbCupones.ToString());
            }
            else
            {
                this.Recibo = this.Recibo.Replace("<<BLOQUE_CUPONES>>", "");
            }

            bool tieneCuponImpresoEnFactura =
                esCuponImpresoFactura ||
                this.Productos.Any(p =>
                    p.DescuentosCupon != null &&
                    p.DescuentosCupon.Any(d =>
                        !string.IsNullOrEmpty(d.codigo) &&
                        d.codigo.StartsWith("CP")));
            //*****
            if (EsUsoCuponPromocional && valorDsctosCuponPromocional > 0 && !tieneCuponImpresoEnFactura)
            {
                if (porcDsctosCuponPromocional == 0)
                {
                    porcDsctosCuponPromocional = porcDescuento;
                }

                itemsDsctos.AppendLine(String.Concat(
                                                    "<b>"
                                                    , (porcDsctosCuponPromocional).ToString("N2")
                                                    , "% "
                                                    , ("DSCTO CUPON").PadRight(14, ' ').Substring(0, 13)
                                                    , Convert.ToChar(9)
                                                    , ":"
                                                    , Control.Common.StringHelper.DevolverConPadding(valorDsctosCuponPromocional.ToString("N2"), 17)
                                                    , "</b>")
                                                    );
            }

            // -----------------------------------------------------------------------------------------
            // BLOQUE 1: DESCUENTO PROMOCIONAL (Empleado, VIP, Otros)
            // -----------------------------------------------------------------------------------------
            // Sumamos todo lo que NO sea Compra Gratis
            var valorOtrosDescuentos = this.Descuentos2
                                           .Where(x => x.Tipo != "COMPRA GRATIS")
                                           .Sum(x => x.Valor);

            string textoPromocional = "";

            if (valorOtrosDescuentos > 0)
            {
                // Usamos la misma alineación (52) para que se vea ordenado
                textoPromocional = "Desc. Promocional".PadRight(48, ' ')
                                   + ":"
                                   + Control.Common.StringHelper.DevolverConPadding(valorOtrosDescuentos.ToString("N2"), 23)
                                   + "\r\n";
            }

            // Reemplazamos la etiqueta <<descuentopromocional>>
            // Si no hay descuento, se reemplaza por vacío y no imprime nada.
            this.Recibo = this.Recibo.Replace("<<descuentopromocional>>", textoPromocional);

            // -----------------------------------------------------------------------------------------
            // 2. NUEVO CÓDIGO: GENERAR LÍNEA DE COMPRA GRATIS (Aquí empieza la magia)
            // -----------------------------------------------------------------------------------------
            // Buscamos el descuento en la lista usando "Tipo"
            var objCompraGratis = this.Descuentos2.FirstOrDefault(x => x.Tipo == "COMPRA GRATIS");
            string textoCompraGratis = "";

            // Usamos .Valor porque es la propiedad que usas en tu código (según vi en tu línea de Max(x=>x.Valor))
            if (objCompraGratis != null && objCompraGratis.Valor > 0)
            {
                // Formato: Texto alineado + tabulador + : + Valor alineado + salto de linea
                textoCompraGratis = "Compra Gratis".PadRight(51, ' ')
                                    + ":"
                                    + Control.Common.StringHelper.DevolverConPadding(objCompraGratis.Valor.ToString("N2"), 23)
                                    + "\r\n";
            }

            // REEMPLAZAMOS LA ETIQUETA EN LA FACTURA AHORA MISMO
            this.Recibo = this.Recibo.Replace("<<det_compragratis>>", textoCompraGratis);
            // -----------------------------------------------------------------------------------------

            var descuento2_ = (this.Descuentos2.Count > 0) ? decimal.Round(this.Descuentos2.Sum(x => x.Valor), 2, MidpointRounding.AwayFromZero) : this.Descuento2;// this.Descuentos2;
            //Por solicitud de ajSaab, las facturas no deben mostrar seccion descuentos si no hay descuentos
            var totalDsctos = totalDsctosProductos + descuento2_ + (this.GetPromoIva() > 0 ? this.getIVA(false) : 0);
            //var totalDsctos = totalDsctosProductos + (this.GetPromoIva() > 0 ? this.getIVA(false) : 0);
            if (totalDsctos == 0)
            {
                this.Recibo = this.Recibo.Replace(@"
                <b>----------DETALLE DE DESCUENTOS---------------------------</b>
                <<descuentoproductos>><<det_compragratis>><<promo_iva>>
                <b>Total descuentos		:<<total_descuentos>></b>", string.Empty);
            }

            this.Recibo = regex.Replace(this.Recibo, items.ToString());
            this.Recibo = this.Recibo.Replace("<<descuentoproductos>>", itemsDsctos.ToString());

            this.Recibo = this.Recibo.Replace("<<basetotal>>", Control.Common.StringHelper.DevolverConPadding((this.GetBase0() + this.GetBase12()).ToString("N2"), 65));
            //this.Recibo = this.Recibo.Replace("<<descuentototal>>", Control.Common.StringHelper.DevolverConPadding((totalDsctosProductos + (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 65));
            this.Recibo = this.Recibo.Replace("<<descuentototal>>", Control.Common.StringHelper.DevolverConPadding((totalDsctosProductos + descuento2_ + (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 65));
            //this.Recibo = this.Recibo.Replace("<<basecondescuentos>>", Control.Common.StringHelper.DevolverConPadding((this.GetBase0() + this.GetBase12() - (totalDsctosProductos+ totalDsctosCompraGratis) - (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 65));
            this.Recibo = this.Recibo.Replace("<<basecondescuentos>>", Control.Common.StringHelper.DevolverConPadding((this.GetBase0() + this.GetBase12() - (totalDsctosProductos + totalDsctosCompraGratis) - descuento2_ - (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 65));

            this.Recibo = this.Recibo.Replace("<<total_descuentos>>", Control.Common.StringHelper.DevolverConPadding((totalDsctosProductos + descuento2_ + (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 17));
            //this.Recibo = this.Recibo.Replace("<<total_descuentos>>", Control.Common.StringHelper.DevolverConPadding((totalDsctosProductos + (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 17));

            //Reemplazo de Formas de pago 
            regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");
            StringBuilder pagos = new StringBuilder();

            foreach (var pago in this.Pagos)
            {
                //pagos.AppendLine(pago.Descripcion + " : " + String.Format("{0,10:0.00}", pago.Valor.ToString("N2")));
                if (pago.Descripcion == "DINE ELECT")
                    pagos.AppendLine(POS.Control.Common.GlobalParameters.MonederoEtiquetaRecibo + Convert.ToChar(9) + ":" + Control.Common.StringHelper.DevolverConPadding(pago.Valor.ToString("N2"), 41));
                else if (pago.Descripcion == "EFECTIVO")
                    pagos.AppendLine(pago.Descripcion + Convert.ToChar(9) + ":" + Control.Common.StringHelper.DevolverConPadding(pago.Valor.ToString("N2"), 65));
                else
                    pagos.AppendLine(pago.Descripcion + Convert.ToChar(9) + ":" + Control.Common.StringHelper.DevolverConPadding(pago.Valor.ToString("N2"), 53));
            }
            this.Recibo = regex.Replace(this.Recibo, pagos.ToString());

            this.Recibo = this.Recibo.Replace("<<dineroElectronico>>", Control.Common.StringHelper.DevolverConPadding(totalDsctosCompraGratis.ToString("N2"), 54));
            if (this.usoTarjetaCompraGratis)
            {
                this.Recibo = this.Recibo.Replace("<<billetera>>", Control.Common.StringHelper.DevolverConPadding(totalDsctosCompraGratis.ToString("N2"), 41));
                this.Recibo = this.Recibo.Replace("<billeteraElectronica>", "").Replace("</billeteraElectronica>", "");
            }
            else
            {
                regex = new System.Text.RegularExpressions.Regex(@"<billeteraElectronica>(.*)\</billeteraElectronica>\r\n");
                this.Recibo = regex.Replace(this.Recibo, "");
            }


            this.Recibo = this.Recibo.Replace("<plantillaItem>", "").Replace("</plantillaItem>", "");
            this.Recibo = this.Recibo.Replace("<plantillaPago>", "").Replace("</plantillaPago>", "");
            this.Recibo = this.Recibo.Replace("<<base0>>", String.Format("{0,10:0.00}", this.GetBase0().ToString("N2")));
            this.Recibo = this.Recibo.Replace("<<base12>>", String.Format("{0,10:0.00}", (this.GetBase12()).ToString("N2")));




            bool estatus14 = false;
            decimal dscto14 = 0;
            POSEntities db = new POSEntities();
            foreach (Producto prod in this.Productos)
            {
                decimal porc_iva;
                porc_iva = ((Control.Common.GlobalParameters.IVAGEN / 100));
                if (Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC)
                {
                    porc_iva = ((Control.Common.GlobalParameters.IVA_ANTERIOR / 100));
                }

                if (Math.Round(prod.SubtotalSinDescuento - (prod.SubtotalSinDescuento * porc_iva), 2) == prod.Subtotal)
                {
                    estatus14 = true;
                    dscto14 += prod.Descuento;

                }
            }
            if (this.GetPromoIva() > 0)
            {
                //this.Recibo = this.Recibo.Replace("<<promo_iva>>", "  DESCUENTO IVA:" + String.Format("{0,09:0.00}", (this.getIVA()).ToString("N2")));
                this.Recibo = this.Recibo.Replace("<<promo_iva>>", "<b>" + ("DESCUENTO IVA").PadRight(30, ' ') + Convert.ToChar(9) + ":" + Control.Common.StringHelper.DevolverConPadding((this.getIVA(false)).ToString("N2"), 17) + "</b>");
            }
            else
            {
                this.Recibo = this.Recibo.Replace("<<promo_iva>>", "");
            }
            this.Recibo = this.Recibo.Replace("<<factura_subtotal>>", String.Format("{0,10:0.00}", ((this.GetBase0() - this.getDescuentos0()) + (this.GetBase12() - ((this.GetDescuentos() + this.getDescuentos2()) - this.getDescuentos0()))).ToString("N2")));

            //this.Recibo = this.Recibo.Replace("<<subtotal_desc>>", String.Format("{0,10:0.00}", (this.getBase12() - ((this.getDescuentos() + this.getDescuentos2()) - this.getDescuentos0())).ToString("N2")));
            //this.Recibo = this.Recibo.Replace("<<subtotal_desc>>", Control.Common.StringHelper.DevolverConPadding((this.GetBase12() - ((this.GetDescuentos() + (this.GetBase12() >= ((this.Descuentos2.Count > 0) ? this.getDescuentos2() : this.Descuento2) ? ((this.Descuentos2.Count > 0) ? this.getDescuentos2() : this.Descuento2) : 0) ) - this.getDescuentos0()) - (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 65));
            //this.Recibo = this.Recibo.Replace("<<subtotal_desc>>", Control.Common.StringHelper.DevolverConPadding((this.GetBase12() - ((this.GetDescuentos() + (this.Descuentos2.Count > 0 ? (decimal.Round(this.Descuentos2.Max(x => x.Porcentaje), 2, MidpointRounding.AwayFromZero) / 100) : (this.Descuento2 / (this.GetBase0() + this.GetBase12()))) * this.GetBase12()) - this.getDescuentos0()) - (this.GetPromoIva() > 0 ? this.getIVA(false) : 0)).ToString("N2"), 65));
            //decimal valorTarifa15Simple = this.GetBase12()
            //                  - (this.GetDescuentos() - this.getDescuentos0()) // Descuentos normales de producto
            //                  - descuento2_;                                   // Tu Compra Gratis + Empleado

            //// 3. Reemplazo en la factura
            //this.Recibo = this.Recibo.Replace("<<subtotal_desc>>", Control.Common.StringHelper.DevolverConPadding(valorTarifa15Simple.ToString("N2"), 65));

            // -----------------------------------------------------------------------------------------
            // CÁLCULO DEFINITIVO DE TARIFAS NETAS (Soporte para descuentos de línea + globales)
            // -----------------------------------------------------------------------------------------

            // 1. Obtenemos bases BRUTAS (Sumatoria de precios full) y Descuentos de LÍNEA
            decimal b0_Bruta = this.GetBase0();
            decimal b12_Bruta = this.GetBase12();

            decimal dsctoLineaTotal = this.GetDescuentos();
            decimal dsctoLinea0 = this.getDescuentos0();
            // El descuento de línea para tarifa 12 es el total menos el de tarifa 0
            decimal dsctoLinea12 = dsctoLineaTotal - dsctoLinea0;

            // 2. Calculamos las BASES "SEMI-NETAS"
            // (Esto es lo que realmente cuestan los productos antes de aplicar descuentos globales)
            // ESTA ES LA CORRECCIÓN CLAVE: Restamos el descuento de línea ANTES de calcular factores.
            decimal b0_SemiNeta = b0_Bruta - dsctoLinea0;
            decimal b12_SemiNeta = b12_Bruta - dsctoLinea12;
            decimal totalSemiNeto = b0_SemiNeta + b12_SemiNeta; // Esto debería coincidir con tu Subtotal

            // 3. Calculamos los FACTORES (Pesos) basados en las bases Semi-Netas
            decimal factor0 = (totalSemiNeto > 0) ? (b0_SemiNeta / totalSemiNeto) : 0;
            decimal factor12 = (totalSemiNeto > 0) ? (b12_SemiNeta / totalSemiNeto) : 0;

            // 4. Obtenemos y repartimos el DESCUENTO GLOBAL (Compra Gratis, Empleado, etc.)
            decimal dineroDescuentoGlobal = this.Descuentos2.Sum(x => x.Valor);

            decimal globalPara0 = dineroDescuentoGlobal * factor0;
            decimal globalPara12 = dineroDescuentoGlobal * factor12;

            // 5. Calculamos los VALORES FINALES A IMPRIMIR
            // Base SemiNeta - Su parte del Descuento Global
            decimal valorFinalTarifa0 = b0_SemiNeta - globalPara0;
            decimal valorFinalTarifa15 = b12_SemiNeta - globalPara12;

            // -----------------------------------------------------------------------------------------
            // INYECCIÓN EN EL RECIBO
            // -----------------------------------------------------------------------------------------
            this.Recibo = this.Recibo.Replace("<<subtotal_desc0>>",
                Control.Common.StringHelper.DevolverConPadding(valorFinalTarifa0.ToString("N2"), 65));

            this.Recibo = this.Recibo.Replace("<<subtotal_desc>>",
                Control.Common.StringHelper.DevolverConPadding(valorFinalTarifa15.ToString("N2"), 65));


            this.Recibo = this.Recibo.Replace("<<factura_descuento>>", String.Format("{0,09:0.00}", (this.GetDescuentos() + this.getDescuentos2() - this.getDescuentos0()).ToString("N2")));
            this.Recibo = this.Recibo.Replace("<<factura_descuento0>>", String.Format("{0,09:0.00}", (this.getDescuentos0()).ToString("N2")));
            this.Recibo = this.Recibo.Replace("<<factura_total>>", Control.Common.StringHelper.DevolverConPadding(this.GetTotal().ToString("N2"), 65));
            this.Recibo = this.Recibo.Replace("<<total_pagar>>", String.Format("{0,10:0.00}", this.GetTotal().ToString("N2")));

            //this.Recibo = this.Recibo.Replace("<<subtotal_desc0>>", Control.Common.StringHelper.DevolverConPadding((this.GetBase0() - ((this.Descuentos2.Count > 0 ? (decimal.Round(this.Descuentos2.Max(x => x.Porcentaje), 2, MidpointRounding.AwayFromZero) / 100) : (this.Descuento2 / (this.GetBase0() + this.GetBase12()))) * this.GetBase0()) - this.getDescuentos0()).ToString("N2"), 65));
            if (this.GetBase12Desc() > 0) this.Recibo = this.Recibo.Replace("<<desc_iva>>", String.Format("{0,10:0.00}", ((this.getDescuentos2() / this.GetBase12Desc()) * this.GetBase12Desc()).ToString("N2"))); //*100/12=8.33 regla de 3 para calculo de subtotal con descuento para productos iva 12
            else this.Recibo = this.Recibo.Replace("<<desc_iva>>", String.Format("{0,10:0.00}", "0.00"));
            this.Recibo = this.Recibo.Replace("<<factura_iva>>", Control.Common.StringHelper.DevolverConPadding(this.getIVA().ToString("N2"), 65));


            this.Recibo = this.Recibo.Replace("<<montoIvaDevolver>>", Control.Common.StringHelper.DevolverConPadding(this.montoIvaDevolver.ToString("N2"), 65));


            //if (aplicaBeneficioDevolucionIVA)
            //{
            //    string etiqueta_devolucionIva = @"Cliente Benfeciciario Devolución de Vida";
            //    this.Recibo = this.Recibo.Replace("<<etiqueta_devolucionIva>>", etiqueta_devolucionIva.ToString());

            //}


            //string.Concat("**** Cliente es Benfeciciario Devolución IVA ***** ").ToString("N2"), 65);

            this.Recibo = this.Recibo.Replace("<<factura_cambio>>", Control.Common.StringHelper.DevolverConPadding(this.Cambio.ToString("N2"), 65));

            //Diferencia con competencia
            decimal precioComercial = totalValorComercial;
            decimal precioDelportal = this.GetTotal();

            this.Recibo = this.Recibo.Replace("<<precioComercial>>", Control.Common.StringHelper.DevolverConPadding((precioComercial).ToString("N2"), 28));
            this.Recibo = this.Recibo.Replace("<<precioDelportal>>", Control.Common.StringHelper.DevolverConPadding((precioDelportal).ToString("N2"), 28));
            this.Recibo = this.Recibo.Replace("<<precioDiferencia>>", Control.Common.StringHelper.DevolverConPadding((precioComercial - precioDelportal).ToString("N2"), 28));

            StringBuilder impriCovid19 = new StringBuilder();
            regex = new System.Text.RegularExpressions.Regex(@"<Covid19Plantilla>(.*)\</Covid19Plantilla>");
            this.ReciboCovid19 = "<Covid19Plantilla></Covid19Plantilla>";
            if (db.core_parametro.Where(k => k.identificador == "ACTIVAR_VACUNA" && k.valor == "TRUE" && k.parametro2.Contains(this.Establecimiento)).Any())
            {
                if (db.TblVacunacion.Where(k => k.CEDULA == this.ClienteIdentificacion).Any())
                {

                    //      var param = db.core_parametro.Where(k => k.identificador == "ACTIVAR_VACUNA" && k.valor == "TRUE" && k.parametro2.Contains(this.Establecimiento)).First();

                    var vacuna = db.TblVacunacion.Where(k => k.CEDULA == this.ClienteIdentificacion).First();
                    //impriCovid19.AppendLine(param .documento);
                    impriCovid19.AppendLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
                    impriCovid19.AppendLine("<h1>VACUNATE</h1>");
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine("<b>     PLAN DE VACUNACIÓN DEL    </b>");
                    impriCovid19.AppendLine("<b>     GOBIERNO NACIONAL 9-100   </b>");
                    impriCovid19.AppendLine("<b>    PARA PREVENIR EL COVID 19       </b>");
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine(string.Concat("Cedula: " + vacuna.CEDULA));
                    impriCovid19.AppendLine(string.Concat("Nombres: " + vacuna.NOM_PADRON));
                    impriCovid19.AppendLine(string.Concat("Provincia: " + vacuna.NOM_PROVINCIA));
                    impriCovid19.AppendLine(string.Concat("Cantón: " + vacuna.NOM_CANTON));
                    impriCovid19.AppendLine(string.Concat("Parroquia: " + vacuna.NOM_PARROQUIA));
                    impriCovid19.AppendLine(string.Concat("Recinto: " + vacuna.NOM_RECINTO));
                    impriCovid19.AppendLine(string.Concat("Dirección: " + vacuna.DIR_RECINTO));
                    impriCovid19.AppendLine(string.Concat("1era. Dosis: " + vacuna.DOSIS_1));
                    impriCovid19.AppendLine(string.Concat("2da. Dosis: " + vacuna.DOSIS_2));
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine("");
                    impriCovid19.AppendLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");
                }
            }

            this.ReciboCovid19 = regex.Replace(this.ReciboCovid19, impriCovid19.ToString());
            this.Recibo = regex.Replace(this.Recibo, "");


            string permite_almacen = db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC" && k.parametro2 == this.Establecimiento).FirstOrDefault().valor;
            bool permitedsctocliente = db.TblDiscountCards.Any(x => x.itemid == this.ClienteIdentificacion);

            if (permite_almacen == "TRUE" && permitedsctocliente)
            {

                DateTime fechatemp = DateTime.Today;
                DateTime fecha1 = new DateTime(fechatemp.Year, fechatemp.Month, 1);
                DateTime fecha2 = fecha1.AddMonths(1);
                Decimal ValorVendidoMensual = db.core_factura.Where(x => x.cliente == this.ClienteIdentificacion
                    && x.fecha_creacion >= fecha1 && x.fecha_creacion < fecha2).Sum(x => x.base0 + x.base12);
                Decimal ValorAhorroMensual = db.core_factura.Where(x => x.cliente == this.ClienteIdentificacion
                    && x.fecha_creacion >= fecha1 && x.fecha_creacion < fecha2).Sum(x => x.descuento + x.descuento2);
                Decimal ValorPagadoMensual = db.core_factura.Where(x => x.cliente == this.ClienteIdentificacion
                    && x.fecha_creacion >= fecha1 && x.fecha_creacion < fecha2).Sum(x => x.subtotal);

                Decimal ventamensual = Decimal.Parse(db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC_VENTA_MENSUAL" && k.parametro2 == this.Establecimiento).FirstOrDefault().valor);
                this.Recibo = this.Recibo + "\n\r========   ACUMULADO DEL MES   ========= ";
                this.Recibo = this.Recibo + "\n\rComprado: $ " + ValorVendidoMensual.ToString();
                this.Recibo = this.Recibo + "\n\r  Ahorro: $ " + ValorAhorroMensual.ToString();
                this.Recibo = this.Recibo + "\n\r  Pagado: $ " + ValorPagadoMensual.ToString();
                this.Recibo = this.Recibo + "\n\r======================================== ";

                /*
                 *             this.Recibo = this.Recibo + "\n\rComprado: $ " + this.getSubTotalSinDescuento().ToString();
                 *             this.Recibo = this.Recibo + "\n\r  Ahorro: $ " + this.getDescuentos().ToString();
                 *             this.Recibo = this.Recibo + "\n\r  Pagado: $ " + this.getSubTotal().ToString();
                 */
                /*
               this.Recibo = this.Recibo + "\n\r======================================== ";
               this.Recibo = this.Recibo + "\n\rSu Acumulado de compras es: $" + ValorVendidoMensual.ToString();
               this.Recibo = this.Recibo + "\n\rPara completar: $" + ventamensual.ToString();
               this.Recibo = this.Recibo + "\n\rDebe Realizar compras por:$" + (ventamensual - ValorVendidoMensual).ToString();
               this.Recibo = this.Recibo + "\n\r========================================";
               */
            }

            if (POS.Control.Common.GlobalParameters.DatoRetencionEnFactura == true)
            {
                var reciboDatoRetencion = db.core_recibo.Where(x => x.identificador == "T_DATO_RETENCION").FirstOrDefault();
                if (reciboDatoRetencion != null)
                {
                    this.Recibo = this.Recibo + "\n\r" + reciboDatoRetencion.cuerpo;

                    this.Recibo = this.Recibo.Replace("<<iva>>", Control.Common.StringHelper.DevolverConPadding((this.getIVA()).ToString("N2"), 28));
                    this.Recibo = this.Recibo.Replace("<<base1>>", Control.Common.StringHelper.DevolverConPadding((retencionSubtotalBase1).ToString("N2"), 28));
                    this.Recibo = this.Recibo.Replace("<<base175>>", Control.Common.StringHelper.DevolverConPadding((retencionSubtotalBase175).ToString("N2"), 28));
                }
            }

        }

        public string preparaTextoVoucher(string texto, string tipo_pago)
        {
            var db = new POSEntities();

            texto = texto.Replace("<<oficina>>", this.Establecimiento_nombre);
            texto = texto.Replace("<<factura>>", this.GetNumeroFactura());
            texto = texto.Replace("<<cajero>>", this.User.nombres);
            if (tipo_pago == "TAR PORTAL")
                texto = texto.Replace("<<factura_fecha>>", this.Fecha.ToString() + "\n========================================\nEmpresa: " + db.core_empresacredito.FirstOrDefault(x => x.id == _tarjetaCreditoInterno.empresa_id).razon_social);
            else
                texto = texto.Replace("<<factura_fecha>>", this.Fecha.ToString());

            texto = texto.Replace("<<cedula>>", (_tarjetaCreditoInternoAdicional == null) ? this.ClienteIdentificacion : ((tipo_pago == "TAR PORTAL") ? _tarjetaCreditoInterno.identificacion : this.ClienteIdentificacion));
            texto = texto.Replace("<<factura_subtotal>>", String.Format("{0,10:0.00}", (this.getSubTotalSinDescuento() - this.GetDescuentos() - this.getDescuentos2()).ToString("N2")));
            texto = texto.Replace("<<factura_iva>>", String.Format("{0,10:0.00}", this.getIVA().ToString("N2")));
            texto = texto.Replace("<<factura_total>>", String.Format("{0,10:0.00}", this.GetTotal().ToString("N2")));
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");
            StringBuilder pagos = new StringBuilder();
            decimal PagoTar = 0M;
            foreach (var pago in this.Pagos)
            {
                if (tipo_pago == pago.Descripcion)
                {
                    pagos.AppendLine(pago.Descripcion + " : " + String.Format("{0,10:0.00}", pago.Valor.ToString("N2")));
                    if (tipo_pago == "GIFT CARD" || tipo_pago == "GIFT CARDV")
                    {
                        if (pago.Pagos != null)
                        {
                            if (pago.Pagos.Count > 0)
                            {
                                _tarjetaRegalo = new TarjetaRegalo();
                                var tarjeta = pago.Pagos.Select(x => x.Codigo).First();
                                var result = _tarjetaRegalo.getSaldoTotalTarjetaGiftCardGen(tarjeta, this.ClienteIdentificacion, this.EsUsoAppMovil);

                                decimal saldoPgo = (pago.Pagos).Sum(x => (decimal?)((PagoGiftCard)x).Saldo) ?? 0M;
                                pagos.AppendLine("SALDO      : " + String.Format("{0,10:0.00}", ((pago.Pagos).Sum(x => (decimal?)((PagoGiftCard)x).Saldo) ?? 0M).ToString("N2")));

                                
                            }
                        }
                    }

                    if (tipo_pago == "TAR PORTAL")
                    {
                        PagoTar = pago.Valor;
                    }
                }
            }


            if (tipo_pago == "TAR PORTAL")
            {
                string cadena = ((_tarjetaCreditoInternoAdicional == null) ? this.Cliente_nombre : _tarjetaCreditoInterno.nombre_tarjeta);
                if (_tarjetaCreditoInternoAdicional != null)
                {
                    var reciboTarAdicional = db.core_recibo.Where(x => x.identificador == "VOUCHER_TARJETAADICIONAL").FirstOrDefault();
                    if (reciboTarAdicional != null)
                    {
                        cadena = cadena +
                                    reciboTarAdicional
                                        .cuerpo
                                        .Replace("<<cedulaAdicional>>", _tarjetaCreditoInternoAdicional.identificacion)
                                        .Replace("<<clienteAdicional>>", _tarjetaCreditoInternoAdicional.nombre_tarjeta)
                                        .Replace("<<saldoAdicional>>", (_tarjetaCreditoInternoAdicional.saldo - PagoTar).ToString("N2"));
                    }
                }
                else
                {
                    cadena = cadena + "\n========================================\nSaldo Tarjeta: " + (_tarjetaCreditoInterno.saldo - PagoTar).ToString("N2");
                }
                texto = texto.Replace("<<cliente>>", cadena);
            }
            else
                texto = texto.Replace("<<cliente>>", this.Cliente_nombre);
            texto = regex.Replace(texto, pagos.ToString());
            return texto;
        }

        private void addCL(DSS.Controles.Impresion.DSSPrint printer, StringBuilder s, string text)
        {
            s.AppendLine(text);
            if (printer.PrinterSettings.PrinterName.Contains("Generic"))
                s.Append("\n");
        }

        public void PrintWonTicket()
        {
            var db = new POSEntities();
            bool flag = false;

            if (activeInvoiceWinner == true)
            {
                DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();
                printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 8, FontStyle.Bold);
                StringBuilder win = new StringBuilder();

                var query = (from tran in db.TblInvoiceCounters
                             where tran.establishment == this.Establecimiento
                             select tran).FirstOrDefault();

                query.counter = query.counter + 1;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "PrintWonTicket", "Imposible ejecutar SaveChanges para actualizar campo 'counter' en 'TblInvoiceCounters', a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                }

                if (flagMain == true)
                {
                    // decimal a = 10M;
                    flag = true;
                    //if (this.Establecimiento != "012")
                    //{
                    //    addCL(printer, win, "<footer>" + " *** ¡GANASTE! ***" + "</footer>");
                    //    addCL(printer, win, " ");
                    //    addCL(printer, win, "========================================");
                    //    addCL(printer, win, "        OLIMPIADA DEL PORTAL");
                    //    addCL(printer, win, "Con la compra que acabaste de realizar, ");
                    //    addCL(printer, win, " DELPORTAL te premia con un");
                    //    addCL(printer, win, " SUPER BALON DEL PORTAL");
                    //    addCL(printer, win, "========================================");
                    //    addCL(printer, win, "              ¡Felicidades! ");
                    //    addCL(printer, win, "========================================");
                    //}
                    //else
                    {


                        addCL(printer, win, "<footer>" + " *** ¡GANASTE! ***" + "</footer>");
                        addCL(printer, win, " ");
                        addCL(printer, win, "========================================");
                        addCL(printer, win, "Con la compra que acabaste de realizar, ");
                        addCL(printer, win, "        DELPORTAL te premia.");
                        addCL(printer, win, "     HAS GANADO UN MINUTO DE");
                        addCL(printer, win, "         COMPRAS GRATIS.");
                        addCL(printer, win, "         FELIZ NAVIDAD!");
                        addCL(printer, win, "========================================");
                        addCL(printer, win, "              ¡Felicidades! ");
                        addCL(printer, win, "========================================");

                    }

                    //addCL(printer, win, "<footer>" + "      $" + this.getTotal().ToString() + "" + "</footer>");
                    //addCL(printer, win, "  *** Devolución Abonos PaviPLAN ***");
                    //addCL(printer, win, " ");
                    //addCL(printer, win, "========================================");
                    //addCL(printer, win, "<footer>" + "     GIFT CARD " + "</footer>");
                    //addCL(printer, win, " ");
                    //addCL(printer, win, "<footer>" + "      $" + a + "</footer>");
                    //addCL(printer, win, " ");

                    /*  aqui se genera la gift card
                    Models.SP_GENERABARCODE_Result res;
                    res = db.SP_GENERABARCODE(this.getTotal()).ToList().FirstOrDefault();  
                    //res = db.SP_GENERABARCODE(a).ToList().FirstOrDefault();
                    Barcode bc = new Barcode();
                    string coded;
                    coded = bc.encodeString(res.CODIGO);
                    //resource = res.CODIGO.ToString();

                    //addCL(printer, win, "<barcode>" + "            " + coded.ToString() + "</barcode>");
                     */
                    addCL(printer, win, " ");
                    addCL(printer, win, " ");
                    addCL(printer, win, " ");
                    /* addCL(printer, win, "             " + resource);
                     addCL(printer, win, "========================================");
                     addCL(printer, win, "Realiza  tu  compra  en  cualquiera  de");
                     addCL(printer, win, "nuestros locales y paga con este ticket.");
                     addCL(printer, win, "========================================");*/
                    //addCL(printer, win, "  Válida hasta el 31 de marzo de 2016.");
                    // addCL(printer, win, "  Validez: 3 meses desde su emisión");
                    addCL(printer, win, "       GRACIAS POR ELEGIRNOS");
                    //addCL(printer, win, "           ¡Feliz Navidad!");
                    this.Recibo = win.ToString();
                }
                else
                {
                    addCL(printer, win, "=====================================");
                    addCL(printer, win, " ¡Sigue  comprando en  DELPORTAL, tú");
                    addCL(printer, win, " puedes  ser  el  próximo ganador de");

                    //if (this.Establecimiento != "012")
                    //{
                    //    addCL(printer, win, "   nuestra promoción OLIMPIADAS ");
                    //    addCL(printer, win, " DEL PORTAL y recibir una BALON gratis.");

                    //}
                    //else
                    {
                        addCL(printer, win, "     nuestra promoción  ");
                        addCL(printer, win, " CARRITO DE COMPRA GRATIS.");
                        addCL(printer, win, " ¡No dejes de comprar! ");

                    }
                    addCL(printer, win, "=====================================");
                    addCL(printer, win, "        GRACIAS POR ELEGIRNOS");
                    //   addCL(printer, win, "       ¡OLIMPIADAS DEL PORTAL!");
                    this.Recibo = win.ToString();
                }

                //printer.TextToPrint = win.ToString();
                //printer.Print();

                if (flag == true)
                {
                    var query3 = (from tran2 in db.TblInvoiceRandoms
                                  where tran2.won == false
                                  && tran2.establishment == this.Establecimiento
                                  && tran2.date <= this.Fecha
                                  select tran2).FirstOrDefault();

                    query3.won = true;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "PrintWonTicket", "Imposible ejecutar SaveChanges para actualizar campo 'won' en 'TblInvoiceCounters', a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    }

                    TblInvoiceWinner winner = new TblInvoiceWinner();
                    winner.invoicedate = this.Fecha;
                    winner.establishment = this.Establecimiento.ToString();
                    winner.emisionseries = this.PtoEmision.ToString();
                    winner.invoiceid = this.Secuencia.ToString();
                    winner.customer = this.ClienteIdentificacion;
                    winner.resource = resource;
                    winner.value = GetTotal();
                    db.TblInvoiceWinners.Add(winner);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "PrintWonTicket", "Imposible agregar registro de ganador nuevo en tabla 'TblInvoiceWinner', a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    }
                }
                else
                {
                    return;
                }
            }
        }

        public string preparaTextoCorteCaja(string texto)
        {
            using (var db = new POSEntities())
            {

                texto = texto.Replace("<<oficina>>", this.Establecimiento_nombre);
                texto = texto.Replace("<<factura>>", this.GetNumeroFactura());
                texto = texto.Replace("<<cajero>>", this.User.nombres);
                texto = texto.Replace("<<factura_fecha>>", DateTime.Now.ToString());
                var viewFac = db.ViewFacturasUsuarios.Where(x => x.usuario == this.User.username && x.establecimiento == this.Establecimiento && x.fecha.Value.Year == DateTime.Now.Year && x.fecha.Value.Month == DateTime.Now.Month && x.fecha.Value.Day == DateTime.Now.Day);

                System.Text.RegularExpressions.Regex regexfact = new System.Text.RegularExpressions.Regex(@"<plantillaFact>(.*)</plantillaFact>",
                System.Text.RegularExpressions.RegexOptions.Singleline);
                StringBuilder fact = new StringBuilder();
                decimal total = 0M;
                foreach (var factura in viewFac)
                {
                    fact.AppendLine("Punto Emision:" + factura.punto_emision);
                    fact.AppendLine("Numero Facturas: #" + String.Format("{0,8:0.00}", factura.numerofactura));
                    fact.AppendLine("Total Facturado: $" + String.Format("{0,8:0.00}", factura.Total));
                    fact.AppendLine("");
                    total += decimal.Parse(factura.Total.ToString());

                }
                fact.AppendLine("Total Acumulado: $" + String.Format("{0,8:0.00}", total));
                texto = regexfact.Replace(texto, fact.ToString());

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)</plantillaPago>",
                System.Text.RegularExpressions.RegexOptions.Singleline);

                StringBuilder pagos = new StringBuilder();

                var viewUs = db.ViewFacturaPagoUsuarios.Where(x => x.usuario == this.User.username && x.establecimiento == this.Establecimiento && x.fecha.Value.Year == DateTime.Now.Year && x.fecha.Value.Month == DateTime.Now.Month && x.fecha.Value.Day == DateTime.Now.Day);
                foreach (var pago in viewUs)
                {
                    pagos.AppendLine(pago.PAYMMODE.Substring(0, pago.PAYMMODE.Length > 25 ? 25 : pago.PAYMMODE.Length) + " : " + String.Format("{0,8:0.00}", pago.valor.ToString()));

                    foreach (var detalle in db.core_parametro.Where(x => x.identificador == "CORTE_DETALLE_PAGO" && x.valor.Equals(pago.PAYMMODE)))
                    {
                        foreach (var registro in db.ViewPagoDetalleUsuarios.Where(x => x.tipo_id == detalle.valor && x.usuario == this.User.username && x.establecimiento == this.Establecimiento && x.fecha.Value.Year == DateTime.Now.Year && x.fecha.Value.Month == DateTime.Now.Month && x.fecha.Value.Day == DateTime.Now.Day))
                        {
                            pagos.AppendLine(" - " + registro.datos.Substring(0, registro.datos.Length > 25 ? 25 : registro.datos.Length)+ "  $" + String.Format("{0,8:0.00}", registro.valor));
                        }
                    }
                }

                if (POS.Control.Common.Promo.EstaActivaPromoPaviPlan())
                {
                    if (db.vw_CreditoLocalesPagos.Where(x => x.CAJERO == this.User.username) != null)
                    {
                        decimal total_paviplan = 0M;
                        decimal total_paviplan_efectivo = 0M;
                        decimal total_paviplan_tarjeta = 0M;
                        decimal total_paviplan_cheque = 0M;
                        foreach (var paviplan in db.vw_CreditoLocalesPagos.Where(x => x.CAJERO == this.User.username && x.ESTABLECIMIENTO == this.Establecimiento && x.FECHA.Year == DateTime.Now.Year && x.FECHA.Month == DateTime.Now.Month && x.FECHA.Day == DateTime.Now.Day))
                        {
                            total_paviplan += decimal.Parse(paviplan.MONTO.ToString());
                        }
                        foreach (var pavi_efectivo in db.vw_CreditoLocalesPagos.Where(x => x.CAJERO == this.User.username && x.ESTABLECIMIENTO == this.Establecimiento && x.FECHA.Year == DateTime.Now.Year && x.FECHA.Month == DateTime.Now.Month && x.FECHA.Day == DateTime.Now.Day && x.FORMAPAGO == 1))
                        {
                            total_paviplan_efectivo += decimal.Parse(pavi_efectivo.MONTO.ToString());
                        }
                        foreach (var pavi_tarjeta in db.vw_CreditoLocalesPagos.Where(x => x.CAJERO == this.User.username && x.ESTABLECIMIENTO == this.Establecimiento && x.FECHA.Year == DateTime.Now.Year && x.FECHA.Month == DateTime.Now.Month && x.FECHA.Day == DateTime.Now.Day && x.FORMAPAGO == 2))
                        {
                            total_paviplan_tarjeta += decimal.Parse(pavi_tarjeta.MONTO.ToString());
                        }
                        foreach (var pavi_cheque in db.vw_CreditoLocalesPagos.Where(x => x.CAJERO == this.User.username && x.ESTABLECIMIENTO == this.Establecimiento && x.FECHA.Year == DateTime.Now.Year && x.FECHA.Month == DateTime.Now.Month && x.FECHA.Day == DateTime.Now.Day && x.FORMAPAGO == 3))
                        {
                            total_paviplan_cheque += decimal.Parse(pavi_cheque.MONTO.ToString());
                        }

                        pagos.AppendLine("");
                        pagos.AppendLine("*************** PAVIPLAN ***************");
                        pagos.AppendLine("");
                        pagos.AppendLine("Total Acumulado: $" + String.Format("{0,7:0.00}", Math.Round(total_paviplan, 2)));
                        pagos.AppendLine("");
                        pagos.AppendLine("FORMAS DE PAGO");
                        pagos.AppendLine("Efectivo       : $" + String.Format("{0,7:0.00}", Math.Round(total_paviplan_efectivo, 2)));
                        pagos.AppendLine("T. Crédito     : $" + String.Format("{0,7:0.00}", Math.Round(total_paviplan_tarjeta, 2)));
                        pagos.AppendLine("Cheque         : $" + String.Format("{0,7:0.00}", Math.Round(total_paviplan_cheque, 2)));
                        pagos.AppendLine("");
                        pagos.AppendLine("DETALLE");
                        pagos.AppendLine("Tarjetas de Crédito:");

                        foreach (var detail in db.vw_CreditoLocalesPagos.Where(x => x.CAJERO == this.User.username && x.ESTABLECIMIENTO == this.Establecimiento && x.FECHA.Year == DateTime.Now.Year && x.FECHA.Month == DateTime.Now.Month && x.FECHA.Day == DateTime.Now.Day && x.FORMAPAGO == 2))
                        {
                            pagos.AppendLine("- " + detail.DETALLE.Substring(0, detail.DETALLE.Length > 25 ? 25 : detail.DETALLE.Length) + " $" + String.Format("{0,7:0.00}", Math.Round(detail.MONTO, 2)));
                        }

                        pagos.AppendLine("Cheques:");

                        foreach (var detail in db.vw_CreditoLocalesPagos.Where(x => x.CAJERO == this.User.username && x.ESTABLECIMIENTO == this.Establecimiento && x.FECHA.Year == DateTime.Now.Year && x.FECHA.Month == DateTime.Now.Month && x.FECHA.Day == DateTime.Now.Day && x.FORMAPAGO == 3))
                        {
                            pagos.AppendLine("- " + detail.DETALLE.Substring(0, detail.DETALLE.Length > 25 ? 25 : detail.DETALLE.Length) + " $" + String.Format("{0,7:0.00}", Math.Round(detail.MONTO, 2)));
                        }
                    }
                }

                texto = regex.Replace(texto, pagos.ToString());
            }
            return texto;

        }


        public string preparaTextoCorteLoteV2(string texto)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");
            StringBuilder pagos = new StringBuilder();
            string fecha = DateTime.Now.Date.ToString("yyyyMMdd");
            Tramas.ProcesoControl pc = new Tramas.ProcesoControl();


            try
            {
                using (var db = new POSEntities())
                {
                    texto = texto.Replace("<<oficina>>", this.Establecimiento_nombre);
                    texto = texto.Replace("<<factura>>", this.GetNumeroFactura());
                    texto = texto.Replace("<<cajero>>", this.User.nombres);
                    texto = texto.Replace("<<factura_fecha>>", DateTime.Now.ToString());

                    pc._numLoteDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_datafast.ToString();
                    pc._numLoteMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_medianet.ToString();
                    pc._secuenciaDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_datafast.ToString();
                    pc._secuenciaMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_broadnet.ToString();
                    pc._MIDDatafast = Control.Common.GlobalParameters.MID_DATAFAST;
                    pc._TIDDatafast = Control.Common.GlobalParameters.TID_DATAFAST;

                    pc._MIDMedianet = Control.Common.GlobalParameters.MID_MEDIANET;
                    pc._TIDMedianet = Control.Common.GlobalParameters.TID_MEDIANET;
                    pc._CID = "LIRISCID0" + this.Establecimiento + this.PtoEmision;


                    string grupoAux = "";
                    var hoy = DateTime.Now;
                    decimal valorgrupo = 0;
                    decimal valortotalmed = 0;
                    var cont = 0;
                    bool pie = false;


                    #region IMPRESION LOTES

                    string sqlQuery = string.Empty;
                    sqlQuery = "Exec dbo.sprConsultaPosVoucher '" + fecha + "' , '" + Program.ID_Caja_POS + "' , '" + pc._CID + "' , 0";
                    // var mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery).ToList();
                    DataSet dtsConsulta = Control.Common.General.GetDataSet(sqlQuery);

                    if (dtsConsulta.Tables.Count > 0)
                    {
                        string DscAutorizador = string.Empty;
                        string AUTORIZADOR = string.Empty;
                        string TipoConsulta = string.Empty;
                        string _numLoteMedianet = string.Empty;
                        string _TIDMedianet = string.Empty;
                        string _numLoteDatafast = string.Empty;
                        string _TIDDatafast = string.Empty;
                        string _CID = string.Empty;


                        for (int index = 0; index < dtsConsulta.Tables.Count - 1; index++)
                        {
                            TipoConsulta = dtsConsulta.Tables[index].Rows[0]["DET_VOUCER"].ToString();

                            string NumLote = string.Empty;
                            string TID = string.Empty;
                            string CID = string.Empty;
                            string MID = string.Empty;

                            if (TipoConsulta == "DET_VOUCER")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[index].Rows)
                                {

                                    string AUTORIZACION = data["AUTORIZACION"].ToString();
                                    string TIPOCONSUMO = data["TIPOCONSUMO"].ToString();
                                    string VALORCONSUMO = data["VALORCONSUMO"].ToString();
                                    long id = long.Parse(data["id"].ToString());

                                    pagos.AppendLine(AUTORIZACION + "     " + TIPOCONSUMO + "     " + VALORCONSUMO);

                                    var procesaMedianet = db.POS_VOUCHER.Where(x => x.id == id).FirstOrDefault();
                                    procesaMedianet.PROCESADOTURNO = true;
                                    db.SaveChanges();

                                }

                                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                                continue;
                            }

                            if (TipoConsulta == "DET_VOUCER_2")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[index].Rows)
                                {

                                    string AUTORIZACION = data["AUTORIZACION"].ToString();
                                    string TIPOCONSUMO = data["TIPOCONSUMO"].ToString();
                                    string VALORCONSUMO = data["VALORCONSUMO"].ToString();
                                    long id = long.Parse(data["id"].ToString());

                                    pagos.AppendLine(AUTORIZACION + "     " + TIPOCONSUMO + "     " + VALORCONSUMO);

                                    var procesaMedianet = db.POS_VOUCHER.Where(x => x.id == id).FirstOrDefault();
                                    procesaMedianet.PROCESADOTURNO = true;
                                    db.SaveChanges();

                                }

                                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);

                            }

                            if (TipoConsulta == "DET_VOUCER_3")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[index].Rows)
                                {

                                    string AUTORIZACION = data["AUTORIZACION"].ToString();
                                    string TIPOCONSUMO = data["TIPOCONSUMO"].ToString();
                                    string VALORCONSUMO = data["VALORCONSUMO"].ToString();
                                    long id = long.Parse(data["id"].ToString());

                                    pagos.AppendLine(AUTORIZACION + "     " + TIPOCONSUMO + "     " + VALORCONSUMO);

                                    var procesaMedianet = db.POS_VOUCHER.Where(x => x.id == id).FirstOrDefault();
                                    procesaMedianet.PROCESADOTURNO = true;
                                    db.SaveChanges();

                                }

                                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);

                            }



                            if (AUTORIZADOR == "1")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[index].Rows)
                                {
                                    NumLote = data["LOTE"].ToString();
                                    TID = data["TID"].ToString();
                                    CID = data["CID"].ToString();
                                    MID = data["MID"].ToString();
                                }
                            }



                        }




                    }

                    #endregion IMPRESION LOTES



                }

                return texto;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Error:" + ex.Message);

            }

            return texto;
        }
        public string preparaTextoCorteLote(string texto)
        {
            using (var db = new POSEntities())
            {
                texto = texto.Replace("<<oficina>>", this.Establecimiento_nombre);
                texto = texto.Replace("<<factura>>", this.GetNumeroFactura());
                texto = texto.Replace("<<cajero>>", this.User.nombres);
                texto = texto.Replace("<<factura_fecha>>", DateTime.Now.ToString());

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");
                StringBuilder pagos = new StringBuilder();

                string fecha = DateTime.Now.Date.ToString("yyyyMMdd");

                Tramas.ProcesoControl pc = new Tramas.ProcesoControl();
                pc._numLoteDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_datafast.ToString();
                pc._numLoteMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_medianet.ToString();
                pc._secuenciaDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_datafast.ToString();
                pc._secuenciaMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_broadnet.ToString();
                pc._MIDDatafast = Control.Common.GlobalParameters.MID_DATAFAST;
                pc._TIDDatafast = Control.Common.GlobalParameters.TID_DATAFAST;

                pc._MIDMedianet = Control.Common.GlobalParameters.MID_MEDIANET;
                pc._TIDMedianet = Control.Common.GlobalParameters.TID_MEDIANET;
                pc._CID = "LIRISCID0" + this.Establecimiento + this.PtoEmision;

                //pc._MIDDatafast = db.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == this.Establecimiento).FirstOrDefault().parametro2;
                //pc._TIDDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().TID_DATAFAST;
                //pc._MIDMedianet = db.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == this.Establecimiento).FirstOrDefault().parametro2;
                //pc._TIDMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().TID;
                //pc._CID = this.Establecimiento + this.PtoEmision;


                #region IMPRESION LOTES

                var hoy = DateTime.Now;
                string sqlQuery = string.Empty;
                sqlQuery = "Exec dbo.sprConsultaPosVoucher '" + fecha + "' , '" + Program.ID_Caja_POS + "' , '" + pc._CID + "' , 2";
                var mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery).ToList();


                string grupoAux = "";
                texto = texto.Replace("<<lote>>", pc._numLoteMedianet);
                texto = texto.Replace("<<tid>>", pc._TIDMedianet);
                texto = texto.Replace("<<cid>>", pc._CID);
                decimal valorgrupo = 0;
                decimal valortotalmed = 0;
                var cont = 0;
                bool pie = false;
                foreach (var linea in mov)//llega a esta linea y se sale del foreach
                {
                    if (grupoAux != linea.GRUPOTAR)
                    {
                        if (cont > 0)
                        {
                            pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                            valorgrupo = 0;
                        }
                        grupoAux = linea.GRUPOTAR;
                        pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                        cont += 1;

                        //sumatoria
                    }
                    decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                    pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));
                    if (pie && valorgrupo != 0)
                    {

                    }
                    valorgrupo += valorvoucher;
                    valortotalmed += valorvoucher;
                    linea.PROCESADOTURNO = true;

                    var procesaMedianet = db.POS_VOUCHER.Where(x => x.id == linea.id).FirstOrDefault();
                    procesaMedianet.PROCESADOTURNO = true;
                }

                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                pagos.AppendLine("\nTOTAL MEDIANET: " + valortotalmed);
                pagos.AppendLine("=======================================\n\n");
                pagos.AppendLine("<footer>DATAFAST</footer>");
                pagos.AppendLine("");
                pagos.AppendLine("LOTE    : " + pc._numLoteDatafast);
                pagos.AppendLine("TERMINAL: " + pc._TIDDatafast);
                pagos.AppendLine("=======================================\n\n");

                db.SaveChanges();

                sqlQuery = "Exec dbo.sprConsultaPosVoucher '" + fecha + "' , '" + Program.ID_Caja_POS + "' , '" + pc._CID + "' , 1";
                mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery).ToList();

                cont = 0;
                grupoAux = "";
                valorgrupo = 0;
                valortotalmed = 0;
                foreach (var linea in mov)//llega a esta linea y se sale del foreach
                {
                    if (grupoAux != linea.GRUPOTAR)
                    {
                        if (cont > 0)
                        {
                            pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                            valorgrupo = 0;
                        }
                        grupoAux = linea.GRUPOTAR;
                        pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                        cont += 1;

                        //sumatoria
                    }
                    decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                    pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));
                    if (pie && valorgrupo != 0)
                    {

                    }
                    valorgrupo += valorvoucher;
                    valortotalmed += valorvoucher;
                    linea.PROCESADOTURNO = true;

                    var procesaDatafast = db.POS_VOUCHER.Where(x => x.id == linea.id).FirstOrDefault();
                    procesaDatafast.PROCESADOTURNO = true;
                }

                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                pagos.AppendLine("\nTOTAL DATAFAST: " + valortotalmed);
                texto = regex.Replace(texto, pagos.ToString());


                sqlQuery = "Exec dbo.sprConsultaPosVoucher '" + fecha + "' , '" + Program.ID_Caja_POS + "' , '" + pc._CID + "' , 3";
                mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery).ToList();

                cont = 0;
                grupoAux = "";
                valorgrupo = 0;
                valortotalmed = 0;
                foreach (var linea in mov)//llega a esta linea y se sale del foreach
                {
                    if (grupoAux != linea.GRUPOTAR)
                    {
                        if (cont > 0)
                        {
                            pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                            valorgrupo = 0;
                        }
                        grupoAux = linea.GRUPOTAR;
                        pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                        cont += 1;

                        //sumatoria
                    }
                    decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                    pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));
                    if (pie && valorgrupo != 0)
                    {

                    }
                    valorgrupo += valorvoucher;
                    valortotalmed += valorvoucher;
                    linea.PROCESADOTURNO = true;

                    var procesaAustro = db.POS_VOUCHER.Where(x => x.id == linea.id).FirstOrDefault();
                    procesaAustro.PROCESADOTURNO = true;
                }

                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                pagos.AppendLine("\nTOTAL AUSTRO: " + valortotalmed);
                texto = regex.Replace(texto, pagos.ToString());
                // FIN: Lote del autorizador AUSTRO.  JM  22-12-2020

                #endregion
                int lote_datafast = 0;
                int lote_medianet = 0;


                var dbProceso = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault();

                lote_datafast = dbProceso.lote_datafast;
                lote_medianet = dbProceso.lote_medianet;

                dbProceso.lote_datafast += 1;
                dbProceso.lote_medianet += 1;
                db.SaveChanges();

                pc._numLoteDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_datafast.ToString();
                pc._numLoteMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_medianet.ToString();

                /// poner espacios en blanco # lote y secuencia de medianet
                pc._numLoteDatafast = "      ";
                pc._numLoteMedianet = "      "; ;
                pc._secuenciaDatafast = "      "; ;
                pc._secuenciaMedianet = "      ";

                string trama = pc.DevuelveTramaPCMultiRed;// "PC000001000001000001000001codigolirisdataiddata01codigolirismediiddata01CAJATEST01     ";
                int PUERTOCOM = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento).FirstOrDefault().puerto_pinpad;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLote", "Trama enviada:" + trama.ToString());
                ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();

                var pos = new POSEntities();
                int timeOut_ = 40000;
                int.TryParse(pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOut_);
                string resp = envio.SendRequestPinpad("", PUERTOCOM, timeOut_, trama, "", 1);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLote", "Trama recibida:" + resp.ToString());
            }

            return texto;

        }

        public string preparaTextoCorteLoteNoDefinitivo(string texto)
        {
            using (var db = new POSEntities())
            {
                texto = texto.Replace("<footer>REPORTE DE TOTALES</footer>", "<footer>REPORTE DE TOTALES</footer>\n<footer>PARCIAL</footer>");
                texto = texto.Replace("<<oficina>>", this.Establecimiento_nombre);
                texto = texto.Replace("<<factura>>", this.GetNumeroFactura());
                texto = texto.Replace("<<cajero>>", this.User.nombres);
                texto = texto.Replace("<<factura_fecha>>", DateTime.Now.ToString());

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");
                StringBuilder pagos = new StringBuilder();

                string fecha = DateTime.Now.Date.ToString("yyyyMMdd");
                Tramas.ProcesoControl pc = new Tramas.ProcesoControl();
                pc._numLoteDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_datafast.ToString();
                pc._numLoteMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_medianet.ToString();
                pc._secuenciaDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_datafast.ToString();
                pc._secuenciaMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_broadnet.ToString();
                pc._MIDDatafast = db.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == this.Establecimiento).FirstOrDefault().parametro2;
                pc._TIDDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().TID_DATAFAST;
                pc._MIDMedianet = db.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == this.Establecimiento).FirstOrDefault().parametro2;
                pc._TIDMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().TID;
                pc._CID = this.Establecimiento + this.PtoEmision;


                #region IMPRESION LOTES

                // Buscar los voucher por el IdCaja.    JM  17-09-2019
                //var mov = db.POS_VOUCHER.Where(x => x.FECHACONSUMO == fecha && x.AUTORIZADOR == 2 && x.PROCESADOTURNO == false && x.PUNTOEMISION == pc._CID).OrderBy(y => y.GRUPOTAR + y.AUTORIZACION + y.TIPOCONSUMO);            
                var posvoucher = db.POS_VOUCHER.Where(x => x.FECHACONSUMO == fecha && x.AUTORIZADOR == 2 && x.PROCESADOTURNO == false && x.PUNTOEMISION == pc._CID).OrderBy(y => y.GRUPOTAR + y.AUTORIZACION + y.TIPOCONSUMO).AsEnumerable();
                var hoy = DateTime.Now;
                var corefactura = db.core_factura.Where(x => x.establecimiento == this.Establecimiento && x.punto_emision == this.PtoEmision
                                   && (DateTime?)System.Data.Entity.DbFunctions.TruncateTime(x.fecha_creacion) == hoy.Date && x.msgError == Program.ID_Caja_POS).AsEnumerable();

                /* var mov = (from a in corefactura
                            join b in posvoucher on "F-" + a.establecimiento + "-" + a.punto_emision + "-" + a.numero.ToString().PadLeft(9, '0') equals b.FACTURA
                            where  a.msgError == Program.ID_Caja_POS
                            select b).OrderBy(y => y.GRUPOTAR + y.AUTORIZACION + y.TIPOCONSUMO);
                            */
                string sqlQuery = string.Empty;
                sqlQuery = " select a.* FROM core_factura as  b  WITH(nolock) join  pos_voucher as a  WITH(nolock) on b.establecimiento =  SUBSTRING(a.factura, 3,3)  AND b.punto_emision =  SUBSTRING(a.factura, 7,3) and b.numero = CAST(SUBSTRING(a.factura,11,9) as int) ";
                //sqlQuery += " WHERE a.FECHACONSUMO = convert(varchar, b.fecha_creacion, 112) ";
                sqlQuery += " WHERE a.FECHACONSUMO = '" + fecha + "'";
                sqlQuery += "  AND b.msgError = '" + Program.ID_Caja_POS + "'";
                sqlQuery += "  AND a.AUTORIZADOR = 2 ";
                sqlQuery += "  AND a.PROCESADOTURNO = 0 ";
                sqlQuery += "  AND a.PUNTOEMISION = '" + pc._CID + "'";
                sqlQuery += " ORDER BY a.GRUPOTAR + a.AUTORIZACION + a.TIPOCONSUMO";

                var mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery)
                            .ToList();

                string grupoAux = "";
                texto = texto.Replace("<<lote>>", pc._numLoteMedianet);
                texto = texto.Replace("<<tid>>", pc._TIDMedianet);
                texto = texto.Replace("<<cid>>", pc._CID);
                decimal valorgrupo = 0;
                decimal valortotalmed = 0;
                var cont = 0;

                foreach (var linea in mov)//llega a esta linea y se sale del foreach
                {
                    if (grupoAux != linea.GRUPOTAR)
                    {
                        if (cont > 0)
                        {
                            pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                            valorgrupo = 0;
                        }
                        grupoAux = linea.GRUPOTAR;
                        pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                        cont += 1;

                        //sumatoria
                    }
                    decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                    pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));

                    valorgrupo += valorvoucher;
                    valortotalmed += valorvoucher;

                }
                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                pagos.AppendLine("\nTOTAL MEDIANET: " + valortotalmed);
                pagos.AppendLine("=======================================\n\n");
                pagos.AppendLine("<footer>DATAFAST</footer>");
                pagos.AppendLine("");
                pagos.AppendLine("LOTE    : " + pc._numLoteDatafast);
                pagos.AppendLine("TERMINAL: " + pc._TIDDatafast);
                pagos.AppendLine("=======================================\n\n");


                // Buscar los voucher por el IdCaja.    JM  17-09-2019
                //mov = db.POS_VOUCHER.Where(x => x.FECHACONSUMO == fecha && x.AUTORIZADOR == 1 && x.PROCESADOTURNO == false && x.PUNTOEMISION == pc._CID).OrderBy(y => y.GRUPOTAR + y.AUTORIZACION + y.TIPOCONSUMO);

                //posvoucher = db.POS_VOUCHER.Where(x => x.FECHACONSUMO == fecha && x.AUTORIZADOR == 1 && x.PROCESADOTURNO == false && x.PUNTOEMISION == pc._CID).OrderBy(y => y.GRUPOTAR + y.AUTORIZACION + y.TIPOCONSUMO).AsEnumerable();                                              
                sqlQuery = " select a.* FROM core_factura as  b  WITH(nolock) join  pos_voucher as a  WITH(nolock) on b.establecimiento =  SUBSTRING(a.factura, 3,3)  AND b.punto_emision =  SUBSTRING(a.factura, 7,3) and b.numero = CAST(SUBSTRING(a.factura,11,9) as int) ";
                //sqlQuery += " WHERE a.FECHACONSUMO = convert(varchar, b.fecha_creacion, 112) ";
                sqlQuery += " WHERE a.FECHACONSUMO = '" + fecha + "'";
                sqlQuery += "  AND b.msgError = '" + Program.ID_Caja_POS + "'";
                sqlQuery += "  AND a.AUTORIZADOR = 1 ";
                sqlQuery += "  AND a.PROCESADOTURNO = 0 ";
                sqlQuery += "  AND a.PUNTOEMISION = '" + pc._CID + "'";
                sqlQuery += " ORDER BY a.GRUPOTAR + a.AUTORIZACION + a.TIPOCONSUMO";

                /* 
                mov = (from a in corefactura
                            join b in posvoucher on "F-" + a.establecimiento + "-" + a.punto_emision + "-" + a.numero.ToString().PadLeft(9, '0') equals b.FACTURA
                            where a.msgError == Program.ID_Caja_POS
                            select b).OrderBy(y => y.GRUPOTAR + y.AUTORIZACION + y.TIPOCONSUMO);
                            */
                mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery)
                         .ToList();

                cont = 0;
                grupoAux = "";
                valorgrupo = 0;
                valortotalmed = 0;
                foreach (var linea in mov)//llega a esta linea y se sale del foreach
                {
                    if (grupoAux != linea.GRUPOTAR)
                    {
                        if (cont > 0)
                        {
                            pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                            valorgrupo = 0;
                        }
                        grupoAux = linea.GRUPOTAR;
                        pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                        cont += 1;

                        //sumatoria
                    }
                    decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                    pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));

                    valorgrupo += valorvoucher;
                    valortotalmed += valorvoucher;
                }
                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                pagos.AppendLine("\nTOTAL DATAFAST: " + valortotalmed);
                pagos.AppendLine("\n****** NO ES REPORTE DEFINITIVO ******");
                texto = regex.Replace(texto, pagos.ToString());
                #endregion

            }
            return texto;

        }

        public string preparaTextoCupon(core_promocionticket ticket)
        {
            var db = new POSEntities();

            string texto = ticket.descripcion_larga;
            texto = texto.Replace("<<titulo>>", ticket.descripcion);
            texto = texto.Replace("<<unidad>>", ticket.ismoneda.Value ? "$" : "Libras");
            texto = texto.Replace("<<valor>>", String.Format("{0,5:0.00}", ticket.valor.ToString("N2")));

            if (db.core_parametro.Where(x => x.identificador == "CUPON_LLENO" && x.valor == "TRUE").FirstOrDefault() != null)
            {
                var query = (from q in db.pos_customer
                             where q.ACCOUNTNUM == ClienteIdentificacion
                             select q).FirstOrDefault();

                if (ClienteIdentificacion != "9999999999999")
                {
                    //DialogResult dr = MessageBox.Show("¿Desea el cupón con los datos del cliente?", "Promoción", MessageBoxButtons.YesNo);

                    //if (dr == DialogResult.Yes)
                    //{
                    texto = texto.Replace("<<nombre>>", query.NAME);
                    texto = texto.Replace("<<telefono>>", query.PHONE);
                    texto = texto.Replace("<<email>>", query.EMAIL);
                    texto = texto.Replace("<<cedula>>", query.ACCOUNTNUM);
                    //}
                    //else
                    //{
                    //texto = texto.Replace("<<nombre>>", "___________________________");
                    //texto = texto.Replace("<<telefono>>", "___________________________");
                    //texto = texto.Replace("<<email>>", "___________________________");
                    //texto = texto.Replace("<<cedula>>", "____________________");
                    //}
                }
                else
                {
                    texto = texto.Replace("<<nombre>>", "___________________________");
                    texto = texto.Replace("<<telefono>>", "___________________________");
                    texto = texto.Replace("<<email>>", "___________________________");
                    texto = texto.Replace("<<cedula>>", "____________________");
                }
            }

            return texto;
        }
        public void prepararCorte()
        {
            var db = new POSEntities();
            if (db.core_recibo.Any(x => x.identificador == "CORTE_CAJA"))
            {
                this.ReciboCorte = preparaTextoCorteCaja(db.core_recibo.First(x => x.identificador == "CORTE_CAJA").cuerpo);
            }
        }
        public void prepararCorteLote()
        {
            var db = new POSEntities();
            var CorteLote = (from deta in db.core_recibo
                             where deta.identificador == "CORTE_LOTE"
                             select deta).ToList().FirstOrDefault();

            if (CorteLote != null)
            {
                bool isMultiRed = Control.Common.GlobalParameters.PINPAD_MULTIRED;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "Ejecuta metodo EjecutaCierreCaja-PreparaTextoCorteLoteV2 ", "Ejecuta metodo EjecutaCierreCaja, cierra lote PINPAD ");

                var cierre = EjecutaCierreCaja(isMultiRed);

                if (cierre == false)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "prepararCorteLote ", "Al ejecutar el cierre de LOTE del PINPAD, genero error.");
                }

                //this.ReciboCorteLote = preparaTextoCorteLote(CorteLote.cuerpo);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "Ejecuta metodo EjecutaCierreCaja-PreparaTextoCorteLoteV2 ", "Ejecuta metodo preparaTextoCorteLoteV2");
                this.ReciboCorteLote = preparaTextoCorteLoteV2(CorteLote.cuerpo, isMultiRed);
                //this.ReciboCorteLote = preparaTextoCorteLote(CorteLote.cuerpo);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "Ejecuta metodo EjecutaCierreCaja-PreparaTextoCorteLoteV2 ", "Corte Lote POS y PINPAD realizado corretamente ");

                // 
            }
        }

        public bool EjecutaCierreCaja(bool isMultiRed)
        {
            bool bollReturn = false;

            Tramas.ProcesoControl procesoControl = new Tramas.ProcesoControl();
            string strTrama = string.Empty;

            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "EjecutaCierreCaja-EjecutaCierreCaja ", "Inicia preparando informacion para la trama de cierre ");

                using (var db = new POSEntities())
                {
                    var detPtoEmision = (from ptoEmi in db.core_puntoemision
                                         where ptoEmi.establecimiento_id == this.Establecimiento
                                         && ptoEmi.punto_emision == this.PtoEmisionOrigen
                                         select ptoEmi).ToList().FirstOrDefault();

                    procesoControl = new Tramas.ProcesoControl();
                    procesoControl._numLoteDatafast = detPtoEmision.lote_datafast.ToString();
                    procesoControl._numLoteMedianet = detPtoEmision.lote_medianet.ToString();
                    procesoControl._secuenciaDatafast = detPtoEmision.secuencia_datafast.ToString();
                    procesoControl._secuenciaMedianet = detPtoEmision.secuencia_broadnet.ToString();
                    procesoControl._MIDDatafast = Control.Common.GlobalParameters.MID_DATAFAST;
                    procesoControl._TIDDatafast = Control.Common.GlobalParameters.TID_DATAFAST;
                    procesoControl._MIDMedianet = Control.Common.GlobalParameters.MID_MEDIANET;
                    procesoControl._TIDMedianet = Control.Common.GlobalParameters.TID_MEDIANET;

                    if (string.IsNullOrEmpty(Control.Common.GlobalParameters.CID))
                    {
                        Control.Common.GlobalParameters.CID = this.Establecimiento + this.PtoEmision;
                    }

                    procesoControl._CID = Control.Common.GlobalParameters.CID;


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_numLoteDatafast: " + detPtoEmision.lote_datafast.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_numLoteMedianet: " + detPtoEmision.lote_medianet.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_secuenciaDatafast: " + detPtoEmision.secuencia_datafast.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_secuenciaMedianet: " + detPtoEmision.secuencia_broadnet.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_MIDDatafast: " + Control.Common.GlobalParameters.MID_DATAFAST);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_TIDDatafast: " + Control.Common.GlobalParameters.TID_DATAFAST);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_MIDMedianet: " + Control.Common.GlobalParameters.MID_MEDIANET);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_TIDMedianet: " + Control.Common.GlobalParameters.TID_MEDIANET);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_CID: " + Control.Common.GlobalParameters.CID);

                    if (isMultiRed)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Parametro Multired Activo");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Ejecuta metodo: procesoControl.DevuelveTramaMultiRed");
                        strTrama = procesoControl.DevuelveTramaPCMultiRed;
                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Parametro Multired Inactivo");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Ejecuta metodo: procesoControl.DevuelveTrama");
                        strTrama = procesoControl.DevuelveTramaPC;
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Trama Enviada:" + strTrama.ToString());

                    var envioGenResponse = new ClsEnviaPinPadGeneral();
                    PinPadRespuesta PagoResp = new PinPadRespuesta();

                    int timeOutCP = Control.Common.GlobalParameters.ConectContingente.TiempoOutCP;
                    string IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                    int PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                    PagoResp = envioGenResponse.EjecutaTrama(IPPinPad, PuertoPinPad, timeOutCP, strTrama, "", 1, "PC");

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Trama Respueta:" + PagoResp.TramaRespuesta.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "PagoResp.CodigoRespuesta; " + PagoResp.CodigoRespuesta.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "PagoResp.CodigoRespuestaEntidad; " + PagoResp.CodigoRespuestaEntidad.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "PagoResp.MensajeRespuesta; " + PagoResp.MensajeRespuesta.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "PagoResp.MensajeRespuestaEntidad; " + PagoResp.MensajeRespuestaEntidad.ToString());

                    if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "PagoResp.MensajeRespuestaEntidad; " + PagoResp.MensajeRespuestaEntidad.ToString());


                        bollReturn = false;
                        return bollReturn;
                    }

                    bollReturn = true;
                    return bollReturn;
                }
            }
            catch (Exception ex)
            {
                bollReturn = false;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Fatura", "preparaTextoCorteLoteV2", "Error Exceptio ; " + ex.Message);
                return bollReturn;
            }

        }
        public string preparaTextoCorteLoteV2(string texto, bool isMultiRed)
        {


            try
            {

                StringBuilder pagos = new StringBuilder();

                decimal valorgrupo = 0;
                decimal valortotalmed = 0;
                var cont = 0;
                bool pie = false;

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");
                string fecha = DateTime.Now.Date.ToString("yyyyMMdd");

                texto = texto.Replace("<<oficina>>", this.Establecimiento_nombre);
                texto = texto.Replace("<<factura>>", this.GetNumeroFactura());
                texto = texto.Replace("<<cajero>>", this.User.nombres);
                texto = texto.Replace("<<factura_fecha>>", DateTime.Now.ToString());

                //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Trama:" + texto.ToString());

                using (var db = new POSEntities())
                {
                    var detPtoEmision = (from ptoEmi in db.core_puntoemision
                                         where ptoEmi.establecimiento_id == this.Establecimiento
                                         && ptoEmi.punto_emision == this.PtoEmisionOrigen
                                         select ptoEmi).ToList().FirstOrDefault();



                    string _numLoteDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_datafast.ToString();
                    string _numLoteMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().lote_medianet.ToString();
                    string _secuenciaDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_datafast.ToString();
                    string _secuenciaMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().secuencia_broadnet.ToString();
                    string _MIDDatafast = db.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == this.Establecimiento).FirstOrDefault().parametro2;
                    string _TIDDatafast = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().TID_DATAFAST;
                    string _MIDMedianet = db.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == this.Establecimiento).FirstOrDefault().parametro2;
                    string _TIDMedianet = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault().TID;
                    string _CID = this.Establecimiento + this.PtoEmision;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_numLoteDatafast:" + _numLoteDatafast.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_numLoteMedianet:" + _numLoteMedianet.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_secuenciaDatafast:" + _secuenciaDatafast.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_secuenciaMedianet:" + _secuenciaMedianet.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_MIDDatafast:" + _MIDDatafast.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_TIDDatafast:" + _TIDDatafast.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_MIDMedianet:" + _MIDMedianet.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_TIDMedianet:" + _TIDMedianet.ToString());
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "_CID:" + _CID.ToString());

                    //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "texto:" + texto.ToString());
                    #region IMPRESION LOTES

                    var hoy = DateTime.Now;
                    string sqlQuery = string.Empty;
                    sqlQuery = "Exec dbo.sprConsultaPosVoucher '" + fecha + "' , '" + Program.ID_Caja_POS + "' , '" + _CID + "' , 2";
                    var mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery).ToList();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "ejecuta script sprConsultaPosVoucher, para MEDIANET: " + sqlQuery);


                    string grupoAux = "";
                    texto = texto.Replace("<<lote>>", _numLoteMedianet);
                    texto = texto.Replace("<<tid>>", _TIDMedianet);
                    texto = texto.Replace("<<cid>>", _CID);

                    foreach (var linea in mov)//llega a esta linea y se sale del foreach
                    {
                        if (grupoAux != linea.GRUPOTAR)
                        {
                            if (cont > 0)
                            {
                                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                                valorgrupo = 0;
                            }
                            grupoAux = linea.GRUPOTAR;
                            pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                            cont += 1;

                            //sumatoria
                        }
                        decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                        pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));
                        if (pie && valorgrupo != 0)
                        {

                        }
                        valorgrupo += valorvoucher;
                        valortotalmed += valorvoucher;
                        linea.PROCESADOTURNO = true;

                        var procesaMedianet = db.POS_VOUCHER.Where(x => x.id == linea.id).FirstOrDefault();
                        procesaMedianet.PROCESADOTURNO = true;
                    }

                    pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                    pagos.AppendLine("\nTOTAL MEDIANET: " + valortotalmed);
                    pagos.AppendLine("=======================================\n\n");
                    pagos.AppendLine("<footer>DATAFAST</footer>");
                    pagos.AppendLine("");
                    pagos.AppendLine("LOTE    : " + _numLoteDatafast);
                    pagos.AppendLine("TERMINAL: " + _TIDDatafast);
                    pagos.AppendLine("=======================================\n\n");

                    db.SaveChanges();

                    sqlQuery = "Exec dbo.sprConsultaPosVoucher '" + fecha + "' , '" + Program.ID_Caja_POS + "' , '" + _CID + "' , 1";
                    mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery).ToList();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "ejecuta script sprConsultaPosVoucher, para DataFast: " + sqlQuery);

                    cont = 0;
                    grupoAux = "";
                    valorgrupo = 0;
                    valortotalmed = 0;
                    foreach (var linea in mov)//llega a esta linea y se sale del foreach
                    {
                        if (grupoAux != linea.GRUPOTAR)
                        {
                            if (cont > 0)
                            {
                                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                                valorgrupo = 0;
                            }
                            grupoAux = linea.GRUPOTAR;
                            pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                            cont += 1;

                            //sumatoria
                        }
                        decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                        pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));
                        if (pie && valorgrupo != 0)
                        {

                        }
                        valorgrupo += valorvoucher;
                        valortotalmed += valorvoucher;
                        linea.PROCESADOTURNO = true;

                        var procesaDatafast = db.POS_VOUCHER.Where(x => x.id == linea.id).FirstOrDefault();
                        procesaDatafast.PROCESADOTURNO = true;
                    }

                    pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                    pagos.AppendLine("\nTOTAL DATAFAST: " + valortotalmed);
                    texto = regex.Replace(texto, pagos.ToString());


                    sqlQuery = "Exec dbo.sprConsultaPosVoucher '" + fecha + "' , '" + Program.ID_Caja_POS + "' , '" + _CID + "' , 3";
                    mov = db.Database.SqlQuery<POS_VOUCHER>(sqlQuery).ToList();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "ejecuta script sprConsultaPosVoucher, para AUSTRO: " + sqlQuery);


                    cont = 0;
                    grupoAux = "";
                    valorgrupo = 0;
                    valortotalmed = 0;
                    foreach (var linea in mov)//llega a esta linea y se sale del foreach
                    {
                        if (grupoAux != linea.GRUPOTAR)
                        {
                            if (cont > 0)
                            {
                                pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                                valorgrupo = 0;
                            }
                            grupoAux = linea.GRUPOTAR;
                            pagos.AppendLine("<b>" + linea.GRUPOTAR + "</b>");
                            cont += 1;

                            //sumatoria
                        }
                        decimal valorvoucher = linea.ANULADO == true ? 0 : decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2));

                        pagos.AppendLine(linea.AUTORIZACION + "     " + linea.TIPOCONSUMO + "      " + decimal.Parse(decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2)).ToString() + (linea.ANULADO == true ? "   ANULADO" : ""));
                        if (pie && valorgrupo != 0)
                        {

                        }
                        valorgrupo += valorvoucher;
                        valortotalmed += valorvoucher;
                        linea.PROCESADOTURNO = true;

                        var procesaAustro = db.POS_VOUCHER.Where(x => x.id == linea.id).FirstOrDefault();
                        procesaAustro.PROCESADOTURNO = true;
                    }

                    pagos.AppendLine("Total " + grupoAux + " : " + valorgrupo);
                    pagos.AppendLine("\nTOTAL AUSTRO: " + valortotalmed);
                    texto = regex.Replace(texto, pagos.ToString());

                    // FIN: Lote del autorizador AUSTRO.  JM  22-12-2020

                    #endregion


                    var dbProceso = db.core_puntoemision.Where(x => x.establecimiento_id == this.Establecimiento && x.punto_emision == this.PtoEmisionOrigen).FirstOrDefault();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Actualiza numero lote dbProceso.lote_datafast: " + dbProceso.lote_datafast);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Actualiza numero lote dbProceso.lote_medianet: " + dbProceso.lote_medianet);

                    dbProceso.lote_datafast += 1;
                    dbProceso.lote_medianet += 1;
                    db.SaveChanges();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Actualiza numero lote dbProceso.lote_datafast: " + dbProceso.lote_datafast);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Actualiza numero lote dbProceso.lote_medianet: " + dbProceso.lote_medianet);

                }


                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "preparaTextoCorteLoteV2", "Texto Tira de Pagos  " + texto);
                return texto;
            }
            catch (Exception ex)
            {
                texto = string.Empty;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Fatura", "preparaTextoCorteLoteV2", "Error: " + ex.Message);
                return texto;
            }



        }
        public void prepararCorteLoteNoDefinitivo()
        {
            var db = new POSEntities();
            if (db.core_recibo.Any(x => x.identificador == "CORTE_LOTE"))
            {
                this.ReciboCorteLote = preparaTextoCorteLoteNoDefinitivo(db.core_recibo.First(x => x.identificador == "CORTE_LOTE").cuerpo);
            }
        }

        public void prepararVoucher(string tipo_voucher, string tipo_pago)
        {
            var db = new POSEntities();

            if (db.core_recibo.Any(x => x.identificador == tipo_voucher))
            {
                var voucher = db.core_recibo.First(x => x.identificador == tipo_voucher);
                if (!this.Voucher.Any(x => x.Tipo == voucher.identificador))
                {
                    this.Voucher.Add(new Voucher() { Tipo = voucher.identificador, Pago = tipo_pago, Texto = preparaTextoVoucher(voucher.cuerpo, tipo_pago) });
                }
            }
        }

        public void prepararImpresionCupones2(decimal valor, string division)
        {


            try
            {
                using (var db = new POSEntities())
                {
                    //(x => x.fecha.Year == DateTime.Now.Year && x.fecha.Month == DateTime.Now.Month && x.fecha.Day == DateTime.Now.Day && x.nivel != 99))
                    decimal itemsFactura = 0;
                    decimal itemsParticipante = 0;
                    int[] _promoCupon = new int[db.core_promocionticket.Count()];
                    int j = 0;
                    bool PermitirPromocionTicketSinItem = false;
                    decimal PromoValorFactItemValor = 0;
                    int PromoValorFactItemIdTick = 0;
                    int PromoValorFactItemCF = 0;
                    decimal _valor;



                    var objPromocionTicketSinItem = (from deta in db.core_parametro
                                                     where deta.identificador == "PROMO_FAFECHA_INICIO_PROMO_TICKET_SIN_ITEMCT_ITEM"
                                                     && deta.valor == "TRUE"
                                                     && deta.documento.Contains(this.Establecimiento + ";")
                                                     //&& DateTime.Now >= DateTime.Parse(deta.parametro2)
                                                     select deta).ToList().FirstOrDefault();

                    if (objPromocionTicketSinItem != null)
                    {
                        if (DateTime.Now >= DateTime.Parse(objPromocionTicketSinItem.parametro2))
                        {
                            PermitirPromocionTicketSinItem = true;
                        }

                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", "consulta de core_promocionticket ");

                    var _listapromo = (from ticket in db.core_promocionticket
                                       where ticket.fecha_hasta >= DateTime.Now && ticket.fecha_desde <= DateTime.Now
                                       select ticket).ToList();


                    if (_listapromo.ToList().Count == 0 || _listapromo == null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", "No se han econtrado promociones activas");
                        return;
                    }

                    foreach (var _promo in _listapromo)
                    {

                        PromoValorFactItemValor = 0;
                        PromoValorFactItemIdTick = 0;
                        PromoValorFactItemCF = 0;


                        if (_promo.establecimiento == this._establecimiento || _promo.establecimiento == null)
                        {

                            var objPromoFactItem = (from deta in db.core_parametro
                                                    where deta.identificador == "PROMO_FACT_ITEM"
                                                    && deta.valor == "TRUE"
                                                    && deta.parametro2.Contains(_promo.id.ToString())
                                                    select deta).ToList().FirstOrDefault();

                            if (objPromoFactItem != null)
                            {
                                var objPromo = objPromoFactItem.parametro2.Split('|');

                                PromoValorFactItemIdTick = Convert.ToInt32(objPromo[0]);
                                PromoValorFactItemValor = Convert.ToDecimal(objPromo[1]);
                                PromoValorFactItemCF = Convert.ToInt16(objPromo[2]);
                            }

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"PromoValorFactItemIdTick: {PromoValorFactItemIdTick}");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"PromoValorFactItemValor: {PromoValorFactItemValor}");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"PromoValorFactItemCF: {PromoValorFactItemCF}");

                            if (db.core_promocionticket_items.Any(x => x.promocionticket_id == _promo.id) || PermitirPromocionTicketSinItem == true)
                            {
                                _valor = 0M;
                                foreach (var _item_promo in db.core_promocionticket_items.Where(x => x.promocionticket_id == _promo.id))
                                {
                                    foreach (var p in this.Productos)
                                    {
                                        if (p.Id.Equals(_item_promo.itemid))
                                        {
                                            _valor += _promo.ismoneda.Value ? p.Total : p.Cantidad;
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"_valor: {_valor}");
                                        }
                                    }
                                }

                                _promoCupon[j] = _promo.id;

                                j++;

                                if (PromoValorFactItemValor > 0 && PromoValorFactItemIdTick == _promo.id)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"PromoValorFactItemValor > 0 && PromoValorFactItemIdTick == _promo.id");

                                    itemsFactura = _valor;
                                    itemsParticipante = valor / PromoValorFactItemValor;

                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"itemsParticipante: {itemsParticipante}");

                                    _valor = ((itemsParticipante > itemsFactura) ? itemsFactura : itemsParticipante) * itemsFactura;

                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"((itemsParticipante > itemsFactura) ? itemsFactura : itemsParticipante) * itemsFactura");
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones2", $"_valor: {_valor}");

                                    _promo.valor = itemsFactura;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Ingresa en opcion de cupon por valor factura e items participantes.");



                                    if (PromoValorFactItemCF == 0 && this.ClienteIdentificacion == "9999999999999")
                                    {
                                        _valor = 0;
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Es consumidor final no va a imprimir cupones.");
                                    }
                                }
                                else
                                {
                                    //valida que la promoción de ticket no se de a consumidor final segun la configuración   
                                    if (PromoValorFactItemValor == 0 && PromoValorFactItemIdTick == _promo.id && +
                                            PromoValorFactItemCF == 0 && this.ClienteIdentificacion == "9999999999999")
                                    {
                                        _valor = 0;
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Es consumidor final no va a imprimir cupones.");
                                    }
                                }



                                if (_valor >= _promo.valor && _valor > 0)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"_valor:{_valor};  _promo.valor : {_promo.valor}");
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Math.Truncate(_valor / _promo.valor): {Math.Truncate(_valor / _promo.valor)}");
                                    string iContCupones = Math.Truncate(_valor / _promo.valor).ToString();

                                    for (var i = 1; i <= Math.Truncate(_valor / _promo.valor); i++)
                                    {   //CAMBIO PARA PROMOCIONES POR PRODUCTOS/UN SOLO CUPON

                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Ingresa en opcion de cupon por valor factura e items participantes.");
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"_promo.impresion_unitario: {_promo.impresion_unitario}");

                                        if (_promo.impresion_unitario == true && i == 1)
                                        {
                                            this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(_promo), Valor = _promo.valor, Unico = true, Giftcard = (_promo.isgiftcard != null) ? (bool)_promo.isgiftcard : false, Valorgiftcard = (_promo.valor_giftcard != null) ? (decimal)_promo.valor_giftcard : 0 });
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Se agrega info de cupon correctamente. solo un cupon.");
                                            break;
                                        }
                                        else
                                        {
                                            this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(_promo), Valor = _promo.valor, Unico = true, Giftcard = (_promo.isgiftcard != null) ? (bool)_promo.isgiftcard : false, Valorgiftcard = (_promo.valor_giftcard != null) ? (decimal)_promo.valor_giftcard : 0 });
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Se agrega info de cupon correctamente. N cupones.");
                                        }

                                    }
                                }


                                if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == null && x.id == _promo.id))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"PromoTicket Establecimiento == null  && Id == _promo.id  ");

                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == null && x.id == _promo.id);
                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                    {
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }

                                if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"PromoTicket Establecimiento == this.Establecimiento  && Id == _promo.id  ");

                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id);


                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Agrega cupon: ");
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }

                                if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == division && x.id == _promo.id))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"PromoTicket Establecimiento == null  && Id == _promo.id  && division == division ");

                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == division && x.id == _promo.id);
                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Agrega cupon: ");
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }
                                if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"PromoTicket Establecimiento == this.Establecimiento  && Id == _promo.id  && division == division ");

                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id);
                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Agrega cupon: ");
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }


                                if (PermitirPromocionTicketSinItem == true && _promo.ismoneda == true && !(db.core_promocionticket_items.Any(x => x.promocionticket_id == _promo.id))) //y q no tenga item configurados
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"PermitirPromocionTicketSinItem {PermitirPromocionTicketSinItem} ; _promo.ismoneda && no encuentre por el codigo del articulo ");

                                    if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == null && x.id == _promo.id))
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"establecimiento == null; division == null   ");


                                        var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == null && x.id == _promo.id);
                                        if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                        {
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Agrega cupon: ");
                                            this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                        }
                                    }
                                    if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id))
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"establecimiento == this.Establecimiento; division == null   ");

                                        var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id);
                                        if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                        {
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Agrega cupon: ");
                                            this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                        }
                                    }
                                    if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == division && x.id == _promo.id))
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"establecimiento == this.Establecimiento; division == division    ");

                                        var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == division && x.id == _promo.id);
                                        if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                        {
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Agrega cupon: ");
                                            this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                        }
                                    }
                                    if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id))
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"establecimiento == this.Establecimiento; division == division    ");

                                        var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id);
                                        if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                        {
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", $"Agrega cupon: ");
                                            this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {


            }


        }


        public void prepararImpresionCupones(decimal valor, string division)
        {
            //ML: Por disposicion Juan Jose Hidalgo los empleados ya no seran excluidos de promo tickets
            //if (division != "EM")
            //{
            //}
            using (var db = new POSEntities())
            {
                //(x => x.fecha.Year == DateTime.Now.Year && x.fecha.Month == DateTime.Now.Month && x.fecha.Day == DateTime.Now.Day && x.nivel != 99))
                decimal itemsFactura = 0;
                decimal itemsParticipante = 0;
                int[] _promoCupon = new int[db.core_promocionticket.Count()];
                int j = 0;
                //int ticketUnitario = 0;
                var _listapromo = from ticket in db.core_promocionticket
                                  where ticket.fecha_hasta >= DateTime.Now && ticket.fecha_desde <= DateTime.Now
                                  select ticket;

                decimal _valor;

                foreach (var _promo in _listapromo)
                {

                    if (_promo.establecimiento == this._establecimiento || _promo.establecimiento == null)
                    {
                        if (db.core_promocionticket_items.Any(x => x.promocionticket_id == _promo.id) || POS.Control.Common.GlobalParameters.PermitirPromocionTicketSinItem == true)
                        {
                            _valor = 0M;
                            foreach (var _item_promo in db.core_promocionticket_items.Where(x => x.promocionticket_id == _promo.id))
                            {
                                foreach (var p in this.Productos)
                                {
                                    if (p.Id.Equals(_item_promo.itemid))
                                    {
                                        _valor += _promo.ismoneda.Value ? p.Total : p.Cantidad;
                                    }
                                }
                            }
                            _promoCupon[j] = _promo.id;
                            j++;

                            if (POS.Control.Common.GlobalParameters.PromoValorFactItemValor > 0 && POS.Control.Common.GlobalParameters.PromoValorFactItemIdTick == _promo.id)
                            {
                                itemsFactura = _valor;
                                itemsParticipante = valor / POS.Control.Common.GlobalParameters.PromoValorFactItemValor;
                                _valor = ((itemsParticipante > itemsFactura) ? itemsFactura : itemsParticipante) * itemsFactura;
                                _promo.valor = itemsFactura;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Ingresa en opcion de cupon por valor factura e items participantes.");
                                if (POS.Control.Common.GlobalParameters.PromoValorFactItemCF == 0 && this.ClienteIdentificacion == "9999999999999")
                                {
                                    _valor = 0;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Es consumidor final no va a imprimir cupones.");
                                }
                            }
                            else
                            { //valida que la promoción de ticket no se de a consumidor final segun la configuración   
                                if (POS.Control.Common.GlobalParameters.PromoValorFactItemValor == 0 && POS.Control.Common.GlobalParameters.PromoValorFactItemIdTick == _promo.id && +
                                        POS.Control.Common.GlobalParameters.PromoValorFactItemCF == 0 && this.ClienteIdentificacion == "9999999999999")
                                {
                                    _valor = 0;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Es consumidor final no va a imprimir cupones.");
                                }
                            }

                            if (_valor >= _promo.valor && _valor > 0)
                            {
                                for (var i = 1; i <= Math.Truncate(_valor / _promo.valor); i++)
                                {   //CAMBIO PARA PROMOCIONES POR PRODUCTOS/UN SOLO CUPON
                                    if (_promo.impresion_unitario == true && i == 1)
                                    {
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(_promo), Valor = _promo.valor, Unico = true, Giftcard = (_promo.isgiftcard != null) ? (bool)_promo.isgiftcard : false, Valorgiftcard = (_promo.valor_giftcard != null) ? (decimal)_promo.valor_giftcard : 0 });
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Se agrega info de cupon correctamente. solo un cupon.");
                                        break;
                                    }
                                    else
                                    {
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(_promo), Valor = _promo.valor, Unico = true, Giftcard = (_promo.isgiftcard != null) ? (bool)_promo.isgiftcard : false, Valorgiftcard = (_promo.valor_giftcard != null) ? (decimal)_promo.valor_giftcard : 0 });
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prepararImpresionCupones", "Se agrega info de cupon correctamente. N cupones.");
                                    }

                                }

                            }
                            if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == null && x.id == _promo.id))
                            {
                                var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == null && x.id == _promo.id);
                                if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                {
                                    this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                }
                            }
                            if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id))
                            {
                                var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id);
                                if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                {
                                    this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                }
                            }
                            if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == division && x.id == _promo.id))
                            {
                                var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == division && x.id == _promo.id);
                                if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                {
                                    this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                }
                            }
                            if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id))
                            {
                                var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id);
                                if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && VerificaCuponPromo(_promoCupon, recibo1.id) && recibo1.id == _promo.id)
                                {
                                    this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                }
                            }


                            if (POS.Control.Common.GlobalParameters.PermitirPromocionTicketSinItem == true && _promo.ismoneda == true && !(db.core_promocionticket_items.Any(x => x.promocionticket_id == _promo.id))) //y q no tenga item configurados
                            {
                                if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == null && x.id == _promo.id))
                                {
                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == null && x.id == _promo.id);
                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                    {
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }
                                if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id))
                                {
                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == null && x.id == _promo.id);
                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                    {
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }
                                if (db.core_promocionticket.Any(x => x.establecimiento == null && x.division == division && x.id == _promo.id))
                                {
                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == null && x.division == division && x.id == _promo.id);
                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                    {
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }
                                if (db.core_promocionticket.Any(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id))
                                {
                                    var recibo1 = db.core_promocionticket.First(x => x.establecimiento == this.Establecimiento && x.division == division && x.id == _promo.id);
                                    if (recibo1 != null && recibo1.valor <= valor && recibo1.fecha_hasta >= DateTime.Now && recibo1.fecha_desde <= DateTime.Now && recibo1.id == _promo.id)
                                    {
                                        this.Cupon.Add(new Cupones() { Texto = preparaTextoCupon(recibo1), Valor = recibo1.valor, Unico = recibo1.impresion_unitario, Giftcard = (recibo1.isgiftcard != null) ? (bool)recibo1.isgiftcard : false, Valorgiftcard = (recibo1.valor_giftcard != null) ? (decimal)recibo1.valor_giftcard : 0 });
                                    }
                                }
                            }
                        }

                    }



                }
            }
        }

        public void prepararImpresionCupones3(string establecimiento, string punto_emision, long numFactura, int promoId = 0)
        {
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            Cupones detCupones = new Cupones();

            try
            {
                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, "exec spGeneraCupones ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" @establecimiento = '{establecimiento}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @punto_emision = '{punto_emision}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @numeroFactura = {numFactura.ToString()} ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @promoId = {promoId.ToString()} ", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(sQuery);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaPromoTicketNumBin ", "EJECUTA SP DE CUPON PROMOCIONES ");

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCuponesDependientes ", " Se han encontrado registros: " + dtsConsulta.Tables[0].Rows.Count.ToString());

                        int indexTable = dtsConsulta.Tables.Count;
                        bool tieneCupones = false;
                        int cantidadCupones = 0;

                        for (int iTable = 0; iTable <= dtsConsulta.Tables.Count - 1; iTable++)
                        {
                            string tipoConsulta = dtsConsulta.Tables[iTable].Rows[0]["tipoConsulta"].ToString();


                            if (tipoConsulta == "CONS_CAB")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[iTable].Rows)
                                {
                                    if (data["cantidadCupones"].ToString() != "0")
                                    {
                                        tieneCupones = true;
                                        cantidadCupones = Int32.Parse(data["cantidadCupones"].ToString());
                                    }
                                }
                                continue;

                            }

                            if (tieneCupones)
                            {
                                if (tipoConsulta == "CONS_DET")
                                {
                                    foreach (DataRow data in dtsConsulta.Tables[iTable].Rows)
                                    {
                                        detCupones = new Cupones();
                                        detCupones.Texto = data["Texto_Ticket"].ToString();
                                        detCupones.Valor = decimal.Parse(data["valor"].ToString());

                                        detCupones.Valorgiftcard = 0;
                                        if (!string.IsNullOrEmpty(data["Valorgiftcard"].ToString()))
                                        {
                                            detCupones.Valorgiftcard = decimal.Parse(data["Valorgiftcard"].ToString());
                                        }

                                        bool Unico = false;
                                        bool Giftcard = false;

                                        if (data["impresion_unitario"].ToString() == "1" || (bool)data["impresion_unitario"] == true)
                                        {
                                            Unico = true;
                                        }

                                        if (data["isgiftcard"].ToString() != "")
                                        {
                                            if (data["isgiftcard"].ToString() == "1" || (bool)data["isgiftcard"] == true)
                                            {
                                                Giftcard = true;
                                            }
                                        }


                                        detCupones.Unico = Unico;
                                        detCupones.Giftcard = Giftcard;
                                        this.Cupon.Add(detCupones);

                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCuponesDependientes ", " Se agrega a la lista CuponBines");

                                    }
                                    continue;
                                }
                            }



                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCuponesDependientes ", "Exception: " + ex.Message);

            }

        }


        public void prepararImpresionCupones4(Factura factura, long promoId, string bin = "")
        {
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            Cupones detCupones = new Cupones();
            UDT_DetFacturaCupon uDT_DetFactura = new UDT_DetFacturaCupon();
            string connectionMark = POS.Properties.Settings.Default.CONECTA_MKT;
            try
            {
                

                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, "exec GenerarTicketCupones_Cursor  ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"   @establecimiento = '{factura.Establecimiento}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"   , @punto_emision = '{factura.PtoEmision}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"   , @numero_factura = {factura.Secuencia} ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"   , @bin = '{bin}' ", Environment.NewLine); // ✅ NUEVO
                sQuery = string.Concat(sQuery, $"   , @es_clienteapp = {(factura.EsClienteApp ? 1 : 0)} ", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(sQuery, connectionMark);

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones4 ", " Se han encontrado registros: " + dtsConsulta.Tables[0].Rows.Count.ToString());

                        int indexTable = dtsConsulta.Tables.Count;
                        bool tieneCupones = false;
                        int cantidadCupones = 0;

                        for (int iTable = 0; iTable <= dtsConsulta.Tables.Count - 1; iTable++)
                        {
                            string tipoConsulta = dtsConsulta.Tables[iTable].Rows[0]["tipoConsulta"].ToString();

                            if (tipoConsulta == "CONS_CAB")
                            {
                                foreach (DataRow dataCab in dtsConsulta.Tables[iTable].Rows)
                                {
                                    if (dataCab["cantidadCupones"].ToString() != "0")
                                    {
                                        tieneCupones = true;
                                        cantidadCupones = Int32.Parse(dataCab["cantidadCupones"].ToString());
                                    }
                                }
                                continue;
                            }

                            if (tieneCupones)
                            {
                                if (tipoConsulta == "CONS_DET")
                                {
                                    foreach (DataRow data in dtsConsulta.Tables[iTable].Rows)
                                    {
                                        detCupones = new Cupones();
                                        detCupones.Texto = data["Texto_Ticket"].ToString();
                                        detCupones.Valor = decimal.Parse(data["valor"].ToString());

                                        detCupones.Valorgiftcard = 0;
                                        if (!string.IsNullOrEmpty(data["Valorgiftcard"].ToString()))
                                        {
                                            detCupones.Valorgiftcard = decimal.Parse(data["Valorgiftcard"].ToString());
                                        }

                                        bool Unico = false;
                                        bool Giftcard = false;

                                        if (data["impresion_unitario"].ToString() == "1" || (bool)data["impresion_unitario"] == true)
                                        {
                                            Unico = true;
                                        }

                                        if (data["isgiftcard"].ToString() != "")
                                        {
                                            try
                                            {
                                                if (data["isgiftcard"].ToString() == "1" || (bool)data["isgiftcard"] == true)
                                                {
                                                    Giftcard = true;
                                                }

                                            }
                                            catch (Exception)
                                            {

                                                Giftcard = false;
                                            }
                                        }

                                        detCupones.Unico = true; // le puse true para probar la impresion de los cupones JCHID
                                        detCupones.Giftcard = Giftcard;

                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones4 ", " Se agrega regsitro Cupon");
                                        this.Cupon.Add(detCupones);
                                    }
                                    continue;
                                }
                            }

                        }

                      

                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones4 ", $" Error: {ex.Message}");

            }


        }

        public void prepararImpresionCuponesUDT(Factura factura, long promoId)
        {
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            Cupones detCupones = new Cupones();
            UDT_DetFacturaCupon uDT_DetFactura = new UDT_DetFacturaCupon();
            SqlConnection con = new SqlConnection();
            DataTable tblDetFactura = new DataTable();

            try
            { 
                string connectionMark = "Data Source=SRV-QA-AX;Initial Catalog=Marketing;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=60;Application Name=EntityFramework";

                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, "BEGIN TRY  ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "DECLARE @UDT_DetFacturaCupon UDT_DetFacturaCupon", Environment.NewLine);
                
                sQuery = string.Concat(sQuery, "exec GenerarTicketCupones_Cursor  ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "   @DetalleFacturaCupon = @UDT_DetFacturaCupon  ", Environment.NewLine);

                sQuery = string.Concat(sQuery, "END TRY   ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "BEGIN CATCH   ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "    Select CodigoRespuesta = ERROR_NUMBER()", Environment.NewLine);
                sQuery = string.Concat(sQuery, "    , MensajeRespuesta = convert(varchar(300), '(POS_VOUCHER) ERROR: ' + ERROR_MESSAGE())", Environment.NewLine);
                sQuery = string.Concat(sQuery, "END CATCH  ", Environment.NewLine);
                tblDetFactura = uDT_DetFactura.GetDataTable();

                StringBuilder logContent = new StringBuilder();
                logContent.AppendLine("Inserto datos UDT_DetFacturaCupon ");
                using (var pos = new POSEntities())
                {

                    try
                    {
                        //string CadenaConexion = POS.Properties.Settings.Default.CONECTA_MKT;
                        using (con = new SqlConnection(connectionMark))
                        {
                            con.Open();

                            using (SqlCommand cmd = new SqlCommand(sQuery, con))
                            {

                                int linea = 1;
                                foreach (var detArticulo in factura.Productos)
                                {
                                    DataRow detalle = tblDetFactura.NewRow();

                                    detalle["establecimiento"] = factura.Establecimiento;
                                    detalle["punto_emision"] = factura.PtoEmision;
                                    detalle["numFactura"] = Int32.Parse(factura.Secuencia.ToString());

                                    detalle["identificacionClte"] = factura.ClienteIdentificacion;
                                    detalle["nombre_cliente"] = factura.Cliente_nombre;
                                    detalle["telefono_cliente"] = factura.Cliente_telefono;
                                    detalle["direccion_cliente"] = factura.Cliente_direccion;
                                    detalle["linea"] = Int32.Parse(linea.ToString());
                                    detalle["item_id"] = detArticulo.Id;
                                    detalle["item_nombre"] = detArticulo.Nombre;
                                    detalle["cantidad"] = decimal.Parse(detArticulo.Cantidad.ToString());
                                    detalle["total"] = decimal.Parse(detArticulo.Total.ToString());

                                    tblDetFactura.Rows.Add(detalle);
                                    linea++;
                                }


                                var objNotacredito = new SqlParameter("@DetalleFacturaCupon", SqlDbType.Structured);
                                objNotacredito.TypeName = "dbo.UDT_DetFacturaCupon";
                                objNotacredito.Value = tblDetFactura;
                                cmd.Parameters.Add(objNotacredito);


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

                                }
                            }

                            con.Close();
                            con.Dispose();
                            return;
                        }



                        if (dtsConsulta.Tables.Count > 0)
                        {
                            if (dtsConsulta.Tables[0].Rows.Count > 0)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones4 ", " Se han encontrado registros: " + dtsConsulta.Tables[0].Rows.Count.ToString());

                                int indexTable = dtsConsulta.Tables.Count;
                                bool tieneCupones = false;
                                int cantidadCupones = 0;

                                for (int iTable = 0; iTable <= dtsConsulta.Tables.Count - 1; iTable++)
                                {
                                    string tipoConsulta = dtsConsulta.Tables[iTable].Rows[0]["tipoConsulta"].ToString();


                                    if (tipoConsulta == "CONS_CAB")
                                    {
                                        foreach (DataRow data in dtsConsulta.Tables[iTable].Rows)
                                        {
                                            if (data["cantidadCupones"].ToString() != "0")
                                            {
                                                tieneCupones = true;
                                                cantidadCupones = Int32.Parse(data["cantidadCupones"].ToString());
                                            }
                                        }
                                        continue;

                                    }

                                    if (tieneCupones)
                                    {
                                        if (tipoConsulta == "CONS_DET")
                                        {
                                            foreach (DataRow data in dtsConsulta.Tables[iTable].Rows)
                                            {
                                                detCupones = new Cupones();
                                                detCupones.Texto = data["Texto_Ticket"].ToString();
                                                detCupones.Valor = decimal.Parse(data["valor"].ToString());

                                                detCupones.Valorgiftcard = 0;
                                                if (!string.IsNullOrEmpty(data["Valorgiftcard"].ToString()))
                                                {
                                                    detCupones.Valorgiftcard = decimal.Parse(data["Valorgiftcard"].ToString());
                                                }

                                                bool Unico = false;
                                                bool Giftcard = false;

                                                if (data["impresion_unitario"].ToString() == "1" || (bool)data["impresion_unitario"] == true)
                                                {
                                                    Unico = true;
                                                }

                                                if (data["isgiftcard"].ToString() != "")
                                                {
                                                    if (data["isgiftcard"].ToString() == "1" || (bool)data["isgiftcard"] == true)
                                                    {
                                                        Giftcard = true;
                                                    }
                                                }


                                                detCupones.Unico = Unico;
                                                detCupones.Giftcard = Giftcard;


                                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCupones4 ", " Se agrega regsitro Cupon");
                                                this.Cupon.Add(detCupones);
                                            }
                                            continue;
                                        }
                                    }

                                }

                            }
                        }


                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    
                }

             
              

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prepararImpresionCuponesDependientes ", "Exception: " + ex.Message);

            }

        }



        public void RecuperaPromoTicketNumBin(string NumBin, string identificacion, decimal valor, string establecimiento, int promocionticket_id, string NumeroFactura)
        {
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            Cupones detCupones = new Cupones();

            try
            {
                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, "exec spGetCuponBines ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" @promocionticket_id = {promocionticket_id} ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @NumBin = '{NumBin}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @identificacion = '{identificacion}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @valor = {valor.ToString()}", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @numeroFactura = '{NumeroFactura}'", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(sQuery);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaPromoTicketNumBin ", "EJECUTA SP DE CUPON PROMOCIONES ");

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaPromoTicketNumBin ", " Se han encontrado registros: " + dtsConsulta.Tables[0].Rows.Count.ToString());


                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            detCupones = new Cupones();
                            detCupones.Texto = data["Texto_Ticket"].ToString();
                            detCupones.Valor = decimal.Parse(data["valor"].ToString());
                            detCupones.Valorgiftcard = decimal.Parse(data["Valorgiftcard"].ToString());

                            bool Unico = false;
                            bool Giftcard = false;

                            if (data["impresion_unitario"].ToString() == "1" || (bool)data["impresion_unitario"] == true)
                            {
                                Unico = true;
                            }

                            if (data["isgiftcard"].ToString() == "1" || (bool)data["isgiftcard"] == true)
                            {
                                Giftcard = true;
                            }

                            detCupones.Unico = Unico;
                            detCupones.Giftcard = Giftcard;
                            this.CuponBines.Add(detCupones);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaPromoTicketNumBin ", " Se agrega a la lista CuponBines");

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaPromoTicketNumBin ", "Exception: " + ex.Message);

            }

        }


        private bool VerificaCuponPromo(int[] arreglo, int cod)
        {
            for (var i = 0; i < arreglo.Count(); i++)
            {
                if (arreglo[i] == cod)
                    return false;
            }
            return true;
        }

        private void creaArhivoTemporalConsumoBilletera(string consumo, string NumeroFactura)
        {
            try
            {
                //se adiciona alguna información y la fecha
                DateTime dateTime = new DateTime();
                dateTime = DateTime.Now;
                string strDate = Convert.ToDateTime(dateTime).ToString("yyyyMMddHHmmss");
                string rutaCompleta = Control.Common.GlobalParameters.ConsumoBilleteraInsertPath + NumeroFactura + "_" + strDate + ".txt";
                if (!string.IsNullOrEmpty(Control.Common.GlobalParameters.ConsumoBilleteraInsertPath))
                {
                    CreateEmptyDirectory(Control.Common.GlobalParameters.ConsumoBilleteraInsertPath);
                    using (StreamWriter mylogs = File.AppendText(rutaCompleta))         //se crea el archivo
                    {
                        mylogs.WriteLine(consumo);

                        mylogs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //Insertar en el log y luego enviar correo.
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "creaArhivoTemporalConsumoBilletera", "Ha ocurrido una excepción al momento de generar archivo POS CONSUMOBILLETERA - A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                //Enviar Correo:
                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                           Properties.Settings.Default.MAILERROR_FROM,
                           Properties.Settings.Default.MAILERROR_ALIAS,
                           Properties.Settings.Default.MAILERROR_DESTINO,
                           Properties.Settings.Default.MAILERROR_CC,
                           "Consumo Billetera no se generó archivo temporal para el ingreso",
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
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "creaArhivoTemporalConsumoBilletera", "No se pudo enviar notificacion del problema al grabar Consumo Billetera en archivo temporal, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
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

        string _establecimiento_nombre;

        public string Establecimiento_nombre
        {
            get { return _establecimiento_nombre; }
            set { _establecimiento_nombre = value; }
        }

        string _establecimiento_direccion;

        public string Establecimiento_direccion
        {
            get { return _establecimiento_direccion; }
            set { _establecimiento_direccion = value; }
        }

        string _establecimiento_telefono;

        public string Establecimiento_telefono
        {
            get { return _establecimiento_telefono; }
            set { _establecimiento_telefono = value; }
        }

        //public string ModeloBalanza { get; internal set; }
        string _ModeloBalanza;

        public string ModeloBalanza
        {
            get { return _ModeloBalanza; }
            set { _ModeloBalanza = value; }
        }
        public string IpPinPad { get; internal set; }
        public bool EstTcpIpPinpad { get; internal set; }
        public int PuertoPinPad { get; internal set; }

        public Factura Clone()
        {
            var cloned = (Factura)this.MemberwiseClone();

            cloned.Productos = new BindingList<Producto>();
            this.Productos.ToList().ForEach(a => cloned.Productos.Add(a));

            return cloned;
        }

        public int CalcularParqueo(ref bool existeItemParqueo, bool tieneComprasAdicionales)
        {
            int totalFracciones = 0;
            try
            {
                if (ObjParking != null)
                {
                    //var item = this.Productos.Where(x => x.Id == ObjParking.ItemIdParqueo || x.Id == ObjParking.ItemIdParqueoSinCompra).FirstOrDefault();

                    //existeItemParqueo = item == null;

                    //decimal totalFac = this.Total - (existeItemParqueo ? 0M : item.Total);

                    decimal totalFac = this.Total;

                    DateTime esteMomento = DateTime.Now;
                    TimeSpan diferenciaMinutos = esteMomento - ObjParking.FechaIngreso.AddMinutes(totalFac >= ObjParking.ValorMinCompraParaMinutosLibre ? ObjParking.MinutosLibreMaxPorCompra : ObjParking.MinutosGracia);

                    totalFracciones = (diferenciaMinutos.TotalMinutes > 0 ? (int)diferenciaMinutos.TotalMinutes : 0) / (tieneComprasAdicionales ? ObjParking.MinutosFraccion : ObjParking.MinutosFraccionSinCompra);
                }
            }
            catch (Exception ex)
            {
                totalFracciones = 0;
            }

            return totalFracciones;
        }
        /// <summary>
        /// Realiza el Caje de los Puntos Acumulados en compras por Monedero electrónico para pagos de Factura.
        /// </summary>
        /// <param name="monto_consumo"></param>
        /// <param name="factura"></param>
        /// <param name="accountnum"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public bool PagoxCanjePuntosMonedero(decimal monto_consumo, string factura, string accountnum, string usuario)
        {

            XmlDocument xmlDoc = new XmlDocument();

            //< root > < req factura = "F-024-007-000175474" accountNum = "0930550827" usuario = "evelasco" valor = "1" /> </ root >

            //string strxml = string.Empty;
            //strxml = "<root> <req factura ='" + factura + "' accountNum = '" + accountnum + "' usuario = '" + usuario + "' valor = '" + monto_consumo + "'/> </root>";

            XmlNode rootNode = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(rootNode);

            XmlNode userNode = xmlDoc.CreateElement("req");
            XmlAttribute attribute = xmlDoc.CreateAttribute("factura");
            attribute.Value = factura;
            userNode.Attributes.Append(attribute);

            XmlAttribute attribute1 = xmlDoc.CreateAttribute("usuario");
            attribute1.Value = POS.Control.Common.GlobalParameters.UserObj.username;
            userNode.Attributes.Append(attribute1);

            XmlAttribute attribute2 = xmlDoc.CreateAttribute("accountNum");
            attribute2.Value = accountnum;
            userNode.Attributes.Append(attribute2);

            XmlAttribute attribute3 = xmlDoc.CreateAttribute("valor");
            attribute3.Value = monto_consumo.ToString();
            userNode.Attributes.Append(attribute3);

            rootNode.AppendChild(userNode);

            bool result = false;
            SqlParameter paramResult = new SqlParameter("@respuesta", SqlDbType.VarChar, -1);
            paramResult.Direction = System.Data.ParameterDirection.Output;
            string parameterValue = xmlDoc.InnerXml.ToString();

            var addParameters = new List<SqlParameter>
             {
              new SqlParameter("@xml", parameterValue),
              paramResult
             };

            Control.Common.GlobalParameters.XmlPagoCanjePuntos = parameterValue;
            StringBuilder seguimiento = new StringBuilder();
            seguimiento.AppendLine(" Inicia seguimiento:");
            seguimiento.AppendLine("Antes de ejecutar SP_SECUENCIAS_IG"); //VJFRANCO 13/09/2022 new spPagoCanjePuntosGen

            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "PagoxCanjePuntosMonedero", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
                return false;
            }

            if (cadenaCon != "")
            {
                SqlConnection conn = new SqlConnection(cadenaCon);
                SqlCommand select = new SqlCommand("Exec PtsCliente.spPagoCanjePuntosGen @xml, @respuesta out", conn);
                try
                {

                    select.Parameters.AddRange(addParameters.ToArray());
                    conn.Open();
                    select.ExecuteNonQuery();

                    conn.Close();

                    string response = (string)paramResult.Value;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "PagoxCanjePuntosMonedero", "respuesta :" + response);
                    seguimiento.AppendLine("Ejecutado procedimiento spPagoCanjePuntosGen. Procediendo a Canjear ");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Fatura", "PagoxCanjePuntosMonedero", seguimiento.ToString());
                    return true;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "PagoxCanjePuntosMonedero", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                    //Guarda consumo de billetera en un archivo cuando no hay srv-pos para procesar por timer 
                    var query = "declare @respuesta as varchar(1000) " +
                        "declare @xml as varchar(1000) = '" + parameterValue + "'" +
                        " Exec PtsCliente.spPagoCanjePuntosGen @xml, @respuesta out; " +
                        " Select @respuesta ";
                    creaArhivoTemporalConsumoBilletera(query, this.GetNumeroFactura());

                    return false;
                }
            }
            else { return false; }
        }

        // Consumir los puntos por Pago con Monedero
        public bool ConsumirPuntosMonedero(ref POSEntities db, decimal puntos_consumo)
        {
            decimal canjeado = 0, ultpuntos = 0;


            var puntoscab = db.TblPuntosCab.FirstOrDefault(x => x.AccountNum == this.ClienteIdentificacion && x.Estado == 1 && x.Saldo > 0);
            if (puntoscab != null)
            {
                var puntos_expirar = (from x in db.TblPuntos
                                      where x.IdTblPuntosCab == puntoscab.IdTblPuntosCab
                                      && x.FechaExpiracion != null && x.Estado == 1 && x.Saldo > 0
                                      orderby x.FechaExpiracion ascending
                                      select x).ToList();

                for (int x = 0; x < puntos_expirar.Count; x++)
                {
                    int id_canje = puntos_expirar[x].IdTblPuntos;
                    canjeado = canjeado + puntos_expirar[x].Saldo;

                    TblPuntos p = (from a in db.TblPuntos
                                   where a.IdTblPuntos == id_canje
                                   && a.Estado == 1
                                   select a).First();
                    if (puntos_consumo >= canjeado)
                    {
                        p.Saldo = 0;
                        p.Estado = 0;
                        //db.SaveChanges();
                    }
                    else
                    {
                        p.Saldo = canjeado - puntos_consumo;
                        canjeado = puntos_consumo;
                        //db.SaveChanges();
                        break;
                    }
                }

                if (puntos_consumo >= canjeado)
                {
                    var puntos_saldo = (from x in db.TblPuntos
                                        where x.IdTblPuntosCab == puntoscab.IdTblPuntosCab
                                        && x.FechaExpiracion == null && x.Estado == 1 && x.Saldo > 0
                                        orderby x.Saldo ascending
                                        select x).ToList();
                    for (int y = 0; y < puntos_saldo.Count; y++)
                    {
                        int id_canje = puntos_saldo[y].IdTblPuntos;
                        canjeado = canjeado + puntos_saldo[y].Saldo;

                        TblPuntos p = (from a in db.TblPuntos
                                       where a.IdTblPuntos == id_canje
                                       && a.Estado == 1
                                       select a).First();
                        if (puntos_consumo >= canjeado)
                        {
                            p.Saldo = 0;
                            p.Estado = 0;
                            //db.SaveChanges();
                        }
                        else
                        {
                            p.Saldo = canjeado - puntos_consumo;
                            canjeado = puntos_consumo;
                            //db.SaveChanges();
                            break;
                        }
                    }
                }

                ultpuntos = (from x in db.TblPuntos
                             where x.IdTblPuntosCab == puntoscab.IdTblPuntosCab
                             && x.Estado == 1
                             select x).ToList().Select(x => x.Saldo).Sum();
                TblPuntosCab pc = puntoscab;
                pc.Saldo = ultpuntos;
                //db.SaveChanges();

                //decimal valconver = Control.WalletPoints.ClsPoints.FactorCanje;
                //var factcanje = new TblFacturaCanjes
                //{
                //    Fecha = DateTime.Now,
                //    ValorConversion = valconver,
                //    AccountNum = this.ClienteIdentificacion,
                //    TotalCanje = puntos_consumo * valconver,
                //    TotalPuntos = puntos_consumo,
                //    Estado = 1,
                //    UsuarioCreacion = this.ClienteIdentificacion
                //};
                //db.TblFacturaCanjes.Add(factcanje);
                //db.SaveChanges(); 

                return true;
            }
            else
                return false;
        }

        // Codigo Promocional.  JM 29-11-2019
        public bool agregarPromocionCodigo(core_descuento d, string forma_pago, string codigo) //decimal descuento 
        {
            return this.agregarDescuentoAdicional(d, forma_pago, codigo);

            //decimal prodPreDescuento = 0M, prodPreDescuentoActual = 0M;
            //foreach (var p in this.Productos)
            //{               
            //    prodPreDescuentoActual = decimal.Round((descuento / 100M), 2);          
            //    prodPreDescuento = Math.Round(Math.Round(p.Pvp * p.Cantidad, 2) * (descuento / 100M), 2);                
            //    p.DescuentoActual = prodPreDescuentoActual;
            //    p.DescuentoAX = prodPreDescuento;
            //    p.update(false);

            //} 
        }

        public void agregarDescuentoCompraGratis(decimal descuento, decimal saldoDisponible, out bool usoCompraGratis, out decimal saldoCompraGratis, out decimal montoCompraGratis) //decimal valor, 
        {
            usoCompraGratis = false;
            saldoCompraGratis = 0;
            montoCompraGratis = 0;
            decimal prodPreDescuento = 0M, prodPreDescuentoActual = 0M;
            foreach (var p in this.Productos)
            {
                //Si no queda saldo de la tarjeta, salir del bucle
                if (saldoDisponible <= 0)
                    break;

                prodPreDescuentoActual = decimal.Round((descuento / 100M), 2);
                prodPreDescuento = Math.Round((Math.Round(p.Pvp * p.Cantidad, 2) - p.Descuento) * (descuento / 100M), 2);

                p.DescuentoActual += prodPreDescuentoActual;
                p.DescuentoAX += prodPreDescuento;
                p.update(false);

                p.DescuentoTarjetasCompraGratis += prodPreDescuento;

                saldoDisponible -= prodPreDescuento;
                usoCompraGratis = true;
                montoCompraGratis += prodPreDescuento;

            }
            saldoCompraGratis = saldoDisponible;
        }

        public bool quitarDescuentoCompraGratis(decimal descuento, out bool usoCompraGratis)
        {
            usoCompraGratis = false;
            //decimal prodPreDescuento = 0M;
            decimal prodPreDescuentoActual = 0M;
            foreach (var p in this.Productos)
            {
                if (p.DescuentoTarjetasCompraGratis > 0)
                {
                    prodPreDescuentoActual = decimal.Round((descuento / 100M), 2);
                    //prodPreDescuento = Math.Round((Math.Round(p.Pvp * p.Cantidad, 2) - p.Descuento) * (descuento / 100M), 2);

                    p.DescuentoActual -= prodPreDescuentoActual;
                    //p.DescuentoAX -= prodPreDescuento;
                    p.Descuento -= p.DescuentoTarjetasCompraGratis;
                    p.update(false);

                    //p.DescuentoTarjetasCompraGratis -= prodPreDescuento;
                    p.DescuentoTarjetasCompraGratis = 0;
                    usoCompraGratis = false;
                }
            }
            return true;
        }

        public void grabaDevolucionIVA()
        {
            string query = string.Empty;
            int codError = 0;
            string msjError = string.Empty;
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", "Graba registors para devolución de IVA ");

            try
            {
                query = string.Empty;
                query = string.Concat(query, "exec spRegFacturaDevIVA", Environment.NewLine);
                query = string.Concat(query, $" @tipoDocumento = '{this.Documento}' ", Environment.NewLine);
                query = string.Concat(query, $" , @establecimiento = '{this.Establecimiento}' ", Environment.NewLine);
                query = string.Concat(query, $" , @punto_emision = '{this.PtoEmision}' ", Environment.NewLine);
                query = string.Concat(query, $" , @numero = {this.Secuencia} ", Environment.NewLine);
                query = string.Concat(query, $" , @montoIvaDevolver = {this.montoIvaDevolver} ", Environment.NewLine);
                query = string.Concat(query, $" , @claveAccesoComprobante = '{this.ClaveAccesoSRI}' ", Environment.NewLine);
                query = string.Concat(query, $" , @estado = 'I' ", Environment.NewLine);
                DataSet dtsRespuesta = Control.Common.General.GetDataSet(query);


                if (dtsRespuesta.Tables.Count > 0)
                {
                    for (int indexTable = 0; indexTable <= dtsRespuesta.Tables.Count - 1; indexTable++)
                    {
                        foreach (DataRow data in dtsRespuesta.Tables[indexTable].Rows)
                        {
                            codError = Int32.Parse(data["codError"].ToString());
                            msjError = data["mjsError"].ToString();


                        }
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", $" codError:{codError}, msjError: {msjError}");

                }

            }
            catch (Exception ex)
            {
                msjError = ex.Message;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", $" codError:-111, msjError: {msjError}");

            }






        }
        public void grabaDevolucionIVA(DevolucionIvaModel devolucionIva)
        {
            string query = string.Empty;
            int codError = 0;
            string msjError = string.Empty;
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", "Graba registors para devolución de IVA ");
            //string tipoDocumento, string establecimiento, string puntoEmision, string numDocumento, decimal montoIvaDevolver, string ClaveAccesoSRI, string estado
            try
            {
                query = string.Empty;
                query = string.Concat(query, "exec spRegFacturaDevIVA", Environment.NewLine);
                query = string.Concat(query, $" @tipoDocumento= '{devolucionIva.tipoDocumento}' ", Environment.NewLine);
                query = string.Concat(query, $" , @establecimiento = '{devolucionIva.establecimiento}' ", Environment.NewLine);
                query = string.Concat(query, $" , @punto_emision = '{devolucionIva.puntoEmision}' ", Environment.NewLine);
                query = string.Concat(query, $" , @numero = {devolucionIva.numDocumento} ", Environment.NewLine);
                query = string.Concat(query, $" , @cliente = '{devolucionIva.cliente}' ", Environment.NewLine);
                query = string.Concat(query, $" , @montoIvaDevolver = {devolucionIva.montoIvaDevolver} ", Environment.NewLine);        //_factura.montoIvaDevolver 
                query = string.Concat(query, $" , @claveAccesoComprobante = '{devolucionIva.ClaveAccesoSRI}' ", Environment.NewLine);
                query = string.Concat(query, $" , @estado = '{devolucionIva.estado}' ", Environment.NewLine);

                DataSet dtsRespuesta = Control.Common.General.GetDataSet(query);


                if (dtsRespuesta.Tables.Count > 0)
                {
                    for (int indexTable = 0; indexTable <=  dtsRespuesta.Tables.Count - 1; indexTable++)
                    {
                        foreach (DataRow data in dtsRespuesta.Tables[indexTable].Rows)
                        {
                            codError = Int32.Parse(data["codError"].ToString());
                            msjError = data["msjError"].ToString();

                        }
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", $" codError:{codError}, msjError: {msjError}");
                }

            }
            catch (Exception ex)
            {
                msjError = ex.Message;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", $" codError:-111, msjError: {msjError}");

            }
        }


        public void grabaDevolucionIVA(string tipoDocumento, string establecimiento, string puntoEmision, string numDocumento, decimal montoIvaDevolver, string ClaveAccesoSRI, string estado)
        {
            string query = string.Empty;
            int codError = 0;
            string msjError = string.Empty;
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", "Graba registors para devolución de IVA ");

            try
            {
                query = string.Empty;
                query = string.Concat(query, "exec spRegFacturaDevIVA", Environment.NewLine);
                query = string.Concat(query, $" @tipDocumento= '{this.Documento}' ", Environment.NewLine);
                query = string.Concat(query, $" , @establecimiento = '{this.Establecimiento}' ", Environment.NewLine);
                query = string.Concat(query, $" , @punto_emision = '{this.PtoEmision}' ", Environment.NewLine);
                query = string.Concat(query, $" , @numero = {this.Secuencia} ", Environment.NewLine);
                query = string.Concat(query, $" , @montoIvaDevolver = {this.montoIvaDevolver} ", Environment.NewLine);
                query = string.Concat(query, $" , @claveAccesoComprobante = '{this.ClaveAccesoSRI}' ", Environment.NewLine);
                query = string.Concat(query, $" , @estado = '{estado}' ", Environment.NewLine);

                DataSet dtsRespuesta = Control.Common.General.GetDataSet(query);


                if (dtsRespuesta.Tables.Count > 0)
                {
                    for (int indexTable = 0; dtsRespuesta.Tables.Count <= indexTable - 1; indexTable++)
                    {
                        foreach (DataRow data in dtsRespuesta.Tables[indexTable].Rows)
                        {
                            codError = Int32.Parse(data["codError"].ToString());
                            msjError = data["msjError"].ToString();

                        }
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", $" codError:{codError}, msjError: {msjError}");

                }

            }
            catch (Exception ex)
            {
                msjError = ex.Message;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "grabaDevolucionIVA", $" codError:-111, msjError: {msjError}");

            }






        }
     


    }
    public class Cupones
    {
        string _texto;
        public string Texto
        {
            get { return _texto; }
            set { _texto = value; }
        }

        bool _unico;
        public bool Unico
        {
            get { return _unico; }
            set { _unico = value; }
        }

        decimal _valor;
        public decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        bool _giftcard;
        public bool Giftcard
        {
            get { return _giftcard; }
            set { _giftcard = value; }
        }

        decimal _valorgiftcard;
        public decimal Valorgiftcard
        {
            get { return _valorgiftcard; }
            set { _valorgiftcard = value; }
        }

        string _referencia;
        public string Referencia
        {
            get { return _referencia; }
            set { _referencia = value; }
        }
    }
    public class Voucher
    {
        string _tipo;
        public string Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }
        string _pago;
        public string Pago
        {
            get { return _pago; }
            set { _pago = value; }
        }

        string _texto;
        public string Texto
        {
            get { return _texto; }
            set { _texto = value; }
        }

    }
    public class DescuentoAdicional
    {
        string _tipo;

        public string Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        decimal valor;

        public decimal Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        decimal porcentaje;

        public decimal Porcentaje
        {
            get { return porcentaje; }
            set { porcentaje = value; }
        }

        string _codigo;

        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }


    }
    public class Descuento
    {
        public string codigo { get; set; }
        public string parametro { get; set; }
        public string parametro2 { get; set; }
        public decimal valor { get; set; }
        public bool especial { get; set; }
    }

    public class RespuestaFactura
    {
        public int codError { get; set; }
        public string msjError { get; set; }
        
    }

}
