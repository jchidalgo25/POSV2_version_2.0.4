using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.CajaPinpad.Modelo
{
    public enum Autorizador
    {
        Default = 0,
        DataFast = 1,
        MediaNet = 2,
        Austro = 3
    }
    public class PinPadRespuesta
    {
        public string _CodigoRespuesta { get; set; }
        public string CodigoRespuesta
        {
            get { return string.IsNullOrEmpty(_CodigoRespuesta) ? "" : _CodigoRespuesta; }
            set { _CodigoRespuesta = string.IsNullOrEmpty(value) ? "" : value; }
            
        }
        public string _CodigoRespuestaEntidad { get; set; }
        public string CodigoRespuestaEntidad
        {
            get { return string.IsNullOrEmpty(_CodigoRespuestaEntidad) ? "" : _CodigoRespuestaEntidad; }
            set { _CodigoRespuestaEntidad = string.IsNullOrEmpty(value) ? "" : value; }
        }
        public string _MensajeRespuesta { get; set; }
        public string MensajeRespuesta
        {
            get { return string.IsNullOrEmpty(_MensajeRespuesta) ? "" : _MensajeRespuesta; }
            set { _MensajeRespuesta = string.IsNullOrEmpty(value) ? "" : value; }
        }
        public string _MensajeRespuestaEntidad { get; set; }
        public string MensajeRespuestaEntidad
        {
            get { return string.IsNullOrEmpty(_MensajeRespuestaEntidad) ? "" : _MensajeRespuestaEntidad; }
            set { _MensajeRespuestaEntidad = string.IsNullOrEmpty(value) ? "" : value; }
        }

        public string TramaRespuesta { get; set; }
        public string NumeroTarjeta { get; set; }

        public string NumBin { get; set; }

        public  byte[] dataReceived;
        public string Respkeydrecha { get; set; }
        public string Respkeyizquiera { get; set; }
        public string respuesdescrip2 { get; set; }
        public RepuestaPago GetDatosPago(string TramaRespuesta) {
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

                if (objRespuesta.CodRespMsj != "00") {

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
                if (TramaRespuesta.Substring(50, 8).ToString().Trim() != "") {
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

                if (TramaRespuesta.Substring(87, 12).ToString().Trim() != "") {
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

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeDatosPago", "Recupera datos desde la trama de respuesta");
                return objRespuesta;
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeDatosPago", "Error: " + ex.Message);
                return objRespuesta;
            }

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
        public  int PuertoPinPadMEDIANET { get; set; }
        public  string IpPinPadDATAFAST { get; internal set; }
        public  int PuertoPinPadDataFast { get; set; }
        public  string IpPinPadAustro { get; set; }
        public  int PuertoPinPadAustro { get; set; }
        public  string MID_MEDIANET { get; internal set; }
        public  string TID_MEDIANET { get; internal set; }
        public  string MID_DATAFAST { get; internal set; }
        public  string TID_DATAFAST { get; internal set; }
        public  string MID_AUSTRO { get; internal set; }
        public  string TID_AUSTRO { get; internal set; }


    }

    public class DetConsultaPinPad
    {
        public int CodError { get; set; }
        public string MsjError { get; set; }
        public string Autorizador { get; set; }
        public string RedAutorizador { get; set; }
        public string MID_MEDIANET { get; set; }
        public string TID_MEDIANET { get; set; }
        public string MID_DATAFAST { get; set; }
        public string TID_DATAFAST { get; set; }
        public string MID_AUSTRO { get; set; }
        public string TID_AUSTRO { get; set; }
        public string TipoTransaccion { get; set; }
        public string TipoPago { get; set; }
        public string IpPinPadMedianet { get; set; }
        public string IpPinPadDataFast{ get; set; }
        public string IpPinPadAustro{ get; set; }
        public string IpAddressPOS { get; set; }
        public int PuertoMedianet { get; set; }
        public int PuertoDataDaFast { get; set; }
        public int PuertoAustro { get; set; }
        public string IpPtoEmision { get; set; }

        public int LoteMedianet { get; set; }
        public int LoteDataFast { get; set; }

        public bool EsPinPadMultiRed { get; set; }

    }



}
