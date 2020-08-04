using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{

    /// <summary>
    /// 学生体温操作
    /// </summary>
    public partial class StudentBbtRecord : Base
    {
        #region 家长给学生测温记录 AddByParent
        /// <summary>
        /// 家长给学生测温记录
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="studentId"></param>
        /// <param name="specDay"></param>
        /// <param name="temperature"></param>
        /// <param name="photoUrl"></param>
        /// <returns></returns>
        public bool AddByParent(Guid parentId, Guid studentId, DateTime specDay, decimal temperature, string photoUrl)
        {
            try
            {
                var q = Db.StudentParent.Where(x => x.ParentId == parentId && x.StudentId == studentId);
                if (!q.Any())
                {
                    ErrorMessage = "家长或者学生信息不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }

                var sp = q.SingleOrDefault();

                var bbt = new StudentBbtRecord();
                bbt.Checkintime = DateTime.Now;
                bbt.ParentId = parentId;
                bbt.PhotoUrl = photoUrl;
                bbt.SpecDay = specDay.Date;
                bbt.Status = (byte)EnumDataStatus.Normal;
                bbt.StudentBbtRecordId = Guid.NewGuid();
                bbt.StudentId = studentId;
                bbt.Temperature = temperature;
                bbt.Type = (byte)EnumBbtType.家测;
                Db.StudentBbtRecord.Add(bbt);            

                //增加信息提示
                var tips = new Tips();
                tips.ActionTime = DateTime.Now;
                tips.Checkintime = DateTime.Now;
                tips.Content = $"家中测温为{temperature:F1}摄氏度";
                if (temperature> (decimal)37.3)
                {
                    tips.FontColor = "red";
                }
                else
                {
                    tips.FontColor = "green";
                }
                
                tips.Status = (byte)EnumDataStatus.Normal;
                tips.StudentId = studentId;
                tips.TipsId = Guid.NewGuid();
                tips.Type = (byte)EnumTipsType.测温;
                Db.Tips.Add(tips);

                Db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }
        #endregion

        #region 记录学校测学生体温 AddByTeacher
        /// <summary>
        /// 记录学校测学生体温
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="studentId"></param>
        /// <param name="specDay"></param>
        /// <param name="type"></param>
        /// <param name="temperature"></param>
        /// <param name="photoUrl"></param>
        /// <returns></returns>
        public bool AddByTeacher(Guid teacherId, Guid studentId, DateTime specDay, EnumBbtType type, decimal temperature, string photoUrl)
        {
            try
            {
                var q = Db.Student.Where(x => x.StudentId == studentId && x.Team.TeamTeacher.Any(y=>y.TeacherId==teacherId));
                if (!q.Any())
                {
                    ErrorMessage = "家长或者学生信息不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }

                var student = q.SingleOrDefault();

                var bbt = new StudentBbtRecord();
                bbt.Checkintime = DateTime.Now;
                bbt.TeacherId = teacherId;
                bbt.PhotoUrl = photoUrl;
                bbt.SpecDay = specDay;
                bbt.Status = (byte)EnumDataStatus.Normal;
                bbt.StudentBbtRecordId = Guid.NewGuid();
                bbt.StudentId = studentId;
                bbt.Temperature = temperature;
                bbt.Type = (byte)type;
                Db.StudentBbtRecord.Add(bbt);
                
                //增加信息提示
                var tips = new Tips();
                tips.ActionTime = DateTime.Now;
                tips.Checkintime = DateTime.Now;
                tips.Content = $"学校测温为{temperature:F1}摄氏度";
                if (temperature > (decimal)37.3)
                {
                    tips.FontColor = "red";
                }
                else
                {
                    tips.FontColor = "green";
                }

                tips.Status = (byte)EnumDataStatus.Normal;
                tips.StudentId = studentId;
                tips.TipsId = Guid.NewGuid();
                tips.Type = (byte)EnumTipsType.测温;
                Db.Tips.Add(tips);

                Db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(ex.Message);
                if (ex is DbEntityValidationException)
                {
                    sb.AppendLine("验证异常：");
                    foreach (var item in ((DbEntityValidationException)ex).EntityValidationErrors)
                    {
                        foreach (var error in item.ValidationErrors)
                        {
                            sb.AppendLine(error.ErrorMessage);
                        }
                    }
                }
                ErrorMessage = sb.ToString();
                return false;
            }
        }
        #endregion

        #region 取学生体温 ListByStudent
        public bool ListByStudent(Guid studentId, DateTime specDay, out List<StudentBbtRecord> bbts)
        {
            bbts = Db.StudentBbtRecord.AsNoTracking().Where(x => x.SpecDay == SpecDay.Date && x.StudentId == studentId).OrderByDescending(x => x.Checkintime).ToList();
            return true;
        }
        #endregion

        #region 取测温历史 ListMonth
        /// <summary>
        /// 获取一个月的测温历史
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="bbts"></param>
        /// <returns></returns>
        public bool ListMonthByStudent(Guid studentId,int year,int month,out List<StudentBbtRecord> bbts)
        {
            bbts = Db.StudentBbtRecord.Where(x => x.StudentId == studentId && x.SpecDay.Year == year && x.SpecDay.Month == month && x.Status == (byte)EnumDataStatus.Normal).OrderBy(x => x.SpecDay).ToList();
            return true;
        }
        #endregion
    }
}
