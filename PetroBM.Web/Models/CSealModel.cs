using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class CSealModel
    {
        public decimal? WorkOrder { get; set; }
        public int CommandID { get; set; }
        public string V_Actual { get; set; }
        public byte? CompartmentOrder { get; set; }
        public double? Vtt { get; set; }
        public string ProductName { get; set; }
        public string Seal1 { get; set; }

        public string Seal2 { get; set; }
        public string Ratio { get; set; }

        public string CardData { get; set; }

        public long? CardSerial { get; set; }
        public DateTime? TimeOrder { get; set; }
        public byte? Flag { get; set; }

    }
}