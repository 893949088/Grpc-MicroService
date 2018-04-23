using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public class MessageCodeException : Exception
    {

        // 错误code
        public string Code { get; set; }

        public MessageCodeException(string message)
            : base(message)
        {
            this.Code = MessageCode.DefaultError;
        }

        public MessageCodeException(string code, string message) : base(message)
        {
            this.Code = code;
        }
    }
}
