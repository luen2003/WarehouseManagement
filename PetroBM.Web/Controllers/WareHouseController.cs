using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using System.Collections.Generic;
using log4net;
using System;
using Newtonsoft.Json;

namespace PetroBM.Web.Controllers
{

    public class WareHouseController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(WareHouseController));

        private readonly IWareHouseService warehouseService;
        private readonly ITankService tankService;
        private readonly ITankGroupService tankgrpService;
        private readonly IProductService productService;
        public WareHouseModel warehouseModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public WareHouseController(IProductService productService, ITankGroupService tankgrpService, IWareHouseService warehouseService, ITankService tankService, WareHouseModel warehouseModel, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.warehouseService = warehouseService;
            this.warehouseModel = warehouseModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.tankService = tankService;
            this.tankgrpService = tankgrpService;
            this.productService = productService;
            this.baseService = baseService;
        }
        public JsonResult GetTankbyWareHouseId(int warehouseid)
        {
            var listtank = new List<TankList>();
            var objwarehouse = warehouseService.GetAllWareHouse().Where(x => x.Id == warehouseid).ToList();
            foreach (var it in objwarehouse)
            {
                var warehousecode = it.WareHouseCode;
                var objtank = tankService.GetAllTank().Where(x => x.WareHouseCode == warehousecode).ToList();
                foreach (var item in objtank)
                {
                    var tank = new TankList();
                    tank.tankid = item.TankId;
                    tank.tankname = item.TankName;
                    listtank.Add(tank);
                }
            }
            return Json(listtank, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetTankIdbyTankName(string tankname)
        {
            List<string> tanknamelistobj = new List<string>();
            var obj = tankService.GetTankIdByTankName(tankname).ToList();
            foreach (var item in obj)
            {
                var tankidobj = item.TankId.ToString();
                tanknamelistobj.Add(tankidobj);

            }
            string jsonobjtank = JsonConvert.SerializeObject(tanknamelistobj);
            return Json(jsonobjtank, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductByWareHouseId(int warehouseid)
        {

            var listproduct = new List<ProductList>();
            var objwarehouse = warehouseService.GetAllWareHouse().Where(x => x.Id == warehouseid).ToList();
            foreach (var it in objwarehouse)
            {
                var warehousecode = it.WareHouseCode;
                var objtank = tankService.GetAllTank().Where(x => x.WareHouseCode == warehousecode).ToList();
                foreach (var item in objtank)
                {
                    var productlist = new ProductList();
                    if (item.MProduct != null)
                    {
                        productlist.productname = item.MProduct.ProductName;
                        productlist.productid = item.MProduct.ProductId;
                    }

                    listproduct.Add(productlist);
                }
            }
            return Json(listproduct, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByTankId(int tankid)
        {
            try
            {
                var listproduct = new List<ProductList>();
                var objtank = tankService.GetAllTank().Where(x => x.TankId == tankid).ToList();
                foreach (var item in objtank)
                {
                    var productlist = new ProductList();
                    productlist.productname = item.MProduct.ProductName;
                    productlist.productid = item.MProduct.ProductId;
                    listproduct.Add(productlist);
                }
                return Json(listproduct, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }


        public JsonResult GetTankByTankGrp(int tankgrpid)
        {
            var listtank = new List<TankList>();
            var objtankgrp = tankgrpService.GetAllTankGrp().Where(x => x.TankGrpId == tankgrpid).ToList();
            foreach (var item in objtankgrp)
            {

                var objtank = item.MTankGrpTanks.ToList();
                foreach (var it in objtank)
                {
                    var tankid = it.TankId;
                    var objtankbyid = tankService.GetAllTank().Where(x => x.TankId == tankid).ToList();

                    foreach (var items in objtankbyid)
                    {
                        var tank = new TankList();
                        tank.tankid = items.TankId;
                        tank.tankname = items.TankName;
                        listtank.Add(tank);
                    }

                }

            }


            return Json(listtank, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTankGrpbyWareHouseId(int warehouseid)
        {
            try
            {
                List<string> tanknamelist = new List<string>();

                var obj = warehouseService.FindWareHouseById(warehouseid);
                var warehousecode = Convert.ToByte(obj.WareHouseCode);
                var objtankgrp = tankgrpService.GetAllTankGrp().Where(x => x.WareHouseCode == warehousecode).ToList();
                var objtank = tankService.GetAllTankByWareHouseCode(warehousecode).ToList();
                var listtankgrp = new List<TankGrp>();
                foreach (var item in objtankgrp)
                {
                    var tankgrp = new TankGrp();
                    tankgrp.tankgrpname = item.TanktGrpName;
                    tankgrp.tankgrpid = item.TankGrpId;
                    listtankgrp.Add(tankgrp);
                }

                return Json(listtankgrp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json("", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Index(int? page, WareHouseModel warehousemodel, string warehousename, string cardSerial, string search)
        {
            log.Info("Index WareHouse");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTWAREHOUSE);
            if (checkPermission)
            {

                try
                {
                    warehousemodel.ListEnumableWareHouse = warehouseService.GetAllWareHouseOrderByName();

                    int pageNumber = (page ?? 1);
                    if (search == null || search == "")
                    {
                        warehouseModel.ListWareHouse = warehouseService.GetAllWareHouse().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        warehouseModel.ListWareHouse = warehouseService.GetAllWareHouse().Where(x => x.WareHouseName.Contains(search)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return View(warehouseModel);
            }
            else
            {
                log.Info("Stop controller WareHouse index  ");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTWAREHOUSE);
            if (checkPermission)
            {
                warehouseModel.WareHouse = warehouseService.FindWareHouseById(id);
                return View(warehouseModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(WareHouseModel warehouseModel)
        {
            log.Info("Start controller WateHouse httpPost edit");
            try
            {

                var warehouse = warehouseService.FindWareHouseById(warehouseModel.WareHouse.Id);
                if (null != warehouse)
                {
                    warehouse.UpdateUser = HttpContext.User.Identity.Name;
                    warehouse.WareHouseAddress = warehouseModel.WareHouse.WareHouseAddress;
                    warehouse.WareHouseCode = warehouseModel.WareHouse.WareHouseCode;
                    warehouse.WareHouseName = warehouseModel.WareHouse.WareHouseName;
                    warehouse.WareHousePNumber = warehouseModel.WareHouse.WareHousePNumber;

                    var rs = warehouseService.UpdateWareHouse(warehouse);
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
            log.Info("Finish controller WateHouse httpPost edit");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTWAREHOUSE);
            if (checkPermission)
            {
                return View();
            }

            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }

        }


        public ActionResult GetJsonAllWareHouse()
        {
            log.Info("Start controller WareHouse GetJsonAllWareHouse");
            try
            {

                warehouseModel.ListEnumableWareHouse = warehouseService.GetAllWareHouse();
                var lstObject = new List<Datum>();

                foreach (var it in warehouseModel.ListEnumableWareHouse)
                {
                    var datum = new Datum();
                    datum.name = it.WareHouseCode.ToString();
                    datum.type = it.WareHouseName;
                    lstObject.Add(datum);
                }
                log.Info("Start controller WareHouse GetJsonAllWareHouse");
                return Json(lstObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);

            }

            return null;
        }


        [HttpPost]
        public ActionResult Create(MWareHouse warehouse)
        {
            log.Info("Start conlller WareHouse HttpPost Create ");
            try
            {

                warehouse.InsertUser = HttpContext.User.Identity.Name;
                warehouse.UpdateUser = HttpContext.User.Identity.Name;

                var rs = warehouseService.CreateWareHouse(warehouse);

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
            log.Info("Finish conlller WareHouse HttpPost Create ");
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            log.Info("Start controller WareHouse Delete");
            try
            {
                var rs = warehouseService.DeleteWareHouse(id);
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
            log.Info("Finish controller WareHouse Delete");
            return RedirectToAction("Index");
        }
    }
}