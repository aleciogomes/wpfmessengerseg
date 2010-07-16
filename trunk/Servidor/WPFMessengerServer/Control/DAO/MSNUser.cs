using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNUser
    {

        public IList<Model.MSNUser> GetList()
        {
            IList<Model.MSNUser> list = new List<Model.MSNUser>();

            DBUtil.Instance.openConnection();
            MySqlCommand command    = null;
            MySqlDataReader reader  = null;
            StringBuilder sql       = new StringBuilder();

            try
            {
                sql.Append(" SELECT ds_login, nm_usuario, ds_pwhash, dt_validade, nr_prazoAlerta, fl_bloqueada, dt_liberacaoBloqueio FROM usuario ");
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
                        catch
                        {
                            user.Expiration = null;
                        }

                        try
                        {
                            user.BlockedDate = DateTime.Parse(reader.GetString("dt_liberacaoBloqueio"));
                        }
                        catch
                        {
                            user.BlockedDate = null;
                        }

                        try
                        {
                            user.TimeAlert = int.Parse(reader.GetString("nr_prazoAlerta"));
                        }
                        catch
                        {
                            user.TimeAlert = 0;
                        }

                        user.Blocked = Convert.ToBoolean( int.Parse(reader.GetString("fl_bloqueada")));
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

            return list;
        }

        public Model.MSNUser Get(string login, string password)
        {

            DBUtil.Instance.openConnection();
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append(" SELECT nm_usuario, dt_validade, nr_prazoAlerta, fl_bloqueada, dt_liberacaoBloqueio FROM usuario WHERE ds_login = '{0}'");

                Object[] sqlParams = null;

                if (!String.IsNullOrEmpty(password))
                {
                    sql.Append(" and ds_pwhash = '{1}' ");
                    sqlParams = new Object[] { login, password };
                }
                else
                {
                    sqlParams = new Object[]{ login};
                }

                command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                reader = command.ExecuteReader();


                if (reader.Read())
                {
                    Model.MSNUser user = new Model.MSNUser();

                    user.Login = login;
                    user.Password = password;
                    user.Name = reader.GetString("nm_usuario");

                    try
                    {
                        user.Expiration = DateTime.Parse(reader.GetString("dt_validade"));
                    }
                    catch
                    {
                        user.Expiration = null;
                    }

                    try
                    {
                        user.BlockedDate = DateTime.Parse(reader.GetString("dt_liberacaoBloqueio"));
                    }
                    catch
                    {
                        user.BlockedDate = null;
                    }

                    try
                    {
                        user.TimeAlert = int.Parse(reader.GetString("nr_prazoAlerta"));
                    }
                    catch
                    {
                        user.TimeAlert = 0;
                    }

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

            return null;
        }


        public Model.MSNUser GetContact(string user)
        {
            return this.Get(user, String.Empty);
        }
    }
}
