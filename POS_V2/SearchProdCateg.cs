using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;
using System.Net;

namespace POS
{
    public partial class SearchProdCateg : Form
    {
        public string code;
        string establecimiento;
        public SearchProdCateg(string Establecimiento)
        {
            establecimiento = Establecimiento;
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var db = new POSEntities();

            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SearchProdCateg_Load(object sender, EventArgs e)
        {
            try
            {

            
            using (POSEntities db = new POSEntities())
            {
                var cant = db.VW_CATEGORIA.ToArray().Count();
                System.Windows.Forms.RadioButton[] radioButtons = new System.Windows.Forms.RadioButton[cant];               
                var arreglo = db.VW_CATEGORIA.ToArray();
                var lin = 0;
                var col = 1;
                for (int i = 0; i < cant; ++i)
                {
                    if (i % 7== 0 && i>1)
                    {
                        lin = 1;
                        col += 205;
                    }
                    else
                    {
                        lin += 1;
                    }
                    radioButtons[i] = new RadioButton();
                    radioButtons[i].Height = 55;
                    radioButtons[i].Width = 200;
                    radioButtons[i].Text = arreglo[i].NOMBRE;
                    radioButtons[i].Tag = arreglo[i].CODIGO;
                    radioButtons[i].Font= new Font(Font.FontFamily, 12);

                    radioButtons[i].Location = new System.Drawing.Point(10 + col , 10 + lin * 60);
                    radioButtons[i].Appearance = Appearance.Button;
                    this.Controls.Add(radioButtons[i]);
                    radioButtons[i].CheckedChanged += RadioButton_CheckedChanged;
                }
               
            }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SearchProdCateg", "SearchProdCateg_Load", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                MessageBox.Show(this, "Por favor intente nuevamente", "Búsqueda Poductos Categoría", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

    
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                var radiobutton = (RadioButton)sender;
                if (radiobutton.Checked)
                {
                    this.Opacity = 0;
                    if (this.ValidaConectividadPathImagenBusqProd())
                    {
                   

                        SearchProductV2 sp = new SearchProductV2(radiobutton.Tag.ToString(), establecimiento);
                    sp.Top = this.Top;
                    sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                    sp.ShowDialog();
                    code = sp.code;
                    this.Close();
                    }
                    else
                    {
                        SearchProduct sp = new SearchProduct(radiobutton.Tag.ToString(), establecimiento);
                        sp.Top = this.Top;
                        sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                        sp.ShowDialog();
                        code = sp.code;
                        this.Close();


                    }
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SearchProdCateg", "SearchProdCateg_Load", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                MessageBox.Show(this, "Por favor intente nuevamente", "Búsqueda Poductos Categoría", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool ValidaConectividadPathImagenBusqProd()
        {
            string url = MainWindow.URLPATHIMGBUSQPROD;

            return RemoteFileExists(url);
        }

        private bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }

    }
}
