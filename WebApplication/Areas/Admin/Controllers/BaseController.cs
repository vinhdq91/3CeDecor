using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using DAL.Core.Utilities;
using DAL.Models.Account;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using DAL.Common;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseController : Controller
    {
        // GET: Admin/Base
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ApplicationUser userLogin = SessionHelpers.GetObjectFromJson<ApplicationUser>(HttpContext.Session, Constant.ADMIN_GROUP);
            // Nếu user lấy từ session null hoặc không phải admin user thì không cho đăng nhập
            if (userLogin == null || (userLogin != null && !userLogin.IsAdminUser))
            {
                RouteValueDictionary route = new RouteValueDictionary(new { Controller = "AuthAdmin", Action = "Login" });
                filterContext.Result = new RedirectToRouteResult(route);
                return;
            }
        }
    }
}