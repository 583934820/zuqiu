using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Models
{
    public class VipModel
    {
        public int Id { get; set; }
        public string WeChatId { get; set; }
        public string VipName { get; set; }
        public int Age { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string CardImgFront { get; set; }
        public string CardImgBack { get; set; }
        public string JuzhuFront { get; set; }
        public string JuzhuBack { get; set; }
        /// <summary>
        /// 0-已注册 1-待审核 2-审核成功 3-审核失败
        /// </summary>
        public int WXStatus { get; set; }
        public string CardNo { get; set; }
        public string Address { get; set; }
        public int FeeStatus { get; set; }
        public string Pwd { get; set; }
        public string ImgPath { get; set; }

        public string VipNo { get; set; }
        public string WellPosition { get; set; }
        public int IsInTeam { get; set; }
        public string RemoveReason { get; set; }
    }
}