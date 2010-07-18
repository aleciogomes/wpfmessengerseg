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

            string[] info = ActionHandler.GetMessage(request).Split(new char[] { ':' }, 3);

            string user = info[0];
            string password = info[1];
            string answer = String.Empty;

            MSNUser msnUser = Util.GetUser(user, password);

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Action.Login:

                    if (msnUser == null)
                    {
                        if (Util.GetContact(user) == null)
                        {
                            //auditoria
                            Util.RegEvent(user, String.Format("Login inválido", user));
                            Console.WriteLine(String.Format("Login inválido", user));
                        }
                        else
                        {
                            //auditoria
                            Util.RegEvent(user, "Senha inválida");
                            Console.WriteLine("Senha inválida");
                        }

                        answer = "Não foi possível entrar. Verifique seu usuário e senha.";
                    }
                    else
                    {
                        if (msnUser.Blocked)
                        {
                            //auditoria
                            Util.RegEvent(user, "Tentativa de logar com conta bloqueada");
                            Console.WriteLine(String.Format("Tentativa de logar com conta bloqueada: {0}", user));

                            answer = "Conta bloqueada pelo administrador do sistema.";
                        }
                        else if (msnUser.Expiration != null && DateTime.Now >= msnUser.Expiration)
                        {
                            //auditoria
                            Util.RegEvent(user, "Tentativa de logar com conta expirada");
                            Console.WriteLine(String.Format("Tentativa de logar com conta expirada: {0}", user));

                            answer = "Conta expirada";
                        }
                        else if (!Util.IsOnline(msnUser.Login))
                        {
                            //auditoria
                            Util.RegEvent(user, "Login efetuado");
                            Console.WriteLine(String.Format("Login realizado: {0}", user));

                            Util.AddOnline(msnUser);
                            answer = String.Format("{0}:{1}:{2}:{3}", MessengerLib.Config.OKMessage, msnUser.Name, msnUser.ExpirationString(false), msnUser.TimeAlert);
                        }
                        else
                        {
                            //auditoria
                            Util.RegEvent(user, "Tentativa de logar com conta já conectada");
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
                        Util.RegEvent(user, "Tentativa de buscar lista de usuários sem login do sistema");
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
                        Util.RegEvent(user, "Tentativa de buscar mensagens sem login do sistema");
                        answer = MessengerLib.Config.EndStackMessage;
                    }

                    break;
                case MessengerLib.Action.GetUserInfo:

                    string user2 = info[2];

                    if (msnUser != null)
                    {
                        answer = Util.GetUserInfo(user2);
                    }
                    else
                    {
                        Util.RegEvent(user, "Tentativa de buscar informações de usuário sem login do sistema");
                    }

                    break;

                case MessengerLib.Action.GetFeatures:

                    int userID = int.Parse(info[2]);

                    if (msnUser != null)
                    {
                        answer = Util.GetFeatures(userID);
                    }
                    else
                    {
                        Util.RegEvent(user, String.Format("Tentativa de buscar permissões do usuário {0} sem login do sistema", userID));
                    }

                    break;

                case MessengerLib.Action.GetLog:

                    if (msnUser != null)
                    {
                        DateTime logDate = DateTime.Parse(info[2]);

                        answer = Util.GetLog(logDate);
                    }
                    else
                    {
                        Util.RegEvent(user, "Tentativa de buscar log sem login do sistema");
                    }

                    break;

                case MessengerLib.Action.GetLogDates:

                    if (msnUser != null)
                    {
                        answer = Util.GetLogDates();
                    }
                    else
                    {
                        Util.RegEvent(user, "Tentativa de buscar datas dos logs sem login do sistema");
                    }

                    break;

                case MessengerLib.Action.UserAvailable:

                    string newUser = info[2];

                     if (msnUser != null)
                     {
                        
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
                    }
                    else
                    {
                        Util.RegEvent(user, String.Format("Tentativa de verificar se o usuário {0} está disponível sem login do sistema", newUser));
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
