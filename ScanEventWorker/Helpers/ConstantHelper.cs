using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    internal static class ConstantHelper
    {
        internal static int LIMIT { get; set; }
        internal static bool ALLOW_NEW_TYPE { get; set; }
        internal static int INIT_EVENT_ID { get; set; }
        internal static int INFO_LOG_TYPE_ID { get; set; }
        internal static int WARNING_LOG_TYPE_ID { get; set; }
        internal static int ERROR_LOG_TYPE_ID { get; set; }
        internal static int OTHER_LOG_TYPE_ID { get; set; }
        internal static int API_TIME { get; set; }

        internal const string DOMAIN = "https://localhost:7165";
        internal const string URL_GET_SCANEVENTS = "/v1/scans/scanevents";
        
    }
}
