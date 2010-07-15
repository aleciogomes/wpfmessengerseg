using MessengerLib;
using WPFMessengerServer.Control;
using System.Collections;
using System.Collections.Generic;

namespace WPFMessengerServer
{
    public static class Util
    {
        private static Control.DAO.MSNUser dao = new Control.DAO.MSNUser();

        public static bool IsValid(string user, string password)
        {
            if (dao.Get(user, password) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IList<Control.Model.MSNUser> GetUsers()
        {
            return dao.GetList();
        }

    }
}
