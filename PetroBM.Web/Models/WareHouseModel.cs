using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;

namespace PetroBM.Web.Models
{
    public class WareHouseModel
    {
        public WareHouseModel()
        {
            WareHouse = new MWareHouse();
           //  ListWareHouse = new List<MWareHouse>();
            ListSelectedField = new List<string>();
            ListEnumableWareHouse = new List<MWareHouse>();
        }
        public int? Id { get; set; }
        public string WareHouseName { get; set; }
        public MWareHouse WareHouse { get; set; }
        public IPagedList<MWareHouse> ListWareHouse { get; set; }

        public IEnumerable<MWareHouse> ListEnumableWareHouse { get; set; }

        public List<string> ListSelectedField { get; set; }
    }
}