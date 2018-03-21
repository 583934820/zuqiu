using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Entity
{
    [Table("t_biz_job_log")]
    public class JobLog
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Result { get; set; }
    }

    [Table("t_biz_job_pay_notify_log")]
    public class PayNotifyLog
    {
        public int Id { get; set; }
        public int VipId { get; set; }
        public int JobLogId { get; set; }
        public DateTime? PayStartDate { get; set; }
        public DateTime? PayEndDate { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    [Table("t_biz_mp_config")]
    public class MPConfig
    {
        public int Id { get; set; }

        public string ConfigValue { get; set; }
    }

    
}