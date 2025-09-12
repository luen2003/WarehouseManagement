namespace PetroBM.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MConfigArmGrpConfigArm")]
    public partial class MConfigArmGrpConfigArm
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Mã nhóm")]
        public int ConfigArmGrpId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Mã họng")]
        public int ConfigArmId { get; set; }
    }
}
