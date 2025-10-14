using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Liris_MenssageDLL.Model;

namespace MensajesLibrary
{    
    public partial class MsgBoxCtrl : UserControl
    {
        private MessageType _TipoMensaje;
        public MessageType TipoMensaje
        {
            get { return _TipoMensaje; }
            set { _TipoMensaje = value; }
        }

        private static string _Mensaje;
        public  string TextMensaje
        {
            get { return _Mensaje; }
            set { _Mensaje = value; }
        }

        private string _TituloMensaje;
        public string TituloMensaje
        {
            get { return _TituloMensaje; }
            set { _TituloMensaje = value; }
        }


        private int _TiempoEspera;
        public int TiempoEspera
        {
            get { return _TiempoEspera; }
            set { _TiempoEspera = value; }
        }

        private bool _MostrarContador;
        public bool MostrarContador
        {
            get { return _MostrarContador; }
            set { _MostrarContador = value; }
        }

        private int _codigoMensaje;
        public int CodigoMensaje
        {
            get { return _codigoMensaje; }
            set { _codigoMensaje = value; }
        }

        private DrawingFontCtrl _drawingFont;

        public DrawingFontCtrl drawingFont
        {
            get { return _drawingFont; }
            set { _drawingFont = value; }
        }

        private MessageBoxResult _RepuestaMensaje;
        public MessageBoxResult RepuestaMensaje
        {
            get { return _RepuestaMensaje; }
            set { _RepuestaMensaje = value; }
        }

        private string _TextMensajeAdicional;
        public string TextMensajeAdicional
        {
            get { return _TextMensajeAdicional; }
            set { _TextMensajeAdicional = (value ?? "").Trim(); }
        }

        private int tiempo;
        private int CodigoMessageBox;
        private string TituloMessageBox;
        private string TextoMessageBox;
        private string TextoMessageAdicionalBox;
        int TiempoMaximo;

        public enum MessageBoxResult
        {
            Yes,
            No,
            Ok,
            Cancel,
            Timeout
        }

        public enum MessageType
        {
            Information,
            Warning,
            Error,
            Success, 
            Question, 
            Stop, 
            Exclamation
        }


        private MessageType _Type;
        private MessageBoxResult _result;
        private int _timeRemaining = 10;
        private Form _containerForm;

        private TaskCompletionSource<MessageBoxResult> _tcs;

        public int timeRemaining;
        private Timer countdownTimer = new Timer();
        private Timer _timer = new Timer();

        private static int WidthFormFinal = 0;
        private static int HeightFormFinal = 0;
        private static int WidthForm = 0;
        private static int HeightForm = 0;

        private static Size SizeForm;
        private static float emSize = 0;
        private static FontFamily fontFamily;
        private System.Drawing.Font DrawingFont;

  

        public MsgBoxCtrl()
        {
            InitializeComponent();

            WidthFormFinal = 690;
            HeightFormFinal = 525;

            WidthForm = 690;
            HeightForm = 420;
           

            SizeForm = new Size(WidthForm, HeightForm);
            this.Size = SizeForm;
           
            //fontFamily = new FontFamily("Segoe UI");
            //DrawingFont = new System.Drawing.Font(fontFamily, emSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1 segundo
            countdownTimer.Tick += CountdownTimer_Tick;
        }


        public MsgBoxCtrl(string Tipo, string Message, string TituloMensaje, int tiempo, bool contador)
        {
            InitializeComponent();

            if (tiempo != 0)
            {
                countdownTimer = new Timer();
                countdownTimer.Interval = 1000; // 1 segundo
                countdownTimer.Tick += CountdownTimer_Tick;
            }
        }

        public MessageBoxResult ShowMessage(bool contador = false)
        {
            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            CodigoMessageBox = CodigoMensaje;
            TituloMessageBox = TituloMensaje;
            TiempoMaximo = 0;
            TextoMessageBox = TextMensaje;
            TextoMessageAdicionalBox = TextMensajeAdicional;

            btnAceptar.Visible = false;
            btnCancelar.Visible = false;

            drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();
            Font font = drawingFont.GetFont();
            DrawingFont = font;

            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = font;
            //this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            panel1.Size = new Size(WidthForm - 3, HeightForm - 6);
            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(WidthForm, HeightForm),
                ShowInTaskbar = false,
                TopMost = true
            };
            
            this.Dock = DockStyle.Fill;
            this.Update();
            _containerForm.Controls.Add(this);


            if (TiempoEspera > 0)
            {
                if (MostrarContador)
                {
                    TextoMessageBox = string.Concat(TextMensaje, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", TiempoEspera.ToString());
                }

                StartCountdown(TiempoEspera);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }

            //TextoMessageAdicionalBox = TextMensajeAdicional;
            this.lblMessageMore.Text = TextoMessageAdicionalBox;

            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (TipoMensaje)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok.";

                    btnCancelar.Visible = false;


                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Sí";

                    btnCancelar.Visible = true;
                    btnCancelar.Text = "No";

                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "Cancelar";


                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok.";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "No";


                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "No";

                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }

           
            //_containerForm.Owner = MainWindow; // o tu formulario principal
            _containerForm.ShowDialog();
             RepuestaMensaje = _result;
            return _result;

        }

        public MessageBoxResult ShowMessage(Form parent, bool contador = false)
        {
            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            CodigoMessageBox = CodigoMensaje;
            TituloMessageBox = TituloMensaje;
            TiempoMaximo = 0;
            TextoMessageBox = TextMensaje;
            TextoMessageAdicionalBox = TextMensajeAdicional;

            btnAceptar.Visible = false;
            btnCancelar.Visible = false;

            drawingFont = new Liris_MenssageDLL.Model.DrawingFontCtrl();
            Font font = drawingFont.GetFont();
            DrawingFont = font;

            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = font;
            //this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            panel1.Size = new Size(WidthForm - 3, HeightForm - 6);
            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(WidthForm, HeightForm),
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            this.Update();
            _containerForm.Controls.Add(this);


            if (TiempoEspera > 0)
            {
                if (MostrarContador)
                {
                    TextoMessageBox = string.Concat(TextMensaje, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", TiempoEspera.ToString());
                }

                StartCountdown(TiempoEspera);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }

            //TextoMessageAdicionalBox = TextMensajeAdicional;
            this.lblMessageMore.Text = TextoMessageAdicionalBox;

            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (TipoMensaje)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok.";

                    btnCancelar.Visible = false;


                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Sí";

                    btnCancelar.Visible = true;
                    btnCancelar.Text = "No";

                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "Cancelar";


                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok.";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "No";


                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "No";

                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            //_containerForm.Owner = MainWindow; // o tu formulario principal
            //JEspinoza.sn Modificación porque se oculta ventana detras de la ventana principal
            _containerForm.Activate();
            _containerForm.BringToFront();
            //if (Application.OpenForms.Count > 0)
            //    _containerForm.ShowDialog(Application.OpenForms[0]);
            //else
            //    _containerForm.ShowDialog();
            _containerForm.Owner = parent;
            _containerForm.ShowDialog();
            //JEspinoza.en
            RepuestaMensaje = _result;
            return _result;

        }
        public MessageBoxResult ShowMessage(string TipoMensaje, string Message, int CodigoMensaje, string TituloMensaje, int tiempo = 0, bool contador = false)
        {
            //InitializeComponent();

            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = tiempo;
            MostrarContador = contador;
            TextoMessageBox = Message;
            CodigoMessageBox = CodigoMensaje;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = DrawingFont;

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                //Size = new Size(694, 516),
                Size = new Size(WidthForm, HeightForm),
                //Size = SizeForm,
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }
            //TextoMessageAdicionalBox = TextMensajeAdicional;
            this.lblMessageMore.Text = TextoMessageAdicionalBox;

            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }


            switch (TipoMensaje)
            {
                case "info":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok.";

                    btnCancelar.Visible = false;


                    break;
                case "question":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Sí";

                    btnCancelar.Visible = true;
                    btnCancelar.Text = "No";

                    break;
                case "warning":
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "Cancelar";


                    break;
                case "error":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok.";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "No";


                    break;
                case "stop":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;

                    btnAceptar.Visible = true;
                    btnAceptar.Text = "Ok";

                    btnCancelar.Visible = false;
                    btnCancelar.Text = "No";

                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }

            _containerForm.ShowDialog();

            return _result;

        }

        public MessageBoxResult ShowMessage(MessageType messageType, string Message)
        {
            /*InitializeComponent(); */

            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = "";
            TiempoMaximo = 0;
            MostrarContador = false;
            TextoMessageBox = Message;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.lblMessage.Font = new System.Drawing.Font(fontFamily, emSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                //Size = new Size(575, 351),
                Size = new Size(694, 516),
                //Size = SizeForm,
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                if (MostrarContador)
                {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }

                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }
            //TextoMessageAdicionalBox = TextMensajeAdicional;
            this.lblMessageMore.Text = TextoMessageAdicionalBox;

            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (messageType)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            _containerForm.ShowDialog();

            return _result;

        }

        public MessageBoxResult ShowMessage(MessageType messageType, string Message, string TituloMensaje)
        {
            //InitializeComponent();

            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = 0;
            MostrarContador = false;
            TextoMessageBox = Message;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.lblMessage.Font = new System.Drawing.Font(fontFamily, emSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                //Size = new Size(575, 351),
                Size = new Size(694, 516),
                //Size = SizeForm,
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                if (MostrarContador) {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }

                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }
            //TextoMessageAdicionalBox = TextMensajeAdicional;
            this.lblMessageMore.Text = TextoMessageAdicionalBox;

            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (messageType)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            _containerForm.ShowDialog();

            return _result;

        }

        public MessageBoxResult ShowMessage(MessageType messageType, string Message, string TituloMensaje, int tiempo = 0, bool contador = false)
        {
            //InitializeComponent();

            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = tiempo;
            MostrarContador = contador;
            TextoMessageBox = Message;



            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.lblMessage.Font = new System.Drawing.Font(fontFamily, emSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                //Size = new Size(575, 351),
                Size = new Size(694, 516),
                //Size = SizeForm,

                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;

            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                if (contador) {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }
                
                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }
            //TextoMessageAdicionalBox = TextMensajeAdicional;
            this.lblMessageMore.Text = TextoMessageAdicionalBox;

            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (messageType)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            _containerForm.ShowDialog();

            return _result;

        }

        public Task<MessageBoxResult> ShowMessageAsync(Control parent, string Tipo, string Message, string TituloMensaje, int tiempo = 0, bool contador = false)
        {
            //InitializeComponent();

            _tcs = new TaskCompletionSource<MessageBoxResult>();

            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = tiempo;
            MostrarContador = contador;
            TextoMessageBox = Message;
            TextoMessageAdicionalBox = TextMensajeAdicional;

            
            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.lblMessage.Font = new System.Drawing.Font(fontFamily, emSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                //Size = new Size(575, 351),
                Size = new Size(694, 516),
                //Size = SizeForm,
                ShowInTaskbar = false,
                TopMost = true
            };


            this.Location = new Point(
               (parent.ClientSize.Width - this.Width) / 2,
               (parent.ClientSize.Height - this.Height) / 2
           );

            parent.Controls.Add(this);
            this.BringToFront();

            if (tiempo > 0)
            {
                if (contador)
                {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }

                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }
            //TextoMessageAdicionalBox = TextMensajeAdicional;
            this.lblMessageMore.Text = TextoMessageAdicionalBox;


            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (Tipo)
            {
                case "info":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case "question":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case "warning":
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case "error":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case "stop":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }

            return _tcs.Task;

        }

        private void StartCountdown(int startTimeInSeconds)
        {
            if (countdownTimer == null) { countdownTimer = new Timer(); }
                
            timeRemaining = startTimeInSeconds;
            countdownTimer.Start(); // Iniciar el temporizador
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            timeRemaining--; // Decrementar el tiempo restante

            string TextoMessageBoxBase = TextoMessageBox;
            TextoMessageBoxBase = TextoMessageBoxBase.Replace("[TIEMPO_ESPERA]", timeRemaining.ToString());
            this.lblMessage.Text = TextoMessageBoxBase;

            //TextoMessageBox = TextoMessageBox;
            // Si el tiempo se agotó, detener el temporizador
            if (timeRemaining == 0)
            {
                TextoMessageBoxBase = TextoMessageBox.Replace("[TIEMPO_ESPERA]", timeRemaining.ToString());
                this.lblMessage.Text = TextoMessageBoxBase;

                countdownTimer.Stop(); // Detener el temporizador
                countdownTimer.Dispose();
                timeRemaining = 0;

                _result = MessageBoxResult.Timeout;

                btnAceptar_Click(sender, e);
              

            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (timeRemaining != 0) { _timer.Stop(); }

            _result = MessageBoxResult.No;
            _containerForm.Close();

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (timeRemaining != 0) { _timer.Stop(); }

            // _tcs.SetResult(MessageBoxResult.Yes);
            // this.Parent.Controls.Remove(this);

            _result = MessageBoxResult.Yes;
            _containerForm.Close();

        }

        public static bool IsFormOpen(Type formType)
        {
            foreach (Form form in Application.OpenForms)
                if (form.GetType().Name == formType.Name)
                    return true;
            return false;
        }


        public void ForceCloseApplication(string msg, string Titulo)
        {
            ShowMessage(MessageType.Success, msg, Titulo);
            //ShowMessage(Titulo, msg);
            Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lnkVerMas_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool mostrarDetalle = !panelDetalle.Visible;
            panelDetalle.Visible = mostrarDetalle;
            lnkVerMas.Text = mostrarDetalle ? "Ver menos" : "Ver más";

            this.Dock = DockStyle.None;

            // Cambiar tamaño del UserControl
            if (mostrarDetalle)
            {
                //this.Size = new Size(700, 600);
                SizeForm = new Size(WidthFormFinal, HeightFormFinal);
            }
            else
            {
                //this.Size = new Size(700, 400);
                SizeForm = new Size(WidthForm, HeightForm);
            }

            this.Size = SizeForm;
            this.Parent.Size = this.Size;
            this.Invalidate();
            this.Update();

            
            // Redimensionar panel1 en el formulario padre
            Form formularioPadre = this.FindForm();
            if (formularioPadre != null)
            {
                Panel panel1 = formularioPadre.Controls.Find("panel1", true).FirstOrDefault() as Panel;
                if (panel1 != null)
                {
                    //panel1.Size = new Size(this.Width, this.Height); // +20 para margen si lo deseas
                    panel1.Size = new Size(this.Width - 3, this.Height - 6);

                }
            }
            
        }
    }


}
