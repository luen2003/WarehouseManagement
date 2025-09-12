namespace PetroBM.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MCustomer")]
    public partial class MCustomer:BaseEntity
    {
        public int ID { get; set; }

       
        [Display(Name = "Mã khách hàng")]
        [Required(ErrorMessage ="(*) mã khách hàng bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "(*) mã khách từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string CustomerCode { get; set; }

       
        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage ="Tên khách hàng bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "(*) Tên khách hàng từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string CustomerName { get; set; }

        [StringLength(250)]
        [Display(Name = "Ảnh đại diện")]
        public string PhotoName { get; set; }

        [Display(Name = "Mã số thuế")]
        [StringLength(50, ErrorMessage = "Mã số thuế  từ {2} đến {1} ký tự.")]
        public string TaxCode { get; set; }

       
        [Display(Name = "Số tài khoản")]
        [StringLength(50, ErrorMessage = "Số tài khoản từ {2} đến {1} ký tự.")]
        public string AccountNo { get; set; }

        [Display(Name = "Người đại diện")]
        [StringLength(50, ErrorMessage = "Người đại diện từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string Deputy { get; set; }


        [Display(Name = "Chức vụ")]
        [StringLength(50, ErrorMessage = "Chức vụ từ {2} đến {1} ký tự.")]
        public string Position { get; set; }


        [Display(Name = "Địa chỉ khách hàng")]
        [StringLength(250, ErrorMessage = "Địa chỉ khách hàng {2} đến {1} ký tự.")]
        public string CustomerAddress { get; set; }

        [Display(Name = "Số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại {2} đến {1} ký tự.")]
        //[RegularExpression("^[0-9]{8,15}$", ErrorMessage = "Số điện thoại nhập không đúng")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Tên đơn vị")]
        [StringLength(250, ErrorMessage = "Tên đơn vị {2} đến {1} ký tự.", MinimumLength = 4)]
        public string UnitName { get; set; }


        [Display(Name = "Vị trí")]
        [StringLength(250, ErrorMessage = "Vị trí {2} đến {1} ký tự.", MinimumLength = 4)]
        public string Postition { get; set; }

        [Display(Name = "Hình thức thanh toán")]
        [StringLength(50, ErrorMessage = "Hình thức thanh toán {2} đến {1} ký tự.", MinimumLength = 2)]
        public string Payments { get; set; }

        [Display(Name = "Giá vùng")]
        [StringLength(50, ErrorMessage = "Giá vùng {2} đến {1} ký tự.", MinimumLength = 2)]
        public string Price { get; set; }

        [Display(Name = "Đơn vị tính")]
        [StringLength(50, ErrorMessage = "Đơn vị tính tối đa {1} ký tự.")]
        public string Unit { get; set; }

        public int? CustomerGroup { get; set; }
    }
}
