using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 测温日历
    /// </summary>
    public class DataBbtCalendar {
        /// <summary>
        /// 测温日期列表
        /// </summary>
        public List<SpecDay> SpecDays { get; set; }
    }

    /// <summary>
    /// 测温信息
    /// </summary>
    public class SpecDay {
        /// <summary>
        /// 日期
        /// </summary>
        public string DateStr { get; set; }

        /// <summary>
        /// 体温是否正常（true:正常 false.异常）
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 测温次数
        /// </summary>
        public int MeasureNum { get; set; }
    }
}