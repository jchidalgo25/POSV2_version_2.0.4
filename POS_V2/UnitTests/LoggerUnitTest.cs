using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;

namespace POS.UnitTests
{
    public partial class LoggerUnitTest : Form
    {
        public LoggerUnitTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Control.Common.Logger.Limpiar_Trace();
                Control.Common.Logger.Agregar_Trace("INSERT TEST 1", true);
                Control.Common.Logger.Agregar_Trace("INSERT TEST 2", true);
                Control.Common.Logger.Agregar_Trace("INSERT TEST 3", true);
                Control.Common.Logger.Graba_TraceFile();
                //POSEntities db = new POSEntities();
                //db.Database.Log = Logger.Graba_TraceFile;
                //var test = new TBLTEST();
                //test.contado = 10;
                //db.TBLTEST.Add(test);
                //db.SaveChanges();
                //db.SaveChangesAsync().Wait();
                //Logger.Graba_TraceFile("Test", Properties.Settings.Default.EF_LOG);
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var nc = new Models.core_notacredito() { fecha = DateTime.Now };
                nc.core_notacreditodetalle.Add(new core_notacreditodetalle() { fecha = DateTime.Now });
                nc.core_notacreditodetalle.Add(new core_notacreditodetalle() { fecha = DateTime.Now });
                Control.Common.Logger.Agregar_Trace_NotaCredito(nc);
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var entidad = new Models.POS_VOUCHER();
                Control.Common.Logger.Agregar_Trace_Voucher(entidad);
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
