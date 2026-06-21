using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class UDT_DetArticuloPOS
    {   
        public string OrigenConsulta { get; set; }
        public string ITEMID { get; set; }
        public string ITEMNAME { get; set; }
        public string ITEMBARCODE { get; set; }
        public string ITEMGROUPID { get; set; }
        public string categoria  { get; set; }
        public string VARIEDAD  { get; set; }
        public string SUBGRUPO { get; set; }
        public string UNITID { get; set; }
        public string GRUPO { get; set; }
        public int Itemtype { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadINEC { get; set; }
        public decimal UNIDADES { get; set; }
        public decimal COST { get; set; }
        public decimal PrecioAX { get; set; }
        public decimal PVP { get; set; }
        public decimal IVA { get; set; }
        public decimal RetencionPorcentaje { get; set; }
        public decimal Descuento { get; set; }
        public decimal DescuentoActual  { get; set; }
        public decimal DescuentoAX { get; set; }
        public decimal SubtotalSinDescuento { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ivaproducto { get; set; }
        public decimal Total  { get; set; }
        public decimal PorcDescuentoDivisionEmpleado  { get; set; }
        public decimal EsExcluidoPromoIVA { get; set; }


        public static DataTable GetDataTableArticuloPOS()
        {
            DataTable DetArticuloPOS = new DataTable();
            DetArticuloPOS.Columns.Add("OrigenConsulta", typeof(string));
            DetArticuloPOS.Columns.Add("ITEMID", typeof(string));
            DetArticuloPOS.Columns.Add("ITEMBARCODE", typeof(string));
            DetArticuloPOS.Columns.Add("ITEMGROUPID", typeof(string));
            DetArticuloPOS.Columns.Add("categoria", typeof(string));
            DetArticuloPOS.Columns.Add("VARIEDAD", typeof(string));
            DetArticuloPOS.Columns.Add("SUBGRUPO", typeof(string));
            DetArticuloPOS.Columns.Add("UNITID", typeof(string));
            DetArticuloPOS.Columns.Add("GRUPO", typeof(string));
            DetArticuloPOS.Columns.Add("Itemtype", typeof(int));
            DetArticuloPOS.Columns.Add("Cantidad", typeof(decimal));
            DetArticuloPOS.Columns.Add("CantidadINEC", typeof(decimal));
            DetArticuloPOS.Columns.Add("COST", typeof(decimal));
            DetArticuloPOS.Columns.Add("PrecioAX", typeof(decimal));
            DetArticuloPOS.Columns.Add("PVP", typeof(decimal));
            DetArticuloPOS.Columns.Add("IVA", typeof(decimal));
            DetArticuloPOS.Columns.Add("RetencionPorcentaje", typeof(decimal));
            DetArticuloPOS.Columns.Add("Descuento", typeof(decimal));
            DetArticuloPOS.Columns.Add("DescuentoActual", typeof(decimal));
            DetArticuloPOS.Columns.Add("DescuentoAX", typeof(decimal));
            DetArticuloPOS.Columns.Add("SubtotalSinDescuento", typeof(decimal));
            DetArticuloPOS.Columns.Add("Subtotal", typeof(decimal));
            DetArticuloPOS.Columns.Add("ivaproducto", typeof(decimal));
            DetArticuloPOS.Columns.Add("Total", typeof(decimal));
            DetArticuloPOS.Columns.Add("PorcDescuentoDivisionEmpleado", typeof(decimal));
            DetArticuloPOS.Columns.Add("EsExcluidoPromoIVA", typeof(int));
            return DetArticuloPOS;
        }

    }

    

}
