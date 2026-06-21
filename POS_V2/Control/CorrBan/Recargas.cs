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
using POS.Control.Common;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Linq;

namespace POS.Control.CorrBan
{
    public partial class Recargas : Telerik.WinControls.UI.RadForm
    {
        #region Constructors

        public Recargas()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Attributes

        System.Windows.Forms.Control focused;

        #endregion

        #region Private Properties

        private pos_customer SelectedCustomer { get; set; }

        #endregion

        #region Controls Methods

        private void Recargas_Load(object sender, EventArgs e)
        {
            InitForm();

            //Si no puede realizar el ping, cerrar el formulario
            if (!ClsCorrBan.RealizarPing(12))
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
            Control.Common.General.TecladoPantalla();

            //OpenKeyboard();
        }

        private void txtControl_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPressNumeric(sender, e);
        }

        private void txtCelular_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPressNumeric(sender, e);
        }

        private void txtPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleKeyPressNumeric(sender, e);
        }

        private void btnRealizarRecarga_Click(object sender, EventArgs e)
        {
            ProcesarRecargaBackGround();// RealizarRecarga();
        }
        
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarControles();
        }

        private void btnCambiarCliente_Click(object sender, EventArgs e)
        {
            var frmSearch = new Control.Clientes.SearchClient(SelectedCustomer);
            frmSearch.ShowDialog();
            SelectedCustomer = frmSearch.SelectedCustomer;
            if (SelectedCustomer != null)
            {
                lblIdentificacion.Text = SelectedCustomer.ACCOUNTNUM;
                lblCliente.Text = SelectedCustomer.NAME;
                btnRealizarRecarga.Focus();
            }
            else
            {
                lblIdentificacion.Text = Common.GlobalParameters.IdConsumidorFinal;
                lblCliente.Text = Common.GlobalParameters.NameConsumidorFinal;
            }
        }

        private void cmbOperadora_SelectedValueChanged(object sender, EventArgs e)
        {
            ChangeLogo();
            txtCelular.Focus();
        }

        private void txtCelular_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtValor.Focus();
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
                btnRealizarRecarga.Focus();
            }

        }

        private void txtPago_TextChanged(object sender, EventArgs e)
        {
            CalcularCambio();
        }

        private void txtValor_TextChanged(object sender, EventArgs e)
        {
            CalcularCambio();
        }

        private void Recargas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (EstadoControles == false) e.Cancel = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    LimpiarControles();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        #endregion

        #region Private Methods

        private void CalcularCambio()
        {
            try
            {
                lblCambio.Text = string.Format("{0:C}", decimal.Parse(txtPago.Text) - decimal.Parse(txtValor.Text));
            }
            catch (Exception)
            {
                lblCambio.Text = string.Format("{0:C}", 0);
            }
        }

        private void InitForm()
        {
            try
            {
                cmbOperadora.DisplayMember = "NombreEntidad";
                cmbOperadora.ValueMember = "CodigoEmpresa";
                cmbOperadora.DataSource = Control.CorrBan.ClsCorrBan.ListInstRecargas;

                EstadoControles = true;

                CalcularCambio();
            }
            catch (Exception)
            {
                
            }
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


            //if (text != null)
            //{
            //    KeyboardControl kbd = new KeyboardControl(text, this.Text);
            //    kbd.Top = this.Height + this.Top - 180;
            //    kbd.Left = this.Left;
            //    kbd.ShowDialog();
            //    if (kbd.Tecla == "\n" && text.Name == "lblCedula")
            //    {
            //        using (POSEntities db = new POSEntities())
            //        {
            //            var cliente = db.pos_customer.Where(x => x.ACCOUNTNUM == lblCedula.Text);
            //            if (cliente != null)
            //            {
            //                lblCedula.Text = cliente.FirstOrDefault().ACCOUNTNUM;
            //                lblNombre.Text = cliente.FirstOrDefault().NAME;
            //            }
            //            else
            //            {
            //                MessageBox.Show(this, "Cliente No Existe");
            //                lblCedula.Text = null;
            //                lblNombre.Text = null;
            //            }
            //        }
            //    }
            //    //this.Close();
            //}
        }

        private void RealizarRecarga()
        {
            On_Off_Controles(false);
            try
            {

                var soapDetail = LirisLibCorrBan.Business.SoapCorrBanBO.GetSoapDetail(8);

                var strTrame = ConstructStrTrame(soapDetail.SoapLiris);

                var xmlWithValues = GetXmlWithValues(soapDetail.SoapLiris, strTrame);


                if (Common.XmlHelper.ValidateXml(xmlWithValues, soapDetail.XsdLiris))
                {
                    //TipoTransaccion: 12 - EmpresaRecarga en CatalogoDet
                    //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
                    var responseXml = LirisLibCorrBan.Business.ProceduresBO.ProcesaSolicitud(
                                                                            xmlWithValues
                                                                          );

                    if (string.IsNullOrEmpty(responseXml)) responseXml = string.Empty;

                    if (Common.XmlHelper.IsMinimallyValidXml(responseXml))
                    {
                        XDocument xDoc = XDocument.Parse(responseXml);
                        //El xml respuesta viene con namespaces propios de soap, entonces para leer su contenido debemos proveer 
                        //estos namespaces a los queries
                        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                        XNamespace srr = "http://www.activaecuador.com/cellphone";

                        var responseObj = (from d in xDoc.Descendants(soap + "Body").Descendants(srr + "SaveRechargeResponse").Descendants(srr + "SaveRechargeResult")
                                           select new
                                           {
                                               errNumber = d.Element(srr + "errNumber").Value,
                                               errDescription = d.Element(srr + "errDescription").Value,
                                               strTrame = d.Element(srr + "strTrame").Value,
                                               TypePrintDocument = d.Element(srr + "TypePrintDocument").Value,
                                               PrintFactura = d.Element(srr + "PrintFactura").Value,
                                               RecActivaCode = d.Element(srr + "RecActivaCode").Value,
                                               CarSequentialSARA = d.Element(srr + "CarSequentialSARA").Value,
                                               RecCltid = d.Element(srr + "RecCltid").Value
                                           }).FirstOrDefault();

                        if (responseObj != null)
                        {
                            //Si la transaccion fue exitosa limpiar los controles
                            if (responseObj.errNumber == "0000")
                            {
                                string impresionRespuesta = string.Empty;

                                //Evaluar si corresponsal nos envió datos para imprimir comprobante
                                if (responseObj.TypePrintDocument.Contains("F"))
                                {
                                    //Preparar parametros impresion
                                    var paramXml = new XmlDocument();
                                    paramXml.LoadXml("<root />");
                                    paramXml.DocumentElement.SetAttribute("codCorr", ((LirisLibCorrBan.Models.InstRecaudos)cmbOperadora.SelectedItem.DataBoundItem).CodigoEmpresa);
                                    paramXml.DocumentElement.SetAttribute("valorRecibido", txtPago.Text);
                                    paramXml.DocumentElement.SetAttribute("valorCambio", lblCambio.Text.Replace("$", ""));
                                    paramXml.DocumentElement.SetAttribute("RecActivaCode", responseObj.RecActivaCode);
                                    paramXml.DocumentElement.SetAttribute("CarSequentialSARA", responseObj.CarSequentialSARA);
                                    paramXml.DocumentElement.SetAttribute("RecCltid", responseObj.RecCltid);
                                    paramXml.DocumentElement.SetAttribute("autSara", responseObj.strTrame.Substring(84, 6));
                                    paramXml.DocumentElement.SetAttribute("fonoCuenta", txtCelular.Text.Trim());
                                    paramXml.DocumentElement.SetAttribute("secuCarr", responseObj.strTrame.Substring(90, 6));

                                    //Imprimir factura con datos recibidos desde corresponsal
                                    Imprimir(responseObj.PrintFactura, paramXml, ref impresionRespuesta);
                                }

                                LimpiarControles();

                                ResponseRecarga = responseObj.errDescription
                                                    + Environment.NewLine
                                                    + impresionRespuesta;
                            }
                            else
                            {
                                ResponseRecarga = "No se ha realizado la transacción" 
                                                    + Environment.NewLine 
                                                    + Environment.NewLine 
                                                    + responseObj.errDescription;
                            }
                        }
                        else
                        {
                            ResponseRecarga = "El servicio no devolvió la respuesta esperada pero aún puede que la recarga si se haya realizado, por favor contacte a administrador para verificar, si la recarga no se realizó inténtelo otra vez en unos breves momentos";
                        }
                    }
                    else
                    {
                        Logger.LogMessage(Common.Enum.LogTypes.Info, "Recargas", "RealizarRecarga", "Trama de respuesta no esperada",
                                          string.Format("Trama: {0}", responseXml));
                        ResponseRecarga = "El servicio retornó una respuesta no esperada, por favor inténtelo otra vez en unos breves momentos";
                    }

                }
                else
                {
                    Logger.LogMessage(Common.Enum.LogTypes.Info, "Recargas", "RealizarRecarga", "Trama de envío no valida",
                                      string.Format("Trama: {0}", xmlWithValues));
                    ResponseRecarga = "La trama que se quiere enviar no es válida, esto puede deberse a un breve mantenimiento por favor inténtelo de nuevo. La trama fue guardada en el log para que pueda ser revisada";
                }

            }
            catch (Exception ex)
            {
                Logger.LogMessage(Common.Enum.LogTypes.Error, "Recargas", "RealizarRecarga", ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                ResponseRecarga = "Su solicitud no pudo ser realizada debido a un inconveniente temporal, esto puede deberse a un breve mantenimiento, por favor inténtelo una vez más";
            }
            finally
            {
                On_Off_Controles(true);
            }
        }

        private string ResponseRecarga { get; set; }
        private bool EstadoControles { get; set; }

        private bool ValidarSolicitudRecarga()
        {
            if (cmbOperadora.SelectedIndex == 0)
            {
                //MessageBox.Show("No ha seleccionado la operadora");
                Control.Common.General.GetMensajeToList(355);
                cmbOperadora.Focus();
                return false;
            }
            if (txtCelular.Text.Trim().Length == 0)
            {
                //MessageBox.Show("No ha indicado el nro de cuenta");
                Control.Common.General.GetMensajeToList(356);
                txtCelular.Focus();
                return false;
            }
            if (txtValor.Text.Trim().Replace(".", "").Length == 0)
            {
                //MessageBox.Show("No ha indicado el valor");
                Control.Common.General.GetMensajeToList(357);
                txtValor.Focus();
                return false;
            }
            if (txtPago.Text.Trim().Replace(".", "").Length == 0)
            {
                //MessageBox.Show("No ha indicado el pago");
                Control.Common.General.GetMensajeToList(358);

                txtPago.Focus();
                return false;
            }
            if (decimal.Parse(txtPago.Text) < decimal.Parse(txtValor.Text))
            {
                //MessageBox.Show("El pago no puede ser menor al valor solicitado");
                Control.Common.General.GetMensajeToList(359);
                txtPago.Focus();
                return false;
            }

            string PrmRecaudoMin = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmRecaudoMin").FirstOrDefault().Descripcion;
            string PrmRecaudoMax = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmRecaudoMax").FirstOrDefault().Descripcion;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if (decimal.Parse(txtValor.Text.Trim()) < decimal.Parse(Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmRecaudoMin").FirstOrDefault().Descripcion))
            {
                parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[PrmRecaudoMin]", valor = PrmRecaudoMin });
                Control.Common.General.GetMensajeToList(360, parametros);


                //MessageBox.Show("El valor es menor al minimo permitido actual: " + Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmRecaudoMin").FirstOrDefault().Descripcion);
                txtCelular.Focus();
                return false;
            }
            if (decimal.Parse(txtValor.Text.Trim()) > decimal.Parse(Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmRecaudoMax").FirstOrDefault().Descripcion))
            {
                parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[PrmRecaudoMax]", valor = PrmRecaudoMax });
                Control.Common.General.GetMensajeToList(361, parametros);

                //MessageBox.Show("El valor es mayor al maximo permitido actual: " + Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmRecaudoMax").FirstOrDefault().Descripcion);
                txtCelular.Focus();
                return false;
            }

            return true;
        }
        
        BackgroundWorker bgw;
        private void ProcesarRecargaBackGround()
        {
            if (!ValidarSolicitudRecarga()) return;

            paneLoading.Visible = true;

            if (bgw == null)
            {
                bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            }
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
            bgw.RunWorkerAsync();
            System.Threading.Thread.Sleep(500);
        }
        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            RealizarRecarga(); 
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            paneLoading.Visible = false;
            cmbOperadora.Focus();
            MessageBox.Show(ResponseRecarga);
        }

        private void On_Off_Controles(bool estado)
        {
            btnRealizarRecarga.Enabled = estado;
            EstadoControles = estado;
            cmbOperadora.Enabled = estado;
            txtCelular.Enabled = estado;
            txtValor.Enabled = estado;
            txtPago.Enabled = estado;
            btnCambiarCliente.Enabled = estado;
            btnLimpiar.Enabled = estado;
            btnKbd.Enabled = estado;
        }
        
        private void ChangeLogo()
        {
            try
            {
                var empSeleccionada = (LirisLibCorrBan.Models.InstRecaudos)cmbOperadora.SelectedItem.DataBoundItem;
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

        private void Imprimir(string strXmlFactura, System.Xml.XmlDocument paramXml, ref string respuesta)
        {
            var recibo = String.Empty;

            try
            {
                //Transfomar xml factura corresponsal a comprobante impresion liris
                recibo = ClsCorrBan.TransformarXmlCorrReciboAReciboLiris(strXmlFactura, paramXml);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(Common.Enum.LogTypes.Error, "Recargas", "Imprimir", ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                respuesta = "Sin impresión de comprobante";

                recibo = respuesta;
            }

            //Enviar a imprimir
            Common.Printer.Imprimir(recibo, 3, 11);
        }

        #endregion

        #region Support Methods

        private string ConstructStrTrame(string xmlStr)
        {
            //TipoMensaje:0200@TipoTrans:2:E:I@Sec:6:I:0@SecCarr:6:I:0@ValorTotal:14:I:0@CodSegCarrier:8:D: @ClavCarrier:8:D: @CodCarrier:5:D: @Age:5:D: @Term:8:D: @Oper:5:D: @CodProdu:2:I:0@Cta:20:D: @AutSara:6:I:0@Ref:30:D: @FchTrans:8:I:0@HoraTras:6:I:0@Canal:3:I:0@Mon:3:I:0

            string response = string.Empty;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);

            var template = ((System.Xml.XmlElement)doc.SelectNodes("/root/req")[0]).Attributes["st"].Value;

            var listParameters = template.ToString().Split('@');
            foreach (var param in listParameters)
            {
                var tags = param.Split(':');
                var value = string.Empty;
                switch (tags[0])
                {
                    case "TipoTrans": //Para recargas solo saldremos con Tiempo Aire (02)
                        value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmTipoTransDefault").FirstOrDefault().Descripcion;
                        break;
                    case "Sec":
                        value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmSecuCajaDefe").FirstOrDefault().Descripcion;
                        break;
                    case "SecCarr":
                        value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmSecuCarrierDefe").FirstOrDefault().Descripcion;
                        break;
                    case "ValorTotal":
                        if ((txtValor.Text.Trim().IndexOf('.') == -1))
                        {
                            value = txtValor.Text.Trim() + "00";
                        }
                        else
                        {
                            var splitValor = txtValor.Text.Trim().Split('.');

                            value = splitValor[0] + splitValor[1].PadRight(2, '0');
                        }
                        break;
                    case "CodSegCarrier":
                        value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarSecurityRecargas").FirstOrDefault().Descripcion;
                        break;
                    case "ClavCarrier":
                        value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "ClaveCarrie").FirstOrDefault().Descripcion;
                        break;
                    case "CodCarrier":
                        value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarCarrier").FirstOrDefault().Descripcion;
                        break;
                    case "Age":
                        value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarAgency").FirstOrDefault().Descripcion;
                        break;
                    case "Term":
                        value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarTerminal").FirstOrDefault().Descripcion;
                        break;
                    case "Oper":
                        value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarOperator").FirstOrDefault().Descripcion;
                        break;
                    case "CodProdu":
                        value = ((LirisLibCorrBan.Models.InstRecaudos)cmbOperadora.SelectedItem.DataBoundItem).CodigoEmpresa;
                        break;
                    case "Cta":
                        value = txtCelular.Text.Trim();
                        break;
                    case "AutSara":
                        value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmAutSARADefault").FirstOrDefault().Descripcion;
                        break;
                    case "Ref":
                        value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmRefOpcionalDefe").FirstOrDefault().Descripcion;
                        break;
                    case "FchTrans":
                        value = DateTime.Now.ToString("yyyyMMdd");
                        break;
                    case "HoraTras":
                        value = string.Concat(DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0'));
                        break;
                    case "Canal":
                        value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmCanal").FirstOrDefault().Descripcion;
                        break;
                    case "Mon":
                        value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmMoneda").FirstOrDefault().Descripcion;
                        break;
                    default:
                        value = tags[1];
                        break;
                }

                if (tags.Count() > 2)
                    //Example: @TipoTrans:2:I:0
                    response += GetPadByParam(value, int.Parse(tags[1]), char.Parse(tags[2]), char.Parse(tags[3]));
                else
                    //Example: @TipoMensaje:0200
                    response += value;

            }

            return response;
        }

        private string GetXmlWithValues(string xmlStr, string strTrame)
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
            if ((txtValor.Text.Trim().IndexOf('.') == -1))
            {
                value = txtValor.Text.Trim();
            }
            else
            {
                var splitValor = txtValor.Text.Trim().Split('.');
                if (splitValor[1].Trim().Length > 0)
                    value = splitValor[0] + "." + splitValor[1].PadRight(2, '0');
                else
                    value = splitValor[0].Trim();
            }

            doc.Root.Element("req").Attribute("tv").Value = value;
            doc.Root.Element("req").Attribute("tr").Value = ((LirisLibCorrBan.Models.InstRecaudos)cmbOperadora.SelectedItem.DataBoundItem).IdIRecaudos.ToString();
            doc.Root.Element("req").Attribute("ts").Value = "0";
            doc.Root.Element("req").Attribute("tc").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmCanal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("tt").Value = Control.CorrBan.ClsCorrBan.ListCatalogosDet.Where(x => x.IdCatalogosRecaudo == 4 && x.Codigo == "PrmTipoTransDefault").FirstOrDefault().Descripcion; 
            doc.Root.Element("req").Attribute("to").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarOperator").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("tl").Value = Control.CorrBan.ClsCorrBan.CatalogoConfiguraEmp.LstCatalogosRecaudoDet.Where(x => x.Codigo == "CarTerminal").FirstOrDefault().Descripcion;
            doc.Root.Element("req").Attribute("cc").Value = txtCelular.Text.Trim();      
            //cod Cliente Auditoria : No puede usarse el mismo que le enviamos a WU xq WU necesita recibir el codigo vacio en caso de CONSUMIDOR FINAL y nosotros necesitamos registrar ese codigo en nuestra tabla
            doc.Root.Element("req").Attribute("ac").Value = (SelectedCustomer != null) ? SelectedCustomer.ACCOUNTNUM : Common.GlobalParameters.IdConsumidorFinal;
            //TipoRecaudoTransaccion: 12 - EmpresaRecarga en CatalogoDet / 02 TiempoAire / 
            //IdMetodo: 8 - Guardar Recarga en SoapCorrBanDet
            doc.Root.Element("req").Attribute("te").Value = "12";
            doc.Root.Element("req").Attribute("mt").Value = "8";
            //Date
            doc.Root.Element("req").Attribute("df").Value = DateTime.Now.ToString("s");
            //strTrame
            doc.Root.Element("req").Attribute("st").Value = strTrame;
            //cusDocumentId
            doc.Root.Element("req").Attribute("ci").Value = (SelectedCustomer != null) ? SelectedCustomer.ACCOUNTNUM : string.Empty;
            //cusTypeDocument
            doc.Root.Element("req").Attribute("td").Value = (SelectedCustomer != null) ? ((SelectedCustomer.ACCOUNTNUM.Trim().Length == 10) ? "C" : "R") : string.Empty;
            //cusFName
            doc.Root.Element("req").Attribute("cf").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(splitNames[0]) : StringHelper.ToAlphaNumeric(SelectedCustomer.NAME)) : string.Empty;
            //cusLName
            doc.Root.Element("req").Attribute("cl").Value = (SelectedCustomer != null) ? ((splitNames.Count() > 1) ? StringHelper.ToAlphaNumeric(SelectedCustomer.NAME.Replace(splitNames[0], "").Trim()) : string.Empty) : string.Empty; //Foo
            //cusMName
            doc.Root.Element("req").Attribute("cm").Value = string.Empty; //Bar
            //cusType
            doc.Root.Element("req").Attribute("ct").Value = (SelectedCustomer != null) ? "N" : string.Empty; 
            //cusAdress
            doc.Root.Element("req").Attribute("ca").Value = (SelectedCustomer != null) ? ((!string.IsNullOrEmpty(SelectedCustomer.ADDRESS)) ? StringHelper.ToAlphaNumeric(SelectedCustomer.ADDRESS) : "No") : "No";
            //cusPhone
            doc.Root.Element("req").Attribute("cp").Value = (SelectedCustomer != null) ? StringHelper.ToAlphaNumeric(SelectedCustomer.PHONE) : string.Empty;
            //cusAltPhone
            doc.Root.Element("req").Attribute("ch").Value = string.Empty;
            //cusEmail
            doc.Root.Element("req").Attribute("e").Value = (SelectedCustomer != null) ? SelectedCustomer.EMAIL : string.Empty;
            //invCity
            doc.Root.Element("req").Attribute("iy").Value = string.Empty;

            return doc.ToString();
        }

        private string GetPadByParam(string value, int totalWidth, char charOrientation, char charFill)
        {
            string response = string.Empty;

            if (charOrientation == 'D')
                return value.PadRight(totalWidth, charFill);
            else
                return value.PadLeft(totalWidth, charFill);
        }

        private void HandleKeyPressNumeric(object sender, KeyPressEventArgs e)
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

        private void LimpiarControles()
        {
            cmbOperadora.SelectedIndex = 0;
            txtCelular.Text = string.Empty;
            txtValor.Text = string.Empty;
            txtPago.Text = string.Empty;
            LimpiarCliente();
        }

        #endregion

        private void btnReimprimir_Click(object sender, EventArgs e)
        {
            Reimpresion frmReimpresion = new Reimpresion();
            frmReimpresion.ShowDialog();
        }

        private void btnCF_Click(object sender, EventArgs e)
        {
            LimpiarCliente();
        }

        private void LimpiarCliente()
        {
            SelectedCustomer = null;
            lblIdentificacion.Text = Common.GlobalParameters.IdConsumidorFinal;
            lblCliente.Text = Common.GlobalParameters.NameConsumidorFinal;
        }
    }
}
