using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Common.Util;
using PagedList;
using PetroBM.Web.Attribute;
using System.Globalization;

namespace PetroBM.Web.Controllers
{
    public class AlarmController : BaseController
    {
        private readonly IUserService userService;
        private readonly ITankService tankService;
        private readonly IAlarmService alarmService;
        private readonly IWareHouseService wareHouseService;
        private readonly IConfigurationService configurationService;
        public AlarmModel alarmModel;

        public AlarmController(IUserService userService, ITankService tankService, 
            IAlarmService alarmService, IConfigurationService configurationService, 
            IWareHouseService wareHouseService,AlarmModel alarmModel)
        {
            this.userService = userService;
            this.tankService = tankService;
            this.alarmService = alarmService;
            this.configurationService = configurationService;
            this.alarmModel = alarmModel;
            this.wareHouseService = wareHouseService;
        }

        [HasPermission(Constants.PERMISSION_ALARM)]
        public ActionResult Index(AlarmModel alarmModel, int? page)
        {
            int pageNumber = (page ?? 1);

            DateTime? startDate = null;
            DateTime? endDate = null;

            alarmModel.TankList = tankService.GetAllTankOrderByName();
            alarmModel.AlarmTypeList = alarmService.GetAllAlarmType();
            string userName = HttpContext.User.Identity.Name;
            alarmModel.WareHouseList = wareHouseService.GetWareHouse_ByUserName(userName);
            if (alarmModel.WareHoueCode==0)
            {
                foreach (var item in alarmModel.WareHouseList)
                {
                    alarmModel.WareHoueCode = (byte)item.WareHouseCode;
                    break;
                }
            }

            if (!String.IsNullOrEmpty(alarmModel.StartDate))
            {
                startDate = DateTime.ParseExact(alarmModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!String.IsNullOrEmpty(alarmModel.EndDate))
            {
                endDate = DateTime.ParseExact(alarmModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            alarmModel.AlarmList = alarmService.GetAlarmByTimeAndWareHouse(startDate, endDate,alarmModel.AlarmTypeId, alarmModel.WareHoueCode)
                .ToPagedList(pageNumber, Constants.PAGE_SIZE);   
            return View(alarmModel);
        }

        [HasPermission(Constants.PERMISSION_ALARM)]
        public ActionResult Detail(int id)
        {
            alarmModel.Alarm = alarmService.GetAlarmById(id);

            return View(alarmModel);
        }

        public PartialViewResult TopAlarm()
        {
            alarmModel.TankList = tankService.GetAllTankOrderByName();
            alarmModel.AlarmTypeList = alarmService.GetAllAlarmType();
            var userName = HttpContext.User.Identity.Name;
            alarmModel.AlarmList = alarmService.GetTopAlarm(Constants.ALARM_NUMBER, userName).ToPagedList(1, Constants.PAGE_SIZE); ;
            return PartialView(alarmModel);
        }

        [HttpPost]
        public ActionResult CheckAlarmNotConfirmed()
        {
            string rs = string.Empty;
            var alarm = alarmService.GetAlarmNotConfirmed();

            if (alarm != null)
            {
                rs = alarm.MAlarmType.Sound == null ? Constants.NULL : alarm.MAlarmType.Sound;
            }

            return Content(rs);
        }

        [HttpPost]
        public JsonResult ConfirmAlarm(string password, int alarmId, string comment)
        {
            bool rs = false;

            string userName = HttpContext.User.Identity.Name;
            bool checkPass = userService.CheckPass(userName, password);

            if (checkPass)
            {
                var alarm = alarmService.GetAlarmById(alarmId);

                if (alarm != null)
                {
                    alarm.ConfirmUser = userName;
                    alarm.ConfirmComment = comment;
                    rs = alarmService.ConfirmAlarm(alarm);
                }
            }

            return Json(new { result = rs }, JsonRequestBehavior.AllowGet); ;
        }

        [HttpPost]
        public JsonResult SolveAlarm(string password, int alarmId, string comment)
        {
            bool rs = false;

            string userName = HttpContext.User.Identity.Name;
            bool checkPass = userService.CheckPass(userName, password);

            if (checkPass)
            {
                var alarm = alarmService.GetAlarmById(alarmId);

                if (alarm != null)
                {
                    alarm.SolveUser = userName;
                    alarm.SolveComment = comment;
                    rs = alarmService.SolveAlarm(alarm);
                }
            }

            return Json(new { result = rs }, JsonRequestBehavior.AllowGet); ;
        }
    }
}