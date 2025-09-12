namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MBarem")]
    public partial class MBarem : BaseEntity
    {
        public int Id { get; set; }

        public byte WareHouseCode { get; set; }

        public int TankId { get; set; }

        [Display(Name = "Chiều cao")]
        public float High { get; set; }

        [Display(Name = "Thể tích")]
        public float Volume { get; set; }

        public virtual MTank MTank { get; set; }




    }
}
