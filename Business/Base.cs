using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    /// <summary>
    /// 系统基类
    /// </summary>
    public class Base
    {

        /// <summary>
        /// 执行是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 错误编号
        /// </summary>
        public int ErrorNumber { get; set; }

        public bool DebugMode = false;

        /// <summary>
        /// 无效日期
        /// </summary>
        public static DateTime NotValidDate = DateTime.Parse("1900-01-01");

        /// <summary>
        /// 数据库对象
        /// </summary>
        public HealthySchoolEntities Db { get; set; }

        public void SetError(Exception ex)
        {
            ErrorMessage = ex.Message + Environment.NewLine + ex + Environment.NewLine + ex.InnerException;
            Success = false;
        }

        public void SetError(string error)
        {
            ErrorMessage = error;
            Success = false;
        }

        public void SetError(EnumErrors errorNumber, string error)
        {
            ErrorNumber = (int)errorNumber;
            ErrorMessage = error;
            Success = false;
        }

        public Base()
        {
            Db = new HealthySchoolEntities();
            ErrorMessage = "";
            Success = false;
        }

        public enum EnumDataStatus
        {
            Delete = 0,
            Normal = 1

        }

        public enum EnumBbtType
        {
            晨测=1,
            午测=2,
            家测=3,
            抽测=4
        }

        public enum EnumColorStatus
        {
            未知=0,
            绿码=1,
            橙码=2,
            红码=3
        }

        //1.正常、2.在校隔离、3.就诊、4.病假、5.事假、6.居家隔离、7.定点隔离、8.疑似、9.确诊 10 治愈 100死亡
        public enum EnumHealthyStatus
        {
            未知=0,
            正常=1,
            在校隔离=2, 
            就诊=3,
            病假=4,
            事假=5,
            居家隔离=6,
            定点隔离=7,
            疑似=8,
            确诊=9,
            治愈=10,
            死亡=100

        }

        public enum EnumStudentStatus
        {
            删除=0,
            正常=1,
            离校=2
        }

        public enum EnumTeamStudentStatus
        {
            删除=0,
            正常=1,
            转走=2
        }

        //动态类型:1.测体温 2.消毒 3.通风 4.人员变化 5 其它
        public enum EnumTipsType
        {
            测温=1,
            消毒=2,
            通风=3,
            人员=4,
            健康码=5,
            其它=9
        }

        public enum EnumScheduleType
        {
            上学=1,
            放学=2,
            就餐=3
        }

        public enum EnumStudentParentStatus
        {
            删除=0,
            正常=1
        }

        public enum EnumNewsStatus {
            删除 = 0,
            保存 = 1,
            发布 = 2
        }

        public enum EnumNewsPeopleType {
            全部 = 0,
            老师 = 1,
            家长 = 2
        }

        public enum EnumNewsRangeType {
            全公开 = 1,
            部分班级 = 2
        }

        public enum EnumRoomType {
            教室 = 1,
            食堂,
            公共厕所,
            老师办公室,
            其他场所
        }

        public enum EnumSendSmsMode
        {
            Text = 1,
            Voice = 2
        }

        [Flags]
        public enum EnumRole
        {
            NotRegister = 0,
            Teacher = 1,
            Parent = 2
        }


        public enum EnumErrors
        {

            非法调用接口 = 100,
            账号未登录 = 101,
            参数校验错误 = 102,
            SessionId无效 = 103,

            信息不存在 = 200,

            //基础
            设备未登记 = 1000,
            设备已登记 = 1001,
            手机格式不正确 = 1002,
            手机号码已使用 = 1003,
            手机号码未注册 = 1004,
            验证码不正确 = 1005,
            短信发送次数超限 = 1006,
            令牌不正确 = 1007,
            请完善注册信息 = 1008,
            禁止登录 = 1009,
            密码不正确 = 1010,

        }

        public static DateTime TermStartDate = DateTime.Parse("2018-09-01");

        public static DateTime NotSetDate = DateTime.Parse("1970-01-01");

        public static string DemoUnionId = "ohxsKwAKd0RjqHq8TYGy8u-1uTuI";


        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="len">子字符串的长度</param>
        /// <param name="start">开始位置的索引</param>
        /// <returns></returns>
        public static string SubString(string sourceStr, int len, int start = 0)
        {
            if (!String.IsNullOrEmpty(sourceStr))
            {
                if (sourceStr.Length >= (start + len))
                    return sourceStr.Substring(start, len);
                else
                    return sourceStr.Substring(start);
            }
            return "";
        }

    }
}