using Senparc.Weixin.Context;
//using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Entities;

namespace szzx.web.MessageHandlers
{
    public class CustomerMessageHandler : MessageHandler<CustomMessageContext>
    {
        public CustomerMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            //WeixinContext.ExpireMinutes = 3;

            //if (!string.IsNullOrEmpty(postModel.AppId))
            //{
            //    appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            //}

            //在指定条件下，不使用消息去重
            //base.OmitRepeatedMessageFunc = requestMessage =>
            //{
            //    var textRequestMessage = requestMessage as RequestMessageText;
            //    if (textRequestMessage != null && textRequestMessage.Content == "容错")
            //    {
            //        return false;
            //    }
            //    return true;
            //};
        }

        public override Senparc.Weixin.MP.Entities.IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            return DefautMessageHande();
        }

        private IResponseMessageBase DefautMessageHande()
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您好，欢迎关注【中捷足球】公众号";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            return DefautMessageHande();

        }

        public override IResponseMessageBase OnEvent_TemplateSendJobFinishRequest(RequestMessageEvent_TemplateSendJobFinish requestMessage)
        {
            switch (requestMessage.Status)
            {
                case "failed:user block":
                case "failed: system failed": SendWxStatusTemplateMsgFailHandle();break;
            }
            return null;
        }

        private void SendWxStatusTemplateMsgFailHandle()
        {
            //TODO 发送短信
        }

        public override Senparc.Weixin.MP.Entities.IResponseMessageBase DefaultResponseMessage(Senparc.Weixin.MP.Entities.IRequestMessageBase requestMessage)
        {
            return DefautMessageHande();

        }
    }

    public class CustomMessageContext : MessageContext<IRequestMessageBase, IResponseMessageBase>
    {

    }
}