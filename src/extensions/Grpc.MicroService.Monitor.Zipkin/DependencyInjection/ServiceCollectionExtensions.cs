using Grpc.MicroService.Internal;
using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddZipkinTracingGrpcClient<T>(this IServiceCollection service, string target, string serviceName)
        {

            service.AddScoped(typeof(T), p =>
            {
                var grpcContext = p.GetRequiredService<GrpcContext>();
                return Activator.CreateInstance(typeof(T), new ZipkinTracingCallInvoker(grpcContext.Request,target, serviceName));
            });

            return service;
        }
    }
}
