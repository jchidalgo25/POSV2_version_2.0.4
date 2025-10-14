using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;

namespace POS.Control.Promos
{
    public class PromoIVA
    {
        public static decimal PorcPromo { get; set; }
        public static bool EsPromoIVA()
        {
            return TieneEstPromoIVA && Common.Promo.EsDiaPromoIVA();
        }
        public static bool TieneEstPromoIVA { get; set; }

        public static bool RecargarParametrosPromoIVA()
        {
            bool respuesta = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //Setear variable Promo IVA
                    var param = db.core_parametro.Where(x => x.identificador == "PROMO_IVA"
                                             && (x.parametro2 == Common.GlobalParameters.Establecimiento || x.parametro2 == null))
                                     .FirstOrDefault();

                    if (param != null)
                    {
                        TieneEstPromoIVA = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        TieneEstPromoIVA = false;
                        throw new Exception("No hay parametro 'PROMO_IVA' para este local (" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.Establecimiento) ? "La variable estática Control.Common.GlobalParameters.Establecimiento tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.Establecimiento) + ") en core_parametro.");
                    }

                    //Setear porcentaje de promo IVA para el local
                    //param = db.core_parametro.Where(x => x.identificador == "DESC_PROMO_IVA")
                    //                 .FirstOrDefault();

                    //if (param != null)
                    //{
                    //    PorcPromo = Decimal.Parse(string.IsNullOrWhiteSpace(param.parametro2) ? "0" : param.parametro2);
                    //}
                    //else
                    //{
                    //    PorcPromo = 0M;
                    //    throw new Exception("No hay parametro 'DESC_PROMO_IVA' en core_parametro.");
                    //}

                    //Setear dias de promo IVA para el local
                    if (!string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode))
                    {
                        param = db.core_parametro.Where(x => x.identificador == ("DIAS_PROMO_IVA_" + Control.Common.GlobalParameters.EstablecimientoAxCode) &&
                                                            x.valor == "TRUE").FirstOrDefault();
                        if (param != null)
                        {
                            string cadenaParametrizada = (string.IsNullOrWhiteSpace(param.parametro2) ? "" : param.parametro2);
                            Common.Promo._listaDiasPromoIVA = cadenaParametrizada.Split(';').Select(x => int.Parse(string.IsNullOrWhiteSpace(x) ? "-1" : x)).ToList();
                        }
                        else
                        {
                            if (TieneEstPromoIVA)
                            {
                                throw new Exception("No esta configurado parametro '" +
                                "DIAS_PROMO_IVA_" + Control.Common.GlobalParameters.EstablecimientoAxCode +
                                "' con valor 'TRUE' en core_parametro");
                            }
                            else { 
                                Common.Promo._listaDiasPromoIVA = "-1".Split(';').Select(x => int.Parse(string.IsNullOrWhiteSpace(x) ? "-1" : x)).ToList();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Aun no esta cargado el codigo de almacen Ax 'ARDP' en la variable Control.Common.GlobalParameters.EstablecimientoAxCode de POS");
                    }             
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "PromoIVA", "RecargarParametrosPromoIVA", Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                respuesta = false;
            }

            return respuesta;
        }
    }
}
