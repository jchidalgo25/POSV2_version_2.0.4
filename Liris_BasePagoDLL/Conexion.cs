using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Liris_BasePagoDLL.Modelos;

namespace Liris_BasePagoDLL
{
    public class Conexion
    {
        public static byte[] dataReceived;

        public static byte[] getDataRecived() => Conexion.dataReceived;

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
                Conexion.dataReceived = new byte[length];
                Array.Copy((Array)numArray, 0, (Array)Conexion.dataReceived, 0, length);
                Console.WriteLine("Received: {0}", (object)BitConverter.ToString(Conexion.dataReceived));
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
        public static PinPadRespuesta ConnectPinPad(string ip, int puerto, byte[] datasend, int timeout, string TipoTans)
        {
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

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
                Conexion.dataReceived = new byte[length];
                Array.Copy((Array)numArray, 0, (Array)Conexion.dataReceived, 0, length);
                Console.WriteLine("Received: {0}", (object)BitConverter.ToString(Conexion.dataReceived));
                stream.Close();
                tcpClient.Close();

                conexionRespuesta.CodigoRespuestaPinPad = "00";
                conexionRespuesta.MensajeRespuestaPinPad = "";
                conexionRespuesta.TramaRespuesta = str;
                conexionRespuesta.dataReceived = Conexion.getDataRecived();
                return conexionRespuesta;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR SDK --> " + ex.Message);
                Logger.LogMessage(Enum.LogTypes.Error, "CajaPinPad_Conexion", "ConnectPinPad", "ERROR SDK -->" + ex.Message);

                conexionRespuesta.CodigoRespuestaPinPad = "21";
                conexionRespuesta.MensajeRespuestaPinPad = "Error: " + ex.Message;
                return conexionRespuesta;
            }

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
                conexionRespuesta.CodigoRespuestaPinPad = RespuestaPinpad.Substring(IndexCabTrama + 4, 2).ToString();

      
                /*
                 * TipoTans => PP Procesa Pago
                 * TipoTans => LT Lectura tarjeta
                 * TipoTans => CT Consulta Tarjeta
                 * TipoTans => PC Procesa Control
                 * TipoTans => CB Configuración Báscia
                 */
                switch (TipoTans)
                {
                    case "PP":
                        conexionRespuesta.MensajeRespuestaPinPad = RespuestaPinpad.Substring(12, RespuestaPinpad.IndexOf(" ", 12)).ToString();
                        break;
                    case "LT":
                    case "CT":
                        conexionRespuesta.MensajeRespuestaPinPad = RespuestaPinpad.Substring(leerTramaRes + 4, 10).ToString();
                        break;
                }

                conexionRespuesta.TramaRespuesta = str;
                conexionRespuesta.dataReceived = Conexion.getDataRecived();
                return conexionRespuesta;
            }
            catch (Exception ex)
            {
                conexionRespuesta.CodigoRespuestaPinPad = "20";
                conexionRespuesta.MensajeRespuestaPinPad = "Error: " + ex.Message;
                Console.WriteLine("ERROR SDK --> " + ex.Message);
                Logger.LogMessage(Enum.LogTypes.Error, "CajaPinPad_Conexion", "ObtenerRespuestaPinPad", "ERROR SDK -->" + ex.Message);

            }
            return conexionRespuesta;
        }


    }
}
