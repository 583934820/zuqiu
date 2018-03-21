using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Entity
{
    [Table("t_sys_user")]
    public class User : BaseEntity
    {
        public string LoginName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public const string SelectSql = @"select * from t_sys_user where 1=1 and isDeleted = 0 ";
        public const string DeleteSql = @"update t_sys_user set isdeleted = 1 where id = @Id";
        public const string UpdateSqlFormat = @"update t_sys_user set {0} where id = @Id";
    }
}
