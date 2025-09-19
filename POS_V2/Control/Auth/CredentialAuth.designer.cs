namespace POS.Control.Auth
{
    partial class CredentialAuth
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtUsername = new Telerik.WinControls.UI.RadTextBox();
            this.txtPassword = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.telerikMetroTouchTheme1 = new Telerik.WinControls.Themes.TelerikMetroTouchTheme();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.btnLogin = new Telerik.WinControls.UI.RadButton();
            this.lblMensaje = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMensaje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtUsername.Location = new System.Drawing.Point(190, 47);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.NullText = "Escribir usuario";
            this.txtUsername.PasswordChar = '*';
            this.txtUsername.Size = new System.Drawing.Size(262, 30);
            this.txtUsername.TabIndex = 3;
            this.txtUsername.TabStop = false;
            this.txtUsername.ThemeName = "TelerikMetroTouch";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtPassword.Location = new System.Drawing.Point(190, 88);
            this.txtPassword.MaxLength = 50;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.NullText = "Escribir contraseña";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(262, 30);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.TabStop = false;
            this.txtPassword.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel2
            // 
            this.radLabel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radLabel2.Location = new System.Drawing.Point(95, 54);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(61, 23);
            this.radLabel2.TabIndex = 5;
            this.radLabel2.Text = "Usuario:";
            this.radLabel2.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel3
            // 
            this.radLabel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radLabel3.Location = new System.Drawing.Point(70, 95);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(86, 23);
            this.radLabel3.TabIndex = 6;
            this.radLabel3.Text = "Contraseña:";
            this.radLabel3.ThemeName = "TelerikMetroTouch";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::POS.Properties.Resources.cancel_26;
            this.btnCancel.Location = new System.Drawing.Point(190, 143);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 37);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Atrás";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.ThemeName = "TelerikMetroTouch";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLogin.Image = global::POS.Properties.Resources.login_26;
            this.btnLogin.Location = new System.Drawing.Point(324, 143);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(128, 37);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "Login";
            this.btnLogin.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLogin.ThemeName = "TelerikMetroTouch";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblMensaje
            // 
            this.lblMensaje.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMensaje.Location = new System.Drawing.Point(12, -4);
            this.lblMensaje.MaximumSize = new System.Drawing.Size(500, 0);
            this.lblMensaje.Name = "lblMensaje";
            // 
            // 
            // 
            this.lblMensaje.RootElement.MaxSize = new System.Drawing.Size(500, 0);
            this.lblMensaje.Size = new System.Drawing.Size(404, 23);
            this.lblMensaje.TabIndex = 6;
            this.lblMensaje.Text = "Ud. necesita credenciales de administrador para esta acción";
            this.lblMensaje.ThemeName = "TelerikMetroTouch";
            // 
            // CredentialAuth
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(530, 204);
            this.ControlBox = false;
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.btnLogin);
            this.Name = "CredentialAuth";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ingresar credenciales de administrador";
            this.ThemeName = "TelerikMetroTouch";
            this.Load += new System.EventHandler(this.CredentialAuth_Load);


            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMensaje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadTextBox txtUsername;
        private Telerik.WinControls.UI.RadTextBox txtPassword;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.Themes.TelerikMetroTouchTheme telerikMetroTouchTheme1;
        private Telerik.WinControls.UI.RadButton btnLogin;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.UI.RadLabel lblMensaje;
    }
}
