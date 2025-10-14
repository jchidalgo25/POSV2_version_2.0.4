using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.NotaCredito
{
    class UDT_NotaCreditoDetalle
    {
        public string numdocumento { get; set; }
        public DateTime fecha { get; set; }
        public int linea { get; set; }
        public string unidad { get; set; }
        public decimal cantidad { get; set; }
        public string item_id { get; set; }
        public decimal pvp { get; set; }
        public decimal descuento { get; set; }
        public decimal iva { get; set; }
        public decimal total { get; set; }
        public decimal TotalNC { get; set; }
        public decimal SubtotalSinDescuento { get; set; }


        public DataTable GetDataTable()
        {
            DataTable tblDetNotaCredito = new DataTable();
            tblDetNotaCredito.Columns.Add("numdocumento", typeof(string));
            tblDetNotaCredito.Columns.Add("fecha", typeof(DateTime));
            tblDetNotaCredito.Columns.Add("linea", typeof(int));
            tblDetNotaCredito.Columns.Add("unidad", typeof(string));
            tblDetNotaCredito.Columns.Add("cantidad", typeof(decimal));
            tblDetNotaCredito.Columns.Add("item_id", typeof(string));
            tblDetNotaCredito.Columns.Add("Nombre", typeof(string));
            tblDetNotaCredito.Columns.Add("pvp", typeof(decimal));
            tblDetNotaCredito.Columns.Add("descuento", typeof(decimal));
            tblDetNotaCredito.Columns.Add("iva", typeof(decimal));
            tblDetNotaCredito.Columns.Add("total", typeof(decimal));
            tblDetNotaCredito.Columns.Add("TotalNC", typeof(decimal));
            tblDetNotaCredito.Columns.Add("SubtotalSinDescuento", typeof(decimal));

            return tblDetNotaCredito;
        }

        public string GetStringInsert(DataTable tblNotaCreditoDetalle)
        {
            string dataReturn = string.Empty;
            // Generar INSERT SQL y guardar en log
            StringBuilder insertDetalleLog = new StringBuilder();
            string insertSQL = string.Empty;
            insertSQL = $"INSERT INTO @UDT_NotaCreditoDetalle (numdocumento, fecha, linea, unidad, cantidad, item_id, Nombre, pvp, descuento, iva, total, TotalNC, SubtotalSinDescuento) ";
            int secuencia = 1;
            
            foreach (DataRow detalle in tblNotaCreditoDetalle.Rows)
            {

                string selectLine = $"SELECT '{detalle["numdocumento"]}', " +
                             $"'{Convert.ToDateTime(detalle["fecha"]).ToString("yyyyMMdd HH:mm:ss")}', " +
                             $"{detalle["linea"]}, " +
                             $"'{detalle["unidad"]}', " +
                             $"{Convert.ToDecimal(detalle["cantidad"]).ToString("0.####", CultureInfo.InvariantCulture)}, " +
                             $"'{detalle["item_id"]}', " +
                             $"'{detalle["Nombre"]}', " +
                             $"{Convert.ToDecimal(detalle["pvp"]).ToString("0.####", CultureInfo.InvariantCulture)}, " +
                             $"{Convert.ToDecimal(detalle["descuento"]).ToString("0.####", CultureInfo.InvariantCulture)}, " +
                             $"{Convert.ToDecimal(detalle["iva"]).ToString("0.####", CultureInfo.InvariantCulture)}, " +
                             $"{Convert.ToDecimal(detalle["total"]).ToString("0.####", CultureInfo.InvariantCulture)}, " +
                             $"{Convert.ToDecimal(detalle["TotalNC"]).ToString("0.####", CultureInfo.InvariantCulture)}, " +
                             $"{Convert.ToDecimal(detalle["SubtotalSinDescuento"]).ToString("0.####", CultureInfo.InvariantCulture)}";


                if (secuencia > 1)
                    insertDetalleLog.AppendLine("UNION ALL");

                insertDetalleLog.AppendLine(selectLine);
                secuencia++;
            }

            dataReturn = insertSQL + "\n" + insertDetalleLog.ToString();
            return dataReturn;

        }



    }
}
