using System.Collections.Generic;
using WPFMessengerServer.Control.Model;
using System;
using System.Text;

namespace WPFMessengerServer
{
    public static class Util
    {
        private static Control.DAO.MSNUser dao = new Control.DAO.MSNUser();
        private static IList<MSNUser> listOnline = new List<MSNUser>();
        private static Dictionary<MSNUser, IList<MSNMessage>> dicMessages = new Dictionary<MSNUser, IList<MSNMessage>>();

        public static Control.Model.MSNUser GetUser(string user, string password)
        {
            return dao.Get(user, password);
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
            sb.Append("0::");

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

        public static void AddMessage(MSNUser from, MSNUser to, string content )
        {
            MSNMessage message = new MSNMessage();
            message.Message = content;
            message.Forwarder = from;

            if (!dicMessages.ContainsKey(to))
            {
                dicMessages.Add(to, new List<MSNMessage>());
            }

            dicMessages[to].Add(message);
        }

        public static IList<string> GetMessages(MSNUser user)
        {
            IList<string> messages = new List<string>();

            foreach(MSNMessage msg in dicMessages[user])
            {
                messages.Add(String.Format("{0}:{1}", msg.Forwarder.Login, msg.Message));
            }

            messages.Add("0:");

            return messages;
        }

    }
}
