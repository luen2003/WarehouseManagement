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
    public class DispatchHistDto
    {
        public int DispatchID { get; set; }
        public int? CertificateNumber { get; set; }
        public DateTime? TimeOrder { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeStop { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName1 { get; set; }
        public string DriverName2 { get; set; }
        public string ProductCode { get; set; }
        public string Department { get; set; }
        public string DstPickup1 { get; set; }
        public string DstPickup2 { get; set; }
        public string DstReceive { get; set; }
        public string Note1 { get; set; }
        public string Remark { get; set; }
        public DateTime InsertDate { get; set; }
        public string InsertUser { get; set; }
        public int VersionNo { get; set; }
        public string SysU { get; set; }
        public DateTime? SysD { get; set; }
    }



    public class DispatchController : Controller
    {

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

        public DispatchModel dispatchModel;
        ILog log = log4net.LogManager.GetLogger(typeof(DispatchController));

        public DispatchController(IDispatchService dispatchService, DispatchModel dispatchModel, 
            IConfigurationService ConfigurationService, IUserService UserService, IProductService ProductService,
             ISealService SealService, IDepartmentService departmentService,
        IWareHouseService WareHouseService, ICustomerService CustomerService, IDriverService DriverService,
            IVehicleService VehicleService, ICardService CardService, IWareHouseService warehouseService, BaseService baseService)
        { 
            this.dispatchModel = dispatchModel;
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
                SysU,
                SysD
            FROM dbo.MDispatch_Hist
            WHERE DispatchID = @DispatchID AND VersionNo = @VersionNo
            ORDER BY VersionNo;";

            var db = new PetroBMContext();
            var p1 = new SqlParameter("@DispatchID", dispatchId);
            var p2 = new SqlParameter("@VersionNo", versionNo);

            var model = db.Database.SqlQuery<DispatchHistModel>(sql, p1, p2).FirstOrDefault();
            if (model == null) return HttpNotFound("Không tìm thấy bản ghi lịch sử.");

            return View("DispatchHistView", model);
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
                    FROM dbo.MDispatch_Hist WITH (NOLOCK)
                    WHERE DispatchID = @p0
                    ORDER BY VersionNo ASC, SysD ASC;";
            try
            {
                using (var db = new PetroBMContext())
                {
                    var rows = db.Database.SqlQuery<DispatchHistDto>(sql, dispatchId).ToList();
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
        public ActionResult RegisterDispatch()
        {
            log.Info("Start controller RegisterDispatch");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_MANUAL);
            if (checkPermission)
            {
                dispatchModel.Dispatch.TimeOrder = DateTime.Now; 
                dispatchModel.ListProduct = ProductService.GetAllProduct().ToList();
                dispatchModel.ListCustomer = CustomerService.GetAllCustomer().ToList(); 
                dispatchModel.ListDepartment = departmentService.GetAllDepartment().ToList();
                //dispatchModel.Command.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                //commandModel.TimeOrder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).ToString(Constants.DATE_FORMAT);
                dispatchModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT);
                dispatchModel.CertificateTime = DateTime.Now.ToString(Constants.DATE_FORMAT);
                dispatchModel.Dispatch.CertificateNumber = dispatchService.getCertificateNumber();
                //dispatchModel.Dispatch.ProductCode = ProductService.GetAllProduct().FirstOrDefault().ProductCode + " - " + ProductService.GetAllProduct().FirstOrDefault().ProductName;
                //dispatchModel.Dispatch.TimeStart = DateTime.Parse(dispatchModel.StartDate.ToString());
                //dispatchModel.Dispatch.TimeStop = DateTime.Parse(dispatchModel.EndDate.ToString());

                #region  Kho
                var lstWH = new List<Datum>();
                var lst = WareHouseService.GetAllWareHouse();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.WareHouseCode + " - " + it.WareHouseName;
                    datum.type = it.WareHouseCode.ToString();
                    lstWH.Add(datum);
                }
                dispatchModel.LstWareHouse = lstWH;
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
                dispatchModel.LstProduct = lstProduct;
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
                dispatchModel.LstCustomer = lstC;
                #endregion

                #region Phương tiện
                var lstV = new List<Datum>();
                var lst2 = VehicleService.GetAllVehicle();
                foreach (var it in lst2)
                {
                    var datum = new Datum();
                    datum.name = it.VehicleNumber;
                    datum.type = it.AccreditationNumber;
                    lstV.Add(datum);
                }
                dispatchModel.LstVehicle = lstV;
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
                dispatchModel.LstDriver = lstD;
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
                return View(dispatchModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }


        [HttpPost]
        public ActionResult RegisterDispatch(DispatchModel dispatchModel, IEnumerable<CProductModel> mProduct, IEnumerable<MCommandDetail> commandDetails)
        {
            string Mess = "";
            log.Info("Start controller command RegisterCommand");

            var oItem = new MDispatch();
            oItem.TimeOrder = DateTime.ParseExact(dispatchModel.TimeOrder.ToString(), Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            oItem.CertificateTime = DateTime.ParseExact(dispatchModel.TimeOrder.ToString(), Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            oItem.CertificateNumber = int.Parse(dispatchModel.Dispatch.CertificateNumber.ToString());
            oItem.VehicleNumber = dispatchModel.Dispatch.VehicleNumber.ToString();
            //commandModel.Command.TimeOrder = ((DateTime)commandModel.TimeOrder).ToString(Constants.DATE_FORMAT);
            //dispatchModel.Dispatch.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            // Xử lại tại đây chi tiết lệnh xuất hàng            
            oItem.InsertUser = HttpContext.User.Identity.Name;  
            if(!string.IsNullOrEmpty(dispatchModel.StartDate.ToString()))
                oItem.TimeStart = DateTime.ParseExact(dispatchModel.StartDate.ToString(), Constants.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(dispatchModel.EndDate.ToString()))
                oItem.TimeStop = DateTime.ParseExact(dispatchModel.EndDate.ToString(), Constants.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
            oItem.DriverName1 = dispatchModel.Dispatch.DriverName1.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString();
            if(!string.IsNullOrEmpty(dispatchModel.Dispatch.DriverName2))
                oItem.DriverName2 = dispatchModel.Dispatch.DriverName2.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString();
            oItem.ProductCode = dispatchModel.Dispatch.ProductCode.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString();
            oItem.Department = dispatchModel.Dispatch.Department.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString(); 

            oItem.DstPickup1 = dispatchModel.Dispatch.DstPickup1.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString();
            if (!string.IsNullOrEmpty(dispatchModel.Dispatch.DstPickup2))
                oItem.DstPickup2 = dispatchModel.Dispatch.DstPickup2.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString();
            oItem.DstReceive = dispatchModel.Dispatch.DstReceive.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString();
            if (!string.IsNullOrEmpty(dispatchModel.Dispatch.Note1))
                oItem.Note1 = dispatchModel.Dispatch.Note1.ToString();
            if (!string.IsNullOrEmpty(dispatchModel.Dispatch.Remark))
                oItem.Remark = dispatchModel.Dispatch.Remark.ToString();

            dispatchService.CreateDispatch(oItem);

            var dispatchid = dispatchService.GetNewId(); 

            log.Info("Finish controller command RegisterCommand");
            if (Mess != "")
                TempData["AlertMessage"] = Mess;
            else
                TempData["AlertMessage"] = "Thành công";
            return RedirectToAction("DispatchView", "Dispatch", new { id = dispatchid });
        }

        public ActionResult Edit(int id)
        {
            log.Info("view");
            dispatchModel.Dispatch = dispatchService.FindDispatchById(id);
            return View(dispatchModel);
        }

        public ActionResult DispatchView(int Id, DispatchModel dispatchModel)
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
            dispatchModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();
            dispatchModel.Dispatch = dispatchService.FindDispatchById(Id);
            dispatchModel.ListDepartment = departmentService.GetAllDepartment().ToList();
            dispatchModel.ListIEDispatch = dispatchService.GetList_Dispatch_byID(Id.ToString()).ToPagedList(pageNumber, Constants.PAGE_SIZE);
            dispatchModel.TimeOrder = ((DateTime)dispatchModel.Dispatch.TimeOrder).ToString(Constants.DATE_FORMAT);
            dispatchModel.ListProduct = ProductService.GetAllProduct().ToList();
            dispatchModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
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
            dispatchModel.LstCustomer = lstC;
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
            dispatchModel.LstProduct = lstP;
            dispatchModel.ListProduct = ProductService.GetAllProduct().ToList();
            dispatchModel.ListVehicle = VehicleService.GetAllVehicle().ToList(); 

            //Lấy danh hàng hóa 
            dispatchModel.ListTemProduct = ProductService.GetAllProduct().Select(x => new DataValue
            {
                Caption = "",
                Code = x.ProductCode,
                Name = x.ProductName,
                Unit = "",
                Value = x.ProductId
            }).ToList();

            #region  Kho
            var lstKho = new List<Datum>();
            var lst3 = WareHouseService.GetAllWareHouse();
            foreach (var it in lst3)
            {
                var datum = new Datum();
                datum.name = it.WareHouseCode + " - " + it.WareHouseName;
                datum.type = it.WareHouseCode.ToString();
                lstKho.Add(datum);
            }
            dispatchModel.LstWareHouse = lstKho;
            #endregion

            #region Vehicle
            var lstV = new List<Datum>();
            var lst2 = VehicleService.GetAllVehicle();
            foreach (var it in lst2)
            {
                var datum = new Datum();
                datum.name = it.VehicleNumber;
                datum.type = it.AccreditationNumber;
                lstV.Add(datum);
            }
            dispatchModel.LstVehicle = lstV;
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
            dispatchModel.LstDriver = lstD;
            #endregion

            // Lấy danh sách phương tiện
            dispatchModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
            {
                Caption = "",
                Code = x.VehicleNumber,
                Name = x.VehicleNumber
            }).ToList();

            dispatchModel.ListSeal = SealService.GetAllSeal().Where(x => x.CommandID == Id).ToList();
            var objDriver = dispatchModel.Dispatch.DriverName1;
            var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
            if (objcmt != null)
            {
                Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
            }
            else
                Session[Constants.Session_TitleIdentificationNumber] = ""; 

            log.Info("Finish controller dispatchView");
            return View(dispatchModel);
        }


        public ActionResult DispatchDetail(DispatchModel dispatchModel, int? page)
        {

            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERDISPATCH);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                log.Info("start controller command commandView");
                DateTime? startdate = DateTime.ParseExact(dispatchModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                DateTime? enddate = DateTime.ParseExact(dispatchModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                // Phần này nên viết lúc đăng nhập
                // ResourceManager rm = new ResourceManager("Resources.Messages", System.Reflection.Assembly.Load("App_GlobalResources"));
                Session[Constants.Session_TitleProviceName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("ProviceName");
                Session[Constants.Session_TitleReportCompanyName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("CompanyName");
                Session[Constants.Session_TitleCompanyAddress] = Constants.Session_Content_TitleCompanyAddress;// rm.GetString("CompanyAddress");
                Session[Constants.Session_TitleCompanyFax] = Constants.Session_Content_TitleCompanyFax;// rm.GetString("CompanyFax");
                Session[Constants.Session_TitleCompanyPhone] = Constants.Session_Content_TitleCompanyPhone;// rm.GetString("CompanyPhone");
                dispatchModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();
                dispatchModel.ListIEDispatch = dispatchService.GetList_Dispatch(byte.Parse(Session[Constants.Session_WareHouse].ToString()), dispatchModel.CertificateNumber, dispatchModel.DstPickup1, dispatchModel.DstPickup1, dispatchModel.DstReceive,
                     startdate, enddate).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                dispatchModel.ListDispatch = dispatchService.GetAllDispatch().ToList();
                dispatchModel.TimeOrder = ((DateTime)dispatchModel.Dispatch.TimeOrder).ToString(Constants.DATE_FORMAT);
                dispatchModel.ListProduct = ProductService.GetAllProduct().ToList();
                dispatchModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
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
                dispatchModel.LstCustomer = lstC;
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
                dispatchModel.LstProduct = lstP;
                dispatchModel.ListProduct = ProductService.GetAllProduct().ToList();
                dispatchModel.ListVehicle = VehicleService.GetAllVehicle().ToList();

                //Lấy danh hàng hóa 
                dispatchModel.ListTemProduct = ProductService.GetAllProduct().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.ProductCode,
                    Name = x.ProductName,
                    Unit = "",
                    Value = x.ProductId
                }).ToList();

                // Lấy danh sách phương tiện
                dispatchModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
                {
                    Caption = "",
                    Code = x.VehicleNumber,
                    Name = x.VehicleNumber
                }).ToList();
                 
                var objDriver = dispatchModel.Dispatch.DriverName1;
                var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
                if (objcmt != null)
                {
                    Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
                }
                else
                    Session[Constants.Session_TitleIdentificationNumber] = "";
                log.Info("Finish controller dispatchView");
                return View(dispatchModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public bool DeleteDispatch(int dispatchId)
        {
            var rs = false;
            rs = dispatchService.DeleteDispatch(dispatchId); 
            return rs; 
        }

        public bool UpdateDispatch(int dispatchId, string timeStart, string timeStop, string vehicle, string product, string driverName1, string driverName2, string dstPickup1, string dstPickup2, string department, string note, string remark, string dstReceive, string user)
        {
            var rs = false;
            rs = dispatchService.UpdateDispatch(dispatchId, timeStart, timeStop, vehicle, product, driverName1, driverName2, dstPickup1, dstPickup2, department, note, remark, dstReceive, HttpContext.User.Identity.Name);
            return rs;
        }
    }
}