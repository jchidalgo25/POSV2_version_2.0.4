namespace POS.Control.ReImprimirFactura
{
    partial class ReImprimirFactura
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
            this.txtResult = new Telerik.WinControls.UI.RadTextBoxControl();
            this.btnPagar = new Telerik.WinControls.UI.RadButton();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.cmbTipoTransaccion = new Telerik.WinControls.UI.RadDropDownList();
            this.boxReimpresion = new System.Windows.Forms.GroupBox();
            this.txtFacReimpresion = new Telerik.WinControls.UI.RadTextBox();
            this.lblFacReimpresion = new Telerik.WinControls.UI.RadLabel();
            this.lblFacReimpresionAyuda = new Telerik.WinControls.UI.RadLabel();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.btnKbd = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPagar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTipoTransaccion)).BeginInit();
            this.boxReimpresion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFacReimpresion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacReimpresion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacReimpresionAyuda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).BeginInit();
            this.SuspendLayout();
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(12, 236);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(731, 69);
            this.txtResult.TabIndex = 0;
            this.txtResult.ThemeName = "TelerikMetroTouch";
            // 
            // btnPagar
            // 
            this.btnPagar.Location = new System.Drawing.Point(768, 150);
            this.btnPagar.Name = "btnPagar";
            this.btnPagar.Size = new System.Drawing.Size(110, 32);
            this.btnPagar.TabIndex = 3;
            this.btnPagar.Text = "Reimprimir";
            this.btnPagar.ThemeName = "TelerikMetroTouch";
            this.btnPagar.Click += new System.EventHandler(this.btnPagar_Click);
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(36, 59);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(123, 30);
            this.radLabel2.TabIndex = 12;
            this.radLabel2.Text = "Transaccion:";
            this.radLabel2.ThemeName = "TelerikMetroTouch";
            // 
            // cmbTipoTransaccion
            // 
            this.cmbTipoTransaccion.AutoSize = false;
            this.cmbTipoTransaccion.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbTipoTransaccion.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTipoTransaccion.Location = new System.Drawing.Point(165, 59);
            this.cmbTipoTransaccion.Name = "cmbTipoTransaccion";
            this.cmbTipoTransaccion.Size = new System.Drawing.Size(257, 43);
            this.cmbTipoTransaccion.TabIndex = 11;
            this.cmbTipoTransaccion.ThemeName = "TelerikMetroTouch";
            this.cmbTipoTransaccion.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.cmbTipoTransaccion_SelectedIndexChanged);
            // 
            // boxReimpresion
            // 
            this.boxReimpresion.BackColor = System.Drawing.Color.Transparent;
            this.boxReimpresion.Controls.Add(this.txtFacReimpresion);
            this.boxReimpresion.Controls.Add(this.lblFacReimpresion);
            this.boxReimpresion.Controls.Add(this.lblFacReimpresionAyuda);
            this.boxReimpresion.Controls.Add(this.radLabel7);
            this.boxReimpresion.Location = new System.Drawing.Point(460, 45);
            this.boxReimpresion.Name = "boxReimpresion";
            this.boxReimpresion.Size = new System.Drawing.Size(437, 80);
            this.boxReimpresion.TabIndex = 25;
            this.boxReimpresion.TabStop = false;
            this.boxReimpresion.Text = "Reimpresión de facturas";
            // 
            // txtFacReimpresion
            // 
            this.txtFacReimpresion.BackColor = System.Drawing.Color.AntiqueWhite;
            this.txtFacReimpresion.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFacReimpresion.Location = new System.Drawing.Point(218, 12);
            this.txtFacReimpresion.MaxLength = 9;
            this.txtFacReimpresion.Name = "txtFacReimpresion";
            this.txtFacReimpresion.Size = new System.Drawing.Size(193, 46);
            this.txtFacReimpresion.TabIndex = 17;
            this.txtFacReimpresion.TabStop = false;
            this.txtFacReimpresion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtFacReimpresion.ThemeName = "TelerikMetroTouch";
            this.txtFacReimpresion.TextChanged += new System.EventHandler(this.txtFacReimpresion_TextChanged);
            // 
            // lblFacReimpresion
            // 
            this.lblFacReimpresion.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacReimpresion.ForeColor = System.Drawing.Color.DarkGray;
            this.lblFacReimpresion.Location = new System.Drawing.Point(298, 56);
            this.lblFacReimpresion.Name = "lblFacReimpresion";
            this.lblFacReimpresion.Size = new System.Drawing.Size(120, 18);
            this.lblFacReimpresion.TabIndex = 20;
            this.lblFacReimpresion.Text = "F-029-002-000000000";
            this.lblFacReimpresion.ThemeName = "TelerikMetroTouch";
            // 
            // lblFacReimpresionAyuda
            // 
            this.lblFacReimpresionAyuda.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacReimpresionAyuda.ForeColor = System.Drawing.Color.Gray;
            this.lblFacReimpresionAyuda.Location = new System.Drawing.Point(113, 12);
            this.lblFacReimpresionAyuda.Name = "lblFacReimpresionAyuda";
            this.lblFacReimpresionAyuda.Size = new System.Drawing.Size(110, 30);
            this.lblFacReimpresionAyuda.TabIndex = 19;
            this.lblFacReimpresionAyuda.Text = "F-029-002-";
            this.lblFacReimpresionAyuda.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel7
            // 
            this.radLabel7.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel7.Location = new System.Drawing.Point(6, 12);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(82, 30);
            this.radLabel7.TabIndex = 18;
            this.radLabel7.Text = "Factura:";
            this.radLabel7.ThemeName = "TelerikMetroTouch";
            // 
            // btnKbd
            // 
            this.btnKbd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnKbd.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnKbd.Location = new System.Drawing.Point(801, 260);
            this.btnKbd.MaximumSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Name = "btnKbd";
            // 
            // 
            // 
            this.btnKbd.RootElement.ControlBounds = new System.Drawing.Rectangle(801, 260, 110, 24);
            this.btnKbd.RootElement.MaxSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Size = new System.Drawing.Size(77, 45);
            this.btnKbd.TabIndex = 28;
            this.btnKbd.Text = "Teclado";
            this.btnKbd.ThemeName = "TelerikMetroTouch";
            this.btnKbd.Click += new System.EventHandler(this.btnKbd_Click);
            // 
            // ReImprimirFactura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 340);
            this.Controls.Add(this.btnKbd);
            this.Controls.Add(this.boxReimpresion);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.cmbTipoTransaccion);
            this.Controls.Add(this.btnPagar);
            this.Controls.Add(this.txtResult);
            this.Name = "ReImprimirFactura";
            this.Text = "Reimpresión de Facturas";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ReImprimirFactura_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPagar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTipoTransaccion)).EndInit();
            this.boxReimpresion.ResumeLayout(false);
            this.boxReimpresion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFacReimpresion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacReimpresion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFacReimpresionAyuda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBoxControl txtResult;
        private Telerik.WinControls.UI.RadButton btnPagar;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadDropDownList cmbTipoTransaccion;
        private System.Windows.Forms.GroupBox boxReimpresion;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private Telerik.WinControls.UI.RadTextBox txtFacReimpresion;
        private Telerik.WinControls.UI.RadLabel lblFacReimpresion;
        private Telerik.WinControls.UI.RadLabel lblFacReimpresionAyuda;
        private Telerik.WinControls.UI.RadButton btnKbd;
    }
}