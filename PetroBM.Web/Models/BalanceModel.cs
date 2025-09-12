using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class BalanceModel
    {
        public string Product { get; set; }
        public int Earlyperiod { get; set; }
        public int Endperiod { get; set; }
        public int Importeriod { get; set; }
        public int Exporteiod { get; set; }
        public int ExportTank { get; set; }
        public int ExportTypeExport { get; set; }
        public int EndStock { get; set; }
        public int EmptyStock { get; set; }
        public int Difference { get; set; }
     }
}