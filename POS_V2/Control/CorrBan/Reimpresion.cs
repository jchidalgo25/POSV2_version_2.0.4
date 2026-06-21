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

namespace POS.Control.CorrBan
{
    public partial class Reimpresion : Telerik.WinControls.UI.RadForm
    {

        #region Constructores

        public Reimpresion()
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

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {
                case Keys.Enter:
                    SearchRecipes();
                    break;

                case Keys.Escape:
                    this.Close();
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
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
                gridRecaudaciones.DataSource = LirisLibCorrBan.Business.RecaudacionBO.GetRecaudacionesReprint(12, dtpFecha.Value, txtCuenta.Text.Trim(), Program.ID_Caja_POS);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Reimpresion", "SearchRecipes", "No fue posible obtener la lista de comprobantes para reimpresion, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //MessageBox.Show("No fue posible obtener la lista de comprobantes para reimpresion, esto puede deberse a una breve eventualidad, por favor inténtelo nuevamente. Si el problema persiste contacte al administrador");
                Control.Common.General.GetMensajeToList(362);
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
                    Control.Common.General.GetMensajeToList(363);
                    return;
                }

                if (((IEnumerable<dynamic>)gridRecaudaciones.DataSource).Count() == 0)
                {
                    //MessageBox.Show("No existen registros para realizar la reimpresión");
                    Control.Common.General.GetMensajeToList(364);
                    return;
                }

                recaudacion = gridRecaudaciones.SelectedRows[0].DataBoundItem as dynamic;

                trama = (string)recaudacion.GetType().GetProperty("Trama").GetValue(recaudacion, null);
                XDocument xDoc = XDocument.Parse(trama);
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
                    string impresionRespuesta = string.Empty;

                    //Preparar parametros impresion
                    var paramXml = new XmlDocument();
                    paramXml.LoadXml("<root />");
                    paramXml.DocumentElement.SetAttribute("codCorr", (string)recaudacion.GetType().GetProperty("CodigoEmpresa").GetValue(recaudacion, null));
                    paramXml.DocumentElement.SetAttribute("valorRecibido", ((decimal)recaudacion.GetType().GetProperty("Valor").GetValue(recaudacion, null)).ToString("N2"));
                    paramXml.DocumentElement.SetAttribute("valorCambio", "0.00");
                    paramXml.DocumentElement.SetAttribute("RecActivaCode", responseObj.RecActivaCode);
                    paramXml.DocumentElement.SetAttribute("CarSequentialSARA", responseObj.CarSequentialSARA);
                    paramXml.DocumentElement.SetAttribute("RecCltid", responseObj.RecCltid);
                    paramXml.DocumentElement.SetAttribute("autSara", responseObj.strTrame.Substring(84, 6));
                    paramXml.DocumentElement.SetAttribute("fonoCuenta", (string)recaudacion.GetType().GetProperty("Cuenta").GetValue(recaudacion, null));
                    paramXml.DocumentElement.SetAttribute("secuCarr", responseObj.strTrame.Substring(90, 6));

                    //Imprimir factura con datos recibidos desde corresponsal
                    Imprimir(responseObj.PrintFactura, paramXml, ref impresionRespuesta);

                    if (!string.IsNullOrEmpty(impresionRespuesta))
                        MessageBox.Show(impresionRespuesta);
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Reimpresion", "btnImprimir_Click", "No fue posible armar el objeto responseObj que permite la reimpresión por lo que se le indicó al usuario que el comprobante no aplicaba para reimpresión");
                    //MessageBox.Show("No se puede realizar la acción porque la transacción original no aplicaba para impresión");
                    Control.Common.General.GetMensajeToList(365);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Reimpresion", "btnImprimir_Click", "No fue posible reimprimir el comprobante solicitado, Trama: " + (string.IsNullOrEmpty(trama) ? "el objeto dinamico recaudacion o bien la trama que debería estar embebida está nula" : trama) + ", a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //MessageBox.Show("No fue posible reimprimir el comprobante solicitado, esto puede deberse a una breve eventualidad por favor inténtelo nuevamente. Si el problema persiste contacte al administrador");
                Control.Common.General.GetMensajeToList(366);
            }
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

        private void txtCuenta_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
