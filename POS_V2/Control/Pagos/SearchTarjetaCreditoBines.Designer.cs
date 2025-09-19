namespace POS.Control.Clientes
{
    partial class SearchTarjetaCreditoBines
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
            this.lblNumeroTarjeta = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnKb = new System.Windows.Forms.Button();
            this.txtNumTarjeta = new Telerik.WinControls.UI.RadTextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.txtNumTarjeta2 = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumTarjeta)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumTarjeta2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNumeroTarjeta
            // 
            this.lblNumeroTarjeta.AutoSize = true;
            this.lblNumeroTarjeta.BackColor = System.Drawing.Color.CornflowerBlue;
            this.lblNumeroTarjeta.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroTarjeta.ForeColor = System.Drawing.Color.White;
            this.lblNumeroTarjeta.Location = new System.Drawing.Point(59, 8);
            this.lblNumeroTarjeta.Name = "lblNumeroTarjeta";
            this.lblNumeroTarjeta.Size = new System.Drawing.Size(399, 24);
            this.lblNumeroTarjeta.TabIndex = 1;
            this.lblNumeroTarjeta.Text = "Ingrese los 6 primeros dígitos de la tarjeta";
            // 
            // btnAceptar
            // 
            this.btnAceptar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAceptar.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnAceptar.Location = new System.Drawing.Point(160, 220);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(110, 41);
            this.btnAceptar.TabIndex = 4;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnExit.Location = new System.Drawing.Point(49, 220);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(110, 41);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Salir";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnKb
            // 
            this.btnKb.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnKb.Location = new System.Drawing.Point(421, 177);
            this.btnKb.Name = "btnKb";
            this.btnKb.Size = new System.Drawing.Size(44, 34);
            this.btnKb.TabIndex = 1;
            this.btnKb.Text = "7";
            this.btnKb.UseVisualStyleBackColor = true;
            this.btnKb.Click += new System.EventHandler(this.btnKb_Click);
            // 
            // txtNumTarjeta
            // 
            this.txtNumTarjeta.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtNumTarjeta.Location = new System.Drawing.Point(161, 141);
            this.txtNumTarjeta.MaxLength = 2;
            this.txtNumTarjeta.Name = "txtNumTarjeta";
            this.txtNumTarjeta.NullText = "99";
            // 
            // 
            // 
            this.txtNumTarjeta.RootElement.ControlBounds = new System.Drawing.Rectangle(53, 61, 100, 20);
            this.txtNumTarjeta.RootElement.StretchVertically = true;
            this.txtNumTarjeta.Size = new System.Drawing.Size(47, 34);
            this.txtNumTarjeta.TabIndex = 1;
            this.txtNumTarjeta.TabStop = false;
            this.txtNumTarjeta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtNumTarjeta.ThemeName = "TelerikMetroTouch";
            this.txtNumTarjeta.Enter += new System.EventHandler(this.txtNumTarjeta_Enter);
            this.txtNumTarjeta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumTarjeta_KeyPress);
            this.txtNumTarjeta.Leave += new System.EventHandler(this.txtNumTarjeta_Leave);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Wingdings 3", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSearch.Location = new System.Drawing.Point(382, 177);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(44, 34);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "8";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::POS.Properties.Resources.visa5;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtNumTarjeta2);
            this.panel1.Controls.Add(this.radLabel3);
            this.panel1.Controls.Add(this.radLabel2);
            this.panel1.Controls.Add(this.radLabel1);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.lblNumeroTarjeta);
            this.panel1.Controls.Add(this.txtNumTarjeta);
            this.panel1.Controls.Add(this.btnAceptar);
            this.panel1.Controls.Add(this.btnKb);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(518, 276);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // radLabel2
            // 
            this.radLabel2.AutoSize = false;
            this.radLabel2.BackColor = System.Drawing.Color.White;
            this.radLabel2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.ForeColor = System.Drawing.Color.Gray;
            this.radLabel2.Location = new System.Drawing.Point(208, 141);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(38, 34);
            this.radLabel2.TabIndex = 22;
            this.radLabel2.Text = "XX";
            this.radLabel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radLabel2.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel1
            // 
            this.radLabel1.AutoSize = false;
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.ForeColor = System.Drawing.Color.Gray;
            this.radLabel1.Location = new System.Drawing.Point(382, 141);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(83, 34);
            this.radLabel1.TabIndex = 21;
            this.radLabel1.Text = "XXXX";
            this.radLabel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radLabel1.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel3
            // 
            this.radLabel3.AutoSize = false;
            this.radLabel3.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.ForeColor = System.Drawing.Color.Gray;
            this.radLabel3.Location = new System.Drawing.Point(273, 141);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(83, 34);
            this.radLabel3.TabIndex = 23;
            this.radLabel3.Text = "XXXX";
            this.radLabel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radLabel3.ThemeName = "TelerikMetroTouch";
            // 
            // txtNumTarjeta2
            // 
            this.txtNumTarjeta2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtNumTarjeta2.Location = new System.Drawing.Point(49, 141);
            this.txtNumTarjeta2.MaxLength = 4;
            this.txtNumTarjeta2.Name = "txtNumTarjeta2";
            this.txtNumTarjeta2.NullText = "9999";
            // 
            // 
            // 
            this.txtNumTarjeta2.RootElement.ControlBounds = new System.Drawing.Rectangle(41, 141, 100, 20);
            this.txtNumTarjeta2.RootElement.StretchVertically = true;
            this.txtNumTarjeta2.Size = new System.Drawing.Size(87, 34);
            this.txtNumTarjeta2.TabIndex = 0;
            this.txtNumTarjeta2.TabStop = false;
            this.txtNumTarjeta2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtNumTarjeta2.ThemeName = "TelerikMetroTouch";
            // 
            // SearchTarjetaCreditoBines
            // 
            this.AcceptButton = this.btnAceptar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(523, 276);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchTarjetaCreditoBines";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Búsqueda de Productos";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SearchTarjetaCreditoBines_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtNumTarjeta)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumTarjeta2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNumeroTarjeta;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnKb;
        private Telerik.WinControls.UI.RadTextBox txtNumTarjeta;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox txtNumTarjeta2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
    }
}