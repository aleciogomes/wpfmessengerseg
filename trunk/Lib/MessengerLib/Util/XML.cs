using System;
using System.Text;
using System.Xml;
using System.IO;

namespace MessengerLib.Util
{
    public class XML
    {

        public const string HASH_TAG = "hash";

        private XmlDocument doc;
        private XmlNode rootNode; 

        public XML()
        {
            this.doc = new XmlDocument();
        }

        public void IntializeXML(string rootElement)
        {
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            this.doc.AppendChild(docNode);

            this.rootNode = doc.CreateElement(rootElement);

            this.doc.AppendChild(this.rootNode);
        }

        public void LoadStream(Stream file)
        {
            this.doc.Load(file);
        }

        public XmlNode CreateElement(string name)
        {
            return doc.CreateElement(name);
        }

        public XmlNode GetRootNode()
        {
            return this.rootNode;
        }

        public XmlNode AppendNode(XmlNode node)
        {
            return this.rootNode.AppendChild(node);
        }

        public XmlNode InsertIntoGroup(XmlNode group, string name, string value)
        {
            XmlNode node = doc.CreateElement(name);
            node.InnerText = value;

            return group.AppendChild(node);
        }

        public string ReadTagValue(string tag)
        {
            try
            {
                XmlElement root = doc.DocumentElement;

                XmlNodeList list = root.GetElementsByTagName(tag);

                string value = string.Empty;

                foreach (XmlNode node in list)
                {
                    value = node.InnerText;
                    break;
                }

                return value;
            }
            catch
            {
                return String.Empty;
            }
        }

        public void UpdateTagValue(string tag, string newValue)
        {
            XmlNodeList list = doc.GetElementsByTagName(tag);

            foreach (XmlNode node in list)
            {
                if (node.Name.Equals(tag))
                {
                    node.InnerText = newValue;
                    break;
                } 
            }
        }

        public void Save(string path)
        {
            this.doc.Save(path);
        }

        public override string ToString()
        {
            return this.doc.InnerXml;
        }


        public void AppendHashNode(string value)
        {
            XmlNode node    = CreateElement(HASH_TAG);
            node.InnerText  = MessengerLib.Util.Encoder.GenerateMD5(value);

            AppendNode(node);
        }
    }
}
