using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Common.Util;
using PagedList;
using System.Linq;
using PetroBM.Web.Attribute;
using log4net;
using System;
using System.Collections.Generic;

namespace PetroBM.Web.Controllers
{

    public class DepartmentController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CustomerController));

        private readonly IDepartmentService departmentService; 
        public DepartmentModel departmentModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public DepartmentController(IDepartmentService departmentService, DepartmentModel departmentModel, IConfigurationService configurationService, IUserService userService,  BaseService baseService)
        {
            this.departmentService = departmentService;
            this.departmentModel = departmentModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.baseService = baseService; 
        }

        public ActionResult Index(int? page, CustomerModel customerModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTCUSTOMER);
            if (checkPermission)
            {

                try
                {
                    log.Info("Customer Index");
                    int pageNumber = (page ?? 1);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return View(customerModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        } 



    }
}