namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MImportInfo")]
    public partial class MImportInfo: BaseEntity
    {
       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MImportInfo()
        {
            MTankImport = new List<MTankImport>();
            MClockExport = new List<MClockExport>();
            MTankImportTemps = new List<MTankImportTemp>();
        }

        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }

        public byte WareHouseCode { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Required(ErrorMessage = "Phương tiện vân chuyển bắt buộc nhập.")]
        [Display(Name = "Phương tiện vận chuyển")]
        public string Vehicle { get; set; }

        [Required(ErrorMessage = "Loại hàng bắt buộc nhập.")]
        [Display(Name = "Loại hàng")]
        public int? ProductId { get; set; }

        [Required(ErrorMessage = "Số lượng xuất kho bắt buộc nhập")]
        [Display(Name = "Số lượng xuất kho(Theo BL)")]
        public float? Export { get; set; }

        [Required(ErrorMessage = "Số lượng phương tiện bắt buộc nhập")]
        [Display(Name = "Số lượng phương tiện Vtt")]
        public float? Vtt { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Số lượng phương tiện V15")]
        public float? V15 { get; set; }

        [Required(ErrorMessage = "Tỷ trọng D15 bắt buộc nhập.")]
        [Display(Name = "Tỷ trọng D15")]
        [Range(0.6, 1, ErrorMessage = "Tỷ trọng D15 nằm trong khoảng 0.6-1")]
        public float? Density { get; set; }

        [Required(ErrorMessage = "Hệ số VCF bắt buộc nhập.")]
        [Display(Name = "Hệ số VCF")]
        public float? VCF { get; set; }

        [Required(ErrorMessage = "Hao hụt định mức bắt buộc nhập.")]
        [Display(Name = "Hao hụt định mức nhập")]
        public double? InputWastageRate { get; set; }

        [Display(Name = "Nhà cung cấp")]
        public string VendorName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Số chứng từ")]
		public int? CertificateNumber { get; set; }

		[Display(Name = "Ngày chứng từ")]
		public DateTime? CertificateTime { get; set; }

		[Required(ErrorMessage = "*")]
        [Range(0, 100, ErrorMessage = "Nhiệt độ nằm trong khoảng 0-100")]
        [Display(Name = "Nhiệt độ")]
        public float? Temperature { get; set; }

        [Required(ErrorMessage = "*")]
        public bool StartFlag { get; set; }

        [Required(ErrorMessage = "*")]
        public bool EndFlag { get; set; }

        [Display(Name = "Thực xuất")]
        public float? V_Actual { get; set; }

        [Display(Name = "Thực xuất L15")]
        public float? V15_Actual { get; set; }

        // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        [NotMapped]
        public virtual IList<MClockExport> MClockExport { get; set; }

        public virtual MProduct MProduct { get; set; }

       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
         [NotMapped]
        public virtual IList<MTankImport> MTankImport { get; set; }

       // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
         [NotMapped]
        public virtual IList<MTankImportTemp> MTankImportTemps { get; set; }

        [NotMapped]
        public virtual MTankLive TankLive { get; set; }




    }
}
