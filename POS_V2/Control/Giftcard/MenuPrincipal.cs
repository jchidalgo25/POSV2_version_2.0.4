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

namespace POS.Control.Giftcard
{
    public partial class MenuPrincipal : Telerik.WinControls.UI.RadForm
    {
        private bool _debeCambiarTipoDocumento = false;
        public bool DebeCambiarTipoDocumento { get { return _debeCambiarTipoDocumento; } set { _debeCambiarTipoDocumento = value; } }

        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void btnRecargas_Click(object sender, EventArgs e)
        {
            _debeCambiarTipoDocumento = true;
            this.Close();
        }

        private void btnPagoServicios_Click(object sender, EventArgs e)
        {
            var frm = new Control.Giftcard.GiftcardSale();
            frm.ShowDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
