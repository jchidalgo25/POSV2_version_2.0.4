namespace POS.Control.Pagos
{
    partial class FrmDevolucionFlete
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
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn2 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDateTimeColumn gridViewDateTimeColumn2 = new Telerik.WinControls.UI.GridViewDateTimeColumn();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnKbd = new Telerik.WinControls.UI.RadButton();
            this.MasterTemplate = new Telerik.WinControls.UI.RadGridView();
            this.btnDevolver = new Telerik.WinControls.UI.RadButton();
            this.lblTotal = new Telerik.WinControls.UI.RadLabel();
            this.lblTotalMotorizado = new Telerik.WinControls.UI.RadLabel();
            this.txtCodigoMotorizado = new Telerik.WinControls.UI.RadTextBox();
            this.btnBuscar = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MasterTemplate.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDevolver)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalMotorizado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoMotorizado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBuscar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel5
            // 
            this.radLabel5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.radLabel5.Location = new System.Drawing.Point(30, 34);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(110, 18);
            this.radLabel5.TabIndex = 7;
            this.radLabel5.Text = "Código Motorizado";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnKbd);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 562);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(776, 60);
            this.panel3.TabIndex = 32;
            // 
            // btnKbd
            // 
            this.btnKbd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnKbd.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnKbd.Location = new System.Drawing.Point(12, 8);
            this.btnKbd.MaximumSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Name = "btnKbd";
            // 
            // 
            // 
            this.btnKbd.RootElement.MaxSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Size = new System.Drawing.Size(110, 24);
            this.btnKbd.TabIndex = 0;
            // 
            // MasterTemplate
            // 
            this.MasterTemplate.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.MasterTemplate.BeginEditMode = Telerik.WinControls.RadGridViewBeginEditMode.BeginEditOnF2;
            this.MasterTemplate.EnableKineticScrolling = true;
            this.MasterTemplate.Location = new System.Drawing.Point(12, 107);
            // 
            // MasterTemplate
            // 
            this.MasterTemplate.MasterTemplate.AllowAddNewRow = false;
            this.MasterTemplate.MasterTemplate.AllowColumnReorder = false;
            gridViewTextBoxColumn5.HeaderText = "OrdenApp";
            gridViewTextBoxColumn5.Name = "OrdenApp";
            gridViewTextBoxColumn6.HeaderText = "Código Motorizado";
            gridViewTextBoxColumn6.Name = "CodigoMotorizado";
            gridViewTextBoxColumn7.HeaderText = "Nombre";
            gridViewTextBoxColumn7.Name = "NombreMotorizado";
            gridViewDecimalColumn2.FormatString = "{0:C}";
            gridViewDecimalColumn2.HeaderText = "Valor";
            gridViewDecimalColumn2.Name = "Total";
            gridViewTextBoxColumn8.HeaderText = "Estado";
            gridViewTextBoxColumn8.Name = "Estado";
            gridViewDateTimeColumn2.HeaderText = "Orden Completado";
            gridViewDateTimeColumn2.Name = "FechaOrdenCompletado";
            this.MasterTemplate.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn5,
            gridViewTextBoxColumn6,
            gridViewTextBoxColumn7,
            gridViewDecimalColumn2,
            gridViewTextBoxColumn8,
            gridViewDateTimeColumn2});
            this.MasterTemplate.MasterTemplate.EnableGrouping = false;
            this.MasterTemplate.Name = "MasterTemplate";
            this.MasterTemplate.ReadOnly = true;
            // 
            // 
            // 
            this.MasterTemplate.RootElement.ControlBounds = new System.Drawing.Rectangle(12, 107, 240, 150);
            this.MasterTemplate.Size = new System.Drawing.Size(731, 379);
            this.MasterTemplate.TabIndex = 50;
            this.MasterTemplate.ThemeName = "TelerikMetroTouch";
            // 
            // btnDevolver
            // 
            this.btnDevolver.Location = new System.Drawing.Point(220, 492);
            this.btnDevolver.Name = "btnDevolver";
            this.btnDevolver.Size = new System.Drawing.Size(151, 52);
            this.btnDevolver.TabIndex = 51;
            this.btnDevolver.Text = "Devolver Dinero";
            this.btnDevolver.ThemeName = "TelerikMetroTouch";
            this.btnDevolver.Click += new System.EventHandler(this.btnDevolver_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(476, 27);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(145, 25);
            this.lblTotal.TabIndex = 54;
            this.lblTotal.Text = "TOTAL Motorizado";
            // 
            // lblTotalMotorizado
            // 
            this.lblTotalMotorizado.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalMotorizado.Location = new System.Drawing.Point(626, 27);
            this.lblTotalMotorizado.Name = "lblTotalMotorizado";
            this.lblTotalMotorizado.Size = new System.Drawing.Size(30, 25);
            this.lblTotalMotorizado.TabIndex = 55;
            this.lblTotalMotorizado.Text = "$ 0";
            // 
            // txtCodigoMotorizado
            // 
            this.txtCodigoMotorizado.AutoSize = false;
            this.txtCodigoMotorizado.BackColor = System.Drawing.SystemColors.Window;
            this.txtCodigoMotorizado.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigoMotorizado.Location = new System.Drawing.Point(146, 25);
            this.txtCodigoMotorizado.Name = "txtCodigoMotorizado";
            this.txtCodigoMotorizado.Size = new System.Drawing.Size(194, 38);
            this.txtCodigoMotorizado.TabIndex = 56;
            this.txtCodigoMotorizado.TabStop = false;
            this.txtCodigoMotorizado.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCodigoMotorizado.ThemeName = "TelerikMetroTouch";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Image = global::POS.Properties.Resources._24_search;
            this.btnBuscar.Location = new System.Drawing.Point(346, 21);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(46, 46);
            this.btnBuscar.TabIndex = 57;
            this.btnBuscar.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBuscar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBuscar.ThemeName = "TelerikMetroTouch";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // FrmDevolucionFlete
            // 
            this.ClientSize = new System.Drawing.Size(776, 622);
            this.ControlBox = false;
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.txtCodigoMotorizado);
            this.Controls.Add(this.lblTotalMotorizado);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnDevolver);
            this.Controls.Add(this.MasterTemplate);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.radLabel5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmDevolucionFlete";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Devolución Fletes Pedidos a Domicilio";
            this.ThemeName = "TelerikMetroTouch";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmDevolucionFlete_FormClosing);
            this.Load += new System.EventHandler(this.FrmDevolucionFlete_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MasterTemplate.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDevolver)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalMotorizado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoMotorizado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBuscar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private System.Windows.Forms.Panel panel3;
        private Telerik.WinControls.UI.RadButton btnKbd;
        private Telerik.WinControls.UI.RadGridView gridItems;
        private Telerik.WinControls.UI.RadButton btnDevolver;
        private Telerik.WinControls.UI.RadGridView MasterTemplate;
        private Telerik.WinControls.UI.RadLabel lblTotal;
        private Telerik.WinControls.UI.RadLabel lblTotalMotorizado;
        private Telerik.WinControls.UI.RadTextBox txtCodigoMotorizado;
        private Telerik.WinControls.UI.RadButton btnBuscar;
    }
}