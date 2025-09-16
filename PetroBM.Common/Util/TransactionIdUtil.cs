using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Common.Util
{
    public class TransactionIdUtil
    {
        public static string Generate()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(100000, 999999);
            return $"{timestamp}_{random}";
        }
    }
}
