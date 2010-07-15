using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessengerLib;
using System.Collections;
using WPFMessengerServer.Control;
using System.Collections.Generic;

namespace WPFMessengerServer
{
    public class Listener
    {

        private Encoding encoder;
        private const int channel = 1012;
        private TcpListener tcpListener;

        public Listener()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, channel);
            this.encoder = Encoding.GetEncoding("iso-8859-1");
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
                this.ProcessRequest(tcpClient,encoder.GetString(message, 0, qtdBytes));

            }

            tcpClient.Close();
        }

        private void ProcessRequest(TcpClient tcpClient, string request)
        {

            string authentication = ActionHandler.GetMessage(request);

            string user = authentication.Split(':')[0];
            string password = authentication.Split(':')[1];
            string answer = "OK";

            Control.Model.MSNUser msnUser = null;

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Action.Login:

                   msnUser = Util.GetUser(user, password);

                    if (msnUser == null)
                    {
                        answer = "Não foi possível entrar. Verifique seu ID e senha.";
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Usuário conectado: {0}", user));
                        Util.AddOnline(msnUser);
                    }

                    SendAnswer(tcpClient, answer);
                    break;

                case MessengerLib.Action.Logoff:

                    msnUser = Util.GetUser(user, password);
                    if (msnUser != null)
                    {
                        Console.WriteLine(String.Format("Usuário desconectado: {0}", user));
                        Util.ShutdownUser(msnUser);
                    }

                    break;

                case MessengerLib.Action.GetUsrs:

                    StringBuilder sb = new StringBuilder();

                    if (Util.GetUser(user, password) != null)
                    {
                        SendAnswer(tcpClient, Util.GetUsers());
                    }

                    break;
                case MessengerLib.Action.GetMsg:

                    break;
            }

        }

        private void SendAnswer(TcpClient tcpClient, string message)
        {

            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = encoder.GetBytes(message);

            //envia Resposta
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

    }
}
