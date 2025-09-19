using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class RutaImagen
    {
        public string _RutaImagen;
        public string _Establecimiento;
        public string _EstablecimientoAxCode;
        public Image _imgenEstablecimiento;

        public string Ruta
        {
            get { return _RutaImagen; }
            set { _RutaImagen = value; }
        }
        public string Establecimiento
        {
            get { return _Establecimiento; }
            set { _Establecimiento = value; }
        }

        public string EstablecimientoAxCode
        {
            get { return _EstablecimientoAxCode; }
            set { _EstablecimientoAxCode = value; }
        }

        public Image imgenEstablecimiento
        {
            get { return _imgenEstablecimiento; }
            set { _imgenEstablecimiento = value; }
        }


    }
}
