namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MInvoiceDetail")]
    public partial class MInvoiceDetail:BaseEntity
    {
        public int ID { get; set; }

        public int InvoiceID { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã hóa đơn")]
        public string InvoiceCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Danh sách ngăn")]
        public string ListVolume { get; set; }

        [StringLength(6)]
        [Display(Name = "Mã hàng hóa")]
        public string ProductCode { get; set; }

        [StringLength(200)]
        [Display(Name = "Hàng hóa")]
        public string ProductName { get; set; }

        [Display(Name = "Số lượng")]
        public double? Quantity { get; set; }
        [Display(Name = "Đơn vị")]
        public double? Unit { get; set; }

        [Display(Name = "Thành tiền")]
        public double? Amount { get; set; }

        [StringLength(50)]
        [Display(Name = "Card data")]
        public string CardData { get; set; }

        [Display(Name = "Card serial")]
        public long? CardSerial { get; set; }
        [Display(Name = "Số lần in")]
        public int? CountPrint { get; set; }

        [Display(Name = "Thời điểm xuất")]
        public DateTime? OutTime { get; set; }

        [Display(Name = "Chiết khấu")]
        public int? Discount { get; set; }

        [Display(Name = "Thuế môi trường")]
        public int? EnvironmentTax { get; set; }

        public float? AvgVcf { get; set; }

        public float? AvgDensity { get; set; }

        public float? AvgTemperature { get; set; }

        //ExportNo
        [Display(Name = "Số phiếu xuất kho")]
        public int? ExportNo { get; set; }

        public string Note { get; set; }
    }
}
