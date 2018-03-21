using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace szzx.web.Common
{
    public static class OAuthHelper
    {
        static string _oauthCallbackUrl  = "/OAuth/Callback";

        public static void DoOAuth(HttpContextBase httpContext, string mobile)
        {
            httpContext.Session["VipMobile"] = mobile;
            var callbackUrl = Senparc.Weixin.HttpUtility.UrlUtility.GenerateOAuthCallbackUrl(httpContext, _oauthCallbackUrl);
            var state = string.Format("{0}|{1}", "FromSenparc", DateTime.Now.Ticks);
            httpContext.Session["state"] = state;
            var url = OAuthApi.GetAuthorizeUrl(AppConfig.Instance.AppId, callbackUrl, state, OAuthScope.snsapi_userinfo);
            httpContext.Response.Redirect(url);
        }
    }
}