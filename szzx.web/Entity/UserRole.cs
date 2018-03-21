using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Entity
{
    [Table("t_sys_user_role")]
    public class UserRole
    {

        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
