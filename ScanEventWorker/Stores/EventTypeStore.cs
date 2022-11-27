using Microsoft.Extensions.Logging;
using ScanEventWorker.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    internal class EventTypeStore
    {
        internal static int GetTypeIdByType(string type, bool checkIsLogged = false)
        {
            var upperCase = type.ToUpper();
            string sql = @"SELECT TypeId FROM EventType WHERE Type = @0";
            if (checkIsLogged)
            {
                sql += @" AND IsLogged = TRUE";
            }
            var res = DatabaseAccessHelper.ExecuteScalar(sql, upperCase);
            if (res == null)
            {
                if (checkIsLogged)
                {
                    return 0;
                }
                else if (ConstantHelper.ALLOW_NEW_TYPE)
                {
                    return InsertEventType(upperCase);
                }
                else
                {
                    WorkerLogStore.Log(ConstantHelper.WARNING_LOG_TYPE_ID, $"Cannot find Event Type '{type}'");
                    return 0;
                }
            }
            else
            {
                return int.Parse(res.ToString());
            }
        }

        internal static int InsertEventType(string upperCase)
        {
            string sql = @"INSERT INTO EventType(Type) VALUES(@0) RETURNING TypeId";
            var res = DatabaseAccessHelper.ExecuteScalar(sql, upperCase);
            if (res == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(res.ToString());
            }
        }
    }
}
