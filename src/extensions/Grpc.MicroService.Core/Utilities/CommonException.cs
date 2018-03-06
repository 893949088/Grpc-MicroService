using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService
{
    public class CommonException : Exception
    {

        // 错误code
        public string Code { get; set; }

        public CommonException(string message)
            : base(message)
        {

        }

        public CommonException(string code, string message) : base(message)
        {
            this.Code = code;
        }
    }
}
