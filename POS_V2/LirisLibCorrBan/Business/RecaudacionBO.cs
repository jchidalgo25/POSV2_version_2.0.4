using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LirisLibCorrBan.Models;
using System.Collections;

namespace LirisLibCorrBan.Business
{
    public static class RecaudacionBO
    {
        public static IEnumerable GetRecaudacionesReprint(short tipoRecaudo, DateTime fecha, string cuenta, string idCajaTurno = "")
        {
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    IEnumerable result = db.Recaudaciones
                                                .Join(db.AudRecaudaciones,
                                                    r => r.Id,
                                                    a => a.IdRecaudacionTran,
                                                    (r, a) => new { r, a })
                                                .Join(db.InstRecaudos,
                                                    ra => ra.r.IdIRecaudos,
                                                    i => i.IdIRecaudos,
                                                    (ra, i) => new { ra, i })
                                                .Where(x => (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.ra.r.FechaCreacion) == (DateTime)System.Data.Entity.DbFunctions.TruncateTime(fecha.Date)
                                                            && x.ra.r.Cuenta.Equals(cuenta)
                                                            && x.ra.a.TipoTransaccion == tipoRecaudo
                                                            && x.ra.r.Estado == "0000" 
                                                            && x.ra.r.IdCajaApertura == idCajaTurno
                                                            && x.ra.a.Trama.Contains("TypePrintDocument>F"))
                                                .Select(x => new {
                                                                Trama = x.ra.a.Trama,
                                                                Cuenta = x.ra.r.Cuenta,
                                                                Fecha = x.ra.r.FechaCreacion,
                                                                Valor = x.ra.r.ValorRecaudado,
                                                                Corresponsal = x.i.NombreEntidad,
                                                                CodigoEmpresa = x.i.CodigoEmpresa,
                                                                SecuencialSara = x.ra.r.SecuencialSara,
                                                                Secuencial = x.ra.r.Secuencial,
                                                                Canal = x.ra.r.Canal,
                                                                Terminal = x.ra.r.Terminal,
                                                                Operador = x.ra.r.Operador,
                                                                IdRecaudacion = x.ra.r.Id,
                                                                IdIRecaudos = x.i.IdIRecaudos,
                                                                IdTipoCorrBan = x.i.IdTipoCorrBan,
                                                                CodigoCliente = x.ra.r.CodigoCliente
                                                })
                                                .ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "RecaudacionBO", "GetRecaudacionesReprint", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                return null;
            }
        }

        public static IEnumerable GetRecaudacionesWFReprint(short tipoRecaudo, DateTime fecha, string cuenta, string idCajaTurno = "")
        {
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    IEnumerable result = db.VieReqsWF
                                                .Where(x => (DateTime)System.Data.Entity.DbFunctions.TruncateTime(x.Fecha) == (DateTime)System.Data.Entity.DbFunctions.TruncateTime(fecha.Date)
                                                            && x.Cuenta.Equals(cuenta)
                                                            && x.TipoRecaudo == tipoRecaudo
                                                            && x.Estado == "0000"
                                                            && x.IdCaja == idCajaTurno
                                                            && (x.WFEstado == 410 || x.WFEstado == 420 || x.WFEstado == 499)
                                                            //&& x.Trama.Contains("TypePrintDocument>F"))
                                                            && x.Trama.Contains("SaveCollectionResult"))
                                                .OrderByDescending(x => x.Fecha)
                                                .ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "RecaudacionBO", "GetRecaudacionesWFReprint", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                return null;
            }
        }
    }
}
