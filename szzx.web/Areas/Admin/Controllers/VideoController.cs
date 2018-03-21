using Framework.MVC;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Areas.Admin.Models;
using szzx.web.Common;
using szzx.web.DataAccess;
using szzx.web.Entity;

namespace szzx.web.Areas.Admin.Controllers
{
    public class VideoController : AuthBaseController
    {
        LessonDal dal = new LessonDal();

        // GET: Admin/Video
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxGet(DataTableAjaxConfig dtConfig, string title)
        {
            var whereStr = " where 1=1";
            if (!string.IsNullOrEmpty(title))
            {
                whereStr += $" and title like '%{title}%'";
            }

            var list = dal.GetPagedVideos(dtConfig, title);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = list
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            ViewBag.UpToken = QiNiuConfig.GetToken();
            if (id == 0)
            {
                return View(new Video());
            }
            else
            {
                ViewBag.CurrentUrl = Url.Content("~/Admin/Video");

                var entity = dal.Get<Video>(id);

                return View(entity);
            }
        }

        public ActionResult AjaxGetClass()
        {
            var classes = dal.GetAll<VideoClass>().Where(p => p.ClassLevel == 2).ToList();

            return PartialView(classes);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddOrEdit(Video model)
        {

            if (model.Id == 0)
            {
                model.CreatedBy = CurrentUser.LoginName;
                model.CreatedTime = DateTime.Now;
                model.UpdatedBy = CurrentUser.LoginName;
                model.UpdatedTime = DateTime.Now;

                dal.Insert(model);
            }
            else
            {
                var entity = dal.Get<Video>(model.Id);
                entity.Title = model.Title;
                entity.ImgPath = model.ImgPath;
                entity.VideoUrl = model.VideoUrl;
                entity.CONTENT = model.CONTENT;
                entity.ClassId = model.ClassId;

                dal.Update(entity);
            }
            return Json(AjaxResult.Success());

        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            var entity = new Video { Id = id };

            dal.Delete(entity);

            return Json(AjaxResult.Success());
        }
    }
}