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
    public class ShipController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ShipController));

        private readonly IShipService shipService;
        public ShipModel shipModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;
        public ShipController(IShipService shipService, ShipModel shipModel, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.shipService = shipService;
            this.shipModel = shipModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.baseService = baseService;
        }
        public ActionResult Index(int? page, ShipModel shipModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTCUSTOMER);
            if (checkPermission)
            {

                try
                {
                    log.Info("Ship Index");
                    int pageNumber = (page ?? 1);
                    if (shipModel.ShipCode == null && shipModel.ShipName == null)
                    {
                        shipModel.ListShip = shipService.GetAllShip().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        if (shipModel.ShipCode != null)
                        {
                            if (shipModel.ShipName != null)
                                shipModel.ListShip = shipService.GetAllShip().
                                    Where(x => x.ShipCode.Contains(shipModel.ShipCode) && x.ShipName.Contains(shipModel.ShipName)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                            else
                                shipModel.ListShip = shipService.GetAllShip().Where(x => x.ShipCode.Contains(shipModel.ShipCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        }
                        else
                        {     
                            shipModel.ListShip = shipService.GetAllShip().Where(x => x.ShipName.Contains(shipModel.ShipName)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return View(shipModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }




        [HttpGet]
        public ActionResult Create()
        {
            log.Info("Ship Create");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTSHIP);
            if (checkPermission)
            {
                var shipModel = new ShipModel
                {
                    Ship = new MShip()
                };
                return View(shipModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(ShipModel shipModel)
        {
            log.Info("Start controlller Ship HttpPost Create");
            try
            {
                shipModel.Ship.InsertUser = HttpContext.User.Identity.Name;
                shipModel.Ship.UpdateUser = HttpContext.User.Identity.Name;

                var rs = shipService.CreateShip(shipModel.Ship);

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
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTSHIP);
            if (checkPermission)
            {
                var shipModel = new ShipModel
                {
                    Ship = shipService.FindShipById(id)
                };
                return View(shipModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(ShipModel shipModel)
        {
            log.Info("Start controlller Ship HttpPost Edit");
            try
            {
                var ship = shipService.FindShipById(shipModel.Ship.ID);
                if (ship != null)
                {
                    ship.UpdateUser = HttpContext.User.Identity.Name;
                    ship.UpdateDate = DateTime.Now;
                    ship.ShipCode = shipModel.Ship.ShipCode;
                    ship.ShipName = shipModel.Ship.ShipName;
                    ship.NumberControl = shipModel.Ship.NumberControl;
                    ship.ManagementUnit = shipModel.Ship.ManagementUnit;
                    ship.Note = shipModel.Ship.Note;
                    ship.ShipStatus = shipModel.Ship.ShipStatus;

                    var rs = shipService.UpdateShip(ship);
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
            log.Info("Finish controlller Ship HttpPost Edit");
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            log.Info("Start controlller Ship HttpPost Delete");
            try
            {
                var rs = shipService.DeleteShip(id);
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
