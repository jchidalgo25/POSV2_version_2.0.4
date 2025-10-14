using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace POS.UnitTests
{
    public partial class DeletedProductsByIdCaja : Form
    {
        public DeletedProductsByIdCaja()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ArmaSeccionProductosEliminados(textBox1.Text.Trim()));
        }

        public string ArmaSeccionProductosEliminados(string idCaja)
        {
            string response = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<br/><br/><strong>ITEMS BORRADOS DURANTE TURNO: </strong><br/>");

                SqlConnection conexion = new SqlConnection("Data Source=srv-pos;Initial Catalog=POS;User ID=svc_sql_pos;password=a17472Ol0");
                string Query = null;
                SqlCommand comando = default(SqlCommand);
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Query = " select    Producto = itm.ItemName, Cantidad = sum(eli.Qty) " +
                        " from		TblDeletedProduct eli with (nolock) " +
                        " 			inner join " +
                        " 			dbo.core_factura fac with (nolock) on	eli.ESTABLISHMENT = fac.establecimiento and " +
                        "													eli.EMISIONSERIES = fac.punto_emision and " +
                        "													eli.NUMBER = fac.numero " +
                        "			inner join " +
                        "			dbo.pos_item itm with (nolock) on eli.ITEMID = itm.ITEMID " +
                        " where		fac.msgError = '" + idCaja + "'" +
                        " group by	itm.ItemName " +
                        " order by  Cantidad desc ";
                using (conexion)
                {
                    conexion.Open();
                    comando = new SqlCommand(Query, conexion);
                    SqlDataReader reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        response += string.Format("{0}: {1}", (string.IsNullOrEmpty(response) ? "" : ", ") + reader["Producto"].ToString(), reader["Cantidad"].ToString());
                    }
                    conexion.Close();
                }

                sb.Append((string.IsNullOrEmpty(response) ? "No hubo eliminación de ítems" : response) + "<br/>");
                response = sb.ToString();
            }
            catch (Exception ex)
            {
                response = string.Empty;
            }
            return response;
        }
    }
}
