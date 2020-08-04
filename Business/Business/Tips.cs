using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public partial class Tips : Base
    {
        /// <summary>
        /// 根据家长信息获取Tips
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="studentId"></param>
        /// <param name="specDay"></param>
        /// <param name="tipsList"></param>
        /// <returns></returns>
        public bool ListByParent(Guid parentId, Guid studentId, DateTime specDay,out List<Tips> tipsList)
        {
            tipsList = Db.Tips.Where(x => x.StudentId == studentId && 
                    DbFunctions.DiffDays(x.Checkintime,specDay)==0 &&
                    x.Student.StudentParent.Any(y => y.ParentId == parentId && y.Status == (byte)EnumDataStatus.Normal) && x.Status == (byte)EnumDataStatus.Normal)
                .OrderByDescending(x => x.ActionTime)
                .ToList();
            return true;
        }

        public bool ListRoomTips(Guid parentId, Guid teamId, DateTime specDay, out List<Tips> tipsList) {
            tipsList = Db.Tips.Where(x => x.Status == (byte)EnumDataStatus.Normal &&
                     x.RoomId != null &&
                    DbFunctions.DiffDays(x.Checkintime, specDay) == 0 &&
                    ((x.Room.Type == (byte)EnumRoomType.教室 && x.Room.TeamClassroom.Any(y => x.Status == (byte)EnumDataStatus.Normal && y.TeamId == teamId)) || x.Room.Type != (byte)EnumRoomType.教室))
                .OrderByDescending(x => x.ActionTime)
                .ToList();
            return true;
        }
    }
}
