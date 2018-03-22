using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using szzx.web.Entity;

namespace szzx.web.DataAccess
{
    public class NoticeDal : BaseDal
    {
        public IEnumerable<Notice> GetNoticeList(DataTableAjaxConfig config)
        {
            return GetPagedEntities<Notice>("select * from t_biz_notice ", config, null, "createdTime", false);
        }
    }
}