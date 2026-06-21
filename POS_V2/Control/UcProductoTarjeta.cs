using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control
{
    public partial class UcProductoTarjeta : UserControl
    {
        public string CodigoBarras { get; set; }
        public decimal PrecioVenta { get; set; }
        public UcProductoTarjeta()
        {
            InitializeComponent();
        }

        public void ConfigurarTarjeta(string nombre, decimal precio, string barras, Image imagen)
        {
            lblNombre.Text = nombre;
            lblPrecio.Text = string.Format("${0:F2}", precio);
            this.CodigoBarras = barras;
            this.PrecioVenta = precio;
            pbImagen.Image = imagen;
        }

        public event EventHandler OnAgregarClick;

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Cuando se haga clic en el botón interno de Guna, disparamos nuestro evento
            OnAgregarClick?.Invoke(this, e);
        }

        // Dentro de UcProductoTarjeta.cs
        public void ActualizarImagen(Image nuevaImagen)
        {
            if (nuevaImagen != null)
            {
                // Reemplaza 'pbImagen' por el nombre real de tu PictureBox en el diseño
                pbImagen.Image = nuevaImagen;
                pbImagen.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
    }
}
