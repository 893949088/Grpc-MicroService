using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting.Internal
{
    public class AutoRequestServicesStartupFilter : IStartupFilter
    {
        public Action<IGrpcServer> Configure(Action<IGrpcServer> next)
        {
            return builder =>
            {
                next(builder);
            };
        }
    }
}
