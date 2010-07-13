using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFMessenger.Core
{
    public class RSSNews
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string link;

        public string Link
        {
            get { return link; }
            set { link = value; }
        }

    }
}
