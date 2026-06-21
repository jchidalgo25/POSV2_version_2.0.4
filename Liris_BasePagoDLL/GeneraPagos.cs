using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liris_BasePagoDLL.Modelos;

namespace Liris_BasePagoDLL
{
 
    public class GeneraPagos
    {
        public bool AplicaDescuentoPromoBines { get; set; }
        public bool ActivaVersionPinPadMedianet { get; set; }
        public bool EsPinPad { get; set; }
        public decimal valorPago { get; set; }
        public string IpPinPadMEDIANET { get; set; }
        public int PuertoPinPadMEDIANET { get; set; }

        string LoggerText = string.Empty;

        public GeneraPagos()
        {
            LoggerText = string.Empty;

        }


        public PinPadRespuesta ProcesaPinpadBackgroundMultiRed_UDT()
        {
            string autorizador = "2";

            string bin_descripcion = string.Empty;
            string bin_red = string.Empty;
            string MID = string.Empty;
            string TID = string.Empty;
            string CID = string.Empty;
            string IPPinPad = string.Empty;
            int PuertoPinPad = 0;

            PinPadRespuesta resultadolectura = new PinPadRespuesta();
            

            try
            {
                Logger.LogMessage(Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed_UDT", " Ejecuta proceso ProcesaPinpadV2 ");
                
                if (EsPinPad)
                {
                    //decimal valorPagoPinpad = Decimal.Parse(valorPago);

                    if (AplicaDescuentoPromoBines || !ActivaVersionPinPadMedianet)
                    {
                        Logger.LogMessage(Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed_UDT", " Se invoca General.ValidaContingente, en caso de que el servicio de Medianet se ha reestablecido o el tiempo para generar pruebas ha concluido");
                        // Control.Common.General.ValidaContingente();

                        IPPinPad = IpPinPadMEDIANET;
                        PuertoPinPad = PuertoPinPadMEDIANET;

                        Logger.LogMessage(Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed_UDT", " IP PinPad: " + IPPinPad + "; PuertoPinPad: " + PuertoPinPad);
                        Logger.LogMessage(Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed_UDT",
                                                                    " GlobalParameters.IPPinPad: " + IPPinPad + "; GlobalParameters.PuertoPinPad: " + PuertoPinPad);

                        Logger.LogMessage(Enum.LogTypes.Info, "BasePagos", "ProcesaPinpadBackgroundMultiRed_UDT", " Lectura Pinpad ");
                        resultadolectura = resultadolectura.LecturaTarjeta(IPPinPad, PuertoPinPad, 65000, "LT", "", 1);


                        //Respuesta del PinPad
                        if (resultadolectura.CodigoRespuestaPinPad == "00")
                        {


                        }


                    }


                }


            }
            catch (Exception ex)
            {

                throw;
            }

            return resultadolectura;

        }


        public PinPadRespuesta ProcesaPagoPinPadMultiRed()
        {
            PinPadRespuesta objRespuesta = new PinPadRespuesta();

            try
            {





            }
            catch (Exception ex)
            {

                objRespuesta.CodigoRespuestaPinPad = "-1";
                objRespuesta.CodigoRespuestaPinPad = "Error: ProcesaPagoPinPadMultiRed - " + ex.Message;
                return objRespuesta;

            }

            return objRespuesta;
        }


    }
}
