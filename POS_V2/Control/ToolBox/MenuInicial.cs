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
using POS.Control.Pagos;
using System.IO;
using MensajesLibrary;

namespace POS.Control.ToolBox
{
    public partial class MenuInicial : Telerik.WinControls.UI.RadForm
    {
        private MainWindow _mainWindow;
        Factura _factura;
        private Telerik.WinControls.UI.RadButton btnProveedorDomicilio;
        List<Control.ToolBox.ClsProveedorDomicilio> listProveedores = new List<Control.ToolBox.ClsProveedorDomicilio>();

        public MenuInicial(MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _factura = mainWindow.FacturaActual;
        }

        private void btnRecargas_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.ToolBox.ToolBoxMenu", "btnRecargas_Click", "Boton probar impresion Voucher presionado");
            using (POSEntities db = new POSEntities())
            {
                try
                {
                    //  PrintNewBasePagos();
                    //var objRecibo = db.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito).FirstOrDefault();
                    //var texto = objRecibo.cuerpo;

                    ////recibo = recibo.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE Y SIN \nPROTESTO EL TOTAL DE ESTE PAGARE MAS LOS INTERESES \nY CARGOS POR SERVICIO. EN CASO DE MORA PAGARE LA \nTASA MAXIMA AUTORIZADA POR EL EMISOR. DECLARO \nQUE EL PRODUCTO DE ESTA TRANSACCION NO SERA UTILI\nZADO EN ACTIVIDADES DE LAVADO DE ACTIVOS, FINANCIA\nMIENTO DEL TERRORISMO Y OTROS DELITOS ");

                    //DevuelveformatoCreditoPavos(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);

                    //texto = objRecibo.cuerpo;
                    //DevuelveformatoPagoPINPAD(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);

                    //texto = objRecibo.cuerpo;
                    //DevuelveformatoClsPagos(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);

                    //texto = objRecibo.cuerpo;
                    //DevuelveformatoBasePagos(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ToolBoxMenu_Load(object sender, EventArgs e)
        {
            CargarMenuProveedores();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "ToolBoxMenu_Load", "Pantalla ToolBox accesado. Usuario logon: " + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));
        }

        private void btnCerrarPOS_Click(object sender, EventArgs e)
        {
            var result = Control.Common.General.GetMensajeToList(554);
            if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "btnCerrarPOS_Click", "Procediendo a cerrar POS desde ToolBox bajo petición del usuario");
                Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }

            //if (MessageBox.Show("Procediendo a cerrar POS, pulse Sí para continuar", "Saliendo de POS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "btnCerrarPOS_Click", "Procediendo a cerrar POS desde ToolBox bajo petición del usuario");
            //    Control.Common.GlobalParameters.MustCloseApplication = true;
            //    Application.Exit();
            //}
        }


        private void CapturarCanalVenta(CANALVENTA _canal)
        {
            bool retornarMenu = false;
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.MenuInicial", "CapturarCanalVenta",
                "Ingresa CapturarCanalVenta");

            try
            {
                if (_canal == CANALVENTA.VENTANORMALPOS)                                                                                                                                                                                                
                {
                    _factura.EsPedidoOtraApp = false;
                    _factura.PedidoOtraApp.Pedido = string.Empty;
                    _factura.PedidoOtraApp.Tipo = (byte)_canal;
                }
                else
                {
                    PedidosOtrasApp pedido;
                    if (_factura.PedidoOtraApp != null)
                    {
                        pedido = new PedidosOtrasApp(_canal);
                        pedido.ShowDialog();

                        _factura.EsPedidoOtraApp = pedido.TienePedidoOtrApp;
                        _factura.PedidoOtraApp.Pedido = pedido.NumeroPedido;
                        _factura.PedidoOtraApp.Tipo = (byte)pedido.CanalVenta;
                        retornarMenu = pedido.Retornar;
                    }
                    else
                    {
                        pedido = new PedidosOtrasApp();
                        pedido.ShowDialog();
                        _factura.PedidoOtraApp = new TblPedido()
                        {
                            Pedido = pedido.TienePedidoOtrApp ? pedido.NumeroPedido : string.Empty,
                            Tipo = pedido.TienePedidoOtrApp ? (byte)pedido.Tipo : (byte)0
                        };
                    }

                }

                if (!retornarMenu)
                    this.Close();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.MenuInicial", "CapturarCanalVenta",
                $"Error: {ex.Message}");


            }
        }

        private void CapturarCanalVenta(CANALVENTA _canal,string delivery, string tipo, string formapago)
        {
            bool retornarMenu = false;
            try
            {
                if (_canal == CANALVENTA.VENTANORMALPOS)
                {
                    _factura.EsPedidoOtraApp = false;
                    _factura.PedidoOtraApp.Pedido = string.Empty;
                    _factura.PedidoOtraApp.Tipo = (byte)_canal;
                }
                else
                {
                    PedidosOtrasApp pedido;
                    if (_factura.PedidoOtraApp != null)
                    {
                        pedido = new PedidosOtrasApp(_canal, delivery, tipo, formapago);
                        pedido.ShowDialog();

                        _factura.EsPedidoOtraApp = pedido.TienePedidoOtrApp;
                        _factura.PedidoOtraApp.Pedido = pedido.NumeroPedido;
                        _factura.PedidoOtraApp.Tipo =  (pedido.CanalVenta != CANALVENTA.VENTAPEDIDOOTROS ? (byte)pedido.CanalVenta : (byte)int.Parse(tipo));
                        _factura.Delivery = delivery;
                        _factura.DeliveryFormaPago = formapago;
                        retornarMenu = pedido.Retornar;
                    }
                    else
                    {
                        pedido = new PedidosOtrasApp();
                        pedido.ShowDialog();
                        _factura.PedidoOtraApp = new TblPedido()
                        {
                            Pedido = pedido.TienePedidoOtrApp ? pedido.NumeroPedido : string.Empty,
                            Tipo = pedido.TienePedidoOtrApp ? (byte)pedido.Tipo : (byte)0
                        };
                    }

                }

                if (!retornarMenu)
                    this.Close();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.MenuInicial", "CapturarCanalVenta",
                $"Error: {ex.Message}");
            }
        }

        private void btnGlovo_Click(object sender, EventArgs e)
        {
            try
            {
                CapturarCanalVenta(CANALVENTA.VENTAPEDIDOGLOVO);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGlovo_Click", "No fue posible abrir formulario para Pedidos de Otras App, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);


            }
        }

        private void btnVtaNormal_Click(object sender, EventArgs e)
        {
            if (Control.Common.GlobalParameters.CompraGratis || Control.Common.GlobalParameters.MonederoActivo)
            {
                Control.Main.MensajeTarjetaVirtual frm = new Control.Main.MensajeTarjetaVirtual();

                MainWindow mainWindow = Application.OpenForms.OfType<MainWindow>().FirstOrDefault();

                if (mainWindow != null && mainWindow.ValidaExisteTmp())
                {
                    mainWindow.EjecutarCargaArchivosTmpAsync();
                }


                frm.ShowDialog();
            }

            CapturarCanalVenta(CANALVENTA.VENTANORMALPOS);
        }

        private void btnVtaApp_Click(object sender, EventArgs e)
        {
            try
            {
                CapturarCanalVenta(CANALVENTA.VENTAAPPPOS);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGlovo_Click", "No fue posible abrir formulario para Pedidos de Otras App, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);


            }
        }

        private void btnVtaRappid_Click(object sender, EventArgs e)
        {
            try
            {
                CapturarCanalVenta(CANALVENTA.VENTAPEDIDORAPPID);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGlovo_Click", "No fue posible abrir formulario para Pedidos de Otras App, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);


            }
        }

        public void CargarMenuProveedores()
        {
            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.MenuInicial", "CargarMenuProveedores",$"Inicia CargarMenuProveedores");

                using (POSEntities db = new POSEntities())
                {
                    var paramProveedores = db.core_parametro.Where(x => x.identificador.Equals("PROVEEDOR_SERVICIO_DOMICILIO_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).ToList();

                    if (paramProveedores == null)
                    {
                        //throw new Exception("El local no tiene parametro PROVEEDOR_SERVICIO_DOMICILIO_XXXX-XXXX configurado en la tabla core_parametro");
                        return;
                    }
                    else
                    {
                        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuInicial));
                        int leftx = 552, lefty = 3, leftSizew = 270, leftSizeh = 335, espace = 25;

                        int ejex = leftx;
                        int ejey = lefty;

                        foreach (var provee in paramProveedores)
                        {
                            var _proveedor = new Control.ToolBox.ClsProveedorDomicilio();
                            var array_config = provee.documento.Split('|');
                            _proveedor.Nombre = provee.parametro2;

                            _proveedor.NombreImagenBoton = array_config[0];
                            _proveedor.Tipo = array_config[1];
                            _proveedor.FormaPago = array_config[2];
                            _proveedor.Estado = (provee.valor == "TRUE");

                            if (listProveedores.Any(x => x.Nombre == _proveedor.Nombre))
                                listProveedores.RemoveAll(x => x.Nombre == _proveedor.Nombre);

                            listProveedores.Add(_proveedor);
                            if (_proveedor.Estado)
                            {
                                btnProveedorDomicilio = new Telerik.WinControls.UI.RadButton();
                                ((System.ComponentModel.ISupportInitialize)(btnProveedorDomicilio)).BeginInit();

                                //FileInfo filePath = new FileInfo("../../Resources/" + _proveedor.NombreImagenBoton);
                                //btnProveedorDomicilio.Image = Image.FromFile(filePath.FullName); //((System.Drawing.Image)(resources.GetObject(_proveedor.NombreBoton)));

                                System.Resources.ResourceManager rm = global::POS.Properties.Resources.ResourceManager;
                                Bitmap myImage = (Bitmap)rm.GetObject(_proveedor.NombreImagenBoton);

                                btnProveedorDomicilio.Image = myImage;
                                btnProveedorDomicilio.Tag = _proveedor.Nombre + "|" + _proveedor.Tipo + "|" + _proveedor.FormaPago;
                                btnProveedorDomicilio.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                                btnProveedorDomicilio.Location = new System.Drawing.Point(ejex, ejey);
                                btnProveedorDomicilio.Name = _proveedor.NombreImagenBoton;
                                btnProveedorDomicilio.Size = new System.Drawing.Size(leftSizew, leftSizeh);
                                btnProveedorDomicilio.TabIndex = 9;
                                btnProveedorDomicilio.Text = _proveedor.Nombre;
                                btnProveedorDomicilio.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
                                btnProveedorDomicilio.TextWrap = true;
                                btnProveedorDomicilio.ThemeName = "TelerikMetroTouch";
                                btnProveedorDomicilio.Visible = true;
                                btnProveedorDomicilio.Click += new System.EventHandler(btnProveedorDomicilio_Click);

                                ejex = ejex + btnProveedorDomicilio.Width + espace;
                                if ((ejex + btnProveedorDomicilio.Width) > flowLayoutPanel2.Width)
                                {
                                    ejex = leftx;
                                    ejey = ejey + btnProveedorDomicilio.Height + espace;
                                }

                                this.flowLayoutPanel2.Controls.Add(btnProveedorDomicilio);
                                ((System.ComponentModel.ISupportInitialize)(btnProveedorDomicilio)).EndInit();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.MenuInicial", "CargarMenuProveedores", $"error: {ex.Message}");
            }
      
        }

        private void btnProveedorDomicilio_Click(object sender, EventArgs e)
        {
            try
            {
                Telerik.WinControls.UI.RadButton btn = (Telerik.WinControls.UI.RadButton)sender;
                var respuesta = btn.Tag.ToString().Split('|');
                string nombre = respuesta[0];
                string tipo = respuesta[1];
                string formapago = respuesta[2];

                CapturarCanalVenta(CANALVENTA.VENTAPEDIDOOTROS, nombre, tipo, formapago);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGlovo_Click", "No fue posible abrir formulario para Pedidos de Otras App, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
