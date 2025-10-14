using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.UnitTests
{
    public partial class ValidaCedulaTest : Form
    {
        public ValidaCedulaTest()
        {
            InitializeComponent();
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            try
            {
                var val = textBox1.Text.Trim();
                if (ValidarIdentificador.ValidarCedula(val))
                {
                    MessageBox.Show(this, "Valido como cedula!");
                }
                else if (ValidarIdentificador.ValidarRUCNatural(val))
                {
                    MessageBox.Show(this, "Valido como RUC natural!");
                }
                else if (ValidarIdentificador.ValidarRUCPrivada(val))
                {
                    MessageBox.Show(this, "Valido como RUC privada!");
                }
                else if (ValidarIdentificador.ValidarRUCPublica(val))
                {
                    MessageBox.Show(this, "Valido como RUC publica!");
                }
                else
                {
                    MessageBox.Show(this, "Cedula o RUC no Valido!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No es posible validar ese codigo con el algoritmo de validacion");
            }
        }
    }
}
