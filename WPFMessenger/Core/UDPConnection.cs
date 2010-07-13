using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace WPFMessenger.Core
{
    class UDPConnection
    {
        private int port = 1011;
        private string serverURL = "larc.inf.furb.br";
        private IPAddress ipAdress = null;
        private string getMsgString = "SEND MSG ";

        private UdpClient udp;

        public UDPConnection()
        {
            IPHostEntry ip = Dns.GetHostEntry(serverURL);
            IPAddress [] ipList = ip.AddressList;

            if (ipList.Length > 0)
            {
                ipAdress = ipList[0];
            }

            udp = new UdpClient();
        }

        public bool SendMessage(MSNUser destintyUser, string message)
        {

            string command = String.Format("{0}{1}:{2}:{3}:{4}", this.getMsgString, MSNSession.User.UserID, MSNSession.User.UserPassword, destintyUser.UserID, message);

            try
            {
                IPEndPoint ip = new IPEndPoint(ipAdress, port);

                byte[] data = Encoding.Default.GetBytes(command);
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
