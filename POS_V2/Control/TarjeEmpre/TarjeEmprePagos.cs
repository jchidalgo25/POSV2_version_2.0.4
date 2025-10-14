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
using System.Data.SqlClient;

namespace POS.Control.CrediEmpre
{
    public partial class TarjeEmprePagos : Telerik.WinControls.UI.RadForm
    {
        private Models.TarjeEmpresa.ClsTarjetaEmpresaPagos _objCobroTarjeEmpre = new Models.TarjeEmpresa.ClsTarjetaEmpresaPagos();       
        pos_customer cliente_actual = null; 

        public TarjeEmprePagos()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
                case Keys.F1:
                    //var frm = new GiftcardTransReprint(_objCobroTarjeEmpre.PieRecibo, _objCobroTarjeEmpre.Recibo);
                    //frm.ShowDialog();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TarjeEmprePagos_Load(object sender, EventArgs e)
        {            
            //tableLayoutPanel_Pagar.Visible = false;

            _objCobroTarjeEmpre.Establecimiento = Control.Common.GlobalParameters.Establecimiento;
            _objCobroTarjeEmpre.PtoEmision = Control.Common.GlobalParameters.PuntoEmision;

            gridPagos.DataSource = _objCobroTarjeEmpre.Pagos;
        } 
         
        private void CalcularTarjeEmpre()
        {
            try
            {
                _objCobroTarjeEmpre.Total = 0;
                if (_objCobroTarjeEmpre.Pagos != null)
                {
                    for (int i = 0; i < _objCobroTarjeEmpre.Pagos.Count; i++)
                    {
                        _objCobroTarjeEmpre.Total = _objCobroTarjeEmpre.Total + _objCobroTarjeEmpre.Pagos[i].Valor;
                        if (_objCobroTarjeEmpre.Pagos[i].Valor == 0)
                        {
                            _objCobroTarjeEmpre.Pagos.Remove(_objCobroTarjeEmpre.Pagos[i]);
                        }
                    }
                }
                lblSaldoActual.Text = string.Format("$ {0}", _objCobroTarjeEmpre.SaldoActual.ToString("N2"));
                lblInteres.Text = string.Format("$ {0}", _objCobroTarjeEmpre.Interes.ToString("N2"));
                lblSaldoFinal.Text = string.Format("$ {0}", _objCobroTarjeEmpre.SaldoFinal.ToString("N2"));

                lblTotal.Text = string.Format("{0:C}", _objCobroTarjeEmpre.Total);
                var cambio = 0M;
                _objCobroTarjeEmpre.ValidarPagos(out cambio,_objCobroTarjeEmpre.SaldoFinal);
                lblCambio.Text = string.Format("{0:C}", this._objCobroTarjeEmpre.Cambio);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjeEmpre.TarjeEmprePagos", "CalcularTarjeEmpre", "Imposible calcular tarejta empresarial en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos calcular el valor de la tarjeta empresarial en este momento, inténtelo nuevamente en unos momentos");

                Control.Common.General.GetMensajeToList(542);
            }
        }

        public void LimpiarPantalla()
        { 
            txtCedula.Text = "";
            lblNombre.Text = "#####";
            lblSaldoActual.Text = "$ 0.00";
            lblInteres.Text = "$ 0.00";
            lblSaldoFinal.Text = "$ 0.00";
            gridItems.Rows.Clear();            
            lblTotal.Text = "$ 0.00";
            lblporprocesar.Text = "$ 0.00";
        }

        private void CambiarTarjetaEmpresarial(string codigo)
        {
            try
            {
                LimpiarPantalla();

                if (Cliente.clienteExiste(codigo))
                {
                    core_tarjetacreditointerno tarjeta = null;
                    core_tarjetacreditointerno tarjetaAdicional = null;
                    cliente_actual = Cliente.getCliente(codigo, out tarjeta, out tarjetaAdicional);

                    if (cliente_actual != null)
                    {
                        setClienteData();
                        _objCobroTarjeEmpre.Customer = cliente_actual;
                        _objCobroTarjeEmpre.TarjeCredInterno = tarjeta;
                        ConsultarFacturasPendientes();
                    }
                    else
                    {
                        clearClienteData();
                        txtTarjetaCode.Text = "";
                        txtTarjetaCode.Focus();
                    }
                    
                    txtTarjetaCode.Clear();
                }
                else
                {
                    //Control.Common.WinForm.ShowMessage("No existe Tarjeta Empresarial para pagos con código '" + codigo + "'");
                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[codigo]", valor = codigo });
                    Control.Common.General.GetMensajeToList(543, parametros);


                    txtTarjetaCode.Text = "";
                    txtTarjetaCode.Focus();
                }
            }
            catch (Exception ex)
            {
                LimpiarPantalla();
                txtTarjetaCode.Text = "";
                txtTarjetaCode.Focus();
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjeEmpre.TarjeEmprePagos", "CambiarTarjetaEmpresarial", "No pudimos obtener la tarjeta empresarial para pagos en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos obtener la tarjeta empresarial para pagos en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(544);

            }
        }

        public void ConsultarFacturasPendientes()
        {
            using (POSEntities db = new POSEntities())
            {
                SqlConnection conexion = new SqlConnection(Properties.Settings.Default.CONECTA_AX);
                SqlCommand comando = default(SqlCommand), comando2 = default(SqlCommand); ;
                using (conexion)
                {
                    conexion.Open();

                    comando = new SqlCommand("sp_getClienteCreditoEmpresa", conexion);
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("@cliente", SqlDbType.VarChar, 40).Value = txtCedula.Text;
                    comando.Parameters.Add("@esCreditoEmpresa", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    comando.ExecuteNonQuery();

                    if (!bool.Parse(comando.Parameters["@esCreditoEmpresa"].Value.ToString()))
                    {
                        //Control.Common.WinForm.ShowMessage("El cliente no pertenece a crédito empresarial");
                        Control.Common.General.GetMensajeToList(545);

                        txtTarjetaCode.Text = "";
                        txtTarjetaCode.Focus();
                        return;
                    }

                    comando2 = new SqlCommand("sp_getFacturasAbiertas", conexion);
                    comando2.CommandType = System.Data.CommandType.StoredProcedure;
                    comando2.Parameters.Add("@cliente", SqlDbType.VarChar, 40).Value = txtCedula.Text;
                    SqlDataReader dr = comando2.ExecuteReader();
                    if (dr.HasRows)
                    {
                        decimal saldoActual = 0, interes = 0;
                        gridItems.Rows.Clear();
                        while (dr.Read())
                        {
                            Telerik.WinControls.UI.GridViewDataRowInfo dataRowInfo = new Telerik.WinControls.UI.GridViewDataRowInfo(this.gridItems.MasterView);
                            dataRowInfo.Cells[0].Value = dr.GetValue(0).ToString();
                            dataRowInfo.Cells[1].Value = dr.GetValue(1).ToString();
                            dataRowInfo.Cells[2].Value = dr.GetValue(2).ToString();
                            gridItems.Rows.Add(dataRowInfo);
                            saldoActual = saldoActual + decimal.Parse(dr.GetValue(2).ToString());
                            interes = decimal.Parse(dr.GetValue(3).ToString());
                        }
                        _objCobroTarjeEmpre.SaldoPorProcesar = (from x in db.TblCobroCreditoEmp
                                                                where x.Cliente_Ax == _objCobroTarjeEmpre.Customer.ACCOUNTNUM && x.Procesado == 0
                                                                select (Decimal?)x.Total).Sum() ?? 0;
                        lblSaldoActual.Text = "$ " + saldoActual.ToString("N2");
                        lblInteres.Text = "$ " + interes.ToString("N2");
                        lblporprocesar.Text = string.Format("$ -{0}", _objCobroTarjeEmpre.SaldoPorProcesar.ToString("N2"));
                        lblSaldoFinal.Text = "$ " + ((saldoActual + interes) - _objCobroTarjeEmpre.SaldoPorProcesar).ToString();
                        _objCobroTarjeEmpre.SaldoActual = saldoActual;
                        _objCobroTarjeEmpre.Interes = interes;
                        _objCobroTarjeEmpre.SaldoFinal = (saldoActual + interes) - _objCobroTarjeEmpre.SaldoPorProcesar;
                        _objCobroTarjeEmpre.Procesado = 0;
                    }
                    else
                    {
                        //Control.Common.WinForm.ShowMessage("No existen facturas pendientes para este cliente");
                        Control.Common.General.GetMensajeToList(546);
                        txtTarjetaCode.Text = "";
                        txtTarjetaCode.Focus();
                    }
                }
            }
        }

        private void setClienteData()
        {
            lblNombre.Text = cliente_actual.NAME;  
            txtCedula.Text = cliente_actual.VATNUM != "" ? cliente_actual.VATNUM : cliente_actual.ACCOUNTNUM;         
        }

        private void clearClienteData()
        {
            cliente_actual = null;
            lblNombre.Text = ""; 
            txtCedula.Text = "";
            lblSaldoActual.Text = "$ 0.00";
            lblInteres.Text = "$ 0.00";
            lblSaldoFinal.Text = "$ 0.00";
            lblTotal.Text = "$ 0.00";
            lblporprocesar.Text = "$ 0.00";
        }

        private void TarjeEmprePagos_Shown(object sender, EventArgs e)
        {
            txtTarjetaCode.Focus();
            CargarParametros();
        }


        private void GrabarTarjetaEmpresarial()
        {
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if ((_objCobroTarjeEmpre.Total - _objCobroTarjeEmpre.Cambio) > _objCobroTarjeEmpre.SaldoFinal)
            {
                //Control.Common.WinForm.ShowMessage("El total pagos excede al total a pagar de deudas");
                Control.Common.General.GetMensajeToList(547);
                txtPagoValor.Clear();
                return;
            }

            if (!btnGrabar.Enabled) return;
            btnGrabar.Enabled = false;
           
            try
            { 
                //Registrar en log el click del boton Grabar y cantidad de Pagos en el momento
                string strLogPagosProductos = "Usuario ha pulsado botón Grabar, la CobroTarjetaEmpresa en este momento es '" + (_objCobroTarjeEmpre == null ? "Objeto _objCobroTarjeEmpre está nulo" : _objCobroTarjeEmpre.GetNumeroFactura()) + "'. Total CobroTarjetaEmpresa: " + lblTotal.Text + ". ";

                if (_objCobroTarjeEmpre == null)
                {
                    strLogPagosProductos += "Informacion de cantidad Pagos no disponible porque el objeto _objCobroTarjeEmpre estaba en nulo";
                }
                else
                {
                    if (_objCobroTarjeEmpre.Pagos == null)
                        strLogPagosProductos += "Cantidad de Pagos no disponible porque el objeto _objCobroTarjeEmpre.Pagos estaba en nulo. ";
                    else
                        strLogPagosProductos += "Cantidad de Pagos: " + _objCobroTarjeEmpre.Pagos.Count.ToString() + ". ";

                    if (_objCobroTarjeEmpre.TarjeCredInterno == null)
                        strLogPagosProductos += "Tarjeta Empresarial no disponible porque el objeto _objCobroTarjeEmpre.TarjeCredInterno estaba en nulo. ";
                    else
                        strLogPagosProductos += "Tarjeta Empresarial: " + _objCobroTarjeEmpre.TarjeCredInterno.codigo + ". ";

                    if (_objCobroTarjeEmpre.Customer == null)
                        strLogPagosProductos += "Cliente no disponible porque el objeto _objCobroTarjeEmpre.Customer estaba en nulo. ";
                    else
                        strLogPagosProductos += "Cliente: " + _objCobroTarjeEmpre.Customer.NAME + " (" + _objCobroTarjeEmpre.Customer.ACCOUNTNUM + "). ";

                }
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjeEmpre.TarjeEmprePagos", "GrabarTarjetaEmpresarial", strLogPagosProductos);
                

                if (_objCobroTarjeEmpre.Validar())
                {
                    if (_objCobroTarjeEmpre.Grabar())
                    {
                        if (!_objCobroTarjeEmpre.ImprimirRecibo())
                        {
                            //Control.Common.WinForm.ShowMessage(_objCobroTarjeEmpre.MsgError);

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[MsgError]", valor = _objCobroTarjeEmpre.MsgError });
                            Control.Common.General.GetMensajeToList(599, parametros);

                        }
                            

                        //Control.Common.WinForm.ShowMessage("Cobro con Tarjeta Empresarial grabado exitosamente. Gracias por preferirnos");
                        Control.Common.General.GetMensajeToList(548);
                        this.Close();
                        return;
                    }
                    else
                    {
                        //Control.Common.WinForm.ShowMessage(_objCobroTarjeEmpre.MsgError);

                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[MsgError]", valor = _objCobroTarjeEmpre.MsgError });
                        Control.Common.General.GetMensajeToList(599, parametros);

                    }
                }
                else
                {
                    //Control.Common.WinForm.ShowMessage("Revise los pagos, los pagos que no sean efectivo no pueden ser mayor a total cuando se usa más de un método de pago.");
                    Control.Common.General.GetMensajeToList(549);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjeEmpre.TarjeEmprePagos", "Grabar", "Imposible finalizar proceso en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No se pudo grabar la transacción en este momento, esto puede deberse a una breve interrupción en la comunicación, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(550);

            }
             
            btnGrabar.Enabled = true;
        }
         

        private void btnEfectivo_Click(object sender, EventArgs e)
        {
            CargarPagoEfectivo();
        }

        private void CargarPagoEfectivo()
        {
            if (_objCobroTarjeEmpre != null && _objCobroTarjeEmpre.SaldoFinal > 0)
            {
                var valor = 0M;
                if (decimal.TryParse(txtPagoValor.Text, out valor) && valor > 0)
                {                    
                    _objCobroTarjeEmpre.AgregarPagoEfectivo(valor);
                    txtPagoValor.Clear();
                    CalcularTarjeEmpre();
                }
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            GrabarTarjetaEmpresarial();
        }

        private void btnTCredito_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjeEmpre.TarjeEmprePagos", "btnTCredito_Click", "Bajo petición de usuario se abre la pantalla pagos de t. credito. Tarjera Empresarial: " + _objCobroTarjeEmpre.GetNumeroFacturaCompleto());
             
            var f = new Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.TarjetaCredito, ref _objCobroTarjeEmpre, GetValorPago());
            f.ShowDialog();
            txtPagoValor.Clear();
            CalcularTarjeEmpre();
        }

        private decimal GetValorPago()
        {
            var valor = 0M;
            decimal.TryParse(txtPagoValor.Text, out valor);
            return valor;
        }

        private void btnPagoBorrar_Click(object sender, EventArgs e)
        {
            var text = txtPagoValor;
            if (text != null)
            {
                if (text.Text.Length > 1)
                    text.Text = text.Text.Substring(0, text.Text.Length - 1);
                else
                    text.Text = "";
            }
        }

        private void pagoBotonEvent(object sender, EventArgs e)
        {
            var text = txtPagoValor;
            if (text != null)
            {
                var button = sender as System.Windows.Forms.Control;
                text.Text = text.Text + button.Text;
                text.Focus();
            }
        }

        private void btnPagoEnter_Click(object sender, EventArgs e)
        {
            CargarPagoEfectivo();
        }

        private void txtPagoValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CargarPagoEfectivo();
            }
        }

        private void btnEliminarPago_Click(object sender, EventArgs e)
        {
            if (gridPagos.SelectedRows.Count > 0)
            {
                var codigo = gridPagos.SelectedRows[0].DataBoundItem as Models.Pago;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjeEmpre.TarjeEmprePagos", "btnEliminarPago_Click", "Acción borrar pago solicitada por usuario sobre pago tipo: " + codigo.Descripcion + ", valor: " + codigo.Valor.ToString("N2"));
                if (codigo.Descripcion == "T. CREDITO")
                {
                    btnTCredito_Click(sender, e);
                }
                else
                {
                    gridPagos.SelectedRows[0].Delete();
                    CalcularTarjeEmpre();
                }
            }
        }
            
        private void btnCheque_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjeEmpre.TarjeEmprePagos", "btnCheque_Click", "Bajo petición de usuario se abre la pantalla pagos de t. credito. CobroTarjetaEmpresa: " + _objCobroTarjeEmpre.GetNumeroFacturaCompleto());            

            var f = new Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.Cheque, ref _objCobroTarjeEmpre, GetValorPago());
            f.ShowDialog();
            txtPagoValor.Clear();
            CalcularTarjeEmpre();
        }

        private void txtTarjetaCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                var val = txtTarjetaCode.Text.Trim();
                if (val.Length > 0)
                {
                    CambiarTarjetaEmpresarial(val);
                }
            }
        }

        private void txtTarjetaCode_Leave(object sender, EventArgs e)
        {
            //if (_objCobroTarjeEmpre != null)
            //{
            //    if (_objCobroTarjeEmpre.Giftcard != null)
            //    {
            //        txtTarjetaCode.Text = _objCobroTarjeEmpre.Giftcard.codigo;
            //    }
            //    else
            //    {
            //        txtTarjetaCode.Clear();
            //    }
            //}
        }

        private void CargarParametros()
        {
            bool parametrosCargados = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var recibo = db.core_recibo.Where(x => x.identificador == "RECIBO_TARJETA_EMPRESARIAL").FirstOrDefault();
                    if (recibo == null)
                    {
                        throw new Exception("No hay recibo 'RECIBO_RETENCION_ELECTRONICA' en core_recibo");
                    }
                    _objCobroTarjeEmpre.Recibo = recibo.cuerpo;                     
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjeEmpre.TarjeEmprePagos", "CargarParametros", "Imposible terminar de cargar parámetros en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                parametrosCargados = false;
            }

            if (!parametrosCargados)
            {
                //Control.Common.WinForm.ShowMessage("No se pudieron cargar todos los parámetros necesarios para el formulario en este momento, esto pudo deberse a una breve interrupción en la comunicación. Vuélvalo a intentar en unos momentos");
                Control.Common.General.GetMensajeToList(551);
                this.Close();
            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        { 
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
                        var t = db.core_tarjetacreditointerno.Single(x => x.identificacion == cliente.ACCOUNTNUM && x.activo == true);

                        core_tarjetacreditointerno tarjetaInterna = null, tarjetaAdicional = null, adi;
                        if (t.identificacionPrincipal == null)
                        {
                            tarjetaInterna = t;                           
                            txtCedula.Text = t.identificacion.Trim();
                        }
                        else
                        {
                            adi = new core_tarjetacreditointerno { activo = t.activo, codigo = t.codigo, core_empresacredito = t.core_empresacredito, cupo = t.cupo, empresa_id = t.empresa_id, fecha_activacion = t.fecha_activacion, fecha_creacion = t.fecha_creacion, fecha_desactivacion = t.fecha_desactivacion, fecha_expiracion = t.fecha_expiracion, fecha_modificacion = t.fecha_modificacion, id = t.id, identificacion = t.identificacion, identificacionPrincipal = t.identificacionPrincipal, nombre_tarjeta = t.nombre_tarjeta, saldo = t.saldo };
                            t = db.core_tarjetacreditointerno.Where(x => x.identificacion == adi.identificacionPrincipal && x.activo == true).OrderByDescending(x => x.fecha_activacion).FirstOrDefault();
                            if (t == null)
                            {

                                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[identificacionPrincipal]", valor = adi.identificacionPrincipal });
                                Control.Common.General.GetMensajeToList(552, parametros);


                                //MessageBox.Show("La tarjeta Delportal deslizada está configurada como adicional sin embargo no existe una tarjeta principal de empleado con cedula '" + adi.identificacionPrincipal + "'. Comunique a rrhh para que ayude al cliente con la regularización");
                                return;
                            }
                            else
                            {
                                tarjetaAdicional = adi;
                                tarjetaInterna = t;
                                
                            }
                        }
                        _objCobroTarjeEmpre.TarjeCredInterno = tarjetaInterna;
                        cliente = db.pos_customer.Where(x => x.ACCOUNTNUM == t.identificacion).FirstOrDefault();

                        _objCobroTarjeEmpre.Customer = cliente;
                        txtCedula.Text = cliente.ACCOUNTNUM;
                        lblNombre.Text = cliente.NAME;

                        ConsultarFacturasPendientes();
                    }
                    else
                    {
                        //MessageBox.Show(this, "Cliente No Existe");
                        Control.Common.General.GetMensajeToList(553);
                        txtCedula.Text = null;
                        lblNombre.Text = null;
                    }
                }
            }
        }         
    }
}
