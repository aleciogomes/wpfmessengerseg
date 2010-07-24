using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MessengerLib.Core;
using MessengerLib.Handler;
using WPFMessengerSeg.Core.util;

namespace WPFMessengerSeg.Core
{

    public delegate bool TCPConnectionCaller();

    public static class TCPConnection
    {

        private static string authentication = String.Empty;
        public static string ExpirationWarning = String.Empty;

        public static IList<MSNUser> GetListUsers(bool onlyContacts)
        {

            IList<MSNUser> list = new List<MSNUser>();
            MSNUser user = null;

            string returnString = GetUsers(onlyContacts);

            if (ValidetReturn(returnString))
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
                            try
                            {
                                user.ID = int.Parse(value);
                            }
                            catch
                            {
                                //fim da cadeia de caracteres
                                break;
                            }
                            countAttributes++;
                        }
                        else if (countAttributes == 1)
                        {
                            user.Login = value;
                            countAttributes++;
                        }
                        else if (countAttributes == 2)
                        {
                            user.Name = value;
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

        public static string ConfirmLogin(string userLogin, string userPassword)
        {
            authentication = String.Empty;
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.ConfirmLogin, String.Format("{0}:{1}", userLogin, userPassword));
            return EstabilishConnection(cmd, false);
        }

        public static string Login()
        {
            authentication = String.Empty;
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.Login, GetAuthentication());
            string result = EstabilishConnection(cmd, false);

            string[] info = result.Split(':');

            if(info.Length > 1){

                MSNSession.User.ID = int.Parse(info[1]);
                MSNSession.User.Name = info[2];

                try
                {
                    MSNSession.User.Expiration = DateTime.Parse(info[3]);
                }
                catch
                {
                    MSNSession.User.Expiration = null;
                }

                MSNSession.User.TimeAlert = int.Parse(info[4]);

                MSNSession.User.ConfigMotherBoardID = info[5];

                //já foi grava o id da placa mãe de onde foi gerado o config
                if (!String.IsNullOrEmpty(MSNSession.User.ConfigMotherBoardID) && !MSNSession.User.ConfigMotherBoardID.Equals(Win32.MotherBoardID) && MSNConfig.IsTempURL)
                {
                    UDPConnection.Logoff();
                    return "Arquivo de configuração não encontrado";
                }
                else
                {
                    if (MSNSession.User.TimeAlert > 0 && MSNSession.User.Expiration.HasValue)
                    {
                        DateTime expiration = (DateTime)MSNSession.User.Expiration;

                        TimeSpan difference = expiration.Subtract(DateTime.Now.Date);

                        if (difference.Days <= MSNSession.User.TimeAlert)
                        {
                            ExpirationWarning = String.Format("Sua conta será expirada em {0} dia(s). Entre em contato com o administrador do sistema!", difference.Days);
                        }
                    }
                }

            }

            return info[0];
        }

        public static string GetUserAvailable(string newUserLogin)
        {
            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), newUserLogin);
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.UserAvailable, info);

            return EstabilishConnection(cmd, false);
        }

        public static MSNUser GetUserInfo(string userLogin)
        {
            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), userLogin);
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.GetUserInfo, info);

            string[] result =  EstabilishConnection(cmd, false).Split(':');

            MSNUser user = new MSNUser();

            user.Name = result[0];
            user.Login = userLogin;
            user.Password = result[1];

            try
            {
                user.Expiration = DateTime.Parse(result[2]);
            }
            catch { }

            try
            {
                user.TimeAlert = int.Parse(result[3]);
            }
            catch { }

            user.Blocked = Convert.ToBoolean(result[4]);

            try
            {
                user.UnblockDate = DateTime.Parse(result[5]);
            }
            catch { }

            return user;
        }

        public static void LoadFeatures(MSNUser user)
        {

            if (user.ListFeature == null)
            {
                user.ListFeature = new List<MSNFeature>();

                string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), user.ID);
                string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.GetFeatures, info);

                string returnString = EstabilishConnection(cmd, false);

                if (ValidetReturn(returnString))
                {

                    string[] returnVector = returnString.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    string value = null;

                    //conta quantos atributos já fora adicionados a um usuário
                    int countAttributes = 0;

                    MSNFeature feature = null;
                    string[] featureVector = null;
                    string featureString = String.Empty;

                    MSNOperation operation = null;
                    string[] operationVector = null;

                    for (int i = 0; i < returnVector.Length; i++)
                    {
                        value = returnVector[i].ToString();
                        featureVector = value.Split('|');

                        //detalhes do recurso
                        featureString = featureVector[0];
                        feature = new MSNFeature();
                        feature.ID = int.Parse(featureString.Split(':')[0]);
                        feature.Name = FeatureHandler.GetFeature(featureString.Split(':')[1]);

                        operationVector = featureVector[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        countAttributes = 0;

                        for (int j = 0; j < operationVector.Length; j++)
                        {
                            value = operationVector[j].ToString();

                            if (countAttributes == 0)
                            {
                                operation = new MSNOperation();
                                operation.ID = int.Parse(value);
                                countAttributes++;
                            }
                            else
                            {
                                //reinicia
                                countAttributes = 0;
                                operation.Name = OperationHandler.GetOperation(value);
                                feature.ListOperation.Add(operation);
                            }
                        }

                        user.ListFeature.Add(feature);
                    }

                }
            }

        }

        public static string[] GetLogDates()
        {
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.GetLogDates, GetAuthentication());
            string returnString = EstabilishConnection(cmd, false);

            if (ValidetReturn(returnString))
            {
                return returnString.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            }

            return new string[]{};
        }

        public static IList<MessengerLib.Core.MSNLog> GetLog(DateTime logDate)
        {
            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), logDate.ToString());
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.GetLog, info);
            string returnString = EstabilishConnection(cmd, true);

            IList<MessengerLib.Core.MSNLog> list = new List<MessengerLib.Core.MSNLog>();

            if (ValidetReturn(returnString))
            {
                string[] returnVector = returnString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries); ;
                int countAttributes = 0;

                MessengerLib.Core.MSNLog log = null;

                foreach (string value in returnVector)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        if (countAttributes == 0)
                        {
                            log = new MessengerLib.Core.MSNLog();

                            try
                            {
                                log.Date = DateTime.Parse(value);
                                countAttributes++;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else
                        {
                            //reinicia
                            countAttributes = 0;
                            log.Event = value;
                            list.Add(log);
                        }
                    }
                }
            }

            return list;

        }

        private static bool ValidetReturn(string returnString)
        {
            if (!String.IsNullOrEmpty(returnString) && returnString.IndexOf(MessengerLib.Util.Config.ErrorMessage) < 0 && !returnString.Equals(MessengerLib.Util.Config.EndStackMessage))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static String GetUsers(bool onlyContacts)
        {

            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), onlyContacts.ToString());
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.GetUsers, info);
            return EstabilishConnection(cmd, true);
        }

        public static IList<MSNMessage> GetMyMessages()
        {
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.GetMsg, GetAuthentication());
            string messageString = EstabilishConnection(cmd, false);

            string value = null;

            IList<MSNMessage> lista = new List<MSNMessage>();
            MSNMessage message = null;

            while (!messageString.Equals(MessengerLib.Util.Config.EndStackMessage))
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
                            //desincriptografa a mensagem
                            message.Message = MessengerLib.Util.Encoder.DecryptMessage(value);
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

        private static string EstabilishConnection(String command, bool useCharStop)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                //Console.WriteLine("Conectando.....");
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(MSNConfig.ServerURL), MSNConfig.TCPPort);
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
                return MessengerLib.Util.Config.ErrorMessage;
            }
        }

        private static string ConvertMessage(Stream stream, bool useCharStop)
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

                        if (compararParada.Equals(MessengerLib.Util.Config.EndStackMessage))
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
                authentication = String.Format("{0}:{1}", MSNSession.User.Login, MSNSession.User.Password);
            }

            return authentication;
        }

    }
}
