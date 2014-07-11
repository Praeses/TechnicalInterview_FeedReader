namespace CreateDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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

            string scriptFile = Path.Combine(dataDirectory, "feedReader.sql");
            Console.WriteLine(scriptFile);
            if (!File.Exists(scriptFile))
            {
                Console.WriteLine("Cannot find script.");
                return;
            }

            string databaseFile = Path.Combine(dataDirectory, "FeedReader.mdf");
            Console.WriteLine(databaseFile);
            if (File.Exists(databaseFile))
            {
                Console.WriteLine("Database already exists!");
                Console.WriteLine("Do you want to overwrite it? (y/n)");
                string verify = Console.ReadLine();
                if (verify != "y")
                {
                    return;
                }
            }

            //Process the script file.
            string sqlScript = File.ReadAllText(scriptFile);
            sqlScript = sqlScript.Replace("GO\r\n", "~");
            string[] sqlCommands = sqlScript.Split('~');

            var existingDatabases = new List<string>();
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
                    try
                    {
                        sqlConnection.Open();
                        if (!existingDatabases.Contains(catalog))
                        {
                            existingDatabases.Add(catalog);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Write("*****");
                        Console.WriteLine(e.Message);
                    }

                    continue;
                }

                if ((sqlConnection == default(SqlConnection)) || (sqlConnection.State == ConnectionState.Closed))
                {
                    continue;
                }


                if (sql.Contains("ALTER DATABASE ") && sql.Contains("DISABLE_BROKER"))
                {
                    continue;
                }

                if (sql.Contains("ALTER DATABASE ") && sql.Contains("DATE_CORRELATION_OPTIMIZATION"))
                {
                    continue;
                }

                if (sql.Contains("ALTER DATABASE ") && sql.Contains("FILESTREAM"))
                {
                    continue;
                }

                if (sql.Contains("DROP DATABASE "))
                {
                    continue;
                }

                start = sql.IndexOf("CREATE DATABASE [", StringComparison.Ordinal);
                if (start != -1)
                {
                    start += 17;
                    int stop = sql.IndexOf(']', start);
                    string catalog = sql.Substring(start, stop - start);
                    if (existingDatabases.Contains(catalog))
                    {
                        continue;
                    }
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
                    try
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.Write("*****");
                        Console.WriteLine(e.Message);
                    }
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