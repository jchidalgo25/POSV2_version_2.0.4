using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.WalletPoints
{
    public class ClsListPoints
    {
        #region Propiedades Publicas

        public int idCampania { get; set; }
        public bool EsOpcionPuntosActiva { get { return _esOpcionPuntosActiva; } set { _esOpcionPuntosActiva = value; } }
        public decimal FactorAcumulacion { get; set; }
        public decimal FactorCanje { get; set; }
        public string PlantillaMonederoFactura { get; set; }
        public string PlantillaDebitoMonedero { get; set; }
        public bool DebeRecargarParametrosEnTiempoReal { get; set; }
        public string PtsCliente_XmlAcumulacion { get; set; }
        public string PtsCliente_XmlAcumulacionGen { get; set; }

        #endregion


        #region Atributos Privados

        private bool _esOpcionPuntosActiva = false;

        #endregion
    }
}
