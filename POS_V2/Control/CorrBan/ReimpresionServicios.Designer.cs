namespace POS.Control.CorrBan
{
    partial class ReimpresionServicios
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.btnConsultar = new Telerik.WinControls.UI.RadButton();
            this.txtCuenta = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.gridRecaudaciones = new Telerik.WinControls.UI.RadGridView();
            this.btnImprimir = new Telerik.WinControls.UI.RadButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnKbd = new Telerik.WinControls.UI.RadButton();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.btnReversar = new Telerik.WinControls.UI.RadButton();
            this.paneLoading = new System.Windows.Forms.Panel();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnConsultar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCuenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecaudaciones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecaudaciones.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnImprimir)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnReversar)).BeginInit();
            this.paneLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConsultar
            // 
            this.btnConsultar.Location = new System.Drawing.Point(608, 12);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(169, 46);
            this.btnConsultar.TabIndex = 11;
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.TextWrap = true;
            this.btnConsultar.ThemeName = "TelerikMetroTouch";
            this.btnConsultar.Click += new System.EventHandler(this.btnConsultar_Click);
            // 
            // txtCuenta
            // 
            this.txtCuenta.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCuenta.Location = new System.Drawing.Point(153, 12);
            this.txtCuenta.Name = "txtCuenta";
            this.txtCuenta.NullText = "Escriba texto...";
            this.txtCuenta.Size = new System.Drawing.Size(420, 46);
            this.txtCuenta.TabIndex = 10;
            this.txtCuenta.TabStop = false;
            this.txtCuenta.ThemeName = "TelerikMetroTouch";
            this.txtCuenta.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCuenta_KeyDown);
            this.txtCuenta.Leave += new System.EventHandler(this.txtControl_Leave);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(27, 28);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(79, 30);
            this.radLabel4.TabIndex = 11;
            this.radLabel4.Text = "Cuenta:";
            this.radLabel4.ThemeName = "TelerikMetroTouch";
            // 
            // gridRecaudaciones
            // 
            this.gridRecaudaciones.EnableKineticScrolling = true;
            this.gridRecaudaciones.Location = new System.Drawing.Point(27, 74);
            // 
            // gridRecaudaciones
            // 
            this.gridRecaudaciones.MasterTemplate.AllowAddNewRow = false;
            this.gridRecaudaciones.MasterTemplate.AllowColumnReorder = false;
            this.gridRecaudaciones.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewTextBoxColumn1.FieldName = "Cuenta";
            gridViewTextBoxColumn1.HeaderText = "Cuenta";
            gridViewTextBoxColumn1.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            gridViewTextBoxColumn1.Name = "Cuenta";
            gridViewTextBoxColumn1.Width = 209;
            gridViewTextBoxColumn2.FieldName = "Corresponsal";
            gridViewTextBoxColumn2.HeaderText = "Operadora";
            gridViewTextBoxColumn2.Name = "Corresponsal";
            gridViewTextBoxColumn2.Width = 209;
            gridViewTextBoxColumn3.FieldName = "Fecha";
            gridViewTextBoxColumn3.HeaderText = "Fecha";
            gridViewTextBoxColumn3.Name = "Fecha";
            gridViewTextBoxColumn3.Width = 209;
            gridViewTextBoxColumn4.FieldName = "Valor";
            gridViewTextBoxColumn4.HeaderText = "Valor";
            gridViewTextBoxColumn4.Name = "Valor";
            gridViewTextBoxColumn4.Width = 104;
            this.gridRecaudaciones.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4});
            this.gridRecaudaciones.MasterTemplate.EnableGrouping = false;
            this.gridRecaudaciones.Name = "gridRecaudaciones";
            this.gridRecaudaciones.ReadOnly = true;
            this.gridRecaudaciones.Size = new System.Drawing.Size(750, 297);
            this.gridRecaudaciones.TabIndex = 12;
            this.gridRecaudaciones.Text = "radGridView1";
            this.gridRecaudaciones.ThemeName = "TelerikMetroTouch";
            // 
            // btnImprimir
            // 
            this.btnImprimir.Location = new System.Drawing.Point(608, 388);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(169, 46);
            this.btnImprimir.TabIndex = 9;
            this.btnImprimir.Text = "Imprimir";
            this.btnImprimir.TextWrap = true;
            this.btnImprimir.ThemeName = "TelerikMetroTouch";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnKbd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 450);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(805, 43);
            this.panel1.TabIndex = 33;
            // 
            // btnKbd
            // 
            this.btnKbd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnKbd.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnKbd.Location = new System.Drawing.Point(12, 7);
            this.btnKbd.MaximumSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Name = "btnKbd";
            // 
            // 
            // 
            this.btnKbd.RootElement.ControlBounds = new System.Drawing.Rectangle(12, 7, 110, 24);
            this.btnKbd.RootElement.MaxSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Size = new System.Drawing.Size(77, 32);
            this.btnKbd.TabIndex = 18;
            this.btnKbd.Text = "Teclado";
            this.btnKbd.ThemeName = "TelerikMetroTouch";
            this.btnKbd.Visible = false;
            this.btnKbd.Click += new System.EventHandler(this.btnKbd_Click);
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(27, 90);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(67, 30);
            this.radLabel2.TabIndex = 35;
            this.radLabel2.Text = "Fecha:";
            this.radLabel2.ThemeName = "TelerikMetroTouch";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Font = new System.Drawing.Font("Segoe UI", 20.25F);
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(153, 77);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(420, 43);
            this.dtpFecha.TabIndex = 36;
            // 
            // btnReversar
            // 
            this.btnReversar.BackColor = System.Drawing.Color.White;
            this.btnReversar.Location = new System.Drawing.Point(27, 388);
            this.btnReversar.Name = "btnReversar";
            this.btnReversar.Size = new System.Drawing.Size(169, 46);
            this.btnReversar.TabIndex = 10;
            this.btnReversar.Text = "Reversar";
            this.btnReversar.TextWrap = true;
            this.btnReversar.ThemeName = "TelerikMetroTouch";
            this.btnReversar.Click += new System.EventHandler(this.btnReversar_Click);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btnReversar.GetChildAt(0))).Text = "Reversar";
            ((Telerik.WinControls.UI.RadButtonElement)(this.btnReversar.GetChildAt(0))).BackColor = System.Drawing.Color.White;
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.btnReversar.GetChildAt(0).GetChildAt(0))).ForeColor = System.Drawing.SystemColors.ControlText;
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.btnReversar.GetChildAt(0).GetChildAt(0))).BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(63)))), ((int)(((byte)(63)))));
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.btnReversar.GetChildAt(0).GetChildAt(0))).Font = new System.Drawing.Font("Segoe UI", 11F);
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btnReversar.GetChildAt(0).GetChildAt(1).GetChildAt(1))).TextWrap = true;
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btnReversar.GetChildAt(0).GetChildAt(1).GetChildAt(1))).ForeColor = System.Drawing.Color.White;
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btnReversar.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // paneLoading
            // 
            this.paneLoading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paneLoading.Controls.Add(this.radLabel7);
            this.paneLoading.Controls.Add(this.pictureBox2);
            this.paneLoading.Location = new System.Drawing.Point(146, 168);
            this.paneLoading.Name = "paneLoading";
            this.paneLoading.Size = new System.Drawing.Size(512, 156);
            this.paneLoading.TabIndex = 37;
            this.paneLoading.Visible = false;
            // 
            // radLabel7
            // 
            this.radLabel7.Font = new System.Drawing.Font("Segoe UI", 28.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel7.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.radLabel7.Location = new System.Drawing.Point(195, 49);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(233, 57);
            this.radLabel7.TabIndex = 7;
            this.radLabel7.Text = "Procesando...";
            this.radLabel7.ThemeName = "TelerikMetroTouch";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::POS.Properties.Resources.loading;
            this.pictureBox2.Location = new System.Drawing.Point(55, 21);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(116, 110);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // ReimpresionServicios
            // 
            this.AcceptButton = this.btnImprimir;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 493);
            this.Controls.Add(this.paneLoading);
            this.Controls.Add(this.btnReversar);
            this.Controls.Add(this.gridRecaudaciones);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.txtCuenta);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.btnConsultar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ReimpresionServicios";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Historial de comprobantes";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Reimpresion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnConsultar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCuenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecaudaciones.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecaudaciones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnImprimir)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnReversar)).EndInit();
            this.paneLoading.ResumeLayout(false);
            this.paneLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnConsultar;
        private Telerik.WinControls.UI.RadTextBox txtCuenta;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadGridView gridRecaudaciones;
        private Telerik.WinControls.UI.RadButton btnImprimir;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadButton btnKbd;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private Telerik.WinControls.UI.RadButton btnReversar;
        private System.Windows.Forms.Panel paneLoading;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
