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
    public partial class XmlToTextTest : Form
    {
        public XmlToTextTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtTransformado.Text = Control.Common.XmlHelper.XmlToText(txtOriginal.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtTransformado.Text.Length > 0)
            {
                Control.Common.Printer.Imprimir(txtTransformado.Text, 3, 11);
            }
        }
    }
}
