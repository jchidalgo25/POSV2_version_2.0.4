namespace POS.Control.Clientes
{
    partial class SearchClient
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
            this.IDENTIFICACION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CLIENTE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblProductLabel = new System.Windows.Forms.Label();
            this.btnChooseProduct = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnKb = new System.Windows.Forms.Button();
            this.txtCliente = new Telerik.WinControls.UI.RadTextBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchProduct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCliente)).BeginInit();
            this.panel1.SuspendLayout();
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
            this.IDENTIFICACION,
            this.CLIENTE});
            this.dgvSearchProduct.Location = new System.Drawing.Point(21, 70);
            this.dgvSearchProduct.MultiSelect = false;
            this.dgvSearchProduct.Name = "dgvSearchProduct";
            this.dgvSearchProduct.ReadOnly = true;
            this.dgvSearchProduct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSearchProduct.Size = new System.Drawing.Size(448, 346);
            this.dgvSearchProduct.TabIndex = 2;
            this.dgvSearchProduct.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearchProduct_CellContentDoubleClick);
            // 
            // IDENTIFICACION
            // 
            this.IDENTIFICACION.DataPropertyName = "ACCOUNTNUM";
            this.IDENTIFICACION.HeaderText = "IDENTIFICACION";
            this.IDENTIFICACION.Name = "IDENTIFICACION";
            this.IDENTIFICACION.ReadOnly = true;
            this.IDENTIFICACION.Width = 130;
            // 
            // CLIENTE
            // 
            this.CLIENTE.DataPropertyName = "NAME";
            this.CLIENTE.HeaderText = "CLIENTE";
            this.CLIENTE.Name = "CLIENTE";
            this.CLIENTE.ReadOnly = true;
            this.CLIENTE.Width = 250;
            // 
            // lblProductLabel
            // 
            this.lblProductLabel.AutoSize = true;
            this.lblProductLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductLabel.Location = new System.Drawing.Point(21, 30);
            this.lblProductLabel.Name = "lblProductLabel";
            this.lblProductLabel.Size = new System.Drawing.Size(60, 16);
            this.lblProductLabel.TabIndex = 1;
            this.lblProductLabel.Text = "Cliente:";
            // 
            // btnChooseProduct
            // 
            this.btnChooseProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseProduct.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnChooseProduct.Location = new System.Drawing.Point(249, 426);
            this.btnChooseProduct.Name = "btnChooseProduct";
            this.btnChooseProduct.Size = new System.Drawing.Size(110, 41);
            this.btnChooseProduct.TabIndex = 4;
            this.btnChooseProduct.Text = "Elegir";
            this.btnChooseProduct.UseVisualStyleBackColor = true;
            this.btnChooseProduct.Click += new System.EventHandler(this.btnChooseProduct_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnExit.Location = new System.Drawing.Point(118, 426);
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
            this.btnKb.Location = new System.Drawing.Point(327, 20);
            this.btnKb.Name = "btnKb";
            this.btnKb.Size = new System.Drawing.Size(44, 34);
            this.btnKb.TabIndex = 1;
            this.btnKb.Text = "7";
            this.btnKb.UseVisualStyleBackColor = true;
            this.btnKb.Visible = false;
            this.btnKb.Click += new System.EventHandler(this.btnKb_Click);
            // 
            // txtCliente
            // 
            this.txtCliente.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtCliente.Location = new System.Drawing.Point(101, 22);
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.NullText = "Cliente a Buscar";
            // 
            // 
            // 
            this.txtCliente.RootElement.ControlBounds = new System.Drawing.Rectangle(101, 22, 100, 20);
            this.txtCliente.RootElement.StretchVertically = true;
            this.txtCliente.Size = new System.Drawing.Size(183, 30);
            this.txtCliente.TabIndex = 0;
            this.txtCliente.TabStop = false;
            this.txtCliente.ThemeName = "TelerikMetroTouch";
            this.txtCliente.TextChanged += new System.EventHandler(this.txtProduct_TextChanged);
            this.txtCliente.Enter += new System.EventHandler(this.txtCliente_Enter);
            this.txtCliente.Leave += new System.EventHandler(this.txtCliente_Leave);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnNuevo.Location = new System.Drawing.Point(374, 20);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(95, 34);
            this.btnNuevo.TabIndex = 6;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Wingdings 3", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSearch.Location = new System.Drawing.Point(279, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(44, 34);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "8";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.dgvSearchProduct);
            this.panel1.Controls.Add(this.btnNuevo);
            this.panel1.Controls.Add(this.lblProductLabel);
            this.panel1.Controls.Add(this.txtCliente);
            this.panel1.Controls.Add(this.btnChooseProduct);
            this.panel1.Controls.Add(this.btnKb);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(495, 486);
            this.panel1.TabIndex = 8;
            // 
            // SearchClient
            // 
            this.AcceptButton = this.btnChooseProduct;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(495, 486);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Búsqueda de Productos";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SearchProduct_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchProduct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCliente)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblProductLabel;
        private System.Windows.Forms.Button btnChooseProduct;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnKb;
        private Telerik.WinControls.UI.RadTextBox txtCliente;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.DataGridViewTextBoxColumn IDENTIFICACION;
        private System.Windows.Forms.DataGridViewTextBoxColumn CLIENTE;
        private System.Windows.Forms.DataGridView dgvSearchProduct;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel panel1;
    }
}