using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Data;
using PetroBM.Web.Models;
using log4net;
using System.Resources;
using System.Reflection;
using System.Globalization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace PetroBM.Web.Controllers
{

    public class CommandController : BaseController
    {
        private readonly IUserService UserService;
        private readonly IConfigurationService ConfigurationService;
        private readonly IProductService ProductService;
        private readonly ISealService SealService;
        private readonly ICommandService CommandService;
        private readonly ICommandDetailService CommandDetailService;
        private readonly IWareHouseService WareHouseService;
        private readonly ICustomerService CustomerService;
        private readonly IVehicleService VehicleService;
        private readonly ICardService CardService;
        private readonly IDriverService DriverService;
        private readonly IWareHouseService warehouseService;
        private readonly BaseService baseService;

        public CommandModel commandModel;
        ILog log = log4net.LogManager.GetLogger(typeof(CommandController));


        public CommandController(ICommandService CommandService, CommandModel commandModel,
            IConfigurationService ConfigurationService, IUserService UserService, IProductService ProductService,
            ICommandDetailService CommandDetailService, ISealService SealService,
        IWareHouseService WareHouseService, ICustomerService CustomerService, IDriverService DriverService,
            IVehicleService VehicleService, ICardService CardService, IWareHouseService warehouseService, BaseService baseService)
        {
            this.CommandService = CommandService;
            this.commandModel = commandModel;
            this.ConfigurationService = ConfigurationService;
            this.UserService = UserService;
            this.ProductService = ProductService;
            this.CommandDetailService = CommandDetailService;
            this.VehicleService = VehicleService;
            this.WareHouseService = WareHouseService;
            this.CustomerService = CustomerService;
            this.CardService = CardService;
            this.DriverService = DriverService;
            this.warehouseService = warehouseService;
            this.SealService = SealService;
            this.baseService = baseService;


        }
        public ActionResult GetDriverNameByVehicleNumber(string vehiclenumber)
        {
            log.Info("start controller command GetDriverNameByVehicleNumber");
            var obj = VehicleService.GetDriverNameByVehicleNumber(vehiclenumber).ToList();
            log.Info("finish controller command GetDriverNameByVehicleNumber");
            return Json(obj, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCardDataByVehicleNumber(string vehiclenumber)
        {
            log.Info("start controller command GetCardDataByVehicleNumber");
            var obj = VehicleService.GetCardDataByVehicleNumber(vehiclenumber).ToList().FirstOrDefault();
            var objCard = new CardTestModel();
            objCard.CardData = obj.CardData ;
            objCard.CardSerial = obj.CardSerial ;
            objCard.CurrentFlag = -1;
            log.Info("finish controller command GetCardDataByVehicleNumber");
            return Json(objCard, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCardSerialByCardData(string cardData)
        {
            log.Info("Start controller Command GetCardSerialByCardData");
            var obj = CardService.GetAllCard().Where(x => x.CardData == cardData).FirstOrDefault();
            if (obj != null)
            {
                log.Info("finish with data controller Command GetCardSerialByCardData");
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                log.Info("finish without data controller Command GetCardSerialByCardData");
                return Json("", JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult GetCommandByWareHouseId(byte warehouseid)
        {
            var listworkorder = new List<CommandList>();
            var obj = warehouseService.FindWareHouseById(warehouseid);
            var warehousecode = Convert.ToByte(obj.WareHouseCode);
            var objcommand = CommandService.GetAllCommand().Where(x => x.WareHouseCode == warehousecode).ToList();
            foreach (var item in objcommand)
            {
                var commandlist = new CommandList();
                commandlist.commandid = item.CommandID;
                commandlist.workorder = (decimal)(item.WorkOrder);
                listworkorder.Add(commandlist);
            }
            return Json(listworkorder, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCardDataByCardSerial(long cardSerial)
        {
            log.Info("Start controller Command GetCardDataByCardSerial");
            //Không có trong hệ thống
            var obj = CardService.GetAllCard().Where(x => x.CardSerial == cardSerial && x.WareHouseCode == Session[Constants.Session_WareHouse].ToString()).FirstOrDefault();
            if (obj == null)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            //Kiểm tra Card Serial đang dùng trong hệ thống không?
            var objCommandDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.CardSerial == cardSerial && x.WareHouseCode == byte.Parse(Session[Constants.Session_WareHouse].ToString()) && x.Flag != Constants.Command_Flag_Invoice && x.Flag != Constants.Command_Flag_CardError && x.Flag != Constants.Command_Flag_InputCancel).FirstOrDefault();
            if (objCommandDetail == null)
            {
                log.Info("finish without data controller Command GetCardDataByCardSerial");

                var objCard = new CardTestModel();
                objCard.CardData = obj.CardData;
                objCard.CardSerial = obj.CardSerial;
                objCard.CurrentFlag = -1;
                return Json(objCard, JsonRequestBehavior.AllowGet);
            }
            else
            {
                log.Info("finish with data controller Command GetCardDataByCardSerial");
                var objCard = new CardTestModel();
                objCard.CardData = objCommandDetail.CardData;
                objCard.CardSerial = objCommandDetail.CardSerial;
                objCard.CurrentFlag = (int)objCommandDetail.Flag;
                return Json(objCard, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CommandbyVehicle(CommandModel commandModel, int? page)
        {
            String userName = HttpContext.User.Identity.Name;
            commandModel.CardSerial = commandModel.CardSerial ?? "";
            commandModel.CardData = commandModel.CardData ?? "";
            commandModel.VehicleNumber = commandModel.VehicleNumber ?? "";
            commandModel.ListVehicle = VehicleService.GetAllVehicle().ToList();
            commandModel.ListCustomer = CustomerService.GetAllCustomer().ToList();
            int pageNumber = (page ?? 1);

            log.Info("Start controller command CommandbyVehicle");
            DateTime? startdate = DateTime.ParseExact(commandModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            DateTime? enddate = DateTime.ParseExact(commandModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

            commandModel.ListIECommand = CommandService.GetList_Command_By_Vehicle(byte.Parse(Session[Constants.Session_WareHouse].ToString()), commandModel.CertificateNumber, commandModel.CardSerial, commandModel.CardData,
                startdate, enddate, commandModel.VehicleNumber, commandModel.Flag, "Command").ToPagedList(pageNumber, Constants.PAGE_SIZE);

            commandModel.ListCommandDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.DeleteFlg == false).OrderBy(x => x.CompartmentOrder).ToList();
            log.Info("Finish controller command CommandbyVehicle");

            return View(commandModel);
        }

        public ActionResult Index(int? page, string search)
        {
            log.Info("Start controller command index");
            log.Info("Start controller Command RegisterCommand");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERCOMMAND);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                if (search == null || search == "")
                {
                    commandModel.ListCommand = CommandService.GetAllCommand().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                }
                else
                {
                    //*********************************************************************truy vấn tạm card Data **********************
                    commandModel.ListCommand = CommandService.GetAllCommand().Where(x => x.CardData.Contains(search)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                }
                commandModel.ListProduct = ProductService.GetAllProduct().Where(x => x.ShowRank == 1).ToList();
                commandModel.ListWareHouse = warehouseService.GetAllWareHouse().ToList();

                log.Info("Finish controller command index");
                return View(commandModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }

        }

        [HttpGet]
        public ActionResult RegisterCommand()
        {
            log.Info("Start controller Command RegisterCommand"); 
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERCOMMAND);
            if (checkPermission)
            {
                commandModel.Command.TimeOrder = DateTime.Now;
                commandModel.Command.CommandID = 0;
                commandModel.ListProduct = ProductService.GetAllProduct().Where(x => x.ShowRank==1).ToList();
                commandModel.Command.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                //commandModel.TimeOrder = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).ToString(Constants.DATE_FORMAT);
                commandModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT);
                commandModel.CertificateTime = DateTime.Now.ToString(Constants.DATE_FORMAT);
                commandModel.ExportMode = Int32.Parse(ConfigurationService.GetConfiguration(Constants.CONFIG_EXPORT_MODE).Value);
                commandModel.Command.CertificateNumber = CommandService.getCertificateNumber();
                var Ocommand = CommandService.GetAllCommand().FirstOrDefault();

                #region  Kho
                var lstWH = new List<Datum>();
                var lst = WareHouseService.GetAllWareHouse();
                foreach (var it in lst)
                {
                    var datum = new Datum();
                    datum.name = it.WareHouseName;
                    datum.type = it.WareHouseCode.ToString();
                    lstWH.Add(datum);
                }
                commandModel.LstWareHouse = lstWH;
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
                commandModel.LstCustomer = lstC;
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
                commandModel.LstVehicle = lstV;
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
                commandModel.LstDriver = lstD;
                #endregion

                log.Info("Finish controller Command RegisterCommand");
                return View(commandModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }


        public ActionResult CommandView(int Id, CommandModel commandModel)
        {
            log.Info("start controller command commandView");

            // Phần này nên viết lúc đăng nhập
            // ResourceManager rm = new ResourceManager("Resources.Messages", System.Reflection.Assembly.Load("App_GlobalResources"));
            Session[Constants.Session_TitleProviceName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("ProviceName");
            Session[Constants.Session_TitleReportCompanyName] = Constants.Session_Content_TitleReportCompanyName;// rm.GetString("CompanyName");
            Session[Constants.Session_TitleCompanyAddress] = Constants.Session_Content_TitleCompanyAddress;// rm.GetString("CompanyAddress");
            Session[Constants.Session_TitleCompanyFax] = Constants.Session_Content_TitleCompanyFax;// rm.GetString("CompanyFax");
            Session[Constants.Session_TitleCompanyPhone] = Constants.Session_Content_TitleCompanyPhone;// rm.GetString("CompanyPhone");
            commandModel.Command = CommandService.FindCommandById(Id);
            commandModel.TimeOrder = ((DateTime)commandModel.Command.TimeOrder).ToString(Constants.DATE_FORMAT);
            //commandModel.TimeOrder = DateTime.Now.ToString(Constants.DATE_FORMAT);
            commandModel.ListCommandDetail = CommandDetailService.GetAllCommandDetail().Where(x => x.CommandID == Id && x.DeleteFlg == false).OrderBy(x => x.CompartmentOrder).ToList();
            commandModel.ListInfo = ConfigurationService.GetAllConfiguration().ToList();
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
            commandModel.LstCustomer = lstC;
            #endregion

            var lstP = new List<Datum>();
            var lst = ProductService.GetAllProduct().Where(x => x.ShowRank == 1);
            foreach (var it in lst)
            {
                var datum = new Datum();
                datum.name = it.ProductCode;
                datum.type = it.ProductName;
                lstP.Add(datum);
            }
            commandModel.LstProduct = lstP;
            commandModel.ListProduct = ProductService.GetAllProduct().Where(x => x.ShowRank == 1).ToList();
            commandModel.ListVehicle = VehicleService.GetAllVehicle().ToList();
            //Tạo dòng ghi chú tổng lượng hàng

            var groupCommandDetails = commandModel.ListCommandDetail.OrderBy(c => c.ProductName); 
            var totalCmn = "";
            var ProductCode = "";
            var productName = "";
            float? subTotal = 0;
            foreach (var commandDetail in groupCommandDetails)
            {
                if (!ProductCode.Equals(commandDetail.ProductCode))
                {
                    if (subTotal != 0)
                    {
                        totalCmn += productName + ": " + subTotal + "; ";
                        subTotal = 0;
                    }

                    ProductCode = commandDetail.ProductCode;
                    foreach (var product in lst)
                    {
                        if (product.ProductCode == ProductCode)
                        {
                            productName = product.ProductName;
                        }
                    }
                    //productName = commandDetail.ProductName;
                }
                subTotal += commandDetail.V_Preset;
            }
            totalCmn += productName + ": " + subTotal;
            commandModel.TotalCommand = totalCmn;

            var customer = CustomerService.GetCustomerCode(commandModel.Command.CustomerCode).First();
            if (customer != null)
            {
                commandModel.CustomerName = customer.CustomerName;
                commandModel.Command.CustomerCode = customer.CustomerCode + " - " + customer.CustomerName;
            }
            else
            {
                commandModel.CustomerName = "";
            }

            //Lấy danh hàng hóa 
            commandModel.ListTemProduct = ProductService.GetAllProduct().Where(x => x.ShowRank == 1).Select(x => new DataValue
            {
                Caption = "",
                Code = x.ProductCode,
                Name = x.ProductName,
                Unit = "",
                Value = x.ProductId
            }).ToList();

            // Lấy danh sách phương tiện
            commandModel.ListTemVehicle = VehicleService.GetAllVehicle().Select(x => new DataValue
            {
                Caption = "",
                Code = x.VehicleNumber,
                Name = x.VehicleNumber
            }).ToList();

            commandModel.ListSeal = SealService.GetAllSeal().Where(x => x.CommandID == Id).ToList();
            var objDriver = commandModel.Command.DriverName;
            var objcmt = DriverService.GetAllDriver().Where(x => x.Name == objDriver).FirstOrDefault();
            if (objcmt != null)
            {
                Session[Constants.Session_TitleIdentificationNumber] = objcmt.IdentificationNumber;
            }
            else
                Session[Constants.Session_TitleIdentificationNumber] = "";
            log.Info("Finish controller command commandView");
            return View(commandModel);
        }




        [HttpPost]
        public ActionResult RegisterCommand(CommandModel commandModel, IEnumerable<CProductModel> mProduct, IEnumerable<MCommandDetail> commandDetails)
        {
            string Mess = "";
            log.Info("Start controller command RegisterCommand");
            #region Xử lí quy luật lấy WorkOrder
            //Lấy var quy luật WorkOrder 
            var objWorkOrderMax = CommandService.GetWorkOrderMax();
            int objWorkOrder = 0;
            if (objWorkOrderMax != null)
            {
                objWorkOrder = (int)objWorkOrderMax;
            }

            decimal intWorkOrder = 0;  //ví dụ: 7236001
            if (objWorkOrder == 0)
                intWorkOrder = (DateTime.Now.Year % 10) * 1000000 + DateTime.Now.DayOfYear * 1000 + 1; //******** Khởi tạo *****************//
            else
                if (objWorkOrder.ToString("0000000").Substring(0, 4) == ((DateTime.Now.Year % 10) * 1000 + DateTime.Now.DayOfYear).ToString("0000").Substring(0, 4)) //Cùng ngày
                intWorkOrder = (int)objWorkOrder + 1;
            else
                intWorkOrder = (DateTime.Now.Year % 10) * 1000000 + DateTime.Now.DayOfYear * 1000 + 1;//Khác ngày

            #endregion
            commandModel.TimeOrder = commandModel.TimeOrder.ToString();
            //commandModel.Command.TimeOrder = ((DateTime)commandModel.TimeOrder).ToString(Constants.DATE_FORMAT);
            commandModel.Command.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            // Xử lại tại đây chi tiết lệnh xuất hàng            
            commandModel.Command.InsertUser = HttpContext.User.Identity.Name;
            commandModel.Command.WorkOrder = intWorkOrder;
            commandModel.Command.TimeOrder = DateTime.ParseExact(commandModel.TimeOrder, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            commandModel.Command.CertificateTime = DateTime.ParseExact(commandModel.CertificateTime, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            commandModel.Command.CustomerCode = commandModel.Command.CustomerCode.Split(new[] { " - " }, StringSplitOptions.None)[0].ToString();

            CommandService.CreateCommand(commandModel.Command);
            var commandid = CommandService.GetNewId();
            commandModel.ListCommandDetail = (List<MCommandDetail>)commandDetails;

            foreach (var item in mProduct)
            {
                
                if (item.Volume == null)
                {
                    //item.Volume = ""; //compartment
                    continue;
                }

                if (item.Volume.Length > 0)
                {
                    if (item.Volume.Length == 1)
                    {
                        var oItem = new MCommandDetail();
                        oItem.ID = 0;
                        oItem.CommandID = commandid;
                        // oItem.WareHouseCode = commandModel.Command.WareHouseCode;
                        oItem.InsertDate = commandModel.Command.InsertDate;
                        oItem.InsertUser = commandModel.Command.InsertUser;
                        oItem.CardData = commandModel.Command.CardData;
                        oItem.CardSerial = commandModel.Command.CardSerial;
                        oItem.CompartmentOrder = byte.Parse(item.Volume);
                        oItem.TimeOrder = DateTime.ParseExact(commandModel.TimeOrder, Constants.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
                        oItem.Vehicle = commandModel.Command.VehicleNumber.Trim(' ');
                        oItem.Flag = Constants.Command_Flag_Register;
                        oItem.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                        var objProduct = ProductService.GetAllProduct().Where(x => x.ShowRank == 1).Where(x => x.ProductName == item.ProductName).First();
                        oItem.ProductName = objProduct.Abbreviations;
                        oItem.ProductCode = objProduct.ProductCode;
                        var objVehicle = VehicleService.GetAllVehicle().Where(x => x.VehicleNumber == oItem.Vehicle).First();
                        oItem.CommandType = objVehicle.CommandType;
                        oItem.CommandFlag = Constants.COMMAND_FLAG_INIT;
                        oItem.WorkOrder = intWorkOrder;
                        oItem.Discount = item.Discount;
                        oItem.EnvironmentTax = commandModel.Command.EnvironmentTax;
                        oItem.CertificateNumber = commandModel.Command.CertificateNumber;
                        oItem.CertificateTime = commandModel.Command.CertificateTime;
                        oItem.Description = commandModel.Command.Description;
                        oItem.GGT = commandModel.Command.GGT;
                        var oVehicle = VehicleService.GetAllVehicle().Where(x => x.VehicleNumber == commandModel.Command.VehicleNumber).FirstOrDefault();
                        if (oVehicle != null)
                        {
                            switch (item.Volume)
                            {
                                case "1":
                                    oItem.V_Preset = oVehicle.Volume1;
                                    break;
                                case "2":
                                    oItem.V_Preset = oVehicle.Volume2;
                                    break;
                                case "3":
                                    oItem.V_Preset = oVehicle.Volume3;
                                    break;
                                case "4":
                                    oItem.V_Preset = oVehicle.Volume4;
                                    break;  
                            } 

                        }

                        if (commandModel.ListCommandDetail[int.Parse(item.Volume) - 1].V_Preset == null)
                        {
                            oItem.V_Preset = 0;
                        }
                        else
                        {
                            if (oItem.V_Preset >= commandModel.ListCommandDetail[int.Parse(item.Volume) - 1].V_Preset) 
                            {
                                if (commandModel.ListCommandDetail[int.Parse(item.Volume) - 1].V_Preset < 50)
                                    oItem.V_Preset = 50;
                                else
                                    oItem.V_Preset = commandModel.ListCommandDetail[int.Parse(item.Volume) - 1].V_Preset;
                            }
                            else
                                Mess = "Lượng đặt quá lượng của ngăn => cập nhật bằng ngăn";
                        }
                            
                        oItem.Vehicle = oVehicle.VehicleNumber;
                        oItem.UpdateDate = commandModel.Command.InsertDate;
                        oItem.UpdateUser = commandModel.Command.UpdateUser;
                        CommandDetailService.CreateCommandDetail(oItem);
                    }
                    else
                    {
                        string[] arr = item.Volume.Select(c => c.ToString()).ToArray();
                        for (int i = 0; i < arr.Length; i++)
                        {

                            var oItem = new MCommandDetail();
                            oItem.ID = 0;
                            oItem.CommandID = commandid;
                            oItem.WareHouseCode = commandModel.Command.WareHouseCode;
                            oItem.InsertDate = commandModel.Command.InsertDate;
                            oItem.InsertUser = commandModel.Command.InsertUser;
                            oItem.CardData = commandModel.Command.CardData;
                            oItem.CardSerial = commandModel.Command.CardSerial;
                            oItem.CompartmentOrder = byte.Parse(arr[i]);
                            oItem.TimeOrder = commandModel.Command.TimeOrder;
                            oItem.Vehicle = commandModel.Command.VehicleNumber;
                            oItem.Flag = Constants.Command_Flag_Register;
                            oItem.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());

                            oItem.WorkOrder = intWorkOrder;
                            oItem.Discount = item.Discount;
                            oItem.EnvironmentTax = commandModel.Command.EnvironmentTax;
                            oItem.CertificateNumber = commandModel.Command.CertificateNumber;
                            oItem.CertificateTime = commandModel.Command.CertificateTime;
                            var objProductCode = ProductService.GetAllProduct().Where(x => x.ShowRank == 1).Where(x => x.ProductName == item.ProductName).First();
                            oItem.ProductCode = objProductCode.ProductCode;
                            oItem.ProductName = objProductCode.Abbreviations;
                            var objVehicle = VehicleService.GetAllVehicle().Where(x => x.VehicleNumber == oItem.Vehicle).First();
                            oItem.CommandType = objVehicle.CommandType;
                            oItem.CommandFlag = Constants.COMMAND_FLAG_INIT;
                            oItem.Description = commandModel.Command.Description;
                            oItem.GGT = commandModel.Command.GGT;
                            
                            var oVehicle = VehicleService.GetAllVehicle().Where(x => x.VehicleNumber == commandModel.Command.VehicleNumber).FirstOrDefault();
                            if (oVehicle != null)
                            {
                                switch (arr[i])
                                {
                                    case "1":
                                        oItem.V_Preset = oVehicle.Volume1;
                                        break;
                                    case "2":
                                        oItem.V_Preset = oVehicle.Volume2;
                                        break;
                                    case "3":
                                        oItem.V_Preset = oVehicle.Volume3;
                                        break;
                                    case "4":
                                        oItem.V_Preset = oVehicle.Volume4;
                                        break;
                                } 
                            }
                            if (commandModel.ListCommandDetail[int.Parse(arr[i]) - 1].V_Preset == null)
                            {
                                oItem.V_Preset = 0;
                            }
                            else
                            {
                                if (oItem.V_Preset >= commandModel.ListCommandDetail[int.Parse(arr[i])-1].V_Preset)
                                {
                                    if (commandModel.ListCommandDetail[int.Parse(arr[i]) - 1].V_Preset < 50)
                                        oItem.V_Preset = 50;
                                    else
                                        oItem.V_Preset = commandModel.ListCommandDetail[int.Parse(arr[i]) - 1].V_Preset;
                                }

                                else
                                    Mess = "Lượng đặt quá lượng của ngăn " + i + " => cập nhật bằng ngăn";
                            }
                            oItem.Vehicle = oVehicle.VehicleNumber;
                            oItem.UpdateDate = commandModel.Command.InsertDate;
                            oItem.UpdateUser = commandModel.Command.UpdateUser;
                            CommandDetailService.CreateCommandDetail(oItem);
                        }
                    }
                } 
            }

            log.Info("Finish controller command RegisterCommand");
            if(Mess != "")
                TempData["AlertMessage"] = Mess;
            else
                TempData["AlertMessage"] = "Thành công";
            return RedirectToAction("CommandView", "Command", new { id = commandid });
        }


        public ActionResult Edit(int id)
        {
            log.Info("view");
            commandModel.Command = CommandService.FindCommandById(id);
            return View(commandModel);
        }

        [HttpPost]
        public ActionResult Edit(CommandModel commandModel)
        {
            log.Info("start");
            var command = CommandService.FindCommandById(commandModel.Command.CommandID);
            if (null != command)
            {
                command.UpdateUser = HttpContext.User.Identity.Name;

                var rs = CommandService.UpdateCommand(command);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }

                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                }
            }
            log.Info("finish");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            log.Info("view");
            return View(commandModel);
        }

        [HttpPost]
        public ActionResult Create(MCommand command)
        {
            log.Info("start");
            command.InsertUser = HttpContext.User.Identity.Name;
            command.UpdateUser = HttpContext.User.Identity.Name;

            var rs = CommandService.CreateCommand(command);

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            }

            log.Info("finish");
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            log.Info("start");
            var rs = CommandService.DeleteCommand(id);
            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
            }

            log.Info("finish");
            return RedirectToAction("Index");
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
                commandModel.Flag = commandModel.Flag ?? 100;
                int pageNumber = (page ?? 1);

                log.Info("Start controller command CommandbyVehicle");
                DateTime? startdate = DateTime.ParseExact(commandModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                DateTime? enddate = DateTime.ParseExact(commandModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);

                List<ListStatus> lstStatus = new List<ListStatus>{
                   new ListStatus{name = "Tất cả",flag = 100},
                   new ListStatus{name = "Đã duyệt",flag = 0},
                   new ListStatus{name = "Chờ duyệt",flag = 99},
                   new ListStatus{name = "Hủy",flag = 5},
                   };
                commandModel.LstStatus = lstStatus;

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

                commandModel.ListIECommand = CommandService.GetList_Command_By_Vehicle(byte.Parse(Session[Constants.Session_WareHouse].ToString()), commandModel.WorkOrder, commandModel.CardSerial, commandModel.CardData,
                    startdate, enddate, commandModel.VehicleNumber, commandModel.Flag, "Command").ToPagedList(pageNumber, Constants.PAGE_SIZE);
                log.Info("Finish controller command CommandbyVehicle");

                commandModel.ListCommandDetail = CommandDetailService.GetAllCommandDetail().ToList();
                log.Info("Finish controller command CommandbyVehicle");

                return View(commandModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public bool UpdateCustomerCommand(int commandId, string customerCode, string user)
        {
            var rs = false;
            rs = CommandService.UpdateCustomerCommandID(commandId, customerCode, user);

            return rs;
        }
    }
}