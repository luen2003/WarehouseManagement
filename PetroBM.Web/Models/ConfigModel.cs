using PetroBM.Domain.Entities;
using System.Collections.Generic;

namespace PetroBM.Web.Models
{
    public class ConfigModel
    {
        public MProduct Product { get; set; }
        public MTank Tank { get; set; }
        public MTankGrp TankGroup { get; set; }
        public IEnumerable<MProduct> ProductList { get; set; }
        public IEnumerable<MTank> TankList { get; set; }
        public IEnumerable<MTankGrp> TankGrpList { get; set; }
        public ConfigModel()
        {
            Product = new MProduct();
            Tank = new MTank();
            TankGroup = new MTankGrp();
            ProductList = new List<MProduct>();
            TankList = new List<MTank>();
            TankGrpList = new List<MTankGrp>();
        }
    }
}