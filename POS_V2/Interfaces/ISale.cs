using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;

namespace POS.Interfaces
{
    public interface ISale
    {
        string Establecimiento { get; set; }
        string PtoEmision { get; set; }
        string ClienteIdentificacion { get; set; }
        BindingList<Pago> Pagos { get; set; }
        core_tarjetacreditointerno TarjetaCreditoInterno { get; set; }
        core_tarjetacreditointerno TarjetaCreditoInternoAdicional { get; set; }
        bool EsClienteApp { get; set; }
        string CodigoClienteApp { get; set; }
        string BinNumeroTarjetaPromo { get; set; }
        bool AplicaDescuentoPromoBines { get; set; }
        decimal GetSaldoMonedero();
        string GetNumeroFactura();
        string GetNumeroFacturaEnmascarado();
        decimal GetTotal();
        decimal GetBase0();
        decimal GetDescuentos();
        decimal GetBase12();
        decimal GetBase12Desc();
        decimal GetPromoIva();
        decimal GetBase12DescPromoIVA(bool verExcluidos);
        void CalcularPagos();
        void AgregarPagoTarjetaCredito(decimal valor, string banco, string nombre, string marca, string tipoPos, string numBin, string bin_descripcion = "N/A");
        void AgregarPagoCheque(decimal valor, string banco, string numero, string cuenta);
        void AgregarPagoTarjetaRegalo(decimal valor, string codigo, decimal saldo, bool estaAsociadaGrupoCliente, string identificacionGrupoCliente, string nombreGrupoCliente, string tipo = "GIFT CARD");
        void AgregarPagoNotaCredito(decimal valor, string codigo);
        void AgregarPagoTarjetaInterna(decimal valor, string codigo, string titularIdentificacion);
        void AgregarPagoMonedero(decimal valor);
        void AgregaOrdenApp(string ordenApp);
        void AgregarDescuentoPagoCompraGratis(decimal valor, string codigo);


    }
}
