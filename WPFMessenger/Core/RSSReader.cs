using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Controls;

namespace WPFMessenger.Core
{
    public class RSSReader
    {
        private string rssLink = "http://www.furb.br/novo/index.php?option=rss&task=noticia&referencia=1&Itemid=10000&sis_id_lang=1";
        private IList<RSSNews> listNews;

        public IList<RSSNews> ListNews
        {
            get { return listNews; }
            set { listNews = value; }
        }

        public void Read()
        {
            listNews = new List<RSSNews>();

            XmlTextReader rssReader = new XmlTextReader(rssLink);
            XmlDocument rssDoc = new XmlDocument();

            try
            {
                rssDoc.Load(rssReader);

                XmlNode nodeRss = null;

                for (int i = 0; i < rssDoc.ChildNodes.Count && nodeRss == null; i++)
                {
                    if (rssDoc.ChildNodes[i].Name == "rss")
                    {
                        nodeRss = rssDoc.ChildNodes[i];
                    }

                }

                XmlNode nodeChannel = null;
                for (int i = 0; i < nodeRss.ChildNodes.Count && nodeChannel == null; i++)
                {
                    if (nodeRss.ChildNodes[i].Name == "channel")
                    {
                        nodeChannel = nodeRss.ChildNodes[i];
                    }
                }

                XmlNode nodeItem = null;
                RSSNews news;
                for (int i = 0; i < nodeChannel.ChildNodes.Count; i++)
                {

                    if (nodeChannel.ChildNodes[i].Name == "item")
                    {

                        nodeItem = nodeChannel.ChildNodes[i];

                        news = new RSSNews();
                        news.Title = nodeItem["title"].InnerText.ToString();
                        news.Link = nodeItem["link"].InnerText.ToString();

                        listNews.Add(news);

                    }

                }
            }
            catch { }
        }

    }
}
