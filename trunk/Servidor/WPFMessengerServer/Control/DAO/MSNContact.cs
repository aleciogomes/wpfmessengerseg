using System;
using System.Text;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNContact
    {

        public void Insert(Model.MSNUser user)
        {
            StringBuilder sql = null;
            Object[] sqlParams = null;

            foreach (int id in user.ListNewContacts)
            {
                sql = new StringBuilder();
                sql.Append(" INSERT INTO contato ");
                sql.Append(" (cd_usuario, cd_contato) ");
                sql.Append(" VALUES ");
                sql.Append(" ( {0}, {1} )");

                sqlParams = new Object[] { user.ID, id };

                DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
            }
        }


    }
}
