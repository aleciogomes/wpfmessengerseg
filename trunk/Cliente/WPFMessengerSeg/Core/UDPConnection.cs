using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WPFMessengerSeg.Core
{
    public static class UDPConnection
    {
        private static IPAddress ipAdress = IPAddress.Parse(MessengerLib.Config.ServerURL);
        private static UdpClient udp = new UdpClient();


        public static bool SendMessage(MSNUser destintyUser, string message)
        {
            string info = String.Format("{0}:{1}:{2}", TCPConnection.GetAuthentication(), destintyUser.UserLogin, message);
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.SendMsg, info);

            return Transfer(cmd);
        }


        public static void Logoff()
        {
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.Logoff, TCPConnection.GetAuthentication());

            Transfer(cmd);
        }

        public static void UpdateAccount(string newName, string newUser, string newPassword)
        {

            if (!String.IsNullOrEmpty(newName) && !String.IsNullOrEmpty(newUser) && !String.IsNullOrEmpty(newPassword))
            {
                string info = String.Format("{0}:{1}:{2}:{3}", TCPConnection.GetAuthentication(), newName, newUser, newPassword);

                string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.UpdateAccount, info);

                Transfer(cmd);

                TCPConnection.ClearAuthentication();
                MSNSession.User.UserName = newName;
                MSNSession.User.UserLogin = newUser;
                MSNSession.User.UserPassword = newPassword;
            }

        }

        private static bool Transfer(string cmd)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(ipAdress, MessengerLib.Config.UDPPort);

                byte[] data = MessengerLib.Encoder.GetEncoding().GetBytes(cmd);
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
