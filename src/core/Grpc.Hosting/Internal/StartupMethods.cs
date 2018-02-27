using Grpc.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Hosting.Internal
{
    public class StartupMethods
    {

        public StartupMethods(object instance, Action<IGrpcServer> configure, Func<IServiceCollection, IServiceProvider> configureServices)
        {
            StartupInstance = instance;
            ConfigureDelegate = configure;
            ConfigureServicesDelegate = configureServices;
        }

        public object StartupInstance { get; }
        public Func<IServiceCollection, IServiceProvider> ConfigureServicesDelegate { get; }
        public Action<IGrpcServer> ConfigureDelegate { get; }
    }
}
