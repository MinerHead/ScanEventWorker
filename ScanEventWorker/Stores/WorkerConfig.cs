using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    internal class WorkerConfig
    {
        internal int EventId { get; set; }
        internal int LimitSize { get; set; }
        internal bool AllowNewType { get; set; }
        internal int ApiTime { get; set; }

    }

    internal class WorkerConfigStore
    {
        internal static void UpdateLastProcessedEventId(int eventId)
        {
            string sql = @"UPDATE WorkerConfig SET EventId = @0 WHERE EventId < @0";
            DatabaseAccessHelper.ExecuteSql(sql, eventId);
        }

        internal static void GetWorkerConfig()
        {
            string sql = @"SELECT EventId, LimitSize, AllowNewType, ApiTime FROM WorkerConfig LIMIT 1";
            var config = DatabaseAccessHelper.ReadIntoWorkerConfig(sql);
            ConstantHelper.LIMIT = config.LimitSize;
            ConstantHelper.ALLOW_NEW_TYPE = config.AllowNewType;
            ConstantHelper.INIT_EVENT_ID = config.EventId + 1;
            ConstantHelper.API_TIME = config.ApiTime;
            
        }

    }
}
