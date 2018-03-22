using Framework.MVC;
using Newtonsoft.Json;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Common;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Controllers
{
    public class UserController : BaseController
    {
        VipFeeDal _feeDal = new VipFeeDal();
        VipDal _dal = new VipDal();
        LessonDal _videoDal = new LessonDal();

        public ActionResult Index()
        {
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret, Request.Url.AbsoluteUri);
            ViewBag.JsPackage = jssdkUiPackage;

            var vip = _dal.Get<Vip>(CurrentVip.VipId);

            return View(vip);
        }

        [HttpPost]
        public ActionResult UploadImg(string serverId = "")
        {
            if (!string.IsNullOrEmpty(serverId))
            {
                var token = AccessTokenContainer.TryGetAccessToken(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret);
                var fileName = $"/upload/cardimg/{Guid.NewGuid().ToString("N")}.jpg";
                Senparc.Weixin.MP.AdvancedAPIs.MediaApi.Get(token, serverId, Server.MapPath("~" + fileName));

                var vip = GetVipInfo();
                vip.CardImg = fileName;

                _dal.Update(vip);

                return Json(AjaxResult.Success());
            }
            else
            {
                return Json(AjaxResult.Fail("请上传图片"));
            }
        }

        [HttpPost]
        public ActionResult GetPayRequest()
        {
            var vip = GetVipInfo();
            var reqData = new PayRequestData { VipName = vip.VipName };

            reqData = PayRequestData.GetPayData(vip, Request.UserHostAddress);
            var feeData = new VipFee
            {
                OrderCode = reqData.OrderCode,
                VipId = vip.Id,
                VipName = vip.VipName,
                Fee = reqData.Fee,
                Status = (int)PayStatus.待支付
            };

            _vipDal.Insert(feeData);

            return PartialView(reqData);
        }


        #region 我的缴费


        public ActionResult PayLog()
        {
            var vipFees = _dal.GetAll<VipFee>().Where(p => p.VipId == CurrentVip.VipId && p.Status == 1).OrderByDescending(p => p.FeeTime).ToList();
            return View(vipFees);
        }

        public ActionResult CommentReply()
        {
            var comments = _videoDal.GetVipComments(CurrentVip.VipId);

            return View(comments);
        }

        #endregion





    }
}