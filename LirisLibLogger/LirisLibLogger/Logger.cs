using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Data;

namespace LirisLibLogger
{
    public class Logger
    {
        public void Graba_Log_Warn(string PI_Texto)
        {
            try
            {
                if (!ConfigurationManager.AppSettings["LogInfo"].Equals("S"))
                    return;
                this.Graba_Log(PI_Texto, "WARN");
            }
            catch (Exception ex)
            {
            }
        }

        public void Graba_Log_Fatal(string PI_Texto)
        {
            try
            {
                if (!ConfigurationManager.AppSettings["LogError"].Equals("S"))
                    return;
                this.Graba_Log(PI_Texto, "FATAL");
            }
            catch (Exception ex)
            {
            }
        }

        public void Graba_Log_Debug(string PI_Texto, string PI_Identificador = "0")
        {
            try
            {
                if (!ConfigurationManager.AppSettings["LogInfo"].Equals("S"))
                    return;
                this.Graba_Log(PI_Texto, "DEBUG");
            }
            catch (Exception ex)
            {
            }
        }

        public void Graba_Log_Info(string PI_Texto)
        {
            try
            {
                if (!ConfigurationManager.AppSettings["LogInfo"].Equals("S"))
                    return;
                this.Graba_Log(PI_Texto, "INFO");
            }
            catch (Exception ex)
            {
            }
        }

        public void Graba_Log_Info(string PI_Texto, string PI_Identificador = "0")
        {
            try
            {
                if (!ConfigurationManager.AppSettings["LogInfo"].Equals("S"))
                    return;
                this.Graba_Log(PI_Texto, "INFO");
            }
            catch (Exception ex)
            {
            }
        }

        public void Graba_Log_Error(string PI_Texto, string PI_Identificador = "0")
        {
            try
            {
                if (!ConfigurationManager.AppSettings["LogError"].Equals("S"))
                    return;
                this.Graba_Log(PI_Texto, "ERROR");
            }
            catch (Exception ex)
            {
            }
        }

        public void Graba_Log(string Datos, string Tipo)
        {
            try
            {
                if (!ConfigurationManager.AppSettings["Auditar"].Equals("S"))
                    return;
                string appSetting = ConfigurationManager.AppSettings["ArchivoLog"];
                DateTime now = DateTime.Now;
                string newValue1 = now.ToString("dd");
                string str1 = appSetting.Replace("|dd", newValue1);
                now = DateTime.Now;
                string newValue2 = now.ToString("MM");
                string str2 = str1.Replace("|MM", newValue2);
                now = DateTime.Now;
                string newValue3 = now.ToString("yyyy");
                string str3 = str2.Replace("|yyyy", newValue3);
                now = DateTime.Now;
                string newValue4 = now.ToString("HH");
                string str4 = str3.Replace("|HH", newValue4);
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(str4));
                string path = Path.Combine(directoryInfo.FullName, str4);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();
                FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write);
                Trace.Listeners.Add((TraceListener)new TextWriterTraceListener((Stream)fileStream));
                string[] strArray = new string[5];
                now = DateTime.Now;
                strArray[0] = now.ToString("yyyy-MM-dd-HH:mm:ss:fff");
                strArray[1] = " ";
                strArray[2] = Tipo;
                strArray[3] = " ";
                strArray[4] = Datos.ToString();
                Trace.WriteLine(string.Concat(strArray));
                Trace.Flush();
                Trace.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
