﻿using System;
using System.Collections.Generic;
using System.Text;
using WPFMessengerServer.Control.Model;
using System.Linq;
using WPFMessengerServer.Control.DAO;

namespace WPFMessengerServer
{
    public static class Util
    {
        private static Control.DAO.MSNUser daoUser = new Control.DAO.MSNUser();
        private static Control.DAO.MSNFeature daoFeature = new Control.DAO.MSNFeature();
        private static Control.DAO.MSNPermission daoPermission = new Control.DAO.MSNPermission();


        private static IList<Control.Model.MSNUser> listOnline = new List<Control.Model.MSNUser>();
        private static Dictionary<string, IList<MSNMessage>> dicMessages = new Dictionary<string, IList<MSNMessage>>();
        private static Object lockMsg = new Object();

        public static Control.Model.MSNUser GetUser(string user, string password)
        {
            return daoUser.Get(user, password);
        }

        public static Control.Model.MSNUser GetContact(string user)
        {
            return daoUser.GetContact(user);
        }

        public static string GetUserInfo(string user)
        {
            Control.Model.MSNUser msnUser = daoUser.GetContact(user);
           return String.Format("{0}:{1}:{2}:{3}:{4}:{5}", msnUser.Name, msnUser.Password, msnUser.ExpirationString(false), msnUser.TimeAlert, msnUser.Blocked, msnUser.UnblockDateString(false));
        }

        public static string GetUsers()
        {
            IList<Control.Model.MSNUser> list = daoUser.GetList();

            StringBuilder sb = new StringBuilder();
            foreach (Control.Model.MSNUser usuario in list)
            {
                sb.Append(String.Format("{0}:{1}:{2}:{3}:", usuario.ID, usuario.Login, usuario.Name, IsOnline(usuario.Login)));
            }

            //fim da cadeia de caracteres
            sb.Append(MessengerLib.Config.EndStackMessage);

            return sb.ToString();
        }

        public static string GetFeatures(int userID)
        {
            IList<MessengerLib.Core.MSNFeature> list = daoFeature.GetList(userID);

            StringBuilder sb = new StringBuilder();
            foreach (MessengerLib.Core.MSNFeature feature in list)
            {
                sb.Append(String.Format("/{0}:{1}|", feature.ID, feature.Name.ToString()));

                foreach (MessengerLib.Core.MSNOperation operation in feature.ListOperation)
                {
                    sb.Append(String.Format("{0}:{1}:", operation.ID, operation.Name.ToString()));
                }
            }

            if (sb.Length == 0)
            {
                sb.Append(MessengerLib.Config.EndStackMessage);
            }

            return sb.ToString();
        }

        public static void UpdatePermissions(Control.Model.MSNUser user, IList<string> listOperation)
        {
            //reseta permissões
            daoPermission.Delete(user);

            foreach (string operation in listOperation)
            {
                //insere a permissão
                daoPermission.Insert(user, operation);
            }
        }

        public static void AddOnline(Control.Model.MSNUser user)
        {
            listOnline.Add(user);
        }

        public static void ShutdownUser(string user)
        {
            IList<Control.Model.MSNUser> query = (from msnUser in listOnline
                                    where msnUser.Login.Equals(user)
                                    select msnUser).ToList();

            foreach (Control.Model.MSNUser msnUser in query)
            {
                listOnline.Remove(msnUser);
            }

        }

        public static void UpdateAccount(string user, Control.Model.MSNUser msnUser)
        {
            daoUser.Update(user, msnUser);
        }

        public static void UpdateAccountOtherInfo(Control.Model.MSNUser msnUser)
        {
            daoUser.UpdateOtherInfo(msnUser);
        }

        public static void CreateAccount(Control.Model.MSNUser user)
        {
            daoUser.Insert(user);
        }

        public static void DeleteAccount(string user)
        {
            ShutdownUser(user);
            daoUser.Delete(user);
        }

        public static bool IsOnline(string user)
        {
            IList<Control.Model.MSNUser> query = (from msnUser in listOnline
                                    where msnUser.Login.Equals(user)
                                    select msnUser).ToList();

            return (query.Count > 0);
        }

        public static void AddMessage(Control.Model.MSNUser forwarder, string destiny, string content)
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
