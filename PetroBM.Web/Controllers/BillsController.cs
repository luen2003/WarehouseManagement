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
using System.Collections.Generic;
using System;
using log4net;


namespace PetroBM.Web.Controllers
{
    public class BillsController : BaseController
    {
        ILog log = log4net.LogManager.GetLogger(typeof(BillsController));
        private static bool flagTest = true;
        private readonly IUserService UserService;
        private readonly ICommandService CommandService;
        private readonly ICustomerService CustomerService;
        private readonly IPriceService PriceService;
        private readonly IProductService ProductService;
        private readonly ICommandDetailService CommandDetailService;
        private readonly ISealService SealService;
        private readonly IConfigurationService ConfigurationService;
        private readonly IDriverService DriverService;
        private readonly IVehicleService VehicleService;
        private readonly BaseService baseService;
        private readonly IWareHouseService WareHouseService;

        public CommandDetailModel CommandDetailModel;
        public InvoiceModel InvoiceModel;
        public IInvoiceDetailService InvoiceDetailService;
        public IInvoiceService InvoiceService;

        public BillsController(ICommandService CommandService, CommandModel commandModel, CommandDetailModel CommandDetailModel,
           IConfigurationService ConfigurationService, IUserService UserService,
           IProductService ProductService, IWareHouseService WareHouseService,
           ICustomerService CustomerService, IVehicleService VehicleService,
           InvoiceModel InvoiceModel, ICommandDetailService CommandDetailService,
           IPriceService PriceService, IInvoiceDetailService InvoiceDetailService,
           IInvoiceService InvoiceService, ISealService SealService,
           IDriverService DriverService, IWareHouseService warehouseService,
           BaseService baseService)
        {
            this.InvoiceModel = InvoiceModel;
            this.ConfigurationService = ConfigurationService;
            this.UserService = UserService;
            this.CommandService = CommandService;
            this.CommandDetailService = CommandDetailService;
            this.CustomerService = CustomerService;
            this.PriceService = PriceService;
            this.InvoiceService = InvoiceService;
            this.InvoiceDetailService = InvoiceDetailService;
            this.ProductService = ProductService;
            this.SealService = SealService;
            this.DriverService = DriverService;
            this.baseService = baseService;
            this.VehicleService = VehicleService;
            this.WareHouseService = warehouseService;
        }


        // GET: Bills

        public ActionResult ViewBills()
        {

            return View();
        }

        public ActionResult Index(InvoiceModel invoiceModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERINVOICE);
            if (checkPermission)
            {
                DateTime? startDate;
                DateTime? endDate;
                if (invoiceModel.VerhicleNumber == null)
                    invoiceModel.VerhicleNumber = "";

                startDate = DateTime.ParseExact(invoiceModel.StartDate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
                endDate = DateTime.ParseExact(invoiceModel.EndDate, Constants.DATE_FORMAT, CultureInfo.CurrentCulture);
                InvoiceModel.ListEnumInvoice = InvoiceService.GetAllInvoice().Where(x => x.WareHousecode == byte.Parse(Session[Constants.Session_WareHouse].ToString())
                && (x.WorkOrder == invoiceModel.WorkOrder || invoiceModel.WorkOrder == null)
                && (x.CardSerial == invoiceModel.CardSerial || invoiceModel.CardSerial == null)
                && (x.InsertDate >= startDate)
                && (x.InsertDate <= endDate)
                && (x.VehicleNumber.Contains(invoiceModel.VerhicleNumber) || invoiceModel.VerhicleNumber == "")
                );
                return View(InvoiceModel);
            }
            else
            {
                log.Info("Stop controller Bills HttpGet CreateInvoice");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpGet]
        public ActionResult CreateInvoice()
        {
            log.Info("Start controller Bills HttpGet CreateInvoice");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERINVOICE);
            if (checkPermission)
            {
                InvoiceModel.ListInvoice = new List<Invoice>();
                InvoiceModel.Invoice.OutTime = DateTime.Now;
                InvoiceModel.Invoice.TaxRate = 10;
                InvoiceModel.ExportMode = Int32.Parse(ConfigurationService.GetConfiguration(Constants.CONFIG_EXPORT_MODE).Value);
                //Lấy tất cả các mặt hàng
                InvoiceModel.ListProduct = ProductService.GetAllProduct().ToList();
                var lstPrice = PriceService.GetAllPrice();

                InvoiceModel.ListProductTemp = ProductService.GetAllProduct().Select(x => new ProductTemp 
                {
                    Abbreviations = x.Abbreviations,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName
                }).ToList();

                InvoiceModel.ListPrice = new List<DataValue>();

                foreach (var item in InvoiceModel.ListProduct)  //Hàng hóa 
                {

                    //    //Lit
                    var obj = lstPrice.Where(x => x.Abbreviations == item.Abbreviations && x.Unit == "Ltt").OrderByDescending(x => x.InsertDate).Take(1).FirstOrDefault(); //Max(x => x.InsertDate);        
                    if (obj != null)
                    {
                        InvoiceModel.ListPrice.Add(new DataValue("M1", obj.AreaPrice1, obj.ProductCode.Trim(), obj.Unit.Trim(), "M1", obj.EnvironmentTax));
                        InvoiceModel.ListPrice.Add(new DataValue("M2", obj.AreaPrice2, obj.ProductCode.Trim(), obj.Unit.Trim(), "M2", obj.EnvironmentTax));
                    }
                    //    //Lit15
                    obj = lstPrice.Where(x => x.Abbreviations == item.Abbreviations && x.Unit == "L15").OrderByDescending(x => x.InsertDate).Take(1).FirstOrDefault(); //Max(x => x.InsertDate);        
                    if (obj != null)
                    {
                        InvoiceModel.ListPrice.Add(new DataValue("M1", obj.AreaPrice1, obj.ProductCode.Trim(), obj.Unit.Trim(), "M1", obj.EnvironmentTax));
                        InvoiceModel.ListPrice.Add(new DataValue("M2", obj.AreaPrice2, obj.ProductCode.Trim(), obj.Unit.Trim(), "M2", obj.EnvironmentTax));
                    }
                    //    //Kg

                    obj = lstPrice.Where(x => x.Abbreviations == item.Abbreviations && x.Unit == "Kg").OrderByDescending(x => x.InsertDate).Take(1).FirstOrDefault(); //Max(x => x.InsertDate);        
                    if (obj != null)
                    {
                        InvoiceModel.ListPrice.Add(new DataValue("M1", obj.AreaPrice1, obj.ProductCode.Trim(), obj.Unit.Trim(), "M1", obj.EnvironmentTax));
                        InvoiceModel.ListPrice.Add(new DataValue("M2", obj.AreaPrice2, obj.ProductCode.Trim(), obj.Unit.Trim(), "M2", obj.EnvironmentTax));

                    }
                }
                InvoiceModel.WareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                InvoiceModel.WareHouseName = Session[Constants.Session_WareHouseName].ToString();
                log.Info("Finish controller Bills HttpGet CreateWareHouse");
                return View(InvoiceModel);
            }
            else
            {
                log.Info("Stop controller Bills HttpGet CreateInvoice");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }


        [HttpGet]
        public ActionResult PrintInvoice(int InvoiceID)
        {
            log.Info("Start controller Bills HttpGet PrintInvoice");

            InvoiceModel.Invoice = InvoiceService.GetAllInvoice().Where(x => x.InvoiceID == InvoiceID).FirstOrDefault();
            InvoiceModel.ListInvoiceDetail = InvoiceDetailService.GetAllInvoiceDetailNoOrder().Where(x => x.InvoiceID == InvoiceID).ToList();
            InvoiceModel.ListCommandDetail = CommandDetailService.GetAllCommandDetailOrderByCompartmentOrder().Where(x => x.CommandID == InvoiceModel.Invoice.CommandID).OrderBy(x => x.ID).ToList();
            InvoiceModel.Command = CommandService.GetAllCommand().Where(x => x.CommandID == InvoiceModel.Invoice.CommandID).FirstOrDefault();
            InvoiceModel.ListSeal = SealService.GetAllSeal().Where(x => x.CommandID == InvoiceModel.Invoice.CommandID).OrderBy(x => x.ID).ToList();
           // InvoiceModel.ListDriver = DriverService.GetAllDriver().Where(x => x.DeleteFlg == false).ToList();
            InvoiceModel.ListProductTemp = ProductService.GetAllProduct().Select(x => new ProductTemp

            {
                Abbreviations = x.Abbreviations,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName
            }).ToList();

            InvoiceModel.OutTime = ((DateTime)InvoiceModel.Invoice.OutTime).ToString(Constants.DATE_FORMAT);
            InvoiceModel.InTime = ((DateTime)InvoiceModel.Invoice.InTime).ToString(Constants.DATE_FORMAT);

            log.Info("Finish controller Bills HttpGet PrintInvoice");
            return View(InvoiceModel);
        }
        [HttpPost]
        public ActionResult PrintInvoice()
        {
            return RedirectToAction("PrintInvoice");
        }


        [HttpGet]
        public ActionResult Delete(int InvoiceID)
        {
            log.Info("Start controller Bills HttpGet delete");

            var rs = InvoiceService.DeleteInvoice(InvoiceID);
            rs = InvoiceDetailService.DeleteInvoiceDetail_By_InvoiceID(InvoiceID);
            log.Info("Finish controller Bills HttpGet delete ");
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult CreateInvoice(InvoiceModel InvoiceModel, List<Invoice> ListInvoice)
        {
            try
            {
               if (flagTest == true)
               {
                flagTest = false;
                log.Info("Start controller Bills HttpPost CreateInvoice");
                InvoiceModel.Invoice.InsertUser = HttpContext.User.Identity.Name;
                InvoiceModel.Invoice.OutTime = DateTime.ParseExact(InvoiceModel.OutTime, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                InvoiceModel.Invoice.InTime = DateTime.ParseExact(InvoiceModel.InTime, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                InvoiceModel.Invoice.WareHousecode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                InvoiceModel.ListCommandDetail = CommandDetailService.GetAllCommandDetailOrderByCompartmentOrder().Where(x => x.CommandID == InvoiceModel.Invoice.CommandID).ToList();
                InvoiceModel.Command = CommandService.GetAllCommand().Where(x => x.CommandID == InvoiceModel.Invoice.CommandID).FirstOrDefault();
                InvoiceModel.Invoice.WorkOrder = InvoiceModel.Command.WorkOrder;
                var lastinvoice = InvoiceService.GetAllInvoice().OrderByDescending(x => x.InvoiceID).First();
                // Tránh việc click 2 tạo trùng invoice
                if (lastinvoice.WorkOrder != InvoiceModel.Invoice.WorkOrder)
                {
                    InvoiceService.CreateInvoice(InvoiceModel.Invoice);
                }
                InvoiceModel.ListProduct = ProductService.GetAllProduct().ToList();
                var lstPrice = PriceService.GetAllPrice();
                float avgVcf = 0;
                float avgDensity = 0;
                float avgTemp = 0;
                float totalV = 0;
                char[] arrListVolume;
                //var created = 0;
                InvoiceModel.ListPrice = new List<DataValue>();
                foreach (var item in InvoiceModel.ListProduct)  //Hàng hóa 
                {

                    //    //Lit
                    var obj = lstPrice.Where(x => x.Abbreviations == item.Abbreviations && x.Unit == "Ltt").OrderByDescending(x => x.InsertDate).Take(1).FirstOrDefault(); //Max(x => x.InsertDate);        
                    if (obj != null)
                    {
                        InvoiceModel.ListPrice.Add(new DataValue("M1", obj.AreaPrice1, obj.ProductCode.Trim(), obj.Unit.Trim(), "M1", obj.EnvironmentTax));
                        InvoiceModel.ListPrice.Add(new DataValue("M2", obj.AreaPrice2, obj.ProductCode.Trim(), obj.Unit.Trim(), "M2", obj.EnvironmentTax));
                    }
                    //    //Lit15
                    obj = lstPrice.Where(x => x.Abbreviations == item.Abbreviations && x.Unit == "L15").OrderByDescending(x => x.InsertDate).Take(1).FirstOrDefault(); //Max(x => x.InsertDate);        
                    if (obj != null)
                    {
                        InvoiceModel.ListPrice.Add(new DataValue("M1", obj.AreaPrice1, obj.ProductCode.Trim(), obj.Unit.Trim(), "M1", obj.EnvironmentTax));
                        InvoiceModel.ListPrice.Add(new DataValue("M2", obj.AreaPrice2, obj.ProductCode.Trim(), obj.Unit.Trim(), "M2", obj.EnvironmentTax));
                    }
                    //    //Kg

                    obj = lstPrice.Where(x => x.Abbreviations == item.Abbreviations && x.Unit == "Kg").OrderByDescending(x => x.InsertDate).Take(1).FirstOrDefault(); //Max(x => x.InsertDate);        
                    if (obj != null)
                    {
                        InvoiceModel.ListPrice.Add(new DataValue("M1", obj.AreaPrice1, obj.ProductCode.Trim(), obj.Unit.Trim(), "M1", obj.EnvironmentTax));
                        InvoiceModel.ListPrice.Add(new DataValue("M2", obj.AreaPrice2, obj.ProductCode.Trim(), obj.Unit.Trim(), "M2", obj.EnvironmentTax));

                    }
                }
                //Calculate Vcf, Density, Temperature vào InvoiceDetail

                // Get last invoice                

                //if (lastInvoice.InTime != InvoiceModel.Invoice.InTime && lastInvoice.CommandID == InvoiceModel.Invoice.CommandID)
                //{                
                //Get last invoice detail
                var lastInvoiceDetail = new MInvoiceDetail();            
                //lastInvoiceDetail = InvoiceDetailService.GetLastInvoiceDetail();
                //    int? lastExportNo = 1;
                //    //compare year of last invoicde detail with current year
                //    if ((DateTime.Now.Month == lastInvoiceDetail.OutTime.Value.Month) && (lastInvoiceDetail.ExportNo != null))
                //    {
                //        lastExportNo = lastInvoiceDetail.ExportNo + 1;
                //    }
                    // int InvoiceID = InvoiceService.GetNewId();               
                    int InvoiceID = InvoiceService.GetID(InvoiceModel.Invoice.WorkOrder);             
                    // Check InvoiceDetail to avoid duplicate rows when click create 2 times
                   var checkInvoiceDetail = InvoiceDetailService.GetAllInvoiceDetail().Where(x => x.InvoiceID == InvoiceID);
              
                    if (checkInvoiceDetail.Count() == 0 || lastinvoice.WorkOrder == InvoiceModel.Invoice.WorkOrder)
                    {
                    for (int i = 0; i < ListInvoice.Count(); i++)
                    {
                        for (var j = 0; j < InvoiceModel.ListPrice.Count; j++)
                        {
                            if (ListInvoice[i].ProductCode == InvoiceModel.ListPrice[j].Code && InvoiceModel.ListPrice[j].Name == InvoiceModel.Invoice.PriceLevel && InvoiceModel.ListPrice[j].Unit == ListInvoice[i].CalcUnit)
                            {

                                lastInvoiceDetail = InvoiceDetailService.GetLastInvoiceDetail();
                                if (lastInvoiceDetail.InvoiceID != InvoiceID || (lastInvoiceDetail.InvoiceID == InvoiceID && lastInvoiceDetail.ListVolume != ListInvoice[i].ListVolume))
                                {
                                     var invoiceDetail = new MInvoiceDetail();
                                     var lastInvoice = InvoiceService.GetAllInvoice().FirstOrDefault();
                                     var listCommandDetail = CommandDetailService.GetAllCommandDetailByCommandID(InvoiceModel.Invoice.CommandID ?? 0);
                                        //int lastID = lastInvoiceDetail.ID + 1;
                                     avgVcf = 0;
                                     avgDensity = 0;
                                     avgTemp = 0;
                                     if (InvoiceModel.EnvironmentTax == "Có")
                                     {
                                          invoiceDetail.EnvironmentTax = InvoiceModel.ListPrice[j].EnvironmentTax;
                                     }
                                     else
                                     {
                                            invoiceDetail.EnvironmentTax = 0;
                                     }
                                     arrListVolume = ListInvoice[i].ListVolume.ToCharArray();
                                     for (var k = 0; k < arrListVolume.Count(); k++)
                                     {
                                         for (var l = 0; l < InvoiceModel.ListCommandDetail.Count(); l++)
                                         {
                                                if (arrListVolume[k].ToString() == InvoiceModel.ListCommandDetail[l].CompartmentOrder.ToString())
                                                {
                                                    avgVcf += InvoiceModel.ListCommandDetail[l].V_Actual * InvoiceModel.ListCommandDetail[l].Vcf ?? 0;
                                                    avgTemp += InvoiceModel.ListCommandDetail[l].AvgTemperature * InvoiceModel.ListCommandDetail[l].V_Actual ?? 0;
                                                    avgDensity += InvoiceModel.ListCommandDetail[l].GasDensity * InvoiceModel.ListCommandDetail[l].V_Actual ?? 0;
                                                    totalV += InvoiceModel.ListCommandDetail[l].V_Actual ?? 0;
                                                    invoiceDetail.OutTime = InvoiceModel.ListCommandDetail[l].TimeStop;

                                                }
                                            }
                                      }
                                        invoiceDetail.AvgDensity = avgDensity / ListInvoice[i].Value;
                                        invoiceDetail.AvgVcf = avgVcf / ListInvoice[i].Value;
                                        invoiceDetail.AvgTemperature = avgTemp / ListInvoice[i].Value;
                                        invoiceDetail.InsertUser = InvoiceModel.Invoice.InsertUser;

                                        var amount = (ListInvoice[i].Value * ListInvoice[i].PriceUnit);
                                        invoiceDetail.InvoiceID = InvoiceID;
                                        invoiceDetail.ListVolume = ListInvoice[i].ListVolume;
                                        //Lưu thêm product code để tham chiếu khi cần
                                        for (var k = 0; k < InvoiceModel.ListProduct.Count; k++)
                                        {
                                            if (InvoiceModel.ListProduct[k].ProductName == ListInvoice[i].ProductName)
                                            {
                                                invoiceDetail.ProductCode = InvoiceModel.ListProduct[k].ProductCode;
                                            }
                                        }
                                        //invoiceDetail.ProductCode = ListInvoice[i].ProductCode;
                                        invoiceDetail.ProductName = ListInvoice[i].ProductName;
                                        invoiceDetail.CardData = InvoiceModel.Invoice.CardData;
                                        invoiceDetail.CardSerial = InvoiceModel.Invoice.CardSerial;
                                        invoiceDetail.Unit = ListInvoice[i].PriceUnit;
                                        invoiceDetail.Quantity = ListInvoice[i].Value;
                                        invoiceDetail.CountPrint = 0;
                                        invoiceDetail.OutTime = InvoiceModel.Invoice.OutTime;
                                        invoiceDetail.Amount = amount;
                                        //invoiceDetail.AvgVcf = 0;
                                        //invoiceDetail.AvgDensity = 0;
                                        //invoiceDetail.AvgTemperature = 0;
                                        invoiceDetail.Discount = ListInvoice[i].Discount;
                                        //Tính toán số phiếu xuất kho = số phiếu cuối + i
                                        int? lastExportNo = 1;
                                        //////compare year of last invoicde detail with current year
                                        if ((DateTime.Now.Month == lastInvoiceDetail.OutTime.Value.Month) && (lastInvoiceDetail.ExportNo != null))
                                        {
                                            lastExportNo = lastInvoiceDetail.ExportNo + 1;

                                        }
                                        invoiceDetail.ExportNo = lastExportNo;
                                        // invoiceDetail.ExportNo = lastExportNo + i;
                                        //Insert vào bảng chi tiết
                                        InvoiceDetailService.CreateInvoiceDetail(invoiceDetail);
                                        lastInvoiceDetail = InvoiceDetailService.GetLastInvoiceDetail();
                                        CommandDetailService.UpdateListCommandDetail_By_CommandID_InvoiceId_Flag((int)InvoiceModel.Invoice.CommandID, lastInvoiceDetail.ID, invoiceDetail.ProductCode, ListInvoice[i].ListVolume.ToString(), InvoiceModel.Invoice.InsertUser, Constants.Command_Flag_Invoice);
                                        //   CommandDetailService.UpdateListCommandDetail_By_CommandID_InvoiceId((int)InvoiceModel.Invoice.CommandID, lastInvoiceDetail.ID, invoiceDetail.ProductCode, ListInvoice[i].ListVolume.ToString(), InvoiceModel.Invoice.InsertUser);
                                        //    }
                                        //created = 1;
                                        //}                        
                                    }
                                }
                            }

                       }
                        flagTest = true;
                        //update CommandDetail trạng thái lệnh 
                     //   CommandDetailService.UpdateListCommandDetail_By_CommandID_Flag_2((int)InvoiceModel.Invoice.CommandID, Constants.Command_Flag_Invoice, InvoiceModel.Invoice.InsertUser);

                 }
                    return Redirect("PrintInvoice/?InvoiceID=" + InvoiceID);
                    //  return RedirectToAction("PrintInvoice", new { InvoiceID });
                    // return RedirectToAction("PrintInvoice", "Bills", new { @InvoiceID = InvoiceID });
                    //   return RedirectToAction("Index", "Bills");
                }

                //}
                //else
                //{
                //    return Redirect("PrintInvoice/?InvoiceID=" + (InvoiceID - 1));
                //}
            }
            catch (Exception ex)
            {
                flagTest = true;
                log.Error(ex);
            }
            log.Info("Finish controller Bills HttpPost CreateInvoice");
         // return Redirect("PrintInvoice/?InvoiceID=0");
            return RedirectToAction("CreateInvoice", "Bills");
        }
        public ActionResult BillList()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERINOUTMANUAL);
            if (checkPermission)
            {
                InvoiceModel.ExportMode = Int32.Parse(ConfigurationService.GetConfiguration(Constants.CONFIG_EXPORT_MODE).Value);

                return View(InvoiceModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public JsonResult GetCommandCardSerial(int dataSerial)
        {
            // kiểm tra chi tiết lệnh đang đã ở trạng thái niêm chì chưa?
            var objDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.CardSerial == dataSerial & x.V_Actual != null & (x.Flag == Constants.Command_Flag_Seal || x.Flag == Constants.Command_Flag_Exported || x.Flag == Constants.Command_Flag_InputHand)).FirstOrDefault();
            if (objDetail == null)
                return Json("", JsonRequestBehavior.AllowGet);
            //Lấy thông tin lệnh
            var obj = CommandService.GetAllCommand().Where(x => x.CardSerial == dataSerial && x.CommandID == objDetail.CommandID).FirstOrDefault();
            if (obj == null)
                return Json("", JsonRequestBehavior.AllowGet);
            else
                return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommandByWorkOrder(int workOrder)
        {
            // kiểm tra chi tiết lệnh đang đã ở trạng thái niêm chì chưa?
            var objDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.WorkOrder == workOrder & x.V_Actual != null & (x.Flag == Constants.Command_Flag_Seal )).FirstOrDefault();
            if (objDetail == null)
                return Json("", JsonRequestBehavior.AllowGet);
            //Lấy thông tin lệnh
            var obj = CommandService.GetAllCommand().Where(x => x.WorkOrder == workOrder && x.CommandID == objDetail.CommandID).FirstOrDefault();
            if (obj == null)
                return Json("", JsonRequestBehavior.AllowGet);
            else
                return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerByCustomerCode(string customerCode)
        {
            var obj = CustomerService.GetAllCustomer().Where(x => x.CustomerCode == customerCode).ToList().FirstOrDefault();
            //var obj = CustomerService.GetAllCustomer().ToList().FirstOrDefault();
            if (obj == null)
                return Json("", JsonRequestBehavior.AllowGet);
            else
                return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListInvoice(int serialNumber)
        {
            var lstCommandDetail = CommandDetailService.GetAllCommandDetailOrderByCompartmentOrder().Where(x => x.CardSerial == serialNumber && x.Flag == Constants.Command_Flag_Seal).ToList();
            //Group lại theo nhóm hàng 
            var groupProduct = lstCommandDetail.GroupBy(x => x.ProductName).ToList();
            var lstInvoice = new List<Invoice>();

            for (var i = 0; i < groupProduct.Count(); i++)
            {
                var obj = new Invoice();
                obj.No = (i + 1).ToString();
                obj.ProductName = groupProduct[i].Key;
                //Thêm thông tin về mã hàng để tiện tham chiếu dữ liệu
                obj.ProductCode = groupProduct[i].ElementAtOrDefault(0).ProductCode;

                for (int j = 0; j < lstCommandDetail.Count(); j++)
                {
                    if (obj.ProductCode == lstCommandDetail[j].ProductCode)
                    {
                        obj.LevelAmount = "M1";
                        obj.ListV += lstCommandDetail[j].V_Actual.ToString() + ",";
                        obj.ListVolume += lstCommandDetail[j].CompartmentOrder.ToString();
                        obj.Value += (float)lstCommandDetail[j].V_Actual;
                        obj.PriceUnit = 0;
                        obj.CalcUnit = "Ltt";
                        obj.Discount = lstCommandDetail[j].Discount ?? 0;
                        obj.Density = lstCommandDetail[j].GasDensity ?? 0;
                        obj.Temp = lstCommandDetail[j].AvgTemperature ?? 0;
                        obj.Vcf = lstCommandDetail[j].Vcf ?? 0;
                        obj.EnvironmentTax = lstCommandDetail[j].EnvironmentTax;
                    }
                }
                obj.ListV = obj.ListV.Substring(0, obj.ListV.Length - 1);

                if (obj.ProductName != null)
                {
                    obj.ProductName = obj.ProductName.ToString().Trim();// Vì có thể bị dài theo định dạng của DB.
                }

                lstInvoice.Add(obj);

            }

            return Json(lstInvoice, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListInvoiceByWorkOrder(int workOrder)
        {
            var lstCommandDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.WorkOrder == workOrder && (x.Flag == Constants.Command_Flag_Seal || x.Flag == Constants.Command_Flag_InputHand)).OrderBy(x => x.CompartmentOrder).ToList();
            //Group lại theo nhóm hàng 
            var groupProduct = lstCommandDetail.GroupBy(x => x.ProductCode).ToList();
            var lstInvoice = new List<Invoice>();

            for (var i = 0; i < groupProduct.Count(); i++)
            {
                var obj = new Invoice();
                obj.No = (i + 1).ToString();
                obj.ProductCode = groupProduct[i].Key;

                for (int j = 0; j < lstCommandDetail.Count(); j++)
                {
                    if (obj.ProductCode == lstCommandDetail[j].ProductCode)
                    {
                        obj.LevelAmount = "M1";
                        obj.ListV += lstCommandDetail[j].V_Actual.ToString() + ",";
                        obj.ListVolume += lstCommandDetail[j].CompartmentOrder.ToString();
                        obj.Value += (float)lstCommandDetail[j].V_Actual;
                        obj.PriceUnit = 0;
                        obj.CalcUnit = "Ltt";
                        obj.ProductName = lstCommandDetail[j].ProductName;

                    }
                }
                obj.ListV = obj.ListV.Substring(0, obj.ListV.Length - 1);

                //if (obj.ProductName != null)
                //{
                //	obj.ProductName = obj.ProductName.ToString().Trim();// Vì có thể bị dài theo định dạng của DB.
                //}

                lstInvoice.Add(obj);
            }

            return Json(lstInvoice, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommandDetail(int cardSerial)
        {
            //Lấy thông tin ngăn tại chi tiết lệnh
            var lstCommandDetail = CommandDetailService.GetAllCommandDetailOrderByCompartmentOrder().Where(x => x.CardSerial == cardSerial && x.Flag == Constants.Command_Flag_Seal).ToList();
            return Json(lstCommandDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommandDetailByWorkOrder(int workOrder)
        {
            //Lấy thông tin ngăn tại chi tiết lệnh
            var lstCommandDetail = CommandDetailService.GetAllCommandDetailOrderByCompartmentOrder().Where(x => x.WorkOrder == workOrder && x.Flag == Constants.Command_Flag_Seal).ToList();
            return Json(lstCommandDetail, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Lấy chi tiết niêm chì khi chọn CardSerial
        /// </summary>
        /// <param name="cardSerial"></param>
        /// <returns></returns>
        public JsonResult GetSeal(int cardSerial)
        {
            var obj = CommandDetailService.GetAllCommandDetail().Where(x => x.CardSerial == cardSerial && x.Flag == Constants.Command_Flag_Seal).First();
            var lstSeal = SealService.GetAllSeal().Where(x => x.CardSerial == cardSerial && x.CommandID == obj.CommandID).ToList();
            return Json(lstSeal, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSealByWorkOrder(int workOrder)
        {
            var obj = CommandDetailService.GetAllCommandDetail().Where(x => x.WorkOrder == workOrder && x.Flag == Constants.Command_Flag_Seal).First();
            var lstSeal = SealService.GetAllSeal().Where(x => x.WorkOrder == workOrder && x.CommandID == obj.CommandID).ToList();
            return Json(lstSeal, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy chi tiết niêm chì khi đã lưu vào hóa đơn
        /// </summary>
        /// <param name="CommandId"></param>
        /// <returns></returns>
        public JsonResult GetSealByCommandID(int CommandId)
        {
            var lstSeal = SealService.GetAllSeal().Where(x => x.CommandID == CommandId).ToList();
            return Json(lstSeal, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataByCardSerial(long cardSerial)
        {
            var obj = CommandService.GetAllCommand().Where(x => x.CardSerial == cardSerial).FirstOrDefault();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataByWorkOrder(long workOrder)
        {
            var obj = CommandService.GetAllCommand().Where(x => x.WorkOrder == workOrder).FirstOrDefault();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommanDetailByCardSerial(long cardSerial)
        {
            //Lấy thông tin để nhập bằng tay
            //var obj = CommandDetailService.GetAllCommandDetail().Where(x => x.CardSerial == cardSerial && x.Flag <= Constants.Command_Flag_StopPressing).ToList();
            var lst = CommandDetailService.GetAllCommandDetail().Where(x => x.CardSerial == cardSerial && (x.Flag == Constants.Command_Flag_StopPressing || x.Flag == Constants.Command_Flag_Approved || x.Flag == Constants.Command_Flag_Exporting || x.Flag == Constants.Command_Flag_PrepareExport || x.Flag == Constants.Command_Flag_InputHand)).OrderBy(x => x.CompartmentOrder).ToList();
            if (lst == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (lst.Count == 0)
                    return Json("", JsonRequestBehavior.AllowGet);
                else
                    return Json(lst, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetCommanDetailByWorkOrder(long workOrder)
        {
            //Lấy thông tin để nhập bằng tay
            //var obj = CommandDetailService.GetAllCommandDetail().Where(x => x.CardSerial == cardSerial && x.Flag <= Constants.Command_Flag_StopPressing).ToList();
            var lst = CommandDetailService.GetAllCommandDetail().Where(x => x.WorkOrder == workOrder && (x.Flag == Constants.Command_Flag_StopPressing || x.Flag == Constants.Command_Flag_Approved || x.Flag == Constants.Command_Flag_Exporting || x.Flag == Constants.Command_Flag_PrepareExport || x.Flag == Constants.Command_Flag_InputHand)).OrderBy(x => x.CompartmentOrder).ToList();
            if (lst == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (lst.Count == 0)
                    return Json("", JsonRequestBehavior.AllowGet);
                else
                    return Json(lst, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetDataByArmno(int armno)
        {
            var warehouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            var obj = CommandDetailService.GetAllCommandDetail().Where(x => x.ArmNo == armno && x.AvgDensity != null && x.GasDensity != null && x.WareHouseCode == warehouse).OrderByDescending(x => x.TimeOrder).FirstOrDefault();
            if (obj == null)
            {
                obj = CommandDetailService.GetAllCommandDetail().Where(x => x.ArmNo == armno).FirstOrDefault();
                if (obj == null)
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(obj, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult RegisterBills()
        {
            CommandDetailModel.ListCommandDetails = CommandDetailService.GetAllCommandDetail().ToList();


            return View(CommandDetailModel);
        }

        [HttpPost]
        public ActionResult RegisterBills(IEnumerable<MCommandDetail> mCommandDetail)
        {

            log.Info("Start controller Bills HttpPost RegisterBills");
            foreach (var item in mCommandDetail)
            {
                var oItem = new MCommandDetail();
                var objcomdetail = CommandDetailService.GetAllCommandDetail().Where(x => x.ID == item.ID).First();
                objcomdetail.MixingRatio = item.MixingRatio;
                objcomdetail.CommandID = item.CommandID;
                objcomdetail.TimeOrder = item.TimeOrder;
                objcomdetail.TimeStart = item.TimeStart;
                objcomdetail.TimeStop = item.TimeStop;
                objcomdetail.WorkOrder = item.WorkOrder;
                objcomdetail.CompartmentOrder = item.CompartmentOrder;
                objcomdetail.ArmNo = item.ArmNo;
                objcomdetail.WareHouseCode = item.WareHouseCode;
                objcomdetail.CardData = item.CardData;
                objcomdetail.CardSerial = item.CardSerial;
                objcomdetail.V_Preset = item.V_Preset;
                objcomdetail.V_Actual = item.V_Actual;
                objcomdetail.ProductName = item.ProductName;
                // objcomdetail.V_Deviation = item.V_Deviation;
                objcomdetail.V_Deviation = item.V_Actual - item.V_Preset;
                objcomdetail.AvgTemperature = item.AvgTemperature;
                objcomdetail.CurrentTemperature = item.CurrentTemperature;
                objcomdetail.Flag = 6;
                objcomdetail.TotalStart = item.TotalStart;
                objcomdetail.TotalEnd = item.TotalEnd;
                objcomdetail.Vehicle = item.Vehicle;
                objcomdetail.MixingRatio = item.MixingRatio;
                objcomdetail.GasDensity = item.GasDensity;
                objcomdetail.AlcoholicDensity = item.AlcoholicDensity;
                objcomdetail.V_Actual_15 = item.V_Actual_15;
                objcomdetail.TotalStart_15 = item.TotalStart_15;
                objcomdetail.TotalEnd_15 = item.TotalEnd_15;
                objcomdetail.V_Actual_Base = item.V_Actual_Base;
                objcomdetail.V_Actual_E = item.V_Actual_E;
                objcomdetail.V_Actual_Base_15 = item.V_Actual_Base_15;
                objcomdetail.V_Actual_E_15 = item.V_Actual_E_15;
                objcomdetail.TotalStart_Base = item.TotalStart_Base;
                objcomdetail.TotalStart_E = item.TotalStart_E;
                objcomdetail.TotalStart_Base_15 = item.TotalStart_Base_15;
                objcomdetail.TotalStart_E_15 = item.TotalStart_E_15;
                objcomdetail.TotalStart_E_15 = item.TotalStart_E_15;
                objcomdetail.TotalEnd_E = item.TotalEnd_E;
                objcomdetail.TotalEnd_Base_15 = item.TotalEnd_Base_15;
                objcomdetail.TotalEnd_E_15 = item.TotalEnd_E_15;
                objcomdetail.AvgDensity = item.AvgDensity;
                objcomdetail.CTL_Base = item.CTL_Base;
                objcomdetail.CTL_E = item.CTL_E;
                objcomdetail.ActualRatio = item.ActualRatio;
                objcomdetail.InsertUser = HttpContext.User.Identity.Name;
                CommandDetailService.UpdateCommandDetail(objcomdetail);
                CommandDetailService.CalculateV15();
            }

            log.Info("Finish controller Bills HttpPost RegisterBills");
            return RedirectToAction("BillList");
        }

        public bool UpdateNoteInvoiceDetail(int invoiceDetailId, string note)
        {
            var rs = false;
            rs = InvoiceDetailService.UpdateNoteInvoiceDetailById(invoiceDetailId, note, HttpContext.User.Identity.Name);

            return rs;
        }

        public bool UpdateFlagCommandDetail(int invoiceDetailId)
        {
            var rs = false;
            rs = InvoiceDetailService.UpdateFlagbyCommandid(invoiceDetailId);

            return rs;
        }

        public ActionResult ApproveCommand(CommandModel commandModel, int? page)
        {
            
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_APPROVECOMMAND);
            if (checkPermission)
            {
                String userName = HttpContext.User.Identity.Name;
                commandModel.CardSerial = commandModel.CardSerial ?? "";
                commandModel.CardData = commandModel.CardData ?? "";
                commandModel.VehicleNumber = commandModel.VehicleNumber ?? "";
                commandModel.ListVehicle = VehicleService.GetAllVehicle().ToList();
                commandModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
                commandModel.ListWareHouse = WareHouseService.GetAllWareHouse().ToList();
                commandModel.Flag = commandModel.Flag ?? 100;
                int pageNumber = (page ?? 1);

                log.Info("Start controller command CommandbyVehicle");
                DateTime? startdate = DateTime.ParseExact(commandModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                DateTime? enddate = DateTime.ParseExact(commandModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                List<ListStatus> lstStatus = new List<ListStatus>{
                   new ListStatus{name = "Tất cả",flag = 100},
                   new ListStatus{name = "Đang xuất",flag = 2},
                   new ListStatus{name = "Xuất xong",flag = 3},
                   new ListStatus{name = "Đã in",flag = 8}, 
                   new ListStatus{name = "Dừng ép",flag = 4},
                   new ListStatus{name = "Hoàn thành",flag = 68},
                   };
                commandModel.LstStatus = lstStatus;

                commandModel.ListIECommand = CommandService.GetList_Command_By_Vehicle(byte.Parse(Session[Constants.Session_WareHouse].ToString()), commandModel.CertificateNumber, commandModel.CardSerial, commandModel.CardData,
                     startdate, enddate, commandModel.VehicleNumber, commandModel.Flag, "Bill").ToPagedList(pageNumber, Constants.PAGE_SIZE);
                log.Info("Finish controller command CommandbyVehicle"); 

                commandModel.ListCommandDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.Flag == 3 || x.Flag == 4 || x.Flag == 6 || x.Flag == 8).OrderBy(x => x.Flag).OrderBy(x => x.CertificateNumber).ThenBy(x => x.CompartmentOrder).ToList();

                //Lấy tất cả các mặt hàng
                commandModel.ListProductTemp = ProductService.GetAllProduct().Select(x => new ProductTemp 
                {
                    Abbreviations = x.Abbreviations,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName
                }).ToList();

                #region Phương tiện
                var lstvehicle = new List<Datum>();
                var lst3 = VehicleService.GetAllVehicle();
                foreach (var it in lst3)
                {
                    var datum = new Datum();
                    datum.type = it.VehicleNumber;
                    datum.name = it.VehicleNumber;
                    lstvehicle.Add(datum);
                }
                commandModel.LstVehicle = lstvehicle;
                #endregion


                log.Info("Finish controller command CommandbyVehicle");

                return View(commandModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
    }
}