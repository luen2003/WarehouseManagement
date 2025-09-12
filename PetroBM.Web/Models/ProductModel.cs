using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class ProductModel
    {
        public ProductModel()
        {
            Product = new MProduct();
            ListTank = new List<MTank>();
            ListSelectedField = new List<string>();
        }

        public MProduct Product { get; set; }
        public MTank Tank { get; set; }
        public MTankLive TankLive { get; set; }
        public IPagedList<MProduct> ListProduct { get; set; }
        public IEnumerable<MTank> ListTank { get; set; }
        public List<string> ListSelectedField { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }

    }
}