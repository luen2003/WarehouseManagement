namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MUserGroupUser")]
    public partial class MUserGroupUser
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string UserName { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GrpId { get; set; }

        public DateTime InsertDate { get; set; }

        [Required]
        [StringLength(20)]
        public string InsertUser { get; set; }

        public DateTime UpdateDate { get; set; }

        [Required]
        [StringLength(20)]
        public string UpdateUser { get; set; }

        public int VersionNo { get; set; }

        public bool DeleteFlg { get; set; }
    }
}
