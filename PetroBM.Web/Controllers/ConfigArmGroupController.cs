using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using System.Collections.Generic;
using PetroBM.Web.Attribute;
using System.Globalization;

namespace PetroBM.Web.Controllers
{
    public class ConfigArmGroupController : Controller
    {
        private readonly IConfigArmGrpService configarmgrpService;
        public ConfigArmGroupModel configarmgrpModel;
        private readonly IUserService userService;
        private readonly IWareHouseService WareHouseService;
        private readonly IConfigArmService configArmService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public ConfigArmGroupController(IConfigArmGrpService configarmgrpService,
            IConfigArmService configArmService, IWareHouseService WareHouseService, ConfigArmGroupModel configarmgrpModel, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.configarmgrpService = configarmgrpService;
            this.configarmgrpModel = configarmgrpModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.WareHouseService = WareHouseService;
            this.configArmService = configArmService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCONFIGARMGROUP);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                configarmgrpModel.ListConfigArmGrp = configarmgrpService.GetAllConfigArmGrp().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                configarmgrpModel.ListWareHouse = WareHouseService.GetAllWareHouse().ToList();
                return View(configarmgrpModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [ChildActionOnly]
        public ActionResult GetCongifArmGrpNumber()
        {
            var count = configarmgrpService.GetAllConfigArmGrp().Count();

            return Content(count.ToString());
        }

        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCONFIGARMGROUP);
            if (checkPermission)
            {
                configarmgrpModel.ConfigArmGrp = configarmgrpService.FindConfigArmGrpById(id);
                configarmgrpModel.ListWareHouse = WareHouseService.GetAllWareHouse().ToList();

                configarmgrpModel.ListConfigArmId = configarmgrpService.Get_ListConfigArmId_By_ConfigArmGroupConfigArm_WareHouseCode(configarmgrpModel.ConfigArmGrp.ConfigArmGrpId,
                    configarmgrpModel.ConfigArmGrp.WareHouseCode, configarmgrpModel.ConfigArmGrp.InsertUser).ToList();

                var lst = new List<ConfigArmTempModel>();
                for (int i = 0; i < configarmgrpModel.ListConfigArmId.Count(); i++)
                {
                    lst.Add(new ConfigArmTempModel { ConfigArmId = configarmgrpModel.ListConfigArmId[i], ArmName = "", ArmNo = (byte)0, WareHouseCode = configarmgrpModel.ConfigArmGrp.WareHouseCode });
                }
                configarmgrpModel.SelectConfigArmTemps = lst;

                configarmgrpModel.ListConfigArmTemps = configArmService.GetAllConfigArm().Select(x => new ConfigArmTempModel
                {
                    ArmName = x.ArmName,
                    ArmNo = x.ArmNo,
                    ConfigArmId = x.ConfigArmID,
                    WareHouseCode = x.WareHouseCode
                }).ToList();
                return View(configarmgrpModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(ConfigArmGroupModel configarmgrpModel)
        {
            var configarmgrp = configarmgrpService.FindConfigArmGrpById(configarmgrpModel.ConfigArmGrp.ConfigArmGrpId);
            configarmgrp.ConfigArmName = configarmgrpModel.ConfigArmGrp.ConfigArmName;
            if (null != configarmgrp)
            {
                configarmgrp.UpdateUser = HttpContext.User.Identity.Name;

                var rs = configarmgrpService.UpdateConfigArmGrp(configarmgrp);

                if (rs)
                {
                    rs = configarmgrpService.UpdateConfigArmGroup_By_ListConfigArm(configarmgrp, configarmgrpModel.ListConfigArmId.ToArray());
                }

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
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCONFIGARMGROUP);
            if (checkPermission)
            {
                configarmgrpModel.ListWareHouse = WareHouseService.GetAllWareHouse().ToList();
                configarmgrpModel.ListConfigArm = configArmService.GetAllConfigArm().ToList();
                return View(configarmgrpModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(ConfigArmGroupModel configarmgrpModel)
        {
            configarmgrpModel.ConfigArmGrp.InsertUser = HttpContext.User.Identity.Name;
            configarmgrpModel.ConfigArmGrp.UpdateUser = HttpContext.User.Identity.Name;
            var rs = configarmgrpService.CreateConfigArmGrp(configarmgrpModel.ConfigArmGrp);

            if (rs)
            {
                configarmgrpService.CreateConfigArmGroup_By_ListConfigArm(configarmgrpModel.ConfigArmGrp, configarmgrpModel.ListConfigArmId.ToArray());
            }

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
            var rs = configarmgrpService.DeleteConfigArmGrp(id);
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

        public ActionResult Table(int id)
        {
            configarmgrpModel.ConfigArmGrp = configarmgrpService.FindConfigArmGrpById(id);
            var wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());

            var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedFields;
            if (!string.IsNullOrEmpty(selectedFields))
            {
                configarmgrpModel.ListSelectedField = selectedFields.Split(',').ToList();
            }
            else
            {
                configarmgrpModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_FIELD).Value.Split(',').ToList();
            }

            return View(configarmgrpModel);
        }

        //[HasPermission(Constants.PERMISSION__GRP)]
        public ActionResult Graphical(int id)
        {
            var rs = configarmgrpService.DeleteConfigArmGrp(id);
            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
            }

            return View(configarmgrpModel);
        }
    }
}