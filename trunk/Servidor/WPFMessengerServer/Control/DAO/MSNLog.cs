using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control.DAO
{
    public class MSNLog
    {

        public IList<MessengerLib.Core.MSNLog> GetList(DateTime date)
        {

            IList<MessengerLib.Core.MSNLog> list = new List<MessengerLib.Core.MSNLog>();

            lock (DBUtil.lockBD)
            {

                DBUtil.Instance.openConnection();
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                StringBuilder sql = new StringBuilder();

                try
                {
                    sql.Append(" SELECT ");
                    sql.Append(" dt_auditoria, ds_auditoria");
                    sql.Append(" FROM Auditoria ");
                    sql.Append(" WHERE dt_auditoria between '{0} 00:00:00' and '{1} 23:59:59' ");

                    Object[] sqlParams = new Object[]{
                         date.ToString(MessengerLib.Config.DateFormatMySQL)
                        ,date.ToString(MessengerLib.Config.DateFormatMySQL)
                    };

                    command = new MySqlCommand(String.Format(sql.ToString(), sqlParams), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    MessengerLib.Core.MSNLog log = null;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            log = new MessengerLib.Core.MSNLog();

                            try
                            {
                                log.Date = DateTime.Parse(reader.GetString("dt_auditoria"));
                            }
                            catch { }

                            log.Event = reader.GetString("ds_auditoria");

                            list.Add(log);
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
                    sql.Append(" SELECT DATE(dt_auditoria) as data FROM auditoria GROUP BY DATE(dt_auditoria)");

                    command = new MySqlCommand(sql.ToString(), DBUtil.Instance.Connection);
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            list.Add(DateTime.Parse(reader.GetString("data")));
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
                log.Date.ToString(MessengerLib.Config.DateFormatMySQLFull), log.Event
            };

            DBUtil.ExecQuery(String.Format(sql.ToString(), sqlParams));
        }

    }
}
