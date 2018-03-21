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
    public class VideoCategoryController : AuthBaseController
    {
        LessonDal dal = new LessonDal();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxGet(DataTableAjaxConfig dtConfig)
        {
            var news = dal.GetPagedEntities<VideoClass>("select * from t_video_class", dtConfig).ToList();

            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = news
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddOrEdit(int id = 0)
        {

            if (id == 0)
            {
                ViewBag.Parents = new List<VideoClass>();
                return PartialView(new VideoClass());
            }
            else
            {

                var entity = dal.Get<VideoClass>(id);
                ViewBag.Parents = dal.GetAll<VideoClass>().Where(p => p.ClassLevel == entity.ClassLevel - 1).ToList();

                return PartialView(entity);
            }
        }

        public ActionResult AjaxGetParents(int level)
        {
            var parents = dal.GetAll<VideoClass>().Where(p => p.ClassLevel == (level - 1)).ToList();

            return PartialView(parents);
        }


        [HttpPost]
        public ActionResult AddOrEdit(VideoClass model)
        {
            if (model.Id == 0)
            {
                var entity = new VideoClass
                {
                    ClassLevel = model.ClassLevel,
                    ClassName = model.ClassName,
                    ParentId = model.ParentId,
                    CreatedBy = CurrentUser.LoginName,
                    CreatedTime = DateTime.Now,
                    UpdatedBy = CurrentUser.LoginName,
                    UpdatedTime = DateTime.Now
                };

                dal.Insert(entity);
            }
            else
            {
                var entity = dal.Get<VideoClass>(model.Id);
                entity.ClassLevel = model.ClassLevel;
                entity.ClassName = model.ClassName;
                entity.ParentId = model.ParentId;

                dal.Update(entity);
            }
            return Json(AjaxResult.Success());
        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            var entity = new VideoClass { Id = id };

            dal.Delete(entity);

            return Json(AjaxResult.Success());
        }
    }
}