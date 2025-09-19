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

namespace POS.Control.CorrBan
{
    public partial class MoreAccounts : Telerik.WinControls.UI.RadForm
    {
        private List<Models.CorrBan.LikeAccount> _lista;

        public Models.CorrBan.LikeAccount SelectedAccount { get; set; }

        public MoreAccounts(List<Models.CorrBan.LikeAccount> lista)
        {
            InitializeComponent();
            _lista = lista;
        }

        private void MoreAccounts_Load(object sender, EventArgs e)
        {
            gridCuentas.DataSource = _lista;
        }

        private void btnElegir_Click(object sender, EventArgs e)
        {
            SelectAccount();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            SelectedAccount = null;
            this.Close();
        }

        private void SelectAccount()
        {
            if (gridCuentas.Rows.Count > 0)
            {
                SelectedAccount = gridCuentas.SelectedRows[0].DataBoundItem as Models.CorrBan.LikeAccount;
                this.Close();
            }
            else
            {
                //MessageBox.Show("No ha seleccionado una cuenta");
                Control.Common.General.GetMensajeToList(339);

            }
        }

        private void gridCuentas_CellDoubleClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            SelectAccount();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    this.Close();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
