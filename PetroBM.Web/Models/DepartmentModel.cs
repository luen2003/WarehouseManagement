using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Web;

namespace PetroBM.Web.Models
{
    public class DepartmentModel
    {
        public DepartmentModel()
        {
            Department = new MDepartment();
            ListDepartment = new List<MDepartment>(); 
            ListSelectedField = new List<string>();
        }

        public MDepartment Department { get; set; } 
        public IEnumerable<MDepartment> ListDepartment { get; set; }
        public List<string> ListSelectedField { get; set; }
        public HttpPostedFileBase ImportFile { get; set; } 
    }
}