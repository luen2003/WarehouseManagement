namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTankLiveChart")]
    public partial class MTankLiveChart
    {
        [Key]
        [Column(Order = 0)]
        public DateTime Time { get; set; }

        [Key]
        [Column(Order = 1)]
        public byte WareHouseCode { get; set; }

        [Key]
        [Column(Order = 2)]
        public byte ArmNo { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? WorkOrder { get; set; }

        public byte? CompartmentOrder { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 6 kí tự")]
        public string ProductCode { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
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

        public byte? Mode { get; set; }

        public byte? ESD { get; set; }
    }
}
