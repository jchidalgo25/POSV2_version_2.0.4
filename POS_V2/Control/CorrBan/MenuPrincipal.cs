using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using POS.Models;
using POS.Control.Pagos;

namespace POS.Control.CorrBan
{
    public partial class MenuPrincipal : Telerik.WinControls.UI.RadForm
    {
        private MainWindow _mainWindow;
        public MenuPrincipal(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            HabilitarOpciones();
            btnRetencionFisicaDev.Visible = false;
            HabilitarDevolucionRetencionFisica();
        }

        private void btnRecargas_Click(object sender, EventArgs e)
        {
            Recargas _frm = new Recargas();
            _frm.ShowDialog();
        }

        private void btnPagoServicios_Click(object sender, EventArgs e)
        {
            PagosServicios _frm = new PagosServicios();
            _frm.ShowDialog();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var db = new POSEntities();

            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
                case Keys.F5:
                    if (MessageBox.Show("Procediendo a actualizar los parámetros de Red Activa, pulse Sí para continuar", "Red Activa", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Control.CorrBan.ClsCorrBan.CargarParametrosRedActiva();
                        MessageBox.Show("Proceso finalizado");

                        _mainWindow.btnCorresponsal.Visible = CorrBan.ClsCorrBan.EsCorresponsalActivo;

                        if (!ClsCorrBan.EsCorresponsalActivo)
                        {
                            MessageBox.Show("Local no se encuentra habilitado o bien los parámetros no se cargaron correctamente. Cerrando el menú principal");
                            this.Close();
                        }
                        else
                            HabilitarOpciones();
                    }
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void HabilitarOpciones()
        {
            if (Control.CorrBan.ClsCorrBan.OpcionesPermitidas != null)
            {
                this.Text = "Corresponsal Bancario [" + ClsCorrBan.ServerName + "]";
                btnRecargas.Visible = (Control.CorrBan.ClsCorrBan.OpcionesPermitidas.Contains(1));
                btnPagoServicios.Visible = (Control.CorrBan.ClsCorrBan.OpcionesPermitidas.Contains(2));
                btnTransferencias.Visible = (Control.CorrBan.ClsCorrBan.OpcionesPermitidas.Contains(3));
            }
            else
            {
                btnRecargas.Visible = false;
                btnPagoServicios.Visible = false;
                btnTransferencias.Visible = false;
            }
            btnRetencionElect.Visible = Control.Common.GlobalParameters.Retencion_TienePermiso;
            btnTarjetaEmpre.Visible = Control.Common.GlobalParameters.TarjetaEmpresa_TienePermiso;
        }

        private void btnRetencionElect_Click(object sender, EventArgs e)
        {
            var f = new Control.Pagos.FrmRetencionElectronica();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
        }

        private void btnTarjetaEmpre_Click(object sender, EventArgs e)
        {
            var f = new Control.CrediEmpre.TarjeEmprePagos();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
        }

        private void btnRetencionFisicaDev_Click(object sender, EventArgs e)
        {
            
             var f = new Control.Pagos.FrmRetencionFisicaDev();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
            
        }

        private void HabilitarDevolucionRetencionFisica()
        {
            string ver = string.Empty;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    if(db.core_parametro.Any(x=> x.identificador=="HABILITAR_DEVOLUCIONRETFISICA"))
                    {
                        ver = db.core_parametro.Where(x => x.identificador == "HABILITAR_DEVOLUCIONRETFISICA").FirstOrDefault().valor;
                        if (ver.ToLower().Equals("true"))
                        {
                            btnRetencionFisicaDev.Visible = true;
                        }
                    }
                }

            }
            catch (Exception ex )
            {                
            }
        }

        private void rbtnDevFleteDomicilio_Click(object sender, EventArgs e)
        {
            try
            {
                FrmDevolucionFlete flete = new FrmDevolucionFlete();
                flete.Show();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            btnRetencionFisicaDev.Visible = Control.Common.GlobalParameters.VisibleDevolucionDineroRetencionFisica;
        }
    }
}
