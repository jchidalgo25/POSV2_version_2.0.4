using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class UDT_DetDescuentos
    {
        public string establecimiento { get; set; }
        public string punto_emision { get; set; }
        public string tipo_descuento { get; set; }
        public string codigo { get; set; }
        public string parametro { get; set; }
        public string parametro2 { get; set; }
        public bool especial { get; set; }
        public decimal valor { get; set; }

        public static DataTable GetDataTableDetDescuentos()
        {
            DataTable DetDescuentos = new DataTable();
            DetDescuentos.Columns.Add("establecimiento", typeof(string));
            DetDescuentos.Columns.Add("punto_emision", typeof(string));
            DetDescuentos.Columns.Add("tipo_descuento", typeof(string));
            DetDescuentos.Columns.Add("codigo", typeof(string));
            DetDescuentos.Columns.Add("parametro", typeof(string));
            DetDescuentos.Columns.Add("especial", typeof(bool));
            DetDescuentos.Columns.Add("valor", typeof(decimal));
            return DetDescuentos;
        }

    }



}
