using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.WalletPoints
{
   public class ClsCuponApp
    {
        public int IdTblPremio { get; set; }
        public string Descripcion { get; set; }
		public int Procesado{ get; set; }
		public string Codigo { get; set; }
        public int CantAplicar { get; set; }
        public decimal Valor { get; set; }
        public bool EstaConfirmado { get; set; }
        public bool SeUsoCuponApp { get; set; }
        public List<Items> Lstitem{ get; set; }
        public Control.WalletPoints.ClsCuponApp Clone()
        {
            var cloned = (Control.WalletPoints.ClsCuponApp)this.MemberwiseClone();

            return cloned;
        }

        /// <summary>
        /// Registra el consumo de gift card en srv-pos
        /// </summary>
        /// <param name="monto_consumo"></param>
        /// <param name="establecimiento"></param>
        /// <param name="desc">Consumo en factura: F-011-999-000000177</param>
        /// <returns></returns>
        public bool realizarConsumoCuponApp(int idTblPremio)
        {

            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(cadenaCon);
                string Query = "Exec PtsCliente.spCuponApp  'PRO'," + "''," + idTblPremio + "";
                System.Data.SqlClient.SqlCommand select = new System.Data.SqlClient.SqlCommand(Query, conn);
                try
                {
                    conn.Open();
                    select.ExecuteNonQuery();
                    conn.Close();
                    
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsCuponApp", "realizarConsumoCuponApp", "premio :" + idTblPremio.ToString());
                    return true;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/WalletPoints/ClsCuponApp", "realizarConsumoCuponApp", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                    //Guarda consumo de premio app en un archivo cuando no hay srv-pos para procesar por timer 
                    var query = "Exec PtsCliente.spCuponApp  'PRO'," + "''," + idTblPremio + "";
                    try
                    {
                        //se adiciona alguna información y la fecha
                        DateTime dateTime = new DateTime();
                        dateTime = DateTime.Now;
                        string strDate = Convert.ToDateTime(dateTime).ToString("yyyyMMddHHmmss");
                        string rutaCompleta = Control.Common.GlobalParameters.ConsumoCuponAppInsertPath + idTblPremio.ToString() + "_" + strDate + ".txt";
                        if (!string.IsNullOrEmpty(Control.Common.GlobalParameters.ConsumoCuponAppInsertPath))
                        {

                            if (!System.IO.Directory.Exists(Control.Common.GlobalParameters.ConsumoCuponAppInsertPath))
                            {
                                System.IO.Directory.CreateDirectory(Control.Common.GlobalParameters.ConsumoCuponAppInsertPath);
                            }
                            using (System.IO.StreamWriter mylogs = System.IO.File.AppendText(rutaCompleta))         //se crea el archivo
                            {
                                mylogs.WriteLine(query);

                                mylogs.Close();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //Insertar en el log y luego enviar correo.
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsCuponApp", "creaArhivoTemporalConsumoCuponApp", "Ha ocurrido una excepción al momento de generar archivo POS CONSUMOCUPONAPP - A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(e), "Stacktrace " + e.StackTrace);
                        //Enviar Correo:
                        var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                   Properties.Settings.Default.MAILERROR_FROM,
                                   Properties.Settings.Default.MAILERROR_ALIAS,
                                   Properties.Settings.Default.MAILERROR_DESTINO,
                                   Properties.Settings.Default.MAILERROR_CC,
                                   "Consumo de Cupon App no se generó archivo temporal para el ingreso",
                                   String.Format("  \n\nDatos caja-----------------" +
                                                   "\n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} \n{6}" +
                                                   "  \n\n-----------------" +
                                                 //datosVoucher,
                                                 string.Empty,
                                               Control.Common.GlobalParameters.Establecimiento,
                                               Control.Common.GlobalParameters.PuntoEmision,
                                               Control.Common.GlobalParameters.IpMaquina,
                                               Control.Common.GlobalParameters.UsuarioNombre,
                                               Control.Common.GlobalParameters.Usuario,
                                               Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                               (false ? "" : ("StackTrace: " + ex.StackTrace + " \n"))),
                                   false,
                                   String.Empty);

                        if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "ClsCuponApp", "creaArhivoTemporalConsumoCuponApp", "No se pudo enviar notificacion del problema al grabar Consumo Cupon App en archivo temporal, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                        }

                    }


                    return false;
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoints/ClsCuponApp", "realizarConsumoCuponApp", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío");
                return false;
            }
        }


    }

    public class Items
    {
        public string ItemId { get; set; }
    }


}
