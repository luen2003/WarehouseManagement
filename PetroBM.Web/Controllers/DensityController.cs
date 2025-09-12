

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
using log4net;
using System;
using System.Collections.Generic;


namespace PetroBM.Web.Controllers
{

    public class DensityController : BaseController
    {
        //log4net
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(DensityController));

        private readonly IDensityService densityService;
        public DensityModel densityModel;
        private readonly IUserService userService;
        private readonly IProductService productService;
        private readonly IConfigArmService configArmService;
        private readonly IConfigurationService configurationService;
        private readonly IWareHouseService warehouseService;
        private readonly BaseService baseService;

        public DensityController(IWareHouseService warehouseService, IDensityService densityService, DensityModel densityModel, IProductService ProductService, IConfigArmService ConfigArmService,
            IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.densityService = densityService;
            this.densityModel = densityModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.productService = ProductService;
            this.configArmService = ConfigArmService;
            this.warehouseService = warehouseService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page, DensityModel densityModel)
        {
            log.Info("Density Index");
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_CONFIGARM);
            if (checkPermission)
            {
                try
                {

                    int pageNumber = (page ?? 1);

                    densityModel.ListWareHouse = warehouseService.GetAllWareHouseOrderByName();

                    //Khởi tạo mã kho
                    if (densityModel.WareHouseCode == null)
                    {
                        foreach (var item in densityModel.ListWareHouse)
                        {
                            densityModel.WareHouseCode = item.WareHouseCode;
                            break;
                        }
                    }
                    densityModel.ListConfigArm = configArmService.GetAllConfigArmOrderByName().Where(x => x.WareHouseCode == densityModel.WareHouseCode).ToList();

                    densityModel.ArmNo = densityModel.ArmNo ?? null;
                    DateTime? startDate = null;
                    DateTime? endDate = null;


                    if (!string.IsNullOrEmpty(densityModel.StartDate))
                    {
                        startDate = DateTime.ParseExact(densityModel.StartDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    }
                    if (!string.IsNullOrEmpty(densityModel.EndDate))
                    {
                        endDate = DateTime.ParseExact(densityModel.EndDate, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                    }

                    densityModel.ListDensity = densityService.GetAllDensity().Where(x => x.WareHouseCode == (byte)densityModel.WareHouseCode
                      && (x.ArmNo == densityModel.ArmNo || densityModel.ArmNo == null)
                      && (x.InsertDate >= startDate)
                      && (x.InsertDate <= endDate)
                    ).ToPagedList(pageNumber, Constants.PAGE_SIZE);


                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return View(densityModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }




        public ActionResult Edit(int id)
        {
            densityModel.Density = densityService.FindDensityById(id);
            return View(densityModel);
        }

        [HttpPost]
        public ActionResult Edit(DensityModel densityModel)
        {
            try
            {
                var density = densityService.FindDensityById(densityModel.Density.ID);
                if (null != density)
                {
                    log.Info("Density Edit");
                    density.UpdateUser = HttpContext.User.Identity.Name;
                    density.WareHouseCode = densityModel.Density.WareHouseCode;
                    density.ArmNo = densityModel.Density.ArmNo;
                    density.ProductCode = densityModel.Density.ProductCode;

                    density.MixingRatio = densityModel.Density.MixingRatio;
                    density.GasDensity = densityModel.Density.GasDensity;
                    density.AlcoholicDensity = densityModel.Density.AlcoholicDensity;
                    density.RecipeProduct = densityModel.Density.RecipeProduct;

                    var rs = densityService.UpdateDensity(density);

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
            byte wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());
            densityModel.ListProduct = productService.GetAllProduct().ToList();
            densityModel.ListProductTemp = productService.GetAllProduct().Select(c => new ProductTemp
            {
                Abbreviations = c.Abbreviations,
                ProductCode = c.ProductCode,
                ProductName = c.ProductName
            }).ToList();

            densityModel.ListConfigArm = configArmService.GetAllConfigArmOrderByName().Where(x => x.WareHouseCode == wareHouse).ToList();
            var obj = densityService.GetAllDensity().Take(1).OrderByDescending(x => x.InsertDate).FirstOrDefault();
            if (obj != null)
            {
                densityModel.Density.GasDensity = obj.GasDensity;
                densityModel.Density.AlcoholicDensity = obj.AlcoholicDensity;
                densityModel.Density.MixingRatio = obj.MixingRatio;
            }

            return View(densityModel);
        }

        [HttpPost]
        public ActionResult Create(MDensity density)
        {
            try
            {
                log.Info("Density Create");
                density.InsertUser = HttpContext.User.Identity.Name;
                density.UpdateUser = HttpContext.User.Identity.Name;
                density.WareHouseCode = byte.Parse(Session[Constants.Session_WareHouse].ToString());

                var rs = densityService.CreateDensity(density);

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
                log.Info("Density Delete");
                var rs = densityService.DeleteDensity(id);
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

        public JsonResult GetProductCode(byte wareHouseCode, int armNo)
        {
            List<string> lst = new List<string>();
            var obj = configArmService.GetAllConfigArm().Where(x => x.WareHouseCode == wareHouseCode && x.ArmNo == armNo).FirstOrDefault();
            if (obj != null)
            {
                lst.Add(obj.ProductCode_1);
                if (obj.ProductCode_1 != obj.ProductCode_2 && obj.ProductCode_2 != null && obj.ProductCode_2 != "")
                {
                    lst.Add(obj.ProductCode_2);
                }
                if (obj.ProductCode_1 != obj.ProductCode_3 && obj.ProductCode_2 != obj.ProductCode_3 && obj.ProductCode_3 != null && obj.ProductCode_3 != "")
                {
                    lst.Add(obj.ProductCode_3);
                }
            }

            return Json(lst, JsonRequestBehavior.AllowGet);

        }


    }
}