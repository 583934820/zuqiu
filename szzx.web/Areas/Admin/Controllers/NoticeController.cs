using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.DataAccess;
using szzx.web.Entity;

namespace szzx.web.Areas.Admin.Controllers
{
    public class NoticeController : AuthBaseController
    {
        NoticeDal _dal = new NoticeDal();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxGet(DataTableAjaxConfig dtConfig)
        {
            var miles = _dal.GetNoticeList(dtConfig);
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
            if (id == 0)
            {
                return View(new Notice());
            }
            else
            {
                var mile = _dal.Get<Notice>(id);

                return View(mile);
            }
            
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddOrEdit(Notice model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.PublishUser = CurrentUser.UserName;
                    model.CreatedBy = CurrentUser.UserName;
                    model.UpdatedBy = CurrentUser.UserName;

                    _dal.Insert(model);
                }
                else
                {
                    var entity = _dal.Get<Notice>(model.Id);
                    if (entity != null)
                    {
                        entity.Title = model.Title;
                        entity.Content = model.Content;
                        entity.ImgPath = model.ImgPath;
                        entity.UpdatedTime = DateTime.Now;

                        _dal.Update(entity);
                    }
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
            _dal.Delete(new Notice { Id = id });

            return Json(AjaxResult.Success());
        }
    }
}