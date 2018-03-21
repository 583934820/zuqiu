using Framework;
using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.BusinessService
{
    public class FunctionService
    {
        private readonly PermissionDal _funcDal = new PermissionDal();


        /// <summary>
        /// 根据角色Id获取功能列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IList<FunctionModel> GetFunctionListByRoleId(int roleId)
        {
            var funcs = _funcDal.GetAllPermissions();
            var rolePermission = _funcDal.GetPermissionsByRole(roleId);

            return funcs.Select(p => new FunctionModel
            {
                Id = p.Id,
                FunctionLevel = p.Level,
                FunctionName = p.PermissionName,
                FunctionSort = p.Sort,
                IconName = p.IconName,
                ParentID = p.ParentId,
                PathUrl = p.PermissionUrl,
                HasRole = rolePermission.Any(rp => rp.PermissionId == p.Id)
            }).ToList();
        }

        /// <summary>
        /// 分页获取功能列表
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IEnumerable<FunctionModel> GetPageFunctionList(DataTableAjaxConfig config)
        {
            var functions = _funcDal.GetPagePermissionsList(config);
            return functions.Select(p => new FunctionModel
            {
                Id = p.Id,
                FunctionName = p.PermissionName,
                FunctionLevel = p.Level,
                FunctionSort = p.Sort,
                PathUrl = p.PermissionUrl,
                ParentName = p.ParentName,
            });
        }
        /// <summary>
        /// 编辑功能
        /// </summary>
        /// <param name="functionModel"></param>
        public void EditFunction(FunctionModel functionModel)
        {
            var function = _funcDal.GetFunction(functionModel.Id);
            if (function == null)
                throw new UserFriendlyException("查询不到function");
            function.PermissionName = functionModel.FunctionName;
            function.Level = functionModel.FunctionLevel;
            function.Sort = functionModel.FunctionSort;
            function.PermissionUrl = functionModel.PathUrl;
            function.ParentId = functionModel.ParentID;
            function.UpdatedTime = DateTime.Now;
            _funcDal.Update(function);
        }

        /// <summary>
        /// 添加功能
        /// </summary>
        /// <param name="model"></param>
        public void AddFunction(PermissionModel model)
        {
            _funcDal.Insert(model);
        }
        /// <summary>
        /// 根据Id获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FunctionModel GetFunctionById(int id)
        {
            var function = _funcDal.GetFunction(id);
            if (function == null)
                throw new UserFriendlyException("查询不到function");

            return new FunctionModel
            {
                Id = function.Id,
                FunctionName = function.PermissionName,
                FunctionLevel = function.Level,
                ParentID = function.ParentId,
                FunctionSort = function.Sort,
                PathUrl = function.PermissionUrl
                
            };
        }

        /// <summary>
        /// 根据等级获取父级目录
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IList<FunctionModel> GetFunctionByLevel(int level)
        {
            var function = _funcDal.GetFunctionByLevel(level);
            return function.Select(p => new FunctionModel
            {
                FunctionName = p.PermissionName,
                Id = p.Id
            }).ToList();
        }

        /// <summary>
        /// 删除功能
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Delete(PermissionModel model) => _funcDal.Delete(model);



        public int GetChildFunction(int parentId)=> _funcDal.GetAll<PermissionModel>().Where(p => p.ParentId == parentId).Count();
   
    }
}
