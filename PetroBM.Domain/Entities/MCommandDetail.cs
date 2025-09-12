namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MCommandDetail")]
    public partial class MCommandDetail:BaseEntity
    {
        public int ID { get; set; }

        public int CommandID { get; set; }

         [Display(Name = "Loại lệnh")]
        public byte? CommandType { get; set; }

		[Display(Name = "Lệnh cập nhật")]
		public byte? CommandFlag { get; set; }

		[Display(Name = "Thời gian đăng ký lệnh")]
        public DateTime TimeOrder { get; set; }

        public DateTime? TimeStart { get; set; }

        public DateTime? TimeStop { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name = "Mã lệnh")]
        public decimal? WorkOrder { get; set; }

        [Display(Name = "Mã ngăn")]
        public byte? CompartmentOrder { get; set; }

        [StringLength(200)]
        [Display(Name = "Hàng hóa")]
        public string ProductName { get; set; }

        [StringLength(6)]
        [Display(Name = "Mã HH")]
        public string ProductCode { get; set; }

        [Display(Name = "Mã họng")]
        public byte? ArmNo { get; set; }

        
        public byte? WareHouseCode { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Card data")]
        public string CardData { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Card serial")]
        public long? CardSerial { get; set; }

        [Display(Name = "Lượng đặt")]
        public float? V_Preset { get; set; }

        [Display(Name = "Lượng xuất")]
        public float? V_Actual { get; set; }

        public float? V_Deviation { get; set; }

        [Display(Name = "Nhiệt độ (C)")]
        public float? AvgTemperature { get; set; }

        [Display(Name = "Nhiệt độ TT")]
        public float? CurrentTemperature { get; set; }

        [Display(Name = "Trạng thái ngăn")]
        public byte? Flag { get; set; }

        public float? TotalStart { get; set; }

        public float? TotalEnd { get; set; }

        [StringLength(10)]
        [Display(Name = "Số phương tiện")]
        public string Vehicle { get; set; }

        [Display(Name = "Thời gian chứng từ")]
        public DateTime? CertificateTime { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Số chứng từ")]
        public int? CertificateNumber { get; set; }

        [Display(Name = "Chiết khấu")]
        public int? Discount { get; set; }

        [Display(Name = "Thuế môi trường")]
        public bool EnvironmentTax { get; set; }


        public float? MixingRatio { get; set; }

        public float? GasDensity { get; set; }

        public float? AlcoholicDensity { get; set; }

        public float? V_Actual_15 { get; set; }

        public float? TotalStart_15 { get; set; }

        public float? TotalEnd_15 { get; set; }

        public float? V_Actual_Base { get; set; }

        public float? V_Actual_E { get; set; }

        public float? V_Actual_Base_15 { get; set; }

        public float? V_Actual_E_15 { get; set; }

        public float? TotalStart_Base { get; set; }

        public float? TotalStart_E { get; set; }

        public float? TotalStart_Base_15 { get; set; }

        public float? TotalStart_E_15 { get; set; }

        public float? TotalEnd_Base { get; set; }

        public float? TotalEnd_E { get; set; }

        public float? TotalEnd_Base_15 { get; set; }

        public float? TotalEnd_E_15 { get; set; }

        public float? AvgDensity { get; set; }

        public float? CTL_Base { get; set; }

        public float? CTL_E { get; set; }

        public float? ActualRatio { get; set; }

        public float? V_Preset_15 { get; set; }

        public float? Vcf { get; set; }

        public float? Wcf { get; set; }

        public int? InvoiceDetailID { get; set; }

        [StringLength(500)]
        public string Flaglog { get; set; }

        [Display(Name = "Ghi chú")]
        public string Description { get; set; }

        [Display(Name = "GGT")]
        public string GGT { get; set; }
    }
}
