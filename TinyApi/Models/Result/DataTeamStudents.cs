using System;
using System.Collections.Generic;

namespace TinyApi.Models.Result
{
    /// <summary>
    /// 班级所有学生
    /// </summary>
    public class DataTeamStudents
    {
        /// <summary>
        /// 班级已有学生
        /// </summary>
        public List<DataTeamStudent> Students { get; set; }
    }

    /// <summary>
    /// 班级学生信息
    /// </summary>
    public class DataTeamStudent
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学生编号
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 是我的孩子
        /// </summary>
        public bool IsMyChild { get; set; }
    }
}