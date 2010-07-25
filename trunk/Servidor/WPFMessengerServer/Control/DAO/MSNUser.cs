using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNUser
    {

        public IList<Model.MSNUser> GetList(Model.MSNUser requester, bool onlyContacts)
        {

            IList<Model.MSNUser> list = new List<Model.MSNUser>();

            lock (DBUtil.lockBD)
            {

                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT ");
                    sql.Append(" cd_usuario, ds_login, nm_usuario, ds_pwhash, dt_validade, nr_prazoAlerta, fl_bloqueada, dt_liberacaoBloqueio, ds_configMbID, ds_puKey");
                    sql.Append(" FROM usuario ");

                    if (onlyContacts)
                    {
                        sql.Append(" WHERE EXISTS ");
                        sql.Append(" ( ");
                        sql.Append("   SELECT contato.cd_usuario from contato ");
                        sql.Append("   WHERE usuario.cd_usuario = contato.cd_contato and contato.cd_usuario = {0} ");
                        sql.Append(" ) ");

                        Object[] sqlParams = new Object[] { requester.ID };
                        command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                    }
                    else
                    {
                        command = new MySqlCommand(sql.ToString(), DBUtil.Instance.Connection);
                    }
                   
                    reader = command.ExecuteReader();

                    Model.MSNUser user = null;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            user = new Model.MSNUser();
                            user.ID = reader.GetInt32("cd_usuario");
                            user.Login = reader.GetString("ds_login");
                            user.Password = reader.GetString("ds_pwhash");
                            user.Name = reader.GetString("nm_usuario");

                            try
                            {
                                user.ConfigMotherBoardID = reader.GetString("ds_configMbID");
                            }
                            catch { }


                            try
                            {
                                user.SignaturePublicKey = reader.GetString("ds_puKey");
                            }
                            catch { }

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
                                user.TimeAlert = reader.GetInt32("nr_prazoAlerta");
                            }
                            catch { }

                            user.Blocked = Convert.ToBoolean(reader.GetInt32("fl_bloqueada"));

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

        public Model.MSNUser Get(string userLogin, string userPassword)
        {
            return this.Get("ds_login", userLogin, userPassword);
        }

        private Model.MSNUser Get(string field, string userIdentifier, string userPassword)
        {

            lock (DBUtil.lockBD)
            {

                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT cd_usuario, nm_usuario, ds_pwhash, dt_validade, nr_prazoAlerta, fl_bloqueada, dt_liberacaoBloqueio, ds_configMbID, ds_puKey FROM usuario WHERE {0} = '{1}'");

                    Object[] sqlParams = null;

                    if (!String.IsNullOrEmpty(userPassword))
                    {
                        sql.Append(" and ds_pwhash = '{2}' ");
                        sqlParams = new Object[] { field, userIdentifier, userPassword };
                    }
                    else
                    {
                        sqlParams = new Object[] { field, userIdentifier };
                    }

                    command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();


                    if (reader.Read())
                    {
                        Model.MSNUser user = new Model.MSNUser();

                        user.ID = reader.GetInt32("cd_usuario");
                        user.Login = userIdentifier;
                        user.Password = reader.GetString("ds_pwhash"); ;
                        user.Name = reader.GetString("nm_usuario");

                        try
                        {
                            user.ConfigMotherBoardID = reader.GetString("ds_configMbID");
                        }
                        catch { }

                        try
                        {
                            user.SignaturePublicKey = reader.GetString("ds_puKey");
                        }
                        catch { }

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
                            user.TimeAlert = reader.GetInt32("nr_prazoAlerta");
                        }
                        catch { }

                        user.Blocked = Convert.ToBoolean(reader.GetInt32("fl_bloqueada"));

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

        public void SaveMotherBoardID(string userLogin, string mbID)
        {
            this.SaveInfo(userLogin, "ds_configMbID", mbID);
        }

        public void SavePublicKey(string userLogin, string key)
        {
            this.SaveInfo(userLogin, "ds_puKey", key);
        }

        private void SaveInfo(string userLogin, string info, string value)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE Usuario SET ");
            sql.Append(" {0}  = '{1}' ");
            sql.Append(" WHERE ds_login = '{2}' ");

            Object[] sqlParams = new Object[]{
                info,
                value,
                userLogin
            };

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

        public void Update(string userLogin, Model.MSNUser msnUser)
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
                userLogin
            };

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
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

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
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

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }


        public void Delete(string userLogin)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" DELETE FROM Usuario ");
            sql.Append(" WHERE ds_login = '{0}' ");

            Object[] sqlParams = new Object[] { userLogin };

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

        public Model.MSNUser GetContact(string user)
        {
            return this.Get(user, String.Empty);
        }

        public Model.MSNUser GetContactByID(int userID)
        {
            return this.Get("cd_usuario", userID.ToString(), String.Empty);
        }

    }
}
