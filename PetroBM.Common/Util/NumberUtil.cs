using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Common.Util
{
    public class NumberUtil
    {
        public static string FormatNumberUn(double? number, int decimalPlace)
        {
            if (number == null)
            {
                return String.Empty;
            }

            string format = "#,##0";
            if (decimalPlace > 0)
            {
                format += ".";

                for (int i = 0; i < decimalPlace; i++)
                {
                    format += "#";
                }
            }

            return ((double)number).ToString(format);
        }

        public static string FormatNumber(double? number, int decimalPlace)
        {
            if (number == null)
            {
                return String.Empty;
            }

            string format = "#,##0";
            if (decimalPlace > 0)
            {
                format += ".";

                for (int i = 0; i < decimalPlace; i++)
                {
                    format += "#";
                }
            }
            else
            {
                return ((double)number).ToString(format).Replace(",",".");
            }    

            return ((double)number).ToString(format);
        }
    }
}
