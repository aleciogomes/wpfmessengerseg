using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WPFMessengerSeg.Core
{

    public delegate bool TCPConnectionCaller();

    public static class TCPConnection
    {

        private static string authentication = String.Empty;

        public static IList<MSNUser> GetListUsers()
        {

            IList<MSNUser> list = new List<MSNUser>();
            MSNUser user = null;

            string returnString = GetUsers();

            if (ValidetReturn(returnString) && !returnString.Equals(MessengerLib.Config.EndStackMessage))
            {
                string[] returnVector = returnString.Split(':');
                string value = null;

                //conta quantos atributos já fora adicionados a um usuário
                int countAttributes = 0;

                for (int i = 0; i < returnVector.Length; i++)
                {
                    value = returnVector[i].ToString();

                    if (!String.IsNullOrEmpty(value))
                    {

                        if (countAttributes == 0)
                        {
                            user = new MSNUser();
                            user.UserLogin = value;
                            countAttributes++;
                        }
                        else if (countAttributes == 1)
                        {
                            user.UserName = value;
                            countAttributes++;
                        }
                        else
                        {
                            //reinicia
                            countAttributes = 0;
                            user.Online = bool.Parse(value);
                            list.Add(user);
                        }
                    }
                }
            }

            /*
            user = new MSNUser();
            user.UserID = 45;
            user.UserName = "Teste";
            list.Add(user);
            */

            return list;

        }

        public static string Login()
        {
            authentication = String.Empty;
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.Login, GetAuthentication());
            string result = EstabilishConnection(cmd, false);

            string[] info = result.Split(':');

            if(info.Length > 1){

                try
                {
                    MSNSession.User.Expiration = DateTime.Parse(info[1]);
                }
                catch
                {
                    MSNSession.User.Expiration = null;
                }

                IList<MSNUser> lista = GetListUsers();
                foreach (MSNUser user in lista)
                {
                    if (user.UserLogin == MSNSession.User.UserLogin)
                    {
                        MSNSession.User.UserName = user.UserName;
                    }
                }
            }

            return info[0];
        }

        public static string GetUserAvailable(string newUser)
        {
            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), newUser);
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.UserAvailable, info);

            return EstabilishConnection(cmd, false);
        }

        private static bool ValidetReturn(string returnString)
        {
            if (!String.IsNullOrEmpty(returnString) && returnString.IndexOf(MessengerLib.Config.ErrorMessage) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static String GetUsers()
        {
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.GetUsrs, GetAuthentication());
            return EstabilishConnection(cmd, true);
        }

        public static IList<MSNMessage> GetMyMessages()
        {
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.GetMsg, GetAuthentication());
            string messageString = EstabilishConnection(cmd, false);

            string value = null;

            IList<MSNMessage> lista = new List<MSNMessage>();
            MSNMessage message = null;

            while (!messageString.Equals(MessengerLib.Config.EndStackMessage))
            {
                string[] returnVector = messageString.Split(new char[] { ':' }, 2);

                message = new MSNMessage();

                for (int i = 0; i < returnVector.Length; i++)
                {
                    value = returnVector[i].ToString();

                    if (!String.IsNullOrEmpty(value))
                    {
                        if (i % 2 != 0)
                        {
                            message.Message = value;
                        }
                        else
                        {
                            message.Forwarder = value;
                        }
                    }

                }

                lista.Add(message);
                messageString = EstabilishConnection(cmd, false);

            }

            return lista;
        }

        private static String EstabilishConnection(String command, bool useCharStop)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                //Console.WriteLine("Conectando.....");
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(MessengerLib.Config.ServerURL), MessengerLib.Config.TCPPort);
                tcpclnt.Connect(serverEndPoint);

                //Console.WriteLine("Conectado!");
                Stream stm = tcpclnt.GetStream();

                //UTF8Encoding enc = new UTF8Encoding();

                Encoding enc = System.Text.Encoding.GetEncoding("iso-8859-1");

                byte[] ba = enc.GetBytes(command);

                //Console.WriteLine("Transmitindo.....");
                stm.Write(ba, 0, ba.Length);

                string message = ConvertMessage(stm, useCharStop);

                tcpclnt.Close();

                return message;
            }
            catch (Exception erro)
            {
                Console.WriteLine("Erro: " + erro.StackTrace);
                return MessengerLib.Config.ErrorMessage;
            }
        }

        private static String ConvertMessage(Stream stream, bool useCharStop)
        {
            byte[] bb = new byte[1000];

            int index = stream.Read(bb, 0, 1000);

            StringBuilder message = new StringBuilder();
            char? charAtual = null;

            if (useCharStop)
            {
                bool continuarLendo = true;
                string compararParada = null;
                char? charAnterior = null;

                while (continuarLendo)
                {
                    for (int i = 0; i < index; i++)
                    {
                        charAtual = Convert.ToChar(bb[i]);
                        message.Append(charAtual.ToString());

                        compararParada = String.Format("{0}{1}", charAnterior, charAtual);
                        charAnterior = charAtual;

                        if (compararParada.Equals(MessengerLib.Config.EndStackMessage))
                        {
                            continuarLendo = false;
                        }
                    }

                    if (continuarLendo)
                    {
                        index = stream.Read(bb, 0, 100);
                    }
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    charAtual = Convert.ToChar(bb[i]);
                    message.Append(charAtual.ToString());
                }
            }


            return message.ToString();
        }

        public static void ClearAuthentication()
        {
            authentication = String.Empty;
        }

        public static string GetAuthentication()
        {
            if (String.IsNullOrEmpty(authentication))
            {
                authentication = String.Format("{0}:{1}", MSNSession.User.UserLogin, MSNSession.User.UserPassword);
            }

            return authentication;
        }

    }
}
