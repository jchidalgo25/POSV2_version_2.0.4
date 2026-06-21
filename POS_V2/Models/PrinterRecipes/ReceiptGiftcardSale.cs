using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.PrinterRecipes
{
    public class ReceiptGiftcardSale
    {
        private string _oficina = string.Empty;
        public string Oficina { get { return _oficina; } set { _oficina = value; } }


        private string _telefono = string.Empty;
        public string Telefono { get { return _telefono; } set { _telefono = value; } }


        private string _nroComprobante = string.Empty;
        public string NroComprobante { get { return _nroComprobante; } set { _nroComprobante = value; } }


        private string _cajeroNombre = string.Empty;
        public string CajeroNombre { get { return _cajeroNombre; } set { _cajeroNombre = value; } }


        private string _clienteIdentificacion = string.Empty;
        public string ClienteIdentificacion { get { return _clienteIdentificacion; } set { _clienteIdentificacion = value; } }


        private string _clienteDireccion = string.Empty;
        public string ClienteDireccion { get { return _clienteDireccion; } set { _clienteDireccion = value; } }


        private DateTime _fechaTransaccion;
        public DateTime FechaTransaccion { get { return _fechaTransaccion; } set { _fechaTransaccion = value; } }


        private string _clienteNombre = string.Empty;
        public string ClienteNombre { get { return _clienteNombre; } set { _clienteNombre = value; } }


        private string _clienteTelefono = string.Empty;
        public string ClienteTelefono { get { return _clienteTelefono; } set { _clienteTelefono = value; } }


        private string _conceptoTransaccion = string.Empty;
        public string ConceptoTransaccion { get { return _conceptoTransaccion; } set { _conceptoTransaccion = value; } }


        private string _codigoTarjeta = string.Empty;
        public string CodigoTarjeta { get { return _codigoTarjeta; } set { _codigoTarjeta = value; } }


        private string _itemDescripcion = string.Empty;
        public string ItemDescripcion { get { return _itemDescripcion; } set { _itemDescripcion = value; } }


        private decimal _itemValor = 0M;
        public decimal ItemValor { get { return _itemValor; } set { _itemValor = value; } }


        private decimal _totalTransacccion = 0M;
        public decimal TotalTransacccion { get { return _totalTransacccion; } set { _totalTransacccion = value; } }


        private decimal _cambioTransacccion = 0M;
        public decimal CambioTransacccion { get { return _cambioTransacccion; } set { _cambioTransacccion = value; } }

        private decimal _monto = 0M;
        public decimal monto { get { return _monto; } set { _monto = value; } }

        private string _pieRecibo = string.Empty;
        public string PieRecibo { get { return _pieRecibo; } set { _pieRecibo = value; } }

        BindingList<Pago> _pagos = new BindingList<Pago>();

        public BindingList<Pago> Pagos
        {
            get { return _pagos; }
            set { _pagos = value; }
        }

        BindingList<core_giftcard> _LstGiftcard = new BindingList<core_giftcard>();
        public BindingList<core_giftcard> LstGiftcard
        {
            get { return _LstGiftcard; }
            set { _LstGiftcard = value; }
        }
    }
}
