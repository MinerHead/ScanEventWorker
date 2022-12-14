using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventWorker.Stores
{
    internal class WorkerLogStore
    {
        internal static void Log(int logTypeId, string msg)
        {
            //log message and log type with current time
            string sql = @"INSERT INTO WorkerLog(LogDateTimeUtc, LogTypeId, Details) VALUES (@0, @1, @2)";
            DatabaseAccessHelper.ExecuteSql(sql, DateTime.UtcNow, logTypeId, msg);
        }

        internal static void LogWithLocalTime(int logTypeId, string msg, DateTime time)
        {
            //log message and log type with given time
            string sql = @"INSERT INTO WorkerLog(LogDateTimeUtc, LogTypeId, Details) VALUES (@0, @1, @2)";
            DatabaseAccessHelper.ExecuteSql(sql, time.ToUniversalTime(), logTypeId, msg);
        }
    }
}
