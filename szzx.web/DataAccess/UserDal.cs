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
    public class UserDal : BaseDal
    {
        public IEnumerable<Permission> GetUserFunctions(int userId)
        {
            var sql = @"select * from t_sys_permission
                        where id in (
                        	select PermissionId from t_sys_role_permission rp inner join t_sys_user_role as ur on ur.RoleId = rp.RoleId
                        	where ur.UserId = @UserId
                        )";
            return Connection.Query<Permission>(sql, new { UserId = userId });
        }

        public User GetUser(string loginName)
        {
            var sql = User.SelectSql + " and loginName = @LoginName";
            return Connection.Query<User>(sql, new { LoginName = loginName }).FirstOrDefault();
        }

        public User GetUser(int userId)
        {
            var sql = User.SelectSql + " and id = @Id";
            return Connection.Query<User>(sql, new { Id = userId }).FirstOrDefault();
        }



        public IEnumerable<User> GetPagedUserList(DataTableAjaxConfig config)
        {
            config.recordCount = GetTotal();
            var sql = @"with t as(
                        	select top (@start + @length) *, ROW_NUMBER() over(order by id) as num
                        	from t_sys_user where isDeleted = 0 and loginName <> 'admin' order by id asc
                        )
                        select Id,UserName,LoginName,Password,CreatedBy,CreatedTime, UpdatedBy,UpdatedTime, IsDeleted 
                        from t where t.num  > @start";
            return Connection.Query<User>(sql, config);
        }

        private int GetTotal()
        {
            var sql = "select count(1) from t_sys_user where isDeleted = 0";
            return Connection.ExecuteScalar<int>(sql);
        }


        public IEnumerable<Role> GetUserRole(int userId)
        {
            var sql = "select * from [dbo].[t_sys_role] where id in (select RoleId from [dbo].[t_sys_user_role] where UserId =@UserId)";
            return Connection.Query<Role>(sql, new { UserId = userId });
        }
    }
}