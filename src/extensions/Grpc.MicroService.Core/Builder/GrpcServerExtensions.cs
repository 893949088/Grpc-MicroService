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

        public static IGrpcServer UseMicroService(this IGrpcServer server, Action<IMicroServiceConfiguration> configuration = null)
        {
            var microserviceConfigration = new MicroServiceConfiguration(server);

            if (configuration != null)
            {
                configuration(microserviceConfigration);
            }

            var loggerFactory = server.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog(microserviceConfigration.NLogBuilder.Build());

            return server;
        }
    }
}
