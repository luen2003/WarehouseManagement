namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MInvoice")]
    public partial class MInvoice:BaseEntity
    {
        [Key]
        public int InvoiceID { get; set; }

        [Display(Name = "Kho")]
        public byte? WareHousecode { get; set; }

        [Display(Name = "Mã lệnh")]
        public decimal? WorkOrder { get; set; }

        [Display(Name = "Lệnh")]
        public int? CommandID { get; set; }

        [StringLength(50)]
        [Display(Name = "Số chứng từ")]
        public string InvoiceNo { get; set; }

        [StringLength(50)]
        [Display(Name = "Card data")]
        public string CardData { get; set; }

        [Display(Name = "Card serial")]
        public long? CardSerial { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã Khách hàng")]
        public string CustomerCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Số phương tiện")]
        public string VehicleNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Lái xe")]
        public string DriverName { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã số thuế")]
        public string TaxCode { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [StringLength(50)]
        [Display(Name = " Tài khoản số")]
        public string AccountNo { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [StringLength(200)]
        [Display(Name = "Tên người mua")]
        public string BuyerName { get; set; }

        [StringLength(500)]
        [Display(Name = "Tên đơn vị")]
        public string CompanyName { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ người mua")]
        public string AddressBuyer { get; set; }

        [StringLength(20)]
        [Display(Name = "Số tài khoản KH")]
        public string AccountCustomNo { get; set; }

        [StringLength(200)]
        [Display(Name = "Thuế xuất")]
        public string TaxExport { get; set; }
        [Display(Name = "Thời gian nhập")]
        public DateTime? InTime { get; set; }
        [Display(Name = "Thời gian xuất")]
        public DateTime? OutTime { get; set; }

        [StringLength(50)]
        [Display(Name = "Niêm chì số")]
        public string SealNo { get; set; }

        [StringLength(200)]
        [Display(Name = "Kiểu thanh toán")]
        public string PaymentType { get; set; }

        [StringLength(50)]
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; }

        [StringLength(50)]
        [Display(Name = "Mức giá")]
        public string PriceLevel { get; set; }

        [Display(Name = "Thuế xuất")]
        public double? TaxRate { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Chiết khấu")]
        public int? Discount { get; set; }
   
        [Display(Name = "Thuế môi trường")]
        public bool? EnvironmentTax { get; set; }
    }
}
