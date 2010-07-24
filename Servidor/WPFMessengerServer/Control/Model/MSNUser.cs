using System;
using System.Collections.Generic;
using MessengerLib.Core;

namespace WPFMessengerServer.Control.Model
{
    public class MSNUser
    {
        public IList<int> ListNewContacts { get; set; }
        public IList<MSNFeature> ListFeature { get; set; }

        public String ConfigMotherBoardID { get; set; }
        public Boolean Blocked { get; set; }
        public int ID { get; set; }
        public String Name { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public int TimeAlert { get; set; }

        public DateTime? UnblockDate;
        public string UnblockDateString(bool mySQL)
        {
            return FormatDateString(UnblockDate, mySQL);
        }

        public DateTime? Expiration;
        public string ExpirationString(bool mySQL)
        {
            return FormatDateString(Expiration, mySQL);
        }

        private string FormatDateString(DateTime? date, bool mySQL)
        {
            if (date.HasValue)
            {
                if (mySQL)
                {
                    return String.Format("'{0}'", date.Value.ToString(MessengerLib.Util.Config.DateFormatMySQL));
                }
                else
                {
                    return ((DateTime)date).ToString(MessengerLib.Util.Config.DateFormat);
                }

            }
            else
            {
                return (mySQL ? "null" : String.Empty);
            }
        }

    }
}
