using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using POS.Control.Main.MainTouch;
using POS.Models;
using POS.Models.Cliente;
using POS.Models.DevolucionIVA;
using POS.Models.SRI;

namespace POS.Control.SRI
{
    public class General
    {
        
        public enum TipoComprobanteElectronico
        {
            Factura = 1,               // "01"
            NotaDeCredito = 2,         // "04"
            Retencion = 3,             // "07"
            GuiaRemision = 4,          // "06"
            DevolucionIVA = 5          // "07"
        }

        public static string ToCodigo(TipoComprobanteElectronico tipo)
        {
            switch (tipo)
            {
                case TipoComprobanteElectronico.Factura:
                    return "01";
                case TipoComprobanteElectronico.NotaDeCredito:
                    return "04";
                case TipoComprobanteElectronico.Retencion:
                    return "07";
                case TipoComprobanteElectronico.GuiaRemision:
                    return "06";
                case TipoComprobanteElectronico.DevolucionIVA:
                    return "07";
                default:
                    throw new ArgumentOutOfRangeException("tipo", tipo, "Tipo de comprobante no reconocido.");
            }
        }


        //private static string GenerarClaveAccesoSRI(TipoComprobanteElectronico tipo, string Establecimiento, string PtoEmision, string Secuencia)
        //{
        //    // 1. Fecha de emisión (invertida a YYYYMMDD)
        //    string fecha = DateTime.Now.ToString("ddMMyyyy");
        //    string hora = DateTime.Now.ToString("HH:mm:ss");
        //    string fechaClave = fecha.Substring(0, 2) + fecha.Substring(2, 2) + fecha.Substring(4, 4);
        //    string fechaClaveInv = fecha.Substring(4, 4) + fecha.Substring(2, 2) + fecha.Substring(0, 2);
        //    string horaClave = hora.Substring(0,2) + hora.Substring(3, 2) + hora.Substring(6, 2);

        //    // 2. Tipo de comprobante (usa nuestro método ToCodigo)
        //    string tipoComprobante = ToCodigo(tipo);

        //    // 3. Datos del emisor
        //    string nRuc = Control.Common.GlobalParameters.NRuc;
        //    string tipoAmbiente = Control.Common.GlobalParameters.TipoAmbiente;
        //    string tipoEmision = Control.Common.GlobalParameters.TipoEmision;

        //    // Validación básica
        //    if (nRuc.Length != 13)
        //        throw new ArgumentException("El RUC debe tener 13 dígitos.");

        //    // 4. Serie concatenada
        //    string serie = Establecimiento + PtoEmision; // Ej: "001001"

        //    // 5. Número secuencial rellenado con ceros
        //    string numero = Secuencia.PadLeft(9, '0');
        //    string numComprobante = Secuencia.PadLeft(8, '0');

        //    // 6. Construcción de la base (47 dígitos)
        //    string claveBase = fechaClave + tipoComprobante + nRuc + tipoAmbiente + serie + numero + numComprobante + tipoEmision;

        //    // Limitar a 47 caracteres (por si acaso)
        //    if (claveBase.Length > 49)
        //        claveBase = claveBase.Substring(0, 49);

        //    // 7. Cálculo del dígito verificador
        //    int modulo = 11;
        //    int peso = 2;
        //    int suma = 0;

        //    for (int i = claveBase.Length - 1; i >= 0; i--)
        //    {
        //        if (!char.IsDigit(claveBase[i]))
        //            throw new ArgumentException("Caracter no numérico encontrado en clave base.");

        //        int digito = int.Parse(claveBase[i].ToString());
        //        suma += digito * peso;

        //        peso = (peso == 7) ? 2 : peso + 1;
        //    }

        //    int dv = modulo - (suma % modulo);

        //    switch (dv)
        //    {
        //        case 11: dv = 0; break;
        //        case 10: dv = 1; break;
        //        default: break;
        //    }

        //    // 8. Devolver clave completa
        //    return claveBase + dv.ToString();
        //}

        public static RespuestaDevlucion validaClienteDevolucion(string identificacion, string claveClte, decimal montoIva)
        {
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            RespuestaDevlucion objRespuesta = new RespuestaDevlucion();


            try
            {
                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, "Exec spValidaClienteDevolucionIVA", Environment.NewLine);
                sQuery = string.Concat(sQuery, $" @identificacion = '{identificacion}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $", @claveClte = '{claveClte}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $", @montoIva = {montoIva}", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(sQuery);

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            objRespuesta = new RespuestaDevlucion();
                            objRespuesta.codigo = data["codError"].ToString();
                            objRespuesta.mensaje = data["msjError"].ToString();

                            if (objRespuesta.codigo != "0")
                            {
                                return objRespuesta;
                            }

                            objRespuesta.beneficiario = new beneficiario();
                            objRespuesta.beneficiario.ruc = data["ACCOUNTNUM"].ToString();
                            objRespuesta.beneficiario.nombre = data["nombre"].ToString();
                            objRespuesta.beneficiario.periodo = data["periodo"].ToString();
                            objRespuesta.beneficiario.montoDevolver = decimal.Parse(data["montoDevolver"].ToString());
                            objRespuesta.beneficiario.estado = data["estado"].ToString();
                        }
                    }
                    else {
                        objRespuesta = new RespuestaDevlucion();
                        objRespuesta.codigo = "-9999";
                        objRespuesta.mensaje = "No se han encontrado registros.";
                        return objRespuesta;
                    }
                }


                return objRespuesta;

            }
            catch (Exception ex)
            {
                objRespuesta = new RespuestaDevlucion();
                objRespuesta.codigo = "-9998";
                objRespuesta.mensaje = "ERROR: " + ex.Message;
                return objRespuesta;
            }

        }

        public static RespuestaDevlucion devolucionesIndividualesRecepciones(string identificacion, string claveClte, Factura factura, string BeaerToken)
        {
            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            RespuestaDevlucion objRespuesta = new RespuestaDevlucion();
            string claveAccesoComprobante = string.Empty;

            try
            {

                SRI_DevolucionIVA_Fact request = new SRI_DevolucionIVA_Fact
                {
                    rucEmisor = factura.Ruc_matriz,
                    //claveAccesoComprobante = factura.generarClaveAccesoSRI("F"),
                    claveAccesoComprobante = factura.ClaveAccesoSRI,
                    idBeneficiario = factura.ClienteIdentificacion,
                    codigoBeneficio = claveClte,
                    baseImponible = factura.GetBase12() + factura.GetBase0(),
                    porcentajeIva = 4.0M,
                    montoIva = factura.getIVA()
                };

       
                string url = $"https://celcer.sri.gob.ec/devolucion-iva/rest/devolucionesIndividualesRecepciones";
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"url: {url}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"request: {request}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"BeaerToken: {BeaerToken}");


                string response = GetWebContent(url, request, BeaerToken);
                
                if(response == null)
                {
                    objRespuesta = new RespuestaDevlucion();
                    objRespuesta.codigo = "-1";
                    objRespuesta.mensaje = "Error: ";
                    objRespuesta.claveAccesoComprobante = request.claveAccesoComprobante;
                    return objRespuesta;
                }

                try
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"response: {response}");

                    Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(response);
                    objRespuesta.codigo = (string)jsonObject["codigo"];
                    objRespuesta.mensaje = (string)jsonObject["mensaje"];
                    objRespuesta.montoIvaDevolver = decimal.Parse(jsonObject["montoIvaDevolver"].ToString());
                    objRespuesta.claveAccesoComprobante = request.claveAccesoComprobante;


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.codigo: {objRespuesta.codigo}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.mensaje: {objRespuesta.mensaje}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.montoIvaDevolver: {objRespuesta.montoIvaDevolver}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.claveAccesoComprobante: {objRespuesta.claveAccesoComprobante}");


                    return objRespuesta;

                }
                catch (Exception ex)
                {


                    objRespuesta = new RespuestaDevlucion();
                    objRespuesta.codigo = "-1";
                    objRespuesta.mensaje = "Error: " + ex.Message;
                    objRespuesta.claveAccesoComprobante = request.claveAccesoComprobante;


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.codigo: {objRespuesta.codigo}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.mensaje: {objRespuesta.mensaje}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.montoIvaDevolver: {objRespuesta.montoIvaDevolver}");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"objRespuesta.claveAccesoComprobante: {objRespuesta.claveAccesoComprobante}");

                    return objRespuesta;
                }

            }
            catch (Exception ex)
            {
                objRespuesta = new RespuestaDevlucion();
                objRespuesta.codigo = "-9998";
                objRespuesta.mensaje = "ERROR: " + ex.Message;
                return objRespuesta;
            }

        }



    public static RespuestaDevlucion devolucionesIndividualesRecepciones(string claveClte, Factura factura, string bearerToken)
    {
        // --- LOG ADICIONAL ---
        // Log de Entrada y medición de tiempo total
        var watchTotal = Stopwatch.StartNew();
        string tokenParcial = (bearerToken != null && bearerToken.Length > 6) ? bearerToken.Substring(bearerToken.Length - 6) : "TOKEN_NULO_O_VACIO";
        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_Enter",$"Iniciando devolucionesIndividualesRecepciones. ClaveClte: {claveClte}, Token (Parcial): ...{tokenParcial}");

        var objRespuesta = new RespuestaDevlucion();

        //validaciones
        if (factura == null)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "Recepcion_Error", "El objeto factura es NULL");
            objRespuesta.codigo = "-998";
            objRespuesta.mensaje = "Factura no puede ser NULL";

            // --- LOG ADICIONAL ---
            watchTotal.Stop();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warn, "General", "Recepcion_Exit_Validation",$"Saliendo por validación (Factura NULL). Duración: {watchTotal.ElapsedMilliseconds}ms");
            return objRespuesta;
        }

        if (string.IsNullOrEmpty(factura.ClaveAccesoSRI))
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "Recepcion_Error", "ClaveAccesoSRI es NULL o vacía");
            objRespuesta.codigo = "-997";
            objRespuesta.mensaje = "ClaveAccesoSRI no puede estar vacía";

            // --- LOG ADICIONAL ---
            watchTotal.Stop();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warn, "General", "Recepcion_Exit_Validation",$"Saliendo por validación (ClaveAccesoSRI vacía). Duración: {watchTotal.ElapsedMilliseconds}ms");
            return objRespuesta;
        }

        // ... (tu lógica de 'claveAccesoLocal' y logs de 'Recepcion_Debug' están perfectos) ...
        string claveAccesoLocal = factura.ClaveAccesoSRI;
        objRespuesta.claveAccesoComprobante = claveAccesoLocal;
        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_Debug",$"ClaveAccesoSRI recibida: {factura.ClaveAccesoSRI}");
        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_Debug",$"Monto Iva Delvolver: {factura._montoIvaDevolver}");

        try
        {
            // 1. Crear el objeto de la petición (Request)
            var request = new RecepcionRequest
            {
                // ... (tus asignaciones de request) ...
                rucEmisor = factura.Ruc_matriz,
                claveAccesoComprobante = factura.ClaveAccesoSRI,
                idBeneficiario = factura.ClienteIdentificacion,
                codigoBeneficio = claveClte,
                baseImponible = factura.GetBase12(),
                porcentajeIva = 4.0M,
                montoIva = factura.getIVA()
            };

            string url = Control.Common.GlobalParameters.SRI_URL_DEV_INDV; // URL parametrizada

            // --- LOG ADICIONAL ---
            // ¡CRÍTICO! Loguear la URL a la que te conectas (¿Pruebas o Producción?)
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_URL",$"Conectando a URL: {url}");

            // (Este log tuyo es excelente)
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_Start", JsonConvert.SerializeObject(request));

            // --- LOG ADICIONAL ---
            // Iniciar cronómetro SÓLO para la llamada al SRI
            var watchSRI = Stopwatch.StartNew();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(url, content).Result;

                // --- LOG ADICIONAL ---
                watchSRI.Stop(); // Detener cronómetro del SRI

                string responseBody = response.Content.ReadAsStringAsync().Result;

                // --- LOG ADICIONAL ---
                // ¡EL LOG MÁS CRÍTICO QUE FALTA!
                // Te dice el código HTTP (401=Token Malo, 400=Request Malo, 500=SRI Caído)
                // y cuánto se demoró en responder.
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_HttpResponse",$"Código HTTP: {(int)response.StatusCode} ({response.ReasonPhrase}). Duración SRI: {watchSRI.ElapsedMilliseconds}ms");

                // (Este log tuyo es excelente)
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_Response", responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    // ... (tu lógica de deserialización de error) ...
                    var sriError = JsonConvert.DeserializeObject<SriApiResponse>(responseBody);
                    objRespuesta.codigo = sriError.Codigo;
                    objRespuesta.mensaje = sriError.Mensaje;
                    //...
                }
                else
                {
                    // ... (tu lógica de deserialización de éxito) ...
                    var sriSuccess = JsonConvert.DeserializeObject<SriApiResponse>(responseBody);
                    objRespuesta.codigo = sriSuccess.Codigo;
                    objRespuesta.mensaje = sriSuccess.Mensaje;
                    objRespuesta.montoIvaDevolver = sriSuccess.MontoIvaDevolver ?? 0;  // agregado para que se asigne el valor monto de iva a de volver 
                    //...
                }
            }
        }
        catch (Exception ex)
        {
            // (Este log tuyo es excelente, solo le agrego el tiempo total)
            watchTotal.Stop();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "Recepcion_Failed",$"Excepción en Recepción. Duración: {watchTotal.ElapsedMilliseconds}ms. Error: {ex.ToString()}");

            objRespuesta.codigo = "-999";
            objRespuesta.mensaje = ex.Message;

            // --- LOG ADICIONAL ---
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "Recepcion_Exit_Exception",$"Saliendo por Excepción. Código: {objRespuesta.codigo}, Mensaje: {objRespuesta.mensaje}");
            return objRespuesta;
        }

        // --- LOG ADICIONAL ---
        // Log de salida final exitoso
        watchTotal.Stop();
        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Recepcion_Exit_Success",$"Recepción finalizada. Duración: {watchTotal.ElapsedMilliseconds}ms. Código: {objRespuesta.codigo}, Mensaje: {objRespuesta.mensaje}");

        return objRespuesta;
    }


    public static RespuestaDevlucion devolucionesIndividualesAnulaciones(string identificacion, string claveClte, Factura factura
            , string BeaerToken, datosDocumnetos datosDocumnetos )
        {
            RespuestaDevlucion objRespuesta = new RespuestaDevlucion();

            try
            {

                anulacionDevolucionIndividual request = new anulacionDevolucionIndividual
                {
                    rucEmisor = datosDocumnetos.ruc_matriz,
                    claveAccesoComprobante = datosDocumnetos.claveAccesoComprobante,
                    idBeneficiario = datosDocumnetos.identificacion,
                    codigoBeneficio = claveClte,
                    baseImponible = datosDocumnetos.baseImponible,
                    porcentajeIva = 4.0M,
                    montoIva = datosDocumnetos.montoIva,
                    montoIvaDevolver = datosDocumnetos.montoIvaDevolver
                };

                string url = $"https://celcer.sri.gob.ec/devolucion-iva/rest/devolucionesIndividualesAnulaciones";

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesAnulaciones", $"url: {url}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesAnulaciones", $"request: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesAnulaciones", $"BeaerToken: {BeaerToken}");

                string response = GetWebContentAnulacion(url, request, BeaerToken);

                if (response == null)
                {
                    objRespuesta.codigo = "-1";
                    objRespuesta.mensaje = "No se obtuvo respuesta del servidor.";
                    objRespuesta.claveAccesoComprobante = request.claveAccesoComprobante;
                    return objRespuesta;
                }

                Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(response);
                objRespuesta.codigo = (string)jsonObject["codigo"];
                objRespuesta.mensaje = (string)jsonObject["mensaje"];

                objRespuesta.claveAccesoComprobante = request.claveAccesoComprobante;


                

                if (objRespuesta.codigo == "2000")
                {
                    try
                    {
                        datosDocumnetos.montoIva = decimal.Parse(jsonObject["montoIva"].ToString());
                        datosDocumnetos.montoIvaDevolver = decimal.Parse(jsonObject["montoIvaDevolver"].ToString());
                    }
                    catch (Exception)
                    {
                        datosDocumnetos.montoIva = 0;
                        datosDocumnetos.montoIvaDevolver = 0;
                    }

                    objRespuesta.montoIvaDevolver = datosDocumnetos.montoIvaDevolver;
                }
                    

               

                return objRespuesta;
            }
            catch (Exception ex)
            {
                objRespuesta.codigo = "-9998";
                objRespuesta.mensaje = "ERROR: " + ex.Message;
                return objRespuesta;
            }
        }

        public static RespuestaDevlucion devolucionesIndividualesAnulaciones(string claveClte, datosDocumnetos datosDocumentos, string bearerToken)
        {
            var objRespuesta = new RespuestaDevlucion();
            objRespuesta.claveAccesoComprobante = datosDocumentos.claveAccesoComprobante; // Asignar siempre

            RespuestaToken respuestaToken = new RespuestaToken();


            try
            {

                if (string.IsNullOrEmpty(bearerToken))
                {
                    bearerToken = Control.Common.GlobalParameters.BEAER_TOCKEN;

                    if (string.IsNullOrEmpty(bearerToken))
                    {
                        respuestaToken = recuperaTocken(datosDocumentos.ruc_matriz);
                        if (respuestaToken.codigo == 0)
                        {
                            bearerToken = respuestaToken.access_token;
                        }
                        else
                        {
                            objRespuesta.codigo = respuestaToken.codigo.ToString();
                            objRespuesta.mensaje = respuestaToken.mensaje;
                            return objRespuesta;
                        }
                    }
                }

                // 1. Crear el objeto de la petición (Request)
                var request = new AnulacionRequest
                {
                    rucEmisor = datosDocumentos.ruc_matriz,
                    claveAccesoComprobante = datosDocumentos.claveAccesoComprobante,
                    idBeneficiario = datosDocumentos.identificacion,
                    codigoBeneficio = claveClte,
                    baseImponible = datosDocumentos.baseImponible,
                    porcentajeIva = 4.0M,
                    montoIva = datosDocumentos.montoIva,
                    montoIvaDevolver = datosDocumentos.montoIvaDevolver
                };

                //string url = "https://celcer.sri.gob.ec/devolucion-iva/rest/devolucionesIndividualesAnulaciones";
                string url = Control.Common.GlobalParameters.SRI_URL_DEV_INDV;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "Anulacion_Start", JsonConvert.SerializeObject(request));

                // 2. Llamar al API Client y obtener la respuesta tipada
                var sriResponse = SriApiClient.Post<SriApiResponse>(url, request, bearerToken);

                // 3. Poblar la respuesta de éxito
                objRespuesta.codigo = sriResponse.Codigo;
                objRespuesta.mensaje = sriResponse.Mensaje;
                objRespuesta.pinClteDevolucion = claveClte;
                objRespuesta.montoIvaDevolver = sriResponse.MontoIvaDevolver ?? datosDocumentos.montoIvaDevolver;
            }
            catch (Exception ex)
            {
                // 4. Manejo centralizado de errores
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "Anulacion_Failed", ex.ToString());
                objRespuesta.codigo = "-999";
                objRespuesta.mensaje = ex.Message;
            }
            return objRespuesta;
        }
  

        public static string ComputeSha512(string input)
        {
            using (var sha512 = new SHA512CryptoServiceProvider())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string ComputeSha512Hash(string input)
        {
            // Convertir el texto a bytes
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Crear el objeto SHA512
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // Opción 1: Devolver como cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.AppendFormat("{0:x2}", b);
                }
                return builder.ToString(); // Ej: a1b2c3d4e5f6...
            }
        }

        public static string ComputeSha512HashBase64(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // Opción 2: Devolver como Base64
                return Convert.ToBase64String(hashBytes);
            }
        }

        private static string ExtractJsonToken(string json)
        {
            // Este es un extractor simple de JSON (no uses en producción sin mejorarlo)
            int start = json.IndexOf("\"access_token\":\"") + 15;
            int end = json.IndexOf("\"", start + 1);
            if (start > 0 && end > start)
            {
                return json.Substring(start, end - start);
            }

            return null;
        }


        public static RespuestaToken GetBearerToken(string ruc, string cedula, string clave)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "AuthHash_Enter",$"Iniciando GetBearerToken (AuthHash). RUC: {ruc}, Cédula: {cedula}");

            var watchTotal = Stopwatch.StartNew(); // iniciar un cronometro para medir la conexion.
          

            RespuestaToken respuestaToken = new RespuestaToken();

            try
            {
                string claveSha512 = ComputeSha512(clave);
                string hashHex = ComputeSha512Hash(clave);
                string hashBase64 = ComputeSha512HashBase64(clave);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "AuthHash_Compute",$"Clave hasheada (Hex): {hashHex}");

                // Codificar los parámetros para evitar caracteres no permitidos en URL
                string encodedRuc = Uri.EscapeDataString(ruc);
                string encodedCedula = Uri.EscapeDataString(cedula);
                string encodedClave = Uri.EscapeDataString(hashHex);
                string baseUrl = Control.Common.GlobalParameters.SRI_URL_ACCESS_TOKEN;

                // Construir la URL correctamente
                //string url = $"https://celcer.sri.gob.ec/sri-seguridad-sso-api-servicio-internet/rest/seguridad-sso-rest/access-token/{encodedRuc}[AD]{encodedCedula}/{encodedClave}";
                string url = $"{baseUrl}{encodedRuc}[AD]{encodedCedula}/{encodedClave}";

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "AuthHash_Request",$"Contactando URL: {url}");

                string response = GetWebContent(url);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "AuthHash_Response",$"Body (Crudo): {response}");

                string accessToken = string.Empty;

                try
                {
                    Newtonsoft.Json.Linq.JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(response);
                    accessToken = (string)jsonObject["access_token"];

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "AuthHash_ParseSuccess","JSON parseado exitosamente. Access token extraído.");

                }
                catch (Exception ex)
                {

                   watchTotal.Stop(); // Detenemos el cronómetro aquí
                   Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "AuthHash_CRITICAL_JsonParseFail",$"FALLO al parsear JSON. Duración: {watchTotal.ElapsedMilliseconds}ms. Error: {ex.ToString()}. Respuesta recibida que falló: {response}");

                    accessToken = null;

                    respuestaToken = new RespuestaToken();
                    respuestaToken.codigo = -2;
                    respuestaToken.mensaje = "Error: " + ex.Message;
                    respuestaToken.access_token = null;
                    return respuestaToken;
                }
                

                respuestaToken = new RespuestaToken();
                respuestaToken.codigo = 0;
                respuestaToken.mensaje = "token generado correctamente";
                respuestaToken.access_token = accessToken;
                watchTotal.Stop();
                string tokenParcial = (accessToken != null && accessToken.Length > 6) ? accessToken.Substring(accessToken.Length - 6) : "N/A";
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "AuthHash_Exit",$"Saliendo con código: {respuestaToken.codigo}. Duración: {watchTotal.ElapsedMilliseconds}ms. Token (Parcial): ...{tokenParcial}");
                return respuestaToken;


            }
            catch (Exception ex)
            {
                watchTotal.Stop();
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "AuthHash_CRITICAL_Exception",$"Excepción CRÍTICA en GetBearerToken (AuthHash). Duración: {watchTotal.ElapsedMilliseconds}ms. Error: {ex.ToString()}");

                respuestaToken = new RespuestaToken();
                respuestaToken.codigo = -1;
                respuestaToken.mensaje = "Error: " + ex.Message;
                respuestaToken.access_token = null;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "General", "AuthHash_Exit",$"Saliendo con código: {respuestaToken.codigo}, Mensaje: {respuestaToken.mensaje}");

                return respuestaToken;
            }

        }

        public static RespuestaToken recuperaTocken(string ruc_matriz)
        {
            RespuestaToken BeaerToken = new RespuestaToken();
            string tokenBear = string.Empty;

            if (Control.Common.GlobalParameters.tokenResponse.ultimaAccess == null || Control.Common.GlobalParameters.tokenResponse.ultimaAccess.ToString("yyyyMMdd") == "00010101")
            {
                BeaerToken = Control.SRI.General.GetBearerToken(ruc_matriz,
                Common.GlobalParameters.SRI_USUARIO_ADICIONAL,
                Common.GlobalParameters.SRI_CLAVE_ADICIONAL);

                Control.Common.GlobalParameters.tokenResponse = BeaerToken;
                Control.Common.GlobalParameters.tokenResponse.ultimaAccess = DateTime.Now;
                tokenBear = Control.Common.GlobalParameters.tokenResponse.access_token;
            }

            if (string.IsNullOrEmpty(Control.Common.GlobalParameters.tokenResponse.access_token))
            {
                BeaerToken = Control.SRI.General.GetBearerToken(ruc_matriz,
                                        Common.GlobalParameters.SRI_USUARIO_ADICIONAL,
                                        Common.GlobalParameters.SRI_CLAVE_ADICIONAL);

                Control.Common.GlobalParameters.tokenResponse = BeaerToken;
                Control.Common.GlobalParameters.tokenResponse.ultimaAccess = DateTime.Now;
                tokenBear = Control.Common.GlobalParameters.tokenResponse.access_token;
            }


            DateTime ultimaFecha = Control.Common.GlobalParameters.tokenResponse.ultimaAccess;
            ultimaFecha = Control.Common.GlobalParameters.tokenResponse.ultimaAccess;
            TimeSpan diferencia = DateTime.Now - ultimaFecha;

            if (diferencia.TotalMinutes > 30)
            {
                BeaerToken = Control.SRI.General.GetBearerToken(ruc_matriz,
                Common.GlobalParameters.SRI_USUARIO_ADICIONAL,
                Common.GlobalParameters.SRI_CLAVE_ADICIONAL);
                Control.Common.GlobalParameters.tokenResponse = BeaerToken;
                Control.Common.GlobalParameters.tokenResponse.ultimaAccess = DateTime.Now;
            }
            else
            {
                BeaerToken = Control.Common.GlobalParameters.tokenResponse;
            }
            

            tokenBear = Control.Common.GlobalParameters.tokenResponse.access_token;
            return BeaerToken;
        }


        public static string GetWebContent(string url)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; // O Tls11 o Tls10 según la necesidad
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(url);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                return content;
            }
            catch (Exception ex)
            {
                // Manejar errores, como no se pudo conectar o la página no existe
                Console.WriteLine("Error al obtener contenido: " + ex.Message);
                return null;
            }
        }
        public static string GetWebContent(string url, recpecionDevolucionIndividual request, string bearerToken)
        {
            try
            {
                // Forzar TLS 1.2 - MUY IMPORTANTE para conexiones seguras
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Crear la solicitud
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers.Add("Authorization", "Bearer " + bearerToken);

                // Serializar el cuerpo
                string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                


                // Escribir el cuerpo de la solicitud
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonRequest);
                    streamWriter.Flush();
                }

                // Obtener respuesta
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string jsonResponse = streamReader.ReadToEnd();
                    Console.WriteLine("jsonResponse: " + jsonResponse);
                    return jsonResponse;
                }
            }
            catch (WebException ex)
            {
                // Leer el cuerpo del error
                using (var stream = ex.Response?.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string errorText = reader.ReadToEnd();

                    Console.WriteLine("Error al llamar al servicio: " + errorText);

                    // Devolver el JSON del error directamente
                    return errorText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
                return Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    codigo = "500",
                    mensaje = "Error interno: " + ex.Message,
                    montoIvaDevolver = 0
                });
            }
        }
        public static string GetWebContentAnulacion(string url, anulacionDevolucionIndividual request, string bearerToken)
        {
            try
            {
                // Forzar TLS 1.2 - MUY IMPORTANTE para conexiones seguras
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Crear la solicitud
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers.Add("Authorization", "Bearer " + bearerToken);

                // Serializar el cuerpo
                string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);



                // Escribir el cuerpo de la solicitud
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonRequest);
                    streamWriter.Flush();
                }

                // Obtener respuesta
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string jsonResponse = streamReader.ReadToEnd();
                    Console.WriteLine("jsonResponse: " + jsonResponse);
                    return jsonResponse;
                }
            }
            catch (WebException ex)
            {
                // Leer el cuerpo del error
                using (var stream = ex.Response?.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string errorText = reader.ReadToEnd();

                    Console.WriteLine("Error al llamar al servicio: " + errorText);

                    // Devolver el JSON del error directamente
                    return errorText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inesperado: " + ex.Message);
                return Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    codigo = "500",
                    mensaje = "Error interno: " + ex.Message,
                    montoIvaDevolver = 0
                });
            }
        }

        public static string GetWebContent(string url, object data, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    return null;
                }
            }
        }



       

        public static string GetBearerToken()
        {
            try
            {
                string ruc = Control.Common.GlobalParameters.SRI_RUC;
                string cedula = Control.Common.GlobalParameters.SRI_USUARIO_ADICIONAL;
                string clave = Control.Common.GlobalParameters.SRI_CLAVE_ADICIONAL;


                // Encriptar la clave con SHA512 en Base64
                string claveSha512 = ComputeSha512(clave);

                // Codificar los parámetros para la URL
                string encodedRuc = Uri.EscapeDataString(ruc);
                string encodedCedula = Uri.EscapeDataString(cedula);
                string encodedClave = Uri.EscapeDataString(claveSha512);

                // Construir la URL
                string url = $"https://celcer.sri.gob.ec/sri-seguridad-sso-api-servicio-internet/rest/seguridad-ssorest/accesstoken/ {encodedRuc}[AD]{encodedCedula}/{encodedClave}";

                // Crear solicitud
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";

                // Manejo de certificados SSL personalizados (si es necesario)
                // ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                // Ejecutar solicitud
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string jsonResponse = reader.ReadToEnd();

                    // Extraer el token (suponiendo que viene como {"access_token":"...","token_type":"..."})
                    var token = ExtractJsonToken(jsonResponse);
                    return token;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string errorText = reader.ReadToEnd();
                    Console.WriteLine("Error al obtener token: " + errorText);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general: " + ex.Message);
                return null;
            }
        }
        public static string ObtenerDevolucionesIndividualesRecepciones()
        {
            string url = "https://pre.sri.gob.ec/devolucion-iva/rest/devolucionesIndividualesRecepciones ";

            try
            {
                // Crear solicitud
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";

                // Si el servicio requiere autenticación por Token
                // request.Headers["Authorization"] = "Bearer tu_token_aqui";

                // Si hay certificados SSL autofirmados (solo en desarrollo)
                // System.Net.ServicePointManager.ServerCertificateValidationCallback =
                //     ((sender, certificate, chain, sslPolicyErrors) => true);

                // Ejecutar solicitud
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string errorText = reader.ReadToEnd();
                    return $"Error {((HttpWebResponse)ex.Response).StatusCode}: {errorText}";
                }
            }
            catch (Exception ex)
            {
                return $"Error general: {ex.Message}";
            }
        }

    }


}
