using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using POS.Models;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Telerik.WinControls.UI;
using System.Threading.Tasks;

namespace POS.Control.Clientes
{
    public partial class ClienteForm : Telerik.WinControls.UI.RadForm
    {
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        public pos_customer _cliente;
        public string identificacion;
        private bool _deseaPermitirCambioCedula = false;
        System.Windows.Forms.Control ultimoControlActivo = null;

        public bool DeseaPermitirCambioCedula
        {
            get { return _deseaPermitirCambioCedula; }
            set { _deseaPermitirCambioCedula = value; }
        }

        private bool _deseaPermitirCambioBasico = false;
        public bool DeseaPermitirCambioBasico
        {
            get { return _deseaPermitirCambioBasico; }
            set { _deseaPermitirCambioBasico = value; }
        }

        private bool _debeComprobarTarPortal = true;
        public bool DebeComprobarTarPortal
        {
            get { return _debeComprobarTarPortal; }
            set { _debeComprobarTarPortal = value; }
        }

        public ClienteForm()
        {
            InitializeComponent();
            txtMail.Select();
            txtMail.Focus();

            SuscribirTextBoxes(this);

        }

        private void SuscribirTextBoxes(System.Windows.Forms.Control parent)
        {
            foreach (System.Windows.Forms.Control control in parent.Controls)
            {
                if (control is TextBox || control is RadTextBox)
                {
                    control.Enter += (s, e) => ultimoControlActivo = control;
                }

                if (control.HasChildren)
                {
                    SuscribirTextBoxes(control);
                }
            }
        }


        private void ClienteForm_Load(object sender, EventArgs e)
        {
            txtCedula.Text = identificacion;

            txtCedula.Enabled = false;
            txtNombres.Enabled = false;
            txtTelefono.Enabled = false;
            txtDireccion.Enabled = false;
            

            if (_cliente != null)
            {
                txtCedula.Text = _cliente.ACCOUNTNUM;
                txtNombres.Text = _cliente.NAME;
                txtTelefono.Text = _cliente.PHONE;
                txtDireccion.Text = _cliente.ADDRESS;
                txtMail.Text = _cliente.EMAIL;

                cmbTipoCliente.SelectedIndex = 0;
                cmbTipoCliente.Enabled = false;
            }

            if (DeseaPermitirCambioCedula) txtCedula.Enabled = true;

            if (DeseaPermitirCambioBasico)
            {
                txtDireccion.Enabled = true;
                txtMail.Enabled = true;
                txtNombres.Enabled = true;
                txtTelefono.Enabled = true;
            }
        }

        private bool verificar()
        {
            bool response = true;
            var identificacion = txtCedula.Text.Trim();

            if (!VerificarBase())
            {
                response = false;
            }
            else if (!(txtCedula.Text.Length > 0 && cmbTipoCliente.SelectedIndex != -1))
            {
                response = false;
                Control.Common.General.GetMensajeToList(318);
                //MessageBox.Show(this, "Revise que todos los campos esten llenos!");
            }
            else if (cmbTipoCliente.SelectedItem.Text == "Natural" && !ValidarIdentificador.ValidarCedula(identificacion))
            {
                response = false;
                Control.Common.General.GetMensajeToList(319);
                //MessageBox.Show(this, "Identificación no es válida, si la identificación es RUC seleccionar persona Jurídica!");
            }
            else if (cmbTipoCliente.SelectedItem.Text == "Juridica" && !(ValidarIdentificador.ValidarRUCPrivada(identificacion)
                                                                    || ValidarIdentificador.ValidarRUCPublica(identificacion)
                                                                    || ValidarIdentificador.ValidarRUCNatural(identificacion)))
            {
                response = false;
                Control.Common.General.GetMensajeToList(320);
                //MessageBox.Show(this, "Identificación no es válida, si la identificación es Cédula seleccionar persona Natural!");
            }
            else if (cmbTipoCliente.SelectedItem.Text == "Extranjero")
            {
                response = false;

                Control.Common.General.GetMensajeToList(321);
                //MessageBox.Show(this, "Identificación no es válida, si es Pasaporte ingrésela en AX !");
            }
            else if (txtMail.Text.Trim() != "" && !ValidarEmail(txtMail.Text.Trim()))
            {
                response = false;
            }

            return response;
        }

        private bool VerificarBase()
        {

            bool response = true;
            var identificacion = txtCedula.Text.Trim();

            txtNombres.Text = Common.StringHelper.ToAlphaNumeric(txtNombres.Text.Trim(), true);
            if (txtNombres.Text.Trim().Length <= 7)
            {
                response = false;
                //MessageBox.Show(this, "Revise que el campo Nombres esté mejor detallado y no usar caracteres especiales!");
                Control.Common.General.GetMensajeToList(322);

            }
            else if (txtDireccion.Text.Trim().Length < 10)
            {
                response = false;
                //MessageBox.Show(this, "Revise que el campo Direccion esté lleno y tenga mínimo 10 caracteres!");
                Control.Common.General.GetMensajeToList(323);

            }
            else if (txtTelefono.Text.Trim().Length <= 1)
            {
                response = false;
                //MessageBox.Show(this, "Revise que el campo Teléfono esté lleno!");
                Control.Common.General.GetMensajeToList(323);
            }

            return response;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void insertActualizaClientePOSAX(string identificacion)
        {
            //string mail = string.Concat(txtMail.Text, txtMailDominio.Text);
            string mail = txtMail.Text;

            string datosCliente = "{ Nombres: " + txtNombres.Text.Trim()
                + ", Cedula: " + txtCedula.Text.Trim() + ", Direccion: "
                + txtDireccion.Text + ", Telefono: "
                + txtTelefono.Text.Trim() + ", Email: "
                + mail.Trim() + " }";

            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            pos_customer cli = new pos_customer();
            var ident = cmbTipoCliente.SelectedItem.Tag as string;

            bool ejecutarActualizacion = false;
            if (_cliente == null)
            {
                if (verificar())
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Enviando solicitud de ingresar cliente "
                                            + datosCliente + ". Usuario logon: "
                                            + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));

                    //var c = Cliente.createCliente(txtNombres.Text, txtCedula.Text, ident, txtDireccion.Text, txtTelefono.Text, txtMail.Text);
                    //var ident = cmbTipoCliente.SelectedItem.Tag as string;
                    using (var db = new POSEntities())
                    {
                        if (db.pos_customer.Any(x => x.ACCOUNTNUM.Equals(txtCedula.Text.Trim())))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Acción denegada. Ya existe registrado un cliente con la identificación '" + txtCedula.Text.Trim());

                            parametros.Add(new ParametrosMensajes() { codigo = "[cedula_cliente]", valor = txtCedula.Text.Trim() });
                            Control.Common.General.GetMensajeToList(325, parametros);

                            //MessageBox.Show(this, "Acción denegada. Ya existe registrado un cliente con la identificación '" + txtCedula.Text.Trim() + "', por favor verifique que haya escrito correctamente el número de identificación");
                            return;
                        }

                        // Extraemos el ObjectContext del DbContext (a partir de Entity Framework 4.1)
                        cli = new pos_customer();
                        cli.NAME = txtNombres.Text.Trim();
                        cli.ACCOUNTNUM = txtCedula.Text.Trim();
                        cli.VATNUM = txtCedula.Text.Trim();
                        cli.COUNTRYREGIONID = txtPais.Text;
                        cli.STREET = txtDireccion.Text;
                        cli.CUSTGROUP = "08";// ident;
                        cli.ADDRESS = txtDireccion.Text.Trim();
                        cli.PHONE = txtTelefono.Text.Trim();
                        cli.EMAIL = mail;
                        cli.CREATEDDATETIME = DateTime.Now;
                        cli.MODIFIEDDATETIME = DateTime.Now;
                        cli.DATAAREAID = "liri";
                        cli.CELLULARPHONE = "";
                        cli.DIMENSION = "";
                        cli.DIMENSION2_ = "";
                        cli.DIMENSION3_ = "";
                        cli.MANDATORYCREDITLIMIT = 0;
                        cli.PHONELOCAL = ident;
                        cli.TELEFAX = "";
                        cli.CREDITMAX = 0;
                        cli.RECID = 69;

                        ejecutarActualizacion = true;
                    }
                }
            }
            else
            {
                if (txtMail.Text != "")
                {
                    if (ValidarEmail(txtMail.Text.Trim()) && VerificarBase())
                    {
                        try
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Enviando solicitud de actualizar cliente " + datosCliente + ". Usuario logon: " + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));

                            cli.ACCOUNTNUM = txtCedula.Text.Trim();
                            cli.NAME = txtNombres.Text.Trim();
                            cli.STREET = txtDireccion.Text;
                            cli.ADDRESS = txtDireccion.Text.Trim();
                            cli.PHONE = txtTelefono.Text.Trim();
                            cli.EMAIL = txtMail.Text.Trim();
                            cli.MODIFIEDDATETIME = DateTime.Now;
                            cli.PHONELOCAL = ident;
                            
                        }
                        catch (Exception ex)
                        {
                            var msjParaUser = "La solicitud de actualizar el cliente no pudo completarse, esto puede deberse a una breve interrupción en la comunicación. Por favor volverlo a intentar en unos momentos. ";
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ClienteForm", "btnGrabar_Click", msjParaUser + "A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                            //MessageBox.Show(msjParaUser);
                            Control.Common.General.GetMensajeToList(326);
                        }

                        ejecutarActualizacion = true;
                    }


                }
                else
                {
                    MessageBox.Show(this, "Debe registrar el e-mail!");
                    return;
                }
            }

            if (ejecutarActualizacion)
            {
                DataSet dtsConsulta = Common.General.InsertaAcutalizaCltePOS(cli);

                int CodError = 0;
                string MsjError = string.Empty;
                int idMensaje = 0;
                string Mensaje = string.Empty;
                string identiticacion = string.Empty;

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            CodError = Int32.Parse(data["CodError"].ToString());
                            MsjError = data["MsjError"].ToString();
                            idMensaje = Int32.Parse(data["idMensaje"].ToString());
                            Mensaje = data["Mensaje"].ToString();
                            identiticacion = data["ACCOUNTNUM"].ToString();
                        }
                    }

                }


                if (CodError != 0)
                {
                    Control.Common.General.GetMensajeToList(idMensaje);
                    return;
                }


                CodError = 0;
                MsjError = string.Empty;
                idMensaje = 0;
                Mensaje = string.Empty;

                DataSet dtsActAX = Common.General.AcutalizaClteAX(identiticacion);
                if (dtsActAX.Tables.Count > 0)
                {
                    if (dtsActAX.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            CodError = Int32.Parse(data["CodError"].ToString());
                            MsjError = data["CodError"].ToString();
                            idMensaje = Int32.Parse(data["idMensaje"].ToString());
                            Mensaje = data["Mensaje"].ToString();
                            identiticacion = data["ACCOUNTNUM"].ToString();
                        }
                    }

                }


                core_tarjetacreditointerno tarjeta = null;
                core_tarjetacreditointerno tarjetaAdicional = null;

                var c = Cliente.setEmail(txtCedula.Text, txtTelefono.Text, txtMail.Text);
                this._cliente = Cliente.getCliente(cli.ACCOUNTNUM, out tarjeta, out tarjetaAdicional, DebeComprobarTarPortal);

                this.Close();
            }
            


        }
        private async Task insertAsyncActualizaClientePOSAX(string identificacion)
        {
            string mail = txtMail.Text;

            string datosCliente = "{ Nombres: " + txtNombres.Text.Trim()
                + ", Cedula: " + txtCedula.Text.Trim() + ", Direccion: "
                + txtDireccion.Text + ", Telefono: "
                + txtTelefono.Text.Trim() + ", Email: "
                + mail.Trim() + " }";

            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            pos_customer cli = new pos_customer();
            var ident = cmbTipoCliente.SelectedItem.Tag as string;

            bool ejecutarActualizacion = false;
            if (_cliente == null)
            {
                if (verificar())
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Enviando solicitud de ingresar cliente "
                                            + datosCliente + ". Usuario logon: "
                                            + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));

                    using (var db = new POSEntities())
                    {
                        if (db.pos_customer.Any(x => x.ACCOUNTNUM.Equals(txtCedula.Text.Trim())))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Acción denegada. Ya existe registrado un cliente con la identificación '" + txtCedula.Text.Trim());

                            parametros.Add(new ParametrosMensajes() { codigo = "[cedula_cliente]", valor = txtCedula.Text.Trim() });
                            Control.Common.General.GetMensajeToList(325, parametros);

                            return;
                        }

                        cli = new pos_customer();
                        cli.NAME = txtNombres.Text.Trim();
                        cli.ACCOUNTNUM = txtCedula.Text.Trim();
                        cli.VATNUM = txtCedula.Text.Trim();
                        cli.COUNTRYREGIONID = txtPais.Text;
                        cli.STREET = txtDireccion.Text;
                        cli.CUSTGROUP = "08";
                        cli.ADDRESS = txtDireccion.Text.Trim();
                        cli.PHONE = txtTelefono.Text.Trim();
                        cli.EMAIL = mail;
                        cli.CREATEDDATETIME = DateTime.Now;
                        cli.MODIFIEDDATETIME = DateTime.Now;
                        cli.DATAAREAID = "liri";
                        cli.CELLULARPHONE = "";
                        cli.DIMENSION = "";
                        cli.DIMENSION2_ = "";
                        cli.DIMENSION3_ = "";
                        cli.MANDATORYCREDITLIMIT = 0;
                        cli.PHONELOCAL = ident;
                        cli.TELEFAX = "";
                        cli.CREDITMAX = 0;
                        cli.RECID = 69;

                        ejecutarActualizacion = true;
                    }
                }
            }
            else
            {
                if (txtMail.Text != "")
                {
                    if (ValidarEmail(txtMail.Text.Trim()) && VerificarBase())
                    {
                        try
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Enviando solicitud de actualizar cliente " + datosCliente + ". Usuario logon: " + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));

                            cli.ACCOUNTNUM = txtCedula.Text.Trim();
                            cli.NAME = txtNombres.Text.Trim();
                            cli.STREET = txtDireccion.Text;
                            cli.ADDRESS = txtDireccion.Text.Trim();
                            cli.PHONE = txtTelefono.Text.Trim();
                            cli.EMAIL = txtMail.Text.Trim();
                            cli.MODIFIEDDATETIME = DateTime.Now;
                            cli.PHONELOCAL = ident;

                        }
                        catch (Exception ex)
                        {
                            var msjParaUser = "La solicitud de actualizar el cliente no pudo completarse, esto puede deberse a una breve interrupción en la comunicación. Por favor volverlo a intentar en unos momentos. ";
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ClienteForm", "btnGrabar_Click", msjParaUser + "A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                            Control.Common.General.GetMensajeToList(326);
                        }

                        this.Close();
                        ejecutarActualizacion = true;
                    }
                    
                }
                else
                {
    
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        Control.Common.General.GetMensajeToList(645);
                    });
                    //Control.Common.General.GetListMensaje(645);
                    //MessageBox.Show(this, "Debe registrar el e-mail!");
                    return;
                }
            }

            if (ejecutarActualizacion)
            {
                DataSet dtsConsulta = Common.General.InsertaAcutalizaCltePOS(cli);

                int CodError = 0;
                string MsjError = string.Empty;
                int idMensaje = 0;
                string Mensaje = string.Empty;
                string identiticacion = string.Empty;

                if (dtsConsulta.Tables.Count > 0 && dtsConsulta.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                    {
                        CodError = Int32.Parse(data["CodError"].ToString());
                        MsjError = data["MsjError"].ToString();
                        idMensaje = Int32.Parse(data["idMensaje"].ToString());
                        Mensaje = data["Mensaje"].ToString();
                        identiticacion = data["ACCOUNTNUM"].ToString();
                    }
                }

                if (CodError != 0)
                {
                    Control.Common.General.GetMensajeToList(idMensaje);
                    return;
                }

                // ✅ Aquí está la parte fija: ahora puedes usar await
                DataSet dtsActAX = await Common.General.ActualizaClteAXAsync(identiticacion);

                if (dtsActAX.Tables.Count > 0 && dtsActAX.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow data in dtsActAX.Tables[0].Rows)
                    {
                        CodError = Int32.Parse(data["CodError"].ToString());
                        MsjError = data["CodError"].ToString();
                        idMensaje = Int32.Parse(data["idMensaje"].ToString());
                        Mensaje = data["Mensaje"].ToString();
                        identiticacion = data["ACCOUNTNUM"].ToString();
                    }
                }

                //core_tarjetacreditointerno tarjeta = null;
                //core_tarjetacreditointerno tarjetaAdicional = null;

                //var c = Cliente.setEmail(txtCedula.Text, txtTelefono.Text, txtMail.Text);
                //this._cliente = Cliente.getCliente(cli.ACCOUNTNUM, out tarjeta, out tarjetaAdicional, DebeComprobarTarPortal);

                //this.Close();
            }
        }


        private void ejecutaGrabarAnt()
        {

            //string mail = string.Concat(txtMail.Text, txtMailDominio.Text);
            string mail = txtMail.Text;

            string datosCliente = "{ Nombres: " + txtNombres.Text.Trim()
                + ", Cedula: " + txtCedula.Text.Trim() + ", Direccion: "
                + txtDireccion.Text + ", Telefono: "
                + txtTelefono.Text.Trim() + ", Email: "
                + mail.Trim() + " }";

            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if (_cliente == null)
            {
                if (verificar())
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Enviando solicitud de ingresar cliente "
                                            + datosCliente + ". Usuario logon: "
                                            + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));


                    var ident = cmbTipoCliente.SelectedItem.Tag as string;
                    //var c = Cliente.createCliente(txtNombres.Text, txtCedula.Text, ident, txtDireccion.Text, txtTelefono.Text, txtMail.Text);

                    //
                    //var ident = cmbTipoCliente.SelectedItem.Tag as string;
                    using (var db = new POSEntities())
                    {
                        if (db.pos_customer.Any(x => x.ACCOUNTNUM.Equals(txtCedula.Text.Trim())))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Acción denegada. Ya existe registrado un cliente con la identificación '" + txtCedula.Text.Trim());

                            parametros.Add(new ParametrosMensajes() { codigo = "[cedula_cliente]", valor = txtCedula.Text.Trim() });
                            Control.Common.General.GetMensajeToList(325, parametros);

                            //MessageBox.Show(this, "Acción denegada. Ya existe registrado un cliente con la identificación '" + txtCedula.Text.Trim() + "', por favor verifique que haya escrito correctamente el número de identificación");
                            return;
                        }

                        pos_customer cli = new pos_customer();

                        // Extraemos el ObjectContext del DbContext (a partir de Entity Framework 4.1)

                        cli.NAME = txtNombres.Text.Trim();
                        cli.ACCOUNTNUM = txtCedula.Text.Trim();
                        cli.VATNUM = txtCedula.Text.Trim();
                        cli.COUNTRYREGIONID = txtPais.Text;
                        cli.STREET = txtDireccion.Text;
                        cli.CUSTGROUP = "08";// ident;
                        cli.ADDRESS = txtDireccion.Text.Trim();
                        cli.PHONE = txtTelefono.Text.Trim();
                        cli.EMAIL = mail;
                        cli.CREATEDDATETIME = DateTime.Now;
                        cli.MODIFIEDDATETIME = DateTime.Now;
                        cli.DATAAREAID = "liri";
                        cli.CELLULARPHONE = "";
                        cli.DIMENSION = "";
                        cli.DIMENSION2_ = "";
                        cli.DIMENSION3_ = "";
                        cli.MANDATORYCREDITLIMIT = 0;
                        cli.PHONELOCAL = ident;
                        cli.TELEFAX = "";
                        cli.CREDITMAX = 0;
                        cli.RECID = 69;


                        db.pos_customer.Add(cli);
                        db.SaveChanges();

                        core_tarjetacreditointerno tarjeta = null;
                        core_tarjetacreditointerno tarjetaAdicional = null;

                        this._cliente = Cliente.getCliente(cli.ACCOUNTNUM, out tarjeta, out tarjetaAdicional, DebeComprobarTarPortal);

                        this.Close();

                    }


                    /* 
                    core_tarjetacreditointerno tarjeta = null;
                    this._cliente = Cliente.getCliente(c.Accountnum, out tarjeta);

                   var c = Cliente.createCliente(txtNombres.Text, txtCedula.Text, ident, txtDireccion.Text, txtTelefono.Text, txtMail.Text);
       
                 

                    if (c.Grabar)
                    {
                        //this._cliente = new pos_customer { ACCOUNTNUM = c.Accountnum, NAME = c.Nombre, ADDRESS = c.Direccion, PHONE = c.Telefono, VATNUM=c.Accountnum };
                        core_tarjetacreditointerno tarjeta = null;
                        this._cliente = Cliente.getCliente(c.Accountnum, out tarjeta);

                        this.Close();
                    }
                    else
                    {
                        if (c.Mensaje.Length > 0)
                        {
                            MessageBox.Show(this,c.Mensaje);
                        }
                    }*/
                }
            }
            else
            {
                if (txtMail.Text != "")
                {
                    if (ValidarEmail(txtMail.Text.Trim()) && VerificarBase())
                    {
                        try
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Clientes.ClienteForm", "btnGrabar_Click", "Enviando solicitud de actualizar cliente " + datosCliente + ". Usuario logon: " + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));

                            using (POSEntities db = new POSEntities())
                            {
                                var cli = db.pos_customer.Where(x => x.ACCOUNTNUM == _cliente.ACCOUNTNUM).FirstOrDefault();
                                if (cli != null)
                                {
                                    cli.NAME = txtNombres.Text.Trim();
                                    cli.STREET = txtDireccion.Text;
                                    cli.ADDRESS = txtDireccion.Text.Trim();
                                    cli.PHONE = txtTelefono.Text.Trim();
                                    cli.EMAIL = txtMail.Text.Trim();
                                    cli.MODIFIEDDATETIME = DateTime.Now;

                                    db.SaveChanges();
                                }
                            }
                            core_tarjetacreditointerno tarjeta = null;
                            core_tarjetacreditointerno tarjetaAdicional = null;
                            var c = Cliente.setEmail(txtCedula.Text, txtTelefono.Text, txtMail.Text);
                            this._cliente = Cliente.getCliente(txtCedula.Text, out tarjeta, out tarjetaAdicional);
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            var msjParaUser = "La solicitud de actualizar el cliente no pudo completarse, esto puede deberse a una breve interrupción en la comunicación. Por favor volverlo a intentar en unos momentos. ";
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ClienteForm", "btnGrabar_Click", msjParaUser + "A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                            //MessageBox.Show(msjParaUser);
                            Control.Common.General.GetMensajeToList(326);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "Debe registrar el e-mail!");
                }
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {

            //this.Close();
            var actualiza = insertAsyncActualizaClientePOSAX(txtCedula.Text.Trim());
            //insertActualizaClientePOSAX(txtCedula.Text.Trim());
            core_tarjetacreditointerno tarjeta = null;
            core_tarjetacreditointerno tarjetaAdicional = null;

            var c = Cliente.setEmail(txtCedula.Text, txtTelefono.Text, txtMail.Text);
            this._cliente = Cliente.getCliente(txtCedula.Text, out tarjeta, out tarjetaAdicional, DebeComprobarTarPortal);
            return;

        }

        public bool ValidarEmail(string cadenaEmails)
        {
            string[] splitMails = cadenaEmails.Split(',');

            foreach (string mail in splitMails)
            {
                if (!Regex.IsMatch(mail, "^([a-zA-Z0-9_.+-])+@(([a-zA-Z0-9-])+\\.)+([a-zA-Z0-9]{2,4})+$") || (mail != mail.Trim()))
                {
                    //MessageBox.Show(this, "Email no es válido! Si son varios sepárelos por coma (,) y sin dejar espacios");
                    Control.Common.General.GetMensajeToList(327);

                    return false;
                }
            }

            return true;
        }


        private void radLabel8_Click(object sender, EventArgs e)
        {

        }
        private void radLabel9_Click(object sender, EventArgs e)
        {

        }

        private void txtCedula_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtCedula_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (cmbTipoCliente.SelectedIndex == -1)
            {
                //MessageBox.Show(this,"Seleccione el tipo de identificación");
                Control.Common.General.GetMensajeToList(328);

                e.Handled = true;
            }
            else
            {
                if (cmbTipoCliente.SelectedItem.Text == "Natural" || cmbTipoCliente.SelectedItem.Text == "Juridica")
                {
                    if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar) || Char.IsSeparator(e.KeyChar))
                    {
                        e.Handled = false;

                        if (cmbTipoCliente.SelectedItem.Text == "Natural")
                        {
                            txtCedula.MaxLength = 10;
                        }
                        if (cmbTipoCliente.SelectedItem.Text == "Juridica")
                        {
                            txtCedula.MaxLength = 13;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                        //MessageBox.Show(this,"Sólo se permite ingresar números");
                        Control.Common.General.GetMensajeToList(329);

                    }
                }
            }
            
        }

        private void txtCedula_Leave(object sender, EventArgs e)
        {
            if (txtCedula.TextLength > 0)
            {
                if (cmbTipoCliente.SelectedItem.Text == "Natural" && txtCedula.TextLength < 10)
                {
                    //MessageBox.Show(this,"El número de cédula debe tener 10 dígitos!");
                    Control.Common.General.GetMensajeToList(330);
                    txtCedula.Focus();
                }
                if (cmbTipoCliente.SelectedItem.Text == "Juridica" && txtCedula.TextLength < 13)
                {
                    //MessageBox.Show(this,"El número de cédula debe tener 13 dígitos!");
                    Control.Common.General.GetMensajeToList(331);
                    txtCedula.Focus();
                }
            }
            
        }

        private void txtCiudad_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnKbd_Click(object sender, EventArgs e)
        {

          

            try
            {

                // Usamos el último control activo, no ActiveControl actual
                if (ultimoControlActivo is TextBox txtBox)
                {
                    var teclado = new ToolBox.frmTecladoCompleto(txtBox);
                    teclado.TopMost = true;
                    teclado.ShowDialog();
                }
                else if (ultimoControlActivo is RadTextBox radTxtBox)
                {
                    var txtInterno = radTxtBox.TextBoxElement.TextBoxItem.HostedControl as TextBox;
                    if (txtInterno != null)
                    {
                        var teclado = new ToolBox.frmTecladoCompleto(txtInterno);
                        teclado.TopMost = true;
                        teclado.ShowDialog();
                    }
                }              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al abrir el teclado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show($"Ocurrió un error al abrir el teclado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClienteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("tabtip");
            for (int i = 0; i < procs.Length; i++)
            {
                try
                {
                    procs[i].Kill();
                }
                catch
                {
                }
            }
            procs = Process.GetProcessesByName("osk");
            for (int i = 0; i < procs.Length; i++)
            {
                try
                {
                    procs[i].Kill();
                }
                catch
                {
                }
            }

        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
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



        private void btnArroba_Click(object sender, EventArgs e)
        {
            string caracterEspecial = "@"; // Carácter a agregar
            string textoActual = txtMail.Text;
            int posicion = txtMail.SelectionStart; // Obtenemos la posición del cursor

            // Insertamos el carácter en la posición del cursor
            string textoModificado = textoActual.Insert(posicion, caracterEspecial);

            // Actualizamos el TextBox
            txtMail.Text = textoModificado;

            // Mover el cursor después del carácter insertado
            txtMail.SelectionStart = posicion + 1;
            txtMail.Focus();
        }

    }
}

