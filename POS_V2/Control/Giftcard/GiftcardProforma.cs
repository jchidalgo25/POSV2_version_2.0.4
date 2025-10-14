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
    public partial class GiftcardProforma : Telerik.WinControls.UI.RadForm
    {

        #region Constructores

        public GiftcardProforma(ref Models.Giftcard.ClsGiftcardSale objGiftcardSale)
        {
            _objGiftcardSale = objGiftcardSale;
            InitializeComponent();
        }

        #endregion

        #region Atributos Privados
        
        private Models.Giftcard.ClsGiftcardSale _objGiftcardSale;

        #endregion

        #region Metodos Controles

        private void GiftcardPreOrder_Load(object sender, EventArgs e)
        {
            switch (_objGiftcardSale.GiftcardSaleType)
            {
                case Common.Enum.GiftcardSaleType.Sale:
                   // lblTipoVenta.Text = "Venta";
                    lblTipoTransaccion.Text = "Venta";
                    break;
                case Common.Enum.GiftcardSaleType.Recharge:
                    //lblTipoVenta.Text = "Recarga";
                    lblTipoTransaccion.Text = "Recarga";
                    break;
                default:
                    break;
            }

            lblClienteIdentificacion.Text = _objGiftcardSale.Customer.ACCOUNTNUM;
            lblClienteNombre.Text = _objGiftcardSale.Customer.NAME;
            lblClieenteDireccion.Text = _objGiftcardSale.Customer.STREET;

            //lblValorCompra.Text = string.Format("$ {0}", _objGiftcardSale.PurchaseValue.ToString("N2"));
            //lblSaldoNuevo.Text = string.Format("$ {0}", _objGiftcardSale.PurchaseValue.ToString("N2"));
            //lblSaldoActual.Text = string.Format("$ {0}", _objGiftcardSale.CurrentValue.ToString("N2"));
            //lblSaldoFinal.Text = string.Format("$ {0}", _objGiftcardSale.FutureValue.ToString("N2"));

            
            if (_objGiftcardSale.LstGiftcard != null)
            {
                lblValorCompra.Text = string.Format("$ {0}", ((decimal)_objGiftcardSale.LstGiftcard.Sum(x => x.monto)).ToString("N2"));
                lblSaldoNuevo.Text = string.Format("$ {0}", ((decimal)_objGiftcardSale.LstGiftcard.Sum(x => x.saldo+ x.monto)).ToString("N2"));
                lblSaldoActual.Text = string.Format("$ {0}", ((decimal)_objGiftcardSale.LstGiftcard.Sum(x => x.saldo)).ToString("N2"));
                lblSaldoFinal.Text = string.Format("$ {0}", ((decimal)_objGiftcardSale.LstGiftcard.Sum(x => x.saldo + x.monto)).ToString("N2"));
            }
          

        }


        #endregion

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            _objGiftcardSale.Confirmed = true;
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Giftcard.GiftcardProforma", "Aceptar GiftCard", "Cliente: "+lblClienteIdentificacion.Text +" Valor Compra: "+lblValorCompra.Text+" Saldo Actual:"+lblSaldoActual.Text +" Saldo Final:"+lblSaldoFinal.Text);
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
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
    }
}
