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
    [HasPermission(Constants.PERMISSION_MANAGEMENT)]
    public class UserManagementController : BaseController
    {
        private readonly IUserService userService;
        private readonly IUserGroupService userGroupService;
        public UserModel userModel;
        public BaseService baseService;

        public UserManagementController(IUserService userService, UserModel userModel, IUserGroupService userGroupService, BaseService baseService)
        {
            this.userService = userService;
            this.userModel = userModel;
            this.userGroupService = userGroupService;
            this.baseService = baseService;
        }

        public ActionResult Index(int? page)
        {
            bool checkPermission = baseService.CheckPermission(HttpContext.User.Identity.Name, Constants.PERMISSION_MANAGEMENT);
            if (checkPermission)
            {
                int pageNumber = (page ?? 1);
                userModel.ListUser = userService.GetAllUser().ToPagedList(pageNumber, Constants.PAGE_SIZE);
                userModel.ListUserGrp = userGroupService.GetAllUserGrp();



                return View(userModel);
            }
            else
            {
                return RedirectToAction("AuthorizeError", "Home");
            }
        }

        public ActionResult Edit(string userName)
        {
            userModel.User = userService.GetUser(userName);
            userModel.ListUserGrp = userGroupService.GetAllUserGrp();
            // userModel.UserGrpId = userModel.User.GrpId;
            userModel.ListUserGrpId = userGroupService.Get_ListUseGroupId_By_UserName(userName).ToList();

            return View(userModel);
        }

        [HttpPost]
        public ActionResult Edit(UserModel userModel)
        {
            var user = userService.GetUser(userModel.User.UserName);
            user.FullName = userModel.User.FullName;
            user.Position = userModel.User.Position;
            // user.GrpId = userModel.UserGrpId;
            user.UpdateUser = HttpContext.User.Identity.Name;

            var rs = userService.UpdateUser(user);
            if (rs)
            {
                rs = userGroupService.UpdateUserGroupUser_By_ListUserGroupId(user, userModel.ListUserGrpId.ToArray());
                if (rs)
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
                else
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }

            return RedirectToAction("Index");
        }

        public ActionResult ChangePass(string userName)
        {
            userModel.User = userService.GetUser(userName);
            userModel.User.Passwd = String.Empty;
            return View(userModel);
        }

        [HttpPost]
        public ActionResult ChangePass(UserModel userModel)
        {
            var user = userService.GetUser(userModel.User.UserName);
            user.Passwd = userModel.User.Passwd;
            user.UpdateUser = HttpContext.User.Identity.Name;

            var rs = userService.ChangePass(user);
            if (rs)
            {

                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_FAILED;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            userModel.ListUserGrp = userGroupService.GetAllUserGrp();
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Create(UserModel userModel)
        {
            userModel.User.InsertUser = HttpContext.User.Identity.Name;
            userModel.User.UpdateUser = HttpContext.User.Identity.Name;
            //  userModel.User.GrpId = userModel.UserGrpId;
            var rs = userService.CreateUser(userModel.User);

            if (rs)
            {
                rs = userGroupService.UpdateUserGroupUser_By_ListUserGroupId(userModel.User, userModel.ListUserGrpId.ToArray());
                if (rs)
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_SUCCESS;
                else
                    TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            }
            else
            {
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_INSERT_FAILED;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string userName)
        {
            var rs = userService.DeleteUser(userName);

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
    }
}