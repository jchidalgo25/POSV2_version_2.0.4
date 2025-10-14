using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using POS.Models;

namespace POS.Control.Main
{
    public partial class MensajeTarjetaVirtual : Telerik.WinControls.UI.RadForm
    {
        public MensajeTarjetaVirtual()
        {            
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            Aceptar();
        }

        private void Aceptar()
        {
            this.Close();
        }

        private void MensajeTarjetaVirtual_Load(object sender, EventArgs e)
        {
            btnAceptar.Focus();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    this.Close();
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


    }
}
