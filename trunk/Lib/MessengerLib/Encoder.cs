using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MessengerLib
{
    public static class Encoder
    {

        private static Encoding encoderISO = Encoding.GetEncoding("iso-8859-1");

        private static byte[] KeyAndIV = ASCIIEncoding.ASCII.GetBytes("wpfKey@#");

        public static string EncryptMessage(string message)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(KeyAndIV, KeyAndIV), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(message);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public static string DecryptMessage(string message)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(message));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(KeyAndIV, KeyAndIV), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

        public static Encoding GetEncoding()
        {
            return encoderISO;
        }

        public static string GenerateMD5(string input)
        {
            //calcula o MD5 hash a partir da string
            MD5 md5      = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            byte[] hash  = md5.ComputeHash(bytes);

            //converte o array de bytes em uma string haxadecimal
            StringBuilder finalString = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                finalString.Append(hash[i].ToString("X2"));
            }

            return finalString.ToString();
        }

    }
}
