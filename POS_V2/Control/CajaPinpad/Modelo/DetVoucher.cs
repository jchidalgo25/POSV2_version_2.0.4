using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Control.PINPAD;
using POS.Models;
using static POS.Control.PINPAD.Tramas;

namespace POS.Control.CajaPinpad.Modelo
{
    public class DetVoucher
    {
        public string TARJETA { get; set; }
        public string CODIGOPROCESO { get; set; }
        public string FECHACONSUMO { get; set; }
        public string HORACONSUMO { get; set; }
        public string NUMEROVOUCHER { get; set; }
        public string AUTORIZACION { get; set; }
        public string VALORCONSUMO { get; set; }
        public string FORMAAUTORIZA { get; set; }
        public string TIPOCONSUMO { get; set; }
        public string PLAZO { get; set; }
        public string TIPOLECTURA { get; set; }
        public string TIPOMONEDA { get; set; }
        public string VALORIVA { get; set; }
        public string VALORSERVICIO { get; set; }
        public string VALORPROPINA { get; set; }
        public string VALORINTERES { get; set; }
        public string VALORFIJO { get; set; }
        public string TIPOPROMOCION { get; set; }
        public string MESESGRACIA { get; set; }
        public string EMPRESASERVICIO { get; set; }
        public string ESTADOTRX { get; set; }
        public string CODIGORESPUESTA { get; set; }
        public string TIPODISPOSITIVO { get; set; }
        public string ADQUIRENTETARJETA { get; set; }
        public string ADQUIRENTESERVICIO { get; set; }
        public string MONTOGRAVAIVA { get; set; }
        public string MONTONOGRAVAIVA { get; set; }
        public string PUNTOEMISION { get; set; }
        public string PROCESADO { get; set; }
        public string GRUPOTAR { get; set; }
        public string AUTORIZADOR { get; set; }
        public string LOTE { get; set; }
        public string ANULADO { get; set; }
        public string PROCESADOTURNO { get; set; }
        public string FACTURA { get; set; }
        public string ARQC { get; set; }
        public string AIDEMV { get; set; }
        public string EMV { get; set; }
        public string TC { get; set; }
        public string PUBLICIDAD { get; set; }
        public string TIPOTRANSACCION { get; set; }
        public string BANCOADQUIRIENTE { get; set; }
        public string TARJETAHABIENTE { get; set; }
        public string MID { get; set; }
        public string TID { get; set; }
        public string VENCTAR { get; set; }
        public string ANULAUTORIZACION { get; set; }
        public string TIPOBANCOTARJETA { get; set; }
        public POS_VOUCHER objVoucher { get; set; }

        public POS_VOUCHER pos_voucher
        {
            get
            {
                POS_VOUCHER detVoucher = new POS_VOUCHER();
                detVoucher.TARJETA = TARJETA;
                detVoucher.CODIGOPROCESO = CODIGOPROCESO;
                detVoucher.FECHACONSUMO = FECHACONSUMO;
                detVoucher.HORACONSUMO = HORACONSUMO;
                detVoucher.NUMEROVOUCHER = NUMEROVOUCHER;
                detVoucher.AUTORIZACION = AUTORIZACION;
                detVoucher.ANULADO = ANULADO == "True" ? true : false;
                detVoucher.VALORCONSUMO = VALORCONSUMO;
                detVoucher.FORMAAUTORIZA = FORMAAUTORIZA;
                detVoucher.TIPOCONSUMO = TIPOCONSUMO;
                detVoucher.PLAZO = PLAZO;
                detVoucher.TIPOLECTURA = TIPOLECTURA;
                detVoucher.TIPOMONEDA = TIPOMONEDA;
                detVoucher.VALORIVA = VALORIVA;
                detVoucher.VALORSERVICIO = VALORSERVICIO;
                detVoucher.VALORPROPINA = VALORPROPINA;
                detVoucher.VALORINTERES = VALORINTERES;
                detVoucher.VALORFIJO = VALORFIJO;
                detVoucher.TIPOPROMOCION = TIPOPROMOCION;
                detVoucher.MESESGRACIA = MESESGRACIA;
                detVoucher.EMPRESASERVICIO = EMPRESASERVICIO;
                detVoucher.ESTADOTRX = ESTADOTRX;
                detVoucher.CODIGORESPUESTA = CODIGORESPUESTA;
                detVoucher.TIPODISPOSITIVO = TIPODISPOSITIVO;
                detVoucher.ADQUIRENTETARJETA = ADQUIRENTETARJETA;
                detVoucher.ADQUIRENTESERVICIO = ADQUIRENTESERVICIO;
                detVoucher.MONTOGRAVAIVA = MONTOGRAVAIVA;
                detVoucher.MONTONOGRAVAIVA = MONTONOGRAVAIVA;
                detVoucher.PUNTOEMISION = PUNTOEMISION;
                detVoucher.PROCESADO = PROCESADO == "True" ? true : false;
                detVoucher.GRUPOTAR = GRUPOTAR;
                detVoucher.AUTORIZADOR = int.Parse(AUTORIZADOR);
                detVoucher.LOTE = LOTE;
                detVoucher.FACTURA = FACTURA;
                detVoucher.ARQC = ARQC;
                detVoucher.AIDEMV = AIDEMV;
                detVoucher.EMV = EMV;
                detVoucher.TC = TC;
                detVoucher.PUBLICIDAD = PUBLICIDAD;
                detVoucher.TIPOTRANSACCION = TIPOTRANSACCION;
                detVoucher.BANCOADQUIRIENTE = BANCOADQUIRIENTE;
                detVoucher.TARJETAHABIENTE = TARJETAHABIENTE;
                detVoucher.MID = MID;
                detVoucher.TID = TID;
                detVoucher.VENCTAR = VENCTAR;
                detVoucher.ANULAUTORIZACION = ANULAUTORIZACION;
                detVoucher.TIPOBANCOTARJETA = TIPOBANCOTARJETA;
                return detVoucher;

            }
        }

        private string datosVoucher() {

            string sQuery = string.Empty;

            sQuery = string.Concat(sQuery, $" TARJETA = '{TARJETA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" CODIGOPROCESO = '{CODIGOPROCESO}'", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" FECHACONSUMO = '{FECHACONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" HORACONSUMO = '{HORACONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" NUMEROVOUCHER = '{NUMEROVOUCHER}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" AUTORIZACION = '{AUTORIZACION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" VALORCONSUMO = '{VALORCONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" FORMAAUTORIZA = '{FORMAAUTORIZA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TIPOCONSUMO = '{TIPOCONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" PLAZO = '{PLAZO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TIPOLECTURA = '{TIPOLECTURA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TIPOMONEDA = '{TIPOMONEDA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" VALORIVA = '{VALORIVA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" VALORSERVICIO = '{VALORSERVICIO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" VALORPROPINA = '{VALORPROPINA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" VALORINTERES = '{VALORINTERES}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" VALORFIJO = '{VALORFIJO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TIPOPROMOCION = '{TIPOPROMOCION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" MESESGRACIA = '{MESESGRACIA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" EMPRESASERVICIO = '{EMPRESASERVICIO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" ESTADOTRX = '{ESTADOTRX}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" CODIGORESPUESTA = '{CODIGORESPUESTA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TIPODISPOSITIVO = '{TIPODISPOSITIVO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" ADQUIRENTETARJETA = '{ADQUIRENTETARJETA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" ADQUIRENTESERVICIO = '{ADQUIRENTESERVICIO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" MONTOGRAVAIVA = '{MONTOGRAVAIVA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" MONTONOGRAVAIVA = '{MONTONOGRAVAIVA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" PUNTOEMISION = '{PUNTOEMISION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" PROCESADO = '{PROCESADO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" GRUPOTAR = '{GRUPOTAR}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" AUTORIZADOR = '{AUTORIZADOR}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" LOTE = '{LOTE}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" ANULADO = '{ANULADO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" PROCESADOTURNO = '{PROCESADOTURNO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" FACTURA = '{FACTURA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" ARQC = '{ARQC}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" AIDEMV = '{AIDEMV}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" EMV = '{EMV}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TC = '{TC}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" PUBLICIDAD = '{PUBLICIDAD}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TIPOTRANSACCION = '{TIPOTRANSACCION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" BANCOADQUIRIENTE = '{BANCOADQUIRIENTE}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TARJETAHABIENTE = '{TARJETAHABIENTE}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" MID = '{MID}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TID = '{TID}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" VENCTAR = '{VENCTAR}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" ANULAUTORIZACION = '{ANULAUTORIZACION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $" TIPOBANCOTARJETA = '{TIPOBANCOTARJETA}' ", Environment.NewLine);
            return sQuery;

        }

        public string GetInsertString()
        {
            string sQuery = string.Empty;
            
            sQuery = string.Concat(sQuery, " Insert into dbo.POS_VOUCHER (", Environment.NewLine);
            sQuery = string.Concat(sQuery, "      TARJETA,CODIGOPROCESO,FECHACONSUMO,HORACONSUMO,NUMEROVOUCHER,AUTORIZACION,VALORCONSUMO,FORMAAUTORIZA ", Environment.NewLine);
            sQuery = string.Concat(sQuery, "      , TIPOCONSUMO,PLAZO,TIPOLECTURA,TIPOMONEDA,VALORIVA,VALORSERVICIO,VALORPROPINA,VALORINTERES,VALORFIJO,TIPOPROMOCION,MESESGRACIA,EMPRESASERVICIO,ESTADOTRX,CODIGORESPUESTA", Environment.NewLine);
            sQuery = string.Concat(sQuery, "      , TIPODISPOSITIVO,ADQUIRENTETARJETA,ADQUIRENTESERVICIO,MONTOGRAVAIVA,MONTONOGRAVAIVA,PUNTOEMISION,PROCESADO,GRUPOTAR,AUTORIZADOR,LOTE,ANULADO ", Environment.NewLine);
            sQuery = string.Concat(sQuery, "      , PROCESADOTURNO,FACTURA,ARQC,AIDEMV,EMV,TC,PUBLICIDAD,TIPOTRANSACCION,BANCOADQUIRIENTE,TARJETAHABIENTE,MID,TID,VENCTAR,ANULAUTORIZACION,TIPOBANCOTARJETA ", Environment.NewLine);
            sQuery = string.Concat(sQuery, " ) ", Environment.NewLine);

            sQuery = string.Concat(sQuery, $"select  TARJETA = '{TARJETA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", CODIGOPROCESO = '{CODIGOPROCESO}'", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", FECHACONSUMO = '{FECHACONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", HORACONSUMO = '{HORACONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", NUMEROVOUCHER = '{NUMEROVOUCHER}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", AUTORIZACION = '{AUTORIZACION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", VALORCONSUMO = '{VALORCONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", FORMAAUTORIZA = '{FORMAAUTORIZA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TIPOCONSUMO = '{TIPOCONSUMO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", PLAZO = '{PLAZO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TIPOLECTURA = '{TIPOLECTURA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TIPOMONEDA = '{TIPOMONEDA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", VALORIVA = '{VALORIVA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", VALORSERVICIO = '{VALORSERVICIO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", VALORPROPINA = '{VALORPROPINA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", VALORINTERES = '{VALORINTERES}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", VALORFIJO = '{VALORFIJO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TIPOPROMOCION = '{TIPOPROMOCION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", MESESGRACIA = '{MESESGRACIA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", EMPRESASERVICIO = '{EMPRESASERVICIO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", ESTADOTRX = '{ESTADOTRX}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", CODIGORESPUESTA = '{CODIGORESPUESTA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TIPODISPOSITIVO = '{TIPODISPOSITIVO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", ADQUIRENTETARJETA = '{ADQUIRENTETARJETA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", ADQUIRENTESERVICIO = '{ADQUIRENTESERVICIO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", MONTOGRAVAIVA = '{MONTOGRAVAIVA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", MONTONOGRAVAIVA = '{MONTONOGRAVAIVA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", PUNTOEMISION = '{PUNTOEMISION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", PROCESADO = '{PROCESADO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", GRUPOTAR = '{GRUPOTAR}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", AUTORIZADOR = '{AUTORIZADOR}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", LOTE = '{LOTE}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", ANULADO = '{ANULADO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", PROCESADOTURNO = '{PROCESADOTURNO}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", FACTURA = '{FACTURA}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", ARQC = '{ARQC}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", AIDEMV = '{AIDEMV}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", EMV = '{EMV}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TC = '{TC}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", PUBLICIDAD = '{PUBLICIDAD}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TIPOTRANSACCION = '{TIPOTRANSACCION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", BANCOADQUIRIENTE = '{BANCOADQUIRIENTE}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TARJETAHABIENTE = '{TARJETAHABIENTE}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", MID = '{MID}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TID = '{TID}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", VENCTAR = '{VENCTAR}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", ANULAUTORIZACION = '{ANULAUTORIZACION}' ", Environment.NewLine);
            sQuery = string.Concat(sQuery, $", TIPOBANCOTARJETA = '{TIPOBANCOTARJETA}' ", Environment.NewLine);
            return sQuery;

        }


        public Respuesta EjecutaGrabar() {

            string sQuery = string.Empty;
            var pos = new POSEntities();
            DataSet dtsConsulta = new DataSet();
            Respuesta objRespuesta = new Respuesta();

            try
            {
                string CadenaConexion = pos.Database.Connection.ConnectionString;
                string CadenaInsert = GetInsertString();


                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, "Begin try", Environment.NewLine);
                sQuery = string.Concat(sQuery, CadenaInsert,  Environment.NewLine);
                sQuery = string.Concat(sQuery, "select CodigoRespuesta = 0 ", Environment.NewLine);
                sQuery = string.Concat(sQuery, ", MensajeRespuesta = 'POS_VOUCHER-Insertado correctamente'  ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "end try", Environment.NewLine);

                sQuery = string.Concat(sQuery, "Begin catch", Environment.NewLine);
                sQuery = string.Concat(sQuery, "    select CodigoRespuesta = ERROR_NUMBER() ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "        , MensajeRespuesta = 'POS_VOUCHER - Error: ' + ERROR_MESSAGE()  ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "end catch", Environment.NewLine);
                objRespuesta.stringVoucher = sQuery;

                dtsConsulta = Control.Common.General.GetDataSet(sQuery);
                

                if (dtsConsulta.Tables.Count > 0) {
                    if(dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows) {
                            objRespuesta.CodigoRespuesta = data["CodigoRespuesta"].ToString();
                            objRespuesta.MensajeRespuesta = data["MensajeRespuesta"].ToString();
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRespuesta.CodigoRespuesta = "-1";
                objRespuesta.MensajeRespuesta = "(ProcesaPinPad - Graba Voucher): Error=>" + ex.Message;
            }

            return objRespuesta;
        }

        public Respuesta EjecutaGrabarUDT()
        {

            string sQuery = string.Empty;
            var pos = new POSEntities();
            DataSet dtsConsulta = new DataSet();
            Respuesta objRespuesta = new Respuesta();
            string CadenaConexion = pos.Database.Connection.ConnectionString;
            DataTable tblVoucher = new DataTable();
            SqlConnection con = new SqlConnection();


            try
            {

                using (con = new SqlConnection(CadenaConexion))
                {
                    con.Open();
                    sQuery = string.Empty;
                    sQuery = string.Concat(sQuery, "BEGIN TRY  ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, "   Exec spPOSRegistraVoucher ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, "   @UDT_POS_Voucher ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, "END TRY   ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, "BEGIN CATCH   ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, "    Select CodigoRespuesta = ERROR_NUMBER()", Environment.NewLine);
                    sQuery = string.Concat(sQuery, "    , MensajeRespuesta = convert(varchar(300), '(POS_VOUCHER) ERROR: ' + ERROR_MESSAGE())", Environment.NewLine);
                    sQuery = string.Concat(sQuery, "END CATCH  ", Environment.NewLine);
                    tblVoucher = GetDataTableVoucher();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed", "EjecutaGrabarUDT - Query Insert" + sQuery );
                    using (SqlCommand cmd = new SqlCommand(sQuery, con))
                    {
                        DataRow workRow = tblVoucher.NewRow();
                        workRow["TARJETA"] = TARJETA;
                        workRow["CODIGOPROCESO"] = CODIGOPROCESO;
                        workRow["FECHACONSUMO"] = FECHACONSUMO;
                        workRow["HORACONSUMO"] = HORACONSUMO;
                        workRow["NUMEROVOUCHER"] = NUMEROVOUCHER;
                        workRow["AUTORIZACION"] = AUTORIZACION;
                        workRow["VALORCONSUMO"] = VALORCONSUMO;
                        workRow["FORMAAUTORIZA"] = FORMAAUTORIZA;
                        workRow["TIPOCONSUMO"] = TIPOCONSUMO;
                        workRow["PLAZO"] = PLAZO;
                        workRow["TIPOLECTURA"] = TIPOLECTURA;
                        workRow["TIPOMONEDA"] = TIPOMONEDA;
                        workRow["VALORIVA"] = VALORIVA;
                        workRow["VALORSERVICIO"] = VALORSERVICIO;
                        workRow["VALORPROPINA"] = VALORPROPINA;
                        workRow["VALORINTERES"] = VALORINTERES;
                        workRow["VALORFIJO"] = VALORFIJO;
                        workRow["TIPOPROMOCION"] = TIPOPROMOCION;
                        workRow["MESESGRACIA"] = MESESGRACIA;
                        workRow["EMPRESASERVICIO"] = EMPRESASERVICIO;
                        workRow["ESTADOTRX"] = ESTADOTRX;
                        workRow["CODIGORESPUESTA"] = CODIGORESPUESTA;
                        workRow["TIPODISPOSITIVO"] = TIPODISPOSITIVO;
                        workRow["ADQUIRENTETARJETA"] = ADQUIRENTETARJETA;
                        workRow["ADQUIRENTESERVICIO"] = ADQUIRENTESERVICIO;
                        workRow["MONTOGRAVAIVA"] = MONTOGRAVAIVA;
                        workRow["MONTONOGRAVAIVA"] = MONTONOGRAVAIVA;
                        workRow["PUNTOEMISION"] = PUNTOEMISION;
                        workRow["PROCESADO"] = PROCESADO;
                        workRow["GRUPOTAR"] = GRUPOTAR;
                        workRow["AUTORIZADOR"] = AUTORIZADOR;
                        workRow["LOTE"] = LOTE;
                        workRow["ANULADO"] = ANULADO;
                        workRow["PROCESADOTURNO"] = PROCESADOTURNO;
                        workRow["FACTURA"] = FACTURA;
                        workRow["ARQC"] = ARQC;
                        workRow["AIDEMV"] = AIDEMV;
                        workRow["EMV"] = EMV;
                        workRow["TC"] = TC;
                        workRow["PUBLICIDAD"] = PUBLICIDAD;
                        workRow["TIPOTRANSACCION"] = TIPOTRANSACCION;
                        workRow["BANCOADQUIRIENTE"] = BANCOADQUIRIENTE;
                        workRow["TARJETAHABIENTE"] = TARJETAHABIENTE;
                        workRow["MID"] = MID;
                        workRow["TID"] = TID;
                        workRow["VENCTAR"] = VENCTAR;
                        workRow["ANULAUTORIZACION"] = ANULAUTORIZACION;
                        workRow["TIPOBANCOTARJETA"] = TIPOBANCOTARJETA;
                        tblVoucher.Rows.Add(workRow);

                        var detVoucher = new SqlParameter("@UDT_POS_Voucher", SqlDbType.Structured);
                        detVoucher.TypeName = "dbo.UDT_POS_Voucher";
                        detVoucher.Value = tblVoucher;
                        cmd.Parameters.Add(detVoucher);

                        cmd.CommandTimeout = 0;
                        cmd.Connection = con;
                        
                        dtsConsulta = new DataSet();
                        using (SqlDataReader readerOferta = cmd.ExecuteReader())
                        {
                            while (!readerOferta.IsClosed)
                            {
                                DataTable dt = new DataTable();
                                dt.Load(readerOferta);
                                dtsConsulta.Tables.Add(dt);
                            }
                            readerOferta.Close();
                        }

                    }


                    con.Close();
                    con.Dispose();
                }

                  
                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            objRespuesta.CodigoRespuesta = data["CodigoRespuesta"].ToString();
                            objRespuesta.MensajeRespuesta = data["MensajeRespuesta"].ToString();

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                objRespuesta.CodigoRespuesta = "-1";
                objRespuesta.MensajeRespuesta = "(ProcesaPinPad - Graba Voucher): Error=>" + ex.Message;
                objRespuesta.exception = ex;
                objRespuesta.StackTrace = ex.StackTrace;

            }

            return objRespuesta;
        }

        private static DataTable GetDataTableVoucher()
        {
            DataTable tblDetVoucher = new DataTable();
            tblDetVoucher.Columns.Add("TARJETA", typeof(string));
            tblDetVoucher.Columns.Add("CODIGOPROCESO", typeof(string));
            tblDetVoucher.Columns.Add("FECHACONSUMO", typeof(string));
            tblDetVoucher.Columns.Add("HORACONSUMO", typeof(string));
            tblDetVoucher.Columns.Add("NUMEROVOUCHER", typeof(string));
            tblDetVoucher.Columns.Add("AUTORIZACION", typeof(string));
            tblDetVoucher.Columns.Add("VALORCONSUMO", typeof(string));
            tblDetVoucher.Columns.Add("FORMAAUTORIZA", typeof(string));
            tblDetVoucher.Columns.Add("TIPOCONSUMO", typeof(string));
            tblDetVoucher.Columns.Add("PLAZO", typeof(string));
            tblDetVoucher.Columns.Add("TIPOLECTURA", typeof(string));
            tblDetVoucher.Columns.Add("TIPOMONEDA", typeof(string));
            tblDetVoucher.Columns.Add("VALORIVA", typeof(string));
            tblDetVoucher.Columns.Add("VALORSERVICIO", typeof(string));
            tblDetVoucher.Columns.Add("VALORPROPINA", typeof(string));
            tblDetVoucher.Columns.Add("VALORINTERES", typeof(string));
            tblDetVoucher.Columns.Add("VALORFIJO", typeof(string));
            tblDetVoucher.Columns.Add("TIPOPROMOCION", typeof(string));
            tblDetVoucher.Columns.Add("MESESGRACIA", typeof(string));
            tblDetVoucher.Columns.Add("EMPRESASERVICIO", typeof(string));
            tblDetVoucher.Columns.Add("ESTADOTRX", typeof(string));
            tblDetVoucher.Columns.Add("CODIGORESPUESTA", typeof(string));
            tblDetVoucher.Columns.Add("TIPODISPOSITIVO", typeof(string));
            tblDetVoucher.Columns.Add("ADQUIRENTETARJETA", typeof(string));
            tblDetVoucher.Columns.Add("ADQUIRENTESERVICIO", typeof(string));
            tblDetVoucher.Columns.Add("MONTOGRAVAIVA", typeof(string));
            tblDetVoucher.Columns.Add("MONTONOGRAVAIVA", typeof(string));
            tblDetVoucher.Columns.Add("PUNTOEMISION", typeof(string));
            tblDetVoucher.Columns.Add("PROCESADO", typeof(string));
            tblDetVoucher.Columns.Add("GRUPOTAR", typeof(string));
            tblDetVoucher.Columns.Add("AUTORIZADOR", typeof(string));
            tblDetVoucher.Columns.Add("LOTE", typeof(string));
            tblDetVoucher.Columns.Add("ANULADO", typeof(string));
            tblDetVoucher.Columns.Add("PROCESADOTURNO", typeof(string));
            tblDetVoucher.Columns.Add("FACTURA", typeof(string));
            tblDetVoucher.Columns.Add("ARQC", typeof(string));
            tblDetVoucher.Columns.Add("AIDEMV", typeof(string));
            tblDetVoucher.Columns.Add("EMV", typeof(string));
            tblDetVoucher.Columns.Add("TC", typeof(string));
            tblDetVoucher.Columns.Add("PUBLICIDAD", typeof(string));
            tblDetVoucher.Columns.Add("TIPOTRANSACCION", typeof(string));
            tblDetVoucher.Columns.Add("BANCOADQUIRIENTE", typeof(string));
            tblDetVoucher.Columns.Add("TARJETAHABIENTE", typeof(string));
            tblDetVoucher.Columns.Add("MID", typeof(string));
            tblDetVoucher.Columns.Add("TID", typeof(string));
            tblDetVoucher.Columns.Add("VENCTAR", typeof(string));
            tblDetVoucher.Columns.Add("ANULAUTORIZACION", typeof(string));
            tblDetVoucher.Columns.Add("TIPOBANCOTARJETA", typeof(string));
            return tblDetVoucher;
        }

    }

    
    public class Respuesta {
        public string CodigoRespuesta { get; set; }
        public string MensajeRespuesta { get; set; }
        public string stringVoucher { get; set; }
        public Exception exception { get; set; }
        public string StackTrace { get; set; }


    }
    public class RespuestaVoucher
    {
        public string CodigoRespuesta { get; set; }
        public string MensajeRespuesta { get; set; }
        public string stringVoucher { get; set; }
        public Exception exception { get; set; }
        public string exceptioStr { get; set; }
        public string StackTrace { get; set; }
        public string scriptInsertarVoucher { get; set; }
        public bool fueProcesadoImpresionVocuhers { get; set; }
        public bool EsPagoOkPromoTarjeta { get; set; }
        public string printWarnings { get; set; }
        public decimal valor { get; set; }
        public string tipovoucher { get; set; }
        public string nombreRed { get; set; }
        public string tipoConsumo { get; set; }
        public POS_VOUCHER pos_voucher { get; set; }
        public string voucher { get; set; }

    }

    public class RepuestaPago
    {
        
        private string _TipoTransaccion = string.Empty;
        private string _TipoMensaje = string.Empty;
        private string _CodRespMsj = string.Empty;
        private string _CodRed = string.Empty;
        private string _CodRespMsjAut = string.Empty;
        private string _MsjRespMsjAut = string.Empty;
        private int _SecTrans = 0;
        private string _NumLote = string.Empty;
        private string _HoraTrans = string.Empty;
        private string _MsjImpPremiosPub = string.Empty;
        private int _CodBcoAdq = 0;
        private string _NombBcoAdq = string.Empty;
        private string _GrupoTarjeta = string.Empty;
        private string _ModLectura = string.Empty;
        private string _NombTarjetaHabiente = string.Empty;
        private int _MontoFijo = 0;
        private string _EMV = string.Empty;
        private string _AID_EMV = string.Empty;
        private string _TipoCriptogramaEMV = string.Empty;
        private string _VerificaPIN = string.Empty;
        private string _ARQC = string.Empty;
        private string _TVR = string.Empty;
        private string _TSI = string.Empty;
        private int _FechaVenc = 0;
        private string _NumTarjetaEncript = string.Empty;
        private string _NumTarjetaTrunc = string.Empty;
        private decimal _Valor = 0;
        private string _MID = string.Empty;
        private string _TID = string.Empty;

        public string TipoTransaccion
        {
            get { return _TipoTransaccion; }
            set { _TipoTransaccion = value; }
        }

        public string TipoMensaje
        {
            get { return _TipoMensaje; }
            set { _TipoMensaje = value; }
        }

        
        public string CodRespMsj
        {
            get { return _CodRespMsj; }
            set { _CodRespMsj = value; }
        }

        
        public string CodRed
        {
            get { return _CodRed; }
            set { _CodRed = value; }
        }

        
        public string CodRespMsjAut
        {
            get { return _CodRespMsjAut; }
            set { _CodRespMsjAut = value; }
        }

        
        public string MsjRespMsjAut
        {
            get { return _MsjRespMsjAut; }
            set { _MsjRespMsjAut = value; }
        }

        public int SecTrans
        {
            get { return _SecTrans; }
            set { _SecTrans = value; }
        }

        public string NumLote
        {
            get { return _NumLote; }
            set { _NumLote = value; }
        }

        public string HoraTrans
        {
            get { return _HoraTrans; }
            set { _HoraTrans  = value; }
        }

        private string _FechaTrans = string.Empty;
        public string FechaTrans
        {
            get { return _FechaTrans; }
            set { _FechaTrans = value; }
        }

        

        public string NumAutorizacion
        {
            get { return _NumAutorizacion; }
            set { _NumAutorizacion = value; }
        }

        public string TerminalID
        {
            get { return _TerminalID; }
            set { _TerminalID = value; }
        }

        private string _NumAutorizacion = string.Empty;
        private string _TerminalID = string.Empty;
        private string _MerchantID = string.Empty;
        private int _ValorInteresFin = 0;

        public string MerchantID
        {
            get { return _MerchantID; }
            set { _MerchantID = value; }
        }


    
        public int ValorInteresFin
        {
            get { return _ValorInteresFin; }
            set { _ValorInteresFin = value; }
        }


      
        public string MsjImpPremiosPub
        {
            get { return _MsjImpPremiosPub; }
            set { _MsjImpPremiosPub = value; }
        }


        
        public int CodBcoAdq
        {
            get { return _CodBcoAdq; }
            set { _CodBcoAdq = value; }
        }

       
        public string NombBcoAdq
        {
            get { return _NombBcoAdq; }
            set { _NombBcoAdq = value; }
        }

      
        public string GrupoTarjeta
        {
            get { return _GrupoTarjeta; }
            set { _GrupoTarjeta = value; }
        }

        
        public string ModLectura
        {
            get { return _ModLectura; }
            set { _ModLectura = value; }
        }

       
        public string NombTarjetaHabiente
        {
            get { return _NombTarjetaHabiente; }
            set { _NombTarjetaHabiente = value; }
        }

       
        public int MontoFijo
        {
            get { return _MontoFijo; }
            set { _MontoFijo = value; }
        }


   
        public string EMV
        {
            get { return _EMV; }
            set { _EMV = value; }
        }


        

        public string AID_EMV
        {
            get { return _AID_EMV; }
            set { _AID_EMV = value; }
        }


        
        public string TipoCriptogramaEMV
        {
            get { return _TipoCriptogramaEMV; }
            set { _TipoCriptogramaEMV = value;  }
        }


        

        public string VerificaPIN
        {
            get { return _VerificaPIN; }
            set { _VerificaPIN = value; }
        }

        
        public string ARQC
        {
            get { return _ARQC; }
            set { _ARQC = value; }
        }

        

        public string TVR
        {
            get { return _TVR; }
            set { _TVR = value; }
        }

        

        public string TSI
        {
            get { return _TSI; }
            set { _TSI = value; }
        }

        private string _NumTrajetaPayClub_DBPWallet = string.Empty;
        public string NumTrajetaPayClub_DBPWallet
        {
            get { return _NumTrajetaPayClub_DBPWallet; }
            set { _NumTrajetaPayClub_DBPWallet = value; }
        }




        public int FechaVenc
        {
            get { return _FechaVenc; }
            set { _FechaVenc = value; }
        }

        
        public string NumTarjetaEncript
        {
            get { return _NumTarjetaEncript; }
            set { _NumTarjetaEncript = value; }
        }

        

        public string NumTarjetaTrunc
        {
            get { return _NumTarjetaTrunc; }
            set { _NumTarjetaTrunc = value; }
        }

        public decimal Valor
        {
            get { return _Valor; }
            set { _Valor = value; }
        }

        
        public string MID
        {
            get { return _MID; }
            set { _MID = value; }
        }

        
        public string TID
        {
            get { return _TID; }
            set { _TID = value; }
        }

    


        public string ObtieneDatos {

            set {

            }
        }

    }



}
