using Framework.Json;
using Framework.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.BusinessService;
using szzx.web.Models;

namespace szzx.web.Areas.Admin.Controllers
{
    public class AccountController : AuthBaseController
    {
        private UserService _userService;

        public AccountController()
        {
            _userService = new UserService();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = GetModelErrors();
                return View();
            }

            var userModel = _userService.Login(loginModel.UserName, loginModel.Password);
            if (userModel == null)
            {
                ViewBag.Error = "用户名或密码错误";
                return View();
            }

            SetAuthCookie(userModel);

            return RedirectToAction("Index", "Default");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            var cookie = HttpContext.Request.Cookies["authData"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.SetCookie(cookie);
            }
            return RedirectToAction("Login");
        }

        private void SetAuthCookie(UserModel user)
        {
            var userInfo = user.ToJson();
            //var cookie = HttpContext.Request.Cookies["autoData"];
            //if (cookie == null)
            //{
            //    cookie = new HttpCookie("authData");
            //    cookie.Value = Utility.DESEncrypt(userInfo, ConfigurationManager.AppSettings["authKey"]);
            //    cookie.Expires = DateTime.Now.AddDays(10);
            //    cookie.Path = "/";
            //    HttpContext.Response.Cookies.Add(cookie);
            //}
            //else
            //{
            //    cookie.Value = Utility.DESEncrypt(userInfo, ConfigurationManager.AppSettings["authKey"]);
            //    cookie.Expires = DateTime.Now.AddDays(10);
            //    HttpContext.Response.SetCookie(cookie);
            //}

            var cookie = new HttpCookie("authData");
            cookie.Value = EncryptHelper.DESEncrypt(userInfo, ConfigurationManager.AppSettings["authKey"]);
            cookie.Expires = DateTime.Now.AddDays(1);
            cookie.Path = "/";
            cookie.HttpOnly = true;
            HttpContext.Response.Cookies.Add(cookie);

        }
    }
}