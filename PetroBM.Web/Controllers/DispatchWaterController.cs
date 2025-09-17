using log4net;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using PagedList;
using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Domain.Entities;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Windows.Threading;
using System.Data.SqlClient;

namespace PetroBM.Web.Controllers
{
    public class DispatchWaterHistDto
    {
        public int DispatchID { get; set; }
        public DateTime? InsertDate { get; set; }
        public string InsertUser { get; set; }
        public DateTime? SysD { get; set; }   // Date thay đổi
        public string SysU { get; set; }      // User thay đổi
        public int VersionNo { get; set; }    // Phiên bản
    }

    public class DispatchWaterController : Controller
    {
        private readonly IDispatchWaterService DispatchWaterService;

        private readonly ILocationService LocationService;
        private readonly IUserService UserService;
        private readonly IConfigurationService ConfigurationService;
        private readonly IProductService ProductService;
        private readonly ISealService SealService;
        private readonly IWareHouseService WareHouseService;
        private readonly ICustomerService CustomerService;
        private readonly IVehicleService VehicleService;
        private readonly ICardService CardService;
        private readonly IDriverService DriverService;
        private readonly IWareHouseService warehouseService;
        private readonly BaseService baseService;
        private readonly IDispatchService dispatchService;
        private readonly IDepartmentService departmentService;
        private readonly IShipService shipService;
        private readonly IImageService imageService;

        public DispatchWaterModel DispatchWaterModel;
        ILog log = log4net.LogManager.GetLogger(typeof(DispatchController));

        public DispatchWaterController(IImageService imageService, IShipService shipService, IDispatchWaterService DispatchWaterService, DispatchWaterModel DispatchWaterModel, IDispatchService dispatchService,
            IConfigurationService ConfigurationService, IUserService UserService, IProductService ProductService,
             ISealService SealService, IDepartmentService departmentService,
        IWareHouseService WareHouseService, ICustomerService CustomerService, IDriverService DriverService,
            IVehicleService VehicleService, ICardService CardService, IWareHouseService warehouseService, BaseService baseService, ILocationService LocationService)
        {
            this.imageService = imageService;
            this.DispatchWaterModel = DispatchWaterModel;
            this.DispatchWaterService = DispatchWaterService;
            this.DispatchWaterModel = DispatchWaterModel;
            this.ConfigurationService = ConfigurationService;
            this.UserService = UserService;
            this.ProductService = ProductService;
            this.VehicleService = VehicleService;
            this.WareHouseService = WareHouseService;
            this.CustomerService = CustomerService;
            this.CardService = CardService;
            this.DriverService = DriverService;
            this.warehouseService = warehouseService;
            this.SealService = SealService;
            this.baseService = baseService;
            this.dispatchService = dispatchService;
            this.departmentService = departmentService;
            this.shipService = shipService;
            this.LocationService = LocationService;

        }


        // GET: Dispatch
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult HistoryDetail(int dispatchId, int versionNo)
        {
            const string sql = @"
            SELECT
                DispatchID,
                CertificateNumber,
                TimeOrder,
                CertificateTime,
                TimeStart,
                TimeStop,
                VehicleNumber,
                DriverName1,
                DriverName2,
                ProductCode,
                Department,
                DstPickup1,
                DstPickup2,
                DstReceive,
                Note1,
                Note2,
                Remark,
                [Status],
                InsertDate,
                InsertUser,
                UpdateDate,
                UpdateUser,
                VersionNo,
                DeleteFlg,
                [From],
                [To],
                Paragraph1,
                Paragraph2,
                Paragraph3,
                Paragraph4,
                ProcessStatus,
                SysU,
                SysD,
                TransactionId
            FROM dbo.MDispatchWater_Hist
            WHERE DispatchID = @DispatchID AND VersionNo = @VersionNo;";

            using (var db = new PetroBMContext())
            {
                var p1 = new SqlParameter("@DispatchID", dispatchId);
                var p2 = new SqlParameter("@VersionNo", versionNo);

                var model = db.Database
                    .SqlQuery<DispatchWaterHistModel>(sql, p1, p2)
                    .FirstOrDefault();

                if (model == null)
                    return HttpNotFound("Không tìm thấy bản ghi lịch sử nước.");

                // Gợi ý: nếu bạn đặt view riêng cho Water:
                return View("DispatchWaterHistView", model);

            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult GetHistory(int dispatchId)
        {
            const string sql = @"
                    SELECT
                        DispatchID,
                        InsertDate,
                        InsertUser,
                        SysD,
                        SysU,
	                    VersionNo
                    FROM dbo.MDispatchWater_Hist WITH (NOLOCK)
                    WHERE DispatchID = @p0
                    ORDER BY VersionNo ASC, SysD ASC;";
            try
            {
                using (var db = new PetroBMContext())
                {
                    var rows = db.Database.SqlQuery<DispatchWaterHistDto>(sql, dispatchId).ToList();
                    return Json(new { ok = true, count = rows.Count, rows }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error($"GetHistory error for DispatchID={dispatchId}", ex);
                Response.StatusCode = 500;
                return Json(new { ok = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult RegisterDispatchWater()
        {
            log.Info("Start controller RegisterDispatch");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_MANUAL);
            if (checkPermission)
            {
                DispatchWaterModel.Dispatch.TimeOrder = DateTime.Now;
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
                DispatchWaterModel.ListDepartment = departmentService.GetAllDepartment().ToList();
                //dispatchModel.Command.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                //commandModel.TimeOrder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).ToString(Constants.DATE_FORMAT);
                DispatchWaterModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT);
                DispatchWaterModel.CertificateTime = DateTime.Now.ToString(Constants.DATE_FORMAT);
                DispatchWaterModel.Dispatch.CertificateNumber = DispatchWaterService.getCertificateNumber();
                //dispatchModel.Dispatch.ProductCode = ProductService.GetAllProduct().FirstOrDefault().ProductCode + " - " + ProductService.GetAllProduct().FirstOrDefault().ProductName;
                //dispatchModel.Dispatch.TimeStart = DateTime.Parse(dispatchModel.StartDate.ToString());
                //dispatchModel.Dispatch.TimeStop = DateTime.Parse(dispatchModel.EndDate.ToString());

                #region  Kho
                var lstWH = new List<Datum>();
                var lst = LocationService.GetAllLocation();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.LocationCode.Trim() + " - " + it.LocationName.Trim();
                    datum.type = it.LocationCode.ToString().Trim();
                    lstWH.Add(datum);
                }
                DispatchWaterModel.LstWareHouse = lstWH;
                #endregion

                #region  Hàng hoá
                var lstProduct = new List<Datum>();
                var lstPr = ProductService.GetAllProduct();
                foreach (var it in lstPr)
                {
                    var datum = new Datum();
                    datum.name = it.ProductCode + " - " + it.ProductName;
                    datum.type = it.ProductCode.ToString();
                    lstProduct.Add(datum);
                }
                DispatchWaterModel.LstProduct = lstProduct;
                #endregion

                #region Khách hàng
                var lstC = new List<Datum>();
                var lst1 = CustomerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                DispatchWaterModel.LstCustomer = lstC;
                #endregion

                #region Phương tiện
                var lstV = new List<Datum>();
                var lst2 = shipService.GetAllShip();
                foreach (var it in lst2)
                {
                    var datum = new Datum();
                    datum.name = it.ShipName;
                    datum.type = it.NumberControl;
                    lstV.Add(datum);
                }
                DispatchWaterModel.LstVehicle = lstV;
                #endregion

                #region Lái xe
                var lstD = new List<Datum>();
                var lst3 = DriverService.GetAllDriver();
                foreach (var it in lst3)
                {
                    var datum = new Datum();
                    datum.name = it.Name;
                    datum.type = it.IdentificationNumber;
                    lstD.Add(datum);
                }
                DispatchWaterModel.LstDriver = lstD;
                #endregion

                #region Phòng ban
                //var lstE = new List<Datum>();
                //var lst4 = departmentService.GetAllDepartment();
                //foreach (var it in lst4)
                //{
                //    var datum = new Datum();
                //    datum.name = it.Name;
                //    datum.type = it.Code;
                //    lstE.Add(datum);
                //}
                //dispatchModel.LstDepartment = lstE;
                #endregion

                log.Info("Finish controller RegisterDispatch");
                return View(DispatchWaterModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }


        [HttpPost]
        public ActionResult RegisterDispatchWater(DispatchWaterModel DispatchWaterModel, IEnumerable<CProductModel> mProduct, IEnumerable<MCommandDetail> commandDetails)
        {
            string Mess = "";
            log.Info("Start controller command RegisterCommand");

            // Kiểm tra null để tránh lỗi
            if (DispatchWaterModel == null || DispatchWaterModel.Dispatch == null)
            {
                log.Error("DispatchWaterModel hoặc DispatchWaterModel.Dispatch đang null.");
                TempData["AlertMessage"] = "Dữ liệu không hợp lệ!";
                return RedirectToAction("DispatchWaterView", "DispatchWater");
            }

            var oItem = new MDispatchWater();

            oItem.TimeOrder = DateTime.ParseExact(DispatchWaterModel.TimeOrder.ToString(), Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            oItem.CertificateTime = DateTime.ParseExact(DispatchWaterModel.TimeOrder.ToString(), Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            oItem.CertificateNumber = int.Parse(DispatchWaterModel.Dispatch.CertificateNumber.ToString());
            oItem.VehicleNumber = DispatchWaterModel.Dispatch.VehicleNumber.ToString();

            oItem.InsertUser = HttpContext.User.Identity.Name;

            if (!string.IsNullOrEmpty(DispatchWaterModel.StartDate))
                oItem.TimeStart = DateTime.ParseExact(DispatchWaterModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(DispatchWaterModel.EndDate))
                oItem.TimeStop = DateTime.ParseExact(DispatchWaterModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

            oItem.DriverName1 = DispatchWaterModel.Dispatch.DriverName1?.Split(new[] { " - " }, StringSplitOptions.None)[0] ?? "";
            oItem.DriverName2 = !string.IsNullOrEmpty(DispatchWaterModel.Dispatch.DriverName2) ? DispatchWaterModel.Dispatch.DriverName2.Split(new[] { " - " }, StringSplitOptions.None)[0] : "";
            oItem.ProductCode = DispatchWaterModel.Dispatch.ProductCode?.Split(new[] { " - " }, StringSplitOptions.None)[0] ?? "";
            oItem.Department = DispatchWaterModel.Dispatch.Department?.Split(new[] { " - " }, StringSplitOptions.None)[0] ?? "";

            oItem.DstPickup1 = DispatchWaterModel.Dispatch.DstPickup1 ?? "";
            oItem.DstPickup2 = DispatchWaterModel.Dispatch.DstPickup2 ?? "";
            oItem.DstReceive = DispatchWaterModel.Dispatch.DstReceive ?? "";
            oItem.From = !string.IsNullOrEmpty(DispatchWaterModel.Dispatch.From)
                ? DispatchWaterModel.Dispatch.From
                : "";

            oItem.To = !string.IsNullOrEmpty(DispatchWaterModel.Dispatch.To)
                ? DispatchWaterModel.Dispatch.To
                : "";

            oItem.Paragraph1 = !string.IsNullOrEmpty(DispatchWaterModel.Dispatch.Paragraph1)
                ? DispatchWaterModel.Dispatch.Paragraph1
                : "";

            oItem.Paragraph2 = !string.IsNullOrEmpty(DispatchWaterModel.Dispatch.Paragraph2)
                ? DispatchWaterModel.Dispatch.Paragraph2
                : "";

            oItem.Paragraph3 = !string.IsNullOrEmpty(DispatchWaterModel.Dispatch.Paragraph3)
                ? DispatchWaterModel.Dispatch.Paragraph3
                : "";

            oItem.Paragraph4 = !string.IsNullOrEmpty(DispatchWaterModel.Dispatch.Paragraph4)
                ? DispatchWaterModel.Dispatch.Paragraph4
                : "";



            oItem.Note1 = DispatchWaterModel.Dispatch.Note1 ?? "";
            oItem.Remark = DispatchWaterModel.Dispatch.Remark ?? "";

            DispatchWaterService.CreateDispatch(oItem);

            var dispatchid = DispatchWaterService.GetNewId();

            log.Info("Finish controller command RegisterCommand");
            TempData["AlertMessage"] = Mess != "" ? Mess : "Thành công";

            return RedirectToAction("DispatchWaterView", "DispatchWater", new { id = dispatchid });
        }


        public ActionResult DispatchDetailWater(DispatchWaterModel DispatchWaterModel, int? page)
        {

            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERDISPATCH);
            if (checkPermission)
            {
                int pageNumber = page ?? 1;
                log.Info("start controller command commandView");
                var startdate = !string.IsNullOrEmpty(DispatchWaterModel.StartDate)
    ? DateTime.ParseExact(DispatchWaterModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
    : (DateTime?)null;

                var enddate = !string.IsNullOrEmpty(DispatchWaterModel.EndDate)
                    ? DateTime.ParseExact(DispatchWaterModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
                    : (DateTime?)null;

                var startTime = DispatchWaterModel.TimeOrder;
                var endTime = DispatchWaterModel.Dispatch.TimeOrder.ToString(Constants.DATE_FORMAT);

                DispatchWaterModel.ListIEDispatch = DispatchWaterService.GetList_Dispatch(
                    byte.Parse(Session[Constants.Session_WareHouse].ToString()),
                    DispatchWaterModel.CertificateNumber,
                    DispatchWaterModel.DstPickup1,
                    DispatchWaterModel.DstPickup2,
                    DispatchWaterModel.DstReceive,
                    startdate,
                    enddate,
                    DispatchWaterModel.From,
                    DispatchWaterModel.To,
                    DispatchWaterModel.Paragraph1,
                    DispatchWaterModel.Paragraph2,
                    DispatchWaterModel.Paragraph3,
                    DispatchWaterModel.Paragraph4
                ).ToPagedList(pageNumber, Constants.PAGE_SIZE);


                // Phần này nên viết lúc đăng nhập
                // ResourceManager rm = new ResourceManager("Resources.Messages", System.Reflection.Assembly.Load("App_GlobalResources"));
                Session[Constants.Session_TitleProviceName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("ProviceName");
                Session[Constants.Session_TitleReportCompanyName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("CompanyName");
                Session[Constants.Session_TitleCompanyAddress] = Constants.Session_Content_TitleCompanyAddress;// rm.GetString("CompanyAddress");
                Session[Constants.Session_TitleCompanyFax] = Constants.Session_Content_TitleCompanyFax;// rm.GetString("CompanyFax");
                Session[Constants.Session_TitleCompanyPhone] = Constants.Session_Content_TitleCompanyPhone;// rm.GetString("CompanyPhone");
                DispatchWaterModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();
                DispatchWaterModel.ListDispatch = DispatchWaterService.GetAllDispatch().ToList();
                DispatchWaterModel.TimeOrder = ((DateTime)DispatchWaterModel.Dispatch.TimeOrder).ToString(Constants.DATE_FORMAT);
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
                //commandModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT); 
                #region Khách hàng

                var lstC = new List<Datum>();
                var lst1 = CustomerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                DispatchWaterModel.LstCustomer = lstC;
                #endregion

                var lstP = new List<Datum>();
                var lst = ProductService.GetAllProduct();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.ProductCode;
                    datum.type = it.ProductName;
                    lstP.Add(datum);
                }
                DispatchWaterModel.LstProduct = lstP;
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListVehicle = shipService.GetAllShip().ToList();
                DispatchWaterModel.MImage = imageService.GetImageByUsername(HttpContext.User.Identity.Name);

                //Lấy danh hàng hóa
                DispatchWaterModel.ListTemProduct = ProductService.GetAllProduct().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.ProductCode,
                    Name = x.ProductName,
                    Unit = "",
                    Value = x.ProductId
                }).ToList();

                // Lấy danh sách phương tiện
                DispatchWaterModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.VehicleNumber,
                    Name = x.VehicleNumber
                }).ToList();

                var objDriver = DispatchWaterModel.Dispatch.DriverName1;
                var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
                if (objcmt != null)
                {
                    Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
                }
                else
                    Session[Constants.Session_TitleIdentificationNumber] = "";
                log.Info("Finish controller dispatchView");
                return View(DispatchWaterModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        public ActionResult DispatchDetailWater_1(DispatchWaterModel DispatchWaterModel, int? page)
        {

            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERDISPATCH);
            if (checkPermission)
            {
                int pageNumber = page ?? 1;
                log.Info("start controller command commandView");
                var startdate = !string.IsNullOrEmpty(DispatchWaterModel.StartDate)
    ? DateTime.ParseExact(DispatchWaterModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
    : (DateTime?)null;

                var enddate = !string.IsNullOrEmpty(DispatchWaterModel.EndDate)
                    ? DateTime.ParseExact(DispatchWaterModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
                    : (DateTime?)null;

                var startTime = DispatchWaterModel.TimeOrder;
                var endTime = DispatchWaterModel.Dispatch.TimeOrder.ToString(Constants.DATE_FORMAT);

                DispatchWaterModel.ListIEDispatch = DispatchWaterService.GetList_Dispatch(
                    byte.Parse(Session[Constants.Session_WareHouse].ToString()),
                    DispatchWaterModel.CertificateNumber,
                    DispatchWaterModel.DstPickup1,
                    DispatchWaterModel.DstPickup2,
                    DispatchWaterModel.DstReceive,
                    startdate,
                    enddate,
                    DispatchWaterModel.From,
                    DispatchWaterModel.To,
                    DispatchWaterModel.Paragraph1,
                    DispatchWaterModel.Paragraph2,
                    DispatchWaterModel.Paragraph3,
                    DispatchWaterModel.Paragraph4
                ).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                DispatchWaterModel.MImage = imageService.GetImageByUsername(HttpContext.User.Identity.Name);


                // Phần này nên viết lúc đăng nhập
                // ResourceManager rm = new ResourceManager("Resources.Messages", System.Reflection.Assembly.Load("App_GlobalResources"));
                Session[Constants.Session_TitleProviceName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("ProviceName");
                Session[Constants.Session_TitleReportCompanyName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("CompanyName");
                Session[Constants.Session_TitleCompanyAddress] = Constants.Session_Content_TitleCompanyAddress;// rm.GetString("CompanyAddress");
                Session[Constants.Session_TitleCompanyFax] = Constants.Session_Content_TitleCompanyFax;// rm.GetString("CompanyFax");
                Session[Constants.Session_TitleCompanyPhone] = Constants.Session_Content_TitleCompanyPhone;// rm.GetString("CompanyPhone");
                DispatchWaterModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();
                DispatchWaterModel.ListDispatch = DispatchWaterService.GetAllDispatch().ToList();
                DispatchWaterModel.TimeOrder = ((DateTime)DispatchWaterModel.Dispatch.TimeOrder).ToString(Constants.DATE_FORMAT);
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
                //commandModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT); 
                #region Khách hàng

                var lstC = new List<Datum>();
                var lst1 = CustomerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                DispatchWaterModel.LstCustomer = lstC;
                #endregion

                var lstP = new List<Datum>();
                var lst = ProductService.GetAllProduct();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.ProductCode;
                    datum.type = it.ProductName;
                    lstP.Add(datum);
                }
                DispatchWaterModel.LstProduct = lstP;
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListVehicle = shipService.GetAllShip().ToList();

                //Lấy danh hàng hóa
                DispatchWaterModel.ListTemProduct = ProductService.GetAllProduct().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.ProductCode,
                    Name = x.ProductName,
                    Unit = "",
                    Value = x.ProductId
                }).ToList();

                // Lấy danh sách phương tiện
                DispatchWaterModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.VehicleNumber,
                    Name = x.VehicleNumber
                }).ToList();

                var objDriver = DispatchWaterModel.Dispatch.DriverName1;
                var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
                if (objcmt != null)
                {
                    Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
                }
                else
                    Session[Constants.Session_TitleIdentificationNumber] = "";
                log.Info("Finish controller dispatchView");
                return View(DispatchWaterModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        public ActionResult DispatchDetailWater_2(DispatchWaterModel DispatchWaterModel, int? page)
        {

            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERDISPATCH);
            if (checkPermission)
            {
                int pageNumber = page ?? 1;
                log.Info("start controller command commandView");
                var startdate = !string.IsNullOrEmpty(DispatchWaterModel.StartDate)
    ? DateTime.ParseExact(DispatchWaterModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
    : (DateTime?)null;

                var enddate = !string.IsNullOrEmpty(DispatchWaterModel.EndDate)
                    ? DateTime.ParseExact(DispatchWaterModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
                    : (DateTime?)null;

                var startTime = DispatchWaterModel.TimeOrder;
                var endTime = DispatchWaterModel.Dispatch.TimeOrder.ToString(Constants.DATE_FORMAT);

                DispatchWaterModel.ListIEDispatch = DispatchWaterService.GetList_Dispatch(
                    byte.Parse(Session[Constants.Session_WareHouse].ToString()),
                    DispatchWaterModel.CertificateNumber,
                    DispatchWaterModel.DstPickup1,
                    DispatchWaterModel.DstPickup2,
                    DispatchWaterModel.DstReceive,
                    startdate,
                    enddate,
                    DispatchWaterModel.From,
                    DispatchWaterModel.To,
                    DispatchWaterModel.Paragraph1,
                    DispatchWaterModel.Paragraph2,
                    DispatchWaterModel.Paragraph3,
                    DispatchWaterModel.Paragraph4
                ).ToPagedList(pageNumber, Constants.PAGE_SIZE);

                DispatchWaterModel.MImage = imageService.GetImageByUsername(HttpContext.User.Identity.Name);

                // Phần này nên viết lúc đăng nhập
                // ResourceManager rm = new ResourceManager("Resources.Messages", System.Reflection.Assembly.Load("App_GlobalResources"));
                Session[Constants.Session_TitleProviceName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("ProviceName");
                Session[Constants.Session_TitleReportCompanyName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("CompanyName");
                Session[Constants.Session_TitleCompanyAddress] = Constants.Session_Content_TitleCompanyAddress;// rm.GetString("CompanyAddress");
                Session[Constants.Session_TitleCompanyFax] = Constants.Session_Content_TitleCompanyFax;// rm.GetString("CompanyFax");
                Session[Constants.Session_TitleCompanyPhone] = Constants.Session_Content_TitleCompanyPhone;// rm.GetString("CompanyPhone");
                DispatchWaterModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();
                DispatchWaterModel.ListDispatch = DispatchWaterService.GetAllDispatch().ToList();
                DispatchWaterModel.TimeOrder = ((DateTime)DispatchWaterModel.Dispatch.TimeOrder).ToString(Constants.DATE_FORMAT);
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
                //commandModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT); 
                #region Khách hàng

                var lstC = new List<Datum>();
                var lst1 = CustomerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                DispatchWaterModel.LstCustomer = lstC;
                #endregion

                var lstP = new List<Datum>();
                var lst = ProductService.GetAllProduct();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.ProductCode;
                    datum.type = it.ProductName;
                    lstP.Add(datum);
                }
                DispatchWaterModel.LstProduct = lstP;
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListVehicle = shipService.GetAllShip().ToList();

                //Lấy danh hàng hóa
                DispatchWaterModel.ListTemProduct = ProductService.GetAllProduct().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.ProductCode,
                    Name = x.ProductName,
                    Unit = "",
                    Value = x.ProductId
                }).ToList();

                // Lấy danh sách phương tiện
                DispatchWaterModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.VehicleNumber,
                    Name = x.VehicleNumber
                }).ToList();

                var objDriver = DispatchWaterModel.Dispatch.DriverName1;
                var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
                if (objcmt != null)
                {
                    Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
                }
                else
                    Session[Constants.Session_TitleIdentificationNumber] = "";
                log.Info("Finish controller dispatchView");
                return View(DispatchWaterModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        public ActionResult DispatchDetailWater_3(DispatchWaterModel DispatchWaterModel, int? page)
        {

            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERDISPATCH);
            if (checkPermission)
            {
                int pageNumber = page ?? 1;
                log.Info("start controller command commandView");
                var startdate = !string.IsNullOrEmpty(DispatchWaterModel.StartDate)
    ? DateTime.ParseExact(DispatchWaterModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
    : (DateTime?)null;

                var enddate = !string.IsNullOrEmpty(DispatchWaterModel.EndDate)
                    ? DateTime.ParseExact(DispatchWaterModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture)
                    : (DateTime?)null;

                var startTime = DispatchWaterModel.TimeOrder;
                var endTime = DispatchWaterModel.Dispatch.TimeOrder.ToString(Constants.DATE_FORMAT);

                DispatchWaterModel.ListIEDispatch = DispatchWaterService.GetList_Dispatch(
                    byte.Parse(Session[Constants.Session_WareHouse].ToString()),
                    DispatchWaterModel.CertificateNumber,
                    DispatchWaterModel.DstPickup1,
                    DispatchWaterModel.DstPickup2,
                    DispatchWaterModel.DstReceive,
                    startdate,
                    enddate,
                    DispatchWaterModel.From,
                    DispatchWaterModel.To,
                    DispatchWaterModel.Paragraph1,
                    DispatchWaterModel.Paragraph2,
                    DispatchWaterModel.Paragraph3,
                    DispatchWaterModel.Paragraph4
                ).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                DispatchWaterModel.MImage = imageService.GetImageByUsername(HttpContext.User.Identity.Name);


                // Phần này nên viết lúc đăng nhập
                // ResourceManager rm = new ResourceManager("Resources.Messages", System.Reflection.Assembly.Load("App_GlobalResources"));
                Session[Constants.Session_TitleProviceName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("ProviceName");
                Session[Constants.Session_TitleReportCompanyName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("CompanyName");
                Session[Constants.Session_TitleCompanyAddress] = Constants.Session_Content_TitleCompanyAddress;// rm.GetString("CompanyAddress");
                Session[Constants.Session_TitleCompanyFax] = Constants.Session_Content_TitleCompanyFax;// rm.GetString("CompanyFax");
                Session[Constants.Session_TitleCompanyPhone] = Constants.Session_Content_TitleCompanyPhone;// rm.GetString("CompanyPhone");
                DispatchWaterModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();
                DispatchWaterModel.ListDispatch = DispatchWaterService.GetAllDispatch().ToList();
                DispatchWaterModel.TimeOrder = ((DateTime)DispatchWaterModel.Dispatch.TimeOrder).ToString(Constants.DATE_FORMAT);
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
                //commandModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT); 
                #region Khách hàng

                var lstC = new List<Datum>();
                var lst1 = CustomerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                DispatchWaterModel.LstCustomer = lstC;
                #endregion

                var lstP = new List<Datum>();
                var lst = ProductService.GetAllProduct();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.ProductCode;
                    datum.type = it.ProductName;
                    lstP.Add(datum);
                }
                DispatchWaterModel.LstProduct = lstP;
                DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
                DispatchWaterModel.ListVehicle = shipService.GetAllShip().ToList();

                //Lấy danh hàng hóa
                DispatchWaterModel.ListTemProduct = ProductService.GetAllProduct().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.ProductCode,
                    Name = x.ProductName,
                    Unit = "",
                    Value = x.ProductId
                }).ToList();

                // Lấy danh sách phương tiện
                DispatchWaterModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.VehicleNumber,
                    Name = x.VehicleNumber
                }).ToList();

                var objDriver = DispatchWaterModel.Dispatch.DriverName1;
                var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
                if (objcmt != null)
                {
                    Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
                }
                else
                    Session[Constants.Session_TitleIdentificationNumber] = "";
                log.Info("Finish controller dispatchView");
                return View(DispatchWaterModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        public ActionResult EditWater(int id)
        {
            log.Info("view");
            DispatchWaterModel.Dispatch = DispatchWaterService.FindDispatchWaterById(id);
            return View(DispatchWaterModel);
        }

        public ActionResult DispatchWaterView(int Id, DispatchWaterModel DispatchWaterModel)
        {
            log.Info("start controller command commandView");
            int pageNumber = 1;
            // Phần này nên viết lúc đăng nhập
            // ResourceManager rm = new ResourceManager("Resources.Messages", System.Reflection.Assembly.Load("App_GlobalResources"));
            Session[Constants.Session_TitleProviceName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("ProviceName");
            Session[Constants.Session_TitleReportCompanyName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("CompanyName");
            Session[Constants.Session_TitleCompanyAddress] = Constants.Session_Content_TitleCompanyAddress;// rm.GetString("CompanyAddress");
            Session[Constants.Session_TitleCompanyFax] = Constants.Session_Content_TitleCompanyFax;// rm.GetString("CompanyFax");
            Session[Constants.Session_TitleCompanyPhone] = Constants.Session_Content_TitleCompanyPhone;// rm.GetString("CompanyPhone");
            DispatchWaterModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();
            DispatchWaterModel.Dispatch = DispatchWaterService.FindDispatchWaterById(Id);
            DispatchWaterModel.ListDepartment = departmentService.GetAllDepartment().ToList();
            DispatchWaterModel.ListIEDispatch = DispatchWaterService.GetList_Dispatch_byID(Id.ToString()).ToPagedList(pageNumber, Constants.PAGE_SIZE);
            DispatchWaterModel.TimeOrder = ((DateTime)DispatchWaterModel.Dispatch.TimeOrder).ToString(Constants.DATE_FORMAT);
            DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
            DispatchWaterModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
            DispatchWaterModel.MImage = imageService.GetImageByUsername(HttpContext.User.Identity.Name);

            //commandModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT); 
            #region Khách hàng

            var lstC = new List<Datum>();
            var lst1 = CustomerService.GetAllCustomer();
            foreach (var it in lst1)
            {
                var datum = new Datum();
                datum.type = it.CustomerName;
                datum.name = it.CustomerCode + " - " + it.CustomerName;
                lstC.Add(datum);
            }
            DispatchWaterModel.LstCustomer = lstC;
            #endregion

            var lstP = new List<Datum>();
            var lst = ProductService.GetAllProduct();
            foreach (var it in lst)
            {
                var datum = new Datum();
                datum.name = it.ProductCode;
                datum.type = it.ProductName;
                lstP.Add(datum);
            }
            DispatchWaterModel.LstProduct = lstP;
            DispatchWaterModel.ListProduct = ProductService.GetAllProduct().ToList();
            DispatchWaterModel.ListVehicle = shipService.GetAllShip().ToList();

            //Lấy danh hàng hóa 
            DispatchWaterModel.ListTemProduct = ProductService.GetAllProduct().Select(x => new DataValue
            {
                Caption = "",
                Code = x.ProductCode,
                Name = x.ProductName,
                Unit = "",
                Value = x.ProductId
            }).ToList();

            #region  Kho
            var lstKho = new List<Datum>();
            var lst3 = LocationService.GetAllLocation();
            foreach (var it in lst3)
            {
                var datum = new Datum();
                datum.name = it.LocationCode.Trim() + " - " + it.LocationName.Trim();
                datum.type = it.LocationCode.ToString().Trim();
                lstKho.Add(datum);
            }
            DispatchWaterModel.LstWareHouse = lstKho;
            #endregion

            #region Vehicle
            var lstV = new List<Datum>();
            var lst2 = shipService.GetAllShip();
            foreach (var it in lst2)
            {
                var datum = new Datum();
                datum.name = it.ShipName;
                datum.type = it.NumberControl;
                lstV.Add(datum);
            }
            DispatchWaterModel.LstVehicle = lstV;
            #endregion

            #region Lái xe
            var lstD = new List<Datum>();
            var lst4 = DriverService.GetAllDriver();
            foreach (var it in lst4)
            {
                var datum = new Datum();
                datum.name = it.Name;
                datum.type = it.IdentificationNumber;
                lstD.Add(datum);
            }
            DispatchWaterModel.LstDriver = lstD;
            #endregion

            // Lấy danh sách phương tiện
            DispatchWaterModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
            {
                Caption = "",
                Code = x.VehicleNumber,
                Name = x.VehicleNumber
            }).ToList();

            DispatchWaterModel.ListSeal = SealService.GetAllSeal().Where(x => x.CommandID == Id).ToList();
            var objDriver = DispatchWaterModel.Dispatch.DriverName1;
            var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
            if (objcmt != null)
            {
                Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
            }
            else
                Session[Constants.Session_TitleIdentificationNumber] = "";

            log.Info("Finish controller dispatchView");
            return View(DispatchWaterModel);
        }

        public bool DeleteWaterDispatch(int dispatchId)
        {
            var rs = false;
            rs = DispatchWaterService.DeleteDispatch(dispatchId);
            return rs;
        }

        public bool UpdateWaterDispatch(int dispatchId, string timeStart, string timeStop, string vehicle, string product, string driverName1, string driverName2, string dstPickup1, string dstPickup2, string department, string note, string remark, string dstReceive, string From, string To, string Paragraph1, string Paragraph2, string Paragraph3, string Paragraph4, string user)
        {
            var rs = false;
            rs = DispatchWaterService.UpdateDispatch(dispatchId, timeStart, timeStop, vehicle, product, driverName1, driverName2, dstPickup1, dstPickup2, department, note, remark, dstReceive, From, To, Paragraph1, Paragraph2, Paragraph3, Paragraph4, HttpContext.User.Identity.Name);
            return rs;
        }

    }
}