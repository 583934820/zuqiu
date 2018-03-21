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
    public class VideoCommentController : AuthBaseController
    {
        LessonDal _dal = new LessonDal();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxGet(DataTableAjaxConfig dtConfig, int isReply = 0, string videoName = "")
        {
            var miles = _dal.GetPagedComments(dtConfig, isReply, videoName);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = miles
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            var mile = _dal.Get<VideoComment>(id);

            return Json(AjaxResult.Success(mile), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrEdit(VideoComment model)
        {
            if (ModelState.IsValid)
            {
                var entity = _dal.Get<VideoComment>(model.Id);
                if (entity != null)
                {
                    entity.Reply = model.Reply;
                    entity.ReplyAdminId = CurrentUser.Id;
                    entity.ReplyTime = DateTime.Now;
                    entity.UpdatedTime = DateTime.Now;

                    _dal.Update(entity);

                }
                return Json(AjaxResult.Success());
            }
            else
            {
                var erros = GetModelErrors();
                return Json(AjaxResult.Fail(erros));
            }
        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            _dal.Delete(new VideoComment { Id = id });

            return Json(AjaxResult.Success());
        }
    }
}