namespace POS.Control.WalletPoints
{
    partial class WalletDetails
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
            this.gridPagos = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos.MasterTemplate)).BeginInit();
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
            gridViewTextBoxColumn1.FieldName = "Campaign";
            gridViewTextBoxColumn1.HeaderText = "Campaña";
            gridViewTextBoxColumn1.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            gridViewTextBoxColumn1.Name = "Campaign";
            gridViewTextBoxColumn1.Width = 290;
            gridViewTextBoxColumn2.FieldName = "Total";
            gridViewTextBoxColumn2.FormatString = "{0:00.00}";
            gridViewTextBoxColumn2.HeaderText = "Total";
            gridViewTextBoxColumn2.Name = "Total";
            gridViewTextBoxColumn2.Width = 44;
            this.gridPagos.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2});
            this.gridPagos.MasterTemplate.EnableGrouping = false;
            this.gridPagos.Name = "gridPagos";
            this.gridPagos.ReadOnly = true;
            this.gridPagos.Size = new System.Drawing.Size(355, 500);
            this.gridPagos.TabIndex = 12;
            this.gridPagos.Text = "radGridView1";
            this.gridPagos.ThemeName = "TelerikMetroTouch";
            // 
            // WalletDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 526);
            this.Controls.Add(this.gridPagos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "WalletDetails";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Detalle de puntos";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PagosServicios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Telerik.WinControls.UI.RadGridView gridPagos;
    }
}
