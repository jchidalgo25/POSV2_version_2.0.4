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
using System.Collections;
using POS.Control.Common;
using MensajesLibrary;

namespace POS.Control.CorrBan
{
    public partial class ReimpresionServicios : Telerik.WinControls.UI.RadForm
    {

        #region Constructores

        public ReimpresionServicios()
        {
            InitializeComponent();
        }

        #endregion

        #region Atributos Privados

        System.Windows.Forms.Control focused;

        #endregion

        #region Metodos Controles

        private void Reimpresion_Load(object sender, EventArgs e)
        {
            //Si no puede realizar el ping, cerrar el formulario
            if (!ClsCorrBan.RealizarPing(14))
                this.Close();

            btnImprimir.Focus();
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
            LoadSubTypes();
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

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            SearchRecipes();
        }

        #endregion

        #region Metodos Privados

        private void SearchRecipes()
        {
            try
            {
                gridRecaudaciones.DataSource = LirisLibCorrBan.Business.RecaudacionBO.GetRecaudacionesWFReprint(11, dtpFecha.Value, txtCuenta.Text.Trim(), Program.ID_Caja_POS);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ReimpresionServicios", "SearchRecipes", "No fue posible obtener la lista de comprobantes para reimpresion, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //MessageBox.Show("No fue posible obtener la lista de comprobantes para reimpresion, esto puede deberse a una breve eventualidad, por favor inténtelo nuevamente. Si el problema persiste contacte al administrador");
                Control.Common.General.GetMensajeToList(367);
            }
        }

        private void LoadSubTypes()
        {
        }

        private void OpenKeyboard()
        {
            var textBox = focused as Telerik.WinControls.UI.RadTextBox;
            if (textBox != null)
            {
                Control.Common.General.TecladoPantalla();

                //KeyboardControl kbd = new KeyboardControl(textBox, this.Text);
                //kbd.Top = this.Height + this.Top - 100;
                //kbd.Left = this.Left;
                //kbd.ShowDialog();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var db = new POSEntities();

            switch (keyData)
            {
                case Keys.Enter:
                    SearchRecipes();
                    break;


                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        #endregion
        
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            dynamic recaudacion = null;
            string trama = string.Empty;
            try
            {
                if (gridRecaudaciones.DataSource == null)
                {
                    //MessageBox.Show("No hay comprobantes para reimprimir. Antes de solicitar la reimpresión debe haber realizado la debida consulta");
                    Control.Common.General.GetMensajeToList(368);

                    return;
                }

                if (((IEnumerable<dynamic>)gridRecaudaciones.DataSource).Count() == 0)
                {
                    //MessageBox.Show("No existen registros para realizar la reimpresión");
                    Control.Common.General.GetMensajeToList(369);
                    return;
                }

                recaudacion = gridRecaudaciones.SelectedRows[0].DataBoundItem as dynamic;

                trama = (string)recaudacion.GetType().GetProperty("Trama").GetValue(recaudacion, null);
                XDocument xDoc = XDocument.Parse(trama);
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
                    string impresionRespuesta = string.Empty;
                    string xmlFactura = string.Empty;

                    //Preparar parametros impresion
                    var paramXml = new XmlDocument();
                    paramXml.LoadXml("<root />");
                    paramXml.DocumentElement.SetAttribute("codCorr", "99");// ((LirisLibCorrBan.Models.InstRecaudos)cmbOperadora.SelectedItem.DataBoundItem).CodigoEmpresa);
                    paramXml.DocumentElement.SetAttribute("valorRecibido", ((decimal)recaudacion.GetType().GetProperty("Valor").GetValue(recaudacion, null)).ToString("N2"));
                    paramXml.DocumentElement.SetAttribute("valorCambio", "0.00");
                    paramXml.DocumentElement.SetAttribute("RecActivaCode", responseObj.CarActivacode);
                    paramXml.DocumentElement.SetAttribute("CarSequentialSARA", responseObj.CarSequentialSARA);
                    paramXml.DocumentElement.SetAttribute("RecCltid", responseObj.CarCltid);
                    paramXml.DocumentElement.SetAttribute("autSara", responseObj.CarSequentialSARA);
                    paramXml.DocumentElement.SetAttribute("fonoCuenta", txtCuenta.Text.Trim());
                    paramXml.DocumentElement.SetAttribute("secuCarr", responseObj.CarSequential);
                    paramXml.DocumentElement.SetAttribute("facAbono", responseObj.CarValueEfective);
                    paramXml.DocumentElement.SetAttribute("facTotalFinal", responseObj.CarValueTotal);
                    paramXml.DocumentElement.SetAttribute("concepto", (string)recaudacion.GetType().GetProperty("Corresponsal").GetValue(recaudacion, null));
                    paramXml.DocumentElement.SetAttribute("facAutorizacion", responseObj.CarAutorizacion);
                    paramXml.DocumentElement.SetAttribute("facValidacion", responseObj.CarValidation);
                    paramXml.DocumentElement.SetAttribute("facTerminal", Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarTerminal").FirstOrDefault().Descripcion);
                    paramXml.DocumentElement.SetAttribute("facTitular", responseObj.CarNames);
                    paramXml.DocumentElement.SetAttribute("facMensaje", ClsCorrBan.PlantillaServiciosBasicosMensajeReimpresion);

                    //Evaluar si corresponsal nos envió datos para imprimir comprobante
                    if (responseObj.CarTypePrintDocument.Contains("F"))
                    {
                        xmlFactura = responseObj.CarPrintFactura;
                    }
                    else
                    {
                        xmlFactura = SimulaTramaFactura(responseObj);
                    }

                    //Imprimir factura con datos recibidos desde corresponsal
                    Imprimir(xmlFactura, paramXml, ref impresionRespuesta);

                    if (!string.IsNullOrEmpty(impresionRespuesta))
                        MessageBox.Show(impresionRespuesta);
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Reimpresion", "btnImprimir_Click", "No fue posible armar el objeto responseObj que permite la reimpresión por lo que se le indicó al usuario que el comprobante no aplicaba para reimpresión");
                    //MessageBox.Show("No se puede realizar la acción porque la transacción original no aplicaba para impresión");
                    Control.Common.General.GetMensajeToList(370);

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Reimpresion", "btnImprimir_Click", "No fue posible reimprimir el comprobante solicitado, Trama: " + (string.IsNullOrEmpty(trama) ? "el objeto dinamico recaudacion o bien la trama que debería estar embebida está nula" : trama) + ", a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //MessageBox.Show("No fue posible reimprimir el comprobante solicitado, esto puede deberse a una breve eventualidad por favor inténtelo nuevamente. Si el problema persiste contacte al administrador");
                Control.Common.General.GetMensajeToList(370);

            }
        }

        private string SimulaTramaFactura(dynamic objResponseCorrBan)
        {
            string response = string.Empty;

            try
            {
                var recaudacion = gridRecaudaciones.SelectedRows[0].DataBoundItem as dynamic;

                string clienteIdentificacion = (string)recaudacion.GetType().GetProperty("CodigoCliente").GetValue(recaudacion, null);
                string clienteNombres = string.Empty;
                using (POSEntities db = new POSEntities())
                {

                    var customer = db.pos_customer.Where(x => x.ACCOUNTNUM == clienteIdentificacion).FirstOrDefault();
                    if (customer != null) clienteNombres = customer.NAME;
                }

                string plantilla = CorrBan.ClsCorrBan.PlantillaXMLDefaultServiciosBasicos;

                plantilla = plantilla.Replace("<<ClienteNombre>>", clienteNombres);
                plantilla = plantilla.Replace("<<ClienteIdentificacion>>", clienteIdentificacion);// (string)objResponseCorrBan.GetType().GetProperty("CarDocumentId").GetValue(objResponseCorrBan, null));
                plantilla = plantilla.Replace("<<Fecha-dd/MM/yyyy>>", DateTime.Now.ToString("dd/MM/yyyy"));
                plantilla = plantilla.Replace("<<Hora-hh:mm:ss>>", string.Concat(DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0')));
                plantilla = plantilla.Replace("<<InstRecaudo>>", (string)recaudacion.GetType().GetProperty("Corresponsal").GetValue(recaudacion, null));
                plantilla = plantilla.Replace("<<ValorTotal>>", decimal.Parse(/*txtValor.Text.Trim()*/"0").ToString("N2"));
                plantilla = plantilla.Replace("<<Operador>>", Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarOperator").FirstOrDefault().Descripcion);

                response = plantilla;
            }
            catch (Exception)
            {
                response = string.Empty;
            }

            return response;
        }


        private void Imprimir(string strXmlFactura, System.Xml.XmlDocument paramXml, ref string respuesta)
        {
            try
            {
                //Transfomar xml factura corresponsal a comprobante impresion liris
                var recibo = ClsCorrBan.TransformarXmlCorrReciboAReciboLiris(strXmlFactura, paramXml);

                //Enviar a imprimir
                Common.Printer.Imprimir(recibo, 3, 11);
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "Reimpresion", "Imprimir", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                respuesta = "Sin impresión de comprobante";
            }
        }

        private void txtCuenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchRecipes();
                btnImprimir.Focus();
                this.AcceptButton = btnImprimir;
            }
            else
            {
                this.AcceptButton = null;
            }
        }

        private void btnReversar_Click(object sender, EventArgs e)
        {
            ProcesarReversoBackGround();
        }


        #region Hilo ReverseCollection

        private bool ValidarSolicitudReverso()
        {
            if (gridRecaudaciones.DataSource == null)
            {
                //MessageBox.Show("No hay comprobantes para reversar. Antes de solicitar un reverso debe haber realizado la debida consulta");
                Control.Common.General.GetMensajeToList(372);
                return false;
            }

            if (((IEnumerable<dynamic>)gridRecaudaciones.DataSource).Count() == 0)
            {
                //MessageBox.Show("No existen registros para realizar el reverso");
                Control.Common.General.GetMensajeToList(373);
                return false;
            }

            Fingerprint.VerificationForm Verifier = new Fingerprint.VerificationForm(Common.GlobalParameters.DataForFingerprint, null);
            Verifier.Tag = "adm";
            if (Verifier.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Control.Common.General.GetMensajeToList(374);
                return false;
            }

            var result = Control.Common.General.GetMensajeToList(375);

            bool reversa = false;
            if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok) {
                reversa = true;
            }
            return reversa;

            //if (MessageBox.Show("Reversando transaccion, una vez hecho no se puede deshacer. Pulse Si para continuar", "Red Activa", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    return true;
            //else
            //    return false;

        }

        private string ResponseReverso { get; set; }
        BackgroundWorker bgwReverso;
        private void ProcesarReversoBackGround()
        {
            if (!ValidarSolicitudReverso()) return;

            paneLoading.Visible = true;

            if (bgwReverso == null)
            {
                bgwReverso = new BackgroundWorker();
                bgwReverso.DoWork += new DoWorkEventHandler(bgwReverso_DoWork);
                bgwReverso.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwReverso_RunWorkerCompleted);
            }
            bgwReverso.WorkerReportsProgress = true;
            bgwReverso.WorkerSupportsCancellation = true;
            bgwReverso.RunWorkerAsync();
            System.Threading.Thread.Sleep(500);
        }
        void bgwReverso_DoWork(object sender, DoWorkEventArgs e)
        {
            RealizarReverso();
        }

        void bgwReverso_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            paneLoading.Visible = false;
            txtCuenta.Focus();
            SearchRecipes();
            if (!string.IsNullOrWhiteSpace(ResponseReverso)) MessageBox.Show(ResponseReverso);
        }

        private string GetXmlWithValues(string xmlStr, int idMetodo, dynamic recaudacion)
        {
            var doc = System.Xml.Linq.XDocument.Parse(xmlStr);

            doc.Root.Attribute("est").Value = Control.Common.GlobalParameters.Establecimiento;
            doc.Root.Attribute("pt").Value = Control.Common.GlobalParameters.PuntoEmision;
            doc.Root.Attribute("us").Value = Control.Common.GlobalParameters.Usuario;
            doc.Root.Attribute("ip").Value = Control.Common.GlobalParameters.IpMaquina;
            doc.Root.Attribute("ik").Value = Program.ID_Caja_POS;

            doc.Root.Element("req").Attribute("tv").Value = "0";
            doc.Root.Element("req").Attribute("tr").Value = ((short)recaudacion.GetType().GetProperty("IdIRecaudos").GetValue(recaudacion, null)).ToString();
            doc.Root.Element("req").Attribute("rs").Value = (string)recaudacion.GetType().GetProperty("CodigoEmpresa").GetValue(recaudacion, null);
            doc.Root.Element("req").Attribute("rt").Value = ((short)recaudacion.GetType().GetProperty("IdTipoCorrBan").GetValue(recaudacion, null)).ToString();
            doc.Root.Element("req").Attribute("ts").Value = ((int)recaudacion.GetType().GetProperty("SecuencialSara").GetValue(recaudacion, null)).ToString();
            doc.Root.Element("req").Attribute("tq").Value = ((int)recaudacion.GetType().GetProperty("Secuencial").GetValue(recaudacion, null)).ToString();
            doc.Root.Element("req").Attribute("tt").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmTipoTransDefault").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sc").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmCanal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sa").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarAgency").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("so").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarOperator").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("st").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarTerminal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("sr").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarCarrier").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("ss").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarSecurity").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("cc").Value = (string)recaudacion.GetType().GetProperty("Cuenta").GetValue(recaudacion, null);
            //cod Cliente Auditoria : No puede usarse el mismo que le enviamos a WU xq WU necesita recibir el codigo vacio en caso de CONSUMIDOR FINAL y nosotros necesitamos registrar ese codigo en nuestra tabla
            doc.Root.Element("req").Attribute("ac").Value = (string)recaudacion.GetType().GetProperty("CodigoCliente").GetValue(recaudacion, null);
            //TipoRecaudoTransaccion: 11 - EmpresaRecarga en CatalogoDet / 02 TiempoAire / 
            //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
            doc.Root.Element("req").Attribute("tm").Value = idMetodo.ToString();
            //Date
            doc.Root.Element("req").Attribute("td").Value = DateTime.Now.ToString("s");
            ////cusDocumentId
            doc.Root.Element("req").Attribute("ci").Value = string.Empty;
            //Si recaudacion ya fue confirmada esta aun se puede reversar mientras este en el limite de tiempo
            //Se modifican los cambios de estado WF 
            if ((short)recaudacion.GetType().GetProperty("WFEstado").GetValue(recaudacion, null) == 420)
            {
                doc.Root.Element("req").Attribute("ws").Value = "422";
                doc.Root.Element("req").Attribute("wx").Value = "421";
            }
            ////cusTypeDocument
            //doc.Root.Element("req").Attribute("cy").Value = (SelectedCustomer != null) ? ((SelectedCustomer.ACCOUNTNUM.Trim().Length == 10) ? "C" : "R") : string.Empty;
            //////cusFName
            //doc.Root.Element("req").Attribute("cn").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(splitNames[0]) : StringHelper.ToAlphaNumeric(SelectedCustomer.NAME)) : string.Empty;
            //////cusLName
            //doc.Root.Element("req").Attribute("ca").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(SelectedCustomer.NAME.Replace(splitNames[0], "").Trim()) : string.Empty) : string.Empty; //Foo
            //////cusMName
            ////doc.Root.Element("req").Attribute("cm").Value = string.Empty; //Bar
            //////cusType
            //doc.Root.Element("req").Attribute("ct").Value = (SelectedCustomer != null) ? "N" : string.Empty;
            //////cusAdress
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

        private void RealizarReverso()
        {
            On_Off_Controles(false);
            try
            {
                int idMetodo = 6;

                var soapDetail = LirisLibCorrBan.Business.SoapCorrBanBO.GetSoapDetail(idMetodo);

                dynamic recaudacion = gridRecaudaciones.SelectedRows[0].DataBoundItem as dynamic;

                var xmlWithValues = GetXmlWithValues(soapDetail.SoapLiris, idMetodo, recaudacion);


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

                                //Enviar a imprimir
                                Common.Printer.Imprimir("Reversado. Retener comprobante original", 3, 11);
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
                            ResponseReverso = "El servicio no devolvió la respuesta esperada pero aún puede que la recarga si se haya realizado, por favor contacte a administrador para verificar, si la recarga no se realizó inténtelo otra vez en unos breves momentos";
                        }
                    }
                    else
                    {
                        Logger.LogMessage(Common.Enum.LogTypes.Info, "Recargas", "RealizarRecarga", "Trama de respuesta no esperada",
                                          string.Format("Trama: {0}", responseXml));
                        ResponseReverso = "El servicio retornó una respuesta no esperada, por favor inténtelo otra vez en unos breves momentos";
                    }

                }
                else
                {
                    Logger.LogMessage(Common.Enum.LogTypes.Info, "Recargas", "RealizarRecarga", "Trama de envío no valida",
                                      string.Format("Trama: {0}", xmlWithValues));
                    ResponseReverso = "La trama que se quiere enviar no es válida, esto puede deberse a un breve mantenimiento por favor inténtelo de nuevo. La trama fue guardada en el log para que pueda ser revisada";
                }

            }
            catch (Exception ex)
            {
                Logger.LogMessage(Common.Enum.LogTypes.Error, "Recargas", "RealizarRecarga", ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                ResponseReverso = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a un breve mantenimiento, por favor inténtelo una vez más";
            }
            finally
            {
                On_Off_Controles(true);
            }
        }

        #endregion

        private bool EstadoControles { get; set; }
        private void On_Off_Controles(bool estado)
        {
            btnConsultar.Enabled = estado;
            EstadoControles = estado;
            btnReversar.Enabled = estado;
            btnImprimir.Enabled = estado;
            txtCuenta.Enabled = estado;
            btnKbd.Enabled = estado;
        }
    }
}
