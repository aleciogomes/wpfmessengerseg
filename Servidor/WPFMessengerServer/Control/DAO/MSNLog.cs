using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNLog
    {
        public IList<DateTime> GetDateList()
        {
            IList<DateTime> list = new List<DateTime>();

            lock (DBUtil.lockBD)
            {
                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT dt_auditoria FROM auditoria GROUP BY dt_auditoria");

                    command = new MySqlCommand(sql.ToString(), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            list.Add(DateTime.Parse(reader.GetString("dt_auditoria")));
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

        public void Insert(MessengerLib.Core.MSNLog log )
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO Auditoria ");
            sql.Append(" (dt_auditoria, ds_auditoria) ");
            sql.Append(" VALUES ");
            sql.Append(" ( '{0}', '{1}' )");

            Object[] sqlParams = new Object[]{
                log.Date.ToString(MessengerLib.Config.DateFormatMySQL), log.Event
            };

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

    }
}
