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

    public class CustomerController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(CustomerController));

        private readonly ICustomerService customerService;
        private readonly ICustomerGroupService customerGroupService;
        public CustomerModel customerModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public CustomerController(ICustomerService customerService, CustomerModel customerModel, IConfigurationService configurationService, IUserService userService, ICustomerGroupService customerGroupService, BaseService baseService)
        {
            this.customerService = customerService;
            this.customerModel = customerModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.baseService = baseService;
            this.customerGroupService = customerGroupService; 
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
                    if (customerModel.CustomerCode == null && customerModel.TaxCode == null)
                    {
                        customerModel.ListCustomer = customerService.GetAllCustomer().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        if (customerModel.CustomerCode != null)
                        {
                            if (customerModel.TaxCode != null)
                                customerModel.ListCustomer = customerService.GetAllCustomer().Where(x => x.CustomerCode.Contains(customerModel.CustomerCode) && x.TaxCode.Contains(customerModel.TaxCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                            else
                                customerModel.ListCustomer = customerService.GetAllCustomer().Where(x => x.CustomerCode.Contains(customerModel.CustomerCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        }
                        else
                            customerModel.ListCustomer = customerService.GetAllCustomer().Where(x => x.TaxCode.Contains(customerModel.TaxCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
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

        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTCUSTOMER);
            if (checkPermission)
            {
                customerModel.Customer = customerService.FindCustomerById(id);
                customerModel.ListCustomerGroup = customerGroupService.GetAllCustomerGroup();
                return View(customerModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(CustomerModel customerModel)
        {
            try
            {
                log.Info("Customer Edit");
                var customer = customerService.FindCustomerById(customerModel.Customer.ID);
                if (null != customer)
                {
                    customer.UpdateUser = HttpContext.User.Identity.Name;
                    customer.CustomerAddress = customerModel.Customer.CustomerAddress;
                    customer.CustomerCode = customerModel.Customer.CustomerCode;
                    customer.Deputy = customerModel.Customer.Deputy;
                    customer.CustomerName = customerModel.Customer.CustomerName;
                    customer.Position = customerModel.Customer.Position;
                    customer.PhotoName = customerModel.Customer.PhotoName;
                    customer.TaxCode = customerModel.Customer.TaxCode;
                    customer.PhoneNumber = customerModel.Customer.PhoneNumber;
                    customer.AccountNo = customerModel.Customer.AccountNo;
                    customer.Unit = customerModel.Customer.Unit;
                    customer.Price = customerModel.Customer.Price;
                    customer.UnitName = customerModel.Customer.UnitName;
                    customer.Payments = customerModel.Customer.Payments;
                    customer.Postition = customerModel.Customer.Postition;
                    customer.CustomerGroup = customerModel.Customer.CustomerGroup;

                    var rs = customerService.UpdateCustomer(customer);

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

        [HttpPost]
        public ActionResult Detail(string identificationNumber)
        {
            try
            {
                log.Info("Detail"); 
                var obj = customerService.GetAllCustomer().Where(x => x.CustomerName == identificationNumber && x.DeleteFlg == false).FirstOrDefault();
                
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTCUSTOMER);
            if (checkPermission)
            {
                customerModel.ListCustomerGroup = customerGroupService.GetAllCustomerGroup();
                return View(customerModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(MCustomer customer)
        {
            try
            {
                log.Info("Customer Create");
                customer.InsertUser = HttpContext.User.Identity.Name;
                customer.UpdateUser = HttpContext.User.Identity.Name;
                
                var rs = customerService.CreateCustomer(customer);

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
                var rs = customerService.DeleteCustomer(id);
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
        public ActionResult Import(CustomerModel customer)
        {
            try
            {
                log.Info("Customer Import");
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                var rs = customerService.Import(customer.ImportFile, HttpContext.User.Identity.Name);

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