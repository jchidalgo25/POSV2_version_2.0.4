using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using POS.Models;

namespace POS.Control
{
    public class TarjetaCreditoInterno
    {
        public static string ACTIVACION = "ACTIVACION";
        public static string ANULACION = "ANULACION";
        public static string CONSUMO = "CONSUMO";

        core_tarjetacreditointerno _tarjeta;
        core_tarjetacreditointerno _tarjetaAdicional;

        public TarjetaCreditoInterno(core_tarjetacreditointerno tAdicional)
        {
            _tarjetaAdicional = tAdicional;
        }

        public TarjetaCreditoInterno()
        {
            //Constructor default
        }




        public bool getTarjeta(string codigo)
        {
            var result=false;
            using (var db=new POSEntities())
            {
                result= db.core_tarjetacreditointerno.Any(x=>x.codigo ==codigo);
                if (result)
                {
                    _tarjeta = db.core_tarjetacreditointerno.Single(x => x.codigo == codigo);
                }
            }
            return result;
        }

        /// <summary>
        /// Consulta de tarjeta interna del portal en srv-pos
        /// </summary>
        /// <param name="codigo">Codigo de tarjeta </param>
        /// <returns></returns>
        public bool getTarjetaGen(string codigo)
        {
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
                       
                        _tarjeta = new core_tarjetacreditointerno();

                        _tarjeta.id = Int32.Parse(dr.GetValue(0).ToString());
                        _tarjeta.fecha_creacion  = dr.GetDateTime(1);
                        _tarjeta.fecha_modificacion  = dr.GetDateTime(2);
                        _tarjeta.empresa_id  = Int32.Parse(dr.GetValue(3).ToString());
                        _tarjeta.codigo  = dr.GetValue(4).ToString();

                        _tarjeta.identificacion = dr.GetValue(5).ToString();
                        _tarjeta.nombre_tarjeta = dr.GetValue(6).ToString();

                        if (!dr.IsDBNull(7))
                            _tarjeta.fecha_activacion = dr.GetDateTime(7);
                        if (!dr.IsDBNull(8))
                            _tarjeta.fecha_expiracion = dr.GetDateTime(8);
                        if (!dr.IsDBNull(9))
                            _tarjeta.fecha_desactivacion = dr.GetDateTime(9);
                        if (!dr.IsDBNull(10))
                            _tarjeta.cupo = decimal.Parse(dr.GetValue(10).ToString());
                        
                        _tarjeta.saldo = decimal.Parse(dr.GetValue(11).ToString());
                        _tarjeta.activo = bool.Parse(dr.GetValue(12).ToString());    
                        
                        if (!dr.IsDBNull(13))
                             _tarjeta.identificacionPrincipal = dr.GetValue(13).ToString();

                         result = true;
                       
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaDelPortal", "getTarjetaGen", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaDelPortal", "getTarjetaGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return result;
        }



        public core_tarjetacreditointerno ConsultarTarjetaInternaGen(string codigo)
        {
            core_tarjetacreditointerno TarjetaInternaResult = new core_tarjetacreditointerno();
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
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaDelPortal", "ConsultarTarjetaInternaGen", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaDelPortal", "ConsultarTarjetaInternaGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return TarjetaInternaResult;
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

        public bool tarjetaNoActivada()
        {
            if (this._tarjeta == null)
            {
                return false;
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

        public decimal getSaldo()
        {
            if (_tarjeta != null)
            {
                return _tarjeta.saldo;
            }
            return 0M;
        }

        private void registrarLOG(string accion, decimal valor, string establecimiento,string descripcion)
        {
            using (var db = new POSEntities())
            {
                var log = new core_tarjetacreditointernotransaccion();
                log.tipo = accion;
                log.valor = valor;
                log.tarjeta = _tarjeta.codigo;
                log.fecha_creacion = DateTime.Now;
                log.fecha_modificacion = DateTime.Now;
              
                log.descripcion = descripcion;
                db.core_tarjetacreditointernotransaccion.Add(log);
                db.SaveChanges();
            }
        }

        private void registrarLOG(string accion, decimal valor, string establecimiento, string descripcion, POSEntities db)
        {
                var log = new core_tarjetacreditointernotransaccion();
                log.tipo = accion;
                log.valor = valor;
                log.tarjeta = _tarjeta.codigo;
                log.fecha_creacion = DateTime.Now;
                log.fecha_modificacion = DateTime.Now;
                
                log.descripcion = descripcion;
                db.core_tarjetacreditointernotransaccion.Add(log);
        }

       

        public bool realizarConsumo(decimal valor, string establecimiento, string desc, POSEntities db)
        {
            try
            {
                _tarjeta = db.core_tarjetacreditointerno.Single(x => x.codigo == _tarjeta.codigo);
                _tarjeta.saldo = _tarjeta.saldo - valor;

                if (_tarjetaAdicional != null)
                {
                    string codigoAdicional = _tarjetaAdicional.codigo;
                    _tarjetaAdicional = db.core_tarjetacreditointerno.Where(x => x.codigo == _tarjetaAdicional.codigo && x.identificacionPrincipal == _tarjetaAdicional.identificacionPrincipal).FirstOrDefault();
                    if (_tarjetaAdicional != null)
                    {
                        _tarjetaAdicional.saldo = _tarjetaAdicional.saldo - valor;
                    }
                    else
                    {
                        throw new Exception("No existe tarjeta adicional con código '" + codigoAdicional + "'. Esto puede deberse a que la tarjeta fue modificada mientras se realizaba la compra en caja");
                    }
                }

                this.registrarLOG(CONSUMO, valor, establecimiento, desc, db);

                return true;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TarjetaDelPortal", "RealizarConsumo", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Registra el consumo de en srv-pos
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="establecimiento"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public bool realizarConsumoGen(decimal valor, string establecimiento, string desc)
        {

            XmlDocument xmlDoc = new XmlDocument();

            XmlNode rootNode = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(rootNode);

            XmlNode userNode = xmlDoc.CreateElement("req");
            XmlAttribute attribute = xmlDoc.CreateAttribute("tarjeta");

            attribute.Value = _tarjeta.codigo;

            userNode.Attributes.Append(attribute);

            XmlAttribute attribute1 = xmlDoc.CreateAttribute("tipo");
            attribute1.Value = CONSUMO;
            userNode.Attributes.Append(attribute1);

            XmlAttribute attribute2 = xmlDoc.CreateAttribute("valor");
            attribute2.Value = valor.ToString();
            userNode.Attributes.Append(attribute2);

            XmlAttribute attribute3 = xmlDoc.CreateAttribute("descripcion");
            attribute3.Value = desc;
            userNode.Attributes.Append(attribute3);

            if (_tarjetaAdicional != null)
            {
                XmlAttribute attribute4 = xmlDoc.CreateAttribute("tarjetaAdicional");
                attribute4.Value = _tarjetaAdicional.codigo;
                userNode.Attributes.Append(attribute4);

                XmlAttribute attribute5 = xmlDoc.CreateAttribute("identificacionPrincipal");
                attribute5.Value = _tarjetaAdicional.identificacionPrincipal;
                userNode.Attributes.Append(attribute5);
            }

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
                SqlCommand select = new SqlCommand("Exec dbo.spPagoTarjetaInternaGen @xmlRequest, @respuesta out", conn);
                try
                {

                    select.Parameters.AddRange(addParameters.ToArray());
                    conn.Open();
                    select.ExecuteNonQuery();

                    conn.Close();

                    string response = (string)paramResult.Value;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaDelPortal", "realizarConsumoGen", "respuesta :" + response);
                    return true;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/TarjetaDelPortal", "realizarConsumoGen", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                   
                    return false;
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/TarjetaDelPortal", "realizarConsumoGen", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío");
                return false;
            }
        }


    }
}
