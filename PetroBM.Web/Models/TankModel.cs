using PagedList;
using PetroBM.Common.Util;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class TankModel
    {
        public TankModel()
        {
            Tank = new MTank();
            TankDensity = new MTankDensity();
            TankLive = new MTankLive();
            TankManual = new MTankManual();
            ListBarem = new List<MBarem>();
            DateCreateTankManual = DateTime.Now.ToString(Constants.DATE_FORMAT);
        }

        public MTank Tank { get; set; }
        public MProduct Product { get; set; }
        public IPagedList<MTank> ListTank { get; set; }
        public MTankDensity TankDensity { get; set; }
        public IPagedList<MTankDensity> ListTankDensity { get; set; }
        public IEnumerable<MProduct> ListProduct { get; set; }
        public MTankLive TankLive { get; set; }
        public String DateCreateTankManual { get; set; }
        public MTankManual TankManual { get; set; }
        public IPagedList<MTankManual> ListTankManual { get; set; }
        public MAlarm Alarm { get; set; }
        public MBarem Barem { get; set; }
        public IEnumerable<MWareHouse> ListWareHouse { get; set; }
        public IEnumerable<MBarem> ListBarem { get; set; }
        public int? TankId { get; set; }
        public int? ProductId { get; set; }
        public byte WareHouseCode { get; set; }
        public float RatioImage { get; set; }

        public HttpPostedFileBase BaremFile { get; set; }
    }
}