
using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using PetroBM.Web.Attribute;
using System.Globalization;
using log4net;
using System;

namespace PetroBM.Web.Controllers
{
    public class LoadingArmController : BaseController
    {

        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(LoadingArmController));
        public LoadingArmModel loadingarmModel;
        private readonly ILiveDataArmService livedataarmService;
        private readonly ICommandDetailService commanddetailService;
        private readonly IConfigArmService configarmService;
        private readonly IVehicleService vehicleService;
        private readonly BaseService baseService;
        private readonly ICommandDetailService commandDetailService;
        private readonly ICommandService commandService;
        private readonly ICustomerService customerService;


        // GET: LoadingArm
        public ActionResult Index()
        {
            return View();
        }

        public LoadingArmController(IConfigArmService configarmService, ILiveDataArmService livedataarmService, ICommandDetailService commanddetailService, IVehicleService vehicleService, LoadingArmModel loadingarmModel, BaseService baseService, 
            ICommandDetailService commandDetailService, ICommandService commandService, ICustomerService customerService)
        {
            this.configarmService = configarmService;
            this.loadingarmModel = loadingarmModel;
            this.livedataarmService = livedataarmService;
            this.commanddetailService = commanddetailService;
            this.baseService = baseService;
            this.commandDetailService = commandDetailService;
            this.commandService = commandService;
            this.customerService = customerService;
        }

        public ActionResult IndexVisualWareHouse()
        {
            try
            {
                log.Info("IndexVisualWareHouse LoadingArm");
                loadingarmModel.ListConfigArm = configarmService.GetAllConfigArm();
                loadingarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArm();
                //loadingarmModel.ListLiveDataArmByWareHouseCode = livedataarmService.GetAllLiveDataArmByWareHouse(warehousecode);
                //                loadingarmModel.ListLiveDataArmByArmNo = livedataarmService.GetAllLiveDataArmByArmNo

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return View(loadingarmModel);
        }

        public ActionResult IndexVisual(LoadingArmModel LoadingArmModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CONFIGARM);
            if (checkPermission)
            {

                try
                {
                    log.Info("IndexVisual LoadingArm");
                    loadingarmModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                    loadingarmModel.ArmNo = LoadingArmModel.ArmNo;
                    loadingarmModel.ListConfigArm = configarmService.GetAllConfigArm();
                    if(loadingarmModel.ArmNo == 0)
                        loadingarmModel.ListLiveDataArmByArmNo = livedataarmService.GetAllLiveDataArm().ToList();
                    else
                        loadingarmModel.ListLiveDataArmByArmNo = livedataarmService.GetAllLiveDataArmByArmNo(1).ToList();
                    loadingarmModel.ListCommandDetailByArmNo = commanddetailService.GetAllCommandDetailByArmNo(1);
                    loadingarmModel.ListLiveDataArmConfigArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().ToList();
                    //loadingarmModel.ListLiveDataArmByWareHouseCode = livedataarmService.GetAllLiveDataArmByWareHouse(byte.Parse((LoadingArmModel.WareHouseCode).ToString()));
                    //loadingarmModel.ListVehicle = vehicleService.GetDriverNameByVehicleNumber(LoadingArmModel.VehicleNumber);
                    loadingarmModel.ListLiveDataArm = livedataarmService.GetLiveDataArmByArmNo(1, 1);
                    loadingarmModel.ListCommandDetail = commandDetailService.GetAllCommandDetail().ToList();
                    loadingarmModel.ListCommand = commandService.GetAllCommand().ToList();
                    loadingarmModel.ListCustomer = customerService.GetAllCustomer().ToList();

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return View(loadingarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public JsonResult GetSearchingData(byte warehousecode, int grouparm)
        {
            try
            {
                log.Info("Get SearchingData");
                var obj = livedataarmService.GetLiveDataArm_By_GroupArm(warehousecode, grouparm).ToList();
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public JsonResult GetDataIndex(byte warehousecode, byte armno)
        {
            try
            {
                log.Info("Get DataIndex");
                //var obj = livedataarmService.GetAllLiveDataArmByArmNo(armno).ToList();
                var obj = livedataarmService.GetLiveDataArmByArmNo(warehousecode, armno).ToList();
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

    }
}