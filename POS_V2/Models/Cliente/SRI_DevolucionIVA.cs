using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Cliente
{
    public class SRI_DevolucionIVA
    {



    }

    public class SRI_DevolucionIVA_Fact
    {
        public string rucEmisor { get; set; }
        public string claveAccesoComprobante { get; set; }
        public string idBeneficiario { get; set; }
        public string codigoBeneficio { get; set; }
        public decimal baseImponible { get; set; }
        public decimal porcentajeIva { get; set; }
        public decimal montoIva { get; set; }

    }

    public class SRI_DevolucionIVA_NC
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

}
