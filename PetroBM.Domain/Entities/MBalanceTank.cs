using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Domain.Entities
{
   public class MBalanceTank
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
