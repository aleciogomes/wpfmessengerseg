using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Atividade01Cliente
{
    public class Client
    {

        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1012);
            client.Connect(serverEndPoint);

            NetworkStream clientStream = client.GetStream();

            ASCIIEncoding encoder = new ASCIIEncoding();

            string password = GenerateMD5("senhaForte123@!@#$");
            string user = "usuario01";

            byte[] buffer = encoder.GetBytes(String.Format("{0}:{1}", user, password));

            //envia usuário e senha para validação no servidor
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            //resposta
            byte[] bb = new byte[1000];
            int index = clientStream.Read(bb, 0, 1000);
            StringBuilder message = new StringBuilder();

            for (int i = 0; i < index; i++)
            {
                message.Append(Convert.ToChar(bb[i]).ToString());
            }

            Console.WriteLine(message.ToString());
            Console.ReadKey();
        }

        static string GenerateMD5(string input)
        {
            //calcula o MD5 hash a partir da string
            MD5 md5 = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(bytes);

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
