using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Areas.Admin.Models
{
    public class VideoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImgPath { get; set; }
        public string VideoUrl { get; set; }
        public string ClassName { get; set; }
    }
}