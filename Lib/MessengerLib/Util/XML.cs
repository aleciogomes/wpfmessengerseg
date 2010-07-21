using System;
using System.Text;
using System.Xml;
using System.IO;

namespace MessengerLib.Util
{
    public class XML
    {

        private XmlDocument doc;
        private XmlNode rootNode; 

        public XML()
        {
            this.doc = new XmlDocument();
        }

        public void IntializeXML()
        {
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            this.doc.AppendChild(docNode);

            this.rootNode = doc.CreateElement("config");

            this.doc.AppendChild(this.rootNode);
        }

        public void CreateGroupElement(string name, string value)
        {
            XmlNode node = doc.CreateElement(name);
            node.AppendChild(doc.CreateTextNode(value));

            this.rootNode.AppendChild(node);
        }

        public string ReadTagValue(Stream file, string tag)
        {
            try
            {
                this.doc.Load(file);

                XmlElement root = doc.DocumentElement;

                XmlNodeList list = root.GetElementsByTagName(tag);

                StringBuilder sb = new StringBuilder();

                foreach (XmlNode node in list)
                {
                    sb.Append(node.InnerText);
                }

                return sb.ToString();
            }
            catch
            {
                return String.Empty;
            }
        }

        public void Save(string path)
        {
            this.doc.Save(path);
        }

        public string ToString()
        {
            return this.doc.InnerXml;
        }
 
    }
}
