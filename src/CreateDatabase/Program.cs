namespace CreateDatabase
{
    using System;
    using System.Data.SqlClient;
    using System.IO;

    internal class Program
    {
        #region Methods

        private static void Main()
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
            foreach (string sqlCmd in sqlCommands)
            {
                string sql = sqlCmd;
                if (string.IsNullOrWhiteSpace(sql))
                {
                    continue;
                }

                Console.WriteLine(sql);

                int start;
                if (sql.StartsWith("USE ["))
                {
                    start = sql.IndexOf('[') + 1;
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

                start = sql.IndexOf("FILENAME = N'", StringComparison.Ordinal);
                while (start != -1)
                {
                    start += 13;
                    int stop = sql.IndexOf('\'', start);
                    string fileName = sql.Substring(start, stop - start);
                    sql = sql.Replace(fileName, Path.Combine(dataDirectory, Path.GetFileName(fileName)));

                    start = sql.IndexOf("FILENAME = N'", start, StringComparison.Ordinal);
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