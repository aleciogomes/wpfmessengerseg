using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNPermission
    {

        public void Insert(Model.MSNUser user, string operation)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO Permissao ");
            sql.Append(" (cd_usuario, cd_operacao) ");
            sql.Append(" SELECT {0}, cd_operacao FROM Operacao WHERE ds_operacao = '{1}' limit 1 ");

            Object[] sqlParams = new Object[]{
                user.ID, operation
            };

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

        public void Delete(Model.MSNUser user)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" DELETE FROM Permissao ");
            sql.Append(" WHERE cd_usuario = {0} ");

            Object[] sqlParams = new Object[] { user.ID };

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

    }
}
