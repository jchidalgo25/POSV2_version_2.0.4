using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Interfaces;

namespace POS.Models.Abstract
{
    public abstract class Sale : Interfaces.ISale
    {
        public string Establecimiento { get; set; }
        public string PtoEmision { get; set; }
        string ISale.ClienteIdentificacion { get; set; }
        bool ISale.EsClienteApp { get; set; }
        string ISale.CodigoClienteApp { get; set; }

        BindingList<Pago> ISale.Pagos { get; set; }
        core_tarjetacreditointerno ISale.TarjetaCreditoInterno { get; set; }
        core_tarjetacreditointerno ISale.TarjetaCreditoInternoAdicional { get; set; }
        string ISale.BinNumeroTarjetaPromo
        {
            get;
            set;
        }
        bool ISale.AplicaDescuentoPromoBines
        {
            get;
            set;
        }

        public abstract void CalcularPagos();

        public abstract void AgregarPagoTarjetaCredito(decimal valor, string banco, string nombre, string marca, string tipoPos, string numBin, string bin_descripcion);

        public abstract decimal GetTotal();

        public abstract decimal GetBase0();

        public abstract string GetNumeroFactura();

        public abstract string GetNumeroFacturaEnmascarado();

        void ISale.AgregarPagoCheque(decimal valor, string banco, string numero, string cuenta)
        {
            throw new NotImplementedException();
        }

        void ISale.AgregarPagoNotaCredito(decimal valor, string codigo)
        {
            throw new NotImplementedException();
        }

        void ISale.AgregarPagoTarjetaInterna(decimal valor, string codigo, string titularIdentificacion)
        {
            throw new NotImplementedException();
        }
        
        void ISale.AgregarPagoTarjetaRegalo(decimal valor, string codigo, decimal saldo, bool estaAsociadaGrupoCliente, string identificacionGrupoCliente, string nombreGrupoCliente, string tipo = "GIFT CARD")
        {
            throw new NotImplementedException();
        }

        void ISale.AgregarPagoMonedero(decimal valor)
        {
            throw new NotImplementedException();
        }

        // Agrega esto en Sale.cs para cumplir con la interfaz ISale

        void ISale.AgregarPagoCompraGratis(decimal valor, string codigo)
        {
            throw new NotImplementedException();
        }

        decimal ISale.GetBase12()
        {
            return 0M;
        }

        decimal ISale.GetBase12Desc()
        {
            return 0M;
        }

        decimal ISale.GetBase12DescPromoIVA(bool verExcluidos)
        {
            return 0M;
        }

        decimal ISale.GetDescuentos()
        {
            return 0M;
        }

        decimal ISale.GetPromoIva()
        {
            return 0M;
        }
        decimal ISale.GetSaldoMonedero()
        {
            return 0M;
        }

        void ISale.AgregaOrdenApp(string ordenApp)
        {
            throw new NotImplementedException();
        }
    }
}
