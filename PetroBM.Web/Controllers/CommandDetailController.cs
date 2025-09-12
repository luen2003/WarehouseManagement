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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.ComponentModel.Design;
using Microsoft.Ajax.Utilities;

namespace PetroBM.Web.Controllers
{

    public class CommandDetailController : BaseController
    {
        private readonly ICommandDetailService commanddetailService;
        public CommandDetailModel commanddetailModel;
        private readonly ICommandService CommandService;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly IVehicleService VehicleService;
        private readonly IConfigArmService configArmService;
        private readonly BaseService baseService;
        private readonly ICustomerService customerService;

        public CommandDetailController(ICommandDetailService commanddetailService, CommandDetailModel commanddetailModel,
            IConfigurationService configurationService, IVehicleService VehicleService,
            IUserService userService, ICommandService CommandService, IConfigArmService configArmService, BaseService baseService, ICustomerService customerService)
        {
            this.commanddetailService = commanddetailService;
            this.commanddetailModel = commanddetailModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.CommandService = CommandService;
            this.VehicleService = VehicleService;
            this.configArmService = configArmService;
            this.baseService = baseService;
            this.customerService = customerService;
        }

        public JsonResult GetAllById(int commanddetailid)
        {
            var obj = commanddetailService.GetAllCommandDetailByID(commanddetailid).ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public ActionResult RegisterCommandDetail()
        {
            commanddetailModel.ListCommandDetails = commanddetailService.GetAllCommandDetail().ToList();

            commanddetailModel.CommandDetail.ID = 0;
            ViewBag.Title = "Application Catalog";

            return View(commanddetailModel);
        }


        [HttpPost]
        public ActionResult RegisterCommandDetail(IEnumerable<MCommandDetail> mCommandDetail)
        {
            try
            {
                commanddetailModel.CommandDetail.InsertUser = HttpContext.User.Identity.Name;

                foreach (var item in mCommandDetail)
                {
                    var obj = commanddetailService.GetAllCommandDetailByID(item.ID);

                    foreach (var items in obj)
                    {
                        var oItem = new MCommandDetail();
                        oItem.CommandID = items.CommandID;
                        oItem.CommandType = items.CommandType;
                        oItem.CommandFlag = Constants.COMMAND_FLAG_NEW;
                        oItem.TimeOrder = items.TimeOrder;
                        oItem.WorkOrder = items.WorkOrder;
                        oItem.CompartmentOrder = items.CompartmentOrder;
                        oItem.ProductCode = items.ProductCode;
                        oItem.ProductName = items.ProductName;
                        oItem.WareHouseCode = items.WareHouseCode;
                        oItem.CardData = items.CardData;
                        oItem.CardSerial = items.CardSerial;
                        oItem.V_Preset = item.V_Preset;
                        oItem.V_Actual = 0;
                        oItem.V_Deviation = items.V_Deviation;
                        oItem.AvgTemperature = items.AvgTemperature;
                        oItem.CurrentTemperature = items.CurrentTemperature;
                        oItem.Flag = Constants.Command_Flag_Approved;
                        oItem.TotalStart = items.TotalStart;
                        oItem.TotalEnd = items.TotalEnd;
                        oItem.Vehicle = items.Vehicle;
                        oItem.MixingRatio = items.MixingRatio;
                        oItem.GasDensity = items.GasDensity;
                        oItem.AlcoholicDensity = items.AlcoholicDensity;
                        oItem.V_Actual_15 = items.V_Actual_15;
                        oItem.TotalStart_15 = items.TotalStart_15;
                        oItem.TotalEnd_15 = items.TotalEnd_15;
                        oItem.V_Actual_Base = items.V_Actual_Base;
                        oItem.V_Actual_E = items.V_Actual_E;
                        oItem.V_Actual_Base_15 = items.V_Actual_Base_15;
                        oItem.V_Actual_E_15 = items.V_Actual_E_15;
                        oItem.TotalStart_Base = items.TotalEnd_Base;
                        oItem.TotalStart_E = items.TotalEnd_E;
                        oItem.TotalStart_Base_15 = items.TotalStart_Base_15;
                        oItem.TotalStart_E_15 = items.TotalStart_E_15;
                        oItem.TotalEnd_Base = items.TotalEnd_Base;
                        oItem.TotalEnd_E = items.TotalEnd_E;
                        oItem.TotalEnd_Base_15 = items.TotalEnd_Base;
                        oItem.TotalEnd_E_15 = items.TotalEnd_E_15;
                        oItem.AvgDensity = items.AvgDensity;
                        oItem.CTL_Base = items.CTL_Base;
                        oItem.CTL_E = items.CTL_E;
                        oItem.InsertDate = items.InsertDate;
                        oItem.InsertUser = items.InsertUser;
                        oItem.UpdateDate = items.UpdateDate;
                        oItem.UpdateUser = items.UpdateUser;
                        oItem.VersionNo = items.VersionNo;
                        oItem.DeleteFlg = items.DeleteFlg;
                        oItem.ActualRatio = items.ActualRatio;

                        commanddetailService.CreateCommandDetail(oItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                commanddetailModel.CommandDetail.InsertUser = HttpContext.User.Identity.Name;

                foreach (var items in mCommandDetail)
                {
                    var objCommandDetail = new MCommandDetail();
                    objCommandDetail = commanddetailService.FindCommandDetailById(items.ID);
                    objCommandDetail.V_Actual = items.V_Actual;
                    objCommandDetail.Flag = Constants.Command_Flag_StopPressing;
                    objCommandDetail.CommandFlag = Constants.COMMAND_FLAG_OLD;

                    commanddetailService.UpdateCommandDetail(objCommandDetail);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult DisplayCommandStatus(byte? status)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CONFIGARM);
            if (checkPermission)
            {

                //Lấy tất cả các xe có tất cả các ngăn flag = 99
                commanddetailModel.ListCommandDetail_Register = new List<CommandFollowStatus>();
                //Lấy tất cả các xe có tất cả các ngăn flag = 0
                commanddetailModel.ListCommandDetail_Approved = new List<CommandFollowStatus>();
                //lấy tất cả các xe trạng thái các ngăn <>0 và 1,2 //Đang xuất
                commanddetailModel.ListCommandDetail_Exporting = new List<CommandFollowStatus>();
                //tât cả các ngăn đang ở trạng thái 6 // Chờ niêm chì 
                commanddetailModel.ListCommandDetail_WaitSeal = new List<CommandFollowStatus>();
                //Tất cả các ngăn đang ở trạng thái 7  //Chờ lấy hóa đơn
                commanddetailModel.ListCommandDetail_Seal = new List<CommandFollowStatus>();
                //Đã xuất hóa đơn và rời khỏi kho
                commanddetailModel.ListCommandDetail_Invoice = new List<CommandFollowStatus>();
                //Lấy tất cả các ngăn đang ở trạng thái 3
                commanddetailModel.ListCommandDetail_Exported = new List<CommandFollowStatus>();
                int noDay = DateTime.Now.DayOfYear;
                int noYear = DateTime.Now.Year;
                commanddetailModel.ListCommandDetails = commanddetailService.GetAllCommandDetail().Where(x => (x.InsertDate.DayOfYear == noDay || x.UpdateDate.DayOfYear == noDay || x.TimeOrder.DayOfYear == noDay) && (x.InsertDate.Year == noYear || x.TimeOrder.Year == noYear)).ToList();

                if (commanddetailModel.ListCommandDetails.Count != 0)
                {
                    if (status == null)
                    {
                        commanddetailModel.ListCommandDetail_Register = ConvertCommandDetailToCommandFollowStatusRegister(commanddetailModel.ListCommandDetails);
                        commanddetailModel.ListCommandDetail_Approved = ConvertCommandDetailToCommandFollowStatusApproved(commanddetailModel.ListCommandDetails);
                        commanddetailModel.ListCommandDetail_Exporting = ConvertCommandDetailToCommandFollowStatusExporting(commanddetailModel.ListCommandDetails);
                        commanddetailModel.ListCommandDetail_WaitSeal = ConvertCommandDetailToCommandFollowStatusWailSeal(commanddetailModel.ListCommandDetails);
                        commanddetailModel.ListCommandDetail_Seal = ConvertCommandDetailToCommandFollowStatusSeal(commanddetailModel.ListCommandDetails);
                        commanddetailModel.ListCommandDetail_Invoice = ConvertCommandDetailToCommandFollowStatusInvoice(commanddetailModel.ListCommandDetails);
                        commanddetailModel.ListCommandDetail_Exported = ConvertCommandDetailToCommandFollowStatusExported(commanddetailModel.ListCommandDetails);
                    }
                    else if (status == Constants.Command_Flag_Register)
                        commanddetailModel.ListCommandDetail_Register = ConvertCommandDetailToCommandFollowStatusRegister(commanddetailModel.ListCommandDetails);
                    else if (status == Constants.Command_Flag_Approved)
                        commanddetailModel.ListCommandDetail_Approved = ConvertCommandDetailToCommandFollowStatusApproved(commanddetailModel.ListCommandDetails);
                    else if (status == Constants.Command_Flag_Exporting || status == Constants.Command_Flag_PrepareExport)
                        commanddetailModel.ListCommandDetail_Exporting = ConvertCommandDetailToCommandFollowStatusExporting(commanddetailModel.ListCommandDetails);
                    //else if (status == Constants.Command_Flag_StopPressing)
                    //	commanddetailModel.ListCommandDetail_WaitSeal = ConvertCommandDetailToCommandFollowStatusWailSeal(commanddetailModel.ListCommandDetails);
                    //else if (status == Constants.Command_Flag_InputHand || status == Constants.Command_Flag_Exported)
                    else if (status == Constants.Command_Flag_InputHand)
                            {
                        commanddetailModel.ListCommandDetail_WaitSeal = ConvertCommandDetailToCommandFollowStatusWailSeal(commanddetailModel.ListCommandDetails);
                    }
                    else if (status == Constants.Command_Flag_Seal)
                        commanddetailModel.ListCommandDetail_Seal = ConvertCommandDetailToCommandFollowStatusSeal(commanddetailModel.ListCommandDetails);
                    else if (status == Constants.Command_Flag_Invoice)
                        commanddetailModel.ListCommandDetail_Invoice = ConvertCommandDetailToCommandFollowStatusInvoice(commanddetailModel.ListCommandDetails);
                    else if (status == Constants.Command_Flag_Exported)
                        commanddetailModel.ListCommandDetail_Exported = ConvertCommandDetailToCommandFollowStatusExported(commanddetailModel.ListCommandDetails);
                }

                commanddetailModel.Status = status;
                commanddetailModel.ListCommand = CommandService.GetAllCommand().ToList();
                commanddetailModel.ListCustomer = customerService.GetAllCustomer().ToList();

                return View(commanddetailModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }

        }

        //Trường hợp có 1 trạng thái đăng ký lệnh
        public List<CommandFollowStatus> ConvertCommandDetailToCommandFollowStatusRegister(List<MCommandDetail> lstCommandDetail)
        {
            var lstCommandFollowStatus = new List<CommandFollowStatus>();
            var lstGroupCommandDetail = lstCommandDetail.Select(x => x.CommandID).Distinct();
            var intRow = 0;
            long cardSerial = 0;
            foreach (var item in lstGroupCommandDetail)
            {
                var itCommandFollowStatus1 = new CommandFollowStatus();
                bool chk = false;
                var obj = lstCommandDetail.Where(x => x.CommandID == item && x.Flag == Constants.Command_Flag_Register).FirstOrDefault();
                if (obj != null)
                {
                    var objcommand = CommandService.GetAllCommand().Where(x => x.CommandID == obj.CommandID).First();//Sử dụng để lấy Số phương tiện và tên lái xe
                    if (objcommand != null)
                    {
                        itCommandFollowStatus1.DriverName = objcommand.DriverName;
                        itCommandFollowStatus1.VehicleNumber = objcommand.VehicleNumber;
                        itCommandFollowStatus1.WorkOrder = obj.WorkOrder.ToString();
                    }

                    itCommandFollowStatus1.CommandID = obj.CommandID;
                    float VPreset = 0;
                    if (obj.V_Preset == null)
                    {
                        VPreset += 0;
                    }
                    else
                    {
                        VPreset += (float)obj.V_Preset;
                    }
                    float VActual = 0;
                    if (obj.V_Actual == null)
                    {
                        VActual += 0;
                    }
                    else
                    {
                        VActual += (float)obj.V_Actual;
                    }
                    itCommandFollowStatus1.V_Preset = VPreset.ToString();
                    itCommandFollowStatus1.V_Actual = VActual.ToString();

                    var lst = lstCommandDetail.Where(x => x.CommandID == item).ToList();
                   

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].Flag != Constants.Command_Flag_Register)
                        {
                            chk = true;
                            break;
                        }

                        switch (lst[i].CompartmentOrder)
                        {
                            case 1:
                                itCommandFollowStatus1.Volume1 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume1 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume1 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 2:
                                itCommandFollowStatus1.Volume2 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume2 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume2 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 3:
                                itCommandFollowStatus1.Volume3 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume3 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume3 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 4:
                                itCommandFollowStatus1.Volume4 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume4 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume4 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 5:
                                itCommandFollowStatus1.Volume5 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume5 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume5 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 6:
                                itCommandFollowStatus1.Volume6 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume6 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume6 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 7:
                                itCommandFollowStatus1.Volume7 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume7 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume7 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 8:
                                itCommandFollowStatus1.Volume8 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume8 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume8 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 9:
                                itCommandFollowStatus1.Volume9 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume9 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume9 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;

                        }
                        cardSerial = lst[i].CardSerial ?? 0;
                    }
                }
                
                if (!chk)
                {
                    intRow += 1;
                    itCommandFollowStatus1.No = intRow.ToString();
                    itCommandFollowStatus1.ProductName = @"Hàng hóa";
                    itCommandFollowStatus1.CardSerial = cardSerial.ToString();
                    lstCommandFollowStatus.Add(itCommandFollowStatus1);
                }
            }

            return lstCommandFollowStatus;
        }

        public List<CommandFollowStatus> ConvertCommandDetailToCommandFollowStatusApproved(List<MCommandDetail> lstCommandDetail)
        {
            var lstCommandFollowStatus = new List<CommandFollowStatus>();
            var lstGroupCommandDetail = lstCommandDetail.Select(x => x.CommandID).Distinct();
            var intRow = 0;
            long cardSerial = 0;
            foreach (var item in lstGroupCommandDetail)
            {
                bool chk = false;
                var itCommandFollowStatus1 = new CommandFollowStatus();

                var obj = lstCommandDetail.Where(x => x.CommandID == item && x.Flag == Constants.Command_Flag_Approved).FirstOrDefault();
                if (obj != null)
                {
                    var objcommand = CommandService.GetAllCommand().Where(x => x.CommandID == obj.CommandID).First();//Sử dụng để lấy Số phương tiện và tên lái xe
                    if (objcommand != null)
                    {
                        itCommandFollowStatus1.DriverName = objcommand.DriverName;
                        itCommandFollowStatus1.VehicleNumber = objcommand.VehicleNumber;
                        itCommandFollowStatus1.WorkOrder = obj.WorkOrder.ToString();
                    }

                    itCommandFollowStatus1.CommandID = obj.CommandID;
                    float VPreset = 0;
                    if (obj.V_Preset == null)
                    {
                        VPreset += 0;
                    }
                    else
                    {
                        VPreset += (float)obj.V_Preset;
                    }
                    float VActual = 0;
                    if (obj.V_Actual == null)
                    {
                        VActual += 0;
                    }
                    else
                    {
                        VActual += (float)obj.V_Actual;
                    }
                    itCommandFollowStatus1.V_Preset = VPreset.ToString();
                    itCommandFollowStatus1.V_Actual = VActual.ToString();

                    var lst = lstCommandDetail.Where(x => x.CommandID == item).ToList();
                    

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].Flag != Constants.Command_Flag_Approved)
                        {
                            chk = true;
                            break;
                        }

                        switch (lst[i].CompartmentOrder)
                        {
                            case 1:
                                itCommandFollowStatus1.Volume1 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume1 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume1 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 2:
                                itCommandFollowStatus1.Volume2 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume2 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume2 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 3:
                                itCommandFollowStatus1.Volume3 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume3 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume3 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 4:
                                itCommandFollowStatus1.Volume4 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume4 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume4 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 5:
                                itCommandFollowStatus1.Volume5 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume5 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume5 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 6:
                                itCommandFollowStatus1.Volume6 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume6 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume6 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 7:
                                itCommandFollowStatus1.Volume7 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume7 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume7 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 8:
                                itCommandFollowStatus1.Volume8 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume8 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume8 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 9:
                                itCommandFollowStatus1.Volume9 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume9 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume9 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;

                        }
                        cardSerial = lst[i].CardSerial ?? 0;
                    }
                }
                
                if (!chk)
                {
                    intRow += 1;
                    itCommandFollowStatus1.No = intRow.ToString();
                    itCommandFollowStatus1.ProductName = @"Hàng hóa";
                    itCommandFollowStatus1.CardSerial = cardSerial.ToString();
                    lstCommandFollowStatus.Add(itCommandFollowStatus1);
                }
            }

            return lstCommandFollowStatus;
        }

        public List<CommandFollowStatus> ConvertCommandDetailToCommandFollowStatusExporting(List<MCommandDetail> lstCommandDetail)
        {
            var lstCommandFollowStatus = new List<CommandFollowStatus>();
            var lstGroupCommandDetail = lstCommandDetail.Select(x => x.WorkOrder).Distinct();
            var intRow = 0;
            long cardSerial = 0;
            foreach (var item in lstGroupCommandDetail)
            {
                var itCommandFollowStatus1 = new CommandFollowStatus();
                bool chk = false;
                var obj = lstCommandDetail.Where(x => x.WorkOrder == item && (x.Flag == Constants.Command_Flag_PrepareExport || x.Flag == Constants.Command_Flag_Exporting)).FirstOrDefault();
                if (obj != null)
                {
                    var objcommand = CommandService.GetAllCommand().Where(x => x.WorkOrder == obj.WorkOrder).First();//Sử dụng để lấy Số phương tiện và tên lái xe
                    if (objcommand != null)
                    {
                        itCommandFollowStatus1.DriverName = objcommand.DriverName;
                        itCommandFollowStatus1.VehicleNumber = objcommand.VehicleNumber;
                        itCommandFollowStatus1.WorkOrder = obj.WorkOrder.ToString();

                    }
                    itCommandFollowStatus1.CommandID = obj.CommandID;
                    float VPreset = 0;
                    if (obj.V_Preset == null)
                    {
                        VPreset += 0;
                    }
                    else
                    {
                        VPreset += (float)obj.V_Preset;
                    }
                    float VActual = 0;
                    if (obj.V_Actual == null)
                    {
                        VActual += 0;
                    }
                    else
                    {
                        VActual += (float)obj.V_Actual;
                    }
                    itCommandFollowStatus1.V_Preset = VPreset.ToString();
                    itCommandFollowStatus1.V_Actual = VActual.ToString();

                    var lst = lstCommandDetail.Where(x => x.WorkOrder == item).ToList(); 

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].Flag == Constants.Command_Flag_PrepareExport || lst[i].Flag == Constants.Command_Flag_Exporting)
                        {
                            chk = true;
                        }

                        switch (lst[i].CompartmentOrder)
                        {
                            case 1:
                                itCommandFollowStatus1.Volume1 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume1 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume1 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 2:
                                itCommandFollowStatus1.Volume2 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume2 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume2 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 3:
                                itCommandFollowStatus1.Volume3 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume3 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume3 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 4:
                                itCommandFollowStatus1.Volume4 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume4 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume4 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 5:
                                itCommandFollowStatus1.Volume5 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume5 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume5 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 6:
                                itCommandFollowStatus1.Volume6 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume6 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume6 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 7:
                                itCommandFollowStatus1.Volume7 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume7 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume7 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 8:
                                itCommandFollowStatus1.Volume8 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume8 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume8 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 9:
                                itCommandFollowStatus1.Volume9 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume9 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume9 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;

                        }
                        cardSerial = lst[i].CardSerial ?? 0;
                    }
                }
                
                if (chk)
                {
                    intRow += 1;
                    itCommandFollowStatus1.No = intRow.ToString();
                    itCommandFollowStatus1.ProductName = @"Hàng hóa";
                    itCommandFollowStatus1.CardSerial = cardSerial.ToString();
                    lstCommandFollowStatus.Add(itCommandFollowStatus1);
                }
            }

            return lstCommandFollowStatus;
        }

        public List<CommandFollowStatus> ConvertCommandDetailToCommandFollowStatusWailSeal(List<MCommandDetail> lstCommandDetail)
        {
            var lstCommandFollowStatus = new List<CommandFollowStatus>();
            var lstGroupCommandDetail = lstCommandDetail.Select(x => x.CommandID).Distinct();
            var intRow = 0;
            long cardSerial = 0;
            foreach (var item in lstGroupCommandDetail)
            {
                var itCommandFollowStatus1 = new CommandFollowStatus();
                bool chk = true;
                //var obj = lstCommandDetail.Where(x => x.CommandID == item && (x.Flag == Constants.Command_Flag_Exported || x.Flag == Constants.Command_Flag_InputHand || x.Flag == Constants.Command_Flag_StopPressing)).FirstOrDefault();
                var obj = lstCommandDetail.Where(x => x.CommandID == item && (x.Flag == Constants.Command_Flag_InputHand || x.Flag == Constants.Command_Flag_StopPressing)).FirstOrDefault();
                if (obj != null)
                {
                    var objcommand = CommandService.GetAllCommand().Where(x => x.CommandID == obj.CommandID).First();//Sử dụng để lấy Số phương tiện và tên lái xe
                    if (objcommand != null)
                    {
                        itCommandFollowStatus1.DriverName = objcommand.DriverName;
                        itCommandFollowStatus1.VehicleNumber = objcommand.VehicleNumber;
                        itCommandFollowStatus1.WorkOrder = obj.WorkOrder.ToString();
                    }
                    itCommandFollowStatus1.CommandID = obj.CommandID;
                    float VPreset = 0;
                    if (obj.V_Preset == null)
                    {
                        VPreset += 0;
                    }
                    else
                    {
                        VPreset += (float)obj.V_Preset;
                    }
                    float VActual = 0;
                    if (obj.V_Actual == null)
                    {
                        VActual += 0;
                    }
                    else
                    {
                        VActual += (float)obj.V_Actual;
                    }
                    itCommandFollowStatus1.V_Preset = VPreset.ToString();
                    itCommandFollowStatus1.V_Actual = VActual.ToString();

                    var lst = lstCommandDetail.Where(x => x.CommandID == item).ToList(); 

                    for (int i = 0; i < lst.Count; i++)
                    {
                        //if (lst[i].Flag == Constants.Command_Flag_Exported || lst[i].Flag == Constants.Command_Flag_InputHand || lst[i].Flag == Constants.Command_Flag_StopPressing)
                        if (lst[i].Flag == Constants.Command_Flag_InputHand || lst[i].Flag == Constants.Command_Flag_StopPressing)
                        {
                            //chk = true;
                        }
                        else
                        {
                            chk = false;
                        }



                        switch (lst[i].CompartmentOrder)
                        {
                            case 1:
                                itCommandFollowStatus1.Volume1 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume1 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume1 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 2:
                                itCommandFollowStatus1.Volume2 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume2 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume2 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 3:
                                itCommandFollowStatus1.Volume3 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume3 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume3 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 4:
                                itCommandFollowStatus1.Volume4 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume4 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume4 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 5:
                                itCommandFollowStatus1.Volume5 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume5 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume5 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 6:
                                itCommandFollowStatus1.Volume6 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume6 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume6 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 7:
                                itCommandFollowStatus1.Volume7 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume7 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume7 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 8:
                                itCommandFollowStatus1.Volume8 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume8 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume8 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 9:
                                itCommandFollowStatus1.Volume9 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume9 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume9 += "(" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;

                        }
                        cardSerial = lst[i].CardSerial ?? 0;
                    }
                }
                
                if (chk)
                {
                    intRow += 1;
                    itCommandFollowStatus1.No = intRow.ToString();
                    itCommandFollowStatus1.ProductName = @"Hàng hóa";
                    itCommandFollowStatus1.CardSerial = cardSerial.ToString();
                    lstCommandFollowStatus.Add(itCommandFollowStatus1);
                }
            }

            return lstCommandFollowStatus;
        }

        public List<CommandFollowStatus> ConvertCommandDetailToCommandFollowStatusSeal(List<MCommandDetail> lstCommandDetail)
        {
            var lstCommandFollowStatus = new List<CommandFollowStatus>();
            var lstGroupCommandDetail = lstCommandDetail.Select(x => x.CommandID).Distinct();
            var intRow = 0;
            long cardSerial = 0;
            foreach (var item in lstGroupCommandDetail)
            {
                var itCommandFollowStatus1 = new CommandFollowStatus();
                bool chk = false;
                var obj = lstCommandDetail.Where(x => x.CommandID == item && x.Flag == Constants.Command_Flag_Seal).FirstOrDefault();
                if (obj != null)
                {
                    var objcommand = CommandService.GetAllCommand().Where(x => x.CommandID == obj.CommandID).First();//Sử dụng để lấy Số phương tiện và tên lái xe
                    if (objcommand != null)
                    {
                        itCommandFollowStatus1.DriverName = objcommand.DriverName;
                        itCommandFollowStatus1.VehicleNumber = objcommand.VehicleNumber;
                        itCommandFollowStatus1.WorkOrder = obj.WorkOrder.ToString();
                    }
                    itCommandFollowStatus1.CommandID = obj.CommandID;
                    float VPreset = 0;
                    if (obj.V_Preset == null)
                    {
                        VPreset += 0;
                    }
                    else
                    {
                        VPreset += (float)obj.V_Preset;
                    }
                    float VActual = 0;
                    if (obj.V_Actual == null)
                    {
                        VActual += 0;
                    }
                    else
                    {
                        VActual += (float)obj.V_Actual;
                    }
                    itCommandFollowStatus1.V_Preset = VPreset.ToString();
                    itCommandFollowStatus1.V_Actual = VActual.ToString();

                    var lst = lstCommandDetail.Where(x => x.CommandID == item).ToList(); 

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].Flag != Constants.Command_Flag_Seal)
                        {
                            chk = true;
                            break;
                        }

                        switch (lst[i].CompartmentOrder)
                        {
                            case 1:
                                itCommandFollowStatus1.Volume1 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume1 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume1 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 2:
                                itCommandFollowStatus1.Volume2 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume2 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume2 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 3:
                                itCommandFollowStatus1.Volume3 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume3 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume3 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 4:
                                itCommandFollowStatus1.Volume4 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume4 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume4 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 5:
                                itCommandFollowStatus1.Volume5 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume5 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume5 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 6:
                                itCommandFollowStatus1.Volume6 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume6 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume6 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 7:
                                itCommandFollowStatus1.Volume7 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume7 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume7 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 8:
                                itCommandFollowStatus1.Volume8 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume8 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume8 += "(" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 9:
                                itCommandFollowStatus1.Volume9 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume9 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume9 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;

                        }
                        cardSerial = lst[i].CardSerial ?? 0;
                    }
                }
                
                if (!chk)
                {
                    intRow += 1;
                    itCommandFollowStatus1.No = intRow.ToString();
                    itCommandFollowStatus1.ProductName = @"Hàng hóa";
                    itCommandFollowStatus1.CardSerial = cardSerial.ToString();
                    lstCommandFollowStatus.Add(itCommandFollowStatus1);
                }
            }

            return lstCommandFollowStatus;
        }

        public List<CommandFollowStatus> ConvertCommandDetailToCommandFollowStatusInvoice(List<MCommandDetail> lstCommandDetail)
        {
            var lstCommandFollowStatus = new List<CommandFollowStatus>();
            var lstGroupCommandDetail = lstCommandDetail.Select(x => x.CommandID).Distinct();
            long cardSerial = 0;
            var intRow = 0;
            foreach (var item in lstGroupCommandDetail)
            {
                var itCommandFollowStatus1 = new CommandFollowStatus();
                bool chk = false;
                var obj = lstCommandDetail.Where(x => x.CommandID == item && x.Flag == Constants.Command_Flag_Invoice).FirstOrDefault();
                if (obj != null)
                {
                    var objcommand = CommandService.GetAllCommand().Where(x => x.CommandID == obj.CommandID).First();//Sử dụng để lấy Số phương tiện và tên lái xe
                    if (objcommand != null)
                    {
                        itCommandFollowStatus1.DriverName = objcommand.DriverName;
                        itCommandFollowStatus1.VehicleNumber = objcommand.VehicleNumber;
                        itCommandFollowStatus1.WorkOrder = obj.WorkOrder.ToString();
                    }
                    itCommandFollowStatus1.CommandID = obj.CommandID;
                    float VPreset = 0;
                    if (obj.V_Preset == null)
                    {
                        VPreset += 0;
                    }
                    else
                    {
                        VPreset += (float)obj.V_Preset;
                    }
                    float VActual = 0;
                    if (obj.V_Actual == null)
                    {
                        VActual += 0;
                    }
                    else
                    {
                        VActual += (float)obj.V_Actual;
                    }
                    itCommandFollowStatus1.V_Preset = VPreset.ToString();
                    itCommandFollowStatus1.V_Actual = VActual.ToString();

                    var lst = lstCommandDetail.Where(x => x.CommandID == item).ToList();
                    
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].Flag != Constants.Command_Flag_Invoice)
                        {
                            chk = true;
                            break;
                        }

                        switch (lst[i].CompartmentOrder)
                        {
                            case 1:
                                itCommandFollowStatus1.Volume1 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume1 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume1 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 2:
                                itCommandFollowStatus1.Volume2 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume2 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume2 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 3:
                                itCommandFollowStatus1.Volume3 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume3 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume3 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 4:
                                itCommandFollowStatus1.Volume4 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume4 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume4 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 5:
                                itCommandFollowStatus1.Volume5 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume5 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume5 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 6:
                                itCommandFollowStatus1.Volume6 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume6 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume6 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 7:
                                itCommandFollowStatus1.Volume7 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume7 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume7 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 8:
                                itCommandFollowStatus1.Volume8 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume8 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume8 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 9:
                                itCommandFollowStatus1.Volume9 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume9 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume9 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;

                        }
                        cardSerial = lst[i].CardSerial ?? 0;
                    }
                }
                
                if (!chk)
                {
                    intRow += 1;
                    itCommandFollowStatus1.No = intRow.ToString();
                    itCommandFollowStatus1.ProductName = @"Hàng hóa";
                    //itCommandFollowStatus1.CardSerial = item.ToString();
                    itCommandFollowStatus1.CardSerial = cardSerial.ToString();
                    lstCommandFollowStatus.Add(itCommandFollowStatus1);
                }
            }

            return lstCommandFollowStatus;
        }
        public List<CommandFollowStatus> ConvertCommandDetailToCommandFollowStatusExported(List<MCommandDetail> lstCommandDetail)
        {
            var lstCommandFollowStatus = new List<CommandFollowStatus>();
            var lstGroupCommandDetail = lstCommandDetail.Select(x => x.WorkOrder).Distinct();
            var intRow = 0;
            long cardSerial = 0;
            foreach (var item in lstGroupCommandDetail)
            {
                var itCommandFollowStatus1 = new CommandFollowStatus();
                bool chk = false;
                var obj = lstCommandDetail.Where(x => x.WorkOrder == item && (x.Flag == Constants.Command_Flag_Exported)).FirstOrDefault();
                if (obj != null)
                {
                    var objcommand = CommandService.GetAllCommand().Where(x => x.WorkOrder == obj.WorkOrder).First();//Sử dụng để lấy Số phương tiện và tên lái xe
                    if (objcommand != null)
                    {
                        itCommandFollowStatus1.DriverName = objcommand.DriverName;
                        itCommandFollowStatus1.VehicleNumber = objcommand.VehicleNumber;
                        itCommandFollowStatus1.WorkOrder = obj.WorkOrder.ToString();

                    }
                    itCommandFollowStatus1.CommandID = obj.CommandID;
                    float VPreset = 0;
                    if (obj.V_Preset == null)
                    {
                        VPreset += 0;
                    }
                    else
                    {
                        VPreset += (float)obj.V_Preset;
                    }
                    float VActual = 0;
                    if (obj.V_Actual == null)
                    {
                        VActual += 0;
                    }
                    else
                    {
                        VActual += (float)obj.V_Actual;
                    }
                    itCommandFollowStatus1.V_Preset = VPreset.ToString();
                    itCommandFollowStatus1.V_Actual = VActual.ToString();

                    var lst = lstCommandDetail.Where(x => x.WorkOrder == item).ToList();
                    

                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i].Flag == Constants.Command_Flag_Exported)
                        {
                            chk = true;
                        }

                        switch (lst[i].CompartmentOrder)
                        {
                            case 1:
                                itCommandFollowStatus1.Volume1 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume1 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume1 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 2:
                                itCommandFollowStatus1.Volume2 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume2 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume2 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 3:
                                itCommandFollowStatus1.Volume3 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume3 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume3 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 4:
                                itCommandFollowStatus1.Volume4 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume4 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume4 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 5:
                                itCommandFollowStatus1.Volume5 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume5 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume5 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 6:
                                itCommandFollowStatus1.Volume6 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume6 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume6 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 7:
                                itCommandFollowStatus1.Volume7 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume7 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume7 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 8:
                                itCommandFollowStatus1.Volume8 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume8 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume8 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;
                            case 9:
                                itCommandFollowStatus1.Volume9 = lst[i].ProductName;
                                if (lst[i].V_Actual == null)
                                    itCommandFollowStatus1.Volume9 += " (0)";
                                else
                                    itCommandFollowStatus1.Volume9 += " (" + ((float)lst[i].V_Actual).ToString() + ")";
                                break;

                        }
                        cardSerial = lst[i].CardSerial ?? 0;
                    }
                }
                
                if (chk)
                {
                    intRow += 1;
                    itCommandFollowStatus1.No = intRow.ToString();
                    itCommandFollowStatus1.ProductName = @"Hàng hóa";
                    itCommandFollowStatus1.CardSerial = cardSerial.ToString();
                    lstCommandFollowStatus.Add(itCommandFollowStatus1);
                }
            }

            return lstCommandFollowStatus;
        }

        public ActionResult CommanddetailbyVehicle(int? page, string search)
        {


            int pageNumber = (page ?? 1);
            if (search == null || search == "")
            {
                commanddetailModel.ListCommandDetail = commanddetailService.GetAllCommandDetail().ToPagedList(pageNumber, Constants.PAGE_SIZE);
            }
            else
            {
                commanddetailModel.ListCommandDetail = commanddetailService.GetAllCommandDetail().Where(x => x.ProductName.Contains(search)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
            }

            return View(commanddetailModel);
        }


        public ActionResult Index(CommandDetailModel commanddetailModel, int? page)
        {
            String userName = HttpContext.User.Identity.Name;
            commanddetailModel.WorkOrder = commanddetailModel.WorkOrder ?? "";
            commanddetailModel.CardSerial = commanddetailModel.CardSerial ?? "";
            commanddetailModel.CardData = commanddetailModel.CardData ?? "";
            commanddetailModel.VerhicleNumber = commanddetailModel.VerhicleNumber ?? "";
            commanddetailModel.ListVehicle = VehicleService.GetAllVehicle().ToList();
            commanddetailModel.ListConfigArm = configArmService.GetAllConfigArmOrderByName().ToList();
            commanddetailModel.ListCustomer = customerService.GetAllCustomer().ToList();
            commanddetailModel.CertificateNumber = commanddetailModel.CertificateNumber ?? "";
            int pageNumber = (page ?? 1);

            DateTime? StartDate = null;
            DateTime? EndDate = null;
            if (!string.IsNullOrEmpty(commanddetailModel.StartDate))
            {
                StartDate = DateTime.ParseExact(commanddetailModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(commanddetailModel.EndDate))
            {
                EndDate = DateTime.ParseExact(commanddetailModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            byte flag = commanddetailModel.Status ?? 100;
            byte armno = commanddetailModel.ArmNo ?? 100;
            commanddetailModel.ListCommand = CommandService.GetAllCommand().ToList();
            commanddetailModel.ListCommandDetail = commanddetailService.GetAllCommandDetail().Where(x => (x.CertificateNumber.ToString().Contains(commanddetailModel.CertificateNumber) || commanddetailModel.CertificateNumber == "")
            && (x.Flag == flag || flag == 100)
            && (x.CardSerial.ToString().Contains(commanddetailModel.CardSerial) || commanddetailModel.CardSerial == "")
            && (x.Vehicle.Contains(commanddetailModel.VerhicleNumber) || commanddetailModel.VerhicleNumber == "")
            && (commanddetailModel.CardData == "" || x.CardData.Contains(commanddetailModel.CardData))
            && (x.TimeOrder >= StartDate && x.TimeOrder <= EndDate)
            && (x.ArmNo == armno || armno == 100)
            ).ToPagedList(pageNumber, Constants.PAGE_SIZE);

            return View(commanddetailModel);
        }

        public ActionResult UpdateFlag(CommandDetailModel commanddetailModel, int? page)
        {
            String userName = HttpContext.User.Identity.Name;
            commanddetailModel.WorkOrder = commanddetailModel.WorkOrder ?? "";
            commanddetailModel.CardSerial = commanddetailModel.CardSerial ?? "";
            commanddetailModel.CardData = commanddetailModel.CardData ?? "";
            commanddetailModel.VerhicleNumber = commanddetailModel.VerhicleNumber ?? "";
            commanddetailModel.ListVehicle = VehicleService.GetAllVehicle().ToList();
            commanddetailModel.ListConfigArm = configArmService.GetAllConfigArmOrderByName().ToList();
            commanddetailModel.ListCustomer = customerService.GetAllCustomer().ToList();
            commanddetailModel.CertificateNumber = commanddetailModel.CertificateNumber ?? "";
            int pageNumber = (page ?? 1);

            DateTime? StartDate = null;
            DateTime? EndDate = null;
            if (!string.IsNullOrEmpty(commanddetailModel.StartDate))
            {
                StartDate = DateTime.ParseExact(commanddetailModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(commanddetailModel.EndDate))
            {
                EndDate = DateTime.ParseExact(commanddetailModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            }

            byte flag = commanddetailModel.Status ?? 100;
            byte armno = commanddetailModel.ArmNo ?? 100;
            commanddetailModel.ListCommand = CommandService.GetAllCommand().ToList();
            commanddetailModel.ListCommandDetail = commanddetailService.GetAllCommandDetail().Where(x => (x.CertificateNumber.ToString().Contains(commanddetailModel.CertificateNumber) || commanddetailModel.CertificateNumber == "")
            && (x.Flag == 2)
            && (x.CardSerial.ToString().Contains(commanddetailModel.CardSerial) || commanddetailModel.CardSerial == "")
            && (x.Vehicle.Contains(commanddetailModel.VerhicleNumber) || commanddetailModel.VerhicleNumber == "")
            && (commanddetailModel.CardData == "" || x.CardData.Contains(commanddetailModel.CardData))
            && (x.TimeOrder >= StartDate && x.TimeOrder <= EndDate)
            && (x.ArmNo == armno || armno == 100)
            ).ToPagedList(pageNumber, Constants.PAGE_SIZE);

            return View(commanddetailModel);
        }

        public ActionResult Edit(int id)
        {
            commanddetailModel.CommandDetail = commanddetailService.FindCommandDetailById(id);
            return View(commanddetailModel);
        }

        [HttpPost]
        public ActionResult Edit(List<String> lstCommandId)
        {

            if (lstCommandId != null)
            {
                foreach (var commandid in lstCommandId)
                {
                    var intCommandId = Convert.ToInt32(commandid);
                    CommandService.CancelCommand(intCommandId, HttpContext.User.Identity.Name);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Approve(List<String> lstCommandId)
        {

            if (lstCommandId != null)
            {
                foreach (var commandid in lstCommandId)
                {
                    var intCommandId = Convert.ToInt32(commandid);
                    if(CommandService.CheckCommandID(intCommandId))
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_ERR_APPROVE;
                    }   
                    else
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_OK_APPROVE;
                        CommandService.ApproveCommand(intCommandId, HttpContext.User.Identity.Name);
                    }    
                        
                }
            }

            return RedirectToAction("ApproveCommand", "Command");
        }

        public string ApproveV2(List<string> lstCommandId)
        {
            string str = "";
            if (lstCommandId != null)
            {
                foreach (var commandid in lstCommandId)
                {
                    var intCommandId = Convert.ToInt32(commandid);
                    if (CommandService.CheckCommandID(intCommandId))
                    { 
                        str = Constants.MESSAGE_ALERT_ERR_APPROVE;
                    }
                    else
                    { 
                        str = Constants.MESSAGE_ALERT_OK_APPROVE;
                        CommandService.ApproveCommand(intCommandId, HttpContext.User.Identity.Name);
                    }

                }
            }

            return str;
        }

        [HttpPost]
        public ActionResult UnApprove(List<String> lstCommandId)
        {

            if (lstCommandId != null)
            {
                foreach (var commandid in lstCommandId)
                {
                    var intCommandId = Convert.ToInt32(commandid);
                    CommandService.UnApproveCommand(intCommandId, HttpContext.User.Identity.Name);
                }
            }

            return RedirectToAction("Index");
        }

        public string UnApproveV2(List<string> lstCommandId)
        {
            string str = "";
            if (lstCommandId != null)
            {
                foreach (var commandid in lstCommandId)
                {
                    var intCommandId = Convert.ToInt32(commandid);
                    CommandService.UnApproveCommand(intCommandId, HttpContext.User.Identity.Name);
                    str = Constants.MESSAGE_ALERT_OK_UNAPPROVE;
                }
            }

            return str;
        }

        public string POSFlag(List<string> lstId)
        {
            string str = "";
            if (lstId != null)
            {
                foreach (var id in lstId)
                {
                    var intCommandId = Convert.ToInt32(id);
                    CommandService.UpdateFlag(intCommandId, HttpContext.User.Identity.Name);
                    str = Constants.MESSAGE_ALERT_OK_UNAPPROVE;
                }
            }

            return str;
        }

        [HttpPost]
        public ActionResult CancelCompartment(List<String> lstId)
        {

            if (lstId != null)
            {
                foreach (var id in lstId)
                {
                    var intId = Convert.ToInt32(id);
                    CommandService.CancelCommandDetail(intId, HttpContext.User.Identity.Name);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(commanddetailModel);
        }



        [HttpPost]
        public ActionResult Create(List<String> lstCommandId)
        {

            if (lstCommandId != null)
            {
                foreach (var commandid in lstCommandId)
                {

                    var intCommandId = Convert.ToInt32(commandid);
                    var objCommand = CommandService.FindCommandById(intCommandId);
                    var timecommand = objCommand.TimeOrder;
                    var timescommand = Convert.ToDateTime(timecommand);
                    var daycommand = (timescommand.AddDays(+1)).ToShortDateString();
                    var dayscommand = Convert.ToDateTime(daycommand);
                    var timeordercommand = dayscommand.Add(new TimeSpan(1, 0, 0));
                    CommandService.UpdateCommandChangeDate(intCommandId, timeordercommand, HttpContext.User.Identity.Name);
                }

            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var rs = commanddetailService.DeleteCommandDetail(id);
            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
            }

            return RedirectToAction("Index");
        }

        public ActionResult CancelCommandByCompartment(CommandDetailModel commanddetailModel, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CANCELCOMMAND);
            if (checkPermission)
            {

                String userName = HttpContext.User.Identity.Name;
                commanddetailModel.WorkOrder = commanddetailModel.WorkOrder ?? "";
                commanddetailModel.CardSerial = commanddetailModel.CardSerial ?? "";
                commanddetailModel.CardData = commanddetailModel.CardData ?? "";
                commanddetailModel.VerhicleNumber = commanddetailModel.VerhicleNumber ?? "";
                commanddetailModel.ListVehicle = VehicleService.GetAllVehicle().ToList();
                int pageNumber = (page ?? 1);

                DateTime? StartDate = null;
                DateTime? EndDate = null;
                if (!string.IsNullOrEmpty(commanddetailModel.StartDate))
                {
                    StartDate = DateTime.ParseExact(commanddetailModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(commanddetailModel.EndDate))
                {
                    EndDate = DateTime.ParseExact(commanddetailModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                }

                byte flag = commanddetailModel.Status ?? 100;
                commanddetailModel.ListCommandDetail = commanddetailService.GetAllCommandDetail().Where(x => (x.WorkOrder.ToString().Contains(commanddetailModel.WorkOrder) || commanddetailModel.WorkOrder == "")
                && (x.Flag == flag || flag == 100)
                && (x.CardSerial.ToString().Contains(commanddetailModel.CardSerial) || commanddetailModel.CardSerial == "")
                && (x.Vehicle.Contains(commanddetailModel.VerhicleNumber) || commanddetailModel.VerhicleNumber == "")
                //&& (x.CardData.ToString().Contains(commanddetailModel.CardData) || commanddetailModel.CardData == "")
                && (x.TimeOrder >= StartDate && x.TimeOrder <= EndDate)
                ).ToPagedList(pageNumber, Constants.PAGE_SIZE);

                return View(commanddetailModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }

        }
    }
}