using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using szzx.web.Entity;
using Dapper;
using Framework.MVC;
using szzx.web.Models;
using szzx.web.Common;
using szzx.web.Areas.Admin.Models;

namespace szzx.web.DataAccess
{
    public class VipDal : BaseDal
    {
        public IEnumerable<VipFee> GetPagedVipFees(DataTableAjaxConfig config, string keyword)
        {
            var whereStr = "";
            if (!string.IsNullOrEmpty(keyword))
            {
                whereStr += " and (vipName like '%' + @Keyword + '%' or orderCode like '%' + @Keyword + '%') ";
            }
            return GetPagedEntities<VipFee>($"select * from t_biz_vip_fee where status = 1 {whereStr}", config, new { Keyword = keyword}, order: "orderCode", isAsc: false);
        }

        public Vip GetVipByOpenId(string openId)
        {
            return Connection.Query<Vip>("select * from t_vip where WeChatId = @OpenId", new { OpenId = openId }).FirstOrDefault();
        }

        public Vip GetByMobile(string mobile)
        {
            return Connection.Query<Vip>("select * from t_vip where MobileNo = @MobileNo", new { MobileNo = mobile }).FirstOrDefault();
        }

        public Vip GetByCardNo(string card)
        {
            return Connection.Query<Vip>("select * from t_vip where CardNo = @CardNo", new { CardNo = card }).FirstOrDefault();
        }

        public int GetMaxVipNo()
        {
            return Connection.Query<int>("select count(1) + 1 from t_vip where createdtime > @Time", new { Time = DateTime.Today }).FirstOrDefault();
        }

        public IEnumerable<VipModel> GetPagedVips(DataTableAjaxConfig dtConfig, int status, string vipName)
        {
            var feeCondition = status == (int)WXStatus.未续费 ? "" : " and feeStatus = 1 ";
            var sql = $"select *,( SELECT count(1) FROM t_biz_team_player AS tbtp WHERE tbtp.VipId = v.Id) as IsInTeam from t_vip as v where 1=1 {feeCondition} ";
            if (status != -1)
            {
                sql += " and wxstatus = @Status ";
            }
            else
            {
                sql += " and wxstatus > 0 ";
            }
            if (!string.IsNullOrEmpty(vipName))
            {
                sql += " and vipName like CONCAT('%',@VipName,'%')  ";
            }
            return GetPagedEntities<VipModel>(sql, dtConfig, new { Status = status, VipName = vipName});
        }

        public IEnumerable<Vip> GetExpiredVips()
        {
            var sql = "select * from t_vip where expireDate is not null and  expireDate < getdate() and feeStatus = 1";
            return Connection.Query<Vip>(sql);
        }
    }

    public class MenuDal : BaseDal
    {
        public IEnumerable<Menu> GetPagedList(DataTableAjaxConfig config)
        {
            return GetPagedEntities<Menu>("select * from t_biz_menu ", config);
        }        
    }

    public class LessonDal : BaseDal
    {
        public IEnumerable<VideoModel> GetPagedVideos(DataTableAjaxConfig dtConfig, string title)
        {
            var whereStr = " where 1=1";
            if (!string.IsNullOrEmpty(title))
            {
                whereStr += $" and title like '%{title}%'";
            }
            var sql = "select v.*, c.ClassName  from t_video as v inner join t_video_class as c on v.classId = c.id" + whereStr ;

            return GetPagedEntities<VideoModel>(sql, dtConfig);
        }

        public IEnumerable<VideoCommentModel> GetPagedComments(DataTableAjaxConfig dtConfig, int isReply, string videoName)
        {
            var whereStr = "";
            var sql = @"SELECT tvc.*, tv.Title
                          FROM t_video_comment AS tvc INNER JOIN t_video AS tv ON tvc.VideoId = tv.Id
                        WHERE 1=1 ";
            if (isReply == 1) //未回复
            {
                whereStr += " and replyTime is  null";
            }
            else if(isReply == 2)
            {
                whereStr += " and replyTime is not null";
            }

            if (!string.IsNullOrEmpty(videoName))
            {
                whereStr += " and title like '%' + @Title + '%'";
            }

            return GetPagedEntities<VideoCommentModel>(sql + whereStr, dtConfig, new { Title = videoName });
        }

        public IEnumerable<VideoCommentModel> GetVipComments(int vipId)
        {
            var sql = @"SELECT tvc.*, tv.Title
                          FROM t_video_comment AS tvc INNER JOIN t_video AS tv ON tvc.VideoId = tv.Id where vipId = @VipId order by tvc.createdTime desc";
            return Connection.Query<VideoCommentModel>(sql, new { VipId = vipId });
        }

    }
}