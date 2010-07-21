using System;
using System.Threading;
using WPFMessengerServer.Core;
using System.Net;

namespace WPFMessengerServer
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("-------------- Servidor WPFMessengerSeg Iniciado --------------");
            Console.WriteLine("IP: " + Dns.GetHostAddresses(Dns.GetHostName()));
            Console.WriteLine("Porta TCP: " + MessengerLib.Util.Config.TCPPort);
            Console.WriteLine("Porta UDP: " + MessengerLib.Util.Config.UDPPort);

            //Requisições TCP

            Console.WriteLine("\nIniciando serviço de mensagens TCP...");
            TCPListener tcp = new TCPListener();
            Thread listenTCP = new Thread(new ThreadStart(tcp.Listen));
            listenTCP.Start();

            Console.WriteLine("Iniciando serviço de mensagens UDP...");
            UDPListener udp = new UDPListener();
            Thread listenUDP = new Thread(new ThreadStart(udp.Listen));
            listenUDP.Start();

            Console.WriteLine("\n\n-------------- EVENTOS-------------- ");
        }
    }
}
