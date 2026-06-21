
using System;
using System.Net.Sockets;
using System.Text;
using POS.Control.CajaPinpad.Modelo;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.IO;

namespace POS.Control.CajaPinpad
{
    public class Conexion
    {
        private static readonly object DataLock = new object(); // Protección de concurrencia
        public static byte[] dataReceived;

        public static byte[] getDataRecived()
        {
            lock (DataLock)
            {
                return dataReceived == null ? null : (byte[])dataReceived.Clone();
            }
        }

        private static bool CanConnectToIp(string ip)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(ip, 1000);  // Timeout de 1 segundo
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al hacer ping a la IP: {ex.Message}");
                return false;
            }
        }

        public static bool Connectar(string ip, int puerto, byte[] datasend, int timeout)
        {
            try
            {
                TcpClient tcpClient = new TcpClient(ip, puerto);
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(datasend, 0, datasend.Length);
                Console.WriteLine("Sent: {0}", (object)Encoding.ASCII.GetString(datasend, 0, datasend.Length));
                Console.WriteLine("Received: {0}", (object)BitConverter.ToString(datasend));
                byte[] numArray = new byte[1024];
                int length = stream.Read(numArray, 0, numArray.Length);
                string str = Encoding.ASCII.GetString(numArray, 0, length);
                if (str != null)
                    Console.WriteLine("Received: {0}", (object)str);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Connectar", "Conectar", " (object)str " + (object)str);


                Conexion.dataReceived = new byte[length];
                Array.Copy((Array)numArray, 0, (Array)Conexion.dataReceived, 0, length);
                Console.WriteLine("Received: {0}", (object)BitConverter.ToString(Conexion.dataReceived));
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Connectar", "Received: {0}", " (object)str " + (object)BitConverter.ToString(Conexion.dataReceived) );

                stream.Close();
                tcpClient.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR SDK --> " + ex.Message);
            }
            return false;
        }

        public static async Task<PinPadRespuesta> ConnectPinPadAsync(string pinpadIP, int pinpadPort, byte[] datasend, int timeout, string TipoTans)
        {
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ConnectPinPadAsync", $"Conectando al PinPad en {pinpadIP}:{pinpadPort}...");
                    await tcpClient.ConnectAsync(pinpadIP, pinpadPort);
                    Console.WriteLine($"✅ Conectado a {pinpadIP}:{pinpadPort}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ConnectPinPadAsync", $"✅ Conectado a {pinpadIP}:{pinpadPort}");

                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        // Enviar datos al PinPad
                        await stream.WriteAsync(datasend, 0, datasend.Length);
                        Console.WriteLine($"📤 Enviado: {Encoding.ASCII.GetString(datasend)}");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ConnectPinPadAsync", $"📤 Enviado: {Encoding.ASCII.GetString(datasend)}");

                        // Leer la respuesta del PinPad
                        byte[] buffer = new byte[1024];
                        int length = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string str = Encoding.ASCII.GetString(buffer, 0, length);

                        if (!string.IsNullOrEmpty(str))
                        {
                            Console.WriteLine($"📥 Recibido: {str}");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ConnectPinPadAsync", $"📥 Recibido: {str}");

                        }

                        // Guardar los datos recibidos en la clase Conexion
                        Conexion.dataReceived = new byte[length];
                        Array.Copy(buffer, 0, Conexion.dataReceived, 0, length);

                        Console.WriteLine($"📥 Datos en bytes: {BitConverter.ToString(Conexion.dataReceived)}");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ConnectPinPadAsync", $"📥 Datos en bytes: {BitConverter.ToString(Conexion.dataReceived)}");

                        // Asignar valores a la respuesta
                        conexionRespuesta.CodigoRespuesta = "00";
                        conexionRespuesta.MensajeRespuesta = "";
                        conexionRespuesta.TramaRespuesta = str;
                        conexionRespuesta.dataReceived = Conexion.getDataRecived();

                        return conexionRespuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR SDK --> {ex.Message}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ConnectPinPadAsync", $"❌ ERROR SDK --> {ex.Message}");

                conexionRespuesta.CodigoRespuesta = "21";
                conexionRespuesta.MensajeRespuesta = $"Error: {ex.Message}";
                return conexionRespuesta;
            }

        }

        public static PinPadRespuesta ConnectPinPad(string ip, int puerto, byte[] datasend, string TipoTans, int SendTimeout = 12000, int ReceiveTimeout = 12000)
        {

            if(SendTimeout == 0) { SendTimeout = 1200; }
            var respuesta = EnviarRecibir(ip, puerto, datasend, TipoTans, SendTimeout, ReceiveTimeout);

            if (respuesta.CodigoRespuesta != "00" || respuesta.dataReceived == null)
            {
                respuesta.CodigoRespuesta = "-2";
                respuesta.MensajeRespuesta = "Objeto de Respuesta es NULL. Confirmar con Sistemas";
                return respuesta;
            }

            return respuesta;

        }

        private static PinPadRespuesta EnviarRecibir(string ip, int puerto, byte[] datasend, string tipoTransaccion, int SendTimeout = 10000, int ReceiveTimeout = 12000)
        {
            var respuesta = new PinPadRespuesta();

            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    // Intentar conexión con timeout manual
                    var connectTask = tcpClient.ConnectAsync(ip, puerto);
                    if (!connectTask.Wait(SendTimeout))
                        throw new TimeoutException("Timeout al intentar conectar con el dispositivo.");

                    tcpClient.SendTimeout = SendTimeout;
                    tcpClient.ReceiveTimeout = ReceiveTimeout;

                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        // Enviar datos
                        stream.Write(datasend, 0, datasend.Length);
                        stream.Flush();

                        string tramaEnviada = Encoding.ASCII.GetString(datasend);
                        Console.WriteLine("Sent: {0}", tramaEnviada);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Conexion", "EnviarRecibir", $"Sent: {tramaEnviada}");

                        // Leer respuesta
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        if (bytesRead <= 0)
                        {
                            respuesta.CodigoRespuesta = "-1";
                            respuesta.MensajeRespuesta = "No se recibió respuesta del dispositivo.";
                            return respuesta;
                        }

                        byte[] dataReceivedLocal = new byte[bytesRead];
                        Array.Copy(buffer, dataReceivedLocal, bytesRead);

                        string tramaRecibida = Encoding.ASCII.GetString(dataReceivedLocal);
                        Console.WriteLine("Received: {0}", tramaRecibida);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Conexion", "EnviarRecibir", $"Received: {tramaRecibida}");

                        lock (DataLock)
                        {
                            dataReceived = dataReceivedLocal;
                        }

                        respuesta.CodigoRespuesta = "00";
                        respuesta.MensajeRespuesta = "OK";
                        respuesta.TramaRespuesta = tramaRecibida;
                        respuesta.dataReceived = dataReceivedLocal;
                        return respuesta;
                    }
                }
            }
            catch (SocketException sockEx)
            {
                string mensaje = (sockEx.ErrorCode == 10061)
                    ? "La conexión fue rechazada. Verifique que el dispositivo esté encendido y accesible."
                    : "Error de red: " + sockEx.Message;

                Console.WriteLine(mensaje);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Conexion", "SocketException", mensaje);
                respuesta.CodigoRespuesta = sockEx.ErrorCode.ToString();
                respuesta.MensajeRespuesta = mensaje;
            }
            catch (TimeoutException timeoutEx)
            {
                Console.WriteLine("Timeout: " + timeoutEx.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Conexion", "TimeoutException", timeoutEx.Message);
                respuesta.CodigoRespuesta = "408";
                respuesta.MensajeRespuesta = "Timeout de conexión: " + timeoutEx.Message;
            }
            catch (IOException ioEx)
            {
                string mensaje = "Error de E/S: " + ioEx.Message;
                if (ioEx.InnerException != null)
                    mensaje += " | Detalle: " + ioEx.InnerException.Message;

                Console.WriteLine(mensaje);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Conexion", "IOException", mensaje);
                respuesta.CodigoRespuesta = "11";
                respuesta.MensajeRespuesta = mensaje;
            }
            catch (Exception ex)
            {
                string mensaje = "Error inesperado: " + ex.Message;
                if (ex.InnerException != null)
                    mensaje += " | Detalle: " + ex.InnerException.Message;

                Console.WriteLine(mensaje);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Conexion", "Exception", mensaje);
                respuesta.CodigoRespuesta = "20";
                respuesta.MensajeRespuesta = mensaje;
            }

            return respuesta;
        }

        //public static PinPadRespuesta ConnectPinPad(string ip, int puerto, byte[] datasend, int timeout, string TipoTans = "")
        //{
        //    PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

        //    try
        //    {
        //        TcpClient tcpClient = new TcpClient(ip, puerto);
        //        NetworkStream stream = tcpClient.GetStream();
        //        stream.Write(datasend, 0, datasend.Length);
        //        var tramaSent = (object)Encoding.ASCII.GetString(datasend, 0, datasend.Length);

        //        Console.WriteLine("Sent: {0}", (object)Encoding.ASCII.GetString(datasend, 0, datasend.Length));
        //        Console.WriteLine("Received: {0}", (object)BitConverter.ToString(datasend));
        //        byte[] numArray = new byte[1024];
        //        int length = stream.Read(numArray, 0, numArray.Length);
        //        string str = Encoding.ASCII.GetString(numArray, 0, length);

        //        if (str != null)
        //            Console.WriteLine("Received: {0}", (object)str);

        //        Conexion.dataReceived = new byte[length];
        //        Array.Copy((Array)numArray, 0, (Array)Conexion.dataReceived, 0, length);
        //        Console.WriteLine("Received: {0}", (object)BitConverter.ToString(Conexion.dataReceived));
        //        stream.Close();
        //        tcpClient.Close();


        //        /**/
        //        var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(Conexion.getDataRecived()));
        //        int leerTramaRes = 75;

        //        if (RespuestaPinpad.Length > 98)
        //        {
        //            leerTramaRes = leerTramaRes + 24;
        //        }

        //        string CabTrama = RespuestaPinpad.Substring(0, RespuestaPinpad.IndexOf(TipoTans, 0)).ToString();
        //        int IndexCabTrama = RespuestaPinpad.IndexOf(TipoTans, 0)+2;

        //        conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();

        //        if (TipoTans == "LT") {
        //            conexionRespuesta.MensajeRespuesta = RespuestaPinpad.Substring(leerTramaRes + 4, 10).ToString();
        //            conexionRespuesta.NumBin = RespuestaPinpad.Substring(IndexCabTrama + 4, 6).ToString();
        //        }

        //        conexionRespuesta.TramaRespuesta = str;
        //        conexionRespuesta.dataReceived = Conexion.getDataRecived();
        //        return conexionRespuesta;
        //    }
        //    catch (Exception ex)
        //    {
        //        conexionRespuesta.CodigoRespuesta = "20";
        //        conexionRespuesta.MensajeRespuesta = "Error: " + ex.Message;
        //        Console.WriteLine("ERROR SDK --> " + ex.Message);
        //    }
        //    return conexionRespuesta;
        //}
        public static PinPadRespuesta ObtenerRespuestaPinPad(string ip, int puerto, byte[] datasend, int timeout, string TipoTans = "")
        {
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

            try
            {
                TcpClient tcpClient = new TcpClient(ip, puerto);
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(datasend, 0, datasend.Length);
                var tramaSent = (object)Encoding.ASCII.GetString(datasend, 0, datasend.Length);

                Console.WriteLine("Sent: {0}", (object)Encoding.ASCII.GetString(datasend, 0, datasend.Length));
                Console.WriteLine("Received: {0}", (object)BitConverter.ToString(datasend));
                byte[] numArray = new byte[1024];
                int length = stream.Read(numArray, 0, numArray.Length);
                string str = Encoding.ASCII.GetString(numArray, 0, length);
                if (str != null)
                    Console.WriteLine("Received: {0}", (object)str);
                Conexion.dataReceived = new byte[length];
                Array.Copy((Array)numArray, 0, (Array)Conexion.dataReceived, 0, length);
                Console.WriteLine("Received: {0}", (object)BitConverter.ToString(Conexion.dataReceived));
                stream.Close();
                tcpClient.Close();

                var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(Conexion.getDataRecived()));
                int leerTramaRes = 75;

                if (RespuestaPinpad.Length > 98)
                {
                    leerTramaRes = leerTramaRes + 24;
                }

                string CabTrama = RespuestaPinpad.Substring(0, RespuestaPinpad.IndexOf(TipoTans, 0)).ToString();
                int IndexCabTrama = RespuestaPinpad.IndexOf(TipoTans, 0) + 2;
                conexionRespuesta.CodigoRespuesta = RespuestaPinpad.Substring(IndexCabTrama+4, 2).ToString();

                // conexionRespuesta.NumBin = string.Empty;
                //if (conexionRespuesta.CodigoRespuesta == "00") {
                //    conexionRespuesta.NumBin = RespuestaPinpad.Substring(IndexCabTrama + 4, 6).ToString();
                //}
    
                /*
                 * TipoTans => PP Procesa Pago
                 * TipoTans => LT Lectura tarjeta
                 * TipoTans => CT Consulta Tarjeta
                 * TipoTans => PC Procesa Control
                 * TipoTans => CB Configuración Báscia
                 */
                switch (TipoTans) {
                    case "PP":
                        conexionRespuesta.MensajeRespuesta = RespuestaPinpad.Substring(12, RespuestaPinpad.IndexOf(" ", 12)).ToString();
                        break;
                    case "LT": case "CT":
                        conexionRespuesta.MensajeRespuesta = RespuestaPinpad.Substring(leerTramaRes + 4, 10).ToString();
                        break;
                }

                conexionRespuesta.TramaRespuesta = str;
                conexionRespuesta.dataReceived = Conexion.getDataRecived();
                return conexionRespuesta;
            }
            catch (Exception ex)
            {
                conexionRespuesta.CodigoRespuesta = "20";
                conexionRespuesta.MensajeRespuesta = "Error: " + ex.Message;
                Console.WriteLine("ERROR SDK --> " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "CajaPinPad_Conexion", "ObtenerRespuestaPinPad", "ERROR SDK -->" + ex.Message);

            }
            return conexionRespuesta;
        }


    }
}
