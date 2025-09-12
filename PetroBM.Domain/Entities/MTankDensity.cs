namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankDensity")]
    public partial class MTankDensity
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


        [Display(Name = "Tỉ trọng")]
        [Required(ErrorMessage ="*")]
        [Range(0.6,1,ErrorMessage = "Tỉ trọng nằm trong khoảng 0.6-1")]
        public float? Density { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(20)]
        [Display(Name = "Người nhập")]
        public string InsertUser { get; set; }

        public virtual MTank MTank { get; set; }
    }
}
