using PetroBM.Web.Models;
using PetroBM.Services.Services;
using PetroBM.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetroBM.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IConfigurationService configurationService;
        public ConfigurationModel configurationModel;

        public HomeController(IConfigurationService configurationService, ConfigurationModel configurationModel)
        {
            this.configurationService = configurationService;
            this.configurationModel = configurationModel;
        }

        public ActionResult Index()
        {
            configurationModel.SystemConfig = configurationService.GetConfiguration(Constants.CONFIG_HOME_IMAGE);
            return View(configurationModel);
        }

        public ActionResult AuthorizeError()
        {
            return View(configurationModel);
        }

        public ActionResult Countdown()
        {
            return View();
        }
    }
}