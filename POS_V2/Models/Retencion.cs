using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace POS.Models
{
    public class Retencion
    {
        public bool result { get; set; }
        public string NumRetencion { get; set; }
        public string NumFactura { get; set; }
        public string NumAutorizacion { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public decimal ValorBase { get; set; }
        public decimal ValorRetFte { get; set; }
        public decimal ValorBaseIVA { get; set; }
        public decimal ValorRetIVA { get; set; }
        public string CodRetIVA { get; set; }
        public decimal CodPorcRetIVA { get; set; }
        public string ConceptoRetIVA { get; set; }
        public Nullable<decimal> valor_ret_fte175 { get; set; }
        public Nullable<decimal> valor_base175 { get; set; }
        public Nullable<decimal> valor_base1 { get; set; }
        public decimal ValorRetFte1 { get; set; }
    }
}
