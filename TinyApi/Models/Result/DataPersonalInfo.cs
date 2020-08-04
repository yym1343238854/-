using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 我的信息
    /// </summary>
    public class DataPersonalInfo {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImage { get; set; }

        /// <summary>
        /// 身份切换-教师身份
        /// </summary>
        public List<DataTeacherIdentity> Teachers { get; set; }

        /// <summary>
        /// 身份切换-家长身份
        /// </summary>
        public List<DataParentIdentity> Parents { get; set; }
    }

    /// <summary>
    /// 教师身份
    /// </summary>
    public class DataTeacherIdentity : DataTeacherRole {
        /// <summary>
        /// 班级名称
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        public string AvatarUrl { get; set; }
    }

    /// <summary>
    /// 家长身份
    /// </summary>
    public class DataParentIdentity : DataParentRole {
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 学生照片
        /// </summary>
        public string AvatarUrl { get; set; }
    }
}