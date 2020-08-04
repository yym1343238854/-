using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 防疫资讯信息
    /// </summary>
    public class DataEpidemicPreventionInfo {
        /// <summary>
        /// 学生数量
        /// </summary>
        public int StudentNum { get; set; }

        /// <summary>
        /// 教职工数量
        /// </summary>
        public int TeacherNum { get; set; }

        /// <summary>
        /// 通知列表
        /// </summary>
        public List<DataNews> News { get; set; }

        /// <summary>
        /// 指南列表
        /// </summary>
        public List<DataDocument> Documents { get; set; }
    }

    /// <summary>
    /// 通知
    /// </summary>
    public class DataNews {
        /// <summary>
        /// 通知编号
        /// </summary>
        public Guid NewsId { get; set; }

        /// <summary>
        /// 通知标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 通知发布时间
        /// </summary>
        public string PublishTimeStr { get; set; }

        /// <summary>
        /// 浏览人次
        /// </summary>
        public int Hits { get; set; }
    }

    /// <summary>
    /// 防疫指南
    /// </summary>
    public class DataDocument {
        /// <summary>
        /// 指南编号
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// 指南标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 指南发布时间
        /// </summary>
        public string PublishTimeStr { get; set; }

        /// <summary>
        /// 下载人次
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// 指南地址
        /// </summary>
        public string DocumentUrl { get; set; }
    }
}