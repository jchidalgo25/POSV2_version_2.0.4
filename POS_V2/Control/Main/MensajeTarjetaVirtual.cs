using System;
using System.Drawing;
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
            EstabilizarImagenes();
        }

        private void EstabilizarImagenes()
        {
            EstabilizarPictureBox(pictureBox1);
            EstabilizarPictureBox(pictureBox2);
        }

        private void EstabilizarPictureBox(PictureBox pb)
        {
            if (pb?.Image == null) return;
            try
            {
                var imagenOriginal = pb.Image;
                pb.Image = new Bitmap(imagenOriginal);
            }
            catch
            {
                pb.Image = null;
            }
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
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