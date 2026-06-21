using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using POS.Models;
using POS.Control.Main.MainTouch;
using POS.Control.Main.MainTouchClte;

namespace POS.Control.ToolBox
{
    public partial class ToolBoxMenu : Telerik.WinControls.UI.RadForm
    {

        private MainWindow _mainWindow;
        
        public ToolBoxMenu(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void btnRecargas_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.ToolBox.ToolBoxMenu", "btnRecargas_Click", "Boton probar impresion Voucher presionado");

            using (POSEntities db = new POSEntities())
            {
                try
                {
                    PrintNewBasePagos();

                    //var objRecibo = db.core_recibo.Where(x => x.identificador == Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito).FirstOrDefault();
                    //var texto = objRecibo.cuerpo;

                    ////recibo = recibo.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE Y SIN \nPROTESTO EL TOTAL DE ESTE PAGARE MAS LOS INTERESES \nY CARGOS POR SERVICIO. EN CASO DE MORA PAGARE LA \nTASA MAXIMA AUTORIZADA POR EL EMISOR. DECLARO \nQUE EL PRODUCTO DE ESTA TRANSACCION NO SERA UTILI\nZADO EN ACTIVIDADES DE LAVADO DE ACTIVOS, FINANCIA\nMIENTO DEL TERRORISMO Y OTROS DELITOS ");

                    //DevuelveformatoCreditoPavos(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);

                    //texto = objRecibo.cuerpo;
                    //DevuelveformatoPagoPINPAD(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);

                    //texto = objRecibo.cuerpo;
                    //DevuelveformatoClsPagos(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);

                    //texto = objRecibo.cuerpo;
                    //DevuelveformatoBasePagos(ref texto);
                    //Control.Common.Printer.Imprimir(texto, 3, 10);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void PrintNewBasePagos()
        {
            var db = new POSEntities();

            var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
            recipe.NomTarjeta = "foobar";
            recipe.MID = "foobar";
            recipe.TID = "foobar";
            recipe.NumTarjeta = "436402XXXXXX2031";
            recipe.NumLote = "foobar";
            recipe.Adquiriente = "foobar";
            recipe.Aprobacion = "foobar";
            recipe.Secuencial = "foobar";
            recipe.NombreTarjetaHabiente = "foobar";
            recipe.FechaTrans = "2018/04/01";
            recipe.HoraTrans = "23:59:00";
            recipe.VenTarjeta = "XX/XXXX";
            recipe.Factura = "F-001-001-000000001";
            var valoranulacion = "";
            if (false)//pp.TipoTransaccion == "03")
            {
                //valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion).FirstOrDefault().VALORCONSUMO;
                //valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00");
            }
            decimal valorinteres = 500;

            recipe.ValorTotal = (500).ToString("###,##0.00");
            string modolectura = "";
            switch ("01")//trama.modoLectura)
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
            }
            recipe.ModoLectura = modolectura;

            if (true)//pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
            {
                recipe.BaseIva = (500).ToString("###,##0.00");
                recipe.BaseSinIva = (59).ToString("###,##0.00");
                recipe.Subtotal = (2.37).ToString("###,##0.00");
                recipe.Iva = (10.46).ToString("###,##0.00");
            }
            else
            {
                recipe.BaseIva = "";
                recipe.BaseSinIva = "";
                recipe.Subtotal = "";
                recipe.Iva = "";
            }

            recipe.CodigoRed = true ? "MEDIANET" : "DATAFAST";
            string tipodebcred = "";
            if (true)//trama.nomGruTar.Contains("DEBIT"))
            {
                tipodebcred = "<footer>     DEBITO</footer>\n";
                recipe.Pagare = " ";
            }
            else
            {
                if (false)//cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                    tipodebcred = "<footer>     ROTATIVO</footer>\n";
                else
                    tipodebcred = "<b>" + "foobar" + "</b>\n\n PLAZO MESES: " + "foobar" + (false ? "\n" + "MESES DE GRACIA:" + "foobar" : "");
            }
            if (true)//pp.TipoTransaccion != "03")
                recipe.TipoDebCredito = tipodebcred;

            if (false)//cmbBancoTarjeta.Text == "DIFERIDO CON INTERESES")
                recipe.Intereses = (500).ToString("###,##0.00");
            else
                recipe.Intereses = "";

            recipe.Arqc = "";
            recipe.Aidemv = "";
            recipe.Emv = "";
            recipe.Tc = "";
            recipe.Publicidad = "";

            Control.Common.Printer.ImprimirVoucherTarjetaCredito(Common.GlobalParameters.ComprobanteVoucherTarjetaCredito, recipe);

        }


        private void DevuelveformatoCreditoPavos(ref string texto)
        {
            texto = texto.Replace("<<NOM_TARJETA>>", "VISA" + "\nCOMERCIO: " + "MID" + "\nTID: " + "TID");
            texto = texto.Replace("<<NUM_TARJETA>>", "foo-bar");
            texto = texto.Replace("<<NUM_LOTE>>", "foo-bar");
            texto = texto.Replace("<<ADQUIRIENTE>>", "foo-bar");
            texto = texto.Replace("<<APROBACION>>", "foo-bar");
            texto = texto.Replace("<<SECUENCIAL>>", "foo-bar");
            texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", "foo-bar");
            texto = texto.Replace("<<FECHA_TRANS>>", "2018/04/01");
            texto = texto.Replace("<<HORA_TRANS>>", "22:36:02");
            texto = texto.Replace("<<VENC_TAR>>", "XX/XX");
            var valoranulacion = "";
            if (false) //pp.TipoTransaccion == "03")
            {
                //valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == "").FirstOrDefault().VALORCONSUMO;
                //valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' ');
            }
            decimal valorinteres = 0;



            texto = texto.Replace("<<VALOR_TOTAL>>", false ? valoranulacion : (123 + valorinteres).ToString("###,##0.00").PadLeft(13, ' '));
            string modolectura = "";
            switch ("01")//trama.modoLectura)
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
            }
            texto = texto.Replace("<<MODOLECTURA>>", modolectura);

            if (true)
            {


                texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA " + Common.GlobalParameters.IVAGEN.ToString()  + ": USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<IVA>>", "               IVA " + Common.GlobalParameters.IVAGEN.ToString() + "%: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));

                //texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA 14: USD$ " + decimal.Round((_factura.getBase12() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + decimal.Round((_factura.getBase0() * porc_pago), 2, MidpointRounding.ToEven).ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + decimal.Round((_factura.getSubTotal() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<IVA>>", "               IVA 14%: USD$ " + decimal.Round((_factura.getIVA() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
            }
            else
            {
                texto = texto.Replace("<<BASEIVA>>", "");
                texto = texto.Replace("<<BASESIVA>>", "");
                texto = texto.Replace("<<SUBTOTAL>>", "");
                texto = texto.Replace("<<IVA>>", "");

            }
            //texto = texto.Replace("<<BASEIVA>>", decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2) ).ToString());
            //texto = texto.Replace("<<BASESIVA>>", decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString() );
            //texto = texto.Replace("<<SUBTOTAL>>", (Decimal.Parse(decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)).ToString()) +decimal.Parse(decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString())).ToString());
            //texto = texto.Replace("<<IVA>>", decimal.Parse(pp.impuestoIvaTransaccion.Substring(1, 10) + "." + pp.impuestoIvaTransaccion.Substring(10, 2)).ToString());
            texto = texto.Replace("<<CODIGORED>>", true ? "MEDIANET" : "DATAFAST");


            string tipodebcred = "";
            if (false) //trama.nomGruTar.Contains("DEBIT"))
            {
                tipodebcred = "<footer>     DEBITO</footer>";

                texto = texto.Replace("<<PAGARE>>", " ");
            }
            else
            {
                //  texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO  QUE  EL  PRODUCTO  DE  ESTA \nTRANSACCION NO SERA UTILIZADO EN \nACTIVIDADES DE LAVADO DE DINERO \nY ACTIVO (LEY 108)");
                texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO QUE EL PRODUCTO DE ESTA TRANSACCION \nNO SERA UTILIZADO EN ACTIVIDADES DE LAVADO DE \nACTIVOS, FINANCIAMIENTO DEL TERRORISMO Y OTROS \nDELITOS ");

                tipodebcred = "<footer>     ROTATIVO</footer>";
            }
            if (true) //pp.TipoTransaccion != "03")
                texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred);
            // else
            //aqui es para anulaciones
            //texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred + "\nANULACION");
            //texto = texto.Replace("<<TRANSACCION>>", txtSecuencial.Text);

            texto = texto.Replace("<<INTERESES>>", "");
            //texto = texto.Replace("<<>>", trama);

            texto = texto.Replace("<<ARQC>>", "ARQC:      " + "foo-bar");
            texto = texto.Replace("<<AIDEMV>>", "AID - EMV: " + "foo-bar");
            texto = texto.Replace("<<EMV>>", "foo-bar");
            texto = texto.Replace("<<TC>>", "TC:        " + "foo-bar");
            texto = texto.Replace("<<PUBLICIDAD>>", "");
            /* texto = texto.Replace("", trama.nomGruTar);
             texto = texto.Replace("", trama.nomGruTar);
             texto = texto.Replace("", trama.nomGruTar);
             */
            //printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
            //printer.TextToPrint = texto;
            //printer.Print();
            //2
        }

        private void DevuelveformatoPagoPINPAD(ref string texto)
        {
            texto = texto.Replace("<<NOM_TARJETA>>", "foo-bar" + "\nCOMERCIO: " + "foo-bar" + "\nTID: " + "foo-bar");
            texto = texto.Replace("<<NUM_TARJETA>>", "foo-bar");
            texto = texto.Replace("<<NUM_LOTE>>", "foo-bar");
            texto = texto.Replace("<<ADQUIRIENTE>>", "foo-bar");
            texto = texto.Replace("<<APROBACION>>", "foo-bar");
            texto = texto.Replace("<<SECUENCIAL>>", "foo-bar");
            texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", "foo-bar");
            texto = texto.Replace("<<FECHA_TRANS>>", "2018/04/01");
            texto = texto.Replace("<<HORA_TRANS>>", "00:00:00");
            texto = texto.Replace("<<VENC_TAR>>", "XX/XX");
            var valoranulacion = "";
            if (false) //pp.TipoTransaccion == "03")
            {
                //valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion && x.NUMEROVOUCHER == pp.secuencialTransaccion)
                //                               .FirstOrDefault()
                //                               .VALORCONSUMO;
                //valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' ');
            }

            texto = texto.Replace("<<VALOR_TOTAL>>", ("50.00").PadLeft(13, ' '));
            string modolectura = "";
            switch ("03")//trama.modoLectura)
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
            }
            texto = texto.Replace("<<MODOLECTURA>>", modolectura);

            if (false)//pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
            {
                //texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA 12: USD$ " + _factura.getBase12().ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + _factura.getBase0().ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + _factura.getSubTotal().ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<IVA>>", "               IVA 12%: USD$ " + _factura.getIVA().ToString("###,##0.00").PadLeft(13, ' '));
            }
            else
            {
                texto = texto.Replace("<<BASEIVA>>", "");
                texto = texto.Replace("<<BASESIVA>>", "");
                texto = texto.Replace("<<SUBTOTAL>>", "");
                texto = texto.Replace("<<IVA>>", "");

            }
            //texto = texto.Replace("<<BASEIVA>>", decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2) ).ToString());
            //texto = texto.Replace("<<BASESIVA>>", decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString() );
            //texto = texto.Replace("<<SUBTOTAL>>", (Decimal.Parse(decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)).ToString()) +decimal.Parse(decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString())).ToString());
            //texto = texto.Replace("<<IVA>>", decimal.Parse(pp.impuestoIvaTransaccion.Substring(1, 10) + "." + pp.impuestoIvaTransaccion.Substring(10, 2)).ToString());
            texto = texto.Replace("<<CODIGORED>>", true ? "MEDIANET" : "DATAFAST");
            string tipodebcred = "";
            if (false)//trama.nomGruTar.Contains("DEBIT"))
            {
                tipodebcred = "<footer>     DEBITO</footer>";

                texto = texto.Replace("<<PAGARE>>", " ");
            }
            else
            {
                texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO  QUE  EL  PRODUCTO  DE  ESTA \nTRANSACCION NO SERA UTILIZADO EN \nACTIVIDADES DE LAVADO DE DINERO \nY ACTIVO (LEY 108)");

                if (false)//cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                    tipodebcred = "<footer>     ROTATIVO</footer>";
                else
                    tipodebcred = "<b>" + "foo-bar" + "</b>\n PLAZO MESES: " + "foo-bar" + "\n" + (true ? "MESES DE GRACIA:" + "03" : "");
            }
            if (true)//pp.TipoTransaccion != "03")
                texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred);
            else
                texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred + "\nANULACION");
            texto = texto.Replace("<<TRANSACCION>>", "foo-bar");

            if (true)//cmbBancoTarjeta.Text == "DIFERIDO CON INTERESES")

                texto = texto.Replace("<<INTERESES>>", "               INTERES: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
            else
                texto = texto.Replace("<<INTERESES>>", "");
            //texto = texto.Replace("<<>>", trama);

            texto = texto.Replace("<<ARQC>>", "ARQC:      " + "foo-bar");
            texto = texto.Replace("<<AIDEMV>>", "AID - EMV: " + "foo-bar");
            texto = texto.Replace("<<EMV>>", "foo-bar");
            texto = texto.Replace("<<TC>>", "TC:        " + "foo-bar");
            texto = texto.Replace("<<PUBLICIDAD>>", "foo-bar");
            /* texto = texto.Replace("", trama.nomGruTar);
             texto = texto.Replace("", trama.nomGruTar);
             texto = texto.Replace("", trama.nomGruTar);
             */
            //3
            //printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
            //printer.TextToPrint = texto;
            //printer.Print();
        }

        private void DevuelveformatoClsPagos(ref string texto)
        {
            texto = texto.Replace("<<NOM_TARJETA>>", "foo-bar" + "\nCOMERCIO: " + "foo-bar" + "\nTID: " + "foo-bar");
            texto = texto.Replace("<<NUM_TARJETA>>", "foo-bar");
            texto = texto.Replace("<<NUM_LOTE>>", "foo-bar");
            texto = texto.Replace("<<ADQUIRIENTE>>", "foo-bar");
            texto = texto.Replace("<<APROBACION>>", "foo-bar");
            texto = texto.Replace("<<SECUENCIAL>>", "foo-bar");
            texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", "foo-bar");
            texto = texto.Replace("<<FECHA_TRANS>>", "2018/04/01");
            texto = texto.Replace("<<HORA_TRANS>>", "00:00:00");
            texto = texto.Replace("<<VENC_TAR>>", "XX/XX");
            var valoranulacion = "";
            if (false)//posVoucher.TIPOTRANSACCION == "03")
            {
                //valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == posVoucher.ANULAUTORIZACION).FirstOrDefault().VALORCONSUMO;
                //valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' ');
            }
            decimal valorinteres = 0;



            texto = texto.Replace("<<VALOR_TOTAL>>", (500).ToString("###,##0.00").PadLeft(13, ' '));
            string modolectura = "";
            switch ("004")
            {
                case "001":
                    modolectura = "Manual";
                    break;
                case "002":
                    modolectura = "Banda";
                    break;
                case "003":
                    modolectura = "Chip";
                    break;
                case "004":
                    modolectura = "Fallback Manual (Chip)";
                    break;
                case "005":
                    modolectura = "Fallback Banda (Chip) ";
                    break;
            }
            texto = texto.Replace("<<MODOLECTURA>>", modolectura);

            if (true)//posVoucher.TIPOTRANSACCION != "03" || posVoucher.TIPOTRANSACCION != "04")
            {


                texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA " + Common.GlobalParameters.IVAGEN.ToString() + ": USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<IVA>>", "               IVA " + Common.GlobalParameters.IVAGEN.ToString() + "%: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));

            }
            else
            {
                texto = texto.Replace("<<BASEIVA>>", "");
                texto = texto.Replace("<<BASESIVA>>", "");
                texto = texto.Replace("<<SUBTOTAL>>", "");
                texto = texto.Replace("<<IVA>>", "");

            }
            texto = texto.Replace("<<CODIGORED>>", true ? "MEDIANET" : "DATAFAST");
            string tipodebcred = "";
            if (false)//posVoucher.GRUPOTAR.Contains("DEBIT"))
            {
                tipodebcred = "<footer>     DEBITO</footer>";

                texto = texto.Replace("<<PAGARE>>", " ");
            }
            else
            {
                //  texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO  QUE  EL  PRODUCTO  DE  ESTA \nTRANSACCION NO SERA UTILIZADO EN \nACTIVIDADES DE LAVADO DE DINERO \nY ACTIVO (LEY 108)");
                texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO QUE EL PRODUCTO DE ESTA TRANSACCION \nNO SERA UTILIZADO EN ACTIVIDADES DE LAVADO DE \nACTIVOS, FINANCIAMIENTO DEL TERRORISMO Y OTROS \nDELITOS ");

                if (true)//posVoucher.PLAZO == "" && posVoucher.MESESGRACIA == "")
                    tipodebcred = "<footer>     ROTATIVO</footer>";
                else
                    tipodebcred = "";// "<b>" + posVoucher.TIPOBANCOTARJETA + "</b>\n PLAZO MESES: " + posVoucher.PLAZO + "\n" + (posVoucher.MESESGRACIA != "" ? "MESES DE GRACIA:" + posVoucher.MESESGRACIA : "");
            }
            if (true)//posVoucher.TIPOTRANSACCION != "03")
                texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred);
            // else
            //aqui es para anulaciones
            //texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred + "\nANULACION");
            //texto = texto.Replace("<<TRANSACCION>>", txtSecuencial.Text);

            if (true)//posVoucher.TIPOBANCOTARJETA == "DIFERIDO CON INTERESES")

                texto = texto.Replace("<<INTERESES>>", "               INTERES: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
            else
                texto = texto.Replace("<<INTERESES>>", "");
            //texto = texto.Replace("<<>>", trama);

            texto = texto.Replace("<<ARQC>>", "ARQC:      " + "foo-bar");
            texto = texto.Replace("<<AIDEMV>>", "AID - EMV: " + "foo-bar");
            texto = texto.Replace("<<EMV>>", "foo-bar");
            texto = texto.Replace("<<TC>>", "TC:        " + "foo-bar");
            texto = texto.Replace("<<PUBLICIDAD>>", "foo-bar");
        }

        private void DevuelveformatoBasePagos(ref string texto)
        {
            texto = texto.Replace("<<NOM_TARJETA>>", "foo-bar" + "\nCOMERCIO: " + "foo-bar" + "\nTID: " + "foo-bar");
            texto = texto.Replace("<<NUM_TARJETA>>", "foo-bar");
            texto = texto.Replace("<<NUM_LOTE>>", "foo-bar");
            texto = texto.Replace("<<ADQUIRIENTE>>", "foo-bar");
            texto = texto.Replace("<<APROBACION>>", "foo-bar");
            texto = texto.Replace("<<SECUENCIAL>>", "foo-bar");
            texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", "foo-bar");
            texto = texto.Replace("<<FECHA_TRANS>>", "2018/04/01");
            texto = texto.Replace("<<HORA_TRANS>>", "00:00:00");
            texto = texto.Replace("<<VENC_TAR>>", "XX/XX");
            texto = texto.Replace("<<FACTURA>>", "foo-bar");
            var valoranulacion = "";
            if (false)//pp.TipoTransaccion == "03")
            {
                //valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion).FirstOrDefault().VALORCONSUMO;
                //valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' ');
            }
            decimal valorinteres = 10;



            texto = texto.Replace("<<VALOR_TOTAL>>", (500).ToString("###,##0.00").PadLeft(13, ' '));
            string modolectura = "";
            switch ("02")//trama.modoLectura)
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
            }
            texto = texto.Replace("<<MODOLECTURA>>", modolectura);

            if (true)//pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
            {


                texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA " + Common.GlobalParameters.IVAGEN.ToString() + ": USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
                texto = texto.Replace("<<IVA>>", "               IVA " + Common.GlobalParameters.IVAGEN.ToString() + "%: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));

                //texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA 14: USD$ " + decimal.Round((_factura.getBase12() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + decimal.Round((_factura.getBase0() * porc_pago), 2, MidpointRounding.ToEven).ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + decimal.Round((_factura.getSubTotal() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
                //texto = texto.Replace("<<IVA>>", "               IVA 14%: USD$ " + decimal.Round((_factura.getIVA() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
            }
            else
            {
                texto = texto.Replace("<<BASEIVA>>", "");
                texto = texto.Replace("<<BASESIVA>>", "");
                texto = texto.Replace("<<SUBTOTAL>>", "");
                texto = texto.Replace("<<IVA>>", "");

            }
            //texto = texto.Replace("<<BASEIVA>>", decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2) ).ToString());
            //texto = texto.Replace("<<BASESIVA>>", decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString() );
            //texto = texto.Replace("<<SUBTOTAL>>", (Decimal.Parse(decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)).ToString()) +decimal.Parse(decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString())).ToString());
            //texto = texto.Replace("<<IVA>>", decimal.Parse(pp.impuestoIvaTransaccion.Substring(1, 10) + "." + pp.impuestoIvaTransaccion.Substring(10, 2)).ToString());
            texto = texto.Replace("<<CODIGORED>>", false ? "MEDIANET" : "DATAFAST");
            string tipodebcred = "";
            if (false)//trama.nomGruTar.Contains("DEBIT"))
            {
                tipodebcred = "<footer>     DEBITO</footer>";

                texto = texto.Replace("<<PAGARE>>", " ");
            }
            else
            {
                //  texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO  QUE  EL  PRODUCTO  DE  ESTA \nTRANSACCION NO SERA UTILIZADO EN \nACTIVIDADES DE LAVADO DE DINERO \nY ACTIVO (LEY 108)");
                texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO QUE EL PRODUCTO DE ESTA TRANSACCION \nNO SERA UTILIZADO EN ACTIVIDADES DE LAVADO DE \nACTIVOS, FINANCIAMIENTO DEL TERRORISMO Y OTROS \nDELITOS ");

                if (false)//cmbDiferido.Text == "" && cmbMesesGracia.Text == "")
                    tipodebcred = "<footer>     ROTATIVO</footer>";
                else
                    tipodebcred = "<b>" + "foo-bar" + "</b>\n PLAZO MESES: " + "foo-bar" + "\n" + ("MESES DE GRACIA:" + "foo-bar");
            }
            if (true)//pp.TipoTransaccion != "03")
                texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred);
            // else
            //aqui es para anulaciones
            //texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred + "\nANULACION");
            //texto = texto.Replace("<<TRANSACCION>>", txtSecuencial.Text);

            if (true)//cmbBancoTarjeta.Text == "DIFERIDO CON INTERESES")

                texto = texto.Replace("<<INTERESES>>", "               INTERES: USD$ " + (500).ToString("###,##0.00").PadLeft(13, ' '));
            else
                texto = texto.Replace("<<INTERESES>>", "");
            //texto = texto.Replace("<<>>", trama);

            texto = texto.Replace("<<ARQC>>", "ARQC:      " + "foo-bar");
            texto = texto.Replace("<<AIDEMV>>", "AID - EMV: " + "foo-bar");
            texto = texto.Replace("<<EMV>>", "foo-bar");
            texto = texto.Replace("<<TC>>", "TC:        " + "foo-bar");
            texto = texto.Replace("<<PUBLICIDAD>>", "foo-bar");
        }

        private void ToolBoxMenu_Load(object sender, EventArgs e)
        {

            btnPantallaCliente.Enabled = false;
            if (Control.Common.GlobalParameters.PANTALLA_CLIENTE)
            {
                btnPantallaCliente.Enabled = true;
            }


            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "ToolBoxMenu_Load", "Pantalla ToolBox accesado. Usuario logon: " + (Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Common.GlobalParameters.UserObj.username));
        }

        private void btnCerrarPOS_Click(object sender, EventArgs e)
        {

            var result = Control.Common.General.GetMensajeToList(554);
            if (result == MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Ok || result == MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Yes)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "btnCerrarPOS_Click", "Procediendo a cerrar POS desde ToolBox bajo petición del usuario");
                Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }


            //if (MessageBox.Show("Procediendo a cerrar POS, pulse Sí para continuar", "Saliendo de POS", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "btnCerrarPOS_Click", "Procediendo a cerrar POS desde ToolBox bajo petición del usuario");
            //    Control.Common.GlobalParameters.MustCloseApplication = true;
            //    Application.Exit();
            //}
        }

        private void btnRecargarRedActiva_Click(object sender, EventArgs e)
        {
            var result = Control.Common.General.GetMensajeToList(653);
            if (result == MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Ok || result == MensajesLibrary.MsgBoxCtrl.MessageBoxResult.Yes)
            {
                Control.CorrBan.ClsCorrBan.CargarParametrosRedActiva();

                _mainWindow.btnCorresponsal.Visible = CorrBan.ClsCorrBan.EsCorresponsalActivo;

                if (CorrBan.ClsCorrBan.EsCorresponsalActivo)
                    // MessageBox.Show("Proceso finalizado. El local ya fue habilitado para Red Activa por lo que se habilitó la opción en la pantalla principal");
                    Control.Common.General.GetMensajeToList(170);

                else
                    //MessageBox.Show("Proceso finalizado. El local NO está habilitado para Red Activa por lo que no se habilitará la opción en la pantalla principal");
                    Control.Common.General.GetMensajeToList(171);


                this.Close();
            }



            //if (MessageBox.Show("Procediendo a actualizar los parámetros de Red Activa, pulse Sí para continuar", "Red Activa", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            //    Control.CorrBan.ClsCorrBan.CargarParametrosRedActiva();

            //    _mainWindow.btnCorresponsal.Visible = CorrBan.ClsCorrBan.EsCorresponsalActivo;

            //    if (CorrBan.ClsCorrBan.EsCorresponsalActivo)
            //        // MessageBox.Show("Proceso finalizado. El local ya fue habilitado para Red Activa por lo que se habilitó la opción en la pantalla principal");
            //        Control.Common.General.GetMensajeToList(170);

            //    else
            //        //MessageBox.Show("Proceso finalizado. El local NO está habilitado para Red Activa por lo que no se habilitará la opción en la pantalla principal");
            //        Control.Common.General.GetMensajeToList(171);


            //    this.Close();
            //}

        }

        private void btnEstresarPOS_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "btnEstresarPOS_Click", "Boton Estresar POS presionado");

            if (Control.Common.GlobalParameters.EsAmbienteProduccion)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "btnEstresarPOS_Click", "Acción denegada por encontrarse en ambiente producción");
                //Control.Common.WinForm.ShowMessage("Ud. se encuentra en ambiente de producción. Acción denegada");
                Control.Common.General.GetMensajeToList(173);

            }
            else
            {
                this.Close();
                _mainWindow.EstresarPOSBackGround();
            }
        }

        private void btnPistoleoMetricas_Click(object sender, EventArgs e)
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "btnPistoleoMetricas_Click", "Boton Metricas Pistoleo presionado");
            this.Close();
            //_mainWindow.EjecutarPistoleoMetricas();
        }

        private void btnProbarBalanzaDL_Click(object sender, EventArgs e)
        {
            try
            {
                 TomaPeso scanner;
                 OposScanner_CCO.OPOSScanner scannerDL;
                 OposScale_CCO.OPOSScale scannerDLW;

               // scanner = new TomaPeso(_factura.PuertoBalanza, TomaPeso.BalanzaMarcas.DATALOGIC, true, false, false);

                /*
                // Send: ESC [ 6 q CR
                _serialPort.Write(new byte[] { 0x1B, 0x5B, 0x36, 0x71, 0x0D }, 0, 5);

                // Send: ESC [ 3 q CR
                _serialPort.Write(new byte[] { 0x1B, 0x5B, 0x33, 0x71, 0x0D }, 0, 5);

                // Send: ESC [ 7 q CR
                _serialPort.Write(new byte[] { 0x1B, 0x5B, 0x37, 0x71, 0x0D }, 0, 5);

                */
            }
            catch (Exception ex )
            {
                //MessageBox.Show("Error : " + ex.Message,"Prueba de Balanza Data Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Control.Common.General.GetMensaje("POS", "Prueba de Balanza Data Logic", "ER");
                Control.Common.General.GetMensajeToList(604);


            }
        }

        private void radButton1_Click(object sender, EventArgs e)
        {

        }

        private void btnEjecutaTramaPinPad_Click(object sender, EventArgs e)
        {
            //ConfiguraPinPad
            Control.PINPAD.ConfiguraPinPad ConfiguraPinPad = new Control.PINPAD.ConfiguraPinPad(this, _mainWindow);
            ConfiguraPinPad.ShowDialog();

        }

        private void btnPantallaCliente_Click(object sender, EventArgs e)
        {
            
            if (Control.Common.GlobalParameters.PANTALLA_CLIENTE)
            {
                if (Control.Common.GlobalParameters.frmTouchClte == null) {
                    Control.ToolBox.ToolBoxMenu.MostrarPantallaClteTouch();
                }
                
            }

        }

        //
        //public static void MostrarPantallaClteTouch()
        //{

        //    try
        //    {
        //        //Control.Common.GlobalParameters.PANTALLA_CLIENTE = false;
        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "MostrarPantallaClteTouch", "Ejecuta metodo para ver pantalla de cliente");

        //        var pantallas = Screen.AllScreens;
        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "MostrarPantallaClteTouch", "Ejecuta metodo para ver pantalla de cliente");

        //        // Posicionar en otra pantalla si es posible
        //        if (Screen.AllScreens.Length > 1)
        //        {
        //            // Define las dimensiones que buscas
        //            int targetWidth = Control.Common.GlobalParameters.targetWidth;
        //            int targetHeight = Control.Common.GlobalParameters.targetHeight;

        //            if (Control.Common.GlobalParameters.PANTALLA_CLIENTE)
        //            {

        //                Rectangle areaToUse = Control.Common.General.GetRectangleClte();
        //                frmMainTouchClte frmTouchClte = new frmMainTouchClte(); //para pruebas JCHID
        //                //frmPromocionPantallaCliente frmTouchClte = new frmPromocionPantallaCliente();
        //                frmTouchClte.StartPosition = FormStartPosition.Manual;
        //                frmTouchClte.Location = new Point(areaToUse.Left, areaToUse.Top);
        //                frmTouchClte.Size = areaToUse.Size; // Ocupa toda la pantalla
        //                frmTouchClte.TopMost = true; // Forzar que esté encima de otros formularios
        //                frmTouchClte.Show();
        //                frmTouchClte.BringToFront();

        //                Control.Common.GlobalParameters.frmTouchClte = frmTouchClte; //comentado para pruebas JCHID
        //                //Control.Common.GlobalParameters.frmPromocionPantallaCliente = frmTouchClte;

        //                // Forzar la posición usando SetWindowPos (opcional)
        //                IntPtr handle = frmTouchClte.Handle;
        //                const uint SWP_SHOWWINDOW = 0x0040;
        //                SetWindowPos(handle, IntPtr.Zero, areaToUse.Left, areaToUse.Top, areaToUse.Width, areaToUse.Height, SWP_SHOWWINDOW);

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MostrarPantallaClteTouch", $"Error: {ex.Message}");
        //    }
            

        //}


        // correcion del metodo para mostrar la pantalla de publicidad 
        public static void MostrarPantallaClteTouch()
        {
            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "MostrarPantallaClteTouch", "Ejecuta metodo para ver pantalla de cliente");

                // Validar si hay más de una pantalla conectada
                if (Screen.AllScreens.Length > 1)
                {
                    // Verificamos si la funcionalidad está activa por configuración
                    if (Control.Common.GlobalParameters.PANTALLA_CLIENTE)
                    {
                        // 1. Obtener el rectángulo de la segunda pantalla (usando tu método que ya validamos que funciona)
                        Rectangle areaToUse = Control.Common.General.GetRectangleClte();

                        // 2. Instanciar el formulario
                        frmMainTouchClte frmTouchClte = new frmMainTouchClte();

                        // --------------------------------------------------------------------------
                        // INICIO DE LA CORRECCIÓN: "Salto de Pantalla"
                        // --------------------------------------------------------------------------

                        // PASO A: Resetear estado. Si nace Maximized, Windows ignora la ubicación.
                        frmTouchClte.WindowState = FormWindowState.Normal;

                        // PASO B: Decirle a Windows que nosotros controlamos la posición
                        frmTouchClte.StartPosition = FormStartPosition.Manual;

                        // PASO C: Asignar la posición y tamaño exactos de la segunda pantalla
                        frmTouchClte.Bounds = areaToUse;

                        // PASO D: Quitar bordes para estética de Pantalla Cliente
                        frmTouchClte.FormBorderStyle = FormBorderStyle.None;

                        // PASO E: Forzar que esté siempre visible
                        frmTouchClte.TopMost = true;

                        // PASO F: Mostrar la ventana (aparecerá en la pantalla 2, tamaño normal)
                        frmTouchClte.Show();

                        // PASO G: AHORA SÍ, Maximizar (se expandirá en el monitor donde ya está ubicada)
                        frmTouchClte.WindowState = FormWindowState.Maximized;

                        // --------------------------------------------------------------------------
                        // FIN DE LA CORRECCIÓN
                        // --------------------------------------------------------------------------

                        // Guardar la referencia en variables globales para poder cerrarla o actualizarla luego
                        Control.Common.GlobalParameters.frmTouchClte = frmTouchClte;

                        // Traer al frente por seguridad
                        frmTouchClte.BringToFront();
                    }
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.ToolBox.ToolBoxMenu", "MostrarPantallaClteTouch", "No se detectaron múltiples pantallas. Se omite apertura.");
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MostrarPantallaClteTouch", $"Error: {ex.Message}");
            }
        }
        // correcion del metodo para mostrar la pantalla de publicidad 

        public static Screen GetScreenByDeviceName(string targetDeviceName)
        {
            if (string.IsNullOrEmpty(targetDeviceName))
                return null;

            foreach (var screen in Screen.AllScreens)
            {
                if (screen.DeviceName.Equals(targetDeviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return screen;
                }
            }

            return null;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //private void btnPantallaCliente_Click(object sender, EventArgs e)
        //{
        //    Control.Common.General.IniciarPantallaCliente();
        //}
    }
}
