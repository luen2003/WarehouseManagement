namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankGrp")]
    public partial class MTankGrp : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTankGrp()
        {
            MTanks = new HashSet<MTank>();
            MTankGrpTanks = new HashSet<MTankGrpTank>();
        }

        [Key]
        [Display(Name = "Id")]
        public int TankGrpId { get; set; }

        [Display(Name = "Kho")]
        public byte  WareHouseCode { get; set; }

        [Required(ErrorMessage ="Tên nhóm bể bắt buộc nhập.")]
        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Tên nhóm bể")]
        public string TanktGrpName { get; set; }

        [Display(Name = "Danh sách bể")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTank> MTanks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankGrpTank> MTankGrpTanks { get; set; }
    }
}
