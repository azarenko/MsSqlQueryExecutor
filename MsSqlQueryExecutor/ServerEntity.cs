
namespace MsSqlQueryExecutor
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text;

    public class ServerEntity
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;
        private string connectionString;

        private string serverName;
        private List<string> databaseNames = new List<string>();

        public ServerEntity(string ConnectionString)
        {
            connectionString = ConnectionString;
            connection = new SqlConnection(connectionString);
            connection.Open();

            serverName = connection.DataSource;

            command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM master.dbo.sysdatabases";
            command.CommandType = System.Data.CommandType.Text;

            reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    databaseNames.Add((string)reader[0]);
                }
            }

            connection.Close();
        }

        public List<string> DatabaseNames
        {
            get
            {
                return databaseNames;
            }
        }

        public string ServerName
        {
            get
            {
                return serverName;
            }
        }

        public string ExecuteText(string Database, string Query)
        {
            StringBuilder sb = new StringBuilder();

            connection = new SqlConnection(connectionString);
            connection.Open();
            connection.ChangeDatabase(Database);

            serverName = connection.DataSource;

            command = connection.CreateCommand();
            command.CommandText = Query;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 0;

            try
            {
                reader = command.ExecuteReader();
            }
            catch(SqlException exp)
            {
                connection.Close();
                return exp.Message;
            }

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        sb.AppendFormat("{0}\t", reader[i]);
                    }
                    sb.AppendLine();
                }
            }

            connection.Close();

            return sb.ToString();
        }
    }
}
