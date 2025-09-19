using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using Telerik.WinControls;
using System.Windows.Forms;
using System.Data.SqlClient;
using MensajesLibrary;


namespace POS.Control
{
    public class Cliente
    {
        /*string _codigoAX;
        string _cedulaRUC;
        string _direccion;
        string _telefono;
        string _custgroup;

        public string CodigoAx {
            get { return _codigoAX; }
        }
        public string CedulaRUC
        {
            get { return _cedulaRUC; }
        }
        public string Direccion
        {
            get { return _direccion; }
        }
        public string Telefono
        {
            get { return _telefono; }
        }

        public string CustGroup
        {
            get { return _custgroup; }
        }*/

        public static MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
        public static MsgBoxCtrl.MessageType messageType;
        public static  MsgBoxCtrl.MessageBoxResult result;


        public static bool clienteExiste(string cedula)
        {
            var existe = false;
            var fechaValida = false;
            
            using (var db = new POSEntities()) 
           {
                if (!cedula.StartsWith("%"))
                {
                    existe = db.pos_customer.Any(x => x.VATNUM == cedula || x.ACCOUNTNUM == cedula);
                    if (!existe)
                    {
                        return false;
                    }
                }
               

                if (cedula.StartsWith("%"))
                {
                    var tarjeta = System.Text.RegularExpressions.Regex.Match(cedula, @"\%(\w+)\^").Groups[1].Value;

                    if (string.IsNullOrEmpty(tarjeta))
                    {
                        tarjeta = System.Text.RegularExpressions.Regex.Match(cedula, @"\%(\w+)\&").Groups[1].Value;
                    }

                    if (string.IsNullOrEmpty(tarjeta))
                    {
                        tarjeta = System.Text.RegularExpressions.Regex.Match(cedula, @"\%(\w+)\*").Groups[1].Value;
                    }
                   
                    tarjeta = tarjeta.ToUpper().Replace("B", "");

                    var t = ConsultarTarjetaInternaGen(tarjeta);

                    //    if (existe == true)
                    if (t != null)
                    {
                        existe = t.activo;

                        if (!t.activo)
                        {
                            //MessageBox.Show("La tarjeta esta desactivada. Comuniquese inmediatamente con el administrador del local.", "Tarjeta Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            Control.Common.General.GetMensajeToList(561);

                            return false;
                        }

                        if (t.fecha_expiracion.HasValue)
                        {
                            //if (fechaValida == false)
                            if (t.fecha_expiracion < DateTime.Now)
                            {
                                //MessageBox.Show("La tarjeta ha expirado. Por favor comuniquese con el administrador del local.", "Tarjeta Expirada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                Control.Common.General.GetMensajeToList(562);
                                return false;
                            }
                        }
                        else
                        {
                            //MessageBox.Show("La tarjeta no tiene fecha de expiración. Por favor comuniquese con el administrador del local.", "Tarjeta Expirada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Control.Common.General.GetMensajeToList(563);
                            return false;
                        }
                    }
                    else
                    {
                        //MessageBox.Show("La tarjeta esta desactivada. Comuniquese inmediatamente con el administrador del local.", "Tarjeta Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Control.Common.General.GetMensajeToList(564);
                        return false;
                    }
                }
              
            }
            
            return existe;
        }


        public static pos_customer getCliente(string cedula, Factura _factura, bool debeComprobarTarPortal = true)
        {

            pos_customer cliente = new pos_customer();
            core_tarjetacreditointerno tarjetaInterna = _factura.TarjetaCreditoInterno;
            core_tarjetacreditointerno tarjetaAdicional = _factura.TarjetaCreditoInternoAdicional;

            try
            {
                using (var db = new POSEntities())
                {
                    cliente = db.pos_customer.Where(x => x.VATNUM == cedula || x.ACCOUNTNUM == cedula).FirstOrDefault();

                    if (debeComprobarTarPortal)
                    {
                        _factura.EsEmpleadoLiris = false;

                        if(cliente.CUSTGROUP == "EM" || cliente.CUSTGROUP == "CE")
                        {
                            _factura.EsEmpleadoLiris = true;
                        }


                        //if ((cliente.CUSTGROUP == "EM" || cliente.CUSTGROUP == "CE") && _factura.TarjetaCreditoInterno.codigo == null)  //Me da opcion a usar tarjeta Delportal
                        //{
                        //    Control.Common.General.GetMensajeToList(16);

                        //    _factura.EsEmpleadoLiris = true;
                        //}
                        else if (cliente.CUSTGROUP == "08" && tarjetaAdicional == null)
                        {
                            var objTarjetaAd = ConsultarTarjetaInternaAdicional2Gen(cedula);
                            if (objTarjetaAd != null)
                            {
                                Control.Common.General.GetMensajeToList(56);
                                return null;
                            }
                        }
                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(18);
                        return null;
                    }
                }

                return cliente;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static pos_customer getCliente(string cedula, out core_tarjetacreditointerno tarjetaInterna, out core_tarjetacreditointerno tarjetaAdicional, bool debeComprobarTarPortal = true) 
        {
           
            pos_customer c = null;
            tarjetaInterna = null;
            tarjetaAdicional = null;
            MsgBoxCtrl msgBox;
            DialogResult dialogResult;


            if (clienteExiste(cedula)) 
            { 
                using (var db = new POSEntities())
                {
                    //cedula = "%B09262482121712202445^?";
                    //cedula = "%B1308184413238202135^?";
                    //cedula = "%091307347427112018130214^?";
                    
                    if (cedula.StartsWith("%"))
                    {
                        var tarjeta = System.Text.RegularExpressions.Regex.Match(cedula, @"\%(\w+)\^").Groups[1].Value;

                        if (string.IsNullOrEmpty(tarjeta))
                        {
                            tarjeta = System.Text.RegularExpressions.Regex.Match(cedula, @"\%(\w+)\&").Groups[1].Value;
                        }

                        if (string.IsNullOrEmpty(tarjeta))
                        {
                            tarjeta = System.Text.RegularExpressions.Regex.Match(cedula, @"\%(\w+)\*").Groups[1].Value;
                        }

                        tarjeta = tarjeta.ToUpper().Replace("B", "");

                        //var t = db.core_tarjetacreditointerno.Single(x => x.codigo == tarjeta && x.activo == true);
                        

                        var t = ConsultarTarjetaInternaGen(tarjeta);

                        core_tarjetacreditointerno adi;
                        if (t != null)
                        {
                            if (t.activo)
                            {
                                if (t.identificacionPrincipal == null)
                                {
                                    tarjetaInterna = t;
                                    if (t.identificacion != null)
                                    {
                                        cedula = t.identificacion.Trim();
                                    }
                                }
                                else
                                {
                                    adi = new core_tarjetacreditointerno {
                                        activo = t.activo
                                        , codigo = t.codigo
                                        , core_empresacredito = t.core_empresacredito
                                        , cupo = t.cupo
                                        , empresa_id = t.empresa_id
                                        , fecha_activacion = t.fecha_activacion
                                        , fecha_creacion = t.fecha_creacion
                                        , fecha_desactivacion = t.fecha_desactivacion
                                        , fecha_expiracion = t.fecha_expiracion
                                        , fecha_modificacion = t.fecha_modificacion
                                        , id = t.id
                                        , identificacion = t.identificacion
                                        , identificacionPrincipal = t.identificacionPrincipal
                                        , nombre_tarjeta = t.nombre_tarjeta
                                        , saldo = t.saldo
                                    };

                                    //t = db.core_tarjetacreditointerno.Where(x => x.identificacion == adi.identificacionPrincipal && x.activo == true).OrderByDescending(x => x.fecha_activacion).FirstOrDefault();
                                    t = ConsultarTarjetaInternaAdicionalGen(adi.identificacionPrincipal);

                                    if (t == null)
                                    {
                                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                        parametros.Add(new ParametrosMensajes() { codigo = "[identificacionPrincipal]", valor = adi.identificacionPrincipal });
                                        Control.Common.General.GetMensajeToList(564, parametros);

                                        //MessageBox.Show("La tarjeta empresarial deslizada está configurada como adicional sin embargo no existe una tarjeta principal de empleado con cedula '" + adi.identificacionPrincipal + "'. Comunique a rrhh para que ayude al cliente con la regularización");
                                    }
                                    else
                                    {
                                        tarjetaAdicional = adi;
                                        tarjetaInterna = t;
                                        cedula = t.identificacion.Trim();
                                    }
                                }
                            }
                        }
                    }
                    
                    if (db.pos_customer.Any(x => x.VATNUM == cedula || x.ACCOUNTNUM == cedula))
                    {
                        var lista = db.pos_customer.Where(x => x.VATNUM == cedula || x.ACCOUNTNUM == cedula).ToList();
                        if (lista.Count > 1)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.IpMaquina))
                                {
                                    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                        Properties.Settings.Default.MAILERROR_FROM,
                                        Properties.Settings.Default.MAILERROR_ALIAS,
                                        Properties.Settings.Default.MAILERROR_DESTINO,
                                        Properties.Settings.Default.MAILERROR_CC,
                                        "POS Cliente duplicado",
                                        String.Format("El cliente con identificacion " + cedula + " tiene registros duplicados en pos_customer sea por su campo AccountNum o VatNum, esto no afecta a la facturacion y normal funcionamiento de POS, pero deberia ser regularizado  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4}",
                                                    Control.Common.GlobalParameters.Establecimiento,
                                                    Control.Common.GlobalParameters.PuntoEmision,
                                                    Control.Common.GlobalParameters.IpMaquina,
                                                    Control.Common.GlobalParameters.UsuarioNombre,
                                                    Control.Common.GlobalParameters.Usuario),
                                        false,
                                        String.Empty);

                                    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Cliente", "getCliente", "No se pudo enviar notificacion de cliente duplicado, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                                    }

                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Cliente", "getCliente", "El cliente con identificacion " + cedula + " tiene registros duplicados en pos_customer sea por su campo AccountNum o VatNum, esto no afecta a la facturacion y normal funcionamiento de POS, pero deberia ser regularizado");
                                }
                            }
                            catch { }
                        }

                        c = db.pos_customer.Where(x => x.VATNUM == cedula || x.ACCOUNTNUM == cedula).FirstOrDefault();

                        if (debeComprobarTarPortal)
                        {
                            if ((c.CUSTGROUP == "EM" || c.CUSTGROUP == "CE") && tarjetaInterna == null)  //Me da opcion a usar tarjeta Delportal
                            {
                                //var result = Control.Common.General.GetMensajeToList(16);
                                //if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                                //{
                                //    Control.Common.General.GetMensajeToList(17);
                                //    return null;
                                //}

                                //if (msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Question, "El cliente es un Empleado, Consulte si desea utilizar tarjeta Delportal...", "POS - Empleado Liris") == MsgBoxCtrl.MessageBoxResult.Ok)
                                //{
                                //    msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Por favor deslice la tarjeta por el lector magnétic", "POS - Empleado Liris");
                                //    return null;
                                //}

                                //DialogResult dr = MessageBox.Show("El cliente es un Empleado, Consulte si desea utilizar tarjeta Delportal...", "Empleado Liris", MessageBoxButtons.YesNo);
                                //if (dr == DialogResult.Yes)
                                //{
                                //    MessageBox.Show("Por favor deslice la tarjeta por el lector magnético");
                                //    return null;
                                //}
                            }
                            else if (c.CUSTGROUP == "08" && tarjetaAdicional == null)
                            {
                                //var t = db.core_tarjetacreditointerno.Where(x => x.identificacion == cedula && x.activo == true && x.identificacionPrincipal != null).FirstOrDefault();
                                var t = ConsultarTarjetaInternaAdicional2Gen(cedula);
                                
                                if (t != null)
                                {
                                    var result = Control.Common.General.GetMensajeToList(565);
                                    if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                                    {
                                        Control.Common.General.GetMensajeToList(17);
                                        return null;
                                    }
                                }

                                //if (t != null)
                                //{
                                //    DialogResult dr = MessageBox.Show("El cliente posee una tarjeta adicional de empleado, consulte si desea utilizar tarjeta Delportal...", "Empleado Liris", MessageBoxButtons.YesNo);
                                //    if (dr == DialogResult.Yes)
                                //    {
                                //        //MessageBox.Show("Por favor deslice la tarjeta por el lector magnético");
                                //        Control.Common.General.GetMensajeToList(17);
                                //        return null;
                                //    }
                                //}
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Cliente no existe en AXapta, comuniquese con el Administrador");
                        Control.Common.General.GetMensajeToList(18);
                        return null;
                    }
                    
                    /*c._codigoAX = cliente.CUSTGROUP;
                    c._cedulaRUC = cliente.VATNUM;
                    c._direccion = cliente.ADDRESS;
                    c._telefono = cliente.PHONE;
                    c._custgroup = cliente.
                    */
                }
            }
            return c;
        }


        public static String setEmail(string cedula, string telefono, string correo)
        {
            var cliente = new CreateCustomerLirisProduccion.Cliente();
            cliente.Grabar = false;
            using (var servicio = new CreateCustomerLirisProduccion.Service1Client())
            {
                servicio.CustomerUpdate(cedula, telefono, correo);
                servicio.Close();
            }
            return cedula;
        }

        //public static AXClienteService.Cliente createCliente(string nombre, string cedula,string identificacion, string direccion, string telefono)
        public static CreateCustomerLirisProduccion.Cliente createCliente(string nombre, string cedula, string identificacion, string direccion, string telefono, string correo)
        {
                //var cliente = new AXClienteService.Cliente();
                var cliente = new CreateCustomerLirisProduccion.Cliente();
                cliente.Grabar = false;
                //using (var servicio = new AXClienteService.ServicioClient())
                
            /*using (var servicio = new CreateCustomerLirisProduccion.Service1Client())
                {                   
                    cliente= servicio.CustomerCreate(nombre.ToUpper(), cedula, identificacion.ToUpper(), "08", "VENTAS", direccion.ToUpper(), telefono, "Ecuador", "Guayas", "Guayaquil", "USD", "ES-MX",correo);
                    //servicio.IdentificationValidate(cedula, identificacion.ToUpper());
                    //MessageBox.Show(this,servicio.CustomerCheck(cedula).ToString());                   
                    servicio.Close();
                }*/

            return cliente;
        }


        public static core_tarjetacreditointerno ConsultarTarjetaInternaGen(string codigo)
        {
            core_tarjetacreditointerno TarjetaInternaResult =null;
            bool result = false;
            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                SqlConnection conn = new SqlConnection(cadenaCon);

                try
                {
                    string Query = "Exec dbo.spConsultaTarjetaInternaGen  " + "'" + codigo + "'";

                    conn.Open();
                    SqlCommand select = new SqlCommand(Query, conn);
                    IAsyncResult iar = select.BeginExecuteReader();
                    SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                    while (dr.Read())
                    {
                        TarjetaInternaResult = new core_tarjetacreditointerno();
                        TarjetaInternaResult.id = Int32.Parse(dr.GetValue(0).ToString());
                        TarjetaInternaResult.fecha_creacion = dr.GetDateTime(1);
                        TarjetaInternaResult.fecha_modificacion = dr.GetDateTime(2);
                        TarjetaInternaResult.empresa_id = Int32.Parse(dr.GetValue(3).ToString());
                        TarjetaInternaResult.codigo = dr.GetValue(4).ToString();
                        TarjetaInternaResult.identificacion = dr.GetValue(5).ToString();
                        TarjetaInternaResult.nombre_tarjeta = dr.GetValue(6).ToString();

                        if (!dr.IsDBNull(7))
                            TarjetaInternaResult.fecha_activacion = dr.GetDateTime(7);
                        if (!dr.IsDBNull(8))
                            TarjetaInternaResult.fecha_expiracion = dr.GetDateTime(8);
                        if (!dr.IsDBNull(9))
                            TarjetaInternaResult.fecha_desactivacion = dr.GetDateTime(9);
                        if (!dr.IsDBNull(10))
                            TarjetaInternaResult.cupo = decimal.Parse(dr.GetValue(10).ToString());

                        TarjetaInternaResult.saldo = decimal.Parse(dr.GetValue(11).ToString());
                        TarjetaInternaResult.activo = bool.Parse(dr.GetValue(12).ToString());

                        if (!dr.IsDBNull(13))
                            TarjetaInternaResult.identificacionPrincipal = dr.GetValue(13).ToString();
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/Clientes", "ConsultarTarjetaInternaGen", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/Clientes", "ConsultarTarjetaInternaGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return TarjetaInternaResult;
        }


        public static core_tarjetacreditointerno ConsultarTarjetaInternaAdicionalGen(string identificacionPrincipal)
        {
            core_tarjetacreditointerno TarjetaInternaResult = null;
            bool result = false;
            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                SqlConnection conn = new SqlConnection(cadenaCon);
                try
                {
                    string Query = "Exec dbo.spConsultaTarjetaInternaAdiGen  " + "'" + identificacionPrincipal + "'";

                    conn.Open();
                    SqlCommand select = new SqlCommand(Query, conn);
                    IAsyncResult iar = select.BeginExecuteReader();
                    SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                    while (dr.Read())
                    {
                        TarjetaInternaResult = new core_tarjetacreditointerno();
                        TarjetaInternaResult.id = Int32.Parse(dr.GetValue(0).ToString());
                        TarjetaInternaResult.fecha_creacion = dr.GetDateTime(1);
                        TarjetaInternaResult.fecha_modificacion = dr.GetDateTime(2);
                        TarjetaInternaResult.empresa_id = Int32.Parse(dr.GetValue(3).ToString());
                        TarjetaInternaResult.codigo = dr.GetValue(4).ToString();
                        TarjetaInternaResult.identificacion = dr.GetValue(5).ToString();
                        TarjetaInternaResult.nombre_tarjeta = dr.GetValue(6).ToString();

                        if (!dr.IsDBNull(7))
                            TarjetaInternaResult.fecha_activacion = dr.GetDateTime(7);
                        if (!dr.IsDBNull(8))
                            TarjetaInternaResult.fecha_expiracion = dr.GetDateTime(8);
                        if (!dr.IsDBNull(9))
                            TarjetaInternaResult.fecha_desactivacion = dr.GetDateTime(9);
                        if (!dr.IsDBNull(10))
                            TarjetaInternaResult.cupo = decimal.Parse(dr.GetValue(10).ToString());

                        TarjetaInternaResult.saldo = decimal.Parse(dr.GetValue(11).ToString());
                        TarjetaInternaResult.activo = bool.Parse(dr.GetValue(12).ToString());

                        if (!dr.IsDBNull(13))
                            TarjetaInternaResult.identificacionPrincipal = dr.GetValue(13).ToString();
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/Clientes", "ConsultarTarjetaInternaAdicionalGen", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/Clientes", "ConsultarTarjetaInternaAdicionalGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return TarjetaInternaResult;
        }

        public static core_tarjetacreditointerno ConsultarTarjetaInternaAdicional2Gen(string identificacion)
        {
            core_tarjetacreditointerno TarjetaInternaResult = null;
            bool result = false;
            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                SqlConnection conn = new SqlConnection(cadenaCon);
                try
                {
                    string Query = "Exec dbo.spConsultaTarjetaInternaAdi2Gen  " + "'" + identificacion + "'";

                    conn.Open();
                    SqlCommand select = new SqlCommand(Query, conn);
                    IAsyncResult iar = select.BeginExecuteReader();
                    SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                    while (dr.Read())
                    {
                        TarjetaInternaResult = new core_tarjetacreditointerno();
                        TarjetaInternaResult.id = Int32.Parse(dr.GetValue(0).ToString());
                        TarjetaInternaResult.fecha_creacion = dr.GetDateTime(1);
                        TarjetaInternaResult.fecha_modificacion = dr.GetDateTime(2);
                        TarjetaInternaResult.empresa_id = Int32.Parse(dr.GetValue(3).ToString());
                        TarjetaInternaResult.codigo = dr.GetValue(4).ToString();
                        TarjetaInternaResult.identificacion = dr.GetValue(5).ToString();
                        TarjetaInternaResult.nombre_tarjeta = dr.GetValue(6).ToString();

                        if (!dr.IsDBNull(7))
                            TarjetaInternaResult.fecha_activacion = dr.GetDateTime(7);
                        if (!dr.IsDBNull(8))
                            TarjetaInternaResult.fecha_expiracion = dr.GetDateTime(8);
                        if (!dr.IsDBNull(9))
                            TarjetaInternaResult.fecha_desactivacion = dr.GetDateTime(9);
                        if (!dr.IsDBNull(10))
                            TarjetaInternaResult.cupo = decimal.Parse(dr.GetValue(10).ToString());

                        TarjetaInternaResult.saldo = decimal.Parse(dr.GetValue(11).ToString());
                        TarjetaInternaResult.activo = bool.Parse(dr.GetValue(12).ToString());

                        if (!dr.IsDBNull(13))
                            TarjetaInternaResult.identificacionPrincipal = dr.GetValue(13).ToString();
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/Clientes", "ConsultarTarjetaInternaAdicional2Gen", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/Clientes", "ConsultarTarjetaInternaAdicional2Gen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return TarjetaInternaResult;
        }

    }
}
