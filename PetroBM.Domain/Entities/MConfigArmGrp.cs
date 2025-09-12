namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MConfigArmGrp")]
    public partial class MConfigArmGrp:BaseEntity
    {
        [Key]
        [Display(Name = "Mã nhóm")]
        [Required(ErrorMessage ="Mã nhóm bắt buộc nhập.")]
        public int ConfigArmGrpId { get; set; }

        [StringLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        [Display(Name = "Tên nhóm")]
        [Required(ErrorMessage ="Tên nhóm bắt buộc nhập.")]
        public string ConfigArmName { get; set; }

        [Display(Name = "Kho")]
        [Required(ErrorMessage = "Kho bắt buộc nhập.")]
        public byte WareHouseCode { get; set; }

    }
}
