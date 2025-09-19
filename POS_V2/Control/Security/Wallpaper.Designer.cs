namespace POS.Control.Security
{
    partial class Wallpaper
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
            this.pbWallpaper = new System.Windows.Forms.PictureBox();
            this.imageTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbWallpaper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // pbWallpaper
            // 
            this.pbWallpaper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbWallpaper.Image = global::POS.Properties.Resources.Pantalla_Cajas_DELPORTAL_01;
            this.pbWallpaper.InitialImage = global::POS.Properties.Resources.Pantalla_Cajas_DELPORTAL_01;
            this.pbWallpaper.Location = new System.Drawing.Point(0, 0);
            this.pbWallpaper.Name = "pbWallpaper";
            this.pbWallpaper.Size = new System.Drawing.Size(1024, 768);
            this.pbWallpaper.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbWallpaper.TabIndex = 16;
            this.pbWallpaper.TabStop = false;
            this.pbWallpaper.Click += new System.EventHandler(this.pbWallpaper_Click);
            // 
            // Wallpaper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.ControlBox = false;
            this.Controls.Add(this.pbWallpaper);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Wallpaper";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Supermercado Delportal";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Wallpaper_FormClosing);
            this.Load += new System.EventHandler(this.Wallpaper_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Wallpaper_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbWallpaper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pbWallpaper;
        private System.Windows.Forms.Timer imageTimer;
    }
}
