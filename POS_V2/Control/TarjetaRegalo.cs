using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using POS.Models;

namespace POS.Control
{
    public class TarjetaRegalo
    {
        public static string ACTIVACION = "ACTIVACION";
        public static string ANULACION = "ANULACION";
        public static string CONSUMO = "CONSUMO";

        core_giftcard _tarjeta;
        Tbl_DineroGiftCardApp _tarjetaGift;

        public bool getTarjeta(string codigo)
        {
            var result = false;
            
            using (var db=new POSEntities())
            {
                result = db.core_giftcard.Any(x=> x.codigo == codigo);

                if (result)
                {
                    _tarjeta = db.core_giftcard.Single(x => x.codigo == codigo);
                }
               // if (db.pos_itembarra.Any(x => x.ITEMBARCODE == codigo))
                //{
                    //var r = db.pos_itembarra.First(y => y.ITEMBARCODE == codigo);
                    //_tarjeta = db.core_giftcard.Single(x => x.codigo == r.ITEMID);
                //}
            }
            return result;
        }

      

        public bool getTarjetaGiftCard(string codigo, string cliente)
        {
            var result = false;
            var db2 = new POSEntities();
            try {               

                result = db2.Tbl_DineroGiftCardApp.Any(x => x.IdCliente == cliente && x.IdGiftCard == codigo);
                if (result)
                {
                    _tarjetaGift = db2.Tbl_DineroGiftCardApp.FirstOrDefault(x => x.IdCliente == cliente && x.IdGiftCard == codigo);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                db2.Dispose();
                GC.Collect();
            }           
            return result;
        }

     
        /// <summary>
        /// Consulta en SRV-POS el saldo total del cliente dueño de la tarjeta  
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public string getSaldoTotalTarjetaGiftCardGen(string tarjeta, string cliente, bool esAppMovil)
        {
            string result = string.Empty;
            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                SqlConnection conn = new SqlConnection(cadenaCon);
                try
                {
                    SqlParameter paramResult = new SqlParameter("@saldo", SqlDbType.VarChar, -1);
                    paramResult.Direction = System.Data.ParameterDirection.Output;

                    var addParameters = new List<SqlParameter>
                     {
                      new SqlParameter("@tarjeta", tarjeta),
                      new SqlParameter("@cliente", cliente),
                      new SqlParameter("@esAppMovil", esAppMovil.ToString()),
                      paramResult
                     };

                    SqlCommand select = new SqlCommand("Exec PtsCliente.spImprimeSaldoGiftCardGen @tarjeta, @cliente, @esAppMovil, @saldo out", conn);
                   
                        select.Parameters.AddRange(addParameters.ToArray());
                        conn.Open();
                        select.ExecuteNonQuery();

                        conn.Close();

                    result  = (string)paramResult.Value;

                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaRegalo", "getSaldoTotalTarjetaGiftCard", "No se pudo consultar el saldo total durante la ejecución del método, a continuacion el detalle de la excepcion - " + ex.Message );
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaRegalo", "getSaldoTotalTarjetaGiftCard", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return result;
        }

        public string getCodigo()
        {
            if (this._tarjeta == null)
            {
                return "";
            }
            else
            {
                return this._tarjeta.codigo;
            }
        }
        public string getCodigoGiftCard()
        {
            if (this._tarjetaGift == null)
            {
                return "";
            }
            else
            {
                return this._tarjetaGift.IdGiftCard;
            }
        }
        public bool tarjetaNoActivada()
        {
            if (this._tarjeta == null)
            {
                return true;
            }

            if (this._tarjeta.fecha_activacion.HasValue)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }
        public bool tarjetaValida()
        {
            if (this._tarjeta == null)
            {
                return false;
            }
            else
            {
               return this._tarjeta.activo;
            }          
        }
        public bool tarjetaValidaGiftCard()
        {
            if (this._tarjetaGift == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public decimal getSaldo()
        {
            if (_tarjeta != null)
            {
                return _tarjeta.saldo;
            }
            return 0M;
        }

        public decimal getSaldoGiftCard()
        {
            if (_tarjetaGift != null)
            {
                return _tarjetaGift.Saldo;
            }
            return 0M;
        }

        public bool TieneSaldoCuadrado(long secuenciaFactura)
        {
            bool respuesta = true;
            string destinoMail = "devteam@liris.com.ec";
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();


            try
            {
                if (_tarjeta != null)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "Se verificará que la Giftcard " + _tarjeta.codigo + " (" + (_tarjeta.tipo == 1 ? "VENTA" : "NO VENTA") + ") no tenga un saldo superior a sus recargas o su valor de referencia.");
                    if (_tarjeta.tipo == 1 && _tarjeta.valor == null && _tarjeta.saldo > 0)
                    {
                        respuesta = false;

                        string msj = String.Format("En la siguiente caja se está intentando pasar una GiftCard de VENTA (" + _tarjeta.codigo + ") que tiene NULO su valor de referencia el cual debe siempre estar lleno para este tipo de tarjetas, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencial: {2} \nIpMaquina: {3} \nIpServidor: {4} \nCajeroNombre: {5} \nCajeroIdentificacion: {6}",
                                    Control.Common.GlobalParameters.Establecimiento,
                                    Control.Common.GlobalParameters.PuntoEmision,
                                    secuenciaFactura.ToString(),
                                    Control.Common.GlobalParameters.IpMaquina,
                                    Control.Common.GlobalParameters.SelectedServerIp,
                                    Control.Common.GlobalParameters.UsuarioNombre,
                                    Control.Common.GlobalParameters.Usuario);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", msj);

                        var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        destinoMail,
                        Properties.Settings.Default.MAILERROR_CC,
                        "POS - Giftcard Sospechosa Detectada",
                        msj,
                        false,
                        String.Empty);

                        if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de alerta durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                        }

                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[numgiftcard]", valor = _tarjeta.codigo });
                        Control.Common.General.GetMensajeToList(566, parametros);

                        //Control.Common.WinForm.ShowMessage("La giftcard '" + _tarjeta.codigo + "' tiene valores alterados, por lo que no puede ser usada para pagar esta factura. Contacte a administrador ahora!");

                        return respuesta;
                    }
                    if (_tarjeta.tipo == 1 && _tarjeta.saldo > 0)
                    {
                        decimal totalRecargado = 0M;

                        //using (POSEntities db = new POSEntities())
                        //{
                        //    totalRecargado = db.TblVentaTarjeta.Where(x => x.CodigoGiftcard == _tarjeta.codigo).Sum(x => (decimal?)x.Total) ?? 0M;
                        //}
                        //CONSULTA A SRV - POS
                        SqlParameter paramResult = new SqlParameter("@saldo", SqlDbType.Float, -1);
                        paramResult.Direction = System.Data.ParameterDirection.Output;
                        var addParameters = new List<SqlParameter>
                        { 
                        new SqlParameter("@tarjeta", _tarjeta.codigo),
                         paramResult
                        };
                        
                        if (Control.Common.GlobalParameters.ConServerPuntos != "")
                        {
                            SqlConnection conn = new SqlConnection(Control.Common.GlobalParameters.ConServerPuntos);
                            try
                            {

                                SqlCommand select = new SqlCommand("Exec PtsCliente.spSaldoGiftCardVTGen @tarjeta, @saldo out", conn);
                                select.Parameters.AddRange(addParameters.ToArray());
                                conn.Open();
                                select.ExecuteNonQuery();
                                conn.Close();
                                var valor = paramResult.Value;
                                totalRecargado = decimal.Parse(valor.ToString());

                            }
                            catch (Exception ex)
                            {
                                conn.Close();
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaRegalo", "TieneSaldoCuadrado | Exec PtsCliente.spSaldoGiftCardVTGen", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                                return false;
                            }

                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaRegalo", "TieneSaldoCuadrado", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío");
                            return false;
                        }




                        if (_tarjeta.saldo > totalRecargado)
                        {
                            respuesta = false;

                            string msj = String.Format("En la siguiente caja se está intentando pasar una GiftCard de VENTA (" + _tarjeta.codigo + "; Saldo Actual: " + _tarjeta.saldo.ToString("N2") + "; Real Recargas: " + totalRecargado.ToString("N2") + ") con un saldo superior al total de recargas sobre la misma, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencial: {2} \nIpMaquina: {3} \nIpServidor: {4} \nCajeroNombre: {5} \nCajeroIdentificacion: {6}",
                                        Control.Common.GlobalParameters.Establecimiento,
                                        Control.Common.GlobalParameters.PuntoEmision,
                                        secuenciaFactura.ToString(),
                                        Control.Common.GlobalParameters.IpMaquina,
                                        Control.Common.GlobalParameters.SelectedServerIp,
                                        Control.Common.GlobalParameters.UsuarioNombre,
                                        Control.Common.GlobalParameters.Usuario);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", msj);

                            var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                            Properties.Settings.Default.MAILERROR_FROM,
                            Properties.Settings.Default.MAILERROR_ALIAS,
                            destinoMail,
                            Properties.Settings.Default.MAILERROR_CC,
                            "POS - Giftcard Sospechosa Detectada",
                            msj,
                            false,
                            String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de alerta durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }


                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[numgiftcard]", valor = _tarjeta.codigo });
                            Control.Common.General.GetMensajeToList(567, parametros);

                            //Control.Common.WinForm.ShowMessage("La giftcard '" + _tarjeta.codigo + "' tiene más saldo que lo que ha recargado por lo que no puede ser usada para pagar esta factura. Contacte a administrador ahora!");

                            return respuesta;
                        }
                    }
                    if (_tarjeta.valor != null && _tarjeta.saldo > 0)
                    {
                        decimal totalConsumido = 0M;
                        //CONSULTA A SRV - POS
                        //using (POSEntities db = new POSEntities())
                        //{
                        //    totalConsumido = db.core_facturapago.Where(x => x.datos == _tarjeta.codigo && x.tipo_id.Contains("GIFT CARD")).Sum(x => (decimal?)x.valor) ?? 0M;
                        //}

                        SqlParameter paramResult = new SqlParameter("@saldo", SqlDbType.Float  , -1);
                        paramResult.Direction = System.Data.ParameterDirection.Output;
                        var addParameters = new List<SqlParameter>
                        {
                        new SqlParameter("@tarjeta", _tarjeta.codigo),
                         paramResult
                        };

                        if (Control.Common.GlobalParameters.ConServerPuntos != "")
                        {
                            SqlConnection conn = new SqlConnection(Control.Common.GlobalParameters.ConServerPuntos);
                            try
                            {
                                SqlCommand select = new SqlCommand("Exec PtsCliente.spSaldoGiftCardTranGen @tarjeta, @saldo out", conn);
                                select.Parameters.AddRange(addParameters.ToArray());
                                conn.Open();
                                select.ExecuteNonQuery();
                                conn.Close();
                                var valor= paramResult.Value;
                                totalConsumido = decimal.Parse(valor.ToString());


                            }
                            catch (Exception ex)
                            {
                                conn.Close();
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaRegalo", "TieneSaldoCuadrado | Exec PtsCliente.spSaldoGiftCardTranGen", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                                return false;
                            }

                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaRegalo", "TieneSaldoCuadrado", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío");
                            return false;
                        }

                        if ((totalConsumido + _tarjeta.saldo) > _tarjeta.valor)
                        {
                            respuesta = false;

                            string msj = String.Format("En la siguiente caja se está intentando pasar una GiftCard (" + _tarjeta.codigo + "; Saldo Actual: " + _tarjeta.saldo.ToString("N2") + "; Historico Consumos: " + totalConsumido.ToString("N2") + "; Valor Referencia: " + ((decimal)_tarjeta.valor).ToString("N2") + ") con un total de consumos mas saldo actual, superior al valor de referencia de la misma, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencial: {2} \nIpMaquina: {3} \nIpServidor: {4} \nCajeroNombre: {5} \nCajeroIdentificacion: {6}",
                                        Control.Common.GlobalParameters.Establecimiento,
                                        Control.Common.GlobalParameters.PuntoEmision,
                                        secuenciaFactura.ToString(),
                                        Control.Common.GlobalParameters.IpMaquina,
                                        Control.Common.GlobalParameters.SelectedServerIp,
                                        Control.Common.GlobalParameters.UsuarioNombre,
                                        Control.Common.GlobalParameters.Usuario);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", msj);

                            var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                            Properties.Settings.Default.MAILERROR_FROM,
                            Properties.Settings.Default.MAILERROR_ALIAS,
                            destinoMail,
                            Properties.Settings.Default.MAILERROR_CC,
                            "POS - Giftcard Sospechosa Detectada",
                            msj,
                            false,
                            String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de alerta durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[numgiftcard]", valor = _tarjeta.codigo });
                            Control.Common.General.GetMensajeToList(568, parametros);
                            //Control.Common.WinForm.ShowMessage("La giftcard '" + _tarjeta.codigo + "' tiene un histórico de consumos superior al valor de referencia de la misma, por lo que no puede ser usada para pagar esta factura. Contacte a administrador ahora!");

                            return respuesta;
                        }
                    }                  
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "Imposible finalizar proceso en este momento por lo que se permitira continuar con la transaccion, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                Properties.Settings.Default.MAILERROR_FROM,
                Properties.Settings.Default.MAILERROR_ALIAS,
                destinoMail,
                Properties.Settings.Default.MAILERROR_CC,
                "POS - No se pudo comprobar Giftcard",
                String.Format("No se pudo completar la validacion de saldo de la giftcard '" + (_tarjeta == null ? "Objeto giftcard esta nulo" : _tarjeta.codigo) + "', por lo que se permitira continuar con la transaccion. Verificar la novedad inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencia: {2} \n\nDatos Excepcion ------------\nClass: {3} \nMethod: {4} \nMessage: {5} \nStackTrace: {6}",
                              Control.Common.GlobalParameters.Establecimiento,
                              Control.Common.GlobalParameters.PuntoEmision,
                              secuenciaFactura,
                              "POS.Control.TarjetaRegalo",
                              "TieneSaldoCuadrado",
                              Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                              ex.StackTrace),
                false,
                String.Empty);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }

                respuesta = true;
            }

            return respuesta;
        }

        public bool TieneSaldoCuadradoGiftEmpl(long secuenciaFactura)
        {
            bool respuesta = true;
            string destinoMail = "devteam@liris.com.ec";
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            string squery = string.Empty;
            DataSet dtsConsulta = new DataSet();


            decimal totalConsumido = 0;
            decimal saldogGC = 0;
            decimal valorCosumo = 0;
            decimal valorGC = 0;
            decimal totalConsumoGc = 0;
            int codError = 0;
            string msjError;
            try
            {
                squery = string.Empty;
                squery = string.Concat(squery, "Exec spCuadraSaldoGiftEmpl", Environment.NewLine);
                squery = string.Concat(squery, $"   @IdGiftCard = '{_tarjetaGift.IdGiftCard }'", Environment.NewLine);
                squery = string.Concat(squery, $"   , @valorXconsumir = 0", Environment.NewLine);

                dtsConsulta = Control.Common.General.GetDataSet(squery);
                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            codError = Int32.Parse(data["codError"].ToString());
                            msjError = data["msjError"].ToString();

                            saldogGC = decimal.Parse(data["saldogGC"].ToString());
                            valorCosumo = decimal.Parse(data["valorCosumo"].ToString());
                            valorGC = decimal.Parse(data["valorGC"].ToString());
                            totalConsumoGc = decimal.Parse(data["totalConsumoGc"].ToString());
                        }
                    }
                }

                if (codError != 0)
                {
                    respuesta = false;

                    string msj = String.Format("En la siguiente caja se está intentando pasar una GiftCard de EMPLEADO (" + _tarjetaGift.IdGiftCard + "; Saldo Actual: " + _tarjeta.saldo.ToString("N2") + "; Saldo Tbl_DineroGiftCardApp: " + _tarjetaGift.Saldo.ToString("N2") + ") con un saldo diferente de core_giftcard, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencial: {2} \nIpMaquina: {3} \nIpServidor: {4} \nCajeroNombre: {5} \nCajeroIdentificacion: {6}",
                                Control.Common.GlobalParameters.Establecimiento,
                                Control.Common.GlobalParameters.PuntoEmision,
                                secuenciaFactura.ToString(),
                                Control.Common.GlobalParameters.IpMaquina,
                                Control.Common.GlobalParameters.SelectedServerIp,
                                Control.Common.GlobalParameters.UsuarioNombre,
                                Control.Common.GlobalParameters.Usuario);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", msj);

                    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                    Properties.Settings.Default.MAILERROR_FROM,
                    Properties.Settings.Default.MAILERROR_ALIAS,
                    destinoMail,
                    Properties.Settings.Default.MAILERROR_CC,
                    "POS - Giftcard Sospechosa Detectada",
                    msj,
                    false,
                    String.Empty);

                    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de alerta durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                    }

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[numgiftcard]", valor = _tarjeta.codigo });
                    Control.Common.General.GetMensajeToList(569, parametros);
                    //Control.Common.WinForm.ShowMessage("La giftcard '" + _tarjeta.codigo + "' tiene más saldo que lo que ha recargado por lo que no puede ser usada para pagar esta factura. Contacte a administrador ahora!");

                    return respuesta;

                }
            }
            catch (SqlException sqlEx)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Warning, "TarjetaRegalo", "TieneSaldoCuadrado",
                   $"SqlException: {sqlEx.Number} - {sqlEx.Message}: ");


                respuesta = false;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "Imposible finalizar proceso en este momento por lo que se permitira continuar con la transaccion, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                Properties.Settings.Default.MAILERROR_FROM,
                Properties.Settings.Default.MAILERROR_ALIAS,
                destinoMail,
                Properties.Settings.Default.MAILERROR_CC,
                "POS - No se pudo comprobar Giftcard",
                String.Format("No se pudo completar la validacion de saldo de la giftcard '" + (_tarjeta == null ? "Objeto giftcard esta nulo" : _tarjeta.codigo) + "', por lo que se permitira continuar con la transaccion. Verificar la novedad inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencia: {2} \n\nDatos Excepcion ------------\nClass: {3} \nMethod: {4} \nMessage: {5} \nStackTrace: {6}",
                              Control.Common.GlobalParameters.Establecimiento,
                              Control.Common.GlobalParameters.PuntoEmision,
                              secuenciaFactura,
                              "POS.Control.TarjetaRegalo",
                              "TieneSaldoCuadrado",
                              Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                              ex.StackTrace),
                false,
                String.Empty);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }

                respuesta = true;

            }
            

            return respuesta;

          
            //try
            //{

            //    //if (_tarjetaGift != null)
            //    //{
            //    //    decimal totalConsumido = 0;
            //    //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "Se verificará que la Giftcard " + _tarjetaGift.IdGiftCard + " no tenga un saldo superior a sus recargas o su valor de referencia.");
            //    //    using (POSEntities db = new POSEntities())
            //    //    {
            //    //        totalConsumido = db.core_facturapago.Where(x => x.datos == _tarjetaGift.IdGiftCard && x.tipo_id.Contains("GIFT CARD")).Sum(x => (decimal?)x.valor) ?? 0M;
            //    //    }


            //    //    if ((totalConsumido + _tarjetaGift.Saldo) > _tarjetaGift.Valor)
            //    //    {
            //    //        respuesta = false;

            //    //        string msj = String.Format("En la siguiente caja se está intentando pasar una GiftCard de EMPLEADO (" + _tarjetaGift.IdGiftCard + "; Saldo Actual: " + _tarjeta.saldo.ToString("N2") + "; Saldo Tbl_DineroGiftCardApp: " + _tarjetaGift.Saldo.ToString("N2") + ") con un saldo diferente de core_giftcard, verificar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencial: {2} \nIpMaquina: {3} \nIpServidor: {4} \nCajeroNombre: {5} \nCajeroIdentificacion: {6}",
            //    //                    Control.Common.GlobalParameters.Establecimiento,
            //    //                    Control.Common.GlobalParameters.PuntoEmision,
            //    //                    secuenciaFactura.ToString(),
            //    //                    Control.Common.GlobalParameters.IpMaquina,
            //    //                    Control.Common.GlobalParameters.SelectedServerIp,
            //    //                    Control.Common.GlobalParameters.UsuarioNombre,
            //    //                    Control.Common.GlobalParameters.Usuario);

            //    //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", msj);

            //    //        var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
            //    //        Properties.Settings.Default.MAILERROR_FROM,
            //    //        Properties.Settings.Default.MAILERROR_ALIAS,
            //    //        destinoMail,
            //    //        Properties.Settings.Default.MAILERROR_CC,
            //    //        "POS - Giftcard Sospechosa Detectada",
            //    //        msj,
            //    //        false,
            //    //        String.Empty);

            //    //        if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
            //    //        {
            //    //            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de alerta durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
            //    //        }

            //    //        parametros = new List<ParametrosMensajes>();
            //    //        parametros.Add(new ParametrosMensajes() { codigo = "[numgiftcard]", valor = _tarjeta.codigo });
            //    //        Control.Common.General.GetMensajeToList(569, parametros);
            //    //        //Control.Common.WinForm.ShowMessage("La giftcard '" + _tarjeta.codigo + "' tiene más saldo que lo que ha recargado por lo que no puede ser usada para pagar esta factura. Contacte a administrador ahora!");

            //    //        return respuesta;
            //    //    }
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "Imposible finalizar proceso en este momento por lo que se permitira continuar con la transaccion, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

            //    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
            //    Properties.Settings.Default.MAILERROR_FROM,
            //    Properties.Settings.Default.MAILERROR_ALIAS,
            //    destinoMail,
            //    Properties.Settings.Default.MAILERROR_CC,
            //    "POS - No se pudo comprobar Giftcard",
            //    String.Format("No se pudo completar la validacion de saldo de la giftcard '" + (_tarjeta == null ? "Objeto giftcard esta nulo" : _tarjeta.codigo) + "', por lo que se permitira continuar con la transaccion. Verificar la novedad inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nSecuencia: {2} \n\nDatos Excepcion ------------\nClass: {3} \nMethod: {4} \nMessage: {5} \nStackTrace: {6}",
            //                  Control.Common.GlobalParameters.Establecimiento,
            //                  Control.Common.GlobalParameters.PuntoEmision,
            //                  secuenciaFactura,
            //                  "POS.Control.TarjetaRegalo",
            //                  "TieneSaldoCuadrado",
            //                  Control.Common.ExceptionHandler.GetExceptionMessages(ex),
            //                  ex.StackTrace),
            //    false,
            //    String.Empty);

            //    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
            //    {
            //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.TarjetaRegalo", "TieneSaldoCuadrado", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
            //    }

            //    respuesta = true;
            //}

            //return respuesta;
        }

        public bool EstaAsociadaGrupoCliente(ref string identificacion, ref string msgError, ref string nombreGrupo)
        {
            bool respuesta = false;

            try
            {
                if (_tarjeta != null)
                {
                    if (_tarjeta.tipo == 2)
                    {
                        respuesta = true;
                        using (POSEntities db = new POSEntities())
                        {
                            if (_tarjeta.IdGrupoCliente == null)
                            {
                                msgError = "La Giftcard '" + _tarjeta.codigo + "' está configurada como tipo GrupoCliente pero no se ha configurado nada en su campo IdGrupoCliente";
                            }
                            else
                            {
                                var grupoCliente = db.LstGrupoCliente.Where(x => x.Id == _tarjeta.IdGrupoCliente).FirstOrDefault();
                                if (grupoCliente == null)
                                {
                                    msgError = "No se encontró ningun grupo configurado con Id '" + _tarjeta.IdGrupoCliente + "' en LstGrupoCliente";
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(grupoCliente.Identificacion))
                                    {
                                        msgError = "La Giftcard está configurada como tipo GrupoCliente pero en el grupo con Id '" + _tarjeta.IdGrupoCliente + "' no se ha configurado nada en su campo Identificacion";
                                    }
                                    else
                                    {
                                        var clientePrincipal = db.pos_customer.Where(x => x.ACCOUNTNUM == grupoCliente.Identificacion).FirstOrDefault();
                                        if (clientePrincipal == null)
                                        {
                                            msgError = "La Giftcard está configurada como tipo GrupoCliente pero no se encontró ningun cliente registrado con identificacion(AccountNum) '" + grupoCliente.Identificacion + "'";
                                        }
                                        else
                                        {
                                            identificacion = grupoCliente.Identificacion;
                                            nombreGrupo = clientePrincipal.NAME;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msgError = string.Format("Excepcion:{0}. StackTrace:{1}", Control.Common.ExceptionHandler.GetExceptionMessages(ex), ex.StackTrace);
            }

            if (!string.IsNullOrEmpty(msgError))
            {
                msgError = "Se detectó que esta giftcard está asociada a un grupo de cliente pero encontramos una novedad, si el problema persiste contacte al administrador. " + msgError;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.TarjetaRegalo", "EstaAsociadaGrupoCliente", msgError);
            }

            return respuesta;
        }

        private void registrarLOG(string accion, decimal valor, string establecimiento,string descripcion)
        {
            using (var db = new POSEntities())
            {
                var log = new core_giftcardtransaccion();
                log.tipo = accion;
                log.valor = valor;
                log.tarjeta = _tarjeta.codigo;
                log.fecha_creacion = DateTime.Now;
                log.fecha_modificacion = DateTime.Now;
                log.establecimiento= establecimiento;
                log.descripcion = descripcion;
                db.core_giftcardtransaccion.Add(log);
                db.SaveChanges();
            }
        }

        private void registrarLOG(string accion, decimal valor, string establecimiento, string descripcion, POSEntities db)
        {
           
                var log = new core_giftcardtransaccion();
                log.tipo = accion;
                log.valor = valor;
                log.tarjeta = _tarjeta.codigo;
                log.fecha_creacion = DateTime.Now;
                log.fecha_modificacion = DateTime.Now;
                log.establecimiento = establecimiento;
                log.descripcion = descripcion;
                db.core_giftcardtransaccion.Add(log);
        }

        private void registrarLOGGiftCard(string accion, decimal valor, string establecimiento, string descripcion, POSEntities db)
        {

            var log = new core_giftcardtransaccion();
            log.tipo = accion;
            log.valor = valor;
            log.tarjeta = _tarjetaGift.IdGiftCard;
            log.fecha_creacion = DateTime.Now;
            log.fecha_modificacion = DateTime.Now;
            log.establecimiento = establecimiento;
            log.descripcion = descripcion;
            db.core_giftcardtransaccion.Add(log);
        }

        public bool activarTarjeta(decimal valor, string establecimiento,string desc)
        {
            try
            {

                using (var db = new POSEntities())
                {
                    _tarjeta = db.core_giftcard.Single(x => x.codigo == _tarjeta.codigo);
                    _tarjeta.saldo = valor;
                    _tarjeta.activo = true;
                    _tarjeta.fecha_activacion = DateTime.Now;
                    _tarjeta.fecha_expiracion = DateTime.Now.AddYears(1);
                    db.SaveChanges();
                }
                this.registrarLOG(ACTIVACION, valor, establecimiento, desc);
                return true;
            }
            catch 
            {
                return false;
            }
            
        }

        public bool ActualizaSaldoGiftCard(decimal valor, string establecimiento, POSEntities db)
        {
            try
            {
                _tarjetaGift = db.Tbl_DineroGiftCardApp.Single(x => x.IdCliente == _tarjetaGift.IdCliente && x.IdGiftCard == _tarjetaGift.IdGiftCard);
                _tarjetaGift.Saldo = _tarjetaGift.Saldo - valor;
                if (_tarjetaGift.Saldo == 0)
                    _tarjetaGift.Estado = 0;
                _tarjetaGift.FechaModificacion = DateTime.Now;

               
                //db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool activarTarjeta(decimal valor, string establecimiento, string desc, POSEntities db)
        {
            try
            {
                    _tarjeta = db.core_giftcard.Single(x => x.codigo == _tarjeta.codigo);
                    _tarjeta.saldo = valor;
                    _tarjeta.activo = true;
                    _tarjeta.fecha_activacion = DateTime.Now;
                _tarjeta.fecha_expiracion = DateTime.Now.AddYears(1);
                this.registrarLOG(ACTIVACION, valor, establecimiento, desc, db);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool realizarConsumo(decimal valor, string establecimiento, string desc, POSEntities db)
        {
            try
            {
                //using (var db = new POSEntities())
                //{
                //    db.SaveChanges();
                //}
                _tarjeta = db.core_giftcard.Single(x => x.codigo == _tarjeta.codigo);
                _tarjeta.saldo = _tarjeta.saldo - valor;

                this.registrarLOG(CONSUMO, valor, establecimiento, desc, db);
                return true;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TarjetaRegalo", "RealizarConsumo", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                return false;
            }
        }


        /// <summary>
        /// Registra el consumo de gift card en srv-pos
        /// </summary>
        /// <param name="monto_consumo"></param>
        /// <param name="establecimiento"></param>
        /// <param name="desc">Consumo en factura: F-011-999-000000177</param>
        /// <returns></returns>
        public bool realizarConsumoGiftCardGen(decimal monto_consumo, string establecimiento, string numFactura, bool EsUsoAppMovil)
        {

            XmlDocument xmlDoc = new XmlDocument();

            XmlNode rootNode = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(rootNode);

            XmlNode userNode = xmlDoc.CreateElement("req");
            XmlAttribute attribute = xmlDoc.CreateAttribute("tarjeta");

            if (EsUsoAppMovil == true)
            {
                attribute.Value = _tarjetaGift.IdGiftCard;
            }
            else
            {
                attribute.Value = _tarjeta.codigo;
            }

            userNode.Attributes.Append(attribute);

            XmlAttribute attribute1 = xmlDoc.CreateAttribute("tipo");
            attribute1.Value = CONSUMO;
            userNode.Attributes.Append(attribute1);

            XmlAttribute attribute2 = xmlDoc.CreateAttribute("valor");
            attribute2.Value = monto_consumo.ToString() ;
            userNode.Attributes.Append(attribute2);

            XmlAttribute attribute3 = xmlDoc.CreateAttribute("descripcion");
            attribute3.Value = "Consumo en factura: " + numFactura;
            userNode.Attributes.Append(attribute3);

            XmlAttribute attribute4 = xmlDoc.CreateAttribute("establecimiento");
            attribute4.Value = establecimiento;
            userNode.Attributes.Append(attribute4);

            XmlAttribute attribute5 = xmlDoc.CreateAttribute("esAppMovil");
            attribute5.Value = EsUsoAppMovil.ToString();
            userNode.Attributes.Append(attribute5);

            rootNode.AppendChild(userNode);

            bool result = false;
            SqlParameter paramResult = new SqlParameter("@respuesta", SqlDbType.VarChar, -1);
            paramResult.Direction = System.Data.ParameterDirection.Output;
            string parameterValue = xmlDoc.InnerXml.ToString();

            var addParameters = new List<SqlParameter>
             {
              new SqlParameter("@xmlRequest", parameterValue),
              paramResult
             };

            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;   
                SqlConnection conn = new SqlConnection(cadenaCon);
                SqlCommand select = new SqlCommand("Exec PtsCliente.spPagoGiftCardGen @xmlRequest, @respuesta out", conn); // srv-pos 
                try
                {

                    select.Parameters.AddRange(addParameters.ToArray());
                    conn.Open();
                    select.ExecuteNonQuery();
                    conn.Close();

                    string response = (string)paramResult.Value;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaRegalo", "realizarConsumoGiftCardGen", "respuesta :" + response);

                    // SINCRONIZACIÓN CON SERVIDOR LOCAL - AGREGADO JCHID
                    try
                    {
                        using (var pos = new POSEntities())
                        {
                            string conexionLocal = pos.Database.Connection.ConnectionString;

                            // Verificar que no sea la misma conexión que el servidor principal
                            if (conexionLocal != cadenaCon)
                            {
                                SqlConnection connLocal = new SqlConnection(conexionLocal);
                                SqlCommand selectLocal = new SqlCommand("Exec PtsCliente.spPagoGiftCardGen @xmlRequest, @respuesta out", connLocal);

                                // Recrear parámetros para evitar conflictos
                                SqlParameter paramResultLocal = new SqlParameter("@respuesta", SqlDbType.VarChar, -1);
                                paramResultLocal.Direction = System.Data.ParameterDirection.Output;

                                selectLocal.Parameters.Add(new SqlParameter("@xmlRequest", parameterValue));
                                selectLocal.Parameters.Add(paramResultLocal);

                                connLocal.Open();
                                selectLocal.ExecuteNonQuery();
                                connLocal.Close();

                                string responseLocal = (string)paramResultLocal.Value;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info,
                                    "Control/TarjetaRegalo", "realizarConsumoGiftCardGen-LOCAL",
                                    "Sincronización local exitosa. Respuesta: " + responseLocal);
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info,
                                    "Control/TarjetaRegalo", "realizarConsumoGiftCardGen-LOCAL",
                                    "Conexión local es igual a la principal, no se requiere sincronización");
                            }
                        }
                    }
                    catch (Exception exLocal)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error,
                            "Control/TarjetaRegalo", "realizarConsumoGiftCardGen-LOCAL",
                            "Error sincronizando con servidor local: " + Control.Common.ExceptionHandler.GetExceptionMessages(exLocal));
                        // No afecta el resultado principal - la transacción continúa
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaRegalo", "realizarConsumoGiftCardGen", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                    //Guarda consumo de GifCard en un archivo cuando no hay srv-pos para procesar por timer 
                    var query = "declare @respuesta as varchar(1000) " +
                      "declare @xmlRequest as varchar(3000) = '" + parameterValue + "'" +
                      " Exec PtsCliente.spPagoGiftCardGen @xmlRequest, @respuesta out; " +
                      " Select @respuesta ";
                    creaArhivoTemporalConsumoGifCard(query, numFactura);

                    return false;
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaRegalo", "realizarConsumoGiftCardGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío");
                return false;
            }
        }

        public bool getTarjetaGen(string codigo, string cliente, bool esAppMovil)
        {
            bool result = false;
            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                SqlConnection conn = new SqlConnection(cadenaCon);
                try
                {

                    string Query = "Exec [PtsCliente].[spConsultaGiftCardGen]  " +
                                    "'" + codigo + "','"+cliente+"','"+ esAppMovil.ToString() + "'";

                    conn.Open();
                    SqlCommand select = new SqlCommand(Query, conn);
                    IAsyncResult iar = select.BeginExecuteReader();
                    SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                    while (dr.Read())
                    {
                        if (esAppMovil == false)
                        {
                            _tarjeta = new core_giftcard();
                            _tarjeta.fecha_creacion = dr.GetDateTime(0);
                            _tarjeta.fecha_modificacion = dr.GetDateTime(1);
                            _tarjeta.codigo = dr.GetValue(2).ToString();
                            if (!dr.IsDBNull(3))
                                _tarjeta.fecha_activacion = dr.GetDateTime(3);
                            if (!dr.IsDBNull(4))
                                _tarjeta.fecha_expiracion = dr.GetDateTime(4);
                            if (!dr.IsDBNull(5))
                                _tarjeta.fecha_desactivacion = dr.GetDateTime(5);
                            if (!dr.IsDBNull(6))
                                _tarjeta.saldo = decimal.Parse(dr.GetValue(6).ToString());

                            _tarjeta.activo = bool.Parse(dr.GetValue(7).ToString());
                            _tarjeta.bono = bool.Parse(dr.GetValue(8).ToString());

                            if (!dr.IsDBNull(9))
                                _tarjeta.valor = decimal.Parse(dr.GetValue(9).ToString());
                            if (!dr.IsDBNull(10))
                                _tarjeta.tipo = byte.Parse(dr.GetValue(10).ToString());
                            if (!dr.IsDBNull(11))
                                _tarjeta.IdGrupoCliente = short.Parse(dr.GetValue(11).ToString());

                            result = true;
                        }
                        else
                        {
                            _tarjetaGift    =   new Tbl_DineroGiftCardApp();
                            _tarjetaGift.Id =   Int32.Parse(dr.GetValue(0).ToString());
                            _tarjetaGift.IdCliente  = dr.GetValue(1).ToString();
                            if (!dr.IsDBNull(2))
                                _tarjetaGift.ProgId = dr.GetValue(2).ToString();
                            _tarjetaGift.IdGiftCard = dr.GetValue(3).ToString();
                            _tarjetaGift.Saldo  = decimal.Parse(dr.GetValue(4).ToString());
                            _tarjetaGift.Estado = byte.Parse(dr.GetValue(5).ToString());
                            _tarjetaGift.FechaCreacion  = dr.GetDateTime(6);
                            if (!dr.IsDBNull(7))
                                _tarjetaGift.UsuarioCreacion    = dr.GetValue(7).ToString();
                            _tarjetaGift.FechaModificacion  = dr.GetDateTime(8);
                            if (!dr.IsDBNull(9))
                                _tarjetaGift.UsuarioModificacion    = dr.GetValue(9).ToString();
                            if (!dr.IsDBNull(10))
                                _tarjetaGift.Valor  = decimal.Parse(dr.GetValue(10).ToString());

                            result = true;

                        }
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaRegalo", "getTarjetaGen", "No se pudo consultar tarjeta, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaRegalo", "getTarjetaGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return result;
        }
        public bool getTarjetaGen(string codigo, string cliente, bool esAppMovil, bool local)
        {
            bool result = false;
            string cadenaCon = "";

            var pos = new POSEntities();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaRegalo", "getTarjetaGen", "Ejecuta getTarjetaGen");
            
           
            cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
            if (local) { cadenaCon = pos.Database.Connection.ConnectionString; } // jchid valida si es local por el parametro booleano.
            
             if (Control.Common.GlobalParameters.ConServerPuntos == "")
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaRegalo", "getTarjetaGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
                return false;
            }

            SqlConnection conn = new SqlConnection(cadenaCon);


            try
            {

                string Query = "Exec [PtsCliente].[spConsultaGiftCardGen]  " +
                                "'" + codigo + "','" + cliente + "','" + esAppMovil.ToString() + "'";

                
                conn.Open();


                SqlCommand select = new SqlCommand(Query, conn);
                IAsyncResult iar = select.BeginExecuteReader();
                SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                while (dr.Read())
                {
                    if (esAppMovil == false)
                    {
                        _tarjeta = new core_giftcard();
                        _tarjeta.fecha_creacion = dr.GetDateTime(0);
                        _tarjeta.fecha_modificacion = dr.GetDateTime(1);
                        _tarjeta.codigo = dr.GetValue(2).ToString();
                        if (!dr.IsDBNull(3))
                            _tarjeta.fecha_activacion = dr.GetDateTime(3);
                        if (!dr.IsDBNull(4))
                            _tarjeta.fecha_expiracion = dr.GetDateTime(4);
                        if (!dr.IsDBNull(5))
                            _tarjeta.fecha_desactivacion = dr.GetDateTime(5);
                        if (!dr.IsDBNull(6))
                            _tarjeta.saldo = decimal.Parse(dr.GetValue(6).ToString());

                        _tarjeta.activo = bool.Parse(dr.GetValue(7).ToString());
                        _tarjeta.bono = bool.Parse(dr.GetValue(8).ToString());

                        if (!dr.IsDBNull(9))
                            _tarjeta.valor = decimal.Parse(dr.GetValue(9).ToString());
                        if (!dr.IsDBNull(10))
                            _tarjeta.tipo = byte.Parse(dr.GetValue(10).ToString());
                        if (!dr.IsDBNull(11))
                            _tarjeta.IdGrupoCliente = short.Parse(dr.GetValue(11).ToString());

                        result = true;
                    }
                    else
                    {
                        _tarjetaGift = new Tbl_DineroGiftCardApp();
                        _tarjetaGift.Id = Int32.Parse(dr.GetValue(0).ToString());
                        _tarjetaGift.IdCliente = dr.GetValue(1).ToString();
                        if (!dr.IsDBNull(2))
                            _tarjetaGift.ProgId = dr.GetValue(2).ToString();
                        _tarjetaGift.IdGiftCard = dr.GetValue(3).ToString();
                        _tarjetaGift.Saldo = decimal.Parse(dr.GetValue(4).ToString());
                        _tarjetaGift.Estado = byte.Parse(dr.GetValue(5).ToString());
                        _tarjetaGift.FechaCreacion = dr.GetDateTime(6);
                        if (!dr.IsDBNull(7))
                            _tarjetaGift.UsuarioCreacion = dr.GetValue(7).ToString();
                        _tarjetaGift.FechaModificacion = dr.GetDateTime(8);
                        if (!dr.IsDBNull(9))
                            _tarjetaGift.UsuarioModificacion = dr.GetValue(9).ToString();
                        if (!dr.IsDBNull(10))
                            _tarjetaGift.Valor = decimal.Parse(dr.GetValue(10).ToString());

                        result = true;

                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaRegalo", "getTarjetaGen", "No se pudo consultar tarjeta, a continuacion el detalle de la excepcion - " + ex.Message);
            }


          
            return result;
        }


        public bool realizarConsumoGiftCard(decimal valor, string establecimiento, string desc, POSEntities db)
        {
            try
            {
                this.registrarLOGGiftCard(CONSUMO, valor, establecimiento, desc, db);
                return true;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TarjetaRegalo", "RealizarConsumo", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Crear un directorio vacio
        /// </summary>
        public static void CreateEmptyDirectory(string fullPath)
        {
            if (!System.IO.Directory.Exists(fullPath))
            {
                System.IO.Directory.CreateDirectory(fullPath);
            }
        }

        /// <summary>
        /// Crea Arhivo Temporal para registas Consumo GifCard que no se pudieron realizar en su momento 
        /// </summary>
        /// <param name="consumo"></param>
        /// <param name="NumeroFactura"></param>
        private void creaArhivoTemporalConsumoGifCard(string consumo, string NumeroFactura)
        {
            try
            {
                //se adiciona alguna información y la fecha
                DateTime dateTime = new DateTime();
                dateTime = DateTime.Now;
                string strDate = Convert.ToDateTime(dateTime).ToString("yyyyMMddHHmmss");
                string rutaCompleta = Control.Common.GlobalParameters.ConsumoGiftCardInsertPath + NumeroFactura + "_" + strDate + ".txt";
                if (!string.IsNullOrEmpty(Control.Common.GlobalParameters.ConsumoGiftCardInsertPath))
                {
                    
                    CreateEmptyDirectory(Control.Common.GlobalParameters.ConsumoGiftCardInsertPath);
                    using (StreamWriter mylogs = File.AppendText(rutaCompleta))         //se crea el archivo
                    {
                        mylogs.WriteLine(consumo);

                        mylogs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //Insertar en el log y luego enviar correo.
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TarjetaRegalo", "creaArhivoTemporalConsumoGiftCard", "Ha ocurrido una excepción al momento de generar archivo POS CONSUMOGIFTCARD - A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                //Enviar Correo:
                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                           Properties.Settings.Default.MAILERROR_FROM,
                           Properties.Settings.Default.MAILERROR_ALIAS,
                           Properties.Settings.Default.MAILERROR_DESTINO,
                           Properties.Settings.Default.MAILERROR_CC,
                           "Consumo GiftCard no se generó archivo temporal para el ingreso",
                           String.Format("  \n\nDatos caja-----------------" +
                                           "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                           "  \n\n-----------------" +
                                         //datosVoucher,
                                         string.Empty,
                                       Control.Common.GlobalParameters.Establecimiento,
                                       Control.Common.GlobalParameters.PuntoEmision,
                                       Control.Common.GlobalParameters.IpMaquina,
                                       Control.Common.GlobalParameters.UsuarioNombre,
                                       Control.Common.GlobalParameters.Usuario,
                                       Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                       (false ? "" : ("StackTrace: " + ex.StackTrace + " \n"))),
                           false,
                           String.Empty);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TarjetaRegalo", "creaArhivoTemporalConsumoGiftCard", "No se pudo enviar notificacion del problema al grabar Consumo Gift Card en archivo temporal, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }

            }
        }
    }
}
