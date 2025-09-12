namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MAlarm")]
    public partial class MAlarm
    {

        [Key]
        public int AlarmId { get; set; }

        public int TypeId { get; set; }

        public byte? WareHouseCode { get; set; }

        public int? TankId { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(50)]
        public string Issue { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(20)]
        public string Value { get; set; }

        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING, ApplyFormatInEditMode = true)]
        public DateTime InsertDate { get; set; }

        public bool ConfirmFlag { get; set; }

        [StringLength(20)]
        public string ConfirmUser { get; set; }

        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING, ApplyFormatInEditMode = true)]
        public DateTime? ConfirmDate { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string ConfirmComment { get; set; }

        public bool SolveFlag { get; set; }

        [StringLength(20)]
        public string SolveUser { get; set; }

        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING, ApplyFormatInEditMode = true)]
        public DateTime? SolveDate { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        public string SolveComment { get; set; }

        public  MAlarmType MAlarmType { get; set; }

        public  MTank MTank { get; set; }
    }
}
