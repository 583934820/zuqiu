using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Entity
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public const string SelectSql = @"select * from t_sys_role_permission where 1=1 ";
    }
}
