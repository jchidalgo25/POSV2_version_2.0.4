using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjecutaPagoDLL
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
        public string CodigoRespuesta { get; set; } = "";
        public string CodigoRespuestaEntidad { get; set; }
        public string MensajeRespuesta { get; set; }
        public string MensajeRespuestaEntidad { get; set; }
        public string TramaRespuesta { get; set; }
        public string NumBin { get; set; }

        public byte[] dataReceived;
        public string Respkeydrecha { get; set; }
        public string Respkeyizquiera { get; set; }
        public string respuesdescrip2 { get; set; }
        
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
        public string IpPinPadDataFast { get; set; }
        public string IpPinPadAustro { get; set; }
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
