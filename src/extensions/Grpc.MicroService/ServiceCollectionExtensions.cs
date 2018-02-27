using Grpc.MicroService.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddMicroService(this IServiceCollection services,Action<IMicroServiceBuilder> configureBuilder)
        {
            var builder = new MicroServiceBuilder(services);

            configureBuilder(builder);
            return services;
        }
    }
}
