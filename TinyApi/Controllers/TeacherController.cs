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
using TinyApi.Models.Input;
using TinyApi.Models.Result;

namespace TinyApi.Controllers
{
    /// <summary>
    /// 老师操作
    /// </summary>
    public class TeacherController : ApiController
    {
        #region 记录学生体温 RecordStudentBbt
        /// <summary>
        /// 记录学生体温
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult RecordStudentBbt(InputTeacherRecordStudentBbtModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Teacher RecordStudentBbt ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult();
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckTeacher(model.UnionId,model.TeamId,out var teacher))
                {
                    var xBbt = new StudentBbtRecord();

                    if (xBbt.AddByTeacher(teacher.TeacherId,model.StudentId,DateTime.Today,(Base.EnumBbtType)model.Type, model.Temperature,""))
                    {
                        xLog.AddLine("Success.");
                        r.Success = true;
                    }
                    else
                    {
                        xLog.AddLine("Error:" + xBbt.ErrorMessage);
                        r.Success = false;
                        r.ErrorMessage = xBbt.ErrorMessage;
                        r.ErrorNumber = xBbt.ErrorNumber;
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "教职工信息不存在！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckTeacher Error:{r.ErrorMessage}");
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
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion
    }
}
