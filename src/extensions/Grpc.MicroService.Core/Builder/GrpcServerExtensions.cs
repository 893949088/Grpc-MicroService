using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public static class GrpcServerExtensions
    {

        public static IGrpcServer UseMicroService(this IGrpcServer server)
        {

            return server;
        }
    }
}
