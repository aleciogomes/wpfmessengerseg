using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace WPFMessengerServer
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Listener listener = new Listener();
            Thread listenThread = new Thread(new ThreadStart(listener.ListenForClients));
            listenThread.Start();
        }
    }
}
