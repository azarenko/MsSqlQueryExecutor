
namespace MsSqlQueryExecutor
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;
    using System.Xml;

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

        public DataSet ExecuteTable(string Database, string Query)
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable("temp");
            ds.Tables.Add(table);

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
            catch (SqlException exp)
            {
                connection.Close();
                table.Columns.Add("Error");
                table.Rows.Add(exp.Message);
                return ds;
            }

            if (reader.HasRows)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i));
                }

                while (reader.Read())
                {
                    object[] values = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        values[i] = reader[i];
                    }
                    table.Rows.Add(values);
                }
            }

            connection.Close();
            return ds;
        }
    }
}
