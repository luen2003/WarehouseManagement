using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetroBM.Services.Services;
using PetroBM.Web.Models;
using PetroBM.Domain.Entities;
using PetroBM.Web.Attribute;
using PetroBM.Common.Util;

namespace PetroBM.Web.Controllers
{
    [HasPermission(Constants.PERMISSION_MANAGEMENT)]
    public class UserGroupManagementController : BaseController
    {
        private readonly IUserGroupService userGrpService;
        private readonly IWareHouseService wareHouseService;
        public UserGroupModel userGrpModel;
        private readonly BaseService baseService;

        public UserGroupManagementController(IUserGroupService userGrpService, IWareHouseService wareHouseService, UserGroupModel userGrpModel, BaseService baseService)
        {
            this.userGrpService = userGrpService;
            this.wareHouseService = wareHouseService;
            this.userGrpModel = userGrpModel;
            this.baseService = baseService;
        }

        // GET: UserGroupManagement
        public ActionResult Index()
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_MANAGEMENT);
            if (checkPermission)
            {
                userGrpModel.UserGrpList = userGrpService.GetAllUserGrp();

                return View(userGrpModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }

        }

        public ActionResult New()
        {
            userGrpModel.UserGrpPermissionList = userGrpService.InitUserGrpPermissionList();
            userGrpModel.WareHouseList = wareHouseService.GetAllWareHouse().ToList();
            return View(userGrpModel);
        }

        [HttpPost]
        public ActionResult DoCreate(UserGroupModel userGrpModel)
        {
            string userName = HttpContext.User.Identity.Name;
            userGrpModel.UserGrp.InsertUser = userName;
            userGrpModel.UserGrp.UpdateUser = userName;
            var rs = userGrpService.CreateUserGrp(userGrpModel.UserGrp, userGrpModel.UserGrpPermissionList);

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

        public ActionResult Edit(int id)
        {
            userGrpService.UpdateUserGrpPermission(id);
            userGrpModel.UserGrp = userGrpService.GetUserGrp(id);
            userGrpModel.UserGrpPermissionList = userGrpModel.UserGrp.MUserGrpPermissions.OrderBy(p => p.PermissionCode).ToList();
            userGrpModel.WareHouseList = wareHouseService.GetAllWareHouse().ToList();
            return View(userGrpModel);
        }

        [HttpPost]
        public ActionResult DoEdit(UserGroupModel userGrpModel)
        {
            var userGrp = userGrpService.GetUserGrp(userGrpModel.UserGrp.GrpId);

            if (userGrp != null)
            {
                userGrp.GrpName = userGrpModel.UserGrp.GrpName;
                userGrp.Description = userGrpModel.UserGrp.Description;
                userGrp.UpdateUser = HttpContext.User.Identity.Name;
                userGrp.WareHouseCode = userGrpModel.UserGrp.WareHouseCode;
                var rs = userGrpService.UpdateUserGrp(userGrp, userGrpModel.UserGrpPermissionList);

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

        public ActionResult DoDeleteUserGrp(int id)
        {
            var userGrp = userGrpService.GetUserGrp(id);

            if (userGrp != null)
            {
                userGrp.UpdateUser = HttpContext.User.Identity.Name;
                var rs = userGrpService.DeleteUserGrp(userGrp);

                if (rs)
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_SUCCESS;
                }
                else
                {
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_DELETE_FAILED;
                }
            }

            return RedirectToAction("Index");
        }
    }
}