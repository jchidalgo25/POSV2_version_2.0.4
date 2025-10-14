using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using System.Windows.Forms;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Data;

namespace POS.Control.WalletPoints
{
    public class ClsAcumulacion
    {
        public static string ProcesaSolicitud(string trama)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "AcumularSQL", "Enviando trama para acumulacion de puntos de factura que si aplica" + Environment.NewLine + trama);
            string response = string.Empty;
            //return response;
            //Sin factura
            //return @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><SaveRechargeResponse xmlns=""http://www.activaecuador.com/cellphone""><SaveRechargeResult><errNumber>0000</errNumber><errDescription>Transacción Correcta</errDescription><Sucess>true</Sucess><strTrame>0210bdaa9f79-ab00-43b4-9fff-30b426cc1d620000Transacción Correcta                    001337000001A137    65   20180502164749000000000004460000000000005400000000000000000000000000000000000000000000000000000500021GRACIAS POR SU COMPRA293521|||                                         840018PYR-5032-A137-149720180502</strTrame><dteDateProcess>2018-05-02T16:48:37.12-05:00</dteDateProcess><PrintFactura/><PrintRecibo/><PrintOtros/><TypePrintDocument>N|</TypePrintDocument><RecActivaCode>PYR-5032-A137-1497</RecActivaCode><RecCltid>bdaa9f79-ab00-43b4-9fff-30b426cc1d62</RecCltid><RecValidation/></SaveRechargeResult></SaveRechargeResponse></soap:Body></soap:Envelope>";
            //Con factura
            //return @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><SaveRechargeResponse xmlns=""http://www.activaecuador.com/cellphone""><SaveRechargeResult><errNumber>0000</errNumber><errDescription>Transacción Correcta</errDescription><Sucess>true</Sucess><strTrame>0210db56b954-ebf5-4e51-ab8b-9bd107ac702b0000Transacción Correcta                    001387000001A137    65   20180503170658000000000001790000000000002100000000000000000000000000000000000000000000000000000200021GRACIAS POR SU COMPRA293621|||                                         840018PYR-5032-A137-154720180503</strTrame><dteDateProcess>2018-05-03T17:07:51.0812208-05:00</dteDateProcess><PrintFactura><![CDATA[<Factura><Cabecera><RazonSocial>TRANSFERUNION S.A. TEST1 </RazonSocial><Empresa>RED ACTIVA WESTERN UNION </Empresa><Direccion1>MATRIZ   : AV. GUILLERMO PAREJA ROLANDO 561 Y ALEJANDRO IDROVO R. CDLA LA GARZOTA MZ 56 EDIF DE BRONCE OFI1-2-3 </Direccion1><RUC>RUC      : 0991286403001 </RUC><Telefono>TELEF    : 1800-937837 </Telefono><Contribuyente>CONTRIBUYENTE  ESPECIAL RESOLUCION  972</Contribuyente><Sucursal>SUCURSAL :LOTIZACIÓN FINCA DE CASA GRANDE AVE. ING. LEÓN FEBRESCORDERO 112 FRENTE A LA PIAZZA DE VILLACLUB</Sucursal><Comisionista>COMISIONISTA: LIRIS S.A.</Comisionista><RUC_Comisionista>RUC         : 0990865477001</RUC_Comisionista><Codigo_Establecimiento>COD. ESTAB. : 008</Codigo_Establecimiento><Direccion_Comisionista>DIRECCION   :LOTIZACIÓN FINCA DE CASA GRANDE AVE. ING. LEÓN FEBRESCORDERO 112 FRENTE A LA PIAZZA DE VILLACLUB</Direccion_Comisionista><NumFactura>Factura No : 131-053-000000005</NumFactura><Cliente>Cliente   : Milton Lindao</Cliente><Documento>CED/RUC   : 0926596578</Documento><DireccionCliente>Direccion : Duran</DireccionCliente><TelefonoCliente>Telef     : N/D</TelefonoCliente><FechaEmision>Fecha Emision : 03/05/2018 : 17:07:49</FechaEmision><ClaveDeAcceso>CLAVE DE ACCESO: </ClaveDeAcceso><Titulo> -   PINES Y RECARGAS - </Titulo><TituloDetalle>-#- -ITEM-                 -P.U- -TOTAL-</TituloDetalle></Cabecera><Detalles><Detalle><Cantidad>1.00</Cantidad><descripcion>CLARO</descripcion><precio>$1.79</precio><Total>$1.79</Total></Detalle></Detalles><Subtotal0>Sub Total   :   1.7857</Subtotal0><Subtotal1>Sub Total  0%  :   0.00</Subtotal1><Subtotal2>Sub Total 12.00%  :   $1.79</Subtotal2><Iva>IVA 12.00%     :   $0.21</Iva><Total>TOTAL       :   $2.00</Total><FormaPago>FORMA DE PAGO                     VALOR</FormaPago><FormaPagoDetalle>EFECTIVO                          $2.00</FormaPagoDetalle><LineaSeparador>____________________________</LineaSeparador><Firmas>Firma del Cliente</Firmas><Agencia>AGENCIA:  SRNP RED DEL PORTAL VILLACLUB DAULE</Agencia><Operador>OPERADOR:1B33</Operador><Ciudad>DAULE</Ciudad><Adquiriente>ORIGINAL - ADQUIRIENTE</Adquiriente><LineaSeparador1>===============================</LineaSeparador1><ResolucionFE>DE ACUERDO CON LA RESOLUCION NAC-DGERCGC14-00790 ESTE COMPROBANTE PODRA SER AUTORIZADO POR EL SRI DENTRO DE LAS PROXIMAS 24 HORAS UNA VEZ AUTORIZADO POR EL SRI USTED PODRA CONSULTAR SU COMPROBANTE EN EL PORTAL WEB: www.redactiva.com </ResolucionFE><Usuario>USUARIO: 0926596578</Usuario><Password>PASSWORD: 0926596578</Password><LineaSeparador2>===============================</LineaSeparador2></Factura>]]></PrintFactura><PrintRecibo/><PrintOtros/><TypePrintDocument>F|</TypePrintDocument><RecActivaCode>PYR-5032-A137-1547</RecActivaCode><RecCltid>db56b954-ebf5-4e51-ab8b-9bd107ac702b</RecCltid><RecValidation/></SaveRechargeResult></SaveRechargeResponse></soap:Body></soap:Envelope>";
            try
            {
                ObjectParameter paramResponse = new ObjectParameter("respuesta", typeof(string));
                using (POSEntities db = new POSEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                    db.Database.CommandTimeout = 0;
                    db.sprCallForTransacSQLAcumularPts(trama,
                                            paramResponse);
                }
                if (!string.IsNullOrEmpty(paramResponse.Value.ToString()))
                {
                    response = (string)paramResponse.Value;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ClsAcumulacion", "ProcesaSolicitud", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = string.Empty;
            }

            return response;
        }


        public static string ProcesaSolicitudGen(string trama)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "ProcesaSolicitudGen", "Enviando trama para acumulacion de puntos de factura que si aplica" + Environment.NewLine + trama);
            string response = string.Empty;
            string cadenaCon = "";
            
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "ProcesaSolicitudGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
                return "";
            }
            
            if (cadenaCon != "")
            {
                SqlConnection conn = new SqlConnection(cadenaCon);

                try
                {

                    SqlParameter paramResponse = new SqlParameter("@respuesta", SqlDbType.VarChar, 1000);
                    //paramResponse.IsNullable = true;
                    paramResponse.Direction = System.Data.ParameterDirection.Output;
                    var addParameters = new List<SqlParameter>
                                 {
                                    new SqlParameter("@xmlRequest", trama),
                                    paramResponse
                                 };

                    SqlCommand select = new SqlCommand("Exec PtsCliente.sprCallForTransacSQLAcumularPtsGen @xmlRequest, @respuesta out", conn);
                    select.Parameters.AddRange(addParameters.ToArray());
                    conn.Open();
                    select.ExecuteNonQuery();
                    conn.Close();

                    if (!string.IsNullOrEmpty(paramResponse.Value.ToString()))
                    {
                        response = (string)paramResponse.Value;
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "Control/WalletPoints/ClsAcumulacion", "ProcesaSolicitudGen", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                    response = string.Empty;
                }
            }
            return response;
        }


        private string GetXmlWithValues(string xmlStr, System.Xml.XmlDocument paramXml)
        {
            string idFactura = paramXml.DocumentElement.HasAttribute("facId") ? paramXml.DocumentElement.GetAttribute("facId") : string.Empty;
            string usaAppMovil = paramXml.DocumentElement.HasAttribute("usaAppMovil") ? paramXml.DocumentElement.GetAttribute("usaAppMovil") : string.Empty;
            //Canal: Si viene desde AppMovil, Servicio Domicilio, etc
            string canal = paramXml.DocumentElement.HasAttribute("canal") ? paramXml.DocumentElement.GetAttribute("canal") : string.Empty;
            string idCampania = paramXml.DocumentElement.HasAttribute("idCampania") ? paramXml.DocumentElement.GetAttribute("idCampania") : string.Empty;

           
            var doc = System.Xml.Linq.XDocument.Parse(xmlStr);

            doc.Root.Element("req").Attribute("idf").Value = idFactura;
            doc.Root.Element("req").Attribute("app").Value = usaAppMovil;
            doc.Root.Element("req").Attribute("can").Value = canal;
            doc.Root.Element("req").Attribute("idc").Value = idCampania;
     

            return doc.ToString();
        }


        private string GetXmlWithValuesGen(string xmlStr, System.Xml.XmlDocument paramXml)
        {
            string idFactura = paramXml.DocumentElement.HasAttribute("facId") ? paramXml.DocumentElement.GetAttribute("facId") : string.Empty;
            string usaAppMovil = paramXml.DocumentElement.HasAttribute("usaAppMovil") ? paramXml.DocumentElement.GetAttribute("usaAppMovil") : string.Empty;
            //Canal: Si viene desde AppMovil, Servicio Domicilio, etc
            string canal = paramXml.DocumentElement.HasAttribute("canal") ? paramXml.DocumentElement.GetAttribute("canal") : string.Empty;
            string idCampania = paramXml.DocumentElement.HasAttribute("idCampania") ? paramXml.DocumentElement.GetAttribute("idCampania") : string.Empty;

            string estable = paramXml.DocumentElement.HasAttribute("est") ? paramXml.DocumentElement.GetAttribute("est") : string.Empty;
            string puntoEmision = paramXml.DocumentElement.HasAttribute("pte") ? paramXml.DocumentElement.GetAttribute("pte") : string.Empty;
            string numero = paramXml.DocumentElement.HasAttribute("num") ? paramXml.DocumentElement.GetAttribute("num") : string.Empty;
            string fecha = paramXml.DocumentElement.HasAttribute("fec") ? paramXml.DocumentElement.GetAttribute("fec") : string.Empty;
            string identificacion = paramXml.DocumentElement.HasAttribute("ide") ? paramXml.DocumentElement.GetAttribute("ide") : string.Empty;
            string usuario = paramXml.DocumentElement.HasAttribute("usu") ? paramXml.DocumentElement.GetAttribute("usu") : string.Empty;
            string valorAplica = paramXml.DocumentElement.HasAttribute("val") ? paramXml.DocumentElement.GetAttribute("val") : string.Empty;
            string pagoTarjetaPortal = paramXml.DocumentElement.HasAttribute("ptp") ? paramXml.DocumentElement.GetAttribute("ptp") : string.Empty;
            string codClienteApp = paramXml.DocumentElement.HasAttribute("alm") ? paramXml.DocumentElement.GetAttribute("alm") : string.Empty;

            var doc = System.Xml.Linq.XDocument.Parse(xmlStr);

            doc.Root.Element("req").Attribute("idf").Value = idFactura;
            doc.Root.Element("req").Attribute("app").Value = usaAppMovil;
            doc.Root.Element("req").Attribute("can").Value = canal;
            doc.Root.Element("req").Attribute("idc").Value = idCampania;
            doc.Root.Element("req").Attribute("est").Value = estable;
            doc.Root.Element("req").Attribute("pte").Value = puntoEmision;
            doc.Root.Element("req").Attribute("num").Value = numero;
            doc.Root.Element("req").Attribute("fec").Value = fecha;
            doc.Root.Element("req").Attribute("ide").Value = identificacion;
            doc.Root.Element("req").Attribute("usu").Value = usuario;
            doc.Root.Element("req").Attribute("val").Value = valorAplica;
            doc.Root.Element("req").Attribute("ptp").Value = pagoTarjetaPortal;
            doc.Root.Element("req").Attribute("alm").Value = codClienteApp;

            return doc.ToString();
        }


        public void AcumularSQL(ref Factura factura, List<Control.WalletPoints.ClsListPoints> listPoints, string clienteApp ="0")
        {
            string mensaje = string.Empty;
            string mensajeFinal = string.Empty;

            //Se acumula puntos por las campañas vigentes.JMM 29 - 08 - 2019
            foreach (var point in listPoints)
            {
                try
                {
                    Control.WalletPoints.ClsPoints.DebeRecargarParametrosEnTiempoReal = point.DebeRecargarParametrosEnTiempoReal;
                    Control.WalletPoints.ClsPoints.EsOpcionPuntosActiva = point.EsOpcionPuntosActiva;
                    Control.WalletPoints.ClsPoints.PlantillaMonederoFactura = point.PlantillaMonederoFactura;
                    Control.WalletPoints.ClsPoints.IdCampania = point.idCampania;
                    Control.WalletPoints.ClsPoints.PtsCliente_XmlAcumulacion = point.PtsCliente_XmlAcumulacion;
                    Control.WalletPoints.ClsPoints.PtsCliente_XmlAcumulacionGen = point.PtsCliente_XmlAcumulacionGen;

                    //if (ClsPoints.DebeRecargarParametrosEnTiempoReal) RecargarParametrosPuntos();

                    if (ClsPoints.EsOpcionPuntosActiva)
                    {
                        var identificacionCliente = string.IsNullOrWhiteSpace(factura.ClienteIdentificacion) ? string.Empty : factura.ClienteIdentificacion.Trim();

                        //Solo clientes de tipo cedula. A este metodo la identificacion ya viene validada
                        if (ValidarIdentificacionCliente(identificacionCliente))
                        {
                            var paramXml = new System.Xml.XmlDocument();
                            paramXml.LoadXml("<root />");
                            paramXml.DocumentElement.SetAttribute("facId", factura.IdFacturaPOS.ToString());
                            paramXml.DocumentElement.SetAttribute("usaAppMovil", factura.AcumulaBilletera ? "1" : "0");
                            paramXml.DocumentElement.SetAttribute("canal", "");
                            paramXml.DocumentElement.SetAttribute("idCampania", ClsPoints.IdCampania.ToString());
                                                        
                            paramXml.DocumentElement.SetAttribute("est", factura.Establecimiento.ToString());
                            paramXml.DocumentElement.SetAttribute("pte", factura.PtoEmision.ToString());
                            paramXml.DocumentElement.SetAttribute("num", factura.Secuencia.ToString());
                            paramXml.DocumentElement.SetAttribute("fec", factura.Fecha.ToString());
                            paramXml.DocumentElement.SetAttribute("ide", factura.ClienteIdentificacion.ToString());
                            paramXml.DocumentElement.SetAttribute("usu", factura.User.username.ToString());

                            paramXml.DocumentElement.SetAttribute("alm", clienteApp);

                            decimal valorAplica = 0.0M;
                            var pagoDE = factura.Pagos.Where(x => x.Descripcion == "DINE ELECT");
                            if (pagoDE.Count() > 0)
                            {
                                 valorAplica = factura.getSubTotal() - pagoDE.ToList().First().Valor;
                            }
                            else {
                                 valorAplica = factura.getSubTotal();
                            }

                            var pagoTP = factura.Pagos.Where(x => x.Descripcion == "TAR PORTAL");
                            if (pagoTP.Count() > 0)
                            {
                                paramXml.DocumentElement.SetAttribute("ptp", "1");
                            }
                            else
                            {
                                paramXml.DocumentElement.SetAttribute("ptp", "0");
                            }


                            paramXml.DocumentElement.SetAttribute("val",valorAplica.ToString());
                            
                            var xmlGen = GetXmlWithValuesGen(Control.WalletPoints.ClsPoints.PtsCliente_XmlAcumulacionGen, paramXml);
                           
                            Control.Common.GlobalParameters.XmlAcumulaPuntos = string.Empty;
                            Control.Common.GlobalParameters.XmlAcumulaPuntos = xmlGen;

                            //14/09/2022 VJFRANCO Acumula en srv-pos
                            string strResponse;
                            strResponse = ProcesaSolicitudGen(xmlGen);

                            if (string.IsNullOrEmpty(strResponse))
                            {
                                var xml = GetXmlWithValues(Control.WalletPoints.ClsPoints.PtsCliente_XmlAcumulacion, paramXml);
                                Control.Common.GlobalParameters.XmlAcumulaPuntos = xml;
                                strResponse = ProcesaSolicitud(xml);
                            }
                           

                            if (!string.IsNullOrEmpty(strResponse))
                            {
                                if (Common.XmlHelper.IsMinimallyValidXml(strResponse))
                                {
                                    System.Xml.Linq.XDocument xDoc = System.Xml.Linq.XDocument.Parse(strResponse);

                                    var responseObj = (from d in xDoc.Descendants("res")
                                                       select new
                                                       {
                                                           pnv = d.Attribute("pnv").Value,
                                                           acu = d.Attribute("acu").Value
                                                       }).FirstOrDefault();

                                    if (!string.IsNullOrEmpty(responseObj.pnv))
                                    {
                                        decimal ptosNuevosTotal = 0M;
                                        decimal monederoSaldo = 0M;

                                        decimal.TryParse(responseObj.pnv, out ptosNuevosTotal);
                                        decimal.TryParse(responseObj.acu, out monederoSaldo);

                                        mensaje = ClsPoints.PlantillaMonederoFactura;

                                        mensaje = mensaje.Replace("<<PTOSNUEVOS>>", ptosNuevosTotal.ToString("N2"));
                                        mensaje = mensaje.Replace("<<PTOSACUMULADOS>>", monederoSaldo.ToString("N2"));

                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "AcumularSQL", "Puntos nuevos: " + ptosNuevosTotal.ToString("N2") + ". Puntos Acumulados: " + monederoSaldo.ToString("N2"));
                                    }
                                    else
                                        mensaje = "";
                                }
                                else
                                {
                                    throw new Exception(strResponse);
                                }
                            }
                            else
                            {
                                throw new Exception("No se acumularon puntos para la factura F-" + factura.Establecimiento + "-" + factura.PtoEmision + "-" + factura.Secuencia.ToString().PadLeft(9, '0') + " del cliente " + factura.ClienteIdentificacion + " pero la opcion de puntos está activa, desactivar el parámetro de acumulación o revisar si es correcto que no lleve puntos");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/WalletPoints/ClsAcumulacion", "AcumularSQL", Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    mensaje = string.Empty;
                }
                mensajeFinal += mensaje;
            }

            factura.Recibo = factura.Recibo.Replace("<<WALLETPOINTS>>", mensajeFinal);
        }

        public void Acumular(ref Factura factura)
        {
            string mensaje = string.Empty;

            decimal ptosNuevosCampania = 0M;
            decimal factorPuntos = 0M;
            decimal ptosNuevosTotal = 0M;
            TblPuntosCab monedero = null;
            try
            {
                if (ClsPoints.DebeRecargarParametrosEnTiempoReal) RecargarParametrosPuntos();

                if (ClsPoints.EsOpcionPuntosActiva)
                {
                    var identificacionCliente = string.IsNullOrWhiteSpace(factura.ClienteIdentificacion) ? string.Empty : factura.ClienteIdentificacion.Trim();                    
                    //Solo clientes de tipo cedula. A este metodo la identificacion ya viene validada
                    if (ValidarIdentificacionCliente(identificacionCliente))
                    {
                        using (POSEntities db = new POSEntities())
                        {
                            var fechaHoy = DateTime.Now;
                            var campanias = db.LstCampania.Where(x => fechaHoy >= x.FechaDesde
                                                                    && fechaHoy < x.FechaHasta
                                                                    && x.Estado == true).ToList();

                            if (campanias.Count > 0)
                            {
                                foreach (Producto producto in factura.Productos)
                                {
                                    producto.FillProductSalesInfo(producto.Id);
                                    producto.FechaCreacion = DateTime.Now;
                                }
                            }

                            foreach (LstCampania campania in campanias)
                            {
                                decimal factorMultiplicador = 0M;
                                factorPuntos = ClsPoints.FactorAcumulacion;

                                //Calcular puntos nuevos
                                ptosNuevosCampania = decimal.Round(factura.getSubTotal() * factorPuntos, 2);
                                
                                //Traer subcampañas para realizar multiplicadores
                                //No permitir que factor multiplicador de puntos este mal definido
                                var subCampanias = campania.LstCampaniaDet.Where(x => fechaHoy >= x.FechaDesde
                                                                                        && fechaHoy < x.FechaHasta
                                                                                        && x.Estado == true
                                                                                        && x.FactorPuntos >= 0).ToList();

                                foreach (LstCampaniaDet subCampania in subCampanias)
                                {
                                    //Si ninguno de los parametros fue configurado no se realiza nada
                                    if (subCampania.Proveedor == null && subCampania.SubGrupo == null && subCampania.Producto == null
                                        && subCampania.FormaPago == null && subCampania.ValorFormaPago == null && subCampania.ValorMinCompra == null)
                                    {
                                        continue;
                                    }

                                    bool cumpleFormaPago = (subCampania.FormaPago == null ? true : (factura.Pagos.Where(x => x.Descripcion == subCampania.FormaPago).Distinct().ToList().Count == 1));
                                    bool cumpleValorFormaPago = (subCampania.ValorFormaPago == null ? true : (cumpleFormaPago && ValidaValorFormaPago(factura, subCampania.ValorFormaPago.ToLower())));
                                    
                                    if (cumpleFormaPago && cumpleValorFormaPago)
                                    {
                                        decimal subtotalAplicante = 

                                                factura.Productos.Where(x => 
                                                    (subCampania.Proveedor == null ? x.ProveedorPricipal == x.ProveedorPricipal : x.ProveedorPricipal == subCampania.Proveedor)
                                                    &&
                                                    (subCampania.SubGrupo == null ? x.SubGrupo == x.SubGrupo : x.SubGrupo == subCampania.SubGrupo)
                                                    &&
                                                    (subCampania.Producto == null ? x.Id == x.Id : x.Id == subCampania.Producto)
                                                )
                                                .Sum(x => x.SubtotalConDescNC);

                                        if (subtotalAplicante > 0)
                                        {
                                            bool cumpleValorMinCompra = (subCampania.ValorMinCompra == null ? true : subtotalAplicante >= subCampania.ValorMinCompra);

                                            if (cumpleValorMinCompra)
                                            {
                                                //Si obtuvo multiplicador, calcular unicamente sobre los productos que aplican
                                                ptosNuevosCampania += decimal.Round((subtotalAplicante * (decimal)subCampania.FactorPuntos) - subtotalAplicante, 2);

                                                //Acumula contador de multiplicadores aplicados por subcampañas para auditoría 
                                                factorMultiplicador += 1;// (decimal)subCampania.FactorPuntos;
                                            }
                                        }
                                    }
                                }

                                if (ptosNuevosCampania > 0)
                                {
                                    //Actualizar o crear monedero de cliente
                                    if (monedero == null)
                                    {
                                        monedero = db.TblPuntosCab.Where(x => x.AccountNum == identificacionCliente && x.Estado == 1).FirstOrDefault();

                                        if (monedero == null)
                                        {
                                            monedero = new TblPuntosCab()
                                            {
                                                AccountNum = factura.ClienteIdentificacion,
                                                Estado = 1,
                                                Puntos = ptosNuevosCampania,
                                                Saldo = ptosNuevosCampania
                                            };
                                            db.TblPuntosCab.Add(monedero);
                                        }
                                        else
                                        {
                                            monedero.Puntos += ptosNuevosCampania;
                                            monedero.Saldo += ptosNuevosCampania;
                                        }
                                    }
                                    else
                                    {
                                        monedero.Puntos += ptosNuevosCampania;
                                        monedero.Saldo += ptosNuevosCampania;
                                    }

                                    monedero.TblPuntos.Add(new TblPuntos()
                                    {
                                        Id_Factura = factura.IdFacturaPOS,
                                        IdLstCampania = campania.IdLstCampania,
                                        FactorPuntos = factorPuntos,
                                        FactorMultiplicador = factorMultiplicador,
                                        Puntos = ptosNuevosCampania,
                                        Saldo = ptosNuevosCampania,
                                        FechaEmision = DateTime.Now,
                                        FechaExpiracion = campania.DiasVigencia == null ? null : (DateTime?)DateTime.Now.AddDays((double)campania.DiasVigencia),
                                        Estado = 1
                                    });

                                    ptosNuevosTotal += ptosNuevosCampania;
                                }
                            }

                            if (ptosNuevosTotal > 0)
                            {
                                db.SaveChanges();

                                mensaje = ClsPoints.PlantillaMonederoFactura;

                                mensaje = mensaje.Replace("<<PTOSNUEVOS>>", ptosNuevosTotal.ToString("N2"));
                                mensaje = mensaje.Replace("<<PTOSACUMULADOS>>", monedero.Saldo.ToString("N2"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "Acumular", Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                mensaje = string.Empty;
            }

            factura.Recibo = factura.Recibo.Replace("<<WALLETPOINTS>>", mensaje);
        }

        private bool ValidaValorFormaPago(Factura factura, string criterioValorFormaPago)
        {
            bool respuesta = true;
            try
            {
                foreach (var p in factura.Pagos)
                {
                    if (p.Pagos.Count > 0)
                    {
                        foreach (var subp in p.Pagos)
                        {
                            if (subp is PagoTarjetaCredito)
                            {
                                var pTarjeta = subp as PagoTarjetaCredito;

                                if (!(pTarjeta.Nombre.ToLower().Contains(criterioValorFormaPago) || pTarjeta.Marca.ToLower().Contains(criterioValorFormaPago) || pTarjeta.Codigo.ToLower().Contains(criterioValorFormaPago) || pTarjeta.BinDescripcion.ToLower().Contains(criterioValorFormaPago)))
                                {
                                    respuesta = false;
                                    break;
                                }
                            }
                            else if (subp is PagoCheque)
                            {
                                var pCheque = subp as PagoCheque;

                                if (!pCheque.Banco.ToLower().Contains(criterioValorFormaPago))
                                {
                                    respuesta = false;
                                    break;
                                }
                            }
                            else if (subp is PagoRetencion)
                            {
                                if (criterioValorFormaPago != ("Retencion").ToLower())
                                {
                                    respuesta = false;
                                    break;
                                }
                            }
                            else if (subp is PagoDescuentoEspanola)
                            {
                                var pagoTj = subp as PagoDescuentoEspanola;

                                if (pagoTj.Codigo.ToLower() != criterioValorFormaPago)
                                {
                                    respuesta = false;
                                    break;
                                }
                            }
                            else if (subp is PagoGiftCard)
                            {
                                var pGC = subp as PagoGiftCard;

                                var t = new Control.TarjetaRegalo();
                                //if (t.getTarjeta(pGC.Codigo))
                                if (t.getTarjetaGen(pGC.Codigo,"",false))
                                {
                                    if (pGC.Codigo.ToLower() != criterioValorFormaPago)
                                    {
                                        respuesta = false;
                                        break;
                                    }
                                }
                            }
                            else if (subp is PagoNotaCredito)
                            {
                                var pGC = subp as PagoNotaCredito;

                                var t = new Control.NotaCredito();
                                if (t.getTarjeta(pGC.Codigo))
                                {
                                    if (pGC.Codigo.ToLower() != criterioValorFormaPago)
                                    {
                                        respuesta = false;
                                        break;
                                    }
                                }
                            }
                            else if (subp is PagoTarjetaInterna)
                            {
                                var pTI = subp as PagoTarjetaInterna;

                                var tInterna = new Control.TarjetaCreditoInterno();
                                //if (tInterna.getTarjeta(pTI.Codigo))
                                if (tInterna.getTarjetaGen(pTI.Codigo)) 
                                {
                                    if (pTI.Codigo.ToLower() != criterioValorFormaPago)
                                    {
                                        respuesta = false;
                                        break;
                                    }
                                }
                            }
                            else if (subp.Codigo.ToLower() != criterioValorFormaPago)
                            {
                                respuesta = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (p.Descripcion.ToLower() != criterioValorFormaPago)
                        {
                            respuesta = false;
                            break;
                        }
                    }

                    //Si ya encontramos un resultado no coincidente entonces salir nomas del bucle
                    //El ValorFormaPagoPuede indica que la factura puede contener unicamente registros que cumplan el criterio 
                    if (respuesta == false) break;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "ValidaValorFormaPago", Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                respuesta = false;
            }

            return respuesta;
        }

        private bool ValidarIdentificacionCliente(string identificacionCliente)
        {
            //return (identificacionCliente != Common.GlobalParameters.IdConsumidorFinal
            //                && (identificacionCliente.Length == 10 ? true : (ValidarIdentificador.ValidarRUCNatural(identificacionCliente) || ValidarIdentificador.ValidarPasaporte(identificacionCliente))));
            return (identificacionCliente != Common.GlobalParameters.IdConsumidorFinal ? true : false);
        }

        private bool ValidaValorFormaPagoNC(List<core_facturapago> pagos, List<core_tarjetacredito_bin> bines, string criterioValorFormaPago)
        {
            bool respuesta = true;

            try
            {
                foreach (var p in pagos)
                {
                    if (!(p.datos.ToLower().Contains(criterioValorFormaPago)))
                    {
                        if (p.tipo_id == "T. CREDITO")
                        {
                            if (bines.Count > 0)
                            {
                                foreach (var b in bines)
                                {
                                    if (!(b.bin_descripcion.ToLower().Contains(criterioValorFormaPago)))
                                    {
                                        respuesta = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                respuesta = false;
                                break;
                            }
                        }
                        else
                        {
                            respuesta = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "ValidaFormaPagoNC", Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                respuesta = false;
            }

            return respuesta;
        }

        public void DebitarPuntosNC(core_notacredito coreNC, Factura facSoporteNC, core_factura coreFactura)
        {
            string mensaje = string.Empty;

            decimal ptosNuevosCampania = 0M;
            decimal factorPuntos = 0M;
            decimal ptosNuevosTotal = 0M;
            decimal ptosAnteriores = 0M;

            try
            {
                //Debitar puntos a una factura no debe requerir de que la opcion de puntos este activa 
                var identificacionCliente = coreFactura.cliente;
                if (ValidarIdentificacionCliente(coreFactura.cliente))
                {
                    using (POSEntities db = new POSEntities())
                    {
                        var fechaFacturaOri = coreFactura.fecha_creacion;
                        var detallesPuntos = db.TblPuntos.Where(x => x.Id_Factura == coreFactura.id && x.Puntos >= 0).ToList();

                        if (detallesPuntos.Count > 0)
                        {
                            var itemToRemove = facSoporteNC.Productos.Where(x => x.EsAjustado == false).ToList();
                            foreach (var item in itemToRemove)
                            {
                                facSoporteNC.Productos.Remove(item);
                            }

                            foreach (Producto producto in facSoporteNC.Productos)
                            {
                                producto.FillProductSalesInfo(producto.Id);
                                producto.FechaCreacion = fechaFacturaOri;
                            }

                            var numeroFactura = "F-" + coreFactura.establecimiento + "-" + coreFactura.punto_emision + "-" + coreFactura.numero.ToString("000000000.##");
                            var vouchersPinpad = db.POS_VOUCHER.Where(x => x.FACTURA == numeroFactura).ToList();

                            var binesTarjeta = new List<core_tarjetacredito_bin>();

                            if (vouchersPinpad.Count > 0)
                            {
                                foreach (var voucher in vouchersPinpad)
                                {
                                    var bin = db.core_tarjetacredito_bin.Where(x => x.bin == voucher.TARJETA.Substring(0, 6)).FirstOrDefault();
                                    binesTarjeta.Add(bin);
                                }
                            }

                            TblPuntosCab monedero = db.TblPuntosCab.Where(x => x.AccountNum == identificacionCliente && x.Estado == 1).FirstOrDefault(); ;
                            ptosAnteriores = monedero.Saldo;

                            foreach (var registroPuntos in detallesPuntos)
                            {
                                decimal factorMultiplicador = 0M;
                                factorPuntos = ClsPoints.FactorAcumulacion;

                                //Calcular puntos nuevos
                                ptosNuevosCampania = decimal.Round(facSoporteNC.getSubTotal() * factorPuntos, 2);

                                //Traer subcampañas para realizar multiplicadores
                                //No permitir que factor multiplicador de puntos este mal definido
                                var subCampanias = db.LstCampaniaDet.Where(x => x.IdLstCampania == registroPuntos.IdLstCampania).ToList();

                                foreach (LstCampaniaDet subCampania in subCampanias)
                                {
                                    //Si ninguno de los parametros fue configurado no se realiza nada
                                    if (subCampania.Proveedor == null && subCampania.SubGrupo == null && subCampania.Producto == null
                                        && subCampania.FormaPago == null && subCampania.ValorFormaPago == null && subCampania.ValorMinCompra == null)
                                    {
                                        continue;
                                    }

                                    bool cumpleFormaPago = (subCampania.FormaPago == null ? true : (coreFactura.core_facturapago.Where(x => x.tipo_id == subCampania.FormaPago).Distinct().ToList().Count == 1));
                                    bool cumpleValorFormaPago = (subCampania.ValorFormaPago == null ? true : (cumpleFormaPago && ValidaValorFormaPagoNC(coreFactura.core_facturapago.ToList(), binesTarjeta, subCampania.ValorFormaPago.ToLower())));

                                    if (cumpleFormaPago && cumpleValorFormaPago)
                                    {
                                        decimal subtotalAplicante =

                                                facSoporteNC.Productos.Where(x =>
                                                    (subCampania.Proveedor == null ? x.ProveedorPricipal == x.ProveedorPricipal : x.ProveedorPricipal == subCampania.Proveedor)
                                                    &&
                                                    (subCampania.SubGrupo == null ? x.SubGrupo == x.SubGrupo : x.SubGrupo == subCampania.SubGrupo)
                                                    &&
                                                    (subCampania.Producto == null ? x.Id == x.Id : x.Id == subCampania.Producto)
                                                )
                                                .Sum(x => x.SubtotalConDescNC);

                                        if (subtotalAplicante > 0)
                                        {
                                            bool cumpleValorMinCompra = (subCampania.ValorMinCompra == null ? true : subtotalAplicante >= subCampania.ValorMinCompra);

                                            if (cumpleValorMinCompra)
                                            {
                                                //Si obtuvo multiplicador, calcular unicamente sobre los productos que aplican
                                                ptosNuevosCampania += decimal.Round((subtotalAplicante * (decimal)subCampania.FactorPuntos) - subtotalAplicante, 2);

                                                //Acumula contador de multiplicadores aplicados por subcampañas para auditoría 
                                                factorMultiplicador += 1;// (decimal)subCampania.FactorPuntos;
                                            }
                                        }
                                    }
                                }

                                if (ptosNuevosCampania > 0)
                                {
                                    //Actualizar monedero de cliente
                                    monedero.Puntos -= ptosNuevosCampania;
                                    monedero.Saldo -= ptosNuevosCampania;

                                    monedero.TblPuntos.Add(new TblPuntos()
                                    {
                                        Id_Factura = coreFactura.id,
                                        IdLstCampania = registroPuntos.IdLstCampania,
                                        FactorPuntos = factorPuntos,
                                        FactorMultiplicador = factorMultiplicador,
                                        Puntos = ptosNuevosCampania * -1,
                                        Saldo = ptosNuevosCampania * -1,
                                        FechaEmision = DateTime.Now,
                                        FechaExpiracion = registroPuntos.FechaExpiracion,
                                        Estado = 1
                                    });

                                    ptosNuevosTotal += ptosNuevosCampania;
                                }
                            }

                            if (ptosNuevosTotal > 0)
                            {
                                db.SaveChanges();

                                mensaje = ClsPoints.PlantillaDebitoMonedero;

                                mensaje = mensaje.Replace("<<NOMBRES>>", coreFactura.razon_social);
                                mensaje = mensaje.Replace("<<IDENTIFICACION>>", monedero.AccountNum);
                                mensaje = mensaje.Replace("<<PUNTOSANTERIORES>>", ptosAnteriores.ToString("N2"));
                                mensaje = mensaje.Replace("<<PUNTOSDEBITADOS>>", ptosNuevosTotal.ToString("N2"));
                                mensaje = mensaje.Replace("<<PTOSACUMULADOS>>", monedero.Saldo.ToString("N2"));

                                //Enviar a imprimir
                                Common.Printer.Imprimir(mensaje, 3, 11);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsAcumulacion", "DebitarPuntosNC", Control.Common.ExceptionHandler.GetExceptionMessages(ex));                
            }
        }
        
        public static void RecargarParametrosPuntos()
        {
            foreach (Form form in Application.OpenForms)
                if (form.GetType().Name == typeof(MainWindow).Name)
                    ((MainWindow)form).RecargarParametrosPuntos();            
        }

        private static void prCambioCadenaConexion(string srvSelect, string srvPedidos)
        {
            try
            {
                string yourConnection = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"].ConnectionString.Replace(srvSelect, srvPedidos);
                var DBCS = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"];
                var writable = typeof(System.Configuration.ConfigurationElement).GetField("_bReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                writable.SetValue(DBCS, false);
                DBCS.ConnectionString = yourConnection;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Factura", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
            }
            catch (Exception ex)
            {
            }
        }

        public List<string> consultarMarca(string id)
        {
            List<string> marcas = new List<string>();
            SqlConnection conn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename='|DataDirectory|DBTag.mdf';Integrated Security=True;User Instance=True");
            // Abre a conexão
            conn.Open();
            try
            {
                SqlCommand select = new SqlCommand("SELECT * from TbMarca where id=@id", conn);
                SqlParameter pID = new SqlParameter("id", id);
                select.Parameters.Add(pID);
                // Lê, linha a linha a tabela
                SqlDataReader dr = select.ExecuteReader();
                while (dr.Read())
                {
                    marcas.Add(dr["id"].ToString());
                    marcas.Add(dr["nome_marca"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex);
            }

            return marcas;
        }


    }
}
