using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.PrinterRecipes
{
    public class VoucherTarjetaCredito
    {
        public VoucherTarjetaCredito()
        {
            NomTarjeta = string.Empty;
            NumTarjeta = string.Empty;
            VenTarjeta = string.Empty;
            NumLote = string.Empty;
            Adquiriente = string.Empty;
            FechaTrans = string.Empty;
            HoraTrans = string.Empty;
            Aprobacion = string.Empty;
            Secuencial = string.Empty;
            Factura = string.Empty;
            MID = string.Empty;
            TID = string.Empty;
            ModoLectura = string.Empty;
            TipoDebCredito = string.Empty;
            Transaccion = string.Empty;
            BaseIva = string.Empty;
            BaseSinIva = string.Empty;
            Subtotal = string.Empty;
            Iva = string.Empty;
            Intereses = string.Empty;
            ValorTotal = string.Empty;
            CodigoRed = string.Empty;
            NombreTarjetaHabiente = string.Empty;
            Emv = string.Empty;
            Arqc = string.Empty;
            Aidemv = string.Empty;
            Tc = string.Empty;
            Publicidad = string.Empty;
            TVR = string.Empty;
            TSI = string.Empty;
            MID2 = string.Empty;
        }

        public string NomTarjeta { get; set; }
        public string NumTarjeta { get; set; }
        public string VenTarjeta { get; set; }
        public string NumLote { get; set; }
        public string Adquiriente { get; set; }
        public string FechaTrans { get; set; }
        public string HoraTrans { get; set; }
        public string Aprobacion { get; set; }
        public string Secuencial { get; set; }
        public string Factura { get; set; }
        public string MID { get; set; }
        public string MID2 { get; set; }
        public string TID { get; set; }
        public string ModoLectura { get; set; }
        public string TipoDebCredito { get; set; }
        public string Transaccion { get; set; }
        public string BaseIva { get; set; }
        public string BaseSinIva { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }
        public string Intereses { get; set; }
        public string ValorTotal { get; set; }
        public string CodigoRed { get; set; }

        private string _pagare = "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE Y SIN \nPROTESTO EL TOTAL DE ESTE PAGARE MAS LOS INTERESES \nY CARGOS POR SERVICIO. EN CASO DE MORA PAGARE LA \nTASA MAXIMA AUTORIZADA POR EL EMISOR. DECLARO \nQUE EL PRODUCTO DE ESTA TRANSACCION NO SERA \nUTILIZADO EN ACTIVIDADES DE LAVADO DE ACTIVOS, \nFINANCIAMIENTO DEL TERRORISMO Y OTROS DELITOS ";
        public string Pagare { get { return _pagare; } set { _pagare = value; } }
        public string NombreTarjetaHabiente { get; set; }
        public string Emv { get; set; }
        public string Arqc { get; set; }
        public string Aidemv { get; set; }
        public string Tc { get; set; }
        public string Publicidad { get; set; }
        public string TVR { get; set; }
        public string TSI { get; set; }
        
    }
}
