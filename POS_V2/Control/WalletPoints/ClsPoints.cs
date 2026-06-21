using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.WalletPoints
{
    public static class ClsPoints
    {
        #region Propiedades Publicas

        public static bool EsOpcionPuntosActiva { get { return _esOpcionPuntosActiva; } set { _esOpcionPuntosActiva = value; } }
        public static decimal FactorAcumulacion { get; set; }
        public static decimal FactorCanje { get; set; }
        public static string PlantillaMonederoFactura { get; set; }
        public static string PlantillaDebitoMonedero { get; set; }
        public static bool DebeRecargarParametrosEnTiempoReal { get; set; }
        public static string PtsCliente_XmlAcumulacion { get; set; }
        public static string PtsCliente_XmlAcumulacionGen { get; set; }
        public static int IdCampania { get; set; }

        #endregion


        #region Atributos Privados

        private static bool _esOpcionPuntosActiva = false;

        #endregion
    }
}
