using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public struct MessageCode
    {
        public const string Success = "10000";

        public const string DefaultError = "99999";
    }
}
