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
using MensajesLibrary;

namespace POS.Control.Giftcard
{
    public partial class GiftcardSale : Telerik.WinControls.UI.RadForm
    {
        private Models.Giftcard.ClsGiftcardSale _objGiftcardSale = new Models.Giftcard.ClsGiftcardSale();
        private bool _permiteRecargarSoloCompradas;
        private int _cantidadVigencia = 0;
        private Control.Common.Enum.TimeAdditionType _tipoAdicionTiempo;
        private decimal _minimoCompra = 0M;
        private decimal _minimoRecarga = 0M;

        public GiftcardSale()
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
                    var frm = new GiftcardTransReprint(_objGiftcardSale.PieRecibo, _objGiftcardSale.Recibo);
                    frm.ShowDialog();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GiftcardSale_Load(object sender, EventArgs e)
        {
            //tableLayoutPanel_Confirmar.Dock = DockStyle.Bottom;
            tableLayoutPanel_Pagar.Visible = false;

            _objGiftcardSale.Establecimiento = Control.Common.GlobalParameters.Establecimiento;
            _objGiftcardSale.PtoEmision = Control.Common.GlobalParameters.PuntoEmision;

            gridPagos.DataSource = _objGiftcardSale.Pagos;
            
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            //Se valida cuando se agrega el monto 
            //if (_objGiftcardSale.Giftcard == null)
            //{
            //    Control.Common.WinForm.ShowMessage("Antes de confirmar debe haber cargado la Giftcard");
            //    txtGiftcardCode.Focus();
            //    return;
            //}
            //if (_objGiftcardSale.Customer == null)
            //{
            //    Control.Common.WinForm.ShowMessage("Antes de confirmar debe haber cargado al cliente");
            //    txtCedula.Focus();
            //    return;
            //}

            //bool esValidoMontoMinimo = true;
            //switch (_objGiftcardSale.GiftcardSaleType)
            //{
            //    case Common.Enum.GiftcardSaleType.Sale:
            //        if (_objGiftcardSale.PurchaseValue < _minimoCompra)
            //        {
            //            esValidoMontoMinimo = false;
            //            Control.Common.WinForm.ShowMessage("Monto deseado insuficiente. El minimo de compra requerido es de $" + _minimoCompra.ToString("N2"));
            //        }
            //        break;
            //    case Common.Enum.GiftcardSaleType.Recharge:
            //        if (_objGiftcardSale.PurchaseValue < _minimoRecarga)
            //        {
            //            esValidoMontoMinimo = false;
            //            Control.Common.WinForm.ShowMessage("Monto deseado insuficiente. El minimo de recarga requerido es de $" + _minimoRecarga.ToString("N2"));
            //        }
            //        break;
            //    default:
            //        string msj = "El tipo de transacción actual está corrupto. La pantalla se cerrará para evitar inconsistencias. Tipo(GiftcardSaleType): " + _objGiftcardSale.GiftcardSaleType.ToString();
            //        esValidoMontoMinimo = false;
            //        Control.Common.WinForm.ShowMessage(msj);
            //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "btnConfirmar_Click", msj);
            //        this.Close();
            //        return;
            //}

            //if (!esValidoMontoMinimo)
            //{
            //    txtMontoDeseado.Focus();
            //    return;
            //}

            var frm = new GiftcardProforma(ref _objGiftcardSale);
            frm.ShowDialog();

            if (_objGiftcardSale.Confirmed)
            {
                //tableLayoutPanel_Confirmar.Dock = DockStyle.None;
                //tableLayoutPanel_Confirmar.Visible = false;
                tableLayoutPanel_Pagar.Visible = true;
                tableLayoutPanel_Pagar.Dock = DockStyle.Bottom;
                btnGrabar.Visible = true;

                txtGiftcardCode.Enabled = false;
                txtCedula.Enabled = false;
                txtMontoDeseado.Enabled = false;
                btnCargarMonto.Enabled = false;
                btnCFinal.Enabled = false;

                gridItems.Visible = false;

                txtGiftcardCode.Visible = false;
                radLabel1.Visible = false;
                radLabel10.Visible = false;
                radLabel4.Visible = false;
                lblSaldoActual.Visible = false;
                btnConfirmar.Visible = false;
                btnBorrarGiftcard.Visible = false;
                btnCargarMonto.Visible = false;
                txtMontoDeseado.Visible = false;
            }
        }

        private void CargarMontoDeseado()
        {
            try
            {

                if (_objGiftcardSale.Giftcard == null)
                {
                    //Control.Common.WinForm.ShowMessage("Antes de confirmar debe haber cargado la Giftcard");
                    Control.Common.General.GetMensajeToList(378);
                    txtGiftcardCode.Focus();
                    return;
                }
                if (_objGiftcardSale.Customer == null)
                {
                    //Control.Common.WinForm.ShowMessage("Antes de confirmar debe haber cargado al cliente");
                    Control.Common.General.GetMensajeToList(379);
                    txtCedula.Focus();
                    return;
                }

                txtMontoDeseado.Text = txtMontoDeseado.Text.Trim();

                decimal montoDeseado = 0M;

                if (!decimal.TryParse(txtMontoDeseado.Text, out montoDeseado))
                {
                    //Control.Common.WinForm.ShowMessage("Debe indicar un valor valido en monto deseado");
                    Control.Common.General.GetMensajeToList(380);

                    txtMontoDeseado.Focus();
                    return;
                }


                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                switch (_objGiftcardSale.GiftcardSaleType)
                {
                    case Common.Enum.GiftcardSaleType.Sale:
                        if (montoDeseado < _minimoCompra)
                        {
                            //Control.Common.WinForm.ShowMessage("El minimo de compra permitido es " + _minimoCompra.ToString("N2"));

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[minimoCompra]", valor = _minimoCompra.ToString("N2") });
                            Control.Common.General.GetMensajeToList(381, parametros);
                            
                            txtMontoDeseado.Focus();
                            return;
                        }
                        break;
                    case Common.Enum.GiftcardSaleType.Recharge:
                        if (montoDeseado < _minimoRecarga)
                        {
                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[minimoRecarga]", valor = _minimoRecarga.ToString("N2") });
                            Control.Common.General.GetMensajeToList(382, parametros);

                            //Control.Common.WinForm.ShowMessage("El minimo de recarga permitido es " + _minimoRecarga.ToString("N2"));
                            txtMontoDeseado.Focus();
                            return;
                        }
                        break;
                    default:

                        string msj = "El tipo de transacción actual está corrupto. La pantalla se cerrará para evitar inconsistencias. Tipo(GiftcardSaleType): " + _objGiftcardSale.GiftcardSaleType.ToString();
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Giftcard.GiftcardSale", "CargarMontoDeseado", msj);

                        //Control.Common.WinForm.ShowMessage(msj);

                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[GiftcardSaleType]", valor = _objGiftcardSale.GiftcardSaleType.ToString() });
                        Control.Common.General.GetMensajeToList(383, parametros);



                        this.Close();
                        return;
                }

                _objGiftcardSale.PurchaseValue = montoDeseado;

                if (_objGiftcardSale.LstGiftcard == null)
                {
                    _objGiftcardSale.LstGiftcard = new BindingList<core_giftcard>();
                }
                if (_objGiftcardSale.LstGiftcard.Count > 0)
                {
                    var gc = _objGiftcardSale.LstGiftcard.ToList().Find(x => x.codigo == _objGiftcardSale.Giftcard.codigo);
                    if (gc != null)
                    {
                        gc.monto = montoDeseado;
                        gc.tipoTransaccion  = lblTipoTransaccion.Text;
                        gc.tipoTransaccionId = (byte)_objGiftcardSale.GiftcardSaleType;
                           
                    }
                    else
                    {
                        _objGiftcardSale.Giftcard.monto = montoDeseado;
                        _objGiftcardSale.Giftcard.tipoTransaccion  = lblTipoTransaccion.Text;
                        _objGiftcardSale.Giftcard.tipoTransaccionId = (byte)_objGiftcardSale.GiftcardSaleType;
                        _objGiftcardSale.LstGiftcard.Add(_objGiftcardSale.Giftcard);
                    }
                }
                else
                {
                    _objGiftcardSale.Giftcard.monto = montoDeseado;
                    _objGiftcardSale.Giftcard.tipoTransaccion = lblTipoTransaccion.Text;
                    _objGiftcardSale.Giftcard.tipoTransaccionId = (byte)_objGiftcardSale.GiftcardSaleType;
                    _objGiftcardSale.LstGiftcard.Add(_objGiftcardSale.Giftcard);
                }

                this.gridItems.DataSource = _objGiftcardSale.LstGiftcard;
                this.gridItems.Refresh();
                btnConfirmar.Visible = true;

                CalcularGiftcard();
                lblSaldoActual.Text = "$ 0.00";
                txtMontoDeseado.Clear();
                btnConfirmar.Focus();

                if (_objGiftcardSale.LstGiftcard != null)
                {
                    if (_objGiftcardSale.LstGiftcard.Count() > 0)
                    {
                        lblCantGiftcard.Text = _objGiftcardSale.LstGiftcard.Count().ToString() + " Giftcard";
                    }
                    else
                    {
                        lblCantGiftcard.Text = "";
                    }
                }
                else
                {
                    lblCantGiftcard.Text = "";
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "CargarMontoDeseado", "Imposible cargar monto deseado en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void btnCargarMonto_Click(object sender, EventArgs e)
        {

            CargarMontoDeseado();

           
            lblSaldoActual.Text = "$ 0.00";
        }

        private void CalcularGiftcard()
        {
            try
            {
                txtGiftcardCode.Clear();
                if (_objGiftcardSale.Pagos != null)
                {
                    for (int i = 0; i < _objGiftcardSale.Pagos.Count; i++)
                    {
                        if (_objGiftcardSale.Pagos[i].Valor == 0)
                        {
                            _objGiftcardSale.Pagos.Remove(_objGiftcardSale.Pagos[i]);
                        }
                    }
                }

                if (_objGiftcardSale.Giftcard != null)
                {
                    _objGiftcardSale.CurrentValue = _objGiftcardSale.Giftcard.saldo;
                }
                else
                {
                    _objGiftcardSale.CurrentValue = 0M;
                }

                _objGiftcardSale.FutureValue = _objGiftcardSale.PurchaseValue + _objGiftcardSale.CurrentValue;

                lblSaldoNuevo.Text = string.Format("$ {0}", _objGiftcardSale.PurchaseValue.ToString("N2"));
                lblSaldoActual.Text = string.Format("$ {0}", _objGiftcardSale.CurrentValue.ToString("N2"));
                if (_objGiftcardSale.LstGiftcard != null)
                {
                    string total= string.Format("$ {0}", ((decimal)_objGiftcardSale.LstGiftcard.Sum(x => x.monto)).ToString("N2"));
                    lblSaldoFinal.Text = total;  //string.Format("$ {0}", _objGiftcardSale.FutureValue.ToString("N2"));
                    lblTotal.Text = string.Format("$ {0}", ((decimal)_objGiftcardSale.LstGiftcard.Sum(x => x.monto)).ToString("N2"));  //string.Format("{0:C}", _objGiftcardSale.PurchaseValue);

                }
                else
                {
                    lblSaldoFinal.Text = "$ 0.00";
                    lblTotal.Text = "$ 0.00";
                }

                lblRestante.Text = string.Format("{0:C}", _objGiftcardSale.GetSaldoDisplay());
                var cambio = 0M;
                _objGiftcardSale.ValidarPagos(out cambio);
                lblCambio.Text = string.Format("{0:C}", this._objGiftcardSale.Cambio);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "CalcularGiftcard", "Imposible calcular giftcard en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos calcular el valor de la giftcard en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(384);


            }
        }

        private void CambiarCliente(string identificacion)
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    if (db.pos_customer.Any(x => x.VATNUM == identificacion || x.ACCOUNTNUM == identificacion))
                    {
                        _objGiftcardSale.Customer = db.pos_customer.Where(x => x.VATNUM == identificacion || x.ACCOUNTNUM == identificacion).FirstOrDefault();
                        txtGiftcardCode.Focus();
                    }
                    else
                    {

                        if (ValidarIdentificador.ValidarCedula(identificacion)
                                || ValidarIdentificador.ValidarRUCPrivada(identificacion)
                                || ValidarIdentificador.ValidarRUCPublica(identificacion)
                                || ValidarIdentificador.ValidarRUCNatural(identificacion))
                        {
                            
                            var result = Control.Common.General.GetMensajeToList(385);

                            if(result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                            {
                                var f = new Control.Clientes.ClienteForm();
                                f._cliente = null;
                                f.identificacion = identificacion;
                                f.DebeComprobarTarPortal = false;
                                f.ShowDialog();

                                if (f._cliente != null)
                                {
                                    _objGiftcardSale.Customer = f._cliente;                                   
                                    txtGiftcardCode.Focus();
                                }
                            }

                            //if (MessageBox.Show(this, "Cliente no existe!. Desea crearlo?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            //{
                            //    var f = new Control.Clientes.ClienteForm();
                            //    f._cliente = null;
                            //    f.identificacion = identificacion;
                            //    f.DebeComprobarTarPortal = false;
                            //    f.ShowDialog();

                            //    if (f._cliente != null)
                            //    {
                            //        _objGiftcardSale.Customer = f._cliente;
                            //        //txtMontoDeseado.Focus();
                            //        txtGiftcardCode.Focus();
                            //    }
                            //}
                        }
                        else
                        {
                            //Control.Common.WinForm.ShowMessage("Cedula o RUC no Valido!\nSi es Pasaporte Ingreselo en AX ");
                            Control.Common.General.GetMensajeToList(386);
                        }
                    }
                }

                if (_objGiftcardSale.Customer != null)
                {
                    txtCedula.Text = _objGiftcardSale.Customer.ACCOUNTNUM;
                    lblNombre.Text = _objGiftcardSale.Customer.NAME;
                }
                else
                {
                    txtCedula.Clear();
                    lblNombre.Text = "####";
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "CambiarCliente", "Imposible cambiar cliente en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos cambiar cliente en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(387);
            }
        }

        private void CambiarGiftcard(string codigo)
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var giftCard = db.core_giftcard.Where(x => x.codigo == codigo).FirstOrDefault();
                    if (giftCard != null)
                    {
                        bool esGiftcardValidaParaTransaccion = true;
                        //Tipo transaccion por defecto: Venta
                        Common.Enum.GiftcardSaleType tipoTransaccion = Common.Enum.GiftcardSaleType.Sale;

                        if (giftCard.tipo == 1 || !_permiteRecargarSoloCompradas)
                        {
                            if (db.TblVentaTarjeta.Any(x => x.CodigoGiftcard == codigo && x.TipoTransaccion == (byte)Common.Enum.GiftcardSaleType.Sale))
                            {
                                var ventaTarjeta = db.TblVentaTarjeta.Where(x => x.CodigoGiftcard == codigo && x.TipoTransaccion == (byte)Common.Enum.GiftcardSaleType.Sale).FirstOrDefault();
                                tipoTransaccion = Common.Enum.GiftcardSaleType.Recharge;
                            }
                            else
                            {
                                if (giftCard.activo)
                                {
                                    tipoTransaccion = Common.Enum.GiftcardSaleType.Recharge;
                                }
                                else
                                    tipoTransaccion = Common.Enum.GiftcardSaleType.Sale;
                            }
                        }
                        else
                        {
                            esGiftcardValidaParaTransaccion = false;
                            //Control.Common.WinForm.ShowMessage("Lo lamentamos, la giftcard no es válida para ventas/recargas");
                            Control.Common.General.GetMensajeToList(388);
                        }

                        if (esGiftcardValidaParaTransaccion)
                        {
                            _objGiftcardSale.GiftcardSaleType = tipoTransaccion;
                            _objGiftcardSale.Giftcard = giftCard;
                            //if (_objGiftcardSale.LstGiftcard==null)
                            //{
                            //    _objGiftcardSale.LstGiftcard = new BindingList<core_giftcard>();
                            //}
                            //    if (_objGiftcardSale.LstGiftcard.Count>0)
                            //{
                            //    var gc = _objGiftcardSale.LstGiftcard.ToList().Find(x => x.codigo == giftCard.codigo);
                            //    if (gc != null) { _objGiftcardSale.LstGiftcard.Remove(gc); }
                            //    _objGiftcardSale.LstGiftcard.Add(giftCard);
                            //}
                            //else
                            //{
                            //    _objGiftcardSale.LstGiftcard.Add(giftCard);
                            //}

                            txtMontoDeseado.Focus();
                            //txtCedula.Focus();

                            switch (_objGiftcardSale.GiftcardSaleType)
                            {
                                case Common.Enum.GiftcardSaleType.Sale:
                                    lblTipoTransaccion.Text = "Venta";
                                    txtMontoDeseado.NullText = "Mínimo " + string.Format("{0:C}", _minimoCompra);
                                    break;
                                case Common.Enum.GiftcardSaleType.Recharge:
                                    lblTipoTransaccion.Text = "Recarga";
                                    txtMontoDeseado.NullText = "Mínimo " + string.Format("{0:C}", _minimoRecarga);
                                    break;
                                default:
                                    break;
                            }

                            CalcularGiftcard();

                        }
                    }
                    else
                    {
                        //Control.Common.WinForm.ShowMessage("No existe Giftcard para venta con código '" + codigo + "'");

                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[codigo_giftcard]", valor = codigo });
                        Control.Common.General.GetMensajeToList(389, parametros);
                    }
                }

                if (_objGiftcardSale.Giftcard != null)
                {
                    txtGiftcardCode.Text = _objGiftcardSale.Giftcard.codigo;
                }
                else
                {
                    txtGiftcardCode.Clear();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "CambiarGiftcard", "No pudimos obtener la giftcard para venta en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos obtener la giftcard para venta en este momento, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(390);

            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                var val = txtCedula.Text.Trim();
                if (val.Length > 0)
                {
                    CambiarCliente(val);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "txtCedula_KeyPress", "Identificacion: " + val);
                }
            }
        }

        private void txtGiftcardCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            
                if (e.KeyChar == (char)Keys.Enter)
            {
                if (_objGiftcardSale.Customer == null)
                {
                    //Control.Common.WinForm.ShowMessage("Antes de confirmar debe haber cargado al cliente");
                    Control.Common.General.GetMensajeToList(391);
                    txtCedula.Focus();
                    txtGiftcardCode.Text = "";
                    return;
                }

                var val = txtGiftcardCode.Text.Trim();
                if (val.Length > 0)
                {
                    CambiarGiftcard(val);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "txtGiftcardCode_KeyPress", "Codigo: " + val);
                }
            }
        }

        private void GiftcardSale_Shown(object sender, EventArgs e)
        {
            txtCedula.Focus();
            btnConfirmar.Visible = false;
            CargarParametros();
        }

        private void CargarParametros()
        {
            bool parametrosCargados = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //------------------------MINIMO COMPRA--------------------------------------------------------
                    var param = db.core_parametro.Where(x => x.identificador == "GIFTCARDVENTA_MINIMOCOMPRA").FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'GIFTCARDVENTA_MINIMOCOMPRA' en core_parametro");
                    }

                    if (!decimal.TryParse(param.valor, out _minimoCompra))
                    {
                        throw new Exception("Campo 'valor' de registro 'GIFTCARDVENTA_MINIMOCOMPRA' en core_parametro es incorrecto y no se puede parsear a decimal");
                    }

                    if (_minimoCompra < 0)
                    {
                        throw new Exception("Parametro 'GIFTCARDVENTA_MINIMOCOMPRA' tiene configurado un valor incorrecto. Valor debe ser mayor o igual a 0");
                    }

                    txtMontoDeseado.NullText = "Mínimo " + string.Format("{0:C}", _minimoCompra);

                    //------------------------MINIMO RECARGA--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "GIFTCARDVENTA_MINIMORECARGA").FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'GIFTCARDVENTA_MINIMORECARGA' en core_parametro");
                    }

                    if (!decimal.TryParse(param.valor, out _minimoRecarga))
                    {
                        throw new Exception("Campo 'valor' de registro 'GIFTCARDVENTA_MINIMORECARGA' en core_parametro es incorrecto y no se puede parsear a decimal");
                    }

                    if (_minimoRecarga < 0)
                    {
                        throw new Exception("Parametro 'GIFTCARDVENTA_MINIMORECARGA' tiene configurado un valor incorrecto. Valor debe ser mayor o igual a 0");
                    }

                    //------------------------TIEMPO VIGENCIA--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "GIFTCARDVENTA_TIEMPOVIGENCIA").FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'GIFTCARDVENTA_TIEMPOVIGENCIA' en core_parametro");
                    }

                    if (!int.TryParse(param.valor, out _cantidadVigencia))
                    {
                        throw new Exception("Campo 'valor' de registro 'GIFTCARDVENTA_TIEMPOVIGENCIA' en core_parametro es incorrecto y no se puede parsear a int");
                    }

                    if (!Enum.TryParse(param.parametro2, out _tipoAdicionTiempo))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'GIFTCARDVENTA_TIEMPOVIGENCIA' en core_parametro es incorrecto y no se puede parsear a POS.Control.Common.Enum.TimeAdditionType");
                    }

                    if (_cantidadVigencia < 0)
                    {
                        throw new Exception("Parametro 'GIFTCARDVENTA_TIEMPOVIGENCIA' tiene configurado un valor incorrecto en campo 'valor'. Valor de minimo de tiempo debe ser mayor o igual a 0");
                    }

                    //------------------------RECARGA SOLO COMPRADAS--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "GIFTCARDVENTA_RECARGASOLOCOMPRADAS").FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'GIFTCARDVENTA_RECARGASOLOCOMPRADAS' en core_parametro");
                    }

                    int valorPermiteRecargaSoloCompradas;
                    if (!int.TryParse(param.valor, out valorPermiteRecargaSoloCompradas))
                    {
                        throw new Exception("Campo 'valor' de registro 'GIFTCARDVENTA_RECARGASOLOCOMPRADAS' en core_parametro es incorrecto y no se puede parsear a int");
                    }

                    _permiteRecargarSoloCompradas = valorPermiteRecargaSoloCompradas == 1 ? true : false;

                    var recibo = db.core_recibo.Where(x => x.identificador == "GIFTCARDVENTA_RECIBO").FirstOrDefault();
                    if (recibo == null)
                    {
                        throw new Exception("No hay recibo 'GIFTCARDVENTA_RECIBO' en core_recibo");
                    }

                    _objGiftcardSale.Recibo = recibo.cuerpo;

                    recibo = db.core_recibo.Where(x => x.identificador == "GIFTCARDVENTA_PIEPAGINA").FirstOrDefault();
                    if (recibo == null)
                    {
                        throw new Exception("No hay recibo 'GIFTCARDVENTA_PIEPAGINA' en core_recibo");
                    }

                    _objGiftcardSale.PieRecibo = recibo.cuerpo;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "CargarParametros", "Imposible terminar de cargar parámetros en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                parametrosCargados = false;
            }

            if (!parametrosCargados)
            {
                Control.Common.General.GetMensajeToList(392);
                //Control.Common.WinForm.ShowMessage("No se pudieron cargar todos los parámetros necesarios para el formulario en este momento, esto pudo deberse a una breve interrupción en la comunicación. Vuélvalo a intentar en unos momentos");
                this.Close();
            }
        }

        private void GrabarGiftcard()
        {
            if (!btnGrabar.Enabled) return;
            btnGrabar.Enabled = false;

            try
            {
                //Registrar en log el click del boton Grabar y cantidad de Pagos en el momento
                string strLogPagosProductos = "Usuario ha pulsado botón Grabar, la VentaGiftcard en este momento es '" + (_objGiftcardSale == null ? "Objeto _objGiftcardSale está nulo" : _objGiftcardSale.GetNumeroFactura()) + "'. Total VentaGiftcard: " + lblTotal.Text + ". ";
                if (_objGiftcardSale == null)
                {
                    strLogPagosProductos += "Informacion de cantidad Pagos no disponible porque el objeto _objGiftcardSale estaba en nulo";
                }
                else
                {
                    if (_objGiftcardSale.Pagos == null)
                        strLogPagosProductos += "Cantidad de Pagos no disponible porque el objeto _objGiftcardSale.Pagos estaba en nulo. ";
                    else
                        strLogPagosProductos += "Cantidad de Pagos: " + _objGiftcardSale.Pagos.Count.ToString() + ". ";

                    switch (_objGiftcardSale.GiftcardSaleType)
                    {
                        case Common.Enum.GiftcardSaleType.Sale:
                            strLogPagosProductos += "Tipo: Venta. ";
                            break;
                        case Common.Enum.GiftcardSaleType.Recharge:
                            strLogPagosProductos += "Tipo: Recarga. ";
                            break;
                        default:
                            strLogPagosProductos += "Tipo: Desconocido. ";
                            break;
                    }

                    if (_objGiftcardSale.Giftcard == null)
                        strLogPagosProductos += "Giftcard no disponible porque el objeto _objGiftcardSale.Giftcard estaba en nulo. ";
                    else
                        strLogPagosProductos += "Giftcard: " + _objGiftcardSale.Giftcard.codigo + ". ";

                    if (_objGiftcardSale.Customer == null)
                        strLogPagosProductos += "Cliente no disponible porque el objeto _objGiftcardSale.Customer estaba en nulo. ";
                    else
                        strLogPagosProductos += "Cliente: " + _objGiftcardSale.Customer.NAME + " (" + _objGiftcardSale.Customer.ACCOUNTNUM + "). ";

                }
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Giftcard.GiftcardSale", "GrabarGiftcard", strLogPagosProductos);


                _objGiftcardSale.CantidadVigencia = _cantidadVigencia;
                _objGiftcardSale.TipoAdicionTiempo = _tipoAdicionTiempo;

                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();


                if (_objGiftcardSale.Validar())
                {

                    
                    //if (_objGiftcardSale.Grabar())
                    if (_objGiftcardSale.GarbarSQL())
                    {
                        if (!_objGiftcardSale.ImprimirRecibo()) {

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[MsgError]", valor = _objGiftcardSale.MsgError });
                            Control.Common.General.GetMensajeToList(570, parametros);


                            //Control.Common.WinForm.ShowMessage(_objGiftcardSale.MsgError);
                        }
                            

                        //Control.Common.WinForm.ShowMessage("Saldo Giftcard grabado exitosamente. Gracias por su compra");
                        Control.Common.General.GetMensajeToList(393);

                        this.Close();
                        return;
                    }
                    else
                    {
                        //Control.Common.WinForm.ShowMessage(_objGiftcardSale.MsgError);
                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[MsgError]", valor = _objGiftcardSale.MsgError });
                        Control.Common.General.GetMensajeToList(570, parametros);
                    }
                }
                else
                {
                    //Control.Common.WinForm.ShowMessage("Revise los pagos, los pagos que no sean efectivo no pueden ser mayor a total cuando se usa más de un método de pago.");
                    Control.Common.General.GetMensajeToList(394);

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardSale", "GrabarGiftcard", "Imposible finalizar proceso en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No se pudo grabar la transacción en este momento, esto puede deberse a una breve interrupción en la comunicación, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(395);
            }

            btnGrabar.Enabled = true;
        }

        private void txtMontoDeseado_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CargarMontoDeseado();
            }
        }

        //private bool RevisarCambioEnTotal(string forma_pago, string codigo)
        //{
        //    if (_factura.buscarDescuentosFormaPago(forma_pago, codigo))
        //    {
        //        var f = new POS.Control.Pagos.CalculoPago(_factura);
        //        f.StartPosition = FormStartPosition.CenterScreen;
        //        f.ShowDialog();
        //        calcularFactura();
        //        return true;
        //    }
        //    return false;
        //}

        private void btnEfectivo_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Giftcard.GiftcardSale", "btnEfectivo_Click", "Pago en efectivo: " + _objGiftcardSale.GetNumeroFacturaCompleto());
            CargarPagoEfectivo();
        }

        private void CargarPagoEfectivo()
        {
            
            if (_objGiftcardSale != null && _objGiftcardSale.GetTotal() > 0)
            {
                var valor = 0M;
                if (decimal.TryParse(txtPagoValor.Text, out valor) && valor > 0)
                {
                    _objGiftcardSale.AgregarPagoEfectivo(valor);
                    txtPagoValor.Clear();
                    CalcularGiftcard();
                }
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            GrabarGiftcard();
        }

        private void btnTCredito_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Giftcard.GiftcardSale", "btnTCredito_Click", "Bajo petición de usuario se abre la pantalla pagos de t. credito. VentaGiftcard: " + _objGiftcardSale.GetNumeroFacturaCompleto());

            var f = new Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.TarjetaCredito, ref _objGiftcardSale, GetValorPago());
            f.ShowDialog();
            txtPagoValor.Clear();
            CalcularGiftcard();
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
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Giftcard.GiftcardSale", "btnEliminarPago_Click", "Acción borrar pago solicitada por usuario sobre pago tipo: " + codigo.Descripcion + ", valor: " + codigo.Valor.ToString("N2"));
                if (codigo.Descripcion == "T. CREDITO")
                {
                    btnTCredito_Click(sender, e);
                }
                else
                {
                    gridPagos.SelectedRows[0].Delete();
                    CalcularGiftcard();
                }
            }
        }

        private void btnCFinal_Click(object sender, EventArgs e)
        {
            CambiarCliente(Control.Common.GlobalParameters.IdConsumidorFinal);
        }

        private void txtGiftcardCode_Leave(object sender, EventArgs e)
        {
            if (_objGiftcardSale != null)
            {
                if (_objGiftcardSale.Giftcard != null)
                {
                    txtGiftcardCode.Text = _objGiftcardSale.Giftcard.codigo;
                }
                else
                {
                    txtGiftcardCode.Clear();
                }
            }
        }

        private void btnBorrarProducto_Click(object sender, EventArgs e)
        {

        }

        private void btnBorrarGiftcard_Click(object sender, EventArgs e)
        {
            if (gridItems.SelectedRows.Count > 0)
            {
                {
                    var item = gridItems.SelectedRows[0].DataBoundItem as core_giftcard;
                    _objGiftcardSale.LstGiftcard.Remove(item);
                }
            }
            else
            {
                //MessageBox.Show(this, "La lista de giftcard está vacía!", "Giftcard", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Control.Common.General.GetMensajeToList(396);
            }
        }

       

        private void txtGiftcardCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyData == Keys.Tab)
            {
                //MessageBox.Show("Tab");
                e.IsInputKey = true;
            }
        }

     
    }
}
