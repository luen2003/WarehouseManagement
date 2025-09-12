using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class LocationModel
    {
        public LocationModel()
        {
            Location = new MLocation();
        }
        public MLocation Location { get; set; }
        public IPagedList<MLocation> ListLocation { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public string Note { get; set; }
        public string LocationStatus { get; set; }
    }
}
