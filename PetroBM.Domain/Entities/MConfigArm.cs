namespace PetroBM.Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MConfigArm")]
    public partial class MConfigArm:BaseEntity
    {
        [Key]
        public int ConfigArmID { get; set; }

        [Display(Name = "Mã kho")]
        [Required(ErrorMessage ="(*) Mã kho bắt buộc nhập")]
        public byte WareHouseCode { get; set; }

       
        [Display(Name = "Tên họng")]
        [Required(ErrorMessage = "(*) Tên họng bắt buộc nhập")]
        [StringLength(50, ErrorMessage = "(*) Tên họng từ {2} đến {1}",MinimumLength =5)]
        public string ArmName { get; set; }

        [Display(Name = "Mã họng")]
        [Required(ErrorMessage = "(*) Mã họng bắt buộc nhập")]
        public byte ArmNo { get; set; }

       
        [Display(Name = "Mã hàng hóa 1")]
        [Required(ErrorMessage = "(*) Mã hàng hóa 1 bắt buộc nhập")]
        [StringLength(6, ErrorMessage = "Tối đa 6 kí tự")]
        public string ProductCode_1 { get; set; }

        [StringLength(6, ErrorMessage = "Tối đa 6 kí tự")]
        [Display(Name = "Mã hàng hóa 2")]
        public string ProductCode_2 { get; set; }

        [StringLength(6, ErrorMessage = "Tối đa 6 kí tự")]
        [Display(Name = "Mã hàng hóa 3")]
        public string ProductCode_3 { get; set; }

        public int? TankID { get; set; }

        [Display(Name = "Phân hệ")]
        public byte TypeExport { get; set; }
        [Display(Name = "Trạng thái kích hoạt")]
        public byte ActiveStatus { get; set; }

        
    }
}
