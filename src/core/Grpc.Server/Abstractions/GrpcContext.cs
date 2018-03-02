using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public class GrpcContext
    {

        public ServerCallContext Request { get; set; }

        public Exception Exception { get; set; }
    }
}
