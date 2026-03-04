using System.Windows.Forms;

namespace POS
{
    partial class FrmSeleccionMenu
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
            this.pnlSuperior = new System.Windows.Forms.Panel();
            this.txtBusqueda = new Guna.UI2.WinForms.Guna2TextBox();
            this.pbIcono = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbHeader = new System.Windows.Forms.Label();
            this.pnlIzquierda = new System.Windows.Forms.Panel();
            this.btnGuardarRamo = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bntCancelar = new System.Windows.Forms.Button();
            this.bntAgregarP = new System.Windows.Forms.Button();
            this.lbResumen = new System.Windows.Forms.Label();
            this.flpnlitems = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcono)).BeginInit();
            this.pnlIzquierda.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSuperior
            // 
            this.pnlSuperior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.pnlSuperior.Controls.Add(this.label4);
            this.pnlSuperior.Controls.Add(this.txtBusqueda);
            this.pnlSuperior.Controls.Add(this.pbIcono);
            this.pnlSuperior.Controls.Add(this.label3);
            this.pnlSuperior.Controls.Add(this.lbHeader);
            this.pnlSuperior.Location = new System.Drawing.Point(0, -2);
            this.pnlSuperior.Name = "pnlSuperior";
            this.pnlSuperior.Size = new System.Drawing.Size(1304, 117);
            this.pnlSuperior.TabIndex = 0;
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBusqueda.DefaultText = "";
            this.txtBusqueda.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtBusqueda.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtBusqueda.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBusqueda.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBusqueda.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBusqueda.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBusqueda.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBusqueda.Location = new System.Drawing.Point(752, 62);
            this.txtBusqueda.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.PlaceholderText = "";
            this.txtBusqueda.SelectedText = "";
            this.txtBusqueda.Size = new System.Drawing.Size(370, 29);
            this.txtBusqueda.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtBusqueda.TabIndex = 4;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            // 
            // pbIcono
            // 
            this.pbIcono.Location = new System.Drawing.Point(20, 34);
            this.pbIcono.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbIcono.Name = "pbIcono";
            this.pbIcono.Size = new System.Drawing.Size(51, 49);
            this.pbIcono.TabIndex = 2;
            this.pbIcono.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(76, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(409, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "ELIJA LOS PRODUCTOS PARA AGREGAR A LA VENTA";
            // 
            // lbHeader
            // 
            this.lbHeader.AutoSize = true;
            this.lbHeader.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHeader.ForeColor = System.Drawing.Color.White;
            this.lbHeader.Location = new System.Drawing.Point(70, 26);
            this.lbHeader.Name = "lbHeader";
            this.lbHeader.Size = new System.Drawing.Size(0, 40);
            this.lbHeader.TabIndex = 0;
            // 
            // pnlIzquierda
            // 
            this.pnlIzquierda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlIzquierda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlIzquierda.Controls.Add(this.btnGuardarRamo);
            this.pnlIzquierda.Controls.Add(this.flowLayoutPanel1);
            this.pnlIzquierda.Controls.Add(this.label2);
            this.pnlIzquierda.Controls.Add(this.label1);
            this.pnlIzquierda.Controls.Add(this.bntCancelar);
            this.pnlIzquierda.Controls.Add(this.bntAgregarP);
            this.pnlIzquierda.Controls.Add(this.lbResumen);
            this.pnlIzquierda.Location = new System.Drawing.Point(972, -43);
            this.pnlIzquierda.Name = "pnlIzquierda";
            this.pnlIzquierda.Size = new System.Drawing.Size(332, 793);
            this.pnlIzquierda.TabIndex = 1;
            // 
            // btnGuardarRamo
            // 
            this.btnGuardarRamo.AutoSize = true;
            this.btnGuardarRamo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.btnGuardarRamo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarRamo.ForeColor = System.Drawing.Color.White;
            this.btnGuardarRamo.Location = new System.Drawing.Point(22, 648);
            this.btnGuardarRamo.Name = "btnGuardarRamo";
            this.btnGuardarRamo.Size = new System.Drawing.Size(290, 45);
            this.btnGuardarRamo.TabIndex = 6;
            this.btnGuardarRamo.Text = "GENERAR ARREGLO FLORAL";
            this.btnGuardarRamo.UseVisualStyleBackColor = false;
            this.btnGuardarRamo.Click += new System.EventHandler(this.btnGuardarRamo_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 201);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(327, 379);
            this.flowLayoutPanel1.TabIndex = 5;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.label2.Location = new System.Drawing.Point(27, 612);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "TOTAL ITEMS:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 584);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "SUBTOTAL:";
            // 
            // bntCancelar
            // 
            this.bntCancelar.AutoSize = true;
            this.bntCancelar.BackColor = System.Drawing.Color.White;
            this.bntCancelar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bntCancelar.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.bntCancelar.Location = new System.Drawing.Point(22, 744);
            this.bntCancelar.Name = "bntCancelar";
            this.bntCancelar.Size = new System.Drawing.Size(290, 37);
            this.bntCancelar.TabIndex = 2;
            this.bntCancelar.Text = "CANCELAR";
            this.bntCancelar.UseVisualStyleBackColor = false;
            this.bntCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // bntAgregarP
            // 
            this.bntAgregarP.AutoSize = true;
            this.bntAgregarP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(88)))), ((int)(((byte)(42)))));
            this.bntAgregarP.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bntAgregarP.ForeColor = System.Drawing.Color.White;
            this.bntAgregarP.Location = new System.Drawing.Point(21, 693);
            this.bntAgregarP.Name = "bntAgregarP";
            this.bntAgregarP.Size = new System.Drawing.Size(290, 43);
            this.bntAgregarP.TabIndex = 1;
            this.bntAgregarP.Text = "AGREGAR A LA VENTA";
            this.bntAgregarP.UseVisualStyleBackColor = false;
            this.bntAgregarP.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // lbResumen
            // 
            this.lbResumen.AutoSize = true;
            this.lbResumen.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResumen.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbResumen.Location = new System.Drawing.Point(7, 173);
            this.lbResumen.Name = "lbResumen";
            this.lbResumen.Size = new System.Drawing.Size(180, 23);
            this.lbResumen.TabIndex = 0;
            this.lbResumen.Text = "RESUMEN SELECCION";
            // 
            // flpnlitems
            // 
            this.flpnlitems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpnlitems.AutoScroll = true;
            this.flpnlitems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.flpnlitems.Location = new System.Drawing.Point(0, 115);
            this.flpnlitems.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.flpnlitems.Name = "flpnlitems";
            this.flpnlitems.Size = new System.Drawing.Size(972, 635);
            this.flpnlitems.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(643, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "BUSCAR";
            // 
            // FrmSeleccionMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1306, 750);
            this.Controls.Add(this.pnlSuperior);
            this.Controls.Add(this.flpnlitems);
            this.Controls.Add(this.pnlIzquierda);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmSeleccionMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmSeleccionMenu";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmSeleccionMenu_Load);
            this.pnlSuperior.ResumeLayout(false);
            this.pnlSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcono)).EndInit();
            this.pnlIzquierda.ResumeLayout(false);
            this.pnlIzquierda.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSuperior;
        private System.Windows.Forms.Label lbHeader;
        private System.Windows.Forms.Panel pnlIzquierda;
        private System.Windows.Forms.Label lbResumen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bntCancelar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button bntAgregarP;
        private System.Windows.Forms.FlowLayoutPanel flpnlitems;
        private System.Windows.Forms.PictureBox pbIcono;
        private Guna.UI2.WinForms.Guna2TextBox txtBusqueda;
        private Button btnGuardarRamo;
        private Label label4;
    }
}