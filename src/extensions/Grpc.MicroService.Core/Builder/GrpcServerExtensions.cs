using Grpc.MicroService;
using Grpc.MicroService.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Server
{
    public static class GrpcServerExtensions
    {

        public static IGrpcServer UseMicroService(this IGrpcServer server, Action<IMicroServiceOptions> configuration = null)
        {
            var microserviceOptions = new MicroServiceOptions(server);

            if (configuration != null)
            {
                configuration(microserviceOptions);
            }

            var loggerFactory = server.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog(microserviceOptions.NLogBuilder.Build());

            return server;
        }
    }
}
