namespace POS
{
    partial class SearchProductV3
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
            this.imgArticulo = new System.Windows.Forms.DataGridViewImageColumn();
            this.articulo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.barras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblProductLabel = new System.Windows.Forms.Label();
            this.btnChooseProduct = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnKb = new System.Windows.Forms.Button();
            this.txtProduct = new Telerik.WinControls.UI.RadTextBox();
            this.panelCuerpoResultado = new System.Windows.Forms.Panel();
            this.tblLay_Top20Productos = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tblLay_CategoriaProducto = new System.Windows.Forms.TableLayoutPanel();
            this.radThemeManager1 = new Telerik.WinControls.RadThemeManager();
            this.rbtnCategoria = new Telerik.WinControls.UI.RadButton();
            this.tblLay_Principal = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblEtiquetaCategoriaSeleccionada = new Telerik.WinControls.UI.RadLabel();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.radbtnUp = new Telerik.WinControls.UI.RadButton();
            this.radbtnDown = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProduct)).BeginInit();
            this.panelCuerpoResultado.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnCategoria)).BeginInit();
            this.tblLay_Principal.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblEtiquetaCategoriaSeleccionada)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radbtnUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radbtnDown)).BeginInit();
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
            this.dgvSearchProduct.Location = new System.Drawing.Point(1070, 89);
            this.dgvSearchProduct.MultiSelect = false;
            this.dgvSearchProduct.Name = "dgvSearchProduct";
            this.dgvSearchProduct.ReadOnly = true;
            this.dgvSearchProduct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSearchProduct.Size = new System.Drawing.Size(293, 272);
            this.dgvSearchProduct.TabIndex = 2;
            this.dgvSearchProduct.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchProduct_CellContentDoubleClick);
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
            // lblProductLabel
            // 
            this.lblProductLabel.AutoSize = true;
            this.lblProductLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductLabel.ForeColor = System.Drawing.Color.White;
            this.lblProductLabel.Location = new System.Drawing.Point(17, 17);
            this.lblProductLabel.Name = "lblProductLabel";
            this.lblProductLabel.Size = new System.Drawing.Size(74, 16);
            this.lblProductLabel.TabIndex = 1;
            this.lblProductLabel.Text = "Producto:";
            // 
            // btnChooseProduct
            // 
            this.btnChooseProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseProduct.Location = new System.Drawing.Point(253, 0);
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
            this.btnExit.Location = new System.Drawing.Point(369, 0);
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
            this.btnKb.Location = new System.Drawing.Point(369, 9);
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
            this.txtProduct.Location = new System.Drawing.Point(97, 9);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.NullText = "Producto a Buscar";
            // 
            // 
            // 
            this.txtProduct.RootElement.ControlBounds = new System.Drawing.Rectangle(97, 9, 100, 20);
            this.txtProduct.RootElement.StretchVertically = true;
            this.txtProduct.Size = new System.Drawing.Size(266, 30);
            this.txtProduct.TabIndex = 0;
            this.txtProduct.TabStop = false;
            this.txtProduct.ThemeName = "TelerikMetroTouch";
            this.txtProduct.TextChanged += new System.EventHandler(this.txtProduct_TextChanged);
            this.txtProduct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProduct_KeyPress);
            // 
            // panelCuerpoResultado
            // 
            this.panelCuerpoResultado.AutoScroll = true;
            this.panelCuerpoResultado.Controls.Add(this.tblLay_Top20Productos);
            this.panelCuerpoResultado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCuerpoResultado.Location = new System.Drawing.Point(85, 64);
            this.panelCuerpoResultado.Name = "panelCuerpoResultado";
            this.panelCuerpoResultado.Size = new System.Drawing.Size(652, 279);
            this.panelCuerpoResultado.TabIndex = 6;
            // 
            // tblLay_Top20Productos
            // 
            this.tblLay_Top20Productos.AutoScroll = true;
            this.tblLay_Top20Productos.ColumnCount = 3;
            this.tblLay_Top20Productos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblLay_Top20Productos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tblLay_Top20Productos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tblLay_Top20Productos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLay_Top20Productos.Location = new System.Drawing.Point(0, 0);
            this.tblLay_Top20Productos.Name = "tblLay_Top20Productos";
            this.tblLay_Top20Productos.RowCount = 2;
            this.tblLay_Top20Productos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLay_Top20Productos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLay_Top20Productos.Size = new System.Drawing.Size(652, 279);
            this.tblLay_Top20Productos.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tblLay_CategoriaProducto);
            this.panel1.Location = new System.Drawing.Point(923, 89);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(127, 305);
            this.panel1.TabIndex = 7;
            // 
            // tblLay_CategoriaProducto
            // 
            this.tblLay_CategoriaProducto.ColumnCount = 2;
            this.tblLay_CategoriaProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLay_CategoriaProducto.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLay_CategoriaProducto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLay_CategoriaProducto.Location = new System.Drawing.Point(0, 0);
            this.tblLay_CategoriaProducto.Name = "tblLay_CategoriaProducto";
            this.tblLay_CategoriaProducto.RowCount = 2;
            this.tblLay_CategoriaProducto.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLay_CategoriaProducto.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLay_CategoriaProducto.Size = new System.Drawing.Size(127, 305);
            this.tblLay_CategoriaProducto.TabIndex = 0;
            // 
            // rbtnCategoria
            // 
            this.rbtnCategoria.Image = global::POS.Properties.Resources.categories;
            this.rbtnCategoria.Location = new System.Drawing.Point(419, 3);
            this.rbtnCategoria.Name = "rbtnCategoria";
            this.rbtnCategoria.Size = new System.Drawing.Size(150, 46);
            this.rbtnCategoria.TabIndex = 8;
            this.rbtnCategoria.Text = "Categorias";
            this.rbtnCategoria.ThemeName = "TelerikMetroTouch";
            this.rbtnCategoria.Click += new System.EventHandler(this.rbtnCategoria_Click);
            // 
            // tblLay_Principal
            // 
            this.tblLay_Principal.ColumnCount = 3;
            this.tblLay_Principal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblLay_Principal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tblLay_Principal.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblLay_Principal.Controls.Add(this.panel2, 1, 2);
            this.tblLay_Principal.Controls.Add(this.panelCuerpoResultado, 1, 1);
            this.tblLay_Principal.Controls.Add(this.panel3, 1, 0);
            this.tblLay_Principal.Controls.Add(this.radPanel1, 2, 1);
            this.tblLay_Principal.Location = new System.Drawing.Point(12, 25);
            this.tblLay_Principal.Name = "tblLay_Principal";
            this.tblLay_Principal.RowCount = 3;
            this.tblLay_Principal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tblLay_Principal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tblLay_Principal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tblLay_Principal.Size = new System.Drawing.Size(823, 408);
            this.tblLay_Principal.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnExit);
            this.panel2.Controls.Add(this.btnChooseProduct);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(85, 349);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(652, 56);
            this.panel2.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblEtiquetaCategoriaSeleccionada);
            this.panel3.Controls.Add(this.txtProduct);
            this.panel3.Controls.Add(this.rbtnCategoria);
            this.panel3.Controls.Add(this.lblProductLabel);
            this.panel3.Controls.Add(this.btnKb);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(85, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(652, 55);
            this.panel3.TabIndex = 4;
            // 
            // lblEtiquetaCategoriaSeleccionada
            // 
            this.lblEtiquetaCategoriaSeleccionada.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtiquetaCategoriaSeleccionada.ForeColor = System.Drawing.Color.White;
            this.lblEtiquetaCategoriaSeleccionada.Location = new System.Drawing.Point(575, 17);
            this.lblEtiquetaCategoriaSeleccionada.Name = "lblEtiquetaCategoriaSeleccionada";
            this.lblEtiquetaCategoriaSeleccionada.Size = new System.Drawing.Size(2, 2);
            this.lblEtiquetaCategoriaSeleccionada.TabIndex = 1;
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.radbtnUp);
            this.radPanel1.Controls.Add(this.radbtnDown);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(743, 64);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(77, 279);
            this.radPanel1.TabIndex = 7;
            // 
            // radbtnUp
            // 
            this.radbtnUp.Dock = System.Windows.Forms.DockStyle.Top;
            this.radbtnUp.Image = global::POS.Properties.Resources.sort_up;
            this.radbtnUp.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radbtnUp.Location = new System.Drawing.Point(0, 0);
            this.radbtnUp.Name = "radbtnUp";
            this.radbtnUp.Size = new System.Drawing.Size(77, 56);
            this.radbtnUp.TabIndex = 10;
            this.radbtnUp.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.radbtnUp.Click += new System.EventHandler(this.radbtnUp_Click);
            // 
            // radbtnDown
            // 
            this.radbtnDown.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radbtnDown.Image = global::POS.Properties.Resources.sort_down;
            this.radbtnDown.ImageAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.radbtnDown.Location = new System.Drawing.Point(0, 223);
            this.radbtnDown.Name = "radbtnDown";
            this.radbtnDown.Size = new System.Drawing.Size(77, 56);
            this.radbtnDown.TabIndex = 7;
            this.radbtnDown.Click += new System.EventHandler(this.radbtnDown_Click);
            // 
            // SearchProductV3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(1024, 586);
            this.Controls.Add(this.tblLay_Principal);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvSearchProduct);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchProductV3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Búsqueda de Productos";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SearchProduct_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProduct)).EndInit();
            this.panelCuerpoResultado.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rbtnCategoria)).EndInit();
            this.tblLay_Principal.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblEtiquetaCategoriaSeleccionada)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radbtnUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radbtnDown)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Panel panelCuerpoResultado;
        private System.Windows.Forms.TableLayoutPanel tblLay_Top20Productos;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.RadThemeManager radThemeManager1;
        private System.Windows.Forms.TableLayoutPanel tblLay_CategoriaProducto;
        private Telerik.WinControls.UI.RadButton rbtnCategoria;
        private System.Windows.Forms.TableLayoutPanel tblLay_Principal;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadButton radbtnUp;
        private Telerik.WinControls.UI.RadButton radbtnDown;
        private Telerik.WinControls.UI.RadLabel lblEtiquetaCategoriaSeleccionada;
    }
}