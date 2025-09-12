using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace PetroBM.Web.Models
{
    public class PriceModel
    {
        public PriceModel()
        {
            Price = new MPrice();
            ListSelectedField = new List<string>();
        }

        public MPrice Price { get; set; }
        public IPagedList<MPrice> ListPrice { get; set; }
        public IEnumerable<SelectListItem>  Items { get; set; }
        public IEnumerable<OptionItem> Item2 { get; set; }
        public List<string> ListSelectedField { get; set; }
        public HttpPostedFileBase ImportFile { get; set; }
        public IEnumerable<ProductTemp> ProductTemps { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        public class OptionItem
        {
            public int val { get; set; }
            public string text { get; set;}
        }
    }
}