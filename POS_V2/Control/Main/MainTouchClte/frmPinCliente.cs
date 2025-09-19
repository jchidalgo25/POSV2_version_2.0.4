using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MensajesLibrary;
using POS.Models;
using POS.Models.DevolucionIVA;
using POS.Models.SRI;

namespace POS.Control.Main.MainTouch
{


    
    public partial class frmPinCliente : Telerik.WinControls.UI.RadForm
    {
        System.Windows.Forms.Control focused;


        public string Pin { get; private set; }
        public string identificacion { get; set; }
        public decimal montoFactura { get; set; }
        public decimal montoIVA { get; set; }
        public string ruc_matriz { get; set; }
        public Factura _factura { get; set; }
        public decimal montoIva { get; set; }
        public decimal montoIvaDevolver { get; set; }
        public string ClaveAccesoSRI { get; set; }
        public string CodigoMensaje { get; set; }
        public string Mensaje { get; set; }
        public string claveClte { get; set; }
        public string claveAccesoComprobanteNC { get; set; }
        public string tipoComprobante { get; set; }
        public bool aplicaBeneficioDevolucionIVA { get; set; }
        public int numSecuenciaNC { get; set; }
        public datosDocumnetos datosDocumento { get; set; }
        

        public MainWindow _mainWindow;
        private string _textoOriginal = string.Empty;
        public string valorOriginalPIN = string.Empty;

        public frmPinCliente()
        {
            InitializeComponent();
            this.ActiveControl = txtNoPIN;
            this.TopMost = true;
            // Inicializar PIN
            this.Pin = string.Empty;
            
        }

        
        private void btnEvent(object sender, EventArgs e)
        {
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                var button = sender as System.Windows.Forms.Control;
                text.Text = text.Text + button.Text;
                text.Focus();
            }
        }

        private void txtNoPIN_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
        }


        private void btnBorrar_Click(object sender, EventArgs e)
        {
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                if (text.Text.Length > 1)
                    text.Text = text.Text.Substring(0, text.Text.Length - 1);
                else
                    text.Text = "";

                this.Pin = text.Text;
            }
        }

        public RespuestaToken recuperaTocket()
        {
            RespuestaToken BeaerToken = new RespuestaToken();
            string tokenBear = string.Empty;

            if (Control.Common.GlobalParameters.tokenResponse.ultimaAccess == null || Control.Common.GlobalParameters.tokenResponse.ultimaAccess.ToString("yyyyMMdd") == "00010101")
            {
                BeaerToken = Control.SRI.General.GetBearerToken(ruc_matriz,
                Common.GlobalParameters.SRI_USUARIO_ADICIONAL,
                Common.GlobalParameters.SRI_CLAVE_ADICIONAL);

                Control.Common.GlobalParameters.tokenResponse = BeaerToken;
                Control.Common.GlobalParameters.tokenResponse.ultimaAccess = DateTime.Now;
                tokenBear = Control.Common.GlobalParameters.tokenResponse.access_token;


            }

            if (string.IsNullOrEmpty(Control.Common.GlobalParameters.tokenResponse.access_token))
            {
                BeaerToken = Control.SRI.General.GetBearerToken(ruc_matriz,
                                        Common.GlobalParameters.SRI_USUARIO_ADICIONAL,
                                        Common.GlobalParameters.SRI_CLAVE_ADICIONAL);

                Control.Common.GlobalParameters.tokenResponse = BeaerToken;
                Control.Common.GlobalParameters.tokenResponse.ultimaAccess = DateTime.Now;
                tokenBear = Control.Common.GlobalParameters.tokenResponse.access_token;
            }


            DateTime ultimaFecha = Control.Common.GlobalParameters.tokenResponse.ultimaAccess;
            ultimaFecha = Control.Common.GlobalParameters.tokenResponse.ultimaAccess;
            TimeSpan diferencia = DateTime.Now - ultimaFecha;

            if (diferencia.TotalMinutes > 30)
            {
                BeaerToken = Control.SRI.General.GetBearerToken(ruc_matriz,
                Common.GlobalParameters.SRI_USUARIO_ADICIONAL,
                Common.GlobalParameters.SRI_CLAVE_ADICIONAL);
                Control.Common.GlobalParameters.tokenResponse = BeaerToken;
                Control.Common.GlobalParameters.tokenResponse.ultimaAccess = DateTime.Now;
            }

            BeaerToken = Control.Common.GlobalParameters.tokenResponse;
            tokenBear = Control.Common.GlobalParameters.tokenResponse.access_token;
            return BeaerToken;
        }



        private void btnEnter_Click(object sender, EventArgs e)
        {




            string pin = string.Empty;
            Screen targetScreenClte = Control.Common.General.GetScreenClte();
            Screen targetScreenCajero = Control.Common.General.GetScreenCajero();
            RespuestaToken BeaerToken = new RespuestaToken();

            string tokenBear = string.Empty;

            try
            {
                pin = valorOriginalPIN;
                ruc_matriz = _factura.Ruc_matriz;

                /*Valido que el PIN no este en blanco */
                if (string.IsNullOrEmpty(pin))
                {
                    /*Ejecuta mensaje del lado del cajero */
                    Task.Run(() =>
                    {
                        //647: El cliente debe de ingresar su PIN. Se activa pantalla del lado del cliente para que este sea ingresado.
                        Control.Common.General.GetMensajeToList(647, targetScreenClte);
                    });

                    return;
                }



                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"tokenBear: {tokenBear}");


                //Factura
                if (tipoComprobante == "01")
                {
                    if (_factura.GetTotal() == 0)
                    {
                        /*Ejecuta mensaje del lado del cajero */
                        Task.Run(() =>
                        {
                            //647: El cliente debe de ingresar su PIN. Se activa pantalla del lado del cliente para que este sea ingresado.
                            Control.Common.General.GetMensajeToList(649, targetScreenCajero);
                        });

                        return;
                    }

                    BeaerToken = recuperaTocket();
                    tokenBear = BeaerToken.access_token;


                    if (BeaerToken.codigo == 0)
                    {
                     
                        RespuestaDevlucion respuestaDevlucion = SRI.General.devolucionesIndividualesRecepciones(valorOriginalPIN, _factura, tokenBear);


                        int CodigoMensaje = Int32.Parse(respuestaDevlucion.codigo);
                        ClaveAccesoSRI = respuestaDevlucion.claveAccesoComprobante;

                        DialogResult dialogResult = DialogResult.OK;

                        if (CodigoMensaje == 2000)
                        {
                            montoIvaDevolver = respuestaDevlucion.montoIvaDevolver;
                            dialogResult = DialogResult.OK;
                        }
                        else
                        {
                            montoIvaDevolver = 0;
                            dialogResult = DialogResult.Cancel;
                        }

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"respuestaDevlucion.codigo: {respuestaDevlucion.codigo}");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"respuestaDevlucion.mensaje: {respuestaDevlucion.mensaje}");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"respuestaDevlucion.claveAccesoComprobante: {respuestaDevlucion.claveAccesoComprobante}");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "General", "devolucionesIndividualesRecepciones", $"respuestaDevlucion.montoIvaDevolver: {respuestaDevlucion.montoIvaDevolver}");

                        Common.GlobalParameters.SRI_PIN_CLTE = valorOriginalPIN;

                        this.CodigoMensaje = respuestaDevlucion.codigo;
                        this.Mensaje = respuestaDevlucion.mensaje;
                        this.ClaveAccesoSRI = respuestaDevlucion.claveAccesoComprobante;
                        this.montoIvaDevolver = respuestaDevlucion.montoIvaDevolver;
                        this.DialogResult = dialogResult;
                        this.claveClte = valorOriginalPIN;
                        this.Pin = valorOriginalPIN;
                       
                    }
                }
            }
            catch (Exception ex)
            {
                this.CodigoMensaje = "999";
                this.Mensaje = "Error exception: " + ex.Message;
                this.DialogResult = DialogResult.Cancel;
                this.ClaveAccesoSRI = ClaveAccesoSRI;
                this.claveClte = valorOriginalPIN;
                this.Pin = valorOriginalPIN;

            }
            finally
            {
                this.Dispose();
            }
        }
        

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //CancelaCierraVentana();

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool _isUpdatingText = false;

        private void txtNoPIN_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isUpdatingText || txtNoPIN.Text == _textoOriginal)
                    return;

                _isUpdatingText = true;
                string textoActual = txtNoPIN.Text;


                // Si el texto es igual al original, es probable que sea por actualización interna
                if (textoActual == _textoOriginal)
                    return;
                // 1. Identificar el nuevo carácter ingresado
                if (textoActual.Length > _textoOriginal.Length)
                {
                    // Se agregó un carácter
                    string nuevoCaracter = textoActual.Substring(_textoOriginal.Length);
                    _textoOriginal += nuevoCaracter;
                }
                else if (textoActual.Length < _textoOriginal.Length)
                {
                    // Opcional: manejar caso de borrado (por ejemplo, retroceso)
                    _textoOriginal = textoActual; // o recortar
                }


                // 2. Asignar a variable global
                valorOriginalPIN = _textoOriginal;

                // 3. Enmascarar antes de mostrar
                string textoEnmascarado = Control.Common.General.EnmascararTexto(_textoOriginal, 0, '*');

                // 4. Actualizar interfaz
                txtNoPIN.Text = textoEnmascarado;

                // 5. Cursor al final
                txtNoPIN.SelectionStart = txtNoPIN.Text.Length;


            }
            catch (Exception ex)
            {
                Console.Write("ERROR: " + ex.Message);
               
            }
            finally
            {
                _isUpdatingText = false;
            }





        }

        private void frmPinCliente_Load(object sender, EventArgs e)
        {
            
        }
        private void frmPinCliente_FormClosed(object sender, FormClosedEventArgs e)
        {
       
            //CancelaCierraVentana();
        }
        private void frmPinCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            //CancelaCierraVentana();
        }


        private void CancelaCierraVentana()
        {

            this.CodigoMensaje = "-99";
            this.Mensaje = "";
            this.DialogResult = DialogResult.Cancel;
            this.ClaveAccesoSRI = ClaveAccesoSRI;
            this.claveClte = valorOriginalPIN;
            this.Dispose();

        }
    }
}
