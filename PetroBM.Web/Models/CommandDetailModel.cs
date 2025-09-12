using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using PetroBM.Common.Util;
using System;
using System.Data;

namespace PetroBM.Web.Models
{
    public class CommandDetailModel
    {
        public CommandDetailModel()
        {
            //StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            //EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToString(Constants.DATE_FORMAT);//   DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToString(Constants.DATE_FORMAT);
            CommandDetail = new MCommandDetail();
            ListCommandDetails = new List<MCommandDetail>();
            ListSelectedField = new List<string>();
            ListCommand = new List<MCommand>();
        }

        public MCommandDetail CommandDetail { get; set; }
        public CommandFollowStatus commandFollowStatus { get; set; }
        public List<MCommandDetail> ListCommandDetails { get; set; }
        public List<MCommand> ListCommand { get; set; }
        public List<CommandFollowStatus> ListCommandDetail_Register { get; set; } // Đã đăng ký lấy hàng
		public List<CommandFollowStatus> ListCommandDetail_Approved { get; set; } // Đã phê duyệt
		public List<CommandFollowStatus> ListCommandDetail_Exporting { get; set; } // Đang xuất hàng
        public List<CommandFollowStatus> ListCommandDetail_WaitSeal { get; set; } //Đã xong tất cả chờ niêm chì
        public List<CommandFollowStatus> ListCommandDetail_Seal { get; set; } //Đã niêm chì chưa nhận hóa đơn
        public List<CommandFollowStatus> ListCommandDetail_Invoice { get; set; }// Đã nhận hóa đơn vả cả khỏi kho
        public List<CommandFollowStatus> ListCommandDetail_Exported { get; set; }// Đã xuất xong
        public List<byte> ListStatus { get; set; }
        public IPagedList<MCommandDetail> ListCommandDetail { get; set; }
        public List<MVehicle> ListVehicle { get; set; }
        public List<MConfigArm> ListConfigArm { get; set; }
        public List<MCustomer> ListCustomer { get; set; }

        public List<string> ListSelectedField { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string WorkOrder { get; set; } //Lệnh

        public string CardSerial { get; set; }

        public string CardData { get; set; }

        public string VerhicleNumber { get; set; }
        public byte? ArmNo { get; set; }
        public int? Flag { get; set; }
        public byte? Status { get; set; }

        public int? Page { get; set; }
        public string CertificateNumber { get; set; } //Mã chứng từ
    }
}