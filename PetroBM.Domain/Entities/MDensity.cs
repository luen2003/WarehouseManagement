namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MDensity")]
    public partial class MDensity:BaseEntity
    {
        public int ID { get; set; }

        [Display(Name = "Mã kho")]
        public byte WareHouseCode { get; set; }

        [Display(Name = "Mã họng")]
        public byte ArmNo { get; set; }

        [Required]
        [StringLength(6)]
        [Display(Name = "Mã hàng hóa")]
        public string ProductCode { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Tỷ lệ trộn bị sai. Giá trị phải nằm trong khoảng 0->6%")]
        [Display(Name = "Tỷ lệ trộn")]
        public float MixingRatio { get; set; }

        [Required]
        [Range(0.653, 1.075, ErrorMessage = "Tỷ trọng xăng gốc bị sai.Giá trị phải nằm trong khoảng 0.653->1.075")]
        [Display(Name = "Tỷ trọng xăng gốc D15")]
        public float GasDensity { get; set; }

        [Required]
        [Range(0.653, 1.075, ErrorMessage = "Tỷ trọng Ethanol bị sai. Giá trị phải nằm trong khoảng 0.653->1.075")]
        [Display(Name = "Tỷ trọng Ethanol D15")]
        public float AlcoholicDensity { get; set; }

        public byte RecipeProduct { get; set; }

      

    }
}
