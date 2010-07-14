using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using MessengerLib;

namespace AtividadesAula
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1012);
            client.Connect(serverEndPoint);

            NetworkStream clientStream = client.GetStream();

            ASCIIEncoding encoder = new ASCIIEncoding();

            MessengerLib.Encoder md5 = new MessengerLib.Encoder();
            string password = md5.GenerateMD5("senhaDoZeca");

            byte[] buffer = encoder.GetBytes(ActionHandler.FormatAction(MessengerLib.Action.UsrValidation, String.Format("zeca:{0}", password)));

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

        }
    }
}
