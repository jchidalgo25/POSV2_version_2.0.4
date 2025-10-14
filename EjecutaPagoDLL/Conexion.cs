using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EjecutaPagoDLL
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

                conexionRespuesta.CodigoRespuesta = "00";
                conexionRespuesta.MensajeRespuesta = "";
                conexionRespuesta.TramaRespuesta = str;
                conexionRespuesta.dataReceived = Conexion.getDataRecived();
                return conexionRespuesta;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR SDK --> " + ex.Message);
               
                conexionRespuesta.CodigoRespuesta = "21";
                conexionRespuesta.MensajeRespuesta = "Error: " + ex.Message;
                return conexionRespuesta;
            }

        }

      

    }
}