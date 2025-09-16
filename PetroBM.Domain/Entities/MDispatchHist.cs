namespace PetroBM.Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MDispatch_Hist")]
    public partial class MDispatchHist
    {
        [Key]
        [Column(Order = 0)] // vì bảng không có PK, phải chỉ định nếu cần composite key
        public int DispatchID { get; set; }

        [Display(Name = "Số chứng từ")]
        public int? CertificateNumber { get; set; }

        [Display(Name = "Thời gian đăng ký lệnh")]
        public DateTime? TimeOrder { get; set; }

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
        [Display(Name = "Phòng ban")]
        public string Department { get; set; }

        [StringLength(100)]
        [Display(Name = "Điểm lấy hàng 1")]
        public string DstPickup1 { get; set; }

        [StringLength(100)]
        [Display(Name = "Điểm lấy hàng 2")]
        public string DstPickup2 { get; set; }

        [StringLength(100)]
        [Display(Name = "Điểm trả hàng")]
        public string DstReceive { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Note1 { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú 2")]
        public string Note2 { get; set; }

        [StringLength(500)]
        [Display(Name = "Nguyên tắc")]
        public string Remark { get; set; }

        [Display(Name = "Trạng thái")]
        public byte? Status { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime InsertDate { get; set; }

        [StringLength(20)]
        [Display(Name = "Người tạo")]
        public string InsertUser { get; set; }

        [Display(Name = "Ngày sửa đổi")]
        public DateTime UpdateDate { get; set; }

        [StringLength(20)]
        [Display(Name = "Người sửa đổi")]
        public string UpdateUser { get; set; }

        [Display(Name = "Phiên bản")]
        public int VersionNo { get; set; }

        [Display(Name = "Cờ xóa mềm")]
        public bool DeleteFlg { get; set; }

        [StringLength(100)]
        [Display(Name = "Người sửa đổi")]
        public string SysU { get; set; }

        [Display(Name = "Ngày sửa đổi")]
        public DateTime? SysD { get; set; }

        [Display(Name = "Trạng thái xử lý")]
        public int ProcessStatus { get; set; }
    }
}
