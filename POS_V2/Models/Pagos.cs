using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace POS.Models
{
    public class Pago : INotifyPropertyChanged
    {

        string _descripcion;

        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; OnPropertyChanged("Descripcion"); }
        }

        decimal _valor;

        public decimal Valor
        {
            get { return _valor; }
            set { _valor = value; OnPropertyChanged("Valor"); }
        }

        string cliente;

        public string Cliente
        {
            get { return cliente; }
            set { cliente = value; OnPropertyChanged("Cliente"); }
        }

        string numBin;
        public string NumBin
        {
            get { return numBin; }
            set { numBin = value; OnPropertyChanged("NumeroBin"); }
        }
        BindingList<PagoBase> _pagos;

        public BindingList<PagoBase> Pagos
        {
            get { return _pagos; }
            set { _pagos = value; }
        }

        public Pago()
        {
            this._pagos = new BindingList<PagoBase>();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void calcularTotal()
        {
            if (this._descripcion != "EFECTIVO")// && this._descripcion != "DESCUENTO PROMOCION (-)") // Adición de Descuento Española
            {
                this.Valor = this.Pagos.Sum(x => x.Valor);
            }
        }
    }

    /***********************************************************/
    public class PagoBase
    {
        string _codigo;

        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        decimal _valor;

        public decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }
    }
    public class PagoTarjetaCredito : PagoBase
    {

        string _banco;

        public string Banco
        {
            get { return _banco; }
            set { _banco = value; }
        }

        string _nombre;

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        string _marca;

        public string Marca
        {
            get { return _marca; }
            set { _marca = value; }
        }

        string _tipoPos;

        public string TipoPos
        {
            get { return _tipoPos; }
            set { _tipoPos = value; }
        }

        string _numeroBin;
            public string NumeroBin
        {
            get { return _numeroBin; }
            set { _numeroBin = value; }
        }
        string _binDescripcion;

        public string BinDescripcion
        {
            get { return _binDescripcion; }
            set { _binDescripcion = value; }
        }
    }
    public class PagoCheque : PagoBase
    {
        string _banco;

        public string Banco
        {
            get { return _banco; }
            set { _banco = value; }
        }

        string _numero;

        public string Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        string _cuenta;

        public string Cuenta
        {
            get { return _cuenta; }
            set { _cuenta = value; }
        }
    }
    public class PagoGiftCard : PagoBase
    {
        decimal _saldo;

        public decimal Saldo
        {
            get { return _saldo; }
            set { _saldo = value; }
        }

        public bool EstaAsociadaGrupoCliente { get; set; }
        public string IdentificacionGrupoCliente { get; set; }
        public string NombreGrupoCliente { get; set; }
    }
    public class PagoTarjetaInterna : PagoBase
    {
        string _titular;

        public string Titular
        {
            get { return _titular; }
            set { _titular = value; }
        }
    }
    public class PagoRetencion : PagoBase
    { }
    public class PagoNotaCredito : PagoBase
    { }
    public class PagoDescuentoEspanola : PagoBase
    { }
    public class PagoMonedero : PagoBase
    { }
    public class PagoDelivery : PagoBase
    { }
}
