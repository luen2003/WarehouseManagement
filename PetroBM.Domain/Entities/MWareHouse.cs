namespace PetroBM.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MWareHouse")]
    public partial class MWareHouse:BaseEntity
    {
        public int Id { get; set; }

        [Display(Name = "Mã kho")]
        [Required(ErrorMessage = "*")]
        [RegularExpression("1?[0-9]{1,2}|2[0-4][0-9]|25[0-5]", ErrorMessage = "Mã kho phải nhập số < 255")]
        [DataType(DataType.Text)]
        public byte? WareHouseCode { get; set; }

        [StringLength(50,ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Tên kho")]
        [Required(ErrorMessage = "*")]
        public string WareHouseName { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(250, ErrorMessage = "Tối đa 250 kí tự")]
        [Display(Name = "Địa chỉ kho")]
        public string WareHouseAddress { get; set; }

        [StringLength(50)]
        [Display(Name = "Số điện thoại kho")]
        [RegularExpression("^[0]{1}[0-9]{9,10}$", ErrorMessage = "Số điện thoại nhập không đúng")]
        public string WareHousePNumber { get; set; }

    }
}
