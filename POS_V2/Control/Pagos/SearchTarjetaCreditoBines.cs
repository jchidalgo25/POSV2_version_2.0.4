using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Control;
namespace POS.Control.Clientes
{
    public partial class SearchTarjetaCreditoBines : Form
    {
        public string code;
        System.Windows.Forms.Control focused;
        Boolean existeBin = false;

        public pos_customer SelectedCustomer { get; set; }
        public string NumeroTarjeta { get; set; }
        public bool ExisteBin() { return this.existeBin;    }
        string Cliente { get; set; }

        public SearchTarjetaCreditoBines()
        {
            Init();
        }

        public SearchTarjetaCreditoBines(string _clienteAccountNum)
        {
            Cliente = _clienteAccountNum;
            Init();
        }

        public SearchTarjetaCreditoBines(pos_customer previousSelectedCustomer)
        {
            SelectedCustomer = previousSelectedCustomer;
            Init();
        }

        private void Init()
        {
            InitializeComponent();            
            focused = (System.Windows.Forms.Control)txtNumTarjeta2;
            
        }

        private void BuscaBinesTarjeta()
        {
            string binTarjeta = "";
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            binTarjeta = txtNumTarjeta2.Text.Trim() + txtNumTarjeta.Text.Trim();
            if (string.IsNullOrEmpty(binTarjeta))
            {
                return;
            } 

            using (POSEntities db = new POSEntities())
            {
                try
                {
                    //Consulta el bin.
                    if (db.core_tarjetacredito_bin.Any(x => x.bin == binTarjeta))
                    {
                        existeBin = true;
                        NumeroTarjeta = binTarjeta;
                    }
                    /*else
                    {
                        core_tarjetacredito_bin tarcred_bin = new core_tarjetacredito_bin();
                        tarcred_bin.bin = binTarjeta;
                        tarcred_bin.bin_descripcion = "";
                        tarcred_bin.bin_red = "1";
                        tarcred_bin.bin_red_cred = "0";
                        db.core_tarjetacredito_bin.Add(tarcred_bin);                        
                        db.SaveChanges();

                        NumeroTarjeta = binTarjeta;
                        //MessageBox.Show(this, "Número de bin no encontrado.", "Busqueda de bin", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }*/
                }
                catch (Exception ex)
                {
                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = ex.Message.ToString() });
                    Control.Common.General.GetMensajeToList(516, parametros);

                    //MessageBox.Show(this, "Se produjo un error en la búsqueda, por favor vuelva a intentarlo. Error: " + ex.Message.ToString(), "Búsqueda de bin", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    //MessageBox.Show(this,"Detalle del error: " + ex.InnerException.ToString());
                }
            }
            //btnAceptar.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BuscaBinesTarjeta();

            if (existeBin)
            {
                this.Close();
            }
            else
            {
                //Control.Common.WinForm.ShowMessage("Número de bin no encontrado.");
                Control.Common.General.GetMensajeToList(517);
            }
            //SearchProductList();
        }
    
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnKb_Click(object sender, EventArgs e)
        {

            Control.Common.General.TecladoPantalla();


            //txtNumTarjeta.Text = "";
            //txtNumTarjeta2.Text = "";
            //var text = focused as Telerik.WinControls.UI.RadTextBox;
            //if (text != null)
            //{
            //    this.Top = this.Top - 80;
            //    KeyboardControl kbd = new KeyboardControl(text, this.Text);
            //    kbd.Top = this.Height+this.Top-50;
            //    kbd.Left = this.Left-200;
            //    kbd.ShowDialog();
            //    code = kbd.Tecla;
            //    this.Top = this.Top + 80;
            //    //this.Close();
            //}
        }
          
        private void txtNumTarjeta_Leave(object sender, EventArgs e)
        {
            AcceptButton = btnAceptar;
        }

        private void txtNumTarjeta_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnSearch;
        }

        private void txtNumTarjeta_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {              
                if (e.KeyChar == (char)Keys.Enter)
                {
                    BuscaBinesTarjeta();
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.SearchTarjetaCreditoBines", "txtNumPedidoDomicilio_KeyPress", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void SearchTarjetaCreditoBines_Load(object sender, EventArgs e)
        {
            txtNumTarjeta2.Focus();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            BuscaBinesTarjeta();

            if (existeBin)
            {
                this.Close();
            }
            else
            {
                //Control.Common.WinForm.ShowMessage("Número de bin no encontrado.");                
                Control.Common.General.GetMensajeToList(517);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
