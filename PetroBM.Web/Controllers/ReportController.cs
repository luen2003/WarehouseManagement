using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using PetroBM.Common.Util;
using System.Globalization;
using PagedList;
using System.Linq;
using PetroBM.Web.Attribute;
using System.Data;
using PetroBM.Domain.Entities;

namespace PetroBM.Web.Controllers
{
    [HasPermission(Constants.PERMISSION_REPORT)]
    public class ReportController : BaseController
    {
        private ReportModel reportModel;
        private readonly ReportService reportService;
        private readonly ITankService tankService;
        private readonly ITankGroupService tankGroupService;
        private readonly IWareHouseService warehouseService;
        private readonly ConfigurationService configurationService;
        private readonly IAlarmService alarmService;
        private readonly IEventService eventService;
        private readonly IProductService productService;
        private readonly IImportService importService;
        private readonly IConfigArmService configarmService;
        private readonly ICommandService commandService;
        private readonly ICommandDetailService commanddetailService;
        private readonly ICustomerService customerService;
        private readonly ICustomerGroupService customerGroupService;
        private readonly IVehicleService vehicleService;
        private readonly BaseService baseService; 


        public ReportController(ReportModel reportModel, ReportService reportService, ITankService tankService,
            ITankGroupService tankGroupService, ConfigurationService configurationService,
            IAlarmService alarmService, IEventService eventService, IProductService productService,
            IImportService importService, IWareHouseService warehouseService, IConfigArmService configarmService,
            ICommandService commandService, ICommandDetailService commanddetailService, ICustomerService customerService, ICustomerGroupService customerGroupService,
            IVehicleService vehicleService, BaseService baseService)
        {
            this.reportModel = reportModel;
            this.reportService = reportService;
            this.tankService = tankService;
            this.tankGroupService = tankGroupService;
            this.configurationService = configurationService;
            this.alarmService = alarmService;
            this.eventService = eventService;
            this.productService = productService;
            this.importService = importService;
            this.warehouseService = warehouseService;
            this.configarmService = configarmService;
            this.commandService = commandService;
            this.commanddetailService = commanddetailService;
            this.vehicleService = vehicleService;
            this.baseService = baseService;
            this.customerService = customerService;
            this.customerGroupService = customerGroupService;
            

        }

        public JsonResult DataCommandDetail(ReportModel reportModel, int configarmid, int warehousecode, string startdate, string enddate)
        {

            DateTime? startDate = null;
            DateTime? endDate = null;
            var armNo = 0;
            armNo = configarmService.FindConfigArmById((int)(configarmid)).ArmNo;
            var warehouseCode = warehouseService.FindWareHouseById((int)(warehousecode)).WareHouseCode;
            if (!string.IsNullOrEmpty(startdate))
            {
                startDate = DateTime.ParseExact(startdate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(enddate))
            {
                endDate = DateTime.ParseExact(enddate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            var obj = commanddetailService.GetAllCommandDetail().Where(x => x.TimeStart > startDate && endDate > x.TimeStart && x.WareHouseCode == (byte)(warehouseCode) && x.ArmNo == armNo).ToList();



            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Datalog(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_DATALOG);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                reportModel.ListWareHouse = warehouseService.GetAllWareHouse();

                if (reportModel.WareHouseCode == null)
                {
                    foreach (var item in reportModel.ListWareHouse)
                    {
                        reportModel.WareHouseCode = item.WareHouseCode;
                        break;
                    }
                }


                DateTime? endDate = null;
                reportModel.EventTypeList = eventService.GetAllEventType();

                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                reportModel.DataTable = reportService.DataLogData(endDate, reportModel.WareHouseCode);

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Datalog(ReportModel reportModel)
        {
            DateTime? endDate = null;


            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("Datalog", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/DatalogReport.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.DataLogData(endDate, reportModel.WareHouseCode);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);

            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_KIEMKE_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_KIEMKE_");
            }
        }

        public ActionResult DifferenceTank(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(System.Web.HttpContext.Current.User.Identity.Name, Constants.PERMISSION_REPORT_DIFFERENCETANK);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouse();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();//Lấy tất cả họng ở tất cả các kho

                //#region Mặc định các thông tin
                ////Lấy Kho mặc định
                //if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;

                //			break;
                //		}
                //	}
                //}

                ////Lấy Họng mặc định
                //if (reportModel.ArmNo == null)
                //{
                //	foreach (var item in reportModel.ListConfigArm)
                //	{
                //		if (reportModel.ArmNo == null && item.WareHouseCode == reportModel.WareHouseCode)
                //		{
                //			reportModel.ArmNo = item.ArmNo;
                //			break;
                //		}
                //	}
                //}


                //if (reportModel.ProductCode == null)
                //{
                //	foreach (var item in reportModel.ListProduct) //lấy hàng hóa mặc định
                //	{
                //		if (reportModel.ProductCode == null)
                //		{
                //			reportModel.ProductCode = item.ProductCode;
                //			break;
                //		}
                //	}
                //}
                //#endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                reportModel.DataTable2 = reportService.Report_Deviation_By_GroupDeviation(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult DifferenceTank(ReportModel reportModel)
        {

            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            DateTime? startDate = null;
            DateTime? endDate = null;
            reportModel.EventTypeList = eventService.GetAllEventType();

            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("DifferenceTank", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
            reportModel.DataTable2 = reportService.Report_Deviation_By_GroupDeviation(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/DifferenceTank.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
            rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_Deviation_By_GroupDeviation(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
            rds2.Name = "DataSet2";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);


            reportModel.StartDate = Convert.ToDateTime(startDate).ToString(Constants.DATE_FORMAT);
            reportModel.EndDate = Convert.ToDateTime(endDate).ToString(Constants.DATE_FORMAT);


            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_SAISOMEXUAT_CHENHLECHLUONGXUAT_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_SAISOMEXUAT_CHENHLECHLUONGXUAT_");
            }

            //return View(reportModel);
        }

        public ActionResult TankExport(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_TANKEXPORT);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>();
                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName();

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}


                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult TankExport(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("TankExport", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/TankExport1.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.ClockExportData(reportModel.ImportId);
            rds2.Name = "DataSet2";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }


            //reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();

            //DateTime? startDate = null;
            //DateTime? endDate = null;

            //reportModel.EventTypeList = eventService.GetAllEventType();
            //if (string.IsNullOrEmpty(reportModel.FileType))
            //{
            //    return RedirectToAction("BalanceTank", reportModel);
            //}
            //if (!string.IsNullOrEmpty(reportModel.StartDate))
            //{
            //    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            //}
            //if (!string.IsNullOrEmpty(reportModel.EndDate))
            //{
            //    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            //}

            //LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Server.MapPath("~/Views/Report/BalanceTank.rdlc");
            //localReport.EnableExternalImages = true;

            //ReportDataSource rds = new ReportDataSource();
            //rds.Value = reportService.InputExportExist_DataTankLog(startDate, endDate, reportModel.WareHouseCode, reportModel.ProductId);
            //rds.Name = "DataSet1";
            //localReport.DataSources.Clear();
            //localReport.DataSources.Add(rds);


            //reportModel.StartDate = Convert.ToDateTime(startDate).ToString(Constants.DATE_FORMAT);
            //reportModel.EndDate = Convert.ToDateTime(endDate).ToString(Constants.DATE_FORMAT);


            //localReport.SetParameters(ReportParam(reportModel));

            //if (reportModel.FileType == "EXCEL")
            //{
            //    return A4HorizontalExcel(localReport, "BAOCAO_CANBANGLUONGXUAT_");
            //}
            //else
            //{
            //    return A4HorizontalPDF(localReport, "BAOCAO_CANBANGLUONGXUAT_");
            //}
            //DateTime? startDate = null;
            //DateTime? endDate = null;


            //reportModel.EventTypeList = eventService.GetAllEventType();
            //if (!string.IsNullOrEmpty(reportModel.StartDate))
            //{
            //	startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            //}
            //if (!string.IsNullOrEmpty(reportModel.EndDate))
            //{
            //	endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            //}
            //reportModel.DataTable = reportService.DataCommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            //return View(reportModel);
        }

        public ActionResult BangKeTHTheoPhuongTien(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT1);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>();
                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName();

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}
                #region Khách hàng
                var lstC = new List<Datum>();
                var lst1 = customerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                reportModel.ListCustomer = lstC;
                #endregion

                #region Nhóm khách hàng
                var lstCG = new List<Datum>();
                var lst2 = customerGroupService.GetAllCustomerGroup();
                foreach (var it in lst2)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerGroupName;
                    datum.name = it.CustomerGroupCode + " - " + it.CustomerGroupName;
                    lstCG.Add(datum);
                }
                reportModel.ListCustomerGroup = lstCG;
                #endregion 

                #region Phương tiện
                var lstvehicle = new List<Datum>();
                var lst3 = vehicleService.GetAllVehicle();
                foreach (var it in lst3)
                {
                    var datum = new Datum();
                    datum.type = it.VehicleNumber;
                    datum.name = it.VehicleNumber;
                    lstvehicle.Add(datum);
                }
                reportModel.ListVehicleFillter = lstvehicle;
                #endregion

                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    if (!string.IsNullOrEmpty(reportModel.Customer))
                        reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(reportModel.CustomerGroup))
                        reportModel.CustomerGroup = reportModel.CustomerGroup.Split(new string[] { " - " }, StringSplitOptions.None)[0];

                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
                    reportModel.BangKeTHTheoPhuongTien = reportService.Report_BangKeTHTheoPhuongTien(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.BangKeTHTheoPhuongTien_Total = reportService.Report_BangKeTHTheoPhuongTien_Total(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult BangKeTHTheoPhuongTien(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BangKeTHTheoPhuongTien", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BangKeTHTheoPhuongTien.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            //rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            //rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_BangKeTHTheoPhuongTien(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
            rds2.Name = "TheoPhuongTien";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            //reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }
        }

        public ActionResult BangKeTHTheoHong(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT1);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>();
                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName();

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}
                #region Khách hàng
                var lstC = new List<Datum>();
                var lst1 = customerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                reportModel.ListCustomer = lstC;
                #endregion

                #region Nhóm khách hàng
                var lstCG = new List<Datum>();
                var lst2 = customerGroupService.GetAllCustomerGroup();
                foreach (var it in lst2)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerGroupName;
                    datum.name = it.CustomerGroupCode + " - " + it.CustomerGroupName;
                    lstCG.Add(datum);
                }
                reportModel.ListCustomerGroup = lstCG;
                #endregion 

                #region Phương tiện
                var lstvehicle = new List<Datum>();
                var lst3 = vehicleService.GetAllVehicle();
                foreach (var it in lst3)
                {
                    var datum = new Datum();
                    datum.type = it.VehicleNumber;
                    datum.name = it.VehicleNumber;
                    lstvehicle.Add(datum);
                }
                reportModel.ListVehicleFillter = lstvehicle;
                #endregion

                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    if (!string.IsNullOrEmpty(reportModel.Customer))
                        reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(reportModel.CustomerGroup))
                        reportModel.CustomerGroup = reportModel.CustomerGroup.Split(new string[] { " - " }, StringSplitOptions.None)[0];

                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
                    reportModel.BangKeTHTheoHong = reportService.Report_BangKeTHTheoHong(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.BangKeTHTheoHong_Total = reportService.Report_BangKeTHTheoHong_Total(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult BangKeTHTheoHong(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BangKeTHTheoHong", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BangKeTHTheoHong.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            //rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            //rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_BangKeTHTheoHong(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
            rds2.Name = "TheoPhuongTien";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            //reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }
        }

        public ActionResult BangKeTHTheoNhomKhach(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT1);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>();
                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName(); 

                #region Khách hàng
                var lstC = new List<Datum>();
                var lst1 = customerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                reportModel.ListCustomer = lstC;
                #endregion

                #region Nhóm khách hàng
                var lstCG = new List<Datum>();
                var lst2 = customerGroupService.GetAllCustomerGroup();
                foreach (var it in lst2)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerGroupName;
                    datum.name = it.CustomerGroupCode + " - " + it.CustomerGroupName;
                    lstCG.Add(datum);
                }
                reportModel.ListCustomerGroup = lstCG;
                #endregion 

                #region Phương tiện
                var lstvehicle = new List<Datum>();
                var lst3 = vehicleService.GetAllVehicle();
                foreach (var it in lst3)
                {
                    var datum = new Datum();
                    datum.type = it.VehicleNumber;
                    datum.name = it.VehicleNumber;
                    lstvehicle.Add(datum);
                }
                reportModel.ListVehicleFillter = lstvehicle;
                #endregion

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //} 

                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                } 
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    if (!string.IsNullOrEmpty(reportModel.Customer))
                        reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(reportModel.CustomerGroup))
                        reportModel.CustomerGroup = reportModel.CustomerGroup.Split(new string[] { " - " }, StringSplitOptions.None)[0];

                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Customer, reportModel.TypeExport);
                    reportModel.BangKeTHTheoNhomKhach = reportService.Report_BangKeTHTheoNhomKhach(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.BangKeTHTheoNhomKhach_Total = reportService.Report_BangKeTHTheoNhomKhach_Total(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult BangKeTHTheoNhomKhach(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BangKeTHTheoNhomKhach", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(reportModel.Customer))
                reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BangKeTHTheoNhomKhach.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            //rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            //rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_BangKeTHTheoNhomKhach(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
            rds2.Name = "TheoNhomKhach";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            //reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }
        }

        public ActionResult BangKeTHTheoNhomKhach2(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT1);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>(); 

                #region Khách hàng
                var lstC = new List<Datum>();
                var lst1 = customerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                reportModel.ListCustomer = lstC;
                #endregion

                #region Nhóm khách hàng
                var lstCG = new List<Datum>();
                var lst2 = customerGroupService.GetAllCustomerGroup();
                foreach (var it in lst2)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerGroupName;
                    datum.name = it.CustomerGroupCode + " - " + it.CustomerGroupName;
                    lstCG.Add(datum);
                }
                reportModel.ListCustomerGroup = lstCG;
                #endregion 

                #region Phương tiện
                var lstvehicle = new List<Datum>();
                var lst3 = vehicleService.GetAllVehicle();
                foreach (var it in lst3)
                {
                    var datum = new Datum();
                    datum.type = it.VehicleNumber;
                    datum.name = it.VehicleNumber;
                    lstvehicle.Add(datum);
                }
                reportModel.ListVehicleFillter = lstvehicle;
                #endregion


                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName();

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}


                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    if (!string.IsNullOrEmpty(reportModel.Customer))
                        reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(reportModel.CustomerGroup))
                        reportModel.CustomerGroup = reportModel.CustomerGroup.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
                    reportModel.BangKeTHTheoNhomKhach2 = reportService.Report_BangKeTHTheoNhomKhach2(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.BangKeTHTheoNhomKhach2_Total = reportService.Report_BangKeTHTheoNhomKhach_Total(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult BangKeTHTheoNhomKhach2(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BangKeTHTheoNhomKhach2", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BangKeTHTheoNhomKhach2.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            //rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            //rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_BangKeTHTheoNhomKhach2(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
            rds2.Name = "TheoNhomKhach2";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            //reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }
        }

        public ActionResult BangKeCTRaVaoKho(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT1);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>();
                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName();

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}


                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                #region Khách hàng
                var lstC = new List<Datum>();
                var lst1 = customerService.GetAllCustomer();
                foreach (var it in lst1)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerName;
                    datum.name = it.CustomerCode + " - " + it.CustomerName;
                    lstC.Add(datum);
                }
                reportModel.ListCustomer = lstC;
                #endregion

                #region Nhóm khách hàng
                var lstCG = new List<Datum>();
                var lst2 = customerGroupService.GetAllCustomerGroup();
                foreach (var it in lst2)
                {
                    var datum = new Datum();
                    datum.type = it.CustomerGroupName;
                    datum.name = it.CustomerGroupCode + " - " + it.CustomerGroupName;
                    lstCG.Add(datum);
                }
                reportModel.ListCustomerGroup = lstCG;
                #endregion 

                #region Phương tiện
                var lstvehicle = new List<Datum>();
                var lst3 = vehicleService.GetAllVehicle();
                foreach (var it in lst3)
                {
                    var datum = new Datum();
                    datum.type = it.VehicleNumber;
                    datum.name = it.VehicleNumber;
                    lstvehicle.Add(datum);
                }
                reportModel.ListVehicleFillter = lstvehicle;
                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    if (!string.IsNullOrEmpty(reportModel.Customer))
                        reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(reportModel.CustomerGroup))
                        reportModel.CustomerGroup = reportModel.CustomerGroup.Split(new string[] { " - " }, StringSplitOptions.None)[0];

                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
                    reportModel.BangKeCTRaVaoKho = reportService.Report_BangKeCTRaVaoKho(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
                    //reportModel.BangKeTHTheoNhomKhach2_Total = reportService.Report_BangKeTHTheoNhomKhach_Total(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult BangKeCTRaVaoKho(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BangKeCTRaVaoKho", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BangKeCTRaVaoKho.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            //rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            //rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_BangKeCTRaVaoKho(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);
            rds2.Name = "CTRaVaoKho";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            //reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }
        }

        public ActionResult BangKeChiTietTheoNgan(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT1);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>();
                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName();

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}


                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    if (!string.IsNullOrEmpty(reportModel.Customer))
                        reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(reportModel.CustomerGroup))
                        reportModel.CustomerGroup = reportModel.CustomerGroup.Split(new string[] { " - " }, StringSplitOptions.None)[0];

                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
                    reportModel.BangKeChiTietTheoNgan = reportService.Report_BangKeChiTietXuatTheoNgan(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
                    reportModel.BangKeChiTiet_ToTal = reportService.Report_BangKeChiTietXuat_Total(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult BangKeChiTietTheoNgan(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BangKeChiTietTheoNgan", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);


            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BangKeChiTietTheoNgan.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_BangKeChiTietXuatTheoNgan(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
            rds2.Name = "DataSet2";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            //reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }
        }

        public ActionResult BangKeChiTiet(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT2);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();
                reportModel.ListVehicle = vehicleService.GetAllVehicleOrderByName().ToList<MVehicle>();
                reportModel.ListCommandDetail = commanddetailService.GetAllCommandDetailOrderByName();

                #region Mặc định các thông tin
                //Lấy Kho mặc định
                //         if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}


                if (reportModel.TypeExport == null)
                {
                    reportModel.TypeExport = 2;
                }

                #endregion

                DateTime? startDate = null;
                DateTime? endDate = null;


                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                if (reportModel.WareHouseCode != null)
                {
                    if (!string.IsNullOrEmpty(reportModel.Customer))
                        reportModel.Customer = reportModel.Customer.Split(new string[] { " - " }, StringSplitOptions.None)[0];
                    if (!string.IsNullOrEmpty(reportModel.CustomerGroup))
                        reportModel.CustomerGroup = reportModel.CustomerGroup.Split(new string[] { " - " }, StringSplitOptions.None)[0];

                    reportModel.DataTable = reportService.DataTankExport(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
                    reportModel.BangKeChiTiet = reportService.Report_BangKeChiTietXuat(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
                    reportModel.BangKeChiTiet_ToTal = reportService.Report_BangKeChiTietXuat_Total(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
                    reportModel.ListCommandOfSearch = reportModel.ListCommand.Where(cm => cm.CertificateTime < endDate && cm.CertificateTime > startDate).ToList();
                    reportModel.ListCommandDetailOfSearch = reportModel.ListCommandDetail.Where(cd => cd.TimeOrder < endDate && cd.TimeOrder > startDate).ToList();
                }


                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult BangKeChiTiet(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            reportModel.ListProduct = productService.GetAllProductOrderByName();
            reportModel.EventTypeList = eventService.GetAllEventType();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BangKeChiTiet", reportModel);
            }

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            reportModel.DataTable = reportService.Report_Deviation_By_CommandDetail(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation, reportModel.Customer, reportModel.CustomerGroup);

            
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BangKeChiTiet.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.TankExportData(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.CustomerName, reportModel.TypeExport);
            rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.Report_BangKeChiTietXuat(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, reportModel.ProductCode, reportModel.Vehicle, reportModel.Deviation);
            rds2.Name = "DataSet2";

            //ReportDataSource rds3 = new ReportDataSource();
            //rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            //rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            //localReport.DataSources.Add(rds3);

            //reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_XUATHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_XUATHANG_");
            }
        }

        public ActionResult ForecastTank(ReportModel reportModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_FORCECASTTANK);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();

                DateTime? startDate = null;
                DateTime? endDate = null;

                reportModel.EventTypeList = eventService.GetAllEventType();

                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        public ActionResult HistoryTank(ReportModel reportModel)
        {
            return View(reportModel);
        }


        public ActionResult HistoryExport(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_HISTORYEXPORT);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();

                reportModel.ListCommand = commandService.GetAllCommandOrderByName();
                DateTime? startDate = null;
                DateTime? endDate = null;

                //#region Mặc định các thông tin
                ////Lấy Kho mặc định
                //if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //			reportModel.ListConfigArm = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == reportModel.WareHouseCode);
                //		}
                //	}
                //}

                //#endregion

                reportModel.EventTypeList = eventService.GetAllEventType();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                int cardSerial = -1;
                decimal workOrder = -1;
                if (reportModel.CardSerial != null) cardSerial = (int)reportModel.CardSerial;
                if (reportModel.WorkOrder != null) workOrder = (decimal)reportModel.WorkOrder;

                reportModel.DataTable = reportService.HistoryDataLogArm(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, workOrder, cardSerial);

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult HistoryExport(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
            reportModel.ListConfigArm = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == reportModel.WareHouseCode);
            //reportModel.ListConfigArm = configarmService.GetAllConfigArmOrderByName();
            // reportModel.ListCommand = commandService.GetAllCommandOrderByName();
            //if (reportModel.Id != null)
            //{
            //    var warehousecode = warehouseService.FindWareHouseById((int)(reportModel.Id)).WareHouseCode;
            //    if (reportModel.CommandID != null)
            //    {
            //        var workorder = commandService.FindCommandById((int)(reportModel.CommandID)).WorkOrder;

            //        var armno = configarmService.GetAllConfigArm().Where(x => x.WareHouseCode == warehousecode && x.TypeExport == 0 || x.TypeExport == 1).ToList();
            //        DateTime? startDate = null;
            //        DateTime? endDate = null;

            //        reportModel.EventTypeList = eventService.GetAllEventType();
            //        if (!string.IsNullOrEmpty(reportModel.StartDate))
            //        {
            //            startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            //        }
            //        if (!string.IsNullOrEmpty(reportModel.EndDate))
            //        {
            //            endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            //        }
            //        foreach (var item in armno)
            //        {
            //            var obj = reportService.DataLogArm(startDate, endDate, warehousecode, item.ArmNo, workorder);
            //            if (obj.Rows.Count > 0)
            //            {
            //                reportModel.DataTable = obj;
            //            }
            //        }
            //    }

            //}

            int cardSerial = -1;
            decimal workOrder = -1;
            if (reportModel.CardSerial != null) cardSerial = (int)reportModel.CardSerial;
            if (reportModel.WorkOrder != null) workOrder = (decimal)reportModel.WorkOrder;
            DateTime? startDate = null;
            DateTime? endDate = null;
            startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            reportModel.DataTable = reportService.HistoryDataLogArm(startDate, endDate, reportModel.WareHouseCode, reportModel.ArmNo, workOrder, cardSerial);
            return View(reportModel);
        }

        [HttpGet]
        public ActionResult BalanceTank(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT2);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();

                //#region Mặc định các thông tin
                ////Lấy Kho mặc định
                //if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var item in reportModel.ListWareHouse) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.WareHouseCode == null)
                //		{
                //			reportModel.WareHouseCode = item.WareHouseCode;
                //		}
                //	}
                //}

                //if (reportModel.ProductId == null)
                //{
                //	foreach (var item in reportModel.ListProduct) //Lấy mã kho mặc định
                //	{
                //		if (reportModel.ProductId == null)
                //		{
                //			reportModel.ProductId = item.ProductId;
                //			break;
                //		}
                //	}
                //}
                //#endregion

                DateTime? startDate = null;
                DateTime? endDate = null;

                reportModel.EventTypeList = eventService.GetAllEventType();

                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                reportModel.DataTable = reportService.InputExportExist_DataTankLog(startDate, endDate, reportModel.WareHouseCode, reportModel.ProductId);
                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult BalanceTank(ReportModel reportModel)
        {
            reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();

            DateTime? startDate = null;
            DateTime? endDate = null;

            reportModel.EventTypeList = eventService.GetAllEventType();
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("BalanceTank", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/BalanceTank.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.InputExportExist_DataTankLog(startDate, endDate, reportModel.WareHouseCode, reportModel.ProductId);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);


            reportModel.StartDate = Convert.ToDateTime(startDate).ToString(Constants.DATE_FORMAT);
            reportModel.EndDate = Convert.ToDateTime(endDate).ToString(Constants.DATE_FORMAT);


            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_CANBANGLUONGXUAT_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_CANBANGLUONGXUAT_");
            }


            //return View(reportModel);
        }

        public ActionResult IOTank(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REPORT2);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.TankList = tankService.GetAllTankOrderByName();
                reportModel.TankGroupList = tankGroupService.GetAllTankGrpOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();

                if (reportModel.TankGroupId != null)
                {
                    var obj = tankGroupService.FindTankById((int)(reportModel.TankGroupId));
                    var TankGroupname = obj.TanktGrpName;
                }

                int pageNumber = (page ?? 1);

                DateTime? startDate = null;
                DateTime? endDate = null;

                reportModel.EventTypeList = eventService.GetAllEventType();

                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                reportModel.DataTable = reportService.IOData(startDate, endDate, reportModel.TankId, reportModel.TankGroupId, reportModel.ProductId, reportModel.WareHouseCode);
                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult IOTank(ReportModel reportModel)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("IOTank", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/IOTankReport.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.IOData(startDate, endDate, reportModel.TankId, reportModel.TankGroupId, reportModel.ProductId, reportModel.WareHouseCode);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);

            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_NHAPXUATTON_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_NHAPXUATTON_");
            }
        }

        public ActionResult Event(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_EVENT);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);

                DateTime? startDate = null;
                DateTime? endDate = null;

                //  reportModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                reportModel.EventTypeList = eventService.GetAllEventType();

                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                reportModel.EventList = eventService.GetEventByTime(startDate, endDate, reportModel.User, reportModel.EventTypeId)
                .ToPagedList(pageNumber, Constants.PAGE_SIZE);

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Event(ReportModel reportModel)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            //reportModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());

            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("Event", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/EventReport.rdlc");
            localReport.EnableExternalImages = true;


            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.EventData(startDate, endDate, reportModel.User, reportModel.EventTypeId, out startDate, out endDate, reportModel.WareHouseCode);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);

            reportModel.StartDate = Convert.ToDateTime(startDate).ToString(Constants.DATE_FORMAT);
            reportModel.EndDate = Convert.ToDateTime(endDate).ToString(Constants.DATE_FORMAT);

            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4Excel(localReport, "BAOCAO_SUKIEN_");
            }
            else
            {
                return A4PDF(localReport, "BAOCAO_SUKIEN_");
            }

        }

        public ActionResult Error(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_ERROR);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);

                DateTime? startDate = null;
                DateTime? endDate = null;
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();

                reportModel.TankList = tankService.GetAllTankOrderByName();
                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (reportModel.Id != null)
                {
                    reportModel.WareHouseCode = warehouseService.FindWareHouseById((int)(reportModel.Id)).WareHouseCode;
                }
                reportModel.AlarmList = alarmService.GetAlarmByTime(startDate, endDate, reportModel.TankId, Constants.ALARM_TYPE_ERROR, reportModel.WareHouseCode)
                   .ToPagedList(pageNumber, Constants.PAGE_SIZE);
                if (reportModel.TankId != null)
                {
                    var objtank = tankService.GetAllTank().Where(x => x.TankId == reportModel.TankId).ToList();
                    foreach (var item in objtank)
                    {
                        reportModel.TankName = item.TankName;
                    }
                }
                var objalarm = alarmService.GetAlarmByTime(startDate, endDate, reportModel.TankId, Constants.ALARM_TYPE_ERROR, reportModel.WareHouseCode).ToList();

                foreach (var items in objalarm)
                {
                    reportModel.TypeId = items.TypeId;
                    var objwarehouse = warehouseService.GetAllWareHouse().Where(x => x.WareHouseCode == items.WareHouseCode).ToList();
                    foreach (var it in objwarehouse)
                    {
                        reportModel.WareHouseName = it.WareHouseName;

                    }
                    reportModel.TypeName = alarmService.GetAlarmTypeById((int)(reportModel.TypeId)).TypeName;
                }

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Error(ReportModel reportModel)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (reportModel.Id != null)
            {
                reportModel.WareHouseCode = warehouseService.FindWareHouseById((int)(reportModel.Id)).WareHouseCode;
            }
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("Error", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/ErrorReport.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.ErrorData(startDate, endDate, reportModel.TankId, out startDate, out endDate, reportModel.WareHouseCode);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);

            reportModel.StartDate = Convert.ToDateTime(startDate).ToString(Constants.DATE_FORMAT);
            reportModel.EndDate = Convert.ToDateTime(endDate).ToString(Constants.DATE_FORMAT);

            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4Excel(localReport, "BAOCAO_LOI_");
            }
            else
            {
                return A4PDF(localReport, "BAOCAO_LOI_");
            }

        }

        public ActionResult Warning(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_WARNING);
            if (checkPermission)
            {

                int pageNumber = (page ?? 1);

                DateTime? startDate = null;
                DateTime? endDate = null;
                reportModel.TankList = tankService.GetAllTankOrderByName();
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.AlarmTypeList = alarmService.GetAllAlarmType().Where(al => al.TypeId != Constants.ALARM_TYPE_ERROR);

                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (reportModel.Id != null)
                {
                    reportModel.WareHouseCode = warehouseService.FindWareHouseById((int)(reportModel.Id)).WareHouseCode;
                }
                reportModel.AlarmList = alarmService.GetAlarmByTime(startDate, endDate, reportModel.TankId, reportModel.AlarmTypeId, reportModel.WareHouseCode)
                    .Where(a => a.TypeId != 1).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                if (reportModel.TankId != null)
                {
                    var objtank = tankService.GetAllTank().Where(x => x.TankId == reportModel.TankId).ToList();
                    foreach (var item in objtank)
                    {
                        reportModel.TankName = item.TankName;
                    }
                }



                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Warning(ReportModel reportModel)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;


            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("Warning", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (reportModel.Id != null)
            {
                reportModel.WareHouseCode = warehouseService.FindWareHouseById((int)(reportModel.Id)).WareHouseCode;
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/WarningReport.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.WarningData(startDate, endDate, reportModel.TankId, reportModel.AlarmTypeId, out startDate, out endDate, reportModel.WareHouseCode);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);


            reportModel.StartDate = Convert.ToDateTime(startDate).ToString(Constants.DATE_FORMAT);
            reportModel.EndDate = Convert.ToDateTime(endDate).ToString(Constants.DATE_FORMAT);


            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4Excel(localReport, "BAOCAO_CANHBAO_");
            }
            else
            {
                return A4PDF(localReport, "BAOCAO_CANHBAO_");
            }
        }

        public ActionResult TankManual(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_TANKEXPORT);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);

                DateTime? startDate = null;
                DateTime? endDate = null;

                reportModel.ListWareHouse = warehouseService.GetAllWareHouse();
                //if (reportModel.WareHouseCode == null)
                //{
                //	foreach (var it in reportModel.ListWareHouse)
                //	{
                //		reportModel.WareHouseCode = it.WareHouseCode;
                //		break;
                //	}
                //}
                if (reportModel.WareHouseCode == null)
                {
                    reportModel.WareHouseCode = 0;
                }

                reportModel.TankList = tankService.GetAllTank().Where(x => x.WareHouseCode == reportModel.WareHouseCode);
                // reportModel.TankList = tankService.GetAllTankOrderByName();

                if (!string.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                reportModel.ListTankManual = tankService.GetTankManualByTime(reportModel.TankId, startDate, endDate).Where(x => x.WareHouseCode == reportModel.WareHouseCode);

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult TankManual(ReportModel reportModel)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("TankManual", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/TankManual.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.TankManualData(startDate, endDate, reportModel.TankId, out startDate, out endDate, reportModel.WareHouseCode);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);


            reportModel.StartDate = Convert.ToDateTime(startDate).ToString(Constants.DATE_FORMAT);
            reportModel.EndDate = Convert.ToDateTime(endDate).ToString(Constants.DATE_FORMAT);


            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_DOTAY_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_DOTAY_BE_");
            }
        }

        public ActionResult ReceiptProduct(ReportModel reportModel, int? page)
        {
            int pageNumber = (page ?? 1);

            DateTime? startDate = null;
            DateTime? endDate = null;
            reportModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            reportModel.TankList = tankService.GetAllTankOrderByName().Where(x => x.WareHouseCode == reportModel.WareHouseCode);
            //reportModel.TankList = tankService.GetAllTankOrderByName();

            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            reportModel.DataTable = reportService.ReceiptProductData(startDate, endDate, reportModel.TankId, reportModel.WareHouseCode);

            return View(reportModel);
        }

        [HttpPost]
        public ActionResult ReceiptProduct(ReportModel reportModel)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            reportModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("ReceiptProduct", reportModel);
            }
            if (!string.IsNullOrEmpty(reportModel.StartDate))
            {
                startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(reportModel.EndDate))
            {
                endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/ReceiptProduct.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.ReceiptProductData(startDate, endDate, reportModel.TankId, reportModel.WareHouseCode);
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);

            localReport.SetParameters(ReportParam(reportModel));

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_KIEMKE_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_KIEMKE_");
            }
        }

        public ActionResult TankImport(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_TANKIMPORT);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);

                DateTime? startDate = null;
                DateTime? endDate = null;
                //reportModel.WareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                if (reportModel.WareHouseCode == null)
                {
                    reportModel.WareHouseCode = reportModel.ListWareHouse.First().WareHouseCode;
                }
                if (!String.IsNullOrEmpty(reportModel.StartDate))
                {
                    startDate = DateTime.ParseExact(reportModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!String.IsNullOrEmpty(reportModel.EndDate))
                {
                    endDate = DateTime.ParseExact(reportModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                reportModel.ImportInfoList = importService.GetImportInfoByTime((byte)reportModel.WareHouseCode, startDate, endDate)
                    .ToPagedList(pageNumber, Constants.PAGE_SIZE);

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult TankImportInfo(int id)
        {
            reportModel.ImportId = id;
            reportModel.ImportInfo = importService.GetImportInfo(id);
            reportModel.TankImportData = reportService.TankImportData(id);
            reportModel.ClockExportData = reportService.ClockExportData(id);
            reportModel.TankImportInfoData = reportService.TankImportInfoData(id);
            reportModel.ExportArmImportData = reportService.ExportArmImportData(id);


            return View(reportModel);
        }


        [HttpPost]
        public ActionResult TankImportInfo(ReportModel reportModel)
        {
            var startSum = 0.0;
            var endSum = 0.0;
            var startSumVtt = 0.0;
            var endSumVtt = 0.0;
            var endSumV15 = 0.0;

            reportModel.TankImportInfoData = reportService.TankImportInfoData(reportModel.Id);
            reportModel.ExportArmImportData = reportService.ExportArmImportData(reportModel.Id ?? 0);
            reportModel.TankImportData = reportService.TankImportData(reportModel.Id ?? 0);

            foreach (System.Data.DataRow row in reportModel.TankImportData.Rows)
            {
                if (@Convert.ToInt32(row["TankId"]) != 9999) //bo qua Tec CC
                {
                    startSum += ((row["StartTemperature"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["StartTemperature"])) * ((row["StartProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["StartProductVolume"]));
                    endSum += ((row["EndTemperature"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndTemperature"])) * ((row["EndProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndProductVolume"]));
                    startSumVtt += (row["StartProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["StartProductVolume"]);
                    endSumVtt += (row["EndProductVolume"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndProductVolume"]);
                    endSumV15 += (row["EndProductVolume15"] == @DBNull.Value) ? 0 : @Convert.ToDouble(row["EndProductVolume15"]);
                }
            }

            double sumStartVTT = 0.0, sumEndVTT = 0.0, sumStartV15 = 0.0, sumEndV15 = 0.0, sumStartTemp = 0.0, sumEndTemp = 0.0, sumEndVCF = 0.0;
            double.TryParse(reportModel.TankImportData.Compute("Sum(StartProductVolume)", "").ToString(), out sumStartVTT);
            double.TryParse(reportModel.TankImportData.Compute("Sum(EndProductVolume)", "").ToString(), out sumEndVTT);
            double.TryParse(reportModel.TankImportData.Compute("Sum(StartProductVolume15)", "").ToString(), out sumStartV15);
            double.TryParse(reportModel.TankImportData.Compute("Sum(EndProductVolume15)", "").ToString(), out sumEndV15);
            double.TryParse(reportModel.TankImportData.Compute("Sum(EndVCF)", "").ToString(), out sumEndVCF);

            if (sumStartVTT != 0)
            {
                sumStartTemp = startSum / startSumVtt;
            }

            if (sumEndVTT != 0)
            {
                sumEndTemp = endSum / endSumVtt;
            }

            double sumExportV = 0.0, sumExportV15 = 0.0;
            double.TryParse(reportModel.ExportArmImportData.Compute("Sum(ExportValue)", "").ToString(), out sumExportV);
            double.TryParse(reportModel.ExportArmImportData.Compute("Sum(ExportValue15)", "").ToString(), out sumExportV15);

            var chenhlech = sumEndTemp - sumStartTemp;
            var vtung = (chenhlech * 0.0009 * sumStartVTT);

            var ketluanVtt = sumEndVTT + sumExportV - sumStartVTT;
            var ketluanV15 = sumEndV15 + sumExportV15 - sumStartV15;

            if (vtung < 0)
            {
                vtung = vtung * -1;
            }

            List<ReportParameter> rptParam = ReportParam(reportModel);
            rptParam.Add(new ReportParameter("ketluanVtt", Math.Round(ketluanVtt).ToString("#,##0")));
            rptParam.Add(new ReportParameter("ketluanV15", Math.Round(ketluanV15).ToString("#,##0")));
            rptParam.Add(new ReportParameter("sumExportV", Math.Round(sumExportV).ToString("#,##0")));
            rptParam.Add(new ReportParameter("sumExportV15", Math.Round(sumExportV15).ToString("#,##0")));
            rptParam.Add(new ReportParameter("vtung", Math.Round(vtung).ToString("#,##0")));
            rptParam.Add(new ReportParameter("chenhlech", Math.Round(chenhlech, 2).ToString("#,##0")));
            //rptParam.Add(new ReportParameter("updateDate", importInfo.UpdateDate.ToString("dd/MM/yyyy HH:mm:ss")));



            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/TankImport.rdlc");
            localReport.EnableExternalImages = true;

            ReportDataSource rds = new ReportDataSource();
            rds.Value = reportService.TankImportData(reportModel.ImportId);
            rds.Name = "DataSet1";

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Value = reportService.ExportArmImportData(reportModel.ImportId ?? 0);
            rds2.Name = "DataSet2";

            ReportDataSource rds3 = new ReportDataSource();
            rds3.Value = reportService.TankImportInfoData(reportModel.ImportId);
            rds3.Name = "DataSet3";

            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);
            localReport.DataSources.Add(rds2);
            localReport.DataSources.Add(rds3);

            reportModel.StartDate = DateTime.Now.ToString(Constants.DATE_FORMAT);
            localReport.SetParameters(rptParam);

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_NHAPHANG_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_NHAPHANG_");
            }
        }

        public ActionResult WarehouseCard(ReportModel reportModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_REPORT_WAREHOUSECARD);
            if (checkPermission)
            {
                reportModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();
                reportModel.ListProduct = productService.GetAllProductOrderByName();


                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(reportModel.Month))
                {
                    var date = DateTime.ParseExact(reportModel.Month, Constants.DATE_MONTH_FORMAT, CultureInfo.InvariantCulture);
                    startDate = new DateTime(date.Year, date.Month, 1);
                    endDate = startDate.Value.AddMonths(1).AddDays(-1);
                }
                reportModel.ListTankLog = reportService.WarehouseCard_Tank_Inventory(startDate, endDate, reportModel.WareHouseCode, reportModel.ProductId);
                reportModel.WarehouseCard = reportService.WarehouseCard(startDate, endDate, reportModel.WareHouseCode, reportModel.ProductId);

                return View(reportModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult WarehouseCard(ReportModel reportModel)
        {
            if (string.IsNullOrEmpty(reportModel.FileType))
            {
                return RedirectToAction("WarehouseCard", reportModel);
            }

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Views/Report/WarehouseCard.rdlc");
            localReport.EnableExternalImages = true;

            DateTime? startDate = null;
            DateTime? endDate = null;

            String month = String.Empty, year = String.Empty;



            if (!string.IsNullOrEmpty(reportModel.Month))
            {
                var date = DateTime.ParseExact(reportModel.Month, Constants.DATE_MONTH_FORMAT, CultureInfo.InvariantCulture);
                month = date.Month.ToString();
                year = date.Year.ToString();
                startDate = new DateTime(date.Year, date.Month, 1);
                reportModel.StartDate = startDate.ToString();
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
                reportModel.EndDate = endDate.ToString();
            }

            ReportDataSource rds = new ReportDataSource();
            reportModel.WarehouseCard = reportService.WarehouseCard(startDate, endDate, reportModel.WareHouseCode, reportModel.ProductId);
            rds.Value = reportModel.WarehouseCard.DT;
            rds.Name = "DataSet1";
            localReport.DataSources.Clear();
            localReport.DataSources.Add(rds);

            List<ReportParameter> rptParam = ReportParam(reportModel);
            rptParam.Add(new ReportParameter("productName", productService.FindProductById(reportModel.ProductId.Value).ProductName));
            rptParam.Add(new ReportParameter("month", month));
            rptParam.Add(new ReportParameter("year", year));
            rptParam.Add(new ReportParameter("totalVtt", reportModel.WarehouseCard.TotalVtt.ToString()));
            rptParam.Add(new ReportParameter("totalV15", reportModel.WarehouseCard.TotalV15.ToString()));
            rptParam.Add(new ReportParameter("avgVtt", reportModel.WarehouseCard.AvgTotalVtt.ToString()));
            rptParam.Add(new ReportParameter("avgV15", reportModel.WarehouseCard.AvgTotalV15.ToString()));

            if (reportModel.WarehouseCard.LastDensity == null)
            {
                rptParam.Add(new ReportParameter("lastDensity"));
            }
            else
            {
                rptParam.Add(new ReportParameter("lastDensity", reportModel.WarehouseCard.LastDensity.ToString()));
            }
            if (reportModel.WarehouseCard.LastVCF == null)
            {
                rptParam.Add(new ReportParameter("lastVCF"));
            }
            else
            {
                rptParam.Add(new ReportParameter("lastVCF", reportModel.WarehouseCard.LastVCF.ToString()));
            }
            if (reportModel.WarehouseCard.LastOutV15 == null)
            {
                rptParam.Add(new ReportParameter("lastOutV15"));
            }
            else
            {
                rptParam.Add(new ReportParameter("lastOutV15", reportModel.WarehouseCard.LastOutV15.ToString()));
            }
            if (reportModel.WarehouseCard.LastDeviation == null)
            {
                rptParam.Add(new ReportParameter("lastDeviation"));
            }
            else
            {
                rptParam.Add(new ReportParameter("lastDeviation", reportModel.WarehouseCard.LastDeviation.ToString()));
            }
            rptParam.Add(new ReportParameter("lastTotalVtt", reportModel.WarehouseCard.LastTotalVtt.ToString()));
            rptParam.Add(new ReportParameter("lastTotalV15", reportModel.WarehouseCard.LastTotalV15.ToString()));

            rptParam.Add(new ReportParameter("storeWastageRate", reportModel.WarehouseCard.StoreWastageRate.ToString()));
            if (reportModel.WarehouseCard.StoreWastageRateVtt == null)
            {
                rptParam.Add(new ReportParameter("storeWastageRateVtt"));
            }
            else
            {
                rptParam.Add(new ReportParameter("storeWastageRateVtt", reportModel.WarehouseCard.StoreWastageRateVtt.ToString()));
            }
            rptParam.Add(new ReportParameter("storeWastageRateV15", reportModel.WarehouseCard.StoreWastageRateV15.ToString()));
            localReport.SetParameters(rptParam);

            if (reportModel.FileType == "EXCEL")
            {
                return A4HorizontalExcel(localReport, "BAOCAO_THEKHO_");
            }
            else
            {
                return A4HorizontalPDF(localReport, "BAOCAO_THEKHO_");
            }
        }

        private List<ReportParameter> ReportParam(ReportModel reportModel)
        {
            List<ReportParameter> rptParam = new List<ReportParameter>();
            rptParam.Add(new ReportParameter("compName", configurationService.GetConfiguration(Constants.CONFIG_COMP_NAME).Value));
            rptParam.Add(new ReportParameter("unit", configurationService.GetConfiguration(Constants.CONFIG_UNIT).Value));
            rptParam.Add(new ReportParameter("fromDate", reportModel.StartDate));
            rptParam.Add(new ReportParameter("toDate", reportModel.EndDate));
            if (!string.IsNullOrEmpty(reportModel.TankName))
            {
                rptParam.Add(new ReportParameter("tankName", reportModel.TankName));
            }
            rptParam.Add(new ReportParameter("logo", new Uri(Server.MapPath(configurationService.GetConfiguration(Constants.CONFIG_LOGO).Value)).AbsoluteUri));
            return rptParam;
        }

        private FileContentResult A4Excel(LocalReport lr, string fileName)
        {
            byte[] renderedBytes;
            string mimeType;
            string encoding;
            string fileNameExtension;
            Warning[] warnings;
            string[] streams;
            string format = "EXCEL";
            string deviceInfo =
             "<DeviceInfo>" +
             "  <OutputFormat>" + format + "</OutputFormat>" +
             "  <PageWidth>" + "21cm" + "</PageWidth>" +
             "  <PageHeight>" + "29.7cm" + "</PageHeight>" +
             "  <MarginTop>" + "0cm" + "</MarginTop>" +
             "  <MarginLeft>" + "0cm" + "</MarginLeft>" +
             "  <MarginRight>" + "0cm" + "</MarginRight>" +
             "  <MarginBottom>" + "0cm" + "</MarginBottom>" +
             "</DeviceInfo>";

            renderedBytes = lr.Render(
                 format,
                 deviceInfo,
                 out mimeType,
                 out encoding,
                 out fileNameExtension,
                 out streams,
                 out warnings);

            var filename = string.Format(fileName + DateTime.Now.ToString(Constants.DATE_FORMAT) + ".{0}", fileNameExtension);
            Response.AddHeader("Content-Disposition", "inline; filename=" + filename);
            return File(renderedBytes, mimeType);
        }

        private FileContentResult A4HorizontalExcel(LocalReport lr, string fileName)
        {
            byte[] renderedBytes;
            string mimeType;
            string encoding;
            string fileNameExtension;
            Warning[] warnings;
            string[] streams;
            string format = "EXCEL";
            string deviceInfo =
             "<DeviceInfo>" +
             "  <OutputFormat>" + format + "</OutputFormat>" +
             "  <PageWidth>" + "29.7cm" + "</PageWidth>" +
             "  <PageHeight>" + "21cm" + "</PageHeight>" +
             "  <MarginTop>" + "0cm" + "</MarginTop>" +
             "  <MarginLeft>" + "0cm" + "</MarginLeft>" +
             "  <MarginRight>" + "0cm" + "</MarginRight>" +
             "  <MarginBottom>" + "0cm" + "</MarginBottom>" +
             "</DeviceInfo>";

            renderedBytes = lr.Render(
                 format,
                 deviceInfo,
                 out mimeType,
                 out encoding,
                 out fileNameExtension,
                 out streams,
                 out warnings);

            var filename = string.Format(fileName + DateTime.Now.ToString(Constants.DATE_FORMAT) + ".{0}", fileNameExtension);
            Response.AddHeader("Content-Disposition", "inline; filename=" + filename);
            return File(renderedBytes, mimeType);
        }

        private FileContentResult A4PDF(LocalReport lr, string fileName)
        {
            byte[] renderedBytes;
            string mimeType;
            string encoding;
            string fileNameExtension;
            Warning[] warnings;
            string[] streams;
            string format = "PDF";
            string deviceInfo =
             "<DeviceInfo>" +
             "  <OutputFormat>" + format + "</OutputFormat>" +
             "  <PageWidth>" + "21cm" + "</PageWidth>" +
             "  <PageHeight>" + "29.7cm" + "</PageHeight>" +
             "  <MarginTop>" + "0cm" + "</MarginTop>" +
             "  <MarginLeft>" + "0cm" + "</MarginLeft>" +
             "  <MarginRight>" + "0cm" + "</MarginRight>" +
             "  <MarginBottom>" + "0cm" + "</MarginBottom>" +
             "</DeviceInfo>";

            renderedBytes = lr.Render(
                 format,
                 deviceInfo,
                 out mimeType,
                 out encoding,
                 out fileNameExtension,
                 out streams,
                 out warnings);

            var filename = string.Format(fileName + DateTime.Now.ToString(Constants.DATE_FORMAT) + ".{0}", fileNameExtension);
            Response.AddHeader("Content-Disposition", "inline; filename=" + filename);
            return File(renderedBytes, mimeType);
        }

        private FileContentResult A4HorizontalPDF(LocalReport lr, string fileName)
        {
            byte[] renderedBytes;
            string mimeType;
            string encoding;
            string fileNameExtension;
            Warning[] warnings;
            string[] streams;
            string format = "PDF";
            string deviceInfo =
             "<DeviceInfo>" +
             "  <OutputFormat>" + format + "</OutputFormat>" +
             "  <PageWidth>" + "21cm" + "</PageWidth>" +
             "  <PageHeight>" + "29.7cm" + "</PageHeight>" +
             "  <MarginTop>" + "0.5cm" + "</MarginTop>" +
             "  <MarginLeft>" + "0cm" + "</MarginLeft>" +
             "  <MarginRight>" + "0cm" + "</MarginRight>" +
             "  <MarginBottom>" + "0.5cm" + "</MarginBottom>" +
             "</DeviceInfo>";

            renderedBytes = lr.Render(
                 format,
                 deviceInfo,
                 out mimeType,
                 out encoding,
                 out fileNameExtension,
                 out streams,
                 out warnings);

            var filename = string.Format(fileName + DateTime.Now.ToString(Constants.DATE_FORMAT) + ".{0}", fileNameExtension);
            Response.AddHeader("Content-Disposition", "inline; filename=" + filename);
            return File(renderedBytes, mimeType);
        }

        //public ActionResult InOut()
        //{
        //    reportModel.WareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
        //    return View(reportModel);
        //}

        public JsonResult GetConfigArm(byte wareHouseCode)
        {
            var lst = configarmService.GetAllConfigArmOrderByName().Where(x => x.WareHouseCode == wareHouseCode).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListTank(byte wareHouseCode)
        {
            var lst = tankService.GetAllTankOrderByName().Where(x => x.WareHouseCode == wareHouseCode)
            .Select(c => new TankTempModel()
            {
                ProductId = 0,
                TankId = c.TankId,
                TankName = c.TankName,
                WareHouseCode = c.WareHouseCode
            });
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


    }



}