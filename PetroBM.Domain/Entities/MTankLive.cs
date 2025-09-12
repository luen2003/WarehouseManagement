namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankLive")]
    public partial class MTankLive
    {
        [Key]
        [Column(Order = 0)]
        public byte WareHouseCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TankId { get; set; }

        [Key]
        [Column(Order = 2)]
        [Display(Name = "Ngày")]
        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING, ApplyFormatInEditMode = true)]
        public DateTime InsertDate { get; set; }

        [Display(Name = "Chiều cao nước")]
        public float? WaterLevel { get; set; }

        [Display(Name = "Chiều cao hàng")]
        public float? ProductLevel { get; set; }

        [Display(Name = "Chiều cao chung")]
        public float? TotalLevel { get; set; }

        [Display(Name = "Thể tích nước")]
        public double? WaterVolume { get; set; }

        [Display(Name = "Thể tích hàng (Vtt)")]
        public double? ProductVolume { get; set; }

        [Display(Name = "Thể tích chung")]
        public double? TotalVolume { get; set; }

        [Display(Name = "Thể tích trống")]
        public double? TankEmpty { get; set; }

        [Display(Name = "Nhiệt độ trung bình")]
        public float? AvgTemperature { get; set; }

        [Display(Name = "Nhiệt độ điểm 1")]
        public float? Temperature1 { get; set; }

        [Display(Name = "Nhiệt độ điểm 2")]
        public float? Temperature2 { get; set; }

        [Display(Name = "Nhiệt độ điểm 3")]
        public float? Temperature3 { get; set; }

        [Display(Name = "Nhiệt độ điểm 4")]
        public float? Temperature4 { get; set; }

        [Display(Name = "Nhiệt độ điểm 5")]
        public float? Temperature5 { get; set; }

        [Display(Name = "Nhiệt độ điểm 6")]
        public float? Temperature6 { get; set; }
        
        [Display(Name = "Nhiệt độ điểm 7")]
        public float? Temperature7 { get; set; }

        [Display(Name = "Nhiệt độ điểm 8")]
        public float? Temperature8 { get; set; }

        [Display(Name = "Nhiệt độ điểm 9")]
        public float? Temperature9 { get; set; }

        [Display(Name = "Nhiệt độ điểm 10")]
        public float? Temperature10 { get; set; }

        [Display(Name = "Level Rate")]
        public double? LevelRate { get; set; }

        [Display(Name = "Flow Rate")]
        public double? FlowRate { get; set; }

        [Display(Name = "Thể tích xuất được")]
        public double? AvailableVolume { get; set; }

        [Display(Name = "Khối lượng")]
        public double? Mass { get; set; }

        [Display(Name = "Mass Rate")]
        public double? MassRate { get; set; }

        [Display(Name = "Thể tích hàng (V15)")]
        public double? ProductVolume15 { get; set; }

        [Display(Name = "VCF")]
        public float? VCF { get; set; }

        [Display(Name = "WCF")]
        public float? WCF { get; set; }
        
       // public virtual MTank MTank { get; set; }
    }
}
