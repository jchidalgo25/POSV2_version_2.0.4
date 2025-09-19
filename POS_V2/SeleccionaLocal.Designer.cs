namespace POS
{
    partial class SeleccionaLocal
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbLocal = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Local";
            // 
            // cmbLocal
            // 
            this.cmbLocal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLocal.FormattingEnabled = true;
            this.cmbLocal.Items.AddRange(new object[] {
            "ALBORADA",
            "BOSQUES DE LA COSTA",
            "BOSQUES DE LA COSTA CAJA 1",
            "GOMEZ RENDON",
            "GRAN MANZANA",
            "LIZARDO GARCIA",
            "ORELLANA",
            "PIAZZA",
            "PPG",
            "VILLA CLUB",
            "LA JOYA",
            "",
            "ALBORADA IP",
            "BOSQUES DE LA COSTA IP",
            "BOSQUES DE LA COSTA CAJA 1 IP",
            "GOMEZ RENDON IP",
            "GRAN MANZANA IP",
            "LIZARDO GARCIA IP",
            "ORELLANA IP",
            "PIAZZA IP",
            "PPG IP",
            "VILLA CLUB IP",
            "LA JOYA IP",
            "",
            "ALBORADA DOMINIO",
            "BOSQUES DE LA COSTA DOMINIO",
            "BOSQUES DE LA COSTA CAJA 1 DOMINIO",
            "GOMEZ RENDON DOMINIO",
            "GRAN MANZANA DOMINIO",
            "LIZARDO GARCIA DOMINIO",
            "ORELLANA DOMINIO",
            "PIAZZA DOMINIO",
            "PPG DOMINIO",
            "VILLA CLUB DOMINIO",
            "LA JOYA DOMINIO",
            "",
            "SERVIDOR PRUEBAS 2",
            "SERVIDOR PRUEBAS 3",
            "MERCADITO",
            "LOCALHOST",
            "ANIBAL IP TEST"});
            this.cmbLocal.Location = new System.Drawing.Point(53, 13);
            this.cmbLocal.Name = "cmbLocal";
            this.cmbLocal.Size = new System.Drawing.Size(263, 21);
            this.cmbLocal.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(240, 85);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "GRABAR";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SeleccionaLocal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 123);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbLocal);
            this.Controls.Add(this.label1);
            this.Name = "SeleccionaLocal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SeleccionaLocal";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SeleccionaLocal_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLocal;
        private System.Windows.Forms.Button btnSave;
    }
}