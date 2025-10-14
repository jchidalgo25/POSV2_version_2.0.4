using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control
{
    public partial class KeyboardControl : Form
    {
        const int WS_EX_NOACTIVATE = 0x08000000;
        Telerik.WinControls.UI.RadTextBox text;
        string titulo;
        public string Tecla;


        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        // Constantes para SetWindowsPos
        //   Valores de wFlags
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOACTIVATE = 0x10;
        const int wFlags = SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE;
        //   Valores de hwndInsertAfter
        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;
        //
        /// <summary>
        /// Para mantener la ventana siempre visible
        /// </summary>
        /// <remarks>No utilizamos el valor devuelto</remarks>
        [DllImport("user32.DLL")]
        private extern static void SetWindowPos(
            int hWnd, int hWndInsertAfter,
            int X, int Y,
            int cx, int cy,
            int wFlags);

        public static void SiempreEncima(int handle)
        {
            SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, wFlags);
        }

        public static void NoSiempreEncima(int handle)
        {
            SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, wFlags);
        }

        public KeyboardControl(Telerik.WinControls.UI.RadTextBox Text, string Titulo)
        {
            text = Text;
            titulo = Titulo;
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= WS_EX_NOACTIVATE;
                return param;
            }
        }

     
        private void btnKbd_Click(object sender, EventArgs e)
        {       
            var button = sender as System.Windows.Forms.Control;
            if (text != null)
            {
                text.Text = text.Text + button.Text;
                text.Focus();
            }
        }
        

        private void btnEnter_Click(object sender, EventArgs e)
        {
            Tecla="\n";
            this.Close();
            /*
            IntPtr calculatorHandle = FindWindowByCaption(IntPtr.Zero, titulo);

            if (calculatorHandle == IntPtr.Zero)
            {
                return;
            }
            SetForegroundWindow(calculatorHandle);
           */
        }

        private void KeyboardControl_Deactivate(object sender, EventArgs e)
        {
            /*
            NoSiempreEncima(MainWindow.ActiveForm.Handle.ToInt32());
            //SetForegroundWindow(this.Handle);
            SiempreEncima(this.Handle.ToInt32());
            */
        }

        private void KeyboardControl_Activated(object sender, EventArgs e)
        {
          //  MessageBox.Show(this,text.Name);
        }

        private void btnDEL_Click(object sender, EventArgs e)
        {
            if (text != null)
            {
                if (text.Text.Length > 1)
                    text.Text = text.Text.Substring(0, text.Text.Length - 1);
                else
                    text.Text = "";
            }
        }
    }
}
