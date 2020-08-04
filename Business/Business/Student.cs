using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public partial class Student:Base
    {
        /// <summary>
        /// 加载学生信息
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        public bool Load(Guid studentId,out Student student)
        {
            student = null;
            try
            {
                var q = Db.Student.Where(x => x.StudentId == studentId);
                if (q.Any())
                {
                    student = q.SingleOrDefault();
                    return true;
                }
                else
                {
                    ErrorMessage = "指定学生不存在！";
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
    }
}
