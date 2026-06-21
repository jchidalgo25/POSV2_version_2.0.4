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
using POS.Control;
using System.Data.SqlClient;
using MensajesLibrary;

namespace POS.Control.Clientes
{
    public partial class SearchPedidoDomicilio : Form
    {
        public MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
        public string code;
        System.Windows.Forms.Control focused;
        Boolean EsPedidoDomicilio_ = false;

        public pos_customer SelectedCustomer { get; set; }
        public string NumeroPedidoDomicilio { get; set; }
        public bool EsPedidoDomicilio() { return this.EsPedidoDomicilio_;    }
        string Cliente { get; set; }

        public SearchPedidoDomicilio()
        {
            Init();
        }

        public SearchPedidoDomicilio(string _clienteAccountNum)
        {
            Cliente = _clienteAccountNum;
            Init();
        }

        public SearchPedidoDomicilio(pos_customer previousSelectedCustomer)
        {
            SelectedCustomer = previousSelectedCustomer;
            Init();
        }

        private void Init()
        {
            InitializeComponent();            
            focused = (System.Windows.Forms.Control)txtNumPedidoDomicilio;
            
        }

        private void BuscaPedidoDomicilio()
        {
            int numPedidoApp = 0;
            var criterio = txtNumPedidoDomicilio.Text.Trim();
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();


            if (string.IsNullOrEmpty(criterio))
            {
                return;
            }
            else
            {
                numPedidoApp = Convert.ToInt32(criterio);
            }
            if (criterio.Length > 0)
            {
                try
                {
                    var db = new POSEntities();
                    if (db.core_facturaAPP.Any(x => x.orderApp == numPedidoApp && x.establecimiento == Control.Common.GlobalParameters.Establecimiento && x.cliente == Cliente))
                    {
                        var factPedDomici = db.core_facturaAPP.Where(x => x.orderApp == numPedidoApp && x.establecimiento == Control.Common.GlobalParameters.Establecimiento && x.cliente == Cliente).FirstOrDefault();
                        if (string.IsNullOrEmpty(factPedDomici.orderID))
                        {
                            EsPedidoDomicilio_ = true;
                            NumeroPedidoDomicilio = criterio;
                        }

                    }
                    else
                    {
                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                        {
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);

                            var db1 = new POSEntities();
                            if (db1.core_facturaAPP.Any(x => x.orderApp == numPedidoApp && x.establecimiento == Control.Common.GlobalParameters.Establecimiento && x.cliente == Cliente))
                            {
                                var factPedDomici = db1.core_facturaAPP.Where(x => x.orderApp == numPedidoApp && x.establecimiento == Control.Common.GlobalParameters.Establecimiento && x.cliente == Cliente).FirstOrDefault();
                                if (string.IsNullOrEmpty(factPedDomici.orderID))
                                {
                                    EsPedidoDomicilio_ = true;
                                    NumeroPedidoDomicilio = criterio;
                                }
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "SearchPedidoDomicilio", "BuscarPedidoDomicilio", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                            }
                            else
                            {
                                Control.Common.General.GetMensajeToList(574);
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Número de Pedido no encontrado, ingrese un Pedido Válido.", "POS - Busqueda de Artículo");
                                //MessageBox.Show(this, "Número de Pedido no encontrado, ingrese un Pedido Válido.", "Busqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                        }
                        else
                        {
                            Control.Common.General.GetMensajeToList(574);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Número de Pedido no encontrado, ingrese un Pedido Válido.", "POS - Busqueda de Artículo");
                            //MessageBox.Show(this, "Número de Pedido no encontrado, ingrese un Pedido Válido.", "Busqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
                catch (Exception ex)
                {

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = ex.Message.ToString() });
                    Control.Common.General.GetMensajeToList(535, parametros);


                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Se produjo un error en la búsqueda, por favor vuelva a intentarlo. Error: " + ex.Message.ToString(), "POS - Busqueda de Artículo");
                    //MessageBox.Show(this, "Se produjo un error en la búsqueda, por favor vuelva a intentarlo. Error: " + ex.Message.ToString(), "Búsqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    //MessageBox.Show(this,"Detalle del error: " + ex.InnerException.ToString());
                }
            }
            
            
            btnChooseProduct.Focus();
        }

        private void txtProduct_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BuscaPedidoDomicilio();
        }

    
        private void prCambioCadenaConexion(string srvSelect , string srvPedidos)
        {
            try
            {
                string yourConnection = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"].ConnectionString.Replace(srvSelect, srvPedidos);
                //dcon = new POSEntities(yourConnection);
                var DBCS = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"];
                var writable = typeof(System.Configuration.ConfigurationElement).GetField("_bReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                writable.SetValue(DBCS, false);
                DBCS.ConnectionString = yourConnection;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "SearchPedidoDomicilio", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
            }
            catch (Exception ex)
            { }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnKb_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();

        }

        private void SearchProduct_Load(object sender, EventArgs e)
        {
            txtNumPedidoDomicilio.Focus();
        }

      
        private void txtCliente_Enter(object sender, EventArgs e)
        {
            AcceptButton = btnSearch;
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            AcceptButton = btnChooseProduct;
        }

        private void txtNumPedidoDomicilio_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
               
                if (e.KeyChar == (char)Keys.Enter)
                {
                    BuscaPedidoDomicilio();
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.SearchPedidoDomicilio", "txtNumPedidoDomicilio_KeyPress", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
            

       }

        private void btnChooseProduct_Click(object sender, EventArgs e)
        {
           
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            BuscaPedidoDomicilio();

            if (EsPedidoDomicilio_)
            {
                
               this.Close();
            }
            else
            {

                Control.Common.General.GetMensajeToList(576);

                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " Debe seleccionar un Pedido a Domicilio válido.", " POS - Serv. Domicilio ");
                //MsgBox m = new MsgBox("info", "Debe seleccionar un Pedido a Domicilio válido.", "Serv. Domicilio");
                //DialogResult dg = m.ShowDialog();

                //Control.Common.WinForm.ShowMessage("Debe seleccionar un Pedido a Domicilio válido.");

            }
        }


        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    this.Close();
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }



    }
}
