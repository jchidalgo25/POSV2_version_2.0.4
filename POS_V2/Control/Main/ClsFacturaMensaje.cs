using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;

namespace POS.Control.Main
{
    public class ClsFacturaMensaje
    {
        public void ShowInvoiceMessages(string accountNum)
        {
            try
            {
                string messagesToShow = string.Empty;
                using (POSEntities db = new POSEntities())
                {
                    var listMessagesTypes = db.core_parametro
                                                .Where(x => x.identificador == "FACTURAMENSAJE" && x.parametro2 == "TRUE")
                                                .ToList();

                    foreach (core_parametro messageType in listMessagesTypes)
                    {
                        if (messageType.valor == "PUNTOS_SALDO")
                        {
                            var ptosCabClient = db.TblPuntosCab.Where(x => x.AccountNum == accountNum).FirstOrDefault();

                            if (ptosCabClient != null)
                            {
                                var listMessages = db.core_parametro
                                                        .Where(x => x.identificador == "FACTURAMENSAJE_DETALLE"
                                                                    &&
                                                                    x.valor == "PUNTOS_SALDO")
                                                        .ToList();

                                foreach (core_parametro message in listMessages)
                                {
                                    var rangoSplit = (string.IsNullOrWhiteSpace(message.parametro2) ? "" : message.parametro2).Split('|');

                                    if (rangoSplit.Count() == 2)
                                    {
                                        decimal desde = -1, hasta = -1;
                                        if (decimal.TryParse(rangoSplit[0], out desde) && 
                                            decimal.TryParse(rangoSplit[1], out hasta))
                                        {
                                            if (desde <= hasta)
                                            {
                                                if (ptosCabClient.Saldo >= desde && ptosCabClient.Saldo <= hasta)
                                                {
                                                    messagesToShow += message.documento + Environment.NewLine;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(messagesToShow))
                {
                    Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Main.ClsFacturaMensaje", "ShowInvoiceMessages", $"messagesToShow: {messagesToShow}");
                    Control.Common.General.GetMensaje("POS", messagesToShow, "I");
                    //MessageBox.Show(messagesToShow, "POS", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Main.ClsFacturaMensaje", "ShowInvoiceMessages", "Los mensajes no pudieron ser mostrados debido a una breve interrupcion en el servicio. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
            }
        }
    }
}
