using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.SRI
{
    public class PinCliente
    {
        public string Pin { get; private set; }
        public string identificacion { get; set; }
        public decimal montoFactura { get; set; }
        public decimal montoIVA { get; set; }
        public string ruc_matriz { get; set; }
        public Factura _factura { get; set; }
        public decimal montoIva { get; set; }
        public decimal montoIvaDevolver { get; set; }
        public string ClaveAccesoSRI { get; set; }
        public string CodigoMensaje { get; set; }
        public string Mensaje { get; set; }
        public string claveClte { get; set; }
        public string claveAccesoComprobanteNC { get; set; }
        public string tipoComprobante { get; set; }
        public bool aplicaBeneficioDevolucionIVA { get; set; }
        public int numSecuenciaNC { get; set; }
    }
}
