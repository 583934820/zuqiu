using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Models;

namespace szzx.web.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult UploadImg(UploadImgModel model)
        {
            var file = HttpContext.Request.Files[0];
            if (file != null)
            {
                var path = Server.MapPath("~/upload/img");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var fileName = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(file.FileName));
                if (!fileName.ToLower().EndsWith(".jpg") && !fileName.ToLower().EndsWith(".jpeg"))
                {
                    return Content("<script>window.parent.alert('请上传jpg格式的图片')</script>");
                }
                file.SaveAs(fileName);
                var _path = Url.Content($"~/upload/img/{Path.GetFileName(fileName)}");

                if (string.IsNullOrEmpty(model.CallbackFun))
                {
                    return Content($"<script>window.parent.$('#{model.FormId}').find('input[type=file]').val('');window.parent.$('#{model.ImgId}')[0].src = '{_path}';window.parent.$('#{model.ImgId}').show();</script>");

                }
                else
                {
                    return Content($"<script>window.parent.{model.CallbackFun}('{model.FormId}','{model.ImgId}','{_path}');</script>");
                }
            }
            else
            {
                return Content("<script>window.parent.alert('请选择上传图片')</script>");
            }
        }

        [HttpPost]
        public ActionResult UploadImg2(UploadImgModel model)
        {
            var file = HttpContext.Request.Files[0];
            if (file != null)
            {
                var path = Server.MapPath("~/upload/img");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var fileName = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName);
                if (!fileName.ToLower().EndsWith(".jpg") && !fileName.ToLower().EndsWith(".jpeg"))
                {
                    return Content("<script>window.parent.alert('请上传jpg格式的图片')</script>");
                }
                file.SaveAs(fileName);
                var _path = $"/upload/img/{Path.GetFileName(fileName)}";

                return Content($"<script>window.parent.$('#{model.FormId}').find('input[type=file]').val('');window.parent.$('#{model.ImgId}')[0].src = '{_path}';window.parent.$('#{model.ImgId}')[0].style.display='block'</script>");
            }
            else
            {
                return Content("<script>window.parent.alert('请选择上传图片')</script>");
            }
        }
    }
}