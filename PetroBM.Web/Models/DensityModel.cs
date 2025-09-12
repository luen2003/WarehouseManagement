using PagedList;
using PetroBM.Domain.Entities;
using System.Collections.Generic;
using PetroBM.Common.Util;
using System;

namespace PetroBM.Web.Models
{
    public class DensityModel
    {
        public DensityModel()
        {
            StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);
            Density = new MDensity();
            ListSelectedField = new List<string>();
            ListWareHouse = new List<MWareHouse>();
            ListProductTemp = new List<ProductTemp>();
        }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string WareHouseName { get; set; }
        public byte? WareHouseCode { get; set; }
        public byte? ArmNo { get; set; }

        public MDensity Density { get; set; }
        public IPagedList<MDensity> ListDensity { get; set; }

        public List<MProduct> ListProduct { get; set; }
        public List<ProductTemp> ListProductTemp { get; set; }
        public IEnumerable<MWareHouse> ListWareHouse { get; set; }
        //public IEnumerable<MConfigArm> ListConfigArm { get; set; }
        public List<MConfigArm> ListConfigArm { get; set; }

        public List<string> ListSelectedField { get; set; }
    }
}