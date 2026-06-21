using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Liris_BasePagoDLL.Modelos
{
    public class General
    {

        public enum LogTypes : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Debug,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Info,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Warn,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Error,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Fatal,
        }

        private static LirisLibLogger.Logger Log = new LirisLibLogger.Logger();

        public static DataTable ConvertModelToDataTable<T>(T modelo)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            var propiedades = typeof(T).GetProperties();

            // Crear columnas
            foreach (var prop in propiedades)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Crear una sola fila
            var valores = new object[propiedades.Length];
            for (int i = 0; i < propiedades.Length; i++)
            {
                valores[i] = propiedades[i].GetValue(modelo, null);
            }

            dataTable.Rows.Add(valores);

            return dataTable;
        }
        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // Obtener todas las propiedades públicas del tipo
            var propiedades = typeof(T).GetProperties();

            // Crear las columnas en el DataTable
            foreach (var prop in propiedades)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Agregar las filas
            foreach (var item in items)
            {
                var valores = new object[propiedades.Length];
                for (int i = 0; i < propiedades.Length; i++)
                {
                    valores[i] = propiedades[i].GetValue(item, null);
                }
                dataTable.Rows.Add(valores);
            }

            return dataTable;
        }
        public static DataTable CreateEmptyDataTable(Type myType)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            foreach (PropertyInfo info in myType.GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            return dt;
        }


        public static void LogMessage(LogTypes logType, string clase, string metodo, string msj, string infoAdicional = null)
        {
            var texto = string.Format("Clase: {0} |Método: {1} |Mensaje: {2} {3}",
                                                              clase,
                                                              metodo,
                                                              msj,
                                                              (string.IsNullOrEmpty(infoAdicional)) ? "" : "|" + infoAdicional);
            switch (logType)
            {
                case LogTypes.Debug:
                    Log.Graba_Log_Debug(texto);
                    break;
                case LogTypes.Info:
                    Log.Graba_Log_Info(texto);
                    break;
                case LogTypes.Warn:
                    Log.Graba_Log_Warn(texto);
                    break;
                case LogTypes.Error:
                    Log.Graba_Log_Error(texto);
                    break;
                case LogTypes.Fatal:
                    Log.Graba_Log_Fatal(texto);
                    break;
            }

        }
    }
}
