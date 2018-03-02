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
    public class ServerMethodInterceptor : Interceptor
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly Type serviceType;
        private readonly IEnumerable<MethodInfo> _methods;

        public ServerMethodInterceptor(Type serviceType, IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this.serviceType = serviceType;
            this._methods = serviceType.GetMethods();
        }

        #region Handler

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var methodName = context.Method.Split('/').Last();
            var callMethod = _methods.FirstOrDefault(p => p.Name == methodName);

            var serviceScopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var grpcContext = scope.ServiceProvider.GetService<GrpcContext>();
                grpcContext.Request = context;
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

        #endregion

        #region Call

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncUnaryCall(request, context, continuation);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncClientStreamingCall(context, continuation);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncServerStreamingCall(request, context, continuation);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncDuplexStreamingCall(context, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.BlockingUnaryCall(request, context, continuation);
        }

        #endregion
    }
}
