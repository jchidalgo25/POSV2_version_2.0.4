using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;

namespace POS.UnitTests
{
    public partial class WebPortal : Form
    {
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();

        public WebPortal()
        {
            InitializeComponent();
        }

        private void PortalCanje_Load(object sender, EventArgs e)
        {
            NavegarPortal();
        }

        private void NavegarPortal()
        {
            this.Opacity = 100;
        }

        private void btnKbd_Click(object sender, EventArgs e)
        {
            //string windir = Environment.GetEnvironmentVariable("WINDIR");
            //string osk = null;
            //if (osk == null)
            //{
            //    osk = "C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe";
            //    if (!File.Exists(osk))
            //    {
            //        osk = null;
            //    }
            //}
            
            //if (osk == null)
            //{
            //    osk = Path.Combine(Path.Combine(windir, "SysWOW64"), "osk.exe");
            //    if (!File.Exists(osk))
            //    {
            //        osk = null;
            //    }
            //}

            //if (osk == null)
            //{
            //    osk = Path.Combine(Path.Combine(windir, "system32"), "osk.exe");
            //    if (!File.Exists(osk))
            //    {
            //        osk = null;
            //    }
            //}

            //if (osk == null)
            //{
            //    osk = "osk.exe";
            //}
            //virtualKeyboard = System.Diagnostics.Process.Start(osk); // open
            
           

        }

    
        private void GarancheckForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("tabtip");
            for (int i = 0; i < procs.Length; i++)
            {
                try
                {
                    procs[i].Kill();
                }
                catch
                {
                }
            }
            procs = Process.GetProcessesByName("osk");
            for (int i = 0; i < procs.Length; i++)
            {
                try
                {
                    procs[i].Kill();
                }
                catch
                {
                }
            }

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
