using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace LirisLibCorrBan.Common
{
    public static class Logger
    {
        private static LirisLibLogger.Logger Log = new LirisLibLogger.Logger();

        #region Metodos Log LirisLibLogger

        public static void LogMessage(Common.Enum.LogTypes logType, string clase, string metodo, string msj, string infoAdicional)
        {
            var texto = string.Format("Clase: {0} |Método: {1} |Mensaje: {2} {3}",
                                                            clase,
                                                            metodo,
                                                            msj,
                                                            (string.IsNullOrEmpty(infoAdicional)) ? "" : "|" + infoAdicional);
            switch (logType)
            {
                case Enum.LogTypes.Debug:
                    Log.Graba_Log_Debug(texto);
                    break;
                case Enum.LogTypes.Info:
                    Log.Graba_Log_Info(texto);
                    break;
                case Enum.LogTypes.Warn:
                    Log.Graba_Log_Warn(texto);
                    break;
                case Enum.LogTypes.Error:
                    Log.Graba_Log_Error(texto);
                    break;
                case Enum.LogTypes.Fatal:
                    Log.Graba_Log_Fatal(texto);
                    break;
            }
        }

        #endregion
    }
}
