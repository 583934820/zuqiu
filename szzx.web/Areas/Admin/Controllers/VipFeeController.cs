using Framework.Json;
using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Common;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Areas.Admin.Controllers
{
    public class VipFeeController : AuthBaseController
    {
        VipDal _dal = new VipDal();

        // GET: Admin/VipFee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxGet(DataTableAjaxConfig dtConfig, string keyword = "")
        {
            var miles = _dal.GetPagedVipFees(dtConfig, keyword);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = miles
            }, JsonRequestBehavior.AllowGet);
        }
    }
}