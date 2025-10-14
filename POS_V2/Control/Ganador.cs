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
    public partial class Ganador : Form
    {
        private string _msjCustomizado = "SU COMPRA ACABA DE SALIR GRATIS, GRACIAS A SUPERMERCADOS DELPORTAL";

        public Ganador()
        {
            InitializeComponent();
        }

        public Ganador(string msjCustom)
        {
            InitializeComponent();
            _msjCustomizado = msjCustom;
        }

        private void Ganador_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = _msjCustomizado;
            panel1.Left = panel1.Width / 2;
            panel1.Top = panel1.Height / 2;
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
            if (this.BackColor == Color.Green)
            {
                this.BackColor = Color.White;
                lblMensaje.ForeColor = Color.Red;
            }
            else
            {
                this.BackColor = Color.Green;
                lblMensaje.ForeColor = Color.White;
            }

            if (cont==5)
            {
                this.BackColor = Color.Green;
                lblMensaje.ForeColor = Color.White;
                tmr.Stop();

            }

        }
    }
}
