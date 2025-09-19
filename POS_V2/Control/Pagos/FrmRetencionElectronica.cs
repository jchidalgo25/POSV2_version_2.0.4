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
using System.Data.SqlClient;
using eComp.PDF;

namespace POS.Control.Pagos
{
   
    public partial class FrmRetencionElectronica : Telerik.WinControls.UI.RadForm
    {
        private Models.RetencionElectronica _objRetEletronica = new Models.RetencionElectronica();

        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        private pos_customer SelectedCustomer { get; set; }

        private bool _gretAnioAnterior = false;
        private DateTime _gdtUltDiaAnio ;
        private string _gContribEspecial;
        private decimal _gOlguraValRet = 0.01M;

        //eevv 2020-04-03
        private decimal gsubtotalRteFte1, gsubtotalRteFte175;
        DateTime dtFechaInicioVigenciaRteFte175 = new DateTime(2020, 04, 01);
        bool bFacturaAplicaRetFte175 = false;

        public FrmRetencionElectronica()
        {
            InitializeComponent();
        }

        private void FrmRetencionElectronica_Load(object sender, EventArgs e)
        {
            var pos = new POSEntities();
            cmbFecAut.SetToNullValue();
            cmbFecReg.SetToNullValue(); 
            
        }

        private void cmbRetIva_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            var pos = new POSEntities();
            //if (cmbRetIva.Text!="" && cmbRetIva.Text!= "POS.Models.vw_RETIVA")
            //{
            //    txtRetIVA.Text = (Math.Round(decimal.Parse(txtIva.Text) * (pos.vw_RETIVA.Where(x => x.CONCEPT == cmbRetIva.Text).First().RETENTION / 100), 2, MidpointRounding.ToEven)).ToString();
            //}
            //txtTotalRet.Text = (decimal.Parse(txtRet.Text) + decimal.Parse(txtRetIVA.Text)).ToString();
        }

        private void btnKbd_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();
            
        }


        private void btnDevolver_Click(object sender, EventArgs e)
        {
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            try
            {

                if(Control.Common.GlobalParameters.BloqRetFteAnio > 0)
                {
                    if (this._objRetEletronica.FechaEmision.Year > Control.Common.GlobalParameters.BloqRetFteAnio)
                    {
                        if (Control.Common.GlobalParameters.BloqRetFte == true)
                        {
                            for (int i = 0; i < gridItems.Rows.Count(); i++)
                            {
                                if(Control.Common.GlobalParameters.BloqRetFteCodigo.Contains(gridItems.Rows[i].Cells[0].Value.ToString()) && Control.Common.GlobalParameters.BloqRetFteMsj != "" )
                                {
                                    //MessageBox.Show(this, Control.Common.GlobalParameters.BloqRetFteMsj);

                                    string BloqRetFteMsj = Control.Common.GlobalParameters.BloqRetFteMsj;
                                    parametros.Add(new ParametrosMensajes() { codigo = "[BloqRetFteMsj]", valor = BloqRetFteMsj });
                                    Control.Common.General.GetMensajeToList(453, parametros);
                                    return;
                                }
                            }
                        }
                    }
                }

                //Fingerprint.VerificationForm Verifier = new Fingerprint.VerificationForm(Common.GlobalParameters.DataForFingerprint, null);
                //Verifier.Tag = "adm";
                //if (Verifier.ShowDialog() != DialogResult.OK)
                //{
                //    MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                //    return;
                //}

                string numeroret = txtEstab.Text.Replace("_", "").PadLeft(3, '0') + txtPtoEmi.Text.Replace("_", "").PadLeft(3, '0') + txtNumeracion.Text.Replace("_", "").PadLeft(9, '0');
                if (numeroret.Replace("0", "") == "" || numeroret.Length < 15 || txtAut.Text.Replace("_", "") == "" || txtAut.Text.Replace("_", "").PadLeft(10, '0').Length < 10 || cmbFecAut.Value.Year.ToString() == "1")
                {
                    //MessageBox.Show(this, "Debe llenar la informacion y/o \n*Verifique el número de Retención debe ser de 15 digitos\n*Verifique el numero de Autorización debe ser de 10 digitos");
                    Control.Common.General.GetMensajeToList(454);
                    return;
                }
                if (string.IsNullOrEmpty(txtCedula.Text))
                {
                    //MessageBox.Show(this, "Ingrese la identificación del cliente");
                    Control.Common.General.GetMensajeToList(455);
                    return;
                }


                decimal retCliente = Math.Round(this._objRetEletronica.ValorRetFte, 2, MidpointRounding.AwayFromZero);
                decimal retPos = Math.Round((Math.Round((gsubtotalRteFte1 * (1M / 100)), 2, MidpointRounding.AwayFromZero) + Math.Round((gsubtotalRteFte175 * (1.75M / 100)), 2, MidpointRounding.AwayFromZero)), 2, MidpointRounding.AwayFromZero);
                decimal direfencia = retPos - retCliente;
                decimal diferenciaPermitidaMax = Control.Common.GlobalParameters.MontoDiferenciaPermitidaEnDevolucionRetencion;
                decimal diferenciaPermitidaMin = Math.Round(Control.Common.GlobalParameters.MontoDiferenciaPermitidaEnDevolucionRetencion * -1, 2, MidpointRounding.AwayFromZero);

                //if (retCliente != retPos)
                if (!Common.GlobalParameters.BloqRetFte)
                {
                    if (direfencia >= diferenciaPermitidaMax || direfencia <= diferenciaPermitidaMin || retCliente == 0.0M || retPos == 0.0M)
                    {
                        //MessageBox.Show(this, "Existe inconsistencia entre los calculos de Retención a la Fuente, Revisar inmediatamente.");
                        Control.Common.General.GetMensajeToList(456);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Retención Electrónica", "Grabar", "Existe inconsistencia entre los calculos de Retención " + this._objRetEletronica.NumRetencion + ", Revisar inmediatamente.(" + retCliente.ToString() + "!=" + retPos.ToString() + ")");
                        return;
                    }
                }
                using (var db = new POSEntities())
                {
                    using (System.Data.Entity.DbContextTransaction dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //this._objRetEletronica.ValorRetFte 
                           
                            var estab = this._objRetEletronica.NumFactura.Substring(0, 3);
                            var ptoemi = this._objRetEletronica.NumFactura.Substring(3, 3);
                            var fact = long.Parse(this._objRetEletronica.NumFactura.Substring(6, 9));
                            var factura = db.core_factura.FirstOrDefault(x => x.establecimiento == estab && x.punto_emision == ptoemi && x.numero == fact && x.cliente == txtCedula.Text);

                            if (factura == null)
                            {
                                //Control.Common.WinForm.ShowMessage("La factura asociada a la retención no existe");
                                Control.Common.General.GetMensajeToList(457);
                                return;
                            }
                            else
                            {
                                if (!(factura.fecha_creacion >= DateTime.Now.AddDays(-1 * (Control.Common.GlobalParameters.Retencion_DiasVigencia + Control.Common.GlobalParameters.Retencion_DiasVigenciaAdicional)).Date
                                    && factura.fecha_creacion <= DateTime.Now.Date)
                                    )
                                {
                                    parametros = new List<ParametrosMensajes>();
                                    parametros.Add(new ParametrosMensajes() { codigo = "[Retencion_DiasVigencia]", valor = Control.Common.GlobalParameters.Retencion_DiasVigencia.ToString() });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[fecha_creacion]", valor = factura.fecha_creacion.ToString("dd/MM/yyyyy") });

                                    //Control.Common.WinForm.ShowMessage("El tiempo máximo para la devolución de dinero es " + Control.Common.GlobalParameters.Retencion_DiasVigencia.ToString() + " días a partir de la emisión de la factura ( fecha de emisión: " + factura.fecha_creacion.ToString("dd/MM/yyyyy") + ") ");
                                    Control.Common.General.GetMensajeToList(458, parametros);
                                    return;
                                }
                            }

                            if (db.core_retencion.Any(x => x.num_retencion == this._objRetEletronica.NumRetencion && x.factura_id == factura.id))
                            {
                                //Control.Common.WinForm.ShowMessage("La retención ya fue registrada.");
                                Control.Common.General.GetMensajeToList(459);
                                return;
                            }

                            core_retencion newRet = new core_retencion();
                            newRet.fecha_creacion = DateTime.Now;
                            newRet.fecha_modificacion = DateTime.Now;
                            newRet.fecha_autorizacion = this._objRetEletronica.FechaEmision;
                            newRet.factura_id = factura.id;
                            newRet.num_retencion = this._objRetEletronica.NumRetencion;
                            newRet.num_autorizacion = this._objRetEletronica.NumAutorizacion;
                            newRet.num_factura = this._objRetEletronica.NumFactura;
                            //newRet.valor_ret_fte = this._objRetEletronica.ValorRetFte;
                            newRet.valor_base = this._objRetEletronica.ValorBase;

                            //eevv 2020-04-03 .ini
                            newRet.valor_ret_fte = (gsubtotalRteFte1 * (1M / 100));//this._objRetEletronica.ValorRetFte;
                            newRet.valor_base1 = gsubtotalRteFte1;
                            newRet.valor_base175 = gsubtotalRteFte175;
                            newRet.valor_ret_fte175 = (gsubtotalRteFte175 * (1.75M / 100));
                            //eevv .fin

                            newRet.CodRetIVA = string.IsNullOrEmpty(this._objRetEletronica.CodRetIVA) ? "0" : this._objRetEletronica.CodRetIVA;
                            newRet.CodPorcRetIVA = this._objRetEletronica.CodPorcRetIVA;
                            newRet.ValorBaseIVA = this._objRetEletronica.ValorBaseIVA;
                            newRet.ValorRetIVA = this._objRetEletronica.ValorRetIVA;
                            newRet.ConceptoRetIVA = this._objRetEletronica.ConceptoRetIVA;
                            newRet.TipoRetencion = 1;
                            newRet.UsuarioCreacion = Control.Common.GlobalParameters.Usuario;
                            newRet.UsuarioModifica = Control.Common.GlobalParameters.Usuario;
                            newRet.Cliente = txtCedula.Text;
                            newRet.Establecimiento = Control.Common.GlobalParameters.Establecimiento;
                            newRet.PuntoEmision = Control.Common.GlobalParameters.PuntoEmision;
                            newRet.Procesado = 0;
                            newRet.ID_Caja = Program.ID_Caja_POS;
                            db.core_retencion.Add(newRet);
                            db.SaveChanges();

                            // generar pdf
                            GenerarPDFLocal(_objRetEletronica.XML);

                            Core_CargaDocumento cardoc = new Core_CargaDocumento();
                            cardoc.Retencion_Id = newRet.id;
                            cardoc.UsuarioCreacion = Control.Common.GlobalParameters.Usuario;
                            cardoc.Documento = this._objRetEletronica.PDF;
                            cardoc.IdCaja = Program.ID_Caja_POS;
                            cardoc.FechaCreacion = DateTime.Now;
                            cardoc.UsuarioModifica = Control.Common.GlobalParameters.Usuario;
                            cardoc.FechaModifcacion = DateTime.Now;
                            cardoc.Procesado = 0;
                            db.Core_CargaDocumento.Add(cardoc);
                            db.SaveChanges();

                            _objRetEletronica.ImprimirRecibo();

                            dbContextTransaction.Commit();
                            //Control.Common.WinForm.ShowMessage("Devolución realizada exitosamente.");
                            Control.Common.General.GetMensajeToList(460);
                            this.Close();
                            
                        }
                        catch (Exception ex)
                        {
                            string msj = ex.ToString();

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Retención Electrónica", "Grabar", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

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
                                          "POS.Control.Pagos.FrmRetencionElectronica",
                                          "grabar",
                                          Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                          ex.StackTrace),
                            false,
                            String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Retención Electrónica", "Grabar", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }
                            dbContextTransaction.Rollback();
                            Control.Common.General.GetMensajeToList(462);
                            //Control.Common.WinForm.ShowMessage("No se pudo grabar la transacción en este momento, esto puede deberse a una breve interrupción en la comunicación, inténtelo nuevamente en unos momentos");
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "btnDevolver_Click", "Imposible devolver retención en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos devolver la retención en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(462);
            }
        }

        /// <summary>
        /// Obtiene los valores de los subtotal del 1% y 1.75% y el valor de la retención.
        /// 
        /// </summary>
        private void GetRetencion175(core_factura factura)
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
                                gsubtotalRteFte1 = detalle.Where(x => x.Item.subtotal > 0 && x.PosItem.retporc == 1).Sum(x => x.Item.subtotal);

                            }
                            catch (Exception)
                            {

                                gsubtotalRteFte1 = 0;
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
                                gsubtotalRteFte175 = detalle.Where(x => x.Item.subtotal > 0 && x.PosItem.retporc == 1.75M).Sum(x => x.Item.subtotal);
                            }
                            catch (Exception)
                            {

                                gsubtotalRteFte175 = 0;
                            }

                            try
                            {
                                dsctoRet175 = detalle.Where(x => x.Item.subtotal > 0 && x.PosItem.retporc == 1.75M).Sum(x => x.Item.descuento);
                            }
                            catch (Exception)
                            {
                                dsctoRet175 = 0;
                            }


                            gsubtotalRteFte1 = gsubtotalRteFte1 - dsctoRet1;
                            gsubtotalRteFte175 = gsubtotalRteFte175 - dsctoRet175;
                            txtRet175Input.Enabled = false;
                            txtRetInput.Enabled = false;
                            txtRetInput.Text = (gsubtotalRteFte1 * (1M / 100)).ToString();
                            txtRet175Input.Text = (gsubtotalRteFte175 * (1.75M / 100)).ToString();

                        }
                    }
                }
                else
                {
                    gsubtotalRteFte1 = 0;
                    gsubtotalRteFte175 = 0;
                    txtRetInput.Text = (gsubtotalRteFte1 * (1M / 100)).ToString();
                    txtRet175Input.Text =(gsubtotalRteFte175 * (1.75M /100)).ToString();
                    txtRet175Input.Enabled = false;
                    txtRetInput.Enabled = false;
                }

                txtSubtotal1.Text = gsubtotalRteFte1.ToString();
                txtsubtotal175.Text = gsubtotalRteFte175.ToString();



            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public void GenerarPDFLocal(string xml)
        {             
            try
            {
                BinaryWriter Writer = null;
                string Name = _objRetEletronica.PDF;
                ///string contents = File.ReadAllText(@"C:\DocAnbaque\GeneracionPDFRetencion\xmlretencion.xml");

                eComp.PDF.GeneraPDF PDFDATA = new eComp.PDF.GeneraPDF();
                var dato = PDFDATA.GeneraComprobanteRetencion(xml, Properties.Settings.Default.RETENCION_PATHLOGO);

                // Create a new stream to write to the file
                Writer = new BinaryWriter(File.OpenWrite(Name));

                // Writer raw data                
                Writer.Write(dato);
                Writer.Flush();
                Writer.Close();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.RetencionElectronica", "GenerarPDFLocal", "Imposible generar comprobante en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                throw ex;
            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            //string cedula = txtCedula.Text;
            //cedula = cedula.Replace(" ","");
            if (InputLanguage.CurrentInputLanguage.Culture.EnglishName.ToUpper() != "ENGLISH (UNITED STATES)")
            {
                foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
                {
                    if (lang.Culture.EnglishName.ToUpper() == "ENGLISH (UNITED STATES)")
                    {
                        InputLanguage.CurrentInputLanguage = lang;
                    }
                }
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                using (POSEntities db = new POSEntities())
                {
                    var cliente = db.pos_customer.Where(x => x.ACCOUNTNUM == txtCedula.Text).FirstOrDefault();
                    if (cliente != null)
                    {
                        _objRetEletronica.Customer = cliente;
                        txtCedula.Text = cliente.ACCOUNTNUM;
                        lblNombre.Text = cliente.NAME;
                        txtClave.ReadOnly = true;
                        txtEstab.Focus();
                    }
                    else
                    {
                        //MessageBox.Show(this, "Cliente No Existe");
                        Control.Common.General.GetMensajeToList(464);
                        txtCedula.Text = null;
                        lblNombre.Text = null;
                    }
                }
            }
        }

        private void txtClave_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    LimpiarPantalla();
                    txtEstab.Text = "";
                    txtPtoEmi.Text = "";
                    txtNumeracion.Text = "";


                    if (CargarRetencion())
                    {
                        KeyPressEventArgs evento = new KeyPressEventArgs((char)Keys.Enter);
                        txtCedula_KeyPress(sender, evento);
                    }
                    else
                    { 
                        txtClave.Text = "";
                        txtClave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "txtClave_KeyPress", "Imposible consultar retención en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos consultar retención en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(465);
            }
        }

        public void LimpiarPantalla()
        {
            cmbFecAut.SetToNullValue();
            txtAut.Text = "";
            cmbFecReg.SetToNullValue();
            gridItems.Rows.Clear();
            txtTotal.Text = "";
            txtFactura.Text = "";
        }

        private void CargarParametros()
        {
            bool parametrosCargados = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var recibo = db.core_recibo.Where(x => x.identificador == "RECIBO_RETENCION_ELECTRONICA").FirstOrDefault();
                    if (recibo == null)
                    {
                        throw new Exception("No hay recibo 'RECIBO_RETENCION_ELECTRONICA' en core_recibo");
                    }
                    _objRetEletronica.Recibo = recibo.cuerpo;

                    var param = db.core_parametro.Where(x => x.identificador == "RETENCION_PATHPDF" && x.valor == Common.GlobalParameters.Establecimiento).FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'RETENCION_PATHPDF' en core_parametro");
                    }
                    Control.Common.GlobalParameters.RetencionElect_PathPDF = param.parametro2;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "CargarParametros", "Imposible terminar de cargar parámetros en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                parametrosCargados = false;
            }

            if (!parametrosCargados)
            {
                //Control.Common.WinForm.ShowMessage("No se pudieron cargar todos los parámetros necesarios para el formulario en este momento, esto pudo deberse a una breve interrupción en la comunicación. Vuélvalo a intentar en unos momentos");
                Control.Common.General.GetMensajeToList(466);
                this.Close();
            }
        }

        private void FrmRetencionElectronica_Shown(object sender, EventArgs e)
        {
            txtCedula.Focus();
            CargarParametros();
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
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnBuscarRetencion_Click(object sender, EventArgs e)
        {
            try
            {
                LimpiarPantalla();
                txtClave.Text = "";
                if (string.IsNullOrEmpty(txtCedula.Text))
                {
                    //MessageBox.Show(this, "Ingrese la identificación del cliente");
                    Control.Common.General.GetMensajeToList(467);
                    txtCedula.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtEstab.Text) || string.IsNullOrEmpty(txtPtoEmi.Text) || string.IsNullOrEmpty(txtNumeracion.Text))
                {
                    //MessageBox.Show(this, "Ingrese la retención: establecimiento, punto de emisión y número");
                    Control.Common.General.GetMensajeToList(468);
                    txtEstab.Focus();
                    return;
                }

                if (txtNumeracion.Text.Length < 9)
                    txtNumeracion.Text = txtNumeracion.Text.PadLeft(9, '0');

                if (!CargarRetencion())
                {
                    txtEstab.Text = "";
                    txtPtoEmi.Text = "";
                    txtNumeracion.Text = "";
                    txtEstab.Focus();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "btnBuscarRetencion_Click", "Imposible consultar retención en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos consultar retención en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(469);

            }
        }

        private void txtNumeracion_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    LimpiarPantalla();
                    txtClave.Text = "";
                    if (string.IsNullOrEmpty(txtCedula.Text))
                    {
                        //MessageBox.Show(this, "Ingrese la identificación del cliente");
                        Control.Common.General.GetMensajeToList(467);
                        txtCedula.Focus();
                        return;
                    }
                    if (string.IsNullOrEmpty(txtEstab.Text) || string.IsNullOrEmpty(txtPtoEmi.Text) || string.IsNullOrEmpty(txtNumeracion.Text))
                    {
                        //MessageBox.Show(this, "Ingrese la retención: establecimiento, punto de emisión y número");
                        Control.Common.General.GetMensajeToList(468);
                        txtEstab.Focus();
                        return;
                    }

                    if (txtNumeracion.Text.Length < 9)
                        txtNumeracion.Text = txtNumeracion.Text.PadLeft(9, '0');

                    if (!CargarRetencion())
                    {
                        txtEstab.Text = "";
                        txtPtoEmi.Text = "";
                        txtNumeracion.Text = "";
                        txtEstab.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "btnBuscarRetencion_Click", "Imposible consultar retención en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos consultar retención en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(469);
            }
        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            var frmSearch = new Control.Clientes.SearchClient(SelectedCustomer);
            frmSearch.ShowDialog();
            SelectedCustomer = frmSearch.SelectedCustomer;
            if (SelectedCustomer != null)
            {
                _objRetEletronica.Customer = SelectedCustomer;
                txtCedula.Text = SelectedCustomer.ACCOUNTNUM;
                lblNombre.Text = SelectedCustomer.NAME;
                txtClave.ReadOnly = true;
                txtEstab.Focus();
            }
        }

        public bool CargarRetencion()
        {
            try
            {
                SqlConnection conexion = new SqlConnection(Properties.Settings.Default.CONECTA_AX);
                SqlCommand comando = default(SqlCommand);

                _objRetEletronica.ValorRetFte = 0;
                _objRetEletronica.ValorBase = 0;

                using (conexion)
                {
                    conexion.Open();

                    comando = new SqlCommand("sp_getRetencionElectronica", conexion);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("@claveacceso", SqlDbType.VarChar, 100).Value = txtClave.Text;
                    comando.Parameters.Add("@identificacion", SqlDbType.VarChar, 80).Value = txtCedula.Text;
                    comando.Parameters.Add("@establecimiento", SqlDbType.VarChar, 3).Value = txtEstab.Text;
                    comando.Parameters.Add("@puntoemision", SqlDbType.VarChar, 3).Value = txtPtoEmi.Text;
                    comando.Parameters.Add("@numero", SqlDbType.VarChar, 9).Value = txtNumeracion.Text;

                    SqlDataReader dr = comando.ExecuteReader();
                    if (dr.HasRows)
                    {
                        decimal total = 0;
                        gridItems.Rows.Clear();
                        while (dr.Read())
                        {
                            _objRetEletronica.NumRetencion = dr.GetValue(2).ToString() + dr.GetValue(3).ToString() + dr.GetValue(4).ToString();
                            _objRetEletronica.NumFactura = dr.GetValue(12).ToString();
                            _objRetEletronica.NumAutorizacion = dr.GetValue(1).ToString();
                            _objRetEletronica.FechaAutorizacion = dr.GetDateTime(5);
                            _objRetEletronica.FechaEmision = dr.GetDateTime(0);

                            _objRetEletronica.CodigoImpuesto = int.Parse(dr.GetValue(14).ToString());
                            if (_objRetEletronica.CodigoImpuesto == 1)
                            {
                                _objRetEletronica.ValorRetFte += decimal.Parse(dr.GetValue(10).ToString());//Ref:vjfranco 12/12/2022 acumula retención
                                _objRetEletronica.ValorBase += decimal.Parse(dr.GetValue(8).ToString());
                            }

                            if (_objRetEletronica.CodigoImpuesto == 2)
                            {
                                _objRetEletronica.ValorBaseIVA = decimal.Parse(dr.GetValue(8).ToString());
                                _objRetEletronica.ValorRetIVA = decimal.Parse(dr.GetValue(10).ToString());
                                _objRetEletronica.CodRetIVA = dr.GetValue(7).ToString();
                                _objRetEletronica.CodPorcRetIVA = decimal.Parse(dr.GetValue(9).ToString());
                            }

                            _objRetEletronica.ConceptoRetIVA = "Retención IVA";
                            _objRetEletronica.XML = dr.GetValue(13).ToString();
                            txtClave.Text = dr.GetValue(15).ToString();
                            cmbFecAut.Text = dr.GetValue(5).ToString();
                            txtAut.Text = dr.GetValue(1).ToString();
                            txtEstab.Text = dr.GetValue(2).ToString();
                            txtPtoEmi.Text = dr.GetValue(3).ToString();
                            txtNumeracion.Text = dr.GetValue(4).ToString();
                            cmbFecReg.Text = dr.GetValue(0).ToString();
                            txtFactura.Text = dr.GetValue(6).ToString();
                            Telerik.WinControls.UI.GridViewDataRowInfo dataRowInfo = new Telerik.WinControls.UI.GridViewDataRowInfo(this.gridItems.MasterView);
                            dataRowInfo.Cells[0].Value = dr.GetValue(7).ToString();
                            dataRowInfo.Cells[1].Value = dr.GetValue(8).ToString();
                            dataRowInfo.Cells[2].Value = dr.GetValue(9).ToString();
                            dataRowInfo.Cells[3].Value = dr.GetValue(10).ToString();
                            gridItems.Rows.Add(dataRowInfo);
                            total = total + decimal.Parse(dr.GetValue(10).ToString());
                            txtCedula.Text = dr.GetValue(11).ToString();
                            _objRetEletronica.ConceptoTransaccion = "";
                            _objRetEletronica.ContribuyenteEspecial = !string.IsNullOrEmpty(dr.GetValue(16).ToString());
                            _gContribEspecial = dr.GetValue(16).ToString();
                        }                       
                        txtTotal.Text = "$ " + total.ToString();
                        _objRetEletronica.Total = total;
                        _objRetEletronica.PDF = Control.Common.GlobalParameters.RetencionElect_PathPDF + "RET" + DateTime.Now.ToString("ddMMyyyy") + "_" + _objRetEletronica.NumRetencion + ".pdf";

                        var fact = GetFactura();
                        this.GetRetencion175(fact);

                        if (!ValidaRetencionDiciembre(_objRetEletronica.FechaEmision, _objRetEletronica.NumFactura))
                        {
                            btnDevolver.Enabled = false;
                        }
                       /* else if (!ValidaDetalleImpuesto())
                        {
                            btnDevolver.Enabled = false;
                        }*/
                        else if (total > 0 && _objRetEletronica.ContribuyenteEspecial)
                        {
                            btnDevolver.Enabled = true;                           
                            btnDevolver.Focus();
                        }
                        else if (!_objRetEletronica.ContribuyenteEspecial && !string.IsNullOrEmpty(_objRetEletronica.CodRetIVA))
                        {
                            //Control.Common.WinForm.ShowMessage("La Retención no es válida, el cliente no es contribuyente especial");
                            Control.Common.General.GetMensajeToList(470);
                            btnDevolver.Enabled = false;
                        }
                        else if (total <= 0)
                        {
                            //Control.Common.WinForm.ShowMessage("La Retención no es válida, el total a devolver es cero.");
                            Control.Common.General.GetMensajeToList(471);
                            btnDevolver.Enabled = false;
                        }
                        else
                            btnDevolver.Enabled = false;

                        return true;
                    }
                    else
                    {
                        //Control.Common.WinForm.ShowMessage("La Retención no existe, o no pertenece a este cliente");
                        Control.Common.General.GetMensajeToList(472);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }            
        }      
        
        private core_factura GetFactura()
        {
            core_factura facturaRetorna= null;
            try
            {
                var estab = this._objRetEletronica.NumFactura.Substring(0, 3);
                var ptoemi = this._objRetEletronica.NumFactura.Substring(3, 3);
                var fact = long.Parse(this._objRetEletronica.NumFactura.Substring(6, 9));
                using (var db = new POSEntities())
                {
                    var factura = db.core_factura.FirstOrDefault(x => x.establecimiento == estab && x.punto_emision == ptoemi && x.numero == fact && x.cliente == txtCedula.Text);
                    facturaRetorna = factura;
                }
            }
            catch (Exception)
            {
                
            }

            return facturaRetorna;
        }
        private bool ValidaDetalleImpuesto()
        {
            bool _ret;
            decimal _rteftecalculado, _rtefte_olgura, _rteIVA_olgura;
            decimal _rteIVAcalculado;
            _rtefte_olgura = 0;
            _rteftecalculado = ((_objRetEletronica.ValorBase * 1) / 100);
            _rtefte_olgura = _rteftecalculado + _gOlguraValRet;
            _rteIVAcalculado = 0;
          
            _ret = true;

            if (_gContribEspecial.Equals("C_ESP"))
            {
                
                //_objRetEletronica.ValorBase 
                if (_objRetEletronica.ValorRetFte > _rteftecalculado && _objRetEletronica.ValorRetFte> _rtefte_olgura)
                {
                    _ret= false;
                    //MessageBox.Show("El valor de la retención en la fuente es mayor al valor calculado por el sistema","Validación Retenciones Electrónicas",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(473);

                }

                _rtefte_olgura = 0;
                _rtefte_olgura = _rteftecalculado - _gOlguraValRet;
                if (_objRetEletronica.ValorRetFte < _rteftecalculado && _objRetEletronica.ValorRetFte < _rtefte_olgura)
                {
                    _ret= false;
                    //MessageBox.Show("El valor de la retención en la fuente es menor al valor calculado por el sistema", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(474);

                }

                if(_objRetEletronica.ValorRetIVA==0)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(475);
                    //MessageBox.Show("para Contribuyentes Especiales debe tener el Retención del IVA", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //obligados a llevar contabilidad.
            if (_gContribEspecial.Equals("C_NOR_OBLI"))
            {

                //_objRetEletronica.ValorBase 
                if (_objRetEletronica.ValorRetFte > _rteftecalculado && _objRetEletronica.ValorRetFte > _rtefte_olgura)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(476);
                    //MessageBox.Show("El valor de la retención en la fuente es mayor al valor calculado por el sistema", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _rtefte_olgura = 0;
                _rtefte_olgura = _rteftecalculado - _gOlguraValRet;
                if (_objRetEletronica.ValorRetFte < _rteftecalculado && _objRetEletronica.ValorRetFte < _rtefte_olgura)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(474);
                    //MessageBox.Show("El valor de la retención en la fuente es menor al valor calculado por el sistema", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (_objRetEletronica.ValorRetIVA == 0)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(475);
                    //MessageBox.Show("para Contribuyentes Especiales debe tener el Retención del IVA", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _rteIVAcalculado = ((_objRetEletronica.ValorBaseIVA * 30) / 100);
                _rteIVA_olgura = 0;
                _rteIVA_olgura = _rteIVAcalculado - _gOlguraValRet;

                if (_objRetEletronica.ValorRetIVA > _rteIVAcalculado && _objRetEletronica.ValorRetIVA > _rteIVA_olgura)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(476);
                    //MessageBox.Show("El valor de la retención IVA es mayor al valor calculado por el sistema", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                
                if (_objRetEletronica.ValorRetIVA < _rteIVAcalculado && _objRetEletronica.ValorRetIVA < _rteIVA_olgura)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(474);
                    //MessageBox.Show("El valor de la retención IVA es menor al valor calculado por el sistema", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            //NO obligados a llevar contabilidad.
            if (_gContribEspecial.Equals("C_NOR_NO_OBLIG"))
            {

                //_objRetEletronica.ValorBase 
                if (_objRetEletronica.ValorRetFte > _rteftecalculado && _objRetEletronica.ValorRetFte > _rtefte_olgura)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(476);
                    //MessageBox.Show("El valor de la retención en la fuente es mayor al valor calculado por el sistema", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _rtefte_olgura = 0;
                _rtefte_olgura = _rteftecalculado - _gOlguraValRet;
                if (_objRetEletronica.ValorRetFte < _rteftecalculado && _objRetEletronica.ValorRetFte < _rtefte_olgura)
                {
                    _ret = false;
                    Control.Common.General.GetMensajeToList(474);
                    //MessageBox.Show("El valor de la retención en la fuente es menor al valor calculado por el sistema", "Validación Retenciones Electrónicas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return _ret;

        }

        /// <summary>
        /// 024-003-000066078
        /// </summary>
        /// <param name="dtFechaEmision"></param>
        /// <param name=""></param>
        private bool ValidaRetencionDiciembre( DateTime dtFechaEmision, string numFactura)
        {
            string[] _split;
            string _establecimiento = string.Empty, _puntoemision = string.Empty, _numero = string.Empty;
            long numero = 0;
            DateTime fechaFactura ;
            bool tieneDatfactura = false;
            try
            {   
                //             
                if (DateTime.Now.Month == 1)
                {
                
                    var estab = numFactura.Substring(0, 3);
                    var ptoemi = numFactura.Substring(3, 3);
                    var fact = long.Parse(numFactura.Substring(6, 9));
                    using (var db = new POSEntities())
                    {
                        var factura = db.core_factura.FirstOrDefault(x => x.establecimiento == estab && x.punto_emision == ptoemi && x.numero == fact && x.cliente == txtCedula.Text);

                        if (factura != null)
                        {

                            fechaFactura = factura.fecha_creacion;
                            _gdtUltDiaAnio = new DateTime(fechaFactura.Year, fechaFactura.Month, 31);
                            if (fechaFactura.Month == 12 && dtFechaEmision != _gdtUltDiaAnio)
                            {
                                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[anioFiscal]", valor = fechaFactura.Year.ToString() });
                                Control.Common.General.GetMensajeToList(477, parametros);

                                //Control.Common.WinForm.ShowMessage("Fecha de Emisión de la Retención incorrecto, solo se permite ingresar retenciones del periodo fiscal anterior con fecha 31/12/" + fechaFactura.Year.ToString());
                                return false;
                            }

                           
                        }
                    }
                    /* _split = numFactura.Split('-');
                     if(_split.Count()>0)
                     {
                         _establecimiento    = _split[0];
                         _puntoemision       = _split[1];
                         _numero             = _split[2];
                         numero = Convert.ToInt64(_numero);
                         tieneDatfactura = true;
                     }*/

                    
                }
                

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "ValidaRetencionDiciembre", "Imposible consultar la factura "+ numFactura + " en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos consultar datos de la factura en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(478);

            }

            return true;
        }        
        private void btnReimprimirRecibo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_objRetEletronica.NumRetencion))
                {
                    //Control.Common.WinForm.ShowMessage("Ingrese la retención a reimprimir.");
                    Control.Common.General.GetMensajeToList(479);
                    return;
                }
                using (var db = new POSEntities())
                {
                    var estab = this._objRetEletronica.NumFactura.Substring(0, 3);
                    var ptoemi = this._objRetEletronica.NumFactura.Substring(3, 3);
                    var fact = long.Parse(this._objRetEletronica.NumFactura.Substring(6, 9));
                    var factura = db.core_factura.FirstOrDefault(x => x.establecimiento == estab && x.punto_emision == ptoemi && x.numero == fact && x.cliente == txtCedula.Text);
                    if (factura == null)
                    {
                        //Control.Common.WinForm.ShowMessage("La factura asociada a la retención no existe");        
                        Control.Common.General.GetMensajeToList(480);
                    }
                    else
                    {
                        if (db.core_retencion.Any(x => x.num_retencion == this._objRetEletronica.NumRetencion && x.factura_id == factura.id))
                        {
                            _objRetEletronica.ConceptoTransaccion = "Devolución de Retención Electrónica (COPIA)";
                            _objRetEletronica.ImprimirRecibo();
                        }
                        else
                        {
                            //Control.Common.WinForm.ShowMessage("La retencion no ha sido devuelta. Por favor realice la devolución");
                            Control.Common.General.GetMensajeToList(481);
                        }
                            
                    }
                }
            }
            catch (Exception)
            {
                //Control.Common.WinForm.ShowMessage("No se pudo realizar la reimpresión del comprobante.");
                Control.Common.General.GetMensajeToList(482);
            }            
        }
    }    
}
