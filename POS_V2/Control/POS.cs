using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using POS.Models;
using System.Net.Sockets;
using Telerik.WinControls;
using System.Windows.Forms;
using System.Messaging;
using static POS.Models.ClsMessageQueue;
using MensajesLibrary;
using System.Diagnostics;

namespace POS.Control
{

    public class POS
    {

        MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
        MsgBoxCtrl.MessageType messageType;
        MsgBoxCtrl.MessageBoxResult result;


        public static string ObtenerIPv4Local()
        {
            try
            {
                string nombrePC = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(nombrePC);

                IPAddress ipv4 = ipEntry.AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                return ipv4?.ToString() ?? "No se encontró una dirección IPv4";
            }
            catch (Exception ex)
            {
                return $"Error al obtener la IP: {ex.Message}";
            }
        }

        /// <summary>
        /// Inicia la aplicación con documento factura por defecto.
        /// Revisa si la IP de la maquina donde se ejecuta la app esta en la BD.
        /// Asigna datos primarios a factura.
        /// </summary>
        /// <param name="factura">Variable de factura global</param>
        /// <returns>Verdadero si no hubo problemas, False si no existe en la BD el punto de emisión o documento asignado a punto de emisión</returns>
        public static bool init(ref Factura factura, string tipo_documento="F")
        {
            string ipAddress = string.Empty;
            string nombrePC = Dns.GetHostName().ToString();
            IPHostEntry ipEntry = Dns.GetHostEntry(nombrePC);
            //IPAddress[] addr = ipEntry.AddressList;
            ////Verificamos la IP de la PC donde se ejecuta la APP.
            //ipAddress = addr.Where(i => !i.IsIPv6LinkLocal && !i.IsIPv6Teredo && i.AddressFamily == AddressFamily.InterNetwork).First().ToString();


            ipAddress = ObtenerIPv4Local();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "init", $"nombrePC: {nombrePC}");
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "init", $"ipEntry: {ipEntry}");
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "init", $"ipEntry.AddressList: {ipEntry.AddressList}");  
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "init", $"ipAddress: {ipAddress}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            

            //ipAddress = "192.168.10.158";
            //ipAddress = "192.168.121.4";
            var hoy = DateTime.Now;
            var result = false;


            //string cedula = factura.User == null? "" : factura.User.username;
            string cedula;
            if (factura.User == null)
            {
                factura.User = new User();
                cedula = "";
            }
            else
            {
                cedula = factura.User.username;
            }

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "init", $"ipAddress: {ipAddress}");

            //var cedula = factura.User.username;
            //Buscamos el punto de emisión
            using (var db = new POSEntities())
            {
                //Modificaciones JEspinoza
                // 1. Unificar las consultas para el punto de emisión.
                var st1 = stopwatch.ElapsedMilliseconds;
                var listaPtos = db.core_puntoemision
                                  .Where(x => x.ip_address == ipAddress)
                                  .ToList();
                var st2 = stopwatch.ElapsedMilliseconds;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_puntoemision", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                if (listaPtos.Count == 0)
                {
                    Control.Common.General.GetMensajeToList(163);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_puntoemision", "Problemas con la asignación de la IP.");
                    return false;
                }

                if (listaPtos.Count > 1)
                {
                    Control.Common.General.GetMensajeToList(164);
                    return false;
                }

                // Obtener el punto de emisión una sola vez.
                var pto = listaPtos.Single();

                // 2. Unificar la consulta para la apertura del cajero.
                st1 = stopwatch.ElapsedMilliseconds;
                var apertura = db.viewAperturaLogin
                                 .Join(db.core_puntoemision,
                                       b => new { c1 = b.establecimiento, c2 = b.punto_emision },
                                       a => new { c1 = a.establecimiento_id, c2 = a.punto_emision },
                                       (b, a) => new { b, a })
                                 .FirstOrDefault(x => x.b.username == cedula);
                st2 = stopwatch.ElapsedMilliseconds;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "viewAperturaLogin", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                if (apertura != null)
                {
                    var ptonew = db.core_puntoemision.Single(x => x.establecimiento_id == apertura.a.establecimiento_id && x.punto_emision == apertura.a.punto_emision);
                    pto.punto_emision = ptonew.punto_emision;
                    pto.id = ptonew.id;
                }
                else
                {
                    // Lógica para cuando no hay apertura, pero el punto de emisión sigue siendo el del equipo.
                    factura.PtoEmisionOrigen = pto.punto_emision;
                }

                // 3. Unificar las consultas para la secuencia de documento.
                st1 = stopwatch.ElapsedMilliseconds;
                var documento_secuencia = db.core_documentosecuencia
                                            .FirstOrDefault(x => x.core_documento.codigo == tipo_documento && x.punto_emision_id == pto.id);
                st2 = stopwatch.ElapsedMilliseconds;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_documentosecuencia", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());

                if (documento_secuencia == null)
                {
                    Control.Common.General.GetMensajeToList(165);
                    return false;
                }

                // Asignar datos a la factura.
                factura.Establecimiento = pto.establecimiento_id;
                factura.PtoEmision = pto.punto_emision;
                factura.PtoEmisionOrigen = pto.punto_emision;

                Common.GlobalParameters.PuntoEmision = pto.punto_emision;
                Common.GlobalParameters.Establecimiento = pto.establecimiento_id;

                st1 = stopwatch.ElapsedMilliseconds;
                factura.Secuencia = MainWindow.GetSecuenciaValidada();
                st2 = stopwatch.ElapsedMilliseconds;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "GetSecuenciaValidada", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                factura.Documento = documento_secuencia.core_documento.codigo;
                factura.Ip_address = ipAddress;
                factura.Establecimiento_nombre = pto.core_establecimiento.nombre;
                factura.Establecimiento_direccion = pto.core_establecimiento.direccion;
                factura.Establecimiento_telefono = pto.core_establecimiento.telefono;
                Common.GlobalParameters.EstablecimientoAxCode = pto.core_establecimiento.almacen;

                // 4. Unificar las consultas para la autorización SRI.
                st1 = stopwatch.ElapsedMilliseconds;
                var autorizacion = db.pos_autorizacionsri
                                     .FirstOrDefault(x => x.activo && hoy >= x.fecha_inicio && hoy <= x.fecha_fin);
                st2 = stopwatch.ElapsedMilliseconds;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "autorizacion", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                if (autorizacion == null)
                {
                    Control.Common.General.GetMensajeToList(166);
                    return false;
                }

                // Asignar datos de autorización a la factura.
                factura.Autorizacion = autorizacion.autorizacion;
                factura.Fecha_inicio_autorizacion = autorizacion.fecha_inicio;
                factura.Fecha_fin_autorizacion = autorizacion.fecha_fin;

                // Asignar otros parámetros de la factura.
                factura.UsaBalanza = pto.usa_balanza;
                factura.UsarScannerIntegrado = pto.scanner_integrado;
                factura.PuertoBalanza = pto.puerto_balanza;
                factura.MarcaBalanza = pto.marca_balanza;
                factura.Nombre_sucursal = pto.core_establecimiento.nombre;
                factura.Direccion_sucursal = pto.core_establecimiento.direccion;
                factura.Direccion_matriz = "KM 5 1/2 VIA DURAN - BABAHOYO";
                factura.Razon_social_matriz = "LIRIS S.A";
                factura.Ruc_matriz = "0990865477001";
                factura.ModeloBalanza = pto.ModeloBalanza;
                factura.IpPinPad = pto.IpPinpad;
                factura.EstTcpIpPinpad = pto.EsTcipPinpad;
                factura.PuertoPinPad = pto.puerto_pinpad;

                //if (factura.Documento == "F")
                //{
                //    //* CAMBIO TEMPORAL PARA FACTURACION ELECTRONICA DE GRAN MANZANA *//
                //    /*if (factura.Establecimiento != "012")
                //    {
                //        var recibo = db.core_recibo.Single(x => x.identificador == "FACTURA");
                //        factura.Recibo = recibo.cuerpo;
                //    }*/
                //    st1 = stopwatch.ElapsedMilliseconds;
                //    var recibo = db.core_recibo.Where(x => x.identificador == (Control.Common.GlobalParameters.ComprobanteFactura + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();
                //    st2 = stopwatch.ElapsedMilliseconds;
                //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_recibo1", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                //    if (recibo == null)
                //    {
                //        st1 = stopwatch.ElapsedMilliseconds;
                //        recibo = db.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteFactura).FirstOrDefault();
                //        st2 = stopwatch.ElapsedMilliseconds;
                //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_recibo2", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                //    }


                //    factura.Recibo = recibo.cuerpo;

                //}
                //else if (factura.Documento == "R")
                //{
                //    var recibo = db.core_recibo.Single(x => x.identificador == "RECIBO_GIFTCARD");
                //    factura.Recibo = recibo.cuerpo;
                //}

                //debug jchid para impresion 17/04/2026
                if (factura.Documento == "F")
                {
                    st1 = stopwatch.ElapsedMilliseconds;
                    var recibo = db.core_recibo.Where(x => x.identificador == (Control.Common.GlobalParameters.ComprobanteFactura + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();
                    st2 = stopwatch.ElapsedMilliseconds;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_recibo1", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                    if (recibo == null)
                    {
                        st1 = stopwatch.ElapsedMilliseconds;
                        recibo = db.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteFactura).FirstOrDefault();
                        st2 = stopwatch.ElapsedMilliseconds;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_recibo2", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());
                    }

                    factura.Recibo = recibo.cuerpo;

                    // ============ DIAG LOG INIT ============
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info,
                        "MainWindow", "DiagImpresion",
                        $"[DIAG-INIT] Recibo cargado en init | " +
                        $"Buscó: [{Control.Common.GlobalParameters.ComprobanteFactura + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode}] | " +
                        $"Encontró: [{recibo?.identificador ?? "NULL"}] | " +
                        $"Largo: {recibo?.cuerpo?.Length ?? 0}");
                    // ============ FIN DIAG LOG INIT ============
                }
                else if (factura.Documento == "R")
                {
                    var recibo = db.core_recibo.Single(x => x.identificador == "RECIBO_GIFTCARD");
                    factura.Recibo = recibo.cuerpo;
                }
                //debug jchid para impresion 17/04/2026
                st1 = stopwatch.ElapsedMilliseconds;
                var aperturaCaja = db.BG_Apertura
                             .FirstOrDefault(x => x.CEDULA == cedula && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0);
                st2 = stopwatch.ElapsedMilliseconds;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "BG_Apertura", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());

                if (aperturaCaja != null)
                {
                    Program.ID_Caja_POS = aperturaCaja.ID_CAJA;
                    Control.Common.GlobalParameters.TieneAperturaCajaManual = false;
                }
                else
                {
                    Control.Common.GlobalParameters.TieneAperturaCajaManual = true;
                    st1 = stopwatch.ElapsedMilliseconds;
                    var existe_idcaja = db.core_factura
                                          .FirstOrDefault(x => x.usuario == cedula &&
                                                               System.Data.Entity.DbFunctions.TruncateTime(x.fecha_creacion) == hoy.Date &&
                                                               x.msgError.Contains("-MA"));
                    st2 = stopwatch.ElapsedMilliseconds;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Debug, "Ejecutar init", "core_factura", st1.ToString() + " " + st2.ToString() + ":" + (st2 - st1).ToString());

                    Program.ID_Caja_POS = existe_idcaja?.msgError ?? "CAJA_ABIERTA_MANUAL";
                }


                #region JEspinoza
                //if (result = db.core_puntoemision.Any(x => x.ip_address == ipAddress))
                //{
                //    //var listaPtos = db.core_puntoemision.Where(x => x.ip_address == ipAddress).ToList();

                //    //if (listaPtos.Count() == 0)
                //    //{
                //    //    // Control.Common.General.GetMensaje("POS", "No tiene asignado una IP contacte a administrador!", "I");
                //    //    Control.Common.General.GetMensajeToList(163);

                //    //    // MessageBox.Show("No tiene asignado una IP contacte a administrador!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    //    return false;
                //    //}

                //    //if (listaPtos.Count() > 1)
                //    //{
                //    //    //Control.Common.General.GetMensaje("POS", "Existe más de una caja configurada con la misma IP, contacte a administrador!", "I");
                //    //    Control.Common.General.GetMensajeToList(164);

                //    //    //MessageBox.Show("Existe más de una caja configurada con la misma IP, contacte a administrador!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    //    return false;
                //    //}

                //    // buscar los cajeros aperturados, para sacar el punto de emision al que pertenece el usuario logeado.  JM  17-09-2019
                //    //var apertura = (from a in db.core_puntoemision
                //    //                join b in db.viewAperturaLogin on new { c1 = a.establecimiento_id, c2 = a.punto_emision } equals new { c1 = b.establecimiento, c2 = b.punto_emision }
                //    //                where b.username == cedula
                //    //                select a).FirstOrDefault();

                //    ////var pto = db.core_puntoemision.Single(x => x.ip_address == ipAddress);
                //    //factura.PtoEmisionOrigen = pto.punto_emision;  // setear el punto de emision del equipo.  JM    17-09-2019
                //    //if (apertura != null)
                //    //{
                //    //    var ptonew = db.core_puntoemision.Single(x => x.establecimiento_id == apertura.establecimiento_id && x.punto_emision == apertura.punto_emision);
                //    //    pto.punto_emision = ptonew.punto_emision;
                //    //    pto.id = ptonew.id;
                //    //}

                //    //No se utiliza en el entorno local JEspinoza
                //    //Buscamos el documento asignado al punto de emisión
                //    //var documentoSecuencia = (from deta in db.core_documentosecuencia
                //    //                          where deta.core_documento.codigo == tipo_documento
                //    //                          && deta.punto_emision_id == pto.id
                //    //                          select deta).ToList();

                //    if (result = db.core_documentosecuencia.Any(x => x.core_documento.codigo == tipo_documento && x.punto_emision_id == pto.id))
                //    {
                //        ////Se asigna datos primarios a la factura.
                //        //var documento_secuencia = db.core_documentosecuencia.FirstOrDefault(x => x.core_documento.codigo == tipo_documento && x.punto_emision_id == pto.id);
                //        ////var documento_secuencia = pto.core_documentosecuencia.FirstOrDefault(x=>x.core_documento.codigo ==tipo_documento);
                //        //Common.GlobalParameters.PuntoEmision = pto.punto_emision; //VF se agregan los datos para uso de f GetSecuenciaValidada 
                //        //Common.GlobalParameters.Establecimiento = pto.establecimiento_id;
                //        //var Secuencia = MainWindow.GetSecuenciaValidada();

                //        //if (documento_secuencia != null)
                //        //{
                //        //    factura.Establecimiento = pto.establecimiento_id;
                //        //    factura.PtoEmision = pto.punto_emision;
                //        //    factura.Secuencia = Secuencia;// documento_secuencia.siguiente;
                //        //    factura.Documento = documento_secuencia.core_documento.codigo;
                //        //    factura.Ip_address = ipAddress;
                //        //    factura.Establecimiento_nombre = pto.core_establecimiento.nombre;
                //        //    factura.Establecimiento_direccion = pto.core_establecimiento.direccion;
                //        //    factura.Establecimiento_telefono = pto.core_establecimiento.telefono;


                //        //    Control.Common.GlobalParameters.EstablecimientoAxCode = pto.core_establecimiento.almacen;
                //        //}
                //        //else
                //        //{

                //        //    //MessageBox.Show("Caja no tiene configurados los secuenciales de documento contacte a administrador!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        //    Control.Common.General.GetMensajeToList(165);
                //        //    result = false;
                //        //}

                //        var detAutorizacion = (from deta in db.pos_autorizacionsri
                //                               where deta.activo
                //                               && hoy >= deta.fecha_inicio && hoy <= deta.fecha_fin
                //                               select deta).ToList();

                //        if (detAutorizacion.Count > 0)
                //        {
                //            var autorizacion = detAutorizacion.FirstOrDefault();
                //            factura.Autorizacion = autorizacion.autorizacion;
                //            factura.Fecha_inicio_autorizacion = autorizacion.fecha_inicio;
                //            factura.Fecha_fin_autorizacion = autorizacion.fecha_fin;

                //        }
                //        //if (db.pos_autorizacionsri.Any(x => x.activo && hoy >= x.fecha_inicio && hoy <= x.fecha_fin ))
                //        //{

                //        //    //var autorizacion = db.core_autorizacionsri.First(x => x.activo && x.fecha_inicio <= hoy && hoy <= x.fecha_fin);                            
                //        //    var autorizacion = db.pos_autorizacionsri.First(x => x.activo && x.fecha_inicio <= hoy && hoy <= x.fecha_fin);
                //        //    factura.Autorizacion = autorizacion.autorizacion;
                //        //    factura.Fecha_inicio_autorizacion = autorizacion.fecha_inicio;
                //        //    factura.Fecha_fin_autorizacion = autorizacion.fecha_fin;

                //        //}
                //        else
                //        {
                //            Control.Common.General.GetMensajeToList(166);
                //            //MessageBox.Show("No tiene autorizacion SRI valida contacte a administrador!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //            result = false;
                //        }


                //        factura.UsaBalanza = pto.usa_balanza;
                //        factura.UsarScannerIntegrado = pto.scanner_integrado;
                //        factura.PuertoBalanza = pto.puerto_balanza;
                //        factura.MarcaBalanza = pto.marca_balanza;
                //        factura.Nombre_sucursal = pto.core_establecimiento.nombre;
                //        factura.Direccion_sucursal = pto.core_establecimiento.direccion;
                //        factura.Direccion_matriz = "KM 5 1/2 VIA DURAN - BABAHOYO";
                //        factura.Razon_social_matriz = "LIRIS S.A";
                //        factura.Ruc_matriz = "0990865477001";
                //        factura.ModeloBalanza = pto.ModeloBalanza;

                //        factura.IpPinPad = pto.IpPinpad;
                //        factura.EstTcpIpPinpad = pto.EsTcipPinpad;
                //        factura.PuertoPinPad = pto.puerto_pinpad;


                //        if (factura.Documento == "F")
                //        {
                //            //* CAMBIO TEMPORAL PARA FACTURACION ELECTRONICA DE GRAN MANZANA *//
                //            /*if (factura.Establecimiento != "012")
                //            {
                //                var recibo = db.core_recibo.Single(x => x.identificador == "FACTURA");
                //                factura.Recibo = recibo.cuerpo;
                //            }*/

                //            var recibo = db.core_recibo.Where(x => x.identificador == (Control.Common.GlobalParameters.ComprobanteFactura + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();
                //            if (recibo == null)
                //                recibo = db.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteFactura).FirstOrDefault();

                //            factura.Recibo = recibo.cuerpo;

                //        }
                //        else if (factura.Documento == "R")
                //        {
                //            var recibo = db.core_recibo.Single(x => x.identificador == "RECIBO_GIFTCARD");
                //            factura.Recibo = recibo.cuerpo;
                //        }


                //    }
                //}

                //// Se busca la apertura por el usuario ya no por ip del equipo.  JM     17-09-2019
                ////if (db.BG_Apertura.Any(x => x.IP == ipAddress && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0))
                //if (db.BG_Apertura.Any(x => x.CEDULA == cedula && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0))
                //{
                //    var pto1 = db.BG_Apertura.Single(x => x.CEDULA == cedula && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0);
                //    //var ID_CAJA = pto1.NUM_CAJA.ToString() + "-" + pto1.FECHA.ToString("yyyy-MM-dd") + "-" + pto1.PUNTO_VENTA + "-" + pto1.TURNO + "-" + pto1.INVENTLOCATION;
                //    Program.ID_Caja_POS = pto1.ID_CAJA;
                //    Control.Common.GlobalParameters.TieneAperturaCajaManual = false;  //eevv 2020-01-08
                //}
                //else
                //{
                //    Control.Common.GlobalParameters.TieneAperturaCajaManual = true;  //eevv 2020-01-08
                //    // Verifica si ya existe una venta con IdCaja manual.   JM  17-09-2019.
                //    if (db.core_factura.Any(x => x.usuario == cedula 
                //            && (DateTime?)System.Data.Entity.DbFunctions.TruncateTime(x.fecha_creacion) == hoy.Date 
                //            && x.msgError.Contains("-MA")))
                //    {


                //        var existe_idcaja = db.core_factura.FirstOrDefault(x => x.usuario == cedula && (DateTime?)System.Data.Entity.DbFunctions.TruncateTime(x.fecha_creacion) == hoy.Date 
                //        && x.msgError.Contains("-MA"));

                //        if (existe_idcaja != null)
                //            Program.ID_Caja_POS = existe_idcaja.msgError;
                //        else {
                //            //Program.ID_Caja_POS = GenereaIdCajaManual(ipAddress);
                //            Program.ID_Caja_POS = "CAJA_ABIERTA_MANUAL";
                //        }
                //    }
                //    else
                //    {
                //        Program.ID_Caja_POS = "CAJA_ABIERTA_MANUAL";
                //        //Program.ID_Caja_POS = GenereaIdCajaManual(ipAddress);
                //    }
                //}
                #endregion

            }
            result = true;
            return result;
        }

        /// <summary>
        /// Valida si tiene Apertura Caja desde AX, si tiene asigna el valor del IdCaja y setea la variable TieneAperturaCajaManual a false.
        /// </summary>
        /// <param name="cedula"></param>
        public static void TieneAperturaCajaAX(string cedula)
        {
            var hoy = DateTime.Now;            
            try
            {
                using (var db = new POSEntities())
                {
                    // Se busca la apertura por el usuario ya no por ip del equipo.  JM     17-09-2019
                    //if (db.BG_Apertura.Any(x => x.IP == ipAddress && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0))
                    if (db.BG_Apertura.Any(x => x.CEDULA == cedula && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0))
                    {
                        var pto1 = db.BG_Apertura.Single(x => x.CEDULA == cedula && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0);
                        //var ID_CAJA = pto1.NUM_CAJA.ToString() + "-" + pto1.FECHA.ToString("yyyy-MM-dd") + "-" + pto1.PUNTO_VENTA + "-" + pto1.TURNO + "-" + pto1.INVENTLOCATION;
                        Program.ID_Caja_POS = pto1.ID_CAJA;
                        Control.Common.GlobalParameters.TieneAperturaCajaManual = false;  //eevv 2020-01-08
                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS", "TieneAperturaCajaAX", "Ocurrió un Error . A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //throw;
            }
        }

        // Genera un IdCaja segun el local y parametros de horarios de turno.   JM  17-09-2019
        public static string GenereaIdCajaManual(string ip)
        {
            try
            {
                using (var db = new POSEntities())
                {
                    DateTime hoy = DateTime.Now;
                    int num_caja = 99;
                    var pto = db.view_Tbl_Punto_Venta.FirstOrDefault(x => x.IP == ip);
                    if (db.BG_Apertura.Where(x => x.INVENTLOCATION == pto.INVENTLOCATION && x.FECHA == hoy.Date).Any())
                    {
                         num_caja = (db.BG_Apertura.Where(x => x.INVENTLOCATION == pto.INVENTLOCATION && x.FECHA == hoy.Date).Max(m => (int?)m.NUM_CAJA) ?? 0) + 1;
                    }


                    // valida turno de cajero ===============
                    //string t = "05:00:00", t2 = "14:00:01", t3 = "14:00:01", t4 = "23:59:00", t5 = "12:00:01";
                    TimeSpan t = TimeSpan.Parse("05:00:00"), t2 = TimeSpan.Parse("14:00:01"), t3 = TimeSpan.Parse("14:00:01"),
                            t4 = TimeSpan.Parse("23:59:00"), t5 = TimeSpan.Parse("12:00:01");
                    string turno = "", idCaja = "";
                    TimeSpan dt = DateTime.Now.TimeOfDay;

                    if (dt >= t && dt < t2)
                    {
                        turno = "01";
                    }

                    if (dt >= t3 && dt < t4)
                    {
                        turno = "02";
                    }

                    //========================================================================================================
                    // valida turno de apertura de cajas PPG
                    if (pto.INVENTLOCATION == "ARDP-0004")
                    {

                        t = TimeSpan.Parse("05:00:00");
                        t2 = TimeSpan.Parse("12:00:01");
                        t3 = TimeSpan.Parse("12:00:01");
                        t4 = TimeSpan.Parse("23:59:00");

                        if (dt >= t && dt < t2)
                        {
                            turno = "01";
                        }

                        if (dt >= t3 && dt < t4)
                        {
                            turno = "02";
                        }
                    }

                    idCaja = num_caja + "-" + hoy.ToString("yyyy-MM-dd") + "-" + pto.PUNTO_VENTA + "-" + turno + "-" + pto.INVENTLOCATION + "-MA";
                    return idCaja;
                }
            }
            catch (Exception ex )
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS", "GenereaIdCajaManual", "Ocurrio un problema, el id_caja sera: CAJA_ABIERTA_MANUAL" + ex.InnerException);
                return "CAJA_ABIERTA_MANUAL";
            }
        }

        /// <summary>
        /// Se actualiza la secuencia del tipo documento con punto de emisión.
        /// </summary>
        /// <param name="factura">factura que se acaba de grabar</param>
        /// <returns></returns>
        public static bool actualizarSecuencia(Factura factura)
        {
            var result = false;
            long secuencia1, secuenciaMSQUEUELocal;
            //long secuenciaActual=0;
            using (var db = new POSEntities())
            {
                // Tomar el max secuencial utilizado para verificar que numero de secuencia este correcto
                int maxNroFactura = db.core_factura.Where(x => x.establecimiento == Control.Common.GlobalParameters.Establecimiento
                                                                    && x.punto_emision == Control.Common.GlobalParameters.PuntoEmision)
                                                       .Max(x => (int?)x.numero) ?? 0;
                secuenciaMSQUEUELocal = Control.POS.validaSecuencialMQ("F");

                var secuencia =db.core_documentosecuencia.Single(x => x.core_puntoemision.establecimiento_id == factura.Establecimiento && x.core_puntoemision.punto_emision == factura.PtoEmision && x.core_documento.codigo == factura.Documento);
       
                if (secuenciaMSQUEUELocal> secuencia.siguiente)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "actualizarSecuencia", "La secuencia de la cola de mensajes '"+ secuenciaMSQUEUELocal + "' es mayor a la secuencia de la tabla core_documentosecuencia '"+ secuencia.siguiente.ToString() + "', se procederá a actualizar la secuencia con el valor de la cola de mensaje.");
                    secuencia1 = secuenciaMSQUEUELocal;
                }
                else if((maxNroFactura + 1)> secuencia.siguiente)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "actualizarSecuencia", "ultima secuencia de las facturas '"+ maxNroFactura.ToString() + "' es mayor a la secuencia de la tabla core_documentosecuencia '" + secuencia.siguiente.ToString() + "', se procederá a actualizar la secuencia con el valor de la ultima secuencia+1 de la tabla de factura ('"+ (maxNroFactura+1).ToString() + "').");
                    secuencia1 = (maxNroFactura + 1);
                }
                else
                {
                    secuencia1 = secuencia.siguiente+1;
                }

                secuencia.siguiente= secuencia1;
                db.SaveChanges();


                try
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "actualizarSecuencia", "Antes de actualizar Secuencial en la cola de mensajes");
                    ClsMessageQueue.receiveMessageQueue("F", factura.Establecimiento, factura.PtoEmision);
                    ClsMessageQueue.setMessageQueue(factura.Establecimiento, factura.PtoEmision, secuencia1.ToString(), "F");//evelasco se graba el secuencial del documento en el message queue del S.O.
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "actualizarSecuencia", "La cola de mensaje se actualizó con el valor de "+ secuencia.siguiente.ToString());
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "actualizarSecuencia", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                }
                
                result = true;
            }
            return result;
        }

        public static bool actualizarSecuenciaNC(Factura factura)
        {
            var result = false;
            using (var db = new POSEntities())
            {
                var secuencia = db.core_documentosecuencia.Single(x => x.core_puntoemision.establecimiento_id == factura.Establecimiento && x.core_puntoemision.punto_emision == factura.PtoEmision && x.core_documento.codigo == "NC");
                secuencia.siguiente++;
                db.SaveChanges();

                try
                {
                    ClsMessageQueue.receiveMessageQueue("F", factura.Establecimiento, factura.PtoEmision);
                    ClsMessageQueue.setMessageQueue(factura.Establecimiento, factura.PtoEmision, secuencia.siguiente.ToString(), "F");//evelasco se graba el secuencial del documento en el message queue del S.O.
                }
                catch (Exception  ex1)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSecuencialFactura", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex1));
                }

                result = true;
            }
            return result;
        }




        public static string obtenerSecuenciaNC(Factura factura)
        {
            //long secuencia = 0;
            long secuenciaMSQUEUELocal = 0;
            long secuencia1 = 0;
            var secuencia = new core_documentosecuencia();
            long maxNroNC = 0;
            string docx = "02901100000002351";
            string a, b, c;
            a = docx.Substring(0, 3);
            b= docx.Substring(3, 3);

            using (var db = new POSEntities())
            {
                 secuencia = db.core_documentosecuencia.Single(x => x.core_puntoemision.establecimiento_id == factura.Establecimiento && x.core_puntoemision.punto_emision == factura.PtoEmision && x.core_documento.codigo == "NC");

                //Tomar el max secuencial utilizado para verificar que numero de secuencia este correcto
                int maxIDNroNC = db.core_notacredito.Where(x => x.numdocumento.Substring(0,3) == Control.Common.GlobalParameters.Establecimiento
                                                                && x.numdocumento.Substring(3, 3) == Control.Common.GlobalParameters.PuntoEmision)
                                                   .Max(x => (int?)x.id) ?? 0;
                if (maxIDNroNC>0)
                { 
                    string numDoc = db.core_notacredito.Where(x =>x.id == maxIDNroNC).FirstOrDefault().numdocumento;
                    maxNroNC = Convert.ToInt64(numDoc.Substring(7));
                }

                //evelasco  obtengo la secuencia del MessageQueue del SO del equipo POS.
                secuenciaMSQUEUELocal = validaSecuencialMQ("NC");
                if (secuenciaMSQUEUELocal == 0)
                {

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "obtenerSecuenciaNC", "No se encontró el valor del secuencial del MSQUEUE." + Environment.NewLine);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "obtenerSecuenciaNC", "Graba secuencial de la tabla documento_secuencia en la cola de mensajes MSQUEUE del Equipo Local.");

                    secuencia1 = secuencia.siguiente;
                    if ((maxNroNC + 1) > secuencia.siguiente)
                    {
                        secuencia1 = (maxNroNC + 1);
                    }

                    ClsMessageQueue.setMessageQueue(Control.Common.GlobalParameters.Establecimiento,Control.Common.GlobalParameters.PuntoEmision, secuencia1.ToString(), "NC");

                }

                if ((secuenciaMSQUEUELocal) > secuencia.siguiente)
                {
                    secuencia1 = secuenciaMSQUEUELocal;
                    secuencia.siguiente = secuencia1;
                    db.SaveChanges();
                }
                else if ((maxNroNC + 1) > secuencia.siguiente)  //Corregir el secuencial de factura de ser necesario
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Se detectó secuencial de facturación incorrecto. Se intentará realizar el ajuste de secuencial" + Environment.NewLine +
                                                                        "Secuencial incorrecto: " + secuencia.siguiente.ToString() + Environment.NewLine +
                                                                        "Secuencial que se va dar: " + (maxNroNC + 1).ToString() + Environment.NewLine +
                                                                        "DireccionIp caja: " + Control.Common.GlobalParameters.IpMaquina + Environment.NewLine +
                                                                        "DireccionIp servidor: " + Control.Common.GlobalParameters.SelectedServerIp + Environment.NewLine +
                                                                        "Usuario Id: " + Control.Common.GlobalParameters.Usuario +
                                                                        "Usuario Nombre: " + Control.Common.GlobalParameters.UsuarioNombre
                                                                        );

                    secuencia1 = maxNroNC + 1;
                    secuencia.siguiente = secuencia1;
                    db.SaveChanges();

                    //crea la cola de mensaje para solocar el nuevo valor del secuencial
                    try
                    {
                        ClsMessageQueue.receiveMessageQueue("NC", Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision);
                        ClsMessageQueue.setMessageQueue(Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision, secuencia.siguiente.ToString(), "NC");
                    }
                    catch (Exception ex1)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSecuencialFactura", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex1));
                    }

                }
                else
                {
                    secuencia1 = secuencia.siguiente;
                    //crea la cola de mensaje para solocar el nuevo valor del secuencial
                    try
                    {
                        ClsMessageQueue.receiveMessageQueue("NC", Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision);
                        ClsMessageQueue.setMessageQueue(Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision, secuencia.siguiente.ToString(), "NC");
                    }
                    catch (Exception ex2)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSecuencialFactura", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex2));
                    }
                }



            }
            return factura.Establecimiento + "-" + factura.PtoEmision + "-" + secuencia.siguiente.ToString("000000000.##");
        }



        public static long GetSecuenciaNC(Factura factura)
        {
            //long secuencia = 0;
            long secuenciaMSQUEUELocal = 0;
            long secuencia1 = 0;
            var secuencia = new core_documentosecuencia();
            long maxNroNC = 0;
            string docx = "02901100000002351";
            string a, b, c;
            a = docx.Substring(0, 3);
            b = docx.Substring(3, 3);

            using (var db = new POSEntities())
            {
                secuencia = db.core_documentosecuencia.Single(x => x.core_puntoemision.establecimiento_id == factura.Establecimiento && x.core_puntoemision.punto_emision == factura.PtoEmision && x.core_documento.codigo == "NC");

                //Tomar el max secuencial utilizado para verificar que numero de secuencia este correcto
                int maxIDNroNC = db.core_notacredito.Where(x => x.numdocumento.Substring(0, 3) == Control.Common.GlobalParameters.Establecimiento
                                                                && x.numdocumento.Substring(3, 3) == Control.Common.GlobalParameters.PuntoEmision)
                                                   .Max(x => (int?)x.id) ?? 0;
                if (maxIDNroNC > 0)
                {
                    string numDoc = db.core_notacredito.Where(x => x.id == maxIDNroNC).FirstOrDefault().numdocumento;
                    maxNroNC = Convert.ToInt64(numDoc.Substring(7));
                }

                //evelasco  obtengo la secuencia del MessageQueue del SO del equipo POS.
                secuenciaMSQUEUELocal = validaSecuencialMQ("NC");
                if (secuenciaMSQUEUELocal == 0)
                {

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "obtenerSecuenciaNC", "No se encontró el valor del secuencial del MSQUEUE." + Environment.NewLine);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "obtenerSecuenciaNC", "Graba secuencial de la tabla documento_secuencia en la cola de mensajes MSQUEUE del Equipo Local.");

                    secuencia1 = secuencia.siguiente;
                    if ((maxNroNC + 1) > secuencia.siguiente)
                    {
                        secuencia1 = (maxNroNC + 1);
                    }

                    ClsMessageQueue.setMessageQueue(Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision, secuencia1.ToString(), "NC");

                }

                if ((secuenciaMSQUEUELocal) > secuencia.siguiente)
                {
                    secuencia1 = secuenciaMSQUEUELocal;
                    secuencia.siguiente = secuencia1;
                    db.SaveChanges();
                }
                else if ((maxNroNC + 1) > secuencia.siguiente)  //Corregir el secuencial de factura de ser necesario
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Se detectó secuencial de facturación incorrecto. Se intentará realizar el ajuste de secuencial" + Environment.NewLine +
                                                                        "Secuencial incorrecto: " + secuencia.siguiente.ToString() + Environment.NewLine +
                                                                        "Secuencial que se va dar: " + (maxNroNC + 1).ToString() + Environment.NewLine +
                                                                        "DireccionIp caja: " + Control.Common.GlobalParameters.IpMaquina + Environment.NewLine +
                                                                        "DireccionIp servidor: " + Control.Common.GlobalParameters.SelectedServerIp + Environment.NewLine +
                                                                        "Usuario Id: " + Control.Common.GlobalParameters.Usuario +
                                                                        "Usuario Nombre: " + Control.Common.GlobalParameters.UsuarioNombre
                                                                        );

                    secuencia1 = maxNroNC + 1;
                    secuencia.siguiente = secuencia1;
                    db.SaveChanges();

                    //crea la cola de mensaje para solocar el nuevo valor del secuencial
                    try
                    {
                        ClsMessageQueue.receiveMessageQueue("NC", Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision);
                        ClsMessageQueue.setMessageQueue(Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision, secuencia.siguiente.ToString(), "NC");
                    }
                    catch (Exception ex1)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSecuencialFactura", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex1));
                    }

                }
                else
                {
                    secuencia1 = secuencia.siguiente;
                    //crea la cola de mensaje para solocar el nuevo valor del secuencial
                    try
                    {
                        ClsMessageQueue.receiveMessageQueue("NC", Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision);
                        ClsMessageQueue.setMessageQueue(Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision, secuencia.siguiente.ToString(), "NC");
                    }
                    catch (Exception ex2)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSecuencialFactura", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex2));
                    }
                }



            }
            return secuencia.siguiente;
        }


        public static bool HasPayType(string type)
        {
            foreach (Form form in Application.OpenForms)
                if (form.GetType().Name == typeof(MainWindow).Name)
                    return ((MainWindow)form).HasPayTipe(type);

            return false;
        }

        public static long validaSecuencialMQ(string tipoDocumento)
        {
            long secuencia = 0;
            ClsFacturaMQExcept factMQ;
            string tipoDoc = string.Empty;
            try
            {

                if (tipoDocumento.Equals("F"))
                {
                    tipoDoc = "Factura";
                }
                if (tipoDocumento.Equals("NC"))
                {
                    tipoDoc = "Nota de Crédito";
                }

                factMQ = ClsMessageQueue.getMessageQueue(tipoDocumento, Control.Common.GlobalParameters.Establecimiento, Control.Common.GlobalParameters.PuntoEmision);
                if (factMQ != null)
                {

                    if (factMQ.tipoException == TipoQueueExcepcion.NINGUNO)
                    {
                        //obtengo la secuencia del MSQUEUE del equipo.
                        secuencia = Convert.ToInt64(factMQ.Secuencia);
                    }
                    if (factMQ.tipoException == TipoQueueExcepcion.MSQNOINSTALADO)
                    {
                        EnvioMailError("Caja no tiene instalado MS MESSAGEQUEUE, contactese con el departamento de Sistemas para la instalación.",
                                    "El POS no pudo obtener la numeración de la "+ tipoDoc+" del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    "Caja no tiene instalado MS MESSAGEQUEUE. " + Environment.NewLine + factMQ._mqExc.Message,
                                    "validaSecuencialMQ"
                                    );

                    }
                    if (factMQ.tipoException == TipoQueueExcepcion.QUEUENOENCONTRADO)
                    {
                        EnvioMailError("Caja no tiene configurado MS MESSAGEQUEUE, contactese con el departamento de Sistemas.",
                                    "El POS no pudo obtener la numeración de la " + tipoDoc + " del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    "El nombre de la cola '" + factMQ.QueueName + "' no fue encontrado. " + Environment.NewLine + factMQ._mqExc.Message,
                                    "validaSecuencialMQ"
                                    );

                    }
                    if (factMQ.tipoException == TipoQueueExcepcion.TIMEOUT)
                    {
                        EnvioMailError("Caja no pudo obtener la Cola de Mensaje(MS MESSAGEQUEUE), contactese con el departamento de Sistemas.",
                                    "El POS no pudo obtener la numeración de la " + tipoDoc + " del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    "expiró el tiempo de respuesta para obtener la cola de mensaje '" + factMQ.QueueName + "'. " + Environment.NewLine + factMQ._mqExc.Message,
                                    "validaSecuencialMQ"
                                    );

                    }

                    if (factMQ.tipoException == TipoQueueExcepcion.NODISPONIBLE)
                    {
                        EnvioMailError("Caja no pudo obtener la Cola de Mensaje(MS MESSAGEQUEUE), contactese con el departamento de Sistemas.",
                                    "El POS no pudo obtener la numeración de la " + tipoDoc + " del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    "La cola de mensaje '" + factMQ.QueueName + "' no se encuentra disponible. " + Environment.NewLine + factMQ._mqExc.Message,
                                    "validaSecuencialMQ"
                                    );

                    }

                }
                else
                {
                    EnvioMailError("Caja no tiene configurado MS MESSAGEQUEUE, contactese con el departamento de Sistemas.",
                                    "El POS no pudo obtener la numeración de la " + tipoDoc + " del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    "no se enconstró registro en el MessageQueue",
                                    "validaSecuencialMQ"
                                    );

                }

            }
            catch (MessageQueueException exMQ)
            {

                EnvioMailError("Caja no tiene configurado MS MESSAGEQUEUE, contactese con el departamento de Sistemas.",
                                "El POS no pudo obtener la numeración de la " + tipoDoc + " del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                Control.Common.ExceptionHandler.GetExceptionMessages(exMQ),
                                "validaSecuencialMQ"
                              );
            }

            return secuencia;
        }

        private static void EnvioMailError(string message, string message1, string messageException, string metodoInvocado)
        {
            try
            {
                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                Properties.Settings.Default.MAILERROR_FROM,
                Properties.Settings.Default.MAILERROR_ALIAS,
                Properties.Settings.Default.MAILERROR_DESTINO,
                Properties.Settings.Default.MAILERROR_CC,
                message,//"Caja no tiene configurado MS MESSAGEQUEUE, contactese con el departamento de Sistemas.",
                String.Format(message1,//"El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                            Control.Common.GlobalParameters.Establecimiento,
                            Control.Common.GlobalParameters.PuntoEmision,
                            Control.Common.GlobalParameters.IpMaquina,
                            Control.Common.GlobalParameters.UsuarioNombre,
                            Control.Common.GlobalParameters.Usuario,
                            
                messageException),
                false,
                String.Empty);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", metodoInvocado, String.Format("El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                            Control.Common.GlobalParameters.Establecimiento,
                            Control.Common.GlobalParameters.PuntoEmision,
                            Control.Common.GlobalParameters.IpMaquina,
                            Control.Common.GlobalParameters.UsuarioNombre,
                            Control.Common.GlobalParameters.Usuario,
                messageException));

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", metodoInvocado, "No se pudo enviar notificacion del problema de caja para obtener ip del servidor sql, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }

            }
            catch (Exception exMail)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", metodoInvocado, "Excepcion grave al llamar a la clase de envio de email, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exMail));
            }
        }

    }
}
