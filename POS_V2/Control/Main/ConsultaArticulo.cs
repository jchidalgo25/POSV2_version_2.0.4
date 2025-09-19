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

namespace POS.Control.Main
{
    public partial class ConsultaArticulo : Telerik.WinControls.UI.RadForm
    {
        pos_customer cliente_actual;
        MainWindow _mainWindow;
        Producto producto = new Producto();
        public ConsultaArticulo(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            core_tarjetacreditointerno tarjeta = null;
            core_tarjetacreditointerno tarjetaAdicional = null;
            cliente_actual = (_mainWindow.ClienteActual != null) ? _mainWindow.ClienteActual : Cliente.getCliente(Control.Common.GlobalParameters.IdConsumidorFinal, out tarjeta, out tarjetaAdicional);
        }

        private void ConsultaArticulo_Load(object sender, EventArgs e)
        {


        }

        private void btnKb_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();
        }

        private void txtProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            string codigo = txtProducto.Text.Trim();
            if (e.KeyChar == (char)Keys.Enter)
            {
                ejecutaConsultaArticulo(codigo);

            }


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string codigo = txtProducto.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigo))
            {
                //MessageBox.Show("No ha indicado el codigo del producto que desea consultar");
                Control.Common.General.GetMensajeToList(399);
                return;
            }

            ejecutaConsultaArticulo(codigo);

        }

        private  bool ejecutaConsultaArticulo(string CodigoArticulo)
        {

            producto = new Producto();
            decimal porcDescuentoDivisionEmpleado = 0;

            try
            {
                producto = Control.Common.General.GetProductoSP(CodigoArticulo);

                if (producto != null)
                {
                    if (producto.Itemtype != 2)
                    {
                        var descuento = producto.getDescuento(_mainWindow.FacturaActual, cliente_actual, ref porcDescuentoDivisionEmpleado);
                        //DescuentoConfiguraciones = descuento;
                        producto.PorcDescuentoDivisionEmpleado = porcDescuentoDivisionEmpleado;
                        producto.calcularDescuento(descuento);
                        // producto.calcularIVA(item.TAXVALUE);
                    }
                    else if (producto.Itemtype == 2)
                    {
                        var descuento = producto.getDescuento(_mainWindow.FacturaActual, cliente_actual, ref porcDescuentoDivisionEmpleado);
                        producto.PorcDescuentoDivisionEmpleado = porcDescuentoDivisionEmpleado;
                        producto.calcularDescuento(descuento);

                        producto.Iva = 0M;
                        producto.IvaProducto = 0M;
                    }
                    producto.Cantidad = 1;

                    producto.Pvp = producto.PrecioLocal - producto.Descuento;
                    producto.EsExcluidoPromoIVA = Control.Common.Promo.EsItemExcluidoPromoIVA(producto.Id);

                    using (POSEntities db = new POSEntities())
                    {
                        _mainWindow.existeEnListaDescuento(CodigoArticulo);
                        if (Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))
                        {
                            producto.actualizarDescuentoPromocionAX(_mainWindow.FacturaActual.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), null);
                        }
                        producto.update();

                        var cmbProducto = (from deta in db.core_parametro
                                           where deta.identificador == "COMBO_PRODUCTS"
                                           && deta.valor == "TRUE"
                                           && deta.parametro2 == Control.Common.GlobalParameters.Establecimiento
                                           select deta).ToList();

                        if (cmbProducto.Count > 0)
                        {
                            //Verifica si el producto tiene promo de combos
                            producto.VerifyComboProducts(CodigoArticulo, _mainWindow.FacturaActual); 
                        }
                        //Calcular al producto descuentos por DescuentoCuponPromocional
                        if (_mainWindow.FacturaActual.EsUsoCuponPromocional)
                        {
                            producto.DescuentoAX = producto.SubtotalSinDescuento * 0.1M;
                            producto.update();
                        }
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


            }
            catch (Exception)
            {
                producto = new Producto();
                return false;
            }
            

            return true;

        }

        private void btnElegir_Click(object sender, EventArgs e)
        {
            try
            {
                // producto = new Producto();




            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
