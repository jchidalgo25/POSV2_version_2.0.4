using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Control.CajaPinpad.Modelo;

namespace POS.Control.CajaPinpad
{
    public class GeneralPagos
    {

        private static DataSet GetDataSetParametrosPinPad(string Establecimiento = "", string PuntoEmision = "")
        {
            DataSet dtsConsulta = new DataSet();
            string sQuery = string.Empty;
            Establecimiento = string.IsNullOrEmpty(Establecimiento) ? Control.Common.GlobalParameters.Establecimiento : Establecimiento;
            PuntoEmision = string.IsNullOrEmpty(PuntoEmision) ? Control.Common.GlobalParameters.PuntoEmision : PuntoEmision;


            try
            {
                sQuery = string.Concat(sQuery, "Exec spPOSParametrosPinPad ", Environment.NewLine);
                sQuery = string.Concat(sQuery, $@"  @Establecimiento = '{Establecimiento}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $@"  , @PuntoEmision =  '{PuntoEmision}'", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(sQuery);
            }
            catch (Exception ex)
            {
                dtsConsulta = new DataSet();
                return dtsConsulta;
            }



            return dtsConsulta;


        }
        public static DetConsultaPinPad RecuperaDatosPinPad(string Establecimiento = "", string PuntoEmision = "")
        {

            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            DetConsultaPinPad detConsulta = new DetConsultaPinPad();
            Establecimiento = string.IsNullOrEmpty(Establecimiento) ? Control.Common.GlobalParameters.Establecimiento : Establecimiento;
            PuntoEmision = string.IsNullOrEmpty(PuntoEmision) ? Control.Common.GlobalParameters.PuntoEmision : PuntoEmision;
            
            try
            {
                dtsConsulta = GetDataSetParametrosPinPad(Establecimiento, PuntoEmision);

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            bool ValidaRedPinPad = false;
                            bool EsPinPad = false;
                            //PinPadActivoEstab

                            if ((int)data["EsPinPad"] == 1) { EsPinPad = true; }
                            if ((string)data["PinPadActivoEstab"] == "TRUE") { ValidaRedPinPad = true; }

                            Control.Common.GlobalParameters.USA_NUM_INPAD = 1;
                            Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = Int32.Parse(data["IdentificaAutorizadorDefault"].ToString());
                            Control.Common.GlobalParameters.ConectContingente.TiempoEsperaContingente = Int32.Parse(data["TiempoEsperaContingente"].ToString());
                            Control.Common.GlobalParameters.ConectContingente.ValidaRedPinPad = ValidaRedPinPad;

                            Control.Common.GlobalParameters.ConectContingente.EsPinPad = EsPinPad;
                            Control.Common.GlobalParameters.ConectContingente.TiempoOutCP = Int32.Parse(data["TiempoOutCP"].ToString());
                            Control.Common.GlobalParameters.ConectContingente.TiempoOutLT = Int32.Parse(data["TiempoOutLT"].ToString());
                            Control.Common.GlobalParameters.ConectContingente.TiempoOutCT = Int32.Parse(data["TiempoOutCT"].ToString());

                            Control.Common.GlobalParameters.IpPinPadMEDIANET = data["IpPinPadMedianet"].ToString();
                            Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET = data["IpPinPadMedianet"].ToString();
                            Control.Common.GlobalParameters.PuertoPinPadMEDIANET = Int32.Parse(data["PuertoMedianet"].ToString());
                            Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET = Int32.Parse(data["PuertoMedianet"].ToString());

                            Control.Common.GlobalParameters.IpPinPadDATAFAST = data["IpPinPadDataFast"].ToString();
                            Control.Common.GlobalParameters.ConectContingente.IpPinPadDATAFAST = data["IpPinPadDataFast"].ToString();
                            Control.Common.GlobalParameters.PuertoPinPadDataFast = Int32.Parse(data["PuertoDataFast"].ToString());
                            Control.Common.GlobalParameters.ConectContingente.PuertoPinPadDataFast = Int32.Parse(data["PuertoDataFast"].ToString());

                            Control.Common.GlobalParameters.IpPinPadAustro = data["IpPinPadAustro"].ToString();
                            Control.Common.GlobalParameters.ConectContingente.IpPinPadAustro = data["IpPinPadAustro"].ToString();
                            Control.Common.GlobalParameters.PuertoPinPadAustro = Int32.Parse(data["PuertoAustro"].ToString());
                            Control.Common.GlobalParameters.ConectContingente.PuertoPinPadAustro = Int32.Parse(data["PuertoAustro"].ToString());

                            Control.Common.GlobalParameters.MID_MEDIANET = data["MID_MEDIANET"].ToString();
                            Control.Common.GlobalParameters.TID_MEDIANET = data["TID_MEDIANET"].ToString();
                            Control.Common.GlobalParameters.MID_DATAFAST = data["MID_DATAFAST"].ToString();
                            Control.Common.GlobalParameters.TID_DATAFAST = data["TID_DATAFAST"].ToString();
                          
                            detConsulta.Autorizador = data["IdentificaAutorizadorDefault"].ToString();
                            Control.Common.GlobalParameters.ConectContingente.Autorizador = Int32.Parse(detConsulta.Autorizador);
                            Control.Common.GlobalParameters.ConectContingente.IdentificaAutorizadorDefault = Int32.Parse(detConsulta.Autorizador);
                            Control.Common.GlobalParameters.CID = data["CIDPuntoEmision"].ToString();


                            detConsulta.MID_MEDIANET = data["MID_MEDIANET"].ToString();
                            detConsulta.TID_MEDIANET = data["TID_MEDIANET"].ToString();
                            detConsulta.MID_DATAFAST = data["MID_DATAFAST"].ToString();
                            detConsulta.TID_DATAFAST = data["TID_DATAFAST"].ToString();


                            string IPPinPad = string.Empty;
                            int PuertoPinPad = 0;
                            bool EstTcpIpPinpad = false;


                            switch (detConsulta.Autorizador)
                            {
                                case "1":
                                    IPPinPad = data["IpPinPadDataFast"].ToString();
                                    PuertoPinPad = int.Parse(data["PuertoDataFast"].ToString());
                                    break;
                                case "2":
                                    IPPinPad = data["IpPinPadMedianet"].ToString();
                                    PuertoPinPad = int.Parse(data["PuertoMedianet"].ToString());
                                    break;
                            }
                           

                            if (data["EstTcpIpPinpad"].ToString() == "True") { EstTcpIpPinpad = true; }
                            Control.Common.GlobalParameters.IPPinPad = IPPinPad;
                            Control.Common.GlobalParameters.PuertoPinPad = PuertoPinPad;
                            Control.Common.GlobalParameters.EstTcpIpPinpad = EstTcpIpPinpad;

                            Control.Common.GlobalParameters.PINPAD_MULTIRED =
                                data["isPinPadMultiRed"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase);
                            
                        }
                    }
                }


                return detConsulta;

            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message);
                detConsulta = new DetConsultaPinPad();

                return detConsulta;
            }
        }

        public static DetConsultaPinPad ConsultaPrametrosPinPad(string Establecimiento, string PtoEmision)
        {

            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            DetConsultaPinPad detPinnPad = new DetConsultaPinPad();
            Establecimiento = string.IsNullOrEmpty(Establecimiento) ? Control.Common.GlobalParameters.Establecimiento : Establecimiento;
            PtoEmision = string.IsNullOrEmpty(PtoEmision) ? Control.Common.GlobalParameters.PuntoEmision : PtoEmision;

            try
            {
                dtsConsulta = GetDataSetParametrosPinPad(Establecimiento, PtoEmision);

                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            detPinnPad = new DetConsultaPinPad();

                            detPinnPad.CodError = 0;
                            detPinnPad.MsjError = "Consulta de Parametros de PINPAD realizada correctamente";
                            detPinnPad.TID_MEDIANET = data["TID_MEDIANET"].ToString();
                            detPinnPad.MID_MEDIANET = data["MID_MEDIANET"].ToString();
                            detPinnPad.MID_DATAFAST = data["MID_DATAFAST"].ToString();
                            detPinnPad.TID_DATAFAST = data["TID_DATAFAST"].ToString();
                            detPinnPad.MID_AUSTRO = data["MID_AUSTRO"].ToString();
                            detPinnPad.TID_AUSTRO = data["TID_AUSTRO"].ToString();
                            detPinnPad.IpPinPadMedianet = data["IpPinPadMedianet"].ToString();
                            detPinnPad.IpPinPadDataFast = data["IpPinPadDataFast"].ToString();
                            detPinnPad.IpPinPadAustro = data["IpPinPadAustro"].ToString();
                            detPinnPad.IpPtoEmision = data["IpPuntoEmision"].ToString();
                            detPinnPad.PuertoMedianet = Int32.Parse(data["PuertoMedianet"].ToString());
                            detPinnPad.PuertoDataDaFast = Int32.Parse(data["PuertoDataFast"].ToString());
                            detPinnPad.PuertoAustro = Int32.Parse(data["PuertoAustro"].ToString());
                            detPinnPad.LoteMedianet = Int32.Parse(data["LoteMedianet"].ToString());
                            detPinnPad.LoteDataFast = Int32.Parse(data["LoteDataFast"].ToString());

                            bool isPinPadMultiRed = false;
                            if (data["isPinPadMultiRed"].ToString() == "0") { isPinPadMultiRed = true; }
                            detPinnPad.EsPinPadMultiRed = isPinPadMultiRed;


                            //detPinnPad.LoteAustro = Int32.Parse(data["LoteAustro"].ToString();


                        }
                    }

                }

                return detPinnPad;

            }
            catch (Exception ex)
            {
                detPinnPad.CodError = -1;
                detPinnPad.MsjError = "Error: " + ex.Message;
                return detPinnPad;
            }

            return detPinnPad;
        }

        public static DataSet ActualizaConfigPINPAD( string sQuery)
        {
            DataSet dtsConsulta = new DataSet();

            try
            {
                dtsConsulta = new DataSet();
                dtsConsulta = Control.Common.General.GetDataSet(sQuery);
                return dtsConsulta;

            }
            catch (Exception)
            {
                dtsConsulta = new DataSet();
                return dtsConsulta;
            }


        }

        public static string  GrabaDatosVoucher(DetVoucher detVoucher)
        {

            return "";
        }

    }
}
