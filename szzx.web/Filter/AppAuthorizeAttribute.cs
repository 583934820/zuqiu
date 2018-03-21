using Framework.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Framework.Json;
using szzx.web.Models;
using szzx.web.BusinessService;

namespace szzx.web
{
    public class AppAuthorizeAttribute : AuthorizeAttribute
    {
        private static readonly string _authKey = ConfigurationManager.AppSettings["authKey"];

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                var authData = GetAuthData(httpContext);
                if (string.IsNullOrEmpty(authData))
                {
                    return false;
                }

                authData = EncryptHelper.DESDecrypt(authData, _authKey);
                if (string.IsNullOrEmpty(authData))
                {
                    return false;
                }

                var userInfo = authData.ToObject<UserModel>();//JsonHelper.Deseriailize<UserModel>(authData);
                if (userInfo == null || userInfo.Id <= 0)
                {
                    return false;
                }

                if (!httpContext.Request.IsAjaxRequest())
                {
                    userInfo.Functions = GetUserFunctions(userInfo.Id, httpContext.Request.Path);
                }
                
                httpContext.Items["CurrentUser"] = userInfo;
                return true;
            }
            catch
            {
                return false;
            }

        }

        private IEnumerable<FunctionModel> GetUserFunctions(int userId, string url)
        {
            var userService = new UserService();
            var functions = userService.GetUserFunctions(userId).ToList();
            HttpContext.Current.Items["Title"] = "Leapmed";

            //for (var i = 0; i < functions.Count(); i++)
            //{
            //    var fun = functions.ElementAt(i);
            //    if (fun.PathUrl!=null && fun.PathUrl.ToLower() == url.ToLower())
            //    {

            //            fun.IsActive = true;
            //            break;
                    
            //    }
            //}
            return functions;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RedirectLogin(filterContext);
        }

        private void RedirectLogin(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Admin/Account/Login");
        }

        private string GetAuthData(HttpContextBase context)
        {
            var cookie = context.Request.Cookies["authData"];
            return cookie?.Value;
        }
    }
}