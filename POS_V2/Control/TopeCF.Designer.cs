namespace POS.Control
{
    partial class TopeCF
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopeCF));
            this.lblMensaje = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmdCambiaCliente = new System.Windows.Forms.Button();
            this.cmdFinalizaFactura = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tmr = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMensaje
            // 
            this.lblMensaje.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblMensaje.Location = new System.Drawing.Point(276, 48);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(507, 203);
            this.lblMensaje.TabIndex = 0;
            this.lblMensaje.Text = "Consumidor Final";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(18, 43);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(243, 208);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // cmdCambiaCliente
            // 
            this.cmdCambiaCliente.Location = new System.Drawing.Point(400, 312);
            this.cmdCambiaCliente.Name = "cmdCambiaCliente";
            this.cmdCambiaCliente.Size = new System.Drawing.Size(202, 79);
            this.cmdCambiaCliente.TabIndex = 2;
            this.cmdCambiaCliente.Text = "Cambiar Cliente";
            this.cmdCambiaCliente.UseVisualStyleBackColor = true;
            this.cmdCambiaCliente.Visible = false;
            this.cmdCambiaCliente.Click += new System.EventHandler(this.cmdCambiaCliente_Click);
            // 
            // cmdFinalizaFactura
            // 
            this.cmdFinalizaFactura.Location = new System.Drawing.Point(608, 312);
            this.cmdFinalizaFactura.Name = "cmdFinalizaFactura";
            this.cmdFinalizaFactura.Size = new System.Drawing.Size(202, 79);
            this.cmdFinalizaFactura.TabIndex = 3;
            this.cmdFinalizaFactura.Text = "Continuar Factura";
            this.cmdFinalizaFactura.UseVisualStyleBackColor = true;
            this.cmdFinalizaFactura.Click += new System.EventHandler(this.cmdFinalizaFactura_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdFinalizaFactura);
            this.panel1.Controls.Add(this.cmdCambiaCliente);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblMensaje);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(813, 391);
            this.panel1.TabIndex = 4;
            // 
            // tmr
            // 
            this.tmr.Enabled = true;
            this.tmr.Interval = 250;
            this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
            // 
            // TopeCF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(839, 404);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Name = "TopeCF";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.TopeCF_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cmdCambiaCliente;
        private System.Windows.Forms.Button cmdFinalizaFactura;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer tmr;
    }
}