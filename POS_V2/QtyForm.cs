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
    public partial class QtyForm : Form
    {
        public QtyForm()
        {
            InitializeComponent();            
        }

        public int q;

        private void btn1_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn1.Text;
            btnEnter.Focus();
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn2.Text;
            btnEnter.Focus();
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn3.Text;
            btnEnter.Focus();
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn4.Text;
            btnEnter.Focus();
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn5.Text;
            btnEnter.Focus();
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn6.Text;
            btnEnter.Focus();
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn7.Text;
            btnEnter.Focus();
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn8.Text;
            btnEnter.Focus();
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn9.Text;
            btnEnter.Focus();
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            txtQty.Text = txtQty.Text + btn0.Text;
            btnEnter.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            txtQty.Clear();
            txtQty.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtQty.Clear();
            this.Close();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQty.Text))
            {
                q = int.Parse(txtQty.Text);
                this.Close();
            }
            
            
        }
        

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
                return;
            }

            //if (txtQty.Text.Length <= 0)
            //    txtQty.Text = "0";
            //q = int.Parse(txtQty.Text);
            //this.Close();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
       {
            if (keyData == Keys.Enter)
            {
                q = int.Parse(txtQty.Text);
                this.Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void QtyForm_Load(object sender, EventArgs e)
        {

        }
    }
}
