using Framework.Json;
using Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szzx.web.Common;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Areas.Admin.Controllers
{
    public class VipController : AuthBaseController
    {
        VipDal _dal = new VipDal();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AjaxGet(DataTableAjaxConfig dtConfig, int status = -1, string vipName = "")
        {
            var miles = _dal.GetPagedVips(dtConfig, status, vipName);
            return Json(new DataTableAjaxResult
            {
                draw = dtConfig.draw,
                recordsFiltered = dtConfig.recordCount,
                recordsTotal = dtConfig.recordCount,
                data = miles
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            var mile = _dal.Get<Vip>(id);

            return Json(AjaxResult.Success(mile), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Vip model, int isRefund = 0)
        {
            if (ModelState.IsValid)
            {
                var entity = _dal.Get<Vip>(model.Id);
                if (entity != null)
                {
                    //var auditResult = "";
                    //entity.WXStatus = model.WXStatus;
                    //entity.RemoveReason = model.RemoveReason;
                    //if (entity.WXStatus == (int)WXStatus.已注册) //移除
                    //{                        
                    //    //entity.FeeStatus = (int)PayStatus.待支付;                                          

                    //    _teamDal.Insert(new ActionHis
                    //    {
                    //        VipId = entity.Id,
                    //        ActionType = ActionTypeEnum.移除球员.ToString(),
                    //        CreatedBy = CurrentUser.LoginName,
                    //        CreatedTime = DateTime.Now,
                    //        Remark = VipTeamRemoveHisModel.Init(entity.Id).ToJson()
                    //    });

                    //    var team = _teamDal.GetByTeamAdminId(entity.Id); //是队长则删除球队，删除球员
                    //    if (team != null)
                    //    {
                    //        _teamDal.DeleteTeamByAdminId(entity.Id);
                    //        _teamDal.DeleteTeamPlayerByTeamId(team.Id);
                    //    }
                    //    else
                    //    {
                    //        _teamDal.ExitTeam(entity.Id);
                    //    }

                    //    _teamDal.DeleteTeamPlayerActHis(entity.Id);
                    //    _teamDal.Insert(new TeamPlayerActionHis
                    //    {
                    //        VipId = entity.Id,
                    //        TeamId = 0,
                    //        ActionType = (int)TeamPlayerActionType.移除,
                    //        CreatedTime = DateTime.Now
                    //    });


                    //    //TODO 发送通知
                    //    auditResult = $"您已被移除苏州足协，原因：{entity.RemoveReason}，如有疑问，请联系客服咨询";

                    //    //退款
                    //    if (isRefund != 0)
                    //    {
                    //        DoRefund(entity);

                    //        //退还建队费用
                    //        DoRefundTeam(entity);
                    //    }
                    //    entity.RemoveReason = "";
                    //}
                    //else if (entity.WXStatus == (int)WXStatus.审核失败)
                    //{
                    //    //TODO 发送通知，退款？
                    //    auditResult = $"审核失败，原因：{entity.RemoveReason}，如有疑问，请联系客服咨询";

                    //    DoRefund(entity);
                        
                    //}
                    //else if (entity.WXStatus == (int)WXStatus.审核成功)
                    //{
                    //    auditResult = "审核成功";
                    //    entity.RemoveReason = "";
                    //}
                    //entity.UpdatedTime = DateTime.Now;

                    //_dal.Update(entity);

                    //if (!string.IsNullOrEmpty(auditResult))
                    //{
                    //    TemplateMsgMethod.SendWxStatusNotify(AppConfig.Instance.AppId, entity.WeChatId, auditResult, DateTime.Now, entity.VipName, entity.VipNo);
                    //}

                }
                return Json(AjaxResult.Success());
            }
            else
            {
                var erros = GetModelErrors();
                return Json(AjaxResult.Fail(erros));
            }
        }


        private void DoRefund(Vip entity)
        {
            var vipFee = _dal.GetAll<VipFee>().Where(p => p.VipId == entity.Id && p.Status == (int)PayStatus.支付成功).OrderByDescending(p=>p.FeeTime).FirstOrDefault();
            if (vipFee != null && entity.FeeStatus == (int)PayStatus.支付成功)
            {
                var refundReqData = RefundRequestData.GetRefundRequestData((int)vipFee.Fee, vipFee.OrderCode, Server.MapPath("~/App_Data/apiclient_cert.p12"));
                _dal.Insert<VipFeeRefund>(new VipFeeRefund
                {
                    RtnOrderCode = refundReqData.RtnOrderCode,
                    OrderCode = vipFee.OrderCode,
                    VipId = vipFee.VipId,
                    VipName = entity.VipName,
                    RefundFee = vipFee.Fee,
                    WXRtnOrderCode = refundReqData.WxRtnOrderCode,
                    Remark = refundReqData.Remark
                });

                if (refundReqData.RefundResult.ToLower() == "success")
                {
                    //entity.IsReturnFee = true;
                    entity.FeeStatus = (int)PayStatus.待支付;
                }
            }
        }

    }
}