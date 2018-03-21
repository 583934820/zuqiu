using Framework;
using Framework.MVC;
using System.Web.Mvc;
using szzx.web.BusinessService;
using szzx.web.Models;

namespace szzx.web.Areas.Admin.Controllers
{
    public class RoleController : AuthBaseController
    {
        private RoleService _roleService;
        private FunctionService _funService;

        public RoleController()
        {
            _roleService = new RoleService();
            _funService = new FunctionService();
        }

        public ActionResult Index()
        {
            //var roles = _roleService.GetRoles();
            return View();
        }

        public ActionResult AjaxGetRoles(DataTableAjaxConfig dtConfig)
        {
            var roles = _roleService.GetRoles(dtConfig);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = roles
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddRoleModel addRoleModel)
        {
            if (ModelState.IsValid)
            {
                addRoleModel.CreatedBy = CurrentUser.LoginName;
                _roleService.AddRole(addRoleModel);
                return Json(AjaxResult.Success());
            }
            else
            {
                var erros = GetModelErrors();
                return Json(AjaxResult.Fail(erros));
            }
        }

        public ActionResult Edit(int roleId = 0)
        {
            var roleModel = _roleService.GetRoleById(roleId);
            return Json(AjaxResult.Success(roleModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(EditRoleModel model)
        {
            if (ModelState.IsValid)
            {
                _roleService.EditRole(model);
                return Json(AjaxResult.Success());
            }
            else
            {
                var erros = GetModelErrors();
                return Json(AjaxResult.Fail(erros));
            }
        }

        public ActionResult GetFunctionsByRoleId(int roleId)
        {
            var functions = _funService.GetFunctionListByRoleId(roleId);
            return PartialView(functions);
        }

        [HttpPost]
        public ActionResult SaveFunctionsForRole(int roleId = 0, string funcIds = null)
        {
            if (roleId == 0)
            {
                throw new UserFriendlyException("roleId 为空");
            }

            _roleService.SaveFunctionsForRole(roleId, funcIds);

            return Json(AjaxResult.Success());
        }

        [HttpPost]

        public ActionResult Delete(int roleid = 0)
        {
            _roleService.Delete(new Entity.Role { Id=roleid});
            return Json(AjaxResult.Success());
        }
    }
}