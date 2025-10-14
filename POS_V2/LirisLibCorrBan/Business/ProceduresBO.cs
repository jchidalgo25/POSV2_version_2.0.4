using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LirisLibCorrBan.Models;
using System.Data.Entity.Core.Objects;

namespace LirisLibCorrBan.Business
{
    public static class ProceduresBO
    {
        public static string ProcesaSolicitud(string trama)
        {
            string response = string.Empty;
            //return response;
            //Sin factura
            //return @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><SaveRechargeResponse xmlns=""http://www.activaecuador.com/cellphone""><SaveRechargeResult><errNumber>0000</errNumber><errDescription>Transacción Correcta</errDescription><Sucess>true</Sucess><strTrame>0210bdaa9f79-ab00-43b4-9fff-30b426cc1d620000Transacción Correcta                    001337000001A137    65   20180502164749000000000004460000000000005400000000000000000000000000000000000000000000000000000500021GRACIAS POR SU COMPRA293521|||                                         840018PYR-5032-A137-149720180502</strTrame><dteDateProcess>2018-05-02T16:48:37.12-05:00</dteDateProcess><PrintFactura/><PrintRecibo/><PrintOtros/><TypePrintDocument>N|</TypePrintDocument><RecActivaCode>PYR-5032-A137-1497</RecActivaCode><RecCltid>bdaa9f79-ab00-43b4-9fff-30b426cc1d62</RecCltid><RecValidation/></SaveRechargeResult></SaveRechargeResponse></soap:Body></soap:Envelope>";
            //Con factura
            //return @"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><soap:Body><SaveRechargeResponse xmlns=""http://www.activaecuador.com/cellphone""><SaveRechargeResult><errNumber>0000</errNumber><errDescription>Transacción Correcta</errDescription><Sucess>true</Sucess><strTrame>0210db56b954-ebf5-4e51-ab8b-9bd107ac702b0000Transacción Correcta                    001387000001A137    65   20180503170658000000000001790000000000002100000000000000000000000000000000000000000000000000000200021GRACIAS POR SU COMPRA293621|||                                         840018PYR-5032-A137-154720180503</strTrame><dteDateProcess>2018-05-03T17:07:51.0812208-05:00</dteDateProcess><PrintFactura><![CDATA[<Factura><Cabecera><RazonSocial>TRANSFERUNION S.A. TEST1 </RazonSocial><Empresa>RED ACTIVA WESTERN UNION </Empresa><Direccion1>MATRIZ   : AV. GUILLERMO PAREJA ROLANDO 561 Y ALEJANDRO IDROVO R. CDLA LA GARZOTA MZ 56 EDIF DE BRONCE OFI1-2-3 </Direccion1><RUC>RUC      : 0991286403001 </RUC><Telefono>TELEF    : 1800-937837 </Telefono><Contribuyente>CONTRIBUYENTE  ESPECIAL RESOLUCION  972</Contribuyente><Sucursal>SUCURSAL :LOTIZACIÓN FINCA DE CASA GRANDE AVE. ING. LEÓN FEBRESCORDERO 112 FRENTE A LA PIAZZA DE VILLACLUB</Sucursal><Comisionista>COMISIONISTA: LIRIS S.A.</Comisionista><RUC_Comisionista>RUC         : 0990865477001</RUC_Comisionista><Codigo_Establecimiento>COD. ESTAB. : 008</Codigo_Establecimiento><Direccion_Comisionista>DIRECCION   :LOTIZACIÓN FINCA DE CASA GRANDE AVE. ING. LEÓN FEBRESCORDERO 112 FRENTE A LA PIAZZA DE VILLACLUB</Direccion_Comisionista><NumFactura>Factura No : 131-053-000000005</NumFactura><Cliente>Cliente   : Milton Lindao</Cliente><Documento>CED/RUC   : 0926596578</Documento><DireccionCliente>Direccion : Duran</DireccionCliente><TelefonoCliente>Telef     : N/D</TelefonoCliente><FechaEmision>Fecha Emision : 03/05/2018 : 17:07:49</FechaEmision><ClaveDeAcceso>CLAVE DE ACCESO: </ClaveDeAcceso><Titulo> -   PINES Y RECARGAS - </Titulo><TituloDetalle>-#- -ITEM-                 -P.U- -TOTAL-</TituloDetalle></Cabecera><Detalles><Detalle><Cantidad>1.00</Cantidad><descripcion>CLARO</descripcion><precio>$1.79</precio><Total>$1.79</Total></Detalle></Detalles><Subtotal0>Sub Total   :   1.7857</Subtotal0><Subtotal1>Sub Total  0%  :   0.00</Subtotal1><Subtotal2>Sub Total 12.00%  :   $1.79</Subtotal2><Iva>IVA 12.00%     :   $0.21</Iva><Total>TOTAL       :   $2.00</Total><FormaPago>FORMA DE PAGO                     VALOR</FormaPago><FormaPagoDetalle>EFECTIVO                          $2.00</FormaPagoDetalle><LineaSeparador>____________________________</LineaSeparador><Firmas>Firma del Cliente</Firmas><Agencia>AGENCIA:  SRNP RED DEL PORTAL VILLACLUB DAULE</Agencia><Operador>OPERADOR:1B33</Operador><Ciudad>DAULE</Ciudad><Adquiriente>ORIGINAL - ADQUIRIENTE</Adquiriente><LineaSeparador1>===============================</LineaSeparador1><ResolucionFE>DE ACUERDO CON LA RESOLUCION NAC-DGERCGC14-00790 ESTE COMPROBANTE PODRA SER AUTORIZADO POR EL SRI DENTRO DE LAS PROXIMAS 24 HORAS UNA VEZ AUTORIZADO POR EL SRI USTED PODRA CONSULTAR SU COMPROBANTE EN EL PORTAL WEB: www.redactiva.com </ResolucionFE><Usuario>USUARIO: 0926596578</Usuario><Password>PASSWORD: 0926596578</Password><LineaSeparador2>===============================</LineaSeparador2></Factura>]]></PrintFactura><PrintRecibo/><PrintOtros/><TypePrintDocument>F|</TypePrintDocument><RecActivaCode>PYR-5032-A137-1547</RecActivaCode><RecCltid>db56b954-ebf5-4e51-ab8b-9bd107ac702b</RecCltid><RecValidation/></SaveRechargeResult></SaveRechargeResponse></soap:Body></soap:Envelope>";
            try
            {
                ObjectParameter paramResponse = new ObjectParameter("respuesta", typeof(string));
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                    db.Database.CommandTimeout = 0;
                    db.spProcesaSolicitud(trama,
                                            paramResponse);
                }

                response = (string)paramResponse.Value;
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ProceduresBO", "ProcesaSolicitud", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = string.Empty;
            }

            return response;
        }

        public static string PingRequest(string trama)
        {
            string response = string.Empty;
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                    db.Database.CommandTimeout = 0;
                    response = db.spPingRequest(trama).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ProceduresBO", "PingRequest", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = string.Empty;
            }

            return response;
        }

        public static string ProcesaRecaudacion(string trama)
        {
            string response = string.Empty;
            try
            {
                ObjectParameter paramResponse = new ObjectParameter("respuesta", typeof(string));
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                    db.Database.CommandTimeout = 0;
                    db.spProcesaRecaudacion(trama,
                                            paramResponse);
                }

                response = (string)paramResponse.Value;
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "ProceduresBO", "ProcesaRecaudacion", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = string.Empty;
            }

            return response;
        }
    }
}
