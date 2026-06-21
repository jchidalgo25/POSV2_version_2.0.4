namespace POS.Control.WalletPoints
{
    partial class CapturaCupon
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
            this.lblValidacion = new Telerik.WinControls.UI.RadLabel();
            this.telerikMetroTouchTheme1 = new Telerik.WinControls.Themes.TelerikMetroTouchTheme();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.txtCodArticulo = new Telerik.WinControls.UI.RadTextBox();
            this.lblDescArticulo = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirmar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblValidacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodArticulo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDescArticulo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Enabled = false;
            this.btnConfirmar.Location = new System.Drawing.Point(115, 289);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(453, 71);
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
            this.txtCodigo.Location = new System.Drawing.Point(187, 186);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.NullText = "Pistolear código...";
            this.txtCodigo.Size = new System.Drawing.Size(420, 46);
            this.txtCodigo.TabIndex = 16;
            this.txtCodigo.TabStop = false;
            this.txtCodigo.ThemeName = "TelerikMetroTouch";
            this.txtCodigo.TextChanged += new System.EventHandler(this.txtCodigo_TextChanged);
            this.txtCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(12, 143);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(250, 30);
            this.radLabel4.TabIndex = 21;
            this.radLabel4.Text = "Escanear código de Cupón";
            this.radLabel4.ThemeName = "TelerikMetroTouch";
            // 
            // btnValidar
            // 
            this.btnValidar.Location = new System.Drawing.Point(613, 186);
            this.btnValidar.Name = "btnValidar";
            this.btnValidar.Size = new System.Drawing.Size(103, 46);
            this.btnValidar.TabIndex = 17;
            this.btnValidar.Text = "Validar";
            this.btnValidar.TextWrap = true;
            this.btnValidar.ThemeName = "TelerikMetroTouch";
            this.btnValidar.Click += new System.EventHandler(this.btnValidar_Click);
            // 
            // lblValidacion
            // 
            this.lblValidacion.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValidacion.Location = new System.Drawing.Point(187, 247);
            this.lblValidacion.Name = "lblValidacion";
            this.lblValidacion.Size = new System.Drawing.Size(148, 29);
            this.lblValidacion.TabIndex = 23;
            this.lblValidacion.Text = "Datos del cupón";
            this.lblValidacion.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(12, 4);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(278, 30);
            this.radLabel2.TabIndex = 22;
            this.radLabel2.Text = "Escanear Código de Producto";
            this.radLabel2.ThemeName = "TelerikMetroTouch";
            // 
            // txtCodArticulo
            // 
            this.txtCodArticulo.BackColor = System.Drawing.Color.Gainsboro;
            this.txtCodArticulo.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodArticulo.Location = new System.Drawing.Point(187, 47);
            this.txtCodArticulo.Name = "txtCodArticulo";
            this.txtCodArticulo.NullText = "Pistolear código Artículo...";
            this.txtCodArticulo.Size = new System.Drawing.Size(420, 46);
            this.txtCodArticulo.TabIndex = 15;
            this.txtCodArticulo.TabStop = false;
            this.txtCodArticulo.ThemeName = "TelerikMetroTouch";
            this.txtCodArticulo.TextChanged += new System.EventHandler(this.txtCodArticulo_TextChanged);
            this.txtCodArticulo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodArticulo_KeyPress);
            this.txtCodArticulo.LostFocus += new System.EventHandler(this.txtCodArticulo_LostFocus);
            // 
            // lblDescArticulo
            // 
            this.lblDescArticulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescArticulo.Location = new System.Drawing.Point(185, 101);
            this.lblDescArticulo.Name = "lblDescArticulo";
            this.lblDescArticulo.Size = new System.Drawing.Size(179, 29);
            this.lblDescArticulo.TabIndex = 24;
            this.lblDescArticulo.Text = "Descripción Artículo";
            this.lblDescArticulo.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(50, 56);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(104, 30);
            this.radLabel1.TabIndex = 24;
            this.radLabel1.Text = "Producto: ";
            this.radLabel1.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.Location = new System.Drawing.Point(88, 201);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(74, 30);
            this.radLabel3.TabIndex = 23;
            this.radLabel3.Text = "Cupón:";
            this.radLabel3.ThemeName = "TelerikMetroTouch";
            // 
            // CapturaCupon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 367);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.lblDescArticulo);
            this.Controls.Add(this.txtCodArticulo);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.lblValidacion);
            this.Controls.Add(this.btnValidar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.radLabel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CapturaCupon";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cupón App";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CapturaParqueo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnConfirmar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblValidacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodArticulo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDescArticulo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnConfirmar;
        private Telerik.WinControls.UI.RadTextBox txtCodigo;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadButton btnValidar;
        private Telerik.WinControls.UI.RadLabel lblValidacion;
        private Telerik.WinControls.Themes.TelerikMetroTouchTheme telerikMetroTouchTheme1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadTextBox txtCodArticulo;
        private Telerik.WinControls.UI.RadLabel lblDescArticulo;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel3;
    }
}
