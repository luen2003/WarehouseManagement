namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MLocation")]
    public partial class MLocation : BaseEntity
    {
        public int ID { get; set; }

        [Display(Name = "Mã địa điểm")]
        [StringLength(6, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Mã địa điểm")]
        public string LocationCode { get; set; }

        [Display(Name = "Tên địa điểm")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Tên địa điểm")]
        public string LocationName { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Địa chỉ")]
        public string LocationAddress { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(200, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Trạng thái")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Trạng thái")]
        public string LocationStatus { get; set; }
    }
}
