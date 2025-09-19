using System;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using POS.Control.Common; // Asegúrate de tener este namespace si usas General.GetMensajeToList
using POS.Models;

namespace POS.Control.Auth
{
    public partial class CredentialAuth : RadForm
    {
        private  bool _esAutorizado;
        public  bool EsAutorizado
        {
            get { return _esAutorizado; }
            set { _esAutorizado = value; }
        }
        public string msjPantalla { get; set; }

        public CredentialAuth(string msjPantalla = "") : this()
        {
            if (!string.IsNullOrEmpty(msjPantalla))
            {
                this.msjPantalla = msjPantalla;
                lblMensaje.Text = msjPantalla;
            }
        }

        public CredentialAuth()
        {
            InitializeComponent();

            // Suscribir evento Shown para enfocar txtUsername
            this.Shown += OnFormShown;

            // Asignar botón por defecto para Enter
            this.AcceptButton = btnLogin;

            // Cargar configuraciones iniciales
            Load += CredentialAuth_Load;
        }

        private void CredentialAuth_Load(object sender, EventArgs e)
        {
            // Puedes realizar inicializaciones adicionales aquí si es necesario
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            // Garantiza que el control txtUsername tenga el foco
            this.BeginInvoke((MethodInvoker)delegate {
                txtUsername.Focus();
            });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == Common.GlobalParameters.MasterUser &&
                txtPassword.Text == Common.GlobalParameters.MasterPassword)
            {
                _esAutorizado = true;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                Control.Common.General.GetMensajeToList(317);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return true;
            }

            if (keyData == Keys.Enter)
            {
                btnLogin.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
            // Manejo extendido opcional: F1, Ctrl+X, etc.
        }
    }
}