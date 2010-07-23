using System;
using System.IO;
using MessengerLib.Util;

namespace MessengerLib.Core
{
    public static class MSNConfig
    {
        private const string CONFIG_FILE    = "wpfmsn.cfg";
        private const string SERVER_TAG     = "serverMSN";
        private const string HASH_TAG       = "hash";
        private const string HASH_KEY       = "wpfKeyConfigM$n";

        public static string ServerURL  = String.Empty;
        public static bool InvalidHash = false;

        private static bool isTempURL = false;

        public static bool IsTempURL
        {
            get
            {
                return isTempURL;
            }
        }

        public static void LoadTempURL()
        {
            isTempURL = true;
            ServerURL = Util.Config.ServerURL;
        }

        public static void LoadServerURL(string configPath)
        {
            try
            {
              

                string content = File.ReadAllText(Path.Combine(configPath, CONFIG_FILE));

                try
                {
                    content = Util.Encoder.DecryptMessage(content);

                    Stream stream = new MemoryStream(Util.Encoder.GetEncoding().GetBytes(content));

                    XML docXML = new XML();
                    docXML.LoadStream(stream);

                    ServerURL = docXML.ReadTagValue(SERVER_TAG);
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

        public static void CreateDefaultConfig(string configPath)
        {
            ServerURL = Config.ServerURL;

            XML docXML = new XML();

            docXML.IntializeXML("config");
            docXML.InsertIntoGroup(docXML.GetRootNode(), SERVER_TAG, ServerURL);
            docXML.InsertIntoGroup(docXML.GetRootNode(), HASH_TAG, Util.Encoder.GenerateMD5(HASH_KEY));

            string xmlContent = docXML.ToString();
            xmlContent = Util.Encoder.EncryptMessage(xmlContent);

            using (StreamWriter file = new StreamWriter(Path.Combine(configPath, CONFIG_FILE)))
            {
                file.Write(xmlContent);
            }
        }

    }
}
