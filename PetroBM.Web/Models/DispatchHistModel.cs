using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using PetroBM.Common.Util;

namespace PetroBM.Web.Models
{
    public class DispatchHistModel
    {
        public DispatchHistModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
                .ToString(Constants.DATE_FORMAT);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
                .ToString(Constants.DATE_FORMAT);

            DispatchHist = new MDispatchHist();
            ListProduct = new List<MProduct>();
            ListDepartment = new List<MDepartment>();
            ListProductTemp = new List<ProductTemp>();
            ListSelectedField = new List<string>();
            ListTemProduct = new List<DataValue>();
            ListDispatch = new List<MDispatch>();
        }

        // --- Bộ lọc / hiển thị ---
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        // --- Dòng dữ liệu lịch sử (phản ánh đúng các cột trong dbo.MDispatch_Hist) ---
        public int DispatchID { get; set; }
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

        public byte? Status { get; set; }

        public DateTime InsertDate { get; set; }
        public string InsertUser { get; set; }

        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        public int VersionNo { get; set; }
        public bool DeleteFlg { get; set; }

        public string SysU { get; set; }
        public DateTime? SysD { get; set; }

        public int ProcessStatus { get; set; }

        // --- Liên quan UI / dữ liệu phụ trợ ---
        public MDispatchHist DispatchHist { get; set; }
        public List<MProduct> ListProduct { get; set; }
        public List<MDepartment> ListDepartment { get; set; }
        public List<ProductTemp> ListProductTemp { get; set; }
        public List<string> ListSelectedField { get; set; }
        public List<DataValue> ListTemProduct { get; set; }
        public List<MDispatch> ListDispatch { get; set; }
    }
}
