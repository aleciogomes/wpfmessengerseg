using System.Security.Cryptography;
using System.Text;

namespace MessengerLib
{
    public static class Encoder
    {
        /*
        private const string keyMD5 = "wpfMSNKey";

        public string Encrypt()
        {

        }

        public string Decrypt()
        {

        }
        */

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
