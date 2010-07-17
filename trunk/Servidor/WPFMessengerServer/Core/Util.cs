﻿using System;
using System.Collections.Generic;
using System.Text;
using WPFMessengerServer.Control.Model;
using System.Linq;

namespace WPFMessengerServer
{
    public static class Util
    {
        private static Control.DAO.MSNUser dao = new Control.DAO.MSNUser();
        private static IList<MSNUser> listOnline = new List<MSNUser>();
        private static Dictionary<string, IList<MSNMessage>> dicMessages = new Dictionary<string, IList<MSNMessage>>();
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

        public static bool IsOnline(string user)
        {
            IList<MSNUser> query = (from msnuser in listOnline 
                                    where msnuser.Login.Equals(user)
                                    select msnuser ).ToList();

            return (query.Count > 0);
        }

        public static void AddMessage(MSNUser forwarder, string destiny, string content )
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

        public static string GetMessages(string user)
        {
            StringBuilder sb = new StringBuilder();

            lock (lockMsg)
            {

                if (dicMessages.ContainsKey(user))
                {
                    foreach (MSNMessage msg in dicMessages[user])
                    {
                        sb.Append(String.Format("{0}:{1}", msg.Forwarder.Login, msg.Message));
                    }

                    //limpa o cache de mensagens
                    dicMessages[user].Clear();

                    if (sb.Length == 0)
                    {
                        sb.Append(MessengerLib.Config.EndStackMessage);
                    }

                }
                else
                {
                    sb.Append(MessengerLib.Config.EndStackMessage);
                }

            }

            return sb.ToString();
        }

    }
}
