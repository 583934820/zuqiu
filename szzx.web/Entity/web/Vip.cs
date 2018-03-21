using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Entity
{
    [Table("t_vip")]
    public class Vip 
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public string WeChatId { get; set; }
        public string VipName { get; set; }
        public string MobileNo { get; set; }
        public string Password { get; set; }
        public string CardImg { get; set; }
        public string CardNo { get; set; }
        public int HasCert { get; set; }

        public int FeeStatus { get; set; }


        public string CertNo { get; set; }

        public DateTime? ExpireDate { get; set; }

        public string ImgPath { get; set; }
        public decimal StudyTime { get; set; }

    }

    [Table("t_video_comment")]
    public  class VideoComment
    {
        public int Id { get; set; }
        public int VipId { get; set; }
        public int VideoId { get; set; }
        public string Comment { get; set; }
        public string Reply { get; set; }
        public int ReplyAdminId { get; set; }
        public Nullable<System.DateTime> ReplyTime { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedTime { get; set; }
        public string VipImgPath { get; set; }
        public string VipName { get; set; }        
    }

    public class VideoCommentModel : VideoComment
    {
        public string  Title { get; set; }
    }

    [Table("t_video_class")]
    public  class VideoClass
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int ClassLevel { get; set; }
        public int ParentId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedTime { get; set; }
    }

    [Table("t_video")]
    public partial class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImgPath { get; set; }
        public string VideoUrl { get; set; }
        public string CONTENT { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedTime { get; set; }
        public int ClassId { get; set; }
    }

    [Table("t_biz_vip_fee")]
    public class VipFee
    {
        [ExplicitKey]
        public string OrderCode { get; set; }
        public int VipId { get; set; }
        public string VipName { get; set; }

        public DateTime? FeeTime { get; set; }
        public decimal Fee { get; set; }
        /// <summary>
        /// 0-待支付，1-支付成功，2-支付失败
        /// </summary>
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "system";
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; } = "system";

        public string WXOrderCode { get; set; }
        public decimal WXFee { get; set; }
        public string Remark { get; set; }
    }

    [Table("t_biz_vip_fee_refund")]
    public class VipFeeRefund
    {
        [ExplicitKey]
        public string RtnOrderCode { get; set; }
        public string OrderCode { get; set; }
        public int VipId { get; set; }
        public string VipName { get; set; }

        public DateTime? RefundTime { get; set; }
        public decimal RefundFee { get; set; }
        /// <summary>
        /// 0-申请退款，1-退款成功，2-退款异常
        /// </summary>
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "system";
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; } = "system";

        public string WXRtnOrderCode { get; set; }
        public decimal WXRefundFee { get; set; }
        public string Remark { get; set; }
    }

    [Table("t_biz_action_his")]
    public class ActionHis
    {
        public int Id { get; set; }
        public int VipId { get; set; }
        public string Remark { get; set; }
        public string ActionType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}