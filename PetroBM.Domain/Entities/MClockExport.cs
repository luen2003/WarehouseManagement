namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MClockExport")]
    public partial class MClockExport
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ImportInfoId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClockId { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0}", ApplyFormatInEditMode = true)]
        public float? StartVtt { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0}", ApplyFormatInEditMode = true)]
        public float? EndVtt { get; set; }

        public virtual MClock MClock { get; set; }

       // public virtual MImportInfo MImportInfo { get; set; }
    }
}
