namespace POS.Control
{
    partial class CanjePavos
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
            Telerik.WinControls.Keyboard.InputBinding inputBinding1 = new Telerik.WinControls.Keyboard.InputBinding();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.txtCodCanje = new Telerik.WinControls.UI.RadTextBox();
            this.btnValidar = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.lblCliente = new Telerik.WinControls.UI.RadLabel();
            this.lblItem = new Telerik.WinControls.UI.RadLabel();
            this.lblPeso = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.txtProducto = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodCanje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCliente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPeso)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(12, 12);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(244, 30);
            this.radLabel2.TabIndex = 23;
            this.radLabel2.Text = "Escanear Código de Canje";
            this.radLabel2.ThemeName = "TelerikMetroTouch";
            // 
            // txtCodCanje
            // 
            this.txtCodCanje.BackColor = System.Drawing.Color.Gainsboro;
            this.txtCodCanje.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodCanje.Location = new System.Drawing.Point(12, 48);
            this.txtCodCanje.Name = "txtCodCanje";
            this.txtCodCanje.NullText = "Pistolear código...";
            this.txtCodCanje.Size = new System.Drawing.Size(468, 46);
            this.txtCodCanje.TabIndex = 24;
            this.txtCodCanje.TabStop = false;
            this.txtCodCanje.ThemeName = "TelerikMetroTouch";
            // 
            // btnValidar
            // 
            this.btnValidar.Location = new System.Drawing.Point(560, 147);
            this.btnValidar.Name = "btnValidar";
            this.btnValidar.Size = new System.Drawing.Size(103, 46);
            this.btnValidar.TabIndex = 25;
            this.btnValidar.Text = "Validar";
            this.btnValidar.TextWrap = true;
            this.btnValidar.ThemeName = "TelerikMetroTouch";
            this.btnValidar.Click += new System.EventHandler(this.btnValidar_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(12, 218);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(83, 30);
            this.radLabel1.TabIndex = 26;
            this.radLabel1.Text = "Cliente: ";
            this.radLabel1.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.Location = new System.Drawing.Point(12, 257);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(104, 30);
            this.radLabel3.TabIndex = 27;
            this.radLabel3.Text = "Producto: ";
            this.radLabel3.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(12, 295);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(63, 30);
            this.radLabel4.TabIndex = 28;
            this.radLabel4.Text = "Peso: ";
            this.radLabel4.ThemeName = "TelerikMetroTouch";
            // 
            // lblCliente
            // 
            this.lblCliente.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCliente.Location = new System.Drawing.Point(151, 219);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(2, 2);
            this.lblCliente.TabIndex = 29;
            this.lblCliente.ThemeName = "TelerikMetroTouch";
            // 
            // lblItem
            // 
            this.lblItem.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItem.Location = new System.Drawing.Point(150, 257);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(2, 2);
            this.lblItem.TabIndex = 30;
            this.lblItem.ThemeName = "TelerikMetroTouch";
            // 
            // lblPeso
            // 
            this.lblPeso.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPeso.Location = new System.Drawing.Point(151, 296);
            this.lblPeso.Name = "lblPeso";
            this.lblPeso.Size = new System.Drawing.Size(2, 2);
            this.lblPeso.TabIndex = 31;
            this.lblPeso.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel5
            // 
            this.radLabel5.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel5.Location = new System.Drawing.Point(12, 111);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(269, 30);
            this.radLabel5.TabIndex = 24;
            this.radLabel5.Text = "Escanear Producto a Canjear";
            this.radLabel5.ThemeName = "TelerikMetroTouch";
            // 
            // txtProducto
            // 
            this.txtProducto.BackColor = System.Drawing.Color.Gainsboro;
            this.txtProducto.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProducto.Location = new System.Drawing.Point(12, 147);
            this.txtProducto.Name = "txtProducto";
            this.txtProducto.NullText = "Pistolear producto...";
            this.txtProducto.Size = new System.Drawing.Size(468, 46);
            this.txtProducto.TabIndex = 25;
            this.txtProducto.TabStop = false;
            this.txtProducto.ThemeName = "TelerikMetroTouch";
            // 
            // CanjePavos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(747, 396);
            inputBinding1.Chord = null;
            this.CommandBindings.AddRange(new Telerik.WinControls.Keyboard.InputBinding[] {
            inputBinding1});
            this.Controls.Add(this.txtProducto);
            this.Controls.Add(this.radLabel5);
            this.Controls.Add(this.lblPeso);
            this.Controls.Add(this.lblItem);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.btnValidar);
            this.Controls.Add(this.txtCodCanje);
            this.Controls.Add(this.radLabel2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CanjePavos";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Canje de Pavos";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CanjePavos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodCanje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCliente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPeso)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadTextBox txtCodCanje;
        private Telerik.WinControls.UI.RadButton btnValidar;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel lblCliente;
        private Telerik.WinControls.UI.RadLabel lblItem;
        private Telerik.WinControls.UI.RadLabel lblPeso;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadTextBox txtProducto;
    }
}