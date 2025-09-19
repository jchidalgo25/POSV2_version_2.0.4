using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class UDT_DetFactura
    {
        public string establecimiento { get; set; }
        public string punto_emision { get; set; }
        public int numFactura { get; set; }
        public string identificacionClte { get; set; }
        public string nombre_cliente { get; set; }
        public string telefono_cliente { get; set; }
        public string direccion_cliente { get; set; }
        public long linea { get; set; }
        public string item_id { get; set; }
        public string item_nombre { get; set; }
        public decimal cantidad { get; set; }
        public decimal total { get; set; }

        public DataTable GetDataTable()
        {
            DataTable tblDetFactura = new DataTable();
            tblDetFactura.Columns.Add("establecimiento", typeof(string));
            tblDetFactura.Columns.Add("punto_emision", typeof(string));
            tblDetFactura.Columns.Add("numFactura", typeof(int));
            tblDetFactura.Columns.Add("identificacionClte", typeof(string));
            tblDetFactura.Columns.Add("nombre_cliente", typeof(string));
            tblDetFactura.Columns.Add("telefono_cliente", typeof(string));
            tblDetFactura.Columns.Add("direccion_cliente", typeof(string));
            tblDetFactura.Columns.Add("linea", typeof(int));
            tblDetFactura.Columns.Add("item_id", typeof(string));
            tblDetFactura.Columns.Add("item_nombre", typeof(string));
            tblDetFactura.Columns.Add("cantidad", typeof(decimal));
            tblDetFactura.Columns.Add("total", typeof(decimal));
            return tblDetFactura;
        }


    }


}

