using PetroBM.Common.Util;
using PetroBM.Web.Models;
using PetroBM.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using log4net;

namespace PetroBM.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IUserService userService;
        ILog log = log4net.LogManager.GetLogger(typeof(AccountController));

        //Contructor
        public AccountController(IUserService userService, UserModel userModel, IUserGroupService userGroupService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            log.Info("View Login");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel loginModel)
        {
            log.Info("post login");
            var rs = userService.Login(loginModel.UserName, loginModel.Password);

            if (rs)
            {
                FormsAuthentication.SetAuthCookie(loginModel.UserName, true);
                if (loginModel.RememberMe)
                {
                    //TODO cookie ne`
                }
                log.Info("login success");
                System.Web.HttpContext.Current.Session[Constants.Session_WareHouse] = 1;//Mặc định kho khi đăng nhập, cần chỉnh sửa lại phần này theo cấu hình ThangNK <23/01/2018>
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "The username or password provided is incorrect.");
            }

            log.Info("login failure");
            return View(loginModel);
        }


        public ActionResult LogOut()
        {
            log.Info(HttpContext.User.Identity.Name + " logout");
            userService.Logout(HttpContext.User.Identity.Name);
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePass()
        {
            return View();
        }

        public ActionResult DoChangePass(LoginModel loginModel)
        {
            string user = HttpContext.User.Identity.Name;
            bool rs = userService.CheckPass(user, loginModel.Password);

            if (rs)
            {
                userService.ChangePass(user, loginModel.Password, loginModel.NewPass);
                TempData["AlertMessage"] = Constants.MESSAGE_ALERT_UPDATE_SUCCESS;
            }
            else
            {
                ModelState.AddModelError("", Constants.MESSAGE_ALERT_UPDATE_FAILED);
            }

            return View("ChangePass", loginModel);
        }
    }
}