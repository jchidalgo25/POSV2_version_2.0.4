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
    public partial class MailUnitTest : Form
    {
        public MailUnitTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(txtUsrEnvia.Text, "TEST FINAL",
                    txtUsrDestino.Text,
                    txtUsrCopia.Text,
                    txtMotivo.Text,
                    txtMensaje.Text,
                    false,
                    txtAttach.Text);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                    throw new Exception(xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                else
                    MessageBox.Show(xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Puchica: " + ex.Message);
            }
        }
    }
}
