﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using MessengerLib;

namespace WPFMessengerSeg.Core
{

    public delegate bool TCPConnectionCaller();

    public static class TCPConnection
    {

        private static int port = 1012;
        private static string errorMsg = "Erro: Id inválido: ";
        private static string serverURL = "127.0.0.1";
        private static string authentication = String.Empty;


        public static IList<MSNUser> GetListUsers()
        {

            IList<MSNUser> list = new List<MSNUser>();
            MSNUser user = null;

            string returnString = GetUsers();

            if (ValidetConnect(returnString) && !returnString.Equals("0::"))
            {
                string[] returnVector = returnString.Split(new char[] { ':' });
                string value = null;

                for (int i = 0; i < returnVector.Length; i++)
                {
                    value = returnVector[i].ToString();

                    if (!String.IsNullOrEmpty(value))
                    {
                        if (i % 2 == 0)
                        {
                            user = new MSNUser();
                            user.UserLogin = value;
                        }
                        else
                        {
                            user.UserName = value;
                            list.Add(user);
                        }
                    }
                }
            }

            /*
            user = new MSNUser();
            user.UserID = 45;
            user.UserName = "Teste";
            list.Add(user);
            */

            return list;

        }

        public static string Connect()
        {
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.Login, GetAuthentication());

            return EstabilishConnection(cmd, false);

            /*
            IList<MSNUser> lista = GetListUsers();

            if (lista.Count > 0)
            {

                foreach (MSNUser user in lista)
                {
                    if (user.UserID == MSNSession.User.UserID)
                    {
                        MSNSession.User.UserName = user.UserName;
                        return true;
                    }
                }

                return false;
            }
            else
            {

                return false;
            }*/
        }

        private static bool ValidetConnect(string returnString)
        {
            if (!String.IsNullOrEmpty(returnString) && returnString.IndexOf(errorMsg) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static String GetUsers()
        {
            string cmd = MessengerLib.ActionHandler.FormatAction(MessengerLib.Action.GetUsrs, GetAuthentication());
            return EstabilishConnection(cmd, true);
        }

        public static IList<MSNMessage> GetMyMessages()
        {
            string cmd = String.Format("{0}{1}:{2}", "GET MSG ", MSNSession.User.UserLogin, MSNSession.User.UserPassword);

            string messageString = EstabilishConnection(cmd, false);
            string value = null;

            IList<MSNMessage> lista = new List<MSNMessage>();
            MSNMessage message = null;

            while (!messageString.Equals("0:"))
            {
                string[] returnVector = messageString.Split(new char[] { ':' }, 2);

                message = new MSNMessage();

                for (int i = 0; i < returnVector.Length; i++)
                {
                    value = returnVector[i].ToString();

                    if (!String.IsNullOrEmpty(value))
                    {
                        if (i % 2 != 0)
                        {
                            message.Message = value;
                        }
                        else
                        {
                            message.Forwarder = value;
                        }
                    }

                }

                lista.Add(message);
                messageString = EstabilishConnection(cmd, false);

            }

            return lista;
        }

        private static String EstabilishConnection(String command, bool useCharStop)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                //Console.WriteLine("Conectando.....");
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverURL), port);
                tcpclnt.Connect(serverEndPoint);

                //Console.WriteLine("Conectado!");
                Stream stm = tcpclnt.GetStream();

                //UTF8Encoding enc = new UTF8Encoding();

                Encoding enc = System.Text.Encoding.GetEncoding("iso-8859-1");

                byte[] ba = enc.GetBytes(command);

                //Console.WriteLine("Transmitindo.....");
                stm.Write(ba, 0, ba.Length);

                string message = ConvertMessage(stm, useCharStop);

                tcpclnt.Close();

                return message;
            }
            catch (Exception erro)
            {
                Console.WriteLine("Erro: " + erro.StackTrace);
                return errorMsg;
            }
        }

        private static String ConvertMessage(Stream stream, bool useCharStop)
        {
            byte[] bb = new byte[1000];

            int index = stream.Read(bb, 0, 1000);

            StringBuilder message = new StringBuilder();
            char? charAtual = null;

            if (useCharStop)
            {
                string parada = "0:";
                bool continuarLendo = true;
                string compararParada = null;
                char? charAnterior = null;

                while (continuarLendo)
                {
                    for (int i = 0; i < index; i++)
                    {
                        charAtual = Convert.ToChar(bb[i]);
                        message.Append(charAtual.ToString());

                        compararParada = String.Format("{0}{1}", charAnterior, charAtual);
                        charAnterior = charAtual;

                        if (compararParada.Equals(parada))
                        {
                            continuarLendo = false;
                        }
                    }

                    if (continuarLendo)
                    {
                        index = stream.Read(bb, 0, 100);
                    }
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    charAtual = Convert.ToChar(bb[i]);
                    message.Append(charAtual.ToString());
                }
            }


            return message.ToString();
        }

        private static string GetAuthentication()
        {
            if (String.IsNullOrEmpty(authentication))
            {
                authentication = String.Format("{0}:{1}", MSNSession.User.UserLogin, MSNSession.User.UserPassword);
            }

            return authentication;
        }

    }
}
