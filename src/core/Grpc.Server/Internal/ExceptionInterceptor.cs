using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Server.Internal
{
    internal class ExceptionInterceptor : Interceptor
    {



        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                var response = Activator.CreateInstance<TResponse>();
                TryProperty(response, "Code", MessageCode.DefaultError);
                TryProperty(response, "Message", "哎呀，服务开了个小差 (>﹏<)~！");
                TryProperty(response, "Success", false);
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


        private void TryProperty<T>(T response, string propertyName, object value)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var prop = props.FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
            {
                prop.SetValue(response, value);
            }
        }

    }
}
