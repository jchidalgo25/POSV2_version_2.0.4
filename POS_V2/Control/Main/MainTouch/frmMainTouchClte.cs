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


        public frmMainTouchClte()
        {
            InitializeComponent();

            // Suscripción a eventos globales
            ClienteService.ClienteActualizado += ClienteService_ClienteActualizado;
            FacturaService.ProductosActualizados += FacturaService_ProductosActualizados;

            // Configurar UI
            ConfigurarGrid();
            RefrescarProductos(_factura);

        }

        public frmMainTouchClte(Factura factura) : this()
        {
            _factura = factura;
            RefrescarProductos(_factura);
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
                NombreCliente = "Cliente General";
                IdentificacionClte = "";
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
