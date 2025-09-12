using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Common.Util
{
    public class DateTimeUtil
    {
        public static double GetJSTime(DateTime date)
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            var uDate = localZone.ToUniversalTime(date);
            return uDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
