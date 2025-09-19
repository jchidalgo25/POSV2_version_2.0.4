using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.NotaCredito
{
    public class NotaCreditoModel
    {
        public int codError { get; set; }
        public string msjError { get; set; }
        public CabNotaCredito cabNotaCredito { get; set; }
        public List<DetNotaCredito> detNotaCredito { get; set; }


        public NotaCreditoModel GetFactura(string establecimiento, string punto_emision, string numero)
        {

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "NotaCreditoModel", "GetFactura", "Recupero los datos de la factura para ejecutar la NC");


            NotaCreditoModel respuesta = new NotaCreditoModel();
            DetNotaCredito detNC = new DetNotaCredito();

            DataSet dtsConsulta = new DataSet();
            string query = string.Empty;
            try
            {

                query = string.Concat(query, "Exec spConsultaFacturaNC", Environment.NewLine);
                query = string.Concat(query, $" @establecimiento = '{establecimiento}'", Environment.NewLine);
                query = string.Concat(query, $" , @punto_emision = '{punto_emision}'", Environment.NewLine);
                query = string.Concat(query, $" , @numFactura = {numero}", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(query);


                if (dtsConsulta.Tables.Count > 0)
                {
                    respuesta.cabNotaCredito = new CabNotaCredito();
                    respuesta.detNotaCredito = new List<DetNotaCredito>();

                    int codError = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["codError"].ToString());
                    string msjError = dtsConsulta.Tables[0].Rows[0]["msjError"].ToString();

                    


                    if (codError != 0) {
                        respuesta.codError = codError;
                        respuesta.msjError = msjError;
                        return respuesta;
                    }

                    respuesta.codError = codError;
                    respuesta.msjError = msjError;


                    for (int iTable = 1; iTable <= dtsConsulta.Tables.Count - 1; iTable++)
                    {
                        string tipoMensaje = dtsConsulta.Tables[iTable].Rows[0]["tipoConsulta"].ToString();
                        if(tipoMensaje == "CABE_CONS")
                        {

                            respuesta.cabNotaCredito = new CabNotaCredito();
                            foreach (DataRow dataRowCab in dtsConsulta.Tables[iTable].Rows)
                            {
                                respuesta.codError = 0;
                                respuesta.cabNotaCredito.documentoAplica = dataRowCab["documentoAplica"].ToString();
                                respuesta.cabNotaCredito.cliente = dataRowCab["cliente"].ToString();
                                respuesta.cabNotaCredito.nombreCliente = dataRowCab["nombreCliente"].ToString();
                                respuesta.cabNotaCredito.direccionCliente = dataRowCab["direccionCliente"].ToString();
                                respuesta.cabNotaCredito.telefonoCliente = dataRowCab["telefonoClte"].ToString();
                                respuesta.cabNotaCredito.subtotal = decimal.Parse(dataRowCab["subtotal"].ToString());
                                respuesta.cabNotaCredito.descuento = decimal.Parse(dataRowCab["descuento"].ToString());
                                respuesta.cabNotaCredito.descuento2 = decimal.Parse(dataRowCab["descuento2"].ToString());
                                respuesta.cabNotaCredito.iva    = decimal.Parse(dataRowCab["iva"].ToString());
                                respuesta.cabNotaCredito.total = decimal.Parse(dataRowCab["total"].ToString());
                                respuesta.cabNotaCredito.base0 = decimal.Parse(dataRowCab["base0"].ToString());
                                respuesta.cabNotaCredito.baseIva = decimal.Parse(dataRowCab["base12"].ToString());

                                bool esBeneficiarioDevolucionIVA = false;
                                if ((bool)dataRowCab["esBeneficiarioDevolucionIVA"])
                                {
                                    esBeneficiarioDevolucionIVA = true;


                                }
                                respuesta.cabNotaCredito.esBeneficiarioDevolucionIVA = esBeneficiarioDevolucionIVA;
                                respuesta.cabNotaCredito.montoIvaDevolver = decimal.Parse(dataRowCab["montoIvaDevolver"].ToString());


                                bool esConsumidorFinal = false;
                                if ((bool)dataRowCab["esConsumidorFinal"])
                                {
                                    esConsumidorFinal = true;

                                }
                                respuesta.cabNotaCredito.esConsumidorFinal = esConsumidorFinal;


                                bool permiteNCConsumidorFinal = false;
                                if ((bool)dataRowCab["permiteNCConsumidorFinal"])
                                {
                                    permiteNCConsumidorFinal = true;

                                }
                                respuesta.cabNotaCredito.permiteNCConsumidorFinal = permiteNCConsumidorFinal;

                                bool tienePago = false;
                                if ((bool)dataRowCab["tienePago"])
                                {
                                    tienePago = true;

                                }
                                respuesta.cabNotaCredito.tienePago = tienePago;


                            }

                            continue;
                        }


                        if (tipoMensaje == "DETA_CONS")
                        {
                            foreach (DataRow dataRowDet in dtsConsulta.Tables[iTable].Rows)
                            {

                                try
                                {
                                    detNC = new DetNotaCredito();
                                    detNC.id = dataRowDet["id"].ToString();
                                    detNC.item_id = dataRowDet["item_id"].ToString();
                                    detNC.item_name = dataRowDet["Nombre"].ToString();
                                    detNC.costo = decimal.Parse(dataRowDet["costo"].ToString());
                                    detNC.unidades = decimal.Parse(dataRowDet["unidades"].ToString());
                                    detNC.unidad = dataRowDet["unidad"].ToString();
                                    detNC.pvp = decimal.Parse(dataRowDet["pvp"].ToString());
                                    detNC.subtotal = decimal.Parse(dataRowDet["subtotal"].ToString());
                                    detNC.iva = decimal.Parse(dataRowDet["iva"].ToString());
                                    detNC.ivaProducto = decimal.Parse(dataRowDet["ivaProducto"].ToString());
                                    detNC.descuento = decimal.Parse(dataRowDet["descuento"].ToString());
                                    detNC.cantidad = decimal.Parse(dataRowDet["cantidad"].ToString());
                                    detNC.cantidadINEC = decimal.Parse(dataRowDet["cantidadINEC"].ToString());
                                    detNC.fechaCreacion = DateTime.Parse(dataRowDet["fechaCreacion"].ToString());
                                    respuesta.detNotaCredito.Add(detNC);
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                                
                            }
                            continue;
                        }
                    }


                }
                

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.codError = -1;
                respuesta.msjError = ex.Message;
                
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "NotaCreditoModel", "GetFactura", $"error: {ex.Message}");
                return respuesta;
            }



        }
    }
    public class CabNotaCredito
    {
        public string documentoAplica { get; set; }
        public string cliente { get; set; }
        public string nombreCliente { get; set; }
        public string direccionCliente { get; set; }
        public string telefonoCliente { get; set; }
        public decimal subtotal { get; set; }
        public decimal descuento { get; set; }
        public decimal descuento2 { get; set; }
        public decimal iva { get; set; }
        public decimal total { get; set; }
        public decimal base0 { get; set; }
        public decimal baseIva { get; set; }
        public bool esBeneficiarioDevolucionIVA { get; set; }
        public decimal montoIvaDevolver { get; set; }
        public bool esConsumidorFinal { get; set; }
        public bool permiteNCConsumidorFinal { get; set; }
        public bool tienePago { get; set; }
    }

    public class DetNotaCredito
    {
        public string id { get; set; }
        public string item_id { get; set; }
        public string item_name { get; set; }
        public decimal costo { get; set; }
        public string unidad { get; set; }
        public decimal pvp { get; set; }
        public decimal subtotal { get; set; }
        public decimal total { get; set; }
        public decimal iva { get; set; }
        public decimal ivaProducto { get; set; }
        public decimal descuento { get; set; }
        public decimal cantidad { get; set; }
        public decimal cantidadINEC { get; set; }
        public decimal unidades { get; set; }
        public bool EsExcluidoPromoIVA { get; set; }
        public DateTime fechaCreacion { get; set; }
        public decimal montoIvaDevolver { get; set; }

    }

    

}
