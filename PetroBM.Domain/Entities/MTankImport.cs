namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankImport")]
    public partial class MTankImport
    {

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ImportInfoId { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte WareHouseCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TankId { get; set; }
        [Display(Name = "Ngày chốt đầu")]
        public DateTime? StartDate { get; set; }

        [Range(0, 100, ErrorMessage = "Nhiệt độ nằm trong khoảng 0-100")]
        [Display(Name = "Nhiệt độ")]
        public float? StartTemperature { get; set; }
        [Display(Name = "Chiều cao hàng")]
        public float? StartProductLevel { get; set; }
        [Display(Name = "Thể tích hàng (Vtt)")]
        public double? StartProductVolume { get; set; }
        [Display(Name = "Tỉ trọng")]
        [Range(0.6, 1, ErrorMessage = "Tỉ trọng nằm trong khoảng 0.6-1")]
        public float? StartDensity { get; set; }

        public float? StartVCF { get; set; }
        [Display(Name = "Thể tích hàng (V15)")]
        public double? StartProductVolume15 { get; set; }
        [Display(Name = "Ngày chốt cuối")]
        public DateTime? EndDate { get; set; }
        [Range(0, 100, ErrorMessage = "Nhiệt độ nằm trong khoảng 0-100")]
        [Display(Name = "Nhiệt độ")]
        public float? EndTemperature { get; set; }
        [Display(Name = "Chiều cao hàng")]
        public float? EndProductLevel { get; set; }
        [Display(Name = "Thể tích hàng (Vtt)")]
        public double? EndProductVolume { get; set; }
        [Display(Name = "Tỉ trọng")]
        [Range(0.6, 1, ErrorMessage = "Tỉ trọng nằm trong khoảng 0.6-1")]
        public float? EndDensity { get; set; }
        public float? EndVCF { get; set; }
        [Display(Name = "Thể tích hàng (V15)")]
        public double? EndProductVolume15 { get; set; }

        public double? ExportVolume { get; set; }

        public double? ExportVolume15 { get; set; }

        public bool? ExportFlg { get; set; }

        //public virtual MImportInfo MImportInfo { get; set; }

        public virtual MTank MTank { get; set; }

    }
}
