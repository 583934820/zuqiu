using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.MP.MvcExtension;
using szzx.web.Filter;
using szzx.web.Models;
using Framework;
using szzx.web.Entity;
using szzx.web.DataAccess;
using szzx.web.Common;
using Senparc.Weixin.MP.TenPayLibV3;
using System.Text;

namespace szzx.web.Controllers
{
    [WeixinInternalRequest("请通过微信客户端访问",Order = 1)]
    [WebAuthorize(Order = 2)]
    public abstract class BaseController : Controller
    {
        protected VipDal _vipDal = new VipDal();

        protected string GetModelErrors()
        {
            var sb = new StringBuilder();
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors != null && state.Errors.Count > 0)
                {
                    foreach (var err in state.Errors)
                    {
                        sb.AppendFormat(",{0}", err.ErrorMessage);
                    }
                }
            }
            return sb.ToString().Substring(1);
        }

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

        protected Vip GetVipInfo()
        {
            return _vipDal.Get<Vip>(CurrentVip.VipId);
        }

        protected MpConfigModel GetMpConfigModel()
        {
            var mpconfig = _vipDal.GetAll<MPConfig>().FirstOrDefault();
            return mpconfig == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<MpConfigModel>(mpconfig.ConfigValue, new Newtonsoft.Json.JsonSerializerSettings
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore
            });
        }

        protected VipFee GetVipFee(int vipId)
        {
            return _vipDal.GetAll<VipFee>().Where(p => p.VipId == vipId && p.Status == 1).OrderByDescending(p => p.FeeTime).FirstOrDefault();
        }

        protected void DoRefund(Vip entity)
        {
            var vipFee = _vipDal.GetAll<VipFee>().Where(p => p.VipId == entity.Id && p.Status == (int)PayStatus.支付成功).OrderByDescending(p => p.FeeTime).FirstOrDefault();
            if (vipFee != null && entity.FeeStatus == (int)PayStatus.支付成功)
            {
                var refundReqData = RefundRequestData.GetRefundRequestData((int)vipFee.Fee, vipFee.OrderCode, Server.MapPath("~/App_Data/apiclient_cert.p12"));
                _vipDal.Insert<VipFeeRefund>(new VipFeeRefund
                {
                    RtnOrderCode = refundReqData.RtnOrderCode,
                    OrderCode = vipFee.OrderCode,
                    VipId = vipFee.VipId,
                    VipName = entity.VipName,
                    RefundFee = vipFee.Fee,
                    WXRtnOrderCode = refundReqData.WxRtnOrderCode,
                    Remark = refundReqData.Remark
                });

                if (refundReqData.RefundResult.ToLower() == "success")
                {
                    //entity.IsReturnFee = true;
                    entity.FeeStatus = (int)PayStatus.待支付;
                }
            }
        }

        //protected bool DoTransfer(Vip entity, VipFee vipFee)
        //{
        //    var _payInfo = TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
        //    var nonceStr = TenPayV3Util.GetNoncestr();
        //    var rtnOrderCode = string.Format("F{0}{1}{2}", _payInfo.MchId, DateTime.Now.ToString("yyyyMMddHHmmss"), TenPayV3Util.BuildRandomStr(6));

        //    //var _refundFee = (int)(refundFee * 100);
        //    var dataInfo = new TenPayV3TransfersRequestData(_payInfo.AppId, _payInfo.MchId, null, nonceStr, rtnOrderCode, entity.WeChatId, _payInfo.Key, "NO_CHECK", "", vipFee.Fee, $"{vipFee.VipName}加入球队的缴费退还", AppConfig.Instance.ServerIp);
        //    //var cert = @"D:\cert\apiclient_cert_SenparcRobot.p12";//根据自己的证书位置修改
        //    var password = _payInfo.MchId;//默认为商户号，建议修改
        //    var result = TenPayV3.Transfers(dataInfo, Server.MapPath("~/App_Data/apiclient_cert.p12"), password);

        //    _vipDal.Insert(new VipFeeTransfer
        //    {
        //        OrderCode = rtnOrderCode,
        //        ToVipId = entity.Id,
        //        ToVipName = entity.VipName,
        //        Fee = vipFee.Fee,
        //        TransferTime = DateTime.Now,
        //        STATUS = result.result_code.ToLower() == "success" ? 1 : 0,
        //        CreatedBy = entity.VipName,
        //        CreatedTime = DateTime.Now,
        //        WXOrderCode = result.payment_no,
        //        Remark = result.return_msg
        //    });

        //    Senparc.Weixin.WeixinTrace.SendCustomLog("付款日志", Newtonsoft.Json.JsonConvert.SerializeObject(vipFee) + "========" + Newtonsoft.Json.JsonConvert.SerializeObject(result));

        //    return result.result_code.ToLower() == "success";
        //}
    }
}