using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.ToolBox
{
    public class ClsProveedorDomicilio
    {
        #region Propiedades Publicas
         
        public string Nombre { get; set; }
        public string RutaLogo { get; set; }
        public string Tipo { get; set; }
        public string FormaPago { get; set; }
        public string NombreImagenBoton { get; set; }
        public string Local { get; set; } 
        public bool Estado { get; set; }

        #endregion
         
    }
}
