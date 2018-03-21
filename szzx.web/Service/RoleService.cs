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
    public class RoleService
    {
        private readonly RoleDal _roleDal = new RoleDal();

        public IEnumerable<RoleModel> GetRoles(DataTableAjaxConfig dtConfig)
        {
            var roles = _roleDal.GetPagedRoleList(dtConfig);

            return roles.Select(p => new RoleModel
            {
                Id = p.Id,
                RoleName = p.RoleName,
                Description = p.Description,
                AddTime = p.CreatedTime
            });
        }

        public void AddRole(AddRoleModel roleModel)
        {
            var role = new Role(roleModel.RoleName, roleModel.RoleDesc, roleModel.CreatedBy);

            _roleDal.AddRole(role);
        }

        public RoleModel GetRoleById(int id)
        {
            var role = _roleDal.GetRole(id);
            if (role == null)
                throw new UserFriendlyException("查询不到Role");

            return new RoleModel
            {
                Id = role.Id,
                Description = role.Description,
                RoleName = role.RoleName,
                AddTime = role.CreatedTime
            };
        }

        public void EditRole(EditRoleModel roleModel)
        {
            var role = _roleDal.GetRole(roleModel.Id);
            if (role == null)
                throw new UserFriendlyException("查询不到Role");

            role.RoleName = roleModel.RoleName;
            role.Description = roleModel.RoleDesc;
            role.UpdatedTime = DateTime.Now;

            _roleDal.EditRole(role);
        }

        public void SaveFunctionsForRole(int roleId, string funcIds)
        {
            var role = _roleDal.GetRole(roleId);
            if (role == null)
                throw new UserFriendlyException("查询不到Role");

            _roleDal.DeleteRoleFunctions(roleId);

            if (!string.IsNullOrWhiteSpace(funcIds))
            {
                var funcIdList = funcIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _roleDal.AddRoleFunctions(roleId, funcIdList.Select(p => int.Parse(p)));
            }
            
        }
        public bool Delete(Role role) => _roleDal.Delete(role);

        public IEnumerable<Role> GetAllRole() => _roleDal.GetAll<Role>();
    }
}
