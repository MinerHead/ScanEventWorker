using Microsoft.Data.Sqlite;
using ScanEventWorker.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScanEventWorker
{
    internal class DatabaseAccessHelper
    {
        internal static SqliteConnection CreateConnection()
        {
            SqliteConnection sqliteConn = new SqliteConnection("Data Source=offlineDB.db");
            try
            {
                sqliteConn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                WorkerLogStore.Log(ConstantHelper.ERROR_LOG_TYPE_ID, ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return sqliteConn;
        }

        internal static void InitializeDatabase()
        {
            SqliteConnection conn = CreateConnection();
            CheckAndCreateTable(conn);
            InitializeLookupData(conn);
            conn.Close();
        }

        internal static void CheckAndCreateTable(SqliteConnection conn)
        {

            SqliteCommand sqliteCMD = conn.CreateCommand();
            sqliteCMD.CommandText = "CREATE TABLE IF NOT EXISTS Parcel(ParcelId INT NOT NULL UNIQUE, EventId INT NOT NULL UNIQUE, TypeId INT, CreatedDateTimeUtc DATETIME, StatusCode VARCHAR(20), RunId VARCHAR(20))";
            sqliteCMD.ExecuteNonQuery();
            sqliteCMD.CommandText = "CREATE TABLE IF NOT EXISTS ParcelEventHistory(ParcelId INT NOT NULL, TypeId INT NOT NULL, CreatedDateTimeUtc DATETIME)";
            sqliteCMD.ExecuteNonQuery();
            sqliteCMD.CommandText = "CREATE TABLE IF NOT EXISTS EventType(TypeId INTEGER PRIMARY KEY AUTOINCREMENT, Type VARCHAR(20) NOT NULL UNIQUE, IsLogged INT DEFAULT 0 NOT NULL)";
            sqliteCMD.ExecuteNonQuery();
            sqliteCMD.CommandText = "CREATE TABLE IF NOT EXISTS WorkerConfig(Id INT NOT NULL UNIQUE, EventId INT NOT NULL, LimitSize INT DEFAULT 100 NOT NULL, AllowNewType INT DEFAULT 0 NOT NULL, ApiTime INT DEFAULT 10000 NOT NULL)";
            sqliteCMD.ExecuteNonQuery();
            sqliteCMD.CommandText = "CREATE TABLE IF NOT EXISTS WorkerLog(LogId INTEGER PRIMARY KEY AUTOINCREMENT, LogDateTimeUtc DATETIME NOT NULL, LogTypeId INT NOT NULL, Details VARCHAR(20))";
            sqliteCMD.ExecuteNonQuery();
            sqliteCMD.CommandText = "CREATE TABLE IF NOT EXISTS LogType(LogTypeId INTEGER PRIMARY KEY AUTOINCREMENT, LogType VARCHAR(20) NOT NULL UNIQUE)";
            sqliteCMD.ExecuteNonQuery();
        }

        internal static void InitializeLookupData(SqliteConnection conn)
        {
            SqliteCommand sqliteCMD = conn.CreateCommand();
            sqliteCMD.CommandText = "INSERT OR IGNORE INTO EventType (Type ,IsLogged) VALUES ('PICKUP', 1), ('DELIVERY', 1), ('STATUS', 0) ";
            sqliteCMD.ExecuteNonQuery();
            sqliteCMD.CommandText = "INSERT OR IGNORE INTO WorkerConfig (Id ,EventId, AllowNewType) VALUES (1, 0, 1)";
            sqliteCMD.ExecuteNonQuery();
            sqliteCMD.CommandText = "INSERT OR IGNORE INTO LogType (LogType) VALUES ('INFO'), ('WARNING'), ('ERROR'), ('OTHER')";
            sqliteCMD.ExecuteNonQuery();
        }

        internal static void ExecuteSql(string sql, params object[] list)
        {
            SqliteConnection conn = CreateConnection();
            SqliteCommand sqliteCMD = conn.CreateCommand();
            sqliteCMD.CommandText = sql;
            for (int i = 0; i < list.Length; i++)
            {
                sqliteCMD.Parameters.AddWithValue($"@{i}", list[i]);
            }
            sqliteCMD.ExecuteNonQuery();
            conn.Close();
        }

        internal static object? ExecuteScalar(string sql, params object[] list)
        {
            SqliteConnection conn = CreateConnection();
            SqliteCommand sqliteCMD = conn.CreateCommand();
            sqliteCMD.CommandText = sql;
            for (int i = 0; i < list.Length; i++)
            {
                sqliteCMD.Parameters.AddWithValue($"@{i}", list[i]);
            }
            var res = sqliteCMD.ExecuteScalar();
            conn.Close();
            return res;
        }

        internal static WorkerConfig ReadIntoWorkerConfig(string sql)
        {
            WorkerConfig config = new WorkerConfig();
            SqliteConnection conn = CreateConnection();
            SqliteCommand sqliteCMD = conn.CreateCommand();
            sqliteCMD.CommandText = sql;
            SqliteDataReader reader = sqliteCMD.ExecuteReader();
            while (reader.Read())
            {
                config.EventId = reader.GetInt32(0);
                config.LimitSize = reader.GetInt32(1);
                config.AllowNewType = reader.GetBoolean(2);
                config.ApiTime = reader.GetInt32(3);
            }
            conn.Close();
            return config;
        }

        
        
    }
}
