using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.Common
{
    public class RawPrinterHelper
    {
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinterName, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [StructLayout(LayoutKind.Sequential)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string DocName;
            [MarshalAs(UnmanagedType.LPStr)] public string OutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string DataType;
        }

        public static void SendStringToPrinter(string printerName, string content)
        {
            IntPtr pBytes = IntPtr.Zero;
            IntPtr printer = IntPtr.Zero;
            int bytesWritten = 0;
            var di = new DOCINFOA();
            di.DocName = "RAW Document";
            di.DataType = "RAW";

            try
            {
                pBytes = Marshal.StringToCoTaskMemAnsi(content);
                if (!OpenPrinter(printerName.Normalize(), out printer, IntPtr.Zero))
                    throw new Exception("No se pudo abrir la impresora");

                if (!StartDocPrinter(printer, 1, di))
                    throw new Exception("No se pudo iniciar el documento");

                if (!StartPagePrinter(printer))
                    throw new Exception("No se pudo iniciar la página");

                if (!WritePrinter(printer, pBytes, content.Length, out bytesWritten))
                    throw new Exception("Error al escribir en la impresora");

                EndPagePrinter(printer);
                EndDocPrinter(printer);
                ClosePrinter(printer);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pBytes);
            }
        }
    }
}
