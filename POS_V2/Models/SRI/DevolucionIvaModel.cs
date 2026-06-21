using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.SRI
{
    public class DevolucionIvaModel
    {
        private string _tipoDocumento;
        public string tipoDocumento
        {
            get { return _tipoDocumento; }
            set { _tipoDocumento = value; }
        }

        private string _establecimiento;
        public string establecimiento
        {
            get { return _establecimiento; }
            set { _establecimiento = value; }
        }

        private string _puntoEmision;
        public string puntoEmision
        {
            get { return _puntoEmision; }
            set { _puntoEmision = value; }
        }

        private string _numDocumento;
        public string numDocumento
        {
            get { return _numDocumento; }
            set { _numDocumento = value; }
        }
        private decimal _montoIvaDevolver;
        public decimal montoIvaDevolver
        {
            get { return _montoIvaDevolver; }
            set { _montoIvaDevolver = value; }
        }

        private string _ClaveAccesoSRI;
        public string ClaveAccesoSRI
        {
            get { return _ClaveAccesoSRI; }
            set { _ClaveAccesoSRI = value; }
        }

        private string _estado;
        public string estado
        {
            get { return _estado; }
            set { _estado = value; }
        }

        private string _cliente;
        public string cliente
        {
            get { return _cliente; }
            set { _cliente = value; }
        }



    }
}
