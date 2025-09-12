using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Domain.Entities
{
    [Table("MSeal")]
    public partial class MSeal:BaseEntity
    {
        public int ID { get; set; }

        public int CommandID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? WorkOrder { get; set; }

        [StringLength(20)]
        public string V_Actual { get; set; }

        public byte? CompartmentOrder { get; set; }

        public double? Vtt { get; set; }

        [StringLength(20)]
        public string ProductName { get; set; }

        [StringLength(10)]
        public string Seal1 { get; set; }

        [StringLength(10)]
        public string Seal2 { get; set; }
        [StringLength(10)]
        public string Ratio { get; set; }

        [StringLength(50)]
        public string CardData { get; set; }

        public long? CardSerial { get; set; }

        public DateTime? TimeOrder { get; set; }
    }
}
