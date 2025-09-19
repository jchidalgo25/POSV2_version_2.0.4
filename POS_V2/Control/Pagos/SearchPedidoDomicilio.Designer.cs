namespace POS.Control.Clientes
{
    partial class SearchPedidoDomicilio
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
            this.lblNumeroPedido = new System.Windows.Forms.Label();
            this.btnChooseProduct = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnKb = new System.Windows.Forms.Button();
            this.txtNumPedidoDomicilio = new Telerik.WinControls.UI.RadTextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumPedidoDomicilio)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNumeroPedido
            // 
            this.lblNumeroPedido.AutoSize = true;
            this.lblNumeroPedido.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroPedido.Location = new System.Drawing.Point(11, 22);
            this.lblNumeroPedido.Name = "lblNumeroPedido";
            this.lblNumeroPedido.Size = new System.Drawing.Size(184, 16);
            this.lblNumeroPedido.TabIndex = 1;
            this.lblNumeroPedido.Text = "Número Pedido Domicilio";
            // 
            // btnChooseProduct
            // 
            this.btnChooseProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseProduct.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnChooseProduct.Location = new System.Drawing.Point(275, 74);
            this.btnChooseProduct.Name = "btnChooseProduct";
            this.btnChooseProduct.Size = new System.Drawing.Size(110, 41);
            this.btnChooseProduct.TabIndex = 4;
            this.btnChooseProduct.Text = "Aceptar";
            this.btnChooseProduct.UseVisualStyleBackColor = true;
            this.btnChooseProduct.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnExit.Location = new System.Drawing.Point(132, 74);
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
            this.btnKb.Location = new System.Drawing.Point(438, 11);
            this.btnKb.Name = "btnKb";
            this.btnKb.Size = new System.Drawing.Size(44, 34);
            this.btnKb.TabIndex = 1;
            this.btnKb.Text = "7";
            this.btnKb.UseVisualStyleBackColor = true;
            this.btnKb.Click += new System.EventHandler(this.btnKb_Click);
            // 
            // txtNumPedidoDomicilio
            // 
            this.txtNumPedidoDomicilio.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtNumPedidoDomicilio.Location = new System.Drawing.Point(202, 11);
            this.txtNumPedidoDomicilio.Name = "txtNumPedidoDomicilio";
            this.txtNumPedidoDomicilio.NullText = "# Pedido Domicilio";
            // 
            // 
            // 
            this.txtNumPedidoDomicilio.RootElement.ControlBounds = new System.Drawing.Rectangle(202, 11, 100, 20);
            this.txtNumPedidoDomicilio.RootElement.StretchVertically = true;
            this.txtNumPedidoDomicilio.Size = new System.Drawing.Size(183, 34);
            this.txtNumPedidoDomicilio.TabIndex = 0;
            this.txtNumPedidoDomicilio.TabStop = false;
            this.txtNumPedidoDomicilio.ThemeName = "TelerikMetroTouch";
            this.txtNumPedidoDomicilio.TextChanged += new System.EventHandler(this.txtProduct_TextChanged);
            this.txtNumPedidoDomicilio.Enter += new System.EventHandler(this.txtCliente_Enter);
            this.txtNumPedidoDomicilio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumPedidoDomicilio_KeyPress);
            this.txtNumPedidoDomicilio.Leave += new System.EventHandler(this.txtCliente_Leave);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Wingdings 3", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSearch.Location = new System.Drawing.Point(391, 11);
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
            this.panel1.Controls.Add(this.lblNumeroPedido);
            this.panel1.Controls.Add(this.txtNumPedidoDomicilio);
            this.panel1.Controls.Add(this.btnChooseProduct);
            this.panel1.Controls.Add(this.btnKb);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(498, 135);
            this.panel1.TabIndex = 8;
            // 
            // SearchPedidoDomicilio
            // 
            this.AcceptButton = this.btnChooseProduct;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(498, 128);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchPedidoDomicilio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Búsqueda de Productos";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SearchProduct_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtNumPedidoDomicilio)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNumeroPedido;
        private System.Windows.Forms.Button btnChooseProduct;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnKb;
        private Telerik.WinControls.UI.RadTextBox txtNumPedidoDomicilio;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Panel panel1;
    }
}