using System;
using PetroBM.Common.Util;
using PetroBM.Domain.Entities;

namespace PetroBM.Web.Models
{
    public class DispatchWaterHistModel
    {
        public DispatchWaterHistModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
                .ToString(Constants.DATE_FORMAT);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
                .ToString(Constants.DATE_FORMAT);
        }

        // Bộ lọc (nếu không cần thì xoá)
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        // Dữ liệu lịch sử (khớp bảng dbo.MDispatchWater_Hist)
        public int DispatchID { get; set; }
        public int VersionNo { get; set; }
        public int? CertificateNumber { get; set; }
        public DateTime? TimeOrder { get; set; }
        public DateTime? CertificateTime { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeStop { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName1 { get; set; }
        public string DriverName2 { get; set; }
        public string ProductCode { get; set; }
        public string Department { get; set; }
        public string DstPickup1 { get; set; }
        public string DstPickup2 { get; set; }
        public string DstReceive { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }   // NVARCHAR(10)
        public DateTime InsertDate { get; set; }
        public string InsertUser { get; set; }   // NVARCHAR(50)
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }   // NVARCHAR(50)
        public bool DeleteFlg { get; set; }
        public string From { get; set; }            // map cột [From]
        public string To { get; set; }
        public string Paragraph1 { get; set; }
        public string Paragraph2 { get; set; }
        public string Paragraph3 { get; set; }
        public string Paragraph4 { get; set; }
        public int ProcessStatus { get; set; }
        public string SysU { get; set; }
        public DateTime? SysD { get; set; }
        public string TransactionId { get; set; }

        // Tuỳ chọn: giữ entity nếu cần dùng trực tiếp
        public MDispatchWaterHist Hist { get; set; }
    }
}
