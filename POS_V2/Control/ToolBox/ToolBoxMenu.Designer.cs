namespace POS.Control.ToolBox
{
    partial class ToolBoxMenu
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
            this.btnRecargas = new Telerik.WinControls.UI.RadButton();
            this.btnPagoServicios = new Telerik.WinControls.UI.RadButton();
            this.btnRecargarRedActiva = new Telerik.WinControls.UI.RadButton();
            this.btnEstresarPOS = new Telerik.WinControls.UI.RadButton();
            this.btnPistoleoMetricas = new Telerik.WinControls.UI.RadButton();
            this.btnProbarBalanzaDL = new Telerik.WinControls.UI.RadButton();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.OpenCashDrawer = new Telerik.WinControls.UI.RadButton();
            this.btnEjecutaTramaPinPad = new Telerik.WinControls.UI.RadButton();
            this.btnPantallaCliente = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecargas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPagoServicios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecargarRedActiva)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEstresarPOS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPistoleoMetricas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProbarBalanzaDL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpenCashDrawer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEjecutaTramaPinPad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPantallaCliente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRecargas
            // 
            this.btnRecargas.Location = new System.Drawing.Point(639, 12);
            this.btnRecargas.Name = "btnRecargas";
            this.btnRecargas.Size = new System.Drawing.Size(39, 37);
            this.btnRecargas.TabIndex = 1;
            this.btnRecargas.Text = "F1";
            this.btnRecargas.ThemeName = "TelerikMetroTouch";
            this.btnRecargas.Click += new System.EventHandler(this.btnRecargas_Click);
            // 
            // btnPagoServicios
            // 
            this.btnPagoServicios.Location = new System.Drawing.Point(12, 230);
            this.btnPagoServicios.Name = "btnPagoServicios";
            this.btnPagoServicios.Size = new System.Drawing.Size(95, 37);
            this.btnPagoServicios.TabIndex = 2;
            this.btnPagoServicios.Text = "Cerrar POS";
            this.btnPagoServicios.ThemeName = "TelerikMetroTouch";
            this.btnPagoServicios.Click += new System.EventHandler(this.btnCerrarPOS_Click);
            // 
            // btnRecargarRedActiva
            // 
            this.btnRecargarRedActiva.Location = new System.Drawing.Point(12, 12);
            this.btnRecargarRedActiva.Name = "btnRecargarRedActiva";
            this.btnRecargarRedActiva.Size = new System.Drawing.Size(162, 52);
            this.btnRecargarRedActiva.TabIndex = 3;
            this.btnRecargarRedActiva.Text = "Recargar Red Activa";
            this.btnRecargarRedActiva.TextWrap = true;
            this.btnRecargarRedActiva.ThemeName = "TelerikMetroTouch";
            this.btnRecargarRedActiva.Click += new System.EventHandler(this.btnRecargarRedActiva_Click);
            // 
            // btnEstresarPOS
            // 
            this.btnEstresarPOS.Location = new System.Drawing.Point(12, 70);
            this.btnEstresarPOS.Name = "btnEstresarPOS";
            this.btnEstresarPOS.Size = new System.Drawing.Size(162, 57);
            this.btnEstresarPOS.TabIndex = 4;
            this.btnEstresarPOS.Text = "Estresar POS (no usar en produccion)";
            this.btnEstresarPOS.TextWrap = true;
            this.btnEstresarPOS.ThemeName = "TelerikMetroTouch";
            this.btnEstresarPOS.Click += new System.EventHandler(this.btnEstresarPOS_Click);
            // 
            // btnPistoleoMetricas
            // 
            this.btnPistoleoMetricas.Location = new System.Drawing.Point(568, 243);
            this.btnPistoleoMetricas.Name = "btnPistoleoMetricas";
            this.btnPistoleoMetricas.Size = new System.Drawing.Size(110, 24);
            this.btnPistoleoMetricas.TabIndex = 7;
            // 
            // btnProbarBalanzaDL
            // 
            this.btnProbarBalanzaDL.Location = new System.Drawing.Point(180, 12);
            this.btnProbarBalanzaDL.Name = "btnProbarBalanzaDL";
            this.btnProbarBalanzaDL.Size = new System.Drawing.Size(179, 52);
            this.btnProbarBalanzaDL.TabIndex = 6;
            this.btnProbarBalanzaDL.Text = "Probar Balanza DataLogic";
            this.btnProbarBalanzaDL.TextWrap = true;
            this.btnProbarBalanzaDL.ThemeName = "TelerikMetroTouch";
            this.btnProbarBalanzaDL.Click += new System.EventHandler(this.btnProbarBalanzaDL_Click);
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(12, 133);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(162, 57);
            this.radButton1.TabIndex = 8;
            this.radButton1.Text = "ON/OFF Metricas de captura producto";
            this.radButton1.TextWrap = true;
            this.radButton1.ThemeName = "TelerikMetroTouch";
            this.radButton1.Click += new System.EventHandler(this.btnPistoleoMetricas_Click);
            // 
            // OpenCashDrawer
            // 
            this.OpenCashDrawer.Location = new System.Drawing.Point(180, 75);
            this.OpenCashDrawer.Name = "OpenCashDrawer";
            this.OpenCashDrawer.Size = new System.Drawing.Size(179, 52);
            this.OpenCashDrawer.TabIndex = 9;
            this.OpenCashDrawer.Text = "Open CashDrawer";
            this.OpenCashDrawer.TextWrap = true;
            this.OpenCashDrawer.ThemeName = "TelerikMetroTouch";
            // 
            // btnEjecutaTramaPinPad
            // 
            this.btnEjecutaTramaPinPad.Location = new System.Drawing.Point(180, 133);
            this.btnEjecutaTramaPinPad.Name = "btnEjecutaTramaPinPad";
            this.btnEjecutaTramaPinPad.Size = new System.Drawing.Size(179, 52);
            this.btnEjecutaTramaPinPad.TabIndex = 10;
            this.btnEjecutaTramaPinPad.Text = "Configuración PINPAD";
            this.btnEjecutaTramaPinPad.TextWrap = true;
            this.btnEjecutaTramaPinPad.ThemeName = "TelerikMetroTouch";
            this.btnEjecutaTramaPinPad.Click += new System.EventHandler(this.btnEjecutaTramaPinPad_Click);
            // 
            // btnPantallaCliente
            // 
            this.btnPantallaCliente.Location = new System.Drawing.Point(378, 12);
            this.btnPantallaCliente.Name = "btnPantallaCliente";
            this.btnPantallaCliente.Size = new System.Drawing.Size(164, 37);
            this.btnPantallaCliente.TabIndex = 11;
            this.btnPantallaCliente.Text = "Ver pantalla Cliente";
            this.btnPantallaCliente.ThemeName = "TelerikMetroTouch";
            this.btnPantallaCliente.Click += new System.EventHandler(this.btnPantallaCliente_Click);
            // 
            // ToolBoxMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 279);
            this.Controls.Add(this.btnPantallaCliente);
            this.Controls.Add(this.btnEjecutaTramaPinPad);
            this.Controls.Add(this.OpenCashDrawer);
            this.Controls.Add(this.radButton1);
            this.Controls.Add(this.btnProbarBalanzaDL);
            this.Controls.Add(this.btnPistoleoMetricas);
            this.Controls.Add(this.btnEstresarPOS);
            this.Controls.Add(this.btnRecargarRedActiva);
            this.Controls.Add(this.btnPagoServicios);
            this.Controls.Add(this.btnRecargas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ToolBoxMenu";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ToolBox";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ToolBoxMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnRecargas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPagoServicios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecargarRedActiva)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEstresarPOS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPistoleoMetricas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnProbarBalanzaDL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpenCashDrawer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEjecutaTramaPinPad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPantallaCliente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Telerik.WinControls.UI.RadButton btnRecargas;
        private Telerik.WinControls.UI.RadButton btnPagoServicios;
        private Telerik.WinControls.UI.RadButton btnRecargarRedActiva;
        private Telerik.WinControls.UI.RadButton btnEstresarPOS;
        private Telerik.WinControls.UI.RadButton btnPistoleoMetricas;
        private Telerik.WinControls.UI.RadButton btnProbarBalanzaDL;
        private Telerik.WinControls.UI.RadButton radButton1;
        private Telerik.WinControls.UI.RadButton OpenCashDrawer;
        private Telerik.WinControls.UI.RadButton btnEjecutaTramaPinPad;
        private Telerik.WinControls.UI.RadButton btnPantallaCliente;
    }
}
