using Framework;
using Framework.Json;
using Framework.MVC;
using Framework.Security;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MvcExtension;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Common;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Controllers
{
    [WeixinInternalRequest("请通过微信客户端访问", Order = 1)]
    public class AccountController : Controller
    {
        protected CurrentVipModel CurrentVip
        {
            get
            {
                var userInfo = HttpContext.Items["CurrentVip"] as CurrentVipModel;
                if (userInfo == null)
                {
                    throw new UserFriendlyException("当前用户没有登录");
                }
                return userInfo;
            }
        }

        VipDal _dal = new VipDal();

        public ActionResult Login()
        {
            ViewBag.Title = "登录";

            return View();
        }


        [HttpPost]
        public ActionResult Login(string mobile, string password)
        {
            ViewBag.Title = "登录";

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "手机号或密码为空";
                return View();
            }
            var user = _dal.GetByMobile(mobile);
            if (user == null || user.Password != password)
            {
                ViewBag.Error = "手机号或密码错误";
                return View();

            }

            
            if (string.IsNullOrEmpty(user.WeChatId))
            {
                OAuthHelper.DoOAuth(HttpContext, mobile);
                return new EmptyResult();
            }

            var userInfo = new CurrentVipModel
            {
                VipId = user.Id,
                OpenId = user.WeChatId
            };

            SetAuthCookie(userInfo);

            HttpContext.Items["CurrentVip"] = userInfo;
            return RedirectToAction("Index","User");
        }


        //#region 注册

        public ActionResult Register()
        {
            ViewBag.Title = "注册";

            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret, Request.Url.AbsoluteUri);
            ViewBag.JsPackage = jssdkUiPackage;

            return View();
        }


        [HttpPost]
        public ActionResult Register(Vip vip, string verifyCode)
        {
            ViewBag.Title = "注册";

            if (string.IsNullOrEmpty(vip.MobileNo) || string.IsNullOrEmpty(vip.Password) || string.IsNullOrEmpty(vip.VipName))
            {
                ViewBag.Error = "请完善资料";
                return View();
            }
            //验证手机号是否存在
            var isCheck = _dal.GetByMobile(vip.MobileNo);
            if (isCheck != null)
            {
                ViewBag.Error = "手机号已存在";
                return View();
            }

            //验证身份证
            var user = _dal.GetByCardNo(vip.CardNo);
            if (user != null)
            {
                ViewBag.Error = "身份证号已存在";
                return View();
            }

            //验证码
            if (Session["VerCode"] == null || Session["PhoneNo"] == null || Session["VerCode"].ToString() != verifyCode || Session["PhoneNo"].ToString() != vip.MobileNo)
            {
                ViewBag.Error = "验证码不正确";
                return View();
            }

            var token = AccessTokenContainer.TryGetAccessToken(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret);
            if (!string.IsNullOrEmpty(vip.CardImg))
            {
                var fileName = $"/upload/cardimg/{Guid.NewGuid().ToString("N")}.jpg";
                Senparc.Weixin.MP.AdvancedAPIs.MediaApi.Get(token, vip.CardImg, Server.MapPath("~" + fileName));

                vip.CardImg = fileName;

            }

            //添加用户
            vip.CreatedBy = "system";
            vip.CreatedTime = DateTime.Now;
            vip.UpdatedBy = "system";
            vip.UpdatedTime = DateTime.Now;
            vip.ImgPath = "/assets/web/images/i_03.png";

            _dal.Insert(vip);

            OAuthHelper.DoOAuth(HttpContext, vip.MobileNo);

            return new EmptyResult();

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

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendSms(string mobile)
        {
            var err = string.Empty;
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(AjaxResult.Fail("手机号为空"), JsonRequestBehavior.AllowGet);
            }

            //判断手机号是否注册
            var user = _dal.GetByMobile(mobile);
            if (user != null)
            {
                return Json(AjaxResult.Fail("手机号已存在"), JsonRequestBehavior.AllowGet);
            }

            Random rd = new Random();
            int num = rd.Next(100000, 999999);
            Session["VerCode"] = num;
            Session["PhoneNo"] = mobile;
            //SendSMS.Send(PhoneNo, num);
            return Json(AjaxResult.Success(num.ToString()), JsonRequestBehavior.AllowGet);
        }
    }
}