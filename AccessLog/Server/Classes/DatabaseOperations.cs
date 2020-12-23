using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Server.Classes
{
    public static class DatabaseOperations
    {
        public static string ConnectionString = @"Data Source=.,1433;Initial Catalog=AccessLog;User ID=sa;Password=YourPassword";

        public static void ConnectionDatabase(string connectionString, List<AccessLogData> data)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                Console.WriteLine("\n\nConnection with database open");
                Console.WriteLine("Create database...");

                SqlCommand cmd = new SqlCommand(CreateDatabase(), connection);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand(CreateTable(), connection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Creating table...");

                Console.WriteLine("Save dates in database... Please wait.");
                InsertAccessLog(connection, data);

                Console.WriteLine("Finish saved");
                connection.Close();
                Console.WriteLine("\n\nConnection close!");
            }
            catch (SqlException error)
            {
                Console.WriteLine($"Error connection: {error}");
            }
        }

        public static string CreateTable()
        {
            return @"
                    DROP TABLE IF EXISTS [AccessLog].[dbo].[AccessLog];
                    CREATE TABLE AccessLog (
	                    id int IDENTITY(1,1) PRIMARY KEY,
	                    timeComplete varchar(400),
	                    durationInMilliseconds bigint,
	                    clientRequest varchar(100),
	                    actionProxy varchar(60),
	                    requestSizeInBytes int,
	                    requestMethod varchar(100),
	                    urlRquest varchar(600),
	                    clientName varchar(100),
	                    proxyRequest varchar(100),
	                    fileDownload varchar(100)
                    );";
        }

        public static string CreateDatabase()
        {
            return @"IF DB_ID('AccessLog') IS NULL
                     CREATE DATABASE AccessLog";
        }

        public static void InsertAccessLog(SqlConnection connection, List<AccessLogData> data)
        {

            string query = string.Empty;
            SqlCommand cmd;
            foreach (var item in data)
            {
                query = @$"INSERT INTO [AccessLog].[dbo].[AccessLog] VALUES(
                    '{item.Time}',
                    {item.DurationInMilliseconds},
                    '{item.ClientRequest}',
                    '{item.ActionProxy}',    
                    {item.RequestSizeInBytes},
                    '{item.RequestMethod}',
                    '{item.UrlRequest}',
                    '{item.ClientName}',
                    '{item.ProxyRequest}',
                    '{item.FileDownload}');";

                cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
        }
    }
}