using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class ConfigArmTempModel
    {
        public int ConfigArmId { get; set; }
        public string ArmName { get; set; }
        public byte ArmNo { get; set; }
        public byte WareHouseCode { get; set; }
    }
}