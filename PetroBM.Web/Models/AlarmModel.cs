using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetroBM.Domain.Entities;
using PagedList;
using PetroBM.Common.Util;

namespace PetroBM.Web.Models
{
    public class AlarmModel
    {
        public AlarmModel()
        {
            StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
              EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);

            TankList = new List<MTank>();
            AlarmTypeList = new List<MAlarmType>();
        }

        public MAlarm Alarm { get; set; }
        public IPagedList<MAlarm> AlarmList { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
        public byte WareHoueCode { get; set; }
        public int? TankId { get; set; }
        public IEnumerable<MTank> TankList { get; set; }
        public int? AlarmTypeId { get; set; }
        public IEnumerable<MWareHouse> WareHouseList { get; set; }
        public IEnumerable<MAlarmType> AlarmTypeList { get; set; }
    }
}