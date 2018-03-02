using Microsoft.AspNetCore.Zipkin.Internal;
using System;
using System.Collections.Generic;
using System.Text;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddZipkinTracingGrpcClient<T>(this IServiceCollection service,string target,string serviceName)
        {

            service.AddScoped(typeof(T), p =>
            {
                return Activator.CreateInstance(typeof(T), new ZipkinTracingCallInvoker(target, serviceName));
            });

            return service;
        }
    }
}
