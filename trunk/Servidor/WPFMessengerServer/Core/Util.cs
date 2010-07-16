using System;
using System.Collections.Generic;
using System.Text;
using WPFMessengerServer.Control.Model;

namespace WPFMessengerServer
{
    public static class Util
    {
        private static Control.DAO.MSNUser dao = new Control.DAO.MSNUser();
        private static IList<MSNUser> listOnline = new List<MSNUser>();
        private static Dictionary<MSNUser, IList<MSNMessage>> dicMessages = new Dictionary<MSNUser, IList<MSNMessage>>();
        private static Object lockMsg = new Object();

        public static MSNUser GetUser(string user, string password)
        {
            return dao.Get(user, password);
        }

        public static MSNUser GetContact(string user)
        {
            return dao.GetContact(user);
        }

        public static string GetUsers()
        {
            StringBuilder sb = new StringBuilder();

            IList<MSNUser> list = dao.GetList();

            foreach (Control.Model.MSNUser usuario in list)
            {
                sb.Append(String.Format("{0}:{1}:", usuario.Login, usuario.Name));
            }

            //fim da cadeia de caracteres
            sb.Append(MessengerLib.Config.EndStackMessage);

            return sb.ToString();
        }

        public static void AddOnline(MSNUser user)
        {
            listOnline.Add(user);
        }

        public static void ShutdownUser(MSNUser user)
        {
            listOnline.Remove(user);
        }

        public static bool IsOnline(MSNUser user)
        {
            return listOnline.Contains(user);
        }

        public static void AddMessage(MSNUser forwarder, MSNUser destiny, string content )
        {
            lock (lockMsg)
            {
                MSNMessage message = new MSNMessage();
                message.Message = content;
                message.Forwarder = forwarder;

                if (!dicMessages.ContainsKey(destiny))
                {
                    dicMessages.Add(destiny, new List<MSNMessage>());
                }

                dicMessages[destiny].Add(message);
            }
        }

        public static string GetMessages(MSNUser user)
        {
            StringBuilder sb = new StringBuilder();

            lock (lockMsg)
            {
                foreach (MSNMessage msg in dicMessages[user])
                {
                    sb.Append(String.Format("{0}:{1}", msg.Forwarder.Login, msg.Message));
                }

                sb.Append(MessengerLib.Config.EndStackMessage);

                //limpa o cache de mensagens
                dicMessages[user].Clear();
            }

            return sb.ToString();
        }

    }
}
