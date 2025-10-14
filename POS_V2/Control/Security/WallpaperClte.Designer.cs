namespace POS.Control.Security
{
    partial class WallpaperClte
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
            this.components = new System.ComponentModel.Container();
            this.pbWallpaperClte = new System.Windows.Forms.PictureBox();
            this.imageTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbWallpaperClte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // pbWallpaperClte
            // 
            this.pbWallpaperClte.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbWallpaperClte.Image = global::POS.Properties.Resources.Pantalla_Cliente_Pollo_horno;
            this.pbWallpaperClte.InitialImage = global::POS.Properties.Resources.Pantalla_Cajas_DELPORTAL_01;
            this.pbWallpaperClte.Location = new System.Drawing.Point(0, 0);
            this.pbWallpaperClte.Name = "pbWallpaperClte";
            this.pbWallpaperClte.Size = new System.Drawing.Size(800, 450);
            this.pbWallpaperClte.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbWallpaperClte.TabIndex = 17;
            this.pbWallpaperClte.TabStop = false;
            // 
            // WallpaperClte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pbWallpaperClte);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WallpaperClte";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "WallpaperClte";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WallpaperClte_FormClosing);
            this.Load += new System.EventHandler(this.WallpaperClte_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WallpaperClte_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbWallpaperClte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbWallpaperClte;
        private System.Windows.Forms.Timer imageTimer;
    }
}