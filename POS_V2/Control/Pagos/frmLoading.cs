using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control.Pagos
{
    public partial class frmLoading : Form
    {

        private ProgressBar progressBar;
        private PictureBox pbLoading;


        public frmLoading()
        {
            InitializeComponent();

            // Configuración del Formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            
            //progressBar = new ProgressBar
            //{
            //    Location = new Point(50, 80),  // Posición del ProgressBar
            //    Size = new Size(300, 30),
            //    Minimum = 0,
            //    Maximum = 100,  // Máximo valor del progreso
            //    Value = 0,      // Valor inicial
            //    Style = ProgressBarStyle.Continuous  // Estilo con valor visible
            //};

     
            Label lblMessage = new Label
            {
                Text = "Cargando, por favor espere...",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14),
                AutoSize = true,
                Location = new Point(90, 40)
            };


            pbLoading = new PictureBox
            {
                Location = new Point(120, 70),
                Size = new Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Properties.Resources.loading // asegúrate que el gif está en recursos
            };

            //this.Controls.Add(progressBar);
            this.Controls.Add(lblMessage);  // Agregar el Label al formulario
            this.Controls.Add(pbLoading);

        }



    }
}
