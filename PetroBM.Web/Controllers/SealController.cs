using PagedList;
using PetroBM.Common.Util;
using PetroBM.Domain.Entities;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace PetroBM.Web.Controllers
{
    public class SealController : BaseController
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(SealController));

        private readonly ISealService sealService;
        public SealModel sealModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly ICommandService commandService;
        private readonly ICommandDetailService commandDetailService;
        private readonly ICustomerService customerService;
        private readonly ICardService cardSevice;
        private readonly IProductService productService;
        private readonly IEventService eventService;
        private readonly CommandDetailModel commanddetailModel;
        private readonly BaseService baseService;

        public SealController(ISealService sealService, SealModel sealModel, CommandDetailModel commanddetailModel, IConfigurationService configurationService, IUserService userService, ICommandService commandService, ICustomerService customerService, ICommandDetailService commandDetailService, ICardService cardSevice, IProductService productService, IEventService eventService, BaseService baseService)
        {
            this.sealService = sealService;
            this.sealModel = sealModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.commandService = commandService;
            this.commandDetailService = commandDetailService;
            this.customerService = customerService;
            this.cardSevice = cardSevice;
            this.commanddetailModel = commanddetailModel;
            this.productService = productService;
            this.eventService = eventService;
            this.baseService = baseService;
        }

        public ActionResult GetCommanDetailByCardData(string cardDataCommandDetail)
        {
            try
            {
                var obj = commandDetailService.GetAllCommandDetailByCardData(cardDataCommandDetail).ToList();
                if (obj != null)
                {
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);

                return null;

            }
        }

        public ActionResult GetCommanDetailByCommandID(int commandID)
        {
            try
            {
                log.Info("Get CommandDetail By CommandID");
                var obj = commandDetailService.GetAllCommandDetailByCommandID(commandID)
                    .Where(c => (c.Flag == Constants.Command_Flag_Exported || c.Flag == Constants.Command_Flag_StopPressing
                    || c.Flag == Constants.Command_Flag_InputHand || c.Flag == Constants.Command_Flag_Seal) && c.CommandFlag != Constants.COMMAND_FLAG_NEW).OrderBy(x => x.CompartmentOrder).ToList();

                for (int i = obj.Count - 1; i >= 0; i--)
                {
                    var item = obj[i];

                    if (item.Flag == Constants.Command_Flag_StopPressing)
                    {
                        var newCommand = commandDetailService.GetAllCommandDetailByCommandID(commandID)
                            .Where(c => (c.Flag == Constants.Command_Flag_Exported || c.Flag == Constants.Command_Flag_InputHand)
                            && c.CommandFlag == Constants.COMMAND_FLAG_NEW && c.CompartmentOrder == item.CompartmentOrder).ToList();
                        if (newCommand.Count == 0)
                        {
                            obj.RemoveAt(i);
                        }
                    }
                }

                //for (int i = 0; i <= obj.Count - 1; i++)
                //{
                //    var item = obj[i];

                //    if (item.Flag == Constants.Command_Flag_StopPressing)
                //    {
                //        var newCommand = commandDetailService.GetAllCommandDetailByCommandID(commandID)
                //            .Where(c => (c.Flag == Constants.Command_Flag_Exported || c.Flag == Constants.Command_Flag_InputHand)
                //            && c.CommandFlag == Constants.COMMAND_FLAG_NEW && c.CompartmentOrder == item.CompartmentOrder).ToList();
                //        if (newCommand.Count == 0)
                //        {
                //            obj.RemoveAt(i);
                //        }
                //    }
                //}

                if (obj != null)
                {
                    return Json(obj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public ActionResult GetCardSerialByCardData(string cardData)
        {
            try
            {
                log.Info("Get CardSerial by CardData!");
                var obj = commandService.GetAllCommandByname(cardData).ToList();
                return Json(obj, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public ActionResult GetCardDataByCardSerial(long cardSerial)
        {
            var warehousecode = Convert.ToByte((Session[Constants.Session_WareHouse]).ToString());
            try
            {
                log.Info("Get CardData by CardSerial");
                MCommandDetail mCommandDetail = commandDetailService.GetAllCommandDetailByCardSerial(cardSerial)
                    .Where(c => c.Flag == Constants.Command_Flag_Exported || c.Flag == Constants.Command_Flag_InputHand || c.Flag == Constants.Command_Flag_StopPressing
                    && c.WareHouseCode == warehousecode).OrderBy(c => c.ID).Take(1).FirstOrDefault();
                if (mCommandDetail != null)
                {
                    var obj = commandService.FindCommandById(mCommandDetail.CommandID);
                    //var obj = commandDetailService.GetAllCommandDetailByCommandID(mCommandDetail.CommandID);
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    return Json(obj, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    return Json("", JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public ActionResult GetCardDataByWorkOrder(long commandeCode)
        {
            var warehousecode = Convert.ToByte((Session[Constants.Session_WareHouse]).ToString());
            try
            {
                log.Info("Get CardData by WorkOrder");
                MCommandDetail mCommandDetail = commandDetailService.GetAllCommandDetailByWorkOrder(commandeCode)
                    .Where(c => c.Flag == Constants.Command_Flag_Exported || c.Flag == Constants.Command_Flag_InputHand || c.Flag == Constants.Command_Flag_Seal

                    && c.WareHouseCode == warehousecode).OrderBy(c => c.ID).Take(1).FirstOrDefault();
                if (mCommandDetail != null)
                {
                    var obj = commandService.FindCommandById(mCommandDetail.CommandID);
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    return Json(obj, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    return Json("", JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public ActionResult GetAllCustomer(string customercode)
        {
            try
            {
                log.Info("Get All Customer");
                var obj = customerService.GetCustomerCode(customercode).ToList();
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public ActionResult GetAllProduct()
        {
            var obj = productService.GetAllProduct().ToList();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int? page, string search)
        {
            int pageNumber = (page ?? 1);
            if (search == null || search == "")
            {
                sealModel.ListSeal = sealService.GetAllSeal().ToPagedList(pageNumber, Constants.PAGE_SIZE);

            }
            else
            {
                sealModel.ListSeal = sealService.GetAllSeal().Where(x => x.V_Actual.Contains(search)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
            }
            sealModel.ListCommand = commandService.GetAllCommand().ToList();

            return View(sealModel);
        }

        [HttpGet]
        public ActionResult RegisterSeal()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_REGISTERSEAL);
            if (checkPermission)
            {
                sealModel.ListCommand = commandService.GetAllCommand().ToList();
                sealModel.ExportMode = Int32.Parse(configurationService.GetConfiguration(Constants.CONFIG_EXPORT_MODE).Value);
                var lstP = new List<Datum>();
                var lst = productService.GetAllProduct();

                foreach (var item in lst)
                {
                    var datum = new Datum();
                    datum.name = item.ProductName;
                    datum.type = item.ProductCode;
                    lstP.Add(datum);
                }
                sealModel.ListProduct = lstP;
                sealModel.Seal.ID = 0;
                return View(sealModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }

        }

        [HttpPost]
        public ActionResult RegisterSeal(IEnumerable<CSealModel> mSeal)
        {
            Decimal? workOrder;
            bool check = false;

            try
            {
                log.Info("Register Seal!");

                sealModel.Seal.InsertUser = HttpContext.User.Identity.Name;
                foreach (var item in mSeal)
                {
                    if (item.Seal1.Length > 0 && commandDetailService.CheckFlagBeforeCreateSeal(item.WorkOrder, item.CompartmentOrder) == false)
                    {
                        var oItem = new MSeal();
                        oItem.CommandID = item.CommandID;
                        oItem.V_Actual = item.V_Actual;
                        oItem.ProductName = item.ProductName;
                        oItem.CompartmentOrder = item.CompartmentOrder;
                        oItem.Vtt = item.Vtt;
                        oItem.Seal1 = item.Seal1;
                        oItem.Seal2 = item.Seal2;
                        oItem.Ratio = item.Ratio;
                        oItem.CardData = item.CardData;
                        oItem.CardSerial = item.CardSerial;
                        oItem.TimeOrder = item.TimeOrder;
                        oItem.WorkOrder = item.WorkOrder;
                        oItem.InsertUser = sealModel.Seal.InsertUser;
                        sealService.CreateSeal(oItem);
                        check = true;
                    }
                }


                if (check)
                {
                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                        Constants.EVENT_CONFIG_SEAL_CREATE, sealModel.Seal.InsertUser);
                    log.Info("Seal Susscess!");
                }


            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                if (check)
                {
                    foreach (var item in mSeal)
                    {
                        var flagcommand = item.Flag;
                        var workorder = item.WorkOrder;
                        var intflagcommand = (int)(flagcommand);
                        var obj = commandDetailService.GetAllCommandDetailByFlag(intflagcommand)
                                .Where(c => c.CommandFlag != Constants.COMMAND_FLAG_NEW && c.WorkOrder == workorder).ToList();

                        foreach (var items in obj)
                        {
                            var objCommandDetail = new MCommandDetail();
                            objCommandDetail = commandDetailService.FindCommandDetailById(items.ID);
                            //objCommandDetail.CommandID = items.CommandID;
                            //objCommandDetail.CommandType = items.CommandType;
                            //objCommandDetail.TimeOrder = items.TimeOrder;
                            //objCommandDetail.TimeStart = items.TimeStart;
                            //objCommandDetail.TimeStop = items.TimeStop;
                            //objCommandDetail.WorkOrder = items.WorkOrder;
                            //objCommandDetail.CompartmentOrder = items.CompartmentOrder;
                            //objCommandDetail.ProductCode = items.ProductCode;
                            //objCommandDetail.ProductName = items.ProductName;
                            //objCommandDetail.ArmNo = items.ArmNo;
                            //objCommandDetail.WareHouseCode = items.WareHouseCode;
                            //objCommandDetail.CardData = items.CardData;
                            //objCommandDetail.CardSerial = items.CardSerial;
                            //objCommandDetail.V_Preset = items.V_Preset;
                            //objCommandDetail.V_Actual = items.V_Actual;
                            //objCommandDetail.V_Deviation = items.V_Deviation;
                            //objCommandDetail.AvgTemperature = items.AvgTemperature;
                            //objCommandDetail.CurrentTemperature = items.CurrentTemperature;
                            objCommandDetail.Flag = 7;
                            //objCommandDetail.TotalStart = items.TotalStart;
                            //objCommandDetail.TotalEnd = items.TotalEnd;
                            //objCommandDetail.Vehicle = items.Vehicle;
                            //objCommandDetail.MixingRatio = items.MixingRatio;
                            //objCommandDetail.GasDensity = items.GasDensity;
                            //objCommandDetail.AlcoholicDensity = items.AlcoholicDensity;
                            //objCommandDetail.V_Actual_15 = items.V_Actual_15;
                            //objCommandDetail.TotalStart_15 = items.TotalStart_15;
                            //objCommandDetail.TotalEnd_15 = items.TotalEnd_15;
                            //objCommandDetail.V_Actual_Base = items.V_Actual_Base;
                            //objCommandDetail.V_Actual_E = items.V_Actual_E;
                            //objCommandDetail.V_Actual_Base_15 = items.V_Actual_Base_15;
                            //objCommandDetail.V_Actual_E_15 = items.V_Actual_E_15;
                            //objCommandDetail.TotalStart_Base = items.TotalEnd_Base;
                            //objCommandDetail.TotalStart_E = items.TotalEnd_E;
                            //objCommandDetail.TotalStart_Base_15 = items.TotalStart_Base_15;
                            //objCommandDetail.TotalStart_E_15 = items.TotalStart_E_15;
                            //objCommandDetail.TotalEnd_Base = items.TotalEnd_Base;
                            //objCommandDetail.TotalEnd_E = items.TotalEnd_E;
                            //objCommandDetail.TotalEnd_Base_15 = items.TotalEnd_Base;
                            //objCommandDetail.TotalEnd_E_15 = items.TotalEnd_E_15;
                            //objCommandDetail.AvgDensity = items.AvgDensity;
                            //objCommandDetail.CTL_Base = items.CTL_Base;
                            //objCommandDetail.CTL_E = items.CTL_E;
                            //objCommandDetail.InsertDate = items.InsertDate;
                            //objCommandDetail.InsertUser = items.InsertUser;
                            //objCommandDetail.UpdateDate = items.UpdateDate;
                            //objCommandDetail.UpdateUser = items.UpdateUser;
                            //objCommandDetail.VersionNo = items.VersionNo;
                            //objCommandDetail.DeleteFlg = items.DeleteFlg;
                            //objCommandDetail.ActualRatio = items.ActualRatio;

                            var rs = commandDetailService.UpdateCommandDetail(objCommandDetail);
                            if (rs)
                            {
                                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_SEAL_SUCCESS;
                            }
                            else
                            {
                                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_SEAL_FAILED;
                            }
                        }
                    }
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_SEAL_FAILED;
                }
            }
            return RedirectToAction("RegisterSeal");
        }

        public ActionResult Edit(int id)
        {
            sealModel.Seal = sealService.FindSealById(id);
            return View(sealModel);
        }

        [HttpPost]
        public ActionResult Edit(SealModel sealModel)
        {
            var seal = sealService.FindSealById(sealModel.Seal.ID);
            if (null != seal)
            {
                seal.UpdateUser = HttpContext.User.Identity.Name;

                var rs = sealService.UpdateSeal(seal);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }

                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                }
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View(sealModel);
        }

        [HttpPost]
        public ActionResult Create(MSeal seal)
        {
            seal.InsertUser = HttpContext.User.Identity.Name;
            seal.UpdateUser = HttpContext.User.Identity.Name;

            var rs = sealService.CreateSeal(seal);

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var rs = sealService.DeleteSeal(id);
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
    }
}