using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Resources;
using System.Data.Entity.Core.EntityClient;
using System.IO;

namespace POS
{
    public partial class SeleccionaLocal : Form
    {
        public SeleccionaLocal()
        {
            InitializeComponent();

            CargarConexiones();
        }

        public string getIPServidor(string local,ref string conn_posEntities ,ref string conn_conectaAx )
        {
            string ip = ""; 

            switch (local)
            {
                case "BOSQUES DE LA COSTA DOMINIO":
                    ip = "srv-pos-bc.interno.liris.com";
                    break;
                case "BOSQUES DE LA COSTA CAJA 1 DOMINIO":
                    ip = "caja01bc.interno.liris.com";
                    break;
                case "PIAZZA DOMINIO":
                    ip = "srv-pos-pia.interno.liris.com";
                    break;
                case "ORELLANA DOMINIO":
                    ip = "srv-pos-ore.interno.liris.com";
                    break;
                case "ALBORADA DOMINIO":
                    ip = "srv-pos-alb.interno.liris.com";
                    break;
                case "GRAN MANZANA DOMINIO":
                    ip = "srv-pos.interno.liris.com";
                    break;
                case "PPG DOMINIO":
                    ip = "srv-pos-ppg.interno.liris.com";
                    break;
                case "GOMEZ RENDON DOMINIO":
                    ip = "srv-pos-gr.interno.liris.com";
                    break;
                case "VILLA CLUB DOMINIO":
                    ip = "srv-pos-vc.interno.liris.com";
                    break;
                case "LA JOYA DOMINIO":
                    ip = "srv-pos-joya.interno.liris.com";
                    break;
                case "BOSQUES DE LA COSTA":
                    ip = "srv-pos-bc";
                    break;
                case "BOSQUES DE LA COSTA CAJA 1":
                    ip = "caja01bc";
                    break;
                case "PIAZZA":
                    ip = "srv-pos-pia";
                    break;
                case "ORELLANA":
                    ip = "srv-pos-ore";
                    break;
                case "ALBORADA":
                    ip = "srv-pos-alb";
                    break;
                case "GRAN MANZANA":
                    ip = "srv-pos";
                    break;
                case "PPG":
                    ip = "srv-pos-ppg";
                    break;
                case "GOMEZ RENDON":
                    ip = "srv-pos-gr";
                    break;
                case "VILLA CLUB":
                    ip = "srv-pos-vc";
                    break;
                case "LA JOYA":
                    ip = "srv-pos-joya";
                    break;
                case "BOSQUES DE LA COSTA IP":
                    ip = "192.168.135.6";
                    break;
                case "BOSQUES DE LA COSTA CAJA 1 IP":
                    ip = "192.168.135.11";
                    break;
                case "PIAZZA IP":
                    ip = "192.168.128.7";
                    break;
                case "ORELLANA IP":
                    ip = "192.168.126.7";
                    break;
                case "ALBORADA IP":
                    ip = "192.168.10.7";
                    break;
                case "GRAN MANZANA IP":
                    ip = "192.168.127.13";
                    break;
                case "PPG IP":
                    ip = "192.168.124.7";
                    break;
                case "GOMEZ RENDON IP":
                    ip = "192.168.123.8";
                    break;
                case "VILLA CLUB IP":
                    ip = "192.168.132.8";
                    break;
                case "LA JOYA IP":
                    ip = "192.168.138.7";
                    break;
                case "LIZARDO GARCIA":
                    ip = "srv-pos-garcia";
                    break;
                case "LIZARDO GARCIA IP":
                    ip = "192.168.137.5";
                    break;
                case "LIZARDO GARCIA DOMINIO":
                    ip = "srv-pos-garcia.interno.liris.com";
                    break;

                case "SERVIDOR PRUEBAS 2":
                    conn_posEntities = ConfigurationManager.ConnectionStrings["POSEntitiesTEST"].ConnectionString;
                    conn_conectaAx = POS.Properties.Resources.CONECTA_AX_PLANTILLA_TEST;
                    ip = "192.168.127.17";
                    break;
                case "SERVIDOR PRUEBAS 3":
                    conn_posEntities = ConfigurationManager.ConnectionStrings["POSEntitiesTEST"].ConnectionString;
                    conn_conectaAx = POS.Properties.Resources.CONECTA_AX_PLANTILLA_TEST;
                    ip = "192.168.130.19";
                    break;
                case "MERCADITO":
                    ip = "192.168.130.196";
                    break;               
                //evelasco .ini 2019-11-21
                case "ANIBAL IP TEST":
                    ip = "192.168.131.107";
                    break;
                //evelasco .fin 2019-11-21

            }
            return ip;
        }
    

        private void btnSave_Click(object sender, EventArgs e)
        {
            string ip = "";

            string conn_conectaAx = POS.Properties.Resources.CONECTA_AX_PLANTILLA;
            string conn_conectMkt = POS.Properties.Resources.CONECTA_MKT_PLANTILLA;
            string conn_posEntities = POS.Properties.Resources.ConectaDB;

            if (!string.IsNullOrEmpty(cmbLocal.Text) && cmbLocal.SelectedValue != null)
            {
                ip = cmbLocal.SelectedValue.ToString();
                var ip_old = this.getIPServidor(cmbLocal.Text, ref conn_posEntities, ref conn_conectaAx);                 
            }
            else
            {
                ip = this.getIPServidor(cmbLocal.Text,ref conn_posEntities,ref conn_conectaAx);  
            }

            if (MessageBox.Show(this,"Esta Seguro de Grabar???", "Pregunta", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {

                POS.Properties.Settings.Default.ROOT_URL = POS.Properties.Resources.ROOT_URL_PLANTILLA.Replace("[ipserver]", ip);
                POS.Properties.Settings.Default.ROOT_URL_GENERAL = POS.Properties.Resources.ROOT_URL_GENERAL_PLANTILLA.Replace("[ipserver]", ip);
                POS.Properties.Settings.Default.CONECTA_AX = conn_conectaAx.Replace("[ipserver]", ip);
                POS.Properties.Settings.Default.CONECTA_MKT = conn_conectMkt.Replace("[ipserver]", ip);
                Properties.Settings.Default.Save();

                SaveConnectionString("POSEntities", conn_posEntities.Replace("[ipserver]", ip));

                Control.Common.GlobalParameters.DecriptedPwd = string.Empty;
                MessageBox.Show(this,"Reinicie la Aplicacion");

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "SeleccionaLocal", "btnSave_Click", "Se solicitó cambio de servidor, debe reiniciarse la Aplicacion. La nueva ip será '" + ip + "'", string.Empty);


                Application.ExitThread();
                //this.Close();
            }
        }


        public static void SaveConnectionString(string connectionStringName, string connectionString)
        {
            Configuration appconfig =
                ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);//System.Reflection.Assembly.GetEntryAssembly().Location/ConfigurationUserLevel.None);// PerUserRoamingAndLocal);//Application.ExecutablePath);// 
            appconfig.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString = connectionString;
            appconfig.AppSettings.SectionInformation.ForceSave = true;
            appconfig.Save();//ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("connectionStrings");
            Properties.Settings.Default.Reload();
        }

        public static string GetServerNameADOConnectionString(string connectionStringName)
        {
            string ServerName = "";
            try
            {
                // Retrieve the ConnectionString from App.config 
                string connectString = ConfigurationManager.ConnectionStrings[connectionStringName].ToString();
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectString);
                // Retrieve the DataSource property.    
                ServerName = builder.DataSource;
            }
            catch (Exception ex)
            {
                ServerName = "No identificado";
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "SeleccionaLocal", "GetServerNameADOConnectionString", Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

            return ServerName;
        }

        public static string GetServerNameEFConnectionString(string connectionStringName)
        {
            string ServerName = "";
            try
            {
                // Retrieve the ConnectionString from App.config 
                Configuration appconfig =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming); //None);Application.ExecutablePath);// 

                string connectString = appconfig.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;
                                //ConfigurationManager.ConnectionStrings[connectionStringName].ToString();

                using (var ec = new EntityConnection(connectString))
                {
                    var sqlConn = ec.StoreConnection as System.Data.SqlClient.SqlConnection;
                    sqlConn.Open();

                    ServerName = sqlConn.DataSource;
                }
            }
            catch (Exception ex)
            {
                ServerName = "No identificado";
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "SeleccionaLocal", "GetServerNameEFConnectionString", Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

            return ServerName;
        }

        public void CargarConexiones()
        {
            try
            {
                string path_remota_conexion = Path.GetDirectoryName(POS.Properties.Settings.Default.CONEXION_PATH_REMOTO);
                string file_conexion_remoto = POS.Properties.Settings.Default.CONEXION_PATH_REMOTO;
                string path_conexion = Path.GetDirectoryName(POS.Properties.Settings.Default.CONEXION_PATH);
                string file_conexion = POS.Properties.Settings.Default.CONEXION_PATH;

                if (!Directory.Exists(path_conexion))
                {
                    Directory.CreateDirectory(path_conexion); 
                }

                try
                {
                    File.Copy(POS.Properties.Settings.Default.CONEXION_PATH_REMOTO, POS.Properties.Settings.Default.CONEXION_PATH, true);
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SeleccionaLocal", "CargarConexiones", "No fue posible cargar las conexiones desde la ruta " + POS.Properties.Settings.Default.CONEXION_PATH_REMOTO + ", a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                }    
                

                if (File.Exists(file_conexion))
                { 
                    using (StreamReader file = new StreamReader(file_conexion))
                    {
                        string ln;
                        Dictionary<string, string> localList = new Dictionary<string, string>();
                        int index = 0;

                        if (new FileInfo(file_conexion).Length > 0)
                            cmbLocal.Items.Clear();

                        while ((ln = file.ReadLine()) != null)
                        {
                            var list_ln = ln.Split('|');

                            if (list_ln.Count() == 2)
                                localList.Add(list_ln[1], list_ln[0]);
                            else
                                localList.Add(index.ToString(), list_ln[0]);

                            index++;
                        }

                        cmbLocal.DataSource = new BindingSource(localList, null);
                        cmbLocal.DisplayMember = "Value";
                        cmbLocal.ValueMember = "Key";

                        file.Close();
                    }
                }
                else
                {
                    CrearArchivoRemoto(file_conexion);                                        
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "SeleccionaLocal", "CargarConexiones", "No fue posible cargar las conexiones desde la ruta " + POS.Properties.Settings.Default.CONEXION_PATH + ", a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                MessageBox.Show(this, ex.Message);
            }
        }

        public void CrearArchivoRemoto(string file_conexion)
        {
            string ip = "";
            string conn_conectaAx = "";
            string conn_posEntities = "";
            
            using (StreamWriter sw = File.CreateText(file_conexion))
            { 
                foreach (var item in cmbLocal.Items)
                {
                    ip = this.getIPServidor(item.ToString(), ref conn_posEntities, ref conn_conectaAx);
                    
                    if(item.ToString() != "" )
                        sw.WriteLine(String.Format("{0}|{1}", item.ToString(), ip));
                    else
                        sw.WriteLine();
                }
                sw.Flush();
                sw.Close();
            }
            //File.Copy(POS.Properties.Settings.Default.CONEXION_PATH, POS.Properties.Settings.Default.CONEXION_PATH_REMOTO, true);
        }

        private void SeleccionaLocal_Load(object sender, EventArgs e)
        {

        }
    }  
}
