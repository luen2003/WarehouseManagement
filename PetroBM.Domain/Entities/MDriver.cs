namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MDriver")]
    public partial class MDriver:BaseEntity
    {
        public int ID { get; set; }
        [Display(Name = "Tên lái xe")]
        [Required(ErrorMessage = "(*) tên lái xe bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "Nhập tên lái xe từ {2} đến {1} ký tự.", MinimumLength = 2)]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        [Display(Name = "Ngày tháng năm sinh")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Số CMND/ Căn cước")]
        [Required(ErrorMessage = "(*) nhập số CMND/ Căn cước bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "Nhập số CMND/ Căn cước từ {2} đến {1} ký tự.", MinimumLength = 4)]
        public string IdentificationNumber { get; set; }

        [Display(Name = "Ngày cấp")]
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? LicenseDate { get; set; }

        [Display(Name = "Nơi cấp")]
        [StringLength(50, ErrorMessage = "Nhập nơi cấp từ {2} đến {1} ký tự.", MinimumLength = 4)]
        public string IssuedBy { get; set; }

        [Display(Name = "Số giấy phép lái xe")]
        [Required(ErrorMessage = "(*) Nhập số Số giấy phép lái xe.")]
        [StringLength(50, ErrorMessage = "Nhập Số giấy phép lái xe từ {2} đến {1} ký tự.", MinimumLength = 4)]
        public string DriversLicense { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        [Display(Name = "Thời hạn giấy phép lái xe")]
        public DateTime? DriversLicenseExpire { get; set; }

       
        [Display(Name = "Số chứng chỉ an toàn")]
        [StringLength(50, ErrorMessage = "Nhập Số chứng chỉ an toàn vượt quá {1} ký tự.")]
        public string SavetyCertificates { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        [Display(Name = "Ngày hết hạn chứng chỉ an toàn")]
        public DateTime? SavetyCertificatesExpire { get; set; }

        [Display(Name = "Số giấy đào tạo nhiệp vụ PCCC")]
        [StringLength(20, ErrorMessage = "Nhập số giấy đào tạo nhiệp vụ phòng cháy chữa cháy {2} đến {1} ký tự.", MinimumLength = 4)]
        public string  FireTrainingLicense { get; set; }

        [Display(Name = "Số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại {2} đến {1} ký tự.", MinimumLength = 4)]
        //[RegularExpression("^[0-9]{8,15}$", ErrorMessage = "Số điện thoại nhập không đúng")]    
        public string PhoneNumber { get; set; }

    }
}
