namespace PetroBM.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MCustomerGroup")]
    public partial class MCustomerGroup:BaseEntity
    {
        public int ID { get; set; }

       
        [Display(Name = "Mã nhóm khách hàng")]
        [Required(ErrorMessage ="(*) mã nhóm khách hàng bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "(*) mã nhóm khách từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string CustomerGroupCode { get; set; }

       
        [Display(Name = "Tên nhóm khách hàng")]
        [Required(ErrorMessage ="Tên nhóm khách hàng bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "(*) Tên nhóm khách hàng từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string CustomerGroupName { get; set; }

        [StringLength(250)]
        [Display(Name = "Tên ngắn")]
        public string ShortName { get; set; }

        [Display(Name = "Loại tính chất")]
        [StringLength(50, ErrorMessage = "Loại tính chất từ {2} đến {1} ký tự.")]
        public string Type { get; set; } 
    }
}
