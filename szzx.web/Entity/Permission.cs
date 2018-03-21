using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Entity
{

    public class Permission:BaseEntity
    {
        public string PermissionName { get; set; }
        public int ParentId { get; set; }
        public string PermissionUrl { get; set; }
        public int Level { get; set; }
        public string IconName { get; set; }
        public int Sort { get; set; }
        public string ParentName { get; set; }

        public const string SelectSql = @"select * from t_sys_permission where 1=1 and isDeleted = 0 ";
        public const string DeleteSql = @"update t_sys_permission set isdeleted = 1 where id = @Id";
        public const string UpdateSqlFormat = @"update t_sys_permission set {0} where id = @Id";
    }





    [Table("t_sys_permission")]
    public class PermissionModel : BaseEntity
    {
        public string PermissionName { get; set; }
        public int ParentId { get; set; }
        public string PermissionUrl { get; set; }
        public int Level { get; set; }
        public string IconName { get; set; }
        public int Sort { get; set; }


    }
}
