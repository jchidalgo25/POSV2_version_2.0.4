using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace POS.Control.Common
{
    public static class Network
    {
        public static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            //Debe estar compuesta por 4 secciones
            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            //Ninguna seccion de la ip puede sobrepasar los 3 caracteres
            if (splitValues.Any(r => r.Length > 3))
                return false;

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public static string GetMyIpAddress()
        {
            string ipAddress = string.Empty;
            string nombrePC = Dns.GetHostName().ToString();
            IPHostEntry ipEntry = Dns.GetHostEntry(nombrePC);
            IPAddress[] addr = ipEntry.AddressList;
            //Verificamos la IP de la PC donde se ejecuta la APP.
            ipAddress = addr.Where(i => !i.IsIPv6LinkLocal && !i.IsIPv6Teredo && i.AddressFamily == AddressFamily.InterNetwork).First().ToString();

            return ipAddress;
        }
    }
}
