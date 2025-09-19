using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control.Pagos
{
    public partial class DescuentoPromocion : Telerik.WinControls.UI.RadForm
    {
        public string codigo;
        string titulo;
        public DescuentoPromocion(string Titulo)
        {
            titulo = Titulo;
            InitializeComponent();
        }        

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)/* && (e.KeyChar != '.')*/)
            {
                e.Handled = true;
            }
        }

        private void txtDescuento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetCard();
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            GetCard();
        }

        private void GetCard()
        {
            if (txtDescuento.Text.ToString() != "" || txtDescuento.Text != null)
            {
                codigo = txtDescuento.Text.ToString();
                this.Close();
            }
            else
            {
                Control.Common.General.GetMensajeToList(427);
                //MessageBox.Show(this,"No ha deslizado la tarjeta de descuento", "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            txtDescuento.Clear();
            codigo = "CLOSEFORM";
            this.Close();
        }

        private void DescuentoPromocion_Load(object sender, EventArgs e)
        {
            lblcaption.Text = titulo;
        }



        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    txtDescuento.Clear();
                    codigo = "CLOSEFORM";
                    this.Close();

                    break;

                case Keys.Enter:
                    try
                    {
                        GetCard();
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.DescuentoPromocion", "ProcessCmdKey", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    }

                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);

        }



    }
}
