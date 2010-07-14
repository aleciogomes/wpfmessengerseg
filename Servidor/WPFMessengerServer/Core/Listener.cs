using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using WPFMessengerServer.Authentication;
using MessengerLib;

namespace WPFMessengerServer
{
    public class Listener
    {

        private TcpListener tcpListener;
        private UserIdentity identity;

        public Listener()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 1012);
            this.identity = new UserIdentity();
        }

        public void ListenForClients()
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

            byte[] message = new byte[4096];
            int qtdBytes;

            while (true)
            {
                qtdBytes = 0;

                try
                {
                    //aguarda receber a mensagem
                    qtdBytes = clientStream.Read(message, 0, 4096);
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
                ASCIIEncoding encoder = new ASCIIEncoding();

                string input = encoder.GetString(message, 0, qtdBytes);

                switch (ActionHandler.GetAction(input))
                {
                    case MessengerLib.Action.UsrValidation:

                        input = ActionHandler.GetMessage(input);
                        string user = input.Split(':')[0];
                        string password = input.Split(':')[1];

                        if (this.identity.IsValid(user, password))
                        {
                            Console.WriteLine("Usuários e senha validados!");
                        }
                        else
                        {
                            Console.WriteLine("Usuários e/ou senha invalido(s)!");
                        }
                        break;
                }

                /*
                NetworkStream stream = tcpClient.GetStream();
                byte[] buffer = encoder.GetBytes("Hello Client!");

                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                */
            }

            tcpClient.Close();
        }

    }
}
