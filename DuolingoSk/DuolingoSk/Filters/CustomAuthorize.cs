using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DuolingoSk.Filters
{
    public class CustomAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int userId = filterContext.HttpContext.Session["UserID"] != null ? Int32.Parse(filterContext.HttpContext.Session["UserID"].ToString()) : 0;

            if (userId == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            Exception = "error"
                        }
                    };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                {
                        { "controller", "Login" },
                        { "action", "Index" }
                });
                    //return;
                }
                return;

            }
            base.OnActionExecuting(filterContext);
        }

    }
}