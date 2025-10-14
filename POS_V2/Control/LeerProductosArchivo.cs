using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control
{
    class LeerProductosArchivo
    {
        public string ARTICULO { get; set; }
        public string BARRAS { get; set; }
        public string CATEGORIA { get; set; }
        public string ESTABLECIMIENTO { get; set; }
        public int ORDEN { get; set; }
        public string campoConsulta { get; set; }
    }

    public class ProductoArticulo
    {
        public string ARTICULO { get; set; }
        public string BARRAS { get; set; }
        public string CATEGORIA { get; set; }
        public string ESTABLECIMIENTO { get; set; }
        public int ORDEN { get; set; }
        public string campoConsulta { get; set; }

    }

    public class ProductoArticulos
    {
        public int CodError { get; set; }
        public string MsjError { get; set; }
        public int ContRegistro { get; set; }
        public List<ProductoArticulo> ProductoArticuloList { get; set; }

    }

}
