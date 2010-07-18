using System;

namespace WPFMessengerSeg.Core.events
{
    public class Arguments : EventArgs
    {

        private string header;
        public string Header
        {
            get { return header; }
        }

        public Arguments(string userHeader)
        {
            this.header = userHeader;
        }
    }
}
