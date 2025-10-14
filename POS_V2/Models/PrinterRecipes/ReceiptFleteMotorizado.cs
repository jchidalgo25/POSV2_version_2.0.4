using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.PrinterRecipes
{
    public class ReceiptFleteMotorizado
    {
        public ReceiptFleteMotorizado()
        {
            Oficina = string.Empty;
            Telefono = string.Empty;
            CajeroNombre = string.Empty;
            CajeroCedula = string.Empty;
            MotorizadoCodigo = string.Empty;
            MotorizadoNombre = string.Empty;
            OrdenApp = string.Empty;
            ClienteTelefono = string.Empty;
            ConceptoTransaccion = string.Empty;           
            NumFactura = string.Empty;
            FechaOrdenCompletada = string.Empty;
            Total = string.Empty;
            IdCaja = string.Empty;
            FechaRecibo = string.Empty;
        }

        public string Oficina { get; set; }
        public string Telefono { get; set; }
        public string CajeroNombre { get; set; }
        public string CajeroCedula { get; set; }
        public string MotorizadoCodigo { get; set; }
        public string MotorizadoNombre { get; set; }
        public string OrdenApp { get; set; }
        public string ClienteTelefono { get; set; }
        public string ConceptoTransaccion { get; set; }        
        public string NumFactura { get; set; }        
        public string FechaOrdenCompletada { get; set; }
        public string FechaRecibo { get; set; }
        public string Total { get; set; }
        public string IdCaja { get; set; }


    }
}
