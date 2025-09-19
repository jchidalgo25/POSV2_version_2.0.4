using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class DatosFacturaCargados
    {
        public string CodigoCliente { get; set; }
        public bool EsEmpleadoLiris { get; set; }
        public bool EsTarjetaCreditoInterno { get; set; }
        public bool EsTarjetaCreditoInternoAdicional { get; set; }
        public bool EsClienteApp { get; set; }
        public string CodigoClienteApp { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public List<object> Pagos { get; set; } = new List<object>(); // Cambia 'object' por la clase base de tus pagos
    }
}
