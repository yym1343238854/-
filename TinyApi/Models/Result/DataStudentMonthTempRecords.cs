using System;
using System.Collections.Generic;

namespace TinyApi.Models.Result
{
    /// <summary>
    /// 月数据
    /// </summary>
    public class DataStudentMonthTempRecords
    {
        /// <summary>
        /// 日数据数组
        /// </summary>
        public List<DataStudentDayRecord> Days { get; set; }
    }

    /// <summary>
    /// 日数据
    /// </summary>
    public class DataStudentDayRecord
    {
        /// <summary>
        /// 日期
        /// </summary>
        public int Day { get; set; }

        public string ColorCss { get; set; }

        /// <summary>
        /// 家庭测温颜色
        /// </summary>
        public DataTempRecord FamilyTempRecord { get; set; }

        /// <summary>
        /// 第一次学校测温颜色
        /// </summary>
        public DataTempRecord FirstTempRecord { get; set; }

        /// <summary>
        /// 第二次学校测温颜色
        /// </summary>
        public DataTempRecord SecondTempRecord { get; set; }

        /// <summary>
        /// 当日记录
        /// </summary>
        public List<DataTempRecord> Records { get; set; }
    }

    /// <summary>
    /// 测温记录
    /// </summary>
    public class DataTempRecord
    {
        public Guid RecordId { get; set; }

        public decimal Temperature { get; set; }

        public string Color { get; set; }

        public string Hhmm { get; set; }

        public string Content { get; set; }
    }
}