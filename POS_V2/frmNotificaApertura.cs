using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Control;
using System.Data.SqlClient;
using System.Configuration;
using System.Resources;
using System.IO;
using MensajesLibrary;

namespace POS
{
    public partial class frmNotificaApertura :  Form,  DPFP.Capture.EventHandler
    {
        public static MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
        public static MsgBoxCtrl.MessageType messageType;
        public static  MsgBoxCtrl.MessageBoxResult result;

        String Username;
        String Password;
        User _current_user;
        public static Models.User user;
        DateTime hoy;
        String ipAddress = String.Empty;
        String ID_CAJA = String.Empty;
        int Leido = 0;
        WebMethods web = new WebMethods();
        string LocalAx = "";

        //DPFP.Capture.Capture Capturer = new DPFP.Capture.Capture();

        DPFP.Sample SampleImg;
        private DPFP.Verification.Verification Verificator;
        private DPFP.Capture.Capture Capturer;

        private AppData Data;
        private byte[] byteArray;
        private DPFP.Template template;

        protected virtual void Init()
        {

            msgBoxCtrl = new MsgBoxCtrl();
            CheckForIllegalCrossThreadCalls = false;
            try
            {

                Capturer = new DPFP.Capture.Capture();				// Create a capture operation.
                if (null != Capturer)
                    Capturer.EventHandler = this;					// Subscribe for capturing events.
                else
                    SetPrompt("Can't initiate capture operation!");
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                //MessageBox.Show(this, "Can't initiate capture operation!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Can't initiate capture operation!", "Notificación Apertura");
                Control.Common.General.GetMensajeToList(133);
            }
        }
    

        protected virtual void Process(DPFP.Sample Sample)
        {
            string strRutaHuella = string.Empty;

            // Draw fingerprint sample image.
            DrawPicture(ConvertSampleToBitmap(Sample));

            if (!chkAceptar.Checked && chkAceptar.Enabled & Username != "999999999999999999999")
            {
                // MessageBox.Show(this, "Debe aceptar los terminos y condiciones");
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Debe aceptar los terminos y condiciones", "Notificación Apertura");
                Control.Common.General.GetMensajeToList(577);

                DialogResult = DialogResult.None;
                return;
            }

            DPFP.Verification.Verification ver = new DPFP.Verification.Verification();
            DPFP.Verification.Verification.Result res = new DPFP.Verification.Verification.Result();
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);


            using (var db = new POSEntities())
            {
                var CajeroApertura = (from usrAuth in db.viewAperturaLogin
                                      where usrAuth.username == Username
                                      && (usrAuth.is_active || !usrAuth.is_staff)
                                      select usrAuth).ToList();

                if (CajeroApertura.Count != 0)
                {
                    var dataHuella = BitConverter.ToString(CajeroApertura.FirstOrDefault().huella);

                    if (dataHuella == "00-00-00-00")
                    {
                        lblStatus.Text = ("Usuario no valido");
                        return;
                    }

                    features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);


                    byteArray = (byte[])CajeroApertura.FirstOrDefault().huella;
                    Stream stream = new MemoryStream(byteArray);
                    template = new DPFP.Template(stream);

                    ver.Verify(features, template, ref res);
                    Data.IsFeatureSetMatched = res.Verified;
                    Data.FalseAcceptRate = res.FARAchieved;

                    if (!res.Verified)
                    {
                        lblStatus.Text = ("Usuario no valido");
                        return;
                    }
                }
                else
                {
                    lblStatus.Text = ("Usuario no valido");
                    return;
                }



                // Verificar los usuarios aperturados el dia actual en el Local.  JM 10-09-2019
                var usuariosQuery = from apert in db.viewAperturaLogin
                                    join usrAuth in db.auth_user on apert.id equals usrAuth.id
                                    select usrAuth;


                if (usuariosQuery.Count() == 0)
                {
                    usuariosQuery = from usrAuth in db.auth_user
                                    where ((usrAuth.is_active || !usrAuth.is_staff)
                                    && usrAuth.username == Username) || usrAuth.is_superuser
                                    select usrAuth;
                }

                if (usuariosQuery.Count() == 0)
                {
                    usuariosQuery = from usrAuth in db.auth_user
                                    where usrAuth.is_active && usrAuth.is_superuser
                                    select usrAuth;

                }

                //obtengo parametro de ruta de la huella (img).
                if (db.core_parametro.Any(x => x.identificador == "RUTAHUELLAPOS"))
                {
                    strRutaHuella = db.core_parametro.First(x => x.identificador == "RUTAHUELLAPOS").valor;
                }

                foreach (var usrAuth in usuariosQuery)
                {

                    //  MessageBox.Show(this,usrAuth.USERNAME);
                    if (BitConverter.ToString(usrAuth.Huella) != "00-00-00-00")
                    {
                        byteArray = (byte[])usrAuth.Huella;
                        Stream stream = new MemoryStream(byteArray);
                        template = new DPFP.Template(stream);

                        Verificator = new DPFP.Verification.Verification(); // Create a fingerprint template 
                                                                            // Process the sample and create a feature set for the enrollment purpose.
                        features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

                        // Check quality of the sample and start verification if it's good
                        if (features != null)
                        {
                            // Compare the feature set with our template
                            DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                            Verificator.Verify(features, template, ref result);

                            if (result.Verified)
                            {
                                var usernameRoute = string.Empty;
                                try
                                {
                                    if (BitConverter.ToString(usrAuth.imagen) == "00-00-00-00")
                                    {
                                        //usrAuth.imagen = Sample.Bytes;
                                        using (var dbimg = new POSEntities())
                                        {
                                            var uptimagen = dbimg.auth_user.Where(x => x.username == usrAuth.username).FirstOrDefault();
                                            uptimagen.imagen = Sample.Bytes;

                                            //1.-colocar en la core_parametro  : "\\\\192.168.127.17\\Prueba_AX\\imagenes\"
                                            //try{
                                            if (string.IsNullOrEmpty(strRutaHuella))
                                            {
                                                strRutaHuella = "\\\\192.168.127.17\\Prueba_AX\\imagenes\\";
                                                //usernameRoute = "\\\\192.168.127.17\\Prueba_AX\\imagenes\\" + (string.IsNullOrWhiteSpace(usrAuth.username) ? "" : usrAuth.username) + ".jpg";
                                            }
                                            usernameRoute = strRutaHuella + (string.IsNullOrWhiteSpace(usrAuth.username) ? "" : usrAuth.username) + ".jpg";
                                            try
                                            {
                                                Picture.Image.Save(usernameRoute);
                                                dbimg.SaveChanges();
                                            }
                                            catch (Exception)
                                            {
                                                Picture.Image = Picture.InitialImage;
                                            }

                                            //2.- poner el cathc la imagen por default
                                            //catch(Exception ex)
                                            //{
                                            //colocar una ruta quemada por default, firma default.
                                            //Picture.Image.Save("\\\\192.168.127.17\\Prueba_AX\\imagenes\\" + usrAuth.username + ".jpg");
                                            // Picture.Image = Picture.InitialImage;
                                            //}
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "frmNotificaApertura", "Process (Metdo de autenticacion)", "No se pudo grabar los bytes de imagen en auth_user o bien no se logro guardar la huella como imagwen en la ruta: '" + usernameRoute + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                                }

                                lblStatus.Text = ("Usuario Correcto");
                                this.DialogResult = DialogResult.OK;

                                this.Tag = IniciaPOS(usrAuth);
                                this.Hide();

                                this.Close();
                                break;
                            }
                            else
                            {
                                //    this.Tag = null;
                                //  this.DialogResult = DialogResult.Cancel;
                                lblStatus.Text = ("Usuario Incorrecto");
                            }
                        }
                    }
                }
            }
        }


        protected void Start()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    SetPrompt("Using the fingerprint reader, scan your fingerprint.");
                }
                catch
                {
                    SetPrompt("Can't initiate capture!");
                }
            }
        }

        protected void Stop()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    SetPrompt("Can't terminate capture!");
                }
            }
        }

        #region EventHandler Members:

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            lblStatus.Text = ("La huella a sido capturada.");
            SetPrompt("Scan the same fingerprint again.");
            Process(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            lblStatus.Text = ("No se alcanzo a leer la huella.");
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            lblStatus.Text = ("Lector tocado.");
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            try { lblStatus.Text = ("Lector conectado."); }
            catch { }
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            try { lblStatus.Text = ("Lector desconectado."); }
            catch { }
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
                lblStatus.Text = ("Calidad de la imagen buena.");
            else
                lblStatus.Text = ("Calidad de la imagen pobre.");
        }

        #endregion

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            SampleImg = Sample;

            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();  // Create a sample convertor.
            Bitmap bitmap = null;                                                           // TODO: the size doesn't matter
            Convertor.ConvertToPicture(Sample, ref bitmap);                                 // TODO: return bitmap as a result
            return bitmap;
        }

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

        protected void SetStatus(string status)
        {
            //this.Invoke(new Function(delegate() {
            //StatusLine.Text = status;
            //}));
        }


        protected void SetPrompt(string prompt)
        {
            //			this.Invoke(new Function(delegate() {
            //	Prompt.Text = prompt;
            //		}));
        }

        private void DrawPicture(Bitmap bitmap)
        {
            //this.Invoke(new Function(delegate() {
            Picture.Image = new Bitmap(bitmap, Picture.Size);   // fit the image into the picture box
                                                                //}));
        }

 
        public frmNotificaApertura()
        {
            InitializeComponent();
            Data = new AppData();
        }
        

        #region CredencialesOK
        private User IniciaPOS(auth_user _usrAuth)
        {
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            user = new User();
            user.username = _usrAuth.username;
            user.nombres = _usrAuth.last_name + ' ' + _usrAuth.first_name;
            user.result = true;
            user.isSuperUser = _usrAuth.is_superuser;
            if (!user.isSuperUser)
            {
                SqlConnection conexion = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                string Query = null;
                SqlCommand comando = default(SqlCommand);

                using (conexion)
                {
                    conexion.Open();
                    try
                    {
                        Query = string.Empty;
                        Query = string.Concat(Query, "UPDATE TBL_MONTOAPERTURA  ", Environment.NewLine);
                        Query = string.Concat(Query, "   SET  FECHALEIDOTZID=37001,leido= 1 ", Environment.NewLine);
                        Query = string.Concat(Query, "   , FECHALEIDO = cast('" + DateTime.Now.AddHours(5).ToString("yyyy-MM-ddTHH:mm:ss") + "' as datetime) ", Environment.NewLine);
                        Query = string.Concat(Query, " WHERE ID_CAJA ='" + ID_CAJA + "'", Environment.NewLine);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "frmNotificaApertura", "IniciaPOS", "Se ejecuta query de Aceptacion de caja: '" + Query + "'");

                        //Query = "UPDATE TBL_MONTOAPERTURA  SET  FECHALEIDOTZID=37001,leido= 1, FECHALEIDO = cast('" + DateTime.Now.AddHours(5).ToString("yyyy-MM-ddTHH:mm:ss") + "' as datetime) WHERE ID_CAJA ='" + ID_CAJA + "'";
                        //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "frmNotificaApertura", "IniciaPOS", "Se ejecuta query de Aceptacion de caja: '" + Query + "'");

                        comando = new SqlCommand(Query, conexion);
                        comando.ExecuteNonQuery();

                        // jchid apertura de caja en TBL_montoapertura, se actualiza el campo leido=1 y FECHALEIDO con la fecha y hora de la aceptacion de la apertura de caja.
                        Control.Common.Printer.OpenCashDrawer_PrinterName(new System.Drawing.Printing.PrinterSettings().PrinterName);
                        // jchid end

                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "frmNotificaApertura", "IniciaPOS", "No se pudo actualizar estado de campo leido en registro de apertura: '" 
                            + (string.IsNullOrWhiteSpace(ID_CAJA) ? "Variable ID_CAJA no cargada" : ID_CAJA) + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));

                        //MessageBox.Show(this, ex.ToString(), "Mensaje");
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, ex.Message.ToString(), "Notificación Apertura");

                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = ex.Message.ToString() });
                        Control.Common.General.GetMensajeToList(578, parametros);


                        //Control.Common.General.GetMensaje("Notificación Apertura", ex.Message.ToString(), "I");
                    }
                    conexion.Close();
                    conexion.Dispose();
                }
            }
            return user;
        }



        #endregion

        private void frmNotificaApertura_Load(object sender, EventArgs e)
        {
            Init();

            Start();												// Start capture operation.

            //Activar panel cambio de server si Ip no tiene permisos para el ambiente actual
            ComprobarIpPermisoAmbiente();

            string establecimiento = "000";
            string Usuario = "USUARIO NO VALIDO";
            string IdCajero = string.Empty;
            string monto = "0.0";
            string PUNTO_VENTA = string.Empty;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            //txtTerminos.Text = POS.Properties.Settings.Default.MENSAJE_CAJA;
            string nombrePC = Dns.GetHostName().ToString();
            IPHostEntry ipEntry = Dns.GetHostEntry(nombrePC);
            IPAddress[] addr = ipEntry.AddressList;
            //Verificamos la IP de la PC donde se ejecuta la APP.
            ipAddress = addr.Where(i => !i.IsIPv6LinkLocal && !i.IsIPv6Teredo && i.AddressFamily == AddressFamily.InterNetwork).First().ToString();
            hoy = DateTime.Now.AddDays(0);
            var pto = new BG_Apertura();// = null;
            var result = false;
            var es2x = false;
            //Buscamos el punto de emisión
            using (var db = new POSEntities())
            {
                try
                { 
                    es2x = db.core_parametro.Any(x => x.identificador == "2X_CONSULTA_POS"
                                                        && x.parametro2 == ipAddress
                                                        && x.valor == "TRUE");
                    if (es2x)//Es para 2x consulta
                    {
                        DialogResult = DialogResult.Retry;
                    }
                    else
                    {
                        result = db.BG_Apertura.Any(x => x.IP == ipAddress && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0);

                        establecimiento = "000";
                        Usuario = "USUARIO NO VALIDO";
                        IdCajero = "------";
                        monto = "${suelto}";
                        Username = "999999999999999999999";
                        PUNTO_VENTA = "-----";

                        chkAceptar.Enabled = false;
                        cmdOK.Enabled = false;

                        //AQUI revisar
                        if (result)
                        {
                            pto = db.BG_Apertura.Single(x => x.IP == ipAddress && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0);
                            establecimiento = pto.ESTABLECIMIENTO;
                            monto = monto.Replace("{suelto}", pto.VALOR_BASE.ToString("N2"));
                            Usuario = pto.CAJERA.ToString();
                            Username = pto.CEDULA;
                            PUNTO_VENTA = pto.PUNTO_VENTA.ToString();

                            string FECHA = ((DateTime)pto.FECHA).ToString("yyyy-MM-dd");
                            ID_CAJA = pto.NUM_CAJA.ToString() + "-" + FECHA + "-" + pto.PUNTO_VENTA + "-" + pto.TURNO + "-" + pto.INVENTLOCATION;

                            Program.ID_Caja_POS = ID_CAJA;
                            LocalAx = pto.INVENTLOCATION;

                            if (pto.LEIDO == 1)
                            {
                                DialogResult = DialogResult.Retry;
                            }
                        }
                       
                        monto = monto.Replace("{suelto}", pto.VALOR_BASE.ToString("N2"));

                        lblUsuario.Text = Usuario;
                        lblMonto.Text = monto;
                        lblCaja.Text = PUNTO_VENTA;


                        if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento).Count() > 0)
                        {
                            if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento).First().valor == "TRUE")
                            {
                                lblPassword.Visible = false;
                                txtPassword.Visible = false;
                                cmdOK.Visible = false;
                                chkAceptar.Enabled = true;
                            }
                            else
                            {
                                lblPassword.Visible = true;
                                txtPassword.Visible = true;
                                cmdOK.Visible = true;
                            }
                        }

                        /*Terminos / Politicas de descuento*/
                        var valtermino = db.core_parametro.Where(x => x.identificador == "TERMINOS" && x.valor == "TRUE").FirstOrDefault().documento;
                        WebTerminos.DocumentText = (valtermino.Replace("{suelto}", lblMonto.Text));

                    }


                 



                }
                catch (Exception ex)
                {
                    
                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = ex.Message.ToString() });
                    Control.Common.General.GetMensajeToList(578, parametros);

                    //MessageBox.Show(this, ex.ToString());
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, ex.Message.ToString(), "Notificación Apertura");
                    //Control.Common.General.GetMensaje("Notificación Apertura", ex.Message.ToString(), "I");

                    SeleccionaLocal local = new SeleccionaLocal();
                    local.ShowDialog();
                }

            }

            loadLogo();
        }

        private void ComprobarIpPermisoAmbiente()
        {
            try
            {
                var environmentSecurity = new POS.Control.Security.Environment();
                if (!environmentSecurity.HasIpRunPermission())
                {
                    panelCambiarServer.Visible = true;

                    panel1.Top = panel1.Top - (panelCambiarServer.Height / 2);
                    panelCambiarServer.Top = panel1.Top + panel1.Height + 5;
                    panelCambiarServer.Left = panel1.Left;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "frmNotificaApertura", "ComprobarIpPermisoAmbiente", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            List<ParametrosMensajes> parametrosMsj = new List<ParametrosMensajes>();

            if (!chkAceptar.Checked)
            {
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Debe aceptar los terminos y condiciones", "Notificación Apertura");
                Control.Common.General.GetMensajeToList(134);

                //MessageBox.Show(this, "Debe aceptar los terminos y condiciones");
                //DialogResult = DialogResult.None;
            }
            else
            {
                if (Username.Length > 0 && txtPassword.Text.Length > 0)
                {
                    StringBuilder parametros = new StringBuilder();

                    parametros.Append("username=" + Username);
                    parametros.Append("&password=" + txtPassword.Text);

                    try
                    {
                        user = web.login(parametros.ToString());
                    }
                    catch (Exception err)
                    {

                        parametrosMsj = new List<ParametrosMensajes>();
                        parametrosMsj.Add(new ParametrosMensajes() { codigo = "[error]", valor = err.ToString() });
                        Control.Common.General.GetMensajeToList(593, parametrosMsj);


                        //Control.Common.General.GetMensajeToList("Notificación Apertura", "Error en conexion a servidor!" + err.ToString() + "Comuniquese con IT", "I");
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Error en conexion a servidor! " + err.ToString() + "Comuniquese con IT", "Notificación Apertura");
                        //MessageBox.Show(this, "Error en conexion a servidor! " + err.ToString(), "Comuniquese con IT");
                    }
                    if (user != null)
                    {
                        if (user.result)
                        {
                            using (var db = new POSEntities())
                            {
                                var usuariosQuery = from usrAuth in db.auth_user
                                                    where ((usrAuth.is_active || !usrAuth.is_staff) && usrAuth.username == Username)
                                                    select usrAuth;
                                foreach (var usrAuth in usuariosQuery)
                                {

                                    this.DialogResult = DialogResult.OK;
                                    //this.Tag = usrAuth;
                                    this.Hide();
                                    IniciaPOS(usrAuth);
                                    this.Close();
                                    break;
                                    //MessageBox.Show(this,"Usuario Reconocido");  //success

                                }
                            }


                        }

                        /*txtPassword.Clear();
                            this.Hide();
                            var form = new MainWindow(user, Data);
                            form.Closed += (sender2, args) => { this.Show(); txtPassword.Focus(); };
                            form.ShowDialog();
                            this.Hide();
                            */
                    }
                    else
                    {
                        //MessageBox.Show(this, user.mensaje, "Mensaje");
                        //DialogResult = DialogResult.None;
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, user.mensaje, "Notificación Apertura");
                        //Control.Common.General.GetMensaje("Notificación Apertura", user.mensaje, "I");

                        parametrosMsj = new List<ParametrosMensajes>();
                        parametrosMsj.Add(new ParametrosMensajes() { codigo = "[MsjNotifApertura]", valor = user.mensaje });
                        Control.Common.General.GetMensajeToList(593, parametrosMsj);

                    }
                }


                else
                {
                    //DialogResult = DialogResult.None;
                    //MessageBox.Show(this, "La contraseña no puede ser nulo!", "Mensaje");
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "La contraseña no puede ser nulo!", "Notificación Apertura");
                    Control.Common.General.GetMensajeToList(136);

                }
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == "test")
            {
                txtPassword.Clear();
                this.DialogResult = DialogResult.Retry;
                this.Hide();


                


            }

        }

        private void frmNotificaApertura_Resize(object sender, EventArgs e)
        {
            panel1.Top = (Screen.PrimaryScreen.WorkingArea.Height / 2) - (panel1.Height / 2);
            panel1.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (panel1.Width / 2);
        }

        private void picImgUsr_Click(object sender, EventArgs e)
        {

        }

        private void Picture_Click(object sender, EventArgs e)
        {

        }

        private void frmNotificaApertura_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radLabel1_Click(object sender, EventArgs e)
        {

        }

        private void cmdCambioServidor_Click(object sender, EventArgs e)
        {
            SeleccionaLocal f = new SeleccionaLocal();
            f.ShowDialog();
        }

        public void loadLogo()
        {
            using (var db = new POSEntities())
            {
                string ruta = Properties.Settings.Default.PATH_IMG;
                try
                {
                    if (string.IsNullOrEmpty(LocalAx))
                    {
                        LocalAx = db.core_puntoemision.Where(x=> x.ip_address == ipAddress).FirstOrDefault().core_establecimiento.almacen;
                    }

                    var parametro = db.core_parametro.Where(x => x.identificador == "LOGO_LOCAL_" + LocalAx).FirstOrDefault();
                    if (parametro != null)
                    {
                        if (File.Exists(parametro.valor))
                        {
                            Control.Common.GlobalParameters.LogoLocal = parametro.valor;
                            
                            FileInfo img = new FileInfo(Control.Common.GlobalParameters.LogoLocal);
                            if (!Directory.Exists(ruta))
                                Directory.CreateDirectory(ruta); 

                            if (File.Exists(ruta + @"\logo.png"))
                                File.Delete(ruta + @"\logo.png"); 

                            img.CopyTo(ruta + @"\logo.png");
                            Control.Common.GlobalParameters.LogoLocal = ruta + @"\logo.png";

                            pictureBox1.Image = Image.FromFile(Control.Common.GlobalParameters.LogoLocal);

                        }
                        else
                        {
                            if (File.Exists(ruta + @"\logo.png"))
                            {
                                Control.Common.GlobalParameters.LogoLocal = ruta + @"\logo.png";
                                pictureBox1.Image = Image.FromFile(Control.Common.GlobalParameters.LogoLocal);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Directory.Exists(ruta + @"\logo.png"))
                    {
                       Control.Common.GlobalParameters.LogoLocal = ruta + @"\logo.png";
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "frmNotificaApertura", "loadLogo", " a continuacion la excepcion encontrada: " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                } 
            }
        }

        private void chkAceptar_CheckedChanged(object sender, EventArgs e)
        {
            cmdOK.Enabled = false;
            cmdOK.Visible = false;
            if (chkAceptar.Checked) {
                cmdOK.Enabled = true;
                cmdOK.Visible = true;
            }
        }
    }
}
