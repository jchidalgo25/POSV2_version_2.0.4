namespace POS.Control.Giftcard
{
    partial class GiftcardTransReprint
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
            this.txtGiftcard = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.btnImprimir = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtGiftcard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnImprimir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // txtGiftcard
            // 
            this.txtGiftcard.BackColor = System.Drawing.Color.Gainsboro;
            this.txtGiftcard.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGiftcard.Location = new System.Drawing.Point(168, 12);
            this.txtGiftcard.Name = "txtGiftcard";
            this.txtGiftcard.NullText = "Ingrese giftcard...";
            this.txtGiftcard.Size = new System.Drawing.Size(420, 46);
            this.txtGiftcard.TabIndex = 17;
            this.txtGiftcard.TabStop = false;
            this.txtGiftcard.ThemeName = "TelerikMetroTouch";
            this.txtGiftcard.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGiftcard_KeyPress);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(42, 28);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(89, 30);
            this.radLabel4.TabIndex = 18;
            this.radLabel4.Text = "Giftcard:";
            this.radLabel4.ThemeName = "TelerikMetroTouch";
            // 
            // btnImprimir
            // 
            this.btnImprimir.Location = new System.Drawing.Point(272, 99);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(169, 46);
            this.btnImprimir.TabIndex = 19;
            this.btnImprimir.Text = "Imprimir";
            this.btnImprimir.TextWrap = true;
            this.btnImprimir.ThemeName = "TelerikMetroTouch";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // GiftcardTransReprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 171);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.txtGiftcard);
            this.Controls.Add(this.radLabel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GiftcardTransReprint";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reimpresión de transacción giftcard";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.txtGiftcard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnImprimir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadTextBox txtGiftcard;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadButton btnImprimir;
    }
}
