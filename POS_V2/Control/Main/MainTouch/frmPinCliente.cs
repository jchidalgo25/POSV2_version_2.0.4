using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control.Main.MainTouch
{
    public partial class frmPinCliente : Telerik.WinControls.UI.RadForm
    {
        System.Windows.Forms.Control focused;

        public string Pin { get; private set; }
        public MainWindow _mainWindow;
        private string _textoOriginal = string.Empty;
        public string valorOriginalPIN = string.Empty;

        public frmPinCliente()
        {


            InitializeComponent();
            this.ActiveControl = txtNoPIN;

            // Inicializar PIN
            this.Pin = string.Empty;
            // Suscribir eventos a todos los RadTextBox (incluidos los anidados)
            //SuscribirTextBoxes(this);
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

        private void btnEnter_Click(object sender, EventArgs e)
        {

            string pin = string.Empty;
            pin = valorOriginalPIN;

            if (string.IsNullOrEmpty(pin))
            {
                Screen targetScreenClte = Control.Common.General.GetScreenClte();
                Control.Common.General.GetMensajeToList(647, targetScreenClte);
                return;
            }



            this.DialogResult = DialogResult.OK; 
            this.Close(); 
        }

         


        private void btnCancelar_Click(object sender, EventArgs e)
        {
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
    }
}
