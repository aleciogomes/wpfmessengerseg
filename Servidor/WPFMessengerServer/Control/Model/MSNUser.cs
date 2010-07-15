using System;

namespace WPFMessengerServer.Control.Model
{
    public class MSNUser
    {
        public Boolean Blocked { get; set; }
        public DateTime? BlockedDate { get; set; }
        public DateTime? Expiration { get; set; }
        public String Name { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public int TimeAlert { get; set; }
    }
}
