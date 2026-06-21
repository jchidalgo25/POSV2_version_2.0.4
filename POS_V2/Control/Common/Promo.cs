using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using POS.Control;

namespace POS.Control.Common
{
    public static class Promo
    {
        public static bool EsItemPromoCarneCerveza(string itemid)
        {
            if (itemid == "PT-CR-000128"
                                            || itemid == "PT-CR-000130"
                                            || itemid == "PT-CR-000131"
                                            || itemid == "PT-CR-000132"
                                            || itemid == "PT-CR-000143"
                                            || itemid == "PT-CR-000145"
                                            || itemid == "PT-CR-000149"
                                            || itemid == "PT-CR-000155"
                                            || itemid == "PT-CR-000156"
                                            || itemid == "PT-CR-000157"
                                            || itemid == "PT-CR-000162"
                                            || itemid == "PT-CR-000166"
                                            || itemid == "PT-CR-000169"
                                            || itemid == "PT-CR-000178"
                                            || itemid == "PT-CR-000197"
                                            || itemid == "PT-CR-000202"
                                            || itemid == "PT-CR-000206"
                                            || itemid == "PT-CR-000232"
                                            || itemid == "PT-CR-000233"
                                            || itemid == "PT-CR-000234"
                                            || itemid == "PT-CR-000236"
                                            || itemid == "PT-CR-000237"
                                            || itemid == "PT-CR-000240"
                                            || itemid == "PT-CR-000241"
                                            || itemid == "PT-CR-000242"
                                            || itemid == "PT-CR-000243"
                                            || itemid == "PT-CR-000244")
            {
                return true;
            }
            else
                return false;
        }

        #region Promo IVA

        //Se debe cambiar a base de datos apenas se confirme ambiente pruebas con datos actualizados
        public static List<int> _listaDiasPromoIVA;

        public static void SetDiasPromoIVA()
        {
            string codigoAlmacenAx = Control.Common.GlobalParameters.EstablecimientoAxCode;
            try
            {
                if (!string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode))
                {
                    using (POSEntities db = new POSEntities())
                    {
                        var param = db.core_parametro.Where(x => x.identificador == ("DIAS_PROMO_IVA_" + codigoAlmacenAx) &&
                                                            x.valor == "TRUE").FirstOrDefault();
                        if (param != null)
                        {
                            string cadenaParametrizada = (string.IsNullOrWhiteSpace(param.parametro2) ? "" : param.parametro2);
                            _listaDiasPromoIVA = cadenaParametrizada.Split(';').Select(x => int.Parse(string.IsNullOrWhiteSpace(x) ? "-1" : x)).ToList();
                        }
                        else
                        {
                            throw new Exception("No esta configurado parametro '" +
                                "DIAS_PROMO_IVA_" + codigoAlmacenAx +
                                "' con valor 'TRUE' en core_parametro");
                        }
                    }
                }
                else
                {
                    throw new Exception("Aun no esta cargado el codigo de almacen Ax 'ARDP' en POS");
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Common.Promo",
                                                                   "SetDiasPromoIVA",
                                                                   "No fue posible tomar dias PROMOIVA para almacen '" +
                                                                   codigoAlmacenAx +
                                                                   "', a continuacion las excepciones encontradas - " + 
                                                                   Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                _listaDiasPromoIVA = new List<int>(); // { 1, 16 };

            }
        }

        private static List<int> ObtenerDiasPromoIVA()
        {
            return _listaDiasPromoIVA;
        }

        public static bool EsDiaPromoIVA()
        {
            if (ObtenerDiasPromoIVA().Any(x => x.Equals(DateTime.Now.Day)))
                return true;
            else
                return false;
        }

        public static bool EsFechaPromoIVA(DateTime fecha)
        {
            if (ObtenerDiasPromoIVA().Any(x => x.Equals(fecha.Day)))
                return true;
            else
                return false;
        }

        public static bool EsPromoIVA(string establecimiento)
        {
            return Control.Promos.PromoIVA.EsPromoIVA();
            //using (var db = new POSEntities())
            //{
            //    if (db.core_parametro.Where(x => x.identificador == "PROMO_IVA" 
            //                                     && x.valor == "TRUE" 
            //                                     && (x.parametro2 == establecimiento || x.parametro2 == null))
            //                         .FirstOrDefault() != null
            //            && EsDiaPromoIVA())
            //        return true;
            //    else
            //        return false;

            //}
        }

        public static bool EsFacturaConPromoIVA(core_factura factura)
        {
            bool respuesta = false;

            if (factura != null)
            {
                respuesta = EsFechaPromoIVA(factura.fecha_creacion);
            }

            return respuesta;
        }

        public static bool EsItemExcluidoPromoIVA(string itemid)
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    if (db.core_parametro.Any(x => x.identificador == "PROMO_IVA_EXCLUYE"
                                                    &&
                                                    x.valor == "TRUE"
                                                    &&
                                                    (x.parametro2 == null || x.parametro2.Contains(Control.Common.GlobalParameters.EstablecimientoAxCode + ";"))
                                                    &&
                                                    x.documento.Contains(itemid + ";")))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Cointrol.Common.Promo", 
                                                                "EsItemExcluidoPromoIVA", 
                                                                "No fue posible identificar si el producto '" + 
                                                                itemid + 
                                                                "' esta excluido de la promo iva, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

            return false;
        }

        #endregion

        public static bool EstaActivaPromoPaviPlan()
        {
            bool boolActivaPaviPlan = false;
            try
            {
                POSEntities db = new POSEntities();
                var ActivaPaviPlan = db.core_parametro.Where(x => x.identificador == "ACTIVA_PAVIPLAN" && x.valor == "TRUE").FirstOrDefault();

                if (ActivaPaviPlan != null) { boolActivaPaviPlan = true; }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promo", "EstaActivaPromoPaviPlan", Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //WinForm.ShowMessage("No se pudo verificar en este momento el permiso para realizar abonos Paviplan, por seguridad se denegará el permiso. Por favor vuélvalo a intentar");
                Control.Common.General.GetMensajeToList(572);
                boolActivaPaviPlan = false;
            }
        

            return boolActivaPaviPlan;
        }

        public static bool PermiteUsarCuponDsctoPaviplan()
        {
            try
            {
                POSEntities db = new POSEntities();
                return db.core_parametro.Where(x => x.identificador == "PAVIPLAN_PERMITECUPONDSCTO"
                                                    &&
                                                    x.valor == "TRUE"
                                                    &&
                                                    (x.parametro2 == null || x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";"))).FirstOrDefault() != null;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promo", "PermiteUsarCuponDsctoPaviplan", Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //WinForm.ShowMessage("No se pudo verificar en este momento el permiso de usar Cupon de Dscto Paviplan, por seguridad se denegará el permiso. Por favor vuélvalo a intentar");
                Control.Common.General.GetMensajeToList(573);
                return false;
            }
        }

        public static void EjecutarPromoRegalaStock(ref Models.Factura _factura, bool EsApp = false)
        {
            int idPromoEnCurso = 0;
            try
            {
                //Eliminar cualquier ticket previo que se haya creado (Cuando la facturacion no se completa por breve inconveniente)
                _factura.Cupon.RemoveAll(x => x.Referencia == "PROMOREGALASTOCK");

                using (POSEntities db = new POSEntities())
                {
                    DateTime fechaActual = DateTime.Now;
                    var promos = db.core_parametro.Where(x => x.identificador == "PROMOREGALASTOCK"
                                                                && x.fecha_creacion <= fechaActual
                                                                && x.fecha_modificacion >= fechaActual
                                                                && (x.valor == "TRUE" || (x.valor == "APP" && EsApp == true))
                                                                && (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento) || x.parametro2 == null))
                                                 .ToList();

                    if (promos.Count > 0)
                    {
                        var mensaje_promo = string.Empty;

                        foreach (var promo in promos)
                        {
                            idPromoEnCurso = promo.id;

                            string[] parametros = promo.documento.Split('|');
                            /*
                                titulo [0]
                                valor [1]
                                es_unitario? [2]
                                es_moneda o cantidad? [3]
                                permite_consumidorfinal [4]
                                por cantidad o unidades [5]
                            */

                            if (parametros[4] == "1" ||
                                (parametros[4] == "0" && _factura.ClienteIdentificacion != Common.GlobalParameters.IdConsumidorFinal))
                            {
                                decimal valorAplicable = 0;
                                if (!string.IsNullOrEmpty(promo.documento))
                                {
                                    var itemsParticipantes = db.core_parametro.Where(x => x.identificador == "PROMOREGALASTOCK_DETALLE"
                                                                                            && x.valor == promo.id.ToString())
                                                                              .ToList();

                                    if (itemsParticipantes.Count > 0)
                                    {
                                        var itemsRegalos = db.core_parametro.Where(x => x.identificador == "PROMOREGALASTOCK_REGALO"
                                                                                                && x.valor == promo.id.ToString())
                                                                                  .ToList();
                                        if (itemsRegalos.Count > 0)
                                        {

                                            foreach (var p in _factura.Productos)
                                            {
                                                if (itemsParticipantes.Any(x => x.parametro2 == p.Id))
                                                {
                                                    //Es moneda (true) o cantidad (false)  parametros[3]
                                                    //Es p.Unidades (true)  o p.Cantidad(false)    JM   18-11-2020
                                                    if (parametros[5] == "1")
                                                        valorAplicable += parametros[3] == "1" ? (p.Total - (Control.Common.Promo.EsPromoIVA(_factura.Establecimiento) ? p.Iva : 0)) : p.Unidades;
                                                    else
                                                        valorAplicable += parametros[3] == "1" ? (p.Total - (Control.Common.Promo.EsPromoIVA(_factura.Establecimiento) ? p.Iva : 0)) : p.Cantidad;
                                                }
                                            }


                                            if (valorAplicable >= decimal.Parse(parametros[1]) && valorAplicable > 0)
                                            {
                                                var cantidadGanada = Math.Truncate(valorAplicable / decimal.Parse(parametros[1]));

                                                foreach (var regalo in itemsRegalos)
                                                {
                                                    int cantidadRecibida = 0;
                                                    if (ExisteStock(regalo.parametro2, cantidadGanada))
                                                    {
                                                        string[] parametrosRegalo = regalo.documento.Split('|');
                                                        /*
                                                            descripcionfactura
                                                            descripcionTicket
                                                        */

                                                        var productoRegalo = new Producto();
                                                        //Valida que exista el producto
                                                        if (productoRegalo.getProducto(regalo.parametro2, _factura, new pos_customer()))
                                                        {
                                                            for (var i = 1; i <= cantidadGanada; i++)
                                                            {
                                                                cantidadRecibida++;

                                                                //Validar si promocion regala un solo producto
                                                                if (parametros[2] == "1" && i == 1) break;
                                                            }

                                                            // Valida que no se haya aplicado el item de promoción. JM  7-12-2020
                                                            if (!_factura.Productos.Any(x => x.Id == regalo.parametro2))
                                                            {
                                                                //Agregar mensaje en factura
                                                                mensaje_promo += string.IsNullOrEmpty(mensaje_promo) ? string.Empty : (Environment.NewLine + "---------------------------------------------------------------------" + Environment.NewLine);
                                                                mensaje_promo += parametrosRegalo[0].Replace("<<cantidad>>", cantidadRecibida.ToString());

                                                                //Agregar cupon desprendible
                                                                _factura.Cupon.Add(new Cupones()
                                                                {
                                                                    Texto = parametrosRegalo[1].Replace("<<titulo>>", parametros[0]).Replace("<<cantidad>>", cantidadRecibida.ToString()).Replace("<<comprobante>>", _factura.GetNumeroFactura()),
                                                                    Valor = decimal.Parse(parametros[1]),
                                                                    Unico = true,
                                                                    Giftcard = false,
                                                                    Valorgiftcard = 0,
                                                                    Referencia = "PROMOREGALASTOCK"
                                                                });

                                                                //Agregar producto con descuento 100%
                                                                productoRegalo.Cantidad = cantidadRecibida;
                                                                productoRegalo.CantidadINEC = cantidadRecibida;
                                                                productoRegalo.Unidades = cantidadRecibida;
                                                                //Representa el 100%
                                                                productoRegalo.DescuentoActual = 1;
                                                                productoRegalo.Descuento = productoRegalo.SubtotalSinDescuento;
                                                                productoRegalo.Iva = decimal.Round(productoRegalo.Subtotal * productoRegalo.IvaProducto, 2);
                                                                productoRegalo.EsRegalo = true;
                                                                _factura.Productos.Add(productoRegalo);
                                                            }
                                                            else if (_factura.Productos.Any(x => x.Id == regalo.parametro2 && x.Total > 0 && !x.EsRegalo))
                                                            {
                                                                //Agregar mensaje en factura
                                                                mensaje_promo += string.IsNullOrEmpty(mensaje_promo) ? string.Empty : (Environment.NewLine + "---------------------------------------------------------------------" + Environment.NewLine);
                                                                mensaje_promo += parametrosRegalo[0].Replace("<<cantidad>>", cantidadRecibida.ToString());

                                                                //Agregar cupon desprendible
                                                                _factura.Cupon.Add(new Cupones()
                                                                {
                                                                    Texto = parametrosRegalo[1].Replace("<<titulo>>", parametros[0]).Replace("<<cantidad>>", cantidadRecibida.ToString()).Replace("<<comprobante>>", _factura.GetNumeroFactura()),
                                                                    Valor = decimal.Parse(parametros[1]),
                                                                    Unico = true,
                                                                    Giftcard = false,
                                                                    Valorgiftcard = 0,
                                                                    Referencia = "PROMOREGALASTOCK"
                                                                });

                                                                var existente = _factura.Productos.Single(x => x.Id == regalo.parametro2);

                                                                existente.Cantidad = existente.Cantidad + cantidadRecibida;
                                                                existente.CantidadINEC = existente.Cantidad;
                                                                existente.Unidades = existente.Unidades + cantidadRecibida;

                                                                if (existente.Id == regalo.parametro2)
                                                                {
                                                                    existente.Descuento = productoRegalo.SubtotalSinDescuento;
                                                                }
                                                                //existente.DescuentoActual = 1;
                                                                //existente.Descuento = productoRegalo.SubtotalSinDescuento;
                                                                //existente.Iva = decimal.Round((productoRegalo.Subtotal) * (productoRegalo.IvaProducto), 2);
                                                                existente.EsRegalo = true;

                                                                existente.update(false);
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(mensaje_promo))
                            _factura.mensaje_promo = mensaje_promo;
                    }

                }
            }
            catch (Exception ex)
            {
                if (idPromoEnCurso == 0)
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promo", "EjecutarPromoRegalaStock", "No se pudo verificar ninguna promocion, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                else
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promo", "EjecutarPromoRegalaStock", "Se presentaron inconvenientes mientras se procesaba la promocion con id '" + idPromoEnCurso + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }
        public static string EjecutarPromoDescuentoProducto(ref Models.Factura _factura, bool EsApp = false)
        {
            int idPromoEnCurso = 0;
            string AplicaPromoDescuento = "";
            try
            {
                //Eliminar cualquier ticket previo que se haya creado (Cuando la facturacion no se completa por breve inconveniente)
                _factura.Cupon.RemoveAll(x => x.Referencia == "PROMODESCUENTO");
                using (POSEntities db = new POSEntities())
                {
                    DateTime fechaActual = DateTime.Now;
                    var promos = db.core_parametro.Where(x => x.identificador == "PROMODESCUENTO"
                                                                && x.fecha_creacion <= fechaActual
                                                                && x.fecha_modificacion >= fechaActual
                                                                && (x.valor == "TRUE" || (x.valor == "APP" && EsApp == true))
                                                                && (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento) || x.parametro2 == null))
                                                 .ToList();

                    if (promos.Count > 0)
                    {
                        var mensaje_promo = string.Empty;

                        foreach (var promo in promos)
                        {
                            idPromoEnCurso = promo.id;

                            string[] parametros = promo.documento.Split('|');
                            /*
                               titulo [0]
                               valor [1]
                               es_unitario? [2]
                               es_moneda o cantidad? [3]
                               permite_consumidorfinal [4]
                               por cantidad o unidades [5]
                           */

                            if (parametros[4] == "1" || (parametros[4] == "0" && _factura.ClienteIdentificacion != Common.GlobalParameters.IdConsumidorFinal))
                            {
                                decimal valorAplicable = 0;
                                if (!string.IsNullOrEmpty(promo.documento))
                                {
                                 
                                    var itemsParticipantes = db.core_parametro.Where(x => x.identificador == "PROMODESCUENTO_DETALLE"
                                                                                            && x.valor == promo.id.ToString())
                                                                              .ToList();

                                    if (itemsParticipantes.Count >= 0)
                                    {
                                        var itemsConDescuento = db.core_parametro.Where(x => x.identificador == "PROMODESCUENTO_PRODUCTO"
                                                                                                && x.valor == promo.id.ToString())
                                                                                  .ToList();
                                        if (itemsConDescuento.Count > 0)
                                        {                                          

                                            foreach (var p in _factura.Productos)
                                            {
                                                if (itemsParticipantes.Count > 0)
                                                { 
                                                    if (itemsParticipantes.Any(x => x.parametro2 == p.Id))
                                                    {

                                                        if (parametros[5] == "1") 
                                                            valorAplicable += parametros[3] == "1" ? (p.Total - (Control.Common.Promo.EsPromoIVA(_factura.Establecimiento) ? p.Iva : 0)) : p.Unidades;
                                                        else
                                                            valorAplicable += parametros[3] == "1" ? (p.Total - (Control.Common.Promo.EsPromoIVA(_factura.Establecimiento) ? p.Iva : 0)) : p.Cantidad;
                                                    }
                                                }
                                                else
                                                {
                                                    if (parametros[5] == "1")
                                                        valorAplicable += parametros[3] == "1" ? (p.Total - (Control.Common.Promo.EsPromoIVA(_factura.Establecimiento) ? p.Iva : 0)) : p.Unidades;
                                                    else
                                                        valorAplicable += parametros[3] == "1" ? (p.Total - (Control.Common.Promo.EsPromoIVA(_factura.Establecimiento) ? p.Iva : 0)) : p.Cantidad;
                                                }
                                            }


                                            if (valorAplicable >= decimal.Parse(parametros[1]) && valorAplicable > 0)
                                            {
                                                //var cantidadGanada = Math.Truncate(valorAplicable / decimal.Parse(parametros[1]));                                                
                                                int validaItemPromo = 0;

                                                foreach (var descuento in itemsConDescuento)
                                                {
                                                    string[] parametrosRegalo = descuento.documento.Split('|');
                            
                                                    var productoRegalo = new Producto();
                                                        //Valida que exista el producto
                                                        //if (productoRegalo.getProducto(descuento.parametro2, _factura, new pos_customer()))
                                                        //{
                                         
                                                            // Valida que no se haya aplicado el item de promoción. JM  7-12-2020
                                                    if (!_factura.Productos.Any(x => x.Id == descuento.parametro2))
                                                    {
                                                        if (parametros[6] == "1")   //JCanarte 28Abril2021 Parámetro 6 indica 1 muestra mensaje promoción  0 no lo muestra
                                                        {
                                                            var codigoBarra = db.pos_item.Where(x => x.ITEMID == descuento.parametro2).FirstOrDefault();
                                                            AplicaPromoDescuento += "\n" + codigoBarra.ITEMNAME + " ";
                                                        }
                                                    }
                                                    else if(_factura.Productos.Any(x => x.Id == descuento.parametro2 && x.Total > 0 && !x.EsRegalo))
                                                    {
                                                        
                                                        foreach (var pro in _factura.Productos)
                                                        {
                                                            if (descuento.parametro2 == pro.Id)
                                                            {
                                                                if (validaItemPromo == 0)
                                                                {
                                                                    if (parametros.Length == 8)
                                                                    {
                                                                        if (parametros[7] == "1")//valida si parametro[7]=0 no hay restrincion de productos promocion, si parametro[7]=1 solo puede escoger 1 item de promo
                                                                        {
                                                                            validaItemPromo = 1;
                                                                        }
                                                                    }
                                                                    if (parametros[2] == "1" && pro.Cantidad <= 1)
                                                                    {                                                                      
                                                                            if (Control.Common.GlobalParameters.AplicaPromoMayor == "FALSE" || string.IsNullOrEmpty(Control.Common.GlobalParameters.AplicaPromoMayor))
                                                                            {
                                                                                pro.DescuentoActual = (decimal.Parse(descuento.documento) / 100);
                                                                                pro.Descuento = productoRegalo.SubtotalSinDescuento * (decimal.Parse(descuento.documento) / 100);
                                                                                pro.update();
                                                                            }
                                                                            else
                                                                            {
                                                                                if (pro.DescuentoActual < (decimal.Parse(descuento.documento) / 100))
                                                                                {
                                                                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Promo", "EjecutarPromoDescuentoProducto", "Aplica descuento mayor, descuento anterior:" + Convert.ToString(pro.DescuentoActual) + ", descuento nuevo:"+ descuento.documento);
                                                                                    pro.DescuentoActual = (decimal.Parse(descuento.documento) / 100);
                                                                                    pro.Descuento = productoRegalo.SubtotalSinDescuento * (decimal.Parse(descuento.documento) / 100);
                                                                                    pro.update();
                                                                                }
                                                                            }
                                                                                                                                              
                                                                    }
                                                                    else if (parametros[2] == "1" && pro.Cantidad > 1)
                                                                    {
                                                                        descuento.documento = "0";
                                                                        pro.DescuentoActual = (decimal.Parse(descuento.documento) / 100);
                                                                        pro.Descuento = productoRegalo.SubtotalSinDescuento * (decimal.Parse(descuento.documento) / 100);
                                                                        pro.update();
                                                                    }
                                                                    else if(parametros[2] == "0")
                                                                    {                                                                        
                                                                            if (Control.Common.GlobalParameters.AplicaPromoMayor == "FALSE" || string.IsNullOrEmpty(Control.Common.GlobalParameters.AplicaPromoMayor))
                                                                            {
                                                                                pro.DescuentoActual = (decimal.Parse(descuento.documento) / 100);
                                                                                pro.Descuento = productoRegalo.SubtotalSinDescuento * (decimal.Parse(descuento.documento) / 100);
                                                                                pro.update();
                                                                            }
                                                                            else
                                                                            {
                                                                                if (pro.DescuentoActual <= (decimal.Parse(descuento.documento) / 100))
                                                                                {
                                                                                    if (Math.Truncate(valorAplicable / decimal.Parse(parametros[1])) >= pro.Cantidad)
                                                                                    {
                                                                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Promo", "EjecutarPromoDescuentoProducto", "Aplica descuento mayor, descuento anterior:" + Convert.ToString(pro.DescuentoActual) + ", descuento nuevo:" + descuento.documento);
                                                                                            pro.DescuentoActual = (decimal.Parse(descuento.documento) / 100);
                                                                                            pro.Descuento = productoRegalo.SubtotalSinDescuento * (decimal.Parse(descuento.documento) / 100);
                                                                                            pro.update();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        pro.DescuentoActual = (decimal.Parse(descuento.documento) / 100);
                                                                                        pro.Descuento = (pro.Pvp * (Math.Truncate(valorAplicable / decimal.Parse(parametros[1])))) * (decimal.Parse(descuento.documento) / 100);
                                                                                        //pro.update();
                                                                                    }
                                                                                }
                                                                            }
                                                                        
                                                                    }                                                                    
                                                                   
                                                                }
                                                                else
                                                                {
                                                                    descuento.documento = "0";
                                                                    pro.DescuentoActual = (decimal.Parse(descuento.documento) / 100);
                                                                    pro.Descuento = productoRegalo.SubtotalSinDescuento * (decimal.Parse(descuento.documento) / 100);
                                                                    pro.update();
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                                if (validaItemPromo == 1) { AplicaPromoDescuento = ""; }
                                            }
                                            else
                                            {
                                                foreach (var descuento in itemsConDescuento)
                                                {
                                                    string[] parametrosRegalo = descuento.documento.Split('|');

                                                    var productoRegalo = new Producto();
                                                    ////Valida que exista el producto
                                                    //if (productoRegalo.getProducto(descuento.parametro2, _factura, new pos_customer()))
                                                    //{

                                                        // Valida que no se haya aplicado el item de promoción. JM  7-12-2020
                                                        if (!_factura.Productos.Any(x => x.Id == descuento.parametro2))
                                                        {
                                                            if (parametros[6] == "1")   //JCanarte 28Abril2021 Parámetro 6 indica 1 muestra mensaje promoción  0 no lo muestra
                                                            {
                                                  
                                                            }
                                                        }
                                                        else if (_factura.Productos.Any(x => x.Id == descuento.parametro2 && x.Total > 0 && !x.EsRegalo))
                                                        {
                                                            foreach (var pro in _factura.Productos)
                                                            {
                                                                if (descuento.parametro2 == pro.Id)
                                                                {
                                                                //if (Control.Common.GlobalParameters.AplicaPromoMayor == "FALSE" || string.IsNullOrEmpty(Control.Common.GlobalParameters.AplicaPromoMayor))
                                                                //{
                                                                    pro.DescuentoActual = 0;
                                                                    pro.Descuento = 0;
                                                                    pro.update();
                                                                //}
                                                                }
                                                            }
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(mensaje_promo))
                            _factura.mensaje_promo = mensaje_promo;
                    }

                }
                return AplicaPromoDescuento;
            }
            catch (Exception ex)
            {
                return "";
                if (idPromoEnCurso == 0)
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promo", "EjecutarPromoDescuentoProducto", "No se pudo verificar ninguna promocion, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                else
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promo", "EjecutarPromoDescuentoProducto", "Se presentaron inconvenientes mientras se procesaba la promocion con id '" + idPromoEnCurso + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private static bool ExisteStock(string itemId, decimal cantidadRequerida)
        {
            return true;
            bool respuesta = true;
            try
            {
                SqlConnection conexion = new SqlConnection(Properties.Settings.Default.CONECTA_AX);
                string Query = null;
                SqlCommand comando = default(SqlCommand);
                using (conexion)
                {
                    conexion.Open();
                    Query = "select ism.AvailPhysical,dim.InventLocationId,dim.InventDimId from [SRV-AX].DynamicsAx1.dbo.InventSum ism with(nolock) inner join " +
                            "[SRV-AX].DynamicsAx1.dbo.InventDim dim with(nolock) on ism.DataAreaId = dim.DataAreaId and ism.InventDimId = dim.InventDimId " +
                            "where ism.DataAreaId = 'liri' and ism.itemid = '" + itemId + "' and InventLocationId = '" + 
                            Common.GlobalParameters.EstablecimientoAxCode + "' ";

                    comando = new SqlCommand(Query, conexion);
                    SqlDataReader dr = comando.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        //Si el inventario fisico disponible es mayor a lo requerido, entonces hay stock
                        if (Decimal.Parse(dr.GetValue(0).ToString()) >= cantidadRequerida)
                            respuesta = true;
                        else
                            respuesta = false;
                    }
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promo", "ExisteStock", "No se pudo obtener la cantidad disponible del item '" + itemId + "' en el establecimiento con codigo Ax '" + Common.GlobalParameters.EstablecimientoAxCode + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                respuesta = false;
            }

            return respuesta;
        }

        public static void EjecutarPromoRegala(ref Models.Factura _factura)
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    DateTime fechaActual = DateTime.Now;
                    var jornada = db.core_parametro.Where(x => x.identificador == "PROMOREGALA_JORNADA"
                                                                && x.valor == "TRUE"
                                                                && (x.parametro2 == Control.Common.GlobalParameters.Establecimiento || x.parametro2 == null)
                                                                && x.fecha_creacion <= fechaActual
                                                                && x.fecha_modificacion >= fechaActual
                                                                )
                                                   .FirstOrDefault();

                    if (jornada != null)
                    {
                        var promo = db.core_parametro.Where(x => x.identificador == "PROMOREGALA"
                                                                    && x.fecha_creacion <= fechaActual
                                                                    && x.fecha_modificacion >= fechaActual
                                                                    && (x.parametro2 == Control.Common.GlobalParameters.Establecimiento || x.parametro2 == null))
                                                     .FirstOrDefault();

                        if (promo != null)
                        {
                            if (int.Parse(promo.valor) > 0)
                            {
                                var splitParametros = promo.documento.Split('|');
                                int maxRandom = int.Parse(splitParametros[3]);
                                int probabiltyPercent = int.Parse(splitParametros[0]);

                                if (TieneSuerte(maxRandom, probabiltyPercent))
                                {
                                    promo.valor = (int.Parse(promo.valor) - 1).ToString();
                                    db.SaveChanges();

                                    _factura.mensaje_promo = splitParametros[1];

                                    System.Windows.Forms.DialogResult dr = new System.Windows.Forms.DialogResult();
                                    Ganador frm = new Ganador(splitParametros[2]);
                                    while (dr != System.Windows.Forms.DialogResult.OK && dr != System.Windows.Forms.DialogResult.Retry)
                                    {
                                        dr = frm.ShowDialog();
                                    }
                                    switch (dr)
                                    {
                                        case System.Windows.Forms.DialogResult.OK:
                                            break;
                                        case System.Windows.Forms.DialogResult.Retry:
                                            break;
                                    }

                                    try
                                    {
                                        var xmlRespuesta = Mail.EnviaCorreo(
                                            Properties.Settings.Default.MAILERROR_FROM,
                                            Properties.Settings.Default.MAILERROR_ALIAS,
                                            Properties.Settings.Default.MAILERROR_CC,
                                            Properties.Settings.Default.MAILERROR_CC,
                                            "TORTA MAMA",
                                            String.Format("Ganador de torta para mama, Cliente: " + (string.IsNullOrEmpty(_factura.Cliente_nombre) ? "" : (_factura.Cliente_nombre + " - ")) + _factura.ClienteIdentificacion + " Factura No: " + _factura.GetNumeroFactura()),
                                            false,
                                            String.Empty);

                                        if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                                        {
                                            //throw new Exception(xmlRespuesta.DocumentElement.GetAttribute("MsgError")); 
                                        }
                                    }
                                    catch { }
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

        private static bool TieneSuerte(int maxRandom, int probabilityPercent)
        {
            try
            {
                //return true;
                Random rnd = new Random();
                int suerte = rnd.Next(1, maxRandom);

                return (suerte <= probabilityPercent);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region DescuentosAx Gestores
        
        private static List<string> ConjuntosClienteExcluidosDescGestores = new List<string> { /* "07", "09", "CE" */ };

        public static bool PuedeConjuntoClienteRecibirDescGestor(string conjuntoCliente)
        {
            if (ConjuntosClienteExcluidosDescGestores.Any(x => x == conjuntoCliente))
                //Si conjunto aparece en la lista entonces no puede recibir descuento de gestor
                return false;
            else
                return true;
        }

        #endregion
    }
}
