using System;

namespace TinyApi.Models.Input
{
    /// <summary>
    /// 学生某月测温记录
    /// </summary>
    public class InputStudentMonthTempRecordsModel:InputBaseModel
    {
        /// <summary>
        /// 年
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 学生编号
        /// </summary>
        public Guid StudentId { get; set; }
    }
}