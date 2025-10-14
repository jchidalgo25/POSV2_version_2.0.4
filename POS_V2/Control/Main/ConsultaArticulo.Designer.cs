namespace POS.Control.Main
{
    partial class ConsultaArticulo
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblProductLabel = new System.Windows.Forms.Label();
            this.txtProducto = new Telerik.WinControls.UI.RadTextBox();
            this.grpProducto = new Telerik.WinControls.UI.RadGroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDescuentos = new Telerik.WinControls.UI.RadLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotal = new Telerik.WinControls.UI.RadLabel();
            this.lblIva = new Telerik.WinControls.UI.RadLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSubtotal = new Telerik.WinControls.UI.RadLabel();
            this.lblNombreProducto = new System.Windows.Forms.Label();
            this.btnKb = new System.Windows.Forms.Button();
            this.btnElegir = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProducto)).BeginInit();
            this.grpProducto.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblDescuentos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblIva)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSubtotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Wingdings 3", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSearch.Location = new System.Drawing.Point(317, 21);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(55, 48);
            this.btnSearch.TabIndex = 20;
            this.btnSearch.Text = "8";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblProductLabel
            // 
            this.lblProductLabel.AutoSize = true;
            this.lblProductLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductLabel.Location = new System.Drawing.Point(12, 9);
            this.lblProductLabel.Name = "lblProductLabel";
            this.lblProductLabel.Size = new System.Drawing.Size(114, 16);
            this.lblProductLabel.TabIndex = 19;
            this.lblProductLabel.Text = "Código Artículo";
            // 
            // txtProducto
            // 
            this.txtProducto.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtProducto.Location = new System.Drawing.Point(15, 28);
            this.txtProducto.Name = "txtProducto";
            this.txtProducto.NullText = "Ingrese Código de Artículo";
            // 
            // 
            // 
            this.txtProducto.RootElement.ControlBounds = new System.Drawing.Rectangle(15, 28, 100, 20);
            this.txtProducto.RootElement.StretchVertically = true;
            this.txtProducto.Size = new System.Drawing.Size(298, 30);
            this.txtProducto.TabIndex = 18;
            this.txtProducto.TabStop = false;
            this.txtProducto.ThemeName = "TelerikMetroTouch";
            this.txtProducto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProducto_KeyPress);
            // 
            // grpProducto
            // 
            this.grpProducto.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.grpProducto.BackColor = System.Drawing.Color.Transparent;
            this.grpProducto.Controls.Add(this.panel1);
            this.grpProducto.Controls.Add(this.lblNombreProducto);
            this.grpProducto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpProducto.HeaderText = "Nombre Producto";
            this.grpProducto.Location = new System.Drawing.Point(12, 82);
            this.grpProducto.Name = "grpProducto";
            // 
            // 
            // 
            this.grpProducto.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.grpProducto.Size = new System.Drawing.Size(419, 217);
            this.grpProducto.TabIndex = 21;
            this.grpProducto.TabStop = false;
            this.grpProducto.Text = "Nombre Producto";
            this.grpProducto.ThemeName = "TelerikMetroTouch";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDescuentos);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lblTotal);
            this.panel1.Controls.Add(this.lblIva);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblSubtotal);
            this.panel1.Location = new System.Drawing.Point(5, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(409, 148);
            this.panel1.TabIndex = 2;
            // 
            // lblDescuentos
            // 
            this.lblDescuentos.AutoSize = false;
            this.lblDescuentos.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescuentos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDescuentos.Location = new System.Drawing.Point(163, 41);
            this.lblDescuentos.Name = "lblDescuentos";
            this.lblDescuentos.Size = new System.Drawing.Size(220, 41);
            this.lblDescuentos.TabIndex = 23;
            this.lblDescuentos.Text = "0.00";
            this.lblDescuentos.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDescuentos.ThemeName = "TelerikMetroTouch";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label4.Location = new System.Drawing.Point(7, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 23);
            this.label4.TabIndex = 22;
            this.label4.Text = "Descuentos:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = false;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(163, 102);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(220, 41);
            this.lblTotal.TabIndex = 21;
            this.lblTotal.Text = "0.00";
            this.lblTotal.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTotal.ThemeName = "TelerikMetroTouch";
            // 
            // lblIva
            // 
            this.lblIva.AutoSize = false;
            this.lblIva.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIva.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblIva.Location = new System.Drawing.Point(163, 71);
            this.lblIva.Name = "lblIva";
            this.lblIva.Size = new System.Drawing.Size(220, 41);
            this.lblIva.TabIndex = 20;
            this.lblIva.Text = "0.00";
            this.lblIva.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblIva.ThemeName = "TelerikMetroTouch";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label3.Location = new System.Drawing.Point(6, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 23);
            this.label3.TabIndex = 18;
            this.label3.Text = "Total:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(6, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 23);
            this.label2.TabIndex = 17;
            this.label2.Text = "Iva:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 23);
            this.label1.TabIndex = 16;
            this.label1.Text = "Precio base:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = false;
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSubtotal.Location = new System.Drawing.Point(163, 10);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(220, 41);
            this.lblSubtotal.TabIndex = 19;
            this.lblSubtotal.Text = "0.00";
            this.lblSubtotal.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSubtotal.ThemeName = "TelerikMetroTouch";
            // 
            // lblNombreProducto
            // 
            this.lblNombreProducto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.lblNombreProducto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreProducto.ForeColor = System.Drawing.Color.White;
            this.lblNombreProducto.Location = new System.Drawing.Point(0, 34);
            this.lblNombreProducto.Name = "lblNombreProducto";
            this.lblNombreProducto.Size = new System.Drawing.Size(442, 23);
            this.lblNombreProducto.TabIndex = 1;
            this.lblNombreProducto.Text = "- - -";
            this.lblNombreProducto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnKb
            // 
            this.btnKb.Font = new System.Drawing.Font("Wingdings", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnKb.Location = new System.Drawing.Point(378, 21);
            this.btnKb.Name = "btnKb";
            this.btnKb.Size = new System.Drawing.Size(53, 48);
            this.btnKb.TabIndex = 22;
            this.btnKb.Text = "7";
            this.btnKb.UseVisualStyleBackColor = true;
            this.btnKb.Click += new System.EventHandler(this.btnKb_Click);
            // 
            // btnElegir
            // 
            this.btnElegir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnElegir.Location = new System.Drawing.Point(212, 305);
            this.btnElegir.Name = "btnElegir";
            this.btnElegir.Size = new System.Drawing.Size(101, 61);
            this.btnElegir.TabIndex = 23;
            this.btnElegir.Text = "Elegir";
            this.btnElegir.UseVisualStyleBackColor = true;
            this.btnElegir.Click += new System.EventHandler(this.btnElegir_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.button2.Location = new System.Drawing.Point(340, 305);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 61);
            this.button2.TabIndex = 24;
            this.button2.Text = "Cancelar";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ConsultaArticulo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(445, 380);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnElegir);
            this.Controls.Add(this.btnKb);
            this.Controls.Add(this.grpProducto);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblProductLabel);
            this.Controls.Add(this.txtProducto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConsultaArticulo";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Ingreso Producto Manual";
            this.ThemeName = "TelerikMetroTouch";
            this.Load += new System.EventHandler(this.ConsultaArticulo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProducto)).EndInit();
            this.grpProducto.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lblDescuentos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblIva)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSubtotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblProductLabel;
        private Telerik.WinControls.UI.RadTextBox txtProducto;
        private Telerik.WinControls.UI.RadGroupBox grpProducto;
        private System.Windows.Forms.Label lblNombreProducto;
        private System.Windows.Forms.Button btnKb;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadLabel lblDescuentos;
        private System.Windows.Forms.Label label4;
        private Telerik.WinControls.UI.RadLabel lblTotal;
        private Telerik.WinControls.UI.RadLabel lblIva;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadLabel lblSubtotal;
        private System.Windows.Forms.Button btnElegir;
        private System.Windows.Forms.Button button2;
    }
}