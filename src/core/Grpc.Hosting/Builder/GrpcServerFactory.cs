using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Hosting.Builder
{
    public class GrpcServerFactory: IGrpcServerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GrpcServerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGrpcServer CreateApplication()
        {
            return new GrpcServer(_serviceProvider);
        }
    }
}
