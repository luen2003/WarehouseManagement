namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MDispatchWater")]
    public partial class MDispatchWater : BaseEntity
    {
        [Key]
        public int DispatchID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Số chứng từ")]
        public int? CertificateNumber { get; set; }

        [Display(Name = "Thời gian đăng ký lệnh")]
        public DateTime TimeOrder { get; set; }

        [Display(Name = "Thời gian chứng từ")]
        public DateTime? CertificateTime { get; set; }

        [Display(Name = "Thời gian thực hiện")]
        public DateTime? TimeStart { get; set; }

        [Display(Name = "Thời gian hoàn thành")]
        public DateTime? TimeStop { get; set; }

        [StringLength(50)]
        [Display(Name = "Số phương tiện")]
        public string VehicleNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Lái xe 1")]
        public string DriverName1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Lái xe 2")]
        public string DriverName2 { get; set; }

        [StringLength(50)]
        [Display(Name = "Hàng hoá")]
        public string ProductCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Phân xưởng")]
        public string Department { get; set; }

        [StringLength(100)]
        [Display(Name = "Điểm bắt đầu")]
        public string DstPickup1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Điểm làm việc")]
        public string DstPickup2 { get; set; }

        [StringLength(100)]
        [Display(Name = "Điểm kết thúc")]
        public string DstReceive { get; set; }

        [StringLength(500)]
        [Display(Name = "Nguyên tắc giao hàng")]
        public string Note1 { get; set; }

        [StringLength(500)]
        [Display(Name = "Note 2")]
        public string Note2 { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Remark { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [StringLength(500)]
        [Display(Name = "Từ")]
        public string From { get; set; }
        [StringLength(500)]
        [Display(Name = "Đến")]
        public string To { get; set; }
        [StringLength(500)]
        [Display(Name = "Đoạn 1")]
        public string Paragraph1 { get; set; }
        [StringLength(500)]
        [Display(Name = "Đoạn 2")]
        public string Paragraph2 { get; set; }
        [StringLength(500)]
        [Display(Name = "Đoạn 3")]
        public string Paragraph3 { get; set; }
        [StringLength(500)]
        [Display(Name = "Đoạn 4")]
        public string Paragraph4 { get; set; }

        public int ProcessStatus { get; set; }
    }
}
