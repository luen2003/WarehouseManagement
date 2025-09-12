using PetroBM.Web.Models;
using PetroBM.Services.Services;
using PetroBM.Common.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetroBM.Web.Attribute;

namespace PetroBM.Web.Controllers
{
    public class ConfigurationController : BaseController
    {
        private readonly IConfigurationService configurationService;
        private readonly IUserService userService;
        private readonly IAlarmService alarmService;
        private ConfigurationModel configurationModel;
        private BaseService baseService;

        public ConfigurationController(IConfigurationService configurationService, IAlarmService alarmService,
        ConfigurationModel configurationModel, IUserService userService, BaseService baseService)
        {
            this.configurationService = configurationService;
            this.alarmService = alarmService;
            this.configurationModel = configurationModel;
            this.userService = userService;
            this.baseService = baseService;

        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult SoundAlarm()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONWARMINGEVENT);
            if (checkPermission)
            {
                configurationModel.AlarmType = alarmService.GetAllAlarmType().ToList();

                return View(configurationModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult SaveSoundAlarm(ConfigurationModel configurationModel)
        {
            var rs = false;
            TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

            try
            {
                var i = 0;

                foreach (var item in configurationModel.AlarmType)
                {
                    var alarmType = alarmService.GetAlarmTypeById(item.TypeId);
                    var file = configurationModel.SoundAlarmFile[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath(Constants.SOUND_ALARM_PATH), fileName);
                        file.SaveAs(path);

                        alarmType.Sound = Path.Combine(Constants.SOUND_ALARM_PATH, fileName);
                        rs = alarmService.UpdateSoundAlarm(alarmType, HttpContext.User.Identity.Name);

                        if (!rs)
                        {
                            break;
                        }
                    }

                    i++;
                }
            }
            catch { }

            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }

            return RedirectToAction("SoundAlarm");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult SelectFields()
        {
            var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedFields;
            if (!string.IsNullOrEmpty(selectedFields))
            {
                configurationModel.SelectedFieldsConfig = selectedFields.Split(',').ToList();
            }
            else
            {
                configurationModel.SelectedFieldsConfig = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_FIELD).Value.Split(',').ToList();
            }
            return View(configurationModel);
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult SelectedConfigArmFields()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONCONFIGARMDISPLAY);
            if (checkPermission)
            {
                var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedConfigArmFields;
                if (!string.IsNullOrEmpty(selectedFields))
                {
                    configurationModel.SelectedConfigArmFieldsConfig = selectedFields.Split(',').ToList();
                }
                else
                {
                    configurationModel.SelectedConfigArmFieldsConfig = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_FIELD).Value.Split(',').ToList();
                }
                return View(configurationModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }




        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult SelectFields(ConfigurationModel configModel)
        {
            //var rs = configurationService.SaveConfiguration(HttpContext.User.Identity.Name, Constants.CONFIG_SELECTED_FIELD, configModel.SelectedFieldsConfig.FirstOrDefault(), Constants.EVENT_CONFIG_SELECTED_FIELD);
            //For single User
            var rs = userService.SaveSelectedField(HttpContext.User.Identity.Name, configModel.SelectedFieldsConfig.FirstOrDefault());
            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }

            return RedirectToAction("SelectFields");
        }


        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult SelectedConfigArmFields(ConfigurationModel configModel)
        {
            var rs = userService.SelectedConfigArmFields(HttpContext.User.Identity.Name, configModel.SelectedConfigArmFieldsConfig.FirstOrDefault());
            if (rs)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }

            return RedirectToAction("SelectedConfigArmFields");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult ExportMode()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONEXPORTMODE);
            if (checkPermission)
            {
                configurationModel.ExportMode = configurationService.GetConfiguration(Constants.CONFIG_EXPORT_MODE);

                return View(configurationModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult SaveExportMode(ConfigurationModel configurationModel)
        {
            var exportMode = configurationModel.ExportMode.Value;
            var rs = configurationService.SaveConfiguration(HttpContext.User.Identity.Name, Constants.CONFIG_EXPORT_MODE,
                exportMode, Constants.EVENT_CONFIG_EXPORT_MODE);

            if (rs == true)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }

            return RedirectToAction("ExportMode");
        }

        public ActionResult LogoImage()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGCOMPANY);
            if (checkPermission)
            {
                configurationModel.LogoConfig = configurationService.GetConfiguration(Constants.CONFIG_LOGO);
                return Content("<img src = '" + configurationModel.LogoConfig.Value + "' />");
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult CompanyName()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGCOMPANY);
            if (checkPermission)
            {
                configurationModel.CompanyConfig = configurationService.GetConfiguration(Constants.CONFIG_COMP_NAME);
                var compName = configurationModel.CompanyConfig.Value;
                return Content("<span class='text-uppercase'>" + compName + "</span>");
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult HomeImage()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGCOMPANY);
            if (checkPermission)
            {
                configurationModel.HomeImageConfig = configurationService.GetConfiguration(Constants.CONFIG_HOME_IMAGE);
                return View(configurationModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult SaveHomeImage(ConfigurationModel configurationModel)
        {
            try
            {
                var file = configurationModel.HomeImageFile;
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath(Constants.IMG_PATH), fileName);
                    file.SaveAs(path);

                    var rs = configurationService.SaveConfiguration(HttpContext.User.Identity.Name, Constants.CONFIG_HOME_IMAGE
                        , Path.Combine(Constants.IMG_PATH, fileName), Constants.EVENT_CONFIG_HOME_IMAGE);
                    if (rs)
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    }
                }
            }
            catch { }

            return RedirectToAction("HomeImage");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Logo()
        {
            configurationModel.LogoConfig = configurationService.GetConfiguration(Constants.CONFIG_LOGO);
            return View(configurationModel);
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult SaveLogo(ConfigurationModel configurationModel)
        {
            try
            {
                var file = configurationModel.LogoFile;
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath(Constants.IMG_PATH), fileName);
                    file.SaveAs(path);

                    var rs = configurationService.SaveConfiguration(HttpContext.User.Identity.Name, Constants.CONFIG_LOGO
                        , Path.Combine(Constants.IMG_PATH, fileName), Constants.EVENT_CONFIG_LOGO);
                    if (rs)
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    }
                }
            }
            catch { }

            return RedirectToAction("Logo");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult CompanyAndUnit()
        {
            configurationModel.CompanyConfig = configurationService.GetConfiguration(Constants.CONFIG_COMP_NAME);
            configurationModel.UnitConfig = configurationService.GetConfiguration(Constants.CONFIG_UNIT);
            return View(configurationModel);
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult EditCompanyAndUnit(ConfigurationModel configurationModel)
        {
            var compName = configurationModel.CompanyConfig.Value;
            var unitName = configurationModel.UnitConfig.Value;
            var rs = configurationService.SaveCompNameAndUnit(HttpContext.User.Identity.Name, Constants.CONFIG_COMP_NAME
                , compName, Constants.CONFIG_UNIT, unitName, Constants.EVENT_CONFIG_COMP_NAME_LOGO);

            if (rs == true)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }

            return RedirectToAction("CompanyAndUnit");
        }

    }
}