using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Entity
{
    [Table("t_sys_role")]
    public class Role : BaseEntity
    {
        public Role() { }

        public Role(string roleName, string desc, string createdBy)
        {
            RoleName = roleName;
            Description = desc;
            CreatedBy = createdBy;
            CreatedTime = DateTime.Now;
            UpdatedBy = createdBy;
            UpdatedTime = DateTime.Now;
        }

        public string RoleName { get; set; }
        public string Description { get; set; }

        public const string SelectSql = @"select * from t_sys_role where 1=1 and isDeleted = 0 ";
        public const string DeleteSql = @"update t_sys_role set isDeleted = 1 where Id in @Id";
        public const string UpdateSqlFormat = @"update t_sys_role set {0} where id = @Id";
        public const string InsertSql = @"insert into t_sys_role(RoleName,Description,CreatedBy,CreatedTime,UpdatedBy,UpdatedTime,IsDeleted) 
                                        select @RoleName, @Description, @CreatedBy,@CreatedTime, @UpdatedBy, @UpdatedTime, @IsDeleted";
    }
}
