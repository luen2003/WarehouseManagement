using System;
using System.Web;
using System.Web.Mvc;
using Resources;
using System.Web.Routing;
using PetroBM.Common.Util;


namespace PetroBM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new AuthorizationPrivilegeFilter());
        }
    }
    //public class AuthorizationPrivilegeFilter : ActionFilterAttribute
    //{

    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        string userId = HttpContext.Current.User.Identity.Name;
    //        HttpContext ctx = HttpContext.Current;
    //        string _SessionID = System.Web.HttpContext.Current.Session.SessionID;

    //        try
    //        {


    //            var wareHouse = ctx.Session[Constants.Session_WareHouse];
    //            if (userId != "" && wareHouse != null)
    //            {
    //                filterContext.Result = new RedirectToRouteResult(
    //                                new RouteValueDictionary{{ "controller", "Account" },
    //                                      { "action", "Login" }
 
    //                                     });
    //            }
    //            else
    //            {
    //                filterContext.Result = new RedirectToRouteResult(
    //                new RouteValueDictionary{{ "controller", "Account" },
    //                                      { "action", "Login" }
 
    //                                     });

    //            }
    //            base.OnActionExecuting(filterContext);

    //        }
    //        catch (Exception ex)
    //        {
    //            // Log.ErrorFormat(_SessionID + "  {0}", ex);
    //            ctx.Response.Redirect("~/Error/Index");
    //        }


    //    }

    //}

}
