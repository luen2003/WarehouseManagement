using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Domain.Entities
{
    [Table("MCard")]
    public partial class MCard : BaseEntity
    {

        public int ID { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Mã kho")]
        [Required(ErrorMessage = "(*) Mã kho bắt buộc nhập.")]
        public string WareHouseCode { get; set; }

        [StringLength(50,ErrorMessage ="Tối đa 50 kí tự")]
        [Required(ErrorMessage ="(*) Card Data bắt buộc nhập.")]
        public string CardData { get; set; }

        [Required(ErrorMessage = "(*) Card Serial bắt buộc nhập.")]
        public long? CardSerial { get; set; }

        [Display(Name = "Trạng thái kích hoạt")]
        public int? ActiveStatus { get; set; }

        [Display(Name = "Trạng thái sử dụng")]
        public int? UseStatus { get; set; }

        [Display(Name = "Người sử dụng")]
        public string UseUser { get; set; }

        [Display(Name = "Ngày sử dụng")]
        public string UseDate { get; set; } 


    }
}
