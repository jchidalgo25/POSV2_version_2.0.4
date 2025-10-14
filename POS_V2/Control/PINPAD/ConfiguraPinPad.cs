using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Control.ToolBox;
using POS.Models;
using POS.Control.CajaPinpad.Modelo;
using System.Net;
using System.Net.Sockets;

namespace POS.Control.PINPAD
{
    public partial class ConfiguraPinPad : Telerik.WinControls.UI.RadForm
    {
        private ToolBoxMenu _ToolBoxMenu;
        private MainWindow _mainWindow;
        private Factura _factura;
        private string ipAddress = string.Empty;
        private string nombrePC = string.Empty;
        
        public ConfiguraPinPad(ToolBoxMenu toolBoxMenu, MainWindow mainWindow)
        {
            _ToolBoxMenu = toolBoxMenu;
            _mainWindow = mainWindow;
            _factura = mainWindow.FacturaActual;

            InitializeComponent();

            txtIpPinPadMedianet.MaxLength = 0;
            txtIpPinPadDataFast.MaxLength = 0;

            _ToolBoxMenu.Hide();

        }


        private void ConfiguraPinPad_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            //this.Close();
        }

        private void ConfiguraPinPad_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ToolBoxMenu.Show();
        }

        private void radButton5_Click(object sender, EventArgs e)
        {

        }

     

        private void ConfiguraPinPad_Load(object sender, EventArgs e)
        {
            EjecutaConsulta();



        }

        private void chkPinPadMultiRed_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {

            
        }

        private void btnGuardarConfigPinPad_Click(object sender, EventArgs e)
        {

            DetConsultaPinPad result = GrabaConfiguracionPINPAD();
            if (result.CodError == 0)
            {
                MessageBox.Show("Configuración de PINPAD actualizada correctamente.", "Configuración de PINPAD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else {
                MessageBox.Show("Error: " + result.MsjError, "Configuración de PINPAD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ConfiguraPINPAD", "btnGuardarConfigPinPad_Click", "result: " + result.CodError
                                                                                                + result.MsjError);

        }

        private DetConsultaPinPad GrabaConfiguracionPINPAD()
        {
            DetConsultaPinPad detConsultaPinPad = new DetConsultaPinPad();

            string sQuery = string.Empty;
            DataSet dtsConsulta = new DataSet();
            string IpPuntoEmision = txtIpPuntoEmision.Text;

            string IpPinPadMedianet = txtIpPinPadMedianet.Text;
            string MID_MEDIANET = txt_MID_MEDIANET.Text;
            string TID_MEDIANET = txtTID_MEDANET.Text;

            int PuertoMedianet = 0; // 

            string IpPinPadDataFast = txtIpPinPadDataFast.Text;
            string MID_DATAFAST = txt_MID_DATAFAST.Text;
            string TID_DATAFAST = txt_TID_DATAFAST.Text;
            int PuertoDataFast = 0; // 

            int LoteMedianet = 0;
            int LoteDataFast = 0;

            int EsMultiRed = 0;
            int chkEstadoPinPad = 0;

            if (!string.IsNullOrEmpty(txtPuertoMedianet.Text))
            {
                PuertoMedianet = Int32.Parse(txtPuertoMedianet.Text);
            }
            else {
                MessageBox.Show("Puerto Medianet no valido", "Ejecuta PINPAD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                detConsultaPinPad.CodError = -1;
                detConsultaPinPad.MsjError = "Puerto Medianet no valido";
                return detConsultaPinPad;
            }

            if (!string.IsNullOrEmpty(txtPuertoDatafast.Text))
            {
                PuertoDataFast = Int32.Parse(txtPuertoDatafast.Text);
            }

            if (this.chkPinPadMultiRed.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                EsMultiRed = 1;
            }

            if (this.chkEstadoPinPad.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                chkEstadoPinPad = 1;
            }


            try
            {

                sQuery = string.Empty;
                sQuery = string.Concat(sQuery, "Exec spPOSActualizaConfigPINPAD", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  @IpPuntoEmision = '{IpPuntoEmision}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @Establecimiento = '{_factura.Establecimiento}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @PuntoEmision = '{_factura.PtoEmision}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @IpPinPadMedianet = '{IpPinPadMedianet}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @MID_MEDIANET = '{MID_MEDIANET}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @TID_MEDIANET = '{TID_MEDIANET}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @PuertoMedianet = {PuertoMedianet}", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @LoteMedianet = {LoteMedianet}", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @IpPinPadDataFast = '{IpPinPadDataFast}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @MID_DATAFAST = '{MID_DATAFAST}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @TID_DATAFAST = '{TID_DATAFAST}'", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @PuertoDataFast = {PuertoDataFast}", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @LoteDataFast = {LoteDataFast}", Environment.NewLine);
                sQuery = string.Concat(sQuery, $"  , @EsMultiRed = {EsMultiRed}", Environment.NewLine);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ConfiguraPINPAD", "GrabaConfiguracionPINPAD", "Ejecuta script: " + sQuery);

                dtsConsulta = Control.CajaPinpad.GeneralPagos.ActualizaConfigPINPAD(sQuery);
                if (dtsConsulta.Tables.Count > 0)
                {
                    if (dtsConsulta.Tables[0].Rows.Count > 0) {

                        foreach (DataRow data in dtsConsulta.Tables[0].Rows)
                        {
                            detConsultaPinPad.CodError = Int32.Parse(data["CodError"].ToString());
                            detConsultaPinPad.MsjError = data["MsjError"].ToString();
                        }
                    }
                }

                return detConsultaPinPad;
            }
            catch (Exception ex)
            {
                detConsultaPinPad = new DetConsultaPinPad();
                detConsultaPinPad.CodError = -1;
                detConsultaPinPad.MsjError = "Error: " + ex.Message;
                return detConsultaPinPad;
            }

        }
        private void EjecutaConsulta()
        {
            DetConsultaPinPad objPinPad = new DetConsultaPinPad();

            try
            {
                //POS.Control.CajaPinpad
                //_factura.Establecimiento
                var consultaPinPad = Control.CajaPinpad.GeneralPagos.ConsultaPrametrosPinPad(_factura.Establecimiento, _factura.PtoEmision);

                this.txtIpPuntoEmision.Text = consultaPinPad.IpPtoEmision;
                this.txtIpPinPadMedianet.Text = consultaPinPad.IpPinPadMedianet;
                this.txtTID_MEDANET.Text = consultaPinPad.TID_MEDIANET;
                this.txt_MID_MEDIANET.Text = consultaPinPad.MID_MEDIANET;
                this.txtPuertoMedianet.Text = consultaPinPad.PuertoMedianet.ToString();
                this.txtLoteMedianet.Text = consultaPinPad.LoteMedianet.ToString();

                this.txtIpPinPadDataFast.Text = consultaPinPad.IpPinPadDataFast;
                this.txt_TID_DATAFAST.Text = consultaPinPad.TID_DATAFAST;
                this.txt_MID_DATAFAST.Text = consultaPinPad.MID_DATAFAST;
                this.txtPuertoDatafast.Text = consultaPinPad.PuertoDataDaFast.ToString();
                this.txtLoteDataFast.Text = consultaPinPad.LoteDataFast.ToString();

                //radCheckBox1.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
                this.chkEstadoPinPad.Checked = true;
                this.chkPinPadMultiRed.Checked = consultaPinPad.EsPinPadMultiRed;


            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "ConfiguraPinPad", "EjecutaConsulta", "Error: " + ex.Message);

            }


        }

    }
}
