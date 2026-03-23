
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using ZXing;
using ZXing.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing.Imaging;
using System.IO.Ports;

namespace POS.Control.Common
{
    public static class Printer
    {
        private static int _barcodeWidth = 300;
        private static int _barcodeHeight = 100;

        public static int BarcodeWidth
        {
            get { return _barcodeWidth; }
            set { _barcodeWidth = value; }
        }

        public static int BarcodeHeight
        {
            get { return _barcodeHeight; }
            set { _barcodeHeight = value; }
        }

        public static string DetectarPuertoImpresora()
        {
            // Obtener todos los puertos COM disponibles
            string[] puertosDisponibles = SerialPort.GetPortNames();

            if (puertosDisponibles.Length > 0)
            {
                // Suponemos que el primer puerto es el correcto
                return puertosDisponibles[0];
            }
            else
            {
                throw new Exception("No se encontraron puertos COM disponibles.");
            }
        }

        public static void OpenCashDrawer()
        {
            try
            {
                string puertoImpresora = DetectarPuertoImpresora();
                

                // ESC/POS command to open the drawer
                byte[] openDrawerCommand = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA };

                using (SerialPort port = new SerialPort(puertoImpresora))
                {
                    port.Open();
                    port.Write(openDrawerCommand, 0, openDrawerCommand.Length);
                    port.Close();
                    Console.WriteLine("Cash Drawer Opened Successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening cash drawer: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "OpenCashDrawer", "Error opening cash drawer: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "OpenCashDrawer", "Error opening cash drawer: " + ex.InnerException.Message);

            }
        }

       

        public static void OpenCashDrawer_COM(string printerPort)
        {
            try
            {
                // ESC/POS command to open the drawer
                byte[] openDrawerCommand = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA };

                using (SerialPort port = new SerialPort(printerPort))
                {
                    port.Open();
                    port.Write(openDrawerCommand, 0, openDrawerCommand.Length);
                    port.Close();
                    Console.WriteLine("Cash Drawer Opened Successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening cash drawer: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "OpenCashDrawer", "Error opening cash drawer: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "OpenCashDrawer", "Error opening cash drawer: " + ex.InnerException.Message);

            }
        }
        public static void OpenCashDrawer_PrinterName(string printerName)
        {
            try
            {
                byte[] openDrawerCommand = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA }; // Comando ESC/POS para abrir cajón
                string command = Encoding.Default.GetString(openDrawerCommand);
                RawPrinterHelper.SendStringToPrinter(printerName, command);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error abriendo cajón: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "OpenCashDrawer", "Error opening cash drawer: " + ex.Message);
            }
        }

        //public static void Imprimir(string texto, int tipo = 1, int ptosInterlineado = 15)
        //{

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(texto?.Trim('\r', '\n')))
        //        {
        //            Control.Common.Logger.LogMessage(
        //                Control.Common.Enum.LogTypes.Info,
        //                "Printer", "Imprimir",
        //                "Se omitió impresión porque el texto está vacío");
        //            return;
        //        }

        //        using (DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint())
        //        {
        //            Console.Write(printer.PrinterSettings);

        //            //printer.PrinterSettings.PrinterName = @"\\192.168.131.193\pos_demo";
        //            printer.YLineSpacing = ptosInterlineado;
        //            switch (tipo)
        //            {
        //                case 1:
        //                    printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
        //                    break;
        //                case 2:
        //                    printer.PrinterFont = new System.Drawing.Font("VERDANA", 8, FontStyle.Bold);
        //                    break;
        //                case 3:
        //                    printer.PrinterFont = new System.Drawing.Font("VERDANA", 6, FontStyle.Bold);
        //                    break;
        //                case 4:
        //                    printer.PrinterFont = new System.Drawing.Font("VERDANA", 5, FontStyle.Bold);
        //                    break;
        //                case 5:
        //                    if (texto.Contains("<barcode>") && texto.Contains("</barcode>"))
        //                    {
        //                        string codigo = texto.Substring(
        //                            texto.IndexOf("<barcode>") + 9,
        //                            texto.IndexOf("</barcode>") - texto.IndexOf("<barcode>") - 9
        //                        );

        //                        // Eliminar las etiquetas y reemplazar por el valor directamente
        //                        texto = texto.Replace("<barcode>", "")
        //                                    .Replace("</barcode>", "")
        //                                    .Replace(codigo, codigo.Trim());
        //                    }
        //                    break;

        //                default:
        //                    printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
        //                    break;
        //            }


        //            printer.TextToPrint = texto;
        //            printer.Print();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "Imprimir", "error: " + ex.Message);
        //    }

        //}


        //NUEVO METODO PARA IMPRIMIR EN EL POS NUEVO JCHID

        public static void Imprimir(string texto, int tipo = 1, int ptosInterlineado = 15)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(texto?.Trim('\r', '\n')))
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "Printer", "Imprimir",
                        "Se omitió impresión porque el texto está vacío");
                    return;
                }

                using (DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint())
                {
                    // --------------------------------------------------------------------------
                    // INICIO CORRECCIÓN: Error "Value '0' is not valid for Duplex"
                    // --------------------------------------------------------------------------
                    try
                    {
                        // Forzamos a Simplex (impresión simple) para sobrescribir el valor 0 inválido del driver
                        printer.PrinterSettings.Duplex = System.Drawing.Printing.Duplex.Simplex;
                    }
                    catch
                    {
                        // Si falla, continuamos (el driver podría ignorarlo, pero evitamos que el programa se detenga)
                    }
                    // --------------------------------------------------------------------------
                    // FIN CORRECCIÓN
                    // --------------------------------------------------------------------------

                    // IMPORTANTE: Esta línea se comenta porque al leer los settings para imprimirlos en consola,
                    // puede detonar el error antes de imprimir.
                    // Console.Write(printer.PrinterSettings);

                    printer.YLineSpacing = ptosInterlineado;

                    switch (tipo)
                    {
                        case 1:
                            printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                            break;
                        case 2:
                            printer.PrinterFont = new System.Drawing.Font("VERDANA", 8, FontStyle.Bold);
                            break;
                        case 3:
                            printer.PrinterFont = new System.Drawing.Font("VERDANA", 6, FontStyle.Bold);
                            break;
                        case 4:
                            printer.PrinterFont = new System.Drawing.Font("VERDANA", 5, FontStyle.Bold);
                            break;
                        case 5:
                            if (texto.Contains("<barcode>") && texto.Contains("</barcode>"))
                            {
                                string codigo = texto.Substring(
                                    texto.IndexOf("<barcode>") + 9,
                                    texto.IndexOf("</barcode>") - texto.IndexOf("<barcode>") - 9
                                );

                                // Eliminar las etiquetas y reemplazar por el valor directamente
                                texto = texto.Replace("<barcode>", "")
                                                .Replace("</barcode>", "")
                                                .Replace(codigo, codigo.Trim());
                            }
                            break;

                        default:
                            printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                            break;
                    }

                    printer.TextToPrint = texto;
                    printer.Print();
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "Imprimir", "error: " + ex.Message);
            }
        }
        //NUEVO METODO PARA IMPRIMIR EN EL POS NUEVO JCHID






        public static void ImprimirVoucherTarjetaCredito(string tipoVoucher, Models.PrinterRecipes.VoucherTarjetaCredito recipe, bool esCopia = false)
        {
            //string 
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var objRecibo = db.core_recibo.Where(x => x.identificador == tipoVoucher).FirstOrDefault();
                    var texto = objRecibo.cuerpo;

                    texto = texto.Replace("<<NOM_TARJETA>>", recipe.NomTarjeta.Trim() + "\n\nCOMERCIO         " + Char.Parse("\u200A") + Char.Parse("\u200A") + Char.Parse("\u200A") + ": " + recipe.MID.Trim() + "\nTID                     " + Char.Parse("\u200A") + Char.Parse("\u200A") + ": " + recipe.TID.Trim());
                    texto = texto.Replace("<<NUM_TARJETA>>", recipe.NumTarjeta.Trim());
                    texto = texto.Replace("V:<<VENC_TAR>>", Common.StringHelper.DevolverConPadding("V:" + recipe.VenTarjeta.Trim(), 25, 7));
                    texto = texto.Replace("<<NUM_LOTE>>", recipe.NumLote.Trim());
                    texto = texto.Replace("<<ADQUIRIENTE>>", recipe.Adquiriente.Trim());
                    texto = texto.Replace("<<FECHA_TRANS>>", recipe.FechaTrans.Trim());
                    texto = texto.Replace("HORA: <<HORA_TRANS>>", Common.StringHelper.DevolverConPadding("HORA: " + recipe.HoraTrans.Trim(), 33, 16));
                    texto = texto.Replace("<<APROBACION>>", recipe.Aprobacion.Trim());
                    texto = texto.Replace("<<SECUENCIAL>>", recipe.Secuencial.Trim());
                    texto = texto.Replace("<<MODOLECTURA>>", recipe.ModoLectura.Trim());
                    texto = texto.Replace("<<TIPODEBCRED>>", recipe.TipoDebCredito);
                    texto = texto.Replace("<<TRANSACCION>>", recipe.Transaccion.Trim());
                    texto = texto.Replace("<<FACTURA>>", recipe.Factura.Trim());
                    texto = texto.Replace("<<BASEIVA>>", recipe.BaseIva.Trim().Length > 0 ? Common.StringHelper.DevolverConPadding("BASE CONSUMO TARIFA " + Common.GlobalParameters.IVAGEN.ToString() + ": USD$", 30, 28) + Common.StringHelper.DevolverConPadding(recipe.BaseIva.Trim(), 33, 8) : "");
                    texto = texto.Replace("<<BASESIVA>>", recipe.BaseSinIva.Trim().Length > 0 ? Common.StringHelper.DevolverConPadding("BASE CONSUMO TARIFA 0: USD$", 30, 29) + Common.StringHelper.DevolverConPadding(recipe.BaseSinIva.Trim(), 33, 8) : "");
                    texto = texto.Replace("<<SUBTOTAL>>", recipe.Subtotal.Trim().Length > 0 ? Common.StringHelper.DevolverConPadding("SUBTOTAL CONSUMOS: USD$", 30, 27) + Common.StringHelper.DevolverConPadding(recipe.Subtotal.Trim(), 33, 8) : "");
                    texto = texto.Replace("<<IVA>>", recipe.Iva.Trim().Length > 0 ? Common.StringHelper.DevolverConPadding("IVA " + Common.GlobalParameters.IVAGEN.ToString() + "%: USD$", 30, 28) + Common.StringHelper.DevolverConPadding(recipe.Iva.Trim(), 33, 8) : "");
                    texto = texto.Replace("<<INTERESES>>", recipe.Intereses.Trim().Length > 0 ? Common.StringHelper.DevolverConPadding("INTERES: USD$", 30, 29) + Common.StringHelper.DevolverConPadding(recipe.Intereses.Trim(), 33, 8) : "");
                    texto = texto.Replace("<<VALOR_TOTAL>>", Common.StringHelper.DevolverConPadding(recipe.ValorTotal.Trim(), 22, 8));
                    texto = texto.Replace("<<CODIGORED>>", recipe.CodigoRed.Trim());
                    texto = texto.Replace("<<CODIGORED2>>", Control.Common.GlobalParameters.EstTcpIpPinpad ? "MEDIANET" : recipe.CodigoRed);
                    texto = texto.Replace("<<PAGARE>>", recipe.Pagare);
                    texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", recipe.NombreTarjetaHabiente.Trim());
                    texto = texto.Replace("<<COMERCIO2>>", recipe.MID2);
                    texto = texto.Replace("<<EMV>>", recipe.Emv);
                    texto = texto.Replace("<<ARQC>>", "ARQC                 : " + recipe.Arqc.Trim());
                    texto = texto.Replace("<<AIDEMV>>", "AID - EMV          : " + recipe.Aidemv.Trim());
                    texto = texto.Replace("<<TC>>", "TC                    " + Char.Parse("\u2000") + ": " + recipe.Tc.Trim());
                    texto = texto.Replace("<<PUBLICIDAD>>", recipe.Publicidad.Trim().Length > 0 ? Environment.NewLine + recipe.Publicidad.Trim() : "");
                    texto = texto.Replace("<<TVR>>", "TVR                    : " + recipe.TVR);
                    texto = texto.Replace("<<TSI>>", "TSI                     : " + recipe.TSI);
                    texto = texto.Replace("<<CIUDAD>>", Control.Common.GlobalParameters.CiudadLocal);

                    if (esCopia) {
                        texto = string.Concat(texto, "\n <<--------- COPIA CLIENTE --------- >>");
                    }
                    Control.Common.Printer.Imprimir(texto, 3, 9);
                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirVoucherTarjetaCredito", "error: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirVoucherTarjetaCredito", "InnerException.Message: " + ex.InnerException.Message);
            }


        }

        public static void ImprimirReciboVentaGiftcard(string texto, Models.PrinterRecipes.ReceiptGiftcardSale receipt)
        {

            try
            {
                texto = texto.Replace("<<oficina>>", receipt.Oficina);
                texto = texto.Replace("<<telefono>>", receipt.Telefono);
                texto = texto.Replace("<<factura>>", receipt.NroComprobante);

                texto = texto.Replace("<<cajero>>", receipt.CajeroNombre);
                texto = texto.Replace("<<factura_fecha>>", receipt.FechaTransaccion.ToString("dd/MM/yyyy HH:mm:ss"));
                texto = texto.Replace("<<cedula>>", receipt.ClienteIdentificacion);
                texto = texto.Replace("<<cliente>>", receipt.ClienteNombre);
                texto = texto.Replace("<<direccion>>", receipt.ClienteDireccion);
                texto = texto.Replace("<<cliente_telefono>>", receipt.ClienteTelefono);
                texto = texto.Replace("<<facConcepto>>", receipt.ConceptoTransaccion);
                //Enmascarar tarjeta
                //if (receipt.CodigoTarjeta.Length > 6)
                //{
                //    string first = receipt.CodigoTarjeta.Substring(0, 3);
                //    string last = receipt.CodigoTarjeta.Substring(receipt.CodigoTarjeta.Length - 3, 3);
                //    receipt.CodigoTarjeta = first + string.Empty.PadRight(receipt.CodigoTarjeta.Length - 6, 'X') + last;
                //    receipt.CodigoTarjeta = first + string.Empty.PadRight(receipt.CodigoTarjeta.Length - 6, 'X') + last;  // jchid comentado para que no salga el codigo
                //}

                //Muchas GC

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaItemGC>(.*)\</plantillaItemGC>");
                StringBuilder items = new StringBuilder();
                foreach (var gc in receipt.LstGiftcard)
                {
                    string codigo_X = gc.codigo;


                    var charArr = codigo_X.Trim().ToArray();
                    var tarj_mask = "";
                    for (int i = 0; i < charArr.Length; i++)
                    {
                        tarj_mask += (i < 7 || charArr.Length - 6 <= i) ? charArr[i].ToString() : "X";
                    }
                    codigo_X = tarj_mask;


                    string descripcion = gc.tipoTransaccion + " GC " + codigo_X;

                    string monto = ((decimal)gc.monto).ToString("N2");
                    items.AppendLine(descripcion + Convert.ToChar(9) + Convert.ToChar(9) + Control.Common.StringHelper.DevolverConPadding(monto, 7));
                    // items.AppendLine(Control.Common.StringHelper.DevolverConPadding(gc.monto.ToString("N2"),1));
                }
                texto = regex.Replace(texto, items.ToString());

                //texto = texto.Replace("<<itemDescripcion>>", strDetalleGC.ToString().Trim());
                //texto = texto.Replace("<<itemValor>>", strDetalleGCValor.ToString().Trim());
                // texto = texto.Replace("<<itemValor>>", Common.StringHelper.DevolverConPadding(receipt.ItemValor.ToString("N2"), 45));


                //texto = texto.Replace("<<codigoTarjeta>>", receipt.CodigoTarjeta);

                texto = texto.Replace("<<total_pagar>>", receipt.TotalTransacccion.ToString("N2"));
                texto = texto.Replace("<<factura_cambio>>", Control.Common.StringHelper.DevolverConPadding(receipt.CambioTransacccion.ToString("N2"), 65));
                texto = texto.Replace("<<mensaje_pieRecibo>>", receipt.PieRecibo);
                //---Pagos
                StringBuilder strPagos = new StringBuilder();
                foreach (var pago in receipt.Pagos)
                {
                    strPagos.AppendLine(pago.Descripcion + Convert.ToChar(9) + ":" + Control.Common.StringHelper.DevolverConPadding(pago.Valor.ToString("N2"), 65));
                }
                texto = texto.Replace("<<FacPagos>>", strPagos.ToString().Trim());
                texto = texto.Replace("<<mensaje_pieRecibo>>", receipt.PieRecibo);

                Control.Common.Printer.Imprimir(texto, 3, 11);
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirReciboVentaGiftcard", "error: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirReciboVentaGiftcard", "InnerException.Message: " + ex.InnerException.Message);
            }

        }

        //  Imprimir retenciones electronicas.  JM 2019-03-20
        public static void ImprimirRetencionElectronica(string texto, Models.PrinterRecipes.ReceiptRetencionElectronica receipt)
        {
            try
            {
                texto = texto.Replace("<<oficina>>", receipt.Oficina);
                texto = texto.Replace("<<telefono>>", receipt.Telefono);
                texto = texto.Replace("<<retencion>>", receipt.NumRetencion);
                texto = texto.Replace("<<cajero>>", receipt.CajeroNombre);
                texto = texto.Replace("<<retencion_fecha>>", receipt.FechaAutorizacion);
                texto = texto.Replace("<<cedula>>", receipt.ClienteIdentificacion);
                texto = texto.Replace("<<cliente>>", receipt.ClienteNombre);
                texto = texto.Replace("<<direccion>>", receipt.ClienteDireccion);
                texto = texto.Replace("<<cliente_telefono>>", receipt.ClienteTelefono);
                texto = texto.Replace("<<concepto>>", receipt.ConceptoTransaccion);
                texto = texto.Replace("<<retencion_devuelto>>", receipt.Total);

                Control.Common.Printer.Imprimir(texto, 3, 11);

            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirRetencionElectronica", "error: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirRetencionElectronica", "InnerException.Message: " + ex.InnerException.Message);
            }

        }

        //  Imprimir retenciones electronicas.  JM 2019-03-20
        public static void ImprimirPagoTarjetaEmpresarial(string texto, Models.PrinterRecipes.ReceiptTarjetaEmpresarial receipt)
        {
            try
            {
                texto = texto.Replace("<<oficina>>", receipt.Oficina);
                texto = texto.Replace("<<telefono>>", receipt.Telefono);
                texto = texto.Replace("<<cajero>>", receipt.CajeroNombre);
                texto = texto.Replace("<<fecha_emision>>", receipt.FechaEmision);
                texto = texto.Replace("<<cedula>>", receipt.ClienteIdentificacion);
                texto = texto.Replace("<<cliente>>", receipt.ClienteNombre);
                texto = texto.Replace("<<direccion>>", receipt.ClienteDireccion);
                texto = texto.Replace("<<cliente_telefono>>", receipt.ClienteTelefono);
                texto = texto.Replace("<<concepto>>", receipt.ConceptoTransaccion);
                texto = texto.Replace("<<total>>", receipt.Total);

                Control.Common.Printer.Imprimir(texto, 3, 11);
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirPagoTarjetaEmpresarial", "error: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirPagoTarjetaEmpresarial", "InnerException.Message: " + ex.InnerException.Message);
            }

        }

        /// <summary>
        /// Impresión de Recibos para Retenciones Fisicas.
        /// Evelasco 2019-10-17
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="receipt"></param>
        public static void ImprimirRetencionFisica(string texto, Models.PrinterRecipes.ReceiptRetencionElectronica receipt)
        {
            try
            {
                texto = texto.Replace("<<oficina>>", receipt.Oficina);
                texto = texto.Replace("<<telefono>>", receipt.Telefono);
                texto = texto.Replace("<<retencion>>", receipt.NumRetencion);
                texto = texto.Replace("<<cajero>>", receipt.CajeroNombre);
                texto = texto.Replace("<<retencion_fecha>>", receipt.FechaAutorizacion);
                texto = texto.Replace("<<cedula>>", receipt.ClienteIdentificacion);
                texto = texto.Replace("<<cliente>>", receipt.ClienteNombre);
                texto = texto.Replace("<<direccion>>", receipt.ClienteDireccion);
                texto = texto.Replace("<<cliente_telefono>>", receipt.ClienteTelefono);
                texto = texto.Replace("<<concepto>>", receipt.ConceptoTransaccion);
                texto = texto.Replace("<<retencion_devuelto>>", receipt.Total);

                Control.Common.Printer.Imprimir(texto, 3, 11);
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirRetencionFisica", "error: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirRetencionFisica", "InnerException.Message: " + ex.InnerException.Message);
            }
            
        }

        public static void ImprimirFlete(string texto, Models.PrinterRecipes.ReceiptFleteMotorizado receipt)
        {
            try
            {
                texto = texto.Replace("<<oficina>>", receipt.Oficina);
                texto = texto.Replace("<<telefono>>", receipt.Telefono);
                texto = texto.Replace("<<cajero>>", receipt.CajeroNombre);
                texto = texto.Replace("<<cajero_cedula>>", receipt.CajeroCedula);
                texto = texto.Replace("<<fechaorden_completado>>", receipt.FechaOrdenCompletada);
                texto = texto.Replace("<<motorizado_codigo>>", receipt.MotorizadoCodigo);
                texto = texto.Replace("<<motorizado_nombre>>", receipt.MotorizadoNombre);
                texto = texto.Replace("<<fecha_recibo>>", receipt.FechaRecibo);
                /* texto = texto.Replace("<<cedula>>", receipt.ClienteIdentificacion);
                 texto = texto.Replace("<<cliente>>", receipt.ClienteNombre);
                 texto = texto.Replace("<<direccion>>", receipt.ClienteDireccion);*/
                //            texto = texto.Replace("<<cliente_telefono>>", receipt.ClienteTelefono);
                texto = texto.Replace("<<concepto>>", receipt.ConceptoTransaccion);
                texto = texto.Replace("<<valor_total>>", receipt.Total);

                Control.Common.Printer.Imprimir(texto, 3, 11);
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirFlete", "error: " + ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Printer", "ImprimirFlete", "InnerException.Message: " + ex.InnerException.Message);
            }
            
        }


        public static void ExportarPDF(string contenido, int tipo = 1, int ptosInterlineado = 15, string documentoFactura = "")
        {
            string rutaArchivo = @"C:\POS\pdfTicketsVenta\" + documentoFactura + ".pdf";

            int numLineas = contenido.Split('\n').Length;
            // float altoPorLinea = 12f; // Espaciado de línea aproximado
            float height = numLineas * ptosInterlineado + 20; // Ajustar margen superior/inferior
            float width = 58f * 2.83f;

            Document documento = new Document(new iTextSharp.text.Rectangle(width, height), 5, 5, 10, 10);

            try
            {

                PdfWriter writer = PdfWriter.GetInstance(documento, new FileStream(rutaArchivo, FileMode.Create, FileAccess.Write));
                documento.Open();

                //System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                System.Drawing.Font fuente;
                iTextSharp.text.Font iTextSharpFuente;
                Paragraph parrafo;

                switch (tipo)
                {
                    case 1:
                        BaseFont fuenteBase = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                        fuente = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                        iTextSharpFuente = new iTextSharp.text.Font(fuenteBase, 7, iTextSharp.text.Font.BOLD);
                        parrafo = new Paragraph(contenido, iTextSharpFuente);
                        documento.Add(parrafo);
                        break;

                    case 2:
                        BaseFont fuenteBase2 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                        fuente = new System.Drawing.Font("VERDANA", 8, FontStyle.Bold);
                        iTextSharpFuente = new iTextSharp.text.Font(fuenteBase2, 8, iTextSharp.text.Font.BOLD);
                        parrafo = new Paragraph(contenido, iTextSharpFuente);
                        documento.Add(parrafo);

                        break;
                    case 3:
                        BaseFont fuenteBase3 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                        fuente = new System.Drawing.Font("VERDANA", 6, FontStyle.Bold);
                        iTextSharpFuente = new iTextSharp.text.Font(fuenteBase3, 6, iTextSharp.text.Font.BOLD);
                        parrafo = new Paragraph(contenido, iTextSharpFuente);
                        documento.Add(parrafo);

                        break;
                    case 4:
                        BaseFont fuenteBase4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.EMBEDDED);
                        fuente = new System.Drawing.Font("VERDANA", 5, FontStyle.Bold);
                        iTextSharpFuente = new iTextSharp.text.Font(fuenteBase4, 6, iTextSharp.text.Font.BOLD);
                        parrafo = new Paragraph(contenido, iTextSharpFuente);
                        documento.Add(parrafo);
                        break;

                    case 5:
                        string codigo = contenido.Substring(contenido.IndexOf("<barcode>") + 9, contenido.IndexOf("</barcode>") - contenido.IndexOf("<barcode>") - 9);
                        contenido = contenido.Replace("<barcode>", Environment.NewLine).Replace("</barcode>", Environment.NewLine);
                        contenido = contenido.Replace(codigo, Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                        codigo = codigo.Trim();

                        System.Drawing.Image qrImage = GenerarCodigoQR(codigo);
                        if (qrImage != null)
                        {
                            iTextSharp.text.Image imagenPdf = iTextSharp.text.Image.GetInstance(qrImage, ImageFormat.Png);
                            imagenPdf.ScaleAbsolute(100f, 100f); // Ajustar tamaño del QR
                            imagenPdf.Alignment = Element.ALIGN_CENTER;
                            documento.Add(imagenPdf);
                        }

                        break;
                    default:

                        BaseFont fuenteBaseDefault = BaseFont.CreateFont("COURIER NEW", BaseFont.CP1252, BaseFont.EMBEDDED);
                        fuente = new System.Drawing.Font("COURIER NEW", 5, FontStyle.Bold);
                        iTextSharpFuente = new iTextSharp.text.Font(fuenteBaseDefault, 6, iTextSharp.text.Font.BOLD);
                        parrafo = new Paragraph(contenido, iTextSharpFuente);
                        documento.Add(parrafo);
                        break;

                }

                Console.WriteLine("PDF generado con éxito en: " + rutaArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

            }
            finally
            {
                documento.Close();

            }


        }

        private static System.Drawing.Image GenerarCodigoQR(string texto)
        {
            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 150,
                    Width = 150,
                    Margin = 1
                }
            };

            Bitmap bitmap = writer.Write(texto);
            return (System.Drawing.Image)bitmap;
        }


    }
}
