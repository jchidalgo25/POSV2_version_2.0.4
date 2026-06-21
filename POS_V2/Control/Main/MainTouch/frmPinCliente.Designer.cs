namespace POS.Control.Main.MainTouch
{
    partial class frmPinCliente
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
            this.txtNoPIN = new Telerik.WinControls.UI.RadTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableNumPad = new System.Windows.Forms.TableLayoutPanel();
            this.btn0 = new Telerik.WinControls.UI.RadButton();
            this.btn9 = new Telerik.WinControls.UI.RadButton();
            this.btn8 = new Telerik.WinControls.UI.RadButton();
            this.btn6 = new Telerik.WinControls.UI.RadButton();
            this.btn5 = new Telerik.WinControls.UI.RadButton();
            this.btn4 = new Telerik.WinControls.UI.RadButton();
            this.btn3 = new Telerik.WinControls.UI.RadButton();
            this.btn2 = new Telerik.WinControls.UI.RadButton();
            this.btn1 = new Telerik.WinControls.UI.RadButton();
            this.btnBorrar = new Telerik.WinControls.UI.RadButton();
            this.btn7 = new Telerik.WinControls.UI.RadButton();
            this.btnEnter = new Telerik.WinControls.UI.RadButton();
            this.btnCancelar = new Telerik.WinControls.UI.RadButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.txtNoPIN)).BeginInit();
            this.panel2.SuspendLayout();
            this.tableNumPad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBorrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelar)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNoPIN
            // 
            this.txtNoPIN.BackColor = System.Drawing.SystemColors.Control;
            this.txtNoPIN.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNoPIN.Location = new System.Drawing.Point(3, 15);
            this.txtNoPIN.MaxLength = 60;
            this.txtNoPIN.Name = "txtNoPIN";
            this.txtNoPIN.PasswordChar = '*';
            this.txtNoPIN.Size = new System.Drawing.Size(409, 46);
            this.txtNoPIN.TabIndex = 27;
            this.txtNoPIN.TabStop = false;
            this.txtNoPIN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNoPIN.ThemeName = "TelerikMetroTouch";
            this.txtNoPIN.TextChanged += new System.EventHandler(this.txtNoPIN_TextChanged);
            this.txtNoPIN.Leave += new System.EventHandler(this.txtNoPIN_Leave);

            ((Telerik.WinControls.UI.RadTextBoxItem)(this.txtNoPIN.GetChildAt(0).GetChildAt(0))).BackColor = System.Drawing.SystemColors.Control;
            ((Telerik.WinControls.UI.RadTextBoxItem)(this.txtNoPIN.GetChildAt(0).GetChildAt(0))).Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.FillPrimitive)(this.txtNoPIN.GetChildAt(0).GetChildAt(1))).BackColor = System.Drawing.SystemColors.Control;
            ((Telerik.WinControls.Primitives.BorderPrimitive)(this.txtNoPIN.GetChildAt(0).GetChildAt(2))).BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableNumPad);
            this.panel2.Location = new System.Drawing.Point(14, 67);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(379, 271);
            this.panel2.TabIndex = 1;
            // 
            // tableNumPad
            // 
            this.tableNumPad.ColumnCount = 4;
            this.tableNumPad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableNumPad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableNumPad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableNumPad.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
            this.tableNumPad.Controls.Add(this.btn0, 1, 3);
            this.tableNumPad.Controls.Add(this.btn9, 2, 2);
            this.tableNumPad.Controls.Add(this.btn8, 1, 2);
            this.tableNumPad.Controls.Add(this.btn6, 2, 1);
            this.tableNumPad.Controls.Add(this.btn5, 1, 1);
            this.tableNumPad.Controls.Add(this.btn4, 0, 1);
            this.tableNumPad.Controls.Add(this.btn3, 2, 0);
            this.tableNumPad.Controls.Add(this.btn2, 1, 0);
            this.tableNumPad.Controls.Add(this.btn1, 0, 0);
            this.tableNumPad.Controls.Add(this.btnBorrar, 3, 0);
            this.tableNumPad.Controls.Add(this.btn7, 0, 2);
            this.tableNumPad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableNumPad.Location = new System.Drawing.Point(0, 0);
            this.tableNumPad.Name = "tableNumPad";
            this.tableNumPad.RowCount = 4;
            this.tableNumPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 61F));
            this.tableNumPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableNumPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableNumPad.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableNumPad.Size = new System.Drawing.Size(379, 271);
            this.tableNumPad.TabIndex = 36;
            // 
            // btn0
            // 
            this.btn0.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn0.Location = new System.Drawing.Point(98, 204);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(90, 64);
            this.btn0.TabIndex = 36;
            this.btn0.Text = "0";
            this.btn0.ThemeName = "TelerikMetroTouch";
            this.btn0.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn0.GetChildAt(0))).Text = "0";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn0.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn0.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn9
            // 
            this.btn9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn9.Location = new System.Drawing.Point(194, 134);
            this.btn9.Name = "btn9";
            this.btn9.Size = new System.Drawing.Size(90, 64);
            this.btn9.TabIndex = 36;
            this.btn9.Text = "9";
            this.btn9.ThemeName = "TelerikMetroTouch";
            this.btn9.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn9.GetChildAt(0))).Text = "9";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn9.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn9.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn8
            // 
            this.btn8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn8.Location = new System.Drawing.Point(98, 134);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(90, 64);
            this.btn8.TabIndex = 36;
            this.btn8.Text = "8";
            this.btn8.ThemeName = "TelerikMetroTouch";
            this.btn8.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn8.GetChildAt(0))).Text = "8";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn8.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn8.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn6
            // 
            this.btn6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn6.Location = new System.Drawing.Point(194, 64);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(90, 64);
            this.btn6.TabIndex = 36;
            this.btn6.Text = "6";
            this.btn6.ThemeName = "TelerikMetroTouch";
            this.btn6.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn6.GetChildAt(0))).Text = "6";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn6.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn6.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn5
            // 
            this.btn5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn5.Location = new System.Drawing.Point(98, 64);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(90, 64);
            this.btn5.TabIndex = 35;
            this.btn5.Text = "5";
            this.btn5.ThemeName = "TelerikMetroTouch";
            this.btn5.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn5.GetChildAt(0))).Text = "5";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn5.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn5.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn4
            // 
            this.btn4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn4.Location = new System.Drawing.Point(3, 64);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(89, 64);
            this.btn4.TabIndex = 34;
            this.btn4.Text = "4";
            this.btn4.ThemeName = "TelerikMetroTouch";
            this.btn4.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn4.GetChildAt(0))).Text = "4";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn4.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn4.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn3
            // 
            this.btn3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn3.Location = new System.Drawing.Point(194, 3);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(90, 55);
            this.btn3.TabIndex = 33;
            this.btn3.Text = "3";
            this.btn3.ThemeName = "TelerikMetroTouch";
            this.btn3.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn3.GetChildAt(0))).Text = "3";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn3.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn3.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn2
            // 
            this.btn2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn2.Location = new System.Drawing.Point(98, 3);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(90, 55);
            this.btn2.TabIndex = 32;
            this.btn2.Text = "2";
            this.btn2.ThemeName = "TelerikMetroTouch";
            this.btn2.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn2.GetChildAt(0))).Text = "2";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn2.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn2.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn1
            // 
            this.btn1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn1.Location = new System.Drawing.Point(3, 3);
            this.btn1.Name = "btn1";
            // 
            // 
            // 
            this.btn1.RootElement.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn1.Size = new System.Drawing.Size(89, 55);
            this.btn1.TabIndex = 31;
            this.btn1.Text = "1";
            this.btn1.ThemeName = "TelerikMetroTouch";
            this.btn1.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn1.GetChildAt(0))).Text = "1";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn1.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn1.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBorrar
            // 
            this.btnBorrar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBorrar.Location = new System.Drawing.Point(290, 3);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(86, 55);
            this.btnBorrar.TabIndex = 30;
            this.btnBorrar.Text = "Borrar";
            this.btnBorrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBorrar.ThemeName = "TelerikMetroTouch";
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // btn7
            // 
            this.btn7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn7.Location = new System.Drawing.Point(3, 134);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(89, 64);
            this.btn7.TabIndex = 35;
            this.btn7.Text = "7";
            this.btn7.ThemeName = "TelerikMetroTouch";
            this.btn7.Click += new System.EventHandler(this.btnEvent);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btn7.GetChildAt(0))).Text = "7";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn7.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btn7.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnEnter
            // 
            this.btnEnter.DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            this.btnEnter.Location = new System.Drawing.Point(264, 352);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(132, 57);
            this.btnEnter.TabIndex = 31;
            this.btnEnter.Text = "Continuar";
            this.btnEnter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEnter.ThemeName = "TelerikMetroTouch";
            this.btnEnter.UseMnemonic = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            ((Telerik.WinControls.UI.RadButtonElement)(this.btnEnter.GetChildAt(0))).TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            ((Telerik.WinControls.UI.RadButtonElement)(this.btnEnter.GetChildAt(0))).DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            ((Telerik.WinControls.UI.RadButtonElement)(this.btnEnter.GetChildAt(0))).Text = "Continuar";
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btnEnter.GetChildAt(0).GetChildAt(1).GetChildAt(1))).UseMnemonic = false;
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btnEnter.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ((Telerik.WinControls.Primitives.TextPrimitive)(this.btnEnter.GetChildAt(0).GetChildAt(1).GetChildAt(1))).Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(31, 353);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(135, 55);
            this.btnCancelar.TabIndex = 31;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelar.ThemeName = "TelerikMetroTouch";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.btnEnter);
            this.panel1.Controls.Add(this.btnCancelar);
            this.panel1.Location = new System.Drawing.Point(5, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(426, 429);
            this.panel1.TabIndex = 28;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.txtNoPIN);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Location = new System.Drawing.Point(6, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(415, 341);
            this.panel3.TabIndex = 1;
            // 
            // frmPinCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(440, 447);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPinCliente";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "frmPinCliente";
            ((System.ComponentModel.ISupportInitialize)(this.txtNoPIN)).EndInit();
            this.panel2.ResumeLayout(false);
            this.tableNumPad.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btn0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBorrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtNoPIN;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableNumPad;
        private Telerik.WinControls.UI.RadButton btn0;
        private Telerik.WinControls.UI.RadButton btn9;
        private Telerik.WinControls.UI.RadButton btn8;
        private Telerik.WinControls.UI.RadButton btn6;
        private Telerik.WinControls.UI.RadButton btn5;
        private Telerik.WinControls.UI.RadButton btn4;
        private Telerik.WinControls.UI.RadButton btn3;
        private Telerik.WinControls.UI.RadButton btn2;
        private Telerik.WinControls.UI.RadButton btn1;
        private Telerik.WinControls.UI.RadButton btn7;
        private Telerik.WinControls.UI.RadButton btnEnter;
        private Telerik.WinControls.UI.RadButton btnBorrar;
        private Telerik.WinControls.UI.RadButton btnCancelar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
    }
}