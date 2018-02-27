using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MicroServiceBuilderExtensions
    {
        public static IMicroServiceBuilder AddService<T>(this IMicroServiceBuilder builder) where T : class, IGrpcService
        {
            builder.Services.AddScoped<T>();
            builder.Services.AddSingleton<IGrpcService, T>();

            return builder;
        }
    }
}
