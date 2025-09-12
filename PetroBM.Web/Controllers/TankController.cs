using PagedList;
using PetroBM.Web.Attribute;
using PetroBM.Common.Util;
using PetroBM.Domain.Entities;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Web.SessionState;
using System.Web;

namespace PetroBM.Web.Controllers
{
    public class TankController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(TankController));
        private readonly ITankService tankService;
        private readonly IProductService productService;
        private readonly IWareHouseService wareHouseService;
        private readonly IAlarmService alarmService;
        private readonly TankModel tankModel;
        private readonly BaseService baseService;

        public TankController(ITankService tankService, TankModel tankModel, IWareHouseService wareHouseService,
            IProductService productService, IAlarmService alarmService, BaseService baseService)
        {
            this.tankService = tankService;
            this.productService = productService;
            this.alarmService = alarmService;
            this.tankModel = tankModel;
            this.wareHouseService = wareHouseService;
            this.baseService = baseService;
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        // GET: Tank
        public ActionResult Index(int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANK);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                tankModel.ListTank = tankService.GetAllTank().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                tankModel.ListWareHouse = wareHouseService.GetAllWareHouse();
                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [ChildActionOnly]
        public ActionResult GetTankNumber()
        {
            var count = tankService.GetAllTank().Count();

            return Content(count.ToString());
        }


        [ChildActionOnly]
        public ActionResult GetTankCount(byte wareHouseCode)
        {
            var count = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouseCode).Count();

            return Content(count.ToString());
        }


        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANK);
            if (checkPermission)
            {
                tankModel.ListProduct = productService.GetAllProduct();
                tankModel.ListWareHouse = wareHouseService.GetAllWareHouse();
                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Create(TankModel tankModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANK);
            if (checkPermission)
            {
                tankModel.Tank.InsertUser = HttpContext.User.Identity.Name;
                tankModel.Tank.ProductId = tankModel.ProductId;

                var rs = tankService.CreateTank(tankModel.Tank);

                if (rs == true)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
                }

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Edit(byte warehousecode, int tankId)
        {
            //tankService.UpdateVolumeMax(id);
            //tankModel.Tank = tankService.FindTankById(id);

            ////if (tankModel.Tank.MProduct != null)
            ////{
            ////    tankModel.ProductId = tankModel.Tank.MProduct.ProductId;
            ////}

            //tankModel.ListProduct = productService.GetAllProduct();
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANK);
            if (checkPermission)
            {
                tankModel.Tank = tankService.FindTankById(tankId, warehousecode);
                tankModel.ListWareHouse = wareHouseService.GetAllWareHouse();
                tankModel.ListProduct = productService.GetAllProduct();
                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Edit(TankModel tankModel)
        {
            tankModel.Tank.UpdateUser = HttpContext.User.Identity.Name;
            //  tankModel.Tank.ProductId = tankModel.ProductId;
            tankModel.Tank.WareHouseCode = Convert.ToByte(tankModel.WareHouseCode);

            var rs = tankService.UpdateTank(tankModel.Tank);

            if (rs == true)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }

            return RedirectToAction("Index");
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        //public ActionResult Delete(int id)
        //{
        //    var rs = tankService.DeleteTank(id);

        //    if (rs == true)
        //    {
        //        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
        //    }
        //    else
        //    {
        //        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
        //    }

        //    return RedirectToAction("Index");
        //}

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Delete(int tankId, byte wareHouseCode)
        {
            var rs = tankService.DeleteTank(tankId, wareHouseCode);

            if (rs == true)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
            }

            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        public ActionResult TankLive(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_TANK);
            if (checkPermission)
            {

                //tankModel.Tank = tankService.FindTankById(id);
                //tankModel.TankLive = tankService.GetNewestTankLive(id);

                //ThangNK modify

                byte wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                tankModel.Tank = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse && x.TankId == id).FirstOrDefault();
                tankModel.TankLive = tankService.GetNewestTankLive(id, wareHouse);

                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult TankImage(int id, float ratio)
        {

            //tankModel.Tank = tankService.FindTankById(id);
            //tankModel.TankLive = tankService.GetNewestTankLive(id);
            //tankModel.RatioImage = ratio;
            //tankModel.Alarm = alarmService.GetNewestAlarm(id);

            //ThangNK Modify
            byte wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            tankModel.Tank = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse && x.TankId == id).FirstOrDefault();
            tankModel.TankLive = tankService.GetNewestTankLive(id, wareHouse);
            tankModel.RatioImage = ratio;
            tankModel.Alarm = alarmService.GetNewestAlarm(id, wareHouse);
            return PartialView(tankModel);
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        public ActionResult TankManual(int id, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_TANK);
            if (checkPermission)
            {

                int pageNumber = (page ?? 1);
                //tankModel.TankId = id;
                //tankModel.Tank = tankService.FindTankById(id);
                //tankModel.ListTankManual = tankService.GetAllTankManual(id).ToPagedList(pageNumber, Constants.PAGE_SIZE);

                byte wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                tankModel.TankId = id;
                tankModel.Tank = tankService.GetAllTank().Where(x => x.TankId == id && x.WareHouseCode == wareHouse).FirstOrDefault();
                tankModel.ListTankManual = tankService.GetAllTankManual().Where(x => x.WareHouseCode == wareHouse && x.TankId == id).ToPagedList(pageNumber, Constants.PAGE_SIZE);

                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        public ActionResult CreateTankManual(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_TANK);
            if (checkPermission)
            {
                tankModel.TankManual.InsertDate = DateTime.Now;
                byte wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                tankModel.Tank = tankService.FindTankById(id, wareHouse);
                tankModel.TankManual.MTank = tankService.FindTankById(id, wareHouse);
                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        [HttpPost]
        public ActionResult DoCreateTankManual(TankModel tankModel)
        {
            if (!String.IsNullOrEmpty(tankModel.DateCreateTankManual))
            {
                tankModel.TankManual.InsertDate =
                    DateTime.ParseExact(tankModel.DateCreateTankManual, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            tankModel.TankManual.TankId = tankModel.TankManual.MTank.TankId;
            tankModel.TankManual.InsertUser = HttpContext.User.Identity.Name;
            tankModel.TankManual.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());

            var rs = tankService.CreateTankManual(tankModel.TankManual);

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            }

            return RedirectToAction("TankManual", new { id = tankModel.TankManual.TankId });
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        public ActionResult EditTankManual(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_TANK);
            if (checkPermission)
            {
                tankModel.TankManual = tankService.GetTankManual(id);
                tankModel.DateCreateTankManual = tankModel.TankManual.InsertDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        [HttpPost]
        public ActionResult DoEditTankManual(TankModel tankModel)
        {
            var tankManual = tankService.GetTankManual(tankModel.TankManual.Id);
            if (tankManual != null)
            {
                if (!String.IsNullOrEmpty(tankModel.DateCreateTankManual))
                {
                    tankManual.InsertDate =
                        DateTime.ParseExact(tankModel.DateCreateTankManual, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                tankManual.WaterLevel = tankModel.TankManual.WaterLevel;
                tankManual.TotalLevel = tankModel.TankManual.TotalLevel;
                tankManual.AvgTemperature = tankModel.TankManual.AvgTemperature;
                tankManual.UpdateUser = HttpContext.User.Identity.Name;
            }

            var rs = tankService.UpdateTankManual(tankManual);

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }

            return RedirectToAction("TankManual", new { id = tankManual.TankId });
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        public ActionResult DoDeleteTankManual(int id)
        {
            var tankManual = tankService.GetTankManual(id);

            if (tankManual != null)
            {
                tankManual.UpdateUser = HttpContext.User.Identity.Name;
                var rs = tankService.DeleteTankManual(tankManual);

                if (rs == true)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
                }
            }

            return RedirectToAction("TankManual", new { id = tankManual.TankId });
        }

        //public ActionResult CompareTankLive(int id, DateTime date)
        //{
        //    tankModel.TankManual = tankService.GetTankManual(id, date);

        //    return View(tankModel);
        //}

        [HasPermission(Constants.PERMISSION_TANK)]
        public ActionResult Density(int id, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_TANK);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                byte wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                tankModel.TankDensity.TankId = id;
                tankModel.TankDensity.WareHouseCode = wareHouse;
                tankModel.Tank = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse && x.TankId == id).FirstOrDefault();
                // tankModel.ListTankDensity = tankService.GetAllTankDensity(id).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                tankModel.ListTankDensity = tankService.GetAllTankDensity(id, wareHouse).ToPagedList(pageNumber, Constants.PAGE_SIZE);

                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_TANK)]
        [HttpPost]
        public ActionResult DoCreateDensity(TankModel tankModel)
        {
            byte wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            tankModel.TankDensity.InsertUser = HttpContext.User.Identity.Name;
            tankModel.TankDensity.InsertDate = DateTime.Now;
            tankModel.TankDensity.WareHouseCode = wareHouse;

            var rs = tankService.CreateTankDensity(tankModel.TankDensity);

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            }

            return RedirectToAction("Density", new { id = tankModel.TankDensity.TankId });
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Barem(int tankId, byte wareHouseCode)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANK);
            if (checkPermission)
            {


                tankModel.TankId = tankId;
                tankModel.WareHouseCode = wareHouseCode;
                tankModel.Tank = tankService.FindTankById(tankId, wareHouseCode);
                tankModel.ListBarem = tankService.GetBarem(tankId, wareHouseCode);
                return View(tankModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Barem(int id, IEnumerable<float> highs)
        {
            highs = highs.Where(h => h != 0);

            var rs = tankService.DeleteBarem(id, highs, HttpContext.User.Identity.Name);

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
            }

            return RedirectToAction("Barem", new { id = id });
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult CreateBarem(byte wareHouseCode, int tankId, float high)
        {
            string rs = Constants.MESSAGE_ALERT_INSERT_FAILED;

            var barem = new MBarem();
            barem.TankId = tankId;
            barem.WareHouseCode = wareHouseCode;
            barem.High = high;
            barem.InsertUser = HttpContext.User.Identity.Name;
            barem.UpdateUser = HttpContext.User.Identity.Name;

            if (tankService.CreateBarem(barem))
            {
                rs = String.Empty;
            }

            return Content(rs);
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult EditHighBarem(byte wareHouseCode, int tankId, float oldHigh, float newHigh)
        {
            string rs = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            var barem = tankService.GetBaremByHigh(wareHouseCode, tankId, newHigh);

            if (barem == null)
            {
                barem = tankService.GetBaremByHigh(wareHouseCode, tankId, oldHigh);
                if (barem != null)
                {
                    barem.High = newHigh;
                    barem.UpdateUser = HttpContext.User.Identity.Name;

                    if (tankService.UpdateBarem(barem))
                    {
                        rs = String.Empty;
                    }
                }
            }
            else
            {
                rs = Constants.MESSAGE_ALERT_BAREM_EXISTED;
            }

            return Content(rs);
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult EditVolumeBarem(byte wareHouseCode, int tankId, float high, float volume)
        {
            string rs = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            var barem = tankService.GetBaremByHigh(wareHouseCode, tankId, high);

            if (barem != null)
            {
                barem.Volume = volume;
                barem.UpdateUser = HttpContext.User.Identity.Name;

                if (tankService.UpdateBarem(barem))
                {
                    rs = String.Empty;
                }
            }

            return Content(rs);
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult ImportBarem(TankModel tankModel)
        {
            TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

            var rs = tankService.ImportBarem((byte)tankModel.WareHouseCode, (int)tankModel.TankId, tankModel.BaremFile, HttpContext.User.Identity.Name);

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }

            return RedirectToAction("Barem", new { tankId = tankModel.TankId, wareHouseCode = tankModel.WareHouseCode });
        }
    }
}