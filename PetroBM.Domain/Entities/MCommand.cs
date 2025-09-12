namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MCommand")]
    public partial class MCommand : BaseEntity
    {
        [Key]
        public int CommandID { get; set; }

        [Column(TypeName = "numeric")]

        [Display(Name = "Mã lệnh")]
        public decimal? WorkOrder { get; set; }

        [Display(Name = "Mã kho")]
        public byte? WareHouseCode { get; set; }

        //[StringLength(50)]
        //public string CommandCode { get; set; }

        [StringLength(20)]
        [Display(Name = "Mã khách hàng")]
        public string CustomerCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Card data")]
        public string CardData { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Card serial")]
        public long? CardSerial { get; set; }

        [Display(Name = "Thời gian đăng ký lệnh")]
        public DateTime TimeOrder { get; set; }

        [StringLength(50)]
        [Display(Name = "Số phương tiện")]
        public string VehicleNumber { get; set; }

        [StringLength(200)]
        [Display(Name = "Tên lái xe")]
        public string DriverName { get; set; }

        [Display(Name = "Nhiệt độ")]
        public double Temperature { get; set; }

        [Display(Name = "Trạng thái")]
        public decimal? Status { get; set; }

        [Display(Name = "Thời gian chứng từ")]
        public DateTime? CertificateTime { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Số chứng từ")]
        public int? CertificateNumber { get; set; }

        [Display(Name = "Chiết khấu")]
        public int? Discount { get; set; }

        [Display(Name = "Khách hàng không trả thuế môi trường")]
        public bool EnvironmentTax { get; set; }

        [Display(Name = "Lý do xuất")]
        public string ExportReason { get; set; }
        

        [Display(Name = "Ghi chú")] 
        public string Description { get; set; }

        [Display(Name = "GGT")]
        public string GGT { get; set; }

        [Display(Name = "IsApprove")]
        public int? IsApprove { get; set; }

        [Display(Name = "Image1")]
        public string Image1 { get; set; }

        [Display(Name = "Image2")]
        public string Image2 { get; set; }

        [Display(Name = "Image3")]
        public string Image3 { get; set; }

        [Display(Name = "Image4")]
        public string Image4 { get; set; }
    }
}
