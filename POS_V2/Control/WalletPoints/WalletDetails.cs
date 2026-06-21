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
    public partial class WalletDetails : Telerik.WinControls.UI.RadForm
    {

        #region Constructores

        public WalletDetails()
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
            List<WalletDetail> ListItems = new List<WalletDetail>();
            ListItems.Add(new WalletDetail { Campaign = "Shakira tour El Dorado", Total = 500 });
            ListItems.Add(new WalletDetail { Campaign = "Ultra Music Festival", Total = 500 });
            ListItems.Add(new WalletDetail { Campaign = "Campaña Navidad", Total = 500 });
            ListItems.Add(new WalletDetail { Campaign = "FIFA Qatar 2022", Total = 500 });

            gridPagos.DataSource = ListItems;
        }
        
        #endregion


    }
}
