using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LirisLibCorrBan.Models;
using System.Data.Entity;

namespace LirisLibCorrBan.Business
{
    public static class CatalogoBO
    {
        public static int? GetDetCatalogId(string nameCab, string nameDet)
        {
            int? response;
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    response = db.CatalogosRecaudoDet
                                        .Where(x => x.Codigo == nameDet 
                                                    && 
                                                    x.LstCatalogosRecaudo.IdTabla == nameCab)
                                        .FirstOrDefault()
                                        .Id;

                    if (response == null)
                    {
                        throw new Exception("La consulta se realizo pero no se encontro una coincidencia con los parametros enviados");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "CatalogoBO", "GetDetCatalogId", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = null;
            }

            return response;
        }

        public static List<CatalogosRecaudo> GetCatalogosCab()
        {
            List<CatalogosRecaudo> response;
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    response = db.CatalogosRecaudo.Where(x => x.Estado == true)
                                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "CatalogoBO", "GetCatalogosCab", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = null;
            }

            return response;
        }

        public static CatalogosRecaudo GetCatalogByName(string description)
        {
            CatalogosRecaudo response;
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    response = db.CatalogosRecaudo.Where(x => x.Estado == true && x.IdTabla.Equals(description))
                                        .Include(x => x.LstCatalogosRecaudoDet)
                                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "CatalogoBO", "GetCatalogByName", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = null;
            }

            return response;
        }

        public static List<CatalogosRecaudoDet> GetCatalogosDet(int? idCab = null)
        {
            List<CatalogosRecaudoDet> response;
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    if (idCab == null)
                        response = db.CatalogosRecaudoDet.Where(x => x.LstCatalogosRecaudo.Estado == true)
                                        .ToList();
                    else
                        response = db.CatalogosRecaudoDet.Where(x => x.LstCatalogosRecaudo.Estado == true && x.IdCatalogosRecaudo == idCab)
                                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "CatalogoBO", "GetCatalogosDet", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                response = null;
            }

            return response;
        }


    }
}
