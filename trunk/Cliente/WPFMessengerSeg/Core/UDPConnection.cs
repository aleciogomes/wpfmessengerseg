using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MessengerLib.Handler;
using WPFMessengerSeg.Core.util;
using MessengerLib.Core;

namespace WPFMessengerSeg.Core
{
    public static class UDPConnection
    {
        private static IPAddress ipAdress = IPAddress.Parse( MSNConfig.ServerURL);
        private static UdpClient udp = new UdpClient();

        public static bool SendMessage(MSNUser destintyUser, string message)
        {
            //criptografa a mensagem
            message = MessengerLib.Util.Encoder.EncryptMessage(message);

            string info = String.Format("{0}:{1}:{2}", TCPConnection.GetAuthentication(), destintyUser.Login, message);
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.SendMsg, info);

            return Transfer(cmd);
        }


        public static void Logoff()
        {
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.Logoff, TCPConnection.GetAuthentication());

            Transfer(cmd);
        }

        public static void UpdateAccount(string userLogin, string newName, string newUser, string newPassword)
        {

            if (!String.IsNullOrEmpty(newName) && !String.IsNullOrEmpty(newUser) && !String.IsNullOrEmpty(newPassword))
            {
                bool selfUpdate = (userLogin.Equals(MSNSession.User.Login));

                string info = String.Format("{0}:{1}:{2}:{3}:{4}", TCPConnection.GetAuthentication(), userLogin, newName, newUser, newPassword);

                string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.UpdateAccMainInfo, info);

                Transfer(cmd);

                //atualizando a própria conta
                if (selfUpdate)
                {
                    TCPConnection.ClearAuthentication();
                    MSNSession.User.Name = newName;
                    MSNSession.User.Login = newUser;
                    MSNSession.User.Password = newPassword;
                }
            }

        }

        public static void UpdateAccountOtherInfo(string userLogin, string expiration, string timeAlert, bool? blocked, string unblockDate)
        {

            string info = String.Format("{0}:{1}:{2}:{3}:{4}:{5}", TCPConnection.GetAuthentication(), userLogin, expiration, timeAlert, blocked.ToString(), unblockDate);

            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.UpdateAccOtherInfo, info);

            Transfer(cmd);

        }

        public static void CreateAccount(string newName, string newUser, string newPassword, string expiration, string timeAlert, bool? blocked)
        {
            if (!String.IsNullOrEmpty(newName) && !String.IsNullOrEmpty(newUser) && !String.IsNullOrEmpty(newPassword))
            {
                string info = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", TCPConnection.GetAuthentication(), newUser, newName, newPassword, expiration, timeAlert, blocked.ToString(), DateTime.Now.ToString(MessengerLib.Util.Config.DateFormat));
                string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.CreateAcc, info);

                Transfer(cmd);
            }
        }

        public static void DeleteAccount(string userLogin)
        {
            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), userLogin);
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.DeleteAcc, info);

            Transfer(cmd);
        }

        public static void UpdatePermissions(IList<Operation> list, MSNUser user)
        {
            StringBuilder sb = new StringBuilder();

            foreach(Operation op in list)
            {
                sb.Append(String.Format("{0}:", op.ToString()));
            }

            string info = String.Format("{0}:{1}:{2}:{3}", TCPConnection.GetAuthentication(), user.Login, user.ID, sb.ToString());
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.UpdatePermissions, info);

            Transfer(cmd);
        }

        public static void InvalidPassword(string userLogin)
        {
            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), userLogin);
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.EventInvalidPassword, info);

            Transfer(cmd);
        }

        public static void SendEmoticonInMsg()
        {
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.EventSendEmoticonInMsg,  TCPConnection.GetAuthentication());
            Transfer(cmd);
        }

        public static void ReceiveEmoticonInMsg()
        {
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.EventRecEmoticonInMsg, TCPConnection.GetAuthentication());
            Transfer(cmd);
        }

        public static void ExportContactList()
        {
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.EventExportContacts, TCPConnection.GetAuthentication());
            Transfer(cmd);
        }

        public static void InvalidConfig()
        {
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.EventInvalidConfig, Win32.GetIP());

            Transfer(cmd);
        }

        public static void SaveMotherBoardID()
        {
            string info = String.Format("{0}:{1}", TCPConnection.GetAuthentication(), MSNSession.User.ConfigMotherBoardID);
            string cmd = MessengerLib.Handler.ActionHandler.FormatAction(MessengerLib.Handler.Action.SaveMotherBoardID, info);

            Transfer(cmd);
        }

        private static bool Transfer(string cmd)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(ipAdress, MSNConfig.UDPPort);

                byte[] data = MessengerLib.Util.Encoder.GetEncoding().GetBytes(cmd);
                udp.Send(data, data.Length, ip);

                return true;
            }
            catch (Exception erro)
            {
                Console.WriteLine("Erro: " + erro.StackTrace);
                return false;
            }
        }

    }
}
