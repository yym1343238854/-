using System;

namespace TinyApi.Models.Input
{
    /// <summary>
    /// 输入基础类
    /// </summary>
    public class InputBaseModel
    {
        /// <summary>
        /// 联合编号
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

    }
}