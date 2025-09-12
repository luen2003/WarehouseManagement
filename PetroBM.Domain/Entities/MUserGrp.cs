namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MUserGrp")]
    public partial class MUserGrp : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MUserGrp()
        {
           // MUsers = new HashSet<MUser>();
            MUserGrpPermissions = new HashSet<MUserGrpPermission>();
        }

        [Key]
        [Display(Name = "Id")]
        public int GrpId { get; set; }

        public byte? WareHouseCode { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Tên nhóm")]
        public string GrpName { get; set; }

        [Column(TypeName = "ntext")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }       

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //[NotMapped]
        //public virtual ICollection<MUser> MUsers { get; set; }
        
        [Display(Name = "Phân quyền")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MUserGrpPermission> MUserGrpPermissions { get; set; }

    }
}
