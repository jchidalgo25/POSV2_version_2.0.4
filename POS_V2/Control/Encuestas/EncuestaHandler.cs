using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;

namespace POS.Control.Encuestas
{
    public static class EncuestaHandler
    {
        public static TblEncuestaRealizada EncuestaEnMemoria { get; set; }

        public static void LevantarEncuestaFactura()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    DateTime esteMomento = DateTime.Now;

                    var encuesta = db.LstEncuesta.Where(x => x.Descripcion == "Factura"
                                                                && x.Estado == true
                                                                && esteMomento >= x.FechaDesde
                                                                && esteMomento < x.FechaHasta
                                                                && (
                                                                    x.EsAplicaTodos == true
                                                                    ||
                                                                    (
                                                                        x.EsAplicaTodos == false
                                                                        &&
                                                                        x.LstEncuestaEstablecimiento.Any(d => d.Establecimiento == Control.Common.GlobalParameters.Establecimiento && d.Estado == true)
                                                                    )
                                                                   )
                                                       )
                                                 .FirstOrDefault();
                    if (encuesta != null)
                    {
                        EncuestaEnMemoria = null;
                        var frm = new Control.Encuestas.Encuesta(encuesta);
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "EncuestaHandler", "LevantarEncuestaFactura", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        public static void PersistirEncuestaEnMemoria(int idFactura)
        {
            try
            {
                if (EncuestaEnMemoria != null)
                {
                    using (POSEntities db = new POSEntities())
                    {
                        EncuestaEnMemoria.IdFactura = idFactura;
                        db.TblEncuestaRealizada.Add(EncuestaEnMemoria);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "EncuestaHandler", "PersistirEncuestaEnMemoria", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }

            EncuestaEnMemoria = null;
        }
    }
}
