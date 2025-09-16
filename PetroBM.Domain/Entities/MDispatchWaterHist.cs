using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetroBM.Domain.Entities
{
    [Table("MDispatchWater_Hist")]
    public partial class MDispatchWaterHist
    {
        // ---- Composite Key: (DispatchID, VersionNo) ----
        [Key, Column(Order = 0)]
        public int DispatchID { get; set; }

        [Key, Column(Order = 1)]
        public int VersionNo { get; set; }

        public int? CertificateNumber { get; set; }

        public DateTime? TimeOrder { get; set; }
        public DateTime? CertificateTime { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeStop { get; set; }

        [StringLength(100)]
        public string VehicleNumber { get; set; }

        [StringLength(200)]
        public string DriverName1 { get; set; }

        [StringLength(200)]
        public string DriverName2 { get; set; }

        [StringLength(100)]
        public string ProductCode { get; set; }

        [StringLength(200)]
        public string Department { get; set; }

        [StringLength(200)]
        public string DstPickup1 { get; set; }

        [StringLength(200)]
        public string DstPickup2 { get; set; }

        [StringLength(200)]
        public string DstReceive { get; set; }

        [StringLength(200)]
        public string Note1 { get; set; }

        [StringLength(200)]
        public string Note2 { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        [StringLength(10)]
        public string Status { get; set; } // NVARCHAR(10)

        [Required]
        public DateTime InsertDate { get; set; }

        [Required, StringLength(50)]
        public string InsertUser { get; set; }

        [Required]
        public DateTime UpdateDate { get; set; }

        [Required, StringLength(50)]
        public string UpdateUser { get; set; }

        [Required]
        public bool DeleteFlg { get; set; }

        [StringLength(500)]
        [Column("From")]
        public string From { get; set; }

        [StringLength(500)]
        public string To { get; set; }

        [StringLength(500)]
        public string Paragraph1 { get; set; }

        [StringLength(500)]
        public string Paragraph2 { get; set; }

        [StringLength(500)]
        public string Paragraph3 { get; set; }

        [StringLength(500)]
        public string Paragraph4 { get; set; }

        [Required]
        public int ProcessStatus { get; set; }

        [StringLength(100)]
        public string SysU { get; set; }

        public DateTime? SysD { get; set; }

        [StringLength(100)]
        public string TransactionId { get; set; }
    }
}
