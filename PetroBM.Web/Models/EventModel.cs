using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetroBM.Domain.Entities;
using PagedList;
using PetroBM.Common.Util;

namespace PetroBM.Web.Models
{
    public class EventModel
    {
        public EventModel()
        {
            StartDate = DateTime.Now.AddDays(Constants.DAYS_CALENDAR_OFFSET).ToString(Constants.DATE_FORMAT);
              EndDate = DateTime.Now.AddMinutes(1).ToString(Constants.DATE_FORMAT);

            EventTypeList = new List<MEventType>();
        }

        public IPagedList<MEvent> EventList { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }

        public string User { get; set; }
        public int? EventTypeId { get; set; }
        public IEnumerable<MEventType> EventTypeList { get; set; }
    }
}