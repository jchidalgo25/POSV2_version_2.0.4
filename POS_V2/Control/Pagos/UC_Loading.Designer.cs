namespace POS.Control.Pagos
{
    partial class UC_Loading
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.paneLoading = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblSeguimientoEvent = new Telerik.WinControls.UI.RadLabel();
            this.lblTextoEspera = new Telerik.WinControls.UI.RadLabel();
            this.lblEtiquetaLoading = new Telerik.WinControls.UI.RadLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.paneLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblSeguimientoEvent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTextoEspera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblEtiquetaLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // paneLoading
            // 
            this.paneLoading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paneLoading.Controls.Add(this.progressBar1);
            this.paneLoading.Controls.Add(this.lblSeguimientoEvent);
            this.paneLoading.Controls.Add(this.lblTextoEspera);
            this.paneLoading.Controls.Add(this.lblEtiquetaLoading);
            this.paneLoading.Controls.Add(this.pictureBox2);
            this.paneLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneLoading.Location = new System.Drawing.Point(0, 0);
            this.paneLoading.Name = "paneLoading";
            this.paneLoading.Size = new System.Drawing.Size(595, 185);
            this.paneLoading.TabIndex = 38;
            this.paneLoading.Paint += new System.Windows.Forms.PaintEventHandler(this.paneLoading_Paint);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(472, 144);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 23);
            this.progressBar1.TabIndex = 41;
            this.progressBar1.Visible = false;
            // 
            // lblSeguimientoEvent
            // 
            this.lblSeguimientoEvent.AutoSize = false;
            this.lblSeguimientoEvent.BorderVisible = true;
            this.lblSeguimientoEvent.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeguimientoEvent.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.lblSeguimientoEvent.Location = new System.Drawing.Point(240, 117);
            this.lblSeguimientoEvent.Name = "lblSeguimientoEvent";
            this.lblSeguimientoEvent.Size = new System.Drawing.Size(306, 21);
            this.lblSeguimientoEvent.TabIndex = 40;
            this.lblSeguimientoEvent.Text = "Generando Pago";
            this.lblSeguimientoEvent.ThemeName = "TelerikMetroTouch";
            // 
            // lblTextoEspera
            // 
            this.lblTextoEspera.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTextoEspera.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.lblTextoEspera.Location = new System.Drawing.Point(240, 90);
            this.lblTextoEspera.Name = "lblTextoEspera";
            this.lblTextoEspera.Size = new System.Drawing.Size(306, 21);
            this.lblTextoEspera.TabIndex = 39;
            this.lblTextoEspera.Text = "un momento por favor, este proceso puede demorar";
            this.lblTextoEspera.ThemeName = "TelerikMetroTouch";
            // 
            // lblEtiquetaLoading
            // 
            this.lblEtiquetaLoading.Font = new System.Drawing.Font("Segoe UI", 28.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtiquetaLoading.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.lblEtiquetaLoading.Location = new System.Drawing.Point(200, 27);
            this.lblEtiquetaLoading.Name = "lblEtiquetaLoading";
            this.lblEtiquetaLoading.Size = new System.Drawing.Size(233, 57);
            this.lblEtiquetaLoading.TabIndex = 7;
            this.lblEtiquetaLoading.Text = "Procesando...";
            this.lblEtiquetaLoading.ThemeName = "TelerikMetroTouch";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::POS.Properties.Resources.loading2;
            this.pictureBox2.Location = new System.Drawing.Point(12, 11);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(182, 139);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // UC_Loading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.paneLoading);
            this.Name = "UC_Loading";
            this.Size = new System.Drawing.Size(595, 185);
            this.paneLoading.ResumeLayout(false);
            this.paneLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblSeguimientoEvent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTextoEspera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblEtiquetaLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel paneLoading;
        private Telerik.WinControls.UI.RadLabel lblEtiquetaLoading;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Telerik.WinControls.UI.RadLabel lblTextoEspera;
        public Telerik.WinControls.UI.RadLabel lblSeguimientoEvent;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
