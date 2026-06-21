using System;
using System.Windows.Forms;

namespace POS.Control.Common
{
    public static class WinForm
    {
        public static bool IsFormOpen(Type formType)
        {
            foreach (Form form in Application.OpenForms)
                if (form.GetType().Name == formType.Name)
                    return true;
            return false;
        }

        public static void ShowMessage(string msg, string title = "POS")
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static void ForceCloseApplication(string msg)
        {
            ShowMessage(msg);
            Control.Common.GlobalParameters.MustCloseApplication = true;
            Application.Exit();
        }
    }
}
