using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Control.PINPAD;
using Trx.Messaging;
using Trx.Utilities;
using System.IO.Ports;
using POS.Models;
using System.IO;
using System.Net.Mail;
using System.Net;
using WinSCP;
using POS.Control.Pagos;

namespace POS.Control.ReImprimirFactura
{
   public partial class ReImprimirFactura : Form
    {
        const int PUERTOCOM = 9;
        Factura _factura;
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        private DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();

        public ReImprimirFactura(ref Factura f)
        {
            InitializeComponent();
            _factura = f;
        }

        private void ReImprimirFactura_Load(object sender, EventArgs e)
        {
            using (var db = new POSEntities())
            {
                cmbTipoTransaccion.Items.Add("Reimprimir Factura");
                cmbTipoTransaccion.DataMember = "idTipo";
                cmbTipoTransaccion.DisplayMember = "NombreTarjetaTransaccion";
            }

            cmbTipoTransaccion.SelectedIndex = 0;
            lblFacReimpresionAyuda.Text = _factura.Documento + "-" + _factura.Establecimiento.PadLeft(3, '0') + "-" + _factura.PtoEmision.PadLeft(3, '0') + "-";
            lblFacReimpresion.Text = lblFacReimpresionAyuda.Text + String.Empty.PadLeft(9, '0');
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            string IPTransaction = "";
            var pos = new POSEntities();
            //Enviar a imprimir

            if (pos.core_parametro.Where(x => x.identificador == "REIMPRIME_FACTURA" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
            {
                CargaPedidoAppDelPortal();
               
                _factura.Pagos.Clear();
                _factura.Productos.Clear();
            }
        }

        private void CargaPedidoAppDelPortal()
        {
            long NumeroPedidoDelPortal = 0;
            long idFactura = 0;
            string sEstablecimiento = "";
            string sPunto = "";
            string usuarioCaja="";
            string ClienteIdentificacion = "";
            string Cliente_codigo = "";
            string Cliente_direccion = "";
            string Cliente_grupo = "";
            string Cliente_nombre = "";
            string Cliente_telefono = "";
            try
            {
                
                NumeroPedidoDelPortal = Convert.ToInt32(this.txtFacReimpresion.Text) ;
                sEstablecimiento = _factura.Establecimiento.PadLeft(3, '0');
                sPunto = _factura.PtoEmision.PadLeft(3, '0');

                POSEntities db = new POSEntities();
                if (NumeroPedidoDelPortal != 0)
                {

                    var querypagos = (from c in db.core_factura
                                      where   c.establecimiento == sEstablecimiento && c.punto_emision == sPunto && c.numero == NumeroPedidoDelPortal
                                      select c
                                      );

                    foreach (var cabFact in querypagos)
                    {

                        var pos_cust = (from cust in db.pos_customer
                                        where cust.ACCOUNTNUM == cabFact.cliente_ax
                                        select cust);

                        foreach (var cliente_actual in pos_cust)
                        {
                            ClienteIdentificacion = _factura.ClienteIdentificacion;
                            Cliente_codigo = _factura.Cliente_codigo;
                            Cliente_direccion = _factura.Cliente_direccion;
                            Cliente_grupo = _factura.Cliente_grupo;
                            Cliente_nombre = _factura.Cliente_nombre;
                            Cliente_telefono = _factura.Cliente_telefono;

                            _factura.ClienteIdentificacion = cliente_actual.VATNUM != "" ? cliente_actual.VATNUM : cliente_actual.ACCOUNTNUM;
                            _factura.Cliente_codigo = cliente_actual.ACCOUNTNUM;
                            _factura.Cliente_direccion = cliente_actual.ADDRESS;
                            _factura.Cliente_grupo = cliente_actual.CUSTGROUP;
                            _factura.Cliente_nombre = cliente_actual.NAME;
                            _factura.Cliente_telefono = cliente_actual.PHONE;

                        }

                        _factura.Subtotal = cabFact.subtotal;
                        _factura.Descuento = cabFact.descuento;
                        _factura.Descuento2 = cabFact.descuento2;                      
                        _factura.Autorizacion = cabFact.autorizacion;
                        _factura.Base_imponible = cabFact.base0 + cabFact.base12;
                        _factura.Iva = cabFact.iva;
                        _factura.Total = cabFact.total;
                        _factura.Fecha = cabFact.fecha_creacion;
                        idFactura = cabFact.id;
                        var ncajero = (from usr in db.auth_user
                                       where usr.username == cabFact.usuario
                                       select usr).FirstOrDefault();
                        usuarioCaja = _factura.User.nombres;
                        _factura.User.nombres = ncajero.first_name + " " + ncajero.last_name;

                        //consulta clave acceso
                        _factura.ClaveAccesoSRI = ( from clv in db.core_ClaveAccesoFE
                                                    where clv.FacturaId== idFactura
                                                    select clv.ClaveAcceso ).FirstOrDefault();
                                             
                    }

                    var query = (from c in db.core_factura
                                 join p in db.core_facturadetalle on c.id equals p.factura_id
                                 where p.factura_id == idFactura
                                 select p
                                 );

                    _factura.Productos.Clear();

                    decimal descuento2 = 0;
                    if (_factura.Descuento2 > 0)
                    {
                        descuento2 = _factura.Descuento2 / _factura.Subtotal;
                    }

                    foreach (var itemApp in query)
                    {

                        Producto itm = new Producto();
                        itm.Id = itemApp.item_id;
                        itm.Nombre = itemApp.item_nombre;
                        itm.CantidadINEC = itemApp.cantidad;
                        itm.Cantidad = itemApp.cantidad;
                        itm.Unidades = Convert.ToInt32(itemApp.unidades);
                        itm.Unidad = itemApp.unidad;
                        itm.Pvp = itemApp.cantidad == 0 ? 0 : itemApp.subtotal / itemApp.cantidad;
                        itm.PrecioAx = itemApp.precio;
                        itm.Costo = itemApp.costo;
                        itm.Subtotal = itemApp.subtotal; 
                        itm.Descuento = itemApp.descuento;
                        itm.Iva = itemApp.iva;
                        itm.IvaProducto = itemApp.iva;
                        itm.EsExcluidoPromoIVA = false;
                        itm.Total = itemApp.total;                         

                        _factura.Productos.Add(itm);
                    }
                }

                _factura.prepararImpresion(NumeroPedidoDelPortal);
                Control.Common.Printer.Imprimir(_factura.Recibo, 3, 11);


                //Limpiar datos de recibo despues de imprimir 
                
                _factura.User.nombres = usuarioCaja;
                _factura.ClienteIdentificacion = ClienteIdentificacion;
                _factura.Cliente_codigo = Cliente_codigo;
                _factura.Cliente_direccion = Cliente_direccion;
                _factura.Cliente_grupo = Cliente_grupo;
                _factura.Cliente_nombre = Cliente_nombre;
                _factura.Cliente_telefono = Cliente_telefono;

                if (_factura.Documento == "F")
                {


                    var recibo = db.core_recibo.Where(x => x.identificador == (Control.Common.GlobalParameters.ComprobanteFactura + "_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();
                    if (recibo == null)
                        recibo = db.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteFactura).FirstOrDefault();

                    _factura.Recibo = recibo.cuerpo;

                }
                else if (_factura.Documento == "R")
                {
                    var recibo = db.core_recibo.Single(x => x.identificador == "RECIBO_GIFTCARD");
                    _factura.Recibo = recibo.cuerpo;
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ReImprimirFactura", "CargaPedidoAppDelPortal", "error: " + ex.Message);
            }
        }

        private void cmbTipoTransaccion_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            var db = new POSEntities();
            try
            {
                //((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo
                switch (((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo)
                {
                     case "01":
                        boxReimpresion.Top = 157;
                        boxReimpresion.Left = 21;
                        boxReimpresion.Visible = true;
                        btnPagar.Text = "Reimprimir";
                        break;
                    default:
                        boxReimpresion.Top = 157;
                        boxReimpresion.Left = 21;
                        boxReimpresion.Visible = true;
                        btnPagar.Text = "Reimprimir";
                        break;
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ReImprimirFactura", "cmbTipoTransaccion_SelectedIndexChanged", "error: " + ex.Message);
            }
        }

        private void txtFacReimpresion_TextChanged(object sender, EventArgs e)
        {
            lblFacReimpresion.Text = lblFacReimpresionAyuda.Text + txtFacReimpresion.Text.PadLeft(9, '0');
        }
        private void btnKbd_Click(object sender, EventArgs e)
        {

            //Common.General.tecladoFlotante();
            Common.General.TecladoPantalla();
           
        }
    }

}


