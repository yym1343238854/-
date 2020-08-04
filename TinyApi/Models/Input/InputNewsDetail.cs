using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Input {
    /// <summary>
    /// 通知详情
    /// </summary>
    public class InputNewsDetail :InputBaseModel {
        /// <summary>
        /// 通知编号
        /// </summary>
        public Guid NewsId { get; set; }
    }
}