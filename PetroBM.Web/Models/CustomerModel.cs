using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Web;

namespace PetroBM.Web.Models
{
    public class CustomerModel
    {
        public CustomerModel()
        {
            Customer = new MCustomer();
            CustomerGroup = new MCustomerGroup();
            ListCustomers = new List<MCustomer>();
            ListCustomerGroup = new List<MCustomerGroup>();
            ListSelectedField = new List<string>();
        }

        public MCustomer Customer { get; set; }

        public MCustomerGroup CustomerGroup { get; set; }
        public IPagedList<MCustomer> ListCustomer { get; set; }
        public IEnumerable<MCustomerGroup> ListCustomerGroup { get; set; }
        public IEnumerable<MCustomer> ListCustomers { get; set; }
        public List<string> ListSelectedField { get; set; }
        public HttpPostedFileBase ImportFile { get; set; }
        public string CustomerCode { get; set; }
        public string TaxCode { get; set; }
    }
}