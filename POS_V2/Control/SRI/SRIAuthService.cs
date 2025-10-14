using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace POS.Control.SRI
{
    public class SRIAuthService
    {
        private readonly string _sriSecurityUrl;
        private readonly string _ruc;
        private readonly string _certPath;
        private readonly string _certPassword;

        public SRIAuthService(string ruc, string certPath, string certPassword)
        {
            _ruc = ruc;
            _certPath = certPath;
            _certPassword = certPassword;
        }

        public static string ComputeSha512Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            // Crear una instancia del servicio SHA-512
            using (var sha512 = SHA512.Create())
            {
                // Convertir la entrada a bytes
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // Calcular el hash
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // Convertir el hash a cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.AppendFormat("{0:x2}", b); // formato hex sin mayúsculas
                }

                return sb.ToString();
            }
        }

        public static async Task<string> GetAccessTokenAsync(string cedulaAdicional)
        {
            var _sriSecurityUrl = Control.Common.GlobalParameters.SRI_URL_ACCESS_TOKEN;
            var _sriSecuriyKey = Control.Common.GlobalParameters.SRI_CLAVE_ADICIONAL;

            string keyEncript = ComputeSha512Hash(_sriSecuriyKey);

            _sriSecurityUrl.Replace("[RUC_CONTRIBUYENTE]", "0990865477001");
            _sriSecurityUrl.Replace("[CEDULA_ADICIONAL]", cedulaAdicional);
            _sriSecurityUrl.Replace("[CLAVE_ENCRIPTADA_ADICIONAL_SHA-512]", keyEncript);
            
            // Configurar certificado para todas las solicitudes
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            // Usar HttpClientHandler normal
            var handler = new HttpClientHandler();

            // Añadir certificado al contexto de la aplicación
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Registrar el certificado antes de hacer la llamada
            ServicePointManager.ServerCertificateValidationCallback += (se, certificado, cadena, errores) => true;

            using (var client = new HttpClient(handler))
            {
                // Agregar encabezados
                client.DefaultRequestHeaders.ConnectionClose = false;

                // Hacer solicitud POST
                var url = $"{_sriSecurityUrl}";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Connection.Clear();
                request.Headers.Connection.Add("keep-alive");

                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error al obtener token: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var json = JObject.Parse(responseBody);
                    return json["accessToken"]?.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Respuesta JSON inválida: " + responseBody, ex);
                }
            }
        }







    }
}
