using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LirisLibCorrBan.Models;

namespace LirisLibCorrBan.Business
{
    public static class SoapCorrBanBO
    {
        public static Models.SoapCorrBanDet GetSoapDetail(int idMethod)
        {
            try
            {
                using (CorrBanEntities db = new CorrBanEntities())
                {
                    var SoapDetail = db.SoapCorrBanDet
                                                .Where(x => x.Id == idMethod)
                                                .FirstOrDefault();
                    return SoapDetail;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "SoapCorrBanBO", "SoapCorrBanDet", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                return null;
            }
        }
    }
}
