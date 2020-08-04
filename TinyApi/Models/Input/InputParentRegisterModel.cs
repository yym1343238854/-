using System;

namespace TinyApi.Models.Input
{
    public class InputParentRegisterModel:InputBaseModel
    {
        /// <summary>
        /// 班级编号
        /// </summary>
        public Guid TeamId { get; set; }

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

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        public int StudentNo { get; set; }

        /// <summary>
        /// 学生身份证号
        /// </summary>
        public string StudentIdNo { get; set; }
    }
}