using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetroBM.Common.Util;

namespace PetroBM.Domain.Entities
{
    public class BaseEntity
    {
        [Display(Name = "Ngày nhập")]
        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING, ApplyFormatInEditMode = true)]
        public DateTime InsertDate { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(15)]
        [Display(Name = "Người nhập")]
        public string InsertUser { get; set; }

        [Display(Name = "Ngày sửa")]
        [DisplayFormat(DataFormatString = Constants.DATE_FORMAT_STRING, ApplyFormatInEditMode = true)]
        public DateTime UpdateDate { get; set; }

        [Required(ErrorMessage ="*")]
        [StringLength(15)]
        [Display(Name = "Người sửa")]
        public string UpdateUser { get; set; }

        public int VersionNo { get; set; }

        [Required(ErrorMessage ="*")]
        public bool DeleteFlg { get; set; }
    }
}
