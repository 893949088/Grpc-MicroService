using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Hosting.Internal;
using Grpc.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using zipkin4net;

namespace Grpc.MicroService.Internal
{
    public class ZipkinTracingServerInterceptor : Interceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _serverName;

        public ZipkinTracingServerInterceptor(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serverName = serviceProvider.GetService<GrpcHostOptions>().ApplicationName;
        }


        #region Handler

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var trace = ResolveTraceSpan(context) ?? Trace.Create();

            try
            {
                trace.Record(Annotations.ServerRecv());
                trace.Record(Annotations.ServiceName(_serverName));
                trace.Record(Annotations.Rpc("grpc"));
                return await continuation(request, context);
            }
            catch(Exception ex)
            {
                trace.Record(Annotations.Tag("error", ex.ToString()));
                throw ex;
            }
            finally
            {
                trace.Record(Annotations.ServerSend());
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

        private Trace ResolveTraceSpan(ServerCallContext context)
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                var headers = context.RequestHeaders.GetEnumerator();
                while (headers.MoveNext())
                {
                    dictionary.Add(headers.Current.Key, headers.Current.Value);
                }
                headers.Reset();

                if (!dictionary.ContainsKey("zipkin_traceid"))
                {
                    return null;
                }
                if (dictionary.ContainsKey("zipkin_parentspanid"))
                {
                    return Trace.CreateFromId(new SpanState(long.Parse(dictionary["zipkin_traceid"]), long.Parse(dictionary["zipkin_parentspanid"]), long.Parse(dictionary["zipkin_spanid"]), SpanFlags.Sampled | SpanFlags.SamplingKnown));
                }

                return Trace.CreateFromId(new SpanState(long.Parse(dictionary["zipkin_traceid"]), null, long.Parse(dictionary["zipkin_spanid"]), SpanFlags.Sampled|SpanFlags.SamplingKnown));

            }
            catch
            {
                return null;
            }
        }
    }
}
