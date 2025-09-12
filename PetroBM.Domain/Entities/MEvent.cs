namespace PetroBM.Domain.Entities
{
    using Common.Util;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MEvent")]
    public partial class MEvent
    {
        [Key]
        public int EventId { get; set; }

        public int TypeId { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(1000)]
        public string EventName { get; set; }

        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING, ApplyFormatInEditMode = true)]
        public DateTime InsertDate { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(20)]
        public string InsertUser { get; set; }

        public virtual MEventType MEventType { get; set; }
    }
}
