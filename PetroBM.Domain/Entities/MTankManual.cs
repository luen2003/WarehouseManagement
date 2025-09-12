namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankManual")]
    public partial class MTankManual : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public byte WareHouseCode { get; set; }

        public int TankId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Chiều cao nước")]
        public float? WaterLevel { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Chiều cao chung")]
        public float? TotalLevel { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(0, 100, ErrorMessage = "Nhiệt độ nằm trong khoảng 0-100")]
        [Display(Name = "Nhiệt độ trung bình")]
        public float? AvgTemperature { get; set; }

        [Display(Name = "Chiều cao nước đo máy")]
        public float? LogWaterLevel { get; set; }

        [Display(Name = "Chiều cao chung đo máy")]
        public float? LogTotalLevel { get; set; }

        [Display(Name = "Nhiệt độ đo máy")]
        public float? LogAvgTemperature { get; set; }

        [Display(Name = "Ngày đo máy")]
        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING)]
        public DateTime? LogInsertDate { get; set; }

        public virtual MTank MTank { get; set; }
    }
}
