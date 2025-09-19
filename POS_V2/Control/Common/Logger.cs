using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace POS.Control.Common
{
    public static class Logger
    {
        private static LirisLibLogger.Logger Log = new LirisLibLogger.Logger();

        #region Metodos Grabar Log Queries
        public static void Graba_TraceFile(string line, string traceFileSetting)
        {
            try
            {
                string traceFileRoute = traceFileSetting;
                traceFileRoute = traceFileRoute.Replace("|dd", DateTime.Now.ToString("dd"));
                traceFileRoute = traceFileRoute.Replace("|MM", DateTime.Now.ToString("MM"));
                traceFileRoute = traceFileRoute.Replace("|yyyy", DateTime.Now.ToString("yyyy"));
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(traceFileRoute));
                string path = Path.Combine(directoryInfo.FullName, Path.GetFileName(traceFileRoute));
                bool existsDirectory = directoryInfo.Exists;
                if (!existsDirectory)
                {
                    directoryInfo.Create();
                }
                FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
                TextWriterTraceListener listener = new TextWriterTraceListener(fileStream);
                Trace.Listeners.Add(listener);
                Trace.WriteLine(string.Concat(new string[]
                {
                        line
                }));
                Trace.Flush();
                Trace.Close();
                fileStream.Close();
            }
            catch (Exception)
            {
            }
        }

        private static List<string> TraceList = new List<string>();

        public static void Limpiar_Trace()
        {
            TraceList = new List<string>();
        }

        public static void Agregar_Trace(string lineTrace, bool indexAtZero = false)
        {
            if (indexAtZero)
                TraceList.Insert(0, lineTrace);
            else
                TraceList.Add(lineTrace);
        }

        public static void Graba_TraceFile()
        {
            try
            {
                bool isNewFile = false;
                string traceFileRoute = Properties.Settings.Default.EF_LOG;
                string traceFilePath = Properties.Settings.Default.EFLOG_PATH;
                traceFileRoute = traceFileRoute.Replace("|dd", DateTime.Now.ToString("dd"));
                traceFileRoute = traceFileRoute.Replace("|MM", DateTime.Now.ToString("MM"));
                traceFileRoute = traceFileRoute.Replace("|yyyy", DateTime.Now.ToString("yyyy"));
                string path = Path.Combine(traceFilePath, traceFileRoute);
                isNewFile = !File.Exists(path);
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
                bool existsDirectory = directoryInfo.Exists;
                if (!existsDirectory)
                {
                    directoryInfo.Create();
                }
                FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
                TextWriterTraceListener listener = new TextWriterTraceListener(fileStream);
                Trace.Listeners.Add(listener);
                //If request creates new file, fill it first with special traces            
                if (isNewFile)
                {
                    var InitialList = GetInitialListForNewFile();
                    if (InitialList != null)
                    {
                        foreach (string trace in InitialList)
                        {
                            Trace.WriteLine(string.Concat(new string[]
                            {
                                trace
                            }));
                        }
                    }
                }
                //Save all the traces in queue
                foreach (string trace in TraceList)
                {
                    Trace.WriteLine(string.Concat(new string[]
                    {
                        trace
                    }));
                }
                Trace.Flush();
                Trace.Close();
                fileStream.Close();

                Limpiar_Trace();
            }
            catch (Exception ex)
            {
            }
        }

        private static List<string> GetInitialListForNewFile()
        {
            List<string> InitialList = new List<string>();
            InitialList.Add("DECLARE @factura_id INT");
            InitialList.Add("DECLARE @notacredito_id INT");

            return InitialList;
        }

        #endregion

        #region Metodos Log LirisLibLogger

        public static void LogMessage(Common.Enum.LogTypes logType, string clase, string metodo, string msj, string infoAdicional = null)
        {
            var texto = string.Format("Clase: {0} |Método: {1} |Mensaje: {2} {3}",
                                                            clase,
                                                            metodo,
                                                            msj,
                                                            (string.IsNullOrEmpty(infoAdicional)) ? "" : "|" + infoAdicional);
            switch (logType)
            {
                case Enum.LogTypes.Debug:
                    Log.Graba_Log_Debug(texto);
                    break;
                case Enum.LogTypes.Info:
                    Log.Graba_Log_Info(texto);
                    break;
                case Enum.LogTypes.Warn:
                case Enum.LogTypes.Warning:
                    Log.Graba_Log_Warn(texto);
                    break;
                case Enum.LogTypes.Error:
                    Log.Graba_Log_Error(texto);
                    break;
                case Enum.LogTypes.Fatal:
                    Log.Graba_Log_Fatal(texto);
                    break;
            }
        }

        #endregion

        #region Metodos Trace

        public static void Agregar_Trace_Factura(Models.core_factura entidad)
        {
            //Preparar Logger
            Limpiar_Trace();

            Agregar_Trace("INSERT INTO [dbo].[core_factura](" +
                        "[fecha_creacion]" +
                        ",[fecha_modificacion]" +
                        ",[establecimiento]" +
                        ",[punto_emision]" +
                        ",[numero]" +
                        ",[documento]" +
                        ",[cliente_ax]" +
                        ",[cliente]" +
                        ",[razon_social]" +
                        ",[direccion]" +
                        ",[base0]" +
                        ",[base12]" +
                        ",[subtotal]" +
                        ",[descuento]" +
                        ",[descuento2]" +
                        ",[iva]" +
                        ",[total]" +
                        ",[autorizacion]" +
                        ",[ip]" +
                        ",[usuario]" +
                        ",[orderID]" +
                        ",[loadinvoice]" +
                        ",[loadpaym]" +
                        ",[msgError]" +
                        ",[paymID]" +
                        ",[descuentoPorc])" +
                        " VALUES " +
                        "('" + entidad.fecha_creacion.ToString("s") + "'" +
                        ",'" + entidad.fecha_modificacion.ToString("s") + "'" +
                        ",'" + entidad.establecimiento + "'" +
                        ",'" + entidad.punto_emision + "'" +
                        "," + entidad.numero +
                        ",'" + entidad.documento + "'" +
                        ",'" + entidad.cliente_ax + "'" +
                        ",'" + entidad.cliente + "'" +
                        ",'" + entidad.razon_social.Replace("'", "''") + "'" +
                        "," + ((entidad.direccion == null) ? "NULL" : String.Format("'{0}'", entidad.direccion.Replace("'", "''"))) +
                        "," + entidad.base0 +
                        "," + entidad.base12 +
                        "," + entidad.subtotal +
                        "," + entidad.descuento +
                        "," + entidad.descuento2 +
                        "," + entidad.iva +
                        "," + entidad.total +
                        ",'" + entidad.autorizacion + "'" +
                        ",'" + entidad.ip + "'" +
                        ",'" + entidad.usuario + "'" +
                        "," + ((entidad.orderID == null) ? "NULL" : String.Format("'{0}'", entidad.orderID)) +
                        ",'" + entidad.loadinvoice + "'" +
                        ",'" + entidad.loadpaym + "'" +
                        ",'" + entidad.msgError + "'" +
                        "," + ((entidad.paymID == null) ? "NULL" : String.Format("'{0}'", entidad.paymID)) +
                        "," + entidad.descuentoPorc +
                        ")");

            //Agregar linea de SCOPE_IDENTITY()
            Agregar_Trace("SET @factura_id = scope_identity()");

            //Agregar lineas de detalle
            foreach (var det in entidad.core_facturadetalle)
            {
                Agregar_Trace_FacturaDetalle(det);
            }

            //Agregar lineas de pago
            foreach (var pago in entidad.core_facturapago)
            {
                Agregar_Trace_FacturaPago(pago);
            }

            //Agregar lineas de retenciones
            foreach (var retencion in entidad.core_retencion)
            {
                Agregar_Trace_FacturaRetencion(retencion);
            }

            Graba_TraceFile();
        }

        public static void Agregar_Trace_FacturaDetalle(Models.core_facturadetalle entidad)
        {
            Agregar_Trace("INSERT INTO [dbo].[core_facturadetalle](" +
                        "[fecha_creacion]" +
                        ",[fecha_modificacion]" +
                        ",[factura_id]" +
                        ",[linea]" +
                        ",[item_id]" +
                        ",[item_nombre]" +
                        ",[cantidad]" +
                        ",[unidades]" +
                        ",[subtotal]" +
                        ",[descuento]" +
                        ",[iva]" +
                        ",[total]" +
                        ",[unidad]" +
                        ",[costo]" +
                        ",[precio])" +
                        " VALUES " +
                        "('" + entidad.fecha_creacion.ToString("s") + "'" +
                        ",'" + entidad.fecha_modificacion.ToString("s") + "'" +
                        ",@factura_id" +
                        "," + entidad.linea +
                        ",'" + entidad.item_id + "'" +
                        ",'" + entidad.item_nombre.Replace("'", "''") + "'" +
                        "," + entidad.cantidad +
                        "," + entidad.unidades +
                        "," + entidad.subtotal +
                        "," + entidad.descuento +
                        "," + entidad.iva +
                        "," + entidad.total +
                        ",'" + entidad.unidad + "'" +
                        "," + entidad.costo +
                        "," + entidad.precio +
                        ")");
        }

        public static void Agregar_Trace_FacturaPago(Models.core_facturapago entidad)
        {
            Agregar_Trace("INSERT INTO [dbo].[core_facturapago](" +
                            "[fecha_creacion]" +
                            ",[fecha_modificacion]" +
                            ",[factura_id]" +
                            ",[tipo_id]" +
                            ",[valor]" +
                            ",[cliente]" +
                            ",[datos]" +
                            ",[JournalNum]" +
                            ",[voucher])" +
                            " VALUES " +
                            "('" + entidad.fecha_creacion.ToString("s") + "'" +
                            ",'" + entidad.fecha_modificacion.ToString("s") + "'" +
                            ",@factura_id" +
                            ",'" + entidad.tipo_id + "'" +
                            "," + entidad.valor +
                            "," + ((entidad.cliente == null) ? "NULL" : String.Format("'{0}'", entidad.cliente)) +
                            ",'" + entidad.datos + "'" +
                            "," + ((entidad.JournalNum == null) ? "NULL" : String.Format("'{0}'", entidad.JournalNum)) +
                            "," + ((entidad.voucher == null) ? "NULL" : String.Format("'{0}'", entidad.voucher)) +
                            ")");
        }

        public static void Agregar_Trace_FacturaRetencion(Models.core_retencion entidad)
        {
            Agregar_Trace("INSERT INTO [dbo].[core_retencion](" +
                            "[fecha_creacion]" +
                            ",[fecha_modificacion]" +
                            ",[fecha_autorizacion]" +
                            ",[factura_id]" +
                            ",[num_retencion]" +
                            ",[num_autorizacion]" +
                            ",[num_factura]" +
                            ",[valor_ret_fte]" +
                            ",[valor_base]" +
                            ",[CodRetIVA]" +
                            ",[CodPorcRetIVA]" +
                            ",[ValorBaseIVA]" +
                            ",[ValorRetIVA]" +
                            ",[ConceptoRetIVA])" +
                            " VALUES " +
                            "('" + entidad.fecha_creacion.ToString("s") + "'" +
                            ",'" + entidad.fecha_modificacion.ToString("s") + "'" +
                            ",'" + entidad.fecha_autorizacion.ToString("s") + "'" +
                            ",@factura_id" +
                            ",'" + entidad.num_retencion + "'" +
                            ",'" + entidad.num_autorizacion + "'" +
                            ",'" + entidad.num_factura + "'" +
                            "," + entidad.valor_ret_fte +
                            "," + entidad.valor_base +
                            ",'" + entidad.CodRetIVA + "'" +
                            "," + entidad.CodPorcRetIVA +
                            "," + entidad.ValorBaseIVA +
                            "," + entidad.ValorRetIVA +
                            ",'" + entidad.ConceptoRetIVA + "'" +
                            ")");
        }

        public static void Agregar_Trace_Voucher(Models.POS_VOUCHER entidad)
        {
            //Preparar Logger
            Limpiar_Trace();

            Agregar_Trace("INSERT INTO [dbo].[POS_VOUCHER]" +
                                    "([TARJETA]" +
                                    ",[CODIGOPROCESO]" +
                                    ",[FECHACONSUMO]" +
                                    ",[HORACONSUMO]" +
                                    ",[NUMEROVOUCHER]" +
                                    ",[AUTORIZACION]" +
                                    ",[VALORCONSUMO]" +
                                    ",[FORMAAUTORIZA]" +
                                    ",[TIPOCONSUMO]" +
                                    ",[PLAZO]" +
                                    ",[TIPOLECTURA]" +
                                    ",[TIPOMONEDA]" +
                                    ",[VALORIVA]" +
                                    ",[VALORSERVICIO]" +
                                    ",[VALORPROPINA]" +
                                    ",[VALORINTERES]" +
                                    ",[VALORFIJO]" +
                                    ",[TIPOPROMOCION]" +
                                    ",[MESESGRACIA]" +
                                    ",[EMPRESASERVICIO]" +
                                    ",[ESTADOTRX]" +
                                    ",[CODIGORESPUESTA]" +
                                    ",[TIPODISPOSITIVO]" +
                                    ",[ADQUIRENTETARJETA]" +
                                    ",[ADQUIRENTESERVICIO]" +
                                    ",[MONTOGRAVAIVA]" +
                                    ",[MONTONOGRAVAIVA]" +
                                    ",[PUNTOEMISION]" +
                                    ",[PROCESADO]" +
                                    ",[GRUPOTAR]" +
                                    ",[AUTORIZADOR]" +
                                    ",[LOTE]" +
                                    ",[ANULADO]" +
                                    ",[PROCESADOTURNO]" +
                                    ",[FACTURA]" +
                                    ",[ARQC]" +
                                    ",[AIDEMV]" +
                                    ",[EMV]" +
                                    ",[TC]" +
                                    ",[PUBLICIDAD]" +
                                    ",[TIPOTRANSACCION]" +
                                    ",[BANCOADQUIRIENTE]" +
                                    ",[TARJETAHABIENTE]" +
                                    ",[MID]" +
                                    ",[TID]" +
                                    ",[VENCTAR]" +
                                    ",[ANULAUTORIZACION]" +
                                    ",[TIPOBANCOTARJETA])" +
                                    " VALUES " +
                                    "('" + entidad.TARJETA + "'" +
                                    ",'" + entidad.CODIGOPROCESO + "'" +
                                    ",'" + entidad.FECHACONSUMO + "'" +
                                    ",'" + entidad.HORACONSUMO + "'" +
                                    ",'" + entidad.NUMEROVOUCHER + "'" +
                                    ",'" + entidad.AUTORIZACION + "'" +
                                    ",'" + entidad.VALORCONSUMO + "'" +
                                    ",'" + entidad.FORMAAUTORIZA + "'" +
                                    ",'" + entidad.TIPOCONSUMO + "'" +
                                    ",'" + entidad.PLAZO + "'" +
                                    ",'" + entidad.TIPOLECTURA + "'" +
                                    ",'" + entidad.TIPOMONEDA + "'" +
                                    ",'" + entidad.VALORIVA + "'" +
                                    ",'" + entidad.VALORSERVICIO + "'" +
                                    ",'" + entidad.VALORPROPINA + "'" +
                                    ",'" + entidad.VALORINTERES + "'" +
                                    ",'" + entidad.VALORFIJO + "'" +
                                    ",'" + entidad.TIPOPROMOCION + "'" +
                                    ",'" + entidad.MESESGRACIA + "'" +
                                    ",'" + entidad.EMPRESASERVICIO + "'" +
                                    ",'" + entidad.ESTADOTRX + "'" +
                                    ",'" + entidad.CODIGORESPUESTA + "'" +
                                    ",'" + entidad.TIPODISPOSITIVO + "'" +
                                    ",'" + entidad.ADQUIRENTETARJETA + "'" +
                                    ",'" + entidad.ADQUIRENTESERVICIO + "'" +
                                    ",'" + entidad.MONTOGRAVAIVA + "'" +
                                    ",'" + entidad.MONTONOGRAVAIVA + "'" +
                                    ",'" + entidad.PUNTOEMISION + "'" +
                                    "," + ((entidad.PROCESADO) ? 1 : 0) +
                                    ",'" + entidad.GRUPOTAR + "'" +
                                    "," + entidad.AUTORIZADOR +
                                    ",'" + entidad.LOTE + "'" +
                                    "," + ((entidad.ANULADO) ? 1 : 0) +
                                    "," + ((entidad.PROCESADOTURNO) ? 1 : 0) +
                                    ",'" + entidad.FACTURA + "'" +
                                    "," + ((entidad.ARQC == null) ? "NULL" : String.Format("'{0}'", entidad.ARQC)) +
                                    "," + ((entidad.AIDEMV == null) ? "NULL" : String.Format("'{0}'", entidad.AIDEMV)) +
                                    "," + ((entidad.EMV == null) ? "NULL" : String.Format("'{0}'", entidad.EMV)) +
                                    "," + ((entidad.TC == null) ? "NULL" : String.Format("'{0}'", entidad.TC)) +
                                    "," + ((entidad.PUBLICIDAD == null) ? "NULL" : String.Format("'{0}'", entidad.PUBLICIDAD)) +
                                    "," + ((entidad.TIPOTRANSACCION == null) ? "NULL" : String.Format("'{0}'", entidad.TIPOTRANSACCION)) +
                                    "," + ((entidad.BANCOADQUIRIENTE == null) ? "NULL" : String.Format("'{0}'", entidad.BANCOADQUIRIENTE)) +
                                    "," + ((entidad.TARJETAHABIENTE == null) ? "NULL" : String.Format("'{0}'", entidad.TARJETAHABIENTE)) +
                                    "," + ((entidad.MID == null) ? "NULL" : String.Format("'{0}'", entidad.MID)) +
                                    "," + ((entidad.TID == null) ? "NULL" : String.Format("'{0}'", entidad.TID)) +
                                    "," + ((entidad.VENCTAR == null) ? "NULL" : String.Format("'{0}'", entidad.VENCTAR)) +
                                    "," + ((entidad.ANULAUTORIZACION == null) ? "NULL" : String.Format("'{0}'", entidad.ANULAUTORIZACION)) +
                                    "," + ((entidad.TIPOBANCOTARJETA == null) ? "NULL" : String.Format("'{0}'", entidad.TIPOBANCOTARJETA)) +
                                    ")");

            Graba_TraceFile();
        }

        public static void Agregar_Trace_NotaCredito(Models.core_notacredito entidad)
        {
            //Preparar Logger
            Limpiar_Trace();

            Agregar_Trace("INSERT INTO [dbo].[core_notacredito]" +
                                    "([fecha]" +
                                    ",[numdocumento]" +
                                    ",[documentoaplica]" +
                                    ",[motivo]" +
                                    ",[cliente]" +
                                    ",[valor]" +
                                    ",[usuario]" +
                                    ",[salesID]" +
                                    ",[loadinvoice]" +
                                    ",[loadpaym]" +
                                    ",[msgError]" +
                                    ",[paymID])" +
                                    " VALUES " +
                                    "('" + entidad.fecha.ToString("s") + "'" +
                                    ",'" + entidad.numdocumento + "'" +
                                    ",'" + entidad.documentoaplica + "'" +
                                    ",'" + entidad.motivo + "'" +
                                    ",'" + entidad.cliente + "'" +
                                    "," + entidad.valor +
                                    ",'" + entidad.usuario + "'" +
                                    "," + ((entidad.salesID == null) ? "NULL" : String.Format("'{0}'", entidad.salesID)) +
                                    "," + ((entidad.loadinvoice) ? 1 : 0) +
                                    "," + ((entidad.loadpaym) ? 1 : 0) +
                                    "," + ((entidad.msgError == null) ? "NULL" : String.Format("'{0}'", entidad.msgError)) +
                                    "," + ((entidad.paymID == null) ? "NULL" : String.Format("'{0}'", entidad.paymID)) +
                                    ") " +
                                    "SET @notacredito_id = scope_identity()", true);

            foreach (Models.core_notacreditodetalle ncdet in entidad.core_notacreditodetalle)
            {
                Agregar_Trace_NotaCreditoDetalle(ncdet);
            }

            Graba_TraceFile();
        }

        public static void Agregar_Trace_NotaCreditoDetalle(Models.core_notacreditodetalle entidad)
        {
            Agregar_Trace("INSERT INTO [dbo].[core_notacreditodetalle]" +
                                    "([fecha]" +
                                    ",[notacredito_id]" +
                                    ",[linea]" +
                                    ",[item_id]" +
                                    ",[cantidad]" +
                                    ",[pvp]" +
                                    ",[descuento]" +
                                    ",[unidad]" +
                                    ",[total])" +
                                    " VALUES " +
                                    "('" + entidad.fecha.ToString("s") + "'" +
                                    ",@notacredito_id" +
                                    "," + entidad.linea +
                                    ",'" + entidad.item_id + "'" +
                                    "," + entidad.cantidad +
                                    "," + entidad.pvp +
                                    "," + entidad.descuento +
                                    ",'" + entidad.unidad + "'" +
                                    "," + entidad.total +
                                    ")");
        }

        public static void Agregar_Trace_Giftcard(Models.core_giftcard entidad)
        {
            //Preparar Logger
            Limpiar_Trace();

            Agregar_Trace("INSERT INTO [dbo].[core_giftcard]" +
                                    "([fecha_creacion]" +
                                    ",[fecha_modificacion]" +
                                    ",[codigo]" +
                                    ",[fecha_activacion]" +
                                    ",[fecha_expiracion]" +
                                    ",[fecha_desactivacion]" +
                                    ",[saldo]" +
                                    ",[activo]" +
                                    ",[bono]" +
                                    ",[valor])" +
                                    " VALUES " +
                                    "('" + entidad.fecha_creacion.ToString("s") + "'" +
                                    ",'" + entidad.fecha_modificacion.ToString("s") + "'" +
                                    ",'" + entidad.codigo + "'" +
                                    "," + ((entidad.fecha_activacion == null) ? "NULL" : String.Format("'{0}'", ((DateTime)entidad.fecha_activacion).ToString("s"))) +
                                    "," + ((entidad.fecha_expiracion == null) ? "NULL" : String.Format("'{0}'", ((DateTime)entidad.fecha_expiracion).ToString("s"))) +
                                    "," + ((entidad.fecha_desactivacion == null) ? "NULL" : String.Format("'{0}'", ((DateTime)entidad.fecha_desactivacion).ToString("s"))) +
                                    "," + entidad.saldo +
                                    "," + ((entidad.activo) ? 1 : 0) +
                                    "," + ((entidad.bono) ? 1 : 0) +
                                    "," + ((entidad.valor == null) ? "NULL" : String.Format("{0}", (decimal)entidad.valor)) +
                                    ")");

            Graba_TraceFile();
        }

        public static void Agregar_Trace_PagoCanjePuntos(string xml)
        {
            Limpiar_Trace();

            Agregar_Trace("declare @xml as xml = cast('" + xml + "' as xml) " +
                            "declare @xmlResponse as varchar(max) " +
                            "exec[PtsCliente].[sprCallForTransacSQLAcumularPts] @xml, @xmlResponse out " +
                            "select @xmlResponse ");
            Graba_TraceFile();
        }
        public static void Agregar_Trace_AcumulaPuntos(string xml)
        {
            Limpiar_Trace();

            Agregar_Trace("declare @xml as xml = cast('" + xml + "' as xml) "+
                            "declare @xmlResponse as varchar(max) "+
                            "exec [PtsCliente].[spPagoCanjePuntos] @xml, @xmlResponse out "+
                            "select @xmlResponse ");
            Graba_TraceFile();
        }
        #endregion
    }
}
