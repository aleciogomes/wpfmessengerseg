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

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Action.UsrValidation:

                    if (!Util.IsValid(user, password))
                    {
                        answer = "Não foi possível entrar. Verifique seu ID e senha.";
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Usuário conectado: {0}", user));
                    }

                    SendAnswer(tcpClient, answer);
                    break;

                case MessengerLib.Action.GetUsrs:

                    StringBuilder sb = new StringBuilder();

                    if (Util.IsValid(user, password))
                    {
                        IList<Control.Model.MSNUser> list = Util.GetUsers();

                        if (list.Count > 0)
                        {
                            
                            foreach (Control.Model.MSNUser msnUser in list)
                            {
                                sb.Append(String.Format("{0}:{1}:", msnUser.Id, msnUser.Name));
                            }
                        }
                    }

                    //fim da cadeia de caracteres
                    sb.Append("0::");

                    SendAnswer(tcpClient, sb.ToString());

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
