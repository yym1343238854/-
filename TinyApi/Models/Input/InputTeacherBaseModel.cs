using System;

namespace TinyApi.Models.Input
{

    /// <summary>
    /// 教职工输入基础类
    /// </summary>
    public class InputTeacherBaseModel:InputBaseModel
    {
        /// <summary>
        /// 班级编号
        /// </summary>
        public Guid TeamId { get; set; }

    }
}