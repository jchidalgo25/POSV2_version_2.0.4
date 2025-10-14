using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liris_BasePagoDLL.Modelos
{
    
    public class PinPadRespuesta
    {
        public string CodigoRespuestaPinPad { get; set; }
        public string CodigoRespuestaEntidad { get; set; }
        public string MensajeRespuestaPinPad { get; set; }
        public string MensajeRespuestaEntidad { get; set; }
        public string TramaRespuesta { get; set; }
        public string NumBin { get; set; }

        public byte[] dataReceived;
        public string Respkeydrecha { get; set; }
        public string Respkeyizquiera { get; set; }
        public string respuesdescrip2 { get; set; }
        public string tramaRespuesta { get; set; }
        public string PinpadKeyIzquierdoTCPIP { get; set; }
        public string PinpadKeyDerechoTCPIP { get; set; }

        public RepuestaPago GetDatosPago(string TramaRespuesta)
        {
            RepuestaPago objRespuesta = new RepuestaPago();

            string hextamaño = TramaRespuesta.Substring(0, 4);
            objRespuesta.TipoMensaje = TramaRespuesta.Substring(4, 2);
            objRespuesta.CodRespMsj = TramaRespuesta.Substring(6, 2);
            objRespuesta.CodRed = TramaRespuesta.Substring(8, 2);
            objRespuesta.CodRespMsjAut = TramaRespuesta.Substring(10, 2);
            objRespuesta.MsjRespMsjAut = TramaRespuesta.Substring(12, 20);
            objRespuesta.SecTrans = Int32.Parse(TramaRespuesta.Substring(32, 6).ToString());
            objRespuesta.NumLote = TramaRespuesta.Substring(38, 6);
            objRespuesta.HoraTrans = TramaRespuesta.Substring(44, 6).ToString(); //  Int32.Parse(TramaRespuesta.Substring(44, 6).ToString());
            objRespuesta.FechaTrans = TramaRespuesta.Substring(50, 8).ToString(); //  Int32.Parse(TramaRespuesta.Substring(50, 8).ToString());
            objRespuesta.NumAutorizacion = TramaRespuesta.Substring(58, 6).ToString();
            objRespuesta.TerminalID = TramaRespuesta.Substring(64, 8).ToString();
            objRespuesta.MerchantID = TramaRespuesta.Substring(72, 15).ToString();
            objRespuesta.ValorInteresFin = Int32.Parse(TramaRespuesta.Substring(87, 12).ToString());
            objRespuesta.MsjImpPremiosPub = TramaRespuesta.Substring(99, 80).ToString();
            objRespuesta.CodBcoAdq = Int32.Parse(TramaRespuesta.Substring(179, 3).ToString());
            objRespuesta.NombBcoAdq = TramaRespuesta.Substring(182, 30).ToString();
            objRespuesta.GrupoTarjeta = TramaRespuesta.Substring(212, 25).ToString();
            objRespuesta.ModLectura = TramaRespuesta.Substring(237, 2).ToString();
            objRespuesta.NombTarjetaHabiente = TramaRespuesta.Substring(239, 40).ToString();

            int MontoFijo = 0;
            if (TramaRespuesta.Substring(279, 12).ToString().Trim() != "") { MontoFijo = Int32.Parse(TramaRespuesta.Substring(279, 12).ToString()); }
            objRespuesta.MontoFijo = MontoFijo;

            objRespuesta.EMV = TramaRespuesta.Substring(291, 20).ToString();
            objRespuesta.AID_EMV = TramaRespuesta.Substring(311, 20).ToString();
            objRespuesta.TipoCriptogramaEMV = TramaRespuesta.Substring(331, 22).ToString();
            objRespuesta.VerificaPIN = TramaRespuesta.Substring(353, 15).ToString();
            objRespuesta.ARQC = TramaRespuesta.Substring(368, 16).ToString();
            objRespuesta.TVR = TramaRespuesta.Substring(384, 10).ToString();
            objRespuesta.TSI = TramaRespuesta.Substring(394, 4).ToString();
            objRespuesta.NumTrajetaPayClub_DBPWallet = TramaRespuesta.Substring(398, 25).ToString();
            objRespuesta.FechaVenc = Int32.Parse(TramaRespuesta.Substring(423, 4).ToString());
            objRespuesta.NumTarjetaEncript = TramaRespuesta.Substring(427, 64).ToString();
            objRespuesta.NumTarjetaTrunc = TramaRespuesta.Substring(491, 25).ToString();
            return objRespuesta;
        }
        public RepuestaPago GetDatosPago(string TramaRespuesta, string Autorizador)
        {
            RepuestaPago objRespuesta = new RepuestaPago();
            string hextamano = string.Empty;
            int CodBcoAdq = 0;
            int SecTrans = 0;
            string HoraTrans = "";
            string FechaTrans = "";
            int ValorInteresFin = 0;

            try
            {

                
                //var tramaNuevo = TramaRespuesta + componenteAleatorio + ComponenteEncriptado;
                var hextamaño = String.Format("0{0:X}", TramaRespuesta.Length).PadLeft(4, '0');

                if (TramaRespuesta.Substring(0, 2) == "PP")
                {
                    TramaRespuesta = string.Concat(hextamaño, TramaRespuesta);
                }

                hextamano = TramaRespuesta.Substring(0, 4);

                objRespuesta.TipoMensaje = TramaRespuesta.Substring(4, 2);
                objRespuesta.CodRespMsj = TramaRespuesta.Substring(6, 2);
                objRespuesta.CodRed = TramaRespuesta.Substring(8, 2);

                //Codigo de Respuesta de Mensaje
                objRespuesta.CodRespMsjAut = TramaRespuesta.Substring(10, 2);

                //Mensaje de Respuesta
                objRespuesta.MsjRespMsjAut = TramaRespuesta.Substring(12, 20);

                if (objRespuesta.CodRespMsj != "00")
                {

                    return objRespuesta;
                }

                //Secuencial Transacción 
                string CadSecTrans = TramaRespuesta.Substring(32, 6).ToString();
                if (!string.IsNullOrEmpty(CadSecTrans)) { objRespuesta.SecTrans = Int32.Parse(TramaRespuesta.Substring(32, 6).ToString()); }

                //Numero de Lote
                objRespuesta.NumLote = TramaRespuesta.Substring(38, 6);

                //*Hora de la Transaccion
                if (TramaRespuesta.Substring(44, 6).ToString().Trim() != "")
                {
                    //string HoraTransText = TramaRespuesta.Substring(44, 6).ToString().Trim().Count();
                    HoraTrans = TramaRespuesta.Substring(44, 6).ToString();
                    //HoraTrans = Int32.Parse(TramaRespuesta.Substring(44, 6).ToString());

                }
                objRespuesta.HoraTrans = HoraTrans;

                // Fecha de la Transaccion
                if (TramaRespuesta.Substring(50, 8).ToString().Trim() != "")
                {
                    // FechaTrans = Int32.Parse(TramaRespuesta.Substring(50, 8).ToString());
                    FechaTrans = TramaRespuesta.Substring(50, 8).ToString();
                }
                objRespuesta.FechaTrans = FechaTrans;

                //Numero de Autorizacion
                objRespuesta.NumAutorizacion = TramaRespuesta.Substring(58, 6).ToString();

                //TerminalID
                objRespuesta.TerminalID = TramaRespuesta.Substring(64, 8).ToString();

                //Merchant Id 
                objRespuesta.MerchantID = TramaRespuesta.Substring(72, 15).ToString();

                if (TramaRespuesta.Substring(87, 12).ToString().Trim() != "")
                {
                    ValorInteresFin = Int32.Parse(TramaRespuesta.Substring(87, 12).ToString());
                }
                objRespuesta.ValorInteresFin = ValorInteresFin;

                objRespuesta.MsjImpPremiosPub = TramaRespuesta.Substring(99, 80).ToString();

                if (TramaRespuesta.Substring(179, 3).ToString().Trim() != "") { CodBcoAdq = Int32.Parse(TramaRespuesta.Substring(179, 3).ToString()); }
                objRespuesta.CodBcoAdq = CodBcoAdq;

                objRespuesta.NombBcoAdq = TramaRespuesta.Substring(182, 30).ToString();
                objRespuesta.GrupoTarjeta = TramaRespuesta.Substring(212, 25).ToString();
                objRespuesta.ModLectura = TramaRespuesta.Substring(237, 2).ToString();
                objRespuesta.NombTarjetaHabiente = TramaRespuesta.Substring(239, 40).ToString();

                int MontoFijo = 0;
                if (TramaRespuesta.Substring(279, 12).ToString().Trim() != "") { MontoFijo = Int32.Parse(TramaRespuesta.Substring(279, 12).ToString()); }
                objRespuesta.MontoFijo = MontoFijo;

                objRespuesta.EMV = TramaRespuesta.Substring(291, 20).ToString();
                objRespuesta.AID_EMV = TramaRespuesta.Substring(311, 20).ToString();
                objRespuesta.TipoCriptogramaEMV = TramaRespuesta.Substring(331, 22).ToString();
                objRespuesta.VerificaPIN = TramaRespuesta.Substring(353, 15).ToString();
                objRespuesta.ARQC = TramaRespuesta.Substring(368, 16).ToString();
                objRespuesta.TVR = TramaRespuesta.Substring(384, 10).ToString();
                objRespuesta.TSI = TramaRespuesta.Substring(394, 4).ToString();
                objRespuesta.NumTrajetaPayClub_DBPWallet = TramaRespuesta.Substring(398, 25).ToString();

                int FechaVenc = 0;
                if (TramaRespuesta.Substring(423, 4).ToString().Trim() != "") { FechaVenc = Int32.Parse(TramaRespuesta.Substring(423, 4).ToString().Trim()); }
                objRespuesta.FechaVenc = FechaVenc;

                // objRespuesta.FechaVenc = Int32.Parse(TramaRespuesta.Substring(423, 4).ToString());
                objRespuesta.NumTarjetaEncript = TramaRespuesta.Substring(427, 64).ToString();
                objRespuesta.NumTarjetaTrunc = TramaRespuesta.Substring(491, 25).ToString();
                return objRespuesta;
            }
            catch (Exception ex)
            {
                return objRespuesta;
            }

        }
        public PinPadRespuesta LecturaTarjeta(string IP, int Puerto, int timeout, string trama, string rutabines, int grabalog, string TipoTrans = "")
        {
            PinPadRespuesta conexionRespuesta = new PinPadRespuesta();

            try
            {
                ClibSecurity.ClsSecurity objetoSecurity = new ClibSecurity.ClsSecurity();
                var componenteAleatorio = objetoSecurity.GeneraComponente();
                var ComponenteEncriptado = objetoSecurity.F3DESEncriptar(PinpadKeyIzquierdoTCPIP, componenteAleatorio, PinpadKeyDerechoTCPIP);

                var tramaNuevo = trama + componenteAleatorio + ComponenteEncriptado;
                var hextamaño = String.Format("0{0:X}", tramaNuevo.Length).PadLeft(4, '0');
                var TramaFinal = hextamaño + tramaNuevo;

                ClibcommunicateSocket.ClsComunicaSocket objconmu = new ClibcommunicateSocket.ClsComunicaSocket();
                var TramaFinalByte = Encoding.ASCII.GetBytes(TramaFinal);

                //int PinPadReceiveTimeout = Control.Common.GlobalParameters.PinPadReceiveTimeout;
                int PinPadReceiveTimeout = timeout;

                if (string.IsNullOrEmpty(TipoTrans)) { TipoTrans = "LT"; }

                //Conexion.Connectar(Control.Common.GlobalParameters.IPPinPad, Control.Common.GlobalParameters.PuertoPinPad, TramaFinalByte, Control.Common.GlobalParameters.PinPadReceiveTimeout);
                conexionRespuesta = Conexion.ConnectPinPad(IP, Puerto, TramaFinalByte, PinPadReceiveTimeout, TipoTrans);

                if (conexionRespuesta.CodigoRespuestaPinPad != "00")
                {
                    return conexionRespuesta;
                }

                byte[] getDataRecived = conexionRespuesta.dataReceived;
                var RespuestaPinpad = ProcessData.hex2AsciiStr(ProcessData.byte2hex(getDataRecived));
                int leerTramaRes = 75;

                if (RespuestaPinpad.Length > 98)
                {
                    leerTramaRes = leerTramaRes + 24;
                }

                string CabTrama = RespuestaPinpad.Substring(0, RespuestaPinpad.IndexOf(TipoTrans, 0)).ToString();
                int IndexCabTrama = RespuestaPinpad.IndexOf(TipoTrans, 0) + 2;

                conexionRespuesta.CodigoRespuestaPinPad = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();

                switch (TipoTrans)
                {
                    case "LT":
                        conexionRespuesta.CodigoRespuestaPinPad = RespuestaPinpad.Substring(IndexCabTrama, 2).ToString();
                        conexionRespuesta.MensajeRespuestaPinPad = RespuestaPinpad.Substring(leerTramaRes + 4, 10).ToString();
                        conexionRespuesta.NumBin = RespuestaPinpad.Substring(IndexCabTrama + 4, 6).ToString();
                        break;
                    case "CT":
                        conexionRespuesta.NumBin = RespuestaPinpad.Substring(IndexCabTrama + 66, 8).ToString();
                        conexionRespuesta.CodigoRespuestaPinPad = RespuestaPinpad.Substring(6, 2).ToString();
                        conexionRespuesta.MensajeRespuestaPinPad = RespuestaPinpad.Substring(84, 20).ToString();

                        break;

                }


                return conexionRespuesta;
            }
            catch (Exception ex)
            {
                conexionRespuesta.CodigoRespuestaPinPad = "20";
                conexionRespuesta.MensajeRespuestaPinPad = "error en LecturaTarjeta: " + ex.InnerException.Message;
                return conexionRespuesta;
            }
            //ObtenerRespuestaPinPad



        }

    }

    public class ConexionContingente
    {
        public bool ValidaRedPinPad { get; set; } = false;
        public bool PinPadActivoEstab { get; set; } = false;
        public bool PinPadContingente { get; set; } = false;
        public int CodigoAutorizador { get; set; }
        public int IdentificaAutorizadorDefault { get; set; }
        public int Autorizador { get; internal set; }
        public DateTime FechaInicioEspera { get; set; }
        public DateTime FechaFinEspera { get; set; }
        public int TiempoEsperaContingente { get; set; }
        public bool EsPinPad { get; set; }
        public bool PinpadMediaNet { get; set; }
        public bool VoucherSinFirma { get; set; }
        public int TiempoOutCP { get; set; }
        public int TiempoOutLT { get; set; }
        public int TiempoOutCT { get; set; }
        public string IpPinPadMEDIANET { get; set; }
        public int PuertoPinPadMEDIANET { get; set; }
        public string IpPinPadDATAFAST { get; internal set; }
        public int PuertoPinPadDataFast { get; set; }
        public string IpPinPadAustro { get; set; }
        public int PuertoPinPadAustro { get; set; }
        public string MID_MEDIANET { get; internal set; }
        public string TID_MEDIANET { get; internal set; }
        public string MID_DATAFAST { get; internal set; }
        public string TID_DATAFAST { get; internal set; }
        public string MID_AUSTRO { get; internal set; }
        public string TID_AUSTRO { get; internal set; }
    }

    public class DetConsultaPinPad
    {
        public string Autorizador { get; set; }
        public string RedAutorizador { get; set; }
        public string MID_MEDIANET { get; set; }
        public string TID_MEDIANET { get; set; }
        public string MID_DATAFAST { get; set; }
        public string TID_DATAFAST { get; set; }
        public string TipoTransaccion { get; set; }
        public string TipoPago { get; set; }


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
            set { _HoraTrans = value; }
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
            set { _TipoCriptogramaEMV = value; }
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

        public string ObtieneDatos
        {
            set
            {

            }
        }

    }

    public class UDT_GeneraTrama
    {
        private static string _TipoProceso = string.Empty;
        private static string _TipoTransaccion = string.Empty;
        private static int _Autorizador = 0;
        private static string _codDiferido = string.Empty;
        private static string _plazoDiferido = string.Empty;
        private static string _mesesGracia = string.Empty;
        private static int _validaTarjeta = 0;
        private static decimal _montoTotalTransaccion = 0;
        private static decimal _montoBaseIVA = 0;
        private static decimal _montoBaseCERO = 0;
        private static decimal _valorIVA = 0;
        private static string _imptoServTrans = string.Empty;
        private static string _propinaTrans = string.Empty;
        private static string _montoFijo = string.Empty;
        private static string _secTransaccion = string.Empty;
        private static string _horaTransaccion = string.Empty;
        private static string _fechaTransaccion = string.Empty;
        private static string _numAutorizacion = string.Empty;
        private static string _MID = string.Empty;
        private static string _TID = string.Empty;
        private static string _tramaRespuesta = string.Empty;

        public static string TipoProceso
        {
            get { return _TipoProceso; }
            set { _TipoProceso = value; }
        }
        public static string TipoTransaccion
        {
            get { return _TipoTransaccion; }
            set { _TipoTransaccion = value; }
        }
        public static int Autorizador
        {
            get { return _Autorizador; }
            set { _Autorizador = value; }
        }
        public static string codDiferido
        {
            get { return _codDiferido; }
            set { _codDiferido = value; }
        }
        public static string plazoDiferido
        {
            get { return _plazoDiferido; }
            set { _plazoDiferido = value; }
        }
        public static string mesesGracia
        {
            get { return _mesesGracia; }
            set { _mesesGracia = value; }
        }
        public static int validaTarjeta
        {
            get { return _validaTarjeta; }
            set { _validaTarjeta = value; }
        }
        public static decimal montoTotalTransaccion
        {
            get { return _montoTotalTransaccion; }
            set { _montoTotalTransaccion = value; }
        }
        public static decimal montoBaseIVA
        {
            get { return _montoBaseIVA; }
            set { _montoBaseIVA = value; }
        }
        public static decimal montoBaseCERO
        {
            get { return _montoBaseCERO; }
            set { _montoBaseCERO = value; }
        }
        public static decimal valorIVA
        {
            get { return _valorIVA; }
            set { _valorIVA = value; }
        }
        public static string imptoServTrans
        {
            get { return _imptoServTrans; }
            set { _imptoServTrans = value; }
        }
        public static string propinaTrans
        {
            get { return _propinaTrans; }
            set { _propinaTrans = value; }
        }
        public static string montoFijo
        {
            get { return _montoFijo; }
            set { _montoFijo = value; }
        }
        public static string secTransaccion
        {
            get { return _secTransaccion; }
            set { secTransaccion = value; }
        }
        public static string horaTransaccion
        {
            get { return _horaTransaccion; }
            set { _horaTransaccion = value; }
        }
        public static string fechaTransaccion
        {
            get { return _fechaTransaccion; }
            set { _fechaTransaccion = value; }
        }
        public static string numAutorizacion
        {
            get { return _numAutorizacion; }
            set { _numAutorizacion = value; }
        }
        public static string MID
        {
            get { return _MID; }
            set { _MID = value; }
        }
        public static string TID
        {
            get { return _TID; }
            set { _TID = value; }
        }
        public static string tramaRespuesta
        {
            get { return _tramaRespuesta; }
            set { _tramaRespuesta = value; }
        }



    }




}
