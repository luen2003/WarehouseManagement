using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class CProductModel
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public string Volume { get; set; }
        public int Discount { get; set; }

    }
}