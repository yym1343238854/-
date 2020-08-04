using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Input {
    /// <summary>
    /// 学生体温信息
    /// </summary>
    public class InputBbtRecords : InputBaseModel {
        /// <summary>
        /// 学生编号
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// 指定月份/日期
        /// </summary>
        public DateTime SpecDay { get; set; }
    }
}