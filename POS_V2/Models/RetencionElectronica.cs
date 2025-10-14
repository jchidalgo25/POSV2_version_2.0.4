using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace POS.Models
{
    public class RetencionElectronica
    { 
        public string NumRetencion { get; set; }
        public string NumFactura { get; set; }
        public string NumAutorizacion { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal ValorBase { get; set; }
        public decimal ValorRetFte { get; set; }
        public decimal ValorBaseIVA { get; set; }
        public decimal ValorRetIVA { get; set; }
        public string CodRetIVA { get; set; }
        public decimal CodPorcRetIVA { get; set; }
        public string ConceptoRetIVA { get; set; }
        public decimal Total { get; set; }
        public int CodigoImpuesto { get; set; }
        public int TipoRetencion { get; set; }
        public pos_customer Customer { get; set; }
        public string Recibo {  get; set; }
        public string XML { get; set; }
        public string PDF { get; set; }
        public string MsgError { get; set; }
        public string ConceptoTransaccion { get; set; }
        public bool ContribuyenteEspecial { get; set; }

        public decimal ValorRetFte175 { get; set; }
        
        public void ImprimirRecibo()
        { 
            try
            {
                var receiptModel = new Models.PrinterRecipes.ReceiptRetencionElectronica();
                receiptModel.CajeroNombre = Control.Common.GlobalParameters.UserObj.nombres;
                receiptModel.ClienteDireccion = Customer.STREET;
                receiptModel.ClienteIdentificacion = Customer.ACCOUNTNUM;
                receiptModel.ClienteNombre = Customer.NAME;
                receiptModel.ClienteTelefono = Customer.PHONE; 
                receiptModel.ConceptoTransaccion = string.IsNullOrEmpty(this.ConceptoTransaccion)? "Devolución de Retención Electrónica" : this.ConceptoTransaccion;
                receiptModel.FechaAutorizacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                receiptModel.NumRetencion = NumRetencion;
                receiptModel.NumAutorizacion = NumAutorizacion;
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
                receiptModel.Total = Total.ToString("N2");              

                Control.Common.Printer.ImprimirRetencionElectronica(Recibo, receiptModel);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.RetencionElectronica", "ImprimirRecibo", "Imposible imprimir comprobante en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);                
                MsgError = "No se pudo realizar la impresion del comprobante de la transacción realizada";
                throw ex;
            }
             
        }
        /// <summary>
        /// Imprime Retenciones Fisicas (Manuales).
        /// </summary>
        public void ImprimirReciboFisica()
        {
            try
            {
                var receiptModel = new Models.PrinterRecipes.ReceiptRetencionElectronica();
                receiptModel.CajeroNombre = Control.Common.GlobalParameters.UserObj.nombres;
                receiptModel.ClienteDireccion = Customer.STREET;
                receiptModel.ClienteIdentificacion = Customer.ACCOUNTNUM;
                receiptModel.ClienteNombre = Customer.NAME;
                receiptModel.ClienteTelefono = Customer.PHONE;
                receiptModel.ConceptoTransaccion = string.IsNullOrEmpty(this.ConceptoTransaccion) ? "Devolución de Retención" : this.ConceptoTransaccion;
                //receiptModel.FechaAutorizacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                receiptModel.FechaAutorizacion = FechaAutorizacion.ToString("dd/MM/yyyy HH:mm:ss");
                receiptModel.NumRetencion = NumRetencion;
                receiptModel.NumAutorizacion = NumAutorizacion;
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
                receiptModel.Total = Total.ToString("N2");

                Control.Common.Printer.ImprimirRetencionFisica(Recibo, receiptModel);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Models.RetencionElectronica", "ImprimirReciboFisica", "Imposible imprimir comprobante en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                MsgError = "No se pudo realizar la impresion del comprobante de la transacción realizada";
                throw ex;
            }

        }
    }
}
