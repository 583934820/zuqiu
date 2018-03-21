using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.BusinessService;
using szzx.web.Entity;

namespace szzx.web.Areas.Admin.Controllers
{
    public class UserController : AuthBaseController
    {

        private UserService _userService;
        private RoleService _roleService;
        public UserController()
        {
            _userService = new UserService();
            _roleService = new RoleService();
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxGetUsers(DataTableAjaxConfig dtConfig)
        {
            var users = _userService.GetPagedUserList(dtConfig);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = users
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(int userId = 0)
        {
            var userModel = _userService.GetUserById(userId);
            return Json(AjaxResult.Success(userModel), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(User model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatedBy = CurrentUser.LoginName;
                _userService.Edit(model);
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
        public ActionResult Add(User model)
        {
            model.CreatedBy = CurrentUser.LoginName;
            model.UpdatedBy = CurrentUser.LoginName;
            _userService.Add(model);
            return Json(AjaxResult.Success());
        }

        [HttpPost]

        public ActionResult Delete(int userId = 0)
        {
            _userService.Delete(new User { Id = userId });
            return Json(AjaxResult.Success());
        }



        public ActionResult GetUserRole(int userId = 0)
        {
            var userRole = _userService.GetUserRole(userId).ToList();
            var roleList = _roleService.GetAllRole().ToList();
            userRole.ForEach(p =>
            {
                roleList.RemoveAll(e => e.Id == p.Id);
            });
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("left", roleList);
            dic.Add("right", userRole);
            return Json(AjaxResult.Success(dic), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveUserRole(IEnumerable<UserRole> model, int userId = 0)
        {
            _userService.SaveUserRole(model, userId);
            return Json(AjaxResult.Success());
        }
    }
}