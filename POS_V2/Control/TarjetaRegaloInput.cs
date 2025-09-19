using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace POS.Control
{
    public partial class TarjetaRegaloInput : Telerik.WinControls.UI.RadForm
    {
        public decimal Valor { get; set; }
        public TarjetaRegaloInput()
        {
            InitializeComponent();
        }

        private void btnEvent(object sender, EventArgs e)
        {
            var button = sender as System.Windows.Forms.Control;
            txtValor.Text = txtValor.Text + button.Text;
            txtValor.Focus();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (txtValor.Text.Length > 1)
                txtValor.Text = txtValor.Text.Substring(0, txtValor.Text.Length - 1);
            else
                txtValor.Text = "";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            decimal val = 0M;
            if (!decimal.TryParse(txtValor.Text, out val) || val==0M)
            {
                this.txtValor.Focus();
                this.txtValor.SelectAll();
                return;
            }
            this.Valor = val;
            this.Close();
        }
    }
}
