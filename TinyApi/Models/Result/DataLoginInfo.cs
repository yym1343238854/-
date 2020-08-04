using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result
{
    /// <summary>
    /// 登录接口返回结果
    /// </summary>
    public class DataLoginInfo
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 个人二维码
        /// </summary>
        public string QrcodeUrl { get; set; }

        public List<DataTeacherRole> Teachers { get; set; }

        public List<DataParentRole> Parents { get; set; }
    }
}