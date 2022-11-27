using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanEventWorker
{
    internal class LogTypeStore
    {
        internal static void InitializeLogTypeId()
        {
            ConstantHelper.INFO_LOG_TYPE_ID = GetLogTypeIdByLogType("INFO");
            ConstantHelper.WARNING_LOG_TYPE_ID = GetLogTypeIdByLogType("WARNING");
            ConstantHelper.ERROR_LOG_TYPE_ID = GetLogTypeIdByLogType("ERROR");
            ConstantHelper.OTHER_LOG_TYPE_ID = GetLogTypeIdByLogType("OTHER");
        }

        internal static int GetLogTypeIdByLogType(string type)
        {
            var upperCase = type.ToUpper();
            string sql = @"SELECT LogTypeId FROM LogType WHERE LogType = @0";
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
