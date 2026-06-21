namespace POS.Control.ToolBox
{
    partial class MenuInicial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuInicial));
            this.btnVtaApp = new Telerik.WinControls.UI.RadButton();
            this.btnVtaNormal = new Telerik.WinControls.UI.RadButton();
            this.btnGlovo = new Telerik.WinControls.UI.RadButton();
            this.btnVtaRappid = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.btnVtaApp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVtaNormal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGlovo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVtaRappid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVtaApp
            // 
            this.btnVtaApp.Image = ((System.Drawing.Image)(resources.GetObject("btnVtaApp.Image")));
            this.btnVtaApp.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnVtaApp.Location = new System.Drawing.Point(279, 3);
            this.btnVtaApp.Name = "btnVtaApp";
            this.btnVtaApp.Size = new System.Drawing.Size(270, 335);
            this.btnVtaApp.TabIndex = 8;
            this.btnVtaApp.Text = "Venta App DelPortal";
            this.btnVtaApp.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.btnVtaApp.Click += new System.EventHandler(this.btnVtaApp_Click);
            // 
            // btnVtaNormal
            // 
            this.btnVtaNormal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnVtaNormal.Image = ((System.Drawing.Image)(resources.GetObject("btnVtaNormal.Image")));
            this.btnVtaNormal.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnVtaNormal.Location = new System.Drawing.Point(3, 3);
            this.btnVtaNormal.Name = "btnVtaNormal";
            this.btnVtaNormal.Size = new System.Drawing.Size(270, 335);
            this.btnVtaNormal.TabIndex = 7;
            this.btnVtaNormal.Text = "Venta POS";
            this.btnVtaNormal.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.btnVtaNormal.TextWrap = true;
            this.btnVtaNormal.ThemeName = "TelerikMetroTouch";
            this.btnVtaNormal.Click += new System.EventHandler(this.btnVtaNormal_Click);
            // 
            // btnGlovo
            // 
            this.btnGlovo.Image = ((System.Drawing.Image)(resources.GetObject("btnGlovo.Image")));
            this.btnGlovo.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnGlovo.ImageScalingSize = new System.Drawing.Size(8, 8);
            this.btnGlovo.Location = new System.Drawing.Point(831, 3);
            this.btnGlovo.Name = "btnGlovo";
            this.btnGlovo.Size = new System.Drawing.Size(270, 335);
            this.btnGlovo.TabIndex = 8;
            this.btnGlovo.Text = "Pedidos Ya";
            this.btnGlovo.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGlovo.TextWrap = true;
            this.btnGlovo.ThemeName = "TelerikMetroTouch";
            this.btnGlovo.Visible = false;
            this.btnGlovo.Click += new System.EventHandler(this.btnGlovo_Click);
            // 
            // btnVtaRappid
            // 
            this.btnVtaRappid.Image = ((System.Drawing.Image)(resources.GetObject("btnVtaRappid.Image")));
            this.btnVtaRappid.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnVtaRappid.Location = new System.Drawing.Point(555, 3);
            this.btnVtaRappid.Name = "btnVtaRappid";
            this.btnVtaRappid.Size = new System.Drawing.Size(270, 335);
            this.btnVtaRappid.TabIndex = 9;
            this.btnVtaRappid.Text = "Rappi";
            this.btnVtaRappid.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.btnVtaRappid.TextWrap = true;
            this.btnVtaRappid.ThemeName = "TelerikMetroTouch";
            this.btnVtaRappid.Visible = false;
            this.btnVtaRappid.Click += new System.EventHandler(this.btnVtaRappid_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.radLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.ForeColor = System.Drawing.Color.White;
            this.radLabel1.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radLabel1.Location = new System.Drawing.Point(426, 3);
            this.radLabel1.Name = "radLabel1";
            // 
            // 
            // 
            this.radLabel1.RootElement.ControlBounds = new System.Drawing.Rectangle(426, 3, 100, 18);
            this.radLabel1.Size = new System.Drawing.Size(487, 53);
            this.radLabel1.TabIndex = 11;
            this.radLabel1.Text = "Seleccione el Tipo de Venta:";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.flowLayoutPanel2.Controls.Add(this.btnVtaNormal);
            this.flowLayoutPanel2.Controls.Add(this.btnVtaApp);
            this.flowLayoutPanel2.Controls.Add(this.btnVtaRappid);
            this.flowLayoutPanel2.Controls.Add(this.btnGlovo);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(30, 56);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1278, 686);
            this.flowLayoutPanel2.TabIndex = 13;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radLabel1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 692F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1339, 755);
            this.tableLayoutPanel1.TabIndex = 12;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // MenuInicial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(1339, 720);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MenuInicial";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ToolBox";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ToolBoxMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnVtaApp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVtaNormal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGlovo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVtaRappid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Telerik.WinControls.UI.RadButton btnVtaApp;
        private Telerik.WinControls.UI.RadButton btnVtaNormal;
        private Telerik.WinControls.UI.RadButton btnGlovo;
        private Telerik.WinControls.UI.RadButton btnVtaRappid;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
