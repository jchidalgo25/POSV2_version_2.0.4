using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace POS.Control
{
    public class WebMethods
    {
        public WebMethods()
        {

        }
        /*.............*/
        private WebRequest create_webrequest(string method, string view)
        {
            var request = WebRequest.Create(Properties.Settings.Default.ROOT_URL_GENERAL + view);
            request.Method = method;
            if (method == "GET")
            {
                request.ContentType = "application/json; charset=utf-8";
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            return request;
        }


        public Models.User login(string parametros)
        {
            var result = new Models.User();
            result.result = false;
            string url = "accounts/login/";
            string json;
            var request = create_webrequest("POST", url);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parametros);
            request.ContentLength = bytes.Length;
            using (Stream os = request.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length); //Push it out there
                os.Close();
            }

            System.Net.WebResponse resp = request.GetResponse();
            if (resp == null)
            {
                result.result = false;
                result.mensaje = "Respuesta Vacia!";

            }
            else
            {
                using (BufferedStream buffer = new BufferedStream(resp.GetResponseStream()))
                {
                    using (StreamReader reader = new StreamReader(buffer))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                result = JsonConvert.DeserializeObject<Models.User>(json);
            }
            return result;
        }


        public string GeneraTicket(string url)
        {
            string result = "";
                       
            WebRequest request = WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";
            
            string json;
        
            System.Net.WebResponse resp = request.GetResponse();
            if (resp == null)
            {
                result ="";
            }
            else
            {
                using (BufferedStream buffer = new BufferedStream(resp.GetResponseStream()))
                {
                    using (StreamReader reader = new StreamReader(buffer))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                result = json;
            }
            return result;
        }

        public Models.Parking.ParkingResumResponse  ObtenerTicketParqueo(string tikect, string urlApiIngreso)
        {
            var result = new Models.Parking.ParkingResumResponse();
            
            WebRequest request = WebRequest.Create(urlApiIngreso);
            
            request.Method = "POST";
            request.ContentType = "application/json";

            var requestData = new
            {
                url = tikect
            };

            string json = JsonConvert.SerializeObject(requestData);

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }

            System.Net.WebResponse resp = request.GetResponse();
            if (resp == null)
            {
                return null;
            }
            else
            {
                using (BufferedStream buffer = new BufferedStream(resp.GetResponseStream()))
                {
                    using (StreamReader reader = new StreamReader(buffer))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                try
                {
                    var _result = JsonConvert.DeserializeObject<Models.Parking.ParkingResponse>(json);
                    result.status = _result.status;
                    result.date_entry = _result.date_entry;
                }
                catch (Exception)
                {   //cuando el pagao el json es diferente
                   var _result = JsonConvert.DeserializeObject<Models.Parking.Pagado.ParkingResponse>(json);
                    result.status = _result.status;
                    //busca la fecha del pago 
                    result.date_entry = _result.doc.bitacora_list.Find(x => x.info.Equals("Pago realizado")).fecha;   
                }
                
            }


            return result;
        }


        public Models.Parking.ParkingResponse PagarTicketParqueo(Models.Parking.ParkingRequest requestData, string urlApiSalida)
        {
            var result = new Models.Parking.ParkingResponse();

            WebRequest request = WebRequest.Create(urlApiSalida);

            request.Method = "POST";
            request.ContentType = "application/json";

            string json = JsonConvert.SerializeObject(requestData);

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }
            
            System.Net.WebResponse resp = request.GetResponse();
            if (resp == null)
            {
                return null;
            }
            else
            {
                using (BufferedStream buffer = new BufferedStream(resp.GetResponseStream()))
                {
                    using (StreamReader reader = new StreamReader(buffer))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                result = JsonConvert.DeserializeObject<Models.Parking.ParkingResponse>(json);
            }
            return result;
        }
    }
}
