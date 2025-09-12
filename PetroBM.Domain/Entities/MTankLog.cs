namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankLog")]
    public partial class MTankLog
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

        public int? ProductId { get; set; }

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

        public float? Density { get; set; }

        public virtual MTank MTank { get; set; }

        //[Key]
        //[Column(Order = 0)]
        //public byte WareHouseCode { get; set; }

        //[Key]
        //[Column(Order = 1)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //public int TankId { get; set; }

        //[Key]
        //[Column(Order = 2)]
        //public DateTime InsertDate { get; set; }

        //public int ProductId { get; set; }

        //public float? WaterLevel { get; set; }

        //public float? ProductLevel { get; set; }

        //public float? TotalLevel { get; set; }

        //public double? WaterVolume { get; set; }

        //public double? ProductVolume { get; set; }

        //public double? TotalVolume { get; set; }

        //public double? TankEmpty { get; set; }

        //public float? AvgTemperature { get; set; }

        //public double? LevelRate { get; set; }

        //public double? FlowRate { get; set; }

        //public double? AvailableVolume { get; set; }

        //public double? Mass { get; set; }

        //public double? MassRate { get; set; }

        //public double? ProductVolume15 { get; set; }

        //public float? VCF { get; set; }

        //public float? WCF { get; set; }

        //public float? Density { get; set; }

        //public virtual MTank MTank { get; set; }
    }
}
