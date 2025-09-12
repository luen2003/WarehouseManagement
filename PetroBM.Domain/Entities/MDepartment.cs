namespace PetroBM.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MDepartment")]
    public partial class MDepartment : BaseEntity
    {
        public int ID { get; set; } 

        [Display(Name = "Mã phòng ban")]
        [Required(ErrorMessage ="(*) mã phòng ban bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "(*) mã khách từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string Code { get; set; }

       
        [Display(Name = "Tên phòng ban")]
        [Required(ErrorMessage ="Tên phòng ban bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "(*) Tên khách hàng từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string Name { get; set; }

        [Display(Name = "Tên ngắn")]  
        public string ShortName { get; set; }

        [Display(Name = "Trực thuộc")]  
        public string Parent { get; set; }
    }
}
