using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Grpc.Hosting.Internal;

namespace Grpc.Hosting
{
    public static class GrpcHostBuilderExtensions
    {

        public static IGrpcHostBuilder UseStartup(this IGrpcHostBuilder hostBuilder, Type startupType)
        {
            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;

            return hostBuilder
                .UseSetting(GrpcHostDefaults.ApplicationKey, startupAssemblyName)
                .UseSetting(GrpcHostDefaults.StartupAssemblyKey, startupAssemblyName)
                .ConfigureServices(services =>
                {
                    if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                    {
                        services.AddSingleton(typeof(IStartup), startupType);
                    }
                    else
                    {
                        services.AddSingleton(typeof(IStartup), sp =>
                        {
                            return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, startupType));
                        });
                    }
                });
        }

        public static IGrpcHostBuilder UseStartup<TStartup>(this IGrpcHostBuilder hostBuilder) where TStartup : class
        {
            return hostBuilder.UseStartup(typeof(TStartup));
        }
    }
}
