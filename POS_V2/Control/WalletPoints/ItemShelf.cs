using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using POS.Models;

namespace POS.Control.WalletPoints
{
    public partial class ItemShelf : Telerik.WinControls.UI.RadForm
    {

        #region Constructores

        public ItemShelf()
        {
            InitializeComponent();
        }

        #endregion

        #region Atributos Privados

        System.Windows.Forms.Control focused;

        #endregion

        #region Metodos Controles

        private void PagosServicios_Load(object sender, EventArgs e)
        {
            List<ItemCampaign> ListItems = new List<ItemCampaign>();
            ListItems.Add(new ItemCampaign { Item = "Entrada General", Puntos = 500, Valor = 85 });
            ListItems.Add(new ItemCampaign { Item = "Entrada Cancha", Puntos = 500, Valor = 110 });
            ListItems.Add(new ItemCampaign { Item = "Entrada Tribuna", Puntos = 500, Valor = 350 });
            ListItems.Add(new ItemCampaign { Item = "Entrada Preferencia", Puntos = 500, Valor = 300 });
            ListItems.Add(new ItemCampaign { Item = "Entrada Pies Descalzos", Puntos = 500, Valor = 750 });
            ListItems.Add(new ItemCampaign { Item = "Entrada El Dorado Box", Puntos = 500, Valor = 1500 });

            gridPagos.DataSource = ListItems;
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            Control.WalletPoints.WalletDetails frmWalletDetails = new WalletDetails();
            frmWalletDetails.ShowDialog();
        }

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }
    }

    public class ItemCampaign
    {
        public string Item { get; set; }
        public decimal Valor { get; set; }
        public decimal Puntos { get; set; }
        public bool EsSeleccionado { get; set; }
    }

    public class WalletDetail
    {
        public string Campaign { get; set; }
        public decimal Total { get; set; }
    }

}
