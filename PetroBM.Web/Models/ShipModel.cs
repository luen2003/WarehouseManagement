using PagedList;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class ShipModel
    {
        public ShipModel()
        {
            Ship = new MShip();
        }

        public MShip Ship { get; set; }
        public IPagedList<MShip> ListShip { get; set; }
        public string ShipCode { get; set; }
        public string ShipName { get; set; }
        public string NumberControl { get; set; }
        public string ManagementUnit { get; set; }
        public string Note { get; set; }
        public string ShipStatus { get; set; }
    }
}
