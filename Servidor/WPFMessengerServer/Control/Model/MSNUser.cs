using System;

namespace WPFMessengerServer.Control.Model
{
    public class MSNUser
    {
        public Boolean Blocked { get; set; }
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
                    return String.Format("'{0}'", date.Value.ToString(MessengerLib.Config.DateFormatMySQL));
                }
                else
                {
                    return String.Format(MessengerLib.Config.DateFormat, date);
                }

            }
            else
            {
                return (mySQL ? "null" : String.Empty);
            }
        }

    }
}
