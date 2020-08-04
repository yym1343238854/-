using System.Linq;
using System.Web;

namespace TinyApi.Models.Result
{
    public class InvokeResult
    {
        /// <summary>
        /// 成功失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误提示
        /// </summary>
        public string ErrorMessage { get; set; }

        public int ErrorNumber { get; set; }


    }

    public class InvokeResult<T>
    {
        /// <summary>
        /// 带数据的返回
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 成功失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误提示
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorNumber { get; set; }
    }
}