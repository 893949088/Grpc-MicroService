using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting
{
    public interface IStartupFilter
    {
        Action<IGrpcServer> Configure(Action<IGrpcServer> next);
    }
}
