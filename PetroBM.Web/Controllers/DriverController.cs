/*
 * Authori: Nguyễn Kim thắng :
 * CV tồn đọng : Xử lí phân trang chưa hợp lí 
 * 1. Khi tìm kiếm xong có nhiều trang==> chọn trang ==> Trả kết quả chưa đúng
 * 
*/
using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using PetroBM.Web.Attribute;
using System.Globalization;
using log4net;
using System;

namespace PetroBM.Web.Controllers
{

    public class InfomationDriver
    {
        public bool BoolDriversLicenseExpire { get; set; }
        public DateTime DriversLicenseExpire { get; set; }
        public bool BoolSavetyCertificatesExpire { get; set; }
        public DateTime SavetyCertificatesExpire { get; set; }
    }


    public class DriverController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DriverController));
        private readonly IDriverService driverService;
        public DriverModel driverModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public DriverController(IDriverService driverService, DriverModel driverModel, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.driverService = driverService;
            this.driverModel = driverModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page, DriverModel driverModel)
        {
            log.Info("Start driver index ");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTDRIVER);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                try
                {
                    if ((driverModel.Name == null && driverModel.IdentificationNumber == null) || (driverModel.Name == "" && driverModel.IdentificationNumber == ""))
                    {
                        driverModel.ListDriver = driverService.GetAllDriver().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        if (driverModel.Name != null)
                        {
                            if (driverModel.IdentificationNumber != null)
                                driverModel.ListDriver = driverService.GetAllDriver().Where(x => x.Name.Contains(driverModel.Name) && x.IdentificationNumber.Contains(driverModel.IdentificationNumber)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                            else
                                driverModel.ListDriver = driverService.GetAllDriver().Where(x => x.Name.Contains(driverModel.Name)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        }
                        else
                            driverModel.ListDriver = driverService.GetAllDriver().Where(x => x.IdentificationNumber.Contains(driverModel.IdentificationNumber)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Info("Finish driver index");
                return View(driverModel);
            }
            else
            {
                log.Info("Stop driver index");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public JsonResult GetInformationCompareDriverName(string driverName)
        {
            if (driverName == "" || driverName == null)
                return Json("", JsonRequestBehavior.AllowGet);
            var obj = driverService.GetAllDriver().Where(x => x.Name == driverName).FirstOrDefault();
            if (obj == null)
                return Json("", JsonRequestBehavior.AllowGet);
            DateTime dt = DateTime.Now.Date;
            var objIn = new InfomationDriver();
            if (dt > obj.DriversLicenseExpire)
            {
                objIn.BoolDriversLicenseExpire = true;
                objIn.DriversLicenseExpire = (DateTime)obj.DriversLicenseExpire;
            }
            if (dt > obj.SavetyCertificatesExpire)
            {
                objIn.BoolSavetyCertificatesExpire = true;
                objIn.SavetyCertificatesExpire = (DateTime)obj.SavetyCertificatesExpire;
            }


            return Json(objIn, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTDRIVER);
            if (checkPermission)
            {
                driverModel.Driver = driverService.FindDriverById(id);
                DateTime birthDate = DateTime.Now;
                if (driverModel.Driver.BirthDate != null)
                {
                    birthDate = (DateTime)driverModel.Driver.BirthDate;
                    driverModel.BirthDate = birthDate.ToString(Constants.DATE_FORMAT);
                }

                if (driverModel.Driver.SavetyCertificatesExpire != null)
                {
                    birthDate = (DateTime)driverModel.Driver.SavetyCertificatesExpire;
                    driverModel.SavetyCertificatesExpire = birthDate.ToString(Constants.DATE_FORMAT);
                }

                if (driverModel.Driver.DriversLicenseExpire != null)
                {
                    birthDate = (DateTime)driverModel.Driver.DriversLicenseExpire;
                    driverModel.DriversLicenseExpire = birthDate.ToString(Constants.DATE_FORMAT);
                }

                if (driverModel.Driver.LicenseDate != null)
                {
                    birthDate = (DateTime)driverModel.Driver.LicenseDate;
                    driverModel.LicenseDate = birthDate.ToString(Constants.DATE_FORMAT);
                }

                return View(driverModel);
            }
            else
            {
                log.Info("Stop driver index");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(DriverModel driverModel)
        {
            log.Info("Start driver edit ");
            try
            {
                var driver = driverService.FindDriverById(driverModel.Driver.ID);
                if (null != driver)
                {
                    driver.UpdateUser = HttpContext.User.Identity.Name;
                    if (driverModel.BirthDate != null && driverModel.BirthDate != "")
                        driver.BirthDate = DateTime.ParseExact(driverModel.BirthDate, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                    if (driverModel.LicenseDate != null && driverModel.LicenseDate != "")
                        driver.LicenseDate = DateTime.ParseExact(driverModel.LicenseDate, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                    driver.DriversLicense = driverModel.Driver.DriversLicense;
                    if (driverModel.DriversLicenseExpire != null && driverModel.DriversLicenseExpire != "")
                        driver.DriversLicenseExpire = DateTime.ParseExact(driverModel.DriversLicenseExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                    driver.IdentificationNumber = driverModel.Driver.IdentificationNumber;
                    driver.SavetyCertificates = driverModel.Driver.SavetyCertificates;
                    if (driverModel.SavetyCertificatesExpire != null && driverModel.SavetyCertificatesExpire != "")
                        driver.SavetyCertificatesExpire = DateTime.ParseExact(driverModel.SavetyCertificatesExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);


                    driver.Name = driverModel.Driver.Name;
                    driver.PhoneNumber = driverModel.Driver.PhoneNumber;
                    driver.FireTrainingLicense = driverModel.Driver.FireTrainingLicense;
                    driver.IssuedBy = driverModel.Driver.IssuedBy;

                    var rs = driverService.UpdateDriver(driver);

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
            log.Info("Finish driver edit ");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTDRIVER);
            if (checkPermission)
            {
                return View();
            }
            else
            {
                log.Info("Stop driver index");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(DriverModel driverModel)
        {
            try
            {
                log.Info("Start driver create ");
                driverModel.Driver.InsertUser = HttpContext.User.Identity.Name;
                driverModel.Driver.UpdateUser = HttpContext.User.Identity.Name;
                if (driverModel.BirthDate != null && driverModel.BirthDate != "")
                    driverModel.Driver.BirthDate = DateTime.ParseExact(driverModel.BirthDate, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                if (driverModel.DriversLicenseExpire != null && driverModel.DriversLicenseExpire != "")
                    driverModel.Driver.DriversLicenseExpire = DateTime.ParseExact(driverModel.DriversLicenseExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                if (driverModel.SavetyCertificatesExpire != null && driverModel.SavetyCertificatesExpire != "")
                    driverModel.Driver.SavetyCertificatesExpire = DateTime.ParseExact(driverModel.SavetyCertificatesExpire, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);
                if (driverModel.LicenseDate != null && driverModel.LicenseDate != "")
                    driverModel.Driver.LicenseDate = DateTime.ParseExact(driverModel.LicenseDate, Constants.DATE_FORMAT_NEW, CultureInfo.InvariantCulture);

                var rs = driverService.CreateDriver(driverModel.Driver);

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
            log.Info("Finish driver create ");
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            log.Info("Start driver delete");
            try
            {

                var rs = driverService.DeleteDriver(id);
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
            log.Info("Finish driver delete ");

            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Import(DriverModel driver)
        {
            log.Info("Start driver import ");
            try
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                var rs = driverService.Import(driver.ImportFile, HttpContext.User.Identity.Name);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish driver import ");
            return RedirectToAction("Index");
        }


    }
}