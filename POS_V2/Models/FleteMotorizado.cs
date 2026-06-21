using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class FleteMotorizado
    {
        public string CajeroNombre { get; set; }
        public string CajeroCedula { get; set; }
        public string MotorizadoCodigo { get; set; }
        public string MotorizadoNombre { get; set; }
        public string OrdenApp { get; set; }
        public string ConceptoTransaccion { get; set; }
        public string IdCaja { get; set; }
        public string Recibo { get; set; }
        public string ReciboCOPIA { get; set; }
        public string Total { get; set; }
        public string ReciboLineas { get; set; }
        public string Fecha { get; set; }

        public string MsgError { get; set; }
        public void ImprimirRecibo()
        {
            try
            {
                var receiptModel = new Models.PrinterRecipes.ReceiptFleteMotorizado();
                receiptModel.CajeroNombre = Control.Common.GlobalParameters.UserObj.nombres;
                receiptModel.CajeroCedula = Control.Common.GlobalParameters.UserObj.username;
                receiptModel.MotorizadoCodigo= MotorizadoCodigo;
                receiptModel.MotorizadoNombre = MotorizadoNombre;
                receiptModel.OrdenApp = OrdenApp;
                receiptModel.Total = Total;

                //receiptModel.ClienteTelefono = Customer.PHONE;
                receiptModel.ConceptoTransaccion = "Flete Pedido a Domicilio Orden App:" + receiptModel.OrdenApp;
                //receiptModel.FechaOrdenCompletada = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                receiptModel.IdCaja = IdCaja;
                receiptModel.FechaRecibo = Fecha;
                //receiptModel.NumAutorizacion = NumAutorizacion;
                //-------DIRECCION ESTABLECIMIENTO------------------------------------

                var sub1 = "";
                var sub2 = "";
                var pos = new POSEntities();

                if (Control.Common.GlobalParameters.EstablecimientoDireccion.Length > 20)
                {
                    int largo = Control.Common.GlobalParameters.EstablecimientoDireccion.Length;
                    sub1 = Environment.NewLine + Control.Common.GlobalParameters.EstablecimientoDireccion.Substring(0, 20);
                    sub2 = Environment.NewLine + Control.Common.GlobalParameters.EstablecimientoDireccion.Substring(20, Control.Common.GlobalParameters.EstablecimientoDireccion.Length - 20);
                }
                receiptModel.Oficina = Control.Common.GlobalParameters.EstablecimientoDireccion + sub1 + sub2;
                //-----------------------------------------------------------------
                receiptModel.Telefono = Control.Common.GlobalParameters.EstablecimientoTelefono;
               // receiptModel.Total = Total.ToString("N2");

                Control.Common.Printer.ImprimirFlete(Recibo, receiptModel);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.FleteMotorizado", "ImprimirRecibo", "Imposible imprimir recibo en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                MsgError = "No se pudo realizar la impresion del recibo de la transacción realizada";
                throw ex;
            }

        }
    }
}
