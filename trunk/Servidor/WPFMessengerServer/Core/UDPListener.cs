using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessengerLib;
using System;
using WPFMessengerServer.Control.Model;

namespace WPFMessengerServer.Core
{
    public class UDPListener
    {
        private const int channel = 1011;
        private UdpClient udpListener;

        public UDPListener()
        {
            this.udpListener = new UdpClient(channel);
        }

        public void Listen()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = null;
            string dataString = "";

            while (true)
            {
                data = udpListener.Receive(ref iep);
                dataString = Encoding.ASCII.GetString(data, 0, data.Length);

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(dataString);
            }
        }

        private void HandleClient(object data)
        {
            string request = data.ToString();

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Action.SendMsg:

                    request = ActionHandler.GetMessage(request);

                    string [] info = request.Split(':');

                    string user     = info[0];
                    string password = info[1];
                    string destiny  = info[2];
                    string msg      = info[3];

                    MSNUser msnForwarder = Util.GetUser(user, password);

                    if (msnForwarder != null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Mensagem enviada de {0} para {1}", user, destiny));

                        MSNUser msnDestiny = Util.GetContact(destiny);
                        Util.AddMessage(msnForwarder, msnDestiny, msg);
                    }

                    break;
            }
        }

    }
}
