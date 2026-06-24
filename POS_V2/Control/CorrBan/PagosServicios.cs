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
using System.Xml.Linq;
using System.Xml;
using POS.Control.Common;

namespace POS.Control.CorrBan
{
    public partial class PagosServicios : Telerik.WinControls.UI.RadForm
    {

        #region Constructores

        public PagosServicios()
        {
            InitializeComponent();
        }

        #endregion

        #region Atributos Privados

        System.Windows.Forms.Control focused;
        private bool _mustReQuery = false;

        #endregion

        #region Propiedades Privadas

        private pos_customer SelectedCustomer { get; set; }
        private List<Models.CorrBan.LikeAccount> ListaCuentasSimilares { get; set; }
        public BindingList<Models.CorrBan.CustAccount> CuentasBinding { get; protected set; }

        #endregion

        #region Metodos Controles

        private void PagosServicios_Load(object sender, EventArgs e)
        {
            CuentasBinding = new BindingList<Models.CorrBan.CustAccount>();
            gridPagos.DataSource = CuentasBinding;

            InitForm();

            //Si no puede realizar el ping, cerrar el formulario
            if (!ClsCorrBan.RealizarPing(14))
                this.Close();

            if (!ClsCorrBan.ValidarAperturaCaja())
                this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnKbd_Click(object sender, EventArgs e)
        {
            //OpenKeyboard();
            Control.Common.General.TecladoPantalla();
        }

        private void txtControl_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
        }

        private void cmbInstRecaudo_SelectedValueChanged(object sender, EventArgs e)
        {
            LimpiarBase();
            LoadSubTypes();
            ChangeCaptionCriteriaLabels();
            ChangeLogo();

            if (cmbTipoPago.Enabled)
                cmbTipoPago.Focus();
            else
                txtCuenta.Focus();

        }

        private void cmbTipoPago_SelectedValueChanged(object sender, EventArgs e)
        {
            LimpiarBase();
            ChangeCaptionCriteriaLabels();
            txtCuenta.Focus();
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Only numbers
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            //Only decimal point
            if ((e.KeyChar == '.') && ((sender as Telerik.WinControls.UI.RadTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            //Only allow up to two decimal places
            if (Char.IsNumber(e.KeyChar) || e.KeyChar == '.')
            {
                Telerik.WinControls.UI.RadTextBox tb = sender as Telerik.WinControls.UI.RadTextBox;

                int cursorPosLeft = tb.SelectionStart;
                int cursorPosRight = tb.SelectionStart + tb.SelectionLength;
                string result = tb.Text.Substring(0, cursorPosLeft) + e.KeyChar + tb.Text.Substring(cursorPosRight);
                string[] parts = result.Split('.');

                if (parts.Length > 1)
                {
                    if (parts[1].Length > 2 || parts.Length > 2)
                    {
                        e.Handled = true;
                    }
                }
            }

        }

        private void txtPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Only numbers
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            //Only decimal point
            if ((e.KeyChar == '.') && ((sender as Telerik.WinControls.UI.RadTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            //Only allow up to two decimal places
            if (Char.IsNumber(e.KeyChar) || e.KeyChar == '.')
            {
                Telerik.WinControls.UI.RadTextBox tb = sender as Telerik.WinControls.UI.RadTextBox;

                int cursorPosLeft = tb.SelectionStart;
                int cursorPosRight = tb.SelectionStart + tb.SelectionLength;
                string result = tb.Text.Substring(0, cursorPosLeft) + e.KeyChar + tb.Text.Substring(cursorPosRight);
                string[] parts = result.Split('.');

                if (parts.Length > 1)
                {
                    if (parts[1].Length > 2 || parts.Length > 2)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            ProcesarConsultaBackGround();
        }

        private void PagosServicios_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (EstadoControles == false) e.Cancel = true;
        }

        private void btnRealizarPago_Click(object sender, EventArgs e)
        {
            ProcesarGuardarBackGround();
        }

        #endregion

        #region Metodos Privados

        private void InitForm()
        {
            try
            {
                cmbInstRecaudo.DisplayMember = "NombreEntidad";
                cmbInstRecaudo.ValueMember = "IdIRecaudos";
                cmbInstRecaudo.DataSource = Control.CorrBan.ClsCorrBan.ListInstRecaudadoras;

                EstadoControles = true;

                CalcularCambio();
            }
            catch (Exception ex)
            {

            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var db = new POSEntities();

            switch (keyData)
            {
                case Keys.Escape:
                    InitForm();
                    this.Close();
                    

                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void LoadSubTypes()
        {
            cmbTipoPago.DisplayMember = "TipoServicio";
            cmbTipoPago.ValueMember = "IdTipoServicio";
            var listaTipoPagos = LirisLibCorrBan.Business.InstRecaudoBO.GetSubtypesRecaudos((short)cmbInstRecaudo.SelectedValue);
            cmbTipoPago.DataSource = listaTipoPagos;

            cmbTipoPago.Enabled = (listaTipoPagos.Count > 1);
        }

        private void ChangeCaptionCriteriaLabels()
        {
            if (cmbTipoPago.SelectedItem != null) lblCampoPrimario.Text = ((LirisLibCorrBan.Models.ReglasRecaudos)cmbTipoPago.SelectedItem.DataBoundItem).CampoObliga;
            var instRecaudo = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem);
            if (instRecaudo.CodigoEmpresa == "00")
                lblPermiteAbono.Text = "";
            else
            {
                lblPermiteAbono.ForeColor = instRecaudo.AplicaAbono ? Color.LimeGreen : Color.DarkRed;
                lblPermiteAbono.Text = instRecaudo.AplicaAbono ? "Esta institución permite abono" : "Esta institución NO permite abono";
            }
        }

        private void OpenKeyboard()
        {
            Control.Common.General.TecladoPantalla();

            //var textBox = focused as Telerik.WinControls.UI.RadTextBox;
            //if (textBox != null)
            //{
            //    KeyboardControl kbd = new KeyboardControl(textBox, this.Text);
            //    kbd.Top = this.Height + this.Top - 100;
            //    kbd.Left = this.Left;
            //    kbd.ShowDialog();
            //}
        }

        private bool ValidarSolicitudConsulta()
        {
            if (cmbInstRecaudo.SelectedIndex == 0)
            {
                //MessageBox.Show("No ha seleccionado la institución");
                Control.Common.General.GetMensajeToList(340);
                cmbInstRecaudo.Focus();
                return false;
            }
            if ((string)cmbTipoPago.SelectedValue == "-1")
            {
                //MessageBox.Show("No ha seleccionado el tipo de pago");
                Control.Common.General.GetMensajeToList(341);
                cmbTipoPago.Focus();
                return false;
            }
            if (txtCuenta.Text.Trim().Length == 0 && txtNombres.Text.Trim().Length == 0)
            {
                //MessageBox.Show("No ha indicado nro de cuenta o nombres para realizar la consulta");
                Control.Common.General.GetMensajeToList(342);
                txtCuenta.Focus();
                return false;
            }

            return true;
        }

        private bool EstadoControles { get; set; }
        private void On_Off_Controles(bool estado)
        {
            btnConsultar.Enabled = estado;
            EstadoControles = estado;
            cmbInstRecaudo.Enabled = estado;
            cmbTipoPago.Enabled = estado;
            txtCuenta.Enabled = estado;
            txtValor.Enabled = estado;
            txtPago.Enabled = estado;
            btnRealizarPago.Enabled = estado;
            btnLimpiar.Enabled = estado;
            btnCF.Enabled = estado;
            btnCambiarCliente.Enabled = estado;
            btnKbd.Enabled = estado;
        }

        #region Hilo QueryCollection

        private string ResponseConsulta { get; set; }
        BackgroundWorker bgwConsulta;
        private void ProcesarConsultaBackGround()
        {
            if (!ValidarSolicitudConsulta()) return;

            paneLoading.Visible = true;

            if (bgwConsulta == null)
            {
                bgwConsulta = new BackgroundWorker();
                bgwConsulta.DoWork += new DoWorkEventHandler(bgwConsulta_DoWork);
                bgwConsulta.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwbgwConsulta_RunWorkerCompleted);
            }
            bgwConsulta.WorkerReportsProgress = true;
            bgwConsulta.WorkerSupportsCancellation = true;
            bgwConsulta.RunWorkerAsync();
            System.Threading.Thread.Sleep(500);
        }
        void bgwConsulta_DoWork(object sender, DoWorkEventArgs e)
        {
            RealizarConsulta();
        }

        void bgwbgwConsulta_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            paneLoading.Visible = false;
            txtValor.Focus();
            LimpiarSeccionPago();

            if (!string.IsNullOrWhiteSpace(ResponseConsulta)) MessageBox.Show(ResponseConsulta);

            if (_mustReQuery)
            {
                _mustReQuery = false;
                ResponseConsulta = string.Empty;

                CorrBan.MoreAccounts frmMore = new MoreAccounts(ListaCuentasSimilares);
                frmMore.ShowDialog();

                if (frmMore.SelectedAccount != null)
                {
                    txtCuenta.Text = frmMore.SelectedAccount.Account.Trim();
                    txtNombres.Clear();

                    ProcesarConsultaBackGround();
                }

                frmMore.Dispose();
                ListaCuentasSimilares = null;
            }
        }

        private void RealizarConsulta()
        {
            On_Off_Controles(false);
            try
            {
                int idMetodo = 2;

                var soapDetail = LirisLibCorrBan.Business.SoapCorrBanBO.GetSoapDetail(idMetodo);

                var xmlWithValues = GetXmlWithValues(soapDetail.SoapLiris, idMetodo);


                if (Common.XmlHelper.ValidateXml(xmlWithValues, soapDetail.XsdLiris))
                {
                    //TipoTransaccion: 12 - EmpresaRecarga en CatalogoDet
                    //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
                    var responseXml = LirisLibCorrBan.Business.ProceduresBO.ProcesaRecaudacion(
                                                                            xmlWithValues
                                                                          );

                    if (string.IsNullOrEmpty(responseXml)) responseXml = string.Empty;

                    if (Common.XmlHelper.IsMinimallyValidXml(responseXml))
                    {
                        XDocument xDoc = XDocument.Parse(responseXml);
                        //El xml respuesta viene con namespaces propios de soap, entonces para leer su contenido debemos proveer 
                        //estos namespaces a los queries
                        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                        XNamespace srr = "http://transferunion.org/";

                        var responseObj = (from d in xDoc.Descendants(soap + "Body").Descendants(srr + "QueryCollectionResponse").Descendants(srr + "QueryCollectionResult")
                                           select new
                                           {
                                               CarCoeId = d.Element(srr + "CarCoeId").Value,
                                               CarCuenta = d.Element(srr + "CarCuenta").Value,
                                               CarDate = d.Element(srr + "CarDate").Value,
                                               ErrNumber = d.Element(srr + "ErrNumber").Value.PadLeft(4, '0'),
                                               ErrDescription = d.Element(srr + "ErrDescription").Value,
                                               CarFactura = d.Element(srr + "CarFactura").Value,
                                               Sucess = d.Element(srr + "Sucess").Value,
                                               CarSequential = d.Element(srr + "CarSequential").Value,
                                               CarSequentialSARA = d.Element(srr + "CarSequentialSARA").Value,
                                               CarAutorizacion = d.Element(srr + "CarAutorizacion").Value,
                                               CarSecurity = d.Element(srr + "CarSecurity").Value,
                                               CarNames = d.Element(srr + "CarNames").Value,
                                               CarAdress = d.Element(srr + "CarAdress").Value,
                                               CarValuePaid = d.Element(srr + "CarValuePaid").Value,
                                               CarObservation = (d.Element(srr + "CarObservation") == null) ? "" : d.Element(srr + "CarObservation").Value,
                                               CarValueEfective = d.Element(srr + "CarValueEfective").Value,
                                               CarValueTotal = d.Element(srr + "CarValueTotal").Value,
                                               CarValueMin = d.Element(srr + "CarValueMin").Value,
                                               CarValueActivaCharge = d.Element(srr + "CarValueActivaCharge").Value,
                                               CarValuePaySuggested = d.Element(srr + "CarValuePaySuggested").Value,
                                               CarValueDisability = d.Element(srr + "CarValueDisability").Value,
                                               carValueTax = d.Element(srr + "carValueTax").Value,
                                               carValueAditional = d.Element(srr + "carValueAditional").Value,
                                               carValueOthers = d.Element(srr + "carValueOthers").Value,
                                               CarDateVen = d.Element(srr + "CarDateVen").Value,
                                               CarActivacode = d.Element(srr + "CarActivacode").Value,
                                               CarCltid = d.Element(srr + "CarCltid").Value,
                                               CarMoreAccounts = d.Element(srr + "CarMoreAccounts").Value,
                                               CarAccounts = (d.Element(srr + "CarAccounts") == null) ? "" : d.Element(srr + "CarAccounts").Value
                                           }).FirstOrDefault();

                        if (responseObj != null)
                        {
                            if (responseObj.ErrNumber == "0000")
                            {
                                if (responseObj.CarMoreAccounts.Contains("S"))
                                {
                                    ListaCuentasSimilares = CorrBan.ClsCorrBan.TransformarListaCuentasSimilares(responseObj.CarAccounts);

                                    if (ListaCuentasSimilares.Count > 0)
                                    {
                                        _mustReQuery = true;
                                    }
                                    else
                                    {
                                        ResponseConsulta = "Se recibió una respuesta sin resultados. Por favor vuélvalo a intentar";
                                    }
                                }
                                else
                                {
                                    CuentasBinding.Clear();
                                    //var listaCuentas = new List<Models.CorrBan.CustAccount>();
                                    CuentasBinding.Add(new Models.CorrBan.CustAccount
                                    {
                                        AccountCode = responseObj.CarCuenta,
                                        CustName = responseObj.CarNames,
                                        CustAddress = responseObj.CarAdress,
                                        ValuePaid = Decimal.Parse(responseObj.CarValuePaid),
                                        ValueEfective = Decimal.Parse(responseObj.CarValueEfective),
                                        ValueTotal = Decimal.Parse(responseObj.CarValueTotal),
                                        ValueMin = Decimal.Parse(responseObj.CarValueMin),
                                        ValueActivaCharge = Decimal.Parse(responseObj.CarValueActivaCharge),
                                        ValuePaySuggested = Decimal.Parse(responseObj.CarValuePaySuggested),
                                        ValueDisability = Decimal.Parse(responseObj.CarValueDisability),
                                        ValueTax = Decimal.Parse(responseObj.carValueTax),
                                        ValueAditional = Decimal.Parse(responseObj.carValueAditional),
                                        ValueOthers = Decimal.Parse(responseObj.carValueOthers),
                                        ValueMinTotal = (Decimal.Parse(responseObj.CarValueMin) + Decimal.Parse(responseObj.CarValueActivaCharge)),
                                        ValueMaxTotal = (Decimal.Parse(responseObj.CarValueTotal) < (Decimal.Parse(responseObj.CarValueMin) + Decimal.Parse(responseObj.CarValueActivaCharge))) ? (Decimal.Parse(responseObj.CarValueMin) + Decimal.Parse(responseObj.CarValueActivaCharge)) : Decimal.Parse(responseObj.CarValueTotal),
                                        SequSara = responseObj.CarSequentialSARA,
                                        SequCarrier = responseObj.CarSequential
                                    });

                                    txtCuenta.Text = responseObj.CarCuenta;
                                    txtNombres.Clear();

                                    ResponseConsulta = string.Empty;
                                }
                            }
                            else
                            {
                                ResponseConsulta = "No se ha realizado la transacción"
                                                    + Environment.NewLine
                                                    + Environment.NewLine
                                                    + responseObj.ErrDescription
                                                    + Environment.NewLine
                                                    + responseObj.CarObservation;
                            }
                        }
                        else
                        {
                            ResponseConsulta = "El servicio no devolvió la respuesta esperada pero aún puede que la recarga si se haya realizado, por favor contacte a administrador para verificar, si la recarga no se realizó inténtelo otra vez en unos breves momentos";
                        }
                    }
                    else
                    {
                        Logger.LogMessage(Common.Enum.LogTypes.Info, "Recargas", "RealizarRecarga", "Trama de respuesta no esperada",
                                          string.Format("Trama: {0}", responseXml));
                        ResponseConsulta = "El servicio retornó una respuesta no esperada, por favor inténtelo otra vez en unos breves momentos";
                    }

                }
                else
                {
                    Logger.LogMessage(Common.Enum.LogTypes.Info, "Recargas", "RealizarRecarga", "Trama de envío no valida",
                                      string.Format("Trama: {0}", xmlWithValues));
                    ResponseConsulta = "La trama que se quiere enviar no es válida, esto puede deberse a un breve mantenimiento por favor inténtelo de nuevo. La trama fue guardada en el log para que pueda ser revisada";
                }

            }
            catch (Exception ex)
            {
                Logger.LogMessage(Common.Enum.LogTypes.Error, "Recargas", "RealizarRecarga", ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                ResponseConsulta = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a un breve mantenimiento, por favor inténtelo una vez más";
            }
            finally
            {
                On_Off_Controles(true);
            }
        }

        #endregion

        #region Hilo SaveCollection



        private bool ValidarSolicitudPago()
        {
            if (gridPagos.DataSource == null)
            {
                //MessageBox.Show("Antes de realizar pagos debe haber realizado la consulta de cuentas");
                Control.Common.General.GetMensajeToList(343);
                return false;
            }
            if (((IEnumerable<Models.CorrBan.CustAccount>)gridPagos.DataSource).Count() == 0)
            {
                //MessageBox.Show("No existen registros para realizar el pago");
                Control.Common.General.GetMensajeToList(344);
                return false;
            }
            if (gridPagos.SelectedRows.Count == 0)
            {
                //MessageBox.Show("No ha seleccionado un registro o bien el registro no es válido");
                Control.Common.General.GetMensajeToList(345);
                return false;
            }
            if (txtValor.Text.Trim().Replace(".", "").Length == 0)
            {
                //MessageBox.Show("No ha indicado el valor");
                Control.Common.General.GetMensajeToList(346);
                txtValor.Focus();
                return false;
            }
            if (decimal.Parse(txtValor.Text) == 0)
            {
                //MessageBox.Show("El valor que se desea pagar debe ser mayor a cero");
                Control.Common.General.GetMensajeToList(347);
                txtValor.Focus();
                return false;
            }
            if (txtPago.Text.Trim().Replace(".", "").Length == 0)
            {
                //MessageBox.Show("No ha indicado el pago");
                Control.Common.General.GetMensajeToList(348);
                txtPago.Focus();
                return false;
            }
            if (decimal.Parse(txtPago.Text) == 0)
            {
                //MessageBox.Show("El efectivo recibido debe ser mayor a cero");
                Control.Common.General.GetMensajeToList(349);
                txtPago.Focus();
                return false;
            }
            if (decimal.Parse(txtPago.Text) < GetPagoMasRecargo())
            {
                //MessageBox.Show("El efectivo recibido no puede ser menor al pago más recargo solicitado");
                Control.Common.General.GetMensajeToList(350);
                txtPago.Focus();
                return false;
            }

            var instRecaudo = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem);
            var cuentaSeleccionada = gridPagos.SelectedRows[0].DataBoundItem as Models.CorrBan.CustAccount;

            if (GetPagoMasRecargo() < cuentaSeleccionada.ValueMinTotal)
            {
                //MessageBox.Show("El pago más recargo es menor al minimo permitido por la institución: " + cuentaSeleccionada.ValueMinTotal.ToString("N2"));

                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[ValueMinTotal]", valor = cuentaSeleccionada.ValueMinTotal.ToString("N2") });
                Control.Common.General.GetMensajeToList(351, parametros);

                txtValor.Focus();
                return false;
            }

            //if (decimal.Parse(txtValor.Text.Trim()) > cuentaSeleccionada.ValueTotal)
            //{
            //    MessageBox.Show("El valor es mayor al monto a pagar de la cuenta seleccionada: " + cuentaSeleccionada.ValueTotal.ToString("N2"));
            //    txtValor.Focus();
            //    return false;
            //}

            if (GetPagoMasRecargo() > instRecaudo.Max)
            {
                //MessageBox.Show("El pago más recargo es mayor al máximo permitido por la institución: " + instRecaudo.Max.ToString("N2"));

                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[instRecaudo]", valor = instRecaudo.Max.ToString("N2") });
                Control.Common.General.GetMensajeToList(352, parametros);


                txtValor.Focus();
                return false;
            }

            if (!instRecaudo.AplicaAbono && GetPagoMasRecargo() != cuentaSeleccionada.ValueMaxTotal)
            {
                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[ValueMaxTotal]", valor = cuentaSeleccionada.ValueMaxTotal.ToString("N2") });
                Control.Common.General.GetMensajeToList(354, parametros);

                //MessageBox.Show("La institución no permite abono, el valor de pago debe ser igual al monto a pagar en la cuenta: " + cuentaSeleccionada.ValueMaxTotal.ToString("N2"));
                txtValor.Focus();
                return false;
            }

            return true;
        }

        private string ResponseGuardar { get; set; }
        private string ResponseReverso { get; set; }
        BackgroundWorker bgwGuardar;
        private void ProcesarGuardarBackGround()
        {
            if (!ValidarSolicitudPago()) return;

            paneLoading.Visible = true;

            if (bgwGuardar == null)
            {
                bgwGuardar = new BackgroundWorker();
                bgwGuardar.DoWork += new DoWorkEventHandler(bgwGuardar_DoWork);
                bgwGuardar.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwGuardar_RunWorkerCompleted);
            }
            bgwGuardar.WorkerReportsProgress = true;
            bgwGuardar.WorkerSupportsCancellation = true;
            bgwGuardar.RunWorkerAsync();
            System.Threading.Thread.Sleep(500);
        }
        void bgwGuardar_DoWork(object sender, DoWorkEventArgs e)
        {
            RealizarGuardar();
        }

        void bgwGuardar_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            paneLoading.Visible = false;
            cmbInstRecaudo.Focus();
            MessageBox.Show(ResponseGuardar);
        }

        private void RealizarGuardar()
        {
            On_Off_Controles(false);

            try
            {
                int idMetodo = 5;

                var soapDetail = LirisLibCorrBan.Business.SoapCorrBanBO.GetSoapDetail(idMetodo);

                var cuentaSeleccionada = gridPagos.SelectedRows[0].DataBoundItem as Models.CorrBan.CustAccount;

                var xmlWithValues = GetXmlWithValues(soapDetail.SoapLiris, idMetodo, cuentaSeleccionada);


                if (Common.XmlHelper.ValidateXml(xmlWithValues, soapDetail.XsdLiris))
                {
                    //TipoTransaccion: 12 - EmpresaRecarga en CatalogoDet
                    //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
                    var responseXml = LirisLibCorrBan.Business.ProceduresBO.ProcesaRecaudacion(
                                                                            xmlWithValues
                                                                          );

                    if (string.IsNullOrEmpty(responseXml)) responseXml = string.Empty;

                    if (Common.XmlHelper.IsMinimallyValidXml(responseXml))
                    {
                        XDocument xDoc = XDocument.Parse(responseXml);
                        //El xml respuesta viene con namespaces propios de soap, entonces para leer su contenido debemos proveer 
                        //estos namespaces a los queries
                        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                        XNamespace srr = "http://transferunion.org/";

                        var responseObj = (from d in xDoc.Descendants(soap + "Body").Descendants(srr + "SaveCollectionResponse").Descendants(srr + "SaveCollectionResult")
                                           select new
                                           {
                                               CarCoeId = (d.Element(srr + "CarCoeId") == null) ? "" : d.Element(srr + "CarCoeId").Value,
                                               CarCuenta = (d.Element(srr + "CarCuenta") == null) ? "" : d.Element(srr + "CarCuenta").Value,
                                               CarDate = (d.Element(srr + "CarDate") == null) ? "" : d.Element(srr + "CarDate").Value,
                                               ErrNumber = (d.Element(srr + "ErrNumber") == null) ? "" : d.Element(srr + "ErrNumber").Value.PadLeft(4, '0'),
                                               ErrDescription = (d.Element(srr + "ErrDescription") == null) ? "" : d.Element(srr + "ErrDescription").Value,
                                               CarFactura = (d.Element(srr + "CarFactura") == null) ? "" : d.Element(srr + "CarFactura").Value,
                                               Sucess = (d.Element(srr + "Sucess") == null) ? "" : d.Element(srr + "Sucess").Value,
                                               CarSequential = (d.Element(srr + "CarSequential") == null) ? "" : d.Element(srr + "CarSequential").Value,
                                               CarSequentialSARA = (d.Element(srr + "CarSequentialSARA") == null) ? "" : d.Element(srr + "CarSequentialSARA").Value,
                                               CarAutorizacion = (d.Element(srr + "CarAutorizacion") == null) ? "" : d.Element(srr + "CarAutorizacion").Value,
                                               CarSecurity = (d.Element(srr + "CarSecurity") == null) ? "" : d.Element(srr + "CarSecurity").Value,
                                               CarNames = (d.Element(srr + "CarNames") == null) ? "" : d.Element(srr + "CarNames").Value,
                                               CarDocumentId = (d.Element(srr + "CarDocumentId") == null) ? "" : d.Element(srr + "CarDocumentId").Value,                                               
                                               CarAdress = (d.Element(srr + "CarAdress") == null) ? "" : d.Element(srr + "CarAdress").Value,
                                               CarValuePaid = (d.Element(srr + "CarValuePaid") == null) ? "" : d.Element(srr + "CarValuePaid").Value,
                                               CarValueEfective = (d.Element(srr + "CarValueEfective") == null) ? "" : d.Element(srr + "CarValueEfective").Value,
                                               CarValueTotal = (d.Element(srr + "CarValueTotal") == null) ? "" : d.Element(srr + "CarValueTotal").Value,
                                               CarValueMin = (d.Element(srr + "CarValueMin") == null) ? "" : d.Element(srr + "CarValueMin").Value,
                                               CarValueActivaCharge = (d.Element(srr + "CarValueActivaCharge") == null) ? "" : d.Element(srr + "CarValueActivaCharge").Value,
                                               CarValuePaySuggested = (d.Element(srr + "CarValuePaySuggested") == null) ? "" : d.Element(srr + "CarValuePaySuggested").Value,
                                               CarValueDisability = (d.Element(srr + "CarValueDisability") == null) ? "" : d.Element(srr + "CarValueDisability").Value,
                                               carValueTax = (d.Element(srr + "carValueTax") == null) ? "" : d.Element(srr + "carValueTax").Value,
                                               carValueAditional = (d.Element(srr + "carValueAditional") == null) ? "" : d.Element(srr + "carValueAditional").Value,
                                               carValueOthers = (d.Element(srr + "carValueOthers") == null) ? "" : d.Element(srr + "carValueOthers").Value,
                                               CarDateVen = (d.Element(srr + "CarDateVen") == null) ? "" : d.Element(srr + "CarDateVen").Value,
                                               CarActivacode = (d.Element(srr + "CarActivacode") == null) ? "" : d.Element(srr + "CarActivacode").Value,
                                               CarPrintFactura = (d.Element(srr + "CarPrintFactura") == null) ? "" : d.Element(srr + "CarPrintFactura").Value,
                                               CarValidation = (d.Element(srr + "CarValidation") == null) ? "" : d.Element(srr + "CarValidation").Value,
                                               CarPrintRecibo = (d.Element(srr + "CarPrintRecibo") == null) ? "" : d.Element(srr + "CarPrintRecibo").Value,
                                               CarPrintOtros = (d.Element(srr + "CarPrintOtros") == null) ? "" : d.Element(srr + "CarPrintOtros").Value,
                                               CarCltid = (d.Element(srr + "CarCltid") == null) ? "" : d.Element(srr + "CarCltid").Value,
                                               CarObservation = (d.Element(srr + "CarObservation") == null) ? "" : d.Element(srr + "CarObservation").Value,
                                               CarTypePrintDocument = (d.Element(srr + "CarTypePrintDocument") == null) ? "" : d.Element(srr + "CarTypePrintDocument").Value,
                                               CarMoreAccounts = (d.Element(srr + "CarMoreAccounts") == null) ? "" : d.Element(srr + "CarMoreAccounts").Value
                                           }).FirstOrDefault();

                        if (responseObj != null)
                        {
                            if (responseObj.ErrNumber == "0000")
                            {
                                string impresionRespuesta = string.Empty;
                                string xmlFactura = string.Empty;

                                //Preparar parametros impresion
                                var paramXml = new XmlDocument();
                                paramXml.LoadXml("<root />");
                                paramXml.DocumentElement.SetAttribute("codCorr", "99");// ((LirisLibCorrBan.Models.InstRecaudos)cmbOperadora.SelectedItem.DataBoundItem).CodigoEmpresa);
                                paramXml.DocumentElement.SetAttribute("valorRecibido", txtPago.Text);
                                paramXml.DocumentElement.SetAttribute("valorCambio", lblCambio.Text.Replace("$", ""));
                                paramXml.DocumentElement.SetAttribute("RecActivaCode", responseObj.CarActivacode);
                                paramXml.DocumentElement.SetAttribute("CarSequentialSARA", responseObj.CarSequentialSARA);
                                paramXml.DocumentElement.SetAttribute("RecCltid", responseObj.CarCltid);
                                paramXml.DocumentElement.SetAttribute("autSara", responseObj.CarSequentialSARA);
                                paramXml.DocumentElement.SetAttribute("fonoCuenta", txtCuenta.Text.Trim());
                                paramXml.DocumentElement.SetAttribute("secuCarr", responseObj.CarSequential);
                                paramXml.DocumentElement.SetAttribute("facAbono", responseObj.CarValueEfective);
                                paramXml.DocumentElement.SetAttribute("facTotalFinal", responseObj.CarValueTotal);
                                paramXml.DocumentElement.SetAttribute("concepto", ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem).NombreEntidad);
                                paramXml.DocumentElement.SetAttribute("facAutorizacion", responseObj.CarAutorizacion);
                                paramXml.DocumentElement.SetAttribute("facValidacion", responseObj.CarValidation);
                                paramXml.DocumentElement.SetAttribute("facTerminal", Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarTerminal").FirstOrDefault().Descripcion);
                                paramXml.DocumentElement.SetAttribute("facTitular", responseObj.CarNames);
                                paramXml.DocumentElement.SetAttribute("facMensaje", string.Empty);
                                paramXml.DocumentElement.SetAttribute("typePrintDocument", responseObj.CarTypePrintDocument);
                           
                                //Evaluar si corresponsal nos envió datos para imprimir comprobante
                                if (responseObj.CarTypePrintDocument.Contains("F"))
                                {
                                    xmlFactura = responseObj.CarPrintFactura;
                                }
                                else if (responseObj.CarTypePrintDocument.Contains("R"))
                                {
                                    xmlFactura = responseObj.CarPrintRecibo;
                                }
                                else
                                {
                                    xmlFactura = SimulaTramaFactura(responseObj);
                                }

                                //Imprimir factura con datos recibidos desde corresponsal
                                Imprimir(xmlFactura, paramXml, ref impresionRespuesta);

                                LimpiarControles();

                                ResponseGuardar = responseObj.ErrDescription
                                                    + Environment.NewLine
                                                    + impresionRespuesta;
                            }
                            else
                            {
                                ResponseGuardar = "No se ha realizado la transacción"
                                                    + Environment.NewLine
                                                    + Environment.NewLine
                                                    + responseObj.ErrDescription
                                                    + Environment.NewLine
                                                    + responseObj.CarObservation;
                            }
                        }
                        else
                        {
                            ResponseGuardar = "El servicio no devolvió la respuesta esperada pero aún puede que la recarga si se haya realizado, por favor contacte a administrador para verificar, si la recarga no se realizó inténtelo otra vez en unos breves momentos";
                        }
                    }
                    else
                    {
                        Logger.LogMessage(Common.Enum.LogTypes.Info, "PagosServicios", "RealizarGuardar", "Trama de respuesta no esperada",
                                          string.Format("Trama: {0}", responseXml));
                        ResponseGuardar = "El servicio retornó una respuesta no esperada, por favor inténtelo otra vez en unos breves momentos";
                    }

                }
                else
                {
                    Logger.LogMessage(Common.Enum.LogTypes.Info, "PagosServicios", "RealizarGuardar", "Trama de envío no valida",
                                      string.Format("Trama: {0}", xmlWithValues));
                    ResponseGuardar = "La trama que se quiere enviar no es válida, esto puede deberse a un breve mantenimiento por favor inténtelo de nuevo. La trama fue guardada en el log para que pueda ser revisada";
                }

            }
            catch (Exception ex)
            {
                string respuestareverso = RealizarReversoTransaccion(); 
                Logger.LogMessage(Common.Enum.LogTypes.Error, "PagosServicios", "RealizarGuardar", ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                ResponseGuardar = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a un breve mantenimiento, por favor inténtelo una vez más";                
            }
            finally
            {
                On_Off_Controles(true);
            }
        }
        

        private string SimulaTramaFactura(dynamic objResponseCorrBan)
        {
            string response = string.Empty;

            try
            {
                var instRecaudo = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem);
                string plantilla = CorrBan.ClsCorrBan.PlantillaXMLDefaultServiciosBasicos;

                plantilla = plantilla.Replace("<<ClienteNombre>>", lblCliente.Text.Trim());
                plantilla = plantilla.Replace("<<ClienteIdentificacion>>", lblIdentificacion.Text.Trim());// (string)objResponseCorrBan.GetType().GetProperty("CarDocumentId").GetValue(objResponseCorrBan, null));
                plantilla = plantilla.Replace("<<Fecha-dd/MM/yyyy>>", DateTime.Now.ToString("dd/MM/yyyy"));
                plantilla = plantilla.Replace("<<Hora-hh:mm:ss>>", string.Concat(DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0')));
                plantilla = plantilla.Replace("<<InstRecaudo>>", instRecaudo.NombreEntidad);
                plantilla = plantilla.Replace("<<ValorTotal>>", decimal.Parse(lblPagoMasRecargo.Text.Replace("$", "").Trim()).ToString("N2"));
                plantilla = plantilla.Replace("<<Operador>>", Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarOperator").FirstOrDefault().Descripcion);

                response = plantilla;
            }
            catch (Exception)
            {
                response = string.Empty;
            }

            return response;
        }



        #endregion

        private void Imprimir(string strXmlFactura, System.Xml.XmlDocument paramXml, ref string respuesta)
        {
            var recibo = String.Empty;

            try
            {
                string typePrintDocument = paramXml.DocumentElement.HasAttribute("typePrintDocument") ? paramXml.DocumentElement.GetAttribute("typePrintDocument") : string.Empty;

                if (typePrintDocument.Contains("R"))
                {
                    //Enviar la trama tal como la solicita el recaudo. Casos: Bco Pacifico, Yanbal
                    recibo = XmlHelper.XmlToText(strXmlFactura);
                }
                else
                {
                    //Transfomar xml factura corresponsal a comprobante impresion liris
                    recibo = ClsCorrBan.TransformarXmlCorrReciboAReciboLiris(strXmlFactura, paramXml);
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage(Common.Enum.LogTypes.Error, "PagosServicios", "Imprimir", ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                respuesta = "Sin impresión de comprobante";

                recibo = respuesta;
            }

            //Enviar a imprimir
            Common.Printer.Imprimir(recibo, 3, 11);

            Control.Common.Printer.OpenCashDrawer_PrinterName(new System.Drawing.Printing.PrinterSettings().PrinterName);
        }

        private void CalcularCambio()
        {
            try
            {
                decimal valorRecargo = 0;

                if (gridPagos.DataSource != null)
                {
                    if (((BindingList<Models.CorrBan.CustAccount>)gridPagos.DataSource).Count > 0)
                    {
                        var cuentaSeleccionada = gridPagos.SelectedRows[0].DataBoundItem as Models.CorrBan.CustAccount;
                        valorRecargo = (cuentaSeleccionada != null) ? cuentaSeleccionada.ValueActivaCharge : 0;
                    }
                }
                var pagoMasrecargo = ((txtValor.Text.Length > 0) ? decimal.Parse(txtValor.Text) : 0) + valorRecargo;

                lblPagoMasRecargo.Text = string.Format("{0:C}", pagoMasrecargo);

                lblCambio.Text = string.Format("{0:C}", (txtPago.Text.Length > 0 ? decimal.Parse(txtPago.Text) : 0) - pagoMasrecargo);
            }
            catch (Exception ex)
            {
                lblCambio.Text = string.Format("{0:C}", 0);
            }
        }

        private decimal GetPagoMasRecargo()
        {
            decimal val = 0;
            decimal.TryParse(lblPagoMasRecargo.Text.Replace("$", "").Trim(), out val);

            return val;
        }

        private string GetXmlWithValues(string xmlStr, int idMetodo, Models.CorrBan.CustAccount cuentaCliente = null)
        {
            string[] splitNames = null;

            if (SelectedCustomer != null)
            {
                splitNames = SelectedCustomer.NAME.Trim().Split(' ');
            }

            var doc = System.Xml.Linq.XDocument.Parse(xmlStr);

            doc.Root.Attribute("est").Value = Control.Common.GlobalParameters.Establecimiento;
            doc.Root.Attribute("pt").Value = Control.Common.GlobalParameters.PuntoEmision;
            doc.Root.Attribute("us").Value = Control.Common.GlobalParameters.Usuario;
            doc.Root.Attribute("ip").Value = Control.Common.GlobalParameters.IpMaquina;
            doc.Root.Attribute("ik").Value = Program.ID_Caja_POS;

            string value = string.Empty;
            if ((lblPagoMasRecargo.Text.Replace("$", "").Trim().IndexOf('.') == -1))
            {
                value = lblPagoMasRecargo.Text.Replace("$", "").Trim();
            }
            else
            {
                var splitValor = lblPagoMasRecargo.Text.Replace("$", "").Trim().Split('.');
                if (splitValor[1].Trim().Length > 0)
                    value = splitValor[0] + "." + splitValor[1].PadRight(2, '0');
                else
                    value = splitValor[0].Trim();
            }

            doc.Root.Element("req").Attribute("tv").Value = string.IsNullOrWhiteSpace(value) ? "0" : value;
            doc.Root.Element("req").Attribute("tr").Value = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem).IdIRecaudos.ToString();
            doc.Root.Element("req").Attribute("rs").Value = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem).CodigoEmpresa.ToString();
            doc.Root.Element("req").Attribute("rt").Value = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem).IdTipoCorrBan.ToString();
            doc.Root.Element("req").Attribute("ts").Value = (cuentaCliente == null) ? "0" : cuentaCliente.SequSara;
            doc.Root.Element("req").Attribute("tq").Value = (cuentaCliente == null) ? "0" : cuentaCliente.SequCarrier;
            doc.Root.Element("req").Attribute("tt").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmTipoTransDefault").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sc").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmCanal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sa").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarAgency").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("so").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarOperator").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("st").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarTerminal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sr").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarCarrier").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("ss").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarSecurity").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("cc").Value = (cuentaCliente == null) ? txtCuenta.Text.Trim() : cuentaCliente.AccountCode;
            if (rbtnNombres.Checked)
            {
                if (doc.Root.Element("req").Attribute("tn") != null) doc.Root.Element("req").Attribute("tn").Value = txtNombres.Text.Trim();
            }
            else
            {
                if (doc.Root.Element("req").Attribute("cio") != null) doc.Root.Element("req").Attribute("cio").Value = txtNombres.Text.Trim();
            }
            //if (rbtnIdentificacion.Checked)
            //cod Cliente Auditoria : No puede usarse el mismo que le enviamos a WU xq WU necesita recibir el codigo vacio en caso de CONSUMIDOR FINAL y nosotros necesitamos registrar ese codigo en nuestra tabla
            doc.Root.Element("req").Attribute("ac").Value = (SelectedCustomer != null) ? SelectedCustomer.ACCOUNTNUM : Common.GlobalParameters.IdConsumidorFinal;
            //TipoRecaudoTransaccion: 11 - EmpresaRecarga en CatalogoDet / 02 TiempoAire / 
            //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
            doc.Root.Element("req").Attribute("tm").Value = idMetodo.ToString();
            //Date
            doc.Root.Element("req").Attribute("td").Value = DateTime.Now.ToString("s");
            ////cusDocumentId
            doc.Root.Element("req").Attribute("ci").Value = (SelectedCustomer != null) ? SelectedCustomer.ACCOUNTNUM : string.Empty;
            ////cusTypeDocument
            doc.Root.Element("req").Attribute("cy").Value = (SelectedCustomer != null) ? ((SelectedCustomer.ACCOUNTNUM.Trim().Length == 10) ? "C" : "R") : string.Empty;
            ////cusFName
            doc.Root.Element("req").Attribute("cn").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(splitNames[0]) : StringHelper.ToAlphaNumeric(SelectedCustomer.NAME)) : string.Empty;
            ////cusLName
            doc.Root.Element("req").Attribute("ca").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(SelectedCustomer.NAME.Replace(splitNames[0], "").Trim()) : string.Empty) : string.Empty; //Foo
            ////cusMName
            //doc.Root.Element("req").Attribute("cm").Value = string.Empty; //Bar
            ////cusType
            doc.Root.Element("req").Attribute("ct").Value = (SelectedCustomer != null) ? "N" : string.Empty;
            ////cusAdress
            doc.Root.Element("req").Attribute("cd").Value = (SelectedCustomer != null) ? ((!string.IsNullOrEmpty(SelectedCustomer.ADDRESS)) ? StringHelper.ToAlphaNumeric(SelectedCustomer.ADDRESS) : "No") : "No";
            ////cusPhone
            //doc.Root.Element("req").Attribute("cp").Value = (SelectedCustomer != null) ? StringHelper.ToAlphaNumeric(SelectedCustomer.PHONE) : string.Empty;
            ////cusAltPhone
            //doc.Root.Element("req").Attribute("ch").Value = string.Empty;
            ////cusEmail
            //doc.Root.Element("req").Attribute("e").Value = (SelectedCustomer != null) ? SelectedCustomer.EMAIL : string.Empty;
            ////invCity
            //doc.Root.Element("req").Attribute("iy").Value = string.Empty;

            return doc.ToString();
        }

        private void LimpiarControles()
        {
            cmbInstRecaudo.SelectedIndex = 0;
            LimpiarBase();
        }

        private void LimpiarBase(bool persistirCuenta = false)
        {
            if (CuentasBinding != null) CuentasBinding.Clear();
            if (!persistirCuenta) txtCuenta.Text = string.Empty;
            txtNombres.Text = string.Empty;
            txtFiltraInstitucion.Text = string.Empty;
            LimpiarCliente();
            LimpiarSeccionPago();
        }

        private void LimpiarSeccionPago()
        {
            txtValor.Text = string.Empty;
            txtPago.Text = string.Empty;
        }

        private void LimpiarCliente()
        {
            SelectedCustomer = null;
            lblIdentificacion.Text = Common.GlobalParameters.IdConsumidorFinal;
            lblCliente.Text = Common.GlobalParameters.NameConsumidorFinal;
        }

        #endregion

        private void btnCambiarCliente_Click(object sender, EventArgs e)
        {
            var frmSearch = new Control.Clientes.SearchClient(SelectedCustomer);
            frmSearch.ShowDialog();
            SelectedCustomer = frmSearch.SelectedCustomer;
            if (SelectedCustomer != null)
            {
                lblIdentificacion.Text = SelectedCustomer.ACCOUNTNUM;
                lblCliente.Text = SelectedCustomer.NAME;
                btnRealizarPago.Focus();
            }
            else
            {
                lblIdentificacion.Text = Common.GlobalParameters.IdConsumidorFinal;
                lblCliente.Text = Common.GlobalParameters.NameConsumidorFinal;
            }
        }

        private void btnCF_Click(object sender, EventArgs e)
        {
            LimpiarCliente();
        }

        private void txtValor_TextChanged(object sender, EventArgs e)
        {
            CalcularCambio();
        }

        private void txtPago_TextChanged(object sender, EventArgs e)
        {
            CalcularCambio();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarControles();
        }

        private void btnReimprimir_Click(object sender, EventArgs e)
        {
            ReimpresionServicios frmReimpresion = new ReimpresionServicios();
            frmReimpresion.ShowDialog();
        }

        private void txtCuenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConsultar.PerformClick();
            }
            else
            {
                LimpiarBase(true);
            }
        }

        private void txtValor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPago.Focus();
            }
        }

        private void txtPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnRealizarPago.Focus();
            }
        }

        private void txtFiltraInstitucion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    var institucion = ((List<LirisLibCorrBan.Models.InstRecaudos>)cmbInstRecaudo.DataSource).Where(x => x.NombreEntidad.ToUpper().Contains(txtFiltraInstitucion.Text.Trim().ToUpper())).FirstOrDefault();

                    if (institucion != null)
                        cmbInstRecaudo.SelectedIndex = cmbInstRecaudo.FindStringExact(institucion.NombreEntidad);
                }
                catch (Exception ex)
                {
                    cmbInstRecaudo.SelectedIndex = 0;
                }
            }
        }

        private void txtNombres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConsultar.PerformClick();
            }
        }

        private void ChangeLogo()
        {
            try
            {
                var empSeleccionada = (LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem;
                if (empSeleccionada.CodigoEmpresa != "00")
                {
                    if (!string.IsNullOrEmpty(empSeleccionada.Logo))
                    {
                        var img = Image.FromFile(empSeleccionada.Logo);
                        if (img.Size.Width > pictureBox1.Size.Width)
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        else
                            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                        pictureBox1.Image = img;
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
            catch (Exception ex)
            {
                pictureBox1.Image = null;
            }
        }

        public string RealizarReversoTransaccion()
        {
            int idMetodo = 6;

            var soapDetail = LirisLibCorrBan.Business.SoapCorrBanBO.GetSoapDetail(idMetodo);

            var recaudacion = gridPagos.SelectedRows[0].DataBoundItem as Models.CorrBan.CustAccount;

            var xmlWithValues = GetXmlWithValuesReverso(soapDetail.SoapLiris, idMetodo, recaudacion);


            if (Common.XmlHelper.ValidateXml(xmlWithValues, soapDetail.XsdLiris))
            {
                //TipoTransaccion: 12 - EmpresaRecarga en CatalogoDet
                //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
                var responseXml = LirisLibCorrBan.Business.ProceduresBO.ProcesaRecaudacion(
                                                                        xmlWithValues
                                                                      );

                if (string.IsNullOrEmpty(responseXml)) responseXml = string.Empty;

                if (Common.XmlHelper.IsMinimallyValidXml(responseXml))
                {
                    XDocument xDoc = XDocument.Parse(responseXml);
                    //El xml respuesta viene con namespaces propios de soap, entonces para leer su contenido debemos proveer 
                    //estos namespaces a los queries
                    XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                    XNamespace srr = "http://transferunion.org/";

                    var responseObj = (from d in xDoc.Descendants(soap + "Body").Descendants(srr + "ReverseCollectionResponse").Descendants(srr + "ReverseCollectionResult")
                                       select new
                                       {
                                           CarCoeId = (d.Element(srr + "CarCoeId") == null) ? "" : d.Element(srr + "CarCoeId").Value,
                                           CarCuenta = (d.Element(srr + "CarCuenta") == null) ? "" : d.Element(srr + "CarCuenta").Value,
                                           CarDate = (d.Element(srr + "CarDate") == null) ? "" : d.Element(srr + "CarDate").Value,
                                           ErrNumber = (d.Element(srr + "ErrNumber") == null) ? "" : d.Element(srr + "ErrNumber").Value.PadLeft(4, '0'),
                                           ErrDescription = (d.Element(srr + "ErrDescription") == null) ? "" : d.Element(srr + "ErrDescription").Value,
                                           CarFactura = (d.Element(srr + "CarFactura") == null) ? "" : d.Element(srr + "CarFactura").Value,
                                           Sucess = (d.Element(srr + "Sucess") == null) ? "" : d.Element(srr + "Sucess").Value,
                                           CarSequential = (d.Element(srr + "CarSequential") == null) ? "" : d.Element(srr + "CarSequential").Value,
                                           CarSequentialSARA = (d.Element(srr + "CarSequentialSARA") == null) ? "" : d.Element(srr + "CarSequentialSARA").Value,
                                           CarAutorizacion = (d.Element(srr + "CarAutorizacion") == null) ? "" : d.Element(srr + "CarAutorizacion").Value,
                                           CarSecurity = (d.Element(srr + "CarSecurity") == null) ? "" : d.Element(srr + "CarSecurity").Value,
                                           CarNames = (d.Element(srr + "CarNames") == null) ? "" : d.Element(srr + "CarNames").Value,
                                           CarAdress = (d.Element(srr + "CarAdress") == null) ? "" : d.Element(srr + "CarAdress").Value,
                                           CarValuePaid = (d.Element(srr + "CarValuePaid") == null) ? "" : d.Element(srr + "CarValuePaid").Value,
                                           CarValueEfective = (d.Element(srr + "CarValueEfective") == null) ? "" : d.Element(srr + "CarValueEfective").Value,
                                           CarValueTotal = (d.Element(srr + "CarValueTotal") == null) ? "" : d.Element(srr + "CarValueTotal").Value,
                                           CarValueMin = (d.Element(srr + "CarValueMin") == null) ? "" : d.Element(srr + "CarValueMin").Value,
                                           CarValueActivaCharge = (d.Element(srr + "CarValueActivaCharge") == null) ? "" : d.Element(srr + "CarValueActivaCharge").Value,
                                           CarValuePaySuggested = (d.Element(srr + "CarValuePaySuggested") == null) ? "" : d.Element(srr + "CarValuePaySuggested").Value,
                                           CarValueDisability = (d.Element(srr + "CarValueDisability") == null) ? "" : d.Element(srr + "CarValueDisability").Value,
                                           carValueTax = (d.Element(srr + "carValueTax") == null) ? "" : d.Element(srr + "carValueTax").Value,
                                           carValueAditional = (d.Element(srr + "carValueAditional") == null) ? "" : d.Element(srr + "carValueAditional").Value,
                                           carValueOthers = (d.Element(srr + "carValueOthers") == null) ? "" : d.Element(srr + "carValueOthers").Value,
                                           CarDateVen = (d.Element(srr + "CarDateVen") == null) ? "" : d.Element(srr + "CarDateVen").Value,
                                           CarActivacode = (d.Element(srr + "CarActivacode") == null) ? "" : d.Element(srr + "CarActivacode").Value,
                                           CarPrintFactura = (d.Element(srr + "CarPrintFactura") == null) ? "" : d.Element(srr + "CarPrintFactura").Value,
                                           CarPrintRecibo = (d.Element(srr + "CarPrintRecibo") == null) ? "" : d.Element(srr + "CarPrintRecibo").Value,
                                           CarPrintOtros = (d.Element(srr + "CarPrintOtros") == null) ? "" : d.Element(srr + "CarPrintOtros").Value,
                                           CarCltid = (d.Element(srr + "CarCltid") == null) ? "" : d.Element(srr + "CarCltid").Value,
                                           CarObservation = (d.Element(srr + "CarObservation") == null) ? "" : d.Element(srr + "CarObservation").Value,
                                           CarTypePrintDocument = (d.Element(srr + "CarTypePrintDocument") == null) ? "" : d.Element(srr + "CarTypePrintDocument").Value,
                                           CarMoreAccounts = (d.Element(srr + "CarMoreAccounts") == null) ? "" : d.Element(srr + "CarMoreAccounts").Value
                                       }).FirstOrDefault();

                    if (responseObj != null)
                    {
                        if (responseObj.ErrNumber == "0000")
                        {
                            ResponseReverso = responseObj.ErrDescription;

                        }
                        else
                        {
                            ResponseReverso = "No se ha realizado la transacción"
                                                + Environment.NewLine
                                                + Environment.NewLine
                                                + responseObj.ErrDescription
                                                + Environment.NewLine
                                                + responseObj.CarObservation;
                        }
                    }
                    else
                    {
                        ResponseReverso = "El servicio no devolvió la respuesta esperada pero aún puede que la recarga si se haya realizado, por favor contacte a administrador para verificar, si la recaudación no se realizó inténtelo otra vez en unos breves momentos";
                    }
                }
                else
                {
                    Logger.LogMessage(Common.Enum.LogTypes.Info, "PagosServicios", "RealizarReversoTransaccion", "Trama de respuesta no esperada",
                                      string.Format("Trama: {0}", responseXml));
                    ResponseReverso = "El servicio retornó una respuesta no esperada, por favor inténtelo otra vez en unos breves momentos";
                }

            }
            else
            {
                Logger.LogMessage(Common.Enum.LogTypes.Info, "PagosServicios", "RealizarReversoTransaccion", "Trama de envío no valida",
                                  string.Format("Trama: {0}", xmlWithValues));
                ResponseReverso = "La trama que se quiere enviar no es válida, esto puede deberse a un breve mantenimiento por favor inténtelo de nuevo. La trama fue guardada en el log para que pueda ser revisada";
            }
            return ResponseReverso;
        }

        private string GetXmlWithValuesReverso(string xmlStr, int idMetodo, Models.CorrBan.CustAccount cuentaCliente = null)
        { 
            var doc = System.Xml.Linq.XDocument.Parse(xmlStr);

            doc.Root.Attribute("est").Value = Control.Common.GlobalParameters.Establecimiento;
            doc.Root.Attribute("pt").Value = Control.Common.GlobalParameters.PuntoEmision;
            doc.Root.Attribute("us").Value = Control.Common.GlobalParameters.Usuario;
            doc.Root.Attribute("ip").Value = Control.Common.GlobalParameters.IpMaquina;
            doc.Root.Attribute("ik").Value = Program.ID_Caja_POS;
             
            doc.Root.Element("req").Attribute("tv").Value = "0";
            doc.Root.Element("req").Attribute("tr").Value = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem).IdIRecaudos.ToString();
            doc.Root.Element("req").Attribute("rs").Value = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem).CodigoEmpresa.ToString();
            doc.Root.Element("req").Attribute("rt").Value = ((LirisLibCorrBan.Models.InstRecaudos)cmbInstRecaudo.SelectedItem.DataBoundItem).IdTipoCorrBan.ToString();
            doc.Root.Element("req").Attribute("ts").Value = (cuentaCliente == null) ? "0" : cuentaCliente.SequSara;
            doc.Root.Element("req").Attribute("tq").Value = (cuentaCliente == null) ? "0" : cuentaCliente.SequCarrier;
            doc.Root.Element("req").Attribute("tt").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmTipoTransDefault").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sc").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmCanal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sa").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarAgency").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("so").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarOperator").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("st").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarTerminal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sr").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarCarrier").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("ss").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarSecurity").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("cc").Value = (cuentaCliente == null) ? txtCuenta.Text.Trim() : cuentaCliente.AccountCode;
             
            //cod Cliente Auditoria : No puede usarse el mismo que le enviamos a WU xq WU necesita recibir el codigo vacio en caso de CONSUMIDOR FINAL y nosotros necesitamos registrar ese codigo en nuestra tabla
            doc.Root.Element("req").Attribute("ac").Value = (cuentaCliente.CustIdentification != null) ? cuentaCliente.CustIdentification : Common.GlobalParameters.IdConsumidorFinal;
            //TipoRecaudoTransaccion: 11 - EmpresaRecarga en CatalogoDet / 02 TiempoAire / 
            //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
            doc.Root.Element("req").Attribute("tm").Value = idMetodo.ToString();
            //Date
            doc.Root.Element("req").Attribute("td").Value = DateTime.Now.ToString("s");
            ////cusDocumentId
            doc.Root.Element("req").Attribute("ci").Value = string.Empty;
            //Si recaudacion ya fue confirmada esta aun se puede reversar mientras este en el limite de tiempo
            //Se modifican los cambios de estado WF  
            //if ((short)recaudacion.GetType().GetProperty("WFEstado").GetValue(recaudacion, null) == 420)
            //{
                doc.Root.Element("req").Attribute("ws").Value = "422";
                doc.Root.Element("req").Attribute("wx").Value = "421";
            //}
            ////cusTypeDocument
            //doc.Root.Element("req").Attribute("cy").Value = (SelectedCustomer != null) ? ((SelectedCustomer.ACCOUNTNUM.Trim().Length == 10) ? "C" : "R") : string.Empty;
            ////cusFName
            //doc.Root.Element("req").Attribute("cn").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(splitNames[0]) : StringHelper.ToAlphaNumeric(SelectedCustomer.NAME)) : string.Empty;
            ////cusLName
            //doc.Root.Element("req").Attribute("ca").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(SelectedCustomer.NAME.Replace(splitNames[0], "").Trim()) : string.Empty) : string.Empty; //Foo
            ////cusMName
            //doc.Root.Element("req").Attribute("cm").Value = string.Empty; //Bar
            ////cusType
            //doc.Root.Element("req").Attribute("ct").Value = (SelectedCustomer != null) ? "N" : string.Empty;
            ////cusAdress
            //doc.Root.Element("req").Attribute("cd").Value = (SelectedCustomer != null) ? ((!string.IsNullOrEmpty(SelectedCustomer.ADDRESS)) ? StringHelper.ToAlphaNumeric(SelectedCustomer.ADDRESS) : "No") : "No";
            ////cusPhone
            //doc.Root.Element("req").Attribute("cp").Value = (SelectedCustomer != null) ? StringHelper.ToAlphaNumeric(SelectedCustomer.PHONE) : string.Empty;
            ////cusAltPhone
            //doc.Root.Element("req").Attribute("ch").Value = string.Empty;
            ////cusEmail
            //doc.Root.Element("req").Attribute("e").Value = (SelectedCustomer != null) ? SelectedCustomer.EMAIL : string.Empty;
            ////invCity
            //doc.Root.Element("req").Attribute("iy").Value = string.Empty;

            return doc.ToString();
        }
    }
}
