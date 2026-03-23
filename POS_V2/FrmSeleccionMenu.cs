using POS.Control;
using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace POS
{
    
    public partial class FrmSeleccionMenu : Form
    {
        private string _idCategoria;
        private List<ProductoSeleccionado> carrito = new List<ProductoSeleccionado>();
        public event Action<List<ProductoSeleccionado>> OnProductosConfirmados;
        private CancellationTokenSource _cts;
        private List<ProductoTemporal> _todosLosProductos = new List<ProductoTemporal>();


        public FrmSeleccionMenu(string tituloMenu, string idCategoria, string rutaImagen) // se recibe estas variables para ver que llega y que se va mostrar 
        {
            InitializeComponent();



            typeof(FlowLayoutPanel).InvokeMember("DoubleBuffered",
            System.Reflection.BindingFlags.SetProperty |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic,
            null, flpnlitems, new object[] { true });


       

            lbHeader.Text = tituloMenu;
            this._idCategoria = idCategoria;

            if (!string.IsNullOrEmpty(rutaImagen) && File.Exists(rutaImagen))
            {
                // pbIcono es el PictureBox que tienes a lado del título
                pbIcono.Image = Image.FromFile(rutaImagen);
                pbIcono.SizeMode = PictureBoxSizeMode.Zoom;
            }

            ConfigurarBotonesEspeciales();


        }

        private async void FrmSeleccionMenu_Load(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource(); // Inicializamos el interruptor

            try
            {
                await CargarProductos(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Se ignora, es normal cuando el usuario cancela
            }

        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DetenerCargas();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DetenerCargas();
            base.OnFormClosing(e);
        }

        private void DetenerCargas()
        {
            if (_cts != null)
            {
                _cts.Cancel(); // Apaga todos los hilos
                _cts.Dispose(); // Libera la memoria del interruptor
                _cts = null;
            }
        }

        // 1. Declaramos la lista de descuentos a nivel de clase para que esté disponible siempre
        private List<core_descuento> _listaDescuentosCache = new List<core_descuento>();

        private async Task CargarProductos(CancellationToken ct)
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    if (ct.IsCancellationRequested) return;
                    this.Cursor = Cursors.WaitCursor;

                    // CARGA ÚNICA DE DESCUENTOS: El gran cuello de botella estaba aquí
                    _listaDescuentosCache = await Task.Run(() => db.core_descuento.Where(x => x.activo).ToList(), ct);

                    string sql = @"SELECT V.ITEMID, V.ARTICULO, V.PRECIO, V.BARRAS, 
                                  V.CATEGORIA, V.GRUPO, V.SUBGRUPO, V.VARIEDAD 
                           FROM POS.dbo.tbl_menu_DET D 
                           INNER JOIN POS.dbo.VW_SEARCHPRODUCT_MENU V ON D.codigo_barras = V.BARRAS 
                           WHERE D.id_menu = @p0 AND V.ESTABLECIMIENTO = @p1 AND D.estado = 1";

                    string estab = Control.Common.GlobalParameters.Establecimiento;

                    _todosLosProductos = await Task.Run(() => db.Database.SqlQuery<ProductoTemporal>(sql,
                        new System.Data.SqlClient.SqlParameter("@p0", _idCategoria.Trim()),
                        new System.Data.SqlClient.SqlParameter("@p1", estab)).ToList(), ct);

                    await RenderizarProductos(_todosLosProductos, ct);
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally { this.Cursor = Cursors.Default; }
        }

        private async Task RenderizarProductos(List<ProductoTemporal> lista, CancellationToken ct)
        {
            // Limpieza rápida
            flpnlitems.Controls.Clear();

            // No usamos SuspendLayout aquí porque impide que el usuario vea progreso mientras scrollea

            foreach (var item in lista)
            {
                if (ct.IsCancellationRequested) return;

                // Usamos la cache que cargamos al inicio (Instantáneo)
                decimal precioFinal = CalcularPrecioLocalEnRAM(item, _listaDescuentosCache,Control.Common.GlobalParameters.Establecimiento);

                UcProductoTarjeta tarjeta = new UcProductoTarjeta();
                tarjeta.ConfigurarTarjeta(item.ARTICULO, precioFinal, item.BARRAS, global::POS.Properties.Resources._default);

                CargarImagenLentaAsync(tarjeta, item.BARRAS, ct);

                tarjeta.btnAgregar.Click += (s, ev) => AgregarAlResumen(item.ARTICULO, precioFinal, item.BARRAS, item.ITEMID);

                flpnlitems.Controls.Add(tarjeta);

                // OPTIMIZACIÓN DE SCROLL: 
                // Cada 5 productos dejamos que la UI procese clics y scroll del usuario
                if (flpnlitems.Controls.Count % 5 == 0)
                {
                    await Task.Delay(1, ct);
                }
            }
        }

        private async void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            // Cancelamos el proceso anterior inmediatamente
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                // Esperamos 300ms antes de empezar a filtrar (Evita que se congele al escribir)
                await Task.Delay(300, token);

                string filtro = txtBusqueda.Text.ToLower().Trim();
                var filtrados = _todosLosProductos
                    .Where(x => x.ARTICULO.ToLower().Contains(filtro) || x.BARRAS.Contains(filtro))
                    .ToList();

                await RenderizarProductos(filtrados, token);
            }
            catch (OperationCanceledException) { }
        }

        private async void CargarImagenLentaAsync(UcProductoTarjeta tarjeta, string barras, CancellationToken ct)
        {
            try
            {
                await Task.Run(() => {
                    // Si el token ya se canceló antes de empezar la descarga, salimos
                    if (ct.IsCancellationRequested) return;

                    Image img = LoadImageTableLayout(barras);

                    // Verificamos si la tarjeta y el token siguen vigentes antes de hacer el Invoke
                    if (!ct.IsCancellationRequested && tarjeta != null && !tarjeta.IsDisposed && tarjeta.IsHandleCreated)
                    {
                        tarjeta.Invoke(new MethodInvoker(() => {
                            if (!ct.IsCancellationRequested && !tarjeta.IsDisposed)
                                tarjeta.ActualizarImagen(img);
                        }));
                    }
                }, ct);
            }
            catch { /* Errores de red o cancelación se ignoran */ }
        }

        // ESTA FUNCIÓN ES TU PROPIEDAD 'PrecioLocal' PERO TRABAJANDO CON LA LISTA EN MEMORIA
        private decimal CalcularPrecioLocalEnRAM(ProductoTemporal item, List<core_descuento> descuentos, string estab)
        {
            decimal pvp = item.PRECIO; // Este es tu PrecioAx

            // Helper para buscar el valor del descuento en la lista precargada
            Func<string, string, string, decimal> buscarDesc = (tipo, param1, param2) => {
                var d = descuentos.FirstOrDefault(x => x.tipo_descuento == tipo && x.parametro == param1 && (param2 == null || x.parametro2 == param2));
                return d != null ? (d.valor / 100) : 0M;
            };

            // Aplicamos la cascada de tu método PrecioLocal
            pvp = pvp - (pvp * buscarDesc("Establecimiento", estab, null));
            pvp = decimal.Round(pvp, 2, MidpointRounding.AwayFromZero);

            pvp = pvp - (pvp * buscarDesc("EstablecimientoCategoria", estab, item.CATEGORIA));
            pvp = Math.Round(pvp, 2, MidpointRounding.AwayFromZero);

            pvp = pvp - (pvp * buscarDesc("EstablecimientoVariedad", estab, item.VARIEDAD));
            pvp = Math.Round(pvp, 2, MidpointRounding.AwayFromZero);

            pvp = pvp - (pvp * buscarDesc("EstablecimientoGrupo", estab, item.GRUPO));
            pvp = Math.Round(pvp, 2, MidpointRounding.AwayFromZero);

            pvp = pvp - (pvp * buscarDesc("EstablecimientoSubGrupo", estab, item.SUBGRUPO));
            pvp = Math.Round(pvp, 2, MidpointRounding.AwayFromZero);

            // Descuento de Producto o LocalProducto
            decimal dsctoProd = buscarDesc("EstablecimientoProducto", estab, item.ITEMID);
            decimal dsctoLocalProd = buscarDesc("Producto", item.ITEMID, null);

            decimal finalDsctoProd = dsctoLocalProd != 0 ? dsctoLocalProd : dsctoProd;

            pvp = pvp - (pvp * finalDsctoProd);
            pvp = Math.Round(pvp, 2, MidpointRounding.AwayFromZero);

            return pvp;
        }

        private decimal subtotalAcumulado = 0;

        private void AgregarAlResumen(string nombre, decimal precio, string codigo, string itemId)
        {
            var itemSeleccionado = new ProductoSeleccionado
            {
                Articulo = nombre,
                Precio = precio,
                Barras = codigo,
                ItemId = itemId

            };

            carrito.Add(itemSeleccionado);

            // 1. Contenedor principal del ítem
            Panel pnlItem = new Panel();
            pnlItem.Size = new Size(flowLayoutPanel1.Width - 25, 35);
            pnlItem.Margin = new Padding(0, 2, 0, 2);
            pnlItem.BackColor = Color.White;

            pnlItem.Tag = itemSeleccionado;

            // 2. Etiqueta de texto (Nombre y Precio) con fuente mejorada
            Label lblInfo = new Label();
            lblInfo.Text = $"{nombre.Trim()} - ${precio:N2}";
            lblInfo.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular); // Letra más legible
            lblInfo.AutoSize = false;
            lblInfo.Size = new Size(pnlItem.Width - 40, 30);
            lblInfo.Location = new Point(5, 5);
            lblInfo.TextAlign = ContentAlignment.MiddleLeft;

            // 3. Botón de eliminar (La "X") usando Guna o un botón estándar
            Button btnEliminar = new Button();
            btnEliminar.Text = "✕";
            btnEliminar.Size = new Size(25, 25);
            btnEliminar.Location = new Point(pnlItem.Width - 30, 4);
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.ForeColor = Color.Red;
            btnEliminar.Cursor = Cursors.Hand;

            // LÓGICA PARA ELIMINAR:
            btnEliminar.Click += (s, ev) => {

                var datoABorrar = (ProductoSeleccionado)pnlItem.Tag;
                carrito.Remove(datoABorrar);

                flowLayoutPanel1.Controls.Remove(pnlItem); // Quita el control visual
                subtotalAcumulado -= precio;               // Resta del total
                ActualizarTotales();                       // Refresca los labels
            };

            // 4. Armar el control
            pnlItem.Controls.Add(lblInfo);
            pnlItem.Controls.Add(btnEliminar);

            // 5. Agregar al flujo y actualizar
            flowLayoutPanel1.Controls.Add(pnlItem);
            subtotalAcumulado += precio;
            ActualizarTotales();

            // Auto-scroll al final para ver el último agregado
            flowLayoutPanel1.ScrollControlIntoView(pnlItem);
        }

        private void ActualizarTotales()
        {
            // label2 es el Subtotal, label1 es el conteo de ítems
            label2.Text = $"SUBTOTAL: ${subtotalAcumulado:N2}";
            label1.Text = $"TOTAL ITEMS: {flowLayoutPanel1.Controls.Count}";
        }

        public Image LoadImageTableLayout(string itemid)
        {
            // URL del servidor de imágenes de Delportal
            string url = "http://reportes.liris.com.ec/ImgAx/100/@barcode.jpg";
            PictureBox picbox = new PictureBox();

            try
            {
                var urlnew = url.Replace("@barcode", itemid);
                picbox.Load(urlnew); // Descarga la imagen de la web
            }
            catch
            {
                // Si no hay imagen, carga una por defecto de tus recursos
                picbox.Image = global::POS.Properties.Resources._default;
            }
            return picbox.Image;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Verificamos que haya algo en el carrito
            if (carrito != null && carrito.Count > 0)
            {
                // Disparamos el evento enviando la lista 'carrito'
                OnProductosConfirmados?.Invoke(carrito);

                // Cerramos el modal indicando que todo salió bien
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                //MessageBox.Show("Por favor, seleccione al menos un producto.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Control.Common.General.GetMensajeToList(10019);
            }
        }

        private void ConfigurarBotonesEspeciales()
        {
            // Usamos lbHeader.Text que ya fue asignado en el constructor
            if (lbHeader.Text.ToUpper().Contains("FLORERÍA") || _idCategoria == "MENU_FL")
            {
                btnGuardarRamo.Visible = true;
            }
            else
            {
                btnGuardarRamo.Visible = false;
            }
        }
        private void btnGuardarRamo_Click(object sender, EventArgs e)
        {
            // 1. Validar que haya flores en el carrito
            if (carrito == null || carrito.Count == 0)
            {
                //MessageBox.Show("Agregue al menos una flor al carrito para armar el ramo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Control.Common.General.GetMensajeToList(10019);
                return;
            }

            try
            {
                using (POSEntities db = new POSEntities())
                {
                    // Iniciamos una transacción de base de datos
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // 2. GENERAR EL CÓDIGO CORRELATIVO (FL00001, FL00002...)
                            string ultimoCodigo = db.Database.SqlQuery<string>(
                                "SELECT TOP 1 IdRamo FROM dbo.tbl_ramo_cab WHERE IdRamo LIKE 'FL%' ORDER BY IdRamo DESC"
                            ).FirstOrDefault();

                            string nuevoCodigo = "FL00001";
                            if (!string.IsNullOrEmpty(ultimoCodigo))
                            {
                                // Extraemos el número, le sumamos 1 y rellenamos con 5 ceros
                                int numero = int.Parse(ultimoCodigo.Replace("FL", ""));
                                nuevoCodigo = "FL" + (numero + 1).ToString("D5");
                            }

                            // 3. GUARDAR LA CABECERA
                            decimal totalRamo = subtotalAcumulado;
                            string sqlCab = "INSERT INTO dbo.tbl_ramo_cab (IdRamo, FechaCreacion, TotalRamo, Estado) VALUES (@p0, GETDATE(), @p1, 1)";
                            db.Database.ExecuteSqlCommand(sqlCab,
                                new System.Data.SqlClient.SqlParameter("@p0", nuevoCodigo),
                                new System.Data.SqlClient.SqlParameter("@p1", totalRamo)
                            );

                            // 4. GUARDAR EL DETALLE (Las flores del carrito)
                            string sqlDet = @"INSERT INTO dbo.tbl_ramo_det (IdRamo, ITEMID, BARRAS, ARTICULO, Cantidad, PrecioUnitario, TotalLinea) 
                                      VALUES (@p0, @p1, @p2, @p3, 1, @p4, @p4)";

                            foreach (var item in carrito)
                            {
                                db.Database.ExecuteSqlCommand(sqlDet,
                                    new System.Data.SqlClient.SqlParameter("@p0", nuevoCodigo),
                                    new System.Data.SqlClient.SqlParameter("@p1", string.IsNullOrEmpty(item.ItemId) ? (object)DBNull.Value : item.ItemId),
                                    new System.Data.SqlClient.SqlParameter("@p2", item.Barras),
                                    new System.Data.SqlClient.SqlParameter("@p3", item.Articulo),
                                    new System.Data.SqlClient.SqlParameter("@p4", item.Precio)
                                );
                            }

                            // Confirmamos que todo se guardó correctamente
                            dbContextTransaction.Commit();

                            // 5. LIMPIAR INTERFAZ Y AVISAR AL USUARIO
                           // MessageBox.Show($"¡Ramo guardado exitosamente!\n\nCódigo del Ramo: {nuevoCodigo}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes()
                            {
                                // Usamos la etiqueta exacta que pusiste en la base de datos
                                codigo = "[nuevoCodigo]",

                                // (Ajusta 'descripcion' por el nombre real de la propiedad que guarda el valor en tu clase, a veces se llama 'valor' o 'texto')
                                valor = nuevoCodigo
                            });

                            // Lanzamos el mensaje visual al cajero usando el ID 10020 de tu tabla
                            Control.Common.General.GetMensajeToList(10020, parametros);

                            ImprimirTicketRamo(nuevoCodigo, totalRamo);

                            // Vaciamos el carrito para el siguiente ramo
                            carrito.Clear();
                            flowLayoutPanel1.Controls.Clear();
                            subtotalAcumulado = 0;
                            ActualizarTotales();
                        }
                        catch (Exception ex)
                        {
                            // Si algo falla, deshacemos todo para no dejar datos corruptos
                            dbContextTransaction.Rollback();
                            MessageBox.Show("Ocurrió un error al guardar el ramo en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ImprimirTicketRamo(string codigoRamo, decimal precioTotal)
        {
            // 1. Construimos el diseño del ticket usando las etiquetas que tu clase soporta
            StringBuilder ticket = new StringBuilder();

            // Título centrado
            ticket.AppendLine("<titulo align=\"center\">ARREGLO FLORAL</titulo>");
            ticket.AppendLine();

            // Información del precio (usando un texto un poco más grande)
            ticket.AppendLine($"<info align=\"center\">Precio Ref: ${precioTotal:N2}</info>");
            ticket.AppendLine();

            // El código de barras centrado
            ticket.AppendLine($"<barcode align=\"center\">{codigoRamo}</barcode>");
            ticket.AppendLine();

            ticket.AppendLine("<legales align=\"left\">Detalle del Ramo:</legales>");

            // --- AQUÍ ESTÁ LA LÓGICA PARA AGRUPAR ---
            // Agrupamos el carrito por Artículo y PrecioUnitario
            var itemsAgrupados = carrito
                .GroupBy(x => new { x.Articulo, x.Precio })
                .Select(grupo => new
                {
                    Articulo = grupo.Key.Articulo,
                    PrecioUnitario = grupo.Key.Precio,
                    Cantidad = grupo.Count(),                  // Cuenta cuántos hay de este tipo
                    TotalLinea = grupo.Sum(x => x.Precio)      // Suma el precio de los repetidos
                });

            // Ahora iteramos sobre la lista agrupada en lugar del carrito original
            foreach (var item in itemsAgrupados)
            {
                // Limita el nombre a 20 caracteres para que no desborde el ticket
                string nombreCorto = item.Articulo.Length > 20 ? item.Articulo.Substring(0, 20) : item.Articulo;

                // Formato: 3 x FLORES FOLLAJE JM - $9.18
                ticket.AppendLine($"<legales align=\"left\">{item.Cantidad} x {nombreCorto} - ${item.TotalLinea:N2}</legales>");
            }
            // --- FIN DE LA AGRUPACIÓN ---

            // 2. Instanciamos tu clase de impresión
            DSS.Controles.Impresion.DSSPrint impresora = new DSS.Controles.Impresion.DSSPrint();

            // Le pasamos el texto con las etiquetas
            impresora.TextToPrint = ticket.ToString();

            // 3. Mandamos a imprimir
            try
            {
                impresora.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir el ticket del ramo: " + ex.Message, "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


    }
    public class ProductoTemporal
    {
        public string ARTICULO { get; set; }
        public decimal PRECIO { get; set; }
        public string BARRAS { get; set; }
        public decimal PVP_REAL { get; set; }
        public string ITEMID { get; set; } // Agregamos esto
        public string CATEGORIA { get; set; }
        public string VARIEDAD { get; internal set; }
        public string GRUPO { get; internal set; }
        public string SUBGRUPO { get; internal set; }
    }
    public class ProductoSeleccionado
    {
        public string Barras { get; set; }
        public string Articulo { get; set; }
        public decimal Precio { get; set; }
        
        public string ItemId { get; set; }
    }


}
