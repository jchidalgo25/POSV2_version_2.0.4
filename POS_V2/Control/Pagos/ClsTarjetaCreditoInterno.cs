using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.Pagos
{
    public class ClsTarjetaCreditoInterno
    {
        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
        public bool TieneYPuedeRealizarPagoTarPortal(Models.Factura factura)
        {
            bool respuesta = true;
            

            try
            {
                if (factura != null)
                {
                    if (factura.Pagos != null)
                    {
                        if (factura.Pagos.Any(x => x.Descripcion.Contains("TAR PORTAL")))
                        {
                            var pagoTarPortal = factura.Pagos.Where(x => x.Descripcion.Contains("TAR PORTAL")).FirstOrDefault();
                            var pagoNoPermitido = (Models.PagoTarjetaInterna)pagoTarPortal.Pagos.Where(x => !((Models.PagoTarjetaInterna)x).Titular
                                            .Equals(factura.ClienteIdentificacion)).FirstOrDefault();

                            if (pagoNoPermitido != null)
                            {
                                respuesta = false;


                                string MsgError = "La Tar.DelPortal no puede ser usada en esta factura porque el cliente actual '" + factura.ClienteIdentificacion + "' no es el empleado principal(" + pagoNoPermitido.Titular + ")";
                                


                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[MsgError]", valor = MsgError });
                                Control.Common.General.GetMensajeToList(571, parametros);

                                //Common.WinForm.ShowMessage(MsgError + ". Vuelva a deslizar la tarjeta");
                                Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "POS.Control.Pagos.ClsTarjetaCreditoInterno", "TieneYPuedeRealizarPagoTarPortal", MsgError + ". Codigo Tarjeta: " + pagoNoPermitido.Codigo + ". FacturaRef: " + factura.GetNumeroFactura());

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Pagos.ClsTarjetaCreditoInterno", "TieneYPuedeRealizarPagoTarPortal", "La verificacion de pago TAR PORTAL no pudo realizarse en la factura " + factura.GetNumeroFactura() + ", se permitirá continuar la facturación. A continuacion las excepciones encontradas - " + Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                //Si la logica tiene problemas para completarse, no bloqueo facturacion
                respuesta = true;
            }

            return respuesta;
        }
    }
}
