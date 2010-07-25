using System;
using System.Security.Cryptography;
using System.Text;

namespace MessengerLib.Util
{
    public static class Encoder
    {
        private static Encoding encoderISO = Encoding.GetEncoding("iso-8859-1");

        private const string CRYPT_KEY = "wpfKey@#Messeng*";

        public static string EncryptMessage(string message)
        {
            return MasterCrypt(message, true);
        }

        public static string DecryptMessage(string message)
        {
            return MasterCrypt(message, false);
        }

        private static string MasterCrypt(string message, bool encrypt)
        {
            byte[] keyArray, toEncryptArray, resultArray;

            keyArray = UTF8Encoding.UTF8.GetBytes(CRYPT_KEY);

            //3DES
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform;

            if (encrypt)
            {
                toEncryptArray = UTF8Encoding.UTF8.GetBytes(message);
                cTransform = tdes.CreateEncryptor();
            }
            else
            {
                toEncryptArray = Convert.FromBase64String(message);
                cTransform = tdes.CreateDecryptor();
            }

            resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            if (encrypt)
            {
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            else
            {
                return UTF8Encoding.UTF8.GetString(resultArray);
            }

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

        public static string Sign(string contentToSign, string xmlPrivateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);

            byte[] signData = rsa.SignData(encoderISO.GetBytes(contentToSign), CryptoConfig.MapNameToOID("SHA1"));

            return Convert.ToBase64String(signData);
        }

        public static bool Verify(string originalContent, string verifyContent, string xmlPublicKey)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(xmlPublicKey);

            return RSA.VerifyData(encoderISO.GetBytes(originalContent), CryptoConfig.MapNameToOID("SHA1"), Convert.FromBase64String(verifyContent));
        }
    }
}
