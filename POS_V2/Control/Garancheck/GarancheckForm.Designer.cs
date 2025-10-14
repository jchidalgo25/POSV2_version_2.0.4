namespace POS.Control.Garancheck
{
    partial class GarancheckForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnKbd = new Telerik.WinControls.UI.RadButton();
            this.geckoWebBrowser1 = new Skybound.Gecko.GeckoWebBrowser();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnKbd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 532);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1034, 60);
            this.panel1.TabIndex = 22;
            // 
            // btnKbd
            // 
            this.btnKbd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnKbd.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnKbd.Location = new System.Drawing.Point(12, 18);
            this.btnKbd.MaximumSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Name = "btnKbd";
            // 
            // 
            // 
            this.btnKbd.RootElement.ControlBounds = new System.Drawing.Rectangle(12, 18, 110, 24);
            this.btnKbd.RootElement.MaxSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Size = new System.Drawing.Size(77, 32);
            this.btnKbd.TabIndex = 18;
            this.btnKbd.Text = "Teclado";
            this.btnKbd.ThemeName = "TelerikMetroTouch";
            this.btnKbd.Visible = false;
            this.btnKbd.Click += new System.EventHandler(this.btnKbd_Click);
            // 
            // geckoWebBrowser1
            // 
            this.geckoWebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.geckoWebBrowser1.Location = new System.Drawing.Point(0, 0);
            this.geckoWebBrowser1.Name = "geckoWebBrowser1";
            this.geckoWebBrowser1.Size = new System.Drawing.Size(1034, 532);
            this.geckoWebBrowser1.TabIndex = 23;
            this.geckoWebBrowser1.Click += new System.EventHandler(this.geckoWebBrowser1_Click);
            // 
            // GarancheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 592);
            this.Controls.Add(this.geckoWebBrowser1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GarancheckForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " ";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GarancheckForm_FormClosing);
            this.Load += new System.EventHandler(this.GarancheckForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadButton btnKbd;
        private Skybound.Gecko.GeckoWebBrowser geckoWebBrowser1;
    }
}