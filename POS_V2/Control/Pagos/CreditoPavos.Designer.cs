namespace POS.Control.Pagos
{
    partial class CreditoPavos
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreditoPavos));
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblIdentificacion = new System.Windows.Forms.Label();
            this.dgvConsultaSaldo = new System.Windows.Forms.DataGridView();
            this.Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Monto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Saldo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Local = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DETALLE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblCantidadAbonar = new System.Windows.Forms.Label();
            this.btnAbonar = new System.Windows.Forms.Button();
            this.lblNombreCliente = new System.Windows.Forms.Label();
            this.lblSaldoLbl = new System.Windows.Forms.Label();
            this.lblSaldo = new System.Windows.Forms.Label();
            this.lblFormaPago = new System.Windows.Forms.Label();
            this.cmbFormaPago = new System.Windows.Forms.ComboBox();
            this.cmbEleccion = new System.Windows.Forms.ComboBox();
            this.lblEleccion = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblTipoPagoTarjeta = new System.Windows.Forms.Label();
            this.cmbTipoPagoTarjeta = new System.Windows.Forms.ComboBox();
            this.pnl1 = new Telerik.WinControls.UI.RadPanel();
            this.txtCuenta = new Telerik.WinControls.UI.RadTextBox();
            this.lblNum = new Telerik.WinControls.UI.RadLabel();
            this.lblCuenta = new Telerik.WinControls.UI.RadLabel();
            this.txtNumCheque = new Telerik.WinControls.UI.RadTextBox();
            this.btnKbd = new Telerik.WinControls.UI.RadButton();
            this.txtIdentificacion = new Telerik.WinControls.UI.RadTextBox();
            this.txtCantidadAbonar = new Telerik.WinControls.UI.RadTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPagoManual = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsultaSaldo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnl1)).BeginInit();
            this.pnl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCuenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCuenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumCheque)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIdentificacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCantidadAbonar)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnPagoManual)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(323, 39);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(65, 29);
            this.btnBuscar.TabIndex = 0;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // lblIdentificacion
            // 
            this.lblIdentificacion.AutoSize = true;
            this.lblIdentificacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdentificacion.Location = new System.Drawing.Point(29, 41);
            this.lblIdentificacion.Name = "lblIdentificacion";
            this.lblIdentificacion.Size = new System.Drawing.Size(124, 24);
            this.lblIdentificacion.TabIndex = 1;
            this.lblIdentificacion.Text = "Identificación:";
            // 
            // dgvConsultaSaldo
            // 
            this.dgvConsultaSaldo.AllowUserToAddRows = false;
            this.dgvConsultaSaldo.AllowUserToDeleteRows = false;
            this.dgvConsultaSaldo.AllowUserToOrderColumns = true;
            this.dgvConsultaSaldo.AllowUserToResizeRows = false;
            this.dgvConsultaSaldo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConsultaSaldo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Fecha,
            this.Monto,
            this.Saldo,
            this.Local,
            this.DETALLE});
            this.dgvConsultaSaldo.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvConsultaSaldo.Location = new System.Drawing.Point(12, 129);
            this.dgvConsultaSaldo.MultiSelect = false;
            this.dgvConsultaSaldo.Name = "dgvConsultaSaldo";
            this.dgvConsultaSaldo.RowHeadersVisible = false;
            this.dgvConsultaSaldo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConsultaSaldo.Size = new System.Drawing.Size(890, 150);
            this.dgvConsultaSaldo.TabIndex = 3;
            // 
            // Fecha
            // 
            this.Fecha.DataPropertyName = "FECHA";
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fecha.DefaultCellStyle = dataGridViewCellStyle1;
            this.Fecha.HeaderText = "Fecha";
            this.Fecha.Name = "Fecha";
            this.Fecha.Width = 125;
            // 
            // Monto
            // 
            this.Monto.DataPropertyName = "MONTO";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.Format = "N2";
            dataGridViewCellStyle2.NullValue = null;
            this.Monto.DefaultCellStyle = dataGridViewCellStyle2;
            this.Monto.HeaderText = "Monto";
            this.Monto.Name = "Monto";
            this.Monto.Width = 110;
            // 
            // Saldo
            // 
            this.Saldo.DataPropertyName = "SALDO";
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = null;
            this.Saldo.DefaultCellStyle = dataGridViewCellStyle3;
            this.Saldo.HeaderText = "Saldo";
            this.Saldo.Name = "Saldo";
            this.Saldo.Width = 110;
            // 
            // Local
            // 
            this.Local.DataPropertyName = "NOMBRELOCAL";
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Local.DefaultCellStyle = dataGridViewCellStyle4;
            this.Local.HeaderText = "Local";
            this.Local.Name = "Local";
            this.Local.Width = 180;
            // 
            // DETALLE
            // 
            this.DETALLE.DataPropertyName = "DETALLE";
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DETALLE.DefaultCellStyle = dataGridViewCellStyle5;
            this.DETALLE.HeaderText = "Detalle";
            this.DETALLE.Name = "DETALLE";
            this.DETALLE.ReadOnly = true;
            this.DETALLE.Width = 180;
            // 
            // lblCantidadAbonar
            // 
            this.lblCantidadAbonar.AutoSize = true;
            this.lblCantidadAbonar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantidadAbonar.Location = new System.Drawing.Point(153, 385);
            this.lblCantidadAbonar.Name = "lblCantidadAbonar";
            this.lblCantidadAbonar.Size = new System.Drawing.Size(168, 24);
            this.lblCantidadAbonar.TabIndex = 4;
            this.lblCantidadAbonar.Text = "Cantidad a abonar:";
            // 
            // btnAbonar
            // 
            this.btnAbonar.Location = new System.Drawing.Point(327, 419);
            this.btnAbonar.Name = "btnAbonar";
            this.btnAbonar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnAbonar.Size = new System.Drawing.Size(75, 35);
            this.btnAbonar.TabIndex = 6;
            this.btnAbonar.Text = "Abonar";
            this.btnAbonar.UseVisualStyleBackColor = true;
            this.btnAbonar.Click += new System.EventHandler(this.btnAbonar_Click);
            // 
            // lblNombreCliente
            // 
            this.lblNombreCliente.AutoSize = true;
            this.lblNombreCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreCliente.Location = new System.Drawing.Point(29, 83);
            this.lblNombreCliente.Name = "lblNombreCliente";
            this.lblNombreCliente.Size = new System.Drawing.Size(0, 26);
            this.lblNombreCliente.TabIndex = 7;
            // 
            // lblSaldoLbl
            // 
            this.lblSaldoLbl.AutoSize = true;
            this.lblSaldoLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldoLbl.Location = new System.Drawing.Point(152, 295);
            this.lblSaldoLbl.Name = "lblSaldoLbl";
            this.lblSaldoLbl.Size = new System.Drawing.Size(63, 24);
            this.lblSaldoLbl.TabIndex = 9;
            this.lblSaldoLbl.Text = "Saldo:";
            // 
            // lblSaldo
            // 
            this.lblSaldo.AutoSize = true;
            this.lblSaldo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldo.Location = new System.Drawing.Point(223, 290);
            this.lblSaldo.Name = "lblSaldo";
            this.lblSaldo.Size = new System.Drawing.Size(0, 29);
            this.lblSaldo.TabIndex = 10;
            // 
            // lblFormaPago
            // 
            this.lblFormaPago.AutoSize = true;
            this.lblFormaPago.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormaPago.Location = new System.Drawing.Point(153, 340);
            this.lblFormaPago.Name = "lblFormaPago";
            this.lblFormaPago.Size = new System.Drawing.Size(146, 24);
            this.lblFormaPago.TabIndex = 11;
            this.lblFormaPago.Text = "Forma de Pago:";
            // 
            // cmbFormaPago
            // 
            this.cmbFormaPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormaPago.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbFormaPago.FormattingEnabled = true;
            this.cmbFormaPago.Items.AddRange(new object[] {
            "Efectivo",
            "Tarjeta Crédito"});
            this.cmbFormaPago.Location = new System.Drawing.Point(327, 337);
            this.cmbFormaPago.Name = "cmbFormaPago";
            this.cmbFormaPago.Size = new System.Drawing.Size(178, 32);
            this.cmbFormaPago.TabIndex = 12;
            this.cmbFormaPago.SelectedIndexChanged += new System.EventHandler(this.cmbFormaPago_SelectedIndexChanged);
            // 
            // cmbEleccion
            // 
            this.cmbEleccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEleccion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEleccion.FormattingEnabled = true;
            this.cmbEleccion.Location = new System.Drawing.Point(517, 337);
            this.cmbEleccion.Name = "cmbEleccion";
            this.cmbEleccion.Size = new System.Drawing.Size(230, 32);
            this.cmbEleccion.TabIndex = 13;
            this.cmbEleccion.SelectedIndexChanged += new System.EventHandler(this.cmbEleccion_SelectedIndexChanged);
            // 
            // lblEleccion
            // 
            this.lblEleccion.AutoSize = true;
            this.lblEleccion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEleccion.Location = new System.Drawing.Point(517, 316);
            this.lblEleccion.Name = "lblEleccion";
            this.lblEleccion.Size = new System.Drawing.Size(0, 20);
            this.lblEleccion.TabIndex = 14;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(767, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(135, 118);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // lblTipoPagoTarjeta
            // 
            this.lblTipoPagoTarjeta.AutoSize = true;
            this.lblTipoPagoTarjeta.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoPagoTarjeta.Location = new System.Drawing.Point(561, 378);
            this.lblTipoPagoTarjeta.Name = "lblTipoPagoTarjeta";
            this.lblTipoPagoTarjeta.Size = new System.Drawing.Size(58, 24);
            this.lblTipoPagoTarjeta.TabIndex = 16;
            this.lblTipoPagoTarjeta.Text = "Tipo:";
            this.lblTipoPagoTarjeta.Visible = false;
            // 
            // cmbTipoPagoTarjeta
            // 
            this.cmbTipoPagoTarjeta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoPagoTarjeta.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTipoPagoTarjeta.FormattingEnabled = true;
            this.cmbTipoPagoTarjeta.Items.AddRange(new object[] {
            "Datafast",
            "Medianet"});
            this.cmbTipoPagoTarjeta.Location = new System.Drawing.Point(626, 374);
            this.cmbTipoPagoTarjeta.Name = "cmbTipoPagoTarjeta";
            this.cmbTipoPagoTarjeta.Size = new System.Drawing.Size(121, 32);
            this.cmbTipoPagoTarjeta.TabIndex = 15;
            this.cmbTipoPagoTarjeta.Visible = false;
            // 
            // pnl1
            // 
            this.pnl1.Controls.Add(this.txtCuenta);
            this.pnl1.Controls.Add(this.lblNum);
            this.pnl1.Controls.Add(this.lblCuenta);
            this.pnl1.Controls.Add(this.txtNumCheque);
            this.pnl1.Location = new System.Drawing.Point(463, 374);
            this.pnl1.Name = "pnl1";
            this.pnl1.Size = new System.Drawing.Size(311, 105);
            this.pnl1.TabIndex = 17;
            this.pnl1.Visible = false;
            // 
            // txtCuenta
            // 
            this.txtCuenta.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCuenta.Location = new System.Drawing.Point(105, 7);
            this.txtCuenta.Name = "txtCuenta";
            this.txtCuenta.NullText = "0101010101";
            this.txtCuenta.Size = new System.Drawing.Size(202, 46);
            this.txtCuenta.TabIndex = 19;
            this.txtCuenta.TabStop = false;
            this.txtCuenta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCuenta.ThemeName = "TelerikMetroTouch";
            // 
            // lblNum
            // 
            this.lblNum.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNum.Location = new System.Drawing.Point(3, 59);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(90, 30);
            this.lblNum.TabIndex = 21;
            this.lblNum.Text = "Número:";
            this.lblNum.ThemeName = "TelerikMetroTouch";
            // 
            // lblCuenta
            // 
            this.lblCuenta.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCuenta.Location = new System.Drawing.Point(3, 7);
            this.lblCuenta.Name = "lblCuenta";
            this.lblCuenta.Size = new System.Drawing.Size(79, 30);
            this.lblCuenta.TabIndex = 18;
            this.lblCuenta.Text = "Cuenta:";
            this.lblCuenta.ThemeName = "TelerikMetroTouch";
            // 
            // txtNumCheque
            // 
            this.txtNumCheque.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumCheque.Location = new System.Drawing.Point(105, 54);
            this.txtNumCheque.Name = "txtNumCheque";
            this.txtNumCheque.NullText = "001";
            this.txtNumCheque.Size = new System.Drawing.Size(202, 46);
            this.txtNumCheque.TabIndex = 20;
            this.txtNumCheque.TabStop = false;
            this.txtNumCheque.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNumCheque.ThemeName = "TelerikMetroTouch";
            // 
            // btnKbd
            // 
            this.btnKbd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnKbd.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnKbd.Location = new System.Drawing.Point(12, 16);
            this.btnKbd.MaximumSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Name = "btnKbd";
            // 
            // 
            // 
            this.btnKbd.RootElement.ControlBounds = new System.Drawing.Rectangle(12, 16, 110, 24);
            this.btnKbd.RootElement.MaxSize = new System.Drawing.Size(120, 50);
            this.btnKbd.Size = new System.Drawing.Size(77, 32);
            this.btnKbd.TabIndex = 18;
            this.btnKbd.Text = "Teclado";
            this.btnKbd.ThemeName = "TelerikMetroTouch";
            this.btnKbd.Visible = false;
            this.btnKbd.Click += new System.EventHandler(this.btnKbd_Click);
            // 
            // txtIdentificacion
            // 
            this.txtIdentificacion.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtIdentificacion.Location = new System.Drawing.Point(156, 38);
            this.txtIdentificacion.Name = "txtIdentificacion";
            this.txtIdentificacion.NullText = "Identificacion";
            // 
            // 
            // 
            this.txtIdentificacion.RootElement.ControlBounds = new System.Drawing.Rectangle(156, 38, 100, 20);
            this.txtIdentificacion.RootElement.StretchVertically = true;
            this.txtIdentificacion.Size = new System.Drawing.Size(161, 30);
            this.txtIdentificacion.TabIndex = 19;
            this.txtIdentificacion.TabStop = false;
            this.txtIdentificacion.ThemeName = "TelerikMetroTouch";
            this.txtIdentificacion.TextChanged += new System.EventHandler(this.txtIdentificacion_TextChanged);
            this.txtIdentificacion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIdentificacion_KeyDown);
            this.txtIdentificacion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIdentificacion_KeyPress);
            this.txtIdentificacion.Leave += new System.EventHandler(this.txtIdentificacion_Leave);
            // 
            // txtCantidadAbonar
            // 
            this.txtCantidadAbonar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtCantidadAbonar.Location = new System.Drawing.Point(327, 381);
            this.txtCantidadAbonar.Name = "txtCantidadAbonar";
            this.txtCantidadAbonar.NullText = "Valor";
            // 
            // 
            // 
            this.txtCantidadAbonar.RootElement.ControlBounds = new System.Drawing.Rectangle(327, 381, 100, 20);
            this.txtCantidadAbonar.RootElement.StretchVertically = true;
            this.txtCantidadAbonar.Size = new System.Drawing.Size(97, 30);
            this.txtCantidadAbonar.TabIndex = 20;
            this.txtCantidadAbonar.TabStop = false;
            this.txtCantidadAbonar.ThemeName = "TelerikMetroTouch";
            this.txtCantidadAbonar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCantidadAbonar_KeyDown);
            this.txtCantidadAbonar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCantidadAbonar_KeyPress);
            this.txtCantidadAbonar.Leave += new System.EventHandler(this.txtIdentificacion_Leave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnKbd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 484);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(914, 60);
            this.panel1.TabIndex = 21;
            // 
            // btnPagoManual
            // 
            this.btnPagoManual.BackColor = System.Drawing.Color.Red;
            this.btnPagoManual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.btnPagoManual.Location = new System.Drawing.Point(440, 378);
            this.btnPagoManual.Name = "btnPagoManual";
            this.btnPagoManual.Size = new System.Drawing.Size(102, 35);
            this.btnPagoManual.TabIndex = 25;
            this.btnPagoManual.Text = "Pago Manual";
            this.btnPagoManual.ThemeName = "TelerikMetroTouch";
            this.btnPagoManual.Visible = false;
            this.btnPagoManual.Click += new System.EventHandler(this.btnPagoManual_Click);
            // 
            // CreditoPavos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(914, 544);
            this.Controls.Add(this.btnPagoManual);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtCantidadAbonar);
            this.Controls.Add(this.txtIdentificacion);
            this.Controls.Add(this.lblTipoPagoTarjeta);
            this.Controls.Add(this.cmbTipoPagoTarjeta);
            this.Controls.Add(this.lblEleccion);
            this.Controls.Add(this.cmbEleccion);
            this.Controls.Add(this.cmbFormaPago);
            this.Controls.Add(this.lblFormaPago);
            this.Controls.Add(this.lblSaldo);
            this.Controls.Add(this.lblSaldoLbl);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblNombreCliente);
            this.Controls.Add(this.btnAbonar);
            this.Controls.Add(this.lblCantidadAbonar);
            this.Controls.Add(this.dgvConsultaSaldo);
            this.Controls.Add(this.lblIdentificacion);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.pnl1);
            this.Name = "CreditoPavos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PaviPLAN";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsultaSaldo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnl1)).EndInit();
            this.pnl1.ResumeLayout(false);
            this.pnl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCuenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCuenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumCheque)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnKbd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIdentificacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCantidadAbonar)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnPagoManual)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblIdentificacion;
        private System.Windows.Forms.DataGridView dgvConsultaSaldo;
        private System.Windows.Forms.Label lblCantidadAbonar;
        private System.Windows.Forms.Button btnAbonar;
        private System.Windows.Forms.Label lblNombreCliente;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblSaldoLbl;
        private System.Windows.Forms.Label lblSaldo;
        private System.Windows.Forms.Label lblFormaPago;
        private System.Windows.Forms.ComboBox cmbFormaPago;
        private System.Windows.Forms.ComboBox cmbEleccion;
        private System.Windows.Forms.Label lblEleccion;
        private System.Windows.Forms.Label lblTipoPagoTarjeta;
        private System.Windows.Forms.ComboBox cmbTipoPagoTarjeta;
        private Telerik.WinControls.UI.RadPanel pnl1;
        private Telerik.WinControls.UI.RadTextBox txtCuenta;
        private Telerik.WinControls.UI.RadLabel lblNum;
        private Telerik.WinControls.UI.RadLabel lblCuenta;
        private Telerik.WinControls.UI.RadTextBox txtNumCheque;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fecha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Monto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Saldo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Local;
        private System.Windows.Forms.DataGridViewTextBoxColumn DETALLE;
        private Telerik.WinControls.UI.RadButton btnKbd;
        private Telerik.WinControls.UI.RadTextBox txtIdentificacion;
        private Telerik.WinControls.UI.RadTextBox txtCantidadAbonar;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadButton btnPagoManual;
    }
}