using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }
    }

    public class UserCenterModel
    {
        public string Mobile { get; set; }
        public string VipName { get; set; }
        public string Team { get; set; }
    }

}