using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PetroBM.Web.Models
{
    public class DriverModel
    {
        public DriverModel()
        {
            Driver = new MDriver();
            ListSelectedField = new List<string>();
        }

        public MDriver Driver { get; set; }
        public IPagedList<MDriver> ListDriver { get; set; }
        public List<string> ListSelectedField { get; set; }
        public HttpPostedFileBase ImportFile { get; set; }


        [Required(ErrorMessage = "Nhập ngày sinh không đúng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string BirthDate { get; set; }

        public string LicenseDate { get; set; }

        [Required(ErrorMessage = "Nhập ngày hết hạn không đúng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string DriversLicenseExpire { get; set; }

        [Required(ErrorMessage = "Nhập ngày hết hạn chứng chỉ an toàn không đúng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string SavetyCertificatesExpire { get; set; }

        public string Name { get; set; }
        public string IdentificationNumber { get; set; }

    }
}