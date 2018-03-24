using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.DataAccess;
using szzx.web.Entity;

namespace szzx.web.Controllers
{
    public class NoticeController : BaseController
    {
        NoticeDal dal = new NoticeDal();
        // GET: Notice
        public ActionResult Index()
        {
            ViewBag.Title = "活动推荐";

            var notices = dal.GetAll<Notice>().OrderByDescending(p => p.CreatedTime);

            return View(notices);
        }

        public ActionResult Detail(int id = 0)
        {
            ViewBag.Title = "活动详情";

            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            var notice = dal.Get<Notice>(id);

            return View(notice);
        }
    }
}