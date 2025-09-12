namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MExportArmImport")]
    public partial class MExportArmImport
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ImportInfoId { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte ArmNo { get; set; }

        [Key]
        [Column(Order = 2)]
        public byte WareHouseCode { get; set; }

        public int ProductId { get; set; }

        [StringLength(6)]
        public string ProductCode { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        public bool DeleteFlag { get; set; }

        public float? StartTotal { get; set; }

        public float? StartTotalBase { get; set; }

        public float? StartTotalE { get; set; }

        public float? EndTotal { get; set; }

        public float? EndTotalBase { get; set; }

        public float? EndTotalE { get; set; }
    }
}
