using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    public class ScanEvent
    {
        [property: JsonPropertyName("EventId")]
        public int EventId { get; set; }
        [property: JsonPropertyName("ParcelId")]
        public int ParcelId { get; set; }
        [property: JsonPropertyName("Type")]
        public string Type { get; set; }
        [property: JsonPropertyName("CreatedDateTimeUtc")]
        public DateTime CreatedDateTimeUtc { get; set; }
        [property: JsonPropertyName("StatusCode")]
        public string StatusCode { get; set; }
        [property: JsonPropertyName("Device")]
        public Device Device { get; set; }
        [property: JsonPropertyName("User")]
        public User User { get; set; }
    }

    public class Device
    {
        [property: JsonPropertyName("DeviceTransactionId")]
        public int DeviceTransactionId { get; set; }
        [property: JsonPropertyName("DeviceId")]
        public int DeviceId { get; set; }
    }

    public class User
    {
        [property: JsonPropertyName("UserId")]
        public string UserId { get; set; }
        [property: JsonPropertyName("CarrierId")]
        public string CarrierId { get; set; }
        [property: JsonPropertyName("RunId")]
        public string RunId { get; set; }
    }

    public record class ScanEventResponse
    {
        [property: JsonPropertyName("ScanEvents")]
        public List<ScanEvent> ScanEvents { get; set; }
    }

    internal class ScanEventStore
    {
        internal static void InsertScanEvent(ScanEvent scanEvent)
        {
            var typeId = EventTypeStore.GetTypeIdByType(scanEvent.Type);
            if (typeId > 0)
            {
                string runId = scanEvent.User?.RunId;
                string sql = @"INSERT INTO Parcel(ParcelId, EventId, TypeId, CreatedDateTimeUtc, StatusCode, RunId)
                                VALUES (@0, @1, @2, @3, @4, @5)";
                DatabaseAccessHelper.ExecuteSql(sql, scanEvent.ParcelId, scanEvent.EventId, typeId, scanEvent.CreatedDateTimeUtc.ToUniversalTime(), scanEvent.StatusCode, runId);
            }
        }

        internal static void RemoveScanEventByParcelId(int parcelId)
        {
            string sql = @"DELETE FROM Parcel WHERE ParcelId = @0";
            DatabaseAccessHelper.ExecuteSql(sql, parcelId);
        }

        internal static bool CheckParcelIdWithLaterDateExist(int parcelId, DateTime createdDatetime)
        {
            string sql = @"SELECT count(parcelId) FROM Parcel WHERE ParcelId = @0 AND CreatedDateTimeUtc >= @1";
            var res = DatabaseAccessHelper.ExecuteScalar(sql, parcelId, createdDatetime.ToUniversalTime());
            if (res == null)
            {
                return true;
            }
            else
            {
                return (long)res > 0;
            }
            
        }
    }
}
