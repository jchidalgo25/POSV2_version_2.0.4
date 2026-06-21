namespace POS.Control.Giftcard
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
            this.btnVentaGiftcard = new Telerik.WinControls.UI.RadButton();
            this.btnCambiarTipoDoc = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnVentaGiftcard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCambiarTipoDoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVentaGiftcard
            // 
            this.btnVentaGiftcard.Location = new System.Drawing.Point(296, 8);
            this.btnVentaGiftcard.Name = "btnVentaGiftcard";
            this.btnVentaGiftcard.Size = new System.Drawing.Size(153, 208);
            this.btnVentaGiftcard.TabIndex = 15;
            this.btnVentaGiftcard.Text = "Venta Giftcard";
            this.btnVentaGiftcard.TextWrap = true;
            this.btnVentaGiftcard.ThemeName = "TelerikMetroTouch";
            this.btnVentaGiftcard.Click += new System.EventHandler(this.btnPagoServicios_Click);
            // 
            // btnCambiarTipoDoc
            // 
            this.btnCambiarTipoDoc.Location = new System.Drawing.Point(42, 8);
            this.btnCambiarTipoDoc.Name = "btnCambiarTipoDoc";
            this.btnCambiarTipoDoc.Size = new System.Drawing.Size(153, 208);
            this.btnCambiarTipoDoc.TabIndex = 16;
            this.btnCambiarTipoDoc.Text = "Giftcard";
            this.btnCambiarTipoDoc.TextWrap = true;
            this.btnCambiarTipoDoc.ThemeName = "TelerikMetroTouch";
            this.btnCambiarTipoDoc.Click += new System.EventHandler(this.btnRecargas_Click);
            // 
            // MenuPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 252);
            this.Controls.Add(this.btnCambiarTipoDoc);
            this.Controls.Add(this.btnVentaGiftcard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MenuPrincipal";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menú Giftcard";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.btnVentaGiftcard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCambiarTipoDoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public Telerik.WinControls.UI.RadButton btnVentaGiftcard;
        public Telerik.WinControls.UI.RadButton btnCambiarTipoDoc;
    }
}
