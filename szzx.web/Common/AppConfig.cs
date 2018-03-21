using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace szzx.web
{
    public class AppConfig
    {
        public static AppConfig Instance = new AppConfig();

        private AppConfig()
        {

        }

        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string ContactNo { get; set; }
        public string ServerIp { get; set; } = "118.25.17.160";
        public string PayNotify { get; set; }
        public string TeamPayNotify { get; set; }
        public decimal Fee { get; set; }

        public void Initialize(string appId, string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
            //ContactNo = ConfigurationManager.AppSettings["ContactNo"];
            PayNotify = ConfigurationManager.AppSettings["TenPayV3_TenpayNotify"];
            Fee = 5000m;
            //TeamPayNotify = ConfigurationManager.AppSettings["Team_TenPayV3_TenpayNotify"];
        }
    }
}