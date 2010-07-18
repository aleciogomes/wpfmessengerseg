using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNFeature
    {

        public IList<MessengerLib.Core.MSNFeature> GetList(int userID)
        {
            IList<MessengerLib.Core.MSNFeature> list = new List<MessengerLib.Core.MSNFeature>();

            lock (DBUtil.lockBD)
            {
                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT REC.cd_recurso, REC.ds_recurso, OP.cd_operacao, OP.ds_operacao FROM ");
                    sql.Append(" Recurso as REC ");
                    sql.Append(" inner join Operacao as OP on ( OP.cd_recurso = REC.cd_recurso ) ");
                    sql.Append(" inner join Permissao as PER on ( PER.cd_operacao = OP.cd_operacao ) ");
                    sql.Append(" where cd_usuario = {0} ");

                    Object[] sqlParams = new Object[] { userID };

                    command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    MessengerLib.Core.MSNFeature feature = null;
                    MessengerLib.Core.MSNOperation operation = null;

                    int featureID = 0;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            featureID = reader.GetInt32("cd_recurso");

                            //nova feature durante o looping
                            if (feature == null || featureID != feature.ID)
                            {
                                feature = new MessengerLib.Core.MSNFeature();
                                feature.ID = featureID;
                                feature.Name = MessengerLib.FeatureHandler.GetFeature(reader.GetString("ds_recurso"));

                                list.Add(feature);
                            }

                            operation = new MessengerLib.Core.MSNOperation();
                            operation.ID = reader.GetInt32("cd_operacao"); ;
                            operation.Name = MessengerLib.OperationHandler.GetOperation(reader.GetString("ds_operacao"));

                            feature.ListOperation.Add(operation);
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

    }
}
