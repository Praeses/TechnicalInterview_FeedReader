namespace CreateDatabase
{
    using System;
    using System.Data.SqlClient;
    using System.IO;

    internal class Program
    {
        #region Methods

        private static void Main(string[] args)
        {
            // Set up the localdb data directory.
            string dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dataDirectory = Path.Combine(dataDirectory, "..\\..\\..\\Database");
            dataDirectory = Path.GetFullPath(dataDirectory);
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            string databaseFile = Path.Combine(dataDirectory, "FeedReader.mdf");
            Console.WriteLine(databaseFile);
            if (File.Exists(databaseFile))
            {
                Console.WriteLine("Database already exists.");
            }

            string scriptFile = Path.Combine(dataDirectory, "feedReader.sql");
            Console.WriteLine(scriptFile);
            if (!File.Exists(scriptFile))
            {
                Console.WriteLine("Cannot find script.");
            }

            //Process the script file.
            string sqlScript = File.ReadAllText(scriptFile);
            sqlScript = sqlScript.Replace("GO\r\n", "~");
            string[] sqlCommands = sqlScript.Split('~');

            SqlConnection sqlConnection = null;
            foreach (string sql in sqlCommands)
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    continue;
                }

                Console.WriteLine(sql);
                if (sql.StartsWith("USE ["))
                {
                    int start = sql.IndexOf('[') + 1;
                    int stop = sql.IndexOf(']');
                    string catalog = sql.Substring(start, stop - start);
                    string connectionString =
                        string.Format(
                            "Data Source=(LocalDB)\\v11.0;Initial Catalog={0};Integrated Security=True",
                            catalog);
                    if (sqlConnection != default(SqlConnection))
                    {
                        sqlConnection.Close();
                    }

                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    continue;
                }

                using (var sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }

            Console.WriteLine("done.");

            if (sqlConnection == default(SqlConnection))
            {
                return;
            }

            sqlConnection.Close();
            sqlConnection.Dispose();
        }

        #endregion
    }
}