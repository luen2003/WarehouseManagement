namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MTank")]
    public partial class MTank : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTank()
        {
            MAlarms = new HashSet<MAlarm>();
            MBarems = new HashSet<MBarem>();
            MTankLogs = new HashSet<MTankLog>();
            MTankLives = new HashSet<MTankLive>();
            MTankManuals = new HashSet<MTankManual>();
            MTankGrps = new HashSet<MTankGrp>();
            MTankImport = new HashSet<MTankImport>();
        }

        [Key]
        [Column(Order = 0)]
        public byte WareHouseCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TankId { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Tên bể")]
        public string TankName { get; set; }

        public int? ProductId { get; set; }

        [Display(Name = "Mức cao")]
        public float? HighLevel { get; set; }

        [Display(Name = "Mức rất cao")]
        public float? HighHighLevel { get; set; }

        [Display(Name = "Mức thấp")]
        public float? LowLevel { get; set; }

        [Display(Name = "Mức rất thấp")]
        public float? LowLowLevel { get; set; }

        [Display(Name = "Chiều cao Min")]
        public float? HighMin { get; set; }

        [Display(Name = "Chiều cao Max")]
        public float? HighMax { get; set; }

        [Range(0, 100, ErrorMessage = "Nhiệt độ nằm trong khoảng 0-100")]
        [Display(Name = "Nhiệt độ Min")]
        public float? TemperatureLow { get; set; }

        [Range(0, 100, ErrorMessage = "Nhiệt độ nằm trong khoảng 0-100")]
        [Display(Name = "Nhiệt độ Max")]
        public float? TemperatureHigh { get; set; }

        [Display(Name = "Lưu lượng Min")]
        public float? LowFlow { get; set; }

        [Display(Name = "Lưu lượng Max")]
        public float? HighFlow { get; set; }

        [Display(Name = "Thể tích Max")]
        public double? VolumeMax { get; set; }

        [Display(Name = "Offset Hàng")]
        public double? ProductOffset { get; set; }

        [Display(Name = "Offset Nước")]
        public float? WaterOffset { get; set; }

        [Display(Name = "Offset Nhiệt độ")]
        public float? TemperatureOffset { get; set; }

        [Display(Name = "Tỷ Trọng(D15)")]
        public float? Density { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAlarm> MAlarms { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MBarem> MBarems { get; set; }

        public virtual MProduct MProduct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankLog> MTankLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankLive> MTankLives { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankManual> MTankManuals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankGrp> MTankGrps { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankDensity> MTankDensity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankImport> MTankImport { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTankImportTemp> MTankImportTemps { get; set; }
    }
}
