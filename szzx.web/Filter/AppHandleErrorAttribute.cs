using Framework;
using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace szzx.web
{
    public class AppHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var msg = (filterContext.Exception is UserFriendlyException) ? filterContext.Exception.Message : "服务器异常";
            //LoggerFactory.GetDefaultLogger().Error(filterContext.Exception);
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new AjaxResult
                    {
                        Code = 500,
                        Message = filterContext.Exception.Message + filterContext.Exception.StackTrace
                    }, JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                filterContext.HttpContext.Items["_ErrorMsg"] = filterContext.Exception.Message;
                base.OnException(filterContext);
            }
        }
    }
}