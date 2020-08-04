using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Input {
    /// <summary>
    /// 家长首页
    /// </summary>
    public class InputStudentIdModel : InputBaseModel {
        /// <summary>
        /// 学生唯一编号
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// 班级唯一编号
        /// </summary>
        public Guid? TeamId { get; set; }
    }
}