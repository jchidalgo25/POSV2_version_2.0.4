using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control.Auditor
{
    public partial class OpcionesAuditor : Form
    {
        public OpcionesAuditor()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Tag = "-1";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Tag = lstOpt.SelectedItem.Tag;
            this.Close();
        }

        private void OpcionesAuditor_Load(object sender, EventArgs e)
        {
            lstOpt.SelectedIndex = 0;
        }
    }
}
