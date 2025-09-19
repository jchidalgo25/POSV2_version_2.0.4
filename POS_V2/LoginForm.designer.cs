namespace POS
{
    partial class LoginForm
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
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.txtUsername = new Telerik.WinControls.UI.RadTextBox();
            this.txtPassword = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.telerikMetroTouchTheme1 = new Telerik.WinControls.Themes.TelerikMetroTouchTheme();
            this.btnBorrar = new Telerik.WinControls.UI.RadButton();
            this.radButton10 = new Telerik.WinControls.UI.RadButton();
            this.radButton7 = new Telerik.WinControls.UI.RadButton();
            this.radButton8 = new Telerik.WinControls.UI.RadButton();
            this.radButton9 = new Telerik.WinControls.UI.RadButton();
            this.radButton4 = new Telerik.WinControls.UI.RadButton();
            this.radButton5 = new Telerik.WinControls.UI.RadButton();
            this.radButton6 = new Telerik.WinControls.UI.RadButton();
            this.radButton3 = new Telerik.WinControls.UI.RadButton();
            this.radButton2 = new Telerik.WinControls.UI.RadButton();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.btnLogin = new Telerik.WinControls.UI.RadButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmdCambioServidor = new Telerik.WinControls.UI.RadButton();
            this.VerificationControl = new DPFP.Gui.Verification.VerificationControl();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.radLabel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBorrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmdCambioServidor)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radLabel1.AutoSize = false;
            this.radLabel1.Controls.Add(this.pictureBox2);
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(427, 541);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(264, 71);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "POS";
            this.radLabel1.ThemeName = "TelerikMetroTouch";
            this.radLabel1.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox2.Image = global::POS.Properties.Resources.ds2incon1;
            this.pictureBox2.Location = new System.Drawing.Point(115, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(92, 61);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtUsername.Location = new System.Drawing.Point(185, 177);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.NullText = "Usuario ej. 1234";
            this.txtUsername.Size = new System.Drawing.Size(222, 30);
            this.txtUsername.TabIndex = 3;
            this.txtUsername.TabStop = false;
            this.txtUsername.ThemeName = "TelerikMetroTouch";
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            this.txtUsername.Enter += new System.EventHandler(this.txtUsername_Enter);
            this.txtUsername.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtUsername_MouseDoubleClick);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtPassword.Location = new System.Drawing.Point(185, 218);
            this.txtPassword.MaxLength = 50;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.NullText = "Contraseña ej. 1234";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(222, 30);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.TabStop = false;
            this.txtPassword.ThemeName = "TelerikMetroTouch";
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // radLabel2
            // 
            this.radLabel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radLabel2.Location = new System.Drawing.Point(103, 183);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(61, 23);
            this.radLabel2.TabIndex = 5;
            this.radLabel2.Text = "Usuario:";
            this.radLabel2.ThemeName = "TelerikMetroTouch";
            this.radLabel2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.radLabel2_MouseDoubleClick);
            // 
            // radLabel3
            // 
            this.radLabel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radLabel3.Location = new System.Drawing.Point(72, 219);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(86, 23);
            this.radLabel3.TabIndex = 6;
            this.radLabel3.Text = "Contraseña:";
            this.radLabel3.ThemeName = "TelerikMetroTouch";
            // 
            // btnBorrar
            // 
            this.btnBorrar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBorrar.Image = global::POS.Properties.Resources.delete_26;
            this.btnBorrar.Location = new System.Drawing.Point(410, 308);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(121, 53);
            this.btnBorrar.TabIndex = 19;
            this.btnBorrar.Text = "Borrar";
            this.btnBorrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBorrar.ThemeName = "TelerikMetroTouch";
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // radButton10
            // 
            this.radButton10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton10.Image = global::POS.Properties.Resources._0_48;
            this.radButton10.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton10.Location = new System.Drawing.Point(250, 541);
            this.radButton10.Name = "radButton10";
            this.radButton10.Size = new System.Drawing.Size(77, 66);
            this.radButton10.TabIndex = 18;
            this.radButton10.Tag = "0";
            this.radButton10.ThemeName = "TelerikMetroTouch";
            this.radButton10.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton7
            // 
            this.radButton7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton7.Image = global::POS.Properties.Resources._9_48;
            this.radButton7.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton7.Location = new System.Drawing.Point(328, 463);
            this.radButton7.Name = "radButton7";
            this.radButton7.Size = new System.Drawing.Size(77, 68);
            this.radButton7.TabIndex = 17;
            this.radButton7.Tag = "9";
            this.radButton7.ThemeName = "TelerikMetroTouch";
            this.radButton7.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton8
            // 
            this.radButton8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton8.Image = global::POS.Properties.Resources._8_48;
            this.radButton8.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton8.Location = new System.Drawing.Point(250, 463);
            this.radButton8.Name = "radButton8";
            this.radButton8.Size = new System.Drawing.Size(77, 68);
            this.radButton8.TabIndex = 16;
            this.radButton8.Tag = "8";
            this.radButton8.ThemeName = "TelerikMetroTouch";
            this.radButton8.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton9
            // 
            this.radButton9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton9.Image = global::POS.Properties.Resources._7_48;
            this.radButton9.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton9.Location = new System.Drawing.Point(172, 463);
            this.radButton9.Name = "radButton9";
            this.radButton9.Size = new System.Drawing.Size(77, 68);
            this.radButton9.TabIndex = 15;
            this.radButton9.Tag = "7";
            this.radButton9.ThemeName = "TelerikMetroTouch";
            this.radButton9.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton4
            // 
            this.radButton4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton4.Image = global::POS.Properties.Resources._6_48;
            this.radButton4.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton4.Location = new System.Drawing.Point(328, 383);
            this.radButton4.Name = "radButton4";
            this.radButton4.Size = new System.Drawing.Size(77, 68);
            this.radButton4.TabIndex = 14;
            this.radButton4.Tag = "6";
            this.radButton4.ThemeName = "TelerikMetroTouch";
            this.radButton4.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton5
            // 
            this.radButton5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton5.Image = global::POS.Properties.Resources._5_48;
            this.radButton5.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton5.Location = new System.Drawing.Point(250, 383);
            this.radButton5.Name = "radButton5";
            this.radButton5.Size = new System.Drawing.Size(77, 68);
            this.radButton5.TabIndex = 13;
            this.radButton5.Tag = "5";
            this.radButton5.ThemeName = "TelerikMetroTouch";
            this.radButton5.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton6
            // 
            this.radButton6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton6.Image = global::POS.Properties.Resources._4_48;
            this.radButton6.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton6.Location = new System.Drawing.Point(172, 383);
            this.radButton6.Name = "radButton6";
            this.radButton6.Size = new System.Drawing.Size(77, 68);
            this.radButton6.TabIndex = 12;
            this.radButton6.Tag = "4";
            this.radButton6.ThemeName = "TelerikMetroTouch";
            this.radButton6.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton3
            // 
            this.radButton3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton3.Image = global::POS.Properties.Resources._3_48;
            this.radButton3.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton3.Location = new System.Drawing.Point(328, 308);
            this.radButton3.Name = "radButton3";
            this.radButton3.Size = new System.Drawing.Size(77, 65);
            this.radButton3.TabIndex = 11;
            this.radButton3.Tag = "3";
            this.radButton3.ThemeName = "TelerikMetroTouch";
            this.radButton3.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton2
            // 
            this.radButton2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton2.Image = global::POS.Properties.Resources._2_48;
            this.radButton2.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton2.Location = new System.Drawing.Point(250, 308);
            this.radButton2.Name = "radButton2";
            this.radButton2.Size = new System.Drawing.Size(77, 65);
            this.radButton2.TabIndex = 10;
            this.radButton2.Tag = "2";
            this.radButton2.ThemeName = "TelerikMetroTouch";
            this.radButton2.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radButton1
            // 
            this.radButton1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radButton1.Image = global::POS.Properties.Resources._1_48;
            this.radButton1.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radButton1.Location = new System.Drawing.Point(172, 308);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(77, 65);
            this.radButton1.TabIndex = 9;
            this.radButton1.Tag = "1";
            this.radButton1.ThemeName = "TelerikMetroTouch";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Image = global::POS.Properties.Resources.cancel_26;
            this.btnCancel.Location = new System.Drawing.Point(198, 258);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 37);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.ThemeName = "TelerikMetroTouch";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLogin.Image = global::POS.Properties.Resources.login_26;
            this.btnLogin.Location = new System.Drawing.Point(332, 258);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(128, 37);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "Login";
            this.btnLogin.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLogin.ThemeName = "TelerikMetroTouch";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = global::POS.Properties.Resources.LogoPrincipal;
            this.pictureBox1.Location = new System.Drawing.Point(82, 59);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(430, 107);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // cmdCambioServidor
            // 
            this.cmdCambioServidor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmdCambioServidor.Image = global::POS.Properties.Resources.login_26;
            this.cmdCambioServidor.Location = new System.Drawing.Point(185, 12);
            this.cmdCambioServidor.Name = "cmdCambioServidor";
            this.cmdCambioServidor.Size = new System.Drawing.Size(222, 39);
            this.cmdCambioServidor.TabIndex = 9;
            this.cmdCambioServidor.Text = "Cambiar Servidor";
            this.cmdCambioServidor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCambioServidor.ThemeName = "TelerikMetroTouch";
            this.cmdCambioServidor.Click += new System.EventHandler(this.cmdCambioServidor_Click);
            // 
            // VerificationControl
            // 
            this.VerificationControl.Active = true;
            this.VerificationControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.VerificationControl.Location = new System.Drawing.Point(427, 177);
            this.VerificationControl.Margin = new System.Windows.Forms.Padding(4);
            this.VerificationControl.Name = "VerificationControl";
            this.VerificationControl.ReaderSerialNumber = "00000000-0000-0000-0000-000000000000";
            this.VerificationControl.Size = new System.Drawing.Size(48, 47);
            this.VerificationControl.TabIndex = 20;
            this.VerificationControl.OnComplete += new DPFP.Gui.Verification.VerificationControl._OnComplete(this.OnComplete);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdCambioServidor);
            this.panel1.Controls.Add(this.VerificationControl);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.radLabel1);
            this.panel1.Controls.Add(this.btnBorrar);
            this.panel1.Controls.Add(this.txtUsername);
            this.panel1.Controls.Add(this.radButton10);
            this.panel1.Controls.Add(this.txtPassword);
            this.panel1.Controls.Add(this.radButton7);
            this.panel1.Controls.Add(this.radLabel2);
            this.panel1.Controls.Add(this.radButton8);
            this.panel1.Controls.Add(this.radLabel3);
            this.panel1.Controls.Add(this.radButton9);
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.radButton4);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.radButton5);
            this.panel1.Controls.Add(this.radButton1);
            this.panel1.Controls.Add(this.radButton6);
            this.panel1.Controls.Add(this.radButton2);
            this.panel1.Controls.Add(this.radButton3);
            this.panel1.Location = new System.Drawing.Point(126, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(706, 615);
            this.panel1.TabIndex = 21;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1016, 654);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Name = "LoginForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login POSax Dynamics KRUP 7";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Initialized += new System.EventHandler(this.LoginForm_Initialized);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.Resize += new System.EventHandler(this.LoginForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.radLabel1.ResumeLayout(false);
            this.radLabel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBorrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmdCambioServidor)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox txtUsername;
        private Telerik.WinControls.UI.RadTextBox txtPassword;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.Themes.TelerikMetroTouchTheme telerikMetroTouchTheme1;
        private Telerik.WinControls.UI.RadButton btnLogin;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.UI.RadButton radButton1;
        private Telerik.WinControls.UI.RadButton radButton2;
        private Telerik.WinControls.UI.RadButton radButton3;
        private Telerik.WinControls.UI.RadButton radButton4;
        private Telerik.WinControls.UI.RadButton radButton5;
        private Telerik.WinControls.UI.RadButton radButton6;
        private Telerik.WinControls.UI.RadButton radButton7;
        private Telerik.WinControls.UI.RadButton radButton8;
        private Telerik.WinControls.UI.RadButton radButton9;
        private Telerik.WinControls.UI.RadButton radButton10;
        private Telerik.WinControls.UI.RadButton btnBorrar;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Telerik.WinControls.UI.RadButton cmdCambioServidor;
        private DPFP.Gui.Verification.VerificationControl VerificationControl;
        private System.Windows.Forms.Panel panel1;
    }
}
