using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MessengerLib.Util;

namespace WPFMessengerSeg.Core.util
{
    public class Report
    {
        private const string LIST_TAG    = "contatos";
        private const string USERS_TAG = "usuarios";
        private const string USER_TAG = "usuario";
        private const string USERID_TAG = "id";
        public const string REPORT_EXT  = ".msnlist";

        public void GenerateContactReport(IList<int> listContacts, string savePath)
        {

            XML docXML = new XML();

            docXML.IntializeXML(LIST_TAG);

            XmlNode rootUsers = docXML.AppendNode(docXML.CreateElement(USERS_TAG));
            XmlNode userNode = null;

            foreach(int id in listContacts)
            {
                //insere id do contato no XML
                userNode = docXML.InsertIntoGroup(rootUsers, USER_TAG, String.Empty);
                docXML.InsertIntoGroup(userNode, USERID_TAG, id.ToString());
            }

            //cria o hash do conteudo
            string hashContent = MessengerLib.Util.Encoder.GenerateMD5(docXML.ToString());
            docXML.AppendHashNode(hashContent);

            using (StreamWriter file = new StreamWriter(savePath))
            {
                //criptografia de todo o conteudo
                string encryptXML =  MessengerLib.Util.Encoder.EncryptMessage(docXML.ToString());
                file.Write(encryptXML);
            }

        }

    }
}
