using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Common
{
    public static class TemplateMsgMethod
    {
        private static string SendWxStatusNotifyMsgId { get; } = "wwuMrs_D03xrHPyyvNRRNZ4t_9qTKfwn2BIrgYzm5IA";

        public static void SendWxStatusNotify(string tokenOrAppId, string openId, string auditResult, DateTime auditDate, string vipName, string vipNo)
        {
            TemplateApi.SendTemplateMessage(tokenOrAppId, openId, new TemplateMessage_SendWxStatusNotify(auditResult, auditDate, vipName, vipNo, SendWxStatusNotifyMsgId));
        }
    }

    public class TemplateMessage_SendWxStatusNotify : TemplateMessageBase
    {
        public TemplateMessage_SendWxStatusNotify(string auditResult, DateTime auditDate, string vipName, string vipNo,
            string templateId, string url = null, string templateName = "认证审核结果通知") : base(templateId, url, templateName)
        {
            /*
             * {{first.DATA}}
                审核事项：{{keyword1.DATA}}
                年报事项：{{keyword2.DATA}}
                审核部门：{{keyword3.DATA}}
                审核结果：{{keyword4.DATA}}
                审核日期：{{keyword5.DATA}}
                {{remark.DATA}}
             */
            first = new TemplateDataItem( $"{vipName}[会员编号:{vipNo}]");
            keyword1 = new TemplateDataItem("注册认证资料审核");
            keyword3 = new TemplateDataItem("苏州足协管理员");
            keyword4 = new TemplateDataItem(auditResult);
            keyword5 = new TemplateDataItem(auditDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public TemplateDataItem first { get; set; }

        public TemplateDataItem keyword1 { get; set; }

        public TemplateDataItem keyword3 { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public TemplateDataItem keyword4 { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public TemplateDataItem keyword5 { get; set; }
    }
}