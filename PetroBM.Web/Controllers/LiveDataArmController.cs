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
using System.Data;


namespace PetroBM.Web.Controllers
{

    public class LiveDataArmController : BaseController
    {
        private readonly ILiveDataArmService livedataarmService;
        private readonly IConfigArmService configArmService;
        public LiveDataArmModel livedataarmModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly IConfigArmGrpConfigArmService configarmgrpconfigarmService;
        private readonly IConfigArmGrpService ConfigArmGrpService;
        private readonly BaseService baseService;
        private readonly ICommandDetailService commandDetailService;
        private readonly ICommandService commandService;
        private readonly ICustomerService customerService;

        public LiveDataArmController(ILiveDataArmService livedataarmService, LiveDataArmModel livedataarmModel, IConfigurationService configurationService,
            IUserService userService, IConfigArmService ConfigArmService, IConfigArmGrpService ConfigArmGrpService,
            IConfigArmGrpConfigArmService configarmgrpconfigarmService, BaseService baseService, ICommandDetailService commandDetailService, 
            ICommandService commandService, ICustomerService customerService)
        {
            this.livedataarmService = livedataarmService;
            this.livedataarmModel = livedataarmModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.configArmService = ConfigArmService;
            this.configarmgrpconfigarmService = configarmgrpconfigarmService;
            this.ConfigArmGrpService = ConfigArmGrpService;
            this.baseService = baseService;
            this.commandDetailService = commandDetailService;
            this.commandService = commandService;
            this.customerService = customerService;
        }

        public ActionResult Index()
        {
            livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo();
            return View(livedataarmModel);
        }

        //[HttpGet]
        //public ActionResult IndexSearch()
        //{
        //    livedataarmModel.ListMConfigArm = ConfigArmService.GetAllConfigArm().ToList();
        //    livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo();
        //    return View(livedataarmModel);
        //}

        //[HttpPost]
        public ActionResult IndexSearch(LiveDataArmModel livedataarmModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CONFIGARM);
            if (checkPermission)
            {
                var wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());

                var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedConfigArmFields;
                if (!string.IsNullOrEmpty(selectedFields))
                {
                    livedataarmModel.ListSelectedField = selectedFields.Split(',').ToList();
                }
                else
                {
                    livedataarmModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_LIVEDATAARM_FIELD).Value.Split(',').ToList();
                }

                if (livedataarmModel.ArmNo == null)
                {
                    livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == wareHouse).ToList();

                }
                else
                {
                    livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.ArmNo == (byte)livedataarmModel.ArmNo && x.WareHouseCode == wareHouse).ToList();

                }

                livedataarmModel.ListMConfigArm = configArmService.GetAllConfigArmOrderByName().Where(x => x.WareHouseCode == wareHouse).ToList();

                livedataarmModel.ListCommandDetail = commandDetailService.GetAllCommandDetail().ToList();
                livedataarmModel.ListCommand = commandService.GetAllCommand().ToList();
                livedataarmModel.ListCustomer = customerService.GetAllCustomer().ToList();

                return View(livedataarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CONFIGARM);
            if (checkPermission)
            {
                livedataarmModel.LiveDataArm = livedataarmService.FindLiveDataArmById(id);
                return View(livedataarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(LiveDataArmModel livedataarmModel)
        {
            // var livedataarm = livedataarmService.FindLiveDataArmById(livedataarmModel.LiveDataArm.ID);
            //var livedataarm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.ArmNo == livedataarmModel.LiveDataArm.ArmNo && x.TimeLog == livedataarmModel.LiveDataArm.TimeLog && x.WareHouseCode == livedataarmModel.LiveDataArm.WareHouseCode).FirstOrDefault();
            //if (null != livedataarm)
            //{
            //    livedataarm.UpdateUser = HttpContext.User.Identity.Name;

            //    var rs = livedataarmService.UpdateLiveDataArm(livedataarm);

            //    if (rs)
            //    {
            //        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            //    }

            //    else
            //    {
            //        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            //    }
            //}
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View(livedataarmModel);
        }

        [HttpPost]
        public ActionResult Create(MLiveDataArm livedataarm)
        {
            //livedataarm.InsertUser = HttpContext.User.Identity.Name;
            //livedataarm.UpdateUser = HttpContext.User.Identity.Name;

            //var rs = livedataarmService.CreateLiveDataArm(livedataarm);

            //if (rs)
            //{
            //    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            //}
            //else
            //{
            //    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            //}

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var rs = livedataarmService.DeleteLiveDataArm(id);
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

        public ActionResult Table(int configarmGroupId, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CONFIGARMGROUP);
            if (checkPermission)
            {
                byte warehouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                livedataarmModel.ConfigArmGrpId = configarmGroupId;
                Session[Constants.Session_ConfigArmGroup] = configarmGroupId;
                var configarm = ConfigArmGrpService.FindConfigArmGrpById(configarmGroupId);
                if (configarm == null)
                {
                    Session[Constants.Session_ConfigArmGroupName] = "";
                }
                else
                {
                    Session[Constants.Session_ConfigArmGroupName] = configarm.ConfigArmName;
                }

                var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedConfigArmFields;
                if (!string.IsNullOrEmpty(selectedFields))
                {
                    livedataarmModel.ListSelectedField = selectedFields.Split(',').ToList();
                }
                else
                {
                    livedataarmModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_LIVEDATAARM_FIELD).Value.Split(',').ToList();
                }

                //Lay cac hong thuoc nhom
                List<int> lstArmNo = configarmgrpconfigarmService.GetArmNo_By_ConfigArmGroupId(configarmGroupId);
                livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo));

                //   livedataarmModel.ListMConfigArm = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo)).ToList();
                return View(livedataarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
        [HttpPost]
        public ActionResult Table(LiveDataArmModel livedataarmModel)
        {
            byte warehouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            //Lay cac hong thuoc nhom
            List<int> lstArmNo = configarmgrpconfigarmService.GetArmNo_By_ConfigArmGroupId((int)livedataarmModel.ConfigArmGrpId);
            if (livedataarmModel.ArmNo == null)
                livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo));
            else
                livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo) && x.ArmNo == livedataarmModel.ArmNo);
            livedataarmModel.ListMConfigArm = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == warehouse).ToList();
            return View(livedataarmModel);
        }

        public ActionResult Graphical(int configarmGroupId, int? armNo, int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CONFIGARMGROUP);
            if (checkPermission)
            {
                byte warehouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
                livedataarmModel.ConfigArmGrpId = configarmGroupId;
                livedataarmModel.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());

                var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedConfigArmFields;
                if (!string.IsNullOrEmpty(selectedFields))
                {
                    livedataarmModel.ListSelectedField = selectedFields.Split(',').ToList();
                }
                else
                {
                    livedataarmModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_LIVEDATAARM_FIELD).Value.Split(',').ToList();
                }

                //Lay cac hong thuoc nhom
                List<int> lstArmNo = configarmgrpconfigarmService.GetArmNo_By_ConfigArmGroupId(configarmGroupId);
                if (armNo == null)
                    livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo));
                else
                    livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo) && x.ArmNo == armNo);
                livedataarmModel.ListMConfigArm = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo)).ToList();
                return View(livedataarmModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Graphical(LiveDataArmModel livedataarmModel)
        {
            byte warehouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            //Lay cac hong thuoc nhom
            List<int> lstArmNo = configarmgrpconfigarmService.GetArmNo_By_ConfigArmGroupId((int)livedataarmModel.ConfigArmGrpId);
            if (livedataarmModel.ArmNo == null)
                livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo));
            else
                livedataarmModel.ListLiveDataArm = livedataarmService.GetAllLiveDataArmOrderByArmNo().Where(x => x.WareHouseCode == warehouse && lstArmNo.Contains(x.ArmNo) && x.ArmNo == livedataarmModel.ArmNo);
            livedataarmModel.ListMConfigArm = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == warehouse).ToList();
            return View(livedataarmModel);
        }

    }
}