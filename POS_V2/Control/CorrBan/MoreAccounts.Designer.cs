namespace POS.Control.CorrBan
{
    partial class MoreAccounts
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.gridCuentas = new Telerik.WinControls.UI.RadGridView();
            this.btnSalir = new Telerik.WinControls.UI.RadButton();
            this.btnElegir = new Telerik.WinControls.UI.RadButton();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.gridCuentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCuentas.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnElegir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // gridCuentas
            // 
            this.gridCuentas.EnableKineticScrolling = true;
            this.gridCuentas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridCuentas.Location = new System.Drawing.Point(12, 54);
            // 
            // gridCuentas
            // 
            this.gridCuentas.MasterTemplate.AllowAddNewRow = false;
            this.gridCuentas.MasterTemplate.AllowColumnReorder = false;
            this.gridCuentas.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewTextBoxColumn1.FieldName = "Account";
            gridViewTextBoxColumn1.HeaderText = "Cuenta";
            gridViewTextBoxColumn1.Name = "Account";
            gridViewTextBoxColumn1.Width = 146;
            gridViewTextBoxColumn2.FieldName = "Names";
            gridViewTextBoxColumn2.HeaderText = "Nombres";
            gridViewTextBoxColumn2.Name = "Names";
            gridViewTextBoxColumn2.Width = 255;
            gridViewTextBoxColumn3.FieldName = "Document";
            gridViewTextBoxColumn3.HeaderText = "Identificación";
            gridViewTextBoxColumn3.Name = "Document";
            gridViewTextBoxColumn3.Width = 130;
            gridViewTextBoxColumn4.FieldName = "Company";
            gridViewTextBoxColumn4.HeaderText = "Institución";
            gridViewTextBoxColumn4.Name = "Company";
            gridViewTextBoxColumn4.Width = 116;
            this.gridCuentas.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4});
            this.gridCuentas.MasterTemplate.EnableGrouping = false;
            this.gridCuentas.Name = "gridCuentas";
            this.gridCuentas.ReadOnly = true;
            this.gridCuentas.Size = new System.Drawing.Size(666, 271);
            this.gridCuentas.TabIndex = 13;
            this.gridCuentas.Text = "radGridView1";
            this.gridCuentas.ThemeName = "TelerikMetroTouch";
            this.gridCuentas.CellDoubleClick += new Telerik.WinControls.UI.GridViewCellEventHandler(this.gridCuentas_CellDoubleClick);
            // 
            // btnSalir
            // 
            this.btnSalir.Location = new System.Drawing.Point(164, 332);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(169, 46);
            this.btnSalir.TabIndex = 14;
            this.btnSalir.Text = "Salir";
            this.btnSalir.TextWrap = true;
            this.btnSalir.ThemeName = "TelerikMetroTouch";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnElegir
            // 
            this.btnElegir.Location = new System.Drawing.Point(359, 332);
            this.btnElegir.Name = "btnElegir";
            // 
            // 
            // 
            this.btnElegir.RootElement.ApplyShapeToControl = true;
            this.btnElegir.Size = new System.Drawing.Size(169, 46);
            this.btnElegir.TabIndex = 15;
            this.btnElegir.Text = "Elegir";
            this.btnElegir.TextWrap = true;
            this.btnElegir.ThemeName = "TelerikMetroTouch";
            this.btnElegir.Click += new System.EventHandler(this.btnElegir_Click);
            // 
            // radLabel4
            // 
            this.radLabel4.AutoSize = false;
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(12, 10);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(666, 38);
            this.radLabel4.TabIndex = 16;
            this.radLabel4.Text = "Su consulta devolvió más de una coincidencia, a continuación selecione la cuenta " +
    "deseada y pulse Elegir";
            this.radLabel4.ThemeName = "TelerikMetroTouch";
            // 
            // MoreAccounts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 382);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.btnElegir);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.gridCuentas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MoreAccounts";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cuentas coincidentes";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MoreAccounts_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridCuentas.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCuentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSalir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnElegir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView gridCuentas;
        private Telerik.WinControls.UI.RadButton btnSalir;
        private Telerik.WinControls.UI.RadButton btnElegir;
        private Telerik.WinControls.UI.RadLabel radLabel4;
    }
}
