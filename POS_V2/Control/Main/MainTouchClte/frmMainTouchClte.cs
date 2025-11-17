using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using POS.Models;

namespace POS.Control.Main.MainTouch
{
    public partial class frmMainTouchClte : Telerik.WinControls.UI.RadForm
    {
        private Factura _factura;

        public frmMainTouchClte(Factura factura) : this()
        {
            _factura = factura;
            //FacturaService.ProductosActualizados += ActualizarTicket;

            // Refrescar productos iniciales
            RefrescarProductos(_factura);

            // Establecer fuente monoespaciada para simular ticket
            txtTicket.Font = new Font("Courier New", 15, FontStyle.Regular);
        }


        
        private void frmMainTouchClte_FormClosing(object sender, FormClosingEventArgs e)
        {


            Control.Common.GlobalParameters.frmTouchClte = null;

        }


        // (En frmMainTouchClte.cs - Constructor)

        public frmMainTouchClte()
        {
            InitializeComponent();

            // --- CÓDIGO FINAL (Modo "Solo Imagen") ---

            // 1. Ocultar todos los otros controles
            gridItemsClte.Visible = false;
            panel1.Visible = false;
            pictureBox1.Visible = false;
            lblNombreCliente.Visible = false;
            lblIdentificacionClte.Visible = false;
            radLabel1.Visible = false;
            radLabel2.Visible = false;
            radGridView1.Visible = false;

            // 2. ¡VOLVEMOS A CARGAR LA IMAGEN!
            CargarImagenIdleDesdeDB();

            // 3. Configurar y mostrar 'picInactive'
            if (picInactive != null)
            {
                // YA NO USAMOS EL COLOR ROJO
                // picInactive.BackColor = Color.Red; 

                picInactive.Dock = DockStyle.Fill;
                picInactive.Visible = true;
                picInactive.BringToFront();
            }
        }

        private  void AgregarProductoAlTicket(Producto producto)
        {
            // Formato: Nombre                          Cantidad x Precio
            string linea = $"{producto.Nombre.PadRight(25)} 1 x ${producto.Pvp:F2}\r\n";
            txtTicket.AppendText(linea);

            // Recalcular total
            decimal total = _factura.Productos.Sum(p => p.Pvp);
            ActualizarTotalEnTicket(total);
        }

        public void ActualizarTotalEnTicket(decimal total)
        {
            // Eliminar la línea de total anterior (si existe)
            string contenido = txtTicket.Text;
            int indexTotal = contenido.IndexOf("Total:");

            if (indexTotal >= 0)
            {
                contenido = contenido.Substring(0, indexTotal);
            }

            // Añadir nueva línea de total
            contenido += $"\r\nTotal:                   ${total:F2}";
            txtTicket.Text = contenido;
        }

        public void EliminarProducto(string codigo)
        {
            // Buscar el producto por Id (código)
            var productoAEliminar = _factura.Productos.FirstOrDefault(p => p.Id == codigo);

            if (productoAEliminar != null)
            {
                _factura.Productos.Remove(productoAEliminar);
                ActualizarTicket(this, _factura.Productos); // Vuelve a dibujar el ticket
            }
        }

        public void AgregarProductoEnNegativo(Producto productoOriginal, decimal cantidadANegar)
        {
            var productoDevuelto = new Producto();

            // Copiar datos del producto original
            productoDevuelto.Id = productoOriginal.Id;
            productoDevuelto.Nombre = productoOriginal.Nombre;
            productoDevuelto.Pvp = productoOriginal.Pvp;
            productoDevuelto.Cantidad = -Math.Abs(cantidadANegar); // Cantidad negativa
            productoDevuelto.Subtotal = productoDevuelto.Cantidad * productoDevuelto.Pvp;

            _factura.Productos.Add(productoDevuelto);
            ActualizarTicket(this, _factura.Productos);
        }

        public void ActualizarTicket(object sender, BindingList<Producto> productos)
        {


            //txtTicket.Font = new Font("Courier New", 15, FontStyle.Regular);

            // Si no hay productos, limpiamos el ticket y mostramos encabezado
            if (productos == null || productos.Count == 0)
            {
                txtTicket.Clear();
                string cabecera = "DESCRIPCIÓN              Cant       PVP       Total \n" + Environment.NewLine;
                cabecera = cabecera + "-------------------------------------------------- \n" + Environment.NewLine;
                txtTicket.AppendText(cabecera);
                return;
            }

            // Obtener solo el último producto agregado
            var ultimoProducto = productos.Last();

            // Formatear la línea del producto
            string line = $"{ultimoProducto.Nombre.Substring(0,20) ,-20}" +
                          $"{ultimoProducto.Cantidad,6:F2}" +
                          $"{ultimoProducto.Pvp,10:C}" +
                          $"{ultimoProducto.Subtotal,10:C}\n" + Environment.NewLine

                          ;

            // Agregar solo la línea del nuevo producto
            //txtTicket.AppendText(line);
        }

     
        private void frmMainTouchClte_Load(object sender, EventArgs e)
        {
            // Puedes dejar esto vacío o usarlo para inicializaciones adicionales
        }



        private void ConfigurarColumnas(RadGridView grid)
        {
            grid.Columns.Clear();
            //grid.Columns.Add(new GridViewTextBoxColumn("Codigo", "Codigo") { HeaderText = "Código" });
            grid.Columns.Add(new GridViewTextBoxColumn("Nombre", "Nombre") { HeaderText = "Nombre" });
            grid.Columns.Add(new GridViewDecimalColumn("Cantidad", "Cantidad") { HeaderText = "Cantidad" });
            grid.Columns.Add(new GridViewDecimalColumn("PVP", "PVP") { HeaderText = "PVP" });
            grid.Columns.Add(new GridViewDecimalColumn("Total", "Total") { HeaderText = "Total" });
            grid.Columns.Add(new GridViewDecimalColumn("IdTemporal", "IdTemporal") { HeaderText = "IdTemporal" });

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClienteService.ClienteActualizado -= ClienteService_ClienteActualizado;
            FacturaService.ProductosActualizados -= FacturaService_ProductosActualizados;

            base.OnFormClosed(e);
        }


        private void FacturaService_ProductosActualizados(object sender, BindingList<Producto> productos)
        {
            this.InvokeIfRequired(() =>
            {
                // --- INICIO DE LA CORRECCIÓN ---

                // El problema era que "if (_factura != null)" era FALSO.
                // Vamos a asegurarnos de que _factura exista
                // o simplemente pasemos los productos directamente.

                // 1. Asegurémonos de que _factura no sea null
                if (_factura == null)
                {
                    _factura = new Factura();
                }

                // 2. Asignamos la nueva lista de productos (la que viene del evento)
                _factura.Productos = productos;

                // 3. AHORA SÍ llamamos a RefrescarProductos
                RefrescarProductos(_factura);

                // --- FIN DE LA CORRECCIÓN ---
            });
        }

        private void ConfigurarGrid()
        {
            gridItemsClte.AutoGenerateColumns = false;
            gridItemsClte.Columns.Clear(); // Limpia columnas del diseñador

            // Basado en tu Designer.cs, tu clase Producto usa estos nombres:

            var colNombre = new GridViewTextBoxColumn("Nombre")
            { HeaderText = "Descripción", Width = 275 };

            var colCantidad = new GridViewDecimalColumn("CantidadINEC")
            { HeaderText = "Cantidad", Width = 80 };

            var colUnidades = new GridViewDecimalColumn("Unidades")
            { HeaderText = "Unidades", Width = 80 };

            var colPrecio = new GridViewDecimalColumn("Pvp")
            { HeaderText = "P.V.P", Width = 60 };

            var colTotal = new GridViewDecimalColumn("TotalPromoIVA")
            { HeaderText = "Total", Width = 90 };

            var colIdTemporal = new GridViewDecimalColumn("IdTemporal")
            { HeaderText = "IdTemporal", IsVisible = false };


            gridItemsClte.Columns.Add(colNombre);
            gridItemsClte.Columns.Add(colCantidad);
            gridItemsClte.Columns.Add(colUnidades);
            gridItemsClte.Columns.Add(colPrecio);
            gridItemsClte.Columns.Add(colTotal);
            gridItemsClte.Columns.Add(colIdTemporal);

            // Tu código de sorting (esto está bien)
            gridItemsClte.EnableCustomSorting = true;
            gridItemsClte.CustomSorting += RadGridView1_CustomSorting;
            if (gridItemsClte.Columns.Contains("IdTemporal"))
            {
                gridItemsClte.Columns["IdTemporal"].SortOrder = RadSortOrder.Descending;
            }
        }


        private void RadGridView1_CustomSorting(object sender, GridViewCustomSortingEventArgs e)
        {
            Int64 row1Freight = (Int64)e.Row1.Cells["IdTemporal"].Value;
            Int64 row2Freight = (Int64)e.Row2.Cells["IdTemporal"].Value;

            if (row1Freight < row2Freight)
            {
                e.SortResult = 1;
            }
            else if (row1Freight > row2Freight)
            {
                e.SortResult = -1;
            }
            else
            {
                e.SortResult = 0;
            }
        }


        public void RefrescarProductos(Factura factura)
        {
            if (factura == null || factura.Productos == null)
            {
                gridItemsClte.DataSource = null;
                return;
            }

            _factura = factura;

            gridItemsClte.DataSource = null;
            gridItemsClte.DataSource = _factura.Productos;
        }

        void ScrollLastGridItems()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(ScrollLastGridItems));
                    return;
                }

                if (gridItemsClte.Rows.Count > 0)
                {
                    gridItemsClte.Update();
                    gridItemsClte.Refresh();

                    int lastIndex = gridItemsClte.Rows.Count - 1;
                    if (lastIndex >= 0)
                    {
                        var lastRow = gridItemsClte.Rows[lastIndex];
                        gridItemsClte.TableElement.ScrollToRow(lastRow);
                        gridItemsClte.CurrentRow = lastRow;
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(ScrollLastGridItems),
                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                    "StackTrace: " + ex.StackTrace);
            }
        }


        private void InvokeIfRequired(MethodInvoker action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void ClienteService_ClienteActualizado(object sender, ClienteEmpleado cliente)
        {
            this.InvokeIfRequired(() => ActualizarDatosCliente(cliente));
        }

        private void SetGlobalRowFormatting(object sender, RowFormattingEventArgs e)
        {
            e.RowElement.DrawFill = true;
            e.RowElement.GradientStyle = Telerik.WinControls.GradientStyles.Solid;
            e.RowElement.BackColor = Control.Common.GlobalParameters.Color_GridViewBackground;
            e.RowElement.BackColor2 = Control.Common.GlobalParameters.Color_GridViewBackground;
            e.RowElement.BackColor3 = Control.Common.GlobalParameters.Color_GridViewBackground;
            e.RowElement.BackColor4 = Control.Common.GlobalParameters.Color_GridViewBackground;
        }

    
        public void ActualizarDatosCliente(ClienteEmpleado cliente)
        {
            string NombreCliente = string.Empty;
            string IdentificacionClte = string.Empty;

            if (cliente == null)
            {
                NombreCliente = "#######";
                IdentificacionClte = "#######";

                lblNombreCliente.Text = NombreCliente;
                lblIdentificacionClte.Text = IdentificacionClte;
                return;
            }

            if (cliente.Identificacion == "9999999999999")
            {
                NombreCliente = "CONSUMIDOR FINAL";
                IdentificacionClte = cliente.Identificacion;
            }
            else
            {
                NombreCliente = cliente.NombreCliente;
                IdentificacionClte = cliente.Identificacion;

            }

            lblNombreCliente.Text = NombreCliente;
            lblIdentificacionClte.Text = IdentificacionClte;
            txtTicket.Clear();


        }

        private void OnTextBoxClick(object sender, EventArgs e)
        {
            var txt = sender as RadTextBox;
            if (txt != null)
            {
                var teclado = new ToolBox.frmTecladoCompleto(txt);
                teclado.ShowDialog();
            }
        }

        private void OnTextBoxEnter(object sender, EventArgs e)
        {
            var txt = sender as RadTextBox;
            if (txt != null)
            {
                txt.SelectionStart = txt.Text.Length;
            }
        }

        private IEnumerable<System.Windows.Forms.Control> GetAllControls(System.Windows.Forms.Control parent)
        {
            foreach (System.Windows.Forms.Control control in parent.Controls)
            {
                yield return control;
                if (control.HasChildren)
                {
                    foreach (var child in GetAllControls(control))
                        yield return child;
                }
            }
        }

        //seccion para llamar una imagen estatica 

        private void CargarImagenIdleDesdeDB()
        {
            string rutaImagen = ObtenerRutaImagenDesdeDB("WALLPAPER_CLIENTE");

            if (!string.IsNullOrEmpty(rutaImagen))
            {
                try
                {
                    // Verificamos si el archivo existe
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        // --- INICIO DE LA MEJORA ---
                        // En lugar de Image.FromFile(ruta), que puede fallar,
                        // leemos el archivo en un array de bytes primero.

                        byte[] imageBytes = System.IO.File.ReadAllBytes(rutaImagen);

                        // Creamos un stream en memoria con esos bytes
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            // Cargamos la imagen desde la memoria
                            // Esto es mucho más robusto.
                            picInactive.Image = Image.FromStream(ms);
                        }
                        // --- FIN DE LA MEJORA ---

                        picInactive.SizeMode = PictureBoxSizeMode.StretchImage; // O Zoom
                    }
                    else
                    {
                        // La ruta es válida pero el archivo no existe
                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Error,
                            nameof(frmMainTouchClte),
                            nameof(CargarImagenIdleDesdeDB),
                            $"La imagen de wallpaper no se encontró en la ruta: {rutaImagen}");
                    }
                }
                catch (Exception ex)
                {
                    // Error al cargar la imagen
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte),
                        nameof(CargarImagenIdleDesdeDB),
                        $"Error al cargar imagen desde ruta: {ex.Message}");
                }
            }
            else
            {
                // El parámetro no se encontró en la base de datos
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Warning,
                    nameof(frmMainTouchClte),
                    nameof(CargarImagenIdleDesdeDB),
                    "No se encontró el parámetro 'WALLPAPER_CLIENTE' en la DB.");
            }
        }
        /// <summary>
        /// Obtiene un valor de parámetro desde la base de datos usando Entity Framework.
        /// </summary>
        /// <param name="identificador">El ID del parámetro a buscar (ej: "WALLPAPER_CLIENTE")</param>
        /// <returns>El valor (la ruta) como string, o null si no se encuentra.</returns>
        private string ObtenerRutaImagenDesdeDB(string identificador)
        {
            try
            {
                // 1. Usa tu contexto de base de datos (POSEntities)
                using (POSEntities db = new POSEntities())
                {
                    // 2. Busca el parámetro específico
                    var parametro = db.core_parametro
                                      .FirstOrDefault(x => x.identificador == identificador);

                    // 3. Verifica si se encontró
                    if (parametro != null)
                    {
                        return parametro.valor; // Devuelve la ruta: "\\srvallia\Shares\..."
                    }
                    else
                    {
                        // Si no se encuentra, lo registra en el log (buena práctica)
                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Warning,
                            nameof(frmMainTouchClte),
                            nameof(ObtenerRutaImagenDesdeDB),
                            $"No se encontró el parámetro '{identificador}' en la tabla core_parametro.");

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Registra cualquier error de conexión o consulta
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(ObtenerRutaImagenDesdeDB),
                    $"Error al consultar la DB por el parámetro '{identificador}'. Error: {ex.Message}");

                return null;
            }
        }

    }
}
