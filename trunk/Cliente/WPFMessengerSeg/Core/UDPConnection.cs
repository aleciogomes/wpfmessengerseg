using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WPFMessengerSeg.Core
{
    public class UDPConnection
    {
        private IPAddress ipAdress = null;
        private UdpClient udp;

        public UDPConnection()
        {
            this.ipAdress = IPAddress.Parse(MessengerLib.Config.ServerURL);
            udp = new UdpClient();
        }

        public bool SendMessage(MSNUser destintyUser, string message)
        {
            string info = String.Format("{0}:{1}:{2}:{3}", MSNSession.User.UserLogin, MSNSession.User.UserPassword, destintyUser.UserLogin, message);
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.SendMsg, info);

            return this.Transfer(cmd);
        }


        public void Logoff()
        {
            string authentication = String.Format("{0}:{1}", MSNSession.User.UserLogin, MSNSession.User.UserPassword);
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.Logoff, authentication);

            this.Transfer(cmd);
        }

        private bool Transfer(string cmd)
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
