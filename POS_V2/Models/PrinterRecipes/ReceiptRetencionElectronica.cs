using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.PrinterRecipes
{
    public class ReceiptRetencionElectronica
    {
        public ReceiptRetencionElectronica()
        {
            Oficina = string.Empty;
            Telefono = string.Empty;
            CajeroNombre = string.Empty;
            ClienteIdentificacion = string.Empty;
            ClienteNombre = string.Empty;
            ClienteDireccion = string.Empty;
            ClienteTelefono = string.Empty;
            ConceptoTransaccion = string.Empty;
            NumRetencion = string.Empty;
            NumFactura = string.Empty;
            NumAutorizacion = string.Empty;
            FechaAutorizacion = string.Empty;
            Total = string.Empty; 
        }
        
        public string Oficina { get; set; }
        public string Telefono { get; set; }    
        public string CajeroNombre { get; set; }
        public string ClienteIdentificacion { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteDireccion { get; set; }
        public string ClienteTelefono { get; set; }
        public string ConceptoTransaccion { get; set; }        
        public string NumRetencion { get; set; }
        public string NumFactura { get; set; }
        public string NumAutorizacion { get; set; }
        public string FechaAutorizacion { get; set; }
        public string Total { get; set; } 
    }
}
