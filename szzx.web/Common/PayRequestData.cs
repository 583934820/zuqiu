using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Common
{
    public class PayRequestData
    {
        public string AppId { get; set; }
        public string TimeStamp { get; set; }
        public string NonceStr { get; set; }
        public string Package { get; set; }
        public string PaySign { get; set; }
        public string OrderCode { get; set; }
        /// <summary>
        /// 费用，单位 分
        /// </summary>
        public decimal Fee { get; set; }
        public string VipName { get; set; }

        static MpConfigModel GetMpConfigModel()
        {
            var _vipDal = new VipDal();
            var mpconfig = _vipDal.GetAll<MPConfig>().FirstOrDefault();
            return mpconfig == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<MpConfigModel>(mpconfig.ConfigValue, new Newtonsoft.Json.JsonSerializerSettings
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore
            });
        }

        public static PayRequestData GetPayData(Vip model, string hostAddress)
        {
            //var _payInfo = TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
            var _payInfo = GetTenPayInfo();
            _payInfo.TenPayV3Notify = AppConfig.Instance.PayNotify ;
            var orderCode = string.Format("{0}{1}{2}", _payInfo.MchId, DateTime.Now.ToString("yyyyMMddHHmmss"), TenPayV3Util.BuildRandomStr(6));
            var productInfo = "中捷足球-教练培训费";

            //var mpconfig = GetMpConfigModel();
            var price = 5000 * 100; //(int)((mpconfig == null  || mpconfig.VipFee == 0 ? 50 : mpconfig.VipFee) * 100);
            var timeStamp = TenPayV3Util.GetTimestamp();
            var nonestr = TenPayV3Util.GetNoncestr();

            var data = new TenPayV3UnifiedorderRequestData(_payInfo.AppId, _payInfo.MchId, productInfo, orderCode, price, hostAddress, _payInfo.TenPayV3Notify,
                                                            Senparc.Weixin.MP.TenPayV3Type.JSAPI, model.WeChatId, _payInfo.Key, nonestr);
            Senparc.Weixin.WeixinTrace.SendCustomLog("支付请求日志1", Newtonsoft.Json.JsonConvert.SerializeObject(data));

            var result = TenPayV3.Unifiedorder(data);

            var package = string.Format("prepay_id={0}", result.prepay_id);

            var reqData = new PayRequestData
            {
                OrderCode = orderCode,
                Fee = price,
                AppId = _payInfo.AppId,
                TimeStamp = timeStamp,
                NonceStr = nonestr,
                Package = package,
                PaySign = TenPayV3.GetJsPaySign(_payInfo.AppId, timeStamp, nonestr, package, _payInfo.Key),
                VipName = model.VipName
            };

            Senparc.Weixin.WeixinTrace.SendCustomLog("支付请求日志1", Newtonsoft.Json.JsonConvert.SerializeObject(reqData));


            return reqData;
        }

        static TenPayV3Info GetTenPayInfo()
        {
            var tenPayV3_MchId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"];
            var tenPayV3_Key = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_Key"];
            var tenPayV3_AppId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppId"];
            var tenPayV3_AppSecret = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppSecret"];
            //var tenPayV3_TenpayNotify = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_TenpayNotify"];

            //var weixinPayInfo = new TenPayInfo(weixinPay_PartnerId, weixinPay_Key, weixinPay_AppId, weixinPay_AppKey, weixinPay_TenpayNotify);
            //TenPayInfoCollection.Register(weixinPayInfo);
            var tenPayV3Info = new TenPayV3Info(tenPayV3_AppId, tenPayV3_AppSecret, tenPayV3_MchId, tenPayV3_Key,
                                                "");

            return tenPayV3Info;
        }

        public static PayRequestData GetTeamPayData(Vip model, string hostAddress)
        {
            //var _payInfo = TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
            var _payInfo = GetTenPayInfo();
            _payInfo.TenPayV3Notify = AppConfig.Instance.TeamPayNotify;
            var orderCode = string.Format("T{0}{1}{2}", _payInfo.MchId, DateTime.Now.ToString("yyyyMMddHHmmss"), TenPayV3Util.BuildRandomStr(6));
            var productInfo = "苏州足协-建立球队缴费";

            var mpconfig = GetMpConfigModel();
            var price = (int)((mpconfig == null || mpconfig.TeamFee == 0 ? 300 : mpconfig.TeamFee) * 100);

            var timeStamp = TenPayV3Util.GetTimestamp();
            var nonestr = TenPayV3Util.GetNoncestr();

            var data = new TenPayV3UnifiedorderRequestData(_payInfo.AppId, _payInfo.MchId, productInfo, orderCode, price, hostAddress, _payInfo.TenPayV3Notify,
                                                            Senparc.Weixin.MP.TenPayV3Type.JSAPI, model.WeChatId, _payInfo.Key, nonestr);
            Senparc.Weixin.WeixinTrace.SendCustomLog("支付请求日志2", Newtonsoft.Json.JsonConvert.SerializeObject(data));

            var result = TenPayV3.Unifiedorder(data);

            var package = string.Format("prepay_id={0}", result.prepay_id);

            var reqData = new PayRequestData
            {
                OrderCode = orderCode,
                Fee = price,
                AppId = _payInfo.AppId,
                TimeStamp = timeStamp,
                NonceStr = nonestr,
                Package = package,
                PaySign = TenPayV3.GetJsPaySign(_payInfo.AppId, timeStamp, nonestr, package, _payInfo.Key),
                VipName = model.VipName
            };

            Senparc.Weixin.WeixinTrace.SendCustomLog("支付请求日志2", Newtonsoft.Json.JsonConvert.SerializeObject(reqData));

            return reqData;
        }
    }

    public class RefundRequestData
    {
        public string RefundResult { get; set; }
        public string RtnOrderCode { get; set; }
        public int RefundFee { get; set; }
        public string Remark { get; set; }
        public string WxRtnOrderCode { get; set; }


        public static RefundRequestData GetRefundRequestData(int refundFee, string orderCode, string cert)
        {
            var _payInfo = TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
            var nonceStr = TenPayV3Util.GetNoncestr();
            var rtnOrderCode = string.Format("RTN{0}{1}{2}", _payInfo.MchId, DateTime.Now.ToString("yyyyMMddHHmmss"), TenPayV3Util.BuildRandomStr(6));

            //var _refundFee = (int)(refundFee * 100);
            var dataInfo = new TenPayV3RefundRequestData(_payInfo.AppId, _payInfo.MchId, _payInfo.Key,
                null, nonceStr, null, orderCode, rtnOrderCode, refundFee, refundFee, _payInfo.MchId, null);
            //var cert = @"D:\cert\apiclient_cert_SenparcRobot.p12";//根据自己的证书位置修改
            var password = _payInfo.MchId;//默认为商户号，建议修改
            var result = TenPayV3.Refund(dataInfo, cert, password);

            return new RefundRequestData
            {
                RefundFee = refundFee,
                RefundResult = result.result_code,
                RtnOrderCode = rtnOrderCode,
                Remark = result.err_code_des,
                 WxRtnOrderCode = result.transaction_id ?? ""
            };
        }
    }
}