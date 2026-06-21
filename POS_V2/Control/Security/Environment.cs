using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Models;
using System.Data.SqlClient;

namespace POS.Control.Security
{
    public class Environment
    {
        public void CheckIpRunPermission()
        {
            bool runPerm = Control.Common.GlobalParameters.EsAmbienteProduccion;

            runPerm = runPerm ? true : Common.GlobalParameters.ListaIpPermitidasAmbienteDev.Any(x => x.Trim() == Control.Common.GlobalParameters.IpMaquina);

            if (!runPerm)
            {
                System.Windows.Forms.MessageBox.Show("Caja no tiene permisos para operar en este ambiente. Solicite que se le actualice la configuracion de servidor y vuelva a abrir POS");
                Control.Common.GlobalParameters.MustCloseApplication = true;
                System.Windows.Forms.Application.Exit();
            }
        }

        public bool HasIpRunPermission()
        {
            bool runPerm = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var paramAmbienteProduccion = db.core_parametro.Where(x => x.identificador.Equals("AMBIENTEPRODUCCION")).FirstOrDefault();

                    if (paramAmbienteProduccion == null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Security.Environment", "HasIpRunPermission", "No está configurado el parametro AMBIENTEPRODUCCION configurado en la tabla core_parametro");
                        //Si una base no tiene configurado el parametro, la consideraremos por defecto PRODUCCION
                        runPerm = true;
                    }
                    else
                    {
                        runPerm = paramAmbienteProduccion.valor == "TRUE" ? true : false;
                        if (!runPerm)
                        {
                            var listaIpPermitidasAmbienteDev = (string.IsNullOrWhiteSpace(paramAmbienteProduccion.parametro2) ? string.Empty : paramAmbienteProduccion.parametro2).Split(';').ToList();
                            if (listaIpPermitidasAmbienteDev.Any(x => x.Contains(Control.Common.Network.GetMyIpAddress())))
                            {
                                runPerm = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Environment", "HasIpRunPermission", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                runPerm = true;
            }
            return runPerm;
        }

        public void CheckHasLatestVersion()
        {
            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    Version ver = null;
                    System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                    ver = ad.CurrentVersion;
                    var revision = ver.Revision;

                    using (POSEntities db = new POSEntities())
                    {
                        var paramLirisPOSVersion = db.core_parametro.Where(x => x.identificador == "VERSION_ACTUAL").FirstOrDefault();

                        if (paramLirisPOSVersion != null)
                        {
                            if (revision != int.Parse(paramLirisPOSVersion.valor))   //JCanarte 20Abril2021 Se reemplazó el < por !=
                            {
                                var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                                    Properties.Settings.Default.MAILERROR_FROM,
                                    Properties.Settings.Default.MAILERROR_ALIAS,
                                    string.IsNullOrWhiteSpace(paramLirisPOSVersion.parametro2) ? "sistemas@liris.com.ec" : paramLirisPOSVersion.parametro2,
                                    Properties.Settings.Default.MAILERROR_CC,
                                    "POS Version desactualizada",
                                    String.Format("La siguiente caja está trabajando con una versión de POS desactualizada, se recomienda regularizar inmediatamente \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nIpServidor: {3} \nCajeroNombre: {4} \nCajeroIdentificacion: {5} \nVersionDelportalActual: {6} \nVersionCaja: {7}",
                                                Control.Common.GlobalParameters.Establecimiento,
                                                Control.Common.GlobalParameters.PuntoEmision,
                                                Control.Common.GlobalParameters.IpMaquina,
                                                Control.Common.GlobalParameters.SelectedServerIp,
                                                Control.Common.GlobalParameters.UsuarioNombre,
                                                Control.Common.GlobalParameters.Usuario,
                                                paramLirisPOSVersion.valor,
                                                revision.ToString()),
                                    false,
                                    String.Empty);

                                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Security.Environment", "CheckHasLatestVersion", "No se pudo enviar notificacion de caja desactualizada, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                                }

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Security.Environment", "CheckHasLatestVersion", "Esta caja está trabajando con una versión de POS desactualizada, se recomienda regularizar inmediatamente -> VersionCaja: " + revision.ToString() + " VersionActual: " + paramLirisPOSVersion.valor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Environment", "CheckHasLatestVersion", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }
        }
        
        public void CheckCompletedChecklists()
        {
            try
            {
                //Solo realizar la validacion si se está en ambiente de produccion y usuario no es superusuario
                if (Control.Common.GlobalParameters.EsAmbienteProduccion && !Control.Common.GlobalParameters.UserObj.isSuperUser)
                {
                    //Por defecto se considerara que no hay checklists pendientes
                    int hasPendingChecklists = 0;

                    bool mustVerifyChecklists = false;

                    using (POSEntities db = new POSEntities())
                    {
                        var param = db.core_parametro.Where(x => x.identificador == "BLOQUEARAPERTCIERRE").FirstOrDefault();
                        if (param != null)
                        {
                            string value = string.IsNullOrWhiteSpace(param.valor) ? "" : param.valor.Trim();
                            if (value != "0" && value != "1")
                            {
                                throw new Exception("El parametro BLOQUEARAPERTCIERRE no tiene el campo valor bien configurado en core_parametro. Valor debe ser 0 para Falso ó 1 para Verdadero");
                            }
                            mustVerifyChecklists = int.Parse(value) == 1 ? true : false;
                        }
                        else
                        {
                            throw new Exception("No existe configurado parametro BLOQUEARAPERTCIERRE en core_parametro");
                        }
                    }

                    if (mustVerifyChecklists)
                    {
                        SqlConnection conexion = new SqlConnection(Properties.Settings.Default.CONECTA_AX);
                        string Query = null;
                        SqlCommand comando = default(SqlCommand);
                        using (conexion)
                        {
                            DateTime fecha = DateTime.Now.AddDays(-1);
                            string dateTimeQuery = fecha.ToString("yyyyMMdd");
                            conexion.Open();
                            Query = "declare @hasPendingChecklists int " +
                                    "set @hasPendingChecklists = 0 " +
                                    "select top 1 @hasPendingChecklists = 1 " +
                                    "from [SRV-AX].DynamicsAx1.dbo.TblChecklistDetails det with(nolock) inner join " +
                                    "[SRV-AX].DynamicsAx1.dbo.TblChecklistLocales cab with(nolock) on cab.RecId = det.RefRecId " +
                                    "where cab.Almacen = '" + Common.GlobalParameters.EstablecimientoAxCode + "' " +
                                    "and cab.Fecha = '" + dateTimeQuery + "' and det.Completado = 0 " +
                                    "select @hasPendingChecklists   ";

                            comando = new SqlCommand(Query, conexion);
                            SqlDataReader dr = comando.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                //Si recordset trae valor 1 (uno) local tiene checklists pendientes, si trae 0 (cero) todo esta completo
                                hasPendingChecklists = int.Parse(dr.GetValue(0).ToString());
                            }
                            conexion.Close();
                        }

                        if (hasPendingChecklists == 1)
                        {
                            //System.Windows.Forms.MessageBox.Show("El local tiene checklists pendientes. Solicite que se regularice en administración y vuelva a abrir POS");
                            Common.General.GetMensajeToList(701);

                            Control.Common.GlobalParameters.MustCloseApplication = true;
                            System.Windows.Forms.Application.Exit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Environment", "CheckCompletedChecklists", Control.Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }
        }
    }
}
