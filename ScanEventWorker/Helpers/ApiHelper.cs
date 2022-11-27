using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    internal static class ApiHelper
    {
        internal static async Task<ScanEventResponse?> AsyncGetScanEvents (int fromEventId = 1, int limit = 100)
        {
            // call API to fetch ScanEvents
            using HttpClient client = new();
            await using Stream stream = await client.GetStreamAsync(ConstantHelper.DOMAIN + ConstantHelper.URL_GET_SCANEVENTS+ $"?FromEventId={fromEventId}&Limit={limit}");
            var events =
                await JsonSerializer.DeserializeAsync<ScanEventResponse>(stream);
            return events;
        }
    }
}
