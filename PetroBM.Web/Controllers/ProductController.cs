using PagedList;
using PetroBM.Web.Attribute;
using PetroBM.Common.Util;
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
    public class ProductController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ProductController));
        private readonly IProductService productService;
        private readonly ProductModel productModel;
        private readonly ITankService tankService;
        private readonly IConfigurationService configurationService;
        private readonly IUserService userService;
        private readonly BaseService baseService;
        public ProductController(IProductService productService, ProductModel productModel, ITankService tankService, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.productService = productService;
            this.productModel = productModel;
            this.tankService = tankService;
            this.configurationService = configurationService;
            this.userService = userService;
            this.baseService = baseService;
        }



        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        // GET: Product
        public ActionResult Index(int? page, ProductModel productModel)
        {
            log.Info("Start product index ");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTPRODUCT);
            if (checkPermission)
            {
                try
                {

                    int pageNumber = (page ?? 1);

                    productModel.ListProduct = productService.GetAllProduct().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    if (productModel.ProductName == null && productModel.ProductCode == null)
                    {
                        productModel.ListProduct = productService.GetAllProduct().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }
                    else
                    {
                        if (productModel.ProductName != null)
                            if (productModel.ProductCode != null)
                                productModel.ListProduct = productService.GetAllProduct().Where(x => x.ProductName.Contains(productModel.ProductName) && x.ProductCode.Contains(productModel.ProductCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                            else
                                productModel.ListProduct = productService.GetAllProduct().Where(x => x.ProductName.Contains(productModel.ProductName)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                        else
                            productModel.ListProduct = productService.GetAllProduct().Where(x => x.ProductCode.Contains(productModel.ProductCode)).ToPagedList(pageNumber, Constants.PAGE_SIZE);
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Info("Finish product index ");
                return View(productModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [ChildActionOnly]
        public ActionResult GetProductNumber()
        {
            var count = productService.GetAllProduct().Count();

            return Content(count.ToString());
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTPRODUCT);
            if (checkPermission)
            {
                return View(productModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Create(ProductModel productModel)
        {
            log.Info("Start product create ");
            try
            {
                productModel.Product.InsertUser = HttpContext.User.Identity.Name;
                productModel.Product.UpdateUser = HttpContext.User.Identity.Name;

                var rs = productService.CreateProduct(productModel.Product);

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
            log.Info("Finish product create ");
            return RedirectToAction("Index");
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_LISTPRODUCT);
            if (checkPermission)
            {
                productModel.Product = productService.FindProductById(id);
                return View(productModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Edit(ProductModel productModel)
        {
            log.Info("Start product edit ");
            try
            {
                var product = productService.FindProductById(productModel.Product.ProductId);
                if (null != product)
                {
                    product.UpdateUser = HttpContext.User.Identity.Name;
                    product.ProductName = productModel.Product.ProductName;
                    product.ProductCode = productModel.Product.ProductCode;
                    product.InputWastageRate = productModel.Product.InputWastageRate;
                    product.ExportWastageRate = productModel.Product.ExportWastageRate;
                    product.Color = productModel.Product.Color;
                    product.Abbreviations = productModel.Product.Abbreviations;
                    product.ProductType = productModel.Product.ProductType;
                    product.OriginalProductCode = productModel.Product.OriginalProductCode;
                    product.MixProductCode = productModel.Product.MixProductCode;
                    product.StoreWastageRate = productModel.Product.StoreWastageRate;

                    var rs = productService.UpdateProduct(product);

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
            log.Info("Finish product edit ");
            return RedirectToAction("Index");
        }

        //[HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Delete(int id)
        {
            log.Info("Start product delete " + id);
            try
            {

                var rs = productService.DeleteProduct(id);
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
            log.Info("Finish product delete " + id);
            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_PRODUCT)]
        public ActionResult Graphical(int id)
        {
            log.Info("Start product graphical ");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_PRODUCT);
            if (checkPermission)
            {
                try
                {
                    productModel.Product = productService.FindProductById(id);
                    productModel.ListTank = tankService.GetAllTankByProduct(id);

                    var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedFields;
                    if (!string.IsNullOrEmpty(selectedFields))
                    {
                        productModel.ListSelectedField = selectedFields.Split(',').ToList();
                    }
                    else
                    {
                        productModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_FIELD).Value.Split(',').ToList();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Info("Finish product graphical ");
                return View(productModel);
            }
            else
            {
                log.Info("Stop product graphical ");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        [HasPermission(Constants.PERMISSION_PRODUCT)]
        public ActionResult Table(int id)
        {
            log.Info("Start product table ");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_PRODUCT);
            if (checkPermission)
            {
                try
                {

                    productModel.Product = productService.FindProductById(id);
                    productModel.ListTank = tankService.GetAllTankByProduct(id);

                    var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedFields;
                    if (!string.IsNullOrEmpty(selectedFields))
                    {
                        productModel.ListSelectedField = selectedFields.Split(',').ToList();
                    }
                    else
                    {
                        productModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_FIELD).Value.Split(',').ToList();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Info("Finish product table ");
                return View(productModel);
            }
            else
            {
                log.Info("Stop product table ");
                return RedirectToAction("AuthorizeError", "Home");
            }
        }
    }
}