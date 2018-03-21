using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.TenPayLib;
using Senparc.Weixin.MP.TenPayLibV3;
using Senparc.Weixin.Threads;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using szzx.web.Common;

namespace szzx.web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //全局异常处理
            GlobalFilters.Filters.Add(new AppHandleErrorAttribute());

            AppConfig.Instance.Initialize(ConfigurationManager.AppSettings["WeixinAppId"], ConfigurationManager.AppSettings["WeixinAppSecret"]);
            InitQiniuConfig();

            ConfigWeixinTraceLog();     //配置微信跟踪日志（按需）
            RegisterWeixinThreads();    //激活微信缓存及队列线程（必须）
            RegisterSenparcWeixin();    //注册Demo所用微信公众号的账号信息（按需）
            //RegisterSenparcQyWeixin();  //注册Demo所用微信企业号的账号信息（已经移植到Work）
            //RegisterSenparcWorkWeixin();  //注册Demo所用企业微信的账号信息（按需）
            RegisterWeixinPay();        //注册微信支付（按需）
        }

        private void InitQiniuConfig()
        {
            QiNiuConfig.AccessKey = ConfigurationManager.AppSettings["AccessKey"];
            QiNiuConfig.SecretKey = ConfigurationManager.AppSettings["SecretKey"];
            QiNiuConfig.Bucket = ConfigurationManager.AppSettings["Bucket"];
            QiNiuConfig.Domain = ConfigurationManager.AppSettings["Domain"];
        }

        private void StartJobs()
        {
            JobScheduler.Add(new AutoPayNotifyJob());

            JobScheduler.Start();
        }

        /// <summary>
        /// 注册Demo所用微信公众号的账号信息
        /// </summary>
        private void RegisterSenparcWeixin()
        {
            //注册公众号
            AccessTokenContainer.Register(
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"],
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"],
                "【中捷足球】公众号");

            //注册小程序（完美兼容）
            //AccessTokenContainer.Register(
            //    System.Configuration.ConfigurationManager.AppSettings["WxOpenAppId"],
            //    System.Configuration.ConfigurationManager.AppSettings["WxOpenAppSecret"],
            //    "【盛派互动】小程序");
        }

        /// <summary>
        /// 配置微信跟踪日志
        /// </summary>
        private void ConfigWeixinTraceLog()
        {
            //这里设为Debug状态时，/App_Data/WeixinTraceLog/目录下会生成日志文件记录所有的API请求日志，正式发布版本建议关闭
            Senparc.Weixin.Config.IsDebug = true;
            Senparc.Weixin.WeixinTrace.SendCustomLog("系统日志", "系统启动");//只在Senparc.Weixin.Config.IsDebug = true的情况下生效

            //自定义日志记录回调
            //Senparc.Weixin.WeixinTrace.OnLogFunc = () =>
            //{
            //    //加入每次触发Log后需要执行的代码
            //};

            ////当发生基于WeixinException的异常时触发
            //Senparc.Weixin.WeixinTrace.OnWeixinExceptionFunc = ex =>
            //{
            //    //加入每次触发WeixinExceptionLog后需要执行的代码

            //    //发送模板消息给管理员
            //    var eventService = new EventService();
            //    eventService.ConfigOnWeixinExceptionFunc(ex);
            //};
        }

        /// <summary>
        /// 注册微信支付
        /// </summary>
        private void RegisterWeixinPay()
        {
            //提供微信支付信息
            //var weixinPay_PartnerId = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_PartnerId"];
            //var weixinPay_Key = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_Key"];
            //var weixinPay_AppId = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_AppId"];
            //var weixinPay_AppKey = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_AppKey"];
            //var weixinPay_TenpayNotify = System.Configuration.ConfigurationManager.AppSettings["WeixinPay_TenpayNotify"];

            var tenPayV3_MchId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_MchId"];
            var tenPayV3_Key = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_Key"];
            var tenPayV3_AppId = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppId"];
            var tenPayV3_AppSecret = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_AppSecret"];
            var tenPayV3_TenpayNotify = System.Configuration.ConfigurationManager.AppSettings["TenPayV3_TenpayNotify"];

            //var weixinPayInfo = new TenPayInfo(weixinPay_PartnerId, weixinPay_Key, weixinPay_AppId, weixinPay_AppKey, weixinPay_TenpayNotify);
            //TenPayInfoCollection.Register(weixinPayInfo);
            var tenPayV3Info = new TenPayV3Info(tenPayV3_AppId, tenPayV3_AppSecret, tenPayV3_MchId, tenPayV3_Key,
                                                tenPayV3_TenpayNotify);
            TenPayV3InfoCollection.Register(tenPayV3Info);
        }

        /// <summary>
        /// 激活微信缓存
        /// </summary>
        private void RegisterWeixinThreads()
        {
            ThreadUtility.Register();//如果不注册此线程，则AccessToken、JsTicket等都无法使用SDK自动储存和管理。
        }
    }
}
