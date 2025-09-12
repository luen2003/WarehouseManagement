using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class ProductTemp
    {
        public string ProductName {get;set;}
        public string ProductCode { get; set; }
        public string Abbreviations { get; set; }
        public double? InputWastageRate { get; set; }
        public double? ExportWastageRate { get; set; }

    }
}