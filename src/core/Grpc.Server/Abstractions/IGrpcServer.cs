using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public interface IGrpcServer
    {
        IServiceProvider ApplicationServices { get; set; }

        IList<Interceptor> ServiceInterceptors { get; }

        IEnumerable<ServerServiceDefinition> BuildServiceDefinition();
        
        IGrpcServer UseInterceptor(Interceptor interceptor);

    }
}
