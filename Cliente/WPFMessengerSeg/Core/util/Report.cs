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

        public IList<string> ImportedValues {get;set;}
        public bool InvalidContent;

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
            docXML.AppendHashNode(docXML.ToString());

            using (StreamWriter file = new StreamWriter(savePath))
            {
                //criptografia de todo o conteudo
                string encryptXML =  MessengerLib.Util.Encoder.EncryptMessage(docXML.ToString());
                file.Write(encryptXML);
            }
        }

        public bool ImportContactReport(string filePath)
        {

            this.InvalidContent = false;

            try
            {
                //faz a descriptografia do conteudo do arquivo
                string content          =  MessengerLib.Util.Encoder.DecryptMessage(File.ReadAllText(filePath));
                Stream streamContent    = new MemoryStream(MessengerLib.Util.Encoder.GetEncoding().GetBytes(content));
                Stream stream           = new MemoryStream(MessengerLib.Util.Encoder.GetEncoding().GetBytes(content));

                XML docXMLContent       = new XML();
                docXMLContent.LoadStream(streamContent);
                docXMLContent.RemoveNode(XML.HASH_TAG);

                XML docXML              = new XML();
                docXML.LoadStream(stream);

                //gera o hash do arquivo
                string hashContent = MessengerLib.Util.Encoder.GenerateMD5(docXMLContent.ToString());

                //le o hash que existe no arquivo de importação
                string hashKeyValue = docXML.ReadTagValue(XML.HASH_TAG);

                //se o valor da tag hash não é igual ao hash do conteudo, não pode importar
                if (!hashKeyValue.Equals(hashContent))
                {
                    return false;
                }

                this.ImportedValues = docXML.ReadTagValues(USERID_TAG);

            }
            catch
            {
                this.InvalidContent = true;
                return false;
            }

            return true;

        }

    }
}
