using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using Telerik.WinControls;
using System.Windows.Forms;
using System.Data;

namespace POS.Control
{

   
    public class NotaCredito
    {
        public static string ACTIVACION = "ACTIVACION";
        public static string ANULACION = "ANULACION";
        public static string CONSUMO = "CONSUMO";
        core_giftcard _tarjeta;
        core_notacredito _notacredito;

        RespuestaNC respuestaNC;

        public bool getTarjeta(string codigo)
        {
            var result = false;
     
            using (var db = new POSEntities())
            {
                DateTime fecha = DateTime.Now.Date;
                result = db.core_giftcard.Any(x=> x.codigo == codigo && x.fecha_expiracion >= fecha);

                if (result)
                {
                    _tarjeta = db.core_giftcard.Single(x => x.codigo == codigo);
                    _tarjeta.tipoTransaccion = "";
                    _tarjeta.tipoTransaccionId = 0;
                    _tarjeta.monto = 0;

;                }
                
                else
                {
                    //MessageBox.Show(MainWindow.ActiveForm,"No existe o se encuentra caducada");
                    Control.Common.General.GetMensajeToList(616);
                }
                
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
                    _tarjeta.monto = 0;
                    _tarjeta.tipoTransaccion = "";
                    _tarjeta.tipoTransaccionId = 0;
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

        public bool activarTarjeta(decimal valor, string establecimiento, string desc, POSEntities db)
        {
            try
            {
                _tarjeta = db.core_giftcard.Single(x => x.codigo == _tarjeta.codigo);
                _tarjeta.saldo = valor;
                _tarjeta.activo = true;
                _tarjeta.fecha_activacion = DateTime.Now;
                _tarjeta.fecha_expiracion = DateTime.Now.AddYears(1);
                _tarjeta.monto = 0;
                _tarjeta.tipoTransaccion = "";
                _tarjeta.tipoTransaccionId = 0;
                this.registrarLOG(ACTIVACION, valor, establecimiento, desc, db);
                return true;
            }
            catch
            {
                return false;
            }

        }


        public bool realizarConsumoNotaCredito(decimal valor, string establecimiento, string descripcion)
        {

            bool bResult = false;
            RespuestaNC returnValue = new RespuestaNC();
            

            string cadenaCon = "";
            string Query = string.Empty;


            try
            {

                if (Control.Common.GlobalParameters.ConServerPuntos != "")
                {

                    cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;

                    Query = string.Concat(Query, "Exec  spRealizaConsumoNC");
                    Query = string.Concat(Query, $" @codigo = '{_tarjeta.codigo}'");
                    Query = string.Concat(Query, $",  @valor = {valor.ToString()} ");
                    Query = string.Concat(Query, $",  @accion = 'CONSUMO' ");
                    Query = string.Concat(Query, $",  @establecimiento = '{establecimiento}'");
                    Query = string.Concat(Query, $",  @descripcion = '{descripcion}'");

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "realizarConsumoNotaCredito", "script Consumo NC " + Query);
                    DataSet dtsConsulta = Control.Common.General.GetDataSet(Query, cadenaCon);

                    if (dtsConsulta.Tables.Count > 0)
                    {
                        if (dtsConsulta.Tables[0].Rows.Count == 0)
                        {
                            returnValue = new RespuestaNC();
                            returnValue.CodError = -1;
                            returnValue.MsjError = "No se realizo cambios en NC";

                            bResult = false;
                            return bResult;

                        }
                        if (dtsConsulta.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                            {
                                returnValue = new RespuestaNC();
                                returnValue.CodError = Int32.Parse(data["CodError"].ToString());
                                returnValue.MsjError = data["MsjError"].ToString();
                            }

                            bResult = true;
                            return bResult;
                        }
                    }
                }
                else
                {
                    returnValue = new RespuestaNC();
                    returnValue.CodError = -1;
                    returnValue.MsjError = "No se realizo cambios en NC";

                    bResult = false;
                    return bResult;
                }
                
            }
            catch (Exception ex)
            {
                returnValue = new RespuestaNC();
                returnValue.CodError = -1;
                returnValue.MsjError = "Error: " + ex.Message;
                bResult = false;
                return bResult;
            }

            return bResult;
        }
        public bool realizarConsumo(decimal valor, string establecimiento, string desc, POSEntities db)
        {
            try
            {
                _tarjeta = db.core_giftcard.Single(x => x.codigo == _tarjeta.codigo);
                _tarjeta.saldo = _tarjeta.saldo - valor;
                _tarjeta.tipoTransaccion = "";
                _tarjeta.tipoTransaccionId = 0;
                _tarjeta.monto = 0;

                this.registrarLOG(CONSUMO, valor, establecimiento, desc, db);
                return true;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "NotaCredito", "RealizarConsumo", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                return false;
            }
        }

        public RespuestaNC validaNotaCredito(string numerodocumento, decimal valorRestante, bool local)
        {
            bool bResult = false;
            RespuestaNC returnValue = new RespuestaNC();
            Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
            bool RECALCULAR_IVA12_X_PAGONC = false;
            POSEntities pos = new POSEntities();

            string cadenaCon = "";
            string Query = string.Empty;

            try
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                if (local) { cadenaCon = pos.Database.Connection.ConnectionString; }


                Query = string.Concat(Query, "Exec  spValidaNotaCredito", Environment.NewLine);
                Query = string.Concat(Query, $" @numerodocumento = '{@numerodocumento}'", Environment.NewLine);
                Query = string.Concat(Query, $" , @valorRestante = {@valorRestante}", Environment.NewLine);
                Query = string.Concat(Query, $" , @RECALCULAR_IVA12_X_PAGONC = {RECALCULAR_IVA12_X_PAGONC}", Environment.NewLine);
                DataSet dtsConsulta = Control.Common.General.GetDataSet(Query, cadenaCon);
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "btnVerificar_Click", $"validaNotaCredito : Query | {Query}");


                if (dtsConsulta.Tables.Count > 0)
                {

                    if (dtsConsulta.Tables[0].Rows.Count == 0)
                    {
                        returnValue.CodError = -1;
                        returnValue.MsjError = "No se encontraon datos";

                        bResult = false;
                        return returnValue;
                    }


                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            returnValue = new RespuestaNC();
                            returnValue.CodError = Int32.Parse(data["CodError"].ToString());
                            returnValue.MsjError = data["MsjError"].ToString();
                            returnValue.idMensaje = Int32.Parse(data["idMensaje"].ToString());
                            returnValue.identificacionClte = string.Empty;
                            returnValue.numeroNC = data["numeroNC"].ToString();
                            returnValue.numerodocumento = data["numerodocumento"].ToString();

                            if (returnValue.CodError != 0)
                            {
                                return returnValue;
                            }

                            returnValue.saldo = decimal.Parse(data["saldo"].ToString());

                            returnValue.identificacionClte = data["identificacionClte"].ToString();
                            returnValue.numerodocumento = data["numerodocumento"].ToString();
                            returnValue.numeroNC = data["numeroNC"].ToString();
                            returnValue.factnum = int.Parse(data["factnum"].ToString());
                            returnValue.establecimiento = data["establecimiento"].ToString();
                            returnValue.punto_emision = data["punto_emision"].ToString();

                            returnValue._tarjeta = new core_giftcard();
                            _tarjeta = new core_giftcard();
                            _notacredito = new core_notacredito();
                            _tarjeta.activo = false;


                            if (!string.IsNullOrEmpty(data["activo"].ToString()))
                            {
                                if ((bool)data["activo"])
                                {
                                    _tarjeta.activo = true;
                                }
                            }


                            _tarjeta.bono = false;

                            if (!string.IsNullOrEmpty(data["bono"].ToString()))
                            {
                                if ((bool)data["bono"])
                                {
                                    _tarjeta.bono = true;
                                }
                            }

                           
                            _tarjeta.monto = 0;
                            _tarjeta.tipoTransaccion = "";
                            _tarjeta.tipoTransaccionId = 0;

                            _tarjeta.saldo = decimal.Parse(data["saldo"].ToString());
                            _tarjeta.valor = decimal.Parse(data["valor"].ToString());

                            //if (data["fecha_activacion"] != null)
                            if (!string.IsNullOrEmpty(data["fecha_activacion"].ToString()))
                            {
                                _tarjeta.fecha_activacion = DateTime.Parse(data["fecha_activacion"].ToString());
                            }

                            if (!string.IsNullOrEmpty(data["fecha_expiracion"].ToString()))
                            {
                                _tarjeta.fecha_expiracion = DateTime.Parse(data["fecha_expiracion"].ToString());
                            }

                            if (!string.IsNullOrEmpty(data["fecha_desactivacion"].ToString()))
                            {
                                _tarjeta.fecha_desactivacion = DateTime.Parse(data["fecha_desactivacion"].ToString());
                            }

                            _tarjeta.codigo = data["numerodocumento"].ToString();
                            returnValue._tarjeta = _tarjeta;

                            Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = (bool)data["RECALCULAR_IVA12_X_PAGONC"];
                            Control.Common.GlobalParameters.VALIDAR_VIGENCIA_IVA12 = (bool)data["VALIDAR_VIGENCIA_IVA12"];
                            Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = (bool)data["RECALCULAR_IVA12_X_PAGONC"];
                            Control.Common.GlobalParameters.VIGENCIA_IVA12_SEGUN_FECHA_NC = (DateTime)data["VIGENCIA_IVA12_SEGUN_FECHA_NC"];
                        }

                    }

                    returnValue.CodError = 0;
                    returnValue.MsjError = "Validacion correcta";
                    bResult = true;
                    return returnValue;
                }
                else
                {
                    returnValue.CodError = -1;
                    returnValue.MsjError = "No se encontraon datos";
                    bResult = false;
                    return returnValue;
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                returnValue.CodError = -1;
                returnValue.MsjError = "Error: " + ex.Message;
                bResult = false;
                return returnValue;
            }

            
        }



    }

    public class RespuestaNC {

        private int _CodError { get; set; }
        public int CodError
        {
            get { return _CodError; }
            set { _CodError = value; }
        }

        private int _idMensaje { get; set; }
        private string _MsjError { get; set; }
        private string _numerodocumento { get; set; }
        private string _numeroNC { get; set; }
        private int _factnum { get; set; }
        private string _establecimiento { get; set; }
        private string _punto_emision { get; set; }
        private string _idNotaCredito { get; set; }
        private decimal _saldo { get; set; }
        private string _identificacionClte { get; set; }

        public int idMensaje
        {
            get { return _idMensaje; }
            set { _idMensaje = value; }
        }
        public string  MsjError
        {
            get { return _MsjError; }
            set { _MsjError = value; }
        }
        public string numerodocumento
        {
            get { return _numerodocumento; }
            set { _numerodocumento = value; }
        }
        public string numeroNC
        {
            get { return _numeroNC; }
            set { _numeroNC = value; }
        }

        public decimal saldo
        {
            get { return _saldo; }
            set { _saldo = value; }
        }
        public int factnum
        {
            get { return _factnum; }
            set { _factnum = value; }
        }
        public string establecimiento
        {
            get { return _establecimiento; }
            set { _establecimiento = value; }
        }
        public string punto_emision
        {
            get { return _punto_emision; }
            set { _punto_emision = value; }
        }

        public string idNotaCredito
        {
            get { return _idNotaCredito; }
            set { _idNotaCredito = value; }
        }
        public string identificacionClte
        {
            get { return _identificacionClte; }
            set { _identificacionClte = value; }
        }
        

        public core_giftcard _tarjeta { get; set; }
        
    }
}
