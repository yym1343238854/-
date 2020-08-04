using Business;
using Codans.Helper.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TinyApi.Core;
using TinyApi.Models.Input;
using TinyApi.Models.Result;

namespace TinyApi.Controllers
{
    /// <summary>
    /// 班级操作
    /// </summary>
    public class TeamController : ApiController
    {
        #region 列出指定班级已登记的所有学生 ListStudents
        /// <summary>
        /// 列出班级已登记的学生
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataTeamStudents> ListStudents(InputTeamIdModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Team ListStudents ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");


            var r = new InvokeResult<DataTeamStudents>() { Data = new DataTeamStudents() };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckUnionId(model.UnionId, out _))
                {
                    var xTeam = new Team();
                    if (xTeam.ListStudents(model.TeamId, out var students))
                    {
                        if (students != null)
                        {
                            r.Data.Students = new List<DataTeamStudent>();
                            foreach (var item in students)
                            {
                                var ts = new DataTeamStudent();
                                ts.AvatarUrl = item.AvatarUrl.FixImagePath(); //修正为全路径字符串
                                ts.Name = item.Name;
                                ts.StudentId = item.StudentId;
                                ts.IsMyChild = item.StudentParent.Any(x => x.Parent.UnionId == model.UnionId && x.Status == 1);
                                r.Data.Students.Add(ts);
                            }
                            r.Success = true;
                            xLog.AddLine($"Success.");
                        }
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xTeam.ErrorMessage;
                        r.ErrorNumber = xTeam.ErrorNumber;
                        xLog.AddLine($"ListStudents Error:{r.ErrorMessage}");
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "当前用户未注册！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckUnion Error:{r.ErrorMessage}");
                }
                return r;

            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r)}");
                xLog.Save();
            }
        }
        #endregion

        #region 加载指定班级基本信息 Load
        /// <summary>
        /// 加载指定班级基本信息  
        ///
        /// 如果有学生编号传入，则同时返回指定学生信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataTeamInfo> Load(InputTeamIdWidthStudentIdModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Team Load ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");


            var r = new InvokeResult<DataTeamInfo>() { Data = new DataTeamInfo() };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckUnionId(model.UnionId, out _))
                {
                    var xTeam = new Team();
                    if (xTeam.Load(model.TeamId, out var team))
                    {
                        if (team != null)
                        {
                            r.Data.ParentNum = team.ParentNum;
                            var teamRoom = team.TeamClassroom.SingleOrDefault(x => x.Status == 1);
                            r.Data.RoomName = teamRoom != null ? teamRoom.Room.Name : "";
                            r.Data.SchoolName = team.School.Name;
                            r.Data.StudentNum = team.StudentNum;
                            r.Data.TeacherNum = team.TeacherNum;
                            r.Data.TeamId = team.TeamId;
                            r.Data.TeamName = team.Name;

                        }

                        if (model.StudentId.HasValue)
                        {
                            var xStudent = new Student();
                            if (xStudent.Load(model.StudentId.Value, out var student))
                            {
                                if (student != null)
                                {
                                    r.Data.Student = new DataStudentInfo();
                                    r.Data.Student.StudentId = student.StudentId;
                                    r.Data.Student.No = student.No;
                                    r.Data.Student.Name = student.Name;
                                    r.Data.Student.IdNo = student.IdNo;
                                    r.Data.Student.AvatarUrl = student.AvatarUrl.FixImagePath();
                                }
                            }
                        }

                        r.Success = true;
                        xLog.AddLine("Success.");
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xTeam.ErrorMessage;
                        r.ErrorNumber = xTeam.ErrorNumber;
                        xLog.AddLine($"ListStudents Error:{r.ErrorMessage}");
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "当前用户未注册！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckUnion Error:{r.ErrorMessage}");
                }

                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r)}");
                xLog.Save();
            }

        }
        #endregion

        /// <summary>
        /// 根据整型Id或者Guid型TeamId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<Guid> LoadTeamId(InputBaseWithTeamIdModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Team LoadTeamId ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");


            var r = new InvokeResult<Guid>() { Data = Guid.Empty };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckUnionId(model.UnionId, out _))
                {
                    var xTeam = new Team();
                    if (xTeam.Load(model.TeamId, out var team))
                    {
                        if (team != null)
                        {
                            r.Data = team.TeamId;
                            r.Success = true;
                        }
                        else
                        {
                            r.Success = false;
                            r.ErrorMessage = xTeam.ErrorMessage;
                            r.ErrorNumber = xTeam.ErrorNumber;
                            xLog.AddLine($"ListStudents Error:{r.ErrorMessage}");
                        }
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xTeam.ErrorMessage;
                        r.ErrorNumber = xTeam.ErrorNumber;
                        xLog.AddLine($"ListStudents Error:{r.ErrorMessage}");
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "当前用户未注册！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckUnion Error:{r.ErrorMessage}");
                }
                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r)}");
                xLog.Save();
            }
        }
    }
}
