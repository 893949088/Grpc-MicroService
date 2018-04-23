using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Grpc.Hosting.Internal;

namespace Grpc.MicroService.Internal
{
    internal class GrpcMethodCallLogInterceptor : Interceptor
    {
        private readonly ILogger _logger;
        private readonly string _serverName;

        public GrpcMethodCallLogInterceptor(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("AliyunLogger");
            _serverName = serviceProvider.GetService<GrpcHostOptions>().ApplicationName;
        }

        #region Handler

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var headers = new Dictionary<string, string>();
            var contextHeaders = context.RequestHeaders.GetEnumerator();
            while (contextHeaders.MoveNext())
            {
                headers.Add(contextHeaders.Current.Key, contextHeaders.Current.Value);
            }
            contextHeaders.Reset();

            dynamic response = null;
            var exception = string.Empty;

            var watch = new Stopwatch();
            watch.Start();

            try
            {

                response = await continuation(request, context);
                return response;
            }
            catch(Exception ex)
            {
                exception = ex.ToString();
                throw ex;
            }
            finally
            {
                watch.Stop();

                _logger.LogInformation("GrpcService", "GrpcCall", _serverName, new Dictionary<string, string>()
                {
                    {"SourceName",headers.ContainsKey("sourcename") ? headers["sourcename"] : string.Empty} ,
                    {"ServerName",_serverName },
                    {"SververHost",context.Host},
                    {"Method",context.Method},
                    {"RequestValue", JsonConvert.SerializeObject(request)},
                    {"ReturnValue", response!=null?JsonConvert.SerializeObject(response):""},
                    {"Time", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") },
                    {"Exception",exception },
                    {"ElapsedMilliseconds",watch.ElapsedMilliseconds.ToString() }
                });
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
