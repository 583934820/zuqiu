using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using szzx.web.Entity;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Framework.MVC;

namespace szzx.web.DataAccess
{
    public class PermissionDal : BaseDal
    {
        public IEnumerable<RolePermission> GetPermissionsByRole(int roleId)
        {
            var sql = RolePermission.SelectSql + " and roleId = @RoleId";

            return Connection.Query<RolePermission>(sql, new { RoleId = roleId });
        }

        public IEnumerable<Permission> GetAllPermissions()
        {
            return Connection.Query<Permission>(Permission.SelectSql);
        }

        public IEnumerable<Permission> GetPagePermissionsList(DataTableAjaxConfig config)
        {
            config.recordCount = GetTotal();
            var sql = @"with t as(select top (@start + @length) *, ROW_NUMBER() over(order by id) as num
                        	from [dbo].[t_sys_permission] where isDeleted = 0 order by id asc  )
                        select Id,PermissionName,PermissionUrl,Level,Sort,isnull( (select PermissionName from   [dbo].[t_sys_permission] where id = t.ParentId) ,'') as  ParentName  , CreatedBy, CreatedTime, UpdatedBy,UpdatedTime, IsDeleted 
                        from t where t.num  > @start";
            return Connection.Query<Permission>(sql, config);
        }

        private int GetTotal()
        {
            var sql = "select count(1) from [dbo].[t_sys_permission] where isDeleted = 0";
            return Connection.ExecuteScalar<int>(sql);
        }




        public PermissionModel GetFunction(int id)
        {
            return Connection.Query<PermissionModel>(Permission.SelectSql + " and id = @Id", new { Id = id }).FirstOrDefault();
        }

        public IEnumerable<Permission> GetFunctionByLevel(int level)
        {
            return Connection.Query<Permission>(Permission.SelectSql + " and Level = @Level", new { Level = level });
        }

    }
}