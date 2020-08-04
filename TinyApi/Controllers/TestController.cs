using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TinyApi.Models.Result;

namespace TinyApi.Controllers
{
    /// <summary>
    /// 测试操作
    /// </summary>
    public class TestController : ApiController
    {
        public InvokeResult HelloWorld()
        {
            return new InvokeResult() { ErrorMessage = "Hello world!", ErrorNumber = 0, Success = true };
        }
    }
}
