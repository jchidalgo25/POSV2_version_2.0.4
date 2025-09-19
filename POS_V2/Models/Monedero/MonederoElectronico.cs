using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Monedero
{

    public class MonederoElectronico
    {
        public Factura _factura;
        private decimal _saldoPuntos;
        private decimal _saldoGifCard;
        private decimal _saldoTarEmp;


        public decimal SaldoPuntos
        {
            get { return _saldoPuntos; }
            set { _saldoPuntos = value; }
        }
        public decimal SaldoGifCard
        {
            get { return _saldoGifCard; }
            set { _saldoGifCard = value; }
        }
        public decimal SaldoTarEmp
        {
            get { return _saldoTarEmp; }
            set { _saldoTarEmp = value; }
        }
    }

    public class MonederoRespuesta
    {
        public int codError { get; set; }
        public string msjError { get; set; }
        public decimal SaldoMonedero { get; set; }

        private decimal _saldoPuntos;
        private decimal _saldoGifCard;
        private decimal _saldoTarEmp;


        public decimal SaldoPuntos
        {
            get { return _saldoPuntos; }
            set { _saldoPuntos = value; }
        }
        public decimal SaldoGifCard
        {
            get { return _saldoGifCard; }
            set { _saldoGifCard = value; }
        }
        public decimal SaldoTarEmp
        {
            get { return _saldoTarEmp; }
            set { _saldoTarEmp = value; }
        }

    }


    public class MetodosBilletera
    {
        public static MonederoRespuesta RecuperaSaldosPorIdentificacion(string identificacion)
        {
            string cadenaCon = "";
            string query = string.Empty;
            MonederoRespuesta monederoRespuesta = new MonederoRespuesta();
            decimal PuntosMonedero = 0;
            decimal saldoGiftCard = 0;
            decimal saldoMonedero = 0;
            decimal saldoTarjetaEmp = 0;
            decimal saldoPtos = 0;

            string saldoGiftCard_ = string.Empty;
            string saldoMonedero_ = string.Empty;
            string saldoTarEmp_ = string.Empty;

            try
            {
                if (Control.Common.GlobalParameters.ConServerPuntos == "")
                {
                    cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"cadenaCon: {cadenaCon}");
                    Control.Common.General.GetMensajeToList(205);

                    monederoRespuesta = new MonederoRespuesta();
                    monederoRespuesta.codError = -1;
                    monederoRespuesta.msjError = "RecuperaSaldos: Candena de conexion invalida o null";
                    monederoRespuesta.SaldoMonedero = 0;
                    return monederoRespuesta;
                }

                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;

                // *** IMPLEMENTACIÓN DE REINTENTO AUTOMÁTICO *** JCHID
                int maxIntentos = 3;
                Exception ultimaExcepcion = null;

                for (int intento = 1; intento <= maxIntentos; intento++)
                {
                    try
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion",
                            $"Intento {intento} de {maxIntentos} - Consultando identificación: {identificacion}");


                        //query = string.Empty;
                        //query = string.Concat(query, $"declare @AccountNum as varchar(20) = '{identificacion}' ", Environment.NewLine);
                        //query = string.Concat(query, $"declare @respuesta as decimal(10,2) ", Environment.NewLine);
                        //query = string.Concat(query, $" exec [PtsCliente].[spValidarHistoricoPuntosGen] ", Environment.NewLine);
                        //query = string.Concat(query, $" @AccountNum ", Environment.NewLine);
                        //query = string.Concat(query, $", @respuesta out ", Environment.NewLine);
                        //query = string.Concat(query, $"Select @respuesta respuesta", Environment.NewLine);

                   
                        //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"query: {query}");
                        
                        //DataSet dtsConsulta = Control.Common.General.GetDataSet(query, cadenaCon);
                        //if (dtsConsulta.Tables.Count > 0)
                        //{
                        //    if (dtsConsulta.Tables[0].Rows.Count > 0)
                        //    {

                        //        //decimal.TryParse(saldoMonedero_, out saldoMonedero);


                        //        string saldoPuntos_ = string.Empty;
                        //        saldoPuntos_ = dtsConsulta.Tables[0].Rows[0]["respuesta"].ToString();
                        //        decimal.TryParse(saldoPuntos_, out saldoMonedero);
                        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", $"saldoMonedero: {saldoMonedero}");

                        //        decimal SaldoPuntos = Math.Round(saldoMonedero * Control.WalletPoints.ClsPoints.FactorCanje, 2);

                        //        monederoRespuesta = new MonederoRespuesta();
                        //        monederoRespuesta.codError = 0;
                        //        monederoRespuesta.msjError = "Datos correcto";
                        //        monederoRespuesta.SaldoMonedero = SaldoPuntos;

                        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", $"_factura.SaldoPuntos: {SaldoPuntos}");

                        //    }
                        //}

                        query = string.Empty;
                        query = string.Concat(query, $"Select * from dbo.[vieInvCustomerRevision] ", Environment.NewLine);
                        query = string.Concat(query, $"where 1=1 ", Environment.NewLine);
                        query = string.Concat(query, $"and identificacion = '{identificacion}'", Environment.NewLine);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"query: {query}");

                        DataSet dtsConsulta = Control.Common.General.GetDataSetBilletera(query, cadenaCon);

                        if (dtsConsulta.Tables.Count > 0)
                        {
                            if (dtsConsulta.Tables[0].Rows.Count > 0)
                            {
                                saldoGiftCard_ = dtsConsulta.Tables[0].Rows[0]["SaldoGifCard"].ToString();
                                saldoMonedero_ = dtsConsulta.Tables[0].Rows[0]["PuntosMonedero"].ToString();
                                saldoTarEmp_ = dtsConsulta.Tables[0].Rows[0]["SaldoTarEmp"].ToString();

                                decimal.TryParse(saldoGiftCard_, out saldoGiftCard);
                                decimal.TryParse(saldoMonedero_, out saldoMonedero);
                                decimal.TryParse(saldoTarEmp_, out saldoTarjetaEmp);

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion",
                                    $"Éxito en intento {intento} - saldoGiftCard_: {saldoGiftCard_}");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"saldoMonedero_: {saldoMonedero_}");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"saldoTarEmp_: {saldoTarEmp_}");

                                monederoRespuesta = new MonederoRespuesta();
                                monederoRespuesta.codError = 0;
                                monederoRespuesta.msjError = "Datos correctos";
                                monederoRespuesta.SaldoGifCard = saldoGiftCard;
                                monederoRespuesta.SaldoMonedero = saldoMonedero;
                                monederoRespuesta.SaldoTarEmp = saldoTarjetaEmp;

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"_factura.saldoGiftCard: {saldoGiftCard}");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"_factura.saldoMonedero: {saldoMonedero}");
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion", $"_factura.saldoTarjetaEmp: {saldoTarjetaEmp}");

                                // *** ÉXITO - Salir del loop de reintentos ***
                                return monederoRespuesta;
                            }
                        }

                        // Si llegamos aquí, la consulta no retornó datos
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warning, "MainWindow", "RecuperaSaldosPorIdentificacion",
                            $"Intento {intento}: Consulta ejecutada pero sin datos para identificación {identificacion}");

                        //// Para consultas sin datos, no reintentar - retornar resultado vacío
                        //monederoRespuesta = new MonederoRespuesta();
                        //monederoRespuesta.codError = 0;
                        //monederoRespuesta.msjError = "Consulta exitosa - Sin datos para la identificación";
                        return monederoRespuesta;
                    }
                    catch (SqlException sqlEx)
                    {
                        ultimaExcepcion = sqlEx;
                        bool permiteReintento = Control.Common.General.EsErrorQuePermiteReintento(sqlEx);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warning, "MainWindow", "RecuperaSaldosPorIdentificacion",
                            $"SqlException en intento {intento}: Código {sqlEx.Number} - {sqlEx.Message} - PermiteReintento: {permiteReintento}");

                        if (permiteReintento && intento < maxIntentos)
                        {
                            const int tiempoEspera = 1200;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion",
                                $"Esperando {tiempoEspera}ms antes del siguiente intento...");

                            System.Threading.Thread.Sleep(tiempoEspera);
                            continue; // Continuar con el siguiente intento
                        }
                        else
                        {
                            // No permite reintento o se agotaron los intentos
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "RecuperaSaldosPorIdentificacion",
                                $"Fallo definitivo después de {intento} intentos - SqlException: {sqlEx.Number} - {sqlEx.Message}");
                            break; // Salir del loop
                        }
                    }
                    catch (Exception ex)
                    {
                        ultimaExcepcion = ex;
                        bool permiteReintento = Control.Common.General.EsErrorQuePermiteReintento(ex);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warning, "MainWindow", "RecuperaSaldosPorIdentificacion",
                            $"Exception en intento {intento}: {ex.Message} - PermiteReintento: {permiteReintento}");

                        if (permiteReintento && intento < maxIntentos)
                        {
                            const int tiempoEspera = 1200;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecuperaSaldosPorIdentificacion",
                                $"Esperando {tiempoEspera}ms antes del siguiente intento...");

                            System.Threading.Thread.Sleep(tiempoEspera);
                            continue;
                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "RecuperaSaldosPorIdentificacion",
                                $"Fallo definitivo después de {intento} intentos - Exception: {ex.Message}");
                            break;
                        }
                    }
                }

                // Si llegamos aquí, todos los intentos fallaron
                monederoRespuesta = new MonederoRespuesta();
                monederoRespuesta.codError = -2;
                monederoRespuesta.msjError = $"Error después de {maxIntentos} intentos: " + ultimaExcepcion?.Message;
                monederoRespuesta.SaldoMonedero = 0;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "RecuperaSaldosPorIdentificacion",
                    "Billetera Electrónica - Todos los intentos fallaron - " + Control.Common.ExceptionHandler.GetExceptionMessages(ultimaExcepcion),
                    "StackTrace: " + ultimaExcepcion?.StackTrace);

                return monederoRespuesta;
            }
            catch (Exception ex)
            {
                // Exception en validación inicial (fuera del loop de reintentos)
                monederoRespuesta = new MonederoRespuesta();
                monederoRespuesta.codError = -3;
                monederoRespuesta.msjError = "Error: " + ex.Message;
                monederoRespuesta.SaldoMonedero = 0;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "RecuperaSaldosPorIdentificacion",
                    "Billetera Electrónica - Error en validación inicial - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                    "StackTrace: " + ex.StackTrace);
            }

            return monederoRespuesta;
        }






        public static MonederoRespuesta ValidarMonederoCampania(string identificacion)
        {
            int idCampania = 0;
            decimal saldoPuntos = 0.0M;
            Boolean _tieneCampania = true;
            MonederoRespuesta monederoRespuesta = new MonederoRespuesta();
            string cadenaCon = "";
            string query = string.Empty;


            if (identificacion == "" || identificacion == "9999999999999")
            {
                monederoRespuesta.codError = -1;
                monederoRespuesta.msjError = "Cliente Consumidor FInal";
                monederoRespuesta.SaldoMonedero = 0;
                return monederoRespuesta;

            }


            try
            {
                using (POSEntities db = new POSEntities())
                {
                    _tieneCampania = false;
                    if (db.core_parametro.Any(x => x.identificador == "CAMPANIA_MONEDERO"))
                    {
                        string valor = db.core_parametro.Where(x => x.identificador == "CAMPANIA_MONEDERO").FirstOrDefault().valor;
                        if (!string.IsNullOrEmpty(valor))
                        {

                            _tieneCampania = false;
                            if (valor != "0")
                            {
                                idCampania = Convert.ToInt32(valor);
                                _tieneCampania = true;
                            }
                        }
                    }
                    if (_tieneCampania)
                    {
                        if (Control.Common.GlobalParameters.ConServerPuntos == "")
                        {
                            cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", $"cadenaCon: {cadenaCon}");
                            Control.Common.General.GetMensajeToList(205);

                            monederoRespuesta = new MonederoRespuesta();
                            monederoRespuesta.codError = -1;
                            monederoRespuesta.msjError = "Candena de conexion invalida o null";
                            monederoRespuesta.SaldoMonedero = 0;
                            return monederoRespuesta;


                        }

                        cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;

                        try
                        {
                            query = string.Empty;
                            query = string.Concat(query, $"declare @AccountNum as varchar(20) = '{identificacion}' ", Environment.NewLine);
                            query = string.Concat(query, $"declare @respuesta as decimal(10,2) ", Environment.NewLine);
                            query = string.Concat(query, $" exec [PtsCliente].[spValidarHistoricoPuntosGen] ", Environment.NewLine);
                            query = string.Concat(query, $" @AccountNum ", Environment.NewLine);
                            query = string.Concat(query, $", @respuesta out ", Environment.NewLine);
                            query = string.Concat(query, $"Select @respuesta respuesta", Environment.NewLine);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", $"query: {query}");


                            DataSet dtsConsulta = Control.Common.General.GetDataSet(query, cadenaCon);
                            if (dtsConsulta.Tables.Count > 0)
                            {
                                if (dtsConsulta.Tables[0].Rows.Count > 0)
                                {
                                    string saldoPuntos_ = string.Empty;
                                    saldoPuntos_ = dtsConsulta.Tables[0].Rows[0]["respuesta"].ToString();
                                    decimal.TryParse(saldoPuntos_, out saldoPuntos);
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", $"saldoPuntos: {saldoPuntos}");

                                    decimal SaldoPuntos = Math.Round(saldoPuntos * Control.WalletPoints.ClsPoints.FactorCanje, 2);

                                    monederoRespuesta = new MonederoRespuesta();
                                    monederoRespuesta.codError = 0;
                                    monederoRespuesta.msjError = "Datos correcto";
                                    monederoRespuesta.SaldoMonedero = SaldoPuntos;

                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", $"_factura.SaldoPuntos: {SaldoPuntos}");

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            monederoRespuesta = new MonederoRespuesta();
                            monederoRespuesta.codError = -2;
                            monederoRespuesta.msjError = "Error: " + ex.Message;
                            monederoRespuesta.SaldoMonedero = 0;

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidarMonederoCampania", "Billetera Electrónica esta fuera de línea, a continuacion las excepciones encontradas - "
                                + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                            return monederoRespuesta;
                        }
                    }

                }


                return monederoRespuesta;
            }
            catch (Exception ex)
            {
                monederoRespuesta = new MonederoRespuesta();
                monederoRespuesta.codError = -3;
                monederoRespuesta.msjError = "Error: " + ex.Message;
                monederoRespuesta.SaldoMonedero = 0;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidarMonederoCampania", "Billetera Electrónica esta fuera de línea, a continuacion las excepciones encontradas - "
                    + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }



            return monederoRespuesta;
        }


    }




}