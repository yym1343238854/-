using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business {
    public partial class News : Base {
        /// <summary>
        /// 家长通知
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public bool ListByParent(Guid teamId, int pageIndex, int pageSize, out List<News> news) {
            news = null;
            try {
                news = Db.News.Where(x => x.Status == (byte)EnumNewsStatus.发布 &&
                    x.PeopleType != (byte)EnumNewsPeopleType.老师 &&
                    (x.RangeType == (byte)EnumNewsRangeType.全公开 ||
                        (x.RangeType == (byte)EnumNewsRangeType.部分班级 &&
                        x.TeamNews.Any(y => y.TeamId == teamId))))
                    .OrderByDescending(x => x.PublishTime).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                    .ToList();
                return true;
            } catch (Exception ex) {
                SetError(ex.Message);
                return false;
            }
        }

        public bool Load(Guid newsId, out News news) {
            news = null;
            try {
                var q = Db.News.Where(x => x.NewsId == newsId);
                if (q.Any()) {
                    news = q.SingleOrDefault();
                    news.Hits = news.Hits.HasValue ? news.Hits + 1 : 1;
                    Db.SaveChanges();
                    return true;
                } else {
                    ErrorMessage = "通知不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
            } catch(Exception ex) {
                SetError(ex.Message);
                return false;
            }
        }
    }
}
