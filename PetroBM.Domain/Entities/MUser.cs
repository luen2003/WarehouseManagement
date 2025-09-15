namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MUser")]
    public partial class MUser : BaseEntity
    {
        [Key]
        [StringLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        [Required(ErrorMessage = "*")]
        [Display(Name = "Tên người dùng")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Mật mã")]
        public string Passwd { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Vị trí")]
        public string Position { get; set; }

        // public int? GrpId { get; set; }

        [StringLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string SelectedFields { get; set; }

        [StringLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string SelectedConfigArmFields { get; set; }
        
        public int JobTitles { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "UserID")]
        public string UserID { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        //[Display(Name = "Nhóm người dùng")]
        //public virtual MUserGrp MUserGrp { get; set; }



    }
}
