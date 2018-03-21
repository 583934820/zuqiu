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
    public class RoleDal : BaseDal
    {
        public IEnumerable<Role> GetPagedRoleList(DataTableAjaxConfig config)
        {
            config.recordCount = GetTotal();
            var sql = @"with t as(
                        	select top (@start + @length) *, ROW_NUMBER() over(order by id) as num
                        	from t_sys_role where isDeleted = 0 order by id asc
                        )
                        select Id,RoleName,Description,CreatedBy, CreatedTime, UpdatedBy,UpdatedTime, IsDeleted 
                        from t where t.num  > @start";
            return Connection.Query<Role>(sql, config);
        }

        private int GetTotal()
        {
            var sql = "select count(1) from t_sys_role where isDeleted = 0";
            return Connection.ExecuteScalar<int>(sql);
        }

        public void AddRole(Role role)
        {
            Connection.Execute(Role.InsertSql, role);
        }

        public void EditRole(Role role)
        {
            var sql = string.Format(Role.UpdateSqlFormat, "RoleName = @RoleName, Description = @Description, UpdatedTime = @UpdatedTime");
            Connection.Execute(sql, role);
        }

        public void DeleteRole(IEnumerable<int> ids)
        {
            Connection.Execute(Role.DeleteSql, new { Id = ids });
        }

        public Role GetRole(int id)
        {
            return Connection.Query<Role>(Role.SelectSql + " and id = @Id", new { Id = id }).FirstOrDefault();
        }

        public void AddRoleFunctions(int roleId, IEnumerable<int> funcIds)
        {
            var sql = "insert into t_sys_role_permission(roleId,permissionId) values(@RoleId, @FuncId)";
            Connection.Execute(sql, funcIds.Select(p => new { RoleId = roleId, FuncId = p }));
        }

        public void DeleteRoleFunctions(int roleId)
        {
            var sql = "delete from t_sys_role_permission where roleId = @Id";
            Connection.Execute(sql, new { Id = roleId });
        }
        public void DeleteUserRole(int userId)
        {
            var sql = "delete from t_sys_user_role where UserId = @userId";
            Connection.Execute(sql, new { userId = userId });
        }
    }
}