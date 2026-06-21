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

namespace POS.Control.Main
{
    public partial class TempInvoiceAlert : Telerik.WinControls.UI.RadForm
    {
        public TempInvoiceAlert()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            Aceptar();
        }

        private void Aceptar()
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "TempInvoiceAlert", "Aceptar", "Usuario acepta que ha leído el manifesto y que la verificación de los items que se cargaron a la pantalla quedan bajo su responsabilidad. Usuario logon: " + (Control.Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : (Control.Common.GlobalParameters.UserObj.nombres + "(" + Control.Common.GlobalParameters.UserObj.username + ")")));
            this.Close();
        }

        private void chkAccept_CheckedChanged(object sender, EventArgs e)
        {
            btnAceptar.Enabled = chkAccept.Checked;
        }
    }
}
