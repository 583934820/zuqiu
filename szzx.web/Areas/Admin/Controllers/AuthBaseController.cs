using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using szzx.web.Models;

namespace szzx.web.Areas.Admin.Controllers
{
    [AppAuthorize]
    public class AuthBaseController : Controller
    {

        public AuthBaseController()
        {
            ViewBag.CurrentUrl = System.Web.HttpContext.Current.Request.Path;
        }
        protected UserModel CurrentUser
        {
            get
            {
                var userInfo = HttpContext.Items["CurrentUser"] as UserModel;
                if (userInfo == null)
                {
                    throw new UserFriendlyException("当前用户没有登录");
                }
                return userInfo;
            }
        }

        protected string GetModelErrors()
        {
            var sb = new StringBuilder();
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors != null && state.Errors.Count > 0)
                {
                    foreach (var err in state.Errors)
                    {
                        sb.AppendFormat(",{0}", err.ErrorMessage);
                    }
                }
            }
            return sb.ToString().Substring(1);
        }


    }
}