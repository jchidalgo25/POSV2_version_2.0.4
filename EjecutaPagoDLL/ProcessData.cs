using java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exception = System.Exception;
using StringBuilder = System.Text.StringBuilder;

namespace EjecutaPagoDLL
{
    public class ProcessData
    {
        public static ProcessData instance;
        private static Random random = new Random();

        public static string[] hexStrings()
        {
            string[] strArray = new string[256];
            for (int index = 0; index < 256; ++index)
            {
                StringBuilder stringBuilder = new StringBuilder(2);
                char ch1 = Character.forDigit((int)(byte)index >> 4 & 15, 16);
                stringBuilder.Append(Character.toUpperCase(ch1));
                char ch2 = Character.forDigit((int)(byte)index & 15, 16);
                stringBuilder.Append(Character.toUpperCase(ch2));
                strArray[index] = stringBuilder.ToString();
            }
            return strArray;
        }

        public static ProcessData getInstance()
        {
            if (ProcessData.instance == null)
                ProcessData.instance = new ProcessData();
            return ProcessData.instance;
        }

        public static byte[] getFinalData(string[] values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (values == null || values.Length == 0)
                return (byte[])null;
            for (int index = 0; index < values.Length; ++index)
                stringBuilder.Append(values[index]);
            return Encoding.UTF8.GetBytes(ProcessData.padleft(Integer.toHexString(stringBuilder.Length), 4, '0') + stringBuilder.ToString());
        }

        public string ConvertStringToHex(string str)
        {
            char[] charArray = str.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < charArray.Length; ++index)
                stringBuilder.Append(charArray[index]);
            return stringBuilder.ToString();
        }

        public static string HexString(byte[] b)
        {
            StringBuilder stringBuilder = new StringBuilder(b.Length * 2);
            foreach (byte num in b)
                stringBuilder.Append(ProcessData.hexStrings()[(int)num & (int)byte.MaxValue]);
            return stringBuilder.ToString();
        }

        public static string padleft(string s, int len, char c)
        {
            if (s == null)
                s = "";
            s = s.Trim();
            if (s.Length > len)
                return (string)null;
            StringBuilder stringBuilder = new StringBuilder(len);
            int num = len - s.Length;
            while (num-- > 0)
                stringBuilder.Append(c);
            stringBuilder.Append(s);
            return stringBuilder.ToString();
        }

        public static string padright(string s, int len, char c)
        {
            s = s.Trim();
            StringBuilder stringBuilder = new StringBuilder(len);
            int num = len - s.Length;
            stringBuilder.Append(s);
            while (num-- > 0)
                stringBuilder.Append(c);
            return stringBuilder.ToString();
        }

        public static string byte2hex(byte[] bs) => ProcessData.Byte2Hex(bs, 0, bs.Length);

        public static string Byte2Hex(byte[] bs, int off, int length)
        {
            if (bs.Length <= off || bs.Length < off + length)
                throw new ArgumentException();
            StringBuilder sb = new StringBuilder(length * 2);
            ProcessData.byte2HexAppend(bs, off, length, sb);
            return sb.ToString().ToUpper();
        }

        private static void byte2HexAppend(byte[] bs, int off, int length, StringBuilder sb)
        {
            if (bs.Length <= off || bs.Length < off + length)
                throw new ArgumentException();
            sb.EnsureCapacity(sb.Length + length * 2);
            for (int index = off; index < off + length; ++index)
            {
                sb.Append(Convert.ToString((int)bs[index] >> 4 & 15, 16));
                sb.Append(Convert.ToString((int)bs[index] & 15, 16));
            }
        }

        public static string hex2AsciiStr(string hexString)
        {
            try
            {
                string empty1 = string.Empty;
                for (int startIndex = 0; startIndex < hexString.Length; startIndex += 2)
                {
                    string empty2 = string.Empty;
                    char ch = Convert.ToChar(Convert.ToUInt32(hexString.Substring(startIndex, 2), 16));
                    empty1 += ch.ToString();
                }
                return empty1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public static byte[] Hex2Byte(string formatterContext)
        {
            int length1 = formatterContext.Length;
            int length2 = length1 / 2;
            byte[] numArray = new byte[length2];
            for (int index = 1; index <= length2; ++index)
                numArray[length2 - index] = Convert.ToByte(Convert.ToInt32(formatterContext.Substring(length1 - index * 2, 2), 16));
            return numArray;
        }

        public static string GetRandomHexNumber(int digits)
        {
            byte[] numArray = new byte[digits / 2];
            ProcessData.random.NextBytes(numArray);
            string str = string.Concat(((IEnumerable<byte>)numArray).Select<byte, string>((Func<byte, string>)(x => x.ToString("X2"))).ToArray<string>());
            return digits % 2 == 0 ? str : str + ProcessData.random.Next(16).ToString("X");
        }

        public static byte[] Str2bcd(string s, bool padleft)
        {
            if (s == null)
                return (byte[])null;
            byte[] d = new byte[s.Length + 1 >> 1];
            return ProcessData.str2bcd(s, padleft, d, 0);
        }

        public static byte[] str2bcd(string s, bool padLeft, byte[] d, int offset)
        {
            int length = s.Length;
            int num1 = (length & 1) == 1 & padLeft ? 1 : 0;
            int num2 = num1;
            while (num2 < length + num1)
                ++num2;
            return d;
        }
    }
}
