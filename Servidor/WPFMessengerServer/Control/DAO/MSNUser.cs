using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNUser
    {

        private Object lockBD;

        public MSNUser()
        {
            this.lockBD = new Object();
        }

        public IList<Model.MSNUser> GetList()
        {

            IList<Model.MSNUser> list = new List<Model.MSNUser>();

            lock (lockBD)
            {

                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT ");
                    sql.Append(" ds_login, nm_usuario, ds_pwhash, dt_validade, nr_prazoAlerta, fl_bloqueada, dt_liberacaoBloqueio ");
                    sql.Append(" FROM usuario ");
                    command = new MySqlCommand(sql.ToString(), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    Model.MSNUser user = null;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            user = new Model.MSNUser();
                            user.Login = reader.GetString("ds_login");
                            user.Password = reader.GetString("ds_pwhash");
                            user.Name = reader.GetString("nm_usuario");

                            try
                            {
                                user.Expiration = DateTime.Parse(reader.GetString("dt_validade"));
                            }
                            catch { }

                            try
                            {
                                user.UnblockDate = DateTime.Parse(reader.GetString("dt_liberacaoBloqueio"));
                            }
                            catch { }

                            try
                            {
                                user.TimeAlert = int.Parse(reader.GetString("nr_prazoAlerta"));
                            }
                            catch { }

                            user.Blocked = Convert.ToBoolean(int.Parse(reader.GetString("fl_bloqueada")));

                            list.Add(user);
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro ao executar query: " + e.Message);
                    throw new Exception(e.Message, e);
                }
                finally
                {
                    DBUtil.Instance.closeConnection();
                }
            }

            return list;
        }

        public Model.MSNUser Get(string login, string password)
        {

            lock (lockBD)
            {

                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT nm_usuario, ds_pwhash, dt_validade, nr_prazoAlerta, fl_bloqueada, dt_liberacaoBloqueio FROM usuario WHERE ds_login = '{0}'");

                    Object[] sqlParams = null;

                    if (!String.IsNullOrEmpty(password))
                    {
                        sql.Append(" and ds_pwhash = '{1}' ");
                        sqlParams = new Object[] { login, password };
                    }
                    else
                    {
                        sqlParams = new Object[] { login };
                    }

                    command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();


                    if (reader.Read())
                    {
                        Model.MSNUser user = new Model.MSNUser();

                        user.Login = login;
                        user.Password = reader.GetString("ds_pwhash"); ;
                        user.Name = reader.GetString("nm_usuario");

                        try
                        {
                            user.Expiration = DateTime.Parse(reader.GetString("dt_validade"));
                        }
                        catch{}

                        try
                        {
                            user.UnblockDate = DateTime.Parse(reader.GetString("dt_liberacaoBloqueio"));
                        }
                        catch { }

                        try
                        {
                            user.TimeAlert = int.Parse(reader.GetString("nr_prazoAlerta"));
                        }
                        catch { }

                        user.Blocked = Convert.ToBoolean(int.Parse(reader.GetString("fl_bloqueada")));

                        return user;
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro ao executar query: " + e.Message);
                    throw new Exception(e.Message, e);
                }
                finally
                {
                    DBUtil.Instance.closeConnection();
                }
            }

            return null;
        }


        public void Update(string user, Model.MSNUser msnUser)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE Usuario SET ");
            sql.Append(" ds_login  = '{0}' ");
            sql.Append(",nm_usuario = '{1}' ");
            sql.Append(",ds_pwhash = '{2}' ");
            sql.Append(" WHERE ds_login = '{3}' ");

            Object[] sqlParams = new Object[]{
                msnUser.Login,
                msnUser.Name,
                msnUser.Password,
                user
            };

            this.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

        public void UpdateOtherInfo(Model.MSNUser msnUser)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE Usuario SET ");
            sql.Append(" dt_validade  = {0} ");
            sql.Append(",nr_prazoAlerta = '{1}' ");
            sql.Append(",fl_bloqueada = '{2}' ");
            sql.Append(",dt_liberacaoBloqueio = {3} ");
            sql.Append(" WHERE ds_login = '{4}' ");

            int blocked = (msnUser.Blocked ? 1 : 0);

            Object[] sqlParams = new Object[]{
                msnUser.ExpirationString(true),
                msnUser.TimeAlert,
                blocked,
                msnUser.UnblockDateString(true),
                msnUser.Login
            };

            this.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

        public void Insert(Model.MSNUser user)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO Usuario ");
            sql.Append(" (ds_login, nm_usuario, ds_pwhash, dt_validade, nr_prazoAlerta, fl_bloqueada, dt_liberacaoBloqueio) ");
            sql.Append(" VALUES ");
            sql.Append(" ( '{0}', '{1}', '{2}', {3}, '{4}', '{5}', {6} )");

            int blocked = (user.Blocked ? 1 : 0);

            Object[] sqlParams = new Object[]{
                user.Login,
                user.Name,
                user.Password,
                user.ExpirationString(true),
                user.TimeAlert,
                blocked,
                user.UnblockDateString(true),
            };

            this.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }


        public void Delete(string user)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" DELETE FROM Usuario ");
            sql.Append(" WHERE ds_login = '{0}' ");

            Object[] sqlParams = new Object[] { user };

            this.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

        private void ExecQuery(string sql)
        {
            lock (lockBD)
            {
                DBUtil.Instance.openConnection();
                DBUtil.Instance.executeQuery(sql);
                DBUtil.Instance.closeConnection();
            }
        }

        public Model.MSNUser GetContact(string user)
        {
            return this.Get(user, String.Empty);
        }

    }
}
