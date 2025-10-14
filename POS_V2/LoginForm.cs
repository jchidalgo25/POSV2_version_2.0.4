using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using POS.Control;
using POS.Models;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using POS.Models.finger;

namespace POS
{
    public partial class LoginForm : Telerik.WinControls.UI.RadForm
    {
        System.Windows.Forms.Control focused;
        //private static readonly ILog log = LogManager.GetLogger(typeof(Main));
        User _current_user;
        WebMethods web = new WebMethods();
        public static Models.User user;
        public LoginForm(AppData data)
        {
            InitializeComponent();

            Data = data;
            //XmlConfigurator.Configure();
            ThemeResolutionService.ApplicationThemeName = "TelerikMetroTouch";
         
        }

        private AppData Data;
        private byte[] byteArray;
        private DPFP.Template template;
        string ipAddress = "";


        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();  // Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);            // TODO: return features as a result?
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }
        private respuestaFinger VerificaHuella(byte[] Huella, DPFP.FeatureSet FeatureSet)
        {
            DPFP.Verification.Verification.Result res = new DPFP.Verification.Verification.Result();
            DPFP.Verification.Verification ver = new DPFP.Verification.Verification();
            respuestaFinger FingerResponse = new respuestaFinger();

            try
            {
                // != "00-00-00-00"
                if (BitConverter.ToString(Huella) == "00-00-00-00") {

                }

                byte[] byteArray = (byte[])Huella;
                Stream stream = new MemoryStream(byteArray);
                template = new DPFP.Template(stream);

                ver.Verify(FeatureSet, template, ref res);
                Data.IsFeatureSetMatched = res.Verified;
                Data.FalseAcceptRate = res.FARAchieved;

                FingerResponse.CodError = 0;
                FingerResponse.MsjError= "Validación Exitosa";
                FingerResponse.Status = DPFP.Gui.EventHandlerStatus.Success;

                if (!res.Verified)
                {
                    FingerResponse.CodError = -1;
                    FingerResponse.MsjError = "Huellas no coinciden";
                    FingerResponse.Status = DPFP.Gui.EventHandlerStatus.Failure;
                    return FingerResponse;
                }

                return FingerResponse;
            }
            catch (Exception ex)
            {
                FingerResponse.CodError = -2;
                FingerResponse.MsjError = "error: " + ex.Message;
                FingerResponse.Status = DPFP.Gui.EventHandlerStatus.Failure;
                return FingerResponse;
            }
        }

        public void OnComplete(object Control, DPFP.FeatureSet FeatureSet, ref DPFP.Gui.EventHandlerStatus Status)
        {
            DPFP.Verification.Verification ver = new DPFP.Verification.Verification();
            DPFP.Verification.Verification.Result res = new DPFP.Verification.Verification.Result();


            using (var db = new POSEntities())
            {
                // Verificar los usuarios aperturados el dia actual en el Local.  JM 10-09-2019 
                DateTime hoy = DateTime.Now;

                List<auth_user> usuariosQuery = new List<auth_user>();

                /*Valido si el cajero*/
                var CajeroApertura = (from usrAuth in db.viewAperturaLogin
                                      where usrAuth.username == txtUsername.Text
                                      && (usrAuth.is_active || !usrAuth.is_staff)
                                      select usrAuth).ToList();

                if (CajeroApertura.Count != 0)
                {
                    var dataHuella = BitConverter.ToString(CajeroApertura.FirstOrDefault().huella);

                    if (dataHuella == "00-00-00-00")
                    {
                        Status = DPFP.Gui.EventHandlerStatus.Failure;
                        return;
                    }

                    byteArray = (byte[])CajeroApertura.FirstOrDefault().huella;
                    Stream stream = new MemoryStream(byteArray);
                    template = new DPFP.Template(stream);

                    ver.Verify(FeatureSet, template, ref res);
                    Data.IsFeatureSetMatched = res.Verified;
                    Data.FalseAcceptRate = res.FARAchieved;

                    if (!res.Verified)
                    {
                        Status = DPFP.Gui.EventHandlerStatus.Failure;
                        return;
                    }
                }
                else
                {
                    Status = DPFP.Gui.EventHandlerStatus.Failure;
                    return;
                }


                usuariosQuery = (from apert in db.viewAperturaLogin
                                 join usrAuth in db.auth_user on apert.id equals usrAuth.id
                                 //where usrAuth.ip == ipAddress 
                                 select usrAuth).ToList();

                if (!usuariosQuery.Any(x => x.username == txtUsername.Text))
                {
                    usuariosQuery = (from usrAuth in db.auth_user
                                     where ((usrAuth.is_active || !usrAuth.is_staff)
                                     && usrAuth.username.Contains(txtUsername.Text))
                                     || usrAuth.is_superuser
                                     select usrAuth).ToList();

                    if (usuariosQuery.Count() == 0)
                    {
                        usuariosQuery = (from usrAuth in db.auth_user
                                         where usrAuth.is_active && usrAuth.is_superuser
                                         select usrAuth).ToList();
                    }
                }

                DPFP.Verification.Verification Verificator = new DPFP.Verification.Verification();

                foreach (var usrAuth in usuariosQuery)
                {
                    //  MessageBox.Show(this,usrAuth.USERNAME);
                    if (BitConverter.ToString(usrAuth.Huella) != "00-00-00-00")
                    {
                        byteArray = (byte[])usrAuth.Huella;
                        Stream stream = new MemoryStream(byteArray);
                        template = new DPFP.Template(stream);

                        ver.Verify(FeatureSet, template, ref res);
                        Data.IsFeatureSetMatched = res.Verified;
                        Data.FalseAcceptRate = res.FARAchieved;

                        if (res.Verified)
                        {
                            bool tienePermisoLogon = usrAuth.is_superuser;
                            if (!tienePermisoLogon)
                            {
                                tienePermisoLogon = TienePermisoLogon(usrAuth.username);
                            }

                            if (tienePermisoLogon)
                            {
                                POS.Control.Common.Logger.LogMessage(POS.Control.Common.Enum.LogTypes.Info, "LoginForm", "OnComplete", "Logon realizado por medio de huella en POS por el usuario " + usrAuth.last_name + ' ' + usrAuth.first_name + " (" + usrAuth.username + "). " + (usrAuth.is_superuser ? "Tiene rol de super usuario" : "Usuario común"));

                                Status = DPFP.Gui.EventHandlerStatus.Success;
                                this.DialogResult = DialogResult.OK;
                                //this.Tag = usrAuth;
                                this.Hide();
                                txtPassword.Clear();
                                user = new User();
                                user.username = usrAuth.username;
                                user.nombres = usrAuth.last_name + ' ' + usrAuth.first_name;
                                user.result = true;
                                user.isSuperUser = usrAuth.is_superuser;
                                this.Tag = user;
                            }
                        }
                    }
                }

            }

            if (!res.Verified)
            {
                Status = DPFP.Gui.EventHandlerStatus.Failure;
            }


            // Data.Update();
        }

        private bool TienePermisoLogon(string cedula)
        {
            //Se da permisos a todos hasta que se pueda analizar mejor que hacer en caso de que Ax este abajo y no puedan Aperturar Cajas
            //Ademas existen casos en los que los cajeros se cambian de su caja asignada originalmente, CuadreCaja valida las ventas por la cedula
            //por lo que no les causa mayor problema
            return true;

            bool respuesta = false;

            try
            {
                string ipAddress = string.Empty;
                string nombrePC = Dns.GetHostName().ToString();
                IPHostEntry ipEntry = Dns.GetHostEntry(nombrePC);
                IPAddress[] addr = ipEntry.AddressList;
                //Verificamos la IP de la PC donde se ejecuta la APP.
                ipAddress = addr.Where(i => !i.IsIPv6LinkLocal && !i.IsIPv6Teredo && i.AddressFamily == AddressFamily.InterNetwork).First().ToString();
                List<ParametrosMensajes> ListParametros = new List<ParametrosMensajes>();

                using (POSEntities db = new POSEntities())
                {
                    var hoy = DateTime.Now;
                    var listaOtrasAperturas = db.BG_Apertura.Where(x => x.IP == ipAddress && x.CERRADO == 0 && x.PRE_CIERRE == 0 && x.FECHA == hoy.Date).ToList();

                    if (listaOtrasAperturas.Any(x => x.CEDULA != cedula))
                    {
                        //MessageBox.Show("Existen otras cédulas aparte de la suya aperturadas en Ax en esta caja lo cual no es correcto, solicite que le verifiquen esta novedad");
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Existen otras cédulas aparte de la suya aperturadas en Ax en esta caja lo cual no es correcto, solicite que le verifiquen esta novedad", "Notificación Apertura");
                        Control.Common.General.GetMensajeToList(56);
                    }
                    else
                    {
                        var listaAperturas = db.BG_Apertura.Where(x => x.CEDULA == cedula && x.CERRADO == 0 && x.PRE_CIERRE == 0 && x.FECHA == hoy.Date).ToList();

                        if (listaAperturas.Count == 0)
                        {
                            // MessageBox.Show("La cédula '" + cedula + "' no tiene apertura en Ax");
                            // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "La cédula '" + cedula + "' no tiene apertura en Ax", "POS - Login");
                            ListParametros.Add(new ParametrosMensajes()
                            {
                                codigo = "[CEDULA_CAJERO]",
                                valor = cedula
                            });

                            Control.Common.General.GetMensajeToList(57, ListParametros);

                        }
                        else if (listaAperturas.Count > 1)
                        {
                            Control.Common.General.GetMensajeToList(52);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "La cédula '" + cedula + "' tiene más de una apertura abierta en este momento en Ax lo cual no es correcto, solicite que le verifiquen esta novedad", "POS - Login");
                            //MessageBox.Show("La cédula '" + cedula + "' tiene más de una apertura abierta en este momento en Ax lo cual no es correcto, solicite que le verifiquen esta novedad");
                        }
                        else if (listaAperturas.Any(x => x.IP != ipAddress))
                        {
                            Control.Common.General.GetMensajeToList(59);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "La cédula '" + cedula + "' si tiene una apertura en este momento pero no para esta caja", "POS - Login");
                            //MessageBox.Show("La cédula '" + cedula + "' si tiene una apertura en este momento pero no para esta caja");
                        }
                        else
                        {
                            respuesta = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "LoginForm", "TienePermisoLogon", "No se pudo comprobar que el logon tuviera permisos: '" + cedula + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //MessageBox.Show("No se pudo comprobar que el logon tuviera permisos: '" + cedula + "'. Por favor inténtelo nuevamente");
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No se pudo comprobar que el logon tuviera permisos: '" + cedula + "'. Por favor inténtelo nuevamente", "POS - Login");
                Control.Common.General.GetMensajeToList(60);
                respuesta = false;
            }

            return respuesta;
        }


        private void LoginForm_Load(object sender, EventArgs e)
        {
            ipAddress = string.Empty;
            string nombrePC = Dns.GetHostName().ToString();
            IPHostEntry ipEntry = Dns.GetHostEntry(nombrePC);
            IPAddress[] addr = ipEntry.AddressList;
            //Verificamos la IP de la PC donde se ejecuta la APP.
            ipAddress = addr.Where(i => !i.IsIPv6LinkLocal && !i.IsIPv6Teredo && i.AddressFamily == AddressFamily.InterNetwork).First().ToString();


            var hoy = DateTime.Now.AddDays(0);
            var result = false;
            var establecimiento = "000";
            var es2x = false;
            //Buscamos el punto de emisión
            using (var db = new POSEntities())
            {
                //Es 2x consulta Pos
                if (es2x = db.core_parametro.Any(x => x.identificador == "2X_CONSULTA_POS" && x.parametro2 == ipAddress && x.valor == "TRUE"))
                {
                    txtPassword.Text = "test";
                    txtUsername.Text = "1234";
                    txtUsername.ReadOnly = true;
                    txtPassword.ReadOnly = true;
                    btnCancel.Visible = false;
                    cmdCambioServidor.Visible = true;
                }
                else
                {
                    if (result = db.BG_Apertura.Any(x => x.IP == ipAddress && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0))
                    {
                        var pto = db.BG_Apertura.Single(x => x.IP == ipAddress && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0);
                        txtUsername.Text = pto.CEDULA.ToString();
                        establecimiento = pto.ESTABLECIMIENTO;
                        txtUsername.ReadOnly = true;
                    }
                    if (establecimiento != "000")
                    {
                        if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento).First().valor == "TRUE")
                        {
                            txtPassword.Visible = false;
                            btnLogin.Visible = false;
                        }
                    }
                }
            }          


            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                Version ver = null;
                System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                ver = ad.CurrentVersion;
                this.Text += " ver. " + ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision;
            }
            //ValidarUsuario(SystemInformation.UserName); 

            if(Control.Common.GlobalParameters.LogoLocal != null)
                pictureBox1.Image = Image.FromFile(Control.Common.GlobalParameters.LogoLocal);
        }
        private void ValidarUsuario(string username)
        {
            /*Esto quedo pendiente hasta revision con el Ing Bastidas 14/04/2014*/
            using (var db = new POSEntities())
            {
                if (db.auth_user.Any(x => x.username == username))
                {
                    var userApp = db.auth_user.First(x => x.username == username);
                    if (db.auth_user_groups.Any(x => x.user_id == userApp.id && x.group_id == db.auth_group.FirstOrDefault(y => y.name == db.core_parametro.FirstOrDefault(z => z.identificador == "GRUPO_OPERADOR_CAJERA").valor).id))
                    {

                    }
                    else
                    {
                        if (userApp == null)
                        {
                            Control.Common.General.GetMensajeToList(88);

                        }
                        else
                        {
                            Control.Common.General.GetMensajeToList(89);
                        }
                    }
                    //GRUPO_OPERADOR_CAJERA

                }
            }
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            //var text = focused as Telerik.WinControls.UI.RadTextBox;

            //if (text != null)
            //{
            //    var button = sender as System.Windows.Forms.Control;
            //    text.Text = text.Text + button.Tag.ToString();                
            //}

            if (txtUsername.Text.Length < 10)
            {
                var button = sender as System.Windows.Forms.Control;
                txtUsername.Text = txtUsername.Text + button.Tag.ToString();
            }
            else if (txtUsername.Text.Length == 10)
            {
                var button = sender as System.Windows.Forms.Control;
                txtPassword.Text = txtPassword.Text + button.Tag.ToString();
            }
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
        }


        private void btnBorrar_Click(object sender, EventArgs e)
        {
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                if (text.Text.Length > 1)
                    text.Text = text.Text.Substring(0, text.Text.Length - 1);
                else
                    text.Text = "";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            List<ParametrosMensajes> parametrosMsj = new List<ParametrosMensajes>();

            if (txtUsername.Text.Length > 0 && txtPassword.Text.Length > 0)
            {
                StringBuilder parametros = new StringBuilder();
                parametros.Append("username=" + txtUsername.Text);
                parametros.Append("&password=" + txtPassword.Text);

                user = new User();
                user.result = true;
                          
                if (user != null)
                {
                    if (user.result)
                    {
                        using (var db = new POSEntities())
                        { 
                           
                            var usuariosQuery = from usrAuth in db.auth_user
                                                where ((usrAuth.is_active || !usrAuth.is_staff) && usrAuth.username == txtUsername.Text)
                                                select usrAuth;
                            foreach (var usrAuth in usuariosQuery)
                            {
                                POS.Control.Common.Logger.LogMessage(POS.Control.Common.Enum.LogTypes.Info, "LoginForm", "btnLogin_Click", "Logon realizado por medio de contraseña en POS por el usuario " + usrAuth.last_name + ' ' + usrAuth.first_name + " (" + usrAuth.username + "). " + (usrAuth.is_superuser ? "Tiene rol de super usuario" : "Usuario común"));

                                this.DialogResult = DialogResult.OK;
                                //this.Tag = usrAuth;
                                this.Hide();
                                txtPassword.Clear();
                                user = new User();
                                user.username = usrAuth.username;
                                user.nombres = usrAuth.last_name + ' ' + usrAuth.first_name;
                                user.result = true;
                                user.isSuperUser = usrAuth.is_superuser;
                                this.Tag = user;

                            }
                        }
                                txtPassword.Clear();
                        
                    }
                    else
                    {
                      
                        parametrosMsj = new List<ParametrosMensajes>();
                        parametrosMsj.Add(new ParametrosMensajes() { codigo = "[Login_Mensaje]", valor = user.mensaje });
                        Control.Common.General.GetMensajeToList(591, parametrosMsj);

                    }
                }

            }
            else
            {
                //MessageBox.Show(this,"Usuario y contraseña no puede ser nulo!", "Mensaje");
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Usuario y contraseña no puede ser nulo!", "POS - Login");
                Control.Common.General.GetMensajeToList(49);

            }

        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('\r'))
                btnLogin_Click(sender, e);
        }

      

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text=="1234"  && txtPassword.Text == "test")            
                cmdCambioServidor.Visible = true;           
            else 
                cmdCambioServidor.Visible = false;
                
        }

        private void LoginForm_Initialized(object sender, EventArgs e)
        {
            
            //this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width,  Screen.PrimaryScreen.WorkingArea.Height);           
        }

        private void cmdCambioServidor_Click(object sender, EventArgs e)
        {
            SeleccionaLocal f = new SeleccionaLocal();
            f.ShowDialog();
        }

        private void txtUsername_MouseDoubleClick(object sender, MouseEventArgs e)
        {
         
        }

        private void radLabel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString() == "Middle")
            {
                txtUsername.Text = String.Empty;
                txtUsername.ReadOnly = false;
            }
            else
                txtUsername.ReadOnly = true;

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text == "1234")
            {
                btnLogin.Visible = true;
                txtPassword.Visible = true;
            }
        }

        private void LoginForm_Resize(object sender, EventArgs e)
        {
            panel1.Top = (Screen.PrimaryScreen.WorkingArea.Height / 2) - (panel1.Height / 2);
            panel1.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (panel1.Width / 2);
        }
        private void VerificationControl_Load(object sender, EventArgs e)
        {

        }
        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Home || e.KeyCode == Keys.Up)
            {
                btnLogin.Visible = false;
                txtPassword.Text = string.Empty;
                txtPassword.Visible = false;
                
            }
        }
    }
}
