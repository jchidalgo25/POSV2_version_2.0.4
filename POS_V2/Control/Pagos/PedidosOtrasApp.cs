using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace POS.Control.Pagos
{
    public partial class PedidosOtrasApp : Form
    {
        System.Windows.Forms.Control focused;
        Boolean _esPedidoValido = false;
        Boolean _retornarMenuInicial = false;

        public Factura factura { get; set; }
        public string NumeroPedido { get; set; }
        CANALVENTA      _canalVenta;
        string CanalVentaNombre;
        int CanalVentaTipo;
        string CanalVentaFormaPago;

        public int Tipo { get; set; }
        public CANALVENTA CanalVenta {
            get
            {
                return _canalVenta;
            }
            set
            {
                _canalVenta = value;
            }
        }
        public bool Retornar
        {
            get
            {
                return _retornarMenuInicial;
            }
            set
            {
                _retornarMenuInicial = value;
            }
        }
        public bool TienePedidoOtrApp { 
            get
            {
                return _esPedidoValido;
            }
            set
            {
                _esPedidoValido = value;
            }
        }
        public PedidosOtrasApp()
        {
            Init();            
        }

        public PedidosOtrasApp(CANALVENTA canalVenta)
        {
            Init();
            _canalVenta = canalVenta;
            switch (canalVenta)
            {
                case CANALVENTA.VENTANORMALPOS:
                    CanalVentaNombre = "Venta POS";

                    break;
                case CANALVENTA.VENTAAPPPOS:
                    CanalVentaNombre = "App DelPortal";
                    break;
                case CANALVENTA.VENTAPEDIDOGLOVO:
                    CanalVentaNombre = "Glovo";
                    break;
                case CANALVENTA.VENTAPEDIDORAPPID:
                    CanalVentaNombre = "Rappi";
                    break;
                default:
                    //Console.WriteLine("Default case");
                    break;
            }

            if (CanalVentaNombre == "Glovo")
            {
                lblNumeroPedido.Text = lblNumeroPedido.Text + " " + "Pedidos Ya";

                if (Common.GlobalParameters.ActivaIntegracionPedidos)
                { 
                    using (WSIntegracion.Service1Client Integ = new WSIntegracion.Service1Client())
                    {
                        var xml = Integ.IntegrationDelivery(XmlIntegracion(CanalVentaNombre, 1));
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "PedidosOtrasApp " + CanalVentaNombre, "Inicializacion:" + xml.ToString());
                        //MessageBox.Show("Debe ingresar un Número de Pedido " + CanalVentaNombre + " ", "Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }

                }
                
            }
            else
                lblNumeroPedido.Text = lblNumeroPedido.Text + " " + CanalVentaNombre;

        }

        private string XmlIntegracion(string canalVenta, int opcion)
        {   
            string xml = string.Empty;
            string Integ = string.Empty;
            string Metodo = string.Empty;
            string versionPOS = "1.1.1.972";
            string OrderId = string.Empty;
            string sInvoice = string.Empty;
            string Id = string.Empty;
            string Description = string.Empty;
            Version ver = null;
            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                    ver = ad.CurrentVersion;
                    versionPOS += ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision;
                }

                switch (canalVenta)
            {
                case "Glovo":
                    Integ = "YA";
                    break;
                case "Rappi":
                    Integ = "RA";
                    break;
                default:
                    //Console.WriteLine("Default case");
                    break;

            }
                if (opcion == 1) //Inicializacion
                {

                    Metodo = "IN"+ Integ;
                }
                else if (opcion == 2)//Recepcion
                {
                    Metodo = "RE" + Integ;
                    OrderId = txtNumPedidoOtraApp.Text;
                }

                XmlDocument xmlDoc = new XmlDocument();
                XmlNode rootNode = xmlDoc.CreateElement("Root");
                XmlAttribute attributer1 = xmlDoc.CreateAttribute("Integ");
                attributer1.Value = Integ;
                rootNode.Attributes.Append(attributer1);

                XmlAttribute attributer2 = xmlDoc.CreateAttribute("ProgId");
                attributer2.Value = Metodo;
                rootNode.Attributes.Append(attributer2);

                xmlDoc.AppendChild(rootNode);

                XmlNode userNode = xmlDoc.CreateElement("req");
                XmlAttribute attribute = xmlDoc.CreateAttribute("DelportalId");
                attribute.Value = Control.Common.GlobalParameters.Establecimiento;
                userNode.Attributes.Append(attribute);

                XmlAttribute attribute1 = xmlDoc.CreateAttribute("orderId");
                attribute1.Value = OrderId;
                userNode.Attributes.Append(attribute1);

                XmlAttribute attribute2 = xmlDoc.CreateAttribute("sInvoice");
                attribute2.Value = sInvoice;
                userNode.Attributes.Append(attribute2);

                XmlAttribute attribute3 = xmlDoc.CreateAttribute("Id");
                attribute3.Value = Id;
                userNode.Attributes.Append(attribute3);

                XmlAttribute attribute4 = xmlDoc.CreateAttribute("Name");
                attribute4.Value = Name;
                userNode.Attributes.Append(attribute4);

                XmlAttribute attribute5 = xmlDoc.CreateAttribute("Description");
                attribute5.Value = Description;
                userNode.Attributes.Append(attribute5);

                XmlAttribute attribute6 = xmlDoc.CreateAttribute("VersionOs");
                attribute6.Value = System.Environment.OSVersion.ToString();
                userNode.Attributes.Append(attribute6);

                XmlAttribute attribute7 = xmlDoc.CreateAttribute("VersionPos");
                attribute7.Value = versionPOS;
                userNode.Attributes.Append(attribute7);                

                rootNode.AppendChild(userNode);

                xml = xmlDoc.InnerXml.ToString();
            }
            catch (Exception ex)
            {

            }
            return xml;
        }
        
        public PedidosOtrasApp(CANALVENTA canalVenta,string delivery, string tipo, string formapago)
        {
            Init();

            _canalVenta = canalVenta;
            CanalVentaTipo = (int)canalVenta;
            switch (canalVenta)
            {
                case CANALVENTA.VENTANORMALPOS:
                    CanalVentaNombre = "Venta POS";
                    break;
                case CANALVENTA.VENTAAPPPOS:
                    CanalVentaNombre = "App DelPortal";
                    break;
                case CANALVENTA.VENTAPEDIDOGLOVO:
                    CanalVentaNombre = "Glovo";
                    break;
                case CANALVENTA.VENTAPEDIDORAPPID:
                    CanalVentaNombre = "Rappi";
                    break;
                case CANALVENTA.VENTAPEDIDOOTROS:
                    CanalVentaNombre = delivery;
                    CanalVentaTipo = int.Parse(tipo);
                    CanalVentaFormaPago = formapago;
                    break;
                default:
                    //Console.WriteLine("Default case");
                    break;
            }

            if (CanalVentaNombre == "Glovo")
                lblNumeroPedido.Text = lblNumeroPedido.Text + " " + "Pedidos Ya";
            else
                lblNumeroPedido.Text = lblNumeroPedido.Text + " " + CanalVentaNombre;

        }

        public PedidosOtrasApp(string _numeroPedido)
        {
            Init();
            NumeroPedido = _numeroPedido;
            if(string.IsNullOrEmpty(_numeroPedido))
            {
                Tipo = 0;
                TienePedidoOtrApp = false;
                
            }
            txtNumPedidoOtraApp.Text = NumeroPedido;
        }
        private void Init()
        {
            InitializeComponent();

            focused = (System.Windows.Forms.Control)txtNumPedidoOtraApp;

            if (CanalVentaNombre == "Glovo")
                this.Text = "Pedido Pedidos Ya";
            else
                this.Text = "Pedido " + CanalVentaNombre;

        }


        public PedidosOtrasApp( Factura _mifactura)
        {
            factura = _mifactura;
            Init();
        }


        private bool Aceptar()
        {
            try
            {

                if (string.IsNullOrEmpty(txtNumPedidoOtraApp.Text))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Pedido " + CanalVentaNombre, "Debe ingresar un Número de Pedido " + CanalVentaNombre + " ");
                    MessageBox.Show("Debe ingresar un Número de Pedido "+ CanalVentaNombre+" ", "Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return false;
                }
                else
                {
                    if (CanalVentaNombre == "Pedidos Ya")
                        CanalVentaNombre = "Glovo";

                    if ( CanalVentaNombre == "Glovo" && txtNumPedidoOtraApp.Text.Length != 9)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Pedidos Ya", "Código del pedido debe de tener 9 caracteres ");
                        MessageBox.Show("Debe ingresar un Número de Pedido de 9 caracteres ", "Pedidos Ya", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return false;
                    }
                    else
                    {
                        
                        /*if(_canalVenta == CANALVENTA.VENTAPEDIDOOTROS && !validaExisteOrdenProveedor())
                        {
                            MessageBox.Show("El Número de Pedido ingresado no existe en "+ CanalVentaNombre +", favor ingrese un número de pedido válido.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }*/

                        if (validaSiExistePedido())
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Número de Pedido ingresado ya lo tenemos registrado, favor ingrese un número de pedido válido.");
                            MessageBox.Show("El Número de Pedido ingresado ya lo tenemos registrado, favor ingrese un número de pedido válido.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        // if (MessageBox.Show(this, "¿Está seguro de Continuar? Si su respuesta es Si, se asocia el pedido "+ CanalVentaNombre + " a la factura.", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        //{
                        NumeroPedido = txtNumPedidoOtraApp.Text;
                        //Tipo = (int)_canalVenta; // Glovo
                        TienePedidoOtrApp = true;
                        if (_canalVenta == CANALVENTA.VENTAAPPPOS)
                        {
                            if (ValidoPedidoAppDelPortal())
                            {
                                TienePedidoOtrApp = true;
                            }
                            else
                            {
                                TienePedidoOtrApp = false;
                                return false;
                            }
                        }
                        //}

                    }

                }
                return true;

            }
            catch (Exception)
            {
                
                throw;
            }
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {

            if(Aceptar())            
                this.Close();

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                txtNumPedidoOtraApp.Text = string.Empty;
                
                 TienePedidoOtrApp = false;
                 NumeroPedido = string.Empty;
            }
            catch (Exception)
            {                
             //   throw;
            }
        }

        private bool validaSiExistePedido()
        {
            bool retorno = false;
            try
            {
                if (CanalVentaNombre == "Glovo")//pedidos ya
                {
                    if (Common.GlobalParameters.ActivaIntegracionPedidos)
                    {
                        using (WSIntegracion.Service1Client Integ = new WSIntegracion.Service1Client())
                        {
                            var xml = Integ.IntegrationDelivery(XmlIntegracion(CanalVentaNombre, 2));

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "validaSiExistePedidos " + CanalVentaNombre, "Reception:" + xml.ToString());

                        }
                    }
                }
                POSEntities db = new POSEntities();
                if (!string.IsNullOrEmpty(txtNumPedidoOtraApp.Text))
                {
                    //if (db.TblPedido.Any(t => t.Tipo == (byte)_canalVenta && t.Pedido == txtNumPedidoOtraApp.Text))
                    if (db.TblPedido.Any(t => t.Tipo == (byte)CanalVentaTipo && t.Pedido == txtNumPedidoOtraApp.Text))
                    {
                        retorno = true;
                    }
                    else
                    {
                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                        {
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                            var db1 = new POSEntities();
                            if (db1.TblPedido.Any(t => t.Tipo == (byte)CanalVentaTipo && t.Pedido == txtNumPedidoOtraApp.Text))
                            {
                                retorno = true;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "validaSiExistePedido", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                            }
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }

            return retorno;
        }
        private void prCambioCadenaConexion(string srvSelect, string srvPedidos)
        {
            try
            { 
                string yourConnection = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"].ConnectionString.Replace(srvSelect, srvPedidos);
                //dcon = new POSEntities(yourConnection);
                var DBCS = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"];
                var writable = typeof(System.Configuration.ConfigurationElement).GetField("_bReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                writable.SetValue(DBCS, false);
                DBCS.ConnectionString = yourConnection;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
            }
            catch (Exception ex)
            {
            }

    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
        }

        private bool ValidoPedidoAppDelPortal()
        {
            bool retorno = false;
            int idFactPedidoApp = 0;
            int NumeroPedidoDelPortal = 0;
            try
            {
                //string conn_posEntities = Properties.Resources.ConectaDB;
                //SeleccionaLocal.SaveConnectionString("POSEntities", conn_posEntities.Replace("[ipserver]", Control.Common.GlobalParameters.ipServerPedAPP));

                NumeroPedidoDelPortal = Convert.ToInt32(txtNumPedidoOtraApp.Text);
                POSEntities db = new POSEntities();
                if (!string.IsNullOrEmpty(txtNumPedidoOtraApp.Text))
                {
                    //Validación que existe el numero de pedido y que no haya sido procesado y que sea del local que hizo el pedido.
                    var queryPedido = db.core_facturaAPP.Where(t => t.orderApp == NumeroPedidoDelPortal && t.orderID == null).FirstOrDefault();
                    

                    if (queryPedido != null)
                    {
                        retorno = true;
                        if (queryPedido.establecimiento != Control.Common.GlobalParameters.Establecimiento)
                        {
                            retorno = false;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no le partenece a este local.");
                            MessageBox.Show("El Pedido de App del Portal no le partenece a este local.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                        {
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                            var db1 = new POSEntities();
                            queryPedido = db1.core_facturaAPP.Where(t => t.orderApp == NumeroPedidoDelPortal && t.orderID == null).FirstOrDefault();
                            if (queryPedido != null)
                            {
                                retorno = true;
                                if (queryPedido.establecimiento != Control.Common.GlobalParameters.Establecimiento)
                                {
                                    retorno = false;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no le partenece a este local.");
                                    MessageBox.Show("El Pedido de App del Portal no le partenece a este local.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "validoPedidoAppDelPortal", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                            }
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                        }
                        else
                        {
                            retorno = false;
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no es válido. Ingrese un Pedido válido.");
                            MessageBox.Show("El Pedido de App del Portal no es válido. Ingrese un Pedido válido.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }

                    }

                    if (retorno)
                    {

                        idFactPedidoApp = queryPedido.id;  
                        
                        var query = (from p in db.core_facturapagoAPP 
                                where p.factura_id == idFactPedidoApp &&
                                    ( p.tipo_id == "EFECTIVO") 
                                select p).FirstOrDefault();

                        if (query != null)
                        {
                            retorno = true;
                        }
                        else
                        {
                            if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                            {
                                prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                                var db1 = new POSEntities();

                                var query1 = (from p in db1.core_facturapagoAPP
                                              where p.factura_id == idFactPedidoApp &&
                                                   (p.tipo_id == "EFECTIVO")
                                              //(p.tipo_id == "EFECTIVO" || p.tipo_id == "DINE ELECT")  // JCañarte 1Feb2021
                                              select p).FirstOrDefault();
                                if (query1 != null)
                                {
                                    retorno = true;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "validoPedidoAppDelPortal", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                                }
                                else
                                {
                                    retorno = false;
                                    // JCañarte 1Feb2021  colocar Info y Error Log en toda pedidos App.
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no es válido. Ingrese un Pedido válido.");
                                    MessageBox.Show("El Pedido de App del Portal no es válido. Ingrese un Pedido válido.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                                prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                            }
                            else
                            {
                                retorno = false;
                                // JCañarte 1Feb2021  colocar Info y Error Log en toda pedidos App.
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no es válido. Ingrese un Pedido válido.");
                                MessageBox.Show("El Pedido de App del Portal no es válido. Ingrese un Pedido válido.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        if (retorno)
                        {
                            //queryPedido = queryPedido2;
                            var queryPick = (from cpo in db.Tbl_PickOrderCab
                                             where cpo.FacturaId == queryPedido.id
                                             select cpo).FirstOrDefault();
                            if (queryPick != null)
                            {
                                if (queryPick.Estado == 5)
                                {
                                    retorno = true;
                                }
                                else
                                {
                                    retorno = false;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no ha realizado el picking. Favor primero realice el picking antes de pasar por POS.");
                                    MessageBox.Show("El Pedido de App del Portal no ha realizado el picking. Favor primero realice el picking antes de pasar por POS.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                                {
                                    prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                                    var db1 = new POSEntities();
                                    var queryPick1 = (from cpo in db1.Tbl_PickOrderCab
                                                     where cpo.FacturaId == queryPedido.id
                                                     select cpo).FirstOrDefault();
                                    if (queryPick1 != null)
                                    {
                                        if (queryPick1.Estado == 5)
                                        {
                                            retorno = true;
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "validoPedidoAppDelPortal", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                                        }
                                        else
                                        {
                                            retorno = false;
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no ha realizado el picking. Favor primero realice el picking antes de pasar por POS.");
                                            MessageBox.Show("El Pedido de App del Portal no ha realizado el picking. Favor primero realice el picking antes de pasar por POS.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        retorno = false;
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no tiene realizado el picking. Favor primero realice el picking antes de pasar por POS.");
                                        MessageBox.Show("El Pedido de App del Portal no tiene realizado el picking. Favor primero realice el picking antes de pasar por POS.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    }
                                    prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                                }
                                else
                                {
                                    retorno = false;
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no tiene realizado el picking. Favor primero realice el picking antes de pasar por POS.");
                                    MessageBox.Show("El Pedido de App del Portal no tiene realizado el picking. Favor primero realice el picking antes de pasar por POS.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                            }

                        }
                        /*

                        var quer1 = from p in db.People
                        join e in db.EmailAddresses
                        on p.BusinessEntityID equals e.BusinessEntityID
                        where p.FirstName == "KEN"
                        select new
                        {
                            ID = p.BusinessEntityID,
                            FirstName = p.FirstName,
                            MiddleName = p.MiddleName,
                            LastName = p.LastName,
                            EmailID = e.EmailAddress1
                        }).ToList();

                        */

                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PedidosOtrasApp", "Confirmación de Pedido " + CanalVentaNombre, "El Pedido de App del Portal no ha realizado el picking. Favor primero realice el picking antes de pasar por POS.");
                MessageBox.Show("Se presentaron problemas al procesar este pedido App.", "Confirmación de Pedido " + CanalVentaNombre, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            return retorno;
        }
        private void txtNumPedidoOtraApp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if(Aceptar())
                    this.Close();
            }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNumPedidoOtraApp.Text))
            {
                if (MessageBox.Show(this, "Tiene ingresado un número de Pedido. ¿Está seguro de Continuar?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    _retornarMenuInicial = true;
                    this.Close();
                }
                
            }
            else
            {
                _retornarMenuInicial = true;
                this.Close();
            }
        }



        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    if (!string.IsNullOrEmpty(txtNumPedidoOtraApp.Text))
                    {
                        if (MessageBox.Show(this, "Tiene ingresado un número de Pedido. ¿Está seguro de Continuar?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            _retornarMenuInicial = true;
                            this.Close();
                        }

                    }
                    else
                    {
                        _retornarMenuInicial = true;
                        this.Close();
                    }
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }



        private bool validaExisteOrdenProveedor()
        {
            bool retorno = false;
            try
            {
                POSEntities db = new POSEntities();
                if (!string.IsNullOrEmpty(txtNumPedidoOtraApp.Text))
                {
                    if (db.TblOrdenDelivery.Any(t => t.IdDelivery == CanalVentaTipo && t.OrderId == txtNumPedidoOtraApp.Text))
                    {
                        retorno = true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            return retorno;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
