using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class VehicleModel
    {
        public VehicleModel()
        {
            Vehicle = new MVehicle();
            Card = new List<MCard>();
            ListVehicle = new List<MVehicle>();
            ListSelectedField = new List<string>();
        }

        public MVehicle Vehicle { get; set; }
        public IEnumerable<MCard> Card { get; set; }
        public IPagedList<MVehicle> ListVehiclePageList { get; set; }
        public IEnumerable<MVehicle> ListVehicle { get; set; }
        public List<string> ListSelectedField { get; set; }
        public HttpPostedFileBase ImportFile { get; set; }
        public  string ExpireDate { get; set; }
        public string AccreditationExpire { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverDefault { get; set; }
        public string FirePreventExpire { get; set; }
        public List<Datum> LstDriver { get; set; }
        public long? CardSerial { get; set; }


    }
}