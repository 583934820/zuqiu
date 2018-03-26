using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.MvcExtension;
using Framework.Security;
using System.Configuration;
using Framework.Json;
using szzx.web.Models;
using System.Web.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using szzx.web.Entity;
using szzx.web.DataAccess;

namespace szzx.web.Filter
{
    public class WebAuthorizeAttribute : ActionFilterAttribute
    {
        //protected string _appId { get; set; } = ConfigurationManager.AppSettings["WeixinAppId"];
        //protected string _oauthCallbackUrl { get; set; } = "/OAuth/Callback";
        //protected OAuthScope _oauthScope { get; set; }

        private static readonly string _authKey = ConfigurationManager.AppSettings["authKey"];

        //public WebAuthorizeAttribute(OAuthScope oauthScope = OAuthScope.snsapi_userinfo) 
        //{
        //    _oauthScope = oauthScope;
        //}

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{

        //    try
        //    {
        //        var httpContext = filterContext.HttpContext;
        //        var authData = GetAuthData(httpContext);
        //        if (string.IsNullOrEmpty(authData))
        //        {
        //            DoOAuth(filterContext);
        //            return;
        //        }

        //        authData = EncryptHelper.DESDecrypt(authData, _authKey);
        //        if (string.IsNullOrEmpty(authData))
        //        {
        //            DoOAuth(filterContext);
        //            return;
        //        }

        //        var userInfo = authData.ToObject<CurrentVipModel>();//JsonHelper.Deseriailize<UserModel>(authData);
        //        if (userInfo == null || userInfo.VipId <= 0 || string.IsNullOrEmpty(userInfo.OpenId))
        //        {
        //            DoOAuth(filterContext);
        //            return;
        //        }
        //        //if (filterContext.HttpContext.Session["_IsValidUser"] == null)
        //        //{
        //        var vip = new DataAccess.VipDal().Get<Vip>(userInfo.VipId);
        //        if (vip == null)
        //        {
        //            DoOAuth(filterContext);
        //            return;
        //        }
        //        //else
        //        //{
        //        //    filterContext.HttpContext.Session["_IsValidUser"] = true;
        //        //}
        //        //}

        //        httpContext.Items["CurrentVip"] = userInfo;

        //    }
        //    catch(Exception ex)
        //    {                
        //        Senparc.Weixin.WeixinTrace.SendCustomLog(this.GetType().Name, ex.Message);

        //        filterContext.HttpContext.Response.Cookies.Clear();
        //        DoOAuth(filterContext);
        //    }
        //}

        //private void DoOAuth(ActionExecutingContext filterContext)
        //{
        //    //没有登录就做OAuth验证
        //    var callbackUrl = Senparc.Weixin.HttpUtility.UrlUtility.GenerateOAuthCallbackUrl(filterContext.HttpContext, _oauthCallbackUrl);
        //    var state = string.Format("{0}|{1}", "FromSenparc", DateTime.Now.Ticks);
        //    filterContext.HttpContext.Session["state"] = state;
        //    var url = OAuthApi.GetAuthorizeUrl(_appId, callbackUrl, state, _oauthScope);
        //    filterContext.Result = new RedirectResult(url);
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var httpContext = filterContext.HttpContext;
                var authData = GetAuthData(httpContext);
                if (string.IsNullOrEmpty(authData))
                {
                    RedirectLogin(filterContext);
                }

                authData = EncryptHelper.DESDecrypt(authData, _authKey);
                if (string.IsNullOrEmpty(authData))
                {
                    RedirectLogin(filterContext);
                }

                var userInfo = authData.ToObject<CurrentVipModel>();//JsonHelper.Deseriailize<UserModel>(authData);
                if (userInfo == null || userInfo.VipId <= 0)
                {
                    RedirectLogin(filterContext);

                }

                var dal = new VipDal();
                var vip = dal.Get<Vip>(userInfo.VipId);
                if (vip == null || Framework.Security.EncryptHelper.Md5(vip.Password) != userInfo.pwd)
                {
                    throw new ArgumentException("vip 不存在");
                }


                httpContext.Items["CurrentVip"] = userInfo;
            }
            catch(Exception ex)
            {
                var cookie = filterContext.HttpContext.Request.Cookies["webAuthData"];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-30);
                    filterContext.HttpContext.Response.SetCookie(cookie);
                }
                RedirectLogin(filterContext);
            }
        }

        private string GetAuthData(HttpContextBase context)
        {
            var cookie = context.Request.Cookies["webAuthData"];
            return cookie?.Value;
        }

        private void RedirectLogin(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}