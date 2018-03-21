using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szzx.web.Models
{
    public class TeamPlayerModel
    {
        public int Id { get; set; }
        public int VipId { get; set; }
        public int TeamId { get; set; }
        public string PlayerName { get; set; }
        public DateTime JoinTime { get; set; }
        public string ImgPath { get; set; }
        public string Address { get; set; }
        public DateTime CreatedTime { get; set; }
        public string MobileNo { get; set; }
        public string WellPosition { get; set; }
        public string PlayerNo { get; set; }
        public string TeamName { get; set; }
        public string VipNo { get; set; }
        public int Age { get; set; }
        public string CardNo { get; set; }
    }
}