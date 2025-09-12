namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MShip")]
    public partial class MShip : BaseEntity
    {
        public int ID { get; set; }

        [Display(Name = "Mã tàu")]
        [StringLength(6, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Mã tàu")]
        public string ShipCode { get; set; }

        [Display(Name = "Tên tàu")]
        [StringLength(200, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Tên tàu")]
        public string ShipName { get; set; }

        [Display(Name = "Biển kiểm soát")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Biển kiểm soát")]
        public string NumberControl { get; set; }

        [Display(Name = "Đơn vị quản lý")]
        [StringLength(200, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Đơn vị quản lý")]
        public string ManagementUnit { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Trạng thái")]
        [StringLength(50, ErrorMessage = "Tối đa {1} kí tự")]
        [Required(ErrorMessage = "Trạng thái")]
        public string ShipStatus { get; set; }

    }
}
