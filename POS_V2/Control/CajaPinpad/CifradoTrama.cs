using java.lang;
using java.security;
using javax.crypto;
using javax.crypto.spec;
using System;
using System.Security.Cryptography;
using System.Text;
using Exception = System.Exception;
using StringBuilder = System.Text.StringBuilder;

namespace POS.Control.CajaPinpad
{
    public class CifradoTrama
    {
        private static string LLAVE_IZQUIERDA = "EF12178E06711C05";
        private static string LLAVE_DERECHA = "BA0078E12733F411";

        public static string getHash()
        {
            string ramdomHex = CifradoTrama.GetRamdomHex(16);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ramdomHex);
            stringBuilder.Append(CifradoTrama.Encrypt3DES(CifradoTrama.LLAVE_IZQUIERDA, ramdomHex, CifradoTrama.LLAVE_DERECHA));
            return stringBuilder.ToString();
        }

        public static bool validateHash(string keyReceived)
        {
            bool flag = false;
            if (keyReceived != null)
            {
                try
                {
                    string ringhtKey = keyReceived.Substring(0, 16);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(keyReceived.Substring(0, 16));
                    stringBuilder.Append(CifradoTrama.Encrypt3DES(CifradoTrama.LLAVE_DERECHA, CifradoTrama.LLAVE_IZQUIERDA, ringhtKey));
                    if (keyReceived.Equals(stringBuilder.ToString()))
                        flag = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ArgumentNullException", (object)ex);
                }
            }
            return flag;
        }

        private static string Encrypt3DES(string dataDes, string leftKey, string ringhtKey)
        {
            Key key1 = (Key)new SecretKeySpec(ProcessData.Hex2Byte(leftKey), "DES");
            Key key2 = (Key)new SecretKeySpec(ProcessData.Hex2Byte(ringhtKey), "DES");
            return ProcessData.HexString(CifradoTrama.CifradoDES(CifradoTrama.DescifradoDES(CifradoTrama.CifradoDES(ProcessData.Hex2Byte(dataDes), key1), key2), key1));
        }

        public static byte[] CifradoDES(byte[] data, Key key) => CifradoTrama.des(data, key, 1);

        private static byte[] DescifradoDES(byte[] encryptedData, Key key)
        {
            return CifradoTrama.des(encryptedData, key, 2);
        }

        public static string GetRamdomHex(int digitos)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();
            for (int index = 0; index < digitos; ++index)
            {
                int num = random.Next(digitos);
                stringBuilder.Append(Integer.toHexString(num));
            }
            return stringBuilder.ToString().ToUpper();
        }

        public static byte[] des(byte[] data, Key key, int cipherMode)
        {
            byte[] numArray = (byte[])null;
            try
            {
                Cipher instance = Cipher.getInstance("DES/ECB/NoPadding");
                instance.init(cipherMode, key);
                numArray = instance.doFinal(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ArgumentNullException", (object)ex);
            }
            return numArray;
        }

        private string getRamdomHex(int digitos)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < digitos; ++index)
            {
                int num = new Random().Next(16);
                stringBuilder.Append(string.Concat((object)num));
            }
            return stringBuilder.ToString().ToUpper();
        }

        public static string convertirSHA256(string password)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = MD5.Create().ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("x2"));
            return stringBuilder.ToString();
        }
    }
}
