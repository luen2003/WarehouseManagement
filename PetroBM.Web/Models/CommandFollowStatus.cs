using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class CommandFollowStatus
    {
        public string No { get; set; }
        public string WorkOrder { get; set; }
        public int CommandID { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
        public string CardSerial { get; set; }
        public string ProductName { get; set; }
        public string V_Preset { get; set; }
        public string V_Actual { get; set; }
        public string Volume1 { get; set; }
        public string Volume2 { get; set; }
        public string Volume3 { get; set; }
        public string Volume4 { get; set; }
        public string Volume5 { get; set; }
        public string Volume6 { get; set; }
        public string Volume7 { get; set; }
        public string Volume8 { get; set; }
        public string Volume9 { get; set; }
        
    }
}