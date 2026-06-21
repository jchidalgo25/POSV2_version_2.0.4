using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.Control.Garancheck
{
    public partial class GarancheckForm : Form
    {
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        public GarancheckForm()
        {
            try
            {
                String ProfileDirectory = Control.Common.GlobalParameters.xulrunnerPath + @"\xulrunner\DefaultProfile";

                if (!Directory.Exists(ProfileDirectory))
                {
                    Directory.CreateDirectory(ProfileDirectory);
                }

                Skybound.Gecko.Xpcom.ProfileDirectory = ProfileDirectory;
                
                String xrPath = Assembly.GetExecutingAssembly().Location;
                xrPath = xrPath.Substring(0, xrPath.LastIndexOf(@"\") + 1) + @"Libs\xulrunner";

                Skybound.Gecko.Xpcom.Initialize(xrPath);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                this.Close();
            }
                       


            InitializeComponent();

        }

        private void btnKbd_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();

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

        private void GarancheckForm_Load(object sender, EventArgs e)
        {
           // webBrowser1.Url= new System.Uri(Control.Common.GlobalParameters.Linkbtn3Proveedor);

            geckoWebBrowser1.Navigate(Control.Common.GlobalParameters.Linkbtn3Proveedor);
        }

        private void geckoWebBrowser1_Click(object sender, EventArgs e)
        {

        }
    }
}
