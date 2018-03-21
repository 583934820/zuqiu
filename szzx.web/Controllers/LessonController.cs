using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Common;
using szzx.web.Controllers;
using szzx.web.DataAccess;
using szzx.web.Entity;

namespace TalService.Web.Controllers
{
    public class LessonController : BaseController
    {
        LessonDal dal = new LessonDal();

        // GET: Lesson
        public ActionResult Categories()
        {
            ViewBag.Title = "课程分类";

            var categories = dal.GetAll<VideoClass>().ToList();
            return View(categories);
        }

        public ActionResult List(int catId = 0)
        {
            ViewBag.Title = "课程列表";

            var videos = dal.GetAll<Video>().Where(p => p.ClassId == catId).ToList();

            return View(videos);
        }

        public ActionResult Detail(int id = 0)
        {
            ViewBag.Title = "课程详情";

            var vip = GetVipInfo();
            if (vip.FeeStatus != 1)
            {
                return Content($"<script>alert('没有支付无法观看课程');window.location.href='{Url.Action("Categories")}'</script>");
            }

            var video = dal.Get<Video>(id);
            if (video == null)
            {
                return RedirectToAction("Categories");
            }

            var vipId = 1;
            var qrImg = $"~/upload/qrcode/qr{vipId.ToString()}.png";
            var qrImgPath = Server.MapPath(qrImg);
            if (!System.IO.File.Exists(qrImgPath))
            {
                QRCodeHelper.GenerateQRCode($"userid={vipId.ToString()}", qrImgPath);
            }
            ViewBag.QrCodePath = qrImg;

            ViewBag.VideoComments = dal.GetAll<VideoComment>().Where(p=>p.VideoId == id).OrderByDescending(p=>p.CreatedTime).ToList();

            return View(video);
        }

        [HttpPost]
        public ActionResult SendVideoComment(int VideoId = 0, string Comment = "")
        {

            var comment = new VideoComment { VideoId = VideoId, Comment = Comment };

            if (comment.VideoId <= 0)
            {
                return Json(AjaxResult.Fail("请求失败"));
            }

            if (string.IsNullOrEmpty(comment.Comment))
            {
                return Json(AjaxResult.Fail("请填写评论"));

            }

            var vip = GetVipInfo();
            comment.CreatedBy = vip.VipName;
            comment.CreatedTime = DateTime.Now;
            comment.UpdatedBy = vip.VipName;
            comment.UpdatedTime = DateTime.Now;
            comment.VipId = vip.Id;
            comment.VipImgPath = vip.ImgPath;
            comment.VipName = vip.VipName;

            dal.Insert(comment);

            return Json(AjaxResult.Success());         
        }
    }
}