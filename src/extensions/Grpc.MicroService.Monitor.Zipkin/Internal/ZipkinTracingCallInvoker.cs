using Grpc.Core;
using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;
using zipkin4net;

namespace Grpc.MicroService.Internal
{
    public class ZipkinTracingCallInvoker : CallInvoker
    {
        private readonly string _target;
        private readonly string _serviceName;
        private readonly ServerCallContext _context;

        public ZipkinTracingCallInvoker(ServerCallContext context, string target, string serviceName)
        {
            _context = context;
            _target = target;
            _serviceName = serviceName;
        }

        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var trace = this.ResolveTraceSpan().Child();

            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.ServiceName(_serviceName));
            trace.Record(Annotations.Rpc("grpc"));

            var channel = new Channel(_target, ChannelCredentials.Insecure);
            var call = CreateCall(channel, method, host, options, trace);
            try
            {

                dynamic response = Calls.BlockingUnaryCall(call, request);
                return response as TResponse;
            }
            finally
            {
                trace.Record(Annotations.ClientRecv());
                channel.ShutdownAsync();
            }
        }

        /// <summary>
        /// Invokes a simple remote call asynchronously.
        /// </summary>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            var trace = this.ResolveTraceSpan().Child();

            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.ServiceName(_serviceName));
            trace.Record(Annotations.Rpc("grpc"));

            var channel = new Channel(_target, ChannelCredentials.Insecure);
            var call = CreateCall(channel, method, host, options, trace);
            try
            {

                var response = Calls.AsyncUnaryCall(call, request);
                return response;
            }
            finally
            {
                trace.Record(Annotations.ClientRecv());
                channel.ShutdownAsync();
            }
        }

        /// <summary>
        /// Invokes a server streaming call asynchronously.
        /// In server streaming scenario, client sends on request and server responds with a stream of responses.
        /// </summary>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            var trace = this.ResolveTraceSpan().Child();

            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.ServiceName(_serviceName));
            trace.Record(Annotations.Rpc("grpc"));

            var channel = new Channel(_target, ChannelCredentials.Insecure);
            var call = CreateCall(channel, method, host, options, trace);
            try
            {
                var response = Calls.AsyncServerStreamingCall(call, request);
                return response;

            }
            finally
            {
                trace.Record(Annotations.ClientRecv());
                channel.ShutdownAsync();
            }
        }

        /// <summary>
        /// Invokes a client streaming call asynchronously.
        /// In client streaming scenario, client sends a stream of requests and server responds with a single response.
        /// </summary>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {

            var trace = this.ResolveTraceSpan().Child();

            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.ServiceName(_serviceName));
            trace.Record(Annotations.Rpc("grpc"));

            var channel = new Channel(_target, ChannelCredentials.Insecure);
            var call = CreateCall(channel, method, host, options, trace);
            try
            {
                var response = Calls.AsyncClientStreamingCall(call);
                return response;
            }
            finally
            {
                trace.Record(Annotations.ClientRecv());
                channel.ShutdownAsync();
            }
        }

        /// <summary>
        /// Invokes a duplex streaming call asynchronously.
        /// In duplex streaming scenario, client sends a stream of requests and server responds with a stream of responses.
        /// The response stream is completely independent and both side can be sending messages at the same time.
        /// </summary>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {

            var trace = this.ResolveTraceSpan().Child();

            trace.Record(Annotations.ClientSend());
            trace.Record(Annotations.ServiceName(_serviceName));
            trace.Record(Annotations.Rpc("grpc"));

            var channel = new Channel(_target, ChannelCredentials.Insecure);
            var call = CreateCall(channel, method, host, options, trace);
            try
            {
                var response = Calls.AsyncDuplexStreamingCall(call);
                return response;

            }
            finally
            {
                trace.Record(Annotations.ClientRecv());
                channel.ShutdownAsync();
            }
        }

        /// <summary>Creates call invocation details for given method.</summary>
        protected virtual CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(Channel channel, Method<TRequest, TResponse> method, string host, CallOptions options, Trace trace)
                where TRequest : class
                where TResponse : class
        {

            var headers = new Metadata();

            headers.Add(new Metadata.Entry("zipkin_traceid", trace.CurrentSpan.TraceId.ToString()));
            headers.Add(new Metadata.Entry("zipkin_parentspanid", trace.CurrentSpan.ParentSpanId.ToString()));
            headers.Add(new Metadata.Entry("zipkin_spanid", trace.CurrentSpan.SpanId.ToString()));

            options = new CallOptions(headers: headers);

            return new CallInvocationDetails<TRequest, TResponse>(channel, method, host, options);

        }

        private Trace ResolveTraceSpan()
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                var headers = _context.RequestHeaders.GetEnumerator();
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
                    return Trace.CreateFromId(new SpanState(long.Parse(dictionary["zipkin_traceid"]), long.Parse(dictionary["zipkin_parentspanid"]), long.Parse(dictionary["zipkin_spanid"]), SpanFlags.Sampled));
                }

                return Trace.CreateFromId(new SpanState(long.Parse(dictionary["zipkin_traceid"]), null, long.Parse(dictionary["zipkin_spanid"]), SpanFlags.Sampled));

            }
            catch
            {
                return null;
            }
        }
    }
}
