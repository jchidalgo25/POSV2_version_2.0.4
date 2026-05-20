using Microsoft.Win32;
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
using Telerik.WinControls;
using Telerik.WinControls.UI;



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

        private Panel pnlEncuestaNueva;
        private PreguntaEncuesta _preguntaActual;
        private string _encuestaNuevaNumeroFactura = "";
        private string _encuestaNuevaIdentificacion = "";
        private string _encuestaNuevaНombreCliente = "";
        private int _ultimaEncuestaId = 0;
        private List<PreguntaEncuesta> _preguntasPendientes = new List<PreguntaEncuesta>();
        private bool _encuestaNuevaCargada = false;
        public bool EncuestaNuevaCargada
        {
            get { return _encuestaNuevaCargada; }
            set { _encuestaNuevaCargada = value; }
        }
        private bool _encuestaCaritasContestada = false;
        string est = Control.Common.GlobalParameters.Establecimiento ?? "";
        private System.Windows.Forms.Timer _timerRecargarCarrusel;
        private string _ultimoUpdatedAt = "";
        public event EventHandler EncuestaTerminada;
        public event EventHandler<PreguntaEncuesta> PreguntaAbiertaRecibida;
        private string _ultimoUpdatedAtBanner = "";
        private System.Windows.Forms.Timer _timerRecargarBanner;
        private System.Windows.Forms.WebBrowser _webBanner;

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
            _timerRecargarCarrusel?.Stop();
            _timerRecargarCarrusel?.Dispose();
            _timerRecargarBanner?.Stop();   
            _timerRecargarBanner?.Dispose();
            webViewAds?.Dispose();
            _webBanner?.Dispose();
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
            MostrarPublicidad();
            InicializarTimerCarrusel();
            InicializarTimerBanner();
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
            PreguntaAbiertaRecibida = null; // ← limpiar suscriptores
            base.OnFormClosed(e);
        }



        private BindingList<Producto> _listaActual = null;

        private void FacturaService_ProductosActualizados(object sender, BindingList<Producto> productos)
        {
            this.InvokeIfRequired(() =>
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Info,
                    nameof(frmMainTouchClte),
                    nameof(FacturaService_ProductosActualizados),
                    $"LLAMADA | StackTrace:{new System.Diagnostics.StackTrace().ToString().Substring(0, 300)}");

                if (_factura == null)
                    _factura = new Factura();

                _factura.Productos = productos;

                if (_listaActual != productos)
                {
                    _listaActual = productos;
                    gridItemsClte.DataSource = productos;
                }
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
                _listaActual = null;
                return;
            }

            _factura = factura;

            if (_listaActual != factura.Productos)
            {
                _listaActual = factura.Productos;
                gridItemsClte.DataSource = factura.Productos;
            }
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
                if (lblClienteVal != null) lblClienteVal.Text = "";
                if (lblClubDelportalVal != null) lblClubDelportalVal.Text = "";
                if (lblBnfIvaVal != null) lblBnfIvaVal.Text = "";
                return;
            }

            //lblCedRucVal.Text = cliente.Identificacion;
            lblClienteVal.Text = cliente.Identificacion == "9999999999999"
                                       ? "CONSUMIDOR FINAL"
                                       : cliente.NombreCliente;
            //lblDireccionVal.Text = cliente.DireccionCliente;
            //lblTelefonoVal.Text = cliente.TelefonoCliente;
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
                    var saldoCompraGratis = cliente.SaldoCompraGratis;
                    Screen screenClte = Screen.FromControl(this);

                    var t = new System.Threading.Thread(() =>
                    {
                        try
                        {
                            var parametros = new List<ParametrosMensajes>
                {
                    new ParametrosMensajes() { codigo = "[NombreApellido]", valor = nombreCliente },
                    new ParametrosMensajes() { codigo = "[SaldoMonedero]",  valor = saldoApp.ToString("N2") },
                    new ParametrosMensajes() {codigo = "[CompraGratis]", valor = cliente.SaldoCompraGratis > 0 ? cliente.SaldoCompraGratis.ToString("N2"): "No disponible"}
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
            pnlHeader.Size = new Size(anchoIzq, 120);
            pnlHeader.BackColor = Color.White;
            pnlHeader.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlHeader);
            pnlHeader.BringToFront();

            picLogo = new PictureBox();
            picLogo.Location = new Point(10, 10);
            picLogo.Size = new Size(90, 100); 
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.BackColor = Color.White;
            pnlHeader.Controls.Add(picLogo);

            // Cargar logo desde BD
            CargarLogoDesdeDB();

            // Datos del cliente — lado derecho del header
            int xLabel = 110;
            int xValor = 320;
            int yInicio = 16;
            int salto = 30;
            Font fuenteGrande = new Font("Segoe UI", 13, FontStyle.Regular);

            //CrearFilaHeader(pnlHeader, "Ced/RUC:", ref lblCedRuc, ref lblCedRucVal, xLabel, xValor, yInicio + (salto * 0));
            CrearFilaHeader(pnlHeader, "Cliente:", ref lblCliente, ref lblClienteVal, xLabel, xValor, yInicio);
            //CrearFilaHeader(pnlHeader, "Dirección:", ref lblDireccion, ref lblDireccionVal, xLabel, xValor, yInicio + (salto * 2));
            //CrearFilaHeader(pnlHeader, "Teléfono:", ref lblTelefono, ref lblTelefonoVal, xLabel, xValor, yInicio + (salto * 3));
            CrearFilaHeader(pnlHeader, "Club Delportal:", ref lblClubDelportal, ref lblClubDelportalVal, xLabel, xValor, yInicio + salto);
            CrearFilaHeader(pnlHeader, "Bnf. Dev. IVA:", ref lblBnfIva, ref lblBnfIvaVal, xLabel, xValor, yInicio + (salto * 2));

            lblClienteVal.Font = fuenteGrande;
            lblClubDelportalVal.Font = fuenteGrande;
            lblBnfIvaVal.Font = fuenteGrande;

            lblCliente.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblClubDelportal.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblBnfIva.Font = new Font("Segoe UI", 13, FontStyle.Bold);


            int altoTotales = 120;
            int altoGrid = formHeight - 125 - altoTotales;

            gridItemsClte.Location = new Point(0, 125);
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
            // ── BANNER DERECHO como WebBrowser ───────────────────────
            pictureBox1.Visible = false; // ocultar el pictureBox original

            _webBanner = new System.Windows.Forms.WebBrowser();
            _webBanner.ScriptErrorsSuppressed = true;
            _webBanner.ScrollBarsEnabled = false;
            _webBanner.Location = new Point(anchoIzq, 0);
            _webBanner.Size = new Size(anchoDer, formHeight);
            _webBanner.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(_webBanner);
            _webBanner.BringToFront();
            _webBanner.Navigate($"http://192.168.127.220:3030/banner.html?est={est}");
            // ─────────────────────────────────────────────────────────
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
                Width = 265,
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

            // ── Sorting descendente igual que pantalla principal ──
            gridItemsClte.EnableCustomSorting = true;
            gridItemsClte.CustomSorting += (s, e) =>
            {
                try
                {
                    Int64 row1 = Convert.ToInt64(e.Row1.Cells["IdTemporal"].Value);
                    Int64 row2 = Convert.ToInt64(e.Row2.Cells["IdTemporal"].Value);
                    e.SortResult = row2.CompareTo(row1);
                    e.Handled = true;
                }
                catch
                {
                    e.SortResult = 0;
                    e.Handled = true;
                }
            };
            if (gridItemsClte.Columns.Contains("IdTemporal"))
                gridItemsClte.Columns["IdTemporal"].SortOrder = RadSortOrder.Descending;
            // ─────────────────────────────────────────────────────

            gridItemsClte.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            gridItemsClte.ReadOnly = true;
            gridItemsClte.MultiSelect = false;
            gridItemsClte.AllowEditRow = false;
            gridItemsClte.SelectionMode = GridViewSelectionMode.FullRowSelect;

            try { gridItemsClte.TableElement.HScrollBar.Visibility = ElementVisibility.Collapsed; } catch { }

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
            lblEtiq.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblEtiq.ForeColor = Color.Black;
            lblEtiq.Location = new Point(xTit, y);
            lblEtiq.Size = new Size(150, 20);
            lblEtiq.TextAlign = ContentAlignment.MiddleLeft;
            pnlTotales.Controls.Add(lblEtiq);

            lblVal = new Label();
            lblVal.Text = "$0.00";
            lblVal.Font = new Font("Segoe UI", 9, FontStyle.Bold);
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

                using (var key2 = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings", true))
                {
                    key2?.SetValue("MaxAgeUI", 0, RegistryValueKind.DWord);
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
                webViewAds.Navigate($"http://192.168.127.220:3030/carrusel.html?est={est}");
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
                        // ── Siempre recargar con timestamp para evitar caché ──
                        string est = Control.Common.GlobalParameters.Establecimiento ?? "";
                        string url = $"http://192.168.127.220:3030/carrusel.html?est={est}&t={DateTime.Now.Ticks}";
                        webViewAds.Navigate(url);
                        // ─────────────────────────────────────────────────────
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
                    foreach (System.Windows.Forms.Control ctrl in this.Controls)
                    {
                        if (ctrl is System.Windows.Forms.WebBrowser && ctrl != webViewAds)
                            ctrl.Visible = true;
                    }
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

        

        private void OcultarEncuesta()
        {
            this.InvokeIfRequired(() =>
            {
                // ── Ocultar caritas ──
                if (pnlEncuesta != null) pnlEncuesta.Visible = false;
                timerEncuesta?.Stop();

                // ── Limpiar pantalla ──
                gridItemsClte.DataSource = null;
                _listaActual = null;
                gridItemsClte.Visible = false;
                pnlHeader?.Hide();
                pnlTotales?.Hide();
                pictureBox1.Visible = false;
                if (webViewAds != null) webViewAds.Visible = false;

                ActualizarDatosCliente(null);
                ActualizarTotales(0, 0, 0, 0);

                // ── Ir a encuesta nueva ──
                MostrarEncuestaNueva(
                    _encuestaNumeroFactura,
                    _encuestaIdentificacion,
                    _encuestaNombreCliente);
            });
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

    
        private PreguntaEncuesta ObtenerPreguntaEncuesta(string establecimiento)
        {
            try
            {
                // Si ya hay preguntas pendientes en cola, devolver la siguiente
                if (_preguntasPendientes != null && _preguntasPendientes.Any())
                {
                    var siguiente = _preguntasPendientes.First();
                    _preguntasPendientes.RemoveAt(0);
                    return siguiente;
                }

                // Si la cola está vacía no hay más preguntas
                return null;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(ObtenerPreguntaEncuesta),
                    $"Error: {ex.Message}");
                return null;
            }
        }

        private void CargarPreguntasEncuesta(string establecimiento)
        {
            try
            {
                _preguntasPendientes = new List<PreguntaEncuesta>();
                string est = establecimiento ?? "";

                using (var db = new POSEntities())
                {
                    // Cargar todas las FIJAS en orden
                    var fijas = db.Database.SqlQuery<PreguntaEncuesta>(@"
                SELECT id, pregunta, tipo, opciones, seleccion_multiple
                FROM dbo.pos_encuesta_pregunta
                WHERE activo = 1
                AND modo = 'FIJA'
                AND (
                        establecimiento IS NULL 
                        OR establecimiento = '' 
                        OR ',' + establecimiento + ',' LIKE '%,' + @est + ',%'
                    )
                ORDER BY orden ASC",
                        new System.Data.SqlClient.SqlParameter("@est", est))
                        .ToList();

                    _preguntasPendientes.AddRange(fijas);

                    // Agregar UNA aleatoria si existen
                    var aleatoria = db.Database.SqlQuery<PreguntaEncuesta>(@"
                SELECT TOP 1 id, pregunta, tipo, opciones, seleccion_multiple
                FROM dbo.pos_encuesta_pregunta
                WHERE activo = 1
                AND modo = 'ALEATORIA'
                AND (establecimiento = @est OR establecimiento IS NULL OR establecimiento = '')
                ORDER BY NEWID()",
                        new System.Data.SqlClient.SqlParameter("@est", est))
                        .FirstOrDefault();

                    if (aleatoria != null)
                        _preguntasPendientes.Add(aleatoria);

                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        nameof(frmMainTouchClte),
                        nameof(CargarPreguntasEncuesta),
                        $"Preguntas cargadas: {_preguntasPendientes.Count} | FIJAS:{fijas.Count}");
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(CargarPreguntasEncuesta),
                    $"Error: {ex.Message}");
            }
        }

        public void MostrarEncuestaNueva(string numeroFactura, string identificacion, string nombreCliente)
        {
            _encuestaNuevaNumeroFactura = numeroFactura;
            _encuestaNuevaIdentificacion = identificacion;
            _encuestaNuevaНombreCliente = nombreCliente;

            this.InvokeIfRequired(() =>
            {
                try
                {
                    if (pnlEncuestaNueva != null)
                    {
                        // Detener cualquier proceso interno
                        pnlEncuestaNueva.Visible = false;

                        // Quitarlo físicamente del formulario
                        if (this.Controls.Contains(pnlEncuestaNueva))
                        {
                            this.Controls.Remove(pnlEncuestaNueva);
                        }

                        // Limpiar sus controles internos (las etiquetas de "Espere")
                        pnlEncuestaNueva.Controls.Clear();

                        // Liberar la memoria RAM que usa el objeto
                        pnlEncuestaNueva.Dispose();

                        // Anular la variable
                        pnlEncuestaNueva = null;
                    }

                    // 2. RESETEAR EL BANNER / NAVEGADOR
                    foreach (System.Windows.Forms.Control ctrl in this.Controls)
                    {
                        if (ctrl is System.Windows.Forms.WebBrowser || ctrl.Name.Contains("web"))
                        {
                            ctrl.Visible = true;
                            ctrl.BringToFront(); // Traer la publicidad al frente
                        }
                    }
                    // ── Limpiar grid y datos cliente ──
                    gridItemsClte.DataSource = null;
                    _listaActual = null;
                    ActualizarDatosCliente(null);
                    ActualizarTotales(0, 0, 0, 0);
                    // ──────────────────────────────────

                    // ── Cargar preguntas solo UNA vez por transacción ──
                    if (!_encuestaNuevaCargada)
                    {
                        _encuestaNuevaCargada = true;
                        CargarPreguntasEncuesta(Control.Common.GlobalParameters.Establecimiento);
                    }

                    // Obtener siguiente pregunta de la cola
                    _preguntaActual = ObtenerPreguntaEncuesta(
                        Control.Common.GlobalParameters.Establecimiento);

                    // Si no hay más preguntas ir a publicidad y resetear para próxima transacción
                    if (_preguntaActual == null)
                    {
                        _encuestaNuevaCargada = false;
                        MostrarPublicidad();
                        EncuestaTerminada?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    // Ocultar todo
                    gridItemsClte.Visible = false;
                    pnlHeader?.Hide();
                    pnlTotales?.Hide();
                    pictureBox1.Visible = false;
                    if (webViewAds != null) webViewAds.Visible = false;
                    if (pnlEncuesta != null) pnlEncuesta.Visible = false;

                    // Construir panel si no existe o reconstruir
                    if (pnlEncuestaNueva != null)
                    {
                        pnlEncuestaNueva.Controls.Clear();
                        this.Controls.Remove(pnlEncuestaNueva);
                        pnlEncuestaNueva = null;
                    }

                    ConstruirPanelEncuestaNueva(_preguntaActual);

                    pnlEncuestaNueva.Visible = true;
                    pnlEncuestaNueva.BringToFront();
                    this.Controls.SetChildIndex(pnlEncuestaNueva, 0);
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte),
                        nameof(MostrarEncuestaNueva),
                        ex.Message);
                    MostrarPublicidad();
                }
            });
        }
        private void ConstruirPanelEncuestaNueva(PreguntaEncuesta pregunta)
        {
            if (pnlEncuestaNueva != null)
            {
                this.Controls.Remove(pnlEncuestaNueva);
                pnlEncuestaNueva.Dispose();
            }
            pnlEncuestaNueva = new Panel();
            pnlEncuestaNueva.Size = this.ClientSize;
            pnlEncuestaNueva.Location = new Point(0, 0);
            pnlEncuestaNueva.BackColor = Color.White;
            this.Controls.Add(pnlEncuestaNueva);

            int cx = pnlEncuestaNueva.Width / 2;
            int cy = pnlEncuestaNueva.Height / 2;

            // ── Banner superior ──
            var picBanner = new PictureBox();
            picBanner.Size = new Size(pnlEncuestaNueva.Width, 120);
            picBanner.Location = new Point(0, 0);
            picBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            picBanner.BackColor = Color.FromArgb(0, 102, 51);
            CargarImagenEnPictureBox(picBanner, "BANNER_SUPERIOR_CLIENTE");
            pnlEncuestaNueva.Controls.Add(picBanner);

            // ── Línea verde ──
            var pnlLinea = new Panel();
            pnlLinea.Size = new Size(pnlEncuestaNueva.Width, 6);
            pnlLinea.Location = new Point(0, 120);
            pnlLinea.BackColor = Color.FromArgb(0, 102, 51);
            pnlEncuestaNueva.Controls.Add(pnlLinea);

            // ── CALCULAR POSICIÓN CENTRADA VERTICALMENTE ──
            // Bajamos el contenido para que esté más cerca del centro de la pantalla
            int yTitulo = (int)(pnlEncuestaNueva.Height * 0.22);  // ← subimos un poco (era 0.28)
            int yPregunta = yTitulo + 75;
            int ySeparador = yPregunta + 150;  // ← más espacio para 2 líneas (era 110)
            int yContenido = ySeparador + 30;

            // ── Título MÁS GRANDE y centrado ──
            var lblTitulo = new Label();
            lblTitulo.Text = "Tu opinión es importante";
            lblTitulo.Font = new Font("Segoe UI", 24, FontStyle.Regular);
            lblTitulo.ForeColor = Color.FromArgb(0, 102, 51);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.Size = new Size(pnlEncuestaNueva.Width - 40, 75);
            lblTitulo.Location = new Point(20, yTitulo);
            pnlEncuestaNueva.Controls.Add(lblTitulo);

            // ── Pregunta: protagonista visual de la pantalla ──
            var lblPregunta = new Label();
            lblPregunta.Text = pregunta.pregunta;
            lblPregunta.ForeColor = Color.Black;
            lblPregunta.TextAlign = ContentAlignment.MiddleCenter;
            lblPregunta.AutoSize = false;
            lblPregunta.Size = new Size(pnlEncuestaNueva.Width - 80, 140); // ← más alto (era 100) para 2 líneas
            lblPregunta.Location = new Point(40, yPregunta);

            // ── Ajustar tamaño de fuente automáticamente según largo del texto ──
            int largoTexto = pregunta.pregunta?.Length ?? 0;
            float tamFuente;
            if (largoTexto <= 40)
                tamFuente = 32f;          // Texto corto → fuente grande
            else if (largoTexto <= 70)
                tamFuente = 26f;          // Texto medio
            else if (largoTexto <= 100)
                tamFuente = 22f;          // Texto largo
            else
                tamFuente = 18f;          // Texto muy largo

            lblPregunta.Font = new Font("Segoe UI", tamFuente, FontStyle.Bold);
            pnlEncuestaNueva.Controls.Add(lblPregunta);

            // ── Separador ──
            var sep = new Panel();
            sep.Size = new Size(700, 2);
            sep.Location = new Point(cx - 350, ySeparador);
            sep.BackColor = Color.FromArgb(220, 220, 220);
            pnlEncuestaNueva.Controls.Add(sep);

            // ── Contenido según tipo ──
            switch (pregunta.tipo?.ToUpper() ?? "EMOJIS")
            {
                case "EMOJIS":
                    ConstruirRespuestaEmojis(pnlEncuestaNueva, cx, yContenido);
                    break;
                case "CERRADA":
                    ConstruirRespuestaCerrada(pnlEncuestaNueva, cx, yContenido);
                    break;
                case "NUMERICA":
                    ConstruirRespuestaНumerica(pnlEncuestaNueva, cx, yContenido);
                    break;
                case "MULTIPLE":
                    ConstruirRespuestaMultiple(pnlEncuestaNueva, cx, yContenido, pregunta);
                    break;
                case "ABIERTA":
                    MostrarMensajeEsperaAbierta(pnlEncuestaNueva);
                    PreguntaAbiertaRecibida?.Invoke(this, _preguntaActual);
                    break;
                default:
                    ConstruirRespuestaEmojis(pnlEncuestaNueva, cx, yContenido);
                    break;
            }
        }
        // ── EMOJIS ────────────────────────────────────────────────────
        private void ConstruirRespuestaEmojis(Panel panel, int cx, int yInicio)
        {
            var emojis = new[]
            {
        new { Color = Color.FromArgb(200,50,50),  Texto = "Muy\ninsatisfecho", Valor = 1, Feliz = false, Neutral = false },
        new { Color = Color.FromArgb(220,130,30), Texto = "Insatisfecho",      Valor = 2, Feliz = false, Neutral = false },
        new { Color = Color.FromArgb(220,200,30), Texto = "Neutral",           Valor = 3, Feliz = false, Neutral = true  },
        new { Color = Color.FromArgb(140,190,60), Texto = "Satisfecho",        Valor = 4, Feliz = true,  Neutral = false },
        new { Color = Color.FromArgb(60,160,50),  Texto = "Muy\nsatisfecho",   Valor = 5, Feliz = true,  Neutral = false },
    };

            int emojiSize = 110;
            int espacioEntre = 35;
            int totalAncho = emojis.Length * emojiSize + (emojis.Length - 1) * espacioEntre;
            int xInicio = cx - totalAncho / 2;

            foreach (var emoji in emojis)
            {
                int v = emoji.Valor;
                string t = emoji.Texto;
                Color col = emoji.Color;
                bool feliz = emoji.Feliz;
                bool neutral = emoji.Neutral;

                var bmp = new Bitmap(emojiSize, emojiSize);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.White);
                    g.FillEllipse(new SolidBrush(col), 2, 2, emojiSize - 4, emojiSize - 4);
                    int eyeY = emojiSize / 2 - 20;
                    g.FillEllipse(Brushes.White, emojiSize / 2 - 24, eyeY, 14, 14);
                    g.FillEllipse(Brushes.White, emojiSize / 2 + 10, eyeY, 14, 14);
                    using (var pen = new Pen(Color.White, 5))
                    {
                        int mouthX = emojiSize / 2 - 18;
                        int mouthW = 36; int mouthH = 18;
                        if (neutral) g.DrawLine(pen, emojiSize / 2 - 16, emojiSize / 2 + 18, emojiSize / 2 + 16, emojiSize / 2 + 18);
                        else if (feliz) g.DrawArc(pen, mouthX, emojiSize / 2 + 6, mouthW, mouthH, 0, 180);
                        else g.DrawArc(pen, mouthX, emojiSize / 2 + 14, mouthW, mouthH, 180, 180);
                    }
                }

                var pic = new PictureBox();
                pic.Size = new Size(emojiSize, emojiSize);
                pic.Location = new Point(xInicio, yInicio);
                pic.BackColor = Color.White;
                pic.Cursor = Cursors.Hand;
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.Normal;

                int vCap = v; string tCap = t;
                pic.Click += (s, ev) => GrabarRespuestaNueva(vCap, tCap.Replace("\n", " "), null);
                panel.Controls.Add(pic);

                var lbl = new Label();
                lbl.Text = t;
                lbl.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                lbl.ForeColor = Color.Black;
                lbl.TextAlign = ContentAlignment.TopCenter;
                lbl.Size = new Size(emojiSize, 50);
                lbl.Location = new Point(xInicio, yInicio + emojiSize + 6);
                lbl.Cursor = Cursors.Hand;
                lbl.Click += (s, ev) => GrabarRespuestaNueva(vCap, tCap.Replace("\n", " "), null);
                panel.Controls.Add(lbl);

                xInicio += emojiSize + espacioEntre;
            }
        }

        // ── SI / NO ───────────────────────────────────────────────────
        private void ConstruirRespuestaCerrada(Panel panel, int cx, int yInicio)
        {
            var opciones = new[]
            {
        new { Texto = "✅  SÍ", Valor = 1, BgColor = Color.FromArgb(0,102,51), FgColor = Color.White },
        new { Texto = "❌  NO", Valor = 0, BgColor = Color.FromArgb(200,50,50), FgColor = Color.White }
    };

            int btnW = 220; int btnH = 90; int espacio = 40;
            int totalAncho = opciones.Length * btnW + (opciones.Length - 1) * espacio;
            int xInicio = cx - totalAncho / 2;

            foreach (var op in opciones)
            {
                int vCap = op.Valor; string tCap = op.Texto;
                var btn = new Button();
                btn.Text = op.Texto;
                btn.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                btn.ForeColor = op.FgColor;
                btn.BackColor = op.BgColor;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Size = new Size(btnW, btnH);
                btn.Location = new Point(xInicio, yInicio + 30);
                btn.Cursor = Cursors.Hand;
                btn.Click += (s, ev) => GrabarRespuestaNueva(vCap, tCap.Replace("✅  ", "").Replace("❌  ", ""), null);
                panel.Controls.Add(btn);
                xInicio += btnW + espacio;
            }
        }

        // ── NUMERICA ──────────────────────────────────────────────────
        private void ConstruirRespuestaНumerica(Panel panel, int cx, int yInicio)
        {
            // 1. Configuraciones de tamaño
            int btnSize = 90;
            int espacio = 15;
            int totalBotones = 5;
            int totalAncho = (totalBotones * btnSize) + ((totalBotones - 1) * espacio);

            // El xInicio debe ser el centro del panel menos la mitad del ancho total del grupo
            int xInicio = cx - (totalAncho / 2);

            // 2. Paleta de colores ajustada (Rojo -> Naranja -> Amarillo -> Verde claro -> Verde fuerte)
            var colores = new[]
            {
                Color.FromArgb(200, 50, 50),   // 1. Rojo (Nunca)
                Color.FromArgb(230, 125, 35),  // 2. Naranja
                Color.FromArgb(235, 200, 30),  // 3. Amarillo (A veces)
                Color.FromArgb(140, 190, 60),  // 4. Verde Lima
                Color.FromArgb(0, 130, 40)     // 5. Verde Fuerte (Siempre)
            };

            // Limpiamos controles previos si es necesario para evitar solapamientos
            // panel.Controls.Clear(); 

            for (int i = 1; i <= totalBotones; i++)
            {
                int vCap = i;
                var btn = new Button();
                btn.Text = i.ToString();
                btn.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                btn.ForeColor = Color.White;
                btn.BackColor = colores[i - 1];
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Size = new Size(btnSize, btnSize);

                // Posicionamiento dinámico centrado
                btn.Location = new Point(xInicio, yInicio + 20);
                btn.Cursor = Cursors.Hand;
                btn.Click += (s, ev) => GrabarRespuestaNueva(vCap, vCap.ToString(), null);

                panel.Controls.Add(btn);

                // Movemos el xInicio para el siguiente botón
                xInicio += btnSize + espacio;
            }

            // --- Etiquetas ---
            int labelY = yInicio + 130;
            int labelWidth = 200; // 200 es suficiente y más seguro

            var lblNunca = new Label();
            lblNunca.Text = "Nunca";
            lblNunca.ForeColor = Color.Black; // Aseguramos color
            lblNunca.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblNunca.Location = new Point(cx - (totalAncho / 2), labelY);
            lblNunca.Size = new Size(labelWidth, 40);
            panel.Controls.Add(lblNunca);
            lblNunca.BringToFront(); // Aseguramos visibilidad

            var lblAVeces = new Label();
            lblAVeces.Text = "A veces";
            lblAVeces.ForeColor = Color.Black; // Aseguramos color
            lblAVeces.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblAVeces.TextAlign = ContentAlignment.MiddleCenter;
            lblAVeces.Location = new Point(cx - (labelWidth / 2), labelY);
            lblAVeces.Size = new Size(labelWidth, 40);
            panel.Controls.Add(lblAVeces);
            lblAVeces.BringToFront(); // <--- CRÍTICO

            var lblSiempre = new Label();
            lblSiempre.Text = "Siempre";
            lblSiempre.ForeColor = Color.Black; // Aseguramos color
            lblSiempre.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblSiempre.TextAlign = ContentAlignment.MiddleRight;
            lblSiempre.Location = new Point(cx + (totalAncho / 2) - labelWidth, labelY);
            lblSiempre.Size = new Size(labelWidth, 40);
            panel.Controls.Add(lblSiempre);
            lblSiempre.BringToFront(); // Aseguramos visibilidad
        }

        // ── MULTIPLE ──────────────────────────────────────────────────
        private List<string> _seleccionesMultiple = new List<string>();

        private void ConstruirRespuestaMultiple(Panel panel, int cx, int yInicio, PreguntaEncuesta pregunta)
        {
            _seleccionesMultiple = new List<string>();

            List<string> opciones = new List<string>();
            try
            {
                if (!string.IsNullOrEmpty(pregunta.opciones))
                    opciones = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(pregunta.opciones);
            }
            catch { opciones = new List<string> { "Opción 1", "Opción 2" }; }

            // ── DETERMINAR NÚMERO DE COLUMNAS DINÁMICAMENTE ──
            // 1-2 opciones → 2 columnas
            // 3 opciones → 3 columnas
            // 4 opciones → 2 columnas (2x2)
            // 5-6 opciones → 3 columnas
            // 7+ opciones → 3 columnas (máximo)
            int cols;
            if (opciones.Count <= 2)
                cols = opciones.Count;
            else if (opciones.Count == 4)
                cols = 2;
            else if (opciones.Count == 3 || opciones.Count >= 5)
                cols = 3;
            else
                cols = 3;

            // Asegurar que no exceda 3 columnas (regla de marketing)
            if (cols > 3) cols = 3;

            int btnW = 260;
            int btnH = 70;
            int espacioH = 20;  // espacio horizontal entre botones
            int espacioV = 15;  // espacio vertical entre botones

            int totalAncho = cols * btnW + (cols - 1) * espacioH;
            int xInicio = cx - totalAncho / 2;
            int col = 0;
            int row = 0;

            var botonesOpciones = new List<Button>();

            foreach (var op in opciones)
            {
                string opCap = op;
                int x = xInicio + col * (btnW + espacioH);
                int y = yInicio + row * (btnH + espacioV);

                var btn = new Button();
                btn.Text = op;
                btn.Font = new Font("Segoe UI", 13, FontStyle.Regular);
                btn.ForeColor = Color.FromArgb(0, 102, 51);
                btn.BackColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Color.FromArgb(0, 102, 51);
                btn.FlatAppearance.BorderSize = 2;
                btn.Size = new Size(btnW, btnH);
                btn.Location = new Point(x, y);
                btn.Cursor = Cursors.Hand;
                btn.Tag = false;

                btn.Click += (s, ev) =>
                {
                    var b = s as Button;
                    bool seleccionado = !(bool)b.Tag;
                    if (!pregunta.seleccion_multiple)
                    {
                        foreach (var ob in botonesOpciones)
                        {
                            ob.Tag = false;
                            ob.BackColor = Color.White;
                            ob.ForeColor = Color.FromArgb(0, 102, 51);
                        }
                    }
                    b.Tag = seleccionado;
                    b.BackColor = seleccionado ? Color.FromArgb(0, 102, 51) : Color.White;
                    b.ForeColor = seleccionado ? Color.White : Color.FromArgb(0, 102, 51);

                    if (!pregunta.seleccion_multiple && seleccionado)
                    {
                        GrabarRespuestaNueva(1, opCap, null);
                    }
                };

                panel.Controls.Add(btn);
                botonesOpciones.Add(btn);

                col++;
                if (col >= cols) { col = 0; row++; }
            }

            // ── Calcular cuántas filas se usaron ──
            int filasUsadas = (int)Math.Ceiling((double)opciones.Count / cols);

            // Botón confirmar solo si es multiselección
            if (pregunta.seleccion_multiple)
            {
                int yBtn = yInicio + filasUsadas * (btnH + espacioV) + 30;
                var btnConfirmar = new Button();
                btnConfirmar.Text = "✓  CONFIRMAR";
                btnConfirmar.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                btnConfirmar.ForeColor = Color.White;
                btnConfirmar.BackColor = Color.FromArgb(0, 102, 51);
                btnConfirmar.FlatStyle = FlatStyle.Flat;
                btnConfirmar.FlatAppearance.BorderSize = 0;
                btnConfirmar.Size = new Size(300, 65);
                btnConfirmar.Location = new Point(cx - 150, yBtn);
                btnConfirmar.Cursor = Cursors.Hand;
                btnConfirmar.Click += (s, ev) =>
                {
                    var seleccionadas = botonesOpciones
                        .Where(b => (bool)b.Tag)
                        .Select(b => b.Text)
                        .ToList();
                    if (!seleccionadas.Any())
                    {
                        btnConfirmar.Text = "Selecciona al menos una opción";
                        var t = new System.Windows.Forms.Timer();
                        t.Interval = 1500;
                        t.Tick += (s2, e2) => { t.Stop(); btnConfirmar.Text = "✓  CONFIRMAR"; };
                        t.Start();
                        return;
                    }
                    string respuesta = string.Join(", ", seleccionadas);
                    GrabarRespuestaNueva(1, respuesta, null);
                };
                panel.Controls.Add(btnConfirmar);
            }
        }

        // ── ABIERTA ───────────────────────────────────────────────────
        private void ConstruirRespuestaАbierta(Panel panel, int cx, int yInicio)
        {
            int txtW = 800;

            // ── Campo de texto ──
            var txtRespuesta = new System.Windows.Forms.TextBox();
            txtRespuesta.Multiline = true;
            txtRespuesta.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            txtRespuesta.Size = new Size(txtW, 80);
            txtRespuesta.Location = new Point(cx - txtW / 2, yInicio + 10);
            txtRespuesta.BorderStyle = BorderStyle.FixedSingle;
            txtRespuesta.BackColor = Color.FromArgb(245, 245, 240);
            txtRespuesta.ReadOnly = true; // solo teclado táctil
            panel.Controls.Add(txtRespuesta);

            // ── Panel del teclado integrado ──
            var pnlTeclado = new Panel();
            pnlTeclado.Size = new Size(panel.Width, 320);
            pnlTeclado.Location = new Point(0, panel.Height - 320);
            pnlTeclado.BackColor = Color.FromArgb(40, 40, 40);
            pnlTeclado.Visible = false;
            panel.Controls.Add(pnlTeclado);

            // ── Botón para mostrar teclado ──
            var btnTeclado = new Button();
            btnTeclado.Text = "⌨️  Tocar para escribir";
            btnTeclado.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            btnTeclado.ForeColor = Color.FromArgb(0, 102, 51);
            btnTeclado.BackColor = Color.White;
            btnTeclado.FlatStyle = FlatStyle.Flat;
            btnTeclado.FlatAppearance.BorderColor = Color.FromArgb(0, 102, 51);
            btnTeclado.FlatAppearance.BorderSize = 2;
            btnTeclado.Size = new Size(txtW, 50);
            btnTeclado.Location = new Point(cx - txtW / 2, yInicio + 100);
            btnTeclado.Cursor = Cursors.Hand;
            btnTeclado.Click += (s, ev) =>
            {
                pnlTeclado.Visible = true;
                pnlTeclado.BringToFront();
            };
            panel.Controls.Add(btnTeclado);

            // ── Construir teclas ──
            ConstruirTecladoIntegrado(pnlTeclado, txtRespuesta);

            // ── Botón omitir ──
            var btnOmitir = new Button();
            btnOmitir.Text = "Omitir";
            btnOmitir.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            btnOmitir.ForeColor = Color.FromArgb(150, 150, 150);
            btnOmitir.BackColor = Color.White;
            btnOmitir.FlatStyle = FlatStyle.Flat;
            btnOmitir.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnOmitir.Size = new Size(150, 50);
            btnOmitir.Location = new Point(cx - 200, yInicio + 165);
            btnOmitir.Cursor = Cursors.Hand;
            btnOmitir.Click += (s, ev) => GrabarRespuestaNueva(0, "Omitido", null);
            panel.Controls.Add(btnOmitir);

            // ── Botón enviar ──
            var btnEnviar = new Button();
            btnEnviar.Text = "✓  Enviar";
            btnEnviar.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            btnEnviar.ForeColor = Color.White;
            btnEnviar.BackColor = Color.FromArgb(0, 102, 51);
            btnEnviar.FlatStyle = FlatStyle.Flat;
            btnEnviar.FlatAppearance.BorderSize = 0;
            btnEnviar.Size = new Size(200, 50);
            btnEnviar.Location = new Point(cx + 10, yInicio + 165);
            btnEnviar.Cursor = Cursors.Hand;
            btnEnviar.Click += (s, ev) =>
            {
                string texto = txtRespuesta.Text.Trim();
                GrabarRespuestaNueva(string.IsNullOrEmpty(texto) ? 0 : 1,
                                     string.IsNullOrEmpty(texto) ? "Sin respuesta" : texto,
                                     null);
            };
            panel.Controls.Add(btnEnviar);
        }

        private void ConstruirTecladoIntegrado(Panel pnlTeclado, System.Windows.Forms.TextBox txtDestino)
        {
            bool mayusculas = false;
            int btnW = 58, btnH = 55, margen = 4;

            var filas = new[]
            {
        new[] { "q","w","e","r","t","y","u","i","o","p" },
        new[] { "a","s","d","f","g","h","j","k","l","ñ" },
        new[] { "z","x","c","v","b","n","m" }
    };

            var especiales = new[] { "@", ".", ",", "-", "_" };
            var botonesLetras = new List<Button>();

            int yBase = 10;

            // ── Fila especiales ──
            int xEsp = (pnlTeclado.Width - (especiales.Length * (btnW + margen))) / 2;
            foreach (var esp in especiales)
            {
                string capEsp = esp;
                var btn = CrearTeclaIntegrada(capEsp, btnW, btnH);
                btn.Location = new Point(xEsp, yBase);
                btn.Click += (s, ev) =>
                {
                    txtDestino.Text += capEsp;
                    txtDestino.SelectionStart = txtDestino.Text.Length;
                };
                pnlTeclado.Controls.Add(btn);
                xEsp += btnW + margen;
            }
            yBase += btnH + margen;

            // ── Filas de letras ──
            foreach (var fila in filas)
            {
                int xInicio = (pnlTeclado.Width - (fila.Length * (btnW + margen))) / 2;
                foreach (var tecla in fila)
                {
                    string cap = tecla;
                    var btn = CrearTeclaIntegrada(cap.ToUpper(), btnW, btnH);
                    btn.Location = new Point(xInicio, yBase);
                    botonesLetras.Add(btn);
                    btn.Click += (s, ev) =>
                    {
                        string letra = mayusculas ? cap.ToUpper() : cap.ToLower();
                        txtDestino.Text += letra;
                        txtDestino.SelectionStart = txtDestino.Text.Length;
                    };
                    pnlTeclado.Controls.Add(btn);
                    xInicio += btnW + margen;
                }
                yBase += btnH + margen;
            }

            // ── Fila inferior: Mayús, Espacio, ⌫, Aceptar ──
            int yFinal = yBase + 4;
            int xFinal = 20;

            // Mayús
            var btnMayus = CrearTeclaIntegrada("⇧ Mayús", 110, btnH);
            btnMayus.Location = new Point(xFinal, yFinal);
            btnMayus.BackColor = Color.FromArgb(80, 80, 80);
            btnMayus.Click += (s, ev) =>
            {
                mayusculas = !mayusculas;
                btnMayus.BackColor = mayusculas
                    ? Color.FromArgb(255, 200, 0)
                    : Color.FromArgb(80, 80, 80);
                btnMayus.ForeColor = mayusculas ? Color.Black : Color.White;
                foreach (var b in botonesLetras)
                    b.Text = mayusculas ? b.Text.ToUpper() : b.Text.ToLower();
            };
            pnlTeclado.Controls.Add(btnMayus);
            xFinal += 120;

            // Espacio
            var btnEsp = CrearTeclaIntegrada("Espacio", pnlTeclado.Width - xFinal - 240, btnH);
            btnEsp.Location = new Point(xFinal, yFinal);
            btnEsp.Click += (s, ev) =>
            {
                txtDestino.Text += " ";
                txtDestino.SelectionStart = txtDestino.Text.Length;
            };
            pnlTeclado.Controls.Add(btnEsp);
            xFinal += btnEsp.Width + 10;

            // Borrar
            var btnBorrar = CrearTeclaIntegrada("⌫", 80, btnH);
            btnBorrar.Location = new Point(xFinal, yFinal);
            btnBorrar.BackColor = Color.FromArgb(180, 50, 50);
            btnBorrar.Click += (s, ev) =>
            {
                if (txtDestino.Text.Length > 0)
                    txtDestino.Text = txtDestino.Text.Substring(0, txtDestino.Text.Length - 1);
            };
            pnlTeclado.Controls.Add(btnBorrar);
            xFinal += 90;

            // Aceptar
            var btnAceptar = CrearTeclaIntegrada("✓ Aceptar", 130, btnH);
            btnAceptar.Location = new Point(xFinal, yFinal);
            btnAceptar.BackColor = Color.FromArgb(0, 102, 51);
            btnAceptar.Click += (s, ev) =>
            {
                pnlTeclado.Visible = false; // ocultar teclado al aceptar
            };
            pnlTeclado.Controls.Add(btnAceptar);
        }

        private Button CrearTeclaIntegrada(string texto, int ancho, int alto)
        {
            return new Button
            {
                Text = texto,
                Size = new Size(ancho, alto),
                Font = new Font("Segoe UI", 13, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.FromArgb(80, 80, 80) },
                Cursor = Cursors.Hand
            };
        }
        private void GrabarRespuestaNueva(int calificacion, string descripcion, string comentario)
        {
            try
            {
                // Ocultar panel encuesta nueva
                this.InvokeIfRequired(() =>
                {
                    if (pnlEncuestaNueva != null)
                        pnlEncuestaNueva.Visible = false;
                });

                // Grabar en background para no bloquear UI
                var t = new System.Threading.Thread(() =>
                {
                    try
                    {
                        using (var db = new POSEntities())
                        {
                            string sql = @"
                        INSERT INTO dbo.pos_encuesta_respuesta
                        (fecha, id_pregunta, pregunta_texto, identificacion, nombre_cliente,
                         calificacion, descripcion, comentario, establecimiento, 
                         pto_emision, cajero, numero_factura)
                        VALUES
                        (GETDATE(), @idPregunta, @preguntaTexto, @id, @nombre,
                         @cal, @desc, @comentario, @est, @pto, @cajero, @factura)";

                            db.Database.ExecuteSqlCommand(sql,
                                new System.Data.SqlClient.SqlParameter("@idPregunta", _preguntaActual?.id ?? 0),
                                new System.Data.SqlClient.SqlParameter("@preguntaTexto", _preguntaActual?.pregunta ?? ""),
                                new System.Data.SqlClient.SqlParameter("@id", _encuestaNuevaIdentificacion ?? ""),
                                new System.Data.SqlClient.SqlParameter("@nombre", _encuestaNuevaНombreCliente ?? ""),
                                new System.Data.SqlClient.SqlParameter("@cal", calificacion),
                                new System.Data.SqlClient.SqlParameter("@desc", descripcion ?? ""),
                                new System.Data.SqlClient.SqlParameter("@comentario", (object)comentario ?? DBNull.Value),
                                new System.Data.SqlClient.SqlParameter("@est", Control.Common.GlobalParameters.Establecimiento ?? ""),
                                new System.Data.SqlClient.SqlParameter("@pto", Control.Common.GlobalParameters.PuntoEmision ?? ""),
                                new System.Data.SqlClient.SqlParameter("@cajero", Control.Common.GlobalParameters.UsuarioNombre ?? ""),
                                new System.Data.SqlClient.SqlParameter("@factura", _encuestaNuevaNumeroFactura ?? "")
                            );

                            Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                nameof(frmMainTouchClte),
                                nameof(GrabarRespuestaNueva),
                                $"Encuesta nueva grabada | Pregunta:{_preguntaActual?.id} | Cal:{calificacion} | Desc:{descripcion} | Factura:{_encuestaNuevaNumeroFactura}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Error,
                            nameof(frmMainTouchClte),
                            nameof(GrabarRespuestaNueva),
                            $"Error grabando encuesta nueva: {ex.Message}");
                    }
                    finally
                    {
                        // ── Mostrar siguiente pregunta o publicidad ──
                        this.InvokeIfRequired(() => MostrarEncuestaNueva(
                            _encuestaNuevaNumeroFactura,
                            _encuestaNuevaIdentificacion,
                            _encuestaNuevaНombreCliente));
                    }
                });
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    nameof(frmMainTouchClte),
                    nameof(GrabarRespuestaNueva),
                    $"Error: {ex.Message}");
                this.InvokeIfRequired(() => MostrarEncuestaNueva(
                    _encuestaNuevaNumeroFactura,
                    _encuestaNuevaIdentificacion,
                    _encuestaNuevaНombreCliente));
            }
        }

        private void InicializarTimerCarrusel()
        {
            _timerRecargarCarrusel = new System.Windows.Forms.Timer();
            _timerRecargarCarrusel.Interval = 15000;
            _timerRecargarCarrusel.Tick += async (s, e) =>
            {
                try
                {
                    string url = "http://192.168.127.220:3030/api/data";

                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        nameof(frmMainTouchClte), "TimerCarrusel",
                        $"Consultando cambios | ultimoTs:{_ultimoUpdatedAt}");

                    using (var client = new System.Net.WebClient())
                    {
                        client.Headers.Add("Cache-Control", "no-cache, no-store");
                        client.Headers.Add("Pragma", "no-cache");
                        url = $"http://192.168.127.220:3030/api/data?_t={DateTime.Now.Ticks}";
                        string json = await Task.Run(() => client.DownloadString(url));
                        

                        Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Info,
                            nameof(frmMainTouchClte), "TimerCarrusel",
                            $"Respuesta recibida | json(100):{json.Substring(0, Math.Min(100, json.Length))}");

                        var match = System.Text.RegularExpressions.Regex.Match(
                            json, @"""updatedAt""\s*:\s*""([^""]+)""");

                        if (match.Success)
                        {
                            string nuevoTs = match.Groups[1].Value;

                            Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                nameof(frmMainTouchClte), "TimerCarrusel",
                                $"updatedAt encontrado | nuevo:{nuevoTs} | anterior:{_ultimoUpdatedAt} | sonIguales:{nuevoTs == _ultimoUpdatedAt}");

                            if (!string.IsNullOrEmpty(_ultimoUpdatedAt) && nuevoTs != _ultimoUpdatedAt)
                            {
                                Control.Common.Logger.LogMessage(
                                    Control.Common.Enum.LogTypes.Info,
                                    nameof(frmMainTouchClte), "TimerCarrusel",
                                    "¡Cambio detectado! Recargando WebBrowser...");
                                // En el bloque donde detectas el cambio (nuevoTs != _ultimoUpdatedAt):
                                this.InvokeIfRequired(() =>
                                {
                                    if (webViewAds != null)
                                    {
                                        string est = Control.Common.GlobalParameters.Establecimiento ?? "";
                                        // El timestamp en ticks garantiza URL única cada vez → IE11 no puede cachear
                                        string nuevaUrl = $"http://192.168.127.220:3030/carrusel.html?est={est}&t={DateTime.Now.Ticks}";

                                        // ← Recargar SIEMPRE, esté visible o no
                                        try
                                        {
                                            webViewAds.Navigate(nuevaUrl);
                                            Control.Common.Logger.LogMessage(
                                                Control.Common.Enum.LogTypes.Info,
                                                nameof(frmMainTouchClte), "TimerCarrusel",
                                                $"Recargado (visible:{webViewAds.Visible}): {nuevaUrl}");
                                        }
                                        catch (Exception exNav)
                                        {
                                            Control.Common.Logger.LogMessage(
                                                Control.Common.Enum.LogTypes.Error,
                                                nameof(frmMainTouchClte), "TimerCarrusel",
                                                $"Error al navegar: {exNav.Message}");
                                        }
                                    }
                                });
                            }
                            else if (string.IsNullOrEmpty(_ultimoUpdatedAt))
                            {
                                Control.Common.Logger.LogMessage(
                                    Control.Common.Enum.LogTypes.Info,
                                    nameof(frmMainTouchClte), "TimerCarrusel",
                                    "Primera lectura — guardando timestamp inicial");
                            }

                            _ultimoUpdatedAt = nuevoTs;
                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Warning,
                                nameof(frmMainTouchClte), "TimerCarrusel",
                                $"updatedAt NO encontrado en respuesta | json:{json.Substring(0, Math.Min(200, json.Length))}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte), "TimerCarrusel",
                        $"Error: {ex.Message}");
                }
            };
            _timerRecargarCarrusel.Start();

            Control.Common.Logger.LogMessage(
                Control.Common.Enum.LogTypes.Info,
                nameof(frmMainTouchClte), "TimerCarrusel",
                "Timer iniciado — intervalo 15 segundos");
        }

        public void LimpiarEncuestaPendiente()
        {
            this.InvokeIfRequired(() =>
            {
                try
                {
                    // Limpiar cola de preguntas pendientes
                    _preguntasPendientes?.Clear();
                    _preguntaActual = null;
                    _encuestaNuevaCargada = false;
                    _encuestaNuevaNumeroFactura = "";
                    _encuestaNuevaIdentificacion = "";
                    _encuestaNuevaНombreCliente = "";

                    // Ocultar panel de encuesta si está visible
                    if (pnlEncuestaNueva != null && pnlEncuestaNueva.Visible)
                    {
                        pnlEncuestaNueva.Visible = false;
                        pnlEncuestaNueva.Controls.Clear();
                        this.Controls.Remove(pnlEncuestaNueva);
                        pnlEncuestaNueva = null;
                    }

                    

                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        nameof(frmMainTouchClte),
                        nameof(LimpiarEncuestaPendiente),
                        "Estado de encuesta limpiado completamente");
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte),
                        nameof(LimpiarEncuestaPendiente),
                        $"Error: {ex.Message}");
                }
            });
        }

        private void MostrarMensajeEsperaAbierta(Panel panel)
        {
            // ← Ocultar el webBanner mientras el cajero responde
            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl is System.Windows.Forms.WebBrowser)
                {
                    ctrl.Visible = false;
                }
            }

            int cx = panel.Width / 2;

            // ── Calcular posición debajo del separador ──
            // El separador está en yPregunta + 110 según ConstruirPanelEncuestaNueva
            // Necesitamos posicionar el subtítulo claramente DEBAJO de eso
            int ySubtitulo = (int)(panel.Height * 0.22) + 75 + 150 + 60;
            // Equivale a: yTitulo + 90 (espacio título→pregunta) + 110 (espacio pregunta→separador) + 60 (margen)

            var lblSubtitulo = new Label();
            lblSubtitulo.Text = "Por favor indique su respuesta al Cajero";
            lblSubtitulo.Font = new Font("Segoe UI", 18, FontStyle.Italic);
            lblSubtitulo.ForeColor = Color.FromArgb(120, 120, 120);
            lblSubtitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblSubtitulo.Size = new Size(panel.Width - 40, 50);
            lblSubtitulo.Location = new Point(20, ySubtitulo);
            panel.Controls.Add(lblSubtitulo);
            lblSubtitulo.BringToFront();
        }

        public void ResponderPreguntaAbiertaDesdeCajero(int calificacion, string respuesta)
        {
            GrabarRespuestaNueva(calificacion, respuesta, null);
        }

        private void InicializarTimerBanner()
        {
            _timerRecargarBanner = new System.Windows.Forms.Timer();
            _timerRecargarBanner.Interval = 15000;
            _timerRecargarBanner.Tick += async (s, e) =>
            {
                try
                {
                    string url = "http://192.168.127.220:3030/api/banner/status";

                    using (var client = new System.Net.WebClient())
                    {
                        client.Headers.Add("Cache-Control", "no-cache, no-store");
                        client.Headers.Add("Pragma", "no-cache");

                        string json = await Task.Run(() =>
                            client.DownloadString(url + "?_t=" + DateTime.Now.Ticks));

                        var match = System.Text.RegularExpressions.Regex.Match(
                            json, @"""updatedAt""\s*:\s*""([^""]+)""");

                        if (match.Success)
                        {
                            string nuevoTs = match.Groups[1].Value;

                            if (!string.IsNullOrEmpty(_ultimoUpdatedAtBanner)
                                && nuevoTs != _ultimoUpdatedAtBanner)
                            {
                                Control.Common.Logger.LogMessage(
                                    Control.Common.Enum.LogTypes.Info,
                                    nameof(frmMainTouchClte), "TimerBanner",
                                    "Cambio detectado en banner — recargando...");

                                this.InvokeIfRequired(() =>
                                {
                                    if (_webBanner != null && !_webBanner.IsDisposed)
                                    {
                                        string est = Control.Common.GlobalParameters
                                            .Establecimiento ?? "";
                                        string nuevaUrl = $"http://192.168.127.220:3030/banner.html" +
                                            $"?est={est}&t={DateTime.Now.Ticks}";
                                        try
                                        {
                                            _webBanner.Document?.InvokeScript("eval",
                                                new object[] { $"window.location.replace('{nuevaUrl}')" });
                                        }
                                        catch
                                        {
                                            _webBanner.Navigate(nuevaUrl);
                                        }

                                        Control.Common.Logger.LogMessage(
                                            Control.Common.Enum.LogTypes.Info,
                                            nameof(frmMainTouchClte), "TimerBanner",
                                            $"Banner recargado: {nuevaUrl}");
                                    }
                                });
                            }

                            _ultimoUpdatedAtBanner = nuevoTs;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        nameof(frmMainTouchClte), "TimerBanner",
                        $"Error: {ex.Message}");
                }
            };
            _timerRecargarBanner.Start();
        }
    }

    public class PreguntaEncuestaRaw
    {
        public int id { get; set; }
        public string pregunta { get; set; }
        public string tipo { get; set; }
        public string opciones { get; set; }
        public bool seleccion_multiple { get; set; }
    }

    public class PreguntaEncuesta
    {
        public int id { get; set; }
        public string pregunta { get; set; }
        public string tipo { get; set; }
        public string opciones { get; set; }
        public bool seleccion_multiple { get; set; }
    }
}
