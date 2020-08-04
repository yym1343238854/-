using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 家庭成员
    /// </summary>
    public class DataFamilyMember {
        /// <summary>
        /// 班级学生具体信息
        /// </summary>
        public DataTeamInfo Team { get; set; }

        /// <summary>
        /// 今日信息
        /// </summary>
        public string TodayString { get; set; }

        /// <summary>
        /// 家庭成员列表
        /// </summary>
        public List<FamilyMember> FamilyMembers { get; set; }
    }

    /// <summary>
    /// 家庭成员
    /// </summary>
    public class FamilyMember {
        /// <summary>
        /// 家长编号
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 家长姓名
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 是否绑定微信
        /// </summary>
        public bool HasBindWeiXin { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImage { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
    }
}