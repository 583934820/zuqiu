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
            var miles = _dal.GetPagedVips(dtConfig, vipName);
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
        public ActionResult AddOrEdit(Vip model)
        {
            if (ModelState.IsValid)
            {
                var entity = _dal.Get<Vip>(model.Id);
                if (entity != null)
                {
                    entity.Password = model.Password;
                    //entity.CardImg = model.CardImg;
                    entity.CertNo = model.CertNo;
                    entity.HasCert = model.HasCert;
                    _dal.Update(entity);

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