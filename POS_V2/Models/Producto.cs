using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace POS.Models
{
    public class Producto : INotifyPropertyChanged
    {
        public const string DESCUENTO_ESTABLECIMIENTO = "Establecimiento";
        public const string DESCUENTO_DIVISION = "Division";
        public const string DESCUENTO_CLIENTE = "Cliente";
        public const string DESCUENTO_CLIENTECATEGORIA = "ClienteCategoria";
        public const string DESCUENTO_PRODUCTO = "Producto";
        public const string DESCUENTO_PRODUCTO_CANTIDAD = "ProductoCantidad";
        public const string DESCUENTO_PRODUCTO_CANTIDAD_PESO = "ProductoCantidadPeso";
        public const string DESCUENTO_PRODUCTO_GRUPO_CANTIDAD = "ProductoGrupoCantidad";
        public const string DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD = "ProductoCategoriaCantidad";
        //        public const string DESCUENTO_ESTABLECIMIENTO_PRODUCTO = "EstablecimientoProducto";
        public const string DESCUENTO_ESTABLECIMIENTO_CATEGORIA = "EstablecimientoCategoria";
        public const string DESCUENTO_PRODUCTO_CLIENTE = "ProductoCliente";
        /** ***/
        public const string DESCUENTO_ESTABLECIMIENTO_VARIEDAD = "EstablecimientoVariedad";
        public const string DESCUENTO_ESTABLECIMIENTO_GRUPO = "EstablecimientoGrupo";
        public const string DESCUENTO_ESTABLECIMIENTO_SUB_GRUPO = "EstablecimientoSubGrupo";
        public const string DESCUENTO_ESTABLECIMIENTO_PRODUCTOS = "EstablecimientoProducto";
        /****/
        public int qtyBar;

        public decimal DescuentoAnterior = 0.0m;
        public decimal DescuentoActualAnterior = 0.0m;
        public decimal DescuentoConfiguraciones = 0.0m;

        public Producto()
        {
            this.TarjetasRegalo = new List<Control.TarjetaRegalo>();
            this.IdTemporal = Int64.Parse(DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + DateTime.Now.Millisecond.ToString().PadLeft(3, '0'));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        string _carnicero;
        public string CarniceroCOD { get { return _carnicero; } set { _carnicero = value; } }

        Int64 _idTemporal;

        public Int64 IdTemporal
        {
            get { return _idTemporal; }
            set { _idTemporal = value; }
        }

        decimal _descuentolocal;

        public decimal DescuentoLocal
        {
            get { return _descuentolocal; }
            set { _descuentolocal = value; }
        }

        decimal _descuentoEstblecimientoprodcutos;

        public decimal DescuentoEstablecimientoProductos
        {
            get { return _descuentoEstblecimientoprodcutos; }
            set { _descuentoEstblecimientoprodcutos = value; }
        }

        //Variable que aloja el descuento de AX
        decimal _descuentoAX;

        public decimal DescuentoAX
        {
            get
            { return decimal.Round(_descuentoAX, 6); } // Opozo cambio de desceuntos
            set { _descuentoAX = value; }
        }

        //Variable que aloja la sumatoria de descuento otorgado por cupones caducidad
        decimal _descuentoCuponesCaducidad;

        public decimal DescuentoCuponesCaducidad
        {
            get { return Math.Round(_descuentoCuponesCaducidad, 2); }
            set { _descuentoCuponesCaducidad = value; }
        }

        //Variable que aloja el descuento que lleva la linea por promo IVA
        decimal _descuentoIVA;

        public decimal DescuentoIVA
        {
            get { return Math.Round(_descuentoIVA, 2); }
            set { _descuentoIVA = value; }
        }

        //Propiedad que retorna el Total de la linea menos el descuento por promo IVA
        public decimal TotalPromoIVA
        {
            get
            {
                return decimal.Round(this.Subtotal + _iva - this.DescuentoIVA, 2);
            }
            set { _total = value; }
        }

        //Variable que aloja la sumatoria de descuentos otorgados por tarjetas PaviPlan
        decimal _descuentoTarjetasPaviPlan;

        public decimal DescuentoTarjetasPaviPlan
        {
            get { return Math.Round(_descuentoTarjetasPaviPlan, 2); }
            set { _descuentoTarjetasPaviPlan = value; }
        }

        //Variable que aloja el descuento otorgado por tarjeta CompraGratis
        decimal _descuentoTarjetasCompraGratis;

        public decimal DescuentoTarjetasCompraGratis
        {
            get { return Math.Round(_descuentoTarjetasCompraGratis, 2); }
            set { _descuentoTarjetasCompraGratis = value; }
        }

        //Variable que aloja el descuento otorgado por tarjeta CompraGratis
        decimal _descuentoCuponPromocional = 0;

        public decimal DescuentoCuponPromocional
        {
            get { return Math.Round(_descuentoCuponPromocional, 2); }
            set { _descuentoCuponPromocional = value; }
        }

        //Variable que aloja el descuento otorgado por combinacion de productos
        decimal _descuentoPorCombinacion;

        public decimal DescuentoPorCombinacion
        {
            get { return Math.Round(_descuentoPorCombinacion, 2); }
            set { _descuentoPorCombinacion = value; }
        }

        //Variable que aloja el descuento otorgado por Combos Caja (SuppItemTable)
        decimal _descuentoPorComboCaja;

        public decimal DescuentoPorComboCaja
        {
            get { return Math.Round(_descuentoPorComboCaja, 2, MidpointRounding.AwayFromZero); }
            set { _descuentoPorComboCaja = value; }
        }

        //Variable que aloja el descuento otorgado por ser item suplementario
        decimal _descuentoPorSerItemSuplemento;

        public decimal DescuentoPorSerItemSuplemento
        {
            get { return Math.Round(_descuentoPorSerItemSuplemento, 2); }
            set { _descuentoPorSerItemSuplemento = value; }
        }


        decimal _descuentolocalproducto;

        public decimal DescuentoLocalProducto
        {
            get { return _descuentolocalproducto; }
            set { _descuentolocalproducto = value; }
        }

        decimal _descuentoproductocliente;
        public decimal DescuentoProductoCliente
        {
            get { return _descuentoproductocliente; }
            set { _descuentoproductocliente = value; }
        }

        bool _tieneDescuentoProductoCliente;


        public bool TieneDescuentoProductoCliente
        {
            get { return _tieneDescuentoProductoCliente; }
            set { _tieneDescuentoProductoCliente = value; }
        }

        //eevv .ini 
        bool _tieneDescuentoPromoBines;
        public bool TieneDescuentoPromoBines
        {
            get { return _tieneDescuentoPromoBines; }
            set { _tieneDescuentoPromoBines = value; }
        }

        decimal _descuentopromobines;

        public decimal DescuentoPromoBines
        {
            get { return _descuentopromobines; }
            set { _descuentopromobines = value; }
        }
        //eevv .fin

        //** NUEVO DESCUENTO PARA PIAZZA **//
        decimal _descuentolocalconjunto;

        //eevv .ini
        decimal _retpor;

        public decimal RetencionPorcentaje
        {
            get { return _retpor; }
            set { _retpor = value; }
        }
        //eevv .fin

        public decimal DescuentoLocalConjunto
        {
            get { return _descuentolocalconjunto; }
            set { _descuentolocalconjunto = value; }
        }

        /*****NUEVOS DESCUENTOS ********/
        /*Establecimiento Variedad */
        decimal _descuentoestablecimientovariedad;
        public decimal DescuentoEstablecimientovariedad
        {
            get { return _descuentoestablecimientovariedad; }
            set { _descuentoestablecimientovariedad = value; }
        }
        /*Establecimiento grupo */
        decimal _descuentoestablecimientogrupo;
        public decimal DescuentoEstablecimientogrupo
        {
            get { return _descuentoestablecimientogrupo; }
            set { _descuentoestablecimientogrupo = value; }
        }
        /*Establecimiento subgrupo */
        decimal _descuentoestablecimientosubgrupo;
        public decimal DescuentoEstablecimientosubgrupo
        {
            get { return _descuentoestablecimientosubgrupo; }
            set { _descuentoestablecimientosubgrupo = value; }
        }
        /*Establecimiento categoria */
        decimal _descuentoestablecimientocategoria;
        public decimal DescuentoEstablecimientocategoria
        {
            get { return _descuentoestablecimientocategoria; }
            set { _descuentoestablecimientocategoria = value; }
        }

        //** FIN **//


        public decimal PrecioLocal
        {

            get
            {
                decimal retornaPrecioLocal = 0M;
                retornaPrecioLocal = this.PrecioAx - (this.PrecioAx * this.DescuentoLocal);
                retornaPrecioLocal = decimal.Round(retornaPrecioLocal, 2, MidpointRounding.AwayFromZero); //opozo redondeo
                retornaPrecioLocal = retornaPrecioLocal - (retornaPrecioLocal * this.DescuentoEstablecimientocategoria);
                retornaPrecioLocal = Math.Round(retornaPrecioLocal, 2, MidpointRounding.AwayFromZero); //opozo redondeo
                retornaPrecioLocal = retornaPrecioLocal - (retornaPrecioLocal * this.DescuentoEstablecimientovariedad);
                retornaPrecioLocal = Math.Round(retornaPrecioLocal, 2, MidpointRounding.AwayFromZero); //opozo redondeo
                retornaPrecioLocal = retornaPrecioLocal - (retornaPrecioLocal * this.DescuentoEstablecimientogrupo);
                retornaPrecioLocal = Math.Round(retornaPrecioLocal, 2, MidpointRounding.AwayFromZero); //opozo redondeo
                retornaPrecioLocal = retornaPrecioLocal - (retornaPrecioLocal * this.DescuentoEstablecimientosubgrupo);
                retornaPrecioLocal = Math.Round(retornaPrecioLocal, 2, MidpointRounding.AwayFromZero); //opozo redondeo

                decimal DsctoProducto = this.DescuentoEstablecimientoProductos;
                if (this.DescuentoLocalProducto != 0)
                {
                    DsctoProducto = this.DescuentoLocalProducto;
                }
               
                retornaPrecioLocal = retornaPrecioLocal - (retornaPrecioLocal * DsctoProducto);
                retornaPrecioLocal = Math.Round(retornaPrecioLocal, 2);

                retornaPrecioLocal = retornaPrecioLocal - (retornaPrecioLocal * this.DescuentoProductoCliente);
                retornaPrecioLocal = Math.Round(retornaPrecioLocal, 2);
                
                return decimal.Round(retornaPrecioLocal, 2, MidpointRounding.AwayFromZero); // redondeo descuento 
            }
        }


        public void actualizarDescuentoPromocionAX(List<Promocion> _listProductosDescuentos, string clienteIdentificacion, Factura _factura)
        {
            const string metodo = "actualizarDescuentoPromocionAX";
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Producto", metodo,
                $"Evaluando promociones AX para producto {this.Id}");

           
            // AHORA SE LIMPIAN LOS DESCUENTOS AQUÍ PARA DESCUENTOS PROMOCIÓN Y DESCUENTOS CUPONES
            this.DescuentoAX = 0M;
            this.DescuentoActual = 0M;
            this.DescuentoPorCombinacion = 0M;

            //cambiar estaa lista

            if (_listProductosDescuentos == null || !_listProductosDescuentos.Any())
                return;
            // AQUI NO ESTA MOSTRANDO LA NUEVA MEJOR PROMOCIO CUANDO SE CAMBIA DE CLIENTE APP A CF, SIGUE MOSTRANDO LA ANTERIOR -- ACTUALIZACION, AHORA SI RE CALCULA EL VALOR DEL DESCUENTO REAL

            // Filtrar promociones válidas: Tipo 2 o 8, con productos
            var promocionesValidas = _listProductosDescuentos
                .Where(d => (d.Tipo == 2 || d.Tipo == 8))  //!d.ListProductos.Any() hacer este cambio
                .ToList();

            if (!promocionesValidas.Any())
                return;

            // Obtener todas las reglas de descuento para este producto desde la vista
            using (var db = new POSEntities())
            {
                var promocionesItem = db.vw_DescuentosDetalleAX
                    .Where(x => x.ITEMID == this.Id &&
                                x.POS == 1) // Solo promociones para POS
                    .OrderByDescending(x => x.DESCUENTO) // Prioridad: mayor descuento
                    .ToList();

                // SE AGREGÓ ESTE CAMBIO

                var datosRelacionados = (from r1 in promocionesItem
                                         join r2 in _listProductosDescuentos on r1.REFRECID equals r2.RecId
                                         select r1).ToList();


                // Buscar la mejor promoción aplicable
                var mejorRegla = datosRelacionados.FirstOrDefault(x =>
                {
                    if (x.SHOWAPP == 1)
                        return _factura.EsClienteApp; // Requiere cliente registrado en app  //AQUI NO ESTA ESCOGIENDO LA PROMO POS=1 CUANDO SE CAMBIA DE CLIENTE APP A CF
                    return true; // SHOWAPP == 0 → siempre aplicable
                });
                //AHORA VALIDAR AQUI

                if (mejorRegla == null)
                    return; // No hay promoción aplicable

                // Verificar que pertenece a una promoción activa en _listProductosDescuentos
                var promoActiva = promocionesValidas.FirstOrDefault(p => p.RecId == mejorRegla.REFRECID);
                if (promoActiva == null)
                    return;

                // Validar si es restrictiva
                if (promoActiva.EsRestrictiva)
                {
                    if (!ValidarIdentificador.ValidarCedula(clienteIdentificacion) &&
                        !ValidarIdentificador.ValidarRUCNatural(clienteIdentificacion) &&
                        !ValidarIdentificador.ValidarPasaporte(clienteIdentificacion))
                    {
                        return; // No cumple condiciones de cliente para promoción restrictiva
                    }

                    // Validar límite diario (MaxCantidadDscto)
                    try
                    {
                        var fecha = DateTime.Now.Date;
                        var cantidadCompradaHoy = db.core_facturadetalle
                            .Join(db.core_factura, det => det.factura_id, fac => fac.id, (det, fac) => new { det, fac })
                            .Where(x => (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.fac.fecha_creacion) == fecha &&
                                        x.fac.cliente == clienteIdentificacion &&
                                        x.det.item_id == this.Id &&
                                        x.det.descuento > 0)
                            .Sum(x => (decimal?)x.det.cantidad) ?? 0;

                        if (cantidadCompradaHoy >= promoActiva.MaxCantidadDscto)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Producto", metodo,
                                $"Límite diario alcanzado para {this.Id}: {cantidadCompradaHoy}/{promoActiva.MaxCantidadDscto}");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Producto", metodo,
                            $"Error al validar límite de promoción restrictiva para {this.Id}: {ex.Message}");
                    }

                }

                //// Calcular descuento
                //decimal cantidadParaDscto = this.Cantidad;

                //// Aplicar límite si es restrictiva
                //if (promoActiva.EsRestrictiva)
                //{
                //    decimal cantidadCompradaHoy = 0;
                //    try
                //    {
                //        var fecha = DateTime.Now.Date;
                //        cantidadCompradaHoy = db.core_facturadetalle
                //            .Join(db.core_factura, det => det.factura_id, fac => fac.id, (det, fac) => new { det, fac })
                //            .Where(x => (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.fac.fecha_creacion) == fecha &&
                //                        x.fac.cliente == clienteIdentificacion &&
                //                        x.det.item_id == this.Id &&
                //                        x.det.descuento > 0)
                //            .Sum(x => (decimal?)x.det.cantidad) ?? 0;
                //    }
                //    catch { }

                //    decimal disponible = promoActiva.MaxCantidadDscto - cantidadCompradaHoy;
                //    if (disponible < 0) disponible = 0;
                //    cantidadParaDscto = Math.Min(cantidadParaDscto, disponible);
                //}
                // 8. Calcular cantidad elegible para descuento
                decimal cantidadParaDscto;

                if (promoActiva.Tipo == 8) // Combo cerrado
                {
                    decimal promo_cant = mejorRegla.CANTIDAD;
                    decimal promo_multiplica = this.Cantidad / promo_cant;
                    long gruposCompletos = (long)promo_multiplica;
                    cantidadParaDscto = gruposCompletos * promo_cant;

                    // Validar límite si es GeneralPromo (por grupo)
                    if (promoActiva.EsRestrictiva && promoActiva.GeneralPromo)
                    {
                        var cantcompradscto = (from a in _factura.Productos
                                               join b in promoActiva.ListProductos on a.Id equals b.Id
                                               group a by new { b.Grupo } into g
                                               select new
                                               {
                                                   grupo = g.Key.Grupo,
                                                   cantidad = g.Sum(x => x.Cantidad)
                                               }).FirstOrDefault();

                        decimal cantidadComprando = cantcompradscto?.cantidad ?? 0;

                        if (!_factura.Productos.Any(p => p.Id == this.Id))
                        {
                            cantidadComprando += this.Cantidad;
                        }

                        if (cantidadComprando > promoActiva.MaxCantidadDscto)
                        {
                            decimal cantdscto = cantidadComprando - promoActiva.MaxCantidadDscto;
                            cantidadParaDscto = Math.Max(0, cantidadParaDscto - cantdscto);
                        }
                    }

                    // Ajustar por combos ya comprados hoy
                    try
                    {
                        var fecha = DateTime.Now.Date;
                        var cantidadCompradaHoy = db.core_facturadetalle
                            .Join(db.core_factura, det => det.factura_id, fac => fac.id, (det, fac) => new { det, fac })
                            .Where(x => (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.fac.fecha_creacion) == fecha &&
                                        x.fac.cliente == clienteIdentificacion &&
                                        x.det.item_id == this.Id &&
                                        x.det.descuento > 0)
                            .Sum(x => (decimal?)x.det.cantidad) ?? 0;

                        long combosComprados = (long)(cantidadCompradaHoy / promo_cant);
                        decimal unidadesEnCombo = combosComprados * promo_cant;

                        if (cantidadParaDscto > (promoActiva.MaxCantidadDscto - unidadesEnCombo))
                        {
                            cantidadParaDscto = Math.Max(0, promoActiva.MaxCantidadDscto - unidadesEnCombo);
                        }
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Producto", metodo,
                            $"Error al validar combos previos para {this.Id}: {ex.Message}");
                    }
                }
                else // Tipo 2 o general
                {
                    cantidadParaDscto = this.Cantidad;

                    if (promoActiva.EsRestrictiva)
                    {
                        try
                        {
                            var fecha = DateTime.Now.Date;
                            var cantidadCompradaHoy = db.core_facturadetalle
                                .Join(db.core_factura, det => det.factura_id, fac => fac.id, (det, fac) => new { det, fac })
                                .Where(x => (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.fac.fecha_creacion) == fecha &&
                                            x.fac.cliente == clienteIdentificacion &&
                                            x.det.item_id == this.Id &&
                                            x.det.descuento > 0)
                                .Sum(x => (decimal?)x.det.cantidad) ?? 0;

                            decimal disponible = Math.Max(0, promoActiva.MaxCantidadDscto - cantidadCompradaHoy);
                            cantidadParaDscto = Math.Min(cantidadParaDscto, disponible);
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Producto", metodo,
                                $"Error al validar cantidad disponible para descuento: {ex.Message}");
                        }
                    }
                }

                //// Aplicar descuento
                //decimal descuentoAplicado = (mejorRegla.DESCUENTO / 100M) * Math.Round(this.Pvp * cantidadParaDscto, 2, MidpointRounding.AwayFromZero);
                //this.DescuentoAX = descuentoAplicado;

                //// Actualizar producto
                //this.update();

                //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Producto", metodo,
                //    $"Aplicado {mejorRegla.DESCUENTO}% de descuento AX a {this.Id} (cantidad: {cantidadParaDscto}, valor: {descuentoAplicado:C})");
                // 9. Aplicar descuento
                if (cantidadParaDscto >= mejorRegla.CANTIDAD)
                {
                    decimal descuentoAplicado = (mejorRegla.DESCUENTO / 100M) *
                        Math.Round(this.Pvp * cantidadParaDscto, 2, MidpointRounding.AwayFromZero);

                    this.DescuentoAX = descuentoAplicado;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Producto", metodo,
                        $"Aplicado {mejorRegla.DESCUENTO}% de descuento AX a {this.Id} " +
                        $"(cantidad: {cantidadParaDscto}, valor: {descuentoAplicado:C})");
                }
            }
        }

        public bool VerifyComboProducts(string codigo, Factura _factura)
        {
            var desctoComboProducts = 0M;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if (codigo != "")
            {
                if (codigo.StartsWith("210") && codigo.Length == 13)
                {
                    string barcodeReplace;
                    barcodeReplace = codigo;
                    barcodeReplace = barcodeReplace.Substring(0, barcodeReplace.Length - 6);
                    codigo = barcodeReplace + "000000";
                }


                //CAMBIO REALIZADO PARA LA PROMOCION DE CERVEZA + HIELO BG
                /*
                
                if ((_factura.Productos.Any(x => x.Id== "PG-AB-000940") || this.Id== "PG-AB-000940")  //sixpack brahma
                                             &&
                   (_factura.Productos.Any(x=> x.Id == "PG-AB-013008") || this.Id == "PG-AB-013008")//hielo all natural
                                            )
                {
                    
                     // PG - AB - 014701                    
                     // HIELERA BRAHMA PROMOCIONAL                  
                     // 2100039000000
                   
                    var cantBrahma = _factura.Productos.FirstOrDefault(x => x.Id == "PG-AB-000940");
                    var cantHielo = _factura.Productos.FirstOrDefault(x => x.Id == "PG-AB-013008");
                    var cantPromoHielera = _factura.Productos.FirstOrDefault(x => (x.Id == "PG-AB-014701" || x.CodigosBarra.Any(y => y.codigo == "2100039000000")));
                    
                        List<decimal> lista = new List<decimal>();
                        lista.Add(cantBrahma != null ? cantBrahma.Cantidad : this.Id == "PG-AB-000940"?this.Cantidad:0);
                        lista.Add(cantHielo != null ? cantHielo.Cantidad : this.Id == "PG-AB-013008" ? this.Cantidad : 0);
                    

                    if ((cantPromoHielera != null?cantPromoHielera.Cantidad:0) < lista.Min())
                    {
                        MessageBox.Show(this,"Recuerdele al cliente que puede llevar " + lista.Min().ToString() + " HIELERA BRAHMA PROMOCIONAL con el 100% de descuento");
                    }
                        if (cantPromoHielera != null)
                        {
                            cantPromoHielera.DescuentoAX = ((cantPromoHielera.Cantidad< lista.Min()? cantPromoHielera.Cantidad:lista.Min()) * cantPromoHielera._pvp) ;
                            cantPromoHielera.update();
                        }
                        else
                        {
                            if (this.Id== "PG-AB-014701")
                            {
                                this.DescuentoAX = ((this.Cantidad != lista.Min() ? this.Cantidad : lista.Min()) * this._pvp);
                                this.update();
                            }
                    }

                }
                */

                using (var db = new POSEntities())
                {
                    var parametro = db.core_parametro.Where(x => x.identificador == "PROMO_CARNES_CERVEZA" && x.valor == "TRUE").FirstOrDefault();

                    if (parametro != null)
                    {
                        if (parametro.fecha_creacion <= DateTime.Now.Date && parametro.fecha_modificacion >= DateTime.Now.Date)
                        {
                            //PROMOCION DE POR 4 LBS CARNE LLEVA 6 CERVEZA STELLA AL 20% DESC, PIAZZA 24 & VILLA CLUB 29

                            if (_factura.Establecimiento == "024" || _factura.Establecimiento == "029")
                            {
                                //Cerveza Stella : PG-AB-000003
                                var itemIdPromo = "PG-AB-000003";
                                var sumLibras = _factura.Productos.Where(x => POS.Control.Common.Promo.EsItemPromoCarneCerveza(x.Id) == true && x.Id != this.Id).Sum(x => x.Cantidad);

                                if (POS.Control.Common.Promo.EsItemPromoCarneCerveza(this.Id))
                                {
                                    sumLibras += this.Cantidad;
                                }

                                var remainderLbs = Math.Truncate(sumLibras / 4M);
                                var remainderCrvz = 0M;

                                //Cerveza Stella : PG-AB-000003
                                if (_factura.Productos.Any(x => x.Id == itemIdPromo) || this.Id == itemIdPromo)
                                {
                                    var cervezaStella = _factura.Productos.FirstOrDefault(x => x.Id == itemIdPromo);

                                    if (cervezaStella != null)
                                    {
                                        remainderCrvz = Math.Truncate(cervezaStella.Cantidad / 6M);
                                        if (remainderCrvz > remainderLbs) remainderCrvz = remainderLbs;
                                        cervezaStella.DescuentoAX = (remainderCrvz * 6M * cervezaStella._pvp) * 0.2M;
                                        cervezaStella.update();
                                    }
                                    else
                                    {
                                        remainderCrvz = Math.Truncate(this.Cantidad / 6M);
                                        if (remainderCrvz > remainderLbs) remainderCrvz = remainderLbs;
                                        this.DescuentoAX = (remainderCrvz * 6M * this._pvp) * 0.2M;
                                        this.update();
                                    }
                                }

                                //Recordatorio de promo
                                if (this.Id == itemIdPromo || POS.Control.Common.Promo.EsItemPromoCarneCerveza(this.Id))
                                {
                                    //string textoMsj = $"Recuerdele al cliente que por la compra de cada 4 libras de carne en los productos participantes le damos un 20% de descuento en el six pack de stella";
                                    //Control.Common.General.GetMensaje("POS - Promociones", textoMsj, "I");
                                    Control.Common.General.GetMensajeToList(605);

                                    // MessageBox.Show("Recuerdele al cliente que por la compra de cada 4 libras de carne en los productos participantes le damos un 20% de descuento en el six pack de stella", "Promociones");
                                }

                            }

                            //PROMOCION DE POR 4 LBS CARNE LLEVA 6 CERVEZA STELLA AL 20% DESC, LOS DEMAS

                            if (_factura.Establecimiento != "024" && _factura.Establecimiento != "029")
                            {
                                //SIX PACK DE PILSENER LIGHT : PG-AB-012961
                                var itemIdPromo = "PG-AB-012961";
                                var sumLibras = _factura.Productos.Where(x => POS.Control.Common.Promo.EsItemPromoCarneCerveza(x.Id) == true && x.Id != this.Id).Sum(x => x.Cantidad);

                                if (POS.Control.Common.Promo.EsItemPromoCarneCerveza(this.Id))
                                {
                                    sumLibras += this.Cantidad;
                                }

                                var remainderLbs = Math.Truncate(sumLibras / 4M);
                                var remainderCrvz = 0M;

                                //SIX PACK DE PILSENER LIGHT : PG-AB-012961
                                if (_factura.Productos.Any(x => x.Id == itemIdPromo) || this.Id == itemIdPromo)
                                {
                                    var sixPilsenerLight = _factura.Productos.FirstOrDefault(x => x.Id == itemIdPromo);

                                    if (sixPilsenerLight != null)
                                    {
                                        remainderCrvz = Math.Truncate(sixPilsenerLight.Cantidad / 1M);
                                        if (remainderCrvz > remainderLbs) remainderCrvz = remainderLbs;
                                        sixPilsenerLight.DescuentoAX = (remainderCrvz * sixPilsenerLight._pvp) * 0.2M;
                                        sixPilsenerLight.update();
                                    }
                                    else
                                    {
                                        remainderCrvz = Math.Truncate(this.Cantidad / 1M);
                                        if (remainderCrvz > remainderLbs) remainderCrvz = remainderLbs;
                                        this.DescuentoAX = (remainderCrvz * this._pvp) * 0.2M;
                                        this.update();
                                    }
                                }

                                //Recordatorio de promo
                                if (this.Id == itemIdPromo || POS.Control.Common.Promo.EsItemPromoCarneCerveza(this.Id))
                                {

                                    Control.Common.General.GetMensajeToList(606);

                                    //string textoMsj = $"Recuerdele al cliente que por la compra de cada 4 libras de carne en los productos participantes le damos un 20% de descuento en el six pack de pilsener light";
                                    //Control.Common.General.GetMensaje("POS - Promociones", textoMsj, "I");

                                    // MessageBox.Show("Recuerdele al cliente que por la compra de cada 4 libras de carne en los productos participantes le damos un 20% de descuento en el six pack de pilsener light", "Promociones");
                                }

                            }
                        }
                    }
                }



                using (var db = new POSEntities())
                {
                    var check = (from q in db.VW_CombosCaja
                                 where (q.ITEMRELATION == codigo
                                 || q.ITEMRELATIONCODE == codigo
                                 || q.SUPPITEMID == codigo
                                 || q.SUPPITEMCODE == codigo)
                                 && (q.ACCOUNTRELATION == _factura.Establecimiento
                                 || q.ACCOUNTRELATION == "")
                                 && q.TODATE >= DateTime.Now
                                 orderby q.ACCOUNTRELATION descending
                                 select q).ToList();

                    if (check.Count != 0 && check.FirstOrDefault().FROMDATE <= DateTime.Now && check.FirstOrDefault().TODATE >= DateTime.Now && (check.FirstOrDefault().ACCOUNTRELATION == _factura.Establecimiento || check.FirstOrDefault().ACCOUNTRELATION == ""))
                    {
                        if (check.FirstOrDefault().MULTIPLEQTY > 0) //CUANDO ES PROMOCION POR CANTIDAD ENTRA AQUI
                        {
                            if (codigo == check.FirstOrDefault().ITEMRELATIONCODE || codigo == check.FirstOrDefault().ITEMRELATION)
                            {
                                decimal cant = 0;
                                decimal qtySup = 0;
                                decimal MULTIPLEQTY = 0;
                                var ITEMRELATIONCODE = "";
                                var SUPPITEMCODE = "";
                                int ExisteItem = 0;
                                int MensajeHijo = 0;

                                //Productos de la factura que estan en combos como producto principal 
                                //y que ademas los productos secundarios tambien estan en la factura
                                foreach (VW_CombosCaja sq in check)
                                {
                                    //Busca la combinacion de producto promocion configurada EN EL LOCAL ACTUAL que coincidan con el codigo de 
                                    //producto proporcionado y esten activas
                                    var check2 = (from q in db.VW_CombosCaja
                                                  where (q.SUPPITEMCODE == sq.SUPPITEMCODE)
                                                  && (q.ACCOUNTRELATION == _factura.Establecimiento
                                                  || q.ACCOUNTRELATION == "")
                                                  && q.TODATE >= DateTime.Now
                                                  orderby q.ACCOUNTRELATION descending
                                                  select q).ToList();

                                    //Recorre, si hubiere, las combinaciones encontradas del local actual
                                    foreach (VW_CombosCaja sq2 in check2)
                                    {
                                        //Recorre los productos de la factura
                                        foreach (var i in _factura.Productos)
                                        {
                                            //Recorre los codigos de barra del producto
                                            foreach (var c in i.CodigosBarra)
                                            {
                                                //Si codigo de barra coincide con combinacion de descuento principal
                                                if (c.codigo == sq.ITEMRELATIONCODE)
                                                    //Se marca que existe combinacion para este producto como principal
                                                    ExisteItem = 1;
                                                //Si codigo de barra coincide con combinacion de descuento secundaria
                                                if (c.codigo == sq2.ITEMRELATIONCODE && sq.SUPPITEMCODE == sq2.SUPPITEMCODE)
                                                {
                                                    //Se suma la cantidad del producto que esta llevando
                                                    qtySup += i._cantidad;
                                                    //Se toma nota de los datos que tiene configurados esa combinacion
                                                    MULTIPLEQTY = sq.MULTIPLEQTY;
                                                    ITEMRELATIONCODE = sq.ITEMRELATIONCODE;
                                                    SUPPITEMCODE = sq.SUPPITEMCODE;
                                                }
                                            }
                                        }
                                    }
                                }

                                //Se busca la combinacion que existan productos secundarios de combos
                                //Si la sumatoria de cantidades supera lo requerido en MultipleQty se actualiza el DescuentoAx del producto con el porcentaje 
                                //Configurado en el campo SuppItemQty
                                /////Descuento al Hijo si entra item Padre
                                foreach (VW_CombosCaja sq in check)
                                {
                                    foreach (var i in _factura.Productos)
                                    {
                                        foreach (var c in i.CodigosBarra)
                                        {
                                            //Se pregunta si este codigo existe como secundario de una combinacion
                                            if (c.codigo == sq.SUPPITEMCODE)
                                            {
                                                //Si no existio combinacion principal, ira sumando cantidades del producto configurado como hijo
                                                if (ExisteItem == 0)
                                                {
                                                    qtySup += this._cantidad;
                                                    MensajeHijo = 1;
                                                }

                                                if (qtySup >= sq.MULTIPLEQTY)
                                                {
                                                    decimal descuento = 0;
                                                    try { descuento = check.First(x => x.ITEMRELATIONCODE == ITEMRELATIONCODE && x.SUPPITEMCODE == SUPPITEMCODE).SUPPITEMQTY; }
                                                    catch { descuento = check.First(x => (x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo) && x.SUPPITEMCODE == c.codigo).SUPPITEMQTY; }
                                                    MULTIPLEQTY = sq.MULTIPLEQTY;
                                                    decimal cal = qtySup / sq.MULTIPLEQTY;
                                                    cal = Math.Truncate(cal);


                                                    if (cal <= i._cantidad)
                                                        desctoComboProducts = (cal * i._pvp) * descuento;
                                                    else
                                                        desctoComboProducts = (i._cantidad * i._pvp) * descuento;

                                                    //Reset descuento combo caja
                                                    i.DescuentoAX = -i.DescuentoPorComboCaja;
                                                    i.update();
                                                    i.DescuentoPorComboCaja = 0;

                                                    desctoComboProducts = Math.Round(desctoComboProducts, 2, MidpointRounding.AwayFromZero);

                                                    i.DescuentoPorComboCaja += desctoComboProducts;
                                                    i.DescuentoAX = desctoComboProducts;
                                                    i.update();
                                                }
                                            }
                                        }
                                    }
                                }

                                if (MULTIPLEQTY < 1)
                                    MULTIPLEQTY = Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).MULTIPLEQTY, 2);

                                if (ExisteItem == 0 && ((qtySup > 0 && MensajeHijo == 0) || qtySup == 0))
                                    qtySup += this._cantidad;
                                try { cant = qtySup / MULTIPLEQTY; }
                                catch { cant = 0; }

                                var objCheck = check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo);
                                var strPrecioSugerido = (objCheck.SUPPITEMOPTIONAL == 0) ? string.Empty : string.Format(" ($ {0})", ((decimal)objCheck.SUPPITEMOPTIONAL / 100M).ToString("N2"));


                                MULTIPLEQTY = objCheck.MULTIPLEQTY;
                                var SUPPITEMNAME = objCheck.SUPPITEMNAME;
                                var SUPPITEMQTY = objCheck.SUPPITEMQTY;
                                var UNITID = objCheck.UNITID;
                                var ITEMNAME = objCheck.ITEMNAME;

                                string textoMsj = string.Empty;
                                if (cant >= 1)
                                {


                                    parametros = new List<ParametrosMensajes>();
                                    parametros.Add(new ParametrosMensajes() { codigo = "[cantidad]", valor = Math.Truncate(cant).ToString() });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[SUPPITEMNAME]", valor = SUPPITEMNAME });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[SUPPITEMQTY]", valor = Math.Round(SUPPITEMQTY * 100, 2).ToString() });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[strPrecioSugerido]", valor = strPrecioSugerido });
                                    Control.Common.General.GetMensajeToList(607, parametros);

                                    
                                    //textoMsj = $"Recuérdele al cliente que se puede llevar {Math.Truncate(cant)} {SUPPITEMNAME}   con el  % {Math.Round(SUPPITEMQTY * 100, 2)} de descuento {strPrecioSugerido}";
                                    //Control.Common.General.GetMensaje("POS - Promociones", textoMsj, "I");

                                    // MessageBox.Show("Recuérdele al cliente que se puede llevar " + Math.Truncate(cant) + " " + check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).SUPPITEMNAME + "  con el  %" + Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).SUPPITEMQTY * 100, 2) + " de descuento" + strPrecioSugerido);
                                }
                                else
                                {
                                    MULTIPLEQTY = Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).MULTIPLEQTY, 2);
                                    UNITID = check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).UNITID;
                                    ITEMNAME = check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).ITEMNAME;
                                    SUPPITEMQTY = Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).SUPPITEMQTY * 100, 2);

                                    parametros = new List<ParametrosMensajes>();
                                    parametros.Add(new ParametrosMensajes() { codigo = "[MULTIPLEQTY]", valor = MULTIPLEQTY.ToString() });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[UNITID]", valor = UNITID.ToString() });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[ITEMNAME]", valor = ITEMNAME });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[SUPPITEMQTY]", valor = SUPPITEMQTY.ToString() });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[strPrecioSugerido]", valor = strPrecioSugerido });
                                    Control.Common.General.GetMensajeToList(608, parametros);



                                    //MessageBox.Show("Recuérdele al cliente que se puede llevar " + "1" + " " + check.FirstOrDefault().SUPPITEMNAME + "  con el  %" + Math.Round(check.FirstOrDefault().SUPPITEMQTY * 100, 2) + " de descuento.");

                                    //MessageBox.Show("Recuérdele al cliente que por la compra de cada:  " 
                                    //     + Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).MULTIPLEQTY, 2) 
                                    //     + " " + check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).UNITID 
                                    //     + " de ** " + check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).ITEMNAME 
                                    //     + " **  Puede llevar ** " + check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).SUPPITEMNAME 
                                    //     + " **  con el %" + Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).SUPPITEMQTY * 100, 2) 
                                    //     + " de descuento" + strPrecioSugerido);

                                }

                            }
                            else
                            {///Descuento al Hijo si entra el mismo
                                decimal qtySup = 0;
                                decimal MULTIPLEQTY = 0;
                                var ITEMRELATIONCODE = "";
                                var SUPPITEMCODE = "";

                                foreach (VW_CombosCaja sq in check)
                                {
                                    foreach (var i in _factura.Productos)
                                    {
                                        foreach (var c in i.CodigosBarra)
                                        {
                                            if (c.codigo == sq.ITEMRELATIONCODE)
                                            {
                                                qtySup += i._cantidad;
                                                MULTIPLEQTY = sq.MULTIPLEQTY;
                                                ITEMRELATIONCODE = sq.ITEMRELATIONCODE;
                                                SUPPITEMCODE = sq.SUPPITEMCODE;
                                            }
                                        }
                                    }
                                }
                                if (qtySup >= MULTIPLEQTY)
                                {
                                    //decimal descuento = check.FirstOrDefault().SUPPITEMQTY;
                                    decimal descuento = 0;
                                    try
                                    { descuento = check.First(x => x.ITEMRELATIONCODE == ITEMRELATIONCODE && x.SUPPITEMCODE == SUPPITEMCODE).SUPPITEMQTY; }
                                    catch
                                    {
                                        descuento = 0;
                                        MULTIPLEQTY = 1;
                                    }
                                    decimal cal = qtySup / MULTIPLEQTY;
                                    cal = Math.Truncate(cal);
                                    if (cal <= this._cantidad)
                                        desctoComboProducts = (cal * this._pvp) * descuento;
                                    else
                                        desctoComboProducts = (this._cantidad * this._pvp) * descuento;

                                    //Reset descuento combo caja
                                    this.DescuentoAX = -this.DescuentoPorComboCaja;
                                    this.update();
                                    this.DescuentoPorComboCaja = 0;

                                    desctoComboProducts = Math.Round(desctoComboProducts, 2, MidpointRounding.AwayFromZero);

                                    this.DescuentoPorComboCaja += desctoComboProducts;
                                    this.DescuentoAX = desctoComboProducts;
                                    this.update();
                                }

                            }
                        }
                        else //SI NO ES POR CANTIDAD ENTRA POR AQUI
                        {
                            if (codigo == check.FirstOrDefault().ITEMRELATIONCODE || codigo == check.FirstOrDefault().ITEMRELATION)
                            {
                                int canastas1 = 0;

                                decimal MULTIPLEQTY = Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).MULTIPLEQTY, 2);
                                string UNITID = check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).UNITID;
                                string ITEMNAME = check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).ITEMNAME;
                                decimal SUPPITEMQTY = Math.Round(check.First(x => x.ITEMRELATIONCODE == codigo || x.ITEMRELATION == codigo).SUPPITEMQTY * 100, 2);
                                string SUPPITEMNAME = check.FirstOrDefault().SUPPITEMNAME;


                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[ITEMNAME]", valor = ITEMNAME });
                                parametros.Add(new ParametrosMensajes() { codigo = "[SUPPITEMNAME]", valor = SUPPITEMNAME.ToString() });                                
                                parametros.Add(new ParametrosMensajes() { codigo = "[SUPPITEMQTY]", valor = SUPPITEMQTY.ToString() });
                                Control.Common.General.GetMensajeToList(609, parametros);


                                //string textoMsj = $"RECUERDELE AL CLIENTE QUE POR LA COMPRA DE {check.FirstOrDefault().ITEMNAME}";
                                //textoMsj = string.Concat(textoMsj, $"**  PUEDE LLEVAR LA PROMOCIÓN DE **");
                                //textoMsj = string.Concat(textoMsj, $" {check.FirstOrDefault().SUPPITEMNAME}");
                                //textoMsj = string.Concat(textoMsj, $" CON EL % {Math.Round(check.FirstOrDefault().SUPPITEMQTY * 100, 2)} DE DESCUENTO.");
                                //Control.Common.General.GetMensaje("POS - Promociones", textoMsj, "I");


                                //MessageBox.Show("RECUERDELE AL CLIENTE QUE POR LA COMPRA DE:  ** " 
                                //    + check.FirstOrDefault().ITEMNAME 
                                //    + " **  PUEDE LLEVAR LA PROMOCIÓN DE ** " + check.FirstOrDefault().SUPPITEMNAME 
                                //    + " **  CON EL %" + Math.Round(check.FirstOrDefault().SUPPITEMQTY * 100, 2) + " DE DESCUENTO.");

                                foreach (VW_CombosCaja sq in check)
                                {
                                    foreach (var i in _factura.Productos)
                                    {
                                        foreach (var c in i.CodigosBarra)
                                        {
                                            if (i.CodigosBarra[0].codigo == sq.ITEMRELATIONCODE)
                                            {
                                                canastas1 += int.Parse(i._cantidad.ToString());
                                            }
                                        }
                                    }
                                }

                                foreach (VW_CombosCaja sq in check)
                                {
                                    foreach (var i in _factura.Productos)
                                    {
                                        foreach (var c in i.CodigosBarra)
                                        {
                                            if (c.codigo == sq.SUPPITEMCODE)
                                            {
                                                if (canastas1 > 0)
                                                {
                                                    decimal descuento = check.FirstOrDefault().SUPPITEMQTY;
                                                    desctoComboProducts = (canastas1 * i._pvp) * descuento;
                                                }
                                                else
                                                {
                                                    decimal descuento = check.FirstOrDefault().SUPPITEMQTY;
                                                    desctoComboProducts = (this._cantidad * i._pvp) * descuento;
                                                }

                                                i.DescuentoAX = desctoComboProducts;
                                                i.update();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                int canastas = 0;

                                foreach (VW_CombosCaja sq in check)
                                {
                                    foreach (var i in _factura.Productos)
                                    {
                                        foreach (var c in i.CodigosBarra)
                                        {
                                            if (c.codigo == sq.ITEMRELATIONCODE)
                                            {
                                                canastas += int.Parse(i._cantidad.ToString());

                                                if (canastas >= this._cantidad)
                                                {
                                                    decimal descuento = check.FirstOrDefault().SUPPITEMQTY;
                                                    desctoComboProducts = this.SubtotalSinDescuento * descuento;

                                                    this.DescuentoAX = desctoComboProducts;
                                                    this.update();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }


            return true;
        }

        public VW_PromoChoose VerifyPromoChoose(string _codigo)
        {
            var db = new POSEntities();
            VW_PromoChoose promo_prod = null;

            if (_codigo != "")
            {
                if (_codigo.StartsWith("210") && _codigo.Length == 13)
                {
                    string barcodeReplace;
                    barcodeReplace = _codigo;
                    barcodeReplace = barcodeReplace.Substring(0, barcodeReplace.Length - 6);
                    _codigo = barcodeReplace + "000000";
                }

                promo_prod = (from q in db.VW_PromoChoose
                              where q.BARCODE == _codigo
                                  || q.ITEMID == _codigo
                              select q).FirstOrDefault();
            }

            return promo_prod;
        }



        public bool esPeso
        {
            get
            {
                if (this.Unidad.ToLower() != "und" && this.Unidad.ToLower() != "uns")
                {
                    return true;
                }
                return false;
            }
        }
        public bool esAjustado;
        public bool EsAjustado
        {
            get
            {
                return esAjustado;
            }
            set
            {
                esAjustado = value;
            }
        }
        public bool esRegalo = false;
        public bool EsRegalo
        {
            get
            {
                return esRegalo;
            }
            set
            {
                esRegalo = value;
            }
        }
        public decimal ajuste;
        public decimal Ajuste
        {
            get
            {
                return ajuste;
            }
            set
            {
                ajuste = value;
            }
        }

        int _itemtype;

        public int Itemtype
        {
            get { return _itemtype; }
            set { _itemtype = value; }
        }

        string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        string _grupo;

        public string Grupo
        {
            get { return _grupo; }
            set { _grupo = value; }
        }

        string _grupoN;

        public string GrupoN
        {
            get { return _grupoN; }
            set { _grupoN = value; }
        }
        string _operador;

        public string Operador
        {
            get { return _operador; }
            set { _operador = value; }
        }

        string _categoria;

        public string Categoria
        {
            get { return _categoria; }
            set { _categoria = value; }
        }

        string _subGrupo;

        public string SubGrupo
        {
            get { return _subGrupo; }
            set { _subGrupo = value; }
        }

        string _variedad;

        public string Variedad
        {
            get { return _variedad; }
            set { _variedad = value; }
        }

        string _proveedorPricipal;

        public string ProveedorPricipal
        {
            get { return _proveedorPricipal; }
            set { _proveedorPricipal = value; }
        }

        string _nombre;

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        public decimal CantidadOriginal { get; set; }
        public decimal DescuentoOriginal { get; set; }

        decimal _cantidad;

        public decimal Cantidad
        {
            get { return _cantidad; }
            set
            {
                _cantidad = value;
                OnPropertyChanged("Cantidad");
                OnPropertyChanged("Subtotal");
                //OnPropertyChanged("Iva");
                OnPropertyChanged("Descuento");
                OnPropertyChanged("Total");
            }
        }

        decimal _cantidadINEC;

        public decimal CantidadINEC
        {
            get { return _cantidadINEC; }
            set { _cantidadINEC = value; }
        }

        int _unidades;

        public int Unidades
        {
            get { return _unidades; }
            set { _unidades = value; OnPropertyChanged("Unidades"); }
        }

        decimal _costo;

        public decimal Costo
        {
            get { return _costo; }
            set { _costo = value; }
        }

        decimal _precioax;

        public decimal PrecioAx
        {
            get { return _precioax; }
            set { _precioax = value; }
        }

        decimal _pvp;

        public decimal Pvp
        {
            get { return _pvp; }
            set { _pvp = value; }
        }




        decimal _descuento;

        public decimal Descuento
        {
            get { return _descuento; }
            set
            {
                _descuento = value;
                OnPropertyChanged("Cantidad");
                OnPropertyChanged("Subtotal");
                OnPropertyChanged("Descuento");
                OnPropertyChanged("Total");
            }
        }

        decimal _subtotal;

        public decimal Subtotal
        {
            get
            {
                var val = Math.Round((this._pvp * this._cantidad) - this._descuento, 2, MidpointRounding.AwayFromZero);
                return val;
            }
            set
            {
                _subtotal = value;
            }
        }

        public decimal SubtotalSinDescuento
        {
            get
            {
                var val = Math.Round(this._pvp * this._cantidad, 2, MidpointRounding.AwayFromZero);
                return val;
            }
        }

        decimal _iva;

        public decimal Iva
        {
            get { return _iva; }
            set { _iva = value; OnPropertyChanged("Iva"); }
        }
        decimal _total;

        public decimal Total
        {
            get
            {
                return decimal.Round(this.Subtotal + _iva, 2);
            }
            set { _total = value; }
        }

        string _unidad;

        public string Unidad
        {
            get { return _unidad; }
            set { _unidad = value; }
        }

        decimal _ivaProducto;

        public decimal IvaProducto
        {
            get { return _ivaProducto; }
            set { _ivaProducto = value; }
        }

        decimal _descuentoActual;

        public decimal DescuentoActual
        {
            get { return _descuentoActual; }
            set { _descuentoActual = value; }
        }

        decimal _porcDescuentoDivisionEmpleado = 0;

        public decimal PorcDescuentoDivisionEmpleado
        {
            get { return _porcDescuentoDivisionEmpleado; }
            set { _porcDescuentoDivisionEmpleado = value; }
        }

        bool _activadoPorcDctoDivisionEmpleado = false;

        public bool ActivadoPorcDctoDivisionEmpleado
        {
            get { return _activadoPorcDctoDivisionEmpleado; }
            set { _activadoPorcDctoDivisionEmpleado = value; }
        }

        bool _esExcluidoPromoIVA = false;

        public bool EsExcluidoPromoIVA
        {
            get { return _esExcluidoPromoIVA; }
            set { _esExcluidoPromoIVA = value; }
        }

        public List<CodigoBarra> CodigosBarra { get; set; }

        public List<Descuento> Descuentos { get; set; }
        public List<Descuento> DescuentosCupon { get; set; }

        DateTime _fechaCreacion;

        public DateTime FechaCreacion
        {
            get { return _fechaCreacion; }
            set { _fechaCreacion = value; }
        }

        #region Propiedades exclusivas para generar NC

        public decimal DescuentoNC
        {
            get
            {
                if (this.IvaProducto > 0)
                {
                    if (POS.Control.Common.Promo.EsFechaPromoIVA(this.FechaCreacion) && this.EsExcluidoPromoIVA == false)
                    {
                        var db = new POSEntities();
                        var porcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;// Decimal.Parse((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));
                        return decimal.Round(this.Descuento + (((this.Pvp * this.Cantidad) - this.Descuento) * porcPromo / 100), 2);
                    }
                    else
                        return this.Descuento;
                }
                else
                    return this.Descuento;
            }
        }

        public decimal SubtotalConDescNC
        {
            get
            {
                return decimal.Round((this.Pvp * this.Cantidad) - this.DescuentoNC, 2);
            }
        }

        public decimal IvaNC
        {
            get
            {
                if (this.IvaProducto > 0)
                {
                    var db = new POSEntities();
                    var porc_iva = Control.Common.GlobalParameters.IVAGEN / 100;
                    return decimal.Round(this.SubtotalConDescNC * porc_iva, 2);
                }
                else
                    return 0;
            }
        }

        public decimal TotalNC
        {
            get
            {
                return decimal.Round(this.SubtotalConDescNC + IvaNC, 2);
            }
        }

        #endregion

        public List<POS.Control.TarjetaRegalo> TarjetasRegalo { get; set; }

        /// <summary>
        /// Calcula el I.V.A de ITEM
        /// </summary>
        /// <param name="iva">Valor de Impuesto</param>
        public void calcularIVA(decimal iva)
        {
            this._ivaProducto = (iva / 100M);
            this.Iva = decimal.Round(this.Subtotal * (iva / 100M), 2);
        }

        /// <summary>
        /// Calcula el descuento Normal
        /// </summary>
        /// <param name="descuento">Valor de descuento a aplicar</param>
        public void calcularDescuento(decimal descuento)
        {
            // prueba de descuento en los item mayor decimales antes en 2 ahora en 6 
            this._descuentoActual = decimal.Round((descuento / 100M), 2);
            this.Descuento = decimal.Round(this._pvp * this._cantidad * (descuento / 100M), 2);
        }



        void setProductoDataSp(pos_item item, Factura factura, pos_customer cliente, string barcode = "")
        {




        }
        void setProductoData(pos_item item, Factura factura, pos_customer cliente, string barcode = "")
        {
            this.Id = item.ITEMID;
            this.Grupo = item.ITEMGROUPID;
            this.Categoria = item.categoria;
            this.Variedad = item.VARIEDAD;
            this.SubGrupo = item.SUBGRUPO;
            this.Unidad = item.UNITID;
            this.Costo = item.COST;
            this.GrupoN = item.GRUPO;

            //this.Pvp = item.PRICE;
            //obtiene porcentaje de retencion del item.
            try
            {
                this.RetencionPorcentaje = (decimal)item.retporc;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Producto", "setProductoData", "Ha courrido una Excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Producto", "setProductoData", "entra al Catch; se asigna el valor de 1 al campo retporc.");
                this.RetencionPorcentaje = 1;
            }


            this.PrecioAx = (decimal)item.PRICE;

            if (barcode != "")
            {
                using (var db = new POSEntities())
                {
                    var qty = (from q in db.VW_ITEMBARCODEQTY
                               where q.ITEMBARCODE == barcode
                               && q.QTY > 0
                               select q).FirstOrDefault();

                    if (qty != null)
                    {
                        this.Cantidad = qty.QTY;
                        this.CantidadINEC = qty.QTY;
                        this.Unidades = (int)qty.QTY;
                    }
                    else
                    {
                        this.Cantidad = 1;
                        this.CantidadINEC = 1;
                        this.Unidades = 1;

                    }
                }
            }
            else
            {
                if (qtyBar > 0) //AGREGA CANTIDAD CASO PANES
                {
                    this.Cantidad = qtyBar;
                    this.CantidadINEC = qtyBar;
                    this.Unidades = qtyBar;
                }
                else
                {
                    this.Cantidad = 1;
                    this.CantidadINEC = 1;
                    this.Unidades = 1;
                }
            }

            this.Itemtype = item.ITEMTYPE;

            decimal porcDescuentoDivisionEmpleado = 0;
            if (Itemtype != 2)
            {
                var descuento = getDescuento(factura, cliente, ref porcDescuentoDivisionEmpleado);
                //DescuentoConfiguraciones = descuento;
                this.PorcDescuentoDivisionEmpleado = porcDescuentoDivisionEmpleado;
                this.calcularDescuento(descuento);
                this.calcularIVA(item.TAXVALUE);
            }
            else if (item.ITEMTYPE == 2)
            {
                var descuento = getDescuento(factura, cliente, ref porcDescuentoDivisionEmpleado);
                this.PorcDescuentoDivisionEmpleado = porcDescuentoDivisionEmpleado;
                this.calcularDescuento(descuento);

                this._iva = 0M;
                this._ivaProducto = 0M;
            }

            this.Pvp = this.PrecioLocal - Descuento;
            this.Nombre = item.ITEMNAME;

            this.EsExcluidoPromoIVA = Control.Common.Promo.EsItemExcluidoPromoIVA(this.Id);

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", $"getProductoData", $" this.Nombre : {item.ITEMNAME}");
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", $"getProductoData", $" barcode: {item.ITEMID}");

        }

        /*
        void updatePvpLocal(Factura factura)
        {           
            if (factura.Documento == "F")
            {
                var desc_proc1 = 0M;

                if (this.Descuentos.Any(x => x.codigo == DESCUENTO_ESTABLECIMIENTO))
                {
                    if (factura.Establecimiento == this.Descuentos.First(x => x.codigo == DESCUENTO_ESTABLECIMIENTO).parametro)
                    {
                        desc_proc1 = this.Descuentos.First(x => x.codigo == DESCUENTO_ESTABLECIMIENTO).valor;
                    }
                }

                //=(p1)+{(100-p1)*(p2/100)}
                if (this.Descuentos.Any(x => x.codigo == DESCUENTO_ESTABLECIMIENTO_PRODUCTO))
                {
                    if (factura.Establecimiento == this.Descuentos.First(x => x.codigo == DESCUENTO_ESTABLECIMIENTO_PRODUCTO).parametro && this.Id == this.Descuentos.First(x => x.codigo == DESCUENTO_ESTABLECIMIENTO_PRODUCTO).parametro2)
                    {
                        desc_proc1 = desc_proc1 + ((100 - desc_proc1) * (this.Descuentos.First(x => x.codigo == DESCUENTO_ESTABLECIMIENTO_PRODUCTO).valor / 100));
                    }
                }
            }
        }
        */


        /// <summary>
        /// Busca el producto si encuentra llena los datos.
        /// </summary>
        /// <param name="codigo">Codigo de barra o Codigo de ITEM</param>
        /// <param name="factura">Para datos de factura</param>
        /// <param name="cliente">Para datos de cliente</param>
        /// <returns>Verdadero si encuentra el ITEM, False si no encuentra</returns>
        public bool getProducto(string codigo, Factura factura, pos_customer cliente)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", $"getProducto", $" codigo : {codigo}");


            codigo = codigo.Trim();
            pos_item item = null;
            var result = false;

            using (var db = new POSEntities())
            {
                var productID = db.pos_item.Any(x => x.ITEMID == codigo); //&& x.ITEMTYPE!=2);
                var barcode = db.pos_itembarra.Any(x => x.ITEMBARCODE == codigo || "F" + x.ITEMBARCODE == codigo);

                result = productID || barcode;

                if (productID)
                {
                    item = db.pos_item.Single(x => x.ITEMID == codigo);
                    setProductoData(item, factura, cliente);

                    this.CodigosBarra = db.pos_itembarra.Where(x => x.ITEMID == this.Id).Select(x => new CodigoBarra {ID = x.ITEMID, codigo = x.ITEMBARCODE }).ToList();
                }
                else if (barcode)
                {

                    var item_id = db.pos_itembarra.FirstOrDefault(x => x.ITEMBARCODE == codigo || "F" + x.ITEMBARCODE == codigo);
                    if (db.pos_item.Any(x => x.ITEMID == item_id.ITEMID))// && x.ITEMTYPE != 2))
                    {
                        item = db.pos_item.Single(x => x.ITEMID == item_id.ITEMID);
                        setProductoData(item, factura, cliente, codigo);
                        this.CodigosBarra = db.pos_itembarra.Where(x => x.ITEMID == this.Id).Select(x => new CodigoBarra { ID = x.ITEMID, codigo = x.ITEMBARCODE }).ToList();
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }

                if (!result)
                {
                    item = getProductoPeso(codigo);

                    if (item != null)
                    {
                        setProductoData(item, factura, cliente);
                        this.CodigosBarra = db.pos_itembarra.Where(x => x.ITEMID == this.Id).Select(x => new CodigoBarra { ID = x.ITEMID, codigo = x.ITEMBARCODE }).ToList();
                        result = true;
                    }
                }
                if (item != null)
                {
                    //if (item.PRICE <= 0 || item.COST > item.PRICE) Ing Antonio aprobo venta costo>precio
                    if (item.PRICE <= 0)
                    {
                        item = null;
                        result = false;
                    }
                }
                
            }
            return result;
        }


        public pos_item getProductoPeso(string codigo)
        {
            try
            {
                using (var db = new POSEntities())
                {
                    //  var cod = db.core_parametro.First(x => x.identificador == "ID_PRODUCTO_PESO");

                    if (codigo.Length >= POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso.Length || codigo.Length >= POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPesoAlt.Length)
                    {
                        if (codigo.ToString().Substring(0, POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso.ToString().Length) ==
                            POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso.ToString() ||
                            codigo.ToString().Substring(0, POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPesoAlt.Length) == POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPesoAlt)
                        {
                            //int qty = int.Parse(codigo.ToString().Substring(7, 5).ToString());
                            var item_barra = (codigo.ToString().Substring(0, codigo.ToString().Length - 6) + "000000").ToString();

                            if (item_barra.Length >= 16)
                            {
                                item_barra = item_barra.Substring(0, 13);
                                item_barra = (codigo.ToString().Substring(0, item_barra.ToString().Length - 6) + "000000").ToString();
                            }

                            if (db.pos_itembarra.Any(x => x.ITEMBARCODE == item_barra))
                            {
                                var barra = db.pos_itembarra.First(x => x.ITEMBARCODE == item_barra);

                                if (db.pos_item.Any(x => x.ITEMID == barra.ITEMID && x.UNITID.ToUpper() == "LB"))
                                {
                                    return db.pos_item.First(x => x.ITEMID == barra.ITEMID && x.UNITID.ToUpper() == "LB");
                                }
                                else if (db.pos_item.Any(x => x.ITEMID == barra.ITEMID && x.UNITID.ToUpper() == "UND"))
                                {
                                    qtyBar = int.Parse(codigo.ToString().Substring(7, 5).ToString());
                                    return db.pos_item.First(x => x.ITEMID == barra.ITEMID && x.UNITID.ToUpper() == "UND");
                                }
                                else
                                {
                                    return db.pos_item.First(x => x.ITEMID == barra.ITEMID);
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public decimal getPrecioEtiqueta(string codigo, string item_id)
        {
            try
            {
                using (var db = new POSEntities())
                {
                    if (db.pos_item.Any(x => x.ITEMID == item_id && x.UNITID.ToUpper() == "LB"))
                    {
                        // var cod = db.core_parametro.First(x => x.identificador == "ID_PRODUCTO_PESO");

                        if (codigo.ToString().Substring(0, POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso.Length) == POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso)
                        {
                            if (!codigo.ToString().Substring(POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso.Length + 4, 6).Equals("000000"))
                            {
                                var enteros = codigo.ToString().Substring(POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso.Length + 4, 3);
                                var decimales = codigo.ToString().Substring(POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso.Length + 7, 2);
                                return decimal.Parse(enteros) + (decimal.Parse(decimales) / 100);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Producto", "getPrecioEtiqueta", ex.Message + ex.StackTrace);
            }
            return 0M;
        }

        public bool getGiftCard(string codigo, Factura factura, pos_customer cliente)
        {
            pos_item gift_card = null; ;
            bool result = false;
            using (var db = new POSEntities())
            {
                if (db.core_parametro.Any(x => x.identificador == "GIFTCARD"))
                {
                    var codigo_giftcard = db.core_parametro.First(x => x.identificador == "GIFTCARD").valor;
                    var codigobarra_giftcard = db.core_parametro.First(x => x.identificador == "GIFTCARD").parametro2;
                    if (db.pos_item.Any(x => x.ITEMID == codigo_giftcard))
                        gift_card = db.pos_item.First(x => x.ITEMID == codigo_giftcard);
                    if (db.pos_itembarra.Any(x => x.ITEMBARCODE == codigobarra_giftcard))
                    {
                        var r = db.pos_itembarra.First(x => x.ITEMBARCODE == codigobarra_giftcard);
                        gift_card = db.pos_item.First(x => x.ITEMID == r.ITEMID);
                    }
                    POS.Control.TarjetaRegalo t = new Control.TarjetaRegalo();

                    //if (t.getTarjeta(codigo)) if (t.getTarjetaGen(pGC.Codigo, "", false))
                    if (t.getTarjetaGen(codigo, "", false))
                    {
                        if (t.tarjetaNoActivada())
                        {
                            this.TarjetasRegalo.Add(t);
                            setProductoData(gift_card, factura, cliente);
                            this.Nombre = this.Nombre + " - " + t.getCodigo();
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Asigna el mayor mayor descuento
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="descuento"></param>
        private void setDescuento(core_descuento desc, ref decimal descuento)
        {
            var hoy = DateTime.Now;
            if (desc.fecha_desde.HasValue && desc.fecha_hasta.HasValue)
            {
                if (hoy >= desc.fecha_desde.Value && hoy <= desc.fecha_hasta.Value)
                {
                    if (desc.valor > descuento)
                    {
                        descuento = desc.valor;
                    }
                }
            }
            else
            {
                if (desc.valor > descuento)
                {
                    descuento = desc.valor;
                }
            }
        }

        /// <summary>
        /// Carga a la linea de detalle una coleccion de descuentos
        /// Al mismo tiempo devuelve el descuento que mas le beneficia al cliente
        /// </summary>
        /// <param name="factura"></param>
        /// <param name="item"></param>
        /// <param name="producto"></param>
        /// <returns>Descuento que mas beneficia al cliente!</returns>
        public decimal getDescuento(Factura factura, pos_customer cliente, ref decimal refPorcEmDivision)
        {

            var descuento = 0M;
            var descuentos_todos = new List<core_descuento>();
            this.Descuentos = new List<Descuento>();
            bool flag_desc = false;
            //Reset de bandera DescProductoCliente
            this.TieneDescuentoProductoCliente = false;

            if (factura.Documento == "F")
            {
                var db = new POSEntities();

                var descuentoClienteProducto = db.core_descuento.Where(z => z.activo && z.tipo_descuento == DESCUENTO_PRODUCTO_CLIENTE && z.parametro == this.Id && z.parametro2 == cliente.ACCOUNTNUM).ToList();

                foreach (var desc2 in descuentoClienteProducto)
                {
                    if (desc2.rango_fecha)
                    {
                        if (desc2.fecha_desde <= DateTime.Now && desc2.fecha_hasta >= DateTime.Now)
                        {
                            //this.DescuentoLocalProducto = desc2.valor / 100;
                            this.DescuentoProductoCliente= desc2.valor / 100;
                            this.TieneDescuentoProductoCliente = true;
                            flag_desc = true;
                        }
                    }
                    else
                    {
                        //this.DescuentoLocalProducto = desc2.valor / 100;
                        this.DescuentoProductoCliente = desc2.valor / 100;
                        this.TieneDescuentoProductoCliente = true;
                        flag_desc = true;
                    }
                }

                if (flag_desc == false)
                {
                    descuentos_todos = db.core_descuento.Where(x => x.activo).ToList();


                    foreach (var desc in descuentos_todos)
                    {


                        if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO)
                        {
                            if (factura.Establecimiento == desc.parametro)
                            {
                                //this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = false });
                                //setDescuento(desc, ref descuento);
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                    {
                                        this.DescuentoLocal = desc.valor / 100;
                                    }
                                }
                                else
                                {
                                    this.DescuentoLocal = desc.valor / 100;
                                }

                            }
                        }

                        /* if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_CATEGORIA)
                         {
                             if (factura.Establecimiento == desc.parametro && this.Grupo == desc.parametro2)
                             {
                                 if (desc.rango_fecha)
                                 {
                                     if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                     {
                                         this.DescuentoLocalProducto = desc.valor / 100;
                                     }
                                 }
                                 else
                                 {
                                     this.DescuentoLocalProducto = desc.valor / 100;
                                 }
                             }
                         }*/

                        //if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_CATEGORIA)
                        //{
                        //    if (factura.Establecimiento == desc.parametro && this.Grupo == desc.parametro2)
                        //    {
                        //        if (desc.rango_fecha)
                        //        {
                        //            if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                        //            {
                        //                this.DescuentoLocalConjunto = desc.valor / 100;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            this.DescuentoLocalConjunto = desc.valor / 100;
                        //        }
                        //    }
                        //}

                        if (desc.tipo_descuento == DESCUENTO_DIVISION)
                        {



                            if ((cliente.CUSTGROUP == desc.parametro || factura.EsTarjetaCreditoInternoAdicional) && this.Grupo == desc.parametro2) //13-07-2015 Se implementa descuento por grupo de articulos
                            {
                                //ML: Solo se dará dsctos division a empleado si este usa la tarjeta empleado como forma de pago
                                //Se guarda el porcentaje de dscto para calcular el dscto para calcular cuando los criterios se hayan cumplido

                                if (!((cliente.CUSTGROUP == "EM" || factura.EsTarjetaCreditoInternoAdicional) && !(Control.POS.HasPayType("TAR PORTAL"))))
                                {
                                    this.Descuentos.Add(new Descuento()
                                    {
                                        codigo = desc.tipo_descuento,
                                        valor = desc.valor,
                                        parametro = desc.parametro,
                                        parametro2 = desc.parametro2,
                                        especial = false
                                    });

                                    setDescuento(desc, ref descuento);

                                    //En caso de ser nuevo producto y cliente es empleado y existir un pago tar portal, activar bandera de que tiene el descuento por division
                                    if (cliente.CUSTGROUP == "EM" || factura.EsTarjetaCreditoInternoAdicional) this.ActivadoPorcDctoDivisionEmpleado = true;
                                }

                                this.PorcDescuentoDivisionEmpleado = (cliente.CUSTGROUP == "EM" || factura.EsTarjetaCreditoInternoAdicional) ? desc.valor : 0;
                                refPorcEmDivision = PorcDescuentoDivisionEmpleado;
                            }




                            //if ((cliente.CUSTGROUP == desc.parametro || factura.TarjetaCreditoInternoAdicional != null) && this.Grupo == desc.parametro2) //13-07-2015 Se implementa descuento por grupo de articulos
                            //{
                            //    //ML: Solo se dará dsctos division a empleado si este usa la tarjeta empleado como forma de pago
                            //    //Se guarda el porcentaje de dscto para calcular el dscto para calcular cuando los criterios se hayan cumplido

                            //    if (!((cliente.CUSTGROUP == "EM" || factura.TarjetaCreditoInternoAdicional != null) && !(Control.POS.HasPayType("TAR PORTAL"))))
                            //    {
                            //        this.Descuentos.Add(new Descuento() {
                            //            codigo = desc.tipo_descuento
                            //            , valor = desc.valor
                            //            , parametro = desc.parametro
                            //            , parametro2 = desc.parametro2
                            //            , especial = false
                            //        });

                            //        setDescuento(desc, ref descuento);

                            //        //En caso de ser nuevo producto y cliente es empleado y existir un pago tar portal, activar bandera de que tiene el descuento por division
                            //        if (cliente.CUSTGROUP == "EM" || factura.TarjetaCreditoInternoAdicional != null) this.ActivadoPorcDctoDivisionEmpleado = true;
                            //    }

                            //    this.PorcDescuentoDivisionEmpleado = (cliente.CUSTGROUP == "EM" || factura.TarjetaCreditoInternoAdicional != null) ? desc.valor : 0;
                            //    refPorcEmDivision = PorcDescuentoDivisionEmpleado;
                            //}



                        }


                        if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_PRODUCTOS)
                        {

                            if (factura.Establecimiento == desc.parametro && this.Id == desc.parametro2/* && (cliente.CUSTGROUP == desc.canal || desc.canal == null)*/) //Se agrega el campo canal
                            {
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                    {
                                        this.DescuentoEstablecimientoProductos = desc.valor / 100;
                                    }
                                }
                                else
                                {
                                    this.DescuentoEstablecimientoProductos = desc.valor / 100;
                                }
                            }
                        }

                        if (desc.tipo_descuento == DESCUENTO_CLIENTE)
                        {
                            if (cliente.ACCOUNTNUM == desc.parametro)
                            {
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_hasta >= DateTime.Now && desc.fecha_desde <= DateTime.Now)
                                    {
                                        this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = false });
                                        setDescuento(desc, ref descuento);
                                    }
                                }
                                else
                                {
                                    this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = false });
                                    setDescuento(desc, ref descuento);

                                }
                            }
                        }


                        if (desc.tipo_descuento == DESCUENTO_CLIENTECATEGORIA)
                        {
                            if (cliente.ACCOUNTNUM == desc.parametro)
                            {
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_hasta >= DateTime.Now && desc.fecha_desde <= DateTime.Now)
                                    {
                                        if (desc.parametro2.Trim() == this.Categoria.Trim())
                                        {
                                            this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = false });
                                            descuento = desc.valor;
                                        }
                                    }
                                }
                                else
                                {
                                    if (desc.parametro2.Trim() == this.Categoria.Trim())
                                    {
                                        this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = false });
                                        descuento = desc.valor;

                                    }
                                }
                            }
                        }

                        /*MODIFICACION ESTABLECIMIENTO VARIEDAD */
                        if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_VARIEDAD)
                        {
                            if (this.Variedad != null)
                            {
                                this.Variedad = this.Variedad.TrimEnd();
                                if (factura.Establecimiento == desc.parametro && this.Variedad == desc.parametro2)
                                {
                                    if (desc.rango_fecha)
                                    {
                                        if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                        {
                                            this.DescuentoEstablecimientovariedad = desc.valor / 100;
                                        }
                                    }
                                    else
                                    {
                                        this.DescuentoEstablecimientovariedad = desc.valor / 100;
                                    }

                                }
                            }
                        }
                        /*MODIFICACION ESTABLECIMIENTO SUBGRUPO */

                        if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_SUB_GRUPO)
                        {
                            if (this.SubGrupo != null)
                            {
                                this.SubGrupo = this.SubGrupo.TrimEnd();
                                if (factura.Establecimiento == desc.parametro && this.SubGrupo == desc.parametro2)
                                {
                                    string textoMsj = this.SubGrupo + " " + desc.parametro2;                                   
                                    Control.Common.General.GetMensaje("POS - Promociones", textoMsj, "I");

                                    // MessageBox.Show(this.SubGrupo + " " + desc.parametro2);

                                    if (desc.rango_fecha)
                                    {
                                        if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                        {
                                            //this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = false });
                                            //descuento = desc.valor;
                                            this.DescuentoEstablecimientosubgrupo = desc.valor / 100;
                                        }
                                    }
                                    else
                                    {
                                        this.DescuentoEstablecimientosubgrupo = desc.valor / 100;
                                    }
                                }
                            }
                        }
                        ///*MODIFICACION ESTABLECIMIENTO GRUPO */
                        if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_GRUPO)
                        {
                            if (this.GrupoN != null)
                            {
                                this.GrupoN = this.GrupoN.TrimEnd();
                                if (factura.Establecimiento == desc.parametro && this.GrupoN == desc.parametro2)
                                {
                                    if (desc.rango_fecha)
                                    {
                                        if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                        {
                                            //this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = false });
                                            //descuento = desc.valor;
                                            this.DescuentoEstablecimientogrupo = desc.valor / 100;
                                        }
                                    }
                                    else
                                    {
                                        this.DescuentoEstablecimientogrupo = desc.valor / 100;
                                    }
                                }
                            }
                        }

                        ///*MODIFICACION ESTABLECIMIENTO CATEGORIA */
                        if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_CATEGORIA)
                        {
                            /* if (this.Categoria != null )
                             {*/
                            this.Categoria = this.Categoria.TrimEnd();
                            if (factura.Establecimiento == desc.parametro && this.Categoria == desc.parametro2)
                            {
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                    {
                                        this.DescuentoEstablecimientocategoria = desc.valor / 100;
                                    }
                                }
                                else
                                {
                                    this.DescuentoEstablecimientocategoria = desc.valor / 100;
                                }
                            }
                            // }
                        }

                        ///*MODIFICACION ESTABLECIMIENTO PRODUCTO */
                        /*if (desc.tipo_descuento == DESCUENTO_ESTABLECIMIENTO_PRODUCTOS)
                        {
                            if (this.Categoria != null)
                            {
                                this.Categoria = this.Categoria.TrimEnd();
                                if (factura.Establecimiento == desc.parametro && this.Categoria == desc.parametro2)
                                {
                                    if (desc.rango_fecha)
                                    {
                                        if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                        {
                                            this.DescuentoEstablecimientoProductos = desc.valor / 100;
                                        }
                                    }
                                    else
                                    {
                                        this.DescuentoEstablecimientoProductos = desc.valor / 100;
                                    }
                                }
                            }
                        }*/



                        if (desc.tipo_descuento == DESCUENTO_PRODUCTO)
                        {
                            if (this.Id == desc.parametro)
                            {
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_desde <= DateTime.Now && desc.fecha_hasta >= DateTime.Now)
                                    {
                                        //this.DescuentoLocalProducto = desc.valor > 0 ? desc.valor / 100 : 0M;
                                        this.DescuentoLocalProducto = desc.valor / 100;
                                    }
                                }
                                else
                                {
                                    //this.DescuentoLocalProducto = desc.valor > 0 ? desc.valor / 100 : 0M;
                                    this.DescuentoLocalProducto = desc.valor / 100;
                                }
                            }
                        }

                        if ((desc.tipo_descuento == DESCUENTO_PRODUCTO_CANTIDAD && (desc.parametro2 == factura.Establecimiento || desc.parametro2 == null)) 
                            || desc.tipo_descuento == DESCUENTO_PRODUCTO_GRUPO_CANTIDAD)
                        {

                            if (this.Grupo == desc.parametro || this.Id == desc.parametro)
                            {
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_hasta >= DateTime.Now && desc.fecha_desde <= DateTime.Now)
                                    {
                                        // Si es por parqueo, y el ingreso del vehiculo esta dentro de rango configurado.  JM  19-03-2019
                                        if (factura.ObjParking != null && Control.Common.GlobalParameters.Parking_TienePermiso)
                                        {
                                            if (factura.ObjParking.EstaConfirmado)
                                            {
                                                if (factura.ObjParking.FechaIngreso >= desc.fecha_desde)
                                                {
                                                    ///Aplica el descuento
                                                    this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = true });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ///Aplica el descuento
                                            this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor
                                                , parametro = desc.parametro, parametro2 = desc.parametro2, especial = true });
                                        }
                                    }
                                }
                                else
                                {
                                    this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = true });
                                }
                            }
                        }


                        if ((desc.tipo_descuento == DESCUENTO_PRODUCTO_CANTIDAD_PESO 
                            && (desc.parametro2 == factura.Establecimiento || desc.parametro2 == null)) || 
                            desc.tipo_descuento == DESCUENTO_PRODUCTO_GRUPO_CANTIDAD)
                        {

                            if (this.Grupo == desc.parametro || this.Id == desc.parametro)
                            {
                                if (desc.rango_fecha)
                                {
                                    if (desc.fecha_hasta >= DateTime.Now && desc.fecha_desde <= DateTime.Now)
                                        this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = true });
                                }
                                else
                                {
                                    this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = desc.parametro2, especial = true });
                                }
                            }
                        }


                        if (desc.tipo_descuento == DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD)
                        {
                            // Tipo descuento (ProductoCategoriaCantidad) parametro2 permitir identificar producto por libra o unidad.      JM   10-12-2020 
                            if (factura.Establecimiento == desc.parametro2 || desc.parametro2 == null || (desc.parametro2.ToUpper() == "LB" || desc.parametro2.ToUpper() == "UND"))
                            {
                                if (this.Categoria.Trim() == desc.parametro || this.Id == desc.parametro)
                                {
                                    if (desc.rango_fecha)
                                    {
                                        if (desc.fecha_hasta >= DateTime.Now && desc.fecha_desde <= DateTime.Now)
                                            this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = (desc.parametro2 != null ? desc.parametro2.ToUpper() : desc.parametro2), especial = true });
                                    }
                                    else
                                    {
                                        this.Descuentos.Add(new Descuento() { codigo = desc.tipo_descuento, valor = desc.valor, parametro = desc.parametro, parametro2 = (desc.parametro2 != null ? desc.parametro2.ToUpper() : desc.parametro2), especial = true });
                                    }
                                }
                            }
                        }

                    }
                }
            }
            /*if (descuento > 0)
            {
                MessageBox.Show(descuento.ToString());
            }*/
            return descuento;


        }

        /// <summary>
        /// Reemplaza los descuentos normales
        /// </summary>
        /// <returns>Verdadero si se aplico un descuento especial</returns>
        bool aplicarDescuentoEspecial()
        {
            //Se declara una variable porque solo se puede aplicar un descuento el mayor
            Descuento descuento_especial = null;
            //Revisamos por estos 2 tipos de descuentos
            //Se usa el campo valor como parametro de division contra cantidad el resultado es la cantidad que regala!
            if (this.Descuentos.Any(x => x.codigo == "ProductoCantidad" || x.codigo == "ProductoGrupoCantidad" || x.codigo == "ProductoCategoriaCantidad" || x.codigo == DESCUENTO_PRODUCTO_CANTIDAD_PESO))
            {
                //Como hay 2 categorias de descuento producto cantidad se verifica cual es el mayor de los 2
                var descuento_producto_cantidad = this.Descuentos.FirstOrDefault(x => x.codigo == "ProductoCantidad" || x.codigo == "ProductoCategoriaCantidad" || x.codigo == DESCUENTO_PRODUCTO_CANTIDAD_PESO || (x.codigo == DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD && x.parametro.Length > 3));
                var descuento_producto_grupo_cantidad = this.Descuentos.FirstOrDefault(x => x.codigo == "ProductoGrupoCantidad");
                if (descuento_producto_cantidad != null && descuento_producto_grupo_cantidad != null)
                {
                    if (descuento_producto_cantidad.valor > descuento_producto_grupo_cantidad.valor)
                    {
                        descuento_especial = descuento_producto_cantidad;
                    }
                    else
                    {
                        descuento_especial = descuento_producto_grupo_cantidad;
                    }
                }
                else
                {
                    descuento_especial = descuento_producto_cantidad != null ? descuento_producto_cantidad : descuento_producto_grupo_cantidad;
                }
                /*
                if(this.Descuentos.Any(x => x.codigo == DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD && x.parametro.Length > 3)
                {
                    descuento_especial=
                }*/
                if (this._cantidad >= descuento_especial.valor)
                {
                    return calcularDescuentoEspecial(descuento_especial);
                }
            }
            return false;
        }

        /// <summary>
        /// Realiza el calculo para regalar producto como descuento
        /// </summary>
        /// <param name="descuento_especial">Descuento Especial</param>
        /// <returns></returns>
        //private bool calcularDescuentoEspecial(Descuento descuento_especial)
        //{
        //    //Cuanta cantidad o peso se va a descontar

        //    // Para promocion de productos por libra toma las unidades.   Se reemplaza this.Cantidad por cantidadAplica .       JM   12-10-2020-10
        //    //INICIO JM
        //    //  var cantidadAplica = descuento_especial.parametro2 == "LB" ? this.Unidades : this.Cantidad;

        //     // var resultado = getCalculoEspecialRegalar(cantidadAplica, descuento_especial.valor);
        //    //FIN JM
        //    /*
        //   ABC CAMBIO INICIO

        //   */
        //    var cantidadAplica = descuento_especial.parametro2 == "LB" ? this.Unidades : this.Cantidad;
        //    var resultado = getCalculoEspecialRegalar(cantidadAplica, descuento_especial.valor);

        //    descuento_especial.valor = descuento_especial.parametro2 == "LB" ? this.Cantidad / this.Unidades : descuento_especial.valor;
        //    resultado = Convert.ToInt32((descuento_especial.parametro2 == "LB" ? this.Cantidad / this.Unidades : descuento_especial.valor));
        //    /*
        //     ABC CAMBIO FIN
        //     */

        //    /*  if (descuento_especial.codigo== DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD)
        //      {
        //          resultado = (int)descuento_especial.valor;
        //      }*/
        //    if (resultado > 0)
        //    {
        //        //Se necesita buscar el descuento normal del producto
        //        var valor_con_descuento_no_especial = this._pvp - this._pvp * (getMayorDescuento() / 100M);
        //        //Valor menos los descuentos normales

        //        var valor_descuento = Math.Round(resultado * this._pvp, 2);
        //        //Diferencia de cantidad que no se esta regalando para calcular el correcto descuento
        //        var restantes = (cantidadAplica - resultado) - (resultado * descuento_especial.valor);
        //        if (descuento_especial.codigo == DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD)
        //        {
        //            if (cantidadAplica >= 2)
        //            {
        //                valor_descuento = Math.Round(((cantidadAplica - 1) / descuento_especial.valor) * this._pvp, 2);
        //                //valor_descuento = Math.Round((this._cantidad-1/descuento_especial.valor) * this._pvp, 2);
        //            }
        //            if (descuento_especial.parametro.Length>3)
        //            {
        //                decimal cantAplica2 = this.Cantidad;
        //                if (descuento_especial.parametro2 == "LB")
        //                {
        //                    decimal cantMod = cantidadAplica/ 2;
        //                    cantAplica2 = (Math.Truncate(this.Cantidad / cantidadAplica)* cantMod) / 2; // devolver la mitad aplicar
        //                    valor_descuento = Math.Round((cantAplica2 / descuento_especial.valor) * this._pvp, 2);
        //                }
        //                else                         
        //                    valor_descuento = Math.Round((Math.Truncate(this.Cantidad / 2) / descuento_especial.valor) * this._pvp, 2);
        //            }
        //        }
        //        if (descuento_especial.codigo == DESCUENTO_PRODUCTO_CANTIDAD_PESO)
        //        {
        //            if (this.Cantidad >= 2)
        //            {
        //                valor_descuento = Math.Round(((this.Cantidad) / descuento_especial.valor) * this._pvp, 2);
        //                //valor_descuento = Math.Round((this._cantidad-1/descuento_especial.valor) * this._pvp, 2);
        //            }

        //        }

        //            if (restantes < 0)
        //            restantes = 0;
        //        //Se suma el descuento que regala el producto mas el descuento que no se regala.
        //        this.Descuento = Math.Round(valor_descuento + (restantes * this._pvp * (getMayorDescuento() / 100M)), 2);
        //        //@descuento_porcentaje=round(@valor_descuento*100/@valor_pago,2)
        //        this._descuentoActual = Math.Round((this._descuento * 100) / valor_descuento, 2, MidpointRounding.AwayFromZero);
        //        return true;
        //    }
        //    return false;
        //}
        private bool calcularDescuentoEspecial(Descuento descuento_especial)
        {
            //Cuanta cantidad o peso se va a descontar
            var resultado = getCalculoEspecialRegalar(this.Cantidad, descuento_especial.valor);

            /*
             ABC CAMBIO INICIO
             
             */
            var cantidadAplica = descuento_especial.parametro2 == "LB" ? this.Unidades : this.Cantidad;
            resultado = getCalculoEspecialRegalar(cantidadAplica, descuento_especial.valor);


            /*
             ABC CAMBIO FIN
             */


            /*  if (descuento_especial.codigo== DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD)
              {
                  resultado = (int)descuento_especial.valor;
              }*/
            if (resultado > 0)
            {
                //descuento_especial.valor = descuento_especial.parametro2 == "LB" ? this.Cantidad / this.Unidades : descuento_especial.valor;
                //resultado = Convert.ToInt32((descuento_especial.parametro2 == "LB" ? this.Cantidad / this.Unidades : descuento_especial.valor));
                //Se necesita buscar el descuento normal del producto
                var valor_con_descuento_no_especial = this._pvp - this._pvp * (getMayorDescuento() / 100M);
                //Valor menos los descuentos normales

                var valor_descuento = Math.Round(resultado * this._pvp, 2);  //Jcanarte 30dic2020 colocar linea como version anterior.
                //Diferencia de cantidad que no se esta regalando para calcular el correcto descuento
                var restantes = (this._cantidad - resultado) - (resultado * descuento_especial.valor);

                decimal porDescuento = 0.50m;

                if (descuento_especial.codigo == DESCUENTO_PRODUCTO_CATEGORIA_CANTIDAD)
                {
                    valor_descuento = 0.0m;

                    if (this.Unidades % descuento_especial.valor == 0 && this.Unidades == descuento_especial.valor)
                    {
                        valor_descuento = Math.Round(((this.PrecioLocal * this.Cantidad) / descuento_especial.valor) * (porDescuento / ((this.Unidades == 2) ? 1 : 2)), 2);
                    }
                    //if (this._cantidad >= 2)
                    //{
                    //    valor_descuento = Math.Round(((this._cantidad - 1) / descuento_especial.valor) * this._pvp, 2);

                    //    //=SI(ES.PAR(C4);((B4*D4)/2)*N$2/ (SI(C4=2;1;2));0)

                    //    //valor_descuento = Math.Round((this._cantidad-1/descuento_especial.valor) * this._pvp, 2);
                    //}
                    else if (this.Unidades > descuento_especial.valor)
                    {
                        var auxvalor_descuento = valor_descuento;
                        //  this.Descuento
                        var auxDescuento = this._descuento;
                        var auxDescuentoActual = this._descuentoActual;
                        //c5 unidad
                        //b5 cantidad
                        //d5 precio
                        //n2 % descuento 0.5
                        //
                        //=SI(ES.PAR(C5);((B5*D5))*N$2/ (SI(C5=2;1;2));0)
                        if (this.Unidades % descuento_especial.valor == 0)
                            valor_descuento = Math.Round(((this.PrecioLocal * this.Cantidad)) * (porDescuento / ((this.Unidades == 2) ? 1 : 2)), 2);
                        else
                        {
                            // valor_descuento = Math.Round(((this.PrecioLocal * this.Cantidad)) * (porDescuento / ((this.Unidades == 2) ? 1 : 2)), 2);
                            this.Descuento = this.DescuentoAnterior;
                            this._descuentoActual = this.DescuentoActualAnterior;
                            valor_descuento = this.DescuentoAnterior;

                        }
                    }
                    //if (descuento_especial.parametro.Length > 3)
                    //{
                    //    valor_descuento = Math.Round((Math.Truncate(this._cantidad / 2) / descuento_especial.valor) * this._pvp, 2);
                    //}
                }


                if (descuento_especial.codigo == DESCUENTO_PRODUCTO_CANTIDAD_PESO)
                {
                    if (this._cantidad >= 2)
                    {
                        valor_descuento = Math.Round(((this._cantidad) / descuento_especial.valor) * this._pvp, 2);
                        //valor_descuento = Math.Round((this._cantidad-1/descuento_especial.valor) * this._pvp, 2);
                    }

                }

                if (restantes < 0)
                    restantes = 0;
                //Se suma el descuento que regala el producto mas el descuento que no se regala.
                this.Descuento = Math.Round(valor_descuento + (restantes * this._pvp * (getMayorDescuento() / 100M)), 2);
                //@descuento_porcentaje=round(@valor_descuento*100/@valor_pago,2)
                this._descuentoActual = Math.Round((this._descuento * 100) / valor_descuento, 2, MidpointRounding.AwayFromZero);

                this.DescuentoActualAnterior = this._descuentoActual;
                this.DescuentoAnterior = this.Descuento;

                return true;
            }
            return false;
        }
        /// <summary>
        /// Recibe cantidad y el parametro por cuantas unidades se va a regalar una.
        /// Inicia in bucle que cuenta cuantas unidades se tienen que descontar.
        /// </summary>
        /// <param name="cantidad">Cantidad/Peso de productos</param>
        /// <param name="valor">Parametro de Decuento</param>
        /// <returns></returns>
        private int getCalculoEspecialRegalar(decimal cantidad, decimal valor)
        {
            var result = 0;
            var i = 1;
            cantidad = Math.Truncate(cantidad);
            while (cantidad > 0)
            {
                //Verifica si el actual se va a descontar
                if (i == valor)
                {
                    i = 1;
                    //Suma uno a los descuentos
                    result++;
                }
                else
                {
                    //Sigue contando
                    i++;
                }
                //Resta cantidad ya contada
                cantidad--;
            }
            return result;
        }
        /// <summary>
        /// Devuelve el mayor descuento
        /// </summary>
        /// <returns></returns>
        public decimal getMayorDescuento()
        {
            return this.Descuentos.Where(x => !x.especial).Count() > 0 ? this.Descuentos.Where(x => !x.especial).Select(x => x.valor).Max() : 0M;
        }

        /// <summary>
        /// Agrega una cantidad adicional, verifica si es por unidad o peso.
        /// </summary>
        /// <param name="cantidad">Solo si es Peso se asigna</param>
        public void agregarAdicional(decimal cantidad = 0M) //SE ASIGNA UNA CANTIDAD ADICIONAL A LA EXISTENTE
        {
            if (this.Unidad.ToUpper() == "UND")
            {
                this._cantidad = cantidad;
                this._unidades = (int)cantidad;
                this._cantidad++;
                this.Unidades++;
                this._cantidadINEC = this._cantidad;
            }
            else
            {
                this._cantidad = this._cantidad + decimal.Round(cantidad, 2);
                this._cantidadINEC += cantidad;
                this.Unidades++;
            }


        }
        /// <summary>
        /// Calcula datos de descuento
        /// </summary>
        public void update(bool debeCalcularDescuento = true)
        {
            if (this.Itemtype != 2)
            {
                /*  ML: 2018-Jul-05
                    Descuento por conjunto de cliente (tipo: Division) pasa a ser primario, y el resto de descuentos se debe aplicar 
                    sobre el resultante de: subtotal menos el descuento obtenido por conjunto de clientes   

                decimal porcDsctoDivision = 0;
                if (Descuentos != null) if (Descuentos.Count > 0) porcDsctoDivision = (Descuentos.Where(x => x.codigo == "Division").Sum(x => x.valor) / 100);
                if (porcDsctoDivision > 0)
                {
                    decimal subtotalConDsctoDivision = this.SubtotalSinDescuento;
                    subtotalConDsctoDivision -= Math.Round(this.SubtotalSinDescuento * porcDsctoDivision, 2);

                    //Obtener el porcentaje otorgado realizando calculo inverso contra el subtotal del producto
                    decimal porcOtorgado = this.DescuentoAX / this.SubtotalSinDescuento;

                    decimal descRecalculado = Math.Round(subtotalConDsctoDivision * porcOtorgado, 2);
                    this.DescuentoAX = descRecalculado;
                } */

                if (!aplicarDescuentoEspecial())
                {
                    //Cuando se requiera presetear variables DescuentoActual y Descuento, enviar false para que la siguiente accion no sea realizada
                    if (debeCalcularDescuento) this.Descuento = Math.Round(Math.Round(this._pvp * this._cantidad, 2) * _descuentoActual, 2);
                }
                /*  if(esAjustado)
                  {
                      this.Subtotal = this.Subtotal + this.Ajuste;
                  }*/

                this.Descuento = this.Descuento + this.DescuentoAX;
                //Los descuentos de un producto no pueden ser mayor al subtotal
                if (this.Descuento > this.SubtotalSinDescuento)
                {
                    this.Descuento = this.SubtotalSinDescuento;
                    //Porcentaje descuento pasa a 1 (representa el 100%)
                    _descuentoActual = 1;
                }
                else if (this.Descuento < 0)
                {
                    this.Descuento = 0;
                    //Porcentaje descuento pasa a 0%
                    _descuentoActual = 0;
                }
                else
                {
                    //Calcular el porcentaje de descuento actual del producto
                    if (SubtotalSinDescuento > 0)
                        _descuentoActual = Math.Round(this.Descuento / SubtotalSinDescuento, 2);
                    else
                        _descuentoActual = 0;
                }

                //Encerar el valor DescuentoAX una vez usado
                this.DescuentoAX = 0M;

                this.Iva = decimal.Round(this.Subtotal * _ivaProducto, 2, MidpointRounding.AwayFromZero); //Opozo descuentos cambio de redondeo
            }
        }

        public bool FillProductSalesInfo(string itemId)
        {
            bool respuesta = true;
            try
            {
                System.Data.SqlClient.SqlConnection conexion = new System.Data.SqlClient.SqlConnection(Properties.Settings.Default.CONECTA_AX);
                string Query = null;
                System.Data.SqlClient.SqlCommand comando = default(System.Data.SqlClient.SqlCommand);
                using (conexion)
                {
                    conexion.Open();
                    Query = "select ItemName, Grupo, SubGrupo, Variedad, Categoria, PrimaryVendorId from InventTable with (nolock) " +
                            "where DataAreaId = 'liri' and ItemId = '" + itemId + "' ";

                    comando = new System.Data.SqlClient.SqlCommand(Query, conexion);
                    System.Data.SqlClient.SqlDataReader dr = comando.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();

                        //this.Id = itemId;
                        //this.Nombre = dr.GetValue(0).ToString();
                        this.Grupo = dr.GetValue(1).ToString();
                        this.SubGrupo = dr.GetValue(2).ToString();
                        this.Variedad = dr.GetValue(3).ToString();
                        this.Categoria = dr.GetValue(4).ToString();
                        this.ProveedorPricipal = dr.GetValue(5).ToString();

                    }
                    conexion.Close();
                }
            }
            catch (Exception)
            {
                respuesta = false;
            }

            return respuesta;
        }

        public void recalcularDescuentoPromocionAX(List<Promocion> _listProductosDescuentos, string clienteIdentificacion, Factura _factura)
        {
            decimal descuent = 0;

            if (_listProductosDescuentos != null)
            {
                _listProductosDescuentos = _listProductosDescuentos.Where(d => d.Tipo == 2 || d.Tipo == 8).ToList();
                decimal descuento_a_aplicar = 0;

                for (int j = 0; j < _factura.Productos.Count; j++)
                {
                    //recorrer lista de promociones activas de almacen
                    for (int i = 0; i < _listProductosDescuentos.Count; i++)
                    {
                        Promocion promo = _listProductosDescuentos[i];

                        //recorrer productos en promocion por cantidad
                        for (int k = 0; k < promo.ListProductos.Count; k++)
                        {
                            decimal prod_cantidad = _factura.Productos[j].Cantidad;
                            decimal prod_precio = _factura.Productos[j].Pvp;
                            string prod_id = _factura.Productos[j].Id;

                            decimal promo_cant = promo.ListProductos[k].Cantidad;
                            decimal promo_descuento = promo.ListProductos[k].Descuento;
                            string promo_id = promo.ListProductos[k].Id;
                            int promo_tipo = _listProductosDescuentos[i].Tipo;

                            if (promo_id == prod_id)
                            {
                                if (promo_descuento > descuento_a_aplicar)
                                {
                                    if (promo_cant <= prod_cantidad)
                                    {
                                        if (!promo.EsRestrictiva
                                            ||
                                            (promo.EsRestrictiva && (ValidarIdentificador.ValidarCedula(clienteIdentificacion) || ValidarIdentificador.ValidarRUCNatural(clienteIdentificacion) || ValidarIdentificador.ValidarPasaporte(clienteIdentificacion))))
                                        {
                                            bool puedeRealizarDscto = true;
                                            decimal cantidadComprada = 0M;
                                            decimal cantidadComprando = 0M;

                                            if (promo.EsRestrictiva)
                                            {
                                                DateTime fecha = DateTime.Now;
                                                try
                                                {
                                                    using (POSEntities db = new POSEntities())
                                                    {
                                                        cantidadComprada = db.core_facturadetalle
                                                                                    .Join(db.core_factura,
                                                                                        det => det.factura_id,
                                                                                        fac => fac.id,
                                                                                        (det, fac) => new { det, fac })
                                                                                    .Where(x => (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.fac.fecha_creacion) == (DateTime)System.Data.Entity.DbFunctions.TruncateTime(fecha.Date)
                                                                                                &&
                                                                                                x.fac.cliente == clienteIdentificacion
                                                                                                &&
                                                                                                x.det.item_id == prod_id
                                                                                                &&
                                                                                                x.det.descuento > 0)
                                                                                    .Sum(x => (decimal?)x.det.cantidad) ?? 0;

                                                        puedeRealizarDscto = (cantidadComprada < promo.MaxCantidadDscto);

                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Producto", "actualizarDescuentoPromocionAX", "No fue posible identificar cuantas unidades de '" + prod_id + "' ha comprado previamente el cliente '" + clienteIdentificacion + "' el dìa de hoy para hacer la evaluacion vs la promo restrictiva '" + promo.Descripcion + "' (" + promo.RecId.ToString() + "), a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                                }
                                            }

                                            //Evitar lineas de codigo en caso de que se prevea que no habrá dscto
                                            if (puedeRealizarDscto)
                                            {
                                                if (promo_tipo == 8)//Promocion por combo cerrado
                                                {
                                                    decimal promo_multiplica = prod_cantidad / promo_cant;
                                                    long valorSinDecimal = (long)promo_multiplica;
                                                    decimal decimales = promo_multiplica - (decimal)valorSinDecimal;
                                                    decimal CantidadSinDescuento = decimales * promo_cant;

                                                    //Disposicion presidencia: Tener un control para tope de cantidad que llevara descuento
                                                    decimal cantidadParaDscto = _factura.Productos[j]._cantidad - CantidadSinDescuento;

                                                    if (promo.EsRestrictiva && promo.GeneralPromo)  // Nueva Promocion por el detalle de la factura.  22-02-2019
                                                    {

                                                        _factura.Productos.Where(x => x.Id == promo_id).Sum(x => x.Cantidad);
                                                        var cantcompradscto = (from a in _factura.Productos
                                                                               join b in promo.ListProductos on a.Id equals b.Id
                                                                               group a by new { b.Grupo } into g
                                                                               select new
                                                                               {
                                                                                   grupo = g.Key.Grupo,
                                                                                   cantidad = g.Sum(x => x.Cantidad)
                                                                               });

                                                        if (cantcompradscto.Count() > 0)
                                                            cantidadComprando = cantcompradscto.FirstOrDefault().cantidad;

                                                        if (!_factura.Productos.Any(x => x.Id == promo_id))
                                                        {
                                                            cantidadComprando += _factura.Productos[j].Cantidad;
                                                            if (cantidadComprando > promo.MaxCantidadDscto)
                                                            {
                                                                cantidadParaDscto = 0;
                                                                cantidadComprando = 0;
                                                            }
                                                        }

                                                        if (cantidadComprando > promo.MaxCantidadDscto)
                                                        {
                                                            decimal cantdscto = cantidadComprando - promo.MaxCantidadDscto;
                                                            cantidadParaDscto = cantidadParaDscto - cantdscto;
                                                        }
                                                    }

                                                    //De los comprados, inferir cuantas unidades fueron combo (SOLO CUANDO PROMO ES TIPO COMBO)
                                                    cantidadComprada = ((long)(cantidadComprada / promo_cant)) * promo_cant;

                                                    if (cantidadParaDscto > (promo.MaxCantidadDscto - cantidadComprada))
                                                    {
                                                        cantidadParaDscto = (promo.MaxCantidadDscto - cantidadComprada);
                                                    }
                                                    if (cantidadParaDscto < 0) cantidadParaDscto = 0;

                                                    descuent = promo_descuento;
                                                    _factura.Productos[j].DescuentoAX = (descuent / 100M) * Math.Round(_factura.Productos[j]._pvp * cantidadParaDscto, 2, MidpointRounding.AwayFromZero);
                                                }
                                                else
                                                {
                                                    decimal cantidadParaDscto = _factura.Productos[j]._cantidad;
                                                    if (cantidadParaDscto > (promo.MaxCantidadDscto - cantidadComprada))
                                                    {
                                                        cantidadParaDscto = (promo.MaxCantidadDscto - cantidadComprada);
                                                    }
                                                    if (cantidadParaDscto < 0) cantidadParaDscto = 0;

                                                    descuent = promo_descuento;
                                                    _factura.Productos[j].DescuentoAX = (descuent / 100M) * Math.Round(_factura.Productos[j]._pvp * cantidadParaDscto, 2, MidpointRounding.AwayFromZero);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    _factura.Productos[j].update();
                }
            }
        }

        /// <summary>
        /// Recupera Lista Precios en objeto Lista, se considera el Establecimiento y Codigo del Articulo
        /// </summary>
        /// <param name="Establecimiento">Codigo Establecimiento</param>
        /// <param name="Itemid">Codigo Articulo</param>
        public static List<Precio> GetListaPreciosInit(string Establecimiento, string Itemid = "")
        {

            Precio listaPrecio = new Precio();
            List<Precio> listaPrecios = new List<Precio>();
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            string IdentificacionClte = string.Empty;

            try
            {

                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, $"Exec spGeneraListaPrecio ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" @Establecimiento = '{Establecimiento}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @IdentificacionClte = '{IdentificacionClte}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @Itemid = '{Itemid}' ", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(sQuery);

                if (dtsConsulta.Tables.Count > 0) {

                    for (int iTable = 0; iTable <= dtsConsulta.Tables.Count - 1; iTable++) {

                        if (dtsConsulta.Tables[iTable].Rows.Count > 0) {
                            string TipoConsulta = dtsConsulta.Tables[iTable].Rows[0]["CTpConsulta"].ToString();

                          
                            if (TipoConsulta == "CONS_LISTA_PRECIO")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[iTable].Rows) {
                                    listaPrecio = new Precio();
                                    listaPrecio.Establecimiento = data["Establecimiento"].ToString();

                                    listaPrecio.ITEMID = data["ITEMID"].ToString();
                                    listaPrecio.ITEMNAME = data["ITEMNAME"].ToString();
                                    listaPrecio.NAMEALIAS = data["NAMEALIAS"].ToString();
                                    listaPrecio.ITEMBARCODE = data["ITEMBARCODE"].ToString();
                                    listaPrecio.UNITID = data["UNITID"].ToString();
                                    listaPrecio.PRICE = decimal.Parse(data["PRICE"].ToString());
                                    listaPrecio.TAXVALUE = decimal.Parse(data["TAXVALUE"].ToString());
                                    listaPrecio.PrecioBase = decimal.Parse(data["PrecioBase"].ToString());
                                    listaPrecio.pvp = decimal.Parse(data["pvp"].ToString());
                                    listaPrecio.PorcDescuento = decimal.Parse(data["PorcDescuento"].ToString());
                                    listaPrecio.valorDescto = decimal.Parse(data["valorDescto"].ToString());
                                    listaPrecio.ValorImpto = decimal.Parse(data["ValorImpto"].ToString());
                                    listaPrecio.pvpImpto = decimal.Parse(data["pvpImpto"].ToString());
                                    listaPrecio.ITEMGROUPID = data["ITEMGROUPID"].ToString();
                                    listaPrecio.Grupo = data["Grupo"].ToString();
                                    listaPrecio.SubGrupo = data["SubGrupo"].ToString();
                                    listaPrecio.Variedad = data["Variedad"].ToString();
                                    listaPrecio.categoria = data["categoria"].ToString();
                                    listaPrecios.Add(listaPrecio);
                                }

                                continue;
                            }

                        }
                    }
                }

                return listaPrecios;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "GetListaPreciosInit", "Error: " + ex.Message);

                listaPrecios = new List<Precio>();
                return listaPrecios;
            }
        }

        /// <summary>
        /// Recupera Lista Precios en objeto Lista, se considera el Establecimiento, la identificación del cliente Codigo del Articulo
        /// </summary>
        /// <param name="Establecimiento">Codigo Establecimiento</param>
        /// <param name="IdentificacionClte">Identificación del Cliente</param>
        /// <param name="Itemid">Codigo Articulo</param>
        /// <returns>La suma de los dos números.</returns>
        public static List<Precio> GetListaPreciosInit(string Establecimiento, string IdentificacionClte = "", string Itemid = "")
        {

            Precio listaPrecio = new Precio();
            List<Precio> listaPrecios = new List<Precio>();
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();

            try
            {

                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, $"Exec spGeneraListaPrecio ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" @Establecimiento = '{Establecimiento}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @IdentificacionClte = '{IdentificacionClte}' ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" , @Itemid = '{Itemid}' ", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(sQuery);

                if (dtsConsulta.Tables.Count > 0)
                {

                    for (int iTable = 0; iTable <= dtsConsulta.Tables.Count - 1; iTable++)
                    {

                        if (dtsConsulta.Tables[iTable].Rows.Count > 0)
                        {
                            string TipoConsulta = dtsConsulta.Tables[iTable].Rows[0]["CTpConsulta"].ToString();

                            //if (TipoConsulta == "CONS_CAB_LISTA_PRECIO") {
                            //    string CodError = dtsConsulta.Tables[iTable].Rows[0]["CodError"].ToString();
                            //    if (CodError != "0") {
                            //        listaPrecios = new List<Precio>();
                            //        return listaPrecios;
                            //    }
                            //}

                            if (TipoConsulta == "CONS_LISTA_PRECIO")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[iTable].Rows)
                                {
                                    listaPrecio = new Precio();
                                    listaPrecio.Establecimiento = data["Establecimiento"].ToString();

                                    listaPrecio.ITEMID = data["ITEMID"].ToString();
                                    listaPrecio.ITEMNAME = data["ITEMNAME"].ToString();
                                    listaPrecio.NAMEALIAS = data["NAMEALIAS"].ToString();
                                    listaPrecio.ITEMBARCODE = data["ITEMBARCODE"].ToString();
                                    listaPrecio.UNITID = data["UNITID"].ToString();
                                    listaPrecio.PRICE = decimal.Parse(data["PRICE"].ToString());
                                    listaPrecio.TAXVALUE = decimal.Parse(data["TAXVALUE"].ToString());
                                    listaPrecio.PrecioBase = decimal.Parse(data["PrecioBase"].ToString());
                                    listaPrecio.pvp = decimal.Parse(data["pvp"].ToString());
                                    listaPrecio.PorcDescuento = decimal.Parse(data["PorcDescuento"].ToString());
                                    listaPrecio.valorDescto = decimal.Parse(data["valorDescto"].ToString());
                                    listaPrecio.ValorImpto = decimal.Parse(data["ValorImpto"].ToString());
                                    listaPrecio.pvpImpto = decimal.Parse(data["pvpImpto"].ToString());
                                    listaPrecio.ITEMGROUPID = data["ITEMGROUPID"].ToString();
                                    listaPrecio.Grupo = data["Grupo"].ToString();
                                    listaPrecio.SubGrupo = data["SubGrupo"].ToString();
                                    listaPrecio.Variedad = data["Variedad"].ToString();
                                    listaPrecio.categoria = data["categoria"].ToString();
                                    listaPrecios.Add(listaPrecio);
                                }

                                continue;
                            }

                        }
                    }
                }

                return listaPrecios;
            }
            catch (Exception)
            {
                listaPrecios = new List<Precio>();
                return listaPrecios;
            }
        }


    }

    public class CodigoBarra
    {
        public string ID { get; set; }
        public string codigo { get; set; }
    }

    public class RespuestaProducto {
        public string CodError { get; set; }
        public string MsjError { get; set; }
        public string ITEMID { get; set; }
        public string ITEMNAME { get; set; }
        public decimal PRICE { get; set; }
    }

    public class LineaFactura
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }

    }


}
