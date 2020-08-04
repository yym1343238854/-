using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public partial class Team : Base
    {
        #region 加载班级 Load
        public bool Load(int id, out Team team)
        {
            team = null;
            try
            {
                var q = Db.Team.AsNoTracking().Where(x => x.Id == id);
                if (q.Any())
                {
                    team = q.SingleOrDefault();
                    return true;
                }
                else
                {
                    ErrorMessage = "指定班级编号不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }

        public bool Load(Guid teamId, out Team team)
        {
            team = null;
            try
            {
                var q = Db.Team.AsNoTracking().Where(x => x.TeamId == teamId);
                if (q.Any())
                {
                    team = q.SingleOrDefault();
                    return true;
                }
                else
                {
                    ErrorMessage = "指定班级编号不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }
        #endregion

        #region 列出所有学生 ListStudents
        public bool ListStudents(Guid teamId, out List<Student> students)
        {
            students = null;
            try
            {
                var q = Db.Team.AsNoTracking().Where(x => x.TeamId == teamId);
                if (q.Any())
                {
                    students = Db.Student.Where(x => x.TeamId == teamId && x.Status == (byte)EnumStudentStatus.正常).ToList();
                    return true;
                }
                else
                {
                    ErrorMessage = "指定班级编号不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }
        #endregion
    }
}
