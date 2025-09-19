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
    public partial class MenuPrincipal : Telerik.WinControls.UI.RadForm
    {
        private string IdentificacionCliente { get; set; }

        public MenuPrincipal(string identificacionCliente)
        {
            InitializeComponent();
            IdentificacionCliente = identificacionCliente;
        }

        private void btnRecargas_Click(object sender, EventArgs e)
        {
            Control.WalletPoints.TermsViewer frm = new Control.WalletPoints.TermsViewer();
            frm.ShowDialog();
        }

        private void btnPagoServicios_Click(object sender, EventArgs e)
        {
            if (IdentificacionCliente != string.Empty && IdentificacionCliente != Common.GlobalParameters.IdConsumidorFinal)
            {
                Control.WalletPoints.PortalCanje frm = new Control.WalletPoints.PortalCanje(IdentificacionCliente);
                frm.ShowDialog();
            }
            else
            {
                Control.Common.General.GetMensajeToList(557);
                //MessageBox.Show("Antes de acceder al portal de canjes debe haber cargado al cliente en la pantalla principal de POS. Recuerde que consumidor final no es válido");
            }
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
