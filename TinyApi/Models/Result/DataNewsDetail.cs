using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 通知详情
    /// </summary>
    public class DataNewsDetail : DataNews {
        /// <summary>
        /// 通知内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<DataNewsAttachMent> AttachMents { get; set; }
    }

    /// <summary>
    /// 附件
    /// </summary>
    public class DataNewsAttachMent {
        /// <summary>
        /// 附件编号
        /// </summary>
        public Guid AttachMentId { get; set; }

        /// <summary>
        /// 附件地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 附件大小
        /// </summary>
        public decimal FileSize { get; set; }

        /// <summary>
        /// 附件文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Ext { get; set; }
    }
}