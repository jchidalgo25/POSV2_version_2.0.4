using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public static class ClsConeccionPersistente
    {
        public static string cadena = "name=POSEntities";
        public static int error = 0;
        public static string password = "";
        public static string cadenaGeneral = "";
        public static string currentConnectionString = "";
        public static POSEntities dbpersistente = new POSEntities();
        
        // static POSEntities dbpersistente = new POSEntities();
        public static string ReConnecction()
        {
            try
            {
                if (error >= 1 && error <= 7)
                {
                    cadena = "POSEntities" + error.ToString();
                    dbpersistente = new POSEntities(cadena);                    
                    cadenaGeneral = dbpersistente.Database.Connection.ConnectionString;

                    SetReconnection(dbpersistente);
                }
                else
                {
                    cadenaGeneral = currentConnectionString;
                }
                if (dbpersistente.core_banco.Any(x => x.id == -1))
                {
                    error = 0;
                }

                error = 0;
                //actualizar todas la ip-s del ambiente.
               
            }
            catch (Exception ex)
            {
                error++;
                if (error <= 7)
                {
                    ReConnecction();
                }
            }
            //finally
            //{
            //  dbpersistente.Dispose();

            //}
            return cadena;

        }
        public static string getServerName(string ConnectionString)
        {
            string serverName = "";
            string[] parts = ConnectionString.Split(';');

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();
                if (part.StartsWith("Data Source="))
                {
                    serverName = part.Replace("Data Source=", "");
                    break;
                }
            }

            return serverName;

        }

        /// <summary>
        /// Setea Cadena de Connection al nuevo servidor Conectado.
        /// </summary>
        /// <param name="POSEntity"></param>
        public static void SetReconnection(POSEntities POSEntity)
        {
            string ip = "";
            string conn_conectaAx = POS.Properties.Resources.CONECTA_AX_PLANTILLA;
            string conn_conectaMkt = POS.Properties.Resources.CONECTA_MKT_PLANTILLA;
            string conn_posEntities = POS.Properties.Resources.ConectaDB;
            try
            {
                string dsPOSConnected = POSEntity.Database.Connection.DataSource;
                switch (dsPOSConnected)
                {                

                    case "srv-pos-pia":
                        ip = "192.168.128.7";
                        break;
                    case "srv-pos-ore":
                        ip = "192.168.126.7";
                        break;
                    case "srv-pos-alb":
                        ip = "192.168.10.7";
                        break;
                    case "srv-pos":
                        ip = "192.168.127.13";
                        break;
                    case "srv-pos-ppg":
                        ip = "192.168.124.7";
                        break;
                    case "srv-pos-gr":
                        ip = "192.168.123.8";
                        break;
                    case "srv-pos-vc":
                        ip = "192.168.132.8";
                        break;
                    case "srv-pos-garcia":
                        ip = "192.168.137.5";
                        break;

                    case "srv-test2":
                        conn_posEntities = ConfigurationManager.ConnectionStrings["POSEntitiesTEST"].ConnectionString;
                        conn_conectaAx = POS.Properties.Resources.CONECTA_AX_PLANTILLA_TEST;
                        ip = "192.168.127.17";
                        break;

                    case "srv-test3":
                        ip = "192.168.130.19";
                        conn_posEntities = ConfigurationManager.ConnectionStrings["POSEntitiesTEST"].ConnectionString;
                        conn_conectaAx = POS.Properties.Resources.CONECTA_AX_PLANTILLA_TEST;
                        break;

                    //evelasco .ini 2019-11-21
                    case "ANIBAL IP":
                        ip = "192.168.131.107";
                        break;
                    case "LOCALHOST":
                        ip = "localhost";
                        break;
                        //evelasco .fin 2019-11-21
                }

                POS.Properties.Settings.Default.ROOT_URL = POS.Properties.Resources.ROOT_URL_PLANTILLA.Replace("[ipserver]", ip);
                POS.Properties.Settings.Default.ROOT_URL_GENERAL = POS.Properties.Resources.ROOT_URL_GENERAL_PLANTILLA.Replace("[ipserver]", ip);
                POS.Properties.Settings.Default.CONECTA_AX = conn_conectaAx.Replace("[ipserver]", ip);
                POS.Properties.Settings.Default.CONECTA_MKT = conn_conectaMkt.Replace("[ipserver]", ip);

                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();

                SaveConnectionString("POSEntities", conn_posEntities.Replace("[ipserver]", ip));

                Control.Common.GlobalParameters.DecriptedPwd = string.Empty;
                // MessageBox.Show(this, "Reinicie la Aplicacion");

                //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "SeleccionaLocal", "btnSave_Click", "Se solicitó cambio de servidor, debe reiniciarse la Aplicacion. La nueva ip será '" + ip + "'", string.Empty);
                MainWindow.IPServidorConectado = ip;
                MainWindow.IsServerChanged = 1;
                //MainWindowV1.IpServerConnectedTitleForm().Text= ip;


            }
            catch (Exception)
            {
                throw;
            }
        }


        private static void SaveConnectionString(string connectionStringName, string connectionString)
        {
            Configuration appconfig =
                ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
            //System.Reflection.Assembly.GetEntryAssembly().Location/ConfigurationUserLevel.None);// PerUserRoamingAndLocal);//Application.ExecutablePath);// 
            appconfig.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString = connectionString;
            appconfig.AppSettings.SectionInformation.ForceSave = true;
            //appconfig.Save();//ConfigurationSaveMode.Full, true);
            appconfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("connectionStrings");
            Properties.Settings.Default.Reload();
        }

    }
}
