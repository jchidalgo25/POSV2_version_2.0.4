using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trx.Messaging;
using POS.Control.Clientes;
using POS.Control.Pagos;
using POS.Control.ToolBox;
using POS.Control.CajaPinpad;
using POS.Control.CajaPinpad.Modelo;
//using CajaPinpad;


namespace POS.Control.Pagos
{

    class ClsEnviaPinPadGeneral
    {

        public void EjecutaContingencia(string CodigoRespuesta, string Autorizador)
        {
            string MensajeRespuesta = string.Empty;

            switch (CodigoRespuesta)
            {
                case "91":
                case "96":
                case "TO":
                case "20": // Esta respuesta es el catch 
                    MensajeRespuesta = "Fuera de Linea / TimeOut ";
                    //Control.CajaPinpad.Modelo.ConexionContingente
                    Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = 1;
                    Control.Common.GlobalParameters.ConectContingente.ValidaRedPinPad = true;

                    DateTime FechaInicioEspera = DateTime.Now;
                    DateTime FechaFinEspera = FechaInicioEspera.AddMinutes(Control.Common.GlobalParameters.ConectContingente.TiempoEsperaContingente);
                    Control.Common.GlobalParameters.ConectContingente.FechaInicioEspera = FechaInicioEspera;
                    Control.Common.GlobalParameters.ConectContingente.FechaFinEspera = FechaFinEspera;
                    Control.Common.GlobalParameters.ConectContingente.PinPadContingente = true;


                    // En caso de que se encuentre fuera de linea, procedo a reasignar
                    switch (Autorizador)
                    {
                        case "1":
                            // Autorizador = "2";
                            Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = 2;
                            Control.Common.GlobalParameters.IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                            Control.Common.GlobalParameters.PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;
                            break;

                        case "2":
                            Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = 1;
                            Control.Common.GlobalParameters.IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadDATAFAST;
                            Control.Common.GlobalParameters.PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadDataFast;

                            break;
                    }

                    break;
            }

        }

        public PinPadRespuesta ObtenerTramaPinPadAnulacion(string IP, int Puerto, int timeout, string Autorizador, string trama, string rutabines, int grabalog, string TipoTrans)
        {
            //SendRequestPinpad2
            string respuesta = "";
            Control.Common.GlobalParameters.MonederoConsumoActivo = true;
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "trama:" + trama);

                if (!Control.Common.GlobalParameters.EstTcpIpPinpad) ///Si no es cumincacion PINPAD via TCPIP se realiza la comunicacion por default via COM
                {
                    Envio envio = new Envio();
                    respuesta = envio.Envio_requerimientoPinpad(IP, Control.Common.GlobalParameters.PuertoPinPad, timeout, trama, rutabines, grabalog);
                }
                else
                {
                    ClibcommunicateSocket.ClsComunicaSocket objconmu = new ClibcommunicateSocket.ClsComunicaSocket();
                    ClibSecurity.ClsSecurity objetoSecurity = new ClibSecurity.ClsSecurity();
                    var componenteAleatorio = objetoSecurity.GeneraComponente();
                    var ComponenteEncriptado = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, componenteAleatorio, Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP);

                    var tramaNuevo = trama + componenteAleatorio + ComponenteEncriptado;
                    var hextamaño = String.Format("0{0:X}", tramaNuevo.Length).PadLeft(4, '0');



                    var TramaFinal = hextamaño + tramaNuevo;
                    var TramaFinalByte = Encoding.ASCII.GetBytes(TramaFinal);
                    Conexion.Connectar(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinalByte, Control.Common.GlobalParameters.PinPadReceiveTimeout);
                    var connResponse = ProcessData.hex2AsciiStr(ProcessData.byte2hex(Conexion.getDataRecived()));

                    string CabTrama = connResponse.Substring(0, connResponse.IndexOf(TipoTrans, 0)).ToString();
                    int IndexCabTrama = connResponse.IndexOf(TipoTrans, 0) + 2;

                    string RespuestaPinPad = connResponse.Substring(IndexCabTrama, 2).ToString();

                    if (RespuestaPinPad != "00")
                    {
                        conexionRespuesta.CodigoRespuesta = RespuestaPinPad;
                        conexionRespuesta.MensajeRespuesta = "Error en respuesta PINPAND " + connResponse.Substring(12, 20).ToString();

                        conexionRespuesta.TramaRespuesta = connResponse;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Error en PINPAD" + connResponse.Substring(12, 20).ToString());
                        return conexionRespuesta;

                    }

                    conexionRespuesta.CodigoRespuesta = "00";
                    conexionRespuesta.MensajeRespuesta = connResponse.Substring(12, 20).ToString();

                    conexionRespuesta.CodigoRespuestaEntidad = connResponse.Substring(IndexCabTrama + 4, 2).ToString();
                    if (conexionRespuesta.CodigoRespuestaEntidad != "00")
                    {
                        conexionRespuesta.MensajeRespuestaEntidad = connResponse.Substring(12, 20).ToString();
                        return conexionRespuesta;
                    }

                    conexionRespuesta.dataReceived = Conexion.getDataRecived();
                    byte[] getDataRecived = Conexion.getDataRecived();
                    var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(getDataRecived));

                    //var RespuestaPinpad = objconmu.StartClient(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinal, Control.Common.GlobalParameters.PinPadReceiveTimeout);
                    var ultimos32bit = RespuestaPinpad.Substring(RespuestaPinpad.Length - 32, 32);
                    var Respkeydrecha = ultimos32bit.Substring(0, 16);
                    var Respkeyizquiera = ultimos32bit.Substring(16, 16);// llave derecha
                    var respuesdescrip2 = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP, Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, Respkeydrecha);

                    if (Respkeyizquiera == respuesdescrip2)
                    {
                        var tramaStandarEnviar = RespuestaPinpad.Substring(4, RespuestaPinpad.Length - 32);
                        respuesta = tramaStandarEnviar;
                    }

                    int leerTramaRes = 75;

                    if (RespuestaPinpad.Length > 98)
                    {
                        leerTramaRes = leerTramaRes + 24;
                    }
                    conexionRespuesta.NumBin = RespuestaPinpad.Substring(398, 6);
                    conexionRespuesta.TramaRespuesta = RespuestaPinpad;

                    string MensajeRespuesta = string.Empty;
                    conexionRespuesta.MensajeRespuestaEntidad = RespuestaPinpad.Substring(14, 18).ToString();


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "trama respuesta:" + respuesta);

                }
                return conexionRespuesta;
            }
            catch (Exception exx)
            {

                conexionRespuesta.CodigoRespuestaEntidad = "91";
                conexionRespuesta.MensajeRespuestaEntidad = "Error: " + exx.Message;

                conexionRespuesta.CodigoRespuesta = "20";
                conexionRespuesta.MensajeRespuesta = "Error al generar el trama, revise por favor. " + exx.InnerException.Message;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "Error al generar el trama, revise por favor." + exx.InnerException.Message);
                return conexionRespuesta;
            }
        }

        public PinPadRespuesta ObtenerTramaPinPad(string IP, int Puerto, int timeout, string Autorizador, string trama, string rutabines, int grabalog, string TipoTrans)
        {
            string respuesta = "";
            Control.Common.GlobalParameters.MonederoConsumoActivo = true;
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "trama:" + trama);
                int PuertoPinPad = Control.Common.GlobalParameters.PuertoPinPad != Puerto ? Puerto : Control.Common.GlobalParameters.PuertoPinPad;

                if (!Control.Common.GlobalParameters.EstTcpIpPinpad) ///Si no es cumincacion PINPAD via TCPIP se realiza la comunicacion por default via COM
                {
                    Envio envio = new Envio();
                    respuesta = envio.Envio_requerimientoPinpad(Control.Common.GlobalParameters.IPPinPad, PuertoPinPad, timeout, trama, rutabines, grabalog);
                }
                else
                {
                    ClibcommunicateSocket.ClsComunicaSocket objconmu = new ClibcommunicateSocket.ClsComunicaSocket();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " objconmu: " + objconmu);

                    ClibSecurity.ClsSecurity objetoSecurity = new ClibSecurity.ClsSecurity();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " objetoSecurity: " + objetoSecurity);

                    var componenteAleatorio = objetoSecurity.GeneraComponente();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " componenteAleatorio: " + componenteAleatorio);

                    var ComponenteEncriptado = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, componenteAleatorio, Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP,: " + Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP,: " + Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP);

                    var tramaNuevo = trama + componenteAleatorio + ComponenteEncriptado;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " tramaNuevo : " + tramaNuevo);


                    var hextamaño = String.Format("0{0:X}", tramaNuevo.Length).PadLeft(4, '0');
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " hextamaño : " + hextamaño);

                    var TramaFinal = hextamaño + tramaNuevo;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " TramaFinal: " + TramaFinal);

                    var TramaFinalByte = Encoding.ASCII.GetBytes(TramaFinal);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " TramaFinalByte: " + TramaFinalByte);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Control.Common.GlobalParameters.IPPinPad: " + Control.Common.GlobalParameters.IPPinPad);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " PuertoPinPad: " + PuertoPinPad);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Control.Common.GlobalParameters.PinPadReceiveTimeout: " + Control.Common.GlobalParameters.PinPadReceiveTimeout);

                    string connResponse = string.Empty;
                    string CabTrama = string.Empty;

                    try
                    {
                        Conexion.Connectar(Control.Common.GlobalParameters.IPPinPad, PuertoPinPad, TramaFinalByte, Control.Common.GlobalParameters.PinPadReceiveTimeout);
                        var Response = ProcessData.hex2AsciiStr(ProcessData.byte2hex(Conexion.getDataRecived()));

                        if (Response == null)
                        {

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " getDataRecivedReturn, retorno valor NULL ");

                            conexionRespuesta = new PinPadRespuesta();
                            conexionRespuesta.CodigoRespuesta = "-1";
                            conexionRespuesta.CodigoRespuestaEntidad = "-1";
                            conexionRespuesta.MensajeRespuestaEntidad = "No se obtuvo respuesta de pinpad";
                            conexionRespuesta.TramaRespuesta = string.Empty;
                            return conexionRespuesta;
                        }


                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Response: " + Response);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Conexion.getDataRecived.Length : " + Conexion.getDataRecived().Length);
                        connResponse = Response.ToString();


                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Ejecuta exepction: " + ex.Message);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Response: " + connResponse);

                        conexionRespuesta = new PinPadRespuesta();
                        conexionRespuesta.CodigoRespuesta = "-1";
                        conexionRespuesta.CodigoRespuestaEntidad = "-1";
                        conexionRespuesta.MensajeRespuestaEntidad = "Error: " + ex.Message; ;
                        conexionRespuesta.TramaRespuesta = string.Empty;
                        return conexionRespuesta;
                    }


                    //connResponse = connResponse.Substring(0, connResponse.IndexOf(TipoTrans, 0)).ToString();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " CabTrama: " + CabTrama);

                    int IndexCabTrama = 0;
                    string RespuestaPinPad = string.Empty;
                    string CodIdentificadoRed = string.Empty;


                    if (TipoTrans == "PC")
                    {

                        try
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " TipoTrans: " + TipoTrans);

                            IndexCabTrama = connResponse.IndexOf(TipoTrans, 0) + 2;
                            RespuestaPinPad = connResponse.Substring(IndexCabTrama, 2).ToString();
                            CodIdentificadoRed = connResponse.Substring(8, 2).ToString();

                            if (RespuestaPinPad != "00")
                            {
                                string MsjRespuesta = string.Empty;
                                conexionRespuesta.CodigoRespuesta = RespuestaPinPad;
                                conexionRespuesta.CodigoRespuestaEntidad = RespuestaPinPad;

                                switch (RespuestaPinPad)
                                {
                                    case "01":
                                        MsjRespuesta = "Error en Trama";
                                        break;
                                    case "02":
                                        MsjRespuesta = "Error conexión Pinpad";
                                        break;
                                    case "20":
                                        MsjRespuesta = "Error durante Proceso";
                                        break;
                                    case "99":
                                        MsjRespuesta = "Error General";
                                        break;
                                    case "ER":
                                        MsjRespuesta = "Error conexión Pinpad deberá";
                                        break;
                                    case "TO":
                                        MsjRespuesta = "Error Timeout - Tiempo de espera Agotado";
                                        break;
                                    default:
                                        MsjRespuesta = "Ejecución Exitosa";
                                        break;


                                }

                                conexionRespuesta.MensajeRespuesta = MsjRespuesta;
                                conexionRespuesta.MensajeRespuestaEntidad = MsjRespuesta;
                                return conexionRespuesta;
                            }
                            //connResponse.Substring(IndexCabTrama + 4, 2).ToString();
                            conexionRespuesta.CodigoRespuesta = "00";
                            conexionRespuesta.CodigoRespuestaEntidad = "00";
                            conexionRespuesta.MensajeRespuesta = connResponse.Substring(10, 20).ToString();
                            conexionRespuesta.MensajeRespuestaEntidad = connResponse.Substring(10, 20).ToString();
                            conexionRespuesta.TramaRespuesta = connResponse;

                            return conexionRespuesta;
                        }
                        catch (Exception ex)
                        {
                            conexionRespuesta.CodigoRespuesta = "-2";
                            conexionRespuesta.CodigoRespuesta = "Error: al recuperar resultado Trama: " + ex.Message;
                            conexionRespuesta.TramaRespuesta = connResponse;

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Error: al recuperar resultado Trama: " + ex.Message);

                            return conexionRespuesta;
                        }

                    }


                    IndexCabTrama = connResponse.IndexOf(TipoTrans, 0) + 2;
                    RespuestaPinPad = connResponse.Substring(IndexCabTrama, 2).ToString();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " IndexCabTrama: " + IndexCabTrama + " RespuestaPinPad: " + RespuestaPinPad);


                    if (RespuestaPinPad != "00")
                    {
                        conexionRespuesta.CodigoRespuesta = RespuestaPinPad;
                        conexionRespuesta.MensajeRespuesta = "Error en respuesta PINPAND " + connResponse.Substring(12, 20).ToString();

                        conexionRespuesta.TramaRespuesta = connResponse;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", " Error en PINPAD" + connResponse.Substring(12, 20).ToString());
                        return conexionRespuesta;

                    }

                    conexionRespuesta.CodigoRespuesta = "00";
                    conexionRespuesta.MensajeRespuesta = connResponse.Substring(12, 20).ToString();
                    conexionRespuesta.CodigoRespuestaEntidad = connResponse.Substring(IndexCabTrama + 4, 2).ToString();

                    if (conexionRespuesta.CodigoRespuestaEntidad != "00")
                    {
                        conexionRespuesta.MensajeRespuestaEntidad = connResponse.Substring(12, 20).ToString();
                        return conexionRespuesta;
                    }

                    conexionRespuesta.dataReceived = Conexion.getDataRecived();
                    byte[] getDataRecived = Conexion.getDataRecived();
                    var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(getDataRecived));

                    //var RespuestaPinpad = objconmu.StartClient(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinal, Control.Common.GlobalParameters.PinPadReceiveTimeout);
                    var ultimos32bit = RespuestaPinpad.Substring(RespuestaPinpad.Length - 32, 32);
                    var Respkeydrecha = ultimos32bit.Substring(0, 16);
                    var Respkeyizquiera = ultimos32bit.Substring(16, 16);// llave derecha
                    var respuesdescrip2 = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP, Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, Respkeydrecha);

                    if (Respkeyizquiera == respuesdescrip2)
                    {
                        var tramaStandarEnviar = RespuestaPinpad.Substring(4, RespuestaPinpad.Length - 32);
                        respuesta = tramaStandarEnviar;
                    }

                    int leerTramaRes = 75;

                    if (RespuestaPinpad.Length > 98)
                    {
                        leerTramaRes = leerTramaRes + 24;
                    }
                    conexionRespuesta.NumBin = RespuestaPinpad.Substring(398, 6);
                    conexionRespuesta.TramaRespuesta = RespuestaPinpad;

                    string MensajeRespuesta = string.Empty;
                    conexionRespuesta.MensajeRespuestaEntidad = RespuestaPinpad.Substring(14, 18).ToString();


                    if (string.IsNullOrEmpty(conexionRespuesta.CodigoRespuestaEntidad))
                    {
                        conexionRespuesta.CodigoRespuestaEntidad = conexionRespuesta.CodigoRespuesta;
                        conexionRespuesta.MensajeRespuestaEntidad = conexionRespuesta.MensajeRespuesta;
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", "trama respuesta:" + respuesta);

                }
                return conexionRespuesta;
            }
            catch (Exception exx)
            {
                conexionRespuesta = new PinPadRespuesta();
                conexionRespuesta.CodigoRespuestaEntidad = "-1";
                conexionRespuesta.MensajeRespuestaEntidad = "Error: " + exx.Message;
                conexionRespuesta.CodigoRespuesta = "20";

                string InnerExceptionMessage = string.Empty;
                if (exx.InnerException != null) { InnerExceptionMessage = exx.InnerException.Message; }
                conexionRespuesta.MensajeRespuesta = "Error al generar el trama, revise por favor. " + InnerExceptionMessage;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", "conexionRespuesta.CodigoRespuestaEntidad: " + conexionRespuesta.CodigoRespuestaEntidad);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", "conexionRespuesta.CodigoRespuesta: " + conexionRespuesta.CodigoRespuesta);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "ObtenerTramaPinPad", "Error al generar el trama, revise por favor." + InnerExceptionMessage);
                return conexionRespuesta;
            }
        }

        public string SendRequestPinpad(string IP, int Puerto, int timeout, string trama, string rutabines, int grabalog)
        {
            string respuesta = "";
            Control.Common.GlobalParameters.MonederoConsumoActivo = true;

            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "trama:" + trama);

                if (!Control.Common.GlobalParameters.EstTcpIpPinpad) ///Si no es cumincacion PINPAD via TCPIP se realiza la comunicacion por default via COM
                {
                    Envio envio = new Envio();
                    respuesta = envio.Envio_requerimientoPinpad(IP, Control.Common.GlobalParameters.PuertoPinPad, timeout, trama, rutabines, grabalog);
                }
                else
                {
                    ClibSecurity.ClsSecurity objetoSecurity = new ClibSecurity.ClsSecurity();
                    var componenteAleatorio = objetoSecurity.GeneraComponente();
                    var ComponenteEncriptado = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, componenteAleatorio, Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP);

                    var tramaNuevo = trama + componenteAleatorio + ComponenteEncriptado;
                    var hextamaño = String.Format("0{0:X}", tramaNuevo.Length).PadLeft(4, '0');
                    var TramaFinal = hextamaño + tramaNuevo;
                    ClibcommunicateSocket.ClsComunicaSocket objconmu = new ClibcommunicateSocket.ClsComunicaSocket();

                    var TramaFinalByte = Encoding.ASCII.GetBytes(TramaFinal);
                    Conexion.Connectar(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinalByte, Control.Common.GlobalParameters.PinPadReceiveTimeout);
                    var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(Conexion.getDataRecived()));

                    //var RespuestaPinpad = objconmu.StartClient(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinal, Control.Common.GlobalParameters.PinPadReceiveTimeout);


                    var ultimos32bit = RespuestaPinpad.Substring(RespuestaPinpad.Length - 32, 32);
                    //  ClibSecurity.ClsSecurity objetodet = new ClsSecurity();
                    var Respkeydrecha = ultimos32bit.Substring(0, 16);
                    var Respkeyizquiera = ultimos32bit.Substring(16, 16);// llave derecha

                    var respuesdescrip2 = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP, Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, Respkeydrecha);
                    //  var respuesdescrip3 = objetodet.F3DESDencriptar(txtkeyderecha.Text, keyizquiera ,txtComponenteClaro.Text );

                    //   var varTramaInical = txtTramaEnvia.Text)

                    if (Respkeyizquiera == respuesdescrip2)
                    {
                        var tramaStandarEnviar = RespuestaPinpad.Substring(4, RespuestaPinpad.Length - 32);
                        respuesta = tramaStandarEnviar;

                        //MessageBox.Show("Validacion OK DE LLAVES" + keyizquiera + " Calculad: " + respuesdescrip2, "VALIDACION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        /// respuesta = 
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "trama respuesta:" + respuesta);

                }
                return respuesta;
            }
            catch (Exception exx)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "Error al generar el trama, revise por favor." + exx.InnerException.Message);
                return respuesta;
            }

        }

        public string SendRequestPinpadAnt(string IP, int Puerto, int timeout, string trama, string rutabines, int grabalog)
        {
            string respuesta = "";
            Control.Common.GlobalParameters.MonederoConsumoActivo = true;

            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "trama:" + trama);

                if (!Control.Common.GlobalParameters.EstTcpIpPinpad) ///Si no es cumincacion PINPAD via TCPIP se realiza la comunicacion por default via COM
                {
                    Envio envio = new Envio();
                    respuesta = envio.Envio_requerimientoPinpad(IP, Control.Common.GlobalParameters.PuertoPinPad, timeout, trama, rutabines, grabalog);
                }
                else
                {
                    ClibSecurity.ClsSecurity objetoSecurity = new ClibSecurity.ClsSecurity();
                    var componenteAleatorio = objetoSecurity.GeneraComponente();
                    var ComponenteEncriptado = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, componenteAleatorio, Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP);

                    var tramaNuevo = trama + componenteAleatorio + ComponenteEncriptado;
                    var hextamaño = String.Format("0{0:X}", tramaNuevo.Length).PadLeft(4, '0');
                    var TramaFinal = hextamaño + tramaNuevo;
                    ClibcommunicateSocket.ClsComunicaSocket objconmu = new ClibcommunicateSocket.ClsComunicaSocket();

                    var TramaFinalByte = Encoding.ASCII.GetBytes(TramaFinal);
                    Conexion.Connectar(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinalByte, Control.Common.GlobalParameters.PinPadReceiveTimeout);
                    var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(Conexion.getDataRecived()));

                    //var RespuestaPinpad = objconmu.StartClient(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinal, Control.Common.GlobalParameters.PinPadReceiveTimeout);


                    var ultimos32bit = RespuestaPinpad.Substring(RespuestaPinpad.Length - 32, 32);
                    //  ClibSecurity.ClsSecurity objetodet = new ClsSecurity();
                    var Respkeydrecha = ultimos32bit.Substring(0, 16);
                    var Respkeyizquiera = ultimos32bit.Substring(16, 16);// llave derecha

                    var respuesdescrip2 = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP, Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, Respkeydrecha);
                    //  var respuesdescrip3 = objetodet.F3DESDencriptar(txtkeyderecha.Text, keyizquiera ,txtComponenteClaro.Text );

                    //   var varTramaInical = txtTramaEnvia.Text)

                    if (Respkeyizquiera == respuesdescrip2)
                    {
                        var tramaStandarEnviar = RespuestaPinpad.Substring(4, RespuestaPinpad.Length - 32);
                        respuesta = tramaStandarEnviar;

                        //MessageBox.Show("Validacion OK DE LLAVES" + keyizquiera + " Calculad: " + respuesdescrip2, "VALIDACION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        /// respuesta = 
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "trama respuesta:" + respuesta);

                }
                return respuesta;
            }
            catch (Exception exx)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsEnviaPinPadGeneral", "SendRequestPinpad", "Error al generar el trama, revise por favor." + exx.InnerException.Message);
                return respuesta;
            }

        }


        public PinPadRespuesta LecturaTarjeta(string IP, int Puerto, int timeout, string trama, string rutabines, int grabalog, string TipoTrans = "")
        {
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

            try
            {
                ClibSecurity.ClsSecurity objetoSecurity = new ClibSecurity.ClsSecurity();
                var componenteAleatorio = objetoSecurity.GeneraComponente();
                var ComponenteEncriptado = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, componenteAleatorio, Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP);

                var tramaNuevo = trama + componenteAleatorio + ComponenteEncriptado;
                var hextamaño = String.Format("0{0:X}", tramaNuevo.Length).PadLeft(4, '0');
                var TramaFinal = hextamaño + tramaNuevo;

                ClibcommunicateSocket.ClsComunicaSocket objconmu = new ClibcommunicateSocket.ClsComunicaSocket();
                var TramaFinalByte = Encoding.ASCII.GetBytes(TramaFinal);

                int PinPadReceiveTimeout = Control.Common.GlobalParameters.PinPadReceiveTimeout;
                int PinPadSendTimeout = Control.Common.GlobalParameters.PinPadSendTimeout;

                if (string.IsNullOrWhiteSpace(TipoTrans)) { TipoTrans = "LT"; }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Ejecuta LT Conexion.ConnectPinPad");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $" PinPadSendTimeout: {PinPadSendTimeout}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", $" PinPadReceiveTimeout: {PinPadReceiveTimeout}");

                conexionRespuesta = Conexion.ConnectPinPad(IP, Puerto, TramaFinalByte, TipoTrans, PinPadSendTimeout, PinPadReceiveTimeout);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", " Si CodigoRespuesta == 00, Consulta Realizada correctamente ");
                if (conexionRespuesta.CodigoRespuesta != "00")
                {
                    return conexionRespuesta;
                }

                byte[] getDataRecived = conexionRespuesta.dataReceived;
                var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(getDataRecived));


                int leerTramaRes = 75;

                if (RespuestaPinpad.Length > 98)
                {
                    leerTramaRes = leerTramaRes + 24;
                }

                string CabTrama = RespuestaPinpad.Substring(0, RespuestaPinpad.IndexOf(TipoTrans, 0)).ToString();
                int IndexCabTrama = RespuestaPinpad.IndexOf(TipoTrans, 0) + 2;

                conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();

                switch (TipoTrans)
                {
                    case "LT":
                        conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();
                        conexionRespuesta.MensajeRespuesta = RespuestaPinpad.Substring(leerTramaRes + 4, 10).ToString();
                        conexionRespuesta.NumBin = RespuestaPinpad.Substring(IndexCabTrama + 4, 6).ToString();
                        conexionRespuesta.NumeroTarjeta = RespuestaPinpad.Substring(5, 64).ToString();


                        break;
                    case "CT":
                        conexionRespuesta.NumBin = RespuestaPinpad.Substring(IndexCabTrama + 66, 8).ToString();
                        conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(6, 2).ToString();
                        conexionRespuesta.MensajeRespuesta = RespuestaPinpad.Substring(84, 20).ToString();

                        break;
                }


                return conexionRespuesta;
            }
            catch (Exception ex)
            {
                conexionRespuesta.CodigoRespuesta = "-1";
                conexionRespuesta.MensajeRespuesta = "Error en LecturaTarjeta: " + ex.InnerException.Message;
                return conexionRespuesta;
            }


        }


        public PinPadRespuesta EjecutaTrama(string IPPinPad, int PuertoPinPad, int timeout, string trama, string rutabines, int grabalog, string TipoTrans)
        {
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();
            string respuesta = string.Empty;

            try
            {
                ClibSecurity.ClsSecurity objetoSecurity = new ClibSecurity.ClsSecurity();
                var componenteAleatorio = objetoSecurity.GeneraComponente();
                var ComponenteEncriptado = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, componenteAleatorio, Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutaTramaMultired", $" componenteAleatorio: {componenteAleatorio} " +
                     $"; ComponenteEncriptado {ComponenteEncriptado} ");

                var tramaNuevo = trama + componenteAleatorio + ComponenteEncriptado;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutaTramaMultired", $" tramaNuevo: {tramaNuevo} ");

                var hextamaño = String.Format("0{0:X}", tramaNuevo.Length).PadLeft(4, '0');
                var TramaFinal = hextamaño + tramaNuevo;

                ClibcommunicateSocket.ClsComunicaSocket objconmu = new ClibcommunicateSocket.ClsComunicaSocket();
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutaTramaMultired", $" TramaFinal: {TramaFinal} ");
                var TramaFinalByte = Encoding.ASCII.GetBytes(TramaFinal);

                int PinPadReceiveTimeout = Control.Common.GlobalParameters.PinPadReceiveTimeout;
                int PinPadSendTimeout = Control.Common.GlobalParameters.PinPadSendTimeout;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "EjecutaTramaMultired", " Control.Common.GlobalParameters.IPPinPad: " + IPPinPad);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "EjecutaTramaMultired", " PuertoPinPad: " + PuertoPinPad);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "EjecutaTramaMultired", $" PinPadSendTimeout: {PinPadSendTimeout}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "EjecutaTramaMultired", $" PinPadReceiveTimeout: {PinPadReceiveTimeout}");

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutaTramaPinPad", $" Ejecuta {TipoTrans} : Conexion.ConnectPinPad");

                conexionRespuesta = Conexion.ConnectPinPad(IPPinPad, PuertoPinPad, TramaFinalByte, TipoTrans, PinPadSendTimeout, PinPadReceiveTimeout);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "EjecutaTramaPinPad", " Si CodigoRespuesta == 00, Consulta Realizada correctamente ");

                if (conexionRespuesta.CodigoRespuesta != "00")
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ClsEnviaPinPadGeneral", "EjecutaTramaPinPad", " getDataRecivedReturn, retorno valor NULL ");
                    PinPadRespuesta errorRespuesta = new PinPadRespuesta();
                    errorRespuesta = new PinPadRespuesta();
                    errorRespuesta.CodigoRespuesta = conexionRespuesta.CodigoRespuesta;
                    errorRespuesta.CodigoRespuestaEntidad = conexionRespuesta.CodigoRespuesta;
                    errorRespuesta.MensajeRespuestaEntidad = conexionRespuesta.MensajeRespuestaEntidad;
                    errorRespuesta.TramaRespuesta = string.Empty;
                    return errorRespuesta;
                }

                byte[] getDataRecived = conexionRespuesta.dataReceived;
                var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(getDataRecived));


                int leerTramaRes = 75;

                if (RespuestaPinpad.Length > 98)
                {
                    leerTramaRes = leerTramaRes + 24;
                }

                string CabTrama = RespuestaPinpad.Substring(0, RespuestaPinpad.IndexOf(TipoTrans, 0)).ToString();
                int IndexCabTrama = RespuestaPinpad.IndexOf(TipoTrans, 0) + 2;
                string CodRespPinPad = string.Empty;
                string CodIdentificadoRed = string.Empty;

                switch (TipoTrans)
                {
                    case "PP":

                        var ultimos32bit = RespuestaPinpad.Substring(RespuestaPinpad.Length - 32, 32);
                        var Respkeydrecha = ultimos32bit.Substring(0, 16);
                        var Respkeyizquiera = ultimos32bit.Substring(16, 16);// llave derecha
                        var respuesdescrip2 = objetoSecurity.F3DESEncriptar(Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP, Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP, Respkeydrecha);

                        if (Respkeyizquiera == respuesdescrip2)
                        {
                            var tramaStandarEnviar = RespuestaPinpad.Substring(4, RespuestaPinpad.Length - 32);
                            respuesta = tramaStandarEnviar;
                        }

                        conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();
                        conexionRespuesta.CodigoRespuestaEntidad = RespuestaPinpad.Substring(IndexCabTrama + 4, 2).ToString();
                        conexionRespuesta.MensajeRespuesta = RespuestaPinpad.Substring(12, 20).ToString();

                        string MensajeRespuesta = string.Empty;
                        conexionRespuesta.MensajeRespuestaEntidad = RespuestaPinpad.Substring(14, 18).ToString();
                        conexionRespuesta.dataReceived = getDataRecived;
                        conexionRespuesta.NumBin = RespuestaPinpad.Substring(398, 6);
                        conexionRespuesta.TramaRespuesta = RespuestaPinpad;


                        break;

                    case "PC":

                        IndexCabTrama = RespuestaPinpad.IndexOf(TipoTrans, 0) + 2;
                        CodRespPinPad = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();
                        CodIdentificadoRed = RespuestaPinpad.Substring(IndexCabTrama + 2, 2).ToString();
                        MensajeRespuesta = RespuestaPinpad.Substring(IndexCabTrama + 4, 20).ToString();

                        CodIdentificadoRed = RespuestaPinpad.Substring(8, 2).ToString();

                        if (CodRespPinPad != "00")
                        {
                            string MsjRespuesta = string.Empty;


                            switch (CodRespPinPad)
                            {
                                case "01":
                                    MsjRespuesta = "Error en Trama";
                                    break;
                                case "02":
                                    MsjRespuesta = "Error conexión Pinpad";
                                    break;
                                case "20":
                                    MsjRespuesta = "Error durante Proceso";
                                    break;
                                case "99":
                                    MsjRespuesta = "Error General";
                                    break;
                                case "ER":
                                    MsjRespuesta = "Error conexión Pinpad deberá";
                                    break;
                                case "TO":
                                    MsjRespuesta = "Error Timeout - Tiempo de espera Agotado";
                                    break;
                                default:
                                    MsjRespuesta = "Ejecución Exitosa";
                                    break;


                            }

                            conexionRespuesta.MensajeRespuesta = MsjRespuesta;
                            conexionRespuesta.MensajeRespuestaEntidad = MensajeRespuesta;
                            return conexionRespuesta;
                        }

                        conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();
                        conexionRespuesta.CodigoRespuestaEntidad = CodRespPinPad;
                        conexionRespuesta.MensajeRespuesta = RespuestaPinpad.Substring(10, 20).ToString();
                        conexionRespuesta.MensajeRespuestaEntidad = RespuestaPinpad.Substring(10, 20).ToString();
                        conexionRespuesta.TramaRespuesta = RespuestaPinpad;

                        break;
                    case "LT":
                        conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();
                        conexionRespuesta.TramaRespuesta = RespuestaPinpad;
                        break;
                    case "CT":

                        conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();
                        conexionRespuesta.TramaRespuesta = RespuestaPinpad;
                        break;
                }

                return conexionRespuesta;
            }
            catch (Exception ex)
            {
                PinPadRespuesta errorRespuesta = new PinPadRespuesta();
                errorRespuesta = new PinPadRespuesta();
                conexionRespuesta.CodigoRespuesta = "-1";
                conexionRespuesta.MensajeRespuesta = "Error en EjecutaTranaPinPad: " + ex.InnerException.Message;
                return conexionRespuesta;
            }
        }


        public PinPadRespuesta LecturaTarjetaManual()
        {
            PinPadRespuesta respuestaSimulada = new PinPadRespuesta();

            // Instanciamos el formulario que acabamos de crear
            using (var frm = new FrmIngresoTarjeta())
            {
                var result = frm.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string tarjetaLimpia = frm.NumeroTarjetaDigitado;

                    // --- SIMULACIÓN DE RESPUESTA DE PINPAD ---

                    // 1. Código de éxito
                    respuestaSimulada.CodigoRespuesta = "00";
                    respuestaSimulada.MensajeRespuesta = "INGRESO MANUAL OK";

                    // 2. Extraer el BIN (Primeros 6 dígitos)
                    // Esto es CRÍTICO: El método principal usa esto para saber si es Medianet o Datafast
                    if (tarjetaLimpia.Length >= 6)
                    {
                        respuestaSimulada.NumBin = tarjetaLimpia.Substring(0, 6);
                    }
                    else
                    {
                        // Fallback por si acaso
                        respuestaSimulada.NumBin = "000000";
                    }

                    // 3. Número de tarjeta
                    respuestaSimulada.NumeroTarjeta = tarjetaLimpia;

                    // 4. Trama cruda (por si se loguea en base de datos)
                    respuestaSimulada.TramaRespuesta = "MANUAL:" + tarjetaLimpia;
                }
                else
                {
                    // El usuario canceló o cerró la ventana
                    respuestaSimulada.CodigoRespuesta = "99";
                    respuestaSimulada.MensajeRespuesta = "CANCELADO POR USUARIO";
                    respuestaSimulada.NumBin = "";
                }
            }

            return respuestaSimulada;
        }




    }
}
