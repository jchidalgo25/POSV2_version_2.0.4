using System;
using System.Security.Cryptography;
using System.Text;


namespace POS.Control.Common
{

    class AesEncryption
    {
        // Tamaño en bytes de la clave AES-256
        private const int KEY_SIZE_BYTES = 32;
        // Número de iteraciones para PBKDF2
        private const int ITERATIONS = 100000;
        
        public static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public static byte[] Base64UrlDecode(string input)
        {
            string padded = input.PadRight(input.Length + (4 - input.Length % 4) % 4, '=')
                .Replace('-', '+')
                .Replace('_', '/');
            return Convert.FromBase64String(padded);
        }

        /// <summary>
        /// Deriva una clave segura usando PBKDF2 con SHA-512
        /// </summary>
        //public static byte[] DeriveKey(string password, byte[] salt)
        //{
        //    const int iterations = 100000;

        //    using (var derive = new Rfc2898DeriveBytes(password, salt, iterations))
        //    {
        //        // En .NET Framework, el algoritmo HMACSHA1 es usado por defecto
        //        // Para usar SHA-512, debemos cambiar manualmente el hash:
        //        derive.HashName = "SHA512";

        //        return derive.GetBytes(KEY_SIZE_BYTES);
        //    }
        //}

        public static byte[] DeriveKey(string password, byte[] salt)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(password)))
            {
                return Pbkdf2(hmac, salt, ITERATIONS, KEY_SIZE_BYTES);
            }
        }


        private static byte[] Pbkdf2(HMAC hmac, byte[] salt, int iterations, int outputLength)
        {
            uint blockIndex = 1;

            int hashSize = hmac.HashSize / 8;
            byte[] saltPlusBlock = new byte[salt.Length + 4];
            Buffer.BlockCopy(salt, 0, saltPlusBlock, 0, salt.Length);

            byte[] result = new byte[outputLength];
            int resultOffset = 0;

            int remaining = outputLength;

            while (remaining > 0)
            {
                int currentBlockSize = Math.Min(remaining, hashSize);

                // Salt + block index
                saltPlusBlock[salt.Length] = (byte)(blockIndex >> 24);
                saltPlusBlock[salt.Length + 1] = (byte)(blockIndex >> 16);
                saltPlusBlock[salt.Length + 2] = (byte)(blockIndex >> 8);
                saltPlusBlock[salt.Length + 3] = (byte)(blockIndex);

                byte[] t = Pbkdf2F(hmac, saltPlusBlock, iterations);

                Buffer.BlockCopy(t, 0, result, resultOffset, currentBlockSize);

                remaining -= currentBlockSize;
                resultOffset += currentBlockSize;
                blockIndex++;
            }

            return result;
        }

        private static byte[] Pbkdf2F(HMAC hmac, byte[] saltPlusBlock, int iterations)
        {
            byte[] intput = saltPlusBlock;
            byte[] output = new byte[hmac.HashSize / 8];

            for (int i = 0; i < iterations; i++)
            {
                intput = hmac.ComputeHash(intput);
                for (int x = 0; x < output.Length; x++)
                {
                    output[x] ^= intput[x];
                }
            }

            return output;
        }


        // Cifrar texto plano
        public static EncryptedData Encrypt(string plainText, string password)
        {
            byte[] salt = RandomBytes(16);
            byte[] iv = RandomBytes(16);

            byte[] key = DeriveKey(password, salt);

            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor();
                encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
            }

            return new EncryptedData
            {
                Salt = Base64UrlEncode(salt),
                IV = Base64UrlEncode(iv),
                CipherText = Base64UrlEncode(encrypted)
            };
        }

        // Descifrar datos
        public static string Decrypt(EncryptedData data, string password)
        {
            byte[] salt = Base64UrlDecode(data.Salt);
            byte[] iv = Base64UrlDecode(data.IV);
            byte[] cipherText = Base64UrlDecode(data.CipherText);

            byte[] key = DeriveKey(password, salt);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor();
                byte[] decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

                return Encoding.UTF8.GetString(decrypted).TrimEnd('\0');
            }
        }

        // Generador de bytes aleatorios
        private static byte[] RandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes); // Rellena los bytes aleatorios
            }
            return randomBytes;
        }

        // Estructura para almacenar los datos cifrados
        public class EncryptedData
        {
            public string Salt { get; set; }
            public string IV { get; set; }
            public string CipherText { get; set; }
        }

      

    }
}
