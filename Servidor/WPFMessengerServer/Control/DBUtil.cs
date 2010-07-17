using System;
using MySql.Data.MySqlClient;

namespace WPFMessengerServer.Control
{
    public class DBUtil
    {
        private static DBUtil instance;
        public MySqlConnection Connection { get; set; }
        public String Database { get; set; }
        public String Host { get; set; }
        public String Port { get; set; }
        public String User { get; set; }
        public String Pass { get; set; }
        public Boolean Configured { get; set; }

        private DBUtil() {}

        public static DBUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBUtil();
                    initConfiguration("localhost", "3306", "root", "", "wpfmess");
                }

                return instance;
            }
        }

        public static void initConfiguration(String host, String port, String user, String pass, String database)
        {
            instance.Host = host;
            instance.Port = port;
            instance.User = user;
            instance.Pass = pass;
            instance.Database = database;

            Object[] connectionParams = new Object[] { host, port, database, user, pass };
            String url = String.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};", connectionParams);
            instance.Connection = new MySqlConnection(url);
        }

        public void openConnection()
        {
            try
            {
                if (instance.Connection != null)
                {
                    instance.Connection.Open();
                }
            }
            catch
            {
                Console.WriteLine("Erro ao abrir conexao com o banco");
            }
        }

        public void closeConnection()
        {
            try
            {
                if (instance.Connection != null)
                {
                    instance.Connection.Close();
                }
            }
            catch
            {
                Console.WriteLine("Erro ao fechar conexao com o banco");
            }
        }

        public void testConnection(bool exibirMsgSucesso)
        {
            try
            {
                if (instance.Connection != null)
                {
                    instance.Connection.Open();
                    instance.Configured = true;
                    if (exibirMsgSucesso)
                    {
                        Console.WriteLine("Conexão estabelecida com sucesso!");
                    }
                }
                else
                {
                    Console.WriteLine("Deseja configurar sua conexão com o banco agora ?");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao abrir conexão com o banco, o servidor retornou:\n" + e.Message);

                instance.Connection = null;
                instance.Configured = false;
            }
            finally
            {
                if (instance.Connection != null)
                {
                    instance.Connection.Close();
                }
            }
        }

        public void executeQuery(String query)
        {
            MySqlCommand command = null;

            try
            {
                command = new MySqlCommand(query, instance.Connection);
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine("Erro ao executar query:\n" + e.Message);
            }
        }

        public int lastInsertedID()
        {
            MySqlCommand command = null;

            try
            {
                String query = " SELECT LAST_INSERT_ID() ";
                command = new MySqlCommand(query, instance.Connection);
                command.ExecuteNonQuery();
                return int.Parse(command.ExecuteScalar().ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao executar query:\n" + e.Message);

                return 0;
            }
        }
    }
}
