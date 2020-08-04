using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 体温记录列表
    /// </summary>
    public class DataBbtRecords {
        /// <summary>
        /// 体温记录列表
        /// </summary>
        public List<BbtRecordDetail> BbtRecords { get; set; }
    }
}