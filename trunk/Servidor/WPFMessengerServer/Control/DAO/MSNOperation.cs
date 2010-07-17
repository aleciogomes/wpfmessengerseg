using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    class MSNOperation
    {
        private Object lockBD;
        private DAO.MSNResource resourceRep;

        public MSNOperation()
        {
            this.lockBD = new Object();
            this.resourceRep = new DAO.MSNResource();
        }

        public IList<Model.MSNOperation> GetList()
        {
            IList<Model.MSNOperation> list = new List<Model.MSNOperation>();

            lock (lockBD)
            {
                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT cd_operacao, ds_operacao, cd_recurso FROM operacao ");
                    command = new MySqlCommand(sql.ToString(), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    Model.MSNOperation operation = null;

                    IList<int> resources = new List<int>();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            operation = new Model.MSNOperation();
                            operation.Codigo = reader.GetInt32("cd_operacao");
                            operation.Descricao = reader.GetString("ds_operacao");
                            resources.Add(reader.GetInt32("cd_recurso"));
                            
                            list.Add(operation);
                        }
                    }
                    reader.Close();

                    for (int i = 0; i < resources.Count; i++)
                    {
                        list[i].Resource = resourceRep.Get(resources[i]);
                    }
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

        public Model.MSNOperation Get(int codigo)
        {
            lock (lockBD)
            {
                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT ds_operacao, cd_recurso FROM operacao WHERE cd_operacao = {0}");

                    Object[] sqlParams = new Object[] { codigo };

                    command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Model.MSNOperation resource = new Model.MSNOperation();

                        resource.Codigo = codigo;
                        resource.Descricao = reader.GetString("ds_operacao");
                        resource.Resource = resourceRep.Get(reader.GetInt32("cd_recurso"));

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

        public void Update(int codigo, string newDescricao, Model.MSNResource resource)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE operacao SET ");
            sql.Append(" ds_operacao  = '{0}' ");
            sql.Append(" cd_recurso = {1} ");
            sql.Append(" WHERE cd_operacao = {2} ");

            Object[] sqlParams = new Object[]{
                newDescricao,
                resource.Codigo,
                codigo
            };

            DBUtil.Instance.openConnection();
            DBUtil.Instance.executeQuery(String.Format(sql.ToString(), sqlParams));
            DBUtil.Instance.closeConnection();
        }
    }
}
