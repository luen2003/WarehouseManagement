using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class VolumeDetailModel
    {
        public string Volume { get; set; } //Số thứ tự ngăn
        public int? Amount { get; set; } //
        public string ProductName { get; set; } //Mã hàng
        public int? Quality { get; set; }

    }
}