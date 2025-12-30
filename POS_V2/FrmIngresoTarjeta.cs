using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class FrmIngresoTarjeta : Form
    {
        public string NumeroTarjetaDigitado
        { get; private set; }
        public string FechaExpiracion
        { get; private set; }
        public FrmIngresoTarjeta()
        {
            InitializeComponent();
            this.TopMost = true; // Obliga a estar encima de todo
            this.StartPosition = FormStartPosition.CenterScreen; // Centrado en la pantalla
            this.Activate(); // Pide el foco del teclado inmediatamente

            this.Text = "Ingreso Manual de Tarjeta";
            this.ControlBox = false; // Quitamos la X para obligar a usar botones
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

        }
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // 1. Validaciones básicas
            string tarjeta = txtNumeroTarjeta.Text.Replace(" ", "").Trim();

            if (string.IsNullOrEmpty(tarjeta) || tarjeta.Length < 15)
            {
                MessageBox.Show("Por favor ingrese un número de tarjeta válido (mínimo 15 dígitos).",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Guardamos el valor en la propiedad pública
            this.NumeroTarjetaDigitado = tarjeta;

            // 3. Cerramos indicando éxito
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Evento para permitir solo números en el TextBox
        // Tienes que ir al diseñador, seleccionar el txtNumeroTarjeta, ir a Eventos (el rayito) 
        // y hacer doble clic en "KeyPress" para enlazar esto, o copiarlo en el constructor.
        private void txtNumeroTarjeta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
