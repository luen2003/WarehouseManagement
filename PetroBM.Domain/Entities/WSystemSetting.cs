namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WSystemSetting")]
    public partial class WSystemSetting
    {
        [Key]
        [StringLength(100)]
        public string KeyCode { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(500)]
        public string Value { get; set; }
    }
}
