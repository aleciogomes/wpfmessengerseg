using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFMessenger.Core
{
    public class MSNUser
    {
        private string userName;
        private string userPassword;
        private int userID;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string UserPassword
        {
            get { return userPassword; }
            set { userPassword = value; }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

    }
}
