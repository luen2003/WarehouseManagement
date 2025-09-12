using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Domain.Entities
{
    [Table("MImage")]
    public class MImage
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(10)]
        public string ImageCode { get; set; }

        [Required]
        [StringLength(50)]
        public string ImageName { get; set; }

        [Required]
        [StringLength(2048)]
        public string ImageURL { get; set; }

        [Required]
        [StringLength(50)]
        public string ImagePosition { get; set; }

        [Required]
        [StringLength(50)]
        public string ImageUser { get; set; }

        [Required]
        public int ProcessStatus { get; set; }

    }
}