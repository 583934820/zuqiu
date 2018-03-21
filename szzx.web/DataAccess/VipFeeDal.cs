using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using szzx.web.Common;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.DataAccess
{
    public class VipFeeDal:BaseDal
    {
        public List<VipFee> GetPagedFeeHistory(PagedModel model, int vipId)
        {
            return GetAll<VipFee>().Where(p => p.Status == (int)PayStatus.支付成功 && p.VipId == vipId).OrderByDescending(p => p.FeeTime)
                                    .Skip(model.PageSize * (model.PageIndex - 1)).Take(model.PageSize).ToList();
        }
    }
}