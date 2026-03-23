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
using Microsoft.Win32;



namespace POS.Control.Main.MainTouch
{
    public partial class frmMainTouchClte : Telerik.WinControls.UI.RadForm
    {
        private Factura _factura;
        private Panel pnlHeader;
        private PictureBox picLogo;
        private Label lblCedRuc, lblCedRucVal;
        private Label lblCliente, lblClienteVal;
        private Label lblDireccion, lblDireccionVal;
        private Label lblTelefono, lblTelefonoVal;
        private Label lblClubDelportal, lblClubDelportalVal;
        private Label lblBnfIva, lblBnfIvaVal;
        private Panel pnlTotales;
        private Label lblValorTit, lblValorVal;
        private Label lblDescuentoTit, lblDescuentoVal;
        private Label lblIvaTit, lblIvaVal;
        private Label lblTotalTit, lblTotalVal;
        private System.Windows.Forms.WebBrowser webViewAds;
        internal bool _cargandoFacturaTemporal = false;
        private string _ultimaIdentificacionMostrada = string.Empty;
        private bool _mensajeAbierto = false;
        private Panel pnlEncuesta;
        private System.Windows.Forms.Timer timerEncuesta;
        private int _segundosEncuesta = 30; // default
        private string _encuestaNumeroFactura = "";
        private string _encuestaIdentificacion = "";
        private string _encuestaNombreCliente = "";

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
            //timerInactividad?.Stop();
            //timerInactividad?.Dispose();
            webViewAds?.Dispose();
            Control.Common.GlobalParameters.frmTouchClte = null;
        }


        // (En frmMainTouchClte.cs - Constructor)

        public frmMainTouchClte()
        {
            InitializeComponent();

            // Ocultar la imagen de publicidad
            if (picInactive != null)
            {
                picInactive.Visible = false;
            }

            // Asegurarte que los controles estén visibles
            gridItemsClte.Visible = true;
            panel1.Visible = true;
            pictureBox1.Visible = true;
            lblNombreCliente.Visible = true;
            lblIdentificacionClte.Visible = true;
            radLabel1.Visible = true;
            radLabel2.Visible = true;
            radGridView1.Visible = true;
        }

        private void AgregarProductoAlTicket(Producto producto)
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
            string line = $"{ultimoProducto.Nombre.Substring(0, 20),-20}" +
                          $"{ultimoProducto.Cantidad,6:F2}" +
                          $"{ultimoProducto.Pvp,10:C}" +
                          $"{ultimoProducto.Subtotal,10:C}\n" + Environment.NewLine

                          ;

            // Agregar solo la línea del nuevo producto
            //txtTicket.AppendText(line);
        }


        private void frmMainTouchClte_Load(object sender, EventArgs e)
        {
          

            ClienteService.ClienteActualizado += ClienteService_ClienteActualizado;
            FacturaService.ProductosActualizados += FacturaService_ProductosActualizados;

            panel1.Visible = false;
            panel3.Visible = false;
            picInactive.Visible = false;

            ConstruirLayoutPDF();
            InicializarWebViewPublicidad();
            //InicializarTimerInactividad();
            this.SetStyle(ControlStyles.Selectable, true);
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
            this.InvokeIfRequired(() => ActualizarDatosCliente(cliente, mostrarMensajeNoApp: true));
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


        public void ActualizarDatosCliente(ClienteEmpleado cliente, bool mostrarMensajeNoApp = true)
        {
            // En ActualizarDatosCliente, al inicio
            Control.Common.Logger.LogMessage(
                Control.Common.Enum.LogTypes.Info,
                nameof(frmMainTouchClte),
                nameof(ActualizarDatosCliente),
                $"LLAMADA => Id:{cliente?.Identificacion} | " +
                $"EsApp:{cliente?.EsClienteApp} | " +
                $"mostrarMsg:{mostrarMensajeNoApp} | " +
                $"cargandoTmp:{_cargandoFacturaTemporal} | " +
                $"StackTrace:{new System.Diagnostics.StackTrace().ToString().Substring(0, 200)}");

            if (cliente == null)
            {
                lblCedRucVal.Text = "";
                lblClienteVal.Text = "";
                lblDireccionVal.Text = "";
                lblTelefonoVal.Text = "";
                lblClubDelportalVal.Text = "";
                lblBnfIvaVal.Text = "";
                return;
            }

            lblCedRucVal.Text = cliente.Identificacion;
            lblClienteVal.Text = cliente.Identificacion == "9999999999999"
                                       ? "CONSUMIDOR FINAL"
                                       : cliente.NombreCliente;
            lblDireccionVal.Text = cliente.DireccionCliente;
            lblTelefonoVal.Text = cliente.TelefonoCliente;
            lblClubDelportalVal.Text = cliente.EsClienteApp ? "Si" : "No";
            lblBnfIvaVal.Text = cliente.esBeneficiarioDevolucionIVA ? "Si" : "No";

            // ── Guardar el último cliente mostrado ──
            _ultimaIdentificacionMostrada = cliente.Identificacion;

            // ── Mensaje cliente NO app ──
            if (mostrarMensajeNoApp
                && !_cargandoFacturaTemporal
                && !cliente.EsClienteApp
                && cliente.Identificacion != "9999999999999")
            {
                if (!_mensajeAbierto)
                {
                    _mensajeAbierto = true;
                    Screen screenClte = Screen.FromControl(this);

                    var t = new System.Threading.Thread(() =>
                    {
                        try { Control.Common.General.GetMensajeToList(10024, screenClte); }
                        finally { _mensajeAbierto = false; }
                    });
                    t.SetApartmentState(System.Threading.ApartmentState.STA);
                    t.Start();
                }
            }
            // ── Mensaje cliente App ──
            else if (mostrarMensajeNoApp
                && !_cargandoFacturaTemporal
                && cliente.EsClienteApp
                && cliente.Identificacion != "9999999999999")
            {
                // ── Solo abrir si no hay mensaje activo ──
                if (!_mensajeAbierto)  // ← ya NO resetear aquí
                {
                    _mensajeAbierto = true;
                    var nombreCliente = cliente.NombreCliente;
                    var saldoApp = cliente.SaldoApp;
                    Screen screenClte = Screen.FromControl(this);

                    var t = new System.Threading.Thread(() =>
                    {
                        try
                        {
                            var parametros = new List<ParametrosMensajes>
                {
                    new ParametrosMensajes() { codigo = "[NombreApellido]", valor = nombreCliente },
                    new ParametrosMensajes() { codigo = "[SaldoMonedero]",  valor = saldoApp.ToString("N2") }
                };
                            Control.Common.General.GetMensajeToList(10025, screenClte, parametros);
                        }
                        finally { _mensajeAbierto = false; }
                    });
                    t.SetApartmentState(System.Threading.ApartmentState.STA);
                    t.Start();
                }
            }
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

        private void ConstruirLayoutPDF()
        {
            int formWidth = this.ClientSize.Width;   // 1536
            int formHeight = this.ClientSize.Height;  // 1108
            int anchoIzq = (int)(formWidth * 0.60); // ~920px
            int anchoDer = formWidth - anchoIzq;    // ~616px

            // Ocultar labels viejos
            radLabel1.Visible = false;
            radLabel2.Visible = false;
            lblNombreCliente.Visible = false;
            lblIdentificacionClte.Visible = false;

            // ── HEADER ────────────────────────────────────────────────
            pnlHeader = new Panel();
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Size = new Size(anchoIzq, 200);
            pnlHeader.BackColor = Color.White;
            pnlHeader.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            picLogo = new PictureBox();
            picLogo.Location = new Point(10, 10);
            picLogo.Size = new Size(130, 175);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.BackColor = Color.White;
            pnlHeader.Controls.Add(picLogo);

            // Cargar logo desde BD
            CargarLogoDesdeDB();

            // Datos del cliente — lado derecho del header
            int xLabel = 155;
            int xValor = 320;
            int yInicio = 15;
            int salto = 28;

            CrearFilaHeader(pnlHeader, "Ced/RUC:", ref lblCedRuc, ref lblCedRucVal, xLabel, xValor, yInicio + (salto * 0));
            CrearFilaHeader(pnlHeader, "Cliente:", ref lblCliente, ref lblClienteVal, xLabel, xValor, yInicio + (salto * 1));
            CrearFilaHeader(pnlHeader, "Dirección:", ref lblDireccion, ref lblDireccionVal, xLabel, xValor, yInicio + (salto * 2));
            CrearFilaHeader(pnlHeader, "Teléfono:", ref lblTelefono, ref lblTelefonoVal, xLabel, xValor, yInicio + (salto * 3));
            CrearFilaHeader(pnlHeader, "Club Delportal:", ref lblClubDelportal, ref lblClubDelportalVal, xLabel, xValor, yInicio + (salto * 4));
            CrearFilaHeader(pnlHeader, "BNF. del IVA:", ref lblBnfIva, ref lblBnfIvaVal, xLabel, xValor, yInicio + (salto * 5));

            int altoTotales = 120;
            int altoGrid = formHeight - 205 - altoTotales;

            gridItemsClte.Location = new Point(0, 205);
            gridItemsClte.Size = new Size(anchoIzq, altoGrid);
            gridItemsClte.Visible = true;
            gridItemsClte.BringToFront();

            // Configurar columnas como el PDF
            ConfigurarGridPDF();

            // Estilo header verde
            gridItemsClte.ViewCellFormatting += (s, e) =>
            {
                if (e.CellElement is GridHeaderCellElement)
                {
                    e.CellElement.DrawFill = true;
                    e.CellElement.GradientStyle = Telerik.WinControls.GradientStyles.Solid;
                    e.CellElement.BackColor = Color.FromArgb(0, 102, 51);
                    e.CellElement.ForeColor = Color.White;
                    e.CellElement.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            };
            // ── PANEL TOTALES ─────────────────────────────────────────
            ConstruirPanelTotales(anchoIzq, formHeight - altoTotales, altoTotales);

            // ── IMAGEN PROMO DERECHA ──────────────────────────────────
            pictureBox1.Location = new Point(anchoIzq, 0);
            pictureBox1.Size = new Size(anchoDer, formHeight);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Visible = true;
            pictureBox1.BringToFront();
            CargarImagenBanner();
        }


        private void CrearFilaHeader(Panel parent, string etiqueta,
            ref Label lblEtiqueta, ref Label lblValor,
            int xLabel, int xValor, int y)
        {
            lblEtiqueta = new Label();
            lblEtiqueta.Text = etiqueta;
            lblEtiqueta.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblEtiqueta.Location = new Point(xLabel, y);
            lblEtiqueta.Size = new Size(155, 24);
            lblEtiqueta.ForeColor = Color.FromArgb(0, 102, 51);
            parent.Controls.Add(lblEtiqueta);

            lblValor = new Label();
            lblValor.Text = "";
            lblValor.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblValor.Location = new Point(xValor, y);
            lblValor.Size = new Size(420, 24);
            lblValor.ForeColor = Color.Black;
            parent.Controls.Add(lblValor);
        }

        private void CargarLogoDesdeDB()
        {
            try
            {
                string ruta = ObtenerRutaImagenDesdeDB("LOGO_CLIENTE");

                if (!string.IsNullOrEmpty(ruta) && System.IO.File.Exists(ruta))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(ruta);
                    var ms = new System.IO.MemoryStream(bytes);
                    picLogo.Image = Image.FromStream(ms);
                }
                else
                {
                    // Si no encuentra la imagen, muestra texto de respaldo
                    picLogo.BackColor = Color.FromArgb(0, 102, 51);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(CargarLogoDesdeDB),
                    $"Error al cargar logo: {ex.Message}");
            }
        }

        private void ConfigurarGridPDF()
        {
            gridItemsClte.AutoGenerateColumns = false;
            gridItemsClte.Columns.Clear();

            var colNombre = new GridViewTextBoxColumn("Nombre")
            {
                HeaderText = "DESCRIPCIÓN",
                Width = 300,
                ReadOnly = true
            };

            var colCantidad = new GridViewTextBoxColumn("CantidadINEC")
            {
                HeaderText = "CANT.",
                Width = 80,
                ReadOnly = true,
                TextAlignment = ContentAlignment.MiddleCenter
            };

            var colPrecio = new GridViewDecimalColumn("Pvp")
            {
                HeaderText = "PRECIO",
                Width = 100,
                ReadOnly = true,
                FormatString = "{0:C}",
                TextAlignment = ContentAlignment.MiddleRight
            };

            var colTotal = new GridViewDecimalColumn("TotalPromoIVA")
            {
                HeaderText = "VALOR",
                Width = 100,
                ReadOnly = true,
                FormatString = "{0:C}",
                TextAlignment = ContentAlignment.MiddleRight
            };

            var colIdTemporal = new GridViewTextBoxColumn("IdTemporal")
            {
                HeaderText = "IdTemporal",
                IsVisible = false
            };

            gridItemsClte.Columns.Add(colNombre);
            gridItemsClte.Columns.Add(colCantidad);
            gridItemsClte.Columns.Add(colPrecio);
            gridItemsClte.Columns.Add(colTotal);
            gridItemsClte.Columns.Add(colIdTemporal);

            // Sorting por IdTemporal descendente
            gridItemsClte.EnableCustomSorting = true;
            gridItemsClte.CustomSorting += RadGridView1_CustomSorting;

            // Estilo de filas
            gridItemsClte.RowFormatting += (s, e) =>
            {
                e.RowElement.DrawFill = true;
                e.RowElement.GradientStyle = Telerik.WinControls.GradientStyles.Solid;
                e.RowElement.BackColor = Color.White;
                e.RowElement.ForeColor = Color.Black;
                e.RowElement.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            };
        }
        private void ConstruirPanelTotales(int ancho, int yInicio, int alto)
        {
            pnlTotales = new Panel();
            pnlTotales.Location = new Point(0, yInicio);
            pnlTotales.Size = new Size(ancho, alto);
            pnlTotales.BackColor = Color.White;
            pnlTotales.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlTotales);
            pnlTotales.BringToFront();

            int xTit = ancho - 280;
            int xVal = ancho - 100;
            int y = 8;
            int salto = 22;

            CrearFilaTotales("Valor:", ref lblValorTit, ref lblValorVal, xTit, xVal, y); y += salto;
            CrearFilaTotales("Descuento:", ref lblDescuentoTit, ref lblDescuentoVal, xTit, xVal, y); y += salto;
            CrearFilaTotales("IVA 15%:", ref lblIvaTit, ref lblIvaVal, xTit, xVal, y); y += salto + 4;

            // Fila TOTAL con fondo verde
            Panel pnlTotal = new Panel();
            pnlTotal.Location = new Point(xTit - 10, y);
            pnlTotal.Size = new Size(ancho - xTit + 10, 32);
            pnlTotal.BackColor = Color.FromArgb(0, 102, 51);
            pnlTotales.Controls.Add(pnlTotal);

            lblTotalTit = new Label();
            lblTotalTit.Text = "TOTAL:";
            lblTotalTit.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTotalTit.ForeColor = Color.White;
            lblTotalTit.Location = new Point(8, 4);
            lblTotalTit.Size = new Size(120, 24);
            lblTotalTit.TextAlign = ContentAlignment.MiddleLeft;
            pnlTotal.Controls.Add(lblTotalTit);

            lblTotalVal = new Label();
            lblTotalVal.Text = "$0.00";
            lblTotalVal.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTotalVal.ForeColor = Color.White;
            lblTotalVal.Location = new Point(pnlTotal.Width - 110, 4);
            lblTotalVal.Size = new Size(100, 24);
            lblTotalVal.TextAlign = ContentAlignment.MiddleRight;
            pnlTotal.Controls.Add(lblTotalVal);
        }

        private void CrearFilaTotales(string etiqueta,
            ref Label lblEtiq, ref Label lblVal, int xTit, int xVal, int y)
        {
            lblEtiq = new Label();
            lblEtiq.Text = etiqueta;
            lblEtiq.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblEtiq.ForeColor = Color.Black;
            lblEtiq.Location = new Point(xTit, y);
            lblEtiq.Size = new Size(150, 20);
            lblEtiq.TextAlign = ContentAlignment.MiddleLeft;
            pnlTotales.Controls.Add(lblEtiq);

            lblVal = new Label();
            lblVal.Text = "$0.00";
            lblVal.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblVal.ForeColor = Color.Black;
            lblVal.Location = new Point(xVal, y);
            lblVal.Size = new Size(90, 20);
            lblVal.TextAlign = ContentAlignment.MiddleRight;
            pnlTotales.Controls.Add(lblVal);
        }
        public void ActualizarTotales(decimal valor, decimal descuento, decimal iva, decimal total)
        {
            this.InvokeIfRequired(() =>
            {
                if (lblValorVal != null) lblValorVal.Text = valor.ToString("C");
                if (lblDescuentoVal != null) lblDescuentoVal.Text = descuento.ToString("C");
                if (lblIvaVal != null) lblIvaVal.Text = iva.ToString("C");
                if (lblTotalVal != null) lblTotalVal.Text = total.ToString("C");
            });
        }

        private void CargarImagenBanner()
        {
            string rutaImagen = ObtenerRutaImagenDesdeDB("BANNER_CLIENTE");

            if (!string.IsNullOrEmpty(rutaImagen) && System.IO.File.Exists(rutaImagen))
            {
                try
                {
                    byte[] imageBytes = System.IO.File.ReadAllBytes(rutaImagen);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte),
                        nameof(CargarImagenBanner),
                        $"Error al cargar banner: {ex.Message}");
                }
            }
        }


        private void InicializarWebViewPublicidad()
        {
            try
            {
                // Forzar IE11 para mejor compatibilidad
                var appName = System.IO.Path.GetFileName(Application.ExecutablePath);
                using (var regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
                {
                    regKey?.SetValue(appName, 11001, Microsoft.Win32.RegistryValueKind.DWord);
                }

                webViewAds = new System.Windows.Forms.WebBrowser();
                webViewAds.ScriptErrorsSuppressed = true;
                webViewAds.ScrollBarsEnabled = false;
                webViewAds.Location = new Point(0, 0);
                webViewAds.Size = this.ClientSize;
                webViewAds.Visible = false;
                webViewAds.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                                  | AnchorStyles.Left | AnchorStyles.Right;
                this.Controls.Add(webViewAds);
                webViewAds.BringToFront();

                // Apunta al servidor Node.js
                webViewAds.Navigate("http://localhost:3000/carrusel.html");
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(InicializarWebViewPublicidad),
                    $"Error: {ex.Message}");
            }
        }

        public void MostrarPublicidad()
        {
            this.InvokeIfRequired(() =>
            {
                try
                {
                    gridItemsClte.Visible = false;
                    pnlHeader?.Hide();
                    pnlTotales?.Hide();
                    pictureBox1.Visible = false;

                    if (webViewAds != null)
                    {
                        webViewAds.Size = this.ClientSize;
                        webViewAds.Visible = true;
                        webViewAds.BringToFront();
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte),
                        nameof(MostrarPublicidad), ex.Message);
                }
            });
        }

        public void OcultarPublicidad()
        {
            this.InvokeIfRequired(() =>
            {
                try
                {
                    if (webViewAds != null)
                        webViewAds.Visible = false;

                    gridItemsClte.Visible = true;
                    pnlHeader?.Show();
                    pnlTotales?.Show();
                    pictureBox1.Visible = true;
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte),
                        nameof(OcultarPublicidad), ex.Message);
                }
            });
        }

        // mostrar encuesta a final de la facturacion 
        public void MostrarEncuestaSatisfaccion(int segundos = 30, string numeroFactura = "", string identificacion = "", string nombreCliente = "")
        {
            _encuestaNumeroFactura = numeroFactura;
            _encuestaIdentificacion = identificacion;
            _encuestaNombreCliente = nombreCliente;
            this.InvokeIfRequired(() =>
            {
                try
                {
                    _segundosEncuesta = segundos;
                    // ── Ocultar todo el contenido normal ──
                    gridItemsClte.Visible = false;
                    pnlHeader?.Hide();
                    pnlTotales?.Hide();
                    pictureBox1.Visible = false;
                    if (webViewAds != null) webViewAds.Visible = false;

                    // ── Construir panel si no existe ──
                    if (pnlEncuesta == null)
                        ConstruirPanelEncuesta();

                    pnlEncuesta.Visible = true;
                    pnlEncuesta.BringToFront();
                    this.Controls.SetChildIndex(pnlEncuesta, 0);
                    this.Activate();
                    this.Focus();
                    pnlEncuesta.Focus();

                    // ── Timer para cerrar automáticamente ──
                    if (timerEncuesta == null)
                    {
                        timerEncuesta = new System.Windows.Forms.Timer();
                        timerEncuesta.Tick += TimerEncuesta_Tick;
                    }
                    timerEncuesta.Interval = segundos * 1000;
                    timerEncuesta.Start();
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte),
                        nameof(MostrarEncuestaSatisfaccion),
                        ex.Message);
                }
            });
        }

        private void TimerEncuesta_Tick(object sender, EventArgs e)
        {
            timerEncuesta.Stop();
            OcultarEncuesta();
        }

        private void OcultarEncuesta()
        {
            this.InvokeIfRequired(() =>
            {
                if (pnlEncuesta != null) pnlEncuesta.Visible = false;

                // ── Limpiar y volver a pantalla normal vacía ──
                gridItemsClte.DataSource = null;
                gridItemsClte.Visible = true;
                pnlHeader?.Show();
                pnlTotales?.Show();
                pictureBox1.Visible = true;

                // Limpiar datos cliente
                ActualizarDatosCliente(null);

                // Limpiar totales
                ActualizarTotales(0, 0, 0, 0);
            });
        }

        private void ConstruirPanelEncuesta()
        {
            pnlEncuesta = new Panel();
            pnlEncuesta.Size = this.ClientSize;
            pnlEncuesta.Location = new Point(0, 0);
            pnlEncuesta.BackColor = Color.White;
            pnlEncuesta.Enabled = true;
            this.Controls.Add(pnlEncuesta);
            pnlEncuesta.BringToFront();

            pnlEncuesta.MouseDown += (s, ev) =>
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Info,
                    nameof(frmMainTouchClte), "PanelEncuestaMouseDown",
                    $"Click en panel X={ev.X} Y={ev.Y}");
            };

            int cx = pnlEncuesta.Width / 2;

            // ── Banner superior ──
            var picBanner = new PictureBox();
            picBanner.Size = new Size(pnlEncuesta.Width, 120);
            picBanner.Location = new Point(0, 0);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.BackColor = Color.FromArgb(0, 102, 51);
            CargarImagenEnPictureBox(picBanner, "BANNER_SUPERIOR_CLIENTE");
            pnlEncuesta.Controls.Add(picBanner);

            // ── Línea verde ──
            var pnlLinea = new Panel();
            pnlLinea.Size = new Size(pnlEncuesta.Width, 6);
            pnlLinea.Location = new Point(0, 120);
            pnlLinea.BackColor = Color.FromArgb(0, 102, 51);
            pnlEncuesta.Controls.Add(pnlLinea);

            // ── Título ──
            var lblTitulo = new Label();
            lblTitulo.Text = "GRACIAS POR SU COMPRA";
            lblTitulo.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(0, 102, 51);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.Size = new Size(pnlEncuesta.Width - 40, 70);
            lblTitulo.Location = new Point(20, 135);
            lblTitulo.Enabled = false; // no bloquea clicks
            pnlEncuesta.Controls.Add(lblTitulo);

            // ── Línea separadora gris ──
            var pnlLineaSep = new Panel();
            pnlLineaSep.Size = new Size(600, 2);
            pnlLineaSep.Location = new Point(cx - 300, 210);
            pnlLineaSep.BackColor = Color.FromArgb(200, 200, 200);
            pnlEncuesta.Controls.Add(pnlLineaSep);

            // ── Subtítulo ──
            var lblSub1 = new Label();
            lblSub1.Text = "Tu opinión nos ayuda a mejorar nuestro servicio";
            lblSub1.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            lblSub1.ForeColor = Color.FromArgb(80, 80, 80);
            lblSub1.TextAlign = ContentAlignment.MiddleCenter;
            lblSub1.Size = new Size(pnlEncuesta.Width - 40, 40);
            lblSub1.Location = new Point(20, 225);
            lblSub1.Enabled = false;
            pnlEncuesta.Controls.Add(lblSub1);

            // ── Pregunta ──
            var lblPregunta = new Label();
            lblPregunta.Text = "¿Qué tan satisfecho/a estás con la atención recibida?";
            lblPregunta.Font = new Font("Segoe UI", 15, FontStyle.Bold | FontStyle.Italic);
            lblPregunta.ForeColor = Color.Black;
            lblPregunta.TextAlign = ContentAlignment.MiddleCenter;
            lblPregunta.Size = new Size(pnlEncuesta.Width - 40, 45);
            lblPregunta.Location = new Point(20, 270);
            lblPregunta.Enabled = false;
            pnlEncuesta.Controls.Add(lblPregunta);

            // ── Definición de emojis ──
            var emojis = new[]
            {
        new { Color = Color.FromArgb(200, 50,  50),  Texto = "Muy\ninsatisfecho", Valor = 1, Feliz = false, Neutral = false },
        new { Color = Color.FromArgb(220, 130, 30),  Texto = "Insatisfecho",      Valor = 2, Feliz = false, Neutral = false },
        new { Color = Color.FromArgb(220, 200, 30),  Texto = "Neutral",           Valor = 3, Feliz = false, Neutral = true  },
        new { Color = Color.FromArgb(140, 190, 60),  Texto = "Satisfecho",        Valor = 4, Feliz = true,  Neutral = false },
        new { Color = Color.FromArgb(60,  160, 50),  Texto = "Muy\nsatisfecho",   Valor = 5, Feliz = true,  Neutral = false },
    };

            int emojiSize = 120;
            int espacioEntre = 40;
            int totalAncho = emojis.Length * emojiSize + (emojis.Length - 1) * espacioEntre;
            int xInicio = cx - totalAncho / 2;
            int yEmoji = 335;

            foreach (var emoji in emojis)
            {
                int v = emoji.Valor;
                string t = emoji.Texto;
                bool feliz = emoji.Feliz;
                bool neutral = emoji.Neutral;
                Color col = emoji.Color;

                // ── Dibujar bitmap del emoji ──
                var bmp = new Bitmap(emojiSize, emojiSize);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.White);

                    // Círculo
                    g.FillEllipse(new SolidBrush(col), 2, 2, emojiSize - 4, emojiSize - 4);

                    // Ojos
                    int eyeY = emojiSize / 2 - 20;
                    g.FillEllipse(Brushes.White, emojiSize / 2 - 24, eyeY, 14, 14);
                    g.FillEllipse(Brushes.White, emojiSize / 2 + 10, eyeY, 14, 14);

                    // Boca
                    using (var pen = new Pen(Color.White, 5))
                    {
                        int mouthX = emojiSize / 2 - 18;
                        int mouthW = 36;
                        int mouthH = 18;

                        if (neutral)
                        {
                            // Línea recta
                            int mouthY = emojiSize / 2 + 18;
                            g.DrawLine(pen, emojiSize / 2 - 16, mouthY,
                                            emojiSize / 2 + 16, mouthY);
                        }
                        else if (feliz)
                        {
                            // Sonrisa hacia arriba: startAngle=0, sweepAngle=180 dibuja arco inferior = sonrisa
                            int mouthY = emojiSize / 2 + 6;
                            g.DrawArc(pen, mouthX, mouthY, mouthW, mouthH, 0, 180);
                        }
                        else
                        {
                            // Triste hacia abajo: startAngle=180, sweepAngle=180 dibuja arco superior = tristeza
                            int mouthY = emojiSize / 2 + 14;
                            g.DrawArc(pen, mouthX, mouthY, mouthW, mouthH, 180, 180);
                        }
                    }
                }

                // ── PictureBox con el emoji ──
                var pic = new PictureBox();
                pic.Size = new Size(emojiSize, emojiSize);
                pic.Location = new Point(xInicio, yEmoji);
                pic.BackColor = Color.White;
                pic.Cursor = Cursors.Hand;
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.Normal;

                int vCap = v;
                string tCap = t;

                pic.Click += (s, ev) =>
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        nameof(frmMainTouchClte), "EmojiClick",
                        $"Click valor={vCap}");
                    timerEncuesta?.Stop();
                    GrabarRespuestaEncuesta(vCap, tCap.Replace("\n", " "));
                    OcultarEncuesta();
                };

                pnlEncuesta.Controls.Add(pic);

                // ── Texto debajo ──
                var lbl = new Label();
                lbl.Text = t;
                lbl.Font = new Font("Segoe UI", 11, FontStyle.Regular);
                lbl.ForeColor = Color.Black;
                lbl.TextAlign = ContentAlignment.TopCenter;
                lbl.Size = new Size(emojiSize, 55);
                lbl.Location = new Point(xInicio, yEmoji + emojiSize + 6);
                lbl.Cursor = Cursors.Hand;
                lbl.Click += (s, ev) =>
                {
                    timerEncuesta?.Stop();
                    GrabarRespuestaEncuesta(vCap, tCap.Replace("\n", " "));
                    OcultarEncuesta();
                };

                pnlEncuesta.Controls.Add(lbl);
                xInicio += emojiSize + espacioEntre;
            }
        }

        private void CargarImagenEnPictureBox(PictureBox pic, string identificador)
        {
            try
            {
                string ruta = ObtenerRutaImagenDesdeDB(identificador);
                if (!string.IsNullOrEmpty(ruta) && System.IO.File.Exists(ruta))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(ruta);
                    pic.Image = Image.FromStream(new System.IO.MemoryStream(bytes));
                }
            }
            catch { }
        }

        private void GrabarRespuestaEncuesta(int calificacion, string descripcion)
        {
            try
            {
                using (var db = new POSEntities())
                {
                    string sql = @"INSERT INTO dbo.pos_encuesta_satisfaccion
                          (fecha, identificacion, nombre_cliente, calificacion, 
                           descripcion, establecimiento, pto_emision, cajero,
                           numero_factura)
                          VALUES (GETDATE(), @id, @nombre, @cal, 
                                  @desc, @est, @pto, @cajero, @factura)";

                    db.Database.ExecuteSqlCommand(sql,
                        new System.Data.SqlClient.SqlParameter("@id", _encuestaIdentificacion),
                        new System.Data.SqlClient.SqlParameter("@nombre", _encuestaNombreCliente),
                        new System.Data.SqlClient.SqlParameter("@cal", calificacion),
                        new System.Data.SqlClient.SqlParameter("@desc", descripcion),
                        new System.Data.SqlClient.SqlParameter("@est", Control.Common.GlobalParameters.Establecimiento ?? ""),
                        new System.Data.SqlClient.SqlParameter("@pto", Control.Common.GlobalParameters.PuntoEmision ?? ""),
                        new System.Data.SqlClient.SqlParameter("@cajero", Control.Common.GlobalParameters.UsuarioNombre ?? ""),
                        new System.Data.SqlClient.SqlParameter("@factura", _encuestaNumeroFactura)
                    );

                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        nameof(frmMainTouchClte),
                        nameof(GrabarRespuestaEncuesta),
                        $"Encuesta grabada: Cal={calificacion} | Factura={_encuestaNumeroFactura} | Cliente={_encuestaIdentificacion}");
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(GrabarRespuestaEncuesta),
                    $"Error grabando encuesta: {ex.Message}");
            }
        }
    }
}
