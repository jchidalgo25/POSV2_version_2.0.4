using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;

namespace POS.Control.Pagos
{
    public static class ClsPagos
    {
        private static DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();
        private static string _msj;

        public static string Msj
        {
            get { return _msj; }
        }

        public static Boolean ReimprimirVoucher(string tipo_voucher, POS_VOUCHER posVoucher)
        {
            try
            {
                var db = new POSEntities();

                var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();

                //Asignar valores por defecto
                posVoucher.ARQC = (posVoucher.ARQC != null) ? posVoucher.ARQC : "";
                posVoucher.AIDEMV = (posVoucher.AIDEMV != null) ? posVoucher.AIDEMV : "";
                posVoucher.EMV = (posVoucher.EMV != null) ? posVoucher.EMV : "";
                posVoucher.TC = (posVoucher.TC != null) ? posVoucher.TC : "";
                posVoucher.PUBLICIDAD = (posVoucher.PUBLICIDAD != null) ? posVoucher.PUBLICIDAD : "";
                posVoucher.TIPOTRANSACCION = (posVoucher.TIPOTRANSACCION != null) ? posVoucher.TIPOTRANSACCION : "";
                posVoucher.BANCOADQUIRIENTE = (posVoucher.BANCOADQUIRIENTE != null) ? posVoucher.BANCOADQUIRIENTE : "";
                posVoucher.TARJETAHABIENTE = (posVoucher.TARJETAHABIENTE != null) ? posVoucher.TARJETAHABIENTE : "";
                posVoucher.MID = (posVoucher.MID != null) ? posVoucher.MID : "";
                posVoucher.TID = (posVoucher.TID != null) ? posVoucher.TID : "";
                posVoucher.VENCTAR = (posVoucher.VENCTAR != null) ? posVoucher.VENCTAR : "";
                posVoucher.ANULAUTORIZACION = (posVoucher.ANULAUTORIZACION != null) ? posVoucher.ANULAUTORIZACION : "";
                posVoucher.TIPOBANCOTARJETA = (posVoucher.TIPOBANCOTARJETA != null) ? posVoucher.TIPOBANCOTARJETA : "";

                recipe.NomTarjeta = posVoucher.GRUPOTAR;
                recipe.MID = posVoucher.MID;
                recipe.TID = posVoucher.TID;
                recipe.NumTarjeta = posVoucher.TARJETA.Trim();
                recipe.NumLote = posVoucher.LOTE;
                recipe.Adquiriente = posVoucher.BANCOADQUIRIENTE;
                recipe.Aprobacion = posVoucher.AUTORIZACION;
                recipe.Secuencial = posVoucher.NUMEROVOUCHER;
                recipe.NombreTarjetaHabiente = posVoucher.TARJETAHABIENTE;
                recipe.FechaTrans = posVoucher.FECHACONSUMO.Substring(0, 4) + "/" + posVoucher.FECHACONSUMO.Substring(4, 2) + "/" + posVoucher.FECHACONSUMO.Substring(6, 2);
                recipe.HoraTrans = posVoucher.HORACONSUMO.Substring(0, 2) + ":" + posVoucher.HORACONSUMO.Substring(2, 2) + ":" + posVoucher.HORACONSUMO.Substring(4, 2);
                recipe.VenTarjeta = posVoucher.VENCTAR;
                var valoranulacion = "";
                if (posVoucher.TIPOTRANSACCION == "03")
                {
                    valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == posVoucher.ANULAUTORIZACION).FirstOrDefault().VALORCONSUMO;
                    valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00");
                }
                decimal valorinteres = posVoucher.VALORINTERES.Trim() == "" ? 0 : Decimal.Parse(posVoucher.VALORINTERES.Substring(0, 11) + "." + posVoucher.VALORINTERES.Substring(11, 2));
                
                recipe.ValorTotal = posVoucher.TIPOTRANSACCION == "03" ? valoranulacion : (Decimal.Parse(posVoucher.VALORCONSUMO.Substring(0, 11) + "." + posVoucher.VALORCONSUMO.Substring(11, 2)) + valorinteres).ToString("###,##0.00");
                string modolectura = "";
                switch (posVoucher.TIPOLECTURA)
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
                recipe.ModoLectura = modolectura;

                if (posVoucher.TIPOTRANSACCION != "03" || posVoucher.TIPOTRANSACCION != "04")
                {
                    recipe.BaseIva = Decimal.Parse(posVoucher.MONTOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTOGRAVAIVA.Substring(11, 2)).ToString("###,##0.00");
                    recipe.BaseSinIva = Decimal.Parse(posVoucher.MONTONOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTONOGRAVAIVA.Substring(11, 2)).ToString("###,##0.00");
                    recipe.Subtotal = (Decimal.Parse(posVoucher.MONTOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTOGRAVAIVA.Substring(11, 2)) + Decimal.Parse(posVoucher.MONTONOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTONOGRAVAIVA.Substring(11, 2))).ToString("###,##0.00");
                    recipe.Iva = Decimal.Parse(posVoucher.VALORIVA.Substring(0, 11) + "." + posVoucher.VALORIVA.Substring(11, 2)).ToString("###,##0.00");
                }
                else
                {
                    recipe.BaseIva = "";
                    recipe.BaseSinIva = "";
                    recipe.Subtotal = "";
                    recipe.Iva = "";

                }
                recipe.CodigoRed = posVoucher.AUTORIZADOR == 2 ? "MEDIANET" : "DATAFAST";
                string tipodebcred = "";
                if (posVoucher.GRUPOTAR.Contains("DEBIT"))
                {
                    tipodebcred = "<b>     DEBITO</b>\n";

                    recipe.Pagare = " ";
                }
                else
                {
                    if (posVoucher.PLAZO == "" && posVoucher.MESESGRACIA == "")
                        tipodebcred = "<b>     ROTATIVO</b>\n";
                    else
                        tipodebcred = "<b>" + posVoucher.TIPOBANCOTARJETA + "</b>\n\n PLAZO MESES: " + posVoucher.PLAZO + (posVoucher.MESESGRACIA != "" ? "\n" + "MESES DE GRACIA:" + posVoucher.MESESGRACIA : "");
                }

                if (posVoucher.TIPOTRANSACCION != "03")
                    recipe.TipoDebCredito = tipodebcred;

                if (posVoucher.TIPOBANCOTARJETA == "DIFERIDO CON INTERESES")
                    recipe.Intereses = Decimal.Parse(posVoucher.VALORINTERES.Substring(0, 11) + "." + posVoucher.VALORINTERES.Substring(11, 2)).ToString("###,##0.00");
                else
                    recipe.Intereses = "";

                recipe.Arqc = posVoucher.ARQC;
                recipe.Aidemv = posVoucher.AIDEMV;
                recipe.Emv = posVoucher.EMV;
                recipe.Tc = posVoucher.TC;
                recipe.Publicidad = posVoucher.PUBLICIDAD;

                Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe);

                /*
                var db = new POSEntities();

                //Asignar valores por defecto
                posVoucher.ARQC = (posVoucher.ARQC != null) ? posVoucher.ARQC : "";
                posVoucher.AIDEMV = (posVoucher.AIDEMV != null) ? posVoucher.AIDEMV : "";
                posVoucher.EMV = (posVoucher.EMV != null) ? posVoucher.EMV : "";
                posVoucher.TC = (posVoucher.TC != null) ? posVoucher.TC : "";
                posVoucher.PUBLICIDAD = (posVoucher.PUBLICIDAD != null) ? posVoucher.PUBLICIDAD : "";
                posVoucher.TIPOTRANSACCION = (posVoucher.TIPOTRANSACCION != null) ? posVoucher.TIPOTRANSACCION : "";
                posVoucher.BANCOADQUIRIENTE = (posVoucher.BANCOADQUIRIENTE != null) ? posVoucher.BANCOADQUIRIENTE : "";
                posVoucher.TARJETAHABIENTE = (posVoucher.TARJETAHABIENTE != null) ? posVoucher.TARJETAHABIENTE : "";
                posVoucher.MID = (posVoucher.MID != null) ? posVoucher.MID : "";
                posVoucher.TID = (posVoucher.TID != null) ? posVoucher.TID : "";
                posVoucher.VENCTAR = (posVoucher.VENCTAR != null) ? posVoucher.VENCTAR : "";
                posVoucher.ANULAUTORIZACION = (posVoucher.ANULAUTORIZACION != null) ? posVoucher.ANULAUTORIZACION : "";
                posVoucher.TIPOBANCOTARJETA = (posVoucher.TIPOBANCOTARJETA != null) ? posVoucher.TIPOBANCOTARJETA : "";

                if (db.core_recibo.Any(x => x.identificador == tipo_voucher))
                {
                    var voucher = db.core_recibo.First(x => x.identificador == tipo_voucher);
                    string texto = voucher.cuerpo;

                    texto = texto.Replace("<<NOM_TARJETA>>", posVoucher.GRUPOTAR + "\nCOMERCIO: " + posVoucher.MID + "\nTID: " + posVoucher.TID);
                    texto = texto.Replace("<<NUM_TARJETA>>", posVoucher.TARJETA.Trim());
                    texto = texto.Replace("<<NUM_LOTE>>", posVoucher.LOTE);
                    texto = texto.Replace("<<ADQUIRIENTE>>", posVoucher.BANCOADQUIRIENTE);
                    texto = texto.Replace("<<APROBACION>>", posVoucher.AUTORIZACION);
                    texto = texto.Replace("<<SECUENCIAL>>", posVoucher.NUMEROVOUCHER);
                    texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", posVoucher.TARJETAHABIENTE);
                    texto = texto.Replace("<<FECHA_TRANS>>", posVoucher.FECHACONSUMO.Substring(0, 4) + "/" + posVoucher.FECHACONSUMO.Substring(4, 2) + "/" + posVoucher.FECHACONSUMO.Substring(6, 2));
                    texto = texto.Replace("<<HORA_TRANS>>", posVoucher.HORACONSUMO.Substring(0, 2) + ":" + posVoucher.HORACONSUMO.Substring(2, 2) + ":" + posVoucher.HORACONSUMO.Substring(4, 2));
                    texto = texto.Replace("<<VENC_TAR>>", posVoucher.VENCTAR);
                    var valoranulacion = "";
                    if (posVoucher.TIPOTRANSACCION == "03")
                    {
                        valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == posVoucher.ANULAUTORIZACION).FirstOrDefault().VALORCONSUMO;
                        valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' ');
                    }
                    decimal valorinteres = posVoucher.VALORINTERES.Trim() == "" ? 0 : Decimal.Parse(posVoucher.VALORINTERES.Substring(0, 11) + "." + posVoucher.VALORINTERES.Substring(11, 2));



                    texto = texto.Replace("<<VALOR_TOTAL>>", posVoucher.TIPOTRANSACCION == "03" ? valoranulacion : (Decimal.Parse(posVoucher.VALORCONSUMO.Substring(0, 11) + "." + posVoucher.VALORCONSUMO.Substring(11, 2)) + valorinteres).ToString("###,##0.00").PadLeft(13, ' '));
                    string modolectura = "";
                    switch (posVoucher.TIPOLECTURA)
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

                    if (posVoucher.TIPOTRANSACCION != "03" || posVoucher.TIPOTRANSACCION != "04")
                    {


                        texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA 12: USD$ " + Decimal.Parse(posVoucher.MONTOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTOGRAVAIVA.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' '));
                        texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + Decimal.Parse(posVoucher.MONTONOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTONOGRAVAIVA.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' '));
                        texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + (Decimal.Parse(posVoucher.MONTOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTOGRAVAIVA.Substring(11, 2)) + Decimal.Parse(posVoucher.MONTONOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTONOGRAVAIVA.Substring(11, 2))).ToString("###,##0.00").PadLeft(13, ' '));
                        texto = texto.Replace("<<IVA>>", "               IVA 12%: USD$ " + Decimal.Parse(posVoucher.VALORIVA.Substring(0, 11) + "." + posVoucher.VALORIVA.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' '));

                    }
                    else
                    {
                        texto = texto.Replace("<<BASEIVA>>", "");
                        texto = texto.Replace("<<BASESIVA>>", "");
                        texto = texto.Replace("<<SUBTOTAL>>", "");
                        texto = texto.Replace("<<IVA>>", "");

                    }
                    texto = texto.Replace("<<CODIGORED>>", posVoucher.AUTORIZADOR == 2 ? "MEDIANET" : "DATAFAST");
                    string tipodebcred = "";
                    if (posVoucher.GRUPOTAR.Contains("DEBIT"))
                    {
                        tipodebcred = "<footer>     DEBITO</footer>";

                        texto = texto.Replace("<<PAGARE>>", " ");
                    }
                    else
                    {
                        //  texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO  QUE  EL  PRODUCTO  DE  ESTA \nTRANSACCION NO SERA UTILIZADO EN \nACTIVIDADES DE LAVADO DE DINERO \nY ACTIVO (LEY 108)");
                        texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO QUE EL PRODUCTO DE ESTA TRANSACCION \nNO SERA UTILIZADO EN ACTIVIDADES DE LAVADO DE \nACTIVOS, FINANCIAMIENTO DEL TERRORISMO Y OTROS \nDELITOS ");

                        if (posVoucher.PLAZO == "" && posVoucher.MESESGRACIA == "")
                            tipodebcred = "<footer>     ROTATIVO</footer>";
                        else
                            tipodebcred = "<b>" + posVoucher.TIPOBANCOTARJETA + "</b>\n PLAZO MESES: " + posVoucher.PLAZO + "\n" + (posVoucher.MESESGRACIA != "" ? "MESES DE GRACIA:" + posVoucher.MESESGRACIA : "");
                    }
                    if (posVoucher.TIPOTRANSACCION != "03")
                        texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred);
                    // else
                    //aqui es para anulaciones
                    //texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred + "\nANULACION");
                    //texto = texto.Replace("<<TRANSACCION>>", txtSecuencial.Text);

                    if (posVoucher.TIPOBANCOTARJETA == "DIFERIDO CON INTERESES")

                        texto = texto.Replace("<<INTERESES>>", "               INTERES: USD$ " + Decimal.Parse(posVoucher.VALORINTERES.Substring(0, 11) + "." + posVoucher.VALORINTERES.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' '));
                    else
                        texto = texto.Replace("<<INTERESES>>", "");
                    //texto = texto.Replace("<<>>", trama);

                    texto = texto.Replace("<<ARQC>>", "ARQC:      " + posVoucher.ARQC);
                    texto = texto.Replace("<<AIDEMV>>", "AID - EMV: " + posVoucher.AIDEMV);
                    texto = texto.Replace("<<EMV>>", posVoucher.EMV);
                    texto = texto.Replace("<<TC>>", "TC:        " + posVoucher.TC);
                    texto = texto.Replace("<<PUBLICIDAD>>", posVoucher.PUBLICIDAD);
                     //texto = texto.Replace("", trama.nomGruTar);
                     //texto = texto.Replace("", trama.nomGruTar);
                     //texto = texto.Replace("", trama.nomGruTar);
                     
                    printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, System.Drawing.FontStyle.Bold);
                    printer.TextToPrint = texto;
                    printer.Print();
                    //1
                    //Control.Common.Printer.Imprimir(texto, 3, 10);

                }
                */

                return true;

            }
            catch (Exception ex)
            {
                _msj = ex.Message + " StackTrace: " + ex.StackTrace;
                return false;
            }
        }

        public static Boolean ReimprimirVoucher(string tipo_voucher, POS_VOUCHER posVoucher, bool ImprimeCopia)
        {
            try
            {
                var db = new POSEntities();

                var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();

                //Asignar valores por defecto
                posVoucher.ARQC = (posVoucher.ARQC != null) ? posVoucher.ARQC : "";
                posVoucher.AIDEMV = (posVoucher.AIDEMV != null) ? posVoucher.AIDEMV : "";
                posVoucher.EMV = (posVoucher.EMV != null) ? posVoucher.EMV : "";
                posVoucher.TC = (posVoucher.TC != null) ? posVoucher.TC : "";
                posVoucher.PUBLICIDAD = (posVoucher.PUBLICIDAD != null) ? posVoucher.PUBLICIDAD : "";
                posVoucher.TIPOTRANSACCION = (posVoucher.TIPOTRANSACCION != null) ? posVoucher.TIPOTRANSACCION : "";
                posVoucher.BANCOADQUIRIENTE = (posVoucher.BANCOADQUIRIENTE != null) ? posVoucher.BANCOADQUIRIENTE : "";
                posVoucher.TARJETAHABIENTE = (posVoucher.TARJETAHABIENTE != null) ? posVoucher.TARJETAHABIENTE : "";
                posVoucher.MID = (posVoucher.MID != null) ? posVoucher.MID : "";
                posVoucher.TID = (posVoucher.TID != null) ? posVoucher.TID : "";
                posVoucher.VENCTAR = (posVoucher.VENCTAR != null) ? posVoucher.VENCTAR : "";
                posVoucher.ANULAUTORIZACION = (posVoucher.ANULAUTORIZACION != null) ? posVoucher.ANULAUTORIZACION : "";
                posVoucher.TIPOBANCOTARJETA = (posVoucher.TIPOBANCOTARJETA != null) ? posVoucher.TIPOBANCOTARJETA : "";

                recipe.NomTarjeta = posVoucher.GRUPOTAR;
                recipe.MID = posVoucher.MID;
                recipe.TID = posVoucher.TID;
                recipe.NumTarjeta = posVoucher.TARJETA.Trim();
                recipe.NumLote = posVoucher.LOTE;
                recipe.Adquiriente = posVoucher.BANCOADQUIRIENTE;
                recipe.Aprobacion = posVoucher.AUTORIZACION;
                recipe.Secuencial = posVoucher.NUMEROVOUCHER;
                recipe.NombreTarjetaHabiente = posVoucher.TARJETAHABIENTE;
                recipe.FechaTrans = posVoucher.FECHACONSUMO.Substring(0, 4) + "/" + posVoucher.FECHACONSUMO.Substring(4, 2) + "/" + posVoucher.FECHACONSUMO.Substring(6, 2);
                recipe.HoraTrans = posVoucher.HORACONSUMO.Substring(0, 2) + ":" + posVoucher.HORACONSUMO.Substring(2, 2) + ":" + posVoucher.HORACONSUMO.Substring(4, 2);
                recipe.VenTarjeta = posVoucher.VENCTAR;
                var valoranulacion = "";
                if (posVoucher.TIPOTRANSACCION == "03")
                {
                    valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == posVoucher.ANULAUTORIZACION).FirstOrDefault().VALORCONSUMO;
                    valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00");
                }
                decimal valorinteres = posVoucher.VALORINTERES.Trim() == "" ? 0 : Decimal.Parse(posVoucher.VALORINTERES.Substring(0, 11) + "." + posVoucher.VALORINTERES.Substring(11, 2));

                recipe.ValorTotal = posVoucher.TIPOTRANSACCION == "03" ? valoranulacion : (Decimal.Parse(posVoucher.VALORCONSUMO.Substring(0, 11) + "." + posVoucher.VALORCONSUMO.Substring(11, 2)) + valorinteres).ToString("###,##0.00");
                string modolectura = "";
                switch (posVoucher.TIPOLECTURA)
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
                recipe.ModoLectura = modolectura;

                if (posVoucher.TIPOTRANSACCION != "03" || posVoucher.TIPOTRANSACCION != "04")
                {
                    recipe.BaseIva = Decimal.Parse(posVoucher.MONTOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTOGRAVAIVA.Substring(11, 2)).ToString("###,##0.00");
                    recipe.BaseSinIva = Decimal.Parse(posVoucher.MONTONOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTONOGRAVAIVA.Substring(11, 2)).ToString("###,##0.00");
                    recipe.Subtotal = (Decimal.Parse(posVoucher.MONTOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTOGRAVAIVA.Substring(11, 2)) + Decimal.Parse(posVoucher.MONTONOGRAVAIVA.Substring(0, 11) + "." + posVoucher.MONTONOGRAVAIVA.Substring(11, 2))).ToString("###,##0.00");
                    recipe.Iva = Decimal.Parse(posVoucher.VALORIVA.Substring(0, 11) + "." + posVoucher.VALORIVA.Substring(11, 2)).ToString("###,##0.00");
                }
                else
                {
                    recipe.BaseIva = "";
                    recipe.BaseSinIva = "";
                    recipe.Subtotal = "";
                    recipe.Iva = "";

                }
                recipe.CodigoRed = posVoucher.AUTORIZADOR == 2 ? "MEDIANET" : "DATAFAST";
                string tipodebcred = "";
                if (posVoucher.GRUPOTAR.Contains("DEBIT"))
                {
                    tipodebcred = "<b>     DEBITO</b>\n";

                    recipe.Pagare = " ";
                }
                else
                {
                    if (posVoucher.PLAZO == "" && posVoucher.MESESGRACIA == "")
                        tipodebcred = "<b>     ROTATIVO</b>\n";
                    else
                        tipodebcred = "<b>" + posVoucher.TIPOBANCOTARJETA + "</b>\n\n PLAZO MESES: " + posVoucher.PLAZO + (posVoucher.MESESGRACIA != "" ? "\n" + "MESES DE GRACIA:" + posVoucher.MESESGRACIA : "");
                }

                if (posVoucher.TIPOTRANSACCION != "03")
                    recipe.TipoDebCredito = tipodebcred;

                if (posVoucher.TIPOBANCOTARJETA == "DIFERIDO CON INTERESES")
                    recipe.Intereses = Decimal.Parse(posVoucher.VALORINTERES.Substring(0, 11) + "." + posVoucher.VALORINTERES.Substring(11, 2)).ToString("###,##0.00");
                else
                    recipe.Intereses = "";

                recipe.Arqc = posVoucher.ARQC;
                recipe.Aidemv = posVoucher.AIDEMV;
                recipe.Emv = posVoucher.EMV;
                recipe.Tc = posVoucher.TC;
                recipe.Publicidad = posVoucher.PUBLICIDAD;

                Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe);
                if (ImprimeCopia) { Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe, ImprimeCopia); }
              

                return true;

            }
            catch (Exception ex)
            {
                _msj = ex.Message + " StackTrace: " + ex.StackTrace;
                return false;
            }
        }
    }
}
