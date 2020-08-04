using Codans.Helper.Logs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public partial class WeiXinUser : Base
    {

        /// <summary>
        /// 校验令牌
        /// </summary>
        /// <param name="unionId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckUnionId(string unionId, out WeiXinUser user)
        {
            var xLog = new Logger();
            xLog.AddLine(" ================== WeiXinUser CheckUnionId ==================== ");
            user = null;
            try
            {
                user = Db.WeiXinUser.SingleOrDefault(x => x.UnionId == unionId || x.OpenId == unionId);
                if (user == null)
                {
                    var ww = new WeiXinUser
                    {
                        UnionId = unionId,
                        OpenId = "",
                        TinyAppOpenId = unionId,
                        NickName = "未知",
                        Gender = "",
                        Language = "",
                        Province = "",
                        City = "",
                        Comment = "",
                        HeadImage = "",
                        ComeFrom = "wechat-tinyapp", //wechat
                        Checkintime = DateTime.Now,
                        Subscribe = false,
                        LastAccessTime = DateTime.Now,
                        Status = 1,
                    };

                    Db.WeiXinUser.Add(ww);
                    Db.SaveChanges();
                    user = ww;
                }
                return true;
            }
            catch (DbEntityValidationException dex)
            {
                xLog.AddLine("DbEntityValidationException:\n" + dex.ToString());
                string error =
                    dex.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
                xLog.AddLine($"Error:{error}");
                SetError(error);
                return false;
            }
            catch (Exception e)
            {
                SetError(e);
                xLog.AddLine($"Exception:{Environment.NewLine}{e.ToString()}");
                return false;
            }
            finally
            {
                xLog.Save();
            }
        }

        public bool CheckParent(string unionId, out Parent parent)
        {
            var xLog = new Logger();
            xLog.AddLine(" ================== WeiXinUser CheckUnionId ==================== ");
            parent = null;
            try
            {
                var q = Db.WeiXinUser.Where(x => x.UnionId == unionId);
                
                if (q.Any())
                {
                    parent = q.SingleOrDefault().Parent.FirstOrDefault(x => x.Status == 1);
                    return true;
                }
                else
                {
                    ErrorMessage = "指定家长不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
            }
            catch (Exception e)
            {
                SetError(e);
                xLog.AddLine($"Exception:{Environment.NewLine}{e.ToString()}");
                return false;
            }
            finally
            {
                xLog.Save();
            }
        }

        public bool CheckTeacher(string unionId, Guid teamId, out Teacher teacher)
        {
            var xLog = new Logger();
            xLog.AddLine(" ================== WeiXinUser CheckTeacher ==================== ");
            teacher = null;
            try
            {
                var q = Db.WeiXinUser.Where(x => x.UnionId == unionId);

                if (q.Any())
                {
                    teacher = q.SingleOrDefault().Teacher.FirstOrDefault(x => x.Status == 1 
                    && x.TeamTeacher.Any(y=>y.TeamId==teamId && y.Status==(byte)EnumDataStatus.Normal));
                    return true;
                }
                else
                {
                    ErrorMessage = "指定家长不存在！";
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
            }
            catch (Exception e)
            {
                SetError(e);
                xLog.AddLine($"Exception:{Environment.NewLine}{e.ToString()}");
                return false;
            }
            finally
            {
                xLog.Save();
            }
        }

        public bool Welcome(string unionId, Guid teamId, out int teamNum, out int teacherNum, out int parentNum, out int studentNum, out EnumRole role)
        {
            role = EnumRole.NotRegister;

            teamNum = 0;
            teacherNum = 0;
            parentNum = 0;
            studentNum = 0;

            try
            {
                var q = Db.WeiXinUser.Where(x => x.UnionId == unionId);
                if (q.Any())
                {
                    var user = q.SingleOrDefault();
                    if (user.Teacher.Any(x => x.Status == 1))
                    {
                        role = EnumRole.Teacher;
                    }

                    if (user.Parent.Any(x => x.Status == 1))
                    {
                        role = EnumRole.Parent;
                    }
                }

                teamNum = Db.Team.Count(x => x.Status == 1);
                teacherNum = Db.Teacher.Count(x => x.Status == 1);
                parentNum = Db.Parent.Where(x => x.Status == 1 && x.StudentParent.Any(y => y.ParentId == x.ParentId && y.Student.TeamId == teamId)).GroupBy(x => x.ParentId).Count();
                studentNum = Db.Student.Count(x => x.Status == 1);

                return true;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }

        public bool FetchRole(string unionId, out WeiXinUser user, out List<StudentParent> parents, out List<TeamTeacher> teachers)
        {
            parents = null;
            teachers = null;
            user = null;
            try
            {
                var q = Db.WeiXinUser.Where(x => x.UnionId == unionId);
                if (q.Any())
                {
                    user = q.SingleOrDefault();

                    if (user.Teacher.Any(x => x.Status == 1))
                    {
                        var teacherId = user.Teacher.FirstOrDefault(x => x.Status > 0).TeacherId;
                        teachers = Db.TeamTeacher.Where(x=>x.Status>0 && x.TeacherId==teacherId).ToList();
                    }

                    if (user.Parent.Any(x => x.Status == 1))
                    {
                        var parentId = user.Parent.FirstOrDefault(x => x.Status > 0).ParentId;
                        parents = Db.StudentParent.Where(x => x.Status > 0 && x.ParentId == parentId).ToList();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }




        }

        #region 加载信息 LoadByTinyAppAccount
        /// <summary>
        /// 加载信息 LoadByTinyAppAccount
        /// </summary>
        /// <param name="unionId"></param>
        /// <param name="openId"></param>
        /// <param name="weiXinUser"></param>
        /// <returns></returns>
        public bool LoadByTinyAppAccount(string unionId, string openId, out WeiXinUser weiXinUser)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== LoadByTinyAppAccount ====================");
            xLog.AddLine($"OpenId  :{openId}");
            xLog.AddLine($"UnionId :{unionId}");
            weiXinUser = null;

            if (string.IsNullOrEmpty(unionId))
            {
                unionId = openId;
            }

            try
            {
                var q = Db.WeiXinUser.Where(x => (x.TinyAppOpenId == openId || (x.UnionId == unionId)) && x.Status == (byte)EnumDataStatus.Normal);
                if (q.Any())
                {
                    xLog.AddLine("Exist!");
                    weiXinUser = q.FirstOrDefault();

                    var id = unionId;
                    var q1 = Db.WeiXinUser.Where(x => x.UnionId == id && x.Status > 0);
                    if (q1.Any())
                    {
                        weiXinUser = q1.SingleOrDefault();

                    }
                    unionId = weiXinUser.UnionId;

                    xLog.AddLine($"OpenId  :{openId}");
                    xLog.AddLine($"UnionId :{unionId}");

                    return true;
                }
                else
                {
                    xLog.AddLine("NOT Exist!");
                    var w = new WeiXinUser
                    {
                        UnionId = !string.IsNullOrEmpty(unionId) ? unionId : openId,
                        OpenId = "",
                        Gender = "",
                        TinyAppOpenId = openId,
                        NickName = "未知",
                        ComeFrom = "wechat-tinyapp", //wechat
                        Checkintime = DateTime.Now,
                        Subscribe = false,
                        LastAccessTime = DateTime.Now,
                        Status = 1
                    };



                    Db.SaveChanges();
                    return true;
                }

            }
            catch (DbEntityValidationException dex)
            {
                xLog.AddLine("DbEntityValidationException:\n" + dex.ToString());
                string error =
                    dex.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
                xLog.AddLine($"Error:{error}");
                SetError(error);
                return false;
            }
            catch (Exception ex)
            {
                xLog.AddLine($"Exception:{ex.ToString()}");
                SetError(ex.ToString());
                return false;
            }
            finally
            {
                xLog.Save();
            }
        }
        #endregion

        #region 小程序注册 TinyAppRegister

        /// <summary>
        /// 小程序注册 TinyAppRegister
        /// </summary>
        /// <param name="unionId"></param>
        /// <param name="openId"></param>
        /// <param name="fromUserId"></param>
        /// <param name="gender"></param>
        /// <param name="language"></param>
        /// <param name="country"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="nickName"></param>
        /// <param name="comment"></param>
        /// <param name="headImage"></param>
        /// <param name="source"></param>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        public bool TinyAppRegister(string unionId, string openId, int fromUserId, int gender, string language, string country, string province,
          string city, string nickName, string comment, string headImage, string source = "weixin-tinyapp", bool subscribe = false)
        {
            var xLog = new Logger();
            xLog.AddLine("============== Business WeiXinUser TinyAppRegister ==================");
            xLog.AddLine($"UnionId :{unionId}");
            xLog.AddLine($"OpenId  :{openId}");
            try
            {
                var q0 = Db.WeiXinUser.Where(x => x.UnionId == unionId).OrderByDescending(x => x.Id);
                if (q0.Any())
                {
                    var w = q0.SingleOrDefault();
                    w.TinyAppOpenId = openId;
                    w.NickName = nickName;
                    w.Gender = (gender == 1 ? "男" : "女");
                    w.Language = language;
                    w.Province = province;
                    w.City = city;
                    w.Country = country;
                    w.Comment = comment;
                    w.HeadImage = headImage;
                    w.Subscribe = subscribe;
                    w.LastAccessTime = DateTime.Now;
                    w.Status = 1;

                    Db.SaveChanges();
                    xLog.AddLine("Done.");
                    return true;
                }

                var q = Db.WeiXinUser.Where(x => x.UnionId == openId && x.TinyAppOpenId == openId).OrderByDescending(x => x.Id);
                if (q.Any())
                {
                    var w = q.SingleOrDefault();

                    w.Status = 0;

                    var ww = new WeiXinUser
                    {
                        UnionId = unionId,
                        OpenId = "",
                        TinyAppOpenId = openId,
                        NickName = nickName,
                        Gender = (gender == 1 ? "男" : "女"),
                        Language = language,
                        Province = province,
                        City = city,
                        Comment = comment,
                        HeadImage = headImage,
                        ComeFrom = source, //wechat
                        Checkintime = DateTime.Now,
                        Subscribe = subscribe,
                        LastAccessTime = DateTime.Now,
                        Status = 1
                    };

                    Db.WeiXinUser.Add(ww);
                    xLog.AddLine("add new user and delete old user.");

                }
                else
                {
                    var w = new WeiXinUser
                    {
                        UnionId = unionId,
                        OpenId = "",
                        TinyAppOpenId = openId,
                        NickName = nickName,
                        Gender = (gender == 1 ? "男" : "女"),
                        Language = language,
                        Province = province,
                        City = city,
                        Comment = comment,
                        HeadImage = headImage,
                        ComeFrom = source, //wechat
                        Checkintime = DateTime.Now,
                        Subscribe = subscribe,
                        LastAccessTime = DateTime.Now,
                        Status = 1
                    };

                    Db.WeiXinUser.Add(w);
                    xLog.AddLine("add new user.");
                }

                Db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException dex)
            {
                string error =
                    dex.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage;
                SetError(error);
                xLog.AddLine($"Exception:{error}");
                return false;
            }
            catch (Exception ex)
            {
                xLog.AddLine($"Exception:{ex.ToString()}");
                SetError(ex);
                return false;
            }
            finally
            {
                xLog.Save();
            }
        }
        #endregion

        #region 家长注册 ParentRegister
        /// <summary>
        /// 家长注册
        /// </summary>
        /// <param name="unionId"></param>
        /// <param name="teamId"></param>
        /// <param name="studentName"></param>
        /// <param name="relationType"></param>
        /// <returns></returns>
        public bool ParentRegister(string unionId, Guid teamId, string studentName, string studentAvatar, string relationType)
        {
            try
            {
                var qUser = Db.WeiXinUser.Where(x => x.UnionId == unionId);
                if (!qUser.Any())
                {
                    ErrorMessage = "用户不存在！";
                    return false;
                }
                var user = qUser.SingleOrDefault();
                var qTeam = Db.Team.Where(x => x.TeamId == teamId);
                if (!qTeam.Any())
                {
                    ErrorMessage = "班级不存在！";
                    return false;
                }
                var team = qTeam.SingleOrDefault();
                var qStudent = Db.Student.Where(x => x.Name == studentName && x.TeamId == teamId);
                Guid studentId = Guid.NewGuid();
                if (!qStudent.Any())
                {
                    var student = new Student();
                    student.Status = 1;
                    student.AvatarUrl = studentAvatar;
                    student.Checkintime = DateTime.Now;
                    student.LastAccessTime = DateTime.Now;
                    student.Name = studentName;

                    student.StudentId = studentId;
                    student.No = 0;
                    student.IdNo = "";
                    student.TeamId = teamId;

                    Db.Student.Add(student);
                }
                else
                {
                    var student = qStudent.SingleOrDefault();
                    student.Status = 1;
                }

                var qTs = Db.TeamStudent.Where(x => x.StudentId == studentId && x.TeamId == teamId);

                if (!qTs.Any())
                {
                    var ts = new TeamStudent();
                    ts.Status = 1;
                    ts.Checkintime = DateTime.Now;
                    ts.StudentId = studentId;
                    ts.No = 0;
                    ts.TeamId = teamId;
                    Db.TeamStudent.Add(ts);
                }
                else
                {
                    var ts = qTs.SingleOrDefault();
                    ts.Status = 1;
                }

                var qPc = Db.StudentParent.Where(x => x.Parent.UnionId == unionId && x.StudentId == studentId);
                if (!qPc.Any())
                {
                    var pc = new StudentParent();
                    pc.Checkintime = DateTime.Now;
                    pc.Relationship = relationType;
                    pc.Status = 1;
                    pc.StudentId = studentId;
                    Db.StudentParent.Add(pc);
                }
                else
                {
                    var pc = qPc.SingleOrDefault();
                    pc.Status = 1;
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
    }
}
