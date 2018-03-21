using Framework.Json;
using Framework.MVC;
using Framework.Security;
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
using Senparc.Weixin.MP.TenPayLibV3;
using szzx.web.Filter;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.Containers;
using System.IO;

namespace szzx.web.Controllers
{
    [WeixinInternalRequest("请通过微信客户端访问", Order = 1)]
    public class RegisterController : Controller
    {

        private VipDal _dal = new VipDal();


        #region 注册
        public ActionResult Mobile(string openId)
        {
            ViewBag.PageTitle = "用户注册";
            if (string.IsNullOrEmpty(openId))
            {
                try
                {
                    Senparc.Weixin.WeixinTrace.SendCustomLog("Mobile", $"Mobile出错：{Request.HttpMethod}" );

                }
                catch (Exception)
                { }
                return Content("Mobile非法请求");
            }

            ViewBag.openId = openId;

            Session["OpenId"] = openId;
            return View();
        }

        [HttpPost]
        public ActionResult SendVerifyCode(string mobile, string openId)
        {           
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(AjaxResult.Fail("手机号为空"));
            }
            if (string.IsNullOrEmpty(openId))
            {
                return Json(AjaxResult.Fail("openId为空"));
            }

            var vip = _dal.GetByMobile(mobile);
            if (vip != null)
            {
                return Json(AjaxResult.Fail("手机号已存在"));
            }

            var code = new Random().Next(100000,999999).ToString();
            Session["VerifyCode"] = code;
            Session["MobileNo"] = mobile;
            Session["OpenId"] = openId;
            return Json(AjaxResult.Success(code));
        }

        public ActionResult VerifyCode()
        {
            ViewBag.PageTitle = "用户注册";
            return View();
        }

        [HttpPost]
        public ActionResult VerifyCode(string code)
        {
            if (code != Session["VerifyCode"].ToString())
            {
                ViewBag.Error = "验证码无效";
                return View();
            }
            return RedirectToAction("Password");
        }

        public ActionResult Password()
        {
            ViewBag.PageTitle = "用户注册";
            Session["token_SavePassword"] = Guid.NewGuid().ToString();
            return View();
        }

        [HttpPost]
        public ActionResult Save(string Pwd)
        {
            var mobile = Session["MobileNo"]?.ToString();
            var openId = Session["OpenId"]?.ToString();

            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(openId))
            {
                try
                {
                    Senparc.Weixin.WeixinTrace.SendCustomLog("SavePassword", $"SavePassword出错：{Request.HttpMethod}" + $"mobile:{mobile}" + $"openId:{openId}");

                }
                catch (Exception)
                { }
                return Json(AjaxResult.Success());
                //return Content("非法请求");
            }

            //验证是否存在手机号或openId
            var vip = _dal.GetByMobile(mobile);
            if (vip != null)
            {
                //ViewBag.Error = "手机号已存在";
                //return View("Password");
                if (Session["token_SavePassword"] == null)
                {
                    return Json(AjaxResult.Success());
                }
                else
                {
                    return Json(AjaxResult.Fail("手机号已存在"));
                }
                
            }

            //vip = _dal.GetVipByOpenId(openId);
            //if (vip != null)
            //{
            //    //ViewBag.Error = "用户已存在";
            //    //return View("Password");
            //    return Json(AjaxResult.Fail("手机号已存在"));

            //}

            _dal.Insert(new Vip
            {
                WeChatId = openId,
                VipName = "用户_" + mobile,
                MobileNo = mobile,
                Pwd = Pwd,
                CreatedBy = "system",
                UpdatedBy = "system",
                VipNo = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMdd"), _dal.GetMaxVipNo().ToString().PadLeft(4, '0'))
            });

             vip = _dal.GetVipByOpenId(openId);

            SetAuthCookie(new CurrentVipModel
            {
                OpenId = vip.WeChatId,
                VipId = vip.Id,
                VipName = vip.VipName
            });

            //Session.Remove("OpenId");
            //Session.Remove("MobileNo");
            Session.Remove("VerifyCode");
            Session["token_SavePassword"] = null;

            return Json(AjaxResult.Success());
        }


        private void SetAuthCookie(CurrentVipModel user)
        {
            var userInfo = user.ToJson();

            var cookie = new HttpCookie("webAuthData");
            cookie.Value = EncryptHelper.DESEncrypt(userInfo, ConfigurationManager.AppSettings["authKey"]);
            cookie.Expires = DateTime.Now.AddDays(30);
            cookie.Path = "/";
            cookie.HttpOnly = true;

            Response.Cookies.Remove("webAuthData");

            Response.Cookies.Add(cookie);

        }

        #endregion

        #region 认证

        [WebAuthorize(Order = 2)]
        public ActionResult AuthStep1(int id, string error = "")
        {
            ViewBag.PageTitle = "认证注册";
            ViewBag.Error = error;
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret, Request.Url.AbsoluteUri);
            ViewBag.JsPackage = jssdkUiPackage;

            var vip = _dal.Get<Vip>(id);
            Session["AuthVip"] = vip;
            return View(vip);
        }

        [HttpPost]
        public ActionResult SaveAuthStep1(Vip model)
        {
            Vip vip = null;
            if ( (vip = Session["AuthVip"] as Vip) != null)
            {
                vip.VipName = model.VipName;
                vip.Email = model.Email;
                vip.Age = model.Age;
                vip.CardNo = model.CardNo;
                var _temp = _dal.GetByCardNo(vip.CardNo);
                if ( _temp != null && _temp.Id != vip.Id)
                {
                    //return RedirectToAction("AuthStep1", new { id = model.Id, error = "身份证号已存在" });
                    return Json(AjaxResult.Fail("身份证号已存在"));
                }

                //Senparc.Weixin.WeixinTrace.SendCustomLog("上传图片日志", model.CardImgFront + ",," + model.CardImgBack);

                var token = AccessTokenContainer.TryGetAccessToken(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret);
                if (!string.IsNullOrEmpty(model.CardImgFront))
                {
                    var fileName = $"/upload/cardimg/{Guid.NewGuid().ToString("N")}.jpg";
                    Senparc.Weixin.MP.AdvancedAPIs.MediaApi.Get(token, model.CardImgFront, Server.MapPath("~/" + fileName));

                    vip.CardImgFront = fileName;

                }
                if (!string.IsNullOrEmpty(model.CardImgBack))
                {
                    var fileName = $"/upload/cardimg/{Guid.NewGuid().ToString("N")}.jpg";
                    Senparc.Weixin.MP.AdvancedAPIs.MediaApi.Get(token, model.CardImgBack, Server.MapPath("~/" + fileName));

                    vip.CardImgBack = fileName;
                }

                //Senparc.Weixin.WeixinTrace.SendCustomLog("上传图片日志", vip.CardImgFront + ",," + vip.CardImgBack);
                vip.Address = model.Address;

                Session["AuthVip"] = vip;
                //return RedirectToAction("AuthStep2", new { Id = vip.Id});
                return Json(AjaxResult.Success(new { Id = vip.Id }));
            }
            else
            {
                try
                {
                Senparc.Weixin.WeixinTrace.SendCustomLog("SaveAuthStep1", $"SaveAuthStep1出错：{Request.HttpMethod}" + Newtonsoft.Json.JsonConvert.SerializeObject(model ?? new Vip { VipName = "none" }));

                }
                catch (Exception)
                { }

                var _vip = Session["AuthVip"] as Vip;
                if (_vip != null)
                {
                    return Json(AjaxResult.Success(new { Id = _vip.Id }));
                }

                //return Content("SaveAuthStep1非法请求，Id 为0");
                return Json(AjaxResult.Fail("SaveAuthStep1非法请求，Id 为0"));
            }
        }

        [WebAuthorize(Order = 2)]
        public ActionResult AuthStep2(Vip model)
        {
            Vip vip = null;
            ViewBag.PageTitle = "认证注册";
            if ((vip = Session["AuthVip"] as Vip) != null)
            {
                var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret, Request.Url.AbsoluteUri);
                ViewBag.JsPackage = jssdkUiPackage;
                return View(vip);
            }
            else
            {
                try
                {
                Senparc.Weixin.WeixinTrace.SendCustomLog("AuthStep2", $"AuthStep2出错：{Request.HttpMethod}" + Newtonsoft.Json.JsonConvert.SerializeObject(model ?? new Vip { VipName = "none" }));

                }
                catch (Exception)
                { }

                var _vip = Session["AuthVip"] as Vip;
                if (_vip != null)
                {
                    return View(_vip);
                }

                return Content("AuthStep2非法请求，Id 为0");
            }
        }

        [HttpPost]
        public ActionResult Pay(Vip model)
        {
            ViewBag.PageTitle = "认证注册";
            Vip vip = null;
            if ( (vip = Session["AuthVip"] as Vip) != null)
            {
                var token = AccessTokenContainer.TryGetAccessToken(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret);
                if (!string.IsNullOrEmpty(model.JuzhuBack))
                {
                    var fileName = $"/upload/cardimg/{Guid.NewGuid().ToString("N")}.jpg";
                    Senparc.Weixin.MP.AdvancedAPIs.MediaApi.Get(token, model.JuzhuBack, Server.MapPath("~/" + fileName));

                    vip.JuzhuBack = fileName;

                }
                if (!string.IsNullOrEmpty(model.JuzhuFront))
                {
                    var fileName = $"/upload/cardimg/{Guid.NewGuid().ToString("N")}.jpg";
                    Senparc.Weixin.MP.AdvancedAPIs.MediaApi.Get(token, model.JuzhuFront, Server.MapPath("~/" + fileName));

                    vip.JuzhuFront = fileName;
                }

                vip.WXStatus = (int)WXStatus.待审核;

                _dal.Update(vip);

                //Session["AuthVip"] = null;

                //设置微信支付参数
                var reqData = PayRequestData.GetPayData(vip, Request.UserHostAddress);
                var feeData = new VipFee
                {
                    OrderCode = reqData.OrderCode,
                    VipId = vip.Id,
                    VipName = vip.VipName,
                    Fee = reqData.Fee,
                    Status = (int)PayStatus.待支付
                };

                _dal.Insert(feeData);
                Session["PayFeeData"] = reqData;

                //return View(reqData);
                return Json(AjaxResult.Success());
            }
            else
            {
                try
                {
                Senparc.Weixin.WeixinTrace.SendCustomLog("Pay", $"支付出错：{Request.HttpMethod}" + Newtonsoft.Json.JsonConvert.SerializeObject(model ?? new Vip {  VipName="none"}));

                }
                catch (Exception)
                { }

                return Content("Pay非法请求，Id 为0");
            }
        }

        [WebAuthorize(Order = 2)]
        public ActionResult PayFee()
        {
            PayRequestData feeData = null;
            if ((feeData = Session["PayFeeData"] as PayRequestData) != null)
            {
                return View("Pay", feeData);
            }
            else
            {
                return Content("PayFee非法请求");
            }
        }

        //private PayRequestData  GetPayData(Vip model)
        //{
        //    var _payInfo = TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
        //    var orderCode = string.Format("{0}{1}{2}", _payInfo.MchId, DateTime.Now.ToString("yyyyMMddHHmmss"), TenPayV3Util.BuildRandomStr(6));
        //    var productInfo = "苏州足协-认证注册";
        //    var price = 1;
        //    var timeStamp = TenPayV3Util.GetTimestamp();
        //    var nonestr = TenPayV3Util.GetNoncestr();

        //    var data = new TenPayV3UnifiedorderRequestData(_payInfo.AppId, _payInfo.MchId, productInfo, orderCode, price, Request.UserHostAddress, _payInfo.TenPayV3Notify,
        //                                                    Senparc.Weixin.MP.TenPayV3Type.JSAPI, model.WeChatId, _payInfo.Key, nonestr);
        //    //Senparc.Weixin.WeixinTrace.SendCustomLog("支付请求日志", Newtonsoft.Json.JsonConvert.SerializeObject(data));

        //    var result = TenPayV3.Unifiedorder(data);

        //    var package = string.Format("prepay_id={0}", result.prepay_id);

        //    var reqData =  new PayRequestData
        //    {
        //        OrderCode = orderCode,
        //        Fee = price,
        //        AppId = _payInfo.AppId,
        //        TimeStamp = timeStamp,
        //        NonceStr = nonestr,
        //        Package = package,
        //        PaySign = TenPayV3.GetJsPaySign(_payInfo.AppId, timeStamp, nonestr, package, _payInfo.Key)
        //    };

        //    //Senparc.Weixin.WeixinTrace.SendCustomLog("支付请求日志", Newtonsoft.Json.JsonConvert.SerializeObject(reqData));

        //    return reqData;
        //}        
        #endregion
    }
}