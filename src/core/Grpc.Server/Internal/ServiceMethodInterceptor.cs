using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Internal
{
    public class ServiceMethodInterceptor : Interceptor
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly Type serviceType;
        private readonly IEnumerable<MethodInfo> _methods;

        public ServiceMethodInterceptor(Type serviceType, IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this.serviceType = serviceType;
            this._methods = serviceType.GetMethods();
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var methodName = context.Method.Split('/').Last();
            var callMethod = _methods.FirstOrDefault(p => p.Name == methodName);

            var serviceScopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var serviceInstance = scope.ServiceProvider.GetService(serviceType);
                var serviceResult = callMethod.Invoke(serviceInstance, new object[] { request, context });

                return serviceResult as Task<TResponse>;
            }
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var methodName = context.Method.Split('/').Last();
            var callMethod = _methods.FirstOrDefault(p => p.Name == methodName);

            var serviceScopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var serviceInstance = scope.ServiceProvider.GetService(serviceType);
                var serviceResult = callMethod.Invoke(serviceInstance, new object[] { requestStream, context });

                return serviceResult as Task<TResponse>;
            }
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var methodName = context.Method.Split('/').Last();
            var callMethod = _methods.FirstOrDefault(p => p.Name == methodName);

            var serviceScopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var serviceInstance = scope.ServiceProvider.GetService(serviceType);
                var serviceResult = callMethod.Invoke(serviceInstance, new object[] { request, responseStream, context });

                return serviceResult as Task;
            }
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var methodName = context.Method.Split('/').Last();
            var callMethod = _methods.FirstOrDefault(p => p.Name == methodName);

            var serviceScopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var serviceInstance = scope.ServiceProvider.GetService(serviceType);
                var serviceResult = callMethod.Invoke(serviceInstance, new object[] { requestStream, responseStream, context });

                return serviceResult as Task;
            }
        }
    }
}
