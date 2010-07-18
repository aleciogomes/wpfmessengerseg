using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MessengerLib;
using WPFMessengerServer.Control.Model;

namespace WPFMessengerServer
{
    public class TCPListener
    {

        private const int channel = 1012;
        private TcpListener tcpListener;

        public TCPListener()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, channel);
        }

        public void Listen()
        {
            this.tcpListener.Start();

            while (true)
            {
               //aguarda a conexão do cliente
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //trata conexão com cliente
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }

        private void HandleClient(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[1000];
            int qtdBytes;

            while (true)
            {
                qtdBytes = 0;

                try
                {
                    //aguarda receber a mensagem
                    qtdBytes = clientStream.Read(message, 0, 1000);
                }
                catch
                {
                    break;
                }

                if (qtdBytes == 0)
                {
                    //cliente disconectado
                    break;
                }

                //mensagem recebida
                this.ProcessRequest(tcpClient, MessengerLib.Encoder.GetEncoding().GetString(message, 0, qtdBytes));

            }

            tcpClient.Close();
        }

        private void ProcessRequest(TcpClient tcpClient, string request)
        {

            string[] info = ActionHandler.GetMessage(request).Split(':');

            string user = info[0];
            string password = info[1];
            string answer = String.Empty;

            MSNUser msnUser = Util.GetUser(user, password);

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Action.Login:

                    if (msnUser == null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Login inválido: {0}/{1}", user, password));

                        answer = "Não foi possível entrar. Verifique seu usuário e senha.";
                    }
                    else
                    {
                        if (!Util.IsOnline(msnUser.Login))
                        {
                            //auditoria
                            Console.WriteLine(String.Format("Usuário conectado: {0}", user));

                            Util.AddOnline(msnUser);
                            answer = String.Format("{0}:{1}", MessengerLib.Config.OKMessage, msnUser.Expiration);
                        }
                        else
                        {
                            //auditoria
                            Console.WriteLine(String.Format("Tentativa de logar com conta já online: {0}", user));

                            answer = "Essa conta já está online no sistema.";
                        }

                    }

                    break;

                case MessengerLib.Action.GetUsers:

                    if (msnUser != null)
                    {
                        answer = Util.GetUsers();
                    }
                    else
                    {
                        answer = MessengerLib.Config.EndStackMessage;
                    }

                    break;
                case MessengerLib.Action.GetMsg:

                    if (msnUser != null)
                    {
                        answer = Util.GetMessages(user);
                    }
                    else
                    {
                        answer = MessengerLib.Config.EndStackMessage;
                    }

                    break;
                case MessengerLib.Action.GetUserInfo:

                    string user2 = info[2];

                    if (msnUser != null)
                    {
                        answer = Util.GetUserInfo(user2);
                    }

                    break;

                case MessengerLib.Action.GetFeatures:

                    int userID = int.Parse(info[2]);

                    if (msnUser != null)
                    {
                        answer = Util.GetFeatures(userID);
                    }

                    break;

                case MessengerLib.Action.UserAvailable:

                    string newUser = info[2];

                    if (Util.GetContact(newUser) != null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Tentativa de cadastrar um usuário que já existe: {0}", newUser));

                        answer = "Usuário já existente na base. Escolha outro login.";
                    }
                    else
                    {
                        answer = MessengerLib.Config.OKMessage;
                    }

                    break;

                default:
                    answer = MessengerLib.Config.ErrorMessage;
                    break;
            }


            SendAnswer(tcpClient, answer);

        }

        private void SendAnswer(TcpClient tcpClient, string message)
        {

            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = MessengerLib.Encoder.GetEncoding().GetBytes(message);

            //envia Resposta
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

    }
}
