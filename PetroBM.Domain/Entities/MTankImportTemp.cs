namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankImportTemp")]
    public partial class MTankImportTemp
    {
        [Key]
        [Column(Order = 0)]
        public int ImportInfoId { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte WareHouseCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TankId { get; set; }

        public DateTime? StartDate { get; set; }

        public float? StartTemperature { get; set; }

        public float? StartProductLevel { get; set; }

        public float? StartDensity { get; set; }

        public DateTime? EndDate { get; set; }

        public float? EndTemperature { get; set; }

        public float? EndProductLevel { get; set; }   

        public virtual MImportInfo MImportInfo { get; set; }

        public virtual MTank MTank { get; set; }
    }
}
