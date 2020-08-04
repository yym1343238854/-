using System.Collections.Generic;

namespace TinyApi.Models.Result
{
    /// <summary>
    /// 家长打卡月数据
    /// </summary>
    public class DataParentMonthCheckinRecords
    {
        public List<DataParentDayCheckinRecord> Days { get; set; }

        public int CheckinDays { get; set; }
    }

    public class DataParentDayCheckinRecord
    {
        /// <summary>
        /// 日
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Hhmm { get; set; }

        /// <summary>
        /// 健康码颜色
        /// </summary>
        public int ColorStatus { get; set; }

        /// <summary>
        /// 颜色css类名
        /// </summary>
        public string ColorCss { get; set; }

        /// <summary>
        /// 附件图片（如果有）
        /// </summary>
        public string PhotoUrl { get; set; }
    }
}