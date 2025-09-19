using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.NotaCredito
{
    class UDT_NotaCredito
    {
        public string cliente { get; set; }
        public string nombre { get; set; }
        public string numdocumento { get; set; }
        public DateTime fecha { get; set; }
        public string usuario { get; set; }
        public decimal valor { get; set; }
        public string documentoAplica { get; set; }
        public string motivo { get; set; }
        public string msgError { get; set; }
        public string claveAccesoSRI { get; set; }
        public string establecimiento { get; set; }
        public string punto_emision { get; set; }
        public string secuenciaNC { get; set; }

        public DataTable GetDataTable()
        {
            DataTable tblDetNotaCredito = new DataTable();
            tblDetNotaCredito.Columns.Add("cliente", typeof(string));
            tblDetNotaCredito.Columns.Add("nombre", typeof(string));
            tblDetNotaCredito.Columns.Add("numdocumento", typeof(string));
            tblDetNotaCredito.Columns.Add("fecha", typeof(DateTime));
            tblDetNotaCredito.Columns.Add("usuario", typeof(string));
            tblDetNotaCredito.Columns.Add("valor", typeof(decimal));
            tblDetNotaCredito.Columns.Add("documentoAplica", typeof(string));
            tblDetNotaCredito.Columns.Add("motivo", typeof(string));
            tblDetNotaCredito.Columns.Add("msgError", typeof(string));
            tblDetNotaCredito.Columns.Add("claveAccesoSRI", typeof(string));
            tblDetNotaCredito.Columns.Add("establecimiento", typeof(string));
            tblDetNotaCredito.Columns.Add("punto_emision", typeof(string));
            tblDetNotaCredito.Columns.Add("secuenciaNC", typeof(string));

            return tblDetNotaCredito;
        }

        public string GetStringInsert(DataTable dtConsulta)
        {
            string dataReturn = string.Empty;
            // Generar INSERT SQL y guardar en log
            StringBuilder insertSql = new StringBuilder();
            insertSql.AppendLine("INSERT INTO @UDT_NotaCredito (");
            insertSql.AppendLine("    cliente, nombre, numdocumento, fecha, usuario, valor, documentoAplica,");
            insertSql.AppendLine("    motivo, msgError, claveAccesoSRI,  ");
            insertSql.AppendLine("    establecimiento, punto_emision, secuenciaNC");
            insertSql.AppendLine(")");


            insertSql.AppendLine("Select ");
            foreach (DataRow workRow in dtConsulta.Rows)
            {
                insertSql.AppendLine($"    '{workRow["cliente"]}', '{workRow["nombre"]}', '{workRow["numdocumento"]}', '{((DateTime)workRow["fecha"]).ToString("yyyyMMdd HH:mm:ss")}',");
                insertSql.AppendLine($"    '{workRow["usuario"]}', {workRow["valor"]}, '{workRow["documentoAplica"]}',");
                insertSql.AppendLine($"    '{workRow["motivo"]}', '{workRow["msgError"]}', '{workRow["claveAccesoSRI"]}',");
                insertSql.AppendLine($"    '{workRow["establecimiento"]}', '{workRow["punto_emision"]}', {workRow["secuenciaNC"]}");
                insertSql.AppendLine(" ");

            }
            dataReturn = insertSql.ToString();
            return dataReturn;

        }

    }





}

