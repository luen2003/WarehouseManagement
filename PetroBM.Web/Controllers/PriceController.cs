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
using log4net;
using System;

namespace PetroBM.Web.Controllers
{

    public class PriceController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(PriceController));
        private readonly IPriceService priceService;
        private readonly IProductService productService;
        public PriceModel priceModel;
        private readonly IUserService userService;
        private readonly IConfigurationService configurationService;
        private readonly BaseService baseService;

        public PriceController(IPriceService priceService, PriceModel priceModel, IProductService productService,
            IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.priceService = priceService;
            this.priceModel = priceModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.productService = productService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page, PriceModel priceModel)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTPRICE);
            if (checkPermission)
            {
                log.Info("Start price index ");
                try
                {
                    int pageNumber = (page ?? 1);
                    if (priceModel.ProductCode == null && priceModel.ProductName == null)
                    {
                        priceModel.ListPrice = priceService.GetAllPrice().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        if (priceModel.ProductCode != null)
                        {
                            if (priceModel.ProductName != null)
                                priceModel.ListPrice = priceService.GetAllPrice().Where(x => x.ProductName.Contains(priceModel.ProductName) && x.ProductCode.Contains(priceModel.ProductCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                            else
                                priceModel.ListPrice = priceService.GetAllPrice().Where(x => x.ProductCode.Contains(priceModel.ProductCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        }
                        else
                        {
                            priceModel.ListPrice = priceService.GetAllPrice().Where(x => x.ProductName.Contains(priceModel.ProductName)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Info("Finish price index ");
                return View(priceModel);
            }
            else
            {
                log.Info("Stop price index ");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTPRICE);
            if (checkPermission)
            {
                priceModel.Price = priceService.FindPriceById(id);
                return View(priceModel);
            }
            else
            {
                log.Info("Stop price index ");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Edit(PriceModel priceModel)
        {
            log.Info("Start price edit ");
            try
            {

                var price = priceService.FindPriceById(priceModel.Price.ProduceId);
                if (null != price)
                {
                    price.UpdateUser = HttpContext.User.Identity.Name;
                    price.AreaPrice1 = priceModel.Price.AreaPrice1;
                    price.AreaPrice2 = priceModel.Price.AreaPrice2;
                    price.EnvironmentTax = priceModel.Price.EnvironmentTax;
                    var rs = priceService.UpdatePrice(price);

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
            log.Info("Finish price edit ");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTPRICE);
            if (checkPermission)
            {
                priceModel.ProductTemps = productService.GetAllProduct().Select(c => new ProductTemp
                {
                    ProductCode = c.ProductCode,
                    ProductName = c.ProductName,
                    Abbreviations = c.Abbreviations
                });

                return View(priceModel);
            }
            else
            {
                log.Info("Stop price index ");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(MPrice price)
        {
            log.Info("Start price create ");
            try
            {

                price.InsertUser = HttpContext.User.Identity.Name;
                price.UpdateUser = HttpContext.User.Identity.Name;

                var rs = priceService.CreatePrice(price);

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
            log.Info("Finish price create ");
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {

            log.Info("Start price delete  " + id);
            try
            {

                var rs = priceService.DeletePrice(id);
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
            log.Info("Finish price delete  " + id);
            return RedirectToAction("Index");
        }
        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Import(PriceModel priceModel)
        {
            log.Info("Start price import ");
            try
            {

                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;

                var rs = priceService.Import(priceModel.ImportFile, HttpContext.User.Identity.Name);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish price import ");
            return RedirectToAction("Index");
        }
    }
}