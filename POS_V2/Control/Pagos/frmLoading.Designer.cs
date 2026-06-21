namespace POS.Control.Pagos
{
    partial class frmLoading
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
            this.paneLoading = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblSeguimientoEvent = new Telerik.WinControls.UI.RadLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.paneLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblSeguimientoEvent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            this.SuspendLayout();
            // 
            // paneLoading
            // 
            this.paneLoading.BackColor = System.Drawing.Color.White;
            this.paneLoading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paneLoading.Controls.Add(this.progressBar1);
            this.paneLoading.Controls.Add(this.lblSeguimientoEvent);
            this.paneLoading.Controls.Add(this.pictureBox2);
            this.paneLoading.Controls.Add(this.radLabel7);
            this.paneLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneLoading.Location = new System.Drawing.Point(0, 0);
            this.paneLoading.Name = "paneLoading";
            this.paneLoading.Size = new System.Drawing.Size(694, 283);
            this.paneLoading.TabIndex = 38;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(23, 255);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(666, 23);
            this.progressBar1.TabIndex = 39;
            // 
            // lblSeguimientoEvent
            // 
            this.lblSeguimientoEvent.BorderVisible = true;
            this.lblSeguimientoEvent.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeguimientoEvent.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.lblSeguimientoEvent.Location = new System.Drawing.Point(294, 109);
            this.lblSeguimientoEvent.Name = "lblSeguimientoEvent";
            this.lblSeguimientoEvent.Size = new System.Drawing.Size(306, 21);
            this.lblSeguimientoEvent.TabIndex = 9;
            this.lblSeguimientoEvent.Text = "un momento por favor, este proceso puede demorar";
            this.lblSeguimientoEvent.ThemeName = "TelerikMetroTouch";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Image = global::POS.Properties.Resources.loading;
            this.pictureBox2.Location = new System.Drawing.Point(54, 46);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(184, 158);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // radLabel7
            // 
            this.radLabel7.Font = new System.Drawing.Font("Segoe UI", 28.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel7.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.radLabel7.Location = new System.Drawing.Point(271, 46);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(233, 57);
            this.radLabel7.TabIndex = 7;
            this.radLabel7.Text = "Procesando...";
            this.radLabel7.ThemeName = "TelerikMetroTouch";
            // 
            // frmLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(694, 283);
            this.Controls.Add(this.paneLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLoading";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmLoading";
            this.TopMost = true;
            this.paneLoading.ResumeLayout(false);
            this.paneLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblSeguimientoEvent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel paneLoading;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private System.Windows.Forms.PictureBox pictureBox2;
        public Telerik.WinControls.UI.RadLabel lblSeguimientoEvent;
        public System.Windows.Forms.ProgressBar progressBar1;
    }
}