using POS;
using POS.Control;
using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;

namespace POS.Control.Fingerprint
{
    public partial class VerificationForm : Telerik.WinControls.UI.RadForm
    {
        Factura _factura;
        private AppData Data;
        private System.Windows.Forms.Timer _keepOnTopTimer;
        private byte[] byteArray;
        private DPFP.Template template;
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;

        private void ForzarAlFrente()
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
        public VerificationForm(AppData data, Factura factura)
        {
            InitializeComponent();
            Data = data;
            this._factura = factura;
        }

        public void OnComplete(object Control, DPFP.FeatureSet FeatureSet, ref DPFP.Gui.EventHandlerStatus Status)
        {

            DPFP.Verification.Verification ver = new DPFP.Verification.Verification();
            DPFP.Verification.Verification.Result res = new DPFP.Verification.Verification.Result();


            try
            {

                /*
                    // Compare feature set with all stored templates.
                    foreach (DPFP.Template template in Data.Templates)
                    {
                        // Get template from storage.
                        if (template != null)
                        {
                            // Compare feature set with particular template.
                            ver.Verify(FeatureSet, template, ref res);
                            Data.IsFeatureSetMatched = res.Verified;
                            Data.FalseAcceptRate = res.FARAchieved;
                            if (res.Verified)
                                break; // success
                        }
                    }
                 */

                using (var db = new POSEntities())
                {
                    string username = _factura != null ? _factura.User.username : ((Common.GlobalParameters.UserObj != null) ? Common.GlobalParameters.UserObj.username : string.Empty);

                    if (this.Tag.ToString() == "usr")
                    {
                        var usuariosQuery = from usrAuth in db.auth_user
                                            where usrAuth.username == username // _factura.User.username
                                            select usrAuth;

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
                                    Status = DPFP.Gui.EventHandlerStatus.Success;
                                    this.DialogResult = DialogResult.OK;
                                    this.Tag = usrAuth.username;
                                    this.Close();
                                    break;
                                    //MessageBox.Show(this,"Usuario Reconocido");  //success

                                }
                            }

                        }
                    }
                    else
                    {
                        var usuariosQuery = from usrAuth in db.TblAuthorizeDeleteProducts
                                            where usrAuth.LOCATIONID == Establecimiento || usrAuth.LOCATIONID == "TODO"
                                            select usrAuth;

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
                                    Status = DPFP.Gui.EventHandlerStatus.Success;
                                    this.DialogResult = DialogResult.OK;
                                    this.Tag = usrAuth.USERID;
                                    this.Close();
                                    break;
                                    //MessageBox.Show(this,"Usuario Reconocido");  //success

                                }
                            }

                        }
                    }


                }

                if (!res.Verified)
                {
                    Status = DPFP.Gui.EventHandlerStatus.Failure;
                }


            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Pagos.VerificationForm", "VerificationForm_Load", "Se presentaron novedades " +
                     "durante la ejecución del método, a continuacion las excepciones " +
                     "encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
            finally
            {

                if (!res.Verified)
                {
                    Status = DPFP.Gui.EventHandlerStatus.Failure;
                }

            }


            // Data.Update();
        }


        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            this.Close();
        }

        private void VerificationForm_Load(object sender, EventArgs e)
        {
            string Tipo = this.Tag.ToString();
            string establecimientoCodePOS;
            Control.Common.GlobalParameters.VerificationInProgress = true;

            try
            {
                this.SuspendLayout();
                Cursor.Current = Cursors.WaitCursor;

                establecimientoCodePOS = Common.GlobalParameters.Establecimiento;
                if (_factura != null)
                {
                    establecimientoCodePOS = this._factura.Establecimiento;
                }

                using (var db = new POSEntities())
                {
                    Establecimiento = db.core_establecimiento
                        .FirstOrDefault(x => x.establecimiento == establecimientoCodePOS).almacen;
                }

                switch (Tipo)
                {
                    case "usr":
                        this.Text = "Estimado Usuario, Verifique su Identidad";
                        break;
                    case "adm":
                        this.Text = "Estimado Administrador, Verifique su Identidad";
                        break;
                    case "aud":
                        this.Text = "Estimado Auditor, Verifique su Identidad";
                        Establecimiento = "AUDITOR";
                        break;
                }

                ForzarAlFrente();
                this.TopMost = true;
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                this.Activate();
                this.Focus();

                // ? Timer que cada 300ms fuerza el formulario al frente
                _keepOnTopTimer = new System.Windows.Forms.Timer();
                _keepOnTopTimer.Interval = 300;
                _keepOnTopTimer.Tick += (s, args) =>
                {
                    if (Control.Common.GlobalParameters.VerificationInProgress)
                    {
                        ForzarAlFrente();
                        this.Activate();
                    }
                };
                _keepOnTopTimer.Start();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.VerificationForm",
                    "VerificationForm_Load", "Se presentaron novedades durante la ejecución del método - " +
                    Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.ResumeLayout();
            }
        }

        private string Establecimiento;

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    this.DialogResult = DialogResult.None;
                    this.Close();
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _keepOnTopTimer?.Stop();
            _keepOnTopTimer?.Dispose();
            Control.Common.GlobalParameters.VerificationInProgress = false;
            base.OnFormClosed(e);
        }


    }
}