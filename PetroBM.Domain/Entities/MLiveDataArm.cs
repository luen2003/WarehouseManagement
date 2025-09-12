namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MLiveDataArm")]
    public partial class MLiveDataArm
    {

        [Key]
        [Column(Order = 0)]
        [Display(Name = "Thời gian")]
        public DateTime TimeLog { get; set; }

        [Key]
        [Column(Order = 1)]
        [Display(Name = "Mã kho")]
        public byte WareHouseCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [Display(Name = "Họng số")]
        public byte ArmNo { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name = "Mã lệnh")]
        public decimal? WorkOrder { get; set; }

        [Display(Name = "Ngăn")]
        public byte? CompartmentOrder { get; set; }

        [StringLength(6)]
        [Display(Name = "Mã hàng hóa")]
        public string ProductCode { get; set; }

        [StringLength(200)]
        [Display(Name = "Tên hàng hóa")]
        public string ProductName { get; set; }

        [StringLength(50)]
        [Display(Name = "Số phương tiện")]
        public string VehicleNumber { get; set; } 

        [StringLength(50)]
        [Display(Name = "Card data")]
        public string CardData { get; set; }

        [Display(Name = "Card serial")]
        public long? CardSerial { get; set; }

        [Display(Name = "V đặt")]
        public float? V_Preset { get; set; }

        [Display(Name = "Vtt")]
        public float? V_Actual { get; set; }

        [Display(Name = "Vbase")]
        public float? V_Actual_Base { get; set; }

        [Display(Name = "Ve")]
        public float? V_Actual_E { get; set; }
        [Display(Name = "Lưu lượng")]
        public float? Flowrate { get; set; }

        [Display(Name = "Lưu lượng xăng gốc")]
        public float? Flowrate_Base { get; set; }

        [Display(Name = "Lưu lượng Ethanol")]
        public float? Flowrate_E { get; set; }
        [Display(Name = "Nhiệt độ trung bình")]
        public float? AvgTemperature { get; set; }
        [Display(Name = "Nhiệt tức thời")]
        public float? CurrentTemperature { get; set; }
        [Display(Name = "Tỷ lệ trộn")]
        public float? MixingRatio { get; set; }
        [Display(Name = "Tỉ lệ trộn thực tế")]
        public float? ActualRatio { get; set; }
        [Display(Name = "Trạng thái")]
        public byte? Status { get; set; }
        [Display(Name = "Chế độ xuất")]
        public byte? ModeLog { get; set; }
        [Display(Name = "Trạng thái dừng sự cố")]
        public byte? ESD { get; set; }
        public byte? ValveStatus { get; set; }
        public byte? CommandType { get; set; }
        public string Abbreviations { get; set; }
    }
}
