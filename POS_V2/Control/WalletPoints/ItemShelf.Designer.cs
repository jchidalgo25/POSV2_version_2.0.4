namespace POS.Control.WalletPoints
{
    partial class ItemShelf
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
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.btnConsultar = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.gridPagos = new Telerik.WinControls.UI.RadGridView();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtProducto = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel8 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel9 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.btnConsultar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConsultar
            // 
            this.btnConsultar.Location = new System.Drawing.Point(377, 51);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(91, 32);
            this.btnConsultar.TabIndex = 3;
            this.btnConsultar.Text = "Ver Puntos";
            this.btnConsultar.TextWrap = true;
            this.btnConsultar.ThemeName = "TelerikMetroTouch";
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(12, 3);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(227, 30);
            this.radLabel1.TabIndex = 5;
            this.radLabel1.Text = "John Doe - 0926596578";
            this.radLabel1.ThemeName = "TelerikMetroTouch";
            // 
            // gridPagos
            // 
            this.gridPagos.EnableKineticScrolling = true;
            this.gridPagos.Location = new System.Drawing.Point(12, 89);
            // 
            // gridPagos
            // 
            this.gridPagos.MasterTemplate.AllowAddNewRow = false;
            this.gridPagos.MasterTemplate.AllowColumnReorder = false;
            this.gridPagos.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewCheckBoxColumn1.FieldName = "EsSeleccionado";
            gridViewCheckBoxColumn1.HeaderText = "Seleccionar";
            gridViewCheckBoxColumn1.Name = "EsSeleccionado";
            gridViewCheckBoxColumn1.Width = 99;
            gridViewTextBoxColumn1.FieldName = "Item";
            gridViewTextBoxColumn1.HeaderText = "Item";
            gridViewTextBoxColumn1.Name = "Item";
            gridViewTextBoxColumn1.Width = 336;
            gridViewTextBoxColumn2.FieldName = "Valor";
            gridViewTextBoxColumn2.FormatString = "$ {0:00.00}";
            gridViewTextBoxColumn2.HeaderText = "Valor";
            gridViewTextBoxColumn2.Name = "Valor";
            gridViewTextBoxColumn2.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            gridViewTextBoxColumn2.Width = 193;
            gridViewTextBoxColumn3.FieldName = "Puntos";
            gridViewTextBoxColumn3.FormatString = "{0:00.00}";
            gridViewTextBoxColumn3.HeaderText = "Puntos";
            gridViewTextBoxColumn3.Name = "Puntos";
            gridViewTextBoxColumn3.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            gridViewTextBoxColumn3.Width = 134;
            this.gridPagos.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewCheckBoxColumn1,
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3});
            this.gridPagos.MasterTemplate.EnableGrouping = false;
            this.gridPagos.Name = "gridPagos";
            this.gridPagos.Size = new System.Drawing.Size(781, 323);
            this.gridPagos.TabIndex = 12;
            this.gridPagos.Text = "radGridView1";
            this.gridPagos.ThemeName = "TelerikMetroTouch";
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(660, 3);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(133, 42);
            this.radButton1.TabIndex = 4;
            this.radButton1.Text = "Canjear";
            this.radButton1.TextWrap = true;
            this.radButton1.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.Location = new System.Drawing.Point(12, 53);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(75, 30);
            this.radLabel3.TabIndex = 7;
            this.radLabel3.Text = "Buscar:";
            this.radLabel3.ThemeName = "TelerikMetroTouch";
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Wingdings 3", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSearch.Location = new System.Drawing.Point(323, 51);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(39, 34);
            this.btnSearch.TabIndex = 19;
            this.btnSearch.Text = "8";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtProducto
            // 
            this.txtProducto.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtProducto.Location = new System.Drawing.Point(93, 53);
            this.txtProducto.Name = "txtProducto";
            this.txtProducto.NullText = "Buscar el producto...";
            // 
            // 
            // 
            this.txtProducto.RootElement.ControlBounds = new System.Drawing.Rectangle(93, 53, 100, 20);
            this.txtProducto.RootElement.StretchVertically = true;
            this.txtProducto.Size = new System.Drawing.Size(232, 30);
            this.txtProducto.TabIndex = 18;
            this.txtProducto.TabStop = false;
            this.txtProducto.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel4
            // 
            this.radLabel4.AutoSize = false;
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.radLabel4.Location = new System.Drawing.Point(611, 53);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(182, 30);
            this.radLabel4.TabIndex = 7;
            this.radLabel4.Text = "00.00 ptos";
            this.radLabel4.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.radLabel4.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel7
            // 
            this.radLabel7.AutoSize = false;
            this.radLabel7.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel7.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.radLabel7.Location = new System.Drawing.Point(660, 421);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(133, 30);
            this.radLabel7.TabIndex = 9;
            this.radLabel7.Text = "00.00 ptos";
            this.radLabel7.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.radLabel7.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel8
            // 
            this.radLabel8.AutoSize = false;
            this.radLabel8.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel8.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.radLabel8.Location = new System.Drawing.Point(466, 454);
            this.radLabel8.Name = "radLabel8";
            this.radLabel8.Size = new System.Drawing.Size(139, 30);
            this.radLabel8.TabIndex = 8;
            this.radLabel8.Text = "Saldo final:";
            this.radLabel8.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.radLabel8.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel9
            // 
            this.radLabel9.AutoSize = false;
            this.radLabel9.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel9.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.radLabel9.Location = new System.Drawing.Point(660, 454);
            this.radLabel9.Name = "radLabel9";
            this.radLabel9.Size = new System.Drawing.Size(133, 30);
            this.radLabel9.TabIndex = 9;
            this.radLabel9.Text = "00.00 ptos";
            this.radLabel9.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.radLabel9.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel6
            // 
            this.radLabel6.AutoSize = false;
            this.radLabel6.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel6.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.radLabel6.Location = new System.Drawing.Point(466, 421);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(139, 30);
            this.radLabel6.TabIndex = 9;
            this.radLabel6.Text = "Por canjear:";
            this.radLabel6.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.radLabel6.ThemeName = "TelerikMetroTouch";
            // 
            // radLabel5
            // 
            this.radLabel5.AutoSize = false;
            this.radLabel5.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel5.ForeColor = System.Drawing.Color.MediumAquamarine;
            this.radLabel5.Location = new System.Drawing.Point(430, 53);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(175, 30);
            this.radLabel5.TabIndex = 9;
            this.radLabel5.Text = "Saldo:";
            this.radLabel5.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.radLabel5.ThemeName = "TelerikMetroTouch";
            // 
            // ItemShelf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 499);
            this.Controls.Add(this.btnConsultar);
            this.Controls.Add(this.radLabel5);
            this.Controls.Add(this.radLabel6);
            this.Controls.Add(this.radLabel8);
            this.Controls.Add(this.radLabel9);
            this.Controls.Add(this.radLabel7);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtProducto);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radButton1);
            this.Controls.Add(this.gridPagos);
            this.Controls.Add(this.radLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ItemShelf";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Punto de Canje";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PagosServicios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnConsultar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridPagos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnConsultar;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadGridView gridPagos;
        private Telerik.WinControls.UI.RadButton radButton1;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private System.Windows.Forms.Button btnSearch;
        private Telerik.WinControls.UI.RadTextBox txtProducto;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private Telerik.WinControls.UI.RadLabel radLabel8;
        private Telerik.WinControls.UI.RadLabel radLabel9;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadLabel radLabel5;
    }
}
