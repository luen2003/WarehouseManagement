using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class TankTempModel
    {
        public int TankId { get; set; }
        public string TankName { get; set; }
        public int ProductId { get; set; }
        public byte WareHouseCode { get; set; }

    }
}