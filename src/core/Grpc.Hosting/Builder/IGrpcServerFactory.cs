using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting.Builder
{
    public interface IGrpcServerFactory
    {
        IGrpcServer CreateApplication();
    }
}
