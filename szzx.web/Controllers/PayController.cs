using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using szzx.web.Common;
using szzx.web.DataAccess;
using szzx.web.Entity;

namespace szzx.web.Controllers
{
    public class PayController : Controller
    {
        VipDal _dal = new VipDal();

        // GET: Pay
        public ActionResult PayNotify()
        {
            try
            {
                ResponseHandler resHandler = new ResponseHandler(null);

                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");

                string res = null;

                var payInfo = TenPayV3InfoCollection.Data[System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"]];
                resHandler.SetKey(payInfo.Key);
                //验证请求是否从微信发过来（安全）
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
                {
                    res = "success";//正确的订单处理
                    //直到这里，才能认为交易真正成功了，可以进行数据库操作，但是别忘了返回规定格式的消息！
                    var orderCode = resHandler.GetParameter("out_trade_no");
                    var wxOrderCode = resHandler.GetParameter("transaction_id");
                    var fee = resHandler.GetParameter("total_fee");

                    var vipFee = _dal.Get<VipFee>(orderCode);
                    if (vipFee != null)
                    {
                        vipFee.FeeTime = DateTime.Now;
                        vipFee.WXFee = decimal.Parse(fee);
                        vipFee.WXOrderCode = wxOrderCode;
                        vipFee.Status = 1;
                        vipFee.UpdatedBy = "paycallback";
                        vipFee.Remark = "支付成功";
                        _dal.Update(vipFee);

                        var vip = _dal.Get<Vip>(vipFee.VipId);
                        if (vip != null)
                        {
                            vip.FeeStatus = (int)PayStatus.支付成功;
                            vip.ExpireDate = vip.ExpireDate == null ? DateTime.Now.AddYears(1) : vip.ExpireDate.Value.AddYears(1);
                            //vip.WXStatus = (int)WXStatus.待审核;
                            _dal.Update(vip);
                        }
                    }
                    
                }
                else
                {
                    res = "wrong";//错误的订单处理
                }

                #region 注释
                /* 这里可以进行订单处理的逻辑 */

                //发送支付成功的模板消息
                //try
                //{
                //    string appId = AppConfig.Instance.AppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
                //    string openId = resHandler.GetParameter("openid");
                //    var templateData = new WeixinTemplate_PaySuccess("https://weixin.senparc.com", "购买商品", "状态：" + return_code);

                //    Senparc.Weixin.WeixinTrace.SendCustomLog("支付成功模板消息参数", appId + " , " + openId);

                //    var result = AdvancedAPIs.TemplateApi.SendTemplateMessage(appId, openId, templateData);
                //}
                //catch (Exception ex)
                //{
                //    Senparc.Weixin.WeixinTrace.SendCustomLog("支付成功模板消息", ex.ToString());
                //}
                #endregion

                #region 记录日志

                var logDir = Server.MapPath(string.Format("~/App_Data/TenPayNotify/{0}", DateTime.Now.ToString("yyyyMMdd")));
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                var logPath = Path.Combine(logDir, string.Format("{0}-{1}-{2}.txt", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), Guid.NewGuid().ToString("n").Substring(0, 8)));

                using (var fileStream = System.IO.File.OpenWrite(logPath))
                {
                    var notifyXml = resHandler.ParseXML();
                    //fileStream.Write(Encoding.Default.GetBytes(res), 0, Encoding.Default.GetByteCount(res));

                    fileStream.Write(Encoding.Default.GetBytes(notifyXml), 0, Encoding.Default.GetByteCount(notifyXml));
                    fileStream.Close();
                }

                #endregion


                string xml = string.Format(@"<xml>
<return_code><![CDATA[{0}]]></return_code>
<return_msg><![CDATA[{1}]]></return_msg>
</xml>", return_code, return_msg);
                return Content(xml, "text/xml");
            }
            catch (Exception ex)
            {
                new WeixinException(ex.Message, ex);
                throw;
            }
        }
    }
}