using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public partial class core_tarjetacredito
    {
        public string nombre_completo 
        { 
            get 
            {
                return this.nombre + " - " + this.tipo;
            } 
        }
    }
}
