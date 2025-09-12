using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Domain.Entities
{
    public class ReportTankExport
    {
        public int ID { get; set; }
        public int CommandID { get; set; }
        public Nullable<System.DateTime> TimeStart { get; set; }
        public Nullable<System.DateTime> TimeStop { get; set; }
        public Nullable<decimal> WorkOrder { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Vehicle { get; set; }
        public Nullable<byte> ArmNo { get; set; }
        public Nullable<byte> WareHouseCode { get; set; }
        public string WareHouseName { get; set; }
        public Nullable<float> V_Preset { get; set; }
        public Nullable<float> V_Actual { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public Nullable<byte> CompartmentOrder { get; set; }
    }
}
