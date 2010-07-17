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

            string authentication = ActionHandler.GetMessage(request);

            string user = authentication.Split(':')[0];
            string password = authentication.Split(':')[1];
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
                            answer = MessengerLib.Config.OKMessage;
                        }
                        else
                        {
                            //auditoria
                            Console.WriteLine(String.Format("Tentativa de logar com conta já online: {0}", user));

                            answer = "Essa conta já está online no sistema.";
                        }

                    }

                    break;

                case MessengerLib.Action.GetUsrs:

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
