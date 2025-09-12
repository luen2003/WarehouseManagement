namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MDataLogArm")]
    public partial class MDataLogArm
    {
        [Key]
        [Column(Order = 0)]
        public DateTime TimeLog { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte WareHouseCode { get; set; }

        [Key]
        [Column(Order = 2)]
        public byte ArmNo { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? WorkOrder { get; set; }

        public byte? CompartmentOrder { get; set; }

        [StringLength(6)]
        public string ProductCode { get; set; }

        [StringLength(50)]
        public string CardData { get; set; }

        public long? CardSerial { get; set; }

        public float? V_Preset { get; set; }

        public float? V_Actual { get; set; }

        public float? V_Actual_Base { get; set; }

        public float? V_Actual_E { get; set; }

        public float? Flowrate { get; set; }

        public float? Flowrate_Base { get; set; }

        public float? Flowrate_E { get; set; }

        public float? AvgTemperature { get; set; }

        public float? CurrentTemperature { get; set; }

        public float? MixingRatio { get; set; }

        public float? ActualRatio { get; set; }

        public byte? Status { get; set; }

        public byte? ModeLog { get; set; }

        public byte? ESD { get; set; }

        public float? Total { get; set; }

        public float? TotalBase { get; set; }

        public float? TotalE { get; set; }
    }
}
