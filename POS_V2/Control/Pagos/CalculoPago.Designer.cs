namespace POS.Control.Pagos
{
    partial class CalculoPago
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
            this.gridPagos = new Telerik.WinControls.UI.RadGridView();
            this.btnOk = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // gridPagos
            // 
            this.gridPagos.EnableKineticScrolling = true;
            this.gridPagos.Location = new System.Drawing.Point(12, 12);
            // 
            // gridPagos
            // 
            this.gridPagos.MasterTemplate.AllowAddNewRow = false;
            this.gridPagos.MasterTemplate.AllowColumnReorder = false;
            this.gridPagos.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewTextBoxColumn1.FieldName = "Descripcion";
            gridViewTextBoxColumn1.HeaderText = "Descripción";
            gridViewTextBoxColumn1.Name = "Descripcion";
            gridViewTextBoxColumn1.Width = 214;
            gridViewTextBoxColumn2.FieldName = "Porcentaje";
            gridViewTextBoxColumn2.HeaderText = "%";
            gridViewTextBoxColumn2.Name = "column1";
            gridViewTextBoxColumn2.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            gridViewTextBoxColumn2.Width = 90;
            gridViewTextBoxColumn3.FieldName = "Valor";
            gridViewTextBoxColumn3.FormatString = "{0:00.00}";
            gridViewTextBoxColumn3.HeaderText = "Valor";
            gridViewTextBoxColumn3.Name = "Valor";
            gridViewTextBoxColumn3.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            gridViewTextBoxColumn3.Width = 169;
            this.gridPagos.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3});
            this.gridPagos.MasterTemplate.EnableGrouping = false;
            this.gridPagos.Name = "gridPagos";
            this.gridPagos.ReadOnly = true;
            this.gridPagos.Size = new System.Drawing.Size(493, 446);
            this.gridPagos.TabIndex = 0;
            this.gridPagos.Text = "radGridView1";
            this.gridPagos.ThemeName = "TelerikMetroTouch";
            this.gridPagos.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.gridPagos_CellFormatting);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(171, 464);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(151, 52);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.ThemeName = "TelerikMetroTouch";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // CalculoPago
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 431);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.gridPagos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalculoPago";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "CalculoPago";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CalculoPago_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView gridPagos;
        private Telerik.WinControls.UI.RadButton btnOk;
    }
}
