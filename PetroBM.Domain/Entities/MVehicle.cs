namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MVehicle")]
    public partial class MVehicle:BaseEntity
    {
        public int ID { get; set; }

        [Display(Name = "Số phương tiện")]
        [Required(ErrorMessage = "Số phương tiện")]
        [StringLength(30, ErrorMessage = "Nhập số phương tiện từ {2} đến {1} ký tự.", MinimumLength = 4)]
        public string VehicleNumber { get; set; }

       
        [Display(Name = "Số đăng ký")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Số đăng ký")]
        public string RegisterNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        [Display(Name = "Ngày hết hạn lưu hành")]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "Giấy phép vận chuyển chất, hàng cháy nổ")]
        [StringLength(20, ErrorMessage = "Giấy phép vận chuyển chất, hàng cháy nổ {2} đến {1} ký tự.", MinimumLength = 4)]
        public string FirePreventLicense { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        [Display(Name = "Thời hạn giấy phép vận chuyển chất, hàng cháy nổ")]
        public DateTime? FirePreventExpire { get; set; }

        [Display(Name = "Số giấy kiểm định")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        public string AccreditationNumber { get; set; }

        [Display(Name = "Ngày kiểm định hết hạn")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        public DateTime? AccreditationExpire { get; set; }

        [Display(Name = "Lái xe mặc định")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage ="Lái xe mặc định bắt buộc nhập")]
        public string Driverdefault { get; set; }

        public int? CardID { get; set; }

        public int? Volume1 { get; set; }

        public int? Volume2 { get; set; }

        public int? Volume3 { get; set; }

        public int? Volume4 { get; set; }

        public int? Volume5 { get; set; }

        public int? Volume6 { get; set; }

        public int? Volume7 { get; set; }

        public int? Volume8 { get; set; }

        public int? Volume9 { get; set; }

        public byte CommandType { get; set; }
    }
}
