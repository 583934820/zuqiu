using Framework.MVC;
using Newtonsoft.Json;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Areas.Admin.Controllers
{
    public class WeiXinManageController : AuthBaseController
    {
        private MenuDal _menuDal = new MenuDal();
        
        public ActionResult Menus()
        {            
            return View();
        }

        public ActionResult MenusList()
        {
            var token = AccessTokenContainer.TryGetAccessToken(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret);
            var menus = CommonApi.GetMenu(AppConfig.Instance.AppId);

            return Json(menus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGet(DataTableAjaxConfig dtConfig)
        {
            var miles = _menuDal.GetPagedList(dtConfig);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = miles
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetParentMenus(int level)
        {
            var menus = _menuDal.GetAll<Menu>().Where(p => p.MenuLevel == level - 1);

            return PartialView(menus);
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            var mile = _menuDal.Get<Menu>(id);

            return Json(AjaxResult.Success(mile), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Menu model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    //创建微信菜单
                    var result = CreateWXMenu(model);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return Json(AjaxResult.Fail(result));
                    }

                    if (model.MenuParentId > 0)
                    {
                        var pMenu = _menuDal.Get<Menu>(model.MenuParentId);
                        model.MenuParentName = pMenu?.MenuName;
                    }
                    model.CreatedBy = CurrentUser.UserName;
                    model.UpdatedBy = CurrentUser.UserName;

                    _menuDal.Insert(model);
                }
                else
                {
                    //创建微信菜单
                    var result = CreateWXMenu(model);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return Json(AjaxResult.Fail(result));
                    }

                    var entity = _menuDal.Get<Menu>(model.Id);
                    if (entity != null)
                    {
                        entity.MenuLevel = model.MenuLevel;
                        entity.MenuName = model.MenuName;
                        entity.MenuParentId = model.MenuParentId;
                        if (model.MenuParentId > 0)
                        {
                            var pMenu = _menuDal.Get<Menu>(model.MenuParentId);
                            entity.MenuParentName = pMenu?.MenuName;
                        }
                        entity.Url = model.Url;
                        entity.UpdatedTime = DateTime.Now;

                        _menuDal.Update(entity);
                    }
                }
                return Json(AjaxResult.Success());
            }
            else
            {
                var erros = GetModelErrors();
                return Json(AjaxResult.Fail(erros));
            }
        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            var menus = _menuDal.GetAll<Menu>();
            if (menus.FirstOrDefault(p => p.MenuParentId == id) != null)
            {
                return Json(AjaxResult.Fail("请先删除子菜单"));
            }

            var m = menus.FirstOrDefault(p => p.Id == id);
            if (m != null)
            {
                var result = DeleteWXMenu(m);
                if (!string.IsNullOrEmpty(result))
                {
                    return Json(AjaxResult.Fail(result));
                }

                _menuDal.Delete(new Menu { Id = id });
            }
            

            return Json(AjaxResult.Success());
        }

        #region 微信菜单

        private string DeleteWXMenu(Menu model)
        {
            try
            {
                var token = AccessTokenContainer.TryGetAccessToken(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret);
                if (!string.IsNullOrEmpty(token))
                {
                    var bg = new ButtonGroup();

                    var menus = _menuDal.GetAll<Menu>().ToList();
                    var _menu = menus.FirstOrDefault(p => p.Id == model.Id);
                    if (_menu != null)
                    {
                        menus.Remove(_menu);
                    }

                    if (menus.Count() == 0)
                    {
                        var r =  CommonApi.DeleteMenu(token);
                        return r.errcode == Senparc.Weixin.ReturnCode.请求成功 ? "" : r.errmsg;
                    }                    

                    foreach (var m in menus.Where(p => p.MenuLevel == 1))
                    {
                        if (menus.FirstOrDefault(p => p.MenuParentId == m.Id) != null)
                        {
                            var subBtn = new SubButton
                            {
                                name = m.MenuName
                            };

                            menus.Where(p => p.MenuParentId == m.Id).ToList().ForEach(p =>
                            {
                                subBtn.sub_button.Add(new SingleViewButton
                                {
                                    name = p.MenuName,
                                    url = p.Url
                                });
                            });

                            bg.button.Add(subBtn);

                        }
                        else
                        {
                            bg.button.Add(new SingleViewButton
                            {
                                name = m.MenuName,
                                url = m.Url
                            });
                        }
                    }

                    var result = CommonApi.CreateMenu(token, bg);

                    return result.errcode == Senparc.Weixin.ReturnCode.请求成功 ? "" : result.errmsg;
                }
                else
                {
                    return "获取微信token失败";
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private string CreateWXMenu(Menu model)
        {
            try
            {
                var token = AccessTokenContainer.TryGetAccessToken(AppConfig.Instance.AppId, AppConfig.Instance.AppSecret);
                if (!string.IsNullOrEmpty(token))
                {
                    var bg = new ButtonGroup();

                    var menus = _menuDal.GetAll<Menu>().ToList();                    
                    if (model.Id == 0) //新增
                    {
                        menus.Add(model);

                    }
                    else
                    {
                        var _model = menus.FirstOrDefault(p => p.Id == model.Id);
                        if (_model != null)
                        {
                            _model.MenuLevel = model.MenuLevel;
                            _model.MenuName = model.MenuName;
                            _model.MenuParentId = model.MenuParentId;
                            _model.Url = model.Url;
                        }

                    }

                    foreach (var m in menus.Where(p => p.MenuLevel == 1))
                    {
                        if (menus.FirstOrDefault(p => p.MenuParentId == m.Id && 0 != m.Id) != null)
                        {
                            var subBtn = new SubButton
                            {
                                name = m.MenuName
                            };

                            menus.Where(p => p.MenuParentId == m.Id && 0 != m.Id).ToList().ForEach(p =>
                            {
                                subBtn.sub_button.Add(new SingleViewButton
                                {
                                    name = p.MenuName,
                                    url = p.Url
                                });
                            });

                            bg.button.Add(subBtn);

                        }
                        else
                        {
                            bg.button.Add(new SingleViewButton
                            {
                                name = m.MenuName,
                                url = m.Url
                            });
                        }
                    }                   

                    var result = CommonApi.CreateMenu(token, bg);

                    return result.errcode == Senparc.Weixin.ReturnCode.请求成功 ? "" : result.errmsg;
                }
                else
                {
                    return "获取微信token失败";
                }

            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

        #endregion

        #region 公众号配置
        public ActionResult MpConfig()
        {
            var config = _menuDal.GetAll<MPConfig>().FirstOrDefault();
            ViewBag.Id = config == null ? 0 : config.Id;          
            return View(config == null ? new MpConfigModel() : 
                JsonConvert.DeserializeObject<MpConfigModel>(config.ConfigValue, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }));
        }

        [HttpPost]
        public ActionResult MpConfig(MpConfigModel config, int id = 0)
        {
            //if (config.PayDateRange != null && config.PayDateRange.StartDate != null && config.PayDateRange.EndDate != null)
            //{
            //    if (config.PayDateRange.StartDate > config.PayDateRange.EndDate)
            //    {
            //        return Json(AjaxResult.Fail("开始缴费日期不能大于结束日期"));
            //    }
            //}

            if (config.RejoinTeamStartDate != null && config.RejoinTeamEndDate != null)
            {
                if (config.RejoinTeamStartDate > config.RejoinTeamEndDate)
                {
                    return Json(AjaxResult.Fail("重新加入球队开始日期不能大于结束日期"));
                }
            }

            if (id == 0)
            {
                _menuDal.Insert(new MPConfig { ConfigValue = JsonConvert.SerializeObject(config) });
            }
            else
            {
                var entity = _menuDal.Get<MPConfig>(id);
                if (entity != null)
                {
                    entity.ConfigValue = Newtonsoft.Json.JsonConvert.SerializeObject(config);
                    _menuDal.Update(entity);
                }
            }

            return Json(AjaxResult.Success());
        }

        #endregion
    }
}