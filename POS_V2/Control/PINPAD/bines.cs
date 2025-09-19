using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.PINPAD
{
    public class bines
    {
        private int codError = 0;
        public int CodError
        {
            get { return codError; }
            set { codError = value; }
        }

        private string msjError = string.Empty;
        public string MsjError
        {
            get { return msjError; }
            set { msjError = value; }
        }

        private int numeroBin = 0;
        public int NumeroBin
        {
            get { return numeroBin; }
            set { numeroBin = value; }
        }

        private bool binBloqueado = false;
        public bool BinBloqueado
        {
            get { return binBloqueado; }
            set { binBloqueado = value; }
        }

        private string txtValidacionBloqueo = string.Empty;
        public string TxtValidacionBloqueo
        {
            get { return txtValidacionBloqueo; }
            set { txtValidacionBloqueo = value; }
        }

        private string txtAutorizador = string.Empty;
        public string TxtAutorizador
        {
            get { return txtAutorizador; }
            set { txtAutorizador = value; }
        }

        private int autorizador = 0;
        public int Autorizador
        {
            get { return autorizador; }
            set { autorizador = value; }
        }

        private int binRed = 0;
        public int BinRed
        {
            get { return binRed; }
            set { binRed = value; }
        }

        private int binRedCred = 0;
        public int BinRedCred
        {
            get { return binRedCred; }
            set { binRedCred = value; }
        }

        private int idMensaje = 0;
        public int IdMensaje
        {
            get { return idMensaje; }
            set { idMensaje = value; }
        }

        private string txtDsctBin = string.Empty;
        public string TxtDsctBin
        {
            get { return txtDsctBin; }
            set { txtDsctBin = value; }
        }

        private decimal _porcDsctBin = 0;
        public decimal porcDsctoBin
        {
            get { return _porcDsctBin; }
            set { _porcDsctBin = value; }
        }

        private bool _binTieneDescuento = false;
        public bool binTieneDescuento
        {
            get { return _binTieneDescuento; }
            set { _binTieneDescuento = value; }
        }

        private bool _establecimientoPromo = false;
        public bool establecimientoPromo
        {
            get { return _establecimientoPromo; }
            set { _establecimientoPromo = value; }
        }
        private bool _tieneArtParticipantesBines = false;
        public bool tieneArtParticipantesBines
        {
            get { return _tieneArtParticipantesBines; }
            set { _tieneArtParticipantesBines = value; }
        }

        private string _itemsParticipantesPromoBines = string.Empty;
        public string itemsParticipantesPromoBines
        {
            get { return _itemsParticipantesPromoBines; }
            set { _itemsParticipantesPromoBines = value; }
        }


        public static bines validaBines(string numeroBin, string establecimiento)
        {
            bines bines = new bines();
            string query = string.Empty;
            DataSet dtsConsulta = new DataSet();

            try
            {

                query = string.Empty;
                query = string.Concat(query, "Exec spValidacionBines", Environment.NewLine);
                query = string.Concat(query, $"  @establecimiento = '{establecimiento}' ", Environment.NewLine);
                query = string.Concat(query, $" , @numeroBin = {numeroBin} ", Environment.NewLine);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "validaBines: ", query);


                dtsConsulta = Control.Common.General.GetDataSet(query);


                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0)
                    {

                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            bines = new bines();
                            bines.CodError = Int32.Parse(data["codError"].ToString());
                            bines.msjError = data["msjError"].ToString();
                            

                            if (bines.CodError != 0)
                            {
                                return bines;
                            }

                            bines.txtValidacionBloqueo = data["txtValidacionBloqueo"].ToString();
                            bines.numeroBin = Int32.Parse(data["numeroBin"].ToString());
                            bines.binBloqueado = (bool)data["binBloqueado"];
                        
                            bines.binTieneDescuento = (bool)data["binTieneDescuento"];
                            bines.porcDsctoBin = decimal.Parse(data["porcDsctoBin"].ToString());
                            bines.txtDsctBin = data["txtDsctBin"].ToString();

                            bines.binRed = Int32.Parse(data["binRed"].ToString());
                            bines.binRedCred = Int32.Parse(data["binRed"].ToString());
                            bines.autorizador = Int32.Parse(data["autorizador"].ToString());
                            bines.idMensaje = Int32.Parse(data["idMensaje"].ToString());
                            bines.txtDsctBin = data["txtDsctBin"].ToString();
                            bines.establecimientoPromo = (bool)data["binTieneDescuento"];
                            bines.tieneArtParticipantesBines = (bool)data["tieneArtParticipantesBines"];
                            bines.itemsParticipantesPromoBines = data["itemsParticipantesPromoBines"].ToString();

                        }

                        return bines;


                    }
                }



            }
            catch (Exception ex)
            {
                bines = new bines();
                bines.CodError = -1;
                bines.msjError = $"Error: {ex.Message}";
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "validaBines: ", bines.msjError);
                return bines;

            }

            return bines;

        }
    }

  

}
