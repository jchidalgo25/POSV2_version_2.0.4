using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class PagoPinpad
    {
        public decimal Valor { get; set; }
        public string Banco { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string TipoPos { get; set; }
        public string NumBin { get; set; }
        public string BinDescripcion { get; set; }
        
    }

    public class PagoTarjetaInternaBk
    {
        public decimal Valor { get; set; }
        public string codigo { get; set; }
        public string titularidentificacion { get; set; }
    }



    




}
