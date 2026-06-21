using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace POS.Control.SRI
{
    public class SriTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }

    // --- Modelos para Recepción de Devolución ---
    public class RecepcionRequest
    {
        [JsonProperty("rucEmisor")]
        public string rucEmisor { get; set; }
        [JsonProperty("claveAccesoComprobante")]
        public string claveAccesoComprobante { get; set; }
        [JsonProperty("idBeneficiario")]
        public string idBeneficiario { get; set; }
        [JsonProperty("codigoBeneficio")]
        public string codigoBeneficio { get; set; }
        [JsonProperty("baseImponible")]
        public decimal baseImponible { get; set; }
        [JsonProperty("porcentajeIva")]
        public decimal porcentajeIva { get; set; }
        [JsonProperty("montoIva")]
        public decimal montoIva { get; set; }
    }

    // --- Modelos para Anulación ---
    public class AnulacionRequest : RecepcionRequest // Hereda porque son similares
    {
        [JsonProperty("montoIvaDevolver")]
        public decimal montoIvaDevolver { get; set; }
    }

    // --- Modelo de Respuesta Común para Recepción y Anulación ---
    public class SriApiResponse
    {
        [JsonProperty("codigo")]
        public string Codigo { get; set; }
        [JsonProperty("mensaje")]
        public string Mensaje { get; set; }
        [JsonProperty("montoIvaDevolver")]
        public decimal? MontoIvaDevolver { get; set; } // Usamos decimal? para que sea opcional
    }

}
