using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace POS.Control.Common
{
    public static class Mail
    {
        private static int _CodError;
        public static int CodError
        {
            get { return _CodError; }
            set { _CodError = value; }
        }

        private static string _MsgError;
        public static string MsgError
        {
            get { return _MsgError; }
            set { _MsgError = value; }
        }

        public static XmlDocument EnviaCorreo(string PI_UsrEnvia,
                                       string PI_AliasEnvia,
                                       string PI_UsrDestino,
                                       string PI_UsrCopia,
                                       string PI_Motivo,
                                       string PI_Mensaje,
                                       bool PI_EsFormatoHtml,                                       
                                       string PI_RutaLocalAttachment)
        {
            string PI_SmtpServidor  = Properties.Settings.Default.SMTP_SERVER;
            int PI_SmtpPuerto       = Properties.Settings.Default.SMTP_PORT;
            bool PI_SSL             = Properties.Settings.Default.SMTP_USESSL;
            string PI_Usuario       = Properties.Settings.Default.SMTP_USER;
            string PI_Clave         = Properties.Settings.Default.SMTP_PASS;

            //Arreglar correos destinos y CC a formato Net.Mailer (El separador esperado es coma <,>)
            PI_UsrDestino           = PI_UsrDestino.Replace(';',',');
            PI_UsrCopia             = PI_UsrCopia.Replace(';', ',');

            //reset variables de errores
            CodError = 0;
            MsgError = "";
            XmlDocument xmlResul = new XmlDocument();
            xmlResul.LoadXml("<Resul />");
            //Create a new blank MailMessage
            System.Net.Mail.MailMessage correo = new System.Net.Mail.MailMessage();
            try
            {
                if (String.IsNullOrEmpty(PI_AliasEnvia))
                    correo.From = new System.Net.Mail.MailAddress(PI_UsrEnvia);
                else
                    correo.From = new System.Net.Mail.MailAddress(PI_UsrEnvia, PI_AliasEnvia);
                //Si son varios correos de destino o de copia, separarlos con coma (,)
                correo.To.Add(PI_UsrDestino);
                if (!String.IsNullOrEmpty(PI_UsrCopia)) correo.CC.Add(PI_UsrCopia);
                correo.Subject = PI_Motivo;
                correo.Body = PI_Mensaje;
                correo.IsBodyHtml = PI_EsFormatoHtml;

                correo.BodyEncoding = System.Text.Encoding.Default;
                correo.Priority = System.Net.Mail.MailPriority.Normal;


                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = PI_SmtpServidor;
                if (PI_SmtpPuerto > 0) smtp.Port = PI_SmtpPuerto;

                smtp.UseDefaultCredentials = true;
                if ((PI_Usuario != string.Empty & PI_Clave != string.Empty))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(PI_Usuario, PI_Clave);
                }
                smtp.EnableSsl = PI_SSL;

                //Si el usuario envia una ruta de archivo para adjuntar
                if (!String.IsNullOrEmpty(PI_RutaLocalAttachment))
                {
                    correo.Attachments.Add(new System.Net.Mail.Attachment(PI_RutaLocalAttachment));
                }

                smtp.Send(correo);
                MsgError = "Correo electrónico enviado";
            }
            catch (Exception ex)
            {
                CodError = -100;
                MsgError = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
            }
            finally
            {
                xmlResul.DocumentElement.SetAttribute("CodError", CodError.ToString());
                xmlResul.DocumentElement.SetAttribute("MsgError", MsgError);
            }

            return xmlResul;

        }
    }
}
