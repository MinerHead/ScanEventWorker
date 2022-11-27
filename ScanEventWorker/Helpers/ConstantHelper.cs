using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    internal static class ConstantHelper
    {
        internal static int LIMIT { get; set; } // page size for API calls
        internal static bool ALLOW_NEW_TYPE { get; set; }  //toggle whether new Event Type can be added 
        internal static int INIT_EVENT_ID { get; set; } //initial event id to fetch from
        internal static int INFO_LOG_TYPE_ID { get; set; } // type id for INFO log type
        internal static int WARNING_LOG_TYPE_ID { get; set; } // type id for WARNING log type
        internal static int ERROR_LOG_TYPE_ID { get; set; } // type id for ERROR log type
        internal static int OTHER_LOG_TYPE_ID { get; set; } // type id for OTHER log type
        internal static int API_TIME { get; set; } //time interval for API calls in idle mode

        internal const string DOMAIN = "https://localhost";
        internal const string URL_GET_SCANEVENTS = "/v1/scans/scanevents";
        
    }
}
