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
        private const string GENERATOR_TAG = "gerador";
        private const string USERS_TAG = "usuarios";
        private const string USER_TAG = "usuario";
        private const string USERID_TAG = "id";
        private const string SIGN_TAG = "assinatura";

        public const string REPORT_EXT  = ".msnlist";

        public IList<string> ImportedValues {get;set;}
        public bool InvalidContent;
        public bool InvalidSignature;
        public bool SignatureNotFound;

        public Report()
        {
            this.InvalidContent = false;
            this.InvalidSignature = false;
            this.SignatureNotFound = false;
        }

        public void GenerateContactReport(IList<int> listContacts, string savePath, bool sign)
        {

            XML docXML = new XML();

            docXML.IntializeXML(LIST_TAG);

            docXML.InsertIntoGroup(docXML.GetRootNode(), GENERATOR_TAG, MSNSession.User.ID.ToString());

            XmlNode rootUsers = docXML.AppendNode(docXML.CreateElement(USERS_TAG));
            XmlNode userNode = null;

            foreach(int id in listContacts)
            {
                //insere id do contato no XML
                userNode = docXML.InsertIntoGroup(rootUsers, USER_TAG, String.Empty);
                docXML.InsertIntoGroup(userNode, USERID_TAG, id.ToString());
            }

            //cria o hash do conteudo
            string hash = docXML.AppendHashNode(docXML.ToString()).InnerText;

            //se deve assinar
            if (sign)
            {
                string signedHash = MessengerLib.Util.Encoder.Sign(hash, MSNSession.User.SignaturePrivateKey);
                docXML.InsertIntoGroup(docXML.GetRootNode(), SIGN_TAG, signedHash);
            }

            using (StreamWriter file = new StreamWriter(savePath))
            {
                //criptografia de todo o conteudo
                string encryptXML   =  docXML.ToString();
                encryptXML          = MessengerLib.Util.Encoder.EncryptMessage(encryptXML);

                file.Write(encryptXML);
            }
        }

        public bool ImportContactReport(string filePath, bool validateSignature)
        {
            try
            {
                //faz a descriptografia do conteudo do arquivo
                string content          =  MessengerLib.Util.Encoder.DecryptMessage(File.ReadAllText(filePath));
                Stream streamContent    = new MemoryStream(MessengerLib.Util.Encoder.GetEncoding().GetBytes(content));
                Stream stream           = new MemoryStream(MessengerLib.Util.Encoder.GetEncoding().GetBytes(content));

                //xml sem as tag HASH e ASSINATURA
                XML docXMLContent = new XML();
                docXMLContent.LoadStream(streamContent);
                docXMLContent.RemoveNode(XML.HASH_TAG);

                try
                {
                    docXMLContent.RemoveNode(SIGN_TAG);
                }
                catch { }

                //xml completo
                XML docXML = new XML();
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

                //valida a assinatura
                if (validateSignature)
                {
                    int userID = int.Parse(docXML.ReadTagValue(GENERATOR_TAG));
                    MSNUser user = TCPConnection.GetAuthorPublicKey(userID);

                    string signedHash = docXML.ReadTagValue(SIGN_TAG);

                    //se não possui assinatura
                    if (String.IsNullOrEmpty(signedHash))
                    {
                        this.SignatureNotFound = true;
                        return false;
                    }
                    else
                    {
                        try
                        {
                            //verifica se a assinatura é válida
                            InvalidSignature = !MessengerLib.Util.Encoder.Verify(hashKeyValue, signedHash, user.SignaturePublicKey);

                            UDPConnection.ValidateSignature(!InvalidSignature, user.Name);

                            if (InvalidSignature)
                            {
                                return false;
                            }
                        }
                        catch
                        {
                            InvalidSignature = true;
                            UDPConnection.ValidateSignature(!InvalidSignature, user.Name);
                            return false;
                        }

                    }
                }

                this.ImportedValues = docXML.ReadTagValues(USERID_TAG);

            }
            catch
            {
                this.InvalidContent = true;
                UDPConnection.InvalidImportFile(this.InvalidContent);
                return false;
            }

            return true;

        }

    }
}
