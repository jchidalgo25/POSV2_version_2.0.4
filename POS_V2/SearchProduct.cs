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
namespace POS
{
    public partial class SearchProduct : Form
    {
        public string code;
        public string categoria;
        string establecimiento;
        System.Windows.Forms.Control focused;
        public int cantidadBuscar = 15;

        public SearchProduct(string Categoria, string Establecimiento)
        {

            InitializeComponent();
            categoria = Categoria;
            establecimiento = Establecimiento;
            dgvSearchProduct.RowTemplate.Height = 28;
            if (categoria != "")
                txtProduct.Text = " ";
            focused = (System.Windows.Forms.Control)txtProduct;         
            SearchProductList();
            CargarParametros();

            //dgvSearchProduct.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        public void CargarParametros()
        {
            using (POSEntities db = new POSEntities())
            {
                //cantidad de registros a buscar
                var param = db.core_parametro.Where(x => x.identificador.Equals("CANTIDAD_PRODUCTO_BUSCAR")).FirstOrDefault();

                if (param != null)
                    cantidadBuscar = int.Parse(param.valor);
            }
        }

        private void SearchProductList()
        {
            var db = new POSEntities();

            //if (txtProduct.Text.Length > 0)
            if (txtProduct.Text.Trim().Length >= 3)            
            {
                try
                {
                    if (categoria == "")
                    {
                        var query = (from tran in db.VW_SEARCHPRODUCT
                                     where tran.ARTICULO.Contains(txtProduct.Text) 
                                     && tran.ESTABLECIMIENTO == establecimiento
                                     select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                        //                   select tran).OrderBy(x => x.ARTICULO).Take(40).ToList();

                        dgvSearchProduct.DataSource = query;
                    }
                    else
                    {
                        var query = (from tran in db.VW_SEARCHPRODUCT
                                     where tran.ARTICULO.Contains(txtProduct.Text) && tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                                     && tran.ESTABLECIMIENTO == establecimiento
                                     select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                        //                   select tran).OrderBy(x => x.ARTICULO).Take(40).ToList();

                        dgvSearchProduct.DataSource = query;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,"Se produjo un error en la busqueda, por favor vuelva a intentarlo. Error: " + ex.Message.ToString(), "Busqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    //MessageBox.Show(this,"Detalle del error: " + ex.InnerException.ToString());
                }
            }
            else
            {
                dgvSearchProduct.ClearSelection();
            }
        }

        private void txtProduct_TextChanged(object sender, EventArgs e)
        {
            SearchProductList();
        }

        private void btnChooseProduct_Click(object sender, EventArgs e)
        {
            int row;

            row = dgvSearchProduct.CurrentCell.RowIndex;
            code = dgvSearchProduct.Rows[row].Cells[1].Value.ToString();            
            this.Close();
        }

        private void dgvSearchProduct_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row;

            row = dgvSearchProduct.CurrentCell.RowIndex;
            code = dgvSearchProduct.Rows[row].Cells[1].Value.ToString();
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnKb_Click(object sender, EventArgs e)
        {
            txtProduct.Text = "";
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                Control.Common.General.TecladoPantalla();

                //this.Top = this.Top - 80;
                //KeyboardControl kbd = new KeyboardControl(text, this.Text);
                //kbd.Top = this.Height+this.Top-50;
                //kbd.Left = this.Left-200;
                //kbd.ShowDialog();
                //code = kbd.Tecla;
                //this.Top = this.Top + 80;
                ////this.Close();
            }
            
           
        }

        private void SearchProduct_Load(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }
    }
}
