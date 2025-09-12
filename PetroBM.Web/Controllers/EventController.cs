using PagedList;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetroBM.Common.Util;
using PetroBM.Web.Attribute;
using System.Globalization;

namespace PetroBM.Web.Controllers
{
    public class EventController : BaseController
    {
        private readonly IEventService eventService;
        public EventModel eventModel;

        public EventController(IEventService eventService, EventModel eventModel)
        {
            this.eventService = eventService;
            this.eventModel = eventModel;
        }

        [HasPermission(Constants.PERMISSION_ALARM)]
        public ActionResult Index(EventModel eventModel, int? page)
        {
            int pageNumber = (page ?? 1);

            DateTime? startDate = null;
            DateTime? endDate = null;

            eventModel.EventTypeList = eventService.GetAllEventType();

            if (!String.IsNullOrEmpty(eventModel.StartDate))
            {
                startDate = DateTime.ParseExact(eventModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(eventModel.EndDate))
            {
                endDate = DateTime.ParseExact(eventModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            eventModel.EventList = eventService.GetEventByTime(startDate, endDate, eventModel.User, eventModel.EventTypeId)
            .ToPagedList(pageNumber, Constants.PAGE_SIZE);

            return View(eventModel);
        }

        public PartialViewResult TopEvent()
        {
            eventModel.EventList = eventService.GetTopEvent(Constants.EVENT_NUMBER)
                .ToPagedList(1, Constants.PAGE_SIZE); ;
            return PartialView(eventModel);
        }
    }
}