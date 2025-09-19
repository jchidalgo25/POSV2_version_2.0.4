namespace POS.Control.Peso
{
    partial class TomaPesoUI
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
            this.lblPeso = new System.Windows.Forms.Label();
            this.grpProducto = new Telerik.WinControls.UI.RadGroupBox();
            this.lblNombreProducto = new System.Windows.Forms.Label();
            this.btnOk = new Telerik.WinControls.UI.RadButton();
            this.tmrDL = new System.Windows.Forms.Timer(this.components);
            this.txtPeso = new System.Windows.Forms.TextBox();
            this.lblpesokl = new System.Windows.Forms.Label();
            this.btnMostarKL = new Telerik.WinControls.UI.RadButton();
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grpProducto)).BeginInit();
            this.grpProducto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMostarKL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPeso
            // 
            this.lblPeso.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPeso.AutoSize = true;
            this.lblPeso.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPeso.Location = new System.Drawing.Point(14, 33);
            this.lblPeso.Name = "lblPeso";
            this.lblPeso.Size = new System.Drawing.Size(392, 108);
            this.lblPeso.TabIndex = 0;
            this.lblPeso.Text = "0.000 lb";
            this.lblPeso.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPeso.TextChanged += new System.EventHandler(this.lblPeso_TextChanged);
            // 
            // grpProducto
            // 
            this.grpProducto.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.grpProducto.BackColor = System.Drawing.Color.Transparent;
            this.grpProducto.Controls.Add(this.lblNombreProducto);
            this.grpProducto.Controls.Add(this.lblPeso);
            this.grpProducto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpProducto.HeaderText = "Nombre Producto";
            this.grpProducto.Location = new System.Drawing.Point(12, 20);
            this.grpProducto.Name = "grpProducto";
            // 
            // 
            // 
            this.grpProducto.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.grpProducto.Size = new System.Drawing.Size(438, 159);
            this.grpProducto.TabIndex = 4;
            this.grpProducto.TabStop = false;
            this.grpProducto.Text = "Nombre Producto";
            this.grpProducto.ThemeName = "TelerikMetroTouch";
            // 
            // lblNombreProducto
            // 
            this.lblNombreProducto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreProducto.Location = new System.Drawing.Point(6, 23);
            this.lblNombreProducto.Name = "lblNombreProducto";
            this.lblNombreProducto.Size = new System.Drawing.Size(426, 23);
            this.lblNombreProducto.TabIndex = 1;
            this.lblNombreProducto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(399, 185);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(126, 55);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Ok";
            this.btnOk.ThemeName = "TelerikMetroTouch";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click_1);
            // 
            // tmrDL
            // 
            this.tmrDL.Interval = 1000;
            this.tmrDL.Tick += new System.EventHandler(this.tmrDL_Tick);
            // 
            // txtPeso
            // 
            this.txtPeso.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPeso.Location = new System.Drawing.Point(12, 199);
            this.txtPeso.Name = "txtPeso";
            this.txtPeso.Size = new System.Drawing.Size(100, 29);
            this.txtPeso.TabIndex = 6;
            this.txtPeso.Visible = false;
            this.txtPeso.TextChanged += new System.EventHandler(this.txtPeso_TextChanged);
            // 
            // lblpesokl
            // 
            this.lblpesokl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblpesokl.AutoSize = true;
            this.lblpesokl.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblpesokl.Location = new System.Drawing.Point(14, 33);
            this.lblpesokl.Name = "lblpesokl";
            this.lblpesokl.Size = new System.Drawing.Size(419, 108);
            this.lblpesokl.TabIndex = 7;
            this.lblpesokl.Text = "0.000 kg";
            this.lblpesokl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMostarKL
            // 
            this.btnMostarKL.Location = new System.Drawing.Point(560, 208);
            this.btnMostarKL.Name = "btnMostarKL";
            this.btnMostarKL.Size = new System.Drawing.Size(119, 32);
            this.btnMostarKL.TabIndex = 6;
            this.btnMostarKL.Text = "Mostrar KL";
            this.btnMostarKL.ThemeName = "TelerikMetroTouch";
            this.btnMostarKL.Visible = false;
            this.btnMostarKL.Click += new System.EventHandler(this.btnMostarKL_Click);
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.radGroupBox1.Controls.Add(this.lblpesokl);
            this.radGroupBox1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGroupBox1.HeaderText = "";
            this.radGroupBox1.Location = new System.Drawing.Point(471, 20);
            this.radGroupBox1.Name = "radGroupBox1";
            // 
            // 
            // 
            this.radGroupBox1.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.radGroupBox1.Size = new System.Drawing.Size(438, 159);
            this.radGroupBox1.TabIndex = 5;
            this.radGroupBox1.TabStop = false;
            this.radGroupBox1.ThemeName = "TelerikMetroTouch";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Peso en libras";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(468, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Peso en kilos";
            // 
            // TomaPesoUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 247);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radGroupBox1);
            this.Controls.Add(this.btnMostarKL);
            this.Controls.Add(this.txtPeso);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.grpProducto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TomaPesoUI";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PesoBalanza";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PesoBalanza_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PesoBalanza_FormClosed);
            this.Load += new System.EventHandler(this.PesoBalanza_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grpProducto)).EndInit();
            this.grpProducto.ResumeLayout(false);
            this.grpProducto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMostarKL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            this.radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPeso;
        private Telerik.WinControls.UI.RadGroupBox grpProducto;
        private System.Windows.Forms.Label lblNombreProducto;
        private Telerik.WinControls.UI.RadButton btnOk;
        private System.Windows.Forms.Timer tmrDL;
        private System.Windows.Forms.TextBox txtPeso;
        private System.Windows.Forms.Label lblpesokl;
        private Telerik.WinControls.UI.RadButton btnMostarKL;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}