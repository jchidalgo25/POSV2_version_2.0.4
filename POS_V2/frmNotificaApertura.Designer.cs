namespace POS
{
    partial class frmNotificaApertura
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNotificaApertura));
            this.cmdCancel = new Telerik.WinControls.UI.RadButton();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMonto = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.cmdOK = new Telerik.WinControls.UI.RadButton();
            this.chkAceptar = new System.Windows.Forms.CheckBox();
            this.lblCaja = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblPassword = new Telerik.WinControls.UI.RadLabel();
            this.txtPassword = new Telerik.WinControls.UI.RadTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.WebTerminos = new System.Windows.Forms.WebBrowser();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Picture = new System.Windows.Forms.PictureBox();
            this.panelCambiarServer = new System.Windows.Forms.Panel();
            this.cmdCambioServidor = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.cmdCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmdOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.panelCambiarServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmdCambioServidor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(712, 502);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(2);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(132, 55);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancelar";
            this.cmdCancel.ThemeName = "TelerikMetroTouch";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(44, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "USUARIO:";
            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonto.Location = new System.Drawing.Point(435, 43);
            this.lblMonto.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(226, 24);
            this.lblMonto.TabIndex = 5;
            this.lblMonto.Text = "Usted recibio $ {suelto}";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(140, 10);
            this.lblUsuario.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(253, 24);
            this.lblUsuario.TabIndex = 6;
            this.lblUsuario.Text = "{NOMBRE_DE_USUARIO}";
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(576, 502);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(2);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(132, 55);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "CONTINUAR";
            this.cmdOK.ThemeName = "TelerikMetroTouch";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // chkAceptar
            // 
            this.chkAceptar.AutoSize = true;
            this.chkAceptar.Location = new System.Drawing.Point(323, 516);
            this.chkAceptar.Margin = new System.Windows.Forms.Padding(2);
            this.chkAceptar.Name = "chkAceptar";
            this.chkAceptar.Size = new System.Drawing.Size(190, 17);
            this.chkAceptar.TabIndex = 7;
            this.chkAceptar.Text = "Acepta los Terminos y condiciones";
            this.chkAceptar.UseVisualStyleBackColor = true;
            this.chkAceptar.CheckedChanged += new System.EventHandler(this.chkAceptar_CheckedChanged);
            // 
            // lblCaja
            // 
            this.lblCaja.AutoSize = true;
            this.lblCaja.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaja.Location = new System.Drawing.Point(140, 43);
            this.lblCaja.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCaja.Name = "lblCaja";
            this.lblCaja.Size = new System.Drawing.Size(177, 24);
            this.lblCaja.TabIndex = 10;
            this.lblCaja.Text = "{NUMERO_CAJA}";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(12, 50);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "PUNTO VENTA:";
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPassword.Location = new System.Drawing.Point(14, 504);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(2);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(86, 23);
            this.lblPassword.TabIndex = 12;
            this.lblPassword.Text = "Contraseña:";
            this.lblPassword.ThemeName = "TelerikMetroTouch";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtPassword.Location = new System.Drawing.Point(95, 502);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtPassword.MaxLength = 50;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.NullText = "Contraseña ej. 1234";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(127, 30);
            this.txtPassword.TabIndex = 11;
            this.txtPassword.TabStop = false;
            this.txtPassword.ThemeName = "TelerikMetroTouch";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.AntiqueWhite;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.WebTerminos);
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.txtPassword);
            this.panel1.Controls.Add(this.lblCaja);
            this.panel1.Controls.Add(this.lblPassword);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.chkAceptar);
            this.panel1.Controls.Add(this.cmdOK);
            this.panel1.Controls.Add(this.lblUsuario);
            this.panel1.Controls.Add(this.cmdCancel);
            this.panel1.Controls.Add(this.lblMonto);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(876, 570);
            this.panel1.TabIndex = 15;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label2.Location = new System.Drawing.Point(379, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "SUELTO:";
            // 
            // WebTerminos
            // 
            this.WebTerminos.AllowNavigation = false;
            this.WebTerminos.AllowWebBrowserDrop = false;
            this.WebTerminos.CausesValidation = false;
            this.WebTerminos.IsWebBrowserContextMenuEnabled = false;
            this.WebTerminos.Location = new System.Drawing.Point(15, 102);
            this.WebTerminos.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebTerminos.Name = "WebTerminos";
            this.WebTerminos.Size = new System.Drawing.Size(847, 395);
            this.WebTerminos.TabIndex = 17;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.DarkRed;
            this.lblStatus.Location = new System.Drawing.Point(5, 538);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 19;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::POS.Properties.Resources.Logo_Horizontal_DELPORTAL;
            this.pictureBox1.Location = new System.Drawing.Point(739, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(105, 92);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // Picture
            // 
            this.Picture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Picture.BackColor = System.Drawing.SystemColors.Window;
            this.Picture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Picture.InitialImage = null;
            this.Picture.Location = new System.Drawing.Point(1132, 172);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(413, 291);
            this.Picture.TabIndex = 18;
            this.Picture.TabStop = false;
            this.Picture.Visible = false;
            this.Picture.Click += new System.EventHandler(this.Picture_Click);
            // 
            // panelCambiarServer
            // 
            this.panelCambiarServer.Controls.Add(this.cmdCambioServidor);
            this.panelCambiarServer.Controls.Add(this.radLabel1);
            this.panelCambiarServer.Location = new System.Drawing.Point(12, 586);
            this.panelCambiarServer.Name = "panelCambiarServer";
            this.panelCambiarServer.Size = new System.Drawing.Size(876, 138);
            this.panelCambiarServer.TabIndex = 16;
            this.panelCambiarServer.Visible = false;
            // 
            // cmdCambioServidor
            // 
            this.cmdCambioServidor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmdCambioServidor.Image = global::POS.Properties.Resources.login_26;
            this.cmdCambioServidor.Location = new System.Drawing.Point(327, 97);
            this.cmdCambioServidor.Name = "cmdCambioServidor";
            this.cmdCambioServidor.Size = new System.Drawing.Size(222, 36);
            this.cmdCambioServidor.TabIndex = 10;
            this.cmdCambioServidor.Text = "Cambiar Servidor";
            this.cmdCambioServidor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCambioServidor.ThemeName = "TelerikMetroTouch";
            this.cmdCambioServidor.Click += new System.EventHandler(this.cmdCambioServidor_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radLabel1.AutoSize = false;
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.radLabel1.ForeColor = System.Drawing.Color.DarkRed;
            this.radLabel1.Location = new System.Drawing.Point(13, 4);
            this.radLabel1.Margin = new System.Windows.Forms.Padding(2);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(849, 96);
            this.radLabel1.TabIndex = 13;
            this.radLabel1.Text = resources.GetString("radLabel1.Text");
            this.radLabel1.ThemeName = "TelerikMetroTouch";
            this.radLabel1.Click += new System.EventHandler(this.radLabel1_Click);
            // 
            // frmNotificaApertura
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(1900, 733);
            this.ControlBox = false;
            this.Controls.Add(this.panelCambiarServer);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmNotificaApertura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apertura de Caja";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmNotificaApertura_FormClosed);
            this.Load += new System.EventHandler(this.frmNotificaApertura_Load);
            this.Resize += new System.EventHandler(this.frmNotificaApertura_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.cmdCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmdOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.panelCambiarServer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmdCambioServidor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadButton cmdCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.Label lblUsuario;
        private Telerik.WinControls.UI.RadButton cmdOK;
        private System.Windows.Forms.CheckBox chkAceptar;
        private System.Windows.Forms.Label lblCaja;
        private System.Windows.Forms.Label label4;
        private Telerik.WinControls.UI.RadLabel lblPassword;
        private Telerik.WinControls.UI.RadTextBox txtPassword;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.WebBrowser WebTerminos;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelCambiarServer;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadButton cmdCambioServidor;
        private System.Windows.Forms.Label label2;
    }
}