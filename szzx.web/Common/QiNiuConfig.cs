using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Common
{
    public static class QiNiuConfig
    {
        public static string AccessKey { get; set; }
        public  static string SecretKey { get; set; }
        public static  string Bucket { get; set; }
        public static string Domain { get; set; }

        public static string GetToken()
        {
            var mac = new Mac(QiNiuConfig.AccessKey, QiNiuConfig.SecretKey);
            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = QiNiuConfig.Bucket;
            string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            return token;
        }
    }
}