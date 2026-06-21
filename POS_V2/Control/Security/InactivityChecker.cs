using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Drawing;

namespace POS.Control.Security
{
    public static class InactivityChecker
    {
        public static Timer IdleTimer = new Timer();
        static bool initialized = false;

        public static void StartSensor()
        {
            LeaveIdleMessageFilter limf = new LeaveIdleMessageFilter();
            Application.AddMessageFilter(limf);
            Application.Idle += new EventHandler(Application_Idle);
            IdleTimer.Interval = Control.Common.GlobalParameters.SensorInactividadSegundosIntervalo;    // One minute; change as needed
            IdleTimer.Tick += TimeDone;
            IdleTimer.Start();

            initialized = true;
        }

        public static void RestartSensor()
        {
            if (initialized)
            {
                IdleTimer.Stop();
                IdleTimer.Interval = Control.Common.GlobalParameters.SensorInactividadSegundosIntervalo;
                IdleTimer.Start();
            }
            else
            {
                StartSensor();
            }
        }

        static private void Application_Idle(Object sender, EventArgs e)
        {
            if (!IdleTimer.Enabled)     // not yet idling?
                IdleTimer.Start();
        }

        static private void TimeDone(object sender, EventArgs e)
        {

            try
            {
                //IdleTimer.Stop();   // not really necessary
                if (!Control.Common.WinForm.IsFormOpen(typeof(Control.Security.Wallpaper)))
                {
                    Screen targetScreenCajero = Control.Common.General.GetScreenCajero();
                    var screenCajero = targetScreenCajero ?? Screen.PrimaryScreen;

                    Task.Run(() =>
                    {
                        if (!Control.Common.GlobalParameters.VerificationInProgress)
                        {
                            using (var frmWallpaper = new Control.Security.Wallpaper())
                            {
                                ////Control.Security.Wallpaper frmWallpaper = new Control.Security.Wallpaper();
                                //frmWallpaper.Location = new Point(
                                //                screenCajero.WorkingArea.Left + (screenCajero.WorkingArea.Width - frmWallpaper.Width) / 2,
                                //                screenCajero.WorkingArea.Top + (screenCajero.WorkingArea.Height - frmWallpaper.Height) / 2);

                                //frmWallpaper.FormBorderStyle = FormBorderStyle.None;
                                //frmWallpaper.StartPosition = FormStartPosition.Manual;
                                //frmWallpaper.Bounds = screenCajero.Bounds;
                                //frmWallpaper.TopMost = true;
                                frmWallpaper.StartPosition = FormStartPosition.CenterScreen;
                                frmWallpaper.WindowState = FormWindowState.Maximized;
                                frmWallpaper.ShowInTaskbar = false;
                                frmWallpaper.ShowDialog();
                            }
                        }

                    });




                }
            }
            catch (Exception ex)
            {

                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.InactivityChecker", "TimeDone"
                  , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }

        }

    }


    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class LeaveIdleMessageFilter : IMessageFilter
    {
        const int WM_NCLBUTTONDOWN = 0x00A1;
        const int WM_NCLBUTTONUP = 0x00A2;
        const int WM_NCRBUTTONDOWN = 0x00A4;
        const int WM_NCRBUTTONUP = 0x00A5;
        const int WM_NCMBUTTONDOWN = 0x00A7;
        const int WM_NCMBUTTONUP = 0x00A8;
        const int WM_NCXBUTTONDOWN = 0x00AB;
        const int WM_NCXBUTTONUP = 0x00AC;
        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_RBUTTONUP = 0x0205;
        const int WM_MBUTTONDOWN = 0x0207;
        const int WM_MBUTTONUP = 0x0208;
        const int WM_XBUTTONDOWN = 0x020B;
        const int WM_XBUTTONUP = 0x020C;

        // The Messages array must be sorted due to use of Array.BinarySearch
        static int[] Messages = new int[] {WM_NCLBUTTONDOWN,
            WM_NCLBUTTONUP, WM_NCRBUTTONDOWN, WM_NCRBUTTONUP, WM_NCMBUTTONDOWN,
            WM_NCMBUTTONUP, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, WM_KEYDOWN, WM_KEYUP,
            WM_LBUTTONDOWN, WM_LBUTTONUP, WM_RBUTTONDOWN, WM_RBUTTONUP,
            WM_MBUTTONDOWN, WM_MBUTTONUP, WM_XBUTTONDOWN, WM_XBUTTONUP};

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE)  // mouse move is high volume
                return false;
            if (!InactivityChecker.IdleTimer.Enabled)     // idling?
                return false;           // No
            if (Array.BinarySearch(Messages, m.Msg) >= 0)
                InactivityChecker.IdleTimer.Stop();
            return false;
        }
    }
}
