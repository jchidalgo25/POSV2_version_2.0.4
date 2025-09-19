using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control.Main.MainTouch
{
    public static class UIHelper
    {
        public static void InvokeIfRequired(this System.Windows.Forms.Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }
    }
}
