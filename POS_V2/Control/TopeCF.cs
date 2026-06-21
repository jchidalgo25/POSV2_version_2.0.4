using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;

namespace POS.Control
{
    public partial class TopeCF : Form
    {
        public TopeCF()
        {
            InitializeComponent();
        }

        private void TopeCF_Load(object sender, EventArgs e)
        {
            panel1.Left = (panel1.Width / 2)-pictureBox1.Width ;
            panel1.Top = panel1.Height / 2;
            using (POSEntities pos = new POSEntities())
            {
                lblMensaje.Text = "No puede tener consumos mayores a " + Control.Common.GlobalParameters.CUPO_CF.ToString() + " dólares como CONSUMIDOR FINAL. Por favor utilice cédula o RUC como identificación.";
            }
        }

        private void cmdCambiaCliente_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cmdFinalizaFactura_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
        }
        int cont;
        private void tmr_Tick(object sender, EventArgs e)
        {
            cont += 1;
            if (this.BackColor == Color.Red)
            {
                this.BackColor = Color.White;
                lblMensaje.ForeColor = Color.Red;
            }
            else
            {
                this.BackColor = Color.Red;
                lblMensaje.ForeColor = Color.White;
            }

            if (cont==5)
            {
                this.BackColor = Color.Red;
                lblMensaje.ForeColor = Color.White;
                tmr.Stop();

            }

        }
    }
}
