using System;

namespace TinyApi.Models.Input
{

    /// <summary>
    /// 教室记录学生体温模型
    /// </summary>
    public class InputTeacherRecordStudentBbtModel:InputTeacherBaseModel
    {
        /// <summary>
        /// 学生编号
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// 摄氏度体温
        /// </summary>
        public decimal Temperature { get; set; }

        /// <summary>
        /// 类型 1 晨测 2 午测 3 家测 4 抽测
        /// </summary>
        public byte Type { get; set; }
    }
}