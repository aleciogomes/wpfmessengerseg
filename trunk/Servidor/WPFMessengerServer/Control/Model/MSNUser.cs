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
        public string UnblockDateString
        {
            get
            {
                if (UnblockDate.HasValue)
                {
                    return String.Format("'{0}'", UnblockDate.Value.ToString(MessengerLib.Config.DateFormatMySQL));
                }
                else
                {
                    return "null";
                }
            }
        }


        public DateTime? Expiration;
        public string ExpirationString
        {
            get
            {
                if (Expiration.HasValue)
                {
                    return String.Format("'{0}'", Expiration.Value.ToString(MessengerLib.Config.DateFormatMySQL));
                }
                else
                {
                    return "null";
                }
            }

        }

    }
}
