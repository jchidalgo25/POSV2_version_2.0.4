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
            FacturaService.ProductosActualizados += ActualizarTicket;

            // Refrescar productos iniciales
            RefrescarProductos(_factura);

            // Establecer fuente monoespaciada para simular ticket
            txtTicket.Font = new Font("Courier New", 15, FontStyle.Regular);
        }


        
        private void frmMainTouchClte_FormClosing(object sender, FormClosingEventArgs e)
        {


            Control.Common.GlobalParameters.frmTouchClte = null;

        }


        public frmMainTouchClte()
        {
            InitializeComponent();
            ClienteService.ClienteActualizado += ClienteService_ClienteActualizado;
            FacturaService.ProductosActualizados += ActualizarTicket; // 👈 Aquí
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
                if (_factura != null)
                {
                    _factura.Productos = new BindingList<Producto>(productos.ToList());
                    RefrescarProductos(_factura);
                }
            });
        }

        private void ConfigurarGrid()
        {
            gridItemsClte.AutoGenerateColumns = false;
            gridItemsClte.Columns.Clear();

            var colNombre = new GridViewTextBoxColumn("Nombre") { HeaderText = "Nombre" };
            var colPrecio = new GridViewDecimalColumn("P.V.P") { HeaderText = "P.V.P" };
            var colCantidad = new GridViewDecimalColumn("Cantidad") { HeaderText = "Cantidad" };
            var colTotal = new GridViewDecimalColumn("Total") { HeaderText = "Total" };
            var colIdTemporal = new GridViewDecimalColumn("IdTemporal") { HeaderText = "IdTemporal" };
            


            gridItemsClte.Columns.Add(colNombre);
            gridItemsClte.Columns.Add(colCantidad);
            gridItemsClte.Columns.Add(colPrecio);
            gridItemsClte.Columns.Add(colTotal);
            gridItemsClte.Columns.Add(colIdTemporal);

            // Estilo opcional
            gridItemsClte.EnableCustomSorting = true;
            gridItemsClte.CustomSorting += RadGridView1_CustomSorting;
            gridItemsClte.Columns["IdTemporal"].SortOrder = RadSortOrder.Descending;
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

    }
}
