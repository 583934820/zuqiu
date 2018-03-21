using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Models
{
    public class MpConfigModel
    {
        public DateTime? RejoinTeamStartDate { get; set; } = null;
        public DateTime? RejoinTeamEndDate { get; set; } = null;
        public decimal VipFee { get; set; }
        public decimal TeamFee { get; set; }
        //public MPConfig_PayDateRangeModel PayDateRange { get; set; }

        public string GetJoinTeamDaterange()
        {
            return $"{this.RejoinTeamStartDate?.ToString("yyyy-MM-dd")} - {this.RejoinTeamEndDate?.ToString("yyyy-MM-dd")}";
        }
    }

    public class MPConfig_PayDateRangeModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}