using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public partial class Parent : Base
    {
        #region 打卡 Checkin
        public bool Checkin(Guid parentId,EnumColorStatus color)
        {
            try
            {
                var pc = new ParentCheckin();
                pc.Checkintime = DateTime.Now;
                pc.ColorStatus = (byte)color;
                pc.ParentCheckinId = Guid.NewGuid();
                pc.ParentId = parentId;
                pc.SpecDay = DateTime.Today;
                pc.Status = 1;
                Db.ParentCheckin.Add(pc);

                var qStudents = Db.StudentParent.Where(x => x.ParentId == parentId && x.Status == (byte)EnumDataStatus.Normal);
                foreach (var sp in qStudents)
                {
                    //增加信息提示
                    var tips = new Tips();
                    tips.ActionTime = DateTime.Now;
                    tips.Checkintime = DateTime.Now;
                    tips.Content = $"家长【{sp.Relationship}】打卡";
                    switch (color)
                    {
                        case EnumColorStatus.绿码:
                            {
                                tips.FontColor = "green";
                                break;
                            }
                        case EnumColorStatus.橙码:
                            {
                                tips.FontColor = "orange";
                                break;
                            }
                        case EnumColorStatus.红码:
                            {
                                tips.FontColor = "red";
                                break;
                            }
                        default:
                            {
                                tips.FontColor = "";
                                break;
                            }

                    }
                                       
                    tips.Status = (byte)EnumDataStatus.Normal;
                    tips.StudentId = sp.StudentId;
                    tips.TipsId = Guid.NewGuid();
                    tips.Type = (byte)EnumTipsType.健康码;

                    Db.Tips.Add(tips);
                }
                

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

        #region 注册
        #region 注册（有StudentId）
        /// <summary>
        /// 注册（有StudentId）
        /// </summary>
        /// <param name="unionId"></param>
        /// <param name="studentId"></param>
        /// <param name="name"></param>
        /// <param name="mobile"></param>
        /// <param name="idNo"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public bool Register(string unionId, Guid studentId, string name, string mobile, string idNo, string relation)
        {
            try
            {
                var q = Db.Parent.Where(x => x.UnionId == unionId);
                var parentId = Guid.NewGuid();
                if (!q.Any())
                {
                    var parent = new Parent();
                    parent.Checkintime = DateTime.Now;
                    parent.ColorStatus = 0;
                    parent.HealthyStatus = 0;
                    parent.IdNo = idNo;
                    parent.LastAccessTime = DateTime.Now;
                    parent.Mobile = mobile;
                    parent.Name = name;
                    parent.ParentId = parentId;
                    parent.Status = (byte)EnumDataStatus.Normal;
                    parent.UnionId = unionId;
                    Db.Parent.Add(parent);
                }
                else
                {
                    parentId = q.SingleOrDefault().ParentId;
                }

                var q1 = Db.StudentParent.Where(x => x.ParentId == parentId && x.StudentId == studentId);
                if (!q1.Any())
                {
                    var sp = new StudentParent();
                    sp.Checkintime = DateTime.Now;
                    sp.ParentId = parentId;
                    sp.Relationship = relation;
                    sp.Status = (byte)EnumDataStatus.Normal;
                    sp.StudentId = studentId;
                    Db.StudentParent.Add(sp);
                }

                Db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException dex)
            {
                string error =
                    dex.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
                SetError(error);
                return false;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 注册家长
        /// </summary>
        /// <param name="unionId"></param>
        /// <param name="name"></param>
        /// <param name="mobile"></param>
        /// <param name="idNo"></param>
        /// <param name="relation"></param>
        /// <param name="teamId"></param>
        /// <param name="studentName"></param>
        /// <param name="studentIdNo"></param>
        /// <param name="studentAvatarUrl"></param>
        /// <param name="studentNo"></param>
        /// <returns></returns>
        public bool Register(string unionId, string name, string mobile, string idNo, string relation,
            Guid teamId, string studentName, string studentIdNo, string studentAvatarUrl, int studentNo,out Guid parentNewId,out Guid studentNewId)
        {
            parentNewId = Guid.Empty;
            studentNewId = Guid.Empty;
            try
            {
                var q = Db.Parent.Where(x => x.UnionId == unionId);
                var parentId = Guid.NewGuid();
                if (!q.Any())
                {
                    var parent = new Parent();
                    parent.Checkintime = DateTime.Now;
                    parent.ColorStatus = (byte)EnumColorStatus.未知;
                    parent.HealthyStatus = (byte)EnumHealthyStatus.未知;
                    parent.IdNo = idNo;
                    parent.LastAccessTime = DateTime.Now;
                    parent.Mobile = mobile;
                    parent.Name = name;
                    parent.ParentId = parentId;
                    parent.Status = (byte)EnumDataStatus.Normal;
                    parent.UnionId = unionId;
                    Db.Parent.Add(parent);
                }
                else
                {
                    parentId = q.SingleOrDefault().ParentId;
                }

                var qTeam = Db.Team.Where(x => x.TeamId == teamId && x.Status != 0);
                if (!qTeam.Any())
                {
                    SetError("班级不存在");
                    return false;
                }

                var team = qTeam.SingleOrDefault();

                var studentId = Guid.NewGuid();

                var qStudent = Db.Student.Where(x => x.Name == studentName && x.TeamId == teamId);
                if (qStudent.Any())
                {
                    //同一班级，假设不存在学生同名的情况
                    var student = qStudent.SingleOrDefault();
                    studentId = student.StudentId;
                    student.Status = (byte)EnumStudentStatus.正常;
                }
                else
                {
                    //创建学生
                    Db.Student.Add(new Student
                    {
                        AvatarUrl = studentAvatarUrl,
                        Checkintime = DateTime.Now,
                        StudentId = studentId,
                        Status = (byte)EnumDataStatus.Normal,
                        IdNo = studentIdNo,
                        Name = studentName,
                        No = studentNo,
                        TeamId = teamId,
                        HealthyStatus = (byte)EnumHealthyStatus.正常,
                        ColorStatus = (byte)EnumColorStatus.未知
                    });

                    //增加信息提示
                    var tips = new Tips();
                    tips.ActionTime = DateTime.Now;
                    tips.Checkintime = DateTime.Now;
                    tips.Content = $"学生【{studentName}】登记了";
                    tips.FontColor = "";
                    tips.Status = (byte)EnumDataStatus.Normal;
                    tips.StudentId = studentId;
                    tips.TipsId = Guid.NewGuid();
                    tips.Type = (byte)EnumTipsType.人员;

                    Db.Tips.Add(tips);
                }

                var q1 = Db.StudentParent.Where(x => x.StudentId == studentId && x.ParentId == parentId);
                if (q1.Any())
                {
                    var sp = q1.SingleOrDefault();
                    sp.Status = (byte)EnumStudentStatus.正常;
                    sp.Relationship = relation;
                }
                else
                {
                    // 创建学生家长关联记录
                    Db.StudentParent.Add(new StudentParent()
                    {
                        Checkintime = DateTime.Now,
                        ParentId = parentId,
                        Relationship = relation,
                        Status = (byte)EnumStudentStatus.正常,
                        StudentId = studentId,
                    });
                }

                var q2 = Db.TeamStudent.Where(x => x.StudentId == studentId && x.TeamId == teamId);
                if (q2.Any())
                {
                    var ts = q2.SingleOrDefault();
                    ts.Status = (byte)EnumTeamStudentStatus.正常;
                }
                else
                {
                    //创建学生班级关联记录
                    Db.TeamStudent.Add(new TeamStudent()
                    {
                        Checkintime = DateTime.Now,
                        StudentId = studentId,
                        TeamId = teamId,
                        Status = (byte)EnumTeamStudentStatus.正常
                    });
                }

                var studentNum = 1 + Db.TeamStudent.Count(x => x.Status == (byte)EnumTeamStudentStatus.正常 && x.TeamId == teamId);
                var parentNum = 1 + Db.Parent.Count(x => x.StudentParent.Any(y => y.Student.TeamId == teamId && y.Status == (byte)EnumStudentParentStatus.正常)
                  && x.Status == (byte)EnumDataStatus.Normal);

                team.StudentNum = studentNum;
                team.ParentNum = parentNum;

                parentNewId = parentId;
                studentNewId = studentId;
                
                Db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException dex)
            {
                string error = dex.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
                SetError(error);
                return false;
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
