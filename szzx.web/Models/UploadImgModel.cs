using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Models
{
    public class UploadImgModel
    {
        public string ImgId { get; set; }
        public string FormId { get; set; }
        public string CallbackFun { get; set; }

    }

    public class UploadFileModel
    {
        public string FormId { get; set; }
        public string FileTextId { get; set; }
        //public string InputFileId { get; set; }
    }

    public class UploadVideoModel
    {
        public string FormId { get; set; }
        public string VideoTextId { get; set; }
        //public string InputFileId { get; set; }
    }

    public class PagedModel
    {
        public int PageCount { get; set; }

        public int PageSize { get; set; } = 10;

        public int PageIndex { get; set; } = 1;
        public int TotalCount { get; set; }
    }
}
