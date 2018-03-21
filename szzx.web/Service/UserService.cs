using Framework;
using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.BusinessService
{
    public class UserService
    {
        private readonly UserDal _dal = new UserDal();
        private readonly RoleDal _roledal = new RoleDal();

        public IEnumerable<FunctionModel> GetUserFunctions(int userId)
        {
            var permissions = _dal.GetUserFunctions(userId);

            return permissions.Select(p => new FunctionModel
            {
                Id = p.Id,
                FunctionLevel = p.Level,
                FunctionName = p.PermissionName,
                IconName = p.IconName,
                ParentID = p.ParentId,
                PathUrl = p.PermissionUrl,
                FunctionSort = p.Sort
            });
        }

        public UserModel Login(string loginName, string password)
        {
            var user = _dal.GetUser(loginName);
            if (user == null || user.Password != password)
            {
                return null;
                //throw new UserFriendlyException("用户名或密码错误");
            }

            return new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                LoginName = user.LoginName
            };
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IEnumerable<User> GetPagedUserList(DataTableAjaxConfig config) => _dal.GetPagedUserList(config);


        public User GetUserById(int userId) => _dal.GetAll<User>().Where(p => p.Id == userId).FirstOrDefault();

        public void Edit(User model)
        {
            var user = _dal.GetAll<User>().Where(p => p.Id == model.Id).FirstOrDefault();
            if (user == null || user.Id < 0)
                throw new UserFriendlyException("查询不到User");
            user.LoginName = model.LoginName;
            user.Password = model.Password;
            user.UserName = model.UserName;
            user.UpdatedTime = DateTime.Now;
            user.UpdatedBy = model.UpdatedBy;
            _dal.Update(user);
        }

        public void Add(User model) => _dal.Insert(model);

        public void Delete(User model) => _dal.Delete(model);



        public IEnumerable<Role> GetUserRole(int userId)
        {

            var userRoleList = _dal.GetUserRole(userId);


            return userRoleList;
        }
        public void SaveUserRole(IEnumerable<UserRole> model, int userId)
        {
            _roledal.DeleteUserRole(userId);
            if (model!=null&&model.Count() > 0)
            {
                _dal.Insert(model);
            }
           
        }
    }
}
