using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    class MSNResource
    {
        private Object lockBD;

        public MSNResource()
        {
            this.lockBD = new Object();
        }

        public IList<Model.MSNResource> GetList()
        {
            IList<Model.MSNResource> list = new List<Model.MSNResource>();

            lock (lockBD)
            {

                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT cd_recurso, ds_recurso FROM recurso ");
                    command = new MySqlCommand(sql.ToString(), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    Model.MSNResource resource = null;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            resource = new Model.MSNResource();
                            resource.Codigo = reader.GetInt32("cd_recurso");
                            resource.Descricao = reader.GetString("ds_recurso");
                            
                            list.Add(resource);
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

        public Model.MSNResource Get(int codigo)
        {

            lock (lockBD)
            {

                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT ds_recurso FROM recurso WHERE cd_recurso = {0}");

                    Object[] sqlParams = new Object[] { codigo };

                    command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Model.MSNResource resource = new Model.MSNResource();

                        resource.Codigo = codigo;
                        resource.Descricao = reader.GetString("ds_recurso");

                        return resource;
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

        public void Update(int codigo, string newDescricao)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE recurso SET ");
            sql.Append(" ds_recurso  = '{0}' ");
            sql.Append(" WHERE cd_recurso = {1} ");

            Object[] sqlParams = new Object[]{
                newDescricao,
                codigo
            };

            DBUtil.Instance.openConnection();
            DBUtil.Instance.executeQuery(String.Format(sql.ToString(), sqlParams));
            DBUtil.Instance.closeConnection();
        }
    }
}
