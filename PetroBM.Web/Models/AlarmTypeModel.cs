using PetroBM.Common.Util;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetroBM.Web.Models
{
    public class AlarmTypeModel
    {
        public AlarmTypeModel()
        {
            StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
            EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);

            AlarmType = new MAlarmType();
            AlarmTypeList = new List<MAlarmType>();
        }
        public MAlarmType AlarmType { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
        public IEnumerable<MAlarmType> AlarmTypeList { get; set; }
    }
}