using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Models
{

    public class FunctionModel
    {
        public int Id { get; set; }
        public string FunctionName { get; set; }

        public string PathUrl { get; set; }
        public int ParentID { get; set; }
        public string IconName { get; set; }
        public int FunctionLevel { get; set; }
        public int FunctionSort { get; set; }

        public bool HasRole { get; set; }

        public string ParentName { get; set; }
    }

}
