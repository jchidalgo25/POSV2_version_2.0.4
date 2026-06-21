
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
using Telerik.WinControls;
using System.Diagnostics;
using System.IO;

namespace POS.Control.Pagos
{
    public partial class RetencionesPagos : Telerik.WinControls.UI.RadForm
    {

        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        Factura _factura;
        private bool _gretAnioAnterior = false;
        private DateTime _gdtUltDiaAnio;
        private DateTime _gdtNullDate = new DateTime(1, 1, 1);
        public RetencionesPagos(Factura factura)
        {
            InitializeComponent();
            this._factura = factura;
        }

        private void RetencionesPagos_Load(object sender, EventArgs e)
        {
            decimal subtotalRetFte1 = 0, subtotalRetFte175 = 0;
            decimal retFt1 = 0M, retFte175 = 0M, RetFteTotal=0M;

            try
            {
                if (Control.Common.GlobalParameters.checkRevisionesRet != "" && Control.Common.GlobalParameters.checkRevisionGeneralRet != "")
                {                    
                    this.Size= new System.Drawing.Size(1052, 656 );
                    gbxValidacionComprobantes.Visible = true;
                    btnOk.Enabled = false;
                    var check = Control.Common.GlobalParameters.checkRevisionesRet.Split('|');
                    for (int i = 0; i < check.Count(); i++)
                    {
                        chkListRevision.Items.Add(check.ElementAt(i));
                    }
                    chkRevisionG.Text = Control.Common.GlobalParameters.checkRevisionGeneralRet;
                }
                else
                {
                    this.Size = new System.Drawing.Size(679, 656);
                    gbxValidacionComprobantes.Visible = false;
                }

                if (Control.Common.GlobalParameters.RetIvaRuc != "")
                {
                    chkListContribuyente.Visible = true;
                    lblDescripcionRuc.Visible = true;
                    pnlRuc.Visible = false;
                    var chkRuc = Control.Common.GlobalParameters.RetIvaRuc.Split('|');
                    for (int i = 0; i < chkRuc.Count(); i++)
                    {
                        chkListContribuyente.Items.Add("RUC " + chkRuc.ElementAt(i));
                    }

                }
                else
                {
                    chkListContribuyente.Visible = false;
                    lblDescripcionRuc.Visible = false;
                    pnlRuc.Visible = true;

                }

                var pos = new POSEntities();
            txtFactura.Text =  _factura.Establecimiento + _factura.PtoEmision + _factura.Secuencia.ToString("000000000.##");
            cmbFecReg.Value = DateTime.Today;

            cmbFecAut.SetToNullValue();
            cmbFecAut.Value = DateTime.Today;

            cmbFecReg.Enabled = false;
            cmbFecReg.ReadOnly = true ;

            txtValor.Text = _factura.getSubTotalSinDescuento().ToString();
            txtDscto.Text = _factura.GetDescuentos().ToString();
            txtSubTot.Text = _factura.getSubTotal().ToString();


            
            subtotalRetFte1 = _factura.getSubTotalRetFte1();
            subtotalRetFte175= _factura.getSubTotalRetFte175();

            txtSubtotal1.Text = subtotalRetFte1.ToString();
            txtsubtotal175.Text = subtotalRetFte175.ToString();

            retFt1 = subtotalRetFte1 * (1M / 100);
            retFte175 = subtotalRetFte175 * (1.75M / 100);

                RetFteTotal = retFt1 + retFte175;

            txtIva.Text = _factura.getIVA().ToString();
            // txtRet.Text = (Math.Round(_factura.getSubTotal() * Decimal.Parse(pos.core_parametro.Where(x => x.identificador == "RETENCION_FTE").First().valor), 2, MidpointRounding.ToEven) ).ToString();
            txtRet.Text = (Math.Round(retFt1, 2, MidpointRounding.ToEven)).ToString();
                txtRetFte175.Text = (Math.Round(retFte175, 2, MidpointRounding.ToEven)).ToString();
                txtTot.Text = _factura.GetTotal().ToString();

            cmbRetIva.DataSource = pos.vw_RETIVA.OrderBy(x=> x.RETENTION).ToList();
            //   cmbBancoTarjeta.DataSource = db.core_tarjetacredito.ToList();
            cmbRetIva.DisplayMember = "CONCEPT";
            cmbRetIva.ValueMember = "CODE";

            
            //Validación para retenciones del Año anterior del mes de diciembre.
            if (_factura.Fecha.Year == DateTime.Now.Year - 1 && _factura.Fecha.Month == 12 && DateTime.Now.Month == 1)
            {
                cmbFecReg.Value = _factura.Fecha;
                cmbFecReg.Enabled = true;
                cmbFecReg.ReadOnly = false;
                _gretAnioAnterior = true;
                _gdtUltDiaAnio = new DateTime(_factura.Fecha.Year, 12, 31);
            }

                /*
                            var objChe = cmbEleccion.SelectedValue as vw_RETIVA;
                            Texto = objChe.nombre + "-" + txtCuenta.Text + "-" + txtNumCheque.Text;
                            break;

                        */
            }
            catch (Exception ex )
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "RetencionPagos", "Load", "Ha courrido una Excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));                
            }
        }
        private Boolean ValidaNumeroRetencionCliente()
        {
            Boolean _valida = true;
            try
            {
                string numeroret = txtEstab.Text.Replace("_", "").PadLeft(3, '0') + txtPtoEmi.Text.Replace("_", "").PadLeft(3, '0') + txtNumeracion.Text.Replace("_", "").PadLeft(9, '0');

                using (POSEntities db = new POSEntities())
                {
                    
                        if (db.core_retencion.Any(x => x.core_factura.cliente == _factura.Cliente_codigo && x.num_retencion == numeroret))
                        {
                            _valida = false;
                            // btnOk.Enabled = false;
                            MessageBox.Show("Ya está registrada esta Retención para este cliente.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    

                }

                return _valida;

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            string numeroret = txtEstab.Text.Replace("_", "").PadLeft(3, '0') + txtPtoEmi.Text.Replace("_", "").PadLeft(3, '0') + txtNumeracion.Text.Replace("_", "").PadLeft(9, '0');
            if (numeroret.Replace("0", "") == ""  || numeroret.Length<15 || txtAut.Text.Replace("_", "") == "" || txtAut.Text.Replace("_", "").PadLeft(10,'0').Length < 10 || cmbFecAut.Value.Year.ToString() == "1" )
            {
                MessageBox.Show(this,"Debe llenar la informacion y/o \n*Verifique el numero de Retencion debe ser de 15 digitos\n*Verifique el numero de Autorización debe ser de 10 digitos");
                return;
            }

            if(!ValidaNumeroRetencionCliente())
            {
                return;
            }

            var valor = 0M;
            if (decimal.TryParse(txtTotalRet.Text, out valor) && valor > 0)
            {
                _factura.agregarPagoRetencion(valor, "RETCLIENTE");
                _factura.Retencion.NumAutorizacion = txtAut.Text.Replace("_", "").PadLeft(10, '0');
                _factura.Retencion.FechaAutorizacion = cmbFecAut.Value;
                _factura.Retencion.NumFactura = txtFactura.Text;
                _factura.Retencion.NumRetencion = numeroret;
                _factura.Retencion.ValorBase = _factura.getSubTotal();
                _factura.Retencion.ValorRetFte = decimal.Parse(txtRet.Text);
                _factura.Retencion.ValorRetFte1 = decimal.Parse(txtRet.Text);
                _factura.Retencion.valor_ret_fte175 = decimal.Parse(txtRetFte175.Text);
                _factura.Retencion.valor_base1 = decimal.Parse(txtSubtotal1.Text);
                _factura.Retencion.valor_base175 = decimal.Parse(txtsubtotal175.Text);
                //_factura.Retencion.valor = decimal.Parse(txtsubtotal175.Text);

                var db = new POSEntities();
                var objRet = db.vw_RETIVA.Where(x => x.CONCEPT == cmbRetIva.Text).First();
                   
                
                _factura.Retencion.CodRetIVA = objRet.CODE;
                _factura.Retencion.CodPorcRetIVA = objRet.RETENTION;
                _factura.Retencion.ValorBaseIVA = decimal.Parse(txtIva.Text);
                _factura.Retencion.ValorRetIVA = decimal.Parse(txtRetIVA.Text);
                _factura.Retencion.ConceptoRetIVA = objRet.CONCEPT;

            }

            this.Close();
        }

        private void cmbRetIva_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            var pos = new POSEntities();
            if (cmbRetIva.Text!="" && cmbRetIva.Text!= "POS.Models.vw_RETIVA")
            {
                txtRetIVA.Text = (Math.Round(decimal.Parse(txtIva.Text) * (pos.vw_RETIVA.Where(x => x.CONCEPT == cmbRetIva.Text).First().RETENTION / 100), 2, MidpointRounding.ToEven)).ToString();
            }
            txtTotalRet.Text = (decimal.Parse(txtRet.Text) + decimal.Parse(txtRetIVA.Text) + decimal.Parse(txtRetFte175.Text)).ToString();


        }

        private void btnKbd_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();

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

        private void RetencionesPagos_FormClosing(object sender, FormClosingEventArgs e)
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


        private void rbtnRucPersonNat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cmbRetIva.SelectedValue = "9";// ret.CodRetIVA;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "RetencionesPagos", "rbtnRucPersonNat_CheckedChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void rbtnRUC_NoObligContabilidad_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cmbRetIva.SelectedValue = "8";// ret.CodRetIVA;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "RetencionesPagos", "rbtnRUC_NoObligContabilidad_CheckedChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void radioButton3rbtn_RUCObligContab_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cmbRetIva.SelectedValue = "1";// ret.CodRetIVA;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "RetencionesPagos", "rbtn_RUCObligContab_CheckedChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void cmbFecReg_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_gretAnioAnterior && cmbFecReg.Value.Date != _gdtUltDiaAnio && cmbFecReg.Value.Date != _gdtNullDate)
                {
                    //Control.Common.WinForm.ShowMessage("La Retención no existe, o no pertenece a este cliente");
                    //MessageBox.Show("La fecha de emisión de la retención no es válida, se rechaza retención.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(595);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "RetencionesPagos", "cmbFecReg_ValueChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void chkRevisionG_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //int valida = 0;
                if (chkRevisionG.Checked == true)
                {
                    if (Control.Common.GlobalParameters.checkRevisionesRet != "" && Control.Common.GlobalParameters.checkRevisionGeneralRet != "")
                    {
                        for (int i = 0; i < chkListRevision.Items.Count; i++)
                        {
                            if (!chkListRevision.GetItemChecked(i))
                            {
                                //valida = 1;
                                //MessageBox.Show("No se han completado los check de revisión.", "validaCheckRevision", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Control.Common.General.GetMensajeToList(596);

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "RetencionesPagos", "chkRevisionG_CheckedChanged", "Falta realizar validación de comprobante.");
                                btnOk.Enabled = false;
                                chkRevisionG.Checked = false;
                                break;
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "RetencionesPagos", "chkRevisionG_CheckedChanged", "Estan completos los Check de revisión.");
                                btnOk.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "RetencionesPagos", "chkRevisionG_CheckedChanged", "Falta check de responsabilidad de revisión.");
                    btnOk.Enabled = false;
                }
            }
            catch (Exception)
            {


            }
        }

        private void chkListRevision_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //int valida = 0;
                if (chkRevisionG.Checked == true)
                {
                    if (Control.Common.GlobalParameters.checkRevisionesRet != "" && Control.Common.GlobalParameters.checkRevisionGeneralRet != "")
                    {
                        for (int i = 0; i < chkListRevision.Items.Count; i++)
                        {
                            if (!chkListRevision.GetItemChecked(i))
                            {
                                //valida = 1;
                                // MessageBox.Show("No se han completado los check de revisión.", "validaCheckRevision", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Control.Common.General.GetMensajeToList(597);

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "RetencionesPagos", "chkRevisionG_CheckedChanged", "Falta realizar validación de comprobante.");
                                btnOk.Enabled = false;
                                chkRevisionG.Checked = false;
                                break;
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "RetencionesPagos", "chkListRevision_SelectedIndexChanged", "Estan completos los Check de revisión.");
                                btnOk.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "RetencionesPagos", "chkListRevision_SelectedIndexChanged", "Falta check de responsabilidad de revisión.");
                    btnOk.Enabled = false;
                }
            }
            catch (Exception)
            {


            }
        }

        private void chkListContribuyente_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int indice = chkListContribuyente.SelectedIndex;
                lblDescripcionRuc.Text = "";
                using (POSEntities pos = new POSEntities())
                {
                    cmbRetIva.DataSource = pos.vw_RETIVA.OrderBy(x => x.RETENTION).ToList();
                    //   cmbBancoTarjeta.DataSource = db.core_tarjetacredito.ToList();
                    cmbRetIva.DisplayMember = "CONCEPT";
                    cmbRetIva.ValueMember = "CODE";
                }
                if (chkListContribuyente.GetItemChecked(indice))
                {
                    var chkTipo = Control.Common.GlobalParameters.RetIvaTipo.Split('|');
                    for (int tipo = 0; tipo < chkTipo.Count(); tipo++)
                    {
                        if (indice == tipo)
                        {
                            cmbRetIva.SelectedValue = chkTipo.ElementAt(tipo);
                            var chkDescripcionRuc = Control.Common.GlobalParameters.RetIvaLeyenda.Split('|');
                            for (int iDescripcion = 0; iDescripcion < chkDescripcionRuc.Count(); iDescripcion++)
                            {
                                if (indice == iDescripcion)
                                {
                                    var chkDescripcionRuc2 = chkDescripcionRuc.ElementAt(iDescripcion).Split('_');
                                    for (int iDescripcion2 = 0; iDescripcion2 < chkDescripcionRuc2.Count(); iDescripcion2++)
                                    {
                                        lblDescripcionRuc.Text = lblDescripcionRuc.Text + chkDescripcionRuc2.ElementAt(iDescripcion2) + ".\n";
                                    }

                                }
                            }
                        }
                    }

                    for (int i1 = 0; i1 < chkListContribuyente.Items.Count; i1++)
                    {
                        if (indice != i1)
                        {
                            chkListContribuyente.SetItemChecked(i1, false);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
