using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Control.PINPAD;
using Trx.Messaging;
using Trx.Utilities;
using System.IO.Ports;
using POS.Models;
using System.IO;
using System.Net.Mail;
using System.Net;
using WinSCP;
using POS.Control.Pagos;
using System.Globalization;
using MensajesLibrary;
using POS.Control.CajaPinpad.Modelo;

namespace POS.Control.PINPAD
{

    public partial class PagoPINPAD : Telerik.WinControls.UI.RadForm
    {
        const int PUERTOCOM = 9;
        Factura _factura;
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        private DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();
        

 
        public PagoPINPAD(ref Factura f)
        {
            InitializeComponent();
            _factura = f;
        }

        private void PagoPINPAD_Load(object sender, EventArgs e)
        {

            //   RESPUESTA = Envio.Envio_requerimientoPinpad(IP, Puerto, timeout, trama, NULL, iGrabaLog)
            using (var db = new POSEntities())
            {
                cmbTipoTransaccion.DataSource = db.pos_tarjeta_transaccion.ToList();
                cmbTipoTransaccion.DataMember = "idTipo";
                cmbTipoTransaccion.DisplayMember = "NombreTarjetaTransaccion";

            }


            lblFacReimpresionAyuda.Text = _factura.Documento + "-" + _factura.Establecimiento.PadLeft(3, '0') + "-" + _factura.PtoEmision.PadLeft(3, '0') + "-";
            lblFacReimpresion.Text = lblFacReimpresionAyuda.Text + String.Empty.PadLeft(9, '0');
        }



        private void EjecutaLecturaConsultaPINPAD(string TipoConsulta) {

            ClsEnviaPinPadGeneral envioGen = new ClsEnviaPinPadGeneral();
            ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
            var pos = new POSEntities();
            string IPPinPad = string.Empty;
            int PuertoPinPad = 0;
            PinPadRespuesta resultadolectura = new PinPadRespuesta();


            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "ConsultaTarjeta", "PagoPINPAD | TRAMA: Consulta Tarjeta ");


            txtResult.Text = "";
            int timeOutCT = 40000;

            if (TipoConsulta == "CT")
            {
                int.TryParse(pos.core_parametro.Where(x => x.identificador == "CT_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutCT);
            }

            if (TipoConsulta == "LT")
            {
                int.TryParse(pos.core_parametro.Where(x => x.identificador == "LT_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutCT);
            }


            //txtResult.Text = envio.SendRequestPinpad("", PUERTOCOM, timeOutCT, "CT", "", 1);

            IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
            PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;
            
            resultadolectura = envioGen.LecturaTarjeta(IPPinPad, PuertoPinPad, 65000, TipoConsulta, "", 1);
            //resultadolectura = envioGen.LecturaTarjeta(IPPinPad, PuertoPinPad, 65000, "LT", "", 1);

            if (resultadolectura.CodigoRespuesta != "00")
            {
                string CodigoRespuesta = resultadolectura.CodigoRespuesta;
                string MensajeRespuesta = resultadolectura.MensajeRespuesta;

                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[CodigoRespuesta]", valor = CodigoRespuesta });
                parametros.Add(new ParametrosMensajes() { codigo = "[MensajeRespuesta]", valor = MensajeRespuesta });
                Control.Common.General.GetMensajeToList(530);
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Error al consultar la Tarjeta, favor revisar. Cod (" + CodigoRespuesta + ") " + MensajeRespuesta);

            }

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "ConsultaTarjeta", "EjecutaLecturaConsultaPINPAD | Termina  ");
        }

        private void btnConsultaTarjeta_click(object sender, EventArgs e)
        {

            EjecutaLecturaConsultaPINPAD("CT");

        }

        private void btnLecturaTarjeta_click(object sender, EventArgs e)
        {

            EjecutaLecturaConsultaPINPAD("LT");

        }

        private bool ValidaSolicitudAnulacion(string secuencialTransaccion)
        {
            bool bValidaAnulacion = false;
            var pos = new POSEntities();


            try
            {

                if (txtValor.Text == null || txtValor.Text == "")
                {
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Debe de ingresar un valor para ejecutar la Anulación / Devolución");
                    Control.Common.General.GetMensajeToList(531);
                    return bValidaAnulacion;
                }


                if (txtnumAut.Text.Trim().Length == 0)
                {
                    //MessageBox.Show(this, "Debe escribir el numero de autorizacion");
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Debe ingresar el número de Autorización", "Solicita Anulación");
                    Control.Common.General.GetMensajeToList(532);
                    return bValidaAnulacion;
                }
                if (txtSecuencial.Text.Trim().Length == 0)
                {
                    // MessageBox.Show(this, "Debe escribir el secuencial de la transaccion");
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Debe ingresar la secuencia de la transacción", "Solicita Anulación");
                    Control.Common.General.GetMensajeToList(533);
                    return bValidaAnulacion;
                }

                var existeVoucher = pos.POS_VOUCHER.Any(x => x.AUTORIZACION == txtnumAut.Text && x.NUMEROVOUCHER == secuencialTransaccion);
                if (!existeVoucher)
                {
                    // MessageBox.Show(this, "No existe voucher registrado con esa informacion");
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El número de Voucher, no se encuentra registrada", "Solicita Anulación");
                    Control.Common.General.GetMensajeToList(34);
                    return bValidaAnulacion;
                }

                bValidaAnulacion = true;

            }
            catch (Exception)
            {
                bValidaAnulacion = false;
                return bValidaAnulacion;
            }

            
            return bValidaAnulacion;
        }

    

        private void GeneraPagoPINPAD(string tipoTransaccion, string secuencialTransaccion)
        {
            var pos = new POSEntities();
            string IPTransaction = "";
            decimal valInteresD = 0;

            Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
            trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
            trama.secuencialTransaccion = txtSecuencial.Text.PadLeft(6, '0');//6N  -- Anulaciones enviar Secuencial / resto en cero

            if (!ValidaSolicitudAnulacion(trama.secuencialTransaccion))
            {
                //Terminar el metodo btnPagar_Click
                return;
            }


            ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
            Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
            string valpag = Decimal.Round(Decimal.Parse(txtValor.Text), 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";

            int timeOutLT = 40000;
            int.TryParse(pos.core_parametro.Where(x => x.identificador == "LT_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutLT);
            string resultadolectura = string.Empty;

            if (_factura.AplicaDescuentoPromoBines)
            {
                try
                {
                    resultadolectura = envio.SendRequestPinpad("", PUERTOCOM, timeOutLT, "LT", "", 1);
                    //string resultadolectura = envio.Envio_requerimientoPinpad("", PUERTOCOM, 40000, "LT", "", 1);

                    txtValor.Text = decimal.Parse(txtValor.Text).ToString("#######.00");
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "PagoPINPAD", "btnPagar_Click", ex.Message);
                    // MessageBox.Show(this, "Error en tarjeta");
                }

                int leerTramaRes = 75;

                if (resultadolectura.Length > 98)
                {
                    leerTramaRes = leerTramaRes + 24;
                }

                if (resultadolectura.Substring(leerTramaRes, 10) == "LECTURA OK")
                {

                    try
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "Lectura OK del PINPAD.");
                    }
                    catch (Exception ex)
                    {
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "Resultado Lectura:" + resultadolectura);

                    var ctb = pos.core_tarjetacredito_bin.Where(x => x.bin == resultadolectura.Substring(6, 6)).FirstOrDefault();

                    if (ctb != null)
                    {
                        var autorizador = trama.TipoTransaccion == "01" ? ctb.bin_red : ctb.bin_red_cred;
                        autorizador = trama.TipoTransaccion == "03" ? ctb.bin_red : autorizador; //para anulaciones enviar bin_red
                        trama.codRed = autorizador;
                        //trama.codRed = ctb.bin_red;
                        if (trama.codRed == "2")
                        {
                            IPTransaction = pos.core_parametro.Where(x => x.identificador == "PINPAD_IP_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha;
                            trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                            trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja

                            trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                        }
                        else
                        {
                            IPTransaction = pos.core_parametro.Where(x => x.identificador == "PINPAD_IP_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                            trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                            trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                            trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                        }

                    }
                    else
                    {
                        try
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "Error en tarjeta.");
                        }
                        catch (Exception ex)
                        {
                        }

                        MessageBox.Show(this, "Error en tarjeta");
                        return;
                    }
                }
                else
                {
                    try
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "Error en PINPAD. Error en la LEctura del PINPAD.");
                    }
                    catch (Exception ex)
                    {
                    }

                    MessageBox.Show(this, "Error en PINPAD");
                    return;
                }

                //INICIO JCañarte 7Ene2021 Traes MID grabado en voucher original
                var VoucherMID = pos.POS_VOUCHER.Where(x => x.AUTORIZACION == txtnumAut.Text && x.NUMEROVOUCHER == trama.secuencialTransaccion).FirstOrDefault().MID;
                if (VoucherMID != "" && VoucherMID != null)
                {
                    trama.MID = VoucherMID;
                }

            }
            else
            {
                trama.codRed = "2";
                IPTransaction = pos.core_parametro.Where(x => x.identificador == "PINPAD_IP_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha;                                         
                trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;

            }

            trama.plazoDiferido = cmbDiferido.Text;
            trama.mesGracia = cmbMesesGracia.Text;
            //filler 1spc
            trama.montoTotalTransaccion = valpag;//12N 10N2D
            trama.montoBaseGravaIVa = _factura.GetBase12().ToString().Replace(".", ""); //12N 10N2D
            trama.montoBaseNoGravaIVa = _factura.GetBase0().ToString().Replace(".", "");//12N 10N2D
            trama.impuestoIvaTransaccion = _factura.getIVA().ToString().Replace(".", "");//12N 10N2D
            trama.impuestoServicioTransaccion = "";//12N 10N2D
            trama.popinaTransaccion = "";//12N 10N2D
            trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras

            //trama.secuencialTransaccion = trama.TipoTransaccion == "03" ? trama.secuencialTransaccion : ""; //evelasco validación si es anulación, entonces envia el numero secuencial del formulario, caso contrario envia vacio.
            trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
            trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
            trama.numAutorizacion = txtnumAut.Text;//6N  solo anulaciones envia autorizacion compra original / resto blancos
            trama.CID = _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 
            trama.TipoMensaje = "PP";
            envio = new ClsEnviaPinPadGeneral();
            int timeOutCP = 45000;
            int.TryParse(pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutCP);
            //txtResult.Text = envio.Envio_requerimientoPinpad(IPTransaction, PUERTOCOM, 300000, trama.DevuelveTrama, "", 1);
            txtResult.Text = envio.SendRequestPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);
            resptrama.ObtieneDato = txtResult.Text;


        }
        private void GeneraPagoPINPADV2(string tipoTransaccion, string secuencialTransaccion)
        {
            var pos = new POSEntities();
            string IPTransaction = "";
            decimal valInteresD = 0;
          
            POS_VOUCHER pos_voucher = new POS_VOUCHER();
            ClsEnviaPinPadGeneral envioGen = new ClsEnviaPinPadGeneral();

            Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
            Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
            PinPadRespuesta resultadolectura = new PinPadRespuesta();

            string ResponseBackground = string.Empty;
            string autorizador = "2";
            string IPPinPad = string.Empty;
            int PuertoPinPad = 0;
            string TarjetaCreditoBin = string.Empty;
            string plazoDiferido = string.Empty;
            string mesGracia = string.Empty;
            string bin_descripcion = string.Empty;
            string valpag = string.Empty;
            decimal valor = 0;
            int timeOutCP = Control.Common.GlobalParameters.ConectContingente.TiempoOutCP;
            string nombreRed = "";

            tipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;

            try
            {


                if (tipoTransaccion == "03")
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Validación Solicitud Anulación Voucher.");

                    if (!ValidaSolicitudAnulacion(secuencialTransaccion))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Validación Solicitud Anulación Voucher. Tiene errores");
                        return;
                    }

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Validación Solicitud Anulación Voucher. realizada correctamente");
                }

     
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", " Ejecuta proceso de validación del Contingente ");
                Control.Common.General.ValidaContingente();

                IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", " Lectura Pinpad ");
                resultadolectura = envioGen.LecturaTarjeta(IPPinPad, PuertoPinPad, 65000, "LT", "", 1);

                if (resultadolectura.CodigoRespuesta == "00")
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Lectura OK del PINPAD.");


                    trama = new Tramas.ProcesaPago();
                    trama.TipoMensaje = "PP"; //Procesa Pago
                    trama.TipoTransaccion = tipoTransaccion;
                    trama.secuencialTransaccion = secuencialTransaccion;

                    int leerTramaRes = 75;
                    if (resultadolectura.TramaRespuesta.Length > 98)
                    {
                        leerTramaRes = leerTramaRes + 24;
                    }


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Consulta BINES de la tarjeta de credito");
                    var ConsultaBin = pos.core_tarjetacredito_bin.Where(x => x.bin == resultadolectura.NumBin).FirstOrDefault();

                    if (ConsultaBin != null)
                    {
                          
                        bin_descripcion = ConsultaBin.bin_descripcion;

                        //autorizador = Control.Common.GlobalParameters.ConectContingente.Autorizador.ToString();
                        autorizador = ConsultaBin.bin_red;
                        trama.MID = string.Empty;
                        trama.TID = string.Empty;

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraPagoPINPADV2", " Asigno valor por defecto de autorizador. Si el parametro contingente esta activo, el codigo de autorizador no debe de modificarse ");

                        //if (!Control.Common.GlobalParameters.ConectContingente.PinPadContingente)
                        //{
                        //    autorizador = Control.Common.GlobalParameters.AutorizadorDefault.ToString();
                        //    Control.Common.GlobalParameters.ConectContingente.Autorizador = Control.Common.GlobalParameters.AutorizadorDefault;
                        //}

                        trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                        if (autorizador == "1")
                        {
                            trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                        }

                        trama.codRed = autorizador;
                        trama.MID = Control.Common.GlobalParameters.MID_DATAFAST;
                        trama.TID = Control.Common.GlobalParameters.TID_DATAFAST;

                        if(autorizador =="2")
                        {
                            IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                            PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                            trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                            trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;
                            trama.codRed = autorizador;

                        }


                        //if (Control.Common.GlobalParameters.ConectContingente.Autorizador == 2)
                        //{
                        //    IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                        //    PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                        //    trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                        //    trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;
                        //    trama.codRed = autorizador;
                        //}

                        if (!Control.Common.GlobalParameters.EstTcpIpPinpad)
                        {
                            ResponseBackground = Common.GlobalParameters.PinpadMsjAutorizadorNoValido;
                            return;
                        }

                        //Control.Common.GlobalParameters.IPPinPad = IPPinPad;
                        //Control.Common.GlobalParameters.PuertoPinPad = PuertoPinPad;
                        trama.TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                        plazoDiferido = cmbDiferido.Text.PadLeft(2, ' ');
                        trama.plazoDiferido = plazoDiferido;

                        mesGracia = cmbMesesGracia.Text.PadLeft(2, ' ');
                        trama.mesGracia = mesGracia;

                        valpag = Decimal.Round(Decimal.Parse(txtValor.Text), 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";

                        trama.montoTotalTransaccion = "";//12N 10N2D
                        trama.montoBaseGravaIVa = _factura.GetBase12().ToString().Replace(".", ""); //12N 10N2D
                        trama.montoBaseNoGravaIVa = _factura.GetBase0().ToString().Replace(".", "");//12N 10N2D
                        trama.impuestoIvaTransaccion = _factura.getIVA().ToString().Replace(".", "");//12N 10N2D
                        trama.impuestoServicioTransaccion = "";//12N 10N2D
                        trama.popinaTransaccion = "";//12N 10N2D
                        trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                                             //trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero                
                        trama.horaTransccion = DateTime.Now.ToString("HHmmss"); //HHMMSS
                        trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); //AAAAMMDD
                        trama.numAutorizacion = txtnumAut.Text;//6N  solo anulaciones envia autorizacion compra original / resto blancos
                        trama.CID = "LIRISCID0" + _factura.Establecimiento + _factura.PtoEmision;//15 identificador de la caja 

                        var envioGenResponse = new ClsEnviaPinPadGeneral();
                        ClsEnviaPinPadGeneral objContingente = new ClsEnviaPinPadGeneral();
                        PinPadRespuesta PagoResp = new PinPadRespuesta();
                        string strTrama = string.Empty;

                        
                        strTrama = trama.DevuelveTrama.ToString();
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "GeneraPagoPINPADV2", "Enviando requerimiento PINPAD, trama: " + strTrama);
                        String tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                        
                        /*Se ejecuta el cobro por tarjeta. Se envia parametro*/
                        PagoResp = new PinPadRespuesta();
                        PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, trama.codRed, strTrama, "", 1, "PP");
                        
                       
                        if (PagoResp.CodigoRespuesta == "00")
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "--Trama PINPAD: " + trama.DevuelveTrama); //evelasco 2020-02-12
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "--Resultado de la transacción : " + PagoResp.MensajeRespuesta); //evelasco 2020-02-12

                            var signo = 1;
                            if (trama.TipoTransaccion == "03" || trama.TipoTransaccion == "04")
                            {
                                signo = -1;
                                tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherAnula;
                              
                                
                                try
                                {
                                    pos = new POSEntities();
                                   
                                    var ValorInteres = pos.POS_VOUCHER.Where(x => x.AUTORIZACION == txtnumAut.Text && x.NUMEROVOUCHER == trama.secuencialTransaccion)
                                                        .FirstOrDefault()
                                                        .VALORINTERES;

                                    
                                    valInteresD = ValorInteres == "            " ? 0 : Decimal.Parse(ValorInteres.Substring(0, 11) + "." + ValorInteres.Substring(11, 2));
                                    //Para anular voucher se lo debe identificar por la combinacion de nro Autorizacion y el nro de Voucher
                                    var VOUCHER = pos.POS_VOUCHER.Where(x => x.AUTORIZACION == txtnumAut.Text && x.NUMEROVOUCHER == trama.secuencialTransaccion)
                                                                 .FirstOrDefault();
                                    if (VOUCHER != null)
                                    {
                                        VOUCHER.ANULADO = true;
                                        pos.SaveChanges();
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Actualiza VOUCHER.ANULADO=TRUE WHERE AUTORIZACION= " + txtnumAut.Text + "NUMEROVOUCHER= " + trama.secuencialTransaccion);
                                    }



                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Trama PINPAD: " + trama.DevuelveTrama);
                                }
                                catch (Exception)
                                {
                                }

                            }

                            //trama.TipoTransaccion
                            RepuestaPago Pago = PagoResp.GetDatosPago(PagoResp.TramaRespuesta, autorizador);

                            prepararVoucherTarjeta(tipovoucher, Pago, trama);

                            valor = (Decimal.Parse(txtValor.Text) - (valInteresD)) * signo;
                            if (trama.TipoTransaccion != "03")
                            {
                                _factura.AgregarPagoTarjetaCredito(valor, resptrama.codigoRed == "02" ? "MEDIANET" : "DATAFAST", resptrama.nomGruTar, resptrama.codBancoAdq, resptrama.codigoRed == "02" ? "MEDIANET" : "DATAFAST");
                            }

                            //grabar transaccion en tabla pos_voucher
                            //cambiar aqui
                            pos_voucher = new POS_VOUCHER();
                            pos_voucher.TARJETA = resptrama.numTarTuncate.Trim().PadRight(19, ' ');// ("520081XXXXXX6017   "); //19 ;

                            //revisar
                            string codigoproceso = "000200";
                            //es 003000 cuando es transacciones con tarjeta de crédito, 001000 cuando es transacción de tarjeta de debito cuenta de ahorro y 002000 cuando es transacción de tarjeta de debito cuenta corriente.

                            pos_voucher.CODIGOPROCESO = codigoproceso;// ("000200"); //6 ;
                            pos_voucher.FECHACONSUMO = Pago.FechaTrans;// ("20161122"); //8 ;
                            pos_voucher.HORACONSUMO = Pago.HoraTrans;// ("114339"); //6 ;
                            pos_voucher.NUMEROVOUCHER = Pago.SecTrans.ToString();// ("000002");//6 ;
                            pos_voucher.AUTORIZACION = Pago.TipoTransaccion == "03" ? Pago.NumAutorizacion : Pago.NumAutorizacion; //6 ;
                            pos_voucher.ANULADO = Pago.TipoTransaccion == "03" ? true : false;

                            pos_voucher.VALORCONSUMO = trama.montoTotalTransaccion.PadLeft(13, '0');// ("0000000001200"); //13 ;
                            pos_voucher.FORMAAUTORIZA = ("1"); //1 ;
                            if (resptrama.codigoRed == "02")
                                pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo; //2 ;
                            else
                                pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData; //2 ;
                            pos_voucher.PLAZO = trama.plazoDiferido.PadLeft(2, '0');// ("06"); //2 ;

                            pos_voucher.TIPOLECTURA = resptrama.modoLectura.PadLeft(3, '0');// ("005"); //3 ;
                            pos_voucher.TIPOMONEDA = ("840"); //3 ;
                            pos_voucher.VALORIVA = trama.impuestoIvaTransaccion.PadLeft(13, '0');// ("0000000000147"); //13 ;
                            pos_voucher.VALORSERVICIO = ("0000000000000"); //13 ;
                            pos_voucher.VALORPROPINA = trama.popinaTransaccion.PadLeft(13, '0');// ("0000000000000"); //13 ;
                            pos_voucher.VALORINTERES = Pago.ValorInteresFin.ToString().PadLeft(13, '0');// ("0000000000057");  //13 ;
                            pos_voucher.VALORFIJO = Pago.MontoFijo.ToString().PadLeft(13, '0');// ("0000000000000");  //13 ;
                            pos_voucher.TIPOPROMOCION = ("00"); //2 ;
                            pos_voucher.MESESGRACIA = trama.mesGracia.PadLeft(2, '0');//                        ("00");  //2 ;
                            pos_voucher.EMPRESASERVICIO = ("0000");  //4 ;
                            pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "R"); //1 ;
                            pos_voucher.CODIGORESPUESTA = Pago.CodRespMsjAut;// ("00"); //2 ;
                            pos_voucher.TIPODISPOSITIVO = ("2"); //1 ;
                            pos_voucher.ADQUIRENTETARJETA = ("CREDIMATIC01"); //12 ;
                            pos_voucher.ADQUIRENTESERVICIO = ("            ");  //12 ;
                            pos_voucher.MONTOGRAVAIVA = trama.montoBaseGravaIVa.PadLeft(13, '0');// ("0000000001053"); //13 ;
                            pos_voucher.MONTONOGRAVAIVA = trama.montoBaseNoGravaIVa.PadLeft(13, '0');// ("0000000000000");  //13 ;
                            pos_voucher.PUNTOEMISION = _factura.Establecimiento + _factura.PtoEmision;
                            pos_voucher.PROCESADO = false;
                            pos_voucher.GRUPOTAR = Pago.GrupoTarjeta;
                            pos_voucher.MID = trama.MID; // JCanarte 7Ene2021
                            pos_voucher.AUTORIZADOR = int.Parse(Pago.CodRed);
                            pos_voucher.LOTE = Pago.NumLote;
                            if (trama.TipoTransaccion != "03")
                            {
                                bool ExisteVoucher = pos.POS_VOUCHER.Any(x => x.FECHACONSUMO == pos_voucher.FECHACONSUMO 
                                                        && x.VALORCONSUMO == pos_voucher.VALORCONSUMO 
                                                        && x.AUTORIZACION == pos_voucher.AUTORIZACION 
                                                        && x.NUMEROVOUCHER == pos_voucher.NUMEROVOUCHER 
                                                        && x.FACTURA == pos_voucher.FACTURA);
                                if (!ExisteVoucher)
                                {
                                    pos.POS_VOUCHER.Add(pos_voucher);
                                }
                                // pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet += 1;
                                pos.SaveChanges();

                                //Agregar lineas de insert
                                Control.Common.Logger.Agregar_Trace_Voucher(pos_voucher);
                            }
                            this.Close();

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "pos_voucher se ha regsitrado la transacción de Anulación");

                        }
                        else
                        {

                         
                            string msgerror = "Error : " + PagoResp.MensajeRespuesta;

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2",  msgerror + "; TipoTransaccion: " + trama.TipoTransaccion);
                            if (trama.TipoTransaccion == "2" || trama.TipoTransaccion == "02")
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Realiza reverso de transacción ");

                                //Enviar trama de reverso de transaccion tipo 04.   JM  25-08-2020
                                envioGenResponse = new ClsEnviaPinPadGeneral();
                                trama.TipoTransaccion = "04";
                                txtResult.Text = envioGenResponse.SendRequestPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);
                                resptrama.ObtieneDato = txtResult.Text;
                                msgerror = msgerror + " \n" + "Reverso : " + resptrama.mensajeRespuesta;
                            }


                            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = msgerror });
                            Control.Common.General.GetMensajeToList(535, parametros);

                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "PINPAD: " + msgerror);

                            // MessageBox.Show(this, msgerror);

                            try
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraPagoPINPADV2", "Realiza reverso de transacción; "+ txtResult.Text);

                            }
                            catch (Exception ex1)
                            {
                            }                                

                        }
                    }
                    else
                    {//Validación de Objeto ConsultaBin
                        ResponseBackground = "Tarjeta no se encuentra en listado de bines";
                        return;
                    }
                }


             
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "GeneraAnulacion", "Error Anulación " + ex.Message);
            }
        }


        private void btnPagar_Click(object sender, EventArgs e)
        {
            string IPTransaction = "";
            var pos = new POSEntities();
            decimal valInteresD = 0;

            ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
            Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
            Tramas.ProcesaPago trama = new Tramas.ProcesaPago();

            string TipoTransaccion = string.Empty;
            string secuencialTransaccion = string.Empty;
   
            try
            {
                if (pos.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                {
                    TipoTransaccion = ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                    secuencialTransaccion = txtSecuencial.Text.PadLeft(6, '0');

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "TipoTransaccion: " + TipoTransaccion);


                    //Si la accion solicitada es reimpresion
                    if (TipoTransaccion == "05")
                    {
                        string NumFactura = lblFacReimpresion.Text.Trim();
                        // NumFactura = "F-011-008-000683036";

                        // var listaVouchers = pos.POS_VOUCHER.Where(x => x.FACTURA == lblFacReimpresion.Text.Trim() && x.TIPOTRANSACCION != null && x.ANULADO == false).ToList();
                        var listaVouchers = pos.POS_VOUCHER.Where(x => x.FACTURA == NumFactura && x.TIPOTRANSACCION != null && x.ANULADO == false).ToList();
                        if (listaVouchers == null)
                        {
                            //msgBoxCtrl = new MsgBoxCtrl();
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No se ha podido consultar la lista de vouchers de esta factura, esto puede deberse a que la red esté ocupada. Por favor inténtelo nuevamente en unos instantes", "POS -  PagoPinPAD");
                            // MessageBox.Show(this, "No se ha podido consultar la lista de vouchers de esta factura, esto puede deberse a que la red esté ocupada. Por favor inténtelo nuevamente en unos instantes");
                            Control.Common.General.GetMensajeToList(537);

                        }
                        else
                        {
                            if (listaVouchers.Count() > 0)
                            {
                                foreach (var voucher in listaVouchers)
                                {
                                    if (!Pagos.ClsPagos.ReimprimirVoucher(Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito, voucher, true))
                                    {
                                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                        parametros.Add(new ParametrosMensajes() { codigo = "[voucherId]", valor = voucher.id.ToString() });
                                        Control.Common.General.GetMensajeToList(538, parametros);

                                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El voucher con id {0} no pudo ser impreso, esto puede deberse a que la red esté ocupada. Por favor inténtelo nuevamente en unos instantes" + voucher.id.ToString(), "POS -  PagoPinPAD");
                                        //MessageBox.Show(this, String.Format("El voucher con id {0} no pudo ser impreso, esto puede deberse a que la red esté ocupada. Por favor inténtelo nuevamente en unos instantes", voucher.id.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                //msgBoxCtrl = new MsgBoxCtrl();
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No existen vouchers para la factura indicada", "POS -  PagoPinPAD");
                                //MessageBox.Show(this, "No existen vouchers para la factura indicada");
                                Control.Common.General.GetMensajeToList(539);

                            }
                        }

                        //Terminar el metodo btnPagar_Click
                        return;
                    }


                    //Si la accion solicitada es anulacion
                    if (TipoTransaccion  == "03")
                    {

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "TipoTransaccion: " + trama.TipoTransaccion);

                        try
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "INICIA ANULACIÓN VOUCHER");
                            
                            if (Control.Common.GlobalParameters.PINPAD_MULTIRED)
                            {
                                GeneraPagoPINPADV2(TipoTransaccion, secuencialTransaccion);
                            }
                            else
                            {
                                GeneraPagoPINPAD(trama.TipoTransaccion, secuencialTransaccion);
                            }
                       
                            //GeneraPagoPINPAD(trama.TipoTransaccion, secuencialTransaccion);
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "TERMINA ANULACIÓN VOUCHER");

                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "Error Anulación " + ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnPagar_Click", "Error btnPagar_Click " + ex.Message);
            }

         

        }

        private void prepararVoucherTarjeta(string tipo_voucher, Tramas.RespuestaProcesoPago trama, Tramas.ProcesaPago pp)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Inicia proceso de impresion");

            var db = new POSEntities();

            var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
            //No se usa //DateTime fecha = DateTime.ParseExact("01-" + trama.fechaVencTar.Substring(2, 2) + "-" + trama.fechaVencTar.Substring(0, 2), "dd-MM-yy", CultureInfo.InvariantCulture);
            //No se usa //string fechaVencTar = Convert.ToString(fecha).Substring(5, 2) + '/' + Convert.ToString(fecha).Substring(0, 4);

            recipe.NomTarjeta = trama.nomGruTar;
            recipe.MID = pp.MID;
            recipe.MID2 = trama.merchantId;
            recipe.TID = pp.TID;
            recipe.NumTarjeta = trama.numTarTuncate;

            recipe.NumLote = trama.numerolote;

            // Nuevo Autorizador "AUSTRO".  JM  04-12-2020   
            if (trama.codigoRed == "01")
                recipe.CodigoRed = "DATAFAST";
            else if (trama.codigoRed == "02")
                recipe.CodigoRed = "MEDIANET";
            else if (trama.codigoRed == "03")
                recipe.CodigoRed = "AUSTRO";

            recipe.Adquiriente = string.IsNullOrEmpty(trama.nomBancoAdq.Trim()) ? recipe.CodigoRed : trama.nomBancoAdq;
            recipe.Aprobacion = pp.TipoTransaccion == "03" ? pp.numAutorizacion : trama.numAut;
            recipe.Secuencial = trama.secuencialtransaccion;
            recipe.NombreTarjetaHabiente = trama.nombreTarjetaHabiente;
            recipe.FechaTrans = trama.fechaTrans.Substring(0, 4) + "/" + trama.fechaTrans.Substring(4, 2) + "/" + trama.fechaTrans.Substring(6, 2);
            recipe.HoraTrans = pp.TipoTransaccion == "03" ? DateTime.Now.ToString("HH:mm:ss") : trama.horaTrans.Substring(0, 2) + ":" + trama.horaTrans.Substring(2, 2) + ":" + trama.horaTrans.Substring(4, 2);
            recipe.VenTarjeta = "XX/XXXX";// trama.codigoRed == "02" ?fechaVencTar : "XX/XXXX";
            var valoranulacion = "";
            var valorinteres = "";
            var valorT = "";
            if (pp.TipoTransaccion == "03")
            {
                valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion && x.NUMEROVOUCHER == pp.secuencialTransaccion)
                                               .FirstOrDefault()
                                               .VALORCONSUMO;
                valorinteres = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion && x.NUMEROVOUCHER == pp.secuencialTransaccion)
                                               .FirstOrDefault()
                                               .VALORINTERES;
                valoranulacion = (Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)) * -1).ToString("###,##0.00");
                valorinteres = (Decimal.Parse(valorinteres.Substring(0, 11) + "." + valorinteres.Substring(11, 2)) * -1).ToString("###,##0.00");
                //valorT = (decimal.Parse(valoranulacion) + decimal.Parse(valorinteres)).ToString("###,##0.00");
                valorT = (decimal.Parse(valoranulacion)).ToString("###,##0.00");
                valoranulacion = valorT;
            }

            recipe.ValorTotal = pp.TipoTransaccion == "03" ? valoranulacion : txtValor.Text;

            string modolectura = "";
            switch (trama.modoLectura)
            {
                case "01":
                    modolectura = "Manual";
                    break;
                case "02":
                    modolectura = "Banda";
                    break;
                case "03":
                    modolectura = "Chip";
                    break;
                case "04":
                    modolectura = "Fallback Manual (Chip)";
                    break;
                case "05":
                    modolectura = "Fallback Banda (Chip) ";
                    break;
                case "07":
                    modolectura = "Contactless";
                    break;
            }
            recipe.ModoLectura = modolectura;

            if (pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
            {
                recipe.BaseIva = _factura.GetBase12().ToString("###,##0.00");
                recipe.BaseSinIva = _factura.GetBase0().ToString("###,##0.00");
                recipe.Subtotal = _factura.getSubTotal().ToString("###,##0.00");
                recipe.Iva = _factura.getIVA().ToString("###,##0.00");
            }
            else
            {
                recipe.BaseIva = "";
                recipe.BaseSinIva = "";
                recipe.Subtotal = "";
                recipe.Iva = "";
            }

            //recipe.CodigoRed = trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST";

            string tipodebcred = "";
            if (trama.nomGruTar.Contains("DEBIT"))
            {
                tipodebcred = "*DEBITO*\n";

                recipe.Pagare = " ";
            }
            else
            {
                if (cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                    tipodebcred = "*ROTATIVO*\n";
                else
                    tipodebcred = "*" + cmbBancoTarjeta.Text + "*\n\n PLAZO MESES: " + cmbDiferido.Text + (cmbMesesGracia.Text != "" ? "\n" + "MESES DE GRACIA:" + cmbMesesGracia.Text : "");
            }
            if (pp.TipoTransaccion != "03")
                recipe.TipoDebCredito = tipodebcred;
            else
                recipe.TipoDebCredito = tipodebcred + "\nANULACION";

            recipe.Transaccion = txtSecuencial.Text;

            if (cmbBancoTarjeta.Text.Contains("DIFERIDO CON INTERESES"))
            {
                decimal valInteres = 0;
                Decimal.TryParse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2), out valInteres);
                recipe.Intereses = valInteres.ToString("###,##0.00");
                //recipe.Intereses = Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2)).ToString("###,##0.00");
            }
            else
                recipe.Intereses = "";

            recipe.Arqc = trama.ARQC;
            recipe.Aidemv = trama.AIDEMV.Trim();
            recipe.Emv = trama.idEMV;
            recipe.Tc = trama.tipoCritoyValorEMV;
            recipe.Publicidad = trama.mensajePremioPublicidad;
            recipe.TVR = trama.TVR;
            recipe.TSI = trama.TSI;

            Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe);


            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Finaliza proceso de impresion");
        }

        private void prepararVoucherTarjetaAnt(string tipo_voucher, Tramas.RespuestaProcesoPago trama, Tramas.ProcesaPago pp)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Inicia proceso de impresion");

            var db = new POSEntities();

            var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
          
            recipe.NomTarjeta = trama.nomGruTar;
            recipe.MID = pp.MID;
            recipe.MID2 = trama.merchantId;
            recipe.TID = pp.TID;
            recipe.NumTarjeta = trama.numTarTuncate;
          

            recipe.NumLote = trama.numerolote;

            // Nuevo Autorizador "AUSTRO".  JM  04-12-2020   
            if (trama.codigoRed == "01")
                recipe.CodigoRed = "DATAFAST";
            else if (trama.codigoRed == "02")
                recipe.CodigoRed = "MEDIANET";
            else if (trama.codigoRed == "03")
                recipe.CodigoRed = "AUSTRO";

            recipe.Adquiriente = string.IsNullOrEmpty(trama.nomBancoAdq.Trim()) ? recipe.CodigoRed : trama.nomBancoAdq;
            recipe.Aprobacion = pp.TipoTransaccion == "03" ? pp.numAutorizacion : trama.numAut;
            recipe.Secuencial = trama.secuencialtransaccion;
            recipe.NombreTarjetaHabiente = trama.nombreTarjetaHabiente;
            recipe.FechaTrans = trama.fechaTrans.Substring(0, 4) + "/" + trama.fechaTrans.Substring(4, 2) + "/" + trama.fechaTrans.Substring(6, 2);
            recipe.HoraTrans = pp.TipoTransaccion == "03" ? DateTime.Now.ToString("HH:mm:ss") : trama.horaTrans.Substring(0, 2) + ":" + trama.horaTrans.Substring(2, 2) + ":" + trama.horaTrans.Substring(4, 2);
            recipe.VenTarjeta = "XX/XXXX";// trama.codigoRed == "02" ?fechaVencTar : "XX/XXXX";
            var valoranulacion = "";
            var valorinteres = "";
            var valorT = "";
            if (pp.TipoTransaccion == "03")
            {
                valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion && x.NUMEROVOUCHER == pp.secuencialTransaccion)
                                               .FirstOrDefault()
                                               .VALORCONSUMO;
                valorinteres = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion && x.NUMEROVOUCHER == pp.secuencialTransaccion)
                                               .FirstOrDefault()
                                               .VALORINTERES;
                valoranulacion = (Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)) * -1).ToString("###,##0.00");
                valorinteres = (Decimal.Parse(valorinteres.Substring(0, 11) + "." + valorinteres.Substring(11, 2)) * -1).ToString("###,##0.00");
                //valorT = (decimal.Parse(valoranulacion) + decimal.Parse(valorinteres)).ToString("###,##0.00");
                valorT = (decimal.Parse(valoranulacion)).ToString("###,##0.00");
                valoranulacion = valorT;
            }

            recipe.ValorTotal = pp.TipoTransaccion == "03" ? valoranulacion : txtValor.Text;

            string modolectura = "";
            switch (trama.modoLectura)
            {
                case "01":
                    modolectura = "Manual";
                    break;
                case "02":
                    modolectura = "Banda";
                    break;
                case "03":
                    modolectura = "Chip";
                    break;
                case "04":
                    modolectura = "Fallback Manual (Chip)";
                    break;
                case "05":
                    modolectura = "Fallback Banda (Chip) ";
                    break;
                case "07":
                    modolectura = "Contactless";
                    break;
            }
            recipe.ModoLectura = modolectura;

            if (pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
            {
                recipe.BaseIva = _factura.GetBase12().ToString("###,##0.00");
                recipe.BaseSinIva = _factura.GetBase0().ToString("###,##0.00");
                recipe.Subtotal = _factura.getSubTotal().ToString("###,##0.00");
                recipe.Iva = _factura.getIVA().ToString("###,##0.00");
            }
            else
            {
                recipe.BaseIva = "";
                recipe.BaseSinIva = "";
                recipe.Subtotal = "";
                recipe.Iva = "";
            }

            //recipe.CodigoRed = trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST";

            string tipodebcred = "";
            if (trama.nomGruTar.Contains("DEBIT"))
            {
                tipodebcred = "*DEBITO*\n";

                recipe.Pagare = " ";
            }
            else
            {
                if (cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                    tipodebcred = "*ROTATIVO*\n";
                else
                    tipodebcred = "*" + cmbBancoTarjeta.Text + "*\n\n PLAZO MESES: " + cmbDiferido.Text + (cmbMesesGracia.Text != "" ? "\n" + "MESES DE GRACIA:" + cmbMesesGracia.Text : "");
            }
            if (pp.TipoTransaccion != "03")
                recipe.TipoDebCredito = tipodebcred;
            else
                recipe.TipoDebCredito = tipodebcred + "\nANULACION";

            recipe.Transaccion = txtSecuencial.Text;

            if (cmbBancoTarjeta.Text.Contains("DIFERIDO CON INTERESES"))
            {
                decimal valInteres = 0;
                Decimal.TryParse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2), out valInteres);
                recipe.Intereses = valInteres.ToString("###,##0.00");
                //recipe.Intereses = Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2)).ToString("###,##0.00");
            }
            else
                recipe.Intereses = "";

            recipe.Arqc = trama.ARQC;
            recipe.Aidemv = trama.AIDEMV.Trim();
            recipe.Emv = trama.idEMV;
            recipe.Tc = trama.tipoCritoyValorEMV;
            recipe.Publicidad = trama.mensajePremioPublicidad;
            recipe.TVR = trama.TVR;
            recipe.TSI = trama.TSI;

            Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe);

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Finaliza proceso de impresion");
        }

        private void prepararVoucherTarjeta(string tipo_voucher, RepuestaPago trama, Tramas.ProcesaPago pp)
        {
            try
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Inicia proceso de impresion");

                var db = new POSEntities();

                var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
                //No se usa //DateTime fecha = DateTime.ParseExact("01-" + trama.fechaVencTar.Substring(2, 2) + "-" + trama.fechaVencTar.Substring(0, 2), "dd-MM-yy", CultureInfo.InvariantCulture);
                //No se usa //string fechaVencTar = Convert.ToString(fecha).Substring(5, 2) + '/' + Convert.ToString(fecha).Substring(0, 4);

                recipe.NomTarjeta = trama.GrupoTarjeta;
                recipe.MID = pp.MID;
                recipe.MID2 = trama.MerchantID;
                recipe.TID = pp.TID;
                recipe.NumTarjeta = trama.NumTarjetaTrunc;
                recipe.NumLote = trama.NumLote;

                // Nuevo Autorizador "AUSTRO".  JM  04-12-2020   
                if (trama.CodRed == "01")
                    recipe.CodigoRed = "DATAFAST";
                else if (trama.CodRed == "02")
                    recipe.CodigoRed = "MEDIANET";
                else if (trama.CodRed == "03")
                    recipe.CodigoRed = "AUSTRO";

                recipe.Adquiriente = string.IsNullOrEmpty(trama.NombBcoAdq.Trim()) ? recipe.CodigoRed : trama.NombBcoAdq;
                recipe.Aprobacion = pp.TipoTransaccion == "03" ? pp.numAutorizacion : trama.NumAutorizacion;
                recipe.Secuencial = trama.SecTrans.ToString();
                recipe.NombreTarjetaHabiente = trama.NombTarjetaHabiente;
                recipe.FechaTrans = trama.FechaTrans.Substring(0, 4) + "/" + trama.FechaTrans.Substring(4, 2) + "/" + trama.FechaTrans.Substring(6, 2);
                recipe.HoraTrans = pp.TipoTransaccion == "03" ? DateTime.Now.ToString("HH:mm:ss") : trama.HoraTrans.Substring(0, 2) + ":" + trama.HoraTrans.Substring(2, 2) + ":" + trama.HoraTrans.Substring(4, 2);
                recipe.VenTarjeta = "XX/XXXX";// trama.codigoRed == "02" ?fechaVencTar : "XX/XXXX";
                var valoranulacion = "";
                var valorinteres = "";
                var valorT = "";

                if (pp.TipoTransaccion == "03")
                {
                    valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion && x.NUMEROVOUCHER == pp.secuencialTransaccion)
                                                   .FirstOrDefault()
                                                   .VALORCONSUMO;
                    valorinteres = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion && x.NUMEROVOUCHER == pp.secuencialTransaccion)
                                                   .FirstOrDefault()
                                                   .VALORINTERES;
                    valoranulacion = (Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)) * -1).ToString("###,##0.00");
                    valorinteres = (Decimal.Parse(valorinteres.Substring(0, 11) + "." + valorinteres.Substring(11, 2)) * -1).ToString("###,##0.00");
                    //valorT = (decimal.Parse(valoranulacion) + decimal.Parse(valorinteres)).ToString("###,##0.00");
                    valorT = (decimal.Parse(valoranulacion)).ToString("###,##0.00");
                    valoranulacion = valorT;
                }

                recipe.ValorTotal = pp.TipoTransaccion == "03" ? valoranulacion : txtValor.Text;

                string modolectura = "";
                switch (trama.ModLectura)
                {
                    case "01":
                        modolectura = "Manual";
                        break;
                    case "02":
                        modolectura = "Banda";
                        break;
                    case "03":
                        modolectura = "Chip";
                        break;
                    case "04":
                        modolectura = "Fallback Manual (Chip)";
                        break;
                    case "05":
                        modolectura = "Fallback Banda (Chip) ";
                        break;
                    case "07":
                        modolectura = "Contactless";
                        break;
                }
                recipe.ModoLectura = modolectura;

                if (pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
                {
                    recipe.BaseIva = _factura.GetBase12().ToString("###,##0.00");
                    recipe.BaseSinIva = _factura.GetBase0().ToString("###,##0.00");
                    recipe.Subtotal = _factura.getSubTotal().ToString("###,##0.00");
                    recipe.Iva = _factura.getIVA().ToString("###,##0.00");
                }
                else
                {
                    recipe.BaseIva = "";
                    recipe.BaseSinIva = "";
                    recipe.Subtotal = "";
                    recipe.Iva = "";
                }

                //recipe.CodigoRed = trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST";

                string tipodebcred = "";
                if (trama.GrupoTarjeta.Contains("DEBIT"))
                {
                    tipodebcred = "*DEBITO*\n";

                    recipe.Pagare = " ";
                }
                else
                {
                    if (cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                        tipodebcred = "*ROTATIVO*\n";
                    else
                        tipodebcred = "*" + cmbBancoTarjeta.Text + "*\n\n PLAZO MESES: " + cmbDiferido.Text + (cmbMesesGracia.Text != "" ? "\n" + "MESES DE GRACIA:" + cmbMesesGracia.Text : "");
                }
                if (pp.TipoTransaccion != "03")
                    recipe.TipoDebCredito = tipodebcred;
                else
                    recipe.TipoDebCredito = tipodebcred + "\nANULACION";

                recipe.Transaccion = txtSecuencial.Text;

                if (cmbBancoTarjeta.Text.Contains("DIFERIDO CON INTERESES"))
                {
                    //decimal valInteres = 0;
                    //Decimal.TryParse(trama.ValorInteresFin.Substring(0, 10) + "." + trama.ValorInteresFin.Substring(10, 2), out valInteres);
                    //recipe.Intereses = valInteres.ToString("###,##0.00");

                    recipe.Intereses = trama.ValorInteresFin.ToString("###,##0.00");
                    //recipe.Intereses = Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2)).ToString("###,##0.00");
                }
                else
                    recipe.Intereses = "";

                recipe.Arqc = trama.ARQC;
                recipe.Aidemv = trama.AID_EMV.Trim();
                recipe.Emv = trama.EMV;


                recipe.Tc = trama.TipoCriptogramaEMV == null ? "" : trama.TipoCriptogramaEMV;
                recipe.Publicidad = trama.MsjImpPremiosPub;
                recipe.TVR = trama.TVR;
                recipe.TSI = trama.TSI;

                Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe);

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Error: " + ex.Message);
            }
            finally
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Finaliza proceso de impresion");
            }

            
        }
        private void prepararVoucherTarjeta2(string tipo_voucher, RepuestaPago trama, string AUTORIZACION, string NUMEROVOUCHER)
        {
            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta", "Inicia proceso de impresion");
                //prepararVoucherTarjeta(tipovoucher, Pago, detVoucher.NUMEROVOUCHER, detVoucher.AUTORIZACION);
                var db = new POSEntities();

                var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
                //No se usa //DateTime fecha = DateTime.ParseExact("01-" + trama.fechaVencTar.Substring(2, 2) + "-" + trama.fechaVencTar.Substring(0, 2), "dd-MM-yy", CultureInfo.InvariantCulture);
                //No se usa //string fechaVencTar = Convert.ToString(fecha).Substring(5, 2) + '/' + Convert.ToString(fecha).Substring(0, 4);

                recipe.NomTarjeta = trama.GrupoTarjeta;
                recipe.MID = trama.MID;
                recipe.MID2 = trama.MerchantID;
                recipe.TID = trama.TID;
                recipe.NumTarjeta = trama.NumTarjetaTrunc;


                recipe.NumLote = trama.NumLote;

                // Nuevo Autorizador "AUSTRO".  JM  04-12-2020   
                if (trama.CodRed == "01")
                    recipe.CodigoRed = "DATAFAST";
                else if (trama.CodRed == "02")
                    recipe.CodigoRed = "MEDIANET";
                else if (trama.CodRed == "03")
                    recipe.CodigoRed = "AUSTRO";

                recipe.Adquiriente = string.IsNullOrEmpty(trama.NombBcoAdq.Trim()) ? recipe.CodigoRed : trama.NombBcoAdq;
                recipe.Aprobacion = trama.TipoTransaccion == "03" ? AUTORIZACION : AUTORIZACION;
                recipe.Secuencial = trama.SecTrans.ToString();
                recipe.NombreTarjetaHabiente = trama.NombTarjetaHabiente;
                recipe.FechaTrans = trama.FechaTrans.Substring(0, 4) + "/" + trama.FechaTrans.Substring(4, 2) + "/" + trama.FechaTrans.Substring(6, 2);
                recipe.HoraTrans = trama.TipoTransaccion == "03" ? DateTime.Now.ToString("HH:mm:ss") : trama.HoraTrans.Substring(0, 2) + ":" + trama.HoraTrans.Substring(2, 2) + ":" + trama.HoraTrans.Substring(4, 2);
                recipe.VenTarjeta = "XX/XXXX";// trama.codigoRed == "02" ?fechaVencTar : "XX/XXXX";
                var valoranulacion = "";
                var valorinteres = "";
                var valorT = "";

                if (trama.TipoTransaccion == "03")
                {
                    var detVoucher = (from deta in db.POS_VOUCHER
                                      where deta.AUTORIZACION == AUTORIZACION
                                      && deta.NUMEROVOUCHER == NUMEROVOUCHER
                                      select deta).FirstOrDefault();

                    valoranulacion = detVoucher.VALORCONSUMO;
                    valorinteres = detVoucher.VALORINTERES;

                    valoranulacion = (Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)) * -1).ToString("###,##0.00");
                    valorinteres = (Decimal.Parse(valorinteres.Substring(0, 11) + "." + valorinteres.Substring(11, 2)) * -1).ToString("###,##0.00");

                    valorT = (decimal.Parse(valoranulacion)).ToString("###,##0.00");
                    valoranulacion = valorT;
                }

                recipe.ValorTotal = trama.TipoTransaccion == "03" ? valoranulacion : txtValor.Text;

                string modolectura = "";
                switch (trama.ModLectura)
                {
                    case "01":
                        modolectura = "Manual";
                        break;
                    case "02":
                        modolectura = "Banda";
                        break;
                    case "03":
                        modolectura = "Chip";
                        break;
                    case "04":
                        modolectura = "Fallback Manual (Chip)";
                        break;
                    case "05":
                        modolectura = "Fallback Banda (Chip) ";
                        break;
                    case "07":
                        modolectura = "Contactless";
                        break;
                }
                recipe.ModoLectura = modolectura;

                if (trama.TipoTransaccion != "03" || trama.TipoTransaccion != "04")
                {
                    recipe.BaseIva = _factura.GetBase12().ToString("###,##0.00");
                    recipe.BaseSinIva = _factura.GetBase0().ToString("###,##0.00");
                    recipe.Subtotal = _factura.getSubTotal().ToString("###,##0.00");
                    recipe.Iva = _factura.getIVA().ToString("###,##0.00");
                }
                else
                {
                    recipe.BaseIva = "";
                    recipe.BaseSinIva = "";
                    recipe.Subtotal = "";
                    recipe.Iva = "";
                }

                //recipe.CodigoRed = trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST";

                string tipodebcred = "";
                if (trama.GrupoTarjeta.Contains("DEBIT"))
                {
                    tipodebcred = "*DEBITO*\n";

                    recipe.Pagare = " ";
                }
                else
                {
                    if (cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                        tipodebcred = "*ROTATIVO*\n";
                    else
                        tipodebcred = "*" + cmbBancoTarjeta.Text + "*\n\n PLAZO MESES: " + cmbDiferido.Text + (cmbMesesGracia.Text != "" ? "\n" + "MESES DE GRACIA:" + cmbMesesGracia.Text : "");
                }
                if (trama.TipoTransaccion != "03")
                    recipe.TipoDebCredito = tipodebcred;
                else
                    recipe.TipoDebCredito = tipodebcred + "\nANULACION";

                recipe.Transaccion = txtSecuencial.Text;

                if (cmbBancoTarjeta.Text.Contains("DIFERIDO CON INTERESES"))
                {
                    //decimal valInteres = 0;
                    //Decimal.TryParse(trama.ValorInteresFin.Substring(0, 10) + "." + trama.ValorInteresFin.Substring(10, 2), out valInteres);
                    //recipe.Intereses = valInteres.ToString("###,##0.00");

                    recipe.Intereses = trama.ValorInteresFin.ToString("###,##0.00");
                    //recipe.Intereses = Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2)).ToString("###,##0.00");
                }
                else
                    recipe.Intereses = "";

                recipe.Arqc = trama.ARQC;
                recipe.Aidemv = trama.AID_EMV.Trim();
                recipe.Emv = trama.EMV;


                recipe.Tc = trama.TipoCriptogramaEMV == null ? "" : trama.TipoCriptogramaEMV;
                recipe.Publicidad = trama.MsjImpPremiosPub;
                recipe.TVR = trama.TVR;
                recipe.TSI = trama.TSI;

                Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe);

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta2", "Error: " + ex.Message);
            }
            finally
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "prepararVoucherTarjeta2", "Finaliza proceso de impresion");
            }
            
        }
        private void ImprimveVoucherAnulado(string TipoTransaccion, string AUTORIZACION, string NUMEROVOUCHER, Tramas.RespuestaProcesoPago trama)
        {
            try
            {


                var pos = new POSEntities();
                POS_VOUCHER pos_voucher = new POS_VOUCHER();
                RepuestaPago Pago = new RepuestaPago();

                string tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                int signo = 1;


                /*JORGE MEDINA: ESTE CODIGO SE AGREGO SOLO PARA QUE PODAMOS IMPRIMIR LOS VOUCHER ANULADOS */
                var detVoucher = (from det in pos.POS_VOUCHER
                                  where det.AUTORIZACION == AUTORIZACION
                                  && det.NUMEROVOUCHER == NUMEROVOUCHER
                                  select det).FirstOrDefault();

                if (pos_voucher.ANULADO)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos", "ImprimveVoucherAnulado", "Transacción Ya Anulada!");

                    var result = Control.Common.General.GetMensaje("POS", "La transacción ya ha sido anulada, desea volver a imprimr el ticket de anulación?", "Q");


                    if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                    {

                        pos = new POSEntities();
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "BasePagos"
                            , "GeneraPagoPINPADV2"
                            , $"Recupero la información de POS_VOUCHER; AUTORIZACION {AUTORIZACION} | NUMEROVOUCHER: {NUMEROVOUCHER}");


                        if (TipoTransaccion == "03" || TipoTransaccion == "04")
                        {
                            signo = -1;
                            tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherAnula;


                            try
                            {

                                Pago = new RepuestaPago();
                                Pago.GrupoTarjeta = detVoucher.GRUPOTAR;
                                Pago.TipoTransaccion = detVoucher.TIPOTRANSACCION;
                                Pago.TID = detVoucher.TID;
                                Pago.MID = detVoucher.MID;
                                Pago.MerchantID = detVoucher.MID;
                                Pago.NumTarjetaTrunc = detVoucher.TARJETA;
                                Pago.FechaTrans = detVoucher.FECHACONSUMO;
                                Pago.HoraTrans = detVoucher.HORACONSUMO;
                                Pago.NumAutorizacion = detVoucher.AUTORIZACION;
                                Pago.NumLote = detVoucher.LOTE;
                                Pago.NombBcoAdq = detVoucher.BANCOADQUIRIENTE;
                                Pago.CodRed = detVoucher.AUTORIZADOR.ToString();
                                Pago.ARQC = detVoucher.ARQC.ToString();
                                Pago.EMV = detVoucher.EMV.ToString();
                                Pago.AID_EMV = detVoucher.AIDEMV.ToString();


                                string valoranulacion = detVoucher.VALORCONSUMO;
                                string valorinteres = detVoucher.VALORINTERES;
                                string valorT;

                                valoranulacion = (Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)) * -1).ToString("###,##0.00");
                                valorinteres = (Decimal.Parse(valorinteres.Substring(0, 11) + "." + valorinteres.Substring(11, 2)) * -1).ToString("###,##0.00");
                                valorT = (decimal.Parse(valoranulacion)).ToString("###,##0.00");
                                valoranulacion = valorT;


                                prepararVoucherTarjeta2(tipovoucher, Pago, detVoucher.NUMEROVOUCHER, detVoucher.AUTORIZACION);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "ImprimveVoucherAnulado", "Trama PINPAD: ");


                            }
                            catch (Exception ex)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "ImprimveVoucherAnulado", "Error: " + ex.InnerException + "| " + ex.Message);

                                Control.Common.General.GetMensaje("POS", "GeneraPagoPINPADV2: " + ex.InnerException + "| " + ex.Message, "ER");
                                return;
                            }

                        }
                    }

                    return;
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "ImprimveVoucherAnulado", "Error: " + ex.Message);
            }
            finally
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "ImprimveVoucherAnulado", "Finaliza proceso de impresion");
            }


        }

        private void btnCerrarLote_Click(object sender, EventArgs e)
        {

            return;

            var pos = new POSEntities();
            string fecha = DateTime.Now.Date.ToString("yyyyMMdd");
            ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
            Tramas.ProcesoControl pc = new Tramas.ProcesoControl();
            pc._numLoteDatafast = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().lote_datafast.ToString();
            pc._numLoteMedianet = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().lote_medianet.ToString();
            pc._secuenciaDatafast = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().secuencia_datafast.ToString();
            pc._secuenciaMedianet = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().secuencia_broadnet.ToString();
            pc._MIDDatafast = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;
            pc._TIDDatafast = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().TID_DATAFAST;
            pc._MIDMedianet = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;
            pc._TIDMedianet = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().TID;
            pc._CID = _factura.Establecimiento + _factura.PtoEmision;


            #region CIERRE MEDIANET

            //CREAR EL ARCHIVO DE LAS TRANSACCIONES  MEDIANET
            String nombre_archivo ="T05"+ "000001" + "0001"+"_"+fecha;


            // CREA CABECERA
            string establecimiento = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2.PadRight(15, ' ');//15 codigo asignado x red blancos a la derecha "000000821841";
            string numeropos = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault().TID.PadLeft(8,'0');//8 identificador del termninal asignado a la caja "TM000001";



            // CREA DETALLE
            string TARJETA = ("520081XXXXXX6017   "); //19 ;
            string CODIGOPROCESO = ("000200"); //6 ;
            string FECHACONSUMO = ("20161122"); //8 ;
            string HORACONSUMO = ("114339"); //6 ;
            string NUMEROVOUCHER = ("000002");//6 ;
            string AUTORIZACION = ("179132"); //6 ;
            string VALORCONSUMO = ("0000000001200"); //13 ;
            string FORMAAUTORIZA = ("1"); //1 ;
            string TIPOCONSUMO = ("CF"); //2 ;
            string PLAZO = ("06"); //2 ;
            string TIPOLECTURA = ("005"); //3 ;
            string TIPOMONEDA = ("840"); //3 ;
            string VALORIVA = ("0000000000147"); //13 ;
            string VALORSERVICIO = ("0000000000000"); //13 ;
            string VALORPROPINA = ("0000000000000"); //13 ;
            string VALORINTERES = ("0000000000057");  //13 ;
            string VALORFIJO = ("0000000000000");  //13 ;
            string TIPOPROMOCION = ("00"); //2 ;
            string MESESGRACIA = ("00");  //2 ;
            string EMPRESASERVICIO = ("0000");  //4 ;
            string ESTADOTRX = ("O"); //1 ;
            string CODIGORESPUESTA = ("00"); //2 ;
            string TIPODISPOSITIVO = ("2"); //1 ;
            string ADQUIRENTETARJETA = ("CREDIMATIC01"); //12 ;
            string ADQUIRENTESERVICIO = ("            ");  //12 ;
            string MONTOGRAVAIVA = ("0000000001053"); //13 ;
            string MONTONOGRAVAIVA = ("0000000000000");  //13 ;
           /*
            String detalle = "D"+ TARJETA + CODIGOPROCESO + FECHACONSUMO + HORACONSUMO + NUMEROVOUCHER + AUTORIZACION + VALORCONSUMO + FORMAAUTORIZA + TIPOCONSUMO + PLAZO + TIPOLECTURA + TIPOMONEDA + VALORIVA + VALORSERVICIO + VALORPROPINA + VALORINTERES + VALORFIJO + TIPOPROMOCION + MESESGRACIA + EMPRESASERVICIO + ESTADOTRX + CODIGORESPUESTA + TIPODISPOSITIVO + ADQUIRENTETARJETA + ADQUIRENTESERVICIO + MONTOGRAVAIVA + MONTONOGRAVAIVA ;
            */
            StringBuilder tmpdetalle = new StringBuilder();
            int cont = 0;
            decimal contotalventa = 0;
            var det = pos.POS_VOUCHER.Where(x => x.PUNTOEMISION == _factura.Establecimiento + _factura.PtoEmision && x.FECHACONSUMO == fecha && x.PROCESADO == false && x.AUTORIZADOR==2 && x.ANULADO==false);
            foreach (var linea in det)//llega a esta linea y se sale del foreach
            {
                TARJETA = linea.TARJETA;
                CODIGOPROCESO = linea.CODIGOPROCESO;
                FECHACONSUMO = linea.FECHACONSUMO;
                HORACONSUMO = linea.HORACONSUMO;
                NUMEROVOUCHER = linea.NUMEROVOUCHER;
                AUTORIZACION = linea.AUTORIZACION;
                VALORCONSUMO = linea.VALORCONSUMO;
                contotalventa += decimal.Parse(VALORCONSUMO.Substring(0,11)+"."+VALORCONSUMO.Substring(11,2));
                FORMAAUTORIZA = linea.FORMAAUTORIZA;
                TIPOCONSUMO = linea.TIPOCONSUMO;
                PLAZO = linea.PLAZO;
                TIPOLECTURA = linea.TIPOLECTURA;
                TIPOMONEDA = linea.TIPOMONEDA;
                VALORIVA = linea.VALORIVA;
                VALORSERVICIO = linea.VALORSERVICIO;
                VALORPROPINA = linea.VALORPROPINA;
                VALORINTERES = linea.VALORINTERES;
                VALORFIJO = linea.VALORFIJO;
                TIPOPROMOCION = linea.TIPOPROMOCION;
                MESESGRACIA = linea.MESESGRACIA;
                EMPRESASERVICIO = linea.EMPRESASERVICIO;
                ESTADOTRX = linea.ESTADOTRX;
                CODIGORESPUESTA = linea.CODIGORESPUESTA;
                TIPODISPOSITIVO = linea.TIPODISPOSITIVO;
                ADQUIRENTETARJETA = linea.ADQUIRENTETARJETA;
                ADQUIRENTESERVICIO = linea.ADQUIRENTESERVICIO;
                MONTOGRAVAIVA = linea.MONTOGRAVAIVA;
                 MONTONOGRAVAIVA = linea.MONTONOGRAVAIVA;
                cont++;
                tmpdetalle.AppendLine("D" + TARJETA + CODIGOPROCESO + FECHACONSUMO + HORACONSUMO + NUMEROVOUCHER + AUTORIZACION + VALORCONSUMO + FORMAAUTORIZA + TIPOCONSUMO + PLAZO + TIPOLECTURA + TIPOMONEDA + VALORIVA + VALORSERVICIO + VALORPROPINA + VALORINTERES + VALORFIJO + TIPOPROMOCION + MESESGRACIA + EMPRESASERVICIO + ESTADOTRX + CODIGORESPUESTA + TIPODISPOSITIVO + ADQUIRENTETARJETA + ADQUIRENTESERVICIO + MONTOGRAVAIVA + MONTONOGRAVAIVA);
            }


            //CREA TOTALES
            string numerosecuencial = "000000";
            string id = "000000";
            string numeroregistro = (cont).ToString().PadLeft(6, '0');

            String totales = "TN"+ fecha + numerosecuencial+id+numeroregistro+ "     "+("").PadRight(127,' ');



            StringBuilder txtarchivo = new StringBuilder();
            //cambiar aqui
            string cantidadvouchers = (cont).ToString().PadLeft(6, '0');//"000017";
            string totalventa = (contotalventa).ToString().Replace(".","").PadLeft(13, '0'); //"0000000043000";
            //cambiar aqui

            string nombreestablecimiento = ("").PadRight(25, ' ');
            string ciudadestablecimiento = ("").PadRight(12, ' ');

            String cabecera = "C" + establecimiento + fecha + pc._numLoteMedianet + numeropos + cantidadvouchers + totalventa + nombreestablecimiento + ciudadestablecimiento+("").PadRight(65,' ');


            txtarchivo.AppendLine(cabecera);
            //cambiar aqui
            txtarchivo.Append(tmpdetalle.ToString());
            txtarchivo.AppendLine(totales);

            // graba el archivo en disco
            //System.IO.File.WriteAllText(@"D:\ARCHIVO_POS\" + nombre_archivo+".txt", txtarchivo.ToString());
            System.IO.File.WriteAllText(@"C:\Log\" + nombre_archivo + ".txt", txtarchivo.ToString());



            var mov = pos.POS_VOUCHER.Where(x => x.FECHACONSUMO == fecha && x.AUTORIZADOR==2).OrderBy(y=> y.GRUPOTAR+y.AUTORIZACION+y.TIPOCONSUMO);
            string lineamov = "";
            string grupoAux = "";
            foreach (var linea in mov)//llega a esta linea y se sale del foreach
            {
                if(grupoAux!= linea.GRUPOTAR)
                {
                    grupoAux = linea.GRUPOTAR;
                    lineamov += linea.GRUPOTAR+"\n";
                    //sumatoria
                }
                lineamov+=  linea.AUTORIZACION + " " + linea.TIPOCONSUMO + " "+ decimal.Parse(linea.VALORCONSUMO.Substring(0,11)).ToString()+"."+linea.VALORCONSUMO.Substring(11,2)+"\n";
            }

            string texto="*CIERRE TERMINAL*\n LOTE#: "+ pc._numLoteMedianet+"\n TERMINAL:"+ pc._TIDMedianet + "\n FECHA:"+ fecha+"\n HORA: "+ DateTime.Now.ToShortTimeString()+"\nMOVIMIENTOS:\n" +
            lineamov;

            printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
            printer.TextToPrint = texto;
            printer.Print();

    
            #endregion
            #region CIERRE DATAFAST

            //CREAR EL ARCHIVO DE LAS TRANSACCIONES  MEDIANET
            String nombre_archivo_datafast = fecha;


            // CREA CABECERA
            establecimiento = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2.PadRight(10, ' ');//15 codigo asignado x red blancos a la derecha "000000821841";
            
            numeropos = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID.PadLeft(8, '0');//8 identificador del termninal asignado a la caja "TM000001";



            // CREA DETALLE
            string TIPOREGISTRO = "1";
            string TID          = "".PadRight(8, ' ');
            string FECHATRAN    = "".PadRight(6, ' ');
            string HORATRAN     = "".PadRight(6, ' ');
            string NUMREF       = "".PadRight(6, ' ');
            string NUMAUT       = "".PadRight(6, ' ');
            string NUMLOTE      = "".PadRight(6, ' ');
            string TOTALTRANS   = "".PadLeft(13, '0');
            string INDICTRANS   = "".PadRight(1, ' ');
            string TIPOCREDITO  = "".PadRight(2, ' '); //DIFERIDO TABLA 1
            string NUMCUOTAS    = "".PadLeft(2, '0');
            string VALORIVADATA = "".PadLeft(13, '0');
            string VALORSERVDATA = "".PadLeft(13, '0');
            string VALORPROPDATA = "".PadLeft(13, '0');
            string VALORINTERESDATA = "".PadLeft(13, '0');
            string VALORMONTOFIJODATA = "".PadLeft(13, '0');
            string VALORICEDATA = "".PadLeft(13, '0');
            string VALOROTROSIMPDATA = "".PadLeft(13, '0');
            string VALORCASHOVERDATA = "".PadLeft(13, '0');
            string VALORBASE0DATA = "".PadLeft(13, '0');
            string VALORBASE14DATA = "".PadLeft(13, '0');
            string FILLERDATA = "".PadRight(13, ' ');


             
            tmpdetalle = new StringBuilder();
            cont = 0;
            contotalventa = 0;
            det = pos.POS_VOUCHER.Where(x => x.PUNTOEMISION == _factura.Establecimiento + _factura.PtoEmision && x.FECHACONSUMO == fecha && x.PROCESADO == false && x.AUTORIZADOR == 1 && x.ANULADO == false);
            foreach (var linea in det)//llega a esta linea y se sale del foreach
            {

                TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST.PadRight(8, ' ');
                FECHATRAN = linea.FECHACONSUMO.Substring(2,6);
                HORATRAN = linea.HORACONSUMO.PadRight(6, ' ');
                NUMREF = linea.NUMEROVOUCHER.PadRight(6, ' ');
                NUMAUT = linea.AUTORIZACION.PadRight(6, ' ');
                NUMLOTE = linea.LOTE.PadRight(6, ' ');

                var valor_total_mas_interes = decimal.Parse(linea.VALORCONSUMO.Substring(0, 11) + "." + linea.VALORCONSUMO.Substring(11, 2)) + decimal.Parse(linea.VALORINTERES.Substring(0, 11) + "." + linea.VALORINTERES.Substring(11, 2)); 
                TOTALTRANS = valor_total_mas_interes.ToString().Replace(".","").PadLeft(13, '0');

//                TOTALTRANS = linea.VALORCONSUMO.PadLeft(13, '0');
                INDICTRANS = "1".PadRight(1, ' ');
                TIPOCREDITO = linea.TIPOCONSUMO.PadRight(2, ' '); //DIFERIDO TABLA 1
                NUMCUOTAS = linea.PLAZO.PadLeft(2, '0');
                VALORIVADATA = linea.VALORIVA.PadLeft(13, '0');
                VALORSERVDATA = linea.VALORSERVICIO.PadLeft(13, '0');
                VALORPROPDATA = linea.VALORPROPINA.PadLeft(13, '0');
                VALORINTERESDATA = linea.VALORINTERES.PadLeft(13, '0');
                VALORMONTOFIJODATA = linea.VALORFIJO.PadLeft(13, '0');
                VALORICEDATA = "".PadLeft(13, '0');
                VALOROTROSIMPDATA = "".PadLeft(13, '0');
                VALORCASHOVERDATA = "".PadLeft(13, '0');
                VALORBASE0DATA = linea.MONTONOGRAVAIVA.PadLeft(13, '0');
                VALORBASE14DATA = linea.MONTOGRAVAIVA.PadLeft(13, '0');
               
                cont++;
                tmpdetalle.AppendLine(TIPOREGISTRO + TID + FECHATRAN + HORATRAN + NUMREF + NUMAUT + NUMLOTE + TOTALTRANS + INDICTRANS + TIPOCREDITO + NUMCUOTAS
                    + VALORIVADATA + VALORSERVDATA + VALORPROPDATA + VALORINTERESDATA + VALORMONTOFIJODATA + VALORICEDATA + VALOROTROSIMPDATA + VALORCASHOVERDATA + VALORBASE0DATA + VALORBASE14DATA + FILLERDATA);

            }

            cont += 1;
            //CREA TOTALES
           numeroregistro = cont.ToString().PadLeft(6,'0') ;

           
            totales = "9" + fecha.Substring(2,6) +  numeroregistro + ("").PadRight(187, ' ');
            txtarchivo = new StringBuilder();
    

           
            cabecera = "1" + establecimiento + fecha.Substring(2, 6) + ("").PadRight(183, ' ');
            /*
             Tipo de registro 1 N Valor fijo = 1 
             Código de comercio 10 N MID asignado al comercio 
             Fecha de transmisión 6 N AAMMDD 
             Filler 183 AN Espacios 
             */

            txtarchivo.AppendLine(cabecera);
            //cambiar aqui
            txtarchivo.Append(tmpdetalle.ToString());
            txtarchivo.AppendLine(totales);

            // graba el archivo en disco
            //System.IO.File.WriteAllText(@"D:\ARCHIVO_POS\" + nombre_archivo_datafast + ".LRS", txtarchivo.ToString());
            System.IO.File.WriteAllText(@"C:\Log\" + nombre_archivo_datafast + ".LRS", txtarchivo.ToString());



            mov = pos.POS_VOUCHER.Where(x => x.FECHACONSUMO == fecha && x.AUTORIZADOR==1).OrderBy(y => y.GRUPOTAR + y.AUTORIZACION + y.TIPOCONSUMO);
            lineamov = "";
            grupoAux = "";
            foreach (var linea in mov)//llega a esta linea y se sale del foreach
            {
                if (grupoAux != linea.GRUPOTAR)
                {
                    grupoAux = linea.GRUPOTAR;
                    lineamov += linea.GRUPOTAR + "\n";
                    //sumatoria
                }
                lineamov += linea.AUTORIZACION + " " + linea.TIPOCONSUMO + " " + decimal.Parse(linea.VALORCONSUMO.Substring(0, 11)).ToString() + "." + linea.VALORCONSUMO.Substring(11, 2) + "\n";
            }
            texto = "*CIERRE TERMINAL*\n LOTE#: " + pc._numLoteDatafast + "\n TERMINAL:" + pc._TIDDatafast + "\n FECHA:" + fecha + "\n HORA: " + DateTime.Now.ToShortTimeString() + "\nMOVIMIENTOS:\n" +
            lineamov;
            printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
            printer.TextToPrint = texto;
            printer.Print();


            #endregion

            int timeOut_ = 40000;
            int.TryParse(pos.core_parametro.Where(x => x.identificador == "PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOut_);
            string trama = pc.DevuelveTramaPC;// "PC000001000001000001000001codigolirisdataiddata01codigolirismediiddata01CAJATEST01     ";
            txtResult.Text = envio.SendRequestPinpad("", PUERTOCOM, timeOut_, trama, "", 1);


            // Validar que resuelve ok el proceso de borrado
            var dbProceso = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmisionOrigen).FirstOrDefault();
            dbProceso.lote_datafast += 1;
            dbProceso.lote_medianet += 1;
            pos.POS_VOUCHER.Where(x => x.PUNTOEMISION == _factura.Establecimiento + _factura.PtoEmision && x.FECHACONSUMO == fecha && x.PROCESADO == false ).ToList().ForEach(x => x.PROCESADO = true);
            pos.SaveChanges();

        }

        private void btnProcesoActualizacion_Click(object sender, EventArgs e)
        {
            return;

            var pos = new POSEntities();
            ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
            int timeOutPA = 40000;
            int.TryParse(pos.core_parametro.Where(x => x.identificador == "PA_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutPA);

            string trama = string.Empty;
            if (!string.IsNullOrEmpty(trama))
            {
                // string trama = "PA2APPFINFT70          LI029002000000862698   10.10.30.200:5008    P";
                txtResult.Text = envio.SendRequestPinpad("", PUERTOCOM, timeOutPA, trama, "", 1);
            }


        }

        private void btnCP_Click(object sender, EventArgs e)
        {
            return;

            var pos = new POSEntities();
            ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
            Tramas.ConfiguraPinPad objTrama = new Tramas.ConfiguraPinPad();

            int timeOutCP = 40000;
            int.TryParse(pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutCP);
            string trama = pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET").FirstOrDefault().valor;
            
            //string cad = Microsoft.VisualBasic.Interaction.InputBox("trama","", trama);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP192.168.126.242255.255.255.0  192.168.126.154 192.168.61.75 3000                       10.10.30.100   7784                             ", "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP 192.168.132.55255.255.255.0    192.168.132.1192.168.61.250 3000                         10.100.1.3   7350                             ", "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP 192.168.132.55255.255.255.0    192.168.132.1192.168.61.250 3000                         10.100.1.3   7350                             ", "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP192.168.128.106255.255.255.0  192.168.128.1  192.168.61.250 3000                       10.10.3.35     7350                             ", "", 1);
            txtResult.Text = envio.SendRequestPinpad("", 9, timeOutCP, trama, "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, trama, "", 1);

        }

        private void cmbTipoTransaccion_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            var db = new POSEntities();
            try
            {
                cmbBancoTarjeta.DataSource = db.pos_tarjeta_tipopago.Where(x => x.idTipo == ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo).ToList();
                cmbBancoTarjeta.DataMember = "idPago";
                cmbBancoTarjeta.DisplayMember = "NombreTipoPago";
                switch (((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo)
                {
                    case "03":
                        boxAnula.Top=157;
                        boxAnula.Left = 21;
                        boxAnula.Visible = true;
                        // boxDiferido.Visible = false;
                        boxDiferido.Visible = true;
                        boxDiferido.Top = 10;
                        boxDiferido.Left = 480;

                        boxReimpresion.Top = 157;
                        boxReimpresion.Left = 480;
                        boxReimpresion.Visible = true;

                        btnPagar.Text = "Anular";
                        break;
                    case "02":
                        boxAnula.Visible = false;
                        boxReimpresion.Visible = false;
                        boxDiferido.Visible = true;
                        boxDiferido.Top = 157;
                        boxDiferido.Left = 21;
                        //this.boxDiferido.Location = new System.Drawing.Point(21, 157);
                        btnPagar.Text = "Realizar Pago";
                        break;
                    case "05":
                        boxAnula.Visible = false;
                        boxDiferido.Visible = false;
                        boxReimpresion.Top = 157;
                        boxReimpresion.Left = 21;
                        boxReimpresion.Visible = true;
                        btnPagar.Text = "Reimprimir";
                        break;
                    default:
                        boxAnula.Visible = false;
                        boxDiferido.Visible = false;
                        boxReimpresion.Visible = false;
                        btnPagar.Text = "Realizar Pago";
                        break;

                }

            }
            catch
            {

            }
           

        }

        public void SendEmail(string Txtto, string Txttosupervisor, string Txtsubject, string Txtbody, string Adjunto)
        {
            using (MailMessage Mm = new MailMessage())
            {
                //My.Settings.usuario.ToString, Txtto)
                Mm.From = new System.Net.Mail.MailAddress("aplicacion@liris.com.ec", "LIRIS Generacion voucher");
                Mm.Sender = new System.Net.Mail.MailAddress("aplicacion@liris.com.ec", "LIRIS Generacion voucher");
                Mm.ReplyToList.Add(new System.Net.Mail.MailAddress("aplicacion@liris.com.ec", "LIRIS Generacion voucher"));
                Mm.Subject = Txtsubject;
                Mm.Body = Txtbody;
                Mm.To.Add(Txtto);
                if (Txttosupervisor.Length > 0)
                {
                    Mm.CC.Add(Txttosupervisor);
                }

             ////   if (CuadreCajaPOSWeb.Properties.Settings.Default.Debug)
             //   {
             //       //Mm.Bcc.Add(CuadreCajaPOSWeb.Properties.Settings.Default.emailDebug);
             //       Mm.Bcc.Add("brayengavilanes@gmail.com");
             //   }

                if ((Adjunto.Length > 0))
                {
                    Mm.Attachments.Add(new Attachment(Adjunto));
                }
                Mm.IsBodyHtml = true;
                //Dim HTMLConImagenes As AlternateView
                //HTMLConImagenes = AlternateView.CreateAlternateViewFromString(Txtbody, Nothing, "text/html")
                //For a = 1 To 39
                //    If Not IsNothing(arreglofotos(a, 1)) Then
                //        Dim imagen1 As LinkedResource
                //        imagen1 = New LinkedResource(arreglofotos(a, 1).ToString)
                //        imagen1.ContentId = arreglofotos(a, 0)
                //        HTMLConImagenes.LinkedResources.Add(imagen1)
                //    End If
                //Next

                //Mm.AlternateViews.Add(HTMLConImagenes)

                SmtpClient Smtp = new SmtpClient();

                Smtp.Host = "mail.liris.com.ec";// CuadreCajaPOSWeb.Properties.Settings.Default.smtp.ToString();
                Smtp.Port = 587;// CuadreCajaPOSWeb.Properties.Settings.Default.puerto;
                Smtp.EnableSsl = false; // CuadreCajaPOSWeb.Properties.Settings.Default.EnableSSL;
                Smtp.UseDefaultCredentials = false;// CuadreCajaPOSWeb.Properties.Settings.Default.UseDefaultCredentials;

                //NetworkCredential Networkcred = new NetworkCredential(CuadreCajaPOSWeb.Properties.Settings.Default.usuario.ToString(), CuadreCajaPOSWeb.Properties.Settings.Default.password.ToString());
                NetworkCredential Networkcred = new NetworkCredential("aplicacion@liris.com.ec", "l1r1sserver+-*123");
                Smtp.Credentials = Networkcred;
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                try
                {
                    Smtp.Send(Mm);
                }
                catch (Exception Ex)
                {

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = Ex.ToString() });
                    Control.Common.General.GetMensajeToList(540, parametros);

                    //MessageBox.Show(this,Ex.ToString());
                    //ClientScript.RegisterStartupScript(GetType(), "Alert", "javascript:alert('" + Ex.ToString() + "');", true);
                }
            }
        }

        private void ObtieneArchivo(string ftpServerName, string ftpUser, string ftpPwd, string filenameToGet, string PathToPlaceFile)
        {
            //Get Files - ideally you would want to wrap this into a try...catch 
            //and possible execute more than once if you can't connect the first time. 
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = ftpServerName, //hostname e.g. IP: 192.54.23.32, or mysftpsite.com
                UserName = ftpUser,
                Password = ftpPwd,
                PortNumber = 22,
                SshHostKeyFingerprint = "ssh-rsa 2048 78:b6:8c:c9:00:ec:22:b1:ad:0f:f4:99:0c:74:0f:46"
            };
            using (Session session = new Session())
            {
                session.SessionLogPath = "D:\\ARCHIVO_POS\\bajados\\log.txt";
                session.Open(sessionOptions); //Attempts to connect to your sFtp site
                                              //Get Ftp File
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary; //The Transfer Mode - Automatic, Binary, or Ascii 
                transferOptions.FilePermissions = null;  //Permissions applied to remote files; 
                                                         //<em style="font-size: 9pt;">null for default permissions.  
                                                         //Can set <em style="font-size: 9pt;">user, Group, or other Read/Write/Execute permissions.  
                transferOptions.PreserveTimestamp = false;  //Set last write time of destination file 
                                                            //to that of source file - basically change the timestamp to match destination and source files.    
                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                TransferOperationResult transferResult;
                //the parameter list is: remote Path, Local Path with filename 
                //(optional - if different from remote path), Delete source file?, transfer Options  
                transferResult = session.GetFiles("/" + filenameToGet, PathToPlaceFile, false, transferOptions);
                //Throw on any error 
                transferResult.Check();
                //Log information and break out if necessary  

            }
        }

        private void SubeArchivo(int cadena,string ftpServerName, string ftpUser, string ftpPwd, string localPathWithFilename, string RemotePath)
        {

            //Send Ftp Files - same idea as above - try...catch and try to repeat this code 
            //if you can't connect the first time, timeout after a certain number of tries. 
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = ftpServerName, //hostname e.g. IP: 192.54.23.32, or mysftpsite.com
                UserName = ftpUser,
                Password = ftpPwd,
                PortNumber = cadena == 2 ? 22: 5344,                
                SshHostKeyFingerprint = cadena==2?"ssh-rsa 2048 78:b6:8c:c9:00:ec:22:b1:ad:0f:f4:99:0c:74:0f:46": "ssh-rsa 2048 7c:1f:ef:5b:eb:c6:c2:4c:fa:9f:bd:a8:74:e5:7e:ef"
            };
            using (Session session = new Session())
            {
                session.SessionLogPath = "D:\\ARCHIVO_POS\\bajados\\log-subidas.txt";
                session.Open(sessionOptions); //Attempts to connect to your sFtp site
                                              //Get Ftp File
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary; //The Transfer Mode - 
                                                                    //&lt;em style="font-size: 9pt;">Automatic, Binary, or Ascii  
                transferOptions.FilePermissions = null; //Permissions applied to remote files; 
                                                        //null for default permissions.  Can set user, 
                                                        //Group, or other Read/Write/Execute permissions. 
                transferOptions.PreserveTimestamp = false; //Set last write time of 
                                                           //destination file to that of source file - basically change the timestamp 
                                                           //to match destination and source files.   
                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                TransferOperationResult transferResult;
                //the parameter list is: local Path, Remote Path, Delete source file?, transfer Options  
                transferResult = session.PutFiles(localPathWithFilename, RemotePath, false, transferOptions);
                //Throw on any error 
                transferResult.Check();
                //Log information and break out if necessary  
            }
        }


        private void btnCreaArch_Click(object sender, EventArgs e)
        {

            string fecha;
            if (MessageBox.Show(this,"Procesar archivo Medianet", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                #region medianet
                using (POSEntities db = new POSEntities())
                {
                    string hora = DateTime.Now.ToString("HH:mm");
                    //if (db.core_EnviosArchivosVoucher.Any(x => x.Hora == hora))
                    {
                        //var horario = db.core_EnviosArchivosVoucher.Where(x => x.Hora == hora).FirstOrDefault();
                        //File.AppendAllText("D:\\ARCHIVO_POS\\servicio\\log.txt", horario.email + " " + horario.Cadena + " " + horario.Hora + "\n\r");

                        ;


                        fecha = DateTime.Now.Date.ToString("yyyyMMdd");

                        #region CIERRE MEDIANET
                        var lotes = db.POS_VOUCHER.Where(x => x.PROCESADO == false && x.PROCESADOTURNO == true && x.ANULADO == false).GroupBy(x => x.PUNTOEMISION + x.LOTE);
                        string registroarchivos = "";
                        int intcont = int.Parse(db.core_parametro.Where(x => x.identificador == "PINPAD_SEC_MEDIANET").FirstOrDefault().valor);
                        bool enviamail=false;
                        int contfile = 0;

                        foreach (var linlote in lotes)//llega a esta linea y se sale del foreach
                        {
                            StringBuilder txtarchivo = new StringBuilder();
                            // MessageBox.Show(this,"lote: " + linea.FirstOrDefault().LOTE + " punto emision: " + linea.FirstOrDefault().PUNTOEMISION);
                            intcont += 1;
                            // MessageBox.Show(this,(intcont).ToString().Trim().PadLeft(4, '0'));

                            //CREAR EL ARCHIVO DE LAS TRANSACCIONES  MEDIANET
                            string secuencial = (intcont).ToString().Trim().PadLeft(4, '0'); // "0030";
                            String nombre_archivo = "T05DCORRAL" + secuencial + "_" + fecha;

                            string numestablecimiento = linlote.FirstOrDefault().PUNTOEMISION.Substring(0, 3);// "029";
                            string ptoemision = linlote.FirstOrDefault().PUNTOEMISION.Substring(3, 3); //"008";
                            string lote = linlote.FirstOrDefault().LOTE.PadLeft(7, '0'); //("000001").PadLeft(7, '0');



                            // CREA CABECERA
                            string establecimiento = db.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == numestablecimiento).FirstOrDefault().parametro2.PadRight(15, ' ');//15 codigo asignado x red blancos a la derecha "000000821841";
                            string numeropos = db.core_puntoemision.Where(x => x.establecimiento_id == numestablecimiento && x.punto_emision == ptoemision).FirstOrDefault().TID.PadLeft(8, '0');//8 identificador del termninal asignado a la caja "TM000001";

                            // CREA DETALLE

                            string TARJETA; //19 ;
                            string CODIGOPROCESO; //6 ;
                            string FECHACONSUMO; //8 ;
                            string HORACONSUMO; //6 ;
                            string NUMEROVOUCHER;//6 ;
                            string AUTORIZACION; //6 ;
                            string VALORCONSUMO; //13 ;
                            string FORMAAUTORIZA; //1 ;
                            string TIPOCONSUMO; //2 ;
                            string PLAZO; //2 ;
                            string TIPOLECTURA; //3 ;
                            string TIPOMONEDA; //3 ;
                            string VALORIVA; //13 ;
                            string VALORSERVICIO; //13 ;
                            string VALORPROPINA; //13 ;
                            string VALORINTERES;  //13 ;
                            string VALORFIJO;  //13 ;
                            string TIPOPROMOCION; //2 ;
                            string MESESGRACIA;  //2 ;
                            string EMPRESASERVICIO;  //4 ;
                            string ESTADOTRX; //1 ;
                            string CODIGORESPUESTA; //2 ;
                            string TIPODISPOSITIVO; //1 ;
                            string ADQUIRENTETARJETA; //12 ;
                            string ADQUIRENTESERVICIO;  //12 ;
                            string MONTOGRAVAIVA; //13 ;
                            string MONTONOGRAVAIVA;  //13 ;
                                                     /*
                                                     String detalle = "D"+ TARJETA + CODIGOPROCESO + FECHACONSUMO + HORACONSUMO + NUMEROVOUCHER + AUTORIZACION + VALORCONSUMO + FORMAAUTORIZA + TIPOCONSUMO + PLAZO + TIPOLECTURA + TIPOMONEDA + VALORIVA + VALORSERVICIO + VALORPROPINA + VALORINTERES + VALORFIJO + TIPOPROMOCION + MESESGRACIA + EMPRESASERVICIO + ESTADOTRX + CODIGORESPUESTA + TIPODISPOSITIVO + ADQUIRENTETARJETA + ADQUIRENTESERVICIO + MONTOGRAVAIVA + MONTONOGRAVAIVA ;
                                                     */
                            StringBuilder tmpdetalle = new StringBuilder();
                            int cont = 0;
                            decimal contotalventa = 0;

                            var det = db.POS_VOUCHER.Where(x => x.PROCESADO == false && x.PROCESADOTURNO == true && x.ANULADO == false &&
                            x.AUTORIZADOR == 2 && x.PUNTOEMISION == numestablecimiento + ptoemision && x.LOTE == lote.Substring(1, 6)).OrderBy(x => x.PUNTOEMISION + x.LOTE + x.NUMEROVOUCHER);

                            foreach (var linea in det)//llega a esta linea y se sale del foreach
                            {
                                TARJETA = linea.TARJETA;
                                CODIGOPROCESO = linea.CODIGOPROCESO;
                                FECHACONSUMO = linea.FECHACONSUMO;
                                HORACONSUMO = linea.HORACONSUMO;
                                NUMEROVOUCHER = linea.NUMEROVOUCHER;
                                AUTORIZACION = linea.AUTORIZACION;
                                VALORCONSUMO = linea.VALORCONSUMO;
                                contotalventa += decimal.Parse(VALORCONSUMO.Substring(0, 11) + "." + VALORCONSUMO.Substring(11, 2));
                                FORMAAUTORIZA = linea.FORMAAUTORIZA;
                                TIPOCONSUMO = linea.TIPOCONSUMO;
                                PLAZO = linea.PLAZO;
                                TIPOLECTURA = linea.TIPOLECTURA;
                                TIPOMONEDA = linea.TIPOMONEDA;
                                VALORIVA = linea.VALORIVA;
                                VALORSERVICIO = linea.VALORSERVICIO;
                                VALORPROPINA = linea.VALORPROPINA;
                                VALORINTERES = linea.VALORINTERES;
                                VALORFIJO = linea.VALORFIJO;
                                TIPOPROMOCION = linea.TIPOPROMOCION;
                                MESESGRACIA = linea.MESESGRACIA;
                                EMPRESASERVICIO = linea.EMPRESASERVICIO;
                                ESTADOTRX = linea.ESTADOTRX;
                                CODIGORESPUESTA = linea.CODIGORESPUESTA;
                                TIPODISPOSITIVO = linea.TIPODISPOSITIVO;
                                ADQUIRENTETARJETA = linea.ADQUIRENTETARJETA;
                                ADQUIRENTESERVICIO = linea.ADQUIRENTESERVICIO;
                                MONTOGRAVAIVA = linea.MONTOGRAVAIVA;
                                MONTONOGRAVAIVA = linea.MONTONOGRAVAIVA;
                                cont++;
                                tmpdetalle.AppendLine("D" + TARJETA + CODIGOPROCESO + FECHACONSUMO + HORACONSUMO + NUMEROVOUCHER + AUTORIZACION + VALORCONSUMO +
                                    FORMAAUTORIZA + TIPOCONSUMO + PLAZO + TIPOLECTURA + TIPOMONEDA + VALORIVA + VALORSERVICIO + VALORPROPINA + VALORINTERES +
                                    VALORFIJO + TIPOPROMOCION + MESESGRACIA + EMPRESASERVICIO + ESTADOTRX + CODIGORESPUESTA + TIPODISPOSITIVO + ADQUIRENTETARJETA +
                                    ADQUIRENTESERVICIO + MONTOGRAVAIVA + MONTONOGRAVAIVA);
                                linea.PROCESADO = true;
                            }


                            ////CREA TOTALES
                            //string numerosecuencial = "000000";
                            string id = "000000";
                            string numeroregistro = (cont).ToString().PadLeft(6, '0');

                            String totales = "TN" + fecha + secuencial.PadLeft(6, '0') + id + numeroregistro + "     " + ("").PadRight(127, ' ');



                            string cantidadvouchers = (cont).ToString().PadLeft(6, '0');//"000017";
                            string totalventa = (contotalventa).ToString().Replace(".", "").PadLeft(13, '0'); //"0000000043000";
                                                                                                              //                                                                                 
                                                                                                              //cambiar aqui
                            string nombreestablecimiento = ("").PadRight(25, ' ');
                            string ciudadestablecimiento = ("").PadRight(12, ' ');

                            String cabecera = ("C" + establecimiento.Substring(0, 12) + fecha + lote + numeropos + cantidadvouchers +
                                   totalventa + nombreestablecimiento + ciudadestablecimiento + ("").PadRight(65, ' ')).PadRight(160, ' ');


                            txtarchivo.AppendLine(cabecera);
                            txtarchivo.Append(tmpdetalle.ToString());
                            txtarchivo.AppendLine(totales);

                            // graba el archivo en disco
                            System.IO.File.WriteAllText(@"D:\ARCHIVO_POS\" + nombre_archivo + ".txt", txtarchivo.ToString());

                            #endregion
                            
                            if (det.Count() > 0)
                            {
                                contfile += 1;
                                enviamail = true;
                                SubeArchivo(2,"200.41.10.202", "usic08001", "Mv#5+4hAP7&mr76R", @"D:\ARCHIVO_POS\" + nombre_archivo + ".txt", "archxprocesar/" + nombre_archivo + ".txt");
                                registroarchivos +=  "<tr><td>"+ nombre_archivo + "</td><td>"+ (cont).ToString() + "</td></tr>";
                                //SendEmail("jpizarro@credimatic.com,operador@credimatic.com,supervisores@credimatic.com,jmackliff@bolivariano.com", "devteam@liris.com.ec,pgarcia@liris.com.ec,dorrala@liris.com.ec,atettke@liris.com.ec", "Envio de Archivo: " + nombre_archivo + "  Lote: " + lote + " Caja: " + numestablecimiento + ptoemision, "LIRIS acaba de transmitir el archivo " + nombre_archivo + " al adquiriente Medianet <br>" + (cont).ToString() + " Registros enviados", "");// @"D:\ARCHIVO_POS\" + nombre_archivo + ".txt");
                            }
                        }
                        if (enviamail)
                        {
                            SendEmail("jpizarro@credimatic.com,operador@credimatic.com,supervisores@credimatic.com,jmackliff@bolivariano.com", "devteam@liris.com.ec,pgarcia@liris.com.ec,dorrala@liris.com.ec,atettke@liris.com.ec", "Cierre CREDIMATIC " + fecha , "Estimados <br> Se han transferido los siguientes archivos: <table><tr><td>Nombre Archivo</td><td>Cant. Registros</td></tr>" + registroarchivos + "</table> al adquiriente Medianet <br>TOTAL ARCHIVOS ENVIADOS: "+ contfile.ToString() + "<br><br>Saludos cordiales" , "");// @"D:\ARCHIVO_POS\" + nombre_archivo + ".txt");
                        }
                        db.core_parametro.Where(x => x.identificador == "PINPAD_SEC_MEDIANET").FirstOrDefault().valor = intcont.ToString();
                        db.SaveChanges();
                        MessageBox.Show(this,"Proceso Medianet Terminado");

                    }

                }

                #endregion
            }
            if (MessageBox.Show(this,"Procesar archivo Datafast", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                #region datafast


                //CREAR EL ARCHIVO DE LAS TRANSACCIONES  MEDIANET

                fecha = DateTime.Now.Date.ToString("yyyyMMdd");
                String nombre_archivo_datafast = DateTime.Now.Date.ToString("ddMMyyyy"); ;

                using (POSEntities pos = new POSEntities())
                {
                    // CREA CABECERA
                    var establecimiento = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2.PadRight(10, ' ');//15 codigo asignado x red blancos a la derecha "000000821841";

                    // CREA DETALLE
                    string TIPOREGISTRO = "2";
                    string TID = "".PadRight(8, ' ');
                    string FECHATRAN = "".PadRight(6, ' ');
                    string HORATRAN = "".PadRight(6, ' ');
                    string NUMREF = "".PadRight(6, ' ');
                    string NUMAUT = "".PadRight(6, ' ');
                    string NUMLOTE = "".PadRight(6, ' ');
                    string TOTALTRANS = "".PadLeft(13, '0');
                    string INDICTRANS = "".PadRight(1, ' ');
                    string TIPOCREDITO = "".PadRight(2, ' '); //DIFERIDO TABLA 1
                    string NUMCUOTAS = "".PadLeft(2, '0');
                    string VALORIVADATA = "".PadLeft(13, '0');
                    string VALORSERVDATA = "".PadLeft(13, '0');
                    string VALORPROPDATA = "".PadLeft(13, '0');
                    string VALORINTERESDATA = "".PadLeft(13, '0');
                    string VALORMONTOFIJODATA = "".PadLeft(13, '0');
                    string VALORICEDATA = "".PadLeft(13, '0');
                    string VALOROTROSIMPDATA = "".PadLeft(13, '0');
                    string VALORCASHOVERDATA = "".PadLeft(13, '0');
                    string VALORBASE0DATA = "".PadLeft(13, '0');
                    string VALORBASE14DATA = "".PadLeft(13, '0');
                    string FILLERDATA = "".PadRight(13, ' ');




                    var tmpdetalle = new StringBuilder();
                    var cont = 0;
                    var contotalventa = 0;
                    // var det = pos.POS_VOUCHER.Where(x => x.PUNTOEMISION == _factura.Establecimiento + _factura.PtoEmision && x.FECHACONSUMO == fecha && x.PROCESADO == false && x.AUTORIZADOR == 1 && x.ANULADO == false);
                    var det = pos.POS_VOUCHER.Where(x => x.PROCESADO == false && x.AUTORIZADOR == 1 && x.ANULADO == false && x.PROCESADOTURNO == true);
                    foreach (var linea in det)//llega a esta linea y se sale del foreach
                    {

                        // TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST.PadRight(8, ' ');
                        TID = pos.core_puntoemision.Where(x => x.establecimiento_id + x.punto_emision == linea.PUNTOEMISION).FirstOrDefault().TID_DATAFAST.PadRight(8, ' ');
                        FECHATRAN = linea.FECHACONSUMO.Substring(2, 6);
                        HORATRAN = linea.HORACONSUMO.PadRight(6, ' ');
                        NUMREF = linea.NUMEROVOUCHER.PadRight(6, ' ');
                        NUMAUT = linea.AUTORIZACION.PadRight(6, ' ');
                        NUMLOTE = linea.LOTE.PadRight(6, ' ');

                        var valor_total_mas_interes = decimal.Parse(linea.VALORCONSUMO.Substring(0, 11) + "." + linea.VALORCONSUMO.Substring(11, 2)) + decimal.Parse(linea.VALORINTERES.Substring(0, 11) + "." + linea.VALORINTERES.Substring(11, 2));
                        TOTALTRANS = valor_total_mas_interes.ToString().Replace(".", "").PadLeft(13, '0');

                        //                TOTALTRANS = linea.VALORCONSUMO.PadLeft(13, '0');
                        INDICTRANS = "1".PadRight(1, ' ');
                        TIPOCREDITO = linea.TIPOCONSUMO.PadRight(2, ' '); //DIFERIDO TABLA 1
                        NUMCUOTAS = linea.PLAZO.PadLeft(2, '0');
                        VALORIVADATA = linea.VALORIVA.PadLeft(13, '0');
                        VALORSERVDATA = linea.VALORSERVICIO.PadLeft(13, '0');
                        VALORPROPDATA = linea.VALORPROPINA.PadLeft(13, '0');
                        VALORINTERESDATA = linea.VALORINTERES.PadLeft(13, '0');
                        VALORMONTOFIJODATA = linea.VALORFIJO.PadLeft(13, '0');
                        VALORICEDATA = "".PadLeft(13, '0');
                        VALOROTROSIMPDATA = "".PadLeft(13, '0');
                        VALORCASHOVERDATA = "".PadLeft(13, '0');
                        VALORBASE0DATA = linea.MONTONOGRAVAIVA.PadLeft(13, '0');
                        VALORBASE14DATA = linea.MONTOGRAVAIVA.PadLeft(13, '0');

                        cont++;
                        tmpdetalle.AppendLine(TIPOREGISTRO + TID + FECHATRAN + HORATRAN + NUMREF + NUMAUT + NUMLOTE + TOTALTRANS + INDICTRANS + TIPOCREDITO + NUMCUOTAS
                            + VALORIVADATA + VALORSERVDATA + VALORPROPDATA + VALORINTERESDATA + VALORMONTOFIJODATA + VALORICEDATA + VALOROTROSIMPDATA + VALORCASHOVERDATA + VALORBASE0DATA + VALORBASE14DATA + FILLERDATA);
                        linea.PROCESADO = true;
                    }

                    cont += 1;
                    //CREA TOTALES
                    var numeroregistro = cont.ToString().PadLeft(6, '0');


                    var totales = "9" + fecha.Substring(2, 6) + numeroregistro + ("").PadRight(187, ' ');



                    StringBuilder txtarchivo = new StringBuilder();



                    var cabecera = "1" + establecimiento + fecha.Substring(2, 6) + ("").PadRight(183, ' ');
                    /*
                     Tipo de registro 1 N Valor fijo = 1 
                     Código de comercio 10 N MID asignado al comercio 
                     Fecha de transmisión 6 N AAMMDD 
                     Filler 183 AN Espacios 
                     */

                    txtarchivo.AppendLine(cabecera);
                    //cambiar aqui
                    txtarchivo.Append(tmpdetalle.ToString());
                    txtarchivo.AppendLine(totales);

                    // graba el archivo en disco
                    System.IO.File.WriteAllText(@"D:\ARCHIVO_POS\" + nombre_archivo_datafast + ".LRS", txtarchivo.ToString());
                    if (det.Count() > 0)
                    {
                            SubeArchivo(1, "201.218.0.247", "sftp_liris", "NXnHQo9E", @"D:\ARCHIVO_POS\" + nombre_archivo_datafast + ".LRS", "Captura/" + nombre_archivo_datafast + ".LRS");
                            SendEmail("Operadores_gye@datafast.com.ec,dabad@datafast.com.ec,jfranco@datafast.com.ec", "devteam@liris.com.ec,pgarcia@liris.com.ec,dorrala@liris.com.ec,atettke@liris.com.ec", "Envio de Archivo: " + nombre_archivo_datafast , "LIRIS acaba de transmitir el archivo " + nombre_archivo_datafast + " al adquiriente Datafast <br>" + (cont).ToString() + " Registros enviados", "");// @"D:\ARCHIVO_POS\" + nombre_archivo + ".txt");
                    }
                    pos.SaveChanges();
                    MessageBox.Show(this,"Proceso Datafast Terminado");
                }
                #endregion
            }

        }

        private void btnKbd_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PagoPINPAD", "btnKbd_Click", "PagoPINPAD | TRAMA: Consulta Tarjeta ");
            //Control.Common.General.tecladoFlotante();
            Control.Common.General.TecladoPantalla();
        }

        private void txtFacReimpresion_TextChanged(object sender, EventArgs e)
        {
            lblFacReimpresion.Text = lblFacReimpresionAyuda.Text + txtFacReimpresion.Text.PadLeft(9, '0');
        }

        private void btnCP2_Click(object sender, EventArgs e)
        {
            return;

            var pos = new POSEntities();
            ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
            string trama = pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET").FirstOrDefault().valor;
            //string cad = Microsoft.VisualBasic.Interaction.InputBox("trama","", trama);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP192.168.126.242255.255.255.0  192.168.126.154 192.168.61.75 3000                       10.10.30.100   7784                             ", "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP 192.168.132.55255.255.255.0    192.168.132.1192.168.61.250 3000                         10.100.1.3   7350                             ", "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP 192.168.132.55255.255.255.0    192.168.132.1192.168.61.250 3000                         10.100.1.3   7350                             ", "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, "CP192.168.128.106255.255.255.0  192.168.128.1  192.168.61.250 3000                       10.10.3.35     7350                             ", "", 1);
            txtResult.Text = envio.SendRequestPinpad("", 9, 40000, "CP192.168.128.106255.255.255.0  192.168.128.1  192.168.61.250 3000                       10.10.3.35     7350                             ", "", 1);
            //txtResult.Text = envio.Envio_requerimientoPinpad("", 9, 40000, trama, "", 1);
        }
    }

}

