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
                dataString = MessengerLib.Encoder.GetEncoding().GetString(data, 0, data.Length);

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(dataString);
            }
        }

        private void HandleClient(object data)
        {
            string request  = data.ToString(),
                   user     = String.Empty,
                   password = String.Empty;

            string[] info = ActionHandler.GetMessage(request).Split(':');
            user = info[0];
            password = info[1];

            MSNUser msnUser = Util.GetUser(user, password);

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Action.SendMsg:


                    string destiny  = info[2];

                    //pode conter emoticons na msg
                    string msg = String.Join(":", info, 3, info.Length - 3);

                    if (msnUser != null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Mensagem enviada de {0} para {1}", user, destiny));

                        if (Util.GetContact(destiny) != null)
                        {
                            Util.AddMessage(msnUser, destiny, msg);
                        }
                    }

                    break;

                case MessengerLib.Action.Logoff:

                    if (msnUser != null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Usuário desconectado: {0}", user));

                        Util.ShutdownUser(msnUser.Login);
                    }

                    break;

                case MessengerLib.Action.UpdateAccount:

                    string newName = info[2],
                           newUser = info[3],
                           newPassword = info[4];

                    if (msnUser != null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Alterando dados da conta: {0}", user));

                        Util.UpdateAccount(user, newName, newUser, newPassword);
                    }


                    break;
            }
        }

    }
}
