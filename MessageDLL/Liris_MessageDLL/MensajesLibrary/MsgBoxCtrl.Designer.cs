namespace MensajesLibrary
{
    partial class MsgBoxCtrl
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelDetalle = new System.Windows.Forms.Panel();
            this.lblMessageMore = new System.Windows.Forms.TextBox();
            this.lnkVerMas = new System.Windows.Forms.LinkLabel();
            this.btnCancelar = new Telerik.WinControls.UI.RadButton();
            this.btnAceptar = new Telerik.WinControls.UI.RadButton();
            this.pbStop = new System.Windows.Forms.PictureBox();
            this.lblMessage = new System.Windows.Forms.TextBox();
            this.pL2 = new System.Windows.Forms.Panel();
            this.pbInfo = new System.Windows.Forms.PictureBox();
            this.pbQue = new System.Windows.Forms.PictureBox();
            this.pbWar = new System.Windows.Forms.PictureBox();
            this.pbError = new System.Windows.Forms.PictureBox();
            this.lblTitiuloMessage = new System.Windows.Forms.Label();
            this.pL1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panelDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAceptar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbQue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbWar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbError)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panelDetalle);
            this.panel1.Controls.Add(this.lnkVerMas);
            this.panel1.Controls.Add(this.btnCancelar);
            this.panel1.Controls.Add(this.btnAceptar);
            this.panel1.Controls.Add(this.pbStop);
            this.panel1.Controls.Add(this.lblMessage);
            this.panel1.Controls.Add(this.pL2);
            this.panel1.Controls.Add(this.pbInfo);
            this.panel1.Controls.Add(this.pbQue);
            this.panel1.Controls.Add(this.pbWar);
            this.panel1.Controls.Add(this.pbError);
            this.panel1.Controls.Add(this.lblTitiuloMessage);
            this.panel1.Controls.Add(this.pL1);
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(684, 525);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panelDetalle
            // 
            this.panelDetalle.Controls.Add(this.lblMessageMore);
            this.panelDetalle.Location = new System.Drawing.Point(3, 426);
            this.panelDetalle.Name = "panelDetalle";
            this.panelDetalle.Size = new System.Drawing.Size(678, 91);
            this.panelDetalle.TabIndex = 18;
            this.panelDetalle.Visible = false;
            // 
            // lblMessageMore
            // 
            this.lblMessageMore.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessageMore.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessageMore.Location = new System.Drawing.Point(5, 2);
            this.lblMessageMore.Margin = new System.Windows.Forms.Padding(2);
            this.lblMessageMore.Multiline = true;
            this.lblMessageMore.Name = "lblMessageMore";
            this.lblMessageMore.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblMessageMore.Size = new System.Drawing.Size(662, 87);
            this.lblMessageMore.TabIndex = 15;
            // 
            // lnkVerMas
            // 
            this.lnkVerMas.AutoSize = true;
            this.lnkVerMas.Location = new System.Drawing.Point(5, 385);
            this.lnkVerMas.Name = "lnkVerMas";
            this.lnkVerMas.Size = new System.Drawing.Size(44, 13);
            this.lnkVerMas.TabIndex = 20;
            this.lnkVerMas.TabStop = true;
            this.lnkVerMas.Text = "ver más";
            this.lnkVerMas.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkVerMas_LinkClicked);
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.Location = new System.Drawing.Point(541, 329);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(129, 77);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.ThemeName = "TelerikMetroTouch";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.btnAceptar.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnAceptar.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            this.btnAceptar.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAceptar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAceptar.Location = new System.Drawing.Point(353, 329);
            this.btnAceptar.Margin = new System.Windows.Forms.Padding(2);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(129, 77);
            this.btnAceptar.TabIndex = 2;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.ThemeName = "TelerikMetroTouch";
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // pbStop
            // 
            this.pbStop.Image = global::Liris_MenssageDLL.Properties.Resources.stop;
            this.pbStop.Location = new System.Drawing.Point(7, 7);
            this.pbStop.Name = "pbStop";
            this.pbStop.Size = new System.Drawing.Size(95, 80);
            this.pbStop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbStop.TabIndex = 15;
            this.pbStop.TabStop = false;
            this.pbStop.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Enabled = false;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(8, 92);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(2);
            this.lblMessage.Multiline = true;
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.lblMessage.Size = new System.Drawing.Size(662, 214);
            this.lblMessage.TabIndex = 14;
            // 
            // pL2
            // 
            this.pL2.BackColor = System.Drawing.Color.Silver;
            this.pL2.Location = new System.Drawing.Point(8, 311);
            this.pL2.Name = "pL2";
            this.pL2.Size = new System.Drawing.Size(662, 13);
            this.pL2.TabIndex = 13;
            // 
            // pbInfo
            // 
            this.pbInfo.Image = global::Liris_MenssageDLL.Properties.Resources.info;
            this.pbInfo.Location = new System.Drawing.Point(7, 7);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(95, 80);
            this.pbInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbInfo.TabIndex = 12;
            this.pbInfo.TabStop = false;
            this.pbInfo.Visible = false;
            // 
            // pbQue
            // 
            this.pbQue.Image = global::Liris_MenssageDLL.Properties.Resources.question;
            this.pbQue.Location = new System.Drawing.Point(7, 7);
            this.pbQue.Name = "pbQue";
            this.pbQue.Size = new System.Drawing.Size(95, 80);
            this.pbQue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbQue.TabIndex = 11;
            this.pbQue.TabStop = false;
            this.pbQue.Visible = false;
            // 
            // pbWar
            // 
            this.pbWar.Image = global::Liris_MenssageDLL.Properties.Resources.warning;
            this.pbWar.Location = new System.Drawing.Point(7, 7);
            this.pbWar.Name = "pbWar";
            this.pbWar.Size = new System.Drawing.Size(95, 80);
            this.pbWar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbWar.TabIndex = 10;
            this.pbWar.TabStop = false;
            this.pbWar.Visible = false;
            // 
            // pbError
            // 
            this.pbError.Image = global::Liris_MenssageDLL.Properties.Resources.error;
            this.pbError.Location = new System.Drawing.Point(7, 7);
            this.pbError.Name = "pbError";
            this.pbError.Size = new System.Drawing.Size(95, 80);
            this.pbError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbError.TabIndex = 9;
            this.pbError.TabStop = false;
            this.pbError.Visible = false;
            // 
            // lblTitiuloMessage
            // 
            this.lblTitiuloMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitiuloMessage.Location = new System.Drawing.Point(130, 22);
            this.lblTitiuloMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitiuloMessage.Name = "lblTitiuloMessage";
            this.lblTitiuloMessage.Size = new System.Drawing.Size(426, 28);
            this.lblTitiuloMessage.TabIndex = 7;
            this.lblTitiuloMessage.Text = "lblTitiuloMessage";
            // 
            // pL1
            // 
            this.pL1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pL1.Location = new System.Drawing.Point(8, 72);
            this.pL1.Name = "pL1";
            this.pL1.Size = new System.Drawing.Size(662, 15);
            this.pL1.TabIndex = 1;
            this.pL1.TabStop = true;
            // 
            // MsgBoxCtrl_v1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "MsgBoxCtrl_v1";
            this.Size = new System.Drawing.Size(690, 531);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelDetalle.ResumeLayout(false);
            this.panelDetalle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAceptar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbQue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbWar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbError)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbStop;
        private System.Windows.Forms.TextBox lblMessage;
        private System.Windows.Forms.Panel pL2;
        private System.Windows.Forms.PictureBox pbInfo;
        private System.Windows.Forms.PictureBox pbQue;
        private System.Windows.Forms.PictureBox pbWar;
        private System.Windows.Forms.PictureBox pbError;
        private System.Windows.Forms.Label lblTitiuloMessage;
        private System.Windows.Forms.Panel pL1;
        private Telerik.WinControls.UI.RadButton btnCancelar;
        public Telerik.WinControls.UI.RadButton btnAceptar;
        private System.Windows.Forms.LinkLabel lnkVerMas;
        private System.Windows.Forms.Panel panelDetalle;
        private System.Windows.Forms.TextBox lblMessageMore;
    }
}
