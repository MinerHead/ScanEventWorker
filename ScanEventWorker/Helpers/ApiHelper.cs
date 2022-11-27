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
            using HttpClient client = new();
            await using Stream stream = await client.GetStreamAsync(ConstantHelper.DOMAIN + ConstantHelper.URL_GET_SCANEVENTS+ $"?FromEventId={fromEventId}&Limit={limit}");
            var events =
                await JsonSerializer.DeserializeAsync<ScanEventResponse>(stream);
            return events;
        }

        internal static async Task AsyncGetScanEventsJson()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            var json = await client.GetStringAsync(ConstantHelper.DOMAIN + ConstantHelper.URL_GET_SCANEVENTS);
            var res = JsonSerializer.Deserialize<ScanEventResponse>(json);
            Console.Write(json);
        }
    }
}
