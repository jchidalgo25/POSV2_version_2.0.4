using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;

namespace POS.Control.Main.MainTouch
{
    public static class ClienteService
    {
        public static event EventHandler<ClienteEmpleado> ClienteActualizado;
        public static event EventHandler<List<Producto>> ProductosActualizados;

        public static void NotificarCambioCliente(ClienteEmpleado cliente)
        {
            ClienteActualizado?.Invoke(null, cliente);
        }

        public static void NotificarActualizacionProductos(List<Producto> productos)
        {
            ProductosActualizados?.Invoke(null, productos);
        }
    }
}
