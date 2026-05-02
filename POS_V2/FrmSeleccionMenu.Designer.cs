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

            // ── pnlSuperior ──────────────────────────────────────────
            this.pnlSuperior.BackColor = System.Drawing.Color.FromArgb(0, 88, 42);
            this.pnlSuperior.Controls.Add(this.label4);
            this.pnlSuperior.Controls.Add(this.txtBusqueda);
            this.pnlSuperior.Controls.Add(this.pbIcono);
            this.pnlSuperior.Controls.Add(this.label3);
            this.pnlSuperior.Controls.Add(this.lbHeader);
            this.pnlSuperior.Dock = System.Windows.Forms.DockStyle.Top;   // <-- CAMBIO: ocupa todo el ancho siempre
            this.pnlSuperior.Location = new System.Drawing.Point(0, 0);
            this.pnlSuperior.Name = "pnlSuperior";
            this.pnlSuperior.Size = new System.Drawing.Size(1024, 100);    // <-- alto reducido a 100
            this.pnlSuperior.TabIndex = 0;

            // ── txtBusqueda ───────────────────────────────────────────
            this.txtBusqueda.Anchor = System.Windows.Forms.AnchorStyles.Top
                                    | System.Windows.Forms.AnchorStyles.Right; // <-- CAMBIO
            this.txtBusqueda.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBusqueda.DefaultText = "";
            this.txtBusqueda.DisabledState.BorderColor = System.Drawing.Color.FromArgb(208, 208, 208);
            this.txtBusqueda.DisabledState.FillColor = System.Drawing.Color.FromArgb(226, 226, 226);
            this.txtBusqueda.DisabledState.ForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtBusqueda.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(138, 138, 138);
            this.txtBusqueda.FocusedState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtBusqueda.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBusqueda.HoverState.BorderColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.txtBusqueda.Location = new System.Drawing.Point(580, 50);  // <-- ajustado
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.PlaceholderText = "";
            this.txtBusqueda.SelectedText = "";
            this.txtBusqueda.Size = new System.Drawing.Size(280, 29);       // <-- ancho reducido
            this.txtBusqueda.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtBusqueda.TabIndex = 4;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);

            // ── pbIcono ───────────────────────────────────────────────
            this.pbIcono.Location = new System.Drawing.Point(20, 25);
            this.pbIcono.Name = "pbIcono";
            this.pbIcono.Size = new System.Drawing.Size(45, 45);
            this.pbIcono.TabIndex = 2;
            this.pbIcono.TabStop = false;

            // ── label3 ────────────────────────────────────────────────
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(72, 58);
            this.label3.Name = "label3";
            this.label3.TabIndex = 1;
            this.label3.Text = "ELIJA LOS PRODUCTOS PARA AGREGAR A LA VENTA";

            // ── lbHeader ──────────────────────────────────────────────
            this.lbHeader.AutoSize = true;
            this.lbHeader.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.lbHeader.ForeColor = System.Drawing.Color.White;
            this.lbHeader.Location = new System.Drawing.Point(70, 20);
            this.lbHeader.Name = "lbHeader";
            this.lbHeader.TabIndex = 0;

            // ── label4 (BUSCAR) ───────────────────────────────────────
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top
                               | System.Windows.Forms.AnchorStyles.Right; // <-- CAMBIO
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(490, 53);      // <-- ajustado
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "BUSCAR";

            // ── pnlIzquierda ─────────────────────────────────────────
            this.pnlIzquierda.Anchor = System.Windows.Forms.AnchorStyles.Top    // <-- CAMBIO: agregado Top
                                     | System.Windows.Forms.AnchorStyles.Bottom
                                     | System.Windows.Forms.AnchorStyles.Right;  // quitamos Left
            this.pnlIzquierda.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlIzquierda.Controls.Add(this.btnGuardarRamo);
            this.pnlIzquierda.Controls.Add(this.flowLayoutPanel1);
            this.pnlIzquierda.Controls.Add(this.label2);
            this.pnlIzquierda.Controls.Add(this.label1);
            this.pnlIzquierda.Controls.Add(this.bntCancelar);
            this.pnlIzquierda.Controls.Add(this.bntAgregarP);
            this.pnlIzquierda.Controls.Add(this.lbResumen);
            this.pnlIzquierda.Location = new System.Drawing.Point(694, 100); // <-- ajustado
            this.pnlIzquierda.Name = "pnlIzquierda";
            this.pnlIzquierda.Size = new System.Drawing.Size(330, 668);      // <-- ajustado
            this.pnlIzquierda.TabIndex = 1;

            // ── lbResumen ─────────────────────────────────────────────
            this.lbResumen.AutoSize = true;
            this.lbResumen.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lbResumen.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbResumen.Location = new System.Drawing.Point(7, 10);
            this.lbResumen.Name = "lbResumen";
            this.lbResumen.TabIndex = 0;
            this.lbResumen.Text = "RESUMEN SELECCION";

            // ── flowLayoutPanel1 (lista del carrito) ──────────────────
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top    // <-- CAMBIO
                                         | System.Windows.Forms.AnchorStyles.Bottom
                                         | System.Windows.Forms.AnchorStyles.Left
                                         | System.Windows.Forms.AnchorStyles.Right;
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 35);       // <-- ajustado
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(322, 490);         // <-- más alto
            this.flowLayoutPanel1.TabIndex = 5;
            this.flowLayoutPanel1.WrapContents = false;

            // ── label1 (SUBTOTAL) ─────────────────────────────────────
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom           // <-- CAMBIO
                               | System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label1.Location = new System.Drawing.Point(7, 535);
            this.label1.Name = "label1";
            this.label1.TabIndex = 3;
            this.label1.Text = "SUBTOTAL:";

            // ── label2 (TOTAL ITEMS) ──────────────────────────────────
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom           // <-- CAMBIO
                               | System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(0, 88, 42);
            this.label2.Location = new System.Drawing.Point(7, 558);
            this.label2.Name = "label2";
            this.label2.TabIndex = 4;
            this.label2.Text = "TOTAL ITEMS:";

            // ── btnGuardarRamo ────────────────────────────────────────
            this.btnGuardarRamo.Anchor = System.Windows.Forms.AnchorStyles.Bottom   // <-- CAMBIO
                                       | System.Windows.Forms.AnchorStyles.Left
                                       | System.Windows.Forms.AnchorStyles.Right;
            this.btnGuardarRamo.AutoSize = true;
            this.btnGuardarRamo.BackColor = System.Drawing.Color.FromArgb(0, 88, 42);
            this.btnGuardarRamo.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnGuardarRamo.ForeColor = System.Drawing.Color.White;
            this.btnGuardarRamo.Location = new System.Drawing.Point(5, 580);
            this.btnGuardarRamo.Name = "btnGuardarRamo";
            this.btnGuardarRamo.Size = new System.Drawing.Size(318, 35);
            this.btnGuardarRamo.TabIndex = 6;
            this.btnGuardarRamo.Text = "GENERAR ARREGLO FLORAL";
            this.btnGuardarRamo.UseVisualStyleBackColor = false;
            this.btnGuardarRamo.Click += new System.EventHandler(this.btnGuardarRamo_Click);

            // ── bntAgregarP ───────────────────────────────────────────
            this.bntAgregarP.Anchor = System.Windows.Forms.AnchorStyles.Bottom      // <-- CAMBIO
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;
            this.bntAgregarP.AutoSize = true;
            this.bntAgregarP.BackColor = System.Drawing.Color.FromArgb(0, 88, 42);
            this.bntAgregarP.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.bntAgregarP.ForeColor = System.Drawing.Color.White;
            this.bntAgregarP.Location = new System.Drawing.Point(5, 620);
            this.bntAgregarP.Name = "bntAgregarP";
            this.bntAgregarP.Size = new System.Drawing.Size(318, 35);
            this.bntAgregarP.TabIndex = 1;
            this.bntAgregarP.Text = "AGREGAR A LA VENTA";
            this.bntAgregarP.UseVisualStyleBackColor = false;
            this.bntAgregarP.Click += new System.EventHandler(this.btnAceptar_Click);

            // ── bntCancelar ───────────────────────────────────────────
            this.bntCancelar.Anchor = System.Windows.Forms.AnchorStyles.Bottom      // <-- CAMBIO
                                    | System.Windows.Forms.AnchorStyles.Left
                                    | System.Windows.Forms.AnchorStyles.Right;
            this.bntCancelar.AutoSize = true;
            this.bntCancelar.BackColor = System.Drawing.Color.White;
            this.bntCancelar.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.bntCancelar.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.bntCancelar.Location = new System.Drawing.Point(5, 660);
            this.bntCancelar.Name = "bntCancelar";
            this.bntCancelar.Size = new System.Drawing.Size(318, 30);
            this.bntCancelar.TabIndex = 2;
            this.bntCancelar.Text = "CANCELAR";
            this.bntCancelar.UseVisualStyleBackColor = false;
            this.bntCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // ── flpnlitems (panel productos izquierda) ────────────────
            this.flpnlitems.Anchor = System.Windows.Forms.AnchorStyles.Top
                                   | System.Windows.Forms.AnchorStyles.Bottom
                                   | System.Windows.Forms.AnchorStyles.Left
                                   | System.Windows.Forms.AnchorStyles.Right;       // ya estaba bien
            this.flpnlitems.AutoScroll = true;
            this.flpnlitems.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.flpnlitems.Location = new System.Drawing.Point(0, 100);
            this.flpnlitems.Name = "flpnlitems";
            this.flpnlitems.Size = new System.Drawing.Size(694, 668);              // <-- ajustado
            this.flpnlitems.TabIndex = 2;

            // ── FrmSeleccionMenu ──────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1024, 768);                   // <-- CAMBIO base 1024x768
            this.MinimumSize = new System.Drawing.Size(900, 650);                   // <-- NUEVO mínimo
            this.Controls.Add(this.pnlSuperior);
            this.Controls.Add(this.flpnlitems);
            this.Controls.Add(this.pnlIzquierda);
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