using Framework.Json;
using Framework.Security;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Models;
using szzx.web.DataAccess;

namespace szzx.web.Controllers
{
    public class OAuthController : Controller
    {
        private VipDal _dal = new VipDal();

        public ActionResult Callback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("授权无效");
            }

            //获取token
            try
            {
                var tokenResult = OAuthApi.GetAccessToken(AppConfig.Instance.AppId,
                                                AppConfig.Instance.AppSecret,
                                                code);
                if (tokenResult.errcode != Senparc.Weixin.ReturnCode.请求成功)
                {
                    return Content(tokenResult.errcode + ":" + tokenResult.errmsg);
                }

                var openId = tokenResult.openid;

                if (Session["VipMobile"] != null)
                {
                    var vip = _dal.GetByMobile(Session["VipMobile"].ToString());
                    if (vip != null)
                    {
                        vip.WeChatId = openId;
                        vip.UpdatedTime = DateTime.Now;

                        _dal.Update(vip);

                        SetAuthCookie(new CurrentVipModel
                        {
                            OpenId = openId,
                            VipId = vip.Id,
                            VipName = vip.VipName
                        });

                        return RedirectToAction("Index", "User");

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }

                //根据openId获取用户，如果没有找到就跳转注册页面，注册过就设置cookie,以后免登陆
                //var vip = _dal.GetVipByOpenId(openId);
                //if (vip == null)
                //{
                //    return RedirectToAction("Mobile", "Register", new { openId = openId });
                //}
                //else
                //{
                //    SetAuthCookie(new CurrentVipModel
                //    {
                //        OpenId = openId,
                //        VipId = vip.Id,
                //        VipName = vip.VipName
                //    });

                //    return RedirectToAction("Index", "Home");
                //}
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        private void SetAuthCookie(CurrentVipModel user)
        {
            var userInfo = user.ToJson();

            var cookie = new HttpCookie("webAuthData");
            cookie.Value = EncryptHelper.DESEncrypt(userInfo, ConfigurationManager.AppSettings["authKey"]);
            cookie.Expires = DateTime.Now.AddDays(3);
            cookie.Path = "/";
            cookie.HttpOnly = true;

            Response.Cookies.Remove("webAuthData");
            Response.Cookies.Add(cookie);

        }
    }
}