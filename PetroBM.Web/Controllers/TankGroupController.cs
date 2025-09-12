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

namespace PetroBM.Web.Controllers
{
    public class TankGroupController : BaseController
    {
        private readonly ITankGroupService tankGroupService;
        private readonly ITankService tankService;
        private readonly IWareHouseService wareHouseService;
        private readonly IProductService productService;
        private readonly TankGroupModel tankGroupModel;
        private readonly ITankGrpTankService TankGrpTankService;
        private readonly IConfigurationService configurationService;
        private readonly IUserService userService;
        private readonly ITankLiveService tankLiveService;
        private readonly BaseService baseService;
        public TankGroupController(ITankGroupService tankGroupService, IProductService productService,
            IWareHouseService wareHouseService,
            ITankService tankService, ITankGrpTankService TankGrpTankService, ITankLiveService tankLiveService,
            TankGroupModel tankGroupModel, IConfigurationService configurationService, IUserService userService, BaseService baseService)
        {
            this.tankGroupService = tankGroupService;
            this.tankService = tankService;
            this.tankGroupModel = tankGroupModel;
            this.configurationService = configurationService;
            this.userService = userService;
            this.TankGrpTankService = TankGrpTankService;
            this.productService = productService;
            this.tankLiveService = tankLiveService;
            this.wareHouseService = wareHouseService;
            this.baseService = baseService;
        }
        // GET: TankGroup
        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Index(int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANKGROUP);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                tankGroupModel.WareHouses = wareHouseService.GetAllWareHouse();
                tankGroupModel.ListTankGrp = tankGroupService.GetAllTankGrp().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                return View(tankGroupModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");

            }
        }

        [ChildActionOnly]
        public ActionResult GetTankGrpNumber()
        {
            var count = tankGroupService.GetAllTankGrp().Count();

            return Content(count.ToString());
        }

        [ChildActionOnly]
        public ActionResult GetTankGrpCount(byte wareHouseCode)
        {
            var count = tankGroupService.GetAllTankGrp().Where(x => x.WareHouseCode == wareHouseCode).Count();

            return Content(count.ToString());
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Create()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANKGROUP);
            if (checkPermission)
            {
                //tankGroupModel.ListTank = tankService.GetAllTankOrderByName().AsEnumerable().Select(c => new SelectListItem
                //{
                //    Value = c.TankId.ToString(),
                //    Text = c.TankName
                //}).ToList();

                tankGroupModel.WareHouses = wareHouseService.GetAllWareHouse();
                tankGroupModel.TankTemps = tankService.GetAllTank().AsEnumerable().Select(c => new TankTempModel
                {
                    TankId = c.TankId,
                    ProductId = (int)c.ProductId,
                    TankName = c.TankName,
                    WareHouseCode = c.WareHouseCode
                });

                return View(tankGroupModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");

            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Create(TankGroupModel tankGroupModel)
        {
            //if (null != tankGroupModel.ListTankId)
            //{
            //    //foreach (var item in tankGroupModel.ListTankId)
            //    //{
            //    //    var tank = tankService.FindTankById(item);
            //    //    tankGroupModel.TankGrp.MTanks.Add(tank);
            //    //}
            //}

            tankGroupModel.TankGrp.InsertUser = HttpContext.User.Identity.Name;
            tankGroupModel.TankGrp.UpdateUser = HttpContext.User.Identity.Name;
            var rs = tankGroupService.CreateTankGrp(tankGroupModel.TankGrp);
            if (rs)
            {
                if (null != tankGroupModel.ListTankId)
                {
                    rs = tankGroupService.CreateTankGrpTank(tankGroupModel.TankGrp.TankGrpId, tankGroupModel.TankGrp.WareHouseCode, tankGroupModel.ListTankId, tankGroupModel.TankGrp.InsertUser);
                }

            }

            if (rs == true)
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
            }
            return RedirectToAction("Index");
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Edit(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_CONFIGPOSITIONTANKGROUP);
            if (checkPermission)
            {
                tankGroupModel.TankGrp = tankGroupService.FindTankById(id);
                tankGroupModel.WareHouses = wareHouseService.GetAllWareHouse();
                tankGroupModel.TankTemps = tankService.GetAllTank().AsEnumerable().Select(c => new TankTempModel
                {
                    TankId = c.TankId,
                    ProductId = (int)c.ProductId,
                    TankName = c.TankName,
                    WareHouseCode = c.WareHouseCode
                });

                tankGroupModel.ListTankId = tankGroupService.Get_ListTankId_By_TankgroupTank_WareHouseCode(tankGroupModel.TankGrp.TankGrpId, tankGroupModel.TankGrp.WareHouseCode, tankGroupModel.TankGrp.InsertUser);

                var lst = new List<TankTempModel>();
                for (int i = 0; i < tankGroupModel.ListTankId.Count(); i++)
                {
                    lst.Add(new TankTempModel { ProductId = 0, TankId = tankGroupModel.ListTankId[i], TankName = "select", WareHouseCode = tankGroupModel.TankGrp.WareHouseCode });
                }

                tankGroupModel.SelectedTankTemps = lst;

                return View(tankGroupModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");

            }
        }

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        [HttpPost]
        public ActionResult Edit(TankGroupModel tankGroupModel)
        {
            var tankGrp = tankGroupService.FindTankById(tankGroupModel.TankGrp.TankGrpId);

            if (null != tankGrp)
            {

                //tankGrp.MTanks.Clear();
                //if (null != tankGroupModel.ListTankId)
                //{
                //    foreach (var item in tankGroupModel.ListTankId)
                //    {
                //        var tank = tankService.FindTankById(item);

                //        tankGrp.MTanks.Add(tank);
                //    }
                //}
                tankGrp.UpdateUser = HttpContext.User.Identity.Name;
                tankGrp.TanktGrpName = tankGroupModel.TankGrp.TanktGrpName;
                var rs = tankGroupService.UpdateTankGrp(tankGrp);
                if (rs)
                {
                    rs = tankGroupService.Update_TankGrpTank_By_ListTankId_WareHouseCode(tankGroupModel.TankGrp.TankGrpId, tankGroupModel.TankGrp.WareHouseCode, tankGroupModel.ListTankId, tankGrp.UpdateUser);
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

        [HasPermission(Constants.PERMISSION_CONFIGURATION)]
        public ActionResult Delete(int id)
        {
            var rs = tankGroupService.DeleteTankGrp(id);

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

        [HasPermission(Constants.PERMISSION_TANK_GRP)]
        public ActionResult Graphical(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_TANKGROUP);
            if (checkPermission)
            {

                tankGroupModel.TankGrp = tankGroupService.FindTankById(id);
                var wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());

                var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedFields;
                if (!string.IsNullOrEmpty(selectedFields))
                {
                    tankGroupModel.ListSelectedField = selectedFields.Split(',').ToList();
                }
                else
                {
                    tankGroupModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_FIELD).Value.Split(',').ToList();
                }

                #region Gộp nhóm bể
                //Lấy nhóm 
                var lst = TankGrpTankService.GetAllTankGrpTank().Where(x => x.WareHouseCode == wareHouse && x.TankGrpId == tankGroupModel.TankGrp.TankGrpId && x.DeleteFlg == false).ToList();
                List<int> lstInt = new List<int>();
                for (int i = 0; i < lst.Count(); i++)
                {
                    lstInt.Add(lst[i].TankId);
                }

                #endregion

                // Lấy các bể thuộc 1 nhóm      
                tankGroupModel.Tanks = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse && lstInt.Contains(x.TankId)).OrderBy(x => x.TankName);
                tankGroupModel.Products = productService.GetAllProduct();
                //*********Lấy các bản ghi gần nhất được thêm vào bảng TankLive tương ứng với các bể thuộc kho đang làm việc chưa làm được*************//

                tankGroupModel.TankLives = tankLiveService.GetTankLives_By_WareHouseCode(wareHouse);
                return View(tankGroupModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");

            }
        }

        [HasPermission(Constants.PERMISSION_TANK_GRP)]
        public ActionResult Table(int id)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_WAREHOUSE_TANKGROUP);
            if (checkPermission)
            {
                tankGroupModel.TankGrp = tankGroupService.FindTankById(id);
                var wareHouse = byte.Parse(Session[Constants.Session_WareHouse].ToString());

                var selectedFields = userService.GetUser(HttpContext.User.Identity.Name).SelectedFields;
                if (!string.IsNullOrEmpty(selectedFields))
                {
                    tankGroupModel.ListSelectedField = selectedFields.Split(',').ToList();
                }
                else
                {
                    tankGroupModel.ListSelectedField = configurationService.GetConfiguration(Constants.CONFIG_SELECTED_FIELD).Value.Split(',').ToList();
                }


                #region Gộp nhóm bể
                //Lấy nhóm 
                var lst = TankGrpTankService.GetAllTankGrpTank().Where(x => x.WareHouseCode == wareHouse && x.TankGrpId == tankGroupModel.TankGrp.TankGrpId && x.DeleteFlg == false).ToList();
                List<int> lstInt = new List<int>();
                for (int i = 0; i < lst.Count(); i++)
                {
                    lstInt.Add(lst[i].TankId);
                }

                #endregion

                // Lấy các bể thuộc 1 nhóm      
                tankGroupModel.Tanks = tankService.GetAllTank().Where(x => x.WareHouseCode == wareHouse && lstInt.Contains(x.TankId)).OrderBy(x => x.TankName);
                tankGroupModel.Products = productService.GetAllProduct();
                //Đang lôi ở Relasionship cần phải sửa bổ sung cho hợp lí
                return View(tankGroupModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");

            }
        }
    }
}