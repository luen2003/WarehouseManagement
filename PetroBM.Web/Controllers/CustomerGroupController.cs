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

namespace PetroBM.Web.Controllers
{

    public class CustomerGroupController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CustomerGroupController));

        private readonly ICustomerGroupService customerGroupService;
        public CustomerGroupModel customerGroupModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public CustomerGroupController(ICustomerGroupService customerGroupService, CustomerGroupModel customerGroupModel, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.customerGroupService = customerGroupService;
            this.customerGroupModel = customerGroupModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page, CustomerGroupModel customerGroupModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTCUSTOMER);
            if (checkPermission)
            {

                try
                {
                    log.Info("Customer Index");
                    int pageNumber = (page ?? 1);
                    if (customerGroupModel.CustomerGroupCode == null && customerGroupModel.TaxCode == null)
                    {
                        customerGroupModel.ListCustomerGroup = customerGroupService.GetAllCustomerGroup().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        if (customerGroupModel.CustomerGroupCode != null)
                        {
                            if (customerGroupModel.TaxCode != null)
                                customerGroupModel.ListCustomerGroup = customerGroupService.GetAllCustomerGroup().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                            else
                                customerGroupModel.ListCustomerGroup = customerGroupService.GetAllCustomerGroup().Where(x => x.CustomerGroupCode.Contains(customerGroupModel.CustomerGroupCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        }
                        else { }
                            customerGroupModel.ListCustomerGroup = customerGroupService.GetAllCustomerGroup().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return View(customerGroupModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTCUSTOMER);
            if (checkPermission)
            {
                customerGroupModel.CustomerGroup = customerGroupService.FindCustomerGroupById(id);
                return View(customerGroupModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(CustomerGroupModel customerGroupModel)
        {
            try
            {
                log.Info("Customer Edit");
                var customerGroup = customerGroupService.FindCustomerGroupById(customerGroupModel.CustomerGroup.ID);
                if (null != customerGroup)
                {
                    customerGroup.UpdateUser = HttpContext.User.Identity.Name;
                    customerGroup.CustomerGroupCode = customerGroupModel.CustomerGroup.CustomerGroupCode;
                    customerGroup.CustomerGroupName = customerGroupModel.CustomerGroup.CustomerGroupName;
                    customerGroup.Type = customerGroupModel.CustomerGroup.Type;
                    customerGroup.ShortName = customerGroupModel.CustomerGroup.ShortName;

                    var rs = customerGroupService.UpdateCustomerGroup(customerGroup);

                    if (rs)
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                    }

                    else
                    {
                        TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTCUSTOMER);
            if (checkPermission)
            {
                return View();
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(MCustomerGroup customerGroup)
        {
            try
            {
                log.Info("Customer Create");
                customerGroup.InsertUser = HttpContext.User.Identity.Name;
                customerGroup.UpdateUser = HttpContext.User.Identity.Name;

                var rs = customerGroupService.CreateCustomerGroup(customerGroup);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                log.Info("Customer Delete");
                var rs = customerGroupService.DeleteCustomerGroup(id);
                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Import(CustomerGroupModel customerGroup)
        {
            try
            {
                log.Info("Customer Import");
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                var rs = customerGroupService.Import(customerGroup.ImportFile, HttpContext.User.Identity.Name);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return RedirectToAction("Index");
        }




    }
}