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
    public partial class ConvertToAlphanumericUnitTest : Form
    {
        public ConvertToAlphanumericUnitTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(POS.Control.Common.StringHelper.ToAlphaNumeric(textBox1.Text));
        }
    }
}
