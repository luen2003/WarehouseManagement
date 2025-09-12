using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetroBM.Web.Models
{
    public class TankGroupModel
    {
        public TankGroupModel()
        {
            TankGrp = new MTankGrp();
            AlertMessage = "";
            ListSelectedField = new List<string>();
        }
        public MTankLive TankLive { get; set; }
        public MTank Tank { get; set; }
        public MProduct Product { get; set; }
        public MTankGrp TankGrp { get; set; }
        public IPagedList<MTankGrp> ListTankGrp { get; set; }
        public List<SelectListItem> ListTank { get; set; }
        public IEnumerable<MTank> Tanks { get; set; }
        public IEnumerable<MProduct> Products { get; set; }
        public IEnumerable<MTankLive> TankLives { get; set; }
        public IEnumerable<MWareHouse> WareHouses { get; set; }
        public IEnumerable<TankTempModel> TankTemps { get; set; }
        public IEnumerable<TankTempModel> SelectedTankTemps { get; set; } //xu li khi sua

        public string AlertMessage { get; set; }
        public int[] ListTankId { get; set;}

        public List<string> ListSelectedField { get; set; }
    }
}