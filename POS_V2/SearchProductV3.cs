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
using System.Net;
using System.Resources;
using System.Reflection;
using System.IO;


namespace POS
{
    public partial class SearchProductV3 : Form
    {
        public string code;
        public string categoria;
        string establecimiento;
        System.Windows.Forms.Control focused;
        public int cantidadBuscar = 50;
        Panel panelCategoria;

        //List<LeerProductosArchivo> productoslista;
        //List<LeerProductosArchivo> productoslista = new List<LeerProductosArchivo>();
        public bool valTipoBusqueda = false;

        public SearchProductV3(string Categoria, string Establecimiento)
        {

            InitializeComponent();
            categoria = Categoria;
            establecimiento = Establecimiento;
            dgvSearchProduct.RowTemplate.Height = 28;
            //if (categoria != "")
            //    txtProduct.Text = " ";
            focused = (System.Windows.Forms.Control)txtProduct;
            CargarParametros();

            panelCategoria = new Panel();
            panelCategoria.Name = "PanelCategoria";
            SearchProdCateg_Load();
            tblLay_Principal.Dock = DockStyle.Fill;

            //GenerateTable(3, 7, "AB");
            //SearchProductList();//eevv comentado , primero debe mostrar las categorías.
            //LoadCategoria;
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
            valTipoBusqueda = false;
            if (CreateFileProducts.productoslista.Count > 0)
            {
                valTipoBusqueda = true;//tiene archivo local
            }

            if (txtProduct.Text.Length >= 3)
            {
                try
                {
                    if (categoria == "")
                    {
                        if (valTipoBusqueda)
                        {
                            //CreateFileProducts.productoslista
                            var query = (from tran in CreateFileProducts.productoslista // .VW_SEARCHRODUCT
                                         where tran.ARTICULO.ToUpper().Contains(txtProduct.Text.ToUpper())
                                         select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                            //productoslista.FindAll(c => c.ARTICULO.Contains(txtProduct.Text)).ToList();
                            dgvSearchProduct.DataSource = query;
                        }
                        else
                        {
                            var db = new POSEntities();
                            var query = (from tran in db.VW_SEARCHPRODUCT // .VW_SEARCHRODUCT
                                         where tran.ARTICULO.Contains(txtProduct.Text)
                                         && tran.ESTABLECIMIENTO == establecimiento
                                         select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                            dgvSearchProduct.DataSource = query;
                        }
                        //                   select tran).OrderBy(x => x.ARTICULO).Take(40).ToList();                      
                    }
                    else
                    {
                        if (valTipoBusqueda)
                        {
                            var query = (from tran in CreateFileProducts.productoslista // .VW_SEARCHRODUCT
                                         where tran.ARTICULO.ToUpper().Contains(txtProduct.Text.ToUpper()) && tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                                         select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                            dgvSearchProduct.DataSource = query;
                        }
                        else
                        {
                            var db = new POSEntities();
                            var query = (from tran in db.VW_SEARCHPRODUCT
                                         where tran.ARTICULO.Contains(txtProduct.Text) && tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                                         && tran.ESTABLECIMIENTO == establecimiento
                                         select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                            // select tran).OrderBy(x => x.ARTICULO).Take(40).ToList();
                            dgvSearchProduct.DataSource = query;
                        }

                    }
                    LoadImagenGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Se produjo un error en la busqueda, por favor vuelva a intentarlo. Error: " + ex.Message.ToString(), "Busqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    //MessageBox.Show(this,"Detalle del error: " + ex.InnerException.ToString());
                }
            }
            else
            {
                if (categoria != "")
                {
                    if (valTipoBusqueda)
                    {
                        var query = (from tran in CreateFileProducts.productoslista // .VW_SEARCHRODUCT
                                     where tran.ARTICULO.ToUpper().Contains(txtProduct.Text.ToUpper()) && tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                                     select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                        dgvSearchProduct.DataSource = query;
                    }
                    else
                    {
                        var db = new POSEntities();
                        var query = (from tran in db.VW_SEARCHPRODUCT
                                     where /*tran.ARTICULO.Contains(txtProduct.Text) && */tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                                     && tran.ESTABLECIMIENTO == establecimiento
                                     select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();
                        dgvSearchProduct.DataSource = query;
                    }
                    //select tran).OrderBy(x => x.ARTICULO).Take(40).ToList();                  
                    LoadImagenGridView();
                }
                else
                    dgvSearchProduct.ClearSelection();

            }
            //if (panelCuerpoResultado.Controls.Find("dgvSearchProduct", true))
            panelCuerpoResultado.Controls.Add(dgvSearchProduct);
        }

        private void SearchProductList_TableLayout()
        {
            valTipoBusqueda = false;

            //if (CreateFileProducts.productoslista.Count >0)
            //{
            //    valTipoBusqueda = true;//tiene archivo local
            //}

            if (CreateFileProducts.productoslistaXML.Count > 0)
            {
                valTipoBusqueda = true;//tiene archivo local
            }


            if (txtProduct.Text.Length >= 3)
            {
                try
                {
                    if (categoria == "")
                    {
                        if (valTipoBusqueda)
                        {
                            var query = (from tran in CreateFileProducts.productoslistaXML
                                         where tran.campoConsulta.ToUpper().Contains(txtProduct.Text.ToUpper())
                                         select tran)
                                         .OrderByDescending(x => x.ORDEN)
                                         .ThenBy(x => x.ARTICULO)
                                         .Take(cantidadBuscar).ToList();

                            //genera el tableLayout de los 20 items de acuerdo al resultado
                            GenerateTableListP(4, 5, string.Empty, query);
                        }
                        else
                        {
                            var query = (from deta in Control.Common.General.GetListConsultaProducto(establecimiento, "", txtProduct.Text.ToUpper()).ProductoArticuloList
                                         select deta).Take(cantidadBuscar).ToList();

                            GeneraTableProducto(4, 5, string.Empty, query);
                            dgvSearchProduct.DataSource = query;

                            //var db = new POSEntities();
                            //var query = (from tran in db.VW_SEARCHPRODUCT 
                            //             where tran.ARTICULO.Contains(txtProduct.Text)
                            //             && tran.ESTABLECIMIENTO == establecimiento
                            //             select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();

                            //genera el tableLayout de los 20 items de acuerdo al resultado
                            //GenerateTable1(4, 5, string.Empty, query);
                            //dgvSearchProduct.DataSource = query;  
                        }
                    }
                    else
                    {
                        if (valTipoBusqueda)
                        {
                            var query = (from tran in CreateFileProducts.productoslistaXML // .VW_SEARCHRODUCT
                                         where tran.campoConsulta.ToUpper().Contains(txtProduct.Text.ToUpper())
                                         && tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                                         select tran)
                                         .OrderByDescending(x => x.ORDEN)
                                         .ThenBy(x => x.ARTICULO)
                                         .Take(cantidadBuscar).ToList();

                            GenerateTableListP(4, 5, string.Empty, query.ToList());
                        }
                        else
                        {


                            var query = (from deta in Control.Common.General.GetListConsultaProducto(establecimiento, categoria.ToUpper(), txtProduct.Text.ToUpper()).ProductoArticuloList.ToList()
                                         select deta).Take(cantidadBuscar).ToList();

                            GeneraTableProducto(4, 5, string.Empty, query);

                            //var db = new posentities();
                            //var query = (from tran in db.vw_searchproduct
                            //             where tran.articulo.contains(txtproduct.text)
                            //             && tran.categoria.toupper().equals(categoria.toupper())
                            //             && tran.establecimiento == establecimiento
                            //             select tran).orderbydescending(x => x.orden).thenby(x => x.articulo).take(cantidadbuscar).tolist();
                            //GenerateTable1(4, 5, string.Empty, query.ToList());
                        }
                        //                   select tran).OrderBy(x => x.ARTICULO).Take(40).ToList();
                        //dgvSearchProduct.DataSource = query;      
                    }
                    //  LoadImagenGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Se produjo un error en la busqueda, por favor vuelva a intentarlo. Error: " + ex.Message.ToString(), "Busqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    //MessageBox.Show(this,"Detalle del error: " + ex.InnerException.ToString());
                }
            }
            else
            {
                if (categoria != "")
                {
                    if (valTipoBusqueda)
                    {
                        var query = (from tran in CreateFileProducts.productoslistaXML
                                     where tran.ARTICULO.ToUpper().Contains(txtProduct.Text.ToUpper()) && tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                                     select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();

                        GenerateTableListP(4, 5, string.Empty, query.ToList());
                    }
                    else
                    {
                        var query = (from deta in Control.Common.General.GetListConsultaProducto(establecimiento, categoria.ToUpper(), txtProduct.Text.ToUpper()).ProductoArticuloList.ToList()
                                     select deta).Take(cantidadBuscar).ToList();

                        GeneraTableProducto(4, 5, string.Empty, query);


                        //var db = new POSEntities();
                        //    var query = (from tran in db.VW_SEARCHPRODUCT
                        //                 where tran.ARTICULO.Contains(txtProduct.Text) && tran.CATEGORIA.ToUpper().Equals(categoria.ToUpper())
                        //                 && tran.ESTABLECIMIENTO == establecimiento
                        //                 select tran).OrderByDescending(x => x.ORDEN).ThenBy(x => x.ARTICULO).Take(cantidadBuscar).ToList();


                        //    GenerateTable1(4, 5, string.Empty, query.ToList());
                    }


                }
                else
                    dgvSearchProduct.ClearSelection();

            }
        }

        //cat
        public Image LoadImageTableLayout(string itemid, bool categoria = false)
        {
            string url = "http://reportes.liris.com.ec/ImgAx/100/@barcode.jpg";
            PictureBox picbox = new PictureBox();
            //Validación para las categorias de los productos
            if (categoria)
            {
                url = url.Replace("100", "CAT");
                // url = url.Replace("jpg", "svg");                
                url = url.Replace("jpg", "png");
            }

            try
            {
                var urlnew = url.Replace("@barcode", itemid);
                picbox.Load(urlnew);

                // dgvSearchProduct.Rows[dr.Index].Cells["imgArticulo"].Value = picbox.Image;
                // dgvSearchProduct.Rows[dr.Index].Height = 75;
            }
            catch (Exception ex)
            {
                try
                {
                    var urldefault = url.Replace("@barcode.jpg", "default.jpg");
                    picbox.Load(urldefault);
                    //    dgvSearchProduct.Rows[dr.Index].Cells["imgArticulo"].Value = picbox.Image;
                    //  dgvSearchProduct.Rows[dr.Index].Height = 75;
                }
                catch (Exception)
                {
                    // picbox.Image= new Image(). 
                    if (categoria)
                    {
                        // picbox.Image = global::POS.Properties.Resources.categories;
                        try
                        {
                            ResourceManager rm = new ResourceManager("myResources", Assembly.GetExecutingAssembly());
                            Bitmap b = (Bitmap)rm.GetObject(itemid);
                            picbox.Image = (Image)b;
                        }
                        catch (Exception ex1)
                        {
                            picbox.Image = global::POS.Properties.Resources.categories;
                        }
                        /*   Bitmap b = (Bitmap)rm.GetObject("anImage");
                           p.Image = (Image)b;
                           p.Height = b.Height;*/

                        /* Bitmap b1  = new Bitmap(
                                                   System.Reflection.Assembly.GetEntryAssembly().
                                                    GetManifestResourceStream(strResourceImg));
                                                    */
                        // GetManifestResourceStream("POS.Resources.AB.png"));

                        //Bitmap b1 = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("POS.Resources.AB.png"));
                    }
                    else
                    {
                        picbox.Image = global::POS.Properties.Resources._default;
                    }

                }
            }

            return picbox.Image;
        }
        public void LoadImagenGridView()
        {
            string url = "http://reportes.liris.com.ec/ImgAx/100/@barcode.jpg";
            PictureBox picbox = new PictureBox();
            //if (dgvSearchProduct.Rows.Count <= cantidadBuscar)
            //{
            foreach (DataGridViewRow dr in dgvSearchProduct.Rows)
            {
                try
                {
                    var urlnew = url.Replace("@barcode", dr.Cells[2].Value.ToString());
                    picbox.Load(urlnew);
                    dgvSearchProduct.Rows[dr.Index].Cells["imgArticulo"].Value = picbox.Image;
                    dgvSearchProduct.Rows[dr.Index].Height = 75;
                }
                catch (Exception)
                {
                    try
                    {
                        var urldefault = url.Replace("@barcode.jpg", "default.jpg");
                        picbox.Load(urldefault);
                        dgvSearchProduct.Rows[dr.Index].Cells["imgArticulo"].Value = picbox.Image;
                        dgvSearchProduct.Rows[dr.Index].Height = 75;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            //}
        }

        private void txtProduct_TextChanged(object sender, EventArgs e)
        {
            if (txtProduct.Text.Count() == 0)
            {
                //panelCategoriaTop
                panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
                panelCuerpoResultado.Visible = true;
                dgvSearchProduct.Visible = false;
                panel1.Visible = false;
                //cargo los 20 productos mas vendidos.
                //cargaTop20ProductosMasVendidos();
                SearchProductList_TableLayout();
            }
            else
            {
                panelCuerpoResultado.Visible = true;
                tblLay_Top20Productos.Visible = false;
                panelCategoria.Visible = false;
                //SearchProductList();
                //dgvSearchProduct.Dock = DockStyle.Fill;
                //dgvSearchProduct.Visible = true;
                //lleno la tabla con los resultados del query.
                SearchProductList_TableLayout();
            }
        }

        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (txtProduct.Text.Length != txtProduct.SelectionStart)
            //{
            //    panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
            //    panelCuerpoResultado.Visible = true;
            //    dgvSearchProduct.Visible = false;
            //    panel1.Visible = false;
            //    //cargo los 20 productos mas vendidos.
            //    cargaTop20ProductosMasVendidos();
            //}

        }

        private void cargaTop20ProductosMasVendidos()
        {
            try
            {

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SearchProdCateg", "SearchProdCateg_Load", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //MessageBox.Show(this, "Por favor intente nuevamente", "Búsqueda Poductos Categoría", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void SearchProdCateg_Load()
        {
            float dimensionCuadro = 33.33F;
            int columnCount = 4;
            int rowCount = 4;

            try
            {
                rbtnCategoria.Visible = false;
                if (panelCategoria.Controls.Count > 0)
                {
                    //panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
                    panelCategoria.Visible = true;
                    tblLay_Top20Productos.Visible = false;
                    //panelCuerpoResultado.Visible = true;
                    //dgvSearchProduct.Visible = false;
                }
                //coloco el panel en la misma posición que estya la grilla.
                //y muestro el panel.
                panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
                panelCuerpoResultado.Visible = true;
                dgvSearchProduct.Visible = false;
                this.panelCuerpoResultado.Width = this.panelCuerpoResultado.Width + 50;

                if (panelCategoria.Controls.Count == 0)
                {
                    //Clear out the existing controls, we are generating a new table layout
                    tblLay_CategoriaProducto.Controls.Clear();


                    //Clear out the existing row and column styles
                    tblLay_CategoriaProducto.ColumnStyles.Clear();
                    tblLay_CategoriaProducto.RowStyles.Clear();

                    //Now we will generate the table, setting up the row and column counts first
                    tblLay_CategoriaProducto.ColumnCount = columnCount;
                    //   tblLay_CategoriaProducto.RowCount = rowCount;


                    using (POSEntities db = new POSEntities())
                    {

                        var cant = db.VW_CATEGORIA.ToArray().Count();
                        System.Windows.Forms.RadioButton[] radioButtons = new System.Windows.Forms.RadioButton[cant];
                        var arreglo = db.VW_CATEGORIA.ToArray();

                        var lin = 0;
                        var col = 1;
                        int i = 0;
                        int i2 = 0;
                        int col1 = 1, row1 = 1;
                        decimal x1 = 0;
                        x1 = (Convert.ToDecimal(cant) / 4);
                        int filas = Convert.ToInt16(decimal.Ceiling(x1));
                        rowCount = filas;
                        tblLay_CategoriaProducto.RowCount = rowCount;
                        if (cant < columnCount)
                        {
                            /* columnCount = cant;
                             tblLay_Top20Productos.ColumnCount = columnCount;
                             dimensionCuadro = 1 / columnCount;
                             */
                            rowCount = rowCount + 1;
                            tblLay_CategoriaProducto.RowCount = rowCount;
                        }
                        //crea la tabla sin controles.
                        for (int r2 = 0; r2 < rowCount; r2++)
                        {
                            tblLay_CategoriaProducto.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));

                            for (int c2 = 0; c2 < columnCount; c2++)
                            {
                                tblLay_CategoriaProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));

                                if (cant > i)
                                {
                                    //Create the control, in this case we will add a button
                                    Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

                                    cmd.Text = arreglo[i].NOMBRE;
                                    cmd.Tag = arreglo[i].CODIGO;
                                    cmd.TextAlignment = ContentAlignment.BottomCenter;
                                    cmd.TextWrap = true;
                                    cmd.Font = new Font(Font.FontFamily, 12);
                                    cmd.Image = LoadImageTableLayout(arreglo[i].CODIGO, true);
                                    //cmd.Margin
                                    cmd.Height = 150;
                                    cmd.Dock = DockStyle.Fill;
                                    //cmd.Width = 80;
                                    //cmd.Image = new Image();
                                    cmd.ImageAlignment = ContentAlignment.MiddleCenter;

                                    //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
                                    tblLay_CategoriaProducto.Controls.Add(cmd, c2, r2);

                                    cmd.Click += Categoria_Click;


                                }
                                i++;

                            }
                        }

                        /*
                        foreach (var cat in db.VW_CATEGORIA.ToList())
                        {
                            //i2++;
                            tblLay_CategoriaProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                            if(i2 ==0)
                            {
                                tblLay_CategoriaProducto.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
                              //  row1++;
                            }

                            if (i2 % 4 ==0 )
                            {
                                tblLay_CategoriaProducto.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
                                row1++;

                                if(i2>0)
                                col1 = 1;
                            }
                            Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();                            
                            cmd.Text = arreglo[i].NOMBRE;
                            cmd.Tag = arreglo[i].CODIGO;
                            cmd.TextAlignment = ContentAlignment.BottomCenter;
                            cmd.TextWrap = true;
                            cmd.Font = new Font(Font.FontFamily, 12);
                            cmd.Image = LoadImageTableLayout(arreglo[i].CODIGO, true);
                            //cmd.Margin
                            cmd.Height = 150;
                            cmd.Dock = DockStyle.Fill;
                            cmd.ImageAlignment = ContentAlignment.MiddleCenter;

                            //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
                            tblLay_CategoriaProducto.Controls.Add(cmd, col1, row1);

                            cmd.Click += Categoria_Click;
                            i2++;
                            col1++;
                        }*/

                        /*
                        for (int x = 0; x < columnCount; x++)
                        {
                            //First add a column
                            tblLay_CategoriaProducto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

                            for (int y = 0; y < rowCount; y++)
                            {
                                //Next, add a row.  Only do this when once, when creating the first column
                                if (x == 0)
                                {
                                    //tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                    tblLay_CategoriaProducto.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
                                }
                                if (cant > i)
                                {

                                    //Create the control, in this case we will add a button
                                    Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

                                    cmd.Text = arreglo[i].NOMBRE;
                                    cmd.Tag = arreglo[i].CODIGO;
                                    cmd.TextAlignment = ContentAlignment.BottomCenter;
                                    cmd.TextWrap = true;
                                    cmd.Font = new Font(Font.FontFamily, 12);
                                    cmd.Image = LoadImageTableLayout(arreglo[i].CODIGO, true);
                                    //cmd.Margin
                                    cmd.Height = 150;
                                    cmd.Dock = DockStyle.Fill;
                                    //cmd.Width = 80;
                                    //cmd.Image = new Image();
                                    cmd.ImageAlignment = ContentAlignment.MiddleCenter;

                                    //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
                                    tblLay_CategoriaProducto.Controls.Add(cmd, x, y);

                                    cmd.Click += Categoria_Click;
                                }
                                i++;
                            }
                           
                        }*/

                        this.panelCategoria.Dock = DockStyle.Fill;
                        panelCategoria.Controls.Add(tblLay_CategoriaProducto);
                        tblLay_CategoriaProducto.AutoScroll = true;
                        tblLay_CategoriaProducto.AutoScrollPosition = new Point(0, 0);

                        this.panelCuerpoResultado.Controls.Add(panelCategoria);
                        tblLay_Top20Productos.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SearchProdCateg", "SearchProdCateg_Load", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                MessageBox.Show(this, "Por favor intente nuevamente", "Búsqueda Poductos Categoría", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void Categoria_Click(object sender, EventArgs e)
        {
            try
            {
                var boton1 = (Telerik.WinControls.UI.RadButton)sender;
                //panelCategoria.Visible = false;
                rbtnCategoria.Visible = true;
                categoria = boton1.Tag.ToString();

                lblEtiquetaCategoriaSeleccionada.Text = boton1.Text;
                // radBreadCrumb1.Image = boton1.Image;
                GenerateTable(4, 7, boton1.Tag.ToString());
            }
            catch (Exception)
            {

                throw;
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

                        //LlenaTbleLayoutTop20(radiobutton.Tag.ToString());

                        GenerateTable(4, 5, radiobutton.Tag.ToString());
                        this.TopMost = true;
                        //SearchProductV2 sp = new SearchProductV2(radiobutton.Tag.ToString(), establecimiento);
                        //sp.Top = this.Top;
                        //sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                        //sp.ShowDialog();
                        //code = sp.code;
                        //this.Close();
                    }
                    else
                    {
                        /*SearchProduct sp = new SearchProduct(radiobutton.Tag.ToString(), establecimiento);
                        sp.Top = this.Top;
                        sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                        sp.ShowDialog();
                        code = sp.code;
                        this.Close();*/


                    }
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SearchProdCateg", "SearchProdCateg_Load", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                MessageBox.Show(this, "Por favor intente nuevamente", "Búsqueda Poductos Categoría", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void Top20MasVendidos_Click(object sender, EventArgs e)
        {
            try
            {
                var boton = (Telerik.WinControls.UI.RadButton)sender;
                code = boton.Tag.ToString();
                this.Close();
                /*
                       this.TopMost = true;
                       pictureBox1.Image = LoadImageTableLayout(boton.Tag.ToString());
                       lblNombreProductoSelec.Text = boton.Text;
               panel1.Visible = true;
               */
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SearchProdCateg", "SearchProdCateg_Load", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //MessageBox.Show(this, "Por favor intente nuevamente", "Búsqueda Poductos Categoría", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        //private void GenerateTable(int columnCount, int rowCount, string categoria)
        //{
        //    float dimensionCuadro = 33.33F;
        //    dimensionCuadro = (float)(Convert.ToDouble(1) / columnCount) * 100;

        //    panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
        //    panelCuerpoResultado.Visible = true;
        //    panelCuerpoResultado.Size = new Size(601, 432);//535, 432
        //    dgvSearchProduct.Visible = false;

        //    panel1.Visible = false;

        //    //Clear out the existing controls, we are generating a new table layout
        //    tblLay_Top20Productos.Controls.Clear();


        //    //Clear out the existing row and column styles
        //    tblLay_Top20Productos.ColumnStyles.Clear();
        //    tblLay_Top20Productos.RowStyles.Clear();

        //    //Now we will generate the table, setting up the row and column counts first
        //    tblLay_Top20Productos.ColumnCount = columnCount;
        //    tblLay_Top20Productos.RowCount = rowCount;
        //    using (POSEntities db = new POSEntities())
        //    {
        //        var cant = db.TblItemsMaxSales.Where(x => x.Categoria == categoria && x.Establecimiento == establecimiento).ToArray().Count();
        //        var arreglo = db.TblItemsMaxSales.Where(x => x.Categoria == categoria && x.Establecimiento == establecimiento).OrderBy(y => y.Orden).ToArray();
        //        int i = 0;
        //        if (cant > 0)
        //        {
        //            decimal x1 = 0;
        //            x1 = (Convert.ToDecimal(cant) / 4);
        //            int filas = Convert.ToInt16(decimal.Ceiling(x1));
        //            if (cant % columnCount == 0)
        //            {
        //                rowCount = filas;
        //            }
        //            else
        //            {
        //                rowCount = filas + 1;
        //            }

        //            //tblLay_CategoriaProducto.RowCount = rowCount;
        //            tblLay_Top20Productos.RowCount = rowCount;
        //            //Si el numero de registros de la consulta es menor a 4, entonces generar el numero de columnas de la cantidad de registro
        //            if (cant < columnCount)
        //            {
        //                columnCount = cant;
        //                tblLay_Top20Productos.ColumnCount = columnCount;
        //                dimensionCuadro = 1 / columnCount;
        //            }


        //            if (cant < columnCount)
        //            {
        //                /* columnCount = cant;
        //                 tblLay_Top20Productos.ColumnCount = columnCount;
        //                 dimensionCuadro = 1 / columnCount;
        //                 */
        //                rowCount = rowCount + 1;
        //                tblLay_Top20Productos.RowCount = rowCount;
        //            }
        //            //crea la tabla sin controles.
        //            for (int r2 = 0; r2 < rowCount; r2++)
        //            {
        //                tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));

        //                for (int c2 = 0; c2 < columnCount; c2++)
        //                {
        //                    tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));

        //                    if (cant > i)
        //                    {
        //                        //Create the control, in this case we will add a button
        //                        Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

        //                        cmd.Text = arreglo[i].Articulo + Environment.NewLine + "Cod. Barra: " + arreglo[i].ItemId;
        //                        cmd.Tag = arreglo[i].ItemId;
        //                        cmd.TextAlignment = ContentAlignment.BottomCenter;
        //                        cmd.TextWrap = true;
        //                        cmd.Font = new Font(Font.FontFamily, 12);
        //                        cmd.Image = LoadImageTableLayout(arreglo[i].ItemId);
        //                        cmd.Height = 150;
        //                        cmd.Dock = DockStyle.Fill;
        //                        cmd.ImageAlignment = ContentAlignment.MiddleCenter;

        //                        //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
        //                        tblLay_Top20Productos.Controls.Add(cmd, c2, r2);

        //                        cmd.Click += Top20MasVendidos_Click;


        //                    }
        //                    i++;

        //                }
        //            }
        //        }


        //        /*
        //        for (int x = 0; x < columnCount; x++)
        //        {

        //            //First add a column
        //            //tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,33.33F));
        //            tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));

        //            for (int y = 0; y < rowCount; y++)
        //            {
        //                //Next, add a row.  Only do this when once, when creating the first column
        //                if (x == 0)
        //                {
        //                    //tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //                    tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.Absolute,200F));
        //                }
        //                if (cant > i)
        //                {

        //                    //Create the control, in this case we will add a button
        //                    Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

        //                    cmd.Text = arreglo[i].Articulo+ Environment.NewLine + "Cod. Barra: "+ arreglo[i].ItemId;
        //                    cmd.Tag = arreglo[i].ItemId;
        //                    cmd.TextAlignment = ContentAlignment.BottomCenter;
        //                    cmd.TextWrap = true;
        //                    cmd.Font = new Font(Font.FontFamily, 12);
        //                    cmd.Image = LoadImageTableLayout(arreglo[i].ItemId);
        //                    cmd.Height = 150;
        //                    cmd.Dock = DockStyle.Fill;
        //                    //cmd.Width = 80;
        //                    //cmd.Image = new Image();
        //                    cmd.ImageAlignment = ContentAlignment.MiddleCenter;

        //                    //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
        //                    tblLay_Top20Productos.Controls.Add(cmd, x, y);

        //                    cmd.Click += Top20MasVendidos_Click;


        //                }
        //                i++;
        //            }
        //        }*/
        //    }
        //    tblLay_Top20Productos.Visible = true;
        //    tblLay_Top20Productos.MaximumSize = new Size(tblLay_Top20Productos.Width, tblLay_Top20Productos.Height);
        //    tblLay_Top20Productos.AutoScroll = true;
        //    tblLay_Top20Productos.AutoScrollPosition = new Point(0, 0);


        //    this.panelCuerpoResultado.Width = this.panelCuerpoResultado.Width + 50;
        //    this.panelCategoria.Visible = false;
        //    this.panelCategoria.Dock = DockStyle.None;
        //    this.panelCuerpoResultado.Visible = true;
        //    this.TopMost = true;
        //}

        //jchid nuevo ajuste al diseño del grid de los productos 
        private void GenerateTable(int columnCount, int rowCount, string categoria)
        {
            float dimensionCuadro = 33.33F;
            dimensionCuadro = (float)(Convert.ToDouble(1) / columnCount) * 100;

            panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
            panelCuerpoResultado.Visible = true;
            panelCuerpoResultado.Size = new Size(601, 432);//535, 432
            dgvSearchProduct.Visible = false;
            panel1.Visible = false;

            //Clear out the existing controls, we are generating a new table layout
            tblLay_Top20Productos.Controls.Clear();

            //Clear out the existing row and column styles
            tblLay_Top20Productos.ColumnStyles.Clear();
            tblLay_Top20Productos.RowStyles.Clear();

            //Now we will generate the table, setting up the row and column counts first
            tblLay_Top20Productos.ColumnCount = columnCount;
            tblLay_Top20Productos.RowCount = rowCount;

            // QUITAR ESTAS LÍNEAS QUE CAUSAN PROBLEMAS:
            // tblLay_Top20Productos.AutoSize = true;
            // tblLay_Top20Productos.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            using (POSEntities db = new POSEntities())
            {
                // OPCIÓN 1: Usando SQL crudo JCHID
                string sqlQuery = @"
                                    SELECT m.* 
                                    FROM dbo.TblItemsMaxSales as m
                                    INNER JOIN DynamicsAx1.dbo.INVENTITEMBARCODE as ib 
                                        ON ib.ITEMBARCODE = m.ItemId COLLATE SQL_Latin1_General_CP1_CI_AS
                                    WHERE m.Establecimiento = @establecimiento 
                                        AND m.Categoria = @categoria
                                        AND ib.QTY <= 1
                                    ORDER BY m.Orden";

                var arreglo = db.Database.SqlQuery<TblItemsMaxSales>(sqlQuery,
                    new System.Data.SqlClient.SqlParameter("@establecimiento", establecimiento),
                    new System.Data.SqlClient.SqlParameter("@categoria", categoria)).ToArray();

                var cant = arreglo.Length;

                int i = 0;
                if (cant > 0)
                {
                    decimal x1 = 0;
                    x1 = (Convert.ToDecimal(cant) / 4);
                    int filas = Convert.ToInt16(decimal.Ceiling(x1));
                    if (cant % columnCount == 0)
                    {
                        rowCount = filas;
                    }
                    else
                    {
                        rowCount = filas + 1;
                    }

                    tblLay_Top20Productos.RowCount = rowCount;
                    if (cant < columnCount)
                    {
                        columnCount = cant;
                        tblLay_Top20Productos.ColumnCount = columnCount;
                        dimensionCuadro = (float)(Convert.ToDouble(1) / columnCount) * 100; // CORREGIR CÁLCULO
                    }

                    //crea la tabla sin controles.
                    for (int r2 = 0; r2 < rowCount; r2++)
                    {
                        // CAMBIO 1: Reducir altura de filas para mejor espaciado
                        tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.Absolute, 165F)); // Era 200F

                        for (int c2 = 0; c2 < columnCount; c2++)
                        {
                            // CAMBIO 2: Solo agregar estilos de columna una vez
                            if (r2 == 0) // Solo en la primera fila
                            {
                                tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));
                            }

                            if (cant > i)
                            {
                                Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

                                // *** AQUÍ ESTÁN LOS ÚNICOS CAMBIOS ***

                                // PASO 1: Usar tu función de corte (igual que antes)
                                string nombreProducto = FormatTextToTwoLines(arreglo[i].Articulo, 20);

                                // PASO 2: Agregar código de barras (igual que antes)
                                string codigoBarras = arreglo[i].ItemId ?? "";
                                string textoCompleto = nombreProducto + Environment.NewLine + codigoBarras;

                                // PASO 3: NUEVO - Aplicar centrado
                                cmd.Text = CenterTextLines(textoCompleto, 15); // Ajusta el 25 según necesites

                                // *** TODO LO DEMÁS IGUAL ***
                                cmd.Tag = arreglo[i].ItemId;
                                cmd.RootElement.ApplyShapeToControl = false;
                                //cmd.TextAlignment = ContentAlignment.BottomCenter;
                                cmd.TextWrap = true;
                                cmd.ButtonElement.TextElement.Font = new Font("Courier New", 8, FontStyle.Bold);
                                cmd.Image = LoadImageTableLayout(arreglo[i].ItemId);
                                cmd.Height = 155;
                                cmd.MaximumSize = new Size(0, 1);
                                cmd.MinimumSize = new Size(110, 155);
                                cmd.Dock = DockStyle.Fill;
                                cmd.ImageAlignment = ContentAlignment.TopCenter;
                                cmd.TextImageRelation = TextImageRelation.ImageAboveText;
                                cmd.Margin = new Padding(2);
                                cmd.Padding = new Padding(2, 2, 2, 10);

                                tblLay_Top20Productos.Controls.Add(cmd, c2, r2);
                                cmd.Click += Top20MasVendidos_Click;
                            }
                            i++;
                        }
                    }
                }
            }

            tblLay_Top20Productos.Visible = true;
            tblLay_Top20Productos.AutoScroll = true;
            tblLay_Top20Productos.AutoScrollPosition = new Point(0, 0);

            this.panelCuerpoResultado.Width = this.panelCuerpoResultado.Width + 50;
            this.panelCategoria.Visible = false;
            this.panelCategoria.Dock = DockStyle.None;
            this.panelCuerpoResultado.Visible = true;
            this.TopMost = true;
        }

        private void GeneraTableProducto(int columnCount, int rowCount, string categoria, List<ProductoArticulo> listaProductos)
        {

            float dimensionCuadro = 33.33F;
            dimensionCuadro = (float)(Convert.ToDouble(1) / columnCount) * 100;

            panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
            panelCuerpoResultado.Visible = true;
            panelCuerpoResultado.Size = new Size(601, 432);//535, 432
            dgvSearchProduct.Visible = false;

            panel1.Visible = false;

            //Clear out the existing controls, we are generating a new table layout
            tblLay_Top20Productos.Controls.Clear();

            if (listaProductos.Count == 0)
                return;

            //Clear out the existing row and column styles
            tblLay_Top20Productos.ColumnStyles.Clear();
            tblLay_Top20Productos.RowStyles.Clear();

            //Now we will generate the table, setting up the row and column counts first
            tblLay_Top20Productos.ColumnCount = columnCount;
            tblLay_Top20Productos.RowCount = rowCount;
            using (POSEntities db = new POSEntities())
            {
                var cant = listaProductos.ToArray().Count();
                var arreglo = listaProductos.ToArray();
                int i = 0;
                decimal x1 = 0;
                x1 = (Convert.ToDecimal(cant) / 4);
                int filas = Convert.ToInt16(decimal.Ceiling(x1));
                rowCount = filas;
                if (cant % columnCount == 0)
                {
                    rowCount = filas;
                }
                else
                {
                    rowCount = filas + 1;
                }

                tblLay_Top20Productos.RowCount = rowCount;

                if (cant < columnCount)
                {
                    rowCount = rowCount + 1;
                    tblLay_Top20Productos.RowCount = rowCount;
                }
                //crea la tabla sin controles.
                for (int r2 = 0; r2 < rowCount; r2++)
                {
                    tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));

                    for (int c2 = 0; c2 < columnCount; c2++)
                    {
                        tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));

                        if (cant > i)
                        {
                            //Create the control, in this case we will add a button
                            Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

                            cmd.Text = arreglo[i].ARTICULO + Environment.NewLine + "Cod. Barra: " + arreglo[i].BARRAS;
                            cmd.Tag = arreglo[i].BARRAS;
                            cmd.TextAlignment = ContentAlignment.BottomCenter;
                            cmd.TextWrap = true;
                            cmd.Font = new Font(Font.FontFamily, 12);
                            cmd.Image = LoadImageTableLayout(arreglo[i].BARRAS);
                            cmd.Height = 150;
                            cmd.Dock = DockStyle.Fill;
                            cmd.ImageAlignment = ContentAlignment.MiddleCenter;
                            tblLay_Top20Productos.Controls.Add(cmd, c2, r2);

                            cmd.Click += Top20MasVendidos_Click;


                        }
                        i++;

                    }
                }
            }
            tblLay_Top20Productos.Visible = true;
            tblLay_Top20Productos.MaximumSize = new Size(tblLay_Top20Productos.Width, tblLay_Top20Productos.Height);
            tblLay_Top20Productos.AutoScroll = true;
            tblLay_Top20Productos.AutoScrollPosition = new Point(0, 0);


            // rscrollbarVertical.Dock = DockStyle.Right;
            // rscrollbarVertical.ScrollType = Telerik.WinControls.UI.ScrollType.Vertical;

            //this.rscrollbarVertical.Maximum = this.tblLay_Top20Productos.Size.Height - this.tblLay_Top20Productos.Size.Height;
            // this.rscrollbarVertical.Scroll += RscrollbarVertical_Scroll;
            //rscrollbarVertical.Visible = true;

            this.panelCuerpoResultado.Width = this.panelCuerpoResultado.Width + 50;
            // this.panelCuerpoResultado.Controls.Add(rscrollbarVertical);
            // tblLay_Top20Productos.Dock = DockStyle.Fill;
            //  this.panelCuerpoResultado.Controls.Add(tblLay_Top20Productos);
            this.panelCategoria.Visible = false;
            this.panelCategoria.Dock = DockStyle.None;
            this.panelCuerpoResultado.Visible = true;
            this.TopMost = true;
            rbtnCategoria.Visible = true;
        }

        private void GenerateTable1(int columnCount, int rowCount, string categoria, List<VW_SEARCHPRODUCT> listaProductos)
        {

            float dimensionCuadro = 33.33F;
            dimensionCuadro = (float)(Convert.ToDouble(1) / columnCount) * 100;

            panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
            panelCuerpoResultado.Visible = true;
            panelCuerpoResultado.Size = new Size(601, 432);//535, 432
            dgvSearchProduct.Visible = false;

            panel1.Visible = false;

            //Clear out the existing controls, we are generating a new table layout
            tblLay_Top20Productos.Controls.Clear();

            if (listaProductos.Count == 0)
                return;

            //Clear out the existing row and column styles
            tblLay_Top20Productos.ColumnStyles.Clear();
            tblLay_Top20Productos.RowStyles.Clear();

            //Now we will generate the table, setting up the row and column counts first
            tblLay_Top20Productos.ColumnCount = columnCount;
            tblLay_Top20Productos.RowCount = rowCount;
            using (POSEntities db = new POSEntities())
            {
                var cant = listaProductos.ToArray().Count();
                var arreglo = listaProductos.ToArray();
                int i = 0;
                decimal x1 = 0;
                x1 = (Convert.ToDecimal(cant) / 4);
                int filas = Convert.ToInt16(decimal.Ceiling(x1));
                rowCount = filas;
                if (cant % columnCount == 0)
                {
                    rowCount = filas;
                }
                else
                {
                    rowCount = filas + 1;
                }

                tblLay_Top20Productos.RowCount = rowCount;

                if (cant < columnCount)
                {
                    /* columnCount = cant;
                     tblLay_Top20Productos.ColumnCount = columnCount;
                     dimensionCuadro = 1 / columnCount;
                     */
                    rowCount = rowCount + 1;
                    tblLay_Top20Productos.RowCount = rowCount;
                }
                //crea la tabla sin controles.
                for (int r2 = 0; r2 < rowCount; r2++)
                {
                    tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));

                    for (int c2 = 0; c2 < columnCount; c2++)
                    {
                        tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));

                        if (cant > i)
                        {
                            //Create the control, in this case we will add a button
                            Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

                            cmd.Text = arreglo[i].ARTICULO + Environment.NewLine + "Cod. Barra: " + arreglo[i].BARRAS;
                            cmd.Tag = arreglo[i].BARRAS;
                            cmd.TextAlignment = ContentAlignment.BottomCenter;
                            cmd.TextWrap = true;
                            cmd.Font = new Font(Font.FontFamily, 12);
                            cmd.Image = LoadImageTableLayout(arreglo[i].BARRAS);
                            cmd.Height = 150;
                            cmd.Dock = DockStyle.Fill;
                            cmd.ImageAlignment = ContentAlignment.MiddleCenter;

                            //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
                            tblLay_Top20Productos.Controls.Add(cmd, c2, r2);

                            cmd.Click += Top20MasVendidos_Click;


                        }
                        i++;

                    }
                }
                //posiciona los controles de acuerdo a los resultados.
                //foreach()
                /*
                                for (int x = 0; x < columnCount; x++)
                                {
                                    //First add a column
                                    //tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                                    tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));

                                    for (int y = 0; y < rowCount; y++)
                                    {
                                        //Next, add a row.  Only do this when once, when creating the first column
                                        if (x == 0)
                                        {
                                            //tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                            tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
                                        }
                                        if (cant > i)
                                        {

                                            //Create the control, in this case we will add a button
                                            Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

                                            cmd.Text = arreglo[i].ARTICULO + Environment.NewLine + "Cod. Barra: " + arreglo[i].BARRAS;
                                            cmd.Tag = arreglo[i].BARRAS;
                                            cmd.TextAlignment = ContentAlignment.BottomCenter;
                                            cmd.TextWrap = true;
                                            cmd.Font = new Font(Font.FontFamily, 12);
                                            cmd.Image = LoadImageTableLayout(arreglo[i].BARRAS);
                                            cmd.Height = 150;
                                            cmd.Dock = DockStyle.Fill;
                                            //cmd.Width = 80;
                                            //cmd.Image = new Image();
                                            cmd.ImageAlignment = ContentAlignment.MiddleCenter;

                                            //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
                                            tblLay_Top20Productos.Controls.Add(cmd, x, y);

                                            cmd.Click += Top20MasVendidos_Click;


                                        }
                                        i++;
                                    }
                                }*/
            }
            tblLay_Top20Productos.Visible = true;
            tblLay_Top20Productos.MaximumSize = new Size(tblLay_Top20Productos.Width, tblLay_Top20Productos.Height);
            tblLay_Top20Productos.AutoScroll = true;
            tblLay_Top20Productos.AutoScrollPosition = new Point(0, 0);


            // rscrollbarVertical.Dock = DockStyle.Right;
            // rscrollbarVertical.ScrollType = Telerik.WinControls.UI.ScrollType.Vertical;

            //this.rscrollbarVertical.Maximum = this.tblLay_Top20Productos.Size.Height - this.tblLay_Top20Productos.Size.Height;
            // this.rscrollbarVertical.Scroll += RscrollbarVertical_Scroll;
            //rscrollbarVertical.Visible = true;

            this.panelCuerpoResultado.Width = this.panelCuerpoResultado.Width + 50;
            // this.panelCuerpoResultado.Controls.Add(rscrollbarVertical);
            // tblLay_Top20Productos.Dock = DockStyle.Fill;
            //  this.panelCuerpoResultado.Controls.Add(tblLay_Top20Productos);
            this.panelCategoria.Visible = false;
            this.panelCategoria.Dock = DockStyle.None;
            this.panelCuerpoResultado.Visible = true;
            this.TopMost = true;
            rbtnCategoria.Visible = true;
        }

        private void GenerateTableListP(int columnCount, int rowCount, string categoria, List<ProductoArticulo> listaProductos)
        {

            float dimensionCuadro = 33.33F;
            dimensionCuadro = (float)(Convert.ToDouble(1) / columnCount) * 100;

            panelCuerpoResultado.Location = new System.Drawing.Point(21, 81);
            panelCuerpoResultado.Visible = true;
            panelCuerpoResultado.Size = new Size(601, 432);//535, 432
            dgvSearchProduct.Visible = false;

            panel1.Visible = false;

            //Clear out the existing controls, we are generating a new table layout
            tblLay_Top20Productos.Controls.Clear();

            if (listaProductos.Count == 0)
                return;

            //Clear out the existing row and column styles
            tblLay_Top20Productos.ColumnStyles.Clear();
            tblLay_Top20Productos.RowStyles.Clear();

            //Now we will generate the table, setting up the row and column counts first
            tblLay_Top20Productos.ColumnCount = columnCount;
            tblLay_Top20Productos.RowCount = rowCount;
            using (POSEntities db = new POSEntities())
            {
                var cant = listaProductos.ToArray().Count();
                var arreglo = listaProductos.ToArray();
                int i = 0;
                decimal x1 = 0;
                x1 = (Convert.ToDecimal(cant) / 4);
                int filas = Convert.ToInt16(decimal.Ceiling(x1));
                rowCount = filas;
                if (cant % columnCount == 0)
                {
                    rowCount = filas;
                }
                else
                {
                    rowCount = filas + 1;
                }

                tblLay_Top20Productos.RowCount = rowCount;

                if (cant < columnCount)
                {
                    /* columnCount = cant;
                     tblLay_Top20Productos.ColumnCount = columnCount;
                     dimensionCuadro = 1 / columnCount;
                     */
                    rowCount = rowCount + 1;
                    tblLay_Top20Productos.RowCount = rowCount;
                }
                //crea la tabla sin controles.
                for (int r2 = 0; r2 < rowCount; r2++)
                {
                    tblLay_Top20Productos.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));

                    for (int c2 = 0; c2 < columnCount; c2++)
                    {
                        tblLay_Top20Productos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, dimensionCuadro));

                        if (cant > i)
                        {
                            //Create the control, in this case we will add a button
                            Telerik.WinControls.UI.RadButton cmd = new Telerik.WinControls.UI.RadButton();

                            string nombreProducto = FormatTextToTwoLines(arreglo[i].ARTICULO, 20);

                            string codigoBarras = arreglo[i].BARRAS ?? "";
                            string textoCompleto = nombreProducto + Environment.NewLine + codigoBarras;

                            cmd.Text = CenterTextLines(textoCompleto, 15);

                            //cmd.Tag = arreglo[i].ItemId;
                            //cmd.RootElement.ApplyShapeToControl = false;

                            //cmd.Text = arreglo[i].ARTICULO + Environment.NewLine + "Cod. Barra: " + arreglo[i].BARRAS;
                            //cmd.Tag = arreglo[i].BARRAS;
                            //cmd.TextAlignment = ContentAlignment.BottomCenter;
                            //cmd.TextWrap = true;
                            //cmd.Font = new Font(Font.FontFamily, 12);
                            //cmd.Image = LoadImageTableLayout(arreglo[i].BARRAS);
                            //cmd.Height = 150;
                            //cmd.Dock = DockStyle.Fill;
                            //cmd.ImageAlignment = ContentAlignment.MiddleCenter;

                            //cmd.Text = string.Format("({0}, {1})", x, y);         //Finally, add the control to the correct location in the table
                            //tblLay_Top20Productos.Controls.Add(cmd, c2, r2);

                            //cmd.Click += Top20MasVendidos_Click;

                            cmd.TextWrap = true;
                            cmd.ButtonElement.TextElement.Font = new Font("Courier New", 8, FontStyle.Bold);
                            cmd.Tag = arreglo[i].BARRAS; // O el identificador que uses (ItemId, BARRAS, etc.)
                            cmd.RootElement.ApplyShapeToControl = false;
                            cmd.Image = LoadImageTableLayout(arreglo[i].BARRAS);
                            cmd.Height = 155;
                            cmd.MaximumSize = new Size(0, 165);
                            cmd.MinimumSize = new Size(110, 155);
                            cmd.Dock = DockStyle.Fill;
                            cmd.ImageAlignment = ContentAlignment.TopCenter;
                            cmd.TextImageRelation = TextImageRelation.ImageAboveText;
                            cmd.Margin = new Padding(2);
                            cmd.Padding = new Padding(2, 2, 2, 10);

                            tblLay_Top20Productos.Controls.Add(cmd, c2, r2);
                            cmd.Click += Top20MasVendidos_Click;


                        }
                        i++;

                    }
                }

            }
            tblLay_Top20Productos.Visible = true;
            tblLay_Top20Productos.MaximumSize = new Size(tblLay_Top20Productos.Width, tblLay_Top20Productos.Height);
            tblLay_Top20Productos.AutoScroll = true;
            tblLay_Top20Productos.AutoScrollPosition = new Point(0, 0);


            // rscrollbarVertical.Dock = DockStyle.Right;
            // rscrollbarVertical.ScrollType = Telerik.WinControls.UI.ScrollType.Vertical;

            //this.rscrollbarVertical.Maximum = this.tblLay_Top20Productos.Size.Height - this.tblLay_Top20Productos.Size.Height;
            // this.rscrollbarVertical.Scroll += RscrollbarVertical_Scroll;
            //rscrollbarVertical.Visible = true;

            this.panelCuerpoResultado.Width = this.panelCuerpoResultado.Width + 50;
            // this.panelCuerpoResultado.Controls.Add(rscrollbarVertical);
            // tblLay_Top20Productos.Dock = DockStyle.Fill;
            //  this.panelCuerpoResultado.Controls.Add(tblLay_Top20Productos);
            this.panelCategoria.Visible = false;
            this.panelCategoria.Dock = DockStyle.None;
            this.panelCuerpoResultado.Visible = true;
            this.TopMost = true;
            rbtnCategoria.Visible = true;
        }
        //private void RscrollbarVertical_Scroll(object sender, ScrollEventArgs e)
        //{
        //    int change = tblLay_Top20Productos.VerticalScroll.Value + tblLay_Top20Productos.VerticalScroll.SmallChange + 30;
        //    this.tblLay_Top20Productos.Top = -this.rscrollbarVertical.Value;
        //    this.tblLay_Top20Productos.AutoScrollPosition = new Point(0, change);
        //}

        private void LlenaTbleLayoutTop20(string categoria)
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var cant = db.TblItemsMaxSales.Where(x => x.Categoria == categoria).ToArray().Count();
                    System.Windows.Forms.RadioButton[] radioButtons = new System.Windows.Forms.RadioButton[cant];
                    var arreglo = db.TblItemsMaxSales.Where(x => x.Categoria == categoria).ToArray();
                    var lin = 0;
                    var col = 1;
                    for (int i = 0; i < cant; ++i)
                    {
                        if (i % 7 == 0 && i > 1)
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
                        radioButtons[i].Text = arreglo[i].Articulo;
                        radioButtons[i].Tag = arreglo[i].ItemId;
                        radioButtons[i].Font = new Font(Font.FontFamily, 12);

                        radioButtons[i].Location = new System.Drawing.Point(10 + col, 10 + lin * 60);
                        radioButtons[i].Appearance = Appearance.Button;
                        this.tblLay_Top20Productos.Controls.Add(radioButtons[i]);
                        radioButtons[i].CheckedChanged += RadioButton_CheckedChanged;
                    }
                    this.panelCuerpoResultado.Controls.Add(tblLay_Top20Productos);
                    tblLay_Top20Productos.Visible = true;

                }
            }
            catch (Exception)
            {

                throw;
            }
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

        private void btnChooseProduct_Click(object sender, EventArgs e)
        {
            int row;
            if (dgvSearchProduct.CurrentCell != null)
            {
                row = dgvSearchProduct.CurrentCell.RowIndex;
                code = dgvSearchProduct.Rows[row].Cells[2].Value.ToString();
                this.Close();
            }
        }

        private void dgvSearchProduct_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row;

            row = dgvSearchProduct.CurrentCell.RowIndex;
            code = dgvSearchProduct.Rows[row].Cells[2].Value.ToString();
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
            LoadImagenGridView();
        }

        private void rbtnCategoria_Click(object sender, EventArgs e)
        {
            try
            {
                categoria = string.Empty;
                this.tblLay_Top20Productos.Visible = false;
                this.panelCategoria.Visible = true;
                lblEtiquetaCategoriaSeleccionada.Text = "Todas";

            }
            catch (Exception)
            {


            }
        }

        private void radbtnUp_Click(object sender, EventArgs e)
        {
            int verticalScroll = 0;
            int verticalscrollSmall = 0;
            int change = 0;
            if (tblLay_CategoriaProducto.Visible)
            {
                verticalScroll = tblLay_CategoriaProducto.VerticalScroll.Value;
                verticalscrollSmall = tblLay_CategoriaProducto.VerticalScroll.SmallChange;
                change = verticalScroll - (verticalscrollSmall + 30);
                if (!tblLay_CategoriaProducto.AutoScrollPosition.IsEmpty)
                {
                    this.tblLay_CategoriaProducto.AutoScrollPosition = new Point(0, change);
                }
            }
            else
            {
                //if(string.IsNullOrEmpty(categoria))
                verticalScroll = tblLay_Top20Productos.VerticalScroll.Value;
                verticalscrollSmall = tblLay_Top20Productos.VerticalScroll.SmallChange;
                change = verticalScroll - (verticalscrollSmall + 30);
                //int change = tblLay_Top20Productos.VerticalScroll.Value - tblLay_Top20Productos.VerticalScroll.SmallChange + 30;
                //this.tblLay_Top20Productos.Top = -this.rscrollbarVertical.Value;
                if (!tblLay_Top20Productos.AutoScrollPosition.IsEmpty)
                {
                    this.tblLay_Top20Productos.AutoScrollPosition = new Point(0, change);
                }
            }
        }

        private void radbtnDown_Click(object sender, EventArgs e)
        {

            int verticalScroll = 0;
            int verticalscrollSmall = 0;
            int change = 0;
            if (tblLay_CategoriaProducto.Visible)
            {
                verticalScroll = tblLay_CategoriaProducto.VerticalScroll.Value;
                verticalscrollSmall = tblLay_CategoriaProducto.VerticalScroll.SmallChange;
                change = verticalScroll + (verticalscrollSmall + 30);
                //if (!tblLay_CategoriaProducto.AutoScrollPosition.IsEmpty)
                //{
                this.tblLay_CategoriaProducto.AutoScrollPosition = new Point(0, change);
                //}
            }
            else
            {
                //if(string.IsNullOrEmpty(categoria))
                verticalScroll = tblLay_Top20Productos.VerticalScroll.Value;
                verticalscrollSmall = tblLay_Top20Productos.VerticalScroll.SmallChange;
                change = verticalScroll + (verticalscrollSmall + 30);
                //int change = tblLay_Top20Productos.VerticalScroll.Value - tblLay_Top20Productos.VerticalScroll.SmallChange + 30;
                //this.tblLay_Top20Productos.Top = -this.rscrollbarVertical.Value;
                //if (!tblLay_Top20Productos.AutoScrollPosition.IsEmpty)
                //{
                this.tblLay_Top20Productos.AutoScrollPosition = new Point(0, change);
                //  }
            }

            //int change = tblLay_Top20Productos.VerticalScroll.Value + tblLay_Top20Productos.VerticalScroll.SmallChange + 30;            
            //this.tblLay_Top20Productos.AutoScrollPosition = new Point(0, change);
        }

        private string FormatTextToTwoLines(string text, int maxCharsPerLine = 20)
        {
            if (string.IsNullOrEmpty(text))
                return "".PadRight(maxCharsPerLine) + Environment.NewLine + "".PadRight(maxCharsPerLine);

            // Remover espacios extra y limpiar el texto
            text = text.Trim().Replace("  ", " ");

            string line1 = "";
            string line2 = "";

            if (text.Length <= maxCharsPerLine)
            {
                // Si el texto cabe en una línea
                line1 = text.PadRight(maxCharsPerLine);
                line2 = "".PadRight(maxCharsPerLine);
            }
            else
            {
                // Cortar por palabras completas aprovechando al máximo la primera línea
                string[] words = text.Split(' ');
                string tempLine1 = "";
                int wordIndex = 0;

                // Llenar la primera línea con tantas palabras completas como sea posible
                for (int i = 0; i < words.Length; i++)
                {
                    string testLine = tempLine1;
                    if (testLine.Length > 0) testLine += " ";
                    testLine += words[i];

                    if (testLine.Length <= maxCharsPerLine)
                    {
                        tempLine1 = testLine;
                        wordIndex = i + 1;
                    }
                    else
                    {
                        break; // Ya no cabe más en la primera línea
                    }
                }

                // Si no pudimos meter ninguna palabra completa (palabra muy larga)
                if (tempLine1.Length == 0)
                {
                    line1 = text.Substring(0, maxCharsPerLine);
                    string remaining = text.Substring(maxCharsPerLine);
                    line2 = remaining.Length > maxCharsPerLine ?
                            remaining.Substring(0, maxCharsPerLine) :
                            remaining.PadRight(maxCharsPerLine);
                }
                else
                {
                    // Completar primera línea con espacios
                    line1 = tempLine1.PadRight(maxCharsPerLine);

                    // Construir segunda línea con las palabras restantes
                    string remainingWords = "";
                    for (int i = wordIndex; i < words.Length; i++)
                    {
                        if (remainingWords.Length > 0) remainingWords += " ";
                        remainingWords += words[i];
                    }

                    // Si la segunda línea es muy larga, cortarla
                    line2 = remainingWords.Length > maxCharsPerLine ?
                            remainingWords.Substring(0, maxCharsPerLine) :
                            remainingWords.PadRight(maxCharsPerLine);
                }
            }

            return line1 + Environment.NewLine + line2;
        }

        private string CenterTextLines(string formattedText, int displayWidth = 25)
        {
            if (string.IsNullOrEmpty(formattedText)) return formattedText;

            string[] lines = formattedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string result = "";

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Solo centrar si la línea no está vacía y no son solo espacios
                if (!string.IsNullOrEmpty(line.Trim()))
                {
                    // Remover padding derecho de tu función original
                    string cleanLine = line.TrimEnd();

                    // Calcular espacios para centrar
                    int spacesNeeded = Math.Max(0, (displayWidth - cleanLine.Length) / 2);
                    line = new string(' ', spacesNeeded) + cleanLine;
                }

                result += line;
                if (i < lines.Length - 1)
                    result += Environment.NewLine;
            }

            return result;
        }


    }
}