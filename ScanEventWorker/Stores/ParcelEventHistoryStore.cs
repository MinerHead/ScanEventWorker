using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    internal class ParcelEventHistoryStore
    {
        internal static void InsertParcelEventHistory(int parcelId, string type, DateTime createdTime)
        {
            // check if given type needs to be logged in ParcelEventHistory table
            var typeId = EventTypeStore.GetTypeIdByType(type, true);
            if (typeId > 0)
            {// needs to be logged
                string sql = @"INSERT INTO ParcelEventHistory(ParcelId, TypeId, CreatedDateTimeUtc)
                                VALUES (@0, @1, @2)";
                DatabaseAccessHelper.ExecuteSql(sql, parcelId, typeId, createdTime.ToUniversalTime());
            }
        }
    }
}
