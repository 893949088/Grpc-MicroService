using App.Metrics;
using App.Metrics.Histogram;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.MicroService.Internal
{
    internal class GrpcMetricsServerInterceptor : Interceptor
    {

        private readonly IMetrics _metrics;
        private readonly string _serverName;

        public GrpcMetricsServerInterceptor(IServiceProvider serviceProvider)
        {
            _serverName = serviceProvider.GetService<GrpcHostOptions>().ApplicationName;
            _metrics = serviceProvider.GetService<IMetrics>();
        }

        #region Handler

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            GrpcMetricsRegister.Timers.GrpcRequestTransactionDuration.Context = _serverName;           
            using (_metrics.Measure.Timer.Time(GrpcMetricsRegister.Timers.GrpcRequestTransactionDuration, context.Method))
            {
                var response = await continuation(request, context);
                return response;
            }
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {

            return continuation(requestStream, context);
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {

            return continuation(request, responseStream, context);
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {

            return continuation(requestStream, responseStream, context);
        }

        #endregion

    }
}
