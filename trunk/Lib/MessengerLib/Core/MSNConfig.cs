using System;
using System.IO;
using MessengerLib.Util;

namespace MessengerLib.Core
{
    public static class MSNConfig
    {
        private const string CONFIG_FILE    = "wpfmsn.cfg";

        private const string SERVER_TAG     = "serverMSN";
        private const string TCP_TAG        = "tcp";
        private const string UDP_TAG        = "udp";

        private const string HASH_TAG       = "hash";
        private const string HASH_KEY       = "wpfKeyConfigM$n";

        public static string ServerURL  = String.Empty;
        public static int TCPPort = 0;
        public static int UDPPort = 0;

        public static bool InvalidHash = false;
        private static bool isTempCfg = false;

        private static string configPath = String.Empty;

        public static bool IsTempURL
        {
            get
            {
                return isTempCfg;
            }
        }

        public static void LoadTempCfg()
        {
            isTempCfg = true;
            ServerURL = Util.Config.DefaultServer;
            TCPPort = Util.Config.DefaultTCPPort;
            UDPPort = Util.Config.DefaultUDPPort;
        }

        public static void LoadConfig(string path)
        {
            try
            {

                configPath = path;

                string content = File.ReadAllText(Path.Combine(configPath, CONFIG_FILE));

                try
                {
                    content = Util.Encoder.DecryptMessage(content);

                    Stream stream = new MemoryStream(Util.Encoder.GetEncoding().GetBytes(content));

                    XML docXML = new XML();
                    docXML.LoadStream(stream);

                    ServerURL = docXML.ReadTagValue(SERVER_TAG);
                    TCPPort = int.Parse(docXML.ReadTagValue(TCP_TAG));
                    UDPPort = int.Parse(docXML.ReadTagValue(UDP_TAG));

                    string configHash = docXML.ReadTagValue(HASH_TAG);

                    //verifica
                    if (!configHash.Equals(Util.Encoder.GenerateMD5(HASH_KEY)))
                    {
                        InvalidHash = true;
                    }
                }
                catch
                {
                    InvalidHash = true;
                }

            }
            catch{}
        }

        public static void CreateDefaultConfig()
        {
            ServerURL = Config.DefaultServer;

            XML docXML = new XML();

            docXML.IntializeXML("config");
            docXML.InsertIntoGroup(docXML.GetRootNode(), SERVER_TAG, Util.Config.DefaultServer);
            docXML.InsertIntoGroup(docXML.GetRootNode(), TCP_TAG, Util.Config.DefaultTCPPort.ToString());
            docXML.InsertIntoGroup(docXML.GetRootNode(), UDP_TAG, Util.Config.DefaultUDPPort.ToString());
            docXML.InsertIntoGroup(docXML.GetRootNode(), HASH_TAG, Util.Encoder.GenerateMD5(HASH_KEY));

            string xmlContent = docXML.ToString();
            xmlContent = Util.Encoder.EncryptMessage(xmlContent);

            using (StreamWriter file = new StreamWriter(Path.Combine(configPath, CONFIG_FILE)))
            {
                file.Write(xmlContent);
            }
        }

        public static void UpdateConfig(string serverURL, int tcpPort, int udpPort)
        {
            try
            {

                string content = File.ReadAllText(Path.Combine(configPath, CONFIG_FILE));
                content = Util.Encoder.DecryptMessage(content);

                Stream stream = new MemoryStream(Util.Encoder.GetEncoding().GetBytes(content));

                XML docXML = new XML();
                docXML.LoadStream(stream);

                //atualiza valores
                docXML.UpdateTagValue(SERVER_TAG, serverURL);
                docXML.UpdateTagValue(TCP_TAG, tcpPort.ToString());
                docXML.UpdateTagValue(UDP_TAG, udpPort.ToString());

                string xmlContent = docXML.ToString();
                xmlContent = Util.Encoder.EncryptMessage(xmlContent);

                using (StreamWriter file = new StreamWriter(Path.Combine(configPath, CONFIG_FILE), false))
                {
                    file.Write(xmlContent);
                }

            }
            catch { }
        }

    }
}
