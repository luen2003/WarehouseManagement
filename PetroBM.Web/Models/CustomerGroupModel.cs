using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Web;

namespace PetroBM.Web.Models
{
    public class CustomerGroupModel
    {
        public CustomerGroupModel()
        {
            CustomerGroup = new MCustomerGroup();
            ListCustomersGroup = new List<MCustomerGroup>();
            ListSelectedField = new List<string>();
        }

        public MCustomerGroup CustomerGroup { get; set; }
        public IPagedList<MCustomerGroup> ListCustomerGroup { get; set; }
        public IEnumerable<MCustomerGroup> ListCustomersGroup { get; set; }
        public List<string> ListSelectedField { get; set; }
        public HttpPostedFileBase ImportFile { get; set; }
        public string CustomerGroupCode { get; set; }
        public string TaxCode { get; set; }
    }
}