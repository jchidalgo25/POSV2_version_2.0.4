using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using LirisLibCorrBan.Models;
using POS.Models;
using System.Net.Sockets;
using MensajesLibrary;

namespace POS.Control.CorrBan
{

    public static class ClsCorrBan
    {
        public static MsgBoxCtrl msgBoxCtrl = new MsgBoxCtrl();
        public static MsgBoxCtrl.MessageType messageType;
        public static MsgBoxCtrl.MessageBoxResult result;


        #region Propiedades Publicas

        public static List<InstRecaudos> ListInstRecargas { get; set; }
        public static List<InstRecaudos> ListInstRecaudadoras { get; set; }
        public static List<CatalogosRecaudo> ListCatalogosCab { get; set; }
        public static List<CatalogosRecaudoDet> ListCatalogosDet { get; set; }
        public static CatalogosRecaudo CatalogoConfiguraEmp { get; set; }
        public static string PlantillaImprimirRecarga { get; set; }
        public static string PlantillaImprimirRecarga2 { get; set; }
        public static string PlantillaImprimirServiciosBasicos { get; set; }
        public static string PlantillaServiciosBasicosRecargo { get; set; }
        public static string PlantillaServiciosBasicosCabecera { get; set; }
        public static string PlantillaServiciosBasicosMensajeReimpresion { get; set; }
        public static string PlantillaServiciosBasicosCredenciales { get; set; }
        public static string PlantillaXMLDefaultServiciosBasicos { get; set; }
        public static bool EsCorresponsalActivo { get { return _esCorresponsalActivo; } set { _esCorresponsalActivo = value; } }
        public static int[] OpcionesPermitidas { get; set; }
        public static string ServerName { get; set; }

        #endregion

        #region Atributos Privados

        private static bool _esCorresponsalActivo = true;

        #endregion

        #region Metodos Publicos

        public static bool RealizarPing(int idPingMethod)
        {
            try
            {
                var soapDetail = LirisLibCorrBan.Business.SoapCorrBanBO.GetSoapDetail(idPingMethod);

                var doc = System.Xml.Linq.XDocument.Parse(soapDetail.SoapLiris);
                doc.Root.Attribute("est").Value = Control.Common.GlobalParameters.Establecimiento;
                doc.Root.Attribute("pt").Value = Control.Common.GlobalParameters.PuntoEmision;
                doc.Root.Attribute("us").Value = Control.Common.GlobalParameters.Usuario;
                doc.Root.Attribute("ip").Value = Control.Common.GlobalParameters.IpMaquina;
                doc.Root.Element("req").Attribute("tm").Value = idPingMethod.ToString();

                var responseXml = LirisLibCorrBan.Business.ProceduresBO.PingRequest(
                                                                        doc.ToString()
                                                                      );

                if (Common.XmlHelper.IsMinimallyValidXml(responseXml))
                {
                    XDocument xDoc = XDocument.Parse(responseXml);
                    //El xml respuesta viene con namespaces propios de soap, entonces para leer su contenido debemos proveer 
                    //estos namespaces a los queries
                    XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                    XNamespace srr = string.Empty;
                    switch (idPingMethod)
                    {
                        case 12:
                            srr = "http://www.activaecuador.com/cellphone";
                            break;
                        case 14:
                            srr = "http://transferunion.org/";
                            break;
                    }

                    var responseObj = (from d in xDoc.Descendants(soap + "Body").Descendants(srr + "PingResponse")
                                       select new
                                       {
                                           PingResult = d.Element(srr + "PingResult").Value
                                       }).FirstOrDefault();

                    if (responseObj != null)
                    {
                        return true;
                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(334);
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Estimado usuario, el servicio de WU devolvió una respuesta no esperada. Esto puede deberse a un breve mantenimiento, por favor vuélvalo a intentar dentro de un momento", " ");
                        //System.Windows.Forms.MessageBox.Show("Estimado usuario, el servicio de WU devolvió una respuesta no esperada. Esto puede deberse a un breve mantenimiento, por favor vuélvalo a intentar dentro de un momento");
                        return false;
                    }
                }
                else
                {
                    Control.Common.General.GetMensajeToList(335);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Estimado usuario, el servicio de WU se encuentra temporalmente fuera de red. Esto puede deberse a un breve mantenimiento, por favor vuélvalo a intentar dentro de un momento", " ");
                    //System.Windows.Forms.MessageBox.Show("Estimado usuario, el servicio de WU se encuentra temporalmente fuera de red. Esto puede deberse a un breve mantenimiento, por favor vuélvalo a intentar dentro de un momento");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "ClsCorrBan", "RealizarPing", Common.ExceptionHandler.GetExceptionMessages(ex));

                Control.Common.General.GetMensajeToList(336);
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Estimado usuario, no hemos podido verificar si el servicio de WU se encuentra activo por un inconveniente temporal. Esto puede deberse a un breve mantenimiento, por favor vuélvalo a intentar dentro de un momento", " ");
                //System.Windows.Forms.MessageBox.Show("Estimado usuario, no hemos podido verificar si el servicio de WU se encuentra activo por un inconveniente temporal. Esto puede deberse a un breve mantenimiento, por favor vuélvalo a intentar dentro de un momento");
                return false;
            }
        }

        public static bool ValidarAperturaCaja()
        {
            try
            {
                bool Apertura = false;
                string ipAddress = string.Empty;
                string nombrePC = Dns.GetHostName().ToString();
                IPHostEntry ipEntry = Dns.GetHostEntry(nombrePC);
                IPAddress[] addr = ipEntry.AddressList;
                //Verificamos la IP de la PC donde se ejecuta la APP.
                ipAddress = addr.Where(i => !i.IsIPv6LinkLocal && !i.IsIPv6Teredo && i.AddressFamily == AddressFamily.InterNetwork).First().ToString();
                var hoy = DateTime.Now.AddDays(0);
                var result = false;
                using (var db = new POSEntities())
                {
                    if (result = db.BG_Apertura.Any(x => x.IP == ipAddress && x.FECHA == hoy.Date && x.CERRADO == 0 && x.PRE_CIERRE == 0))
                    {
                        Apertura = true;
                    }
                }
                return Apertura;
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "ClsCorrBan", "ValidarAperturaCaja", Common.ExceptionHandler.GetExceptionMessages(ex));
                //System.Windows.Forms.MessageBox.Show("Estimado usuario, La opción no se puede utilizar porque no han aperturado caja");
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Estimado usuario, La opción no se puede utilizar porque no han aperturado caja", "POS - Apertura Caja");
                Control.Common.General.GetMensajeToList(337);
                return false;
            }
        }


        public static void CargarParametrosRedActiva()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var paramIpServerCorrBan = db.core_parametro.Where(x => x.identificador == "REDACTIVA_IPSERVER").FirstOrDefault();

                    if (paramIpServerCorrBan == null)
                    {
                        throw new Exception("Parametro 'REDACTIVA_IPSERVER' no esta configurado en core_parametro, por favor configurar");
                    }

                    if (string.IsNullOrEmpty(paramIpServerCorrBan.parametro2))
                    {
                        throw new Exception("Valor parametro2 de 'REDACTIVA_IPSERVER' esta vacio, por favor configurar");
                    }

                    var credentials = paramIpServerCorrBan.parametro2.Split('|');

                    if (credentials.Count() != 2)
                    {
                        throw new Exception("Valor parametro2 de 'REDACTIVA_IPSERVER' no tiene el formato correcto '{usrSql}|{pswSql}', por favor configurar");
                    }

                    var usrSql = credentials[0];
                    var pswSql = credentials[1];

                    SeleccionaLocal.SaveConnectionString("CorrBanEntities", Properties.Resources.ConectaCorrBan
                                                                                        .Replace("[ipserver]", paramIpServerCorrBan.valor)
                                                                                        .Replace("[usrSql]", usrSql)
                                                                                        .Replace("[pswSql]", pswSql));

                    ServerName = paramIpServerCorrBan.valor;

                    //Almacenar el nombre del server actual con lo que ve EntityFramework en runtime
                    string ServerActual = string.Empty; // SeleccionaLocal.GetServerNameEFConnectionString("CorrBanEntities"); 
                    using (var dbCB = new CorrBanEntities())
                    {
                        var sqlConn = dbCB.Database.Connection; 
                        ServerActual = sqlConn.DataSource;
                    }

                    if (ServerActual != ServerName)
                    {
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " Server para RedActiva ha cambiado. Reinicie POS para cargar las nuevas configuraciones ", "POS - RedActiva ");
                        Control.Common.General.GetMensajeToList(338);

                        //System.Windows.Forms.MessageBox.Show("Server para RedActiva ha cambiado. Reinicie POS para cargar las nuevas configuraciones");
                        Control.Common.GlobalParameters.MustCloseApplication = true;
                        Application.Exit();
                        return;
                    }

                    EsCorresponsalActivo = db.core_parametro.Any(x => x.identificador.Equals("REDACTIVA_" + Control.Common.GlobalParameters.EstablecimientoAxCode) && x.valor.Equals("TRUE"));

                    if (EsCorresponsalActivo)
                    {
                        var opciones = db.core_parametro.Where(x => x.identificador.Equals("REDACTIVA_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault().parametro2;
                        if (opciones != null)
                            OpcionesPermitidas = opciones.Split(';').Select(x => int.Parse(x)).ToArray();
                        else
                            OpcionesPermitidas = null;
                    }
                    else
                        OpcionesPermitidas = null;

                    var reciboRecarga = db.core_recibo.Where(x => x.identificador == "RECARGA").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaImprimirRecarga = reciboRecarga.cuerpo;

                    reciboRecarga = db.core_recibo.Where(x => x.identificador == "RECARGA2").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaImprimirRecarga2 = reciboRecarga.cuerpo;

                    reciboRecarga = db.core_recibo.Where(x => x.identificador == "CB_SERVICIOSBASICOS").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaImprimirServiciosBasicos = reciboRecarga.cuerpo;

                    reciboRecarga = db.core_recibo.Where(x => x.identificador == "CB_SERVICIOSBASICOS_CREDENCIALES").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaServiciosBasicosCredenciales = reciboRecarga.cuerpo;

                    reciboRecarga = db.core_recibo.Where(x => x.identificador == "CB_SERVICIOSBASICOS_RECARGO").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaServiciosBasicosRecargo = reciboRecarga.cuerpo;

                    reciboRecarga = db.core_recibo.Where(x => x.identificador == "CB_SERVICIOSBASICOS_CABFACTURA").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaServiciosBasicosCabecera = reciboRecarga.cuerpo;

                    reciboRecarga = db.core_recibo.Where(x => x.identificador == "CB_SERVICIOSBASICOS_REIMPRESION").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaServiciosBasicosMensajeReimpresion = reciboRecarga.cuerpo;

                    reciboRecarga = db.core_recibo.Where(x => x.identificador == "CB_DEFAULTXMLSERVICIOS").FirstOrDefault();
                    if (reciboRecarga != null) PlantillaXMLDefaultServiciosBasicos = reciboRecarga.cuerpo;
                }

                var tipoEmpresaRecarga = (int)LirisLibCorrBan.Business.CatalogoBO.GetDetCatalogId(
                        Control.Common.Enum.CatalogHeaders.TblTipoCorrBan.ToString("F"),
                        "401");// Control.Common.Enum.CatalogDetails.EmpresaRecarga.ToString("F"));

                ListInstRecargas = LirisLibCorrBan.Business.InstRecaudoBO.GetRecaudosForCombo(tipoEmpresaRecarga);

                var tipoEmpresaRecaudadora = (int)LirisLibCorrBan.Business.CatalogoBO.GetDetCatalogId(
                        Control.Common.Enum.CatalogHeaders.TblTipoCorrBan.ToString("F"),
                        "400");// Control.Common.Enum.CatalogDetails.EmpresaRecaudadora.ToString("F"));

                ListInstRecaudadoras = LirisLibCorrBan.Business.InstRecaudoBO.GetRecaudosForCombo(tipoEmpresaRecaudadora);

                ListCatalogosCab = LirisLibCorrBan.Business.CatalogoBO.GetCatalogosCab();
                ListCatalogosDet = LirisLibCorrBan.Business.CatalogoBO.GetCatalogosDet();
                CatalogoConfiguraEmp = LirisLibCorrBan.Business.CatalogoBO.GetCatalogByName("TblConfiguraEmp" + Common.GlobalParameters.Establecimiento);

            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "ClsCorrBan", "CargarParametrosRedActiva", Common.ExceptionHandler.GetExceptionMessages(ex));
                //System.Windows.Forms.MessageBox.Show("No se pudo iniciar el módulo de Corresponsal Bancario, esto puede deberse a que la red estuvo fuera de servicio brevemente. Reinicie POS para cargar el módulo");
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No se pudo iniciar el módulo de Corresponsal Bancario, esto puede deberse a que la red estuvo fuera de servicio brevemente. Reinicie POS para cargar el módulo", "POS - RedActiva");
                Control.Common.General.GetMensajeToList(174);

                EsCorresponsalActivo = false;
            }
        }

        /// <summary>
        /// Formato xml esperado
        /// DocumentElement
        ///     tblAccounts
        ///         Cuenta
        ///         NombresyApellidos
        ///         Documento
        ///         Institucion
        ///     /tblAccounts
        /// /DocumentElement    
        /// </summary>
        /// <param name="strXmlCorr"></param>
        /// <returns></returns>
        public static List<Models.CorrBan.LikeAccount> TransformarListaCuentasSimilares(string strXmlCorr)
        {
            List<Models.CorrBan.LikeAccount> lista = new List<Models.CorrBan.LikeAccount>();
            try
            {
                var xDoc = System.Xml.Linq.XDocument.Parse(strXmlCorr);
                foreach (var node in xDoc.Root.Elements())
                {
                    lista.Add(new Models.CorrBan.LikeAccount {
                        Account = node.Element("Cuenta").Value,
                        Names = node.Element("NombresyApellidos").Value,
                        Document = node.Element("Documento").Value,
                        Company = node.Element("Institucion").Value
                    });
                }

                lista = lista.OrderBy(x => x.Names).ToList();
            }
            catch (Exception ex)
            {

            }

            return lista;
        }

        public static string TransformarXmlCorrReciboAReciboLiris(string strXmlCorr, System.Xml.XmlDocument paramXml)
        {
            string codCorr = paramXml.DocumentElement.GetAttribute("codCorr");
            decimal valorRecibido = decimal.Parse(paramXml.DocumentElement.GetAttribute("valorRecibido"));
            decimal valorCambio = decimal.Parse(paramXml.DocumentElement.GetAttribute("valorCambio"));
            string recCltid = paramXml.DocumentElement.GetAttribute("RecCltid");
            string fonoCuenta = paramXml.DocumentElement.GetAttribute("fonoCuenta");
            string recActivaCode = paramXml.DocumentElement.GetAttribute("RecActivaCode");
            string autSara = paramXml.DocumentElement.GetAttribute("autSara");
            string secuCarr = paramXml.DocumentElement.GetAttribute("secuCarr");

            string facAbono = paramXml.DocumentElement.HasAttribute("facAbono") ? paramXml.DocumentElement.GetAttribute("facAbono") : string.Empty;
            string facTotalFinal = paramXml.DocumentElement.HasAttribute("facTotalFinal") ? paramXml.DocumentElement.GetAttribute("facTotalFinal") : string.Empty;
            string facConcepto = paramXml.DocumentElement.HasAttribute("concepto") ? paramXml.DocumentElement.GetAttribute("concepto") : string.Empty;
            string facNumAutorizacion = paramXml.DocumentElement.HasAttribute("facAutorizacion") ? paramXml.DocumentElement.GetAttribute("facAutorizacion") : string.Empty;
            string facTerminal = paramXml.DocumentElement.HasAttribute("facTerminal") ? paramXml.DocumentElement.GetAttribute("facTerminal") : string.Empty;
            string facValidacion = paramXml.DocumentElement.HasAttribute("facValidacion") ? paramXml.DocumentElement.GetAttribute("facValidacion") : string.Empty;
            string facTitular = paramXml.DocumentElement.HasAttribute("facTitular") ? paramXml.DocumentElement.GetAttribute("facTitular") : string.Empty;
            string facMensaje = paramXml.DocumentElement.HasAttribute("facMensaje") ? paramXml.DocumentElement.GetAttribute("facMensaje") : string.Empty;

            //Variables para desglose de detalles
            string[] formaPagoDetalle = null;
            System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.None;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[ ]{2,}", options);

            string recibo = string.Empty;
            StringBuilder sb = new StringBuilder();
            int maxStringLineLength = 105;
            int maxStringLineLengthAddress = 42;
            string formatoFechaLarga = "yyyy/MM/dd HH:mm:ss";
            string formatoFechaCorta = "yyyy/MM/dd";

            var xDoc = System.Xml.Linq.XDocument.Parse(strXmlCorr);

            switch (codCorr)
            {
                case "05": case "08":
                    recibo = PlantillaImprimirRecarga2;
                      
                    if(xDoc.Root.Descendants("Detalles").Any())
                    {
                        //Obtener el desglose de detalles de la factura
                        foreach (var node in xDoc.Root.Element("Detalles").Elements())
                        {

                            sb.AppendLine(string.Format("{0}{1}",
                                                            Control.Common.StringHelper.DevolverConPadding(node.Element("descripcion").Value, 50, 8, false),
                                                            Control.Common.StringHelper.DevolverConPadding(node.Element("Total").Value, 12)));
                        }
                        recibo = recibo.Replace("<<DetDesglose>>", sb.ToString());

                        //sb.AppendLine(Common.StringHelper.ToLimitedLength(element.Value, maxStringLineLength));

                        recibo = recibo.Replace("<<CorrRazonSocial>>", Common.StringHelper.ToLimitedLength((xDoc.Root.Element("Cabecera").Element("RazonSocial") == null) ? "" : xDoc.Root.Element("Cabecera").Element("RazonSocial").Value.Replace("\n", ""), maxStringLineLength));
                        recibo = recibo.Replace("<<CorrDireccion>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Direccion1") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Direccion1").Value, 1, ':'));
                        recibo = recibo.Replace("<<CorrRUC>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("RUC") == null) ? "" : xDoc.Root.Element("Cabecera").Element("RUC").Value, 1, ':'));
                        recibo = recibo.Replace("<<ComisEstablecimiento>>", Common.GlobalParameters.EstablecimientoNombre);
                        recibo = recibo.Replace("<<FacRef>>", Common.StringHelper.ToLimitedLength((xDoc.Root.Element("Cabecera").Element("NumFactura") == null) ? "" : xDoc.Root.Element("Cabecera").Element("NumFactura").Value.Replace("\n", ""), 50));
                        recibo = recibo.Replace("<<FacFecha>>", DateTime.Now.ToString(formatoFechaLarga));// Common.StringHelper.GetSplitContent(xDoc.Root.Element("Cabecera").Element("strDateEmision").Value, 1, ':'));
                        recibo = recibo.Replace("<<ClieIdentificacion>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Documento") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Documento").Value, 1, ':'));
                        recibo = recibo.Replace("<<ClieNombre>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Cliente") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Cliente").Value, 1, ':'));
                        recibo = recibo.Replace("<<ClieTelefono>>", "");
                        recibo = recibo.Replace("<<ClieFecha>>", ""); 
                         
                        recibo = recibo.Replace("<<ClieHora>>", "");
                        recibo = recibo.Replace("<<CorrMsj>>", "");
                        recibo = recibo.Replace("<<CorrMsjAdicional>>", "");

                        recibo = recibo.Replace("<<FacPagos>>", string.Format("{0}:{1}", Control.Common.StringHelper.DevolverConPadding("Efectivo", 30, 8, false), Control.Common.StringHelper.DevolverConPadding(valorRecibido.ToString("N2"), 48)));
                        recibo = recibo.Replace("<<FacCambio>>", Control.Common.StringHelper.DevolverConPadding(valorCambio.ToString("N2"), 49));
                        recibo = recibo.Replace("<<CorrRedActiva>>", recActivaCode);
                        recibo = recibo.Replace("<<CorrAutSara>>", autSara);
                        recibo = recibo.Replace("<<CorrActivaId>>", recCltid);
                        recibo = recibo.Replace("<<ClieFonoCta>>", fonoCuenta);
                        recibo = recibo.Replace("<<CorrSecCarr>>", secuCarr);
                    }
                    else
                    {
                        foreach (var node in xDoc.Root.Element("Detalle").Elements())
                        {

                            sb.AppendLine(string.Format("{0}{1}",
                                                            Control.Common.StringHelper.DevolverConPadding(node.Element("strDenominacionTotal").Value, 50, 8, false),
                                                            Control.Common.StringHelper.DevolverConPadding(node.Element("strValueTotal").Value, 12)));
                        }
                        recibo = recibo.Replace("<<DetDesglose>>", sb.ToString());

                        //sb.AppendLine(Common.StringHelper.ToLimitedLength(element.Value, maxStringLineLength));

                        recibo = recibo.Replace("<<CorrRazonSocial>>", Common.StringHelper.ToLimitedLength((xDoc.Root.Element("Cabecera").Element("strRazonSocial") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strRazonSocial").Value.Replace("\n", ""), maxStringLineLength));
                        recibo = recibo.Replace("<<CorrDireccion>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("strAddress") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strAddress").Value, 1, ':'));
                        recibo = recibo.Replace("<<CorrRUC>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("strRUC") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strRUC").Value, 1, ':'));
                        recibo = recibo.Replace("<<ComisEstablecimiento>>", Common.GlobalParameters.EstablecimientoNombre);
                        recibo = recibo.Replace("<<FacRef>>", Common.StringHelper.ToLimitedLength((xDoc.Root.Element("Cabecera").Element("strReferenteInvoice") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strReferenteInvoice").Value.Replace("\n", ""), 50));
                        recibo = recibo.Replace("<<FacFecha>>", DateTime.Now.ToString(formatoFechaLarga));// Common.StringHelper.GetSplitContent(xDoc.Root.Element("Cabecera").Element("strDateEmision").Value, 1, ':'));
                        recibo = recibo.Replace("<<ClieIdentificacion>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("strDocumentCustomer") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strDocumentCustomer").Value, 1, ':'));
                        recibo = recibo.Replace("<<ClieNombre>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("strNameCustomer") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strNameCustomer").Value, 1, ':'));
                        recibo = recibo.Replace("<<ClieTelefono>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("strNumber") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strNumber").Value, 1, ':'));
                        if (xDoc.Root.Element("Cabecera").Element("strDate") == null)
                        {
                            recibo = recibo.Replace("<<ClieFecha>>", "");
                        }
                        else
                        {
                            //Obtener tag de fecha en xml y aplicar split por slash (/)
                            //Formato que Corresponsal nos envìa es dd/MM/yyyy
                            //Debe hacerse de esta forma para evitar conflicto de configuracion regional en las cajas
                            string[] splitFechaCorresponsal = Common.StringHelper.GetSplitContent(xDoc.Root.Element("Cabecera").Element("strDate").Value, 1, ':').Split('/');
                            //Obtener fecha para pasarla a nuestro formato custom de fecha corta
                            DateTime fechaCorresponsal = new DateTime(int.Parse(splitFechaCorresponsal[2]), int.Parse(splitFechaCorresponsal[1]), int.Parse(splitFechaCorresponsal[0]));
                            recibo = recibo.Replace("<<ClieFecha>>", fechaCorresponsal.ToString(formatoFechaCorta));
                        }
                        recibo = recibo.Replace("<<ClieHora>>", (xDoc.Root.Element("Cabecera").Element("strHour") == null) ? "" : xDoc.Root.Element("Cabecera").Element("strHour").Value.Replace("Hora:", "").Trim());
                        recibo = recibo.Replace("<<CorrMsj>>", (xDoc.Root.Element("strMessage") == null) ? "" : xDoc.Root.Element("strMessage").Value);
                        recibo = recibo.Replace("<<CorrMsjAdicional>>", (xDoc.Root.Element("strMessageInformative") == null) ? "" : xDoc.Root.Element("strMessageInformative").Value);

                        recibo = recibo.Replace("<<FacPagos>>", string.Format("{0}:{1}", Control.Common.StringHelper.DevolverConPadding("Efectivo", 30, 8, false), Control.Common.StringHelper.DevolverConPadding(valorRecibido.ToString("N2"), 48)));
                        recibo = recibo.Replace("<<FacCambio>>", Control.Common.StringHelper.DevolverConPadding(valorCambio.ToString("N2"), 49));
                        recibo = recibo.Replace("<<CorrRedActiva>>", recActivaCode);
                        recibo = recibo.Replace("<<CorrAutSara>>", autSara);
                        recibo = recibo.Replace("<<CorrActivaId>>", recCltid);
                        recibo = recibo.Replace("<<ClieFonoCta>>", fonoCuenta);
                        recibo = recibo.Replace("<<CorrSecCarr>>", secuCarr);
                    }
                           
                    return recibo;
                default:

                    if (codCorr == "99")
                    {
                        recibo = PlantillaImprimirServiciosBasicos;

                        decimal valorRecargo = decimal.Parse(Common.StringHelper.GetSplitContent((xDoc.Root.Element("Total") == null) ? "0" : xDoc.Root.Element("Total").Value, 1, ':').Replace("$", ""));
                        if (valorRecargo > 0)
                        {
                            recibo = recibo.Replace("<<CabComprobante>>", PlantillaServiciosBasicosCabecera);
                            recibo = recibo.Replace("<<CorrSeccionRecargo>>", PlantillaServiciosBasicosRecargo);
                        }
                        else
                        {
                            recibo = recibo.Replace("<<CabComprobante>>", "RECAUDACION DE SERVICIOS ACTIVA");
                            recibo = recibo.Replace("<<CorrSeccionRecargo>>", string.Empty);
                        }

                        var tieneCredenciales = (xDoc.Root.Element("Usuario") == null) ? false : true;
                        if (tieneCredenciales)
                        {
                            recibo = recibo.Replace("<<CorrSeccionCredenciales>>", PlantillaServiciosBasicosCredenciales);

                            recibo = recibo.Replace("<<CorrUsuario>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Usuario") == null) ? "" : xDoc.Root.Element("Usuario").Value, 1, ':'));
                            recibo = recibo.Replace("<<CorrPassword>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Password") == null) ? "" : xDoc.Root.Element("Password").Value, 1, ':'));
                            recibo = recibo.Replace("<<CorrMessage1>>", Common.StringHelper.ToLimitedLength((xDoc.Root.Element("Message1") == null) ? "" : xDoc.Root.Element("Message1").Value, maxStringLineLengthAddress));
                            recibo = recibo.Replace("<<CorrMessage2>>", Common.StringHelper.ToLimitedLength((xDoc.Root.Element("Message2") == null) ? "" : xDoc.Root.Element("Message2").Value, maxStringLineLengthAddress));
                        }
                        else
                            recibo = recibo.Replace("<<CorrSeccionCredenciales>>", string.Empty);
                    }
                    else
                        recibo = PlantillaImprimirRecarga;

                    //Obtener el desglose de detalles de la factura
                    foreach (var node in xDoc.Root.Element("Detalles").Elements())
                    {

                        sb.AppendLine(string.Format("{0}   {1}{2}{3}",
                                                       node.Element("Cantidad").Value,
                                                       Control.Common.StringHelper.DevolverConPadding(node.Element("descripcion").Value, 20, 20, false),
                                                       Control.Common.StringHelper.DevolverConPadding(node.Element("precio").Value.Replace("$", ""), 15),
                                                       Control.Common.StringHelper.DevolverConPadding(node.Element("Total").Value.Replace("$", ""), 12)));
                    }
                    recibo = recibo.Replace("<<DetDesglose>>", sb.ToString());

                    //Obtener pago con length controlado
                    formaPagoDetalle = regex.Replace(xDoc.Root.Element("FormaPagoDetalle").Value, " ").Trim().Split();
                    recibo = recibo.Replace("<<FacPagos>>", string.Format("{0}:{1}", Control.Common.StringHelper.DevolverConPadding(Control.Common.StringHelper.ToTitleCase(formaPagoDetalle[0]), 30, 8, false), Control.Common.StringHelper.DevolverConPadding(valorRecibido.ToString("N2"), 48)));
                    recibo = recibo.Replace("<<FacCambio>>", Control.Common.StringHelper.DevolverConPadding(valorCambio.ToString("N2"), 49));

                    //sb.AppendLine(Common.StringHelper.ToLimitedLength(element.Value, maxStringLineLength));

                    recibo = recibo.Replace("<<CorrRazonSocial>>", (xDoc.Root.Element("Cabecera").Element("RazonSocial") == null) ? "" : xDoc.Root.Element("Cabecera").Element("RazonSocial").Value);
                    recibo = recibo.Replace("<<CorrEmpresa>>", (xDoc.Root.Element("Cabecera").Element("Empresa") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Empresa").Value);
                    recibo = recibo.Replace("<<CorrContribuyente>>", (xDoc.Root.Element("Cabecera").Element("Contribuyente") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Contribuyente").Value);
                    recibo = recibo.Replace("<<CorrDireccion>>", Common.StringHelper.ToLimitedLength("Matriz  		: " + Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Direccion1") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Direccion1").Value, 1, ':'), maxStringLineLengthAddress));
                    recibo = recibo.Replace("<<CorrRUC>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("RUC") == null) ? "" : xDoc.Root.Element("Cabecera").Element("RUC").Value, 1, ':'));
                    recibo = recibo.Replace("<<CorrTelefono>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Telefono") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Telefono").Value, 1, ':'));
                    recibo = recibo.Replace("<<CorrSucursal>>", Common.StringHelper.ToLimitedLength("Sucursal		: " + Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Sucursal") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Sucursal").Value, 1, ':'), maxStringLineLengthAddress));
                    recibo = recibo.Replace("<<ComisNombre>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Comisionista") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Comisionista").Value, 1, ':'));
                    recibo = recibo.Replace("<<ComisRUC>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("RUC_Comisionista") == null) ? "" : xDoc.Root.Element("Cabecera").Element("RUC_Comisionista").Value, 1, ':'));
                    recibo = recibo.Replace("<<ComisEstablecimiento>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Codigo_Establecimiento") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Codigo_Establecimiento").Value, 1, ':'));
                    recibo = recibo.Replace("<<ComisEstableciNombre>>", Common.GlobalParameters.EstablecimientoNombre);
                    recibo = recibo.Replace("<<ComisDireccion>>", Common.StringHelper.ToLimitedLength("Dirección		: " + Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Direccion_Comisionista") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Direccion_Comisionista").Value, 1, ':'), maxStringLineLengthAddress));
                    recibo = recibo.Replace("<<FacNumero>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("NumFactura") == null) ? "" : xDoc.Root.Element("Cabecera").Element("NumFactura").Value, 1, ':'));
                    recibo = recibo.Replace("<<ClieNombre>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Cliente") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Cliente").Value, 1, ':'));
                    recibo = recibo.Replace("<<ClieIdentificacion>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Documento") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Documento").Value, 1, ':'));
                    recibo = recibo.Replace("<<ClieDireccion>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("DireccionCliente") == null) ? "" : xDoc.Root.Element("Cabecera").Element("DireccionCliente").Value, 1, ':'));
                    recibo = recibo.Replace("<<ClieTelefono>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("TelefonoCliente") == null) ? "" : xDoc.Root.Element("Cabecera").Element("TelefonoCliente").Value, 1, ':'));
                    recibo = recibo.Replace("<<FacFecha>>", DateTime.Now.ToString(formatoFechaLarga));// Common.StringHelper.GetSplitContent(xDoc.Root.Element("Cabecera").Element("FechaEmision").Value, 1, ':'));
                    recibo = recibo.Replace("<<FacAcceso>>", Common.StringHelper.ToLimitedLength("Clave Acceso		: " + Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("ClaveDeAcceso") == null) ? "" : xDoc.Root.Element("Cabecera").Element("ClaveDeAcceso").Value, 1, ':'), maxStringLineLengthAddress));
                    recibo = recibo.Replace("<<DetTitulo>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Cabecera").Element("Titulo") == null) ? "" : xDoc.Root.Element("Cabecera").Element("Titulo").Value, 1, ':'));

                    recibo = recibo.Replace("<<FacBbase0>>", Control.Common.StringHelper.DevolverConPadding(Common.StringHelper.GetSplitContent((xDoc.Root.Element("Subtotal1") == null) ? "" : xDoc.Root.Element("Subtotal1").Value, 1, ':').Replace("$", ""), 49));
                    recibo = recibo.Replace("<<FacBase12>>", Control.Common.StringHelper.DevolverConPadding(Common.StringHelper.GetSplitContent((xDoc.Root.Element("Subtotal2") == null) ? "" : xDoc.Root.Element("Subtotal2").Value, 1, ':').Replace("$", ""), 49));
                    recibo = recibo.Replace("<<FacSubtotal>>", Control.Common.StringHelper.DevolverConPadding(decimal.Parse(Common.StringHelper.GetSplitContent((xDoc.Root.Element("Subtotal0") == null) ? "" : xDoc.Root.Element("Subtotal0").Value, 1, ':').Replace("$", "")).ToString("N2"), 49));
                    recibo = recibo.Replace("<<FacIva>>", Control.Common.StringHelper.DevolverConPadding(Common.StringHelper.GetSplitContent((xDoc.Root.Element("Iva") == null) ? "" : xDoc.Root.Element("Iva").Value, 1, ':').Replace("$", ""), 49));
                    recibo = recibo.Replace("<<FacTotal>>", Control.Common.StringHelper.DevolverConPadding(Common.StringHelper.GetSplitContent((xDoc.Root.Element("Total") == null) ? "" : xDoc.Root.Element("Total").Value, 1, ':').Replace("$", ""), 33));

                    recibo = recibo.Replace("<<ComisAgencia>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Agencia") == null) ? "" : xDoc.Root.Element("Agencia").Value, 1, ':'));
                    recibo = recibo.Replace("<<ComisOperador>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Operador") == null) ? "" : xDoc.Root.Element("Operador").Value, 1, ':'));
                    recibo = recibo.Replace("<<ComisCiudad>>", Common.StringHelper.GetSplitContent((xDoc.Root.Element("Ciudad") == null) ? "" : xDoc.Root.Element("Ciudad").Value, 1, ':'));
                    recibo = recibo.Replace("<<cajero>>", Control.Common.GlobalParameters.UsuarioNombre);
                    recibo = recibo.Replace("<<CorrRedActiva>>", recActivaCode);
                    recibo = recibo.Replace("<<CorrAutSara>>", autSara);
                    recibo = recibo.Replace("<<CorrActivaId>>", Common.StringHelper.ToLimitedLength("Activa Id		: " + recCltid, maxStringLineLengthAddress));
                    recibo = recibo.Replace("<<ClieFonoCta>>", fonoCuenta);
                    recibo = recibo.Replace("<<CorrSecCarr>>", secuCarr);

                    recibo = recibo.Replace("<<FacRecargo>>", Control.Common.StringHelper.DevolverConPadding(Common.StringHelper.GetSplitContent((xDoc.Root.Element("Total") == null) ? "" : xDoc.Root.Element("Total").Value, 1, ':').Replace("$", ""), 49));
                    recibo = recibo.Replace("<<FacConcepto>>", facConcepto);
                    recibo = recibo.Replace("<<FacRecaudacion>>", Control.Common.StringHelper.DevolverConPadding(facAbono, 49));
                    recibo = recibo.Replace("<<FacRecaudacion2>>", Control.Common.StringHelper.DevolverConPadding(facAbono, 33)); 
                    recibo = recibo.Replace("<<FacTotalFinal>>", Control.Common.StringHelper.DevolverConPadding(decimal.Parse(string.IsNullOrWhiteSpace(facTotalFinal) ? "0" : facTotalFinal).ToString("N2"), 33));
                    recibo = recibo.Replace("<<CorrNumTransaccion>>", facNumAutorizacion);
                    recibo = recibo.Replace("<<CorrValidacion>>", facValidacion);
                    recibo = recibo.Replace("<<ComisTerminal>>", facTerminal);
                    recibo = recibo.Replace("<<FacTitular>>", facTitular);
                    recibo = recibo.Replace("<<ComisMensaje>>", facMensaje);

                    return recibo;
            }
        }

        #endregion

    }
}
