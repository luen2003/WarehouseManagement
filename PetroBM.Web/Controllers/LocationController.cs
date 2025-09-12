using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using PetroBM.Web.Attribute;
using log4net;
using System;
using System.Collections.Generic;

namespace PetroBM.Web.Controllers
{
    public class LocationController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(LocationController));

        private readonly ILocationService locationService;
        public LocationModel locationModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public LocationController(ILocationService locationService, LocationModel locationModel, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.locationService = locationService;
            this.locationModel = locationModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page, LocationModel locationModel)
        {
            log.Info("Location Index");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTLOCATION);
            if (checkPermission)
            {
                try
                {
                    int pageNumber = (page ?? 1);
                    locationModel.ListLocation = locationService.GetAllLocation().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return View(locationModel);
            }
            else
            {
                log.Info("You don't have permission to access this function");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Info("Location Create - GET");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTLOCATION);
            if (checkPermission)
            {
                var locationModel = new LocationModel
                {
                    Location = new MLocation()
                };
                return View(locationModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(LocationModel locationModel)
        {
            log.Info("Start controlller Location HttpPost Create");
            try
            {
                locationModel.Location.InsertUser = HttpContext.User.Identity.Name;
                locationModel.Location.UpdateUser = HttpContext.User.Identity.Name;

                var rs = locationService.CreateLocation(locationModel.Location);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish controlller Location HttpPost Create");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTLOCATION);
            if (checkPermission)
            {
                var locationModel = new LocationModel
                {
                    Location = locationService.FindLocationById(id)
                };
                // locationModel.Location = locationService.FindLocationById(id);
                return View(locationModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(LocationModel locationModel)
        {
            log.Info("Start controlller Location HttpPost Edit");
            try
            {
                var location = locationService.FindLocationById(locationModel.Location.ID);
                if (location != null)
                {
                    location.UpdateUser = HttpContext.User.Identity.Name;
                    location.UpdateDate = DateTime.Now;
                    location.LocationCode = locationModel.Location.LocationCode;
                    location.LocationName = locationModel.Location.LocationName;
                    location.LocationAddress = locationModel.Location.LocationAddress;
                    location.Note = locationModel.Location.Note;
                    location.LocationStatus = locationModel.Location.LocationStatus;

                    var rs = locationService.UpdateLocation(location);
                    if (rs)
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    }
                    else
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish controlller Location HttpPost Edit");
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            log.Info("Start controlller Location HttpPost Delete");
            try
            {
                var rs = locationService.DeleteLocation(id);
                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish controlller Location HttpPost Delete");
            return RedirectToAction("Index");
        }
    }
}
