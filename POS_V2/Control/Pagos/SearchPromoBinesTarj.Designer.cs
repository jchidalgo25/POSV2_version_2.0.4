namespace POS.Control.Clientes
{
    partial class SearchPromoBinesTarj
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
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNumTarjeta = new Telerik.WinControls.UI.RadTextBox();
            this.lblMsjPromocion = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumTarjeta)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNumeroPedido
            // 
            this.lblNumeroPedido.AutoSize = true;
            this.lblNumeroPedido.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblNumeroPedido.Location = new System.Drawing.Point(17, 86);
            this.lblNumeroPedido.Name = "lblNumeroPedido";
            this.lblNumeroPedido.Size = new System.Drawing.Size(150, 25);
            this.lblNumeroPedido.TabIndex = 1;
            this.lblNumeroPedido.Text = "Número Tarjeta";
            // 
            // btnAceptar
            // 
            this.btnAceptar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAceptar.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnAceptar.Location = new System.Drawing.Point(339, 182);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(110, 57);
            this.btnAceptar.TabIndex = 4;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Visible = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.btnExit.Location = new System.Drawing.Point(161, 182);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(110, 57);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Salir";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtNumTarjeta);
            this.panel1.Controls.Add(this.lblMsjPromocion);
            this.panel1.Controls.Add(this.lblNumeroPedido);
            this.panel1.Controls.Add(this.btnAceptar);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(602, 247);
            this.panel1.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(3, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(587, 17);
            this.label2.TabIndex = 29;
            this.label2.Text = "deslice la tarjeta por el lector universal para validar si tarjeta aplica promoci" +
    "ón.";
            // 
            // txtNumTarjeta
            // 
            this.txtNumTarjeta.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumTarjeta.Location = new System.Drawing.Point(173, 74);
            this.txtNumTarjeta.MaxLength = 60;
            this.txtNumTarjeta.Name = "txtNumTarjeta";
            this.txtNumTarjeta.PasswordChar = '*';
            this.txtNumTarjeta.Size = new System.Drawing.Size(402, 46);
            this.txtNumTarjeta.TabIndex = 1;
            this.txtNumTarjeta.TabStop = false;
            this.txtNumTarjeta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNumTarjeta.ThemeName = "TelerikMetroTouch";
            this.txtNumTarjeta.TextChanged += new System.EventHandler(this.txtNumTarjeta_TextChanged);
            this.txtNumTarjeta.Enter += new System.EventHandler(this.txtNumTarjeta_Enter);
            this.txtNumTarjeta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumTarjeta_KeyPress);
            // 
            // lblMsjPromocion
            // 
            this.lblMsjPromocion.AutoSize = true;
            this.lblMsjPromocion.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMsjPromocion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblMsjPromocion.Location = new System.Drawing.Point(12, 135);
            this.lblMsjPromocion.Name = "lblMsjPromocion";
            this.lblMsjPromocion.Size = new System.Drawing.Size(0, 29);
            this.lblMsjPromocion.TabIndex = 8;
            // 
            // SearchPromoBinesTarj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(633, 272);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchPromoBinesTarj";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lector Tarjeta Promoción Bines Tarjeta Crédito.";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SearchPromoBinesTarj_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumTarjeta)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblNumeroPedido;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblMsjPromocion;
        private Telerik.WinControls.UI.RadTextBox txtNumTarjeta;
        private System.Windows.Forms.Label label2;
    }
}