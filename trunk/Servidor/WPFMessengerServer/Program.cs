using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using WPFMessengerServer.Core;

namespace WPFMessengerServer
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //Requisições TCP
            TCPListener tcp = new TCPListener();
            Thread listenTCP = new Thread(new ThreadStart(tcp.Listen));
            listenTCP.Start();

            UDPListener udp = new UDPListener();
            Thread listenUDP = new Thread(new ThreadStart(udp.Listen));
            listenUDP.Start();
        }
    }
}
