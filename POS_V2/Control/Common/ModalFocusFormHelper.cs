using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control.Common
{
    public static class ModalFocusFormHelper
    {
        public static void ShowNonModalWithFocus(Form owner, Form childForm)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (childForm == null) throw new ArgumentNullException(nameof(childForm));

            // Centrar el formulario hijo respecto al owner
            childForm.Load += (s, e) =>
            {
                childForm.StartPosition = FormStartPosition.Manual;
                var ownerRect = owner.Bounds;
                int x = ownerRect.Left + (ownerRect.Width - childForm.Width) / 2;
                int y = ownerRect.Top + (ownerRect.Height - childForm.Height) / 2;
                childForm.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
            };

            //Timer focusTimer = new Timer { Interval = 500 };
            //focusTimer.Tick += (s, e) =>
            //{
            //    if (!childForm.Focused && !childForm.Disposing && !childForm.IsDisposed)
            //    {
            //        childForm.Invoke(new Action(() =>
            //        {
            //            if (!childForm.Focused)
            //                childForm.Activate();
            //        }));
            //    }
            //};

            //childForm.Activated += (s, e) => focusTimer.Start();
            //childForm.Deactivate += (s, e) => focusTimer.Start();

            //childForm.FormClosed += (s, e) =>
            //{
            //    focusTimer.Stop();
            //    focusTimer.Dispose();

            //    // Reactivar formulario padre al cerrar el hijo
            //    if (!owner.IsDisposed)
            //    {
            //        owner.Enabled = true;
            //        owner.Activate();
            //    }
            //};

            // Mostrar sin bloqueo modal
            childForm.Show(owner);
        }

    }

}
