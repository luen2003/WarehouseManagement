namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MProduct")]
    public partial class MProduct : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MProduct()
        {
            //MTanks = new HashSet<MTank>();
        }

        [Key]
        [Display(Name = "Id")]
        public int ProductId { get; set; }


        [Display(Name = "Mặt hàng")]
        [Required(ErrorMessage = "(*) Mặt hàng bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "Mặt hàng từ {2} đến {1} kí tự",MinimumLength =2)]
        public string ProductName { get; set; }

        [Display(Name = "Viết tắt")]
        [Required(ErrorMessage = "(*) Viết tắt mặt hàng buộc nhập.")]
        [StringLength(50, ErrorMessage = "Mặt hàng từ {2} đến {1} kí tự", MinimumLength = 2)]
        public string Abbreviations { get; set; }

        [Display(Name = "Mã hàng")]
        [Required(ErrorMessage ="(*) Mã hàng bắt buộc nhập.")]
        [StringLength(6, ErrorMessage = "Mã hàng tối đa {1}")]
        public string ProductCode { get; set; }

        [StringLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        [Display(Name = "Màu sắc")]
        public string Color { get; set; }

        [Display(Name = "Mã hàng gốc")]        
        public string OriginalProductCode { get; set; }

        [Display(Name = "Mã hàng trộn")]
        public string MixProductCode { get; set; }

        [Display(Name = "Định mức hao hụt nhập")]
        public double? InputWastageRate { get; set; }

        [Display(Name = "Định mức hao hụt xuất")]
        public double? ExportWastageRate { get; set; }

		[Display(Name = "Định mức hao hụt tồn chứa")]
		public double? StoreWastageRate { get; set; }

		[StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Loại hàng hóa")]
        public string ProductType { get; set; }

        public int ShowRank { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [NotMapped]
        public virtual ICollection<MTank> MTanks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MImportInfo> MImportInfo { get; set; }
    }
}
