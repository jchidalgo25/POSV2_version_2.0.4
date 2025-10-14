using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.DevolucionIVA
{
    public class SolicitudDevolucion
    {
        
    }
    public class beneficiario
    {
        public string ruc { get; set; }
        public string nombre { get; set; }
        public string periodo { get; set; }
        public decimal montoDevolver { get; set; }
        public string estado { get; set; }

    }

    public class recpecionDevolucionIndividual
    {
        public string rucEmisor { get; set; }
        public string claveAccesoComprobante { get; set; }
        public string idBeneficiario { get; set; }
        public string codigoBeneficio { get; set; }
        public decimal baseImponible { get; set; }
        public decimal porcentajeIva { get; set; }
        //public decimal codigoImptoIva { get; set; }
        public decimal montoIva { get; set; }
        

    }

    public class anulacionDevolucionIndividual
    {
        public string rucEmisor { get; set; }
        public string claveAccesoComprobante { get; set; }
        public string idBeneficiario { get; set; }
        public string codigoBeneficio { get; set; }
        public decimal baseImponible { get; set; }
        public decimal porcentajeIva { get; set; }
        public decimal montoIva { get; set; }
        public decimal montoIvaDevolver { get; set; }
    }


    public class RespuestaDevlucion
    {
        public string codigo { get; set; } = string.Empty;
        public string mensaje { get; set; } = string.Empty;
        public decimal montoIvaDevolver { get; set; } = 0;
        public beneficiario beneficiario { get; set; }
        public string claveAccesoComprobante { get; set; } = string.Empty;
        public string pinClteDevolucion { get; set; } = string.Empty;
        public bool aplicaBeneficioDevolucionIVA { get; set; } = false;
    }

    public class RespuestaToken
    {
        public int codigo { get; set; } = 0;
        public string mensaje { get; set; } = string.Empty;
        public string access_token { get; set; } = string.Empty;
        public DateTime ultimaAccess { get; set; }


    }
    public class DevolucionBeneficiarioRequest
    {
        public string rucEmisor { get; set; }
        public string claveAccesoComprobante { get; set; }
        public string idBeneficiario { get; set; }
        public string codigoBeneficio { get; set; }
        public decimal baseImponible { get; set; }
        public int porcentajeIva { get; set; }
        public decimal montoIva { get; set; }
    }

}
