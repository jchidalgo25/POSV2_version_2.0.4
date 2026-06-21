namespace POS.Control.Parking
{
    partial class CapturaParqueo
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
            this.btnConfirmar = new Telerik.WinControls.UI.RadButton();
            this.txtCodigo = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.btnValidar = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.lblValidacion = new Telerik.WinControls.UI.RadLabel();
            this.chkEnlazarFactura = new System.Windows.Forms.CheckBox();
            this.boxEnlazarFactura = new System.Windows.Forms.GroupBox();
            this.lklCancelar = new System.Windows.Forms.LinkLabel();
            this.txtPtoEmisionEnlazar = new Telerik.WinControls.UI.RadTextBox();
            this.txtNroFacturaEnlazar = new Telerik.WinControls.UI.RadTextBox();
            this.lblFacEnlazar = new Telerik.WinControls.UI.RadLabel();
            this.lblFacEnlazarAyuda = new Telerik.WinControls.UI.RadLabel();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.telerikMetroTouchTheme1 = new Telerik.WinControls.Themes.TelerikMetroTouchTheme();
            this.btnNoTicket = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirmar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblValidacion)).BeginInit();
            this.boxEnlazarFactura.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtoEmisionEnlazar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroFacturaEnlazar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacEnlazar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacEnlazarAyuda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNoTicket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Enabled = false;
            this.btnConfirmar.Location = new System.Drawing.Point(402, 200);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(169, 46);
            this.btnConfirmar.TabIndex = 25;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.TextWrap = true;
            this.btnConfirmar.ThemeName = "TelerikMetroTouch";
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // txtCodigo
            // 
            this.txtCodigo.BackColor = System.Drawing.Color.Gainsboro;
            this.txtCodigo.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.Location = new System.Drawing.Point(134, 39);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.NullText = "Pistolear código...";
            this.txtCodigo.Size = new System.Drawing.Size(420, 46);
            this.txtCodigo.TabIndex = 15;
            this.txtCodigo.TabStop = false;
            this.txtCodigo.ThemeName = "TelerikMetroTouch";
            this.txtCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(32, 55);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(80, 30);
            this.radLabel4.TabIndex = 21;
            this.radLabel4.Text = "Código:";
            this.radLabel4.ThemeName = "TelerikMetroTouch";
            // 
            // btnValidar
            // 
            this.btnValidar.Location = new System.Drawing.Point(551, 39);
            this.btnValidar.Name = "btnValidar";
            this.btnValidar.Size = new System.Drawing.Size(80, 46);
            this.btnValidar.TabIndex = 16;
            this.btnValidar.Text = "Validar";
            this.btnValidar.TextWrap = true;
            this.btnValidar.ThemeName = "TelerikMetroTouch";
            this.btnValidar.Click += new System.EventHandler(this.btnValidar_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(0, 1);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(711, 25);
            this.radLabel1.TabIndex = 22;
            this.radLabel1.Text = "Pistolear el código de barras para validar el tiempo del parqueo y luego pulse en" +
    " confirmar";
            this.radLabel1.ThemeName = "TelerikMetroTouch";
            // 
            // lblValidacion
            // 
            this.lblValidacion.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValidacion.Location = new System.Drawing.Point(134, 88);
            this.lblValidacion.Name = "lblValidacion";
            this.lblValidacion.Size = new System.Drawing.Size(173, 29);
            this.lblValidacion.TabIndex = 23;
            this.lblValidacion.Text = "Hora ingreso: 08:45";
            this.lblValidacion.ThemeName = "TelerikMetroTouch";
            // 
            // chkEnlazarFactura
            // 
            this.chkEnlazarFactura.AutoSize = true;
            this.chkEnlazarFactura.Location = new System.Drawing.Point(264, 135);
            this.chkEnlazarFactura.Name = "chkEnlazarFactura";
            this.chkEnlazarFactura.Size = new System.Drawing.Size(215, 17);
            this.chkEnlazarFactura.TabIndex = 24;
            this.chkEnlazarFactura.Text = "Enlazar con otra factura (Gerente Local)";
            this.chkEnlazarFactura.UseVisualStyleBackColor = true;
            this.chkEnlazarFactura.CheckedChanged += new System.EventHandler(this.chkEnlazarFactura_CheckedChanged);
            // 
            // boxEnlazarFactura
            // 
            this.boxEnlazarFactura.BackColor = System.Drawing.Color.Transparent;
            this.boxEnlazarFactura.Controls.Add(this.lklCancelar);
            this.boxEnlazarFactura.Controls.Add(this.txtPtoEmisionEnlazar);
            this.boxEnlazarFactura.Controls.Add(this.txtNroFacturaEnlazar);
            this.boxEnlazarFactura.Controls.Add(this.lblFacEnlazar);
            this.boxEnlazarFactura.Controls.Add(this.lblFacEnlazarAyuda);
            this.boxEnlazarFactura.Controls.Add(this.radLabel7);
            this.boxEnlazarFactura.Controls.Add(this.radLabel3);
            this.boxEnlazarFactura.Location = new System.Drawing.Point(134, 121);
            this.boxEnlazarFactura.Name = "boxEnlazarFactura";
            this.boxEnlazarFactura.Size = new System.Drawing.Size(437, 70);
            this.boxEnlazarFactura.TabIndex = 26;
            this.boxEnlazarFactura.TabStop = false;
            this.boxEnlazarFactura.Text = "Enlazar otra factura";
            this.boxEnlazarFactura.Visible = false;
            // 
            // lklCancelar
            // 
            this.lklCancelar.AutoSize = true;
            this.lklCancelar.Location = new System.Drawing.Point(8, 45);
            this.lklCancelar.Name = "lklCancelar";
            this.lklCancelar.Size = new System.Drawing.Size(61, 13);
            this.lklCancelar.TabIndex = 19;
            this.lklCancelar.TabStop = true;
            this.lklCancelar.Text = "<- Cancelar";
            this.lklCancelar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lklCancelar_LinkClicked);
            // 
            // txtPtoEmisionEnlazar
            // 
            this.txtPtoEmisionEnlazar.BackColor = System.Drawing.Color.AntiqueWhite;
            this.txtPtoEmisionEnlazar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPtoEmisionEnlazar.Location = new System.Drawing.Point(182, 12);
            this.txtPtoEmisionEnlazar.MaxLength = 3;
            this.txtPtoEmisionEnlazar.Name = "txtPtoEmisionEnlazar";
            this.txtPtoEmisionEnlazar.Size = new System.Drawing.Size(71, 32);
            this.txtPtoEmisionEnlazar.TabIndex = 17;
            this.txtPtoEmisionEnlazar.TabStop = false;
            this.txtPtoEmisionEnlazar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPtoEmisionEnlazar.ThemeName = "TelerikMetroTouch";
            this.txtPtoEmisionEnlazar.TextChanged += new System.EventHandler(this.txtPtoEmisionEnlazar_TextChanged);
            // 
            // txtNroFacturaEnlazar
            // 
            this.txtNroFacturaEnlazar.BackColor = System.Drawing.Color.AntiqueWhite;
            this.txtNroFacturaEnlazar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNroFacturaEnlazar.Location = new System.Drawing.Point(259, 12);
            this.txtNroFacturaEnlazar.MaxLength = 9;
            this.txtNroFacturaEnlazar.Name = "txtNroFacturaEnlazar";
            this.txtNroFacturaEnlazar.Size = new System.Drawing.Size(157, 32);
            this.txtNroFacturaEnlazar.TabIndex = 18;
            this.txtNroFacturaEnlazar.TabStop = false;
            this.txtNroFacturaEnlazar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNroFacturaEnlazar.ThemeName = "TelerikMetroTouch";
            this.txtNroFacturaEnlazar.TextChanged += new System.EventHandler(this.txtNroFacturaEnlazar_TextChanged);
            // 
            // lblFacEnlazar
            // 
            this.lblFacEnlazar.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacEnlazar.ForeColor = System.Drawing.Color.DarkGray;
            this.lblFacEnlazar.Location = new System.Drawing.Point(305, 44);
            this.lblFacEnlazar.Name = "lblFacEnlazar";
            this.lblFacEnlazar.Size = new System.Drawing.Size(120, 18);
            this.lblFacEnlazar.TabIndex = 20;
            this.lblFacEnlazar.Text = "F-029-000-000000000";
            this.lblFacEnlazar.ThemeName = "TelerikMetroTouch";
            // 
            // lblFacEnlazarAyuda
            // 
            this.lblFacEnlazarAyuda.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacEnlazarAyuda.ForeColor = System.Drawing.Color.Gray;
            this.lblFacEnlazarAyuda.Location = new System.Drawing.Point(120, 12);
            this.lblFacEnlazarAyuda.Name = "lblFacEnlazarAyuda";
            this.lblFacEnlazarAyuda.Size = new System.Drawing.Size(69, 30);
            this.lblFacEnlazarAyuda.TabIndex = 19;
            this.lblFacEnlazarAyuda.Text = "F-029-";
            this.lblFacEnlazarAyuda.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel7
            // 
            this.radLabel7.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel7.Location = new System.Drawing.Point(6, 12);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(82, 30);
            this.radLabel7.TabIndex = 18;
            this.radLabel7.Text = "Factura:";
            this.radLabel7.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.ForeColor = System.Drawing.Color.Gray;
            this.radLabel3.Location = new System.Drawing.Point(248, 12);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(17, 30);
            this.radLabel3.TabIndex = 20;
            this.radLabel3.Text = "-";
            this.radLabel3.ThemeName = "TelerikMetroTouch";
            // 
            // btnNoTicket
            // 
            this.btnNoTicket.Location = new System.Drawing.Point(134, 200);
            this.btnNoTicket.Name = "btnNoTicket";
            this.btnNoTicket.Size = new System.Drawing.Size(169, 46);
            this.btnNoTicket.TabIndex = 26;
            this.btnNoTicket.Text = "No tiene ticket";
            this.btnNoTicket.TextWrap = true;
            this.btnNoTicket.ThemeName = "TelerikMetroTouch";
            this.btnNoTicket.Click += new System.EventHandler(this.btnNoTicket_Click);
            // 
            // CapturaParqueo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 267);
            this.Controls.Add(this.btnNoTicket);
            this.Controls.Add(this.chkEnlazarFactura);
            this.Controls.Add(this.lblValidacion);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.btnValidar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.boxEnlazarFactura);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CapturaParqueo";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Parking";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CapturaParqueo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirmar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblValidacion)).EndInit();
            this.boxEnlazarFactura.ResumeLayout(false);
            this.boxEnlazarFactura.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPtoEmisionEnlazar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroFacturaEnlazar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacEnlazar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacEnlazarAyuda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNoTicket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnConfirmar;
        private Telerik.WinControls.UI.RadTextBox txtCodigo;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadButton btnValidar;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel lblValidacion;
        private System.Windows.Forms.CheckBox chkEnlazarFactura;
        private System.Windows.Forms.GroupBox boxEnlazarFactura;
        private Telerik.WinControls.UI.RadTextBox txtPtoEmisionEnlazar;
        private Telerik.WinControls.UI.RadTextBox txtNroFacturaEnlazar;
        private Telerik.WinControls.UI.RadLabel lblFacEnlazar;
        private Telerik.WinControls.UI.RadLabel lblFacEnlazarAyuda;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private System.Windows.Forms.LinkLabel lklCancelar;
        private Telerik.WinControls.Themes.TelerikMetroTouchTheme telerikMetroTouchTheme1;
        private Telerik.WinControls.UI.RadButton btnNoTicket;
    }
}
