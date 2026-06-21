using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.AppCupones
{
    public class CuponRespuesta
    {
        public bool EsValido { get; set; }
        public int CodigoMensaje { get; set; }
        public string Mensaje { get; set; }
        public int IdCupon { get; set; }
        public string Descripcion { get; set; }

        public string TipoDescuento { get; set; }
        public decimal ValorDescuento { get; set; }

        public bool PermiteCombinar { get; set; } // variable para permitir el uso de cupones grupales y individual jchid.

        public List<AlcanceCupon> Alcance { get; set; } = new List<AlcanceCupon>();
    }

    public class AlcanceCupon
    {
        public string TipoAlcance { get; set; }
        public string ValorAlcance { get; set; }
    }
}
