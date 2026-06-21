using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using POS.Control.Main.MainTouch;

namespace POS
{
    static class Program
    {
        public static string ID_Caja_POS;

        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "POS_UNIQUE_INSTANCE", out createdNew))
            {
                if (!createdNew)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Warning,
                        "Program",
                        "Main",
                        "Ya existe una instancia de la aplicación en ejecución."
                    );

                    Control.Common.General.GetMensajeToList(48);
                    return;
                }

                ConfigureGlobalExceptionHandling();
                RunApplication();
            }
        }

        private static void ConfigureGlobalExceptionHandling()
        {
            Application.ThreadException += (s, e) => GlobalExceptionHandler(e.Exception, "UI Thread");
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                GlobalExceptionHandler((Exception)e.ExceptionObject, "Unhandled");
        }

        private static void GlobalExceptionHandler(Exception ex, string source)
        {
            try
            {
                var inner = ex.InnerException?.Message ?? "No inner";
                Control.Common.Logger.LogMessage(
                    Control.Common.Enum.LogTypes.Fatal,
                    source,
                    "Exception",
                    $"Excepción: {ex.Message}\nStack: {ex.StackTrace}\nInner: {inner}"
                );
            }
            catch
            {
                // último recurso: no propagamos más errores
            }
        }

        private static void RunApplication()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Verificar configuración inicial
                if ((POS.Properties.Settings.Default.ROOT_URL == "http://[ipserver]:8000") ||
                    (POS.Properties.Settings.Default.ROOT_URL_GENERAL == "http://[ipserver]:8000/webrequests/"))
                {
                    using (SeleccionaLocal f = new SeleccionaLocal())
                    {
                        f.ShowDialog();
                    }
                }

                // Notificación de apertura
                DialogResult dr;
                POS.Models.User user;
                AppData data = new AppData();

                using (frmNotificaApertura apertura = new frmNotificaApertura())
                {
                    dr = apertura.ShowDialog();
                    user = (POS.Models.User)apertura.Tag;
                }

                // Si el usuario cancela o requiere login
                if (dr == DialogResult.Retry || dr == DialogResult.Cancel)
                {
                    using (LoginForm login = new LoginForm(data))
                    {
                        dr = login.ShowDialog();
                        user = (POS.Models.User)login.Tag;
                    }
                }

                // Iniciar ventana principal
                if (dr == DialogResult.OK && user != null)
                {
                    Application.Run(new MainWindow(user, data));
                }
                else
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "Program",
                        "Main",
                        "La aplicación se cerró antes de iniciar MainWindow."
                    );
                }
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler(ex, "Program.Main");
            }
        }
    }
}
