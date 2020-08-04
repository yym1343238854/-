using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Input {
    /// <summary>
    /// 上传体温
    /// </summary>
    public class InputUploadBbt : InputBaseModel {
        /// <summary>
        /// 学生编号
        /// </summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        public decimal Temperature { get; set; }

        /// <summary>
        /// 测温类型(1.晨测 2.午测 3.家测 4.临时检查)
        /// </summary>
        public byte Type { get; set; }
    }
}