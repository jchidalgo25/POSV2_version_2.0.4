using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class ClienteEmpleado
    {
        private bool _esUsoAppMovil = false;
        public bool EsUsoAppMovil
        {
            get { return _esUsoAppMovil; }
            set { _esUsoAppMovil = value; }
        }

        private string _tipoConsulta = string.Empty;
        public string tipoConsulta
        {
            get { return _tipoConsulta; }
            set { _tipoConsulta = value; }
        }

        private string _CodError = string.Empty;
        public string CodError
        {
            get { return _CodError; }
            set { _CodError = value; }
        }

        private string _MsjError = string.Empty;
        public string MsjError
        {
            get { return _MsjError; }
            set { _MsjError = value; }
        }
        private string _CodMensaje = string.Empty;
        public string CodMensaje
        {
            get { return _CodMensaje; }
            set { _CodMensaje = value; }
        }
        private string _TextMensaje = string.Empty;
        public string TextMensaje
        {
            get { return _TextMensaje; }
            set { _TextMensaje = value; }
        }
        private string _Identificacion = string.Empty;
        public string Identificacion
        {
            get { return _Identificacion; }
            set { _Identificacion = value; }
        }
        private string _NombreCliente = string.Empty;
        public string NombreCliente
        {
            get { return _NombreCliente; }
            set { _NombreCliente = value; }
        }
        private string _DireccionCliente = string.Empty;
        public string DireccionCliente
        {
            get { return _DireccionCliente; }
            set { _DireccionCliente = value; }
        }
        private string _TelefonoCliente = string.Empty;
        public string TelefonoCliente
        {
            get { return _TelefonoCliente; }
            set { _TelefonoCliente = value; }
        }
        private string _EmailCliente = string.Empty;
        public string EmailCliente
        {
            get { return _EmailCliente; }
            set { _EmailCliente = value; }
        }
        private string _GrupoCliente = string.Empty;
        public string GrupoCliente
        {
            get { return _GrupoCliente; }
            set { _GrupoCliente = value; }
        }
        private bool _EsClienteApp = false;
        public bool EsClienteApp
        {
            get { return _EsClienteApp; }
            set { _EsClienteApp = value; }
        }
        private string _CodigoClienteApp = string.Empty;
        public string CodigoClienteApp
        {
            get { return _CodigoClienteApp; }
            set { _CodigoClienteApp = value; }
        }
        private bool _EsEmpleadoLiris = false;
        public bool EsEmpleadoLiris
        {
            get { return _EsEmpleadoLiris; }
            set { _EsEmpleadoLiris = value; }
        }
        private decimal _PorcEmpleadoLiris = 0M;
        public decimal PorcEmpleadoLiris
        {
            get { return _PorcEmpleadoLiris; }
            set { _PorcEmpleadoLiris = value; }
        }
        private bool _ExisteCliente = false;
        public bool ExisteCliente
        {
            get { return _ExisteCliente; }
            set { _ExisteCliente = value; }
        }

        private bool _EsTarjetaEmpresa = false;
        public bool EsTarjetaEmpresa
        {
            get { return _EsTarjetaEmpresa; }
            set { _EsTarjetaEmpresa = value; }
        }
        private string _NumeroTarjetaEmpresa = string.Empty;
        public string NumeroTarjetaEmpresa
        {
            get { return _NumeroTarjetaEmpresa; }
            set { _NumeroTarjetaEmpresa = value; }
        }
        private decimal _SaldoTarjetaEmpresa = 0M;
        public decimal SaldoTarjetaEmpresa
        {
            get { return _SaldoTarjetaEmpresa; }
            set { _SaldoTarjetaEmpresa = value; }
        }
        private bool _EsTarjetaEmpresaAdicional = false;
        public bool EsTarjetaEmpresaAdicional
        {
            get { return _EsTarjetaEmpresaAdicional; }
            set { _EsTarjetaEmpresaAdicional = value; }
        }
        private string _NumeroTarjetaEmpresaAdicional = string.Empty;
        public string NumeroTarjetaEmpresaAdicional
        {
            get { return _NumeroTarjetaEmpresaAdicional; }
            set { _NumeroTarjetaEmpresaAdicional = value; }
        }
        private decimal _SaldoTarjetaEmpresaAdicional = 0M;
        public decimal SaldoTarjetaEmpresaAdicional
        {
            get { return _SaldoTarjetaEmpresaAdicional; }
            set { _SaldoTarjetaEmpresaAdicional = value; }
        }
        private bool _EsCompraGratis = false;
        public bool EsCompraGratis
        {
            get { return _EsCompraGratis; }
            set { _EsCompraGratis = value; }
        }
        public DateTime CompraGratisFechaIni { get; set; }
        public core_tarjetacreditointerno _tarjeta { get; set; }
        public core_tarjetacreditointerno tarjeta
        {
            get { return _tarjeta; }
            set { _tarjeta = value; }
        }

        public core_tarjetacreditointerno _tarjetaAdicional { get; set; }
        public core_tarjetacreditointerno tarjetaAdicional
        {
            get { return _tarjetaAdicional; }
            set { _tarjetaAdicional = value; }
        }

        public bool _EsClienteDuplicado { get; set; } = false;
        public bool EsClienteDuplicado
        {
            get { return _EsClienteDuplicado; }
            set { _EsClienteDuplicado = value; }
        }

        public bool _AcumulaBilletera { get; set; } = false;
        public bool AcumulaBilletera
        {
            get { return _AcumulaBilletera; }
            set { _AcumulaBilletera = value; }
        }

        private bool _tieneTajetaDelportal { get; set; }

        public bool tieneTajetaDelportal
        {
            get { return _tieneTajetaDelportal; }
            set { _tieneTajetaDelportal = value; }
        }
        private bool _tieneTajetaAdicionalDelportal { get; set; }

        public bool tieneTajetaAdicionalDelportal
        {
            get { return _tieneTajetaAdicionalDelportal; }
            set { _tieneTajetaAdicionalDelportal = value; }
        }


        private bool _esBeneficiarioDevolucionIVA { get; set; }
        public bool esBeneficiarioDevolucionIVA
        {
            get { return _esBeneficiarioDevolucionIVA; }
            set { _esBeneficiarioDevolucionIVA = value; }
        }


        private decimal _saldoDispDevolucionIVA { get; set; }
        public decimal saldoDispDevolucionIVA
        {
            get { return _saldoDispDevolucionIVA; }
            set { _saldoDispDevolucionIVA = value; }
        }


        private bool _esClienteEmpresarial { get; set; }
        public bool esClienteEmpresarial
        {
            get { return _esClienteEmpresarial; }
            set { _esClienteEmpresarial = value; }
        }

        private decimal _montoCreditoEmpresarial { get; set; }
        public decimal montoCreditoEmpresarial
        {
            get { return _montoCreditoEmpresarial; }
            set { _montoCreditoEmpresarial = value; }
        }

        public decimal SaldoApp { get; set; }

    }

  
    public class custtable
    {
        public string ACCOUNTNUM { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE { get; set; }
        public string CUSTGROUP { get; set; }
        public string EMAIL { get; set; }
        public string CELLULARPHONE { get; set; }
        public string PHONELOCAL { get; set; }
        public string CITY { get; set; }
        public string STREET { get; set; }
        	
    }

    public class custclassificationgroup
    {
        public string CODE { get; set; }
        public string TXT { get; set; }
        public string MAILADMINISTRARTIVO { get; set; }
        public string MAILRH { get; set; }
        public int DIASVENCIMIENTO { get; set; }
        public string CUSTID { get; set; }
        public bool BLOQUEOTEMPORAL { get; set; }
        public int DIACORTE2 { get; set; }
        public bool APLICAINTERES { get; set; }
        public decimal MONTO_CREDITO { get; set; }
        public decimal TASAINTERES { get; set; }

       
    }

}
