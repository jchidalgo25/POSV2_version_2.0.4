namespace POS.Control.Main
{
    partial class PriceViewer
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
            this.lblDescuentos = new Telerik.WinControls.UI.RadLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNombreProducto = new System.Windows.Forms.Label();
            this.lblTotal = new Telerik.WinControls.UI.RadLabel();
            this.lblIva = new Telerik.WinControls.UI.RadLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSubtotal = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProducto)).BeginInit();
            this.grpProducto.SuspendLayout();
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
            this.btnSearch.Location = new System.Drawing.Point(335, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(39, 34);
            this.btnSearch.TabIndex = 17;
            this.btnSearch.Text = "8";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblProductLabel
            // 
            this.lblProductLabel.AutoSize = true;
            this.lblProductLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductLabel.Location = new System.Drawing.Point(25, 24);
            this.lblProductLabel.Name = "lblProductLabel";
            this.lblProductLabel.Size = new System.Drawing.Size(74, 16);
            this.lblProductLabel.TabIndex = 16;
            this.lblProductLabel.Text = "Pistolear:";
            // 
            // txtProducto
            // 
            this.txtProducto.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtProducto.Location = new System.Drawing.Point(105, 16);
            this.txtProducto.Name = "txtProducto";
            this.txtProducto.NullText = "Pistolee el producto";
            // 
            // 
            // 
            this.txtProducto.RootElement.ControlBounds = new System.Drawing.Rectangle(105, 16, 100, 20);
            this.txtProducto.RootElement.StretchVertically = true;
            this.txtProducto.Size = new System.Drawing.Size(232, 30);
            this.txtProducto.TabIndex = 15;
            this.txtProducto.TabStop = false;
            this.txtProducto.ThemeName = "TelerikMetroTouch";
            this.txtProducto.TextChanged += new System.EventHandler(this.txtProducto_TextChanged);
            this.txtProducto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProducto_KeyPress);
            // 
            // grpProducto
            // 
            this.grpProducto.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.grpProducto.BackColor = System.Drawing.Color.Transparent;
            this.grpProducto.Controls.Add(this.lblDescuentos);
            this.grpProducto.Controls.Add(this.label4);
            this.grpProducto.Controls.Add(this.lblNombreProducto);
            this.grpProducto.Controls.Add(this.lblTotal);
            this.grpProducto.Controls.Add(this.lblIva);
            this.grpProducto.Controls.Add(this.label3);
            this.grpProducto.Controls.Add(this.label2);
            this.grpProducto.Controls.Add(this.label1);
            this.grpProducto.Controls.Add(this.lblSubtotal);
            this.grpProducto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpProducto.HeaderText = "Nombre Producto";
            this.grpProducto.Location = new System.Drawing.Point(0, 54);
            this.grpProducto.Name = "grpProducto";
            // 
            // 
            // 
            this.grpProducto.RootElement.Padding = new System.Windows.Forms.Padding(2, 18, 2, 2);
            this.grpProducto.Size = new System.Drawing.Size(419, 211);
            this.grpProducto.TabIndex = 18;
            this.grpProducto.TabStop = false;
            this.grpProducto.Text = "Nombre Producto";
            this.grpProducto.ThemeName = "TelerikMetroTouch";
            // 
            // lblDescuentos
            // 
            this.lblDescuentos.AutoSize = false;
            this.lblDescuentos.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescuentos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDescuentos.Location = new System.Drawing.Point(191, 100);
            this.lblDescuentos.Name = "lblDescuentos";
            this.lblDescuentos.Size = new System.Drawing.Size(220, 41);
            this.lblDescuentos.TabIndex = 15;
            this.lblDescuentos.Text = "0.00";
            this.lblDescuentos.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDescuentos.ThemeName = "TelerikMetroTouch";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label4.Location = new System.Drawing.Point(7, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 23);
            this.label4.TabIndex = 14;
            this.label4.Text = "Descuentos:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNombreProducto
            // 
            this.lblNombreProducto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.lblNombreProducto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreProducto.ForeColor = System.Drawing.Color.White;
            this.lblNombreProducto.Location = new System.Drawing.Point(0, 34);
            this.lblNombreProducto.Name = "lblNombreProducto";
            this.lblNombreProducto.Size = new System.Drawing.Size(419, 23);
            this.lblNombreProducto.TabIndex = 1;
            this.lblNombreProducto.Text = "- - -";
            this.lblNombreProducto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = false;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(190, 161);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(220, 41);
            this.lblTotal.TabIndex = 13;
            this.lblTotal.Text = "0.00";
            this.lblTotal.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTotal.ThemeName = "TelerikMetroTouch";
            // 
            // lblIva
            // 
            this.lblIva.AutoSize = false;
            this.lblIva.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIva.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblIva.Location = new System.Drawing.Point(190, 130);
            this.lblIva.Name = "lblIva";
            this.lblIva.Size = new System.Drawing.Size(220, 41);
            this.lblIva.TabIndex = 12;
            this.lblIva.Text = "0.00";
            this.lblIva.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblIva.ThemeName = "TelerikMetroTouch";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label3.Location = new System.Drawing.Point(6, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "Total:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(6, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Iva:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label1.Location = new System.Drawing.Point(6, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Precio base:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = false;
            this.lblSubtotal.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSubtotal.Location = new System.Drawing.Point(190, 69);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(220, 41);
            this.lblSubtotal.TabIndex = 11;
            this.lblSubtotal.Text = "0.00";
            this.lblSubtotal.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSubtotal.ThemeName = "TelerikMetroTouch";
            // 
            // PriceViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 266);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblProductLabel);
            this.Controls.Add(this.txtProducto);
            this.Controls.Add(this.grpProducto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PriceViewer";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visor de precios";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PriceViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpProducto)).EndInit();
            this.grpProducto.ResumeLayout(false);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNombreProducto;
        private Telerik.WinControls.UI.RadLabel lblSubtotal;
        private Telerik.WinControls.UI.RadLabel lblTotal;
        private Telerik.WinControls.UI.RadLabel lblIva;
        private Telerik.WinControls.UI.RadLabel lblDescuentos;
        private System.Windows.Forms.Label label4;
    }
}
