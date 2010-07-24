using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessengerLib.Util;
using System.IO;
using System.Xml;

namespace WPFMessengerSeg.Core.util
{
    public enum ReportType
    {
        user,
    }

    class Report
    {
        public void GenerateReport(ReportType reportType, String path)
        {
            XML xml = new XML();
            xml.IntializeXML("users");

            switch (reportType)
            {
                case ReportType.user:
                    {
                        UsersReport(xml, ref path);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }

            xml.AppendHashNode();

            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(MessengerLib.Util.Encoder.EncryptMessage(xml.ToString()));
            }
        }

        private void UsersReport(XML xml, ref String path)
        {
            IList<MSNUser> users = TCPConnection.GetListUsers(false);

            foreach (MSNUser user in users)
            {
                XmlNode group = xml.CreateElement("user");

                xml.InsertIntoGroup(group, "name", user.Name);
                xml.InsertIntoGroup(group, "login", user.Login);
                xml.InsertIntoGroup(group, "blocked", user.Blocked.ToString());
                xml.InsertIntoGroup(group, "timeAlert", user.TimeAlert.ToString());
                xml.InsertIntoGroup(group, "expiration", user.ExpirationString);
                xml.InsertIntoGroup(group, "unblockDate", user.UnblockDateString);
                xml.AppendNode(group);
            }

            if (String.IsNullOrEmpty(path))
            {
                path = "users.xml";
            }
        }
    }
}
