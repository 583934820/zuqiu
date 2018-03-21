using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Entity
{
    [Table("t_biz_notice")]
    public class Notice : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImgPath { get; set; }
        public string PublishUser { get; set; }
    }


}