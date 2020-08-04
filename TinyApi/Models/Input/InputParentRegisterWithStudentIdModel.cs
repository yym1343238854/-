using System;

namespace TinyApi.Models.Input
{
    /// <summary>
    /// 家长注册输入模型
    /// </summary>
    public class InputParentRegisterWithStudentIdModel:InputBaseModel
    {
        /// <summary>
        /// 班级编号
        /// </summary>
        public string TeamId { get;set;}

        /// <summary>
        /// 学生编号
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// 家长姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdNo { get; set; }

        /// <summary>
        /// 与孩子关系
        /// </summary>
        public string Relation { get; set; }

    }
}