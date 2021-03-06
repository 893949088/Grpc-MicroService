﻿using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zipkin4net;

namespace Microsoft.AspNetCore.Zipkin.Internal
{
    internal class ZipkinTracingCallInvoker : CallInvoker
    {
        private readonly string _target;
        private readonly string _serviceName;

        public ZipkinTracingCallInvoker(string target, string serviceName)
        {
            _target = target;
            _serviceName = serviceName;
        }

        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            using (var clientTrace = new ClientTrace(_serviceName, "grpc"))
            {
                var trace = clientTrace.Trace;

                var channel = new Channel(_target, ChannelCredentials.Insecure);
                var call = CreateCall(channel, method, host, options, trace);
                try
                {
                    trace.Record(Annotations.Tag("grpc.host", _target));
                    trace.Record(Annotations.Tag("grpc.request", JsonConvert.SerializeObject(request)));
                    var response = Calls.BlockingUnaryCall(call, request);
                    trace.Record(Annotations.Tag("grpc.response", JsonConvert.SerializeObject(response)));
                    trace.Record(Annotations.Tag("warning", "warning"));
                    return response;
                }
                finally
                {
                    channel.ShutdownAsync();
                }
            }
        }

        /// <summary>
        /// Invokes a simple remote call asynchronously.
        /// </summary>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            using (var clientTrace = new ClientTrace(_serviceName, "grpc"))
            {
                var trace = clientTrace.Trace;

                var channel = new Channel(_target, ChannelCredentials.Insecure);
                var call = CreateCall(channel, method, host, options, trace);
                try
                {
                    trace.Record(Annotations.Tag("grpc.host", _target));
                    trace.Record(Annotations.Tag("grpc.request", JsonConvert.SerializeObject(request)));
                    var response = Calls.AsyncUnaryCall(call, request);
                    trace.Record(Annotations.Tag("grpc.response", JsonConvert.SerializeObject(response)));
                    return response;
                }
                finally
                {
                    channel.ShutdownAsync();
                }
            }
        }

        /// <summary>
        /// Invokes a server streaming call asynchronously.
        /// In server streaming scenario, client sends on request and server responds with a stream of responses.
        /// </summary>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {

            using (var clientTrace = new ClientTrace(_serviceName, "grpc"))
            {
                var trace = clientTrace.Trace;

                var channel = new Channel(_target, ChannelCredentials.Insecure);
                var call = CreateCall(channel, method, host, options, trace);
                try
                {
                    var response = Calls.AsyncServerStreamingCall(call, request);
                    return response;

                }
                finally
                {
                    channel.ShutdownAsync();
                }
            }
        }

        /// <summary>
        /// Invokes a client streaming call asynchronously.
        /// In client streaming scenario, client sends a stream of requests and server responds with a single response.
        /// </summary>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {

            using (var clientTrace = new ClientTrace(_serviceName, "grpc"))
            {
                var trace = clientTrace.Trace;

                var channel = new Channel(_target, ChannelCredentials.Insecure);
                var call = CreateCall(channel, method, host, options, trace);
                try
                {
                    var response = Calls.AsyncClientStreamingCall(call);
                    return response;
                }
                finally
                {
                    channel.ShutdownAsync();
                }
            }
        }

        /// <summary>
        /// Invokes a duplex streaming call asynchronously.
        /// In duplex streaming scenario, client sends a stream of requests and server responds with a stream of responses.
        /// The response stream is completely independent and both side can be sending messages at the same time.
        /// </summary>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {

            using (var clientTrace = new ClientTrace(_serviceName, "grpc"))
            {
                var trace = clientTrace.Trace;

                var channel = new Channel(_target, ChannelCredentials.Insecure);
                var call = CreateCall(channel, method, host, options,trace);
                try
                {
                    var response = Calls.AsyncDuplexStreamingCall(call);
                    return response;

                }
                finally
                {
                    channel.ShutdownAsync();
                }
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

            trace.Record(Annotations.Tag("grpc.method", method.Name));

            return new CallInvocationDetails<TRequest, TResponse>(channel, method, host, options);

        }
    }
}
