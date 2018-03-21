using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Entity
{
    [Table("t_biz_menu")]
    public class Menu:BaseEntity
    {
        public string Url { get; set; }
        public string MenuName { get; set; }
        public int MenuLevel  { get; set; }
        public int MenuParentId { get; set; }
        public string MenuParentName { get; set; }
    }
}