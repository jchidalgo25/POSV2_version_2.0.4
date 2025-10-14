using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;

namespace POS.Control.Main.MainTouch
{
    class FacturaService
    {
        public static event EventHandler<BindingList<Producto>> ProductosActualizados;
        public static BindingList<Producto> Lineas { get; set; } = new BindingList<Producto>();


 

        public static void NotificarProductosActualizados(BindingList<Producto> productos)
        {
            ProductosActualizados?.Invoke(null, productos);
        }
        public static void NotificarCambioProductos(BindingList<Producto> productos)
        {
            ProductosActualizados?.Invoke(null, productos);
        }

        public static void AgregarProducto(Producto producto, Factura facturaActual)
        {
            facturaActual.Productos.Add(producto);
            NotificarCambioProductos(facturaActual.Productos);
        }


        public static void AgregarProducto(DataGridView dgv, Factura facturaActual)
        {
            if (dgv.SelectedRows.Count == 0 || facturaActual == null) return;

            var productoSeleccionado = dgv.SelectedRows[0].DataBoundItem as Producto;

            if (productoSeleccionado == null) return;

            AgregarProducto(productoSeleccionado, facturaActual);
        }

        // Ejemplo: eliminar producto
        public static void EliminarProducto(string codigo, Factura facturaActual)
        {
            var producto = facturaActual.Productos.FirstOrDefault(p => p.Id == codigo);
            if (producto != null)
            {
                facturaActual.Productos.Remove(producto);
                NotificarCambioProductos(facturaActual.Productos);
            }
        }

    }
}
