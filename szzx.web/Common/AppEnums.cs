using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Common
{
    public enum WXStatus
    {
        已注册 = 0,
        待审核,
        审核成功,
        审核失败,
        未续费
    }

    public enum PayStatus
    {
        待支付 = 0,
        支付成功,
        支付失败
    }

    public enum WellPosition
    {
        均可 = 0,
        前锋,
        中锋,
        后卫
    }

    public enum MPConfigType
    {
        重新加入球队日期 = 1,
        缴费时间段
    }

    public enum TeamPlayerActionType
    {
        加入 = 1,
        退出,
        移除
    }

    public enum ActionTypeEnum
    {
        移除球员
    }
}