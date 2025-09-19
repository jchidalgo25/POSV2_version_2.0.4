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
using Telerik.WinControls.UI;

namespace POS.Control.Pagos
{
    /// <summary>
    /// Formulario de Devolución de Retenciones Manuales - Fisicas.
    /// Desarrollado por: Erick Velasco 
    /// Fecha : 2019-10-17
    /// </summary>
    public partial class FrmRetencionFisicaDev : Telerik.WinControls.UI.RadForm
    {
        private Models.RetencionElectronica _objRetFisica = new Models.RetencionElectronica();
        private pos_customer CustomerPOS { get; set; }
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        Factura _factura;
        private bool consultaRetencion = false;
        private decimal olguraValRet = 0.01M;
        private bool _gretAnioAnterior = false;
        private DateTime _gdtUltDiaAnio ;
        private DateTime _gdtNullDate = new DateTime(1, 1, 1);
        private decimal subtotalRteFte1, subtotalRteFte175;
        DateTime dtFechaInicioVigenciaRteFte175 = new DateTime(2020, 04, 01);
        bool bFacturaAplicaRetFte175 = false;
        /// <summary>
        /// Número de Días para solicitar la Devolución de la Retención despues de la Emisión de la Factura.
        /// </summary>
        private int NumeroDiasDevoluDespuesFactura = 5;
        public FrmRetencionFisicaDev()
        {
            InitializeComponent();
        }

        private void FrmRetencionFisicaDev_Load(object sender, EventArgs e)
        {
            cmbFecAut.Value = DateTime.Now.Date;
            cmbFecReg.Value = DateTime.Now.Date;
            cmbRetIva.Enabled = false;
            txtRet.Visible = false;
            txtRetIVA.Visible = false;
            txtTotalRet.Visible = false;
            if (Control.Common.GlobalParameters.checkRevisionesRet != "" && Control.Common.GlobalParameters.checkRevisionGeneralRet != "")
            {
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
                gbxValidacionComprobantes.Visible = false;              
            }

            if (Control.Common.GlobalParameters.RetIvaRuc != "")
            {
                chkListContribuyente.Visible = true;
                lblDescripcionRuc.Visible = true;
                pnlRuc.Visible=false;
                var chkRuc = Control.Common.GlobalParameters.RetIvaRuc.Split('|');
                for (int i = 0; i < chkRuc.Count(); i++)
                {
                    chkListContribuyente.Items.Add("RUC "+chkRuc.ElementAt(i));
                }

            }
            else
            {
                chkListContribuyente.Visible = false;
                lblDescripcionRuc.Visible = false;
                pnlRuc.Visible = true;

            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            
            try
            {                
                var pos = new POSEntities();

                string numeroFactura = txtEstabFact.Text.Replace("_", "").PadLeft(3, '0') + txtPtoEmiFact.Text.Replace("_", "").PadLeft(3, '0') + txtNumeracionFact.Text.Replace("_", "").PadLeft(9, '0');
                string numeroret = txtEstab.Text.Replace("_", "").PadLeft(3, '0') + txtPtoEmi.Text.Replace("_", "").PadLeft(3, '0') + txtNumeracion.Text.Replace("_", "").PadLeft(9, '0');
                if (numeroret.Replace("0", "") == ""  || numeroret.Length<15 || txtAut.Text.Replace("_", "") == "" || txtAut.Text.Replace("_", "").PadLeft(10,'0').Length < 10 || cmbFecAut.Value.Year.ToString() == "1" )
                {
                    Control.Common.General.GetMensajeToList(483);
                    //MessageBox.Show(this,"Debe llenar la informacion y/o \n*Verifique el número de Retención debe ser de 15 digitos\n*Verifique el numero de Autorización debe ser de 10 digitos");
                    return;
                }

                //Validación si existe registrada esa retención.
                // if (pos.core_retencion.Any(x => x.num_factura == txtFactura.Text && x.TipoRetencion == 0))
                if (pos.core_retencion.Any(x => x.num_factura == numeroFactura && x.TipoRetencion == 0))
                {
                    //MessageBox.Show("El número de Factura ingresado no existe o no es el correcto, favor ingrese una factura válida.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //var retencionFisica = pos.core_retencion.Where(x => x.num_factura == txtFactura.Text && x.TipoRetencion == 0).FirstOrDefault();
                    var retencionFisica = pos.core_retencion.Where(x => x.num_factura == numeroFactura && x.TipoRetencion == 0).FirstOrDefault();
                    //MessageBox.Show("Esta retención ya fué registrada.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Control.Common.General.GetMensajeToList(485);

                    return;
                }
                //Valida si lo ingresado por el usuario corresponde a los validado por el sistema, tomando como tolerancia a los valores +-1 centavo.
                if(!validaCajasTextoRetencion())
                {
                    return;
                }

                if(!ValidaNumeroRetencionCliente())
                {
                    return;
                }

                if (_gretAnioAnterior && cmbFecReg.Value.Date != _gdtUltDiaAnio && cmbFecReg.Value.Date != _gdtNullDate)
                {
                    //MessageBox.Show("La fecha de emisión de la retención no es válida, se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(486);
                    return;
                }
                
                if (Control.Common.GlobalParameters.RetIvaRuc != "")
                {
                    int contador = 0;
                    for (int indice = 0; indice < chkListContribuyente.Items.Count; indice++)
                    {
                        if (!chkListContribuyente.GetItemChecked(indice))
                        {
                            contador = contador + 1;
                        }

                    }
                    if (contador == chkListContribuyente.Items.Count)
                    {
                        //MessageBox.Show("Debe Seleccionar el tipo de Retención IVA.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Control.Common.General.GetMensajeToList(487);
                        return;
                    }
                }
                else
                {
                   
                    if (!rbtnRucContribuyEspec.Checked && !radioButton3rbtn_RUCObligContab.Checked && !rbtnRUC_NoObligContabilidad.Checked)
                    {
                        //MessageBox.Show("Debe Seleccionar el tipo de Retención IVA.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Control.Common.General.GetMensajeToList(487);
                        return;
                    }
                }
                SetObjRetencion(numeroret);

                var valor = 0M;
            if ((decimal.TryParse(txtTotalRetInput.Text.Replace("$", ""), out valor) && valor > 0))
            {
                    using (var db1 = new POSEntities())
                    {

                        using (System.Data.Entity.DbContextTransaction dbContextTransaction = db1.Database.BeginTransaction())
                        {
                            try
                            {
                                decimal _retFte = 0, _retFte175 = 0, _retIva = 0;
                                _retFte = string.IsNullOrEmpty(txtRetInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetInput.Text.Replace("$", ""));
                                _retFte175 = string.IsNullOrEmpty(txtRet175Input.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRet175Input.Text.Replace("$", ""));
                                _retIva = string.IsNullOrEmpty(txtRetIVAInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetIVAInput.Text.Replace("$", ""));

                                core_retencion retencion = new core_retencion();
                                retencion.num_autorizacion = txtAut.Text.Replace("_", "").PadLeft(10, '0');
                                retencion.fecha_autorizacion = cmbFecAut.Value;
                                //retencion.num_factura = txtFactura.Text;
                                retencion.num_factura = numeroFactura;
                                retencion.num_retencion = numeroret;
                                retencion.valor_base = Convert.ToDecimal(txtSubTot.Text);
                                //retencion.valor_ret_fte = decimal.Parse(txtRet.Text);
                                retencion.valor_ret_fte = _retFte;
                                retencion.valor_ret_fte175 = _retFte175;
                                retencion.valor_base175 = subtotalRteFte175;
                                retencion.valor_base1 = subtotalRteFte1;
                                retencion.TipoRetencion = 0;
                                var db = new POSEntities();
                                var objRet = db.vw_RETIVA.Where(x => x.CONCEPT == cmbRetIva.Text).First();


                                retencion.CodRetIVA = objRet.CODE;
                                retencion.CodPorcRetIVA = objRet.RETENTION;
                                retencion.ValorBaseIVA = decimal.Parse(txtIva.Text);
                                retencion.ValorRetIVA = _retIva;
                                retencion.ConceptoRetIVA = objRet.CONCEPT;
                                retencion.factura_id = _factura.IdFacturaPOS;
                                retencion.fecha_creacion = DateTime.Now;
                                if (_gretAnioAnterior && cmbFecReg.Value.Year+1 == DateTime.Now.Year && cmbFecReg.Value.Month == 12 && DateTime.Now.Month == 1)
                                {
                                    
                                    retencion.fecha_modificacion = _gdtUltDiaAnio;
                                }
                                else
                                {
                                    retencion.fecha_modificacion = cmbFecReg.Value;
                                }

                               // retencion.fecha_modificacion = DateTime.Now;
                                retencion.UsuarioCreacion = Control.Common.GlobalParameters.Usuario;
                                retencion.UsuarioModifica = Control.Common.GlobalParameters.Usuario;
                                retencion.Cliente = txtCedula.Text;
                                retencion.Establecimiento = Control.Common.GlobalParameters.Establecimiento;
                                retencion.PuntoEmision = Control.Common.GlobalParameters.PuntoEmision;
                                retencion.Procesado = 0;
                                retencion.ID_Caja = Program.ID_Caja_POS;
                                db1.core_retencion.Add(retencion);

                                db1.SaveChanges();

                                _objRetFisica.ImprimirReciboFisica();

                                dbContextTransaction.Commit();
                                //Control.Common.WinForm.ShowMessage("Devolución realizada exitosamente.");
                                Control.Common.General.GetMensajeToList(489);
                                this.Close();

                            }
                            catch (Exception ex)
                            {
                                string msj = ex.ToString();

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "btnOk_Click", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                Properties.Settings.Default.MAILERROR_FROM,
                                Properties.Settings.Default.MAILERROR_ALIAS,
                                Properties.Settings.Default.MAILERROR_DESTINO,
                                Properties.Settings.Default.MAILERROR_CC,
                                Properties.Settings.Default.MAILERROR_MOTIVO,

                                String.Format("Establecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroId: {4} \n\nDatos Excepcion ------------\nClass: {5} \nMethod: {6} \nMessage: {7} \nStackTrace: {8}",
                                              Control.Common.GlobalParameters.Establecimiento,
                                              Control.Common.GlobalParameters.PuntoEmision,
                                              Control.Common.GlobalParameters.IpMaquina,
                                              Control.Common.GlobalParameters.UserObj.nombres,
                                              Control.Common.GlobalParameters.UserObj.username,
                                              "POS.Control.Pagos.FrmRetencionFisicaDev",
                                              "btnOk_Click",
                                              Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                              ex.StackTrace),
                                false,
                                String.Empty);

                                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "btnOk_Click - Envio Correo en Exception", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                                }
                                dbContextTransaction.Rollback();
                                //Control.Common.WinForm.ShowMessage("No se pudo grabar la transacción en este momento, esto puede deberse a una breve interrupción en la comunicación, inténtelo nuevamente en unos momentos");
                                Control.Common.General.GetMensajeToList(488);
                            }

                        }
                    }
                   

                    //_factura.agregarPagoRetencion(valor, "RETCLIENTE");


                }
            this.Close();

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "btnOk_Click", "Ha ocurrido una excepción al grabar la Retención, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //MessageBox.Show("Lo sentimos, no se pudo grabar la retención intente de nuevo.","Devolución de Retencion Fisica", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Control.Common.General.GetMensajeToList(490);
            }
        }

        private void cmbRetIva_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            try
            {
                
                var pos = new POSEntities();
                if (cmbRetIva.Text != "" && cmbRetIva.Text != "POS.Models.vw_RETIVA")
                {
                    txtRetIVA.Text = (Math.Round(decimal.Parse(txtIva.Text) * (pos.vw_RETIVA.Where(x => x.CONCEPT == cmbRetIva.Text).First().RETENTION / 100), 2, MidpointRounding.ToEven)).ToString();
                    txtRetIVAInput.Text = txtRetIVA.Text;
                }
                else
                {
                    txtRetIVA.Text = decimal.Parse("0.00").ToString();
                    txtRetIVAInput.Text = txtRetIVA.Text;
                }

                txtTotalRet.Text = (decimal.Parse(txtRet.Text)+ (string.IsNullOrEmpty(txtRet175.Text) ? 0 : decimal.Parse(txtRet175.Text)) + (string.IsNullOrEmpty(txtRetIVA.Text)? 0 : decimal.Parse(txtRetIVA.Text))).ToString();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "cmbRetIva_SelectedIndexChanged", "Ha ocurrido una excepción al calcular la Retención, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
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

        private void FrmRetencionFisicaDev_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "FrmRetencionFisicaDev_FormClosing", "Ha ocurrido una excepción al Cerrar el Formulario, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void txtFactura_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    GetDatosFactura();
                }

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtFactura_KeyPress", "Ha ocurrido una excepción al presionar Obtener Datos de la Factura, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        #region Métodos
        private void GetDatosFactura()
        {
            string establecimiento = string.Empty;
            string puntoEmision = string.Empty;
            string numeroSecuencia = string.Empty;
            long numero = 0;
            decimal RteFte = 0,RteIVA=0;
            decimal subtotal1 = 0, subtotal175 = 0;
            string numeroRetencion = string.Empty;
            var pos = new POSEntities();
            bool tieneRetencion = false;
            string ivaRetCODE = string.Empty;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            try
            {
                btnOk.Enabled = true;
                btnReimprimirRecibo.Enabled = false;
                LimpiarPantalla();

                /*establecimiento = txtFactura.Text.Substring(0, 3);
                puntoEmision = txtFactura.Text.Substring(3, 3);
                numeroSecuencia = txtFactura.Text.Substring(6);
                numero = Convert.ToInt64(numeroSecuencia);*/
                establecimiento = txtEstabFact.Text;
                puntoEmision = txtPtoEmiFact.Text;
                numeroSecuencia = txtNumeracionFact.Text;
                numero = Convert.ToInt64(numeroSecuencia);
                string numeroFactura = string.Empty;
                numeroFactura = establecimiento + puntoEmision + numeroSecuencia;

                using (POSEntities db = new POSEntities())
                {
                    if (db.core_factura.Any(x => x.establecimiento == establecimiento && x.punto_emision == puntoEmision && x.numero == numero))
                    {
                        var factura = db.core_factura.Where(x => x.establecimiento == establecimiento && x.punto_emision == puntoEmision && x.numero == numero).FirstOrDefault();

                        txtCedula.Text = factura.cliente;
                        lblNombre.Text = factura.razon_social;

                        if (db.core_retencion.Any(x => x.num_factura == numeroFactura && x.TipoRetencion == 0))
                        {
                            

                            //MessageBox.Show("El número de Factura ingresado no existe o no es el correcto, favor ingrese una factura válida.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            var retencionFisica = db.core_retencion.Where(x => x.num_factura == numeroFactura && x.TipoRetencion == 0).FirstOrDefault();

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[num_retencion]", valor = retencionFisica.num_retencion });
                            Control.Common.General.GetMensajeToList(491, parametros);

                            //MessageBox.Show("Esta factura ya se encuentra registrada en la retención '" + retencionFisica.num_retencion + "'.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            
                            //txtCedula.Clear();
                            //lblNombre.Text= string.Empty;
                            //factura = null;
                            btnOk.Enabled = false;
                            btnReimprimirRecibo.Enabled = true;
                            consultaRetencion = true;
                            txtEstab.Text = retencionFisica.num_retencion.Substring(0, 3);
                            txtPtoEmi.Text = retencionFisica.num_retencion.Substring(3, 3);
                            txtNumeracion.Text = retencionFisica.num_retencion.Substring(6);
                            txtAut.Text = retencionFisica.num_autorizacion;
                            cmbFecAut.Value = retencionFisica.fecha_autorizacion;
                            //cmbFecReg.Value = retencionFisica.fecha_creacion;
                            cmbFecReg.Value = retencionFisica.fecha_modificacion;
                            RteIVA = retencionFisica.ValorRetIVA;
                            txtTotalRet.Text = (retencionFisica.valor_ret_fte + retencionFisica.ValorRetIVA + (retencionFisica.valor_ret_fte175==null?0: retencionFisica.valor_ret_fte175)).ToString(); ;
                            txtSubtotal1.Text = retencionFisica.valor_base1==null?"0":retencionFisica.valor_base1.ToString();// RetInput.Text = txtTotalRet.Text;
                            txtsubtotal175.Text = retencionFisica.valor_base175 == null ? "0" : retencionFisica.valor_base175.ToString();
                            //bFacturaAplicaRetFte175
                            cmbRetIva.Enabled = false;
                            numeroRetencion = retencionFisica.num_retencion;
                            txtRetIVA.Text = RteIVA.ToString();
                            txtRetIVAInput.Text = RteIVA.ToString();
                            ivaRetCODE = retencionFisica.CodRetIVA;
                            txtRet.Text = retencionFisica.valor_ret_fte.ToString();
                            txtRetInput.Text = txtRet.Text;
                            txtRet175.Text = retencionFisica.valor_ret_fte175==null?"0":retencionFisica.valor_ret_fte175.ToString();
                            txtRet175Input.Text = txtRet175.Text;
                            //Read Only
                            txtEstab.ReadOnly = true;
                            txtPtoEmi.ReadOnly = true;
                            txtNumeracion.ReadOnly = true;
                            txtAut.ReadOnly = true;
                            cmbFecAut.ReadOnly = true;
                            cmbFecReg.ReadOnly = true;
                            tieneRetencion = true;
                            rbtnRucContribuyEspec.Enabled = false;
                            radioButton3rbtn_RUCObligContab.Enabled = false;
                            rbtnRUC_NoObligContabilidad.Enabled = false;


                        }
                        else
                        {
                            consultaRetencion = false;
                                                       
                            
                            if ((DateTime.Now.Date >= factura.fecha_creacion.AddDays(NumeroDiasDevoluDespuesFactura)) && (!_gretAnioAnterior))
                            {
                                btnOk.Enabled = false;

                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[NumeroDiasDevoluDespuesFactura]", valor = NumeroDiasDevoluDespuesFactura.ToString() });
                                parametros.Add(new ParametrosMensajes() { codigo = "[fecha_creacion]", valor = factura.fecha_creacion.ToString("dd/MM/yyyy")  });
                                Control.Common.General.GetMensajeToList(492, parametros);


                                //Control.Common.WinForm.ShowMessage("El tiempo máximo para la devolución de dinero es " + NumeroDiasDevoluDespuesFactura.ToString() + " días a partir de la emisión de la factura ( fecha de emisión: " + factura.fecha_creacion.ToString("dd/MM/yyyy") + ") ");
                                txtCedula.Clear();
                                lblNombre.Text = string.Empty;
                                return;
                            }

                            cmbFecEmisionFact.Value = factura.fecha_creacion;
                            //Validación para retenciones del Año anterior del mes de diciembre.
                            if (factura.fecha_creacion.Year == DateTime.Now.Year - 1 && factura.fecha_creacion.Month == 12 && DateTime.Now.Month == 1)
                            {
                                cmbFecReg.Enabled = true;
                                cmbFecReg.ReadOnly = false;
                                _gretAnioAnterior = true;
                                _gdtUltDiaAnio = new DateTime(factura.fecha_creacion.Year, 12, 31);
                            }

                            if (factura.cliente.Length < 13)
                            {
                                btnOk.Enabled = false;
                                //MessageBox.Show("El cliente no tiene Ruc. No se puede realizar Devolución.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                Control.Common.General.GetMensajeToList(493);

                                txtCedula.Clear();
                                lblNombre.Text = string.Empty;
                                return;
                            }

                            if (factura.cliente == "9999999999999")
                            {
                                btnOk.Enabled = false;
                                //MessageBox.Show("No se puede recibir Retención para factura emitida a CONSUMIDOR FINAL.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                Control.Common.General.GetMensajeToList(494);

                                txtCedula.Clear();
                                lblNombre.Text = string.Empty;
                                return;
                            }

                            //Validación de facturas con pago Tarjeta Empresarial.
                            if (factura.core_facturapago.Any(x => x.tipo_id == "TAR PORTAL"))
                            {
                                btnOk.Enabled = false;
                                //MessageBox.Show("No se puede realizar Devolución para Facturas con pago Tarjeta Empresarial.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                Control.Common.General.GetMensajeToList(495);

                                txtCedula.Clear();
                                lblNombre.Text = string.Empty;
                                return;
                            }
                            //cmbRetIva.Enabled = true;

                            //Read Only
                            txtEstab.ReadOnly = false;
                            txtPtoEmi.ReadOnly = false;
                            txtNumeracion.ReadOnly = false;
                            txtAut.ReadOnly = false;
                            cmbFecAut.ReadOnly = false;
                            cmbFecReg.ReadOnly = false;

                            cmbFecAut.Value = DateTime.Now.Date;
                            cmbFecReg.Value = DateTime.Now.Date;

                            rbtnRucContribuyEspec.Enabled = true;
                            radioButton3rbtn_RUCObligContab.Enabled = true;
                            rbtnRUC_NoObligContabilidad.Enabled = true;

                        }

                        GetCustomer(factura.cliente);
                        if (factura.fecha_creacion >= dtFechaInicioVigenciaRteFte175)
                            bFacturaAplicaRetFte175 = true;
                        else
                            bFacturaAplicaRetFte175 = false;

                        if (bFacturaAplicaRetFte175)
                        {
                            GetRetencion175(factura);//obtiene
                        }
                        else
                        {
                            txtRet175Input.Enabled = false;
                        }

                        _factura = new Factura();
                        _factura.IdFacturaPOS = factura.id;
                        _factura.Fecha = factura.fecha_creacion;

                        //if
                        //Valida que no haya sido ingresado la misma retención anteriormente.
                        //if (db.core_retencion.Any(x => x.num_factura == txtFactura.Text && x.TipoRetencion == 0))

                        /*else
                        {*/

                        txtValor.Text = Math.Round(factura.subtotal, 2).ToString();// .getSubTotalSinDescuento().ToString();
                        txtDscto.Text = Math.Round(factura.descuento, 2).ToString(); // GetDescuentos().ToString();
                        txtSubTot.Text = Math.Round(factura.subtotal, 2).ToString(); //_factura.getSubTotal().ToString();

                        if (!bFacturaAplicaRetFte175)
                        {
                            txtSubtotal1.Text = txtSubTot.Text;
                            subtotalRteFte1 = factura.subtotal;
                        }

                       // txtSubtotal1.Text = string.Empty;
                       // txtsubtotal175.Text = string.Empty;
                        txtIva.Text = Math.Round(factura.iva, 2).ToString(); //_factura.getIVA().ToString();

                        //RteFte
                        if (db.core_parametro.Any(x => x.identificador == "RETENCION_FTE"))
                        {
                            
                            if (!tieneRetencion)
                            {
                                RteFte = Decimal.Parse(db.core_parametro.Where(x => x.identificador == "RETENCION_FTE").First().valor);
                                txtRet.Text = (Math.Round(subtotalRteFte1 * RteFte, 2, MidpointRounding.ToEven)).ToString();
                                txtRetInput.Text = txtRet.Text;

                                if (bFacturaAplicaRetFte175)
                                {
                                    txtRet175.Text = (Math.Round(subtotalRteFte175 * (1.75M / 100), 2, MidpointRounding.ToEven)).ToString();
                                    txtRet175Input.Text = txtRet175.Text;
                                }
                            }
                        }


                        txtTot.Text = Math.Round(subtotalRteFte1+ subtotalRteFte175, 2).ToString();//_factura.GetTotal().ToString();
                        RadListDataItem _raitm;
                        _raitm = new RadListDataItem("NO APLICA RETENCIÓN IVA", 0);

                        cmbRetIva.DataSource = db.vw_RETIVA.Where((x => x.CODE == "9" || x.CODE == "1" || x.CODE == "8")).ToList();
                        //cmbRetIva.Items.Add(_raitm);
                            /*var retiva = from vw_RETIVA in ret
                                     where ret.CODE == "9" || ret.CODE == "1"
                                     select ret;*/

                        //.OrderBy(x => x.RETENTION).ToList();
                        //   cmbBancoTarjeta.DataSource = db.core_tarjetacredito.ToList();
                        cmbRetIva.DisplayMember = "CONCEPT";
                            cmbRetIva.ValueMember = "CODE";
                        cmbRetIva.SelectedValue = "8";

                        if (tieneRetencion)
                        {
                            cmbRetIva.SelectedValue = ivaRetCODE;
                            switch (ivaRetCODE)
                                {
                                case "1":
                                    {
                                        radioButton3rbtn_RUCObligContab.Checked = true;
                                        break;
                                    }
                                case "8":
                                    {
                                        rbtnRUC_NoObligContabilidad.Checked = true;
                                        break;
                                    }
                                case "9":
                                    {
                                        rbtnRucContribuyEspec.Checked= true;
                                        break;
                                    }
                                    
                                }
                        }
                            if (!string.IsNullOrEmpty(numeroRetencion))
                            {
                            txtRetIVA.Text =RteIVA.ToString();
                            txtRetIVAInput.Text = txtRetIVA.Text;
                            //txtRet.Text = RteFte
                            SetObjRetencion(numeroRetencion);
                            }
                        consultaRetencion = false;
                        //btnOk.Enabled = true;

                        //}
                        if (btnOk.Enabled == true && validaCheckRevision() == true)
                        {
                            btnOk.Enabled = true;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "GetDatosFactura()", "Todos los check de validacion estan completos.");
                        }
                        else
                        {
                            btnOk.Enabled = false;
                        }
                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(496);
                        //MessageBox.Show("El número de Factura ingresado no existe o no es el correcto, favor ingrese una factura válida.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        CustomerPOS = null;
                        LimpiarPantalla();
                        btnOk.Enabled = false;
                        txtNumeracionFact.Focus();
                    }

                    
                }
                

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "GetDatosFactura", "Ha ocurrido una excepción al consultar datos de la Factura, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //Control.Common.WinForm.ShowMessage("Complete la infromación de la Factura");
                Control.Common.General.GetMensajeToList(497);
            }
        }
        private bool validaCheckRevision()
        {
            bool valida = true;
            if (Control.Common.GlobalParameters.checkRevisionesRet != "" && Control.Common.GlobalParameters.checkRevisionGeneralRet != "")
            {
                for (int i = 0; i < chkListRevision.Items.Count; i++)
                {
                    if (!chkListRevision.GetItemChecked(i))
                    {
                        //MessageBox.Show("No se han completado los check de revisión.", "validaCheckRevision", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Control.Common.General.GetMensajeToList(498);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "validaCheckRevision", "Falta realizar validación de comprobante.");
                        return false;
                    }
                }
                if(chkRevisionG.Checked == false)
                {
                    Control.Common.General.GetMensajeToList(499);
                    //MessageBox.Show("No ha seleccionado el check de Responsabilidad de revisión.", "validaCheckRevision", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "validaCheckRevision", "Falta check de responsabilidad de revisión.");
                    return false;
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "validaCheckRevision", "No se valida check de revisión.");
                valida = true;
            }
            return valida;
        }
        private void GetRetencion175(core_factura factura  )
        {
            decimal subtotalRet1 = 0, subtotalRet175 = 0;
            decimal dsctoRet1 = 0, dsctoRet175 = 0;
            
            try
            {
                if (factura.fecha_creacion >= dtFechaInicioVigenciaRteFte175)
                {
                    using (POSEntities db = new POSEntities())
                    {
                        if (db.core_facturadetalle.Any(x => x.factura_id == factura.id))
                        {
                            var detalle = (from d in db.core_facturadetalle
                                           join pi in db.pos_item on d.item_id equals pi.ITEMID
                                           where d.factura_id == factura.id
                                           select new { Item = d, PosItem = pi });
                            //foreach(var i in detalle)
                            //{
                            //    subtotalRteFte1 += i.PosItem.retporc == 1 ? i.Item.subtotal : 0;
                            //    subtotalRteFte175 += i.PosItem.retporc == 1.75M ? i.Item.subtotal : 0;
                            //}
                            try
                            {
                                subtotalRteFte1 = detalle.Where(x => x.Item.subtotal > 0 && x.PosItem.retporc == 1).Sum(x => x.Item.subtotal);
                                
                            }
                            catch (Exception)
                            {

                                subtotalRteFte1 = 0;
                            }

                            try
                            {
                                dsctoRet1 = detalle.Where(x => x.Item.subtotal > 0 && x.PosItem.retporc == 1).Sum(x => x.Item.descuento);
                            }
                            catch (Exception)
                            {

                                dsctoRet1 = 0; ;
                            }


                            try
                            {
                                subtotalRteFte175 = detalle.Where(x => x.Item.subtotal > 0 && x.PosItem.retporc == 1.75M).Sum(x => x.Item.subtotal);
                            }
                            catch (Exception)
                            {

                                subtotalRteFte175 = 0;
                            }

                            try
                            {
                                dsctoRet175 = detalle.Where(x => x.Item.subtotal > 0 && x.PosItem.retporc == 1.75M).Sum(x => x.Item.descuento);
                            }
                            catch (Exception)
                            {
                                dsctoRet175 = 0;
                            }
                            
                            
                            subtotalRteFte1 = subtotalRteFte1 - dsctoRet1;
                            subtotalRteFte175 = subtotalRteFte175 - dsctoRet175;
                            txtRet175Input.Enabled = true;

                        }
                    }
                }
                else
                {
                    subtotalRteFte1 = 0;
                    subtotalRteFte175 = 0;
                    txtRet175Input.Enabled = false;
                }

                txtSubtotal1.Text = subtotalRteFte1.ToString();
                txtsubtotal175.Text = subtotalRteFte175.ToString();



            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private bool validaCajasTextoRetencion()
        {
            decimal _retFte, _retIva;
            decimal _retFteInput, _retIvaInput;
            bool _ret = true;
            

            _retFteInput = Convert.ToDecimal(txtRetInput.Text.Replace("$", string.Empty));
            _retIvaInput = Convert.ToDecimal(txtRetIVAInput.Text.Replace("$", string.Empty));

            _retFte = Convert.ToDecimal(txtRet.Text);
            _retIva = Convert.ToDecimal(txtRetIVA.Text);

            if (_retFteInput> _retFte && _retFteInput !=(_retFte+ olguraValRet))
            {
                //Control.Common.WinForm.ShowMessage("Valores de Retención no corresponden a lo validado por el sistema, favor revise los valores. ");
                Control.Common.General.GetMensajeToList(500);
                _ret = false;
            }

            if (_retFteInput < _retFte && _retFteInput != (_retFte - olguraValRet))
            {
                //Control.Common.WinForm.ShowMessage("Valores de Retención no corresponden a lo validado por el sistema, favor revise los valores. ");
                Control.Common.General.GetMensajeToList(500);
                _ret = false;
            }

            if (rbtnRUC_NoObligContabilidad.Checked)
            {
                if(_retIvaInput!=0)
                {
                    //Control.Common.WinForm.ShowMessage("Valores de Retención no corresponden a lo validado por el sistema, favor revise los valores. ");
                    Control.Common.General.GetMensajeToList(500);
                    _ret = false;
                }
            }
            else
            {
                if (_retIvaInput > _retIva && _retIvaInput != (_retIva + olguraValRet))
                {
                    //Control.Common.WinForm.ShowMessage("Valores de Retención no corresponden a lo validado por el sistema, favor revise los valores. ");
                    Control.Common.General.GetMensajeToList(500);
                    _ret = false;
                }

                if (_retIvaInput < _retIva && _retIvaInput != (_retIva - olguraValRet))
                {
                    //Control.Common.WinForm.ShowMessage("Valores de Retención no corresponden a lo validado por el sistema, favor revise los valores. ");
                    Control.Common.General.GetMensajeToList(500);
                    _ret = false;
                }
            }

            return _ret;
        }

        /*
        1.Crear una validación que no permita realizar una devolución de dinero cuando la fecha de retención es posterior al periodo contable del mes de diciembre 31 (incluir para retenciones electronicas ) 
        2.Si el cliente presenta en el mes siguiente una retención valida (si se encuentra en el periodo) los diarios que se generen deben ser con fecha 31 de diciembre del año anterior (incluir para retenciones electronicas ) .
        */
        private bool validaFechaRetencionDiciembre()
        {
            bool _ret;
            _ret= true;

            if (DateTime.Now.Month == 1 )
            {

            }

            return _ret;

        }

        /// <summary>
        /// Llena el Objeto Retención con los datos ingresados al Formulario.
        /// Evelasco 219-10-17
        /// </summary>
        /// <param name="numeroret"></param>
        private void SetObjRetencion(string numeroret)
        {
            decimal _retFte = 0, _retFte175 = 0, _retIva = 0;
            try
            {
                

                _retFte = string.IsNullOrEmpty(txtRetInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetInput.Text.Replace("$", ""));
                _retFte175 = string.IsNullOrEmpty(txtRet175Input.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRet175Input.Text.Replace("$", ""));
                _retIva = string.IsNullOrEmpty(txtRetIVAInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetIVAInput.Text.Replace("$", ""));

                string numeroFactura = txtEstabFact.Text.Replace("_", "").PadLeft(3, '0') + txtPtoEmiFact.Text.Replace("_", "").PadLeft(3, '0') + txtNumeracionFact.Text.Replace("_", "").PadLeft(9, '0');
                _objRetFisica.NumAutorizacion = txtAut.Text.Replace("_", "").PadLeft(10, '0');
                _objRetFisica.FechaAutorizacion = cmbFecAut.Value;
                _objRetFisica.NumFactura = numeroFactura;
                _objRetFisica.NumRetencion = numeroret;
                _objRetFisica.ValorBase = Convert.ToDecimal(txtSubTot.Text);
                //_objRetFisica.ValorRetFte = decimal.Parse(txtRetInput.Text); 
                _objRetFisica.ValorRetFte = _retFte;
                _objRetFisica.ValorRetFte175 = _retFte175;
                _objRetFisica.TipoRetencion = 0;
                var db = new POSEntities();
                var objRet = db.vw_RETIVA.Where(x => x.CONCEPT == cmbRetIva.Text).First();


                _objRetFisica.CodRetIVA = objRet.CODE;
                _objRetFisica.CodPorcRetIVA = objRet.RETENTION;
                _objRetFisica.ValorBaseIVA = decimal.Parse(txtIva.Text);
                //_objRetFisica.ValorRetIVA = string.IsNullOrEmpty(txtRetIVAInput.Text) ? 0 : decimal.Parse(txtRetIVA.Text);
                _objRetFisica.ValorRetIVA = _retIva;
                _objRetFisica.ConceptoRetIVA = objRet.CONCEPT;
                _objRetFisica.Customer = CustomerPOS;
                _objRetFisica.ConceptoTransaccion = "Devolución de Retención Fisica";
                _objRetFisica.FechaEmision = cmbFecReg.Value;
                //_objRetFisica.Total = Convert.ToDecimal(txtTot.Text);
                // _objRetFisica.Total = decimal.Parse(txtRet.Text) + (string.IsNullOrEmpty(txtRetIVA.Text) ? 0 : decimal.Parse(txtRetIVA.Text));
                _objRetFisica.Total = _retFte + _retIva+_retFte175;

            }
            catch (Exception)
            {
                throw;
            }
        }
        private void GetCustomer(string cedula)
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var query = db.pos_customer.Where(x => x.ACCOUNTNUM == cedula).FirstOrDefault();
                    CustomerPOS = query;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "GetCustomer", "No se pudo obtener la infromación del cliente, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }
        private void CargarParametros()
        {
            bool parametrosCargados = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {                    
                   var recibo = db.core_recibo.Where(x => x.identificador == "RECIBO_RETENCION_FISICA").FirstOrDefault();
                   if (recibo == null)
                    {
                        throw new Exception("No hay recibo 'RECIBO_RETENCION_FISICA' en core_recibo");
                    }
                   
                    _objRetFisica.Recibo = recibo.cuerpo;
                    if (db.core_parametro.Any(x=> x.identificador =="NUM_DIAS_DEVOLUCION_RETFISICA"))
                    {
                        var coreParam = db.core_parametro.Where(x => x.identificador == "NUM_DIAS_DEVOLUCION_RETFISICA").FirstOrDefault();
                        try
                        {
                            NumeroDiasDevoluDespuesFactura = Convert.ToInt32(coreParam.valor);
                        }catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionFisicaDev", "CargarParametros", "No se pudo realizar el cast a valor numerico, verifique el valor del parámetro, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                        }

                    }



                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionFisicaDev", "CargarParametros", "Imposible terminar de cargar parámetros en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                parametrosCargados = false;
            }

            if (!parametrosCargados)
            {
                //Control.Common.WinForm.ShowMessage("No se pudieron cargar todos los parámetros necesarios para el formulario en este momento, esto pudo deberse a una breve interrupción en la comunicación. Vuélvalo a intentar en unos momentos");
                Control.Common.General.GetMensajeToList(501);
                this.Close();
            }
        }

        public void LimpiarPantalla(int opcion =0)
        {
            if (opcion ==0)
            {
               /* txtEstab.Clear();
                txtPtoEmi.Clear();
                txtNumeracion.Clear();
                txtCedula.Clear();*/
            }

            if(opcion ==1)
            {
                //txtFactura.Clear();
                txtEstabFact.Clear();
                txtPtoEmiFact.Clear();
                txtNumeracionFact.Clear();
            }

             txtEstab.Clear();
                txtPtoEmi.Clear();
                txtNumeracion.Clear();
                txtCedula.Clear();

            cmbFecAut.SetToNullValue();
            txtAut.Clear();
            cmbFecReg.SetToNullValue();
            //gridItems.Rows.Clear();
            //txtTotal.Text = "";
            //txtFactura.Text = "";

            txtValor.Clear();
            txtDscto.Clear();
            txtSubTot.Clear();
            txtIva.Clear();
            txtRet.Clear();
            txtRetInput.Clear();
            txtRetIVA.Clear();
            txtRetIVAInput.Clear();
            txtTotalRet.Clear();
            txtTot.Clear();
            CustomerPOS = null;
            //txtCedula.Clear();
            lblNombre.Text= string.Empty;
            _factura = null;
            
        }

        private bool CargarRetencion(string numRet )
        {
            using (POSEntities db = new POSEntities())
            {
                //Valida que no haya sido ingresado la misma retención anteriormente.
                if (db.core_retencion.Any(x => x.num_retencion == numRet && x.TipoRetencion == 0))
                {
                    //MessageBox.Show("El número de Factura ingresado no existe o no es el correcto, favor ingrese una factura válida.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    var retencionFisica = db.core_retencion.Where(x => x.num_retencion == numRet && x.TipoRetencion == 0).FirstOrDefault();
                    //retencionFisica.core_factura.
                    // MessageBox.Show("Esta factura ya se encuentra registrada en la retención '" + retencionFisica.num_retencion + "'.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    llenaObjRetencionFisicaConsulta(retencionFisica);
                    btnOk.Enabled = false;
                    //factura = null;
                    return true;
                }
                else
                {
                    //MessageBox.Show("Esta factura ya se encuentra registrada en la retencion '" + retencionFisica.num_retencion + "'.", "Devolución Retenciones Fisicas - POS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //Control.Common.WinForm.ShowMessage("La Retención no existe, o no pertenece a este cliente");
                    Control.Common.General.GetMensajeToList(502);
                    LimpiarPantalla();
                    return false;
                }
            }

            }

        private void llenaObjRetencionFisicaConsulta(core_retencion ret)
        {
            

            //txtFactura.Text = ret.core_factura.establecimiento + ret.core_factura.punto_emision + ret.core_factura.numero.ToString();

            txtEstabFact.Text = ret.core_factura.establecimiento;
            txtPtoEmiFact.Text = ret.core_factura.punto_emision;
            txtNumeracionFact.Text = ret.core_factura.numero.ToString();


            txtCedula.Text = ret.core_factura.cliente;
            lblNombre.Text = ret.core_factura.razon_social;
            GetCustomer(txtCedula.Text);

            txtAut.Text= ret.num_autorizacion;
            //cmbFecAut.Value;
            
             txtSubTot.Text=ret.valor_base.ToString();
             txtRet.Text= ret.valor_ret_fte.ToString();

           /* var db = new POSEntities();
            var objRet = db.vw_RETIVA.Where(x => x.CONCEPT == cmbRetIva.Text).First();
            */

            //_objRetFisica.CodRetIVA = objRet.CODE;
            //_objRetFisica.CodPorcRetIVA = objRet.RETENTION;
            txtIva.Text=ret.ValorBaseIVA.ToString();
            txtRetIVA.Text =ret.ValorRetIVA.ToString();
            //_objRetFisica.ConceptoRetIVA = objRet.CONCEPT;
            //_objRetFisica.Customer = CustomerPOS;
            //_objRetFisica.ConceptoTransaccion = "Devolución de Retención Fisica";
            cmbFecReg.Value=  ret.fecha_creacion;
            cmbFecAut.Value = ret.fecha_autorizacion;
            txtTot.Text = ret.core_factura.total.ToString();
            
            txtValor.Text = Math.Round(ret.core_factura.subtotal, 2).ToString();// .getSubTotalSinDescuento().ToString();
            txtDscto.Text = Math.Round(ret.core_factura.descuento, 2).ToString(); // GetDescuentos().ToString();
            txtSubTot.Text = Math.Round(ret.core_factura.subtotal, 2).ToString(); //_factura.getSubTotal().ToString();
            txtIva.Text = Math.Round(ret.core_factura.iva, 2).ToString(); //_factura.getIVA().ToString();

            using (POSEntities pos = new POSEntities())
            {
                cmbRetIva.DataSource = pos.vw_RETIVA.OrderBy(x => x.RETENTION).ToList();
                //   cmbBancoTarjeta.DataSource = db.core_tarjetacredito.ToList();
                cmbRetIva.DisplayMember = "CONCEPT";
                cmbRetIva.ValueMember = "CODE";
            }
            cmbRetIva.SelectedValue = ret.CodRetIVA;

            SetObjRetencion(ret.num_retencion);



        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var db = new POSEntities();

            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
                case Keys.Tab:
                    if(txtEstab.Focused)
                    {
                        MessageBox.Show("ok");
                    }
                    break;

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtEstab_KeyPress(object sender, KeyPressEventArgs e)
    {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    var establecimiento = txtEstab.Text.Trim().Replace("_", "");
                    txtEstab.Text=txtEstab.Text.Replace("__", "");
                    if (establecimiento.Length <3)
                    {
                        
                        txtEstab.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                        txtEstab.MaskType = Telerik.WinControls.UI.MaskType.None;
                        txtEstab.Clear();
                        txtEstab.Text = establecimiento.PadLeft(3, '0');
                        //txtEstab.MaskType = Telerik.WinControls.UI.MaskType.Standard;
                    }
                    
                    
                    if (txtEstab.Text.Length >= 3)
                    {
                        txtPtoEmi.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtEstab_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
            
            
        }

        private void txtPtoEmi_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            { 
            if (e.KeyChar == (char)Keys.Enter)
            {
                    var ptoEmi = txtPtoEmi.Text.Trim().Replace("_", "");
                    if (ptoEmi.Length < 3)
                    {
                        txtPtoEmi.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                        txtPtoEmi.MaskType = Telerik.WinControls.UI.MaskType.None;
                        txtPtoEmi.Clear();
                        txtPtoEmi.Text = ptoEmi.PadLeft(3, '0');
                       
                    }

                   
                if (txtPtoEmi.Text.Length >= 3)
                {
                    txtNumeracion.Focus();
                }
            }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtPtoEmi_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void txtNumeracion_KeyPress(object sender, KeyPressEventArgs e)
        {
            try { 
                if (e.KeyChar == (char)Keys.Enter)
                {

                    var numeracion = txtNumeracion.Text.Trim().Replace("_", "");
                    
                        txtNumeracion.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                        txtNumeracion.MaskType = Telerik.WinControls.UI.MaskType.None;
                        txtNumeracion.Clear();
                        txtNumeracion.Text = numeracion.PadLeft(9, '0');


                    ValidaNumeroRetencionCliente();
                    txtAut.Focus();                
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtNumeracion_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

         }

        private void FrmRetencionFisicaDev_Shown(object sender, EventArgs e)
        {
            txtEstabFact.Focus();
            CargarParametros();
        }

        private void btnReimprimirRecibo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_objRetFisica.NumRetencion))
                {
                    //Control.Common.WinForm.ShowMessage("Ingrese la retención a reimprimir.");
                    Control.Common.General.GetMensajeToList(502);
                    return;
                }
                using (var db = new POSEntities())
                {
                    var estab = this._objRetFisica.NumFactura.Substring(0, 3);
                    var ptoemi = this._objRetFisica.NumFactura.Substring(3, 3);
                    var fact = long.Parse(this._objRetFisica.NumFactura.Substring(6, 9));
                    var factura = db.core_factura.FirstOrDefault(x => x.establecimiento == estab && x.punto_emision == ptoemi && x.numero == fact && x.cliente == txtCedula.Text);
                    if (factura == null)
                    {
                        //Control.Common.WinForm.ShowMessage("La factura asociada a la retención no existe");
                        Control.Common.General.GetMensajeToList(598);
                    }
                    else
                    {
                        if (db.core_retencion.Any(x => x.num_retencion == this._objRetFisica.NumRetencion && x.factura_id == factura.id))
                        {
                            _objRetFisica.ConceptoTransaccion = "Devolución de Retención Fisica (COPIA)";
                            _objRetFisica.ImprimirReciboFisica();
                        }
                        else {
                            Control.Common.General.GetMensajeToList(503);
                            //Control.Common.WinForm.ShowMessage("La retencion no ha sido devuelta. Por favor realice la devolución");
                        }
                           
                    }
                }
            }
            catch (Exception)
            {
                //Control.Common.WinForm.ShowMessage("No se pudo realizar la reimpresión del comprobante.");
                Control.Common.General.GetMensajeToList(504);
            }
        }

        private void txtFactura_TextChanged(object sender, EventArgs e)
        {
            
            if (!consultaRetencion)
            {
                consultaRetencion = false;
                LimpiarPantalla(0);
            }
        }

        private void btnBuscarRetencion_Click(object sender, EventArgs e)
        {
            string numeroRetencionaConsultar = string.Empty;
            try
            {
                consultaRetencion = true;
                LimpiarPantalla(1);
                //txtClave.Text = "";
                if (string.IsNullOrEmpty(txtCedula.Text))
                {
                    //MessageBox.Show(this, "Ingrese la identificación del cliente");
                    Control.Common.General.GetMensajeToList(505);
                    txtCedula.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtEstab.Text) || string.IsNullOrEmpty(txtPtoEmi.Text) || string.IsNullOrEmpty(txtNumeracion.Text))
                {
                    
                    //MessageBox.Show(this, "Ingrese la retención: establecimiento, punto de emisión y número");
                    Control.Common.General.GetMensajeToList(506);
                    txtEstab.Focus();
                    return;
                }

                if (txtNumeracion.Text.Length < 9)
                    txtNumeracion.Text = txtNumeracion.Text.PadLeft(9, '0');

                numeroRetencionaConsultar = txtEstab.Text + txtPtoEmi.Text + txtNumeracion.Text;

                if (!CargarRetencion(numeroRetencionaConsultar))
                {
                    txtEstab.Text = "";
                    txtPtoEmi.Text = "";
                    txtNumeracion.Text = "";
                    txtEstab.Focus();
                }
                consultaRetencion = false;
            }
            catch (Exception ex)
            {
                consultaRetencion = false;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "btnBuscarRetencion_Click", "Imposible consultar retención en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos consultar retención en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(507);

            }
        }

        /// <summary>
        /// Valida que no se ingrese mas de una vez un número de retención para el mismo cliente.
        /// </summary>
        private Boolean ValidaNumeroRetencionCliente()
        {
            Boolean _valida = true;
            try
            {
                string numeroret = txtEstab.Text.Replace("_", "").PadLeft(3, '0') + txtPtoEmi.Text.Replace("_", "").PadLeft(3, '0') + txtNumeracion.Text.Replace("_", "").PadLeft(9, '0');
                
                using (POSEntities db = new POSEntities())
                {
                    if(!consultaRetencion)
                    {                    
                        if (db.core_retencion.Any(x => x.core_factura.cliente == txtCedula.Text && x.num_retencion== numeroret))
                        {
                            _valida = false;
                            // btnOk.Enabled = false;
                            //MessageBox.Show("Ya está registrada esta Retención para este cliente.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Control.Common.General.GetMensajeToList(508);
                        }
                    }

                }

                return _valida;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txtEstab_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                MessageBox.Show("Tab");
                e.IsInputKey = true;
            }
        }

        private void txtEstab_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                MessageBox.Show("Tab");
                //e.IsInputKey = true;
            }
        }

        private void txtEstab_TextChanged(object sender, EventArgs e)
        {
            /*var establecimiento = txtEstab.Text.Trim().Replace("_", "");
            txtEstab.Text = txtEstab.Text.Replace("__", "");
            if (establecimiento.Length < 3)
            {

                txtEstab.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                txtEstab.MaskType = Telerik.WinControls.UI.MaskType.None;
                txtEstab.Clear();
                txtEstab.Text = establecimiento.PadLeft(3, '0');
                //txtEstab.MaskType = Telerik.WinControls.UI.MaskType.Standard;
            }*/

        }

        private void txtEstabFact_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    var establecimiento = txtEstabFact.Text.Trim().Replace("_", "");
                    txtEstabFact.Text = txtEstabFact.Text.Replace("__", "");
                    if (establecimiento.Length < 3)
                    {

                        txtEstabFact.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                        txtEstabFact.MaskType = Telerik.WinControls.UI.MaskType.None;
                        txtEstabFact.Clear();
                        txtEstabFact.Text = establecimiento.PadLeft(3, '0');
                        //txtEstab.MaskType = Telerik.WinControls.UI.MaskType.Standard;
                    }


                    if (txtEstabFact.Text.Length >= 3)
                    {
                        txtPtoEmiFact.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtEstab_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

        }

        private void txtPtoEmiFact_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    var ptoEmi = txtPtoEmiFact.Text.Trim().Replace("_", "");
                    if (ptoEmi.Length < 3)
                    {
                        txtPtoEmiFact.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                        txtPtoEmiFact.MaskType = Telerik.WinControls.UI.MaskType.None;
                        txtPtoEmiFact.Clear();
                        txtPtoEmiFact.Text = ptoEmi.PadLeft(3, '0');

                    }


                    if (txtPtoEmiFact.Text.Length >= 3)
                    {
                        txtNumeracionFact.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtPtoEmi_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void txtNumeracionFact_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {

                     var numeracion = txtNumeracionFact.Text.Trim().Replace("_", "");

                         txtNumeracionFact.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                         txtNumeracionFact.MaskType = Telerik.WinControls.UI.MaskType.None;
                         //txtNumeracionFact.Text
                         txtNumeracionFact.Text = numeracion.PadLeft(9, '0');

                    GetDatosFactura();

                    txtEstab.Focus();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtNumeracion_KeyPress", "Ha ocurrido una excepción al presionar Enter, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void txtNumeracionFact_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!consultaRetencion)
                {
                    consultaRetencion = false;
                    LimpiarPantalla(0);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtNumeracionFact_TextChanged", "Ha ocurrido una excepción al Buscar la Factura, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                GetDatosFactura();

               /* if(consultaRetencion)
                {
                    btnOk.Enabled = false;
                }*/
                
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "btnBuscar_Click", "Ha ocurrido una excepción al Buscar la Factura, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
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
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "rbtnRucPersonNat_CheckedChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
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
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "rbtnRUC_NoObligContabilidad_CheckedChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
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
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "radioButton3rbtn_RUCObligContab_CheckedChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));                
            }
        }

        private void txtRetInput_TextChanged(object sender, EventArgs e)
        {
            decimal _retFte, _retFte175, _rtIva;
            try
            {
                _retFte = string.IsNullOrEmpty(txtRetInput.Text.Replace("$","")) ? 0 :Convert.ToDecimal(txtRetInput.Text.Replace("$", ""));
                _retFte175 = string.IsNullOrEmpty(txtRet175Input.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRet175Input.Text.Replace("$", ""));
                _rtIva = string.IsNullOrEmpty(txtRetIVAInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetIVAInput.Text.Replace("$", ""));
                txtTotalRetInput.Text = (_retFte + _rtIva+ _retFte175).ToString();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtRetInput_TextChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

        }

        private void txtRetIVAInput_TextChanged(object sender, EventArgs e)
        {
            decimal _retFte, _retFte175, _rtIva;
            try
            {
                _retFte = string.IsNullOrEmpty(txtRetInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetInput.Text.Replace("$", ""));
                _retFte175 = string.IsNullOrEmpty(txtRet175Input.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRet175Input.Text.Replace("$", ""));
                _rtIva = string.IsNullOrEmpty(txtRetIVAInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetIVAInput.Text.Replace("$", ""));
                txtTotalRetInput.Text = (_retFte + _rtIva + _retFte175).ToString();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtRetIVAInput_TextChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void txtRet175Input_TextChanged(object sender, EventArgs e)
        {
            decimal _retFte, _retFte175, _rtIva;
            try
            {
                _retFte = string.IsNullOrEmpty(txtRetInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetInput.Text.Replace("$", ""));
                _retFte175 = string.IsNullOrEmpty(txtRet175Input.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRet175Input.Text.Replace("$", ""));
                _rtIva = string.IsNullOrEmpty(txtRetIVAInput.Text.Replace("$", "")) ? 0 : Convert.ToDecimal(txtRetIVAInput.Text.Replace("$", ""));
                txtTotalRetInput.Text = (_retFte + _rtIva + _retFte175).ToString();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "txtRetIVAInput_TextChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void cmbFecReg_ValueChanged(object sender, EventArgs e)
        {
            DateTime _dtFechaMaxRecepcionRet;
            try
            {
                if(cmbFecReg.Value != _gdtNullDate && !consultaRetencion)
                {
                    if (_gretAnioAnterior && cmbFecReg.Value.Date != _gdtUltDiaAnio && cmbFecReg.Value.Date!= _gdtNullDate)
                    {
                        //Control.Common.WinForm.ShowMessage("La Retención no existe, o no pertenece a este cliente");
                        //MessageBox.Show("La Fecha de Retención no es válida, se rechaza retención.", "Devolución de Retenciones Fisicas",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        Control.Common.General.GetMensajeToList(509);
                        btnOk.Enabled = false;
                    }

                    

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

                    if (_factura!=null)

                    {
                        _dtFechaMaxRecepcionRet = _factura.Fecha.AddDays(NumeroDiasDevoluDespuesFactura);
                        if (cmbFecReg.Value.Date > _dtFechaMaxRecepcionRet.Date)
                        {
                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[NumeroDiasDevoluDespuesFactura]", valor = NumeroDiasDevoluDespuesFactura.ToString()  });
                            parametros.Add(new ParametrosMensajes() { codigo = "[fecha_emision]", valor = _factura.Fecha.Date.ToString("dd/MM/yyyy") });
                            Control.Common.General.GetMensajeToList(510, parametros);

                            //MessageBox.Show("La Fecha de Retención excede los " + NumeroDiasDevoluDespuesFactura.ToString()+ " días de la Fecha de Emisión de la Factura "+_factura.Fecha.Date.ToString("dd/MM/yyyy")+", se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            btnOk.Enabled = false;
                        }
                        else if(cmbFecReg.Value.Date < _factura.Fecha.Date)
                        {

                            parametros = new List<ParametrosMensajes>();                           
                            parametros.Add(new ParametrosMensajes() { codigo = "[fecha_emision]", valor = _factura.Fecha.Date.ToString("dd/MM/yyyy") });
                            Control.Common.General.GetMensajeToList(511, parametros);

                            //MessageBox.Show("La Fecha de Retención no puede ser inferior a la fecha de Emisión de la factura [" + _factura.Fecha.Date.ToString("dd/MM/yyyy") + "], se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            btnOk.Enabled = false;

                        }
                        else if (cmbFecReg.Value.Date > DateTime.Now.Date)
                        {
                            //MessageBox.Show("La Fecha de Retención no puede ser superior a la fecha Actual, se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Control.Common.General.GetMensajeToList(512);
                            btnOk.Enabled = false;

                        }

                        else
                        {
                            if (!btnOk.Enabled)
                            {
                                btnOk.Enabled = true;
                                if (btnOk.Enabled == true && validaCheckRevision() == true)
                                {
                                    btnOk.Enabled = true;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "cmbFecReg_ValueChanged", "Todos los check de validacion estan completos.");
                                }
                                else
                                {
                                    btnOk.Enabled = false;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "cmbFecReg_ValueChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private void cmbFecAut_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbFecAut.Value.Date > DateTime.Now.Date)
                {
                    //MessageBox.Show("La fecha de Autorización de la retención no puede ser posterior a la fecha Actual, se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(513);
                    btnOk.Enabled = false;
                }
                else
                {
                    if (!btnOk.Enabled && validaFechas())
                    {
                        if(!consultaRetencion)
                        btnOk.Enabled = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "cmbFecAut_ValueChanged", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }

        private bool validaFechas()
        {
            DateTime _dtFechaMaxRecepcionRet;
            bool retorno = true;
            if(!consultaRetencion)
            {                 //Validación en Fecha de Emisión.
                if (_gretAnioAnterior && cmbFecReg.Value.Date != _gdtUltDiaAnio && cmbFecReg.Value.Date != _gdtNullDate)
                {
                    //Control.Common.WinForm.ShowMessage("La Retención no existe, o no pertenece a este cliente");
                    //MessageBox.Show("La Fecha de Retención no es válida, se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(513);
                    retorno = false;
                }

                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

                if (_factura != null)
                {
                    _dtFechaMaxRecepcionRet = _factura.Fecha.AddDays(NumeroDiasDevoluDespuesFactura);
                    if (cmbFecReg.Value.Date > _dtFechaMaxRecepcionRet.Date)
                    {
                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[NumeroDiasDevoluDespuesFactura]", valor = NumeroDiasDevoluDespuesFactura.ToString() });
                        parametros.Add(new ParametrosMensajes() { codigo = "[fecha_emision]", valor = _factura.Fecha.Date.ToString("dd/MM/yyyy") });
                        Control.Common.General.GetMensajeToList(510, parametros);

                        //MessageBox.Show("La Fecha de Retención excede los " + NumeroDiasDevoluDespuesFactura.ToString() + " días de la Fecha de Emisión de la Factura " + _factura.Fecha.Date.ToString("dd/MM/yyyy") + ", se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        retorno = false;
                    }
                    else if (cmbFecReg.Value.Date < _factura.Fecha.Date)
                    {
                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[fecha_emision]", valor = _factura.Fecha.Date.ToString("dd/MM/yyyy") });
                        Control.Common.General.GetMensajeToList(511, parametros);

                        //MessageBox.Show("La Fecha de Retención no puede ser inferior a la fecha de Emisión de la factura [" + _factura.Fecha.Date.ToString("dd/MM/yyyy") + "], se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        retorno = false;

                    }
                    else if (cmbFecReg.Value.Date > DateTime.Now.Date)
                    {
                        //MessageBox.Show("La Fecha de Retención no puede ser superior a la fecha Actual, se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Control.Common.General.GetMensajeToList(512);
                        retorno = false;

                    }



                }

                //Validación en Fecha de Autorización.
                if (cmbFecAut.Value.Date > DateTime.Now.Date)
                {
                    //MessageBox.Show("La fecha de Autorización de la retención no puede ser posterior a la fecha Actual, se rechaza retención.", "Devolución de Retenciones Fisicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(513);
                    retorno = false;
                }

            }
            return retorno;
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
                                MessageBox.Show("No se han completado los check de revisión.", "validaCheckRevision", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "chkRevisionG_CheckedChanged", "Falta realizar validación de comprobante.");
                                btnOk.Enabled = false;
                                chkRevisionG.Checked = false;
                                break;
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "chkRevisionG_CheckedChanged", "Estan completos los Check de revisión.");
                                btnOk.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "chkRevisionG_CheckedChanged", "Falta check de responsabilidad de revisión.");
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
                                //MessageBox.Show("No se han completado los check de revisión.", "validaCheckRevision", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Control.Common.General.GetMensajeToList(515);

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "chkRevisionG_CheckedChanged", "Falta realizar validación de comprobante.");
                                btnOk.Enabled = false;
                                chkRevisionG.Checked = false;
                                break;
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "chkRevisionG_CheckedChanged", "Estan completos los Check de revisión.");
                                btnOk.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "FrmRetencionFisicaDev", "chkRevisionG_CheckedChanged", "Falta check de responsabilidad de revisión.");
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
                        if(indice == tipo)
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

        private void txtNumeracionFact_Click(object sender, EventArgs e)
        {

        }
    }    
}
