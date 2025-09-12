namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MPrice")]
    public partial class MPrice:BaseEntity
    {
        [Key]
        public int ProduceId { get; set; }

        [Required(ErrorMessage ="Tên hàng hóa bắt buộc nhập.")]
        [StringLength(50,ErrorMessage ="Tối đa {1} kí tự")]
        [Display(Name = "Tên hàng hóa")]
        public string ProductName { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Display(Name = "Tên viết tắt")]
        public string Abbreviations { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa {0} kí tự")]
        [Display(Name = "Mã hàng hóa")]
        public string ProductCode { get; set; }

        [StringLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; }

        [Display(Name = "Giá vùng 1")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Giá 1 bắt buộc nhập.")]
        [Range(0,1000000, ErrorMessage = "Giá vùng 1 nhập sai.")]
        public int? AreaPrice1 { get; set; }

        [Display(Name = "Giá vùng 2")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Giá vùng 2 bắt buộc nhập.")]
        [Range(0, 10000000, ErrorMessage = "Giá vùng 2 nhập sai.")]
        public int? AreaPrice2 { get; set; }

        [Display(Name = "Thuế môi trường")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Thuế môi trường bắt buộc nhập.")]
        [Range(0, 1000000, ErrorMessage = "Thuế môi trường nhập sai.")]
        public int? EnvironmentTax { get; set; }


    }
}
