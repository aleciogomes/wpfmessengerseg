using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace WPFMessengerSeg.Core.util
{
    public class Signature
    {
        private string publicKey;
        private string privateKey;

        public const string KEY_EXT = ".msnkey";

        public Signature(bool generateKeys)
        {
            if (generateKeys)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                this.privateKey = rsa.ToXmlString(true);
                this.publicKey = rsa.ToXmlString(false);
            }
        }

        public void ReadSginatureFile(string filePath)
        {
            string content = MessengerLib.Util.Encoder.DecryptMessage(File.ReadAllText(filePath));
            MSNSession.User.SignaturePrivateKey = content;
        }

        public void GenerateSignatureFile(string savePath)
        {
            //salva as chaves do usuário no objeto de sessão
            MSNSession.User.SignaturePublicKey = publicKey;
            MSNSession.User.SignaturePrivateKey = privateKey;

            //salva no BD a chave publica
            UDPConnection.SavePublicKey(MessengerLib.Util.Encoder.EncryptMessage(publicKey));

            using (StreamWriter file = new StreamWriter(savePath))
            {
                //criptografia da private key
                file.Write(MessengerLib.Util.Encoder.EncryptMessage(privateKey));
            }
        }

    }
}
