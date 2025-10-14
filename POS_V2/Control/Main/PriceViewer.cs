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


namespace POS.Control.Main
{
    public partial class PriceViewer : Telerik.WinControls.UI.RadForm
    {
        public pos_customer cliente_actual;
        public Factura FacturaActual;
        public MainWindow _mainWindow;
        private bool activarPermitido = true;

        //public PriceViewer(MainWindow mainWindow)
        //{
        //    InitializeComponent();

        //    _mainWindow = mainWindow;
        //    core_tarjetacreditointerno tarjeta = null;
        //    core_tarjetacreditointerno tarjetaAdicional = null;
        //    cliente_actual = (_mainWindow.ClienteActual != null) ? _mainWindow.ClienteActual : Cliente.getCliente(Control.Common.GlobalParameters.IdConsumidorFinal, out tarjeta, out tarjetaAdicional);
        //}

        public PriceViewer()
        {
            InitializeComponent();

            //_mainWindow = mainWindow;
            core_tarjetacreditointerno tarjeta = null;
            core_tarjetacreditointerno tarjetaAdicional = null;
            cliente_actual = (cliente_actual != null) ? cliente_actual : Cliente.getCliente(Control.Common.GlobalParameters.IdConsumidorFinal, out tarjeta, out tarjetaAdicional);
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "PriceViewer", "Inicia forma PriceViewer");

        }

        protected override void OnActivated(EventArgs e)
        {

            base.OnActivated(e);
            //if (!activarPermitido)
            //    return;

            //activarPermitido = true;


            //this.Focus();
            //this.BringToFront();
        }

        //protected override void OnDeactivate(EventArgs e)
        //{
        //    base.OnDeactivate(e);

        //    if (!activarPermitido)
        //    {
        //        activarPermitido = true;

        //        this.BeginInvoke(new MethodInvoker(() =>
        //        {
        //            this.TopMost = true;
        //            this.Focus();
        //            this.Activate();
        //            this.TopMost = false;

        //            activarPermitido = false;
        //        }));
        //    }


        //    activarPermitido = false;
        //}


        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchPrice();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "PriceViewer", "Ejecuta btnSearch_Click");
        }

        private void txtProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "PriceViewer", "Ejecuta txtProducto_KeyPress");
                SearchPrice();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;

                //case Keys.Enter:

                //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "PriceViewer", "Ejecuta ProcessCmdKey");
                //    SearchPrice();
                //    break;

            }


            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SearchPrice()
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "PriceViewer", "Ejecuta SearchPrice");
            try
            {
                
                string codigo = txtProducto.Text.Trim();
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    //MessageBox.Show("No ha indicado el codigo del producto que desea consultar");
                    Control.Common.General.GetMensajeToList(399);
                    return;
                }

                if (cliente_actual == null) {
                    return;
                }

                var producto = new Producto();

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "SearchPrice", $"codigo {codigo}");
                if (producto.getProducto(codigo, FacturaActual, cliente_actual))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "SearchPrice", $"Articulo encontrado");
                    producto.Cantidad = 1;

                    //Descuentos
                    using (POSEntities db = new POSEntities())
                    {
                        _mainWindow.existeEnListaDescuento(codigo);

     
                        if (Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))
                        {
                            producto.actualizarDescuentoPromocionAX(FacturaActual.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), FacturaActual);
                        }

                        producto.update();
                        if (db.core_parametro.Where(x => x.identificador == "COMBO_PRODUCTS" 
                                                        && x.valor == "TRUE" 
                                                        && x.parametro2 == Control.Common.GlobalParameters.Establecimiento).FirstOrDefault() != null)
                        {
                            producto.VerifyComboProducts(codigo, FacturaActual); //Verifica si el producto tiene promo de combos
                        }
                        //Calcular al producto descuentos por DescuentoCuponPromocional
                        if (FacturaActual.EsUsoCuponPromocional)
                        {
                            producto.DescuentoAX = producto.SubtotalSinDescuento * 0.1M;
                            producto.update();
                        }
                        //Descuento IVA
                        if (!producto.EsExcluidoPromoIVA)
                        {
                            if (Control.Common.GlobalParameters.PROMO_IVA)
                            {
                                var porcPromo = Control.Common.GlobalParameters.DESC_PROMO_IVA;
                                if (Control.Common.Promo.EsDiaPromoIVA() && producto.IvaProducto != 0)
                                {
                                    var descuentoPromoIVA = (producto.Subtotal * producto.IvaProducto);
                                    producto.DescuentoIVA = descuentoPromoIVA;
                                }
                            }
                        }
                    }                    

                    lblNombreProducto.Text = producto.Nombre;
                    lblSubtotal.Text = "$ " + producto.Pvp.ToString("N2");
                    lblDescuentos.Text = "$ " + (producto.Descuento + producto.DescuentoIVA).ToString("N2");
                    lblIva.Text = "$ " + producto.Iva.ToString("N2");
                    lblTotal.Text = "$ " + producto.TotalPromoIVA.ToString("N2");
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "SearchPrice", $"Articulo No encontrado");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "SearchPrice", $"Valida que no sea barra de descuento");
                    if (!codigo.StartsWith("30"))//Valida que no sea barra de descuento
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "SearchPrice", $" barra de descuento");

                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[codigo]", valor = codigo });
                        Control.Common.General.GetMensajeToList(400, parametros);

                        //MessageBox.Show(this, "Código no existente o sin precio " + codigo.ToString() + "!", "Acción no valida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PriceViewer", "SearchPrice", $" barra de descuento");
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "PriceViewer", "SearchPrice", "No fue posible consultar el precio del producto, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //MessageBox.Show(this, "No fue posible consultar el precio del producto, inténtelo nuevamente dentro de unos momentos", "POS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Control.Common.General.GetMensajeToList(401);
            }

            txtProducto.Clear();
            txtProducto.Focus();
        }

        private void PriceViewer_Load(object sender, EventArgs e)
        {

        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
