using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using PetroBM.Web.Attribute;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using log4net;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Data;
using System.Drawing.Printing;

namespace PetroBM.Web.Controllers
{

    public class VehicleController : BaseController
    {

        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(VehicleController));

        private readonly IVehicleService vehicleService;
        public VehicleModel VehicleModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly IDriverService driverService;
        private readonly BaseService baseService;
        private readonly CardService cardService;

        public VehicleController(IVehicleService vehicleService, VehicleModel VehicleModel, IConfigurationService configurationService, IUserService userService, IDriverService driverService, BaseService baseService, CardService CardService)
        {
            this.vehicleService = vehicleService;
            this.VehicleModel = VehicleModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.driverService = driverService;
            this.baseService = baseService;
            this.cardService = CardService;
        }
        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Index(int? page, VehicleModel VehicleModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTVERHICLE);
            if (checkPermission)
            {
                try
                {

                    log.Info("Index Vehicle");
                    int pageNumber = (page ?? 1);

                    VehicleModel.Card = cardService.GetAllCard();
                    DataTable dt = vehicleService.FindVehicle(VehicleModel.VehicleNumber, VehicleModel.DriverDefault, VehicleModel.CardSerial.ToString());
                    List<MVehicle> rowsList = vehicleService.ConvertDataTableToMVehicleList(dt);
                    VehicleModel.ListVehiclePageList = rowsList.ToPagedList(pageNumber, Constants.PAGE_SIZE);

                    //if (VehicleModel.DriverDefault == null && VehicleModel.VehicleNumber == null && VehicleModel.CardSerial == null)
                    //    VehicleModel.ListVehiclePageList = vehicleService.GetAllVehicle().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    //else
                    //{
                    //    var cards = cardService.GetAllCard();
                    //    var vehicles = vehicleService.GetAllVehicle();
                    //    if (VehicleModel.DriverDefault != null)
                    //    {
                    //        if (VehicleModel.VehicleNumber != null)
                    //            VehicleModel.ListVehiclePageList = vehicles.Where(x => x.VehicleNumber.Contains(VehicleModel.VehicleNumber) 
                    //                && x.Driverdefault.Contains(VehicleModel.DriverDefault)) 
                    //                .ToPagedList(pageNumber, Constants.PAGE_SIZE); 
                    //        else
                    //            VehicleModel.ListVehiclePageList = vehicleService.GetAllVehicle().Where(x => x.Driverdefault.Contains(VehicleModel.DriverDefault)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    //    }
                    //    else
                    //    {
                    //        if (VehicleModel.VehicleNumber != null)
                    //            VehicleModel.ListVehiclePageList = vehicleService.GetAllVehicle().Where(x => x.VehicleNumber.Contains(VehicleModel.VehicleNumber)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return View(VehicleModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [ChildActionOnly]
        public ActionResult GetVehicleNumber()
        {
            var count = vehicleService.GetAllVehicle().Count();

            return Content(count.ToString());
        }
        // [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTVERHICLE);
            if (checkPermission)
            {
                VehicleModel.Vehicle = vehicleService.FindVehicleById(id);
                VehicleModel.Card = cardService.GetCardByIdVehicle(id);
                #region Lái xe
                var lstD = new List<Datum>();
                var lst = driverService.GetAllDriver();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.Name;
                    datum.type = it.IdentificationNumber;
                    lstD.Add(datum);
                }
                VehicleModel.LstDriver = lstD;
                #endregion

                DateTime date = DateTime.Now;
                if (VehicleModel.Vehicle.ExpireDate != null)
                {
                    date = (DateTime)VehicleModel.Vehicle.ExpireDate;
                    VehicleModel.ExpireDate = date.ToString(Constants.DATE_FORMAT_NEW);
                }
                if (VehicleModel.Vehicle.AccreditationExpire != null)
                {
                    date = (DateTime)VehicleModel.Vehicle.AccreditationExpire;
                    VehicleModel.AccreditationExpire = date.ToString(Constants.DATE_FORMAT_NEW);
                }
                if (VehicleModel.Vehicle.FirePreventExpire != null)
                {
                    date = (DateTime)VehicleModel.Vehicle.FirePreventExpire;
                    VehicleModel.FirePreventExpire = date.ToString(Constants.DATE_FORMAT_NEW);
                }

                return View(VehicleModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Edit(VehicleModel VehicleModel)
        {
            try
            {
                log.Info("Edit Vehicle");
                var vehicle = vehicleService.FindVehicleById(VehicleModel.Vehicle.ID);
                if (null != vehicle)
                {
                    vehicle.UpdateUser = HttpContext.User.Identity.Name;
                    vehicle.AccreditationExpire = VehicleModel.Vehicle.AccreditationExpire;
                    vehicle.AccreditationNumber = VehicleModel.Vehicle.AccreditationNumber;
                    vehicle.Driverdefault = VehicleModel.Vehicle.Driverdefault;
                    vehicle.RegisterNumber = VehicleModel.Vehicle.RegisterNumber;
                    vehicle.VehicleNumber = VehicleModel.Vehicle.VehicleNumber;
                    vehicle.CommandType = VehicleModel.Vehicle.CommandType;
                    if(!string.IsNullOrEmpty(VehicleModel.ExpireDate))
                        vehicle.ExpireDate = DateTime.ParseExact(VehicleModel.ExpireDate, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(VehicleModel.AccreditationExpire))
                        vehicle.AccreditationExpire = DateTime.ParseExact(VehicleModel.AccreditationExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                    vehicle.FirePreventLicense = VehicleModel.Vehicle.FirePreventLicense;
                    if (!string.IsNullOrEmpty(VehicleModel.FirePreventExpire))
                        vehicle.FirePreventExpire = DateTime.ParseExact(VehicleModel.FirePreventExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                    vehicle.CardID = VehicleModel.Vehicle.CardID;
                    vehicle.Volume1 = VehicleModel.Vehicle.Volume1;
                    vehicle.Volume2 = VehicleModel.Vehicle.Volume2;
                    vehicle.Volume3 = VehicleModel.Vehicle.Volume3;
                    vehicle.Volume4 = VehicleModel.Vehicle.Volume4;
                    vehicle.Volume5 = VehicleModel.Vehicle.Volume5;
                    vehicle.Volume6 = VehicleModel.Vehicle.Volume6;
                    vehicle.Volume7 = VehicleModel.Vehicle.Volume7;
                    vehicle.Volume8 = VehicleModel.Vehicle.Volume8;
                    vehicle.Volume9 = VehicleModel.Vehicle.Volume9;

                    var rs = vehicleService.UpdateVehicle(vehicle);

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


        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTVERHICLE);
            if (checkPermission)
            {
                VehicleModel.ExpireDate = DateTime.Now.AddMinutes(0).ToString(Constants.DATE_FORMAT_NEW);
                VehicleModel.AccreditationExpire = DateTime.Now.AddMinutes(0).ToString(Constants.DATE_FORMAT_NEW);
                VehicleModel.FirePreventExpire = DateTime.Now.AddMinutes(0).ToString(Constants.DATE_FORMAT_NEW);
                VehicleModel.Card = cardService.GetAllCardNotUse();
                #region Lái xe
                var lstD = new List<Datum>();
                var lst = driverService.GetAllDriver();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.Name;
                    datum.type = it.IdentificationNumber;
                    lstD.Add(datum);
                }
                VehicleModel.LstDriver = lstD;
                #endregion

                return View(VehicleModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Create(VehicleModel VehicleModel)
        {
            try
            {
                log.Info("Create Vehicle");
                VehicleModel.Vehicle.InsertUser = HttpContext.User.Identity.Name;
                VehicleModel.Vehicle.UpdateUser = HttpContext.User.Identity.Name;
                VehicleModel.Vehicle.ExpireDate = DateTime.ParseExact(VehicleModel.ExpireDate, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                VehicleModel.Vehicle.AccreditationExpire = DateTime.ParseExact(VehicleModel.AccreditationExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                VehicleModel.Vehicle.FirePreventExpire = DateTime.ParseExact(VehicleModel.FirePreventExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);

                var rs = vehicleService.CreateVehicle(VehicleModel.Vehicle);

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
        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Delete(int id)
        {
            try
            {
                log.Info("Delete Vehicle");
                var rs = vehicleService.DeleteVehicle(id);
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

        [HttpPost]
        public ActionResult Detail(string identificationNumber)
        {
            try
            {
                log.Info("Detail");
                var lst = new List<VolumeDetailModel>();
                var obj = vehicleService.GetAllVehicle().Where(x => x.VehicleNumber == identificationNumber && x.DeleteFlg == false).FirstOrDefault();
                if (obj == null)
                {
                    lst.Add(new VolumeDetailModel { Volume = "-1", Amount = 0, ProductName = "", Quality = 0 });
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                if (obj.Volume1 != null && obj.Volume1 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "1", Amount = obj.Volume1, ProductName = "", Quality = 0 });
                }
                if (obj.Volume2 != null && obj.Volume2 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "2", Amount = obj.Volume2, ProductName = "", Quality = 0 });
                }
                if (obj.Volume3 != null && obj.Volume3 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "3", Amount = obj.Volume3, ProductName = "", Quality = 0 });
                }
                if (obj.Volume4 != null && obj.Volume4 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "4", Amount = obj.Volume4, ProductName = "", Quality = 0 });
                }

                if (obj.Volume5 != null && obj.Volume5 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "5", Amount = obj.Volume5, ProductName = "", Quality = 0 });
                }
                if (obj.Volume6 != null && obj.Volume6 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "6", Amount = obj.Volume6, ProductName = "", Quality = 0 });
                }
                if (obj.Volume7 != null && obj.Volume7 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "7", Amount = obj.Volume7, ProductName = "", Quality = 0 });
                }
                if (obj.Volume8 != null && obj.Volume8 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "8", Amount = obj.Volume8, ProductName = "", Quality = 0 });
                }
                if (obj.Volume9 != null && obj.Volume9 != 0)
                {
                    lst.Add(new VolumeDetailModel { Volume = "9", Amount = obj.Volume9, ProductName = "", Quality = 0 });
                }
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }
        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Import(VehicleModel vehicle)
        {
            try
            {
                log.Info("Import Vehicle");
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                var rs = vehicleService.Import(vehicle.ImportFile, HttpContext.User.Identity.Name);

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


        public ActionResult ViewHTML()
        {
            return View();
        }

        public ActionResult SealHTML()
        {
            return View();
        }

    }
}
