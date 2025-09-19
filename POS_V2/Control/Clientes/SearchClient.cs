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
    public partial class SearchClient : Form
    {
        public string code;
        System.Windows.Forms.Control focused;

        public pos_customer SelectedCustomer { get; set; }

        public SearchClient()
        {
            Init();
        }

        public SearchClient(pos_customer previousSelectedCustomer)
        {
            SelectedCustomer = previousSelectedCustomer;
            Init();
        }

        private void Init()
        {
            InitializeComponent();
            dgvSearchProduct.RowTemplate.Height = 28;
            focused = (System.Windows.Forms.Control)txtCliente;
            SearchProductList();
        }

        private void SearchProductList()
        {
            var cadena = txtCliente.Text.Trim();
            using (POSEntities db = new POSEntities())
            {
                if (cadena.Length > 0)
                {
                    try
                    {
                        var query = db.pos_customer.Where(x => x.ACCOUNTNUM.Contains(cadena) || x.NAME.Contains(cadena)).ToList();

                        dgvSearchProduct.AutoGenerateColumns = false;
                        dgvSearchProduct.DataSource = query;
                    }
                    catch (Exception ex)
                    {

                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.Message.ToString() });
                        Control.Common.General.GetMensajeToList(332, parametros);

                        //MessageBox.Show(this, "Se produjo un error en la busqueda, por favor vuelva a intentarlo. Error: " + ex.Message.ToString(), "Busqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        //MessageBox.Show(this,"Detalle del error: " + ex.InnerException.ToString());
                    }
                }
                else
                {
                    dgvSearchProduct.ClearSelection();
                }
            }
            btnChooseProduct.Focus();
        }

        private void txtProduct_TextChanged(object sender, EventArgs e)
        {
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {
                case Keys.Enter:
                    SearchProductList();
                    break;


                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;


            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchProductList();
        }

        private void btnChooseProduct_Click(object sender, EventArgs e)
        {
            SelectCustomer();
        }

        private void dgvSearchProduct_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectCustomer();
        }

        private void SelectCustomer()
        {
            if (dgvSearchProduct.Rows.Count > 0)
            {
                int row;
                row = dgvSearchProduct.CurrentCell.RowIndex;
                SelectedCustomer = (pos_customer)dgvSearchProduct.Rows[row].DataBoundItem;
                this.Close();
            }
            else
            {
                //MessageBox.Show("No ha seleccionado un cliente, si no lo encuentra puede crearlo primero pulsando en el botón NUEVO");
                Control.Common.General.GetMensajeToList(333);

            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnKb_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();
            
            //txtCliente.Text = "";
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

        private void SearchProduct_Load(object sender, EventArgs e)
        {
            txtCliente.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var f = new Control.Clientes.ClienteForm();
            f._cliente = null;
            f.DeseaPermitirCambioCedula = true;
            f.identificacion = string.Empty; 
            f.ShowDialog();

            if (f._cliente != null)
            {
                txtCliente.Text = f._cliente.ACCOUNTNUM;
                SearchProductList();
            }
        }

        private void txtCliente_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnSearch;
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            AcceptButton = btnChooseProduct;
        }
    }
}
