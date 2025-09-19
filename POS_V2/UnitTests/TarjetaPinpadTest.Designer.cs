namespace POS.UnitTests
{
    partial class TarjetaPinpadTest
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnValidar = new System.Windows.Forms.Button();
            this.txtEstablecimiento = new System.Windows.Forms.TextBox();
            this.txtPtoEmision = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(63, 157);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(266, 20);
            this.textBox1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Ingrese tarjeta:";
            // 
            // btnValidar
            // 
            this.btnValidar.Location = new System.Drawing.Point(62, 183);
            this.btnValidar.Name = "btnValidar";
            this.btnValidar.Size = new System.Drawing.Size(267, 23);
            this.btnValidar.TabIndex = 50;
            this.btnValidar.Text = "Validar como Pago Pinpad";
            this.btnValidar.UseVisualStyleBackColor = true;
            this.btnValidar.Click += new System.EventHandler(this.btnValidar_Click);
            // 
            // txtEstablecimiento
            // 
            this.txtEstablecimiento.Location = new System.Drawing.Point(63, 95);
            this.txtEstablecimiento.Name = "txtEstablecimiento";
            this.txtEstablecimiento.Size = new System.Drawing.Size(79, 20);
            this.txtEstablecimiento.TabIndex = 6;
            // 
            // txtPtoEmision
            // 
            this.txtPtoEmision.Location = new System.Drawing.Point(148, 95);
            this.txtPtoEmision.Name = "txtPtoEmision";
            this.txtPtoEmision.Size = new System.Drawing.Size(74, 20);
            this.txtPtoEmision.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Establecimiento:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Pto Emision:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(62, 212);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(267, 23);
            this.button1.TabIndex = 51;
            this.button1.Text = "Validar como BasePagos Procesa Pinpad";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TarjetaPinpadTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPtoEmision);
            this.Controls.Add(this.txtEstablecimiento);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnValidar);
            this.Name = "TarjetaPinpadTest";
            this.Text = "TarjetaPinpadTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnValidar;
        private System.Windows.Forms.TextBox txtEstablecimiento;
        private System.Windows.Forms.TextBox txtPtoEmision;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
    }
}