using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Liris_MenssageDLL.Model;

namespace Liris_MenssageDLL
{
    public partial class MsgBoxCtrl_v2 : Telerik.WinControls.UI.RadForm
    {
        #region Enums

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

        #endregion
        #region Properties



        private int tiempo;
        private int CodigoMessageBox;
        private string TituloMessageBox;
        private string TextoMessageBox;
        private string TextoMessageAdicionalBox;
        int TiempoMaximo;


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


        private MessageType _type;
        private string _message;
        private string _title;
        private int _timeoutSeconds = 0;
        private bool _showCounter = false;
        private string _details = "";

        private MessageType _TipoMensaje;
        public MessageType TipoMensaje
        {
            get { return _type; }
            set { _type = value; }
        }

        private static string _Mensaje;
        public string TextMensaje
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _TituloMensaje;
        public string TituloMensaje
        {
            get { return _title; }
            set { _title = value; }
        }


        private int _TiempoEspera;
        public int TiempoEspera
        {
            get { return _timeoutSeconds; }
            set { _timeoutSeconds = value; }
        }

        private bool _MostrarContador;
        public bool MostrarContador
        {
            get { return _showCounter; }
            set { _showCounter = value; }
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
            set { _TextMensajeAdicional = value; }
        }

       
        private Timer _countdownTimer;

        #endregion


        public MsgBoxCtrl_v2()
        {
            InitializeComponent();

            ConfigureForm();

        }

        

        private void ConfigureForm()
        {
            //this.Text = "Mensaje";
            //this.Size = new Size(694, 516);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;

            WidthFormFinal = 690;
            HeightFormFinal = 525;

            WidthForm = 690;
            HeightForm = 420;

            SizeForm = new Size(WidthForm, HeightForm);
            this.Size = SizeForm;

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1 segundo
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        #region Public Methods


        public MessageBoxResult ShowMessage()
        {
            Configure(TipoMensaje, TextMensaje, TituloMensaje, TextMensajeAdicional, TiempoEspera, MostrarContador);


            return this.ShowDialog() == DialogResult.OK ? MessageBoxResult.Ok : MessageBoxResult.Cancel;
        }

        public MessageBoxResult ShowMessage(IWin32Window owner)
        {
            Configure(TipoMensaje, TextMensaje, TituloMensaje, TextMensajeAdicional, TiempoEspera, MostrarContador);

            if (tiempo != 0)
            {
                countdownTimer = new Timer();
                countdownTimer.Interval = 1000; // 1 segundo
                countdownTimer.Tick += CountdownTimer_Tick;
            }

            return this.ShowDialog() == DialogResult.OK ? MessageBoxResult.Ok : MessageBoxResult.Cancel;
        }

        #endregion

        #region Configuración Interna
        private void Configure(
            MessageType type,
            string message,
            string title,
            string details,
            int timeoutSeconds,
            bool showCounter)
        {
            this.Text = title;
            lblTitiuloMessage.Text = title;
            lblTitiuloMessage.Visible = !string.IsNullOrEmpty(title);

            
            lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            lblMessage.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblMessage.Text = message;


            if (!string.IsNullOrEmpty(details))
            {
                lblMessageMore.Text = details;
                lnkVerMas.Visible = true;
                panelDetalle.Visible = false;
            }
            else
            {
                lnkVerMas.Visible = false;
                panelDetalle.Visible = false;
            }

            ConfigureMessageType(type);

            if (timeoutSeconds > 0)
            {
                if (showCounter)
                    lblMessage.Text = message.Replace("[TIEMPO_ESPERA]", timeoutSeconds.ToString());

                StartCountdown(timeoutSeconds);
            }

            

            this.Refresh();
        }

        private void ConfigureMessageType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);
                    pbInfo.Show();
                    btnAceptar.Text = "Ok.";
                    btnCancelar.Hide();
                    break;

                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);
                    pbQue.Show();
                    btnAceptar.Text = "Sí";
                    btnCancelar.Show();
                    btnCancelar.Text = "No";
                    break;

                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);
                    pbWar.Show();
                    btnAceptar.Text = "Ok";
                    btnCancelar.Hide();
                    break;

                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);
                    pbError.Show();
                    btnAceptar.Text = "Ok.";
                    btnCancelar.Hide();
                    break;

                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);
                    pbStop.Show();
                    btnAceptar.Text = "Ok";
                    btnCancelar.Hide();
                    break;

                default:
                    pL1.BackColor = Color.LightGray;
                    btnAceptar.Text = "Ok";
                    btnCancelar.Hide();
                    break;
            }
        }

        private void StartCountdown(int seconds)
        {
            _timeRemaining = seconds;
            _countdownTimer = new Timer { Interval = 1000 };
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            _timeRemaining--;
            if (_timeRemaining <= 0)
            {
                _countdownTimer.Stop();
                _countdownTimer.Dispose();
                _result = MessageBoxResult.Timeout;
                this.Close();
            }
            else if (MostrarContador)
            {
                lblMessage.Text = TextMensaje.Replace("[TIEMPO_ESPERA]", _timeRemaining.ToString());
            }
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            _result = MessageBoxResult.Yes;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            _result = MessageBoxResult.No;
            this.Close();
        }

        private void LnkVerMas_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool mostrar = !panelDetalle.Visible;
            panelDetalle.Visible = mostrar;
            lnkVerMas.Text = mostrar ? "Ver menos" : "Ver más";

            this.Height = mostrar ? 600 : 516;
        }

        #endregion
    }
}
