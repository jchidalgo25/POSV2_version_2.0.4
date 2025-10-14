namespace POS.Control.WalletPoints
{
    partial class MenuPrincipal
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
            this.btnPortalCanje = new Telerik.WinControls.UI.RadButton();
            this.btnAyuda = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnPortalCanje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAyuda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPortalCanje
            // 
            this.btnPortalCanje.Location = new System.Drawing.Point(296, 8);
            this.btnPortalCanje.Name = "btnPortalCanje";
            this.btnPortalCanje.Size = new System.Drawing.Size(153, 208);
            this.btnPortalCanje.TabIndex = 15;
            this.btnPortalCanje.Text = "Portal Canje";
            this.btnPortalCanje.TextWrap = true;
            this.btnPortalCanje.ThemeName = "TelerikMetroTouch";
            this.btnPortalCanje.Click += new System.EventHandler(this.btnPagoServicios_Click);
            // 
            // btnAyuda
            // 
            this.btnAyuda.Location = new System.Drawing.Point(42, 8);
            this.btnAyuda.Name = "btnAyuda";
            this.btnAyuda.Size = new System.Drawing.Size(153, 208);
            this.btnAyuda.TabIndex = 16;
            this.btnAyuda.Text = "Ayuda";
            this.btnAyuda.TextWrap = true;
            this.btnAyuda.ThemeName = "TelerikMetroTouch";
            this.btnAyuda.Click += new System.EventHandler(this.btnRecargas_Click);
            // 
            // MenuPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 252);
            this.Controls.Add(this.btnAyuda);
            this.Controls.Add(this.btnPortalCanje);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MenuPrincipal";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Puntos Delportal";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.btnPortalCanje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAyuda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public Telerik.WinControls.UI.RadButton btnPortalCanje;
        public Telerik.WinControls.UI.RadButton btnAyuda;
    }
}
