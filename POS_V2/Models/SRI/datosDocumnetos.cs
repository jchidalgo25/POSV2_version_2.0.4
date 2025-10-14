using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.SRI
{
    public class datosDocumnetos
    {
        public string ruc_matriz { get; set; }
        public string tipoDocumnto { get; set; }
        public string tipoComprobante { get; set; }
        public string identificacion { get; set; }
        public long numSecuencia { get; set; }
        public decimal baseImponible { get; set; }
        public decimal montoIva { get; set; }
        public decimal montoIvaDevolver { get; set; }
        public string claveAccesoComprobante { get; set; }
        public bool aplicaBeneficioDevolucionIVA { get; set; }

    }
}
