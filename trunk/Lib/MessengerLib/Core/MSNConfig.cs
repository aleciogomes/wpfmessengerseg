using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessengerLib.Util;
using System.IO;

namespace MessengerLib.Core
{
    public static class MSNConfig
    {
        private const string CONFIG_FILE    = "wpfmsn.cfg";
        private const string SERVER_TAG     = "serverMSN";

        public static string ServerURL     = String.Empty;

        public static void LoadServerURL(string configPath)
        {
            XML docXML = new XML();

            string content = File.ReadAllText(Path.Combine(configPath, CONFIG_FILE));
            content = Util.Encoder.DecryptMessage(content);

            /*
            string tagValue = docXML.ReadTagValue(fStream, SERVER_TAG);

            if (!String.IsNullOrEmpty(tagValue))
            {
                ServerURL = tagValue;
            }
             * */
        }

        public static string CreateDefaultConfig()
        {
            ServerURL = Config.ServerURL;

            XML docXML = new XML();

            docXML.IntializeXML();
            docXML.CreateGroupElement(SERVER_TAG, ServerURL);

            string xmlContent = docXML.ToString();

            return Util.Encoder.EncryptMessage(xmlContent);
        }

        private static void SaveDefaultConfig()
        {

        }

    }
}
