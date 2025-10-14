using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LirisLibCorrBan.Models;

namespace LirisLibCorrBan.Business
{
    public static class InstRecaudoBO
    {
        public static List<Models.InstRecaudos> GetRecaudosForCombo(int tipoRecaudo)
        {
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    var listaRecaudos = db.InstRecaudos
                                                .Where(x => x.IdTipoCorrBan == tipoRecaudo && x.Estado == true)
                                                .OrderBy(x => x.NombreEntidad)
                                                .ToList();
                    listaRecaudos.Insert(0, new InstRecaudos()
                    {
                        IdIRecaudos     = (short)0,
                        CodigoEmpresa   = "00",
                        NombreEntidad   = "-- SELECCIONE --"
                    });
                    return listaRecaudos;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "InstRecaudoBO", "GetRecaudosForCombo", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                return null;
            }
        }

        public static List<Models.ReglasRecaudos> GetSubtypesRecaudos(short idRecaudo)
        {
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    var listSubtypesRecaudos = db.ReglasRecaudos
                                                    .Where(x => x.IdIRecaudos == idRecaudo && x.Estado == true)
                                                    .OrderBy(x => x.TipoServicio)
                                                    .ToList();

                    //Cuando no existan subtipos o cuando haya mas de un subtipo, se presentara el item "Seleccione"
                    //Objetivo: No permitir error del cajero, obligandolo a seleccionar
                    if (listSubtypesRecaudos.Count > 1 || idRecaudo == 0)
                    {
                        listSubtypesRecaudos.Insert(0, new ReglasRecaudos()
                        {
                            Id = 0,
                            IdIRecaudos = 0,
                            CampoObliga = "Cuenta",
                            CampoOpcional = "Cuenta",
                            IdTipoServicio = "-1",
                            TipoServicio = "-- SELECCIONE --"
                        });
                    }
                    else if (listSubtypesRecaudos.Count == 0)
                    {
                        listSubtypesRecaudos.Insert(0, new ReglasRecaudos()
                        {
                            Id = 0,
                            IdIRecaudos = 0,
                            CampoObliga = "Cuenta",
                            CampoOpcional = "Cuenta",
                            IdTipoServicio = "0",
                            TipoServicio = "RECAUDACION GENERAL"
                        });
                    }
                    return listSubtypesRecaudos;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "InstRecaudoBO", "GetSubtypesRecaudos", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                return null;
            }
        }
    }
}
