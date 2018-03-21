using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using szzx.web.DataAccess;
using szzx.web.Entity;
using szzx.web.Models;

namespace szzx.web.Common
{
    public class AutoPayNotifyJob : IJob
    {              

        public void Run()
        {
            Task.Factory.StartNew(DoRun);
        }

        private void DoRun()
        {
            while (true)
            {
                try
                {
                    VipDal _dal = new VipDal();

                    var jobLog = new JobLog
                    {
                        JobName = "续费通知",
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        Result = ""
                    };

                    var vips = _dal.GetExpiredVips();

                    foreach (var v in vips)
                    {
                        v.FeeStatus = (int)PayStatus.待支付;

                    }

                    _dal.Update(vips);

                    jobLog.EndTime = DateTime.Now;
                    jobLog.Result = "成功";
                    _dal.Insert(jobLog);
                }
                catch (Exception ex)
                {
                    Senparc.Weixin.WeixinTrace.SendCustomLog("AutoPayNotifyJob", ex.Message);

                }


                Thread.Sleep(1000 * 60 * 10);
            }
        }
    }
}