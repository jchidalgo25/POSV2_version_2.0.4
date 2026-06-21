using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;


namespace POS.Control.SRI
{
    public static class SriApiClient
    {
        // Método único para peticiones GET
        public static T Get<T>(string url, string bearerToken = null)
        {
            string jsonResponse = MakeRequest(url, "GET", null, bearerToken);
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        // Método único para peticiones POST/PUT
        public static T Post<T>(string url, object data, string bearerToken = null)
        {
            string jsonResponse = MakeRequest(url, "POST", data, bearerToken);
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        private static string MakeRequest(string url, string method, object data, string bearerToken)
        {
            try
            {
                // Forzar TLS 1.2, crucial para APIs modernas
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // 3072 es el valor numérico para Tls12

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = method;
                httpWebRequest.Accept = "application/json";
                httpWebRequest.ContentType = "application/json";

                // Agregar token de autorización si se proporciona
                if (!string.IsNullOrEmpty(bearerToken))
                {
                    httpWebRequest.Headers.Add("Authorization", "Bearer " + bearerToken);
                }

                // Escribir datos en el cuerpo para métodos POST/PUT
                if (data != null && (method == "POST" || method == "PUT"))
                {
                    string jsonData = JsonConvert.SerializeObject(data);
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(jsonData);
                    }
                }

                // Obtener la respuesta
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                // Si la API devuelve un error (4xx, 5xx), lo leemos y lo lanzamos
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        // Lanzamos una nueva excepción con detalles claros
                        throw new Exception($"Error de la API del SRI ({errorResponse.StatusCode}): {errorText}", wex);
                    }
                }
                // Si el error es de red (ej. no hay internet), relanzamos la excepción original
                throw new Exception("Error de red al contactar al SRI: " + wex.Message, wex);
            }
        }
    }
}
