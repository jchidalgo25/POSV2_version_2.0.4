namespace POS.Control.Encuestas
{
    partial class Encuesta
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
            this.lblPregunta = new Telerik.WinControls.UI.RadLabel();
            this.btnSiguiente = new Telerik.WinControls.UI.RadButton();
            this.btnAbandonar = new Telerik.WinControls.UI.RadButton();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.txtRespuesta = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.lblPregunta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSiguiente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAbandonar)).BeginInit();
            this.pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPregunta
            // 
            this.lblPregunta.Font = new System.Drawing.Font("Segoe UI", 19.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPregunta.Location = new System.Drawing.Point(3, 3);
            this.lblPregunta.MaximumSize = new System.Drawing.Size(1000, 0);
            this.lblPregunta.Name = "lblPregunta";
            // 
            // 
            // 
            this.lblPregunta.RootElement.MaxSize = new System.Drawing.Size(1000, 0);
            this.lblPregunta.Size = new System.Drawing.Size(758, 39);
            this.lblPregunta.TabIndex = 5;
            this.lblPregunta.Text = "¿Qué faltó para completar su compra? (Marca/Presentacion)";
            this.lblPregunta.ThemeName = "TelerikMetroTouch";
            // 
            // btnSiguiente
            // 
            this.btnSiguiente.Location = new System.Drawing.Point(513, 36);
            this.btnSiguiente.Name = "btnSiguiente";
            this.btnSiguiente.Size = new System.Drawing.Size(287, 46);
            this.btnSiguiente.TabIndex = 11;
            this.btnSiguiente.Text = "Siguiente pregunta >";
            this.btnSiguiente.TextWrap = true;
            this.btnSiguiente.ThemeName = "TelerikMetroTouch";
            this.btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            // 
            // btnAbandonar
            // 
            this.btnAbandonar.Location = new System.Drawing.Point(7, 36);
            this.btnAbandonar.Name = "btnAbandonar";
            this.btnAbandonar.Size = new System.Drawing.Size(169, 46);
            this.btnAbandonar.TabIndex = 12;
            this.btnAbandonar.Text = "Abandonar";
            this.btnAbandonar.TextWrap = true;
            this.btnAbandonar.ThemeName = "TelerikMetroTouch";
            this.btnAbandonar.Click += new System.EventHandler(this.btnAbandonar_Click);
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.txtRespuesta);
            this.pnlControls.Controls.Add(this.lblPregunta);
            this.pnlControls.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlControls.Location = new System.Drawing.Point(4, 97);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(796, 400);
            this.pnlControls.TabIndex = 35;
            // 
            // txtRespuesta
            // 
            this.txtRespuesta.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRespuesta.Location = new System.Drawing.Point(26, 46);
            this.txtRespuesta.Name = "txtRespuesta";
            this.txtRespuesta.Size = new System.Drawing.Size(744, 281);
            this.txtRespuesta.TabIndex = 6;
            this.txtRespuesta.Text = "";
            // 
            // Encuesta
            // 
            this.AcceptButton = this.btnSiguiente;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 530);
            this.ControlBox = false;
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.btnAbandonar);
            this.Controls.Add(this.btnSiguiente);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Encuesta";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Consulta";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Encuesta_FormClosed);
            this.Load += new System.EventHandler(this.Encuesta_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lblPregunta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSiguiente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAbandonar)).EndInit();
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Telerik.WinControls.UI.RadLabel lblPregunta;
        private Telerik.WinControls.UI.RadButton btnSiguiente;
        private Telerik.WinControls.UI.RadButton btnAbandonar;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.RichTextBox txtRespuesta;
    }
}
