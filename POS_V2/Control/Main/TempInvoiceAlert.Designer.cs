namespace POS.Control.Main
{
    partial class TempInvoiceAlert
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
            this.btnAceptar = new Telerik.WinControls.UI.RadButton();
            this.chkAccept = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnAceptar)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAceptar
            // 
            this.btnAceptar.Enabled = false;
            this.btnAceptar.Location = new System.Drawing.Point(194, 129);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(169, 40);
            this.btnAceptar.TabIndex = 15;
            this.btnAceptar.Text = "Acepto";
            this.btnAceptar.TextWrap = true;
            this.btnAceptar.ThemeName = "TelerikMetroTouch";
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // chkAccept
            // 
            this.chkAccept.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAccept.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAccept.Location = new System.Drawing.Point(18, 35);
            this.chkAccept.Name = "chkAccept";
            this.chkAccept.Size = new System.Drawing.Size(517, 74);
            this.chkAccept.TabIndex = 16;
            this.chkAccept.Text = "Se han cargado items temporales a la factura. He leído este mensaje y entiendo qu" +
    "e es mi responsabilidad verificar estos items";
            this.chkAccept.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkAccept.UseVisualStyleBackColor = true;
            this.chkAccept.CheckedChanged += new System.EventHandler(this.chkAccept_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnAceptar);
            this.panel1.Controls.Add(this.chkAccept);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(548, 185);
            this.panel1.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Factura temporal";
            // 
            // TempInvoiceAlert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 185);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TempInvoiceAlert";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Factura temporal";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.btnAceptar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public Telerik.WinControls.UI.RadButton btnAceptar;
        private System.Windows.Forms.CheckBox chkAccept;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}
