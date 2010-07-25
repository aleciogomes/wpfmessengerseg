using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;

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
            this.rootNode = this.doc.DocumentElement;
        }

        public XmlNode CreateElement(string tagName)
        {
            return doc.CreateElement(tagName);
        }

        public XmlNode GetRootNode()
        {
            return this.rootNode;
        }

        public XmlNode AppendNode(XmlNode node)
        {
            return this.rootNode.AppendChild(node);
        }

        public void RemoveNode(string tagName)
        {

            //seleciona o nodo
            XmlNode node = this.rootNode.SelectSingleNode(tagName);

            //faz a remoção
            this.rootNode.RemoveChild(node);
        }

        public XmlNode InsertIntoGroup(XmlNode group, string tagName, string value)
        {
            XmlNode node = doc.CreateElement(tagName);
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

        public IList<string> ReadTagValues(string tag)
        {
            IList<string> listResult = new List<string>();

            try
            {
                XmlElement root = doc.DocumentElement;

                XmlNodeList list = root.GetElementsByTagName(tag);

                foreach (XmlNode node in list)
                {
                    listResult.Add(node.InnerText);
                }
            }
            catch{}

            return listResult;
        }

        public void UpdateTagValue(string tagName, string newValue)
        {
            XmlNodeList list = doc.GetElementsByTagName(tagName);

            foreach (XmlNode node in list)
            {
                if (node.Name.Equals(tagName))
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


        public XmlNode AppendHashNode(string value)
        {
            XmlNode node    = CreateElement(HASH_TAG);
            node.InnerText  = MessengerLib.Util.Encoder.GenerateMD5(value);

            return AppendNode(node);
        }
    }
}
