using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace POS.Control.Common
{
    

    public class General
    {

        private static System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        public static List<Mensajes> MensajeslistaXML = new List<Mensajes>();
        public enum VersionLAN
        {
            Multired,
            FastTrack,
        }



        public enum TipoBilletera
        {
            [Description("PayClub")] OTTDiners,
            [Description("BDP Wallet")] OTTPacifico,
        }


        public static string _ConnectionString { get; set; }

        public General(POSEntities dbo)
        {
            _ConnectionString = dbo.Database.Connection.ConnectionString.ToString();
        }
       
        private static string ConnectionString()
        {

            try
            {
                POSEntities dbo = new POSEntities();

                _ConnectionString = dbo.Database.Connection.ConnectionString.ToString();
            }
            catch (Exception)
            {
                _ConnectionString = string.Empty;
            }
            return _ConnectionString;
        }

        static async Task<DataSet> GetDataSetAsync(string query, string _ConnectionString = "", int CommandTimeout = 0)
        {
            // Inicializamos el DataSet
            DataSet ds = new DataSet();

            // Validamos si la cadena de conexión está vacía o nula
            if (string.IsNullOrEmpty(_ConnectionString))
            {
                _ConnectionString = ConnectionString();  // Método que retorna la cadena de conexión
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                {
                    // Creamos el comando SQL
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = CommandTimeout;

                        // Abrimos la conexión
                        await con.OpenAsync();

                        // Usamos el SqlDataAdapter para llenar el DataSet
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // Llenamos el DataSet de forma asíncrona (usando Task.Run porque Fill no es async)
                            await Task.Run(() => da.Fill(ds));
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Loguear el error de SQL
                LogError($"SQL Error: {sqlEx.Message}", sqlEx);
                return new DataSet();  // Devolvemos un DataSet vacío en caso de error SQL
            }
            catch (Exception ex)
            {
                // Loguear cualquier otro error
                LogError($"Error general: {ex.Message}", ex);
                return new DataSet();  // Devolvemos un DataSet vacío en caso de fallo general
            }

            return ds;  // Devolvemos el DataSet cargado
        }
        public static async Task<DataSet> GetDataSetAsync(string query, string connectionStrings, List<SqlParameter> parametros = null, int CommandTimeout = 0)
        {
            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandTimeout = CommandTimeout;

                        if (parametros != null && parametros.Count > 0)
                        {
                            cmd.Parameters.AddRange(parametros.ToArray());
                        }

                        await con.OpenAsync(); // Abrimos la conexión de forma asíncrona

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // Fill no tiene versión async, pero lo envolvemos en Task.Run para evitar bloquear UI o thread-pool
                            await Task.Run(() => da.Fill(ds));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes agregar logging si usas un sistema de logs
                Console.WriteLine($"Error al ejecutar GetDataSetAsync: {ex.Message}");
                return new DataSet(); // Devuelve un DataSet vacío en caso de error
            }

            return ds;
        }

        public static DataSet GetDataSet(string query, string connectionStrings)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    try
                    {
                        con.Open();
                        cmd.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        da.Dispose();
                    }
                    catch (Exception ex)
                    {
                        return ds;
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
            finally
            {
                ds.Dispose();
            }
        }
        public static DataSet GetDataSet(string query, string connectionStrings, List<SqlParameter> parametros = null, int CommandTimeout = 0)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        try
                        {
                            con.Open();
                            cmd.CommandTimeout = 0;

                            // Agregar parámetros si existen
                            if (parametros != null)
                            {
                                cmd.Parameters.AddRange(parametros.ToArray());
                            }

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(ds);
                            }
                        }
                        catch (Exception)
                        {
                            // Se puede agregar log aquí si deseas registrar errores
                            return ds;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
                return ds;
            }
            catch (Exception)
            {
                return ds;
            }
        }

        public static DataSet GetDataSet(string query, string _ConnectionString = "", int CommandTimeout = 0)
        {
            // Inicializamos el DataSet
            DataSet ds = new DataSet();

            // Validamos si la cadena de conexión está vacía o nula
            if (string.IsNullOrEmpty(_ConnectionString))
            {
                _ConnectionString = ConnectionString();  // Método que retorna la cadena de conexión
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                {
                    // Creamos el comando SQL
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandTimeout = CommandTimeout;  // Establecer un timeout razonable (30 segundos)

                    try
                    {
                        // Abrimos la conexión
                        con.Open();

                        // Usamos el SqlDataAdapter para llenar el DataSet
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        // Loguear el error de SQL
                        LogError($"SQL Error: {sqlEx.Message}", sqlEx);
                        return new DataSet();  // Devolvemos un DataSet vacío en caso de error SQL
                    }
                    catch (Exception ex)
                    {
                        // Loguear cualquier otro error
                        LogError($"Error: {ex.Message}", ex);
                        return new DataSet();  // Devolvemos un DataSet vacío en caso de error general
                    }
                    finally
                    {

                        con.Close();
                        con.Dispose(); 
                    }
                }
            }
            catch (Exception ex)
            {
                // Loguear cualquier error de conexión o general
                LogError($"General Error: {ex.Message}", ex);
                return new DataSet();  // Devolvemos un DataSet vacío en caso de fallo general
            }

            return ds;  // Devolvemos el DataSet cargado
        }

        // Método para loguear los errores (esto es solo un ejemplo, puedes adaptarlo según tu sistema de logs)
        private static void LogError(string message, Exception ex)
        {
            // Loguear el error de alguna forma, por ejemplo, en un archivo de log o una base de datos.
            Console.WriteLine($"[ERROR] {message}");
            Console.WriteLine($"Exception Details: {ex.ToString()}");
        }


        public static System.Data.DataTable CreateEmptyDataTable(Type myType)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            foreach (PropertyInfo info in myType.GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            return dt;
        }


        public static string ObtenerRed(int index)
        {
            switch (index)
            {
                case 0:
                    return "1";
                case 1:
                    return "2";
                case 2:
                    return "3";
                default:
                    return "1";
            }
        }

        internal static int ObtenerTamanoTrama(int versionSHA)
        {
            switch (versionSHA)
            {
                case 0:
                    return 40;
                case 1:
                    return 40;
                case 2:
                    return 64;
                default:
                    return 40;
            }
        }

        public static string ObtenerTipoCredito(int index)
        {
            switch (index)
            {
                case 0:
                    return "00";
                case 1:
                    return "01";
                case 2:
                    return "02";
                case 3:
                    return "03";
                case 4:
                    return "07";
                case 5:
                    return "09";
                case 6:
                    return "21";
                case 7:
                    return "22";
                default:
                    return "00";
            }
        }

        public static string ObtenerTipoTrx(int index)
        {
            switch (index)
            {
                case 0:
                    return "01";
                case 1:
                    return "02";
                case 2:
                    return "03";
                case 3:
                    return "04";
                case 4:
                    return "06";
                case 5:
                    return "07";
                default:
                    return "01";
            }
        }

        public static string ObtenerTipoBilletera(int index)
        {
            switch ((TipoBilletera)index)
            {
                case TipoBilletera.OTTDiners:
                    return "01";
                case TipoBilletera.OTTPacifico:
                    return "02";
                default:
                    return "  ";
            }
        }

        public static string ObtenerDescripcionRed(string CodRed)
        {
            switch (CodRed)
            {
                case "01":
                case "1":
                    return "DATAFAST";
                case "02":
                case "2":
                    return "MEDIANET";
                case "03":
                case "3":
                    return "AUSTRO";
                
            }
            return CodRed;
        }

        public static int ObtenerVersionLan(int index)
        {
            switch ((VersionLAN)index)
            {
                case VersionLAN.Multired:
                    return 1;
                case VersionLAN.FastTrack:
                    return 2;
                default:
                    return 2;
            }
        }

        public static string GetDescription<T>(T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return (string)null;
            string description = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
            if (field != (FieldInfo)null)
            {
                object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (customAttributes != null && customAttributes.Length != 0)
                    description = ((DescriptionAttribute)customAttributes[0]).Description;
            }
            return description;
        }


        public static decimal ConvertStringToDecimal(string valor)
        {
            decimal valorRespuesta = 0;
            
            try
            {
                if (string.IsNullOrEmpty(valor)) { valor = "0.00"; }
                valorRespuesta = decimal.Parse(valor.Trim());
            }
            catch (Exception)
            {
                if (valor.Trim().Substring(0, 1) == ".")
                {
                    valor = "0" + valor;
                    valorRespuesta = decimal.Parse(valor.Trim());
                }
            }

            return valorRespuesta;

        }

        public static void ValidaContingente()
        {


            if (Control.Common.GlobalParameters.ConectContingente.PinPadContingente)
            {

                var validaFecha = (DateTime.Now >= Control.Common.GlobalParameters.ConectContingente.FechaInicioEspera &&
                                   DateTime.Now <= Control.Common.GlobalParameters.ConectContingente.FechaFinEspera);
                if (!validaFecha)
                {
                    Control.Common.GlobalParameters.Autorizador = 2;
                    Control.Common.GlobalParameters.ConectContingente.Autorizador = 2;
                    Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = 2;
                    Control.Common.GlobalParameters.ConectContingente.PinPadContingente = false;
                    Control.Common.GlobalParameters.ConectContingente.FechaInicioEspera = new DateTime();
                    Control.Common.GlobalParameters.ConectContingente.FechaFinEspera = new DateTime();
                }


            }

        }


        

        public static string GetArticulo(string CodArticulo)
        {

            string Query = string.Empty;
            string ITEMNAME = string.Empty;
            try
            {

                Query = string.Concat(Query, "Select itm.* , itmBar.ITEMBARCODE from pos_item  itm ", Environment.NewLine);
                Query = string.Concat(Query, "inner join pos_itembarra itmBar ", Environment.NewLine);
                Query = string.Concat(Query, "  on itm.ITEMID = itmBar.ITEMID ", Environment.NewLine);
                Query = string.Concat(Query, "where 1=1 ", Environment.NewLine);
                Query = string.Concat(Query, "and itm.DATAAREAID = 'liri'", Environment.NewLine);
                Query = string.Concat(Query, $"and itm.ITEMID = '{CodArticulo}'", Environment.NewLine);

                DataSet dtsConsulta = new DataSet();
                dtsConsulta = GetDataSet(Query);
                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        ITEMNAME = dtsConsulta.Tables[0].Rows[0]["ITEMNAME"].ToString();
                    }
                }

                return ITEMNAME;
            }
            catch (Exception)
            {

                ITEMNAME = "ERROR";
                return ITEMNAME;
            }

        }


        public static custclassificationgroup ValidaClienteEmpresarial(string identificacion)
        {
            string query = string.Empty;
            custclassificationgroup emplEmp = new custclassificationgroup();
            string connectString = Properties.Settings.Default.CONECTA_AX;
            DataSet dtsConsulta = new DataSet();

            identificacion = identificacion.Replace("'", "-");

            try
            {
                query = string.Empty;

                query = string.Concat(query, " SELECT  CODE, TXT ", Environment.NewLine);
                query = string.Concat(query, " , deta.MAILADMINISTRARTIVO ", Environment.NewLine);
                query = string.Concat(query, " , deta.MAILRH, deta.DIASVENCIMIENTO, deta.CUSTID ", Environment.NewLine);
                query = string.Concat(query, " , deta.BLOQUEOTEMPORAL, deta.DIACORTE2, deta.FECHA_CONTRATO ", Environment.NewLine);
                query = string.Concat(query, " , deta.CONTRATO_GENEREADO, deta.CONTACTO ", Environment.NewLine);
                query = string.Concat(query, " , deta.DIRECCION, deta.MONTO_CREDITO ", Environment.NewLine);
                query = string.Concat(query, " , deta.TASAINTERES, deta.APLICAINTERES ", Environment.NewLine);
                query = string.Concat(query, " From DynamicsAx1.ax.custclassificationgroup deta", Environment.NewLine);
                query = string.Concat(query, " where 1=1 and CUSTID = '", identificacion, "'", Environment.NewLine);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "validaClienteEmpresarial", "query: " + query);
                dtsConsulta = GetDataSet(query, connectString);

                if (dtsConsulta.Tables.Count > 0)
                {

                    emplEmp = new custclassificationgroup();
                    foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                    {
                        emplEmp.CODE = data["CODE"].ToString();
                        emplEmp.TXT = data["TXT"].ToString();
                        emplEmp.MAILADMINISTRARTIVO = data["MAILADMINISTRARTIVO"].ToString();
                        emplEmp.MAILRH = data["MAILRH"].ToString();
                        emplEmp.DIASVENCIMIENTO = Int32.Parse(data["DIASVENCIMIENTO"].ToString());
                        emplEmp.CUSTID = data["CUSTID"].ToString();
                        emplEmp.BLOQUEOTEMPORAL = (bool)data["BLOQUEOTEMPORAL"];
                        emplEmp.DIACORTE2 = Int32.Parse(data["DIACORTE2"].ToString());
                        //emplEmp.FECHA_CONTRATO = DateTime.Parse(data["FECHA_CONTRATO"].ToString());
                        emplEmp.BLOQUEOTEMPORAL = (bool)data["BLOQUEOTEMPORAL"];
                        emplEmp.APLICAINTERES = (bool)data["APLICAINTERES"];
                        emplEmp.MONTO_CREDITO = decimal.Parse(data["MONTO_CREDITO"].ToString());
                        emplEmp.TASAINTERES = decimal.Parse(data["TASAINTERES"].ToString());

                    }
                    

                }

                return emplEmp;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", $"ValidaClienteEmpleado,  Error: {ex.Message}");
                emplEmp = new custclassificationgroup();
                return emplEmp;
            }

          

        }

        public static ClienteEmpleado ValidaClienteEmpleado(string identificacion, string establecimiento, bool EsPinPadMultiRed = false, string ipAddress = "", string idCaja = "")
        {

            ClienteEmpleado clteEmpl = new ClienteEmpleado();
            string query = string.Empty;
            DataSet dtsConsulta = new DataSet();

            core_tarjetacreditointerno tarjeta = null;
            core_tarjetacreditointerno tarjetaAdicional = null;


            try
            {
                identificacion = identificacion.Replace("'", "-");

                query = string.Empty;
                query = string.Concat(query, " Exec spValidaClienteEmpleado ", Environment.NewLine);
                query = string.Concat(query, "  @Identificacion = '", identificacion, "'", Environment.NewLine);
                query = string.Concat(query, "  , @ipAddress = '", ipAddress, "'", Environment.NewLine);
                query = string.Concat(query, "  , @idCaja = '", idCaja, "'", Environment.NewLine);
                query = string.Concat(query, "  , @Establicimiento = '", establecimiento, "'", Environment.NewLine);
                query = string.Concat(query, "  , @EsPinPadMultiRed = '", EsPinPadMultiRed == true ? 1 : 0, "'", Environment.NewLine);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "query ValidaClienteEmpleado: " + query);

                dtsConsulta = GetDataSet(query);

                if (dtsConsulta.Tables.Count > 0)
                {
                    for (int indexTable = 0; indexTable <= dtsConsulta.Tables.Count - 1; indexTable++)
                    {
                        string tipoConsulta = string.Empty;
                        if (dtsConsulta.Tables[indexTable].Rows.Count > 0)
                        {
                            tipoConsulta = dtsConsulta.Tables[indexTable].Rows[0]["tipoConsulta"].ToString();

                            if(tipoConsulta == "CONS_CLIENTE")
                            {
                                bool EsClienteDuplicado = false;
                                foreach (DataRow data in dtsConsulta.Tables[indexTable].Rows)
                                {
                                    clteEmpl = new ClienteEmpleado();
                                    clteEmpl.CodError = data["CodError"].ToString();
                                    clteEmpl.MsjError = data["MsjError"].ToString();

                                    clteEmpl.CodMensaje = data["CodMensaje"].ToString();
                                    clteEmpl.TextMensaje = data["TextMensaje"].ToString();

                                    if (clteEmpl.CodError != "0")
                                    {
                                        string error = string.Concat(clteEmpl.CodError, " - ", clteEmpl.MsjError);
                                        clteEmpl.Identificacion = data["Identificacion"].ToString();
                                        
                                        if (data["EsClienteDuplicado"].ToString() == "1")
                                        {
                                            EsClienteDuplicado = true;
                                        }

                                        clteEmpl.EsClienteDuplicado = EsClienteDuplicado;
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", $"ValidaClienteEmpleado,  Error: {error}");
                                        return clteEmpl;

                                    }

                                    clteEmpl.EsClienteDuplicado = EsClienteDuplicado;
                                    clteEmpl.Identificacion = data["Identificacion"].ToString();
                                    clteEmpl.NombreCliente = data["NombreCliente"].ToString();
                                    clteEmpl.DireccionCliente = data["DireccionCliente"].ToString();
                                    clteEmpl.TelefonoCliente = data["TelefonoCliente"].ToString();
                                    clteEmpl.EmailCliente = data["EmailCliente"].ToString();
                                    clteEmpl.GrupoCliente = data["GrupoCliente"].ToString();


                                    //clteEmpl.ExisteCliente = (bool)data["ExisteCliente"]; // bool.Parse(data["ExisteCliente"].ToString());

                                    bool esBeneficiarioDevolucionIVA = false;
                                    clteEmpl.esBeneficiarioDevolucionIVA = false;
                                    if ((bool)data["esBeneficiarioDevolucionIVA"] == true)
                                    {
                                        esBeneficiarioDevolucionIVA = true;
                                    }

                                    decimal saldoDispDevolucionIVA = 0;
                                    if (data["saldoDispDevolucionIVA"].ToString() != "0")
                                    {
                                        saldoDispDevolucionIVA = decimal.Parse(data["saldoDispDevolucionIVA"].ToString());
                                    }

                                    clteEmpl.esBeneficiarioDevolucionIVA = esBeneficiarioDevolucionIVA;
                                    clteEmpl.saldoDispDevolucionIVA = saldoDispDevolucionIVA;
                                    

                                    bool EsEmpleadoLiris = false;
                                    clteEmpl.EsEmpleadoLiris = false;

                                    if (data["EsEmpleadoLiris"].ToString() == "1")
                                    {
                                        EsEmpleadoLiris = true;    
                                    }

                                    clteEmpl.PorcEmpleadoLiris = decimal.Parse(data["PorcDsctoEmpleado"].ToString());
                                    clteEmpl.EsEmpleadoLiris = EsEmpleadoLiris;

                                    bool EsClienteApp = false;
                                    string CodigoClienteApp = "";

                                        //_CodigoClienteApp
                                    if (data["EsClienteApp"].ToString() == "1")
                                    {
                                        EsClienteApp = true;
                                        CodigoClienteApp = data["CodigoClienteApp"].ToString();
                                    }

                                    clteEmpl.EsClienteApp = EsClienteApp;
                                    clteEmpl.CodigoClienteApp = CodigoClienteApp;

                                    clteEmpl.EsTarjetaEmpresa = false;
                                    tarjeta = null; // new core_tarjetacreditointerno();
                                    clteEmpl.tarjeta = null; // new core_tarjetacreditointerno();

                                    bool EsTarjetaEmpresa = false;
                                    string NumeroTarjetaEmpresa = string.Empty;
                                    decimal SaldoTarjetaEmpresa = 0;
                                    bool tieneTajetaDelportal = false;


                                    if ((bool)data["tieneTajetaDelportal"])
                                    {
                                        tieneTajetaDelportal = true;
                                    }

                                    
                                    if (data["EsTarjetaEmpresa"].ToString() == "1")
                                    {
                                        EsTarjetaEmpresa = true;
                                        NumeroTarjetaEmpresa = data["NumeroTarjetaEmpresa"].ToString();
                                        SaldoTarjetaEmpresa = decimal.Parse(data["SaldoTarjetaEmpresa"].ToString());
                                    }

                                    clteEmpl.tieneTajetaDelportal = tieneTajetaDelportal;
                                    clteEmpl.EsTarjetaEmpresa = EsTarjetaEmpresa;
                                    clteEmpl.NumeroTarjetaEmpresa = NumeroTarjetaEmpresa;
                                    clteEmpl.SaldoTarjetaEmpresa = SaldoTarjetaEmpresa;

                                    bool EsTarjetaEmpresaAdicional = false;
                                    string NumeroTarjetaEmpresaAdicional = string.Empty;
                                    decimal SaldoTarjetaEmpresaAdicional = 0;

   
                                    tarjetaAdicional = null; // new core_tarjetacreditointerno();
                                    clteEmpl.tarjetaAdicional = null; // new core_tarjetacreditointerno();

                                    bool tieneTajetaAdicionalDelportal= false;
                                    if ((bool)data["tieneTajetaAdicionalDelportal"])
                                    {
                                        tieneTajetaAdicionalDelportal = true;

                                        if (data["EsTarjetaEmpresaAdicional"].ToString() == "1")
                                        {
                                            EsTarjetaEmpresaAdicional = true;
                                            NumeroTarjetaEmpresaAdicional = data["NumeroTarjetaEmpresaAdicional"].ToString();
                                            SaldoTarjetaEmpresaAdicional = decimal.Parse(data["SaldoTarjetaEmpresaAdicional"].ToString());
                                        }
                                    }

                                    clteEmpl.tieneTajetaAdicionalDelportal = tieneTajetaAdicionalDelportal;
                                    clteEmpl.EsTarjetaEmpresaAdicional = EsTarjetaEmpresaAdicional;
                                    clteEmpl.NumeroTarjetaEmpresaAdicional = NumeroTarjetaEmpresaAdicional;
                                    clteEmpl.SaldoTarjetaEmpresaAdicional = SaldoTarjetaEmpresaAdicional;

                                    clteEmpl.CodigoClienteApp = data["CodigoClienteApp"].ToString();
                                    clteEmpl.CodMensaje = data["CodMensaje"].ToString();
                                    clteEmpl.TextMensaje = data["TextoMensaje"].ToString();
                                    

                                    bool AcumulaBilletera = false;
                                    if ((bool)data["AcumulaBilletera"])
                                    {
                                        AcumulaBilletera = true;
                                    }
                                    clteEmpl.AcumulaBilletera = AcumulaBilletera;


                                    //if (AcumulaBilletera)
                                    //{
                                    //    ValidarMonederoCampania(clteEmpleado.Identificacion);
                                    //}
                                    

                                }
                                continue;
                            }


                            tarjeta = null; // new core_tarjetacreditointerno();
                            clteEmpl.tarjeta = null; // new core_tarjetacreditointerno();
                            if (tipoConsulta == "CONS_TARJETA")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[indexTable].Rows)
                                {
                                    tarjeta = new core_tarjetacreditointerno();
                                    tarjeta.empresa_id = Int32.Parse(data["empresa_id"].ToString());
                                    tarjeta.id = Int32.Parse(data["id"].ToString());
                                    tarjeta.fecha_creacion = DateTime.Parse(data["fecha_creacion"].ToString());
                                    tarjeta.fecha_modificacion = DateTime.Parse(data["fecha_modificacion"].ToString());
                                    tarjeta.empresa_id = Int32.Parse(data["empresa_id"].ToString());
                                    tarjeta.codigo = data["empresa_id"].ToString();
                                    tarjeta.identificacion = data["identificacion"].ToString();
                                    tarjeta.nombre_tarjeta = data["nombre_tarjeta"].ToString();
                                    tarjeta.fecha_activacion = DateTime.Parse(data["fecha_activacion"].ToString());
                                    tarjeta.fecha_expiracion = DateTime.Parse(data["fecha_expiracion"].ToString());

                                    if (!string.IsNullOrEmpty(data["fecha_desactivacion"].ToString()))
                                    {
                                        tarjeta.fecha_desactivacion = DateTime.Parse(data["fecha_desactivacion"].ToString());
                                    }
                                    
                                    tarjeta.cupo = decimal.Parse(data["cupo"].ToString());
                                    tarjeta.saldo = decimal.Parse(data["saldo"].ToString());
                                    tarjeta.activo = (bool)data["activo"];
                                    tarjeta.identificacionPrincipal = data["identificacionPrincipal"].ToString();
                                }


                                clteEmpl.tarjeta = tarjeta;

                                continue;

                            }

                            tarjetaAdicional = null; // new core_tarjetacreditointerno();
                            clteEmpl.tarjetaAdicional = null; // new core_tarjetacreditointerno();
                            if (tipoConsulta == "CONS_TARJETA_ADICIONAL")
                            {
                                foreach (DataRow data in dtsConsulta.Tables[indexTable].Rows)
                                {
                                    tarjetaAdicional = new core_tarjetacreditointerno();
                                    tarjetaAdicional.id = Int32.Parse(data["id"].ToString());
                                    tarjetaAdicional.fecha_creacion = DateTime.Parse(data["fecha_creacion"].ToString());
                                    tarjetaAdicional.fecha_modificacion = DateTime.Parse(data["fecha_modificacion"].ToString());
                                    tarjetaAdicional.empresa_id = Int32.Parse(data["empresa_id"].ToString());
                                    tarjetaAdicional.codigo = data["empresa_id"].ToString();
                                    tarjetaAdicional.identificacion = data["identificacion"].ToString();
                                    tarjetaAdicional.nombre_tarjeta = data["nombre_tarjeta"].ToString();
                                    tarjetaAdicional.fecha_activacion = DateTime.Parse(data["fecha_activacion"].ToString());
                                    
                                    tarjetaAdicional.fecha_expiracion = DateTime.Parse(data["fecha_expiracion"].ToString());
                                    tarjetaAdicional.fecha_desactivacion = DateTime.Parse(data["fecha_desactivacion"].ToString());

                                    if (!string.IsNullOrEmpty(data["fecha_desactivacion"].ToString()))
                                    {
                                        tarjeta.fecha_desactivacion = DateTime.Parse(data["fecha_desactivacion"].ToString());
                                    }


                                    tarjetaAdicional.cupo = decimal.Parse(data["cupo"].ToString());
                                    tarjetaAdicional.saldo = decimal.Parse(data["saldo"].ToString());
                                    tarjetaAdicional.activo = (bool)data["activo"];
                                    tarjetaAdicional.identificacionPrincipal = data["identificacionPrincipal"].ToString();
                                }

                                clteEmpl.tarjetaAdicional = tarjetaAdicional;
                                continue;
                            }
                        }
                    }
                 

                }

                if (tarjeta != null)
                {
                    clteEmpl.tarjeta = tarjeta;
                }

                if (tarjetaAdicional != null)
                {
                    clteEmpl.tarjetaAdicional = tarjetaAdicional;
                }

        

                return clteEmpl;
            }
            catch (Exception ex)
            {
                clteEmpl = new ClienteEmpleado();
                clteEmpl.CodError = "99999";
                clteEmpl.MsjError = "Error" + ex.Message;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "ValidaClienteEmpleado,  Exception: " 
                                    + ex.Message);
                return clteEmpl;
            }


        }





        public static void TecladoPantalla(ref System.Diagnostics.Process virtualKeyboard)
        {
            try
            {
                // Limpiar cualquier instancia previa
                if (virtualKeyboard != null && !virtualKeyboard.HasExited)
                {
                    try
                    {
                        virtualKeyboard.Kill();
                        virtualKeyboard.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                            "Common.General.TecladoPantalla", "TecladoPantalla",
                            "Error al cerrar instancia previa del teclado: " + ex.Message);
                    }
                }

                Version version = Environment.OSVersion.Version;

                if (version.Major == 10 && version.Build >= 22000)
                {
                    // Windows 11 - Abrir ajustes de teclado táctil
                    Process.Start("cmd.exe", "/c start ms-settings:easeofaccess-keyboard");
                    return;
                }

                string windir = Environment.GetEnvironmentVariable("WINDIR");
                string osk = null;

                // Buscar en ubicaciones conocidas
                osk = @"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe";
                if (!File.Exists(osk))
                {
                    osk = Path.Combine(windir, "SysWOW64", "osk.exe");
                    if (!File.Exists(osk))
                    {
                        osk = Path.Combine(windir, "system32", "osk.exe");
                        if (!File.Exists(osk))
                        {
                            osk = "osk.exe"; // Último intento usando PATH
                        }
                    }
                }

                if (File.Exists(osk) || osk == "osk.exe")
                {
                    virtualKeyboard = Process.Start(osk);
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                        "Common.General.TecladoPantalla", "TecladoPantalla",
                        "No se encontró un teclado en pantalla válido.");
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                    "Common.General.TecladoPantalla", "TecladoPantalla",
                    "Error al iniciar el teclado: " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        public static void TecladoPantalla()
        {
            try
            {
                Version version = Environment.OSVersion.Version;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info,
                    "Common.General.TecladoPantalla", "TecladoPantalla", "Versión del sistema: " + version);

                if (version.Major == 10 && version.Build >= 22000)
                {
                    // Windows 11 - Abrir configuración de teclado táctil
                    Process.Start("cmd.exe", "/c start ms-settings:easeofaccess-keyboard");
                    return;
                }

                // Cerrar teclados existentes
                var processNamesToKill = new[] { "TabTip", "osk" };
                foreach (var name in processNamesToKill)
                {
                    foreach (var proc in Process.GetProcessesByName(name))
                    {
                        try
                        {
                            proc.Kill();
                            proc.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                                "Common.General.TecladoPantalla", "TecladoPantalla",
                                $"No se pudo cerrar el proceso {name}: {ex.Message}");
                        }
                    }
                }

                string oskPath = null;

                // Buscar en ubicaciones conocidas
                string inkPath = @"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe";
                if (File.Exists(inkPath))
                {
                    oskPath = inkPath;
                }
                else
                {
                    string system32 = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "System32");
                    string oskTest = Path.Combine(system32, "osk.exe");

                    if (File.Exists(oskTest))
                    {
                        oskPath = oskTest;
                    }
                    else
                    {
                        oskPath = "osk.exe"; // Último intento usando PATH
                    }
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info,
                    "Common.General.TecladoPantalla", "TecladoPantalla", "Ejecutando: " + oskPath);

                if (File.Exists(oskPath) || oskPath == "osk.exe")
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = oskPath,
                        UseShellExecute = true
                    };

                    Process.Start(startInfo);
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                        "Common.General.TecladoPantalla", "TecladoPantalla",
                        "No se encontró un teclado en pantalla válido.");
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                    "Common.General.TecladoPantalla", "TecladoPantalla",
                    "Error al iniciar el teclado: " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        public static void TecladoPantallav1()
        {
            System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();

            try
            {
                //System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();

                Version version = Environment.OSVersion.Version;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Common.General.TecladoPantalla"
                    , "TecladoPantalla"
                    , "version: " + version);

                if (version.Major == 10 && version.Build >= 22000)
                {
                    // Windows 11
                    Process.Start("cmd.exe", "/c start ms-settings:easeofaccess-keyboard");
                }
                else
                {
                    // Windows 10 o versiones anteriores
                    string windir = Environment.GetEnvironmentVariable("WINDIR");

                    string osk = null;
                    if (osk == null)
                    {
                        //osk = @"C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe";
                        osk = @"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe";
                        if (!File.Exists(osk))
                        {
                            osk = null;
                        }
                    }

                    if (osk == null)
                    {
                        osk = Path.Combine(Path.Combine(windir, "SysWOW64"), "osk.exe");
                        if (!File.Exists(osk))
                        {
                            osk = null;
                        }
                    }

                    if (osk == null)
                    {
                        osk = Path.Combine(Path.Combine(windir, "system32"), "osk.exe");
                        if (!File.Exists(osk))
                        {
                            osk = null;
                        }
                    }

                    if (osk == null)
                    {
                        osk = "osk.exe";
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Common.General.TecladoPantalla"
                    , "TecladoPantalla"
                    , "osk: " + osk);



                    //virtualKeyboard = System.Diagnostics.Process.Start(osk); // open
                    
                    bool isRunningTabTip = Process.GetProcessesByName("TabTip").Length > 0;
                    bool isRunningosk = Process.GetProcessesByName("osk").Length > 0;


                    if (isRunningTabTip || isRunningosk)
                    {
                        foreach (var proc in Process.GetProcessesByName("TabTip"))
                        {
                            proc.Kill();
                        }

                        foreach (var proc in Process.GetProcessesByName("osk"))
                        {
                            proc.Kill();
                        }
                    }
                    

                    if (File.Exists(osk))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = osk;
                        startInfo.UseShellExecute = true; // necesario para usar "Verb"
//                        startInfo.Verb = "runas";          // esto ejecuta como administrador
                        Process.Start(startInfo);
                    }
                }

                // virtualKeyboard

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Common.General.TecladoPantalla"
                    , "TecladoPantalla"
                    , "Error: " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }


        #region "region de mensajes dianicos"



        private static MensajesLibrary.MsgBoxCtrl msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
        private static MensajesLibrary.MsgBoxCtrl.MessageBoxResult respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();
        private static MensajesLibrary.MsgBoxCtrl.MessageType messageType = new MensajesLibrary.MsgBoxCtrl.MessageType();


        public static List<Mensajes> GetListMensaje()
        {
            Mensajes objMensaje = new Mensajes();
            List<Mensajes> mensajes = new List<Mensajes>();

            DataSet dtsConsulta = new DataSet();
            string query = string.Empty;
            string opcion = string.Empty;


            try
            {
                dtsConsulta = GetDataSetMensaje(0, opcion);
                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        mensajes = new List<Mensajes>();
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            objMensaje = new Mensajes();
                            objMensaje.id = Int32.Parse(data["id"].ToString());
                            objMensaje.opcion = data["opcion"].ToString();
                            objMensaje.descripcion = data["descripcion"].ToString();
                            objMensaje.titulo_mensaje = data["titulo_mensaje"].ToString();
                            objMensaje.tipo_mensaje = data["tipo_mensaje"].ToString();
                            objMensaje.tiempo_espera = Int32.Parse(data["tiempo_espera"].ToString());
                            objMensaje.activo = bool.Parse(data["activo"].ToString());


                            messageType = new MensajesLibrary.MsgBoxCtrl.MessageType();
                            switch (objMensaje.tipo_mensaje)
                            {
                                case "I":
                                    messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                                    break;
                                case "Q":
                                    messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Question;
                                    break;
                                case "W":
                                    messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Warning;
                                    break;
                                case "S":
                                    messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Stop;
                                    break;
                                case "ER":
                                    messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Error;
                                    break;
                                case "EX":
                                    messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Exclamation;
                                    break;
                                default:
                                    messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                                    break;

                            }

                            objMensaje.TipoMensaje = messageType;
                            mensajes.Add(objMensaje);
                        }
                    }
                }
                GlobalParameters.ListMensaje = mensajes;
                return mensajes;
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "GetListMensaje", " error al generar la lista de mensajes " + ex.Message);
                mensajes = new List<Mensajes>();
                return mensajes;
            }
        }

        public static List<Mensajes> GetListMensaje(int id)
        {

            Mensajes objMensaje = new Mensajes();
            List<Mensajes> ListMensajes = GlobalParameters.ListMensaje;
            List<Mensajes> mensajes = new List<Mensajes>();


            DataSet dtsConsulta = new DataSet();
            string query = string.Empty;
            string opcion = string.Empty;

            try
            {
                if (ListMensajes == null || ListMensajes.Count == 0)
                {
                    dtsConsulta = GetDataSetMensaje(id, opcion);
                    if (dtsConsulta.Tables.Count > 0)
                    {
                        if (dtsConsulta.Tables[0].Rows.Count > 0)
                        {
                            mensajes = new List<Mensajes>();
                            foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                            {
                                objMensaje = new Mensajes();
                                objMensaje.id = Int32.Parse(data["id"].ToString());
                                objMensaje.opcion = data["opcion"].ToString();
                                objMensaje.descripcion = data["descripcion"].ToString();
                                objMensaje.titulo_mensaje = data["titulo_mensaje"].ToString();
                                objMensaje.tipo_mensaje = data["tipo_mensaje"].ToString();
                                objMensaje.tiempo_espera = Int32.Parse(data["tiempo_espera"].ToString());
                                objMensaje.activo = bool.Parse(data["activo"].ToString());


                                messageType = new MensajesLibrary.MsgBoxCtrl.MessageType();
                                switch (objMensaje.tipo_mensaje)
                                {
                                    case "I":
                                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                                        break;
                                    case "Q":
                                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Question;
                                        break;
                                    case "W":
                                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Warning;
                                        break;
                                    case "S":
                                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Stop;
                                        break;
                                    case "ER":
                                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Error;
                                        break;
                                    case "EX":
                                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Exclamation;
                                        break;
                                    default:
                                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                                        break;

                                }

                                objMensaje.TipoMensaje = messageType;
                                mensajes.Add(objMensaje);
                            }


                            GlobalParameters.ListMensaje = mensajes;
                        }
                    }
                }
                else
                {

                    objMensaje = new Mensajes();
                    mensajes = (from deta in ListMensajes
                                where (deta.id == id || id ==0) && deta.activo == true
                                select deta).ToList();


                    foreach (var msj in mensajes)
                    {
                        messageType = new MensajesLibrary.MsgBoxCtrl.MessageType();

                        switch (msj.tipo_mensaje)
                        {
                            case "I":
                                messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                                break;
                            case "Q":
                                messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Question;
                                break;
                            case "W":
                                messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Warning;
                                break;
                            case "S":
                                messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Stop;
                                break;
                            case "ER":
                                messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Error;
                                break;
                            case "EX":
                                messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Exclamation;
                                break;
                            default:
                                messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                                break;

                        }

                        objMensaje.TipoMensaje = messageType;
                    }
                  

                    GlobalParameters.ListMensaje = mensajes;
                    
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "GetListMensaje", "Error: " + ex.Message); 
                

                ListMensajes = new List<Mensajes>();
                GlobalParameters.ListMensaje = new List<Mensajes>();

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MuestraMensaje", "", "Error al mostrar el mensaje " + ex.Message);
                return ListMensajes;
            }

            return mensajes;
        }


        private string ConectividadSharedTmpFile(string shared, string idPOS, string tipo)
        {
            bool recursoCompartidoDisponible = true;
            string txtFile = string.Empty;
            try
            {
                //bool recursoCompartidoDisponible = QuickBestGuessAboutAccessibilityOfNetworkPath(shared);                
                recursoCompartidoDisponible = Directory.Exists(@shared);
                if (recursoCompartidoDisponible)
                {
                    return shared + idPOS + tipo;
                }
                else
                {
                    GlobalParameters.DBIdCaja = GlobalParameters.DBIdCajaLocal;
                    return GlobalParameters.DBIdCajaLocal + idPOS + tipo;
                }

            }
            catch (Exception)
            {
                //Si existe error desconocido o no tratado, entonces que obtenga la ruta Local.
                GlobalParameters.DBIdCaja = GlobalParameters.DBIdCajaLocal;
                return GlobalParameters.DBIdCajaLocal + idPOS + tipo;
            }
        }


        public static void ConsultaDetalleMensajesTmpFile()
        {
            string linea;
            string fileName = GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "DetMensajes.txt";
            string fileText = @"C:\Log\POSInserts\" + fileName;
            List<Mensajes> mensajes = new List<Mensajes>();

            using (StreamReader ReaderObject = new StreamReader(fileText))
            {
                while ((linea = ReaderObject.ReadLine()) != null)
                {
                    string[] leer = linea.Split('|');
                    //buffer.AppendLine(String.Format("{0}||{1}||{2}||{3}||{4}||{5}||{6}"
                    //, deta.id, deta.descripcion, deta.titulo_mensaje, deta.tipo_mensaje
                    //, deta.tiempo_espera, deta.activo, deta.TipoMensaje))

                    bool isActive = false;
                    if (leer[5].ToString() == "True") { isActive = true; }


                    MensajesLibrary.MsgBoxCtrl.MessageType messageType = new MensajesLibrary.MsgBoxCtrl.MessageType();
                    switch (leer[5])
                    {
                        case "I":
                            messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                            break;
                        case "Q":
                            messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Question;
                            break;
                        case "W":
                            messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Warning;
                            break;
                        case "S":
                            messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Stop;
                            break;
                        case "ER":
                            messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Error;
                            break;
                        case "EX":
                            messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Exclamation;
                            break;
                        default:
                            messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                            break;

                    }


                    mensajes.Add(new Mensajes()
                    {
                        id = Int32.Parse(leer[0].ToString()),
                        descripcion = leer[1],
                        titulo_mensaje = leer[2],
                        tipo_mensaje = leer[3],
                        tiempo_espera = Int32.Parse(leer[4].ToString()),
                        activo = isActive,
                        TipoMensaje = messageType
                    });
                }
            }
        }


        public static void AgregaDetalleMensajesTmpFileXML(List<Mensajes> ListMensajes)
        {
            string tipoDet = string.Empty;
            string rutaInsert = @"C:\Log\POSInserts\";

            tipoDet = "DetMensajes.xml";
            //string textFile = GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "DetMensajes.txt";
            string textFile = string.Concat(GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoDet);
            string rutaCompleta = string.Concat(rutaInsert, textFile);

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "AgregaDetalleMensajesTmpFileXML", "Se ejecuta ");

            bool validaFechas = (!(File.GetCreationTime(rutaCompleta).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd").ToString()
               || File.GetLastWriteTime(rutaCompleta).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd").ToString()));


            if (File.Exists(rutaCompleta))
            {
                File.Delete(rutaCompleta);
            }

            if (!File.Exists(rutaCompleta))
            {
                var mensajes = (from deta in ListMensajes
                                select deta)
                                     .OrderBy(producto => producto.id)
                                     .ThenBy(prod => prod.descripcion).ToList();


                var archivoCreado = GuardarMensajesXML(mensajes, rutaCompleta);

            }
            else
            {
                var ListaArticulosXML = LeerXMLDetMensajes(rutaCompleta, "", "");

                if (validaFechas)
                {
                    General.EliminarArchivoXML(rutaCompleta);

                    var listMensajes = Control.Common.General.GetListMensaje();
                    var mensajes = (from deta in listMensajes
                                     select deta)
                                        .OrderBy(producto => producto.id)
                                        .ThenBy(prod => prod.descripcion).ToList();


                    var archivoCreado = GuardarMensajesXML(mensajes, rutaCompleta);
                }
                else
                {
                    AgregaMensajesFaltantes(rutaCompleta, MensajeslistaXML);
                }
            }

            MensajeslistaXML = new List<Mensajes>();
            var detMensajes = LeerXMLDetMensajes(rutaCompleta, "", "");
            MensajeslistaXML.AddRange(detMensajes);


        }

        private static bool GuardarMensajesXML(List<Mensajes> detmensajes, string fileText)
        {

            try
            {
             
                XElement xml = new XElement("DetalleMensajes",
               new XElement("Mensajes",
                   new List<XElement>(
                       detmensajes.ConvertAll(a => new XElement("Mensaje",
                           new XElement("id", a.id),
                           new XElement("descripcion", a.descripcion),
                           new XElement("opcion", a.opcion),
                           new XElement("titulo_mensaje", a.titulo_mensaje),
                           new XElement("tipo_mensaje", a.tipo_mensaje),
                           new XElement("tiempo_espera", a.tiempo_espera),
                           new XElement("activo", a.activo),
                           new XElement("fecha_creacion", a.fecha_creacion),
                           new XElement("fecha_modificacion", a.fecha_modificacion)
                       ))
                   )
               )
           );
                xml.Save(fileText);

                return true;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CreaXml", "Error: " + ex.Message);
                return false;
            }
        }


        public static void EliminarArchivoXML(string rutaXML)
        {
            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EliminarArchivoXML", "Inicia Metodo EliminarArchivoXML");

                if (File.Exists(rutaXML))
                {
                    File.Delete(rutaXML);
                    Console.WriteLine("Archivo XML eliminado.");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EliminarArchivoXML", "Archivo XML eliminado.");

                }
                else
                {
                    Console.WriteLine("El archivo XML no existe.");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EliminarArchivoXML", "El archivo XML no existe.");

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CreaXml", "Error: " + ex.Message);
                
            }

        }

        public static void AgregaMensajesFaltantes(string rutaXML, List<Mensajes> baseRegistros)
        {
            XElement xmlFile = null;

            try
            {
                xmlFile = XElement.Load(rutaXML);

            }
            catch (Exception ex)
            {
                
            }


           
            var DeMensajesXml = xmlFile.Element("Mensajes")?.Elements("Mensaje")
              .Select(x => new Mensajes
              {
                  id = (int)x.Element("id"),
                  descripcion = (string)x.Element("descripcion"),
                  opcion = (string)x.Element("opcion"),
                  titulo_mensaje = (string)x.Element("titulo_mensaje"),
                  tipo_mensaje = (string)x.Element("tipo_mensaje"),
                  tiempo_espera = (int?)x.Element("tiempo_espera") ?? 0,
                  activo = (bool)x.Element("activo"),
                  fecha_creacion = (DateTime)x.Element("fecha_creacion"),
                  fecha_modificacion = (DateTime)x.Element("fecha_modificacion"),
              }).ToList() ?? new List<Mensajes>();


            var faltantes = baseRegistros
               .Where(b => !DeMensajesXml.Any(x => x.id == b.id))
               .ToList();

            if (DeMensajesXml.Count == 0)
            {
                Console.WriteLine("No hay registros faltantes para agregar.");
                return;
            }


            var articulosElement = xmlFile.Element("Mensajes");
            if (articulosElement == null)
            {
                articulosElement = new XElement("Mensajes");
                xmlFile.Add(articulosElement);
            }

         
            foreach (var faltante in faltantes)
            {
                XElement nuevoArticulo = new XElement("Mensaje",
                    new XElement("id", faltante.id),
                    new XElement("descripcion", faltante.descripcion),
                    new XElement("opcion", faltante.opcion),
                    new XElement("titulo_mensaje", faltante.titulo_mensaje),
                    new XElement("tipo_mensaje", faltante.tipo_mensaje),
                    new XElement("tiempo_espera", faltante.tiempo_espera),
                    new XElement("activo", faltante.activo),
                    new XElement("fecha_creacion", faltante.fecha_creacion),
                    new XElement("fecha_modificacion", faltante.fecha_modificacion)

                );
                articulosElement.Add(nuevoArticulo);
            }

            // Actualizar ContRegistro
            xmlFile.SetElementValue("ContRegistro", DeMensajesXml.Count + faltantes.Count);
            xmlFile.Save(rutaXML);

            Console.WriteLine($"Se agregaron {faltantes.Count} registros faltantes al XML.");


        }
        public static List<Mensajes> LeerXMLDetMensajes(string rutaXML, string establecimiento, string campoConsulta)
        {
            if (!File.Exists(rutaXML))
            {
                Console.WriteLine("El archivo XML no existe.");
                return new List<Mensajes>();
            }

            List<Mensajes> lista = new List<Mensajes>();

            XElement xmlFile = XElement.Load(rutaXML);
            var articulos = xmlFile.Element("Mensajes")?.Elements("Mensaje");

            if (articulos != null)
            {
                foreach (var elemento in articulos)
                {
                    Mensajes articulo = new Mensajes
                    {
                        id = (int)elemento.Element("id"),
                        descripcion = (string)elemento.Element("descripcion"),
                        opcion = (string)elemento.Element("descripcion"),
                        titulo_mensaje = (string)elemento.Element("titulo_mensaje"),
                        tipo_mensaje = (string)elemento.Element("tipo_mensaje"),
                        tiempo_espera = (int)elemento.Element("tiempo_espera"),
                        activo = (bool)elemento.Element("activo"),
                        fecha_creacion = (DateTime)elemento.Element("fecha_creacion"),
                        fecha_modificacion = (DateTime)elemento.Element("fecha_modificacion"),

                    };
                    lista.Add(articulo);
                }
            }
       

            return lista ?? new List<Mensajes>();
        }
       
        private static DataSet GetDataSetMensaje(int id, string opcion)
        {

            DataSet dtsConsulta = new DataSet();
            string query = string.Empty;

            try
            {
                query = string.Empty;
                query = string.Concat("Select * from TblMensaje ");
                query = string.Concat(query, "where 1=1 ");
                if (id > 0) { query = string.Concat(query, " and id = ", id); }
                if (!string.IsNullOrEmpty(opcion)) { query = string.Concat(query, " and opcion = '", opcion, "'" ); }

                dtsConsulta = GetDataSet(query);


            }
            catch (Exception)
            {
                dtsConsulta = new DataSet();
                return dtsConsulta;
            }


            return dtsConsulta;

        }
       
        private static MensajesLibrary.MsgBoxCtrl SetMensaje(int id, List<ParametrosMensajes> listParametros)
        {

            Mensajes objMensaje = new Mensajes();
            List<Mensajes> ListMensajes = GlobalParameters.ListMensaje;
            List<Mensajes> mensajes = GlobalParameters.ListMensaje;
            DataSet dtsConsulta = new DataSet();
            string query = string.Empty;
            string opcion = string.Empty;

            try
            {
                if (ListMensajes == null || ListMensajes.Count == 0)
                {
                    dtsConsulta = GetDataSetMensaje(id, opcion);



                    if (dtsConsulta.Tables.Count > 0)
                    {
                        if (dtsConsulta.Tables[0].Rows.Count > 0)
                        {
                            ListMensajes = new List<Mensajes>();
                            foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                            {
                                objMensaje = new Mensajes();
                                objMensaje.id = Int32.Parse(data["id"].ToString());
                                objMensaje.opcion = data["opcion"].ToString();

                                foreach (var param in listParametros)
                                {
                                    string descripcion = data["descripcion"].ToString();

                                    if (descripcion.Contains(param.codigo))
                                    {
                                        descripcion.Replace("[CEDULA_CAJERO]", param.valor);
                                    }

                                }
                                objMensaje.descripcion = data["descripcion"].ToString();


                                objMensaje.titulo_mensaje = data["titulo_mensaje"].ToString();
                                objMensaje.tipo_mensaje = data["tipo_mensaje"].ToString();
                                objMensaje.tiempo_espera = Int32.Parse(data["tiempo_espera"].ToString());
                                objMensaje.activo = bool.Parse(data["activo"].ToString());
                                ListMensajes.Add(objMensaje);
                            }
                        }
                    }
                }
                else
                {

                    objMensaje = new Mensajes();
                    ListMensajes = (from deta in mensajes
                                    where deta.id == id && deta.activo == true
                                    select deta).ToList();

                    objMensaje = ListMensajes.FirstOrDefault();
                }




                switch (objMensaje.tipo_mensaje)
                {
                    case "I":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                        break;
                    case "Q":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Question;
                        break;
                    case "W":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Warning;
                        break;
                    case "S":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Stop;
                        break;
                    case "ER":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Error;
                        break;
                    case "EX":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Exclamation;
                        break;
                    default:
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                        break;

                }

                msgBoxCtrl.TipoMensaje = messageType;
                msgBoxCtrl.TituloMensaje = objMensaje.titulo_mensaje;
                msgBoxCtrl.TiempoEspera = objMensaje.tiempo_espera;
                msgBoxCtrl.TextMensaje = objMensaje.descripcion;
                msgBoxCtrl.MostrarContador = true;

            }
            catch (Exception ex)
            {
                //respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MuestraMensaje", "", "Error al mostrar el mensaje " + ex.Message);
            }

            return msgBoxCtrl;


        }
        private static MensajesLibrary.MsgBoxCtrl SetMensaje(string titulo_mensaje, string descripcion, string TipoMensaje, int tiempo_espera = 0)
        {

            Mensajes objMensaje = new Mensajes();
            List<Mensajes> ListMensajes = GlobalParameters.ListMensaje;
            List<Mensajes> mensajes = GlobalParameters.ListMensaje;

            try
            {
                objMensaje = new Mensajes();

                switch (TipoMensaje)
                {
                    case "I":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                        break;
                    case "Q":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Question;
                        break;
                    case "W":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Warning;
                        break;
                    case "S":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Stop;
                        break;
                    case "ER":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Error;
                        break;
                    case "EX":
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Exclamation;
                        break;
                    default:
                        messageType = MensajesLibrary.MsgBoxCtrl.MessageType.Information;
                        break;

                }

                msgBoxCtrl.TipoMensaje = messageType;
                msgBoxCtrl.TituloMensaje = titulo_mensaje;
                msgBoxCtrl.TiempoEspera = tiempo_espera;
                msgBoxCtrl.TextMensaje = descripcion;
                msgBoxCtrl.MostrarContador = true;
            }
            catch (Exception ex)
            {
                //respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MuestraMensaje", "", "Error al mostrar el mensaje " + ex.Message);
            }

            return msgBoxCtrl;


        }
     
        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensaje(string titulo_mensaje, string descripcion, string TipoMensaje, int tiempo_espera = 0)
        {

            msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
            respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();

            try
            {
                msgBoxCtrl = SetMensaje(titulo_mensaje,descripcion, TipoMensaje, 0);
                respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GetMensajeToList", $"error {ex.Message}");
                respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return respuestaMsj;
            }



        }
        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensaje(int id, List<ParametrosMensajes> listParametros)
        {

            msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
            respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();

            try
            {
                msgBoxCtrl = SetMensaje(id, listParametros);
                respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception)
            {
                respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return respuestaMsj;
            }



        }


        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensajeToList(int id, string opcion = "", string Mensaje = "", int activo = 1)
        {


            List<Mensajes> mensajes = new List<Mensajes>();

            try
            {
                bool isActivo = activo == 1 ? true : false;
                mensajes = GlobalParameters.ListMensaje;

                if (mensajes == null || mensajes.ToList().Count == 0)
                {
                    mensajes = GetListMensaje(id);
                    GlobalParameters.ListMensaje = mensajes;
                }

                if (id != 0)
                {
                    mensajes = (from deta in GlobalParameters.ListMensaje
                                where deta.id == id
                                && deta.activo == isActivo
                                select deta).ToList();
                }


                msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
                msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();

                respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();
                msgBoxCtrl.drawingFont.fontStyle = Common.GlobalParameters.fontStyle;
                msgBoxCtrl.drawingFont.fontFamily = Common.GlobalParameters.fontFamily;
                msgBoxCtrl.drawingFont.emSize = Common.GlobalParameters.emSize;


                foreach (var deta in mensajes)
                {
                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.MostrarContador = true;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;

                    msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl
                    {
                        fontStyle = Common.GlobalParameters.fontStyle,
                        fontFamily = Common.GlobalParameters.fontFamily,
                        emSize = Common.GlobalParameters.emSize
                    };

                    msgBoxCtrl.CodigoMensaje = id;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = deta.descripcion;
                    msgBoxCtrl.TextMensajeAdicional = string.Concat("Cod. Mensaje:", msgBoxCtrl.CodigoMensaje);

                    //msgBoxCtrl.TextMensajeAdicional = string.Concat("(Cod. Mensaje ", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                }
                respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception ex)
            {
                respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return respuestaMsj;
            }
        }
        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensajeToList(IWin32Window owner, int id, string opcion = "", string Mensaje = "", int activo = 1)
        {


            List<Mensajes> mensajes = new List<Mensajes>();

            try
            {

                bool isActivo = activo == 1 ? true : false;
                mensajes = GlobalParameters.ListMensaje;

                if (mensajes == null || mensajes.ToList().Count == 0)
                {
                    mensajes = GetListMensaje(id);
                    GlobalParameters.ListMensaje = mensajes;
                }

                if (id != 0)
                {
                    mensajes = (from deta in GlobalParameters.ListMensaje
                                where deta.id == id
                                && deta.activo == isActivo
                                select deta).ToList();
                }


                msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
                msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();

                respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();
                msgBoxCtrl.drawingFont.fontStyle = Common.GlobalParameters.fontStyle;
                msgBoxCtrl.drawingFont.fontFamily = Common.GlobalParameters.fontFamily;
                msgBoxCtrl.drawingFont.emSize = Common.GlobalParameters.emSize;


                foreach (var deta in mensajes)
                {
                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.MostrarContador = true;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;

                    msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl
                    {
                        fontStyle = Common.GlobalParameters.fontStyle,
                        fontFamily = Common.GlobalParameters.fontFamily,
                        emSize = Common.GlobalParameters.emSize
                    };

                    msgBoxCtrl.CodigoMensaje = id;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = deta.descripcion;
                    msgBoxCtrl.TextMensajeAdicional = string.Concat("Cod. Mensaje:", msgBoxCtrl.CodigoMensaje);

                    //msgBoxCtrl.TextMensajeAdicional = string.Concat("(Cod. Mensaje ", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                }


                respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception ex)
            {
                respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return respuestaMsj;
            }
        }
        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensajeToList(int id, List<ParametrosMensajes> listParametros, string textAdicional = "", int activo = 1)
        {
            List<Mensajes> mensajes = new List<Mensajes>();

            try
            {
                bool isActivo = activo == 1 ? true : false;
                mensajes = GlobalParameters.ListMensaje;
                if (mensajes == null || mensajes.ToList().Count == 0) { mensajes = GetListMensaje(id); }

                if (id != 0)
                {
                    mensajes = (from deta in GlobalParameters.ListMensaje
                                where deta.id == id
                                && deta.activo == isActivo
                                select deta).ToList();
                }

                msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
                respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();

                foreach (var deta in mensajes)
                {
                    var detParam = (from detaP in listParametros
                                    where deta.descripcion.Contains(detaP.codigo)
                                    select detaP).ToList();

                    string textomensaje = string.Empty;
                    textomensaje = deta.descripcion;

                    if (detParam.Count >= 1)
                    {
                        foreach (var det1 in detParam)
                        {
                            textomensaje = textomensaje.Replace(det1.codigo, det1.valor);
                        }
                    }


                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = textomensaje;
                    msgBoxCtrl.CodigoMensaje = id;

                    msgBoxCtrl.TextMensajeAdicional = string.Concat("(", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                    if (!string.IsNullOrEmpty(textAdicional)) { msgBoxCtrl.TextMensajeAdicional = string.Concat(msgBoxCtrl.TextMensajeAdicional, ": ", textAdicional); }


                    msgBoxCtrl.MostrarContador = true;
                    msgBoxCtrl.CodigoMensaje = id;

                    msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();
                    msgBoxCtrl.drawingFont.fontStyle = Common.GlobalParameters.fontStyle;
                    msgBoxCtrl.drawingFont.fontFamily = Common.GlobalParameters.fontFamily;
                    msgBoxCtrl.drawingFont.emSize = Common.GlobalParameters.emSize;

                }

                respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GetMensajeToList", $"error {ex.Message}");
                respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return respuestaMsj;
            }
        }

        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensajeToList(int id)
        {
            List<Mensajes> mensajes = new List<Mensajes>();
            List<ParametrosMensajes> listParametros = new List<ParametrosMensajes>();
            string textAdicional = "";
            int activo = 1;

            try
            {
                bool isActivo = activo == 1 ? true : false;
                mensajes = GlobalParameters.ListMensaje;
                if (mensajes == null || mensajes.ToList().Count == 0) { mensajes = GetListMensaje(id); }

                if (id != 0)
                {
                    mensajes = (from deta in GlobalParameters.ListMensaje
                                where deta.id == id
                                && deta.activo == isActivo
                                select deta).ToList();
                }

                msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
                respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();

                foreach (var deta in mensajes)
                {
                    var detParam = (from detaP in listParametros
                                    where deta.descripcion.Contains(detaP.codigo)
                                    select detaP).ToList();

                    string textomensaje = string.Empty;
                    textomensaje = deta.descripcion;

                    if (detParam.Count >= 1)
                    {
                        foreach (var det1 in detParam)
                        {
                            textomensaje = textomensaje.Replace(det1.codigo, det1.valor);
                        }
                    }


                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = textomensaje;
                    msgBoxCtrl.CodigoMensaje = id;

                    msgBoxCtrl.TextMensajeAdicional = string.Concat("(", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                    if (!string.IsNullOrEmpty(textAdicional)) { msgBoxCtrl.TextMensajeAdicional = string.Concat(msgBoxCtrl.TextMensajeAdicional, ": ", textAdicional); }


                    msgBoxCtrl.MostrarContador = true;
                    msgBoxCtrl.CodigoMensaje = id;

                    msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();
                    msgBoxCtrl.drawingFont.fontStyle = Common.GlobalParameters.fontStyle;
                    msgBoxCtrl.drawingFont.fontFamily = Common.GlobalParameters.fontFamily;
                    msgBoxCtrl.drawingFont.emSize = Common.GlobalParameters.emSize;

                }

                respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception)
            {
                respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return respuestaMsj;
            }
        }
        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensajeToList(int id, Screen targetScreen = null)
        {

            List<Mensajes> mensajes = new List<Mensajes>();
            MensajesLibrary.MsgBoxCtrl.MessageBoxResult respuestaMsj;
            MensajesLibrary.MsgBoxCtrl msgBoxCtrl;
            bool isActivo = true;

            try
            {
                mensajes = GlobalParameters.ListMensaje;

                if (mensajes == null || mensajes.ToList().Count == 0)
                {
                    mensajes = GetListMensaje(id);
                    GlobalParameters.ListMensaje = mensajes;
                }

                if (id != 0)
                {
                    mensajes = (from deta in GlobalParameters.ListMensaje
                                where deta.id == id
                                && deta.activo == isActivo
                                select deta).ToList();
                }


                msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
                msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();

                respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();
                msgBoxCtrl.drawingFont.fontStyle = Common.GlobalParameters.fontStyle;
                msgBoxCtrl.drawingFont.fontFamily = Common.GlobalParameters.fontFamily;
                msgBoxCtrl.drawingFont.emSize = Common.GlobalParameters.emSize;


                foreach (var deta in mensajes)
                {
                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.MostrarContador = true;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;

                    msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl
                    {
                        fontStyle = Common.GlobalParameters.fontStyle,
                        fontFamily = Common.GlobalParameters.fontFamily,
                        emSize = Common.GlobalParameters.emSize
                    };

                    msgBoxCtrl.CodigoMensaje = id;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = deta.descripcion;
                    msgBoxCtrl.TextMensajeAdicional = string.Concat("Cod. Mensaje:", msgBoxCtrl.CodigoMensaje);

                    //msgBoxCtrl.TextMensajeAdicional = string.Concat("(Cod. Mensaje ", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                }

                var form = new Form
                {
                    StartPosition = FormStartPosition.Manual,
                    FormBorderStyle = FormBorderStyle.None,
                    ShowInTaskbar = false,
                    TopMost = true
                };


                // Si no se pasa pantalla, usar la principal
                var screen = targetScreen ?? Screen.PrimaryScreen;


                form.Location = new Point(
                    screen.WorkingArea.Left + (screen.WorkingArea.Width - form.Width) / 2,
                    screen.WorkingArea.Top + (screen.WorkingArea.Height - form.Height) / 2);

                form.Load += (sender, e) =>
                {
                    respuestaMsj = msgBoxCtrl.ShowMessage();
                    form.DialogResult = DialogResult.OK;
                    form.Close(); // Cierra el formulario después de mostrar el mensaje
                };

                // Mostrar el formulario
                form.ShowDialog();

                // Asegúrate de liberar recursos después de usarlo
                form.Dispose(); // Opcional, pero recomendado si no se reutiliza
                return respuestaMsj;

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "Common",
                    "GetMensajeToList",
                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                    "StackTrace: " + ex.StackTrace);

                //respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
            }
        }

        public static async Task<MensajesLibrary.MsgBoxCtrl.MessageBoxResult> GetMensajeToListAsync(int id, Screen targetScreen = null)
        {
            var tcs = new TaskCompletionSource<MensajesLibrary.MsgBoxCtrl.MessageBoxResult>();

            List<Mensajes> mensajes = new List<Mensajes>();
            MensajesLibrary.MsgBoxCtrl.MessageBoxResult respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
            bool isActivo = true;

            try
            {
                // Obtener listado de mensajes
                mensajes = GlobalParameters.ListMensaje;

                if (mensajes == null || !mensajes.Any())
                {
                    mensajes = GetListMensaje(id);
                    GlobalParameters.ListMensaje = mensajes;
                }

                if (id != 0)
                {
                    mensajes = mensajes.Where(d => d.id == id && d.activo == isActivo).ToList();
                }

                if (!mensajes.Any())
                {
                    tcs.TrySetResult(respuestaMsj); // Devuelve "Cancel" si no hay mensajes
                    return await tcs.Task;
                }

                // Crear control de mensaje
                var msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl
                {
                    drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl
                    {
                        fontStyle = Common.GlobalParameters.fontStyle,
                        fontFamily = Common.GlobalParameters.fontFamily,
                        emSize = Common.GlobalParameters.emSize
                    }
                };

                foreach (var deta in mensajes)
                {
                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.MostrarContador = true;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;

                    msgBoxCtrl.CodigoMensaje = id;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = deta.descripcion;
                    msgBoxCtrl.TextMensajeAdicional = $"Cod. Mensaje: {msgBoxCtrl.CodigoMensaje}";
                }

                // Configurar formulario
                var form = new Form
                {
                    StartPosition = FormStartPosition.Manual,
                    FormBorderStyle = FormBorderStyle.None,
                    ShowInTaskbar = false,
                    TopMost = true
                };

                var screen = targetScreen ?? Screen.PrimaryScreen;
                form.Location = new Point(
                    screen.WorkingArea.Left + (screen.WorkingArea.Width - form.Width) / 2,
                    screen.WorkingArea.Top + (screen.WorkingArea.Height - form.Height) / 2);

                form.Load += async (sender, e) =>
                {
                    try
                    {
                        respuestaMsj = msgBoxCtrl.ShowMessage(); // Mostrar mensaje
                    }
                    finally
                    {
                        await Task.Delay(100); // Pequeño delay opcional
                        form.Close();
                        tcs.TrySetResult(respuestaMsj);
                    }
                };

                form.FormClosed += (s, args) => form.Dispose();

                form.Show(); // No bloquea el hilo principal

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "Common",
                    "GetMensajeToListAsync",
                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                    "StackTrace: " + ex.StackTrace);

                tcs.TrySetResult(MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel);
                return await tcs.Task;
            }
        }
        
        
        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensajeToList(int id, Screen targetScreen = null, List<ParametrosMensajes> listParametros = null)
        {

            List<Mensajes> mensajes = new List<Mensajes>();
            MensajesLibrary.MsgBoxCtrl.MessageBoxResult respuestaMsj;
            MensajesLibrary.MsgBoxCtrl msgBoxCtrl;
            bool isActivo = true;

            try
            {
                mensajes = GlobalParameters.ListMensaje;

                if (mensajes == null || mensajes.ToList().Count == 0)
                {
                    mensajes = GetListMensaje(id);
                    GlobalParameters.ListMensaje = mensajes;
                }

                if (id != 0)
                {
                    mensajes = (from deta in GlobalParameters.ListMensaje
                                where deta.id == id
                                && deta.activo == isActivo
                                select deta).ToList();
                }


                msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
                msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();

                respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();
                msgBoxCtrl.drawingFont.fontStyle = Common.GlobalParameters.fontStyle;
                msgBoxCtrl.drawingFont.fontFamily = Common.GlobalParameters.fontFamily;
                msgBoxCtrl.drawingFont.emSize = Common.GlobalParameters.emSize;


                foreach (var deta in mensajes)
                {
                    var detParam = (from detaP in listParametros
                                    where deta.descripcion.Contains(detaP.codigo)
                                    select detaP).ToList();

                    string textomensaje = string.Empty;
                    textomensaje = deta.descripcion;

                    if (detParam.Count >= 1)
                    {
                        foreach (var det1 in detParam)
                        {
                            textomensaje = textomensaje.Replace(det1.codigo, det1.valor);
                        }
                    }



                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.MostrarContador = true;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;

                    msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl
                    {
                        fontStyle = Common.GlobalParameters.fontStyle,
                        fontFamily = Common.GlobalParameters.fontFamily,
                        emSize = Common.GlobalParameters.emSize
                    };

                    msgBoxCtrl.CodigoMensaje = id;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = textomensaje;
                    msgBoxCtrl.TextMensajeAdicional = string.Concat("Cod. Mensaje:", msgBoxCtrl.CodigoMensaje);

                    //msgBoxCtrl.TextMensajeAdicional = string.Concat("(Cod. Mensaje ", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                }

                // Mostrar en una ventana modal en la pantalla deseada
                using (var form = new Form())
                {
                    form.StartPosition = FormStartPosition.Manual;

                    // Si no se pasa pantalla, usar la principal
                    var screen = targetScreen ?? Screen.PrimaryScreen;

                    // Centrar en la pantalla objetivo
                    form.Location = new Point(
                        screen.WorkingArea.Left + (screen.WorkingArea.Width - form.Width) / 2,
                        screen.WorkingArea.Top + (screen.WorkingArea.Height - form.Height) / 2);

                    form.FormBorderStyle = FormBorderStyle.None;
                    form.ShowInTaskbar = false;
                    form.TopMost = true;
                    form.Load += (sender, e) =>
                    {
                        // Cerrar formulario después de mostrar el mensaje
                        respuestaMsj = msgBoxCtrl.ShowMessage();
                        form.Close();
                    };

                    Application.Run(form); // Ejecutar contexto de formulario temporal
                }

                //respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "Common",
                    "GetMensajeToList",
                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                    "StackTrace: " + ex.StackTrace);

                //respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
            }
        }
        public static MensajesLibrary.MsgBoxCtrl.MessageBoxResult GetMensajeToList(int id, Screen targetScreen = null, List<ParametrosMensajes> listParametros = null, string textAdicional = "")
        {

            List<Mensajes> mensajes = new List<Mensajes>();
            MensajesLibrary.MsgBoxCtrl.MessageBoxResult respuestaMsj;
            MensajesLibrary.MsgBoxCtrl msgBoxCtrl;
            bool isActivo = true;

            try
            {
                mensajes = GlobalParameters.ListMensaje;

                if (mensajes == null || mensajes.ToList().Count == 0)
                {
                    mensajes = GetListMensaje(id);
                    GlobalParameters.ListMensaje = mensajes;
                }

                if (id != 0)
                {
                    mensajes = (from deta in GlobalParameters.ListMensaje
                                where deta.id == id
                                && deta.activo == isActivo
                                select deta).ToList();
                }


                msgBoxCtrl = new MensajesLibrary.MsgBoxCtrl();
                msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();

                respuestaMsj = new MensajesLibrary.MsgBoxCtrl.MessageBoxResult();
                msgBoxCtrl.drawingFont.fontStyle = Common.GlobalParameters.fontStyle;
                msgBoxCtrl.drawingFont.fontFamily = Common.GlobalParameters.fontFamily;
                msgBoxCtrl.drawingFont.emSize = Common.GlobalParameters.emSize;


                foreach (var deta in mensajes)
                {
                    var detParam = (from detaP in listParametros
                                    where deta.descripcion.Contains(detaP.codigo)
                                    select detaP).ToList();

                    string textomensaje = string.Empty;
                    textomensaje = deta.descripcion;

                    if (detParam.Count >= 1)
                    {
                        foreach (var det1 in detParam)
                        {
                            textomensaje = textomensaje.Replace(det1.codigo, det1.valor);
                        }
                    }



                    msgBoxCtrl.TipoMensaje = deta.TipoMensaje;
                    msgBoxCtrl.TiempoEspera = deta.tiempo_espera;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = textomensaje;

                    msgBoxCtrl.TextMensajeAdicional = string.Concat("(", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                    if (!string.IsNullOrEmpty(textAdicional)) { msgBoxCtrl.TextMensajeAdicional = string.Concat(msgBoxCtrl.TextMensajeAdicional, ": ", textAdicional); }

                    msgBoxCtrl.drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl
                    {
                        fontStyle = Common.GlobalParameters.fontStyle,
                        fontFamily = Common.GlobalParameters.fontFamily,
                        emSize = Common.GlobalParameters.emSize
                    };

                    msgBoxCtrl.CodigoMensaje = id;
                    msgBoxCtrl.TituloMensaje = deta.titulo_mensaje;
                    msgBoxCtrl.TextMensaje = deta.descripcion;
                    msgBoxCtrl.TextMensajeAdicional = string.Concat("Cod. Mensaje:", msgBoxCtrl.CodigoMensaje);

                    //msgBoxCtrl.TextMensajeAdicional = string.Concat("(Cod. Mensaje ", msgBoxCtrl.CodigoMensaje, "). ", msgBoxCtrl.TextMensaje);
                }

                // Mostrar en una ventana modal en la pantalla deseada
                using (var form = new Form())
                {
                    form.StartPosition = FormStartPosition.Manual;

                    // Si no se pasa pantalla, usar la principal
                    var screen = targetScreen ?? Screen.PrimaryScreen;

                    // Centrar en la pantalla objetivo
                    form.Location = new Point(
                        screen.WorkingArea.Left + (screen.WorkingArea.Width - form.Width) / 2,
                        screen.WorkingArea.Top + (screen.WorkingArea.Height - form.Height) / 2);

                    form.FormBorderStyle = FormBorderStyle.None;
                    form.ShowInTaskbar = false;
                    form.TopMost = true;
                    form.Load += (sender, e) =>
                    {
                        // Cerrar formulario después de mostrar el mensaje
                        respuestaMsj = msgBoxCtrl.ShowMessage();
                        form.Close();
                    };

                    Application.Run(form); // Ejecutar contexto de formulario temporal
                }

                //respuestaMsj = msgBoxCtrl.ShowMessage();

                return respuestaMsj;

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Error,
                    "Common",
                    "GetMensajeToList",
                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                    "StackTrace: " + ex.StackTrace);

                //respuestaMsj = MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
                return MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Cancel;
            }
        }

        
        #endregion "region de mensajes dianicos"


        public static Producto GetProductoSP(string consultaArituclo)
        {

            Producto producto = new Producto();
            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;

            try
            {

                squery = string.Empty;
                squery = string.Concat(squery, " Exec spDetConsultaDatosArticulo");
                squery = string.Concat(squery, $"  @CodigoArticuloBusq = '{consultaArituclo}'");
                dtsConsulta = GetDataSet(squery);

                if (dtsConsulta.Tables.Count > 0)
                {
                    producto = new Producto();
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            producto.Id = data["ITEMID"].ToString();
                            producto.Nombre = data["ITEMNAME"].ToString();
                            producto.Grupo = data["ITEMGROUPID"].ToString();
                            producto.Categoria = data["Categoria"].ToString();
                            producto.Variedad = data["VARIEDAD"].ToString();
                            producto.SubGrupo = data["SUBGRUPO"].ToString();
                            producto.Unidad = data["UNITID"].ToString();
                            producto.Costo = decimal.Parse(data["COST"].ToString());
                            producto.GrupoN = data["GRUPO"].ToString();
                            producto.RetencionPorcentaje = decimal.Parse(data["retporc"].ToString());
                            producto.PrecioAx = decimal.Parse(data["PRICE"].ToString());
                            producto.Cantidad = decimal.Parse(data["Cantidad"].ToString());
                            producto.CantidadINEC = decimal.Parse(data["CantidadINEC"].ToString());
                            producto.Unidades = Int32.Parse(data["Unidades"].ToString());
                            producto.IvaProducto = decimal.Parse(data["ivaProducto"].ToString());
                            producto.Iva = decimal.Parse(data["iva"].ToString());
                            //producto.IvaProducto= decimal.Parse(data["ivaProducto"].ToString());
                            //producto.iva= decimal.Parse(data["iva"].ToString());
                        }

                    }
                    


                }

                return producto;

            }
            catch (Exception)
            {
                producto = new Producto();
                return producto;
            }


            // return producto;

        }


        public static DataSet GetDataSetConsultaProducto(string establecimiento, string categoria, string valorConsulta)
        {
            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;


            try
            {
                squery = string.Empty;
                squery = string.Concat(squery, " begin try ", Environment.NewLine);
                squery = string.Concat(squery, "    Exec spConsultaProducto", Environment.NewLine);
                squery = string.Concat(squery, $"   @establecimiento = '{establecimiento}'", Environment.NewLine);
                squery = string.Concat(squery, $"   , @categoria = '{categoria}'", Environment.NewLine);
                squery = string.Concat(squery, $"   , @valorconsulta = '{valorConsulta}'", Environment.NewLine);
                squery = string.Concat(squery, " end try ", Environment.NewLine);
                squery = string.Concat(squery, " begin catch", Environment.NewLine);

                squery = string.Concat(squery, "    SELECT codError = ERROR_NUMBER() ", Environment.NewLine);
                squery = string.Concat(squery, "    , msjError = 'Error: ' + ERROR_MESSAGE() ", Environment.NewLine);
                squery = string.Concat(squery, "    , tipoConsulta = 'SearchProduct' ", Environment.NewLine);
                
                squery = string.Concat(squery, " end catch ", Environment.NewLine);


                dtsConsulta = GetDataSet(squery);
            }
            catch (Exception)
            {
                dtsConsulta = new DataSet();
                return dtsConsulta;
            }
            return dtsConsulta;
        }
       
        public static ProductoArticulos GetListConsultaProducto(string establecimiento, string categoria, string valorConsulta)
        {
            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;

            ProductoArticulo Items = new ProductoArticulo();
            List<ProductoArticulo> listArticulos = new List<ProductoArticulo>();

            ProductoArticulos _searchProduct = new ProductoArticulos();
            List<ProductoArticulos> ListProduct = new List<ProductoArticulos>();

            try
            {

                dtsConsulta = GetDataSetConsultaProducto(establecimiento, categoria, valorConsulta);


                if (dtsConsulta.Tables.Count > 0)
                {
                    int ContRegistro = 0;
                    for (int indexTable = 0 ; indexTable <= dtsConsulta.Tables.Count - 1; indexTable++)
                    {
                        int RowsCount = dtsConsulta.Tables[indexTable].Rows.Count;
                        string tipoConsulta = string.Empty;

                        if (RowsCount > 0)
                        {
                            tipoConsulta = dtsConsulta.Tables[indexTable].Rows[0]["tipoConsulta"].ToString();

                            if (tipoConsulta == "SearchProduct")
                            {
                                ContRegistro = Int32.Parse(dtsConsulta.Tables[indexTable].Rows[0]["contResultados"].ToString());
                                continue;
                            }

                            if (tipoConsulta == "SearchProductList")
                            {
                                _searchProduct = new ProductoArticulos();

                                if (ContRegistro == 0)
                                {
                                    _searchProduct.CodError = -1;
                                    _searchProduct.MsjError = "No se han encontrado Articulos / Items / Productos. ";
                                    _searchProduct.ProductoArticuloList = new List<ProductoArticulo>();
                                    continue;
                                }

                                foreach (DataRow data in dtsConsulta.Tables[indexTable].Rows)
                                {
                                    try
                                    {
                                        Items = new ProductoArticulo();
                                        Items.ARTICULO = data["ARTICULO"].ToString();
                                        Items.BARRAS = data["BARRAS"].ToString();
                                        Items.CATEGORIA = data["CATEGORIA"].ToString();
                                        Items.ESTABLECIMIENTO = data["ESTABLECIMIENTO"].ToString();
                                        Items.ORDEN = Int32.Parse(data["ORDEN"].ToString());
                                        Items.campoConsulta = data["campoConsulta"].ToString();
                                        listArticulos.Add(Items);
                                    }
                                    catch (Exception)
                                    {

                                        continue;
                                    }
                                    
                                }

                                _searchProduct.CodError = 0;
                                _searchProduct.MsjError = "Se han encontrado " + ContRegistro.ToString();
                                _searchProduct.ProductoArticuloList = listArticulos;

                            }

                        }
                      
                    }

                }

               
            }
            catch (Exception ex)
            {

                _searchProduct.CodError = -2;
                _searchProduct.MsjError = "No se han encontrado Articulos / Items / Productos. " + ex.Message;
                _searchProduct.ProductoArticuloList = new List<ProductoArticulo>();

                return _searchProduct;
            }

            return _searchProduct;
        }



        
        public static DataSet GetDataSetDescuentoSP(string Cliente, string establecimiento
            , string puntoEmision,  string EsPortalTC, int TCAdicionalInterna,    string ITEMID)
        {

            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;


            try
            {
                squery = string.Empty;
                squery = string.Concat(squery, " Exec spDetConsultaDescuento");
                squery = string.Concat(squery, $" @Cliente = '{Cliente}'");
                squery = string.Concat(squery, $" , @establecimiento = '{establecimiento}'");
                squery = string.Concat(squery, $" , @ItemId = '{ITEMID}'");
                squery = string.Concat(squery, $" , @TCAdicionalInterna = '{TCAdicionalInterna}'");
                squery = string.Concat(squery, $" , @EsPortalTC = '{EsPortalTC}'");
                dtsConsulta = GetDataSet(squery);



            }
            catch (Exception)
            {

                throw;
            }


            return dtsConsulta;

        }

        public static DataSet InsertaAcutalizaCltePOS(pos_customer datosClte)
        {
            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;

            try
            {

                squery = string.Concat(squery, "exec sp_InsertUpdateCliente", Environment.NewLine);
                squery = string.Concat(squery, $"@ACCOUNTNUM = '{datosClte.ACCOUNTNUM}'",  Environment.NewLine);
                squery = string.Concat(squery, $",@NAME = '{datosClte.NAME}'", Environment.NewLine);
                squery = string.Concat(squery, $",@ADDRESS = '{datosClte.ADDRESS}'", Environment.NewLine);
                squery = string.Concat(squery, $",@PHONE = '{datosClte.PHONE}'", Environment.NewLine);
                squery = string.Concat(squery, $",@EMAIL = '{datosClte.EMAIL}'", Environment.NewLine);
                squery = string.Concat(squery, $",@CELLULARPHONE = '{datosClte.CELLULARPHONE}'", Environment.NewLine);
                squery = string.Concat(squery, $",@PHONELOCAL = '{datosClte.CELLULARPHONE}'", Environment.NewLine);
                squery = string.Concat(squery, $",@STREET = '{datosClte.ADDRESS}'", Environment.NewLine);
                squery = string.Concat(squery, $",@COUNTRYREGIONID = '{datosClte.COUNTRYREGIONID}'", Environment.NewLine);
                dtsConsulta = GetDataSet(squery);
                return dtsConsulta;


            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ClienteForm", "InsertaAcutalizaCltePOS", "A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);

                dtsConsulta = new DataSet();
                return dtsConsulta;
            }
            
            
        }

        public static DataSet AcutalizaClteAX(string identificacion)
        {

            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;

            try
            {

                squery = string.Concat(squery, "exec sp_ActualizaDatosAXS", Environment.NewLine);
                squery = string.Concat(squery, $"@ACCOUNTNUM = '{identificacion}'", Environment.NewLine);
                
                dtsConsulta = GetDataSet(squery);
                return dtsConsulta;


            }
            catch (Exception)
            {
                dtsConsulta = new DataSet();
                return dtsConsulta;
            }
        }

        public static async Task<DataSet> ActualizaClteAXAsync(string identificacion)
        {
            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;

            try
            {
                squery = $"exec sp_ActualizaDatosAXS @ACCOUNTNUM = '{identificacion}'";

                dtsConsulta = await GetDataSetAsync(squery);
                return dtsConsulta;
            }
            catch (Exception)
            {
                return new DataSet(); // Retorna un dataset vacío en caso de error
            }
        }

        public static Screen GetScreenCajero()
        {
            try
            {
                var screens = Screen.AllScreens;

                // Si solo hay una pantalla, devolver la única disponible
                if (screens.Length <= 1)
                {
                    return Screen.PrimaryScreen;
                }

                // Opción 1: Usar PrimaryScreen si se espera que siempre sea la del cajero
                return Screen.PrimaryScreen;
            }
            catch (Exception ex)
            {
                // Registrar error completo
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                    "POS.Control.Common.General", "GetScreenCajero",
                    $"Error al obtener la pantalla del cajero: {ex.Message}\n{ex.StackTrace}");

                // En caso de fallo, devolver la pantalla principal como respaldo
                return Screen.PrimaryScreen;
            }
        }
        public static Screen GetScreenClte()
        {
            try
            {
                // Obtener todas las pantallas conectadas
                var pantallas = Screen.AllScreens;

                // Si solo hay una pantalla, usar la principal
                if (pantallas.Length <= 1 || !Control.Common.GlobalParameters.PANTALLA_CLIENTE)
                {
                    return Screen.PrimaryScreen;
                }

                string targetDeviceName = Control.Common.GlobalParameters.DeviceName?.Trim();
                int targetWidth = Control.Common.GlobalParameters.targetWidth;
                int targetHeight = Control.Common.GlobalParameters.targetHeight;

                // Registrar inicio de búsqueda
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info,
                    "POS.Control.Common.General", "GetScreenClte",
                    "Buscando pantalla del cliente...");

                // Buscar por DeviceName primero
                Screen targetScreen = null;

                if (!string.IsNullOrEmpty(targetDeviceName))
                {
                    targetScreen = pantallas.FirstOrDefault(s => s.DeviceName.Equals(targetDeviceName, StringComparison.OrdinalIgnoreCase));
                }

                // Si no se encontró por DeviceName, intentar por resolución
                if (targetScreen == null && targetWidth > 0 && targetHeight > 0)
                {
                    targetScreen = pantallas.FirstOrDefault(s =>
                        s.Bounds.Width == targetWidth && s.Bounds.Height == targetHeight);
                }

                // Fallback: usar la última pantalla disponible
                if (targetScreen == null)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warn,
                        "POS.Control.Common.General", "GetScreenClte",
                        "No se encontró la pantalla por DeviceName ni resolución. Usando última pantalla disponible.");

                    targetScreen = pantallas.LastOrDefault();
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info,
                        "POS.Control.Common.General", "GetScreenClte",
                        $"Pantalla encontrada: {targetScreen.DeviceName} - {targetScreen.Bounds}");
                }

                return targetScreen ?? Screen.PrimaryScreen;
            }
            catch (Exception ex)
            {
                // Loggear el error completo
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                    "POS.Control.Common.General", "GetScreenClte",
                    $"Error al obtener la pantalla del cliente: {ex.Message}\n{ex.StackTrace}");

                // En caso de error, regresar la pantalla principal
                return Screen.PrimaryScreen;
            }
        }

        public static Rectangle GetRectangleClte()
        {
            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Common.General", "GetRectangleClte", "Iniciando detección de pantalla de cliente.");

                var pantallas = Screen.AllScreens;
                Screen targetScreen = null;

                // --- Manejo de casos sencillos primero (Guard Clauses) ---

                // CASO 1: La funcionalidad de pantalla de cliente está desactivada.
                if (!Control.Common.GlobalParameters.PANTALLA_CLIENTE)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Common.General", "GetRectangleClte", "La función de pantalla de cliente está desactivada. Usando pantalla principal.");
                    targetScreen = Screen.PrimaryScreen;
                }
                // CASO 2: Solo hay una pantalla conectada.
                else if (pantallas.Length <= 1)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Common.General", "GetRectangleClte", "Solo se detectó una pantalla. Usando pantalla principal.");
                    targetScreen = Screen.PrimaryScreen;
                }
                // CASO 3: Hay múltiples pantallas, procedemos a buscar la correcta.
                else
                {
                    string targetDeviceName = Control.Common.GlobalParameters.DeviceName;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Common.General", "GetRectangleClte", $"Buscando pantalla con DeviceName: '{targetDeviceName}'");

                    // Intenta encontrar la pantalla por su nombre de dispositivo (método principal y preferido).
                    targetScreen = pantallas.FirstOrDefault(screen => screen.DeviceName == targetDeviceName);

                    // Si no se encontró por nombre, aplicamos un fallback más seguro.
                    if (targetScreen == null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warn, "POS.Control.Common.General", "GetRectangleClte", "No se encontró la pantalla por DeviceName. Aplicando fallback.");

                        // Fallback seguro: buscar la primera pantalla que NO sea la primaria.
                        targetScreen = pantallas.FirstOrDefault(s => !s.Primary);

                        // Si todos los fallbacks fallan, usar la primaria como último recurso.
                        if (targetScreen == null)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Common.General", "GetRectangleClte", "Fallback falló. No se encontró pantalla secundaria. Usando pantalla principal.");
                            targetScreen = Screen.PrimaryScreen;
                        }
                    }
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Common.General", "GetRectangleClte", $"Pantalla seleccionada: {targetScreen.DeviceName}, Primaria: {targetScreen.Primary}");

                // Devuelve el área de trabajo si es válida, si no, los límites completos.
                return targetScreen.WorkingArea.Width > 0 && targetScreen.WorkingArea.Height > 0
                       ? targetScreen.WorkingArea
                       : targetScreen.Bounds;
            }
            catch (Exception ex)
            {
                // El bloque catch ahora es un seguro final para cualquier error imprevisto.
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Fatal, "POS.Control.Common.General", "GetRectangleClte", $"Excepción inesperada: {ex.Message}. Usando pantalla principal como emergencia.");
                return Screen.PrimaryScreen.WorkingArea;
            }


        }



        public static DataSet InsertaGiftCard(  string codigo,  decimal saldo,   decimal valor)
        {

            string squery = string.Empty;
            squery = string.Concat(squery, "BEGIN TRY ", Environment.NewLine);
            squery = string.Concat(squery, "Insert into core_giftcard (", Environment.NewLine);
            squery = string.Concat(squery, "fecha_creacion,fecha_modificacion,fecha_activacion,fecha_expiracion ", Environment.NewLine);
            squery = string.Concat(squery, ",activo, bono , codigo", Environment.NewLine);

            squery = string.Concat(squery, ",tipoTransaccionId)", Environment.NewLine);

            squery = string.Concat(squery, "Select fecha_creacion = getdate()", Environment.NewLine);
            squery = string.Concat(squery, ", fecha_modificacion = getdate()", Environment.NewLine);
            squery = string.Concat(squery, ", fecha_activacion = getdate()", Environment.NewLine);
            squery = string.Concat(squery, ", fecha_expiracion = dateadd(year, 1, getdate()) ", Environment.NewLine);
            squery = string.Concat(squery, ", activo = 1 ", Environment.NewLine);
            squery = string.Concat(squery, ", bono = 1 ", Environment.NewLine);
            //squery = string.Concat(squery, $", codigo = '{"000" + lblSecuenciaNC.Text.Replace(" - ", "")}'", Environment.NewLine);
            //squery = string.Concat(squery, $", saldo = {decimal.Parse(lblTotSel.Text)}", Environment.NewLine);
            //squery = string.Concat(squery, $", valor = {decimal.Parse(lblTotSel.Text)}", Environment.NewLine);

            squery = string.Concat(squery, $", codigo = '{codigo}'", Environment.NewLine);
            squery = string.Concat(squery, $", saldo = '{saldo}'", Environment.NewLine);
            squery = string.Concat(squery, $", valor = '{valor}'", Environment.NewLine);

            
            squery = string.Concat(squery, "  Select CodError = 0, MsjError = 'GiftCardRegsitrada'  ", Environment.NewLine);

            squery = string.Concat(squery, "END TRY ", Environment.NewLine);
            squery = string.Concat(squery, "begin catch", Environment.NewLine);
            squery = string.Concat(squery, "  Select CodError = ERROR_NUMBER(), MsjError = ERROR_MESSAGE() ", Environment.NewLine);
            squery = string.Concat(squery, "end catch", Environment.NewLine);

            DataSet dtsConsulta = GetDataSet(squery);
            return dtsConsulta;
        }

        public decimal recuperaSaldoBilletera(string identificacion, string cadenaCon)
        {
            decimal saldoPuntos = 0;
            decimal salboBilletera = 0;

            if (cadenaCon != "")
            {
                SqlConnection conn = new SqlConnection(cadenaCon);
                try
                {
                    SqlParameter paramResult = new SqlParameter("@respuesta", SqlDbType.VarChar, -1);
                    paramResult.IsNullable = true;
                    paramResult.Direction = System.Data.ParameterDirection.Output;
                    var addParameters = new List<SqlParameter>
                                 {
                                    new SqlParameter("@AccountNum", identificacion),
                                    paramResult
                                 };
                    SqlCommand select = new SqlCommand("Exec PtsCliente.spValidarHistoricoPuntosGen @AccountNum, @respuesta out", conn);
                    select.Parameters.AddRange(addParameters.ToArray());
                    conn.Open();
                    select.ExecuteNonQuery();
                    conn.Close();
                    string saldoPuntos_ = (string)paramResult.Value;
                    decimal.TryParse(paramResult.Value.ToString(), out saldoPuntos);

                    return saldoPuntos;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }


            return saldoPuntos;
        }


        public static Mensajes GetMensajePtosClientes(string identificacion)
        {
            Mensajes detMensaje = new Mensajes();
            DataSet dtsConsulta = new DataSet();
            string squery = string.Empty;
            int idMensaje = -1;

            try
            {

                squery = string.Empty;
                squery = string.Concat(squery, " Exec spConsultaMsjsPorCltes");
                squery = string.Concat(squery, $"  @AccountNum = '{identificacion}'");
                dtsConsulta = GetDataSet(squery);

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            idMensaje = Int32.Parse(data["idMensaje"].ToString());
                            detMensaje = new Mensajes();

                            if (idMensaje != -1)
                            {
                                detMensaje.id = idMensaje;
                                detMensaje.descripcion = data["text_mensaje"].ToString();
                                detMensaje.titulo_mensaje = data["titulo_mensaje"].ToString();
                                detMensaje.tipo_mensaje = data["tipo_mensaje"].ToString();
                                detMensaje.tiempo_espera = Int32.Parse(data["tiempo_espera"].ToString());
                            }
                            else
                            {
                                detMensaje.id = idMensaje;
                                detMensaje.descripcion = "";

                            }

                        }

                    }
                }

                return detMensaje;

            }
            catch (Exception)
            {
                detMensaje = new Mensajes();
                detMensaje.id = -1;

                return detMensaje;
            }


            // return producto;

        }


        public static string EnmascararTexto(string texto, int ultimosVisible = 4)
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            int largo = texto.Length;
            int visibleDesde = Math.Max(0, largo - ultimosVisible);

            // Si hay menos de X caracteres, no enmascarar
            if (largo <= ultimosVisible)
                return texto;

            string parteOculta = new string('*', visibleDesde);
            string parteVisible = texto.Substring(visibleDesde);

            return parteOculta + parteVisible;
        }
        public static string EnmascararTexto(string texto, int ultimosVisible = 4, char textoMascara='*')
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            int largo = texto.Length;
            int visibleDesde = Math.Max(0, largo - ultimosVisible);

            // Si hay menos de X caracteres, no enmascarar
            if (largo <= ultimosVisible)
                return texto;


            string parteOculta = new string((char)textoMascara, visibleDesde);
            string parteVisible = texto.Substring(visibleDesde);

            
            return parteOculta + parteVisible;
        }




        //public static void IniciarPantallaCliente()
        //{
        //    // Verificar que haya al menos 3 pantallas conectadas
        //    Screen[] screens = Screen.AllScreens;

        //    if (screens.Length < 3)
        //    {
        //        MessageBox.Show($"Solo hay {screens.Length} pantallas conectadas. Se requieren al menos 3.", "Error de Pantalla");
        //        return;
        //    }

        //    try
        //    {
        //        // Seleccionar la tercera pantalla (índice 2)
        //        Screen targetScreen = screens[2];

        //        // Crear y configurar el formulario
        //        Form frmMainTouch = new Control.Main.frmMainTouch();
        //        frmMainTouch.StartPosition = FormStartPosition.Manual;
        //        frmMainTouch.Location = targetScreen.WorkingArea.Location;
        //        frmMainTouch.WindowState = FormWindowState.Maximized;

        //        // Mostrar el formulario
        //        frmMainTouch.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Ocurrió un error al abrir el formulario: " + ex.Message);
        //    }


        //}

        /// <summary>
        /// Determina si el error permite reintento automático JCHID 
        /// </summary>
        /// <param name="ex">Exception a evaluar</param>
        /// <returns>True si permite reintento</returns>
        public static bool EsErrorQuePermiteReintento(Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    // CÓDIGOS CONFIRMADOS DE SQL SERVER
                    case -2:        // Timeout (el más común en tu caso)
                    case 2:         // Network name not found
                    case 53:        // Network path not found  
                    case 1205:      // Deadlock victim
                    case 18456:     // Login failed (puede ser temporal)
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EsErrorQuePermiteReintento",
                            $"Error SQL confirmado {sqlEx.Number} permite reintento: {sqlEx.Message}");
                        return true;

                    // CÓDIGOS DE RED (Windows Socket Errors)
                    case 10054:     // Connection reset by peer
                    case 10060:     // Connection timeout
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EsErrorQuePermiteReintento",
                            $"Error de red {sqlEx.Number} permite reintento: {sqlEx.Message}");
                        return true;

                    // OTROS CÓDIGOS COMUNES DE CONECTIVIDAD
                    case 4060:      // Cannot open database (temporal)
                    case 18452:     // Cannot open database (network issue)
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EsErrorQuePermiteReintento",
                            $"Error de conectividad {sqlEx.Number} permite reintento: {sqlEx.Message}");
                        return true;

                    default:
                        // Para códigos no conocidos, registrar para análisis futuro
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EsErrorQuePermiteReintento",
                            $"Error SQL desconocido {sqlEx.Number} NO permite reintento: {sqlEx.Message}");
                        return false;
                }
            }

            // Para otras excepciones (no SqlException), evaluar por mensaje
            if (ex.Message.ToLower().Contains("timeout") ||
                ex.Message.ToLower().Contains("network") ||
                ex.Message.ToLower().Contains("connection"))
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EsErrorQuePermiteReintento",
                    $"Error general de conectividad permite reintento: {ex.Message}");
                return true;
            }

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EsErrorQuePermiteReintento",
                $"Error NO permite reintento: {ex.GetType().Name} - {ex.Message}");
            return false;
        }


        /// <summary>
        /// Método GetDataSet específico para billetera electrónica que permite reintentos automáticos JCHID
        /// </summary>
        /// <param name="query">Consulta SQL a ejecutar</param>
        /// <param name="connectionStrings">Cadena de conexión</param>
        /// <returns>DataSet con los resultados</returns>
        public static DataSet GetDataSetBilletera(string query, string connectionStrings)
        {
            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();

                        // *** TIMEOUT CONFIGURADO PARA PERMITIR DETECCIÓN ***
                        cmd.CommandTimeout = 5; // 5 segundos (no infinito como el original)

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "GetDataSetBilletera", "GetDataSetBilletera",
                            $"Ejecutando consulta billetera con timeout 30s");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "GetDataSetBilletera", "GetDataSetBilletera",
                            $"Consulta exitosa - Tablas: {ds.Tables.Count}, Filas: {(ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0)}");

                        return ds;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // *** LOGEAR ERROR ESPECÍFICO PARA DIAGNÓSTICO ***
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "GetDataSetBilletera", "GetDataSetBilletera",
                    $"SqlException - Código: {sqlEx.Number} - Mensaje: {sqlEx.Message}");

                // *** PROPAGAR EXCEPCIONES QUE PERMITEN REINTENTO ***
                // Esto permite que tu código de reintentos funcione correctamente
                throw;
            }
            catch (Exception ex)
            {
                // *** LOGEAR OTROS ERRORES ***
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "GetDataSetBilletera", "GetDataSetBilletera",
                    $"Exception general: {ex.Message}");

                // *** PROPAGAR EXCEPCIONES DE CONECTIVIDAD ***
                throw;
            }
        }

    }
}
