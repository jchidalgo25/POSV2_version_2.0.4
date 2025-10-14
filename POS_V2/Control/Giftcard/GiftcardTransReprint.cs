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

namespace POS.Control.Giftcard
{
    public partial class GiftcardTransReprint : Telerik.WinControls.UI.RadForm
    {
        private string _pieRecibo;
        public string PieRecibo
        {
            get { return _pieRecibo; }
            set { _pieRecibo = value; }
        }

        private string _recibo;
        public string Recibo
        {
            get { return _recibo; }
            set { _recibo = value; }
        }

        public GiftcardTransReprint(string pieRecibo, string reciboFormato)
        {
            InitializeComponent();
            _pieRecibo = pieRecibo;
            _recibo = reciboFormato;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Reimprimir()
        {
            try
            {
                int idOpcion = 0;
                string msjResultado = string.Empty;
                DateTime fecha = DateTime.Now;

                using (POSEntities db = new POSEntities())
                {
                    var lastSale = db.TblVentaTarjeta
                                                .Where(x => x.CodigoGiftcard == txtGiftcard.Text.Trim()
                                                            &&
                                                            x.Establecimiento == Common.GlobalParameters.Establecimiento
                                                            &&
                                                            x.PuntoEmision == Common.GlobalParameters.PuntoEmision
                                                            &&
                                                            (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.FechaCreacion) == (DateTime)System.Data.Entity.DbFunctions.TruncateTime(fecha.Date))
                                                .OrderByDescending(x => x.FechaCreacion).FirstOrDefault();

                    List<POS_VOUCHER> listaVouchers = new List<POS_VOUCHER>();
                    string fechaFormateada = DateTime.Now.ToString("yyyyMMdd");
                    string ptoEmisionVoucher = Common.GlobalParameters.Establecimiento + Common.GlobalParameters.PuntoEmision;

                    if (lastSale == null)
                    {
                        msjResultado += "- En esta caja, no existe hoy una venta/recarga de giftcard con código: " + txtGiftcard.Text.Trim();
                    }
                    else
                    {

                        var user = db.auth_user.Where(x => x.username == lastSale.Usuario).FirstOrDefault();

                        var _objGiftcardSale = new Models.Giftcard.ClsGiftcardSale();
                        _objGiftcardSale.Customer = db.pos_customer.Where(x => x.ACCOUNTNUM == lastSale.Cliente_Ax).FirstOrDefault();
                        _objGiftcardSale.Giftcard = db.core_giftcard.Where(x => x.codigo == lastSale.CodigoGiftcard).FirstOrDefault();
                        _objGiftcardSale.GiftcardSaleType = (Control.Common.Enum.GiftcardSaleType)lastSale.TipoTransaccion;
                        _objGiftcardSale.PurchaseValue = lastSale.Total;
                        _objGiftcardSale.Cambio = 0;
                        _objGiftcardSale.PieRecibo = PieRecibo;
                        _objGiftcardSale.Recibo = Recibo;
                        _objGiftcardSale.FechaTrans = lastSale.FechaCreacion;
                        _objGiftcardSale.AtendidoPor = (user == null) ? string.Empty : (user.last_name + " " + user.first_name);
                        _objGiftcardSale.LstGiftcard = new BindingList<core_giftcard>();

                        var lastSaleLst = db.TblVentaTarjeta
                                                .Where(x => x.VentaPagoId == lastSale.VentaPagoId).OrderByDescending(x => x.FechaCreacion);

                        if (lastSaleLst != null)
                        {
                            //Lista GiftCard Vendidas 
                            foreach (var gc in lastSaleLst)
                            {
                                var _objgiftC = new core_giftcard();

                                _objgiftC.codigo = gc.CodigoGiftcard;
                                _objgiftC.monto = gc.Total;
                                _objgiftC.tipoTransaccion = (gc.TipoTransaccion == 1) ? "Recarga" : "Venta";

                                _objGiftcardSale.LstGiftcard.Add(_objgiftC);

                                foreach (var pago in gc.TblVentaPago)
                                {
                                    _objGiftcardSale.Pagos.Add(new Pago() { Descripcion = pago.Tipo_Id, Valor = pago.Valor });
                                }

                                var _listaVouchers = db.POS_VOUCHER
                                                                .Where(x => x.FACTURA == gc.CodigoGiftcard
                                                                            &&
                                                                            x.TIPOTRANSACCION != null
                                                                            &&
                                                                            x.ANULADO == false
                                                                            &&
                                                                            x.FECHACONSUMO == fechaFormateada
                                                                            &&
                                                                            x.PUNTOEMISION == ptoEmisionVoucher)
                                                                .ToList();

                                if (_listaVouchers != null)
                                {
                                    listaVouchers.AddRange(_listaVouchers);
                                }


                            }
                        }

                        //foreach (var pago in lastSale.TblVentaPago)
                        //{
                        //    _objGiftcardSale.Pagos.Add(new Pago() { Descripcion = pago.Tipo_Id, Valor = pago.Valor });
                        //}

                        if (!_objGiftcardSale.ImprimirRecibo())
                            msjResultado += _objGiftcardSale.MsgError;
                        else
                            msjResultado += "- Comprobante de pago reimpreso";

                    }


                    if (listaVouchers == null)
                    {
                        msjResultado += Environment.NewLine + "- No se ha podido consultar la lista de vouchers de esta giftcard, esto puede deberse a que la red esté ocupada. Por favor inténtelo nuevamente en unos instantes";
                    }
                    else
                    {
                        if (listaVouchers.Count() > 0)
                        {
                            foreach (var voucher in listaVouchers)
                            {
                                if (!Pagos.ClsPagos.ReimprimirVoucher(Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito, voucher))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardTransReprint", "Reimprimir", "El voucher con id " + voucher.id.ToString() + " perteneciente a la giftcard con código " + txtGiftcard.Text.Trim() + " no pudo ser reimpreso, esto puede deberse a una interrupción en la comunicación, a continuacion las excepciones encontradas - " + Pagos.ClsPagos.Msj);
                                    msjResultado += Environment.NewLine + String.Format("- El voucher con id {0} no pudo ser impreso, esto puede deberse a una interrupción en la comunicación. Por favor inténtelo nuevamente en unos instantes", voucher.id.ToString());
                                }
                            }
                            msjResultado += Environment.NewLine + "- Voucher(s) de cobros con pinpad de la giftcard reimpresos";
                        }
                        else
                        {
                            msjResultado += Environment.NewLine + "- En esta caja, no hubo cobros con Pinpad el día de hoy para la giftcard " + txtGiftcard.Text.Trim();
                        }
                    }
                }


                //Control.Common.WinForm.ShowMessage(msjResultado);


                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[msjResultado]", valor = msjResultado });
                Control.Common.General.GetMensajeToList(397, parametros);
                
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Giftcard.GiftcardTransReprint", "Reimprimir", "Imposible finalizar proceso en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No se pudo realizar impresiones en este momento, esto puede deberse a una breve interrupción en la comunicación, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(398);

            }
        }
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            Reimprimir();
        }

        private void txtGiftcard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                var val = txtGiftcard.Text.Trim();
                if (val.Length > 0)
                {
                    Reimprimir();
                }
            }
        }
    }
}
