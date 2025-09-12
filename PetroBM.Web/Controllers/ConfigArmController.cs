
using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using PetroBM.Web.Attribute;
using System.Globalization;
using log4net;
using System;

namespace PetroBM.Web.Controllers
{

    public class ConfigArmController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ConfigArmController));
        private readonly IConfigArmService configarmService;
        private readonly ITankService tankService;
        private readonly IProductService productService;
        public ConfigArmModel configarmModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly IWareHouseService warehouseService;
        private readonly BaseService baseService;

        public ConfigArmController(IWareHouseService warehouseService, IConfigArmService configarmService, ConfigArmModel configarmModel, IProductService productService,
            IConfigurationService configurationService, ITankService tankService,
            IUserService userService, BaseService baseService)
        {
            this.configarmService = configarmService;
            this.configarmModel = configarmModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.tankService = tankService;
            this.productService = productService;
            this.warehouseService = warehouseService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page, ConfigArmModel configarmModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCONFIGARM);
            if (checkPermission)
            {
                try
                {
                    configarmModel.ListWareHouse = warehouseService.GetAllWareHouse();
                    log.Info("Index ConfigArm");
                    int pageNumber = (page ?? 1);

                    // Khởi tạo dữ liệu mặc định ===============================
                    if (configarmModel.WareHouseCode == 0)
                    {
                        foreach (var item in configarmModel.ListWareHouse)
                        {
                            configarmModel.WareHouseCode = (byte)item.WareHouseCode;
                            break;
                        }
                    }

                    if (configarmModel.ActiveStatus == null)
                    {
                        configarmModel.ActiveStatus = 2;
                    }
                    //========================================================

                    if (configarmModel.ActiveStatus == 2)
                    {
                        if (configarmModel.ArmName == "" || configarmModel.ArmName == null)
                            configarmModel.ListConfigArm = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == configarmModel.WareHouseCode).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        else
                            configarmModel.ListConfigArm = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == configarmModel.WareHouseCode && x.ArmName.Contains(configarmModel.ArmName)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        if (configarmModel.ArmName == "" || configarmModel.ArmName == null)
                            configarmModel.ListConfigArm = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == configarmModel.WareHouseCode && x.ActiveStatus == configarmModel.ActiveStatus).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        else
                            configarmModel.ListConfigArm = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == configarmModel.WareHouseCode && x.ActiveStatus == configarmModel.ActiveStatus && x.ArmName.Contains(configarmModel.ArmName)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return View(configarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public JsonResult GetSearchingData(int activestatus, string armname)
        {
            try
            {
                log.Info("Get SearchingData");
                var obj = configarmService.GetAllConfigArmByActiveStatusAndArmName(activestatus, armname).ToList();
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCONFIGARM);
            if (checkPermission)
            {
                configarmModel.ConfigArm = configarmService.FindConfigArmById(id);
                configarmModel.ListProduct = productService.GetAllProduct();
                configarmModel.ListCheckConfigArm = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == configarmModel.ConfigArm.WareHouseCode).ToList();

                configarmModel.ListTankTemps = tankService.GetAllTank().Where(x => x.WareHouseCode == configarmModel.ConfigArm.WareHouseCode).Select(x => new TankTempModel
                {
                    ProductId = (int)x.ProductId,
                    TankId = x.TankId,
                    TankName = x.TankName,
                    WareHouseCode = x.WareHouseCode
                });
                configarmModel.WareHouseName = warehouseService.GetAllWareHouse().Where(x => x.WareHouseCode == configarmModel.ConfigArm.WareHouseCode).First().WareHouseName;
                return View(configarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(ConfigArmModel ConfigArmModel)
        {
            try
            {
                log.Info("Edit ConfigArm");
                var configarm = configarmService.FindConfigArmById(ConfigArmModel.ConfigArm.ConfigArmID);
                if (null != configarm)
                {
                    configarm.UpdateUser = HttpContext.User.Identity.Name;
                    configarm.WareHouseCode = ConfigArmModel.ConfigArm.WareHouseCode;
                    configarm.ArmName = ConfigArmModel.ConfigArm.ArmName;
                    configarm.ArmNo = ConfigArmModel.ConfigArm.ArmNo;
                    configarm.ProductCode_1 = ConfigArmModel.ConfigArm.ProductCode_1;
                    configarm.ProductCode_2 = ConfigArmModel.ConfigArm.ProductCode_2;
                    configarm.ProductCode_3 = ConfigArmModel.ConfigArm.ProductCode_3;
                    configarm.TankID = ConfigArmModel.ConfigArm.TankID;
                    configarm.ActiveStatus = ConfigArmModel.ConfigArm.ActiveStatus;
                    configarm.TypeExport = ConfigArmModel.ConfigArm.TypeExport;

                    var rs = configarmService.UpdateConfigArm(configarm);

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
            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCONFIGARM);
            if (checkPermission)
            {

                configarmModel.ListTankTemps = tankService.GetAllTank().Select(x => new TankTempModel
                {
                    ProductId = (int)x.ProductId,
                    TankId = x.TankId,
                    TankName = x.TankName,
                    WareHouseCode = x.WareHouseCode
                });

                configarmModel.ListCheckConfigArm = configarmService.GetAllConfigArm();
                configarmModel.ListProduct = productService.GetAllProduct();
                configarmModel.ListWareHouse = warehouseService.GetAllWareHouse();
                return View(configarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(ConfigArmModel ConfigArmModel)
        {
            try
            {
                log.Info("Create ConfigArm");
                // var wareHouse = ConfigArmModel.ConfigArm.WareHouseCode;
                // byte armNo = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == wareHouse).Max(x => x.ArmNo);
                // ConfigArmModel.ConfigArm.ArmNo = (byte)(armNo + 1);
                ConfigArmModel.ConfigArm.InsertUser = HttpContext.User.Identity.Name;
                ConfigArmModel.ConfigArm.UpdateUser = HttpContext.User.Identity.Name;
                var rs = configarmService.CreateConfigArm(ConfigArmModel.ConfigArm);


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
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                log.Info("Delete ConfigArm");
                var rs = configarmService.DeleteConfigArm(id);
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

            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Import(ConfigArmModel ConfigArm)
        {
            try
            {
                log.Info("Import ConfigArm");
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                var rs = configarmService.Import(ConfigArm.ImportFile, HttpContext.User.Identity.Name);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return RedirectToAction("Index");
        }
        [ChildActionOnly]
        public ActionResult GetConfigArmNumber()
        {
            var count = configarmService.GetAllConfigArm().Count();

            return Content(count.ToString());
        }

        [ChildActionOnly]
        public ActionResult GetConfigArmCount(byte wareHouseCode)
        {
            var count = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == wareHouseCode).Count();

            return Content(count.ToString());
        }


        public JsonResult GetTank(byte wareHouse)
        {
            var lstTank = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse).ToList();

            if (lstTank.Count != 0)
                return Json(lstTank, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);

        }


    }
}