
using PetroBM.Common.Util;
using PetroBM.Services.Services;
using System.Web.Mvc;

namespace PetroBM.Web.Attribute
{
    public class HasPermissionAttribute : ActionFilterAttribute
    {
        private int permission;
        private IUserService userService;

        public HasPermissionAttribute(int permission)
        {
            this.permission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.userService = DependencyResolver.Current.GetService<IUserService>();

            if (!userService.CheckPermission(filterContext.HttpContext.User.Identity.Name, permission, Constants.FLAG_ON))
            {
                // If this user does not have the required permission then redirect to login page
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("/Home/Index");
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }
        }
    }
}