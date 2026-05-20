using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace POS.Control.ToolBox
{
    public partial class frmTecladoCompleto : Form
    {
        private TextBox textBoxActivo;
        private RadTextBox radtextBoxActivo;
        private TableLayoutPanel panelTeclado;
        private bool mayusculas = false;

        // Constructor por defecto
        public frmTecladoCompleto()
        {
        }

        // Constructor principal
        public frmTecladoCompleto(TextBox referenciaTextBox)
        {
            textBoxActivo = referenciaTextBox;
            InitializeComponent1();
            CrearBotones();
            CenterToScreen();
        }

        public frmTecladoCompleto(RadTextBox referenciaRadTextBox)
        {
            radtextBoxActivo = referenciaRadTextBox;
            // Extraer el TextBox interno de RadTextBox
            if (radtextBoxActivo?.TextBoxElement?.TextBoxItem?.HostedControl is TextBox txt)
            {
                textBoxActivo = txt;
            }
            InitializeComponent1();
            CrearBotones();
            CenterToScreen();
        }

        public frmTecladoCompleto(TextBox referenciaTextBox, Screen pantalla)
        {
            textBoxActivo = referenciaTextBox;
            InitializeComponent1();
            CrearBotones();

            this.StartPosition = FormStartPosition.Manual;
            this.Load += (s, e) =>
            {
                // Posicionar después de que el form tenga tamaño real
                int x = pantalla.WorkingArea.Left + (pantalla.WorkingArea.Width - this.Width) / 2;
                int y = pantalla.WorkingArea.Bottom - this.Height - 10;
                this.Location = new Point(x, y);
            };
        }

        void InitializeComponent1()
        {
            this.Text = "Teclado Alfanumérico";
            this.Size = new Size(1000, 500); // Ajuste de altura
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            panelTeclado = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(45, 45, 45)
            };

            panelTeclado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66.6F)); // Izquierda (alfabético)
            panelTeclado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F)); // Derecha (numérico)

            this.Controls.Add(panelTeclado);
        }

        void CrearBotones()
        {
            // PANEL IZQUIERDO - TECLADO ALFABÉTICO
            TableLayoutPanel panelAlfabetico = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 15,
                RowCount = 5,
                Padding = new Padding(5),
                BackColor = Color.FromArgb(45, 45, 45)
            };

            for (int i = 0; i < 15; i++)
                panelAlfabetico.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            for (int i = 0; i < 5; i++)
                panelAlfabetico.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 5));

            string[] filasAlfabetico = new string[]
            {
                "Esc|Mayús|@|,|.",
                "q|w|e|r|t|y|u|i|o|p",
                "a|s|d|f|g|h|j|k|l|ñ",
                "z|x|c|v|b|n|m"
            };

            int filaIndex = 0;
            foreach (string fila in filasAlfabetico)
            {
                string[] teclas = fila.Split('|');
                int colIndex = 0;
                foreach (string tecla in teclas)
                {
                    Button btn = CrearBoton(tecla);
                    btn.Click += (s, e) =>
                    {
                        if (textBoxActivo != null)
                        {
                            char c = tecla.Length > 0 ? tecla[0] : '\0';
                            if (char.IsLetter(c))
                                c = mayusculas ? char.ToUpper(c) : char.ToLower(c);

                            textBoxActivo.Text += c.ToString();
                            textBoxActivo.SelectionStart = textBoxActivo.Text.Length;
                        }
                    };
                    panelAlfabetico.Controls.Add(btn, colIndex++, filaIndex);
                }
                filaIndex++;
            }

            // Botón MAYÚS
            Button btnMayus = CrearBoton("MAYÚS");
            btnMayus.BackColor = Color.Gold;
            btnMayus.Click += (s, e) =>
            {
                mayusculas = !mayusculas;
                ActualizarLetras(panelAlfabetico, mayusculas);
            };
            panelAlfabetico.Controls.Add(btnMayus, 0, panelAlfabetico.RowCount++);
            panelAlfabetico.SetColumnSpan(btnMayus, 15);

            // Agregar panel alfabético al layout principal
            panelTeclado.Controls.Add(panelAlfabetico, 0, 0);


            // PANEL DERECHO - TECLADO NUMÉRICO
            TableLayoutPanel panelNumerico = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 5,
                Padding = new Padding(5),
                BackColor = Color.FromArgb(45, 45, 45)
            };

            for (int i = 0; i < 3; i++)
                panelNumerico.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            for (int i = 0; i < 5; i++)
                panelNumerico.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 5));

            string[] numeros = { "7", "8", "9", "4", "5", "6", "1", "2", "3", "0" };
            int index = 0;
            foreach (var numero in numeros)
            {
                Button btn = CrearBoton(numero);
                btn.Click += (s, e) =>
                {
                    if (textBoxActivo != null)
                    {
                        textBoxActivo.Text += btn.Text;
                        textBoxActivo.SelectionStart = textBoxActivo.Text.Length;
                    }
                };
                panelNumerico.Controls.Add(btn, index % 3, index / 3);
                index++;
            }

            // Botón Borrar
            Button btnBorrar = CrearBoton("⌫");
            btnBorrar.BackColor = Color.Red;
            btnBorrar.Click += (s, e) =>
            {
                if (textBoxActivo != null && textBoxActivo.Text.Length > 0)
                {
                    textBoxActivo.Text = textBoxActivo.Text.Substring(0, textBoxActivo.Text.Length - 1);
                    textBoxActivo.SelectionStart = textBoxActivo.Text.Length;
                }
            };
            panelNumerico.Controls.Add(btnBorrar, 2, 3);

            // Botón Espacio
            Button btnEspacio = CrearBoton("Espacio");
            panelNumerico.Controls.Add(btnEspacio, 0, 4);
            panelNumerico.SetColumnSpan(btnEspacio, 2);
            btnEspacio.Click += (s, e) =>
            {
                if (textBoxActivo != null)
                {
                    textBoxActivo.Text += " ";
                    textBoxActivo.SelectionStart = textBoxActivo.Text.Length;
                }
            };

            // Botón Aceptar
            Button btnCerrar = CrearBoton("Aceptar");
            btnCerrar.BackColor = Color.Green;
            panelNumerico.Controls.Add(btnCerrar, 2, 4);
            btnCerrar.Click += (s, e) => this.DialogResult = DialogResult.OK;

            // Agregar panel numérico al layout principal
            panelTeclado.Controls.Add(panelNumerico, 1, 0);
        }

        private Button CrearBoton(string texto)
        {
            return new Button
            {
                Text = texto,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                Font = new Font("Segoe UI", 14F, FontStyle.Regular),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(100, 100, 100) },
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private void ActualizarLetras(TableLayoutPanel panel, bool estadoMayusculas)
        {
            //foreach (System.Windows.Forms.Control control in panel.Controls)
            //{
            //    if (control is Button btn && btn.Text.Length == 1 && btn.Text != "MAYÚS")
            //    {
            //        char c = btn.Text[0];
            //        if (c >= 'a' && c <= 'z')
            //            btn.Text = estadoMayusculas ? char.ToUpper(c).ToString() : char.ToLower(c).ToString();
            //    }
            //}

            foreach (System.Windows.Forms.Control control in panel.Controls)
            {
                if (control is Button btn && btn.Text.Length == 1 && btn.Text != "MAYÚS")
                {
                    char c = btn.Text[0];
                    if (char.IsLetter(c))
                        btn.Text = estadoMayusculas ? char.ToUpper(c).ToString() : char.ToLower(c).ToString();
                }
            }

        }

        private void frmTecladoCompleto_Load(object sender, EventArgs e)
        {

        }
    }
}
