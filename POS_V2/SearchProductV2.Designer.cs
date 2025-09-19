namespace POS
{
    partial class SearchProductV2
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
            this.dgvSearchProduct = new System.Windows.Forms.DataGridView();
            this.lblProductLabel = new System.Windows.Forms.Label();
            this.btnChooseProduct = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnKb = new System.Windows.Forms.Button();
            this.txtProduct = new Telerik.WinControls.UI.RadTextBox();
            this.imgArticulo = new System.Windows.Forms.DataGridViewImageColumn();
            this.articulo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.barras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProduct)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSearchProduct
            // 
            this.dgvSearchProduct.AllowUserToAddRows = false;
            this.dgvSearchProduct.AllowUserToDeleteRows = false;
            this.dgvSearchProduct.AllowUserToResizeColumns = false;
            this.dgvSearchProduct.AllowUserToResizeRows = false;
            this.dgvSearchProduct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchProduct.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.imgArticulo,
            this.articulo,
            this.barras});
            this.dgvSearchProduct.Location = new System.Drawing.Point(21, 81);
            this.dgvSearchProduct.MultiSelect = false;
            this.dgvSearchProduct.Name = "dgvSearchProduct";
            this.dgvSearchProduct.ReadOnly = true;
            this.dgvSearchProduct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSearchProduct.Size = new System.Drawing.Size(535, 346);
            this.dgvSearchProduct.TabIndex = 2;
            this.dgvSearchProduct.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchProduct_CellContentDoubleClick);
            // 
            // lblProductLabel
            // 
            this.lblProductLabel.AutoSize = true;
            this.lblProductLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductLabel.Location = new System.Drawing.Point(21, 41);
            this.lblProductLabel.Name = "lblProductLabel";
            this.lblProductLabel.Size = new System.Drawing.Size(74, 16);
            this.lblProductLabel.TabIndex = 1;
            this.lblProductLabel.Text = "Producto:";
            // 
            // btnChooseProduct
            // 
            this.btnChooseProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseProduct.Location = new System.Drawing.Point(297, 436);
            this.btnChooseProduct.Name = "btnChooseProduct";
            this.btnChooseProduct.Size = new System.Drawing.Size(110, 41);
            this.btnChooseProduct.TabIndex = 4;
            this.btnChooseProduct.Text = "Elegir";
            this.btnChooseProduct.UseVisualStyleBackColor = true;
            this.btnChooseProduct.Click += new System.EventHandler(this.btnChooseProduct_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(166, 436);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(110, 41);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Salir";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnKb
            // 
            this.btnKb.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnKb.Location = new System.Drawing.Point(373, 33);
            this.btnKb.Name = "btnKb";
            this.btnKb.Size = new System.Drawing.Size(44, 35);
            this.btnKb.TabIndex = 1;
            this.btnKb.Text = "7";
            this.btnKb.UseVisualStyleBackColor = true;
            this.btnKb.Visible = false;
            this.btnKb.Click += new System.EventHandler(this.btnKb_Click);
            // 
            // txtProduct
            // 
            this.txtProduct.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtProduct.Location = new System.Drawing.Point(101, 33);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.NullText = "Producto a Buscar";
            // 
            // 
            // 
            this.txtProduct.RootElement.ControlBounds = new System.Drawing.Rectangle(101, 33, 100, 20);
            this.txtProduct.RootElement.StretchVertically = true;
            this.txtProduct.Size = new System.Drawing.Size(266, 30);
            this.txtProduct.TabIndex = 0;
            this.txtProduct.TabStop = false;
            this.txtProduct.ThemeName = "TelerikMetroTouch";
            this.txtProduct.TextChanged += new System.EventHandler(this.txtProduct_TextChanged);
            // 
            // imgArticulo
            // 
            this.imgArticulo.HeaderText = "FOTO";
            this.imgArticulo.Name = "imgArticulo";
            this.imgArticulo.ReadOnly = true;
            // 
            // articulo
            // 
            this.articulo.DataPropertyName = "ARTICULO";
            this.articulo.HeaderText = "ARTICULO";
            this.articulo.Name = "articulo";
            this.articulo.ReadOnly = true;
            this.articulo.Width = 280;
            // 
            // barras
            // 
            this.barras.DataPropertyName = "BARRAS";
            this.barras.HeaderText = "CODIGO";
            this.barras.Name = "barras";
            this.barras.ReadOnly = true;
            // 
            // SearchProductV2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkKhaki;
            this.ClientSize = new System.Drawing.Size(577, 491);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.btnKb);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnChooseProduct);
            this.Controls.Add(this.lblProductLabel);
            this.Controls.Add(this.dgvSearchProduct);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchProductV2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Búsqueda de Productos";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SearchProduct_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProduct)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProductLabel;
        private System.Windows.Forms.DataGridView dgvSearchProduct;
        private System.Windows.Forms.Button btnChooseProduct;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnKb;
        private Telerik.WinControls.UI.RadTextBox txtProduct;
        private System.Windows.Forms.DataGridViewImageColumn imgArticulo;
        private System.Windows.Forms.DataGridViewTextBoxColumn articulo;
        private System.Windows.Forms.DataGridViewTextBoxColumn barras;
    }
}