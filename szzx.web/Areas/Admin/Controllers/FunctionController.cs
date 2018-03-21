using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.BusinessService;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Areas.Admin.Controllers
{
    public class FunctionController : AuthBaseController
    {
       
        private FunctionService _funService;

        public FunctionController()
        {
         
            _funService = new FunctionService();
        }

        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult AjaxGetFunctions(DataTableAjaxConfig dtConfig)
        {
            var functions = _funService.GetPageFunctionList(dtConfig);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = functions
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int functionid = 0)
        {
            var functionModel = _funService.GetFunctionById(functionid);
            return Json(AjaxResult.Success(functionModel), JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult GetFunctionByLevel(int level = 0)
        {
            var functionModel = _funService.GetFunctionByLevel(level-1<0?0:level-1);
            return Json(AjaxResult.Success(functionModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public ActionResult Edit(FunctionModel model)
        {
            if (ModelState.IsValid)
            {
                _funService.EditFunction(model);
                return Json(AjaxResult.Success());
            }
            else
            {
                var erros = GetModelErrors();
                return Json(AjaxResult.Fail(erros));
            }
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(PermissionModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = CurrentUser.LoginName;
                model.UpdatedBy = CurrentUser.LoginName;
                _funService.AddFunction(model);
                return Json(AjaxResult.Success());
            }
            else
            {
                var erros = GetModelErrors();
                return Json(AjaxResult.Fail(erros));
            }
        }

        [HttpPost]
        public ActionResult Delete(int functionid=-1)
        {
            var count = _funService.GetChildFunction(functionid);
            if (count > 0) return Json(AjaxResult.Fail("该功能下还有子功能,无法删除"));

            _funService.Delete(new PermissionModel { Id=functionid});
            return Json(AjaxResult.Success());
        }
    }
}